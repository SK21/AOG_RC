using System;
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
        private bool ShowCoverageRemaining = false;
        private bool ShowQuantityRemaining = true;

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
            this.BackColor = Properties.Settings.Default.DayColour;
        }

        public int CurrentProduct()
        {
            return Prd.ID;
        }

        private void bntOK_Click(object sender, EventArgs e)
        {
            this.Close();
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
            timerMain.Enabled = false;

            if (this.WindowState == FormWindowState.Normal)
            {
                mf.Tls.SaveFormData(this);
            }

            if (mf.UseLargeScreen) mf.LargeScreenExit = true;
            mf.WindowState = FormWindowState.Normal;
        }

        private void frmLargeScreen_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            UpdateForm();
            timerMain.Enabled = true;
            this.Text = "RC [" + Path.GetFileNameWithoutExtension(Properties.Settings.Default.FileName) + "]";
        }

        private void lbAogConnected_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
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
            Form frmAbout = new FormAbout(mf);
            frmAbout.ShowDialog();
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
            mf.LargeScreenRestart = true;
            this.Close();
        }

        private void MnuEnglish_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.setF_culture = "en";
            Properties.Settings.Default.Save();
            mf.LargeScreenRestart = true;
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
            mf.LargeScreenRestart = true;
            this.Close();
        }

        private void mnuNetwork_Click(object sender, EventArgs e)
        {
            Form frmWifi = new frmWifi(mf);
            frmWifi.ShowDialog();
        }

        private void MnuNew_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = mf.Tls.SettingsDir();
            saveFileDialog1.Title = "New File";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                    mf.Tls.NewFile(saveFileDialog1.FileName);
                    mf.LoadSettings();
                    UpdateForm();
                }
            }
        }

        private void MnuOpen_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = mf.Tls.SettingsDir();
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
            saveFileDialog1.InitialDirectory = mf.Tls.SettingsDir();
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
            this.Close();
        }

        private void russianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.setF_culture = "ru";
            Properties.Settings.Default.Save();
            mf.LargeScreenRestart = true;
            this.Close();
        }

        private void serialMonitorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form Monitor = new frmMonitor(mf);
            Monitor.ShowDialog();
        }

        private void simulationToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void timerMain_Tick(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void UpdateForm()
        {
            // set highlight
            if (mf.SimMode == SimType.VirtualNano)
            {
                panel0.BackColor = mf.SimColor;
                panel1.BackColor = mf.SimColor;
                panel2.BackColor = mf.SimColor;
                panel3.BackColor = mf.SimColor;
            }
            else
            {
                panel0.BackColor = Properties.Settings.Default.DayColour;
                panel1.BackColor = Properties.Settings.Default.DayColour;
                panel2.BackColor = Properties.Settings.Default.DayColour;
                panel3.BackColor = Properties.Settings.Default.DayColour;
            }
            switch (Prd.ID)
            {
                case 0:
                    panel0.BackColor = SystemColors.Highlight;
                    break;

                case 1:
                    panel1.BackColor = SystemColors.Highlight;
                    break;

                case 2:
                    panel2.BackColor = SystemColors.Highlight;
                    break;

                case 3:
                    panel3.BackColor = SystemColors.Highlight;
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
                MnuOptions.DropDownItems["mnuMetric"].Image = Properties.Resources.Xmark;
            }
            else
            {
                MnuOptions.DropDownItems["mnuMetric"].Image = Properties.Resources.CheckMark;
            }

            // aog
            if (mf.AutoSteerPGN.Connected())
            {
                lbAogConnected.BackColor = Color.LightGreen;
            }
            else
            {
                lbAogConnected.BackColor = Color.Red;
            }

            // bins
            clsProduct PD = mf.Products.Item(0);
            double Size = PD.TankSize;
            double Rem = PD.TankStart - PD.UnitsApplied();
            if (Size == 0 || Size < Rem) Size = Rem * 2;
            if (Size == 0) Size = 100;
            int Level = (int)(Rem / Size * 100);
            if (Level > 100) Level = 100;
            if (Level < 0) Level = 0;
            verticalProgressBar0.Value = Level;

            PD = mf.Products.Item(1);
            Size = PD.TankSize;
            Rem = PD.TankStart - PD.UnitsApplied();
            if (Size == 0 || Size < Rem) Size = Rem * 2;
            if (Size == 0) Size = 100;
            Level = (int)(Rem / Size * 100);
            if (Level > 100) Level = 100;
            if (Level < 0) Level = 0;
            verticalProgressBar1.Value = Level;

            PD = mf.Products.Item(2);
            Size = PD.TankSize;
            Rem = PD.TankStart - PD.UnitsApplied();
            if (Size == 0 || Size < Rem) Size = Rem * 2;
            if (Size == 0) Size = 100;
            Level = (int)(Rem / Size * 100);
            if (Level > 100) Level = 100;
            if (Level < 0) Level = 0;
            verticalProgressBar2.Value = Level;

            PD = mf.Products.Item(3);
            Size = PD.TankSize;
            Rem = PD.TankStart - PD.UnitsApplied();
            if (Size == 0 || Size < Rem) Size = Rem * 2;
            if (Size == 0) Size = 100;
            Level = (int)(Rem / Size * 100);
            if (Level > 100) Level = 100;
            if (Level < 0) Level = 0;
            verticalProgressBar3.Value = Level;

            // fans
            if (mf.SimMode == SimType.None)
            {
                // fan 1
                clsProduct prd = mf.Products.Item(mf.MaxProducts - 2);
                lbRPM1.Text = prd.CurrentRate().ToString("N0") + " RPM";
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
                lbRPM2.Text = prd.CurrentRate().ToString("N0") + " RPM";
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