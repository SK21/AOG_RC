
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
            this.lbRateAmount = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // btRestore
            // 
            this.btRestore.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(220)))), ((int)(((byte)(231)))));
            this.btRestore.BackgroundImage = global::RateController.Properties.Resources.RC_logo;
            this.btRestore.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btRestore.Location = new System.Drawing.Point(6, 6);
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
            // lbRateAmount
            // 
            this.lbRateAmount.BackColor = System.Drawing.Color.Transparent;
            this.lbRateAmount.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRateAmount.ForeColor = System.Drawing.Color.Yellow;
            this.lbRateAmount.Location = new System.Drawing.Point(70, 0);
            this.lbRateAmount.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbRateAmount.Name = "lbRateAmount";
            this.lbRateAmount.Size = new System.Drawing.Size(104, 62);
            this.lbRateAmount.TabIndex = 200;
            this.lbRateAmount.Text = "0.0";
            this.lbRateAmount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbRateAmount.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseDown);
            this.lbRateAmount.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseMove);
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
            this.ClientSize = new System.Drawing.Size(185, 68);
            this.ControlBox = false;
            this.Controls.Add(this.lbRateAmount);
            this.Controls.Add(this.btRestore);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "RCRestore";
            this.ShowInTaskbar = false;
            this.Text = "RCRestore";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.RCRestore_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btRestore;
        private System.Windows.Forms.Label lbRateAmount;
        private System.Windows.Forms.Timer timer1;
    }
}