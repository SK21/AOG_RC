using AgOpenGPS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RateController.Classes
{
    public class clsCalibrate
    {
        private bool AlarmStart;
        private ApplicationMode ApplicationModeStart;
        private int CalPWM;
        private double cCalFactor;
        private TextBox cCalFactorBox;
        private Label cDescriptionLabel;
        private bool cEdited;
        private Label cExpected;
        private int cID = 0;
        private bool cIsLocked;
        private Button cLocked;
        private TextBox cMeasured;
        private Button cPower;
        private bool cPowerOn;
        private clsProduct cProduct;
        private ProgressBar cProgress;
        private Label cPulses;
        private Label cPWMDisplay;
        private TextBox cRateBox;
        private bool cRunning;
        private Button cStopRun;
        private Timer cTimer = new Timer();
        private bool Initializing;
        private double MeasuredAmount;
        private bool ProductEnabledStart;
        private double PulseCountStart;
        private double PulseCountTotal;
        private int SetCount;
        private bool TestingRate = false;

        public clsCalibrate(int ID)
        {
            cID = ID;

            cProduct = Core.Products.Item(cID);
            cCalFactor = cProduct.MeterCal;
            PulseCountStart = cProduct.Pulses();

            ApplicationModeStart = cProduct.AppMode;
            ProductEnabledStart = cProduct.Enabled;
            AlarmStart = cProduct.UseOffRateAlarm;

            cTimer.Interval = 1000;
            cTimer.Enabled = false;
            cTimer.Tick += CTimer_Tick;
        }

        public event EventHandler<EventArgs> Edited;

        #region Controls

        public TextBox CalFactorBox
        {
            set
            {
                cCalFactorBox = value;

                cCalFactorBox.Enter += CCalFactor_Enter;
                cCalFactorBox.TextChanged += CCalFactor_TextChanged;
                cCalFactorBox.Validating += CCalFactor_Validating;

                Initializing = true;
                cCalFactorBox.Text = cProduct.MeterCal.ToString("N1");
                Initializing = false;
            }
        }

        public Label Description
        {
            set
            {
                cDescriptionLabel = value;
                cDescriptionLabel.Text = (cID + 1).ToString() + ". " + cProduct.ProductName
                 + "  - " + Props.ControlTypeDescription(cProduct.ControlType);
            }
        }

        public Label Expected
        { get { return cExpected; } set { cExpected = value; } }

        public Button Locked
        {
            get { return cLocked; }
            set
            {
                cLocked = value;
                cLocked.Click += CLocked_Click;
            }
        }

        public TextBox Measured
        {
            get { return cMeasured; }
            set
            {
                cMeasured = value;
                cMeasured.Enter += CMeasured_Enter;
                cMeasured.TextChanged += CMeasured_TextChanged;
                cMeasured.Validating += CMeasured_Validating;
            }
        }

        public Button Power
        {
            get { return cPower; }
            set
            {
                cPower = value;
                cPower.Click += CPower_Click;
            }
        }

        public ProgressBar Progress
        {
            get { return cProgress; }
            set
            {
                cProgress = value;
                cProgress.Minimum = 0;
                cProgress.Maximum = 100;
                cProgress.Style = ProgressBarStyle.Marquee;
                cProgress.MarqueeAnimationSpeed = 50;
            }
        }

        public Label Pulses
        { get { return cPulses; } set { cPulses = value; } }

        public Label PWMDisplay
        {
            get { return cPWMDisplay; }
            set
            {
                cPWMDisplay = value;
                switch (cProduct.ControlType)
                {
                    case ControlTypeEnum.Valve:
                    case ControlTypeEnum.ComboCloseTimed:
                    case ControlTypeEnum.ComboClose:
                        cPWMDisplay.Visible = false;
                        break;

                    case ControlTypeEnum.Motor:
                    case ControlTypeEnum.MotorWeights:
                    case ControlTypeEnum.Fan:
                        cPWMDisplay.Visible = true;
                        break;
                }
            }
        }

        public TextBox RateBox
        {
            get { return cRateBox; }
            set
            {
                cRateBox = value;
                cRateBox.Enter += CRateBox_Enter;
                cRateBox.TextChanged += CRateBox_TextChanged;
                cRateBox.Validating += CRateBox_Validating;
            }
        }

        public Button StopRun
        {
            set
            {
                cStopRun = value;
            }
        }

        #endregion Controls

        #region Controls events

        private void CCalFactor_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(cCalFactorBox.Text, out tempD);
            using (var form = new FormNumeric(0, 2000, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    cCalFactorBox.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void CCalFactor_TextChanged(object sender, EventArgs e)
        {
            if (!Initializing)
            {
                cEdited = true;
                Edited(this, EventArgs.Empty);
            }
        }

        private void CCalFactor_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            double tempD;
            double.TryParse(cCalFactorBox.Text, out tempD);
            if (tempD < 0 || tempD > 2000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void CLocked_Click(object sender, EventArgs e)
        {
            cIsLocked = !cIsLocked;
            if (cIsLocked) cCalFactor = cProduct.MeterCal;   // use saved cal factor
            Update();
        }

        private void CMeasured_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(cMeasured.Text, out tempD);
            using (var form = new FormNumeric(0, 2000, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    cMeasured.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void CMeasured_TextChanged(object sender, EventArgs e)
        {
            if (!Initializing)
            {
                cEdited = true;
                Edited(this, EventArgs.Empty);
                double.TryParse(cMeasured.Text, out MeasuredAmount);
                cCalFactorBox.Text = NewCalFactor().ToString("N1");
            }
        }

        private void CMeasured_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            double tempD;
            double.TryParse(cMeasured.Text, out tempD);
            if (tempD < 0 || tempD > 2000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void CPower_Click(object sender, EventArgs e)
        {
            cPowerOn = !cPowerOn;

            if (cPowerOn)
            {
                cCalFactor = cProduct.MeterCal;
                Initializing = true;
                cCalFactorBox.Text = cCalFactor.ToString("N1");
                Initializing = false;

                // save non-calibration state
                ApplicationModeStart = cProduct.AppMode;
                ProductEnabledStart = cProduct.Enabled;
                AlarmStart = cProduct.UseOffRateAlarm;

                cProduct.AppMode = ApplicationMode.ConstantUPM;
                cProduct.Enabled = false;
                cProduct.UseOffRateAlarm = false;
            }
            else
            {
                RestoreState();
            }

            Update();
        }

        private void CRateBox_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(cRateBox.Text, out tempD);
            using (var form = new FormNumeric(0, 50000, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    cRateBox.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void CRateBox_TextChanged(object sender, EventArgs e)
        {
            if (!Initializing)
            {
                cEdited = true;
                Edited(this, EventArgs.Empty);
            }
        }

        private void CRateBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            double tempD;
            double.TryParse(cRateBox.Text, out tempD);
            if (tempD < 0 || tempD > 50000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void CTimer_Tick(object sender, EventArgs e)
        {
            Update();
        }

        #endregion Controls events

        public int ID
        { get { return cID; } }

        public bool PowerOn
        { get { return cPowerOn; } }

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
                            cProgress.Value = 0;
                            PulseCountStart = cProduct.Pulses();
                            MeasuredAmount = 0;
                            cProduct.Enabled = true;
                            TestingRate = cIsLocked;
                        }
                    }
                    else
                    {
                        // calibration stopped
                        cRunning = false;
                        cProduct.Enabled = false;
                        Reset();
                    }
                    Update();
                }
            }
        }

        public void Close()
        {
            RestoreState();
        }

        public void Load()
        {
            if (int.TryParse(Props.GetProp(Name() + "_CalPWM"), out int pwm)) CalPWM = pwm;
        }

        public void Reset()
        {
            bool Last = Initializing;
            Initializing = true;
            cPulses.Text = PulseCountTotal.ToString("N0");
            cCalFactorBox.Text = cCalFactor.ToString("N1");
            MeasuredAmount = 0;
            cMeasured.Text = MeasuredAmount.ToString("N1");
            cRateBox.Text = cProduct.RateSet.ToString("N1");
            Initializing = Last;
        }

        public void Save()
        {
            if (cEdited && cPowerOn)
            {
                if (Props.ReadOnly)
                {
                    Props.ShowMessage("File is read only.", "Help", 5000, false, false, true);
                }
                else
                {
                    Props.SetProp(Name() + "_CalPWM", CalPWM.ToString());

                    if (double.TryParse(cCalFactorBox.Text, out double NewFactor))
                    {
                        cCalFactor = NewFactor;
                        cProduct.MeterCal = NewFactor;
                        cProduct.Save();
                    }

                    cEdited = false;
                }
            }
        }

        public void Update()
        {
            Initializing = true;

            if (cPowerOn)
            {
                cPower.Image = Properties.Resources.FanOn;
                cTimer.Enabled = cRunning;

                // pulses
                PulseCountTotal = cProduct.Pulses() - PulseCountStart;
                cPulses.Text = PulseCountTotal.ToString("N0");

                // base rate
                cRateBox.Enabled = !cRunning && !cIsLocked;
                cRateBox.Text = cProduct.RateSet.ToString("N1");

                // Meter Cal
                cCalFactorBox.Enabled = !cRunning && !cIsLocked;

                // Progress bar
                cProgress.Visible = (!cIsLocked && Running);

                if (cIsLocked)
                {
                    // Testing Rate, run in manual at CalPWM

                    // Expected Amount
                    if (TestingRate && cCalFactor > 0)
                    {
                        cExpected.Text = (PulseCountTotal / cCalFactor).ToString("N1");
                    }

                    // Measured Amount
                    cMeasured.Enabled = !cRunning && TestingRate;
                    cMeasured.Text = MeasuredAmount.ToString("N1");

                    // Lock
                    cLocked.Enabled = !cRunning;
                    cLocked.Image = Properties.Resources.ColorLocked;
                    cLocked.Visible = true;

                    // PWM display
                    cPWMDisplay.Text = CalPWM.ToString("N0");
                }
                else
                {
                    // Setting PWM, auto on, find CalPWM

                    // Expected Amount
                    cExpected.Text = "0";

                    // Measured Amount
                    cMeasured.Enabled = false;
                    cMeasured.Text = "0";

                    // Lock
                    cLocked.Enabled = !cRunning;
                    cLocked.Image = Properties.Resources.ColorUnlocked;
                    cLocked.Visible = !cRunning;

                    // PWM display
                    cPWMDisplay.Text = cProduct.PWM().ToString("N0");
                }

                if (cRunning)
                {
                    if (!cIsLocked && MeterIsSet())
                    {
                        cIsLocked = true;
                        switch (cProduct.ControlType)
                        {
                            case ControlTypeEnum.Valve:
                            case ControlTypeEnum.ComboCloseTimed:
                            case ControlTypeEnum.ComboClose:
                                cProduct.ManualPWM = 0;
                                break;

                            case ControlTypeEnum.Motor:
                            case ControlTypeEnum.MotorWeights:
                            case ControlTypeEnum.Fan:
                                CalPWM = (int)cProduct.PWM();
                                break;
                        }
                        cStopRun.PerformClick();
                    }

                    cProduct.CalIsLocked = cIsLocked;
                    cProduct.ManualPWM = CalPWM;
                }
            }
            else
            {
                cPower.Image = Properties.Resources.FanOff;
                cTimer.Enabled = false;

                // pulses
                cPulses.Text = "0";

                // Base Rate
                cRateBox.Enabled = false;
                cRateBox.Text = "0";

                // Meter Cal
                cCalFactorBox.Enabled = false;
                cCalFactorBox.Text = "0";

                // Expected amount
                cExpected.Text = "0";

                // Measured Amount
                cMeasured.Enabled = false;
                cMeasured.Text = "0";

                // Progress bar
                cProgress.Visible = false;

                // Lock
                cLocked.Enabled = false;
                cLocked.Image = Properties.Resources.ColorUnlocked;
                cLocked.Visible = true;

                // PWM display
                cPWMDisplay.Text = "0";
            }
            Initializing = false;
        }

        private bool MeterIsSet()
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

        private string Name()
        {
            return "Calibrate" + cID.ToString();
        }

        private double NewCalFactor()
        {
            double Result = 0;
            if (MeasuredAmount > 0)
            {
                // Meter Cal = PPM/UPM
                // = pulses/amount in same time frame
                Result = PulseCountTotal / MeasuredAmount;
            }
            return Result;
        }

        private void RestoreState()
        {
            // restore initial settings
            cProduct.AppMode = ApplicationModeStart;
            cProduct.Enabled = ProductEnabledStart;
            cProduct.UseOffRateAlarm = AlarmStart;

            cProduct.Save();
            Running = false;
        }
    }
}
