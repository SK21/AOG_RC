using RateController.Classes;
using RateController.Language;
using RateController.Menu;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RateController.Forms
{
    public partial class frmMain : Form
    {
        public RCRestore MainMini;
        private int AlarmButtonCountDown = 5;

        private bool FlashState;

        private int LastAlarm;
        private AlarmKind LastAlarmKind = AlarmKind.None;

        private bool LastAogConnected;

        private ProductState[] LastState = new ProductState[Props.MaxProducts];

        private bool LastUseLight;

        private bool LastVRenabled;

        private Point MouseDownLocation;

        private PictureBox[] ProductIcons = new PictureBox[6];

        private bool ShowFans;

        // the collapsed display keeps only the product name, the rates, quantity,
        // area and the bar - the remaining controls are shifted to the top left
        // corner of the small window, that is the right hand side of the normal
        // layout

        private const int CompactShiftX = 148;

        private const int CompactShiftY = 66;

        private const int CompactHeight = 219;

        // gap either side of the product name and the alarm button, which are cut
        // short to the space left of the bar in the collapsed display
        private const int CompactTextMargin = 3;

        private bool CompactMode;

        private Rectangle FullNameBounds;

        private Rectangle FullAlarmBounds;

        private Control[] CompactMoved;

        private Size FullClientSize;

        private Point CompactTapStart;

        public frmMain()
        {
            InitializeComponent();
        }

        public event EventHandler Minimize;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Define the border color and thickness
            Color borderColor = Properties.Settings.Default.DisplayForeColour;
            // a wider pen is centred on the edge pixels, so half of it is cut off on the top
            // and left but not on the right and bottom - 1 keeps all four sides the same
            int borderWidth = 1;

            // Draw the border
            using (Pen pen = new Pen(borderColor, borderWidth))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1);
            }
        }

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        private static bool IsTabletPC() => GetSystemMetrics(86) != 0; // SM_TABLETPC

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

        private void ApplyVisibility()
        {
            // quantity and area are not used with the fans
            bool Products = !ShowFans;

            lbProductName.Visible = Products;
            lbQuantityAmount.Visible = Products;
            lbQuantityType.Visible = Products;
            lbCoverageAmount.Visible = Products;
            lbCoverageType.Visible = Products;
            barQuantity.Visible = Products;
            btnResetQuantity.Visible = Products;
            btnResetAcres.Visible = Products;

            // the rest of the layout is hidden while the display is collapsed
            bool Full = !CompactMode;

            // the buttons that select the quantity and the area belong to the
            // full products layout
            btnQuantity.Visible = Products && Full;
            btnCoverage.Visible = Products && Full;
            prod1.Visible = Full;
            prod2.Visible = Full;
            prod3.Visible = Full;
            prod4.Visible = Full;
            prod5.Visible = Full;
            Fans.Visible = Full;
            pbAOGstatus.Visible = Full;
            btnUnits.Visible = Full;
            btnTarget.Visible = Full;
            btnMaster.Visible = Full;
            btnAuto.Visible = Full;
            btnMinimize.Visible = Full;
            btnMenu.Visible = Full;
            btnVR.Visible = Full;
            butPowerOff.Visible = Full;
            UL1.Visible = Full;

            // the alarm button is placed and shown by the flash timer
            if (CompactMode) btAlarm.Visible = false;
        }

        private void CompactTap_MouseDown(object sender, MouseEventArgs e)
        {
            CompactTapStart = Location;
        }

        private void CompactTap_MouseUp(object sender, MouseEventArgs e)
        {
            // dragging moves the small display, only a tap brings the whole display back
            if (CompactMode && Math.Abs(Left - CompactTapStart.X) < 5 && Math.Abs(Top - CompactTapStart.Y) < 5)
            {
                SetCompactDisplay(false);
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

        private void btnCoverage_Click(object sender, EventArgs e)
        {
            Props.ShowCoverageRemaining = !Props.ShowCoverageRemaining;
            UpdateForm();
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
            if (Props.CompactDisplay)
            {
                SetCompactDisplay(!CompactMode);
            }
            else
            {
                Minimize?.Invoke(this, EventArgs.Empty);
                Core.SetMainDisplay(false);
            }
        }

        private void btnQuantity_Click(object sender, EventArgs e)
        {
            Props.ShowQuantityRemaining = !Props.ShowQuantityRemaining;
            UpdateForm();
        }

        private void btnResetAcres_Click(object sender, EventArgs e)
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

        private void btnTarget_Click(object sender, EventArgs e)
        {
            if (ShowFans)
            {
                Props.CurrentMenuName = "frmMenuRate";

                frmMenu fs = (frmMenu)Props.IsFormOpen("frmMenu");
                if (fs == null)
                {
                    fs = new frmMenu();
                    fs.Show();
                }
                fs.ChangeProduct(6);
                fs.Focus();
            }
            else
            {
                if (!Props.VariableRateEnabled)
                {
                    clsProduct Prd = Core.Products.Item(Props.CurrentProduct);
                    if (Prd.UseAltRate)
                    {
                        btnTarget.Text = Core.Tls.ClipText(Lang.lgTargetRate, 14);
                        Prd.UseAltRate = false;
                    }
                    else
                    {
                        btnTarget.Text = Core.Tls.ClipText(Lang.lgTargetRateAlt, 14);
                        Prd.UseAltRate = true;
                    }
                }
                UpdateForm();
            }
        }

        private void btnUnits_Click(object sender, EventArgs e)
        {
            Props.CurrentMenuName = "frmMenuRate";

            frmMenu fs = (frmMenu)Props.IsFormOpen("frmMenu");
            if (fs == null)
            {
                fs = new frmMenu();
                fs.Show();
            }
            if (ShowFans) fs.ChangeProduct(5);
            fs.Focus();
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
        }

        private void Fans_Click(object sender, EventArgs e)
        {
            SwitchDisplay(true);
            UpdateForm();
        }

        private void UnderlineProduct()
        {
            int offset = 16;
            if (ShowFans)
            {
                UL1.Left = Fans.Left + offset;
            }
            else
            {
                switch (Props.CurrentProduct)
                {
                    case 1:
                        UL1.Left = prod2.Left + offset;
                        break;

                    case 2:
                        UL1.Left = prod3.Left + offset;
                        break;

                    case 3:
                        UL1.Left = prod4.Left + offset;
                        break;

                    case 4:
                        UL1.Left = prod5.Left + offset;
                        break;

                    default:
                        UL1.Left = prod1.Left + offset;
                        break;
                }
            }
        }

        private void FlashTimer_Tick(object sender, EventArgs e)
        {
            ShowAlarmButton();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Core.AppShutDown(e))
            {
                Core.UpdateStatus -= Core_UpdateStatus;
                Core.ColorChanged -= Core_ColorChanged;
                Core.SwitchBox.SwitchPGNreceived -= SwitchBox_SwitchPGNreceived;

                // save the position of the full display, not the collapsed one
                if (CompactMode) Location = new Point(Left - CompactShiftX, Top - CompactShiftY);

                Props.SaveFormLocation(this);
                FlashTimer.Stop();
                Props.SetAppProp("StartMinimized", MainMini.Visible.ToString());
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void frmMainDisplay_Load(object sender, EventArgs e)
        {
            MainMini = new RCRestore();
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

            // controls that stay on the screen when the display is collapsed - the product
            // name and the alarm button stay too, but are sized separately in SetCompactDisplay
            CompactMoved = new Control[] { lbRateAmount, lbRateType, lbTargetAmount, lbTargetType,
                                           lbQuantityAmount, lbQuantityType, lbCoverageAmount, lbCoverageType,
                                           btnResetQuantity, btnResetAcres, barQuantity };

            // a tap on any of these brings the whole display back
            Control[] CompactTap = new Control[] { this, lbProductName,
                                                   lbRateAmount, lbRateType, lbTargetAmount, lbTargetType,
                                                   lbQuantityAmount, lbQuantityType, lbCoverageAmount, lbCoverageType };
            foreach (Control c in CompactTap)
            {
                c.MouseDown += CompactTap_MouseDown;
                c.MouseUp += CompactTap_MouseUp;
            }

            // location is restored in Core.Initialize, before the floating display forms open
            SetColors();

            for (int i = 0; i < Props.MaxProducts; i++)
            {
                LastState[i] = ProductState.Off;
            }

            SwitchDisplay(false);
            UpdateSwitches();
            UpdateForm();

            // Touchscreen only: clear focus and hover state after every button tap.
            // Moving the cursor off the button triggers WM_MOUSELEAVE, clearing the hot/hover highlight.
            if (IsTabletPC())
            {
                foreach (Control c in Controls)
                {
                    c.Click += (s, ea) =>
                    {
                        ActiveControl = null;
                        Cursor.Position = PointToScreen(new Point(0, 0));
                    };
                }
            }
            FlashTimer.Start();

            if (JobManager.ShowJobs)
            {
                frmMenu menu = new frmMenu(false);
                menu.Show();
                Form frm = new frmMenuJobs(menu);
                frm.Owner = menu;
                frm.Show();
            }

            bool sm = bool.TryParse(Props.GetAppProp("StartMinimized"), out bool mm) ? mm : false;
            if (sm)
            {
                Minimize?.Invoke(this, EventArgs.Empty);
                Core.SetMainDisplay(false);
            }
        }

        private void mouseMove_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Left) MouseDownLocation = e.Location;
        }

        private void mouseMove_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Left) this.Location = new Point(this.Left + e.X - MouseDownLocation.X, this.Top + e.Y - MouseDownLocation.Y);
        }

        private void pbQuantity_Click(object sender, EventArgs e)
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

        /// <summary>
        /// Makes the display smaller - only the product name, the rates, the quantity,
        /// the area and the bar stay on the screen, everything else is hidden. The
        /// remaining controls keep their place on the screen, the window is shrunk
        /// to the right hand side of the normal layout.
        /// </summary>
        private void SetCompactDisplay(bool Compact)
        {
            if (CompactMode != Compact)
            {
                // the fan display has no quantity, area or bar, so show the products
                if (Compact && ShowFans)
                {
                    SwitchDisplay(false);
                    UpdateForm();
                }

                CompactMode = Compact;

                if (Compact)
                {
                    FullClientSize = ClientSize;

                    foreach (Control c in CompactMoved)
                    {
                        c.Location = new Point(c.Left - CompactShiftX, c.Top - CompactShiftY);
                    }

                    // the product name and the alarm button are wider than the small window,
                    // so they get the space left of the bar and are cut short with an ellipsis
                    FullNameBounds = lbProductName.Bounds;
                    FullAlarmBounds = btAlarm.Bounds;
                    int TextWidth = barQuantity.Left - 2 * CompactTextMargin;
                    lbProductName.Bounds = new Rectangle(CompactTextMargin, FullNameBounds.Top - CompactShiftY, TextWidth, FullNameBounds.Height);
                    btAlarm.Bounds = new Rectangle(CompactTextMargin, FullAlarmBounds.Top - CompactShiftY, TextWidth, FullAlarmBounds.Height);

                    // the small window is the right hand side of the normal layout, so the bar
                    // keeps the same gap to the right edge as in the full display
                    ClientSize = new Size(FullClientSize.Width - CompactShiftX, CompactHeight);

                    // the remaining controls stay where they are on the screen, and so do
                    // the windows latched to the display
                    clsLatch.MoveHostInPlace(this, new Point(Left + CompactShiftX, Top + CompactShiftY));
                }
                else
                {
                    ClientSize = FullClientSize;

                    foreach (Control c in CompactMoved)
                    {
                        c.Location = new Point(c.Left + CompactShiftX, c.Top + CompactShiftY);
                    }

                    lbProductName.Bounds = FullNameBounds;
                    btAlarm.Bounds = FullAlarmBounds;

                    // the small display may have been dragged, so open where it is,
                    // but keep the whole display on the screen
                    Rectangle wa = Screen.FromControl(this).WorkingArea;
                    int newLeft = Math.Max(wa.Left, Math.Min(wa.Right - Width, Left - CompactShiftX));
                    int newTop = Math.Max(wa.Top, Math.Min(wa.Bottom - Height, Top - CompactShiftY));
                    clsLatch.MoveHostInPlace(this, new Point(newLeft, newTop));
                }

                ApplyVisibility();

                // shrinking the window does not repaint it, so the border would be missing
                // along the new right and bottom edges
                Invalidate();
            }
        }

        private void SetColors()
        {
            this.BackColor = Properties.Settings.Default.DisplayBackColour;
            Props.ApplyOpacity(this);
            foreach (Control c in this.Controls)
            {
                if (c == barQuantity) continue;
                c.ForeColor = Properties.Settings.Default.DisplayForeColour;
            }
            barQuantity.BackColor = Properties.Settings.Default.MainBackColour;
            barQuantity.BorderColor = Properties.Settings.Default.DisplayForeColour;
            UL1.BackColor = Properties.Settings.Default.DisplayForeColour;
        }

        private void ShowAlarmButton()
        {
            if (Core.RCalarm.AlarmIsOn)
            {
                if (!btAlarm.Visible) AlarmButtonCountDown--;
                btAlarm.Visible = (AlarmButtonCountDown < 1);
                if (btAlarm.Visible)
                {
                    btAlarm.BringToFront();
                    FlashState = !FlashState;

                    // announce the most recently latched alarm; the product tiles
                    // enumerate any others (ProductState.Error)
                    AlarmKind kind = Core.RCalarm.Announced(out int idx);
                    if (kind != AlarmKind.None)
                    {
                        // a newly announced product alarm switches the display to
                        // that product, once
                        if ((kind == AlarmKind.Rate || kind == AlarmKind.Bin)
                            && (LastAlarmKind != kind || LastAlarm != idx))
                        {
                            LastAlarmKind = kind;
                            LastAlarm = idx;
                            Props.CurrentProduct = idx;
                            SwitchDisplay(false);
                            UpdateForm();
                        }

                        string family;
                        string specific;
                        switch (kind)
                        {
                            case AlarmKind.Pressure:
                                family = "Pressure Alarm";
                                specific = "High Pressure";
                                break;

                            case AlarmKind.Bin:
                                family = "Bin Empty";
                                specific = Core.Tls.ClipText((idx + 1).ToString() + ". " + Core.Products.Item(idx).ProductName, 20);
                                break;

                            default:
                                family = "Rate Alarm";
                                specific = Core.Tls.ClipText((idx + 1).ToString() + ". " + Core.Products.Item(idx).ProductName, 20);
                                break;
                        }

                        int count = Core.RCalarm.ActiveCount;
                        if (count > 1) family += " (" + count.ToString() + ")";

                        btAlarm.Text = FlashState ? family : specific;
                    }
                }
            }
            else
            {
                btAlarm.Visible = false;
                LastAlarm = -1;
                LastAlarmKind = AlarmKind.None;
                AlarmButtonCountDown = 5;
            }
        }

        private void SwitchBox_SwitchPGNreceived(object sender, EventArgs e)
        {
            BeginInvoke((Action)UpdateSwitches);
            //UpdateForm();
        }

        private void SwitchDisplay(bool DisplayFans)
        {
            if (ShowFans != DisplayFans)
            {
                ShowFans = DisplayFans;
                ApplyVisibility();

                if (ShowFans)
                {
                    btnUnits.Text = "Fan 1";
                    btnTarget.Text = "Fan 2";
                    lbRateType.Text = "RPM";
                    lbTargetType.Text = "RPM";
                }
                else
                {
                    btnUnits.Text = Core.Tls.ClipText(Lang.lgCurrentRate, 14);
                }
            }
        }

        private void UpdateFans()
        {
            lbRateAmount.Text = Core.Products.Item(5).CurrentRate().ToString("N0");
            lbTargetAmount.Text = Core.Products.Item(6).CurrentRate().ToString("N0");

            // the products view may have left the rate readout alarm-red
            lbRateAmount.ForeColor = lbTargetAmount.ForeColor;
            btnUnits.ForeColor = btnTarget.ForeColor;
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
            UnderlineProduct();
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

                // caption and value go alarm-red while this product's rate alarm is
                // latched; the target label/button never alarm, so they always carry
                // the themed colour to restore
                if (Core.RCalarm.Alarms[Prd.ID])
                {
                    btnUnits.Text = "Rate Error";
                    btnUnits.ForeColor = Core.Tls.AlarmColor();
                    lbRateAmount.ForeColor = Core.Tls.AlarmColor();
                }
                else
                {
                    btnUnits.Text = Core.Tls.ClipText(Lang.lgCurrentRate, 14);
                    btnUnits.ForeColor = btnTarget.ForeColor;
                    lbRateAmount.ForeColor = lbTargetAmount.ForeColor;
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
                lbQuantityType.Text = Core.Tls.ClipText(Prd.QuantityDescription, 3);
                double Tnk = 0;
                if (Props.ShowQuantityRemaining)
                {
                    // calculate remaining
                    btnQuantity.Text = Core.Tls.ClipText(Lang.lgTank_Remaining, 14);
                    Tnk = Prd.CurrentTankAmount;
                }
                else
                {
                    // show amount done
                    btnQuantity.Text = Core.Tls.ClipText(Lang.lgQuantityApplied, 14);
                    Tnk = Prd.UnitsApplied();
                }

                // bin sensor overrides the quantity caption while empty; caption and
                // value go alarm-red; the coverage button/amount never alarm, so they
                // always carry the themed colour to restore
                if (Core.RCalarm.BinAlarms[Prd.ID])
                {
                    btnQuantity.Text = "Bin Empty";
                    btnQuantity.ForeColor = Core.Tls.AlarmColor();
                    lbQuantityAmount.ForeColor = Core.Tls.AlarmColor();

                    // display only - the product's tank amount is not modified
                    Tnk = 0;
                }
                else
                {
                    btnQuantity.ForeColor = btnCoverage.ForeColor;
                    lbQuantityAmount.ForeColor = lbCoverageAmount.ForeColor;
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
                lbCoverageType.Text = Core.Tls.ClipText(Prd.UnitsAbbr(), 2);
                if (Props.ShowCoverageRemaining)
                {
                    btnCoverage.Text = Core.Tls.ClipText(Lang.lgAreaRemain, 14);
                    double RT = Prd.TargetRate();

                    if (RT > 0)
                    {
                        lbCoverageAmount.Text = (Prd.CurrentTankAmount / RT).ToString("N1");
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
                    btnCoverage.Text = Core.Tls.ClipText(Lang.lgAreaApplied, 14);
                }

                if (Props.VariableRateEnabled)
                {
                    btnTarget.Text = "Rx Target";
                }
                else if (Prd.UseAltRate)
                {
                    btnTarget.Text = Core.Tls.ClipText(Lang.lgTargetRateAlt, 14);
                }
                else
                {
                    btnTarget.Text = Core.Tls.ClipText(Lang.lgTargetRate, 14);
                }

                // graph
                double Rem = Prd.CurrentTankAmount;
                double Size = Prd.TankSize;
                if (Size < 0.01 || Size < Rem) Size = Rem * 2;
                if (Size < 0.01) Size = 100;
                int Level = (int)(Rem / Size * 100);
                if (Level > 100) Level = 100;
                if (Level < 0) Level = 0;
                barQuantity.Value = Level;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMain/UpdateProducts: " + ex.Message);
            }
        }

        private void UpdateStatusIcons()
        {
            bool UseLight = Core.Tls.UseLightContrast();
            bool ThemeChanged = (UseLight != LastUseLight);
            LastUseLight = UseLight;

            // products
            for (int i = 0; i < Props.MaxProducts - 1; i++)
            {
                ProductState CurrentState = Core.Products.ProductsState(i);
                Image img;
                if (CurrentState != LastState[i] || ThemeChanged)
                {
                    switch (CurrentState)
                    {
                        case ProductState.On:
                            img = UseLight ? ImagesOnLight[i] : ImagesOn[i];
                            break;

                        case ProductState.Sending:
                            img = UseLight ? ImagesRCVlight[i] : ImagesRCV[i];
                            break;

                        case ProductState.Error:
                            img = UseLight ? ImagesErrorLight[i] : ImagesError[i];
                            break;

                        default:
                            img = UseLight ? ImagesOffLight[i] : ImagesOff[i];
                            break;
                    }
                    ProductIcons[i].Image = img;
                    LastState[i] = CurrentState;
                }
            }

            // Rx button
            if (ThemeChanged || Props.VariableRateEnabled != LastVRenabled)
            {
                LastVRenabled = Props.VariableRateEnabled;
                if (Props.VariableRateEnabled)
                {
                    btnVR.Image = UseLight ? Properties.Resources.VRonLight : Properties.Resources.VRon;
                }
                else
                {
                    btnVR.Image = UseLight ? Properties.Resources.VRoffLight : Properties.Resources.VRoff;
                }
            }

            // other buttons
            if (ThemeChanged)
            {
                btnMenu.Image = UseLight ? Properties.Resources.MenuLight : Properties.Resources.article;
                btnMinimize.Image = UseLight ? Properties.Resources.MinimizeLight : Properties.Resources.arrow_circle_down_right;
                butPowerOff.Image = UseLight ? Properties.Resources.SwitchOffLight : Properties.Resources.SwitchOff;
            }

            // AOG
            bool aogConnected = Core.GPS.TWOLconnected() || Core.AutoSteerPGN.Connected();

            if (ThemeChanged || aogConnected != LastAogConnected)
            {
                LastAogConnected = Core.GPS.TWOLconnected() || Core.AutoSteerPGN.Connected();

                if (Core.GPS.TWOLconnected() || Core.AutoSteerPGN.Connected())
                {
                    pbAOGstatus.Image = UseLight ? Properties.Resources.AOG_On_Light : Properties.Resources.AOG_On;
                }
                else
                {
                    pbAOGstatus.Image = UseLight ? Properties.Resources.AOG_Off_Light : Properties.Resources.AOG_Off;
                }
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
                btnMaster.Image = Properties.Resources.SprayOff2;
            }
        }
    }
}