namespace RateController.Menu
{
    partial class frmMenuOptions
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.ckRateDisplay = new System.Windows.Forms.CheckBox();
            this.ckMetric = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabSpeed = new System.Windows.Forms.TabPage();
            this.rbIsoBusSpeed = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbAOG = new System.Windows.Forms.RadioButton();
            this.btnCal = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbPulses = new System.Windows.Forms.Label();
            this.lbCal = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lbPin = new System.Windows.Forms.Label();
            this.butUpdateModules = new System.Windows.Forms.Button();
            this.tbWheelPin = new System.Windows.Forms.TextBox();
            this.rbSimulated = new System.Windows.Forms.RadioButton();
            this.tbWheelCal = new System.Windows.Forms.TextBox();
            this.tbSimSpeed = new System.Windows.Forms.TextBox();
            this.lbModule = new System.Windows.Forms.Label();
            this.rbWheel = new System.Windows.Forms.RadioButton();
            this.tbWheelModule = new System.Windows.Forms.TextBox();
            this.lbSimUnits = new System.Windows.Forms.Label();
            this.tabIsoBus = new System.Windows.Forms.TabPage();
            this.gbxPort = new System.Windows.Forms.GroupBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.cbComPort = new System.Windows.Forms.ComboBox();
            this.lbDriverFound = new System.Windows.Forms.Label();
            this.ckDiagnostics = new System.Windows.Forms.CheckBox();
            this.lbConnected = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.gbxDrivers = new System.Windows.Forms.GroupBox();
            this.rbAdapter1 = new System.Windows.Forms.RadioButton();
            this.rbAdapter3 = new System.Windows.Forms.RadioButton();
            this.rbAdapter2 = new System.Windows.Forms.RadioButton();
            this.ckIsoBus = new System.Windows.Forms.CheckBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tabControl1.SuspendLayout();
            this.tabSpeed.SuspendLayout();
            this.tabIsoBus.SuspendLayout();
            this.gbxPort.SuspendLayout();
            this.gbxDrivers.SuspendLayout();
            this.SuspendLayout();
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
            this.btnCancel.Location = new System.Drawing.Point(380, 603);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(6);
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
            this.btnOK.Location = new System.Drawing.Point(458, 603);
            this.btnOK.Margin = new System.Windows.Forms.Padding(6);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(70, 63);
            this.btnOK.TabIndex = 161;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // ckRateDisplay
            // 
            this.ckRateDisplay.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckRateDisplay.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckRateDisplay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckRateDisplay.Location = new System.Drawing.Point(300, 524);
            this.ckRateDisplay.Margin = new System.Windows.Forms.Padding(6);
            this.ckRateDisplay.Name = "ckRateDisplay";
            this.ckRateDisplay.Size = new System.Drawing.Size(192, 36);
            this.ckRateDisplay.TabIndex = 334;
            this.ckRateDisplay.Text = "Rate Display";
            this.ckRateDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckRateDisplay.UseVisualStyleBackColor = true;
            this.ckRateDisplay.CheckedChanged += new System.EventHandler(this.ckLargeScreen_CheckedChanged);
            // 
            // ckMetric
            // 
            this.ckMetric.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckMetric.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckMetric.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckMetric.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckMetric.Location = new System.Drawing.Point(45, 524);
            this.ckMetric.Margin = new System.Windows.Forms.Padding(6);
            this.ckMetric.Name = "ckMetric";
            this.ckMetric.Size = new System.Drawing.Size(192, 36);
            this.ckMetric.TabIndex = 340;
            this.ckMetric.Text = "Metric Units";
            this.ckMetric.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckMetric.UseVisualStyleBackColor = true;
            this.ckMetric.CheckedChanged += new System.EventHandler(this.ckLargeScreen_CheckedChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabControl1.Controls.Add(this.tabSpeed);
            this.tabControl1.Controls.Add(this.tabIsoBus);
            this.tabControl1.ItemSize = new System.Drawing.Size(100, 50);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(516, 469);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.TabIndex = 390;
            // 
            // tabSpeed
            // 
            this.tabSpeed.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tabSpeed.Controls.Add(this.rbIsoBusSpeed);
            this.tabSpeed.Controls.Add(this.groupBox2);
            this.tabSpeed.Controls.Add(this.rbAOG);
            this.tabSpeed.Controls.Add(this.btnCal);
            this.tabSpeed.Controls.Add(this.groupBox1);
            this.tabSpeed.Controls.Add(this.lbPulses);
            this.tabSpeed.Controls.Add(this.lbCal);
            this.tabSpeed.Controls.Add(this.groupBox3);
            this.tabSpeed.Controls.Add(this.lbPin);
            this.tabSpeed.Controls.Add(this.butUpdateModules);
            this.tabSpeed.Controls.Add(this.tbWheelPin);
            this.tabSpeed.Controls.Add(this.rbSimulated);
            this.tabSpeed.Controls.Add(this.tbWheelCal);
            this.tabSpeed.Controls.Add(this.tbSimSpeed);
            this.tabSpeed.Controls.Add(this.lbModule);
            this.tabSpeed.Controls.Add(this.rbWheel);
            this.tabSpeed.Controls.Add(this.tbWheelModule);
            this.tabSpeed.Controls.Add(this.lbSimUnits);
            this.tabSpeed.Location = new System.Drawing.Point(4, 54);
            this.tabSpeed.Name = "tabSpeed";
            this.tabSpeed.Padding = new System.Windows.Forms.Padding(3);
            this.tabSpeed.Size = new System.Drawing.Size(508, 411);
            this.tabSpeed.TabIndex = 0;
            this.tabSpeed.Text = "Speed Source";
            this.tabSpeed.UseVisualStyleBackColor = true;
            // 
            // rbIsoBusSpeed
            // 
            this.rbIsoBusSpeed.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbIsoBusSpeed.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbIsoBusSpeed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbIsoBusSpeed.Location = new System.Drawing.Point(162, 61);
            this.rbIsoBusSpeed.Name = "rbIsoBusSpeed";
            this.rbIsoBusSpeed.Size = new System.Drawing.Size(187, 36);
            this.rbIsoBusSpeed.TabIndex = 348;
            this.rbIsoBusSpeed.Text = "ISOBUS";
            this.rbIsoBusSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbIsoBusSpeed.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Location = new System.Drawing.Point(15, 108);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(480, 3);
            this.groupBox2.TabIndex = 347;
            this.groupBox2.TabStop = false;
            // 
            // rbAOG
            // 
            this.rbAOG.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbAOG.Checked = true;
            this.rbAOG.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbAOG.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbAOG.Location = new System.Drawing.Point(162, 8);
            this.rbAOG.Name = "rbAOG";
            this.rbAOG.Size = new System.Drawing.Size(187, 36);
            this.rbAOG.TabIndex = 244;
            this.rbAOG.TabStop = true;
            this.rbAOG.Text = "GPS";
            this.rbAOG.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbAOG.UseVisualStyleBackColor = true;
            // 
            // btnCal
            // 
            this.btnCal.BackColor = System.Drawing.Color.Transparent;
            this.btnCal.FlatAppearance.BorderSize = 0;
            this.btnCal.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkSlateBlue;
            this.btnCal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCal.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCal.Image = global::RateController.Properties.Resources.clock;
            this.btnCal.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnCal.Location = new System.Drawing.Point(282, 330);
            this.btnCal.Name = "btnCal";
            this.btnCal.Size = new System.Drawing.Size(192, 71);
            this.btnCal.TabIndex = 346;
            this.btnCal.Text = "Calibrate";
            this.btnCal.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCal.UseVisualStyleBackColor = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(18, 51);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(480, 3);
            this.groupBox1.TabIndex = 342;
            this.groupBox1.TabStop = false;
            // 
            // lbPulses
            // 
            this.lbPulses.Enabled = false;
            this.lbPulses.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPulses.Location = new System.Drawing.Point(391, 308);
            this.lbPulses.Name = "lbPulses";
            this.lbPulses.Size = new System.Drawing.Size(100, 29);
            this.lbPulses.TabIndex = 345;
            this.lbPulses.Text = "(pulses/mile)";
            this.lbPulses.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lbCal
            // 
            this.lbCal.AutoSize = true;
            this.lbCal.Enabled = false;
            this.lbCal.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCal.Location = new System.Drawing.Point(333, 280);
            this.lbCal.Name = "lbCal";
            this.lbCal.Size = new System.Drawing.Size(52, 24);
            this.lbCal.TabIndex = 223;
            this.lbCal.Text = "Cal #";
            // 
            // groupBox3
            // 
            this.groupBox3.Location = new System.Drawing.Point(15, 210);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(480, 3);
            this.groupBox3.TabIndex = 344;
            this.groupBox3.TabStop = false;
            // 
            // lbPin
            // 
            this.lbPin.AutoSize = true;
            this.lbPin.Enabled = false;
            this.lbPin.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPin.Location = new System.Drawing.Point(196, 280);
            this.lbPin.Name = "lbPin";
            this.lbPin.Size = new System.Drawing.Size(37, 24);
            this.lbPin.TabIndex = 221;
            this.lbPin.Text = "Pin";
            // 
            // butUpdateModules
            // 
            this.butUpdateModules.BackColor = System.Drawing.Color.Transparent;
            this.butUpdateModules.FlatAppearance.BorderSize = 0;
            this.butUpdateModules.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkSlateBlue;
            this.butUpdateModules.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butUpdateModules.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.butUpdateModules.Image = global::RateController.Properties.Resources.UpArrow64;
            this.butUpdateModules.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.butUpdateModules.Location = new System.Drawing.Point(27, 330);
            this.butUpdateModules.Name = "butUpdateModules";
            this.butUpdateModules.Size = new System.Drawing.Size(192, 71);
            this.butUpdateModules.TabIndex = 342;
            this.butUpdateModules.Text = "Send to Module";
            this.butUpdateModules.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.butUpdateModules.UseVisualStyleBackColor = false;
            // 
            // tbWheelPin
            // 
            this.tbWheelPin.Enabled = false;
            this.tbWheelPin.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbWheelPin.Location = new System.Drawing.Point(239, 278);
            this.tbWheelPin.Name = "tbWheelPin";
            this.tbWheelPin.Size = new System.Drawing.Size(58, 29);
            this.tbWheelPin.TabIndex = 239;
            this.tbWheelPin.TabStop = false;
            this.tbWheelPin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // rbSimulated
            // 
            this.rbSimulated.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbSimulated.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbSimulated.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbSimulated.Location = new System.Drawing.Point(162, 121);
            this.rbSimulated.Name = "rbSimulated";
            this.rbSimulated.Size = new System.Drawing.Size(187, 36);
            this.rbSimulated.TabIndex = 246;
            this.rbSimulated.Text = "Simulated Speed";
            this.rbSimulated.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbSimulated.UseVisualStyleBackColor = true;
            // 
            // tbWheelCal
            // 
            this.tbWheelCal.Enabled = false;
            this.tbWheelCal.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbWheelCal.Location = new System.Drawing.Point(391, 278);
            this.tbWheelCal.Name = "tbWheelCal";
            this.tbWheelCal.Size = new System.Drawing.Size(100, 29);
            this.tbWheelCal.TabIndex = 240;
            this.tbWheelCal.TabStop = false;
            this.tbWheelCal.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbSimSpeed
            // 
            this.tbSimSpeed.Enabled = false;
            this.tbSimSpeed.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSimSpeed.Location = new System.Drawing.Point(200, 170);
            this.tbSimSpeed.Margin = new System.Windows.Forms.Padding(6);
            this.tbSimSpeed.MaxLength = 8;
            this.tbSimSpeed.Name = "tbSimSpeed";
            this.tbSimSpeed.Size = new System.Drawing.Size(78, 30);
            this.tbSimSpeed.TabIndex = 338;
            this.tbSimSpeed.Text = "0";
            this.tbSimSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbModule
            // 
            this.lbModule.AutoSize = true;
            this.lbModule.Enabled = false;
            this.lbModule.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbModule.Location = new System.Drawing.Point(15, 280);
            this.lbModule.Name = "lbModule";
            this.lbModule.Size = new System.Drawing.Size(74, 24);
            this.lbModule.TabIndex = 242;
            this.lbModule.Text = "Module";
            // 
            // rbWheel
            // 
            this.rbWheel.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbWheel.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbWheel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbWheel.Location = new System.Drawing.Point(162, 226);
            this.rbWheel.Name = "rbWheel";
            this.rbWheel.Size = new System.Drawing.Size(187, 36);
            this.rbWheel.TabIndex = 245;
            this.rbWheel.Text = "Wheel Sensor";
            this.rbWheel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbWheel.UseVisualStyleBackColor = true;
            // 
            // tbWheelModule
            // 
            this.tbWheelModule.Enabled = false;
            this.tbWheelModule.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbWheelModule.Location = new System.Drawing.Point(95, 278);
            this.tbWheelModule.Name = "tbWheelModule";
            this.tbWheelModule.Size = new System.Drawing.Size(58, 29);
            this.tbWheelModule.TabIndex = 243;
            this.tbWheelModule.TabStop = false;
            this.tbWheelModule.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbSimUnits
            // 
            this.lbSimUnits.AutoSize = true;
            this.lbSimUnits.Enabled = false;
            this.lbSimUnits.ForeColor = System.Drawing.Color.DarkGreen;
            this.lbSimUnits.Location = new System.Drawing.Point(290, 173);
            this.lbSimUnits.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbSimUnits.Name = "lbSimUnits";
            this.lbSimUnits.Size = new System.Drawing.Size(48, 24);
            this.lbSimUnits.TabIndex = 339;
            this.lbSimUnits.Text = "mph";
            this.lbSimUnits.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabIsoBus
            // 
            this.tabIsoBus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tabIsoBus.Controls.Add(this.gbxPort);
            this.tabIsoBus.Controls.Add(this.lbDriverFound);
            this.tabIsoBus.Controls.Add(this.ckDiagnostics);
            this.tabIsoBus.Controls.Add(this.lbConnected);
            this.tabIsoBus.Controls.Add(this.label2);
            this.tabIsoBus.Controls.Add(this.label13);
            this.tabIsoBus.Controls.Add(this.gbxDrivers);
            this.tabIsoBus.Controls.Add(this.ckIsoBus);
            this.tabIsoBus.Location = new System.Drawing.Point(4, 54);
            this.tabIsoBus.Name = "tabIsoBus";
            this.tabIsoBus.Padding = new System.Windows.Forms.Padding(3);
            this.tabIsoBus.Size = new System.Drawing.Size(508, 411);
            this.tabIsoBus.TabIndex = 1;
            this.tabIsoBus.Text = "ISOBUS";
            this.tabIsoBus.UseVisualStyleBackColor = true;
            // 
            // gbxPort
            // 
            this.gbxPort.Controls.Add(this.btnRefresh);
            this.gbxPort.Controls.Add(this.cbComPort);
            this.gbxPort.Location = new System.Drawing.Point(379, 69);
            this.gbxPort.Name = "gbxPort";
            this.gbxPort.Size = new System.Drawing.Size(119, 149);
            this.gbxPort.TabIndex = 357;
            this.gbxPort.TabStop = false;
            this.gbxPort.Text = "Port";
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.Transparent;
            this.btnRefresh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnRefresh.Image = global::RateController.Properties.Resources.Update;
            this.btnRefresh.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnRefresh.Location = new System.Drawing.Point(20, 74);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(6);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(70, 63);
            this.btnRefresh.TabIndex = 356;
            this.btnRefresh.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnRefresh.UseVisualStyleBackColor = false;
            // 
            // cbComPort
            // 
            this.cbComPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbComPort.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.cbComPort.FormattingEnabled = true;
            this.cbComPort.Location = new System.Drawing.Point(9, 34);
            this.cbComPort.Name = "cbComPort";
            this.cbComPort.Size = new System.Drawing.Size(100, 31);
            this.cbComPort.TabIndex = 353;
            // 
            // lbDriverFound
            // 
            this.lbDriverFound.BackColor = System.Drawing.SystemColors.Control;
            this.lbDriverFound.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDriverFound.Image = global::RateController.Properties.Resources.Off;
            this.lbDriverFound.Location = new System.Drawing.Point(137, 367);
            this.lbDriverFound.Name = "lbDriverFound";
            this.lbDriverFound.Size = new System.Drawing.Size(41, 37);
            this.lbDriverFound.TabIndex = 352;
            this.lbDriverFound.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ckDiagnostics
            // 
            this.ckDiagnostics.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckDiagnostics.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckDiagnostics.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckDiagnostics.Location = new System.Drawing.Point(149, 306);
            this.ckDiagnostics.Margin = new System.Windows.Forms.Padding(6);
            this.ckDiagnostics.Name = "ckDiagnostics";
            this.ckDiagnostics.Size = new System.Drawing.Size(192, 36);
            this.ckDiagnostics.TabIndex = 350;
            this.ckDiagnostics.Text = "Diagnostics";
            this.ckDiagnostics.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckDiagnostics.UseVisualStyleBackColor = true;
            // 
            // lbConnected
            // 
            this.lbConnected.BackColor = System.Drawing.SystemColors.Control;
            this.lbConnected.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbConnected.Image = global::RateController.Properties.Resources.Off;
            this.lbConnected.Location = new System.Drawing.Point(457, 367);
            this.lbConnected.Name = "lbConnected";
            this.lbConnected.Size = new System.Drawing.Size(41, 37);
            this.lbConnected.TabIndex = 349;
            this.lbConnected.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(9, 374);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 23);
            this.label2.TabIndex = 351;
            this.label2.Text = "Driver Found";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(221, 374);
            this.label13.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(227, 23);
            this.label13.TabIndex = 348;
            this.label13.Text = "IsoBus Module Connected";
            // 
            // gbxDrivers
            // 
            this.gbxDrivers.Controls.Add(this.rbAdapter1);
            this.gbxDrivers.Controls.Add(this.rbAdapter3);
            this.gbxDrivers.Controls.Add(this.rbAdapter2);
            this.gbxDrivers.Location = new System.Drawing.Point(123, 69);
            this.gbxDrivers.Name = "gbxDrivers";
            this.gbxDrivers.Size = new System.Drawing.Size(250, 223);
            this.gbxDrivers.TabIndex = 347;
            this.gbxDrivers.TabStop = false;
            this.gbxDrivers.Text = "Can Driver";
            // 
            // rbAdapter1
            // 
            this.rbAdapter1.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbAdapter1.Checked = true;
            this.rbAdapter1.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbAdapter1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbAdapter1.Location = new System.Drawing.Point(31, 41);
            this.rbAdapter1.Name = "rbAdapter1";
            this.rbAdapter1.Size = new System.Drawing.Size(187, 36);
            this.rbAdapter1.TabIndex = 343;
            this.rbAdapter1.TabStop = true;
            this.rbAdapter1.Text = "SLCAN";
            this.rbAdapter1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbAdapter1.UseVisualStyleBackColor = true;
            // 
            // rbAdapter3
            // 
            this.rbAdapter3.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbAdapter3.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbAdapter3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbAdapter3.Location = new System.Drawing.Point(31, 157);
            this.rbAdapter3.Name = "rbAdapter3";
            this.rbAdapter3.Size = new System.Drawing.Size(187, 36);
            this.rbAdapter3.TabIndex = 346;
            this.rbAdapter3.Text = "PCAN";
            this.rbAdapter3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbAdapter3.UseVisualStyleBackColor = true;
            // 
            // rbAdapter2
            // 
            this.rbAdapter2.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbAdapter2.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbAdapter2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbAdapter2.Location = new System.Drawing.Point(31, 99);
            this.rbAdapter2.Name = "rbAdapter2";
            this.rbAdapter2.Size = new System.Drawing.Size(187, 36);
            this.rbAdapter2.TabIndex = 345;
            this.rbAdapter2.Text = "InnoMaker";
            this.rbAdapter2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbAdapter2.UseVisualStyleBackColor = true;
            // 
            // ckIsoBus
            // 
            this.ckIsoBus.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckIsoBus.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckIsoBus.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckIsoBus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckIsoBus.Location = new System.Drawing.Point(24, 24);
            this.ckIsoBus.Margin = new System.Windows.Forms.Padding(6);
            this.ckIsoBus.Name = "ckIsoBus";
            this.ckIsoBus.Size = new System.Drawing.Size(458, 36);
            this.ckIsoBus.TabIndex = 342;
            this.ckIsoBus.Text = "Enabled";
            this.ckIsoBus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckIsoBus.UseVisualStyleBackColor = true;
            this.ckIsoBus.CheckedChanged += new System.EventHandler(this.ckIsobusEnabled_CheckedChanged);
            // 
            // frmMenuOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 678);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.ckMetric);
            this.Controls.Add(this.ckRateDisplay);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "frmMenuOptions";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuDisplay";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMenuDisplay_FormClosed);
            this.Load += new System.EventHandler(this.frmMenuDisplay_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabSpeed.ResumeLayout(false);
            this.tabSpeed.PerformLayout();
            this.tabIsoBus.ResumeLayout(false);
            this.tabIsoBus.PerformLayout();
            this.gbxPort.ResumeLayout(false);
            this.gbxDrivers.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox ckMetric;
        private System.Windows.Forms.CheckBox ckRateDisplay;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabSpeed;
        private System.Windows.Forms.RadioButton rbIsoBusSpeed;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbAOG;
        private System.Windows.Forms.Button btnCal;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbPulses;
        private System.Windows.Forms.Label lbCal;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lbPin;
        private System.Windows.Forms.Button butUpdateModules;
        private System.Windows.Forms.TextBox tbWheelPin;
        private System.Windows.Forms.RadioButton rbSimulated;
        private System.Windows.Forms.TextBox tbWheelCal;
        private System.Windows.Forms.TextBox tbSimSpeed;
        private System.Windows.Forms.Label lbModule;
        private System.Windows.Forms.RadioButton rbWheel;
        private System.Windows.Forms.TextBox tbWheelModule;
        private System.Windows.Forms.Label lbSimUnits;
        private System.Windows.Forms.TabPage tabIsoBus;
        private System.Windows.Forms.GroupBox gbxPort;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.ComboBox cbComPort;
        private System.Windows.Forms.Label lbDriverFound;
        private System.Windows.Forms.CheckBox ckDiagnostics;
        private System.Windows.Forms.Label lbConnected;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.GroupBox gbxDrivers;
        private System.Windows.Forms.RadioButton rbAdapter1;
        private System.Windows.Forms.RadioButton rbAdapter3;
        private System.Windows.Forms.RadioButton rbAdapter2;
        private System.Windows.Forms.CheckBox ckIsoBus;
        private System.Windows.Forms.Timer timer1;
    }
}