namespace RateController
{
    public class PGN32624
    {
        // PCB pins
        //0     HeaderLo    112
        //1     HeaderHi    127
        //2     Steer DIR
        //3     Steer PWM
        //4     Steer switch
        //5     Wheel angle sensor
        //6     Steer relay
        //7     Work switch
        //8     Current sensor
        //9     Pressure sensor
        //10    Encoder
        //11    Rate DIR
        //12    Rate PWM
        //13    Speed Pulse
        //14    RS485 send enable
        //15    Restart module

        private byte[] cData = new byte[16];
        private frmPCBsettings cf;

        public PGN32624(frmPCBsettings CalledFrom)
        {
            cf = CalledFrom;
            cData[0] = 112;
            cData[1] = 127;
        }

        public void Send(bool RestartModule = false)
        {
            byte val;

            for (int i = 2; i < 15; i++)
            {
                byte.TryParse(cf.mf.Tls.LoadProperty(cf.CFG[i + 9].Name), out val);
                cData[i] = val;
            }

            if (RestartModule) cData[15] = 1; else cData[15] = 0;

            cf.mf.SendSerial(cData);
            cf.mf.UDPconfig.SendUDPMessage(cData);
        }
    }
}