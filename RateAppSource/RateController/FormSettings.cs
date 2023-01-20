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

        private const byte BrakePointDefault = 30;  // %
        private const byte DeadbandDefault = 3;     // %
        private const byte LowMaxMultiplier = 3;

        private double CalculatedCPU;
        private clsProduct CurrentProduct;
        private bool Initializing = false;
        private double[] MaxError = new double[5];
        private TextBox[] PIDs;
        private Label[] Sec;
        private SimType SelectedSimulation;
        private bool[] SwON = new bool[9];
        private TabPage[] tbs;
        private TimeSpan WtSpan;
        private DateTime[] WtStart = new DateTime[5];
        private DateTime[] WtTimeEnd = new DateTime[5];

        public FormSettings(FormStart CallingForm, int Page)
        {
            Initializing = true;
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
            lbSensorCounts.Text = Lang.lgSensorCounts;
            lb3.Text = Lang.lgBaseRate;
            lb6.Text = Lang.lgTankSize;
            btnResetCoverage.Text = Lang.lgCoverage;
            btnResetTank.Text = Lang.lgStartQuantity;
            btnResetQuantity.Text = Lang.lgQuantity;

            label7.Text = Lang.lgHighMax;
            label3.Text = Lang.lgMinPWM;
            btnPIDloadDefaults.Text = Lang.lgLoad_Defaults;

            grpSensor.Text = Lang.lgSensorLocation;
            lbConID.Text = Lang.lgModuleID;
            lbSensorID.Text = Lang.lgSensorID;
            ckSimulate.Text = Lang.lgSimulate;

            lb32.Text = Lang.lgUPMTarget;
            lb33.Text = Lang.lgUPMApplied;
            label15.Text = Lang.lgUPMerror;
            label24.Text = Lang.lgCountsRev;
            label23.Text = Lang.lgRPM;
            lbSpeed.Text = Lang.lgSpeed;
            lbWidth.Text = Lang.lgWorkingWidthFT;
            lbWorkRate.Text = Lang.lgHectares_Hr;
            lbSections.Text = Lang.lgSection;

            label14.Text = Lang.lgSensorTotalCounts;
            lbFlowMeasured.Text = Lang.lgQuantityMeasured;
            lbWeightMeasured.Text = Lang.lgQuantityMeasured;
            label16.Text = Lang.lgSensorCounts;
            btnCalStart.Text = Lang.lgResetStart;
            btnCalStop.Text = Lang.lgStop;
            btnCalCalculate.Text = Lang.lgCalculate;
            btnCalCopy.Text = Lang.lgCalCopy;

            ValveType.Items[0] = Lang.lgStandard;
            ValveType.Items[1] = Lang.lgComboClose;
            ValveType.Items[2] = Lang.lgMotor;
            ValveType.Items[3] = Lang.lgMotorWeight;

            AreaUnits.Items[0] = Lang.lgAcres;
            AreaUnits.Items[1] = Lang.lgHectares;
            AreaUnits.Items[2] = Lang.lgMinute;
            AreaUnits.Items[3] = Lang.lgHour;

            lbAltRate.Text = Lang.lgAltRate;
            lbVariableRate.Text = Lang.lgVariableRate;
            lbResponseRate.Text = Lang.lgResponerate;
            lbIntegral.Text = Lang.lgIntegralAdjustment;
            ckTimedResponse.Text = Lang.lgTimedAjustment;

            lbSensorID.Text = Lang.lgSensorID;
            grpRateMethod.Text = Lang.lgRateMethod;
            rbSinglePulse.Text = Lang.lgTimeForSingle;
            rbMultiPulse.Text = Lang.lgTimeForMulti;

            lbMinimumUPM.Text = Lang.lgMinUPM;
            ckOffRate.Text = Lang.lgOffRate;
            lbSwitches.Text = Lang.lgSwitch;

            swMasterOff.Text = Lang.lgMasterOff;
            swMasterOn.Text = Lang.lgMasterOn;
            swAuto.Text = Lang.lgAuto;
            swUp.Text = Lang.lgRateUp;
            swDown.Text = Lang.lgRateDown;

            label34.Text = Lang.lgTime;

            #endregion // language

            mf = CallingForm;
            tbs = new TabPage[] { tbs0, tbs4, tbs6, tbs3, tbs5 };
            if (Page == 0)
            {
                CurrentProduct = mf.Products.Item(0);
            }
            else
            {
                CurrentProduct = mf.Products.Item(Page - 1);
            }

            openFileDialog1.InitialDirectory = mf.Tls.SettingsDir();
            saveFileDialog1.InitialDirectory = mf.Tls.SettingsDir();

            Sec = new Label[] { sec0, sec1, sec2, sec3, sec4, sec5, sec6, sec7, sec8, sec9, sec10, sec11, sec12, sec13, sec14, sec15 };

            PIDs = new TextBox[] { tbPIDkp, tbTimedAdjustment, tbPIDHighMax, tbPIDMinPWM, tbPIDki };
            for (int i = 0; i < 5; i++)
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
                    CalculatedCPU = FlowMeterCounts() / Measured;
                }
            }
            lbCalCPU.Text = CalculatedCPU.ToString("N1");
        }

        private void btnCalCopy_Click(object sender, EventArgs e)
        {
            if (CurrentProduct.ControlType == ControlTypeEnum.MotorWeights)
            {
                FlowCal.Text = lbWTcal.Text;
            }
            else
            {
                FlowCal.Text = CalculatedCPU.ToString("N3");
            }
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
            byte tmp = 0;
            byte.TryParse(tbWTpwm.Text, out tmp);
            if (tmp == 0 && CurrentProduct.ControlType == ControlTypeEnum.MotorWeights)
            {
                mf.Tls.ShowHelp("PWM must be greater than 0.", "Weight Cal", 15000);
            }
            else
            {
                CurrentProduct.CalPWM = tmp;
                CurrentProduct.DoCal = true;
                CurrentProduct.CalStart = CurrentProduct.UnitsApplied();
                SetCalButtons();
                WtStart[CurrentProduct.ID] = DateTime.Now;
                lbWTcal.Text = "0.0";
                lbWTquantity.Text = "0.0";
            }
        }

        private void btnCalStart_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Resets and starts the sensor count for the calibration test.";

            mf.Tls.ShowHelp(Message, "Start");
            hlpevent.Handled = true;
        }

        private void btnCalStop_Click(object sender, EventArgs e)
        {
            CurrentProduct.CalEnd = CurrentProduct.UnitsApplied();
            CurrentProduct.DoCal = false;
            SetCalButtons();
            btnCalCopy.Focus();

            if (CurrentProduct.ControlType == ControlTypeEnum.MotorWeights)
            {
                // calibrate by weight
                WtTimeEnd[CurrentProduct.ID] = DateTime.Now;
                double tmp = CurrentProduct.CalEnd - CurrentProduct.CalStart;
                if (tmp > 0)
                {
                    lbWTquantity.Text = tmp.ToString("N1");

                    // UPM/PWM
                    WtSpan = (WtTimeEnd[CurrentProduct.ID] - WtStart[CurrentProduct.ID]);
                    double Minutes = WtSpan.TotalSeconds / 60.0;
                    double UPM = 0;
                    if (Minutes > 0)
                    {
                        UPM = tmp / Minutes;
                    }
                    double UPMperPWM = 0;
                    if (CurrentProduct.CalPWM > 0)
                    {
                        UPMperPWM = UPM / CurrentProduct.CalPWM;
                    }
                    lbWTcal.Text = UPMperPWM.ToString("N3");
                }
            }
            else
            {
                // calibrate by flow meter counts
                lbFlowMeterCounts.Text = FlowMeterCounts().ToString("N0");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            if (CurrentProduct.ID > 0)
            {
                CurrentProduct = mf.Products.Item(CurrentProduct.ID - 1);
                ShowSpan();
                UpdateForm();
            }
        }

        private void btnPIDloadDefaults_Click(object sender, EventArgs e)
        {
            LoadDefaults();
        }

        private void btnResetCoverage_Click(object sender, EventArgs e)
        {
            CurrentProduct.ResetCoverage();
        }

        private void btnResetCoverage_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Reset coverage to 0.";

            mf.Tls.ShowHelp(Message, "Coverage", 10000);
            hlpevent.Handled = true;
        }

        private void btnResetQuantity_Click(object sender, EventArgs e)
        {
            CurrentProduct.ResetApplied();
        }

        private void btnResetQuantity_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Reset quantity applied to 0.";

            mf.Tls.ShowHelp(Message, "Quantity", 10000);
            hlpevent.Handled = true;
        }

        private void btnResetTank_Click(object sender, EventArgs e)
        {
            CurrentProduct.ResetTank();
            TankRemain.Text = CurrentProduct.TankStart.ToString("N0");
        }

        private void btnResetTank_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Reset starting quantity to tank size.";

            mf.Tls.ShowHelp(Message, "Tank", 10000);
            hlpevent.Handled = true;
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            if (CurrentProduct.ID < 4)
            {
                CurrentProduct = mf.Products.Item(CurrentProduct.ID + 1);
                ShowSpan();
                UpdateForm();
            }
        }

        private void btnTare_Click(object sender, EventArgs e)
        {
            tbTare.Text = CurrentProduct.CurrentWeight().ToString("N0");
        }

        private void btnTare_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Zero out the scale.";

            mf.Tls.ShowHelp(Message, "Tare", 15000);
            hlpevent.Handled = true;
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

            if (mf.Products.UniqueModSen(ModID, SenID, CurrentProduct.ID))
            {
                Unique = true;
            }
            else
            {
                // set unique pair
                for (int i = 0; i < 16; i++)
                {
                    Unique = mf.Products.UniqueModSen(i, SenID, CurrentProduct.ID);
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
            using (var form = new FormNumeric(1, 10000, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    FlowCal.Text = form.ReturnValue.ToString("N3");
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
            if (tempD < 1 || tempD > 10000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private double FlowMeterCounts()
        {
            double Result = 0;
            if (CurrentProduct.MeterCal > 0)
            {
                Result = (CurrentProduct.CalEnd - CurrentProduct.CalStart) * CurrentProduct.MeterCal;   // convert units back to counts
            }
            return Result;
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

        private void label7_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "The maximum power sent to the valve/motor when far from the target rate.";

            mf.Tls.ShowHelp(Message, "PWM High Max");
            hlpevent.Handled = true;
        }

        private void lb2_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            // coverage
            string Message = "Area or time units. When using time units AOG is not required.";

            mf.Tls.ShowHelp(Message, "Coverage");
            hlpevent.Handled = true;
        }

        private void lb4_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "If control type is by weight this is Units per Minute for each Pulse Width Modulation value. " +
                "If control type is a flow sensor this is sensor counts for 1 unit of application.";

            mf.Tls.ShowHelp(Message, "Sensor Counts");
            hlpevent.Handled = true;
        }

        private void lb5_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "1 - Standard, use a valve to vary rate \n " +
                "2 - Combo Close, use a valve to vary rate and on/off \n" +
                "3 - Motor, vary motor speed to control rate \n" +
                "4 - Motor control using change in tank weight.";

            mf.Tls.ShowHelp(Message, "Control Type");
            hlpevent.Handled = true;
        }

        private void lbConID_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "The unique ID of each arduino module.";

            mf.Tls.ShowHelp(Message, "Module ID");
            hlpevent.Handled = true;
        }

        private void lbProduct_Click(object sender, EventArgs e)
        {
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
            tbPIDHighMax.Text = "60";
            tbPIDMinPWM.Text = "10";
            tbTimedAdjustment.Text = "0";
            ckTimedResponse.Checked = false;
        }

        private void LoadSettings()
        {
            byte tempB;
            tbProduct.Text = CurrentProduct.ProductName;
            tbVolumeUnits.Text = CurrentProduct.QuantityDescription;
            AreaUnits.SelectedIndex = CurrentProduct.CoverageUnits;
            RateSet.Text = CurrentProduct.RateSet.ToString("N1");
            tbAltRate.Text = CurrentProduct.RateAlt.ToString("N0");
            FlowCal.Text = CurrentProduct.MeterCal.ToString("N3");
            TankSize.Text = CurrentProduct.TankSize.ToString("N0");
            ValveType.SelectedIndex = (int)CurrentProduct.ControlType;
            cbVR.SelectedIndex = CurrentProduct.VariableRate;

            TankRemain.Text = CurrentProduct.TankStart.ToString("N0");

            tbCountsRev.Text = (CurrentProduct.CountsRev.ToString("N0"));

            string tmp = CurrentProduct.ModuleID.ToString();
            if (tmp == "99") tmp = "";
            tbConID.Text = tmp;

            SelectedSimulation = CurrentProduct.SimulationType;
            ckSimulate.Checked = (SelectedSimulation == SimType.VirtualNano);

            // PID
            tbPIDkp.Text = CurrentProduct.PIDkp.ToString("N0");
            tbPIDki.Text = CurrentProduct.PIDki.ToString("N0");
            tbPIDMinPWM.Text = CurrentProduct.PIDminPWM.ToString("N0");
            tbPIDHighMax.Text = CurrentProduct.PIDHighMax.ToString("N0");

            byte temp = CurrentProduct.PIDTimed;
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

            tbSenID.Text = CurrentProduct.SensorID.ToString();

            rbSinglePulse.Checked = !(CurrentProduct.UseMultiPulse);
            rbMultiPulse.Checked = (CurrentProduct.UseMultiPulse);

            // load defaults if blank
            byte.TryParse(tbPIDkp.Text, out tempB);
            if (tempB == 0)
            {
                LoadDefaults();
                SaveSettings();
            }

            tbMinUPM.Text = CurrentProduct.MinUPM.ToString("N1");
            ckOffRate.Checked = CurrentProduct.UseOffRateAlarm;
            tbOffRate.Text = CurrentProduct.OffRateSetting.ToString("N0");
            tbWTpwm.Text = CurrentProduct.CalPWM.ToString("N0");
            tbScaleCountsPerUnit.Text = CurrentProduct.ScaleCountsPerUnit.ToString("N1");
            SetCalButtons();
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

        private void rbMultiPulse_CheckedChanged(object sender, EventArgs e)
        {
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
            if (CurrentProduct.MeterCal > 0)
            {
                Result = (CurrentProduct.UnitsApplied() - CurrentProduct.CalStart) * CurrentProduct.MeterCal;
            }
            return Result;
        }

        private void SaveSettings()
        {
            double tempD;
            int tempInt;
            byte tempB;

            CurrentProduct.CoverageUnits = Convert.ToByte(AreaUnits.SelectedIndex);
            CurrentProduct.QuantityDescription = tbVolumeUnits.Text;

            double.TryParse(RateSet.Text, out tempD);
            CurrentProduct.RateSet = tempD;

            double.TryParse(tbAltRate.Text, out tempD);
            CurrentProduct.RateAlt = tempD;

            double.TryParse(FlowCal.Text, out tempD);
            CurrentProduct.MeterCal = tempD;

            double.TryParse(TankSize.Text, out tempD);
            CurrentProduct.TankSize = tempD;

            CurrentProduct.ControlType = (ControlTypeEnum)(ValveType.SelectedIndex);

            CurrentProduct.VariableRate = Convert.ToByte(cbVR.SelectedIndex);

            double.TryParse(TankRemain.Text, out tempD);
            CurrentProduct.TankStart = tempD;

            CurrentProduct.SimulationType = SelectedSimulation;
            CurrentProduct.ProductName = tbProduct.Text;

            byte.TryParse(tbConID.Text, out tempB);
            CurrentProduct.ModuleID = tempB;

            // PID
            byte.TryParse(tbPIDkp.Text, out tempB);
            CurrentProduct.PIDkp = tempB;

            byte.TryParse(tbPIDki.Text, out tempB);
            CurrentProduct.PIDki = tempB;

            byte.TryParse(tbPIDMinPWM.Text, out tempB);
            CurrentProduct.PIDminPWM = tempB;

            byte.TryParse(tbPIDHighMax.Text, out tempB);
            CurrentProduct.PIDHighMax = tempB;

            CurrentProduct.PIDLowMax = (byte)(CurrentProduct.PIDminPWM * LowMaxMultiplier);
            if (CurrentProduct.PIDLowMax > CurrentProduct.PIDHighMax)
            {
                CurrentProduct.PIDLowMax = CurrentProduct.PIDHighMax;
            }

            CurrentProduct.PIDdeadband = DeadbandDefault;

            CurrentProduct.PIDbrakepoint = BrakePointDefault;

            byte.TryParse(tbTimedAdjustment.Text, out tempB);
            CurrentProduct.PIDTimed = tempB;

            int.TryParse(tbCountsRev.Text, out tempInt);
            CurrentProduct.CountsRev = tempInt;

            byte.TryParse(tbSenID.Text, out tempB);
            CurrentProduct.SensorID = tempB;

            CurrentProduct.UseMultiPulse = (rbMultiPulse.Checked);

            double.TryParse(tbMinUPM.Text, out tempD);
            CurrentProduct.MinUPM = tempD;

            CurrentProduct.UseOffRateAlarm = ckOffRate.Checked;

            byte.TryParse(tbOffRate.Text, out tempB);
            CurrentProduct.OffRateSetting = tempB;

            byte.TryParse(tbWTpwm.Text, out tempB);
            CurrentProduct.CalPWM = tempB;

            double.TryParse(tbScaleCountsPerUnit.Text, out tempD);
            CurrentProduct.ScaleCountsPerUnit = tempD;

            double.TryParse(tbTare.Text, out tempD);
            CurrentProduct.ScaleTare = tempD;

            CurrentProduct.Save();
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
            tbCalMeasured.Enabled = true;
            btnCalStart.Enabled = !CurrentProduct.DoCal;
            btnCalStop.Enabled = CurrentProduct.DoCal;
            btnCalCalculate.Enabled = !CurrentProduct.DoCal;
            btnCalCopy.Enabled = !CurrentProduct.DoCal;
            tbWTpwm.Enabled = !CurrentProduct.DoCal;
            tbCalMeasured.Enabled = !CurrentProduct.DoCal;
            tbScaleCountsPerUnit.Enabled = !CurrentProduct.DoCal;
            btnTare.Enabled = !CurrentProduct.DoCal;
            tbTare.Enabled = !CurrentProduct.DoCal;
        }

        private void SetCalDescription()
        {
            if (ValveType.SelectedIndex == 3)
            {
                lbSensorCounts.Text = Lang.lgUPMPWM;
            }
            else
            {
                lbSensorCounts.Text = Lang.lgSensorCounts;
            }
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
            if (mf.Products.Item(CurrentProduct.ID).ArduinoModule.Connected())
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
                    ckTimedResponse.Enabled = false;
                    ckTimedResponse.Checked = false;
                    break;

                default:
                    // 0 standard valve, 1 fast close valve
                    ckTimedResponse.Enabled = true;
                    break;
            }
        }

        private void ShowSpan()
        {
            if (CurrentProduct.DoCal)
            {
                WtSpan = (DateTime.Now - WtStart[CurrentProduct.ID]);
            }
            else
            {
                WtSpan = (WtTimeEnd[CurrentProduct.ID] - WtStart[CurrentProduct.ID]);
            }
            lbWTtime.Text = WtSpan.Minutes.ToString("00") + ":" + WtSpan.Seconds.ToString("00");
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
            double.TryParse(tbAltRate.Text, out tempD);
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

        private void tbTare_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbTare.Text, out tempD);
            using (var form = new FormNumeric(1, 100000, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbTare.Text = form.ReturnValue.ToString("N0");
                }
            }
        }

        private void tbTare_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbTare_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbTare.Text, out tempD);
            if (tempD < 1 || tempD > 100000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbVolumeUnits_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbWTpwm_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbWTpwm.Text, out tempD);
            using (var form = new FormNumeric(1, 255, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbWTpwm.Text = form.ReturnValue.ToString("N0");
                }
            }
        }

        private void tbWTpwm_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Used to set a constant applicator speed for calibration." +
                "Run until a sufficient quantity is measured for an accurate result. ";

            mf.Tls.ShowHelp(Message, "PWM Cal", 15000);
            hlpevent.Handled = true;
        }

        private void tbWTpwm_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbWTpwm_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbWTpwm.Text, out tempD);
            if (tempD < 1 || tempD > 255)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateDiags();
            if (CurrentProduct.DoCal) lbFlowMeterCounts.Text = RunningCounts().ToString("N0");
            SetModuleIndicator();
            ShowSpan();
        }

        private void UpdateDiags()
        {
            if (mf.UseInches)
            {
                lbWorkRate.Text = Lang.lgAcresHr;
            }
            else
            {
                lbWorkRate.Text = Lang.lgHectares_Hr;
            }

            double Target = CurrentProduct.TargetUPM();
            double Applied = CurrentProduct.UPMapplied();
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

            lbPWMdata.Text = CurrentProduct.PWM().ToString("N0");

            lbWidthData.Text = CurrentProduct.Width().ToString("N1");
            lbWorkRateData.Text = CurrentProduct.WorkRate().ToString("N1");

            if (mf.UseInches)
            {
                lbWidth.Text = Lang.lgWorkingWidthFT;
            }
            else
            {
                lbWidth.Text = Lang.lgWorkingWidthM;
            }

            lbSpeedData.Text = CurrentProduct.Speed().ToString("N1");
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
            if (CurrentProduct.CountsRev > 0)
            {
                float RPM = (float)((CurrentProduct.MeterCal * Applied) / CurrentProduct.CountsRev);
                lbRPM.Text = RPM.ToString("N0");
            }
            else
            {
                lbRPM.Text = "0";
            }

            // wifi
            wifiBar.Value = CurrentProduct.WifiStrength;

            if (CurrentProduct.DoCal)
            {
                lbWTquantity.Text = (CurrentProduct.UnitsApplied() - CurrentProduct.CalStart).ToString("N1");
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
            SetCalDescription();

            lbProduct.Text = (CurrentProduct.ID + 1).ToString() + ". " + CurrentProduct.ProductName;
            if (CurrentProduct.SimulationType != SimType.None)
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

            // calibration panels
            pnlFlow.Visible = CurrentProduct.ControlType != ControlTypeEnum.MotorWeights;
            pnlWeight.Visible = CurrentProduct.ControlType == ControlTypeEnum.MotorWeights;
            lbWTcalAverage.Text = CurrentProduct.UPMperPWM().ToString("N3");

            TankSize.Enabled = CurrentProduct.ControlType != ControlTypeEnum.MotorWeights;
            TankRemain.Enabled = CurrentProduct.ControlType != ControlTypeEnum.MotorWeights;
            btnResetTank.Enabled = CurrentProduct.ControlType != ControlTypeEnum.MotorWeights;

            tbTare.Text = CurrentProduct.ScaleTare.ToString("N0");

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
            SetCalButtons();
            SetCalDescription();

            // calibration panels
            pnlFlow.Visible = CurrentProduct.ControlType != ControlTypeEnum.MotorWeights;
            pnlWeight.Visible = CurrentProduct.ControlType == ControlTypeEnum.MotorWeights;

            TankSize.Enabled = CurrentProduct.ControlType != ControlTypeEnum.MotorWeights;
            TankRemain.Enabled = CurrentProduct.ControlType != ControlTypeEnum.MotorWeights;
            btnResetTank.Enabled = CurrentProduct.ControlType != ControlTypeEnum.MotorWeights;
        }

        private void WeightCal_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbScaleCountsPerUnit.Text, out tempD);
            using (var form = new FormNumeric(1, 10000, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbScaleCountsPerUnit.Text = form.ReturnValue.ToString("N1");
                }
            }
        }

        private void WeightCal_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "The number of weigh scale counts for each unit of weight. ";

            mf.Tls.ShowHelp(Message, "Weight Cal", 15000);
            hlpevent.Handled = true;
        }

        private void WeightCal_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void WeightCal_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbScaleCountsPerUnit.Text, out tempD);
            if (tempD < 1 || tempD > 10000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }
    }
}