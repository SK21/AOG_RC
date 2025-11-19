using RateController.Classes;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmPressureDisplay : Form
    {
        private Form FormToTrack = null;
        private bool IsManuallyMoved = false;
        private FormStart mf;
        private Point MouseDownLocation;
        private string NumberFormat;
        private Point Offset;
        private bool trackingAttached = false;

        public frmPressureDisplay(FormStart CallingForm)
        {
            InitializeComponent();
            mf = CallingForm;
            mf.ColorChanged += Mf_ColorChanged;
            Props.ScreensSwitched += Props_ScreensSwitched;
            Props.UnitsChanged += Props_UnitsChanged;
        }

        private bool IsPinned => this.Owner != null && FormToTrack == this.Owner;

        public void DetachFromOwner()
        {
            try
            {
                this.Owner = null;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmPressureDisplay/Detach: " + ex.Message);
            }
        }

        public void TrackingSetup()
        {
            try
            {
                Form newFormToTrack = null;

                // Priority selection
                if (Props.IsFormOpen("frmLargeScreen", false) != null &&
                    mf.Lscrn != null &&
                    mf.Lscrn.WindowState != FormWindowState.Minimized)
                {
                    newFormToTrack = mf.Lscrn;
                }
                else if (mf.WindowState != FormWindowState.Minimized)
                {
                    newFormToTrack = mf;
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
                Props.WriteErrorLog("frmPressureDisplay/TrackingSetup: " + ex.Message);
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
                Props.WriteErrorLog("frmPressureDisplay/TryToPin: " + ex.Message);
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
                Props.WriteErrorLog("frmPressureDisplay/AttachTracking: " + ex.Message);
            }
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
                Props.WriteErrorLog("frmPressureDisplay/DetachTracking: " + ex.Message);
            }
            finally
            {
                trackingAttached = false;
            }
        }

        private void frmPressureDisplay_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
            timer1.Enabled = false;
            mf.ColorChanged -= Mf_ColorChanged;
            Props.ScreensSwitched -= Props_ScreensSwitched;
            Props.UnitsChanged -= Props_UnitsChanged;
            DetachTrackingFromCurrentForm();
        }

        private void frmPressureDisplay_Load(object sender, EventArgs e)
        {
            Props.LoadFormLocation(this);
            TrackingSetup();
            timer1.Enabled = true;
            lbPressureValue.ForeColor = Properties.Settings.Default.DisplayForeColour;
            this.BackColor = Properties.Settings.Default.DisplayBackColour;
            SetForUnits();
            UpdateForm();
        }

        private void frmPressureDisplay_LocationChanged(object sender, EventArgs e)
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
                Props.WriteErrorLog("frmPressureDisplay/locationChanged " + ex.Message);
            }
        }

        private void frmPressureDisplay_MouseUp(object sender, MouseEventArgs e)
        {
            // When user releases mouse, attempt re-pin if overlapping tracked form.
            IsManuallyMoved = false;
            if (FormToTrack != null && !FormToTrack.IsDisposed)
            {
                TryToPin();
            }
        }

        private void Mf_ColorChanged(object sender, EventArgs e)
        {
            lbPressureValue.ForeColor = Properties.Settings.Default.DisplayForeColour;
            this.BackColor = Properties.Settings.Default.DisplayBackColour;
            this.Invalidate();
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

        private void Props_ScreensSwitched(object sender, EventArgs e)
        {
            TrackingSetup();
        }

        private void Props_UnitsChanged(object sender, EventArgs e)
        {
            SetForUnits();
            // Width change can affect desired visual relationship; recompute offset if pinned.
            if (IsPinned) Offset = new Point(this.Location.X - FormToTrack.Location.X,
                                             this.Location.Y - FormToTrack.Location.Y);
        }

        private void SetForUnits()
        {
            if (Props.UseMetric)
            {
                this.Width = 230;
                NumberFormat = "N2";
            }
            else
            {
                this.Width = 210;
                NumberFormat = "N0";
            }
            lbPressureValue.Width = this.Width - 79;
        }

        private void StopTrackingForm(object sender, FormClosingEventArgs e)
        {
            // Tracked form is closing; detach ownership so this form remains.
            DetachTrackingFromCurrentForm();
            this.Owner = null;
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
                Props.WriteErrorLog("frmPressureDisplay/TrackForm: " + ex.Message);
            }
        }

        private void UpdateForm()
        {
            try
            {
                double Pressure = 0;
                int ModuleID = mf.Products.Items[mf.CurrentProduct()].ModuleID;
                double RawData = mf.ModulesStatus.PressureReading(ModuleID);
                Pressure = Props.PressureReading(ModuleID, RawData);
                lbPressureValue.Text = Pressure.ToString(NumberFormat);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmPressureDisplay/UpdateForm: " + ex.Message);
            }
        }
    }
}