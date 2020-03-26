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
    public class CRateCals
    {
        public readonly FormRateControl mf;

        public PGN32761 Switches32761;
        
        // Arduino
        private PGN35000 ArdSend35000;
        private PGN35100 ArdSend35100;
        private PGN35200 ArdRec35200 = new PGN35200();

        // AgOpenGPS
        public PGN35400 AogRec35400 = new PGN35400();

        string[] QuantityDescriptions = new string[] { "Imp. Gallons", "US Gallons", "Lbs", "Lbs NH3", "Litres", "Kgs", "Kgs NH3" };
        string[] CoverageDescriptions = new string[] { "Acre", "Hectare", "Minute", "Hour" };

        private double TankRemaining = 0;
        private DateTime StartTime;
        private double CurrentMinutes;
        private DateTime LastTime;

        private double QuantityApplied = 0;
        private double CurrentQuantity = 0;
        private double LastQuantity = 0;
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

        public bool SimFlow = false;
        private double CurrentWidth;

        public bool ArduinoConnected = false;
        private DateTime ArduinoReceiveTime;

        public bool AogConnected = false;
        private DateTime AogReceiveTime;

        private bool PauseArea = false;

        public CRateCals(FormRateControl CallingForm)
        {
            mf = CallingForm;

            Switches32761 = new PGN32761(this);

            ArdSend35000 = new PGN35000(this);
            ArdSend35100 = new PGN35100(this);

            LoadSettings();
        }

        public bool SimulateFlow { get { return SimFlow; } set { SimFlow = value; } }

        public byte KP { get { return ArdSend35100.KP; } set { ArdSend35100.KP = value; } }

        public byte KI { get { return ArdSend35100.KI; } set { ArdSend35100.KI = value; } }

        public byte KD { get { return ArdSend35100.KD; } set { ArdSend35100.KD = value; } }

        public byte DeadBand { get { return ArdSend35100.Deadband; } set { ArdSend35100.Deadband = value; } }

        public byte MinPWM { get { return ArdSend35100.MinPWM; } set { ArdSend35100.MinPWM = value; } }

        public byte MaxPWM { get { return ArdSend35100.MaxPWM; } set { ArdSend35100.MaxPWM = value; } }

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
                TotalWorkedArea = AogRec35400.WorkedArea(); // hectares

                if (PauseArea)
                {
                    // exclude area worked while paused
                    LastWorkedArea = TotalWorkedArea;
                    PauseArea = false;
                }
                CurrentWorkedArea = TotalWorkedArea - LastWorkedArea;
                LastWorkedArea = TotalWorkedArea;

                // work rate
                CurrentWidth = AogRec35400.WorkingWidth();

                HectaresPerMinute = CurrentWidth * AogRec35400.Speed() * 0.1 / 60.0;

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

            // send to arduino
            if (AogConnected)
            {
                ArdSend35000.Send();
                ArdSend35100.Send();
            }

            // send comm to AOG
            Switches32761.Send();
        }

        private void UpdateQuantity(double AccQuantity)
        {
            if (AccQuantity > LastQuantity)
            {
                CurrentQuantity = AccQuantity - LastQuantity;
                LastQuantity = AccQuantity;

                // tank remaining
                TankRemaining -= CurrentQuantity;

                // quantity applied
                QuantityApplied += CurrentQuantity;
            }
            else
            {
                // reset
                LastQuantity = AccQuantity;
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
                    V = RateSet * HectaresPerMinute ;
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
                        V = ArdRec35200.UPMaverage() / (HectaresPerMinute * 2.47);
                        break;
                    case 1:
                        // hectares
                        V = ArdRec35200.UPMaverage() / HectaresPerMinute;
                        break;
                    case 2:
                        // minutes
                        V = ArdRec35200.UPMaverage();
                        break;
                    default:
                        // hours
                        V = ArdRec35200.UPMaverage() * 60;
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

        public void ResetCoverage()
        {
            Coverage = 0;
            LastWorkedArea = AogRec35400.WorkedArea();
            LastTime = DateTime.Now;
        }

        public void ResetTank()
        {
            TankRemaining = TankSize;
        }

        public string CurrentRate()
        {
            if (ArduinoConnected & AogConnected & HectaresPerMinute  > 0)
            {
                return RateApplied().ToString("N1");
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

        public void CommFromArduino(string sentence)
        {
            int end = sentence.IndexOf("\r");
            sentence = sentence.Substring(0, end);
            string[] words = sentence.Split(',');
            if (ArdRec35200.ParseStringData(words))
            {
                UpdateQuantity(ArdRec35200.AccumulatedQuantity());
            }

            if (Switches32761.ParseStringData(words))
            {
                ArduinoReceiveTime = DateTime.Now;
            }
        }

        public void UDPcommFromArduino(byte[] data)
        {
            if (ArdRec35200.ParseByteData(data))
            {
                UpdateQuantity(ArdRec35200.AccumulatedQuantity());
            }

            if (Switches32761.ParseByteData(data))
            {
                ArduinoReceiveTime = DateTime.Now;
            }
        }

       public void UDPcommFromAOG(byte[] data)
        {
            if(AogRec35400.ParseByteData(data))
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
            SimFlow = Properties.Settings.Default.SimulateFlow;
            ArdSend35100.KP = Properties.Settings.Default.KP;
            ArdSend35100.KI = Properties.Settings.Default.KI;
            ArdSend35100.KD = Properties.Settings.Default.KD;
            ArdSend35100.Deadband = Properties.Settings.Default.DeadBand;
            ArdSend35100.MinPWM = Properties.Settings.Default.MinPWM;
            ArdSend35100.MaxPWM = Properties.Settings.Default.MaxPWM;
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
            Properties.Settings.Default.SimulateFlow = SimFlow;
            Properties.Settings.Default.KP = ArdSend35100.KP;
            Properties.Settings.Default.KI = ArdSend35100.KI;
            Properties.Settings.Default.KD = ArdSend35100.KD;
            Properties.Settings.Default.DeadBand = ArdSend35100.Deadband;
            Properties.Settings.Default.MinPWM = ArdSend35100.MinPWM;
            Properties.Settings.Default.MaxPWM = ArdSend35100.MaxPWM;
        }
    }
}
