namespace RateController
{
    public class PGN32622
    {
        // PCB config 1
        //0     HeaderLo    110
        //1     HeaderHi    127
        //2     Receiver    0-None, 1-SimpleRTK2B, 2-Sparkfun F9P
        //3     NMEA serial port, 1-8
        //4     RTCM serial port, 1-8
        //5     RTCM UDP port # lo
        //6     RTCM UDP port # Hi
        //7     IMU         0-None, 1-Sparkfun BNO, 2-CMPS14, 3-Adafruit BNO
        //8     Read Delay
        //9     Report Interval
        //10    Zero offset Lo
        //11    Zero offset Hi
        //12    Restart module

        private byte[] cData = new byte[13];
        private frmPCBsettings cf;

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

            double.TryParse(cf.mf.Tls.LoadProperty(cf.CFG[0].Name), out val);
            cData[3] = (byte)val;

            double.TryParse(cf.mf.Tls.LoadProperty(cf.CFG[1].Name), out val);
            cData[4] = (byte)val;

            double.TryParse(cf.mf.Tls.LoadProperty(cf.CFG[2].Name), out val);
            cData[5] = (byte)val;
            cData[6] = (byte)((int)val >> 8);

            byte.TryParse(cf.mf.Tls.LoadProperty("IMU"), out tmp);
            cData[7] = tmp;

            double.TryParse(cf.mf.Tls.LoadProperty(cf.CFG[3].Name), out val);
            cData[8] = (byte)val;

            double.TryParse(cf.mf.Tls.LoadProperty(cf.CFG[4].Name), out val);
            cData[9] = (byte)val;

            double.TryParse(cf.mf.Tls.LoadProperty(cf.CFG[5].Name), out val);
            cData[10] = (byte)val;
            cData[11] = (byte)((int)val >> 8);

            if (RestartModule) cData[12] = 1; else cData[12] = 0;

            cf.mf.SendSerial(cData);
            cf.mf.UDPmodulesConfig.SendUDPMessage(cData);
        }
    }
}