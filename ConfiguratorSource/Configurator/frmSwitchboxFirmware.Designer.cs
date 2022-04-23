namespace Configurator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSwitchboxFirmware));
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnDefault = new System.Windows.Forms.Button();
            this.bntOK = new System.Windows.Forms.Button();
            this.tbHexfile = new System.Windows.Forms.TextBox();
            this.btnUpload = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.ckSWethernet = new System.Windows.Forms.CheckBox();
            this.ckSWOldBootloader = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 82);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(249, 20);
            this.progressBar1.TabIndex = 160;
            this.progressBar1.Visible = false;
            // 
            // btnDefault
            // 
            this.btnDefault.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnDefault.Location = new System.Drawing.Point(144, 146);
            this.btnDefault.Margin = new System.Windows.Forms.Padding(17);
            this.btnDefault.Name = "btnDefault";
            this.btnDefault.Size = new System.Drawing.Size(117, 83);
            this.btnDefault.TabIndex = 159;
            this.btnDefault.Text = "Use Default";
            this.btnDefault.UseVisualStyleBackColor = true;
            this.btnDefault.Click += new System.EventHandler(this.btnDefault_Click);
            // 
            // bntOK
            // 
            this.bntOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bntOK.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.bntOK.Image = ((System.Drawing.Image)(resources.GetObject("bntOK.Image")));
            this.bntOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.bntOK.Location = new System.Drawing.Point(408, 146);
            this.bntOK.Margin = new System.Windows.Forms.Padding(17);
            this.bntOK.Name = "bntOK";
            this.bntOK.Size = new System.Drawing.Size(117, 83);
            this.bntOK.TabIndex = 158;
            this.bntOK.Text = "Close";
            this.bntOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.bntOK.UseVisualStyleBackColor = true;
            this.bntOK.Click += new System.EventHandler(this.bntOK_Click);
            // 
            // tbHexfile
            // 
            this.tbHexfile.Location = new System.Drawing.Point(12, 9);
            this.tbHexfile.Margin = new System.Windows.Forms.Padding(17);
            this.tbHexfile.Name = "tbHexfile";
            this.tbHexfile.Size = new System.Drawing.Size(515, 30);
            this.tbHexfile.TabIndex = 157;
            // 
            // btnUpload
            // 
            this.btnUpload.Image = ((System.Drawing.Image)(resources.GetObject("btnUpload.Image")));
            this.btnUpload.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnUpload.Location = new System.Drawing.Point(276, 146);
            this.btnUpload.Margin = new System.Windows.Forms.Padding(17);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(117, 83);
            this.btnUpload.TabIndex = 156;
            this.btnUpload.Text = "Upload";
            this.btnUpload.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Image = ((System.Drawing.Image)(resources.GetObject("btnBrowse.Image")));
            this.btnBrowse.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnBrowse.Location = new System.Drawing.Point(12, 146);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(17);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(117, 83);
            this.btnBrowse.TabIndex = 155;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // ckSWethernet
            // 
            this.ckSWethernet.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckSWethernet.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckSWethernet.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ckSWethernet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckSWethernet.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ckSWethernet.Location = new System.Drawing.Point(276, 58);
            this.ckSWethernet.Margin = new System.Windows.Forms.Padding(2);
            this.ckSWethernet.Name = "ckSWethernet";
            this.ckSWethernet.Size = new System.Drawing.Size(117, 69);
            this.ckSWethernet.TabIndex = 283;
            this.ckSWethernet.Text = "Use Ethernet";
            this.ckSWethernet.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckSWethernet.UseVisualStyleBackColor = true;
            // 
            // ckSWOldBootloader
            // 
            this.ckSWOldBootloader.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckSWOldBootloader.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckSWOldBootloader.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ckSWOldBootloader.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckSWOldBootloader.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ckSWOldBootloader.Location = new System.Drawing.Point(410, 58);
            this.ckSWOldBootloader.Margin = new System.Windows.Forms.Padding(2);
            this.ckSWOldBootloader.Name = "ckSWOldBootloader";
            this.ckSWOldBootloader.Size = new System.Drawing.Size(117, 69);
            this.ckSWOldBootloader.TabIndex = 284;
            this.ckSWOldBootloader.Text = "Use Old Bootloader";
            this.ckSWOldBootloader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckSWOldBootloader.UseVisualStyleBackColor = true;
            // 
            // frmSwitchboxFirmware
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(542, 240);
            this.Controls.Add(this.ckSWOldBootloader);
            this.Controls.Add(this.ckSWethernet);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnDefault);
            this.Controls.Add(this.bntOK);
            this.Controls.Add(this.tbHexfile);
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.btnBrowse);
            this.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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
        private ProgressBar progressBar1;
        private Button btnDefault;
        private Button bntOK;
        private TextBox tbHexfile;
        private Button btnUpload;
        private Button btnBrowse;
        private CheckBox ckSWethernet;
        private CheckBox ckSWOldBootloader;
    }
}