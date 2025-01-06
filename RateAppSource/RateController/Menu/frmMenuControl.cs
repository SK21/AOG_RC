using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuControl : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private FormStart mf;

        public frmMenuControl(FormStart main, frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            mf = main;
            this.Tag = false;
        }

        public bool Edited
        { get { return cEdited; } }

        public void UpdateForm()
        {
            Debug.Print("MenuControl: " + MainMenu.CurrentProduct.ID);
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

            HShigh.Value = MainMenu.CurrentProduct.HighAdjust;
            HSlow.Value = MainMenu.CurrentProduct.LowAdjust;
            HSthreshold.Value = MainMenu.CurrentProduct.Threshold;
            HSscaling.Value = MainMenu.CurrentProduct.MaxAdjust;
            HSmin.Value = MainMenu.CurrentProduct.MinAdjust;
            HSscaling.Value = MainMenu.CurrentProduct.ScalingFactor;
            UpdateControlDisplay();
            Initializing = false;
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
                MainMenu.CurrentProduct.HighAdjust = HShigh.Value;
                MainMenu.CurrentProduct.LowAdjust = HSlow.Value;
                MainMenu.CurrentProduct.Threshold = HSthreshold.Value;
                MainMenu.CurrentProduct.MaxAdjust = HSmax.Value;
                MainMenu.CurrentProduct.MinAdjust = HSmin.Value;
                MainMenu.CurrentProduct.ScalingFactor = HSscaling.Value;
                SetButtons(false);
                UpdateForm();
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmMenuControl/btnOk_Click: " + ex.Message);
            }
        }

        private void btnPIDloadDefaults_Click(object sender, EventArgs e)
        {
            HShigh.Value = mf.HighAdjustDefault;
            HSlow.Value = mf.LowAdjustDefault;
            HSthreshold.Value = mf.ThresholdDefault;
            HSscaling.Value = mf.MaxAdjustDefault;
            HSmin.Value = mf.MinAdjustDefault;
            HSscaling.Value = mf.ScalingDefault;
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            MainMenu.ChangeProduct(MainMenu.CurrentProduct.ID + 1);
            UpdateForm();
        }

        private void frmMenuControl_Activated(object sender, EventArgs e)
        {
            switch (this.Text)
            {
                case "Focused":
                    this.Text = "";
                    UpdateForm();
                    break;
            }
        }

        private void frmMenuControl_FormClosed(object sender, FormClosedEventArgs e)
        {
            mf.Tls.SaveFormData(this);
        }

        private void frmMenuControl_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void frmMenuControl_Load(object sender, EventArgs e)
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
            btnRight.Left = btnCancel.Left - 78;
            btnRight.Top = btnOK.Top;
            btnLeft.Left = btnRight.Left - 78;
            btnLeft.Top = btnOK.Top;
            btnPIDloadDefaults.Left = btnLeft.Left - 78;
            btnPIDloadDefaults.Top = btnOK.Top;
            PositionForm();

            foreach (Control con in this.Controls)
            {
                if (con is Label ctl)
                {
                    ctl.ForeColor = Properties.Settings.Default.ForeColour;
                    ctl.BackColor = Properties.Settings.Default.BackColour;
                    ctl.Font = Properties.Settings.Default.MenuFontSmall;
                }
                if (con is Button but)
                {
                    but.ForeColor = Properties.Settings.Default.ForeColour;
                    but.BackColor = Properties.Settings.Default.BackColour;
                    but.FlatAppearance.MouseDownBackColor = Properties.Settings.Default.MouseDown;
                }
                if (con is Panel pnl)
                {
                    pnl.BackColor = Properties.Settings.Default.BackColour;
                    foreach (Control pcon in pnl.Controls)
                    {
                        if (pcon is Label pctl)
                        {
                            pctl.ForeColor = Properties.Settings.Default.ForeColour;
                            pctl.BackColor = Properties.Settings.Default.BackColour;
                            pctl.Font = Properties.Settings.Default.MenuFontSmall;
                        }

                        if (pcon is TextBox tb)
                        {
                            tb.ForeColor = Properties.Settings.Default.ForeColour;
                            tb.BackColor = Properties.Settings.Default.BackColour;
                            tb.Font = Properties.Settings.Default.MenuFontSmall;
                            tb.BorderStyle = BorderStyle.FixedSingle;
                        }

                        if (pcon is CheckBox cb)
                        {
                            cb.BackColor = Properties.Settings.Default.ForeColour;
                            cb.FlatAppearance.CheckedBackColor = Color.LightGreen;
                        }
                    }
                }
            }
            lbProduct.Font = new Font(lbProduct.Font.FontFamily, 18, FontStyle.Underline);
            UpdateForm();
        }

        private void HShigh_ValueChanged(object sender, EventArgs e)
        {
            SetButtons(true);
            UpdateControlDisplay();
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
        }

        private void UpdateControlDisplay()
        {
            lbHigh.Text = HShigh.Value.ToString("N0");
            lbLow.Text = HSlow.Value.ToString("N0");
            lbThresholdValue.Text = HSthreshold.Value.ToString("N0");
            lbMaxValue.Text = HSscaling.Value.ToString("N0");
            lbMinValue.Text = HSmin.Value.ToString("N0");
            lbBoost.Text = HSscaling.Value.ToString("N0");
        }
    }
}