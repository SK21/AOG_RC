using PCBsetup.Forms;

namespace PCBsetup
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
        //11        IP address

        private byte[] cData = new byte[12];
        private frmSwitchboxSettings cf;

        public PGN32627(frmSwitchboxSettings CalledFrom)
        {
            cf = CalledFrom;
            cData[0] = 115;
            cData[1] = 127;
        }

        public bool Send()
        {
            cData[2] = (byte)cf.Boxes.Value("tbSW1");
            cData[3] = (byte)cf.Boxes.Value("tbSW2");
            cData[4] = (byte)cf.Boxes.Value("tbSW3");
            cData[5] = (byte)cf.Boxes.Value("tbSW4");
            cData[6] = (byte)cf.Boxes.Value("tbAuto");

            cData[7] = (byte)cf.Boxes.Value("tbMasterOn");
            cData[8] = (byte)cf.Boxes.Value("tbMasterOff");
            cData[9] = (byte)cf.Boxes.Value("tbRateUp");
            cData[10] = (byte)cf.Boxes.Value("tbRateDown");
            cData[11] = (byte)cf.Boxes.Value("tbIPaddress");

            bool Result = cf.mf.CommPort.Send(cData);
            //cf.mf.UDPmodulesConfig.SendUDPMessage(cData);

            return Result;
        }
    }
}