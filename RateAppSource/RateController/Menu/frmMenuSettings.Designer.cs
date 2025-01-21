namespace RateController.Menu
{
    partial class frmMenuSettings
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
            this.ckScale = new System.Windows.Forms.CheckBox();
            this.ckBumpButtons = new System.Windows.Forms.CheckBox();
            this.ckDefault = new System.Windows.Forms.CheckBox();
            this.lbPercent = new System.Windows.Forms.Label();
            this.tbOffRate = new System.Windows.Forms.TextBox();
            this.ckOffRate = new System.Windows.Forms.CheckBox();
            this.ckOnScreen = new System.Windows.Forms.CheckBox();
            this.grpMinUPM = new System.Windows.Forms.GroupBox();
            this.tbUPMspeed = new System.Windows.Forms.TextBox();
            this.rbUPMSpeed = new System.Windows.Forms.RadioButton();
            this.rbUPMFixed = new System.Windows.Forms.RadioButton();
            this.tbMinUPM = new System.Windows.Forms.TextBox();
            this.grpSensor = new System.Windows.Forms.GroupBox();
            this.lbSensorID = new System.Windows.Forms.Label();
            this.tbSenID = new System.Windows.Forms.TextBox();
            this.ModuleIndicator = new System.Windows.Forms.Label();
            this.lbConID = new System.Windows.Forms.Label();
            this.tbConID = new System.Windows.Forms.TextBox();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lbProduct = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.grpMinUPM.SuspendLayout();
            this.grpSensor.SuspendLayout();
            this.SuspendLayout();
            // 
            // ckScale
            // 
            this.ckScale.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckScale.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckScale.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckScale.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckScale.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckScale.Location = new System.Drawing.Point(305, 409);
            this.ckScale.Name = "ckScale";
            this.ckScale.Size = new System.Drawing.Size(162, 34);
            this.ckScale.TabIndex = 149;
            this.ckScale.Text = "Scale Weight";
            this.ckScale.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckScale.UseVisualStyleBackColor = true;
            this.ckScale.CheckedChanged += new System.EventHandler(this.ckDefault_CheckedChanged);
            // 
            // ckBumpButtons
            // 
            this.ckBumpButtons.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckBumpButtons.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckBumpButtons.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckBumpButtons.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckBumpButtons.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckBumpButtons.Location = new System.Drawing.Point(75, 409);
            this.ckBumpButtons.Name = "ckBumpButtons";
            this.ckBumpButtons.Size = new System.Drawing.Size(162, 34);
            this.ckBumpButtons.TabIndex = 148;
            this.ckBumpButtons.Text = "Bump Buttons";
            this.ckBumpButtons.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckBumpButtons.UseVisualStyleBackColor = true;
            this.ckBumpButtons.CheckedChanged += new System.EventHandler(this.ckDefault_CheckedChanged);
            // 
            // ckDefault
            // 
            this.ckDefault.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckDefault.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckDefault.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckDefault.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckDefault.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckDefault.Location = new System.Drawing.Point(75, 344);
            this.ckDefault.Name = "ckDefault";
            this.ckDefault.Size = new System.Drawing.Size(162, 34);
            this.ckDefault.TabIndex = 0;
            this.ckDefault.Text = "Default Product";
            this.ckDefault.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckDefault.UseVisualStyleBackColor = true;
            this.ckDefault.CheckedChanged += new System.EventHandler(this.ckDefault_CheckedChanged);
            // 
            // lbPercent
            // 
            this.lbPercent.AutoSize = true;
            this.lbPercent.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPercent.Location = new System.Drawing.Point(275, 479);
            this.lbPercent.Name = "lbPercent";
            this.lbPercent.Size = new System.Drawing.Size(29, 23);
            this.lbPercent.TabIndex = 146;
            this.lbPercent.Text = "%";
            // 
            // tbOffRate
            // 
            this.tbOffRate.Enabled = false;
            this.tbOffRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbOffRate.Location = new System.Drawing.Point(224, 476);
            this.tbOffRate.MaxLength = 8;
            this.tbOffRate.Name = "tbOffRate";
            this.tbOffRate.Size = new System.Drawing.Size(45, 29);
            this.tbOffRate.TabIndex = 145;
            this.tbOffRate.Text = "20";
            this.tbOffRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbOffRate.TextChanged += new System.EventHandler(this.ckDefault_CheckedChanged);
            this.tbOffRate.Enter += new System.EventHandler(this.tbOffRate_Enter);
            this.tbOffRate.Validating += new System.ComponentModel.CancelEventHandler(this.tbOffRate_Validating);
            // 
            // ckOffRate
            // 
            this.ckOffRate.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckOffRate.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckOffRate.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckOffRate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckOffRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckOffRate.Location = new System.Drawing.Point(75, 474);
            this.ckOffRate.Name = "ckOffRate";
            this.ckOffRate.Size = new System.Drawing.Size(143, 34);
            this.ckOffRate.TabIndex = 144;
            this.ckOffRate.Text = "Off-rate Alarm   ";
            this.ckOffRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckOffRate.UseVisualStyleBackColor = true;
            this.ckOffRate.CheckedChanged += new System.EventHandler(this.ckOffRate_CheckedChanged);
            // 
            // ckOnScreen
            // 
            this.ckOnScreen.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckOnScreen.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckOnScreen.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckOnScreen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckOnScreen.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckOnScreen.Location = new System.Drawing.Point(305, 344);
            this.ckOnScreen.Name = "ckOnScreen";
            this.ckOnScreen.Size = new System.Drawing.Size(162, 34);
            this.ckOnScreen.TabIndex = 143;
            this.ckOnScreen.Text = "On Screen";
            this.ckOnScreen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckOnScreen.UseVisualStyleBackColor = true;
            this.ckOnScreen.CheckedChanged += new System.EventHandler(this.ckDefault_CheckedChanged);
            // 
            // grpMinUPM
            // 
            this.grpMinUPM.Controls.Add(this.tbUPMspeed);
            this.grpMinUPM.Controls.Add(this.rbUPMSpeed);
            this.grpMinUPM.Controls.Add(this.rbUPMFixed);
            this.grpMinUPM.Controls.Add(this.tbMinUPM);
            this.grpMinUPM.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpMinUPM.Location = new System.Drawing.Point(86, 196);
            this.grpMinUPM.Name = "grpMinUPM";
            this.grpMinUPM.Size = new System.Drawing.Size(354, 117);
            this.grpMinUPM.TabIndex = 142;
            this.grpMinUPM.TabStop = false;
            this.grpMinUPM.Text = "Minimum UPM";
            this.grpMinUPM.Paint += new System.Windows.Forms.PaintEventHandler(this.grpMinUPM_Paint);
            // 
            // tbUPMspeed
            // 
            this.tbUPMspeed.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbUPMspeed.Location = new System.Drawing.Point(250, 73);
            this.tbUPMspeed.MaxLength = 8;
            this.tbUPMspeed.Name = "tbUPMspeed";
            this.tbUPMspeed.Size = new System.Drawing.Size(75, 29);
            this.tbUPMspeed.TabIndex = 124;
            this.tbUPMspeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbUPMspeed.TextChanged += new System.EventHandler(this.ckDefault_CheckedChanged);
            this.tbUPMspeed.Enter += new System.EventHandler(this.tbUPMspeed_Enter);
            // 
            // rbUPMSpeed
            // 
            this.rbUPMSpeed.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbUPMSpeed.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbUPMSpeed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbUPMSpeed.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbUPMSpeed.Location = new System.Drawing.Point(15, 70);
            this.rbUPMSpeed.Margin = new System.Windows.Forms.Padding(2);
            this.rbUPMSpeed.Name = "rbUPMSpeed";
            this.rbUPMSpeed.Size = new System.Drawing.Size(128, 37);
            this.rbUPMSpeed.TabIndex = 123;
            this.rbUPMSpeed.Tag = "0";
            this.rbUPMSpeed.Text = "By Speed";
            this.rbUPMSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbUPMSpeed.UseVisualStyleBackColor = true;
            this.rbUPMSpeed.CheckedChanged += new System.EventHandler(this.rbUPMSpeed_CheckedChanged);
            // 
            // rbUPMFixed
            // 
            this.rbUPMFixed.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbUPMFixed.Checked = true;
            this.rbUPMFixed.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbUPMFixed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbUPMFixed.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbUPMFixed.Location = new System.Drawing.Point(15, 27);
            this.rbUPMFixed.Margin = new System.Windows.Forms.Padding(2);
            this.rbUPMFixed.Name = "rbUPMFixed";
            this.rbUPMFixed.Size = new System.Drawing.Size(128, 37);
            this.rbUPMFixed.TabIndex = 122;
            this.rbUPMFixed.TabStop = true;
            this.rbUPMFixed.Tag = "0";
            this.rbUPMFixed.Text = "Fixed Value";
            this.rbUPMFixed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbUPMFixed.UseVisualStyleBackColor = true;
            this.rbUPMFixed.CheckedChanged += new System.EventHandler(this.rbUPMFixed_CheckedChanged);
            // 
            // tbMinUPM
            // 
            this.tbMinUPM.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbMinUPM.Location = new System.Drawing.Point(250, 30);
            this.tbMinUPM.MaxLength = 8;
            this.tbMinUPM.Name = "tbMinUPM";
            this.tbMinUPM.Size = new System.Drawing.Size(75, 29);
            this.tbMinUPM.TabIndex = 1;
            this.tbMinUPM.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbMinUPM.TextChanged += new System.EventHandler(this.ckDefault_CheckedChanged);
            this.tbMinUPM.Enter += new System.EventHandler(this.tbMinUPM_Enter);
            // 
            // grpSensor
            // 
            this.grpSensor.Controls.Add(this.lbSensorID);
            this.grpSensor.Controls.Add(this.tbSenID);
            this.grpSensor.Controls.Add(this.ModuleIndicator);
            this.grpSensor.Controls.Add(this.lbConID);
            this.grpSensor.Controls.Add(this.tbConID);
            this.grpSensor.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpSensor.Location = new System.Drawing.Point(86, 48);
            this.grpSensor.Name = "grpSensor";
            this.grpSensor.Size = new System.Drawing.Size(354, 117);
            this.grpSensor.TabIndex = 141;
            this.grpSensor.TabStop = false;
            this.grpSensor.Text = "Rate Sensor Location";
            this.grpSensor.Paint += new System.Windows.Forms.PaintEventHandler(this.grpSensor_Paint);
            // 
            // lbSensorID
            // 
            this.lbSensorID.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSensorID.Location = new System.Drawing.Point(11, 79);
            this.lbSensorID.Name = "lbSensorID";
            this.lbSensorID.Size = new System.Drawing.Size(93, 23);
            this.lbSensorID.TabIndex = 151;
            this.lbSensorID.Text = "Sensor ID";
            // 
            // tbSenID
            // 
            this.tbSenID.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSenID.Location = new System.Drawing.Point(184, 76);
            this.tbSenID.MaxLength = 8;
            this.tbSenID.Name = "tbSenID";
            this.tbSenID.Size = new System.Drawing.Size(67, 30);
            this.tbSenID.TabIndex = 1;
            this.tbSenID.Text = "0";
            this.tbSenID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSenID.TextChanged += new System.EventHandler(this.tbConID_TextChanged);
            this.tbSenID.Enter += new System.EventHandler(this.tbSenID_Enter);
            this.tbSenID.Validating += new System.ComponentModel.CancelEventHandler(this.tbSenID_Validating);
            // 
            // ModuleIndicator
            // 
            this.ModuleIndicator.BackColor = System.Drawing.SystemColors.Control;
            this.ModuleIndicator.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ModuleIndicator.Image = global::RateController.Properties.Resources.Off;
            this.ModuleIndicator.Location = new System.Drawing.Point(299, 46);
            this.ModuleIndicator.Name = "ModuleIndicator";
            this.ModuleIndicator.Size = new System.Drawing.Size(41, 37);
            this.ModuleIndicator.TabIndex = 149;
            this.ModuleIndicator.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbConID
            // 
            this.lbConID.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbConID.Location = new System.Drawing.Point(6, 30);
            this.lbConID.Name = "lbConID";
            this.lbConID.Size = new System.Drawing.Size(97, 23);
            this.lbConID.TabIndex = 148;
            this.lbConID.Text = "Module ID";
            // 
            // tbConID
            // 
            this.tbConID.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbConID.Location = new System.Drawing.Point(184, 28);
            this.tbConID.MaxLength = 8;
            this.tbConID.Name = "tbConID";
            this.tbConID.Size = new System.Drawing.Size(67, 30);
            this.tbConID.TabIndex = 0;
            this.tbConID.Text = "0";
            this.tbConID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbConID.TextChanged += new System.EventHandler(this.tbConID_TextChanged);
            this.tbConID.Enter += new System.EventHandler(this.tbConID_Enter);
            this.tbConID.Validating += new System.ComponentModel.CancelEventHandler(this.tbConID_Validating);
            // 
            // btnRight
            // 
            this.btnRight.BackColor = System.Drawing.Color.Transparent;
            this.btnRight.FlatAppearance.BorderSize = 0;
            this.btnRight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRight.Image = global::RateController.Properties.Resources.ArrowRight1;
            this.btnRight.Location = new System.Drawing.Point(300, 546);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(72, 72);
            this.btnRight.TabIndex = 330;
            this.btnRight.UseVisualStyleBackColor = false;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnLeft
            // 
            this.btnLeft.BackColor = System.Drawing.Color.Transparent;
            this.btnLeft.FlatAppearance.BorderSize = 0;
            this.btnLeft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLeft.Image = global::RateController.Properties.Resources.ArrowLeft1;
            this.btnLeft.Location = new System.Drawing.Point(222, 546);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(72, 72);
            this.btnLeft.TabIndex = 329;
            this.btnLeft.UseVisualStyleBackColor = false;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
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
            this.btnCancel.TabIndex = 328;
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
            this.btnOK.TabIndex = 327;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lbProduct
            // 
            this.lbProduct.BackColor = System.Drawing.SystemColors.Control;
            this.lbProduct.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbProduct.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbProduct.Location = new System.Drawing.Point(71, 9);
            this.lbProduct.Name = "lbProduct";
            this.lbProduct.Size = new System.Drawing.Size(396, 36);
            this.lbProduct.TabIndex = 332;
            this.lbProduct.Text = "Product";
            this.lbProduct.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // frmMenuSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 630);
            this.Controls.Add(this.lbProduct);
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.btnLeft);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.ckScale);
            this.Controls.Add(this.ckBumpButtons);
            this.Controls.Add(this.ckDefault);
            this.Controls.Add(this.lbPercent);
            this.Controls.Add(this.tbOffRate);
            this.Controls.Add(this.ckOffRate);
            this.Controls.Add(this.ckOnScreen);
            this.Controls.Add(this.grpMinUPM);
            this.Controls.Add(this.grpSensor);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMenuSettings";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuSettings";
            this.Activated += new System.EventHandler(this.frmMenuSettings_Activated);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMenuSettings_FormClosed);
            this.Load += new System.EventHandler(this.frmMenuSettings_Load);
            this.grpMinUPM.ResumeLayout(false);
            this.grpMinUPM.PerformLayout();
            this.grpSensor.ResumeLayout(false);
            this.grpSensor.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox ckScale;
        private System.Windows.Forms.CheckBox ckBumpButtons;
        private System.Windows.Forms.CheckBox ckDefault;
        private System.Windows.Forms.Label lbPercent;
        private System.Windows.Forms.TextBox tbOffRate;
        private System.Windows.Forms.CheckBox ckOffRate;
        private System.Windows.Forms.CheckBox ckOnScreen;
        private System.Windows.Forms.GroupBox grpMinUPM;
        private System.Windows.Forms.TextBox tbUPMspeed;
        private System.Windows.Forms.RadioButton rbUPMSpeed;
        private System.Windows.Forms.RadioButton rbUPMFixed;
        private System.Windows.Forms.TextBox tbMinUPM;
        private System.Windows.Forms.GroupBox grpSensor;
        private System.Windows.Forms.Label lbSensorID;
        private System.Windows.Forms.TextBox tbSenID;
        private System.Windows.Forms.Label ModuleIndicator;
        private System.Windows.Forms.Label lbConID;
        private System.Windows.Forms.TextBox tbConID;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lbProduct;
        private System.Windows.Forms.Timer timer1;
    }
}