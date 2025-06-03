namespace RateController.Menu
{
    partial class frmMenuControl
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
            this.btnRight = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnPIDloadDefaults = new System.Windows.Forms.Button();
            this.lbBoost = new System.Windows.Forms.Label();
            this.lbMinValue = new System.Windows.Forms.Label();
            this.lbMaxValue = new System.Windows.Forms.Label();
            this.lbScaling = new System.Windows.Forms.Label();
            this.lbMax = new System.Windows.Forms.Label();
            this.lbMin = new System.Windows.Forms.Label();
            this.lbProduct = new System.Windows.Forms.Label();
            this.HSmin = new System.Windows.Forms.HScrollBar();
            this.HSmax = new System.Windows.Forms.HScrollBar();
            this.HSscaling = new System.Windows.Forms.HScrollBar();
            this.butGraph = new System.Windows.Forms.Button();
            this.HSintegral = new System.Windows.Forms.HScrollBar();
            this.lbIntegral = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lbGainAdjust = new System.Windows.Forms.Label();
            this.lbIntegralAdjust = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // btnRight
            // 
            this.btnRight.BackColor = System.Drawing.Color.Transparent;
            this.btnRight.FlatAppearance.BorderSize = 0;
            this.btnRight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRight.Image = global::RateController.Properties.Resources.ArrowRight1;
            this.btnRight.Location = new System.Drawing.Point(296, 603);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(70, 63);
            this.btnRight.TabIndex = 164;
            this.btnRight.UseVisualStyleBackColor = false;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnLeft
            // 
            this.btnLeft.BackColor = System.Drawing.Color.Transparent;
            this.btnLeft.FlatAppearance.BorderSize = 0;
            this.btnLeft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLeft.Image = global::RateController.Properties.Resources.ArrowLeft1;
            this.btnLeft.Location = new System.Drawing.Point(216, 603);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(70, 63);
            this.btnLeft.TabIndex = 163;
            this.btnLeft.UseVisualStyleBackColor = false;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnCancel.Enabled = false;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnCancel.Image = global::RateController.Properties.Resources.Cancel64;
            this.btnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCancel.Location = new System.Drawing.Point(376, 603);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(70, 63);
            this.btnCancel.TabIndex = 162;
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.Transparent;
            this.btnOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnOK.Enabled = false;
            this.btnOK.FlatAppearance.BorderSize = 0;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnOK.Image = global::RateController.Properties.Resources.Save;
            this.btnOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnOK.Location = new System.Drawing.Point(456, 603);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(70, 63);
            this.btnOK.TabIndex = 161;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnPIDloadDefaults
            // 
            this.btnPIDloadDefaults.BackColor = System.Drawing.Color.Transparent;
            this.btnPIDloadDefaults.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnPIDloadDefaults.FlatAppearance.BorderSize = 0;
            this.btnPIDloadDefaults.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPIDloadDefaults.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnPIDloadDefaults.Image = global::RateController.Properties.Resources.VehFileLoad;
            this.btnPIDloadDefaults.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnPIDloadDefaults.Location = new System.Drawing.Point(136, 603);
            this.btnPIDloadDefaults.Name = "btnPIDloadDefaults";
            this.btnPIDloadDefaults.Size = new System.Drawing.Size(70, 63);
            this.btnPIDloadDefaults.TabIndex = 165;
            this.btnPIDloadDefaults.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnPIDloadDefaults.UseVisualStyleBackColor = false;
            this.btnPIDloadDefaults.Click += new System.EventHandler(this.btnPIDloadDefaults_Click);
            // 
            // lbBoost
            // 
            this.lbBoost.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbBoost.Location = new System.Drawing.Point(418, 126);
            this.lbBoost.Name = "lbBoost";
            this.lbBoost.Size = new System.Drawing.Size(59, 23);
            this.lbBoost.TabIndex = 187;
            this.lbBoost.Text = "100";
            this.lbBoost.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbMinValue
            // 
            this.lbMinValue.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMinValue.Location = new System.Drawing.Point(418, 369);
            this.lbMinValue.Name = "lbMinValue";
            this.lbMinValue.Size = new System.Drawing.Size(59, 23);
            this.lbMinValue.TabIndex = 184;
            this.lbMinValue.Text = "100";
            this.lbMinValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbMaxValue
            // 
            this.lbMaxValue.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMaxValue.Location = new System.Drawing.Point(418, 288);
            this.lbMaxValue.Name = "lbMaxValue";
            this.lbMaxValue.Size = new System.Drawing.Size(59, 23);
            this.lbMaxValue.TabIndex = 183;
            this.lbMaxValue.Text = "100";
            this.lbMaxValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbScaling
            // 
            this.lbScaling.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbScaling.Location = new System.Drawing.Point(14, 115);
            this.lbScaling.Name = "lbScaling";
            this.lbScaling.Size = new System.Drawing.Size(133, 45);
            this.lbScaling.TabIndex = 185;
            this.lbScaling.Text = "Gain";
            this.lbScaling.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbMax
            // 
            this.lbMax.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMax.Location = new System.Drawing.Point(14, 277);
            this.lbMax.Name = "lbMax";
            this.lbMax.Size = new System.Drawing.Size(133, 45);
            this.lbMax.TabIndex = 172;
            this.lbMax.Text = "Max Power";
            this.lbMax.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbMin
            // 
            this.lbMin.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMin.Location = new System.Drawing.Point(14, 358);
            this.lbMin.Name = "lbMin";
            this.lbMin.Size = new System.Drawing.Size(133, 45);
            this.lbMin.TabIndex = 171;
            this.lbMin.Text = "Min Power";
            this.lbMin.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbProduct
            // 
            this.lbProduct.BackColor = System.Drawing.SystemColors.Control;
            this.lbProduct.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbProduct.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbProduct.Location = new System.Drawing.Point(73, 53);
            this.lbProduct.Name = "lbProduct";
            this.lbProduct.Size = new System.Drawing.Size(396, 28);
            this.lbProduct.TabIndex = 188;
            this.lbProduct.Text = "Product";
            this.lbProduct.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // HSmin
            // 
            this.HSmin.LargeChange = 1;
            this.HSmin.Location = new System.Drawing.Point(153, 358);
            this.HSmin.Name = "HSmin";
            this.HSmin.Size = new System.Drawing.Size(263, 45);
            this.HSmin.TabIndex = 333;
            this.HSmin.ValueChanged += new System.EventHandler(this.HShigh_ValueChanged);
            // 
            // HSmax
            // 
            this.HSmax.LargeChange = 1;
            this.HSmax.Location = new System.Drawing.Point(153, 277);
            this.HSmax.Name = "HSmax";
            this.HSmax.Size = new System.Drawing.Size(263, 45);
            this.HSmax.TabIndex = 334;
            this.HSmax.ValueChanged += new System.EventHandler(this.HShigh_ValueChanged);
            // 
            // HSscaling
            // 
            this.HSscaling.LargeChange = 1;
            this.HSscaling.Location = new System.Drawing.Point(153, 115);
            this.HSscaling.Name = "HSscaling";
            this.HSscaling.Size = new System.Drawing.Size(263, 45);
            this.HSscaling.TabIndex = 335;
            this.HSscaling.ValueChanged += new System.EventHandler(this.HShigh_ValueChanged);
            // 
            // butGraph
            // 
            this.butGraph.BackColor = System.Drawing.Color.Transparent;
            this.butGraph.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.butGraph.FlatAppearance.BorderSize = 0;
            this.butGraph.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butGraph.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.butGraph.Image = global::RateController.Properties.Resources.Chart;
            this.butGraph.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.butGraph.Location = new System.Drawing.Point(56, 603);
            this.butGraph.Margin = new System.Windows.Forms.Padding(6);
            this.butGraph.Name = "butGraph";
            this.butGraph.Size = new System.Drawing.Size(70, 63);
            this.butGraph.TabIndex = 339;
            this.butGraph.UseVisualStyleBackColor = false;
            this.butGraph.Click += new System.EventHandler(this.butGraph_Click);
            // 
            // HSintegral
            // 
            this.HSintegral.LargeChange = 1;
            this.HSintegral.Location = new System.Drawing.Point(153, 196);
            this.HSintegral.Name = "HSintegral";
            this.HSintegral.Size = new System.Drawing.Size(263, 45);
            this.HSintegral.TabIndex = 342;
            this.HSintegral.ValueChanged += new System.EventHandler(this.HShigh_ValueChanged);
            // 
            // lbIntegral
            // 
            this.lbIntegral.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbIntegral.Location = new System.Drawing.Point(418, 207);
            this.lbIntegral.Name = "lbIntegral";
            this.lbIntegral.Size = new System.Drawing.Size(59, 23);
            this.lbIntegral.TabIndex = 341;
            this.lbIntegral.Text = "100";
            this.lbIntegral.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(14, 196);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(133, 45);
            this.label2.TabIndex = 340;
            this.label2.Text = "Integral";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(117, 445);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(194, 45);
            this.label1.TabIndex = 343;
            this.label1.Text = "Gain PWM Adjust";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(113, 488);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(199, 45);
            this.label3.TabIndex = 344;
            this.label3.Text = "Integral PWM Adjust";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbGainAdjust
            // 
            this.lbGainAdjust.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbGainAdjust.Location = new System.Drawing.Point(317, 456);
            this.lbGainAdjust.Name = "lbGainAdjust";
            this.lbGainAdjust.Size = new System.Drawing.Size(59, 23);
            this.lbGainAdjust.TabIndex = 345;
            this.lbGainAdjust.Text = "100";
            this.lbGainAdjust.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbIntegralAdjust
            // 
            this.lbIntegralAdjust.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbIntegralAdjust.Location = new System.Drawing.Point(318, 499);
            this.lbIntegralAdjust.Name = "lbIntegralAdjust";
            this.lbIntegralAdjust.Size = new System.Drawing.Size(59, 23);
            this.lbIntegralAdjust.TabIndex = 346;
            this.lbIntegralAdjust.Text = "100";
            this.lbIntegralAdjust.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // frmMenuControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 678);
            this.Controls.Add(this.lbIntegralAdjust);
            this.Controls.Add(this.lbGainAdjust);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.HSintegral);
            this.Controls.Add(this.lbIntegral);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.butGraph);
            this.Controls.Add(this.HSscaling);
            this.Controls.Add(this.HSmax);
            this.Controls.Add(this.HSmin);
            this.Controls.Add(this.lbProduct);
            this.Controls.Add(this.lbBoost);
            this.Controls.Add(this.lbMinValue);
            this.Controls.Add(this.lbMaxValue);
            this.Controls.Add(this.lbScaling);
            this.Controls.Add(this.lbMax);
            this.Controls.Add(this.lbMin);
            this.Controls.Add(this.btnPIDloadDefaults);
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.btnLeft);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMenuControl";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuControl";
            this.Activated += new System.EventHandler(this.frmMenuControl_Activated);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMenuControl_FormClosed);
            this.Load += new System.EventHandler(this.frmMenuControl_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnPIDloadDefaults;
        private System.Windows.Forms.Label lbBoost;
        private System.Windows.Forms.Label lbMinValue;
        private System.Windows.Forms.Label lbMaxValue;
        private System.Windows.Forms.Label lbScaling;
        private System.Windows.Forms.Label lbMax;
        private System.Windows.Forms.Label lbMin;
        private System.Windows.Forms.Label lbProduct;
        private System.Windows.Forms.HScrollBar HSmin;
        private System.Windows.Forms.HScrollBar HSmax;
        private System.Windows.Forms.HScrollBar HSscaling;
        private System.Windows.Forms.Button butGraph;
        private System.Windows.Forms.HScrollBar HSintegral;
        private System.Windows.Forms.Label lbIntegral;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbGainAdjust;
        private System.Windows.Forms.Label lbIntegralAdjust;
        private System.Windows.Forms.Timer timer1;
    }
}