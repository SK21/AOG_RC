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
        //4     Cal Lo      actual X 1000
        //5     Cal Mid
        //6     Cal Hi
        //7     CRC

        private const byte cByteCount = 8;
        private byte[] cData = new byte[cByteCount];
        private double cWheelCal;
        private int cWheelModule;
        private int cWheelPin;
        private FormStart mf;

        public PGN32504(FormStart CalledFrom)
        {
            mf = CalledFrom;
            LoadData();
        }

        public double WheelCal
        {
            get { return cWheelCal; }
            set
            {
                if (value >= 0.01 && value <= 16700)
                {
                    cWheelCal = value;
                    Props.SetProp("WheelCal", cWheelCal.ToString());
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
                    Props.SetProp("WheelModule", cWheelModule.ToString());
                }
            }
        }

        public int WheelPin
        {
            get { return cWheelPin; }
            set
            {
                if (value >= 0 && value <= 50)
                {
                    cWheelPin = value;
                    Props.SetProp("WheelPin", cWheelPin.ToString());
                }
            }
        }

        public void Send()
        {
            Array.Clear(cData, 0, cByteCount);
            cData[0] = 248;
            cData[1] = 126;
            cData[2] = (byte)cWheelModule;
            cData[3] = (byte)cWheelPin;

            double Tmp = cWheelCal * 1000.0;
            cData[4] = (byte)Tmp;
            cData[5] = (byte)((int)Tmp >> 8);
            cData[6] = (byte)((int)Tmp >> 16);

            cData[cByteCount - 1] = mf.Tls.CRC(cData, cByteCount - 1);
            mf.UDPmodules.SendUDPMessage(cData);
        }

        private void LoadData()
        {
            cWheelModule = int.TryParse(Props.GetProp("WheelModule"), out int wm) ? wm : 0;
            cWheelPin = int.TryParse(Props.GetProp("WheelPin"), out int wp) ? wp : 0;
            cWheelCal = double.TryParse(Props.GetProp("WheelCal"), out double wc) ? wc : 1;
        }
    }
}