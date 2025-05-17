using RateController.Classes;
using RateController.Language;
using System;
using System.Diagnostics;

namespace RateController
{
    public class clsProduct
    {
        public readonly FormStart mf;

        public byte CoverageUnits = 0;
        public PGN32500 ModuleRateSettings;
        public PGN32400 RateSensor;
        private double AccumulatedLast = 0;
        private ApplicationMode cAppMode = ApplicationMode.ControlledUPM;
        private bool cBumpButtons;
        private bool cCalRun;
        private bool cCalSetMeter;
        private bool cCalUseBaseRate;
        private ControlTypeEnum cControlType = 0;
        private int cCountsRev;
        private bool cEnabled = true;
        private bool cEnableProdDensity = false;
        private bool cFanOn;
        private double cHectaresPerMinute;
        private double cHours1;
        private double cHours2;
        private int cManualPWM;
        private int cMaxAdjust;
        private int cIntegral;
        private double cMeterCal = 0;
        private int cMinAdjust;
        private double cMinUPM;
        private double cMinUPMbySpeed;
        private int cModID;
        private byte cOffRateSetting;
        private bool cOnScreen;
        private double Coverage = 0;
        private double Coverage2 = 0;
        private double cProdDensity = 0;
        private int cProductID;
        private string cProductName = "";
        private string cQuantityDescription = "Lbs";
        private double cRateAlt = 100;
        private double cRateSet = 0;
        private int cScalingFactor;
        private int cSenID;
        private int cSerialPort;
        private int cShiftRange = 4;
        private double cTankSize = 0;
        private double cTankStart = 0;
        private double cUnitsApplied = 0;
        private double cUnitsApplied2 = 0;
        private double CurrentWorkedArea_Hc = 0;
        private bool cUseAltRate = false;
        private bool cUseMinUPMbySpeed = false;
        private bool cUseOffRateAlarm;
        private PGN32502 ModuleControlSettings;
        private Stopwatch UpdateStopWatch;

        public clsProduct(FormStart CallingForm, int ProdID)
        {
            mf = CallingForm;
            cProductID = ProdID;
            cModID = ProdID / 2;
            cSenID = (byte)(ProdID % 2);

            RateSensor = new PGN32400(this);
            ModuleRateSettings = new PGN32500(this);
            ModuleControlSettings = new PGN32502(this);

            if (cProductID > Props.MaxProducts - 3)
            {
                cControlType = ControlTypeEnum.Fan;
                ProductName = "fan";
            }

            UpdateStopWatch = new Stopwatch();
        }

        public ApplicationMode AppMode
        {
            get { return cAppMode; }
            set
            { cAppMode = value; }
        }

        public bool BumpButtons
        {
            get { return cBumpButtons; }
            set { cBumpButtons = value; }
        }

        public bool CalRun
        {
            // notifies module Master switch on for calibrate and use current meter cal in manual mode
            // current meter position is used and not adjusted

            get { return cCalRun; }
            set
            {
                cCalRun = value;
                if (cCalRun) cCalSetMeter = false;
            }
        }

        public bool CalSetMeter
        {
            // notifies module Master switch on for calibrate and use auto mode to find meter cal
            // adjusts meter position to match base rate

            get { return cCalSetMeter; }
            set
            {
                cCalSetMeter = value;
                if (cCalSetMeter) cCalRun = false;
            }
        }

        public bool CalUseBaseRate
        {
            // use base rate for cal and not vr rate
            get { return cCalUseBaseRate; }
            set { cCalUseBaseRate = value; }
        }

