﻿namespace RateController
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.bntOK = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label13 = new System.Windows.Forms.Label();
            this.tbRTCMserialPort = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.tbNMEAserialPort = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbRTCM = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbIMUinterval = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbIMUdelay = new System.Windows.Forms.TextBox();
            this.cbIMU = new System.Windows.Forms.ComboBox();
            this.tbZeroOffset = new System.Windows.Forms.TextBox();
            this.lb4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbReceiver = new System.Windows.Forms.ComboBox();
            this.lb5 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.ckADS = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbPulseCal = new System.Windows.Forms.TextBox();
            this.tbRS485port = new System.Windows.Forms.TextBox();
            this.ckFlowOn = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbMaxSpeed = new System.Windows.Forms.TextBox();
            this.tbModule = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.ckRelayOn = new System.Windows.Forms.CheckBox();
            this.ckUseRate = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ckInvertRoll = new System.Windows.Forms.CheckBox();
            this.ckSwapPitchRoll = new System.Windows.Forms.CheckBox();
            this.tbMinSpeed = new System.Windows.Forms.TextBox();
            this.ckGyro = new System.Windows.Forms.CheckBox();
            this.ckGGA = new System.Windows.Forms.CheckBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.label22 = new System.Windows.Forms.Label();
            this.tbSendEnable = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.tbSpeedPulse = new System.Windows.Forms.TextBox();
            this.tbPressureSensor = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.tbDir2 = new System.Windows.Forms.TextBox();
            this.label34 = new System.Windows.Forms.Label();
            this.tbPwm2 = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.tbWAS = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.tbEncoder = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.tbSteerSwitch = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.tbCurrentSensor = new System.Windows.Forms.TextBox();
            this.label29 = new System.Windows.Forms.Label();
            this.tbWorkSwitch = new System.Windows.Forms.TextBox();
            this.label30 = new System.Windows.Forms.Label();
            this.tbSteerRelay = new System.Windows.Forms.TextBox();
            this.label32 = new System.Windows.Forms.Label();
            this.tbPwm1 = new System.Windows.Forms.TextBox();
            this.label33 = new System.Windows.Forms.Label();
            this.tbDir1 = new System.Windows.Forms.TextBox();
            this.btnLoadDefaults = new System.Windows.Forms.Button();
            this.btnSendToModule = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.tbAdsWasPin = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnCancel.Enabled = false;
            this.btnCancel.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCancel.Location = new System.Drawing.Point(419, 316);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(115, 72);
            this.btnCancel.TabIndex = 136;
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
            this.bntOK.Location = new System.Drawing.Point(543, 316);
            this.bntOK.Name = "bntOK";
            this.bntOK.Size = new System.Drawing.Size(115, 72);
            this.bntOK.TabIndex = 135;
            this.bntOK.Text = "Close";
            this.bntOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.bntOK.UseVisualStyleBackColor = true;
            this.bntOK.Click += new System.EventHandler(this.bntOK_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(646, 298);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label13);
            this.tabPage1.Controls.Add(this.tbRTCMserialPort);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.tbNMEAserialPort);
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.tbRTCM);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.tbIMUinterval);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.tbIMUdelay);
            this.tabPage1.Controls.Add(this.cbIMU);
            this.tabPage1.Controls.Add(this.tbZeroOffset);
            this.tabPage1.Controls.Add(this.lb4);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.cbReceiver);
            this.tabPage1.Controls.Add(this.lb5);
            this.tabPage1.Location = new System.Drawing.Point(4, 33);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPage1.Size = new System.Drawing.Size(638, 261);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Config 1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(6, 124);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(148, 23);
            this.label13.TabIndex = 143;
            this.label13.Text = "RTCM serial port";
            // 
            // tbRTCMserialPort
            // 
            this.tbRTCMserialPort.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRTCMserialPort.Location = new System.Drawing.Point(200, 122);
            this.tbRTCMserialPort.MaxLength = 8;
            this.tbRTCMserialPort.Name = "tbRTCMserialPort";
            this.tbRTCMserialPort.Size = new System.Drawing.Size(102, 30);
            this.tbRTCMserialPort.TabIndex = 142;
            this.tbRTCMserialPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(6, 86);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(149, 23);
            this.label11.TabIndex = 141;
            this.label11.Text = "NMEA serial port";
            // 
            // tbNMEAserialPort
            // 
            this.tbNMEAserialPort.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbNMEAserialPort.Location = new System.Drawing.Point(200, 84);
            this.tbNMEAserialPort.MaxLength = 8;
            this.tbNMEAserialPort.Name = "tbNMEAserialPort";
            this.tbNMEAserialPort.Size = new System.Drawing.Size(102, 30);
            this.tbNMEAserialPort.TabIndex = 140;
            this.tbNMEAserialPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(6, 162);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(160, 23);
            this.label9.TabIndex = 139;
            this.label9.Text = "RTCM UDP port #";
            // 
            // tbRTCM
            // 
            this.tbRTCM.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRTCM.Location = new System.Drawing.Point(200, 160);
            this.tbRTCM.MaxLength = 8;
            this.tbRTCM.Name = "tbRTCM";
            this.tbRTCM.Size = new System.Drawing.Size(102, 30);
            this.tbRTCM.TabIndex = 138;
            this.tbRTCM.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(332, 124);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(168, 23);
            this.label3.TabIndex = 123;
            this.label3.Text = "IMU report interval";
            // 
            // tbIMUinterval
            // 
            this.tbIMUinterval.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbIMUinterval.Location = new System.Drawing.Point(526, 122);
            this.tbIMUinterval.MaxLength = 8;
            this.tbIMUinterval.Name = "tbIMUinterval";
            this.tbIMUinterval.Size = new System.Drawing.Size(102, 30);
            this.tbIMUinterval.TabIndex = 122;
            this.tbIMUinterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbIMUinterval.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.tbIMUinterval_HelpRequested);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(332, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(138, 23);
            this.label2.TabIndex = 121;
            this.label2.Text = "IMU read delay";
            // 
            // tbIMUdelay
            // 
            this.tbIMUdelay.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbIMUdelay.Location = new System.Drawing.Point(526, 84);
            this.tbIMUdelay.MaxLength = 8;
            this.tbIMUdelay.Name = "tbIMUdelay";
            this.tbIMUdelay.Size = new System.Drawing.Size(102, 30);
            this.tbIMUdelay.TabIndex = 120;
            this.tbIMUdelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbIMUdelay.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.tbIMUdelay_HelpRequested);
            // 
            // cbIMU
            // 
            this.cbIMU.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbIMU.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbIMU.FormattingEnabled = true;
            this.cbIMU.Items.AddRange(new object[] {
            "None",
            "Sparkfun BNO",
            "CMPS14",
            "Adafruit BNO"});
            this.cbIMU.Location = new System.Drawing.Point(466, 47);
            this.cbIMU.Name = "cbIMU";
            this.cbIMU.Size = new System.Drawing.Size(161, 31);
            this.cbIMU.TabIndex = 118;
            this.cbIMU.SelectedIndexChanged += new System.EventHandler(this.tb_TextChanged);
            // 
            // tbZeroOffset
            // 
            this.tbZeroOffset.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbZeroOffset.Location = new System.Drawing.Point(526, 160);
            this.tbZeroOffset.MaxLength = 8;
            this.tbZeroOffset.Name = "tbZeroOffset";
            this.tbZeroOffset.Size = new System.Drawing.Size(102, 30);
            this.tbZeroOffset.TabIndex = 116;
            this.tbZeroOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lb4
            // 
            this.lb4.AutoSize = true;
            this.lb4.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb4.Location = new System.Drawing.Point(332, 162);
            this.lb4.Name = "lb4";
            this.lb4.Size = new System.Drawing.Size(142, 23);
            this.lb4.TabIndex = 117;
            this.lb4.Text = "WAS zero offset";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(332, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 23);
            this.label1.TabIndex = 119;
            this.label1.Text = "IMU";
            // 
            // cbReceiver
            // 
            this.cbReceiver.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbReceiver.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbReceiver.FormattingEnabled = true;
            this.cbReceiver.Items.AddRange(new object[] {
            "None",
            "SimpleRTK2B",
            "Sparkfun F9P"});
            this.cbReceiver.Location = new System.Drawing.Point(141, 46);
            this.cbReceiver.Name = "cbReceiver";
            this.cbReceiver.Size = new System.Drawing.Size(161, 31);
            this.cbReceiver.TabIndex = 112;
            this.cbReceiver.SelectedIndexChanged += new System.EventHandler(this.tb_TextChanged);
            // 
            // lb5
            // 
            this.lb5.AutoSize = true;
            this.lb5.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb5.Location = new System.Drawing.Point(6, 49);
            this.lb5.Name = "lb5";
            this.lb5.Size = new System.Drawing.Size(116, 23);
            this.lb5.TabIndex = 113;
            this.lb5.Text = "GPS receiver";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.tbAdsWasPin);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.ckADS);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.tbPulseCal);
            this.tabPage2.Controls.Add(this.tbRS485port);
            this.tabPage2.Controls.Add(this.ckFlowOn);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.tbMaxSpeed);
            this.tabPage2.Controls.Add(this.tbModule);
            this.tabPage2.Controls.Add(this.label10);
            this.tabPage2.Controls.Add(this.ckRelayOn);
            this.tabPage2.Controls.Add(this.ckUseRate);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.ckInvertRoll);
            this.tabPage2.Controls.Add(this.ckSwapPitchRoll);
            this.tabPage2.Controls.Add(this.tbMinSpeed);
            this.tabPage2.Controls.Add(this.ckGyro);
            this.tabPage2.Controls.Add(this.ckGGA);
            this.tabPage2.Location = new System.Drawing.Point(4, 33);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPage2.Size = new System.Drawing.Size(638, 261);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Config 2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(11, 82);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(142, 23);
            this.label6.TabIndex = 129;
            this.label6.Text = "Speed pulse cal";
            // 
            // ckADS
            // 
            this.ckADS.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckADS.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckADS.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ckADS.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckADS.Location = new System.Drawing.Point(180, 187);
            this.ckADS.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ckADS.Name = "ckADS";
            this.ckADS.Size = new System.Drawing.Size(100, 60);
            this.ckADS.TabIndex = 149;
            this.ckADS.Text = "Use ADS1115";
            this.ckADS.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckADS.UseVisualStyleBackColor = true;
            this.ckADS.CheckedChanged += new System.EventHandler(this.tb_TextChanged);
            this.ckADS.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ckADS_HelpRequested);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(353, 47);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(152, 23);
            this.label7.TabIndex = 141;
            this.label7.Text = "RS485 serial port";
            // 
            // tbPulseCal
            // 
            this.tbPulseCal.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPulseCal.Location = new System.Drawing.Point(180, 79);
            this.tbPulseCal.MaxLength = 8;
            this.tbPulseCal.Name = "tbPulseCal";
            this.tbPulseCal.Size = new System.Drawing.Size(102, 30);
            this.tbPulseCal.TabIndex = 128;
            this.tbPulseCal.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbPulseCal.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.tbPulseCal_HelpRequested);
            // 
            // tbRS485port
            // 
            this.tbRS485port.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRS485port.Location = new System.Drawing.Point(527, 43);
            this.tbRS485port.MaxLength = 8;
            this.tbRS485port.Name = "tbRS485port";
            this.tbRS485port.Size = new System.Drawing.Size(102, 30);
            this.tbRS485port.TabIndex = 140;
            this.tbRS485port.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbRS485port.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.tbRS485port_HelpRequested);
            // 
            // ckFlowOn
            // 
            this.ckFlowOn.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckFlowOn.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckFlowOn.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ckFlowOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckFlowOn.Location = new System.Drawing.Point(353, 187);
            this.ckFlowOn.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ckFlowOn.Name = "ckFlowOn";
            this.ckFlowOn.Size = new System.Drawing.Size(100, 60);
            this.ckFlowOn.TabIndex = 148;
            this.ckFlowOn.Text = "Flow on high";
            this.ckFlowOn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckFlowOn.UseVisualStyleBackColor = true;
            this.ckFlowOn.CheckedChanged += new System.EventHandler(this.tb_TextChanged);
            this.ckFlowOn.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ckFlowOn_HelpRequested);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(11, 45);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(147, 23);
            this.label5.TabIndex = 127;
            this.label5.Text = "Maximum speed";
            // 
            // tbMaxSpeed
            // 
            this.tbMaxSpeed.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbMaxSpeed.Location = new System.Drawing.Point(180, 43);
            this.tbMaxSpeed.MaxLength = 8;
            this.tbMaxSpeed.Name = "tbMaxSpeed";
            this.tbMaxSpeed.Size = new System.Drawing.Size(102, 30);
            this.tbMaxSpeed.TabIndex = 126;
            this.tbMaxSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbModule
            // 
            this.tbModule.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbModule.Location = new System.Drawing.Point(527, 77);
            this.tbModule.MaxLength = 8;
            this.tbModule.Name = "tbModule";
            this.tbModule.Size = new System.Drawing.Size(102, 30);
            this.tbModule.TabIndex = 134;
            this.tbModule.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(354, 81);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(97, 23);
            this.label10.TabIndex = 135;
            this.label10.Text = "Module ID";
            // 
            // ckRelayOn
            // 
            this.ckRelayOn.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckRelayOn.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckRelayOn.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ckRelayOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckRelayOn.Location = new System.Drawing.Point(353, 117);
            this.ckRelayOn.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ckRelayOn.Name = "ckRelayOn";
            this.ckRelayOn.Size = new System.Drawing.Size(100, 60);
            this.ckRelayOn.TabIndex = 147;
            this.ckRelayOn.Text = "Relay on high";
            this.ckRelayOn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckRelayOn.UseVisualStyleBackColor = true;
            this.ckRelayOn.CheckedChanged += new System.EventHandler(this.tb_TextChanged);
            this.ckRelayOn.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ckRelayOn_HelpRequested);
            // 
            // ckUseRate
            // 
            this.ckUseRate.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckUseRate.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckUseRate.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ckUseRate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckUseRate.Location = new System.Drawing.Point(180, 117);
            this.ckUseRate.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ckUseRate.Name = "ckUseRate";
            this.ckUseRate.Size = new System.Drawing.Size(100, 60);
            this.ckUseRate.TabIndex = 146;
            this.ckUseRate.Text = "Use rate control";
            this.ckUseRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckUseRate.UseVisualStyleBackColor = true;
            this.ckUseRate.CheckedChanged += new System.EventHandler(this.tb_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(11, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(143, 23);
            this.label4.TabIndex = 125;
            this.label4.Text = "Minimum speed";
            // 
            // ckInvertRoll
            // 
            this.ckInvertRoll.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckInvertRoll.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckInvertRoll.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ckInvertRoll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckInvertRoll.Location = new System.Drawing.Point(526, 187);
            this.ckInvertRoll.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ckInvertRoll.Name = "ckInvertRoll";
            this.ckInvertRoll.Size = new System.Drawing.Size(100, 60);
            this.ckInvertRoll.TabIndex = 145;
            this.ckInvertRoll.Text = "Invert roll";
            this.ckInvertRoll.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckInvertRoll.UseVisualStyleBackColor = true;
            this.ckInvertRoll.CheckedChanged += new System.EventHandler(this.tb_TextChanged);
            // 
            // ckSwapPitchRoll
            // 
            this.ckSwapPitchRoll.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckSwapPitchRoll.BackColor = System.Drawing.Color.Transparent;
            this.ckSwapPitchRoll.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckSwapPitchRoll.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ckSwapPitchRoll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckSwapPitchRoll.Location = new System.Drawing.Point(526, 117);
            this.ckSwapPitchRoll.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ckSwapPitchRoll.Name = "ckSwapPitchRoll";
            this.ckSwapPitchRoll.Size = new System.Drawing.Size(100, 60);
            this.ckSwapPitchRoll.TabIndex = 144;
            this.ckSwapPitchRoll.Text = "Swap pitch roll";
            this.ckSwapPitchRoll.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.ckSwapPitchRoll.UseVisualStyleBackColor = false;
            // 
            // tbMinSpeed
            // 
            this.tbMinSpeed.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbMinSpeed.Location = new System.Drawing.Point(180, 6);
            this.tbMinSpeed.MaxLength = 8;
            this.tbMinSpeed.Name = "tbMinSpeed";
            this.tbMinSpeed.Size = new System.Drawing.Size(102, 30);
            this.tbMinSpeed.TabIndex = 124;
            this.tbMinSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ckGyro
            // 
            this.ckGyro.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckGyro.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckGyro.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ckGyro.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckGyro.Location = new System.Drawing.Point(7, 117);
            this.ckGyro.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ckGyro.Name = "ckGyro";
            this.ckGyro.Size = new System.Drawing.Size(100, 60);
            this.ckGyro.TabIndex = 142;
            this.ckGyro.Text = "Gyro On";
            this.ckGyro.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckGyro.UseVisualStyleBackColor = true;
            this.ckGyro.CheckedChanged += new System.EventHandler(this.tb_TextChanged);
            // 
            // ckGGA
            // 
            this.ckGGA.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckGGA.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckGGA.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ckGGA.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckGGA.Location = new System.Drawing.Point(7, 187);
            this.ckGGA.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ckGGA.Name = "ckGGA";
            this.ckGGA.Size = new System.Drawing.Size(100, 60);
            this.ckGGA.TabIndex = 143;
            this.ckGGA.Text = "GGA last";
            this.ckGGA.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckGGA.UseVisualStyleBackColor = true;
            this.ckGGA.CheckedChanged += new System.EventHandler(this.tb_TextChanged);
            this.ckGGA.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ckGGA_HelpRequested);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.label22);
            this.tabPage3.Controls.Add(this.tbSendEnable);
            this.tabPage3.Controls.Add(this.label23);
            this.tabPage3.Controls.Add(this.tbSpeedPulse);
            this.tabPage3.Controls.Add(this.tbPressureSensor);
            this.tabPage3.Controls.Add(this.label24);
            this.tabPage3.Controls.Add(this.label31);
            this.tabPage3.Controls.Add(this.tbDir2);
            this.tabPage3.Controls.Add(this.label34);
            this.tabPage3.Controls.Add(this.tbPwm2);
            this.tabPage3.Controls.Add(this.label25);
            this.tabPage3.Controls.Add(this.tbWAS);
            this.tabPage3.Controls.Add(this.label26);
            this.tabPage3.Controls.Add(this.tbEncoder);
            this.tabPage3.Controls.Add(this.label27);
            this.tabPage3.Controls.Add(this.tbSteerSwitch);
            this.tabPage3.Controls.Add(this.label28);
            this.tabPage3.Controls.Add(this.tbCurrentSensor);
            this.tabPage3.Controls.Add(this.label29);
            this.tabPage3.Controls.Add(this.tbWorkSwitch);
            this.tabPage3.Controls.Add(this.label30);
            this.tabPage3.Controls.Add(this.tbSteerRelay);
            this.tabPage3.Controls.Add(this.label32);
            this.tabPage3.Controls.Add(this.tbPwm1);
            this.tabPage3.Controls.Add(this.label33);
            this.tabPage3.Controls.Add(this.tbDir1);
            this.tabPage3.Location = new System.Drawing.Point(4, 33);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(638, 261);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Pins";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.Location = new System.Drawing.Point(353, 190);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(171, 23);
            this.label22.TabIndex = 183;
            this.label22.Text = "RS485 send enable";
            // 
            // tbSendEnable
            // 
            this.tbSendEnable.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSendEnable.Location = new System.Drawing.Point(531, 186);
            this.tbSendEnable.MaxLength = 8;
            this.tbSendEnable.Name = "tbSendEnable";
            this.tbSendEnable.Size = new System.Drawing.Size(102, 30);
            this.tbSendEnable.TabIndex = 182;
            this.tbSendEnable.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(353, 154);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(113, 23);
            this.label23.TabIndex = 181;
            this.label23.Text = "Speed pulse";
            // 
            // tbSpeedPulse
            // 
            this.tbSpeedPulse.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSpeedPulse.Location = new System.Drawing.Point(531, 150);
            this.tbSpeedPulse.MaxLength = 8;
            this.tbSpeedPulse.Name = "tbSpeedPulse";
            this.tbSpeedPulse.Size = new System.Drawing.Size(102, 30);
            this.tbSpeedPulse.TabIndex = 180;
            this.tbSpeedPulse.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbPressureSensor
            // 
            this.tbPressureSensor.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPressureSensor.Location = new System.Drawing.Point(531, 6);
            this.tbPressureSensor.MaxLength = 8;
            this.tbPressureSensor.Name = "tbPressureSensor";
            this.tbPressureSensor.Size = new System.Drawing.Size(102, 30);
            this.tbPressureSensor.TabIndex = 178;
            this.tbPressureSensor.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.Location = new System.Drawing.Point(353, 10);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(141, 23);
            this.label24.TabIndex = 179;
            this.label24.Text = "Pressure sensor";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label31.Location = new System.Drawing.Point(353, 118);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(96, 23);
            this.label31.TabIndex = 165;
            this.label31.Text = "Rate PWM";
            // 
            // tbDir2
            // 
            this.tbDir2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDir2.Location = new System.Drawing.Point(531, 78);
            this.tbDir2.MaxLength = 8;
            this.tbDir2.Name = "tbDir2";
            this.tbDir2.Size = new System.Drawing.Size(102, 30);
            this.tbDir2.TabIndex = 158;
            this.tbDir2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label34.Location = new System.Drawing.Point(353, 82);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(86, 23);
            this.label34.TabIndex = 159;
            this.label34.Text = "Rate DIR";
            // 
            // tbPwm2
            // 
            this.tbPwm2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPwm2.Location = new System.Drawing.Point(531, 114);
            this.tbPwm2.MaxLength = 8;
            this.tbPwm2.Name = "tbPwm2";
            this.tbPwm2.Size = new System.Drawing.Size(102, 30);
            this.tbPwm2.TabIndex = 164;
            this.tbPwm2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(7, 118);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(174, 23);
            this.label25.TabIndex = 177;
            this.label25.Text = "Wheel angle sensor";
            // 
            // tbWAS
            // 
            this.tbWAS.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbWAS.Location = new System.Drawing.Point(202, 114);
            this.tbWAS.MaxLength = 8;
            this.tbWAS.Name = "tbWAS";
            this.tbWAS.Size = new System.Drawing.Size(102, 30);
            this.tbWAS.TabIndex = 176;
            this.tbWAS.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.Location = new System.Drawing.Point(353, 46);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(79, 23);
            this.label26.TabIndex = 175;
            this.label26.Text = "Encoder";
            // 
            // tbEncoder
            // 
            this.tbEncoder.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbEncoder.Location = new System.Drawing.Point(531, 42);
            this.tbEncoder.MaxLength = 8;
            this.tbEncoder.Name = "tbEncoder";
            this.tbEncoder.Size = new System.Drawing.Size(102, 30);
            this.tbEncoder.TabIndex = 174;
            this.tbEncoder.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label27.Location = new System.Drawing.Point(7, 82);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(112, 23);
            this.label27.TabIndex = 173;
            this.label27.Text = "Steer switch";
            // 
            // tbSteerSwitch
            // 
            this.tbSteerSwitch.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSteerSwitch.Location = new System.Drawing.Point(202, 78);
            this.tbSteerSwitch.MaxLength = 8;
            this.tbSteerSwitch.Name = "tbSteerSwitch";
            this.tbSteerSwitch.Size = new System.Drawing.Size(102, 30);
            this.tbSteerSwitch.TabIndex = 172;
            this.tbSteerSwitch.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.Location = new System.Drawing.Point(7, 226);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(133, 23);
            this.label28.TabIndex = 171;
            this.label28.Text = "Current sensor";
            // 
            // tbCurrentSensor
            // 
            this.tbCurrentSensor.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbCurrentSensor.Location = new System.Drawing.Point(202, 222);
            this.tbCurrentSensor.MaxLength = 8;
            this.tbCurrentSensor.Name = "tbCurrentSensor";
            this.tbCurrentSensor.Size = new System.Drawing.Size(102, 30);
            this.tbCurrentSensor.TabIndex = 170;
            this.tbCurrentSensor.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label29.Location = new System.Drawing.Point(7, 190);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(111, 23);
            this.label29.TabIndex = 169;
            this.label29.Text = "Work switch";
            // 
            // tbWorkSwitch
            // 
            this.tbWorkSwitch.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbWorkSwitch.Location = new System.Drawing.Point(202, 186);
            this.tbWorkSwitch.MaxLength = 8;
            this.tbWorkSwitch.Name = "tbWorkSwitch";
            this.tbWorkSwitch.Size = new System.Drawing.Size(102, 30);
            this.tbWorkSwitch.TabIndex = 168;
            this.tbWorkSwitch.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label30.Location = new System.Drawing.Point(7, 154);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(100, 23);
            this.label30.TabIndex = 167;
            this.label30.Text = "Steer relay";
            // 
            // tbSteerRelay
            // 
            this.tbSteerRelay.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSteerRelay.Location = new System.Drawing.Point(202, 150);
            this.tbSteerRelay.MaxLength = 8;
            this.tbSteerRelay.Name = "tbSteerRelay";
            this.tbSteerRelay.Size = new System.Drawing.Size(102, 30);
            this.tbSteerRelay.TabIndex = 166;
            this.tbSteerRelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSteerRelay.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.tbSteerRelay_HelpRequested);
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.Location = new System.Drawing.Point(7, 46);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(102, 23);
            this.label32.TabIndex = 163;
            this.label32.Text = "Steer PWM";
            // 
            // tbPwm1
            // 
            this.tbPwm1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPwm1.Location = new System.Drawing.Point(202, 42);
            this.tbPwm1.MaxLength = 8;
            this.tbPwm1.Name = "tbPwm1";
            this.tbPwm1.Size = new System.Drawing.Size(102, 30);
            this.tbPwm1.TabIndex = 162;
            this.tbPwm1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label33.Location = new System.Drawing.Point(7, 10);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(92, 23);
            this.label33.TabIndex = 161;
            this.label33.Text = "Steer DIR";
            // 
            // tbDir1
            // 
            this.tbDir1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDir1.Location = new System.Drawing.Point(202, 6);
            this.tbDir1.MaxLength = 8;
            this.tbDir1.Name = "tbDir1";
            this.tbDir1.Size = new System.Drawing.Size(102, 30);
            this.tbDir1.TabIndex = 160;
            this.tbDir1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnLoadDefaults
            // 
            this.btnLoadDefaults.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoadDefaults.Location = new System.Drawing.Point(207, 316);
            this.btnLoadDefaults.Name = "btnLoadDefaults";
            this.btnLoadDefaults.Size = new System.Drawing.Size(100, 72);
            this.btnLoadDefaults.TabIndex = 138;
            this.btnLoadDefaults.Text = "Load Defaults";
            this.btnLoadDefaults.UseVisualStyleBackColor = true;
            this.btnLoadDefaults.Click += new System.EventHandler(this.btnLoadDefaults_Click);
            // 
            // btnSendToModule
            // 
            this.btnSendToModule.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSendToModule.Location = new System.Drawing.Point(313, 316);
            this.btnSendToModule.Name = "btnSendToModule";
            this.btnSendToModule.Size = new System.Drawing.Size(100, 72);
            this.btnSendToModule.TabIndex = 139;
            this.btnSendToModule.Text = "Send to Module";
            this.btnSendToModule.UseVisualStyleBackColor = true;
            this.btnSendToModule.Click += new System.EventHandler(this.btnSendToModule_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(353, 10);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(162, 23);
            this.label8.TabIndex = 151;
            this.label8.Text = "ADS1115 WAS pin";
            // 
            // tbAdsWasPin
            // 
            this.tbAdsWasPin.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbAdsWasPin.Location = new System.Drawing.Point(527, 6);
            this.tbAdsWasPin.MaxLength = 8;
            this.tbAdsWasPin.Name = "tbAdsWasPin";
            this.tbAdsWasPin.Size = new System.Drawing.Size(102, 30);
            this.tbAdsWasPin.TabIndex = 150;
            this.tbAdsWasPin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbAdsWasPin.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.tbAdsWasPin_HelpRequested);
            // 
            // frmPCBsettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(667, 400);
            this.Controls.Add(this.btnSendToModule);
            this.Controls.Add(this.btnLoadDefaults);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.bntOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPCBsettings";
            this.ShowInTaskbar = false;
            this.Text = "PCB Settings";
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

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button bntOK;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ComboBox cbReceiver;
        private System.Windows.Forms.Label lb5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbMinSpeed;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbIMUinterval;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbIMUdelay;
        private System.Windows.Forms.ComboBox cbIMU;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lb4;
        private System.Windows.Forms.TextBox tbZeroOffset;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbPulseCal;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbMaxSpeed;
        private System.Windows.Forms.Button btnLoadDefaults;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbRS485port;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbModule;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox tbSendEnable;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox tbSpeedPulse;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox tbPressureSensor;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.TextBox tbWAS;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.TextBox tbEncoder;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.TextBox tbSteerSwitch;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.TextBox tbCurrentSensor;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.TextBox tbWorkSwitch;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.TextBox tbSteerRelay;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.TextBox tbPwm2;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.TextBox tbPwm1;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.TextBox tbDir1;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.TextBox tbDir2;
        private System.Windows.Forms.CheckBox ckADS;
        private System.Windows.Forms.CheckBox ckFlowOn;
        private System.Windows.Forms.CheckBox ckRelayOn;
        private System.Windows.Forms.CheckBox ckUseRate;
        private System.Windows.Forms.CheckBox ckInvertRoll;
        private System.Windows.Forms.CheckBox ckSwapPitchRoll;
        private System.Windows.Forms.CheckBox ckGyro;
        private System.Windows.Forms.CheckBox ckGGA;
        private System.Windows.Forms.Button btnSendToModule;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox tbRTCMserialPort;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tbNMEAserialPort;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbRTCM;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbAdsWasPin;
    }
}