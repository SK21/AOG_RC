using RateController.Classes;
using System;

namespace RateController
{
    public class PGN32618
    {
        // to Rate Controller from arduino switch box
        // 0   106
        // 1   127
        // 2    - bit 0 -
        //      - bit 1 MasterOn
        //      - bit 2 MasterOff
        //      - bit 3 RateUp
        //      - bit 4 RateDown
        //      - bit 5 AutoSection
        //      - bit 6 AutoRate
        //		- bit 7 Work switch
        // 3    sw0 to sw7
        // 4    sw8 to sw15
        // 5    crc

        private const byte cByteCount = 6;
        private const byte HeaderHi = 127;
        private const byte HeaderLo = 106;
        private readonly FormStart mf;
        private bool cAutoRateDisabled = false;
        private bool cMasterOn = false;
        private bool cRateDown = false;
        private bool cRateUp = false;
        private bool cUseWorkSwitch = false;
        private DateTime ReceivedReal;
        private DateTime ReceiveTime;
        private bool[] SW = new bool[24];

        public PGN32618(FormStart CalledFrom)
        {
            mf = CalledFrom;
            SW[(int)SwIDs.AutoSection] = true; // default to auto in case of no switchbox
            SW[(int)SwIDs.AutoRate] = true;

            if (bool.TryParse(mf.Tls.LoadProperty("UseWorkSwitch"), out bool uw)) cUseWorkSwitch = uw;
            if (bool.TryParse(mf.Tls.LoadProperty("SwitchboxAutoRateDisabled"), out bool auto)) cAutoRateDisabled = auto;
        }

        public event EventHandler SwitchPGNreceived;

        public bool AutoRateDisabled
        {
            get { return cAutoRateDisabled; }
            set
            {
                cAutoRateDisabled = value;
                mf.Tls.SaveProperty("SwitchboxAutoRateDisabled", cAutoRateDisabled.ToString());
            }
        }

        public bool AutoRateOn
        {
            get
            {
                bool Result = false;
                if (!AutoRateDisabled) Result = SW[(int)SwIDs.AutoRate];
                return Result;
            }
        }

        public bool AutoSectionOn
        {
            get { return SW[(int)SwIDs.AutoSection]; }
        }

        public bool MasterOn
        {
            get { return cMasterOn; }
            set { cMasterOn = value; }
        }

        public bool RateDown
        {
            get { return cRateDown; }
            set { cRateDown = value; }
        }

        public bool RateUp
        {
            get { return cRateUp; }
            set { cRateUp = value; }
        }

        public bool UseWorkSwitch
        {
            get { return cUseWorkSwitch; }
            set
            {
                cUseWorkSwitch = value;
                mf.Tls.SaveProperty("UseWorkSwitch", cUseWorkSwitch.ToString());
            }
        }

        public bool WorkOn
        {
            // returns true if switchbox work switch is on or any
            // module shows workswitch on
            get
            {
                bool Result = true;
                if (cUseWorkSwitch) Result = SW[23] || mf.ModulesStatus.WorkSwitchOn();
                return Result;
            }
        }

        public bool Connected()
        {
            return (DateTime.Now - ReceiveTime).TotalSeconds < 4;
        }

        public bool ParseByteData(byte[] Data, bool RealSwitchBox = false)
        {
            bool Result = false;

            if (Data[0] == HeaderLo && Data[1] == HeaderHi && Data.Length >= cByteCount && mf.Tls.GoodCRC(Data))
            {
                if (RealSwitchBox) ReceivedReal = DateTime.Now;

                // master
                SW[1] = mf.Tls.BitRead(Data[2], 1);     // master on
                SW[2] = mf.Tls.BitRead(Data[2], 2);     // master off

                if (SW[1]) cMasterOn = true;
                else if (SW[2]) cMasterOn = false;

                SW[3] = mf.Tls.BitRead(Data[2], 3);     // rate up
                SW[4] = mf.Tls.BitRead(Data[2], 4);     // rate down

                SW[21] = mf.Tls.BitRead(Data[2], 5);    // auto section
                SW[22] = mf.Tls.BitRead(Data[2], 6);    // auto rate
                SW[23] = mf.Tls.BitRead(Data[2], 7);    // work switch

                if (SW[3])
                {
                    cRateUp = true;
                    cRateDown = false;
                }
                else if (SW[4])
                {
                    cRateUp = false;
                    cRateDown = true;
                }
                else if (!SW[3] && !SW[4])
                {
                    cRateUp = false;
                    cRateDown = false;
                }

                // section switches
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        SW[5 + j + i * 8] = mf.Tls.BitRead(Data[i + 3], j);
                    }
                }

                SwitchPGNreceived?.Invoke(this, EventArgs.Empty);

                ReceiveTime = DateTime.Now;
                Result = true;
            }
            return Result;
        }

        public bool ParseStringData(string[] Data)
        {
            bool Result = false;
            byte[] BD;
            if (Data.Length < 100)
            {
                BD = new byte[Data.Length];
                for (int i = 0; i < Data.Length; i++)
                {
                    byte.TryParse(Data[i], out BD[i]);
                }
                Result = ParseByteData(BD);
            }
            return Result;
        }

        public bool RealConnected()
        {
            return (DateTime.Now - ReceivedReal).TotalSeconds < 4;
        }

        public bool SectionSwitchOn(int ID)
        {
            bool Result = false;
            if ((ID >= 0) && (ID <= 15))
            {
                Result = SW[ID + (int)SwIDs.sw0];
            }
            return Result;
        }

        public bool SwitchIsOn(SwIDs ID)
        {
            return SW[(int)ID];
        }
    }
}