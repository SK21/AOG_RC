namespace RateController
{
    public class PGN230
    {
        // vr data to RC
        // 0    128
        // 1    129
        // 2    source  0
        // 3    AGIO PGN 0xE6(230)
        // 4    length 10
        // 5    rate 0 Lo
        // 6    rate 0 Hi
        // 7    rate 1 Lo
        // 8    rate 1 Hi
        // 9    rate 2 Lo
        // 10   rate 2 Hi
        // 11   rate 3 Lo
        // 12   rate 3 Hi
        // 13   rate 4 Lo
        // 14   rate 4 Hi
        // 15   CRC

        private byte[] cRate = new byte[10];
        private int Length = 16;

        public PGN230()
        {
            for (int i = 0; i < 5; i++)
            {
                // set 2 bytes to 255 X 100 (0x639C) - no data
                cRate[i * 2 + 1] = 0x63;
                cRate[i * 2] = 0x9c;
            }
        }

        public void ParseByteData(byte[] Data)
        {
            if (Data.Length == Length)
            {
                if (GoodCRC(Data))
                {
                    for (int i = 0; i < 10; i++)
                    {
                        cRate[i] = Data[i + 5];
                    }
                }
            }
        }

        public double Rate(byte RateID)
        {
            double Result = 255;
            if (RateID < 5)
            {
                Result = (cRate[RateID * 2 + 1] << 8 | cRate[RateID * 2]) / 100.0;
            }
            return Result;
        }

        private bool GoodCRC(byte[] Data)
        {
            int CK = 0;
            for (int i = 2; i < Data.Length - 1; i++)
            {
                CK += Data[i];
            }
            return (byte)CK == Data[Data.Length - 1];
        }
    }
}