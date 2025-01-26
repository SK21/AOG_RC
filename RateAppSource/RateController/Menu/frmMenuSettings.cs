using AgOpenGPS;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RateController.Language;

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
                    byte.TryParse(tbConID.Text, out byte tmp1);
                    byte.TryParse(tbSenID.Text, out byte tmp2);
                    MainMenu.CurrentProduct.ChangeID(tmp1, tmp2);
                    if (double.TryParse(tbMinUPM.Text, out double mu)) MainMenu.CurrentProduct.MinUPM = mu;
                    if (double.TryParse(tbUPMspeed.Text, out double sp)) MainMenu.CurrentProduct.MinUPMbySpeed = sp;
                    if (ckDefault.Checked) mf.DefaultProduct = MainMenu.CurrentProduct.ID;
                    MainMenu.CurrentProduct.OnScreen = ckOnScreen.Checked;
                    MainMenu.CurrentProduct.BumpButtons = ckBumpButtons.Checked;
                    mf.SetScale(MainMenu.CurrentProduct.ID, ckScale.Checked);
                    MainMenu.CurrentProduct.UseOffRateAlarm = ckOffRate.Checked;
                    if (byte.TryParse(tbOffRate.Text, out byte off)) MainMenu.CurrentProduct.OffRateSetting = off;
                    MainMenu.CurrentProduct.UseMinUPMbySpeed = rbUPMSpeed.Checked;

                    MainMenu.CurrentProduct.Save();
                    SetButtons(false);
                    UpdateForm();
                }
                else
                {
                    mf.Tls.ShowMessage("Module ID / Sensor ID pair must be unique.", "Help", 3000);
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmMenuSettings/btnOk_Click: " + ex.Message);
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
            mf.Tls.SaveFormData(this);
            timer1.Enabled = false;
        }

        private void frmMenuSettings_Load(object sender, EventArgs e)
        {
            // menu 800,600
            // sub menu 540,630
            //SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            MainMenu.ProductChanged += MainMenu_ProductChanged;
            mf.Tls.LoadFormData(this, "", false);
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
            PositionForm();
            MainMenu.StyleControls(this);
            lbProduct.Font = new Font(lbProduct.Font.FontFamily, 18, FontStyle.Underline);
            timer1.Enabled = true;
            UpdateForm();
            SetLanguage();
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
            ckOnScreen.Text = Lang.lgOnScreen;
            ckScale.Text = Lang.lgScaleWeight;
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
            this.Top = MainMenu.Top + 30;
            this.Left = MainMenu.Left + 246;
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

        private void SetModuleIndicator()
        {
            if (mf.Products.Item(MainMenu.CurrentProduct.ID).RateSensor.Connected())
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
            if (tempInt < 0 || tempInt > 15)
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

            SetModuleIndicator();
            if (MainMenu.CurrentProduct.ID > mf.MaxProducts - 3)
            {
                // fans
                lbProduct.Text = "Fan " + (3 - (mf.MaxProducts - MainMenu.CurrentProduct.ID)).ToString();
                grpMinUPM.Visible = false;
                ckDefault.Visible = false;
                ckBumpButtons.Visible = false;
                ckScale.Visible = false;

                ckOnScreen.Left = 184;
                ckOnScreen.Top = 193;
                ckOffRate.Left = 122;
                ckOffRate.Top = 253;
                tbOffRate.Left = 314;
                tbOffRate.Top = 256;
                lbPercent.Left = 353;
                lbPercent.Top = 259;
            }
            else
            {
                lbProduct.Text = (MainMenu.CurrentProduct.ID + 1).ToString() + ". " + MainMenu.CurrentProduct.ProductName;
                grpMinUPM.Visible = true;
                ckDefault.Visible = true;
                ckBumpButtons.Visible = true;
                ckScale.Visible = true;

                ckOnScreen.Left = 305;
                ckOnScreen.Top = 344;
                ckOffRate.Left = 75;
                ckOffRate.Top = 474;
                tbOffRate.Left = 224;
                tbOffRate.Top = 477;
                lbPercent.Left = 275;
                lbPercent.Top = 480;
                ckScale.Checked = mf.ShowScale(MainMenu.CurrentProduct.ID);
            }
            rbUPMSpeed.Checked = MainMenu.CurrentProduct.UseMinUPMbySpeed;
            rbUPMFixed.Checked = !MainMenu.CurrentProduct.UseMinUPMbySpeed;
            ckDefault.Checked = (mf.DefaultProduct == MainMenu.CurrentProduct.ID);
            ckOnScreen.Checked = MainMenu.CurrentProduct.OnScreen;
            ckBumpButtons.Checked = MainMenu.CurrentProduct.BumpButtons;
            ckOffRate.Checked = MainMenu.CurrentProduct.UseOffRateAlarm;
            tbOffRate.Text = MainMenu.CurrentProduct.OffRateSetting.ToString("N0");
            tbMinUPM.Text = MainMenu.CurrentProduct.MinUPM.ToString("N1");
            tbUPMspeed.Text = MainMenu.CurrentProduct.MinUPMbySpeed.ToString("N1");

            string tmp = MainMenu.CurrentProduct.ModuleID.ToString();
            if (tmp == "99") tmp = "";
            tbConID.Text = tmp;

            ckOffRate.Checked = MainMenu.CurrentProduct.UseOffRateAlarm;
            tbSenID.Text = MainMenu.CurrentProduct.SensorID.ToString();

            Initializing = false;
        }
    }
}