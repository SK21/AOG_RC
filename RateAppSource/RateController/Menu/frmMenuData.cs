using RateController.Language;
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
    public partial class frmMenuData : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private FormStart mf;

        public frmMenuData(FormStart main, frmMenu menu)
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
                mf.Tls.WriteErrorLog("frmMenuData/btnOk_Click: " + ex.Message);
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
            mf.Tls.SaveFormData(this);
            timer1.Enabled = false;
        }

        private void frmMenuData_Load(object sender, EventArgs e)
        {
            // menu 800,600
            // sub menu 540,630
            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            MainMenu.ProductChanged += MainMenu_ProductChanged;
            mf.Tls.LoadFormData(this, "", false);
            this.BackColor = Properties.Settings.Default.BackColour;
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
            MainMenu.StyleControls(this);
            PositionForm();
            lbProduct.Font = new Font(lbProduct.Font.FontFamily, 18, FontStyle.Underline);
            UpdateForm();
            timer1.Enabled = true;
        }

        private void MainMenu_ProductChanged(object sender, EventArgs e)
        {
            UpdateForm();
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

        private void SetLanguage()
        {
            lbAcres1.Text = Lang.lgCoverage + " 1";
            lbAcres2.Text = Lang.lgCoverage + " 2";
            lbGallons1.Text = Lang.lgQuantity + " 1";
            lbGallons2.Text = Lang.lgQuantity + " 2";
            lbHours1.Text = Lang.lgHours + " 1";
            lbHours2.Text = Lang.lgHours + " 2";
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
            if (MainMenu.CurrentProduct.ID > mf.MaxProducts - 3)
            {
                // fans
                lbProduct.Text = "Fan " + (3 - (mf.MaxProducts - MainMenu.CurrentProduct.ID)).ToString();
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

            Initializing = false;
        }
    }
}