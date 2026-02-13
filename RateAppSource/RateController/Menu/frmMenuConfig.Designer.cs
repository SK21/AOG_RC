namespace RateController.Menu
{
    partial class frmMenuConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMenuConfig));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.ckADS1115enabled = new System.Windows.Forms.CheckBox();
            this.lbRelay = new System.Windows.Forms.Label();
            this.cbOnboardRelays = new System.Windows.Forms.ComboBox();
            this.ckFlowOn = new System.Windows.Forms.CheckBox();
            this.ckRelayOn = new System.Windows.Forms.CheckBox();
            this.tbSensorCount = new System.Windows.Forms.TextBox();
            this.lbSensorCount = new System.Windows.Forms.Label();
            this.tbModuleID = new System.Windows.Forms.TextBox();
            this.lbModuleID = new System.Windows.Forms.Label();
            this.btnRescan = new System.Windows.Forms.Button();
            this.lbRemoteRelay = new System.Windows.Forms.Label();
            this.cbRemoteRelays = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbEthernet = new System.Windows.Forms.RadioButton();
            this.rbIsobus = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
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
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(70, 63);
            this.btnCancel.TabIndex = 1;
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
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(70, 63);
            this.btnOK.TabIndex = 0;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // ckADS1115enabled
            // 
            this.ckADS1115enabled.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckADS1115enabled.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckADS1115enabled.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckADS1115enabled.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckADS1115enabled.Location = new System.Drawing.Point(345, 479);
            this.ckADS1115enabled.Name = "ckADS1115enabled";
            this.ckADS1115enabled.Size = new System.Drawing.Size(117, 69);
            this.ckADS1115enabled.TabIndex = 175;
            this.ckADS1115enabled.Text = "ADS1115 ";
            this.ckADS1115enabled.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckADS1115enabled.UseVisualStyleBackColor = true;
            this.ckADS1115enabled.CheckedChanged += new System.EventHandler(this.tbModuleID_TextChanged);
            // 
            // lbRelay
            // 
            this.lbRelay.AutoSize = true;
            this.lbRelay.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRelay.Location = new System.Drawing.Point(59, 346);
            this.lbRelay.Name = "lbRelay";
            this.lbRelay.Size = new System.Drawing.Size(202, 24);
            this.lbRelay.TabIndex = 174;
            this.lbRelay.Text = "Onboard Relay Control";
            // 
            // cbOnboardRelays
            // 
            this.cbOnboardRelays.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOnboardRelays.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbOnboardRelays.FormattingEnabled = true;
            this.cbOnboardRelays.Items.AddRange(new object[] {
            "No Relays",
            "GPIOs",
            "PCA9555  8 relays",
            "PCA9555  16 relays",
            "MCP23017",
            "PCA9685",
            "PCF8574"});
            this.cbOnboardRelays.Location = new System.Drawing.Point(270, 342);
            this.cbOnboardRelays.Name = "cbOnboardRelays";
            this.cbOnboardRelays.Size = new System.Drawing.Size(192, 32);
            this.cbOnboardRelays.TabIndex = 173;
            this.cbOnboardRelays.TabStop = false;
            this.cbOnboardRelays.SelectedIndexChanged += new System.EventHandler(this.tbModuleID_TextChanged);
            // 
            // ckFlowOn
            // 
            this.ckFlowOn.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckFlowOn.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckFlowOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckFlowOn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckFlowOn.Location = new System.Drawing.Point(204, 479);
            this.ckFlowOn.Name = "ckFlowOn";
            this.ckFlowOn.Size = new System.Drawing.Size(117, 69);
            this.ckFlowOn.TabIndex = 170;
            this.ckFlowOn.Text = "Invert Flow Control";
            this.ckFlowOn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckFlowOn.UseVisualStyleBackColor = true;
            this.ckFlowOn.CheckedChanged += new System.EventHandler(this.tbModuleID_TextChanged);
            // 
            // ckRelayOn
            // 
            this.ckRelayOn.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckRelayOn.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckRelayOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckRelayOn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckRelayOn.Location = new System.Drawing.Point(63, 479);
            this.ckRelayOn.Name = "ckRelayOn";
            this.ckRelayOn.Size = new System.Drawing.Size(117, 69);
            this.ckRelayOn.TabIndex = 169;
            this.ckRelayOn.Text = "Invert Relays";
            this.ckRelayOn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckRelayOn.UseVisualStyleBackColor = true;
            this.ckRelayOn.CheckedChanged += new System.EventHandler(this.tbModuleID_TextChanged);
            // 
            // tbSensorCount
            // 
            this.tbSensorCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSensorCount.Location = new System.Drawing.Point(270, 285);
            this.tbSensorCount.Name = "tbSensorCount";
            this.tbSensorCount.Size = new System.Drawing.Size(58, 29);
            this.tbSensorCount.TabIndex = 168;
            this.tbSensorCount.TabStop = false;
            this.tbSensorCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSensorCount.TextChanged += new System.EventHandler(this.tbModuleID_TextChanged);
            this.tbSensorCount.Enter += new System.EventHandler(this.tbSensorCount_Enter);
            // 
            // lbSensorCount
            // 
            this.lbSensorCount.AutoSize = true;
            this.lbSensorCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSensorCount.Location = new System.Drawing.Point(59, 288);
            this.lbSensorCount.Name = "lbSensorCount";
            this.lbSensorCount.Size = new System.Drawing.Size(125, 24);
            this.lbSensorCount.TabIndex = 167;
            this.lbSensorCount.Text = "Sensor Count";
            // 
            // tbModuleID
            // 
            this.tbModuleID.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbModuleID.Location = new System.Drawing.Point(270, 229);
            this.tbModuleID.Name = "tbModuleID";
            this.tbModuleID.Size = new System.Drawing.Size(58, 29);
            this.tbModuleID.TabIndex = 166;
            this.tbModuleID.TabStop = false;
            this.tbModuleID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbModuleID.TextChanged += new System.EventHandler(this.tbModuleID_TextChanged);
            this.tbModuleID.Enter += new System.EventHandler(this.tbModuleID_Enter);
            // 
            // lbModuleID
            // 
            this.lbModuleID.AutoSize = true;
            this.lbModuleID.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbModuleID.Location = new System.Drawing.Point(59, 232);
            this.lbModuleID.Name = "lbModuleID";
            this.lbModuleID.Size = new System.Drawing.Size(96, 24);
            this.lbModuleID.TabIndex = 165;
            this.lbModuleID.Text = "Module ID";
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
            this.btnRescan.Location = new System.Drawing.Point(304, 608);
            this.btnRescan.Name = "btnRescan";
            this.btnRescan.Size = new System.Drawing.Size(70, 63);
            this.btnRescan.TabIndex = 219;
            this.btnRescan.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnRescan.UseVisualStyleBackColor = false;
            this.btnRescan.Click += new System.EventHandler(this.btnRescan_Click);
            // 
            // lbRemoteRelay
            // 
            this.lbRemoteRelay.AutoSize = true;
            this.lbRemoteRelay.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRemoteRelay.Location = new System.Drawing.Point(59, 405);
            this.lbRemoteRelay.Name = "lbRemoteRelay";
            this.lbRemoteRelay.Size = new System.Drawing.Size(193, 24);
            this.lbRemoteRelay.TabIndex = 221;
            this.lbRemoteRelay.Text = "Remote Relay Control";
            // 
            // cbRemoteRelays
            // 
            this.cbRemoteRelays.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRemoteRelays.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbRemoteRelays.FormattingEnabled = true;
            this.cbRemoteRelays.Items.AddRange(new object[] {
            "No Relays",
            "GPIOs",
            "PCA9555  8 relays",
            "PCA9555  16 relays",
            "MCP23017",
            "PCA9685",
            "PCF8574"});
            this.cbRemoteRelays.Location = new System.Drawing.Point(270, 401);
            this.cbRemoteRelays.Name = "cbRemoteRelays";
            this.cbRemoteRelays.Size = new System.Drawing.Size(192, 32);
            this.cbRemoteRelays.TabIndex = 220;
            this.cbRemoteRelays.TabStop = false;
            this.cbRemoteRelays.SelectedIndexChanged += new System.EventHandler(this.tbModuleID_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbIsobus);
            this.groupBox1.Controls.Add(this.rbEthernet);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(101, 42);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(324, 146);
            this.groupBox1.TabIndex = 222;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Communication Mode";
            this.groupBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox1_Paint);
            // 
            // rbEthernet
            // 
            this.rbEthernet.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbEthernet.Checked = true;
            this.rbEthernet.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbEthernet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbEthernet.Location = new System.Drawing.Point(71, 38);
            this.rbEthernet.Name = "rbEthernet";
            this.rbEthernet.Size = new System.Drawing.Size(187, 36);
            this.rbEthernet.TabIndex = 344;
            this.rbEthernet.Text = "Ethernet";
            this.rbEthernet.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbEthernet.UseVisualStyleBackColor = true;
            this.rbEthernet.CheckedChanged += new System.EventHandler(this.tbModuleID_TextChanged);
            // 
            // rbIsobus
            // 
            this.rbIsobus.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbIsobus.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbIsobus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbIsobus.Location = new System.Drawing.Point(71, 92);
            this.rbIsobus.Name = "rbIsobus";
            this.rbIsobus.Size = new System.Drawing.Size(187, 36);
            this.rbIsobus.TabIndex = 345;
            this.rbIsobus.Text = "ISOBUS";
            this.rbIsobus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbIsobus.UseVisualStyleBackColor = true;
            this.rbIsobus.CheckedChanged += new System.EventHandler(this.tbModuleID_TextChanged);
            // 
            // frmMenuConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 678);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lbRemoteRelay);
            this.Controls.Add(this.cbRemoteRelays);
            this.Controls.Add(this.btnRescan);
            this.Controls.Add(this.ckADS1115enabled);
            this.Controls.Add(this.lbRelay);
            this.Controls.Add(this.cbOnboardRelays);
            this.Controls.Add(this.ckFlowOn);
            this.Controls.Add(this.ckRelayOn);
            this.Controls.Add(this.tbSensorCount);
            this.Controls.Add(this.lbSensorCount);
            this.Controls.Add(this.tbModuleID);
            this.Controls.Add(this.lbModuleID);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMenuConfig";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuConfig";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMenuConfig_FormClosed);
            this.Load += new System.EventHandler(this.frmMenuConfig_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox ckADS1115enabled;
        private System.Windows.Forms.Label lbRelay;
        private System.Windows.Forms.ComboBox cbOnboardRelays;
        private System.Windows.Forms.CheckBox ckFlowOn;
        private System.Windows.Forms.CheckBox ckRelayOn;
        private System.Windows.Forms.TextBox tbSensorCount;
        private System.Windows.Forms.Label lbSensorCount;
        private System.Windows.Forms.TextBox tbModuleID;
        private System.Windows.Forms.Label lbModuleID;
        private System.Windows.Forms.Button btnRescan;
        private System.Windows.Forms.Label lbRemoteRelay;
        private System.Windows.Forms.ComboBox cbRemoteRelays;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbIsobus;
        private System.Windows.Forms.RadioButton rbEthernet;
    }
}