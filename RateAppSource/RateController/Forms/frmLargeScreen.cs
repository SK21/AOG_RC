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
        public FormStart mf;
        public clsProduct Prd;
        public clsAlarm RCalarm;
        private int Fan1RateType = 0;
        private int Fan2RateType = 0;
        private bool IsTransparent = false;
        private bool masterOn;
        private bool MasterPressed;
        private Color RateColour = Color.GreenYellow;
        private bool SwitchingScreens = false;
        private int TransLeftOffset = 6;
        private int TransTopOffset = 30;
        private Point MouseDownLocation;

        public frmLargeScreen(FormStart CallingForm)
        {
            InitializeComponent();

            #region // language

            lbTarget.Text = Lang.lgTargetRate;
            lbCoverage.Text = Lang.lgCoverage;
            lbQuantity.Text = Lang.lgTank_Remaining;
            lbUnits.Text = Lang.lgApplied;

            mnuSettings.Items["MnuProducts"].Text = Lang.lgProducts;
            mnuSettings.Items["MnuSections"].Text = Lang.lgSections;
            mnuSettings.Items["MnuRelays"].Text = Lang.lgRelays;
            mnuSettings.Items["MnuComm"].Text = Lang.lgComm;
            mnuSettings.Items["calibrateToolStripMenuItem1"].Text = Lang.lgCalibrate;
            mnuSettings.Items["networkToolStripMenuItem"].Text = Lang.lgModules;
            mnuSettings.Items["MnuOptions"].Text = Lang.lgOptions;
            mnuSettings.Items["exitToolStripMenuItem"].Text = Lang.lgExit;

            mnuSettings.Items["commDiagnosticsToolStripMenuItem1"].Text = Lang.lgCommDiagnostics;
            mnuSettings.Items["newToolStripMenuItem"].Text = Lang.lgNew;
            mnuSettings.Items["openToolStripMenuItem"].Text = Lang.lgOpen;
            mnuSettings.Items["saveAsToolStripMenuItem"].Text = Lang.lgSaveAs;

            #endregion // language

            mf = CallingForm;
            Prd = mf.Products.Item(0);
            RCalarm = new clsAlarm(mf, btAlarm);

            mf.SwitchBox.SwitchPGNreceived += SwitchBox_SwitchPGNreceived;

            this.BackColor = Properties.Settings.Default.MainBackColour;
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
            mf.ColorChanged += Mf_ColorChanged;
        }

        public int CurrentProduct()
        {
            return Prd.ID;
        }

        public void SetTransparent()
        {
            IsTransparent = mf.UseTransparent;
            if (mf.UseTransparent)
            {
                this.Text = string.Empty;
                this.TransparencyKey = Properties.Settings.Default.MainBackColour;
                //this.Opacity = .5;
                this.HelpButton = false;
                this.ControlBox = false;
                this.FormBorderStyle = FormBorderStyle.None;
                this.Top += TransTopOffset;
                this.Left += TransLeftOffset;
                //SetDisplay(Properties.Settings.Default.ForeColour);
                SetDisplay(Color.Yellow);
            }
            else
            {
                this.Text = "RateController";
                this.TransparencyKey = Color.Transparent;
                //this.Opacity = 100;
                this.HelpButton = true;
                this.ControlBox = true;
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.Top += -TransTopOffset;
                this.Left += -TransLeftOffset;
                SetDisplay(SystemColors.ControlText);
            }
            SetFont();
        }

        public void SwitchToStandard()
        {
            this.ShowInTaskbar = false;
            mf.ShowInTaskbar = true;
            SwitchingScreens = true;
            this.Close();
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
            Form restoreform = new RCRestore(this, mf.RateType, Prd,mf);
            restoreform.Show();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (mf.SwitchBox.AutoRateOn)
            {
                Prd.RateSet = Prd.RateSet / 1.05;
            }
            else
            {
                Prd.ManualPWM -= 5;
            }
        }

        private void btnFan1_Click(object sender, EventArgs e)
        {
            clsProduct fn = mf.Products.Item(mf.MaxProducts - 2);
            fn.FanOn = !fn.FanOn;
        }

        private void btnFan2_Click(object sender, EventArgs e)
        {
            clsProduct fn = mf.Products.Item(mf.MaxProducts - 1);
            fn.FanOn = !fn.FanOn;
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            Button btnSender = (Button)sender;
            Point ptLowerLeft = new Point(0, btnSender.Height);
            ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
            mnuSettings.Show(ptLowerLeft);
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (mf.SwitchBox.AutoRateOn)
            {
                Prd.RateSet = Prd.RateSet * 1.05;
            }
            else
            {
                Prd.ManualPWM += 5;
            }
        }

        private void calibrateToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //check if window already exists
            Form fs = mf.Tls.IsFormOpen("frmCalibrate");

            if (fs == null)
            {
                Form frm = new frmCalibrate(mf);
                frm.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private void commDiagnosticsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form fs = mf.Tls.IsFormOpen("frmModule");

            if (fs == null)
            {
                Form frm = new frmModule(mf);
                frm.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmLargeScreen_Activated(object sender, EventArgs e)
        {
            lbName0.Focus();
        }

        private void frmLargeScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            mf.Tls.SaveFormData(this);

            timerMain.Enabled = false;
            if (mf.UseTransparent)
            {
                // move the window back to the default location
                this.Top += -TransTopOffset;
                this.Left += -TransLeftOffset;
            }

            if (mf.UseLargeScreen) mf.LargeScreenExit = true;
            mf.WindowState = FormWindowState.Normal;
            mf.vSwitchBox.LargeScreenOn = false;
        }

        private void frmLargeScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!SwitchingScreens && !mf.Restart && mf.Products.Connected())
            {
                var Hlp = new frmMsgBox(mf, "Confirm Exit?", "Exit", true);
                Hlp.TopMost = true;
                Hlp.ShowDialog();
                bool Result = Hlp.Result;
                Hlp.Close();
                if (!Result) e.Cancel = true;
            }
        }

        private void frmLargeScreen_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            Prd = mf.Products.Item(mf.DefaultProduct);

            UpdateForm();
            timerMain.Enabled = true;
            SwitchingScreens = false;
            mf.vSwitchBox.LargeScreenOn = true;
            mf.vSwitchBox.PressSwitch(SwIDs.MasterOff);
            tmrRelease.Enabled = true;
            UpdateSwitches();
            UpdateForm();
        }

        private void lbCoverage_Click(object sender, EventArgs e)
        {
            mf.ShowCoverageRemaining = !mf.ShowCoverageRemaining;
            UpdateForm();
        }

        private void lbCoverage_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Shows either coverage applied (A) or area that can be done with the remaining quantity (R)." +
                "\n Press to change.";

            mf.Tls.ShowHelp(Message, "Coverage");
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

        private void lbFan1_Click(object sender, EventArgs e)
        {
            //check if window already exists
            Form fs = mf.Tls.IsFormOpen("FormSettings");

            if (fs != null)
            {
                fs.Focus();
                return;
            }

            Form frm = new FormSettings(mf, mf.MaxProducts - 1);
            frm.Show();
        }

        private void lbFan2_Click(object sender, EventArgs e)
        {
            //check if window already exists
            Form fs = mf.Tls.IsFormOpen("FormSettings");

            if (fs != null)
            {
                fs.Focus();
                return;
            }

            Form frm = new FormSettings(mf, mf.MaxProducts);
            frm.Show();
        }

        private void lbName0_Click(object sender, EventArgs e)
        {
            ShowSettings(0);
        }

        private void lbName0_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Indicates if the sensor is connected. Click to access settings.";

            mf.Tls.ShowHelp(Message);
            hlpevent.Handled = true;
        }

        private void lbName1_Click(object sender, EventArgs e)
        {
            ShowSettings(1);
        }

        private void lbName2_Click(object sender, EventArgs e)
        {
            ShowSettings(2);
        }

        private void lbName3_Click(object sender, EventArgs e)
        {
            ShowSettings(3);
        }

        private void lbQuantity_Click(object sender, EventArgs e)
        {
            mf.ShowQuantityRemaining = !mf.ShowQuantityRemaining;
            UpdateForm();
        }

        private void lbQuantity_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Shows either quantity applied (A) or quantity remaining (R)." +
                "\n Press to change.";

            mf.Tls.ShowHelp(Message, "Remaining");
            hlpevent.Handled = true;
        }

        private void lbQuantityAmount_Click(object sender, EventArgs e)
        {
            //check if window already exists
            Form fs = mf.Tls.IsFormOpen("frmResetQuantity");

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
            if (mf.RateType < 1)
            {
                mf.RateType++;
            }
            else
            {
                mf.RateType = 0;
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

            mf.Tls.ShowHelp(Message, "Rate");
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
            if (Prd.UseAltRate)
            {
                Prd.UseAltRate = false;
                lbTargetType.Text = "T";
            }
            else
            {
                Prd.UseAltRate = true;
                lbTargetType.Text = "A";
            }
        }

        private void lbTarget_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Press to switch between base rate (R) and alternate rate (A).";

            mf.Tls.ShowHelp(Message, "Target Rate");
            hlpevent.Handled = true;
        }

        private void mainform_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && mf.UseTransparent)
            {
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
                tmrBorder.Start();
            }
        }

        private void Mf_ColorChanged(object sender, EventArgs e)
        {
            //SetDisplay(Properties.Settings.Default.ForeColour);
        }

        private void MnuComm_Click(object sender, EventArgs e)
        {
            Form fs = mf.Tls.IsFormOpen("frmComm");

            if (fs == null)
            {
                Form frm = new frmComm(mf);
                frm.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private void MnuOptions_Click(object sender, EventArgs e)
        {
            Form fs = mf.Tls.IsFormOpen("frmOptions");

            if (fs == null)
            {
                Form frm = new frmOptions(mf);
                frm.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private void MnuProducts_Click(object sender, EventArgs e)
        {
            //check if window already exists
            Form fs = mf.Tls.IsFormOpen("FormSettings");

            if (fs != null)
            {
                fs.Focus();
                return;
            }

            Form frm = new FormSettings(mf, Prd.ID + 1);
            frm.Show();
        }

        private void MnuRelays_Click(object sender, EventArgs e)
        {
            Form fs = mf.Tls.IsFormOpen("frmRelays");

            if (fs != null)
            {
                fs.Focus();
                return;
            }

            Form frm = new frmRelays(mf);
            frm.Show();
        }

        private void MnuSections_Click(object sender, EventArgs e)
        {
            Form fs = mf.Tls.IsFormOpen("frmSections");

            if (fs != null)
            {
                fs.Focus();
                return;
            }

            Form frm = new frmSections(mf);
            frm.Show();
        }

        private void mouseMove_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) MouseDownLocation = e.Location;
        }

        private void mouseMove_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) this.Location = new Point(this.Left + e.X - MouseDownLocation.X, this.Top + e.Y - MouseDownLocation.Y);
        }

        private void networkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form fs = mf.Tls.IsFormOpen("frmModuleConfig");

            if (fs == null)
            {
                Form frm = new frmModuleConfig(mf);
                frm.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mf.NewFile();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mf.OpenFile();
        }

        private void pbRate0_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Indicates Current rate compared to Target rate." +
                " Target rate is the centre of the graph. " +
                "Within 10 % of target, the graph is dark green, otherwise red." +
                " Click to select product and view" +
                " the product's information.";

            mf.Tls.ShowHelp(Message);
            hlpevent.Handled = true;
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mf.SaveFileAs();
        }

        private void SetDisplay(Color NewColor)
        {
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
        }

        private void SetFont()
        {
            if (mf.UseTransparent)
            {
                string TransparentFont = "MS Gothic";
                //string TransparentFont = "Courier New";
                //string TransparentFont = "Candara Light";
                //string TransparentFont = "Tahoma";

                foreach (Control Ctrl in Controls)
                {
                    if (Ctrl.Name == "lbRateAmount" || Ctrl.Name == "lbTargetAmount"
                       || Ctrl.Name == "lbCoverageAmount" || Ctrl.Name == "lbQuantityAmount")
                    {
                        Ctrl.Font = new Font(TransparentFont, 16, FontStyle.Bold);
                    }
                    else if (Ctrl.Name == "btAuto" || Ctrl.Name == "btMaster")
                    {
                        Ctrl.Font = new Font(TransparentFont, 10, FontStyle.Bold);
                    }
                    else
                    {
                        Ctrl.Font = new Font(TransparentFont, 14, FontStyle.Bold);
                    }
                }
            }
            else
            {
                foreach (Control Ctrl in Controls)
                {
                    if (Ctrl.Name == "btAuto" || Ctrl.Name == "btMaster")
                    {
                        Ctrl.Font = new Font("MS Gothic", 10, FontStyle.Bold);
                    }
                    else
                    {
                        Ctrl.Font = new Font("Tahoma", 14);
                    }
                }
            }
        }

        private void ShowProducts()
        {
            clsProduct Prd = mf.Products.Item(0);
            lbName0.Visible = Prd.OnScreen;
            pnlRate0.Visible = Prd.OnScreen;
            pnlQuantity0.Visible = Prd.OnScreen;
            pnlSelect0.Visible = Prd.OnScreen;

            Prd = mf.Products.Item(1);
            lbName1.Visible = Prd.OnScreen;
            pnlRate1.Visible = Prd.OnScreen;
            pnlQuantity1.Visible = Prd.OnScreen;
            pnlSelect1.Visible = Prd.OnScreen;

            Prd = mf.Products.Item(2);
            lbName2.Visible = Prd.OnScreen;
            pnlRate2.Visible = Prd.OnScreen;
            pnlQuantity2.Visible = Prd.OnScreen;
            pnlSelect2.Visible = Prd.OnScreen;

            Prd = mf.Products.Item(3);
            lbName3.Visible = Prd.OnScreen;
            pnlRate3.Visible = Prd.OnScreen;
            pnlQuantity3.Visible = Prd.OnScreen;
            pnlSelect3.Visible = Prd.OnScreen;

            Prd = mf.Products.Item(4);
            lbFan1.Visible = Prd.OnScreen;
            lbRPM1.Visible = Prd.OnScreen;
            btnFan1.Visible = Prd.OnScreen;

            Prd = mf.Products.Item(5);
            lbFan2.Visible = Prd.OnScreen;
            lbRPM2.Visible = Prd.OnScreen;
            btnFan2.Visible = Prd.OnScreen;

            for (int i = 0; i < 5; i++)
            {
                Prd = mf.Products.Item(i);
                if (i == 4)
                {
                    btnDown.Visible = false;
                    btnDown.Enabled = false;
                    btnUp.Visible = false;
                    btnUp.Enabled = false;
                }
                else if (Prd.BumpButtons)
                {
                    btnUp.Visible = true;
                    btnDown.Visible = true;
                    btnUp.Enabled = true;
                    btnDown.Enabled = true;

                    Label posLbl = (Label)(this.Controls.Find("lbName" + i, true)[0]);
                    ProgressBar posPb = (ProgressBar)(this.Controls.Find("pbRate" + i, true)[0]);

                    int posX = posLbl.Left;
                    int posY = posLbl.Top;
                    int Width = posLbl.Width;
                    int Height = posPb.Height + posLbl.Height;

                    btnUp.Left = posX;
                    btnDown.Left = posX;
                    btnUp.Width = Width;
                    btnDown.Width = Width;
                    btnUp.Top = posY;
                    btnUp.Height = (Height + 10) / 2;
                    btnDown.Top = posY + btnUp.Height + 6;
                    btnDown.Height = btnUp.Height;
                    break;
                }
            }
        }

        private void ShowSettings(int ProductID)
        {
            Prd = mf.Products.Item(ProductID);
            UpdateForm();

            //check if window already exists
            Form fs = mf.Tls.IsFormOpen("FormSettings");

            if (fs != null)
            {
                fs.Focus();
                return;
            }

            Form frm = new FormSettings(mf, Prd.ID + 1);
            frm.Show();
        }

        private void SwitchBox_SwitchPGNreceived(object sender, EventArgs e)
        {
            UpdateSwitches();
        }

        private void timerMain_Tick(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void tmrBorder_tick(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            tmrBorder.Stop();
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
            if (mf.UseTransparent != IsTransparent) SetTransparent();

            this.Text = "RC [" + Path.GetFileNameWithoutExtension(Properties.Settings.Default.FileName) + "]";

            if (Prd.UseVR)
            {
                lbTargetType.Text = "V";
            }
            else if (Prd.UseAltRate)
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
            switch (Prd.ID)
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
            if (Product.RateSensor.ModuleSending())
            {
                if (Product.RateSensor.ModuleReceiving())
                {
                    lbName0.BackColor = Color.LightGreen;
                }
                else
                {
                    lbName0.BackColor = Color.LightBlue;
                }
            }
            else
            {
                lbName0.BackColor = Color.Red;
            }

            // 1
            Product = mf.Products.Item(1);
            if (Product.RateSensor.ModuleSending())
            {
                if (Product.RateSensor.ModuleReceiving())
                {
                    lbName1.BackColor = Color.LightGreen;
                }
                else
                {
                    lbName1.BackColor = Color.LightBlue;
                }
            }
            else
            {
                lbName1.BackColor = Color.Red;
            }

            // 2
            Product = mf.Products.Item(2);
            if (Product.RateSensor.ModuleSending())
            {
                if (Product.RateSensor.ModuleReceiving())
                {
                    lbName2.BackColor = Color.LightGreen;
                }
                else
                {
                    lbName2.BackColor = Color.LightBlue;
                }
            }
            else
            {
                lbName2.BackColor = Color.Red;
            }

            // 3
            Product = mf.Products.Item(3);
            if (Product.RateSensor.ModuleSending())
            {
                if (Product.RateSensor.ModuleReceiving())
                {
                    lbName3.BackColor = Color.LightGreen;
                }
                else
                {
                    lbName3.BackColor = Color.LightBlue;
                }
            }
            else
            {
                lbName3.BackColor = Color.Red;
            }

            // rate
            switch (mf.RateType)
            {
                case 1:
                    lbRateType.Text = "I";
                    lbRateAmount.Text = Prd.CurrentRate().ToString("N1");
                    break;

                case 2:
                    lbRateType.Text = "O";
                    lbRateAmount.Text = Prd.AverageRate().ToString("N1");
                    break;

                default:
                    lbRateType.Text = "C";
                    lbRateAmount.Text = Prd.SmoothRate().ToString("N1");
                    break;
            }

            lbTargetAmount.Text = Prd.TargetRate().ToString("N1");

            // coverage
            if (mf.ShowCoverageRemaining)
            {
                lbCoverageType.Text = "R";
                double RT = Prd.SmoothRate();
                if (RT == 0) RT = Prd.TargetRate();

                if ((RT > 0) & (Prd.TankStart > 0))
                {
                    lbCoverageAmount.Text = ((Prd.TankStart - Prd.UnitsApplied()) / RT).ToString("N1");
                }
                else
                {
                    lbCoverageAmount.Text = "0.0";
                }
            }
            else
            {
                // show amount done
                lbCoverageAmount.Text = Prd.CurrentCoverage().ToString("N1");
                lbCoverageType.Text = "A";
            }
            lbCoverage.Text = Prd.CoverageDescription();

            // quantity
            if (mf.ShowQuantityRemaining)
            {
                lbQuantityType.Text = "R";
                // calculate remaining
                lbQuantityAmount.Text = (Prd.TankStart - Prd.UnitsApplied()).ToString("N0");
            }
            else
            {
                // show amount done
                lbQuantityType.Text = "A";
                lbQuantityAmount.Text = Prd.UnitsApplied().ToString("N0");
            }
            lbQuantity.Text = Prd.QuantityDescription;

            // aog
            if (mf.SimMode == SimType.Sim_Speed)
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

            clsProduct prd = mf.Products.Item(mf.MaxProducts - 2);

            if (Fan1RateType == 1)
            {
                lbRPM1.Text = prd.CurrentRate().ToString("N0") + " RPM-I";
            }
            else
            {
                lbRPM1.Text = prd.SmoothRate().ToString("N0") + " RPM";
            }

            if (prd.RateSensor.ModuleSending())
            {
                if (prd.RateSensor.ModuleReceiving())
                {
                    lbFan1.BackColor = Color.LightGreen;
                }
                else
                {
                    lbFan1.BackColor = Color.LightBlue;
                }
            }
            else
            {
                lbFan1.BackColor = Color.Red;
            }

            // fan 2
            prd = mf.Products.Item(mf.MaxProducts - 1);
            if (Fan2RateType == 1)
            {
                lbRPM2.Text = prd.CurrentRate().ToString("N0") + " RPM-I";
            }
            else
            {
                lbRPM2.Text = prd.SmoothRate().ToString("N0") + " RPM";
            }

            if (prd.RateSensor.ModuleSending())
            {
                if (prd.RateSensor.ModuleReceiving())
                {
                    lbFan2.BackColor = Color.LightGreen;
                }
                else
                {
                    lbFan2.BackColor = Color.LightBlue;
                }
            }
            else
            {
                lbFan2.BackColor = Color.Red;
            }

            // fan 1 button
            clsProduct fn = mf.Products.Item(mf.MaxProducts - 2);
            if (fn.FanOn)
            {
                btnFan1.Image = Properties.Resources.FanOn;
            }
            else
            {
                btnFan1.Image = Properties.Resources.FanOff;
            }

            // fan 2 button
            fn = mf.Products.Item(mf.MaxProducts - 1);
            if (fn.FanOn)
            {
                btnFan2.Image = Properties.Resources.FanOn;
            }
            else
            {
                btnFan2.Image = Properties.Resources.FanOff;
            }

            RCalarm.CheckAlarms();
            ShowProducts();
        }

        private void UpdateSwitches()
        {
            // auto button
            if (mf.SwitchBox.SwitchIsOn(SwIDs.AutoRate)||mf.SwitchBox.SwitchIsOn(SwIDs.AutoSection))
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

            mf.Tls.ShowHelp(Message);
            hlpevent.Handled = true;
        }

        private void verticalProgressBar1_Click(object sender, EventArgs e)
        {
            Prd = mf.Products.Item(0);
            UpdateForm();
        }

        private void verticalProgressBar2_Click(object sender, EventArgs e)
        {
            Prd = mf.Products.Item(1);
            UpdateForm();
        }

        private void verticalProgressBar3_Click(object sender, EventArgs e)
        {
            Prd = mf.Products.Item(2);
            UpdateForm();
        }

        private void verticalProgressBar4_Click(object sender, EventArgs e)
        {
            Prd = mf.Products.Item(3);
            UpdateForm();
        }
    }
}