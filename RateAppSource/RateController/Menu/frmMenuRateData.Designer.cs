namespace RateController.Forms
{
    partial class frmMenuRateData
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMenuRateData));
            this.rbTarget = new System.Windows.Forms.RadioButton();
            this.rbApplied = new System.Windows.Forms.RadioButton();
            this.rbProductD = new System.Windows.Forms.RadioButton();
            this.rbProductC = new System.Windows.Forms.RadioButton();
            this.rbProductB = new System.Windows.Forms.RadioButton();
            this.rbProductA = new System.Windows.Forms.RadioButton();
            this.gbMap = new System.Windows.Forms.GroupBox();
            this.lbSensorCounts = new System.Windows.Forms.Label();
            this.HSrefreshMap = new System.Windows.Forms.HScrollBar();
            this.HSresolution = new System.Windows.Forms.HScrollBar();
            this.lbResolution = new System.Windows.Forms.Label();
            this.lbRefresh = new System.Windows.Forms.Label();
            this.lbResolutionDescription = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbCoverage = new System.Windows.Forms.GroupBox();
            this.ckRecord = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.gbRecord = new System.Windows.Forms.GroupBox();
            this.HSRecordInterval = new System.Windows.Forms.HScrollBar();
            this.label2 = new System.Windows.Forms.Label();
            this.lbRecordInterval = new System.Windows.Forms.Label();
            this.tbRateDataFile = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.lbFileName = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.gbMap.SuspendLayout();
            this.gbCoverage.SuspendLayout();
            this.gbRecord.SuspendLayout();
            this.SuspendLayout();
            // 
            // rbTarget
            // 
            this.rbTarget.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbTarget.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbTarget.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbTarget.Location = new System.Drawing.Point(273, 40);
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
            this.rbApplied.Checked = true;
            this.rbApplied.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbApplied.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbApplied.Location = new System.Drawing.Point(134, 40);
            this.rbApplied.Name = "rbApplied";
            this.rbApplied.Size = new System.Drawing.Size(89, 64);
            this.rbApplied.TabIndex = 358;
            this.rbApplied.TabStop = true;
            this.rbApplied.Text = "Applied";
            this.rbApplied.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbApplied.UseVisualStyleBackColor = true;
            this.rbApplied.CheckedChanged += new System.EventHandler(this.rbProductA_CheckedChanged);
            // 
            // rbProductD
            // 
            this.rbProductD.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbProductD.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbProductD.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbProductD.Location = new System.Drawing.Point(293, 38);
            this.rbProductD.Name = "rbProductD";
            this.rbProductD.Size = new System.Drawing.Size(66, 40);
            this.rbProductD.TabIndex = 360;
            this.rbProductD.Text = "D";
            this.rbProductD.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbProductD.UseVisualStyleBackColor = true;
            this.rbProductD.CheckedChanged += new System.EventHandler(this.rbProductA_CheckedChanged);
            // 
            // rbProductC
            // 
            this.rbProductC.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbProductC.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbProductC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbProductC.Location = new System.Drawing.Point(201, 38);
            this.rbProductC.Name = "rbProductC";
            this.rbProductC.Size = new System.Drawing.Size(66, 40);
            this.rbProductC.TabIndex = 361;
            this.rbProductC.Text = "C";
            this.rbProductC.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbProductC.UseVisualStyleBackColor = true;
            this.rbProductC.CheckedChanged += new System.EventHandler(this.rbProductA_CheckedChanged);
            // 
            // rbProductB
            // 
            this.rbProductB.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbProductB.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbProductB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbProductB.Location = new System.Drawing.Point(109, 38);
            this.rbProductB.Name = "rbProductB";
            this.rbProductB.Size = new System.Drawing.Size(66, 40);
            this.rbProductB.TabIndex = 362;
            this.rbProductB.Text = "B";
            this.rbProductB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbProductB.UseVisualStyleBackColor = true;
            this.rbProductB.CheckedChanged += new System.EventHandler(this.rbProductA_CheckedChanged);
            // 
            // rbProductA
            // 
            this.rbProductA.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbProductA.Checked = true;
            this.rbProductA.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbProductA.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbProductA.Location = new System.Drawing.Point(17, 38);
            this.rbProductA.Name = "rbProductA";
            this.rbProductA.Size = new System.Drawing.Size(66, 40);
            this.rbProductA.TabIndex = 363;
            this.rbProductA.TabStop = true;
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
            this.gbMap.Location = new System.Drawing.Point(62, 110);
            this.gbMap.Name = "gbMap";
            this.gbMap.Size = new System.Drawing.Size(367, 103);
            this.gbMap.TabIndex = 364;
            this.gbMap.TabStop = false;
            this.gbMap.Text = "Product to Display";
            this.gbMap.Paint += new System.Windows.Forms.PaintEventHandler(this.gbMap_Paint);
            // 
            // lbSensorCounts
            // 
            this.lbSensorCounts.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSensorCounts.Location = new System.Drawing.Point(13, 227);
            this.lbSensorCounts.Name = "lbSensorCounts";
            this.lbSensorCounts.Size = new System.Drawing.Size(170, 23);
            this.lbSensorCounts.TabIndex = 366;
            this.lbSensorCounts.Text = "Interval (Seconds)";
            this.lbSensorCounts.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // HSrefreshMap
            // 
            this.HSrefreshMap.LargeChange = 1;
            this.HSrefreshMap.Location = new System.Drawing.Point(190, 216);
            this.HSrefreshMap.Maximum = 300;
            this.HSrefreshMap.Minimum = 15;
            this.HSrefreshMap.Name = "HSrefreshMap";
            this.HSrefreshMap.Size = new System.Drawing.Size(239, 45);
            this.HSrefreshMap.TabIndex = 375;
            this.HSrefreshMap.Value = 15;
            this.HSrefreshMap.Scroll += new System.Windows.Forms.ScrollEventHandler(this.HSresolution_Scroll);
            // 
            // HSresolution
            // 
            this.HSresolution.LargeChange = 1;
            this.HSresolution.Location = new System.Drawing.Point(190, 271);
            this.HSresolution.Minimum = 1;
            this.HSresolution.Name = "HSresolution";
            this.HSresolution.Size = new System.Drawing.Size(241, 45);
            this.HSresolution.TabIndex = 374;
            this.HSresolution.Value = 1;
            this.HSresolution.Scroll += new System.Windows.Forms.ScrollEventHandler(this.HSresolution_Scroll);
            // 
            // lbResolution
            // 
            this.lbResolution.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbResolution.Location = new System.Drawing.Point(436, 282);
            this.lbResolution.Name = "lbResolution";
            this.lbResolution.Size = new System.Drawing.Size(59, 23);
            this.lbResolution.TabIndex = 373;
            this.lbResolution.Text = "100";
            this.lbResolution.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbRefresh
            // 
            this.lbRefresh.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRefresh.Location = new System.Drawing.Point(436, 227);
            this.lbRefresh.Name = "lbRefresh";
            this.lbRefresh.Size = new System.Drawing.Size(59, 23);
            this.lbRefresh.TabIndex = 372;
            this.lbRefresh.Text = "100";
            this.lbRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbResolutionDescription
            // 
            this.lbResolutionDescription.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbResolutionDescription.Location = new System.Drawing.Point(13, 282);
            this.lbResolutionDescription.Name = "lbResolutionDescription";
            this.lbResolutionDescription.Size = new System.Drawing.Size(170, 23);
            this.lbResolutionDescription.TabIndex = 371;
            this.lbResolutionDescription.Text = "Resolution (Acres)";
            this.lbResolutionDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.btnCancel.Location = new System.Drawing.Point(344, 554);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(89, 64);
            this.btnCancel.TabIndex = 378;
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // gbCoverage
            // 
            this.gbCoverage.Controls.Add(this.HSrefreshMap);
            this.gbCoverage.Controls.Add(this.lbSensorCounts);
            this.gbCoverage.Controls.Add(this.gbMap);
            this.gbCoverage.Controls.Add(this.rbApplied);
            this.gbCoverage.Controls.Add(this.lbResolutionDescription);
            this.gbCoverage.Controls.Add(this.rbTarget);
            this.gbCoverage.Controls.Add(this.HSresolution);
            this.gbCoverage.Controls.Add(this.lbRefresh);
            this.gbCoverage.Controls.Add(this.lbResolution);
            this.gbCoverage.Location = new System.Drawing.Point(12, 217);
            this.gbCoverage.Name = "gbCoverage";
            this.gbCoverage.Size = new System.Drawing.Size(516, 331);
            this.gbCoverage.TabIndex = 380;
            this.gbCoverage.TabStop = false;
            this.gbCoverage.Text = "Coverage Map";
            this.gbCoverage.Paint += new System.Windows.Forms.PaintEventHandler(this.gbMap_Paint);
            // 
            // ckRecord
            // 
            this.ckRecord.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckRecord.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckRecord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckRecord.Image = global::RateController.Properties.Resources.Start;
            this.ckRecord.Location = new System.Drawing.Point(62, 131);
            this.ckRecord.Name = "ckRecord";
            this.ckRecord.Size = new System.Drawing.Size(89, 64);
            this.ckRecord.TabIndex = 382;
            this.ckRecord.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckRecord.UseVisualStyleBackColor = true;
            this.ckRecord.CheckedChanged += new System.EventHandler(this.rbProductA_CheckedChanged);
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
            this.btnOK.Location = new System.Drawing.Point(439, 554);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(89, 64);
            this.btnOK.TabIndex = 383;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // gbRecord
            // 
            this.gbRecord.Controls.Add(this.HSRecordInterval);
            this.gbRecord.Controls.Add(this.label2);
            this.gbRecord.Controls.Add(this.lbRecordInterval);
            this.gbRecord.Controls.Add(this.tbRateDataFile);
            this.gbRecord.Controls.Add(this.btnSave);
            this.gbRecord.Controls.Add(this.btnLoad);
            this.gbRecord.Controls.Add(this.btnNew);
            this.gbRecord.Controls.Add(this.lbFileName);
            this.gbRecord.Controls.Add(this.ckRecord);
            this.gbRecord.Location = new System.Drawing.Point(12, 12);
            this.gbRecord.Name = "gbRecord";
            this.gbRecord.Size = new System.Drawing.Size(516, 199);
            this.gbRecord.TabIndex = 384;
            this.gbRecord.TabStop = false;
            this.gbRecord.Text = "Record Rate Data";
            this.gbRecord.Paint += new System.Windows.Forms.PaintEventHandler(this.gbMap_Paint);
            // 
            // HSRecordInterval
            // 
            this.HSRecordInterval.LargeChange = 1;
            this.HSRecordInterval.Location = new System.Drawing.Point(190, 66);
            this.HSRecordInterval.Maximum = 300;
            this.HSRecordInterval.Minimum = 15;
            this.HSRecordInterval.Name = "HSRecordInterval";
            this.HSRecordInterval.Size = new System.Drawing.Size(239, 45);
            this.HSRecordInterval.TabIndex = 393;
            this.HSRecordInterval.Value = 15;
            this.HSRecordInterval.Scroll += new System.Windows.Forms.ScrollEventHandler(this.HSresolution_Scroll);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(13, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(170, 23);
            this.label2.TabIndex = 391;
            this.label2.Text = "Interval (Seconds)";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbRecordInterval
            // 
            this.lbRecordInterval.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRecordInterval.Location = new System.Drawing.Point(436, 77);
            this.lbRecordInterval.Name = "lbRecordInterval";
            this.lbRecordInterval.Size = new System.Drawing.Size(59, 23);
            this.lbRecordInterval.TabIndex = 392;
            this.lbRecordInterval.Text = "100";
            this.lbRecordInterval.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbRateDataFile
            // 
            this.tbRateDataFile.Location = new System.Drawing.Point(176, 28);
            this.tbRateDataFile.Name = "tbRateDataFile";
            this.tbRateDataFile.Size = new System.Drawing.Size(255, 29);
            this.tbRateDataFile.TabIndex = 390;
            this.tbRateDataFile.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnSave.Image = global::RateController.Properties.Resources.Save;
            this.btnSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSave.Location = new System.Drawing.Point(354, 133);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(82, 60);
            this.btnSave.TabIndex = 389;
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.FlatAppearance.BorderSize = 0;
            this.btnLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoad.Image = global::RateController.Properties.Resources.VehFileLoad;
            this.btnLoad.Location = new System.Drawing.Point(259, 133);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(82, 60);
            this.btnLoad.TabIndex = 388;
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnNew
            // 
            this.btnNew.FlatAppearance.BorderSize = 0;
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Image = global::RateController.Properties.Resources.NewFile;
            this.btnNew.Location = new System.Drawing.Point(164, 133);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(82, 60);
            this.btnNew.TabIndex = 387;
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // lbFileName
            // 
            this.lbFileName.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbFileName.Location = new System.Drawing.Point(69, 31);
            this.lbFileName.Name = "lbFileName";
            this.lbFileName.Size = new System.Drawing.Size(101, 23);
            this.lbFileName.TabIndex = 383;
            this.lbFileName.Text = "File Name";
            this.lbFileName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "CSV|*.CSV";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "CSV|*.CSV";
            // 
            // frmMenuRateData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 630);
            this.Controls.Add(this.gbRecord);
            this.Controls.Add(this.gbCoverage);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMenuRateData";
            this.ShowInTaskbar = false;
            this.Text = "Rate Data";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmRates_FormClosed);
            this.Load += new System.EventHandler(this.frmRates_Load);
            this.gbMap.ResumeLayout(false);
            this.gbCoverage.ResumeLayout(false);
            this.gbRecord.ResumeLayout(false);
            this.gbRecord.PerformLayout();
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
        private System.Windows.Forms.HScrollBar HSrefreshMap;
        private System.Windows.Forms.HScrollBar HSresolution;
        private System.Windows.Forms.Label lbResolution;
        private System.Windows.Forms.Label lbRefresh;
        private System.Windows.Forms.Label lbResolutionDescription;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox gbCoverage;
        private System.Windows.Forms.CheckBox ckRecord;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox gbRecord;
        private System.Windows.Forms.Label lbFileName;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox tbRateDataFile;
        private System.Windows.Forms.HScrollBar HSRecordInterval;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbRecordInterval;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}