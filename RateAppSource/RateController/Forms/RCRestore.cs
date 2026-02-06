using RateController.Classes;
using RateController.Forms;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController
{
    public partial class RCRestore : Form
    {
        private bool IsShutDown = false;
        private MouseButtons MouseButtonClicked;
        private Point MouseDownLocation;

        public RCRestore()
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
            SetColor();
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

        private void RCRestore_FormClosing(object sender, FormClosingEventArgs e)
        {
            ShutDown();
        }

        private void RCRestore_Load(object sender, EventArgs e)
        {
            Core.ColorChanged += Core_ColorChanged;
            Core.AppExit += Core_AppExit;

            frmMain FormToHide = Core.MainForm;
            this.Top = FormToHide.Top + FormToHide.Height - this.Height;
            this.Left = FormToHide.Left + FormToHide.Width - this.Width;

            FormToHide.WindowState = FormWindowState.Minimized;
            timer1.Enabled = true;
            SetColor();
            UpdateForm();
        }

        private void RestoreLC_Click(object sender, EventArgs e)
        {
            if (MouseButtonClicked == MouseButtons.Left)
            {
                timer1.Enabled = false;

                Core.MainForm.WindowState = FormWindowState.Normal;
                Core.RaiseRestoreMain();
                this.Close();
            }
        }

        private void SetColor()
        {
            lbRate.ForeColor = Properties.Settings.Default.DisplayForeColour;
            this.BackColor = Properties.Settings.Default.DisplayBackColour;
        }

        private void ShutDown()
        {
            if (!IsShutDown)
            {
                Core.ColorChanged -= Core_ColorChanged;
                Core.AppExit -= Core_AppExit;

                IsShutDown = true;
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
                double Rate = Core.Products.Items[Props.CurrentProduct].CurrentRate();
                lbRate.Text = Rate.ToString("N1");
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("RCRestore/UpdateForm: " + ex.Message);
            }
        }
    }
}