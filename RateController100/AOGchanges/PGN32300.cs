using System;
using System.Windows.Forms;

namespace AgOpenGPS
{
    public class PGN32300
    {
        //PGN In
        //0	header 0        126     0x7E
        //1	header 1         44     0x2C
        //2	autoBtnSetState	0 - no change, 1 - On, 2 - Off
        //3	manBtnSetOn Hi(16-9)
        //4	manBtnSetOn Lo(8-1)
        //5	manBtnSetAuto Hi
        //6	manBtnSetAuto Lo
        //7	manBtnSetOff Hi
        //8	manBtnSetOff Lo

        // total 9 bytes

        private const byte cByteCount = 9;
        private const byte HeaderHi = 126;
        private const byte HeaderLo = 44;

        private byte autoBtnSetState;
        public byte manBtnSetOnHi;
        public byte manBtnSetOnLo;
        private byte manBtnSetAutoHi;
        private byte manBtnSetAutoLo;
        private byte manBtnSetOffHi;
        private byte manBtnSetOffLo;

        int MaxSections;
        byte Bit;

        string ButtonName;
        Button SectionButton;

        private byte[] PO2 = new byte[] { 1, 2, 4, 8, 16, 32, 64, 128 }; // powers of 2

        private readonly FormGPS mf;

        public PGN32300(FormGPS CallingForm)
        {
            mf = CallingForm;
        }

        public void ParseByteData(byte[] Data)
        {
            bool NewData = false;
            if (Data[0] == HeaderHi & Data[1] == HeaderLo & Data.Length >= cByteCount)
            {
                autoBtnSetState = Data[2];
                manBtnSetOnHi = Data[3];
                manBtnSetOnLo = Data[4];
                manBtnSetAutoHi = Data[5];
                manBtnSetAutoLo = Data[6];
                manBtnSetOffHi = Data[7];
                manBtnSetOffLo = Data[8];

                NewData = true;
            }

            if (NewData)
            {
                // do remote switches

                // auto button
                switch (autoBtnSetState)
                {
                    case 1:
                        // turn on
                        mf.autoBtnState = FormGPS.btnStates.Off;
                        mf.btnSectionOffAutoOn.PerformClick();
                        break;

                    case 2:
                        // turn off
                        mf.autoBtnState = FormGPS.btnStates.Auto;
                        mf.btnSectionOffAutoOn.PerformClick();
                        break;
                }

                // section buttons
                for (int r = 0; r < 2; r++)
                {
                    MaxSections = mf.tool.numOfSections;
                    if (MaxSections > 7 + r * 8) MaxSections = 7 + r * 8;

                    for (int i = 0 + r * 8; i < MaxSections; i++)
                    {
                        Bit = PO2[i - r * 8];
                        if (r == 0)
                        {
                            // sections 0-7
                            if ((byte)(manBtnSetOnLo & Bit) == Bit)
                            {
                                // set section on
                                if (mf.section[i].manBtnState != FormGPS.manBtn.Auto) mf.section[i].manBtnState = FormGPS.manBtn.Auto;
                                ButtonName = "btnSection" + (i + 1).ToString() + "Man";
                                if (FindButton(ButtonName)) SectionButton.PerformClick();
                            }

                            if((byte)(manBtnSetOffLo&Bit)==Bit)
                            {
                                // set section off
                                if (mf.section[i].manBtnState != FormGPS.manBtn.On) mf.section[i].manBtnState = FormGPS.manBtn.On;
                                ButtonName = "btnSection" + (i + 1).ToString() + "Man";
                                if (FindButton(ButtonName)) SectionButton.PerformClick();
                            }

                            if((byte)(manBtnSetAutoLo&Bit)==Bit)
                            {
                                // set section auto
                                if (mf.section[i].manBtnState != FormGPS.manBtn.Off) mf.section[i].manBtnState = FormGPS.manBtn.Off;
                                ButtonName = "btnSection" + (i + 1).ToString() + "Man";
                                if (FindButton(ButtonName)) SectionButton.PerformClick();
                            }
                        }
                        else
                        {
                            // sections 8-15
                            if ((byte)(manBtnSetOnHi & Bit) == Bit)
                            {
                                // set section on
                                if (mf.section[i].manBtnState != FormGPS.manBtn.Auto) mf.section[i].manBtnState = FormGPS.manBtn.Auto;
                                ButtonName = "btnSection" + (i + 1).ToString() + "Man";
                                if (FindButton(ButtonName)) SectionButton.PerformClick();
                            }

                            if ((byte)(manBtnSetOffHi & Bit) == Bit)
                            {
                                // set section off
                                if (mf.section[i].manBtnState != FormGPS.manBtn.On) mf.section[i].manBtnState = FormGPS.manBtn.On;
                                ButtonName = "btnSection" + (i + 1).ToString() + "Man";
                                if (FindButton(ButtonName)) SectionButton.PerformClick();
                            }

                            if ((byte)(manBtnSetAutoHi & Bit) == Bit)
                            {
                                // set section auto
                                if (mf.section[i].manBtnState != FormGPS.manBtn.Off) mf.section[i].manBtnState = FormGPS.manBtn.Off;
                                ButtonName = "btnSection" + (i + 1).ToString() + "Man";
                                if (FindButton(ButtonName)) SectionButton.PerformClick();
                            }
                        }
                    }
                }
            }
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
