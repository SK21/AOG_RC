using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController
{
    public class PGN32503
    {
        // to Rate Controller from WifiAOG
        // 0	247
        // 1	126
        // 2	Module ID
        // 3	RSSI
        // 4	Status
        //		- bit 0 Wifi connected
        // 5	DebugVal1
        // 6	DebugVal2
        // 7	CRC

        private const byte cByteCount = 8;
        private const byte HeaderHi = 126;
        private const byte HeaderLo = 247;
        private FormStart mf;

        public struct ModuleData
        {
            public int RSSI;
            public byte Status;
            public byte DebugVal1;
            public byte DebugVal2;
        }
        ModuleData[] MDL;

        public PGN32503(FormStart CalledFrom)
        {
            mf = CalledFrom;
            MDL = new ModuleData[20];
        }

        public bool ParseByteData(byte[] Data)
        {
            bool Result = false;
            if (Data[1] == HeaderHi && Data[0] == HeaderLo &&
                Data.Length >= cByteCount && mf.Tls.GoodCRC(Data))
            {
                byte ID = Data[2];
                if (ID < 20)
                {
                    MDL[ID].RSSI = (Data[3] & 0b01111111) * -1;
                    MDL[ID].Status = Data[4];
                    MDL[ID].DebugVal1 = Data[5];
                    MDL[ID].DebugVal2 = Data[6];
                    Result = true;
                    Debug.Print(ID.ToString() + ", " + MDL[ID].RSSI.ToString() + ", "
                        + MDL[ID].Status.ToString() + ", "
                        + MDL[ID].DebugVal1.ToString() + " "
                        + MDL[ID].DebugVal2.ToString());
                }
            }
            return Result;
        }
    }
}
