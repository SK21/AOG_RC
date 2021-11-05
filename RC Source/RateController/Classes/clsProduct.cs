using System;
using System.Diagnostics;

namespace RateController
{
    public enum ControlType { Standard, FastClose, Motor }

    public class clsProduct
    {
        public readonly FormStart mf;

        public PGN32613 ArduinoModule;
        public byte CoverageUnits = 0;
        public double cRateSet = 0;
        public bool EraseApplied = false;
        public double FlowCal = 0;
        public byte QuantityUnits = 0;
        public double TankSize = 0;
        public byte ValveType = 0;  // 0 standard, 1 fast close, 3 motor
        public clsArduino VirtualNano;
        
        private int cCountsRev;
        private double cHectaresPerMinute;
        private double cManualRateFactor = 1;
        private double cMinUPM;
        private int cModID; // arduino ID, 0-15, high 4 bits
        private byte cOffRateSetting;
        private double Coverage = 0;
        private int cProductID;
        private string cProductName = "";
        private double cQuantityApplied = 0;
        
        private int cSenID; // rate sensor ID on arduino, 0-15, low 4 bits
        
        private SimType cSimulationType = 0;

        private double CurrentMinutes;
        private double CurrentQuantity = 0;
        private double CurrentWorkedArea_Hc = 0;
        private bool cUseMultiPulse;
        private bool cUseOffRateAlarm;
        private byte cVariableRate = 0;
        private double cWorkingWidth_cm;
        private double LastAccQuantity = 0;
        private double LastQuantityDifference = 0;
        private DateTime LastUpdateTime;
        private bool PauseWork = false;
        private PGN32616 PIDtoArduino;
        private PGN32614 RateToArduino;
        private bool SwitchIDsSent;
        private double TankRemaining = 0;
        private DateTime UpdateStartTime;
        private byte[] VRconversion = { 255, 0, 1, 2, 3, 4 };   // 255 = off

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

        public int ID { get { return cProductID; } }
        public double ManualRateFactor { get { return cManualRateFactor; } set { cManualRateFactor = value; } }
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

