using RateController.Classes;
using System;

namespace RateController.PGNs
{
    public class PGN32507
    {
        //PGN32507, sensor pins from RC to modules, one packet per sensor
        //0     HeaderLo    251
        //1     HeaderHi    126
        //2     Mod/Sen ID          module 0-15 high nibble, sensor 0-15 low nibble
        //3     flow pin            255 = not connected
        //4     dir pin
        //5     pwm pin
        //6     bin sensor pin      255 = not connected, bin alarm off
        //7     flags
        //      bit 0   invert bin sensor
        //8     spare pin           255
        //9     spare pin           255
        //10    CRC

        public const byte NC = 255;
        public const int MaxSensors = 6;

        private const byte cByteCount = 11;
        private const byte HeaderHi = 126;
        private const byte HeaderLo = 251;

        private byte[] cBinPin = new byte[MaxSensors];
        private byte[] cDirPin = new byte[MaxSensors];
        private byte[] cFlowPin = new byte[MaxSensors];
        private bool[] cInvertBin = new bool[MaxSensors];
        private byte[] cPWMPin = new byte[MaxSensors];

        public PGN32507()
        {
            Load();
        }

        public byte BinPin(int Sensor)
        {
            byte Result = NC;
            if (ValidSensor(Sensor)) Result = cBinPin[Sensor];
            return Result;
        }

        public byte DirPin(int Sensor)
        {
            byte Result = NC;
            if (ValidSensor(Sensor)) Result = cDirPin[Sensor];
            return Result;
        }

        public byte FlowPin(int Sensor)
        {
            byte Result = NC;
            if (ValidSensor(Sensor)) Result = cFlowPin[Sensor];
            return Result;
        }

        public bool InvertBin(int Sensor)
        {
            bool Result = false;
            if (ValidSensor(Sensor)) Result = cInvertBin[Sensor];
            return Result;
        }

        public byte PWMPin(int Sensor)
        {
            byte Result = NC;
            if (ValidSensor(Sensor)) Result = cPWMPin[Sensor];
            return Result;
        }

        public void SetPins(int Sensor, byte Flow, byte Dir, byte PWM, byte Bin, bool Invert)
        {
            if (ValidSensor(Sensor))
            {
                cFlowPin[Sensor] = Flow;
                cDirPin[Sensor] = Dir;
                cPWMPin[Sensor] = PWM;
                cBinPin[Sensor] = Bin;
                cInvertBin[Sensor] = Invert;
            }
        }

        public void Load()
        {
            for (int i = 0; i < MaxSensors; i++)
            {
                cFlowPin[i] = LoadPin(i, "Flow");
                cDirPin[i] = LoadPin(i, "Dir");
                cPWMPin[i] = LoadPin(i, "PWM");
                cBinPin[i] = LoadPin(i, "Bin");

                cInvertBin[i] = false;
                if (bool.TryParse(Props.GetProp("SensorPins_" + i.ToString() + "_InvertBin"), out bool Inv))
                {
                    cInvertBin[i] = Inv;
                }
            }

            // migrate: profiles from before PGN 32507 hold the sensor 0/1 pins only in
            // the module config bytes (ModuleConfig_7..12) - seed from those once so
            // the 32700 mirror does not overwrite real pins with NC
            if (!byte.TryParse(Props.GetProp("SensorPins_0_Flow"), out _))
            {
                cFlowPin[0] = LoadLegacyPin(7);
                cDirPin[0] = LoadLegacyPin(8);
                cPWMPin[0] = LoadLegacyPin(9);
                cFlowPin[1] = LoadLegacyPin(10);
                cDirPin[1] = LoadLegacyPin(11);
                cPWMPin[1] = LoadLegacyPin(12);
            }
        }

        public void Save()
        {
            for (int i = 0; i < MaxSensors; i++)
            {
                Props.SetProp("SensorPins_" + i.ToString() + "_Flow", cFlowPin[i].ToString());
                Props.SetProp("SensorPins_" + i.ToString() + "_Dir", cDirPin[i].ToString());
                Props.SetProp("SensorPins_" + i.ToString() + "_PWM", cPWMPin[i].ToString());
                Props.SetProp("SensorPins_" + i.ToString() + "_Bin", cBinPin[i].ToString());
                Props.SetProp("SensorPins_" + i.ToString() + "_InvertBin", cInvertBin[i].ToString());
            }
        }

        public void Send()
        {
            try
            {
                // target module and sensor count come from the module config,
                // the single source for which module is being configured
                byte[] config = Core.ModuleConfig.GetData();
                byte ModuleID = config[2];
                int SensorCount = config[3];
                if (SensorCount > MaxSensors) SensorCount = MaxSensors;

                byte[] Data = new byte[cByteCount];
                for (int i = 0; i < SensorCount; i++)
                {
                    Data[0] = HeaderLo;
                    Data[1] = HeaderHi;
                    Data[2] = Core.Tls.BuildModSenID(ModuleID, (byte)i);
                    Data[3] = cFlowPin[i];
                    Data[4] = cDirPin[i];
                    Data[5] = cPWMPin[i];
                    Data[6] = cBinPin[i];
                    Data[7] = cInvertBin[i] ? (byte)1 : (byte)0;
                    Data[8] = NC;
                    Data[9] = NC;
                    Data[10] = Core.Tls.CRC(Data, cByteCount - 1);

                    if (Props.CanEnabled) Core.CanBridgeComm?.SendModuleCommand(Data);
                    Core.UDPmodules.Send(Data);
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("PGN32507/Send: " + ex.Message);
            }
        }

        private byte LoadLegacyPin(int ConfigByte)
        {
            byte Result = NC;
            if (byte.TryParse(Props.GetProp("ModuleConfig_" + ConfigByte.ToString()), out byte Val))
            {
                Result = Val;
            }
            return Result;
        }

        private byte LoadPin(int Sensor, string Field)
        {
            byte Result = NC;
            if (byte.TryParse(Props.GetProp("SensorPins_" + Sensor.ToString() + "_" + Field), out byte Val))
            {
                Result = Val;
            }
            return Result;
        }

        private bool ValidSensor(int Sensor)
        {
            return (Sensor >= 0 && Sensor < MaxSensors);
        }
    }
}
