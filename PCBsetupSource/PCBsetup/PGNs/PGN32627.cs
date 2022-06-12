using PCBsetup.Forms;

namespace PCBsetup
{
    public class PGN32627
    {
        // Switchbox pins
        //0         HeaderLo    115
        //1         HeaderHi    127
        //2         Auto
        //3         Master On
        //4         Master Off
        //5         Rate Up
        //6         Rate Down
        //7         IP address
        //8-23      switches 1-16
        //24        crc

        private byte[] cData = new byte[25];
        private frmSwitchboxSettings cf;

        public PGN32627(frmSwitchboxSettings CalledFrom)
        {
            cf = CalledFrom;
            cData[0] = 115;
            cData[1] = 127;
        }

        public bool Send()
        {
            cData[2] = (byte)cf.Boxes.Value("tbAuto");
            cData[3] = (byte)cf.Boxes.Value("tbMasterOn");
            cData[4] = (byte)cf.Boxes.Value("tbMasterOff");
            cData[5] = (byte)cf.Boxes.Value("tbRateUp");
            cData[6] = (byte)cf.Boxes.Value("tbRateDown");
            cData[7] = (byte)cf.Boxes.Value("tbIPaddress");

            cData[8] = (byte)cf.Boxes.Value("tbSW1");
            cData[9] = (byte)cf.Boxes.Value("tbSW2");
            cData[10] = (byte)cf.Boxes.Value("tbSW3");
            cData[11] = (byte)cf.Boxes.Value("tbSW4");
            cData[12] = (byte)cf.Boxes.Value("tbSW5");
            cData[13] = (byte)cf.Boxes.Value("tbSW6");
            cData[14] = (byte)cf.Boxes.Value("tbSW7");
            cData[15] = (byte)cf.Boxes.Value("tbSW8");

            cData[16] = (byte)cf.Boxes.Value("tbSW9");
            cData[17] = (byte)cf.Boxes.Value("tbSW10");
            cData[18] = (byte)cf.Boxes.Value("tbSW11");
            cData[19] = (byte)cf.Boxes.Value("tbSW12");
            cData[20] = (byte)cf.Boxes.Value("tbSW13");
            cData[21] = (byte)cf.Boxes.Value("tbSW14");
            cData[22] = (byte)cf.Boxes.Value("tbSW15");
            cData[23] = (byte)cf.Boxes.Value("tbSW16");

            cData[24] = cf.mf.Tls.CRC(cData, 24);

            bool Result = cf.mf.CommPort.Send(cData);
            //cf.mf.UDPmodulesConfig.SendUDPMessage(cData);

            return Result;
        }
    }
}