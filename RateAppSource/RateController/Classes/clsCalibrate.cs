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
        private int PulseCountStart;
        private int SetCount;

        public clsCalibrate(int ID)
        {
            cID = ID;

            cProduct = Core.Products.Item(cID);
            cProduct.CalibrateOjbect = this;
            cMeterCal = cProduct.MeterCal;
            PulseCountStart = (int)cProduct.Pulses();

            ApplicationModeStart = cProduct.AppMode;
            ProductEnabledStart = cProduct.Enabled;
            AlarmStart = cProduct.UseOffRateAlarm;

            cTimer.Interval = 1000;
            cTimer.Enabled = false;
            cTimer.Tick += CTimer_Tick;

            cCalPWM = int.TryParse(Props.GetProp(cProduct.ProductName + "_CalPWM"), out int pwm) ? pwm : 0;
        }
        public clsProduct Product { get { return cProduct; } }

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
        { get { return cCalPWM; } }

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
                cPowerOn = value;
                SetRunState(cPowerOn);
            }
        }

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
                            PulseCountStart = (int)cProduct.Pulses();
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
            RestoreState();
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

        public void Save()
        {
            if (!Props.ReadOnly)
            {
                Props.SetProp(cProduct.ProductName + "_CalPWM", cCalPWM.ToString());

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

        private void Update()
        {
            if (cPowerOn)
            {
                cTimer.Enabled = cRunning;
                cPulseCount = (int)(cProduct.Pulses() - PulseCountStart);

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