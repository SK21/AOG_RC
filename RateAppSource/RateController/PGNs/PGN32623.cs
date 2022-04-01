using System;

namespace RateController
{
    public class PGN32623
    {
        // PCB config 2
        //0     HeaderLo    111
        //1     HeaderHi    127
        //2     RTCM port lo
        //3     RTCM port Hi
        //4     ADS1115 WAS pin, 1-4
        //5     Module ID
        //6     Power relay #
        //7     RS485 Serial port
        //8     Commands
        //          - Gyro on
        //          - GGA last
        //          - use rate control
        //          - use ADS1115
        //          - relay on high
        //          - flow on high
        //          - swap pitch for roll
        //          - invert roll
        //9     Restart module

        private byte[] cData = new byte[10];
        private frmPCBsettings cf;
        private string Name;

        public PGN32623(frmPCBsettings CalledFrom)
        {
            cf = CalledFrom;
            cData[0] = 111;
            cData[1] = 127;
        }

        public void Send(bool RestartModule = false)
        {
            bool Checked;
            double val;

            // text boxes
            Name = cf.CFG[6].Name;
            double.TryParse(cf.mf.Tls.LoadProperty(Name), out val);
            cData[2] = (byte)val;
            cData[3] = (byte)((int)val >> 8);

            for (int i = 4; i < 8; i++)
            {
                Name = cf.CFG[i + 3].Name;
                double.TryParse(cf.mf.Tls.LoadProperty(Name), out val);
                cData[i] = (byte)val;
            }

            // check boxes
            cData[8] = 0;
            for (int i = 0; i < cf.CKs.Length; i++)
            {
                Name = cf.CKs[i].Name;
                bool.TryParse(cf.mf.Tls.LoadProperty(Name), out Checked);
                if (Checked) cData[8] |= (byte)(Math.Pow(2, i));
            }

            if (RestartModule) cData[9] = 1; else cData[9] = 0;

            cf.mf.SendSerial(cData);
            cf.mf.UDPconfig.SendUDPMessage(cData);
        }
    }
}