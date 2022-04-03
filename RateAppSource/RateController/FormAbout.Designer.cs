
namespace RateController
{
    partial class FormAbout
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAbout));
            this.IPA1 = new System.Windows.Forms.TextBox();
            this.IPA2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.IPA3 = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.lbAppVersion = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.bntOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // IPA1
            // 
            this.IPA1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IPA1.Location = new System.Drawing.Point(159, 46);
            this.IPA1.Name = "IPA1";
            this.IPA1.Size = new System.Drawing.Size(36, 53);
            this.IPA1.TabIndex = 1;
            this.IPA1.Text = "255";
            this.IPA1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.IPA1.Click += new System.EventHandler(this.IPA1_Click);
            this.IPA1.TextChanged += new System.EventHandler(this.IPA1_TextChanged);
            // 
            // IPA2
            // 
            this.IPA2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IPA2.Location = new System.Drawing.Point(201, 46);
            this.IPA2.Name = "IPA2";
            this.IPA2.Size = new System.Drawing.Size(36, 53);
            this.IPA2.TabIndex = 2;
            this.IPA2.Text = "255";
            this.IPA2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.IPA2.Click += new System.EventHandler(this.IPA2_Click);
            this.IPA2.TextChanged += new System.EventHandler(this.IPA2_TextChanged);
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(285, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 30);
            this.label2.TabIndex = 3;
            this.label2.Text = "255";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // IPA3
            // 
            this.IPA3.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IPA3.Location = new System.Drawing.Point(243, 46);
            this.IPA3.Name = "IPA3";
            this.IPA3.Size = new System.Drawing.Size(36, 53);
            this.IPA3.TabIndex = 3;
            this.IPA3.Text = "255";
            this.IPA3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.IPA3.Click += new System.EventHandler(this.IPA3_Click);
            this.IPA3.TextChanged += new System.EventHandler(this.IPA3_TextChanged);
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label27.Location = new System.Drawing.Point(11, 50);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(257, 46);
            this.label27.TabIndex = 177;
            this.label27.Text = "Destination IP";
            this.label27.Click += new System.EventHandler(this.label27_Click);
            // 
            // lbAppVersion
            // 
            this.lbAppVersion.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbAppVersion.Location = new System.Drawing.Point(12, 20);
            this.lbAppVersion.Name = "lbAppVersion";
            this.lbAppVersion.Size = new System.Drawing.Size(315, 18);
            this.lbAppVersion.TabIndex = 180;
            this.lbAppVersion.Text = "Version 2.1.9";
            this.lbAppVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 11.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(315, 18);
            this.label1.TabIndex = 181;
            this.label1.Text = "Version";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnCancel
            // 
            this.btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnCancel.Enabled = false;
            this.btnCancel.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCancel.Location = new System.Drawing.Point(12, 91);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(115, 72);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // bntOK
            // 
            this.bntOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bntOK.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.bntOK.Image = ((System.Drawing.Image)(resources.GetObject("bntOK.Image")));
            this.bntOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.bntOK.Location = new System.Drawing.Point(212, 91);
            this.bntOK.Name = "bntOK";
            this.bntOK.Size = new System.Drawing.Size(115, 72);
            this.bntOK.TabIndex = 0;
            this.bntOK.Text = "Close";
            this.bntOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.bntOK.UseVisualStyleBackColor = true;
            this.bntOK.Click += new System.EventHandler(this.bntOK_Click);
            // 
            // FormAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 27F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(339, 175);
            this.Controls.Add(this.IPA1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.IPA2);
            this.Controls.Add(this.lbAppVersion);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.IPA3);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label27);
            this.Controls.Add(this.bntOK);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAbout";
            this.ShowInTaskbar = false;
            this.Text = "About";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormAbout_FormClosed);
            this.Load += new System.EventHandler(this.FormAbout_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Button bntOK;
        private System.Windows.Forms.TextBox IPA3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox IPA1;
        private System.Windows.Forms.TextBox IPA2;
        private System.Windows.Forms.Label lbAppVersion;
        private System.Windows.Forms.Label label1;
    }
}