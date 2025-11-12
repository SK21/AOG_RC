using AgOpenGPS;
using RateController.Classes;
using RateController.Language;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuMonitoring : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private FormStart mf;
        private Label[] Sec;

        public frmMenuMonitoring(FormStart main, frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            mf = main;
            this.Tag = false;
            Sec = new Label[] { sec0, sec1, sec2, sec3, sec4, sec5, sec6, sec7, sec8, sec9, sec10, sec11, sec12, sec13, sec14, sec15 };
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            MainMenu.ChangeProduct(MainMenu.CurrentProduct.ID - 1, true);
            UpdateForm();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (int.TryParse(tbCountsRev.Text, out int cr)) MainMenu.CurrentProduct.CountsRev = cr;
                MainMenu.CurrentProduct.Save();
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
            MainMenu.ChangeProduct(MainMenu.CurrentProduct.ID + 1, true);
            UpdateForm();
        }

        private void frmMenuMonitoring_Activated(object sender, EventArgs e)
        {
            switch (this.Text)
            {
                case "Focused":
                    this.Text = "";
                    UpdateForm();
                    break;
            }
        }

        private void frmMenuMonitoring_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
            timer1.Enabled = false;
        }

        private void frmMenuMonitoring_Load(object sender, EventArgs e)
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

            tbCountsRev.Enabled = Enabled;
        }

        private void SetLanguage()
        {
            lbUPMApplied.Text = Lang.lgUPMApplied;
            lbTarget.Text = Lang.lgUPMTarget;
            lbError.Text = Lang.lgUPMerror;
            lbCounts.Text = Lang.lgCountsRev;
            lbSpeed.Text = Lang.lgSpeed;
            lbWidth.Text = Lang.lgWorkingWidthFT;
            lbWorkRate.Text = Lang.lgAcresHr;
            lbWifi.Text = Lang.lgWifi;
            lbSections.Text = Lang.lgSections;
        }

        private void tbCountsRev_Enter(object sender, EventArgs e)
        {
            int tempInt;
            int.TryParse(tbCountsRev.Text, out tempInt);
            using (var form = new FormNumeric(0, 10000, tempInt))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbCountsRev.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void tbCountsRev_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbCountsRev_Validating(object sender, CancelEventArgs e)
        {
            int Tmp;
            int.TryParse(tbCountsRev.Text, out Tmp);
            if (Tmp < 0 || Tmp > 10000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            double Target = MainMenu.CurrentProduct.TargetUPM();
            double Applied = MainMenu.CurrentProduct.UPMapplied();
            double RateError = 0;

            if (Target > 0)
            {
                RateError = ((Applied - Target) / Target) * 100;
                bool IsNegative = RateError < 0;
                RateError = Math.Abs(RateError);
                if (RateError > 100) RateError = 100;
                if (IsNegative) RateError *= -1;
            }

            lbErrorPercent.Text = RateError.ToString("N1");
            lbRateAppliedData.Text = Applied.ToString("N1");
            lbRateSetData.Text = Target.ToString("N1");
            lbPWMdata.Text = MainMenu.CurrentProduct.PWM().ToString("N0");
            lbHzData.Text = MainMenu.CurrentProduct.Hz().ToString("N1");

            if (MainMenu.CurrentProduct.CountsRev > 0)
            {
                float RPM = (float)((MainMenu.CurrentProduct.MeterCal * Applied) / MainMenu.CurrentProduct.CountsRev);
                lbRPM.Text = RPM.ToString("N0");
            }
            else
            {
                lbRPM.Text = "0";
            }

            if (Props.UseMetric)
            {
                lbSpeed.Text = Lang.lgKPH;
                lbSpeedData.Text = Props.Speed_KMH.ToString("N1");
            }
            else
            {
                lbSpeed.Text = Lang.lgMPH;
                double speed = Props.Speed_KMH / Props.MPHtoKPH;
                lbSpeedData.Text = speed.ToString("N1");
            }

                lbWidthData.Text = mf.Sections.WorkingWidth(!Props.UseMetric).ToString("N1");
            if (!Props.UseMetric)
            {
                lbWidth.Text = Lang.lgWorkingWidthFT;
            }
            else
            {
                lbWidth.Text = Lang.lgWorkingWidthM;
            }

            lbWorkRateData.Text = MainMenu.CurrentProduct.WorkRate().ToString("N1");
            if (!Props.UseMetric)
            {
                lbWorkRate.Text = Lang.lgAcresHr;
            }
            else
            {
                lbWorkRate.Text = Lang.lgHectares_Hr;
            }

            wifiBar.Value = mf.ModulesStatus.WifiStrength(MainMenu.CurrentProduct.ModuleID);

            // update sections
            for (int i = 0; i < 16; i++)
            {
                Sec[i].Enabled = (mf.Sections.Item(i).Enabled);
                if (mf.Sections.Item(i).IsON)
                //if (mf.Sections.IsSectionOn(i))
                {
                    Sec[i].Image = Properties.Resources.OnSmall;
                }
                else
                {
                    Sec[i].Image = Properties.Resources.OffSmall;
                }
            }
        }

        private void UpdateForm()
        {
            Initializing = true;
            if (MainMenu.CurrentProduct.ID > Props.MaxProducts - 3 && MainMenu.LastScreen == this.Text)
            {
                // fans, move to product only
                MainMenu.ChangeProduct(MainMenu.CurrentProduct.ID - 1, true);
            }
            else
            {
                lbProduct.Text = (MainMenu.CurrentProduct.ID + 1).ToString() + ". " + MainMenu.CurrentProduct.ProductName;
            }
            tbCountsRev.Text = (MainMenu.CurrentProduct.CountsRev.ToString("N0"));
            SetEnabled();

            Initializing = false;
        }
    }
}