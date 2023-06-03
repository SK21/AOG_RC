using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmLargeScreen : Form
    {
        private FormStart mf;
        private clsProduct Prd;
        private int RateType = 0;   // 0 current rate, 1 instantaneous rate, 2 overall rate
        private int Fan1RateType = 0;
        private int Fan2RateType = 0;
        private bool ShowCoverageRemaining = false;
        private bool ShowQuantityRemaining = true;
        public clsAlarm RCalarm;
        private bool SwitchingScreens = false;
        private Color RateColour = Color.DarkOliveGreen;
        private int TransTopOffset = 30;
        private int TransLeftOffset = 6;
        private int windowTop = 0;
        private int windowLeft = 0;
        private int mouseX = 0;
        private int mouseY = 0;
        private bool[] SwON = new bool[9];
        private bool masterOn;
        private bool automode = true;

        public frmLargeScreen(FormStart CallingForm)
        {
            InitializeComponent();

            #region // language

            lbRate.Text = Lang.lgCurrentRate;
            lbTarget.Text = Lang.lgTargetRate;
            lbCoverage.Text = Lang.lgCoverage;
            lbQuantity.Text = Lang.lgTank_Remaining + " ...";

            mnuSettings.Items["MnuProducts"].Text = Lang.lgProducts;
            mnuSettings.Items["MnuSections"].Text = Lang.lgSection;
            mnuSettings.Items["MnuOptions"].Text = Lang.lgOptions;
            mnuSettings.Items["MnuComm"].Text = Lang.lgComm;
            mnuSettings.Items["MnuRelays"].Text = Lang.lgRelays;
            mnuSettings.Items["MnuPressures"].Text = Lang.lgPressure;

            MnuOptions.DropDownItems["MnuAbout"].Text = Lang.lgAbout;
            MnuOptions.DropDownItems["MnuNew"].Text = Lang.lgNew;
            MnuOptions.DropDownItems["MnuOpen"].Text = Lang.lgOpen;
            MnuOptions.DropDownItems["MnuSaveAs"].Text = Lang.lgSaveAs;
            MnuOptions.DropDownItems["MnuLanguage"].Text = Lang.lgLanguage;
            MnuOptions.DropDownItems["mnuMetric"].Text = Lang.lgMetric;
            MnuOptions.DropDownItems["mnuNetwork"].Text = Lang.lgNetwork;

            #endregion // language

            mf = CallingForm;
            Prd = mf.Products.Item(0);
            RCalarm = new clsAlarm(mf, btAlarm);

            mf.SwitchBox.SwitchPGNreceived += SwitchBox_SwitchPGNreceived;

            this.BackColor = Properties.Settings.Default.DayColour;
            pnlRate0.BackColor = Properties.Settings.Default.DayColour;
            pnlRate1.BackColor = Properties.Settings.Default.DayColour;
            pnlRate1.BackColor = Properties.Settings.Default.DayColour;
            pnlRate1.BackColor = Properties.Settings.Default.DayColour;
            pnlQuantity0.BackColor = Properties.Settings.Default.DayColour;
            pnlQuantity1.BackColor = Properties.Settings.Default.DayColour;
            pnlQuantity2.BackColor = Properties.Settings.Default.DayColour;
            pnlQuantity3.BackColor = Properties.Settings.Default.DayColour;
            btnUp.BackColor = Properties.Settings.Default.DayColour;
            btnDown.BackColor = Properties.Settings.Default.DayColour;


            foreach(Control Ctrl in Controls)
            {
                if(Ctrl.Name !="btnSettings" && Ctrl.Name!="btAuto")
                {
                    Ctrl.MouseDown += mouseMove_MouseDown;
                    Ctrl.MouseMove += mouseMove_MouseMove;
                }
                else if (Ctrl.Name == "btAuto")
                {
                    Ctrl.MouseDown += btAuto_MouseDown;
                }
            }
        }

        private void SetFont()
        {
            if (transparentToolStripMenuItem.Checked)
            {
                string TransparentFont = "MS Gothic";
                //string TransparentFont = "Courier New";
                //string TransparentFont = "Candara Light";

                foreach (Control Ctrl in Controls)
                {
                    if (Ctrl.Name != "lbName0" && Ctrl.Name != "lbName1" && Ctrl.Name != "lbName2" && Ctrl.Name != "lbName3"
                         && Ctrl.Name != "lbFan1" && Ctrl.Name != "lbFan2" && Ctrl.Name != "btAuto" && Ctrl.Name != "lblManAuto")
                    {
                        Ctrl.Font = new Font(TransparentFont, 14, FontStyle.Bold);
                    }
                    else if (Ctrl.Name == "btAuto" || Ctrl.Name == "lblManAuto")
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
                    Ctrl.Font = new Font("Tahoma", 14);
                }
            }
        }

        public void SetTransparent(bool frmtrans)
        {
            transparentToolStripMenuItem.Checked = frmtrans;
            if (transparentToolStripMenuItem.Checked)
            {
                mf.UseTransparent = true;
                this.Text = string.Empty;
                this.TransparencyKey = (Properties.Settings.Default.IsDay) ? Properties.Settings.Default.DayColour : Properties.Settings.Default.NightColour;
                //this.Opacity = 0;
                this.HelpButton = false;
                this.ControlBox = false;
                this.FormBorderStyle = FormBorderStyle.None;
                this.Top += TransTopOffset;
                this.Left += TransLeftOffset;
                Color txtcolor = SystemColors.ControlLightLight;
                lbRate.ForeColor = txtcolor;
                lbTarget.ForeColor = txtcolor;
                lbCoverage.ForeColor = txtcolor;
                lbQuantity.ForeColor = txtcolor;
                lbRateAmount.ForeColor = txtcolor;
                lbTargetAmount.ForeColor = txtcolor;
                lbCoverageAmount.ForeColor = txtcolor;
                lbQuantityAmount.ForeColor = txtcolor;
                lbRPM1.ForeColor = txtcolor;
                lbRPM2.ForeColor = txtcolor;
                lbUnits.ForeColor = txtcolor;
                lblManAuto.ForeColor = txtcolor;

                //btnMenu.BackColor = (Properties.Settings.Default.IsDay) ? Properties.Settings.Default.NightColour : Properties.Settings.Default.DayColour;


            }
            else
            {
                mf.UseTransparent = false;
                this.Text = "RateController";
                this.TransparencyKey = Color.Transparent;
                //this.Opacity = 100;
                this.HelpButton = true;
                this.ControlBox = true;
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.Top += -TransTopOffset;
                this.Left += -TransLeftOffset;

                Color txtcolor = SystemColors.ControlText;
                lbRate.ForeColor = txtcolor;
                lbTarget.ForeColor = txtcolor;
                lbCoverage.ForeColor = txtcolor;
                lbQuantity.ForeColor = txtcolor;
                lbRateAmount.ForeColor = txtcolor;
                lbTargetAmount.ForeColor = txtcolor;
                lbCoverageAmount.ForeColor = txtcolor;
                lbQuantityAmount.ForeColor = txtcolor;
                lbRPM1.ForeColor = txtcolor;
                lbRPM2.ForeColor = txtcolor;
                lbUnits.ForeColor = txtcolor;
                lblManAuto.ForeColor = txtcolor;

                //btnMenu.BackColor = Color.Transparent;

            }
            SetFont();
        }

        public int CurrentProduct()
        {
            return Prd.ID;
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            Button btnSender = (Button)sender;
            Point ptLowerLeft = new Point(0, btnSender.Height);
            ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
            mnuSettings.Show(ptLowerLeft);
        }

        private void frmLargeScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                mf.Tls.SaveFormData(this);
            }

            timerMain.Enabled = false;
            if (mf.UseTransparent)
            {
                // move the window back to the default location
                this.Top += -TransTopOffset;
                this.Left += -TransLeftOffset;
            }

            if (mf.UseLargeScreen) mf.LargeScreenExit = true;
            mf.WindowState = FormWindowState.Normal;
        }
        
        private void frmLargeScreen_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            Prd = mf.Products.Item(mf.DefaultProduct);

            UpdateForm();
            timerMain.Enabled = true;
            SwitchingScreens = false;
        }

        private void lbCoverage_Click(object sender, EventArgs e)
        {
            ShowCoverageRemaining = !ShowCoverageRemaining;
            UpdateForm();
        }

        private void lbFan1_Click(object sender, EventArgs e)
        {
            //check if window already exists
            Form fs = Application.OpenForms["FormSettings"];

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
            Form fs = Application.OpenForms["FormSettings"];

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
            Prd = mf.Products.Item(0);
            UpdateForm();

            //check if window already exists
            Form fs = Application.OpenForms["FormSettings"];

            if (fs != null)
            {
                fs.Focus();
                return;
            }

            Form frm = new FormSettings(mf, Prd.ID + 1);
            frm.Show();
        }

        private void lbName1_Click(object sender, EventArgs e)
        {
            Prd = mf.Products.Item(1);
            UpdateForm();

            //check if window already exists
            Form fs = Application.OpenForms["FormSettings"];

            if (fs != null)
            {
                fs.Focus();
                return;
            }

            Form frm = new FormSettings(mf, Prd.ID + 1);
            frm.Show();
        }

        private void lbName2_Click(object sender, EventArgs e)
        {
            Prd = mf.Products.Item(2);
            UpdateForm();

            //check if window already exists
            Form fs = Application.OpenForms["FormSettings"];

            if (fs != null)
            {
                fs.Focus();
                return;
            }

            Form frm = new FormSettings(mf, Prd.ID + 1);
            frm.Show();
        }

        private void lbName3_Click(object sender, EventArgs e)
        {
            Prd = mf.Products.Item(3);
            UpdateForm();

            //check if window already exists
            Form fs = Application.OpenForms["FormSettings"];

            if (fs != null)
            {
                fs.Focus();
                return;
            }

            Form frm = new FormSettings(mf, Prd.ID + 1);
            frm.Show();
        }

        private void lbQuantity_Click(object sender, EventArgs e)
        {
            ShowQuantityRemaining = !ShowQuantityRemaining;
            UpdateForm();
        }

        private void lbRate_Click(object sender, EventArgs e)
        {
            RateType++;
            if (RateType > 2) RateType = 0;
            UpdateForm();
        }

        private void lbTarget_Click(object sender, EventArgs e)
        {
            if (lbTarget.Text == Lang.lgTargetRate)
            {
                lbTarget.Text = Lang.lgTargetRateAlt;
                Prd.UseAltRate = true;
            }
            else
            {
                lbTarget.Text = Lang.lgTargetRate;
                Prd.UseAltRate = false;
            }
        }

        private void MnuAbout_Click(object sender, EventArgs e)
        {
            //check if window already exists
            Form fs = Application.OpenForms["FormAbout"];

            if (fs != null)
            {
                fs.Focus();
                return;
            }

            Form frm = new FormAbout(mf);
            frm.Show();
        }

        private void MnuComm_Click(object sender, EventArgs e)
        {
            Form frm = new frmComm(mf);
            frm.ShowDialog();
        }

        private void MnuDeustch_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.setF_culture = "de";
            Properties.Settings.Default.Save();
            mf.Restart = true;
            this.Close();
        }

        private void MnuEnglish_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.setF_culture = "en";
            Properties.Settings.Default.Save();
            mf.Restart = true;
            this.Close();
        }

        private void mnuMetric_Click(object sender, EventArgs e)
        {
            mf.UseInches = !mf.UseInches;
            mf.Tls.SaveProperty("UseInches", mf.UseInches.ToString());
        }

        private void MnuNederlands_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.setF_culture = "nl";
            Properties.Settings.Default.Save();
            mf.Restart = true;
            this.Close();
        }

        private void mnuNetwork_Click(object sender, EventArgs e)
        {
            Form frmWifi = new frmWifi(mf);
            frmWifi.ShowDialog();
        }

        private void MnuNew_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = mf.Tls.FilesDir();
            saveFileDialog1.Title = "New File";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                    mf.Tls.OpenFile(saveFileDialog1.FileName);
                    mf.LoadSettings();
                    UpdateForm();
                }
            }
        }

        private void MnuOpen_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = mf.Tls.FilesDir();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                mf.Tls.PropertiesFile = openFileDialog1.FileName;
                mf.Products.Load();
                mf.LoadSettings();
                UpdateForm();
            }
        }

        private void MnuPressures_Click(object sender, EventArgs e)
        {
            Form frmPressure = new FormPressure(mf);
            frmPressure.ShowDialog();
        }

        private void MnuProducts_Click(object sender, EventArgs e)
        {
            //check if window already exists
            Form fs = Application.OpenForms["FormSettings"];

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
            Form tmp = new frmRelays(mf);
            tmp.ShowDialog();
        }

        private void MnuSaveAs_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = mf.Tls.FilesDir();
            saveFileDialog1.Title = "Save As";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                    mf.Tls.SaveFile(saveFileDialog1.FileName);
                    mf.LoadSettings();
                    UpdateForm();
                }
            }
        }

        private void MnuSections_Click(object sender, EventArgs e)
        {
            Form Sec = new frmSections(mf);
            Sec.ShowDialog();
        }

        private void mnuStandard_Click(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;
            mf.ShowInTaskbar = true;
            mf.UseLargeScreen = false;
            SwitchingScreens = true;
            this.Close();
        }

        private void russianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.setF_culture = "ru";
            Properties.Settings.Default.Save();
            mf.Restart = true;
            this.Close();
        }

        private void serialMonitorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form Monitor = new frmMonitor(mf);
            Monitor.Show();
        }

        private void timerMain_Tick(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void UpdateForm()
        {
            this.Text = "RC [" + Path.GetFileNameWithoutExtension(Properties.Settings.Default.FileName) + "]";
            
            // set highlight
            if (mf.SimMode == SimType.VirtualNano)
            {
                pnlSelect0.BackColor = mf.SimColor;
                pnlSelect1.BackColor = mf.SimColor;
                pnlSelect2.BackColor = mf.SimColor;
                pnlSelect3.BackColor = mf.SimColor;
            }
            else
            {
                pnlSelect0.BackColor = Properties.Settings.Default.DayColour;
                pnlSelect1.BackColor = Properties.Settings.Default.DayColour;
                pnlSelect2.BackColor = Properties.Settings.Default.DayColour;
                pnlSelect3.BackColor = Properties.Settings.Default.DayColour;
            }
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

            if (mf.SimMode == SimType.None)
            {
                // 0
                clsProduct Product = mf.Products.Item(0);
                if (Product.ArduinoModule.ModuleSending())
                {
                    if (Product.ArduinoModule.ModuleReceiving())
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
                if (Product.ArduinoModule.ModuleSending())
                {
                    if (Product.ArduinoModule.ModuleReceiving())
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
                if (Product.ArduinoModule.ModuleSending())
                {
                    if (Product.ArduinoModule.ModuleReceiving())
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
                if (Product.ArduinoModule.ModuleSending())
                {
                    if (Product.ArduinoModule.ModuleReceiving())
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
            }
            else
            {
                lbName0.BackColor = mf.SimColor;
                lbName1.BackColor = mf.SimColor;
                lbName2.BackColor = mf.SimColor;
                lbName3.BackColor = mf.SimColor;
            }

            // rate
            switch (RateType)
            {
                case 1:
                    lbRate.Text = Lang.lgInstantRate;
                    lbRateAmount.Text = Prd.CurrentRate().ToString("N1");
                    break;

                case 2:
                    lbRate.Text = Lang.lgOverallRate;
                    lbRateAmount.Text = Prd.AverageRate().ToString("N1");
                    break;

                default:
                    lbRate.Text = Lang.lgCurrentRate;
                    lbRateAmount.Text = Prd.SmoothRate().ToString("N1");
                    break;
            }

            lbTargetAmount.Text = Prd.TargetRate().ToString("N1");
            lbUnits.Text = Prd.Units();

            // coverage
            if (ShowCoverageRemaining)
            {
                lbCoverage.Text = mf.CoverageDescriptions[Prd.CoverageUnits] + " Left ...";
                double RT = Prd.SmoothRate();
                if (RT == 0) RT = Prd.TargetRate();

                if (Prd.ControlType == ControlTypeEnum.MotorWeights)
                {
                    // using weights
                    if (Prd.Scale.Counts > 0)
                    {
                        lbCoverageAmount.Text = (Prd.CurrentWeight() / RT).ToString("N1");
                    }
                    else
                    {
                        lbCoverageAmount.Text = "0.0";
                    }
                }
                else
                {
                    if ((RT > 0) & (Prd.TankStart > 0))
                    {
                        lbCoverageAmount.Text = ((Prd.TankStart - Prd.UnitsApplied()) / RT).ToString("N1");
                    }
                    else
                    {
                        lbCoverageAmount.Text = "0.0";
                    }
                }
            }
            else
            {
                // show amount done
                lbCoverageAmount.Text = Prd.CurrentCoverage().ToString("N1");
                lbCoverage.Text = Prd.CoverageDescription() + " ...";
            }

            // quantity
            if (ShowQuantityRemaining)
            {
                lbQuantity.Text = Lang.lgTank_Remaining + " ...";
                if (Prd.ControlType == ControlTypeEnum.MotorWeights)
                {
                    // show weight
                    lbQuantityAmount.Text = (Prd.CurrentWeight()).ToString("N0");
                }
                else
                {
                    // calculate remaining
                    lbQuantityAmount.Text = (Prd.TankStart - Prd.UnitsApplied()).ToString("N1");
                }
            }
            else
            {
                // show amount done
                lbQuantity.Text = Lang.lgQuantityApplied + " ...";
                lbQuantityAmount.Text = Prd.UnitsApplied().ToString("N1");
            }

            // metric
            if (mf.UseInches)
            {
                MnuOptions.DropDownItems["mnuMetric"].Image = Properties.Resources.Cancel40;
            }
            else
            {
                MnuOptions.DropDownItems["mnuMetric"].Image = Properties.Resources.Check;
            }

            // transparent
            if(mf.UseTransparent)
            {
                MnuOptions.DropDownItems["transparentToolStripMenuItem"].Image = Properties.Resources.Check;
            }
            else
            {
                MnuOptions.DropDownItems["transparentToolStripMenuItem"].Image = Properties.Resources.Cancel40;
            }

            // aog
            if (mf.AutoSteerPGN.Connected())
            {
                btnMenu.Image = Properties.Resources.GreenGear;
            }
            else
            {
                btnMenu.Image = Properties.Resources.RedGear;
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
            if(RtLevel< 0) RtLevel = 0;
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

            // fans
            if (mf.SimMode == SimType.None)
            {
                // fan 1
                clsProduct prd = mf.Products.Item(mf.MaxProducts - 2);

                if (Fan1RateType == 1)
                {
                    lbRPM1.Text = prd.CurrentRate().ToString("N0") + " RPM-I";
                }
                else
                {
                    lbRPM1.Text = prd.SmoothRate().ToString("N0") + " RPM";
                }

                if (prd.ArduinoModule.ModuleSending())
                {
                    if (prd.ArduinoModule.ModuleReceiving())
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

                if (prd.ArduinoModule.ModuleSending())
                {
                    if (prd.ArduinoModule.ModuleReceiving())
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
            }
            else
            {
                lbFan1.BackColor = mf.SimColor;
                lbFan2.BackColor = mf.SimColor;
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

                    Label posLbl = (Label) (this.Controls.Find("lbName" + i, true)[0]);
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
                    btnUp.Height = (Height +10) / 2;
                    btnDown.Top = posY + btnUp.Height + 10;
                    btnDown.Height = btnUp.Height;
                    break;
                }
            }
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

        private void lbName0_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Indicates if the sensor is connected. Click to access settings.";

            mf.Tls.ShowHelp(Message);
            hlpevent.Handled = true;
        }

        private void verticalProgressBar0_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Indicates quantity remaining. Click to select product and view" +
                " the product's information.";

            mf.Tls.ShowHelp(Message);
            hlpevent.Handled = true;
        }

        private void lbRate_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "1 - Current Rate, shows" +
                " the target rate when it is within 10% of target. Outside this range it" +
                " shows the exact rate being applied. \n 2 - Instant Rate, shows the exact rate." +
                "\n 3 - Overall, averages total quantity applied over area done." +
                "\n Press to change.";

            mf.Tls.ShowHelp(Message, "Rate");
            hlpevent.Handled = true;
        }

        private void lbTarget_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Press to switch between base rate and alternate rate.";

            mf.Tls.ShowHelp(Message, "Target Rate");
            hlpevent.Handled = true;
        }

        private void lbCoverage_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Shows either coverage done or area that can be done with the remaining quantity." +
                "\n Press to change.";

            mf.Tls.ShowHelp(Message, "Coverage");
            hlpevent.Handled = true;
        }

        private void lbQuantity_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Shows either quantity applied or quantity remaining." +
                "\n Press to change.";

            mf.Tls.ShowHelp(Message, "Remaining");
            hlpevent.Handled = true;
        }

        private void btAlarm_Click(object sender, EventArgs e)
        {
            RCalarm.Silence();
        }

        private void frmLargeScreen_Activated(object sender, EventArgs e)
        {
            lbName0.Focus();
        }

        private void MnuOptions_Click(object sender, EventArgs e)
        {

        }

        private void frmLargeScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!SwitchingScreens && !mf.Restart)
            {
                var Hlp = new frmMsgBox(mf, "Confirm Exit?", "Exit", true);
                Hlp.TopMost = true;
                Hlp.ShowDialog();
                bool Result = Hlp.Result;
                Hlp.Close();
                if (!Result) e.Cancel = true;
            }
        }

        private void switchesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //check if window already exists
            Form fs = Application.OpenForms["frmSimulation"];

            if (fs == null)
            {
                Form frm = new frmSwitches(mf);
                frm.Show();
            }
            else
            {
                fs.Focus();
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

        private void transparentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transparentToolStripMenuItem.Checked = !transparentToolStripMenuItem.Checked;
            SetTransparent(transparentToolStripMenuItem.Checked);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            this.Close();

        }

        private void btAuto_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                automode = !automode;
                if (automode)
                {
                    lblManAuto.Text = "AUTO";
                }
                else
                {
                    lblManAuto.Text = "MASTER";
                }

                UpdateSwitches();
            }
        }

        private void mainform_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && transparentToolStripMenuItem.Checked)
            {
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
                tmrBorder.Start();
            }
        }

        private void tmrBorder_tick(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            tmrBorder.Stop();
        }

        private void mouseMove_MouseDown(object sender, MouseEventArgs e)
        {

            // Log the current window location and the mouse location.
            if (e.Button == MouseButtons.Right)
            {
                windowTop = this.Top;
                windowLeft = this.Left;
                mouseX = e.X;
                mouseY = e.Y;
            }
        }

        private void mouseMove_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                windowTop = this.Top;
                windowLeft = this.Left;
                
                Point pos = new Point(0,0);

                pos.X = windowLeft + e.X - mouseX;
                pos.Y = windowTop + e.Y - mouseY;
                this.Location = pos;

            }
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

        private void SwitchBox_SwitchPGNreceived(object sender, PGN32618.SwitchPGNargs e)
        {
            SwON = e.Switches;
            UpdateSwitches();
        }

        private void btAuto_Click(object sender, EventArgs e)
        {
            if (automode)
            {
                mf.SwitchBox.PressSwitch(SwIDs.Auto, true);
            }
            else
            {
                if (masterOn) { mf.SwitchBox.PressSwitch(SwIDs.MasterOff,true); }
                else { mf.SwitchBox.PressSwitch(SwIDs.MasterOn,true); }
            }

        }

        private void UpdateSwitches()
        {
            if (automode)
            {
                // show auto button
                if (SwON[0])
                {
                    btAuto.BackColor = Color.LightGreen;
                    btAuto.Text = "AUTO";
                    btAuto.ForeColor = Color.Black;
                }
                else
                {
                    btAuto.BackColor = Color.Red;
                    btAuto.Text = "OFF";
                    btAuto.ForeColor = Color.White;
                }
            }
            else
            {
                // show master button
                if(mf.SwitchBox.MasterOn)
                {
                    btAuto.BackColor = Color.Yellow;
                    btAuto.Text = "ON";
                    btAuto.ForeColor = Color.Black;
                    automode = false;
                    masterOn = true;
                }
                else
                {
                    btAuto.BackColor = Color.Red;
                    btAuto.Text = "OFF";
                    btAuto.ForeColor = Color.White;
                    masterOn = false;
                }
            }

            if (SwON[3])
            {
                btnUp.BackColor = Color.Blue;
            }
            else
            {
                btnUp.BackColor = Properties.Settings.Default.DayColour;
            }

            if (SwON[4])
            {
                btnDown.BackColor = Color.Blue;
            }
            else
            {
                btnDown.BackColor = Properties.Settings.Default.DayColour;
            }
        }

        private void btMinimize_Click(object sender, EventArgs e)
        {
            Form restoreform = new RCRestore(this);
            restoreform.Show();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (SwON[0])
            {
                Prd.RateSet = Prd.RateSet * 1.05;
            }
            else
            {
                Prd.ManualPWM += 5;
            }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (SwON[0])
            {
                Prd.RateSet = Prd.RateSet / 1.05;
            }
            else
            {
                Prd.ManualPWM -= 5;
            }
        }
    }
}