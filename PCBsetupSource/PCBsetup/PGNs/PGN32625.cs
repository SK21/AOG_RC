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
        //4     IP address
        //5     Commands
        //      - Use MCP23107 
        //      - Relay on high
        //      - Flow on high
        //6     debounce minimum ms
        //7     crc

        private byte[] cData = new byte[8];
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
            bool Result = false;

            // text boxes
            cData[2] = (byte)cf.Boxes.Value("tbNanoModuleID");
            cData[3] = (byte)cf.Boxes.Value("tbNanoSensorCount");
            cData[4] = (byte)cf.Boxes.Value("tbNanoIP");

            // check boxes
            cData[5] = 0;
            for (int i = 0; i < cf.CKs.Length; i++)
            {
                Name = cf.Text + "/" + cf.CKs[i].Name;
                bool.TryParse(cf.mf.Tls.LoadProperty(Name), out Checked);
                if (Checked) cData[5] |= (byte)Math.Pow(2, i);
            }

            cData[6] = (byte)cf.Boxes.Value("tbNanoDebounce");

            // crc  
            cData[7] = cf.mf.Tls.CRC(cData, 7);

            Result = cf.mf.CommPort.Send(cData);
            //cf.mf.UDPmodulesConfig.SendUDPMessage(cData);
            return Result;
        }
    }
}