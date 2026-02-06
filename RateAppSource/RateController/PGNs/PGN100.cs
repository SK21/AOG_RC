using RateController.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.PGNs
{
    public class PGN100
    {
        // data from AOG
        // corrected position
        // 0        header Hi       128 0x80
        // 1        header Lo       129 0x81
        // 2        source          127 0x7F
        // 3        AGIO PGN        100 0x64
        // 4        length          16
        // 5-12     longitude       double
        // 13-20    latitude        double
        // 21       CRC

        // corrected position alternate
        // 0        header Hi       128 0x80
        // 1        header Lo       129 0x81
        // 2        source          127 0x7F
        // 3        AGIO PGN        100 0x64
        // 4        length          24
        // 5-12     longitude       double
        // 13-20    latitude        double
        // 21-28    Fix2Fix         double
        // 29       CRC

        private const byte HeaderCount = 5;
        private double cFix2Fix;
        private double cLatitude;
        private double cLongitude;
        private bool ExtendedPGN = false;
        private DateTime ReceiveTime;

        public PGN100()
        {
            cFix2Fix = 1000;    // invalid data flag
        }

        public double Fix2FixHeading
        {
            get
            {
                if (Connected() && ExtendedPGN)
                {
                    return cFix2Fix;
                }
                else
                {
                    return 1000;
                }
            }
        }

        public double Latitude
        {
            get
            {
                if (Connected())
                {
                    return cLatitude;
                }
                else
                {
                    return 0;
                }
            }
        }

        public double Longitude
        {
            get
            {
                if (Connected())
                {
                    return cLongitude;
                }
                else
                {
                    return 0;
                }
            }
        }

        public bool Connected()
        {
            return (DateTime.Now - ReceiveTime).TotalSeconds < 4;
        }

        public void ParseByteData(byte[] Data)
        {
            try
            {
                if ((Data.Length > HeaderCount) && (Data.Length == Data[4] + HeaderCount + 1))
                {
                    ExtendedPGN = (Data[4] == 24);
                    if (Core.Tls.GoodCRC(Data, 2))
                    {
                        cLongitude = BitConverter.ToDouble(Data, 5);
                        cLatitude = BitConverter.ToDouble(Data, 13);
                        if (Data[4] == 24) cFix2Fix = BitConverter.ToDouble(Data, 21);  // alternate pgn
                        ReceiveTime = DateTime.Now;
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("PGN100/ParseByteData: " + ex.ToString());
            }
        }
    }
}