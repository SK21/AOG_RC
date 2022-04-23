namespace Configurator
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
            this.teensieToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.firmwareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nanoRateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.firmwareToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.nanoSwitchboxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.firmwareToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.cboPort1 = new System.Windows.Forms.ComboBox();
            this.btnRescan = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.PortIndicator1 = new System.Windows.Forms.Label();
            this.btnConnect1 = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.teensieToolStripMenuItem,
            this.nanoRateToolStripMenuItem,
            this.nanoSwitchboxToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(495, 31);
            this.menuStrip1.TabIndex = 23;
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
            this.fileToolStripMenuItem.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(50, 27);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(145, 28);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(145, 28);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(145, 28);
            this.saveAsToolStripMenuItem.Text = "Save As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(142, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(145, 28);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(145, 28);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // teensieToolStripMenuItem
            // 
            this.teensieToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.firmwareToolStripMenuItem});
            this.teensieToolStripMenuItem.Name = "teensieToolStripMenuItem";
            this.teensieToolStripMenuItem.Size = new System.Drawing.Size(86, 27);
            this.teensieToolStripMenuItem.Text = "Teensie";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(158, 28);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // firmwareToolStripMenuItem
            // 
            this.firmwareToolStripMenuItem.Name = "firmwareToolStripMenuItem";
            this.firmwareToolStripMenuItem.Size = new System.Drawing.Size(158, 28);
            this.firmwareToolStripMenuItem.Text = "Firmware";
            this.firmwareToolStripMenuItem.Click += new System.EventHandler(this.firmwareToolStripMenuItem_Click);
            // 
            // nanoRateToolStripMenuItem
            // 
            this.nanoRateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem1,
            this.firmwareToolStripMenuItem1});
            this.nanoRateToolStripMenuItem.Name = "nanoRateToolStripMenuItem";
            this.nanoRateToolStripMenuItem.Size = new System.Drawing.Size(110, 27);
            this.nanoRateToolStripMenuItem.Text = "Nano Rate";
            // 
            // settingsToolStripMenuItem1
            // 
            this.settingsToolStripMenuItem1.Name = "settingsToolStripMenuItem1";
            this.settingsToolStripMenuItem1.Size = new System.Drawing.Size(158, 28);
            this.settingsToolStripMenuItem1.Text = "Settings";
            this.settingsToolStripMenuItem1.Click += new System.EventHandler(this.settingsToolStripMenuItem1_Click);
            // 
            // firmwareToolStripMenuItem1
            // 
            this.firmwareToolStripMenuItem1.Name = "firmwareToolStripMenuItem1";
            this.firmwareToolStripMenuItem1.Size = new System.Drawing.Size(158, 28);
            this.firmwareToolStripMenuItem1.Text = "Firmware";
            this.firmwareToolStripMenuItem1.Click += new System.EventHandler(this.firmwareToolStripMenuItem1_Click);
            // 
            // nanoSwitchboxToolStripMenuItem
            // 
            this.nanoSwitchboxToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem2,
            this.firmwareToolStripMenuItem2});
            this.nanoSwitchboxToolStripMenuItem.Name = "nanoSwitchboxToolStripMenuItem";
            this.nanoSwitchboxToolStripMenuItem.Size = new System.Drawing.Size(157, 27);
            this.nanoSwitchboxToolStripMenuItem.Text = "Nano Switchbox";
            // 
            // settingsToolStripMenuItem2
            // 
            this.settingsToolStripMenuItem2.Name = "settingsToolStripMenuItem2";
            this.settingsToolStripMenuItem2.Size = new System.Drawing.Size(158, 28);
            this.settingsToolStripMenuItem2.Text = "Settings";
            this.settingsToolStripMenuItem2.Click += new System.EventHandler(this.settingsToolStripMenuItem2_Click);
            // 
            // firmwareToolStripMenuItem2
            // 
            this.firmwareToolStripMenuItem2.Name = "firmwareToolStripMenuItem2";
            this.firmwareToolStripMenuItem2.Size = new System.Drawing.Size(158, 28);
            this.firmwareToolStripMenuItem2.Text = "Firmware";
            this.firmwareToolStripMenuItem2.Click += new System.EventHandler(this.firmwareToolStripMenuItem2_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Configurator.Properties.Resources.AS13;
            this.pictureBox1.Location = new System.Drawing.Point(12, 93);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(469, 435);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 25;
            this.pictureBox1.TabStop = false;
            // 
            // cboPort1
            // 
            this.cboPort1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPort1.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.cboPort1.FormattingEnabled = true;
            this.cboPort1.Location = new System.Drawing.Point(142, 46);
            this.cboPort1.Margin = new System.Windows.Forms.Padding(11);
            this.cboPort1.Name = "cboPort1";
            this.cboPort1.Size = new System.Drawing.Size(145, 37);
            this.cboPort1.TabIndex = 149;
            this.cboPort1.SelectedIndexChanged += new System.EventHandler(this.cboPort1_SelectedIndexChanged);
            // 
            // btnRescan
            // 
            this.btnRescan.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnRescan.Location = new System.Drawing.Point(359, 46);
            this.btnRescan.Name = "btnRescan";
            this.btnRescan.Size = new System.Drawing.Size(122, 37);
            this.btnRescan.TabIndex = 150;
            this.btnRescan.Text = "Rescan";
            this.btnRescan.UseVisualStyleBackColor = true;
            this.btnRescan.Click += new System.EventHandler(this.btnRescan_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "con";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "con";
            // 
            // PortIndicator1
            // 
            this.PortIndicator1.Image = global::Configurator.Properties.Resources.Off;
            this.PortIndicator1.Location = new System.Drawing.Point(301, 46);
            this.PortIndicator1.Name = "PortIndicator1";
            this.PortIndicator1.Size = new System.Drawing.Size(41, 37);
            this.PortIndicator1.TabIndex = 151;
            // 
            // btnConnect1
            // 
            this.btnConnect1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnConnect1.Location = new System.Drawing.Point(12, 46);
            this.btnConnect1.Name = "btnConnect1";
            this.btnConnect1.Size = new System.Drawing.Size(122, 37);
            this.btnConnect1.TabIndex = 152;
            this.btnConnect1.Text = "Connect";
            this.btnConnect1.UseVisualStyleBackColor = true;
            this.btnConnect1.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(495, 540);
            this.Controls.Add(this.btnConnect1);
            this.Controls.Add(this.PortIndicator1);
            this.Controls.Add(this.btnRescan);
            this.Controls.Add(this.cboPort1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.Text = "Configurator";
            this.Activated += new System.EventHandler(this.frmMain_Activated);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem teensieToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem firmwareToolStripMenuItem;
        private ToolStripMenuItem nanoRateToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem1;
        private ToolStripMenuItem firmwareToolStripMenuItem1;
        private ToolStripMenuItem nanoSwitchboxToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem2;
        private ToolStripMenuItem firmwareToolStripMenuItem2;
        private PictureBox pictureBox1;
        private ComboBox cboPort1;
        private Button btnRescan;
        private SaveFileDialog saveFileDialog1;
        private OpenFileDialog openFileDialog1;
        private Label label1;
        private Button btnConnect1;
        private Label PortIndicator1;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem aboutToolStripMenuItem;
    }
}