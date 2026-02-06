namespace RateController.Forms
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.lbUnits = new System.Windows.Forms.Label();
            this.lbTarget = new System.Windows.Forms.Label();
            this.lbQuantity = new System.Windows.Forms.Label();
            this.lbCoverage = new System.Windows.Forms.Label();
            this.Fans = new System.Windows.Forms.PictureBox();
            this.prod5 = new System.Windows.Forms.PictureBox();
            this.lbQuantityAmount = new System.Windows.Forms.Label();
            this.pnlQuantity = new System.Windows.Forms.Panel();
            this.pbQuantity = new RateController.VerticalProgressBar();
            this.lbCoverageAmount = new System.Windows.Forms.Label();
            this.lbTargetAmount = new System.Windows.Forms.Label();
            this.lbRateAmount = new System.Windows.Forms.Label();
            this.lbQuantityType = new System.Windows.Forms.Label();
            this.lbCoverageType = new System.Windows.Forms.Label();
            this.lbTargetType = new System.Windows.Forms.Label();
            this.lbRateType = new System.Windows.Forms.Label();
            this.prod1 = new System.Windows.Forms.PictureBox();
            this.prod4 = new System.Windows.Forms.PictureBox();
            this.prod3 = new System.Windows.Forms.PictureBox();
            this.prod2 = new System.Windows.Forms.PictureBox();
            this.btnMaster = new System.Windows.Forms.Button();
            this.btnAuto = new System.Windows.Forms.Button();
            this.btnMinimize = new System.Windows.Forms.Button();
            this.btnMenu = new System.Windows.Forms.Button();
            this.btnVR = new System.Windows.Forms.Button();
            this.lbProductName = new System.Windows.Forms.Label();
            this.butPowerOff = new System.Windows.Forms.Button();
            this.btAlarm = new System.Windows.Forms.Button();
            this.pbAOGstatus = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.Fans)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.prod5)).BeginInit();
            this.pnlQuantity.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.prod1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.prod4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.prod3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.prod2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAOGstatus)).BeginInit();
            this.SuspendLayout();
            // 
            // lbUnits
            // 
            this.lbUnits.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbUnits.Location = new System.Drawing.Point(2, 109);
            this.lbUnits.Name = "lbUnits";
            this.lbUnits.Size = new System.Drawing.Size(146, 30);
            this.lbUnits.TabIndex = 428;
            this.lbUnits.Text = "Current Rate";
            this.lbUnits.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbTarget
            // 
            this.lbTarget.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTarget.Location = new System.Drawing.Point(2, 143);
            this.lbTarget.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbTarget.Name = "lbTarget";
            this.lbTarget.Size = new System.Drawing.Size(146, 30);
            this.lbTarget.TabIndex = 425;
            this.lbTarget.Text = "Target Rate";
            this.lbTarget.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbTarget.Click += new System.EventHandler(this.lbTarget_Click);
            // 
            // lbQuantity
            // 
            this.lbQuantity.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbQuantity.Location = new System.Drawing.Point(2, 177);
            this.lbQuantity.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbQuantity.Name = "lbQuantity";
            this.lbQuantity.Size = new System.Drawing.Size(146, 30);
            this.lbQuantity.TabIndex = 426;
            this.lbQuantity.Text = "Quantity";
            this.lbQuantity.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbQuantity.Click += new System.EventHandler(this.lbQuantity_Click);
            // 
            // lbCoverage
            // 
            this.lbCoverage.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCoverage.Location = new System.Drawing.Point(2, 211);
            this.lbCoverage.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbCoverage.Name = "lbCoverage";
            this.lbCoverage.Size = new System.Drawing.Size(146, 30);
            this.lbCoverage.TabIndex = 427;
            this.lbCoverage.Text = "Applied";
            this.lbCoverage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbCoverage.Click += new System.EventHandler(this.lbCoverage_Click);
            // 
            // Fans
            // 
            this.Fans.BackColor = System.Drawing.Color.Transparent;
            this.Fans.Image = global::RateController.Properties.Resources.OffFans;
            this.Fans.Location = new System.Drawing.Point(317, 8);
            this.Fans.Name = "Fans";
            this.Fans.Size = new System.Drawing.Size(61, 59);
            this.Fans.TabIndex = 416;
            this.Fans.TabStop = false;
            this.Fans.Click += new System.EventHandler(this.Fans_Click);
            this.Fans.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox3_MouseDown);
            this.Fans.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox3_MouseMove);
            // 
            // prod5
            // 
            this.prod5.BackColor = System.Drawing.Color.Transparent;
            this.prod5.Image = global::RateController.Properties.Resources.Offp5;
            this.prod5.Location = new System.Drawing.Point(256, 8);
            this.prod5.Name = "prod5";
            this.prod5.Size = new System.Drawing.Size(61, 59);
            this.prod5.TabIndex = 415;
            this.prod5.TabStop = false;
            this.prod5.Click += new System.EventHandler(this.Prod5_Click);
            this.prod5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox3_MouseDown);
            this.prod5.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox3_MouseMove);
            // 
            // lbQuantityAmount
            // 
            this.lbQuantityAmount.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbQuantityAmount.Location = new System.Drawing.Point(150, 177);
            this.lbQuantityAmount.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbQuantityAmount.Name = "lbQuantityAmount";
            this.lbQuantityAmount.Size = new System.Drawing.Size(98, 30);
            this.lbQuantityAmount.TabIndex = 204;
            this.lbQuantityAmount.Text = "500,000.0";
            this.lbQuantityAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lbQuantityAmount.Click += new System.EventHandler(this.lbQuantityAmount_Click);
            this.lbQuantityAmount.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseDown);
            this.lbQuantityAmount.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseMove);
            // 
            // pnlQuantity
            // 
            this.pnlQuantity.BackColor = System.Drawing.Color.Transparent;
            this.pnlQuantity.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlQuantity.Controls.Add(this.pbQuantity);
            this.pnlQuantity.Location = new System.Drawing.Point(335, 75);
            this.pnlQuantity.Name = "pnlQuantity";
            this.pnlQuantity.Size = new System.Drawing.Size(38, 166);
            this.pnlQuantity.TabIndex = 431;
            // 
            // pbQuantity
            // 
            this.pbQuantity.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
            this.pbQuantity.ForeColor = System.Drawing.Color.LimeGreen;
            this.pbQuantity.Location = new System.Drawing.Point(0, 1);
            this.pbQuantity.Name = "pbQuantity";
            this.pbQuantity.Size = new System.Drawing.Size(38, 166);
            this.pbQuantity.TabIndex = 0;
            this.pbQuantity.Tag = "0";
            this.pbQuantity.Value = 95;
            this.pbQuantity.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseDown);
            this.pbQuantity.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseMove);
            // 
            // lbCoverageAmount
            // 
            this.lbCoverageAmount.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCoverageAmount.Location = new System.Drawing.Point(150, 211);
            this.lbCoverageAmount.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbCoverageAmount.Name = "lbCoverageAmount";
            this.lbCoverageAmount.Size = new System.Drawing.Size(98, 30);
            this.lbCoverageAmount.TabIndex = 205;
            this.lbCoverageAmount.Text = "142.8";
            this.lbCoverageAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lbCoverageAmount.Click += new System.EventHandler(this.lbCoverageAmount_Click);
            this.lbCoverageAmount.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseDown);
            this.lbCoverageAmount.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseMove);
            // 
            // lbTargetAmount
            // 
            this.lbTargetAmount.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTargetAmount.Location = new System.Drawing.Point(150, 143);
            this.lbTargetAmount.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbTargetAmount.Name = "lbTargetAmount";
            this.lbTargetAmount.Size = new System.Drawing.Size(98, 30);
            this.lbTargetAmount.TabIndex = 206;
            this.lbTargetAmount.Text = "7.8";
            this.lbTargetAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lbTargetAmount.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseDown);
            this.lbTargetAmount.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseMove);
            // 
            // lbRateAmount
            // 
            this.lbRateAmount.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRateAmount.Location = new System.Drawing.Point(150, 109);
            this.lbRateAmount.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbRateAmount.Name = "lbRateAmount";
            this.lbRateAmount.Size = new System.Drawing.Size(98, 30);
            this.lbRateAmount.TabIndex = 207;
            this.lbRateAmount.Text = "7.5";
            this.lbRateAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lbRateAmount.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseDown);
            this.lbRateAmount.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseMove);
            // 
            // lbQuantityType
            // 
            this.lbQuantityType.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbQuantityType.Location = new System.Drawing.Point(250, 177);
            this.lbQuantityType.Name = "lbQuantityType";
            this.lbQuantityType.Size = new System.Drawing.Size(82, 30);
            this.lbQuantityType.TabIndex = 211;
            this.lbQuantityType.Text = "Lbs";
            this.lbQuantityType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbCoverageType
            // 
            this.lbCoverageType.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCoverageType.Location = new System.Drawing.Point(250, 211);
            this.lbCoverageType.Name = "lbCoverageType";
            this.lbCoverageType.Size = new System.Drawing.Size(82, 30);
            this.lbCoverageType.TabIndex = 210;
            this.lbCoverageType.Text = "Acres";
            this.lbCoverageType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbTargetType
            // 
            this.lbTargetType.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTargetType.Location = new System.Drawing.Point(250, 143);
            this.lbTargetType.Name = "lbTargetType";
            this.lbTargetType.Size = new System.Drawing.Size(82, 30);
            this.lbTargetType.TabIndex = 209;
            this.lbTargetType.Text = "Lbs/Ac";
            this.lbTargetType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbTargetType.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseDown);
            this.lbTargetType.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseMove);
            // 
            // lbRateType
            // 
            this.lbRateType.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRateType.Location = new System.Drawing.Point(250, 109);
            this.lbRateType.Name = "lbRateType";
            this.lbRateType.Size = new System.Drawing.Size(82, 30);
            this.lbRateType.TabIndex = 208;
            this.lbRateType.Text = "Lbs/Ac";
            this.lbRateType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbRateType.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseDown);
            this.lbRateType.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseMove);
            // 
            // prod1
            // 
            this.prod1.BackColor = System.Drawing.Color.Transparent;
            this.prod1.Image = global::RateController.Properties.Resources.Offp1;
            this.prod1.Location = new System.Drawing.Point(12, 8);
            this.prod1.Name = "prod1";
            this.prod1.Size = new System.Drawing.Size(61, 59);
            this.prod1.TabIndex = 417;
            this.prod1.TabStop = false;
            this.prod1.Click += new System.EventHandler(this.prod1_Click);
            this.prod1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox3_MouseDown);
            this.prod1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox3_MouseMove);
            // 
            // prod4
            // 
            this.prod4.BackColor = System.Drawing.Color.Transparent;
            this.prod4.Image = global::RateController.Properties.Resources.Offp4;
            this.prod4.Location = new System.Drawing.Point(195, 8);
            this.prod4.Name = "prod4";
            this.prod4.Size = new System.Drawing.Size(61, 59);
            this.prod4.TabIndex = 423;
            this.prod4.TabStop = false;
            this.prod4.Click += new System.EventHandler(this.prod4_Click);
            this.prod4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox3_MouseDown);
            this.prod4.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox3_MouseMove);
            // 
            // prod3
            // 
            this.prod3.BackColor = System.Drawing.Color.Transparent;
            this.prod3.Image = global::RateController.Properties.Resources.Offp3;
            this.prod3.Location = new System.Drawing.Point(134, 8);
            this.prod3.Name = "prod3";
            this.prod3.Size = new System.Drawing.Size(61, 59);
            this.prod3.TabIndex = 422;
            this.prod3.TabStop = false;
            this.prod3.Click += new System.EventHandler(this.prod3_Click);
            this.prod3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox3_MouseDown);
            this.prod3.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox3_MouseMove);
            // 
            // prod2
            // 
            this.prod2.BackColor = System.Drawing.Color.Transparent;
            this.prod2.Image = global::RateController.Properties.Resources.Offp2;
            this.prod2.Location = new System.Drawing.Point(73, 8);
            this.prod2.Name = "prod2";
            this.prod2.Size = new System.Drawing.Size(61, 59);
            this.prod2.TabIndex = 421;
            this.prod2.TabStop = false;
            this.prod2.Click += new System.EventHandler(this.prod2_Click);
            this.prod2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox3_MouseDown);
            this.prod2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox3_MouseMove);
            // 
            // btnMaster
            // 
            this.btnMaster.BackColor = System.Drawing.Color.Transparent;
            this.btnMaster.FlatAppearance.BorderSize = 0;
            this.btnMaster.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkSlateBlue;
            this.btnMaster.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMaster.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMaster.Image = global::RateController.Properties.Resources.SprayOff;
            this.btnMaster.Location = new System.Drawing.Point(321, 248);
            this.btnMaster.Name = "btnMaster";
            this.btnMaster.Size = new System.Drawing.Size(60, 60);
            this.btnMaster.TabIndex = 419;
            this.btnMaster.UseVisualStyleBackColor = false;
            this.btnMaster.Click += new System.EventHandler(this.btnMaster_Click);
            this.btnMaster.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnMaster_MouseUp);
            // 
            // btnAuto
            // 
            this.btnAuto.BackColor = System.Drawing.Color.Transparent;
            this.btnAuto.FlatAppearance.BorderSize = 0;
            this.btnAuto.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkSlateBlue;
            this.btnAuto.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAuto.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAuto.Image = global::RateController.Properties.Resources.AutoOff;
            this.btnAuto.Location = new System.Drawing.Point(258, 248);
            this.btnAuto.Name = "btnAuto";
            this.btnAuto.Size = new System.Drawing.Size(60, 60);
            this.btnAuto.TabIndex = 418;
            this.btnAuto.UseVisualStyleBackColor = false;
            this.btnAuto.Click += new System.EventHandler(this.btnAuto_Click);
            // 
            // btnMinimize
            // 
            this.btnMinimize.BackColor = System.Drawing.Color.Transparent;
            this.btnMinimize.FlatAppearance.BorderSize = 0;
            this.btnMinimize.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkSlateBlue;
            this.btnMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimize.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMinimize.Image = global::RateController.Properties.Resources.arrow_circle_down_right;
            this.btnMinimize.Location = new System.Drawing.Point(69, 248);
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.Size = new System.Drawing.Size(60, 60);
            this.btnMinimize.TabIndex = 414;
            this.btnMinimize.UseVisualStyleBackColor = false;
            this.btnMinimize.Click += new System.EventHandler(this.btnMinimize_Click);
            // 
            // btnMenu
            // 
            this.btnMenu.BackColor = System.Drawing.Color.Transparent;
            this.btnMenu.FlatAppearance.BorderSize = 0;
            this.btnMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMenu.Image = global::RateController.Properties.Resources.article;
            this.btnMenu.Location = new System.Drawing.Point(132, 248);
            this.btnMenu.Name = "btnMenu";
            this.btnMenu.Size = new System.Drawing.Size(60, 60);
            this.btnMenu.TabIndex = 413;
            this.btnMenu.UseVisualStyleBackColor = false;
            this.btnMenu.Click += new System.EventHandler(this.btnMenu_Click);
            // 
            // btnVR
            // 
            this.btnVR.BackColor = System.Drawing.Color.Transparent;
            this.btnVR.FlatAppearance.BorderSize = 0;
            this.btnVR.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkSlateBlue;
            this.btnVR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVR.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVR.Image = global::RateController.Properties.Resources.VRon;
            this.btnVR.Location = new System.Drawing.Point(195, 248);
            this.btnVR.Name = "btnVR";
            this.btnVR.Size = new System.Drawing.Size(60, 60);
            this.btnVR.TabIndex = 429;
            this.btnVR.UseVisualStyleBackColor = false;
            this.btnVR.Click += new System.EventHandler(this.btnVR_Click);
            // 
            // lbProductName
            // 
            this.lbProductName.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbProductName.Location = new System.Drawing.Point(88, 75);
            this.lbProductName.Name = "lbProductName";
            this.lbProductName.Size = new System.Drawing.Size(241, 27);
            this.lbProductName.TabIndex = 1;
            this.lbProductName.Text = "Sprayer";
            this.lbProductName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbProductName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseDown);
            this.lbProductName.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseMove);
            // 
            // butPowerOff
            // 
            this.butPowerOff.BackColor = System.Drawing.Color.Transparent;
            this.butPowerOff.FlatAppearance.BorderSize = 0;
            this.butPowerOff.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkSlateBlue;
            this.butPowerOff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butPowerOff.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.butPowerOff.Image = global::RateController.Properties.Resources.SwitchOff;
            this.butPowerOff.Location = new System.Drawing.Point(6, 248);
            this.butPowerOff.Name = "butPowerOff";
            this.butPowerOff.Size = new System.Drawing.Size(60, 60);
            this.butPowerOff.TabIndex = 434;
            this.butPowerOff.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.butPowerOff.UseVisualStyleBackColor = false;
            this.butPowerOff.Click += new System.EventHandler(this.butPowerOff_Click);
            // 
            // btAlarm
            // 
            this.btAlarm.BackColor = System.Drawing.Color.Red;
            this.btAlarm.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btAlarm.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btAlarm.Location = new System.Drawing.Point(93, 71);
            this.btAlarm.Name = "btAlarm";
            this.btAlarm.Size = new System.Drawing.Size(236, 32);
            this.btAlarm.TabIndex = 430;
            this.btAlarm.Text = "Rate Alarm";
            this.btAlarm.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btAlarm.UseVisualStyleBackColor = false;
            this.btAlarm.Visible = false;
            this.btAlarm.Click += new System.EventHandler(this.btAlarm_Click);
            // 
            // pbAOGstatus
            // 
            this.pbAOGstatus.Image = global::RateController.Properties.Resources.AOG_Off;
            this.pbAOGstatus.Location = new System.Drawing.Point(12, 73);
            this.pbAOGstatus.Name = "pbAOGstatus";
            this.pbAOGstatus.Size = new System.Drawing.Size(70, 29);
            this.pbAOGstatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbAOGstatus.TabIndex = 435;
            this.pbAOGstatus.TabStop = false;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(237)))), ((int)(((byte)(197)))));
            this.ClientSize = new System.Drawing.Size(389, 316);
            this.Controls.Add(this.pbAOGstatus);
            this.Controls.Add(this.pnlQuantity);
            this.Controls.Add(this.lbQuantityAmount);
            this.Controls.Add(this.butPowerOff);
            this.Controls.Add(this.lbCoverageAmount);
            this.Controls.Add(this.lbTargetAmount);
            this.Controls.Add(this.btnVR);
            this.Controls.Add(this.lbRateAmount);
            this.Controls.Add(this.lbUnits);
            this.Controls.Add(this.lbQuantityType);
            this.Controls.Add(this.lbTarget);
            this.Controls.Add(this.lbCoverageType);
            this.Controls.Add(this.lbQuantity);
            this.Controls.Add(this.lbTargetType);
            this.Controls.Add(this.lbRateType);
            this.Controls.Add(this.lbCoverage);
            this.Controls.Add(this.Fans);
            this.Controls.Add(this.prod5);
            this.Controls.Add(this.prod1);
            this.Controls.Add(this.prod4);
            this.Controls.Add(this.prod3);
            this.Controls.Add(this.prod2);
            this.Controls.Add(this.btnMaster);
            this.Controls.Add(this.btnAuto);
            this.Controls.Add(this.btnMinimize);
            this.Controls.Add(this.btnMenu);
            this.Controls.Add(this.lbProductName);
            this.Controls.Add(this.btAlarm);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "frmMain";
            this.Text = "Rate Controller";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMainDisplay_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this.Fans)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.prod5)).EndInit();
            this.pnlQuantity.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.prod1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.prod4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.prod3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.prod2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAOGstatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lbUnits;
        private System.Windows.Forms.Label lbTarget;
        private System.Windows.Forms.Label lbQuantity;
        private System.Windows.Forms.Label lbCoverage;
        private System.Windows.Forms.PictureBox Fans;
        private System.Windows.Forms.PictureBox prod5;
        private System.Windows.Forms.Label lbQuantityAmount;
        private System.Windows.Forms.Label lbCoverageAmount;
        private System.Windows.Forms.Label lbTargetAmount;
        private System.Windows.Forms.Label lbRateAmount;
        private System.Windows.Forms.Label lbQuantityType;
        private System.Windows.Forms.Label lbCoverageType;
        private System.Windows.Forms.Label lbTargetType;
        private System.Windows.Forms.Label lbRateType;
        private System.Windows.Forms.PictureBox prod1;
        private System.Windows.Forms.PictureBox prod4;
        private System.Windows.Forms.PictureBox prod3;
        private System.Windows.Forms.PictureBox prod2;
        private System.Windows.Forms.Button btnMaster;
        private System.Windows.Forms.Button btnAuto;
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.Button btnMenu;
        private VerticalProgressBar pbQuantity;
        private System.Windows.Forms.Button btnVR;
        private System.Windows.Forms.Panel pnlQuantity;
        private System.Windows.Forms.Label lbProductName;
        private System.Windows.Forms.Button butPowerOff;
        private System.Windows.Forms.Button btAlarm;
        private System.Windows.Forms.PictureBox pbAOGstatus;
    }
}