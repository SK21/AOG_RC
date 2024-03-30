using AgOpenGPS;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace RateController
{
    public partial class FormSettings : Form
    {
        public FormStart mf;

        public clsProduct CurrentProduct;
        private bool Initializing = false;
        private Label[] Sec;

        private TabPage[] Tabs;
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
            tcProducts.TabPages[2].Text = Lang.lgControl;
            tcProducts.TabPages[3].Text = Lang.lgOptions;
            tcProducts.TabPages[4].Text = Lang.lgDiagnostics;

            lb0.Text = Lang.lgProductName;
            lb5.Text = Lang.lgControlType;
            lb1.Text = Lang.lgQuantity;
            lb2.Text = Lang.lgCoverage;
            lbSensorCounts.Text = Lang.lgSensorCounts;
            lbBaseRateDes.Text = Lang.lgBaseRate;
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

            ValveType.Items[0] = Lang.lgStandard;
            ValveType.Items[1] = Lang.lgComboClose;
            ValveType.Items[2] = Lang.lgMotor;
            //ValveType.Items[3] = Lang.lgMotorWeight;
            //ValveType.Items[4] = Lang.lgComboTimed;

            AreaUnits.Items[0] = Lang.lgAcres;
            AreaUnits.Items[1] = Lang.lgHectares;
            AreaUnits.Items[2] = Lang.lgMinute;
            AreaUnits.Items[3] = Lang.lgHour;

            lbAltRate.Text = Lang.lgAltRate;
            lbVariableRate.Text = Lang.lgChannel;
            lbMaxRate.Text = Lang.lgMaxRate;
            lbMinRate.Text = Lang.lgMinRate;
            ckVR.Text = Lang.lgUseVR;

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
            ckDefault.Text = Lang.lgDefaultProduct;
            ckOnScreen.Text = Lang.lgOnScreen;

            label2.Text = Lang.lgWifiSignal;
            LabProdDensity.Text = Lang.lgDensity;

            #endregion // language

            mf = CallingForm;
            Tabs = new TabPage[] { tbs0, tabPage1, tbs4, tbs6, tbs3 };

            if (Page == 0)
            {
                CurrentProduct = mf.Products.Item(0);
            }
            else
            {
                CurrentProduct = mf.Products.Item(Page - 1);
            }

            openFileDialog1.InitialDirectory = mf.Tls.FilesDir();
            saveFileDialog1.InitialDirectory = mf.Tls.FilesDir();

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
                UpdateForm();
            }
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

        private void FormSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
                mf.Tls.SaveFormData(this);
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
            string Message = "This is the sensor counts for 1 unit of product.";
            //string Message = "For flow sensors this is the sensor counts for 1 unit of product.\n";
                //"For weight control this is Units per Minute for each Pulse Width Modulation value.";

            mf.Tls.ShowHelp(Message, "Sensor Counts");
            hlpevent.Handled = true;
        }

        private void lb5_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "1 - Standard, use a valve to vary rate \n " +
                "2 - Combo Close, use a valve to vary rate and on/off \n" +
                "3 - Motor, vary motor speed to control rate \n";

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
            tbKP.Text = "1";
            tbKI.Text = "0";
            tbKD.Text = "0";
            tbMaxPWM.Text = "100";
            tbMinPWM.Text = "5";
            cbShift.SelectedIndex = 0;
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
            ValveType.SelectedIndex = ConvertControlType(CurrentProduct.ControlType);
            cbVR.SelectedIndex = CurrentProduct.VRID;
            ckVR.Checked = CurrentProduct.UseVR;
            tbMaxRate.Text = CurrentProduct.VRmax.ToString("N1");
            tbMinRate.Text = CurrentProduct.VRmin.ToString("N1");

            TankRemain.Text = CurrentProduct.TankStart.ToString("N0");

            tbCountsRev.Text = (CurrentProduct.CountsRev.ToString("N0"));

            string tmp = CurrentProduct.ModuleID.ToString();
            if (tmp == "99") tmp = "";
            tbConID.Text = tmp;

            // PID
            tbKP.Text = CurrentProduct.PIDkp.ToString("N0");
            tbKI.Text = CurrentProduct.PIDki.ToString("N0");
            tbKD.Text = CurrentProduct.PIDkd.ToString("N0");
            tbMinPWM.Text = CurrentProduct.PIDmin.ToString("N0");
            tbMaxPWM.Text = CurrentProduct.PIDmax.ToString("N0");

            tbSenID.Text = CurrentProduct.SensorID.ToString();

            rbSinglePulse.Checked = !(CurrentProduct.UseMultiPulse);
            rbMultiPulse.Checked = (CurrentProduct.UseMultiPulse);

            tbMinUPM.Text = CurrentProduct.MinUPM.ToString("N1");
            ckOffRate.Checked = CurrentProduct.UseOffRateAlarm;
            tbOffRate.Text = CurrentProduct.OffRateSetting.ToString("N0");

            ckOnScreen.Checked = CurrentProduct.OnScreen;
            ckBumpButtons.Checked = CurrentProduct.BumpButtons;

            ckConstantUPM.Checked = CurrentProduct.ConstantUPM;
            cbShift.SelectedIndex = CurrentProduct.PIDscale;
            UpdateExample();
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

        private ControlTypeEnum ConvertValveIndex(int ValveType)
        {
            // convert valve type index to ControlTypeEnum
            // valve types: Standard Valve, Fast Close Valve, Motor, Motor / Weights, Combo Timed
            ControlTypeEnum Result;
            switch (ValveType)
            {
                case 1:
                    Result = ControlTypeEnum.ComboClose;
                    break;

                case 2:
                    Result = ControlTypeEnum.Motor;
                    break;

                default:
                    Result = ControlTypeEnum.Valve;
                    break;
            }
            return Result;
        }

        private int ConvertControlType(ControlTypeEnum Type)
        {
            // convert control type to valve type index
            int Result;
            switch (Type)
            {
                case ControlTypeEnum.ComboClose:
                    Result = (int)ControlTypeEnum.ComboClose;
                    break;

                case ControlTypeEnum.Motor:
                    Result = (int)ControlTypeEnum.Motor;
                    break;

                default:
                    Result = (int)ControlTypeEnum.Valve;
                    break;
            }
            return Result;
        }

        private void SaveSettings()
        {
            double TempDB;
            int tempInt;
            byte tempB;

            CurrentProduct.ControlType = ConvertValveIndex(ValveType.SelectedIndex);

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

            if (double.TryParse(tbMaxRate.Text, out double tmp10) &&
                double.TryParse(tbMinRate.Text, out double tmp11))
            {
                if (tmp11 > tmp10)
                {
                    mf.Tls.ShowHelp("Minimum VR rate must be less than Maximum.", "VR", 10000);
                }
                else
                {
                    CurrentProduct.VRmax = tmp10;
                    CurrentProduct.VRmin = tmp11;
                }
            }

                CurrentProduct.VRID = Convert.ToByte(cbVR.SelectedIndex);
            CurrentProduct.UseVR = (ckVR.Checked);

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

            CurrentProduct.ManualPWM = tempB;

            CurrentProduct.ConstantUPM = ckConstantUPM.Checked;

            CurrentProduct.OnScreen = ckOnScreen.Checked;
            CurrentProduct.BumpButtons = ckBumpButtons.Checked;

            CurrentProduct.Save();

            if (ckDefault.Checked) mf.DefaultProduct = CurrentProduct.ID;
            CurrentProduct.PIDscale = cbShift.SelectedIndex;
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
                }
                else
                {
                    btnCancel.Enabled = false;
                    btnLeft.Enabled = true;
                    btnRight.Enabled = true;
                    btnOK.Image = Properties.Resources.OK;
                }

                FormEdited = Edited;
            }
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

                for (int i = 0; i < 4; i++)
                {
                    Tabs[i].BackColor = Properties.Settings.Default.DayColour;
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

                for (int i = 0; i < 4; i++)
                {
                    Tabs[i].BackColor = Properties.Settings.Default.NightColour;
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

        private void tbVolumeUnits_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateDiags();
            SetModuleIndicator();
            SetFanStarted();
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

            // fan button
            if (CurrentProduct.FanOn)
            {
                btnFan.Image = Properties.Resources.FanOn;
            }
            else
            {
                btnFan.Image = Properties.Resources.FanOff;
            }
        }

        private void setBumpButtonVisibility(bool visible)
        {
            ckBumpButtons.Visible = visible;
            ckBumpButtons.Enabled = visible;
        }

        private void UpdateForm()
        {
            Initializing = true;

            UpdateDiags();
            LoadSettings();
            SetModuleIndicator();
            SetFanStarted();
            SetDayMode();
            SetCalDescription();

            if (CurrentProduct.ID == mf.MaxProducts - 1)
            {
                lbProduct.Text = "Fan 2";
                setBumpButtonVisibility(false);
            }
            else if (CurrentProduct.ID == mf.MaxProducts - 2)
            {
                lbProduct.Text = "Fan 1";
                setBumpButtonVisibility(false);
            }
            else
            {
                lbProduct.Text = (CurrentProduct.ID + 1).ToString() + ". " + CurrentProduct.ProductName;
                setBumpButtonVisibility(true);
            }

            if (mf.SimMode != SimType.None)
            {
                lbProduct.BackColor = mf.SimColor;
                lbProduct.BorderStyle = BorderStyle.FixedSingle;
            }
            else
            {
                lbProduct.ForeColor = SystemColors.ControlText;
                lbProduct.BackColor = Properties.Settings.Default.DayColour;
                lbProduct.BorderStyle = BorderStyle.None;
            }

            ckDefault.Checked = (mf.DefaultProduct == CurrentProduct.ID);

            Initializing = false;
            UpdateOnTypeChange();

            if (CurrentProduct.ControlType == ControlTypeEnum.Fan)
            {
                ckDefault.Visible = false;
                if (tcProducts.TabCount > 3)
                {
                    // remove tabs
                    Temp1 = tcProducts.TabPages[1];
                    Temp2 = tcProducts.TabPages[4];
                    tcProducts.Controls.Remove(Temp1);
                    tcProducts.Controls.Remove(Temp2);
                }
            }
            else
            {
                ckDefault.Visible = true;
                if (tcProducts.TabCount < 5)
                {
                    // add back the removed tabs
                    tcProducts.TabPages.Insert(1, Temp1);
                    tcProducts.TabPages.Add(Temp2);
                }
            }

            lbBaseRate.Enabled = !CurrentProduct.UseVR;
            lbAltRate.Enabled = !CurrentProduct.UseVR;
            lbBaseRateDes.Enabled = !CurrentProduct.UseVR;
            tbAltRate.Enabled = !CurrentProduct.UseVR;
            lbVariableRate.Enabled = CurrentProduct.UseVR;
            cbVR.Enabled = CurrentProduct.UseVR;
            lbMaxRate.Enabled = CurrentProduct.UseVR;
            tbMaxRate.Enabled = CurrentProduct.UseVR;
            lbMinRate.Enabled = CurrentProduct.UseVR;
            tbMinRate.Enabled = CurrentProduct.UseVR;
        }

        void UpdateOnTypeChange()
        {
            TankSize.Enabled = CurrentProduct.ControlType != ControlTypeEnum.MotorWeights;
            TankRemain.Enabled = CurrentProduct.ControlType != ControlTypeEnum.MotorWeights;
            btnResetTank.Enabled = CurrentProduct.ControlType != ControlTypeEnum.MotorWeights;

            pnlFan.Visible = (CurrentProduct.ControlType == ControlTypeEnum.Fan);
            pnlMain.Visible = (CurrentProduct.ControlType != ControlTypeEnum.Fan);
        }

        private void ValveType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtons(true);
            SetCalDescription();
            UpdateOnTypeChange();
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
            using (var form = new FormNumeric(0, 255, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbKP.Text = form.ReturnValue.ToString("N0");
                }
            }
        }

        private void tbKI_Enter(object sender, EventArgs e)
        {
            double temp;
            double.TryParse(tbKI.Text, out temp);
            using (var form = new FormNumeric(0, 255, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbKI.Text = form.ReturnValue.ToString("N0");
                }
            }
        }

        private void tbKD_Enter(object sender, EventArgs e)
        {
            double temp;
            double.TryParse(tbKD.Text, out temp);
            using (var form = new FormNumeric(0, 255, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbKD.Text = form.ReturnValue.ToString("N0");
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
            UpdateExample();
        }
        private void UpdateExample()
        {
            int val = (int)CurrentProduct.PIDkp;
            if (int.TryParse(tbKP.Text, out int ex)) val = ex;
            lbExample.Text = (val * Math.Pow(10, -cbShift.SelectedIndex)).ToString("N7");
        }

        private void tbKP_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbKP.Text, out tempD);
            if (tempD < 0 || tempD > 255)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbKI_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbKI.Text, out tempD);
            if (tempD < 0 || tempD > 255)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbKD_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbKD.Text, out tempD);
            if (tempD < 0 || tempD > 255)
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
            Form fcg = mf.Tls.IsFormOpen("FormPIDGraph");

            if (fcg != null)
            {
                fcg.Focus();
                return;
            }

            //
            Form formG = new FormPIDGraph(this);
            formG.Show(this);
        }

        private void btnFan_Click(object sender, EventArgs e)
        {
            CurrentProduct.FanOn = !CurrentProduct.FanOn;
        }

        private void ckOnScreen_CheckedChanged(object sender, EventArgs e)
        {
            if (ckOnScreen.Checked)
            {
                ckBumpButtons.Checked = false;
                ckBumpButtons.Enabled = false;
            }
            else
            {
                ckBumpButtons.Enabled = true;
            }
            SetButtons(true);
        }

        private void ckBumpButtons_CheckChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void ckConstantUPM_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void ckOnScreen_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Show on Large Screen.";

            mf.Tls.ShowHelp(Message, "On Screen");
            hlpevent.Handled = true;
        }

        private void ckConstantUPM_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "UPM does not vary with the number of sections on or off.";

            mf.Tls.ShowHelp(Message, "Constant UPM");
            hlpevent.Handled = true;
        }

        private void ckDefault_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void ckDefault_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Product that is loaded at startup.";

            mf.Tls.ShowHelp(Message, "Default Product");
            hlpevent.Handled = true;
        }

        private void lb1_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Quantity units for product. ex: lbs, kgs";

            mf.Tls.ShowHelp(Message, "Quantity");
            hlpevent.Handled = true;
        }

        private void pnlMain_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ckVR_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbMaxRate_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbMinRate_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbMaxRate_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbMaxRate.Text, out tempD);
            using (var form = new FormNumeric(0, 100000, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbMaxRate.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void tbMinRate_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbMinRate.Text, out tempD);
            using (var form = new FormNumeric(0, 100000, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbMinRate.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void cbVR_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void cbShift_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtons(true);
            UpdateExample();
        }
    }
}