using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using AgOpenGPS;

namespace RateController
{
    public partial class FormSettings : Form
    {
        private TabPage[] tbs;
        private int CurrentProduct;
        public FormStart mf;

        private bool Initializing = false;
        private SimType SelectedSimulation;

        public FormSettings(FormStart CallingForm, int Page)
        {
            InitializeComponent();
            tbs = new TabPage[4] { tbs0, tbs1, tbs2, tbs3 };
            mf = CallingForm;
            CurrentProduct = Page - 1;
            if (CurrentProduct < 0) CurrentProduct = 0;

            openFileDialog1.InitialDirectory = mf.Tls.SettingsDir();
            saveFileDialog1.InitialDirectory = mf.Tls.SettingsDir();
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);

            LoadRCbox();
            SetPortButtons();

            SetDayMode();
            timer1.Enabled = true;

            Initializing = true;
            UpdateDisplay();
            Initializing = false;
        }

        private void SetDayMode()
        {
            if (Properties.Settings.Default.IsDay)
            {
                this.BackColor = Properties.Settings.Default.DayColour;

                for (int i = 0; i < 4; i++)
                {
                    tbs[i].BackColor = Properties.Settings.Default.DayColour;
                }

                foreach (Control c in this.Controls)
                {
                    c.ForeColor = Color.Black;
                }
            }
            else
            {
                this.BackColor = Properties.Settings.Default.NightColour;

                for (int i = 0; i < 4; i++)
                {
                    tbs[i].BackColor = Properties.Settings.Default.NightColour;
                }

                foreach (Control c in this.Controls)
                {
                    c.ForeColor = Color.White;
                }
            }

            // fix backcolor
            this.BackColor = Properties.Settings.Default.DayColour;
            for (int i = 0; i < tc.TabCount; i++)
            {
                tc.TabPages[i].BackColor = Properties.Settings.Default.DayColour;
            }
            tbVCNdescription.BackColor = Properties.Settings.Default.DayColour;
        }

        private void tb14_TextChanged(object sender, EventArgs e)
        {

        }

        private void tb10_TextChanged(object sender, EventArgs e)
        {

        }

        private void UpdateDisplay()
        {
            lbProduct.Text = (CurrentProduct + 1).ToString() + ". " + mf.RateCals[CurrentProduct].ProductName;
            UpdateDiags();
            LoadSettings();
            SetPortButtons();
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            if (CurrentProduct > 0)
            {
                CurrentProduct--;
                Initializing = true;
                UpdateDisplay();
                Initializing = false;
            }
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            if (CurrentProduct < 4)
            {
                CurrentProduct++;
                Initializing = true;
                UpdateDisplay();
                Initializing = false;
            }
        }

