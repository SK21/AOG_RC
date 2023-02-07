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
            this.lbIPdes = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.bntOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(65, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Version";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbAppVersion
            // 
            this.lbAppVersion.AutoSize = true;
            this.lbAppVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbAppVersion.Location = new System.Drawing.Point(158, 9);
            this.lbAppVersion.Name = "lbAppVersion";
            this.lbAppVersion.Size = new System.Drawing.Size(167, 24);
            this.lbAppVersion.TabIndex = 1;
            this.lbAppVersion.Text = "23-Apr-2022   1.0.0";
            this.lbAppVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbIP
            // 
            this.lbIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbIP.Location = new System.Drawing.Point(158, 43);
            this.lbIP.Name = "lbIP";
            this.lbIP.Size = new System.Drawing.Size(149, 31);
            this.lbIP.TabIndex = 2;
            this.lbIP.Text = "192.168.5.100";
            this.lbIP.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbIPdes
            // 
            this.lbIPdes.AutoSize = true;
            this.lbIPdes.Location = new System.Drawing.Point(65, 46);
            this.lbIPdes.Name = "lbIPdes";
            this.lbIPdes.Size = new System.Drawing.Size(76, 24);
            this.lbIPdes.TabIndex = 4;
            this.lbIPdes.Text = "Local IP";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.Control;
            this.textBox1.Location = new System.Drawing.Point(12, 77);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(363, 383);
            this.textBox1.TabIndex = 6;
            this.textBox1.Text = resources.GetString("textBox1.Text");
            // 
            // bntOK
            // 
            this.bntOK.BackColor = System.Drawing.Color.Transparent;
            this.bntOK.FlatAppearance.BorderSize = 0;
            this.bntOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bntOK.Image = global::PCBsetup.Properties.Resources.bntOK_Image;
            this.bntOK.Location = new System.Drawing.Point(157, 466);
            this.bntOK.Name = "bntOK";
            this.bntOK.Size = new System.Drawing.Size(72, 72);
            this.bntOK.TabIndex = 3;
            this.bntOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.bntOK.UseVisualStyleBackColor = false;
            this.bntOK.Click += new System.EventHandler(this.bntOK_Click);
            // 
            // frmAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(387, 543);
            this.Controls.Add(this.textBox1);
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
        private System.Windows.Forms.TextBox textBox1;
    }
}