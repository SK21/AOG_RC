namespace RateController.Menu
{
    partial class frmMenuMonitoring
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
            this.lbProduct = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.wifiBar = new System.Windows.Forms.ProgressBar();
            this.tbCountsRev = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.lbRPM = new System.Windows.Forms.Label();
            this.lbErrorPercent = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.lbSections = new System.Windows.Forms.Label();
            this.lbRateAppliedData = new System.Windows.Forms.Label();
            this.lb33 = new System.Windows.Forms.Label();
            this.lbRateSetData = new System.Windows.Forms.Label();
            this.lb32 = new System.Windows.Forms.Label();
            this.lbSpeedData = new System.Windows.Forms.Label();
            this.lbSpeed = new System.Windows.Forms.Label();
            this.lbWidthData = new System.Windows.Forms.Label();
            this.lbWidth = new System.Windows.Forms.Label();
            this.lbPWMdata = new System.Windows.Forms.Label();
            this.lb34 = new System.Windows.Forms.Label();
            this.lbWorkRateData = new System.Windows.Forms.Label();
            this.lbWorkRate = new System.Windows.Forms.Label();
            this.sec15 = new System.Windows.Forms.Label();
            this.sec14 = new System.Windows.Forms.Label();
            this.sec13 = new System.Windows.Forms.Label();
            this.sec12 = new System.Windows.Forms.Label();
            this.sec11 = new System.Windows.Forms.Label();
            this.sec10 = new System.Windows.Forms.Label();
            this.sec9 = new System.Windows.Forms.Label();
            this.sec8 = new System.Windows.Forms.Label();
            this.sec7 = new System.Windows.Forms.Label();
            this.sec6 = new System.Windows.Forms.Label();
            this.sec5 = new System.Windows.Forms.Label();
            this.sec4 = new System.Windows.Forms.Label();
            this.sec3 = new System.Windows.Forms.Label();
            this.sec2 = new System.Windows.Forms.Label();
            this.sec1 = new System.Windows.Forms.Label();
            this.sec0 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
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
            this.btnRight.TabIndex = 172;
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
            this.btnLeft.TabIndex = 171;
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
            this.btnCancel.TabIndex = 170;
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
            this.btnOK.TabIndex = 169;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lbProduct
            // 
            this.lbProduct.BackColor = System.Drawing.SystemColors.Control;
            this.lbProduct.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbProduct.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbProduct.Location = new System.Drawing.Point(71, 9);
            this.lbProduct.Name = "lbProduct";
            this.lbProduct.Size = new System.Drawing.Size(396, 36);
            this.lbProduct.TabIndex = 190;
            this.lbProduct.Text = "Product";
            this.lbProduct.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(83, 412);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 23);
            this.label2.TabIndex = 249;
            this.label2.Text = "Wifi Signal";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // wifiBar
            // 
            this.wifiBar.Location = new System.Drawing.Point(320, 412);
            this.wifiBar.Maximum = 3;
            this.wifiBar.Name = "wifiBar";
            this.wifiBar.Size = new System.Drawing.Size(130, 25);
            this.wifiBar.Step = 3;
            this.wifiBar.TabIndex = 248;
            // 
            // tbCountsRev
            // 
            this.tbCountsRev.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbCountsRev.Location = new System.Drawing.Point(320, 213);
            this.tbCountsRev.MaxLength = 8;
            this.tbCountsRev.Name = "tbCountsRev";
            this.tbCountsRev.Size = new System.Drawing.Size(130, 30);
            this.tbCountsRev.TabIndex = 210;
            this.tbCountsRev.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbCountsRev.TextChanged += new System.EventHandler(this.tbCountsRev_TextChanged);
            this.tbCountsRev.Enter += new System.EventHandler(this.tbCountsRev_Enter);
            this.tbCountsRev.Validating += new System.ComponentModel.CancelEventHandler(this.tbCountsRev_Validating);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.Location = new System.Drawing.Point(83, 217);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(105, 23);
            this.label24.TabIndex = 247;
            this.label24.Text = "Counts/Rev";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(83, 256);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(47, 23);
            this.label23.TabIndex = 246;
            this.label23.Text = "RPM";
            // 
            // lbRPM
            // 
            this.lbRPM.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbRPM.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRPM.Location = new System.Drawing.Point(320, 255);
            this.lbRPM.Name = "lbRPM";
            this.lbRPM.Size = new System.Drawing.Size(130, 25);
            this.lbRPM.TabIndex = 245;
            this.lbRPM.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbErrorPercent
            // 
            this.lbErrorPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbErrorPercent.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbErrorPercent.Location = new System.Drawing.Point(320, 138);
            this.lbErrorPercent.Name = "lbErrorPercent";
            this.lbErrorPercent.Size = new System.Drawing.Size(130, 25);
            this.lbErrorPercent.TabIndex = 243;
            this.lbErrorPercent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(83, 139);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(77, 23);
            this.label15.TabIndex = 244;
            this.label15.Text = "Error %";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(408, 515);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(30, 23);
            this.label20.TabIndex = 242;
            this.label20.Text = "16";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(94, 514);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(20, 23);
            this.label19.TabIndex = 241;
            this.label19.Text = "9";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(408, 478);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(20, 23);
            this.label18.TabIndex = 240;
            this.label18.Text = "8";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(94, 479);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(20, 23);
            this.label17.TabIndex = 239;
            this.label17.Text = "1";
            // 
            // lbSections
            // 
            this.lbSections.AutoSize = true;
            this.lbSections.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSections.Location = new System.Drawing.Point(217, 448);
            this.lbSections.Name = "lbSections";
            this.lbSections.Size = new System.Drawing.Size(79, 23);
            this.lbSections.TabIndex = 223;
            this.lbSections.Text = "Sections";
            // 
            // lbRateAppliedData
            // 
            this.lbRateAppliedData.BackColor = System.Drawing.Color.Transparent;
            this.lbRateAppliedData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbRateAppliedData.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRateAppliedData.Location = new System.Drawing.Point(320, 60);
            this.lbRateAppliedData.Name = "lbRateAppliedData";
            this.lbRateAppliedData.Size = new System.Drawing.Size(130, 25);
            this.lbRateAppliedData.TabIndex = 209;
            this.lbRateAppliedData.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb33
            // 
            this.lb33.AutoSize = true;
            this.lb33.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb33.Location = new System.Drawing.Point(83, 61);
            this.lb33.Name = "lb33";
            this.lb33.Size = new System.Drawing.Size(115, 23);
            this.lb33.TabIndex = 221;
            this.lb33.Text = "UPM Applied";
            // 
            // lbRateSetData
            // 
            this.lbRateSetData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbRateSetData.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRateSetData.Location = new System.Drawing.Point(320, 99);
            this.lbRateSetData.Name = "lbRateSetData";
            this.lbRateSetData.Size = new System.Drawing.Size(130, 25);
            this.lbRateSetData.TabIndex = 219;
            this.lbRateSetData.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb32
            // 
            this.lb32.AutoSize = true;
            this.lb32.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb32.Location = new System.Drawing.Point(83, 100);
            this.lb32.Name = "lb32";
            this.lb32.Size = new System.Drawing.Size(108, 23);
            this.lb32.TabIndex = 220;
            this.lb32.Text = "UPM Target";
            // 
            // lbSpeedData
            // 
            this.lbSpeedData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbSpeedData.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSpeedData.Location = new System.Drawing.Point(320, 294);
            this.lbSpeedData.Name = "lbSpeedData";
            this.lbSpeedData.Size = new System.Drawing.Size(130, 25);
            this.lbSpeedData.TabIndex = 217;
            this.lbSpeedData.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbSpeed
            // 
            this.lbSpeed.AutoSize = true;
            this.lbSpeed.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSpeed.Location = new System.Drawing.Point(83, 295);
            this.lbSpeed.Name = "lbSpeed";
            this.lbSpeed.Size = new System.Drawing.Size(63, 23);
            this.lbSpeed.TabIndex = 218;
            this.lbSpeed.Text = "Speed";
            // 
            // lbWidthData
            // 
            this.lbWidthData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbWidthData.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbWidthData.Location = new System.Drawing.Point(320, 333);
            this.lbWidthData.Name = "lbWidthData";
            this.lbWidthData.Size = new System.Drawing.Size(130, 25);
            this.lbWidthData.TabIndex = 215;
            this.lbWidthData.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbWidth
            // 
            this.lbWidth.AutoSize = true;
            this.lbWidth.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbWidth.Location = new System.Drawing.Point(83, 334);
            this.lbWidth.Name = "lbWidth";
            this.lbWidth.Size = new System.Drawing.Size(166, 23);
            this.lbWidth.TabIndex = 216;
            this.lbWidth.Text = "Working Width (ft)";
            // 
            // lbPWMdata
            // 
            this.lbPWMdata.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbPWMdata.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPWMdata.Location = new System.Drawing.Point(320, 177);
            this.lbPWMdata.Name = "lbPWMdata";
            this.lbPWMdata.Size = new System.Drawing.Size(130, 25);
            this.lbPWMdata.TabIndex = 213;
            this.lbPWMdata.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb34
            // 
            this.lb34.AutoSize = true;
            this.lb34.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb34.Location = new System.Drawing.Point(83, 178);
            this.lb34.Name = "lb34";
            this.lb34.Size = new System.Drawing.Size(52, 23);
            this.lb34.TabIndex = 214;
            this.lb34.Text = "PWM";
            // 
            // lbWorkRateData
            // 
            this.lbWorkRateData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbWorkRateData.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbWorkRateData.Location = new System.Drawing.Point(320, 372);
            this.lbWorkRateData.Name = "lbWorkRateData";
            this.lbWorkRateData.Size = new System.Drawing.Size(130, 25);
            this.lbWorkRateData.TabIndex = 211;
            this.lbWorkRateData.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbWorkRate
            // 
            this.lbWorkRate.AutoSize = true;
            this.lbWorkRate.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbWorkRate.Location = new System.Drawing.Point(83, 373);
            this.lbWorkRate.Name = "lbWorkRate";
            this.lbWorkRate.Size = new System.Drawing.Size(108, 23);
            this.lbWorkRate.TabIndex = 212;
            this.lbWorkRate.Text = "Hectares/hr";
            // 
            // sec15
            // 
            this.sec15.BackColor = System.Drawing.SystemColors.Control;
            this.sec15.Cursor = System.Windows.Forms.Cursors.Default;
            this.sec15.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sec15.Image = global::RateController.Properties.Resources.OffSmall;
            this.sec15.Location = new System.Drawing.Point(372, 515);
            this.sec15.Name = "sec15";
            this.sec15.Size = new System.Drawing.Size(30, 23);
            this.sec15.TabIndex = 238;
            this.sec15.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // sec14
            // 
            this.sec14.BackColor = System.Drawing.SystemColors.Control;
            this.sec14.Cursor = System.Windows.Forms.Cursors.Default;
            this.sec14.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sec14.Image = global::RateController.Properties.Resources.OffSmall;
            this.sec14.Location = new System.Drawing.Point(336, 515);
            this.sec14.Name = "sec14";
            this.sec14.Size = new System.Drawing.Size(30, 23);
            this.sec14.TabIndex = 237;
            this.sec14.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // sec13
            // 
            this.sec13.BackColor = System.Drawing.SystemColors.Control;
            this.sec13.Cursor = System.Windows.Forms.Cursors.Default;
            this.sec13.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sec13.Image = global::RateController.Properties.Resources.OffSmall;
            this.sec13.Location = new System.Drawing.Point(300, 515);
            this.sec13.Name = "sec13";
            this.sec13.Size = new System.Drawing.Size(30, 23);
            this.sec13.TabIndex = 236;
            this.sec13.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // sec12
            // 
            this.sec12.BackColor = System.Drawing.SystemColors.Control;
            this.sec12.Cursor = System.Windows.Forms.Cursors.Default;
            this.sec12.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sec12.Image = global::RateController.Properties.Resources.OffSmall;
            this.sec12.Location = new System.Drawing.Point(264, 515);
            this.sec12.Name = "sec12";
            this.sec12.Size = new System.Drawing.Size(30, 23);
            this.sec12.TabIndex = 235;
            this.sec12.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // sec11
            // 
            this.sec11.BackColor = System.Drawing.SystemColors.Control;
            this.sec11.Cursor = System.Windows.Forms.Cursors.Default;
            this.sec11.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sec11.Image = global::RateController.Properties.Resources.OffSmall;
            this.sec11.Location = new System.Drawing.Point(228, 515);
            this.sec11.Name = "sec11";
            this.sec11.Size = new System.Drawing.Size(30, 23);
            this.sec11.TabIndex = 234;
            this.sec11.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // sec10
            // 
            this.sec10.BackColor = System.Drawing.SystemColors.Control;
            this.sec10.Cursor = System.Windows.Forms.Cursors.Default;
            this.sec10.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sec10.Image = global::RateController.Properties.Resources.OffSmall;
            this.sec10.Location = new System.Drawing.Point(192, 515);
            this.sec10.Name = "sec10";
            this.sec10.Size = new System.Drawing.Size(30, 23);
            this.sec10.TabIndex = 233;
            this.sec10.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // sec9
            // 
            this.sec9.BackColor = System.Drawing.SystemColors.Control;
            this.sec9.Cursor = System.Windows.Forms.Cursors.Default;
            this.sec9.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sec9.Image = global::RateController.Properties.Resources.OffSmall;
            this.sec9.Location = new System.Drawing.Point(156, 515);
            this.sec9.Name = "sec9";
            this.sec9.Size = new System.Drawing.Size(30, 23);
            this.sec9.TabIndex = 232;
            this.sec9.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // sec8
            // 
            this.sec8.BackColor = System.Drawing.SystemColors.Control;
            this.sec8.Cursor = System.Windows.Forms.Cursors.Default;
            this.sec8.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sec8.Image = global::RateController.Properties.Resources.OffSmall;
            this.sec8.Location = new System.Drawing.Point(120, 515);
            this.sec8.Name = "sec8";
            this.sec8.Size = new System.Drawing.Size(30, 23);
            this.sec8.TabIndex = 231;
            this.sec8.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // sec7
            // 
            this.sec7.BackColor = System.Drawing.SystemColors.Control;
            this.sec7.Cursor = System.Windows.Forms.Cursors.Default;
            this.sec7.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sec7.Image = global::RateController.Properties.Resources.OffSmall;
            this.sec7.Location = new System.Drawing.Point(372, 479);
            this.sec7.Name = "sec7";
            this.sec7.Size = new System.Drawing.Size(30, 23);
            this.sec7.TabIndex = 230;
            this.sec7.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // sec6
            // 
            this.sec6.BackColor = System.Drawing.SystemColors.Control;
            this.sec6.Cursor = System.Windows.Forms.Cursors.Default;
            this.sec6.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sec6.Image = global::RateController.Properties.Resources.OffSmall;
            this.sec6.Location = new System.Drawing.Point(336, 479);
            this.sec6.Name = "sec6";
            this.sec6.Size = new System.Drawing.Size(30, 23);
            this.sec6.TabIndex = 229;
            this.sec6.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // sec5
            // 
            this.sec5.BackColor = System.Drawing.SystemColors.Control;
            this.sec5.Cursor = System.Windows.Forms.Cursors.Default;
            this.sec5.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sec5.Image = global::RateController.Properties.Resources.OffSmall;
            this.sec5.Location = new System.Drawing.Point(300, 479);
            this.sec5.Name = "sec5";
            this.sec5.Size = new System.Drawing.Size(30, 23);
            this.sec5.TabIndex = 228;
            this.sec5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // sec4
            // 
            this.sec4.BackColor = System.Drawing.SystemColors.Control;
            this.sec4.Cursor = System.Windows.Forms.Cursors.Default;
            this.sec4.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sec4.Image = global::RateController.Properties.Resources.OffSmall;
            this.sec4.Location = new System.Drawing.Point(264, 479);
            this.sec4.Name = "sec4";
            this.sec4.Size = new System.Drawing.Size(30, 23);
            this.sec4.TabIndex = 227;
            this.sec4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // sec3
            // 
            this.sec3.BackColor = System.Drawing.SystemColors.Control;
            this.sec3.Cursor = System.Windows.Forms.Cursors.Default;
            this.sec3.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sec3.Image = global::RateController.Properties.Resources.OffSmall;
            this.sec3.Location = new System.Drawing.Point(228, 479);
            this.sec3.Name = "sec3";
            this.sec3.Size = new System.Drawing.Size(30, 23);
            this.sec3.TabIndex = 226;
            this.sec3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // sec2
            // 
            this.sec2.BackColor = System.Drawing.SystemColors.Control;
            this.sec2.Cursor = System.Windows.Forms.Cursors.Default;
            this.sec2.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sec2.Image = global::RateController.Properties.Resources.OffSmall;
            this.sec2.Location = new System.Drawing.Point(192, 479);
            this.sec2.Name = "sec2";
            this.sec2.Size = new System.Drawing.Size(30, 23);
            this.sec2.TabIndex = 225;
            this.sec2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // sec1
            // 
            this.sec1.BackColor = System.Drawing.SystemColors.Control;
            this.sec1.Cursor = System.Windows.Forms.Cursors.Default;
            this.sec1.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sec1.Image = global::RateController.Properties.Resources.OffSmall;
            this.sec1.Location = new System.Drawing.Point(156, 479);
            this.sec1.Name = "sec1";
            this.sec1.Size = new System.Drawing.Size(30, 23);
            this.sec1.TabIndex = 224;
            this.sec1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // sec0
            // 
            this.sec0.BackColor = System.Drawing.SystemColors.Control;
            this.sec0.Cursor = System.Windows.Forms.Cursors.Default;
            this.sec0.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sec0.Image = global::RateController.Properties.Resources.OffSmall;
            this.sec0.Location = new System.Drawing.Point(120, 479);
            this.sec0.Name = "sec0";
            this.sec0.Size = new System.Drawing.Size(30, 23);
            this.sec0.TabIndex = 222;
            this.sec0.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // frmMenuMonitoring
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 630);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.wifiBar);
            this.Controls.Add(this.tbCountsRev);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.lbRPM);
            this.Controls.Add(this.lbErrorPercent);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.lbSections);
            this.Controls.Add(this.lbRateAppliedData);
            this.Controls.Add(this.lb33);
            this.Controls.Add(this.lbRateSetData);
            this.Controls.Add(this.lb32);
            this.Controls.Add(this.lbSpeedData);
            this.Controls.Add(this.lbSpeed);
            this.Controls.Add(this.lbWidthData);
            this.Controls.Add(this.lbWidth);
            this.Controls.Add(this.lbPWMdata);
            this.Controls.Add(this.lb34);
            this.Controls.Add(this.lbWorkRateData);
            this.Controls.Add(this.lbWorkRate);
            this.Controls.Add(this.sec15);
            this.Controls.Add(this.sec14);
            this.Controls.Add(this.sec13);
            this.Controls.Add(this.sec12);
            this.Controls.Add(this.sec11);
            this.Controls.Add(this.sec10);
            this.Controls.Add(this.sec9);
            this.Controls.Add(this.sec8);
            this.Controls.Add(this.sec7);
            this.Controls.Add(this.sec6);
            this.Controls.Add(this.sec5);
            this.Controls.Add(this.sec4);
            this.Controls.Add(this.sec3);
            this.Controls.Add(this.sec2);
            this.Controls.Add(this.sec1);
            this.Controls.Add(this.sec0);
            this.Controls.Add(this.lbProduct);
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.btnLeft);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMenuMonitoring";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuMonitoring";
            this.Activated += new System.EventHandler(this.frmMenuMonitoring_Activated);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMenuMonitoring_FormClosed);
            this.Load += new System.EventHandler(this.frmMenuMonitoring_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lbProduct;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ProgressBar wifiBar;
        private System.Windows.Forms.TextBox tbCountsRev;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label lbRPM;
        private System.Windows.Forms.Label lbErrorPercent;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label lbSections;
        private System.Windows.Forms.Label lbRateAppliedData;
        private System.Windows.Forms.Label lb33;
        private System.Windows.Forms.Label lbRateSetData;
        private System.Windows.Forms.Label lb32;
        private System.Windows.Forms.Label lbSpeedData;
        private System.Windows.Forms.Label lbSpeed;
        private System.Windows.Forms.Label lbWidthData;
        private System.Windows.Forms.Label lbWidth;
        private System.Windows.Forms.Label lbPWMdata;
        private System.Windows.Forms.Label lb34;
        private System.Windows.Forms.Label lbWorkRateData;
        private System.Windows.Forms.Label lbWorkRate;
        private System.Windows.Forms.Label sec15;
        private System.Windows.Forms.Label sec14;
        private System.Windows.Forms.Label sec13;
        private System.Windows.Forms.Label sec12;
        private System.Windows.Forms.Label sec11;
        private System.Windows.Forms.Label sec10;
        private System.Windows.Forms.Label sec9;
        private System.Windows.Forms.Label sec8;
        private System.Windows.Forms.Label sec7;
        private System.Windows.Forms.Label sec6;
        private System.Windows.Forms.Label sec5;
        private System.Windows.Forms.Label sec4;
        private System.Windows.Forms.Label sec3;
        private System.Windows.Forms.Label sec2;
        private System.Windows.Forms.Label sec1;
        private System.Windows.Forms.Label sec0;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
    }
}