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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMenuRateData));
            this.rbTarget = new System.Windows.Forms.RadioButton();
            this.rbApplied = new System.Windows.Forms.RadioButton();
            this.rbProductD = new System.Windows.Forms.RadioButton();
            this.rbProductC = new System.Windows.Forms.RadioButton();
            this.rbProductB = new System.Windows.Forms.RadioButton();
            this.rbProductA = new System.Windows.Forms.RadioButton();
            this.gbMap = new System.Windows.Forms.GroupBox();
            this.HSresolution = new System.Windows.Forms.HScrollBar();
            this.lbResolution = new System.Windows.Forms.Label();
            this.lbResolutionDescription = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbCoverage = new System.Windows.Forms.GroupBox();
            this.ckRecord = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.gbRecord = new System.Windows.Forms.GroupBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lbDataPoints = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnImport = new System.Windows.Forms.Button();
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
            this.gbMap.Location = new System.Drawing.Point(64, 136);
            this.gbMap.Name = "gbMap";
            this.gbMap.Size = new System.Drawing.Size(367, 103);
            this.gbMap.TabIndex = 364;
            this.gbMap.TabStop = false;
            this.gbMap.Text = "Product to Display";
            this.gbMap.Paint += new System.Windows.Forms.PaintEventHandler(this.gbMap_Paint);
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
            this.btnCancel.Location = new System.Drawing.Point(382, 603);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(70, 63);
            this.btnCancel.TabIndex = 378;
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // gbCoverage
            // 
            this.gbCoverage.Controls.Add(this.gbMap);
            this.gbCoverage.Controls.Add(this.rbApplied);
            this.gbCoverage.Controls.Add(this.lbResolutionDescription);
            this.gbCoverage.Controls.Add(this.rbTarget);
            this.gbCoverage.Controls.Add(this.HSresolution);
            this.gbCoverage.Controls.Add(this.lbResolution);
            this.gbCoverage.Location = new System.Drawing.Point(12, 252);
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
            this.ckRecord.Image = global::RateController.Properties.Resources.record;
            this.ckRecord.Location = new System.Drawing.Point(17, 28);
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
            this.btnOK.Location = new System.Drawing.Point(458, 603);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(70, 63);
            this.btnOK.TabIndex = 383;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // gbRecord
            // 
            this.gbRecord.Controls.Add(this.btnImport);
            this.gbRecord.Controls.Add(this.btnDelete);
            this.gbRecord.Controls.Add(this.lbDataPoints);
            this.gbRecord.Controls.Add(this.label1);
            this.gbRecord.Controls.Add(this.ckRecord);
            this.gbRecord.Location = new System.Drawing.Point(12, 35);
            this.gbRecord.Name = "gbRecord";
            this.gbRecord.Size = new System.Drawing.Size(516, 199);
            this.gbRecord.TabIndex = 384;
            this.gbRecord.TabStop = false;
            this.gbRecord.Text = "Record Rate Data";
            this.gbRecord.Paint += new System.Windows.Forms.PaintEventHandler(this.gbMap_Paint);
            // 
            // btnDelete
            // 
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Image = global::RateController.Properties.Resources.Trash;
            this.btnDelete.Location = new System.Drawing.Point(163, 69);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(82, 64);
            this.btnDelete.TabIndex = 397;
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lbDataPoints
            // 
            this.lbDataPoints.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDataPoints.Location = new System.Drawing.Point(392, 89);
            this.lbDataPoints.Name = "lbDataPoints";
            this.lbDataPoints.Size = new System.Drawing.Size(87, 23);
            this.lbDataPoints.TabIndex = 396;
            this.lbDataPoints.Text = "0";
            this.lbDataPoints.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(251, 89);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 23);
            this.label1.TabIndex = 395;
            this.label1.Text = "Data points:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(17, 116);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(89, 64);
            this.btnImport.TabIndex = 398;
            this.btnImport.Text = "Export CSV";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // frmMenuRateData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 678);
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
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMenuRateData_FormClosed);
            this.Load += new System.EventHandler(this.frmRates_Load);
            this.gbMap.ResumeLayout(false);
            this.gbCoverage.ResumeLayout(false);
            this.gbRecord.ResumeLayout(false);
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
        private System.Windows.Forms.HScrollBar HSresolution;
        private System.Windows.Forms.Label lbResolution;
        private System.Windows.Forms.Label lbResolutionDescription;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox gbCoverage;
        private System.Windows.Forms.CheckBox ckRecord;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox gbRecord;
        private System.Windows.Forms.Label lbDataPoints;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnImport;
    }
}