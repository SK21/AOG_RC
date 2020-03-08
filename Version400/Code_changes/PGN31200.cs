using System;
using System.Windows.Forms;

namespace AgOpenGPS
{
    class PGN31200
    {
        //  Comm data from arduino to AgOpenGPS
        // PGN 31200 (B)
        //	0. HeaderHi			121		0x79
        //	1. HeaderLo			224		0xE0
        //  2. Relay Hi			8-15
        //	3. Relay Lo			0-7
        //	4. SecSwOff[1] Hi	sections 8 to 15
        //	5. SecSwOff[0] Lo	sections 0 to 7
        //	6. Command		    command byte out to AOG
        //			- bit 0		- AOG section buttons auto (xxxxxxx1)
        //			- bit 1		- AOG section buttons auto off (xxxxxx1x)
        //			- bits 2,3	- change rate steps 0 (xxxx00xx) no change, 1 (xxxx01xx), 2 (xxxx10xx), 3 (xxxx11xx)
        //			- bit 4		- 0 change rate left (xxx0xxxx), 1 change rate right (xxx1xxxx)
        //			- bit 5		- 0 rate down (xx0xxxxx), 1 rate up (xx1xxxxx)
        // total 7 bytes
        // UDP port 6100

        private const byte cByteCount = 7;
        private const byte HeaderHi = 121;
        private const byte HeaderLo = 224;

        public byte RelayHi;
        public byte RelayLo;
        public byte SecSwOffHi;
        public byte SecSwOffLo;
        public byte Command;

        private int Temp;
        private byte CommandOld;
        private byte OffLoOld;
        private byte OffHiOld;
        private double cRateSet;
        int MaxSections;
        byte Bit;
        string ButtonName;
        Button SectionButton;
        byte DataByte;
        byte DataByteOld;

        float RateCalcFactor = 1.03F;   // rate change amount for each step.  ex: 1.10 means 10% for each step

        private byte[] PO2 = new byte[]{ 1, 2, 4, 8, 16, 32, 64, 128 }; // powers of 2

        private readonly FormGPS mf;

        public PGN31200(FormGPS CallingForm)
        {
            mf = CallingForm;
        }

        public double RateSet { get { return cRateSet; } set { cRateSet = value; } }

        public bool ParseStringData(string[] Data)
        {
            bool Result = false;
            if (Data.Length >= cByteCount)
            {
                int.TryParse(Data[0], out Temp);
                if (Temp == HeaderHi)
                {
                    int.TryParse(Data[1], out Temp);
                    if (Temp == HeaderLo)
                    {
                        // relay bytes
                        byte.TryParse(Data[2], out RelayHi);
                        byte.TryParse(Data[3], out RelayLo);

                        // section off bytes
                        byte.TryParse(Data[4], out SecSwOffHi);
                        byte.TryParse(Data[5], out SecSwOffLo);

                        // command byte
                        byte.TryParse(Data[6], out Command);

                        Result = true;
                    }
                }
            }
            return Result;
        }

        public bool ParseByteData(byte[] Data)
        {
            bool Result = false;
            if (Data[0] == HeaderHi & Data[1] == HeaderLo & Data.Length >= cByteCount)
            {
                RelayHi = Data[2];
                RelayLo = Data[3];
                SecSwOffHi = Data[4];
                SecSwOffLo = Data[5];
                Command = Data[6];

                Result = true;
            }
            return Result;
        }

