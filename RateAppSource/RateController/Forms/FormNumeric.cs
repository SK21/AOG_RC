using System;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace AgOpenGPS
{
    public partial class FormNumeric : Form
    {
        private readonly double max;
        private readonly double min;
        private bool isFirstKey;

        public bool IsBlank { get; private set; }   // <-- added

        public FormNumeric(double _min, double _max, double currentValue)
        {
            max = _max;
            min = _min;
            InitializeComponent();

            this.Text = "Enter a Value";
            tboxNumber.Text = currentValue.ToString();

            isFirstKey = true;
            IsBlank = false;                        // <-- initialize
        }

        public double ReturnValue { get; set; }

        private void BtnDistanceDn_MouseDown(object sender, MouseEventArgs e)
        {
            if (tboxNumber.Text == "" || tboxNumber.Text == "-" || tboxNumber.Text == "Error ")
                tboxNumber.Text = "0";
            double tryNumber = double.Parse(tboxNumber.Text, CultureInfo.CurrentCulture);

            tryNumber--;
            if (tryNumber < min) tryNumber = min;

            tboxNumber.Text = tryNumber.ToString();
            isFirstKey = false;
        }

        private void BtnDistanceUp_MouseDown(object sender, MouseEventArgs e)
        {
            if (tboxNumber.Text == "" || tboxNumber.Text == "-" || tboxNumber.Text == "Error ")
                tboxNumber.Text = "0";
            double tryNumber = double.Parse(tboxNumber.Text, CultureInfo.CurrentCulture);

            tryNumber++;
            if (tryNumber > max) tryNumber = max;

            tboxNumber.Text = tryNumber.ToString();
            isFirstKey = false;
        }

        private void FormNumeric_Load(object sender, EventArgs e)
        {
            lblMax.Text = max.ToString();
            lblMin.Text = min.ToString();
            tboxNumber.SelectionStart = tboxNumber.Text.Length;
            tboxNumber.SelectionLength = 0;
            keypad1.Focus();
        }

        private void RegisterKeypad1_ButtonPressed(object sender, KeyPressEventArgs e)
        {
            if (isFirstKey && (e.KeyChar != 'K'))
            {
                tboxNumber.Text = "";
                isFirstKey = false;
            }

            if (tboxNumber.Text == "Number required.")
            {
                tboxNumber.Text = "";
                lblMin.ForeColor = SystemColors.ControlText;
                lblMax.ForeColor = SystemColors.ControlText;
            }

            if (Char.IsNumber(e.KeyChar))
            {
                tboxNumber.Text += e.KeyChar;
            }
            else if (e.KeyChar == 'B')
            {
                if (tboxNumber.Text.Length > 0)
                    tboxNumber.Text = tboxNumber.Text.Remove(tboxNumber.Text.Length - 1);
            }
            else if (e.KeyChar == '.')
            {
                if (!tboxNumber.Text.Contains(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator))
                {
                    tboxNumber.Text += Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                    if (tboxNumber.Text.IndexOf(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator) == 0)
                        tboxNumber.Text = "0" + tboxNumber.Text;
                    if (tboxNumber.Text.IndexOf("-") == 0 && tboxNumber.Text.IndexOf(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator) == 1)
                        tboxNumber.Text = "-0" + Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                }
            }
            else if (e.KeyChar == '-')
            {
                if (!tboxNumber.Text.Contains("-"))
                    tboxNumber.Text = "-" + tboxNumber.Text;
                else if (tboxNumber.Text.StartsWith("-"))
                    tboxNumber.Text = tboxNumber.Text.Substring(1);
            }
            else if (e.KeyChar == 'X')
            {
                this.DialogResult = DialogResult.Cancel;
                Close();
            }
            else if (e.KeyChar == 'C')
            {
                tboxNumber.Text = "";
            }
            else if (e.KeyChar == 'K')
            {
                // Allow blank: treat as “unset”
                if (tboxNumber.Text == "")
                {
                    IsBlank = true;
                    this.DialogResult = DialogResult.OK;
                    Close();
                    return;
                }

                double tryNumber = double.Parse(tboxNumber.Text, CultureInfo.CurrentCulture);

                if (tryNumber < min)
                {
                    tboxNumber.Text = "Error";
                    lblMin.ForeColor = Color.Red;
                }
                else if (tryNumber > max)
                {
                    tboxNumber.Text = "Error";
                    lblMax.ForeColor = Color.Red;
                }
                else
                {
                    ReturnValue = tryNumber;
                    this.DialogResult = DialogResult.OK;
                    Close();
                }
            }
        }
    }
}