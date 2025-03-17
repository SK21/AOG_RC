namespace RateController.Forms
{
    partial class frmRates
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRates));
            this.rbTarget = new System.Windows.Forms.RadioButton();
            this.rbApplied = new System.Windows.Forms.RadioButton();
            this.rbProductD = new System.Windows.Forms.RadioButton();
            this.rbProductC = new System.Windows.Forms.RadioButton();
            this.rbProductB = new System.Windows.Forms.RadioButton();
            this.rbProductA = new System.Windows.Forms.RadioButton();
            this.gbMap = new System.Windows.Forms.GroupBox();
            this.lbSensorCounts = new System.Windows.Forms.Label();
            this.HSrefresh = new System.Windows.Forms.HScrollBar();
            this.HSresolution = new System.Windows.Forms.HScrollBar();
            this.lbResolution = new System.Windows.Forms.Label();
            this.lbRefresh = new System.Windows.Forms.Label();
            this.lbResolutionDescription = new System.Windows.Forms.Label();
            this.butClose = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ckDisplayRates = new System.Windows.Forms.CheckBox();
            this.ckRecord = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.gbMap.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rbTarget
            // 
            this.rbTarget.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbTarget.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbTarget.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbTarget.Location = new System.Drawing.Point(110, 47);
            this.rbTarget.Name = "rbTarget";
            this.rbTarget.Size = new System.Drawing.Size(89, 64);
            this.rbTarget.TabIndex = 357;
            this.rbTarget.Text = "Target";
            this.rbTarget.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbTarget.UseVisualStyleBackColor = true;
            // 
            // rbApplied
            // 
            this.rbApplied.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbApplied.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbApplied.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbApplied.Location = new System.Drawing.Point(6, 47);
            this.rbApplied.Name = "rbApplied";
            this.rbApplied.Size = new System.Drawing.Size(89, 64);
            this.rbApplied.TabIndex = 358;
            this.rbApplied.Text = "Applied";
            this.rbApplied.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbApplied.UseVisualStyleBackColor = true;
            // 
            // rbProductD
            // 
            this.rbProductD.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbProductD.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbProductD.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbProductD.Location = new System.Drawing.Point(269, 38);
            this.rbProductD.Name = "rbProductD";
            this.rbProductD.Size = new System.Drawing.Size(66, 40);
            this.rbProductD.TabIndex = 360;
            this.rbProductD.Text = "D";
            this.rbProductD.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbProductD.UseVisualStyleBackColor = true;
            // 
            // rbProductC
            // 
            this.rbProductC.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbProductC.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbProductC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbProductC.Location = new System.Drawing.Point(185, 38);
            this.rbProductC.Name = "rbProductC";
            this.rbProductC.Size = new System.Drawing.Size(66, 40);
            this.rbProductC.TabIndex = 361;
            this.rbProductC.Text = "C";
            this.rbProductC.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbProductC.UseVisualStyleBackColor = true;
            // 
            // rbProductB
            // 
            this.rbProductB.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbProductB.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbProductB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbProductB.Location = new System.Drawing.Point(101, 38);
            this.rbProductB.Name = "rbProductB";
            this.rbProductB.Size = new System.Drawing.Size(66, 40);
            this.rbProductB.TabIndex = 362;
            this.rbProductB.Text = "B";
            this.rbProductB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbProductB.UseVisualStyleBackColor = true;
            // 
            // rbProductA
            // 
            this.rbProductA.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbProductA.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbProductA.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbProductA.Location = new System.Drawing.Point(17, 38);
            this.rbProductA.Name = "rbProductA";
            this.rbProductA.Size = new System.Drawing.Size(66, 40);
            this.rbProductA.TabIndex = 363;
            this.rbProductA.Text = "A";
            this.rbProductA.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbProductA.UseVisualStyleBackColor = true;
            this.rbProductA.CheckedChanged += new System.EventHandler(this.rbProductA_CheckedChanged);
            // 
            // gbMap
            // 
            this.gbMap.Controls.Add(this.rbProductA);
            this.gbMap.Controls.Add(this.rbProductD);
            this.gbMap.Controls.Add(this.rbProductC);
            this.gbMap.Controls.Add(this.rbProductB);
            this.gbMap.Location = new System.Drawing.Point(214, 28);
            this.gbMap.Name = "gbMap";
            this.gbMap.Size = new System.Drawing.Size(352, 103);
            this.gbMap.TabIndex = 364;
            this.gbMap.TabStop = false;
            this.gbMap.Text = "Product to Display";
            // 
            // lbSensorCounts
            // 
            this.lbSensorCounts.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSensorCounts.Location = new System.Drawing.Point(11, 155);
            this.lbSensorCounts.Name = "lbSensorCounts";
            this.lbSensorCounts.Size = new System.Drawing.Size(236, 23);
            this.lbSensorCounts.TabIndex = 366;
            this.lbSensorCounts.Text = "Refresh Interval (Seconds)";
            this.lbSensorCounts.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // HSrefresh
            // 
            this.HSrefresh.LargeChange = 1;
            this.HSrefresh.Location = new System.Drawing.Point(265, 144);
            this.HSrefresh.Name = "HSrefresh";
            this.HSrefresh.Size = new System.Drawing.Size(239, 45);
            this.HSrefresh.TabIndex = 375;
            // 
            // HSresolution
            // 
            this.HSresolution.LargeChange = 1;
            this.HSresolution.Location = new System.Drawing.Point(263, 204);
            this.HSresolution.Name = "HSresolution";
            this.HSresolution.Size = new System.Drawing.Size(241, 45);
            this.HSresolution.TabIndex = 374;
            this.HSresolution.Scroll += new System.Windows.Forms.ScrollEventHandler(this.HSlow_Scroll);
            // 
            // lbResolution
            // 
            this.lbResolution.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbResolution.Location = new System.Drawing.Point(507, 216);
            this.lbResolution.Name = "lbResolution";
            this.lbResolution.Size = new System.Drawing.Size(59, 23);
            this.lbResolution.TabIndex = 373;
            this.lbResolution.Text = "100";
            this.lbResolution.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbRefresh
            // 
            this.lbRefresh.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRefresh.Location = new System.Drawing.Point(507, 155);
            this.lbRefresh.Name = "lbRefresh";
            this.lbRefresh.Size = new System.Drawing.Size(59, 23);
            this.lbRefresh.TabIndex = 372;
            this.lbRefresh.Text = "100";
            this.lbRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lbRefresh.Click += new System.EventHandler(this.lbHigh_Click);
            // 
            // lbResolutionDescription
            // 
            this.lbResolutionDescription.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbResolutionDescription.Location = new System.Drawing.Point(11, 216);
            this.lbResolutionDescription.Name = "lbResolutionDescription";
            this.lbResolutionDescription.Size = new System.Drawing.Size(236, 23);
            this.lbResolutionDescription.TabIndex = 371;
            this.lbResolutionDescription.Text = "Resolution (Acres)";
            this.lbResolutionDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // butClose
            // 
            this.butClose.BackColor = System.Drawing.Color.Transparent;
            this.butClose.FlatAppearance.BorderSize = 0;
            this.butClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkSlateBlue;
            this.butClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.butClose.Image = ((System.Drawing.Image)(resources.GetObject("butClose.Image")));
            this.butClose.Location = new System.Drawing.Point(512, 292);
            this.butClose.Name = "butClose";
            this.butClose.Size = new System.Drawing.Size(89, 64);
            this.butClose.TabIndex = 377;
            this.butClose.UseVisualStyleBackColor = false;
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
            this.btnCancel.Location = new System.Drawing.Point(322, 292);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(89, 64);
            this.btnCancel.TabIndex = 378;
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.HSrefresh);
            this.groupBox1.Controls.Add(this.lbSensorCounts);
            this.groupBox1.Controls.Add(this.gbMap);
            this.groupBox1.Controls.Add(this.rbApplied);
            this.groupBox1.Controls.Add(this.lbResolutionDescription);
            this.groupBox1.Controls.Add(this.rbTarget);
            this.groupBox1.Controls.Add(this.HSresolution);
            this.groupBox1.Controls.Add(this.lbRefresh);
            this.groupBox1.Controls.Add(this.lbResolution);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(585, 266);
            this.groupBox1.TabIndex = 380;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Rate Display";
            // 
            // ckDisplayRates
            // 
            this.ckDisplayRates.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckDisplayRates.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckDisplayRates.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckDisplayRates.Location = new System.Drawing.Point(175, 292);
            this.ckDisplayRates.Name = "ckDisplayRates";
            this.ckDisplayRates.Size = new System.Drawing.Size(89, 64);
            this.ckDisplayRates.TabIndex = 381;
            this.ckDisplayRates.Text = "Display Rates";
            this.ckDisplayRates.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckDisplayRates.UseVisualStyleBackColor = true;
            // 
            // ckRecord
            // 
            this.ckRecord.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckRecord.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckRecord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckRecord.Location = new System.Drawing.Point(69, 292);
            this.ckRecord.Name = "ckRecord";
            this.ckRecord.Size = new System.Drawing.Size(89, 64);
            this.ckRecord.TabIndex = 382;
            this.ckRecord.Text = "Record Data";
            this.ckRecord.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckRecord.UseVisualStyleBackColor = true;
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
            this.btnOK.Location = new System.Drawing.Point(417, 292);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(89, 64);
            this.btnOK.TabIndex = 383;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            // 
            // frmRates
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(609, 368);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.ckRecord);
            this.Controls.Add(this.ckDisplayRates);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.butClose);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmRates";
            this.ShowInTaskbar = false;
            this.Text = "Rate Data";
            this.gbMap.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.RadioButton rbTarget;
        private System.Windows.Forms.RadioButton rbApplied;
        private System.Windows.Forms.RadioButton rbProductD;
        private System.Windows.Forms.RadioButton rbProductC;
        private System.Windows.Forms.RadioButton rbProductB;
        private System.Windows.Forms.RadioButton rbProductA;
        private System.Windows.Forms.GroupBox gbMap;
        private System.Windows.Forms.Label lbSensorCounts;
        private System.Windows.Forms.HScrollBar HSrefresh;
        private System.Windows.Forms.HScrollBar HSresolution;
        private System.Windows.Forms.Label lbResolution;
        private System.Windows.Forms.Label lbRefresh;
        private System.Windows.Forms.Label lbResolutionDescription;
        private System.Windows.Forms.Button butClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox ckDisplayRates;
        private System.Windows.Forms.CheckBox ckRecord;
        private System.Windows.Forms.Button btnOK;
    }
}