using System;

namespace RateController.Classes
{
    public class clsSensor
    {
        // PIDtime          time interval in ms the pid runs
        // Deadband         error % below which no adjustment is made, actual X 100
        // BrakePoint       error % where adjustment rate changes between 100% and the slow rate %
        // PIDslowAdjust    slow rate %
        // SlewRate         slew rate limit. Max total pwm change per loop. Used for motor only.
        // MaxMotorIntegral max integral pwm change per loop. Ex: 0.1 = max 2 pwm/sec change at 50 ms sample time, actual X 10
        // MaxValveIntegral max total integral pwm change per loop for valve
        // KP               proportional adjustment
        // KI               integral adjustment
        // MaxPWM           maximum pwm value
        // MinPWM           minimum pwm value
        // TimedMinStart    minimum start ratio. Used to quickly increase from 0 for a timed combo valve. Actual X 100
        // TimedPause       time in ms where there is no adjustment of the combo valve.
        // TimedAdjust      time in ms where there is adjustment of the combo valve.
        // PulseMaxHz       maximum Hz of the flow sensor
        // PulseMinHz       minimum Hz of the flow sensor, actual X 10
        // PulseSampeSize   number of pulses used to get the median Hz reading
        // DirPin           uC GPIO pin for flow control direction (increase/decrease)
        // PWMPin           uc GPIO pin for rate of flow adjustment (0-255)
        // FlowPin          uc GPIO pin that receives the flow meter pulses

