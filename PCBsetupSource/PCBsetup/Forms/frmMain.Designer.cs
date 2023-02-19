namespace PCBsetup.Forms
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SerialMonitorItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cboPort1 = new System.Windows.Forms.ComboBox();
            this.btnRescan = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnConnect1 = new System.Windows.Forms.Button();
            this.PortIndicator1 = new System.Windows.Forms.Label();
            this.ModuleIndicator = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbModule = new System.Windows.Forms.ComboBox();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnFirmware = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.SerialMonitorItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(438, 32);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.aboutToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(53, 28);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(149, 28);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(149, 28);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(149, 28);
            this.saveAsToolStripMenuItem.Text = "Save As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(146, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(149, 28);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(149, 28);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // SerialMonitorItem
            // 
            this.SerialMonitorItem.Name = "SerialMonitorItem";
            this.SerialMonitorItem.Size = new System.Drawing.Size(137, 28);
            this.SerialMonitorItem.Text = "Serial Monitor";
            this.SerialMonitorItem.Click += new System.EventHandler(this.SerialMonitorItem_Click);
            // 
            // cboPort1
            // 
            this.cboPort1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPort1.DropDownWidth = 145;
            this.cboPort1.FormattingEnabled = true;
            this.cboPort1.Location = new System.Drawing.Point(6, 28);
            this.cboPort1.Name = "cboPort1";
            this.cboPort1.Size = new System.Drawing.Size(398, 32);
            this.cboPort1.TabIndex = 3;
            this.cboPort1.SelectedIndexChanged += new System.EventHandler(this.cboPort1_SelectedIndexChanged);
            this.cboPort1.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.cboPort1_HelpRequested);
            // 
            // btnRescan
            // 
            this.btnRescan.Location = new System.Drawing.Point(117, 69);
            this.btnRescan.Name = "btnRescan";
            this.btnRescan.Size = new System.Drawing.Size(99, 37);
            this.btnRescan.TabIndex = 5;
            this.btnRescan.Text = "Rescan";
            this.btnRescan.UseVisualStyleBackColor = true;
            this.btnRescan.Click += new System.EventHandler(this.btnRescan_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnConnect1
            // 
            this.btnConnect1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnConnect1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnConnect1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConnect1.Location = new System.Drawing.Point(6, 69);
            this.btnConnect1.Name = "btnConnect1";
            this.btnConnect1.Size = new System.Drawing.Size(105, 37);
            this.btnConnect1.TabIndex = 123;
            this.btnConnect1.Text = "Connect";
            this.btnConnect1.UseVisualStyleBackColor = false;
            this.btnConnect1.Click += new System.EventHandler(this.btnConnect1_Click_1);
            // 
            // PortIndicator1
            // 
            this.PortIndicator1.BackColor = System.Drawing.SystemColors.Control;
            this.PortIndicator1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PortIndicator1.Image = global::PCBsetup.Properties.Resources.Off;
            this.PortIndicator1.Location = new System.Drawing.Point(316, 69);
            this.PortIndicator1.Name = "PortIndicator1";
            this.PortIndicator1.Size = new System.Drawing.Size(41, 37);
            this.PortIndicator1.TabIndex = 124;
            this.PortIndicator1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.PortIndicator1.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.PortIndicator1_HelpRequested);
            // 
            // ModuleIndicator
            // 
            this.ModuleIndicator.BackColor = System.Drawing.SystemColors.Control;
            this.ModuleIndicator.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ModuleIndicator.Image = global::PCBsetup.Properties.Resources.Off;
            this.ModuleIndicator.Location = new System.Drawing.Point(363, 69);
            this.ModuleIndicator.Name = "ModuleIndicator";
            this.ModuleIndicator.Size = new System.Drawing.Size(41, 37);
            this.ModuleIndicator.TabIndex = 125;
            this.ModuleIndicator.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ModuleIndicator.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ModuleIndicator_HelpRequested);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cboPort1);
            this.groupBox1.Controls.Add(this.ModuleIndicator);
            this.groupBox1.Controls.Add(this.PortIndicator1);
            this.groupBox1.Controls.Add(this.btnRescan);
            this.groupBox1.Controls.Add(this.btnConnect1);
            this.groupBox1.Location = new System.Drawing.Point(12, 35);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(412, 117);
            this.groupBox1.TabIndex = 126;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Serial Connection";
            this.groupBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.GroupBoxPaint);
            // 
            // cbModule
            // 
            this.cbModule.FormattingEnabled = true;
            this.cbModule.Items.AddRange(new object[] {
            "Teensy AutoSteer",
            "Teensy Rate",
            "Nano Rate",
            "Nano SwitchBox",
            "Wifi AOG"});
            this.cbModule.Location = new System.Drawing.Point(12, 160);
            this.cbModule.Name = "cbModule";
            this.cbModule.Size = new System.Drawing.Size(182, 32);
            this.cbModule.TabIndex = 127;
            this.cbModule.SelectedIndexChanged += new System.EventHandler(this.cbModule_SelectedIndexChanged);
            // 
            // btnSettings
            // 
            this.btnSettings.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSettings.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSettings.Location = new System.Drawing.Point(200, 158);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(105, 37);
            this.btnSettings.TabIndex = 128;
            this.btnSettings.Text = "Settings";
            this.btnSettings.UseVisualStyleBackColor = false;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // btnFirmware
            // 
            this.btnFirmware.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnFirmware.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnFirmware.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFirmware.Location = new System.Drawing.Point(319, 158);
            this.btnFirmware.Name = "btnFirmware";
            this.btnFirmware.Size = new System.Drawing.Size(105, 37);
            this.btnFirmware.TabIndex = 129;
            this.btnFirmware.Text = "Firmware";
            this.btnFirmware.UseVisualStyleBackColor = false;
            this.btnFirmware.Click += new System.EventHandler(this.btnFirmware_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(438, 203);
            this.Controls.Add(this.btnFirmware);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.cbModule);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.Text = "PCBsetup";
            this.Activated += new System.EventHandler(this.frmMain_Activated);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ComboBox cboPort1;
        private System.Windows.Forms.Button btnRescan;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnConnect1;
        private System.Windows.Forms.Label PortIndicator1;
        private System.Windows.Forms.Label ModuleIndicator;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ToolStripMenuItem SerialMonitorItem;
        private System.Windows.Forms.ComboBox cbModule;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnFirmware;
    }
}