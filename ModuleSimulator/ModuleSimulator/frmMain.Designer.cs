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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.sendTimer = new System.Windows.Forms.Timer(this.components);
            this.loopTimer = new System.Windows.Forms.Timer(this.components);
            this.grpModule = new System.Windows.Forms.GroupBox();
            this.lblModuleID = new System.Windows.Forms.Label();
            this.nudModuleID = new System.Windows.Forms.NumericUpDown();
            this.lblSensorID = new System.Windows.Forms.Label();
            this.nudSensorID = new System.Windows.Forms.NumericUpDown();
            this.lblSubnet = new System.Windows.Forms.Label();
            this.txtSubnet = new System.Windows.Forms.TextBox();
            this.grpSensor = new System.Windows.Forms.GroupBox();
            this.lblSimRateCaption = new System.Windows.Forms.Label();
            this.lblSimRate = new System.Windows.Forms.Label();
            this.lblAccQtyCaption = new System.Windows.Forms.Label();
            this.lblAccQty = new System.Windows.Forms.Label();
            this.btnResetQty = new System.Windows.Forms.Button();
            this.lblHzCaption = new System.Windows.Forms.Label();
            this.lblHz = new System.Windows.Forms.Label();
            this.lblPWMCaption = new System.Windows.Forms.Label();
            this.lblPWM = new System.Windows.Forms.Label();
            this.grpSim = new System.Windows.Forms.GroupBox();
            this.lblMaxHzCap = new System.Windows.Forms.Label();
            this.nudMaxHz = new System.Windows.Forms.NumericUpDown();
            this.lblNoiseCap = new System.Windows.Forms.Label();
            this.nudNoise = new System.Windows.Forms.NumericUpDown();
            this.lblValveLagCap = new System.Windows.Forms.Label();
            this.nudValveLag = new System.Windows.Forms.NumericUpDown();
            this.lblValvePosCap = new System.Windows.Forms.Label();
            this.lblValvePos = new System.Windows.Forms.Label();
            this.grpModStatus = new System.Windows.Forms.GroupBox();
            this.lblWheelSpeedCaption = new System.Windows.Forms.Label();
            this.nudWheelSpeed = new System.Windows.Forms.NumericUpDown();
            this.lblPressureCaption = new System.Windows.Forms.Label();
            this.nudPressure = new System.Windows.Forms.NumericUpDown();
            this.ckWorkSwitch = new System.Windows.Forms.CheckBox();
            this.grpCommands = new System.Windows.Forms.GroupBox();
            this.lblSetRateCaption = new System.Windows.Forms.Label();
            this.lblCmdSetRate = new System.Windows.Forms.Label();
            this.lblFlowCalRxCaption = new System.Windows.Forms.Label();
            this.lblCmdFlowCal = new System.Windows.Forms.Label();
            this.lblMasterOnCaption = new System.Windows.Forms.Label();
            this.lblCmdMasterOn = new System.Windows.Forms.Label();
            this.lblAutoOnCaption = new System.Windows.Forms.Label();
            this.lblCmdAutoOn = new System.Windows.Forms.Label();
            this.lblRelaysCaption = new System.Windows.Forms.Label();
            this.lblCmdRelays = new System.Windows.Forms.Label();
            this.lblConfigCaption = new System.Windows.Forms.Label();
            this.lblCmdConfig = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.grpModule.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudModuleID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSensorID)).BeginInit();
            this.grpSensor.SuspendLayout();
            this.grpSim.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxHz)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNoise)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudValveLag)).BeginInit();
            this.grpModStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWheelSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPressure)).BeginInit();
            this.grpCommands.SuspendLayout();
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
            // grpModule
            // 
            this.grpModule.Controls.Add(this.lblModuleID);
            this.grpModule.Controls.Add(this.nudModuleID);
            this.grpModule.Controls.Add(this.lblSensorID);
            this.grpModule.Controls.Add(this.nudSensorID);
            this.grpModule.Controls.Add(this.lblSubnet);
            this.grpModule.Controls.Add(this.txtSubnet);
            this.grpModule.Location = new System.Drawing.Point(8, 8);
            this.grpModule.Name = "grpModule";
            this.grpModule.Size = new System.Drawing.Size(474, 88);
            this.grpModule.TabIndex = 0;
            this.grpModule.TabStop = false;
            this.grpModule.Text = "Module";
            // 
            // lblModuleID
            // 
            this.lblModuleID.AutoSize = true;
            this.lblModuleID.Location = new System.Drawing.Point(10, 26);
            this.lblModuleID.Name = "lblModuleID";
            this.lblModuleID.Size = new System.Drawing.Size(65, 15);
            this.lblModuleID.TabIndex = 0;
            this.lblModuleID.Text = "Module ID:";
            // 
            // nudModuleID
            // 
            this.nudModuleID.Location = new System.Drawing.Point(110, 24);
            this.nudModuleID.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.nudModuleID.Name = "nudModuleID";
            this.nudModuleID.Size = new System.Drawing.Size(55, 23);
            this.nudModuleID.TabIndex = 1;
            // 
            // lblSensorID
            // 
            this.lblSensorID.AutoSize = true;
            this.lblSensorID.Location = new System.Drawing.Point(10, 56);
            this.lblSensorID.Name = "lblSensorID";
            this.lblSensorID.Size = new System.Drawing.Size(59, 15);
            this.lblSensorID.TabIndex = 2;
            this.lblSensorID.Text = "Sensor ID:";
            // 
            // nudSensorID
            // 
            this.nudSensorID.Location = new System.Drawing.Point(110, 54);
            this.nudSensorID.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSensorID.Name = "nudSensorID";
            this.nudSensorID.Size = new System.Drawing.Size(55, 23);
            this.nudSensorID.TabIndex = 3;
            // 
            // lblSubnet
            // 
            this.lblSubnet.AutoSize = true;
            this.lblSubnet.Location = new System.Drawing.Point(210, 26);
            this.lblSubnet.Name = "lblSubnet";
            this.lblSubnet.Size = new System.Drawing.Size(47, 15);
            this.lblSubnet.TabIndex = 4;
            this.lblSubnet.Text = "Subnet:";
            // 
            // txtSubnet
            // 
            this.txtSubnet.Location = new System.Drawing.Point(270, 24);
            this.txtSubnet.Name = "txtSubnet";
            this.txtSubnet.Size = new System.Drawing.Size(130, 23);
            this.txtSubnet.TabIndex = 5;
            this.txtSubnet.Text = "192.168.1";
            // 
            // grpSensor
            // 
            this.grpSensor.Controls.Add(this.lblSimRateCaption);
            this.grpSensor.Controls.Add(this.lblSimRate);
            this.grpSensor.Controls.Add(this.lblAccQtyCaption);
            this.grpSensor.Controls.Add(this.lblAccQty);
            this.grpSensor.Controls.Add(this.btnResetQty);
            this.grpSensor.Controls.Add(this.lblHzCaption);
            this.grpSensor.Controls.Add(this.lblHz);
            this.grpSensor.Controls.Add(this.lblPWMCaption);
            this.grpSensor.Controls.Add(this.lblPWM);
            this.grpSensor.Location = new System.Drawing.Point(8, 104);
            this.grpSensor.Name = "grpSensor";
            this.grpSensor.Size = new System.Drawing.Size(474, 96);
            this.grpSensor.TabIndex = 1;
            this.grpSensor.TabStop = false;
            this.grpSensor.Text = "Sensor Output";
            // 
            // lblSimRateCaption
            // 
            this.lblSimRateCaption.AutoSize = true;
            this.lblSimRateCaption.Location = new System.Drawing.Point(10, 28);
            this.lblSimRateCaption.Name = "lblSimRateCaption";
            this.lblSimRateCaption.Size = new System.Drawing.Size(73, 15);
            this.lblSimRateCaption.TabIndex = 0;
            this.lblSimRateCaption.Text = "Actual UPM:";
            // 
            // lblSimRate
            // 
            this.lblSimRate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblSimRate.Location = new System.Drawing.Point(150, 28);
            this.lblSimRate.Name = "lblSimRate";
            this.lblSimRate.Size = new System.Drawing.Size(80, 18);
            this.lblSimRate.TabIndex = 1;
            this.lblSimRate.Text = "0.0";
            // 
            // lblAccQtyCaption
            // 
            this.lblAccQtyCaption.AutoSize = true;
            this.lblAccQtyCaption.Location = new System.Drawing.Point(10, 58);
            this.lblAccQtyCaption.Name = "lblAccQtyCaption";
            this.lblAccQtyCaption.Size = new System.Drawing.Size(52, 15);
            this.lblAccQtyCaption.TabIndex = 2;
            this.lblAccQtyCaption.Text = "Acc Qty:";
            // 
            // lblAccQty
            // 
            this.lblAccQty.Location = new System.Drawing.Point(150, 58);
            this.lblAccQty.Name = "lblAccQty";
            this.lblAccQty.Size = new System.Drawing.Size(80, 18);
            this.lblAccQty.TabIndex = 3;
            this.lblAccQty.Text = "0.0";
            // 
            // btnResetQty
            // 
            this.btnResetQty.Location = new System.Drawing.Point(234, 54);
            this.btnResetQty.Name = "btnResetQty";
            this.btnResetQty.Size = new System.Drawing.Size(55, 24);
            this.btnResetQty.TabIndex = 4;
            this.btnResetQty.Text = "Reset";
            this.btnResetQty.Click += new System.EventHandler(this.btnResetQty_Click);
            // 
            // lblHzCaption
            // 
            this.lblHzCaption.AutoSize = true;
            this.lblHzCaption.Location = new System.Drawing.Point(320, 26);
            this.lblHzCaption.Name = "lblHzCaption";
            this.lblHzCaption.Size = new System.Drawing.Size(24, 15);
            this.lblHzCaption.TabIndex = 5;
            this.lblHzCaption.Text = "Hz:";
            // 
            // lblHz
            // 
            this.lblHz.Location = new System.Drawing.Point(380, 26);
            this.lblHz.Name = "lblHz";
            this.lblHz.Size = new System.Drawing.Size(80, 18);
            this.lblHz.TabIndex = 6;
            this.lblHz.Text = "0.0";
            // 
            // lblPWMCaption
            // 
            this.lblPWMCaption.AutoSize = true;
            this.lblPWMCaption.Location = new System.Drawing.Point(320, 58);
            this.lblPWMCaption.Name = "lblPWMCaption";
            this.lblPWMCaption.Size = new System.Drawing.Size(39, 15);
            this.lblPWMCaption.TabIndex = 7;
            this.lblPWMCaption.Text = "PWM:";
            // 
            // lblPWM
            // 
            this.lblPWM.Location = new System.Drawing.Point(380, 58);
            this.lblPWM.Name = "lblPWM";
            this.lblPWM.Size = new System.Drawing.Size(80, 18);
            this.lblPWM.TabIndex = 8;
            this.lblPWM.Text = "0";
            // 
            // grpSim
            // 
            this.grpSim.Controls.Add(this.lblMaxHzCap);
            this.grpSim.Controls.Add(this.nudMaxHz);
            this.grpSim.Controls.Add(this.lblNoiseCap);
            this.grpSim.Controls.Add(this.nudNoise);
            this.grpSim.Controls.Add(this.lblValveLagCap);
            this.grpSim.Controls.Add(this.nudValveLag);
            this.grpSim.Controls.Add(this.lblValvePosCap);
            this.grpSim.Controls.Add(this.lblValvePos);
            this.grpSim.Location = new System.Drawing.Point(8, 208);
            this.grpSim.Name = "grpSim";
            this.grpSim.Size = new System.Drawing.Size(474, 88);
            this.grpSim.TabIndex = 2;
            this.grpSim.TabStop = false;
            this.grpSim.Text = "Flow Simulation";
            // 
            // lblMaxHzCap
            // 
            this.lblMaxHzCap.AutoSize = true;
            this.lblMaxHzCap.Location = new System.Drawing.Point(10, 26);
            this.lblMaxHzCap.Name = "lblMaxHzCap";
            this.lblMaxHzCap.Size = new System.Drawing.Size(108, 15);
            this.lblMaxHzCap.TabIndex = 0;
            this.lblMaxHzCap.Text = "Max Hz (full open):";
            // 
            // nudMaxHz
            // 
            this.nudMaxHz.Location = new System.Drawing.Point(165, 24);
            this.nudMaxHz.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.nudMaxHz.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudMaxHz.Name = "nudMaxHz";
            this.nudMaxHz.Size = new System.Drawing.Size(70, 23);
            this.nudMaxHz.TabIndex = 1;
            this.nudMaxHz.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // lblNoiseCap
            // 
            this.lblNoiseCap.AutoSize = true;
            this.lblNoiseCap.Location = new System.Drawing.Point(255, 26);
            this.lblNoiseCap.Name = "lblNoiseCap";
            this.lblNoiseCap.Size = new System.Drawing.Size(53, 15);
            this.lblNoiseCap.TabIndex = 2;
            this.lblNoiseCap.Text = "Noise %:";
            // 
            // nudNoise
            // 
            this.nudNoise.Location = new System.Drawing.Point(330, 24);
            this.nudNoise.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudNoise.Name = "nudNoise";
            this.nudNoise.Size = new System.Drawing.Size(55, 23);
            this.nudNoise.TabIndex = 3;
            this.nudNoise.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // lblValveLagCap
            // 
            this.lblValveLagCap.AutoSize = true;
            this.lblValveLagCap.Location = new System.Drawing.Point(10, 57);
            this.lblValveLagCap.Name = "lblValveLagCap";
            this.lblValveLagCap.Size = new System.Drawing.Size(96, 15);
            this.lblValveLagCap.TabIndex = 4;
            this.lblValveLagCap.Text = "Valve travel (ms):";
            // 
            // nudValveLag
            // 
            this.nudValveLag.Increment = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nudValveLag.Location = new System.Drawing.Point(165, 55);
            this.nudValveLag.Maximum = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            this.nudValveLag.Minimum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nudValveLag.Name = "nudValveLag";
            this.nudValveLag.Size = new System.Drawing.Size(80, 23);
            this.nudValveLag.TabIndex = 5;
            this.nudValveLag.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // lblValvePosCap
            // 
            this.lblValvePosCap.AutoSize = true;
            this.lblValvePosCap.Location = new System.Drawing.Point(255, 57);
            this.lblValvePosCap.Name = "lblValvePosCap";
            this.lblValvePosCap.Size = new System.Drawing.Size(59, 15);
            this.lblValvePosCap.TabIndex = 6;
            this.lblValvePosCap.Text = "Valve Pos:";
            // 
            // lblValvePos
            // 
            this.lblValvePos.Location = new System.Drawing.Point(330, 57);
            this.lblValvePos.Name = "lblValvePos";
            this.lblValvePos.Size = new System.Drawing.Size(55, 18);
            this.lblValvePos.TabIndex = 7;
            this.lblValvePos.Text = "0";
            // 
            // grpModStatus
            // 
            this.grpModStatus.Controls.Add(this.lblWheelSpeedCaption);
            this.grpModStatus.Controls.Add(this.nudWheelSpeed);
            this.grpModStatus.Controls.Add(this.lblPressureCaption);
            this.grpModStatus.Controls.Add(this.nudPressure);
            this.grpModStatus.Controls.Add(this.ckWorkSwitch);
            this.grpModStatus.Location = new System.Drawing.Point(8, 304);
            this.grpModStatus.Name = "grpModStatus";
            this.grpModStatus.Size = new System.Drawing.Size(474, 90);
            this.grpModStatus.TabIndex = 3;
            this.grpModStatus.TabStop = false;
            this.grpModStatus.Text = "Module Status";
            // 
            // lblWheelSpeedCaption
            // 
            this.lblWheelSpeedCaption.AutoSize = true;
            this.lblWheelSpeedCaption.Location = new System.Drawing.Point(10, 26);
            this.lblWheelSpeedCaption.Name = "lblWheelSpeedCaption";
            this.lblWheelSpeedCaption.Size = new System.Drawing.Size(118, 15);
            this.lblWheelSpeedCaption.TabIndex = 0;
            this.lblWheelSpeedCaption.Text = "Wheel Speed (km/h):";
            // 
            // nudWheelSpeed
            // 
            this.nudWheelSpeed.DecimalPlaces = 1;
            this.nudWheelSpeed.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.nudWheelSpeed.Location = new System.Drawing.Point(170, 24);
            this.nudWheelSpeed.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudWheelSpeed.Name = "nudWheelSpeed";
            this.nudWheelSpeed.Size = new System.Drawing.Size(80, 23);
            this.nudWheelSpeed.TabIndex = 1;
            this.nudWheelSpeed.Value = new decimal(new int[] {
            50,
            0,
            0,
            65536});
            // 
            // lblPressureCaption
            // 
            this.lblPressureCaption.AutoSize = true;
            this.lblPressureCaption.Location = new System.Drawing.Point(10, 57);
            this.lblPressureCaption.Name = "lblPressureCaption";
            this.lblPressureCaption.Size = new System.Drawing.Size(54, 15);
            this.lblPressureCaption.TabIndex = 2;
            this.lblPressureCaption.Text = "Pressure:";
            // 
            // nudPressure
            // 
            this.nudPressure.Location = new System.Drawing.Point(170, 55);
            this.nudPressure.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.nudPressure.Name = "nudPressure";
            this.nudPressure.Size = new System.Drawing.Size(80, 23);
            this.nudPressure.TabIndex = 3;
            // 
            // ckWorkSwitch
            // 
            this.ckWorkSwitch.AutoSize = true;
            this.ckWorkSwitch.Location = new System.Drawing.Point(310, 26);
            this.ckWorkSwitch.Name = "ckWorkSwitch";
            this.ckWorkSwitch.Size = new System.Drawing.Size(92, 19);
            this.ckWorkSwitch.TabIndex = 4;
            this.ckWorkSwitch.Text = "Work Switch";
            // 
            // grpCommands
            // 
            this.grpCommands.Controls.Add(this.lblSetRateCaption);
            this.grpCommands.Controls.Add(this.lblCmdSetRate);
            this.grpCommands.Controls.Add(this.lblFlowCalRxCaption);
            this.grpCommands.Controls.Add(this.lblCmdFlowCal);
            this.grpCommands.Controls.Add(this.lblMasterOnCaption);
            this.grpCommands.Controls.Add(this.lblCmdMasterOn);
            this.grpCommands.Controls.Add(this.lblAutoOnCaption);
            this.grpCommands.Controls.Add(this.lblCmdAutoOn);
            this.grpCommands.Controls.Add(this.lblRelaysCaption);
            this.grpCommands.Controls.Add(this.lblCmdRelays);
            this.grpCommands.Controls.Add(this.lblConfigCaption);
            this.grpCommands.Controls.Add(this.lblCmdConfig);
            this.grpCommands.Location = new System.Drawing.Point(8, 402);
            this.grpCommands.Name = "grpCommands";
            this.grpCommands.Size = new System.Drawing.Size(474, 150);
            this.grpCommands.TabIndex = 4;
            this.grpCommands.TabStop = false;
            this.grpCommands.Text = "RC Commands Received";
            // 
            // lblSetRateCaption
            // 
            this.lblSetRateCaption.AutoSize = true;
            this.lblSetRateCaption.Location = new System.Drawing.Point(10, 24);
            this.lblSetRateCaption.Name = "lblSetRateCaption";
            this.lblSetRateCaption.Size = new System.Drawing.Size(52, 15);
            this.lblSetRateCaption.TabIndex = 0;
            this.lblSetRateCaption.Text = "Set Rate:";
            // 
            // lblCmdSetRate
            // 
            this.lblCmdSetRate.Location = new System.Drawing.Point(120, 24);
            this.lblCmdSetRate.Name = "lblCmdSetRate";
            this.lblCmdSetRate.Size = new System.Drawing.Size(80, 18);
            this.lblCmdSetRate.TabIndex = 1;
            this.lblCmdSetRate.Text = "—";
            // 
            // lblFlowCalRxCaption
            // 
            this.lblFlowCalRxCaption.AutoSize = true;
            this.lblFlowCalRxCaption.Location = new System.Drawing.Point(10, 50);
            this.lblFlowCalRxCaption.Name = "lblFlowCalRxCaption";
            this.lblFlowCalRxCaption.Size = new System.Drawing.Size(55, 15);
            this.lblFlowCalRxCaption.TabIndex = 2;
            this.lblFlowCalRxCaption.Text = "Flow Cal:";
            // 
            // lblCmdFlowCal
            // 
            this.lblCmdFlowCal.Location = new System.Drawing.Point(120, 50);
            this.lblCmdFlowCal.Name = "lblCmdFlowCal";
            this.lblCmdFlowCal.Size = new System.Drawing.Size(80, 18);
            this.lblCmdFlowCal.TabIndex = 3;
            this.lblCmdFlowCal.Text = "—";
            // 
            // lblMasterOnCaption
            // 
            this.lblMasterOnCaption.AutoSize = true;
            this.lblMasterOnCaption.Location = new System.Drawing.Point(10, 76);
            this.lblMasterOnCaption.Name = "lblMasterOnCaption";
            this.lblMasterOnCaption.Size = new System.Drawing.Size(65, 15);
            this.lblMasterOnCaption.TabIndex = 4;
            this.lblMasterOnCaption.Text = "Master On:";
            // 
            // lblCmdMasterOn
            // 
            this.lblCmdMasterOn.ForeColor = System.Drawing.Color.Gray;
            this.lblCmdMasterOn.Location = new System.Drawing.Point(120, 76);
            this.lblCmdMasterOn.Name = "lblCmdMasterOn";
            this.lblCmdMasterOn.Size = new System.Drawing.Size(50, 18);
            this.lblCmdMasterOn.TabIndex = 5;
            this.lblCmdMasterOn.Text = "—";
            // 
            // lblAutoOnCaption
            // 
            this.lblAutoOnCaption.AutoSize = true;
            this.lblAutoOnCaption.Location = new System.Drawing.Point(210, 76);
            this.lblAutoOnCaption.Name = "lblAutoOnCaption";
            this.lblAutoOnCaption.Size = new System.Drawing.Size(55, 15);
            this.lblAutoOnCaption.TabIndex = 6;
            this.lblAutoOnCaption.Text = "Auto On:";
            // 
            // lblCmdAutoOn
            // 
            this.lblCmdAutoOn.ForeColor = System.Drawing.Color.Gray;
            this.lblCmdAutoOn.Location = new System.Drawing.Point(290, 76);
            this.lblCmdAutoOn.Name = "lblCmdAutoOn";
            this.lblCmdAutoOn.Size = new System.Drawing.Size(50, 18);
            this.lblCmdAutoOn.TabIndex = 7;
            this.lblCmdAutoOn.Text = "—";
            // 
            // lblRelaysCaption
            // 
            this.lblRelaysCaption.AutoSize = true;
            this.lblRelaysCaption.Location = new System.Drawing.Point(10, 102);
            this.lblRelaysCaption.Name = "lblRelaysCaption";
            this.lblRelaysCaption.Size = new System.Drawing.Size(77, 15);
            this.lblRelaysCaption.TabIndex = 8;
            this.lblRelaysCaption.Text = "Relays (0-15):";
            // 
            // lblCmdRelays
            // 
            this.lblCmdRelays.Font = new System.Drawing.Font("Courier New", 9F);
            this.lblCmdRelays.Location = new System.Drawing.Point(120, 102);
            this.lblCmdRelays.Name = "lblCmdRelays";
            this.lblCmdRelays.Size = new System.Drawing.Size(170, 18);
            this.lblCmdRelays.TabIndex = 9;
            this.lblCmdRelays.Text = "—";
            // 
            // lblConfigCaption
            // 
            this.lblConfigCaption.AutoSize = true;
            this.lblConfigCaption.Location = new System.Drawing.Point(10, 126);
            this.lblConfigCaption.Name = "lblConfigCaption";
            this.lblConfigCaption.Size = new System.Drawing.Size(87, 15);
            this.lblConfigCaption.TabIndex = 10;
            this.lblConfigCaption.Text = "Config (32700):";
            // 
            // lblCmdConfig
            // 
            this.lblCmdConfig.ForeColor = System.Drawing.Color.Gray;
            this.lblCmdConfig.Location = new System.Drawing.Point(120, 126);
            this.lblCmdConfig.Name = "lblCmdConfig";
            this.lblCmdConfig.Size = new System.Drawing.Size(120, 18);
            this.lblCmdConfig.TabIndex = 11;
            this.lblCmdConfig.Text = "—";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(8, 562);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(80, 30);
            this.btnStart.TabIndex = 5;
            this.btnStart.Text = "Start";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(96, 562);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(80, 30);
            this.btnStop.TabIndex = 6;
            this.btnStop.Text = "Stop";
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblStatus.Location = new System.Drawing.Point(190, 569);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(280, 18);
            this.lblStatus.TabIndex = 7;
            this.lblStatus.Text = "Stopped";
            // 
            // frmMain
            // 
            this.ClientSize = new System.Drawing.Size(492, 602);
            this.Controls.Add(this.grpModule);
            this.Controls.Add(this.grpSensor);
            this.Controls.Add(this.grpSim);
            this.Controls.Add(this.grpModStatus);
            this.Controls.Add(this.grpCommands);
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
            this.grpModule.ResumeLayout(false);
            this.grpModule.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudModuleID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSensorID)).EndInit();
            this.grpSensor.ResumeLayout(false);
            this.grpSensor.PerformLayout();
            this.grpSim.ResumeLayout(false);
            this.grpSim.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxHz)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNoise)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudValveLag)).EndInit();
            this.grpModStatus.ResumeLayout(false);
            this.grpModStatus.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWheelSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPressure)).EndInit();
            this.grpCommands.ResumeLayout(false);
            this.grpCommands.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        // ── Control declarations ──────────────────────────────────────────────────
        private System.Windows.Forms.Timer sendTimer;
        private System.Windows.Forms.Timer loopTimer;

        private System.Windows.Forms.GroupBox      grpModule;
        private System.Windows.Forms.Label         lblModuleID;
        private System.Windows.Forms.NumericUpDown nudModuleID;
        private System.Windows.Forms.Label         lblSensorID;
        private System.Windows.Forms.NumericUpDown nudSensorID;
        private System.Windows.Forms.Label         lblSubnet;
        private System.Windows.Forms.TextBox       txtSubnet;

        private System.Windows.Forms.GroupBox      grpSensor;
        private System.Windows.Forms.Label         lblSimRateCaption;
        private System.Windows.Forms.Label         lblSimRate;
        private System.Windows.Forms.Label         lblAccQtyCaption;
        private System.Windows.Forms.Label         lblAccQty;
        private System.Windows.Forms.Button        btnResetQty;
        private System.Windows.Forms.Label         lblHzCaption;
        private System.Windows.Forms.Label         lblHz;
        private System.Windows.Forms.Label         lblPWMCaption;
        private System.Windows.Forms.Label         lblPWM;

        private System.Windows.Forms.GroupBox      grpSim;
        private System.Windows.Forms.Label         lblMaxHzCap;
        private System.Windows.Forms.NumericUpDown nudMaxHz;
        private System.Windows.Forms.Label         lblNoiseCap;
        private System.Windows.Forms.NumericUpDown nudNoise;
        private System.Windows.Forms.Label         lblValveLagCap;
        private System.Windows.Forms.NumericUpDown nudValveLag;
        private System.Windows.Forms.Label         lblValvePosCap;
        private System.Windows.Forms.Label         lblValvePos;

        private System.Windows.Forms.GroupBox      grpModStatus;
        private System.Windows.Forms.Label         lblWheelSpeedCaption;
        private System.Windows.Forms.NumericUpDown nudWheelSpeed;
        private System.Windows.Forms.Label         lblPressureCaption;
        private System.Windows.Forms.NumericUpDown nudPressure;
        private System.Windows.Forms.CheckBox      ckWorkSwitch;

        private System.Windows.Forms.GroupBox      grpCommands;
        private System.Windows.Forms.Label         lblSetRateCaption;
        private System.Windows.Forms.Label         lblCmdSetRate;
        private System.Windows.Forms.Label         lblFlowCalRxCaption;
        private System.Windows.Forms.Label         lblCmdFlowCal;
        private System.Windows.Forms.Label         lblMasterOnCaption;
        private System.Windows.Forms.Label         lblCmdMasterOn;
        private System.Windows.Forms.Label         lblAutoOnCaption;
        private System.Windows.Forms.Label         lblCmdAutoOn;
        private System.Windows.Forms.Label         lblRelaysCaption;
        private System.Windows.Forms.Label         lblCmdRelays;
        private System.Windows.Forms.Label         lblConfigCaption;
        private System.Windows.Forms.Label         lblCmdConfig;

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label  lblStatus;
    }
}
