using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmScaleDisplay : Form
    {
        private int cDisplayMode = 0;
        private int cProductID;
        private bool IsTransparent;
        private FormStart mf;
        private int mouseX = 0;
        private int mouseY = 0;
        private double StartingAcres = 0;
        private double StartingWeight = 0;
        private double TareWeight = 0;
        private int TransLeftOffset = 6;
        private int TransTopOffset = 30;
        private int windowLeft = 0;
        private int windowTop = 0;
        // Display mode: 0 - weight, 1 - applied, 2 - acres, 3 - rate

        public frmScaleDisplay(FormStart CallingForm, int ProductID)
        {
            InitializeComponent();
            mf = CallingForm;
            this.BackColor = Properties.Settings.Default.DayColour;
            pictureBox1.BackColor = Properties.Settings.Default.DayColour;
            lbValue.BackColor = Properties.Settings.Default.DayColour;
            cProductID = ProductID;
            this.Text = "Scale " + Prd();
        }

        private void btnScale_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Click to cycle through display.\nCurrent weight, Applied weight, Area, and Rate.";

            mf.Tls.ShowHelp(Message);
            hlpevent.Handled = true;
        }

        private double CurrentAcres()
        {
            double Area = mf.Products.Item(cProductID).CurrentCoverage();
            return Area;
        }

        private void frmScaleDisplay_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (mf.UseTransparent)
            {
                // move the window back to the default location
                this.Top += -TransTopOffset;
                this.Left += -TransLeftOffset;
            }
            mf.Tls.SaveFormData(this, cProductID.ToString());
            timer1.Enabled = false;
            SaveData();
        }

        private void frmScaleDisplay_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this, cProductID.ToString());
            timer1.Enabled = true;
            LoadData();
            UpdateForm();
        }

        private void lbValue_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Click to reset starting weight and acres.\nRight click to reset tare weight.";

            mf.Tls.ShowHelp(Message);
            hlpevent.Handled = true;
        }

        private void lbValue_MouseClick(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    var Hlp = new frmMsgBox(mf, "Reset starting weight and acres?", "Reset", true);
                    Hlp.TopMost = true;

                    Hlp.ShowDialog();
                    bool Result = Hlp.Result;
                    Hlp.Close();
                    if (Result)
                    {
                        StartingAcres = CurrentAcres();
                        StartingWeight = NetWeight();
                    }
                    break;

                case MouseButtons.Right:
                    var Hlp2 = new frmMsgBox(mf, "Reset tare weight?", "Reset", true);
                    Hlp2.TopMost = true;

                    Hlp2.ShowDialog();
                    bool Result2 = Hlp2.Result;
                    Hlp2.Close();
                    if (Result2)
                    {
                        TareWeight = mf.ScaleIndicator.Value(cProductID);
                    }
                    break;
            }
        }

        private void LoadData()
        {
            if (int.TryParse(mf.Tls.LoadProperty("Scale_DisplayMode" + cProductID.ToString()), out int md)) cDisplayMode = md;
            if (double.TryParse(mf.Tls.LoadProperty("Scale_Area" + cProductID.ToString()), out double ar)) StartingAcres = ar;
            if (double.TryParse(mf.Tls.LoadProperty("Scale_Weight" + cProductID.ToString()), out double wt)) StartingWeight = wt;
            if (double.TryParse(mf.Tls.LoadProperty("Scale_Tare" + cProductID.ToString()), out double ta)) TareWeight = ta;
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

        private double NetWeight()
        {
            double wt = mf.ScaleIndicator.Value(cProductID) - TareWeight;
            return wt;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    cDisplayMode++;
                    if (cDisplayMode > 3) cDisplayMode = 0;
                    UpdateForm();
                    break;

                case MouseButtons.Right:
                    break;
            }
        }

        private string Prd()
        {
            return (cProductID + 1).ToString();
        }

        private void SaveData()
        {
            mf.Tls.SaveProperty("Scale_DisplayMode" + cProductID.ToString(), cDisplayMode.ToString());
            mf.Tls.SaveProperty("Scale_Area" + cProductID.ToString(), StartingAcres.ToString());
            mf.Tls.SaveProperty("Scale_Weight" + cProductID.ToString(), StartingWeight.ToString());
            mf.Tls.SaveProperty("Scale_Tare" + cProductID.ToString(), TareWeight.ToString());
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

            switch (cDisplayMode)
            {
                case 1:
                    // applied
                    lbValue.Text = (StartingWeight - NetWeight()).ToString("N1") + "\n(Applied " + Prd() + ")";
                    break;

                case 2:
                    // acres
                    lbValue.Text = (StartingAcres - CurrentAcres()).ToString("N1") + "\n(Area " + Prd() + ")";
                    break;

                case 3:
                    // rate
                    double Applied = StartingWeight - NetWeight();
                    double Area = StartingAcres - CurrentAcres();
                    double Rate = 0;
                    if (Area > 0)
                    {
                        Rate = Applied / Area;
                    }
                    lbValue.Text = Rate.ToString("N1") + "\n(Rate " + Prd() + ")";
                    break;

                default:
                    // weight
                    lbValue.Text = NetWeight().ToString("N1") + "\n(Weight " + Prd() + ")";
                    break;
            }
        }
    }
}