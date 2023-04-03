using System;
using System.Diagnostics;
using System.Globalization;

namespace RateController
{
    public enum ControlTypeEnum
    { Valve, ComboClose, Motor, MotorWeights, Fan }

    public class clsProduct
    {
        public readonly FormStart mf;

        public PGN32613 ArduinoModule;
        public byte CoverageUnits = 0;
        private bool cEraseAccumulatedUnits = false;
        public PGN32614 RateToArduino;
        public PGN32501 Scale;
        public double TankSize = 0;
        public clsArduino VirtualNano;
        private double cCalEnd;
        private int cManualPWM;
        private double cCalStart;
        private ControlTypeEnum cControlType = 0;
        private int cCountsRev;
        private bool cDebugArduino = false;
        private bool cDoCal;
        private double cHectaresPerMinute;
        private bool cLogRate;
        private double cMeterCal = 0;
        private double cMinUPM;
        private int cModID;
        private byte cOffRateSetting;
        private double Coverage = 0;
        private int cProductID;
        private string cProductName = "";
        private string cQuantityDescription = "Lbs";
        private double cRateAlt = 100;
        private double cRateSet = 0;
        private double cScaleTare;
        private double cProdDensity = 0;
        private bool cEnableProdDensity = false;

        private double cScaleUnitsCal;

        private int cSenID;
        private int cSerialPort;
        private double cTankStart = 0;
        private double cUnitsApplied = 0;
        private double CurrentMinutes;
        private double CurrentWorkedArea_Hc = 0;
        private bool cUseAltRate = false;
        private bool cUseMultiPulse;
        private bool cUseOffRateAlarm;
        private byte cVariableRate = 0;
        private byte cWifiStrength;
        private double LastAccQuantity = 0;
        private double LastScaleCounts = 0;
        private DateTime LastUpdateTime;
        private bool PauseWork = false;
        private PGN32616 PIDtoArduino;

        private bool SwitchIDsSent;
        private double UnitsOffset = 0;
        private byte[] VRconversion = { 255, 0, 1, 2, 3, 4 };   // 255 = off
        private bool cFanOn;
        private bool cOnScreen;
        private bool cBumpButtons;
        private bool cConstantUPM;

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

            if (cProductID > mf.MaxProducts - 3)
            {
                cControlType = ControlTypeEnum.Fan;
                ProductName = "fan";
            }
        }

        public double CalEnd
        {
            get { return cCalEnd; }
            set
            {
                if (value >= cCalStart)
                {
                    cCalEnd = value;
                }
            }
        }