        private readonly FormStart mf;
        private byte cBrakePoint;
        private byte cDeadband;
        private byte cDirPin;
        private byte cFlowPin;
        private bool cIsNew = true;
        private byte cKI;
        private byte cKP;
        private byte cMaxMotorIntegral;
        private byte cMaxPWM;
        private byte cMaxValveIntegral;
        private byte cMinPWM;
        private int cModuleID;
        private string cName;
        private byte cPIDslowAdjust;
        private byte cPIDtime;
        private UInt16 cPulseMaxHz;
        private byte cPulseMinHz;
        private byte cPulseSampleSize;
        private byte cPWMPin;
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
            cName = "_RateSensor_" + cRecID.ToString();
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
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        public byte DeadBand
        {
            // actual X 100
            get { return cDeadband; }
            set
            {
                if (value >= 0 && value <= 50)
                {
                    cDeadband = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        public byte DirPin
        {
            get { return cDirPin; }
            set { cDirPin = value; }
        }

        public byte FlowPin
        {
            get { return cFlowPin; }
            set { cFlowPin = value; }
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
                    throw new ArgumentOutOfRangeException();
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
                    KP = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        public byte MaxMotorIntegral
        {
            // actual X 10
            get { return cMaxMotorIntegral; }
            set
            {
                if (value >= 0 && value <= 100)
                {
                    cMaxMotorIntegral = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
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
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        public byte MaxValveIntegral
        { get { return cMaxValveIntegral; } set { cMaxValveIntegral = value; } }

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
                    throw new ArgumentOutOfRangeException();
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
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        public byte PIDtime
        {
            get { return cPIDtime; }
            set
            {
                if (value >= 10 && value <= 100)
                {
                    cPIDtime = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
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
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        public byte PulseMinHz
        {
            // actual X 10
            get { return cPulseMinHz; }
            set
            {
                if (value >= 5)
                {
                    cPulseMinHz = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
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
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        public byte PWMPin
        {
            get { return cPWMPin; }
            set { cPWMPin = value; }
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
                if (value >= 1 && value <= 20)
                {
                    cSlewRate = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
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
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        public byte TimedMinStart
        {
            // actual X 100
            get { return cTimedMinStart; }
            set { cTimedMinStart = value; }
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
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        public bool Load()
        {
            bool Result = false;
            if (int.TryParse(Props.GetProp("SensorID" + cName), out int sid)) cSensorID = sid;
            if (int.TryParse(Props.GetProp("ModuleID" + cName), out int mn)) cModuleID = mn;
            cIsNew = (cSensorID == -1 || cModuleID == -1);
            if (!cIsNew)
            {
                if (byte.TryParse(Props.GetProp("BrakePoint" + cName), out byte T)) cBrakePoint = T;
                if (byte.TryParse(Props.GetProp("DeadBand" + cName), out byte sw)) cDeadband = sw;
                if (byte.TryParse(Props.GetProp("KP" + cName), out byte kp)) cKP = kp;
                if (byte.TryParse(Props.GetProp("KI" + cName), out byte ki)) cKI = ki;
                if (byte.TryParse(Props.GetProp("MaxMotorIntegral" + cName), out byte mi)) cMaxMotorIntegral = mi;
                if (byte.TryParse(Props.GetProp("MaxPWM" + cName), out byte pwm)) cMaxPWM = pwm;
                if (byte.TryParse(Props.GetProp("MaxValveIntegral" + cName), out byte mv)) cMaxValveIntegral = mv;
                if (byte.TryParse(Props.GetProp("MinPWM" + cName), out byte mp)) cMinPWM = mp;
                if (byte.TryParse(Props.GetProp("PIDslowAdjust" + cName), out byte sa)) cPIDslowAdjust = sa;
                if (byte.TryParse(Props.GetProp("PIDtime" + cName), out byte pt)) cPIDtime = pt;
                if (UInt16.TryParse(Props.GetProp("PulseMaxHz" + cName), out UInt16 mz)) cPulseMaxHz = mz;
                if (byte.TryParse(Props.GetProp("PulseMinHz" + cName), out byte minz)) cPulseMinHz = minz;
                if (byte.TryParse(Props.GetProp("PulseSampleSize" + cName), out byte sz)) cPulseSampleSize = sz;
                if (byte.TryParse(Props.GetProp("SlewRate" + cName), out byte sr)) cSlewRate = sr;
                if (UInt16.TryParse(Props.GetProp("TimedAdjust" + cName), out UInt16 ta)) cTimedAdjust = ta;
                if (byte.TryParse(Props.GetProp("TimedMinStart" + cName), out byte ms)) cTimedMinStart = ms;
                if (UInt16.TryParse(Props.GetProp("TimedPause" + cName), out UInt16 tp)) cTimedPause = tp;
                if (byte.TryParse(Props.GetProp("DirPin" + cName), out byte dir)) cDirPin = dir;
                if (byte.TryParse(Props.GetProp("PWMPin" + cName), out byte pp)) cPWMPin = pp;
                if (byte.TryParse(Props.GetProp("FlowPin" + cName), out byte Flow)) cFlowPin = Flow;
                Result = true;
            }
            return Result;
        }

        public void Save()
        {
            Props.SetProp("BrakePoint" + cName, cBrakePoint.ToString());
            Props.SetProp("DeadBand" + cName, cDeadband.ToString());
            Props.SetProp("KP" + cName, cKP.ToString());
            Props.SetProp("KI" + cName, cKI.ToString());
            Props.SetProp("MaxMotorIntegral" + cName, cMaxMotorIntegral.ToString());
            Props.SetProp("MaxPWM" + cName, cMaxPWM.ToString());
            Props.SetProp("MaxValveIntegral" + cName, cMaxValveIntegral.ToString());
            Props.SetProp("MinPWM" + cName, cMinPWM.ToString());
            Props.SetProp("ModuleID" + cName, cModuleID.ToString());
            Props.SetProp("PIDslowAdjust" + cName, cPIDslowAdjust.ToString());
            Props.SetProp("PIDtime" + cName, cPIDtime.ToString());
            Props.SetProp("PulseMaxHz" + cName, cPulseMaxHz.ToString());
            Props.SetProp("PulseMinHz" + cName, cPulseMinHz.ToString());
            Props.SetProp("PulseSampleSize" + cName, cPulseSampleSize.ToString());
            Props.SetProp("SlewRate" + cName, cSlewRate.ToString());
            Props.SetProp("TimedAdjust" + cName, cTimedAdjust.ToString());
            Props.SetProp("TimedMinStart" + cName, cTimedMinStart.ToString());
            Props.SetProp("TimedPause" + cName, cTimedPause.ToString());
            Props.SetProp("DirPin" + cName, cDirPin.ToString());
            Props.SetProp("PWMPin" + cName, cPWMPin.ToString());
            Props.SetProp("FlowPin" + cName, cFlowPin.ToString());
            Props.SetProp("SensorID" + cName, cSensorID.ToString());
        }

        public void SetDefaults()
        {
            cTimedAdjust = 80;
            cDeadband = 15;
            cKI = 0;
            cKP = 50;
            cMaxPWM = 100;
            cMaxMotorIntegral = 1;
            cMaxValveIntegral = 100;
            cMinPWM = 0;
            cTimedMinStart = 3;
            cTimedPause = 400;
            cPulseMaxHz = 4000;
            cPulseMinHz = 1;
            cPulseSampleSize = 12;
            cPIDtime = 50;
            cSlewRate = 6;
            cPIDslowAdjust = 30;
            cDirPin = 0;
            cFlowPin = 0;
            cPWMPin = 0;
            cModuleID = -1;
            cSensorID = -1;
            cIsNew = true;
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
    }
}