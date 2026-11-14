namespace RateController.Menu
{
    partial class frmMenuPrimed
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
            this.ckResume = new System.Windows.Forms.CheckBox();
            this.lbDelaySeconds = new System.Windows.Forms.Label();
            this.tbTime = new System.Windows.Forms.TextBox();
            this.tbSpeed = new System.Windows.Forms.TextBox();
            this.tbDelay = new System.Windows.Forms.TextBox();
            this.lbPrimedSpeed = new System.Windows.Forms.Label();
            this.lbSpeed = new System.Windows.Forms.Label();
            this.lbDelay = new System.Windows.Forms.Label();
            this.lbOnTime = new System.Windows.Forms.Label();
            this.lbOnSeconds = new System.Windows.Forms.Label();
            this.lbPrimeRate = new System.Windows.Forms.Label();
            this.tbPrimeRate = new System.Windows.Forms.TextBox();
            this.lbPrimeRatePct = new System.Windows.Forms.Label();
            this.gbPrimeEnds = new System.Windows.Forms.GroupBox();
            this.rbEndTime = new System.Windows.Forms.RadioButton();
            this.rbEndMoving = new System.Windows.Forms.RadioButton();
            this.tbEndSpeed = new System.Windows.Forms.TextBox();
            this.lbEndSpeedUnit = new System.Windows.Forms.Label();
            this.lbPrimeHint = new System.Windows.Forms.Label();
            this.gbPrimeEnds.SuspendLayout();
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
            this.btnCancel.Location = new System.Drawing.Point(380, 603);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(70, 63);
            this.btnCancel.TabIndex = 1;
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
            this.btnOK.Location = new System.Drawing.Point(458, 603);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(70, 63);
            this.btnOK.TabIndex = 0;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // ckResume
            // 
            this.ckResume.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckResume.Location = new System.Drawing.Point(62, 431);
            this.ckResume.Name = "ckResume";
            this.ckResume.Size = new System.Drawing.Size(466, 27);
            this.ckResume.TabIndex = 7;
            this.ckResume.Text = "Keep Master on when Prime Duration ends";
            this.ckResume.UseVisualStyleBackColor = true;
            this.ckResume.CheckedChanged += new System.EventHandler(this.tbTime_TextChanged);
            // 
            // lbDelaySeconds
            // 
            this.lbDelaySeconds.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDelaySeconds.ForeColor = System.Drawing.Color.DarkGreen;
            this.lbDelaySeconds.Location = new System.Drawing.Point(377, 102);
            this.lbDelaySeconds.Name = "lbDelaySeconds";
            this.lbDelaySeconds.Size = new System.Drawing.Size(89, 24);
            this.lbDelaySeconds.TabIndex = 344;
            this.lbDelaySeconds.Text = "seconds";
            this.lbDelaySeconds.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbTime
            // 
            this.tbTime.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbTime.Location = new System.Drawing.Point(306, 148);
            this.tbTime.MaxLength = 8;
            this.tbTime.Name = "tbTime";
            this.tbTime.Size = new System.Drawing.Size(65, 30);
            this.tbTime.TabIndex = 340;
            this.tbTime.Text = "0";
            this.tbTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbTime.TextChanged += new System.EventHandler(this.tbTime_TextChanged);
            this.tbTime.Enter += new System.EventHandler(this.tbTime_Enter);
            this.tbTime.Validating += new System.ComponentModel.CancelEventHandler(this.tbTime_Validating);
            // 
            // tbSpeed
            // 
            this.tbSpeed.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSpeed.Location = new System.Drawing.Point(306, 197);
            this.tbSpeed.MaxLength = 8;
            this.tbSpeed.Name = "tbSpeed";
            this.tbSpeed.Size = new System.Drawing.Size(65, 30);
            this.tbSpeed.TabIndex = 337;
            this.tbSpeed.Text = "0";
            this.tbSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSpeed.TextChanged += new System.EventHandler(this.tbPrime_TextChanged);
            this.tbSpeed.Enter += new System.EventHandler(this.tbSpeed_Enter);
            this.tbSpeed.Validating += new System.ComponentModel.CancelEventHandler(this.tbSpeed_Validating);
            // 
            // tbDelay
            // 
            this.tbDelay.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDelay.Location = new System.Drawing.Point(306, 99);
            this.tbDelay.MaxLength = 8;
            this.tbDelay.Name = "tbDelay";
            this.tbDelay.Size = new System.Drawing.Size(65, 30);
            this.tbDelay.TabIndex = 343;
            this.tbDelay.Text = "0";
            this.tbDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbDelay.TextChanged += new System.EventHandler(this.tbTime_TextChanged);
            this.tbDelay.Enter += new System.EventHandler(this.tbDelay_Enter);
            this.tbDelay.Validating += new System.ComponentModel.CancelEventHandler(this.tbDelay_Validating);
            // 
            // lbPrimedSpeed
            // 
            this.lbPrimedSpeed.AutoSize = true;
            this.lbPrimedSpeed.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPrimedSpeed.Location = new System.Drawing.Point(62, 201);
            this.lbPrimedSpeed.Name = "lbPrimedSpeed";
            this.lbPrimedSpeed.Size = new System.Drawing.Size(153, 23);
            this.lbPrimedSpeed.TabIndex = 5;
            this.lbPrimedSpeed.Text = "Simulated Speed";
            // 
            // lbSpeed
            // 
            this.lbSpeed.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSpeed.ForeColor = System.Drawing.Color.DarkGreen;
            this.lbSpeed.Location = new System.Drawing.Point(377, 200);
            this.lbSpeed.Name = "lbSpeed";
            this.lbSpeed.Size = new System.Drawing.Size(57, 24);
            this.lbSpeed.TabIndex = 338;
            this.lbSpeed.Text = "mph";
            this.lbSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbDelay
            // 
            this.lbDelay.AutoSize = true;
            this.lbDelay.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDelay.Location = new System.Drawing.Point(62, 103);
            this.lbDelay.Name = "lbDelay";
            this.lbDelay.Size = new System.Drawing.Size(228, 23);
            this.lbDelay.TabIndex = 3;
            this.lbDelay.Text = "Hold Master to start Prime";
            // 
            // lbOnTime
            // 
            this.lbOnTime.AutoSize = true;
            this.lbOnTime.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbOnTime.Location = new System.Drawing.Point(62, 152);
            this.lbOnTime.Name = "lbOnTime";
            this.lbOnTime.Size = new System.Drawing.Size(135, 23);
            this.lbOnTime.TabIndex = 6;
            this.lbOnTime.Text = "Prime Duration";
            // 
            // lbOnSeconds
            // 
            this.lbOnSeconds.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbOnSeconds.ForeColor = System.Drawing.Color.DarkGreen;
            this.lbOnSeconds.Location = new System.Drawing.Point(377, 151);
            this.lbOnSeconds.Name = "lbOnSeconds";
            this.lbOnSeconds.Size = new System.Drawing.Size(89, 24);
            this.lbOnSeconds.TabIndex = 341;
            this.lbOnSeconds.Text = "seconds";
            this.lbOnSeconds.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbPrimeRate
            // 
            this.lbPrimeRate.AutoSize = true;
            this.lbPrimeRate.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPrimeRate.Location = new System.Drawing.Point(62, 250);
            this.lbPrimeRate.Name = "lbPrimeRate";
            this.lbPrimeRate.Size = new System.Drawing.Size(145, 23);
            this.lbPrimeRate.TabIndex = 345;
            this.lbPrimeRate.Text = "Application Rate";
            // 
            // tbPrimeRate
            // 
            this.tbPrimeRate.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPrimeRate.Location = new System.Drawing.Point(306, 246);
            this.tbPrimeRate.MaxLength = 8;
            this.tbPrimeRate.Name = "tbPrimeRate";
            this.tbPrimeRate.Size = new System.Drawing.Size(65, 30);
            this.tbPrimeRate.TabIndex = 346;
            this.tbPrimeRate.Text = "100";
            this.tbPrimeRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbPrimeRate.TextChanged += new System.EventHandler(this.tbPrime_TextChanged);
            this.tbPrimeRate.Enter += new System.EventHandler(this.tbPrimeRate_Enter);
            this.tbPrimeRate.Validating += new System.ComponentModel.CancelEventHandler(this.tbPrimeRate_Validating);
            // 
            // lbPrimeRatePct
            // 
            this.lbPrimeRatePct.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPrimeRatePct.ForeColor = System.Drawing.Color.DarkGreen;
            this.lbPrimeRatePct.Location = new System.Drawing.Point(377, 249);
            this.lbPrimeRatePct.Name = "lbPrimeRatePct";
            this.lbPrimeRatePct.Size = new System.Drawing.Size(57, 24);
            this.lbPrimeRatePct.TabIndex = 347;
            this.lbPrimeRatePct.Text = "%";
            this.lbPrimeRatePct.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gbPrimeEnds
            // 
            this.gbPrimeEnds.Controls.Add(this.rbEndTime);
            this.gbPrimeEnds.Controls.Add(this.rbEndMoving);
            this.gbPrimeEnds.Controls.Add(this.tbEndSpeed);
            this.gbPrimeEnds.Controls.Add(this.lbEndSpeedUnit);
            this.gbPrimeEnds.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbPrimeEnds.Location = new System.Drawing.Point(62, 298);
            this.gbPrimeEnds.Name = "gbPrimeEnds";
            this.gbPrimeEnds.Size = new System.Drawing.Size(392, 114);
            this.gbPrimeEnds.TabIndex = 8;
            this.gbPrimeEnds.TabStop = false;
            this.gbPrimeEnds.Text = "Prime ends";
            this.gbPrimeEnds.Paint += new System.Windows.Forms.PaintEventHandler(this.gbPrimeEnds_Paint);
            // 
            // rbEndTime
            // 
            this.rbEndTime.AutoSize = true;
            this.rbEndTime.Location = new System.Drawing.Point(15, 32);
            this.rbEndTime.Name = "rbEndTime";
            this.rbEndTime.Size = new System.Drawing.Size(254, 27);
            this.rbEndTime.TabIndex = 0;
            this.rbEndTime.TabStop = true;
            this.rbEndTime.Text = "When Prime Duration ends";
            this.rbEndTime.UseVisualStyleBackColor = true;
            this.rbEndTime.CheckedChanged += new System.EventHandler(this.tbPrime_TextChanged);
            // 
            // rbEndMoving
            // 
            this.rbEndMoving.AutoSize = true;
            this.rbEndMoving.Location = new System.Drawing.Point(15, 72);
            this.rbEndMoving.Name = "rbEndMoving";
            this.rbEndMoving.Size = new System.Drawing.Size(166, 27);
            this.rbEndMoving.TabIndex = 1;
            this.rbEndMoving.TabStop = true;
            this.rbEndMoving.Text = "When moving at";
            this.rbEndMoving.UseVisualStyleBackColor = true;
            this.rbEndMoving.CheckedChanged += new System.EventHandler(this.tbPrime_TextChanged);
            // 
            // tbEndSpeed
            // 
            this.tbEndSpeed.Location = new System.Drawing.Point(244, 70);
            this.tbEndSpeed.MaxLength = 8;
            this.tbEndSpeed.Name = "tbEndSpeed";
            this.tbEndSpeed.Size = new System.Drawing.Size(65, 30);
            this.tbEndSpeed.TabIndex = 2;
            this.tbEndSpeed.Text = "0.8";
            this.tbEndSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbEndSpeed.TextChanged += new System.EventHandler(this.tbTime_TextChanged);
            this.tbEndSpeed.Enter += new System.EventHandler(this.tbEndSpeed_Enter);
            this.tbEndSpeed.Validating += new System.ComponentModel.CancelEventHandler(this.tbEndSpeed_Validating);
            // 
            // lbEndSpeedUnit
            // 
            this.lbEndSpeedUnit.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbEndSpeedUnit.ForeColor = System.Drawing.Color.DarkGreen;
            this.lbEndSpeedUnit.Location = new System.Drawing.Point(315, 73);
            this.lbEndSpeedUnit.Name = "lbEndSpeedUnit";
            this.lbEndSpeedUnit.Size = new System.Drawing.Size(57, 24);
            this.lbEndSpeedUnit.TabIndex = 3;
            this.lbEndSpeedUnit.Text = "KMH";
            this.lbEndSpeedUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbPrimeHint
            // 
            this.lbPrimeHint.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPrimeHint.Location = new System.Drawing.Point(62, 478);
            this.lbPrimeHint.Name = "lbPrimeHint";
            this.lbPrimeHint.Size = new System.Drawing.Size(392, 60);
            this.lbPrimeHint.TabIndex = 349;
            this.lbPrimeHint.Text = "A product\'s Min UPM still applies.";
            // 
            // frmMenuPrimed
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 678);
            this.Controls.Add(this.lbPrimeHint);
            this.Controls.Add(this.gbPrimeEnds);
            this.Controls.Add(this.ckResume);
            this.Controls.Add(this.lbPrimeRatePct);
            this.Controls.Add(this.tbPrimeRate);
            this.Controls.Add(this.lbPrimeRate);
            this.Controls.Add(this.lbDelaySeconds);
            this.Controls.Add(this.tbTime);
            this.Controls.Add(this.tbSpeed);
            this.Controls.Add(this.tbDelay);
            this.Controls.Add(this.lbPrimedSpeed);
            this.Controls.Add(this.lbSpeed);
            this.Controls.Add(this.lbDelay);
            this.Controls.Add(this.lbOnTime);
            this.Controls.Add(this.lbOnSeconds);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMenuPrimed";
            this.ShowInTaskbar = false;
            this.Text = "frmPrimed";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMenuPrimed_FormClosed);
            this.Load += new System.EventHandler(this.frmMenuPrimed_Load);
            this.gbPrimeEnds.ResumeLayout(false);
            this.gbPrimeEnds.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox ckResume;
        private System.Windows.Forms.Label lbDelaySeconds;
        private System.Windows.Forms.TextBox tbTime;
        private System.Windows.Forms.TextBox tbSpeed;
        private System.Windows.Forms.TextBox tbDelay;
        private System.Windows.Forms.Label lbPrimedSpeed;
        private System.Windows.Forms.Label lbSpeed;
        private System.Windows.Forms.Label lbDelay;
        private System.Windows.Forms.Label lbOnTime;
        private System.Windows.Forms.Label lbOnSeconds;
        private System.Windows.Forms.Label lbPrimeRate;
        private System.Windows.Forms.TextBox tbPrimeRate;
        private System.Windows.Forms.Label lbPrimeRatePct;
        private System.Windows.Forms.GroupBox gbPrimeEnds;
        private System.Windows.Forms.RadioButton rbEndTime;
        private System.Windows.Forms.RadioButton rbEndMoving;
        private System.Windows.Forms.TextBox tbEndSpeed;
        private System.Windows.Forms.Label lbEndSpeedUnit;
        private System.Windows.Forms.Label lbPrimeHint;
    }
}
