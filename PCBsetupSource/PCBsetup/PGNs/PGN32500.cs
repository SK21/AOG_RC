using PCBsetup.Forms;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCBsetup
{
    public class PGN32500
    {
        // rate module config
        // 0        244
        // 1        126
        // 2        ID
        // 3        Sensor count
        // 4        IPpart3
        // 5        Commands
        //          - Relay on high
        //          - Flow on high
        // 6        Relay control type  0 - no relays, 1 - RS485, 2 - PCA9555 8 relays, 3 - PCA9555 16 relays, 4 - MCP23017, 5 - Teensy GPIO
        // 7        Wemos serial port
        // 8        Sensor 0, flow pin
        // 9        Sensor 0, dir pin
        // 10       Sensor 0, pwm pin
        // 11       Sensor 1, flow pin
        // 12       Sensor 1, dir pin
        // 13       Sensor 1, pwm pin
        // 14-29    Relay pins 0-15
        // 30       debounce minimum ms
        // 31       CRC

        private byte[] cData = new byte[32];
        private frmTRsettings cf;
        private string Name;

        public PGN32500(frmTRsettings CalledFrom)
        {
            cf = CalledFrom;
            cData[0] = 244;
            cData[1] = 126;
        }

        public bool Send()
        {
            byte tmp;
            bool Checked;

            // text boxes
            cData[2] = (byte)cf.Boxes.Value("tbTRModuleID");
            cData[3] = (byte)cf.Boxes.Value("tbTRSensorCount");
            cData[4] = (byte)cf.Boxes.Value("tbTRIP");
            cData[7] = (byte)cf.Boxes.Value("tbTRWemosPort");

            // check boxes
            cData[5] = 0;
            for (int i = 0; i < cf.CKs.Length; i++)
            {
                Name = cf.Text + "/" + cf.CKs[i].Name;
                bool.TryParse(cf.mf.Tls.LoadProperty(Name), out Checked);
                if (Checked) cData[5] |= (byte)Math.Pow(2, i);
            }

            // combo boxes
            byte.TryParse(cf.mf.Tls.LoadProperty(cf.Text + "/" + "TRRelayControl"), out tmp);
            cData[6] = tmp;

            // pins
            cData[8] = (byte)cf.Boxes.Value("tbTRFlow1");
            cData[9] = (byte)cf.Boxes.Value("tbTRDir1");
            cData[10] = (byte)cf.Boxes.Value("tbTRPWM1");
            cData[11] = (byte)cf.Boxes.Value("tbTRFlow2");
            cData[12] = (byte)cf.Boxes.Value("tbTRDir2");
            cData[13] = (byte)cf.Boxes.Value("tbTRPWM2");

            cData[14] = (byte)cf.Boxes.Value("tbTRRelay1");
            cData[15] = (byte)cf.Boxes.Value("tbTRRelay2");
            cData[16] = (byte)cf.Boxes.Value("tbTRRelay3");
            cData[17] = (byte)cf.Boxes.Value("tbTRRelay4");
            cData[18] = (byte)cf.Boxes.Value("tbTRRelay5");
            cData[19] = (byte)cf.Boxes.Value("tbTRRelay6");
            cData[20] = (byte)cf.Boxes.Value("tbTRRelay7");
            cData[21] = (byte)cf.Boxes.Value("tbTRRelay8");

            cData[22] = (byte)cf.Boxes.Value("tbTRRelay9");
            cData[23] = (byte)cf.Boxes.Value("tbTRRelay10");
            cData[24] = (byte)cf.Boxes.Value("tbTRRelay11");
            cData[25] = (byte)cf.Boxes.Value("tbTRRelay12");
            cData[26] = (byte)cf.Boxes.Value("tbTRRelay13");
            cData[27] = (byte)cf.Boxes.Value("tbTRRelay14");
            cData[28] = (byte)cf.Boxes.Value("tbTRRelay15");
            cData[29] = (byte)cf.Boxes.Value("tbTRRelay16");

            cData[30] = (byte)cf.Boxes.Value("tbTRDebounce");

            cData[31] = cf.mf.Tls.CRC(cData, 31);
            bool Result = cf.mf.CommPort.Send(cData);
            return Result;
        }
    }
}
