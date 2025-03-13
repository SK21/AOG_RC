using RateController.Classes;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmPressureDisplay : Form
    {
        private FormStart mf;
        private Point MouseDownLocation;

        public frmPressureDisplay(FormStart CallingForm)
        {
            InitializeComponent();
            mf = CallingForm;
            mf.ColorChanged += Mf_ColorChanged;
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

        private void frmPressureDisplay_FormClosed(object sender, FormClosedEventArgs e)
        {
            mf.Tls.SaveFormData(this);
            timer1.Enabled = false;
        }

        private void frmPressureDisplay_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            timer1.Enabled = true;
            lbPressureValue.ForeColor = Properties.Settings.Default.ForeColour;
            this.BackColor = Properties.Settings.Default.BackColour;
            UpdateForm();
        }

        private void Mf_ColorChanged(object sender, EventArgs e)
        {
            lbPressureValue.ForeColor = Properties.Settings.Default.ForeColour;
            this.BackColor = Properties.Settings.Default.BackColour;
        }

        private void mouseMove_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) MouseDownLocation = e.Location;
        }

        private void mouseMove_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) this.Location = new Point(this.Left + e.X - MouseDownLocation.X, this.Top + e.Y - MouseDownLocation.Y);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void UpdateForm()
        {
            double Pressure = 0;

            int ModuleID = mf.Products.Items[mf.CurrentProduct()].ModuleID;
            double RawData = mf.ModulesStatus.Pressure(ModuleID);
            clsPressure pres = mf.PressureObjects.Item(ModuleID);
            if (RawData > pres.MinimumRawData)
            {
                Pressure = pres.Slope * RawData + pres.Intercept;
            }
            lbPressureValue.Text = Pressure.ToString("N1");
        }
    }
}