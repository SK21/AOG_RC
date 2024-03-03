using System;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace RateController
{
    public partial class FormPIDGraph : Form
    {
        private readonly FormSettings mf = null;

        private string dataPWM = "-1";

        //chart data
        private string dataSteerAngle = "0";

        private bool isAuto = false;

        public FormPIDGraph(Form callingForm)
        {
            mf = callingForm as FormSettings;
            InitializeComponent();
            #region // language
            this.label5.Text = Lang.lgUPMTarget;
            this.label1.Text = Lang.lgUPMApplied;
            this.Text = Lang.lgPIDTune;
            label7.Text = Lang.lgError;
            #endregion
        }

        private void btnGainAuto_Click(object sender, EventArgs e)
        {
            unoChart.ChartAreas[0].AxisY.Maximum = Double.NaN;
            unoChart.ChartAreas[0].AxisY.Minimum = Double.NaN;
            unoChart.ChartAreas[0].RecalculateAxesScale();
            unoChart.ResetAutoValues();
            lblMax.Text = "Auto";
            lblMin.Text = "";
            isAuto = true;
        }

        private void btnGainDown_Click(object sender, EventArgs e)
        {
            if (isAuto)
            {
                unoChart.ChartAreas[0].AxisY.Minimum = -1000;
                unoChart.ChartAreas[0].AxisY.Maximum = 1000;
                unoChart.ResetAutoValues();
                lblMax.Text = ((int)(unoChart.ChartAreas[0].AxisY.Maximum * 0.01)).ToString();
                lblMin.Text = ((int)(unoChart.ChartAreas[0].AxisY.Minimum * 0.01)).ToString();
                isAuto = false;
                return;
            }

            if (unoChart.ChartAreas[0].AxisY.Minimum >= -200)
            {
                unoChart.ChartAreas[0].AxisY.Minimum = -200;
                unoChart.ChartAreas[0].AxisY.Maximum = 200;
                unoChart.ResetAutoValues();
                lblMax.Text = ((int)(unoChart.ChartAreas[0].AxisY.Maximum * 0.01)).ToString();
                lblMin.Text = ((int)(unoChart.ChartAreas[0].AxisY.Minimum * 0.01)).ToString();
                return;
            }

            unoChart.ChartAreas[0].AxisY.Minimum *= 0.66666;
            unoChart.ChartAreas[0].AxisY.Maximum *= 0.66666;
            unoChart.ChartAreas[0].AxisY.Minimum = (int)unoChart.ChartAreas[0].AxisY.Minimum;
            unoChart.ChartAreas[0].AxisY.Maximum = (int)unoChart.ChartAreas[0].AxisY.Maximum;
            unoChart.ResetAutoValues();
            lblMax.Text = ((int)(unoChart.ChartAreas[0].AxisY.Maximum * 0.01)).ToString();
            lblMin.Text = ((int)(unoChart.ChartAreas[0].AxisY.Minimum * 0.01)).ToString();
        }

        private void btnGainUp_Click(object sender, EventArgs e)
        {
            if (isAuto)
            {
                unoChart.ChartAreas[0].AxisY.Minimum = -1000;
                unoChart.ChartAreas[0].AxisY.Maximum = 1000;
                unoChart.ResetAutoValues();
                lblMax.Text = ((int)(unoChart.ChartAreas[0].AxisY.Maximum * 0.01)).ToString();
                lblMin.Text = ((int)(unoChart.ChartAreas[0].AxisY.Minimum * 0.01)).ToString();
                isAuto = false;
                return;
            }

            unoChart.ChartAreas[0].AxisY.Minimum *= 1.5;
            unoChart.ChartAreas[0].AxisY.Maximum *= 1.5;
            unoChart.ChartAreas[0].AxisY.Minimum = (int)unoChart.ChartAreas[0].AxisY.Minimum;
            unoChart.ChartAreas[0].AxisY.Maximum = (int)unoChart.ChartAreas[0].AxisY.Maximum;
            unoChart.ResetAutoValues();
            lblMax.Text = ((int)(unoChart.ChartAreas[0].AxisY.Maximum * 0.01)).ToString();
            lblMin.Text = ((int)(unoChart.ChartAreas[0].AxisY.Minimum * 0.01)).ToString();
        }

        private void DrawChart()
        {
            {
                //word 0 - steerangle, 1 - pwmDisplay
                dataSteerAngle = mf.CurrentProduct.UPMapplied().ToString(CultureInfo.InvariantCulture);
                dataPWM = mf.CurrentProduct.TargetUPM().ToString(CultureInfo.InvariantCulture);

                lblSteerAng.Text = mf.CurrentProduct.UPMapplied().ToString("N1");
                lblPWM.Text = mf.CurrentProduct.TargetUPM().ToString("N1");
                lbPWMvalue.Text = mf.CurrentProduct.PWM().ToString("N0");
                lbErrorValue.Text = ErrorPercent().ToString("N1");
            }

            //chart data
            Series s = unoChart.Series["S"];
            Series w = unoChart.Series["PWM"];
            double nextX = 1;
            double nextX5 = 1;

            if (s.Points.Count > 0) nextX = s.Points[s.Points.Count - 1].XValue + 1;
            if (w.Points.Count > 0) nextX5 = w.Points[w.Points.Count - 1].XValue + 1;

            unoChart.Series["S"].Points.AddXY(nextX, dataSteerAngle);
            unoChart.Series["PWM"].Points.AddXY(nextX5, dataPWM);

            //if (isScroll)
            {
                while (s.Points.Count > 30)
                {
                    s.Points.RemoveAt(0);
                }
                while (w.Points.Count > 30)
                {
                    w.Points.RemoveAt(0);
                }
                unoChart.ResetAutoValues();
            }
        }

        private double ErrorPercent()
        {
            double Target = mf.CurrentProduct.TargetUPM();
            double Applied = mf.CurrentProduct.UPMapplied();
            double RateError = 0;
            if (Target > 0)
            {
                RateError = ((Applied - Target) / Target) * 100;
                bool IsNegative = RateError < 0;
                RateError = Math.Abs(RateError);
                if (RateError > 100) RateError = 100;
                if (IsNegative) RateError *= -1;
            }
            return RateError;
        }

        private void FormPIDGraph_FormClosed(object sender, FormClosedEventArgs e)
        {
                mf.mf.Tls.SaveFormData(this);
        }

        private void FormSteerGraph_Load(object sender, EventArgs e)
        {
            mf.mf.Tls.LoadFormData(this);

            timer1.Interval = (int)((1 / 20.0) * 1000);

            unoChart.ChartAreas[0].AxisY.Minimum = -1000;
            unoChart.ChartAreas[0].AxisY.Maximum = 1000;
            unoChart.ResetAutoValues();

            lblMax.Text = ((int)(unoChart.ChartAreas[0].AxisY.Maximum * 0.01)).ToString();
            lblMin.Text = ((int)(unoChart.ChartAreas[0].AxisY.Minimum * 0.01)).ToString();

            btnGainAuto.PerformClick();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DrawChart();
        }
    }
}