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
            this.btnRight = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnPIDloadDefaults = new System.Windows.Forms.Button();
            this.lbBoost = new System.Windows.Forms.Label();
            this.lbMinValue = new System.Windows.Forms.Label();
            this.lbMaxValue = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lbThresholdValue = new System.Windows.Forms.Label();
            this.lbLow = new System.Windows.Forms.Label();
            this.lbHigh = new System.Windows.Forms.Label();
            this.lbThreshold = new System.Windows.Forms.Label();
            this.lbRateLow = new System.Windows.Forms.Label();
            this.lbProportional = new System.Windows.Forms.Label();
            this.lbMax = new System.Windows.Forms.Label();
            this.lbMin = new System.Windows.Forms.Label();
            this.lbProduct = new System.Windows.Forms.Label();
            this.HSmin = new System.Windows.Forms.HScrollBar();
            this.HSmax = new System.Windows.Forms.HScrollBar();
            this.HSscaling = new System.Windows.Forms.HScrollBar();
            this.HSthreshold = new System.Windows.Forms.HScrollBar();
            this.HSlow = new System.Windows.Forms.HScrollBar();
            this.HShigh = new System.Windows.Forms.HScrollBar();
            this.butGraph = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnRight
            // 
            this.btnRight.BackColor = System.Drawing.Color.Transparent;
            this.btnRight.FlatAppearance.BorderSize = 0;
            this.btnRight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRight.Image = global::RateController.Properties.Resources.ArrowRight1;
            this.btnRight.Location = new System.Drawing.Point(300, 546);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(72, 72);
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
            this.btnLeft.Location = new System.Drawing.Point(222, 546);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(72, 72);
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
            this.btnCancel.Location = new System.Drawing.Point(378, 546);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 72);
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
            this.btnOK.Location = new System.Drawing.Point(456, 546);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(72, 72);
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
            this.btnPIDloadDefaults.Location = new System.Drawing.Point(144, 546);
            this.btnPIDloadDefaults.Name = "btnPIDloadDefaults";
            this.btnPIDloadDefaults.Size = new System.Drawing.Size(72, 72);
            this.btnPIDloadDefaults.TabIndex = 165;
            this.btnPIDloadDefaults.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnPIDloadDefaults.UseVisualStyleBackColor = false;
            this.btnPIDloadDefaults.Click += new System.EventHandler(this.btnPIDloadDefaults_Click);
            // 
            // lbBoost
            // 
            this.lbBoost.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbBoost.Location = new System.Drawing.Point(438, 312);
            this.lbBoost.Name = "lbBoost";
            this.lbBoost.Size = new System.Drawing.Size(59, 23);
            this.lbBoost.TabIndex = 187;
            this.lbBoost.Text = "100";
            this.lbBoost.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbMinValue
            // 
            this.lbMinValue.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMinValue.Location = new System.Drawing.Point(438, 474);
            this.lbMinValue.Name = "lbMinValue";
            this.lbMinValue.Size = new System.Drawing.Size(59, 23);
            this.lbMinValue.TabIndex = 184;
            this.lbMinValue.Text = "100";
            this.lbMinValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbMaxValue
            // 
            this.lbMaxValue.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMaxValue.Location = new System.Drawing.Point(438, 393);
            this.lbMaxValue.Name = "lbMaxValue";
            this.lbMaxValue.Size = new System.Drawing.Size(59, 23);
            this.lbMaxValue.TabIndex = 183;
            this.lbMaxValue.Text = "100";
            this.lbMaxValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(34, 301);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(133, 45);
            this.label3.TabIndex = 185;
            this.label3.Text = "Scaling Factor";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbThresholdValue
            // 
            this.lbThresholdValue.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbThresholdValue.Location = new System.Drawing.Point(438, 231);
            this.lbThresholdValue.Name = "lbThresholdValue";
            this.lbThresholdValue.Size = new System.Drawing.Size(59, 23);
            this.lbThresholdValue.TabIndex = 182;
            this.lbThresholdValue.Text = "100";
            this.lbThresholdValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbLow
            // 
            this.lbLow.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbLow.Location = new System.Drawing.Point(438, 150);
            this.lbLow.Name = "lbLow";
            this.lbLow.Size = new System.Drawing.Size(59, 23);
            this.lbLow.TabIndex = 181;
            this.lbLow.Text = "100";
            this.lbLow.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbHigh
            // 
            this.lbHigh.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbHigh.Location = new System.Drawing.Point(438, 69);
            this.lbHigh.Name = "lbHigh";
            this.lbHigh.Size = new System.Drawing.Size(59, 23);
            this.lbHigh.TabIndex = 180;
            this.lbHigh.Text = "100";
            this.lbHigh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbThreshold
            // 
            this.lbThreshold.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbThreshold.Location = new System.Drawing.Point(34, 220);
            this.lbThreshold.Name = "lbThreshold";
            this.lbThreshold.Size = new System.Drawing.Size(133, 45);
            this.lbThreshold.TabIndex = 176;
            this.lbThreshold.Text = "Threshold";
            this.lbThreshold.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbRateLow
            // 
            this.lbRateLow.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRateLow.Location = new System.Drawing.Point(34, 139);
            this.lbRateLow.Name = "lbRateLow";
            this.lbRateLow.Size = new System.Drawing.Size(133, 45);
            this.lbRateLow.TabIndex = 174;
            this.lbRateLow.Text = "Low Adjust";
            this.lbRateLow.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbProportional
            // 
            this.lbProportional.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbProportional.Location = new System.Drawing.Point(34, 58);
            this.lbProportional.Name = "lbProportional";
            this.lbProportional.Size = new System.Drawing.Size(133, 45);
            this.lbProportional.TabIndex = 170;
            this.lbProportional.Text = "High Adjust";
            this.lbProportional.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbMax
            // 
            this.lbMax.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMax.Location = new System.Drawing.Point(34, 382);
            this.lbMax.Name = "lbMax";
            this.lbMax.Size = new System.Drawing.Size(133, 45);
            this.lbMax.TabIndex = 172;
            this.lbMax.Text = "Max Power";
            this.lbMax.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbMin
            // 
            this.lbMin.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMin.Location = new System.Drawing.Point(34, 463);
            this.lbMin.Name = "lbMin";
            this.lbMin.Size = new System.Drawing.Size(133, 45);
            this.lbMin.TabIndex = 171;
            this.lbMin.Text = "Min Power";
            this.lbMin.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbProduct
            // 
            this.lbProduct.BackColor = System.Drawing.SystemColors.Control;
            this.lbProduct.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbProduct.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbProduct.Location = new System.Drawing.Point(71, 9);
            this.lbProduct.Name = "lbProduct";
            this.lbProduct.Size = new System.Drawing.Size(396, 28);
            this.lbProduct.TabIndex = 188;
            this.lbProduct.Text = "Product";
            this.lbProduct.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // HSmin
            // 
            this.HSmin.LargeChange = 1;
            this.HSmin.Location = new System.Drawing.Point(173, 463);
            this.HSmin.Name = "HSmin";
            this.HSmin.Size = new System.Drawing.Size(263, 45);
            this.HSmin.TabIndex = 333;
            this.HSmin.ValueChanged += new System.EventHandler(this.HShigh_ValueChanged);
            // 
            // HSmax
            // 
            this.HSmax.LargeChange = 1;
            this.HSmax.Location = new System.Drawing.Point(173, 382);
            this.HSmax.Name = "HSmax";
            this.HSmax.Size = new System.Drawing.Size(263, 45);
            this.HSmax.TabIndex = 334;
            this.HSmax.ValueChanged += new System.EventHandler(this.HShigh_ValueChanged);
            // 
            // HSscaling
            // 
            this.HSscaling.LargeChange = 1;
            this.HSscaling.Location = new System.Drawing.Point(173, 301);
            this.HSscaling.Name = "HSscaling";
            this.HSscaling.Size = new System.Drawing.Size(263, 45);
            this.HSscaling.TabIndex = 335;
            this.HSscaling.ValueChanged += new System.EventHandler(this.HShigh_ValueChanged);
            // 
            // HSthreshold
            // 
            this.HSthreshold.LargeChange = 1;
            this.HSthreshold.Location = new System.Drawing.Point(173, 220);
            this.HSthreshold.Name = "HSthreshold";
            this.HSthreshold.Size = new System.Drawing.Size(263, 45);
            this.HSthreshold.TabIndex = 336;
            this.HSthreshold.ValueChanged += new System.EventHandler(this.HShigh_ValueChanged);
            // 
            // HSlow
            // 
            this.HSlow.LargeChange = 1;
            this.HSlow.Location = new System.Drawing.Point(173, 139);
            this.HSlow.Name = "HSlow";
            this.HSlow.Size = new System.Drawing.Size(263, 45);
            this.HSlow.TabIndex = 337;
            this.HSlow.ValueChanged += new System.EventHandler(this.HShigh_ValueChanged);
            // 
            // HShigh
            // 
            this.HShigh.LargeChange = 1;
            this.HShigh.Location = new System.Drawing.Point(173, 58);
            this.HShigh.Name = "HShigh";
            this.HShigh.Size = new System.Drawing.Size(263, 45);
            this.HShigh.TabIndex = 338;
            this.HShigh.ValueChanged += new System.EventHandler(this.HShigh_ValueChanged);
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
            this.butGraph.Location = new System.Drawing.Point(54, 543);
            this.butGraph.Margin = new System.Windows.Forms.Padding(6);
            this.butGraph.Name = "butGraph";
            this.butGraph.Size = new System.Drawing.Size(81, 72);
            this.butGraph.TabIndex = 339;
            this.butGraph.UseVisualStyleBackColor = false;
            this.butGraph.Click += new System.EventHandler(this.butGraph_Click);
            // 
            // frmMenuControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 630);
            this.Controls.Add(this.butGraph);
            this.Controls.Add(this.HShigh);
            this.Controls.Add(this.HSlow);
            this.Controls.Add(this.HSthreshold);
            this.Controls.Add(this.HSscaling);
            this.Controls.Add(this.HSmax);
            this.Controls.Add(this.HSmin);
            this.Controls.Add(this.lbProduct);
            this.Controls.Add(this.lbBoost);
            this.Controls.Add(this.lbMinValue);
            this.Controls.Add(this.lbMaxValue);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbThresholdValue);
            this.Controls.Add(this.lbLow);
            this.Controls.Add(this.lbHigh);
            this.Controls.Add(this.lbThreshold);
            this.Controls.Add(this.lbRateLow);
            this.Controls.Add(this.lbProportional);
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
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbThresholdValue;
        private System.Windows.Forms.Label lbLow;
        private System.Windows.Forms.Label lbHigh;
        private System.Windows.Forms.Label lbThreshold;
        private System.Windows.Forms.Label lbRateLow;
        private System.Windows.Forms.Label lbProportional;
        private System.Windows.Forms.Label lbMax;
        private System.Windows.Forms.Label lbMin;
        private System.Windows.Forms.Label lbProduct;
        private System.Windows.Forms.HScrollBar HSmin;
        private System.Windows.Forms.HScrollBar HSmax;
        private System.Windows.Forms.HScrollBar HSscaling;
        private System.Windows.Forms.HScrollBar HSthreshold;
        private System.Windows.Forms.HScrollBar HSlow;
        private System.Windows.Forms.HScrollBar HShigh;
        private System.Windows.Forms.Button butGraph;
    }
}