        public double DoSwitches(double CurrentRate)
        {
            // adapted from Matthias's Switch routine

            // rate change amount
            int RateSteps = 0;
            if ((Command & 4) == 4) RateSteps = 1;
            if ((Command & 8) == 8) RateSteps += 2;

            if (RateSteps > 0)
            {
                // rate direction
                bool RateUp = false;
                RateUp = ((Command & 32) == 32);

                // change rate
                float ChangeAmount = 1;
                for (byte a = 1; a <= RateSteps; a++)
                {
                    ChangeAmount *= RateCalcFactor;
                }
                if (RateUp)
                {
                    CurrentRate = Math.Round((CurrentRate * ChangeAmount), 1);
                }
                else
                {
                    CurrentRate = Math.Round((CurrentRate / ChangeAmount), 1);
                }
            }


            // master switch changed?
            if (Command != CommandOld)
            {
                // Master switch On
                if ((Command & 1) == 1)
                {
                    // set button off and then press it ON
                    mf.autoBtnState = FormGPS.btnStates.Off;
                    mf.btnSectionOffAutoOn.PerformClick();
                }

                // Master switch Off
                if ((Command & 2) == 2)
                {
                    // set button on and then press it Off
                    mf.autoBtnState = FormGPS.btnStates.Auto;
                    mf.btnSectionOffAutoOn.PerformClick();
                }
            }
            CommandOld = Command;


            // Relay ON signal
            for (int r = 0; r < 2; r++)
            {
                if (r == 0)
                {
                    DataByte = RelayLo;
                }
                else
                {
                    DataByte = RelayHi;
                }

                if (DataByte != 0)
                {
                    MaxSections = mf.tool.numOfSections;
                    if (MaxSections > 8 + r * 8) MaxSections = 8 + r * 8;

                    for (int i = 0 + r * 8; i < MaxSections; i++)
                    {
                        Bit = PO2[i - r * 8];
                        if ((DataByte & Bit) == Bit)
                        {
                            if (mf.section[i].manBtnState != FormGPS.manBtn.Auto) mf.section[i].manBtnState = FormGPS.manBtn.Auto;
                            ButtonName = "btnSection" + (i + 1).ToString() + "Man";
                            if (FindButton(ButtonName)) SectionButton.PerformClick();
                        }

                    }
                }
            }


            // switch off changed?
            for (int r = 0; r < 2; r++)
            {
                if (r == 0)
                {
                    DataByte = SecSwOffLo;
                    DataByteOld = OffLoOld;
                }
                else
                {
                    DataByte = SecSwOffHi;
                    DataByteOld = OffHiOld;
                }

                if (DataByte != DataByteOld)
                {
                    //if Main = Auto then change section to Auto if Off signal from Arduino stopped
                    if (mf.autoBtnState == FormGPS.btnStates.Auto)
                    {
                        MaxSections = mf.tool.numOfSections;
                        if (MaxSections > 8 + r * 8) MaxSections = 8 + r * 8;

                        for (int i = 0 + r * 8; i < MaxSections; i++)
                        {
                            Bit = PO2[i - r * 8];
                            if (((DataByteOld & Bit) == Bit) & ((DataByte & Bit) != Bit) & (mf.section[i].manBtnState == FormGPS.manBtn.Off))
                            {
                                ButtonName = "btnSection" + (i + 1).ToString() + "Man";
                                if (FindButton(ButtonName)) SectionButton.PerformClick();
                            }
                        }

                    }
                }
            }
            OffLoOld = SecSwOffLo;
            OffHiOld = SecSwOffHi;


            // OFF signal from arduino
            for (int r = 0; r < 2; r++)
            {
                if (r == 0)
                {
                    DataByte = SecSwOffLo;
                }
                else
                {
                    DataByte = SecSwOffHi;
                }

                //if section SW in Arduino is switched to OFF; check always, if switch is locked to off GUI should not change
                MaxSections = mf.tool.numOfSections;
                if (MaxSections > 8 + r * 8) MaxSections = 8 + r * 8;

                for (int i = 0 + r * 8; i < MaxSections; i++)
                {
                    Bit = PO2[i - r * 8];
                    if (((DataByte & Bit) == Bit) & (mf.section[i].manBtnState != FormGPS.manBtn.Off))
                    {
                        mf.section[i].manBtnState = FormGPS.manBtn.On;
                        ButtonName = "btnSection" + (i + 1).ToString() + "Man";
                        if (FindButton(ButtonName)) SectionButton.PerformClick();
                    }
                }
            }
            return CurrentRate;
        }

        private bool FindButton(string Name)
        {
            bool Result = false;
            foreach (Control c in mf.Controls)
            {
                if (c.Name == Name)
                {
                    SectionButton = (Button)c;
                    Result = true;
                }
            }
            return Result;
        }
    }
}

