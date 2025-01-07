using AgOpenGPS;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuDisplay : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private FormStart mf;

        public frmMenuDisplay(FormStart main, frmMenu menu)
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

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                mf.UseTransparent = ckTransparent.Checked;
                mf.ShowPressure = ckPressure.Checked;
                if (double.TryParse(tbPressureCal.Text, out double Pressure)) mf.PressureCal = Pressure;
                if (double.TryParse(tbPressureOffset.Text, out double PresOff)) mf.PressureOffset = PresOff;
                mf.UseLargeScreen = ckLargeScreen.Checked;
                if (ckSingle.Checked) mf.SwitchScreens(true);
                SetButtons(false);
                if (ckReset.Checked) mf.Products.Load(true);
                UpdateForm();
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmMenuDisplay/btnOk_Click: " + ex.Message);
            }
        }

        private void ckLargeScreen_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void ckReset_CheckedChanged(object sender, EventArgs e)
        {
            if (ckReset.Checked)
            {
                var Hlp = new frmMsgBox(mf, "Confirm reset all products to default values?", "Reset", true);
                Hlp.TopMost = true;

                Hlp.ShowDialog();
                bool Result = Hlp.Result;
                Hlp.Close();
                if (Result)
                {
                    SetButtons(true);
                }
                else
                {
                    ckReset.Checked = false;
                }
            }
        }

        private void ckSingle_CheckedChanged(object sender, EventArgs e)
        {
            if (ckSingle.Checked) SetButtons(true);
        }

        private void frmMenuDisplay_FormClosed(object sender, FormClosedEventArgs e)
        {
            mf.Tls.SaveFormData(this);
        }

        private void frmMenuDisplay_Load(object sender, EventArgs e)
        {
            // menu 800,600
            // sub menu 540,630
            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            mf.Tls.LoadFormData(this, "", false);
            this.BackColor = Properties.Settings.Default.BackColour;
            this.Width = MainMenu.Width - 260;
            this.Height = MainMenu.Height - 50;
            btnOK.Left = this.Width - 84;
            btnOK.Top = this.Height - 84;
            btnCancel.Left = btnOK.Left - 78;
            btnCancel.Top = btnOK.Top;
            MainMenu.StyleControls(this);
            PositionForm();
            UpdateForm();
        }

        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
            mf.Tls.DrawGroupBox((GroupBox)sender, e.Graphics, this.BackColor, Color.Black, Color.Blue);
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

        private void SetButtons(bool Edited)
        {
            if (!Initializing)
            {
                if (Edited)
                {
                    btnCancel.Enabled = true;
                    btnOK.Enabled = true;
                }
                else
                {
                    btnCancel.Enabled = false;
                    btnOK.Enabled = false;
                }

                cEdited = Edited;
                this.Tag = cEdited;
            }
        }

        private void SetLanguage()
        {
        }

        private void tbPressureCal_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbPressureCal.Text, out tempD);
            using (var form = new FormNumeric(0, 10000, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbPressureCal.Text = form.ReturnValue.ToString("N1");
                }
            }
        }

        private void tbPressureCal_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbPressureCal.Text, out tempD);
            if (tempD < 0 || tempD > 10000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbPressureOffset_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbPressureOffset.Text, out tempD);
            using (var form = new FormNumeric(-10000, 10000, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbPressureOffset.Text = form.ReturnValue.ToString("N1");
                }
            }
        }

        private void tbPressureOffset_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbPressureOffset.Text, out tempD);
            if (tempD < -10000 || tempD > 10000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void UpdateForm()
        {
            Initializing = true;

            ckTransparent.Checked = mf.UseTransparent;
            ckPressure.Checked = mf.ShowPressure;
            ckLargeScreen.Checked = mf.UseLargeScreen;
            tbPressureOffset.Text = mf.PressureOffset.ToString("N1");
            tbPressureCal.Text = mf.PressureCal.ToString("N1");
            ckReset.Checked = false;
            ckSingle.Checked = false;

            Initializing = false;
        }
    }
}