using System;

namespace RateController
{
    public class PGN254
    {
        // AutoSteer Data
        // 0    header Hi       128 0x80
        // 1    header Lo       129 0x81
        // 2    source          126 0x7E
        // 3    AGIO PGN         254 0xFE
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

        private byte cRelayHi;
        private byte cRelayLo;
        private float cSpeed;
        private float KalG = 0.0F;
        private float KalP = 1.0F;
        private float KalPc = 0.0F;
        private float KalResult = 0.0F;
        private FormStart mf;

        private float KalVariance = 0.01F;   // larger is more filtering
        private float KalProcess = 0.005F;  // smaller is more filtering
        private DateTime ReceiveTime;

        private byte RelayHiLast;
        private byte RelayLoLast;
        private int totalHeaderByteCount = 5;

        public PGN254(FormStart CalledFrom)
        {
            mf = CalledFrom;
        }

        public event EventHandler<RelaysChangedArgs> RelaysChanged;

        public byte RelayHi
        { get { return cRelayHi; } }

        public byte RelayLo
        { get { return cRelayLo; } }

        public bool Connected()
        {
            return (DateTime.Now - ReceiveTime).TotalSeconds < 4;
        }

        public void ParseByteData(byte[] Data)
        {
            if (Data.Length > totalHeaderByteCount)
            {
                if (Data.Length == Data[4] + totalHeaderByteCount + 1)
                {
                    if (mf.Tls.GoodCRC(Data, 2))
                    {
                        cSpeed = (float)((Data[6] << 8 | Data[5]) / 10.0);

                        // Kalmen filter
                        KalPc = KalP + KalProcess;
                        KalG = KalPc / (KalPc + KalVariance);
                        KalP = (1 - KalG) * KalPc;
                        KalResult = KalG * (cSpeed - KalResult) + KalResult;
                        cSpeed = KalResult;

                        cRelayLo = Data[11];
                        cRelayHi = Data[12];

                        if (cRelayLo != RelayLoLast || cRelayHi != RelayHiLast)
                        {
                            // raise event
                            RelaysChangedArgs args = new RelaysChangedArgs();
                            args.RelayHi = cRelayHi;
                            args.RelayLo = cRelayLo;
                            RelaysChanged?.Invoke(this, args);

                            RelayHiLast = cRelayHi;
                            RelayLoLast = cRelayLo;
                        }

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

        public class RelaysChangedArgs : EventArgs
        {
            public byte RelayHi { get; set; }
            public byte RelayLo { get; set; }
        }
    }
}