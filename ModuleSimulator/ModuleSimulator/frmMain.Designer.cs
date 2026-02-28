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
            this.sendTimer  = new System.Windows.Forms.Timer(this.components);
            this.loopTimer  = new System.Windows.Forms.Timer(this.components);
            this.tabModules = new System.Windows.Forms.TabControl();
            this.ckEnable   = new System.Windows.Forms.CheckBox();
            this.lblSubnet  = new System.Windows.Forms.Label();
            this.txtSubnet  = new System.Windows.Forms.TextBox();
            this.btnStart   = new System.Windows.Forms.Button();
            this.btnStop    = new System.Windows.Forms.Button();
            this.lblStatus  = new System.Windows.Forms.Label();

            // Timers
            this.sendTimer.Interval = 200;
            this.sendTimer.Tick    += new System.EventHandler(this.sendTimer_Tick);
            this.loopTimer.Interval = 50;
            this.loopTimer.Tick    += new System.EventHandler(this.loopTimer_Tick);

            // Subnet bar
            this.lblSubnet.Text     = "Subnet:";
            this.lblSubnet.Location = new System.Drawing.Point(8, 12);
            this.lblSubnet.AutoSize = true;

            this.txtSubnet.Location = new System.Drawing.Point(70, 10);
            this.txtSubnet.Size     = new System.Drawing.Size(130, 22);
            this.txtSubnet.Text     = "192.168.1";

            // ckEnable — placed inside Module 2 tab by BuildModuleTabs()
            this.ckEnable.Text     = "Enabled";
            this.ckEnable.Location = new System.Drawing.Point(330, 24);
            this.ckEnable.AutoSize = true;
            this.ckEnable.Checked  = false;

            // TabControl (tabs populated by BuildModuleTabs)
            this.tabModules.Location = new System.Drawing.Point(8, 40);
            this.tabModules.Size     = new System.Drawing.Size(476, 500);
            this.tabModules.Font     = new System.Drawing.Font("Segoe UI", 9F);

            // Bottom controls
            this.btnStart.Text     = "Start";
            this.btnStart.Location = new System.Drawing.Point(8, 548);
            this.btnStart.Size     = new System.Drawing.Size(80, 30);
            this.btnStart.Click   += new System.EventHandler(this.btnStart_Click);

            this.btnStop.Text     = "Stop";
            this.btnStop.Location = new System.Drawing.Point(96, 548);
            this.btnStop.Size     = new System.Drawing.Size(80, 30);
            this.btnStop.Enabled  = false;
            this.btnStop.Click   += new System.EventHandler(this.btnStop_Click);

            this.lblStatus.Text      = "Stopped";
            this.lblStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblStatus.Location  = new System.Drawing.Point(190, 555);
            this.lblStatus.Size      = new System.Drawing.Size(290, 18);
            this.lblStatus.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);

            // Form
            this.Text            = "Module Simulator";
            this.ClientSize      = new System.Drawing.Size(492, 586);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;
            this.StartPosition   = System.Windows.Forms.FormStartPosition.Manual;
            this.Font            = new System.Drawing.Font("Segoe UI", 9F);

            this.Controls.Add(this.lblSubnet);
            this.Controls.Add(this.txtSubnet);
            this.Controls.Add(this.tabModules);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.lblStatus);
        }

        // ── Control declarations ──────────────────────────────────────────────────
        private System.Windows.Forms.Timer        sendTimer;
        private System.Windows.Forms.Timer        loopTimer;
        private System.Windows.Forms.TabControl   tabModules;
        private System.Windows.Forms.CheckBox     ckEnable;
        private System.Windows.Forms.Label        lblSubnet;
        private System.Windows.Forms.TextBox      txtSubnet;
        private System.Windows.Forms.Button       btnStart;
        private System.Windows.Forms.Button       btnStop;
        private System.Windows.Forms.Label        lblStatus;

        // Per-module arrays — initialised by BuildModuleTabs()
        private System.Windows.Forms.NumericUpDown[] nudModuleID;
        private System.Windows.Forms.NumericUpDown[] nudSensorID;
        private System.Windows.Forms.NumericUpDown[] nudMaxHz;
        private System.Windows.Forms.NumericUpDown[] nudNoise;
        private System.Windows.Forms.NumericUpDown[] nudValveLag;
        private System.Windows.Forms.NumericUpDown[] nudWheelSpeed;
        private System.Windows.Forms.NumericUpDown[] nudPressure;
        private System.Windows.Forms.CheckBox[]      ckWorkSwitch;
        private System.Windows.Forms.Button[]        btnResetQty;
        private System.Windows.Forms.Label[]         lblSimRate;
        private System.Windows.Forms.Label[]         lblHz;
        private System.Windows.Forms.Label[]         lblPWM;
        private System.Windows.Forms.Label[]         lblValvePos;
        private System.Windows.Forms.Label[]         lblAccQty;
        private System.Windows.Forms.Label[]         lblCmdSetRate;
        private System.Windows.Forms.Label[]         lblCmdFlowCal;
        private System.Windows.Forms.Label[]         lblCmdMasterOn;
        private System.Windows.Forms.Label[]         lblCmdAutoOn;
        private System.Windows.Forms.Label[]         lblCmdRelays;
        private System.Windows.Forms.Label[]         lblCmdConfig;
    }
}
