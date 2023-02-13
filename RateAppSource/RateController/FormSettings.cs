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
        public clsProduct CurrentProduct;
        private bool Initializing = false;
        private double[] MaxError = new double[6];
        private Label[] Sec;
        private TabPage[] tbs;
        private TimeSpan CalTimeSpan;
        private DateTime[] CalTimeStart = new DateTime[6];
        private DateTime[] CalTimeEnd = new DateTime[6];

        private TabPage Temp1;
        private TabPage Temp2;
        private bool FormEdited = false;

        public FormSettings(FormStart CallingForm, int Page)
        {
            Initializing = true;
            InitializeComponent();

            #region // language

            lbProduct.Text = Lang.lgProduct;
            tcProducts.TabPages[0].Text = Lang.lgRate;
            tcProducts.TabPages[1].Text = Lang.lgControl;
            tcProducts.TabPages[2].Text = Lang.lgOptions;
            tcProducts.TabPages[3].Text = Lang.lgDiagnostics;
            tcProducts.TabPages[4].Text = Lang.lgCalibrate;

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

            lbMax.Text = Lang.lgHighMax;
            lbMin.Text = Lang.lgMinPWM;

            grpSensor.Text = Lang.lgSensorLocation;
            lbConID.Text = Lang.lgModuleID;
            lbSensorID.Text = Lang.lgSensorID;

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
            btnCalCalculate.Text = Lang.lgSensorCounts;

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

            lbProportional.Text = Lang.lgProportional;
            lbIntegral.Text = Lang.lgIntegral;
            lbDerivative.Text = Lang.lgDerivative;
            lbMax.Text = Lang.lgPWMmax;
            lbMin.Text = Lang.lgPWMmin;

            lbSensorID.Text = Lang.lgSensorID;
            grpRateMethod.Text = Lang.lgRateMethod;
            rbSinglePulse.Text = Lang.lgTimeForSingle;
            rbMultiPulse.Text = Lang.lgTimeForMulti;

            lbMinimumUPM.Text = Lang.lgMinUPM;
            ckOffRate.Text = Lang.lgOffRate;

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
        }

        private void AreaUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void bntOK_Click(object sender, EventArgs e)
        {
            if (!FormEdited)
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
            try
            {
                if (CurrentProduct.ControlType != ControlTypeEnum.Valve && CurrentProduct.ControlType != ControlTypeEnum.ComboClose)
                {
                    // set motor speed
                    byte tmp = 0;

                    if (pnlFlow.Visible)
                    {
                        // use flow pwm value
                        byte.TryParse(tbFLpwm.Text, out tmp);
                    }
                    else
                    {
                        // use weight pwm value
                        byte.TryParse(tbWTpwm.Text, out tmp);
                    }

                    CurrentProduct.ManualPWM = tmp;
                }

                CurrentProduct.DoCal = true;
                mf.RateCalibrationOn = true;
                CurrentProduct.CalStart = CurrentProduct.UnitsApplied();
                SetCalButtons();
                CalTimeStart[CurrentProduct.ID] = DateTime.Now;
                lbWTcal.Text = "0.0";
                lbWTquantity.Text = "0.0";
                lbFlowMeterCounts.Text = "0.0";

                //mf.SwitchBox.PressSwitch(SwIDs.MasterOn);
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp("FormSettings/btnCalStart " + ex.Message);
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
            try
            {
                CurrentProduct.CalEnd = CurrentProduct.UnitsApplied();
                CurrentProduct.DoCal = false;
                mf.RateCalibrationOn = false;
                SetCalButtons();
                btnCalCopy.Focus();

                CalTimeEnd[CurrentProduct.ID] = DateTime.Now;
                CalTimeSpan = (CalTimeEnd[CurrentProduct.ID] - CalTimeStart[CurrentProduct.ID]);
                if (CurrentProduct.ControlType == ControlTypeEnum.MotorWeights)
                {
                    // calibrate by weight
                    double tmp = CurrentProduct.CalEnd - CurrentProduct.CalStart;
                    if (tmp > 0)
                    {
                        lbWTquantity.Text = tmp.ToString("N1");

                        // UPM/PWM
                        double Minutes = CalTimeSpan.TotalSeconds / 60.0;
                        double UPM = 0;
                        if (Minutes > 0)
                        {
                            UPM = tmp / Minutes;
                        }
                        double UPMperPWM = 0;
                        if (CurrentProduct.ManualPWM > 0)
                        {
                            UPMperPWM = UPM / CurrentProduct.ManualPWM;
                        }
                        lbWTcal.Text = UPMperPWM.ToString("N3");
                    }
                }
                else
                {
                    // calibrate by flow meter counts
                    lbFlowMeterCounts.Text = FlowMeterCounts().ToString("N0");
                }
                //mf.SwitchBox.PressSwitch(SwIDs.MasterOff);
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp("FormSettings/btnCalStop " + ex.Message);
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
            if (CurrentProduct.ID < mf.MaxProducts - 1)
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
            //else
            //{
            //    // set unique pair
            //    for (int i = 0; i < 16; i++)
            //    {
            //        Unique = mf.Products.UniqueModSen(i, SenID, CurrentProduct.ID);
            //        if (Unique)
            //        {
            //            Initializing = true;
            //            tbConID.Text = i.ToString();
            //            Initializing = false;

            //            mf.Tls.ShowHelp("Module set to " + i.ToString() + "\n Module/Sensor pair must be unique. Change as necessary.", "Help", 3000);

            //            break;
            //        }
            //    }
            //}
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

        private void FlowCal_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(FlowCal.Text, out tempD);
            using (var form = new FormNumeric(0.01, 10000, tempD))
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
            if (tempD < 0.01 || tempD > 10000)
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

        private void label24_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Enter the counts per revolution so RPM can be calculated.";

            mf.Tls.ShowHelp(Message, "Counts/Rev");
            hlpevent.Handled = true;
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

        private void lbWidth_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "The width of the implement that is applying product.";

            mf.Tls.ShowHelp(Message, "Working Width");
            hlpevent.Handled = true;
        }

        private void LoadDefaults()
        {
            tbKP.Text = "1.0000";
            tbKI.Text = "0";
            tbKD.Text = "0";
            tbMaxPWM.Text = "100";
            tbMinPWM.Text = "5";
        }

        private void LoadSettings()
        {
            if (CurrentProduct.ControlType == ControlTypeEnum.Fan)
            {
                tbTargetRPM.Text = CurrentProduct.RateSet.ToString("N1");
                tbCountsRPM.Text = CurrentProduct.MeterCal.ToString("N3");
            }
            else
            {
                lbBaseRate.Text = CurrentProduct.RateSet.ToString("N1");
                FlowCal.Text = CurrentProduct.MeterCal.ToString("N3");
            }

            tbProduct.Text = CurrentProduct.ProductName;
            tbVolumeUnits.Text = CurrentProduct.QuantityDescription;
            AreaUnits.SelectedIndex = CurrentProduct.CoverageUnits;
            CbUseProdDensity.Checked = CurrentProduct.EnableProdDensity;
            if (!CbUseProdDensity.Checked)
            { CbUseProdDensity_CheckedChanged_1(CbUseProdDensity, EventArgs.Empty); }
            ProdDensity.Text = CurrentProduct.ProdDensity.ToString("N1");
            tbAltRate.Text = CurrentProduct.RateAlt.ToString("N0");
            TankSize.Text = CurrentProduct.TankSize.ToString("N0");
            if (CurrentProduct.ControlType != ControlTypeEnum.Fan) ValveType.SelectedIndex = (int)CurrentProduct.ControlType;
            cbVR.SelectedIndex = CurrentProduct.VariableRate;

            TankRemain.Text = CurrentProduct.TankStart.ToString("N0");

            tbCountsRev.Text = (CurrentProduct.CountsRev.ToString("N0"));

            string tmp = CurrentProduct.ModuleID.ToString();
            if (tmp == "99") tmp = "";
            tbConID.Text = tmp;

            // PID
            tbKP.Text = CurrentProduct.PIDkp.ToString("N4");
            tbKI.Text = CurrentProduct.PIDki.ToString("N4");
            tbKD.Text = CurrentProduct.PIDkd.ToString("N4");
            tbMinPWM.Text = CurrentProduct.PIDmin.ToString("N0");
            tbMaxPWM.Text = CurrentProduct.PIDmax.ToString("N0");

            tbSenID.Text = CurrentProduct.SensorID.ToString();

            rbSinglePulse.Checked = !(CurrentProduct.UseMultiPulse);
            rbMultiPulse.Checked = (CurrentProduct.UseMultiPulse);

            // load defaults if blank
            //double.TryParse(tbKP.Text, out double TmpDB);
            //if (TmpDB == 0)
            //{
            //    LoadDefaults();
            //    SaveSettings();
            //}

            tbMinUPM.Text = CurrentProduct.MinUPM.ToString("N1");
            ckOffRate.Checked = CurrentProduct.UseOffRateAlarm;
            tbOffRate.Text = CurrentProduct.OffRateSetting.ToString("N0");
            tbWTpwm.Text = CurrentProduct.ManualPWM.ToString("N0");
            tbFLpwm.Text = CurrentProduct.ManualPWM.ToString("N0");
            tbScaleCountsPerUnit.Text = CurrentProduct.ScaleCountsPerUnit.ToString("N1");
            SetCalButtons();
        }

        private void RateSet_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(lbBaseRate.Text, out tempD);
            using (var form = new FormNumeric(0, 50000, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    lbBaseRate.Text = form.ReturnValue.ToString();
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
            double.TryParse(lbBaseRate.Text, out tempD);
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
            double TempDB;
            int tempInt;
            byte tempB;

            CurrentProduct.ControlType = (ControlTypeEnum)(ValveType.SelectedIndex);

            if (CurrentProduct.ControlType == ControlTypeEnum.Fan)
            {
                // set rate by fan
                double.TryParse(tbTargetRPM.Text, out TempDB);
                CurrentProduct.RateSet = TempDB;

                double.TryParse(tbCountsRPM.Text, out TempDB);
                CurrentProduct.MeterCal = TempDB;

                CurrentProduct.CoverageUnits = 2;   // minutes
            }
            else
            {
                // set rate by product
                double.TryParse(lbBaseRate.Text, out TempDB);
                CurrentProduct.RateSet = TempDB;

                double.TryParse(ProdDensity.Text, out TempDB);
                CurrentProduct.ProdDensity = TempDB;

                CurrentProduct.EnableProdDensity = CbUseProdDensity.Checked;

                double.TryParse(FlowCal.Text, out TempDB);
                CurrentProduct.MeterCal = TempDB;

                CurrentProduct.CoverageUnits = Convert.ToByte(AreaUnits.SelectedIndex);
            }

            CurrentProduct.QuantityDescription = tbVolumeUnits.Text;

            double.TryParse(tbAltRate.Text, out TempDB);
            CurrentProduct.RateAlt = TempDB;


            double.TryParse(TankSize.Text, out TempDB);
            CurrentProduct.TankSize = TempDB;

            CurrentProduct.VariableRate = Convert.ToByte(cbVR.SelectedIndex);

            double.TryParse(TankRemain.Text, out TempDB);
            CurrentProduct.TankStart = TempDB;

            CurrentProduct.ProductName = tbProduct.Text;

            byte.TryParse(tbConID.Text, out byte tmp1);
            byte.TryParse(tbSenID.Text, out byte tmp2);
            CurrentProduct.ChangeID(tmp1, tmp2);

            // PID
            double.TryParse(tbKP.Text, out TempDB);
            CurrentProduct.PIDkp = TempDB;

            double.TryParse(tbKI.Text, out TempDB);
            CurrentProduct.PIDki = TempDB;

            double.TryParse(tbKD.Text, out TempDB);
            CurrentProduct.PIDkd = TempDB;

            byte.TryParse(tbMinPWM.Text, out tempB);
            CurrentProduct.PIDmin = tempB;

            byte.TryParse(tbMaxPWM.Text, out tempB);
            CurrentProduct.PIDmax = tempB;

            int.TryParse(tbCountsRev.Text, out tempInt);
            CurrentProduct.CountsRev = tempInt;

            CurrentProduct.UseMultiPulse = (rbMultiPulse.Checked);

            double.TryParse(tbMinUPM.Text, out TempDB);
            CurrentProduct.MinUPM = TempDB;

            CurrentProduct.UseOffRateAlarm = ckOffRate.Checked;

            byte.TryParse(tbOffRate.Text, out tempB);
            CurrentProduct.OffRateSetting = tempB;

            if (pnlFlow.Visible)
            {
                // use flow pwm value
                byte.TryParse(tbFLpwm.Text, out tempB);
            }
            else
            {
                // use weight pwm value
                byte.TryParse(tbWTpwm.Text, out tempB);
            }
            CurrentProduct.ManualPWM = tempB;

            double.TryParse(tbScaleCountsPerUnit.Text, out TempDB);
            CurrentProduct.ScaleCountsPerUnit = TempDB;

            double.TryParse(tbTare.Text, out TempDB);
            CurrentProduct.ScaleTare = TempDB;

            CurrentProduct.Save();
        }

        private void SetButtons(bool Edited)
        {
            if (!Initializing)
            {
                if (Edited)
                {
                    btnCancel.Enabled = true;
                    btnLeft.Enabled = false;
                    btnRight.Enabled = false;
                    btnOK.Image = Properties.Resources.Save;
                    btnCalStart.Enabled=false;
                }
                else
                {
                    btnCancel.Enabled = false;
                    btnLeft.Enabled = true;
                    btnRight.Enabled = true;
                    btnOK.Image = Properties.Resources.OK;
                    btnCalStart.Enabled = true;
                }

                FormEdited = Edited;
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
            tbFLpwm.Enabled = !CurrentProduct.DoCal;
            btnFlowUp.Enabled = !CurrentProduct.DoCal;
            btnFlowDown.Enabled = !CurrentProduct.DoCal;
            btnWeightUp.Enabled = !CurrentProduct.DoCal;
            btnWeightDown.Enabled = !CurrentProduct.DoCal;
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
                lbFanStarted.BackColor = Properties.Settings.Default.DayColour;

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
            for (int i = 0; i < tcProducts.TabCount; i++)
            {
                tcProducts.TabPages[i].BackColor = Properties.Settings.Default.DayColour;
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

        private void SetFanStarted()
        {
            if (CurrentProduct.FanOn)
            {
                lbFanStarted.Image = Properties.Resources.On;
            }
            else
            {
                lbFanStarted.Image = Properties.Resources.Off;
            }
        }

        private void ShowSpan()
        {
            if (CurrentProduct.DoCal)
            {
                CalTimeSpan = (DateTime.Now - CalTimeStart[CurrentProduct.ID]);
            }
            else
            {
                CalTimeSpan = (CalTimeEnd[CurrentProduct.ID] - CalTimeStart[CurrentProduct.ID]);
            }
            lbWTtime.Text = CalTimeSpan.Minutes.ToString("00") + ":" + CalTimeSpan.Seconds.ToString("00");
            lbFLtime.Text = CalTimeSpan.Minutes.ToString("00") + ":" + CalTimeSpan.Seconds.ToString("00");
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
            using (var form = new FormNumeric(0, 100000, tempD))
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
            if (tempD < 0 || tempD > 100000)
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
            SetFanStarted();
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
            lbFanRPMvalue.Text = Applied.ToString("N0");

            if (Target > 0)
            {
                RateError = ((Applied - Target) / Target) * 100;
                bool IsNegative = RateError < 0;
                RateError = Math.Abs(RateError);
                if (RateError > 100) RateError = 100;
                if (IsNegative) RateError *= -1;
            }
            lbErrorPercent.Text = RateError.ToString("N1");
            lbFanErrorValue.Text = RateError.ToString("N1");

            lbPWMdata.Text = CurrentProduct.PWM().ToString("N0");
            lbFanPWMvalue.Text = CurrentProduct.PWM().ToString("N0");

            lbWidthData.Text = mf.Sections.WorkingWidth(mf.UseInches).ToString("N1");
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
                //if (CurrentProduct.EnableProdDensity && CurrentProduct.ProdDensity > 0) RPM /= (float)CurrentProduct.ProdDensity;
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

            // product name
            if (mf.SimMode != SimType.None)
            {
                //lbProduct.Text = lbProduct.Text;
                lbProduct.BackColor = mf.SimColor;
                lbProduct.BorderStyle = BorderStyle.FixedSingle;
            }
            else
            {
                lbProduct.ForeColor = SystemColors.ControlText;
                lbProduct.BackColor = Properties.Settings.Default.DayColour;
                lbProduct.BorderStyle = BorderStyle.None;
            }
        }

        private void UpdateForm()
        {
            Initializing = true;

            UpdateDiags();
            LoadSettings();
            SetModuleIndicator();
            SetFanStarted();
            SetCalButtons();
            SetDayMode();
            SetCalDescription();

            if (CurrentProduct.ID == mf.MaxProducts - 1)
            {
                lbProduct.Text = "Fan 2";
            }
            else if (CurrentProduct.ID == mf.MaxProducts - 2)
            {
                lbProduct.Text = "Fan 1";
            }
            else
            {
                lbProduct.Text = (CurrentProduct.ID + 1).ToString() + ". " + CurrentProduct.ProductName;
            }

            if (mf.SimMode != SimType.None)
            {
                //lbProduct.Text = lbProduct.Text;
                lbProduct.BackColor = mf.SimColor;
                lbProduct.BorderStyle = BorderStyle.FixedSingle;
            }
            else
            {
                lbProduct.ForeColor = SystemColors.ControlText;
                lbProduct.BackColor = Properties.Settings.Default.DayColour;
                lbProduct.BorderStyle = BorderStyle.None;
            }

            tbTare.Text = CurrentProduct.ScaleTare.ToString("N0");

            Initializing = false;
            UpdateOnTypeChange();

            if (CurrentProduct.ControlType == ControlTypeEnum.Fan)
            {
                if (tcProducts.TabCount > 4)
                {
                    // remove tabs
                    Temp1 = tcProducts.TabPages[3];
                    Temp2 = tcProducts.TabPages[4];
                    tcProducts.Controls.Remove(Temp1);
                    tcProducts.Controls.Remove(Temp2);
                }
            }
            else
            {
                if (tcProducts.TabCount < 5)
                {
                    // add back the removed tabs
                    tcProducts.TabPages.Add(Temp1);
                    tcProducts.TabPages.Add(Temp2);
                }
            }
        }

        void UpdateOnTypeChange()
        {
            // calibration panels
            pnlFlow.Visible = CurrentProduct.ControlType != ControlTypeEnum.MotorWeights;
            pnlWeight.Visible = CurrentProduct.ControlType == ControlTypeEnum.MotorWeights;

            TankSize.Enabled = CurrentProduct.ControlType != ControlTypeEnum.MotorWeights;
            TankRemain.Enabled = CurrentProduct.ControlType != ControlTypeEnum.MotorWeights;
            btnResetTank.Enabled = CurrentProduct.ControlType != ControlTypeEnum.MotorWeights;

            pnlFan.Visible = (CurrentProduct.ControlType == ControlTypeEnum.Fan);
            pnlMain.Visible = (CurrentProduct.ControlType != ControlTypeEnum.Fan);

            // tb flow pwm
            if (CurrentProduct.ControlType == ControlTypeEnum.Valve || CurrentProduct.ControlType == ControlTypeEnum.ComboClose)
            {
                tbFLpwm.Visible = false;
                grpFlowPWM.Text = "Rate";
            }
            else
            {
                tbFLpwm.Visible = true;
                grpFlowPWM.Text = "PWM";
            }
        }

        private void ValveType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetCalButtons();
            SetButtons(true);
            SetCalDescription();
            UpdateOnTypeChange();
        }

        private void WeightCal_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbScaleCountsPerUnit.Text, out tempD);
            using (var form = new FormNumeric(-10000, 10000, tempD))
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
            if (tempD < -10000 || tempD > 10000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbTargetRPM_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbTargetRPM.Text, out tempD);
            using (var form = new FormNumeric(0, 50000, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbTargetRPM.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void tbTargetRPM_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbTargetRPM_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbTargetRPM.Text, out tempD);
            if (tempD < 0 || tempD > 50000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbCountsRPM_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbCountsRPM.Text, out tempD);
            using (var form = new FormNumeric(0, 50000, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbCountsRPM.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void tbCountsRPM_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbCountsRPM_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbCountsRPM.Text, out tempD);
            if (tempD < 0 || tempD > 50000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void ProdDensity_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(ProdDensity.Text, out tempD);
            using (var form = new FormNumeric(0, 10000, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    ProdDensity.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void ProdDensity_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void CbUseProdDensity_CheckedChanged_1(object sender, EventArgs e)
        {
            if (CbUseProdDensity.Checked)
            {
                ProdDensity.Enabled = true;
                LabProdDensity.Enabled = true;
            }
            else
            {
                ProdDensity.Enabled = false;
                LabProdDensity.Enabled = false;
            }

            SetButtons(true);
        }

        private void tbKP_Enter(object sender, EventArgs e)
        {
            double temp;
            double.TryParse(tbKP.Text, out temp);
            using (var form = new FormNumeric(0, 10000, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbKP.Text = form.ReturnValue.ToString("N4");
                }
            }
        }

        private void tbKI_Enter(object sender, EventArgs e)
        {
            double temp;
            double.TryParse(tbKI.Text, out temp);
            using (var form = new FormNumeric(0, 10000, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbKI.Text = form.ReturnValue.ToString("N4");
                }
            }
        }

        private void tbKD_Enter(object sender, EventArgs e)
        {
            double temp;
            double.TryParse(tbKD.Text, out temp);
            using (var form = new FormNumeric(0, 10000, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbKD.Text = form.ReturnValue.ToString("N4");
                }
            }
        }

        private void tbMaxPWM_Enter(object sender, EventArgs e)
        {
            byte temp;
            byte.TryParse(tbMaxPWM.Text, out temp);
            using (var form = new FormNumeric(0, 255, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbMaxPWM.Text = form.ReturnValue.ToString("N0");
                }
            }
        }

        private void tbMinPWM_Enter(object sender, EventArgs e)
        {
            byte temp;
            byte.TryParse(tbMinPWM.Text, out temp);
            using (var form = new FormNumeric(0, 255, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbMinPWM.Text = form.ReturnValue.ToString("N0");
                }
            }
        }

        private void tbKP_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbKP_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbKP.Text, out tempD);
            if (tempD < 0 || tempD > 10000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbKI_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbKI.Text, out tempD);
            if (tempD < 0 || tempD > 10000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbKD_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbKD.Text, out tempD);
            if (tempD < 0 || tempD > 10000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbMaxPWM_Validating(object sender, CancelEventArgs e)
        {
            byte temp;
            byte.TryParse(tbMaxPWM.Text, out temp);
            if (temp < 0 || temp > 255)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbMinPWM_Validating(object sender, CancelEventArgs e)
        {
            byte temp;
            byte.TryParse(tbMinPWM.Text, out temp);
            if (temp < 0 || temp > 255)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbKD_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Derivative looks at past errors in the system and" +
                " calculates the slope of those errors to predict future error values.";

            mf.Tls.ShowHelp(Message, "Derivative");
            hlpevent.Handled = true;
        }

        private void tbKP_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Proportional control output has a direct ratio to the error." +
                " Higher Proportional has a greater response to error.";

            mf.Tls.ShowHelp(Message, "Proportional");
            hlpevent.Handled = true;
        }

        private void btnTuningGraph_Click(object sender, EventArgs e)
        {
            //check if window already exists
            Form fcg = Application.OpenForms["FormPIDGraph"];

            if (fcg != null)
            {
                fcg.Focus();
                return;
            }

            //
            Form formG = new FormPIDGraph(this);
            formG.Show(this);
        }

        private void label26_Click(object sender, EventArgs e)
        {

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            CurrentProduct.FanOn = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            CurrentProduct.FanOn = false;
        }

        private void tbFLpwm_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbFLpwm.Text, out tempD);
            using (var form = new FormNumeric(1, 255, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbFLpwm.Text = form.ReturnValue.ToString("N0");
                }
            }
        }

        private void tbFLpwm_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Used to set a constant applicator speed for calibration." +
                "Run until a sufficient quantity is measured for an accurate result. ";

            mf.Tls.ShowHelp(Message, "PWM Cal", 15000);
            hlpevent.Handled = true;
        }

        private void tbFLpwm_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbFLpwm.Text, out tempD);
            if (tempD < 1 || tempD > 255)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void lbWTcal_Click(object sender, EventArgs e)
        {

        }

        private void lbWeightMeasured_Click(object sender, EventArgs e)
        {

        }

        private void btnFlowUp_Click(object sender, EventArgs e)
        {
            if (CurrentProduct.ControlType == ControlTypeEnum.Valve || CurrentProduct.ControlType == ControlTypeEnum.ComboClose)
            {
                mf.SwitchBox.PressSwitch(SwIDs.RateUp);
            }
            else
            {
                int.TryParse(tbFLpwm.Text, out int Tmp);
                Tmp += 5;
                if (Tmp > 255) Tmp = 255;
                tbFLpwm.Text = Tmp.ToString();
            }
        }

        private void btnFlowDown_Click(object sender, EventArgs e)
        {
            if (CurrentProduct.ControlType == ControlTypeEnum.Valve || CurrentProduct.ControlType == ControlTypeEnum.ComboClose)
            {
                mf.SwitchBox.PressSwitch(SwIDs.RateDown);
            }
            else
            {
                int.TryParse(tbFLpwm.Text, out int Tmp);
                Tmp -= 5;
                if (Tmp < 0) Tmp = 0;
                tbFLpwm.Text = Tmp.ToString();
            }
        }

        private void btnWeightUp_Click(object sender, EventArgs e)
        {
            int.TryParse(tbWTpwm.Text, out int Tmp);
            Tmp += 5;
            if (Tmp > 255) Tmp = 255;
            tbWTpwm.Text = Tmp.ToString();
        }

        private void btnWeightDown_Click(object sender, EventArgs e)
        {
            int.TryParse(tbWTpwm.Text, out int Tmp);
            Tmp -= 5;
            if (Tmp < 0) Tmp = 0;
            tbWTpwm.Text = Tmp.ToString();
        }
    }
}