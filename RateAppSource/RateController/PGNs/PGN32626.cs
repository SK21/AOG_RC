using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController
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

        private byte[] cData = new byte[24];
        private frmNanoSettings cf;

        public PGN32626(frmNanoSettings CalledFrom)
        {
            cf = CalledFrom;
            cData[0] = 114;
            cData[1] = 127;
        }

        public void Send()
        {
            byte val;

            for (int i = 2; i < 22; i++)
            {
                byte.TryParse(cf.mf.Tls.LoadProperty(cf.CFG[i + 2].Name), out val);
                cData[i] = val;
            }

            cf.mf.SendSerial(cData);
            cf.mf.UDPmodulesConfig.SendUDPMessage(cData);
        }
    }
}
