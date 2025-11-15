using AgOpenGPS;
using RateController.Classes;
using RateController.Language;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuSettings : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private FormStart mf;

        public frmMenuSettings(FormStart main, frmMenu menu)
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

        private void btnLeft_Click(object sender, EventArgs e)
        {
            MainMenu.ChangeProduct(MainMenu.CurrentProduct.ID - 1);
            UpdateForm();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                byte ModID = 0;
                byte SenID = 0;
                byte.TryParse(tbConID.Text, out ModID);
                byte.TryParse(tbSenID.Text, out SenID);

                if (mf.Products.UniqueModSen(ModID, SenID, MainMenu.CurrentProduct.ID))
                {
                    MainMenu.CurrentProduct.ModuleID = ModID;
                    MainMenu.CurrentProduct.SensorID = SenID;

                    if (double.TryParse(tbMinUPM.Text, out double mu)) MainMenu.CurrentProduct.MinUPM = mu;
                    if (double.TryParse(tbUPMspeed.Text, out double sp)) MainMenu.CurrentProduct.MinUPMbySpeed = sp;
                    if (ckDefault.Checked) Props.DefaultProduct = MainMenu.CurrentProduct.ID;
                    MainMenu.CurrentProduct.BumpButtons = ckBumpButtons.Checked;
                    mf.SetScale(MainMenu.CurrentProduct.ID, ckScale.Checked);
                    MainMenu.CurrentProduct.UseOffRateAlarm = ckOffRate.Checked;
                    if (byte.TryParse(tbOffRate.Text, out byte off)) MainMenu.CurrentProduct.OffRateSetting = off;
                    MainMenu.CurrentProduct.UseMinUPMbySpeed = rbUPMSpeed.Checked;
                    MainMenu.CurrentProduct.Enabled = ckEnabled.Checked;

                    MainMenu.CurrentProduct.Save();
                    SetButtons(false);
                    UpdateForm();
                    Props.RaiseProductSettingsChanged();
                }
                else
                {
                    Props.ShowMessage("Invalid or duplicate sensor ID.", "Help", 5000);
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuSettings/btnOk_Click: " + ex.Message);
            }
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            MainMenu.ChangeProduct(MainMenu.CurrentProduct.ID + 1);
            UpdateForm();
        }

        private void ckDefault_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void ckEnabled_CheckedChanged(object sender, EventArgs e)
        {
            SetEnabled();
            SetButtons(true);
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

        private void frmMenuSettings_Activated(object sender, EventArgs e)
        {
            switch (this.Text)
            {
                case "Focused":
                    this.Text = "";
                    UpdateForm();
                    break;
            }
        }

        private void frmMenuSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
            timer1.Enabled = false;
        }

        private void frmMenuSettings_Load(object sender, EventArgs e)
        {
            SubMenuLayout.SetFormLayout(this, MainMenu, null);
            btnOK.Left = this.Width - 84;
            btnOK.Top = this.Height - 84;
            btnCancel.Left = btnOK.Left - 78;
            btnCancel.Top = btnOK.Top;
            btnRight.Left = btnCancel.Left - 78;
            btnRight.Top = btnOK.Top;
            btnLeft.Left = btnRight.Left - 78;
            btnLeft.Top = btnOK.Top;
            PositionForm();
            MainMenu.StyleControls(this);
            //SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            MainMenu.ProductChanged += MainMenu_ProductChanged;
            lbProduct.Font = new Font(lbProduct.Font.FontFamily, 18, FontStyle.Underline);
            timer1.Enabled = true;
            UpdateForm();
            SetLanguage();
        }

        private void grpMinUPM_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            mf.Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }

        private void grpSensor_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            mf.Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }

        private void MainMenu_MenuMoved(object sender, EventArgs e)
        {
            PositionForm();
        }

        private void MainMenu_ProductChanged(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void PositionForm()
        {
            this.Top = MainMenu.Top + SubMenuLayout.TopOffset;
            this.Left = MainMenu.Left + SubMenuLayout.LeftOffset;
        }

        private void rbUPMFixed_CheckedChanged(object sender, EventArgs e)
        {
            tbUPMspeed.Text = "0.0";
            SetButtons(true);
        }

        private void rbUPMSpeed_CheckedChanged(object sender, EventArgs e)
        {
            tbMinUPM.Text = "0.0";
            SetButtons(true);
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

        private void SetEnabled()
        {
            bool Enabled = ckEnabled.Checked;

            grpSensor.Enabled = Enabled;
            grpMinUPM.Enabled = Enabled;
            ckOffRate.Enabled = Enabled;
            tbOffRate.Enabled = Enabled;
            ckDefault.Enabled = Enabled;
            ckBumpButtons.Enabled = Enabled;
            ckScale.Enabled = Enabled;
        }

        private void SetLanguage()
        {
            grpSensor.Text = Lang.lgSensorLocation;
            grpMinUPM.Text = Lang.lgMinUPM;
            lbModuleID.Text = Lang.lgModuleID;
            lbSensorID.Text = Lang.lgSensorID;
            rbUPMFixed.Text = Lang.lgUPMFixed;
            rbUPMSpeed.Text = Lang.lgUPMSpeed;
            ckDefault.Text = Lang.lgDefaultProduct;
            ckBumpButtons.Text = Lang.lgBumpButtons;
            ckOffRate.Text = Lang.lgOffRate;
            ckScale.Text = Lang.lgScaleWeight;
        }

        private void SetModuleIndicator()
        {
            if (mf.Products.Item(MainMenu.CurrentProduct.ID).RateSensorData.Connected())
            {
                ModuleIndicator.Image = Properties.Resources.On;
            }
            else
            {
                ModuleIndicator.Image = Properties.Resources.Off;
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
            if (tempInt < 0 || tempInt > 7)
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            SetModuleIndicator();
        }

        private void UpdateForm()
        {
            Initializing = true;

            if (!MainMenu.CurrentProduct.Enabled) mf.Products.SetEnabledDefault();
            ckEnabled.Checked = MainMenu.CurrentProduct.Enabled;

            SetModuleIndicator();
            if (MainMenu.CurrentProduct.ID > Props.MaxProducts - 3)
            {
                // fans
                lbProduct.Text = "Fan " + (3 - (Props.MaxProducts - MainMenu.CurrentProduct.ID)).ToString();
                grpMinUPM.Visible = false;
                ckDefault.Visible = false;
                ckBumpButtons.Visible = false;
                ckScale.Visible = false;

                ckOffRate.Left = 180;
                ckOffRate.Top = 290;

                tbOffRate.Left = ckOffRate.Left + 149;
                tbOffRate.Top = ckOffRate.Top + 3;

                lbPercent.Left = ckOffRate.Left + 200;
                lbPercent.Top = ckOffRate.Top + 7;
            }
            else
            {
                // products
                lbProduct.Text = (MainMenu.CurrentProduct.ID + 1).ToString() + ". " + MainMenu.CurrentProduct.ProductName;
                grpMinUPM.Visible = true;
                ckDefault.Visible = true;
                ckBumpButtons.Visible = true;
                ckScale.Visible = true;

                ckOffRate.Left = 66;
                ckOffRate.Top = 465;

                tbOffRate.Left = ckOffRate.Left + 149;
                tbOffRate.Top = ckOffRate.Top + 3;

                lbPercent.Left = ckOffRate.Left + 200;
                lbPercent.Top = ckOffRate.Top + 7;

                ckScale.Checked = mf.ShowScale(MainMenu.CurrentProduct.ID);
            }

            rbUPMSpeed.Checked = MainMenu.CurrentProduct.UseMinUPMbySpeed;
            rbUPMFixed.Checked = !MainMenu.CurrentProduct.UseMinUPMbySpeed;
            ckDefault.Checked = (Props.DefaultProduct == MainMenu.CurrentProduct.ID);
            ckBumpButtons.Checked = MainMenu.CurrentProduct.BumpButtons;
            ckOffRate.Checked = MainMenu.CurrentProduct.UseOffRateAlarm;
            tbOffRate.Text = MainMenu.CurrentProduct.OffRateSetting.ToString("N0");
            tbMinUPM.Text = MainMenu.CurrentProduct.MinUPM.ToString("N1");
            tbUPMspeed.Text = MainMenu.CurrentProduct.MinUPMbySpeed.ToString("N1");
            ckOffRate.Checked = MainMenu.CurrentProduct.UseOffRateAlarm;

            if (MainMenu.CurrentProduct.Enabled)
            {
                tbConID.Text = MainMenu.CurrentProduct.ModuleID.ToString();
                tbSenID.Text = MainMenu.CurrentProduct.SensorID.ToString();
            }
            else
            {
                tbConID.Text = "";
                tbSenID.Text = "";
            }

            Initializing = false;
        }
    }
}