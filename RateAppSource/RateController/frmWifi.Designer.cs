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
            this.lb0 = new System.Windows.Forms.Label();
            this.tbSSID = new System.Windows.Forms.TextBox();
            this.lbPassword = new System.Windows.Forms.Label();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.lbHeading = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.grpHotSpot = new System.Windows.Forms.GroupBox();
            this.cbNetworks = new System.Windows.Forms.ComboBox();
            this.lbIP = new System.Windows.Forms.Label();
            this.btnSetIP = new System.Windows.Forms.Button();
            this.btnRescan = new System.Windows.Forms.Button();
            this.grpHotSpot.SuspendLayout();
            this.SuspendLayout();
            // 
            // lb0
            // 
            this.lb0.AutoSize = true;
            this.lb0.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb0.Location = new System.Drawing.Point(6, 30);
            this.lb0.Name = "lb0";
            this.lb0.Size = new System.Drawing.Size(52, 23);
            this.lb0.TabIndex = 124;
            this.lb0.Text = "SSID";
            // 
            // tbSSID
            // 
            this.tbSSID.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSSID.Location = new System.Drawing.Point(121, 27);
            this.tbSSID.MaxLength = 15;
            this.tbSSID.Name = "tbSSID";
            this.tbSSID.Size = new System.Drawing.Size(170, 30);
            this.tbSSID.TabIndex = 123;
            this.tbSSID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSSID.TextChanged += new System.EventHandler(this.tbSSID_TextChanged);
            // 
            // lbPassword
            // 
            this.lbPassword.AutoSize = true;
            this.lbPassword.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPassword.Location = new System.Drawing.Point(6, 69);
            this.lbPassword.Name = "lbPassword";
            this.lbPassword.Size = new System.Drawing.Size(88, 23);
            this.lbPassword.TabIndex = 126;
            this.lbPassword.Text = "Password";
            // 
            // tbPassword
            // 
            this.tbPassword.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPassword.Location = new System.Drawing.Point(121, 66);
            this.tbPassword.MaxLength = 15;
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(170, 30);
            this.tbPassword.TabIndex = 125;
            this.tbPassword.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbPassword.TextChanged += new System.EventHandler(this.tbPassword_TextChanged);
            // 
            // lbHeading
            // 
            this.lbHeading.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbHeading.Location = new System.Drawing.Point(12, 9);
            this.lbHeading.Name = "lbHeading";
            this.lbHeading.Size = new System.Drawing.Size(376, 23);
            this.lbHeading.TabIndex = 127;
            this.lbHeading.Text = "Wireless Network";
            this.lbHeading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnStop
            // 
            this.btnStop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnStop.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStop.Image")));
            this.btnStop.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnStop.Location = new System.Drawing.Point(10, 105);
            this.btnStop.Margin = new System.Windows.Forms.Padding(6);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(115, 72);
            this.btnStop.TabIndex = 140;
            this.btnStop.Text = "Stop";
            this.btnStop.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnStart.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnStart.Image = ((System.Drawing.Image)(resources.GetObject("btnStart.Image")));
            this.btnStart.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnStart.Location = new System.Drawing.Point(176, 105);
            this.btnStart.Margin = new System.Windows.Forms.Padding(6);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(115, 72);
            this.btnStart.TabIndex = 141;
            this.btnStart.Text = "Start";
            this.btnStart.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnCancel.Enabled = false;
            this.btnCancel.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCancel.Location = new System.Drawing.Point(146, 325);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(115, 72);
            this.btnCancel.TabIndex = 143;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnClose.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnClose.Location = new System.Drawing.Point(273, 325);
            this.btnClose.Margin = new System.Windows.Forms.Padding(6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(115, 72);
            this.btnClose.TabIndex = 144;
            this.btnClose.Text = "Close";
            this.btnClose.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // grpHotSpot
            // 
            this.grpHotSpot.Controls.Add(this.btnStart);
            this.grpHotSpot.Controls.Add(this.tbSSID);
            this.grpHotSpot.Controls.Add(this.lb0);
            this.grpHotSpot.Controls.Add(this.btnStop);
            this.grpHotSpot.Controls.Add(this.tbPassword);
            this.grpHotSpot.Controls.Add(this.lbPassword);
            this.grpHotSpot.Location = new System.Drawing.Point(47, 124);
            this.grpHotSpot.Name = "grpHotSpot";
            this.grpHotSpot.Size = new System.Drawing.Size(307, 192);
            this.grpHotSpot.TabIndex = 145;
            this.grpHotSpot.TabStop = false;
            this.grpHotSpot.Text = "Windows Hotspot";
            this.grpHotSpot.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox1_Paint);
            // 
            // cbNetworks
            // 
            this.cbNetworks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbNetworks.FormattingEnabled = true;
            this.cbNetworks.Location = new System.Drawing.Point(47, 85);
            this.cbNetworks.Name = "cbNetworks";
            this.cbNetworks.Size = new System.Drawing.Size(157, 31);
            this.cbNetworks.TabIndex = 146;
            // 
            // lbIP
            // 
            this.lbIP.Location = new System.Drawing.Point(47, 43);
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
            this.btnSetIP.Location = new System.Drawing.Point(227, 81);
            this.btnSetIP.Name = "btnSetIP";
            this.btnSetIP.Size = new System.Drawing.Size(127, 37);
            this.btnSetIP.TabIndex = 149;
            this.btnSetIP.Text = "Set IP";
            this.btnSetIP.UseVisualStyleBackColor = false;
            this.btnSetIP.Click += new System.EventHandler(this.btnSetIP_Click);
            // 
            // btnRescan
            // 
            this.btnRescan.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRescan.Image = ((System.Drawing.Image)(resources.GetObject("btnRescan.Image")));
            this.btnRescan.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnRescan.Location = new System.Drawing.Point(13, 325);
            this.btnRescan.Name = "btnRescan";
            this.btnRescan.Size = new System.Drawing.Size(124, 72);
            this.btnRescan.TabIndex = 151;
            this.btnRescan.Text = "Rescan";
            this.btnRescan.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnRescan.UseVisualStyleBackColor = true;
            this.btnRescan.Click += new System.EventHandler(this.btnRescan_Click);
            // 
            // frmWifi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(402, 408);
            this.Controls.Add(this.btnRescan);
            this.Controls.Add(this.btnSetIP);
            this.Controls.Add(this.lbIP);
            this.Controls.Add(this.cbNetworks);
            this.Controls.Add(this.grpHotSpot);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lbHeading);
            this.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmWifi";
            this.ShowInTaskbar = false;
            this.Text = "Wifi";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmWifi_FormClosed);
            this.Load += new System.EventHandler(this.frmWifi_Load);
            this.grpHotSpot.ResumeLayout(false);
            this.grpHotSpot.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lb0;
        private System.Windows.Forms.TextBox tbSSID;
        private System.Windows.Forms.Label lbPassword;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Label lbHeading;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.GroupBox grpHotSpot;
        private System.Windows.Forms.ComboBox cbNetworks;
        private System.Windows.Forms.Label lbIP;
        private System.Windows.Forms.Button btnSetIP;
        private System.Windows.Forms.Button btnRescan;
    }
}