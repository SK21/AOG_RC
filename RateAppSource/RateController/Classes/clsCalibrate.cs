using AgOpenGPS;
using RateController.Classes;
using System;
using System.Windows.Forms;

namespace RateController
{
    public class clsCalibrate
    {
        private ApplicationMode ApplicationModeStart;
        private int CalPWM;
        private double cCalFactor;
        private TextBox cCalFactorBox;
        private Label cDescriptionLabel;
        private bool cEdited;
        private bool cEnabled;
        private Label cExpected;
        private int cID = 0;
        private bool cIsLocked;
        private Button cLocked;
        private TextBox cMeasured;
        private Button cPower;
        private clsProduct cProduct;
        private ProgressBar cProgress;
        private Label cPulses;
        private TextBox cRateBox;
        private bool cRunning;
        private Timer cTimer = new Timer();
        private bool FirstRun;
        private bool Initializing;
        private double MeasuredAmount;
        private FormStart mf;
        private double PulseCountStart;
        private double PulseCountTotal;
        private int SetCount;

        public clsCalibrate(FormStart CallingFrom, int ID)
        {
            mf = CallingFrom;
            cID = ID;
            FirstRun = true;
            cProduct = mf.Products.Item(cID);
            cCalFactor = cProduct.MeterCal;
            //ApplicationModeStart = cProduct.AppMode;
            //cProduct.Enabled = false;
            //PulseCountStart = cProduct.Pulses();
            //cProduct.AppMode = ApplicationMode.ConstantUPM;

            cTimer.Interval = 1000;
            cTimer.Enabled = false;
            cTimer.Tick += CTimer_Tick;
        }

        public event EventHandler<EventArgs> Edited;

        public TextBox CalFactorBox
        {
            get { return cCalFactorBox; }
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
        { get { return cDescriptionLabel; } set { cDescriptionLabel = value; } }

        public bool Enabled
        { get { return cEnabled; } }

        public Label Expected
        { get { return cExpected; } set { cExpected = value; } }

        public int ID
        { get { return cID; } }

        public bool IsLocked
        { get { return cIsLocked; } }

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

        public bool Running
        {
            get { return cRunning; }
            set
            {
                if (value)
                {
                    if (cEnabled && (cRunning != value))
                    {
                        // calibration started
                        cRunning = true;
                        cProgress.Value = 0;
                        PulseCountStart = cProduct.Pulses();
                        MeasuredAmount = 0;
                        cProduct.Enabled = true;
                    }
                }
                else
                {
                    // calibration stopped
                    cRunning = false;
                    cProduct.Enabled = false;
                }
                cPower.Enabled = !value;
                Update();
            }
        }

        public void Close()
        {
            // restore initial settings
            cProduct.Enabled = true;
            cProduct.AppMode = ApplicationModeStart;
            cProduct.Save();
            cProduct.CalUseBaseRate = false;
        }

        public void Load()
        {
            if (bool.TryParse(mf.Tls.LoadProperty(Name() + "_IsLocked"), out bool lk)) cIsLocked = lk;
            if (double.TryParse(mf.Tls.LoadProperty(Name() + "_Pulses"), out double pl)) PulseCountTotal = pl;
            if (double.TryParse(mf.Tls.LoadProperty(Name() + "_Amount"), out double amt)) MeasuredAmount = amt;
            if (int.TryParse(mf.Tls.LoadProperty(Name() + "_CalPWM"), out int pwm)) CalPWM = pwm;
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
            if (cEdited && cEnabled)
            {
                if (mf.Tls.ReadOnly)
                {
                    mf.Tls.ShowMessage("File is read only.", "Help", 5000, false, false, true);
                }
                else
                {
                    Props.SetProp(Name() + "_Pulses", PulseCountTotal.ToString());
                    Props.SetProp(Name() + "_Amount", MeasuredAmount.ToString());
                    Props.SetProp(Name() + "_CalPWM", CalPWM.ToString());
                    Props.SetProp(Name() + "_IsLocked", cIsLocked.ToString());

                    double.TryParse(cCalFactorBox.Text, out cCalFactor);
                    cProduct.MeterCal = cCalFactor;
                    double.TryParse(cRateBox.Text, out double tmp);
                    cProduct.RateSet = tmp;
                    cProduct.Save();

                    cEdited = false;
                    PulseCountStart = cProduct.Pulses();
                    Update();
                }
            }
        }

        public void Update()
        {
            Initializing = true;
            if (cEnabled)
            {
                cPower.Image = Properties.Resources.FanOn;
            }
            else
            {
                cPower.Image = Properties.Resources.FanOff;
                cRunning = false;
            }

            cTimer.Enabled = cRunning;
            cCalFactorBox.Enabled = !cRunning && cEnabled;
            cMeasured.Enabled = !cRunning && cEnabled && cIsLocked;
            cRateBox.Enabled = !cRunning && cEnabled;
            cLocked.Enabled = !cRunning && cEnabled;

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
                            //cProduct.ManualPWM = (int)cProduct.PWM();
                            CalPWM = (int)cProduct.PWM();
                            break;
                    }
                }

                cProduct.CalRun = cIsLocked;
                cProduct.CalSetMeter = !cIsLocked;
                cProduct.ManualPWM = CalPWM;
            }
            else
            {
                Reset();
                cProduct.CalRun = false;
                cProduct.CalSetMeter = false;
            }

            if (cIsLocked)
            {
                cLocked.Image = Properties.Resources.ColorLocked;
                cLocked.Visible = true;
                cProgress.Visible = false;
            }
            else
            {
                cLocked.Image = Properties.Resources.ColorUnlocked;
                cLocked.Visible = !cRunning;
                cProgress.Visible = cRunning;
            }

            PulseCountTotal = cProduct.Pulses() - PulseCountStart;
            cPulses.Text = PulseCountTotal.ToString("N0");
            cRateBox.Text = cProduct.RateSet.ToString("N1");
            cMeasured.Text = MeasuredAmount.ToString("N1");
            if (cCalFactor > 0)
            {
                cExpected.Text = (PulseCountTotal / cCalFactor).ToString("N1");
            }
            else
            {
                cExpected.Text = "0.0";
            }

            cDescriptionLabel.Text = (cID + 1).ToString() + ". " + cProduct.ProductName
                + "  - " + mf.Tls.ControlTypeDescription(cProduct.ControlType);

            Initializing = false;
        }

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
            if (FirstRun)
            {
                // Prevent turning off product when frmMenuCalibrate loads.
                // Only turn off when calibration is going to be used.
                FirstRun = false;
                cCalFactor = cProduct.MeterCal;
                ApplicationModeStart = cProduct.AppMode;
                cProduct.Enabled = false;
                PulseCountStart = cProduct.Pulses();
                cProduct.AppMode = ApplicationMode.ConstantUPM;
            }
            cEnabled = !cEnabled;
            cProduct.CalUseBaseRate = cEnabled;
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
    }
}