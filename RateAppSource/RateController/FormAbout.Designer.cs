
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
            this.lbAppVersion = new System.Windows.Forms.Label();
            this.lbVersion = new System.Windows.Forms.Label();
            this.bntOK = new System.Windows.Forms.Button();
            this.lbVersionDate = new System.Windows.Forms.Label();
            this.lbDate = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbAppVersion
            // 
            this.lbAppVersion.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbAppVersion.Location = new System.Drawing.Point(127, 2);
            this.lbAppVersion.Name = "lbAppVersion";
            this.lbAppVersion.Size = new System.Drawing.Size(144, 23);
            this.lbAppVersion.TabIndex = 180;
            this.lbAppVersion.Text = "1.0.0";
            this.lbAppVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbVersion
            // 
            this.lbVersion.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbVersion.Location = new System.Drawing.Point(12, 2);
            this.lbVersion.Name = "lbVersion";
            this.lbVersion.Size = new System.Drawing.Size(105, 23);
            this.lbVersion.TabIndex = 181;
            this.lbVersion.Text = "RC Version";
            this.lbVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // bntOK
            // 
            this.bntOK.BackColor = System.Drawing.Color.Transparent;
            this.bntOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bntOK.FlatAppearance.BorderSize = 0;
            this.bntOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bntOK.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.bntOK.Image = global::RateController.Properties.Resources.OK;
            this.bntOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.bntOK.Location = new System.Drawing.Point(89, 69);
            this.bntOK.Name = "bntOK";
            this.bntOK.Size = new System.Drawing.Size(115, 72);
            this.bntOK.TabIndex = 0;
            this.bntOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.bntOK.UseVisualStyleBackColor = false;
            this.bntOK.Click += new System.EventHandler(this.bntOK_Click);
            // 
            // lbVersionDate
            // 
            this.lbVersionDate.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbVersionDate.Location = new System.Drawing.Point(12, 35);
            this.lbVersionDate.Name = "lbVersionDate";
            this.lbVersionDate.Size = new System.Drawing.Size(105, 23);
            this.lbVersionDate.TabIndex = 205;
            this.lbVersionDate.Text = "RC Date";
            this.lbVersionDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbDate
            // 
            this.lbDate.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDate.Location = new System.Drawing.Point(127, 35);
            this.lbDate.Name = "lbDate";
            this.lbDate.Size = new System.Drawing.Size(144, 23);
            this.lbDate.TabIndex = 204;
            this.lbDate.Text = "01 Jan 01";
            this.lbDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(283, 153);
            this.Controls.Add(this.lbVersionDate);
            this.Controls.Add(this.lbDate);
            this.Controls.Add(this.lbVersion);
            this.Controls.Add(this.lbAppVersion);
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

        }

        #endregion
        private System.Windows.Forms.Button bntOK;
        private System.Windows.Forms.Label lbAppVersion;
        private System.Windows.Forms.Label lbVersion;
        private System.Windows.Forms.Label lbVersionDate;
        private System.Windows.Forms.Label lbDate;
    }
}