namespace RateController
{
    partial class frmProductDisplay
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
            this.lbValue = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lbProductName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbValue
            // 
            this.lbValue.BackColor = System.Drawing.Color.Transparent;
            this.lbValue.Font = new System.Drawing.Font("Tahoma", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbValue.ForeColor = System.Drawing.Color.Yellow;
            this.lbValue.Location = new System.Drawing.Point(110, 0);
            this.lbValue.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbValue.Name = "lbValue";
            this.lbValue.Size = new System.Drawing.Size(140, 62);
            this.lbValue.TabIndex = 2;
            this.lbValue.Text = "0.0";
            this.lbValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbValue.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseDown);
            this.lbValue.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseMove);
            this.lbValue.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmProductDisplay_MouseUp);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lbProductName
            // 
            this.lbProductName.AutoEllipsis = true;
            this.lbProductName.BackColor = System.Drawing.Color.Transparent;
            this.lbProductName.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbProductName.ForeColor = System.Drawing.Color.Yellow;
            this.lbProductName.Location = new System.Drawing.Point(6, 20);
            this.lbProductName.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbProductName.Name = "lbProductName";
            this.lbProductName.Size = new System.Drawing.Size(100, 26);
            this.lbProductName.TabIndex = 3;
            this.lbProductName.Text = "Product 1";
            this.lbProductName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbProductName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseDown);
            this.lbProductName.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseMove);
            this.lbProductName.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmProductDisplay_MouseUp);
            // 
            // frmProductDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(256, 68);
            this.Controls.Add(this.lbProductName);
            this.Controls.Add(this.lbValue);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmProductDisplay";
            this.ShowInTaskbar = false;
            this.Text = "Product";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmProductDisplay_FormClosing);
            this.Load += new System.EventHandler(this.frmProductDisplay_Load);
            this.LocationChanged += new System.EventHandler(this.frmProductDisplay_LocationChanged);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmProductDisplay_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbValue;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lbProductName;
    }
}
