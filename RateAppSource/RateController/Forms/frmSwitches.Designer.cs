namespace RateController
{
    partial class frmSwitches
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSwitches));
            this.btnMaster = new System.Windows.Forms.Button();
            this.btnPrime = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnAutoRate = new System.Windows.Forms.Button();
            this.btnAutoSection = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnMaster
            // 
            this.btnMaster.BackColor = System.Drawing.Color.LightGreen;
            this.btnMaster.Location = new System.Drawing.Point(12, 12);
            this.btnMaster.Name = "btnMaster";
            this.btnMaster.Size = new System.Drawing.Size(64, 46);
            this.btnMaster.TabIndex = 167;
            this.btnMaster.Text = "MST";
            this.btnMaster.UseVisualStyleBackColor = false;
            this.btnMaster.Click += new System.EventHandler(this.btnMaster_Click);
            this.btnMaster.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnMaster_MouseUp);
            //
            // btnPrime
            // 
            this.btnPrime.Location = new System.Drawing.Point(90, 12);
            this.btnPrime.Name = "btnPrime";
            this.btnPrime.Size = new System.Drawing.Size(64, 46);
            this.btnPrime.TabIndex = 164;
            this.btnPrime.Text = "PRM";
            this.btnPrime.UseVisualStyleBackColor = true;
            this.btnPrime.Click += new System.EventHandler(this.btnPrime_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnDown
            // 
            this.btnDown.BackColor = System.Drawing.Color.Transparent;
            this.btnDown.FlatAppearance.BorderSize = 0;
            this.btnDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDown.Image = global::RateController.Properties.Resources.RateDown;
            this.btnDown.Location = new System.Drawing.Point(246, 12);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(64, 46);
            this.btnDown.TabIndex = 166;
            this.btnDown.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnDown.UseVisualStyleBackColor = false;
            this.btnDown.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnDown_MouseDown);
            this.btnDown.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnDown_MouseUp);
            // 
            // btnUp
            // 
            this.btnUp.BackColor = System.Drawing.Color.Transparent;
            this.btnUp.FlatAppearance.BorderSize = 0;
            this.btnUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUp.Image = global::RateController.Properties.Resources.RateUp;
            this.btnUp.Location = new System.Drawing.Point(168, 12);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(64, 46);
            this.btnUp.TabIndex = 165;
            this.btnUp.UseVisualStyleBackColor = false;
            this.btnUp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnUp_MouseDown);
            this.btnUp.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnUp_MouseUp);
            // 
            // btnAutoRate
            // 
            this.btnAutoRate.Location = new System.Drawing.Point(12, 184);
            this.btnAutoRate.Name = "btnAutoRate";
            this.btnAutoRate.Size = new System.Drawing.Size(142, 46);
            this.btnAutoRate.TabIndex = 168;
            this.btnAutoRate.Text = "Auto Rate";
            this.btnAutoRate.UseVisualStyleBackColor = true;
            this.btnAutoRate.Click += new System.EventHandler(this.btnAutoRate_Click);
            // 
            // btnAutoSection
            // 
            this.btnAutoSection.Location = new System.Drawing.Point(168, 184);
            this.btnAutoSection.Name = "btnAutoSection";
            this.btnAutoSection.Size = new System.Drawing.Size(142, 46);
            this.btnAutoSection.TabIndex = 169;
            this.btnAutoSection.Text = "Auto Section";
            this.btnAutoSection.UseVisualStyleBackColor = true;
            this.btnAutoSection.Click += new System.EventHandler(this.btnAutoSection_Click);
            //
            // frmSwitches
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(323, 239);
            this.Controls.Add(this.btnAutoSection);
            this.Controls.Add(this.btnAutoRate);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnMaster);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnPrime);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSwitches";
            this.ShowInTaskbar = false;
            this.Text = "Switches";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmSwitches_Closed);
            this.Load += new System.EventHandler(this.frmSwitches_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnPrime;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnMaster;
        private System.Windows.Forms.Button btnAutoRate;
        private System.Windows.Forms.Button btnAutoSection;
    }
}