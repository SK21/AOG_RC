namespace PCBsetup.Forms
{
    partial class frmPCBsettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPCBsettings));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tbRS485port = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tbWemosSerialPort = new System.Windows.Forms.TextBox();
            this.label31 = new System.Windows.Forms.Label();
            this.lbIPpart4 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.tbIPaddress = new System.Windows.Forms.TextBox();
            this.label29 = new System.Windows.Forms.Label();
            this.cbIMU = new System.Windows.Forms.ComboBox();
            this.tbZeroOffset = new System.Windows.Forms.TextBox();
            this.tbIMUinterval = new System.Windows.Forms.TextBox();
            this.tbIMUdelay = new System.Windows.Forms.TextBox();
            this.tbRTCM = new System.Windows.Forms.TextBox();
            this.tbRTCMserialPort = new System.Windows.Forms.TextBox();
            this.tbNMEAserialPort = new System.Windows.Forms.TextBox();
            this.cbReceiver = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label28 = new System.Windows.Forms.Label();
            this.cbRelayControl = new System.Windows.Forms.ComboBox();
            this.cbAnalog = new System.Windows.Forms.ComboBox();
            this.ckActuator = new System.Windows.Forms.CheckBox();
            this.ckInvertRoll = new System.Windows.Forms.CheckBox();
            this.ckSwapPitchRoll = new System.Windows.Forms.CheckBox();
            this.ckOnBoard = new System.Windows.Forms.CheckBox();
            this.ckFlowOn = new System.Windows.Forms.CheckBox();
            this.ckUseRate = new System.Windows.Forms.CheckBox();
            this.ckRelayOn = new System.Windows.Forms.CheckBox();
            this.ckGyro = new System.Windows.Forms.CheckBox();
            this.tbModule = new System.Windows.Forms.TextBox();
            this.tbPulseCal = new System.Windows.Forms.TextBox();
            this.tbMaxSpeed = new System.Windows.Forms.TextBox();
            this.tbMinSpeed = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tbCurrentSensor = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.tbSendEnable = new System.Windows.Forms.TextBox();
            this.tbSpeedPulse = new System.Windows.Forms.TextBox();
            this.tbPwm2 = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.tbDir2 = new System.Windows.Forms.TextBox();
            this.tbEncoder = new System.Windows.Forms.TextBox();
            this.tbPressureSensor = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.tbWorkSwitch = new System.Windows.Forms.TextBox();
            this.tbSteerRelay = new System.Windows.Forms.TextBox();
            this.tbWAS = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.tbSteerSwitch = new System.Windows.Forms.TextBox();
            this.tbPwm1 = new System.Windows.Forms.TextBox();
            this.tbDir1 = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.btnLoadDefaults = new System.Windows.Forms.Button();
            this.btnSendToModule = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.bntOK = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(754, 344);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tbRS485port);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.tbWemosSerialPort);
            this.tabPage1.Controls.Add(this.label31);
            this.tabPage1.Controls.Add(this.lbIPpart4);
            this.tabPage1.Controls.Add(this.label30);
            this.tabPage1.Controls.Add(this.tbIPaddress);
            this.tabPage1.Controls.Add(this.label29);
            this.tabPage1.Controls.Add(this.cbIMU);
            this.tabPage1.Controls.Add(this.tbZeroOffset);
            this.tabPage1.Controls.Add(this.tbIMUinterval);
            this.tabPage1.Controls.Add(this.tbIMUdelay);
            this.tabPage1.Controls.Add(this.tbRTCM);
            this.tabPage1.Controls.Add(this.tbRTCMserialPort);
            this.tabPage1.Controls.Add(this.tbNMEAserialPort);
            this.tabPage1.Controls.Add(this.cbReceiver);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 33);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(746, 307);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Config 1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tbRS485port
            // 
            this.tbRS485port.Location = new System.Drawing.Point(233, 209);
            this.tbRS485port.Name = "tbRS485port";
            this.tbRS485port.Size = new System.Drawing.Size(118, 29);
            this.tbRS485port.TabIndex = 27;
            this.tbRS485port.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbRS485port.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.tbRS485port_HelpRequested);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 211);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(151, 24);
            this.label10.TabIndex = 26;
            this.label10.Text = "RS485 serial port";
            // 
            // tbWemosSerialPort
            // 
            this.tbWemosSerialPort.Location = new System.Drawing.Point(233, 165);
            this.tbWemosSerialPort.Name = "tbWemosSerialPort";
            this.tbWemosSerialPort.Size = new System.Drawing.Size(118, 29);
            this.tbWemosSerialPort.TabIndex = 23;
            this.tbWemosSerialPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbWemosSerialPort.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.tbWemosSerialPort_HelpRequested);
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(7, 167);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(161, 24);
            this.label31.TabIndex = 22;
            this.label31.Text = "Wemos serial port";
            // 
            // lbIPpart4
            // 
            this.lbIPpart4.AutoSize = true;
            this.lbIPpart4.Location = new System.Drawing.Point(670, 211);
            this.lbIPpart4.Name = "lbIPpart4";
            this.lbIPpart4.Size = new System.Drawing.Size(45, 24);
            this.lbIPpart4.TabIndex = 21;
            this.lbIPpart4.Text = ".126";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(528, 211);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(80, 24);
            this.label30.TabIndex = 20;
            this.label30.Text = "192.168.";
            // 
            // tbIPaddress
            // 
            this.tbIPaddress.Location = new System.Drawing.Point(614, 209);
            this.tbIPaddress.Name = "tbIPaddress";
            this.tbIPaddress.Size = new System.Drawing.Size(50, 29);
            this.tbIPaddress.TabIndex = 19;
            this.tbIPaddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(387, 211);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(101, 24);
            this.label29.TabIndex = 18;
            this.label29.Text = "IP Address";
            // 
            // cbIMU
            // 
            this.cbIMU.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbIMU.FormattingEnabled = true;
            this.cbIMU.Items.AddRange(new object[] {
            "None",
            "Sparkfun BNO",
            "CMPS14",
            "Adafruit BNO",
            "Serial IMU"});
            this.cbIMU.Location = new System.Drawing.Point(544, 34);
            this.cbIMU.Name = "cbIMU";
            this.cbIMU.Size = new System.Drawing.Size(187, 32);
            this.cbIMU.TabIndex = 15;
            this.cbIMU.SelectedIndexChanged += new System.EventHandler(this.cbIMU_SelectedIndexChanged);
            // 
            // tbZeroOffset
            // 
            this.tbZeroOffset.Location = new System.Drawing.Point(614, 165);
            this.tbZeroOffset.Name = "tbZeroOffset";
            this.tbZeroOffset.Size = new System.Drawing.Size(118, 29);
            this.tbZeroOffset.TabIndex = 14;
            this.tbZeroOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbIMUinterval
            // 
            this.tbIMUinterval.Location = new System.Drawing.Point(614, 121);
            this.tbIMUinterval.Name = "tbIMUinterval";
            this.tbIMUinterval.Size = new System.Drawing.Size(118, 29);
            this.tbIMUinterval.TabIndex = 13;
            this.tbIMUinterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbIMUinterval.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.tbIMUinterval_HelpRequested);
            // 
            // tbIMUdelay
            // 
            this.tbIMUdelay.Location = new System.Drawing.Point(614, 77);
            this.tbIMUdelay.Name = "tbIMUdelay";
            this.tbIMUdelay.Size = new System.Drawing.Size(118, 29);
            this.tbIMUdelay.TabIndex = 12;
            this.tbIMUdelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbIMUdelay.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.tbIMUdelay_HelpRequested);
            // 
            // tbRTCM
            // 
            this.tbRTCM.Location = new System.Drawing.Point(233, 253);
            this.tbRTCM.Name = "tbRTCM";
            this.tbRTCM.Size = new System.Drawing.Size(118, 29);
            this.tbRTCM.TabIndex = 11;
            this.tbRTCM.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbRTCM.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.tbRTCM_HelpRequested);
            // 
            // tbRTCMserialPort
            // 
            this.tbRTCMserialPort.Location = new System.Drawing.Point(233, 122);
            this.tbRTCMserialPort.Name = "tbRTCMserialPort";
            this.tbRTCMserialPort.Size = new System.Drawing.Size(118, 29);
            this.tbRTCMserialPort.TabIndex = 10;
            this.tbRTCMserialPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbRTCMserialPort.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.tbRTCMserialPort_HelpRequested);
            // 
            // tbNMEAserialPort
            // 
            this.tbNMEAserialPort.Location = new System.Drawing.Point(233, 79);
            this.tbNMEAserialPort.Name = "tbNMEAserialPort";
            this.tbNMEAserialPort.Size = new System.Drawing.Size(118, 29);
            this.tbNMEAserialPort.TabIndex = 9;
            this.tbNMEAserialPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbNMEAserialPort.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.tbNMEAserialPort_HelpRequested);
            // 
            // cbReceiver
            // 
            this.cbReceiver.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbReceiver.FormattingEnabled = true;
            this.cbReceiver.Items.AddRange(new object[] {
            "None",
            "SimpleRTK2B",
            "Sparkfun F9P"});
            this.cbReceiver.Location = new System.Drawing.Point(164, 33);
            this.cbReceiver.Name = "cbReceiver";
            this.cbReceiver.Size = new System.Drawing.Size(187, 32);
            this.cbReceiver.TabIndex = 8;
            this.cbReceiver.SelectedIndexChanged += new System.EventHandler(this.cbReceiver_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(387, 167);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(143, 24);
            this.label8.TabIndex = 7;
            this.label8.Text = "WAS zero offset";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(387, 123);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(161, 24);
            this.label7.TabIndex = 6;
            this.label7.Text = "IMU report interval";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(387, 79);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(136, 24);
            this.label6.TabIndex = 5;
            this.label6.Text = "IMU read delay";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(387, 38);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 24);
            this.label5.TabIndex = 4;
            this.label5.Text = "IMU";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 255);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(159, 24);
            this.label4.TabIndex = 3;
            this.label4.Text = "RTCM UDP port #";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 123);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(150, 24);
            this.label3.TabIndex = 2;
            this.label3.Text = "RTCM serial port";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(152, 24);
            this.label2.TabIndex = 1;
            this.label2.Text = "NMEA serial port";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "GPS receiver";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label28);
            this.tabPage2.Controls.Add(this.cbRelayControl);
            this.tabPage2.Controls.Add(this.cbAnalog);
            this.tabPage2.Controls.Add(this.ckActuator);
            this.tabPage2.Controls.Add(this.ckInvertRoll);
            this.tabPage2.Controls.Add(this.ckSwapPitchRoll);
            this.tabPage2.Controls.Add(this.ckOnBoard);
            this.tabPage2.Controls.Add(this.ckFlowOn);
            this.tabPage2.Controls.Add(this.ckUseRate);
            this.tabPage2.Controls.Add(this.ckRelayOn);
            this.tabPage2.Controls.Add(this.ckGyro);
            this.tabPage2.Controls.Add(this.tbModule);
            this.tabPage2.Controls.Add(this.tbPulseCal);
            this.tabPage2.Controls.Add(this.tbMaxSpeed);
            this.tabPage2.Controls.Add(this.tbMinSpeed);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.label11);
            this.tabPage2.Controls.Add(this.label12);
            this.tabPage2.Controls.Add(this.label13);
            this.tabPage2.Controls.Add(this.label14);
            this.tabPage2.Location = new System.Drawing.Point(4, 33);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(746, 307);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Config 2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(412, 57);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(122, 24);
            this.label28.TabIndex = 39;
            this.label28.Text = "Relay Control";
            // 
            // cbRelayControl
            // 
            this.cbRelayControl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRelayControl.FormattingEnabled = true;
            this.cbRelayControl.Items.AddRange(new object[] {
            "No Relays",
            "RS485",
            "PCA9555  8 relays",
            "PCA9555  16 relays"});
            this.cbRelayControl.Location = new System.Drawing.Point(544, 53);
            this.cbRelayControl.Name = "cbRelayControl";
            this.cbRelayControl.Size = new System.Drawing.Size(187, 32);
            this.cbRelayControl.TabIndex = 38;
            this.cbRelayControl.SelectedIndexChanged += new System.EventHandler(this.cbRelayControl_SelectedIndexChanged_1);
            // 
            // cbAnalog
            // 
            this.cbAnalog.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAnalog.FormattingEnabled = true;
            this.cbAnalog.Items.AddRange(new object[] {
            "ADS1115 (Teensy)",
            "pins (Teensy)",
            "ADS1115 (D1 Mini)"});
            this.cbAnalog.Location = new System.Drawing.Point(544, 8);
            this.cbAnalog.Name = "cbAnalog";
            this.cbAnalog.Size = new System.Drawing.Size(187, 32);
            this.cbAnalog.TabIndex = 37;
            this.cbAnalog.SelectedIndexChanged += new System.EventHandler(this.cbAnalog_SelectedIndexChanged);
            this.cbAnalog.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.cbAnalog_HelpRequested);
            // 
            // ckActuator
            // 
            this.ckActuator.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckActuator.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ckActuator.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckActuator.Location = new System.Drawing.Point(601, 232);
            this.ckActuator.Name = "ckActuator";
            this.ckActuator.Size = new System.Drawing.Size(130, 69);
            this.ckActuator.TabIndex = 36;
            this.ckActuator.Text = "Use Actuator";
            this.ckActuator.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckActuator.UseVisualStyleBackColor = true;
            this.ckActuator.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ckActuator_HelpRequested);
            // 
            // ckInvertRoll
            // 
            this.ckInvertRoll.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckInvertRoll.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ckInvertRoll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckInvertRoll.Location = new System.Drawing.Point(10, 232);
            this.ckInvertRoll.Name = "ckInvertRoll";
            this.ckInvertRoll.Size = new System.Drawing.Size(130, 69);
            this.ckInvertRoll.TabIndex = 35;
            this.ckInvertRoll.Text = "Invert roll";
            this.ckInvertRoll.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckInvertRoll.UseVisualStyleBackColor = true;
            // 
            // ckSwapPitchRoll
            // 
            this.ckSwapPitchRoll.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckSwapPitchRoll.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ckSwapPitchRoll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckSwapPitchRoll.Location = new System.Drawing.Point(10, 146);
            this.ckSwapPitchRoll.Name = "ckSwapPitchRoll";
            this.ckSwapPitchRoll.Size = new System.Drawing.Size(130, 69);
            this.ckSwapPitchRoll.TabIndex = 34;
            this.ckSwapPitchRoll.Text = "Swap pitch roll";
            this.ckSwapPitchRoll.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckSwapPitchRoll.UseVisualStyleBackColor = true;
            // 
            // ckOnBoard
            // 
            this.ckOnBoard.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckOnBoard.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ckOnBoard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckOnBoard.Location = new System.Drawing.Point(207, 146);
            this.ckOnBoard.Name = "ckOnBoard";
            this.ckOnBoard.Size = new System.Drawing.Size(130, 69);
            this.ckOnBoard.TabIndex = 32;
            this.ckOnBoard.Text = "On-board motor driver";
            this.ckOnBoard.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckOnBoard.UseVisualStyleBackColor = true;
            this.ckOnBoard.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ckOnBoard_HelpRequested);
            // 
            // ckFlowOn
            // 
            this.ckFlowOn.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckFlowOn.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ckFlowOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckFlowOn.Location = new System.Drawing.Point(404, 232);
            this.ckFlowOn.Name = "ckFlowOn";
            this.ckFlowOn.Size = new System.Drawing.Size(130, 69);
            this.ckFlowOn.TabIndex = 31;
            this.ckFlowOn.Text = "Flow on High";
            this.ckFlowOn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckFlowOn.UseVisualStyleBackColor = true;
            this.ckFlowOn.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ckFlowOn_HelpRequested);
            // 
            // ckUseRate
            // 
            this.ckUseRate.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckUseRate.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ckUseRate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckUseRate.Location = new System.Drawing.Point(601, 146);
            this.ckUseRate.Name = "ckUseRate";
            this.ckUseRate.Size = new System.Drawing.Size(130, 69);
            this.ckUseRate.TabIndex = 29;
            this.ckUseRate.Text = "Use rate control";
            this.ckUseRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckUseRate.UseVisualStyleBackColor = true;
            // 
            // ckRelayOn
            // 
            this.ckRelayOn.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckRelayOn.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ckRelayOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckRelayOn.Location = new System.Drawing.Point(404, 146);
            this.ckRelayOn.Name = "ckRelayOn";
            this.ckRelayOn.Size = new System.Drawing.Size(130, 69);
            this.ckRelayOn.TabIndex = 28;
            this.ckRelayOn.Text = "Relay on High";
            this.ckRelayOn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckRelayOn.UseVisualStyleBackColor = true;
            this.ckRelayOn.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ckRelayOn_HelpRequested);
            // 
            // ckGyro
            // 
            this.ckGyro.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckGyro.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ckGyro.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckGyro.Location = new System.Drawing.Point(207, 232);
            this.ckGyro.Name = "ckGyro";
            this.ckGyro.Size = new System.Drawing.Size(130, 69);
            this.ckGyro.TabIndex = 27;
            this.ckGyro.Text = "Gyro On";
            this.ckGyro.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckGyro.UseVisualStyleBackColor = true;
            // 
            // tbModule
            // 
            this.tbModule.Location = new System.Drawing.Point(613, 98);
            this.tbModule.Name = "tbModule";
            this.tbModule.Size = new System.Drawing.Size(118, 29);
            this.tbModule.TabIndex = 26;
            this.tbModule.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbPulseCal
            // 
            this.tbPulseCal.Location = new System.Drawing.Point(211, 98);
            this.tbPulseCal.Name = "tbPulseCal";
            this.tbPulseCal.Size = new System.Drawing.Size(118, 29);
            this.tbPulseCal.TabIndex = 23;
            this.tbPulseCal.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbPulseCal.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.tbPulseCal_HelpRequested);
            // 
            // tbMaxSpeed
            // 
            this.tbMaxSpeed.Location = new System.Drawing.Point(211, 54);
            this.tbMaxSpeed.Name = "tbMaxSpeed";
            this.tbMaxSpeed.Size = new System.Drawing.Size(118, 29);
            this.tbMaxSpeed.TabIndex = 22;
            this.tbMaxSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbMinSpeed
            // 
            this.tbMinSpeed.Location = new System.Drawing.Point(211, 10);
            this.tbMinSpeed.Name = "tbMinSpeed";
            this.tbMinSpeed.Size = new System.Drawing.Size(118, 29);
            this.tbMinSpeed.TabIndex = 21;
            this.tbMinSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(412, 100);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(96, 24);
            this.label9.TabIndex = 20;
            this.label9.Text = "Module ID";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(412, 12);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(116, 24);
            this.label11.TabIndex = 18;
            this.label11.Text = "Analog Input";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(10, 100);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(146, 24);
            this.label12.TabIndex = 17;
            this.label12.Text = "Speed pulse cal";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(10, 56);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(151, 24);
            this.label13.TabIndex = 16;
            this.label13.Text = "Maximum speed";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(10, 12);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(146, 24);
            this.label14.TabIndex = 15;
            this.label14.Text = "Minimum speed";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.tbCurrentSensor);
            this.tabPage3.Controls.Add(this.label27);
            this.tabPage3.Controls.Add(this.tbSendEnable);
            this.tabPage3.Controls.Add(this.tbSpeedPulse);
            this.tabPage3.Controls.Add(this.tbPwm2);
            this.tabPage3.Controls.Add(this.label24);
            this.tabPage3.Controls.Add(this.label25);
            this.tabPage3.Controls.Add(this.label26);
            this.tabPage3.Controls.Add(this.tbDir2);
            this.tabPage3.Controls.Add(this.tbEncoder);
            this.tabPage3.Controls.Add(this.tbPressureSensor);
            this.tabPage3.Controls.Add(this.label21);
            this.tabPage3.Controls.Add(this.label22);
            this.tabPage3.Controls.Add(this.label23);
            this.tabPage3.Controls.Add(this.tbWorkSwitch);
            this.tabPage3.Controls.Add(this.tbSteerRelay);
            this.tabPage3.Controls.Add(this.tbWAS);
            this.tabPage3.Controls.Add(this.label18);
            this.tabPage3.Controls.Add(this.label19);
            this.tabPage3.Controls.Add(this.label20);
            this.tabPage3.Controls.Add(this.tbSteerSwitch);
            this.tabPage3.Controls.Add(this.tbPwm1);
            this.tabPage3.Controls.Add(this.tbDir1);
            this.tabPage3.Controls.Add(this.label15);
            this.tabPage3.Controls.Add(this.label16);
            this.tabPage3.Controls.Add(this.label17);
            this.tabPage3.Location = new System.Drawing.Point(4, 33);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(746, 307);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Pins";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tbCurrentSensor
            // 
            this.tbCurrentSensor.Location = new System.Drawing.Point(220, 261);
            this.tbCurrentSensor.Name = "tbCurrentSensor";
            this.tbCurrentSensor.Size = new System.Drawing.Size(118, 29);
            this.tbCurrentSensor.TabIndex = 49;
            this.tbCurrentSensor.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(16, 263);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(134, 24);
            this.label27.TabIndex = 48;
            this.label27.Text = "Current sensor";
            // 
            // tbSendEnable
            // 
            this.tbSendEnable.Location = new System.Drawing.Point(609, 220);
            this.tbSendEnable.Name = "tbSendEnable";
            this.tbSendEnable.Size = new System.Drawing.Size(118, 29);
            this.tbSendEnable.TabIndex = 47;
            this.tbSendEnable.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbSpeedPulse
            // 
            this.tbSpeedPulse.Location = new System.Drawing.Point(609, 179);
            this.tbSpeedPulse.Name = "tbSpeedPulse";
            this.tbSpeedPulse.Size = new System.Drawing.Size(118, 29);
            this.tbSpeedPulse.TabIndex = 46;
            this.tbSpeedPulse.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbPwm2
            // 
            this.tbPwm2.Location = new System.Drawing.Point(609, 138);
            this.tbPwm2.Name = "tbPwm2";
            this.tbPwm2.Size = new System.Drawing.Size(118, 29);
            this.tbPwm2.TabIndex = 45;
            this.tbPwm2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(405, 222);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(175, 24);
            this.label24.TabIndex = 44;
            this.label24.Text = "RS485 send enable";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(405, 181);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(117, 24);
            this.label25.TabIndex = 43;
            this.label25.Text = "Speed pulse";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(405, 140);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(119, 24);
            this.label26.TabIndex = 42;
            this.label26.Text = "Motor2 PWM";
            // 
            // tbDir2
            // 
            this.tbDir2.Location = new System.Drawing.Point(609, 97);
            this.tbDir2.Name = "tbDir2";
            this.tbDir2.Size = new System.Drawing.Size(118, 29);
            this.tbDir2.TabIndex = 41;
            this.tbDir2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbEncoder
            // 
            this.tbEncoder.Location = new System.Drawing.Point(609, 56);
            this.tbEncoder.Name = "tbEncoder";
            this.tbEncoder.Size = new System.Drawing.Size(118, 29);
            this.tbEncoder.TabIndex = 40;
            this.tbEncoder.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbPressureSensor
            // 
            this.tbPressureSensor.Location = new System.Drawing.Point(609, 15);
            this.tbPressureSensor.Name = "tbPressureSensor";
            this.tbPressureSensor.Size = new System.Drawing.Size(118, 29);
            this.tbPressureSensor.TabIndex = 39;
            this.tbPressureSensor.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(405, 99);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(103, 24);
            this.label21.TabIndex = 38;
            this.label21.Text = "Motor2 DIR";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(405, 58);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(194, 24);
            this.label22.TabIndex = 37;
            this.label22.Text = "Encoder/Flow Sensor";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(405, 17);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(147, 24);
            this.label23.TabIndex = 36;
            this.label23.Text = "Pressure sensor";
            // 
            // tbWorkSwitch
            // 
            this.tbWorkSwitch.Location = new System.Drawing.Point(220, 220);
            this.tbWorkSwitch.Name = "tbWorkSwitch";
            this.tbWorkSwitch.Size = new System.Drawing.Size(118, 29);
            this.tbWorkSwitch.TabIndex = 35;
            this.tbWorkSwitch.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbSteerRelay
            // 
            this.tbSteerRelay.Location = new System.Drawing.Point(220, 179);
            this.tbSteerRelay.Name = "tbSteerRelay";
            this.tbSteerRelay.Size = new System.Drawing.Size(118, 29);
            this.tbSteerRelay.TabIndex = 34;
            this.tbSteerRelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSteerRelay.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.tbSteerRelay_HelpRequested);
            // 
            // tbWAS
            // 
            this.tbWAS.Location = new System.Drawing.Point(220, 138);
            this.tbWAS.Name = "tbWAS";
            this.tbWAS.Size = new System.Drawing.Size(118, 29);
            this.tbWAS.TabIndex = 33;
            this.tbWAS.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(16, 222);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(111, 24);
            this.label18.TabIndex = 32;
            this.label18.Text = "Work switch";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(16, 181);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(99, 24);
            this.label19.TabIndex = 31;
            this.label19.Text = "Steer relay";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(16, 140);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(185, 24);
            this.label20.TabIndex = 30;
            this.label20.Text = "Wheel Angle Sensor";
            // 
            // tbSteerSwitch
            // 
            this.tbSteerSwitch.Location = new System.Drawing.Point(220, 97);
            this.tbSteerSwitch.Name = "tbSteerSwitch";
            this.tbSteerSwitch.Size = new System.Drawing.Size(118, 29);
            this.tbSteerSwitch.TabIndex = 29;
            this.tbSteerSwitch.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbPwm1
            // 
            this.tbPwm1.Location = new System.Drawing.Point(220, 56);
            this.tbPwm1.Name = "tbPwm1";
            this.tbPwm1.Size = new System.Drawing.Size(118, 29);
            this.tbPwm1.TabIndex = 28;
            this.tbPwm1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbDir1
            // 
            this.tbDir1.Location = new System.Drawing.Point(220, 15);
            this.tbDir1.Name = "tbDir1";
            this.tbDir1.Size = new System.Drawing.Size(118, 29);
            this.tbDir1.TabIndex = 27;
            this.tbDir1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(16, 99);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(111, 24);
            this.label15.TabIndex = 26;
            this.label15.Text = "Steer switch";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(16, 58);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(105, 24);
            this.label16.TabIndex = 25;
            this.label16.Text = "Steer PWM";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(16, 17);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(89, 24);
            this.label17.TabIndex = 24;
            this.label17.Text = "Steer DIR";
            // 
            // btnLoadDefaults
            // 
            this.btnLoadDefaults.BackColor = System.Drawing.Color.Transparent;
            this.btnLoadDefaults.FlatAppearance.BorderSize = 0;
            this.btnLoadDefaults.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadDefaults.Image = global::PCBsetup.Properties.Resources.VehFileLoad;
            this.btnLoadDefaults.Location = new System.Drawing.Point(242, 362);
            this.btnLoadDefaults.Name = "btnLoadDefaults";
            this.btnLoadDefaults.Size = new System.Drawing.Size(115, 72);
            this.btnLoadDefaults.TabIndex = 23;
            this.btnLoadDefaults.UseVisualStyleBackColor = false;
            this.btnLoadDefaults.Click += new System.EventHandler(this.btnLoadDefaults_Click);
            this.btnLoadDefaults.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.btnLoadDefaults_HelpRequested);
            // 
            // btnSendToModule
            // 
            this.btnSendToModule.BackColor = System.Drawing.Color.Transparent;
            this.btnSendToModule.FlatAppearance.BorderSize = 0;
            this.btnSendToModule.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSendToModule.Image = global::PCBsetup.Properties.Resources.UpArrow64;
            this.btnSendToModule.Location = new System.Drawing.Point(377, 362);
            this.btnSendToModule.Name = "btnSendToModule";
            this.btnSendToModule.Size = new System.Drawing.Size(115, 72);
            this.btnSendToModule.TabIndex = 22;
            this.btnSendToModule.UseVisualStyleBackColor = false;
            this.btnSendToModule.Click += new System.EventHandler(this.btnSendToModule_Click);
            this.btnSendToModule.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.btnSendToModule_HelpRequested);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.Enabled = false;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Image = global::PCBsetup.Properties.Resources.Cancel64;
            this.btnCancel.Location = new System.Drawing.Point(512, 362);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(115, 72);
            this.btnCancel.TabIndex = 21;
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // bntOK
            // 
            this.bntOK.BackColor = System.Drawing.Color.Transparent;
            this.bntOK.FlatAppearance.BorderSize = 0;
            this.bntOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bntOK.Image = global::PCBsetup.Properties.Resources.bntOK_Image;
            this.bntOK.Location = new System.Drawing.Point(647, 362);
            this.bntOK.Name = "bntOK";
            this.bntOK.Size = new System.Drawing.Size(115, 72);
            this.bntOK.TabIndex = 20;
            this.bntOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.bntOK.UseVisualStyleBackColor = false;
            this.bntOK.Click += new System.EventHandler(this.bntOK_Click);
            // 
            // frmPCBsettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(778, 445);
            this.Controls.Add(this.btnLoadDefaults);
            this.Controls.Add(this.btnSendToModule);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.bntOK);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPCBsettings";
            this.ShowInTaskbar = false;
            this.Text = "Teensy AutoSteer Settings";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmPCBsettings_FormClosed);
            this.Load += new System.EventHandler(this.frmPCBsettings_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbReceiver;
        private System.Windows.Forms.ComboBox cbIMU;
        private System.Windows.Forms.TextBox tbZeroOffset;
        private System.Windows.Forms.TextBox tbIMUinterval;
        private System.Windows.Forms.TextBox tbIMUdelay;
        private System.Windows.Forms.TextBox tbRTCM;
        private System.Windows.Forms.TextBox tbRTCMserialPort;
        private System.Windows.Forms.TextBox tbNMEAserialPort;
        private System.Windows.Forms.TextBox tbModule;
        private System.Windows.Forms.TextBox tbPulseCal;
        private System.Windows.Forms.TextBox tbMaxSpeed;
        private System.Windows.Forms.TextBox tbMinSpeed;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckBox ckInvertRoll;
        private System.Windows.Forms.CheckBox ckSwapPitchRoll;
        private System.Windows.Forms.CheckBox ckOnBoard;
        private System.Windows.Forms.CheckBox ckFlowOn;
        private System.Windows.Forms.CheckBox ckUseRate;
        private System.Windows.Forms.CheckBox ckRelayOn;
        private System.Windows.Forms.CheckBox ckGyro;
        private System.Windows.Forms.Button btnLoadDefaults;
        private System.Windows.Forms.Button btnSendToModule;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button bntOK;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TextBox tbCurrentSensor;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.TextBox tbSendEnable;
        private System.Windows.Forms.TextBox tbSpeedPulse;
        private System.Windows.Forms.TextBox tbPwm2;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.TextBox tbDir2;
        private System.Windows.Forms.TextBox tbEncoder;
        private System.Windows.Forms.TextBox tbPressureSensor;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox tbWorkSwitch;
        private System.Windows.Forms.TextBox tbSteerRelay;
        private System.Windows.Forms.TextBox tbWAS;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox tbSteerSwitch;
        private System.Windows.Forms.TextBox tbPwm1;
        private System.Windows.Forms.TextBox tbDir1;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label lbIPpart4;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.TextBox tbIPaddress;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.CheckBox ckActuator;
        private System.Windows.Forms.ComboBox cbAnalog;
        private System.Windows.Forms.TextBox tbWemosSerialPort;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.TextBox tbRS485port;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.ComboBox cbRelayControl;
    }
}