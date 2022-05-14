using PCBsetup.Forms;
using System;

namespace PCBsetup
{
    public class PGN32623
    {
        // PCB config 2
        //0     HeaderLo    111
        //1     HeaderHi    127
        //2     Minimum speed
        //3     Maximum speed
        //4     Speed pulse cal X 10 Lo
        //5     Speed pulse cal X 10 Hi
        //6     ADS1115 WAS pin
        //7     RS485 Serial port, 0-8
        //8     Module ID
        //9     Commands
        //          - use rate control
        //          - use ADS1115
        //          - relay on high
        //          - flow on high
        //          - swap pitch for roll
        //          - invert roll
        //          - Gyro on

        private byte[] cData = new byte[10];
        private frmPCBsettings cf;
        private string Name;

        public PGN32623(frmPCBsettings CalledFrom)
        {
            cf = CalledFrom;
            cData[0] = 111;
            cData[1] = 127;
        }

        public bool Send()
        {
            bool Checked;
            double val;

            // text boxes
            cData[2] = (byte)cf.Boxes.Value("tbMinSpeed");
            cData[3] = (byte)cf.Boxes.Value("tbMaxSpeed");

            val = cf.Boxes.Value("tbPulseCal");
            cData[4] = (byte)(val * 10);
            cData[5] = (byte)((int)(val * 10) >> 8);

            cData[6] = (byte)cf.Boxes.Value("tbAdsWasPin");
            cData[7] = (byte)cf.Boxes.Value("tbRS485port");
            cData[8] = (byte)cf.Boxes.Value("tbModule");

            // check boxes
            cData[9] = 0;
            for (int i = 0; i < cf.CKs.Length; i++)
            {
                Name = cf.CKs[i].Name;
                bool.TryParse(cf.mf.Tls.LoadProperty(Name), out Checked);
                if (Checked) cData[9] |= (byte)Math.Pow(2, i);
            }

            bool Result = cf.mf.CommPort.Send(cData);
            //cf.mf.UDPmodulesConfig.SendUDPMessage(cData);

            return Result;
        }
    }
}