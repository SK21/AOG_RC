namespace IMUapp
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.lblCurrentBaud = new System.Windows.Forms.Label();
            this.cboxBaud = new System.Windows.Forms.ComboBox();
            this.lbArduinoConnected = new System.Windows.Forms.Label();
            this.btnRescan = new System.Windows.Forms.Button();
            this.btnOpenSerialArduino = new System.Windows.Forms.Button();
            this.btnCloseSerialArduino = new System.Windows.Forms.Button();
            this.cboxArdPort = new System.Windows.Forms.ComboBox();
            this.lblCurrentArduinoPort = new System.Windows.Forms.Label();
            this.tbDisplay = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.bntOK = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.rbPitch = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.ckInvert = new System.Windows.Forms.CheckBox();
            this.ckBoxViewData = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lbLocalIP = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.label43 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbDestinationIP = new System.Windows.Forms.Label();
            this.lbNetworkIP = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lbNetReceive = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.label45 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.btnDay = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lbVersion = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblCurrentBaud
            // 
            this.lblCurrentBaud.AutoSize = true;
            this.lblCurrentBaud.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentBaud.Location = new System.Drawing.Point(235, 91);
            this.lblCurrentBaud.Name = "lblCurrentBaud";
            this.lblCurrentBaud.Size = new System.Drawing.Size(53, 23);
            this.lblCurrentBaud.TabIndex = 82;
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
            this.cboxBaud.Location = new System.Drawing.Point(294, 83);
            this.cboxBaud.Name = "cboxBaud";
            this.cboxBaud.Size = new System.Drawing.Size(127, 37);
            this.cboxBaud.TabIndex = 81;
            this.cboxBaud.SelectedIndexChanged += new System.EventHandler(this.cboxBaud_SelectedIndexChanged);
            // 
            // lbArduinoConnected
            // 
            this.lbArduinoConnected.BackColor = System.Drawing.Color.Red;
            this.lbArduinoConnected.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbArduinoConnected.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbArduinoConnected.Location = new System.Drawing.Point(6, 289);
            this.lbArduinoConnected.Name = "lbArduinoConnected";
            this.lbArduinoConnected.Size = new System.Drawing.Size(417, 23);
            this.lbArduinoConnected.TabIndex = 80;
            this.lbArduinoConnected.Text = "Port Disconnected";
            this.lbArduinoConnected.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnRescan
            // 
            this.btnRescan.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnRescan.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnRescan.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRescan.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnRescan.Location = new System.Drawing.Point(443, 162);
            this.btnRescan.Name = "btnRescan";
            this.btnRescan.Size = new System.Drawing.Size(120, 72);
            this.btnRescan.TabIndex = 79;
            this.btnRescan.Text = "Rescan Ports";
            this.btnRescan.UseVisualStyleBackColor = false;
            this.btnRescan.Click += new System.EventHandler(this.btnRescan_Click);
            // 
            // btnOpenSerialArduino
            // 
            this.btnOpenSerialArduino.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnOpenSerialArduino.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnOpenSerialArduino.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenSerialArduino.Location = new System.Drawing.Point(443, 6);
            this.btnOpenSerialArduino.Name = "btnOpenSerialArduino";
            this.btnOpenSerialArduino.Size = new System.Drawing.Size(120, 72);
            this.btnOpenSerialArduino.TabIndex = 78;
            this.btnOpenSerialArduino.Text = "Connect";
            this.btnOpenSerialArduino.UseVisualStyleBackColor = false;
            this.btnOpenSerialArduino.Click += new System.EventHandler(this.btnOpenSerialArduino_Click);
            // 
            // btnCloseSerialArduino
            // 
            this.btnCloseSerialArduino.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnCloseSerialArduino.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCloseSerialArduino.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCloseSerialArduino.Location = new System.Drawing.Point(443, 84);
            this.btnCloseSerialArduino.Name = "btnCloseSerialArduino";
            this.btnCloseSerialArduino.Size = new System.Drawing.Size(120, 72);
            this.btnCloseSerialArduino.TabIndex = 77;
            this.btnCloseSerialArduino.Text = "Disconnect";
            this.btnCloseSerialArduino.UseVisualStyleBackColor = false;
            this.btnCloseSerialArduino.Click += new System.EventHandler(this.btnCloseSerialArduino_Click);
            // 
            // cboxArdPort
            // 
            this.cboxArdPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxArdPort.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold);
            this.cboxArdPort.FormattingEnabled = true;
            this.cboxArdPort.Location = new System.Drawing.Point(56, 82);
            this.cboxArdPort.Name = "cboxArdPort";
            this.cboxArdPort.Size = new System.Drawing.Size(127, 37);
            this.cboxArdPort.TabIndex = 76;
            this.cboxArdPort.SelectedIndexChanged += new System.EventHandler(this.cboxArdPort_SelectedIndexChanged);
            // 
            // lblCurrentArduinoPort
            // 
            this.lblCurrentArduinoPort.AutoSize = true;
            this.lblCurrentArduinoPort.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentArduinoPort.Location = new System.Drawing.Point(7, 91);
            this.lblCurrentArduinoPort.Name = "lblCurrentArduinoPort";
            this.lblCurrentArduinoPort.Size = new System.Drawing.Size(43, 23);
            this.lblCurrentArduinoPort.TabIndex = 75;
            this.lblCurrentArduinoPort.Text = "Port";
            // 
            // tbDisplay
            // 
            this.tbDisplay.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDisplay.Location = new System.Drawing.Point(6, 126);
            this.tbDisplay.Multiline = true;
            this.tbDisplay.Name = "tbDisplay";
            this.tbDisplay.ReadOnly = true;
            this.tbDisplay.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbDisplay.Size = new System.Drawing.Size(417, 151);
            this.tbDisplay.TabIndex = 83;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // bntOK
            // 
            this.bntOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bntOK.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.bntOK.Image = ((System.Drawing.Image)(resources.GetObject("bntOK.Image")));
            this.bntOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.bntOK.Location = new System.Drawing.Point(443, 240);
            this.bntOK.Name = "bntOK";
            this.bntOK.Size = new System.Drawing.Size(120, 72);
            this.bntOK.TabIndex = 94;
            this.bntOK.Text = "Close";
            this.bntOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.bntOK.UseVisualStyleBackColor = true;
            this.bntOK.Click += new System.EventHandler(this.bntOK_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(592, 360);
            this.tabControl1.TabIndex = 95;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.rbPitch);
            this.tabPage1.Controls.Add(this.radioButton1);
            this.tabPage1.Controls.Add(this.ckInvert);
            this.tabPage1.Controls.Add(this.tbDisplay);
            this.tabPage1.Controls.Add(this.lbArduinoConnected);
            this.tabPage1.Controls.Add(this.ckBoxViewData);
            this.tabPage1.Controls.Add(this.cboxBaud);
            this.tabPage1.Controls.Add(this.lblCurrentArduinoPort);
            this.tabPage1.Controls.Add(this.lblCurrentBaud);
            this.tabPage1.Controls.Add(this.cboxArdPort);
            this.tabPage1.Controls.Add(this.btnOpenSerialArduino);
            this.tabPage1.Controls.Add(this.btnCloseSerialArduino);
            this.tabPage1.Controls.Add(this.btnRescan);
            this.tabPage1.Controls.Add(this.bntOK);
            this.tabPage1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPage1.Location = new System.Drawing.Point(4, 32);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(584, 324);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Main";
            this.tabPage1.Click += new System.EventHandler(this.tabPage1_Click);
            // 
            // rbPitch
            // 
            this.rbPitch.AutoSize = true;
            this.rbPitch.Location = new System.Drawing.Point(71, 45);
            this.rbPitch.Name = "rbPitch";
            this.rbPitch.Size = new System.Drawing.Size(104, 27);
            this.rbPitch.TabIndex = 99;
            this.rbPitch.Text = "Use Pitch";
            this.rbPitch.UseVisualStyleBackColor = true;
            this.rbPitch.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(71, 8);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(94, 27);
            this.radioButton1.TabIndex = 98;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Use Roll";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // ckInvert
            // 
            this.ckInvert.AutoSize = true;
            this.ckInvert.Location = new System.Drawing.Point(239, 12);
            this.ckInvert.Name = "ckInvert";
            this.ckInvert.Size = new System.Drawing.Size(79, 27);
            this.ckInvert.TabIndex = 97;
            this.ckInvert.Text = "Invert";
            this.ckInvert.UseVisualStyleBackColor = true;
            this.ckInvert.CheckedChanged += new System.EventHandler(this.ckInvert_CheckedChanged);
            // 
            // ckBoxViewData
            // 
            this.ckBoxViewData.AutoSize = true;
            this.ckBoxViewData.Location = new System.Drawing.Point(239, 45);
            this.ckBoxViewData.Name = "ckBoxViewData";
            this.ckBoxViewData.Size = new System.Drawing.Size(151, 27);
            this.ckBoxViewData.TabIndex = 96;
            this.ckBoxViewData.Text = "View IMU data";
            this.ckBoxViewData.UseVisualStyleBackColor = true;
            this.ckBoxViewData.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.label25);
            this.tabPage2.Controls.Add(this.label26);
            this.tabPage2.Controls.Add(this.label29);
            this.tabPage2.Controls.Add(this.label42);
            this.tabPage2.Controls.Add(this.label45);
            this.tabPage2.Controls.Add(this.label40);
            this.tabPage2.Controls.Add(this.btnDay);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.label11);
            this.tabPage2.Controls.Add(this.lbVersion);
            this.tabPage2.Controls.Add(this.label17);
            this.tabPage2.Controls.Add(this.label14);
            this.tabPage2.Controls.Add(this.label16);
            this.tabPage2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPage2.Location = new System.Drawing.Point(4, 32);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(584, 324);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "About";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lbLocalIP);
            this.groupBox2.Controls.Add(this.label33);
            this.groupBox2.Controls.Add(this.label44);
            this.groupBox2.Controls.Add(this.label43);
            this.groupBox2.Controls.Add(this.label24);
            this.groupBox2.Controls.Add(this.label22);
            this.groupBox2.Controls.Add(this.label23);
            this.groupBox2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(294, 31);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(278, 168);
            this.groupBox2.TabIndex = 96;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Local UDP";
            this.groupBox2.Paint += new System.Windows.Forms.PaintEventHandler(this.GroupBoxPaint);
            // 
            // lbLocalIP
            // 
            this.lbLocalIP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbLocalIP.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbLocalIP.Location = new System.Drawing.Point(134, 30);
            this.lbLocalIP.Name = "lbLocalIP";
            this.lbLocalIP.Size = new System.Drawing.Size(130, 25);
            this.lbLocalIP.TabIndex = 156;
            this.lbLocalIP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label33
            // 
            this.label33.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label33.Location = new System.Drawing.Point(9, 131);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(255, 23);
            this.label33.TabIndex = 173;
            this.label33.Text = "AgOpenGPS UDP must be on.";
            this.label33.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label44.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label44.Location = new System.Drawing.Point(212, 65);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(52, 25);
            this.label44.TabIndex = 158;
            this.label44.Text = "9999";
            this.label44.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label43.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label43.Location = new System.Drawing.Point(212, 98);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(52, 25);
            this.label43.TabIndex = 160;
            this.label43.Text = "8500";
            this.label43.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.Location = new System.Drawing.Point(8, 99);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(113, 23);
            this.label24.TabIndex = 170;
            this.label24.Text = "Receive Port";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.Location = new System.Drawing.Point(8, 31);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(126, 23);
            this.label22.TabIndex = 168;
            this.label22.Text = "Destination IP";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(8, 66);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(92, 23);
            this.label23.TabIndex = 169;
            this.label23.Text = "Send Port";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbDestinationIP);
            this.groupBox1.Controls.Add(this.lbNetworkIP);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.lbNetReceive);
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(10, 31);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(278, 168);
            this.groupBox1.TabIndex = 96;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Network UDP";
            this.groupBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.GroupBoxPaint);
            // 
            // lbDestinationIP
            // 
            this.lbDestinationIP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbDestinationIP.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDestinationIP.Location = new System.Drawing.Point(133, 59);
            this.lbDestinationIP.Name = "lbDestinationIP";
            this.lbDestinationIP.Size = new System.Drawing.Size(130, 25);
            this.lbDestinationIP.TabIndex = 176;
            this.lbDestinationIP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbNetworkIP
            // 
            this.lbNetworkIP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbNetworkIP.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbNetworkIP.Location = new System.Drawing.Point(133, 25);
            this.lbNetworkIP.Name = "lbNetworkIP";
            this.lbNetworkIP.Size = new System.Drawing.Size(130, 25);
            this.lbNetworkIP.TabIndex = 146;
            this.lbNetworkIP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(7, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 23);
            this.label1.TabIndex = 175;
            this.label1.Text = "Destination IP";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label13.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(211, 93);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(52, 25);
            this.label13.TabIndex = 148;
            this.label13.Text = "9999";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbNetReceive
            // 
            this.lbNetReceive.AutoSize = true;
            this.lbNetReceive.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbNetReceive.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbNetReceive.Location = new System.Drawing.Point(211, 127);
            this.lbNetReceive.Name = "lbNetReceive";
            this.lbNetReceive.Size = new System.Drawing.Size(52, 25);
            this.lbNetReceive.TabIndex = 150;
            this.lbNetReceive.Text = "8000";
            this.lbNetReceive.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(7, 128);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(113, 23);
            this.label19.TabIndex = 163;
            this.label19.Text = "Receive Port";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(7, 94);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(92, 23);
            this.label18.TabIndex = 162;
            this.label18.Text = "Send Port";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(7, 26);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(99, 23);
            this.label12.TabIndex = 161;
            this.label12.Text = "IP Address";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(6, 201);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(0, 23);
            this.label25.TabIndex = 165;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.Location = new System.Drawing.Point(7, 234);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(0, 23);
            this.label26.TabIndex = 166;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label29.Location = new System.Drawing.Point(6, 267);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(0, 23);
            this.label29.TabIndex = 167;
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label42.Location = new System.Drawing.Point(25, 253);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(0, 23);
            this.label42.TabIndex = 157;
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label45.Location = new System.Drawing.Point(24, 286);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(0, 23);
            this.label45.TabIndex = 159;
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label40.Location = new System.Drawing.Point(24, 218);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(0, 23);
            this.label40.TabIndex = 155;
            // 
            // btnDay
            // 
            this.btnDay.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnDay.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnDay.Image = ((System.Drawing.Image)(resources.GetObject("btnDay.Image")));
            this.btnDay.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnDay.Location = new System.Drawing.Point(13, 210);
            this.btnDay.Name = "btnDay";
            this.btnDay.Size = new System.Drawing.Size(209, 72);
            this.btnDay.TabIndex = 154;
            this.btnDay.Text = "Day/Night";
            this.btnDay.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnDay.UseVisualStyleBackColor = true;
            this.btnDay.Click += new System.EventHandler(this.btnDay_Click);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Tahoma", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(91, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(238, 23);
            this.label4.TabIndex = 153;
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(23, 53);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(0, 23);
            this.label11.TabIndex = 145;
            // 
            // lbVersion
            // 
            this.lbVersion.AutoSize = true;
            this.lbVersion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbVersion.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbVersion.Location = new System.Drawing.Point(480, 259);
            this.lbVersion.Name = "lbVersion";
            this.lbVersion.Size = new System.Drawing.Size(92, 20);
            this.lbVersion.TabIndex = 152;
            this.lbVersion.Text = "10/Jan/2020";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(340, 260);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(91, 18);
            this.label17.TabIndex = 151;
            this.label17.Text = "Version Date";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(24, 115);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(0, 23);
            this.label14.TabIndex = 147;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(23, 119);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(0, 23);
            this.label16.TabIndex = 149;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(611, 379);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.Text = "IMU App";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.FormMain_Resize);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblCurrentBaud;
        private System.Windows.Forms.ComboBox cboxBaud;
        private System.Windows.Forms.Label lbArduinoConnected;
        private System.Windows.Forms.Button btnRescan;
        private System.Windows.Forms.Button btnOpenSerialArduino;
        private System.Windows.Forms.Button btnCloseSerialArduino;
        private System.Windows.Forms.ComboBox cboxArdPort;
        private System.Windows.Forms.Label lblCurrentArduinoPort;
        private System.Windows.Forms.TextBox tbDisplay;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Button bntOK;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
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
        private System.Windows.Forms.Label lbNetReceive;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbDestinationIP;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox ckBoxViewData;
        private System.Windows.Forms.CheckBox ckInvert;
        private System.Windows.Forms.RadioButton rbPitch;
        private System.Windows.Forms.RadioButton radioButton1;
    }
}

