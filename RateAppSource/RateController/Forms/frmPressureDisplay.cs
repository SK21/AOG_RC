using RateController.Classes;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmPressureDisplay : Form
    {
        private bool IsManuallyMoved = false;
        private FormStart mf;
        private Point MouseDownLocation;
        private Point Offset;
        private bool PinForm = false;

        public frmPressureDisplay(FormStart CallingForm)
        {
            InitializeComponent();
            mf = CallingForm;
            mf.ColorChanged += Mf_ColorChanged;
            Props.ScreensSwitched += Props_ScreensSwitched;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Define the border color and thickness
            Color borderColor = Properties.Settings.Default.DisplayForeColour;
            int borderWidth = 1;

            // Draw the border
            using (Pen pen = new Pen(borderColor, borderWidth))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1);
            }
        }

        private void frmPressureDisplay_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
            timer1.Enabled = false;
            mf.ColorChanged -= Mf_ColorChanged;
            Props.ScreensSwitched -= Props_ScreensSwitched;

            if (Props.IsFormOpen("frmLargeScreen", false) != null)
            {
                mf.Lscrn.LocationChanged -= TrackLscrn;
                mf.Lscrn.FormClosing -= Lscrn_FormClosing;
            }
        }

        private void frmPressureDisplay_Load(object sender, EventArgs e)
        {
            Props.LoadFormLocation(this);
            TrackingSetup();
            timer1.Enabled = true;
            lbPressureValue.ForeColor = Properties.Settings.Default.DisplayForeColour;
            this.BackColor = Properties.Settings.Default.DisplayBackColour;
            UpdateForm();
        }

        private void frmPressureDisplay_LocationChanged(object sender, EventArgs e)
        {
            try
            {
                if (Props.IsFormOpen("frmLargeScreen", false) != null)
                {
                    if (PinForm && IsManuallyMoved)
                    {
                        Offset = new Point(this.Location.X - mf.Lscrn.Location.X, this.Location.Y - mf.Lscrn.Location.Y);
                    }

                    PinForm = PinToLargeScreen();
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmPressureDisplay/locationChanged " + ex.Message);
            }
        }

        private void frmPressureDisplay_MouseUp(object sender, MouseEventArgs e)
        {
            IsManuallyMoved = false;
        }

        private void Lscrn_FormClosing(object sender, FormClosingEventArgs e)
        {
            mf.Lscrn.LocationChanged -= TrackLscrn;
            mf.Lscrn.FormClosing -= Lscrn_FormClosing;
            this.Owner = null;  // prevent this form closing
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
                this.Location = new Point(this.Left + e.X - MouseDownLocation.X, this.Top + e.Y - MouseDownLocation.Y);
                IsManuallyMoved = true;
            }
        }

        private bool PinToLargeScreen()
        {
            bool Intersects = false;
            this.Owner = null;
            if (Props.IsFormOpen("frmLargeScreen", false) != null)
            {
                // only pin to Lscrn if this screen is intentionally over Lscrn
                Rectangle RecThis = this.Bounds;
                Rectangle RecLS = mf.Lscrn.Bounds;
                Intersects = RecThis.IntersectsWith(RecLS);
                if (Intersects) this.Owner = mf.Lscrn;
            }
            return Intersects;
        }

        private void Props_ScreensSwitched(object sender, EventArgs e)
        {
            TrackingSetup();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void TrackingSetup()
        {
            try
            {
                if (Props.IsFormOpen("frmLargeScreen", false) != null)
                {
                    mf.Lscrn.LocationChanged += TrackLscrn;
                    mf.Lscrn.FormClosing += Lscrn_FormClosing;
                    if (PinToLargeScreen())
                    {
                        Offset = new Point(this.Location.X - mf.Lscrn.Location.X, this.Location.Y - mf.Lscrn.Location.Y);
                        PinForm = true;
                    }

                    // refresh
                    this.TopMost = false;
                    this.TopMost = true;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmPressureDisplay/TrackingSetup: " + ex.Message);
            }
        }

        private void TrackLscrn(object sender, EventArgs e)
        {
            try
            {
                if (PinForm)
                {
                    Point desiredLocation = new Point(mf.Lscrn.Location.X + Offset.X, mf.Lscrn.Location.Y + Offset.Y);

                    // Only update if location has changed
                    if (this.Location != desiredLocation)
                    {
                        Point oldLocation = this.Location;
                        this.Location = desiredLocation;

                        // Revert if new location is off-screen
                        if (!Props.CheckOnScreen(this, false)) this.Location = oldLocation;

                        // refresh
                        this.TopMost = false;
                        this.TopMost = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmPressureDisplay/TrackLscrn: " + ex.Message);
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
                lbPressureValue.Text = Pressure.ToString("N2");
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmPressureDisplay/UpdateForm: " + ex.Message);
            }
        }
    }
}