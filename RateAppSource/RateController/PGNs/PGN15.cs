using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RateController.PGNs
{
    public class PGN15
    {
        // module config 1
        // 0	new module ID
        // 1	sensor count
        // 2	relay control type   0 - no relays, 1 - GPIOs, 2 - PCA9555 8 relays, 3 - PCA9555 16 relays, 4 - MCP23017
        //                           , 5 - PCA9685, 6 - PCF8574
        // 3	wifi serial port
        // 4	work pin
        // 5	pressure pin
        // 6	commands
        //      bit 0 - Relay on high
        //      bit 1 - Flow on high
        //      bit 2 - client mode
        //      bit 3 - work pin is momentary
        //      bit 4 - Is3Wire valve
        //      bit 5 - ADS1115 enabled

        private byte[] cData;
        private FormStart mf;
        private int cModuleID;

        public PGN15(FormStart Main, int ModuleID)
        {
            mf = Main;
            cData = new byte[7];
            cModuleID = ModuleID;
            Load();
        }

        public bool ADS1115enabled
        {
            get
            {
                return ((cData[6] & 0b0010_0000) == 0b0010_0000);
            }
            set
            {
                if (value)
                {
                    cData[6] = (byte)(cData[6] | 0b0010_0000);
                }
                else
                {
                    cData[6] = (byte)(cData[6] & 0b1101_1111);
                }
            }
        }

        public bool ClientMode
        {
            get
            {
                return ((cData[6] & 4) == 4);
            }
            set
            {
                if (value)
                {
                    cData[6] = (byte)(cData[6] | 4);
                }
                else
                {
                    cData[6] = (byte)(cData[6] & 0b1111_1011);
                }
            }
        }

        public bool InvertFlow
        {
            get
            {
                return ((cData[6] & 2) == 2);
            }
            set
            {
                if (value)
                {
                    cData[6] = (byte)(cData[6] | 2);
                }
                else
                {
                    cData[6] = (byte)(cData[6] & 0b1111_1101);
                }
            }
        }

        public bool InvertRelay
        {
            get
            {
                return ((cData[6] & 1) == 1);
            }
            set
            {
                if (value)
                {
                    cData[6] = (byte)(cData[6] | 1);
                }
                else
                {
                    cData[6] = (byte)(cData[6] & 0b1111_1110);
                }
            }
        }

        public bool Is3Wire
        {
            get
            {
                return ((cData[6] & 0b0001_0000) == 0b0001_0000);
            }
            set
            {
                if (value)
                {
                    cData[6] = (byte)(cData[6] | 0b0001_0000);
                }
                else
                {
                    cData[6] = (byte)(cData[6] & 0b1110_1111);
                }
            }
        }

        public byte ModuleID
        { set { cData[0] = value; } }

        public bool Momentary
        {
            get
            {
                return ((cData[6] & 8) == 8);
            }
            set
            {
                if (value)
                {
                    cData[6] = (byte)(cData[6] | 8);
                }
                else
                {
                    cData[6] = (byte)(cData[6] & 0b1111_0111);
                }
            }
        }

        public byte PressurePin
        { get { return cData[5]; } set { cData[5] = value; } }

        public byte RelayType
        { set { cData[2] = value; } }

        public byte SensorCount
        { set { cData[1] = value; } }

        public byte WifiPort
        { set { cData[3] = value; } }

        public byte WorkPin
        { get { return cData[4]; } set { cData[4] = value; } }

        public byte[] GetData()
        {
            return cData;
        }

        public void Save()
        {
            String Name;
            for (int i = 0; i < cData.Length; i++)
            {
                Name = "Module"+ cModuleID.ToString()+"_Config1_Data" + i.ToString();
                mf.Tls.SaveProperty(Name, cData[i].ToString());
            }
        }

        public void Send()
        {
            mf.CanBus1.SendCanMessage(15, (byte)cModuleID, 0, cData);
        }

        private void Load()
        {
            String Name;
            Array.Clear(cData, 0, cData.Length);

            for (int i = 0; i < cData.Length; i++)
            {
                Name = "Module" + cModuleID.ToString() + "_Config1_Data" + i.ToString();
                if (byte.TryParse(mf.Tls.LoadProperty(Name), out byte Val))
                {
                    cData[i] = Val;
                }
            }
        }
    }
}