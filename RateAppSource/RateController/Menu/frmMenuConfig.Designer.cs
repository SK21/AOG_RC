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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.ckADS1115enabled = new System.Windows.Forms.CheckBox();
            this.lbRelay = new System.Windows.Forms.Label();
            this.cbRelayControl = new System.Windows.Forms.ComboBox();
            this.tbWifiPort = new System.Windows.Forms.TextBox();
            this.lbWifiPort = new System.Windows.Forms.Label();
            this.ckFlowOn = new System.Windows.Forms.CheckBox();
            this.ckRelayOn = new System.Windows.Forms.CheckBox();
            this.tbSensorCount = new System.Windows.Forms.TextBox();
            this.lbSensorCount = new System.Windows.Forms.Label();
            this.tbModuleID = new System.Windows.Forms.TextBox();
            this.lbModuleID = new System.Windows.Forms.Label();
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
            this.btnCancel.Location = new System.Drawing.Point(378, 546);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 72);
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
            this.btnOK.Location = new System.Drawing.Point(456, 546);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(72, 72);
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
            this.ckADS1115enabled.Location = new System.Drawing.Point(345, 334);
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
            this.lbRelay.Location = new System.Drawing.Point(84, 265);
            this.lbRelay.Name = "lbRelay";
            this.lbRelay.Size = new System.Drawing.Size(122, 24);
            this.lbRelay.TabIndex = 174;
            this.lbRelay.Text = "Relay Control";
            // 
            // cbRelayControl
            // 
            this.cbRelayControl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRelayControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbRelayControl.FormattingEnabled = true;
            this.cbRelayControl.Items.AddRange(new object[] {
            "No Relays",
            "GPIOs",
            "PCA9555  8 relays",
            "PCA9555  16 relays",
            "MCP23017",
            "PCA9685",
            "PCF8574"});
            this.cbRelayControl.Location = new System.Drawing.Point(233, 261);
            this.cbRelayControl.Name = "cbRelayControl";
            this.cbRelayControl.Size = new System.Drawing.Size(187, 32);
            this.cbRelayControl.TabIndex = 173;
            this.cbRelayControl.TabStop = false;
            this.cbRelayControl.SelectedIndexChanged += new System.EventHandler(this.tbModuleID_TextChanged);
            // 
            // tbWifiPort
            // 
            this.tbWifiPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbWifiPort.Location = new System.Drawing.Point(362, 213);
            this.tbWifiPort.Name = "tbWifiPort";
            this.tbWifiPort.Size = new System.Drawing.Size(58, 29);
            this.tbWifiPort.TabIndex = 172;
            this.tbWifiPort.TabStop = false;
            this.tbWifiPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbWifiPort.TextChanged += new System.EventHandler(this.tbModuleID_TextChanged);
            this.tbWifiPort.Enter += new System.EventHandler(this.tbWifiPort_Enter);
            // 
            // lbWifiPort
            // 
            this.lbWifiPort.AutoSize = true;
            this.lbWifiPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbWifiPort.Location = new System.Drawing.Point(84, 215);
            this.lbWifiPort.Name = "lbWifiPort";
            this.lbWifiPort.Size = new System.Drawing.Size(135, 24);
            this.lbWifiPort.TabIndex = 171;
            this.lbWifiPort.Text = "Wifi  Serial Port";
            // 
            // ckFlowOn
            // 
            this.ckFlowOn.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckFlowOn.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckFlowOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckFlowOn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckFlowOn.Location = new System.Drawing.Point(204, 334);
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
            this.ckRelayOn.Location = new System.Drawing.Point(63, 334);
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
            this.tbSensorCount.Location = new System.Drawing.Point(362, 165);
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
            this.lbSensorCount.Location = new System.Drawing.Point(84, 167);
            this.lbSensorCount.Name = "lbSensorCount";
            this.lbSensorCount.Size = new System.Drawing.Size(125, 24);
            this.lbSensorCount.TabIndex = 167;
            this.lbSensorCount.Text = "Sensor Count";
            // 
            // tbModuleID
            // 
            this.tbModuleID.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbModuleID.Location = new System.Drawing.Point(362, 117);
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
            this.lbModuleID.Location = new System.Drawing.Point(84, 119);
            this.lbModuleID.Name = "lbModuleID";
            this.lbModuleID.Size = new System.Drawing.Size(96, 24);
            this.lbModuleID.TabIndex = 165;
            this.lbModuleID.Text = "Module ID";
            // 
            // frmMenuConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 630);
            this.Controls.Add(this.ckADS1115enabled);
            this.Controls.Add(this.lbRelay);
            this.Controls.Add(this.cbRelayControl);
            this.Controls.Add(this.tbWifiPort);
            this.Controls.Add(this.lbWifiPort);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox ckADS1115enabled;
        private System.Windows.Forms.Label lbRelay;
        private System.Windows.Forms.ComboBox cbRelayControl;
        private System.Windows.Forms.TextBox tbWifiPort;
        private System.Windows.Forms.Label lbWifiPort;
        private System.Windows.Forms.CheckBox ckFlowOn;
        private System.Windows.Forms.CheckBox ckRelayOn;
        private System.Windows.Forms.TextBox tbSensorCount;
        private System.Windows.Forms.Label lbSensorCount;
        private System.Windows.Forms.TextBox tbModuleID;
        private System.Windows.Forms.Label lbModuleID;
    }
}