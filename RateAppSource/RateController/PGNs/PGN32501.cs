
using System.Diagnostics;

namespace RateController
{
    public class PGN32501
    {
        // receive scale counts from arduino
        // 0    HeaderLo    245
        // 1    HeaderHi    126
        // 2    Mod/Sen ID  0-15/0-15
        // 3    weight byte 0
        // 4    Counts byte 1
        // 5    Counts byte 2
        // 6    Counts byte 3
        // 7    CRC

        private const byte cByteCount = 8;
        private const byte HeaderHi = 126;
        private const byte HeaderLo = 245;
        private readonly clsProduct Prod;
        private byte[] cData = new byte[cByteCount];
        private long cCounts;

        public PGN32501(clsProduct CalledFrom)
        {
            Prod = CalledFrom;
        }

        public long Counts
        {
            get { return cCounts; }
        }

        public bool ParseByteData(byte[] Data)
        {
            bool Result = false;
            if (Data[1] == HeaderHi && Data[0] == HeaderLo &&
                Data.Length >= cByteCount && Prod.mf.Tls.GoodCRC(Data))
            {
                int tmp = Prod.mf.Tls.ParseModID(Data[2]);
                if (Prod.ModuleID == tmp)
                {
                    tmp = Prod.mf.Tls.ParseSenID(Data[2]);
                    if (Prod.SensorID == tmp)
                    {
                        cCounts = Data[3] | Data[4] << 8 | Data[5] << 16 | Data[6] << 24;
                        Result = true;
                    }
                }
            }
            return Result;
        }
    }
}