        private void FormSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                mf.Tls.SaveFormData(this);
            }
            timer1.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateDiags();
        }

        void UpdateDiags()
        {
            lbWorkRateData.Text = mf.RateCals[CurrentProduct].WorkRate().ToString("N1");
            if (mf.RateCals[CurrentProduct].CoverageUnits == 0)
            {
                lbWorkRate.Text = Lang.lgAcresHr;
            }
            else
            {
                lbWorkRate.Text = Lang.lgHectares_Hr;
            }

            lbRateSetData.Text = mf.RateCals[CurrentProduct].TargetUPM().ToString("N1");
            lbRateAppliedData.Text = mf.RateCals[CurrentProduct].UPMapplied().ToString("N1");
            lbPWMdata.Text = mf.RateCals[CurrentProduct].PWM().ToString("N0");

            lbWidthData.Text = mf.RateCals[CurrentProduct].Width().ToString("N1");
            if (mf.RateCals[CurrentProduct].CoverageUnits == 0)
            {
                lbWidth.Text = Lang.lgWorkingWidthFT;
            }
            else
            {
                lbWidth.Text = Lang.lgWorkingWidthM;
            }

            lbSecHiData.Text = mf.RateCals[CurrentProduct].SectionHi().ToString();
            lbSecLoData.Text = mf.RateCals[CurrentProduct].SectionLo().ToString();

            lbSpeedData.Text = mf.RateCals[CurrentProduct].Speed().ToString("N1");
            if (mf.RateCals[CurrentProduct].CoverageUnits == 0)
            {
                lbSpeed.Text = Lang.lgMPH;
            }
            else
            {
                lbSpeed.Text = Lang.lgKPH;
            }
            tbConID.Text = mf.RateCals[CurrentProduct].ControllerID.ToString();
        }

        private void LoadSettings()
        {
            tbProduct.Text = mf.RateCals[CurrentProduct].ProductName;
            VolumeUnits.SelectedIndex = mf.RateCals[CurrentProduct].QuantityUnits;
            AreaUnits.SelectedIndex = mf.RateCals[CurrentProduct].CoverageUnits;
            RateSet.Text = mf.RateCals[CurrentProduct].RateSet.ToString("N1");
            FlowCal.Text = mf.RateCals[CurrentProduct].FlowCal.ToString("N1");
            TankSize.Text = mf.RateCals[CurrentProduct].TankSize.ToString("N0");
            ValveType.SelectedIndex = mf.RateCals[CurrentProduct].ValveType;
            TankRemain.Text = mf.RateCals[CurrentProduct].CurrentTankRemaining().ToString("N0");
            tbVCN.Text = (mf.RateCals[CurrentProduct].VCN).ToString("G0");
            tbSend.Text = (mf.RateCals[CurrentProduct].SendTime).ToString("N0");
            tbWait.Text = (mf.RateCals[CurrentProduct].WaitTime).ToString("N0");
            tbMinPWM.Text = (mf.RateCals[CurrentProduct].MinPWM).ToString("N0");

            SelectedSimulation = mf.RateCals[CurrentProduct].SimulationType;
            switch (SelectedSimulation)
            {
                case SimType.VirtualNano:
                    rbVirtual.Checked = true;
                    break;

                case SimType.RealNano:
                    rbReal.Checked = true;
                    break;

                default:
                    rbNone.Checked = true;
                    break;
            }
        }

        private void tbProduct_TextChanged(object sender, EventArgs e)
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
                    this.bntOK.Text = Lang.lgSave;
                }
                else
                {
                    btnCancel.Enabled = false;
                    this.bntOK.Text = Lang.lgClose;
                }
            }
        }

        private void bntOK_Click(object sender, EventArgs e)
        {
            Button ButtonClicked = (Button)sender;
            if (ButtonClicked.Text == Lang.lgClose)
            {
                this.Close();
            }
            else
            {
                // save changes
                SaveSettings();

                string Title = "RC [" + Path.GetFileNameWithoutExtension(Properties.Settings.Default.FileName) + "]";

                switch (SelectedSimulation)
                {
                    case SimType.VirtualNano:
                        mf.SER[CurrentProduct].CloseRCport();
                        SetPortButtons();
                        break;

                    case SimType.RealNano:
                    default:
                        SetPortButtons();
                        break;
                }

                SetButtons(false);
                Initializing = true;
                LoadSettings();
                Initializing = false;
            }
        }

        private void SaveSettings()
        {
            double tempD;
            int tempInt;
            byte tempB;

            mf.RateCals[CurrentProduct].QuantityUnits = Convert.ToByte(VolumeUnits.SelectedIndex);
            mf.RateCals[CurrentProduct].CoverageUnits = Convert.ToByte(AreaUnits.SelectedIndex);

            double.TryParse(RateSet.Text, out tempD);
            mf.RateCals[CurrentProduct].RateSet = tempD;

            double.TryParse(FlowCal.Text, out tempD);
            mf.RateCals[CurrentProduct].FlowCal = tempD;

            double.TryParse(TankSize.Text, out tempD);
            mf.RateCals[CurrentProduct].TankSize = tempD;

            mf.RateCals[CurrentProduct].ValveType = Convert.ToByte(ValveType.SelectedIndex);

            double.TryParse(TankRemain.Text, out tempD);
            mf.RateCals[CurrentProduct].SetTankRemaining(tempD);

            int.TryParse(tbVCN.Text, out tempInt);
            mf.RateCals[CurrentProduct].VCN = tempInt;

            int.TryParse(tbSend.Text, out tempInt);
            mf.RateCals[CurrentProduct].SendTime = tempInt;

            int.TryParse(tbWait.Text, out tempInt);
            mf.RateCals[CurrentProduct].WaitTime = tempInt;

            byte.TryParse(tbMinPWM.Text, out tempB);
            mf.RateCals[CurrentProduct].MinPWM = tempB;

            mf.RateCals[CurrentProduct].SimulationType = SelectedSimulation;
            mf.RateCals[CurrentProduct].ProductName = tbProduct.Text;

            byte.TryParse(tbConID.Text, out tempB);
            mf.RateCals[CurrentProduct].ControllerID = tempB;

            mf.RateCals[CurrentProduct].SaveSettings();
        }

        private void SetPortButtons()
        {
            cboPort.SelectedIndex = cboPort.FindStringExact(mf.SER[CurrentProduct].RCportName);
            cboBaud.SelectedIndex = cboBaud.FindStringExact(mf.SER[CurrentProduct].RCportBaud.ToString());

            if (mf.SER[CurrentProduct].RCport.IsOpen)
            {
                cboBaud.Enabled = false;
                cboPort.Enabled = false;
                btnConnect.Text = Lang.lgDisconnect;
                PortIndicator.BackColor = Color.LightGreen;
            }
            else
            {
                cboBaud.Enabled = true;
                cboPort.Enabled = true;
                btnConnect.Text = Lang.lgConnect;
                PortIndicator.BackColor = Color.Red;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            LoadSettings();
            SetButtons(false);
        }

        private void btnLoadSettings_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                mf.Tls.PropertiesFile = openFileDialog1.FileName;
                for(int i=0;i<5;i++)
                {
                    mf.RateCals[i].LoadSettings();
                }
                LoadSettings();
                mf.LoadSettings();
            }
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                    mf.Tls.SaveFile(saveFileDialog1.FileName);
                }
            }
        }

        private void cboPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            mf.SER[CurrentProduct].RCportName = cboPort.Text;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if(btnConnect.Text==Lang.lgConnect)
            {
                mf.SER[CurrentProduct].OpenRCport();
            }
            else
            {
                mf.SER[CurrentProduct].CloseRCport();
            }
            SetPortButtons();
        }

        private void btnRescan_Click(object sender, EventArgs e)
        {
            LoadRCbox();
            SetPortButtons();
        }

        private void LoadRCbox()
        {
            cboPort.Items.Clear();
            foreach (String s in System.IO.Ports.SerialPort.GetPortNames()) { cboPort.Items.Add(s); }
        }

        private void btnLoadDefaults_Click(object sender, EventArgs e)
        {
            tbVCN.Text = "743";
            tbSend.Text = "200";
            tbWait.Text = "750";
            tbMinPWM.Text = "145";
        }

        private void btnResetCoverage_Click(object sender, EventArgs e)
        {
            mf.RateCals[CurrentProduct].ResetCoverage();
        }

        private void btnResetTank_Click(object sender, EventArgs e)
        {
            mf.RateCals[CurrentProduct].ResetTank();
            TankRemain.Text = mf.RateCals[CurrentProduct].CurrentTankRemaining().ToString("N0");
        }

        private void cboBaud_SelectedIndexChanged(object sender, EventArgs e)
        {
            mf.SER[CurrentProduct].RCport.BaudRate = Convert.ToInt32(cboBaud.Text);
            mf.SER[CurrentProduct].RCportBaud = Convert.ToInt32(cboBaud.Text);
        }

        private void FlowCal_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(FlowCal.Text, out tempD);
            using (var form = new FormNumeric(0, 10000, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    FlowCal.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void FlowCal_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void FlowCal_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(FlowCal.Text, out tempD);
            if (tempD < 0 || tempD > 10000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
                FlowCal.Select(0, FlowCal.Text.Length);
                mf.Tls.TimedMessageBox(Lang.lgMeterCalError, "Min 0, Max 10,000", 3000, true);
            }
        }

        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            mf.Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }

        private void groupBox2_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            mf.Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }

        private void RadioButtonChanged(object sender, EventArgs e)
        {
            int Result;
            RadioButton rb = sender as RadioButton;
            if (rb != null)
            {
                if (rb.Checked)
                {
                    int.TryParse(rb.Tag.ToString(), out Result);
                    if (SelectedSimulation != (SimType)Result) SetButtons(true);
                    SelectedSimulation = (SimType)Result;
                }
            }
        }

        private void RateSet_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(RateSet.Text, out tempD);
            using (var form = new FormNumeric(0, 500, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    RateSet.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void RateSet_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void RateSet_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(RateSet.Text, out tempD);
            if (tempD < 0 || tempD > 10000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
                RateSet.Select(0, RateSet.Text.Length);
                var ErrForm = new FormTimedMessage(Lang.lgRateSetError, "Min 0, Max 10,000");
                ErrForm.Show();
            }
        }

        private void TankRemain_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(TankRemain.Text, out tempD);
            using (var form = new FormNumeric(0, 100000, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    TankRemain.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void TankRemain_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void TankRemain_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(TankRemain.Text, out tempD);
            if (tempD < 0 || tempD > 100000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
                TankRemain.Select(0, TankRemain.Text.Length);
                var ErrForm = new FormTimedMessage(Lang.lgTankRemainError, "Min 0, Max 100,000");
                ErrForm.Show();
            }
        }

        private void TankSize_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(TankSize.Text, out tempD);
            using (var form = new FormNumeric(0, 100000, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    TankSize.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void TankSize_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void TankSize_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(TankSize.Text, out tempD);
            if (tempD < 0 || tempD > 100000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
                TankSize.Select(0, TankSize.Text.Length);
                var ErrForm = new FormTimedMessage(Lang.lgTankSizeError, "Min 0, Max 100,000");
                ErrForm.Show();
            }
        }

        private void tbMinPWM_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbMinPWM.Text, out tempD);
            using (var form = new FormNumeric(0, 255, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbMinPWM.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void tbMinPWM_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbMinPWM_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbMinPWM.Text, out tempD);
            if (tempD < 0 || tempD > 255)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
                tbMinPWM.Select(0, tbMinPWM.Text.Length);
                var ErrForm = new FormTimedMessage("PWM error.", "Min 0, Max 255");
                ErrForm.Show();
            }
        }

        private void tbSend_Enter(object sender, EventArgs e)
        {
            int tempInt;
            int.TryParse(tbSend.Text, out tempInt);
            using (var form = new FormNumeric(20, 2000, tempInt))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbSend.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void tbSend_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbSend_Validating(object sender, CancelEventArgs e)
        {
            int tempInt;
            int.TryParse(tbSend.Text, out tempInt);
            if (tempInt < 20 || tempInt > 2000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
                tbSend.Select(0, tbSend.Text.Length);
                var ErrForm = new FormTimedMessage(Lang.lgSendTimeError, "Min 20, Max 2000");
                ErrForm.Show();
            }
        }

        private void tbVCN_Enter(object sender, EventArgs e)
        {
            int tempInt;
            int.TryParse(tbVCN.Text, out tempInt);
            using (var form = new FormNumeric(0, 9999, tempInt))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbVCN.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void tbVCN_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbVCN_Validating(object sender, CancelEventArgs e)
        {
            int tempInt;
            int.TryParse(tbVCN.Text, out tempInt);
            if (tempInt < 0 || tempInt > 9999)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
                tbVCN.Select(0, tbVCN.Text.Length);
                var ErrForm = new FormTimedMessage(Lang.lgVCNError, "Min 0, Max 9999");
                ErrForm.Show();
            }
        }

        private void tbWait_Enter(object sender, EventArgs e)
        {
            int tempInt;
            int.TryParse(tbWait.Text, out tempInt);
            using (var form = new FormNumeric(20, 2000, tempInt))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbWait.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void tbWait_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbWait_Validating(object sender, CancelEventArgs e)
        {
            int tempInt;
            int.TryParse(tbWait.Text, out tempInt);
            if (tempInt < 20 || tempInt > 2000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
                tbWait.Select(0, tbWait.Text.Length);
                var ErrForm = new FormTimedMessage(Lang.lgWaitTimeError, "Min 20, Max 2000");
                ErrForm.Show();
            }
        }

        private void ValveType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void VolumeUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbConID_Enter(object sender, EventArgs e)
        {
            int tempInt;
            int.TryParse(tbConID.Text, out tempInt);
            using (var form = new FormNumeric(0, 4, tempInt))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbConID.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void tbConID_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbConID_Validating(object sender, CancelEventArgs e)
        {
            int tempInt;
            int.TryParse(tbConID.Text, out tempInt);
            if (tempInt < 0 || tempInt > 4)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
                tbConID.Select(0, tbConID.Text.Length);
                var ErrForm = new FormTimedMessage("Controller error.", "Min 0, Max 4");
                ErrForm.Show();
            }
        }

        private void btnResetQuantity_Click(object sender, EventArgs e)
        {
            mf.RateCals[CurrentProduct].ResetApplied();
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            Form frmAbout = new FormAbout(this);
            frmAbout.ShowDialog();
            SetDayMode();
        }
    }
}