        public ControlTypeEnum ControlType
        {
            get
            {
                if (cProductID > Props.MaxProducts - 3)
                {
                    return ControlTypeEnum.Fan;
                }
                else
                {
                    return cControlType;
                }
            }
            set
            {
                if (cProductID > Props.MaxProducts - 3)
                {
                    cControlType = ControlTypeEnum.Fan;
                }
                else
                {
                    cControlType = value;
                }
            }
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

        public double ElapsedTime
        { get { return RateSensor.ElapsedTime(); } }

        public bool Enabled
        {
            get { return cEnabled; }
            set
            {
                cEnabled = value;
            }
        }

        public bool EnableProdDensity
        { get { return cEnableProdDensity; } set { cEnableProdDensity = value; } }

        public bool EraseAccumulatedUnits { get; set; }

        public bool FanOn
        {
            get { return cFanOn; }
            set
            {
                cFanOn = value;
            }
        }


        public double Hours1
        { get { return cHours1; } }

        public double Hours2
        { get { return cHours2; } }

        public int ID
        { get { return cProductID; } }


        public int ManualPWM
        {
            get { return cManualPWM; }
            set
            {
                if (cControlType == ControlTypeEnum.Valve || cControlType == ControlTypeEnum.ComboClose
                    || ControlType == ControlTypeEnum.ComboCloseTimed)
                {
                    if (value < -255) cManualPWM = -255;
                    else if (value > 255) cManualPWM = 255;
                    else cManualPWM = value;
                }
                else
                {
                    if (value < 0) cManualPWM = 0;
                    else if (value > 255) cManualPWM = 255;
                    else cManualPWM = (byte)value;
                }
            }
        }

        public int MaxAdjust
        {
            get { return cMaxAdjust; }
            set
            {
                if (value >= 0 && value <= 100) cMaxAdjust = value;
            }
        }
        public int Integral
        {
            get { return cIntegral; }
            set
            {
                if (value >= 0 && value <= 100) cIntegral = value;
            }
        }

        public double MeterCal
        {
            get { return cMeterCal; }
            set
            {
                if (value > 0 && value < 16701)
                {
                    cMeterCal = value;
                }
            }
        }

        public int MinAdjust
        {
            get { return cMinAdjust; }
            set
            {
                if (value >= 0 && value <= 100) cMinAdjust = value;
            }
        }

        public double MinUPM
        {
            get { return cMinUPM; }
            set
            {
                if (value >= 0 && value < 1000)
                {
                    cMinUPM = value;
                }
                else
                {
                    throw new ArgumentException("Invalid value.");
                }
            }
        }

        public double MinUPMbySpeed
        {
            get { return cMinUPMbySpeed; }
            set
            {
                if (value >= 0 && value < 50)
                {
                    cMinUPMbySpeed = value;
                }
                else
                {
                    throw new ArgumentException("Invalid value.");
                }
            }
        }

        public int ModuleID
        {
            get { return (byte)cModID; }
            set
            {
                if (value > -1 && value < 255)
                {
                    if (mf.Products.UniqueModSen(value, cSenID, cProductID))
                    {
                        cModID = value;
                    }
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

        public bool OnScreen
        {
            get { return cOnScreen; }
            set { cOnScreen = value; }
        }

        public double ProdDensity
        { get { return cProdDensity; } set { cProdDensity = value; } }

        public string ProductName
        {
            get
            {
                if (cControlType == ControlTypeEnum.Fan)
                {
                    int tmp = 3 - (Props.MaxProducts - cProductID);
                    cProductName = "Fan " + tmp.ToString();
                }
                return cProductName;
            }
            set
            {
                if (cControlType == ControlTypeEnum.Fan)
                {
                    int tmp = 3 - (Props.MaxProducts - cProductID);
                    cProductName = "Fan " + tmp.ToString();
                }
                else
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
                    cQuantityDescription = "Lbs";
                }
                else
                {
                    cQuantityDescription = value;
                }
            }
        }

        public double RateAlt
        {
            get { return cRateAlt; }
            set
            {
                if (value >= 0 && value < 151) cRateAlt = value;
            }
        }

        public double RateSet
        {
            get { return cRateSet; }
            set
            {
                if (value >= 0 && value < 50001) cRateSet = value;
            }
        }

        public int ScalingFactor
        {
            get { return cScalingFactor; }
            set
            {
                if (value >= 0 && value <= 100) cScalingFactor = value;
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

        public double TankSize
        {
            get { return cTankSize; }
            set { cTankSize = value; }
        }

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

        public bool UseMinUPMbySpeed
        {
            get { return cUseMinUPMbySpeed; }
            set { cUseMinUPMbySpeed = value; }
        }

        public bool UseOffRateAlarm
        { get { return cUseOffRateAlarm; } set { cUseOffRateAlarm = value; } }

        private string IDname
        { get { return cProductID.ToString(); } }

        public double AverageRate()
        {
            if (ProductOn() && Coverage > 0)
            {
                double V = (cUnitsApplied / Coverage);
                if (cEnableProdDensity && cProdDensity > 0) V *= cProdDensity;
                return V;
            }
            else
            {
                return 0;
            }
        }

        public bool ChangeID(int ModID, int SenID, bool Override = false)
        {
            bool Result = false;
            if (Override)
            {
                cModID = ModID;
                cSenID = SenID;
                Result = true;
            }
            else if (ModID > -1 && ModID < Props.MaxModules && SenID > -1 && SenID < Props.MaxSensors)
            {
                if (mf.Products.UniqueModSen(ModID, SenID, cProductID))
                {
                    cModID = ModID;
                    cSenID = SenID;
                    Result = true;
                }
            }
            return Result;
        }

        public string CoverageDescription()
        {
            return mf.CoverageDescriptions[CoverageUnits];
        }

        public double CurrentCoverage()
        {
            return Coverage;
        }

        public double CurrentCoverage2()
        {
            return Coverage2;
        }

        public double CurrentRate()
        {
            if (ProductOn())
            {
                double V = RateApplied();
                if (cEnableProdDensity && cProdDensity > 0) V *= cProdDensity;
                return V;
            }
            else
            {
                return 0;
            }
        }

        public bool IsNew()
        {
            bool Result = true;
            if (bool.TryParse(Props.GetProp("IsNew" + IDname), out bool nw)) Result = nw;
            return Result;
        }

        public void Load()
        {
            int tmp;

            double.TryParse(Props.GetProp("Coverage" + IDname), out Coverage);
            double.TryParse(Props.GetProp("Coverage2" + IDname), out Coverage2);
            byte.TryParse(Props.GetProp("CoverageUnits" + IDname), out CoverageUnits);

            double.TryParse(Props.GetProp("TankStart" + IDname), out cTankStart);
            double.TryParse(Props.GetProp("QuantityApplied" + IDname), out cUnitsApplied);
            double.TryParse(Props.GetProp("QuantityApplied2" + IDname), out cUnitsApplied2);

            if (double.TryParse(Props.GetProp("AccumulatedLast" + IDname), out double oa)) AccumulatedLast = oa;

            cQuantityDescription = Props.GetProp("QuantityDescription" + IDname);
            if (cQuantityDescription == "") cQuantityDescription = "Lbs";

            double.TryParse(Props.GetProp("RateSet" + IDname), out cRateSet);
            if (cRateSet < 0 || cRateSet > 50000) cRateSet = 0;

            double.TryParse(Props.GetProp("RateAlt" + IDname), out cRateAlt);

            double.TryParse(Props.GetProp("cProdDensity" + IDname), out cProdDensity);
            bool.TryParse(Props.GetProp("cEnableProdDensity" + IDname), out cEnableProdDensity);

            double.TryParse(Props.GetProp("FlowCal" + IDname), out cMeterCal);
            if (double.TryParse(Props.GetProp("TankSize" + IDname), out double ts)) cTankSize = ts;

            cProductName = Props.GetProp("ProductName" + IDname);

            int.TryParse(Props.GetProp("CountsRev" + IDname), out cCountsRev);

            int tmpModuleID = -1;
            if (int.TryParse(Props.GetProp("ModuleID" + IDname), out int tmp1)) tmpModuleID = tmp1;
            int.TryParse(Props.GetProp("SensorID" + IDname), out int tmp2);
            ChangeID(tmpModuleID, tmp2);

            bool.TryParse(Props.GetProp("OffRateAlarm" + IDname), out cUseOffRateAlarm);
            byte.TryParse(Props.GetProp("OffRateSetting" + IDname), out cOffRateSetting);

            double.TryParse(Props.GetProp("MinUPM" + IDname), out cMinUPM);
            double.TryParse(Props.GetProp("MinUPMbySpeed" + IDname), out cMinUPMbySpeed);
            if (bool.TryParse(Props.GetProp("UseMinUPMbySpeed" + IDname), out bool ms)) cUseMinUPMbySpeed = ms;

            tmp = 0;
            int.TryParse(Props.GetProp("SerialPort" + IDname), out tmp);
            cSerialPort = tmp;

            tmp = 0;
            int.TryParse(Props.GetProp("ManualPWM" + IDname), out tmp);
            cManualPWM = tmp;

            if (ID > Props.MaxProducts - 3)
            {
                cControlType = ControlTypeEnum.Fan;
            }
            else
            {
                Enum.TryParse(Props.GetProp("ValveType" + IDname), true, out cControlType);
            }

            if (bool.TryParse(Props.GetProp("OnScreen" + IDname), out bool OS))
            {
                cOnScreen = OS;
            }
            else
            {
                cOnScreen = true;
            }

            if (bool.TryParse(Props.GetProp("BumpButtons" + IDname), out bool BB))
            {
                cBumpButtons = BB;
            }
            else
            {
                cBumpButtons = false;
            }

            if (int.TryParse(Props.GetProp("ShiftRange" + IDname), out int sr)) cShiftRange = sr;
            if (double.TryParse(Props.GetProp("Hours1" + IDname), out double h1)) cHours1 = h1;
            if (double.TryParse(Props.GetProp("Hours2" + IDname), out double h2)) cHours2 = h2;

            if (Enum.TryParse(Props.GetProp("AppMode" + IDname), true, out ApplicationMode am)) cAppMode = am;

            if (int.TryParse(Props.GetProp("MaxAdjust" + IDname), out int ma)) cMaxAdjust = ma;
            if (int.TryParse(Props.GetProp("MinAdjust" + IDname), out int mina)) cMinAdjust = mina;
            if (int.TryParse(Props.GetProp("Scaling" + IDname), out int sc)) cScalingFactor = sc;

            if (int.TryParse(Props.GetProp("Integral" + IDname), out int IG)) cIntegral = IG;
        }

        public void LoadDefaultControlSettings()
        {
            cMaxAdjust = Props.MaxAdjustDefault;
            cMinAdjust = Props.MinAdjustDefault;
            cScalingFactor = Props.ScalingDefault;
            cIntegral = Props.IntegralDefault;
        }

        public double MinUPMinUse()
        {
            double Result = cMinUPM;
            if (cUseMinUPMbySpeed)
            {
                double KPH = cMinUPMbySpeed;
                if (!Props.UseMetric) KPH *= Props.MPHtoKPH;
                double HPM = mf.Sections.TotalWidth(false) * KPH / 600.0;   // hectares per minute
                Result = TargetRate() * HPM;
                if (CoverageUnits == 0) Result *= 2.47;
            }
            return Result;
        }

        public double Pulses()
        {
            return cUnitsApplied * cMeterCal;
        }

        public double PWM()
        {
            return RateSensor.PWMsetting;
        }
        public double Hz()
        {
            return RateSensor.Hz;
        }

        public double RateApplied()
        {
            double Result = 0;
            switch (CoverageUnits)
            {
                case 0:
                    // acres
                    if (cAppMode == ApplicationMode.ControlledUPM || cAppMode == ApplicationMode.DocumentApplied)
                    {
                        // section controlled UPM or Document applied
                        if (cHectaresPerMinute > 0) Result = RateSensor.UPM / (cHectaresPerMinute * 2.47);
                    }
                    else if (cAppMode == ApplicationMode.ConstantUPM)
                    {
                        // Constant UPM
                        // same upm no matter how many sections are on
                        double HPM = mf.Sections.TotalWidth(false) * KMH() / 600.0;
                        if (HPM > 0) Result = RateSensor.UPM / (HPM * 2.47);
                    }
                    else
                    {
                        // Document target rate
                        Result = TargetRate();
                    }
                    break;

                case 1:
                    // hectares
                    if (cAppMode == ApplicationMode.ControlledUPM || cAppMode == ApplicationMode.DocumentApplied)
                    {
                        // section controlled UPM or Document applied
                        if (cHectaresPerMinute > 0) Result = RateSensor.UPM / cHectaresPerMinute;
                    }
                    else if (cAppMode == ApplicationMode.ConstantUPM)
                    {
                        // Constant UPM
                        // same upm no matter how many sections are on
                        double HPM = mf.Sections.TotalWidth(false) * KMH() / 600.0;
                        if (HPM > 0) Result = RateSensor.UPM / HPM;
                    }
                    else
                    {
                        // Document target rate
                        Result = TargetRate();
                    }
                    break;

                case 2:
                    // minutes
                    if (cAppMode == ApplicationMode.DocumentTarget)
                    {
                        // document target rate
                        Result = TargetRate();
                    }
                    else
                    {
                        Result = RateSensor.UPM;
                    }
                    break;

                default:
                    // hours
                    if (cAppMode == ApplicationMode.DocumentTarget)
                    {
                        // document target rate
                        Result = TargetRate();
                    }
                    else
                    {
                        Result = RateSensor.UPM * 60;
                    }
                    break;
            }

            return Result;
        }

        public void ResetApplied()
        {
            cUnitsApplied = 0;
            EraseAccumulatedUnits = true;
        }

        public void ResetApplied2()
        {
            cUnitsApplied2 = 0;
        }

        public void ResetCoverage()
        {
            Coverage = 0;
        }

        public void ResetCoverage2()
        {
            Coverage2 = 0;
        }

        public void ResetHours1()
        {
            cHours1 = 0;
        }

        public void ResetHours2()
        {
            cHours2 = 0;
        }

        public void ResetTank()
        {
            cTankStart = TankSize;
        }

        public void Save()
        {
            Props.SetProp("IsNew" + IDname, "false");
            Props.SetProp("Coverage" + IDname, Coverage.ToString());
            Props.SetProp("Coverage2" + IDname, Coverage2.ToString());
            Props.SetProp("CoverageUnits" + IDname, CoverageUnits.ToString());

            Props.SetProp("TankStart" + IDname, cTankStart.ToString());
            Props.SetProp("QuantityDescription" + IDname, cQuantityDescription);

            Props.SetProp("QuantityApplied" + IDname, cUnitsApplied.ToString());
            Props.SetProp("QuantityApplied2" + IDname, cUnitsApplied2.ToString());
            Props.SetProp("AccumulatedLast" + IDname, AccumulatedLast.ToString());

            Props.SetProp("cProdDensity" + IDname, cProdDensity.ToString());
            Props.SetProp("cEnableProdDensity" + IDname, cEnableProdDensity.ToString());

            Props.SetProp("RateSet" + IDname, cRateSet.ToString());
            Props.SetProp("RateAlt" + IDname, cRateAlt.ToString());
            Props.SetProp("FlowCal" + IDname, cMeterCal.ToString());
            Props.SetProp("TankSize" + IDname, TankSize.ToString());
            Props.SetProp("ValveType" + IDname, cControlType.ToString());

            Props.SetProp("ProductName" + IDname, cProductName);

            Props.SetProp("CountsRev" + IDname, cCountsRev.ToString());

            Props.SetProp("ModuleID" + IDname, cModID.ToString());
            Props.SetProp("SensorID" + IDname, cSenID.ToString());

            Props.SetProp("OffRateAlarm" + IDname, cUseOffRateAlarm.ToString());
            Props.SetProp("OffRateSetting" + IDname, cOffRateSetting.ToString());

            Props.SetProp("MinUPM" + IDname, cMinUPM.ToString());
            Props.SetProp("MinUPMbySpeed" + IDname, cMinUPMbySpeed.ToString());
            Props.SetProp("UseMinUPMbySpeed" + IDname, cUseMinUPMbySpeed.ToString());

            Props.SetProp("SerialPort" + IDname, cSerialPort.ToString());
            Props.SetProp("ManualPWM" + IDname, cManualPWM.ToString());

            Props.SetProp("OnScreen" + IDname, cOnScreen.ToString());
            Props.SetProp("BumpButtons" + IDname, cBumpButtons.ToString());

            Props.SetProp("ShiftRange" + IDname, cShiftRange.ToString());
            Props.SetProp("Hours1" + IDname, cHours1.ToString());
            Props.SetProp("Hours2" + IDname, cHours2.ToString());

            Props.SetProp("AppMode" + IDname, cAppMode.ToString());

            Props.SetProp("MaxAdjust" + IDname, cMaxAdjust.ToString());
            Props.SetProp("MinAdjust" + IDname, cMinAdjust.ToString());
            Props.SetProp("Scaling" + IDname, cScalingFactor.ToString());

            Props.SetProp("Integral" + IDname, cIntegral.ToString());
        }

        public void SendPID()
        {
            ModuleControlSettings.Send();
        }

        public double SmoothRate()
        {
            double Result = 0;
            if (ProductOn())
            {
                double Ra = RateApplied();
                if (cEnableProdDensity && cProdDensity > 0) Ra *= cProdDensity;

                if (TargetRate() > 0)
                {
                    double Rt = Ra / TargetRate();

                    if (Rt >= .9 && Rt <= 1.1 && mf.SwitchBox.AutoRateOn)
                    {
                        Result = TargetRate();
                    }
                    else
                    {
                        Result = Ra;
                    }
                }
                else
                {
                    Result = Ra;
                }
            }
            return Result;
        }

        public double Speed()
        {
            double Result = 0;
            if (Props.SimMode == SimType.Sim_Speed || mf.SectionControl.PrimeOn)
            {
                Result = Props.SimSpeed;
            }
            else
            {
                if (!Props.UseMetric)
                {
                    Result = mf.AutoSteerPGN.Speed_KMH() * 0.621371;
                }
                else
                {
                    Result = mf.AutoSteerPGN.Speed_KMH();
                }
            }
            return Result;
        }

        public double TargetRate()
        {
            double Result = 0;
            if (!CalUseBaseRate && Props.VariableRateEnabled)
            {
                Result = mf.Tls.Manager.GetRate(ID);
            }
            else
            {
                Result = cRateSet;
                if (UseAltRate) Result *= cRateAlt * 0.01;
            }
            return Result;
        }

        public double TargetUPM() // returns units per minute set rate
        {
            double Result = 0;
            switch (CoverageUnits)
            {
                case 0:
                    // acres
                    if (cAppMode == ApplicationMode.ConstantUPM)
                    {
                        // Constant UPM
                        // same upm no matter how many sections are on
                        double HPM = mf.Sections.TotalWidth(false) * KMH() / 600.0;
                        Result = TargetRate() * HPM * 2.47;
                    }
                    else
                    {
                        // section controlled UPM, Document applied or Document target
                        Result = TargetRate() * cHectaresPerMinute * 2.47;
                    }
                    break;

                case 1:
                    // hectares
                    if (cAppMode == ApplicationMode.ConstantUPM)
                    {
                        // Constant UPM
                        // same upm no matter how many sections are on
                        double HPM = mf.Sections.TotalWidth(false) * KMH() / 600.0;
                        Result = TargetRate() * HPM;
                    }
                    else
                    {
                        // section controlled UPM, Document applied or Document target
                        Result = TargetRate() * cHectaresPerMinute;
                    }
                    break;

                case 2:
                    // minutes
                    Result = TargetRate();
                    break;

                default:
                    // hours
                    Result = TargetRate() / 60;
                    break;
            }

            // added this back in to change from lb/min to ft^3/min, Moved from PGN32614.
            if (cEnableProdDensity && cProdDensity > 0) { Result /= cProdDensity; }

            return Result;
        }

        public void UDPcommFromArduino(byte[] data, int PGN)
        {
            try
            {
                if (cAppMode != ApplicationMode.DocumentTarget)    // block real module
                {
                    switch (PGN)
                    {
                        case 32400:
                            if (RateSensor.ParseByteData(data)) UpdateUnitsApplied();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsProduct/UDPcomFromArduino: " + ex.Message);
            }
        }

        public string Units()
        {
            string s = cQuantityDescription + "/" + mf.CoverageAbbr[CoverageUnits];
            return s;
        }

        public double UnitsApplied()
        {
            double Result = cUnitsApplied;
            if (cEnableProdDensity && cProdDensity > 0) Result *= cProdDensity;
            return Result;
        }

        public double UnitsApplied2()
        {
            double Result = cUnitsApplied2;
            if (cEnableProdDensity && cProdDensity > 0) Result *= cProdDensity;
            return Result;
        }

        public void Update()
        {
            if (RateSensor.ModuleSending() || cAppMode == ApplicationMode.DocumentTarget)
            {
                double CurrentMinutes = UpdateStopWatch.ElapsedMilliseconds / 60000.0;
                UpdateStopWatch.Restart();

                // update worked area
                cHectaresPerMinute = mf.Sections.WorkingWidth(false) * KMH() / 600.0;
                CurrentWorkedArea_Hc = cHectaresPerMinute * CurrentMinutes;

                //coverage
                if (cHectaresPerMinute > 0)    // Is application on?
                {
                    switch (CoverageUnits)
                    {
                        case 0:
                            // acres
                            Coverage += CurrentWorkedArea_Hc * 2.47105;
                            Coverage2 += CurrentWorkedArea_Hc * 2.47105;
                            break;

                        case 1:
                            // hectares
                            Coverage += CurrentWorkedArea_Hc;
                            Coverage2 += CurrentWorkedArea_Hc;
                            break;

                        case 2:
                            // minutes
                            Coverage += CurrentMinutes;
                            Coverage2 += CurrentMinutes;
                            break;

                        default:
                            // hours
                            Coverage += CurrentMinutes / 60;
                            Coverage2 += CurrentMinutes / 60;
                            break;
                    }

                    cHours1 += CurrentMinutes / 60.0;
                    cHours2 += CurrentMinutes / 60.0;
                }

                // send to arduino
                ModuleRateSettings.Send();

                if (cAppMode == ApplicationMode.DocumentTarget)
                {
                    // send pgn from virtual module
                    byte[] Data = new byte[13];
                    Data[0] = 144;
                    Data[1] = 126;
                    Data[2] = (byte)(cModID * 16 + cSenID);
                    double Hz = (TargetUPM() * MeterCal / 60.0) * 1000;
                    Data[3] = (byte)Hz;
                    Data[4] = (byte)((int)Hz >> 8);
                    Data[5] = (byte)((int)Hz >> 16);
                    Data[11] = 0b00000001; // sensor connected
                    Data[12] = mf.Tls.CRC(Data, 12);
                    if (RateSensor.ParseByteData(Data)) UpdateUnitsApplied();
                }
            }
            else
            {
                // connection lost
                UpdateStopWatch.Reset();
            }

            if (cControlType == ControlTypeEnum.MotorWeights)
            {
                UpdateUnitsApplied();
            }
        }

        public double UPMapplied()
        {
            return RateSensor.UPM;
        }

        public double WorkRate()
        {
            double Result = 0;
            if (mf.Sections.WorkingWidth(true) > 0)
            {
                if (!Props.UseMetric)
                {
                    Result = cHectaresPerMinute * 2.47105 * 60.0;
                }
                else
                {
                    Result = cHectaresPerMinute * 60.0;
                }
            }
            return Result;
        }

        private double KMH()
        {
            double Result = 0;
            if (Props.SimMode == SimType.Sim_Speed || mf.SectionControl.PrimeOn)
            {
                if (!Props.UseMetric)
                {
                    Result = Props.SimSpeed / 0.621371;  // convert mph back to kmh
                }
                else
                {
                    Result = Props.SimSpeed;
                }
            }
            else
            {
                Result = mf.AutoSteerPGN.Speed_KMH();
            }
            return Result;
        }

        public bool ProductOn()
        {
            bool Result = false;
            if (ControlType == ControlTypeEnum.Fan)
            {
                Result = RateSensor.Connected();
            }
            else
            {
                Result = (RateSensor.Connected() && cHectaresPerMinute > 0);
            }
            return Result;
        }

        private void UpdateUnitsApplied()
        {
            double AccumulatedUnits = RateSensor.AccumulatedQuantity;
            double Diff = AccumulatedUnits - AccumulatedLast;
            if (Diff < 0 || Diff > 1000) Diff = 0;
            AccumulatedLast = AccumulatedUnits;

            cUnitsApplied += Diff;
            cUnitsApplied2 += Diff;

            if (cAppMode == ApplicationMode.ConstantUPM && mf.Sections.TotalWidth() > 0)
            {
                // constant upm, subtract amount for sections that are off
                double Offset = (1.0 - (mf.Sections.WorkingWidth() / mf.Sections.TotalWidth())) * Diff;
                cUnitsApplied -= Offset;
                cUnitsApplied2 -= Offset;
            }
        }
    }
}