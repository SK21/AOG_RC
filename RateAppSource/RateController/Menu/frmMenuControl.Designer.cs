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
            this.tbPIDtime = new System.Windows.Forms.TextBox();
            this.lbSampleTime = new System.Windows.Forms.Label();
            this.tbSlowAdj = new System.Windows.Forms.TextBox();
            this.lbSlowAdj = new System.Windows.Forms.Label();
            this.tbDeadband = new System.Windows.Forms.TextBox();
            this.lbDeadband = new System.Windows.Forms.Label();
            this.tbSlewRate = new System.Windows.Forms.TextBox();
            this.lbSlewRate = new System.Windows.Forms.Label();
            this.tbBrakepoint = new System.Windows.Forms.TextBox();
            this.lbBrakepoint = new System.Windows.Forms.Label();
            this.tbMinStart = new System.Windows.Forms.TextBox();
            this.lbMinStart = new System.Windows.Forms.Label();
            this.tbPauseTm = new System.Windows.Forms.TextBox();
            this.lbPauseTm = new System.Windows.Forms.Label();
            this.tbAdjustTm = new System.Windows.Forms.TextBox();
            this.lbAdjustTm = new System.Windows.Forms.Label();
            this.tbMaxMotorI = new System.Windows.Forms.TextBox();
            this.lbMaxMotorI = new System.Windows.Forms.Label();
            this.ckAdvanced = new System.Windows.Forms.CheckBox();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.pnlAdvanced = new System.Windows.Forms.Panel();
            this.tbMinHz = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbSampleSize = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbMaxHz = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pnlMain.SuspendLayout();
            this.pnlAdvanced.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnRight
            // 
            this.btnRight.BackColor = System.Drawing.Color.Transparent;
            this.btnRight.FlatAppearance.BorderSize = 0;
            this.btnRight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRight.Image = global::RateController.Properties.Resources.ArrowRight1;
            this.btnRight.Location = new System.Drawing.Point(300, 603);
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
            this.btnLeft.Location = new System.Drawing.Point(226, 603);
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
            this.btnCancel.Location = new System.Drawing.Point(374, 603);
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
            this.btnPIDloadDefaults.Location = new System.Drawing.Point(152, 603);
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
            this.lbBoost.Location = new System.Drawing.Point(459, 61);
            this.lbBoost.Name = "lbBoost";
            this.lbBoost.Size = new System.Drawing.Size(59, 23);
            this.lbBoost.TabIndex = 187;
            this.lbBoost.Text = "100";
            this.lbBoost.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbMinValue
            // 
            this.lbMinValue.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMinValue.Location = new System.Drawing.Point(459, 301);
            this.lbMinValue.Name = "lbMinValue";
            this.lbMinValue.Size = new System.Drawing.Size(59, 23);
            this.lbMinValue.TabIndex = 184;
            this.lbMinValue.Text = "100";
            this.lbMinValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbMaxValue
            // 
            this.lbMaxValue.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMaxValue.Location = new System.Drawing.Point(459, 221);
            this.lbMaxValue.Name = "lbMaxValue";
            this.lbMaxValue.Size = new System.Drawing.Size(59, 23);
            this.lbMaxValue.TabIndex = 183;
            this.lbMaxValue.Text = "100";
            this.lbMaxValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbScaling
            // 
            this.lbScaling.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbScaling.Location = new System.Drawing.Point(16, 55);
            this.lbScaling.Name = "lbScaling";
            this.lbScaling.Size = new System.Drawing.Size(82, 34);
            this.lbScaling.TabIndex = 185;
            this.lbScaling.Text = "Gain";
            this.lbScaling.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbMax
            // 
            this.lbMax.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMax.Location = new System.Drawing.Point(16, 215);
            this.lbMax.Name = "lbMax";
            this.lbMax.Size = new System.Drawing.Size(82, 34);
            this.lbMax.TabIndex = 172;
            this.lbMax.Text = "Max";
            this.lbMax.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbMin
            // 
            this.lbMin.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMin.Location = new System.Drawing.Point(16, 295);
            this.lbMin.Name = "lbMin";
            this.lbMin.Size = new System.Drawing.Size(82, 34);
            this.lbMin.TabIndex = 171;
            this.lbMin.Text = "Min";
            this.lbMin.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbProduct
            // 
            this.lbProduct.BackColor = System.Drawing.SystemColors.Control;
            this.lbProduct.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbProduct.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbProduct.Location = new System.Drawing.Point(17, 10);
            this.lbProduct.Name = "lbProduct";
            this.lbProduct.Size = new System.Drawing.Size(507, 28);
            this.lbProduct.TabIndex = 188;
            this.lbProduct.Text = "Product";
            this.lbProduct.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // HSmin
            // 
            this.HSmin.LargeChange = 1;
            this.HSmin.Location = new System.Drawing.Point(188, 290);
            this.HSmin.Name = "HSmin";
            this.HSmin.Size = new System.Drawing.Size(182, 45);
            this.HSmin.TabIndex = 333;
            this.HSmin.ValueChanged += new System.EventHandler(this.HShigh_ValueChanged);
            // 
            // HSmax
            // 
            this.HSmax.LargeChange = 1;
            this.HSmax.Location = new System.Drawing.Point(185, 210);
            this.HSmax.Name = "HSmax";
            this.HSmax.Size = new System.Drawing.Size(182, 45);
            this.HSmax.TabIndex = 334;
            this.HSmax.ValueChanged += new System.EventHandler(this.HShigh_ValueChanged);
            // 
            // HSscaling
            // 
            this.HSscaling.LargeChange = 1;
            this.HSscaling.Location = new System.Drawing.Point(185, 50);
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
            this.butGraph.Location = new System.Drawing.Point(4, 603);
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
            this.HSintegral.Location = new System.Drawing.Point(185, 130);
            this.HSintegral.Name = "HSintegral";
            this.HSintegral.Size = new System.Drawing.Size(182, 45);
            this.HSintegral.TabIndex = 342;
            this.HSintegral.ValueChanged += new System.EventHandler(this.HShigh_ValueChanged);
            // 
            // lbIntegral
            // 
            this.lbIntegral.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbIntegral.Location = new System.Drawing.Point(459, 141);
            this.lbIntegral.Name = "lbIntegral";
            this.lbIntegral.Size = new System.Drawing.Size(59, 23);
            this.lbIntegral.TabIndex = 341;
            this.lbIntegral.Text = "100";
            this.lbIntegral.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(16, 135);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 34);
            this.label2.TabIndex = 340;
            this.label2.Text = "Integral";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.btGainPlus.Location = new System.Drawing.Point(378, 41);
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
            this.btGainMinus.Location = new System.Drawing.Point(104, 41);
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
            this.btIntegralPlus.Location = new System.Drawing.Point(378, 121);
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
            this.btMaxPlus.Location = new System.Drawing.Point(378, 201);
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
            this.btMinPlus.Location = new System.Drawing.Point(378, 281);
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
            this.btIntegralMinus.Location = new System.Drawing.Point(104, 121);
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
            this.btMaxMinus.Location = new System.Drawing.Point(104, 201);
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
            this.btMinMinus.Location = new System.Drawing.Point(104, 281);
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
            // tbPIDtime
            // 
            this.tbPIDtime.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPIDtime.Location = new System.Drawing.Point(122, 65);
            this.tbPIDtime.Name = "tbPIDtime";
            this.tbPIDtime.Size = new System.Drawing.Size(54, 29);
            this.tbPIDtime.TabIndex = 352;
            this.tbPIDtime.TabStop = false;
            this.tbPIDtime.Text = "50";
            this.tbPIDtime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbSampleTime
            // 
            this.lbSampleTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSampleTime.Location = new System.Drawing.Point(5, 67);
            this.lbSampleTime.Name = "lbSampleTime";
            this.lbSampleTime.Size = new System.Drawing.Size(105, 24);
            this.lbSampleTime.TabIndex = 351;
            this.lbSampleTime.Text = "PID Time";
            // 
            // tbSlowAdj
            // 
            this.tbSlowAdj.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSlowAdj.Location = new System.Drawing.Point(483, 8);
            this.tbSlowAdj.Name = "tbSlowAdj";
            this.tbSlowAdj.Size = new System.Drawing.Size(54, 29);
            this.tbSlowAdj.TabIndex = 358;
            this.tbSlowAdj.TabStop = false;
            this.tbSlowAdj.Text = "30";
            this.tbSlowAdj.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbSlowAdj
            // 
            this.lbSlowAdj.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSlowAdj.Location = new System.Drawing.Point(370, 10);
            this.lbSlowAdj.Name = "lbSlowAdj";
            this.lbSlowAdj.Size = new System.Drawing.Size(105, 24);
            this.lbSlowAdj.TabIndex = 357;
            this.lbSlowAdj.Text = "Slow Adj";
            // 
            // tbDeadband
            // 
            this.tbDeadband.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDeadband.Location = new System.Drawing.Point(123, 8);
            this.tbDeadband.Name = "tbDeadband";
            this.tbDeadband.Size = new System.Drawing.Size(54, 29);
            this.tbDeadband.TabIndex = 356;
            this.tbDeadband.TabStop = false;
            this.tbDeadband.Text = "1.5";
            this.tbDeadband.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbDeadband
            // 
            this.lbDeadband.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDeadband.Location = new System.Drawing.Point(5, 10);
            this.lbDeadband.Name = "lbDeadband";
            this.lbDeadband.Size = new System.Drawing.Size(105, 24);
            this.lbDeadband.TabIndex = 355;
            this.lbDeadband.Text = "Deadband";
            // 
            // tbSlewRate
            // 
            this.tbSlewRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSlewRate.Location = new System.Drawing.Point(303, 65);
            this.tbSlewRate.Name = "tbSlewRate";
            this.tbSlewRate.Size = new System.Drawing.Size(54, 29);
            this.tbSlewRate.TabIndex = 362;
            this.tbSlewRate.TabStop = false;
            this.tbSlewRate.Text = "6";
            this.tbSlewRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbSlewRate
            // 
            this.lbSlewRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSlewRate.Location = new System.Drawing.Point(190, 67);
            this.lbSlewRate.Name = "lbSlewRate";
            this.lbSlewRate.Size = new System.Drawing.Size(105, 24);
            this.lbSlewRate.TabIndex = 361;
            this.lbSlewRate.Text = "Slew Rate";
            // 
            // tbBrakepoint
            // 
            this.tbBrakepoint.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbBrakepoint.Location = new System.Drawing.Point(303, 8);
            this.tbBrakepoint.Name = "tbBrakepoint";
            this.tbBrakepoint.Size = new System.Drawing.Size(54, 29);
            this.tbBrakepoint.TabIndex = 360;
            this.tbBrakepoint.TabStop = false;
            this.tbBrakepoint.Text = "35";
            this.tbBrakepoint.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbBrakepoint
            // 
            this.lbBrakepoint.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbBrakepoint.Location = new System.Drawing.Point(190, 10);
            this.lbBrakepoint.Name = "lbBrakepoint";
            this.lbBrakepoint.Size = new System.Drawing.Size(105, 24);
            this.lbBrakepoint.TabIndex = 359;
            this.lbBrakepoint.Text = "Brakepoint";
            // 
            // tbMinStart
            // 
            this.tbMinStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbMinStart.Location = new System.Drawing.Point(123, 122);
            this.tbMinStart.Name = "tbMinStart";
            this.tbMinStart.Size = new System.Drawing.Size(54, 29);
            this.tbMinStart.TabIndex = 372;
            this.tbMinStart.TabStop = false;
            this.tbMinStart.Text = "3";
            this.tbMinStart.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbMinStart
            // 
            this.lbMinStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMinStart.Location = new System.Drawing.Point(5, 124);
            this.lbMinStart.Name = "lbMinStart";
            this.lbMinStart.Size = new System.Drawing.Size(105, 24);
            this.lbMinStart.TabIndex = 371;
            this.lbMinStart.Text = "Min Start";
            // 
            // tbPauseTm
            // 
            this.tbPauseTm.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPauseTm.Location = new System.Drawing.Point(483, 122);
            this.tbPauseTm.Name = "tbPauseTm";
            this.tbPauseTm.Size = new System.Drawing.Size(54, 29);
            this.tbPauseTm.TabIndex = 370;
            this.tbPauseTm.TabStop = false;
            this.tbPauseTm.Text = "400";
            this.tbPauseTm.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbPauseTm
            // 
            this.lbPauseTm.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPauseTm.Location = new System.Drawing.Point(370, 124);
            this.lbPauseTm.Name = "lbPauseTm";
            this.lbPauseTm.Size = new System.Drawing.Size(105, 24);
            this.lbPauseTm.TabIndex = 369;
            this.lbPauseTm.Text = "Pause Tm";
            // 
            // tbAdjustTm
            // 
            this.tbAdjustTm.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbAdjustTm.Location = new System.Drawing.Point(303, 122);
            this.tbAdjustTm.Name = "tbAdjustTm";
            this.tbAdjustTm.Size = new System.Drawing.Size(54, 29);
            this.tbAdjustTm.TabIndex = 366;
            this.tbAdjustTm.TabStop = false;
            this.tbAdjustTm.Text = "80";
            this.tbAdjustTm.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbAdjustTm
            // 
            this.lbAdjustTm.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbAdjustTm.Location = new System.Drawing.Point(190, 124);
            this.lbAdjustTm.Name = "lbAdjustTm";
            this.lbAdjustTm.Size = new System.Drawing.Size(105, 24);
            this.lbAdjustTm.TabIndex = 365;
            this.lbAdjustTm.Text = "Adjust Tm";
            // 
            // tbMaxMotorI
            // 
            this.tbMaxMotorI.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbMaxMotorI.Location = new System.Drawing.Point(483, 65);
            this.tbMaxMotorI.Name = "tbMaxMotorI";
            this.tbMaxMotorI.Size = new System.Drawing.Size(54, 29);
            this.tbMaxMotorI.TabIndex = 364;
            this.tbMaxMotorI.TabStop = false;
            this.tbMaxMotorI.Text = "0.10";
            this.tbMaxMotorI.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbMaxMotorI
            // 
            this.lbMaxMotorI.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMaxMotorI.Location = new System.Drawing.Point(370, 67);
            this.lbMaxMotorI.Name = "lbMaxMotorI";
            this.lbMaxMotorI.Size = new System.Drawing.Size(105, 24);
            this.lbMaxMotorI.TabIndex = 363;
            this.lbMaxMotorI.Text = "Mx Integral";
            // 
            // ckAdvanced
            // 
            this.ckAdvanced.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckAdvanced.Checked = true;
            this.ckAdvanced.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckAdvanced.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckAdvanced.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckAdvanced.Image = global::RateController.Properties.Resources.caret_circle_up;
            this.ckAdvanced.Location = new System.Drawing.Point(78, 603);
            this.ckAdvanced.Name = "ckAdvanced";
            this.ckAdvanced.Size = new System.Drawing.Size(70, 63);
            this.ckAdvanced.TabIndex = 374;
            this.ckAdvanced.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckAdvanced.UseVisualStyleBackColor = true;
            this.ckAdvanced.CheckedChanged += new System.EventHandler(this.ckAdvanced_CheckedChanged);
            this.ckAdvanced.Click += new System.EventHandler(this.ckAdvanced_Click);
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.lbProduct);
            this.pnlMain.Controls.Add(this.lbMin);
            this.pnlMain.Controls.Add(this.lbMax);
            this.pnlMain.Controls.Add(this.lbScaling);
            this.pnlMain.Controls.Add(this.lbMaxValue);
            this.pnlMain.Controls.Add(this.lbMinValue);
            this.pnlMain.Controls.Add(this.lbBoost);
            this.pnlMain.Controls.Add(this.HSmin);
            this.pnlMain.Controls.Add(this.HSmax);
            this.pnlMain.Controls.Add(this.HSscaling);
            this.pnlMain.Controls.Add(this.label2);
            this.pnlMain.Controls.Add(this.lbIntegral);
            this.pnlMain.Controls.Add(this.HSintegral);
            this.pnlMain.Controls.Add(this.btGainPlus);
            this.pnlMain.Controls.Add(this.btGainMinus);
            this.pnlMain.Controls.Add(this.btIntegralPlus);
            this.pnlMain.Controls.Add(this.btMaxPlus);
            this.pnlMain.Controls.Add(this.btMinPlus);
            this.pnlMain.Controls.Add(this.btIntegralMinus);
            this.pnlMain.Controls.Add(this.btMaxMinus);
            this.pnlMain.Controls.Add(this.btMinMinus);
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(540, 356);
            this.pnlMain.TabIndex = 375;
            // 
            // pnlAdvanced
            // 
            this.pnlAdvanced.Controls.Add(this.tbMinHz);
            this.pnlAdvanced.Controls.Add(this.label1);
            this.pnlAdvanced.Controls.Add(this.tbSampleSize);
            this.pnlAdvanced.Controls.Add(this.label3);
            this.pnlAdvanced.Controls.Add(this.tbMaxHz);
            this.pnlAdvanced.Controls.Add(this.label4);
            this.pnlAdvanced.Controls.Add(this.tbDeadband);
            this.pnlAdvanced.Controls.Add(this.lbSampleTime);
            this.pnlAdvanced.Controls.Add(this.tbPIDtime);
            this.pnlAdvanced.Controls.Add(this.tbSlowAdj);
            this.pnlAdvanced.Controls.Add(this.lbSlowAdj);
            this.pnlAdvanced.Controls.Add(this.tbMinStart);
            this.pnlAdvanced.Controls.Add(this.lbDeadband);
            this.pnlAdvanced.Controls.Add(this.lbMinStart);
            this.pnlAdvanced.Controls.Add(this.lbBrakepoint);
            this.pnlAdvanced.Controls.Add(this.tbPauseTm);
            this.pnlAdvanced.Controls.Add(this.tbBrakepoint);
            this.pnlAdvanced.Controls.Add(this.lbPauseTm);
            this.pnlAdvanced.Controls.Add(this.tbMaxMotorI);
            this.pnlAdvanced.Controls.Add(this.lbSlewRate);
            this.pnlAdvanced.Controls.Add(this.tbSlewRate);
            this.pnlAdvanced.Controls.Add(this.tbAdjustTm);
            this.pnlAdvanced.Controls.Add(this.lbMaxMotorI);
            this.pnlAdvanced.Controls.Add(this.lbAdjustTm);
            this.pnlAdvanced.Location = new System.Drawing.Point(0, 373);
            this.pnlAdvanced.Name = "pnlAdvanced";
            this.pnlAdvanced.Size = new System.Drawing.Size(540, 220);
            this.pnlAdvanced.TabIndex = 376;
            // 
            // tbMinHz
            // 
            this.tbMinHz.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbMinHz.Location = new System.Drawing.Point(122, 179);
            this.tbMinHz.Name = "tbMinHz";
            this.tbMinHz.Size = new System.Drawing.Size(54, 29);
            this.tbMinHz.TabIndex = 379;
            this.tbMinHz.TabStop = false;
            this.tbMinHz.Text = "3";
            this.tbMinHz.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 181);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 24);
            this.label1.TabIndex = 378;
            this.label1.Text = "Min Hz";
            // 
            // tbSampleSize
            // 
            this.tbSampleSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSampleSize.Location = new System.Drawing.Point(482, 179);
            this.tbSampleSize.Name = "tbSampleSize";
            this.tbSampleSize.Size = new System.Drawing.Size(54, 29);
            this.tbSampleSize.TabIndex = 377;
            this.tbSampleSize.TabStop = false;
            this.tbSampleSize.Text = "400";
            this.tbSampleSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(369, 181);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 24);
            this.label3.TabIndex = 376;
            this.label3.Text = "Smp Size";
            // 
            // tbMaxHz
            // 
            this.tbMaxHz.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbMaxHz.Location = new System.Drawing.Point(302, 179);
            this.tbMaxHz.Name = "tbMaxHz";
            this.tbMaxHz.Size = new System.Drawing.Size(54, 29);
            this.tbMaxHz.TabIndex = 375;
            this.tbMaxHz.TabStop = false;
            this.tbMaxHz.Text = "80";
            this.tbMaxHz.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(189, 181);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(105, 24);
            this.label4.TabIndex = 374;
            this.label4.Text = "Max Hz";
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(4, 362);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(530, 5);
            this.groupBox1.TabIndex = 373;
            this.groupBox1.TabStop = false;
            // 
            // frmMenuControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 678);
            this.Controls.Add(this.pnlAdvanced);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.ckAdvanced);
            this.Controls.Add(this.butGraph);
            this.Controls.Add(this.btnPIDloadDefaults);
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnLeft);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMenuControl";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuControl";
            this.Activated += new System.EventHandler(this.frmMenuControl_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMenuControl_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMenuControl_FormClosed);
            this.Load += new System.EventHandler(this.frmMenuControl_Load);
            this.pnlMain.ResumeLayout(false);
            this.pnlAdvanced.ResumeLayout(false);
            this.pnlAdvanced.PerformLayout();
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
        private System.Windows.Forms.TextBox tbPIDtime;
        private System.Windows.Forms.Label lbSampleTime;
        private System.Windows.Forms.TextBox tbSlowAdj;
        private System.Windows.Forms.Label lbSlowAdj;
        private System.Windows.Forms.TextBox tbDeadband;
        private System.Windows.Forms.Label lbDeadband;
        private System.Windows.Forms.TextBox tbSlewRate;
        private System.Windows.Forms.Label lbSlewRate;
        private System.Windows.Forms.TextBox tbBrakepoint;
        private System.Windows.Forms.Label lbBrakepoint;
        private System.Windows.Forms.TextBox tbMinStart;
        private System.Windows.Forms.Label lbMinStart;
        private System.Windows.Forms.TextBox tbPauseTm;
        private System.Windows.Forms.Label lbPauseTm;
        private System.Windows.Forms.TextBox tbAdjustTm;
        private System.Windows.Forms.Label lbAdjustTm;
        private System.Windows.Forms.TextBox tbMaxMotorI;
        private System.Windows.Forms.Label lbMaxMotorI;
        private System.Windows.Forms.CheckBox ckAdvanced;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Panel pnlAdvanced;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbMinHz;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbSampleSize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbMaxHz;
        private System.Windows.Forms.Label label4;
    }
}