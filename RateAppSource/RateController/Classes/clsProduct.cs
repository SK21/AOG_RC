using System;
using System.Diagnostics;

namespace RateController
{
    public enum ControlTypeEnum
    { Standard, FastClose, Motor, MotorWeights }

    public class clsProduct
    {
        public readonly FormStart mf;

        public PGN32613 ArduinoModule;
        public byte CoverageUnits = 0;
        public bool EraseAccumulatedUnits = false;
        public PGN32614 RateToArduino;
        public PGN32501 Scale;
        public double TankSize = 0;
        public clsArduino VirtualNano;
        private double AccumulatedUnits;
        private double CalPWMave;
        private double cCalEnd;
        private byte cCalPWM;
        private double cCalStart;
        private ControlTypeEnum cControlType = 0;
        private int cCountsRev;
        private bool cDebugArduino = false;
        private bool cDoCal;
        private double cHectaresPerMinute;
        private bool cLogRate;
        private byte cManualAdjust = 0;
        private double cMeterCal = 0;
        private double cMinUPM;
        private int cModID;
        private byte cOffRateSetting;
        private double Coverage = 0;
        private int cProductID;
        private string cProductName = "";
        private string cQuantityDescription = "Litres";
        private double cRateAlt = 100;
        private double cRateSet = 0;
        private double cScaleCountsCal;
        private double cScaleTare;

        private double cScaleUnitsCal;

        private int cSenID;
        private int cSerialPort;
        private SimType cSimulationType = 0;
        private double cTankStart = 0;
        private double cUnitsApplied = 0;
        private double CurrentMinutes;
        private double CurrentWorkedArea_Hc = 0;
        private bool cUseAltRate = false;
        private bool cUseMultiPulse;
        private bool cUseOffRateAlarm;
        private byte cVariableRate = 0;
        private byte cWifiStrength;
        private double cWorkingWidth_cm;
        private double LastAccQuantity = 0;
        private double LastScaleCounts = 0;
        private DateTime LastUpdateTime;
        private bool PauseWork = false;
        private PGN32616 PIDtoArduino;
        private DateTime ScaleCalTime;

        private double ScaleCountsStart;

        private bool SwitchIDsSent;
        private double UnitsOffset = 0;
        private DateTime UpdateStartTime;
        private byte[] VRconversion = { 255, 0, 1, 2, 3, 4 };   // 255 = off

        private double cSimSpeed;

        public clsProduct(FormStart CallingForm, int ProdID)
        {
            mf = CallingForm;
            cProductID = ProdID;
            cModID = 99;  // default other than 0
            PauseWork = true;

            ArduinoModule = new PGN32613(this);
            RateToArduino = new PGN32614(this);
            PIDtoArduino = new PGN32616(this);
            VirtualNano = new clsArduino(this);
            Scale = new PGN32501(this);
            cLogRate = true;
            cScaleUnitsCal = 1;
        }

        public double CalEnd
        {
            get { return cCalEnd; }
            set
            {
                if (cDoCal && value > cCalStart)
                {
                    cCalEnd = value;
                }
            }
        }

        public int CalPWM
        {
            get { return cCalPWM; }
            set
            {
                if (value < 0) cCalPWM = 0;
                else if (value > 255) cCalPWM = 255;
                else cCalPWM = (byte)value;
            }
        }

        public double SimSpeed
        {
            get { return cSimSpeed; }
            set
            {
                if(value>0 && value <20)
                {
                    cSimSpeed = value;
                    Debug.Print("New value for product "+ID.ToString()+" : " + cSimSpeed.ToString());
                }
            }
        }

        public double CalStart
        {
            get { return cCalStart; }
            set
            {
                if (cDoCal && value > 0)
                {
                    cCalStart = value;
                }
            }
        }

        public ControlTypeEnum ControlType
        {
            get { return cControlType; }
            set { cControlType = value; }
        }

        public int CountsRev
        {
            get { return cCountsRev; }
            set
            {
                if (value >= 0 && value < 10000)
                {
                    cCountsRev = value;
                }
            }
        }

        public bool DebugArduino
        { get { return cDebugArduino; } set { cDebugArduino = value; } }

