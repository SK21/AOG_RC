namespace PCBsetup.Forms
{
    partial class frmAbout
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAbout));
            this.label1 = new System.Windows.Forms.Label();
            this.lbAppVersion = new System.Windows.Forms.Label();
            this.lbIP = new System.Windows.Forms.Label();
            this.bntOK = new System.Windows.Forms.Button();
            this.lbIPdes = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(231, 31);
            this.label1.TabIndex = 0;
            this.label1.Text = "Version";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbAppVersion
            // 
            this.lbAppVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbAppVersion.Location = new System.Drawing.Point(12, 42);
            this.lbAppVersion.Name = "lbAppVersion";
            this.lbAppVersion.Size = new System.Drawing.Size(231, 31);
            this.lbAppVersion.TabIndex = 1;
            this.lbAppVersion.Text = "23-Apr-2022   1.0.0";
            this.lbAppVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbIP
            // 
            this.lbIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbIP.Location = new System.Drawing.Point(94, 85);
            this.lbIP.Name = "lbIP";
            this.lbIP.Size = new System.Drawing.Size(149, 31);
            this.lbIP.TabIndex = 2;
            this.lbIP.Text = "192.168.5.100";
            this.lbIP.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // bntOK
            // 
            this.bntOK.Image = global::PCBsetup.Properties.Resources.bntOK_Image;
            this.bntOK.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.bntOK.Location = new System.Drawing.Point(72, 118);
            this.bntOK.Name = "bntOK";
            this.bntOK.Size = new System.Drawing.Size(115, 72);
            this.bntOK.TabIndex = 3;
            this.bntOK.Text = "Close";
            this.bntOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.bntOK.UseVisualStyleBackColor = true;
            this.bntOK.Click += new System.EventHandler(this.bntOK_Click);
            // 
            // lbIPdes
            // 
            this.lbIPdes.AutoSize = true;
            this.lbIPdes.Location = new System.Drawing.Point(12, 88);
            this.lbIPdes.Name = "lbIPdes";
            this.lbIPdes.Size = new System.Drawing.Size(76, 24);
            this.lbIPdes.TabIndex = 4;
            this.lbIPdes.Text = "Local IP";
            // 
            // frmAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(255, 202);
            this.Controls.Add(this.lbIPdes);
            this.Controls.Add(this.bntOK);
            this.Controls.Add(this.lbIP);
            this.Controls.Add(this.lbAppVersion);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAbout";
            this.ShowInTaskbar = false;
            this.Text = "About";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmAbout_FormClosed);
            this.Load += new System.EventHandler(this.frmAbout_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbAppVersion;
        private System.Windows.Forms.Label lbIP;
        private System.Windows.Forms.Button bntOK;
        private System.Windows.Forms.Label lbIPdes;
    }
}