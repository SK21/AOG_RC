using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

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

        public PGN32761 Switches32761;                  //to Rate Controller from arduino, to AOG from Rate Controller				

        // Arduino
        private PGN32742 ArdSend32742;                  // to Arduino from Rate Controller
        private PGN32743 ArdSend32743;                  // to Arduino from Rate Controller
        private PGN32741 ArdRec32741 = new PGN32741();  // to Rate Controller from Arduino

        // AgOpenGPS
        public PGN32740 AogRec32740 = new PGN32740();   // to Rate Controller from AOG

        string[] QuantityDescriptions = new string[] { "Imp. Gallons", "US Gallons", "Lbs", "Lbs N (NH3)", "Litres", "Kgs", "Kgs N (NH3)" };
        string[] CoverageDescriptions = new string[] { "Acre", "Hectare", "Minute", "Hour" };

        private double TankRemaining = 0;
        private DateTime StartTime;
        private double CurrentMinutes;
        private DateTime LastTime;

        private double QuantityApplied = 0;
        private double CurrentQuantity = 0;
        private double LastAccQuantity = 0;
        private double LastQuantityDifference = 0;

        public bool EraseApplied = false;

        private double Coverage = 0;
        private double TotalWorkedArea = 0;
        private double LastWorkedArea = 0;
        private double CurrentWorkedArea = 0;
        private double HectaresPerMinute = 0;

        public byte QuantityUnits = 0;
        public byte CoverageUnits = 0;
        public double RateSet = 0;
        public double FlowCal = 0;
        public double TankSize = 0;
        public byte ValveType = 0;  // 0 standard, 1 fast close

        private double CurrentWidth;

        public bool ArduinoConnected = false;
        private DateTime ArduinoReceiveTime;

        public bool AogConnected = false;
        private DateTime AogReceiveTime;

        private bool PauseArea = false;
        public clsArduino Nano;
        private SimType cSimulationType = 0; // 0 none, 1 virtual nano, 2 real nano

        double Ratio;
        DateTime QcheckLast;

        public CRateCals(FormRateControl CallingForm)
        {
            mf = CallingForm;

            Switches32761 = new PGN32761(this);

            ArdSend32742 = new PGN32742(this);
            ArdSend32743 = new PGN32743(this);

            Nano = new clsArduino(this);

            LoadSettings();
        }

        public byte KP { get { return ArdSend32743.KP; } set { ArdSend32743.KP = value; } }

        public byte KI { get { return ArdSend32743.KI; } set { ArdSend32743.KI = value; } }

        public byte KD { get { return ArdSend32743.KD; } set { ArdSend32743.KD = value; } }

        public byte DeadBand { get { return ArdSend32743.Deadband; } set { ArdSend32743.Deadband = value; } }

        public byte MinPWM { get { return ArdSend32743.MinPWM; } set { ArdSend32743.MinPWM = value; } }

        public byte MaxPWM { get { return ArdSend32743.MaxPWM; } set { ArdSend32743.MaxPWM = value; } }

        public SimType SimulationType { get { return cSimulationType; } set { cSimulationType = value; } }

        public byte AdjustmentFactor { get { return ArdSend32743.AdjustmentFactor; } set { ArdSend32743.AdjustmentFactor = value; } }

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
                ArdSend32743.Send();

                // send comm to AOG
                Switches32761.Send();
            }

            if (cSimulationType == SimType.VirtualNano) Nano.MainLoop();
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

        public void ResetApplied()
        {
            QuantityApplied = 0;
            EraseApplied = true;
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

        public string Units()
        {
            string s = QuantityDescriptions[QuantityUnits] + " / " + CoverageDescriptions[CoverageUnits];
            return s;
        }

        public string CoverageDescription()
        {
            return CoverageDescriptions[CoverageUnits] + "s";
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

        public string CurrentCoverage()
        {
            return Coverage.ToString("N1");
        }

        public string CurrentTankRemaining()
        {
            return TankRemaining.ToString("N0");
        }

        public string CurrentApplied()
        {
            return QuantityApplied.ToString("N0");
        }

        public void SetTankRemaining(double Remaining)
        {
            if (Remaining > 0 && Remaining <= 100000)
            {
                TankRemaining = Remaining;
            }
        }

        public void CommFromArduino(string sentence, bool RealNano = true)
        {
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
                    }
                }

                if (Switches32761.ParseStringData(words))
                {
                    ArduinoReceiveTime = DateTime.Now;
                }
            }
            catch (Exception)
            {

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

        public void UDPcommFromAOG(byte[] data)
        {
            if (AogRec32740.ParseByteData(data))
            {
                AogReceiveTime = DateTime.Now;
            }
        }

        void LoadSettings()
        {
            Coverage = Properties.Settings.Default.Coverage;
            TankRemaining = Properties.Settings.Default.TankRemaining;
            QuantityApplied = Properties.Settings.Default.QuantityApplied;
            QuantityUnits = Properties.Settings.Default.QuantityUnits;
            CoverageUnits = Properties.Settings.Default.CoverageUnits;
            RateSet = Properties.Settings.Default.RateSet;
            FlowCal = Properties.Settings.Default.FlowCal;
            TankSize = Properties.Settings.Default.TankSize;
            ValveType = Properties.Settings.Default.ValveType;
            ArdSend32743.KP = Properties.Settings.Default.KP;
            ArdSend32743.KI = Properties.Settings.Default.KI;
            ArdSend32743.KD = Properties.Settings.Default.KD;
            ArdSend32743.Deadband = Properties.Settings.Default.DeadBand;
            ArdSend32743.MinPWM = Properties.Settings.Default.MinPWM;
            ArdSend32743.MaxPWM = Properties.Settings.Default.MaxPWM;
            cSimulationType = (SimType)(Properties.Settings.Default.SimulateType);
            ArdSend32743.AdjustmentFactor = Properties.Settings.Default.AdjustmentFactor;
        }

        public void SaveSettings()
        {
            Properties.Settings.Default.Coverage = Coverage;
            Properties.Settings.Default.TankRemaining = TankRemaining;
            Properties.Settings.Default.QuantityApplied = QuantityApplied;
            Properties.Settings.Default.QuantityUnits = QuantityUnits;
            Properties.Settings.Default.CoverageUnits = CoverageUnits;
            Properties.Settings.Default.RateSet = RateSet;
            Properties.Settings.Default.FlowCal = FlowCal;
            Properties.Settings.Default.TankSize = TankSize;
            Properties.Settings.Default.ValveType = ValveType;
            Properties.Settings.Default.KP = ArdSend32743.KP;
            Properties.Settings.Default.KI = ArdSend32743.KI;
            Properties.Settings.Default.KD = ArdSend32743.KD;
            Properties.Settings.Default.DeadBand = ArdSend32743.Deadband;
            Properties.Settings.Default.MinPWM = ArdSend32743.MinPWM;
            Properties.Settings.Default.MaxPWM = ArdSend32743.MaxPWM;
            Properties.Settings.Default.SimulateType = (int)cSimulationType;
            Properties.Settings.Default.AdjustmentFactor = ArdSend32743.AdjustmentFactor;
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
    }
}
