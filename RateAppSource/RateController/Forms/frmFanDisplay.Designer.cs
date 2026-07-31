namespace RateController
{
    partial class frmFanDisplay
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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lbFanName = new System.Windows.Forms.Label();
            this.lbFanValue = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // timer1
            //
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            //
            // lbFanName
            //
            this.lbFanName.BackColor = System.Drawing.Color.Transparent;
            this.lbFanName.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbFanName.ForeColor = System.Drawing.Color.Yellow;
            this.lbFanName.Location = new System.Drawing.Point(6, 0);
            this.lbFanName.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbFanName.Name = "lbFanName";
            this.lbFanName.Size = new System.Drawing.Size(74, 62);
            this.lbFanName.TabIndex = 3;
            this.lbFanName.Text = "Fan 1";
            this.lbFanName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbFanName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseDown);
            this.lbFanName.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseMove);
            this.lbFanName.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmFanDisplay_MouseUp);
            //
            // lbFanValue
            //
            this.lbFanValue.BackColor = System.Drawing.Color.Transparent;
            this.lbFanValue.Font = new System.Drawing.Font("Tahoma", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbFanValue.ForeColor = System.Drawing.Color.Yellow;
            this.lbFanValue.Location = new System.Drawing.Point(84, 0);
            this.lbFanValue.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbFanValue.Name = "lbFanValue";
            this.lbFanValue.Size = new System.Drawing.Size(140, 62);
            this.lbFanValue.TabIndex = 1;
            this.lbFanValue.Text = "3500";
            this.lbFanValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbFanValue.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseDown);
            this.lbFanValue.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseMove);
            this.lbFanValue.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmFanDisplay_MouseUp);
            //
            // frmFanDisplay
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(230, 68);
            this.Controls.Add(this.lbFanName);
            this.Controls.Add(this.lbFanValue);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmFanDisplay";
            this.ShowInTaskbar = false;
            this.Text = "Fan";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmFanDisplay_FormClosing);
            this.Load += new System.EventHandler(this.frmFanDisplay_Load);
            this.LocationChanged += new System.EventHandler(this.frmFanDisplay_LocationChanged);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmFanDisplay_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lbFanName;
        private System.Windows.Forms.Label lbFanValue;
    }
}
