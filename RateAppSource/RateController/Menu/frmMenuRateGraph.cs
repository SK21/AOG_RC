using RateController.Language;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace RateController.Menu
{
    public partial class frmMenuRateGraph : Form
    {
        private string dataPWM = "-1";
        private string dataSteerAngle = "0";
        private bool isAuto = false;
        private FormStart mf;
        private clsProduct Prod;

        public frmMenuRateGraph(FormStart CallingForm)
        {
            InitializeComponent();
            mf = CallingForm;
            Prod = mf.Products.Item(mf.CurrentProduct());

            #region // language

            this.lbSetPoint.Text = Lang.lgUPMTarget;
            this.lbActual.Text = Lang.lgUPMApplied;
            this.Text = Lang.lgPIDTune;
            lbError.Text = Lang.lgError;

            #endregion // language
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
            dataSteerAngle = Prod.UPMapplied().ToString(CultureInfo.InvariantCulture);
            dataPWM = Prod.TargetUPM().ToString(CultureInfo.InvariantCulture);

            lblSteerAng.Text = Prod.UPMapplied().ToString("N1");
            lblPWM.Text = Prod.TargetUPM().ToString("N1");
            lbPWMvalue.Text = Prod.PWM().ToString("N0");
            lbErrorValue.Text = ErrorPercent().ToString("N1");

            //chart data
            Series s = unoChart.Series["S"];
            Series w = unoChart.Series["PWM"];
            double nextX = 1;
            double nextX5 = 1;

            if (s.Points.Count > 0) nextX = s.Points[s.Points.Count - 1].XValue + 1;
            if (w.Points.Count > 0) nextX5 = w.Points[w.Points.Count - 1].XValue + 1;

            unoChart.Series["S"].Points.AddXY(nextX, dataSteerAngle);
            unoChart.Series["PWM"].Points.AddXY(nextX5, dataPWM);

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

        private double ErrorPercent()
        {
            double Target = Prod.TargetUPM();
            double Applied = Prod.UPMapplied();
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

        private void frmMenuRateGraph_FormClosed(object sender, FormClosedEventArgs e)
        {
            mf.Tls.SaveFormData(this);
        }

        private void frmMenuRateGraph_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);

            timer1.Interval = (int)((1 / 20.0) * 1000);

            unoChart.ChartAreas[0].AxisY.Minimum = -1000;
            unoChart.ChartAreas[0].AxisY.Maximum = 1000;
            unoChart.ResetAutoValues();

            lblMax.Text = ((int)(unoChart.ChartAreas[0].AxisY.Maximum * 0.01)).ToString();
            lblMin.Text = ((int)(unoChart.ChartAreas[0].AxisY.Minimum * 0.01)).ToString();

            btnGainAuto.PerformClick();
            SetLanguage();
        }

        private void SetLanguage()
        {
            lbActual.Text = Lang.lgActual;
            lbSetPoint.Text = Lang.lgSetPoint;
            lbError.Text = Lang.lgUPMerror;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DrawChart();
        }
    }
}