        public byte PIDbrakepoint { get { return PIDtoArduino.BrakePoint; } set { PIDtoArduino.BrakePoint = value; } }
        public byte PIDdeadband { get { return PIDtoArduino.DeadBand; } set { PIDtoArduino.DeadBand = value; } }
        public byte PIDHighMax { get { return PIDtoArduino.HighMax; } set { PIDtoArduino.HighMax = value; } }
        public byte PIDkp { get { return PIDtoArduino.KP; } set { PIDtoArduino.KP = value; } }
        public byte PIDLowMax { get { return PIDtoArduino.LowMax; } set { PIDtoArduino.LowMax = value; } }
        public byte PIDminPWM { get { return PIDtoArduino.MinPWM; } set { PIDtoArduino.MinPWM = value; } }
        public byte PIDTimed { get { return PIDtoArduino.TimedAdjustment; } set { PIDtoArduino.TimedAdjustment = value; } }
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
                    cProductName = "Product " + (cProductID + 1).ToString();
                }
                else
                {
                    cProductName = value;
                }
            }
        }

        public double RateSet { get { return cRateSet; } set { cRateSet = value; } }

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

        public SimType SimulationType { get { return cSimulationType; } set { cSimulationType = value; } }

        public bool UseMultiPulse { get { return cUseMultiPulse; } set { cUseMultiPulse = value; } }

        public bool UseOffRateAlarm { get { return cUseOffRateAlarm; } set { cUseOffRateAlarm = value; } }

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

        private string IDname { get { return cProductID.ToString(); } }

        public double AverageRate()
        {
            if (ArduinoModule.Connected() & mf.AutoSteerPGN.Connected()
                & cHectaresPerMinute > 0 & Coverage > 0)
            {
                return (cQuantityApplied / Coverage);
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

        public string CurrentApplied()
        {
            return cQuantityApplied.ToString("N0");
        }

        public string CurrentCoverage()
        {
            return Coverage.ToString("N1");
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

        public double CurrentTankRemaining()
        {
            return TankRemaining;
        }

        public void Load()
        {
            double.TryParse(mf.Tls.LoadProperty("Coverage" + IDname), out Coverage);
            byte.TryParse(mf.Tls.LoadProperty("CoverageUnits" + IDname), out CoverageUnits);

            double.TryParse(mf.Tls.LoadProperty("TankRemaining" + IDname), out TankRemaining);
            double.TryParse(mf.Tls.LoadProperty("QuantityApplied" + IDname), out cQuantityApplied);
            byte.TryParse(mf.Tls.LoadProperty("QuantityUnits" + IDname), out QuantityUnits);
            double.TryParse(mf.Tls.LoadProperty("LastAccQuantity" + IDname), out LastAccQuantity);

            double.TryParse(mf.Tls.LoadProperty("RateSet" + IDname), out cRateSet);
            double.TryParse(mf.Tls.LoadProperty("FlowCal" + IDname), out FlowCal);
            double.TryParse(mf.Tls.LoadProperty("TankSize" + IDname), out TankSize);
            byte.TryParse(mf.Tls.LoadProperty("ValveType" + IDname), out ValveType);
            Enum.TryParse(mf.Tls.LoadProperty("cSimulationType" + IDname), true, out cSimulationType);

            cProductName = mf.Tls.LoadProperty("ProductName" + IDname);

            byte.TryParse(mf.Tls.LoadProperty("KP" + IDname), out PIDtoArduino.KP);
            byte.TryParse(mf.Tls.LoadProperty("PIDMinPWM" + IDname), out PIDtoArduino.MinPWM);
            byte.TryParse(mf.Tls.LoadProperty("PIDLowMax" + IDname), out PIDtoArduino.LowMax);

            byte.TryParse(mf.Tls.LoadProperty("PIDHighMax" + IDname), out PIDtoArduino.HighMax);
            byte.TryParse(mf.Tls.LoadProperty("PIDdeadband" + IDname), out PIDtoArduino.DeadBand);
            byte.TryParse(mf.Tls.LoadProperty("PIDbrakepoint" + IDname), out PIDtoArduino.BrakePoint);
            byte.TryParse(mf.Tls.LoadProperty("TimedAdjustment" + IDname), out PIDtoArduino.TimedAdjustment);

            bool.TryParse(mf.Tls.LoadProperty("UseMultiPulse" + IDname), out cUseMultiPulse);
            int.TryParse(mf.Tls.LoadProperty("CountsRev" + IDname), out cCountsRev);

            int tmp;
            int.TryParse(mf.Tls.LoadProperty("SensorID" + IDname), out tmp);
            SensorID = (byte)tmp;

            tmp = 0;
            int.TryParse(mf.Tls.LoadProperty("ModuleID" + IDname), out tmp);
            ModuleID = (byte)tmp;

            bool.TryParse(mf.Tls.LoadProperty("OffRateAlarm" + IDname), out cUseOffRateAlarm);
            byte.TryParse(mf.Tls.LoadProperty("OffRateSetting" + IDname), out cOffRateSetting);

            double.TryParse(mf.Tls.LoadProperty("MinUPM" + IDname), out cMinUPM);
            byte.TryParse(mf.Tls.LoadProperty("VariableRate" + IDname), out cVariableRate);
        }

        public double PWM()
        {
            return ArduinoModule.PWMsetting();
        }

        public double QuantityApplied()
        {
            return cQuantityApplied;
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
            cQuantityApplied = 0;
            EraseApplied = true;
        }

        public void ResetCoverage()
        {
            Coverage = 0;
            LastUpdateTime = DateTime.Now;
        }

        public void ResetTank()
        {
            TankRemaining = TankSize;
        }

        public void Save()
        {
            mf.Tls.SaveProperty("Coverage" + IDname, Coverage.ToString());
            mf.Tls.SaveProperty("CoverageUnits" + IDname, CoverageUnits.ToString());

            mf.Tls.SaveProperty("TankRemaining" + IDname, TankRemaining.ToString());
            mf.Tls.SaveProperty("QuantityApplied" + IDname, cQuantityApplied.ToString());
            mf.Tls.SaveProperty("QuantityUnits" + IDname, QuantityUnits.ToString());
            mf.Tls.SaveProperty("LastAccQuantity" + IDname, LastAccQuantity.ToString());

            mf.Tls.SaveProperty("RateSet" + IDname, cRateSet.ToString());
            mf.Tls.SaveProperty("FlowCal" + IDname, FlowCal.ToString());
            mf.Tls.SaveProperty("TankSize" + IDname, TankSize.ToString());
            mf.Tls.SaveProperty("ValveType" + IDname, ValveType.ToString());
            mf.Tls.SaveProperty("cSimulationType" + IDname, cSimulationType.ToString());

            mf.Tls.SaveProperty("ProductName" + IDname, cProductName);

            mf.Tls.SaveProperty("KP" + IDname, PIDtoArduino.KP.ToString());
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
                        UpdateQuantity(ArduinoModule.AccumulatedQuantity());
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

        public void SetTankRemaining(double Remaining)
        {
            if (Remaining > 0 && Remaining <= 100000)
            {
                TankRemaining = Remaining;
            }
        }

        public double SmoothRate()
        {
            if (ArduinoModule.Connected() & mf.AutoSteerPGN.Connected() & cHectaresPerMinute > 0)
            {
                if (TargetRate() > 0)
                {
                    double Rt = RateApplied() / TargetRate();

                    if (Rt >= .9 & Rt <= 1.1 & mf.Sections.SwitchOn(Switches.Auto))
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
            if (CoverageUnits == 0)
            {
                return mf.AutoSteerPGN.Speed_KMH() * 0.621371;
            }
            else
            {
                return mf.AutoSteerPGN.Speed_KMH();
            }
        }

        public double TargetRate()
        {
            double Result = cRateSet;
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

        public void UDPcommFromArduino(byte[] data)
        {
            try
            {
                if (SimulationType != SimType.VirtualNano)  // block PGN32613 from real nano when simulation is with virtual nano
                {
                    if (ArduinoModule.ParseByteData(data))
                    {
                        UpdateQuantity(ArduinoModule.AccumulatedQuantity());
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public string Units()
        {
            string s = mf.QuantityAbbr[QuantityUnits] + " / " + mf.CoverageAbbr[CoverageUnits];
            return s;
        }

        public void Update()
        {
            if (ArduinoModule.Connected() & mf.AutoSteerPGN.Connected())
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
                    if (mf.Sections.IsSectionOn(i))
                    {
                        cWorkingWidth_cm += mf.Sections.Item(i).Width_cm;
                    }
                }

                cHectaresPerMinute = (cWorkingWidth_cm / 100.0) * mf.AutoSteerPGN.Speed_KMH() / 600.0;
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
                PIDtoArduino.Send();
            }
            else
            {
                // connection lost
                PauseWork = true;
            }
        }

        public double UPMapplied()
        {
            return ArduinoModule.UPM();
        }

        public double Width()
        {
            if (CoverageUnits == 0)
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
            if (CoverageUnits == 0)
            {
                return cHectaresPerMinute * 2.47105 * 60;
            }
            else
            {
                return cHectaresPerMinute * 60;
            }
        }

        private bool QuantityValid(double CurrentDifference)
        {
            bool Result = true;
            double Ratio = 0;
            try
            {
                // check quantity error
                if (LastQuantityDifference > 0)
                {
                    Ratio = CurrentDifference / LastQuantityDifference;
                    if (Ratio > 10) Result = false; // too much of a change in quantity
                }
                else
                {
                    Result = false;
                }

                LastQuantityDifference = CurrentDifference;
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("clsProduct: QuantityValid: " + ex.Message);
                Result = false;
            }
            return Result;
        }

        private void UpdateQuantity(double AccQuantity)
        {
            if (AccQuantity > LastAccQuantity)
            {
                CurrentQuantity = AccQuantity - LastAccQuantity;
                LastAccQuantity = AccQuantity;

                if (QuantityValid(CurrentQuantity))
                {
                    // tank remaining
                    TankRemaining -= CurrentQuantity;

                    // quantity applied
                    cQuantityApplied += CurrentQuantity;
                }
            }
            else
            {
                // reset
                LastAccQuantity = AccQuantity;
            }
        }
    }
}