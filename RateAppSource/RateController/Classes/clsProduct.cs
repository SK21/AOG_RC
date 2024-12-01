using System;
using System.Diagnostics;

namespace RateController
{
    public enum ApplicationMode
    { ControlledUPM, ConstantUPM, DocumentApplied, DocumentTarget }

    public enum ControlTypeEnum
    { Valve, ComboClose, Motor, MotorWeights, Fan, ComboCloseTimed }

    public class clsProduct
    {
        public readonly FormStart mf;

        public PGN32400 RateSensor;
        public byte CoverageUnits = 0;
        public PGN32500 ModuleRateSettings;
        public double TankSize = 0;
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
        private double cMeterCal = 0;
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
        private int cSenID;
        private int cSerialPort;
        private int cShiftRange = 4;
        private double cTankStart = 0;
        private double cUnitsApplied = 0;
        private double cUnitsApplied2 = 0;
        private double CurrentMinutes;
        private double CurrentWorkedArea_Hc = 0;
        private bool cUseAltRate = false;
        private bool cUseMinUPMbySpeed = false;
        private bool cUseOffRateAlarm;
        private bool cUseVR;
        private byte cVRID = 0;
        private double cVRmax;
        private double cVRmin;
        private DateTime LastHours1;
        private DateTime LastHours2;
        private DateTime LastUpdateTime;
        private PGN32502 ModulePIDdata;
        private bool PauseWork = false;

