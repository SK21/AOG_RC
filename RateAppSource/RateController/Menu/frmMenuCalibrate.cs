using AgOpenGPS;
using RateController.Classes;
using RateController.Language;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuCalibrate : Form
    {
        private clsCalibrate Cal;
        private clsCalibrates Cals;
        private bool cEdited;
        private int CurrentProduct;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private clsProduct Prd;
        private SpeedType StartSim;

        public frmMenuCalibrate(frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
        }

        private void btnCalStart_Click(object sender, EventArgs e)
        {
            // btnCalStop needs to be next in tab order
            // after btnCalStart to receive the focus
            if (Cals.ReadyToCalibrate())
            {
                Props.SpeedMode = SpeedType.Simulated;
                pbRunning.Value = 0;
                Cals.Running(true);
                SetButtons();
                Props.RateCalibrationOn = true;
                timer1.Enabled = true;
                if (Cal.Locked)
                {
                    Props.ShowMessage("Setting meter cal.", "Calibrate", 5000);
                }
                else
                {
                    Props.ShowMessage("Setting calibration speed.", "Calibrate", 5000);
                }
            }
        }

        private void btnCalStop_Click(object sender, EventArgs e)
        {
            StopCal();
            timer1.Enabled = false;
            UpdateForm();
            if (Cal.Locked)
            {
                Props.ShowMessage("Enter measured amount.", "Calibrate", 5000);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Cal.MeasuredAmount = 0;
            SetButtons(false);
            UpdateForm();
        }

        private void btnLocked_Click(object sender, EventArgs e)
        {
            Cal.Locked = !Cal.Locked;
            UpdateForm();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (double.TryParse(tbSpeed.Text, out double Tmp))
                {
                    if (Props.UseMetric)
                    {
                        Props.SimSpeed_KMH = Tmp;
                    }
                    else
                    {
                        Props.SimSpeed_KMH = Tmp * Props.MPHtoKPH;
                    }
                    clsCalibrate Cal = Cals.Item(CurrentProduct);

                    Cal.BaseRate = double.TryParse(tbBaseRate.Text, out double br) ? br : 0;
                    Cal.MeterCal = double.TryParse(tbMeterCal.Text, out double mc) ? mc : 0;
                    Cal.MeasuredAmount = double.TryParse(tbMeasured.Text, out double me) ? me : 0;
                    Cals.Save();

                    SetButtons(false);
                    UpdateForm();
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuCalibrate/btnOK_Click: " + ex.Message);
            }
        }

        private void btnPower_Click(object sender, EventArgs e)
        {
            Cal.PowerOn = !Cal.PowerOn;
            UpdateForm();
            UpdateProductButtons();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ChangeProduct(0);
            UpdateForm();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ChangeProduct(1);
            UpdateForm();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ChangeProduct(2);
            UpdateForm();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ChangeProduct(3);
            UpdateForm();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ChangeProduct(4);
            UpdateForm();
        }

        private void Cal_CalFinished(object sender, EventArgs e)
        {
            int count = 0;
            foreach (clsCalibrate cl in Cals.Items)
            {
                if (cl.Running) count++;
            }
            if (count == 1) StopCal();
            Props.ShowMessage("Step 1 complete: Meter speed / flow rate has been set for Product "
                 + (CurrentProduct + 1).ToString() 
                 + ".\n\nProceed to Step 2: Run the calibration again and measure the product output to calculate the final meter calibration."
             );
        }

        private void ChangeProduct(int NewProduct)
        {
            CurrentProduct = NewProduct;
            Prd = Core.Products.Item(NewProduct);
            Cal = Cals.Item(NewProduct);

            lbName.Text = (CurrentProduct + 1).ToString() + ". " + Prd.ProductName
                + " - " + Props.ControlTypeDescription(Prd.ControlType);

            switch (Prd.ControlType)
            {
                case ControlTypeEnum.Motor:
                case ControlTypeEnum.MotorWeights:
                case ControlTypeEnum.Fan:
                    lbPWM.Visible = true;
                    lbPWMData.Visible = true;
                    break;

                default:
                    lbPWM.Visible = false;
                    lbPWMData.Visible = false;
                    break;
            }
        }

        private void frmMenuCalibrate_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Enabled = false;
            Props.SaveFormLocation(this);
            Props.SpeedMode = StartSim;
            Cal.CalFinished -= Cal_CalFinished;
            Cals.Close();
        }

        private void frmMenuCalibrate_Load(object sender, EventArgs e)
        {
            this.Tag = false;
            SubMenuLayout.SetFormLayout(this, MainMenu, btnOK);

            btnCancel.Left = btnOK.Left - SubMenuLayout.ButtonSpacing;
            btnCancel.Top = btnOK.Top;
            MainMenu.StyleControls(this);
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            SetLanguage();

            var Fnt = new Font("Tahoma", 12);
            var FC = SystemColors.Highlight;

            lbPWM.Font = Fnt;
            lbPWM.ForeColor = FC;
            lbPulses.Font = Fnt;
            lbPulses.ForeColor = FC;
            lbBaseRate.Font = Fnt;
            lbBaseRate.ForeColor = FC;
            lbCalFactor.Font = Fnt;
            lbCalFactor.ForeColor = FC;
            lbExpected.Font = Fnt;
            lbExpected.ForeColor = FC;
            lbMeasured.Font = Fnt;
            lbMeasured.ForeColor = FC;
            lbMeterSet.Font = Fnt;
            lbMeterSet.ForeColor = FC;
            lbSpeed.Font = Fnt;
            lbSpeed.ForeColor = Color.DarkGreen;
            lbName.Font = new Font("Tahoma", 14, FontStyle.Underline);

            PositionForm();

            if (!Props.UseMetric)
            {
                lbSpeed.Text = "MPH";
            }
            else
            {
                lbSpeed.Text = "KMH";
            }

            StartSim = Props.SpeedMode;

            Cals = new clsCalibrates();
            Cals.Load();

            ChangeProduct(Props.CurrentProduct);
            Cal.CalFinished += Cal_CalFinished;

            SetButtons();
            UpdateForm();
        }

        private void MainMenu_MenuMoved(object sender, EventArgs e)
        {
            PositionForm();
        }

        private void PositionForm()
        {
            this.Top = MainMenu.Top + SubMenuLayout.TopOffset;
            this.Left = MainMenu.Left + SubMenuLayout.LeftOffset;
        }

        private void SetButtons(bool Edited = false)
        {
            if (!Initializing)
            {
                if (Edited)
                {
                    btnCancel.Enabled = true;
                    btnOK.Enabled = true;
                    btnCalStop.Enabled = false;
                    btnCalStart.Enabled = false;
                    button1.Enabled = false;
                    button2.Enabled = false;
                    button3.Enabled = false;
                    button4.Enabled = false;
                    button5.Enabled = false;
                }
                else
                {
                    btnCancel.Enabled = false;
                    btnOK.Enabled = false;
                    btnCalStop.Enabled = Cal.Running;
                    btnCalStart.Enabled = !Cal.Running;
                    button1.Enabled = true;
                    button2.Enabled = true;
                    button3.Enabled = true;
                    button4.Enabled = true;
                    button5.Enabled = true;
                }

                tbSpeed.Enabled = !Cal.Running;
                btnPower.Enabled = !Cal.Running;
                cEdited = Edited;
                this.Tag = cEdited;
            }
        }

        private void SetLanguage()
        {
            this.Text = Lang.lgCalibrate;
            lbPulses.Text = Lang.lgPulses;
            lbBaseRate.Text = Lang.lgBaseRate;
            lbCalFactor.Text = Lang.lgCalFactor;
            lbExpected.Text = Lang.lgExpectedAmount;
            lbMeasured.Text = Lang.lgMeasuredAmount;
            lbMeterSet.Text = Lang.lgMeterSet;
            lbCalSpeed.Text = Lang.lgCalSpeed;
            lbPWM.Text = Lang.lgPWM;
        }

        private void StopCal()
        {
            Props.SpeedMode = StartSim;
            Cals.Running(false);
            SetButtons();
            Props.RateCalibrationOn = false;
        }

        private void tbBaseRate_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbBaseRate.Text, out tempD);
            using (var form = new FormNumeric(0, 50000, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbBaseRate.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void tbBaseRate_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbBaseRate_Validating(object sender, CancelEventArgs e)
        {
            double.TryParse(tbBaseRate.Text, out double tmp);
            if (tmp < 0 || tmp > 50000) e.Cancel = true;
        }

        private void tbMeasured_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbMeasured.Text, out tempD);
            using (var form = new FormNumeric(0, 2000, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbMeasured.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void tbMeasured_Validating(object sender, CancelEventArgs e)
        {
            double.TryParse(tbMeasured.Text, out double tmp);
            if (tmp < 0 || tmp > 2000) e.Cancel = true;
        }

        private void tbMeterCal_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbMeterCal.Text, out tempD);
            using (var form = new FormNumeric(0, 2000, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbMeterCal.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void tbMeterCal_Validating(object sender, CancelEventArgs e)
        {
            double.TryParse(tbMeterCal.Text, out double tmp);
            if (tmp < 0 || tmp > 2000) e.Cancel = true;
        }

        private void tbSpeed_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbSpeed.Text, out tempD);
            using (var form = new FormNumeric(0, 40, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbSpeed.Text = form.ReturnValue.ToString("N1");
                }
            }
        }

        private void tbSpeed_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbSpeed_Validating(object sender, CancelEventArgs e)
        {
            double.TryParse(tbSpeed.Text, out double tmp);
            if (tmp < 0 || tmp > 40) e.Cancel = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void UpdateForm()
        {
            Initializing = true;

            if (Cal.PowerOn)
            {
                UpdateWithPowerOn();
            }
            else
            {
                UpdateWithPowerOff();
            }

            double displaySpeed = Props.UseMetric ? Props.SimSpeed_KMH : Props.SimSpeed_KMH / Props.MPHtoKPH;
            tbSpeed.Text = displaySpeed.ToString("N1");

            Initializing = false;
        }

        private void UpdateProductButtons()
        {
            switch (CurrentProduct)
            {
                case 0:
                    if (Cal.PowerOn)
                    {
                        button1.Image = Properties.Resources.OneGreen;
                    }
                    else
                    {
                        button1.Image = Properties.Resources.number_circle_one;
                    }
                    break;

                case 1:
                    if (Cal.PowerOn)
                    {
                        button2.Image = Properties.Resources.TwoGreen;
                    }
                    else
                    {
                        button2.Image = Properties.Resources.number_circle_two;
                    }
                    break;

                case 2:
                    if (Cal.PowerOn)
                    {
                        button3.Image = Properties.Resources.ThreeGreen;
                    }
                    else
                    {
                        button3.Image = Properties.Resources.number_circle_three;
                    }
                    break;

                case 3:
                    if (Cal.PowerOn)
                    {
                        button4.Image = Properties.Resources.FourGreen;
                    }
                    else
                    {
                        button4.Image = Properties.Resources.number_circle_four;
                    }
                    break;

                case 4:
                    if (Cal.PowerOn)
                    {
                        button5.Image = Properties.Resources.FiveGreen;
                    }
                    else
                    {
                        button5.Image = Properties.Resources.number_circle_five;
                    }
                    break;
            }
        }

        private void UpdateWithPowerOff()
        {
            btnPower.Image = Properties.Resources.FanOff;

            // pulses
            lbPulsesData.Text = Cal.PulseCount.ToString("N0");

            // base rate
            tbBaseRate.Enabled = false;
            tbBaseRate.Text = "0";

            // meter cal
            tbMeterCal.Enabled = false;
            tbMeterCal.Text = "0";

            // expected amount
            lbExpectedData.Text = "0";

            // measured amount
            tbMeasured.Enabled = false;
            tbMeasured.Text = "0";

            // progress bar
            pbRunning.Visible = false;

            // lock
            btnLocked.Enabled = false;
            btnLocked.Image = Properties.Resources.ColorUnlocked;
            btnLocked.Visible = true;

            // pwm display
            lbPWMData.Text = Cal.CalPWM.ToString("N0");
        }

        private void UpdateWithPowerOn()
        {
            btnPower.Image = Properties.Resources.FanOn;

            // pulses
            lbPulsesData.Text = Cal.PulseCount.ToString("N0");

            // base rate
            tbBaseRate.Enabled = (!Cal.Running && !Cal.Locked);
            tbBaseRate.Text = Prd.RateSet.ToString("N1");

            // meter cal
            tbMeterCal.Enabled = !Cal.Running && !Cal.Locked;
            tbMeterCal.Text = Cal.MeterCal.ToString("N1");

            // progress bar
            pbRunning.Visible = (!Cal.Locked && Cal.Running);

            if (Cal.Locked)
            {
                // Testing Rate, run in manual at CalPWM

                // expected amount
                lbExpectedData.Text = Cal.ExpectedAmount.ToString("N1");

                // measured amount
                tbMeasured.Enabled = (!Cal.Running && Cal.TestingRate);
                tbMeasured.Text = Cal.MeasuredAmount.ToString("N1");

                // lock
                btnLocked.Enabled = !Cal.Running;
                btnLocked.Image = Properties.Resources.ColorLocked;
                btnLocked.Visible = true;

                // pwm display
                lbPWMData.Text = Cal.CalPWM.ToString("N0");
            }
            else
            {
                // Setting PWM, auto on, find CalPWM

                // expected amount
                lbExpectedData.Text = "0";

                // measured amount
                tbMeasured.Enabled = false;
                tbMeasured.Text = "0";

                // lock
                btnLocked.Enabled = !Cal.Running;
                btnLocked.Image = Properties.Resources.ColorUnlocked;
                btnLocked.Visible = !Cal.Running;

                // pwm display
                if (Cal.Running)
                {
                    lbPWMData.Text = Prd.PWM().ToString("N0");
                }
                else
                {
                    lbPWMData.Text = Cal.CalPWM.ToString("N0");
                }
            }
        }
    }
}