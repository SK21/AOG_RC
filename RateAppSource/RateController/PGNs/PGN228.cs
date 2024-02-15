namespace RateController
{
    public class PGN228
    {
        // VR Data
        // 0    header Hi       128 0x80
        // 1    header Lo       129 0x81
        // 2    source          127 0x7F
        // 3    AGIO PGN        228 0xE4
        // 4    length          8
        // 5    Channel 0
        // 6    Channel 1
        // 7    Channel 2
        // 8    Channel 3
        // 9    Channel 4
        // 10   Channel 5
        // 11   Channel 6
        // 12   Channel 7
        // 13   CRC

        private const byte cChannelCount = 8;
        private const byte HeaderCount = 5;
        private readonly FormStart mf;
        private double[] Channels;

        public PGN228(FormStart CalledFrom)
        {
            mf = CalledFrom;
            Channels = new double[cChannelCount];
        }

        public int ChannelCount
        { get { return cChannelCount; } }

        public void ParseByteData(byte[] Data)
        {
            if (Data.Length > HeaderCount)
            {
                if (Data.Length == Data[4] + HeaderCount + 1)
                {
                    if (mf.Tls.GoodCRC(Data, 2))
                    {
                        for (int i = 0; i < cChannelCount; i++)
                        {
                            Channels[i] = Data[i + 5];
                            if (Channels[i] > 100) Channels[i] = 100;
                        }
                    }
                }
            }
        }

        public double Percentage(byte ID)
        {
            double Result = 0;
            if (ID < cChannelCount) Result = Channels[ID];
            return Result;
        }
    }
}