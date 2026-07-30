using AgOpenGPS;
using RateController.Classes;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuMode : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;

        public frmMenuMode(frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
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
                SaveAppMode(MainMenu.CurrentProduct);
                SetButtons(false);
                UpdateForm();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuMode/btnOk_Click: " + ex.Message);
            }
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            MainMenu.ChangeProduct(MainMenu.CurrentProduct.ID + 1);
            UpdateForm();
        }

        private void frmMenuMode_Activated(object sender, EventArgs e)
        {
            switch (this.Text)
            {
                case "Focused":
                    this.Text = "";
                    UpdateForm();
                    break;
            }
        }

        private void frmMenuMode_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
        }

        private void frmMenuMode_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainMenu.MenuMoved -= MainMenu_MenuMoved;
            MainMenu.ProductChanged -= MainMenu_ProductChanged;
            MainMenu.ProductEnabled -= MainMenu_ProductEnabled;
        }

        private void frmMenuMode_Load(object sender, EventArgs e)
        {
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            MainMenu.ProductChanged += MainMenu_ProductChanged;
            MainMenu.ProductEnabled += MainMenu_ProductEnabled;

            SubMenuLayout.SetFormLayout(this, MainMenu, null);
            btnOK.Left = this.Width - 84;
            btnOK.Top = this.Height - 84;
            btnCancel.Left = btnOK.Left - 78;
            btnCancel.Top = btnOK.Top;
            btnRight.Left = btnCancel.Left - 78;
            btnRight.Top = btnOK.Top;
            btnLeft.Left = btnRight.Left - 78;
            btnLeft.Top = btnOK.Top;
            MainMenu.StyleControls(this);
            SetLanguage();
            this.BackColor = Properties.Settings.Default.MainBackColour;
            PositionForm();
            lbProduct.Font = new Font(lbProduct.Font.FontFamily, 18, FontStyle.Underline);
            UpdateForm();
        }

        private void MainMenu_MenuMoved(object sender, EventArgs e)
        {
            PositionForm();
        }

        private void MainMenu_ProductChanged(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void MainMenu_ProductEnabled(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void PositionForm()
        {
            this.Top = MainMenu.Top + SubMenuLayout.TopOffset;
            this.Left = MainMenu.Left + SubMenuLayout.LeftOffset;
        }

        private void rbModeControlledUPM_CheckedChanged(object sender, EventArgs e)
        {
            SetEnabled();
            SetButtons(true);
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
            else if (rbModeManual.Checked)
            {
                Prd.AppMode = ApplicationMode.DocumentAppliedManual;
            }
            else
            {
                Prd.AppMode = ApplicationMode.DocumentTarget;
            }

            if (double.TryParse(tbUPM.Text, out double mu)) Prd.ManualUPM = mu;

            MainMenu.CurrentProduct.Save();
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
            bool Enabled = MainMenu.CurrentProduct.Enabled;

            rbModeApplied.Enabled = Enabled;
            rbModeConstant.Enabled = Enabled;
            rbModeControlledUPM.Enabled = Enabled;
            rbModeTarget.Enabled = Enabled;
            rbModeManual.Enabled = Enabled;

            gbManual.Enabled = Enabled && rbModeManual.Checked;
        }

        private void SetLanguage()
        {
        }

        private void UpdateForm()
        {
            Initializing = true;
            if (MainMenu.CurrentProduct.ID > Props.MaxProducts - 3)
            {
                // fans
                lbProduct.Text = MainMenu.CurrentProduct.ProductName;
            }
            else
            {
                lbProduct.Text = (MainMenu.CurrentProduct.ID + 1).ToString() + ". " + MainMenu.CurrentProduct.ProductName;
            }

            switch (MainMenu.CurrentProduct.AppMode)
            {
                case ApplicationMode.ConstantUPM:
                    rbModeConstant.Checked = true;
                    break;

                case ApplicationMode.DocumentApplied:
                    rbModeApplied.Checked = true;
                    break;

                case ApplicationMode.DocumentTarget:
                    rbModeTarget.Checked = true;
                    break;

                case ApplicationMode.DocumentAppliedManual:
                    rbModeManual.Checked = true;
                    break;

                default:
                    rbModeControlledUPM.Checked = true;
                    break;
            }

            tbUPM.Text = MainMenu.CurrentProduct.ManualUPM.ToString("N1");

            SetEnabled();

            Initializing = false;
        }

        private void tbUPM_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbUPM.Text, out tempD);
            using (var form = new FormNumeric(0, 50000, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbUPM.Text = form.ReturnValue.ToString("N1");

                    // The value is total output with everything running; flow is reported
                    // scaled to the width currently switched on (clsProduct.ManualUPMinUse).
                    // That only matches the machine if sections and dispensers line up.
                    Props.ShowMessage("Enter the total output of the machine with all sections on."
                        + "\r\n\r\nThere must be one section per dispenser, and each section's width must match"
                        + " the ground that dispenser covers. A section turned off then lowers the reported"
                        + " flow by that dispenser's share.", "Manual Rate", 15000);
                }
            }
        }

        private void tbUPM_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbUPM_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbUPM.Text, out tempD);
            if (tempD < 0 || tempD > 50000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            Props.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }
    }
}