namespace RateController
{
    partial class frmWifi
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmWifi));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lbEthernet = new System.Windows.Forms.Label();
            this.btnSetEthernet = new System.Windows.Forms.Button();
            this.cbEthernet = new System.Windows.Forms.ComboBox();
            this.btnRescan = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSendSubnet = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbIP = new System.Windows.Forms.Label();
            this.btnSetIP = new System.Windows.Forms.Button();
            this.cbNetworks = new System.Windows.Forms.ComboBox();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lbEthernet);
            this.groupBox2.Controls.Add(this.btnSetEthernet);
            this.groupBox2.Controls.Add(this.cbEthernet);
            this.groupBox2.Location = new System.Drawing.Point(8, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(375, 108);
            this.groupBox2.TabIndex = 153;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Ethernet";
            this.groupBox2.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox1_Paint);
            // 
            // lbEthernet
            // 
            this.lbEthernet.Location = new System.Drawing.Point(37, 26);
            this.lbEthernet.Name = "lbEthernet";
            this.lbEthernet.Size = new System.Drawing.Size(307, 26);
            this.lbEthernet.TabIndex = 152;
            this.lbEthernet.Text = "192.168.137.1";
            this.lbEthernet.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSetEthernet
            // 
            this.btnSetEthernet.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnSetEthernet.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSetEthernet.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSetEthernet.Location = new System.Drawing.Point(217, 55);
            this.btnSetEthernet.Name = "btnSetEthernet";
            this.btnSetEthernet.Size = new System.Drawing.Size(127, 37);
            this.btnSetEthernet.TabIndex = 153;
            this.btnSetEthernet.Text = "Set";
            this.btnSetEthernet.UseVisualStyleBackColor = false;
            this.btnSetEthernet.Click += new System.EventHandler(this.btnSetEthernet_Click);
            // 
            // cbEthernet
            // 
            this.cbEthernet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEthernet.FormattingEnabled = true;
            this.cbEthernet.Location = new System.Drawing.Point(37, 59);
            this.cbEthernet.Name = "cbEthernet";
            this.cbEthernet.Size = new System.Drawing.Size(157, 31);
            this.cbEthernet.TabIndex = 151;
            // 
            // btnRescan
            // 
            this.btnRescan.BackColor = System.Drawing.Color.Transparent;
            this.btnRescan.FlatAppearance.BorderSize = 0;
            this.btnRescan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRescan.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRescan.Image = global::RateController.Properties.Resources.Update;
            this.btnRescan.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnRescan.Location = new System.Drawing.Point(110, 243);
            this.btnRescan.Name = "btnRescan";
            this.btnRescan.Size = new System.Drawing.Size(72, 72);
            this.btnRescan.TabIndex = 151;
            this.btnRescan.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnRescan.UseVisualStyleBackColor = false;
            this.btnRescan.Click += new System.EventHandler(this.btnRescan_Click);
            this.btnRescan.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.btnRescan_HelpRequested);
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
            this.btnCancel.Location = new System.Drawing.Point(207, 243);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 72);
            this.btnCancel.TabIndex = 143;
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnClose.Image = global::RateController.Properties.Resources.OK;
            this.btnClose.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnClose.Location = new System.Drawing.Point(304, 243);
            this.btnClose.Margin = new System.Windows.Forms.Padding(6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(72, 72);
            this.btnClose.TabIndex = 144;
            this.btnClose.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSendSubnet
            // 
            this.btnSendSubnet.BackColor = System.Drawing.Color.Transparent;
            this.btnSendSubnet.FlatAppearance.BorderSize = 0;
            this.btnSendSubnet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSendSubnet.Image = ((System.Drawing.Image)(resources.GetObject("btnSendSubnet.Image")));
            this.btnSendSubnet.Location = new System.Drawing.Point(13, 243);
            this.btnSendSubnet.Name = "btnSendSubnet";
            this.btnSendSubnet.Size = new System.Drawing.Size(72, 72);
            this.btnSendSubnet.TabIndex = 210;
            this.btnSendSubnet.UseVisualStyleBackColor = false;
            this.btnSendSubnet.Click += new System.EventHandler(this.btnSendSubnet_Click);
            this.btnSendSubnet.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.btnSendSubnet_HelpRequested);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbIP);
            this.groupBox1.Controls.Add(this.btnSetIP);
            this.groupBox1.Controls.Add(this.cbNetworks);
            this.groupBox1.Location = new System.Drawing.Point(8, 126);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(375, 108);
            this.groupBox1.TabIndex = 211;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Wifi";
            this.groupBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox1_Paint_1);
            // 
            // lbIP
            // 
            this.lbIP.Location = new System.Drawing.Point(37, 23);
            this.lbIP.Name = "lbIP";
            this.lbIP.Size = new System.Drawing.Size(307, 26);
            this.lbIP.TabIndex = 147;
            this.lbIP.Text = "192.168.137.1";
            this.lbIP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSetIP
            // 
            this.btnSetIP.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnSetIP.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSetIP.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSetIP.Location = new System.Drawing.Point(217, 52);
            this.btnSetIP.Name = "btnSetIP";
            this.btnSetIP.Size = new System.Drawing.Size(127, 37);
            this.btnSetIP.TabIndex = 149;
            this.btnSetIP.Text = "Set";
            this.btnSetIP.UseVisualStyleBackColor = false;
            this.btnSetIP.Click += new System.EventHandler(this.btnSetIP_Click);
            // 
            // cbNetworks
            // 
            this.cbNetworks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbNetworks.FormattingEnabled = true;
            this.cbNetworks.Location = new System.Drawing.Point(37, 56);
            this.cbNetworks.Name = "cbNetworks";
            this.cbNetworks.Size = new System.Drawing.Size(157, 31);
            this.cbNetworks.TabIndex = 146;
            // 
            // frmWifi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(391, 323);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnSendSubnet);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnRescan);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnClose);
            this.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmWifi";
            this.ShowInTaskbar = false;
            this.Text = "Networks";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmWifi_FormClosed);
            this.Load += new System.EventHandler(this.frmWifi_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnRescan;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lbEthernet;
        private System.Windows.Forms.Button btnSetEthernet;
        private System.Windows.Forms.ComboBox cbEthernet;
        private System.Windows.Forms.Button btnSendSubnet;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbIP;
        private System.Windows.Forms.Button btnSetIP;
        private System.Windows.Forms.ComboBox cbNetworks;
    }
}