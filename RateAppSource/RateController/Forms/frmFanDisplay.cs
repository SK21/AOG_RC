using RateController.Classes;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController
{
    // Small always-on-top fan RPM window. Same behaviour and theme as frmPressureDisplay
    // (no title bar, drag anywhere, pins to the main form when overlapping it, detaches
    // when the main form minimizes), with two differences: this form is bound to one
    // specific fan for its lifetime rather than following the current product, and RPM
    // has no metric/imperial variation so there is no units handling.
    public partial class frmFanDisplay : Form
    {
        private readonly int FanProductID;
        private readonly string InstanceKey;
        private Form FormToTrack = null;
        private bool IsManuallyMoved = false;
        private bool IsShutDown = false;
        private Point MouseDownLocation;
        private Point Offset;
        private bool trackingAttached = false;

        public frmFanDisplay(int ProductID)
        {
            InitializeComponent();
            FanProductID = ProductID;
            InstanceKey = ProductID.ToString();
        }

        // Distinguishes this fan's window from the other one, for both the FormManager
        // registry and the saved screen position.
        public string Instance
        { get { return InstanceKey; } }

        private bool IsPinned => this.Owner != null && FormToTrack == this.Owner;

        public void DetachFromOwner()
        {
            try
            {
                this.Owner = null;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmFanDisplay/Detach: " + ex.Message);
            }
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
                Props.WriteErrorLog("frmFanDisplay/TrackingSetup: " + ex.Message);
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
                Props.WriteErrorLog("frmFanDisplay/TryToPin: " + ex.Message);
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
                Props.WriteErrorLog("frmFanDisplay/AttachTracking: " + ex.Message);
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
                Props.WriteErrorLog("frmFanDisplay/DetachTracking: " + ex.Message);
            }
            finally
            {
                trackingAttached = false;
            }
        }

        private void frmFanDisplay_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!IsShutDown) ShutDown();
        }

        private void frmFanDisplay_Load(object sender, EventArgs e)
        {
            Core.ColorChanged += Core_ColorChanged;
            Core.MainForm.Minimize += MainForm_Minimize;
            Core.AppExit += Core_AppExit;
            Core.RestoreMain += Core_RestoreMain;

            Props.LoadFormLocation(this, InstanceKey);

            // With no saved position both fan windows centre on the same spot and sit
            // exactly on top of each other, so the second one looks like it never opened.
            // Step each one below the last only on that first run; afterwards the user's
            // own placement is what gets loaded.
            if (Props.GetAppProp(this.Name + InstanceKey + ".Top") == string.Empty)
            {
                this.Top += (FanProductID - (Props.MaxProducts - 2)) * (this.Height + 6);
                Props.IsOnScreen(this);
            }

            TrackingSetup();
            timer1.Enabled = true;
            SetColor();
            UpdateForm();
        }

        private void frmFanDisplay_LocationChanged(object sender, EventArgs e)
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
                Props.WriteErrorLog("frmFanDisplay/locationChanged " + ex.Message);
            }
        }

        private void frmFanDisplay_MouseUp(object sender, MouseEventArgs e)
        {
            // When user releases mouse, attempt re-pin if overlapping tracked form.
            IsManuallyMoved = false;
            if (FormToTrack != null && !FormToTrack.IsDisposed)
            {
                TryToPin();
            }
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
            lbFanName.ForeColor = Properties.Settings.Default.DisplayForeColour;
            lbFanValue.ForeColor = Properties.Settings.Default.DisplayForeColour;
            this.BackColor = Properties.Settings.Default.DisplayBackColour;
        }

        private void ShutDown()
        {
            Props.SaveFormLocation(this, InstanceKey);
            timer1.Enabled = false;
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
                Props.WriteErrorLog("frmFanDisplay/TrackForm: " + ex.Message);
            }
        }

        private void UpdateForm()
        {
            try
            {
                clsProduct Fan = Core.Products.Items[FanProductID];
                lbFanName.Text = Fan.ProductName;
                lbFanValue.Text = Fan.UPMapplied().ToString("N0");
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmFanDisplay/UpdateForm: " + ex.Message);
            }
        }
    }
}
