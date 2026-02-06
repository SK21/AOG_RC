namespace RateController.Menu
{
    partial class frmMenuOptions
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
            this.ckRateDisplay = new System.Windows.Forms.CheckBox();
            this.ckMetric = new System.Windows.Forms.CheckBox();
            this.tbSimSpeed = new System.Windows.Forms.TextBox();
            this.lbSimUnits = new System.Windows.Forms.Label();
            this.btnCal = new System.Windows.Forms.Button();
            this.lbPulses = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.butUpdateModules = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbSimulated = new System.Windows.Forms.RadioButton();
            this.rbWheel = new System.Windows.Forms.RadioButton();
            this.rbAOG = new System.Windows.Forms.RadioButton();
            this.tbWheelModule = new System.Windows.Forms.TextBox();
            this.lbModule = new System.Windows.Forms.Label();
            this.tbWheelCal = new System.Windows.Forms.TextBox();
            this.tbWheelPin = new System.Windows.Forms.TextBox();
            this.lbPin = new System.Windows.Forms.Label();
            this.lbCal = new System.Windows.Forms.Label();
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
            this.btnCancel.Margin = new System.Windows.Forms.Padding(6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(70, 63);
            this.btnCancel.TabIndex = 162;
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
            this.btnOK.Margin = new System.Windows.Forms.Padding(6);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(70, 63);
            this.btnOK.TabIndex = 161;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // ckRateDisplay
            // 
            this.ckRateDisplay.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckRateDisplay.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckRateDisplay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckRateDisplay.Location = new System.Drawing.Point(300, 524);
            this.ckRateDisplay.Margin = new System.Windows.Forms.Padding(6);
            this.ckRateDisplay.Name = "ckRateDisplay";
            this.ckRateDisplay.Size = new System.Drawing.Size(192, 36);
            this.ckRateDisplay.TabIndex = 334;
            this.ckRateDisplay.Text = "Rate Display";
            this.ckRateDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckRateDisplay.UseVisualStyleBackColor = true;
            this.ckRateDisplay.CheckedChanged += new System.EventHandler(this.ckLargeScreen_CheckedChanged);
            // 
            // ckMetric
            // 
            this.ckMetric.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckMetric.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckMetric.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckMetric.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckMetric.Location = new System.Drawing.Point(45, 524);
            this.ckMetric.Margin = new System.Windows.Forms.Padding(6);
            this.ckMetric.Name = "ckMetric";
            this.ckMetric.Size = new System.Drawing.Size(192, 36);
            this.ckMetric.TabIndex = 340;
            this.ckMetric.Text = "Metric Units";
            this.ckMetric.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckMetric.UseVisualStyleBackColor = true;
            this.ckMetric.CheckedChanged += new System.EventHandler(this.ckLargeScreen_CheckedChanged);
            // 
            // tbSimSpeed
            // 
            this.tbSimSpeed.Enabled = false;
            this.tbSimSpeed.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSimSpeed.Location = new System.Drawing.Point(216, 208);
            this.tbSimSpeed.Margin = new System.Windows.Forms.Padding(6);
            this.tbSimSpeed.MaxLength = 8;
            this.tbSimSpeed.Name = "tbSimSpeed";
            this.tbSimSpeed.Size = new System.Drawing.Size(78, 30);
            this.tbSimSpeed.TabIndex = 338;
            this.tbSimSpeed.Text = "0";
            this.tbSimSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSimSpeed.TextChanged += new System.EventHandler(this.ckLargeScreen_CheckedChanged);
            this.tbSimSpeed.Enter += new System.EventHandler(this.tbSimSpeed_Enter);
            this.tbSimSpeed.Validating += new System.ComponentModel.CancelEventHandler(this.tbSimSpeed_Validating);
            // 
            // lbSimUnits
            // 
            this.lbSimUnits.AutoSize = true;
            this.lbSimUnits.Enabled = false;
            this.lbSimUnits.ForeColor = System.Drawing.Color.DarkGreen;
            this.lbSimUnits.Location = new System.Drawing.Point(306, 211);
            this.lbSimUnits.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbSimUnits.Name = "lbSimUnits";
            this.lbSimUnits.Size = new System.Drawing.Size(48, 24);
            this.lbSimUnits.TabIndex = 339;
            this.lbSimUnits.Text = "mph";
            this.lbSimUnits.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnCal
            // 
            this.btnCal.BackColor = System.Drawing.Color.Transparent;
            this.btnCal.FlatAppearance.BorderSize = 0;
            this.btnCal.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkSlateBlue;
            this.btnCal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCal.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCal.Image = global::RateController.Properties.Resources.clock;
            this.btnCal.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnCal.Location = new System.Drawing.Point(298, 387);
            this.btnCal.Name = "btnCal";
            this.btnCal.Size = new System.Drawing.Size(192, 71);
            this.btnCal.TabIndex = 346;
            this.btnCal.Text = "Calibrate";
            this.btnCal.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCal.UseVisualStyleBackColor = false;
            this.btnCal.Click += new System.EventHandler(this.btnCal_Click);
            // 
            // lbPulses
            // 
            this.lbPulses.Enabled = false;
            this.lbPulses.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPulses.Location = new System.Drawing.Point(407, 365);
            this.lbPulses.Name = "lbPulses";
            this.lbPulses.Size = new System.Drawing.Size(100, 29);
            this.lbPulses.TabIndex = 345;
            this.lbPulses.Text = "(pulses/mile)";
            this.lbPulses.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // groupBox3
            // 
            this.groupBox3.Location = new System.Drawing.Point(31, 267);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(480, 3);
            this.groupBox3.TabIndex = 344;
            this.groupBox3.TabStop = false;
            // 
            // butUpdateModules
            // 
            this.butUpdateModules.BackColor = System.Drawing.Color.Transparent;
            this.butUpdateModules.FlatAppearance.BorderSize = 0;
            this.butUpdateModules.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkSlateBlue;
            this.butUpdateModules.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butUpdateModules.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.butUpdateModules.Image = global::RateController.Properties.Resources.UpArrow64;
            this.butUpdateModules.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.butUpdateModules.Location = new System.Drawing.Point(43, 387);
            this.butUpdateModules.Name = "butUpdateModules";
            this.butUpdateModules.Size = new System.Drawing.Size(192, 71);
            this.butUpdateModules.TabIndex = 342;
            this.butUpdateModules.Text = "Send to Module";
            this.butUpdateModules.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.butUpdateModules.UseVisualStyleBackColor = false;
            this.butUpdateModules.Click += new System.EventHandler(this.butUpdateModules_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(34, 108);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(480, 3);
            this.groupBox1.TabIndex = 342;
            this.groupBox1.TabStop = false;
            // 
            // rbSimulated
            // 
            this.rbSimulated.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbSimulated.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbSimulated.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbSimulated.Location = new System.Drawing.Point(178, 156);
            this.rbSimulated.Name = "rbSimulated";
            this.rbSimulated.Size = new System.Drawing.Size(187, 36);
            this.rbSimulated.TabIndex = 246;
            this.rbSimulated.Text = "Simulated Speed";
            this.rbSimulated.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbSimulated.UseVisualStyleBackColor = true;
            this.rbSimulated.CheckedChanged += new System.EventHandler(this.rbAOG_CheckedChanged);
            // 
            // rbWheel
            // 
            this.rbWheel.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbWheel.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbWheel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbWheel.Location = new System.Drawing.Point(178, 283);
            this.rbWheel.Name = "rbWheel";
            this.rbWheel.Size = new System.Drawing.Size(187, 36);
            this.rbWheel.TabIndex = 245;
            this.rbWheel.Text = "Wheel Sensor";
            this.rbWheel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbWheel.UseVisualStyleBackColor = true;
            this.rbWheel.CheckedChanged += new System.EventHandler(this.rbWheel_CheckedChanged);
            // 
            // rbAOG
            // 
            this.rbAOG.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbAOG.Checked = true;
            this.rbAOG.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbAOG.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbAOG.Location = new System.Drawing.Point(178, 65);
            this.rbAOG.Name = "rbAOG";
            this.rbAOG.Size = new System.Drawing.Size(187, 36);
            this.rbAOG.TabIndex = 244;
            this.rbAOG.TabStop = true;
            this.rbAOG.Text = "GPS";
            this.rbAOG.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbAOG.UseVisualStyleBackColor = true;
            this.rbAOG.CheckedChanged += new System.EventHandler(this.rbAOG_CheckedChanged);
            // 
            // tbWheelModule
            // 
            this.tbWheelModule.Enabled = false;
            this.tbWheelModule.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbWheelModule.Location = new System.Drawing.Point(111, 335);
            this.tbWheelModule.Name = "tbWheelModule";
            this.tbWheelModule.Size = new System.Drawing.Size(58, 29);
            this.tbWheelModule.TabIndex = 243;
            this.tbWheelModule.TabStop = false;
            this.tbWheelModule.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbWheelModule.TextChanged += new System.EventHandler(this.rbWheel_CheckedChanged);
            this.tbWheelModule.Enter += new System.EventHandler(this.tbWheelModule_Enter);
            this.tbWheelModule.Validating += new System.ComponentModel.CancelEventHandler(this.tbWheelModule_Validating);
            // 
            // lbModule
            // 
            this.lbModule.AutoSize = true;
            this.lbModule.Enabled = false;
            this.lbModule.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbModule.Location = new System.Drawing.Point(31, 337);
            this.lbModule.Name = "lbModule";
            this.lbModule.Size = new System.Drawing.Size(74, 24);
            this.lbModule.TabIndex = 242;
            this.lbModule.Text = "Module";
            // 
            // tbWheelCal
            // 
            this.tbWheelCal.Enabled = false;
            this.tbWheelCal.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbWheelCal.Location = new System.Drawing.Point(407, 335);
            this.tbWheelCal.Name = "tbWheelCal";
            this.tbWheelCal.Size = new System.Drawing.Size(100, 29);
            this.tbWheelCal.TabIndex = 240;
            this.tbWheelCal.TabStop = false;
            this.tbWheelCal.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbWheelCal.TextChanged += new System.EventHandler(this.rbWheel_CheckedChanged);
            this.tbWheelCal.Enter += new System.EventHandler(this.tbWheelCal_Enter);
            this.tbWheelCal.Validating += new System.ComponentModel.CancelEventHandler(this.tbWheelCal_Validating);
            // 
            // tbWheelPin
            // 
            this.tbWheelPin.Enabled = false;
            this.tbWheelPin.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbWheelPin.Location = new System.Drawing.Point(255, 335);
            this.tbWheelPin.Name = "tbWheelPin";
            this.tbWheelPin.Size = new System.Drawing.Size(58, 29);
            this.tbWheelPin.TabIndex = 239;
            this.tbWheelPin.TabStop = false;
            this.tbWheelPin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbWheelPin.TextChanged += new System.EventHandler(this.rbWheel_CheckedChanged);
            this.tbWheelPin.Enter += new System.EventHandler(this.tbWheelPin_Enter);
            this.tbWheelPin.Validating += new System.ComponentModel.CancelEventHandler(this.tbWheelPin_Validating);
            // 
            // lbPin
            // 
            this.lbPin.AutoSize = true;
            this.lbPin.Enabled = false;
            this.lbPin.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPin.Location = new System.Drawing.Point(212, 337);
            this.lbPin.Name = "lbPin";
            this.lbPin.Size = new System.Drawing.Size(37, 24);
            this.lbPin.TabIndex = 221;
            this.lbPin.Text = "Pin";
            // 
            // lbCal
            // 
            this.lbCal.AutoSize = true;
            this.lbCal.Enabled = false;
            this.lbCal.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCal.Location = new System.Drawing.Point(349, 337);
            this.lbCal.Name = "lbCal";
            this.lbCal.Size = new System.Drawing.Size(52, 24);
            this.lbCal.TabIndex = 223;
            this.lbCal.Text = "Cal #";
            // 
            // frmMenuOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 678);
            this.Controls.Add(this.ckMetric);
            this.Controls.Add(this.rbAOG);
            this.Controls.Add(this.btnCal);
            this.Controls.Add(this.ckRateDisplay);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lbPulses);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lbCal);
            this.Controls.Add(this.lbSimUnits);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.tbWheelModule);
            this.Controls.Add(this.lbPin);
            this.Controls.Add(this.rbWheel);
            this.Controls.Add(this.butUpdateModules);
            this.Controls.Add(this.lbModule);
            this.Controls.Add(this.tbWheelPin);
            this.Controls.Add(this.tbSimSpeed);
            this.Controls.Add(this.rbSimulated);
            this.Controls.Add(this.tbWheelCal);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "frmMenuOptions";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuDisplay";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMenuDisplay_FormClosed);
            this.Load += new System.EventHandler(this.frmMenuDisplay_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox ckMetric;
        private System.Windows.Forms.TextBox tbSimSpeed;
        private System.Windows.Forms.Label lbSimUnits;
        private System.Windows.Forms.CheckBox ckRateDisplay;
        private System.Windows.Forms.TextBox tbWheelCal;
        private System.Windows.Forms.TextBox tbWheelPin;
        private System.Windows.Forms.Label lbPin;
        private System.Windows.Forms.Label lbCal;
        private System.Windows.Forms.RadioButton rbSimulated;
        private System.Windows.Forms.RadioButton rbWheel;
        private System.Windows.Forms.RadioButton rbAOG;
        private System.Windows.Forms.TextBox tbWheelModule;
        private System.Windows.Forms.Label lbModule;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button butUpdateModules;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lbPulses;
        private System.Windows.Forms.Button btnCal;
    }
}