namespace RateController.Menu
{
    partial class frmMenuPins
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.tbPressure = new System.Windows.Forms.TextBox();
            this.lbPressure = new System.Windows.Forms.Label();
            this.ckMomentary = new System.Windows.Forms.CheckBox();
            this.tbWrk = new System.Windows.Forms.TextBox();
            this.lbWorkPin = new System.Windows.Forms.Label();
            this.tbPWM2 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbPWM1 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbDir2 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbDir1 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbFlow2 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbFlow1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnCancel.Enabled = false;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnCancel.Image = global::RateController.Properties.Resources.Cancel64;
            this.btnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCancel.Location = new System.Drawing.Point(378, 546);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 72);
            this.btnCancel.TabIndex = 153;
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.Transparent;
            this.btnOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnOK.Enabled = false;
            this.btnOK.FlatAppearance.BorderSize = 0;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnOK.Image = global::RateController.Properties.Resources.Save;
            this.btnOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnOK.Location = new System.Drawing.Point(456, 546);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(72, 72);
            this.btnOK.TabIndex = 152;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // tbPressure
            // 
            this.tbPressure.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPressure.Location = new System.Drawing.Point(246, 433);
            this.tbPressure.Name = "tbPressure";
            this.tbPressure.Size = new System.Drawing.Size(58, 29);
            this.tbPressure.TabIndex = 238;
            this.tbPressure.TabStop = false;
            this.tbPressure.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbPressure
            // 
            this.lbPressure.AutoSize = true;
            this.lbPressure.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPressure.Location = new System.Drawing.Point(91, 435);
            this.lbPressure.Name = "lbPressure";
            this.lbPressure.Size = new System.Drawing.Size(117, 24);
            this.lbPressure.TabIndex = 237;
            this.lbPressure.Text = "Pressure Pin";
            // 
            // ckMomentary
            // 
            this.ckMomentary.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckMomentary.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckMomentary.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckMomentary.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckMomentary.Location = new System.Drawing.Point(360, 374);
            this.ckMomentary.Name = "ckMomentary";
            this.ckMomentary.Size = new System.Drawing.Size(119, 40);
            this.ckMomentary.TabIndex = 236;
            this.ckMomentary.Text = "Momentary";
            this.ckMomentary.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckMomentary.UseVisualStyleBackColor = true;
            this.ckMomentary.CheckedChanged += new System.EventHandler(this.Boxes_TextChanged);
            // 
            // tbWrk
            // 
            this.tbWrk.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbWrk.Location = new System.Drawing.Point(246, 380);
            this.tbWrk.Name = "tbWrk";
            this.tbWrk.Size = new System.Drawing.Size(58, 29);
            this.tbWrk.TabIndex = 235;
            this.tbWrk.TabStop = false;
            this.tbWrk.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbWorkPin
            // 
            this.lbWorkPin.AutoSize = true;
            this.lbWorkPin.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbWorkPin.Location = new System.Drawing.Point(91, 382);
            this.lbWorkPin.Name = "lbWorkPin";
            this.lbWorkPin.Size = new System.Drawing.Size(86, 24);
            this.lbWorkPin.TabIndex = 234;
            this.lbWorkPin.Text = "Work Pin";
            // 
            // tbPWM2
            // 
            this.tbPWM2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPWM2.Location = new System.Drawing.Point(246, 327);
            this.tbPWM2.Name = "tbPWM2";
            this.tbPWM2.Size = new System.Drawing.Size(58, 29);
            this.tbPWM2.TabIndex = 233;
            this.tbPWM2.TabStop = false;
            this.tbPWM2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(91, 329);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 24);
            this.label7.TabIndex = 232;
            this.label7.Text = "PWM 2";
            // 
            // tbPWM1
            // 
            this.tbPWM1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPWM1.Location = new System.Drawing.Point(246, 274);
            this.tbPWM1.Name = "tbPWM1";
            this.tbPWM1.Size = new System.Drawing.Size(58, 29);
            this.tbPWM1.TabIndex = 231;
            this.tbPWM1.TabStop = false;
            this.tbPWM1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(91, 276);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 24);
            this.label8.TabIndex = 230;
            this.label8.Text = "PWM 1";
            // 
            // tbDir2
            // 
            this.tbDir2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDir2.Location = new System.Drawing.Point(246, 221);
            this.tbDir2.Name = "tbDir2";
            this.tbDir2.Size = new System.Drawing.Size(58, 29);
            this.tbDir2.TabIndex = 229;
            this.tbDir2.TabStop = false;
            this.tbDir2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(91, 223);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 24);
            this.label5.TabIndex = 228;
            this.label5.Text = "Dir 2";
            // 
            // tbDir1
            // 
            this.tbDir1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDir1.Location = new System.Drawing.Point(246, 168);
            this.tbDir1.Name = "tbDir1";
            this.tbDir1.Size = new System.Drawing.Size(58, 29);
            this.tbDir1.TabIndex = 227;
            this.tbDir1.TabStop = false;
            this.tbDir1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(91, 170);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 24);
            this.label6.TabIndex = 226;
            this.label6.Text = "Dir 1";
            // 
            // tbFlow2
            // 
            this.tbFlow2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbFlow2.Location = new System.Drawing.Point(246, 115);
            this.tbFlow2.Name = "tbFlow2";
            this.tbFlow2.Size = new System.Drawing.Size(58, 29);
            this.tbFlow2.TabIndex = 225;
            this.tbFlow2.TabStop = false;
            this.tbFlow2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(91, 117);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 24);
            this.label3.TabIndex = 224;
            this.label3.Text = "Flow 2";
            // 
            // tbFlow1
            // 
            this.tbFlow1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbFlow1.Location = new System.Drawing.Point(246, 62);
            this.tbFlow1.Name = "tbFlow1";
            this.tbFlow1.Size = new System.Drawing.Size(58, 29);
            this.tbFlow1.TabIndex = 223;
            this.tbFlow1.TabStop = false;
            this.tbFlow1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(91, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 24);
            this.label4.TabIndex = 222;
            this.label4.Text = "Flow 1";
            // 
            // frmMenuPins
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 630);
            this.Controls.Add(this.tbPressure);
            this.Controls.Add(this.lbPressure);
            this.Controls.Add(this.ckMomentary);
            this.Controls.Add(this.tbWrk);
            this.Controls.Add(this.lbWorkPin);
            this.Controls.Add(this.tbPWM2);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tbPWM1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.tbDir2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbDir1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tbFlow2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbFlow1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMenuPins";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuPins";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMenuPins_FormClosed);
            this.Load += new System.EventHandler(this.frmMenuPins_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox tbPressure;
        private System.Windows.Forms.Label lbPressure;
        private System.Windows.Forms.CheckBox ckMomentary;
        private System.Windows.Forms.TextBox tbWrk;
        private System.Windows.Forms.Label lbWorkPin;
        private System.Windows.Forms.TextBox tbPWM2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbPWM1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbDir2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbDir1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbFlow2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbFlow1;
        private System.Windows.Forms.Label label4;
    }
}