        public int ManualPWM
        {
            get { return cManualPWM; }
            set
            {
                if (cControlType == ControlTypeEnum.Valve || cControlType == ControlTypeEnum.ComboClose)
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

        public bool FanOn
        {
            get { return cFanOn; }
            set
            {
                cFanOn = value;
            }
        }

        public bool ConstantUPM
        {
            get { return cConstantUPM; }
            set { cConstantUPM = value; }
        }

        public bool OnScreen
        {
            get { return cOnScreen; }
            set { cOnScreen = value; }
        }

        public bool BumpButtons
        {
            get { return cBumpButtons; }
            set { cBumpButtons = value; }
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
            get
            { 
                if(cProductID> mf.MaxProducts - 3) 
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

        public double ProdDensity
        { get { return cProdDensity; } set { cProdDensity = value; } }

        public bool EnableProdDensity
        { get { return cEnableProdDensity;} set { cEnableProdDensity = value; } }

        public int ID
        { get { return cProductID; } }

        public bool LogRate
        { get { return cLogRate; } set { cLogRate = value; } }

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

        public bool ChangeID(int ModID, int SenID)
        {
            bool Result = false;
            if (ModID == 99) ModID = cProductID;

            if (ModID < 16 && SenID < 16)
            {
                if (mf.Products.UniqueModSen(ModID, SenID, cProductID))
                {
                    cModID = ModID;
                    cSenID = SenID;
                }
            }
            return Result;
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

        public byte PIDmax
        { get { return PIDtoArduino.MaxPWM; } set { PIDtoArduino.MaxPWM = value; } }

        public double PIDki
        { get { return PIDtoArduino.KI; } set { PIDtoArduino.KI = value; } }

        public double PIDkp
        { get { return PIDtoArduino.KP; } set { PIDtoArduino.KP = value; } }

        public double PIDkd
        { get { return PIDtoArduino.KD; } set { PIDtoArduino.KD = value; } }

        public byte PIDmin
        { get { return PIDtoArduino.MinPWM; } set { PIDtoArduino.MinPWM = value; } }

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
        { get { return cRateAlt; } set { cRateAlt = value; } }

        public double RateSet
        {
            get { return cRateSet; }
            set
            {
                if (value < 0 || value > 50000)
                {
                    throw new ArgumentException("Must be between 0 and 50,000");
                }
                else
                {
                    cRateSet = value;
                }
            }
        }

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

        public bool EraseAccumulatedUnits { get => cEraseAccumulatedUnits; set => cEraseAccumulatedUnits = value; }

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

        public double CurrentWeight()
        {
            if (cScaleUnitsCal > 0)
            {
                double V = NetScaleCounts() / cScaleUnitsCal;
                if (cEnableProdDensity && cProdDensity > 0) V *= cProdDensity;
                return V;
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
            double TempDB;

            double.TryParse(mf.Tls.LoadProperty("Coverage" + IDname), out Coverage);
            byte.TryParse(mf.Tls.LoadProperty("CoverageUnits" + IDname), out CoverageUnits);

            double.TryParse(mf.Tls.LoadProperty("TankStart" + IDname), out cTankStart);
            double.TryParse(mf.Tls.LoadProperty("QuantityApplied" + IDname), out cUnitsApplied);
            double.TryParse(mf.Tls.LoadProperty("LastAccQuantity" + IDname), out LastAccQuantity);

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

            bool.TryParse(mf.Tls.LoadProperty("UseMultiPulse" + IDname), out cUseMultiPulse);
            int.TryParse(mf.Tls.LoadProperty("CountsRev" + IDname), out cCountsRev);

            int tmpModuleID = 99;
            if (int.TryParse(mf.Tls.LoadProperty("ModuleID" + IDname), out int tmp1)) tmpModuleID = tmp1;
            int.TryParse(mf.Tls.LoadProperty("SensorID" + IDname), out int tmp2);
            ChangeID(tmpModuleID, tmp2);

            bool.TryParse(mf.Tls.LoadProperty("OffRateAlarm" + IDname), out cUseOffRateAlarm);
            byte.TryParse(mf.Tls.LoadProperty("OffRateSetting" + IDname), out cOffRateSetting);

            double.TryParse(mf.Tls.LoadProperty("MinUPM" + IDname), out cMinUPM);
            byte.TryParse(mf.Tls.LoadProperty("VariableRate" + IDname), out cVariableRate);

            tmp = 0;
            int.TryParse(mf.Tls.LoadProperty("SerialPort" + IDname), out tmp);
            cSerialPort = tmp;

            tmp = 0;
            int.TryParse(mf.Tls.LoadProperty("ManualPWM" + IDname), out tmp);
            cManualPWM = tmp;

            double.TryParse(mf.Tls.LoadProperty("ScaleUnitsCal" + IDname), out cScaleUnitsCal);
            double.TryParse(mf.Tls.LoadProperty("ScaleTare" + IDname), out cScaleTare);

            TempDB = 0;
            double.TryParse(mf.Tls.LoadProperty("KP" + IDname), out TempDB);
            PIDtoArduino.KP = TempDB;

            TempDB = 0;
            double.TryParse(mf.Tls.LoadProperty("KI" + IDname), out TempDB);
            PIDtoArduino.KI = TempDB;

            TempDB = 0;
            double.TryParse(mf.Tls.LoadProperty("KD" + IDname), out TempDB);
            PIDtoArduino.KD = TempDB;

            val = 0;
            byte.TryParse(mf.Tls.LoadProperty("MinPWM" + IDname), out val);
            PIDtoArduino.MinPWM = val;

            val = 0;
            byte.TryParse(mf.Tls.LoadProperty("MaxPWM" + IDname), out val);
            PIDtoArduino.MaxPWM = val;

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

            bool.TryParse(mf.Tls.LoadProperty("ConstantUPM" + IDname), out cConstantUPM);
        }

        public double PWM()
        {
            return ArduinoModule.PWMsetting();
        }

        public double RateApplied()
        {
            double Result = 0;
            switch (CoverageUnits)
            {
                case 0:
                    // acres
                    if (cConstantUPM)
                    {
                        // same upm no matter how many sections are on
                        double HPM = mf.Sections.TotalWidth(false) * KMH() / 600.0;
                        if (HPM > 0) Result = ArduinoModule.UPM() / (HPM * 2.47);
                    }
                    else
                    {
                        if (cHectaresPerMinute > 0) Result = ArduinoModule.UPM() / (cHectaresPerMinute * 2.47);
                    }
                    break;

                case 1:
                    // hectares
                    if (cConstantUPM)
                    {
                        double HPM = mf.Sections.TotalWidth(false) * KMH() / 600.0;
                        if (HPM > 0) Result = ArduinoModule.UPM() / HPM;
                    }
                    else
                    {
                        if (cHectaresPerMinute > 0) Result = ArduinoModule.UPM() / cHectaresPerMinute;
                    }
                    break;

                case 2:
                    // minutes
                    Result = ArduinoModule.UPM();
                    break;

                default:
                    // hours
                    Result = ArduinoModule.UPM() * 60;
                    break;
            }

            return Result;

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

            mf.Tls.SaveProperty("cProdDensity" + IDname, cProdDensity.ToString());
            mf.Tls.SaveProperty("cEnableProdDensity" + IDname, cEnableProdDensity.ToString());

            mf.Tls.SaveProperty("RateSet" + IDname, cRateSet.ToString());
            mf.Tls.SaveProperty("RateAlt" + IDname, cRateAlt.ToString());
            mf.Tls.SaveProperty("FlowCal" + IDname, cMeterCal.ToString());
            mf.Tls.SaveProperty("TankSize" + IDname, TankSize.ToString());
            mf.Tls.SaveProperty("ValveType" + IDname, cControlType.ToString());

            mf.Tls.SaveProperty("ProductName" + IDname, cProductName);

            mf.Tls.SaveProperty("UseMultiPulse" + IDname, cUseMultiPulse.ToString());
            mf.Tls.SaveProperty("CountsRev" + IDname, cCountsRev.ToString());

            mf.Tls.SaveProperty("ModuleID" + IDname, cModID.ToString());
            mf.Tls.SaveProperty("SensorID" + IDname, cSenID.ToString());

            mf.Tls.SaveProperty("OffRateAlarm" + IDname, cUseOffRateAlarm.ToString());
            mf.Tls.SaveProperty("OffRateSetting" + IDname, cOffRateSetting.ToString());

            mf.Tls.SaveProperty("MinUPM" + IDname, cMinUPM.ToString());
            mf.Tls.SaveProperty("VariableRate" + IDname, cVariableRate.ToString());
            mf.Tls.SaveProperty("SerialPort" + IDname, cSerialPort.ToString());
            mf.Tls.SaveProperty("ManualPWM" + IDname, cManualPWM.ToString());

            mf.Tls.SaveProperty("ScaleUnitsCal" + IDname, cScaleUnitsCal.ToString());
            mf.Tls.SaveProperty("ScaleTare" + IDname, cScaleTare.ToString());

            mf.Tls.SaveProperty("KP" + IDname, PIDtoArduino.KP.ToString());
            mf.Tls.SaveProperty("KI" + IDname, PIDtoArduino.KI.ToString());
            mf.Tls.SaveProperty("KD" + IDname, PIDtoArduino.KD.ToString());
            mf.Tls.SaveProperty("MinPWM" + IDname, PIDtoArduino.MinPWM.ToString());
            mf.Tls.SaveProperty("MaxPWM" + IDname, PIDtoArduino.MaxPWM.ToString());

            mf.Tls.SaveProperty("OnScreen" + IDname, cOnScreen.ToString());
            mf.Tls.SaveProperty("BumpButtons" + IDname, cBumpButtons.ToString());
            mf.Tls.SaveProperty("ConstantUPM" + IDname, cConstantUPM.ToString());
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
                if (RealNano && mf.SimMode == SimType.VirtualNano)
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

        private bool ProductOn()
        {
            bool Result = false;
            if (ControlType == ControlTypeEnum.Fan)
            {
                Result = ArduinoModule.Connected();
            }
            else
            {
                Result = (ArduinoModule.Connected() && mf.AutoSteerPGN.Connected() && cHectaresPerMinute > 0);
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

                    if (Rt >= .9 && Rt <= 1.1 && mf.SwitchBox.SwitchOn(SwIDs.Auto))
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
            if (mf.SimMode == SimType.Speed)
            {
                return mf.SimSpeed;
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
            if (mf.SimMode == SimType.Speed)
            {
                if (mf.UseInches)
                {
                    return mf.SimSpeed / 0.621371;  // convert mph back to kmh
                }
                else
                {
                    return mf.SimSpeed;
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
                    Result = Percent / 100.0 * cRateSet;
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
                    if (cConstantUPM)
                    {
                        double HPM = mf.Sections.TotalWidth(false) * KMH() / 600.0;
                        V = TargetRate() * HPM * 2.47;
                    }
                    else
                    {
                        V = TargetRate() * cHectaresPerMinute * 2.47;
                    }
                    break;

                case 1:
                    // hectares
                    if (cConstantUPM)
                    {
                        double HPM = mf.Sections.TotalWidth(false) * KMH() / 600.0;
                        V = TargetRate() * HPM;
                    }
                    else
                    {
                        V = TargetRate() * cHectaresPerMinute;
                    }
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

            // added this back in to change from lb/min to ft^3/min, Moved from PGN32614.
            if (cEnableProdDensity && cProdDensity > 0) { V /= cProdDensity; }

            return V;
        }

        public void UDPcommFromArduino(byte[] data, int PGN)
        {
            try
            {
                if (mf.SimMode != SimType.VirtualNano)  // block pgns from real nano when simulation is with virtual nano
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
            string s = cQuantityDescription + "/" + mf.CoverageAbbr[CoverageUnits];
            return s;
        }

        public double UnitsApplied()
        {
            double Result = cUnitsApplied;
            if (cEnableProdDensity && cProdDensity > 0) Result *= cProdDensity;
            return Result;
        }

        public void Update()
        {
            DateTime UpdateStartTime;
            if (ArduinoModule.ModuleSending())
            //if (ArduinoModule.ModuleSending() && (mf.AutoSteerPGN.Connected() || CoverageUnits > 1)
            //    || mf.SimMode == SimType.Speed)
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

                if (CurrentMinutes < 0 || CurrentMinutes > 1 || PauseWork)
                {
                    CurrentMinutes = 0;
                    PauseWork = false;
                }

                // update worked area
                cHectaresPerMinute = mf.Sections.WorkingWidth(false) * KMH()/ 600.0;
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
                    if (mf.SimMode == SimType.VirtualNano)
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
            double AccumulatedUnits;

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