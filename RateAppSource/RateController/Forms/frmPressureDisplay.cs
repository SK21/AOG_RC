using RateController.Classes;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmPressureDisplay : Form
    {
        private clsLatch Latch;
        private bool IsShutDown = false;
        private Point MouseDownLocation;
        private string NumberFormat;

        public frmPressureDisplay()
        {
            InitializeComponent();
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

        private void frmPressureDisplay_FormClosing(object sender, FormClosingEventArgs e)
        {
            // A latched window is an owned window, so the window it is latched to closing
            // drags this one into the close as well. TargetIsClosing unlatches and returns
            // true when that is all this is, and the display stays open.
            bool Rescued = (Latch != null) && Latch.TargetIsClosing(e);

            if (!Rescued && !IsShutDown) ShutDown();
        }

        private void frmPressureDisplay_Load(object sender, EventArgs e)
        {
            Latch = new clsLatch(this);

            Props.UnitsChanged += Props_UnitsChanged;
            Core.ColorChanged += Core_ColorChanged;
            Core.MainForm.Minimize += MainForm_Minimize;
            Core.AppExit += Core_AppExit;
            Core.RestoreMain += Core_RestoreMain;

            Props.LoadFormLocation(this);
            Latch.Setup();
            timer1.Enabled = true;
            SetColor();
            SetForUnits();
            UpdateForm();
        }

        private void frmPressureDisplay_LocationChanged(object sender, EventArgs e)
        {
            // Latch is null until Load runs; the designer sets a location before that.
            if (Latch != null) Latch.Moved();
        }

        private void frmPressureDisplay_MouseUp(object sender, MouseEventArgs e)
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

        private void Props_UnitsChanged(object sender, EventArgs e)
        {
            SetForUnits();
            // Width change can affect desired visual relationship; recompute the offset.
            Latch.Refresh();
        }

        private void SetColor()
        {
            lbPressureValue.ForeColor = Properties.Settings.Default.DisplayForeColour;
            this.BackColor = Properties.Settings.Default.DisplayBackColour;
        }

        private void SetForUnits()
        {
            this.Width = 230;
            if (Props.UseMetric)
            {
                NumberFormat = "N2";
            }
            else
            {
                NumberFormat = "N0";
            }
            lbPressureValue.Width = this.Width - 79;
        }

        private void ShutDown()
        {
            Props.SaveFormLocation(this);
            timer1.Enabled = false;
            Core.ColorChanged -= Core_ColorChanged;
            Props.UnitsChanged -= Props_UnitsChanged;
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
                double Pressure = 0;
                int ModuleID = Core.Products.Items[Props.CurrentProduct].ModuleID;
                double RawData = Core.ModulesStatus.PressureReading(ModuleID);
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