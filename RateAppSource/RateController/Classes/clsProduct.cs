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
        public PGN32400 RateSensorData;
        private double AccumulatedLast = 0;
        private ApplicationMode cAppMode = ApplicationMode.ControlledUPM;
        private bool cBumpButtons;
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
        private double cMeterCal = 0;
        private double cMinUPM;
        private double cMinUPMbySpeed;
        private byte cOffRateSetting;
        private bool cOnScreen;
        private double Coverage = 0;
        private double Coverage2 = 0;
        private double cProdDensity = 0;
        private int cProductID;
        private string cProductName = "";
        private string cQuantityDescription = "Lbs";
        private double cRateAlt = 100;
        private clsSensor cRateSensor;
        private double cRateSet = 0;
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
        private PGN32502 SensorControlSettings;
        private Stopwatch UpdateStopWatch;
        private CalibrationMode cCalMode = CalibrationMode.Off;

        public clsProduct(FormStart CallingForm, int ProdID)
        {
            mf = CallingForm;
            cProductID = ProdID;
            //int Mod = ProdID / 2;
            //int Sen = (byte)(ProdID % 2);
            //cRateSensor = mf.RateSensors.Item(Mod, Sen);
            //if (cRateSensor == null) cRateSensor = mf.RateSensors.AddSensor(Mod, Sen);

            RateSensorData = new PGN32400(this);
            ModuleRateSettings = new PGN32500(this);
            SensorControlSettings = new PGN32502(this);

            if (cProductID > Props.MaxProducts - 3)
            {
                cControlType = ControlTypeEnum.Fan;
                ProductName = "fan";
            }

            UpdateStopWatch = new Stopwatch();
        }

        #region Control edit

        public byte Brakepoint
        {
            get { return cRateSensor.BrakePoint; }
            set
            {
                cRateSensor.BrakePoint = value;
            }
        }

        public byte Deadband
        {
            get { return cRateSensor.DeadBand; }
            set
            {
                cRateSensor.DeadBand = value;
            }
        }
        public byte PulseMinHz
        {
            get { return cRateSensor.PulseMinHz; }
            set
            {  cRateSensor.PulseMinHz = value;}
        }
        public UInt16 PulseMaxHz
        {
            get { return cRateSensor.PulseMaxHz; }
            set { cRateSensor.PulseMaxHz = value;}
        }
        public byte PulseSampleSize
        {
            get { return cRateSensor.PulseSampleSize; }
            set { cRateSensor.PulseSampleSize=value;}
        }

        public int KI
        {
            get { return cRateSensor.KI; }
            set
            {
                cRateSensor.KI = (byte)value;
            }
        }

        public int KP
        {
            get { return cRateSensor.KP; }
            set
            {
                cRateSensor.KP = (byte)value;
            }
        }

        public byte MaxMotorIntegral
        {
            get { return cRateSensor.MaxIntegral; }
            set
            {
                cRateSensor.MaxIntegral = value;
            }
        }

        public int MaxPWMadjust
        {
            get { return cRateSensor.MaxPWM; }
            set
            {
                cRateSensor.MaxPWM = (byte)value;
            }
        }

        public int MinPWMadjust
        {
            get { return cRateSensor.MinPWM; }
            set
            {
                cRateSensor.MinPWM = (byte)value;
            }
        }

        public byte PIDslowAdjust
        {
            get { return cRateSensor.PIDslowAdjust; }
            set
            {
                cRateSensor.PIDslowAdjust = value;
            }
        }

        public byte PIDtime
        {
            get { return cRateSensor.PIDtime; }
            set
            {
                cRateSensor.PIDtime = value;
            }
        }

        public clsSensor RateSensor
        { get { return cRateSensor; } }

        public byte SlewRate
        {
            get { return cRateSensor.SlewRate; }
            set
            {
                cRateSensor.SlewRate = value;
            }
        }

        public UInt16 TimedAdjust
        {
            get { return cRateSensor.TimedAdjust; }
            set
            {
                cRateSensor.TimedAdjust = value;
            }
        }

        public byte TimedMinStart
        {
            get { return cRateSensor.TimedMinStart; }
            set
            {
                cRateSensor.TimedMinStart = value;
            }
        }

        public int TimedPause
        {
            get { return cRateSensor.TimedPause; }
            set
            {
                cRateSensor.TimedPause = (ushort)value;
            }
        }

        #endregion Control edit

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

        //public bool CalRun
        //{
        //    // notifies module Master switch on for calibrate and use current meter cal in manual mode
        //    // current meter position is used and not adjusted

        //    get { return cCalRun; }
        //    set
        //    {
        //        cCalRun = value;
        //        if (cCalRun) cCalSetMeter = false;
        //    }
        //}

        //public bool CalSetMeter
        //{
        //    // notifies module Master switch on for calibrate and use auto mode to find meter cal
        //    // adjusts meter position to match base rate

        //    get { return cCalSetMeter; }
        //    set
        //    {
        //        cCalSetMeter = value;
        //        if (cCalSetMeter) cCalRun = false;
        //    }
        //}

        //public bool CalUseBaseRate
        //{
        //    // use base rate for cal and not vr rate
        //    get { return cCalUseBaseRate; }
        //    set { cCalUseBaseRate = value; }
        //}

        public CalibrationMode CalMode
        {
            get { return cCalMode; }
            set
            {
                cCalMode = value;
                if(cCalMode==CalibrationMode.Off)
                {
                    cCalUseBaseRate = false;
                }
                else
                {
                    // use base rate for cal and not vr rate
                    cCalUseBaseRate = true;
                }
            }
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
        { get { return RateSensorData.ElapsedTime(); } }

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
            get { return cRateSensor.ModuleID; }
        }

        public bool ModuleSending
        { get { return RateSensorData.ModuleSending(); } }

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

        public byte SensorID
        {
            get { return (byte)cRateSensor.SensorID; }
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
        { get { return "Product_" + cProductID.ToString() + "_"; } }

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

        public bool EditSensorIDs(int ModID, int SenID)
        {
            return mf.RateSensors.EditSensorIDs(cRateSensor, ModID, SenID);
        }

        public double Hz()
        {
            return RateSensorData.Hz;
        }

        public bool IsNew()
        {
            bool Result = true;
            if (bool.TryParse(Props.GetProp(IDname + "IsNew"), out bool nw)) Result = nw;
            return Result;
        }

        public void Load()
        {
            int tmp;

            double.TryParse(Props.GetProp(IDname + "Coverage"), out Coverage);
            double.TryParse(Props.GetProp(IDname + "Coverage2"), out Coverage2);
            byte.TryParse(Props.GetProp(IDname + "CoverageUnits"), out CoverageUnits);

            double.TryParse(Props.GetProp(IDname + "TankStart"), out cTankStart);
            double.TryParse(Props.GetProp(IDname + "QuantityApplied"), out cUnitsApplied);
            double.TryParse(Props.GetProp(IDname + "QuantityApplied2"), out cUnitsApplied2);

            if (double.TryParse(Props.GetProp(IDname + "AccumulatedLast"), out double oa)) AccumulatedLast = oa;

            cQuantityDescription = Props.GetProp(IDname + "QuantityDescription");
            if (cQuantityDescription == "") cQuantityDescription = "Lbs";

            double.TryParse(Props.GetProp(IDname + "RateSet"), out cRateSet);
            if (cRateSet < 0 || cRateSet > 50000) cRateSet = 0;

            double.TryParse(Props.GetProp(IDname + "RateAlt"), out cRateAlt);

            double.TryParse(Props.GetProp(IDname + "cProdDensity"), out cProdDensity);
            bool.TryParse(Props.GetProp(IDname + "cEnableProdDensity"), out cEnableProdDensity);

            double.TryParse(Props.GetProp(IDname + "FlowCal"), out cMeterCal);
            if (double.TryParse(Props.GetProp(IDname + "TankSize"), out double ts)) cTankSize = ts;

            cProductName = Props.GetProp(IDname + "ProductName");

            int.TryParse(Props.GetProp(IDname + "CountsRev"), out cCountsRev);

            int tmpModuleID = -1;
            if (int.TryParse(Props.GetProp(IDname + "ModuleID"), out int tmp1)) tmpModuleID = tmp1;
            int.TryParse(Props.GetProp(IDname + "SensorID"), out int tmp2);
            LoadSensor(tmp1, tmp2);

            bool.TryParse(Props.GetProp(IDname + "OffRateAlarm"), out cUseOffRateAlarm);
            byte.TryParse(Props.GetProp(IDname + "OffRateSetting"), out cOffRateSetting);

            double.TryParse(Props.GetProp(IDname + "MinUPM"), out cMinUPM);
            double.TryParse(Props.GetProp(IDname + "MinUPMbySpeed"), out cMinUPMbySpeed);
            if (bool.TryParse(Props.GetProp(IDname + "UseMinUPMbySpeed"), out bool ms)) cUseMinUPMbySpeed = ms;

            tmp = 0;
            int.TryParse(Props.GetProp(IDname + "SerialPort"), out tmp);
            cSerialPort = tmp;

            tmp = 0;
            int.TryParse(Props.GetProp(IDname + "ManualPWM"), out tmp);
            cManualPWM = tmp;

            if (ID > Props.MaxProducts - 3)
            {
                cControlType = ControlTypeEnum.Fan;
            }
            else
            {
                Enum.TryParse(Props.GetProp(IDname + "ValveType"), true, out cControlType);
            }

            if (bool.TryParse(Props.GetProp(IDname + "OnScreen"), out bool OS))
            {
                cOnScreen = OS;
            }
            else
            {
                cOnScreen = true;
            }

            if (bool.TryParse(Props.GetProp(IDname + "BumpButtons"), out bool BB))
            {
                cBumpButtons = BB;
            }
            else
            {
                cBumpButtons = false;
            }

            if (int.TryParse(Props.GetProp(IDname + "ShiftRange"), out int sr)) cShiftRange = sr;
            if (double.TryParse(Props.GetProp(IDname + "Hours1"), out double h1)) cHours1 = h1;
            if (double.TryParse(Props.GetProp(IDname + "Hours2"), out double h2)) cHours2 = h2;
            if (Enum.TryParse(Props.GetProp(IDname + "AppMode"), true, out ApplicationMode am)) cAppMode = am;
        }

        public void LoadSensor(int ModID, int SenID)
        {
            cRateSensor = mf.RateSensors.Item(ModID, SenID);
            if (cRateSensor == null) cRateSensor = mf.RateSensors.AddSensor(ModID, SenID);
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

        public bool ProductOn()
        {
            bool Result = false;
            if (ControlType == ControlTypeEnum.Fan)
            {
                Result = RateSensorData.Connected();
            }
            else
            {
                Result = (RateSensorData.Connected() && (cHectaresPerMinute > 0 || Props.RateCalibrationOn));
            }
            return Result;
        }

        public double Pulses()
        {
            return cUnitsApplied * cMeterCal;
        }

        public double PWM()
        {
            return RateSensorData.PWMsetting;
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
                        if (cHectaresPerMinute > 0) Result = RateSensorData.UPM / (cHectaresPerMinute * 2.47);
                    }
                    else if (cAppMode == ApplicationMode.ConstantUPM)
                    {
                        // Constant UPM
                        // same upm no matter how many sections are on
                        double HPM = mf.Sections.TotalWidth(false) * KMH() / 600.0;
                        if (HPM > 0) Result = RateSensorData.UPM / (HPM * 2.47);
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
                        if (cHectaresPerMinute > 0) Result = RateSensorData.UPM / cHectaresPerMinute;
                    }
                    else if (cAppMode == ApplicationMode.ConstantUPM)
                    {
                        // Constant UPM
                        // same upm no matter how many sections are on
                        double HPM = mf.Sections.TotalWidth(false) * KMH() / 600.0;
                        if (HPM > 0) Result = RateSensorData.UPM / HPM;
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
                        Result = RateSensorData.UPM;
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
                        Result = RateSensorData.UPM * 60;
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
            Props.SetProp(IDname + "IsNew", "false");
            Props.SetProp(IDname + "Coverage", Coverage.ToString());
            Props.SetProp(IDname + "Coverage2", Coverage2.ToString());
            Props.SetProp(IDname + "CoverageUnits", CoverageUnits.ToString());

            Props.SetProp(IDname + "TankStart", cTankStart.ToString());
            Props.SetProp(IDname + "QuantityDescription", cQuantityDescription);

            Props.SetProp(IDname + "QuantityApplied", cUnitsApplied.ToString());
            Props.SetProp(IDname + "QuantityApplied2", cUnitsApplied2.ToString());
            Props.SetProp(IDname + "AccumulatedLast", AccumulatedLast.ToString());

            Props.SetProp(IDname + "cProdDensity", cProdDensity.ToString());
            Props.SetProp(IDname + "cEnableProdDensity", cEnableProdDensity.ToString());

            Props.SetProp(IDname + "RateSet", cRateSet.ToString());
            Props.SetProp(IDname + "RateAlt", cRateAlt.ToString());
            Props.SetProp(IDname + "FlowCal", cMeterCal.ToString());
            Props.SetProp(IDname + "TankSize", TankSize.ToString());
            Props.SetProp(IDname + "ValveType", cControlType.ToString());

            Props.SetProp(IDname + "ProductName", cProductName);

            Props.SetProp(IDname + "CountsRev", cCountsRev.ToString());

            Props.SetProp(IDname + "ModuleID", cRateSensor.ModuleID.ToString());
            Props.SetProp(IDname + "SensorID", cRateSensor.SensorID.ToString());

            Props.SetProp(IDname + "OffRateAlarm", cUseOffRateAlarm.ToString());
            Props.SetProp(IDname + "OffRateSetting", cOffRateSetting.ToString());

            Props.SetProp(IDname + "MinUPM", cMinUPM.ToString());
            Props.SetProp(IDname + "MinUPMbySpeed", cMinUPMbySpeed.ToString());
            Props.SetProp(IDname + "UseMinUPMbySpeed", cUseMinUPMbySpeed.ToString());

            Props.SetProp(IDname + "SerialPort", cSerialPort.ToString());
            Props.SetProp(IDname + "ManualPWM", cManualPWM.ToString());

            Props.SetProp(IDname + "OnScreen", cOnScreen.ToString());
            Props.SetProp(IDname + "BumpButtons", cBumpButtons.ToString());

            Props.SetProp(IDname + "ShiftRange", cShiftRange.ToString());
            Props.SetProp(IDname + "Hours1", cHours1.ToString());
            Props.SetProp(IDname + "Hours2", cHours2.ToString());

            Props.SetProp(IDname + "AppMode", cAppMode.ToString());

            cRateSensor.Save();
        }

        public void SendSensorSettings()
        {
            SensorControlSettings.Send();
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
            if (!cCalUseBaseRate && Props.VariableRateEnabled)
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
                        if (cHectaresPerMinute == 0 && cCalMode==CalibrationMode.Off) HPM = 0;   // all sections off
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
                        if (cHectaresPerMinute == 0 && cCalMode == CalibrationMode.Off) HPM = 0;
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
                            if (RateSensorData.ParseByteData(data)) UpdateUnitsApplied();
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
            if (RateSensorData.ModuleSending() || cAppMode == ApplicationMode.DocumentTarget)
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
                    Data[2] = (byte)(cRateSensor.ModuleID * 16 + cRateSensor.SensorID);
                    double Hz = (TargetUPM() * MeterCal / 60.0) * 1000;
                    Data[3] = (byte)Hz;
                    Data[4] = (byte)((int)Hz >> 8);
                    Data[5] = (byte)((int)Hz >> 16);
                    Data[11] = 0b00000001; // sensor connected
                    Data[12] = mf.Tls.CRC(Data, 12);
                    if (RateSensorData.ParseByteData(Data)) UpdateUnitsApplied();
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
            return RateSensorData.UPM;
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

        private void UpdateUnitsApplied()
        {
            double AccumulatedUnits = RateSensorData.AccumulatedQuantity;
            double Diff = AccumulatedUnits - AccumulatedLast;
            if (Diff < 0 || Diff > 1000) Diff = 0;
            AccumulatedLast = AccumulatedUnits;

            cUnitsApplied += Diff;
            cUnitsApplied2 += Diff;

            if (cAppMode == ApplicationMode.ConstantUPM && mf.Sections.TotalWidth() > 0 && !Props.RateCalibrationOn)
            {
                // constant upm, subtract amount for sections that are off
                double Offset = (1.0 - (mf.Sections.WorkingWidth() / mf.Sections.TotalWidth())) * Diff;
                cUnitsApplied -= Offset;
                cUnitsApplied2 -= Offset;
            }
        }
    }
}