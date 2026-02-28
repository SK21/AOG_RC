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

            // Timers
            this.sendTimer = new System.Windows.Forms.Timer(this.components);
            this.loopTimer = new System.Windows.Forms.Timer(this.components);

            // Module group
            this.grpModule        = new System.Windows.Forms.GroupBox();
            this.lblModuleID      = new System.Windows.Forms.Label();
            this.nudModuleID      = new System.Windows.Forms.NumericUpDown();
            this.lblSensorID      = new System.Windows.Forms.Label();
            this.nudSensorID      = new System.Windows.Forms.NumericUpDown();
            this.lblSubnet        = new System.Windows.Forms.Label();
            this.txtSubnet        = new System.Windows.Forms.TextBox();

            // Sensor output group
            this.grpSensor         = new System.Windows.Forms.GroupBox();
            this.lblSimRateCaption = new System.Windows.Forms.Label();
            this.lblSimRate        = new System.Windows.Forms.Label();
            this.lblAccQtyCaption  = new System.Windows.Forms.Label();
            this.lblAccQty         = new System.Windows.Forms.Label();
            this.btnResetQty       = new System.Windows.Forms.Button();
            this.lblHzCaption      = new System.Windows.Forms.Label();
            this.lblHz             = new System.Windows.Forms.Label();
            this.lblPWMCaption     = new System.Windows.Forms.Label();
            this.lblPWM            = new System.Windows.Forms.Label();

            // Flow simulation group
            this.grpSim          = new System.Windows.Forms.GroupBox();
            this.lblMaxHzCap     = new System.Windows.Forms.Label();
            this.nudMaxHz        = new System.Windows.Forms.NumericUpDown();
            this.lblNoiseCap     = new System.Windows.Forms.Label();
            this.nudNoise        = new System.Windows.Forms.NumericUpDown();
            this.lblValveLagCap  = new System.Windows.Forms.Label();
            this.nudValveLag     = new System.Windows.Forms.NumericUpDown();
            this.lblValvePosCap  = new System.Windows.Forms.Label();
            this.lblValvePos     = new System.Windows.Forms.Label();

            // Module status group
            this.grpModStatus           = new System.Windows.Forms.GroupBox();
            this.lblWheelSpeedCaption   = new System.Windows.Forms.Label();
            this.nudWheelSpeed          = new System.Windows.Forms.NumericUpDown();
            this.lblPressureCaption     = new System.Windows.Forms.Label();
            this.nudPressure            = new System.Windows.Forms.NumericUpDown();
            this.ckWorkSwitch           = new System.Windows.Forms.CheckBox();

            // RC Commands group
            this.grpCommands          = new System.Windows.Forms.GroupBox();
            this.lblSetRateCaption    = new System.Windows.Forms.Label();
            this.lblCmdSetRate        = new System.Windows.Forms.Label();
            this.lblFlowCalRxCaption  = new System.Windows.Forms.Label();
            this.lblCmdFlowCal        = new System.Windows.Forms.Label();
            this.lblMasterOnCaption   = new System.Windows.Forms.Label();
            this.lblCmdMasterOn       = new System.Windows.Forms.Label();
            this.lblAutoOnCaption     = new System.Windows.Forms.Label();
            this.lblCmdAutoOn         = new System.Windows.Forms.Label();
            this.lblRelaysCaption     = new System.Windows.Forms.Label();
            this.lblCmdRelays         = new System.Windows.Forms.Label();
            this.lblConfigCaption     = new System.Windows.Forms.Label();
            this.lblCmdConfig         = new System.Windows.Forms.Label();

            // Bottom
            this.btnStart  = new System.Windows.Forms.Button();
            this.btnStop   = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();

            // ── sendTimer (200 ms — UDP send, matches Teensy SendTime) ───────────
            this.sendTimer.Interval = 200;
            this.sendTimer.Tick    += new System.EventHandler(this.sendTimer_Tick);

            // ── loopTimer (50 ms — PID + simulation, matches Teensy LoopTime) ────
            this.loopTimer.Interval = 50;
            this.loopTimer.Tick    += new System.EventHandler(this.loopTimer_Tick);

            // ── grpModule ────────────────────────────────────────────────────────
            this.grpModule.Text     = "Module";
            this.grpModule.Location = new System.Drawing.Point(8, 8);
            this.grpModule.Size     = new System.Drawing.Size(474, 88);

            this.lblModuleID.Text     = "Module ID:";
            this.lblModuleID.Location = new System.Drawing.Point(10, 26);
            this.lblModuleID.AutoSize = true;

            this.nudModuleID.Location = new System.Drawing.Point(110, 24);
            this.nudModuleID.Size     = new System.Drawing.Size(55, 22);
            this.nudModuleID.Minimum  = 0;
            this.nudModuleID.Maximum  = 15;

            this.lblSensorID.Text     = "Sensor ID:";
            this.lblSensorID.Location = new System.Drawing.Point(10, 56);
            this.lblSensorID.AutoSize = true;

            this.nudSensorID.Location = new System.Drawing.Point(110, 54);
            this.nudSensorID.Size     = new System.Drawing.Size(55, 22);
            this.nudSensorID.Minimum  = 0;
            this.nudSensorID.Maximum  = 1;

            this.lblSubnet.Text     = "Subnet:";
            this.lblSubnet.Location = new System.Drawing.Point(210, 26);
            this.lblSubnet.AutoSize = true;

            this.txtSubnet.Location = new System.Drawing.Point(270, 24);
            this.txtSubnet.Size     = new System.Drawing.Size(130, 22);
            this.txtSubnet.Text     = "192.168.1";

            this.grpModule.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblModuleID, this.nudModuleID,
                this.lblSensorID, this.nudSensorID,
                this.lblSubnet,   this.txtSubnet });

            // ── grpSensor ────────────────────────────────────────────────────────
            this.grpSensor.Text     = "Sensor Output";
            this.grpSensor.Location = new System.Drawing.Point(8, 104);
            this.grpSensor.Size     = new System.Drawing.Size(474, 96);

            this.lblSimRateCaption.Text     = "Actual UPM:";
            this.lblSimRateCaption.Location = new System.Drawing.Point(10, 28);
            this.lblSimRateCaption.AutoSize = true;

            this.lblSimRate.Text      = "0.0";
            this.lblSimRate.Location  = new System.Drawing.Point(150, 28);
            this.lblSimRate.Size      = new System.Drawing.Size(80, 18);
            this.lblSimRate.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);

            this.lblAccQtyCaption.Text     = "Acc Qty:";
            this.lblAccQtyCaption.Location = new System.Drawing.Point(10, 58);
            this.lblAccQtyCaption.AutoSize = true;

            this.lblAccQty.Text     = "0.0";
            this.lblAccQty.Location = new System.Drawing.Point(150, 58);
            this.lblAccQty.Size     = new System.Drawing.Size(80, 18);

            this.btnResetQty.Text     = "Reset";
            this.btnResetQty.Location = new System.Drawing.Point(234, 54);
            this.btnResetQty.Size     = new System.Drawing.Size(55, 24);
            this.btnResetQty.Click   += new System.EventHandler(this.btnResetQty_Click);

            this.lblHzCaption.Text     = "Hz:";
            this.lblHzCaption.Location = new System.Drawing.Point(320, 26);
            this.lblHzCaption.AutoSize = true;

            this.lblHz.Text     = "0.0";
            this.lblHz.Location = new System.Drawing.Point(380, 26);
            this.lblHz.Size     = new System.Drawing.Size(80, 18);

            this.lblPWMCaption.Text     = "PWM:";
            this.lblPWMCaption.Location = new System.Drawing.Point(320, 58);
            this.lblPWMCaption.AutoSize = true;

            this.lblPWM.Text     = "0";
            this.lblPWM.Location = new System.Drawing.Point(380, 58);
            this.lblPWM.Size     = new System.Drawing.Size(80, 18);

            this.grpSensor.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblSimRateCaption, this.lblSimRate,
                this.lblAccQtyCaption,  this.lblAccQty, this.btnResetQty,
                this.lblHzCaption,      this.lblHz,
                this.lblPWMCaption,     this.lblPWM });

            // ── grpSim ───────────────────────────────────────────────────────────
            this.grpSim.Text     = "Flow Simulation";
            this.grpSim.Location = new System.Drawing.Point(8, 208);
            this.grpSim.Size     = new System.Drawing.Size(474, 88);

            this.lblMaxHzCap.Text     = "Max Hz (full open):";
            this.lblMaxHzCap.Location = new System.Drawing.Point(10, 26);
            this.lblMaxHzCap.AutoSize = true;

            this.nudMaxHz.Location = new System.Drawing.Point(165, 24);
            this.nudMaxHz.Size     = new System.Drawing.Size(70, 22);
            this.nudMaxHz.Minimum  = 1;
            this.nudMaxHz.Maximum  = 2000;
            this.nudMaxHz.Value    = 100;

            this.lblNoiseCap.Text     = "Noise %:";
            this.lblNoiseCap.Location = new System.Drawing.Point(255, 26);
            this.lblNoiseCap.AutoSize = true;

            this.nudNoise.Location = new System.Drawing.Point(330, 24);
            this.nudNoise.Size     = new System.Drawing.Size(55, 22);
            this.nudNoise.Minimum  = 0;
            this.nudNoise.Maximum  = 50;
            this.nudNoise.Value    = 5;

            this.lblValveLagCap.Text     = "Valve travel (ms):";
            this.lblValveLagCap.Location = new System.Drawing.Point(10, 57);
            this.lblValveLagCap.AutoSize = true;

            this.nudValveLag.Location  = new System.Drawing.Point(165, 55);
            this.nudValveLag.Size      = new System.Drawing.Size(80, 22);
            this.nudValveLag.Minimum   = 500;
            this.nudValveLag.Maximum   = 30000;
            this.nudValveLag.Increment = 500;
            this.nudValveLag.Value     = 5000;

            this.lblValvePosCap.Text     = "Valve Pos:";
            this.lblValvePosCap.Location = new System.Drawing.Point(255, 57);
            this.lblValvePosCap.AutoSize = true;

            this.lblValvePos.Text     = "0";
            this.lblValvePos.Location = new System.Drawing.Point(330, 57);
            this.lblValvePos.Size     = new System.Drawing.Size(55, 18);

            this.grpSim.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblMaxHzCap,    this.nudMaxHz,
                this.lblNoiseCap,    this.nudNoise,
                this.lblValveLagCap, this.nudValveLag,
                this.lblValvePosCap, this.lblValvePos });

            // ── grpModStatus ─────────────────────────────────────────────────────
            this.grpModStatus.Text     = "Module Status";
            this.grpModStatus.Location = new System.Drawing.Point(8, 304);
            this.grpModStatus.Size     = new System.Drawing.Size(474, 90);

            this.lblWheelSpeedCaption.Text     = "Wheel Speed (km/h):";
            this.lblWheelSpeedCaption.Location = new System.Drawing.Point(10, 26);
            this.lblWheelSpeedCaption.AutoSize = true;

            this.nudWheelSpeed.Location      = new System.Drawing.Point(170, 24);
            this.nudWheelSpeed.Size          = new System.Drawing.Size(80, 22);
            this.nudWheelSpeed.Minimum       = 0;
            this.nudWheelSpeed.Maximum       = 50;
            this.nudWheelSpeed.DecimalPlaces = 1;
            this.nudWheelSpeed.Increment     = new decimal(new int[] { 5, 0, 0, 65536 });  // 0.5
            this.nudWheelSpeed.Value         = new decimal(new int[] { 50, 0, 0, 65536 }); // 5.0

            this.lblPressureCaption.Text     = "Pressure:";
            this.lblPressureCaption.Location = new System.Drawing.Point(10, 57);
            this.lblPressureCaption.AutoSize = true;

            this.nudPressure.Location = new System.Drawing.Point(170, 55);
            this.nudPressure.Size     = new System.Drawing.Size(80, 22);
            this.nudPressure.Minimum  = 0;
            this.nudPressure.Maximum  = 9999;

            this.ckWorkSwitch.Text     = "Work Switch";
            this.ckWorkSwitch.Location = new System.Drawing.Point(310, 26);
            this.ckWorkSwitch.AutoSize = true;

            this.grpModStatus.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblWheelSpeedCaption, this.nudWheelSpeed,
                this.lblPressureCaption,   this.nudPressure,
                this.ckWorkSwitch });

            // ── grpCommands ──────────────────────────────────────────────────────
            this.grpCommands.Text     = "RC Commands Received";
            this.grpCommands.Location = new System.Drawing.Point(8, 402);
            this.grpCommands.Size     = new System.Drawing.Size(474, 150);

            this.lblSetRateCaption.Text     = "Set Rate:";
            this.lblSetRateCaption.Location = new System.Drawing.Point(10, 24);
            this.lblSetRateCaption.AutoSize = true;

            this.lblCmdSetRate.Text     = "—";
            this.lblCmdSetRate.Location = new System.Drawing.Point(120, 24);
            this.lblCmdSetRate.Size     = new System.Drawing.Size(80, 18);

            this.lblFlowCalRxCaption.Text     = "Flow Cal:";
            this.lblFlowCalRxCaption.Location = new System.Drawing.Point(10, 50);
            this.lblFlowCalRxCaption.AutoSize = true;

            this.lblCmdFlowCal.Text     = "—";
            this.lblCmdFlowCal.Location = new System.Drawing.Point(120, 50);
            this.lblCmdFlowCal.Size     = new System.Drawing.Size(80, 18);

            this.lblMasterOnCaption.Text     = "Master On:";
            this.lblMasterOnCaption.Location = new System.Drawing.Point(10, 76);
            this.lblMasterOnCaption.AutoSize = true;

            this.lblCmdMasterOn.Text      = "—";
            this.lblCmdMasterOn.Location  = new System.Drawing.Point(120, 76);
            this.lblCmdMasterOn.Size      = new System.Drawing.Size(50, 18);
            this.lblCmdMasterOn.ForeColor = System.Drawing.Color.Gray;

            this.lblAutoOnCaption.Text     = "Auto On:";
            this.lblAutoOnCaption.Location = new System.Drawing.Point(210, 76);
            this.lblAutoOnCaption.AutoSize = true;

            this.lblCmdAutoOn.Text      = "—";
            this.lblCmdAutoOn.Location  = new System.Drawing.Point(290, 76);
            this.lblCmdAutoOn.Size      = new System.Drawing.Size(50, 18);
            this.lblCmdAutoOn.ForeColor = System.Drawing.Color.Gray;

            this.lblRelaysCaption.Text     = "Relays (0-15):";
            this.lblRelaysCaption.Location = new System.Drawing.Point(10, 102);
            this.lblRelaysCaption.AutoSize = true;

            this.lblCmdRelays.Text     = "—";
            this.lblCmdRelays.Location = new System.Drawing.Point(120, 102);
            this.lblCmdRelays.Size     = new System.Drawing.Size(170, 18);
            this.lblCmdRelays.Font     = new System.Drawing.Font("Courier New", 9F);

            this.lblConfigCaption.Text     = "Config (32700):";
            this.lblConfigCaption.Location = new System.Drawing.Point(10, 126);
            this.lblConfigCaption.AutoSize = true;

            this.lblCmdConfig.Text      = "—";
            this.lblCmdConfig.Location  = new System.Drawing.Point(120, 126);
            this.lblCmdConfig.Size      = new System.Drawing.Size(120, 18);
            this.lblCmdConfig.ForeColor = System.Drawing.Color.Gray;

            this.grpCommands.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblSetRateCaption,   this.lblCmdSetRate,
                this.lblFlowCalRxCaption, this.lblCmdFlowCal,
                this.lblMasterOnCaption,  this.lblCmdMasterOn,
                this.lblAutoOnCaption,    this.lblCmdAutoOn,
                this.lblRelaysCaption,    this.lblCmdRelays,
                this.lblConfigCaption,    this.lblCmdConfig });

            // ── Bottom controls ───────────────────────────────────────────────────
            this.btnStart.Text     = "Start";
            this.btnStart.Location = new System.Drawing.Point(8, 562);
            this.btnStart.Size     = new System.Drawing.Size(80, 30);
            this.btnStart.Click   += new System.EventHandler(this.btnStart_Click);

            this.btnStop.Text     = "Stop";
            this.btnStop.Location = new System.Drawing.Point(96, 562);
            this.btnStop.Size     = new System.Drawing.Size(80, 30);
            this.btnStop.Enabled  = false;
            this.btnStop.Click   += new System.EventHandler(this.btnStop_Click);

            this.lblStatus.Text      = "Stopped";
            this.lblStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblStatus.Location  = new System.Drawing.Point(190, 569);
            this.lblStatus.Size      = new System.Drawing.Size(280, 18);
            this.lblStatus.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);

            // ── frmMain ──────────────────────────────────────────────────────────
            this.Text            = "Module Simulator";
            this.ClientSize      = new System.Drawing.Size(492, 602);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Font            = new System.Drawing.Font("Segoe UI", 9F);

            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.grpModule,
                this.grpSensor,
                this.grpSim,
                this.grpModStatus,
                this.grpCommands,
                this.btnStart,
                this.btnStop,
                this.lblStatus });
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
