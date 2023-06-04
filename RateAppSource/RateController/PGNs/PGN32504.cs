using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController
{
    public class PGN32504
    {
        // PGN32504 module status - for debug, etc., from Arduino to RateController
        // 0	248
        // 1	126
        // 2    Controller ID
        // 3	Data 0 Lo
        // 4	Data 0 Hi
        // 5	Data 1 Lo
        // 6    Data 1 Hi
        // 7    Data 2 Lo
        // 8    Data 2 Hi
        // 9    Data 3
        // 10   Data 4
        // 11   InoID Lo
        // 12   InoID Hi
        // 13	CRC

        private const byte cByteCount = 14;
        private const byte HeaderHi = 126;
        private const byte HeaderLo = 248;
        private readonly FormStart mf;

        public UInt16[] StatusData = new UInt16[6];
        public byte ModuleID;
        public byte SensorID;

        public PGN32504(FormStart CalledFrom)
        {
            mf = CalledFrom;
        }

        public bool ParseByteData(byte[] Data)
        {
            bool Result = false;
            if (Data[1] == HeaderHi && Data[0] == HeaderLo &&
                Data.Length >= cByteCount && mf.Tls.GoodCRC(Data))
            {
                ModuleID = mf.Tls.ParseModID(Data[2]);
                SensorID = mf.Tls.ParseSenID(Data[2]);

                StatusData[0] = (ushort)(Data[3] | Data[4] << 8);
                StatusData[1] = (ushort)(Data[5] | Data[6] << 8);
                StatusData[2] = (ushort)(Data[7] | Data[8] << 8);
                StatusData[3] = Data[9];
                StatusData[4] = Data[10];
                StatusData[5] = (ushort)(Data[11] | Data[12] << 8);

                Result = true;
            }
            return Result;
        }
    }
}
