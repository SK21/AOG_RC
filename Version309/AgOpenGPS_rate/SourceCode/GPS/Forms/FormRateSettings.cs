using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AgOpenGPS.Forms
{
    public partial class FormRateSettings : Form
    {
        private readonly FormGPS mf;
        private bool Initializing = false;
        byte tempB = 0;
        double tempD = 0;
        public FormRateSettings(FormGPS CallingForm)
        {
            mf = CallingForm;
            InitializeComponent();
            Initializing = true;
            LoadSettings();
            Initializing = false;
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
            tbKP.Text = (mf.RC.KP ).ToString("N0");
            tbKI.Text = (mf.RC.KI ).ToString("N0");
            tbKD.Text = (mf.RC.KD ).ToString("N0");
            tbDeadband.Text = (mf.RC.DeadBand ).ToString("N0");
            tbMinPWM.Text = (mf.RC.MinPWM).ToString("N0");
            tbMaxPWM.Text = (mf.RC.MaxPWM).ToString("N0");
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            LoadSettings();
            SetButtons(false);
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

        private void RateSet_Validating(object sender, CancelEventArgs e)
        {
            double.TryParse(RateSet.Text, out tempD);
            if (tempD < 0 || tempD > 10000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
                RateSet.Select(0, RateSet.Text.Length);
                var ErrForm = new FormTimedMessage(2000, "Rate Set Error", "Min 0, Max 10,000");
                ErrForm.Show();
            }
        }

        private void RateSet_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
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

        private void AreaUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void FlowCal_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void TankSize_TextChanged(object sender, EventArgs e)
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

        private void FlowCal_Validating(object sender, CancelEventArgs e)
        {
            double.TryParse(FlowCal.Text, out tempD);
            if (tempD < 0 || tempD > 10000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
                FlowCal.Select(0, FlowCal.Text.Length);
                var ErrForm = new FormTimedMessage(2000, "Meter Cal Error", "Min 0, Max 10,000");
                ErrForm.Show();
            }
        }

        private void TankSize_Validating(object sender, CancelEventArgs e)
        {
            double.TryParse(TankSize.Text, out tempD);
            if (tempD < 0 || tempD > 100000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
                TankSize.Select(0, TankSize.Text.Length);
                var ErrForm = new FormTimedMessage(2000, "Tank Size Error", "Min 0, Max 100,000");
                ErrForm.Show();
            }
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
                var ErrForm = new FormTimedMessage(2000, "Tank Remaining Error", "Min 0, Max 100,000");
                ErrForm.Show();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            mf.RC.SimulateFlow = checkBox1.Checked;
            SetButtons(true);
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label34_Click(object sender, EventArgs e)
        {

        }

        private void tbKP_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbKI_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbKD_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbDeadband_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbMinPWM_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbMaxPWM_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void FormRateSettings_Load(object sender, EventArgs e)
        {

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
    }
}
