using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmPressureDisplay : Form
    {
        private bool IsTransparent;
        private FormStart mf;
        private int mouseX = 0;
        private int mouseY = 0;
        private int TransLeftOffset = 6;
        private int TransTopOffset = 30;
        private int windowLeft = 0;
        private int windowTop = 0;

        public frmPressureDisplay(FormStart CallingForm)
        {
            InitializeComponent();
            mf = CallingForm;
            this.BackColor = Properties.Settings.Default.DayColour;
            pictureBox1.BackColor = Properties.Settings.Default.DayColour;
            lbPressureValue.BackColor = Properties.Settings.Default.DayColour;
        }

        private void frmPressureDisplay_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (mf.UseTransparent)
            {
                // move the window back to the default location
                this.Top += -TransTopOffset;
                this.Left += -TransLeftOffset;
            }
                mf.Tls.SaveFormData(this);
            timer1.Enabled = false;
        }

        private void frmPressureDisplay_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            timer1.Enabled = true;
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

        private void SetFont()
        {
            if (mf.UseTransparent)
            {
                string TransparentFont = "MS Gothic";

                foreach (Control Ctrl in Controls)
                {
                    Ctrl.Font = new Font(TransparentFont, 18);
                }
            }
            else
            {
                foreach (Control Ctrl in Controls)
                {
                    Ctrl.Font = new Font("Tahoma", 14);
                }
            }
        }

        private void SetTransparent()
        {
            if (mf.UseTransparent)
            {
                this.TransparencyKey = (Properties.Settings.Default.IsDay) ? Properties.Settings.Default.DayColour : Properties.Settings.Default.NightColour;
                this.ControlBox = false;
                this.FormBorderStyle = FormBorderStyle.None;
                this.Top += TransTopOffset;
                this.Left += TransLeftOffset;
                IsTransparent = true;

                Color txtcolor = Color.Yellow;
                lbPressureValue.ForeColor = txtcolor;
            }
            else
            {
                this.Text = "Pressure";
                this.TransparencyKey = Color.Empty;
                this.ControlBox = true;
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.Top += -TransTopOffset;
                this.Left += -TransLeftOffset;
                IsTransparent = false;

                Color txtcolor = SystemColors.ControlText;
                lbPressureValue.ForeColor = txtcolor;
            }
            SetFont();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void UpdateForm()
        {
            if (mf.UseTransparent != IsTransparent) SetTransparent();
            if (mf.PressureToShow > 0)
            {
                float Prs = mf.PressureObjects.Item(mf.PressureToShow - 1).Pressure();
                lbPressureValue.Text = Prs.ToString("N1");
            }
        }

        private void lbPressureValue_Click(object sender, EventArgs e)
        {

        }
    }
}