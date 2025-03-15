using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.PGNs
{
    public class PGN10
    {
        // PGN 10, sensor info 1
        // 0	rate applied lo		1000 X actual
        // 1	rate mid
        // 2	rate hi
        // 3	pwm lo
        // 4	pwm hi
        // 5	accumulated quantiy X 10 byte lo
        // 6    acc. Mid
        // 7    acc. Hi

        private readonly clsProduct Prod;
        private double cElapsedTime;
        private double cPWMsetting;
        private double cQuantity;
        private double cUPM;
        private bool LastModuleReceiving;
        private bool LastModuleSending;
        private Stopwatch PGNstopWatch;
        private DateTime ReceiveTime;

        public PGN10(clsProduct CalledFrom)
        {
            Prod = CalledFrom;
            PGNstopWatch = new Stopwatch();
        }

        public double AccumulatedQuantity
        { get { return cQuantity; } }

        public double PWMsetting
        { get { return cPWMsetting; } }

        public double UPM
        { get { return cUPM; } }

        public bool Connected()
        {
            return ModuleReceiving() && ModuleSending();
        }

        public double ElapsedTime()
        {
            double Result = 0;
            if (ModuleSending()) Result = cElapsedTime;
            UpdateActivity();
            return Result;
        }

        public bool ModuleReceiving()
        {
            return Prod.RateSensorInfo2.SensorReceiving && ModuleSending();
        }

        public bool ModuleSending()
        {
            return ((DateTime.Now - ReceiveTime).TotalSeconds < 4);
        }

        public void ParseData(byte[] Data)
        {
            PGNstopWatch.Stop();
            cElapsedTime = PGNstopWatch.Elapsed.TotalMilliseconds;
            PGNstopWatch.Restart();

            cUPM = (Data[5] << 16 | Data[4] << 8 | Data[3]) / 1000.0;
            cQuantity = (Data[8] << 16 | Data[7] << 8 | Data[6]) / 10.0;
            cPWMsetting = (Int16)(Data[10] << 8 | Data[9]);  // need to cast to 16 bit integer to preserve the sign bit

            ReceiveTime = DateTime.Now;
            UpdateActivity();
        }

        public void UpdateActivity()
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
        }
    }
}