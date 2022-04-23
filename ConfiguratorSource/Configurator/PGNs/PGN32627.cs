namespace Configurator
{
    public class PGN32627
    {
        // Switchbox pins
        //0         HeaderLo    115
        //1         HeaderHi    127
        //2         SW0
        //3         SW1
        //4         SW2
        //5         SW3
        //6         Auto
        //7         Master On
        //8         Master Off
        //9         Rate Up
        //10        Rate Down

        private byte[] cData = new byte[11];
        private frmSwitchboxSettings cf;

        public PGN32627(frmSwitchboxSettings CalledFrom)
        {
            cf = CalledFrom;
            cData[0] = 115;
            cData[1] = 127;
        }

        public void Send()
        {
            byte val;

            for (int i = 2; i < 11; i++)
            {
                byte.TryParse(cf.mf.Tls.LoadProperty(cf.CFG[i - 2].Name), out val);
                cData[i] = val;
            }
            
            cf.mf.CommPort.SendData(cData);
            cf.mf.UDPmodulesConfig.SendUDPMessage(cData);
        }
    }
}