        public bool DoCal
        {
            get { return cDoCal; }
            set
            {
                cDoCal = value;
                if (cDoCal)
                {
                    // reset on start
                    cCalStart = 0;
                    cCalEnd = 0;
                }
            }
        }

        public int ID
        { get { return cProductID; } }

        public bool LogRate
        { get { return cLogRate; } set { cLogRate = value; } }

        public byte ManualAdjust
        { get { return cManualAdjust; } set { cManualAdjust = value; } }

        public double MeterCal
        {
            get { return cMeterCal; }
            set
            {
                if (value > 0 && value < 10000)
                {
                    cMeterCal = value;
                }
            }
        }

        public double MinUPM
        {
            get { return cMinUPM; }
            set
            {
                if (value >= 0 & value < 1000)
                {
                    cMinUPM = value;
                }
                else
                {
                    throw new ArgumentException("Invalid value.");
                }
            }
        }

        public byte ModuleID
        {
            get { return (byte)cModID; }
            set
            {
                if (value < 16 || value == 99)
                {
                    if (mf.Products.UniqueModSen(value, cSenID, cProductID)) cModID = value;
                }
                else
                {
                    throw new ArgumentException("Invalid ModuleID.");
                }
            }
        }

        public byte OffRateSetting
        {
            get { return cOffRateSetting; }
            set
            {
                if (value >= 0 && value <= 40)
                {
                    cOffRateSetting = value;
                }
                else
                {
                    throw new ArgumentException("Invalid Off-rate setting.");
                }
            }
        }

        public byte PIDbrakepoint
        { get { return PIDtoArduino.BrakePoint; } set { PIDtoArduino.BrakePoint = value; } }

        public byte PIDdeadband
        { get { return PIDtoArduino.DeadBand; } set { PIDtoArduino.DeadBand = value; } }

        public byte PIDHighMax
        { get { return PIDtoArduino.HighMax; } set { PIDtoArduino.HighMax = value; } }

        public byte PIDki
        { get { return PIDtoArduino.KI; } set { PIDtoArduino.KI = value; } }

        public byte PIDkp
        { get { return PIDtoArduino.KP; } set { PIDtoArduino.KP = value; } }

        public byte PIDLowMax
        { get { return PIDtoArduino.LowMax; } set { PIDtoArduino.LowMax = value; } }

        public byte PIDminPWM
        { get { return PIDtoArduino.MinPWM; } set { PIDtoArduino.MinPWM = value; } }

        public byte PIDTimed
        { get { return PIDtoArduino.TimedAdjustment; } set { PIDtoArduino.TimedAdjustment = value; } }

        public string ProductName
        {
            get { return cProductName; }
            set
            {
                if (value.Length > 20)
                {
                    cProductName = value.Substring(0, 20);
                }
                else if (value.Length == 0)
                {
                    cProductName = Lang.lgProduct;
                }
                else
                {
                    cProductName = value;
                }
            }
        }

        public string QuantityDescription
        {
            get { return cQuantityDescription; }
            set
            {
                if (value.Length > 20)
                {
                    cQuantityDescription = value.Substring(0, 20);
                }
                else if (value.Length == 0)
                {
                    cQuantityDescription = "Litres";
                }
                else
                {
                    cQuantityDescription = value;
                }
            }
        }

        public double RateAlt
        { get { return cRateAlt; } set { cRateAlt = value; } }

        public double RateSet
        { get { return cRateSet; } set { cRateSet = value; } }

        public double ScaleCountsPerUnit
        {
            // (scale counts)/(unit of weight), ex: 100 counts per Lb.
            get { return cScaleUnitsCal; }
            set
            {
                if (value > 0)
                {
                    cScaleUnitsCal = value;
                }
            }
        }

