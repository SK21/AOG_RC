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
        // 3	dBm lo
        // 4	dBm Hi
        // 5	Status
        //		- bit 0 Wifi connected
        //		- bit 1 Teensy connected
        // 6	DebugVal1
        // 7	DebugVal2
        // 8	CRC

        private const byte cByteCount = 9;
        private const byte HeaderHi = 126;
        private const byte HeaderLo = 247;
        private FormStart mf;

        public struct ModuleData
        {
            public Int16 RSSI;
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
                    MDL[ID].RSSI = (short)(Data[3] | Data[4] << 8);
                    MDL[ID].Status = Data[5];
                    MDL[ID].DebugVal1 = Data[6];
                    MDL[ID].DebugVal2 = Data[7];
                    Result = true;


                    //Debug.Print(ID.ToString() + ", " + MDL[ID].RSSI.ToString()
                    //    + ", " + Data[5].ToString()
                    //    + ", " + Data[6].ToString()
                    //    + ", " + Data[7].ToString());
                }
            }
            return Result;
        }
    }
}
