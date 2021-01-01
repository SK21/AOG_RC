using AgOpenGPS;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RateController
{
    public partial class FormRateSettings : Form
    {
        private readonly FormRateControl mf;
        private bool Initializing = false;
        private int Result;
        private SimType SelectedSimulation;
        private byte tempB = 0;
        private double tempD = 0;
        private int tempInt = 0;

        public FormRateSettings(FormRateControl CallingForm)
        {
            InitializeComponent();
            mf = CallingForm;
            Initializing = true;
            LoadSettings();
            Initializing = false;
            openFileDialog1.InitialDirectory = mf.Tls.SettingsDir();
            saveFileDialog1.InitialDirectory = mf.Tls.SettingsDir();
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

                string Title = "RC [" + Path.GetFileNameWithoutExtension(Properties.Settings.Default.FileName) + "]";

                switch (SelectedSimulation)
                {
                    case SimType.VirtualNano:
                        mf.Text = Title + " (V)";
                        mf.SER.CloseRCport();
                        SetRCbuttons();
                        break;

                    case SimType.RealNano:
                        mf.Text = Title + " (R)";
                        //mf.SER.OpenRCport();
                        SetRCbuttons();
                        break;

                    default:
                        mf.Text = Title;
                        //mf.SER.OpenRCport();
                        SetRCbuttons();
                        break;
                }

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

        private void btnCloseSerialArduino_Click(object sender, EventArgs e)
        {
            mf.SER.CloseRCport();
            SetRCbuttons();
        }

        private void btnDay_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.IsDay)
            {
                Properties.Settings.Default.IsDay = false;
            }
            else
            {
                Properties.Settings.Default.IsDay = true;
            }
            SetDayMode();
        }

        private void btnLoadSettings_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                mf.Tls.PropertiesFile = openFileDialog1.FileName;
                mf.RC.LoadSettings();
                LoadSettings();
                mf.LoadSettings();
            }
        }

        private void btnOpenSerialArduino_Click(object sender, EventArgs e)
        {
            mf.SER.OpenRCport();
            SetRCbuttons();
        }

        private void btnRescan_Click(object sender, EventArgs e)
        {
            LoadRCbox();
            SetRCbuttons();
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

        private void butLoadDefaults_Click(object sender, EventArgs e)
        {
            tbVCN.Text = "743";
            tbSend.Text = "200";
            tbWait.Text = "750";
            tbMaxPWM.Text = "255";
            tbMinPWM.Text = "145";
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

        private void cboxArdPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            mf.SER.RCportName = cboxArdPort.Text;
        }

        private void cboxBaud_SelectedIndexChanged(object sender, EventArgs e)
        {
            mf.SER.RCport.BaudRate = Convert.ToInt32(cboxBaud.Text);
            mf.SER.RCportBaud = Convert.ToInt32(cboxBaud.Text);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void FlowCal_Enter(object sender, EventArgs e)
        {
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
                mf.Tls.SaveFormData(this);
            }
            timer1.Enabled = false;
        }

        private void FormRateSettings_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);

            lbNetworkIP.Text = mf.NetworkIP;
            lbLocalIP.Text = mf.LocalIP;
            lbVersion.Text = "Version Date   " + mf.Tls.VersionDate();
            lbDestinationIP.Text = mf.Tls.LoadProperty("DestinationIP");
            LoadRCbox();
            SetRCbuttons();
            SetDayMode();
            SetBackColor();
        }

        private void GroupBoxPaint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            mf.Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }

        private void LoadRCbox()
        {
            cboxArdPort.Items.Clear();
            foreach (String s in System.IO.Ports.SerialPort.GetPortNames()) { cboxArdPort.Items.Add(s); }
        }

        private void LoadSettings()
        {
            VolumeUnits.SelectedIndex = mf.RC.QuantityUnits;
            AreaUnits.SelectedIndex = mf.RC.CoverageUnits;
            RateSet.Text = mf.RC.RateSet.ToString("N1");
            FlowCal.Text = mf.RC.FlowCal.ToString("N1");
            TankSize.Text = mf.RC.TankSize.ToString("N0");
            ValveType.SelectedIndex = mf.RC.ValveType;
            TankRemain.Text = mf.RC.CurrentTankRemaining();
            tbVCN.Text = (mf.RC.VCN).ToString("G0");
            tbSend.Text = (mf.RC.SendTime).ToString("N0");
            tbWait.Text = (mf.RC.WaitTime).ToString("N0");
            tbMaxPWM.Text = (mf.RC.MaxPWM).ToString("N0");
            tbMinPWM.Text = (mf.RC.MinPWM).ToString("N0");

            SelectedSimulation = mf.RC.SimulationType;
            switch (SelectedSimulation)
            {
                case SimType.VirtualNano:
                    rbNano.Checked = true;
                    break;

                case SimType.RealNano:
                    rbFlow.Checked = true;
                    break;

                default:
                    rbNone.Checked = true;
                    break;
            }
        }

        private void RadioButtonChanged(object sender, EventArgs e)
        {
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

            int.TryParse(tbVCN.Text, out tempInt);
            mf.RC.VCN = tempInt;

            int.TryParse(tbSend.Text, out tempInt);
            mf.RC.SendTime = tempInt;

            int.TryParse(tbWait.Text, out tempInt);
            mf.RC.WaitTime = tempInt;

            byte.TryParse(tbMaxPWM.Text, out tempB);
            mf.RC.MaxPWM = tempB;

            byte.TryParse(tbMinPWM.Text, out tempB);
            mf.RC.MinPWM = tempB;

            mf.RC.SimulationType = SelectedSimulation;

            mf.RC.SaveSettings();
        }

        private void SetBackColor()
        {
            this.BackColor = Properties.Settings.Default.DayColour;
            for (int i = 0; i < tabControl1.TabCount; i++)
            {
                tabControl1.TabPages[i].BackColor = Properties.Settings.Default.DayColour;
            }
            tbVCNdescription.BackColor = Properties.Settings.Default.DayColour;
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

        private void SetDayMode()
        {
            if (Properties.Settings.Default.IsDay)
            {
                btnDay.Image = Properties.Resources.WindowDayMode;
            }
            else
            {
                btnDay.Image = Properties.Resources.WindowNightMode;
            }
        }

        private void SetRCbuttons()
        {
            cboxArdPort.SelectedIndex = cboxArdPort.FindStringExact(mf.SER.RCportName);
            cboxBaud.SelectedIndex = cboxBaud.FindStringExact(mf.SER.RCportBaud.ToString());

            if (mf.SER.RCport.IsOpen)
            {
                cboxBaud.Enabled = false;
                cboxArdPort.Enabled = false;
                btnCloseSerialArduino.Enabled = true;
                btnOpenSerialArduino.Enabled = false;
                lbArduinoConnected.Text = mf.SER.RCportName + " Connected";
                lbArduinoConnected.BackColor = Color.LightGreen;
            }
            else
            {
                cboxBaud.Enabled = true;
                cboxArdPort.Enabled = true;
                btnCloseSerialArduino.Enabled = false;
                btnOpenSerialArduino.Enabled = true;
                lbArduinoConnected.Text = mf.SER.RCportName + " Disconnected";
                lbArduinoConnected.BackColor = Color.Red;
            }
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            timer1.Enabled = (tabControl1.SelectedIndex == 3);
        }

        private void TankRemain_Enter(object sender, EventArgs e)
        {
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

        private void TankSize_Enter(object sender, EventArgs e)
        {
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

        private void tbMaxPWM_Enter(object sender, EventArgs e)
        {
            double.TryParse(tbMaxPWM.Text, out tempD);
            using (var form = new FormNumeric(0, 255, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbMaxPWM.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void tbMaxPWM_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbMinPWM_Enter(object sender, EventArgs e)
        {
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

        private void tbSecondsAverage_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbSend_Enter(object sender, EventArgs e)
        {
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
            int.TryParse(tbSend.Text, out tempInt);
            if (tempInt < 20 || tempInt > 2000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
                tbSend.Select(0, tbSend.Text.Length);
                var ErrForm = new FormTimedMessage("Send Time Error", "Min 20, Max 2000");
                ErrForm.Show();
            }
        }

        private void tbVCN_Enter(object sender, EventArgs e)
        {
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
            int.TryParse(tbVCN.Text, out tempInt);
            if (tempInt < 0 || tempInt > 9999)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
                tbVCN.Select(0, tbVCN.Text.Length);
                var ErrForm = new FormTimedMessage("VCN Number Error", "Min 0, Max 9999");
                ErrForm.Show();
            }
        }

        private void tbWait_Enter(object sender, EventArgs e)
        {
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
            int.TryParse(tbWait.Text, out tempInt);
            if (tempInt < 20 || tempInt > 2000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
                tbWait.Select(0, tbWait.Text.Length);
                var ErrForm = new FormTimedMessage("Wait Time Error", "Min 20, Max 2000");
                ErrForm.Show();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lbWorkRateData.Text = mf.RC.WorkRate().ToString("N1");
            if (mf.RC.CoverageUnits == 0)
            {
                lbWorkRate.Text = "Acres/Hr";
            }
            else
            {
                lbWorkRate.Text = "Hectares/Hr";
            }

            lbRateSetData.Text = mf.RC.TargetUPM().ToString("N1");
            lbRateAppliedData.Text = mf.RC.UPMapplied().ToString("N1");
            lbPWMdata.Text = mf.RC.PWM().ToString("N0");

            lbWidthData.Text = mf.RC.Width().ToString("N1");
            if (mf.RC.CoverageUnits == 0)
            {
                lbWidth.Text = "Working Width (FT)";
            }
            else
            {
                lbWidth.Text = "Working Width (M)";
            }

            lbSecHiData.Text = mf.RC.SectionHi().ToString();
            lbSecLoData.Text = mf.RC.SectionLo().ToString();

            lbSpeedData.Text = mf.RC.Speed().ToString("N1");
            if (mf.RC.CoverageUnits == 0)
            {
                lbSpeed.Text = "MPH";
            }
            else
            {
                lbSpeed.Text = "KPH";
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
    }
}