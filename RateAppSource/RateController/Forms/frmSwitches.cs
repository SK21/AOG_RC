using RateController.Classes;
using RateController.Language;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmSwitches : Form
    {
        private static readonly SwIDs[] SwIdMap = {
            SwIDs.sw0, SwIDs.sw1, SwIDs.sw2,  SwIDs.sw3,
            SwIDs.sw4, SwIDs.sw5, SwIDs.sw6,  SwIDs.sw7,
            SwIDs.sw8, SwIDs.sw9, SwIDs.sw10, SwIDs.sw11,
            SwIDs.sw12,SwIDs.sw13,SwIDs.sw14, SwIDs.sw15
        };
        private RoundedButton[] _switchButtons = new RoundedButton[0];
        private Point DownHome;
        private Form FormToTrack = null;
        private int FullWidth;
        private bool IsManuallyMoved = false;
        private bool IsShutDown = false;
        private Point MouseDownLocation;
        private Point Offset;
        private bool trackingAttached = false;
        private Point UpHome;

        public frmSwitches()
        {
            InitializeComponent();

            // designer layout, restored when not in rate-only mode
            FullWidth = this.ClientSize.Width;
            UpHome = btnUp.Location;
            DownHome = btnDown.Location;

            // drawn triangles rather than font glyphs, so they sit exactly centred and follow
            // the theme (RoundedButton fills them with ForeColor)
            btnUp.Symbol = ButtonSymbol.Up;
            btnDown.Symbol = ButtonSymbol.Down;
        }

        private bool IsPinned => this.Owner != null && FormToTrack == this.Owner;

        public void CreateSwitchButtons()
        {
            foreach (Button b in _switchButtons) Controls.Remove(b);
            _switchButtons = new RoundedButton[0];

            const int btnW = 64, btnH = 46, startX = 12, startY = 68, spacingX = 78, spacingY = 57;

            bool RateOnly = Props.RateSwitchesOnly;
            btnMaster.Visible = !RateOnly;
            btnPrime.Visible = !RateOnly;
            btnAutoRate.Visible = !RateOnly;
            btnAutoSection.Visible = !RateOnly;

            if (RateOnly)
            {
                // rate up/down only, centred in a small panel
                const int panelW = 155, panelH = 68;
                int gapX = (panelW - 2 * btnW) / 3;
                int top = (panelH - btnH) / 2;

                this.ClientSize = new Size(panelW, panelH);
                btnUp.Location = new Point(gapX, top);
                btnDown.Location = new Point(2 * gapX + btnW, top);
            }
            else
            {
                btnUp.Location = UpHome;
                btnDown.Location = DownHome;

                int count = Props.SwitchCount;
                _switchButtons = new RoundedButton[count];

                for (int i = 0; i < count; i++)
                {
                    RoundedButton btn = new RoundedButton();
                    btn.CornerRadius = 8;
                    btn.Size = new Size(btnW, btnH);
                    btn.Location = new Point(startX + (i % 4) * spacingX, startY + (i / 4) * spacingY);
                    btn.Text = (i + 1).ToString();
                    btn.Tag = i;
                    btn.Click += SwitchButton_Click;
                    btn.MouseDown += button_MouseDown;
                    btn.MouseMove += button_MouseMove;
                    btn.MouseUp += frmSwitches_MouseUp;
                    _switchButtons[i] = btn;
                    Controls.Add(btn);
                }

                int rows = count / 4;
                int autoY = startY + rows * spacingY + 11;
                btnAutoRate.Location = new Point(startX, autoY);
                btnAutoSection.Location = new Point(startX + 2 * spacingX, autoY);
                this.ClientSize = new Size(FullWidth, autoY + btnH + 10);
            }

            SetColor();

            // the panel changed size, so re-evaluate whether it still overlaps the tracked form
            TryToPin();
        }

        public void DetachFromOwner()
        {
            try
            {
                this.Owner = null;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmSwitches/Detach: " + ex.Message);
            }
        }

        public void SetDescriptions()
        {
            for (int i = 0; i < _switchButtons.Length; i++)
                _switchButtons[i].Text = (i + 1).ToString();
        }

        public void TrackingSetup()
        {
            try
            {
                Form newFormToTrack = null;

                // Priority selection
                if (Core.MainForm.WindowState != FormWindowState.Minimized)
                {
                    newFormToTrack = Core.MainForm;
                }
                else if (Props.IsFormOpen("RCRestore", false) != null)
                {
                    newFormToTrack = Props.IsFormOpen("RCRestore", false);
                }

                if (newFormToTrack != FormToTrack)
                {
                    // Switch tracking cleanly
                    DetachTrackingFromCurrentForm();
                    FormToTrack = newFormToTrack;
                    AttachTracking(FormToTrack);
                }

                // Attempt pin if appropriate
                if (FormToTrack != null) TryToPin();

                // Refresh z-order
                if (!this.TopMost) this.TopMost = true;
                this.BringToFront();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmSwitches/TrackingSetup: " + ex.Message);
            }
        }

        public bool TryToPin()
        {
            bool Intersects = false;
            try
            {
                if (FormToTrack != null && !FormToTrack.IsDisposed)
                {
                    // Always drop owner first; we will reassign if pin conditions met.
                    this.Owner = null;

                    Rectangle recThis = this.Bounds;
                    Rectangle recTrackForm = FormToTrack.Bounds;

                    Intersects = recThis.IntersectsWith(recTrackForm);
                    if (Intersects)
                    {
                        this.Owner = FormToTrack;
                        // Recompute offset every time we pin to allow manual repositioning before pin.
                        Offset = new Point(this.Location.X - FormToTrack.Location.X,
                                           this.Location.Y - FormToTrack.Location.Y);
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmSwitches/TryToPin: " + ex.Message);
            }
            return Intersects;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Color borderColor = Properties.Settings.Default.DisplayForeColour;
            int borderWidth = 1;
            using (Pen pen = new Pen(borderColor, borderWidth))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1);
            }
        }

        private void AttachTracking(Form frm)
        {
            if (frm == null || frm.IsDisposed || trackingAttached) return;
            try
            {
                frm.LocationChanged += TrackForm;
                frm.FormClosing += StopTrackingForm;
                trackingAttached = true;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmSwitches/AttachTracking: " + ex.Message);
            }
        }

        private void btnAutoRate_Click(object sender, EventArgs e)
        {
            Core.vSwitchBox.PressSwitch(SwIDs.AutoRate);
        }

        private void btnAutoSection_Click(object sender, EventArgs e)
        {
            Core.vSwitchBox.PressSwitch(SwIDs.AutoSection);
        }

        // The buttons cover nearly the whole panel, so the right button drags it from anywhere.
        // Switch actions are left-button only, otherwise starting a drag on a rate button would
        // also send a rate press.
        private void btnDown_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) Core.vSwitchBox.RateDownPressed();
        }

        private void btnDown_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) Core.vSwitchBox.RateDownReleased();
        }

        private void btnMaster_Click(object sender, EventArgs e)
        {
            Core.vSwitchBox.MasterPressed();
        }

        private void btnMaster_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) Core.vSwitchBox.MasterReleased();
        }

        private void btnPrime_Click(object sender, EventArgs e)
        {
            Core.vSwitchBox.PrimePressed();
        }

        private void btnUp_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) Core.vSwitchBox.RateUpPressed();
        }

        private void btnUp_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) Core.vSwitchBox.RateUpReleased();
        }

        private void button_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) MouseDownLocation = e.Location;
        }

        private void button_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.Location = new Point(this.Left + e.X - MouseDownLocation.X,
                                          this.Top + e.Y - MouseDownLocation.Y);
                IsManuallyMoved = true;
            }
        }

        private void Core_AppExit(object sender, EventArgs e)
        {
            if (!IsShutDown) ShutDown();
        }

        private void Core_ColorChanged(object sender, EventArgs e)
        {
            SetColor();
        }

        private void Core_ProfileChanged(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void Core_RestoreMain(object sender, EventArgs e)
        {
            TrackingSetup();
        }

        private void DetachTrackingFromCurrentForm()
        {
            if (!trackingAttached || FormToTrack == null) return;
            try
            {
                FormToTrack.LocationChanged -= TrackForm;
                FormToTrack.FormClosing -= StopTrackingForm;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmSwitches/DetachTracking: " + ex.Message);
            }
            finally
            {
                trackingAttached = false;
            }
        }

        private void frmSwitches_Closed(object sender, FormClosedEventArgs e)
        {
            if (!IsShutDown) ShutDown();
        }

        private void frmSwitches_Load(object sender, EventArgs e)
        {
            Core.SwitchBox.SwitchPGNreceived += SwitchBox_SwitchPGNreceived;
            Core.ProfileChanged += Core_ProfileChanged;
            Core.ColorChanged += Core_ColorChanged;
            Core.MainForm.Minimize += MainForm_Minimize;
            Core.AppExit += Core_AppExit;
            Core.RestoreMain += Core_RestoreMain;

            Props.LoadFormLocation(this);
            CreateSwitchButtons();
            TrackingSetup();
            timer1.Enabled = true;
            Core.vSwitchBox.UsefrmSwitches = true;
            Core.vSwitchBox.PressSwitch(SwIDs.MasterOff);
            SetDescriptions();
            LoadSettings();
        }

        private void frmSwitches_LocationChanged(object sender, EventArgs e)
        {
            try
            {
                // If user manually moves while previously pinned, drop pin until re-evaluated.
                if (IsManuallyMoved && IsPinned)
                {
                    this.Owner = null;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmSwitches/locationChanged " + ex.Message);
            }
        }

        private void frmSwitches_MouseUp(object sender, MouseEventArgs e)
        {
            // When user releases mouse, attempt re-pin if overlapping tracked form.
            IsManuallyMoved = false;
            if (FormToTrack != null && !FormToTrack.IsDisposed)
            {
                TryToPin();
            }
        }

        private void LoadSettings()
        {
            UpdateForm();
            SetLanguage();
        }

        private void MainForm_Minimize(object sender, EventArgs e)
        {
            DetachFromOwner();
        }

        private void mouseMove_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Left) MouseDownLocation = e.Location;
        }

        private void mouseMove_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Left)
            {
                this.Location = new Point(this.Left + e.X - MouseDownLocation.X,
                                          this.Top + e.Y - MouseDownLocation.Y);
                IsManuallyMoved = true;
            }
        }

        private void SetColor()
        {
            this.BackColor = Properties.Settings.Default.DisplayBackColour;

            foreach (Control c in this.Controls)
            {
                if (c is Button) c.ForeColor = Properties.Settings.Default.DisplayForeColour;
            }
            UpdateForm();
            this.Invalidate();   // repaint the border
        }

        private void SetSwitchColor(Button btn, bool IsOn)
        {
            // off leaves BackColor unset so it inherits the themed form background, the same way
            // btnUnits on the main form does
            if (IsOn)
            {
                btn.BackColor = Core.Tls.SwitchOnColor();
            }
            else
            {
                btn.ResetBackColor();
            }
        }

        private void SetLanguage()
        {
            btnAutoRate.Text = Lang.lgAutoRate;
            btnAutoSection.Text = Lang.lgAutoSection;
        }

        private void ShutDown()
        {
            Props.SaveFormLocation(this);
            timer1.Enabled = false;
            Core.vSwitchBox.UsefrmSwitches = false;
            Core.SwitchBox.SwitchPGNreceived -= SwitchBox_SwitchPGNreceived;
            Core.ProfileChanged -= Core_ProfileChanged;
            Core.ColorChanged -= Core_ColorChanged;
            Core.MainForm.Minimize -= MainForm_Minimize;
            Core.AppExit -= Core_AppExit;
            Core.RestoreMain -= Core_RestoreMain;

            DetachTrackingFromCurrentForm();
            IsShutDown = true;
        }

        private void StopTrackingForm(object sender, FormClosingEventArgs e)
        {
            // Tracked form is closing; detach ownership so this form remains.
            DetachTrackingFromCurrentForm();
            this.Owner = null;
        }

        private void SwitchBox_SwitchPGNreceived(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void SwitchButton_Click(object sender, EventArgs e)
        {
            int idx = (int)((Button)sender).Tag;
            if (idx < SwIdMap.Length) Core.vSwitchBox.PressSwitch(SwIdMap[idx]);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void TrackForm(object sender, EventArgs e)
        {
            try
            {
                if (FormToTrack == null || FormToTrack.IsDisposed) return;

                if (IsPinned)
                {
                    Point desiredLocation = new Point(FormToTrack.Location.X + Offset.X,
                                                      FormToTrack.Location.Y + Offset.Y);

                    if (this.Location != desiredLocation)
                    {
                        Point oldLocation = this.Location;
                        this.Location = desiredLocation;

                        // Revert if new location off-screen
                        if (!Props.IsOnScreen(this, false))
                        {
                            this.Location = oldLocation;
                        }

                        // Bring to front (less flicker than toggling TopMost)
                        if (!this.TopMost) this.TopMost = true;
                        this.BringToFront();
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmSwitches/TrackForm: " + ex.Message);
            }
        }

        private void UpdateForm()
        {
            btnPrime.Enabled = !Props.MasterMaintained && Props.Speed_KMH < 0.1;

            SetSwitchColor(btnPrime, Core.SectionControl.PrimeOn);
            SetSwitchColor(btnMaster, Core.SwitchBox.MasterOn);

            SetSwitchColor(btnUp, Core.SwitchBox.SwitchIsOn(SwIDs.RateUp));
            SetSwitchColor(btnDown, Core.SwitchBox.SwitchIsOn(SwIDs.RateDown));

            for (int i = 0; i < _switchButtons.Length; i++)
                SetSwitchColor(_switchButtons[i], Core.SwitchBox.SwitchIsOn(SwIdMap[i]));

            SetSwitchColor(btnAutoSection, Core.SwitchBox.AutoSectionOn);
            SetSwitchColor(btnAutoRate, Core.SwitchBox.AutoRateOn);
        }
    }
}
