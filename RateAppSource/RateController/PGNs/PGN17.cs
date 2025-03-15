using System;

namespace RateController.PGNs
{
    public class PGN17
    {
        // module config 3
        // 0	relay 0 pin
        // 1	relay 1 pin
        // 2	relay 2 pin
        // 3	relay 3 pin
        // 4	relay 4 pin
        // 5	relay 5 pin
        // 6	relay 6 pin
        // 7	relay 7 pin

        private byte[] cData;
        private int cModuleID;
        private FormStart mf;

        public PGN17(FormStart Main, int ModuleID)
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
                Name = "Module" + cModuleID.ToString() + "_Config3_Data" + i.ToString();
                mf.Tls.SaveProperty(Name, cData[i].ToString());
            }
        }

        public void Send()
        {
            mf.CanBus1.SendCanMessage(17, (byte)cModuleID, 0, cData);
        }

        public void SetRelayPins(byte[] RelayPins)
        {
            for (int i = 0; i < 8; i++)
            {
                cData[i] = RelayPins[i];
            }
        }

        private void Load()
        {
            String Name;
            Array.Clear(cData, 0, cData.Length);

            for (int i = 0; i < cData.Length; i++)
            {
                Name = "Module" + cModuleID.ToString() + "_Config3_Data" + i.ToString();
                if (byte.TryParse(mf.Tls.LoadProperty(Name), out byte Val))
                {
                    cData[i] = Val;
                }
            }
        }
    }
}
