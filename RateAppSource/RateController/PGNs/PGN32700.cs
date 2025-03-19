using RateController.Classes;
using System;

namespace RateController
{
    public class PGN32700
    {
        //PGN32700, module config from RC to modules
        //0     HeaderLo    188
        //1     HeaderHi    127
        //2     Module ID   0-15
        //3	    sensor count
        //4     commands
        //      bit 0 - Relay on high
        //      bit 1 - Flow on high
        //      bit 2 - client mode
        //      bit 3 - work pin is momentary
        //      bit 4 - Is3Wire valve
        //      bit 5 - ADS1115 enabled
        //5	    relay control type   0 - no relays, 1 - GPIOs, 2 - PCA9555 8 relays, 3 - PCA9555 16 relays, 4 - MCP23017
        //                           , 5 - PCA9685, 6 - PCF8574
        //6	    wifi module serial port
        //7	    Sensor 0, Flow pin
        //8     Sensor 0, Dir pin
        //9     Sensor 0, PWM pin
        //10    Sensor 1, Flow pin
        //11    Sensor 1, Dir pin
        //12    Sensor 1, PWM pin
        //13    Relay pins 0-15, bytes 13-28
        //29    work pin
        //30    pressure pin
        //31    -
        //32    CRC

        private const byte cByteCount = 33;
        private const byte HeaderHi = 127;
        private const byte HeaderLo = 188;
        private byte[] cData = new byte[cByteCount];
        private FormStart mf;

        public PGN32700(FormStart Main)
        {
            mf = Main;
            Load();
        }

        public bool ADS1115enabled
        {
            get
            {
                return ((cData[4] & 0b0010_0000) == 0b0010_0000);
            }
            set
            {
                if (value)
                {
                    cData[4] = (byte)(cData[4] | 0b0010_0000);
                }
                else
                {
                    cData[4] = (byte)(cData[4] & 0b1101_1111);
                }
            }
        }

        public bool ClientMode
        {
            get
            {
                return ((cData[4] & 4) == 4);
            }
            set
            {
                if (value)
                {
                    cData[4] = (byte)(cData[4] | 4);
                }
                else
                {
                    cData[4] = (byte)(cData[4] & 0b1111_1011);
                }
            }
        }

        public bool InvertFlow
        {
            get
            {
                return ((cData[4] & 2) == 2);
            }
            set
            {
                if (value)
                {
                    cData[4] = (byte)(cData[4] | 2);
                }
                else
                {
                    cData[4] = (byte)(cData[4] & 0b1111_1101);
                }
            }
        }

        public bool InvertRelay
        {
            get
            {
                return ((cData[4] & 1) == 1);
            }
            set
            {
                if (value)
                {
                    cData[4] = (byte)(cData[4] | 1);
                }
                else
                {
                    cData[4] = (byte)(cData[4] & 0b1111_1110);
                }
            }
        }

        public bool Is3Wire
        {
            get
            {
                return ((cData[4] & 0b0001_0000) == 0b0001_0000);
            }
            set
            {
                if (value)
                {
                    cData[4] = (byte)(cData[4] | 0b0001_0000);
                }
                else
                {
                    cData[4] = (byte)(cData[4] & 0b1110_1111);
                }
            }
        }

        public byte ModuleID
        { set { cData[2] = value; } }

        public bool Momentary
        {
            get
            {
                return ((cData[4] & 8) == 8);
            }
            set
            {
                if (value)
                {
                    cData[4] = (byte)(cData[4] | 8);
                }
                else
                {
                    cData[4] = (byte)(cData[4] & 0b1111_0111);
                }
            }
        }

        public byte PressurePin
        { set { cData[30] = value; } }

        public byte RelayType
        { set { cData[5] = value; } }

        public byte Sensor0Dir
        {
            get { return cData[8]; }
            set { cData[8] = value; }
        }

        public byte Sensor0Flow
        {
            get { return cData[7]; }
            set { cData[7] = value; }
        }

        public byte Sensor0PWM
        {
            get { return cData[9]; }
            set { cData[9] = value; }
        }

        public byte Sensor1Dir
        {
            get { return cData[11]; }
            set { cData[11] = value; }
        }

        public byte Sensor1Flow
        {
            get { return cData[10]; }
            set { cData[10] = value; }
        }

        public byte Sensor1PWM
        {
            get { return cData[12]; }
            set { cData[12] = value; }
        }

        public byte SensorCount
        { set { cData[3] = value; } }

        public byte WifiPort
        { set { cData[6] = value; } }

        public byte WorkPin
        { set { cData[29] = value; } }

        public byte[] GetData()
        {
            return cData;
        }

        public void Load()
        {
            String Name;
            Array.Clear(cData, 0, cByteCount);
            cData[0] = HeaderLo;
            cData[1] = HeaderHi;

            for (int i = 2; i < cByteCount; i++)
            {
                Name = "ModuleConfig_" + i.ToString();
                if (byte.TryParse(mf.Tls.LoadProperty(Name), out byte Val))
                {
                    cData[i] = Val;
                }
            }
        }

        public void RelayPins(byte[] RelayPin)
        {
            for (int i = 0; i < 16; i++)
            {
                cData[i + 13] = RelayPin[i];
            }
        }

        public void Save()
        {
            String Name;
            for (int i = 2; i < cByteCount; i++)
            {
                Name = "ModuleConfig_" + i.ToString();
                Props.SetProp(Name, cData[i].ToString());
            }
        }

        public void Send()
        {
            // CRC
            cData[cByteCount - 1] = mf.Tls.CRC(cData, cByteCount - 1);

            // send
            mf.UDPmodules.SendUDPMessage(cData);
        }
    }
}