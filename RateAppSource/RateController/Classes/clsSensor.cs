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

        private readonly int cID;
        private readonly FormStart mf;
        private byte cBrakePoint;
        private byte cDeadband;
        private byte cKI;
        private byte cKP;
        private byte cMaxMotorIntegral;
        private byte cMaxPWM;
        private byte cMaxValveIntegral;
        private byte cMinPWM;
        private byte cModuleID;
        private string cName;
        private byte cPIDslowAdjust;
        private byte cPIDtime;
        private UInt16 cPulseMaxHz;
        private byte cPulseMinHz;
        private byte cPulseSampleSize;
        private byte cSlewRate;
        private UInt16 cTimedAdjust;
        private byte cTimedMinStart;
        private UInt16 cTimedPause;

        public clsSensor(FormStart main, int SensorID, int ModuleID)
        {
            mf = main;
            cID = SensorID;
            cModuleID = (byte)ModuleID;
            cName = "_Sensor" + cID.ToString() + "_M" + cModuleID.ToString();
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

        public int ID
        { get { return cID; } }

        public byte KI
        { get { return cKI; } set { cKI = value; } }

        public byte KP
        { get { return cKP; } set { cKP = value; } }

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
        { get { return cMaxPWM; } set { cMaxPWM = value; } }

        public byte MaxValveIntegral
        { get { return cMaxValveIntegral; } set { cMaxValveIntegral = value; } }

        public byte MinPWM
        { get { return cMinPWM; } set { cMinPWM = value; } }

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
        }

        public void Load()
        {
            if (byte.TryParse(Props.GetProp("BrakePoint" + cName), out byte T)) cBrakePoint = T;
            if (byte.TryParse(Props.GetProp("DeadBand" + cName), out byte sw)) cDeadband = sw;
            if (byte.TryParse(Props.GetProp("KP" + cName), out byte kp)) cKP = kp;
            if (byte.TryParse(Props.GetProp("KI" + cName), out byte ki)) cKI = ki;
            if (byte.TryParse(Props.GetProp("MaxMotorIntegral" + cName), out byte mi)) cMaxMotorIntegral = mi;
            if (byte.TryParse(Props.GetProp("MaxPWM" + cName), out byte pwm)) cMaxPWM = pwm;
        }

        public void Save()
        {
            // Should only be called from clsRelays. Need to run sub
            // BuildPowerRelays on change.
            Props.SetProp("RelayType" + cName, cType.ToString());
            Props.SetProp("RelaySection" + cName, cSectionID.ToString());
            Props.SetProp("RelaySwitch" + cName, cSwitchID.ToString());
        }

    }
}