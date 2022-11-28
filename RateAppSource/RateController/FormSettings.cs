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
        private bool[] SwON = new bool[9];
        private TabPage[] tbs;

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
            btnResetCoverage.Text = Lang.lgCoverage;
            btnResetTank.Text = Lang.lgStartQuantity;
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
            ckSimulate.Text = Lang.lgSimulate;

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

            PIDs = new TextBox[] { tbPIDkp, tbPIDDeadBand, tbTimedAdjustment, tbPIDHighMax, tbPIDBrakePoint, tbPIDLowMax, tbPIDMinPWM, tbPIDki };
            for (int i = 0; i < 8; i++)
            {
                PIDs[i].Tag = i;

                PIDs[i].Enter += tbPID_Enter;
                PIDs[i].TextChanged += tbPID_TextChanged;
                PIDs[i].Validating += tbPID_Validating;
            }

            mf.SwitchBox.SwitchPGNreceived += SwitchBox_SwitchPGNreceived;
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

                    // send to modules
                    mf.Products.UpdatePID();
                }
                else
                {
                    mf.Tls.ShowHelp("Module ID / Sensor ID pair must be unique.", "Help", 3000);
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

        private void btnCalCopy_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Copies the calculated sensor counts/unit to the Rate page " +
                "for the product.";

            mf.Tls.ShowHelp(Message, "Copy");
            hlpevent.Handled = true;
        }

        private void btnCalStart_Click(object sender, EventArgs e)
        {
            mf.DoCal = true;
            mf.CalCounterStart = mf.Products.Item(CurrentProduct).QuantityApplied();
            SetCalButtons();
        }

        private void btnCalStart_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Resets or Starts the sensor count for the calibration test.";

            mf.Tls.ShowHelp(Message, "Reset/Start");
            hlpevent.Handled = true;
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

        private void btnPIDloadDefaults_Click(object sender, EventArgs e)
        {
            LoadDefaults();
        }

        private void btnResetCoverage_Click(object sender, EventArgs e)
        {
            mf.Products.Item(CurrentProduct).ResetCoverage();
        }

        private void btnResetCoverage_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Reset coverage to 0.";

            mf.Tls.ShowHelp(Message, "Coverage", 10000);
            hlpevent.Handled = true;
        }

        private void btnResetQuantity_Click(object sender, EventArgs e)
        {
            mf.Products.Item(CurrentProduct).ResetApplied();
        }

        private void btnResetQuantity_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Reset quantity applied to 0.";

            mf.Tls.ShowHelp(Message, "Quantity", 10000);
            hlpevent.Handled = true;
        }

        private void btnResetTank_Click(object sender, EventArgs e)
        {
            clsProduct Prd = mf.Products.Item(CurrentProduct);

            Prd.ResetTank();
            TankRemain.Text = Prd.CurrentTankRemaining().ToString("N0");
        }

        private void btnResetTank_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Reset starting quantity to tank size.";

            mf.Tls.ShowHelp(Message, "Tank", 10000);
            hlpevent.Handled = true;
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            if (CurrentProduct < 4)
            {
                CurrentProduct++;
                UpdateForm();
            }
        }

        private double CalCounts()
        {
            clsProduct Prd = mf.Products.Item(CurrentProduct);

            double Result = 0;
            if (Prd.FlowCal > 0)
            {
                Result = (mf.CalCounterEnd - mf.CalCounterStart) * Prd.FlowCal;
            }
            return Result;
        }

        private void cbVR_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtons(true);
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

                        mf.Tls.ShowHelp("Module set to " + i.ToString() + "\n Module/Sensor pair must be unique. Change as necessary.", "Help", 3000);

                        break;
                    }
                }
            }
            return Unique;
        }

        private void ckOffRate_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);

            if (ckOffRate.Checked)
            {
                tbOffRate.Enabled = true;
            }
            else
            {
                tbOffRate.Enabled = false;
                tbOffRate.Text = "0";
            }
        }

        private void ckOffRate_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "The % rate error where an alarm sounds.";

            mf.Tls.ShowHelp(Message, "Off-rate Alarm");
            hlpevent.Handled = true;
        }

        private void ckSimulate_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
            if (ckSimulate.Checked)
            {
                SelectedSimulation = SimType.VirtualNano;
            }
            else
            {
                SelectedSimulation = SimType.None;
            }
        }

        private void ckSimulate_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Simulate flow rate without needing an attached arduino module. It will " +
                "simulate a varying random rate 4% above or below the target rate. Uncheck for " +
                "normal operation.";

            mf.Tls.ShowHelp(Message, "Simulate Rate");
            hlpevent.Handled = true;
        }

        private void ckTimedResponse_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);

            if (ckTimedResponse.Checked)
            {
                tbTimedAdjustment.Enabled = true;
            }
            else
            {
                tbTimedAdjustment.Enabled = false;
                tbTimedAdjustment.Text = "0";
            }
        }

        private void ckTimedResponse_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Adjusts the valve for a selected number of milliseconds, then pauses adjustment" +
                " for 3 X the selected milliseconds. This allows the rate to settle before making further adjustments.";

            mf.Tls.ShowHelp(Message, "Timed Adjustment");
            hlpevent.Handled = true;
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

        private void grpSections_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            mf.Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void label2_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "How fast the valve/motor responds to rate changes.";

            mf.Tls.ShowHelp(Message, "Response Rate");
            hlpevent.Handled = true;
        }

        private void label22_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Used to test if Switchbox switches are working.";

            mf.Tls.ShowHelp(Message, "Switches");
            hlpevent.Handled = true;
        }

        private void label24_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Enter the counts per revolution so RPM can be calculated.";

            mf.Tls.ShowHelp(Message, "Counts/Rev");
            hlpevent.Handled = true;
        }

        private void label25_Click(object sender, EventArgs e)
        {
        }

        private void label25_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Minimum flow rate for acceptable application.";

            mf.Tls.ShowHelp(Message, "Minimum UPM");
            hlpevent.Handled = true;
        }

        private void label26_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "The unique flow sensor ID within each arduino module.";

            mf.Tls.ShowHelp(Message, "Sensor ID");
            hlpevent.Handled = true;
        }

        private void label27_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Not implemented.";

            mf.Tls.ShowHelp(Message, "Variable Rate");
            hlpevent.Handled = true;
        }

        private void label28_Click(object sender, EventArgs e)
        {
        }

        private void label3_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "The minimum power sent to the valve/motor. The power needed to start to make the" +
                " valve/motor move.";

            mf.Tls.ShowHelp(Message, "PWM Minimum");
            hlpevent.Handled = true;
        }

        private void label4_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "The maximum power sent to the valve/motor when near the target rate. ";

            mf.Tls.ShowHelp(Message, "PWM Low Max");
            hlpevent.Handled = true;
        }

        private void label5_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "The percent error below which a lower maximum power is used. It is used to slowly adjust the " +
                "rate as it nears the target rate.";

            mf.Tls.ShowHelp(Message, "Error Brake Point");
            hlpevent.Handled = true;
        }

        private void label6_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "The allowable amount of error where no adjustment is made.";

            mf.Tls.ShowHelp(Message, "Deadband");
            hlpevent.Handled = true;
        }

        private void label7_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "The maximum power sent to the valve/motor when far from the target rate.";

            mf.Tls.ShowHelp(Message, "PWM High Max");
            hlpevent.Handled = true;
        }

        private void lb4_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Sensors counts for 1 unit of application.";

            mf.Tls.ShowHelp(Message, "Sensor Counts");
            hlpevent.Handled = true;
        }

        private void lb5_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "1 - Standard, use a valve to vary rate \n " +
                "2 - Combo Close, use a valve to vary rate and on/off \n" +
                "3 - Motor, vary motor speed to control rate.";

            mf.Tls.ShowHelp(Message, "Control Type");
            hlpevent.Handled = true;
        }

        private void lbConID_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "The unique ID of each arduino module.";

            mf.Tls.ShowHelp(Message, "Module ID");
            hlpevent.Handled = true;
        }

        private void lbWidth_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "The width of the implement that is applying product.";

            mf.Tls.ShowHelp(Message, "Working Width");
            hlpevent.Handled = true;
        }

        private void LoadDefaults()
        {
            tbPIDkp.Text = "30";
            tbPIDki.Text = "1";
            tbPIDDeadBand.Text = "3";
            tbPIDHighMax.Text = "60";
            tbPIDBrakePoint.Text = "30";
            tbPIDLowMax.Text = "30";
            tbPIDMinPWM.Text = "10";
            tbTimedAdjustment.Text = "0";
            ckTimedResponse.Checked = false;
        }

        private void LoadSettings()
        {
            byte tempB;
            clsProduct Prd = mf.Products.Item(CurrentProduct);

            tbProduct.Text = Prd.ProductName;
            VolumeUnits.SelectedIndex = Prd.QuantityUnits;
            AreaUnits.SelectedIndex = Prd.CoverageUnits;
            RateSet.Text = Prd.RateSet.ToString("N1");
            tbAltRate.Text = Prd.RateAlt.ToString("N0");
            FlowCal.Text = Prd.FlowCal.ToString("N1");
            TankSize.Text = Prd.TankSize.ToString("N0");
            ValveType.SelectedIndex = Prd.ValveType;
            cbVR.SelectedIndex = Prd.VariableRate;

            TankRemain.Text = Prd.CurrentTankRemaining().ToString("N0");

            tbCountsRev.Text = (Prd.CountsRev.ToString("N0"));

            string tmp = Prd.ModuleID.ToString();
            if (tmp == "99") tmp = "";
            tbConID.Text = tmp;

            SelectedSimulation = Prd.SimulationType;
            ckSimulate.Checked = (SelectedSimulation == SimType.VirtualNano);

            // PID
            tbPIDkp.Text = Prd.PIDkp.ToString("N0");
            tbPIDki.Text = Prd.PIDki.ToString("N0");
            tbPIDMinPWM.Text = Prd.PIDminPWM.ToString("N0");
            tbPIDLowMax.Text = Prd.PIDLowMax.ToString("N0");
            tbPIDHighMax.Text = Prd.PIDHighMax.ToString("N0");
            tbPIDDeadBand.Text = Prd.PIDdeadband.ToString("N0");
            tbPIDBrakePoint.Text = Prd.PIDbrakepoint.ToString("N0");

            byte temp = Prd.PIDTimed;
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

            tbSenID.Text = Prd.SensorID.ToString();

            rbSinglePulse.Checked = !(Prd.UseMultiPulse);
            rbMultiPulse.Checked = (Prd.UseMultiPulse);

            // load defaults if blank
            byte.TryParse(tbPIDkp.Text, out tempB);
            if (tempB == 0)
            {
                LoadDefaults();
                SaveSettings();
            }

            tbMinUPM.Text = Prd.MinUPM.ToString("N1");
            ckOffRate.Checked = Prd.UseOffRateAlarm;
            tbOffRate.Text = Prd.OffRateSetting.ToString("N0");
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

        private void rbMultiPulse_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Use the average time of multiple pulses to measure flow rate " +
                "when each flow sensor pulse takes less than 50 milliseconds.";

            mf.Tls.ShowHelp(Message, "Rate Method");
            hlpevent.Handled = true;
        }

        private void rbSinglePulse_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void rbSinglePulse_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Use the time for one pulse to measure flow rate when" +
                " each flow sensor pulse takes more than 50 milliseconds.";

            mf.Tls.ShowHelp(Message, "Rate Method");
            hlpevent.Handled = true;
        }

        private double RunningCounts()
        {
            double Result = 0;
            clsProduct Prd = mf.Products.Item(CurrentProduct);

            if (Prd.FlowCal > 0)
            {
                Result = (Prd.QuantityApplied() - mf.CalCounterStart) * Prd.FlowCal;
            }
            return Result;
        }

        private void SaveSettings()
        {
            double tempD;
            int tempInt;
            byte tempB;
            clsProduct Prd = mf.Products.Item(CurrentProduct);

            Prd.QuantityUnits = Convert.ToByte(VolumeUnits.SelectedIndex);
            Prd.CoverageUnits = Convert.ToByte(AreaUnits.SelectedIndex);

            double.TryParse(RateSet.Text, out tempD);
            Prd.RateSet = tempD;

            double.TryParse(tbAltRate.Text, out tempD);
            Prd.RateAlt = tempD;

            double.TryParse(FlowCal.Text, out tempD);
            Prd.FlowCal = tempD;

            double.TryParse(TankSize.Text, out tempD);
            Prd.TankSize = tempD;

            Prd.ValveType = Convert.ToByte(ValveType.SelectedIndex);

            Prd.VariableRate = Convert.ToByte(cbVR.SelectedIndex);

            double.TryParse(TankRemain.Text, out tempD);
            Prd.SetTankRemaining(tempD);

            Prd.SimulationType = SelectedSimulation;
            Prd.ProductName = tbProduct.Text;

            byte.TryParse(tbConID.Text, out tempB);
            Prd.ModuleID = tempB;

            // PID
            byte.TryParse(tbPIDkp.Text, out tempB);
            Prd.PIDkp = tempB;

            byte.TryParse(tbPIDki.Text, out tempB);
            Prd.PIDki = tempB;

            byte.TryParse(tbPIDMinPWM.Text, out tempB);
            Prd.PIDminPWM = tempB;

            byte.TryParse(tbPIDLowMax.Text, out tempB);
            Prd.PIDLowMax = tempB;

            byte.TryParse(tbPIDHighMax.Text, out tempB);
            Prd.PIDHighMax = tempB;

            byte.TryParse(tbPIDDeadBand.Text, out tempB);
            Prd.PIDdeadband = tempB;

            byte.TryParse(tbPIDBrakePoint.Text, out tempB);
            Prd.PIDbrakepoint = tempB;

            byte.TryParse(tbTimedAdjustment.Text, out tempB);
            Prd.PIDTimed = tempB;

            int.TryParse(tbCountsRev.Text, out tempInt);
            Prd.CountsRev = tempInt;

            byte.TryParse(tbSenID.Text, out tempB);
            Prd.SensorID = tempB;

            Prd.UseMultiPulse = (rbMultiPulse.Checked);

            double.TryParse(tbMinUPM.Text, out tempD);
            Prd.MinUPM = tempD;

            Prd.UseOffRateAlarm = ckOffRate.Checked;

            byte.TryParse(tbOffRate.Text, out tempB);
            Prd.OffRateSetting = tempB;

            Prd.Save();
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

        private void swAuto_Click(object sender, EventArgs e)
        {
            mf.SwitchBox.PressSwitch(SwIDs.Auto);
        }

        private void swDown_Click(object sender, EventArgs e)
        {
            mf.SwitchBox.PressSwitch(SwIDs.RateDown);
        }

        private void swFour_Click(object sender, EventArgs e)
        {
            mf.SwitchBox.PressSwitch(SwIDs.sw3);
        }

        private void SwitchBox_SwitchPGNreceived(object sender, PGN32618.SwitchPGNargs e)
        {
            SwON = e.Switches;
            UpdateSwitches();
        }

        private void swMasterOff_Click(object sender, EventArgs e)
        {
            mf.SwitchBox.PressSwitch(SwIDs.MasterOff);
        }

        private void swMasterOn_Click(object sender, EventArgs e)
        {
            mf.SwitchBox.PressSwitch(SwIDs.MasterOn);
        }

        private void swOne_Click(object sender, EventArgs e)
        {
            mf.SwitchBox.PressSwitch(SwIDs.sw0);
        }

        private void swThree_Click(object sender, EventArgs e)
        {
            mf.SwitchBox.PressSwitch(SwIDs.sw2);
        }

        private void swTwo_Click(object sender, EventArgs e)
        {
            mf.SwitchBox.PressSwitch(SwIDs.sw1);
        }

        private void swUp_Click(object sender, EventArgs e)
        {
            mf.SwitchBox.PressSwitch(SwIDs.RateUp);
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

        private void tbAltRate_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbAltRate.Text, out tempD);
            using (var form = new FormNumeric(1, 200, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbAltRate.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void tbAltRate_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Alternate Rate as % of base rate.";

            mf.Tls.ShowHelp(Message, "Alternate Rate");
            hlpevent.Handled = true;
        }

        private void tbAltRate_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbAltRate_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(RateSet.Text, out tempD);
            if (tempD < 1 || tempD > 200)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
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

        private void tbMinUPM_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
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

                case 7:
                    max = 25;
                    min = 0;
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

                case 7:
                    max = 25;
                    min = 0;
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

        private void tbPIDki_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Integral accumulates errors to provide an offset to the" +
                " rate adjustment. Higher integral increases the offset due to past " +
                "rate errors. ";

            mf.Tls.ShowHelp(Message, "Integral");
            hlpevent.Handled = true;
        }

        private void tbProduct_TextChanged(object sender, EventArgs e)
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
            clsProduct Prd = mf.Products.Item(CurrentProduct);

            if (mf.UseInches)
            {
                lbWorkRate.Text = Lang.lgAcresHr;
            }
            else
            {
                lbWorkRate.Text = Lang.lgHectares_Hr;
            }

            double Target = Prd.TargetUPM();
            double Applied = Prd.UPMapplied();
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

            lbPWMdata.Text = Prd.PWM().ToString("N0");

            lbWidthData.Text = Prd.Width().ToString("N1");
            lbWorkRateData.Text = Prd.WorkRate().ToString("N1");

            if (mf.UseInches)
            {
                lbWidth.Text = Lang.lgWorkingWidthFT;
            }
            else
            {
                lbWidth.Text = Lang.lgWorkingWidthM;
            }

            lbSpeedData.Text = Prd.Speed().ToString("N1");
            if (mf.UseInches)
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
                if (mf.Sections.Item(i).IsON)
                //if (mf.Sections.IsSectionOn(i))
                {
                    Sec[i].Image = Properties.Resources.OnSmall;
                }
                else
                {
                    Sec[i].Image = Properties.Resources.OffSmall;
                }
            }

            // RPM
            if (Prd.CountsRev > 0)
            {
                float RPM = (float)((Prd.FlowCal * Applied) / Prd.CountsRev);
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
            clsProduct Prd = mf.Products.Item(CurrentProduct);

            lbProduct.Text = (CurrentProduct + 1).ToString() + ". " + Prd.ProductName;
            if (Prd.SimulationType != SimType.None)
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

        private void UpdateSwitches()
        {
            if (SwON[0])
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

        private void ValveType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void VolumeUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void lb2_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            // coverage
            string Message = "Area or time units. When using time units AOG is not required.";

            mf.Tls.ShowHelp(Message, "Coverage");
            hlpevent.Handled = true;
        }

        private void rbMultiPulse_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}