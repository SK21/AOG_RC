using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.PGNs
{
    public class PGN18
    {
        // module config 4
        // 0	relay 8 pin
        // 1	relay 9 pin
        // 2	relay 10 pin
        // 3	relay 11 pin
        // 4	relay 12 pin
        // 5	relay 13 pin
        // 6	relay 14 pin
        // 7	relay 15 pin

        private byte[] cData;
        private int cModuleID;
        private FormStart mf;

        public PGN18(FormStart Main, int ModuleID)
        {
            mf = Main;
            cData = new byte[8];
            cModuleID = ModuleID;
            Load();
        }

        public byte[] GetData()
        {
            return cData;
        }

        public void Save()
        {
            String Name;
            for (int i = 0; i < cData.Length; i++)
            {
                Name = "Module" + cModuleID.ToString() + "_Config4_Data" + i.ToString();
                mf.Tls.SaveProperty(Name, cData[i].ToString());
            }
        }

        public void Send()
        {
            mf.CanBus1.SendCanMessage(18, (byte)cModuleID, 0, cData);
        }

        public void SetRelayPins(byte[] RelayPins)
        {
            for (int i = 0; i < 8; i++)
            {
                cData[i] = RelayPins[i - 8];
            }
        }

        private void Load()
        {
            String Name;
            Array.Clear(cData, 0, cData.Length);

            for (int i = 0; i < cData.Length; i++)
            {
                Name = "Module" + cModuleID.ToString() + "_Config4_Data" + i.ToString();
                if (byte.TryParse(mf.Tls.LoadProperty(Name), out byte Val))
                {
                    cData[i] = Val;
                }
            }
        }
    }
}