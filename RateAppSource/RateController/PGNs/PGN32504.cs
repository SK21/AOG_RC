using System;

namespace RateController
{
    public class PGN32504
    {
        // PGN32504 module status - for debug, etc., from Arduino to RateController
        // 0	248
        // 1	126
        // 2    Controller ID
        // 3    InoID Lo
        // 4    InoID Hi
        // 5	Data 0 Lo
        // 6	Data 0 Hi
        // 7	Data 1 Lo
        // 8    Data 1 Hi
        // 9    Data 2 Lo
        // 10   Data 2 Hi
        // 11   Data 3
        // 12   Data 4
        // 13	CRC

        public UInt16[] StatusData = new UInt16[5];
        private const byte cByteCount = 14;
        private const byte HeaderHi = 126;
        private const byte HeaderLo = 248;
        private readonly FormStart mf;

        private UInt16[] cInoID = new UInt16[20];
        private byte[] cModuleID = new byte[20];
        private byte[] cSensorID = new byte[20];
        private DateTime LastRead;
        private byte ModuleCount;

        public PGN32504(FormStart CalledFrom)
        {
            mf = CalledFrom;
        }

        public UInt16 InoID
        {
            get
            {
                if (ModuleCount > 1)
                {
                    // return last module
                    return cInoID[ModuleCount - 1];
                }
                else
                {
                    return cInoID[0];
                }
            }
        }

        public byte ModuleID
        {
            get
            {
                if (ModuleCount > 1)
                {
                    // return last module
                    return cModuleID[ModuleCount - 1];
                }
                else
                {
                    return cModuleID[0];
                }
            }
        }

        public byte SensorID
        {
            get
            {
                if (ModuleCount > 1)
                {
                    // return last module
                    return cSensorID[ModuleCount - 1];
                }
                else
                {
                    return cSensorID[0];
                }
            }
        }

        public bool ParseByteData(byte[] Data)
        {
            bool Result = false;
            if (Data[1] == HeaderHi && Data[0] == HeaderLo &&
                Data.Length >= cByteCount && mf.Tls.GoodCRC(Data))
            {
                byte ID = mf.Tls.ParseModID(Data[2]);
                UpdateCount(ID);

                cModuleID[ModuleCount - 1] = ID;
                cSensorID[ModuleCount - 1] = mf.Tls.ParseSenID(Data[2]);
                cInoID[ModuleCount - 1] = (ushort)(Data[3] | Data[4] << 8);

                StatusData[0] = (ushort)(Data[5] | Data[6] << 8);
                StatusData[1] = (ushort)(Data[7] | Data[8] << 8);
                StatusData[2] = (ushort)(Data[9] | Data[10] << 8);
                StatusData[3] = Data[11];
                StatusData[4] = Data[12];

                Result = true;
            }
            return Result;
        }

        private void UpdateCount(byte ID)
        {
            if ((DateTime.Now - LastRead).TotalSeconds > 10)
            {
                ModuleCount = 1;
                cModuleID[0] = ID;
            }
            else
            {
                bool Found = false;
                for (int i = 0; i < ModuleCount; i++)
                {
                    if (cModuleID[i] == ID)
                    {
                        Found = true;
                        break;
                    }
                }
                if (!Found) cModuleID[ModuleCount++] = ID;
            }
            LastRead = DateTime.Now;
        }
    }
}