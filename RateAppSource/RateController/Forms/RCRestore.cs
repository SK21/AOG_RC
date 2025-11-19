using RateController.Classes;
using RateController.Forms;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController
{
    public partial class RCRestore : Form
    {
        private int cRateType;
        private Form FormToHide;
        private FormStart mf;
        private MouseButtons MouseButtonClicked;
        private Point MouseDownLocation;

        public RCRestore(Form CallingForm, int RateType, FormStart Main)
        {
            mf = Main;
            FormToHide = CallingForm;
            cRateType = RateType;
            this.TransparencyKey = this.BackColor;
            Main.ColorChanged += Main_ColorChanged;
            InitializeComponent();
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

        private void Main_ColorChanged(object sender, EventArgs e)
        {
            lbRate.ForeColor = Properties.Settings.Default.DisplayForeColour;
            this.BackColor = Properties.Settings.Default.DisplayBackColour;
        }

        private void mouseMove_MouseDown(object sender, MouseEventArgs e)
        {
            // Log the current window location and the mouse location.
            MouseButtonClicked = e.Button;
            if (e.Button == MouseButtons.Right) MouseDownLocation = e.Location;
        }

        private void mouseMove_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) this.Location = new Point(this.Left + e.X - MouseDownLocation.X, this.Top + e.Y - MouseDownLocation.Y);
        }

        private void RCRestore_Load(object sender, EventArgs e)
        {
            this.Top = FormToHide.Top + FormToHide.Height - this.Height;
            this.Left = FormToHide.Left + FormToHide.Width - this.Width;

            FormToHide.WindowState = FormWindowState.Minimized;
            timer1.Enabled = true;
            this.BackColor = Properties.Settings.Default.DisplayBackColour;
            lbRate.ForeColor = Properties.Settings.Default.DisplayForeColour;
            UpdateForm();

            frmPressureDisplay pressure = (frmPressureDisplay)Props.IsFormOpen("frmPressureDisplay", false);
            if (pressure != null)
            {
                // Ensure it starts tracking this RCRestore window (and pins if overlapping)
                pressure.TrackingSetup();
            }
        }

        private void RestoreLC_Click(object sender, EventArgs e)
        {
            if (MouseButtonClicked == MouseButtons.Left)
            {
                FormToHide.WindowState = FormWindowState.Normal;

                frmPressureDisplay pressure = (frmPressureDisplay)Props.IsFormOpen("frmPressureDisplay", false);
                if (pressure != null)
                {
                    // Re-evaluate to attach back to the main form or large screen as needed
                    pressure.TrackingSetup();
                }

                timer1.Enabled = false;

                frmRate RT = (frmRate)Props.IsFormOpen("frmRate", false);
                if (RT != null)
                {
                    RT.TrackingSetup();
                }

                this.Close();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void UpdateForm()
        {
            try
            {
                double Rate = mf.Products.Items[mf.CurrentProduct()].CurrentRate();
                lbRate.Text = Rate.ToString("N1");
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("RCRestore/UpdateForm: " + ex.Message);
            }
        }
    }
}