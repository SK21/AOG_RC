namespace RateController.Forms
{
    partial class frmRate
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
            this.lbRate = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btRestore = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbRate
            // 
            this.lbRate.BackColor = System.Drawing.Color.Transparent;
            this.lbRate.Font = new System.Drawing.Font("Tahoma", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRate.ForeColor = System.Drawing.Color.Yellow;
            this.lbRate.Location = new System.Drawing.Point(65, 0);
            this.lbRate.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbRate.Name = "lbRate";
            this.lbRate.Size = new System.Drawing.Size(159, 62);
            this.lbRate.TabIndex = 2;
            this.lbRate.Text = "0.0";
            this.lbRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbRate.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseDown);
            this.lbRate.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseMove);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick_1);
            // 
            // btRestore
            // 
            this.btRestore.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(220)))), ((int)(((byte)(231)))));
            this.btRestore.BackgroundImage = global::RateController.Properties.Resources.RC_logo;
            this.btRestore.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btRestore.Location = new System.Drawing.Point(6, 8);
            this.btRestore.Margin = new System.Windows.Forms.Padding(2);
            this.btRestore.Name = "btRestore";
            this.btRestore.Size = new System.Drawing.Size(50, 52);
            this.btRestore.TabIndex = 200;
            this.btRestore.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btRestore.UseVisualStyleBackColor = false;
            this.btRestore.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseDown);
            this.btRestore.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseMove);
            // 
            // frmRate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(230, 68);
            this.Controls.Add(this.btRestore);
            this.Controls.Add(this.lbRate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmRate";
            this.ShowInTaskbar = false;
            this.Text = "frmRate";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmRate_FormClosed);
            this.Load += new System.EventHandler(this.frmRate_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbRate;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btRestore;
    }
}