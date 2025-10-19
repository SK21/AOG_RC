using System;

namespace RateController.Classes
{
    public class clsSensor
    {
        // MaxPWM           maximum pwm value
        // MinPWM           minimum pwm value
        // KP               proportional adjustment
        // KI               integral adjustment
        // Deadband         error % below which no adjustment is made, actual X 10
        // BrakePoint       error % where adjustment rate changes between 100% and the slow rate %
        // PIDslowAdjust    slow rate %
        // SlewRate         slew rate limit. Max total pwm change per loop. Used for motor only.
        // MaxIntegral max integral pwm change per loop. Ex: 0.1 = max 2 pwm/sec change at 50 ms sample time, actual X 10
        // TimedMinStart    minimum start ratio %. Used to quickly increase from 0 for a timed combo valve.
        // TimedAdjust      time in ms where there is adjustment of the combo valve.
        // TimedPause       time in ms where there is no adjustment of the combo valve.
        // PIDtime          time interval in ms the pid runs
        // PulseMinHz       minimum Hz of the flow sensor, actual X 10
        // PulseMaxHz       maximum Hz of the flow sensor
        // PulseSampeSize   number of pulses used to get the median Hz reading

        private readonly FormStart mf;
        private byte cBrakePoint;
        private byte cDeadband;
        private bool cIsNew = true;
        private byte cKI;
        private byte cKP;
        private byte cMaxIntegral;
        private byte cMaxPWM;
        private byte cMinPWM;
        private int cModuleID;
        private string cName;
        private byte cPIDslowAdjust;
        private byte cPIDtime;
        private UInt16 cPulseMaxHz;
        private byte cPulseMinHz;
        private byte cPulseSampleSize;
        private int cRecID;
        private int cSensorID;
        private byte cSlewRate;
        private UInt16 cTimedAdjust;
        private byte cTimedMinStart;
        private UInt16 cTimedPause;

        public clsSensor(FormStart main, int ID)
        {
            mf = main;
            cRecID = ID;
            cName = "RateSensor_" + cRecID.ToString() + "_";
            SetNewRecord();
            SetDefaults();
        }

        public byte BrakePoint
        {
            get { return cBrakePoint; }
            set
            {
                if (value >= 0 && value <= 75)
                {
                    cBrakePoint = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("BrakePoint");
                }
            }
        }

        public byte DeadBand
        {
            // actual X 10
            get { return cDeadband; }
            set
            {
                if (value >= 0 && value <= 100)
                {
                    cDeadband = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("DeadBand");
                }
            }
        }

        public bool IsNew
        { get { return cIsNew; } }

        public byte KI
        {
            get { return cKI; }
            set
            {
                if (value >= 0 && value <= 100)
                {
                    cKI = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("KI");
                }
            }
        }

        public byte KP
        {
            get { return cKP; }
            set
            {
                if (value >= 0 && value <= 100)
                {
                    cKP = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("KP");
                }
            }
        }

        public byte MaxIntegral
        {
            // actual X 10
            get { return cMaxIntegral; }
            set
            {
                if (value >= 0 && value <= 250)
                {
                    cMaxIntegral = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("MaxIntegral");
                }
            }
        }

        public byte MaxPWM
        {
            get { return cMaxPWM; }
            set
            {
                if (value >= 0 && value <= 100)
                {
                    cMaxPWM = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("MaxPWM");
                }
            }
        }

        public byte MinPWM
        {
            get { return cMinPWM; }
            set
            {
                if (value >= 0 && value <= 100)
                {
                    cMinPWM = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("MinPWM");
                }
            }
        }

        public int ModuleID
        {
            get { return cModuleID; }
        }

        public byte PIDslowAdjust
        {
            get { return cPIDslowAdjust; }
            set
            {
                if (value >= 0 && value <= 100)
                {
                    cPIDslowAdjust = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("PIDslowAdjust");
                }
            }
        }

        public byte PIDtime
        {
            get { return cPIDtime; }
            set
            {
                if (value >= 10 && value <= 250)
                {
                    cPIDtime = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("PIDtime");
                }
            }
        }

        public UInt16 PulseMaxHz
        {
            get { return cPulseMaxHz; }
            set
            {
                if (value >= 10 && value <= 10000)
                {
                    cPulseMaxHz = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("PulseMaxHz");
                }
            }
        }

        public byte PulseMinHz
        {
            // actual X 10
            get { return cPulseMinHz; }
            set
            {
                if (value >= 1)
                {
                    cPulseMinHz = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("PulseMinHz");
                }
            }
        }

        public byte PulseSampleSize
        {
            get { return cPulseSampleSize; }
            set
            {
                if (value >= 4 && value <= 30)
                {
                    cPulseSampleSize = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("PulseSampleSize");
                }
            }
        }

        public int SensorID
        {
            get { return cSensorID; }
        }

        public byte SlewRate
        {
            get { return cSlewRate; }
            set
            {
                if (value >= 1 && value <= 50)
                {
                    cSlewRate = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("SlewRate");
                }
            }
        }

        public UInt16 TimedAdjust
        {
            get { return cTimedAdjust; }
            set
            {
                if (value >= 20 && value <= 2000)
                {
                    cTimedAdjust = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("TimedAdjust");
                }
            }
        }

        public byte TimedMinStart
        {
            // actual X 100 (%)
            get { return cTimedMinStart; }
            set
            {
                if (value >= 0 && value <= 50)
                {
                    cTimedMinStart = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Timed Minimum Start");
                }
            }
        }

