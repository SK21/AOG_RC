using AgOpenGPS;
using RateController.Classes;
using RateController.Language;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuRate : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private FormStart mf;

        public frmMenuRate(FormStart main, frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            mf = main;
            this.Tag = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
        }

        private void btnFan_Click(object sender, EventArgs e)
        {
            MainMenu.CurrentProduct.FanOn = !MainMenu.CurrentProduct.FanOn;
            UpdateFans();
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            MainMenu.ChangeProduct(MainMenu.CurrentProduct.ID - 1);
            UpdateForm();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                // save changes
                MainMenu.CurrentProduct.ProductName = tbProduct.Text;
                MainMenu.CurrentProduct.ControlType = ConvertValveIndex(ValveType.SelectedIndex);
                MainMenu.CurrentProduct.QuantityDescription = tbVolumeUnits.Text;

                if (MainMenu.CurrentProduct.ControlType == ControlTypeEnum.Fan)
                {
                    // set rate by fan
                    if (double.TryParse(tbTargetRPM.Text, out var rpm)) MainMenu.CurrentProduct.RateSet = rpm;
                    if (double.TryParse(tbCountsRPM.Text, out double cnts)) MainMenu.CurrentProduct.MeterCal = cnts;
                    MainMenu.CurrentProduct.CoverageUnits = 2;   // minutes
                }
                else
                {
                    // set rate by product
                    MainMenu.CurrentProduct.EnableProdDensity = CbUseProdDensity.Checked;
                    if (double.TryParse(lbBaseRate.Text, out double bs)) MainMenu.CurrentProduct.RateSet = bs;
                    if (double.TryParse(ProdDensity.Text, out double de)) MainMenu.CurrentProduct.ProdDensity = de;
                    if (double.TryParse(FlowCal.Text, out double fc)) MainMenu.CurrentProduct.MeterCal = fc;
                    MainMenu.CurrentProduct.CoverageUnits = Convert.ToByte(AreaUnits.SelectedIndex);
                }
                if (double.TryParse(tbAltRate.Text, out double ar)) MainMenu.CurrentProduct.RateAlt = ar;
                if (double.TryParse(TankSize.Text, out double tk)) MainMenu.CurrentProduct.TankSize = tk;
                if (double.TryParse(TankRemain.Text, out double tr)) MainMenu.CurrentProduct.TankStart = tr;

                string Title = "RC [" + Path.GetFileNameWithoutExtension(Properties.Settings.Default.FileName) + "]";
                MainMenu.CurrentProduct.Save();
                SetButtons(false);
                UpdateForm();
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmMenuRates/btnOK_Click: " + ex.Message);
            }
        }

        private void btnResetTank_Click(object sender, EventArgs e)
        {
            //check if window already exists
            Form fs = mf.Tls.IsFormOpen("frmResetQuantity");

            if (fs != null)
            {
                fs.Focus();
                return;
            }

            Form frm = new frmResetQuantity(mf);
            frm.ShowDialog();
            UpdateForm();
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            MainMenu.ChangeProduct(MainMenu.CurrentProduct.ID + 1);
            UpdateForm();
        }

        private void CbUseProdDensity_CheckedChanged(object sender, EventArgs e)
        {
            if (CbUseProdDensity.Checked)
            {
                ProdDensity.Enabled = true;
                //LabProdDensity.Enabled = true;
            }
            else
            {
                ProdDensity.Enabled = false;
                //LabProdDensity.Enabled = false;
            }

            SetButtons(true);
        }

        private void CbUseProdDensity_Click(object sender, EventArgs e)
        {
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
            using (var form = new FormNumeric(0.01, 16700, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    FlowCal.Text = form.ReturnValue.ToString("N3");
                }
            }
        }

        private void FlowCal_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(FlowCal.Text, out tempD);
            if (tempD < 0.01 || tempD > 16700)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void frmMenuRate_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
            timer1.Enabled = false;
        }

        private void frmMenuRate_Load(object sender, EventArgs e)
        {
            // menu 800,600
            // sub menu 540,630
            SetLanguage();
            MainMenu.MenuMoved += MyMenu_MenuMoved;
            MainMenu.ProductChanged += MainMenu_ProductChanged;
            this.Width = MainMenu.Width - 260;
            this.Height = MainMenu.Height - 50;
            btnOK.Left = this.Width - 84;
            btnOK.Top = this.Height - 84;
            btnCancel.Left = btnOK.Left - 78;
            btnCancel.Top = btnOK.Top;
            btnRight.Left = btnCancel.Left - 78;
            btnRight.Top = btnOK.Top;
            btnLeft.Left = btnRight.Left - 78;
            btnLeft.Top = btnOK.Top;
            btnResetTank.Left = btnLeft.Left - 78;
            btnResetTank.Top = btnOK.Top;
            PositionForm();
            MainMenu.StyleControls(this);
            lbProduct.Font = new Font(lbProduct.Font.FontFamily, 18, FontStyle.Underline);
            UpdateForm();
            timer1.Enabled = true;
        }

        private void lbBaseRate_Enter(object sender, EventArgs e)
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

        private void lbBaseRate_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(lbBaseRate.Text, out tempD);
            if (tempD < 0 || tempD > 50000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void MainMenu_ProductChanged(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void MyMenu_MenuMoved(object sender, EventArgs e)
        {
            PositionForm();
        }

        private void PositionForm()
        {
            this.Top = MainMenu.Top + 30;
            this.Left = MainMenu.Left + 246;
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

        private void SetButtons(bool Edited)
        {
            if (!Initializing)
            {
                if (Edited)
                {
                    btnCancel.Enabled = true;
                    btnLeft.Enabled = false;
                    btnRight.Enabled = false;
                    btnOK.Enabled = true;
                }
                else
                {
                    btnCancel.Enabled = false;
                    btnLeft.Enabled = true;
                    btnRight.Enabled = true;
                    btnOK.Enabled = false;
                }

                cEdited = Edited;
                this.Tag = cEdited;
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

        private void SetLanguage()
        {
            lb0.Text = Lang.lgProductName;
            lb5.Text = Lang.lgControlType;
            lb1.Text = Lang.lgQuantity;
            lb2.Text = Lang.lgCoverage;
            lbSensorCounts.Text = Lang.lgSensorCounts;
            LabProdDensity.Text = Lang.lgDensity;
            lbBaseRateDes.Text = Lang.lgBaseRate;
            lbAltRate.Text = Lang.lgAltRate;
            lb6.Text = Lang.lgTankSize;
            lbStartQuantity.Text = Lang.lgStartQuantity;

            ValveType.Items[0] = Lang.lgStandard;
            ValveType.Items[1] = Lang.lgComboClose;
            ValveType.Items[2] = Lang.lgMotor;
            ValveType.Items[3] = Lang.lgComboTimed;

            AreaUnits.Items[0] = Lang.lgAcres;
            AreaUnits.Items[1] = Lang.lgHectares;
            AreaUnits.Items[2] = Lang.lgMinute;
            AreaUnits.Items[3] = Lang.lgHour;
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

        private void tbProduct_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateFans();
        }

        private void UpdateFans()
        {
            double Target = MainMenu.CurrentProduct.TargetUPM();
            double Applied = MainMenu.CurrentProduct.UPMapplied();
            double RateError = 0;

            lbFanRPMvalue.Text = Applied.ToString("N0");

            if (Target > 0)
            {
                RateError = ((Applied - Target) / Target) * 100;
                bool IsNegative = RateError < 0;
                RateError = Math.Abs(RateError);
                if (RateError > 100) RateError = 100;
                if (IsNegative) RateError *= -1;
            }

            lbFanErrorValue.Text = RateError.ToString("N1");
            lbFanPWMvalue.Text = MainMenu.CurrentProduct.PWM().ToString("N0");

            // fan button
            if (MainMenu.CurrentProduct.FanOn)
            {
                btnFan.Image = Properties.Resources.FanOn;
                lbFanStarted.Image = Properties.Resources.On;
            }
            else
            {
                btnFan.Image = Properties.Resources.FanOff;
                lbFanStarted.Image = Properties.Resources.Off;
            }
        }

        private void UpdateForm()
        {
            Initializing = true;
            SetCalDescription();

            if (MainMenu.CurrentProduct.ControlType == ControlTypeEnum.Fan)
            {
                tbTargetRPM.Text = MainMenu.CurrentProduct.RateSet.ToString("N1");
                tbCountsRPM.Text = MainMenu.CurrentProduct.MeterCal.ToString("N3");
                UpdateFans();
            }
            else
            {
                lbBaseRate.Text = MainMenu.CurrentProduct.RateSet.ToString("N1");
                FlowCal.Text = MainMenu.CurrentProduct.MeterCal.ToString("N3");
            }

            tbProduct.Text = MainMenu.CurrentProduct.ProductName;
            tbVolumeUnits.Text = MainMenu.CurrentProduct.QuantityDescription;
            AreaUnits.SelectedIndex = MainMenu.CurrentProduct.CoverageUnits;
            CbUseProdDensity.Checked = MainMenu.CurrentProduct.EnableProdDensity;
            if (!CbUseProdDensity.Checked)
            { CbUseProdDensity_CheckedChanged(CbUseProdDensity, EventArgs.Empty); }
            ProdDensity.Text = MainMenu.CurrentProduct.ProdDensity.ToString("N1");
            tbAltRate.Text = MainMenu.CurrentProduct.RateAlt.ToString("N0");
            TankSize.Text = MainMenu.CurrentProduct.TankSize.ToString("N0");
            ValveType.SelectedIndex = ConvertControlType(MainMenu.CurrentProduct.ControlType);

            if (MainMenu.CurrentProduct.ID > mf.MaxProducts - 3)
            {
                // fans
                pnlFan.Visible = true;
                pnlFan.Left = 71;
                pnlFan.Top = 48;
                pnlMain.Visible = false;
                lbProduct.Text = "Fan " + (3 - (mf.MaxProducts - MainMenu.CurrentProduct.ID)).ToString();
            }
            else
            {
                pnlFan.Visible = false;
                pnlMain.Visible = true;
                lbProduct.Text = (MainMenu.CurrentProduct.ID + 1).ToString() + ". " + MainMenu.CurrentProduct.ProductName;
            }

            TankSize.Enabled = MainMenu.CurrentProduct.ControlType != ControlTypeEnum.MotorWeights;
            TankRemain.Enabled = MainMenu.CurrentProduct.ControlType != ControlTypeEnum.MotorWeights;
            btnResetTank.Enabled = MainMenu.CurrentProduct.ControlType != ControlTypeEnum.MotorWeights;
            TankRemain.Text = MainMenu.CurrentProduct.TankStart.ToString("N0");

            Initializing = false;
        }

        private void ValveType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtons(true);
            SetCalDescription();
            TankSize.Enabled = MainMenu.CurrentProduct.ControlType != ControlTypeEnum.MotorWeights;
            TankRemain.Enabled = MainMenu.CurrentProduct.ControlType != ControlTypeEnum.MotorWeights;
            btnResetTank.Enabled = MainMenu.CurrentProduct.ControlType != ControlTypeEnum.MotorWeights;
        }
    }
}