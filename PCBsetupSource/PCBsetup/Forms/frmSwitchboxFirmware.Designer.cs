namespace PCBsetup.Forms
{
    partial class frmSwitchboxFirmware
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSwitchboxFirmware));
            this.ckSWOldBootloader = new System.Windows.Forms.CheckBox();
            this.ckSWethernet = new System.Windows.Forms.CheckBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnDefault = new System.Windows.Forms.Button();
            this.btnUpload = new System.Windows.Forms.Button();
            this.bntOK = new System.Windows.Forms.Button();
            this.tbHexfile = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.WatchDogTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // ckSWOldBootloader
            // 
            this.ckSWOldBootloader.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckSWOldBootloader.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ckSWOldBootloader.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckSWOldBootloader.Location = new System.Drawing.Point(398, 54);
            this.ckSWOldBootloader.Name = "ckSWOldBootloader";
            this.ckSWOldBootloader.Size = new System.Drawing.Size(117, 69);
            this.ckSWOldBootloader.TabIndex = 25;
            this.ckSWOldBootloader.Text = "Use Old Bootloader";
            this.ckSWOldBootloader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckSWOldBootloader.UseVisualStyleBackColor = true;
            // 
            // ckSWethernet
            // 
            this.ckSWethernet.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckSWethernet.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ckSWethernet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckSWethernet.Location = new System.Drawing.Point(275, 54);
            this.ckSWethernet.Name = "ckSWethernet";
            this.ckSWethernet.Size = new System.Drawing.Size(117, 69);
            this.ckSWethernet.TabIndex = 24;
            this.ckSWethernet.Text = "Use Ethernet";
            this.ckSWethernet.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckSWethernet.UseVisualStyleBackColor = true;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Image = global::PCBsetup.Properties.Resources.btnBrowse_Image;
            this.btnBrowse.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnBrowse.Location = new System.Drawing.Point(25, 157);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(115, 72);
            this.btnBrowse.TabIndex = 3;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnDefault
            // 
            this.btnDefault.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnDefault.Location = new System.Drawing.Point(150, 157);
            this.btnDefault.Name = "btnDefault";
            this.btnDefault.Size = new System.Drawing.Size(115, 72);
            this.btnDefault.TabIndex = 2;
            this.btnDefault.Text = "Use Default";
            this.btnDefault.UseVisualStyleBackColor = true;
            this.btnDefault.Click += new System.EventHandler(this.btnDefault_Click);
            // 
            // btnUpload
            // 
            this.btnUpload.Image = global::PCBsetup.Properties.Resources.btnUpload_Image;
            this.btnUpload.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnUpload.Location = new System.Drawing.Point(275, 157);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(115, 72);
            this.btnUpload.TabIndex = 0;
            this.btnUpload.Text = "Upload";
            this.btnUpload.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // bntOK
            // 
            this.bntOK.Image = global::PCBsetup.Properties.Resources.bntOK_Image;
            this.bntOK.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.bntOK.Location = new System.Drawing.Point(400, 157);
            this.bntOK.Name = "bntOK";
            this.bntOK.Size = new System.Drawing.Size(115, 72);
            this.bntOK.TabIndex = 1;
            this.bntOK.Text = "Close";
            this.bntOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.bntOK.UseVisualStyleBackColor = true;
            this.bntOK.Click += new System.EventHandler(this.bntOK_Click);
            // 
            // tbHexfile
            // 
            this.tbHexfile.Location = new System.Drawing.Point(25, 12);
            this.tbHexfile.Name = "tbHexfile";
            this.tbHexfile.Size = new System.Drawing.Size(492, 29);
            this.tbHexfile.TabIndex = 19;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(25, 78);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(240, 20);
            this.progressBar1.TabIndex = 18;
            // 
            // WatchDogTimer
            // 
            this.WatchDogTimer.Interval = 1000;
            this.WatchDogTimer.Tick += new System.EventHandler(this.WatchDogTimer_Tick);
            // 
            // frmSwitchboxFirmware
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(542, 240);
            this.Controls.Add(this.ckSWOldBootloader);
            this.Controls.Add(this.ckSWethernet);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.btnDefault);
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.bntOK);
            this.Controls.Add(this.tbHexfile);
            this.Controls.Add(this.progressBar1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSwitchboxFirmware";
            this.ShowInTaskbar = false;
            this.Text = "Nano Switchbox Firmware";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmSwitchboxFirmware_FormClosed);
            this.Load += new System.EventHandler(this.frmSwitchboxFirmware_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox ckSWOldBootloader;
        private System.Windows.Forms.CheckBox ckSWethernet;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnDefault;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.Button bntOK;
        private System.Windows.Forms.TextBox tbHexfile;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Timer WatchDogTimer;
    }
}