using RateController.Classes;
using RateController.Forms;
using RateController.Language;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmLargeScreen : Form
    {
        public clsProduct cCurrentProduct;
        public FormStart mf;
        public clsAlarm RCalarm;
        private int CompactWidth = 250;
        private int Fan1RateType = 0;
        private int Fan2RateType = 0;
        private bool IsTransparent = false;
        private int MainPanelLeft = 157;
        private bool masterOn;
        private bool MasterPressed;
        private Point MouseDownLocation;
        private int NormalWidth = 403;
        private int[] PanelPositions = { 6, 106, 206, 306 };
        private Color RateColour = Color.GreenYellow;
        private int WidthOffset = 0;

        public frmLargeScreen(FormStart CallingForm)
        {
            InitializeComponent();

            #region // language

            lbTarget.Text = Lang.lgTargetRate;
            lbCoverage.Text = Lang.lgCoverage;
            lbQuantity.Text = Lang.lgTank_Remaining;
            lbUnits.Text = Lang.lgApplied;

            #endregion // language

            mf = CallingForm;
            cCurrentProduct = mf.Products.Item(0);
            RCalarm = new clsAlarm(mf, btAlarm);
            WidthOffset = CompactWidth - NormalWidth;

            this.BackColor = Properties.Settings.Default.DisplayBackColour;
            pnlRate0.BackColor = Properties.Settings.Default.MainBackColour;
            pnlRate1.BackColor = Properties.Settings.Default.MainBackColour;
            pnlRate1.BackColor = Properties.Settings.Default.MainBackColour;
            pnlRate1.BackColor = Properties.Settings.Default.MainBackColour;
            pnlQuantity0.BackColor = Properties.Settings.Default.MainBackColour;
            pnlQuantity1.BackColor = Properties.Settings.Default.MainBackColour;
            pnlQuantity2.BackColor = Properties.Settings.Default.MainBackColour;
            pnlQuantity3.BackColor = Properties.Settings.Default.MainBackColour;
            btnUp.BackColor = Properties.Settings.Default.MainBackColour;
            btnDown.BackColor = Properties.Settings.Default.MainBackColour;

            foreach (Control Ctrl in Controls)
            {
                if (Ctrl.Name != "btnSettings" && Ctrl.Name != "btAuto" && Ctrl.Name != "btMaster")
                {
                    Ctrl.MouseDown += mouseMove_MouseDown;
                    Ctrl.MouseMove += mouseMove_MouseMove;
                }
            }

            mf.SwitchBox.SwitchPGNreceived += SwitchBox_SwitchPGNreceived;
            mf.ColorChanged += Mf_ColorChanged;
            Props.ProductSettingsChanged += Props_ProductSettingsChanged;
            Props.ProfileChanged += Props_ProfileChanged;
        }

        public int CurrentProduct()
        {
            return cCurrentProduct.ID;
        }

        public void SetTransparent()
        {
            IsTransparent = Props.UseTransparent;
            if (Props.UseTransparent)
            {
                this.TransparencyKey = Properties.Settings.Default.DisplayBackColour;
            }
            else
            {
                this.TransparencyKey = Color.Empty;
            }
            SetFont();
            SetDisplay();
        }

        public void SwitchToStandard()
        {
            this.ShowInTaskbar = false;
            mf.ShowInTaskbar = true;
            this.Close();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!Props.UseTransparent)
            {
                // Define the border color and thickness
                Color borderColor = Properties.Settings.Default.DisplayForeColour;
                int borderWidth = 1;

                // Draw the border
                using (Pen pen = new Pen(borderColor, borderWidth))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1);
                }
            }
        }

        private void btAlarm_Click(object sender, EventArgs e)
        {
            RCalarm.Silence();
        }

        private void btAuto_Click(object sender, EventArgs e)
        {
            mf.vSwitchBox.PressSwitch(SwIDs.AutoRate, true);
            if (mf.vSwitchBox.AutoRateOn != mf.vSwitchBox.AutoSectionOn) mf.vSwitchBox.PressSwitch(SwIDs.AutoSection, true);
        }

        private void btMaster_Click(object sender, EventArgs e)
        {
            if (masterOn)
            {
                mf.vSwitchBox.PressSwitch(SwIDs.MasterOff, true);
                MasterPressed = true;
                tmrRelease.Enabled = true;
            }
            else
            {
                mf.vSwitchBox.PressSwitch(SwIDs.MasterOn, true);
                MasterPressed = true;
                tmrRelease.Enabled = true;
            }
        }

        private void btMaster_MouseUp(object sender, MouseEventArgs e)
        {
            MasterPressed = false;
        }

        private void btMinimize_Click(object sender, EventArgs e)
        {
            // Detach pressure display (and similar pinned forms) before minimizing so they stay visible.
            frmPressureDisplay pressure = (frmPressureDisplay)Props.IsFormOpen("frmPressureDisplay", false);
            if (pressure != null && pressure.Owner == this)
            {
                pressure.DetachFromOwner();
            }
            frmRate rateDisp = (frmRate)Props.IsFormOpen("frmRate", false);
            if (rateDisp != null && rateDisp.Owner == this)
            {
                // if you also pin the rate form similarly, detach it
                rateDisp.Owner = null;
            }

            Form restoreform = new RCRestore(this, Props.UserRateType, mf);
            restoreform.Show();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (mf.SwitchBox.AutoRateOn)
            {
                cCurrentProduct.RateSet = cCurrentProduct.RateSet / 1.05;
            }
            else
            {
                cCurrentProduct.ManualPWM -= 5;
            }
        }

        private void btnFan1_Click(object sender, EventArgs e)
        {
            clsProduct fn = mf.Products.Item(Props.MaxProducts - 2);
            fn.FanOn = !fn.FanOn;
        }

        private void btnFan2_Click(object sender, EventArgs e)
        {
            clsProduct fn = mf.Products.Item(Props.MaxProducts - 1);
            fn.FanOn = !fn.FanOn;
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            ShowSettings(cCurrentProduct.ID, true);
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (mf.SwitchBox.AutoRateOn)
            {
                cCurrentProduct.RateSet = cCurrentProduct.RateSet * 1.05;
            }
            else
            {
                cCurrentProduct.ManualPWM += 5;
            }
        }

        private void CheckDisplay()
        {
            // check if panel is not visible when it should be
            Panel pnl = pnlProd0;
            if (pnl.Enabled)
            {
                if (!pnl.Visible) Props.WriteErrorLog("Panel 0 not visible.");
                if (!Props.IsOnScreen(pnl))
                {
                    Props.WriteErrorLog("Panel 0 not on screen. Left = " + pnl.Left.ToString()
                        + " WidthOffset = " + WidthOffset.ToString());
                }
            }
        }

        private void frmLargeScreen_Activated(object sender, EventArgs e)
        {
            lbName0.Focus();
        }

        private void frmLargeScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            mf.SwitchBox.SwitchPGNreceived -= SwitchBox_SwitchPGNreceived;
            mf.ColorChanged -= Mf_ColorChanged;
            Props.ProductSettingsChanged -= Props_ProductSettingsChanged;
            Props.ProfileChanged -= Props_ProfileChanged;

            timerMain.Enabled = false;
            tmrRelease.Enabled = false;

            Props.SaveFormLocation(this);

            if (Props.UseLargeScreen) mf.LargeScreenExit = true;
            mf.WindowState = FormWindowState.Normal;
            mf.vSwitchBox.LargeScreenOn = false;
        }

        private void frmLargeScreen_Load(object sender, EventArgs e)
        {
            Props.LoadFormLocation(this);
            cCurrentProduct = mf.Products.Item(Props.DefaultProduct);
            timerMain.Enabled = true;
            mf.vSwitchBox.LargeScreenOn = true;
            mf.vSwitchBox.PressSwitch(SwIDs.MasterOff);
            tmrRelease.Enabled = true;
            UpdateSwitches();
            ShowProducts();
            UpdateForm();
        }

        private void lbCoverage_Click(object sender, EventArgs e)
        {
            Props.ShowCoverageRemaining = !Props.ShowCoverageRemaining;
            UpdateForm();
        }

        private void lbCoverage_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Shows either coverage applied (A) or area that can be done with the remaining quantity (R)." +
                "\n Press to change.";

            Props.ShowMessage(Message, "Coverage");
            hlpevent.Handled = true;
        }

        private void lbCoverageAmount_Click(object sender, EventArgs e)
        {
            var Hlp = new frmMsgBox(mf, "Reset area?", "Reset", true);
            Hlp.TopMost = true;

            Hlp.ShowDialog();
            bool Result = Hlp.Result;
            Hlp.Close();
            if (Result)
            {
                mf.Products.Item(CurrentProduct()).ResetCoverage();
            }
        }

        private void lbName0_Click(object sender, EventArgs e)
        {
            ShowSettings(0, true);
        }

        private void lbName0_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Indicates if the sensor is connected. Click to access settings.";

            Props.ShowMessage(Message);
            hlpevent.Handled = true;
        }

        private void lbName1_Click(object sender, EventArgs e)
        {
            ShowSettings(1, true);
        }

        private void lbName2_Click(object sender, EventArgs e)
        {
            ShowSettings(2, true);
        }

        private void lbName3_Click(object sender, EventArgs e)
        {
            ShowSettings(3, true);
        }

        private void lbQuantity_Click(object sender, EventArgs e)
        {
            Props.ShowQuantityRemaining = !Props.ShowQuantityRemaining;
            UpdateForm();
        }

        private void lbQuantity_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Shows either quantity applied (A) or quantity remaining (R)." +
                "\n Press to change.";

            Props.ShowMessage(Message, "Remaining");
            hlpevent.Handled = true;
        }

        private void lbQuantityAmount_Click(object sender, EventArgs e)
        {
            //check if window already exists
            Form fs = Props.IsFormOpen("frmResetQuantity");

            if (fs != null)
            {
                fs.Focus();
                return;
            }

            Form frm = new frmResetQuantity(mf);
            frm.Show();
        }

        private void lbRate_Click(object sender, EventArgs e)
        {
            if (Props.UserRateType < 1)
            {
                Props.UserRateType++;
            }
            else
            {
                Props.UserRateType = 0;
            }
            UpdateForm();
        }

        private void lbRate_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "C - Current Rate, shows" +
                " the target rate when it is within 10% of target. Outside this range it" +
                " shows the exact rate being applied. \n I - Instant Rate, shows the exact rate." +
                "\n O - Overall, averages total quantity applied over area done." +
                "\n Press to change.";

            Props.ShowMessage(Message, "Rate");
            hlpevent.Handled = true;
        }

        private void lbRPM1_Click(object sender, EventArgs e)
        {
            Fan1RateType++;
            if (Fan1RateType > 1) Fan1RateType = 0;
            UpdateForm();
        }

        private void lbRPM2_Click(object sender, EventArgs e)
        {
            Fan2RateType++;
            if (Fan2RateType > 1) Fan2RateType = 0;
            UpdateForm();
        }

        private void lbTarget_Click(object sender, EventArgs e)
        {
            if (cCurrentProduct.UseAltRate)
            {
                cCurrentProduct.UseAltRate = false;
                lbTargetType.Text = "T";
            }
            else
            {
                cCurrentProduct.UseAltRate = true;
                lbTargetType.Text = "A";
            }
        }

        private void lbTarget_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Press to switch between base rate (R) and alternate rate (A).";

            Props.ShowMessage(Message, "Target Rate");
            hlpevent.Handled = true;
        }

        private void Mf_ColorChanged(object sender, EventArgs e)
        {
            SetDisplay();
        }

        private void mouseMove_MouseDown(object sender, MouseEventArgs e)
        {
            MouseDownLocation = e.Location;
        }

        private void mouseMove_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Right) || (e.Button == MouseButtons.Left)) this.Location = new Point(this.Left + e.X - MouseDownLocation.X, this.Top + e.Y - MouseDownLocation.Y);
        }

        private void pbRate0_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Indicates Current rate compared to Target rate." +
                " Target rate is the centre of the graph. " +
                "Within 10 % of target, the graph is dark green, otherwise red." +
                " Click to select product and view" +
                " the product's information.";

            Props.ShowMessage(Message);
            hlpevent.Handled = true;
        }

        private void Props_ProductSettingsChanged(object sender, EventArgs e)
        {
            ShowProducts();
        }

        private void Props_ProfileChanged(object sender, EventArgs e)
        {
            ShowProducts();
        }

        private void SetDisplay()
        {
            Color NewColor = Properties.Settings.Default.DisplayForeColour;
            lbTarget.ForeColor = NewColor;
            lbCoverage.ForeColor = NewColor;
            lbQuantity.ForeColor = NewColor;
            lbRateAmount.ForeColor = NewColor;
            lbTargetAmount.ForeColor = NewColor;
            lbCoverageAmount.ForeColor = NewColor;
            lbQuantityAmount.ForeColor = NewColor;
            lbRPM1.ForeColor = NewColor;
            lbRPM2.ForeColor = NewColor;
            lbUnits.ForeColor = NewColor;
            lbRateType.ForeColor = NewColor;
            lbTargetType.ForeColor = NewColor;
            lbCoverageType.ForeColor = NewColor;
            lbQuantityType.ForeColor = NewColor;

            this.BackColor = Properties.Settings.Default.DisplayBackColour;
        }

        private void SetFont()
        {
            if (Props.UseTransparent)
            {
                string TransparentFont = "MS Gothic";
                //string TransparentFont = "Courier New";
                //string TransparentFont = "Candara Light";
                //string TransparentFont = "Tahoma";

                foreach (Control Ctrl in Controls)
                {
                    Ctrl.Font = new Font(TransparentFont, 14, FontStyle.Bold);
                }

                lbRateAmount.Font = new Font(TransparentFont, 16, FontStyle.Bold);
                lbTargetAmount.Font = new Font(TransparentFont, 16, FontStyle.Bold);
                lbQuantityAmount.Font = new Font(TransparentFont, 16, FontStyle.Bold);
                lbCoverageAmount.Font = new Font(TransparentFont, 16, FontStyle.Bold);

                btAuto.Font = new Font(TransparentFont, 10, FontStyle.Bold);
                btMaster.Font = new Font(TransparentFont, 10, FontStyle.Bold);
            }
            else
            {
                foreach (Control Ctrl in Controls)
                {
                    Ctrl.Font = new Font("Tahoma", 14);
                }

                lbRateAmount.Font = new Font("Tahoma", 16, FontStyle.Bold);
                lbTargetAmount.Font = new Font("Tahoma", 16, FontStyle.Bold);
                lbQuantityAmount.Font = new Font("Tahoma", 16, FontStyle.Bold);
                lbCoverageAmount.Font = new Font("Tahoma", 16, FontStyle.Bold);

                btAuto.Font = new Font("MS Gothic", 10, FontStyle.Bold);
                btMaster.Font = new Font("MS Gothic", 10, FontStyle.Bold);
            }
        }

        private void ShowBumpButtons()
        {
            try
            {
                for (int i = 0; i < 5; i++)
                {
                    clsProduct Prduct = mf.Products.Item(i);
                    if (i == 4)
                    {
                        btnDown.Visible = false;
                        btnDown.Enabled = false;
                        btnUp.Visible = false;
                        btnUp.Enabled = false;
                    }
                    else if (Prduct.BumpButtons && Prduct.Enabled)
                    {
                        btnUp.Visible = true;
                        btnDown.Visible = true;
                        btnUp.Enabled = true;
                        btnDown.Enabled = true;

                        var panelArr = Controls.Find("pnlProd" + i, true);
                        var rateArr = Controls.Find("pbRate" + i, true);
                        if (panelArr.Length == 0 || rateArr.Length == 0)
                        {
                            // Cannot position bump buttons if required controls are missing.
                            continue;
                        }

                        Panel posPnl = (Panel)panelArr[0];

                        int posX = posPnl.Left;
                        int posY = posPnl.Top;
                        int Width = posPnl.Width;
                        int Height = posPnl.Height;

                        btnUp.Left = posX;
                        btnDown.Left = posX;
                        btnUp.Width = Width;
                        btnDown.Width = Width;
                        btnUp.Top = posY;
                        btnUp.Height = Height / 2;
                        btnDown.Top = posY + btnUp.Height;
                        btnDown.Height = btnUp.Height;

                        btnUp.BringToFront();
                        btnDown.BringToFront();

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmLargeScreen/ShowBumpButtons: " + ex.Message);
            }
        }

        private void ShowProducts()
        {
            try
            {
                cCurrentProduct = mf.Products.Item(Props.DefaultProduct);

                // fans
                clsProduct Prduct = mf.Products.Item(4);
                lbFan1.Visible = Prduct.Enabled;
                lbRPM1.Visible = Prduct.Enabled;
                btnFan1.Visible = Prduct.Enabled;

                Prduct = mf.Products.Item(5);
                lbFan2.Visible = Prduct.Enabled;
                lbRPM2.Visible = Prduct.Enabled;
                btnFan2.Visible = Prduct.Enabled;

                // products
                int PanelCount = 0;
                int CurrentPosition = 3;

                for (int i = 3; i > -1; i--)
                {
                    var panels = Controls.Find("pnlProd" + i, true);
                    if (panels.Length == 0) continue;
                    Panel posPnl = (Panel)panels[0];
                    clsProduct Prod = mf.Products.Item(i);
                    posPnl.Visible = Prod.Enabled;
                    if (Prod.Enabled)
                    {
                        PanelCount++;
                        posPnl.Left = PanelPositions[CurrentPosition];
                        CurrentPosition--;
                    }
                }

                // resize form
                if (PanelCount > 2 || lbFan1.Visible == true || lbFan2.Visible == true)
                {
                    // normal view
                    this.Width = NormalWidth;
                    pnlMain.Left = MainPanelLeft;
                }
                else
                {
                    // compact view
                    int CenterOffset = -73;
                    if (PanelCount == 2) CenterOffset = -23;

                    this.Width = CompactWidth;
                    pnlProd0.Left += WidthOffset + CenterOffset;
                    pnlProd1.Left += WidthOffset + CenterOffset;
                    pnlProd2.Left += WidthOffset + CenterOffset;
                    pnlProd3.Left += WidthOffset + CenterOffset;
                    pnlMain.Left = MainPanelLeft + WidthOffset;
                }

                ShowBumpButtons();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmLargeScreen/ShowProducts: " + ex.Message);
            }
        }

        private void ShowSettings(int ProductID, bool OpenLast = false)
        {
            cCurrentProduct = mf.Products.Item(ProductID);
            UpdateForm();

            //check if window already exists
            Form fs = Props.IsFormOpen("frmMenu");

            if (fs == null)
            {
                Form frm = new frmMenu(mf, cCurrentProduct.ID, OpenLast);
                frm.Show();
            }
        }

        private void SwitchBox_SwitchPGNreceived(object sender, EventArgs e)
        {
            UpdateSwitches();
        }

        private void timerMain_Tick(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void tmrRelease_Tick(object sender, EventArgs e)
        {
            if (!MasterPressed)
            {
                mf.vSwitchBox.ReleaseSwitch();
                tmrRelease.Enabled = false;
            }
        }

        private void UpdateForm()
        {
            if (Props.UseTransparent != IsTransparent) SetTransparent();

            this.Text = "RC [" + Path.GetFileNameWithoutExtension(Properties.Settings.Default.CurrentFile) + "]";

            if (Props.VariableRateEnabled)
            {
                lbTargetType.Text = "V";
            }
            else if (cCurrentProduct.UseAltRate)
            {
                lbTargetType.Text = "A";
            }
            else
            {
                lbTargetType.Text = "T";
            }

            // set highlight
            pnlSelect0.BackColor = Properties.Settings.Default.MainBackColour;
            pnlSelect1.BackColor = Properties.Settings.Default.MainBackColour;
            pnlSelect2.BackColor = Properties.Settings.Default.MainBackColour;
            pnlSelect3.BackColor = Properties.Settings.Default.MainBackColour;
            switch (cCurrentProduct.ID)
            {
                case 0:
                    pnlSelect0.BackColor = SystemColors.Highlight;
                    break;

                case 1:
                    pnlSelect1.BackColor = SystemColors.Highlight;
                    break;

                case 2:
                    pnlSelect2.BackColor = SystemColors.Highlight;
                    break;

                case 3:
                    pnlSelect3.BackColor = SystemColors.Highlight;
                    break;
            }

            // product info
            lbName0.Text = mf.Products.Item(0).ProductName;
            lbName1.Text = mf.Products.Item(1).ProductName;
            lbName2.Text = mf.Products.Item(2).ProductName;
            lbName3.Text = mf.Products.Item(3).ProductName;

            if (lbName0.Text == "") lbName0.Text = "1";
            if (lbName1.Text == "") lbName1.Text = "2";
            if (lbName2.Text == "") lbName2.Text = "3";
            if (lbName3.Text == "") lbName3.Text = "4";

            clsProduct Product = mf.Products.Item(0);
            if (Product.RateSensorData.ModuleSending())
            {
                if (Product.RateSensorData.ModuleReceiving())
                {
                    lbName0.BackColor = Color.LightGreen;
                }
                else
                {
                    lbName0.BackColor = Color.DeepSkyBlue;
                }
            }
            else
            {
                lbName0.BackColor = Color.Red;
            }

            // 1
            Product = mf.Products.Item(1);
            if (Product.RateSensorData.ModuleSending())
            {
                if (Product.RateSensorData.ModuleReceiving())
                {
                    lbName1.BackColor = Color.LightGreen;
                }
                else
                {
                    lbName1.BackColor = Color.DeepSkyBlue;
                }
            }
            else
            {
                lbName1.BackColor = Color.Red;
            }

            // 2
            Product = mf.Products.Item(2);
            if (Product.RateSensorData.ModuleSending())
            {
                if (Product.RateSensorData.ModuleReceiving())
                {
                    lbName2.BackColor = Color.LightGreen;
                }
                else
                {
                    lbName2.BackColor = Color.DeepSkyBlue;
                }
            }
            else
            {
                lbName2.BackColor = Color.Red;
            }

            // 3
            Product = mf.Products.Item(3);
            if (Product.RateSensorData.ModuleSending())
            {
                if (Product.RateSensorData.ModuleReceiving())
                {
                    lbName3.BackColor = Color.LightGreen;
                }
                else
                {
                    lbName3.BackColor = Color.DeepSkyBlue;
                }
            }
            else
            {
                lbName3.BackColor = Color.Red;
            }

            // rate
            switch (Props.UserRateType)
            {
                case 1:
                    lbRateType.Text = "I";
                    lbRateAmount.Text = cCurrentProduct.CurrentRate().ToString("N1");
                    break;

                case 2:
                    lbRateType.Text = "O";
                    lbRateAmount.Text = cCurrentProduct.AverageRate().ToString("N1");
                    break;

                default:
                    lbRateType.Text = "C";
                    lbRateAmount.Text = cCurrentProduct.SmoothRate().ToString("N1");
                    break;
            }

            lbTargetAmount.Text = cCurrentProduct.TargetRate().ToString("N1");

            // coverage
            if (Props.ShowCoverageRemaining)
            {
                lbCoverageType.Text = "R";
                double RT = cCurrentProduct.SmoothRate();
                if (RT == 0) RT = cCurrentProduct.TargetRate();

                if ((RT > 0) & (cCurrentProduct.TankStart > 0))
                {
                    double amt = (cCurrentProduct.TankStart - cCurrentProduct.UnitsApplied()) / RT;
                    if (Math.Abs(amt) >= 1000)
                    {
                        lbCoverageAmount.Text = amt.ToString("N0");
                    }
                    else
                    {
                        lbCoverageAmount.Text = amt.ToString("N1");
                    }
                }
                else
                {
                    lbCoverageAmount.Text = "0.0";
                }
            }
            else
            {
                // show amount done
                lbCoverageAmount.Text = cCurrentProduct.CurrentCoverage().ToString("N1");
                lbCoverageType.Text = "A";
            }
            lbCoverage.Text = cCurrentProduct.CoverageDescription();

            // quantity
            if (Props.ShowQuantityRemaining)
            {
                lbQuantityType.Text = "R";
                // calculate remaining
                lbQuantityAmount.Text = (cCurrentProduct.TankStart - cCurrentProduct.UnitsApplied()).ToString("N0");
            }
            else
            {
                // show amount done
                lbQuantityType.Text = "A";
                lbQuantityAmount.Text = cCurrentProduct.UnitsApplied().ToString("N0");
            }
            lbQuantity.Text = cCurrentProduct.QuantityDescription;

            // aog
            if (Props.SpeedMode == SpeedType.Simulated)
            {
                btnMenu.Image = Properties.Resources.SimGear;
            }
            else
            {
                if (mf.AutoSteerPGN.Connected())
                {
                    btnMenu.Image = Properties.Resources.GreenGear;
                }
                else
                {
                    btnMenu.Image = Properties.Resources.RedGear;
                }
            }

            // graphs
            // product 0
            clsProduct PD = mf.Products.Item(0);
            double Size = PD.TankSize;
            double Rem = PD.TankStart - PD.UnitsApplied();
            if (Size == 0 || Size < Rem) Size = Rem * 2;
            if (Size == 0) Size = 100;
            int Level = (int)(Rem / Size * 100);
            if (Level > 100) Level = 100;
            if (Level < 0) Level = 0;
            pbQuantity0.Value = Level;

            double Rt = PD.SmoothRate();
            double Tg = PD.TargetRate();
            int RtLevel = 0;
            if (Tg > 0) RtLevel = (int)((Rt / Tg) * 50) - 30;
            if (RtLevel > 40) RtLevel = 40;
            if (RtLevel < 0) RtLevel = 0;
            if (Tg > 0 && RtLevel < 1) RtLevel = 1;
            if (RtLevel > 25 || RtLevel < 15)
            {
                pbRate0.ForeColor = Color.Red;
            }
            else
            {
                pbRate0.ForeColor = RateColour;
            }
            pbRate0.Value = RtLevel;

            // product 1
            PD = mf.Products.Item(1);
            Size = PD.TankSize;
            Rem = PD.TankStart - PD.UnitsApplied();
            if (Size == 0 || Size < Rem) Size = Rem * 2;
            if (Size == 0) Size = 100;
            Level = (int)(Rem / Size * 100);
            if (Level > 100) Level = 100;
            if (Level < 0) Level = 0;
            pbQuantity1.Value = Level;

            Rt = PD.SmoothRate();
            Tg = PD.TargetRate();
            RtLevel = 0;
            if (Tg > 0) RtLevel = (int)((Rt / Tg) * 50) - 30;
            if (RtLevel > 40) RtLevel = 40;
            if (RtLevel < 0) RtLevel = 0;
            if (Tg > 0 && RtLevel < 1) RtLevel = 1;
            if (RtLevel > 25 || RtLevel < 15)
            {
                pbRate1.ForeColor = Color.Red;
            }
            else
            {
                pbRate1.ForeColor = RateColour;
            }
            pbRate1.Value = RtLevel;

            // product 2
            PD = mf.Products.Item(2);
            Size = PD.TankSize;
            Rem = PD.TankStart - PD.UnitsApplied();
            if (Size == 0 || Size < Rem) Size = Rem * 2;
            if (Size == 0) Size = 100;
            Level = (int)(Rem / Size * 100);
            if (Level > 100) Level = 100;
            if (Level < 0) Level = 0;
            pbQuantity2.Value = Level;

            Rt = PD.SmoothRate();
            Tg = PD.TargetRate();
            RtLevel = 0;
            if (Tg > 0) RtLevel = (int)((Rt / Tg) * 50) - 30;
            if (RtLevel > 40) RtLevel = 40;
            if (RtLevel < 0) RtLevel = 0;
            if (Tg > 0 && RtLevel < 1) RtLevel = 1;
            if (RtLevel > 25 || RtLevel < 15)
            {
                pbRate2.ForeColor = Color.Red;
            }
            else
            {
                pbRate2.ForeColor = RateColour;
            }
            pbRate2.Value = RtLevel;

            // product 3
            PD = mf.Products.Item(3);
            Size = PD.TankSize;
            Rem = PD.TankStart - PD.UnitsApplied();
            if (Size == 0 || Size < Rem) Size = Rem * 2;
            if (Size == 0) Size = 100;
            Level = (int)(Rem / Size * 100);
            if (Level > 100) Level = 100;
            if (Level < 0) Level = 0;
            pbQuantity3.Value = Level;

            Rt = PD.SmoothRate();
            Tg = PD.TargetRate();
            RtLevel = 0;
            if (Tg > 0) RtLevel = (int)((Rt / Tg) * 50) - 30;
            if (RtLevel > 40) RtLevel = 40;
            if (RtLevel < 0) RtLevel = 0;
            if (Tg > 0 && RtLevel < 1) RtLevel = 1;
            if (RtLevel > 25 || RtLevel < 15)
            {
                pbRate3.ForeColor = Color.Red;
            }
            else
            {
                pbRate3.ForeColor = RateColour;
            }
            pbRate3.Value = RtLevel;

            clsProduct prd = mf.Products.Item(Props.MaxProducts - 2);

            if (Fan1RateType == 1)
            {
                lbRPM1.Text = prd.CurrentRate().ToString("N0") + " RPM-I";
            }
            else
            {
                lbRPM1.Text = prd.SmoothRate().ToString("N0") + " RPM";
            }

            if (prd.RateSensorData.ModuleSending())
            {
                if (prd.RateSensorData.ModuleReceiving())
                {
                    lbFan1.BackColor = Color.LightGreen;
                }
                else
                {
                    lbFan1.BackColor = Color.DeepSkyBlue;
                }
            }
            else
            {
                lbFan1.BackColor = Color.Red;
            }

            // fan 2
            prd = mf.Products.Item(Props.MaxProducts - 1);
            if (Fan2RateType == 1)
            {
                lbRPM2.Text = prd.CurrentRate().ToString("N0") + " RPM-I";
            }
            else
            {
                lbRPM2.Text = prd.SmoothRate().ToString("N0") + " RPM";
            }

            if (prd.RateSensorData.ModuleSending())
            {
                if (prd.RateSensorData.ModuleReceiving())
                {
                    lbFan2.BackColor = Color.LightGreen;
                }
                else
                {
                    lbFan2.BackColor = Color.DeepSkyBlue;
                }
            }
            else
            {
                lbFan2.BackColor = Color.Red;
            }

            // fan 1 button
            clsProduct fn = mf.Products.Item(Props.MaxProducts - 2);
            if (fn.FanOn)
            {
                btnFan1.Image = Properties.Resources.FanOn;
            }
            else
            {
                btnFan1.Image = Properties.Resources.FanOff;
            }

            // fan 2 button
            fn = mf.Products.Item(Props.MaxProducts - 1);
            if (fn.FanOn)
            {
                btnFan2.Image = Properties.Resources.FanOn;
            }
            else
            {
                btnFan2.Image = Properties.Resources.FanOff;
            }

            RCalarm.CheckAlarms();
            CheckDisplay();
        }

        private void UpdateSwitches()
        {
            // auto button
            if (mf.SwitchBox.AutoRateOn || mf.SwitchBox.AutoSectionOn)
            {
                btAuto.BackColor = Color.LightGreen;
                btAuto.Text = "AUTO";
                btAuto.ForeColor = Color.Black;
            }
            else
            {
                btAuto.BackColor = Color.Red;
                btAuto.Text = "AUTO";
                btAuto.ForeColor = Color.Black;
            }

            // master button
            if (mf.SwitchBox.MasterOn)
            {
                btMaster.BackColor = Color.Yellow;
                btMaster.Text = "MSTR";
                btMaster.ForeColor = Color.Black;
                masterOn = true;
            }
            else
            {
                btMaster.BackColor = Color.Red;
                btMaster.Text = "MSTR";
                btMaster.ForeColor = Color.Black;
                masterOn = false;
            }

            if (mf.SwitchBox.SwitchIsOn(SwIDs.RateUp))
            {
                btnUp.BackColor = Color.Blue;
            }
            else
            {
                btnUp.BackColor = Properties.Settings.Default.MainBackColour;
            }

            if (mf.SwitchBox.SwitchIsOn(SwIDs.RateDown))
            {
                btnDown.BackColor = Color.Blue;
            }
            else
            {
                btnDown.BackColor = Properties.Settings.Default.MainBackColour;
            }
        }

        private void verticalProgressBar0_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Indicates quantity remaining. Click to select product and view" +
                " the product's information.";

            Props.ShowMessage(Message);
            hlpevent.Handled = true;
        }

        private void verticalProgressBar1_Click(object sender, EventArgs e)
        {
            cCurrentProduct = mf.Products.Item(0);
            UpdateForm();
        }

        private void verticalProgressBar2_Click(object sender, EventArgs e)
        {
            cCurrentProduct = mf.Products.Item(1);
            UpdateForm();
        }

        private void verticalProgressBar3_Click(object sender, EventArgs e)
        {
            cCurrentProduct = mf.Products.Item(2);
            UpdateForm();
        }

        private void verticalProgressBar4_Click(object sender, EventArgs e)
        {
            cCurrentProduct = mf.Products.Item(3);
            UpdateForm();
        }
    }
}