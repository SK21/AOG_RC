using AgOpenGPS;
using RateController.Classes;
using RateController.Language;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuOptions : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private bool WheelSpeedChanged = false;

        public frmMenuOptions(frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            this.Tag = false;
        }

        private void btnCal_Click(object sender, EventArgs e)
        {
            double calNumber = 0;
            bool wasCanceled = true;

            using (var dlg = new RateController.Forms.frmSpeedCal())
            {
                var result = dlg.ShowDialog(this);
                wasCanceled = (result != DialogResult.OK) || dlg.Canceled;
                calNumber = dlg.CalNumber;
            }

            if (!wasCanceled) tbWheelCal.Text = calNumber.ToString("N0");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
            WheelSpeedChanged = false;
            butUpdateModules.Enabled = rbWheel.Checked;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (rbAOG.Checked)
                {
                    Props.SpeedMode = SpeedType.GPS;
                }
                else if (rbWheel.Checked)
                {
                    Props.SpeedMode = SpeedType.Wheel;
                }
                else
                {
                    Props.SpeedMode = SpeedType.Simulated;
                }

                if (int.TryParse(tbWheelModule.Text, out int wm)) Core.WheelSpeed.WheelModule = wm;

                if (int.TryParse(tbWheelPin.Text, out int wp) && (ValidPin(wp)))
                {
                    Core.WheelSpeed.WheelPin = wp;
                }
                else
                {
                    Core.WheelSpeed.WheelPin = 255;
                }

                if (double.TryParse(tbWheelCal.Text, out double wc)) Core.WheelSpeed.WheelCal = wc;

                if (double.TryParse(tbSimSpeed.Text, out double Speed))
                {
                    if (Props.UseMetric)
                    {
                        Props.SimSpeed_KMH = Speed;
                    }
                    else
                    {
                        Props.SimSpeed_KMH = Speed * Props.MPHtoKPH;
                    }
                }

                Props.UseMetric = ckMetric.Checked;
                Props.UseRateDisplay = ckRateDisplay.Checked;

                if (WheelSpeedChanged)
                {
                    Core.WheelSpeed.Send();
                    WheelSpeedChanged = false;
                    butUpdateModules.Enabled = rbWheel.Checked;
                }

                SetButtons(false);
                UpdateForm();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuDisplay/btnOk_Click: " + ex.Message);
            }
        }

        private void butUpdateModules_Click(object sender, EventArgs e)
        {
            Core.WheelSpeed.Send();
        }

        private void ckLargeScreen_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void frmMenuDisplay_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
        }

        private void frmMenuDisplay_Load(object sender, EventArgs e)
        {
            SubMenuLayout.SetFormLayout(this, MainMenu, btnOK);

            btnCancel.Left = btnOK.Left - SubMenuLayout.ButtonSpacing;
            btnCancel.Top = btnOK.Top;
            MainMenu.StyleControls(this);
            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            this.BackColor = Properties.Settings.Default.MainBackColour;
            lbPulses.Font = new Font(lbPulses.Font.FontFamily, 12f, lbPulses.Font.Style,
                                lbPulses.Font.Unit, lbPulses.Font.GdiCharSet, lbPulses.Font.GdiVerticalFont);

            PositionForm();
            SetBoxes();
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

        private void rbAOG_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing)
            {
                SetButtons(true);
                SetBoxes();
            }
        }

        private void rbWheel_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing)
            {
                WheelSpeedChanged = true;
                SetButtons(true);
                SetBoxes();
            }
        }

        private void SetBoxes()
        {
            tbWheelModule.Enabled = rbWheel.Checked;
            tbWheelPin.Enabled = rbWheel.Checked;
            tbWheelCal.Enabled = rbWheel.Checked;
            tbSimSpeed.Enabled = rbSimulated.Checked;
            lbCal.Enabled = rbWheel.Checked;
            lbModule.Enabled = rbWheel.Checked;
            lbPin.Enabled = rbWheel.Checked;
            butUpdateModules.Enabled = rbWheel.Checked;
            lbPulses.Enabled = rbWheel.Checked;
            btnCal.Enabled = rbWheel.Checked;
        }

        private void SetButtons(bool Edited)
        {
            if (!Initializing)
            {
                if (Edited)
                {
                    btnCancel.Enabled = true;
                    btnOK.Enabled = true;
                }
                else
                {
                    btnCancel.Enabled = false;
                    btnOK.Enabled = false;
                }

                cEdited = Edited;
                this.Tag = cEdited;
            }
        }

        private void SetLanguage()
        {
            ckMetric.Text = Lang.lgMetric;
            rbSimulated.Text = Lang.lgSimulateSpeed;
        }

        private void tbSimSpeed_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbSimSpeed.Text, out tempD);
            using (var form = new FormNumeric(0, 40, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbSimSpeed.Text = form.ReturnValue.ToString("N1");
                }
            }
        }

        private void tbSimSpeed_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbSimSpeed.Text, out tempD);
            if (tempD < 0 || tempD > 40)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbWheelCal_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbWheelCal.Text, out tempD);
            using (var form = new FormNumeric(0, 0xffffff, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbWheelCal.Text = form.ReturnValue.ToString("N0");
                }
            }
        }

        private void tbWheelCal_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbWheelCal.Text, out tempD);
            if (tempD < 0 || tempD > 0xffffff)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbWheelModule_Enter(object sender, EventArgs e)
        {
            int tempInt;
            int.TryParse(tbWheelModule.Text, out tempInt);
            using (var form = new FormNumeric(0, 7, tempInt))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbWheelModule.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void tbWheelModule_Validating(object sender, CancelEventArgs e)
        {
            int tempInt;
            int.TryParse(tbWheelModule.Text, out tempInt);
            if (tempInt < 0 || tempInt > 7)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbWheelPin_Enter(object sender, EventArgs e)
        {
            var bx = (System.Windows.Forms.TextBox)sender;
            double temp = 0;
            if (double.TryParse(bx.Text.Trim(), out double vl)) temp = vl;

            using (var form = new FormNumeric(0, 50, temp))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (form.IsBlank)
                    {
                        bx.Text = "-";
                    }
                    else
                    {
                        bx.Text = form.ReturnValue.ToString("N0");
                    }
                }
            }
        }

        private void tbWheelPin_Validating(object sender, CancelEventArgs e)
        {
            int tempInt;
            int.TryParse(tbWheelPin.Text, out tempInt);
            if (tempInt < 0 || tempInt > 50)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void UpdateForm()
        {
            Initializing = true;

            switch (Props.SpeedMode)
            {
                case SpeedType.Wheel:
                    rbWheel.Checked = true;
                    break;

                case SpeedType.Simulated:
                    rbSimulated.Checked = true;
                    break;

                default:
                    rbAOG.Checked = true;
                    break;
            }

            tbWheelModule.Text = Core.WheelSpeed.WheelModule.ToString("N0");

            if (Core.WheelSpeed.WheelPin == 255)
            {
                tbWheelPin.Text = "-";
            }
            else
            {
                tbWheelPin.Text = Core.WheelSpeed.WheelPin.ToString("N0");
            }

            tbWheelCal.Text = Core.WheelSpeed.WheelCal.ToString("N0");

            if (Props.UseMetric)
            {
                lbSimUnits.Text = "KMH";
                lbPulses.Text = "(pulses/km)";
                tbSimSpeed.Text = Props.SimSpeed_KMH.ToString("N1");
            }
            else
            {
                lbSimUnits.Text = "MPH";
                lbPulses.Text = "(pulses/mile)";
                tbSimSpeed.Text = (Props.SimSpeed_KMH / Props.MPHtoKPH).ToString("N1");
            }

            ckMetric.Checked = Props.UseMetric;
            ckRateDisplay.Checked = Props.UseRateDisplay;

            SetBoxes();

            Initializing = false;
        }

        private bool ValidPin(int pin)
        {
            bool Result = true;
            if (Core.ModuleConfig.Sensor0Flow == pin || Core.ModuleConfig.Sensor1Flow == pin)
            {
                Result = false;
                Props.ShowMessage("Invalid pin, duplicate of flow pin.");
            }
            return Result;
        }
    }
}