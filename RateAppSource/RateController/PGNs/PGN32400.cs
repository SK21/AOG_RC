using RateController.Classes;
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
        //12    Hz
        //13    -
        //14    CRC

        private const byte cByteCount = 15;
        private const byte HeaderHi = 126;
        private const byte HeaderLo = 144;
        private readonly clsProduct Prod;
        private double cElapsedTime;
        private byte cHz;
        private double cPWMsetting;
        private double cQuantity;
        private bool cSensorReceiving;
        private double cUPM;
        private bool LastModuleReceiving;
        private bool LastModuleSending;
        private Stopwatch PGNstopWatch;
        private DateTime ReceiveTime;

        public PGN32400(clsProduct CalledFrom)
        {
            Prod = CalledFrom;
            PGNstopWatch = new Stopwatch();
        }

        public double AccumulatedQuantity
        { get { return cQuantity; } }

        public byte Hz
        { get { return cHz; } }

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
            return cSensorReceiving && ModuleSending();
        }

        public bool ModuleSending()
        {
            return ((DateTime.Now - ReceiveTime).TotalSeconds < 4);
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
                        cSensorReceiving = ((Data[11] & 0b00000001) == 0b00000001);
                        cHz = Data[12];

                        ReceiveTime = DateTime.Now;
                        Result = true;
                    }
                }
            }
            UpdateActivity();
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

        public void UpdateActivity()
        {
            string Mes;
            if (LastModuleSending != ModuleSending())
            {
                LastModuleSending = ModuleSending();
                Mes = "Module:" + Prod.ModuleID + "  Sensor:" + Prod.SensorID + "  Sending: " + ModuleSending().ToString();

                Props.WriteActivityLog(Mes, false, true);
            }

            if (LastModuleReceiving != ModuleReceiving())
            {
                LastModuleReceiving = ModuleReceiving();
                Mes = "Module:" + Prod.ModuleID + "  Sensor:" + Prod.SensorID + "  Receiving: " + ModuleReceiving().ToString();

                Props.WriteActivityLog(Mes, false, true);
            }
        }
    }
}