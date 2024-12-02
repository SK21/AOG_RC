using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmPressureDisplay : Form
    {
        private FormStart mf;
        private int mouseX = 0;
        private int mouseY = 0;
        private int windowLeft = 0;
        private int windowTop = 0;

        public frmPressureDisplay(FormStart CallingForm)
        {
            InitializeComponent();
            mf = CallingForm;
            mf.ColorChanged += Mf_ColorChanged;
        }

        private void Mf_ColorChanged(object sender, EventArgs e)
        {
            lbPressureValue.ForeColor = Properties.Settings.Default.ForeColour;
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
            UpdateForm();
        }

        private void mouseMove_MouseDown(object sender, MouseEventArgs e)
        {
            // Log the current window location and the mouse location.
            if (e.Button == MouseButtons.Right)
            {
                windowTop = this.Top;
                windowLeft = this.Left;
                mouseX = e.X;
                mouseY = e.Y;
            }
        }

        private void mouseMove_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                windowTop = this.Top;
                windowLeft = this.Left;

                Point pos = new Point(0, 0);

                pos.X = windowLeft + e.X - mouseX;
                pos.Y = windowTop + e.Y - mouseY;
                this.Location = pos;
            }
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void UpdateForm()
        {
            double Pressure = mf.ModulesStatus.Pressure(mf.Products.Items[mf.CurrentProduct()].ModuleID);
            if (mf.PressureCal > 0) Pressure = Pressure / mf.PressureCal;
            Pressure += mf.PressureOffset;
            lbPressureValue.Text = Pressure.ToString("N1");
        }
    }
}