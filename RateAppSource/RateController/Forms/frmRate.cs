using RateController.Classes;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.Forms
{
    public partial class frmRate : Form
    {
        private bool IsShutDown = false;
        private Point MouseDownLocation;

        public frmRate()
        {
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

        private void Core_AppExit(object sender, EventArgs e)
        {
            ShutDown();
        }

        private void Core_ColorChanged(object sender, EventArgs e)
        {
            lbRate.ForeColor = Properties.Settings.Default.DisplayForeColour;
            this.BackColor = Properties.Settings.Default.DisplayBackColour;
            this.Invalidate();
        }

        private void frmRate_FormClosed(object sender, FormClosedEventArgs e)
        {
            ShutDown();
        }

        private void frmRate_Load(object sender, EventArgs e)
        {
            Core.ColorChanged += Core_ColorChanged;
            Core.MainForm.Minimize += MainForm_Minimize;
            Core.AppExit += Core_AppExit;

            Props.LoadFormLocation(this);
            timer1.Enabled = true;
            lbRate.ForeColor = Properties.Settings.Default.DisplayForeColour;
            this.BackColor = Properties.Settings.Default.DisplayBackColour;
            UpdateForm();
        }


        private void MainForm_Minimize(object sender, EventArgs e)
        {
            Owner = null;   // prevent this form minimizing
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
            }
        }

        private void ShutDown()
        {
            if (!IsShutDown)
            {
                Props.SaveFormLocation(this);
                timer1.Enabled = false;
                Core.ColorChanged -= Core_ColorChanged;
                Core.MainForm.Minimize -= MainForm_Minimize;
                Core.AppExit -= Core_AppExit;
                IsShutDown = true;
            }
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void UpdateForm()
        {
            try
            {
                double Rate = Core.Products.Items[Props.CurrentProduct].CurrentRate();
                lbRate.Text = Rate.ToString("N1");
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmRate/UpdateForm: " + ex.Message);
            }
        }
    }
}