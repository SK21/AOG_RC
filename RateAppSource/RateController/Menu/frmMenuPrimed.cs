using AgOpenGPS;
using RateController.Classes;
using RateController.Language;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuPrimed : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;

        public frmMenuPrimed(frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            this.Tag = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (double.TryParse(tbSpeed.Text, out double Spd))
                {
                    if (Props.UseMetric)
                    {
                        Props.SimSpeed_KMH = Spd;
                    }
                    else
                    {
                        Props.SimSpeed_KMH = Spd * Props.MPHtoKPH;
                    }
                }
                if (double.TryParse(tbTime.Text, out double Time)) Props.PrimeTime = Time;
                if (int.TryParse(tbDelay.Text, out int Delay)) Props.PrimeDelay = Delay;
                Props.ResumeAfterPrime = ckResume.Checked;

                if (int.TryParse(tbPrimeRate.Text, out int Pct)) Props.PrimeRatePct = Pct;
                Props.PrimeEndOnMovement = rbEndMoving.Checked;
                if (double.TryParse(tbEndSpeed.Text, out double EndSpd))
                {
                    Props.PrimeEndSpeed_KMH = Props.UseMetric ? EndSpd : EndSpd * Props.MPHtoKPH;
                }
                SetButtons(false);
                UpdateForm();
                MainMenu.HighlightUpdateButton();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuPrimed/btnOk_Click: " + ex.Message);
            }
        }

        private void frmMenuPrimed_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
        }

        private void frmMenuPrimed_Load(object sender, EventArgs e)
        {
            SubMenuLayout.SetFormLayout(this, MainMenu, null);
            btnOK.Left = this.Width - 84;
            btnOK.Top = this.Height - 84;
            btnCancel.Left = btnOK.Left - 78;
            btnCancel.Top = btnOK.Top;
            Font hintFont = lbPrimeHint.Font;      // StyleControls sets every label to the menu font
            MainMenu.StyleControls(this);
            lbPrimeHint.Font = hintFont;
            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            this.BackColor = Properties.Settings.Default.MainBackColour;
            PositionForm();
            UpdateForm();
        }

        private void gbPrimeEnds_Paint(object sender, PaintEventArgs e)
        {
            Props.DrawGroupBox((GroupBox)sender, e.Graphics, this.BackColor, Color.Black, Color.Blue);
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
            lbOnTime.Text = Lang.lgOnTime;
            lbPrimedSpeed.Text = Lang.lgSimulateSpeed;       // lbSpeed is the unit, set in UpdateForm
            lbDelay.Text = Lang.lgSwitchDelay;
            ckResume.Text = Lang.lgKeepMasterOn;
            lbOnSeconds.Text = Lang.lgSeconds;
            lbDelaySeconds.Text = Lang.lgSeconds;
            lbPrimeRate.Text = Lang.lgPrimeRate;
            gbPrimeEnds.Text = Lang.lgPrimeEnds;
            rbEndTime.Text = Lang.lgPrimeEndTime;
            rbEndMoving.Text = Lang.lgPrimeEndMoving;
        }

        private void tbEndSpeed_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbEndSpeed.Text, out tempD);
            using (var form = new FormNumeric(EndSpeedMin(), EndSpeedMax(), tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbEndSpeed.Text = form.ReturnValue.ToString("N1");
                }
            }
        }

        private void tbEndSpeed_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbEndSpeed.Text, out tempD);
            if (tempD < EndSpeedMin() || tempD > EndSpeedMax())
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        // the end speed range, 0.3 to 5 km/h, in the units shown
        private double EndSpeedMin()
        {
            return Props.UseMetric ? 0.3 : 0.2;
        }

        private double EndSpeedMax()
        {
            return Props.UseMetric ? 5 : 3;
        }

        private void tbPrime_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
            UpdatePrimeInfo();
        }

        private void tbPrimeRate_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbPrimeRate.Text, out tempD);
            using (var form = new FormNumeric(0, 100, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbPrimeRate.Text = form.ReturnValue.ToString("N0");
                }
            }
        }

        private void tbPrimeRate_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbPrimeRate.Text, out tempD);
            if (tempD < 0 || tempD > 100)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        // the end speed box follows the "When moving" choice; the hint says what will flow
        private void UpdatePrimeInfo()
        {
            tbEndSpeed.Enabled = rbEndMoving.Checked;

            double.TryParse(tbSpeed.Text, out double Spd);
            double.TryParse(tbPrimeRate.Text, out double Pct);
            if (Spd <= 0 || Pct <= 0)
            {
                lbPrimeHint.Text = Lang.lgPrimeNoFlow;
            }
            else
            {
                string Hint = Lang.lgPrimeFloorNote;
                if (rbEndMoving.Checked) Hint += Environment.NewLine + Lang.lgPrimeMaxStopped;
                lbPrimeHint.Text = Hint;
            }
        }

        private void tbDelay_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbDelay.Text, out tempD);
            using (var form = new FormNumeric(3, 8, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbDelay.Text = form.ReturnValue.ToString("N0");
                }
            }
        }

        private void tbDelay_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbDelay.Text, out tempD);
            if (tempD < 3 || tempD > 8)     // Props.PrimeDelay accepts 3-8: a shorter hold would start prime by accident
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
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

        private void tbSpeed_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbSpeed.Text, out tempD);
            if (tempD < 0 || tempD > 40)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbTime_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbTime.Text, out tempD);
            using (var form = new FormNumeric(0, 300, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbTime.Text = form.ReturnValue.ToString("N1");
                }
            }
        }

        private void tbTime_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbTime_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbTime.Text, out tempD);
            if (tempD < 0 || tempD > 300)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void UpdateForm()
        {
            Initializing = true;

            double displaySpeed = Props.UseMetric ? Props.SimSpeed_KMH : Props.SimSpeed_KMH / Props.MPHtoKPH;
            tbSpeed.Text = displaySpeed.ToString("N1");

            tbTime.Text = Props.PrimeTime.ToString("N0");
            tbDelay.Text = Props.PrimeDelay.ToString("N0");

            ckResume.Checked = Props.ResumeAfterPrime;

            tbPrimeRate.Text = Props.PrimeRatePct.ToString("N0");
            rbEndMoving.Checked = Props.PrimeEndOnMovement;
            rbEndTime.Checked = !Props.PrimeEndOnMovement;
            double endSpeed = Props.UseMetric ? Props.PrimeEndSpeed_KMH : Props.PrimeEndSpeed_KMH / Props.MPHtoKPH;
            tbEndSpeed.Text = endSpeed.ToString("N1");

            if (!Props.UseMetric)
            {
                lbSpeed.Text = "MPH";
            }
            else
            {
                lbSpeed.Text = "KMH";
            }
            lbEndSpeedUnit.Text = lbSpeed.Text;
            UpdatePrimeInfo();
            Initializing = false;
        }
    }
}