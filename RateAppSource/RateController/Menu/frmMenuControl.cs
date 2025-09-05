using AgOpenGPS;
using RateController.Classes;
using RateController.Language;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuControl : Form
    {
        private System.Windows.Forms.TextBox[] Boxes;
        private string[] BoxesFormat;
        private double[] BoxesMax;
        private double[] BoxesMin;
        private Button ButtonPressed;
        private int ButtonStep = 5;
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private FormStart mf;
        private Button[] RateButtons;

        public frmMenuControl(FormStart main, frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            mf = main;
            this.Tag = false;

            RateButtons = new Button[]{ btGainMinus, btGainPlus,btIntegralMinus,btIntegralPlus
            , btMaxMinus,btMaxPlus,btMinMinus,btMinPlus };

            foreach (Button btn in RateButtons)
            {
                btn.Click += Btn_Click;
                btn.MouseDown += Btn_MouseDown;
                btn.MouseUp += Btn_MouseUp;
            }

            Boxes = new System.Windows.Forms.TextBox[] { tbDeadband, tbBrakepoint, tbSlowAdj, tbSlewRate,
                tbMaxMotorI, tbMaxValveI, tbMinStart, tbAdjustTm,tbPauseTm,tbPIDtime };

            for (int i = 0; i < 8; i++)
            {
                Boxes[i].Tag = i;
                Boxes[i].Enter += BoxEnter;
                Boxes[i].TextChanged += BoxTextChanged;
            }
            BoxesMin = new double[] { 5, 5, 5, 1, 1, 0, 1, 10, 10, 10 };
            BoxesMax = new double[] { 50, 50, 75, 50, 20, 255, 10, 1000, 1000, 255 };
            BoxesFormat = new string[] { "N1", "N0", "N0", "N0", "N2", "N0", "N0", "N0", "N0", "N0" };
        }

        private void BoxEnter(object sender, EventArgs e)
        {
            System.Windows.Forms.TextBox bx = (System.Windows.Forms.TextBox)sender;
            double temp = 0;
            if (double.TryParse(bx.Text.Trim(), out double vl)) temp = vl;
            using (var form = new FormNumeric(BoxesMin[(int)bx.Tag], BoxesMax[(int)bx.Tag], temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    bx.Text = form.ReturnValue.ToString(BoxesFormat[(int)bx.Tag]);
                }
            }
        }

        private void BoxTextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            Button ClickedButton = (Button)sender;
            switch (ClickedButton.Name)
            {
                case "btGainMinus":
                    if (HSscaling.Value >= (HSscaling.Minimum + ButtonStep))
                    {
                        HSscaling.Value = HSscaling.Value - ButtonStep;
                    }
                    else
                    {
                        HSscaling.Value = HSscaling.Minimum;
                    }
                    break;

                case "btGainPlus":
                    if (HSscaling.Value <= (HSscaling.Maximum - ButtonStep))
                    {
                        HSscaling.Value = HSscaling.Value + ButtonStep;
                    }
                    else
                    {
                        HSscaling.Value = HSscaling.Maximum;
                    }
                    break;

                case "btIntegralMinus":
                    if (HSintegral.Value >= (HSintegral.Minimum + ButtonStep))
                    {
                        HSintegral.Value = HSintegral.Value - ButtonStep;
                    }
                    else
                    {
                        HSintegral.Value = HSintegral.Minimum;
                    }
                    break;

                case "btIntegralPlus":
                    if (HSintegral.Value <= (HSintegral.Maximum - ButtonStep))
                    {
                        HSintegral.Value = HSintegral.Value + ButtonStep;
                    }
                    else
                    {
                        HSintegral.Value = HSintegral.Maximum;
                    }
                    break;

                case "btMaxMinus":
                    if (HSmax.Value >= (HSmax.Minimum + ButtonStep))
                    {
                        HSmax.Value = HSmax.Value - ButtonStep;
                    }
                    else
                    {
                        HSmax.Value = HSmax.Minimum;
                    }
                    break;

                case "btMaxPlus":
                    if (HSmax.Value <= (HSmax.Maximum - ButtonStep))
                    {
                        HSmax.Value = HSmax.Value + ButtonStep;
                    }
                    else
                    {
                        HSmax.Value = HSmax.Maximum;
                    }
                    break;

                case "btMinMinus":
                    if (HSmin.Value >= (HSmin.Minimum + ButtonStep))
                    {
                        HSmin.Value = HSmin.Value - ButtonStep;
                    }
                    else
                    {
                        HSmin.Value = HSmin.Minimum;
                    }
                    break;

                case "btMinPlus":
                    if (HSmin.Value <= (HSmin.Maximum - ButtonStep))
                    {
                        HSmin.Value = HSmin.Value + ButtonStep;
                    }
                    else
                    {
                        HSmin.Value = HSmin.Maximum;
                    }
                    break;
            }
        }

        private void Btn_MouseDown(object sender, MouseEventArgs e)
        {
            timer1.Enabled = true;
            ButtonPressed = (Button)sender;
        }

        private void Btn_MouseUp(object sender, MouseEventArgs e)
        {
            timer1.Enabled = false;
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
                MainMenu.CurrentProduct.MaxPWMadjust = HSmax.Value;
                MainMenu.CurrentProduct.MinPWMadjust = HSmin.Value;
                MainMenu.CurrentProduct.KP = HSscaling.Value;
                MainMenu.CurrentProduct.KI = HSintegral.Value;

                if (byte.TryParse(tbDeadband.Text, out byte db)) MainMenu.CurrentProduct.Deadband = (byte)(db * 10);
                if (byte.TryParse(tbBrakepoint.Text, out byte bp)) MainMenu.CurrentProduct.Brakepoint = bp;
                if (byte.TryParse(tbSlowAdj.Text, out byte sa)) MainMenu.CurrentProduct.PIDslowAdjust = sa;

                MainMenu.CurrentProduct.Save();
                SetButtons(false);
                UpdateForm();

                mf.Products.UpdatePID();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuControl/btnOk_Click: " + ex.Message);
            }
        }

        private void btnPIDloadDefaults_Click(object sender, EventArgs e)
        {
            HSmax.Value = Props.MaxPWMdefault;
            HSmin.Value = Props.MinPWMdefault;
            HSscaling.Value = Props.KPdefault;
            HSintegral.Value = Props.KIdefault;

            tbDeadband.Text = (Props.DeadbandDefault / 10.0).ToString("N1");
            tbBrakepoint.Text = Props.BrakePointDefault.ToString();
            tbSlowAdj.Text = Props.PIDslowAdjustDefault.ToString();
            tbSlewRate.Text = Props.SlewRateDefault.ToString();
            tbMaxMotorI.Text = Props.MaxMotorIntegralDefault.ToString();
            tbMaxValveI.Text = Props.MaxValveIntegralDefault.ToString();
            tbMinStart.Text = Props.TimedMinStartDefault.ToString();
            tbAdjustTm.Text = Props.TimedAdjustDefault.ToString();
            tbPauseTm.Text = Props.TimedPauseDefault.ToString();
            tbMinHz.Text = Props.PulseMinHzDefault.ToString();
            tbMaxHz.Text = Props.PulseMaxHzDefault.ToString();
            tbSampleSize.Text = Props.PulseSampleSizeDefault.ToString();
            tbPIDtime.Text = Props.PIDtimeDefault.ToString();
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            MainMenu.ChangeProduct(MainMenu.CurrentProduct.ID + 1);
            UpdateForm();
        }

        private void butGraph_Click(object sender, EventArgs e)
        {
            Form fs = Props.IsFormOpen("frmMenuRateGraph");

            if (fs == null)
            {
                Form frm = new frmMenuRateGraph(mf);
                frm.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private void ckAdvanced_CheckedChanged(object sender, EventArgs e)
        {
            Props.SetProp("ShowAdvancedSettings", ckAdvanced.Checked.ToString());
        }

        private void ckAdvanced_Click(object sender, EventArgs e)
        {
            SetAdvanced();
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
            Props.SaveFormLocation(this);
        }

        private void frmMenuControl_Load(object sender, EventArgs e)
        {
            SubMenuLayout.SetFormLayout(this, MainMenu, btnOK);

            if (bool.TryParse(Props.GetProp("ShowAdvancedSettings"), out bool show))
            {
                ckAdvanced.Checked = show;
            }
            else
            {
                ckAdvanced.Checked = false;
            }
            SetAdvanced();

            btnCancel.Left = btnOK.Left - SubMenuLayout.ButtonSpacing;
            btnCancel.Top = btnOK.Top;
            btnRight.Left = btnCancel.Left - SubMenuLayout.ButtonSpacing;
            btnRight.Top = btnOK.Top;
            btnLeft.Left = btnRight.Left - SubMenuLayout.ButtonSpacing;
            btnLeft.Top = btnOK.Top;
            btnPIDloadDefaults.Left = btnLeft.Left - SubMenuLayout.ButtonSpacing;
            btnPIDloadDefaults.Top = btnOK.Top;
            MainMenu.StyleControls(this);
            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            MainMenu.ProductChanged += MainMenu_ProductChanged;
            PositionForm();
            UpdateForm();
            lbProduct.Font = new Font(lbProduct.Font.FontFamily, 18, FontStyle.Underline);
            Font ValFont = new Font(lbProduct.Font.FontFamily, 14, FontStyle.Bold);
            lbBoost.Font = ValFont;
            lbMaxValue.Font = ValFont;
            lbMinValue.Font = ValFont;
            lbIntegral.Font = ValFont;
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

        private void MainMenu_ProductChanged(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void PositionForm()
        {
            this.Top = MainMenu.Top + SubMenuLayout.TopOffset;
            this.Left = MainMenu.Left + SubMenuLayout.LeftOffset;
        }

        private void SetAdvanced()
        {
            if (ckAdvanced.Checked)
            {
                pnlAdvanced.Visible = true;
                pnlMain.Top = 0;
            }
            else
            {
                pnlAdvanced.Visible = false;
                pnlMain.Top = 115;
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

        private void SetLanguage()
        {
            lbScaling.Text = Lang.lgScaling;
            lbMax.Text = Lang.lgPWMmax;
            lbMin.Text = Lang.lgPWMmin;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ButtonPressed.PerformClick();
        }

        private void UpdateControlDisplay()
        {
            lbMaxValue.Text = HSmax.Value.ToString("N0");
            lbMinValue.Text = HSmin.Value.ToString("N0");
            lbBoost.Text = HSscaling.Value.ToString("N0");
            lbIntegral.Text = HSintegral.Value.ToString("N0");
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

            HSmax.Value = MainMenu.CurrentProduct.MaxPWMadjust;
            HSmin.Value = MainMenu.CurrentProduct.MinPWMadjust;
            HSscaling.Value = MainMenu.CurrentProduct.KP;
            HSintegral.Value = MainMenu.CurrentProduct.KI;
            UpdateControlDisplay();
            SetAdvanced();
            Initializing = false;
        }
    }
}