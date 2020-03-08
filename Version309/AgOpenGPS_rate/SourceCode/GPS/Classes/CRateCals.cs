using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace AgOpenGPS
{
    public class CRateCals
    {
        private readonly FormGPS mf;

        // receive data
        private PGN31100 Rec31100 = new PGN31100();
        private PGN31200 Rec31200;

        // send data
        private PGN31300 Send31300 = new PGN31300();
        private PGN31400 Send31400 = new PGN31400();

        string[] QuantityDescriptions = new string[] { "Imp. Gallons", "US Gallons", "Lbs", "Lbs NH3", "Litres", "Kgs", "Kgs NH3" };
        string[] CoverageDescriptions = new string[] { "Acre", "Hectare", "Minute", "Hour" };

        private double ApplicationRate = 0;
        private double UPM = 0; // units per minute
        private double TankRemaining = 0;
        private CAveraging UnitsPerMinute;

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

        private DateTime ReceiveTime;

        public bool ControllerConnected = false;
        private bool PauseArea = false;
        private bool PauseQuantity = false;

        string SettingsDir;

        public CRateCals(FormGPS CallingForm)
        {
            mf = CallingForm;
            Rec31200 = new PGN31200(mf);
            SettingsDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\AOG\\";
            if (Directory.Exists(SettingsDir))
            {
                LoadSettings();
                Rec31200.RateSet = RateSet;
            }
            else
            {
                Directory.CreateDirectory(SettingsDir);
            }
            UnitsPerMinute = new CAveraging(6);
        }

        public bool SimulateFlow { get { return SimFlow; } set { SimFlow = value; } }

        public byte KP { get { return Send31400.KP; } set { Send31400.KP = value; } }

        public byte KI { get { return Send31400.KI; } set { Send31400.KI = value; } }

        public byte KD { get { return Send31400.KD; } set { Send31400.KD = value; } }

        public byte DeadBand { get { return Send31400.Deadband; } set { Send31400.Deadband = value; } }

        public byte MinPWM { get { return Send31400.MinPWM; } set { Send31400.MinPWM = value; } }

        public byte MaxPWM { get { return Send31400.MaxPWM; } set { Send31400.MaxPWM = value; } }

        public void Update()
        {
            StartTime = DateTime.Now;
            CurrentMinutes = (StartTime - LastTime).TotalMinutes;
            if (CurrentMinutes < 0) CurrentMinutes = 0;
            LastTime = StartTime;

            // check connection
            if ((StartTime - ReceiveTime).TotalSeconds > 3)
            {
                // connection lost
                ControllerConnected = false;
                ApplicationRate = 0;
                PauseArea = true;
                PauseQuantity = true;
            }
            else
            {
                ControllerConnected = true;

                // worked area
                TotalWorkedArea = mf.fd.workedAreaTotal;    // square meters
                if (PauseArea)
                {
                    // exclude area worked while paused
                    LastWorkedArea = TotalWorkedArea;
                    PauseArea = false;
                }
                CurrentWorkedArea = TotalWorkedArea - LastWorkedArea;
                LastWorkedArea = TotalWorkedArea;

                // work rate
                CurrentWidth = 0;

                // is supersection on?
                if (mf.section[mf.vehicle.numOfSections].isSectionOn)
                {
                    CurrentWidth = mf.vehicle.toolWidth;
                }

                //individual sections are possibly on
                else
                {
                    for (int i = 0; i < mf.vehicle.numOfSections; i++)
                    {
                        if (mf.section[i].isSectionOn) CurrentWidth += mf.section[i].sectionWidth;
                    }
                }
                HectaresPerMinute = (CurrentWidth * mf.pn.speed * .1) / 60;

                //coverage
                ApplicationRate = 0;
                if (HectaresPerMinute > 0)    // Is application on?
                {
                    switch (CoverageUnits)
                    {
                        case 0:
                            // acres
                            Coverage += CurrentWorkedArea * 0.000247105;
                            ApplicationRate = UnitsPerMinute.Average() / (HectaresPerMinute * 2.47);
                            break;
                        case 1:
                            // hectares
                            Coverage += CurrentWorkedArea * .0001;
                            ApplicationRate = UnitsPerMinute.Average() / HectaresPerMinute;
                            break;
                        case 2:
                            // minutes
                            Coverage += CurrentMinutes;
                            ApplicationRate = UnitsPerMinute.Average();
                            break;
                        default:
                            // hours
                            Coverage += CurrentMinutes / 60;
                            ApplicationRate = UnitsPerMinute.Average() * 60.0;
                            break;
                    }
                }
            }

            // update display
            foreach (Form f in Application.OpenForms)
            {
                if (f is Forms.FormRateControl)
                {
                    mf.FRC.UpdateStatus();
                    break;
                }
            }

            // comm to arduino
            CommToArduino();

            //remote switches
            if (ControllerConnected) DoRemoteSwitches();
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
            LastWorkedArea = mf.fd.workedAreaTotal;
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
            return ApplicationRate.ToString("N1");
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

            mf.BuildRelayByte();
            Send31300.RelayLo = mf.mc.relayData[mf.mc.rdSectionControlByteLo];
            Send31300.RelayHi = mf.mc.relayData[mf.mc.rdSectionControlByteHi];
            int Temp = (int)(UPMsetting() * 100);
            Send31300.RateSetHi = (byte)(Temp >> 8);
            Send31300.RateSetLo = (byte)Temp;

            Temp = (int)(FlowCal * 100);
            Send31300.FlowCalHi = (byte)(Temp >> 8);
            Send31300.FlowCalLo = (byte)Temp;

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

            Send31300.Command = OutCommand;

            // send the data
            // serial
            if (mf.spRelay.IsOpen)
            {
                try { mf.spRelay.Write(Send31300.Data(), 0, Send31300.ByteCount()); }
                catch (Exception e)
                {
                    mf.WriteErrorLog("Rate Data to arduino" + e.ToString());
                    mf.SerialPortRelayClose();
                }
            }

            // UDP
            if (mf.isUDPSendConnected)
            {
                mf.SendUDPMessage(Send31300.Data());
            }


            // send data (B)
            // serial
            if (mf.spRelay.IsOpen)
            {
                try { mf.spRelay.Write(Send31400.Data(), 0, Send31400.ByteCount()); }
                catch (Exception e)
                {
                    mf.WriteErrorLog("Rate Data to arduino" + e.ToString());
                    mf.SerialPortRelayClose();
                }
            }

            // UDP
            if (mf.isUDPSendConnected)
            {
                mf.SendUDPMessage(Send31400.Data());
            }
        }

        public void CommFromArduino(string sentence)
        {
            int end = sentence.IndexOf("\r");
            sentence = sentence.Substring(0, end);
            string[] words = sentence.Split(',');
            if(Rec31100.ParseStringData(words))
            {
                UPM = Rec31100.RateApplied();
                UnitsPerMinute.AddDataPoint(UPM);
                AccQuantity = Rec31100.AccumulatedQuantity();
                UpdateQuantity();
            }

            if(Rec31200.ParseStringData(words))
            {
                ReceiveTime = DateTime.Now;
            }
        }

        public void UDPcommFromArduino(byte[] data)
        {
            if (Rec31100.ParseByteData(data))
            {
                UPM = Rec31100.RateApplied();
                UnitsPerMinute.AddDataPoint(UPM);
                AccQuantity = Rec31100.AccumulatedQuantity();
                UpdateQuantity();
            }

            if (Rec31200.ParseByteData(data))
            {
                ReceiveTime = DateTime.Now;
            }
        }

        public void DoRemoteSwitches()
        {
            RateSet = Rec31200.DoSwitches(RateSet);
        }

        void LoadSettings()
        {
            try
            {
                string[] lines = File.ReadAllLines(SettingsDir + "Rate.txt", Encoding.UTF8);
                if (lines.Length == 16)
                {
                    Coverage = Convert.ToDouble(lines[0]);
                    TankRemaining = Convert.ToDouble(lines[1]);
                    QuantityApplied = Convert.ToDouble(lines[2]);
                    QuantityUnits = Convert.ToByte(lines[3]);
                    CoverageUnits = Convert.ToByte(lines[4]);
                    RateSet = Convert.ToDouble(lines[5]);
                    FlowCal = Convert.ToDouble(lines[6]);
                    TankSize = Convert.ToDouble(lines[7]);
                    ValveType = Convert.ToByte(lines[8]);
                    SimFlow = Convert.ToBoolean(lines[9]);
                    Send31400.KP = Convert.ToByte(lines[10]);
                    Send31400.KI = Convert.ToByte(lines[11]);
                    Send31400.KD = Convert.ToByte(lines[12]);
                    Send31400.Deadband = Convert.ToByte(lines[13]);
                    Send31400.MinPWM = Convert.ToByte(lines[14]);
                    Send31400.MaxPWM = Convert.ToByte(lines[15]);
                }
            }
            catch (Exception)
            {

            }
        }

        public void SaveSettings()
        {
            try
            {
                using (StreamWriter SW = new StreamWriter(SettingsDir + "Rate.txt"))
                {
                    SW.WriteLine(Coverage.ToString("N2"));
                    SW.WriteLine(TankRemaining.ToString("N2"));
                    SW.WriteLine(QuantityApplied.ToString("N2"));
                    SW.WriteLine(QuantityUnits.ToString("N0"));
                    SW.WriteLine(CoverageUnits.ToString("N0"));
                    SW.WriteLine(RateSet.ToString("N2"));
                    SW.WriteLine(FlowCal.ToString("N2"));
                    SW.WriteLine(TankSize.ToString("N2"));
                    SW.WriteLine(ValveType.ToString("N0"));
                    SW.WriteLine(SimulateFlow.ToString());
                    SW.WriteLine(Send31400.KP.ToString("N0"));
                    SW.WriteLine(Send31400.KI.ToString("N0"));
                    SW.WriteLine(Send31400.KD.ToString("N0"));
                    SW.WriteLine(Send31400.Deadband.ToString("N0"));
                    SW.WriteLine(Send31400.MinPWM.ToString("N0"));
                    SW.WriteLine(Send31400.MaxPWM.ToString("N0"));
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
