using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace RateController.Classes
{
    public class clsCalibrate
    {
        private bool AlarmStart;
        private ApplicationMode ApplicationModeStart;
        private double cBaseRate;
        private int cCalPWM;
        private double cExpectedAmount;
        private int cID = 0;
        private bool cLocked;
        private double cMeasuredAmount;
        private double cMeterCal;
        private bool cPowerOn;
        private clsProduct cProduct;
        private int cPulseCount;
        private bool cRunning;
        private bool cTestingRate;
        private Timer cTimer = new Timer();
        private double NewMeterCal;
        private bool ProductEnabledStart;
        private double PulseCountStart;
        private int SetCount;

        public clsCalibrate(int ID)
        {
            cID = ID;

            cProduct = Core.Products.Item(cID);
            cProduct.CalibrateOjbect = this;
            cMeterCal = cProduct.MeterCal;
            PulseCountStart = cProduct.Pulses();

            ApplicationModeStart = cProduct.AppMode;
            ProductEnabledStart = cProduct.Enabled;
            AlarmStart = cProduct.UseOffRateAlarm;

            cTimer.Interval = 1000;
            cTimer.Enabled = false;
            cTimer.Tick += CTimer_Tick;

            cCalPWM = int.TryParse(Props.GetProp(cProduct.ProductName + "_CalPWM"), out int pwm) ? pwm : 0;
            cLocked = bool.TryParse(Props.GetProp(cProduct.ProductName + "_CalLocked"), out bool lk) && lk;   // meter set, kept between sessions
            cBaseRate = cProduct.RateSet;
        }
        public clsProduct Product { get { return cProduct; } }

        public static string RestoreKeyFor(int ProductID)
        {
            // profile prop holding a calibrating product's original AppMode|Enabled|OffRateAlarm
            return "CalRestore" + ProductID.ToString();
        }

        private string RestoreKey
        { get { return RestoreKeyFor(cID); } }

        public event EventHandler CalFinished;

        public double BaseRate
        {
            get { return cBaseRate; }
            set
            {
                if (value >= 0 & value <= 50000)
                {
                    cBaseRate = value;
                }
            }
        }

        public int CalPWM
        {
            get { return cCalPWM; }
            set
            {
                if (value >= 0 & value <= 255)
                {
                    cCalPWM = value;
                }
            }
        }

        public double ExpectedAmount
        { get { return cExpectedAmount; } }

        public int ID
        { get { return cID; } }

        public bool Locked
        {
            get { return cLocked; }
            set
            {
                cLocked = value;
                if (cLocked) cMeterCal = cProduct.MeterCal;   // use saved cal factor
                Update();
            }
        }

        public double MeasuredAmount
        {
            get { return cMeasuredAmount; }
            set
            {
                if (value > 0 && value <= 2000)
                {
                    cMeasuredAmount = value;
                    SetNewMeterCal();
                }
            }
        }

        public double MeterCal
        {
            get { return cMeterCal; }
            set
            {
                if (value > 0 & value <= 2000)
                {
                    cMeterCal = value;
                    NewMeterCal = value;
                }
            }
        }

        public bool PowerOn
        {
            get { return cPowerOn; }
            set
            {
                // only an enabled product can be powered on, a disabled one's module/sensor IDs aren't checked
                if (!value || cProduct.Enabled)
                {
                    cPowerOn = value;
                    SetRunState(cPowerOn);
                }
            }
        }

        public bool OriginalEnabled
        { get { return ProductEnabledStart; } }

        public bool OriginalOffRateAlarm
        { get { return AlarmStart; } }

        public int PulseCount
        { get { return cPulseCount; } }

        public bool Running
        {
            get { return cRunning; }
            set
            {
                if (cPowerOn)
                {
                    if (value)
                    {
                        if (cRunning != value)
                        {
                            // calibration started
                            cRunning = true;
                            PulseCountStart = cProduct.Pulses();
                            cProduct.Enabled = true;
                            cTestingRate = cLocked;
                        }
                    }
                    else
                    {
                        // calibration stopped
                        cRunning = false;
                        cProduct.Enabled = false;
                    }
                    cMeasuredAmount = 0;
                    Update();
                }
            }
        }

        public bool TestingRate
        { get { return cTestingRate; } }

        public void Close()
        {
            // only a product powered on for calibration had its state changed; restoring the others
            // would overwrite changes made since the session opened (e.g. product enabled elsewhere)
            if (cPowerOn) RestoreState();
            cPowerOn = false;       // product keeps this object after the session closes
            cTimer.Enabled = false;
            cTimer = null;
        }

        public bool MeterIsSet()
        {
            // true if within 10%
            bool Result = false;
            if (cProduct.RateSet > 0)
            {
                double Ratio = Math.Abs(cProduct.RateSet - cProduct.RateApplied()) / cProduct.RateSet;
                if (Ratio < 0.1)
                {
                    SetCount++;
                    if (SetCount > 3) Result = true;
                }
                else
                {
                    SetCount = 0;
                }
            }
            return Result;
        }

        public void SetOriginal(bool Enabled, bool OffRateAlarm)
        {
            // settings page edits while powered on for calibration, applied when the product is restored
            ProductEnabledStart = Enabled;
            AlarmStart = OffRateAlarm;
            WriteRestoreRecord();
        }

        public void Save()
        {
            if (!Props.ReadOnly)
            {
                Props.SetProp(cProduct.ProductName + "_CalPWM", cCalPWM.ToString());
                Props.SetProp(cProduct.ProductName + "_CalLocked", cLocked.ToString());     // meter set, kept between sessions

                if (cPowerOn) cProduct.RateSet = cBaseRate;     // base rate box, only when calibrating this product
                cMeterCal = NewMeterCal;
                cProduct.MeterCal = cMeterCal;
                cProduct.Save();
            }
        }

        private void CTimer_Tick(object sender, EventArgs e)
        {
            Update();
        }

        private void RestoreState()
        {
            if (!Props.ReadOnly)
            {
                cProduct.AppMode = ApplicationModeStart;
                cProduct.Enabled = ProductEnabledStart;
                cProduct.UseOffRateAlarm = AlarmStart;
                cProduct.ManualPWM = 0;
                cProduct.Save();
                Props.SetProp(RestoreKey, "");      // restored cleanly
            }

            cRunning = false;
        }

        private void SetNewMeterCal()
        {
            if (cMeasuredAmount > 0)
            {
                // Meter Cal = PPM/UPM
                // = pulses/amount in same time frame
                NewMeterCal = cPulseCount / cMeasuredAmount;
            }
        }

        private void SetRunState(bool state)
        {
            if (state)
            {
                // save non-calibration state
                ApplicationModeStart = cProduct.AppMode;
                ProductEnabledStart = cProduct.Enabled;
                AlarmStart = cProduct.UseOffRateAlarm;

                // recovery record, written to disk before any calibration value can be saved.
                // still present at the next load means calibration was interrupted (power loss, crash)
                WriteRestoreRecord();

                cProduct.AppMode = ApplicationMode.ConstantUPM;
                cProduct.Enabled = false;
                cProduct.UseOffRateAlarm = false;

                cMeterCal = cProduct.MeterCal;
            }
            else
            {
                RestoreState();
            }
        }

        private void WriteRestoreRecord()
        {
            Props.SetProp(RestoreKey, ApplicationModeStart.ToString() + "|" + ProductEnabledStart.ToString() + "|" + AlarmStart.ToString());
        }

        private void Update()
        {
            if (cPowerOn)
            {
                cTimer.Enabled = cRunning;
                cPulseCount = (int)Math.Round(cProduct.Pulses() - PulseCountStart);    // difference in double, the pulse total can exceed int

                if (cLocked)
                {
                    // Testing Rate, run in manual at CalPWM

                    // Expected Amount
                    if (TestingRate && cMeterCal > 0)
                    {
                        cExpectedAmount = (cPulseCount / cMeterCal);
                    }
                    else
                    {
                        cExpectedAmount = 0;
                    }
                }
                else
                {
                    // Setting PWM, auto on, find CalPWM
                }

                if (cRunning)
                {
                    if (!cLocked && MeterIsSet())
                    {
                        cLocked = true;
                        switch (cProduct.ControlType)
                        {
                            case ControlTypeEnum.Valve:
                            case ControlTypeEnum.ComboCloseTimed:
                            case ControlTypeEnum.ComboClose:
                                cCalPWM = 0;
                                break;

                            case ControlTypeEnum.Motor:
                            case ControlTypeEnum.MotorWeights:
                            case ControlTypeEnum.Fan:
                                cCalPWM = (int)cProduct.PWM();
                                break;
                        }

                        CalFinished?.Invoke(this, EventArgs.Empty);
                    }

                    cProduct.ManualPWM = cCalPWM;
                }
            }
            else
            {
                cTimer.Enabled = false;
            }
        }
    }
}