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
        public clsProduct CurrentProduct;
        public FormStart mf;
        private bool FormEdited = false;
        private bool Initializing = false;
        private Label[] Sec;

        private int SelectedTab = 0;
        private TabPage tbTemp1;
        private TabPage tbTemp2;

        public FormSettings(FormStart CallingForm, int Page)
        {
            Initializing = true;
            InitializeComponent();

            #region // language

            lbProduct.Text = Lang.lgProduct;
            tcProducts.TabPages[0].Text = Lang.lgRate;
            tcProducts.TabPages[1].Text = Lang.lgControl;
            tcProducts.TabPages[3].Text = Lang.lgOptions;
            tcProducts.TabPages[5].Text = Lang.lgDiagnostics;

            lb0.Text = Lang.lgProductName;
            lb5.Text = Lang.lgControlType;
            lb1.Text = Lang.lgQuantity;
            lb2.Text = Lang.lgCoverage;
            lbSensorCounts.Text = Lang.lgSensorCounts;
            lbBaseRateDes.Text = Lang.lgBaseRate;
            lb6.Text = Lang.lgTankSize;
            btnResetTank.Text = Lang.lgStartQuantity;

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
            ValveType.Items[3] = Lang.lgComboTimed;

            AreaUnits.Items[0] = Lang.lgAcres;
            AreaUnits.Items[1] = Lang.lgHectares;
            AreaUnits.Items[2] = Lang.lgMinute;
            AreaUnits.Items[3] = Lang.lgHour;

            lbAltRate.Text = Lang.lgAltRate;
            lbVariableRate.Text = Lang.lgChannel;
            lbMaxRate.Text = Lang.lgMaxRate;
            lbMinRate.Text = Lang.lgMinRate;
            ckVR.Text = Lang.lgUseVR;

            lbProportional.Text = Lang.lgRateHigh;
            lbRateLow.Text = Lang.lgRateLow;
            lbThreshold.Text = Lang.lgThreshold;
            lbMax.Text = Lang.lgPWMmax;
            lbMin.Text = Lang.lgPWMmin;

            lbSensorID.Text = Lang.lgSensorID;

            grpMinUPM.Text = Lang.lgMinUPM;
            ckOffRate.Text = Lang.lgOffRate;
            ckDefault.Text = Lang.lgDefaultProduct;
            ckOnScreen.Text = Lang.lgOnScreen;

            label2.Text = Lang.lgWifiSignal;
            LabProdDensity.Text = Lang.lgDensity;

            lbAcres1.Text = Lang.lgCoverage + " 1";
            lbAcres2.Text = Lang.lgCoverage + " 2";
            lbGallons1.Text = Lang.lgQuantity + " 1";
            lbGallons2.Text = Lang.lgQuantity + " 2";

            #endregion // language

            mf = CallingForm;

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
                if (mf.Tls.ReadOnly)
                {
                    mf.Tls.ShowHelp("File is read only.", "Help", 5000, false, false, true);
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
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
        }

        private void btnFan_Click(object sender, EventArgs e)
        {
            CurrentProduct.FanOn = !CurrentProduct.FanOn;
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

        private void cbVR_SelectedIndexChanged_1(object sender, EventArgs e)
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

        private void ckBumpButtons_CheckChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void ckBumpButtons_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Display rate UP/Down arrows.";

            mf.Tls.ShowHelp(Message, "Bump Buttons");
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

        private void ckOnScreen_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Show on Large Screen.";

            mf.Tls.ShowHelp(Message, "On Screen");
            hlpevent.Handled = true;
        }

        private void ckQuanitiy2_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void ckScale_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void ckVR_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private int ConvertControlType(ControlTypeEnum Type)
        {
            // convert control type to valve type index
            int Result;
            switch (Type)
            {
                case ControlTypeEnum.ComboClose:
                    Result = 1;
                    break;

                case ControlTypeEnum.Motor:
                    Result = 2;
                    break;

                case ControlTypeEnum.ComboCloseTimed:
                    Result = 3;
                    break;

                default:
                    Result = 0;
                    break;
            }
            return Result;
        }

        private ControlTypeEnum ConvertValveIndex(int ID)
        {
            // convert valve type index to ControlTypeEnum
            // valve types: Standard Valve, Fast Close Valve, Motor, Combo Timed
            ControlTypeEnum Result;
            switch (ID)
            {
                case 1:
                    Result = ControlTypeEnum.ComboClose;
                    break;

                case 2:
                    Result = ControlTypeEnum.Motor;
                    break;

                case 3:
                    Result = ControlTypeEnum.ComboCloseTimed;
                    break;

                default:
                    Result = ControlTypeEnum.Valve;
                    break;
            }
            return Result;
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
            mf.Tls.SaveProperty("SettingsSelectedTab", tcProducts.SelectedIndex.ToString());
            timer1.Enabled = false;
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            if (int.TryParse(mf.Tls.LoadProperty("SettingsSelectedTab"), out int TB)) SelectedTab = TB;
            tcProducts.SelectedIndex = SelectedTab;
            timer1.Enabled = true;
            UpdateForm();
        }

        private void grpSections_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            mf.Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }

        private void HShigh_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Fast rate adjustment above threshold.";

            mf.Tls.ShowHelp(Message, "Adjust High");
            hlpevent.Handled = true;
        }

        private void HShigh_ValueChanged_1(object sender, EventArgs e)
        {
            SetButtons(true);
            UpdateControlDisplay();
        }

        private void HSlow_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Slow rate of adjustment below threshold.";

            mf.Tls.ShowHelp(Message, "Adjust Low");
            hlpevent.Handled = true;
        }

        private void HSmax_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Maximum power sent to the valve/motor.";

            mf.Tls.ShowHelp(Message, "Maximum power.");
            hlpevent.Handled = true;
        }

        private void HSmin_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "The minimum power sent to the valve/motor. The power needed to start to make the" +
    " valve/motor move.";

            mf.Tls.ShowHelp(Message, "Minimum power");
            hlpevent.Handled = true;
        }

        private void HSthreshold_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "The % of rate error where adjustment changes from fast to slow.";

            mf.Tls.ShowHelp(Message, "Adjust Threshold");
            hlpevent.Handled = true;
        }

        private void label24_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Enter the counts per revolution so RPM can be calculated.";

            mf.Tls.ShowHelp(Message, "Counts/Rev");
            hlpevent.Handled = true;
        }

        private void label26_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "The unique flow sensor ID within each arduino module.";

            mf.Tls.ShowHelp(Message, "Sensor ID");
            hlpevent.Handled = true;
        }

        private void lb1_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Quantity units for product. ex: lbs, kgs";

            mf.Tls.ShowHelp(Message, "Quantity");
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
                "3 - Motor, vary motor speed to control rate \n" +
                "4 - Combo Timed, use adjust/pause time for control";

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
            HShigh.Value = 70;
            HSlow.Value = 7;
            HSthreshold.Value = 50;
            HSmax.Value = 100;
            HSmin.Value = 7;
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

            // flow control
            HShigh.Value = 15;
            HSlow.Value = 10;
            HSthreshold.Value = 30;
            HSmax.Value = 100;
            HSmin.Value = 1;

            tbSenID.Text = CurrentProduct.SensorID.ToString();

            tbMinUPM.Text = CurrentProduct.MinUPM.ToString("N1");
            tbUPMspeed.Text = CurrentProduct.MinUPMbySpeed.ToString("N1");
            rbUPMSpeed.Checked = CurrentProduct.UseMinUPMbySpeed;

            ckOffRate.Checked = CurrentProduct.UseOffRateAlarm;
            tbOffRate.Text = CurrentProduct.OffRateSetting.ToString("N0");

            ckOnScreen.Checked = CurrentProduct.OnScreen;
            ckBumpButtons.Checked = CurrentProduct.BumpButtons;

            SetAppMode(CurrentProduct);
        }

        private void pnlMain_Paint(object sender, PaintEventArgs e)
        {
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

        private void rbUPMFixed_Click(object sender, EventArgs e)
        {
            tbUPMspeed.Text = "0.0";
            SetButtons(true);
        }

        private void rbUPMSpeed_Click(object sender, EventArgs e)
        {
            tbMinUPM.Text = "0.0";
            SetButtons(true);
        }

        private void rbUPMSpeed_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Speed used to calculate minimum UPM based on application rate.";

            mf.Tls.ShowHelp(Message, "Minimum UPM using speed");
            hlpevent.Handled = true;
        }

        private void SaveAppMode(clsProduct Prd)
        {
            if (rbModeControlledUPM.Checked)
            {
                Prd.AppMode = ApplicationMode.ControlledUPM;
            }
            else if (rbModeConstant.Checked)
            {
                Prd.AppMode = ApplicationMode.ConstantUPM;
            }
            else if (rbModeApplied.Checked)
            {
                Prd.AppMode = ApplicationMode.DocumentApplied;
            }
            else
            {
                Prd.AppMode = ApplicationMode.DocumentTarget;
            }
        }

        private void SaveData()
        {
            if (ckArea1.Checked)
            {
                CurrentProduct.ResetCoverage();
                ckArea1.Checked = false;
            }

            if (ckArea2.Checked)
            {
                CurrentProduct.ResetCoverage2();
                ckArea2.Checked = false;
            }

            if (ckQuantity1.Checked)
            {
                CurrentProduct.ResetApplied();
                ckQuantity1.Checked = false;
            }

            if (ckQuantity2.Checked)
            {
                CurrentProduct.ResetApplied2();
                ckQuantity2.Checked = false;
            }

            if (ckHours1.Checked)
            {
                CurrentProduct.ResetHours1();
                ckHours1.Checked = false;
            }

            if (ckHours2.Checked)
            {
                CurrentProduct.ResetHours2();
                ckHours2.Checked = false;
            }
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

            // flow control
            CurrentProduct.HighAdjust = HShigh.Value;
            CurrentProduct.LowAdjust = HSlow.Value;
            CurrentProduct.Threshold = HSthreshold.Value;
            CurrentProduct.MaxAdjust = HSmax.Value;
            CurrentProduct.MinAdjust = HSmin.Value;

            int.TryParse(tbCountsRev.Text, out tempInt);
            CurrentProduct.CountsRev = tempInt;

            CurrentProduct.UseMinUPMbySpeed = rbUPMSpeed.Checked;

            if (double.TryParse(tbMinUPM.Text, out double mu)) CurrentProduct.MinUPM = mu;
            if (double.TryParse(tbUPMspeed.Text, out double sp)) CurrentProduct.MinUPMbySpeed = sp;

            CurrentProduct.UseOffRateAlarm = ckOffRate.Checked;

            byte.TryParse(tbOffRate.Text, out tempB);
            CurrentProduct.OffRateSetting = tempB;

            CurrentProduct.ManualPWM = tempB;
            SaveAppMode(CurrentProduct);

            CurrentProduct.OnScreen = ckOnScreen.Checked;
            CurrentProduct.BumpButtons = ckBumpButtons.Checked;

            CurrentProduct.Save();

            if (ckDefault.Checked) mf.DefaultProduct = CurrentProduct.ID;

            mf.SetScale(CurrentProduct.ID, ckScale.Checked);

            SaveData();
        }

        private void SetAppMode(clsProduct Prd)
        {
            switch (Prd.AppMode)
            {
                case ApplicationMode.ControlledUPM:
                    rbModeControlledUPM.Checked = true;
                    break;

                case ApplicationMode.ConstantUPM:
                    rbModeConstant.Checked = true;
                    break;

                case ApplicationMode.DocumentApplied:
                    rbModeApplied.Checked = true;
                    break;

                case ApplicationMode.DocumentTarget:
                    rbModeTarget.Checked = true;
                    break;

                default:
                    break;
            }
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
            if (ConvertValveIndex(ValveType.SelectedIndex) == ControlTypeEnum.MotorWeights)
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

                foreach (TabPage tb in tcProducts.TabPages)
                {
                    tb.BackColor = Properties.Settings.Default.DayColour;
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

                foreach (TabPage tb in tcProducts.TabPages)
                {
                    tb.BackColor = Properties.Settings.Default.NightColour;
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

        private void SetFanDisplay(bool IsFan)
        {
            ckBumpButtons.Visible = !IsFan;
            ckBumpButtons.Enabled = !IsFan;
            grpMinUPM.Visible = !IsFan;

            if (IsFan)
            {
                ckDefault.Visible = false;
                if (tcProducts.TabCount > 5)
                {
                    // remove tabs
                    tbTemp1 = tcProducts.TabPages["tbVR"];
                    tbTemp2 = tcProducts.TabPages["tbDiagnostics"];
                    tcProducts.Controls.Remove(tbTemp1);
                    tcProducts.Controls.Remove(tbTemp2);

                    ckOffRate.Location = new Point(29, 195);
                    tbOffRate.Location = new Point(169, 196);
                    label28.Location = new Point(208, 200);
                }
            }
            else
            {
                ckDefault.Visible = true;
                if (tcProducts.TabCount < 7)
                {
                    // add back the removed tabs
                    tcProducts.TabPages.Insert(2, tbTemp1);
                    tcProducts.TabPages.Insert(5, tbTemp2);

                    ckOffRate.Location = new Point(29, 323);
                    tbOffRate.Location = new Point(169, 324);
                    label28.Location = new Point(208, 328);
                }
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

        private void SetModuleIndicator()
        {
            if (mf.Products.Item(CurrentProduct.ID).RateSensor.Connected())
            {
                ModuleIndicator.Image = Properties.Resources.On;
            }
            else
            {
                ModuleIndicator.Image = Properties.Resources.Off;
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
            using (var form = new FormNumeric(0, 7, tempInt))
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

        private void tbMaxRate_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
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

        private void tbMinRate_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
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

        private void tbUPMspeed_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbUPMspeed.Text, out tempD);
            using (var form = new FormNumeric(0, 30, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbUPMspeed.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void tbUPMspeed_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Speed to set minimum flow rate for acceptable application.";

            mf.Tls.ShowHelp(Message, "Minimum UPM Speed");
            hlpevent.Handled = true;
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
            UpdateData();
        }

        private void UpdateControlDisplay()
        {
            lbHigh.Text = HShigh.Value.ToString("N0");
            lbLow.Text = HSlow.Value.ToString("N0");
            lbThresholdValue.Text = HSthreshold.Value.ToString("N0");
            lbMaxValue.Text = HSmax.Value.ToString("N0");
            lbMinValue.Text = HSmin.Value.ToString("N0");
        }

        private void UpdateData()
        {
            lbArea1.Text = CurrentProduct.CurrentCoverage().ToString("N1");
            lbArea2.Text = CurrentProduct.CurrentCoverage2().ToString("N1");
            lbQuantity1.Text = CurrentProduct.UnitsApplied().ToString("N1");
            lbQuantity2.Text = CurrentProduct.UnitsApplied2().ToString("N1");

            lbHours1value.Text = CurrentProduct.Hours1.ToString("N2");
            lbHours2value.Text = CurrentProduct.Hours2.ToString("N2");
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
            wifiBar.Value = mf.ModulesStatus.WifiStrength(CurrentProduct.ModuleID);

            // product name
            if (mf.SimMode != SimType.Sim_None)
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
                SetFanDisplay(true);
            }
            else if (CurrentProduct.ID == mf.MaxProducts - 2)
            {
                lbProduct.Text = "Fan 1";
                SetFanDisplay(true);
            }
            else
            {
                lbProduct.Text = (CurrentProduct.ID + 1).ToString() + ". " + CurrentProduct.ProductName;
                SetFanDisplay(false);
            }

            if (mf.SimMode != SimType.Sim_None)
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

            UpdateOnTypeChange();

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

            ckArea1.Checked = false;
            ckArea2.Checked = false;
            ckQuantity1.Checked = false;
            ckQuantity2.Checked = false;
            ckHours1.Checked = false;
            ckHours2.Checked = false;

            lbAcres1.Text = "*" + CurrentProduct.CoverageDescription() + " 1";
            lbAcres2.Text = CurrentProduct.CoverageDescription() + " 2";
            lbGallons1.Text = "*" + CurrentProduct.QuantityDescription + " 1";
            lbGallons2.Text = CurrentProduct.QuantityDescription + " 2";

            rbUPMSpeed.Checked = CurrentProduct.UseMinUPMbySpeed;
            rbUPMFixed.Checked = !CurrentProduct.UseMinUPMbySpeed;

            ckScale.Checked = mf.ShowScale(CurrentProduct.ID);
            ckScale.Visible = (CurrentProduct.ID < 4);

            HShigh.Value = CurrentProduct.HighAdjust;
            HSlow.Value = CurrentProduct.LowAdjust;
            HSthreshold.Value = CurrentProduct.Threshold;
            HSmax.Value = CurrentProduct.MaxAdjust;
            HSmin.Value = CurrentProduct.MinAdjust;

            UpdateControlDisplay();
            Initializing = false;
        }

        private void UpdateOnTypeChange()
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
    }
}