namespace RateController.Menu
{
    partial class frmMenuComm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMenuComm));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.gbxDrivers = new System.Windows.Forms.GroupBox();
            this.rbAdapter1 = new System.Windows.Forms.RadioButton();
            this.rbAdapter3 = new System.Windows.Forms.RadioButton();
            this.rbAdapter2 = new System.Windows.Forms.RadioButton();
            this.gbxPort = new System.Windows.Forms.GroupBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.cbComPort = new System.Windows.Forms.ComboBox();
            this.lbDriverFound = new System.Windows.Forms.Label();
            this.lbConnected = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.cbEthernet = new System.Windows.Forms.ComboBox();
            this.lbSubnet = new System.Windows.Forms.Label();
            this.btnSendSubnet = new System.Windows.Forms.Button();
            this.btnRescan = new System.Windows.Forms.Button();
            this.lbIP = new System.Windows.Forms.Label();
            this.lbModuleIP = new System.Windows.Forms.Label();
            this.ckEthernet = new System.Windows.Forms.CheckBox();
            this.ckDiagnostics = new System.Windows.Forms.CheckBox();
            this.ckCanBus = new System.Windows.Forms.CheckBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.gbxDrivers.SuspendLayout();
            this.gbxPort.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
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
            this.btnCancel.Location = new System.Drawing.Point(380, 611);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(70, 63);
            this.btnCancel.TabIndex = 164;
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
            this.btnOK.Location = new System.Drawing.Point(458, 611);
            this.btnOK.Margin = new System.Windows.Forms.Padding(6);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(70, 63);
            this.btnOK.TabIndex = 163;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // gbxDrivers
            // 
            this.gbxDrivers.Controls.Add(this.rbAdapter1);
            this.gbxDrivers.Controls.Add(this.rbAdapter3);
            this.gbxDrivers.Controls.Add(this.rbAdapter2);
            this.gbxDrivers.Location = new System.Drawing.Point(44, 164);
            this.gbxDrivers.Name = "gbxDrivers";
            this.gbxDrivers.Size = new System.Drawing.Size(250, 223);
            this.gbxDrivers.TabIndex = 348;
            this.gbxDrivers.TabStop = false;
            this.gbxDrivers.Text = "Driver";
            this.gbxDrivers.Paint += new System.Windows.Forms.PaintEventHandler(this.gbEthernet_Paint);
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
            this.rbAdapter1.CheckedChanged += new System.EventHandler(this.rbAdapter1_CheckedChanged);
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
            this.rbAdapter3.CheckedChanged += new System.EventHandler(this.rbAdapter1_CheckedChanged);
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
            this.rbAdapter2.CheckedChanged += new System.EventHandler(this.rbAdapter1_CheckedChanged);
            // 
            // gbxPort
            // 
            this.gbxPort.Controls.Add(this.btnRefresh);
            this.gbxPort.Controls.Add(this.cbComPort);
            this.gbxPort.Location = new System.Drawing.Point(337, 164);
            this.gbxPort.Name = "gbxPort";
            this.gbxPort.Size = new System.Drawing.Size(119, 149);
            this.gbxPort.TabIndex = 358;
            this.gbxPort.TabStop = false;
            this.gbxPort.Text = "Port";
            this.gbxPort.Paint += new System.Windows.Forms.PaintEventHandler(this.gbEthernet_Paint);
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
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
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
            this.cbComPort.SelectedIndexChanged += new System.EventHandler(this.cbComPort_SelectedIndexChanged);
            // 
            // lbDriverFound
            // 
            this.lbDriverFound.BackColor = System.Drawing.SystemColors.Control;
            this.lbDriverFound.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDriverFound.Image = global::RateController.Properties.Resources.Off;
            this.lbDriverFound.Location = new System.Drawing.Point(415, 320);
            this.lbDriverFound.Name = "lbDriverFound";
            this.lbDriverFound.Size = new System.Drawing.Size(41, 37);
            this.lbDriverFound.TabIndex = 362;
            this.lbDriverFound.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbConnected
            // 
            this.lbConnected.BackColor = System.Drawing.SystemColors.Control;
            this.lbConnected.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbConnected.Image = global::RateController.Properties.Resources.Off;
            this.lbConnected.Location = new System.Drawing.Point(415, 357);
            this.lbConnected.Name = "lbConnected";
            this.lbConnected.Size = new System.Drawing.Size(41, 37);
            this.lbConnected.TabIndex = 360;
            this.lbConnected.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(337, 327);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 23);
            this.label2.TabIndex = 361;
            this.label2.Text = "Driver";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(337, 364);
            this.label13.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(71, 23);
            this.label13.TabIndex = 359;
            this.label13.Text = "Module";
            // 
            // cbEthernet
            // 
            this.cbEthernet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEthernet.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbEthernet.FormattingEnabled = true;
            this.cbEthernet.Location = new System.Drawing.Point(252, 231);
            this.cbEthernet.Name = "cbEthernet";
            this.cbEthernet.Size = new System.Drawing.Size(157, 32);
            this.cbEthernet.TabIndex = 349;
            this.cbEthernet.SelectedIndexChanged += new System.EventHandler(this.cbComPort_SelectedIndexChanged);
            // 
            // lbSubnet
            // 
            this.lbSubnet.AutoSize = true;
            this.lbSubnet.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSubnet.Location = new System.Drawing.Point(83, 191);
            this.lbSubnet.Name = "lbSubnet";
            this.lbSubnet.Size = new System.Drawing.Size(149, 24);
            this.lbSubnet.TabIndex = 350;
            this.lbSubnet.Text = "Selected Subnet";
            // 
            // btnSendSubnet
            // 
            this.btnSendSubnet.BackColor = System.Drawing.Color.Transparent;
            this.btnSendSubnet.FlatAppearance.BorderSize = 0;
            this.btnSendSubnet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSendSubnet.Image = global::RateController.Properties.Resources.Update4;
            this.btnSendSubnet.Location = new System.Drawing.Point(276, 287);
            this.btnSendSubnet.Name = "btnSendSubnet";
            this.btnSendSubnet.Size = new System.Drawing.Size(72, 72);
            this.btnSendSubnet.TabIndex = 348;
            this.btnSendSubnet.UseVisualStyleBackColor = false;
            this.btnSendSubnet.Click += new System.EventHandler(this.btnSendSubnet_Click);
            // 
            // btnRescan
            // 
            this.btnRescan.BackColor = System.Drawing.Color.Transparent;
            this.btnRescan.FlatAppearance.BorderSize = 0;
            this.btnRescan.FlatAppearance.MouseDownBackColor = System.Drawing.Color.LightGreen;
            this.btnRescan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRescan.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRescan.Image = ((System.Drawing.Image)(resources.GetObject("btnRescan.Image")));
            this.btnRescan.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnRescan.Location = new System.Drawing.Point(154, 287);
            this.btnRescan.Name = "btnRescan";
            this.btnRescan.Size = new System.Drawing.Size(72, 72);
            this.btnRescan.TabIndex = 347;
            this.btnRescan.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnRescan.UseVisualStyleBackColor = false;
            this.btnRescan.Click += new System.EventHandler(this.btnRescan_Click);
            // 
            // lbIP
            // 
            this.lbIP.AutoSize = true;
            this.lbIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbIP.Location = new System.Drawing.Point(83, 234);
            this.lbIP.Name = "lbIP";
            this.lbIP.Size = new System.Drawing.Size(76, 24);
            this.lbIP.TabIndex = 352;
            this.lbIP.Text = "Local IP";
            // 
            // lbModuleIP
            // 
            this.lbModuleIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbModuleIP.Location = new System.Drawing.Point(248, 191);
            this.lbModuleIP.Name = "lbModuleIP";
            this.lbModuleIP.Size = new System.Drawing.Size(161, 24);
            this.lbModuleIP.TabIndex = 351;
            this.lbModuleIP.Text = "192.168.100.100";
            this.lbModuleIP.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ckEthernet
            // 
            this.ckEthernet.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckEthernet.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckEthernet.Checked = true;
            this.ckEthernet.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckEthernet.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckEthernet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckEthernet.Location = new System.Drawing.Point(159, 129);
            this.ckEthernet.Margin = new System.Windows.Forms.Padding(6);
            this.ckEthernet.Name = "ckEthernet";
            this.ckEthernet.Size = new System.Drawing.Size(192, 36);
            this.ckEthernet.TabIndex = 346;
            this.ckEthernet.Text = "Enabled";
            this.ckEthernet.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckEthernet.UseVisualStyleBackColor = true;
            this.ckEthernet.CheckedChanged += new System.EventHandler(this.ckEthernet_CheckedChanged);
            // 
            // ckDiagnostics
            // 
            this.ckDiagnostics.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckDiagnostics.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckDiagnostics.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckDiagnostics.Location = new System.Drawing.Point(264, 119);
            this.ckDiagnostics.Margin = new System.Windows.Forms.Padding(6);
            this.ckDiagnostics.Name = "ckDiagnostics";
            this.ckDiagnostics.Size = new System.Drawing.Size(192, 36);
            this.ckDiagnostics.TabIndex = 363;
            this.ckDiagnostics.Text = "Diagnostics";
            this.ckDiagnostics.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckDiagnostics.UseVisualStyleBackColor = true;
            this.ckDiagnostics.CheckedChanged += new System.EventHandler(this.cbComPort_SelectedIndexChanged);
            // 
            // ckCanBus
            // 
            this.ckCanBus.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckCanBus.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckCanBus.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckCanBus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckCanBus.Location = new System.Drawing.Point(44, 119);
            this.ckCanBus.Margin = new System.Windows.Forms.Padding(6);
            this.ckCanBus.Name = "ckCanBus";
            this.ckCanBus.Size = new System.Drawing.Size(192, 36);
            this.ckCanBus.TabIndex = 347;
            this.ckCanBus.Text = "Enabled";
            this.ckCanBus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckCanBus.UseVisualStyleBackColor = true;
            this.ckCanBus.CheckedChanged += new System.EventHandler(this.ckCanBus_CheckedChanged);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 29);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(516, 561);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.tabControl1.TabIndex = 366;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.cbEthernet);
            this.tabPage1.Controls.Add(this.ckEthernet);
            this.tabPage1.Controls.Add(this.lbSubnet);
            this.tabPage1.Controls.Add(this.lbModuleIP);
            this.tabPage1.Controls.Add(this.btnSendSubnet);
            this.tabPage1.Controls.Add(this.lbIP);
            this.tabPage1.Controls.Add(this.btnRescan);
            this.tabPage1.Location = new System.Drawing.Point(4, 33);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(508, 524);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Ethernet";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.ckDiagnostics);
            this.tabPage2.Controls.Add(this.ckCanBus);
            this.tabPage2.Controls.Add(this.lbConnected);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.lbDriverFound);
            this.tabPage2.Controls.Add(this.gbxDrivers);
            this.tabPage2.Controls.Add(this.label13);
            this.tabPage2.Controls.Add(this.gbxPort);
            this.tabPage2.Location = new System.Drawing.Point(4, 33);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(508, 524);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "CanBus";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // frmMenuComm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 678);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "frmMenuComm";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuComm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMenuComm_FormClosing);
            this.Load += new System.EventHandler(this.frmMenuComm_Load);
            this.gbxDrivers.ResumeLayout(false);
            this.gbxPort.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox gbxDrivers;
        private System.Windows.Forms.RadioButton rbAdapter1;
        private System.Windows.Forms.RadioButton rbAdapter3;
        private System.Windows.Forms.RadioButton rbAdapter2;
        private System.Windows.Forms.GroupBox gbxPort;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.ComboBox cbComPort;
        private System.Windows.Forms.Label lbDriverFound;
        private System.Windows.Forms.Label lbConnected;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox ckEthernet;
        private System.Windows.Forms.ComboBox cbEthernet;
        private System.Windows.Forms.Label lbSubnet;
        private System.Windows.Forms.Button btnSendSubnet;
        private System.Windows.Forms.Button btnRescan;
        private System.Windows.Forms.Label lbIP;
        private System.Windows.Forms.Label lbModuleIP;
        private System.Windows.Forms.CheckBox ckCanBus;
        private System.Windows.Forms.CheckBox ckDiagnostics;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
    }
}