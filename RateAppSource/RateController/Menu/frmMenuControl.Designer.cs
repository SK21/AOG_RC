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
            this.btGainPlus = new System.Windows.Forms.Button();
            this.btGainMinus = new System.Windows.Forms.Button();
            this.btIntegralPlus = new System.Windows.Forms.Button();
            this.btMaxPlus = new System.Windows.Forms.Button();
            this.btMinPlus = new System.Windows.Forms.Button();
            this.btIntegralMinus = new System.Windows.Forms.Button();
            this.btMaxMinus = new System.Windows.Forms.Button();
            this.btMinMinus = new System.Windows.Forms.Button();
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
            this.lbBoost.Location = new System.Drawing.Point(471, 165);
            this.lbBoost.Name = "lbBoost";
            this.lbBoost.Size = new System.Drawing.Size(59, 23);
            this.lbBoost.TabIndex = 187;
            this.lbBoost.Text = "100";
            this.lbBoost.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbMinValue
            // 
            this.lbMinValue.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMinValue.Location = new System.Drawing.Point(471, 462);
            this.lbMinValue.Name = "lbMinValue";
            this.lbMinValue.Size = new System.Drawing.Size(59, 23);
            this.lbMinValue.TabIndex = 184;
            this.lbMinValue.Text = "100";
            this.lbMinValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbMaxValue
            // 
            this.lbMaxValue.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMaxValue.Location = new System.Drawing.Point(471, 363);
            this.lbMaxValue.Name = "lbMaxValue";
            this.lbMaxValue.Size = new System.Drawing.Size(59, 23);
            this.lbMaxValue.TabIndex = 183;
            this.lbMaxValue.Text = "100";
            this.lbMaxValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbScaling
            // 
            this.lbScaling.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbScaling.Location = new System.Drawing.Point(15, 159);
            this.lbScaling.Name = "lbScaling";
            this.lbScaling.Size = new System.Drawing.Size(90, 34);
            this.lbScaling.TabIndex = 185;
            this.lbScaling.Text = "Gain";
            this.lbScaling.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbMax
            // 
            this.lbMax.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMax.Location = new System.Drawing.Point(15, 357);
            this.lbMax.Name = "lbMax";
            this.lbMax.Size = new System.Drawing.Size(90, 34);
            this.lbMax.TabIndex = 172;
            this.lbMax.Text = "Max";
            this.lbMax.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbMin
            // 
            this.lbMin.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMin.Location = new System.Drawing.Point(15, 456);
            this.lbMin.Name = "lbMin";
            this.lbMin.Size = new System.Drawing.Size(90, 34);
            this.lbMin.TabIndex = 171;
            this.lbMin.Text = "Min";
            this.lbMin.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbProduct
            // 
            this.lbProduct.BackColor = System.Drawing.SystemColors.Control;
            this.lbProduct.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbProduct.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbProduct.Location = new System.Drawing.Point(19, 92);
            this.lbProduct.Name = "lbProduct";
            this.lbProduct.Size = new System.Drawing.Size(507, 28);
            this.lbProduct.TabIndex = 188;
            this.lbProduct.Text = "Product";
            this.lbProduct.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // HSmin
            // 
            this.HSmin.LargeChange = 1;
            this.HSmin.Location = new System.Drawing.Point(197, 451);
            this.HSmin.Name = "HSmin";
            this.HSmin.Size = new System.Drawing.Size(182, 45);
            this.HSmin.TabIndex = 333;
            this.HSmin.ValueChanged += new System.EventHandler(this.HShigh_ValueChanged);
            // 
            // HSmax
            // 
            this.HSmax.LargeChange = 1;
            this.HSmax.Location = new System.Drawing.Point(197, 352);
            this.HSmax.Name = "HSmax";
            this.HSmax.Size = new System.Drawing.Size(182, 45);
            this.HSmax.TabIndex = 334;
            this.HSmax.ValueChanged += new System.EventHandler(this.HShigh_ValueChanged);
            // 
            // HSscaling
            // 
            this.HSscaling.LargeChange = 1;
            this.HSscaling.Location = new System.Drawing.Point(197, 154);
            this.HSscaling.Name = "HSscaling";
            this.HSscaling.Size = new System.Drawing.Size(182, 45);
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
            this.HSintegral.Location = new System.Drawing.Point(197, 253);
            this.HSintegral.Name = "HSintegral";
            this.HSintegral.Size = new System.Drawing.Size(182, 45);
            this.HSintegral.TabIndex = 342;
            this.HSintegral.ValueChanged += new System.EventHandler(this.HShigh_ValueChanged);
            // 
            // lbIntegral
            // 
            this.lbIntegral.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbIntegral.Location = new System.Drawing.Point(471, 264);
            this.lbIntegral.Name = "lbIntegral";
            this.lbIntegral.Size = new System.Drawing.Size(59, 23);
            this.lbIntegral.TabIndex = 341;
            this.lbIntegral.Text = "100";
            this.lbIntegral.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(15, 258);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 34);
            this.label2.TabIndex = 340;
            this.label2.Text = "Integral";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btGainPlus
            // 
            this.btGainPlus.BackColor = System.Drawing.Color.Transparent;
            this.btGainPlus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btGainPlus.FlatAppearance.BorderSize = 0;
            this.btGainPlus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btGainPlus.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btGainPlus.Image = global::RateController.Properties.Resources.plus_square;
            this.btGainPlus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btGainPlus.Location = new System.Drawing.Point(390, 145);
            this.btGainPlus.Name = "btGainPlus";
            this.btGainPlus.Size = new System.Drawing.Size(70, 63);
            this.btGainPlus.TabIndex = 343;
            this.btGainPlus.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btGainPlus.UseVisualStyleBackColor = false;
            // 
            // btGainMinus
            // 
            this.btGainMinus.BackColor = System.Drawing.Color.Transparent;
            this.btGainMinus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btGainMinus.FlatAppearance.BorderSize = 0;
            this.btGainMinus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btGainMinus.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btGainMinus.Image = global::RateController.Properties.Resources.minus_square;
            this.btGainMinus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btGainMinus.Location = new System.Drawing.Point(116, 145);
            this.btGainMinus.Name = "btGainMinus";
            this.btGainMinus.Size = new System.Drawing.Size(70, 63);
            this.btGainMinus.TabIndex = 344;
            this.btGainMinus.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btGainMinus.UseVisualStyleBackColor = false;
            // 
            // btIntegralPlus
            // 
            this.btIntegralPlus.BackColor = System.Drawing.Color.Transparent;
            this.btIntegralPlus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btIntegralPlus.FlatAppearance.BorderSize = 0;
            this.btIntegralPlus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btIntegralPlus.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btIntegralPlus.Image = global::RateController.Properties.Resources.plus_square;
            this.btIntegralPlus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btIntegralPlus.Location = new System.Drawing.Point(390, 244);
            this.btIntegralPlus.Name = "btIntegralPlus";
            this.btIntegralPlus.Size = new System.Drawing.Size(70, 63);
            this.btIntegralPlus.TabIndex = 345;
            this.btIntegralPlus.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btIntegralPlus.UseVisualStyleBackColor = false;
            // 
            // btMaxPlus
            // 
            this.btMaxPlus.BackColor = System.Drawing.Color.Transparent;
            this.btMaxPlus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btMaxPlus.FlatAppearance.BorderSize = 0;
            this.btMaxPlus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btMaxPlus.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btMaxPlus.Image = global::RateController.Properties.Resources.plus_square;
            this.btMaxPlus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btMaxPlus.Location = new System.Drawing.Point(390, 343);
            this.btMaxPlus.Name = "btMaxPlus";
            this.btMaxPlus.Size = new System.Drawing.Size(70, 63);
            this.btMaxPlus.TabIndex = 346;
            this.btMaxPlus.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btMaxPlus.UseVisualStyleBackColor = false;
            // 
            // btMinPlus
            // 
            this.btMinPlus.BackColor = System.Drawing.Color.Transparent;
            this.btMinPlus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btMinPlus.FlatAppearance.BorderSize = 0;
            this.btMinPlus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btMinPlus.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btMinPlus.Image = global::RateController.Properties.Resources.plus_square;
            this.btMinPlus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btMinPlus.Location = new System.Drawing.Point(390, 442);
            this.btMinPlus.Name = "btMinPlus";
            this.btMinPlus.Size = new System.Drawing.Size(70, 63);
            this.btMinPlus.TabIndex = 347;
            this.btMinPlus.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btMinPlus.UseVisualStyleBackColor = false;
            // 
            // btIntegralMinus
            // 
            this.btIntegralMinus.BackColor = System.Drawing.Color.Transparent;
            this.btIntegralMinus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btIntegralMinus.FlatAppearance.BorderSize = 0;
            this.btIntegralMinus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btIntegralMinus.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btIntegralMinus.Image = global::RateController.Properties.Resources.minus_square;
            this.btIntegralMinus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btIntegralMinus.Location = new System.Drawing.Point(116, 244);
            this.btIntegralMinus.Name = "btIntegralMinus";
            this.btIntegralMinus.Size = new System.Drawing.Size(70, 63);
            this.btIntegralMinus.TabIndex = 348;
            this.btIntegralMinus.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btIntegralMinus.UseVisualStyleBackColor = false;
            // 
            // btMaxMinus
            // 
            this.btMaxMinus.BackColor = System.Drawing.Color.Transparent;
            this.btMaxMinus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btMaxMinus.FlatAppearance.BorderSize = 0;
            this.btMaxMinus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btMaxMinus.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btMaxMinus.Image = global::RateController.Properties.Resources.minus_square;
            this.btMaxMinus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btMaxMinus.Location = new System.Drawing.Point(116, 343);
            this.btMaxMinus.Name = "btMaxMinus";
            this.btMaxMinus.Size = new System.Drawing.Size(70, 63);
            this.btMaxMinus.TabIndex = 349;
            this.btMaxMinus.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btMaxMinus.UseVisualStyleBackColor = false;
            // 
            // btMinMinus
            // 
            this.btMinMinus.BackColor = System.Drawing.Color.Transparent;
            this.btMinMinus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btMinMinus.FlatAppearance.BorderSize = 0;
            this.btMinMinus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btMinMinus.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btMinMinus.Image = global::RateController.Properties.Resources.minus_square;
            this.btMinMinus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btMinMinus.Location = new System.Drawing.Point(116, 442);
            this.btMinMinus.Name = "btMinMinus";
            this.btMinMinus.Size = new System.Drawing.Size(70, 63);
            this.btMinMinus.TabIndex = 350;
            this.btMinMinus.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btMinMinus.UseVisualStyleBackColor = false;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // frmMenuControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 678);
            this.Controls.Add(this.btMinMinus);
            this.Controls.Add(this.btMaxMinus);
            this.Controls.Add(this.btIntegralMinus);
            this.Controls.Add(this.btMinPlus);
            this.Controls.Add(this.btMaxPlus);
            this.Controls.Add(this.btIntegralPlus);
            this.Controls.Add(this.btGainMinus);
            this.Controls.Add(this.btGainPlus);
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
        private System.Windows.Forms.Button btGainPlus;
        private System.Windows.Forms.Button btGainMinus;
        private System.Windows.Forms.Button btIntegralPlus;
        private System.Windows.Forms.Button btMaxPlus;
        private System.Windows.Forms.Button btMinPlus;
        private System.Windows.Forms.Button btIntegralMinus;
        private System.Windows.Forms.Button btMaxMinus;
        private System.Windows.Forms.Button btMinMinus;
        private System.Windows.Forms.Timer timer1;
    }
}