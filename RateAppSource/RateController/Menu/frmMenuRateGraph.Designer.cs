namespace RateController.Menu
{
    partial class frmMenuRateGraph
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMenuRateGraph));
            this.unoChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label2 = new System.Windows.Forms.Label();
            this.lbPWMvalue = new System.Windows.Forms.Label();
            this.lbErrorValue = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lbError = new System.Windows.Forms.Label();
            this.lblMin = new System.Windows.Forms.Label();
            this.lblPWM = new System.Windows.Forms.Label();
            this.lblSteerAng = new System.Windows.Forms.Label();
            this.lbSetPoint = new System.Windows.Forms.Label();
            this.lbActual = new System.Windows.Forms.Label();
            this.btnGainAuto = new System.Windows.Forms.Button();
            this.btnGainDown = new System.Windows.Forms.Button();
            this.btnGainUp = new System.Windows.Forms.Button();
            this.lblMax = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.lbHz = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.unoChart)).BeginInit();
            this.SuspendLayout();
            // 
            // unoChart
            // 
            this.unoChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.unoChart.AntiAliasing = System.Windows.Forms.DataVisualization.Charting.AntiAliasingStyles.None;
            this.unoChart.BackColor = System.Drawing.Color.Black;
            chartArea1.AxisX.LabelAutoFitMaxFontSize = 8;
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.DimGray;
            chartArea1.AxisY.LineWidth = 2;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.DimGray;
            chartArea1.BackColor = System.Drawing.Color.Black;
            chartArea1.BorderWidth = 0;
            chartArea1.Name = "ChartArea1";
            chartArea1.Position.Auto = false;
            chartArea1.Position.Height = 100F;
            chartArea1.Position.Width = 100F;
            this.unoChart.ChartAreas.Add(chartArea1);
            this.unoChart.Location = new System.Drawing.Point(3, 12);
            this.unoChart.Margin = new System.Windows.Forms.Padding(0);
            this.unoChart.Name = "unoChart";
            this.unoChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.None;
            series1.BackSecondaryColor = System.Drawing.Color.White;
            series1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            series1.BorderWidth = 2;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series1.Color = System.Drawing.Color.OrangeRed;
            series1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            series1.Legend = "Legend1";
            series1.Name = "S";
            series2.BorderWidth = 2;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series2.Color = System.Drawing.Color.Lime;
            series2.IsVisibleInLegend = false;
            series2.Legend = "Legend1";
            series2.Name = "PWM";
            this.unoChart.Series.Add(series1);
            this.unoChart.Series.Add(series2);
            this.unoChart.Size = new System.Drawing.Size(481, 304);
            this.unoChart.TabIndex = 181;
            this.unoChart.TextAntiAliasingQuality = System.Windows.Forms.DataVisualization.Charting.TextAntiAliasingQuality.SystemDefault;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.ControlText;
            this.label2.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Lime;
            this.label2.Location = new System.Drawing.Point(472, 152);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 33);
            this.label2.TabIndex = 204;
            this.label2.Text = "<";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbPWMvalue
            // 
            this.lbPWMvalue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbPWMvalue.AutoSize = true;
            this.lbPWMvalue.BackColor = System.Drawing.SystemColors.ControlText;
            this.lbPWMvalue.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPWMvalue.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lbPWMvalue.Location = new System.Drawing.Point(362, 339);
            this.lbPWMvalue.Name = "lbPWMvalue";
            this.lbPWMvalue.Size = new System.Drawing.Size(68, 23);
            this.lbPWMvalue.TabIndex = 218;
            this.lbPWMvalue.Text = "label5";
            this.lbPWMvalue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbErrorValue
            // 
            this.lbErrorValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbErrorValue.AutoSize = true;
            this.lbErrorValue.BackColor = System.Drawing.SystemColors.ControlText;
            this.lbErrorValue.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbErrorValue.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lbErrorValue.Location = new System.Drawing.Point(362, 316);
            this.lbErrorValue.Name = "lbErrorValue";
            this.lbErrorValue.Size = new System.Drawing.Size(68, 23);
            this.lbErrorValue.TabIndex = 217;
            this.lbErrorValue.Text = "label1";
            this.lbErrorValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.AutoEllipsis = true;
            this.label6.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label6.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label6.Location = new System.Drawing.Point(230, 339);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(102, 23);
            this.label6.TabIndex = 220;
            this.label6.Text = "PWM";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbError
            // 
            this.lbError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbError.AutoEllipsis = true;
            this.lbError.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lbError.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbError.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lbError.Location = new System.Drawing.Point(231, 316);
            this.lbError.Name = "lbError";
            this.lbError.Size = new System.Drawing.Size(125, 23);
            this.lbError.TabIndex = 219;
            this.lbError.Text = "Error %";
            this.lbError.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblMin
            // 
            this.lblMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMin.BackColor = System.Drawing.SystemColors.ControlText;
            this.lblMin.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMin.ForeColor = System.Drawing.Color.White;
            this.lblMin.Location = new System.Drawing.Point(436, 316);
            this.lblMin.Name = "lblMin";
            this.lblMin.Size = new System.Drawing.Size(53, 23);
            this.lblMin.TabIndex = 216;
            this.lblMin.Text = "10";
            this.lblMin.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblPWM
            // 
            this.lblPWM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblPWM.AutoSize = true;
            this.lblPWM.BackColor = System.Drawing.SystemColors.ControlText;
            this.lblPWM.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPWM.ForeColor = System.Drawing.Color.Lime;
            this.lblPWM.Location = new System.Drawing.Point(156, 339);
            this.lblPWM.Name = "lblPWM";
            this.lblPWM.Size = new System.Drawing.Size(68, 23);
            this.lblPWM.TabIndex = 213;
            this.lblPWM.Text = "label5";
            this.lblPWM.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSteerAng
            // 
            this.lblSteerAng.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSteerAng.AutoSize = true;
            this.lblSteerAng.BackColor = System.Drawing.SystemColors.ControlText;
            this.lblSteerAng.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSteerAng.ForeColor = System.Drawing.Color.OrangeRed;
            this.lblSteerAng.Location = new System.Drawing.Point(156, 316);
            this.lblSteerAng.Name = "lblSteerAng";
            this.lblSteerAng.Size = new System.Drawing.Size(68, 23);
            this.lblSteerAng.TabIndex = 212;
            this.lblSteerAng.Text = "label1";
            this.lblSteerAng.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbSetPoint
            // 
            this.lbSetPoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbSetPoint.AutoEllipsis = true;
            this.lbSetPoint.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lbSetPoint.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSetPoint.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lbSetPoint.Location = new System.Drawing.Point(70, 339);
            this.lbSetPoint.Name = "lbSetPoint";
            this.lbSetPoint.Size = new System.Drawing.Size(80, 23);
            this.lbSetPoint.TabIndex = 215;
            this.lbSetPoint.Text = "Set";
            this.lbSetPoint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbActual
            // 
            this.lbActual.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbActual.AutoEllipsis = true;
            this.lbActual.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lbActual.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbActual.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lbActual.Location = new System.Drawing.Point(66, 316);
            this.lbActual.Name = "lbActual";
            this.lbActual.Size = new System.Drawing.Size(84, 23);
            this.lbActual.TabIndex = 214;
            this.lbActual.Text = "Actual";
            this.lbActual.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnGainAuto
            // 
            this.btnGainAuto.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnGainAuto.BackColor = System.Drawing.Color.Transparent;
            this.btnGainAuto.FlatAppearance.BorderSize = 2;
            this.btnGainAuto.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGainAuto.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGainAuto.ForeColor = System.Drawing.Color.White;
            this.btnGainAuto.Location = new System.Drawing.Point(12, 129);
            this.btnGainAuto.Name = "btnGainAuto";
            this.btnGainAuto.Size = new System.Drawing.Size(48, 55);
            this.btnGainAuto.TabIndex = 223;
            this.btnGainAuto.Text = "A";
            this.btnGainAuto.UseVisualStyleBackColor = false;
            this.btnGainAuto.Click += new System.EventHandler(this.btnGainAuto_Click);
            // 
            // btnGainDown
            // 
            this.btnGainDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGainDown.BackColor = System.Drawing.Color.Transparent;
            this.btnGainDown.FlatAppearance.BorderSize = 2;
            this.btnGainDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGainDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGainDown.ForeColor = System.Drawing.Color.White;
            this.btnGainDown.Location = new System.Drawing.Point(12, 236);
            this.btnGainDown.Name = "btnGainDown";
            this.btnGainDown.Size = new System.Drawing.Size(48, 55);
            this.btnGainDown.TabIndex = 222;
            this.btnGainDown.Text = "-";
            this.btnGainDown.UseVisualStyleBackColor = false;
            this.btnGainDown.Click += new System.EventHandler(this.btnGainDown_Click);
            // 
            // btnGainUp
            // 
            this.btnGainUp.BackColor = System.Drawing.Color.Transparent;
            this.btnGainUp.FlatAppearance.BorderSize = 2;
            this.btnGainUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGainUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGainUp.ForeColor = System.Drawing.Color.White;
            this.btnGainUp.Location = new System.Drawing.Point(12, 12);
            this.btnGainUp.Name = "btnGainUp";
            this.btnGainUp.Size = new System.Drawing.Size(48, 55);
            this.btnGainUp.TabIndex = 221;
            this.btnGainUp.Text = "+";
            this.btnGainUp.UseVisualStyleBackColor = false;
            this.btnGainUp.Click += new System.EventHandler(this.btnGainUp_Click);
            // 
            // lblMax
            // 
            this.lblMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMax.BackColor = System.Drawing.SystemColors.ControlText;
            this.lblMax.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMax.ForeColor = System.Drawing.Color.White;
            this.lblMax.Location = new System.Drawing.Point(432, 1);
            this.lblMax.Name = "lblMax";
            this.lblMax.Size = new System.Drawing.Size(57, 23);
            this.lblMax.TabIndex = 224;
            this.lblMax.Text = "Auto";
            this.lblMax.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoEllipsis = true;
            this.label1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label1.Location = new System.Drawing.Point(12, 316);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 23);
            this.label1.TabIndex = 225;
            this.label1.Text = "Hz";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbHz
            // 
            this.lbHz.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbHz.AutoEllipsis = true;
            this.lbHz.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lbHz.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbHz.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lbHz.Location = new System.Drawing.Point(12, 339);
            this.lbHz.Name = "lbHz";
            this.lbHz.Size = new System.Drawing.Size(52, 23);
            this.lbHz.TabIndex = 226;
            this.lbHz.Text = "1000";
            this.lbHz.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frmMenuRateGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(498, 369);
            this.Controls.Add(this.lbHz);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblMax);
            this.Controls.Add(this.btnGainAuto);
            this.Controls.Add(this.btnGainDown);
            this.Controls.Add(this.btnGainUp);
            this.Controls.Add(this.lbPWMvalue);
            this.Controls.Add(this.lbErrorValue);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lbError);
            this.Controls.Add(this.lblMin);
            this.Controls.Add(this.lblPWM);
            this.Controls.Add(this.lblSteerAng);
            this.Controls.Add(this.lbSetPoint);
            this.Controls.Add(this.lbActual);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.unoChart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMenuRateGraph";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuRateGraph";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMenuRateGraph_FormClosed);
            this.Load += new System.EventHandler(this.frmMenuRateGraph_Load);
            ((System.ComponentModel.ISupportInitialize)(this.unoChart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart unoChart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbPWMvalue;
        private System.Windows.Forms.Label lbErrorValue;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lbError;
        private System.Windows.Forms.Label lblMin;
        private System.Windows.Forms.Label lblPWM;
        private System.Windows.Forms.Label lblSteerAng;
        private System.Windows.Forms.Label lbSetPoint;
        private System.Windows.Forms.Label lbActual;
        private System.Windows.Forms.Button btnGainAuto;
        private System.Windows.Forms.Button btnGainDown;
        private System.Windows.Forms.Button btnGainUp;
        private System.Windows.Forms.Label lblMax;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbHz;
    }
}