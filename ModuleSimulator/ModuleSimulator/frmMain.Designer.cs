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
            this.tabModules = new System.Windows.Forms.TabControl();
            this.ckEnable = new System.Windows.Forms.CheckBox();
            this.lblSubnet = new System.Windows.Forms.Label();
            this.txtSubnet = new System.Windows.Forms.TextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
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
            // tabModules
            // 
            this.tabModules.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tabModules.Location = new System.Drawing.Point(8, 40);
            this.tabModules.Name = "tabModules";
            this.tabModules.SelectedIndex = 0;
            this.tabModules.Size = new System.Drawing.Size(476, 500);
            this.tabModules.TabIndex = 2;
            // 
            // ckEnable
            // 
            this.ckEnable.AutoSize = true;
            this.ckEnable.Location = new System.Drawing.Point(330, 24);
            this.ckEnable.Name = "ckEnable";
            this.ckEnable.Size = new System.Drawing.Size(104, 24);
            this.ckEnable.TabIndex = 0;
            this.ckEnable.Text = "Enabled";
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
            this.txtSubnet.Size = new System.Drawing.Size(130, 23);
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
            // frmMain
            // 
            this.ClientSize = new System.Drawing.Size(492, 586);
            this.Controls.Add(this.lblSubnet);
            this.Controls.Add(this.txtSubnet);
            this.Controls.Add(this.tabModules);
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
            this.ResumeLayout(false);
            this.PerformLayout();

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
