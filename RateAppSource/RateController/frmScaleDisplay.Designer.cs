namespace RateController
{
    partial class frmScaleDisplay
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmScaleDisplay));
            this.lbValue = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnScale = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbValue
            // 
            this.lbValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbValue.Location = new System.Drawing.Point(122, 12);
            this.lbValue.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbValue.Name = "lbValue";
            this.lbValue.Size = new System.Drawing.Size(121, 76);
            this.lbValue.TabIndex = 4;
            this.lbValue.Text = "10000 (AC)";
            this.lbValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbValue.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lbValue_MouseClick);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnScale
            // 
            this.btnScale.BackColor = System.Drawing.Color.Transparent;
            this.btnScale.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnScale.FlatAppearance.BorderSize = 0;
            this.btnScale.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnScale.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnScale.Image = global::RateController.Properties.Resources.scale;
            this.btnScale.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnScale.Location = new System.Drawing.Point(12, 12);
            this.btnScale.Name = "btnScale";
            this.btnScale.Size = new System.Drawing.Size(101, 76);
            this.btnScale.TabIndex = 262;
            this.btnScale.Text = "4";
            this.btnScale.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.btnScale.UseVisualStyleBackColor = false;
            this.btnScale.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseClick);
            this.btnScale.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseDown);
            this.btnScale.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseMove);
            // 
            // frmScaleDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(252, 99);
            this.Controls.Add(this.btnScale);
            this.Controls.Add(this.lbValue);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmScaleDisplay";
            this.ShowInTaskbar = false;
            this.Text = "Scale";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmScaleDisplay_FormClosed);
            this.Load += new System.EventHandler(this.frmScaleDisplay_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lbValue;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnScale;
    }
}