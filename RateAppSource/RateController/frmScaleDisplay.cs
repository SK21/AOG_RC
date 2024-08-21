using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmScaleDisplay : Form
    {
        private int cDisplayMode = 0;
        private bool IsTransparent;
        private FormStart mf;
        private int mouseX = 0;
        private int mouseY = 0;
        private double StartingAcres = 0;
        private double StartingWeight = 0;
        private int TransLeftOffset = 6;
        private int TransTopOffset = 30;
        private int windowLeft = 0;
        private int windowTop = 0;
        // Display mode: 0 - weight, 1 - applied, 2 - acres, 3 - rate

        public frmScaleDisplay(FormStart CallingForm)
        {
            InitializeComponent();
            mf = CallingForm;
            this.BackColor = Properties.Settings.Default.DayColour;
            pictureBox1.BackColor = Properties.Settings.Default.DayColour;
            lbValue.BackColor = Properties.Settings.Default.DayColour;
        }

        private double CurrentAcres()
        {
            return 0;
        }

        private void frmScaleDisplay_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (mf.UseTransparent)
            {
                // move the window back to the default location
                this.Top += -TransTopOffset;
                this.Left += -TransLeftOffset;
            }
            mf.Tls.SaveFormData(this);
            timer1.Enabled = false;
            // to do, save starting area, weight, display mode
        }

        private void frmScaleDisplay_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            timer1.Enabled = true;
            UpdateForm();
            // to do, load starting area, weight, display mode
        }

        private void lbValue_Click(object sender, EventArgs e)
        {
            var Hlp = new frmMsgBox(mf, "Reset?", "Reset", true);
            Hlp.TopMost = true;

            Hlp.ShowDialog();
            bool Result = Hlp.Result;
            Hlp.Close();
            if (Result)
            {
                StartingAcres = CurrentAcres();
                StartingWeight = mf.ScaleIndicator.Value;
            }
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

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            cDisplayMode++;
            if (cDisplayMode > 3) cDisplayMode = 0;
            UpdateForm();
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
                lbValue.ForeColor = txtcolor;
            }
            else
            {
                this.Text = "Scale";
                this.TransparencyKey = Color.Empty;
                this.ControlBox = true;
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.Top += -TransTopOffset;
                this.Left += -TransLeftOffset;
                IsTransparent = false;

                Color txtcolor = SystemColors.ControlText;
                lbValue.ForeColor = txtcolor;
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
            double Weight = mf.ScaleIndicator.Value;

            switch (cDisplayMode)
            {
                case 1:
                    // applied
                    lbValue.Text = (StartingWeight - Weight).ToString("N1") + " (A)";
                    break;

                case 2:
                    // acres
                    lbValue.Text = (StartingAcres - CurrentAcres()).ToString("N1") + " (AR)";
                    break;

                case 3:
                    // rate
                    double Applied = StartingWeight - Weight;
                    double Area = StartingAcres - CurrentAcres();
                    double Rate = 0;
                    if (Area > 0)
                    {
                        Rate = Applied / Area;
                    }
                    lbValue.Text = Rate.ToString("N1") + " (R)";
                    break;

                default:
                    // weight
                    lbValue.Text = Weight.ToString("N1") + " (W)";
                    break;
            }
        }
    }
}