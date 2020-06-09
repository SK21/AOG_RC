using System;

namespace RateController
{
    public enum SimType
    {
        None,
        VirtualNano,
        RealNano
    }

    public class CRateCals
    {
        public readonly FormRateControl mf;

        public bool AogConnected = false;

        public PGN32740 AogRec32740 = new PGN32740();

        public bool ArduinoConnected = false;
        public byte CoverageUnits = 0;
        public bool EraseApplied = false;
        public double FlowCal = 0;
        public clsArduino Nano;
        public byte QuantityUnits = 0;
        public double RateSet = 0;
        public PGN32761 Switches32761;                  //to Rate Controller from arduino, to AOG from Rate Controller

        public double TankSize = 0;

        public byte ValveType = 0;        // 0 standard, 1 fast close

        private DateTime AogReceiveTime;

        private PGN32741 ArdRec32741 = new PGN32741();

        private PGN32742 ArdSend32742;                  // to Arduino from Rate Controller

        private DateTime ArduinoReceiveTime;
        private double Coverage = 0;
        private string[] CoverageDescriptions = new string[] { "Acre", "Hectare", "Minute", "Hour" };
        private SimType cSimulationType = 0;
        private double CurrentMinutes;
        private double CurrentQuantity = 0;
        private double CurrentWidth;
        private double CurrentWorkedArea = 0;
        private double HectaresPerMinute = 0;
        private double LastAccQuantity = 0;
        private double LastQuantityDifference = 0;
        private DateTime LastTime;
        private double LastWorkedArea = 0;

        private bool PauseArea = false;

        private DateTime QcheckLast;
        private double QuantityApplied = 0;
        private string[] QuantityDescriptions = new string[] { "Imp. Gallons", "US Gallons", "Lbs", "Lbs N (NH3)", "Litres", "Kgs", "Kgs N (NH3)" };
        private double Ratio;
        private DateTime StartTime;

        private double TankRemaining = 0;

        private double TotalWorkedArea = 0;

        private PGN32744 ValveCal32744;                  // to Arduino from Rate Controller

        public CRateCals(FormRateControl CallingForm)
        {
            mf = CallingForm;

            Switches32761 = new PGN32761(this);

            ArdSend32742 = new PGN32742(this);
            ValveCal32744 = new PGN32744(this);

            Nano = new clsArduino(this);

            LoadSettings();
            PauseArea = true;
        }

        public byte MaxPWM { get { return ValveCal32744.MaxPWM; } set { ValveCal32744.MaxPWM = value; } }
        public byte MinPWM { get { return ValveCal32744.MinPWM; } set { ValveCal32744.MinPWM = value; } }
        public double SecondsAve { get { return ArdRec32741.SecondsAverage; } set { ArdRec32741.SecondsAverage = value; } }
        public int SendTime { get { return ValveCal32744.SendTime; } set { ValveCal32744.SendTime = value; } }
        public SimType SimulationType { get { return cSimulationType; } set { cSimulationType = value; } }  // 0 none, 1 virtual nano, 2 real nano

        public int VCN { get { return ValveCal32744.VCN; } set { ValveCal32744.VCN = value; } }
        public int WaitTime { get { return ValveCal32744.WaitTime; } set { ValveCal32744.WaitTime = value; } }

        public string AverageRate()
        {
            if (ArduinoConnected & AogConnected
                & HectaresPerMinute > 0 & Coverage > 0)
            {
                return (QuantityApplied / Coverage).ToString("N1");
            }
            else
            {
                return "0.0";
            }
        }

