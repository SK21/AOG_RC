using AgOpenGPS;
using RateController.Classes;
using System;
using System.Windows.Forms;

namespace RateController.Forms
{
    public partial class frmSpeedCal : Form
    {
        private double cCalNumber = 0;
        private bool cCanceled = true;
        private UInt32 Counts;
        private bool DistanceChanged = false;
        private double DistanceToTravel;
        private UInt32 LastCounts;
        private FormStart mf;

        public frmSpeedCal(FormStart CalledFrom)
        {
            InitializeComponent();
            mf = CalledFrom;
        }

        public double CalNumber
        { get { return cCalNumber; } }

        public bool Canceled
        { get { return cCanceled; } }

        private void btnCalStart_Click(object sender, EventArgs e)
        {
            mf.WheelSpeed.EraseCounts = true;
            mf.WheelSpeed.Send();
            timer1.Enabled = true;
            btnCalStart.Enabled = false;
            btnCalStop.Enabled = true;
        }

        private void btnCalStop_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            btnCalStart.Enabled = true;
            btnCalStop.Enabled = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            cCanceled = true;
            timer1.Enabled = false;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Make sure latest values are computed before returning
            UpdateForm();
            cCanceled = false;
            timer1.Enabled = false;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void frmSpeedCal_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SetProp("SpeedCalDistance", DistanceToTravel.ToString());
            Props.SaveFormLocation(this);
        }

        private void frmSpeedCal_Load(object sender, EventArgs e)
        {
            Props.LoadFormLocation(this);
            this.BackColor = Properties.Settings.Default.MainBackColour;
            if (double.TryParse(Props.GetProp("SpeedCalDistance"), out double di)) DistanceToTravel = di;

            // Make Enter/ESC map to OK/Cancel
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;

            if (Props.UseMetric)
            {
                lbDistanceUnits.Text = "(meters)";
                lbCalUnits.Text = "(pulses/km)";
            }
            else
            {
                lbDistanceUnits.Text = "(feet)";
                lbCalUnits.Text = "(pulses/mile)";
            }

            UpdateForm();
        }

        private void tbDistance_Enter(object sender, EventArgs e)
        {
            int tempInt;
            int.TryParse(tbDistance.Text, out tempInt);
            using (var form = new FormNumeric(0, 1000, tempInt))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbDistance.Text = form.ReturnValue.ToString();
                }
            }
            UpdateForm();
            btnOK.Focus();
        }

        private void tbDistance_TextChanged(object sender, EventArgs e)
        {
            if (double.TryParse(tbDistance.Text, out double di)) DistanceToTravel = di;
            DistanceChanged = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void UpdateForm()
        {
            tbDistance.Text = DistanceToTravel.ToString("N1");

            if (timer1.Enabled)
            {
                Counts = mf.ModulesStatus.WheelCounts(mf.WheelSpeed.WheelModule);
                lbPulses.Text = Counts.ToString("N0");
            }

            if (Counts != LastCounts || DistanceChanged)
            {
                LastCounts = Counts;
                DistanceChanged = false;
                cCalNumber = 0;
                if (Props.UseMetric)
                {
                    if (DistanceToTravel > 0) cCalNumber = (Counts / DistanceToTravel) * 1000.0;
                }
                else
                {
                    if (DistanceToTravel > 0) cCalNumber = (Counts / DistanceToTravel) * 5280.0;
                }
                lbWheelCal.Text = CalNumber.ToString("N0");
            }
        }
    }
}