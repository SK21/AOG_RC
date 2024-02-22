using System;
using System.Diagnostics;

namespace RateController
{
    public class PGN32401
    {
        //PGN32401, module, analog info from module to RC
        //0     145
        //1     126
        //2     module ID
        //3     analog 0, Lo
        //4     analog 0, Hi
        //5     analog 1, Lo
        //6     analog 1, Hi
        //7     analog 2, Lo
        //8     analog 2, Hi
        //9     analog 3, Lo
        //10    analog 3, Hi
        //11    InoID lo
        //12    InoID hi
        //13    status
        //14    CRC

        private const byte cByteCount = 15;
        private const byte HeaderHi = 126;
        private const byte HeaderLo = 145;
        private readonly FormStart mf;
        private UInt16[] cInoID;
        private byte cModuleID;
        private UInt16[,] cReading = new UInt16[255, 4];

        public PGN32401(FormStart CalledFrom)
        {
            mf = CalledFrom;
            cInoID = new ushort[mf.MaxModules];
        }

        public UInt16 InoID(int Module)
        {
            return cInoID[Module];
        }

        public bool ParseByteData(byte[] Data)
        {
            bool Result = false;
            if (Data[1] == HeaderHi && Data[0] == HeaderLo && Data.Length >= cByteCount && mf.Tls.GoodCRC(Data))
            {
                cModuleID = Data[2];
                for (int i = 0; i < 4; i++)
                {
                    cReading[cModuleID, i] = (UInt16)(Data[i * 2 + 4] << 8 | Data[i * 2 + 3]);
                }
                cInoID[cModuleID] = (ushort)(Data[11] | Data[12] << 8);

                mf.UpdateModuleConnected(cModuleID);

                Result = true;
            }
            return Result;
        }

        public bool ParseStringData(string[] Data)
        {
            bool Result = false;
            byte[] BD;
            if (Data.Length < 100)
            {
                BD = new byte[Data.Length];
                for (int i = 0; i < Data.Length; i++)
                {
                    byte.TryParse(Data[i], out BD[i]);
                }
                Result = ParseByteData(BD);
            }
            return Result;
        }

        public UInt16 Reading(byte ModuleID, byte SensorID)
        {
            if (SensorID < 4 && ModuleID < 255)
            {
                return cReading[ModuleID, SensorID];
            }
            else
            {
                return 0;
            }
        }
    }
}