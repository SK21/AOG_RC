
namespace RateController
{
    partial class RCRestore
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
            this.btRestore = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btRestore
            // 
            this.btRestore.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(220)))), ((int)(((byte)(231)))));
            this.btRestore.BackgroundImage = global::RateController.Properties.Resources.RC_logo;
            this.btRestore.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btRestore.Location = new System.Drawing.Point(12, 12);
            this.btRestore.Name = "btRestore";
            this.btRestore.Size = new System.Drawing.Size(100, 100);
            this.btRestore.TabIndex = 199;
            this.btRestore.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btRestore.UseVisualStyleBackColor = false;
            this.btRestore.Click += new System.EventHandler(this.RestoreLC_Click);
            // 
            // RCRestore
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(125, 125);
            this.ControlBox = false;
            this.Controls.Add(this.btRestore);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "RCRestore";
            this.Text = "RCRestore";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.RCRestore_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btRestore;
    }
}