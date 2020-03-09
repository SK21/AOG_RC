namespace AgOpenGPS.Forms
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
            this.components = new System.ComponentModel.Container();
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
            this.bntOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.TankRemain = new System.Windows.Forms.TextBox();
            this.butResetTank = new System.Windows.Forms.Button();
            this.label34 = new System.Windows.Forms.Label();
            this.butResetApplied = new System.Windows.Forms.Button();
            this.butResetAcres = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.tbKP = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbKI = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbKD = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbDeadband = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbMinPWM = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbMaxPWM = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.butLoadDefaults = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TankSize
            // 
            this.TankSize.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TankSize.Location = new System.Drawing.Point(207, 256);
            this.TankSize.MaxLength = 8;
            this.TankSize.Name = "TankSize";
            this.TankSize.Size = new System.Drawing.Size(102, 30);
            this.TankSize.TabIndex = 6;
            this.TankSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.TankSize.TextChanged += new System.EventHandler(this.TankSize_TextChanged);
            this.TankSize.Validating += new System.ComponentModel.CancelEventHandler(this.TankSize_Validating);
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.Location = new System.Drawing.Point(10, 260);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(90, 23);
            this.label32.TabIndex = 54;
            this.label32.Text = "Tank Size";
            // 
            // FlowCal
            // 
            this.FlowCal.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FlowCal.Location = new System.Drawing.Point(206, 156);
            this.FlowCal.MaxLength = 8;
            this.FlowCal.Name = "FlowCal";
            this.FlowCal.Size = new System.Drawing.Size(102, 30);
            this.FlowCal.TabIndex = 3;
            this.FlowCal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.FlowCal.TextChanged += new System.EventHandler(this.FlowCal_TextChanged);
            this.FlowCal.Validating += new System.ComponentModel.CancelEventHandler(this.FlowCal_Validating);
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label30.Location = new System.Drawing.Point(10, 160);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(128, 23);
            this.label30.TabIndex = 52;
            this.label30.Text = "Flowmeter Cal";
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
            this.VolumeUnits.Location = new System.Drawing.Point(147, 6);
            this.VolumeUnits.Name = "VolumeUnits";
            this.VolumeUnits.Size = new System.Drawing.Size(161, 31);
            this.VolumeUnits.TabIndex = 0;
            this.VolumeUnits.SelectedIndexChanged += new System.EventHandler(this.VolumeUnits_SelectedIndexChanged);
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.Location = new System.Drawing.Point(10, 10);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(80, 23);
            this.label28.TabIndex = 50;
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
            this.AreaUnits.Location = new System.Drawing.Point(147, 56);
            this.AreaUnits.Name = "AreaUnits";
            this.AreaUnits.Size = new System.Drawing.Size(161, 31);
            this.AreaUnits.TabIndex = 1;
            this.AreaUnits.SelectedIndexChanged += new System.EventHandler(this.AreaUnits_SelectedIndexChanged);
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label27.Location = new System.Drawing.Point(10, 60);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(88, 23);
            this.label27.TabIndex = 48;
            this.label27.Text = "Coverage";
            // 
            // RateSet
            // 
            this.RateSet.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RateSet.Location = new System.Drawing.Point(206, 106);
            this.RateSet.MaxLength = 8;
            this.RateSet.Name = "RateSet";
            this.RateSet.Size = new System.Drawing.Size(102, 30);
            this.RateSet.TabIndex = 2;
            this.RateSet.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.RateSet.TextChanged += new System.EventHandler(this.RateSet_TextChanged);
            this.RateSet.Validating += new System.ComponentModel.CancelEventHandler(this.RateSet_Validating);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.Location = new System.Drawing.Point(10, 110);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(81, 23);
            this.label21.TabIndex = 44;
            this.label21.Text = "Rate Set";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(10, 210);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 23);
            this.label3.TabIndex = 43;
            this.label3.Text = "Valve Type";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // ValveType
            // 
            this.ValveType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ValveType.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ValveType.FormattingEnabled = true;
            this.ValveType.Items.AddRange(new object[] {
            "Standard",
            "Fast Close"});
            this.ValveType.Location = new System.Drawing.Point(147, 206);
            this.ValveType.Name = "ValveType";
            this.ValveType.Size = new System.Drawing.Size(161, 31);
            this.ValveType.TabIndex = 4;
            this.ValveType.SelectedIndexChanged += new System.EventHandler(this.ValveType_SelectedIndexChanged);
            // 
            // bntOK
            // 
            this.bntOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bntOK.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.bntOK.Image = global::AgOpenGPS.Properties.Resources.OK64;
            this.bntOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.bntOK.Location = new System.Drawing.Point(641, 356);
            this.bntOK.Name = "bntOK";
            this.bntOK.Size = new System.Drawing.Size(120, 72);
            this.bntOK.TabIndex = 17;
            this.bntOK.Text = "Close";
            this.bntOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.bntOK.UseVisualStyleBackColor = true;
            this.bntOK.Click += new System.EventHandler(this.bntOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnCancel.Enabled = false;
            this.btnCancel.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnCancel.Image = global::AgOpenGPS.Properties.Resources.Cancel64;
            this.btnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCancel.Location = new System.Drawing.Point(515, 356);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(120, 72);
            this.btnCancel.TabIndex = 16;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // TankRemain
            // 
            this.TankRemain.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TankRemain.Location = new System.Drawing.Point(206, 306);
            this.TankRemain.MaxLength = 8;
            this.TankRemain.Name = "TankRemain";
            this.TankRemain.Size = new System.Drawing.Size(102, 30);
            this.TankRemain.TabIndex = 7;
            this.TankRemain.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.TankRemain.TextChanged += new System.EventHandler(this.TankRemain_TextChanged);
            this.TankRemain.Validating += new System.ComponentModel.CancelEventHandler(this.TankRemain_Validating);
            // 
            // butResetTank
            // 
            this.butResetTank.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.butResetTank.Location = new System.Drawing.Point(263, 356);
            this.butResetTank.Name = "butResetTank";
            this.butResetTank.Size = new System.Drawing.Size(120, 72);
            this.butResetTank.TabIndex = 58;
            this.butResetTank.Text = "Reset Tank";
            this.butResetTank.UseVisualStyleBackColor = true;
            this.butResetTank.Click += new System.EventHandler(this.butResetTank_Click);
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label34.Location = new System.Drawing.Point(10, 310);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(146, 23);
            this.label34.TabIndex = 60;
            this.label34.Text = "Tank Remaining";
            this.label34.Click += new System.EventHandler(this.label34_Click);
            // 
            // butResetApplied
            // 
            this.butResetApplied.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.butResetApplied.Location = new System.Drawing.Point(389, 356);
            this.butResetApplied.Name = "butResetApplied";
            this.butResetApplied.Size = new System.Drawing.Size(120, 72);
            this.butResetApplied.TabIndex = 62;
            this.butResetApplied.Text = "Reset Quantity";
            this.butResetApplied.UseVisualStyleBackColor = true;
            this.butResetApplied.Click += new System.EventHandler(this.butResetApplied_Click);
            // 
            // butResetAcres
            // 
            this.butResetAcres.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.butResetAcres.Location = new System.Drawing.Point(137, 356);
            this.butResetAcres.Name = "butResetAcres";
            this.butResetAcres.Size = new System.Drawing.Size(120, 72);
            this.butResetAcres.TabIndex = 61;
            this.butResetAcres.Text = "Reset Coverage";
            this.butResetAcres.UseVisualStyleBackColor = true;
            this.butResetAcres.Click += new System.EventHandler(this.butResetAcres_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(579, 312);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 23);
            this.label1.TabIndex = 63;
            this.label1.Text = "Simulate Flow";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(742, 316);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(15, 14);
            this.checkBox1.TabIndex = 15;
            this.toolTip1.SetToolTip(this.checkBox1, "simulate flow");
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // tbKP
            // 
            this.tbKP.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbKP.Location = new System.Drawing.Point(708, 62);
            this.tbKP.MaxLength = 8;
            this.tbKP.Name = "tbKP";
            this.tbKP.Size = new System.Drawing.Size(49, 30);
            this.tbKP.TabIndex = 9;
            this.tbKP.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.tbKP, "Proportional Gain 0-255 in tenths\r\nex: 1 = 0.1");
            this.tbKP.TextChanged += new System.EventHandler(this.tbKP_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(586, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 23);
            this.label2.TabIndex = 65;
            this.label2.Text = "KP";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 13.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(619, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 23);
            this.label4.TabIndex = 66;
            this.label4.Text = "PID values";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // tbKI
            // 
            this.tbKI.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbKI.Location = new System.Drawing.Point(708, 103);
            this.tbKI.MaxLength = 8;
            this.tbKI.Name = "tbKI";
            this.tbKI.Size = new System.Drawing.Size(49, 30);
            this.tbKI.TabIndex = 10;
            this.tbKI.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.tbKI, "Integeral Gain 0-255 ten thousandths\r\nex: 1 = 0.0001");
            this.tbKI.TextChanged += new System.EventHandler(this.tbKI_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(584, 107);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(28, 23);
            this.label5.TabIndex = 68;
            this.label5.Text = "KI";
            // 
            // tbKD
            // 
            this.tbKD.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbKD.Location = new System.Drawing.Point(708, 144);
            this.tbKD.MaxLength = 8;
            this.tbKD.Name = "tbKD";
            this.tbKD.Size = new System.Drawing.Size(49, 30);
            this.tbKD.TabIndex = 11;
            this.tbKD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.tbKD, "Derivative Gain 0-255 in tenths\r\nex: 1 = 0.1");
            this.tbKD.TextChanged += new System.EventHandler(this.tbKD_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(584, 148);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 23);
            this.label6.TabIndex = 70;
            this.label6.Text = "KD";
            // 
            // tbDeadband
            // 
            this.tbDeadband.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDeadband.Location = new System.Drawing.Point(708, 185);
            this.tbDeadband.MaxLength = 8;
            this.tbDeadband.Name = "tbDeadband";
            this.tbDeadband.Size = new System.Drawing.Size(49, 30);
            this.tbDeadband.TabIndex = 12;
            this.tbDeadband.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.tbDeadband, "% error that can still be present\r\n when the motor stops 0-15");
            this.tbDeadband.TextChanged += new System.EventHandler(this.tbDeadband_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(584, 189);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(97, 23);
            this.label7.TabIndex = 72;
            this.label7.Text = "Deadband";
            // 
            // tbMinPWM
            // 
            this.tbMinPWM.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbMinPWM.Location = new System.Drawing.Point(708, 226);
            this.tbMinPWM.MaxLength = 8;
            this.tbMinPWM.Name = "tbMinPWM";
            this.tbMinPWM.Size = new System.Drawing.Size(49, 30);
            this.tbMinPWM.TabIndex = 13;
            this.tbMinPWM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.tbMinPWM, "0-255");
            this.tbMinPWM.TextChanged += new System.EventHandler(this.tbMinPWM_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(584, 230);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(94, 23);
            this.label8.TabIndex = 74;
            this.label8.Text = "Min. PWM";
            // 
            // tbMaxPWM
            // 
            this.tbMaxPWM.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbMaxPWM.Location = new System.Drawing.Point(708, 267);
            this.tbMaxPWM.MaxLength = 8;
            this.tbMaxPWM.Name = "tbMaxPWM";
            this.tbMaxPWM.Size = new System.Drawing.Size(49, 30);
            this.tbMaxPWM.TabIndex = 14;
            this.tbMaxPWM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.tbMaxPWM, "0-255");
            this.tbMaxPWM.TextChanged += new System.EventHandler(this.tbMaxPWM_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(584, 271);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(98, 23);
            this.label9.TabIndex = 76;
            this.label9.Text = "Max. PWM";
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 5000;
            this.toolTip1.InitialDelay = 1000;
            this.toolTip1.ReshowDelay = 100;
            this.toolTip1.UseAnimation = false;
            this.toolTip1.UseFading = false;
            // 
            // butLoadDefaults
            // 
            this.butLoadDefaults.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.butLoadDefaults.Location = new System.Drawing.Point(11, 356);
            this.butLoadDefaults.Name = "butLoadDefaults";
            this.butLoadDefaults.Size = new System.Drawing.Size(120, 72);
            this.butLoadDefaults.TabIndex = 77;
            this.butLoadDefaults.Text = "Load PID Defaults";
            this.butLoadDefaults.UseVisualStyleBackColor = true;
            this.butLoadDefaults.Click += new System.EventHandler(this.butLoadDefaults_Click);
            // 
            // FormRateSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(769, 431);
            this.Controls.Add(this.butLoadDefaults);
            this.Controls.Add(this.tbMaxPWM);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.tbMinPWM);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.tbDeadband);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tbKD);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tbKI);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbKP);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.butResetApplied);
            this.Controls.Add(this.butResetAcres);
            this.Controls.Add(this.TankRemain);
            this.Controls.Add(this.butResetTank);
            this.Controls.Add(this.label34);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.bntOK);
            this.Controls.Add(this.TankSize);
            this.Controls.Add(this.label32);
            this.Controls.Add(this.FlowCal);
            this.Controls.Add(this.label30);
            this.Controls.Add(this.VolumeUnits);
            this.Controls.Add(this.label28);
            this.Controls.Add(this.AreaUnits);
            this.Controls.Add(this.label27);
            this.Controls.Add(this.RateSet);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ValveType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FormRateSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Rate Settings";
            this.Load += new System.EventHandler(this.FormRateSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

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
        private System.Windows.Forms.Button bntOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox TankRemain;
        private System.Windows.Forms.Button butResetTank;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Button butResetApplied;
        private System.Windows.Forms.Button butResetAcres;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox tbKP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbKI;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbKD;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbDeadband;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbMinPWM;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbMaxPWM;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button butLoadDefaults;
    }
}