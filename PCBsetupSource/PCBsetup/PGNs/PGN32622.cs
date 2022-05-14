﻿using PCBsetup.Forms;

namespace PCBsetup
{
    public class PGN32622
    {
        // PCB config 1
        //0     HeaderLo    110
        //1     HeaderHi    127
        //2     Receiver    0-None, 1-SimpleRTK2B, 2-Sparkfun F9P
        //3     NMEA serial port, 0-8
        //4     RTCM serial port, 0-8
        //5     RTCM UDP port # lo
        //6     RTCM UDP port # Hi
        //7     IMU         0-None, 1-Sparkfun BNO, 2-CMPS14, 3-Adafruit BNO
        //8     Read Delay
        //9     Report Interval
        //10    Zero offset Lo
        //11    Zero offset Hi
        //12    RelayControl 0 - no relays, 1 - RS485, 2 - PCA9555 8 relays, 3 - PCA9555 16 relays
        //13    IP address, 3rd octet

        private byte[] cData = new byte[14];
        private frmPCBsettings cf;

        public PGN32622(frmPCBsettings CalledFrom)
        {
            cf = CalledFrom;
            cData[0] = 110;
            cData[1] = 127;
        }

        public bool Send()
        {
            byte tmp;
            double val;

            byte.TryParse(cf.mf.Tls.LoadProperty("GPSreceiver"), out tmp);
            cData[2] = tmp;

            cData[3] = (byte)cf.Boxes.Value("tbNMEAserialPort");
            cData[4] = (byte)cf.Boxes.Value("tbRTCMserialPort");

            val = cf.Boxes.Value("tbRTCM");
            cData[5] = (byte)val;
            cData[6] = (byte)((int)val >> 8);

            byte.TryParse(cf.mf.Tls.LoadProperty("IMU"), out tmp);
            cData[7] = tmp;
            
            cData[8] = (byte)cf.Boxes.Value("tbIMUdelay");
            cData[9] = (byte)cf.Boxes.Value("tbIMUinterval");

            val = cf.Boxes.Value("tbZeroOffset");
            cData[10] = (byte)val;
            cData[11] = (byte)((int)val >> 8);

            byte.TryParse(cf.mf.Tls.LoadProperty("RelayControl"), out tmp);
            cData[12] = tmp;

            cData[13] = (byte)cf.Boxes.Value("tbIPaddress");

            bool Result = cf.mf.CommPort.Send(cData);
            //cf.mf.UDPmodulesConfig.SendUDPMessage(cData);

            return Result;
        }
    }
}