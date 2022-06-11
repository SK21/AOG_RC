using PCBsetup.Forms;
using System;
using System.Diagnostics;

namespace PCBsetup
{
    public class PGN32626
    {
        // Nano PCB pins
        //0         HeaderLo    114
        //1         HeaderHi    127
        //2         Flow 1
        //3         Flow 2
        //4         Dir 1
        //5         Dir 2
        //6         PWM 1
        //7         PWM 2
        //8 - 23    Relays 1 - 16    
        //24        crc

        private byte[] cData = new byte[25];
        private frmNanoSettings cf;

        public PGN32626(frmNanoSettings CalledFrom)
        {
            cf = CalledFrom;
            cData[0] = 114;
            cData[1] = 127;
        }

        public bool Send()
        {
            cData[2] = (byte)cf.Boxes.Value("tbNanoFlow1");
            cData[3] = (byte)cf.Boxes.Value("tbNanoFlow2");
            cData[4] = (byte)cf.Boxes.Value("tbNanoDir1");
            cData[5] = (byte)cf.Boxes.Value("tbNanoDir2");
            cData[6] = (byte)cf.Boxes.Value("tbNanoPWM1");
            cData[7] = (byte)cf.Boxes.Value("tbNanoPWM2");

            cData[8] = (byte)cf.Boxes.Value("tbRelay1");
            cData[9] = (byte)cf.Boxes.Value("tbRelay2");
            cData[10] = (byte)cf.Boxes.Value("tbRelay3");
            cData[11] = (byte)cf.Boxes.Value("tbRelay4");
            cData[12] = (byte)cf.Boxes.Value("tbRelay5");
            cData[13] = (byte)cf.Boxes.Value("tbRelay6");
            cData[14] = (byte)cf.Boxes.Value("tbRelay7");
            cData[15] = (byte)cf.Boxes.Value("tbRelay8");

            cData[16] = (byte)cf.Boxes.Value("tbRelay9");
            cData[17] = (byte)cf.Boxes.Value("tbRelay10");
            cData[18] = (byte)cf.Boxes.Value("tbRelay11");
            cData[19] = (byte)cf.Boxes.Value("tbRelay12");
            cData[20] = (byte)cf.Boxes.Value("tbRelay13");
            cData[21] = (byte)cf.Boxes.Value("tbRelay14");
            cData[22] = (byte)cf.Boxes.Value("tbRelay15");
            cData[23] = (byte)cf.Boxes.Value("tbRelay16");

            // crc  
            cData[24] = cf.mf.Tls.CRC(cData, 24);

            bool Result = cf.mf.CommPort.Send(cData);
            //cf.mf.UDPmodulesConfig.SendUDPMessage(cData);
            return Result;
        }
    }
}