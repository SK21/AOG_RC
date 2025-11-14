using RateController.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RateController.PGNs
{
    public class PGN32504
    {
        // PGN32504, Wheel Speed sensor settings from RC to module
        //0     HeaderLo    248
        //1     HeaderHi    126
        //2     ModuleID    0-7
        //3     GPIO pin    0-50
        //4     Cal Lo
        //5     Cal Mid
        //6     Cal Hi
        //7     Commands
        //          - bit 0, erase counts
        //8     CRC

        private const byte cByteCount = 9;
        private byte[] cData = new byte[cByteCount];
        private bool cEraseCounts = false;
        private double cWheelCal;
        private int cWheelModule;
        private int cWheelPin;
        private FormStart mf;

        public PGN32504(FormStart CalledFrom)
        {
            mf = CalledFrom;
            LoadData();
        }

        public bool EraseCounts
        { set { cEraseCounts = value; } }

        public double WheelCal
        {
            get { return cWheelCal; }
            set
            {
                if (value >= 0 && value <= 0xffffff)
                {
                    cWheelCal = value;
                    Props.SetAppProp("WheelCal", cWheelCal.ToString());
                }
            }
        }

        public int WheelModule
        {
            get { return cWheelModule; }
            set
            {
                if (value >= 0 && value < Props.MaxModules)
                {
                    cWheelModule = value;
                    Props.SetAppProp("WheelModule", cWheelModule.ToString());
                }
            }
        }

        public int WheelPin
        {
            get { return cWheelPin; }
            set
            {
                if ((value >= 0 && value <= 50) || value == 255)
                {
                    cWheelPin = value;
                    Props.SetAppProp("WheelPin", cWheelPin.ToString());
                }
            }
        }

        public void Send()
        {
            Array.Clear(cData, 0, cByteCount);
            cData[0] = 248;
            cData[1] = 126;
            cData[2] = (byte)cWheelModule;

            if (Props.SpeedMode == SpeedType.Wheel)
            {
                cData[3] = (byte)cWheelPin;
            }
            else
            {
                cData[3] = 255; // disable wheel sensor
            }

            cData[4] = (byte)cWheelCal;
            cData[5] = (byte)((int)cWheelCal >> 8);
            cData[6] = (byte)((int)cWheelCal >> 16);

            cData[7] = 0;
            if (cEraseCounts) cData[7] = 1;

            cData[cByteCount - 1] = mf.Tls.CRC(cData, cByteCount - 1);
            mf.UDPmodules.SendUDPMessage(cData);
            cEraseCounts = false;
        }

        private void LoadData()
        {
            cWheelModule = int.TryParse(Props.GetAppProp("WheelModule"), out int wm) ? wm : 0;
            cWheelPin = int.TryParse(Props.GetAppProp("WheelPin"), out int wp) ? wp : 0;
            cWheelCal = double.TryParse(Props.GetAppProp("WheelCal"), out double wc) ? wc : 1;
        }
    }
}