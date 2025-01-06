using AgOpenGPS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuSettings : Form
    {
        private bool cEdited;
        private bool HelpMode = false;
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

        public bool Edited
        { get { return cEdited; } }

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
            btnHelp.Left = btnLeft.Left - 78;
            btnHelp.Top = btnOK.Top;
            PositionForm();
            MainMenu.StyleControls(this);
            lbProduct.Font = new Font(lbProduct.Font.FontFamily, 18, FontStyle.Underline);
            timer1.Enabled = true;
            UpdateForm();
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

        private void PositionForm()
        {
            this.Top = MainMenu.Top + 30;
            this.Left = MainMenu.Left + 246;
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            SetModuleIndicator();
        }

        private void UpdateForm()
        {
            Initializing = true;

            if (MainMenu.CurrentProduct.ID > mf.MaxProducts - 3)
            {
                // fans
                lbProduct.Text = "Fan " + (3 - (mf.MaxProducts - MainMenu.CurrentProduct.ID)).ToString();
                grpMinUPM.Visible = false;
                ckDefault.Visible = false;
                ckBumpButtons.Visible = false;
                ckScale.Visible = false;
            }
            else
            {
                lbProduct.Text = (MainMenu.CurrentProduct.ID + 1).ToString() + ". " + MainMenu.CurrentProduct.ProductName;
                grpMinUPM.Visible = true;
                ckDefault.Visible = true;
                ckBumpButtons.Visible = true;
                ckScale.Visible = true;
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
            SetModuleIndicator();

            Initializing = false;
        }

        private void frmMenuSettings_Activated(object sender, EventArgs e)
        {
            UpdateForm();
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

        private void tbConID_Click(object sender, EventArgs e)
        {
            if (HelpMode)
            {
                string Message = "The unique ID of each arduino module.";
                mf.Tls.ShowHelp(Message, "Module ID");
                btnHelp.PerformClick();
            }
            else
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

        private void tbSenID_Click(object sender, EventArgs e)
        {
            if (HelpMode)
            {
                string Message = "The unique flow sensor ID within each arduino module.";
                mf.Tls.ShowHelp(Message, "Sensor ID");
                btnHelp.PerformClick();
            }
            else
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
    }
}