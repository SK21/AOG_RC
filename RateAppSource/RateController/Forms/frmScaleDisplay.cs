using RateController.Classes;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmScaleDisplay : Form
    {
        private int cDisplayMode = 0;
        private int cProductID;

        // Display mode: 0 - weight, 1 - applied, 2 - acres, 3 - rate
        private bool IsManuallyMoved = false;

        private FormStart mf;
        private Point MouseDownLocation;
        private Point Offset;
        private bool PinForm = false;
        private double StartingAcres = 0;
        private double StartingWeight = 0;
        private double TareWeight = 0;

        public frmScaleDisplay(FormStart CallingForm, int ProductID)
        {
            InitializeComponent();
            mf = CallingForm;
            cProductID = ProductID;
            this.Text = "Scale " + Prd();
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

        private void btnScale_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Click to cycle through display.\nCurrent weight, Applied weight, Area, and Rate.";

            Props.ShowMessage(Message);
            hlpevent.Handled = true;
        }

        private double CurrentAcres()
        {
            double Area = mf.Products.Item(cProductID).CurrentCoverage();
            return Area;
        }

        private void frmScaleDisplay_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this, cProductID.ToString());
            timer1.Enabled = false;
            SaveData();

            mf.ColorChanged -= Mf_ColorChanged;
            Props.ScreensSwitched -= Props_ScreensSwitched;

            if (Props.IsFormOpen("frmLargeScreen", false) != null)
            {
                mf.Lscrn.LocationChanged -= TrackLscrn;
                mf.Lscrn.FormClosing -= Lscrn_FormClosing;
            }
        }

        private void frmScaleDisplay_Load(object sender, EventArgs e)
        {
            Props.LoadFormLocation(this, cProductID.ToString());
            TrackingSetup();
            timer1.Enabled = true;
            LoadData();
            lbValue.ForeColor = Properties.Settings.Default.DisplayForeColour;
            this.BackColor = Properties.Settings.Default.DisplayBackColour;
            UpdateForm();
        }

        private void frmScaleDisplay_LocationChanged(object sender, EventArgs e)
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
                Props.WriteErrorLog("frmScaleDisplay/locationChanged " + ex.Message);
            }
        }

        private void frmScaleDisplay_MouseUp(object sender, MouseEventArgs e)
        {
            IsManuallyMoved = false;
        }

        private void lbValue_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Click to reset starting weight and acres.\nRight click to reset tare weight.";

            Props.ShowMessage(Message);
            hlpevent.Handled = true;
        }

        private void lbValue_MouseClick(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    var Hlp = new frmMsgBox(mf, "Reset starting weight and acres?", "Reset", true);
                    Hlp.TopMost = true;

                    Hlp.ShowDialog();
                    bool Result = Hlp.Result;
                    Hlp.Close();
                    if (Result)
                    {
                        StartingAcres = CurrentAcres();
                        StartingWeight = NetWeight();
                    }
                    break;

                case MouseButtons.Right:
                    var Hlp2 = new frmMsgBox(mf, "Reset tare weight?", "Reset", true);
                    Hlp2.TopMost = true;

                    Hlp2.ShowDialog();
                    bool Result2 = Hlp2.Result;
                    Hlp2.Close();
                    if (Result2)
                    {
                        TareWeight = mf.ScaleIndicator.Value(cProductID);
                    }
                    break;
            }
        }

        private void LoadData()
        {
            if (int.TryParse(Props.GetProp("Scale_DisplayMode" + cProductID.ToString()), out int md)) cDisplayMode = md;
            if (double.TryParse(Props.GetProp("Scale_Area" + cProductID.ToString()), out double ar)) StartingAcres = ar;
            if (double.TryParse(Props.GetProp("Scale_Weight" + cProductID.ToString()), out double wt)) StartingWeight = wt;
            if (double.TryParse(Props.GetProp("Scale_Tare" + cProductID.ToString()), out double ta)) TareWeight = ta;
        }

        private void Lscrn_FormClosing(object sender, FormClosingEventArgs e)
        {
            mf.Lscrn.LocationChanged -= TrackLscrn;
            mf.Lscrn.FormClosing -= Lscrn_FormClosing;
            this.Owner = null;  // prevent this form closing
        }

        private void Mf_ColorChanged(object sender, EventArgs e)
        {
            lbValue.ForeColor = Properties.Settings.Default.DisplayForeColour;
            this.BackColor = Properties.Settings.Default.DisplayBackColour;
        }

        private void mouseMove_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) MouseDownLocation = e.Location;
        }

        private void mouseMove_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Left)
            {
                this.Location = new Point(this.Left + e.X - MouseDownLocation.X, this.Top + e.Y - MouseDownLocation.Y);
                IsManuallyMoved = true;
            }
        }

        private double NetWeight()
        {
            double wt = mf.ScaleIndicator.Value(cProductID) - TareWeight;
            return wt;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    cDisplayMode++;
                    if (cDisplayMode > 3) cDisplayMode = 0;
                    UpdateForm();
                    break;

                case MouseButtons.Right:
                    break;
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

        private string Prd()
        {
            return (cProductID + 1).ToString();
        }

        private void Props_ScreensSwitched(object sender, EventArgs e)
        {
            TrackingSetup();
        }

        private void SaveData()
        {
            Props.SetProp("Scale_DisplayMode" + cProductID.ToString(), cDisplayMode.ToString());
            Props.SetProp("Scale_Area" + cProductID.ToString(), StartingAcres.ToString());
            Props.SetProp("Scale_Weight" + cProductID.ToString(), StartingWeight.ToString());
            Props.SetProp("Scale_Tare" + cProductID.ToString(), TareWeight.ToString());
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
                        if (!Props.IsOnScreen(this, false)) this.Location = oldLocation;

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
                switch (cDisplayMode)
                {
                    case 1:
                        // applied
                        lbValue.Text = (StartingWeight - NetWeight()).ToString("N1") + "\n(Applied " + Prd() + ")";
                        break;

                    case 2:
                        // acres
                        lbValue.Text = (StartingAcres - CurrentAcres()).ToString("N1") + "\n(Area " + Prd() + ")";
                        break;

                    case 3:
                        // rate
                        double Applied = StartingWeight - NetWeight();
                        double Area = StartingAcres - CurrentAcres();
                        double Rate = 0;
                        if (Area > 0)
                        {
                            Rate = Applied / Area;
                        }
                        lbValue.Text = Rate.ToString("N1") + "\n(Rate " + Prd() + ")";
                        break;

                    default:
                        // weight
                        lbValue.Text = NetWeight().ToString("N1") + "\n(Weight " + Prd() + ")";
                        break;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmScaleDisplay/UpdateForm: " + ex.Message);
            }
        }
    }
}