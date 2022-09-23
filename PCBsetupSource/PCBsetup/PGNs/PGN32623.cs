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
        //6     Analog method 0 ADS1115 (Teensy), 1 pins (Teensy), 2 ADS1115 (D1 Mini)
        //7     RelayControl 0 - no relays, 1 - RS485, 2 - PCA9555 8 relays, 3 - PCA9555 16 relays
        //8     Module ID
        //9     Commands
        //          - use rate control
        //          - use TB6612 motor controller
        //          - relay on high
        //          - flow on high
        //          - swap pitch for roll
        //          - invert roll
        //          - Gyro on
        //          - use linear actuator
        //10    CRC

        private byte[] cData = new byte[11];
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
            byte tmp;
            bool Checked;
            double val;

            // text boxes
            cData[2] = (byte)cf.Boxes.Value("tbMinSpeed");
            cData[3] = (byte)cf.Boxes.Value("tbMaxSpeed");

            val = cf.Boxes.Value("tbPulseCal");
            cData[4] = (byte)(val * 10);
            cData[5] = (byte)((int)(val * 10) >> 8);

            byte.TryParse(cf.mf.Tls.LoadProperty("AnalogMethod"), out tmp);
            cData[6] = tmp;

            byte.TryParse(cf.mf.Tls.LoadProperty("RelayControl"), out tmp);
            cData[7] = tmp;

            cData[8] = (byte)cf.Boxes.Value("tbModule");

            // check boxes
            cData[9] = 0;
            for (int i = 0; i < cf.CKs.Length; i++)
            {
                Name = cf.CKs[i].Name;
                bool.TryParse(cf.mf.Tls.LoadProperty(Name), out Checked);
                if (Checked) cData[9] |= (byte)Math.Pow(2, i);
            }

            cData[10] = cf.mf.Tls.CRC(cData, 10);

            bool Result = cf.mf.CommPort.Send(cData);
            //cf.mf.UDPmodulesConfig.SendUDPMessage(cData);

            return Result;
        }
    }
}