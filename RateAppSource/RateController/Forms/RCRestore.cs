using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController
{
    public partial class RCRestore : Form
    {
        private clsProduct cCurrentProduct;
        private int cRateType;
        private Form FormToHide;
        private MouseButtons MouseButtonClicked;
        private Point MouseDownLocation;

        public RCRestore(Form CallingForm, int RateType, clsProduct CurrentProduct, FormStart Main)
        {
            FormToHide = CallingForm;
            cRateType = RateType;
            cCurrentProduct = CurrentProduct;
            this.TransparencyKey = this.BackColor;
            Main.ColorChanged += Main_ColorChanged;
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Define the border color and thickness
            Color borderColor = Properties.Settings.Default.ForeColour;
            int borderWidth = 1;

            // Draw the border
            using (Pen pen = new Pen(borderColor, borderWidth))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1);
            }
        }

        private void Main_ColorChanged(object sender, EventArgs e)
        {
            lbRateAmount.ForeColor = Properties.Settings.Default.ForeColour;
            this.BackColor = Properties.Settings.Default.BackColour;
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
            this.BackColor = Properties.Settings.Default.BackColour;
            lbRateAmount.ForeColor = Properties.Settings.Default.ForeColour;
        }

        private void RestoreLC_Click(object sender, EventArgs e)
        {
            if (MouseButtonClicked == MouseButtons.Left)
            {
                FormToHide.WindowState = FormWindowState.Normal;
                timer1.Enabled = false;
                this.Close();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            switch (cRateType)
            {
                case 1:
                    lbRateAmount.Text = cCurrentProduct.CurrentRate().ToString("N1");
                    break;

                default:
                    lbRateAmount.Text = cCurrentProduct.SmoothRate().ToString("N1");
                    break;
            }
        }
    }
}