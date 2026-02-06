using RateController.Classes;
using RateController.Language;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuData : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;

        public frmMenuData(frmMenu menu)
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
                if (ckArea1.Checked)
                {
                    MainMenu.CurrentProduct.ResetCoverage();
                    ckArea1.Checked = false;
                }

                if (ckArea2.Checked)
                {
                    MainMenu.CurrentProduct.ResetCoverage2();
                    ckArea2.Checked = false;
                }

                if (ckQuantity1.Checked)
                {
                    MainMenu.CurrentProduct.ResetApplied();
                    ckQuantity1.Checked = false;
                }

                if (ckQuantity2.Checked)
                {
                    MainMenu.CurrentProduct.ResetApplied2();
                    ckQuantity2.Checked = false;
                }

                if (ckHours1.Checked)
                {
                    MainMenu.CurrentProduct.ResetHours1();
                    ckHours1.Checked = false;
                }

                if (ckHours2.Checked)
                {
                    MainMenu.CurrentProduct.ResetHours2();
                    ckHours2.Checked = false;
                }
                MainMenu.CurrentProduct.Save();
                SetButtons(false);
                UpdateForm();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuData/btnOk_Click: " + ex.Message);
            }
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            MainMenu.ChangeProduct(MainMenu.CurrentProduct.ID + 1);
            UpdateForm();
        }

        private void ckArea1_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void frmMenuData_Activated(object sender, EventArgs e)
        {
            switch (this.Text)
            {
                case "Focused":
                    this.Text = "";
                    UpdateForm();
                    break;
            }
        }

        private void frmMenuData_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
            timer1.Enabled = false;
        }

        private void frmMenuData_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainMenu.MenuMoved -= MainMenu_MenuMoved;
            MainMenu.ProductChanged -= MainMenu_ProductChanged;
            MainMenu.ProductEnabled -= MainMenu_ProductEnabled;
        }

        private void frmMenuData_Load(object sender, EventArgs e)
        {
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            MainMenu.ProductChanged += MainMenu_ProductChanged;
            MainMenu.ProductEnabled += MainMenu_ProductEnabled;

            SubMenuLayout.SetFormLayout(this, MainMenu, btnOK);
            btnCancel.Left = btnOK.Left - SubMenuLayout.ButtonSpacing;
            btnCancel.Top = btnOK.Top;
            btnRight.Left = btnCancel.Left - SubMenuLayout.ButtonSpacing;
            btnRight.Top = btnOK.Top;
            btnLeft.Left = btnRight.Left - SubMenuLayout.ButtonSpacing;
            btnLeft.Top = btnOK.Top;
            MainMenu.StyleControls(this);
            SetLanguage();
            this.BackColor = Properties.Settings.Default.MainBackColour;
            PositionForm();
            lbProduct.Font = new Font(lbProduct.Font.FontFamily, 18, FontStyle.Underline);
            UpdateForm();
            timer1.Enabled = true;
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

            ckArea1.Enabled = Enabled;
            ckArea2.Enabled = Enabled;
            ckQuantity1.Enabled = Enabled;
            ckQuantity2.Enabled = Enabled;
            ckHours1.Enabled = Enabled;
            ckHours2.Enabled = Enabled;
        }

        private void SetLanguage()
        {
            lbAcres1.Text = Lang.lgCoverage + " 1";
            lbAcres2.Text = Lang.lgCoverage + " 2";
            lbGallons1.Text = Lang.lgQuantity + " 1";
            lbGallons2.Text = Lang.lgQuantity + " 2";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateData();
        }

        private void UpdateData()
        {
            lbArea1.Text = MainMenu.CurrentProduct.CurrentCoverage().ToString("N1");
            lbArea2.Text = MainMenu.CurrentProduct.CurrentCoverage2().ToString("N1");
            lbQuantity1.Text = MainMenu.CurrentProduct.UnitsApplied().ToString("N1");
            lbQuantity2.Text = MainMenu.CurrentProduct.UnitsApplied2().ToString("N1");
            lbHours1value.Text = MainMenu.CurrentProduct.Hours1.ToString("N2");
            lbHours2value.Text = MainMenu.CurrentProduct.Hours2.ToString("N2");
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
            lbAcres1.Text = "*" + MainMenu.CurrentProduct.CoverageDescription() + " 1";
            lbAcres2.Text = MainMenu.CurrentProduct.CoverageDescription() + " 2";
            lbGallons1.Text = "*" + MainMenu.CurrentProduct.QuantityDescription + " 1";
            lbGallons2.Text = MainMenu.CurrentProduct.QuantityDescription + " 2";
            UpdateData();

            ckArea1.Checked = false;
            ckArea2.Checked = false;
            ckQuantity1.Checked = false;
            ckQuantity2.Checked = false;
            ckHours1.Checked = false;
            ckHours2.Checked = false;
            SetEnabled();

            Initializing = false;
        }
    }
}