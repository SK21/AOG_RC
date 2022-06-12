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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.teensieToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.firmwareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nanoRateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.firmwareToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.nanoSwitchboxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.firmwareToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.cboPort1 = new System.Windows.Forms.ComboBox();
            this.btnRescan = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnConnect1 = new System.Windows.Forms.Button();
            this.PortIndicator1 = new System.Windows.Forms.Label();
            this.ModuleIndicator = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::PCBsetup.Properties.Resources.AS13;
            this.pictureBox1.Location = new System.Drawing.Point(12, 117);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(469, 411);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.teensieToolStripMenuItem,
            this.nanoRateToolStripMenuItem,
            this.nanoSwitchboxToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(495, 32);
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
            // teensieToolStripMenuItem
            // 
            this.teensieToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.firmwareToolStripMenuItem});
            this.teensieToolStripMenuItem.Name = "teensieToolStripMenuItem";
            this.teensieToolStripMenuItem.Size = new System.Drawing.Size(91, 28);
            this.teensieToolStripMenuItem.Text = "Teensie";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(159, 28);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // firmwareToolStripMenuItem
            // 
            this.firmwareToolStripMenuItem.Name = "firmwareToolStripMenuItem";
            this.firmwareToolStripMenuItem.Size = new System.Drawing.Size(159, 28);
            this.firmwareToolStripMenuItem.Text = "Firmware";
            this.firmwareToolStripMenuItem.Click += new System.EventHandler(this.firmwareToolStripMenuItem_Click);
            // 
            // nanoRateToolStripMenuItem
            // 
            this.nanoRateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem1,
            this.firmwareToolStripMenuItem1});
            this.nanoRateToolStripMenuItem.Name = "nanoRateToolStripMenuItem";
            this.nanoRateToolStripMenuItem.Size = new System.Drawing.Size(111, 28);
            this.nanoRateToolStripMenuItem.Text = "Nano Rate";
            // 
            // settingsToolStripMenuItem1
            // 
            this.settingsToolStripMenuItem1.Name = "settingsToolStripMenuItem1";
            this.settingsToolStripMenuItem1.Size = new System.Drawing.Size(159, 28);
            this.settingsToolStripMenuItem1.Text = "Settings";
            this.settingsToolStripMenuItem1.Click += new System.EventHandler(this.settingsToolStripMenuItem1_Click);
            // 
            // firmwareToolStripMenuItem1
            // 
            this.firmwareToolStripMenuItem1.Name = "firmwareToolStripMenuItem1";
            this.firmwareToolStripMenuItem1.Size = new System.Drawing.Size(159, 28);
            this.firmwareToolStripMenuItem1.Text = "Firmware";
            this.firmwareToolStripMenuItem1.Click += new System.EventHandler(this.firmwareToolStripMenuItem1_Click);
            // 
            // nanoSwitchboxToolStripMenuItem
            // 
            this.nanoSwitchboxToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem2,
            this.firmwareToolStripMenuItem2});
            this.nanoSwitchboxToolStripMenuItem.Name = "nanoSwitchboxToolStripMenuItem";
            this.nanoSwitchboxToolStripMenuItem.Size = new System.Drawing.Size(160, 28);
            this.nanoSwitchboxToolStripMenuItem.Text = "Nano Switchbox";
            // 
            // settingsToolStripMenuItem2
            // 
            this.settingsToolStripMenuItem2.Name = "settingsToolStripMenuItem2";
            this.settingsToolStripMenuItem2.Size = new System.Drawing.Size(159, 28);
            this.settingsToolStripMenuItem2.Text = "Settings";
            this.settingsToolStripMenuItem2.Click += new System.EventHandler(this.settingsToolStripMenuItem2_Click);
            // 
            // firmwareToolStripMenuItem2
            // 
            this.firmwareToolStripMenuItem2.Name = "firmwareToolStripMenuItem2";
            this.firmwareToolStripMenuItem2.Size = new System.Drawing.Size(159, 28);
            this.firmwareToolStripMenuItem2.Text = "Firmware";
            this.firmwareToolStripMenuItem2.Click += new System.EventHandler(this.firmwareToolStripMenuItem2_Click);
            // 
            // cboPort1
            // 
            this.cboPort1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPort1.DropDownWidth = 145;
            this.cboPort1.FormattingEnabled = true;
            this.cboPort1.Location = new System.Drawing.Point(241, 28);
            this.cboPort1.Name = "cboPort1";
            this.cboPort1.Size = new System.Drawing.Size(114, 32);
            this.cboPort1.TabIndex = 3;
            this.cboPort1.SelectedIndexChanged += new System.EventHandler(this.cboPort1_SelectedIndexChanged);
            // 
            // btnRescan
            // 
            this.btnRescan.Location = new System.Drawing.Point(126, 26);
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
            this.btnConnect1.Location = new System.Drawing.Point(5, 26);
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
            this.PortIndicator1.Location = new System.Drawing.Point(375, 24);
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
            this.ModuleIndicator.Location = new System.Drawing.Point(422, 24);
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
            this.groupBox1.Size = new System.Drawing.Size(469, 76);
            this.groupBox1.TabIndex = 126;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Serial Connection";
            this.groupBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.GroupBoxPaint);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(495, 540);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBox1);
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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem teensieToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem firmwareToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nanoRateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem firmwareToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem nanoSwitchboxToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem firmwareToolStripMenuItem2;
        private System.Windows.Forms.ComboBox cboPort1;
        private System.Windows.Forms.Button btnRescan;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnConnect1;
        private System.Windows.Forms.Label PortIndicator1;
        private System.Windows.Forms.Label ModuleIndicator;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}