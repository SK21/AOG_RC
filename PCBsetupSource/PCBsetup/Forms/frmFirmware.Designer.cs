namespace PCBsetup.Forms
{
    partial class frmFirmware
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFirmware));
            this.label1 = new System.Windows.Forms.Label();
            this.lbTeensies = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.tbHexfile = new System.Windows.Forms.TextBox();
            this.bntOK = new System.Windows.Forms.Button();
            this.btnUpload = new System.Windows.Forms.Button();
            this.btnDefault = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(191, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Connected Teensies:";
            // 
            // lbTeensies
            // 
            this.lbTeensies.FormattingEnabled = true;
            this.lbTeensies.ItemHeight = 24;
            this.lbTeensies.Location = new System.Drawing.Point(12, 36);
            this.lbTeensies.Name = "lbTeensies";
            this.lbTeensies.Size = new System.Drawing.Size(492, 100);
            this.lbTeensies.TabIndex = 1;
            this.lbTeensies.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.lbTeensies_HelpRequested);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 154);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 24);
            this.label2.TabIndex = 2;
            this.label2.Text = "Firmware:";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(118, 155);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(115, 20);
            this.progressBar.TabIndex = 3;
            // 
            // tbHexfile
            // 
            this.tbHexfile.Location = new System.Drawing.Point(12, 191);
            this.tbHexfile.Name = "tbHexfile";
            this.tbHexfile.Size = new System.Drawing.Size(492, 29);
            this.tbHexfile.TabIndex = 4;
            this.tbHexfile.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.tbHexfile_HelpRequested);
            // 
            // bntOK
            // 
            this.bntOK.BackColor = System.Drawing.Color.Transparent;
            this.bntOK.FlatAppearance.BorderSize = 0;
            this.bntOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bntOK.Image = global::PCBsetup.Properties.Resources.bntOK_Image;
            this.bntOK.Location = new System.Drawing.Point(387, 235);
            this.bntOK.Name = "bntOK";
            this.bntOK.Size = new System.Drawing.Size(115, 72);
            this.bntOK.TabIndex = 1;
            this.bntOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.bntOK.UseVisualStyleBackColor = false;
            this.bntOK.Click += new System.EventHandler(this.bntOK_Click);
            // 
            // btnUpload
            // 
            this.btnUpload.BackColor = System.Drawing.Color.Transparent;
            this.btnUpload.FlatAppearance.BorderSize = 0;
            this.btnUpload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpload.Image = global::PCBsetup.Properties.Resources.UpArrow64;
            this.btnUpload.Location = new System.Drawing.Point(262, 235);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(115, 72);
            this.btnUpload.TabIndex = 0;
            this.btnUpload.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnUpload.UseVisualStyleBackColor = false;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            this.btnUpload.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.btnUpload_HelpRequested);
            // 
            // btnDefault
            // 
            this.btnDefault.BackColor = System.Drawing.Color.Transparent;
            this.btnDefault.FlatAppearance.BorderSize = 0;
            this.btnDefault.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDefault.Image = global::PCBsetup.Properties.Resources.VehFileLoad;
            this.btnDefault.Location = new System.Drawing.Point(137, 235);
            this.btnDefault.Name = "btnDefault";
            this.btnDefault.Size = new System.Drawing.Size(115, 72);
            this.btnDefault.TabIndex = 2;
            this.btnDefault.UseVisualStyleBackColor = false;
            this.btnDefault.Click += new System.EventHandler(this.btnDefault_Click);
            this.btnDefault.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.btnDefault_HelpRequested);
            // 
            // btnBrowse
            // 
            this.btnBrowse.BackColor = System.Drawing.Color.Transparent;
            this.btnBrowse.FlatAppearance.BorderSize = 0;
            this.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowse.Image = global::PCBsetup.Properties.Resources.btnBrowse_Image;
            this.btnBrowse.Location = new System.Drawing.Point(12, 235);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(115, 72);
            this.btnBrowse.TabIndex = 3;
            this.btnBrowse.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnBrowse.UseVisualStyleBackColor = false;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            this.btnBrowse.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.btnBrowse_HelpRequested);
            // 
            // frmFirmware
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(516, 319);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.btnDefault);
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.bntOK);
            this.Controls.Add(this.tbHexfile);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbTeensies);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmFirmware";
            this.ShowInTaskbar = false;
            this.Text = "Teensy AutoSteer Firmware";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmFirmware_FormClosed);
            this.Load += new System.EventHandler(this.frmFirmware_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lbTeensies;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.TextBox tbHexfile;
        private System.Windows.Forms.Button bntOK;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.Button btnDefault;
        private System.Windows.Forms.Button btnBrowse;
    }
}