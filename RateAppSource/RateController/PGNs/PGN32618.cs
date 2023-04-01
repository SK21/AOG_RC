using System;
using System.Timers;

namespace RateController
{
    public enum SwIDs
    { Auto, MasterOn, MasterOff, RateUp, RateDown, sw0, sw1, sw2, sw3, sw4, sw5, sw6, sw7, sw8, sw9, sw10, sw11, sw12, sw13, sw14, sw15 };

    public class PGN32618
    {
        // to Rate Controller from arduino switch box
        // 0   106
        // 1   127
        // 2    - bit 0 Auto
        //      - bit 1 MasterOn
        //      - bit 2 MasterOff
        //      - bit 3 RateUp
        //      - bit 4 RateDown
        // 3    sw0 to sw7
        // 4    sw8 to sw15
        // 5    crc

        private const byte cByteCount = 6;
        private const byte HeaderHi = 127;
        private const byte HeaderLo = 106;
        private readonly FormStart mf;
        private bool cAutoOn = true;
        private bool cMasterOn = false;
        private bool cRateDown = false;
        private bool cRateUp = false;
        private byte[] DataLast;
        private bool DownPressed;
        private bool MasterPressed;
        private byte[] PressedData;
        private DateTime ReceiveTime;
        private System.Timers.Timer ReleaseTimer;
        private bool[] SW = new bool[21];
        private bool[] SWlast = new bool[21];
        private bool UpPressed;
        private int ReleaseCount;

        public PGN32618(FormStart CalledFrom)
        {
            mf = CalledFrom;
            SW[(int)SwIDs.Auto] = true; // default to auto in case of no switchbox
            DataLast = new byte[cByteCount];
            PressedData = new byte[cByteCount];

            ReleaseTimer = new System.Timers.Timer();
            ReleaseTimer.Elapsed += ReleaseTimer_Elapsed;
            ReleaseTimer.Interval = 500;
        }

        public event EventHandler<SwitchPGNargs> SwitchPGNreceived;

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

        public bool[] Switches
        { get { return SW; } }

        public bool Connected()
        {
            return ((DateTime.Now - ReceiveTime).TotalSeconds < 4);
        }

        public bool ParseByteData(byte[] Data)
        {
            bool Result = false;

            if (Data[0] == HeaderLo && Data[1] == HeaderHi && Data.Length >= cByteCount && mf.Tls.GoodCRC(Data))
            {
                // auto
                SW[0] = mf.Tls.BitRead(Data[2], 0);
                cAutoOn = SW[0];

                // master
                SW[1] = mf.Tls.BitRead(Data[2], 1);     // master on
                SW[2] = mf.Tls.BitRead(Data[2], 2);     // master off

                if (SW[1]) cMasterOn = true;
                else if (SW[2]) cMasterOn = false;

                // rate
                SW[3] = mf.Tls.BitRead(Data[2], 3);     // rate up
                SW[4] = mf.Tls.BitRead(Data[2], 4);     // rate down

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

                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        SW[5 + j + i * 8] = mf.Tls.BitRead(Data[i + 3], j);
                    }
                }

                DataLast = Data;

                SwitchPGNargs args = new SwitchPGNargs();
                args.Switches = SW;
                SwitchPGNreceived?.Invoke(this, args);

                ReceiveTime = DateTime.Now;
                Result = true;
            }
            return Result;
        }

        public bool ParseStringData(string[] Data)
        {
            bool Result = false;
            byte[] Dt = new byte[cByteCount];

            if (Data.Length >= cByteCount && mf.Tls.GoodCRC(Data))
            {
                for (int i = 0; i < cByteCount; i++)
                {
                    Dt[i] = 0;
                    Result = byte.TryParse(Data[i], out Dt[i]);
                }
            }
            if (Result) Result = ParseByteData(Dt);

            return Result;
        }

        public void PressSwitch(SwIDs ID)
        {
            PressedData = DataLast;
            PressedData[0] = HeaderLo;
            PressedData[1] = HeaderHi;
            switch (ID)
            {
                case SwIDs.Auto:
                    if (cAutoOn)
                    {
                        // turn off
                        PressedData[2] = mf.Tls.BitClear(PressedData[2], 0);
                    }
                    else
                    {
                        // turn on
                        PressedData[2] = mf.Tls.BitSet(PressedData[2], 0);
                    }
                    break;

                case SwIDs.MasterOn:
                    if (!MasterPressed)
                    {
                        PressedData[2] = mf.Tls.BitSet(PressedData[2], 1);
                        PressedData[2] = mf.Tls.BitClear(PressedData[2], 2);
                    }

                    MasterPressed = true;
                    ReleaseTimer.Enabled = true;

                    break;

                case SwIDs.MasterOff:
                    if (!MasterPressed)
                    {
                        PressedData[2] = mf.Tls.BitClear(PressedData[2], 1);
                        PressedData[2] = mf.Tls.BitSet(PressedData[2], 2);
                    }

                    MasterPressed = true;
                    ReleaseTimer.Enabled = true;

                    break;

                case SwIDs.RateUp:
                    PressedData[2] = mf.Tls.BitSet(PressedData[2], 3);
                    PressedData[2] = mf.Tls.BitClear(PressedData[2], 4);

                    UpPressed = true;
                    ReleaseTimer.Enabled = true;

                    break;

                case SwIDs.RateDown:
                    PressedData[2] = mf.Tls.BitClear(PressedData[2], 3);
                    PressedData[2] = mf.Tls.BitSet(PressedData[2], 4);

                    DownPressed = true;
                    ReleaseTimer.Enabled = true;

                    break;

                default:
                    int Num = (int)ID - 5;

                    if (Num < 8)
                    {
                        if (SW[(int)ID])
                        {
                            // turn off, lo
                            PressedData[3] = mf.Tls.BitClear(PressedData[3], Num);
                        }
                        else
                        {
                            // turn on, lo
                            PressedData[3] = mf.Tls.BitSet(PressedData[3], Num);
                        }
                    }
                    else
                    {
                        if (SW[(int)ID])
                        {
                            // turn off, hi
                            PressedData[4] = mf.Tls.BitClear(PressedData[4], Num - 8);
                        }
                        else
                        {
                            // turn on, hi
                            PressedData[4] = mf.Tls.BitSet(PressedData[4], Num - 8);
                        }
                    }
                    break;
            }

            PressedData[5] = mf.Tls.CRC(PressedData, 4);
            ParseByteData(PressedData);
        }

        public void ReleaseSwitch(SwIDs ID)
        {
            // for momentary switches Rate Up, Rate Down, Master
            switch (ID)
            {
                case SwIDs.MasterOn:
                case SwIDs.MasterOff:
                    MasterPressed = false;
                    break;

                case SwIDs.RateUp:
                    UpPressed = false;
                    break;

                case SwIDs.RateDown:
                    DownPressed = false;
                    break;
            }
        }

        public bool SwitchOn(SwIDs ID)
        {
            return SW[(int)ID];
        }

        private void ReleaseTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ReleaseCount++; // check for no ReleaseSwitch statement, max 2 seconds

            if ((!UpPressed && !DownPressed && !MasterPressed) || ReleaseCount > 4)
            {
                // release momentary buttons
                if (cAutoOn)
                {
                    PressedData[2] = 1;
                }
                else
                {
                    PressedData[2] = 0;
                }

                PressedData[5] = mf.Tls.CRC(PressedData, 4);
                ParseByteData(PressedData);

                ReleaseTimer.Enabled = false;
                ReleaseCount = 0;
            }
        }

        public class SwitchPGNargs : EventArgs
        {
            public bool[] Switches { get; set; }
        }
    }
}