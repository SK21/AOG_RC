using PCBsetup.Forms;
using System;

namespace PCBsetup
{
    public class PGN32625
    {
        // Nano PCB config
        //0     HeaderLo    113
        //1     HeaderHi    127
        //2     Module ID
        //3     flow sensor count
        //4     Commands
        //      - Use MCP23107 
        //      - Relay on high
        //      - Flow on high

        private byte[] cData = new byte[5];
        private frmNanoSettings cf;
        private string Name;

        public PGN32625(frmNanoSettings CalledFrom)
        {
            cf = CalledFrom;
            cData[0] = 113;
            cData[1] = 127;
        }

        public bool Send()
        {
            bool Checked;
            double val;
            bool Result = false;

            // text boxes
            double.TryParse(cf.mf.Tls.LoadProperty(cf.CFG[0].Name), out val);
            cData[2] = (byte)val;

            double.TryParse(cf.mf.Tls.LoadProperty(cf.CFG[1].Name), out val);
            cData[3] = (byte)val;

            // check boxes
            cData[4] = 0;
            for (int i = 0; i < cf.CKs.Length; i++)
            {
                Name = cf.CKs[i].Name;
                bool.TryParse(cf.mf.Tls.LoadProperty(Name), out Checked);
                if (Checked) cData[4] |= (byte)Math.Pow(2, i);
            }

            Result = cf.mf.CommPort.SendData(cData);
            cf.mf.UDPmodulesConfig.SendUDPMessage(cData);
            return Result;
        }
    }
}