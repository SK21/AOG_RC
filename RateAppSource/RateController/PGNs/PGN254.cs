using RateController.Classes;
using System;

namespace RateController
{
    public class PGN254
    {
        // AutoSteer Data
        // 0    header Hi       128 0x80
        // 1    header Lo       129 0x81
        // 2    source          127 0x7F
        // 3    AGIO PGN        254 0xFE
        // 4    length          8
        // 5    speed Lo - kmh X 10
        // 6    speed Hi
        // 7    status
        // 8    steer angle Lo
        // 9    steer angle Hi
        // 10   -
        // 11   Relay Lo
        // 12   Relay Hi
        // 13   CRC

        private readonly FormStart mf;
        private byte CRelayHi;
        private byte cRelayLo;
        private float cSpeed;
        private DateTime ReceiveTime;
        private byte RelayHiLast;
        private byte RelayLoLast;
        private int totalHeaderByteCount = 5;

        public PGN254(FormStart CalledFrom)
        {
            mf = CalledFrom;
        }

        public event EventHandler SectionsChanged;

        public byte RelayHi
        { get { return CRelayHi; } }

        public byte RelayLo
        { get { return cRelayLo; } }

        public bool Connected()
        {
            return (DateTime.Now - ReceiveTime).TotalSeconds < 4;
        }

        public void ParseByteData(byte[] Data)
        {
            if ((Props.SimMode == SimType.Sim_None) && (Data.Length > totalHeaderByteCount))
            {
                if (Data.Length == Data[4] + totalHeaderByteCount + 1)
                {
                    if (mf.Tls.GoodCRC(Data, 2))
                    {
                        cSpeed = (float)((Data[6] << 8 | Data[5]) / 10.0);
                        cRelayLo = Data[11];
                        CRelayHi = Data[12];
                        if (Changed()) SectionsChanged?.Invoke(this, EventArgs.Empty);

                        ReceiveTime = DateTime.Now;
                    }
                }
            }
        }

        public double Speed_KMH()
        {
            if (Connected())
            {
                return cSpeed;
            }
            else
            {
                return 0;
            }
        }

        private bool Changed()
        {
            bool Result = false;
            if (cRelayLo != RelayLoLast || CRelayHi != RelayHiLast)
            {
                Result = true;
                RelayLoLast = cRelayLo;
                RelayHiLast = CRelayHi;
            }
            return Result;
        }
    }
}