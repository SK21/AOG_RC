
namespace RateController
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
            this.lblFWType = new System.Windows.Forms.Label();
            this.tbHexfile = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnUpload = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lbTeensies = new System.Windows.Forms.ListBox();
            this.bntOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblFWType
            // 
            this.lblFWType.AutoSize = true;
            this.lblFWType.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFWType.Location = new System.Drawing.Point(18, 248);
            this.lblFWType.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblFWType.Name = "lblFWType";
            this.lblFWType.Size = new System.Drawing.Size(0, 15);
            this.lblFWType.TabIndex = 37;
            // 
            // tbHexfile
            // 
            this.tbHexfile.Location = new System.Drawing.Point(20, 210);
            this.tbHexfile.Margin = new System.Windows.Forms.Padding(5);
            this.tbHexfile.Name = "tbHexfile";
            this.tbHexfile.Size = new System.Drawing.Size(492, 30);
            this.tbHexfile.TabIndex = 36;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(24, 287);
            this.progressBar.Margin = new System.Windows.Forms.Padding(5);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(115, 33);
            this.progressBar.TabIndex = 35;
            this.progressBar.Visible = false;
            // 
            // btnUpload
            // 
            this.btnUpload.Image = ((System.Drawing.Image)(resources.GetObject("btnUpload.Image")));
            this.btnUpload.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnUpload.Location = new System.Drawing.Point(272, 250);
            this.btnUpload.Margin = new System.Windows.Forms.Padding(5);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(115, 72);
            this.btnUpload.TabIndex = 34;
            this.btnUpload.Text = "Upload";
            this.btnUpload.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Image = ((System.Drawing.Image)(resources.GetObject("btnBrowse.Image")));
            this.btnBrowse.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnBrowse.Location = new System.Drawing.Point(147, 250);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(5);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(115, 72);
            this.btnBrowse.TabIndex = 33;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 177);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 23);
            this.label2.TabIndex = 32;
            this.label2.Text = "Firmware:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 16);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(184, 23);
            this.label1.TabIndex = 31;
            this.label1.Text = "Connected Teensies:";
            // 
            // lbTeensies
            // 
            this.lbTeensies.DisplayMember = "description";
            this.lbTeensies.FormattingEnabled = true;
            this.lbTeensies.ItemHeight = 23;
            this.lbTeensies.Location = new System.Drawing.Point(20, 44);
            this.lbTeensies.Margin = new System.Windows.Forms.Padding(5);
            this.lbTeensies.Name = "lbTeensies";
            this.lbTeensies.Size = new System.Drawing.Size(492, 119);
            this.lbTeensies.TabIndex = 30;
            // 
            // bntOK
            // 
            this.bntOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bntOK.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.bntOK.Image = ((System.Drawing.Image)(resources.GetObject("bntOK.Image")));
            this.bntOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.bntOK.Location = new System.Drawing.Point(397, 250);
            this.bntOK.Margin = new System.Windows.Forms.Padding(5);
            this.bntOK.Name = "bntOK";
            this.bntOK.Size = new System.Drawing.Size(115, 72);
            this.bntOK.TabIndex = 137;
            this.bntOK.Text = "Close";
            this.bntOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.bntOK.UseVisualStyleBackColor = true;
            this.bntOK.Click += new System.EventHandler(this.bntOK_Click);
            // 
            // frmFirmware
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(530, 332);
            this.Controls.Add(this.bntOK);
            this.Controls.Add(this.lblFWType);
            this.Controls.Add(this.tbHexfile);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbTeensies);
            this.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmFirmware";
            this.Text = "Upload Teensy Firmware";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmFirmware_FormClosed);
            this.Load += new System.EventHandler(this.frmFirmware_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblFWType;
        private System.Windows.Forms.TextBox tbHexfile;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lbTeensies;
        private System.Windows.Forms.Button bntOK;
    }
}