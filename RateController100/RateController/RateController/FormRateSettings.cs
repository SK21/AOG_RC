using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RateController
{
    public partial class FormRateSettings : Form
    {
        private readonly FormRateControl mf;
        private bool Initializing = false;
        private byte tempB = 0;
        private double tempD = 0;

        public FormRateSettings(FormRateControl CallingForm)
        {
            InitializeComponent();
            mf = CallingForm;
            Initializing = true;
            LoadSettings();
            Initializing = false;
        }

        private void AreaUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void bntOK_Click(object sender, EventArgs e)
        {
            Button ButtonClicked = (Button)sender;
            if (ButtonClicked.Text == "Close")
            {
                this.Close();
            }
            else
            {
                // save changes
                SaveSettings();
                SetButtons(false);
                Initializing = true;
                LoadSettings();
                Initializing = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            LoadSettings();
            SetButtons(false);
        }

        private void butLoadDefaults_Click(object sender, EventArgs e)
        {
            tbKP.Text = "100";
            tbKI.Text = "0";
            tbKD.Text = "0";
            tbDeadband.Text = "3";
            tbMinPWM.Text = "50";
            tbMaxPWM.Text = "255";
        }

        private void butResetAcres_Click(object sender, EventArgs e)
        {
            mf.RC.ResetCoverage();
        }

        private void butResetApplied_Click(object sender, EventArgs e)
        {
            mf.RC.ResetApplied();
        }

        private void butResetTank_Click(object sender, EventArgs e)
        {
            mf.RC.ResetTank();
            TankRemain.Text = mf.RC.CurrentTankRemaining();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void FlowCal_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void FlowCal_Validating(object sender, CancelEventArgs e)
        {
            double.TryParse(FlowCal.Text, out tempD);
            if (tempD < 0 || tempD > 10000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
                FlowCal.Select(0, FlowCal.Text.Length);
                mf.Tls.TimedMessageBox("Meter Cal Error", "Min 0, Max 10,000", 3000, true);
            }
        }

        private void FormRateSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                // save location and size if the state is normal
                Properties.Settings.Default.FormRSlocation = this.Location;
            }
            else
            {
                // save the RestoreBounds if the form is minimized or maximized!
                Properties.Settings.Default.FormRSlocation = this.RestoreBounds.Location;
            }

            // don't forget to save the settings
            Properties.Settings.Default.Save();
        }

        private void FormRateSettings_Load(object sender, EventArgs e)
        {
            this.Location = Properties.Settings.Default.FormRSlocation;
            mf.Tls.IsOnScreen(this, true);
            lbIP.Text = mf.UDP.LocalIP;
            lbVersion.Text = mf.Tls.VersionDate();
        }

        private void LoadSettings()
        {
            VolumeUnits.SelectedIndex = mf.RC.QuantityUnits;
            AreaUnits.SelectedIndex = mf.RC.CoverageUnits;
            RateSet.Text = mf.RC.RateSet.ToString("N1");
            FlowCal.Text = mf.RC.FlowCal.ToString("N1");
            TankSize.Text = mf.RC.TankSize.ToString("N1");
            ValveType.SelectedIndex = mf.RC.ValveType;
            TankRemain.Text = mf.RC.CurrentTankRemaining();
            checkBox1.Checked = mf.RC.SimulateFlow;
            tbKP.Text = (mf.RC.KP).ToString("N0");
            tbKI.Text = (mf.RC.KI).ToString("N0");
            tbKD.Text = (mf.RC.KD).ToString("N0");
            tbDeadband.Text = (mf.RC.DeadBand).ToString("N0");
            tbMinPWM.Text = (mf.RC.MinPWM).ToString("N0");
            tbMaxPWM.Text = (mf.RC.MaxPWM).ToString("N0");
        }

        private void RateSet_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void RateSet_Validating(object sender, CancelEventArgs e)
        {
            double.TryParse(RateSet.Text, out tempD);
            if (tempD < 0 || tempD > 10000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
                RateSet.Select(0, RateSet.Text.Length);
                var ErrForm = new FormTimedMessage("Rate Set Error", "Min 0, Max 10,000");
                ErrForm.Show();
            }
        }

        private void SaveSettings()
        {
            mf.RC.QuantityUnits = Convert.ToByte(VolumeUnits.SelectedIndex);
            mf.RC.CoverageUnits = Convert.ToByte(AreaUnits.SelectedIndex);

            double.TryParse(RateSet.Text, out tempD);
            mf.RC.RateSet = tempD;

            double.TryParse(FlowCal.Text, out tempD);
            mf.RC.FlowCal = tempD;

            double.TryParse(TankSize.Text, out tempD);
            mf.RC.TankSize = tempD;

            mf.RC.ValveType = Convert.ToByte(ValveType.SelectedIndex);

            double.TryParse(TankRemain.Text, out tempD);
            mf.RC.SetTankRemaining(tempD);

            mf.RC.SimulateFlow = checkBox1.Checked;

            byte.TryParse(tbKP.Text, out tempB);
            mf.RC.KP = tempB;

            byte.TryParse(tbKI.Text, out tempB);
            mf.RC.KI = tempB;

            byte.TryParse(tbKD.Text, out tempB);
            mf.RC.KD = tempB;

            byte.TryParse(tbDeadband.Text, out tempB);
            if (tempB > 15) tempB = 15;
            mf.RC.DeadBand = tempB;

            byte.TryParse(tbMinPWM.Text, out tempB);
            mf.RC.MinPWM = tempB;

            byte.TryParse(tbMaxPWM.Text, out tempB);
            mf.RC.MaxPWM = tempB;
            mf.RC.SaveSettings();
        }

        private void SetButtons(bool Edited)
        {
            if (!Initializing)
            {
                if (Edited)
                {
                    btnCancel.Enabled = true;
                    this.bntOK.Text = "Save";
                }
                else
                {
                    btnCancel.Enabled = false;
                    this.bntOK.Text = "Close";
                }
            }
        }

        private void TankRemain_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void TankRemain_Validating(object sender, CancelEventArgs e)
        {
            double.TryParse(TankRemain.Text, out tempD);
            if (tempD < 0 || tempD > 100000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
                TankRemain.Select(0, TankRemain.Text.Length);
                var ErrForm = new FormTimedMessage("Tank Remaining Error", "Min 0, Max 100,000");
                ErrForm.Show();
            }
        }

        private void TankSize_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void TankSize_Validating(object sender, CancelEventArgs e)
        {
            double.TryParse(TankSize.Text, out tempD);
            if (tempD < 0 || tempD > 100000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
                TankSize.Select(0, TankSize.Text.Length);
                var ErrForm = new FormTimedMessage("Tank Size Error", "Min 0, Max 100,000");
                ErrForm.Show();
            }
        }

        private void tbDeadband_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbKD_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbKI_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbKP_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbMaxPWM_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbMinPWM_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void ValveType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void VolumeUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }
    }
}