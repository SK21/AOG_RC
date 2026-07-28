using RateController.Classes;
using System;

namespace RateController.PGNs
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
        //      bit 6 - assign module ID: board adopts byte 2 as its new ID
        //              (only one board may be connected); clear = byte 2 is a
        //              filter, board applies config only when it matches its ID
        //      bit 7 - invert work switch (NO sensor)
        //5	    onboard relay control   0 - no relays, 1 - GPIOs, 2 - PCA9555 8 relays, 3 - PCA9555 16 relays, 4 - MCP23017
        //                              , 5 - PCA9685, 6 - PCF8574
        //6	    remote relay control    0 - no relays, 1 - GPIOs, 2 - PCA9555 8 relays, 3 - PCA9555 16 relays, 4 - MCP23017
        //                              , 5 - PCA9685, 6 - PCF8574
        //7	    Sensor 0, Flow pin
        //8     Sensor 0, Dir pin
        //9     Sensor 0, PWM pin
        //10    Sensor 1, Flow pin
        //11    Sensor 1, Dir pin
        //12    Sensor 1, PWM pin
        //13    Relay pins 0-15, bytes 13-28
        //29    work pin
        //30    pressure pin
        //31    CommMode             0 - UDP only, 1 - CAN Proprietary
        //32    CRC

        private const byte cByteCount = 33;
        private const byte HeaderHi = 127;
        private const byte HeaderLo = 188;
        private byte[] cData = new byte[cByteCount];

        public PGN32700()
        {
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
                return ((cData[4] & 0b0000_0100) == 0b0000_0100);
            }
            set
            {
                if (value)
                {
                    cData[4] = (byte)(cData[4] | 0b0000_0100);
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

        public bool InvertWork
        {
            get
            {
                return ((cData[4] & 0b1000_0000) == 0b1000_0000);
            }
            set
            {
                if (value)
                {
                    cData[4] = (byte)(cData[4] | 0b1000_0000);
                }
                else
                {
                    cData[4] = (byte)(cData[4] & 0b0111_1111);
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

        public byte OnboardRelayType
        { set { cData[5] = value; } }

        public byte PressurePin
        { set { cData[30] = value; } }

        public byte RemoteRelayType
        { set { cData[6] = value; } }

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

        public byte WorkPin
        { set { cData[29] = value; } }

        public byte[] GetData()
        {
            return cData;
        }

        public void Load()
        {
            // byte 31 - comm mode is overwritten from Props.CanEnabled at send
            String Name;
            Array.Clear(cData, 0, cByteCount);
            cData[0] = HeaderLo;
            cData[1] = HeaderHi;

            for (int i = 2; i < cByteCount; i++)
            {
                Name = "ModuleConfig_" + i.ToString();
                if (byte.TryParse(Props.GetProp(Name), out byte Val))
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
            cData[31] = Props.CanEnabled ? (byte)1 : (byte)0;

            String Name;
            for (int i = 2; i < cByteCount; i++)
            {
                Name = "ModuleConfig_" + i.ToString();
                Props.SetProp(Name, cData[i].ToString());
            }
        }

        public void Send(byte? commMode = null, bool assignID = false)
        {
            // sensor 0/1 pins mirror the sensor-pins config (PGN 32507) so firmware
            // that only reads 32700 stays in sync with the pins grid. Only slots below
            // the sensor count (byte 3) are mirrored - unused slots go out as NC, the
            // same as PGN 32507 which sends one packet per configured sensor. The
            // firmware validates bytes 7-12 against a board specific pin list no matter
            // what the sensor count is, so an unused sensor's stale pins would otherwise
            // get the whole config rejected.
            if (Core.SensorPins != null)
            {
                for (int i = 0; i < 2; i++)
                {
                    bool InUse = (i < cData[3]);
                    cData[7 + i * 3] = InUse ? Core.SensorPins.FlowPin(i) : PGN32507.NC;
                    cData[8 + i * 3] = InUse ? Core.SensorPins.DirPin(i) : PGN32507.NC;
                    cData[9 + i * 3] = InUse ? Core.SensorPins.PWMPin(i) : PGN32507.NC;
                }
            }

            // bit 6 is per-send, not part of the saved config - written both ways
            // so a normal send can never carry a leftover assign bit
            if (assignID)
            {
                cData[4] = (byte)(cData[4] | 0b0100_0000);
            }
            else
            {
                cData[4] = (byte)(cData[4] & 0b1011_1111);
            }

            cData[31] = commMode ?? (Props.CanEnabled ? (byte)1 : (byte)0);
            cData[cByteCount - 1] = Core.Tls.CRC(cData, cByteCount - 1);

            // Send via whichever transport(s) are currently active.
            // Callers that need to pre-send a CommMode change before switching transports
            // must do so explicitly while the old transport is still active (see frmMenuOptions,
            // frmMenuNetwork btnOK_Click).
            if (Props.CanEnabled) Core.CanBridgeComm?.SendModuleCommand(cData);
            Core.UDPmodules.Send(cData);
        }
    }
}
