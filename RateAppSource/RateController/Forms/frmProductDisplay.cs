using RateController.Classes;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController
{
    // Small always-on-top window showing one product's number - applied rate for a
    // product, RPM for a fan. Bound to one product for its lifetime, so several can be
    // open at once, each keyed by product ID for the FormManager registry and for its
    // own saved screen position. Opened and closed by Props.DisplayProducts().
    //
    // Latching (drop it on another window and it moves with that window) is handled by
    // clsLatch, shared with the pressure display and the switch panel.
    public partial class frmProductDisplay : Form
    {
        private readonly int DisplayProductID;
        private readonly string InstanceKey;
        private readonly bool IsFan;
        private clsLatch Latch;
        private bool IsShutDown = false;
        private Point MouseDownLocation;

        public frmProductDisplay(int ProductID)
        {
            InitializeComponent();
            DisplayProductID = ProductID;
            InstanceKey = ProductID.ToString();
            IsFan = (ProductID > Props.MaxProducts - 3);
        }

        // Distinguishes this product's window from the others, for both the FormManager
        // registry and the saved screen position.
        public string Instance
        { get { return InstanceKey; } }

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
            Latch.Setup();
        }

        private void frmProductDisplay_FormClosing(object sender, FormClosingEventArgs e)
        {
            // A latched window is an owned window, so the window it is latched to closing
            // drags this one into the close as well. TargetIsClosing unlatches and returns
            // true when that is all this is, and the display stays open.
            bool Rescued = (Latch != null) && Latch.TargetIsClosing(e);

            if (!Rescued && !IsShutDown) ShutDown();
        }

        private void frmProductDisplay_Load(object sender, EventArgs e)
        {
            Latch = new clsLatch(this);

            Core.ColorChanged += Core_ColorChanged;
            Core.MainForm.Minimize += MainForm_Minimize;
            Core.AppExit += Core_AppExit;
            Core.RestoreMain += Core_RestoreMain;

            Props.LoadFormLocation(this, InstanceKey);

            // With no saved position every display window centres on the same spot and
            // sits exactly on top of the others, so all but one look like they never
            // opened. Step each one below the last only on that first run; afterwards
            // the user's own placement is what gets loaded.
            if (Props.GetAppProp(this.Name + InstanceKey + ".Top") == string.Empty)
            {
                this.Top += DisplayProductID * (this.Height + 6);
                Props.IsOnScreen(this);
            }

            Latch.Setup();
            timer1.Enabled = true;
            SetColor();
            UpdateForm();
        }

        private void frmProductDisplay_LocationChanged(object sender, EventArgs e)
        {
            // Latch is null until Load runs; the designer sets a location before that.
            if (Latch != null) Latch.Moved();
        }

        private void frmProductDisplay_MouseUp(object sender, MouseEventArgs e)
        {
            Latch.Dropped();
        }

        private void MainForm_Minimize(object sender, EventArgs e)
        {
            Latch.HostMinimized();
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
                Latch.MovedByHand();
            }
        }

        private void SetColor()
        {
            lbProductName.ForeColor = Properties.Settings.Default.DisplayForeColour;
            lbValue.ForeColor = Properties.Settings.Default.DisplayForeColour;
            this.BackColor = Properties.Settings.Default.DisplayBackColour;
            this.Invalidate();   // repaint the border
        }

        private void ShutDown()
        {
            Props.SaveFormLocation(this, InstanceKey);
            timer1.Enabled = false;
            Core.ColorChanged -= Core_ColorChanged;
            Core.MainForm.Minimize -= MainForm_Minimize;
            Core.AppExit -= Core_AppExit;
            Core.RestoreMain -= Core_RestoreMain;

            if (Latch != null) Latch.ShutDown();
            IsShutDown = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void UpdateForm()
        {
            try
            {
                clsProduct Prd = Core.Products.Items[DisplayProductID];
                lbProductName.Text = Prd.ProductName;

                // a fan is set and read in RPM, everything else in rate units
                if (IsFan)
                {
                    lbValue.Text = Prd.UPMapplied().ToString("N0");
                }
                else
                {
                    lbValue.Text = Prd.CurrentRate().ToString("N1");
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmProductDisplay/UpdateForm: " + ex.Message);
            }
        }
    }
}
