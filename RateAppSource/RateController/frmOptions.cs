using AgOpenGPS;
using RateController.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmOptions : Form
    {
        public FormStart mf;
        private bool FormEdited;
        private bool Initializing;
        private string[] LanguageIDs;
        private RadioButton[] LanguageRBs;
        private bool SimSpeedChanged = false;
        private TabPage[] Tabs;

        public frmOptions(FormStart CalledFrom)
        {
            InitializeComponent();

            #region // language

            tcOptions.TabPages[0].Text = Lang.lgDisplay;
            tcOptions.TabPages[1].Text = Lang.lgConfig;
            tcOptions.TabPages[2].Text = Lang.lgPrimedStart;
            tcOptions.TabPages[3].Text = Lang.lgLanguage;

            ckMetric.Text = Lang.lgMetric;
            ckScreenSwitches.Text = Lang.lgSwitches;
            ckWorkSwitch.Text = Lang.lgWorkSwitch;
            ckLargeScreen.Text = Lang.lgLargeScreen;
            ckTransparent.Text = Lang.lgTransparent;

            lbOnTime.Text = Lang.lgOnTime;
            lbPrimedSpeed.Text = Lang.lgSpeed;
            lbDelay.Text = Lang.lgSwitchDelay;
            lbOnSeconds.Text = Lang.lgSeconds;
            lbDelaySeconds.Text = Lang.lgSeconds;

            #endregion // language

            mf = CalledFrom;

            LanguageRBs = new RadioButton[] { rbEnglish, rbDeustch, rbHungarian, rbNederlands, rbPolish, rbRussian, rbFrench };
            LanguageIDs = new string[] { "en", "de", "hu", "nl", "pl", "ru", "fr" };
            for (int i = 0; i < LanguageRBs.Length; i++)
            {
                LanguageRBs[i].CheckedChanged += Language_CheckedChanged;
            }

            Tabs = new TabPage[] { tabPage1, tabPage2, tabPage3, tabPage4 };
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm();
            btnOK.Focus();
            SetButtons(false);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (!FormEdited)
                {
                    this.Close();
                }
                else
                {
                    Save();
                    SetButtons(false);
                    UpdateForm();
                }
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp(ex.Message, this.Text, 3000, true);
            }
        }

        private void ckDualAuto_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void ckNoMaster_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void ckTransparent_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void frmOptions_FormClosed(object sender, FormClosedEventArgs e)
        {
            mf.Tls.SaveFormData(this);
        }

        private void frmOptions_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);

            SetDayMode();
            UpdateForm();
        }

        private void Language_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void rbLarge_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void rbStandard_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void Save()
        {
            if (SimSpeedChanged)
            {
                if (double.TryParse(tbSimSpeed.Text, out double Speed)) mf.SimSpeed = Speed;
                SimSpeedChanged = false;
            }
            else
            {
                if (double.TryParse(tbSpeed.Text, out double Spd)) mf.SimSpeed = Spd;
            }

            if (double.TryParse(tbTime.Text, out double Time)) mf.PrimeTime = Time;
            if (int.TryParse(tbDelay.Text, out int Delay)) mf.PrimeDelay = Delay;

            mf.MasterOverride = ckNoMaster.Checked;
            mf.UseTransparent = ckTransparent.Checked;
            mf.UseInches = !ckMetric.Checked;
            mf.ShowSwitches = ckScreenSwitches.Checked;
            mf.SwitchBox.UseWorkSwitch = ckWorkSwitch.Checked;
            mf.ShowPressure = ckPressure.Checked;

            if (ckSimSpeed.Checked)
            {
                mf.SimMode = SimType.Speed;
            }
            else
            {
                mf.SimMode = SimType.None;
            }

            if (ckLargeScreen.Checked)
            {
                // use large screen
                Form fs = mf.Tls.IsFormOpen("frmLargeScreen");
                if (fs == null)
                {
                    if (!mf.UseLargeScreen) mf.StartLargeScreen();
                }
            }
            else
            {
                // use standard screen
                Form fs = mf.Tls.IsFormOpen("frmLargeScreen");
                if (fs != null)
                {
                    mf.Lscrn.SwitchToStandard();
                }
            }

            mf.UseDualAuto = ckDualAuto.Checked;
            mf.ResumeAfterPrime = ckResume.Checked;

            SaveLanguage();
        }

        private void SaveLanguage()
        {
            int Lan = 0;
            for (int i = 0; i < LanguageRBs.Length; i++)
            {
                if (LanguageRBs[i].Checked)
                {
                    Lan = i;
                    break;
                }
            }

            if (Properties.Settings.Default.setF_culture != LanguageIDs[Lan])
            {
                Properties.Settings.Default.setF_culture = LanguageIDs[Lan];
                Settings.Default.UserLanguageChange = true;
                Properties.Settings.Default.Save();

                Form fs = mf.Tls.IsFormOpen("frmLargeScreen");
                if (fs != null)
                {
                    mf.Restart = true;
                    mf.Lscrn.Close();
                }
                else
                {
                    mf.ChangeLanguage();
                }
            }
        }

        private void SetButtons(bool Edited = false)
        {
            if (!Initializing)
            {
                if (Edited)
                {
                    btnCancel.Enabled = true;
                    btnOK.Image = Properties.Resources.Save;
                }
                else
                {
                    btnCancel.Enabled = false;
                    btnOK.Image = Properties.Resources.OK;
                    btnOK.Enabled = true;
                }
                FormEdited = Edited;
            }
        }

        private void SetDayMode()
        {
            if (Properties.Settings.Default.IsDay)
            {
                this.BackColor = Properties.Settings.Default.DayColour;
                foreach (Control c in this.Controls)
                {
                    c.ForeColor = Color.Black;
                }

                for (int i = 0; i < Tabs.Length; i++)
                {
                    Tabs[i].BackColor = Properties.Settings.Default.DayColour;
                }
            }
            else
            {
                this.BackColor = Properties.Settings.Default.NightColour;
                foreach (Control c in this.Controls)
                {
                    c.ForeColor = Color.White;
                }

                for (int i = 0; i < Tabs.Length; i++)
                {
                    Tabs[i].BackColor = Properties.Settings.Default.NightColour;
                }
            }
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

        private void tbSimSpeed_TextChanged(object sender, EventArgs e)
        {
            SimSpeedChanged = true;
            SetButtons(true);
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
            tbSimSpeed.Text = mf.SimSpeed.ToString("N1");

            tbTime.Text = mf.PrimeTime.ToString("N0");
            tbDelay.Text = mf.PrimeDelay.ToString("N0");

            ckTransparent.Checked = mf.UseTransparent;
            ckMetric.Checked = !mf.UseInches;
            ckScreenSwitches.Checked = mf.ShowSwitches;
            ckWorkSwitch.Checked = mf.SwitchBox.UseWorkSwitch;
            ckPressure.Checked = mf.ShowPressure;
            ckSimSpeed.Checked = (mf.SimMode == SimType.Speed);
            ckDualAuto.Checked = mf.UseDualAuto;
            ckLargeScreen.Checked = mf.UseLargeScreen;
            ckResume.Checked = mf.ResumeAfterPrime;
            ckNoMaster.Checked = mf.MasterOverride;

            // language
            for (int i = 0; i < LanguageRBs.Length; i++)
            {
                if (LanguageIDs[i] == Properties.Settings.Default.setF_culture)
                {
                    LanguageRBs[i].Checked = true;
                    break;
                }
            }

            if (mf.UseInches)
            {
                lbSpeed.Text = "MPH";
                lbSimUnits.Text = "MPH";
            }
            else
            {
                lbSpeed.Text = "KMH";
                lbSimUnits.Text = "KMH";
            }

            Initializing = false;
        }
    }
}