namespace PCBsetup.Forms
{
    partial class frmNanoSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNanoSettings));
            this.btnLoadDefaults = new System.Windows.Forms.Button();
            this.btnSendToModule = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.bntOK = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tbNanoDebounce = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.lbIPpart4 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.tbNanoIP = new System.Windows.Forms.TextBox();
            this.label29 = new System.Windows.Forms.Label();
            this.tbNanoSensorCount = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbNanoModuleID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ckNanoFlowOn = new System.Windows.Forms.CheckBox();
            this.ckUseMCP23017 = new System.Windows.Forms.CheckBox();
            this.ckNanoRelayOn = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tbRelay16 = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.tbRelay15 = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.tbRelay14 = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.tbRelay13 = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.tbRelay12 = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.tbRelay11 = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.tbRelay10 = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.tbRelay9 = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.tbRelay8 = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.tbRelay7 = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.tbRelay6 = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.tbRelay5 = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.tbRelay4 = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.tbRelay3 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.tbRelay2 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbRelay1 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tbNanoPWM2 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbNanoPWM1 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbNanoDir2 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbNanoDir1 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbNanoFlow2 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbNanoFlow1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLoadDefaults
            // 
            this.btnLoadDefaults.BackColor = System.Drawing.Color.Transparent;
            this.btnLoadDefaults.FlatAppearance.BorderSize = 0;
            this.btnLoadDefaults.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadDefaults.Image = global::PCBsetup.Properties.Resources.VehFileLoad;
            this.btnLoadDefaults.Location = new System.Drawing.Point(18, 523);
            this.btnLoadDefaults.Name = "btnLoadDefaults";
            this.btnLoadDefaults.Size = new System.Drawing.Size(115, 72);
            this.btnLoadDefaults.TabIndex = 19;
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
            this.btnSendToModule.Location = new System.Drawing.Point(153, 523);
            this.btnSendToModule.Name = "btnSendToModule";
            this.btnSendToModule.Size = new System.Drawing.Size(115, 72);
            this.btnSendToModule.TabIndex = 18;
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
            this.btnCancel.Location = new System.Drawing.Point(288, 523);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(115, 72);
            this.btnCancel.TabIndex = 17;
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
            this.bntOK.Location = new System.Drawing.Point(423, 523);
            this.bntOK.Name = "bntOK";
            this.bntOK.Size = new System.Drawing.Size(115, 72);
            this.bntOK.TabIndex = 16;
            this.bntOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.bntOK.UseVisualStyleBackColor = false;
            this.bntOK.Click += new System.EventHandler(this.bntOK_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(530, 505);
            this.tabControl1.TabIndex = 20;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tbNanoDebounce);
            this.tabPage1.Controls.Add(this.label25);
            this.tabPage1.Controls.Add(this.lbIPpart4);
            this.tabPage1.Controls.Add(this.label30);
            this.tabPage1.Controls.Add(this.tbNanoIP);
            this.tabPage1.Controls.Add(this.label29);
            this.tabPage1.Controls.Add(this.tbNanoSensorCount);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.tbNanoModuleID);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.ckNanoFlowOn);
            this.tabPage1.Controls.Add(this.ckUseMCP23017);
            this.tabPage1.Controls.Add(this.ckNanoRelayOn);
            this.tabPage1.Location = new System.Drawing.Point(4, 33);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(522, 468);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Config 1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tbNanoDebounce
            // 
            this.tbNanoDebounce.Location = new System.Drawing.Point(287, 228);
            this.tbNanoDebounce.Name = "tbNanoDebounce";
            this.tbNanoDebounce.Size = new System.Drawing.Size(58, 29);
            this.tbNanoDebounce.TabIndex = 29;
            this.tbNanoDebounce.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbNanoDebounce.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.tbNanoDebounce_HelpRequested);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(60, 230);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(99, 24);
            this.label25.TabIndex = 28;
            this.label25.Text = "Debounce";
            // 
            // lbIPpart4
            // 
            this.lbIPpart4.AutoSize = true;
            this.lbIPpart4.Location = new System.Drawing.Point(351, 180);
            this.lbIPpart4.Name = "lbIPpart4";
            this.lbIPpart4.Size = new System.Drawing.Size(45, 24);
            this.lbIPpart4.TabIndex = 27;
            this.lbIPpart4.Text = ".xxx";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(201, 180);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(80, 24);
            this.label30.TabIndex = 26;
            this.label30.Text = "192.168.";
            // 
            // tbNanoIP
            // 
            this.tbNanoIP.Location = new System.Drawing.Point(287, 178);
            this.tbNanoIP.Name = "tbNanoIP";
            this.tbNanoIP.Size = new System.Drawing.Size(58, 29);
            this.tbNanoIP.TabIndex = 25;
            this.tbNanoIP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(60, 180);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(101, 24);
            this.label29.TabIndex = 24;
            this.label29.Text = "IP Address";
            // 
            // tbNanoSensorCount
            // 
            this.tbNanoSensorCount.Location = new System.Drawing.Point(287, 128);
            this.tbNanoSensorCount.Name = "tbNanoSensorCount";
            this.tbNanoSensorCount.Size = new System.Drawing.Size(58, 29);
            this.tbNanoSensorCount.TabIndex = 23;
            this.tbNanoSensorCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbNanoSensorCount.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.tbNanoSensorCount_HelpRequested);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(60, 130);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 24);
            this.label2.TabIndex = 22;
            this.label2.Text = "Sensor Count";
            // 
            // tbNanoModuleID
            // 
            this.tbNanoModuleID.Location = new System.Drawing.Point(287, 78);
            this.tbNanoModuleID.Name = "tbNanoModuleID";
            this.tbNanoModuleID.Size = new System.Drawing.Size(58, 29);
            this.tbNanoModuleID.TabIndex = 21;
            this.tbNanoModuleID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(60, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 24);
            this.label1.TabIndex = 20;
            this.label1.Text = "Module ID";
            // 
            // ckNanoFlowOn
            // 
            this.ckNanoFlowOn.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckNanoFlowOn.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ckNanoFlowOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckNanoFlowOn.Location = new System.Drawing.Point(181, 311);
            this.ckNanoFlowOn.Name = "ckNanoFlowOn";
            this.ckNanoFlowOn.Size = new System.Drawing.Size(117, 69);
            this.ckNanoFlowOn.TabIndex = 19;
            this.ckNanoFlowOn.Text = "Flow on High";
            this.ckNanoFlowOn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckNanoFlowOn.UseVisualStyleBackColor = true;
            this.ckNanoFlowOn.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ckNanoFlowOn_HelpRequested);
            // 
            // ckUseMCP23017
            // 
            this.ckUseMCP23017.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckUseMCP23017.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ckUseMCP23017.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckUseMCP23017.Location = new System.Drawing.Point(314, 311);
            this.ckUseMCP23017.Name = "ckUseMCP23017";
            this.ckUseMCP23017.Size = new System.Drawing.Size(117, 69);
            this.ckUseMCP23017.TabIndex = 18;
            this.ckUseMCP23017.Text = "Use MCP23017";
            this.ckUseMCP23017.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckUseMCP23017.UseVisualStyleBackColor = true;
            this.ckUseMCP23017.CheckedChanged += new System.EventHandler(this.ckUseMCP23017_CheckedChanged);
            this.ckUseMCP23017.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ckUseMCP23017_HelpRequested);
            // 
            // ckNanoRelayOn
            // 
            this.ckNanoRelayOn.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckNanoRelayOn.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ckNanoRelayOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckNanoRelayOn.Location = new System.Drawing.Point(48, 311);
            this.ckNanoRelayOn.Name = "ckNanoRelayOn";
            this.ckNanoRelayOn.Size = new System.Drawing.Size(117, 69);
            this.ckNanoRelayOn.TabIndex = 17;
            this.ckNanoRelayOn.Text = "Relay on High";
            this.ckNanoRelayOn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckNanoRelayOn.UseVisualStyleBackColor = true;
            this.ckNanoRelayOn.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ckNanoRelayOn_HelpRequested);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tbRelay16);
            this.tabPage2.Controls.Add(this.label13);
            this.tabPage2.Controls.Add(this.tbRelay15);
            this.tabPage2.Controls.Add(this.label15);
            this.tabPage2.Controls.Add(this.tbRelay14);
            this.tabPage2.Controls.Add(this.label16);
            this.tabPage2.Controls.Add(this.tbRelay13);
            this.tabPage2.Controls.Add(this.label17);
            this.tabPage2.Controls.Add(this.tbRelay12);
            this.tabPage2.Controls.Add(this.label18);
            this.tabPage2.Controls.Add(this.tbRelay11);
            this.tabPage2.Controls.Add(this.label19);
            this.tabPage2.Controls.Add(this.tbRelay10);
            this.tabPage2.Controls.Add(this.label20);
            this.tabPage2.Controls.Add(this.tbRelay9);
            this.tabPage2.Controls.Add(this.label21);
            this.tabPage2.Controls.Add(this.tbRelay8);
            this.tabPage2.Controls.Add(this.label22);
            this.tabPage2.Controls.Add(this.tbRelay7);
            this.tabPage2.Controls.Add(this.label23);
            this.tabPage2.Controls.Add(this.tbRelay6);
            this.tabPage2.Controls.Add(this.label24);
            this.tabPage2.Controls.Add(this.tbRelay5);
            this.tabPage2.Controls.Add(this.label14);
            this.tabPage2.Controls.Add(this.tbRelay4);
            this.tabPage2.Controls.Add(this.label11);
            this.tabPage2.Controls.Add(this.tbRelay3);
            this.tabPage2.Controls.Add(this.label12);
            this.tabPage2.Controls.Add(this.tbRelay2);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.tbRelay1);
            this.tabPage2.Controls.Add(this.label10);
            this.tabPage2.Controls.Add(this.tbNanoPWM2);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.tbNanoPWM1);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.tbNanoDir2);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.tbNanoDir1);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.tbNanoFlow2);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.tbNanoFlow1);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Location = new System.Drawing.Point(4, 33);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(522, 468);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Nano Pins";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tbRelay16
            // 
            this.tbRelay16.Location = new System.Drawing.Point(405, 426);
            this.tbRelay16.Name = "tbRelay16";
            this.tbRelay16.Size = new System.Drawing.Size(58, 29);
            this.tbRelay16.TabIndex = 67;
            this.tbRelay16.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(284, 428);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(82, 24);
            this.label13.TabIndex = 66;
            this.label13.Text = "Relay 16";
            // 
            // tbRelay15
            // 
            this.tbRelay15.Location = new System.Drawing.Point(405, 384);
            this.tbRelay15.Name = "tbRelay15";
            this.tbRelay15.Size = new System.Drawing.Size(58, 29);
            this.tbRelay15.TabIndex = 65;
            this.tbRelay15.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(284, 386);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(82, 24);
            this.label15.TabIndex = 64;
            this.label15.Text = "Relay 15";
            // 
            // tbRelay14
            // 
            this.tbRelay14.Location = new System.Drawing.Point(405, 342);
            this.tbRelay14.Name = "tbRelay14";
            this.tbRelay14.Size = new System.Drawing.Size(58, 29);
            this.tbRelay14.TabIndex = 63;
            this.tbRelay14.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(284, 344);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(82, 24);
            this.label16.TabIndex = 62;
            this.label16.Text = "Relay 14";
            // 
            // tbRelay13
            // 
            this.tbRelay13.Location = new System.Drawing.Point(405, 300);
            this.tbRelay13.Name = "tbRelay13";
            this.tbRelay13.Size = new System.Drawing.Size(58, 29);
            this.tbRelay13.TabIndex = 61;
            this.tbRelay13.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(284, 302);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(82, 24);
            this.label17.TabIndex = 60;
            this.label17.Text = "Relay 13";
            // 
            // tbRelay12
            // 
            this.tbRelay12.Location = new System.Drawing.Point(405, 258);
            this.tbRelay12.Name = "tbRelay12";
            this.tbRelay12.Size = new System.Drawing.Size(58, 29);
            this.tbRelay12.TabIndex = 59;
            this.tbRelay12.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(284, 260);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(82, 24);
            this.label18.TabIndex = 58;
            this.label18.Text = "Relay 12";
            // 
            // tbRelay11
            // 
            this.tbRelay11.Location = new System.Drawing.Point(405, 216);
            this.tbRelay11.Name = "tbRelay11";
            this.tbRelay11.Size = new System.Drawing.Size(58, 29);
            this.tbRelay11.TabIndex = 57;
            this.tbRelay11.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(284, 218);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(82, 24);
            this.label19.TabIndex = 56;
            this.label19.Text = "Relay 11";
            // 
            // tbRelay10
            // 
            this.tbRelay10.Location = new System.Drawing.Point(405, 174);
            this.tbRelay10.Name = "tbRelay10";
            this.tbRelay10.Size = new System.Drawing.Size(58, 29);
            this.tbRelay10.TabIndex = 55;
            this.tbRelay10.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(284, 176);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(82, 24);
            this.label20.TabIndex = 54;
            this.label20.Text = "Relay 10";
            // 
            // tbRelay9
            // 
            this.tbRelay9.Location = new System.Drawing.Point(405, 132);
            this.tbRelay9.Name = "tbRelay9";
            this.tbRelay9.Size = new System.Drawing.Size(58, 29);
            this.tbRelay9.TabIndex = 53;
            this.tbRelay9.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(284, 134);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(72, 24);
            this.label21.TabIndex = 52;
            this.label21.Text = "Relay 9";
            // 
            // tbRelay8
            // 
            this.tbRelay8.Location = new System.Drawing.Point(405, 90);
            this.tbRelay8.Name = "tbRelay8";
            this.tbRelay8.Size = new System.Drawing.Size(58, 29);
            this.tbRelay8.TabIndex = 51;
            this.tbRelay8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(284, 92);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(72, 24);
            this.label22.TabIndex = 50;
            this.label22.Text = "Relay 8";
            // 
            // tbRelay7
            // 
            this.tbRelay7.Location = new System.Drawing.Point(405, 48);
            this.tbRelay7.Name = "tbRelay7";
            this.tbRelay7.Size = new System.Drawing.Size(58, 29);
            this.tbRelay7.TabIndex = 49;
            this.tbRelay7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(284, 50);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(72, 24);
            this.label23.TabIndex = 48;
            this.label23.Text = "Relay 7";
            // 
            // tbRelay6
            // 
            this.tbRelay6.Location = new System.Drawing.Point(405, 6);
            this.tbRelay6.Name = "tbRelay6";
            this.tbRelay6.Size = new System.Drawing.Size(58, 29);
            this.tbRelay6.TabIndex = 47;
            this.tbRelay6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(284, 8);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(72, 24);
            this.label24.TabIndex = 46;
            this.label24.Text = "Relay 6";
            // 
            // tbRelay5
            // 
            this.tbRelay5.Location = new System.Drawing.Point(160, 426);
            this.tbRelay5.Name = "tbRelay5";
            this.tbRelay5.Size = new System.Drawing.Size(58, 29);
            this.tbRelay5.TabIndex = 45;
            this.tbRelay5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(39, 428);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(72, 24);
            this.label14.TabIndex = 44;
            this.label14.Text = "Relay 5";
            // 
            // tbRelay4
            // 
            this.tbRelay4.Location = new System.Drawing.Point(160, 384);
            this.tbRelay4.Name = "tbRelay4";
            this.tbRelay4.Size = new System.Drawing.Size(58, 29);
            this.tbRelay4.TabIndex = 43;
            this.tbRelay4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(39, 386);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(72, 24);
            this.label11.TabIndex = 42;
            this.label11.Text = "Relay 4";
            // 
            // tbRelay3
            // 
            this.tbRelay3.Location = new System.Drawing.Point(160, 342);
            this.tbRelay3.Name = "tbRelay3";
            this.tbRelay3.Size = new System.Drawing.Size(58, 29);
            this.tbRelay3.TabIndex = 41;
            this.tbRelay3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(39, 344);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(72, 24);
            this.label12.TabIndex = 40;
            this.label12.Text = "Relay 3";
            // 
            // tbRelay2
            // 
            this.tbRelay2.Location = new System.Drawing.Point(160, 300);
            this.tbRelay2.Name = "tbRelay2";
            this.tbRelay2.Size = new System.Drawing.Size(58, 29);
            this.tbRelay2.TabIndex = 39;
            this.tbRelay2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(39, 302);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(72, 24);
            this.label9.TabIndex = 38;
            this.label9.Text = "Relay 2";
            // 
            // tbRelay1
            // 
            this.tbRelay1.Location = new System.Drawing.Point(160, 258);
            this.tbRelay1.Name = "tbRelay1";
            this.tbRelay1.Size = new System.Drawing.Size(58, 29);
            this.tbRelay1.TabIndex = 37;
            this.tbRelay1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(39, 260);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(72, 24);
            this.label10.TabIndex = 36;
            this.label10.Text = "Relay 1";
            // 
            // tbNanoPWM2
            // 
            this.tbNanoPWM2.Location = new System.Drawing.Point(160, 216);
            this.tbNanoPWM2.Name = "tbNanoPWM2";
            this.tbNanoPWM2.Size = new System.Drawing.Size(58, 29);
            this.tbNanoPWM2.TabIndex = 35;
            this.tbNanoPWM2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(39, 218);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 24);
            this.label7.TabIndex = 34;
            this.label7.Text = "PWM 2";
            // 
            // tbNanoPWM1
            // 
            this.tbNanoPWM1.Location = new System.Drawing.Point(160, 174);
            this.tbNanoPWM1.Name = "tbNanoPWM1";
            this.tbNanoPWM1.Size = new System.Drawing.Size(58, 29);
            this.tbNanoPWM1.TabIndex = 33;
            this.tbNanoPWM1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(39, 176);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 24);
            this.label8.TabIndex = 32;
            this.label8.Text = "PWM 1";
            // 
            // tbNanoDir2
            // 
            this.tbNanoDir2.Location = new System.Drawing.Point(160, 132);
            this.tbNanoDir2.Name = "tbNanoDir2";
            this.tbNanoDir2.Size = new System.Drawing.Size(58, 29);
            this.tbNanoDir2.TabIndex = 31;
            this.tbNanoDir2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(39, 134);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 24);
            this.label5.TabIndex = 30;
            this.label5.Text = "Dir 2";
            // 
            // tbNanoDir1
            // 
            this.tbNanoDir1.Location = new System.Drawing.Point(160, 90);
            this.tbNanoDir1.Name = "tbNanoDir1";
            this.tbNanoDir1.Size = new System.Drawing.Size(58, 29);
            this.tbNanoDir1.TabIndex = 29;
            this.tbNanoDir1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(39, 92);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 24);
            this.label6.TabIndex = 28;
            this.label6.Text = "Dir 1";
            // 
            // tbNanoFlow2
            // 
            this.tbNanoFlow2.Location = new System.Drawing.Point(160, 48);
            this.tbNanoFlow2.Name = "tbNanoFlow2";
            this.tbNanoFlow2.Size = new System.Drawing.Size(58, 29);
            this.tbNanoFlow2.TabIndex = 27;
            this.tbNanoFlow2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(39, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 24);
            this.label3.TabIndex = 26;
            this.label3.Text = "Flow 2";
            // 
            // tbNanoFlow1
            // 
            this.tbNanoFlow1.Location = new System.Drawing.Point(160, 6);
            this.tbNanoFlow1.Name = "tbNanoFlow1";
            this.tbNanoFlow1.Size = new System.Drawing.Size(58, 29);
            this.tbNanoFlow1.TabIndex = 25;
            this.tbNanoFlow1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbNanoFlow1.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.tbNanoFlow1_HelpRequested);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(39, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 24);
            this.label4.TabIndex = 24;
            this.label4.Text = "Flow 1";
            // 
            // frmNanoSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(554, 606);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnLoadDefaults);
            this.Controls.Add(this.btnSendToModule);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.bntOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmNanoSettings";
            this.ShowInTaskbar = false;
            this.Text = "Nano Rate Settings";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmNanoSettings_FormClosed);
            this.Load += new System.EventHandler(this.frmNanoSettings_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnLoadDefaults;
        private System.Windows.Forms.Button btnSendToModule;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button bntOK;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckBox ckNanoFlowOn;
        private System.Windows.Forms.CheckBox ckUseMCP23017;
        private System.Windows.Forms.CheckBox ckNanoRelayOn;
        private System.Windows.Forms.TextBox tbNanoSensorCount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbNanoModuleID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbRelay16;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox tbRelay15;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox tbRelay14;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox tbRelay13;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox tbRelay12;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox tbRelay11;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox tbRelay10;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox tbRelay9;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox tbRelay8;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox tbRelay7;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox tbRelay6;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox tbRelay5;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox tbRelay4;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tbRelay3;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox tbRelay2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbRelay1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbNanoPWM2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbNanoPWM1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbNanoDir2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbNanoDir1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbNanoFlow2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbNanoFlow1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbIPpart4;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.TextBox tbNanoIP;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.TextBox tbNanoDebounce;
        private System.Windows.Forms.Label label25;
    }
}