
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
            this.components = new System.ComponentModel.Container();
            this.btRestore = new System.Windows.Forms.Button();
            this.lbRate = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // btRestore
            // 
            this.btRestore.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(220)))), ((int)(((byte)(231)))));
            this.btRestore.BackgroundImage = global::RateController.Properties.Resources.RC_logo;
            this.btRestore.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btRestore.Location = new System.Drawing.Point(6, 7);
            this.btRestore.Margin = new System.Windows.Forms.Padding(2);
            this.btRestore.Name = "btRestore";
            this.btRestore.Size = new System.Drawing.Size(50, 52);
            this.btRestore.TabIndex = 199;
            this.btRestore.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btRestore.UseVisualStyleBackColor = false;
            this.btRestore.Click += new System.EventHandler(this.RestoreLC_Click);
            this.btRestore.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseDown);
            this.btRestore.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseMove);
            // 
            // lbRate
            // 
            this.lbRate.BackColor = System.Drawing.Color.Transparent;
            this.lbRate.Font = new System.Drawing.Font("Tahoma", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRate.ForeColor = System.Drawing.Color.Yellow;
            this.lbRate.Location = new System.Drawing.Point(65, 0);
            this.lbRate.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbRate.Name = "lbRate";
            this.lbRate.Size = new System.Drawing.Size(140, 62);
            this.lbRate.TabIndex = 200;
            this.lbRate.Text = "10000";
            this.lbRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbRate.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseDown);
            this.lbRate.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseMove);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // RCRestore
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(210, 68);
            this.ControlBox = false;
            this.Controls.Add(this.lbRate);
            this.Controls.Add(this.btRestore);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "RCRestore";
            this.ShowInTaskbar = false;
            this.Text = "RCRestore";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RCRestore_FormClosing);
            this.Load += new System.EventHandler(this.RCRestore_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btRestore;
        private System.Windows.Forms.Label lbRate;
        private System.Windows.Forms.Timer timer1;
    }
}