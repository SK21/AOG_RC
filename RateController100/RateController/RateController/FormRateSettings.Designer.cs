namespace RateController
{
    partial class FormRateSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRateSettings));
            this.butLoadDefaults = new System.Windows.Forms.Button();
            this.tbMaxPWM = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbMinPWM = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbDeadband = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbKD = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbKI = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbKP = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.butResetApplied = new System.Windows.Forms.Button();
            this.butResetAcres = new System.Windows.Forms.Button();
            this.TankRemain = new System.Windows.Forms.TextBox();
            this.butResetTank = new System.Windows.Forms.Button();
            this.label34 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.bntOK = new System.Windows.Forms.Button();
            this.TankSize = new System.Windows.Forms.TextBox();
            this.label32 = new System.Windows.Forms.Label();
            this.FlowCal = new System.Windows.Forms.TextBox();
            this.label30 = new System.Windows.Forms.Label();
            this.VolumeUnits = new System.Windows.Forms.ComboBox();
            this.label28 = new System.Windows.Forms.Label();
            this.AreaUnits = new System.Windows.Forms.ComboBox();
            this.label27 = new System.Windows.Forms.Label();
            this.RateSet = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ValveType = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.lblCurrentBaud = new System.Windows.Forms.Label();
            this.cboxBaud = new System.Windows.Forms.ComboBox();
            this.lbArduinoConnected = new System.Windows.Forms.Label();
            this.btnRescan = new System.Windows.Forms.Button();
            this.btnOpenSerialArduino = new System.Windows.Forms.Button();
            this.btnCloseSerialArduino = new System.Windows.Forms.Button();
            this.cboxArdPort = new System.Windows.Forms.ComboBox();
            this.lblCurrentArduinoPort = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.label10 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.label43 = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.label45 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.lbLocalIP = new System.Windows.Forms.Label();
            this.btnDay = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lbVersion = new System.Windows.Forms.Label();
            this.lbNetworkIP = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // butLoadDefaults
            // 
            this.butLoadDefaults.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.butLoadDefaults.Location = new System.Drawing.Point(177, 417);
            this.butLoadDefaults.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.butLoadDefaults.Name = "butLoadDefaults";
            this.butLoadDefaults.Size = new System.Drawing.Size(160, 89);
            this.butLoadDefaults.TabIndex = 112;
            this.butLoadDefaults.Text = "Load PID Defaults";
            this.butLoadDefaults.UseVisualStyleBackColor = true;
            this.butLoadDefaults.Click += new System.EventHandler(this.butLoadDefaults_Click);
            // 
            // tbMaxPWM
            // 
            this.tbMaxPWM.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbMaxPWM.Location = new System.Drawing.Point(299, 289);
            this.tbMaxPWM.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbMaxPWM.MaxLength = 8;
            this.tbMaxPWM.Name = "tbMaxPWM";
            this.tbMaxPWM.Size = new System.Drawing.Size(64, 36);
            this.tbMaxPWM.TabIndex = 90;
            this.tbMaxPWM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbMaxPWM.TextChanged += new System.EventHandler(this.tbMaxPWM_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(133, 294);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(126, 29);
            this.label9.TabIndex = 111;
            this.label9.Text = "Max. PWM";
            // 
            // tbMinPWM
            // 
            this.tbMinPWM.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbMinPWM.Location = new System.Drawing.Point(299, 239);
            this.tbMinPWM.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbMinPWM.MaxLength = 8;
            this.tbMinPWM.Name = "tbMinPWM";
            this.tbMinPWM.Size = new System.Drawing.Size(64, 36);
            this.tbMinPWM.TabIndex = 89;
            this.tbMinPWM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbMinPWM.TextChanged += new System.EventHandler(this.tbMinPWM_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(133, 244);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(119, 29);
            this.label8.TabIndex = 110;
            this.label8.Text = "Min. PWM";
            // 
            // tbDeadband
            // 
            this.tbDeadband.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDeadband.Location = new System.Drawing.Point(299, 188);
            this.tbDeadband.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbDeadband.MaxLength = 8;
            this.tbDeadband.Name = "tbDeadband";
            this.tbDeadband.Size = new System.Drawing.Size(64, 36);
            this.tbDeadband.TabIndex = 88;
            this.tbDeadband.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbDeadband.TextChanged += new System.EventHandler(this.tbDeadband_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(133, 193);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(120, 29);
            this.label7.TabIndex = 109;
            this.label7.Text = "Deadband";
            // 
            // tbKD
            // 
            this.tbKD.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbKD.Location = new System.Drawing.Point(299, 138);
            this.tbKD.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbKD.MaxLength = 8;
            this.tbKD.Name = "tbKD";
            this.tbKD.Size = new System.Drawing.Size(64, 36);
            this.tbKD.TabIndex = 87;
            this.tbKD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbKD.TextChanged += new System.EventHandler(this.tbKD_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(133, 143);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 29);
            this.label6.TabIndex = 108;
            this.label6.Text = "KD";
            // 
            // tbKI
            // 
            this.tbKI.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbKI.Location = new System.Drawing.Point(299, 87);
            this.tbKI.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbKI.MaxLength = 8;
            this.tbKI.Name = "tbKI";
            this.tbKI.Size = new System.Drawing.Size(64, 36);
            this.tbKI.TabIndex = 86;
            this.tbKI.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbKI.TextChanged += new System.EventHandler(this.tbKI_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(133, 92);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 29);
            this.label5.TabIndex = 107;
            this.label5.Text = "KI";
            // 
            // tbKP
            // 
            this.tbKP.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbKP.Location = new System.Drawing.Point(299, 37);
            this.tbKP.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbKP.MaxLength = 8;
            this.tbKP.Name = "tbKP";
            this.tbKP.Size = new System.Drawing.Size(64, 36);
            this.tbKP.TabIndex = 85;
            this.tbKP.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbKP.TextChanged += new System.EventHandler(this.tbKP_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(136, 42);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 29);
            this.label2.TabIndex = 105;
            this.label2.Text = "KP";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(344, 348);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(18, 17);
            this.checkBox1.TabIndex = 91;
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(127, 345);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(160, 29);
            this.label1.TabIndex = 104;
            this.label1.Text = "Simulate Flow";
            // 
            // butResetApplied
            // 
            this.butResetApplied.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.butResetApplied.Location = new System.Drawing.Point(376, 442);
            this.butResetApplied.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.butResetApplied.Name = "butResetApplied";
            this.butResetApplied.Size = new System.Drawing.Size(160, 89);
            this.butResetApplied.TabIndex = 103;
            this.butResetApplied.Text = "Reset Quantity";
            this.butResetApplied.UseVisualStyleBackColor = true;
            this.butResetApplied.Click += new System.EventHandler(this.butResetApplied_Click);
            // 
            // butResetAcres
            // 
            this.butResetAcres.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.butResetAcres.Location = new System.Drawing.Point(15, 442);
            this.butResetAcres.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.butResetAcres.Name = "butResetAcres";
            this.butResetAcres.Size = new System.Drawing.Size(160, 89);
            this.butResetAcres.TabIndex = 102;
            this.butResetAcres.Text = "Reset Coverage";
            this.butResetAcres.UseVisualStyleBackColor = true;
            this.butResetAcres.Click += new System.EventHandler(this.butResetAcres_Click);
            // 
            // TankRemain
            // 
            this.TankRemain.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TankRemain.Location = new System.Drawing.Point(400, 391);
            this.TankRemain.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TankRemain.MaxLength = 8;
            this.TankRemain.Name = "TankRemain";
            this.TankRemain.Size = new System.Drawing.Size(135, 36);
            this.TankRemain.TabIndex = 84;
            this.TankRemain.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.TankRemain.TextChanged += new System.EventHandler(this.TankRemain_TextChanged);
            this.TankRemain.Validating += new System.ComponentModel.CancelEventHandler(this.TankRemain_Validating);
            // 
            // butResetTank
            // 
            this.butResetTank.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.butResetTank.Location = new System.Drawing.Point(195, 442);
            this.butResetTank.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.butResetTank.Name = "butResetTank";
            this.butResetTank.Size = new System.Drawing.Size(160, 89);
            this.butResetTank.TabIndex = 100;
            this.butResetTank.Text = "Reset Tank";
            this.butResetTank.UseVisualStyleBackColor = true;
            this.butResetTank.Click += new System.EventHandler(this.butResetTank_Click);
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label34.Location = new System.Drawing.Point(16, 396);
            this.label34.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(183, 29);
            this.label34.TabIndex = 101;
            this.label34.Text = "Tank Remaining";
            // 
            // btnCancel
            // 
            this.btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnCancel.Enabled = false;
            this.btnCancel.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCancel.Location = new System.Drawing.Point(233, 608);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(160, 89);
            this.btnCancel.TabIndex = 92;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // bntOK
            // 
            this.bntOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bntOK.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.bntOK.Image = ((System.Drawing.Image)(resources.GetObject("bntOK.Image")));
            this.bntOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.bntOK.Location = new System.Drawing.Point(413, 608);
            this.bntOK.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bntOK.Name = "bntOK";
            this.bntOK.Size = new System.Drawing.Size(160, 89);
            this.bntOK.TabIndex = 93;
            this.bntOK.Text = "Close";
            this.bntOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.bntOK.UseVisualStyleBackColor = true;
            this.bntOK.Click += new System.EventHandler(this.bntOK_Click);
            // 
            // TankSize
            // 
            this.TankSize.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TankSize.Location = new System.Drawing.Point(400, 330);
            this.TankSize.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TankSize.MaxLength = 8;
            this.TankSize.Name = "TankSize";
            this.TankSize.Size = new System.Drawing.Size(135, 36);
            this.TankSize.TabIndex = 83;
            this.TankSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.TankSize.TextChanged += new System.EventHandler(this.TankSize_TextChanged);
            this.TankSize.Validating += new System.ComponentModel.CancelEventHandler(this.TankSize_Validating);
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.Location = new System.Drawing.Point(16, 335);
            this.label32.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(115, 29);
            this.label32.TabIndex = 99;
            this.label32.Text = "Tank Size";
            // 
            // FlowCal
            // 
            this.FlowCal.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FlowCal.Location = new System.Drawing.Point(400, 207);
            this.FlowCal.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.FlowCal.MaxLength = 8;
            this.FlowCal.Name = "FlowCal";
            this.FlowCal.Size = new System.Drawing.Size(135, 36);
            this.FlowCal.TabIndex = 81;
            this.FlowCal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.FlowCal.TextChanged += new System.EventHandler(this.FlowCal_TextChanged);
            this.FlowCal.Validating += new System.ComponentModel.CancelEventHandler(this.FlowCal_Validating);
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label30.Location = new System.Drawing.Point(16, 212);
            this.label30.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(127, 29);
            this.label30.TabIndex = 98;
            this.label30.Text = "Rate Cal #";
            // 
            // VolumeUnits
            // 
            this.VolumeUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.VolumeUnits.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VolumeUnits.FormattingEnabled = true;
            this.VolumeUnits.Items.AddRange(new object[] {
            "Imp. Gallons",
            "US Gallons",
            "Lbs",
            "Lbs NH3",
            "Litres",
            "Kgs",
            "Kgs NH3"});
            this.VolumeUnits.Location = new System.Drawing.Point(321, 22);
            this.VolumeUnits.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.VolumeUnits.Name = "VolumeUnits";
            this.VolumeUnits.Size = new System.Drawing.Size(213, 37);
            this.VolumeUnits.TabIndex = 78;
            this.VolumeUnits.SelectedIndexChanged += new System.EventHandler(this.VolumeUnits_SelectedIndexChanged);
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.Location = new System.Drawing.Point(16, 27);
            this.label28.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(102, 29);
            this.label28.TabIndex = 97;
            this.label28.Text = "Quantity";
            // 
            // AreaUnits
            // 
            this.AreaUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AreaUnits.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AreaUnits.FormattingEnabled = true;
            this.AreaUnits.Items.AddRange(new object[] {
            "Acre",
            "Hectare",
            "Minute",
            "Hour"});
            this.AreaUnits.Location = new System.Drawing.Point(321, 84);
            this.AreaUnits.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.AreaUnits.Name = "AreaUnits";
            this.AreaUnits.Size = new System.Drawing.Size(213, 37);
            this.AreaUnits.TabIndex = 79;
            this.AreaUnits.SelectedIndexChanged += new System.EventHandler(this.AreaUnits_SelectedIndexChanged);
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label27.Location = new System.Drawing.Point(16, 89);
            this.label27.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(113, 29);
            this.label27.TabIndex = 96;
            this.label27.Text = "Coverage";
            // 
            // RateSet
            // 
            this.RateSet.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RateSet.Location = new System.Drawing.Point(400, 145);
            this.RateSet.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.RateSet.MaxLength = 8;
            this.RateSet.Name = "RateSet";
            this.RateSet.Size = new System.Drawing.Size(135, 36);
            this.RateSet.TabIndex = 80;
            this.RateSet.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.RateSet.TextChanged += new System.EventHandler(this.RateSet_TextChanged);
            this.RateSet.Validating += new System.ComponentModel.CancelEventHandler(this.RateSet_Validating);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.Location = new System.Drawing.Point(16, 150);
            this.label21.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(104, 29);
            this.label21.TabIndex = 95;
            this.label21.Text = "Rate Set";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(16, 273);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(130, 29);
            this.label3.TabIndex = 94;
            this.label3.Text = "Valve Type";
            // 
            // ValveType
            // 
            this.ValveType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ValveType.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ValveType.FormattingEnabled = true;
            this.ValveType.Items.AddRange(new object[] {
            "Standard",
            "Fast Close"});
            this.ValveType.Location = new System.Drawing.Point(321, 268);
            this.ValveType.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ValveType.Name = "ValveType";
            this.ValveType.Size = new System.Drawing.Size(213, 37);
            this.ValveType.TabIndex = 82;
            this.ValveType.SelectedIndexChanged += new System.EventHandler(this.ValveType_SelectedIndexChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(19, 12);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(560, 588);
            this.tabControl1.TabIndex = 122;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.VolumeUnits);
            this.tabPage1.Controls.Add(this.ValveType);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label21);
            this.tabPage1.Controls.Add(this.RateSet);
            this.tabPage1.Controls.Add(this.label27);
            this.tabPage1.Controls.Add(this.AreaUnits);
            this.tabPage1.Controls.Add(this.label28);
            this.tabPage1.Controls.Add(this.label30);
            this.tabPage1.Controls.Add(this.FlowCal);
            this.tabPage1.Controls.Add(this.label32);
            this.tabPage1.Controls.Add(this.TankSize);
            this.tabPage1.Controls.Add(this.label34);
            this.tabPage1.Controls.Add(this.TankRemain);
            this.tabPage1.Controls.Add(this.butResetApplied);
            this.tabPage1.Controls.Add(this.butResetTank);
            this.tabPage1.Controls.Add(this.butResetAcres);
            this.tabPage1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPage1.Location = new System.Drawing.Point(4, 41);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage1.Size = new System.Drawing.Size(552, 543);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Main";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.tbKP);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.checkBox1);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.tbKI);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.tbKD);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.tbDeadband);
            this.tabPage2.Controls.Add(this.butLoadDefaults);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.tbMaxPWM);
            this.tabPage2.Controls.Add(this.tbMinPWM);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Location = new System.Drawing.Point(4, 41);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage2.Size = new System.Drawing.Size(552, 543);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "PID";
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage3.Controls.Add(this.lblCurrentBaud);
            this.tabPage3.Controls.Add(this.cboxBaud);
            this.tabPage3.Controls.Add(this.lbArduinoConnected);
            this.tabPage3.Controls.Add(this.btnRescan);
            this.tabPage3.Controls.Add(this.btnOpenSerialArduino);
            this.tabPage3.Controls.Add(this.btnCloseSerialArduino);
            this.tabPage3.Controls.Add(this.cboxArdPort);
            this.tabPage3.Controls.Add(this.lblCurrentArduinoPort);
            this.tabPage3.Location = new System.Drawing.Point(4, 41);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(552, 543);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Serial";
            // 
            // lblCurrentBaud
            // 
            this.lblCurrentBaud.AutoSize = true;
            this.lblCurrentBaud.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentBaud.Location = new System.Drawing.Point(105, 260);
            this.lblCurrentBaud.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCurrentBaud.Name = "lblCurrentBaud";
            this.lblCurrentBaud.Size = new System.Drawing.Size(66, 29);
            this.lblCurrentBaud.TabIndex = 74;
            this.lblCurrentBaud.Text = "Baud";
            // 
            // cboxBaud
            // 
            this.cboxBaud.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.cboxBaud.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxBaud.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold);
            this.cboxBaud.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.cboxBaud.FormattingEnabled = true;
            this.cboxBaud.Items.AddRange(new object[] {
            "4800",
            "9600",
            "19200",
            "38400",
            "57600",
            "115200"});
            this.cboxBaud.Location = new System.Drawing.Point(208, 250);
            this.cboxBaud.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboxBaud.Name = "cboxBaud";
            this.cboxBaud.Size = new System.Drawing.Size(168, 44);
            this.cboxBaud.TabIndex = 73;
            this.cboxBaud.SelectedIndexChanged += new System.EventHandler(this.cboxBaud_SelectedIndexChanged);
            // 
            // lbArduinoConnected
            // 
            this.lbArduinoConnected.BackColor = System.Drawing.Color.Red;
            this.lbArduinoConnected.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbArduinoConnected.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbArduinoConnected.Location = new System.Drawing.Point(16, 503);
            this.lbArduinoConnected.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbArduinoConnected.Name = "lbArduinoConnected";
            this.lbArduinoConnected.Size = new System.Drawing.Size(517, 28);
            this.lbArduinoConnected.TabIndex = 72;
            this.lbArduinoConnected.Text = "Port Disconnected";
            this.lbArduinoConnected.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnRescan
            // 
            this.btnRescan.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnRescan.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnRescan.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRescan.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnRescan.Location = new System.Drawing.Point(16, 395);
            this.btnRescan.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnRescan.Name = "btnRescan";
            this.btnRescan.Size = new System.Drawing.Size(160, 89);
            this.btnRescan.TabIndex = 69;
            this.btnRescan.Text = "Rescan Ports";
            this.btnRescan.UseVisualStyleBackColor = false;
            this.btnRescan.Click += new System.EventHandler(this.btnRescan_Click);
            // 
            // btnOpenSerialArduino
            // 
            this.btnOpenSerialArduino.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnOpenSerialArduino.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnOpenSerialArduino.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenSerialArduino.Location = new System.Drawing.Point(373, 395);
            this.btnOpenSerialArduino.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnOpenSerialArduino.Name = "btnOpenSerialArduino";
            this.btnOpenSerialArduino.Size = new System.Drawing.Size(160, 89);
            this.btnOpenSerialArduino.TabIndex = 68;
            this.btnOpenSerialArduino.Text = "Connect";
            this.btnOpenSerialArduino.UseVisualStyleBackColor = false;
            this.btnOpenSerialArduino.Click += new System.EventHandler(this.btnOpenSerialArduino_Click);
            // 
            // btnCloseSerialArduino
            // 
            this.btnCloseSerialArduino.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnCloseSerialArduino.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCloseSerialArduino.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCloseSerialArduino.Location = new System.Drawing.Point(195, 395);
            this.btnCloseSerialArduino.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCloseSerialArduino.Name = "btnCloseSerialArduino";
            this.btnCloseSerialArduino.Size = new System.Drawing.Size(160, 89);
            this.btnCloseSerialArduino.TabIndex = 67;
            this.btnCloseSerialArduino.Text = "Disconnect";
            this.btnCloseSerialArduino.UseVisualStyleBackColor = false;
            this.btnCloseSerialArduino.Click += new System.EventHandler(this.btnCloseSerialArduino_Click);
            // 
            // cboxArdPort
            // 
            this.cboxArdPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxArdPort.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold);
            this.cboxArdPort.FormattingEnabled = true;
            this.cboxArdPort.Location = new System.Drawing.Point(208, 171);
            this.cboxArdPort.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboxArdPort.Name = "cboxArdPort";
            this.cboxArdPort.Size = new System.Drawing.Size(168, 44);
            this.cboxArdPort.TabIndex = 66;
            this.cboxArdPort.SelectedIndexChanged += new System.EventHandler(this.cboxArdPort_SelectedIndexChanged);
            // 
            // lblCurrentArduinoPort
            // 
            this.lblCurrentArduinoPort.AutoSize = true;
            this.lblCurrentArduinoPort.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentArduinoPort.Location = new System.Drawing.Point(105, 181);
            this.lblCurrentArduinoPort.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCurrentArduinoPort.Name = "lblCurrentArduinoPort";
            this.lblCurrentArduinoPort.Size = new System.Drawing.Size(56, 29);
            this.lblCurrentArduinoPort.TabIndex = 65;
            this.lblCurrentArduinoPort.Text = "Port";
            this.lblCurrentArduinoPort.Click += new System.EventHandler(this.lblCurrentArduinoPort_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage4.Controls.Add(this.label10);
            this.tabPage4.Controls.Add(this.label31);
            this.tabPage4.Controls.Add(this.label22);
            this.tabPage4.Controls.Add(this.label23);
            this.tabPage4.Controls.Add(this.label24);
            this.tabPage4.Controls.Add(this.label25);
            this.tabPage4.Controls.Add(this.label26);
            this.tabPage4.Controls.Add(this.label29);
            this.tabPage4.Controls.Add(this.label20);
            this.tabPage4.Controls.Add(this.label12);
            this.tabPage4.Controls.Add(this.label18);
            this.tabPage4.Controls.Add(this.label19);
            this.tabPage4.Controls.Add(this.label42);
            this.tabPage4.Controls.Add(this.label43);
            this.tabPage4.Controls.Add(this.label44);
            this.tabPage4.Controls.Add(this.label45);
            this.tabPage4.Controls.Add(this.label40);
            this.tabPage4.Controls.Add(this.lbLocalIP);
            this.tabPage4.Controls.Add(this.btnDay);
            this.tabPage4.Controls.Add(this.label4);
            this.tabPage4.Controls.Add(this.label11);
            this.tabPage4.Controls.Add(this.lbVersion);
            this.tabPage4.Controls.Add(this.lbNetworkIP);
            this.tabPage4.Controls.Add(this.label17);
            this.tabPage4.Controls.Add(this.label14);
            this.tabPage4.Controls.Add(this.label15);
            this.tabPage4.Controls.Add(this.label13);
            this.tabPage4.Controls.Add(this.label16);
            this.tabPage4.Location = new System.Drawing.Point(4, 41);
            this.tabPage4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(552, 543);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Misc";
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Tahoma", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(4, 353);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(541, 28);
            this.label10.TabIndex = 143;
            this.label10.Text = "Day/Night";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label31
            // 
            this.label31.Font = new System.Drawing.Font("Tahoma", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label31.Location = new System.Drawing.Point(4, 175);
            this.label31.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(541, 28);
            this.label31.TabIndex = 142;
            this.label31.Text = "Local UDP";
            this.label31.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.Location = new System.Drawing.Point(73, 217);
            this.label22.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(127, 29);
            this.label22.TabIndex = 139;
            this.label22.Text = "IP Address";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(73, 260);
            this.label23.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(116, 29);
            this.label23.TabIndex = 140;
            this.label23.Text = "Send Port";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.Location = new System.Drawing.Point(73, 300);
            this.label24.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(146, 29);
            this.label24.TabIndex = 141;
            this.label24.Text = "Receive Port";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(73, 217);
            this.label25.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(0, 29);
            this.label25.TabIndex = 136;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.Location = new System.Drawing.Point(75, 257);
            this.label26.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(0, 29);
            this.label26.TabIndex = 137;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label29.Location = new System.Drawing.Point(73, 298);
            this.label29.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(0, 29);
            this.label29.TabIndex = 138;
            // 
            // label20
            // 
            this.label20.Font = new System.Drawing.Font("Tahoma", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(4, 5);
            this.label20.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(541, 28);
            this.label20.TabIndex = 135;
            this.label20.Text = "Network UDP";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(73, 49);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(127, 29);
            this.label12.TabIndex = 132;
            this.label12.Text = "IP Address";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(73, 90);
            this.label18.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(116, 29);
            this.label18.TabIndex = 133;
            this.label18.Text = "Send Port";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(73, 130);
            this.label19.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(146, 29);
            this.label19.TabIndex = 134;
            this.label19.Text = "Receive Port";
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label42.Location = new System.Drawing.Point(99, 260);
            this.label42.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(0, 29);
            this.label42.TabIndex = 128;
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label43.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label43.Location = new System.Drawing.Point(345, 299);
            this.label43.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(67, 31);
            this.label43.TabIndex = 131;
            this.label43.Text = "8888";
            this.label43.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label44.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label44.Location = new System.Drawing.Point(345, 258);
            this.label44.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(67, 31);
            this.label44.TabIndex = 129;
            this.label44.Text = "9999";
            this.label44.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label45.Location = new System.Drawing.Point(97, 300);
            this.label45.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(0, 29);
            this.label45.TabIndex = 130;
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label40.Location = new System.Drawing.Point(97, 217);
            this.label40.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(0, 29);
            this.label40.TabIndex = 125;
            // 
            // lbLocalIP
            // 
            this.lbLocalIP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbLocalIP.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbLocalIP.Location = new System.Drawing.Point(241, 215);
            this.lbLocalIP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbLocalIP.Name = "lbLocalIP";
            this.lbLocalIP.Size = new System.Drawing.Size(173, 30);
            this.lbLocalIP.TabIndex = 126;
            this.lbLocalIP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnDay
            // 
            this.btnDay.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnDay.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnDay.Image = ((System.Drawing.Image)(resources.GetObject("btnDay.Image")));
            this.btnDay.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnDay.Location = new System.Drawing.Point(193, 385);
            this.btnDay.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnDay.Name = "btnDay";
            this.btnDay.Size = new System.Drawing.Size(160, 89);
            this.btnDay.TabIndex = 123;
            this.btnDay.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnDay.UseVisualStyleBackColor = true;
            this.btnDay.Click += new System.EventHandler(this.btnDay_Click);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Tahoma", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(96, 5);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(317, 28);
            this.label4.TabIndex = 122;
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(96, 49);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(0, 29);
            this.label11.TabIndex = 114;
            // 
            // lbVersion
            // 
            this.lbVersion.AutoSize = true;
            this.lbVersion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbVersion.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbVersion.Location = new System.Drawing.Point(285, 496);
            this.lbVersion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbVersion.Name = "lbVersion";
            this.lbVersion.Size = new System.Drawing.Size(115, 25);
            this.lbVersion.TabIndex = 121;
            this.lbVersion.Text = "10/Jan/2020";
            // 
            // lbNetworkIP
            // 
            this.lbNetworkIP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbNetworkIP.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbNetworkIP.Location = new System.Drawing.Point(241, 48);
            this.lbNetworkIP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbNetworkIP.Name = "lbNetworkIP";
            this.lbNetworkIP.Size = new System.Drawing.Size(173, 30);
            this.lbNetworkIP.TabIndex = 115;
            this.lbNetworkIP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(99, 497);
            this.label17.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(116, 23);
            this.label17.TabIndex = 120;
            this.label17.Text = "Version Date";
            this.label17.Click += new System.EventHandler(this.label17_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(97, 90);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(0, 29);
            this.label14.TabIndex = 116;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label15.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(345, 129);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(67, 31);
            this.label15.TabIndex = 119;
            this.label15.Text = "8000";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label13.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(345, 89);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(67, 31);
            this.label13.TabIndex = 117;
            this.label13.Text = "9999";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(96, 130);
            this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(0, 29);
            this.label16.TabIndex = 118;
            // 
            // FormRateSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(591, 708);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.bntOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.Name = "FormRateSettings";
            this.ShowInTaskbar = false;
            this.Text = "Rate Settings";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormRateSettings_FormClosed);
            this.Load += new System.EventHandler(this.FormRateSettings_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button butLoadDefaults;
        private System.Windows.Forms.TextBox tbMaxPWM;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbMinPWM;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbDeadband;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbKD;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbKI;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbKP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button butResetApplied;
        private System.Windows.Forms.Button butResetAcres;
        private System.Windows.Forms.TextBox TankRemain;
        private System.Windows.Forms.Button butResetTank;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button bntOK;
        private System.Windows.Forms.TextBox TankSize;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.TextBox FlowCal;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.ComboBox VolumeUnits;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.ComboBox AreaUnits;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.TextBox RateSet;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox ValveType;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button btnRescan;
        private System.Windows.Forms.Button btnOpenSerialArduino;
        private System.Windows.Forms.Button btnCloseSerialArduino;
        private System.Windows.Forms.ComboBox cboxArdPort;
        private System.Windows.Forms.Label lblCurrentArduinoPort;
        private System.Windows.Forms.Label lbArduinoConnected;
        private System.Windows.Forms.Label lblCurrentBaud;
        private System.Windows.Forms.ComboBox cboxBaud;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.Label label43;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.Label label45;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.Label lbLocalIP;
        private System.Windows.Forms.Button btnDay;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lbVersion;
        private System.Windows.Forms.Label lbNetworkIP;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label10;
    }
}