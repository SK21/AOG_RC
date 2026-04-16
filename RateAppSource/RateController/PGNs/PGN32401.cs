using RateController.Classes;
using System;
using System.Diagnostics;

namespace RateController.PGNs
{
    public class PGN32401
    {
        //PGN32401, module info from module to RC
        //0     145
        //1     126
        //2     module ID
        //3     Pressure Lo
        //4     Pressure Hi
        //5     wheel speed Lo  actual * 10
        //6     wheel speed Hi
        //7     wheel count Lo
        //8     wheel count mid
        //9     wheel count Hi
        //10    InoType
        //11    InoID lo
        //12    InoID hi
        //13    status
        //      bit 0   work switch
        //      bit 1   wifi rssi < -80
        //      bit 2	wifi rssi < -70
        //      bit 3	wifi rssi < -65
        //      bit 4   ethernet connected
        //      bit 5   good pin configuration
        //      bit 6   0 - 2 wire relays, 1 - 3 wire relays
        //14    CRC

        private const byte cByteCount = 15;
        private const byte HeaderHi = 126;
        private const byte HeaderLo = 145;
        private double[] cElapsedTime;
        private bool[] cEthernetConnected;
        private bool[] cGoodPins;
        private UInt16[] cInoID;
        private UInt16[] cInoType;
        private bool[] cIs3Wire;
        private double[] cPressureReading;
        private UInt16[] cWheelCounts;
        private double[] cWheelSpeed;
        private byte[] cWifiSignal;
        private bool[] cWorkSwitch;
        private ModuleStatus[] LastStatus;
        private Stopwatch[] PGNstopWatch;
        private DateTime[] ReceiveTime;

        public PGN32401()
        {
            cInoID = new ushort[Props.MaxModules];
            cWorkSwitch = new bool[Props.MaxModules];
            cPressureReading = new double[Props.MaxModules];
            ReceiveTime = new DateTime[Props.MaxModules];
            cWifiSignal = new byte[Props.MaxModules];
            cEthernetConnected = new bool[Props.MaxModules];
            cGoodPins = new bool[Props.MaxModules];

            cInoType = new ushort[Props.MaxModules];
            cWheelSpeed = new double[Props.MaxModules];
            cWheelCounts = new ushort[Props.MaxModules];
            cElapsedTime = new double[Props.MaxModules];

            PGNstopWatch = new Stopwatch[Props.MaxModules];
            for (int i = 0; i < Props.MaxModules; i++)
            {
                PGNstopWatch[i] = new Stopwatch();
            }

            cIs3Wire = new bool[Props.MaxModules];

            LastStatus = new ModuleStatus[Props.MaxModules];
            for (int i = 0; i < Props.MaxModules; i++)
            {
                LastStatus[i] = new ModuleStatus();
            }
        }

        public event EventHandler<PinStatusEventArgs> PinStatusChanged;

        public bool Connected(int Module)
        {
            return (ValidID(Module) && (DateTime.Now - ReceiveTime[Module]).TotalSeconds < 4);
        }

        public double ElapsedTime(int ModuleID)
        {
            double Result = 0;
            if (ValidID(ModuleID) && ModuleSending(ModuleID)) Result = cElapsedTime[ModuleID];
            return Result;
        }

        public bool Is3Wire(int Module)
        {
            return (ValidID(Module) && cIs3Wire[Module]);
        }

        public bool ModuleSending(int ModuleID)
        {
            return (ValidID(ModuleID) && (DateTime.Now - ReceiveTime[ModuleID]).TotalSeconds < 4);
        }

        public UInt16 ModuleType(int Module)
        {
            UInt16 Result = 0;
            if (ValidID(Module)) Result = cInoType[Module];
            return Result;
        }

        public string ModuleVersion(int Module)
        {
            string Result = "-";
            if (ValidID(Module)) Result = Props.ParseDate(cInoID[Module].ToString());
            return Result;
        }

