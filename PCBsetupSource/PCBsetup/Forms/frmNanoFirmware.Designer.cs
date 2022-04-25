namespace PCBsetup.Forms
{
    partial class frmNanoFirmware
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNanoFirmware));
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnDefault = new System.Windows.Forms.Button();
            this.btnUpload = new System.Windows.Forms.Button();
            this.bntOK = new System.Windows.Forms.Button();
            this.tbHexfile = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.ckRtEthernet = new System.Windows.Forms.CheckBox();
            this.ckRtOldBootloader = new System.Windows.Forms.CheckBox();
            this.WatchDogTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // btnBrowse
            // 
            this.btnBrowse.Image = global::PCBsetup.Properties.Resources.btnBrowse_Image;
            this.btnBrowse.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnBrowse.Location = new System.Drawing.Point(26, 157);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(115, 72);
            this.btnBrowse.TabIndex = 15;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnDefault
            // 
            this.btnDefault.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnDefault.Location = new System.Drawing.Point(151, 157);
            this.btnDefault.Name = "btnDefault";
            this.btnDefault.Size = new System.Drawing.Size(115, 72);
            this.btnDefault.TabIndex = 14;
            this.btnDefault.Text = "Use Default";
            this.btnDefault.UseVisualStyleBackColor = true;
            this.btnDefault.Click += new System.EventHandler(this.btnDefault_Click);
            // 
            // btnUpload
            // 
            this.btnUpload.Image = global::PCBsetup.Properties.Resources.btnUpload_Image;
            this.btnUpload.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnUpload.Location = new System.Drawing.Point(276, 157);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(115, 72);
            this.btnUpload.TabIndex = 13;
            this.btnUpload.Text = "Upload";
            this.btnUpload.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // bntOK
            // 
            this.bntOK.Image = global::PCBsetup.Properties.Resources.bntOK_Image;
            this.bntOK.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.bntOK.Location = new System.Drawing.Point(401, 157);
            this.bntOK.Name = "bntOK";
            this.bntOK.Size = new System.Drawing.Size(115, 72);
            this.bntOK.TabIndex = 12;
            this.bntOK.Text = "Close";
            this.bntOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.bntOK.UseVisualStyleBackColor = true;
            this.bntOK.Click += new System.EventHandler(this.bntOK_Click);
            // 
            // tbHexfile
            // 
            this.tbHexfile.Location = new System.Drawing.Point(26, 12);
            this.tbHexfile.Name = "tbHexfile";
            this.tbHexfile.Size = new System.Drawing.Size(492, 29);
            this.tbHexfile.TabIndex = 11;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(26, 78);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(240, 20);
            this.progressBar1.TabIndex = 10;
            // 
            // ckRtEthernet
            // 
            this.ckRtEthernet.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckRtEthernet.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ckRtEthernet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckRtEthernet.Location = new System.Drawing.Point(276, 54);
            this.ckRtEthernet.Name = "ckRtEthernet";
            this.ckRtEthernet.Size = new System.Drawing.Size(117, 69);
            this.ckRtEthernet.TabIndex = 16;
            this.ckRtEthernet.Text = "Use Ethernet";
            this.ckRtEthernet.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckRtEthernet.UseVisualStyleBackColor = true;
            // 
            // ckRtOldBootloader
            // 
            this.ckRtOldBootloader.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckRtOldBootloader.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ckRtOldBootloader.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckRtOldBootloader.Location = new System.Drawing.Point(399, 54);
            this.ckRtOldBootloader.Name = "ckRtOldBootloader";
            this.ckRtOldBootloader.Size = new System.Drawing.Size(117, 69);
            this.ckRtOldBootloader.TabIndex = 17;
            this.ckRtOldBootloader.Text = "Use Old Bootloader";
            this.ckRtOldBootloader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckRtOldBootloader.UseVisualStyleBackColor = true;
            // 
            // WatchDogTimer
            // 
            this.WatchDogTimer.Interval = 1000;
            // 
            // frmNanoFirmware
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(542, 241);
            this.Controls.Add(this.ckRtOldBootloader);
            this.Controls.Add(this.ckRtEthernet);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.btnDefault);
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.bntOK);
            this.Controls.Add(this.tbHexfile);
            this.Controls.Add(this.progressBar1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmNanoFirmware";
            this.ShowInTaskbar = false;
            this.Text = "Nano Rate Firmware";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmNanoFirmware_FormClosed);
            this.Load += new System.EventHandler(this.frmNanoFirmware_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnDefault;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.Button bntOK;
        private System.Windows.Forms.TextBox tbHexfile;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.CheckBox ckRtEthernet;
        private System.Windows.Forms.CheckBox ckRtOldBootloader;
        private System.Windows.Forms.Timer WatchDogTimer;
    }
}