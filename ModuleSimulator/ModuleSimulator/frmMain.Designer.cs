namespace ModuleSimulator
{
    partial class frmMain
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.sendTimer = new System.Windows.Forms.Timer(this.components);
            this.loopTimer = new System.Windows.Forms.Timer(this.components);
            this.lblSubnet = new System.Windows.Forms.Label();
            this.txtSubnet = new System.Windows.Forms.TextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ModuleID = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SensorID = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lbUPM = new System.Windows.Forms.Label();
            this.lbQuantity = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.lbHz = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lbPWM = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lbValve = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.Speed = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.Pressure = new System.Windows.Forms.NumericUpDown();
            this.ckWork = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label15 = new System.Windows.Forms.Label();
            this.MaxHz = new System.Windows.Forms.NumericUpDown();
            this.Noise = new System.Windows.Forms.NumericUpDown();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.ValveMS = new System.Windows.Forms.NumericUpDown();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label18 = new System.Windows.Forms.Label();
            this.lbSetRate = new System.Windows.Forms.Label();
            this.lbFlowCal = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.lbMaster = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.lbAuto = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.lbRelays = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.lbConfig = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.btn1 = new System.Windows.Forms.Button();
            this.btn2 = new System.Windows.Forms.Button();
            this.btn3 = new System.Windows.Forms.Button();
            this.btn4 = new System.Windows.Forms.Button();
            this.btn5 = new System.Windows.Forms.Button();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.ckEnabled = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ModuleID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SensorID)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Speed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Pressure)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MaxHz)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Noise)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ValveMS)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // sendTimer
            // 
            this.sendTimer.Interval = 200;
            this.sendTimer.Tick += new System.EventHandler(this.sendTimer_Tick);
            // 
            // loopTimer
            // 
            this.loopTimer.Interval = 50;
            this.loopTimer.Tick += new System.EventHandler(this.loopTimer_Tick);
            //
            // lblSubnet
            // 
            this.lblSubnet.AutoSize = true;
            this.lblSubnet.Location = new System.Drawing.Point(8, 12);
            this.lblSubnet.Name = "lblSubnet";
            this.lblSubnet.Size = new System.Drawing.Size(47, 15);
            this.lblSubnet.TabIndex = 0;
            this.lblSubnet.Text = "Subnet:";
            // 
            // txtSubnet
            // 
            this.txtSubnet.Location = new System.Drawing.Point(70, 10);
            this.txtSubnet.Name = "txtSubnet";
            this.txtSubnet.Size = new System.Drawing.Size(87, 23);
            this.txtSubnet.TabIndex = 1;
            this.txtSubnet.Text = "192.168.1";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(8, 548);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(80, 30);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "Start";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(96, 548);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(80, 30);
            this.btnStop.TabIndex = 4;
            this.btnStop.Text = "Stop";
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblStatus.Location = new System.Drawing.Point(190, 555);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(290, 18);
            this.lblStatus.TabIndex = 5;
            this.lblStatus.Text = "Stopped";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ckEnabled);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.SensorID);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.ModuleID);
            this.groupBox1.Location = new System.Drawing.Point(4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(462, 71);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Config";
            this.groupBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox1_Paint);
            // 
            // ModuleID
            // 
            this.ModuleID.Location = new System.Drawing.Point(215, 26);
            this.ModuleID.Name = "ModuleID";
            this.ModuleID.Size = new System.Drawing.Size(60, 23);
            this.ModuleID.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(144, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Module ID:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(322, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Sensor ID:";
            // 
            // SensorID
            // 
            this.SensorID.Location = new System.Drawing.Point(393, 24);
            this.SensorID.Name = "SensorID";
            this.SensorID.Size = new System.Drawing.Size(60, 23);
            this.SensorID.TabIndex = 2;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lbValve);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.lbPWM);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.lbHz);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.btnReset);
            this.groupBox2.Controls.Add(this.lbQuantity);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.lbUPM);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(4, 80);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(462, 100);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Sensor Output";
            this.groupBox2.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox1_Paint);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "Rate (UPM):";
            // 
            // lbUPM
            // 
            this.lbUPM.Location = new System.Drawing.Point(139, 29);
            this.lbUPM.Name = "lbUPM";
            this.lbUPM.Size = new System.Drawing.Size(54, 15);
            this.lbUPM.TabIndex = 3;
            this.lbUPM.Text = "0.0";
            this.lbUPM.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbQuantity
            // 
            this.lbQuantity.Location = new System.Drawing.Point(139, 60);
            this.lbQuantity.Name = "lbQuantity";
            this.lbQuantity.Size = new System.Drawing.Size(54, 15);
            this.lbQuantity.TabIndex = 5;
            this.lbQuantity.Text = "0.0";
            this.lbQuantity.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 60);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(127, 15);
            this.label6.TabIndex = 4;
            this.label6.Text = "Accumulated Quantity";
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(199, 56);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 6;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnResetQty_Click);
            // 
            // lbHz
            // 
            this.lbHz.Location = new System.Drawing.Point(299, 29);
            this.lbHz.Name = "lbHz";
            this.lbHz.Size = new System.Drawing.Size(34, 15);
            this.lbHz.TabIndex = 8;
            this.lbHz.Text = "0.0";
            this.lbHz.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(269, 29);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(24, 15);
            this.label8.TabIndex = 7;
            this.label8.Text = "Hz:";
            // 
            // lbPWM
            // 
            this.lbPWM.Location = new System.Drawing.Point(402, 29);
            this.lbPWM.Name = "lbPWM";
            this.lbPWM.Size = new System.Drawing.Size(34, 15);
            this.lbPWM.TabIndex = 10;
            this.lbPWM.Text = "0.0";
            this.lbPWM.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(357, 29);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(39, 15);
            this.label10.TabIndex = 9;
            this.label10.Text = "PWM:";
            // 
            // lbValve
            // 
            this.lbValve.Location = new System.Drawing.Point(402, 60);
            this.lbValve.Name = "lbValve";
            this.lbValve.Size = new System.Drawing.Size(34, 15);
            this.lbValve.TabIndex = 12;
            this.lbValve.Text = "0.0";
            this.lbValve.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(357, 60);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(37, 15);
            this.label12.TabIndex = 11;
            this.label12.Text = "Valve:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label17);
            this.groupBox3.Controls.Add(this.ValveMS);
            this.groupBox3.Controls.Add(this.label15);
            this.groupBox3.Controls.Add(this.MaxHz);
            this.groupBox3.Controls.Add(this.Noise);
            this.groupBox3.Controls.Add(this.label16);
            this.groupBox3.Location = new System.Drawing.Point(4, 186);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(462, 77);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Simulation";
            this.groupBox3.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox1_Paint);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(13, 35);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(42, 15);
            this.label13.TabIndex = 3;
            this.label13.Text = "Speed:";
            // 
            // Speed
            // 
            this.Speed.Location = new System.Drawing.Point(84, 31);
            this.Speed.Name = "Speed";
            this.Speed.Size = new System.Drawing.Size(60, 23);
            this.Speed.TabIndex = 2;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(169, 35);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(54, 15);
            this.label14.TabIndex = 5;
            this.label14.Text = "Pressure:";
            // 
            // Pressure
            // 
            this.Pressure.Location = new System.Drawing.Point(240, 31);
            this.Pressure.Name = "Pressure";
            this.Pressure.Size = new System.Drawing.Size(60, 23);
            this.Pressure.TabIndex = 4;
            // 
            // ckWork
            // 
            this.ckWork.AutoSize = true;
            this.ckWork.Location = new System.Drawing.Point(360, 33);
            this.ckWork.Name = "ckWork";
            this.ckWork.Size = new System.Drawing.Size(92, 19);
            this.ckWork.TabIndex = 6;
            this.ckWork.Text = "Work Switch";
            this.ckWork.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.ckWork);
            this.groupBox4.Controls.Add(this.label14);
            this.groupBox4.Controls.Add(this.Speed);
            this.groupBox4.Controls.Add(this.Pressure);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Location = new System.Drawing.Point(4, 269);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(462, 75);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Sensor Inputs";
            this.groupBox4.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox1_Paint);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(169, 34);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(53, 15);
            this.label15.TabIndex = 9;
            this.label15.Text = "Noise %:";
            // 
            // MaxHz
            // 
            this.MaxHz.Location = new System.Drawing.Point(84, 30);
            this.MaxHz.Name = "MaxHz";
            this.MaxHz.Size = new System.Drawing.Size(60, 23);
            this.MaxHz.TabIndex = 6;
            // 
            // Noise
            // 
            this.Noise.Location = new System.Drawing.Point(240, 30);
            this.Noise.Name = "Noise";
            this.Noise.Size = new System.Drawing.Size(60, 23);
            this.Noise.TabIndex = 8;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(13, 34);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(50, 15);
            this.label16.TabIndex = 7;
            this.label16.Text = "Max Hz:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(321, 34);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(56, 15);
            this.label17.TabIndex = 11;
            this.label17.Text = "Valve ms:";
            // 
            // ValveMS
            // 
            this.ValveMS.Location = new System.Drawing.Point(392, 30);
            this.ValveMS.Name = "ValveMS";
            this.ValveMS.Size = new System.Drawing.Size(60, 23);
            this.ValveMS.TabIndex = 10;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.lbConfig);
            this.groupBox5.Controls.Add(this.label29);
            this.groupBox5.Controls.Add(this.lbRelays);
            this.groupBox5.Controls.Add(this.label27);
            this.groupBox5.Controls.Add(this.lbAuto);
            this.groupBox5.Controls.Add(this.label25);
            this.groupBox5.Controls.Add(this.lbMaster);
            this.groupBox5.Controls.Add(this.label23);
            this.groupBox5.Controls.Add(this.lbFlowCal);
            this.groupBox5.Controls.Add(this.label21);
            this.groupBox5.Controls.Add(this.lbSetRate);
            this.groupBox5.Controls.Add(this.label18);
            this.groupBox5.Location = new System.Drawing.Point(4, 350);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(462, 119);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Commands from RC";
            this.groupBox5.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox1_Paint);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(6, 28);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(52, 15);
            this.label18.TabIndex = 3;
            this.label18.Text = "Set Rate:";
            // 
            // lbSetRate
            // 
            this.lbSetRate.Location = new System.Drawing.Point(64, 28);
            this.lbSetRate.Name = "lbSetRate";
            this.lbSetRate.Size = new System.Drawing.Size(73, 15);
            this.lbSetRate.TabIndex = 4;
            this.lbSetRate.Text = "-----";
            // 
            // lbFlowCal
            // 
            this.lbFlowCal.Location = new System.Drawing.Point(277, 28);
            this.lbFlowCal.Name = "lbFlowCal";
            this.lbFlowCal.Size = new System.Drawing.Size(73, 15);
            this.lbFlowCal.TabIndex = 6;
            this.lbFlowCal.Text = "-----";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(219, 28);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(55, 15);
            this.label21.TabIndex = 5;
            this.label21.Text = "Flow Cal:";
            // 
            // lbMaster
            // 
            this.lbMaster.Location = new System.Drawing.Point(64, 56);
            this.lbMaster.Name = "lbMaster";
            this.lbMaster.Size = new System.Drawing.Size(73, 15);
            this.lbMaster.TabIndex = 8;
            this.lbMaster.Text = "off";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(6, 56);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(46, 15);
            this.label23.TabIndex = 7;
            this.label23.Text = "Master:";
            // 
            // lbAuto
            // 
            this.lbAuto.Location = new System.Drawing.Point(277, 56);
            this.lbAuto.Name = "lbAuto";
            this.lbAuto.Size = new System.Drawing.Size(73, 15);
            this.lbAuto.TabIndex = 10;
            this.lbAuto.Text = "off";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(219, 56);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(36, 15);
            this.label25.TabIndex = 9;
            this.label25.Text = "Auto:";
            // 
            // lbRelays
            // 
            this.lbRelays.Location = new System.Drawing.Point(64, 87);
            this.lbRelays.Name = "lbRelays";
            this.lbRelays.Size = new System.Drawing.Size(160, 15);
            this.lbRelays.TabIndex = 12;
            this.lbRelays.Text = "0000 0000 0000 0000";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(6, 87);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(43, 15);
            this.label27.TabIndex = 11;
            this.label27.Text = "Relays:";
            // 
            // lbConfig
            // 
            this.lbConfig.Location = new System.Drawing.Point(277, 87);
            this.lbConfig.Name = "lbConfig";
            this.lbConfig.Size = new System.Drawing.Size(73, 15);
            this.lbConfig.TabIndex = 14;
            this.lbConfig.Text = "-----";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(219, 87);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(46, 15);
            this.label29.TabIndex = 13;
            this.label29.Text = "Config:";
            // 
            // btn1
            // 
            this.btn1.Location = new System.Drawing.Point(8, 39);
            this.btn1.Name = "btn1";
            this.btn1.Size = new System.Drawing.Size(60, 23);
            this.btn1.TabIndex = 7;
            this.btn1.Text = "1";
            this.btn1.UseVisualStyleBackColor = true;
            this.btn1.Click += new System.EventHandler(this.btn1_Click);
            // 
            // btn2
            // 
            this.btn2.Location = new System.Drawing.Point(110, 39);
            this.btn2.Name = "btn2";
            this.btn2.Size = new System.Drawing.Size(60, 23);
            this.btn2.TabIndex = 8;
            this.btn2.Text = "2";
            this.btn2.UseVisualStyleBackColor = true;
            this.btn2.Click += new System.EventHandler(this.btn2_Click);
            // 
            // btn3
            // 
            this.btn3.Location = new System.Drawing.Point(212, 39);
            this.btn3.Name = "btn3";
            this.btn3.Size = new System.Drawing.Size(60, 23);
            this.btn3.TabIndex = 9;
            this.btn3.Text = "3";
            this.btn3.UseVisualStyleBackColor = true;
            this.btn3.Click += new System.EventHandler(this.btn3_Click);
            // 
            // btn4
            // 
            this.btn4.Location = new System.Drawing.Point(314, 39);
            this.btn4.Name = "btn4";
            this.btn4.Size = new System.Drawing.Size(60, 23);
            this.btn4.TabIndex = 10;
            this.btn4.Text = "4";
            this.btn4.UseVisualStyleBackColor = true;
            this.btn4.Click += new System.EventHandler(this.btn4_Click);
            // 
            // btn5
            // 
            this.btn5.Location = new System.Drawing.Point(416, 39);
            this.btn5.Name = "btn5";
            this.btn5.Size = new System.Drawing.Size(60, 23);
            this.btn5.TabIndex = 11;
            this.btn5.Text = "5";
            this.btn5.UseVisualStyleBackColor = true;
            this.btn5.Click += new System.EventHandler(this.btn5_Click);
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.groupBox1);
            this.pnlMain.Controls.Add(this.groupBox2);
            this.pnlMain.Controls.Add(this.groupBox3);
            this.pnlMain.Controls.Add(this.groupBox4);
            this.pnlMain.Controls.Add(this.groupBox5);
            this.pnlMain.Location = new System.Drawing.Point(8, 68);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(472, 474);
            this.pnlMain.TabIndex = 13;
            // 
            // ckEnabled
            // 
            this.ckEnabled.AutoSize = true;
            this.ckEnabled.Location = new System.Drawing.Point(31, 27);
            this.ckEnabled.Name = "ckEnabled";
            this.ckEnabled.Size = new System.Drawing.Size(68, 19);
            this.ckEnabled.TabIndex = 7;
            this.ckEnabled.Text = "Enabled";
            this.ckEnabled.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            this.ClientSize = new System.Drawing.Size(491, 586);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.btn5);
            this.Controls.Add(this.btn4);
            this.Controls.Add(this.btn3);
            this.Controls.Add(this.btn2);
            this.Controls.Add(this.btn1);
            this.Controls.Add(this.lblSubnet);
            this.Controls.Add(this.txtSubnet);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.lblStatus);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Module Simulator";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ModuleID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SensorID)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Speed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Pressure)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MaxHz)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Noise)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ValveMS)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.pnlMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        // ── Control declarations ──────────────────────────────────────────────────
        private System.Windows.Forms.Timer        sendTimer;
        private System.Windows.Forms.Timer        loopTimer;
        private System.Windows.Forms.Label        lblSubnet;
        private System.Windows.Forms.TextBox      txtSubnet;
        private System.Windows.Forms.Button       btnStart;
        private System.Windows.Forms.Button       btnStop;
        private System.Windows.Forms.Label        lblStatus;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown SensorID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown ModuleID;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lbQuantity;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lbUPM;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbValve;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lbPWM;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lbHz;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown Pressure;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown Speed;
        private System.Windows.Forms.CheckBox ckWork;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.NumericUpDown ValveMS;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.NumericUpDown MaxHz;
        private System.Windows.Forms.NumericUpDown Noise;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label lbSetRate;
        private System.Windows.Forms.Label lbConfig;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label lbRelays;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label lbAuto;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label lbMaster;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label lbFlowCal;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Button btn1;
        private System.Windows.Forms.Button btn2;
        private System.Windows.Forms.Button btn3;
        private System.Windows.Forms.Button btn4;
        private System.Windows.Forms.Button btn5;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.CheckBox ckEnabled;
    }
}