        public bool ParseByteData(byte[] Data)
        {
            bool Result = false;
            try
            {
                if (Data[1] == HeaderHi && Data[0] == HeaderLo && Data.Length >= cByteCount && Core.Tls.GoodCRC(Data))
                {
                    byte ModuleID = Data[2];
                    if (ModuleID < Props.MaxModules)
                    {
                        PGNstopWatch[ModuleID].Stop();
                        cElapsedTime[ModuleID] = PGNstopWatch[ModuleID].Elapsed.TotalMilliseconds;
                        PGNstopWatch[ModuleID].Restart();

                        cPressureReading[ModuleID] = (double)(Data[3] | Data[4] << 8);

                        cWheelSpeed[ModuleID] = ((double)(Data[5] | Data[6] << 8)) / 10.0;

                        cWheelCounts[ModuleID] = (ushort)(Data[7] | Data[8] << 8 | Data[9] << 16);

                        cInoType[ModuleID] = Data[10];
                        cInoID[ModuleID] = (ushort)(Data[11] | Data[12] << 8);
                        cWorkSwitch[ModuleID] = ((Data[13] & 0b00000001) == 0b00000001);

                        // wifi strength
                        cWifiSignal[ModuleID] = 0;
                        if ((Data[13] & 0b00000010) == 0b00000010) cWifiSignal[ModuleID] = 1;
                        if ((Data[13] & 0b00000100) == 0b00000100) cWifiSignal[ModuleID] = 2;
                        if ((Data[13] & 0b00001000) == 0b00001000) cWifiSignal[ModuleID] = 3;

                        cEthernetConnected[ModuleID] = ((Data[13] & 0b00010000) == 0b00010000);
                        cGoodPins[ModuleID] = ((Data[13] & 0b00100000) == 0b00100000);
                        cIs3Wire[ModuleID] = ((Data[13] & 0b01000000) == 0b01000000);

                        ReceiveTime[ModuleID] = DateTime.Now;
                        Result = true;
                    }
                }
                UpdateActivity();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("PGN32401/ParseByteData: " + ex.Message);
            }
            return Result;
        }

        public double PressureReading(int Module)
        {
            double Result = 0;
            if (ValidID(Module)) Result = cPressureReading[Module];
            return Result;
        }

        public void UpdateActivity()
        {
            string Mes;
            for (int i = 0; i < Props.MaxModules; i++)
            {
                if (Connected(i))
                {
                    if (!LastStatus[i].Initialized || LastStatus[i].EthernetConnected != cEthernetConnected[i])
                    {
                        LastStatus[i].EthernetConnected = cEthernetConnected[i];
                        Mes = "Module " + i.ToString() + ", Ethernet connected: " + cEthernetConnected[i].ToString();
                        Props.WriteActivityLog(Mes, false, true);
                    }

                    if (!LastStatus[i].Initialized || LastStatus[i].WifiSignal != cWifiSignal[i])
                    {
                        LastStatus[i].WifiSignal = cWifiSignal[i];
                        Mes = "Module " + i.ToString() + ", Wifi Strength: " + cWifiSignal[i].ToString();
                        Props.WriteActivityLog(Mes, false, true);
                    }

                    // valve types
                    if (!LastStatus[i].Initialized || LastStatus[i].Is3Wire != cIs3Wire[i])
                    {
                        LastStatus[i].Is3Wire = cIs3Wire[i];
                        if (cIs3Wire[i])
                        {
                            Mes = "Module " + i.ToString() + ", Valves are 3 Wire.";
                        }
                        else
                        {
                            Mes = "Module " + i.ToString() + ", Valves are 2 Wire.";
                        }
                        Props.WriteActivityLog(Mes, false, true);
                    }

                    if (!LastStatus[i].Initialized || LastStatus[i].GoodPins != cGoodPins[i])
                    {
                        LastStatus[i].GoodPins = cGoodPins[i];
                        if (cGoodPins[i])
                        {
                            Mes = "Module " + i.ToString() + ", Pin Configuration correct.";
                        }
                        else
                        {
                            Mes = "Module " + i.ToString() + ", Pin Configuration not correct.";
                        }
                        Props.WriteActivityLog(Mes, false, true);

                        PinStatusEventArgs args = new PinStatusEventArgs();
                        args.GoodPins = cGoodPins[i];
                        args.Module = i;
                        PinStatusChanged?.Invoke(this, args);
                    }

                    LastStatus[i].Initialized = true;
                }
            }
        }

        public UInt16 WheelCounts(int Module)
        {
            UInt16 Result = 0;
            if (ValidID(Module)) Result = cWheelCounts[Module];
            return Result;
        }

        public double WheelSpeed(int Module)
        {
            double Result = 0;
            if (ValidID(Module)) Result = cWheelSpeed[Module];
            return Result;
        }

        public byte WifiStrength(int Module)
        {
            byte Result = 0;
            if (ValidID(Module)) Result = cWifiSignal[Module];
            return Result;
        }

        public bool WorkSwitchOn()
        {
            // returns true if any module workswitch is on
            bool Result = false;
            for (int i = 0; i < Props.MaxModules; i++)
            {
                if (Core.ModulesStatus.Connected(i))
                {
                    Result = cWorkSwitch[i];
                    if (Result) break;
                }
            }
            return Result;
        }

        private bool ValidID(int ModuleID)
        {
            return (ModuleID >= 0 && ModuleID < Props.MaxModules);
        }

        public class PinStatusEventArgs : EventArgs
        {
            public bool GoodPins { get; set; }
            public int Module { get; set; }
        }

        private class ModuleStatus
        {
            public bool EthernetConnected { get; set; }
            public bool GoodPins { get; set; }
            public bool Initialized { get; set; }
            public bool Is3Wire { get; set; }
            public byte WifiSignal { get; set; }
        }
    }
}