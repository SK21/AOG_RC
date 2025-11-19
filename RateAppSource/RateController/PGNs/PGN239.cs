using RateController.Classes;
using System;

namespace RateController
{
    public class PGN239
    {
        // Machine Data
        // 0    header Hi       128
        // 1    header Lo       129
        // 2    source          126
        // 3    AGIO PGN        239
        // 4    length          8
        // 5    uturn
        // 6    speed*10
        // 7    hydLift
        // 8    tram
        // 9    GeoStop
        // 10   -
        // 11   Relay Lo
        // 12   Relay Hi
        // 13   CRC

        private readonly FormStart mf;
        private byte cGeoStop;
        private byte cHydLift;
        private byte cTram;
        private DateTime ReceiveTime;
        private int totalHeaderByteCount = 5;

        public PGN239(FormStart CalledFrom)
        {
            mf = CalledFrom;
        }

        public bool GeoStop
        {
            get
            {
                return (cGeoStop == 1) && Connected();
            }
        }

        public byte HydLift
        {
            get
            {
                byte Result = 0;
                if (Connected()) Result = cHydLift;
                return Result;
            }
        }

        public bool TramLeft
        {
            get
            {
                return mf.Tls.BitRead(cTram, 1) && Connected();
            }
        }

        public bool TramRight
        {
            get
            {
                return mf.Tls.BitRead(cTram, 0) && Connected();
            }
        }

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
                        cHydLift = Data[7];
                        cTram = Data[8];
                        cGeoStop = Data[9];
                        ReceiveTime = DateTime.Now;
                    }
                }
            }
        }
    }
}
