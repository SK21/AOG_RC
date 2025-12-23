using NetTopologySuite.Planargraph;
using RateController.Classes;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuMode : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;

        public frmMenuMode( frmMenu menu)
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

        private void frmMenuMode_Load(object sender, EventArgs e)
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
            MainMenu.StyleControls(this);
            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            MainMenu.ProductChanged += MainMenu_ProductChanged;
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

        private void PositionForm()
        {
            this.Top = MainMenu.Top + SubMenuLayout.TopOffset;
            this.Left = MainMenu.Left + SubMenuLayout.LeftOffset;
        }

        private void rbModeControlledUPM_CheckedChanged(object sender, EventArgs e)
        {
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
            else
            {
                Prd.AppMode = ApplicationMode.DocumentTarget;
            }
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
                lbProduct.Text = "Fan " + (3 - (Props.MaxProducts - MainMenu.CurrentProduct.ID)).ToString();
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

                default:
                    rbModeControlledUPM.Checked = true;
                    break;
            }
            SetEnabled();

            Initializing = false;
        }
    }
}