        public UInt16 TimedPause
        {
            get { return cTimedPause; }

            set
            {
                if (value >= 20 && value <= 2000)
                {
                    cTimedPause = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("TimedPause");
                }
            }
        }

        public bool Load()
        {
            bool Result = false;
            if (int.TryParse(Props.GetProp(cName + "SensorID"), out int sid)) cSensorID = sid;
            if (int.TryParse(Props.GetProp(cName + "ModuleID"), out int mn)) cModuleID = mn;
            cIsNew = (cSensorID == -1 || cModuleID == -1);
            if (!cIsNew)
            {
                if (byte.TryParse(Props.GetProp(cName + "BrakePoint"), out byte T)) cBrakePoint = T;
                if (byte.TryParse(Props.GetProp(cName + "DeadBand"), out byte sw)) cDeadband = sw;
                if (byte.TryParse(Props.GetProp(cName + "KP"), out byte kp)) cKP = kp;
                if (byte.TryParse(Props.GetProp(cName + "KI"), out byte ki)) cKI = ki;
                if (byte.TryParse(Props.GetProp(cName + "MaxIntegral"), out byte mi)) cMaxIntegral = mi;
                if (byte.TryParse(Props.GetProp(cName + "MaxPWM"), out byte pwm)) cMaxPWM = pwm;
                if (byte.TryParse(Props.GetProp(cName + "MinPWM"), out byte mp)) cMinPWM = mp;
                if (byte.TryParse(Props.GetProp(cName + "PIDslowAdjust"), out byte sa)) cPIDslowAdjust = sa;
                if (byte.TryParse(Props.GetProp(cName + "PIDtime"), out byte pt)) cPIDtime = pt;
                if (UInt16.TryParse(Props.GetProp(cName + "PulseMaxHz"), out UInt16 mz)) cPulseMaxHz = mz;
                if (byte.TryParse(Props.GetProp(cName + "PulseMinHz"), out byte minz)) cPulseMinHz = minz;
                if (byte.TryParse(Props.GetProp(cName + "PulseSampleSize"), out byte sz)) cPulseSampleSize = sz;
                if (byte.TryParse(Props.GetProp(cName + "SlewRate"), out byte sr)) cSlewRate = sr;
                if (UInt16.TryParse(Props.GetProp(cName + "TimedAdjust"), out UInt16 ta)) cTimedAdjust = ta;
                if (byte.TryParse(Props.GetProp(cName + "TimedMinStart"), out byte ms)) cTimedMinStart = ms;
                if (UInt16.TryParse(Props.GetProp(cName + "TimedPause"), out UInt16 tp)) cTimedPause = tp;
                Result = true;
            }
            return Result;
        }

        public void Save()
        {
            Props.SetProp(cName + "BrakePoint", cBrakePoint.ToString());
            Props.SetProp(cName + "DeadBand", cDeadband.ToString());
            Props.SetProp(cName + "KP", cKP.ToString());
            Props.SetProp(cName + "KI", cKI.ToString());
            Props.SetProp(cName + "MaxIntegral", cMaxIntegral.ToString());
            Props.SetProp(cName + "MaxPWM", cMaxPWM.ToString());
            Props.SetProp(cName + "MinPWM", cMinPWM.ToString());
            Props.SetProp(cName + "ModuleID", cModuleID.ToString());
            Props.SetProp(cName + "PIDslowAdjust", cPIDslowAdjust.ToString());
            Props.SetProp(cName + "PIDtime", cPIDtime.ToString());
            Props.SetProp(cName + "PulseMaxHz", cPulseMaxHz.ToString());
            Props.SetProp(cName + "PulseMinHz", cPulseMinHz.ToString());
            Props.SetProp(cName + "PulseSampleSize", cPulseSampleSize.ToString());
            Props.SetProp(cName + "SlewRate", cSlewRate.ToString());
            Props.SetProp(cName + "TimedAdjust", cTimedAdjust.ToString());
            Props.SetProp(cName + "TimedMinStart", cTimedMinStart.ToString());
            Props.SetProp(cName + "TimedPause", cTimedPause.ToString());
            Props.SetProp(cName + "SensorID", cSensorID.ToString());
        }

        public void SetDefaults()
        {
            cBrakePoint = Props.BrakePointDefault;
            cDeadband = Props.DeadbandDefault;
            cKI = Props.KIdefault;
            cKP = Props.KPdefault;
            cMaxIntegral = Props.MaxIntegralDefault;
            cMaxPWM = Props.MaxPWMdefault;
            cPulseMinHz = Props.PulseMinHzDefault;
            cMinPWM = Props.MinPWMdefault;
            cPIDslowAdjust = Props.PIDslowAdjustDefault;
            cPIDtime = Props.PIDtimeDefault;
            cPulseMaxHz = Props.PulseMaxHzDefault;
            cPulseMinHz = Props.PulseMinHzDefault;
            cPulseSampleSize = Props.PulseSampleSizeDefault;
            cSlewRate = Props.SlewRateDefault;
            cTimedAdjust = Props.TimedAdjustDefault;
            cTimedMinStart = Props.TimedMinStartDefault;
            cTimedPause = Props.TimedPauseDefault;
        }

        public bool SetModuleSensor(int Mod, int Sen, object Caller)
        {
            bool Result = false;
            if (Caller is clsSensors)
            {
                cSensorID = Sen;
                cModuleID = Mod;
                Result = true;
            }
            cIsNew = (cSensorID == -1 || cModuleID == -1);
            return Result;
        }

        private void SetNewRecord()
        {
            cModuleID = -1;
            cSensorID = -1;
            cIsNew = true;
        }
    }
}