        public clsProduct(FormStart CallingForm, int ProdID)
        {
            mf = CallingForm;
            cProductID = ProdID;
            cModID = ProdID / 2;
            cSenID = (byte)(ProdID % 2);
            PauseWork = true;

            RateSensor = new PGN32400(this);
            ModuleRateSettings = new PGN32500(this);
            ModulePIDdata = new PGN32502(this);

            if (cProductID > mf.MaxProducts - 3)
            {
                cControlType = ControlTypeEnum.Fan;
                ProductName = "fan";
            }

            LastHours1 = DateTime.Now;
            LastHours2 = DateTime.Now;
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
                if (cProductID > mf.MaxProducts - 3)
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
                if (cProductID > mf.MaxProducts - 3)
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

        public double PIDkd
        { get { return ModulePIDdata.KD; } set { ModulePIDdata.KD = value; } }

        public double PIDki
        { get { return ModulePIDdata.KI; } set { ModulePIDdata.KI = value; } }

        public double PIDkp
        { get { return ModulePIDdata.KP; } set { ModulePIDdata.KP = value; } }

        public byte PIDmax
        { get { return ModulePIDdata.MaxPWM; } set { ModulePIDdata.MaxPWM = value; } }

        public byte PIDmin
        { get { return ModulePIDdata.MinPWM; } set { ModulePIDdata.MinPWM = value; } }

        public int PIDscale
        { get { return cShiftRange; } set { cShiftRange = value; } }

        public double ProdDensity
        { get { return cProdDensity; } set { cProdDensity = value; } }

        public string ProductName
        {
            get
            {
                if (cControlType == ControlTypeEnum.Fan)
                {
                    int tmp = 3 - (mf.MaxProducts - cProductID);
                    cProductName = "Fan " + tmp.ToString();
                }
                return cProductName;
            }
            set
            {
                if (cControlType == ControlTypeEnum.Fan)
                {
                    int tmp = 3 - (mf.MaxProducts - cProductID);
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

        public bool UseVR
        {
            get { return cUseVR; }
            set { cUseVR = value; }
        }

        public byte VRID
        {
            get { return cVRID; }
            set
            {
                if (value < (mf.VRdata.ChannelCount))
                {
                    cVRID = value;
                }
                else
                {
                    throw new ArgumentException("Invalid Variable Rate option.");
                }
            }
        }

        public double VRmax
        {
            get { return cVRmax; }
            set
            {
                if (value > 0 && value < 100000) cVRmax = value;
            }
        }

        public double VRmin
        {
            get { return cVRmin; }
            set
            {
                if (value >= 0 && value < 100000) cVRmin = value;
            }
        }


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
            else if (ModID > -1 && ModID < mf.MaxModules && SenID > -1 && SenID < mf.MaxSensors)
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
            bool Result = (PIDkd == 0 && PIDki == 0 && PIDkp == 0 && PIDmin == 0 && PIDmax == 0);
            return Result;
        }

        public void Load()
        {
            int tmp;
            byte val;
            double TempDB;

            double.TryParse(mf.Tls.LoadProperty("Coverage" + IDname), out Coverage);
            double.TryParse(mf.Tls.LoadProperty("Coverage2" + IDname), out Coverage2);
            byte.TryParse(mf.Tls.LoadProperty("CoverageUnits" + IDname), out CoverageUnits);

            double.TryParse(mf.Tls.LoadProperty("TankStart" + IDname), out cTankStart);
            double.TryParse(mf.Tls.LoadProperty("QuantityApplied" + IDname), out cUnitsApplied);
            double.TryParse(mf.Tls.LoadProperty("QuantityApplied2" + IDname), out cUnitsApplied2);

            if (double.TryParse(mf.Tls.LoadProperty("AccumulatedLast" + IDname), out double oa)) AccumulatedLast = oa;

            cQuantityDescription = mf.Tls.LoadProperty("QuantityDescription" + IDname);
            if (cQuantityDescription == "") cQuantityDescription = "Lbs";

            double.TryParse(mf.Tls.LoadProperty("RateSet" + IDname), out cRateSet);
            if (cRateSet < 0 || cRateSet > 50000) cRateSet = 0;

            double.TryParse(mf.Tls.LoadProperty("RateAlt" + IDname), out cRateAlt);

            double.TryParse(mf.Tls.LoadProperty("cProdDensity" + IDname), out cProdDensity);
            bool.TryParse(mf.Tls.LoadProperty("cEnableProdDensity" + IDname), out cEnableProdDensity);

            double.TryParse(mf.Tls.LoadProperty("FlowCal" + IDname), out cMeterCal);
            double.TryParse(mf.Tls.LoadProperty("TankSize" + IDname), out TankSize);

            cProductName = mf.Tls.LoadProperty("ProductName" + IDname);

            int.TryParse(mf.Tls.LoadProperty("CountsRev" + IDname), out cCountsRev);

            int tmpModuleID = -1;
            if (int.TryParse(mf.Tls.LoadProperty("ModuleID" + IDname), out int tmp1)) tmpModuleID = tmp1;
            int.TryParse(mf.Tls.LoadProperty("SensorID" + IDname), out int tmp2);
            ChangeID(tmpModuleID, tmp2);

            bool.TryParse(mf.Tls.LoadProperty("OffRateAlarm" + IDname), out cUseOffRateAlarm);
            byte.TryParse(mf.Tls.LoadProperty("OffRateSetting" + IDname), out cOffRateSetting);

            double.TryParse(mf.Tls.LoadProperty("MinUPM" + IDname), out cMinUPM);
            double.TryParse(mf.Tls.LoadProperty("MinUPMbySpeed" + IDname), out cMinUPMbySpeed);
            if (bool.TryParse(mf.Tls.LoadProperty("UseMinUPMbySpeed" + IDname), out bool ms)) cUseMinUPMbySpeed = ms;

            byte.TryParse(mf.Tls.LoadProperty("VRID" + IDname), out cVRID);

            if (bool.TryParse(mf.Tls.LoadProperty("UseVR" + IDname), out bool tmp3)) cUseVR = tmp3;
            if (double.TryParse(mf.Tls.LoadProperty("VRmax" + IDname), out double tmp4)) cVRmax = tmp4;
            if (double.TryParse(mf.Tls.LoadProperty("VRmin" + IDname), out double tmp5)) cVRmin = tmp5;

            tmp = 0;
            int.TryParse(mf.Tls.LoadProperty("SerialPort" + IDname), out tmp);
            cSerialPort = tmp;

            tmp = 0;
            int.TryParse(mf.Tls.LoadProperty("ManualPWM" + IDname), out tmp);
            cManualPWM = tmp;

            TempDB = 0;
            double.TryParse(mf.Tls.LoadProperty("KP" + IDname), out TempDB);
            ModulePIDdata.KP = TempDB;

            TempDB = 0;
            double.TryParse(mf.Tls.LoadProperty("KI" + IDname), out TempDB);
            ModulePIDdata.KI = TempDB;

            TempDB = 0;
            double.TryParse(mf.Tls.LoadProperty("KD" + IDname), out TempDB);
            ModulePIDdata.KD = TempDB;

            val = 0;
            byte.TryParse(mf.Tls.LoadProperty("MinPWM" + IDname), out val);
            ModulePIDdata.MinPWM = val;

            val = 0;
            byte.TryParse(mf.Tls.LoadProperty("MaxPWM" + IDname), out val);
            ModulePIDdata.MaxPWM = val;

            if (ID > mf.MaxProducts - 3)
            {
                cControlType = ControlTypeEnum.Fan;
            }
            else
            {
                Enum.TryParse(mf.Tls.LoadProperty("ValveType" + IDname), true, out cControlType);
            }

            if (bool.TryParse(mf.Tls.LoadProperty("OnScreen" + IDname), out bool OS))
            {
                cOnScreen = OS;
            }
            else
            {
                cOnScreen = true;
            }

            if (bool.TryParse(mf.Tls.LoadProperty("BumpButtons" + IDname), out bool BB))
            {
                cBumpButtons = BB;
            }
            else
            {
                cBumpButtons = false;
            }

            if (int.TryParse(mf.Tls.LoadProperty("ShiftRange" + IDname), out int sr)) cShiftRange = sr;
            if (double.TryParse(mf.Tls.LoadProperty("Hours1" + IDname), out double h1)) cHours1 = h1;
            if (double.TryParse(mf.Tls.LoadProperty("Hours2" + IDname), out double h2)) cHours2 = h2;

            if (Enum.TryParse(mf.Tls.LoadProperty("AppMode" + IDname), true, out ApplicationMode am)) cAppMode = am;
        }

        public double MinUPMinUse()
        {
            double Result = cMinUPM;
            if (cUseMinUPMbySpeed)
            {
                double KPH = cMinUPMbySpeed;
                if (mf.UseInches) KPH *= mf.MPHtoKPH;
                double HPM = mf.Sections.TotalWidth(false) * KPH / 600.0;
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

        public void RecordHours()
        {
            if (ProductOn() && TargetRate() > 0)
            {
                cHours1 += (DateTime.Now - LastHours1).TotalHours;
                cHours2 += (DateTime.Now - LastHours2).TotalHours;
            }
            LastHours1 = DateTime.Now;
            LastHours2 = DateTime.Now;
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
            LastUpdateTime = DateTime.Now;
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
            mf.Tls.SaveProperty("Coverage" + IDname, Coverage.ToString());
            mf.Tls.SaveProperty("Coverage2" + IDname, Coverage2.ToString());
            mf.Tls.SaveProperty("CoverageUnits" + IDname, CoverageUnits.ToString());

            mf.Tls.SaveProperty("TankStart" + IDname, cTankStart.ToString());
            mf.Tls.SaveProperty("QuantityDescription" + IDname, cQuantityDescription);

            mf.Tls.SaveProperty("QuantityApplied" + IDname, cUnitsApplied.ToString());
            mf.Tls.SaveProperty("QuantityApplied2" + IDname, cUnitsApplied2.ToString());
            mf.Tls.SaveProperty("AccumulatedLast" + IDname, AccumulatedLast.ToString());

            mf.Tls.SaveProperty("cProdDensity" + IDname, cProdDensity.ToString());
            mf.Tls.SaveProperty("cEnableProdDensity" + IDname, cEnableProdDensity.ToString());

            mf.Tls.SaveProperty("RateSet" + IDname, cRateSet.ToString());
            mf.Tls.SaveProperty("RateAlt" + IDname, cRateAlt.ToString());
            mf.Tls.SaveProperty("FlowCal" + IDname, cMeterCal.ToString());
            mf.Tls.SaveProperty("TankSize" + IDname, TankSize.ToString());
            mf.Tls.SaveProperty("ValveType" + IDname, cControlType.ToString());

            mf.Tls.SaveProperty("ProductName" + IDname, cProductName);

            mf.Tls.SaveProperty("CountsRev" + IDname, cCountsRev.ToString());

            mf.Tls.SaveProperty("ModuleID" + IDname, cModID.ToString());
            mf.Tls.SaveProperty("SensorID" + IDname, cSenID.ToString());

            mf.Tls.SaveProperty("OffRateAlarm" + IDname, cUseOffRateAlarm.ToString());
            mf.Tls.SaveProperty("OffRateSetting" + IDname, cOffRateSetting.ToString());

            mf.Tls.SaveProperty("MinUPM" + IDname, cMinUPM.ToString());
            mf.Tls.SaveProperty("MinUPMbySpeed" + IDname, cMinUPMbySpeed.ToString());
            mf.Tls.SaveProperty("UseMinUPMbySpeed" + IDname, cUseMinUPMbySpeed.ToString());

            mf.Tls.SaveProperty("VRID" + IDname, cVRID.ToString());
            mf.Tls.SaveProperty("UseVR" + IDname, cUseVR.ToString());
            mf.Tls.SaveProperty("VRmax" + IDname, cVRmax.ToString());
            mf.Tls.SaveProperty("VRmin" + IDname, cVRmin.ToString());

            mf.Tls.SaveProperty("SerialPort" + IDname, cSerialPort.ToString());
            mf.Tls.SaveProperty("ManualPWM" + IDname, cManualPWM.ToString());

            mf.Tls.SaveProperty("KP" + IDname, ModulePIDdata.KP.ToString());
            mf.Tls.SaveProperty("KI" + IDname, ModulePIDdata.KI.ToString());
            mf.Tls.SaveProperty("KD" + IDname, ModulePIDdata.KD.ToString());
            mf.Tls.SaveProperty("MinPWM" + IDname, ModulePIDdata.MinPWM.ToString());
            mf.Tls.SaveProperty("MaxPWM" + IDname, ModulePIDdata.MaxPWM.ToString());

            mf.Tls.SaveProperty("OnScreen" + IDname, cOnScreen.ToString());
            mf.Tls.SaveProperty("BumpButtons" + IDname, cBumpButtons.ToString());

            mf.Tls.SaveProperty("ShiftRange" + IDname, cShiftRange.ToString());
            mf.Tls.SaveProperty("Hours1" + IDname, cHours1.ToString());
            mf.Tls.SaveProperty("Hours2" + IDname, cHours2.ToString());

            mf.Tls.SaveProperty("AppMode" + IDname, cAppMode.ToString());
        }

        public void SendPID()
        {
            ModulePIDdata.Send();
        }

        public bool SerialFromAruduino(string[] words, bool RealNano = true)
        {
            bool Result = false;    // return true if there is good comm
            try
            {
                if (RealNano && mf.SimMode == SimType.VirtualNano)
                {
                    // block PGN32400 from real nano when simulation is with virtual nano
                }
                else
                {
                    if (RateSensor.ParseStringData(words))
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
            if (mf.SimMode == SimType.Speed || mf.SectionControl.PrimeOn)
            {
                Result = mf.SimSpeed;
            }
            else
            {
                if (mf.UseInches)
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
            if (cUseVR && !CalUseBaseRate)
            {
                Result = cVRmin + (cVRmax - cVRmin) * mf.VRdata.Percentage(cVRID) * 0.01;
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
                mf.Tls.WriteErrorLog("clsProduct/UDPcomFromArduino: " + ex.Message);
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
            DateTime UpdateStartTime;
            if (RateSensor.ModuleSending() || cAppMode == ApplicationMode.DocumentTarget)
            {
                UpdateStartTime = DateTime.Now;
                CurrentMinutes = (UpdateStartTime - LastUpdateTime).TotalMinutes;
                LastUpdateTime = UpdateStartTime;

                if (CurrentMinutes < 0 || CurrentMinutes > 1 || PauseWork)
                {
                    CurrentMinutes = 0;
                    PauseWork = false;
                }

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
                PauseWork = true;
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
                if (mf.UseInches)
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
            if (mf.SimMode == SimType.Speed || mf.SectionControl.PrimeOn)
            {
                if (mf.UseInches)
                {
                    Result = mf.SimSpeed / 0.621371;  // convert mph back to kmh
                }
                else
                {
                    Result = mf.SimSpeed;
                }
            }
            else
            {
                Result = mf.AutoSteerPGN.Speed_KMH();
            }
            return Result;
        }

        private bool ProductOn()
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
            if (AccumulatedLast > AccumulatedUnits) AccumulatedLast = 0;
            double Diff = AccumulatedUnits - AccumulatedLast;
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