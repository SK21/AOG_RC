using AgOpenGPS;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RateController
{
    public partial class FormSettings : Form
    {
        public FormStart mf;
        private double CalculatedCPU;
        private int CurrentProduct;
        private bool Initializing = false;
        private double[] MaxError = new double[5];
        private TextBox[] PIDs;
        private Label[] Sec;
        private SimType SelectedSimulation;
        private TabPage[] tbs;
        bool[] SwON = new bool[9];

        public FormSettings(FormStart CallingForm, int Page)
        {
            InitializeComponent();
            #region // language

            lbProduct.Text = Lang.lgProduct;
            tc.TabPages[0].Text = Lang.lgRate;
            tc.TabPages[1].Text = Lang.lgControl;
            tc.TabPages[2].Text = Lang.lgOptions;
            tc.TabPages[3].Text = Lang.lgDiagnostics;
            tc.TabPages[4].Text = Lang.lgCalibrate;
            btnCancel.Text = Lang.lgCancel;
            bntOK.Text = Lang.lgClose;

            lb0.Text = Lang.lgProductName;
            lb5.Text = Lang.lgControlType;
            lb1.Text = Lang.lgQuantity;
            lb2.Text = Lang.lgCoverage;
            lb4.Text = Lang.lgSensorCounts;
            lb3.Text = Lang.lgBaseRate;
            lb6.Text = Lang.lgTankSize;
            lb7.Text = Lang.lgTank_Remaining;
            btnResetCoverage.Text = Lang.lgCoverage;
            btnResetTank.Text = Lang.lgTank;
            btnResetQuantity.Text = Lang.lgQuantity;

            label7.Text = Lang.lgHighMax;
            label5.Text = Lang.lgBrakePoint;
            label4.Text = Lang.lgLowMax;
            label3.Text = Lang.lgMinPWM;
            label6.Text = Lang.lgDeadband;
            btnPIDloadDefaults.Text = Lang.lgLoad_Defaults;

            grpSensor.Text = Lang.lgSensorLocation;
            lbConID.Text = Lang.lgModuleID;
            label26.Text = Lang.lgSensorID;
            grpSimulate.Text = Lang.lgSimulate;
            rbNone.Text = Lang.lgSimulationOff;
            rbVirtual.Text = Lang.lgVirtualNano;
            rbReal.Text = Lang.lgRealNano;

            lb32.Text = Lang.lgUPMTarget;
            lb33.Text = Lang.lgUPMApplied;
            label15.Text = Lang.lgUPMerror;
            label24.Text = Lang.lgCountsRev;
            label23.Text = Lang.lgRPM;
            lbSpeed.Text = Lang.lgSpeed;
            lbWidth.Text = Lang.lgWorkingWidthFT;
            lbWorkRate.Text = Lang.lgHectares_Hr;
            label1.Text = Lang.lgSection;

            label14.Text = Lang.lgSensorTotalCounts;
            label21.Text = Lang.lgQuantityMeasured;
            label16.Text = Lang.lgSensorCounts;
            btnCalStart.Text = Lang.lgResetStart;
            btnCalStop.Text = Lang.lgStop;
            btnCalCalculate.Text = Lang.lgCalculate;
            btnCalCopy.Text = Lang.lgCalCopy;

            ValveType.Items[0] = Lang.lgStandard;
            ValveType.Items[1] = Lang.lgComboClose;
            ValveType.Items[2] = Lang.lgMotor;

            AreaUnits.Items[0] = Lang.lgAcres;
            AreaUnits.Items[1] = Lang.lgHectares;
            AreaUnits.Items[2] = Lang.lgHour;
            AreaUnits.Items[3] = Lang.lgMinute;

            #endregion // language

            mf = CallingForm;
            Initializing = true;
            tbs = new TabPage[] { tbs0, tbs4, tbs6, tbs3, tbs5 };
            CurrentProduct = Page - 1;
            if (CurrentProduct < 0) CurrentProduct = 0;

            openFileDialog1.InitialDirectory = mf.Tls.SettingsDir();
            saveFileDialog1.InitialDirectory = mf.Tls.SettingsDir();

            Sec = new Label[] { sec0, sec1, sec2, sec3, sec4, sec5, sec6, sec7, sec8, sec9, sec10, sec11, sec12, sec13, sec14, sec15 };

            PIDs = new TextBox[] { tbPIDkp, tbPIDDeadBand, tbTimedAdjustment, tbPIDHighMax, tbPIDBrakePoint, tbPIDLowMax, tbPIDMinPWM };
            for (int i = 0; i < 7; i++)
            {
                PIDs[i].Tag = i;

                PIDs[i].Enter += tbPID_Enter;
                PIDs[i].TextChanged += tbPID_TextChanged;
                PIDs[i].Validating += tbPID_Validating;
            }

            mf.SwitchBox.SwitchPGNreceived += SwitchBox_SwitchPGNreceived;
        }

        private void SwitchBox_SwitchPGNreceived(object sender, PGN32618.SwitchPGNargs e)
        {
            SwON = e.Switches;
            UpdateSwitches();
        }

        private void AreaUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtons(true);
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
                if (CheckModSen())
                {
                    // save changes
                    SaveSettings();
                    mf.Sections.CheckSwitchDefinitions();

                    string Title = "RC [" + Path.GetFileNameWithoutExtension(Properties.Settings.Default.FileName) + "]";

                    switch (SelectedSimulation)
                    {
                        case SimType.VirtualNano:
                            break;

                        case SimType.RealNano:
                        default:
                            break;
                    }

                    SetButtons(false);
                    UpdateForm();
                }
                else
                {
                    mf.Tls.TimedMessageBox("Module ID / Sensor ID pair must be unique.");
                }
            }
        }

        private void btnCalCalculate_Click(object sender, EventArgs e)
        {
            CalculatedCPU = 0;
            double Measured;
            if (double.TryParse(tbCalMeasured.Text, out Measured))
            {
                if (Measured > 0)
                {
                    CalculatedCPU = CalCounts() / Measured;
                }
            }
            lbCalCPU.Text = CalculatedCPU.ToString("N1");
        }

        private void btnCalCopy_Click(object sender, EventArgs e)
        {
            FlowCal.Text = CalculatedCPU.ToString("N1");
        }

        private void btnCalStart_Click(object sender, EventArgs e)
        {
            mf.DoCal = true;
            mf.CalCounterStart = mf.Products.Item(CurrentProduct).QuantityApplied();
            SetCalButtons();
        }

        private void btnCalStop_Click(object sender, EventArgs e)
        {
            mf.DoCal = false;
            SetCalButtons();
            mf.CalCounterEnd = mf.Products.Item(CurrentProduct).QuantityApplied();
            lbCalCounts.Text = CalCounts().ToString("N0");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            if (CurrentProduct > 0)
            {
                CurrentProduct--;
                UpdateForm();
            }
        }

        private void btnLoadSettings_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                mf.Tls.PropertiesFile = openFileDialog1.FileName;
                mf.Products.Load();
                UpdateForm();
                mf.LoadSettings();
            }
        }

        private void btnPIDloadDefaults_Click(object sender, EventArgs e)
        {
            tbPIDkp.Text = "50";
            tbPIDHighMax.Text = "100";
            tbPIDBrakePoint.Text = "20";
            tbPIDLowMax.Text = "80";
            tbPIDMinPWM.Text = "30";
            tbPIDDeadBand.Text = "4";
            tbTimedAdjustment.Text = "0";
            ckTimedResponse.Checked = false;
        }

        private void btnResetCoverage_Click(object sender, EventArgs e)
        {
            mf.Products.Item(CurrentProduct).ResetCoverage();
        }

        private void btnResetQuantity_Click(object sender, EventArgs e)
        {
            mf.Products.Item(CurrentProduct).ResetApplied();
        }

        private void btnResetTank_Click(object sender, EventArgs e)
        {
            mf.Products.Item(CurrentProduct).ResetTank();
            TankRemain.Text = mf.Products.Item(CurrentProduct).CurrentTankRemaining().ToString("N0");
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            if (CurrentProduct < 4)
            {
                CurrentProduct++;
                UpdateForm();
            }
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                    mf.Tls.SaveFile(saveFileDialog1.FileName);
                    mf.LoadSettings();
                }
            }
        }

        private void btnSections_Click(object sender, EventArgs e)
        {
        }

        private double CalCounts()
        {
            double Result = 0;
            if (mf.Products.Item(CurrentProduct).FlowCal > 0)
            {
                Result = (mf.CalCounterEnd - mf.CalCounterStart) * mf.Products.Item(CurrentProduct).FlowCal;
            }
            return Result;
        }

        private bool CheckModSen()
        {
            byte ModID = 0;
            byte SenID = 0;
            byte.TryParse(tbConID.Text, out ModID);
            byte.TryParse(tbSenID.Text, out SenID);
            bool Unique = false;

            if (mf.Products.UniqueModSen(ModID, SenID, CurrentProduct))
            {
                Unique = true;
            }
            else
            {
                // set unique pair
                for (int i = 0; i < 16; i++)
                {
                    Unique = mf.Products.UniqueModSen(i, SenID, CurrentProduct);
                    if (Unique)
                    {
                        Initializing = true;
                        tbConID.Text = i.ToString();
                        Initializing = false;

                        mf.Tls.TimedMessageBox("Module set to " + i.ToString(), "Module/Sensor pair must be unique. Change as necessary.");

                        break;
                    }
                }
            }
            return Unique;
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

        private void FormSettings_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            timer1.Enabled = true;

            UpdateForm();
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

        private void groupBox3_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            mf.Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }

        private void groupBox4_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            mf.Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }

        private void grpSections_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            mf.Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }

        private void LoadSettings()
        {
            byte tempB;

            tbProduct.Text = mf.Products.Item(CurrentProduct).ProductName;
            VolumeUnits.SelectedIndex = mf.Products.Item(CurrentProduct).QuantityUnits;
            AreaUnits.SelectedIndex = mf.Products.Item(CurrentProduct).CoverageUnits;
            RateSet.Text = mf.Products.Item(CurrentProduct).RateSet.ToString("N1");
            FlowCal.Text = mf.Products.Item(CurrentProduct).FlowCal.ToString("N1");
            TankSize.Text = mf.Products.Item(CurrentProduct).TankSize.ToString("N0");
            ValveType.SelectedIndex = mf.Products.Item(CurrentProduct).ValveType;
            cbVR.SelectedIndex = mf.Products.Item(CurrentProduct).VariableRate;

            TankRemain.Text = mf.Products.Item(CurrentProduct).CurrentTankRemaining().ToString("N0");

            tbCountsRev.Text = (mf.Products.Item(CurrentProduct).CountsRev.ToString("N0"));

            string tmp = mf.Products.Item(CurrentProduct).ModuleID.ToString();
            if (tmp == "99") tmp = "";
            tbConID.Text = tmp;

            SelectedSimulation = mf.Products.Item(CurrentProduct).SimulationType;
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

            // PID
            tbPIDkp.Text = mf.Products.Item(CurrentProduct).PIDkp.ToString("N0");
            tbPIDMinPWM.Text = mf.Products.Item(CurrentProduct).PIDminPWM.ToString("N0");
            tbPIDLowMax.Text = mf.Products.Item(CurrentProduct).PIDLowMax.ToString("N0");

            tbPIDHighMax.Text = mf.Products.Item(CurrentProduct).PIDHighMax.ToString("N0");
            tbPIDDeadBand.Text = mf.Products.Item(CurrentProduct).PIDdeadband.ToString("N0");
            tbPIDBrakePoint.Text = mf.Products.Item(CurrentProduct).PIDbrakepoint.ToString("N0");

            byte temp = mf.Products.Item(CurrentProduct).PIDTimed;
            if (temp == 0)
            {
                ckTimedResponse.Checked = false;
                tbTimedAdjustment.Enabled = false;
                tbTimedAdjustment.Text = "0";
            }
            else
            {
                ckTimedResponse.Checked = true;
                tbTimedAdjustment.Enabled = true;
                tbTimedAdjustment.Text = temp.ToString("N0");
            }

            tbSenID.Text = mf.Products.Item(CurrentProduct).SensorID.ToString();

            rbSinglePulse.Checked = !(mf.Products.Item(CurrentProduct).UseMultiPulse);
            rbMultiPulse.Checked= (mf.Products.Item(CurrentProduct).UseMultiPulse);

            // load defaults if blank
            byte.TryParse(tbPIDkp.Text, out tempB);
            if (tempB == 0)
            {
                tbPIDkp.Text = "50";
                tbPIDHighMax.Text = "100";
                tbPIDBrakePoint.Text = "20";
                tbPIDLowMax.Text = "80";
                tbPIDMinPWM.Text = "30";
                tbPIDDeadBand.Text = "4";
                tbTimedAdjustment.Text = "0";
                ckTimedResponse.Checked = false;

                SaveSettings();
            }

            tbMinUPM.Text = mf.Products.Item(CurrentProduct).MinUPM.ToString("N1");
            ckOffRate.Checked = mf.Products.Item(CurrentProduct).UseOffRateAlarm;
            tbOffRate.Text = mf.Products.Item(CurrentProduct).OffRateSetting.ToString("N0");
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
            using (var form = new FormNumeric(0, 50000, tempD))
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
            if (tempD < 0 || tempD > 50000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void rbPID_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void rbVCN_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private double RunningCounts()
        {
            double Result = 0;
            if (mf.Products.Item(CurrentProduct).FlowCal > 0)
            {
                Result = (mf.Products.Item(CurrentProduct).QuantityApplied() - mf.CalCounterStart) * mf.Products.Item(CurrentProduct).FlowCal;
            }
            return Result;
        }

        private void SaveSettings()
        {
            double tempD;
            int tempInt;
            byte tempB;

            mf.Products.Item(CurrentProduct).QuantityUnits = Convert.ToByte(VolumeUnits.SelectedIndex);
            mf.Products.Item(CurrentProduct).CoverageUnits = Convert.ToByte(AreaUnits.SelectedIndex);

            double.TryParse(RateSet.Text, out tempD);
            mf.Products.Item(CurrentProduct).RateSet = tempD;

            double.TryParse(FlowCal.Text, out tempD);
            mf.Products.Item(CurrentProduct).FlowCal = tempD;

            double.TryParse(TankSize.Text, out tempD);
            mf.Products.Item(CurrentProduct).TankSize = tempD;

            mf.Products.Item(CurrentProduct).ValveType = Convert.ToByte(ValveType.SelectedIndex);

            mf.Products.Item(CurrentProduct).VariableRate = Convert.ToByte(cbVR.SelectedIndex);

            double.TryParse(TankRemain.Text, out tempD);
            mf.Products.Item(CurrentProduct).SetTankRemaining(tempD);

            mf.Products.Item(CurrentProduct).SimulationType = SelectedSimulation;
            mf.Products.Item(CurrentProduct).ProductName = tbProduct.Text;

            byte.TryParse(tbConID.Text, out tempB);
            mf.Products.Item(CurrentProduct).ModuleID = tempB;

            // PID
            byte.TryParse(tbPIDkp.Text, out tempB);
            mf.Products.Item(CurrentProduct).PIDkp = tempB;

            byte.TryParse(tbPIDMinPWM.Text, out tempB);
            mf.Products.Item(CurrentProduct).PIDminPWM = tempB;

            byte.TryParse(tbPIDLowMax.Text, out tempB);
            mf.Products.Item(CurrentProduct).PIDLowMax = tempB;

            byte.TryParse(tbPIDHighMax.Text, out tempB);
            mf.Products.Item(CurrentProduct).PIDHighMax = tempB;

            byte.TryParse(tbPIDDeadBand.Text, out tempB);
            mf.Products.Item(CurrentProduct).PIDdeadband = tempB;

            byte.TryParse(tbPIDBrakePoint.Text, out tempB);
            mf.Products.Item(CurrentProduct).PIDbrakepoint = tempB;

            byte.TryParse(tbTimedAdjustment.Text, out tempB);
            mf.Products.Item(CurrentProduct).PIDTimed = tempB;

            int.TryParse(tbCountsRev.Text, out tempInt);
            mf.Products.Item(CurrentProduct).CountsRev = tempInt;

            byte.TryParse(tbSenID.Text, out tempB);
            mf.Products.Item(CurrentProduct).SensorID = tempB;

            mf.Products.Item(CurrentProduct).UseMultiPulse = (rbMultiPulse.Checked);

            double.TryParse(tbMinUPM.Text, out tempD);
            mf.Products.Item(CurrentProduct).MinUPM = tempD;

            mf.Products.Item(CurrentProduct).UseOffRateAlarm = ckOffRate.Checked;

            byte.TryParse(tbOffRate.Text, out tempB);
            mf.Products.Item(CurrentProduct).OffRateSetting = tempB;

            mf.Products.Item(CurrentProduct).Save();
            }

            private void SetButtons(bool Edited)
        {
            if (!Initializing)
            {
                if (Edited)
                {
                    btnCancel.Enabled = true;
                    this.bntOK.Text = Lang.lgSave;
                    btnLeft.Enabled = false;
                    btnRight.Enabled = false;
                }
                else
                {
                    btnCancel.Enabled = false;
                    this.bntOK.Text = Lang.lgClose;
                    btnLeft.Enabled = true;
                    btnRight.Enabled = true;
                }
            }
        }

        private void SetCalButtons()
        {
            btnCalStart.Enabled = !mf.DoCal;
            btnCalStop.Enabled = mf.DoCal;
            btnCalCalculate.Enabled = !mf.DoCal;
            btnCalCopy.Enabled = !mf.DoCal;
        }

        private void SetDayMode()
        {
            if (Properties.Settings.Default.IsDay)
            {
                this.BackColor = Properties.Settings.Default.DayColour;

                for (int i = 0; i < 5; i++)
                {
                    tbs[i].BackColor = Properties.Settings.Default.DayColour;
                }

                ModuleIndicator.BackColor = Properties.Settings.Default.DayColour;
                lbProduct.BackColor = Properties.Settings.Default.DayColour;

                foreach (Control c in this.Controls)
                {
                    c.ForeColor = Color.Black;
                }

                for (int i = 0; i < 16; i++)
                {
                    Sec[i].BackColor = Properties.Settings.Default.DayColour;
                }
            }
            else
            {
                this.BackColor = Properties.Settings.Default.NightColour;

                for (int i = 0; i < 5; i++)
                {
                    tbs[i].BackColor = Properties.Settings.Default.NightColour;
                }

                foreach (Control c in this.Controls)
                {
                    c.ForeColor = Color.White;
                }

                for (int i = 0; i < 16; i++)
                {
                    Sec[i].BackColor = Properties.Settings.Default.NightColour;
                }
            }

            // fix backcolor
            this.BackColor = Properties.Settings.Default.DayColour;
            for (int i = 0; i < tc.TabCount; i++)
            {
                tc.TabPages[i].BackColor = Properties.Settings.Default.DayColour;
            }
        }

        private void SetModuleIndicator()
        {
            if (mf.Products.Item(CurrentProduct).ArduinoModule.Connected())
            {
                ModuleIndicator.Image = Properties.Resources.On;
            }
            else
            {
                ModuleIndicator.Image = Properties.Resources.Off;
            }
        }

        private void SetVCNpid()
        {
            switch (ValveType.SelectedIndex)
            {
                case 2:
                    // motor
                    tbPIDBrakePoint.Enabled = false;
                    tbPIDLowMax.Enabled = false;
                    tbPIDBrakePoint.Text = "0";
                    tbPIDLowMax.Text = "0";
                    ckTimedResponse.Enabled = false;
                    ckTimedResponse.Checked = false;
                    break;

                default:
                    // 0 standard valve, 1 fast close valve
                    tbPIDBrakePoint.Enabled = true;
                    tbPIDLowMax.Enabled = true;
                    ckTimedResponse.Enabled = true;
                    break;
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
            }
        }

        private void tbConID_Enter(object sender, EventArgs e)
        {
            int tempInt;
            int.TryParse(tbConID.Text, out tempInt);
            using (var form = new FormNumeric(0, 15, tempInt))
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
            if (tempInt < 0 || tempInt > 15)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbCountsRev_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbCountsRev_Validating(object sender, CancelEventArgs e)
        {
            int Tmp;
            int.TryParse(tbCountsRev.Text, out Tmp);
            if (Tmp < 0 || Tmp > 10000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbPID_Enter(object sender, EventArgs e)
        {
            int index = (int)((TextBox)sender).Tag;
            int tmp;
            int max;
            int min;

            switch (index)
            {
                case 1:
                    max = 10;
                    min = 1;
                    break;

                case 2:
                    max = 255;
                    min = 50;
                    break;

                default:
                    max = 100;
                    min = 1;
                    break;
            }

            int.TryParse(PIDs[index].Text, out tmp);
            using (var form = new FormNumeric(min, max, tmp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    PIDs[index].Text = form.ReturnValue.ToString();
                }
            }
        }

        private void tbPID_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbPID_Validating(object sender, CancelEventArgs e)
        {
            int index = (int)((TextBox)sender).Tag;
            int tmp;
            int max;
            int min;

            switch (index)
            {
                case 1:
                    max = 10;
                    min = 1;
                    break;

                case 2:
                    max = 255;
                    min = 50;
                    break;

                default:
                    max = 100;
                    min = 1;
                    break;
            }

            int.TryParse(PIDs[index].Text, out tmp);
            if (tmp < min || tmp > max)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbProduct_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbSectionCount_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbSenID_Enter(object sender, EventArgs e)
        {
            int tempInt;
            int.TryParse(tbSenID.Text, out tempInt);
            using (var form = new FormNumeric(0, 15, tempInt))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbSenID.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void tbSenID_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbSenID_Validating(object sender, CancelEventArgs e)
        {
            int tempInt;
            int.TryParse(tbSenID.Text, out tempInt);
            if (tempInt < 0 || tempInt > 15)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateDiags();
            if (mf.DoCal) lbCalCounts.Text = RunningCounts().ToString("N0");
            SetModuleIndicator();
        }

        private void UpdateDiags()
        {
            if (mf.Products.Item(CurrentProduct).CoverageUnits == 0)
            {
                lbWorkRate.Text = Lang.lgAcresHr;
            }
            else
            {
                lbWorkRate.Text = Lang.lgHectares_Hr;
            }

            double Target = mf.Products.Item(CurrentProduct).TargetUPM();
            double Applied = mf.Products.Item(CurrentProduct).UPMapplied();
            double RateError = 0;

            lbRateSetData.Text = Target.ToString("N1");
            lbRateAppliedData.Text = Applied.ToString("N1");

            if (Target > 0)
            {
                RateError = ((Applied - Target) / Target) * 100;
                bool IsNegative = RateError < 0;
                RateError = Math.Abs(RateError);
                if (RateError > 100) RateError = 100;
                if (IsNegative) RateError *= -1;
            }
            lbErrorPercent.Text = RateError.ToString("N1");

            lbPWMdata.Text = mf.Products.Item(CurrentProduct).PWM().ToString("N0");

            lbWidthData.Text = mf.Products.Item(CurrentProduct).Width().ToString("N1");
            lbWorkRateData.Text = mf.Products.Item(CurrentProduct).WorkRate().ToString("N1");

            if (mf.Products.Item(CurrentProduct).CoverageUnits == 0)
            {
                lbWidth.Text = Lang.lgWorkingWidthFT;
            }
            else
            {
                lbWidth.Text = Lang.lgWorkingWidthM;
            }

            lbSpeedData.Text = mf.Products.Item(CurrentProduct).Speed().ToString("N1");
            if (mf.Products.Item(CurrentProduct).CoverageUnits == 0)
            {
                lbSpeed.Text = Lang.lgMPH;
            }
            else
            {
                lbSpeed.Text = Lang.lgKPH;
            }

            // update sections
            for (int i = 0; i < 16; i++)
            {
                Sec[i].Enabled = (mf.Sections.Item(i).Enabled);
                if (mf.Sections.IsSectionOn(i))
                {
                    Sec[i].Image = Properties.Resources.OnSmall;
                }
                else
                {
                    Sec[i].Image = Properties.Resources.OffSmall;
                }
            }

            // RPM
            if (mf.Products.Item(CurrentProduct).CountsRev > 0)
            {
                float RPM = (float)((mf.Products.Item(CurrentProduct).FlowCal * Applied) / mf.Products.Item(CurrentProduct).CountsRev);
                lbRPM.Text = RPM.ToString("N0");
            }
            else
            {
                lbRPM.Text = "0";
            }
        }

        private void UpdateForm()
        {
            Initializing = true;

            UpdateDiags();
            LoadSettings();
            SetVCNpid();
            SetModuleIndicator();
            SetCalButtons();
            SetDayMode();

            lbProduct.Text = (CurrentProduct + 1).ToString() + ". " + mf.Products.Item(CurrentProduct).ProductName;
            if (mf.Products.Item(CurrentProduct).SimulationType != SimType.None)
            {
                lbProduct.Text = lbProduct.Text + "   Simulation";
                //lbProduct.ForeColor = mf.SimColor;
                lbProduct.BackColor = mf.SimColor;
                lbProduct.BorderStyle = BorderStyle.FixedSingle;
            }
            else
            {
                lbProduct.ForeColor = SystemColors.ControlText;
                lbProduct.BackColor = Properties.Settings.Default.DayColour;
                lbProduct.BorderStyle = BorderStyle.None;
            }

            Initializing = false;
            UpdateSwitches();
        }

        private void ValveType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void VolumeUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbCountsRev_Click(object sender, EventArgs e)
        {
            int tempInt;
            int.TryParse(tbCountsRev.Text, out tempInt);
            using (var form = new FormNumeric(0, 10000, tempInt))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbCountsRev.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void tbCalMeasured_Click(object sender, EventArgs e)
        {
            int tempInt;
            int.TryParse(tbCalMeasured.Text, out tempInt);
            using (var form = new FormNumeric(0, 10000, tempInt))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbCalMeasured.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void UpdateSwitches()
        {
            if(SwON[0])
            {
                swAuto.BackColor = Color.LightGreen;
            }
            else
            {
                swAuto.BackColor = Color.Red;
            }

            if (SwON[1])
            {
                swMasterOn.BackColor = Color.LightGreen;
            }
            else
            {
                swMasterOn.BackColor = tbs3.BackColor;
            }

            if (SwON[2])
            {
                swMasterOff.BackColor = Color.LightGreen;
            }
            else
            {
                swMasterOff.BackColor = tbs3.BackColor;
            }

            if (SwON[3])
            {
                swUp.BackColor = Color.LightGreen;
            }
            else
            {
                swUp.BackColor = tbs3.BackColor;
            }

            if (SwON[4])
            {
                swDown.BackColor = Color.LightGreen;
            }
            else
            {
                swDown.BackColor = tbs3.BackColor;
            }


            if (SwON[5])
            {
                swOne.BackColor = Color.LightGreen;
            }
            else
            {
                swOne.BackColor = Color.Red;
            }


            if (SwON[6])
            {
                swTwo.BackColor = Color.LightGreen;
            }
            else
            {
                swTwo.BackColor = Color.Red;
            }

            if (SwON[7])
            {
                swThree.BackColor = Color.LightGreen;
            }
            else
            {
                swThree.BackColor = Color.Red;
            }

            if (SwON[8])
            {
                swFour.BackColor = Color.LightGreen;
            }
            else
            {
                swFour.BackColor = Color.Red;
            }
        }

        private void rbSinglePulse_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbs4_Click(object sender, EventArgs e)
        {
        }

        private void tbs4_Enter(object sender, EventArgs e)
        {

        }

        private void tbPIDkp_TextChanged(object sender, EventArgs e)
        {

        }

        private void ckOffRate_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);

            if(ckOffRate.Checked)
            {
                tbOffRate.Enabled = true;
            }
            else
            {
                tbOffRate.Enabled = false;
                tbOffRate.Text = "0";
            }
        }

        private void tbMinUPM_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void label25_Click(object sender, EventArgs e)
        {

        }

        private void tbMinUPM_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbMinUPM.Text, out tempD);
            using (var form = new FormNumeric(0, 500, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbMinUPM.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void cbVR_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void ckTimedResponse_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);

            if(ckTimedResponse.Checked)
            {
                tbTimedAdjustment.Enabled = true;
            }
            else
            {
                tbTimedAdjustment.Enabled = false;
                tbTimedAdjustment.Text = "0";
            }
        }

        private void tbOffRate_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbOffRate.Text, out tempD);
            using (var form = new FormNumeric(0, 40, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbOffRate.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void tbOffRate_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbOffRate_Validating(object sender, CancelEventArgs e)
        {
            int tempInt;
            int.TryParse(tbOffRate.Text, out tempInt);
            if (tempInt < 0 || tempInt > 40)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void label33_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void tbs3_Click(object sender, EventArgs e)
        {

        }

        private void label44_Click(object sender, EventArgs e)
        {

        }

        private void swDown_Click(object sender, EventArgs e)
        {

        }

        private void lbWorkRateData_Click(object sender, EventArgs e)
        {

        }
    }
}