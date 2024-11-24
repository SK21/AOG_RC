using System;
using System.Diagnostics;

namespace RateController
{
    public class PGN32400
    {
        //PGN32400, Sensor info from module to RC
        //0     HeaderLo    144
        //1     HeaderHi    126
        //2     Mod/Sen ID          0-15/0-15
        //3	    rate applied Lo 	1000 X actual
        //4     rate applied Mid
        //5	    rate applied Hi
        //6	    acc.Quantity Lo		10 X actual
        //7	    acc.Quantity Mid
        //8     acc.Quantity Hi
        //9     PWM Lo
        //10    PWM Hi
        //11    Status
        //      bit 0   sensor connected
        //12    CRC

        private const byte cByteCount = 13;
        private const byte HeaderHi = 126;
        private const byte HeaderLo = 144;
        private readonly clsProduct Prod;
        private double cElapsedTime;
        private bool cEthernetConnected;
        private bool cGoodPins;
        private byte cLastStrength;
        private bool cModuleIsReceivingData;
        private double cPWMsetting;
        private double cQuantity;
        private double cUPM;
        private byte cWifiStrength;
        private bool LastEthernetConnected;
        private bool LastGoodPins;
        private bool LastModuleReceiving;
        private bool LastModuleSending;
        private Stopwatch PGNstopWatch;
        private DateTime ReceiveTime;

        public PGN32400(clsProduct CalledFrom)
        {
            Prod = CalledFrom;
            PGNstopWatch = new Stopwatch();
        }

        public event EventHandler<PinStatusArgs> PinStatusChanged;

        public double AccumulatedQuantity
        { get { return cQuantity; } }

        public bool GoodPins
        { get { return cGoodPins; } }

        public double PWMsetting
        { get { return cPWMsetting; } }

        public double UPM
        { get { return cUPM; } }

        public void CheckModuleComm()
        {
            string Mes;
            if (LastModuleSending != ModuleSending())
            {
                LastModuleSending = ModuleSending();
                Mes = "Module:" + Prod.ModuleID + "  Sensor:" + Prod.SensorID + "  Sending: " + ModuleSending().ToString();

                Prod.mf.Tls.WriteActivityLog(Mes, false, true);
            }

            if (LastModuleReceiving != ModuleReceiving())
            {
                LastModuleReceiving = ModuleReceiving();
                Mes = "Module:" + Prod.ModuleID + "  Sensor:" + Prod.SensorID + "  Receiving: " + ModuleReceiving().ToString();

                Prod.mf.Tls.WriteActivityLog(Mes, false, true);
            }

            if (LastEthernetConnected != cEthernetConnected)
            {
                LastEthernetConnected = cEthernetConnected;
                Mes = "Ethernet connected: " + cEthernetConnected.ToString();
                Prod.mf.Tls.WriteActivityLog(Mes, false, true);
            }

            if (cLastStrength != cWifiStrength)
            {
                cLastStrength = cWifiStrength;
                Mes = "Wifi Strength: " + cWifiStrength.ToString();
                Prod.mf.Tls.WriteActivityLog(Mes, false, true);
            }

            if (LastGoodPins != cGoodPins)
            {
                LastGoodPins = cGoodPins;
                if (cGoodPins)
                {
                    Mes = "Pin Configuration correct.";
                }
                else
                {
                    Mes = "Pin Configuration not correct.";
                }
                Prod.mf.Tls.WriteActivityLog(Mes, false, true);

                PinStatusArgs args = new PinStatusArgs();
                args.GoodPins = cGoodPins;
                PinStatusChanged?.Invoke(this, args);
            }
        }

        public bool Connected()
        {
            return ModuleReceiving() && ModuleSending();
        }

        public double ElapsedTime()
        {
            double Result = 0;
            if (ModuleSending()) Result = cElapsedTime;
            CheckModuleComm();
            return Result;
        }

        public bool ModuleReceiving()
        {
            if (Prod.mf.SimMode == SimType.VirtualNano)
            {
                return true;
            }
            else
            {
                return cModuleIsReceivingData && ModuleSending();
            }
        }

        public bool ModuleSending()
        {
            if (Prod.mf.SimMode == SimType.VirtualNano)
            {
                return true;
            }
            else
            {
                return ((DateTime.Now - ReceiveTime).TotalSeconds < 4);
            }
        }

        public bool ParseByteData(byte[] Data)
        {
            bool Result = false;

            if (Data[1] == HeaderHi && Data[0] == HeaderLo &&
                Data.Length >= cByteCount && Prod.mf.Tls.GoodCRC(Data))
            {
                int tmp = Prod.mf.Tls.ParseModID(Data[2]);
                if (Prod.ModuleID == tmp)
                {
                    tmp = Prod.mf.Tls.ParseSenID(Data[2]);
                    if (Prod.SensorID == tmp)
                    {
                        PGNstopWatch.Stop();
                        cElapsedTime = PGNstopWatch.Elapsed.TotalMilliseconds;
                        PGNstopWatch.Restart();

                        cUPM = (Data[5] << 16 | Data[4] << 8 | Data[3]) / 1000.0;
                        cQuantity = (Data[8] << 16 | Data[7] << 8 | Data[6]) / 10.0;
                        cPWMsetting = (Int16)(Data[10] << 8 | Data[9]);  // need to cast to 16 bit integer to preserve the sign bit

                        // status
                        if (tmp == 0)
                        {
                            // sensor 0
                            cModuleIsReceivingData = ((Data[11] & 0b00000001) == 0b00000001);
                        }
                        else
                        {
                            // sensor 1
                            cModuleIsReceivingData = ((Data[11] & 0b00000010) == 0b00000010);
                        }

                        // wifi strength
                        cWifiStrength = 0;
                        if ((Data[11] & 0b00000100) == 0b00000100) cWifiStrength = 1;
                        if ((Data[11] & 0b00001000) == 0b00001000) cWifiStrength = 2;
                        if ((Data[11] & 0b00010000) == 0b00010000) cWifiStrength = 3;
                        Prod.WifiStrength = cWifiStrength;

                        //HzOnly = ((Data[11] & 0b00100000) == 0b00100000);
                        cEthernetConnected = ((Data[11] & 0b01000000) == 0b01000000);
                        cGoodPins = ((Data[11] & 0b10000000) == 0b10000000);

                        ReceiveTime = DateTime.Now;
                        Result = true;
                    }
                }
            }
            CheckModuleComm();
            return Result;
        }

        public bool ParseStringData(string[] Data)
        {
            bool Result = false;
            byte[] BD;
            if (Data.Length < 100)
            {
                BD = new byte[Data.Length];
                for (int i = 0; i < Data.Length; i++)
                {
                    byte.TryParse(Data[i], out BD[i]);
                }
                Result = ParseByteData(BD);
            }
            return Result;
        }

        public class PinStatusArgs : EventArgs
        {
            public bool GoodPins { get; set; }
        }
    }
}