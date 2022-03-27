using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController
{
    public class PGN32622
    {
        // PCB config 1
        //0     HeaderLo    110
        //1     HeaderHi    127
        //2     Receiver    0-None, 1-SimpleRTK2B, 2-Sparkfun F9P
        //3     IMU         0-None, 1-Sparkfun BNO, 2-CMPS14, 3-Adafruit BNO
        //4     Read Delay  
        //5     Report Interval
        //6     Zero offset Lo
        //7     Zero offset Hi
        //8     Minimum speed
        //9     Maximum speed
        //10    Speed pulse cal X 10 Lo
        //11    Speed pulse cal X 10 Hi
        //12    Restart module

        private frmPCBsettings cf;
        private byte[] cData = new byte[13];

        public PGN32622(frmPCBsettings CalledFrom)
        {
            cf = CalledFrom;
            cData[0] = 110;
            cData[1] = 127;
        }

        public void Send(bool RestartModule = false)
        {
            byte tmp;
            double val;

            byte.TryParse(cf.mf.Tls.LoadProperty("GPSreceiver"), out tmp);
            cData[2] = tmp;

            byte.TryParse(cf.mf.Tls.LoadProperty("IMU"), out tmp);
            cData[3] = tmp;

            double.TryParse(cf.mf.Tls.LoadProperty(cf.CFG[0].Name), out val);
            cData[4] = (byte)val;

            double.TryParse(cf.mf.Tls.LoadProperty(cf.CFG[1].Name), out val);
            cData[5] = (byte)val;

            double.TryParse(cf.mf.Tls.LoadProperty(cf.CFG[2].Name), out val);
            cData[6] = (byte)val;
            cData[7] = (byte)((int)val >> 8);

            double.TryParse(cf.mf.Tls.LoadProperty(cf.CFG[3].Name), out val);
            cData[8] = (byte)val;

            double.TryParse(cf.mf.Tls.LoadProperty(cf.CFG[4].Name), out val);
            cData[9] = (byte)val;

            double.TryParse(cf.mf.Tls.LoadProperty(cf.CFG[5].Name), out val);
            cData[10] = (byte)(val * 10);
            cData[11] = (byte)((int)(val * 10) >> 8);

            if (RestartModule) cData[12] = 1; else cData[12] = 0;

            cf.mf.SendSerial(cData);
            cf.mf.UDPnetwork.SendUDPMessage(cData);
        }
    }
}
