using AgOpenGPS;
using RateController.Classes;
using RateController.Language;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuPrimed : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private FormStart mf;

        public frmMenuPrimed(FormStart main, frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            mf = main;
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
                if (double.TryParse(tbSpeed.Text, out double Spd)) mf.SimSpeed = Spd;
                if (double.TryParse(tbTime.Text, out double Time)) mf.PrimeTime = Time;
                if (int.TryParse(tbDelay.Text, out int Delay)) mf.PrimeDelay = Delay;
                mf.ResumeAfterPrime = ckResume.Checked;
                SetButtons(false);
                UpdateForm();
                MainMenu.HighlightUpdateButton();
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmMenuPrimed/btnOk_Click: " + ex.Message);
            }
        }

        private void frmMenuPrimed_FormClosed(object sender, FormClosedEventArgs e)
        {
            mf.Tls.SaveFormData(this);
        }

        private void frmMenuPrimed_Load(object sender, EventArgs e)
        {
            // menu 800,600
            // sub menu 540,630
            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            mf.Tls.LoadFormData(this, "", false);
            this.BackColor = Properties.Settings.Default.MainBackColour;
            this.Width = MainMenu.Width - 260;
            this.Height = MainMenu.Height - 50;
            btnOK.Left = this.Width - 84;
            btnOK.Top = this.Height - 84;
            btnCancel.Left = btnOK.Left - 78;
            btnCancel.Top = btnOK.Top;
            MainMenu.StyleControls(this);
            PositionForm();
            UpdateForm();
        }

        private void lbDelay_Click(object sender, EventArgs e)
        {
        }

        private void MainMenu_MenuMoved(object sender, EventArgs e)
        {
            PositionForm();
        }

        private void PositionForm()
        {
            this.Top = MainMenu.Top + 30;
            this.Left = MainMenu.Left + 246;
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
            lbSpeed.Text = Lang.lgSpeed;
            lbDelay.Text = Lang.lgSwitchDelay;
            ckResume.Text = Lang.lgResume;
            lbOnSeconds.Text = Lang.lgSeconds;
            lbDelaySeconds.Text = Lang.lgSeconds;
        }

        private void tbDelay_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbDelay.Text, out tempD);
            using (var form = new FormNumeric(0, 8, tempD))
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
            if (tempD < 0 || tempD > 8)
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
            using (var form = new FormNumeric(0, 30, tempD))
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
            if (tempD < 0 || tempD > 30)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void UpdateForm()
        {
            Initializing = true;
            tbSpeed.Text = mf.SimSpeed.ToString("N1");
            tbTime.Text = mf.PrimeTime.ToString("N0");
            tbDelay.Text = mf.PrimeDelay.ToString("N0");

            if (!Props.UseMetric)
            {
                lbSpeed.Text = "MPH";
            }
            else
            {
                lbSpeed.Text = "KMH";
            }
            Initializing = false;
        }
    }
}