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
        private readonly FormRateControl mf;
        
        // Arduino
        private PGN31100 ArdRec31100 = new PGN31100();
        private PGN31200 ArdRec31200;
        private PGN31300 ArdSend31300 = new PGN31300();
        private PGN31400 ArdSend31400 = new PGN31400();

        // AgOpenGPS
        public PGN32100 AogRec32100 = new PGN32100();
        public PGN32200 AogRec32200 = new PGN32200();
        public PGN32300 AogSend32300 = new PGN32300();

        string[] QuantityDescriptions = new string[] { "Imp. Gallons", "US Gallons", "Lbs", "Lbs NH3", "Litres", "Kgs", "Kgs NH3" };
        string[] CoverageDescriptions = new string[] { "Acre", "Hectare", "Minute", "Hour" };

        private double UPM = 0; // units per minute
        private double TankRemaining = 0;
        private CAveraging UnitsPerMinute;
        private CAveraging AppRate;

        private DateTime StartTime;
        private double CurrentMinutes;
        private DateTime LastTime;

        private double QuantityApplied = 0;
        private double AccQuantity = 0;
        private double CurrentQuantity = 0;
        private double LastQuantity = 0;
        private bool EraseApplied = false;

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
        private byte OutCommand;

        private bool SimFlow = false;

        private double CurrentWidth;

        public bool ArduinoConnected = false;
        private DateTime ArduinoReceiveTime;

        public bool AogConnected = false;
        private DateTime AogReceiveTime;

        private bool PauseArea = false;
        private bool PauseQuantity = false;

        public CRateCals(FormRateControl CallingForm)
        {
            mf = CallingForm;
            ArdRec31200 = new PGN31200(this);

            LoadSettings();
            ArdRec31200.RateSet = RateSet;

            UnitsPerMinute = new CAveraging(4);
            AppRate = new CAveraging(4);
        }

        public bool SimulateFlow { get { return SimFlow; } set { SimFlow = value; } }

        public byte KP { get { return ArdSend31400.KP; } set { ArdSend31400.KP = value; } }

        public byte KI { get { return ArdSend31400.KI; } set { ArdSend31400.KI = value; } }

        public byte KD { get { return ArdSend31400.KD; } set { ArdSend31400.KD = value; } }

        public byte DeadBand { get { return ArdSend31400.Deadband; } set { ArdSend31400.Deadband = value; } }

        public byte MinPWM { get { return ArdSend31400.MinPWM; } set { ArdSend31400.MinPWM = value; } }

        public byte MaxPWM { get { return ArdSend31400.MaxPWM; } set { ArdSend31400.MaxPWM = value; } }

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
                TotalWorkedArea = AogRec32100.WorkedArea(); // hectares
                Debug.WriteLine("Total worked area " + TotalWorkedArea.ToString());

                if (PauseArea)
                {
                    // exclude area worked while paused
                    LastWorkedArea = TotalWorkedArea;
                    PauseArea = false;
                }
                CurrentWorkedArea = TotalWorkedArea - LastWorkedArea;
                LastWorkedArea = TotalWorkedArea;

                // work rate
                CurrentWidth = AogRec32100.WorkingWidth();

                HectaresPerMinute = CurrentWidth * AogRec32100.Speed() * .1 / 60;

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
                PauseQuantity = true;
            }

            CommToArduino();

            if (ArduinoConnected) RateSet = ArdRec31200.DoSwitches(RateSet);

            CommToAOG();
        }

        private void UpdateQuantity()
        {
            CurrentQuantity = 0;
            if (AccQuantity > LastQuantity)
            {
                if (PauseQuantity)
                {
                    // exclude quantity while paused
                    LastQuantity = AccQuantity;
                    PauseQuantity = false;
                }
                CurrentQuantity = AccQuantity - LastQuantity;
            }
            LastQuantity = AccQuantity;

            // tank remaining
            TankRemaining -= CurrentQuantity;

            // quantity applied
            QuantityApplied += CurrentQuantity;
        }

        private double UPMsetting() // returns units per minute set rate
        {
            double V;
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

        public string Units()
        {
            string s = QuantityDescriptions[QuantityUnits] + " / " + CoverageDescriptions[CoverageUnits];
            return s;
        }

        public void ResetCoverage()
        {
            Coverage = 0;
            LastWorkedArea = AogRec32100.WorkedArea();
            LastTime = DateTime.Now;
        }

        public void ResetTank()
        {
            TankRemaining = TankSize;
        }

        public void ResetApplied()
        {
            QuantityApplied = 0;
            EraseApplied = true;
        }

        public string CurrentRate()
        {
            if (ArduinoConnected & AogConnected & HectaresPerMinute  > 0)
            {
                return AppRate.Average().ToString("N1");
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


        public void CommToArduino()
        {
            // send data (A)

            ArdSend31300.RelayLo = AogRec32100.SectionControlByteLo;
            ArdSend31300.RelayHi = AogRec32100.SectionControlByteHi;
            int Temp = (int)(UPMsetting() * 100);
            ArdSend31300.RateSetHi = (byte)(Temp >> 8);
            ArdSend31300.RateSetLo = (byte)Temp;

            Temp = (int)(FlowCal * 100);
            ArdSend31300.FlowCalHi = (byte)(Temp >> 8);
            ArdSend31300.FlowCalLo = (byte)Temp;

            // command byte
            OutCommand = 0;
            if (EraseApplied) OutCommand |= 0b00000001;
            EraseApplied = false;

            switch (ValveType)
            {
                case 1:
                    OutCommand &= 0b11111011; // clear bit 2
                    OutCommand |= 0b00000010; // set bit 1
                    break;
                case 2:
                    OutCommand |= 0b00000100; // set bit 2
                    OutCommand &= 0b11111101; // clear bit 1
                    break;
                case 3:
                    OutCommand |= 0b00000110; // set bit 2 and 1
                    break;
                default:
                    OutCommand &= 0b11111001; // clear bit 2 and 1
                    break;
            }

            if (SimFlow) OutCommand |= 0b00001000; else OutCommand &= 0b11110111;

            ArdSend31300.Command = OutCommand;

            // send the data
            // serial
            //if (mf.spMachine.IsOpen)
            //{
            //    try { mf.spMachine.Write(ArdSend31300.Data(), 0, ArdSend31300.ByteCount()); }
            //    catch (Exception e)
            //    {
            //        mf.WriteErrorLog("Rate Data to arduino" + e.ToString());
            //        mf.SerialPortMachineClose();
            //    }
            //}

            // UDP
            if (mf.UDP.isUDPSendConnected)
            {
                mf.UDP.SendUDPMessage(ArdSend31300.Data());
            }


            //// send data (B)
            //// serial
            //if (mf.spMachine.IsOpen)
            //{
            //    try { mf.spMachine.Write(ArdSend31400.Data(), 0, ArdSend31400.ByteCount()); }
            //    catch (Exception e)
            //    {
            //        mf.WriteErrorLog("Rate Data to arduino" + e.ToString());
            //        mf.SerialPortMachineClose();
            //    }
            //}

            // UDP
            if (mf.UDP.isUDPSendConnected)
            {
                mf.UDP.SendUDPMessage(ArdSend31400.Data());
            }
        }

        public void CommFromArduino(string sentence)
        {
            int end = sentence.IndexOf("\r");
            sentence = sentence.Substring(0, end);
            string[] words = sentence.Split(',');
            if (ArdRec31100.ParseStringData(words))
            {
                UPM = ArdRec31100.RateApplied();
                UnitsPerMinute.AddDataPoint(UPM);
                AccQuantity = ArdRec31100.AccumulatedQuantity();
                UpdateQuantity();

                double NP = (1.0 - ArdRec31100.PercentError() / 100) * RateSet;
                AppRate.AddDataPoint(NP);
            }

            if (ArdRec31200.ParseStringData(words))
            {
                ArduinoReceiveTime = DateTime.Now;
            }
        }

        public void CommToAOG()
        {
            if(mf.UDP.isUDPSendConnected)
            {
                mf.UDP.SendUDPMessage(AogSend32300.Data());
            }
        }

        public void UDPcommFromArduino(byte[] data)
        {
            if (ArdRec31100.ParseByteData(data))
            {
                UPM = ArdRec31100.RateApplied();
                UnitsPerMinute.AddDataPoint(UPM);
                AccQuantity = ArdRec31100.AccumulatedQuantity();
                UpdateQuantity();

                double NP = (1.0 - ArdRec31100.PercentError() / 100) * RateSet;
                AppRate.AddDataPoint(NP);
            }

            if (ArdRec31200.ParseByteData(data))
            {
                ArduinoReceiveTime = DateTime.Now;
            }
        }

        public void UDPcommFromAOG(byte[] data)
        {
            if(AogRec32100.ParseByteData(data))
            {
                AogReceiveTime = DateTime.Now;
            }

            if(AogRec32200.ParseByteData(data))
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
            ArdSend31400.KP = Properties.Settings.Default.KP;
            ArdSend31400.KI = Properties.Settings.Default.KI;
            ArdSend31400.KD = Properties.Settings.Default.KD;
            ArdSend31400.Deadband = Properties.Settings.Default.DeadBand;
            ArdSend31400.MinPWM = Properties.Settings.Default.MinPWM;
            ArdSend31400.MaxPWM = Properties.Settings.Default.MaxPWM;
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
            Properties.Settings.Default.KP = ArdSend31400.KP;
            Properties.Settings.Default.KI = ArdSend31400.KI;
            Properties.Settings.Default.KD = ArdSend31400.KD;
            Properties.Settings.Default.DeadBand = ArdSend31400.Deadband;
            Properties.Settings.Default.MinPWM = ArdSend31400.MinPWM;
            Properties.Settings.Default.MaxPWM = ArdSend31400.MaxPWM;
        }
    }
}
