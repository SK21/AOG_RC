using RateController.Classes;
using System;

namespace RateController
{
    public class PGN32401
    {
        //PGN32401, module info from module to RC
        //0     145
        //1     126
        //2     module ID
        //3     Pressure Lo
        //4     Pressure Hi
        //5     -
        //6     -
        //7     -
        //8     -
        //9     -
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
        //14    CRC

        private const byte cByteCount = 15;
        private const byte HeaderHi = 126;
        private const byte HeaderLo = 145;
        private readonly FormStart mf;
        private bool[] cEthernetConnected;
        private bool[] cGoodPins;
        private UInt16[] cInoID;
        private UInt16[] cInoType;
        private double[] cPressureReading;
        private byte[] cWifiSignal;
        private bool[] cWorkSwitch;
        private bool[] EthernetConnectedLast;
        private bool[] GoodPinsLast;
        private DateTime[] ReceiveTime;
        private byte[] WifiSignalLast;

        public PGN32401(FormStart CalledFrom)
        {
            mf = CalledFrom;
            cInoID = new ushort[Props.MaxModules];
            cWorkSwitch = new bool[Props.MaxModules];
            cPressureReading = new double[Props.MaxModules];
            ReceiveTime = new DateTime[Props.MaxModules];
            cWifiSignal = new byte[Props.MaxModules];
            cEthernetConnected = new bool[Props.MaxModules];
            cGoodPins = new bool[Props.MaxModules];

            EthernetConnectedLast = new bool[Props.MaxModules];
            WifiSignalLast = new byte[Props.MaxModules];
            GoodPinsLast = new bool[Props.MaxModules];

            cInoType = new ushort[Props.MaxModules];
        }

        public event EventHandler<PinStatusEventArgs> PinStatusChanged;

        public bool Connected(int Module)
        {
            return ((DateTime.Now - ReceiveTime[Module]).TotalSeconds < 4);
        }

        public UInt16 ModuleType(int Module)
        {
            return cInoType[Module];
        }

        public string ModuleVersion(int Module)
        {
            return Props.ParseDate(cInoID[Module].ToString());
        }

        public bool ParseByteData(byte[] Data)
        {
            bool Result = false;
            if (Data[1] == HeaderHi && Data[0] == HeaderLo && Data.Length >= cByteCount && mf.Tls.GoodCRC(Data))
            {
                byte ModuleID = Data[2];
                cPressureReading[ModuleID] = (double)(Data[3] | Data[4] << 8);
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

                ReceiveTime[ModuleID] = DateTime.Now;
                Result = true;
            }
            UpdateActivity();
            return Result;
        }

        public double PressureReading(int Module)
        {
            return cPressureReading[Module];
        }

        public void UpdateActivity()
        {
            string Mes;
            for (int i = 0; i < Props.MaxModules; i++)
            {
                if (EthernetConnectedLast[i] != cEthernetConnected[i])
                {
                    EthernetConnectedLast[i] = cEthernetConnected[i];
                    Mes = "Module " + i.ToString() + ", Ethernet connected: " + cEthernetConnected[i].ToString();
                    Props.WriteActivityLog(Mes, false, true);
                }

                if (WifiSignalLast[i] != cWifiSignal[i])
                {
                    WifiSignalLast[i] = cWifiSignal[i];
                    Mes = "Module " + i.ToString() + ", Wifi Strength: " + cWifiSignal[i].ToString();
                    Props.WriteActivityLog(Mes, false, true);
                }

                if (GoodPinsLast[i] != cGoodPins[i])
                {
                    GoodPinsLast[i] = cGoodPins[i];
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
            }
        }

        public byte WifiStrength(int Module)
        {
            return cWifiSignal[Module];
        }

        public bool WorkSwitchOn()
        {
            // returns true if any module workswitch is on
            bool Result = false;
            for (int i = 0; i < Props.MaxModules; i++)
            {
                if (mf.ModulesStatus.Connected(i))
                {
                    Result = cWorkSwitch[i];
                    if (Result) break;
                }
            }
            return Result;
        }

        public class PinStatusEventArgs : EventArgs
        {
            public bool GoodPins { get; set; }
            public int Module { get; set; }
        }
    }
}