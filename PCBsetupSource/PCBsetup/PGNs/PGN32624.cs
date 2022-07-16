using PCBsetup.Forms;

namespace PCBsetup
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
        //15    CRC

        private byte[] cData = new byte[16];
        private frmPCBsettings cf;

        public PGN32624(frmPCBsettings CalledFrom)
        {
            cf = CalledFrom;
            cData[0] = 112;
            cData[1] = 127;
        }

        public bool Send()
        {
            cData[2] = (byte)cf.Boxes.Value("tbDir1");
            cData[3] = (byte)cf.Boxes.Value("tbPwm1");
            cData[4] = (byte)cf.Boxes.Value("tbSteerSwitch");
            cData[5] = (byte)cf.Boxes.Value("tbWAS");
            cData[6] = (byte)cf.Boxes.Value("tbSteerRelay");
            cData[7] = (byte)cf.Boxes.Value("tbWorkSwitch");

            cData[8] = (byte)cf.Boxes.Value("tbCurrentSensor");
            cData[9] = (byte)cf.Boxes.Value("tbPressureSensor");
            cData[10] = (byte)cf.Boxes.Value("tbEncoder");
            cData[11] = (byte)cf.Boxes.Value("tbDir2");

            cData[12] = (byte)cf.Boxes.Value("tbPwm2");
            cData[13] = (byte)cf.Boxes.Value("tbSpeedPulse");
            cData[14] = (byte)cf.Boxes.Value("tbSendEnable");

            cData[15] = cf.mf.Tls.CRC(cData, 15);

            bool Result = cf.mf.CommPort.Send(cData);
            //cf.mf.UDPmodulesConfig.SendUDPMessage(cData);

            return Result;
        }
    }
}