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

        private readonly float KalProcess = 0.005F;
        private readonly float KalVariance = 0.01F;
        private readonly FormStart mf;
        private float cSpeed;
        private float KalG = 0.0F;
        private float KalP = 1.0F;
        private float KalPc = 0.0F;
        private float KalResult = 0.0F;
        private DateTime ReceiveTime;
        private int totalHeaderByteCount = 5;

        public PGN254(FormStart CalledFrom)
        {
            mf = CalledFrom;
        }

        public bool Connected()
        {
            return (DateTime.Now - ReceiveTime).TotalSeconds < 4;
        }

        public void ParseByteData(byte[] Data)
        {
            if ((mf.SimMode == SimType.None) && (Data.Length > totalHeaderByteCount))
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
    }
}