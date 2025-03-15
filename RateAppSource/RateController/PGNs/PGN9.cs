using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.PGNs
{
    public class PGN9
    {
        // PGN 9, module info
        // 0    pressure X 10, byte lo
        // 1    pressure hi
        // 2    InoID Lo
        // 3    InoID Hi
        // 4    Status
        //      bit 0   work switch
        //      bit 1   wifi rssi < -80
        //      bit 2	wifi rssi < -70
        //      bit 3	wifi rssi < -65
        //      bit 4   ethernet connected
        //      bit 5   good pin configuration

        private readonly FormStart mf;
        private bool[] cEthernetConnected;
        private bool[] cGoodPins;
        private UInt16[] cInoID;
        private double[] cPressure;
        private byte[] cWifiSignal;
        private bool[] cWorkSwitch;
        private bool[] EthernetConnectedLast;
        private bool[] GoodPinsLast;
        private DateTime[] ReceiveTime;
        private byte[] WifiSignalLast;

        public PGN9(FormStart CalledFrom)
        {
            mf = CalledFrom;
            cInoID = new ushort[mf.MaxModules];
            cWorkSwitch = new bool[mf.MaxModules];
            cPressure = new double[mf.MaxModules];
            ReceiveTime = new DateTime[mf.MaxModules];
            cWifiSignal = new byte[mf.MaxModules];
            cEthernetConnected = new bool[mf.MaxModules];
            cGoodPins = new bool[mf.MaxModules];
            EthernetConnectedLast = new bool[mf.MaxModules];
            WifiSignalLast = new byte[mf.MaxModules];
            GoodPinsLast = new bool[mf.MaxModules];
        }

        public event EventHandler<PinStatusEventArgs> PinStatusChanged;

        public bool Connected(int Module)
        {
            return ((DateTime.Now - ReceiveTime[Module]).TotalSeconds < 4);
        }

        public bool EthernetConnected(int Module)
        {
            return cEthernetConnected[Module];
        }

        public bool GoodPins(int Module)
        {
            return cGoodPins[Module];
        }

        public UInt16 InoID(int Module)
        {
            return cInoID[Module];
        }

        public void ParseData(byte ModuleID, byte[] Data)
        {
            cPressure[ModuleID] = (double)(Data[3] | Data[4] << 8);
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
            UpdateActivity();
        }

        public double Pressure(int Module)
        {
            return cPressure[Module];
        }

        public void UpdateActivity()
        {
            string Mes;
            for (int i = 0; i < mf.MaxModules; i++)
            {
                if (EthernetConnectedLast[i] != cEthernetConnected[i])
                {
                    EthernetConnectedLast[i] = cEthernetConnected[i];
                    Mes = "Module " + i.ToString() + ", Ethernet connected: " + cEthernetConnected[i].ToString();
                    mf.Tls.WriteActivityLog(Mes, false, true);
                }

                if (WifiSignalLast[i] != cWifiSignal[i])
                {
                    WifiSignalLast[i] = cWifiSignal[i];
                    Mes = "Module " + i.ToString() + ", Wifi Strength: " + cWifiSignal[i].ToString();
                    mf.Tls.WriteActivityLog(Mes, false, true);
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
                    mf.Tls.WriteActivityLog(Mes, false, true);

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
            for (int i = 0; i < mf.MaxModules; i++)
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