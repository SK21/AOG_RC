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
        //7     RS485 Serial port, 1-8
        //8     Module ID
        //9     Commands
        //          - Gyro on
        //          - GGA last
        //          - use rate control
        //          - use ADS1115
        //          - relay on high
        //          - flow on high
        //          - swap pitch for roll
        //          - invert roll
        //10    Restart module

        private byte[] cData = new byte[11];
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
            double.TryParse(cf.mf.Tls.LoadProperty(cf.CFG[6].Name), out val);
            cData[2] = (byte)val;

            double.TryParse(cf.mf.Tls.LoadProperty(cf.CFG[7].Name), out val);
            cData[3] = (byte)val;

            double.TryParse(cf.mf.Tls.LoadProperty(cf.CFG[8].Name), out val);
            cData[4] = (byte)(val * 10);
            cData[5] = (byte)((int)(val * 10) >> 8);

            double.TryParse(cf.mf.Tls.LoadProperty(cf.CFG[9].Name), out val);
            cData[6] = (byte)val;

            double.TryParse(cf.mf.Tls.LoadProperty(cf.CFG[10].Name), out val);
            cData[7] = (byte)val;

            double.TryParse(cf.mf.Tls.LoadProperty(cf.CFG[11].Name), out val);
            cData[8] = (byte)val;

            // check boxes
            cData[9] = 0;
            for (int i = 0; i < cf.CKs.Length; i++)
            {
                Name = cf.CKs[i].Name;
                bool.TryParse(cf.mf.Tls.LoadProperty(Name), out Checked);
                if (Checked) cData[9] |= (byte)Math.Pow(2, i);
            }

            if (RestartModule) cData[10] = 1; else cData[10] = 0;

            cf.mf.UDPmodulesConfig.SendUDPMessage(cData);
        }
    }
}