        public double ScaleTare
        {
            get
            {
                if (cScaleUnitsCal > 0)
                {
                    return cScaleTare / cScaleUnitsCal;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (cScaleUnitsCal > 0 && value > 0 && value < 100000)
                {
                    cScaleTare = value * cScaleUnitsCal;
                }
            }
        }

        public byte SensorID
        {
            get { return (byte)cSenID; }
            set
            {
                if (value < 16)
                {
                    if (mf.Products.UniqueModSen(cModID, value, cProductID)) cSenID = value;
                }
                else
                {
                    throw new ArgumentException("Invalid SensorID.");
                }
            }
        }

        public int SerialPort
        {
            get { return cSerialPort; }
            set
            {
                if (value > -2 && value < 3)
                {
                    cSerialPort = value;
                }
            }
        }

        public SimType SimulationType
        { get { return cSimulationType; } set { cSimulationType = value; } }

        public double TankStart
        {
            get { return cTankStart; }
            set
            {
                if (value > 0 && value < 100000)
                {
                    cTankStart = value;
                }
            }
        }

        public bool UseAltRate
        { get { return cUseAltRate; } set { cUseAltRate = value; } }

        public bool UseMultiPulse
        { get { return cUseMultiPulse; } set { cUseMultiPulse = value; } }

        public bool UseOffRateAlarm
        { get { return cUseOffRateAlarm; } set { cUseOffRateAlarm = value; } }

        public byte VariableRate
        {
            get { return cVariableRate; }
            set
            {
                if (value < 6)
                {
                    cVariableRate = value;
                }
                else
                {
                    throw new ArgumentException("Invalid Variable Rate option.");
                }
            }
        }

        public byte WifiStrength
        {
            get { return cWifiStrength; }
            set
            {
                cWifiStrength = value;
            }
        }

        private string IDname
        { get { return cProductID.ToString(); } }

        public double AverageRate()
        {
            if (ArduinoModule.Connected() & mf.AutoSteerPGN.Connected()
                & cHectaresPerMinute > 0 & Coverage > 0)
            {
                return (cUnitsApplied / Coverage);
            }
            else
            {
                return 0;
            }
        }

        public string CoverageDescription()
        {
            return mf.CoverageDescriptions[CoverageUnits];
        }

        public double CurrentCoverage()
        {
            return Coverage;
        }

        public double CurrentRate()
        {
            if (ArduinoModule.Connected() & mf.AutoSteerPGN.Connected() & cHectaresPerMinute > 0)
            {
                return RateApplied();
            }
            else
            {
                return 0;
            }
        }

        public double CurrentWeight()
        {
            if (cScaleUnitsCal > 0)
            {
                return NetScaleCounts() / cScaleUnitsCal;
            }
            else
            {
                return 0;
            }
        }

        public void Load()
        {
            int tmp;
            byte val;

            double.TryParse(mf.Tls.LoadProperty("Coverage" + IDname), out Coverage);
            byte.TryParse(mf.Tls.LoadProperty("CoverageUnits" + IDname), out CoverageUnits);

            double.TryParse(mf.Tls.LoadProperty("TankStart" + IDname), out cTankStart);
            double.TryParse(mf.Tls.LoadProperty("QuantityApplied" + IDname), out cUnitsApplied);
            double.TryParse(mf.Tls.LoadProperty("LastAccQuantity" + IDname), out LastAccQuantity);

            cQuantityDescription = mf.Tls.LoadProperty("QuantityDescription" + IDname);
            if (cQuantityDescription == "") cQuantityDescription = "Litres";

            double.TryParse(mf.Tls.LoadProperty("RateSet" + IDname), out cRateSet);

            string StrVal = mf.Tls.LoadProperty("RateAlt" + IDname);
            if (StrVal != "") double.TryParse(StrVal, out cRateAlt);

            double.TryParse(mf.Tls.LoadProperty("FlowCal" + IDname), out cMeterCal);
            double.TryParse(mf.Tls.LoadProperty("TankSize" + IDname), out TankSize);
            Enum.TryParse(mf.Tls.LoadProperty("ValveType" + IDname), true, out cControlType);
            Enum.TryParse(mf.Tls.LoadProperty("cSimulationType" + IDname), true, out cSimulationType);

            cProductName = mf.Tls.LoadProperty("ProductName" + IDname);

            val = 0;
            byte.TryParse(mf.Tls.LoadProperty("KP" + IDname), out val);
            PIDtoArduino.KP = val;

            val = 0;
            byte.TryParse(mf.Tls.LoadProperty("KI" + IDname), out val);
            PIDtoArduino.KI = val;

            val = 0;
            byte.TryParse(mf.Tls.LoadProperty("PIDMinPWM" + IDname), out val);
            PIDtoArduino.MinPWM = val;

            val = 0;
            byte.TryParse(mf.Tls.LoadProperty("PIDLowMax" + IDname), out val);
            PIDtoArduino.LowMax = val;

            val = 0;
            byte.TryParse(mf.Tls.LoadProperty("PIDHighMax" + IDname), out val);
            PIDtoArduino.HighMax = val;

            val = 0;
            byte.TryParse(mf.Tls.LoadProperty("PIDdeadband" + IDname), out val);
            PIDtoArduino.DeadBand = val;

            val = 0;
            byte.TryParse(mf.Tls.LoadProperty("PIDbrakepoint" + IDname), out val);
            PIDtoArduino.BrakePoint = val;

            val = 0;
            byte.TryParse(mf.Tls.LoadProperty("TimedAdjustment" + IDname), out val);
            PIDtoArduino.TimedAdjustment = val;

            bool.TryParse(mf.Tls.LoadProperty("UseMultiPulse" + IDname), out cUseMultiPulse);
            int.TryParse(mf.Tls.LoadProperty("CountsRev" + IDname), out cCountsRev);

            int.TryParse(mf.Tls.LoadProperty("SensorID" + IDname), out tmp);
            SensorID = (byte)tmp;

            tmp = 0;
            int.TryParse(mf.Tls.LoadProperty("ModuleID" + IDname), out tmp);
            ModuleID = (byte)tmp;

            bool.TryParse(mf.Tls.LoadProperty("OffRateAlarm" + IDname), out cUseOffRateAlarm);
            byte.TryParse(mf.Tls.LoadProperty("OffRateSetting" + IDname), out cOffRateSetting);

            double.TryParse(mf.Tls.LoadProperty("MinUPM" + IDname), out cMinUPM);
            byte.TryParse(mf.Tls.LoadProperty("VariableRate" + IDname), out cVariableRate);

            tmp = 0;
            int.TryParse(mf.Tls.LoadProperty("SerialPort" + IDname), out tmp);
            cSerialPort = tmp;

            val = 0;
            byte.TryParse(mf.Tls.LoadProperty("CalPWM" + IDname), out val);
            cCalPWM = val;

            double.TryParse(mf.Tls.LoadProperty("ScaleUnitsCal" + IDname), out cScaleUnitsCal);
            double.TryParse(mf.Tls.LoadProperty("ScaleTare" + IDname), out cScaleTare);

            double.TryParse(mf.Tls.LoadProperty("SimSpeed" + IDname), out cSimSpeed);
        }

        public double PWM()
        {
            return ArduinoModule.PWMsetting();
        }

        public double RateApplied()
        {
            if (cHectaresPerMinute > 0)
            {
                double V = 0;
                switch (CoverageUnits)
                {
                    case 0:
                        // acres
                        V = ArduinoModule.UPM() / (cHectaresPerMinute * 2.47);
                        break;

                    case 1:
                        // hectares
                        V = ArduinoModule.UPM() / cHectaresPerMinute;
                        break;

                    case 2:
                        // minutes
                        V = ArduinoModule.UPM();
                        break;

                    default:
                        // hours
                        V = ArduinoModule.UPM() * 60;
                        break;
                }
                return V;
            }
            else
            {
                return 0;
            }
        }

        public void ResetApplied()
        {
            cUnitsApplied = 0;
            UnitsOffset = 0;
            EraseAccumulatedUnits = true;
            LastScaleCounts = NetScaleCounts();
        }

        public void ResetCoverage()
        {
            Coverage = 0;
            LastUpdateTime = DateTime.Now;
        }

        public void ResetTank()
        {
            cTankStart = TankSize;
        }

        public void Save()
        {
            mf.Tls.SaveProperty("Coverage" + IDname, Coverage.ToString());
            mf.Tls.SaveProperty("CoverageUnits" + IDname, CoverageUnits.ToString());

            mf.Tls.SaveProperty("TankStart" + IDname, cTankStart.ToString());
            mf.Tls.SaveProperty("QuantityApplied" + IDname, cUnitsApplied.ToString());
            mf.Tls.SaveProperty("LastAccQuantity" + IDname, LastAccQuantity.ToString());
            mf.Tls.SaveProperty("QuantityDescription" + IDname, cQuantityDescription);

            mf.Tls.SaveProperty("RateSet" + IDname, cRateSet.ToString());
            mf.Tls.SaveProperty("RateAlt" + IDname, cRateAlt.ToString());
            mf.Tls.SaveProperty("FlowCal" + IDname, cMeterCal.ToString());
            mf.Tls.SaveProperty("TankSize" + IDname, TankSize.ToString());
            mf.Tls.SaveProperty("ValveType" + IDname, cControlType.ToString());
            mf.Tls.SaveProperty("cSimulationType" + IDname, cSimulationType.ToString());

            mf.Tls.SaveProperty("ProductName" + IDname, cProductName);

            mf.Tls.SaveProperty("KP" + IDname, PIDtoArduino.KP.ToString());
            mf.Tls.SaveProperty("KI" + IDname, PIDtoArduino.KI.ToString());
            mf.Tls.SaveProperty("PIDMinPWM" + IDname, PIDtoArduino.MinPWM.ToString());
            mf.Tls.SaveProperty("PIDLowMax" + IDname, PIDtoArduino.LowMax.ToString());

            mf.Tls.SaveProperty("PIDHighMax" + IDname, PIDtoArduino.HighMax.ToString());
            mf.Tls.SaveProperty("PIDdeadband" + IDname, PIDtoArduino.DeadBand.ToString());
            mf.Tls.SaveProperty("PIDbrakepoint" + IDname, PIDtoArduino.BrakePoint.ToString());
            mf.Tls.SaveProperty("TimedAdjustment" + IDname, PIDtoArduino.TimedAdjustment.ToString());

            mf.Tls.SaveProperty("UseMultiPulse" + IDname, cUseMultiPulse.ToString());
            mf.Tls.SaveProperty("CountsRev" + IDname, cCountsRev.ToString());

            mf.Tls.SaveProperty("ModuleID" + IDname, cModID.ToString());
            mf.Tls.SaveProperty("SensorID" + IDname, cSenID.ToString());

            mf.Tls.SaveProperty("OffRateAlarm" + IDname, cUseOffRateAlarm.ToString());
            mf.Tls.SaveProperty("OffRateSetting" + IDname, cOffRateSetting.ToString());

            mf.Tls.SaveProperty("MinUPM" + IDname, cMinUPM.ToString());
            mf.Tls.SaveProperty("VariableRate" + IDname, cVariableRate.ToString());
            mf.Tls.SaveProperty("SerialPort" + IDname, cSerialPort.ToString());
            mf.Tls.SaveProperty("CalPWM" + IDname, cCalPWM.ToString());

            mf.Tls.SaveProperty("ScaleUnitsCal" + IDname, cScaleUnitsCal.ToString());
            mf.Tls.SaveProperty("ScaleTare" + IDname, cScaleTare.ToString());

            mf.Tls.SaveProperty("SimSpeed" + IDname, cSimSpeed.ToString());
        }

        public void SendPID()
        {
            PIDtoArduino.Send();
        }

        public bool SerialFromAruduino(string[] words, bool RealNano = true)
        {
            bool Result = false;    // return true if there is good comm
            try
            {
                if (RealNano & SimulationType == SimType.VirtualNano)
                {
                    // block PGN32613 from real nano when simulation is with virtual nano
                }
                else
                {
                    if (ArduinoModule.ParseStringData(words))
                    {
                        UpdateUnitsApplied();
                        Result = true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return Result;
        }

        public double SmoothRate()
        {
            if (ArduinoModule.Connected() & mf.AutoSteerPGN.Connected() & cHectaresPerMinute > 0)
            {
                if (TargetRate() > 0)
                {
                    double Rt = RateApplied() / TargetRate();

                    if (Rt >= .9 & Rt <= 1.1 & mf.SwitchBox.SwitchOn(SwIDs.Auto))
                    {
                        return TargetRate();
                    }
                    else
                    {
                        return RateApplied();
                    }
                }
                else
                {
                    return RateApplied();
                }
            }
            else
            {
                return 0;
            }
        }

        public double Speed()
        {
            if (cSimulationType == SimType.Speed)
            {
                return cSimSpeed;
            }
            else
            {
                if (mf.UseInches)
                {
                    return mf.AutoSteerPGN.Speed_KMH() * 0.621371;
                }
                else
                {
                    return mf.AutoSteerPGN.Speed_KMH();
                }
            }
        }

        private double KMH()
        {
            if (cSimulationType == SimType.Speed)
            {
                if (mf.UseInches)
                {
                    return cSimSpeed / 0.621371;
                }
                else
                {
                    return cSimSpeed;
                }
            }
            else
            {
                return mf.AutoSteerPGN.Speed_KMH();
            }
        }

        public double TargetRate()
        {
            double Result = cRateSet;
            if (UseAltRate) Result *= cRateAlt / 100;
            if (VRconversion[cVariableRate] < 5)
            {
                double Percent = mf.VRdata.Rate(VRconversion[cVariableRate]);
                if ((int)Percent != 255)
                {
                    Result = (double)Percent / 100.0 * cRateSet;
                }
            }
            return Result;
        }

        public double TargetUPM() // returns units per minute set rate
        {
            double V = 0;
            switch (CoverageUnits)
            {
                case 0:
                    // acres
                    V = TargetRate() * cHectaresPerMinute * 2.47;
                    break;

                case 1:
                    // hectares
                    V = TargetRate() * cHectaresPerMinute;
                    break;

                case 2:
                    // minutes
                    V = TargetRate();
                    break;

                default:
                    // hours
                    V = TargetRate() / 60;
                    break;
            }
            return V;
        }

        public void UDPcommFromArduino(byte[] data, int PGN)
        {
            try
            {
                if (SimulationType != SimType.VirtualNano)  // block pgns from real nano when simulation is with virtual nano
                {
                    switch (PGN)
                    {
                        case 32501:
                            Scale.ParseByteData(data);
                            break;

                        case 32613:
                            if (ArduinoModule.ParseByteData(data))
                            {
                                UpdateUnitsApplied();
                            }
                            break;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public string Units()
        {
            string s = cQuantityDescription + " / " + mf.CoverageAbbr[CoverageUnits];
            return s;
        }

        public double UnitsApplied()
        {
            return cUnitsApplied;
        }

        public void Update()
        {
            if (ArduinoModule.ModuleSending() && (mf.AutoSteerPGN.Connected() || CoverageUnits > 1)
                ||cSimulationType==SimType.Speed)
            {
                if (!SwitchIDsSent)
                {
                    // send switch IDs to each arduino when first connected
                    mf.SwitchIDs.Send();
                    SwitchIDsSent = true;
                }

                UpdateStartTime = DateTime.Now;
                CurrentMinutes = (UpdateStartTime - LastUpdateTime).TotalMinutes;
                LastUpdateTime = UpdateStartTime;

                if (CurrentMinutes < 0 | CurrentMinutes > 1 | PauseWork)
                {
                    CurrentMinutes = 0;
                    PauseWork = false;
                }

                // update worked area
                CurrentWorkedArea_Hc = 0;
                cWorkingWidth_cm = 0;

                for (int i = 0; i < 16; i++)
                {
                    if (mf.Sections.Item(i).IsON)
                    {
                        cWorkingWidth_cm += mf.Sections.Item(i).Width_cm;
                    }
                }

                cHectaresPerMinute = (cWorkingWidth_cm / 100.0) * KMH() / 600.0;
                CurrentWorkedArea_Hc = cHectaresPerMinute * CurrentMinutes;

                //coverage
                if (cHectaresPerMinute > 0)    // Is application on?
                {
                    switch (CoverageUnits)
                    {
                        case 0:
                            // acres
                            Coverage += CurrentWorkedArea_Hc * 2.47105;
                            break;

                        case 1:
                            // hectares
                            Coverage += CurrentWorkedArea_Hc;
                            break;

                        case 2:
                            // minutes
                            Coverage += CurrentMinutes;
                            break;

                        default:
                            // hours
                            Coverage += CurrentMinutes / 60;
                            break;
                    }
                }

                // send to arduino
                RateToArduino.Send();
                if (cLogRate) LogTheRate();
            }
            else
            {
                // connection lost
                PauseWork = true;
                cWifiStrength = 0;
            }

            if (cControlType == ControlTypeEnum.MotorWeights)
            {
                GetScaleCountsCal();
                UpdateUnitsApplied();
            }
        }

        public double UPMapplied()
        {
            double Result = 0;
            if (cControlType == ControlTypeEnum.MotorWeights)
            {
                if (cScaleUnitsCal > 0)
                {
                    if (cSimulationType == SimType.VirtualNano)
                    {
                        Result = mf.Tls.NoisyData(PWM() * MeterCal , VirtualNano.ErrorRange);
                    }
                    else
                    {
                        Result = PWM() * MeterCal;
                    }
                }
            }
            else
            {
                Result = ArduinoModule.UPM();
            }
            return Result;
        }

        public double UPMperPWM()
        {
            if (cScaleUnitsCal > 0)
            {
                return cScaleCountsCal / cScaleUnitsCal;
            }
            else
            {
                return 0;
            }
        }

        public double Width()
        {
            if (mf.UseInches)
            {
                return (cWorkingWidth_cm / 100) * 3.28;
            }
            else
            {
                return (cWorkingWidth_cm / 100);
            }
        }

        public double WorkRate()
        {
            if (mf.UseInches)
            {
                return cHectaresPerMinute * 2.47105 * 60;
            }
            else
            {
                return cHectaresPerMinute * 60;
            }
        }

        private void GetScaleCountsCal()
        {
            // (counts per minute)/PWM, 10 minute average
            if (cHectaresPerMinute > 0 && ScaleCountsStart > NetScaleCounts() && !PauseWork)
            {
                // 10 minute average pwm, (600 seconds)
                CalPWMave = (CalPWMave * 599.0 / 600.0) + (PWM() / 600.0);

                if ((DateTime.Now - ScaleCalTime).TotalMinutes > 10)
                {
                    // calculate 10 minute average metercal
                    double CountsApplied = (double)(ScaleCountsStart - NetScaleCounts()) / 10.0;
                    if (CalPWMave > 0) cScaleCountsCal = CountsApplied / CalPWMave;

                    ScaleCalTime = DateTime.Now;
                    ScaleCountsStart = NetScaleCounts();
                }
            }
            else
            {
                // reset
                ScaleCalTime = DateTime.Now;
                ScaleCountsStart = NetScaleCounts();
                CalPWMave = PWM();
                if (cScaleCountsCal == 0) cScaleCountsCal = cMeterCal * cScaleUnitsCal;
            }
        }

        private void LogTheRate()
        {
            double Target = TargetRate();
            double Applied = RateApplied();
            if (Target > 0 && Applied > 0)
            {
                double Ratio = Applied / Target;
                if (Ratio < 0.80 || Ratio > 1.20)
                {
                    string Mes = "Product: " + cProductID;
                    Mes += "\t Coverage: " + Coverage.ToString("N1");
                    Mes += "\t Target: " + Target.ToString("N1");
                    Mes += "\t Applied: " + Applied.ToString("N1");
                    Mes += "\t Ratio: " + Ratio.ToString("N2");
                    mf.Tls.WriteActivityLog(Mes);
                }
            }
        }

        private double NetScaleCounts()
        {
            if (Scale.Counts > cScaleTare)
            {
                return Scale.Counts - cScaleTare;
            }
            else
            {
                return 0;
            }
        }

        private void UpdateUnitsApplied()
        {
            if (ControlType == ControlTypeEnum.MotorWeights)
            {
                if (LastScaleCounts >= NetScaleCounts() && cScaleUnitsCal > 0)
                {
                    cUnitsApplied += (LastScaleCounts - NetScaleCounts()) / cScaleUnitsCal;
                    LastScaleCounts = NetScaleCounts();
                }
                else
                {
                    // reset

                    LastScaleCounts = NetScaleCounts();
                }
            }
            else
            {
                if (!EraseAccumulatedUnits)
                {
                    AccumulatedUnits = ArduinoModule.AccumulatedQuantity();
                    if ((AccumulatedUnits + UnitsOffset) < cUnitsApplied)
                    {
                        // account for arduino losing accumulated quantity, ex: power loss
                        UnitsOffset = cUnitsApplied - AccumulatedUnits;
                    }
                    cUnitsApplied = AccumulatedUnits + UnitsOffset;
                }
            }
        }
    }
}