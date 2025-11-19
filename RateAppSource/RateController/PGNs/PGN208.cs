using RateController.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.PGNs
{
    public class PGN208
    {
        // GPS data from AOG
        // 0        header Hi       128 0x80
        // 1        header Lo       129 0x81
        // 2        source          124 0x7C
        // 3        AGIO PGN        208 0xD0
        // 4        length          24
        // 5-12     longitude       double
        // 13-20    latitude        double
        // 21-28    speed, KMH      double
        // 29       CRC, 0

        public PGN100 GPS0; // for compatibility with AgOpenGPS
        private const byte Overhead = 6;
        private double cKMH;
        private double cLatitude;
        private double cLongitude;
        private FormStart mf;
        private DateTime ReceiveTime;
       

        public PGN208(FormStart CalledFrom)
        {
            mf = CalledFrom;
            GPS0 = new PGN100(mf);
        }

        public double Latitude
        {
            get
            {
                if (TWOLconnected())
                {
                    return cLatitude;
                }
                else
                {
                    return GPS0.Latitude;
                }
            }
        }

        public double Longitude
        {
            get
            {
                if (TWOLconnected())
                {
                    return cLongitude;
                }
                else
                {
                    return GPS0.Longitude;
                }
            }
        }

        public double Speed_KMH
        {
            get
            {
                if (TWOLconnected())
                {
                    return cKMH;
                }
                else
                {
                    return mf.AutoSteerPGN.Speed_KMH();
                }
            }
        }

        public bool TWOLconnected()
        {
            return (DateTime.Now - ReceiveTime).TotalSeconds < 4;
        }

        public bool Connected()
        {
            if(TWOLconnected())
            {
                return true;
            }
            else
            {
                return GPS0.Connected();
            }
        }

        public void ParseByteData(byte[] Data)
        {
            try
            {
                if ((Data.Length > Overhead) && (Data.Length == Data[4] + Overhead))
                {
                    if (mf.Tls.GoodCRC(Data, 2))
                    {
                        cLongitude = BitConverter.ToDouble(Data, 5);
                        cLatitude = BitConverter.ToDouble(Data, 13);
                        cKMH = (BitConverter.ToDouble(Data, 21) * 0.1) + (cKMH * 0.9);
                        ReceiveTime = DateTime.Now;
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("PGN208/ParseByteData: " + ex.ToString());
            }
        }
    }
}