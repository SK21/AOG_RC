namespace RateController.Forms
{
    partial class frmFirmwareUpdate
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFirmwareUpdate));
            this.lbBoardName = new System.Windows.Forms.Label();
            this.lbBoard = new System.Windows.Forms.Label();
            this.lbModuleName = new System.Windows.Forms.Label();
            this.lbModule = new System.Windows.Forms.Label();
            this.lbLinkName = new System.Windows.Forms.Label();
            this.lbLink = new System.Windows.Forms.Label();
            this.lbCurrentName = new System.Windows.Forms.Label();
            this.lbCurrent = new System.Windows.Forms.Label();
            this.lbFileName = new System.Windows.Forms.Label();
            this.lbFile = new System.Windows.Forms.Label();
            this.lbHexFile = new System.Windows.Forms.Label();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.lbStatus = new System.Windows.Forms.Label();
            this.btnCheck = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.ofdHex = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // lbBoardName
            // 
            this.lbBoardName.AutoSize = true;
            this.lbBoardName.Location = new System.Drawing.Point(12, 15);
            this.lbBoardName.Name = "lbBoardName";
            this.lbBoardName.Size = new System.Drawing.Size(60, 24);
            this.lbBoardName.TabIndex = 0;
            this.lbBoardName.Text = "Board";
            // 
            // lbBoard
            // 
            this.lbBoard.AutoEllipsis = true;
            this.lbBoard.Location = new System.Drawing.Point(180, 15);
            this.lbBoard.Name = "lbBoard";
            this.lbBoard.Size = new System.Drawing.Size(320, 24);
            this.lbBoard.TabIndex = 1;
            this.lbBoard.Text = "Teensy Rate 12:34:56";
            // 
            // lbModuleName
            // 
            this.lbModuleName.AutoSize = true;
            this.lbModuleName.Location = new System.Drawing.Point(12, 50);
            this.lbModuleName.Name = "lbModuleName";
            this.lbModuleName.Size = new System.Drawing.Size(74, 24);
            this.lbModuleName.TabIndex = 2;
            this.lbModuleName.Text = "Module";
            // 
            // lbModule
            // 
            this.lbModule.AutoEllipsis = true;
            this.lbModule.Location = new System.Drawing.Point(180, 50);
            this.lbModule.Name = "lbModule";
            this.lbModule.Size = new System.Drawing.Size(320, 24);
            this.lbModule.TabIndex = 3;
            this.lbModule.Text = "0";
            // 
            // lbLinkName
            // 
            this.lbLinkName.AutoSize = true;
            this.lbLinkName.Location = new System.Drawing.Point(12, 85);
            this.lbLinkName.Name = "lbLinkName";
            this.lbLinkName.Size = new System.Drawing.Size(107, 24);
            this.lbLinkName.TabIndex = 4;
            this.lbLinkName.Text = "Connection";
            // 
            // lbLink
            // 
            this.lbLink.AutoEllipsis = true;
            this.lbLink.Location = new System.Drawing.Point(180, 85);
            this.lbLink.Name = "lbLink";
            this.lbLink.Size = new System.Drawing.Size(320, 24);
            this.lbLink.TabIndex = 5;
            this.lbLink.Text = "Ethernet";
            // 
            // lbCurrentName
            // 
            this.lbCurrentName.AutoSize = true;
            this.lbCurrentName.Location = new System.Drawing.Point(12, 120);
            this.lbCurrentName.Name = "lbCurrentName";
            this.lbCurrentName.Size = new System.Drawing.Size(136, 24);
            this.lbCurrentName.TabIndex = 6;
            this.lbCurrentName.Text = "Board firmware";
            // 
            // lbCurrent
            // 
            this.lbCurrent.AutoEllipsis = true;
            this.lbCurrent.Location = new System.Drawing.Point(180, 120);
            this.lbCurrent.Name = "lbCurrent";
            this.lbCurrent.Size = new System.Drawing.Size(320, 24);
            this.lbCurrent.TabIndex = 7;
            this.lbCurrent.Text = "14-Nov-2026";
            // 
            // lbFileName
            // 
            this.lbFileName.AutoSize = true;
            this.lbFileName.Location = new System.Drawing.Point(12, 155);
            this.lbFileName.Name = "lbFileName";
            this.lbFileName.Size = new System.Drawing.Size(117, 24);
            this.lbFileName.TabIndex = 8;
            this.lbFileName.Text = "Firmware file";
            // 
            // lbFile
            // 
            this.lbFile.AutoEllipsis = true;
            this.lbFile.Location = new System.Drawing.Point(180, 155);
            this.lbFile.Name = "lbFile";
            this.lbFile.Size = new System.Drawing.Size(320, 24);
            this.lbFile.TabIndex = 9;
            this.lbFile.Text = "v2026.11.14";
            //
            // lbHexFile
            //
            this.lbHexFile.AutoEllipsis = true;
            this.lbHexFile.Location = new System.Drawing.Point(180, 185);
            this.lbHexFile.Name = "lbHexFile";
            this.lbHexFile.Size = new System.Drawing.Size(320, 24);
            this.lbHexFile.TabIndex = 17;
            this.lbHexFile.Text = "RCteensy.ino.hex";
            //
            // pbProgress
            //
            this.pbProgress.Location = new System.Drawing.Point(16, 226);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(484, 24);
            this.pbProgress.TabIndex = 10;
            // 
            // lbStatus
            // 
            this.lbStatus.Location = new System.Drawing.Point(12, 260);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(488, 84);
            this.lbStatus.TabIndex = 11;
            this.lbStatus.Text = "Status";
            // 
            // btnCheck
            // 
            this.btnCheck.BackColor = System.Drawing.Color.Transparent;
            this.btnCheck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnCheck.FlatAppearance.BorderSize = 0;
            this.btnCheck.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCheck.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnCheck.Image = global::RateController.Properties.Resources.Update;
            this.btnCheck.Location = new System.Drawing.Point(12, 352);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(72, 72);
            this.btnCheck.TabIndex = 12;
            this.btnCheck.UseVisualStyleBackColor = false;
            this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.BackColor = System.Drawing.Color.Transparent;
            this.btnBrowse.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnBrowse.FlatAppearance.BorderSize = 0;
            this.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowse.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnBrowse.Image = ((System.Drawing.Image)(resources.GetObject("btnBrowse.Image")));
            this.btnBrowse.Location = new System.Drawing.Point(92, 352);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(72, 72);
            this.btnBrowse.TabIndex = 13;
            this.btnBrowse.UseVisualStyleBackColor = false;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.Transparent;
            this.btnStart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnStart.Enabled = false;
            this.btnStart.FlatAppearance.BorderSize = 0;
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnStart.Image = global::RateController.Properties.Resources.Update4;
            this.btnStart.Location = new System.Drawing.Point(268, 352);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(72, 72);
            this.btnStart.TabIndex = 14;
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
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
            this.btnCancel.Location = new System.Drawing.Point(348, 352);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 72);
            this.btnCancel.TabIndex = 15;
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
            this.btnClose.Location = new System.Drawing.Point(428, 352);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(72, 72);
            this.btnClose.TabIndex = 16;
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // ofdHex
            // 
            this.ofdHex.Filter = "Firmware files (*.hex)|*.hex";
            // 
            // frmFirmwareUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(512, 436);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.btnCheck);
            this.Controls.Add(this.lbStatus);
            this.Controls.Add(this.pbProgress);
            this.Controls.Add(this.lbHexFile);
            this.Controls.Add(this.lbFile);
            this.Controls.Add(this.lbFileName);
            this.Controls.Add(this.lbCurrent);
            this.Controls.Add(this.lbCurrentName);
            this.Controls.Add(this.lbLink);
            this.Controls.Add(this.lbLinkName);
            this.Controls.Add(this.lbModule);
            this.Controls.Add(this.lbModuleName);
            this.Controls.Add(this.lbBoard);
            this.Controls.Add(this.lbBoardName);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmFirmwareUpdate";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Firmware Update";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmFirmwareUpdate_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmFirmwareUpdate_FormClosed);
            this.Load += new System.EventHandler(this.frmFirmwareUpdate_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbBoardName;
        private System.Windows.Forms.Label lbBoard;
        private System.Windows.Forms.Label lbModuleName;
        private System.Windows.Forms.Label lbModule;
        private System.Windows.Forms.Label lbLinkName;
        private System.Windows.Forms.Label lbLink;
        private System.Windows.Forms.Label lbCurrentName;
        private System.Windows.Forms.Label lbCurrent;
        private System.Windows.Forms.Label lbFileName;
        private System.Windows.Forms.Label lbFile;
        private System.Windows.Forms.Label lbHexFile;
        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.Button btnCheck;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.OpenFileDialog ofdHex;
    }
}
