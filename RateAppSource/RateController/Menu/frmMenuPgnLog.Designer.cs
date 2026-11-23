namespace RateController.Menu
{
    partial class frmMenuPgnLog
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
            this.pnlTop = new System.Windows.Forms.Panel();
            this.cbPGN = new System.Windows.Forms.ComboBox();
            this.lbPGN = new System.Windows.Forms.Label();
            this.cbModule = new System.Windows.Forms.ComboBox();
            this.lbModule = new System.Windows.Forms.Label();
            this.ckAOG = new System.Windows.Forms.CheckBox();
            this.ckTC = new System.Windows.Forms.CheckBox();
            this.ckCAN = new System.Windows.Forms.CheckBox();
            this.ckUDP = new System.Windows.Forms.CheckBox();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.btnFile = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.lstLog = new System.Windows.Forms.ListBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pnlTop.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlTop
            //
            this.pnlTop.Controls.Add(this.cbPGN);
            this.pnlTop.Controls.Add(this.lbPGN);
            this.pnlTop.Controls.Add(this.cbModule);
            this.pnlTop.Controls.Add(this.lbModule);
            this.pnlTop.Controls.Add(this.ckAOG);
            this.pnlTop.Controls.Add(this.ckTC);
            this.pnlTop.Controls.Add(this.ckCAN);
            this.pnlTop.Controls.Add(this.ckUDP);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(760, 56);
            this.pnlTop.TabIndex = 0;
            //
            // cbPGN
            //
            this.cbPGN.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPGN.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.cbPGN.FormattingEnabled = true;
            this.cbPGN.Location = new System.Drawing.Point(600, 12);
            this.cbPGN.Name = "cbPGN";
            this.cbPGN.Size = new System.Drawing.Size(150, 31);
            this.cbPGN.TabIndex = 7;
            this.cbPGN.SelectedIndexChanged += new System.EventHandler(this.Filter_Changed);
            //
            // lbPGN
            //
            this.lbPGN.AutoSize = true;
            this.lbPGN.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.lbPGN.Location = new System.Drawing.Point(546, 16);
            this.lbPGN.Name = "lbPGN";
            this.lbPGN.Size = new System.Drawing.Size(48, 23);
            this.lbPGN.TabIndex = 6;
            this.lbPGN.Text = "PGN";
            //
            // cbModule
            //
            this.cbModule.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbModule.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.cbModule.FormattingEnabled = true;
            this.cbModule.Location = new System.Drawing.Point(440, 12);
            this.cbModule.Name = "cbModule";
            this.cbModule.Size = new System.Drawing.Size(90, 31);
            this.cbModule.TabIndex = 5;
            this.cbModule.SelectedIndexChanged += new System.EventHandler(this.Filter_Changed);
            //
            // lbModule
            //
            this.lbModule.AutoSize = true;
            this.lbModule.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.lbModule.Location = new System.Drawing.Point(358, 16);
            this.lbModule.Name = "lbModule";
            this.lbModule.Size = new System.Drawing.Size(71, 23);
            this.lbModule.TabIndex = 4;
            this.lbModule.Text = "Module";
            //
            // ckAOG
            //
            this.ckAOG.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckAOG.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckAOG.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckAOG.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.ckAOG.Location = new System.Drawing.Point(264, 8);
            this.ckAOG.Name = "ckAOG";
            this.ckAOG.Size = new System.Drawing.Size(80, 40);
            this.ckAOG.TabIndex = 3;
            this.ckAOG.Text = "AOG";
            this.ckAOG.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckAOG.UseVisualStyleBackColor = true;
            this.ckAOG.CheckedChanged += new System.EventHandler(this.ckAOG_CheckedChanged);
            //
            // ckTC
            //
            this.ckTC.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckTC.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckTC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckTC.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.ckTC.Location = new System.Drawing.Point(178, 8);
            this.ckTC.Name = "ckTC";
            this.ckTC.Size = new System.Drawing.Size(80, 40);
            this.ckTC.TabIndex = 2;
            this.ckTC.Text = "TC";
            this.ckTC.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckTC.UseVisualStyleBackColor = true;
            this.ckTC.CheckedChanged += new System.EventHandler(this.Filter_Changed);
            //
            // ckCAN
            //
            this.ckCAN.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckCAN.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckCAN.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckCAN.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.ckCAN.Location = new System.Drawing.Point(92, 8);
            this.ckCAN.Name = "ckCAN";
            this.ckCAN.Size = new System.Drawing.Size(80, 40);
            this.ckCAN.TabIndex = 1;
            this.ckCAN.Text = "CAN";
            this.ckCAN.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckCAN.UseVisualStyleBackColor = true;
            this.ckCAN.CheckedChanged += new System.EventHandler(this.Filter_Changed);
            //
            // ckUDP
            //
            this.ckUDP.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckUDP.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckUDP.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckUDP.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.ckUDP.Location = new System.Drawing.Point(6, 8);
            this.ckUDP.Name = "ckUDP";
            this.ckUDP.Size = new System.Drawing.Size(80, 40);
            this.ckUDP.TabIndex = 0;
            this.ckUDP.Text = "UDP";
            this.ckUDP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckUDP.UseVisualStyleBackColor = true;
            this.ckUDP.CheckedChanged += new System.EventHandler(this.Filter_Changed);
            //
            // pnlBottom
            //
            this.pnlBottom.Controls.Add(this.btnFile);
            this.pnlBottom.Controls.Add(this.btnClear);
            this.pnlBottom.Controls.Add(this.btnPause);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 548);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(760, 72);
            this.pnlBottom.TabIndex = 2;
            //
            // btnFile
            //
            this.btnFile.BackColor = System.Drawing.Color.Transparent;
            this.btnFile.FlatAppearance.BorderSize = 0;
            this.btnFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFile.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnFile.Image = global::RateController.Properties.Resources.file_text;
            this.btnFile.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnFile.Location = new System.Drawing.Point(184, 2);
            this.btnFile.Name = "btnFile";
            this.btnFile.Size = new System.Drawing.Size(94, 68);
            this.btnFile.TabIndex = 2;
            this.btnFile.Text = "File";
            this.btnFile.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnFile.UseVisualStyleBackColor = false;
            this.btnFile.Click += new System.EventHandler(this.btnFile_Click);
            //
            // btnClear
            //
            this.btnClear.BackColor = System.Drawing.Color.Transparent;
            this.btnClear.FlatAppearance.BorderSize = 0;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnClear.Image = global::RateController.Properties.Resources.file_text;
            this.btnClear.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnClear.Location = new System.Drawing.Point(84, 2);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(94, 68);
            this.btnClear.TabIndex = 1;
            this.btnClear.Text = "Clear";
            this.btnClear.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            //
            // btnPause
            //
            this.btnPause.BackColor = System.Drawing.Color.Transparent;
            this.btnPause.FlatAppearance.BorderSize = 0;
            this.btnPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPause.Image = global::RateController.Properties.Resources.Pause;
            this.btnPause.Location = new System.Drawing.Point(6, 4);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(70, 63);
            this.btnPause.TabIndex = 0;
            this.btnPause.UseVisualStyleBackColor = false;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            //
            // lstLog
            //
            this.lstLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstLog.Font = new System.Drawing.Font("Consolas", 12F);
            this.lstLog.FormattingEnabled = true;
            this.lstLog.HorizontalScrollbar = true;
            this.lstLog.IntegralHeight = false;
            this.lstLog.ItemHeight = 19;
            this.lstLog.Location = new System.Drawing.Point(0, 56);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(760, 492);
            this.lstLog.TabIndex = 1;
            //
            // timer1
            //
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            //
            // frmMenuPgnLog
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 620);
            this.Controls.Add(this.lstLog);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlTop);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(776, 300);
            this.Name = "frmMenuPgnLog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "PGN Log";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMenuPgnLog_FormClosed);
            this.Load += new System.EventHandler(this.frmMenuPgnLog_Load);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.ComboBox cbPGN;
        private System.Windows.Forms.Label lbPGN;
        private System.Windows.Forms.ComboBox cbModule;
        private System.Windows.Forms.Label lbModule;
        private System.Windows.Forms.CheckBox ckAOG;
        private System.Windows.Forms.CheckBox ckTC;
        private System.Windows.Forms.CheckBox ckCAN;
        private System.Windows.Forms.CheckBox ckUDP;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Button btnFile;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.Timer timer1;
    }
}
