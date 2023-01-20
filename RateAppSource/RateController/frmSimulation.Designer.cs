namespace RateController
{
    partial class frmSimulation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSimulation));
            this.tbSpeed = new System.Windows.Forms.TextBox();
            this.rbRate = new System.Windows.Forms.RadioButton();
            this.rbSpeed = new System.Windows.Forms.RadioButton();
            this.rbPWM = new System.Windows.Forms.RadioButton();
            this.lbMPH = new System.Windows.Forms.Label();
            this.rbOff = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // tbSpeed
            // 
            this.tbSpeed.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSpeed.Location = new System.Drawing.Point(15, 191);
            this.tbSpeed.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tbSpeed.MaxLength = 8;
            this.tbSpeed.Name = "tbSpeed";
            this.tbSpeed.Size = new System.Drawing.Size(73, 30);
            this.tbSpeed.TabIndex = 4;
            this.tbSpeed.Text = "5.5";
            this.tbSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSpeed.Enter += new System.EventHandler(this.tbSpeed_Enter);
            // 
            // rbRate
            // 
            this.rbRate.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbRate.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.rbRate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbRate.Location = new System.Drawing.Point(13, 145);
            this.rbRate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbRate.Name = "rbRate";
            this.rbRate.Size = new System.Drawing.Size(173, 36);
            this.rbRate.TabIndex = 1;
            this.rbRate.Tag = "0";
            this.rbRate.Text = "Rate - no module";
            this.rbRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbRate.UseVisualStyleBackColor = true;
            this.rbRate.Click += new System.EventHandler(this.rbOff_Click);
            // 
            // rbSpeed
            // 
            this.rbSpeed.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbSpeed.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.rbSpeed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbSpeed.Location = new System.Drawing.Point(13, 57);
            this.rbSpeed.Margin = new System.Windows.Forms.Padding(4);
            this.rbSpeed.Name = "rbSpeed";
            this.rbSpeed.Size = new System.Drawing.Size(173, 36);
            this.rbSpeed.TabIndex = 2;
            this.rbSpeed.Tag = "0";
            this.rbSpeed.Text = "Speed";
            this.rbSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbSpeed.UseVisualStyleBackColor = true;
            this.rbSpeed.Click += new System.EventHandler(this.rbOff_Click);
            // 
            // rbPWM
            // 
            this.rbPWM.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbPWM.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.rbPWM.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbPWM.Location = new System.Drawing.Point(13, 101);
            this.rbPWM.Margin = new System.Windows.Forms.Padding(4);
            this.rbPWM.Name = "rbPWM";
            this.rbPWM.Size = new System.Drawing.Size(173, 36);
            this.rbPWM.TabIndex = 3;
            this.rbPWM.Tag = "0";
            this.rbPWM.Text = "PWM";
            this.rbPWM.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbPWM.UseVisualStyleBackColor = true;
            this.rbPWM.Click += new System.EventHandler(this.rbOff_Click);
            // 
            // lbMPH
            // 
            this.lbMPH.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMPH.Location = new System.Drawing.Point(97, 190);
            this.lbMPH.Name = "lbMPH";
            this.lbMPH.Size = new System.Drawing.Size(89, 30);
            this.lbMPH.TabIndex = 152;
            this.lbMPH.Text = "MPH";
            this.lbMPH.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rbOff
            // 
            this.rbOff.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbOff.Checked = true;
            this.rbOff.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.rbOff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbOff.Location = new System.Drawing.Point(13, 13);
            this.rbOff.Margin = new System.Windows.Forms.Padding(4);
            this.rbOff.Name = "rbOff";
            this.rbOff.Size = new System.Drawing.Size(173, 36);
            this.rbOff.TabIndex = 0;
            this.rbOff.Tag = "0";
            this.rbOff.Text = "Off";
            this.rbOff.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbOff.UseVisualStyleBackColor = true;
            this.rbOff.Click += new System.EventHandler(this.rbOff_Click);
            // 
            // frmSimulation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(197, 228);
            this.Controls.Add(this.rbOff);
            this.Controls.Add(this.lbMPH);
            this.Controls.Add(this.rbPWM);
            this.Controls.Add(this.rbSpeed);
            this.Controls.Add(this.rbRate);
            this.Controls.Add(this.tbSpeed);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSimulation";
            this.ShowInTaskbar = false;
            this.Text = "Simulation";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmSimulation_FormClosed);
            this.Load += new System.EventHandler(this.frmSimulation_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbSpeed;
        private System.Windows.Forms.RadioButton rbRate;
        private System.Windows.Forms.RadioButton rbSpeed;
        private System.Windows.Forms.RadioButton rbPWM;
        private System.Windows.Forms.Label lbMPH;
        private System.Windows.Forms.RadioButton rbOff;
    }
}