        public bool CommFromArduino(string sentence, bool RealNano = true)
        {
            bool Result = false;    // return true if there is good comm
            try
            {
                int end = sentence.IndexOf("\r");
                sentence = sentence.Substring(0, end);
                string[] words = sentence.Split(',');

                if (RealNano & SimulationType == SimType.VirtualNano)
                {
                    // block PGN32741 from real nano when simulation is with virtual nano
                    // do nothing
                }
                else
                {
                    if (ArdRec32741.ParseStringData(words))
                    {
                        UpdateQuantity(ArdRec32741.AccumulatedQuantity());
                        ArduinoReceiveTime = DateTime.Now;
                        Result = true;
                    }
                }

                if (Switches32761.ParseStringData(words))
                {
                    ArduinoReceiveTime = DateTime.Now;
                    Result = true;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return Result;
        }

        public string CoverageDescription()
        {
            return CoverageDescriptions[CoverageUnits] + "s";
        }

        public string CurrentApplied()
        {
            return QuantityApplied.ToString("N0");
        }

        public string CurrentCoverage()
        {
            return Coverage.ToString("N1");
        }

        public string CurrentRate()
        {
            if (ArduinoConnected & AogConnected & HectaresPerMinute > 0)
            {
                return RateApplied().ToString("N1");
            }
            else
            {
                return "0.0";
            }
        }

        public string CurrentTankRemaining()
        {
            return TankRemaining.ToString("N0");
        }

        public void LoadSettings()
        {
            double.TryParse(mf.Tls.LoadProperty("Coverage"), out Coverage);
            byte.TryParse(mf.Tls.LoadProperty("CoverageUnits"), out CoverageUnits);

            double.TryParse(mf.Tls.LoadProperty("TankRemaining"), out TankRemaining);
            double.TryParse(mf.Tls.LoadProperty("QuantityApplied"), out QuantityApplied);
            byte.TryParse(mf.Tls.LoadProperty("QuantityUnits"), out QuantityUnits);
            double.TryParse(mf.Tls.LoadProperty("LastAccQuantity"), out LastAccQuantity);

            double.TryParse(mf.Tls.LoadProperty("RateSet"), out RateSet);
            double.TryParse(mf.Tls.LoadProperty("FlowCal"), out FlowCal);
            double.TryParse(mf.Tls.LoadProperty("TankSize"), out TankSize);
            byte.TryParse(mf.Tls.LoadProperty("ValveType"), out ValveType);
            Enum.TryParse(mf.Tls.LoadProperty("cSimulationType"), true, out cSimulationType);

            int.TryParse(mf.Tls.LoadProperty("VCN"), out ValveCal32744.VCN);
            int.TryParse(mf.Tls.LoadProperty("SendTime"), out ValveCal32744.SendTime);
            int.TryParse(mf.Tls.LoadProperty("WaitTime"), out ValveCal32744.WaitTime);
            byte.TryParse(mf.Tls.LoadProperty("MaxPWM"), out ValveCal32744.MaxPWM);
            byte.TryParse(mf.Tls.LoadProperty("MinPWM"), out ValveCal32744.MinPWM);

            double temp;
            double.TryParse(mf.Tls.LoadProperty("SecondsAverage"), out temp);
            ArdRec32741.SecondsAverage = temp;
        }

        public double PWM()
        {
            return ArdRec32741.PWMsetting();
        }

        public double RateApplied()
        {
            if (HectaresPerMinute > 0)
            {
                double V = 0;
                switch (CoverageUnits)
                {
                    case 0:
                        // acres
                        V = ArdRec32741.UPMaverage() / (HectaresPerMinute * 2.47);
                        break;

                    case 1:
                        // hectares
                        V = ArdRec32741.UPMaverage() / HectaresPerMinute;
                        break;

                    case 2:
                        // minutes
                        V = ArdRec32741.UPMaverage();
                        break;

                    default:
                        // hours
                        V = ArdRec32741.UPMaverage() * 60;
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
            QuantityApplied = 0;
            EraseApplied = true;
        }

        public void ResetCoverage()
        {
            Coverage = 0;
            LastWorkedArea = AogRec32740.WorkedArea();
            LastTime = DateTime.Now;
        }

        public void ResetTank()
        {
            TankRemaining = TankSize;
        }

        public void SaveSettings()
        {
            mf.Tls.SaveProperty("Coverage", Coverage.ToString());
            mf.Tls.SaveProperty("CoverageUnits", CoverageUnits.ToString());

            mf.Tls.SaveProperty("TankRemaining", TankRemaining.ToString());
            mf.Tls.SaveProperty("QuantityApplied", QuantityApplied.ToString());
            mf.Tls.SaveProperty("QuantityUnits", QuantityUnits.ToString());
            mf.Tls.SaveProperty("LastAccQuantity", LastAccQuantity.ToString());

            mf.Tls.SaveProperty("RateSet", RateSet.ToString());
            mf.Tls.SaveProperty("FlowCal", FlowCal.ToString());
            mf.Tls.SaveProperty("TankSize", TankSize.ToString());
            mf.Tls.SaveProperty("ValveType", ValveType.ToString());
            mf.Tls.SaveProperty("cSimulationType", cSimulationType.ToString());

            mf.Tls.SaveProperty("VCN", ValveCal32744.VCN.ToString());
            mf.Tls.SaveProperty("SendTime", ValveCal32744.SendTime.ToString());
            mf.Tls.SaveProperty("WaitTime", ValveCal32744.WaitTime.ToString());
            mf.Tls.SaveProperty("MaxPWM", ValveCal32744.MaxPWM.ToString());
            mf.Tls.SaveProperty("MinPWM", ValveCal32744.MinPWM.ToString());

            mf.Tls.SaveProperty("SecondsAverage", ArdRec32741.SecondsAverage.ToString());
        }

        public byte SectionHi()
        {
            return AogRec32740.SectionControlByteHi;
        }

        public byte SectionLo()
        {
            return AogRec32740.SectionControlByteLo;
        }

        public void SetTankRemaining(double Remaining)
        {
            if (Remaining > 0 && Remaining <= 100000)
            {
                TankRemaining = Remaining;
            }
        }

        public double Speed()
        {
            if (CoverageUnits == 0)
            {
                return AogRec32740.Speed() * .621371;
            }
            else
            {
                return AogRec32740.Speed();
            }
        }

        public void UDPcommFromAOG(byte[] data)
        {
            if (AogRec32740.ParseByteData(data))
            {
                AogReceiveTime = DateTime.Now;
            }
        }

        public void UDPcommFromArduino(byte[] data)
        {
            try
            {
                if (SimulationType != SimType.VirtualNano)  // block PGN32741 from real nano when simulation is with virtual nano
                {
                    if (ArdRec32741.ParseByteData(data))
                    {
                        UpdateQuantity(ArdRec32741.AccumulatedQuantity());
                        ArduinoReceiveTime = DateTime.Now;
                    }
                }

                if (Switches32761.ParseByteData(data))
                {
                    ArduinoReceiveTime = DateTime.Now;
                }
            }
            catch (Exception)
            {
            }
        }

        public string Units()
        {
            string s = QuantityDescriptions[QuantityUnits] + " / " + CoverageDescriptions[CoverageUnits];
            return s;
        }

        public void Update()
        {
            StartTime = DateTime.Now;
            CurrentMinutes = (StartTime - LastTime).TotalMinutes;
            if (CurrentMinutes < 0) CurrentMinutes = 0;
            LastTime = StartTime;

            // check connections
            ArduinoConnected = ((StartTime - ArduinoReceiveTime).TotalSeconds < 4);
            AogConnected = ((StartTime - AogReceiveTime).TotalSeconds < 4);

            if (ArduinoConnected & AogConnected)
            {
                // still connected

                // worked area
                TotalWorkedArea = AogRec32740.WorkedArea(); // hectares

                if (PauseArea)
                {
                    // exclude area worked while paused
                    LastWorkedArea = TotalWorkedArea;
                    PauseArea = false;
                }
                CurrentWorkedArea = TotalWorkedArea - LastWorkedArea;
                LastWorkedArea = TotalWorkedArea;

                // work rate
                CurrentWidth = AogRec32740.WorkingWidth();

                HectaresPerMinute = CurrentWidth * AogRec32740.Speed() * 0.1 / 60.0;

                //coverage
                if (HectaresPerMinute > 0)    // Is application on?
                {
                    switch (CoverageUnits)
                    {
                        case 0:
                            // acres
                            Coverage += CurrentWorkedArea * 2.47105;
                            break;

                        case 1:
                            // hectares
                            Coverage += CurrentWorkedArea;
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
            }
            else
            {
                // connection lost

                PauseArea = true;
            }

            if (ArduinoConnected) RateSet = Switches32761.NewRate(RateSet);

            if (AogConnected & ArduinoConnected)
            {
                // send to arduino
                ArdSend32742.Send();
                ValveCal32744.Send();

                // send comm to AOG
                Switches32761.Send();
            }

            if (cSimulationType == SimType.VirtualNano) Nano.MainLoop();
        }

        public double UPMapplied()
        {
            return ArdRec32741.UPMaverage();
        }

        public double UPMset()
        {
            return UPMsetting();
        }

        public double UPMsetting() // returns units per minute set rate
        {
            double V = 0;
            switch (CoverageUnits)
            {
                case 0:
                    // acres
                    V = RateSet * HectaresPerMinute * 2.47;
                    break;

                case 1:
                    // hectares
                    V = RateSet * HectaresPerMinute;
                    break;

                case 2:
                    // minutes
                    V = RateSet;
                    break;

                default:
                    // hours
                    V = RateSet / 60;
                    break;
            }
            return V;
        }

        public double Width()
        {
            if (CoverageUnits == 0)
            {
                return CurrentWidth * 3.28;
            }
            else
            {
                return CurrentWidth;
            }
        }

        public double WorkRate()
        {
            if (CoverageUnits == 0)
            {
                return HectaresPerMinute * 2.47105 * 60;
            }
            else
            {
                return HectaresPerMinute * 60;
            }
        }

        private bool QuantityValid(double CurrentDifference)
        {
            bool Result = true;
            try
            {
                // check quantity error
                if (LastQuantityDifference > 0)
                {
                    Ratio = CurrentDifference / LastQuantityDifference;
                    if ((Ratio > 4) | (Ratio < 0.25))
                    {
                        mf.Tls.WriteActivityLog("Quantity Check Ratio: " + Ratio.ToString("N2")
                            + " Current Amount: " + CurrentDifference.ToString("N2") + " Last Amount: " + LastQuantityDifference.ToString("N2")
                            + " RateSet: " + RateSet.ToString("N2") + " Current Rate: " + CurrentRate() + "\n");
                    }
                    if (Ratio > 10) Result = false; // too much of a change in quantity
                }

                // check rate error
                if (RateSet > 0)
                {
                    Ratio = RateApplied() / RateSet;
                    if ((Ratio > 1.5) | (Ratio < 0.67))
                    {
                        mf.Tls.WriteActivityLog("Rate Check Ratio: " + Ratio.ToString("N2")
                            + " Current Amount: " + CurrentDifference.ToString("N2") + " Last Amount: " + LastQuantityDifference.ToString("N2")
                            + " RateSet: " + RateSet.ToString("N2") + " Current Rate: " + CurrentRate() + "\n");
                    }
                }

                // record values periodically
                if ((DateTime.Now - QcheckLast).TotalMinutes > 30)
                {
                    QcheckLast = DateTime.Now;

                    if (LastQuantityDifference > 0)
                    {
                        Ratio = CurrentDifference / LastQuantityDifference;
                        mf.Tls.WriteActivityLog("Quantity Check Ratio: " + Ratio.ToString("N2")
                            + " Current Amount: " + CurrentDifference.ToString("N2") + " Last Amount: " + LastQuantityDifference.ToString("N2")
                            + " RateSet: " + RateSet.ToString("N2") + " Current Rate: " + CurrentRate() + "\n");
                    }
                }

                LastQuantityDifference = CurrentDifference;
                return Result;
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("cRateCals: QuantityValid: " + ex.Message);
                return false;
            }
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
                    QuantityApplied += CurrentQuantity;
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