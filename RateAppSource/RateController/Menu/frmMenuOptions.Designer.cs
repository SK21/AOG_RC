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
            this.ckLargeScreen = new System.Windows.Forms.CheckBox();
            this.ckTransparent = new System.Windows.Forms.CheckBox();
            this.ckMetric = new System.Windows.Forms.CheckBox();
            this.tbSimSpeed = new System.Windows.Forms.TextBox();
            this.lbSimUnits = new System.Windows.Forms.Label();
            this.gbNetwork = new System.Windows.Forms.GroupBox();
            this.tbWheelCal = new System.Windows.Forms.TextBox();
            this.tbWheelPin = new System.Windows.Forms.TextBox();
            this.lbSubnet = new System.Windows.Forms.Label();
            this.lbIP = new System.Windows.Forms.Label();
            this.tbWheelModule = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.rbAOG = new System.Windows.Forms.RadioButton();
            this.rbWheel = new System.Windows.Forms.RadioButton();
            this.rbSimulated = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.gbNetwork.SuspendLayout();
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
            this.ckRateDisplay.Location = new System.Drawing.Point(290, 471);
            this.ckRateDisplay.Margin = new System.Windows.Forms.Padding(6);
            this.ckRateDisplay.Name = "ckRateDisplay";
            this.ckRateDisplay.Size = new System.Drawing.Size(192, 36);
            this.ckRateDisplay.TabIndex = 334;
            this.ckRateDisplay.Text = "Rate Display";
            this.ckRateDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckRateDisplay.UseVisualStyleBackColor = true;
            this.ckRateDisplay.CheckedChanged += new System.EventHandler(this.ckLargeScreen_CheckedChanged);
            // 
            // ckLargeScreen
            // 
            this.ckLargeScreen.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckLargeScreen.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckLargeScreen.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckLargeScreen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckLargeScreen.Location = new System.Drawing.Point(35, 546);
            this.ckLargeScreen.Margin = new System.Windows.Forms.Padding(6);
            this.ckLargeScreen.Name = "ckLargeScreen";
            this.ckLargeScreen.Size = new System.Drawing.Size(192, 36);
            this.ckLargeScreen.TabIndex = 127;
            this.ckLargeScreen.Text = "Large Screen";
            this.ckLargeScreen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckLargeScreen.UseVisualStyleBackColor = true;
            this.ckLargeScreen.CheckedChanged += new System.EventHandler(this.ckLargeScreen_CheckedChanged);
            // 
            // ckTransparent
            // 
            this.ckTransparent.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckTransparent.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckTransparent.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckTransparent.Location = new System.Drawing.Point(290, 546);
            this.ckTransparent.Margin = new System.Windows.Forms.Padding(6);
            this.ckTransparent.Name = "ckTransparent";
            this.ckTransparent.Size = new System.Drawing.Size(192, 36);
            this.ckTransparent.TabIndex = 119;
            this.ckTransparent.Text = "Transparent";
            this.ckTransparent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckTransparent.UseVisualStyleBackColor = true;
            this.ckTransparent.CheckedChanged += new System.EventHandler(this.ckLargeScreen_CheckedChanged);
            // 
            // ckMetric
            // 
            this.ckMetric.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckMetric.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckMetric.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckMetric.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckMetric.Location = new System.Drawing.Point(35, 471);
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
            this.tbSimSpeed.Location = new System.Drawing.Point(63, 346);
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
            this.lbSimUnits.Location = new System.Drawing.Point(153, 349);
            this.lbSimUnits.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbSimUnits.Name = "lbSimUnits";
            this.lbSimUnits.Size = new System.Drawing.Size(48, 24);
            this.lbSimUnits.TabIndex = 339;
            this.lbSimUnits.Text = "mph";
            this.lbSimUnits.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gbNetwork
            // 
            this.gbNetwork.Controls.Add(this.groupBox2);
            this.gbNetwork.Controls.Add(this.groupBox1);
            this.gbNetwork.Controls.Add(this.rbSimulated);
            this.gbNetwork.Controls.Add(this.tbSimSpeed);
            this.gbNetwork.Controls.Add(this.rbWheel);
            this.gbNetwork.Controls.Add(this.rbAOG);
            this.gbNetwork.Controls.Add(this.lbSimUnits);
            this.gbNetwork.Controls.Add(this.tbWheelModule);
            this.gbNetwork.Controls.Add(this.label1);
            this.gbNetwork.Controls.Add(this.tbWheelCal);
            this.gbNetwork.Controls.Add(this.tbWheelPin);
            this.gbNetwork.Controls.Add(this.lbSubnet);
            this.gbNetwork.Controls.Add(this.lbIP);
            this.gbNetwork.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbNetwork.Location = new System.Drawing.Point(122, 34);
            this.gbNetwork.Name = "gbNetwork";
            this.gbNetwork.Size = new System.Drawing.Size(264, 392);
            this.gbNetwork.TabIndex = 341;
            this.gbNetwork.TabStop = false;
            this.gbNetwork.Text = "Speed Source";
            this.gbNetwork.Paint += new System.Windows.Forms.PaintEventHandler(this.gbNetwork_Paint);
            // 
            // tbWheelCal
            // 
            this.tbWheelCal.Enabled = false;
            this.tbWheelCal.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbWheelCal.Location = new System.Drawing.Point(137, 234);
            this.tbWheelCal.Name = "tbWheelCal";
            this.tbWheelCal.Size = new System.Drawing.Size(100, 29);
            this.tbWheelCal.TabIndex = 240;
            this.tbWheelCal.TabStop = false;
            this.tbWheelCal.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbWheelCal.TextChanged += new System.EventHandler(this.ckLargeScreen_CheckedChanged);
            this.tbWheelCal.Enter += new System.EventHandler(this.tbWheelCal_Enter);
            this.tbWheelCal.Validating += new System.ComponentModel.CancelEventHandler(this.tbWheelCal_Validating);
            // 
            // tbWheelPin
            // 
            this.tbWheelPin.Enabled = false;
            this.tbWheelPin.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbWheelPin.Location = new System.Drawing.Point(179, 187);
            this.tbWheelPin.Name = "tbWheelPin";
            this.tbWheelPin.Size = new System.Drawing.Size(58, 29);
            this.tbWheelPin.TabIndex = 239;
            this.tbWheelPin.TabStop = false;
            this.tbWheelPin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbWheelPin.TextChanged += new System.EventHandler(this.ckLargeScreen_CheckedChanged);
            this.tbWheelPin.Enter += new System.EventHandler(this.tbWheelPin_Enter);
            this.tbWheelPin.Validating += new System.ComponentModel.CancelEventHandler(this.tbWheelPin_Validating);
            // 
            // lbSubnet
            // 
            this.lbSubnet.AutoSize = true;
            this.lbSubnet.Enabled = false;
            this.lbSubnet.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSubnet.Location = new System.Drawing.Point(44, 189);
            this.lbSubnet.Name = "lbSubnet";
            this.lbSubnet.Size = new System.Drawing.Size(37, 24);
            this.lbSubnet.TabIndex = 221;
            this.lbSubnet.Text = "Pin";
            // 
            // lbIP
            // 
            this.lbIP.AutoSize = true;
            this.lbIP.Enabled = false;
            this.lbIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbIP.Location = new System.Drawing.Point(44, 236);
            this.lbIP.Name = "lbIP";
            this.lbIP.Size = new System.Drawing.Size(52, 24);
            this.lbIP.TabIndex = 223;
            this.lbIP.Text = "Cal #";
            // 
            // tbWheelModule
            // 
            this.tbWheelModule.Enabled = false;
            this.tbWheelModule.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbWheelModule.Location = new System.Drawing.Point(179, 140);
            this.tbWheelModule.Name = "tbWheelModule";
            this.tbWheelModule.Size = new System.Drawing.Size(58, 29);
            this.tbWheelModule.TabIndex = 243;
            this.tbWheelModule.TabStop = false;
            this.tbWheelModule.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbWheelModule.TextChanged += new System.EventHandler(this.ckLargeScreen_CheckedChanged);
            this.tbWheelModule.Enter += new System.EventHandler(this.tbWheelModule_Enter);
            this.tbWheelModule.Validating += new System.ComponentModel.CancelEventHandler(this.tbWheelModule_Validating);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Enabled = false;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(44, 142);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 24);
            this.label1.TabIndex = 242;
            this.label1.Text = "Module";
            // 
            // rbAOG
            // 
            this.rbAOG.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbAOG.Checked = true;
            this.rbAOG.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbAOG.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbAOG.Location = new System.Drawing.Point(40, 33);
            this.rbAOG.Name = "rbAOG";
            this.rbAOG.Size = new System.Drawing.Size(187, 36);
            this.rbAOG.TabIndex = 244;
            this.rbAOG.TabStop = true;
            this.rbAOG.Text = "GPS";
            this.rbAOG.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbAOG.UseVisualStyleBackColor = true;
            this.rbAOG.CheckedChanged += new System.EventHandler(this.rbAOG_CheckedChanged);
            // 
            // rbWheel
            // 
            this.rbWheel.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbWheel.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbWheel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbWheel.Location = new System.Drawing.Point(40, 92);
            this.rbWheel.Name = "rbWheel";
            this.rbWheel.Size = new System.Drawing.Size(187, 36);
            this.rbWheel.TabIndex = 245;
            this.rbWheel.Text = "Wheel Sensor";
            this.rbWheel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbWheel.UseVisualStyleBackColor = true;
            this.rbWheel.CheckedChanged += new System.EventHandler(this.rbAOG_CheckedChanged);
            // 
            // rbSimulated
            // 
            this.rbSimulated.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbSimulated.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbSimulated.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbSimulated.Location = new System.Drawing.Point(40, 295);
            this.rbSimulated.Name = "rbSimulated";
            this.rbSimulated.Size = new System.Drawing.Size(187, 36);
            this.rbSimulated.TabIndex = 246;
            this.rbSimulated.Text = "Simulated Speed";
            this.rbSimulated.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbSimulated.UseVisualStyleBackColor = true;
            this.rbSimulated.CheckedChanged += new System.EventHandler(this.rbAOG_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(18, 79);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(230, 3);
            this.groupBox1.TabIndex = 342;
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Location = new System.Drawing.Point(18, 280);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(230, 3);
            this.groupBox2.TabIndex = 343;
            this.groupBox2.TabStop = false;
            // 
            // frmMenuOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 678);
            this.Controls.Add(this.gbNetwork);
            this.Controls.Add(this.ckMetric);
            this.Controls.Add(this.ckRateDisplay);
            this.Controls.Add(this.ckLargeScreen);
            this.Controls.Add(this.ckTransparent);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "frmMenuOptions";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuDisplay";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMenuDisplay_FormClosed);
            this.Load += new System.EventHandler(this.frmMenuDisplay_Load);
            this.gbNetwork.ResumeLayout(false);
            this.gbNetwork.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox ckLargeScreen;
        private System.Windows.Forms.CheckBox ckTransparent;
        private System.Windows.Forms.CheckBox ckMetric;
        private System.Windows.Forms.TextBox tbSimSpeed;
        private System.Windows.Forms.Label lbSimUnits;
        private System.Windows.Forms.CheckBox ckRateDisplay;
        private System.Windows.Forms.GroupBox gbNetwork;
        private System.Windows.Forms.TextBox tbWheelCal;
        private System.Windows.Forms.TextBox tbWheelPin;
        private System.Windows.Forms.Label lbSubnet;
        private System.Windows.Forms.Label lbIP;
        private System.Windows.Forms.RadioButton rbSimulated;
        private System.Windows.Forms.RadioButton rbWheel;
        private System.Windows.Forms.RadioButton rbAOG;
        private System.Windows.Forms.TextBox tbWheelModule;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}