using RateController.Classes;
using RateController.Language;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.Forms
{
    public partial class frmMain : Form
    {
        #region Images

        private Image[] ImagesError = new Image[]
        {
            Properties.Resources.Error1,
            Properties.Resources.Error2,
            Properties.Resources.Error3,
            Properties.Resources.Error4,
            Properties.Resources.Error5,
            Properties.Resources.ErrorFans
          };

        private Image[] ImagesErrorLight = new Image[]
         {
            Properties.Resources.ErrorLightP1,
            Properties.Resources.ErrorLightP2,
            Properties.Resources.ErrorLightP3,
            Properties.Resources.ErrorLightP4,
            Properties.Resources.ErrorLightP5,
            Properties.Resources.ErrorLightFans
         };

        private Image[] ImagesOff = new Image[]
                        {
            Properties.Resources.Offp1,
            Properties.Resources.Offp2,
            Properties.Resources.Offp3,
            Properties.Resources.Offp4,
            Properties.Resources.Offp5,
            Properties.Resources.OffFans
        };

        private Image[] ImagesOffLight = new Image[]
        {
            Properties.Resources.Off1Light,
            Properties.Resources.Off2Light,
            Properties.Resources.Off3Light,
            Properties.Resources.Off4Light,
            Properties.Resources.Off5Light,
            Properties.Resources.OffFansLight
        };

        private Image[] ImagesOn = new Image[]
        {
            Properties.Resources.OnP1,
            Properties.Resources.OnP2,
            Properties.Resources.OnP3,
            Properties.Resources.OnP4,
            Properties.Resources.OnP5,
            Properties.Resources.OnFans
        };

        private Image[] ImagesOnLight = new Image[]
        {
            Properties.Resources.OnLightP1,
            Properties.Resources.OnLightP2,
            Properties.Resources.OnLightP3,
            Properties.Resources.OnLightP4,
            Properties.Resources.OnLightP5,
            Properties.Resources.OnLightFans
        };

        private Image[] ImagesRCV = new Image[]
        {
            Properties.Resources.RCV1,
            Properties.Resources.RCV2,
            Properties.Resources.RCV3,
            Properties.Resources.RCV4,
            Properties.Resources.RCV5,
            Properties.Resources.RCVfans
        };

        private Image[] ImagesRCVlight = new Image[]
        {
            Properties.Resources.RCVlightP1,
            Properties.Resources.RCVlightP2,
            Properties.Resources.RCVlightP3,
            Properties.Resources.RCVlightP4,
            Properties.Resources.RCVlightP5,
            Properties.Resources.RCVlightFans
        };

        #endregion Images

        private int AlarmButtonCountDown = 5;
        private bool[] Alarms;
        private bool FlashState;
        private int LastAlarm;
        private Point MouseDownLocation;
        private PictureBox[] ProductIcons = new PictureBox[6];
        private bool ShowFans = false;

        public frmMain()
        {
            InitializeComponent();
        }

        public event EventHandler Minimize;

        public void SendRelays()
        {
            for (int i = 0; i < Props.MaxModules; i++)
            {
                if (Core.ModulesStatus.Connected(i)) Core.RelaySettings[i].Send();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Define the border color and thickness
            Color borderColor = Properties.Settings.Default.DisplayForeColour;
            int borderWidth = 2;

            // Draw the border
            using (Pen pen = new Pen(borderColor, borderWidth))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1);
            }
        }

        private void btAlarm_Click(object sender, EventArgs e)
        {
            Core.RCalarm.Silence();
        }

        private void btnAuto_Click(object sender, EventArgs e)
        {
            Core.vSwitchBox.PressSwitch(SwIDs.AutoRate, true);
            if (Core.vSwitchBox.AutoRateOn != Core.vSwitchBox.AutoSectionOn) Core.vSwitchBox.PressSwitch(SwIDs.AutoSection, true);
            UpdateSwitches();
        }

        private void btnMaster_Click(object sender, EventArgs e)
        {
            Core.vSwitchBox.MasterPressed(true);
        }

        private void btnMaster_MouseUp(object sender, MouseEventArgs e)
        {
            Core.vSwitchBox.MasterReleased();
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            Form fs = Props.IsFormOpen("frmMenu");
            if (fs == null)
            {
                fs = new frmMenu();
                fs.Show();
            }
            fs.Focus();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            Minimize?.Invoke(this, EventArgs.Empty);
            Form restoreform = new RCRestore();
            restoreform.Show();
        }

        private void btnVR_Click(object sender, EventArgs e)
        {
            if (Props.VariableRateEnabled)
            {
                Props.VariableRateEnabled = false;
            }
            else
            {
                Props.VariableRateEnabled = true;
            }
            UpdateForm();
        }

        private void butPowerOff_Click(object sender, EventArgs e)
        {
            Core.RequestUserExit();
        }

        private void CheckAlarms()
        {
            if (Core.RCalarm.AlarmIsOn(out Alarms))
            {
                if (!btAlarm.Visible) AlarmButtonCountDown--;
                btAlarm.Visible = (AlarmButtonCountDown < 1);
                if (btAlarm.Visible)
                {
                    btAlarm.BringToFront();
                    FlashState = !FlashState;

                    for (int i = 0; i < Props.MaxProducts; i++)
                    {
                        if (Alarms[i])
                        {
                            if (LastAlarm != i)
                            {
                                // switch to first alarming product
                                LastAlarm = i;
                                Props.CurrentProduct = i;
                                SwitchDisplay(false);
                                UpdateForm();
                            }
                            if (FlashState)
                            {
                                btAlarm.Text = Core.Tls.ClipText((i + 1).ToString() + ". " + Core.Products.Item(i).ProductName, 20);
                            }
                            else
                            {
                                btAlarm.Text = "Rate Alarm";
                            }
                            break;
                        }
                    }
                }
            }
            else
            {
                btAlarm.Visible = false;
                LastAlarm = -1;
                AlarmButtonCountDown = 5;
            }
        }

        private void Core_ColorChanged(object sender, EventArgs e)
        {
            SetColors();
            UpdateSwitches();
            UpdateForm();
            this.Invalidate(); // schedules a repaint
        }

        private void Core_UpdateStatus(object sender, EventArgs e)
        {
            UpdateForm();
            SendRelays();
            CheckAlarms();
        }

        private void Fans_Click(object sender, EventArgs e)
        {
            SwitchDisplay(true);
            UpdateForm();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Core.AppShutDown(e))
            {
                Core.UpdateStatus -= Core_UpdateStatus;
                Core.ColorChanged -= Core_ColorChanged;
                Core.SwitchBox.SwitchPGNreceived -= SwitchBox_SwitchPGNreceived;
                Props.SaveFormLocation(this);
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void frmMainDisplay_Load(object sender, EventArgs e)
        {
            Core.Initialize(this);

            Core.SwitchBox.SwitchPGNreceived += SwitchBox_SwitchPGNreceived;
            Core.UpdateStatus += Core_UpdateStatus;
            Core.ColorChanged += Core_ColorChanged;

            ProductIcons[0] = prod1;
            ProductIcons[1] = prod2;
            ProductIcons[2] = prod3;
            ProductIcons[3] = prod4;
            ProductIcons[4] = prod5;
            ProductIcons[5] = Fans;

            Alarms = new bool[Props.MaxProducts];

            Props.LoadFormLocation(this);
            SetColors();

            SwitchDisplay(false);
            UpdateSwitches();
            UpdateForm();
        }

        private void lbCoverage_Click(object sender, EventArgs e)
        {
            Props.ShowCoverageRemaining = !Props.ShowCoverageRemaining;
            UpdateForm();
        }

        private void lbCoverageAmount_Click(object sender, EventArgs e)
        {
            var Hlp = new frmMsgBox("Reset area?", "Reset", true);
            Hlp.TopMost = true;

            Hlp.ShowDialog();
            bool Result = Hlp.Result;
            Hlp.Close();
            if (Result)
            {
                clsProduct Prd = Core.Products.Item(Props.CurrentProduct);
                Prd.ResetCoverage();
                UpdateForm();
            }
        }

        private void lbQuantity_Click(object sender, EventArgs e)
        {
            Props.ShowQuantityRemaining = !Props.ShowQuantityRemaining;
            UpdateForm();
        }

        private void lbQuantityAmount_Click(object sender, EventArgs e)
        {
            Form fs = Props.IsFormOpen("frmResetQuantity");

            if (fs != null)
            {
                fs.Focus();
                return;
            }

            Form frm = new frmResetQuantity();
            frm.Show();
            UpdateForm();
        }

        private void lbTarget_Click(object sender, EventArgs e)
        {
            if (!Props.VariableRateEnabled)
            {
                clsProduct Prd = Core.Products.Item(Props.CurrentProduct);
                if (Prd.UseAltRate)
                {
                    lbTarget.Text = Core.Tls.ClipText(Lang.lgTargetRate, 14);
                    Prd.UseAltRate = false;
                }
                else
                {
                    lbTarget.Text = Core.Tls.ClipText(Lang.lgTargetRateAlt, 14);
                    Prd.UseAltRate = true;
                }
            }
            UpdateForm();
        }

        private void mouseMove_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Left) MouseDownLocation = e.Location;
        }

        private void mouseMove_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Left) this.Location = new Point(this.Left + e.X - MouseDownLocation.X, this.Top + e.Y - MouseDownLocation.Y);
        }

        private void pictureBox3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) MouseDownLocation = e.Location;
        }

        private void pictureBox3_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) this.Location = new Point(this.Left + e.X - MouseDownLocation.X, this.Top + e.Y - MouseDownLocation.Y);
        }

        private void prod1_Click(object sender, EventArgs e)
        {
            Props.CurrentProduct = 0;
            SwitchDisplay(false);
            UpdateForm();
        }

        private void prod2_Click(object sender, EventArgs e)
        {
            Props.CurrentProduct = 1;
            SwitchDisplay(false);
            UpdateForm();
        }

        private void prod3_Click(object sender, EventArgs e)
        {
            Props.CurrentProduct = 2;
            SwitchDisplay(false);
            UpdateForm();
        }

        private void prod4_Click(object sender, EventArgs e)
        {
            Props.CurrentProduct = 3;
            SwitchDisplay(false);
            UpdateForm();
        }

        private void Prod5_Click(object sender, EventArgs e)
        {
            Props.CurrentProduct = 4;
            SwitchDisplay(false);
            UpdateForm();
        }

        private void SetColors()
        {
            this.BackColor = Properties.Settings.Default.DisplayBackColour;
            foreach (Control c in this.Controls)
            {
                c.ForeColor = Properties.Settings.Default.DisplayForeColour;
            }
        }

        private void SwitchBox_SwitchPGNreceived(object sender, EventArgs e)
        {
            UpdateSwitches();
        }

        private void SwitchDisplay(bool DisplayFans)
        {
            if (ShowFans != DisplayFans)
            {
                ShowFans = DisplayFans;

                lbQuantity.Visible = !ShowFans;
                lbCoverage.Visible = !ShowFans;
                lbCoverageAmount.Visible = !ShowFans;
                lbCoverageType.Visible = !ShowFans;
                lbQuantityAmount.Visible = !ShowFans;
                lbQuantityType.Visible = !ShowFans;
                lbProductName.Visible = !ShowFans;
                pnlQuantity.Visible = !ShowFans;

                if (ShowFans)
                {
                    lbUnits.Text = "Fan 1";
                    lbTarget.Text = "Fan 2";
                    lbRateType.Text = "RPM";
                    lbTargetType.Text = "RPM";
                }
                else
                {
                    lbUnits.Text = Core.Tls.ClipText(Lang.lgCurrentRate, 14);
                }
            }
        }

        private void UpdateFans()
        {
            lbRateAmount.Text = Core.Products.Item(5).CurrentRate().ToString("N0");
            lbTargetAmount.Text = Core.Products.Item(6).CurrentRate().ToString("N0");
        }

        private void UpdateForm()
        {
            if (ShowFans)
            {
                UpdateFans();
            }
            else
            {
                UpdateProducts();
            }

            UpdateStatusIcons();
        }

        private void UpdateProducts()
        {
            try
            {
                bool UseLight = Core.Tls.UseLightContrast();

                clsProduct Prd = Core.Products.Item(Props.CurrentProduct);

                lbProductName.Text = Core.Tls.ClipText((Prd.ID + 1).ToString() + ". " + Prd.ProductName, 20);

                // current rate
                lbRateType.Text = Core.Tls.ClipText(Prd.Units(), 8);
                double rt = Prd.CurrentRate();
                if (rt > 999)
                {
                    lbRateAmount.Text = rt.ToString("N0");
                }
                else
                {
                    lbRateAmount.Text = rt.ToString("N1");
                }

                // target rate
                lbTargetType.Text = Core.Tls.ClipText(Prd.Units(), 8);
                rt = Prd.TargetRate();
                if (rt > 999)
                {
                    lbTargetAmount.Text = rt.ToString("N0");
                }
                else
                {
                    lbTargetAmount.Text = rt.ToString("N1");
                }

                // quantity
                lbQuantityType.Text = Core.Tls.ClipText(Prd.QuantityDescription, 8);
                double Tnk = 0;
                if (Props.ShowQuantityRemaining)
                {
                    // calculate remaining
                    lbQuantity.Text = Core.Tls.ClipText(Lang.lgTank_Remaining, 14);
                    Tnk = Prd.TankStart - Prd.UnitsApplied();
                }
                else
                {
                    // show amount done
                    lbQuantity.Text = Core.Tls.ClipText(Lang.lgQuantityApplied, 14);
                    Tnk = Prd.UnitsApplied();
                }
                if (Math.Abs(Tnk) > 9999)
                {
                    lbQuantityAmount.Text = Tnk.ToString("N0");
                }
                else
                {
                    lbQuantityAmount.Text = Tnk.ToString("N1");
                }

                // area
                lbCoverageType.Text = Core.Tls.ClipText(Prd.CoverageDescription(), 8);
                if (Props.ShowCoverageRemaining)
                {
                    lbCoverage.Text = Core.Tls.ClipText(Lang.lgAreaRemain, 14);
                    double RT = Prd.SmoothRate();
                    if (RT < 0.01) RT = Prd.TargetRate();

                    if ((RT > 0) && (Prd.TankStart > 0))
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
                    lbCoverage.Text = Core.Tls.ClipText(Lang.lgAreaApplied, 14);
                }

                if (Props.VariableRateEnabled)
                {
                    lbTarget.Text = "VR Target";
                }
                else if (Prd.UseAltRate)
                {
                    lbTarget.Text = Core.Tls.ClipText(Lang.lgTargetRateAlt, 14);
                }
                else
                {
                    lbTarget.Text = Core.Tls.ClipText(Lang.lgTargetRate, 14);
                }

                // graph
                double Rem = Prd.TankStart - Prd.UnitsApplied();
                double Size = Prd.TankSize;
                if (Size < 0.01 || Size < Rem) Size = Rem * 2;
                if (Size < 0.01) Size = 100;
                int Level = (int)(Rem / Size * 100);
                if (Level > 100) Level = 100;
                if (Level < 0) Level = 0;
                pbQuantity.Value = Level;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMain/UpdateProducts: " + ex.Message);
            }
        }

        private void UpdateStatusIcons()
        {
            bool UseLight = Core.Tls.UseLightContrast();

            for (int i = 0; i < Props.MaxProducts - 1; i++)
            {
                if (Alarms[i])
                {
                    if (FlashState)
                    {
                        ProductIcons[i].Image = UseLight ? ImagesErrorLight[i] : ImagesError[i];
                    }
                    else
                    {
                        clsProduct pd = Core.Products.Item(i);
                        if (pd.RateSensorData.ModuleSending())
                        {
                            if (pd.RateSensorData.ModuleReceiving())
                            {
                                ProductIcons[i].Image = UseLight ? ImagesOnLight[i] : ImagesOn[i];
                            }
                            else
                            {
                                ProductIcons[i].Image = UseLight ? ImagesRCVlight[i] : ImagesRCV[i];
                            }
                        }
                        else
                        {
                            ProductIcons[i].Image = UseLight ? ImagesOffLight[i] : ImagesOff[i];
                        }
                    }
                }
                else
                {
                    clsProduct pd = Core.Products.Item(i);
                    if (pd.RateSensorData.ModuleSending())
                    {
                        if (pd.RateSensorData.ModuleReceiving())
                        {
                            ProductIcons[i].Image = UseLight ? ImagesOnLight[i] : ImagesOn[i];
                        }
                        else
                        {
                            ProductIcons[i].Image = UseLight ? ImagesRCVlight[i] : ImagesRCV[i];
                        }
                    }
                    else
                    {
                        ProductIcons[i].Image = UseLight ? ImagesOffLight[i] : ImagesOff[i];
                    }
                }
            }

            if (Props.VariableRateEnabled)
            {
                btnVR.Image = UseLight ? Properties.Resources.VRonLight : Properties.Resources.VRon;
            }
            else
            {
                btnVR.Image = UseLight ? Properties.Resources.VRoffLight : Properties.Resources.VRoff;
            }

            btnMenu.Image = UseLight ? Properties.Resources.MenuLight : Properties.Resources.article;
            btnMinimize.Image = UseLight ? Properties.Resources.MinimizeLight : Properties.Resources.arrow_circle_down_right;
            butPowerOff.Image = UseLight ? Properties.Resources.SwitchOffLight : Properties.Resources.SwitchOff;

            if (Core.GPS.TWOLconnected() || Core.AutoSteerPGN.Connected())
            {
                pbAOGstatus.Image = UseLight ? Properties.Resources.AOG_On_Light : Properties.Resources.AOG_On;
            }
            else
            {
                pbAOGstatus.Image = UseLight ? Properties.Resources.AOG_Off_Light : Properties.Resources.AOG_Off;
            }
        }

        private void UpdateSwitches()
        {
            // auto button
            if (Core.SwitchBox.AutoRateOn || Core.SwitchBox.AutoSectionOn)
            {
                btnAuto.Image = Core.Tls.UseLightContrast() ? Properties.Resources.AutoOnLight : Properties.Resources.AutoOn;
            }
            else
            {
                btnAuto.Image = Core.Tls.UseLightContrast() ? Properties.Resources.AutoOffLight : Properties.Resources.AutoOff;
            }

            // master button
            if (Core.SwitchBox.MasterOn)
            {
                btnMaster.Image = Properties.Resources.SprayOn;
            }
            else
            {
                btnMaster.Image = Properties.Resources.SprayOff;
            }
        }
    }
}