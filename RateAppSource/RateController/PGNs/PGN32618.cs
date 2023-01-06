using System;

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
        private DateTime ReceiveTime;
        private bool[] SW = new bool[21];

        public PGN32618(FormStart CalledFrom)
        {
            mf = CalledFrom;
            SW[(int)SwIDs.Auto] = true; // default to auto in case of no switchbox
        }

        public event EventHandler<SwitchPGNargs> SwitchPGNreceived;

        public class SwitchPGNargs : EventArgs
        {
            public bool[] Switches { get; set; }
        }

        public bool Connected()
        {
            return ((DateTime.Now - ReceiveTime).TotalSeconds < 4);
        }

        public bool ParseByteData(byte[] Data)
        {
            bool Result = false;

            if (Data[0] == HeaderLo && Data[1] == HeaderHi && Data.Length >= cByteCount && mf.Tls.GoodCRC(Data))
            {
                SW[0] = mf.Tls.BitRead(Data[2], 0);     // auto on
                SW[1] = mf.Tls.BitRead(Data[2], 1);     // master on
                SW[2] = mf.Tls.BitRead(Data[2], 2);     // master off
                SW[3] = mf.Tls.BitRead(Data[2], 3);     // rate up
                SW[4] = mf.Tls.BitRead(Data[2], 4);     // rate down

                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        SW[5 + j + i * 8] = mf.Tls.BitRead(Data[i + 3], j);
                    }
                }

                ReceiveTime = DateTime.Now;
                Result = true;
            }
            if (Result)
            {
                SwitchPGNargs args = new SwitchPGNargs();
                args.Switches = SW;
                SwitchPGNreceived?.Invoke(this, args);
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

        public bool SectionSwitchOn(int ID)
        {
            bool Result = false;
            if ((ID >= 0) && (ID <= 15))
            {
                Result = SW[ID + (int)SwIDs.sw0];
            }
            return Result;
        }

        public bool SwitchOn(SwIDs ID)
        {
            return SW[(int)ID];
        }

        public void PressSwitch(SwIDs ID)
        {
            ReceiveTime = DateTime.Now;
            
            switch (ID)
            {
                case SwIDs.MasterOn:
                    SW[1] = true;
                    SW[2] = false;
                    break;
                case SwIDs.MasterOff:
                    SW[1] = false;
                    SW[2] = true;
                    break;
                case SwIDs.RateUp:
                    SW[3] = true;
                    SW[4] = false;
                    break;
                case SwIDs.RateDown:
                    SW[3] = false;
                    SW[4] = true;
                    break;
                default:
                    SW[(int)ID] = !SW[(int)ID];
                    break;
            }
            SwitchPGNargs args = new SwitchPGNargs();
            args.Switches = SW;
            SwitchPGNreceived?.Invoke(this, args);
        }
    }
}