namespace RateController.Forms
{
    partial class frmGenerateZones
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.grpYield        = new System.Windows.Forms.GroupBox();
            this.pnlYield        = new System.Windows.Forms.Panel();
            this.lblWeightHeader = new System.Windows.Forms.Label();
            this.btnAll          = new System.Windows.Forms.Button();
            this.btnNone         = new System.Windows.Forms.Button();
            this.grpLayers       = new System.Windows.Forms.GroupBox();
            this.ckElevation     = new System.Windows.Forms.CheckBox();
            this.nudElevWeight   = new System.Windows.Forms.NumericUpDown();
            this.ckEC            = new System.Windows.Forms.CheckBox();
            this.nudEcWeight     = new System.Windows.Forms.NumericUpDown();
            this.grpSettings     = new System.Windows.Forms.GroupBox();
            this.lblZoneCount    = new System.Windows.Forms.Label();
            this.nudZoneCount    = new System.Windows.Forms.NumericUpDown();
            this.lblMinSize      = new System.Windows.Forms.Label();
            this.nudMinSize      = new System.Windows.Forms.NumericUpDown();
            this.lblHa           = new System.Windows.Forms.Label();
            this.lblTotal        = new System.Windows.Forms.Label();
            this.btnGenerate     = new System.Windows.Forms.Button();
            this.btnCancel       = new System.Windows.Forms.Button();
            this.grpYield.SuspendLayout();
            this.grpLayers.SuspendLayout();
            this.grpSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudElevWeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudEcWeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudZoneCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinSize)).BeginInit();
            this.SuspendLayout();
            //
            // grpYield
            //
            this.grpYield.Controls.Add(this.pnlYield);
            this.grpYield.Controls.Add(this.lblWeightHeader);
            this.grpYield.Controls.Add(this.btnAll);
            this.grpYield.Controls.Add(this.btnNone);
            this.grpYield.Location = new System.Drawing.Point(8, 8);
            this.grpYield.Name     = "grpYield";
            this.grpYield.Size     = new System.Drawing.Size(508, 248);
            this.grpYield.TabIndex = 0;
            this.grpYield.TabStop  = false;
            this.grpYield.Text     = "Yield data";
            //
            // pnlYield  (dynamic yield rows added in code)
            //
            this.pnlYield.AutoScroll = true;
            this.pnlYield.Location   = new System.Drawing.Point(8, 28);
            this.pnlYield.Name       = "pnlYield";
            this.pnlYield.Size       = new System.Drawing.Size(492, 172);
            this.pnlYield.TabIndex   = 0;
            //
            // lblWeightHeader
            //
            this.lblWeightHeader.AutoSize = true;
            this.lblWeightHeader.Location = new System.Drawing.Point(322, 8);
            this.lblWeightHeader.Name     = "lblWeightHeader";
            this.lblWeightHeader.Text     = "Wt %";
            this.lblWeightHeader.Visible  = false;
            //
            // btnAll
            //
            this.btnAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAll.Location  = new System.Drawing.Point(8, 210);
            this.btnAll.Name      = "btnAll";
            this.btnAll.Size      = new System.Drawing.Size(80, 28);
            this.btnAll.TabIndex  = 1;
            this.btnAll.Text      = "All";
            this.btnAll.Click    += new System.EventHandler(this.btnAll_Click);
            //
            // btnNone
            //
            this.btnNone.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNone.Location  = new System.Drawing.Point(96, 210);
            this.btnNone.Name      = "btnNone";
            this.btnNone.Size      = new System.Drawing.Size(80, 28);
            this.btnNone.TabIndex  = 2;
            this.btnNone.Text      = "None";
            this.btnNone.Click    += new System.EventHandler(this.btnNone_Click);
            //
            // grpLayers
            //
            this.grpLayers.Controls.Add(this.ckElevation);
            this.grpLayers.Controls.Add(this.nudElevWeight);
            this.grpLayers.Controls.Add(this.ckEC);
            this.grpLayers.Controls.Add(this.nudEcWeight);
            this.grpLayers.Location = new System.Drawing.Point(8, 264);
            this.grpLayers.Name     = "grpLayers";
            this.grpLayers.Size     = new System.Drawing.Size(508, 88);
            this.grpLayers.TabIndex = 1;
            this.grpLayers.TabStop  = false;
            this.grpLayers.Text     = "Other layers";
            //
            // ckElevation
            //
            this.ckElevation.AutoSize  = true;
            this.ckElevation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckElevation.Location  = new System.Drawing.Point(12, 28);
            this.ckElevation.Name      = "ckElevation";
            this.ckElevation.Text      = "Elevation / TWI  (no data)";
            this.ckElevation.TabIndex  = 0;
            //
            // nudElevWeight  (hidden — elevation not yet integrated into algorithm)
            //
            this.nudElevWeight.Location  = new System.Drawing.Point(322, 26);
            this.nudElevWeight.Minimum   = 1;
            this.nudElevWeight.Maximum   = 100;
            this.nudElevWeight.Value     = 100;
            this.nudElevWeight.Increment = 5;
            this.nudElevWeight.Width     = 62;
            this.nudElevWeight.Visible   = false;
            this.nudElevWeight.TabIndex  = 1;
            //
            // ckEC
            //
            this.ckEC.AutoSize  = true;
            this.ckEC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckEC.Location  = new System.Drawing.Point(12, 58);
            this.ckEC.Name      = "ckEC";
            this.ckEC.Text      = "EC  (no data)";
            this.ckEC.TabIndex  = 2;
            //
            // nudEcWeight
            //
            this.nudEcWeight.Location  = new System.Drawing.Point(322, 56);
            this.nudEcWeight.Minimum   = 1;
            this.nudEcWeight.Maximum   = 100;
            this.nudEcWeight.Value     = 100;
            this.nudEcWeight.Increment = 5;
            this.nudEcWeight.Width     = 62;
            this.nudEcWeight.Visible   = false;
            this.nudEcWeight.TabIndex  = 3;
            //
            // grpSettings
            //
            this.grpSettings.Controls.Add(this.lblZoneCount);
            this.grpSettings.Controls.Add(this.nudZoneCount);
            this.grpSettings.Controls.Add(this.lblMinSize);
            this.grpSettings.Controls.Add(this.nudMinSize);
            this.grpSettings.Controls.Add(this.lblHa);
            this.grpSettings.Location = new System.Drawing.Point(8, 360);
            this.grpSettings.Name     = "grpSettings";
            this.grpSettings.Size     = new System.Drawing.Size(508, 92);
            this.grpSettings.TabIndex = 2;
            this.grpSettings.TabStop  = false;
            this.grpSettings.Text     = "Zone settings";
            //
            // lblZoneCount
            //
            this.lblZoneCount.AutoSize = true;
            this.lblZoneCount.Location = new System.Drawing.Point(12, 30);
            this.lblZoneCount.Name     = "lblZoneCount";
            this.lblZoneCount.Text     = "Number of zones";
            //
            // nudZoneCount
            //
            this.nudZoneCount.Location = new System.Drawing.Point(350, 28);
            this.nudZoneCount.Minimum  = 2;
            this.nudZoneCount.Maximum  = 5;
            this.nudZoneCount.Value    = 3;
            this.nudZoneCount.Width    = 60;
            this.nudZoneCount.TabIndex = 0;
            //
            // lblMinSize
            //
            this.lblMinSize.AutoSize = true;
            this.lblMinSize.Location = new System.Drawing.Point(12, 62);
            this.lblMinSize.Name     = "lblMinSize";
            this.lblMinSize.Text     = "Minimum zone size";
            //
            // nudMinSize  (range/default set in SetupUnits based on metric/imperial)
            //
            this.nudMinSize.Location      = new System.Drawing.Point(350, 60);
            this.nudMinSize.Minimum       = new decimal(new int[] { 1, 0, 0, 65536 }); // 0.1
            this.nudMinSize.Maximum       = 20;
            this.nudMinSize.Value         = new decimal(new int[] { 5, 0, 0, 65536 }); // 0.5
            this.nudMinSize.DecimalPlaces = 1;
            this.nudMinSize.Increment     = new decimal(new int[] { 1, 0, 0, 65536 }); // 0.1
            this.nudMinSize.Width         = 70;
            this.nudMinSize.TabIndex      = 1;
            //
            // lblHa
            //
            this.lblHa.AutoSize = true;
            this.lblHa.Location = new System.Drawing.Point(426, 64);
            this.lblHa.Name     = "lblHa";
            this.lblHa.Text     = "ha";
            //
            // lblTotal
            //
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(12, 462);
            this.lblTotal.Name     = "lblTotal";
            this.lblTotal.Text     = "";
            //
            // btnGenerate
            //
            this.btnGenerate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerate.Location  = new System.Drawing.Point(290, 458);
            this.btnGenerate.Name      = "btnGenerate";
            this.btnGenerate.Size      = new System.Drawing.Size(108, 32);
            this.btnGenerate.TabIndex  = 10;
            this.btnGenerate.Text      = "Generate";
            this.btnGenerate.Click    += new System.EventHandler(this.btnGenerate_Click);
            //
            // btnCancel
            //
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location  = new System.Drawing.Point(404, 458);
            this.btnCancel.Name      = "btnCancel";
            this.btnCancel.Size      = new System.Drawing.Size(104, 32);
            this.btnCancel.TabIndex  = 11;
            this.btnCancel.Text      = "Cancel";
            this.btnCancel.Click    += new System.EventHandler(this.btnCancel_Click);
            //
            // frmGenerateZones
            //
            this.AcceptButton        = this.btnGenerate;
            this.CancelButton        = this.btnCancel;
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(524, 500);
            this.Controls.Add(this.grpYield);
            this.Controls.Add(this.grpLayers);
            this.Controls.Add(this.grpSettings);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.btnCancel);
            this.Font            = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin          = new System.Windows.Forms.Padding(6);
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.Name            = "frmGenerateZones";
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text            = "Generate Zones";
            this.TopMost         = true;
            this.grpYield.ResumeLayout(false);
            this.grpYield.PerformLayout();
            this.grpLayers.ResumeLayout(false);
            this.grpLayers.PerformLayout();
            this.grpSettings.ResumeLayout(false);
            this.grpSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudElevWeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudEcWeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudZoneCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox      grpYield;
        private System.Windows.Forms.Panel         pnlYield;
        private System.Windows.Forms.Label         lblWeightHeader;
        private System.Windows.Forms.Button        btnAll;
        private System.Windows.Forms.Button        btnNone;
        private System.Windows.Forms.GroupBox      grpLayers;
        private System.Windows.Forms.CheckBox      ckElevation;
        private System.Windows.Forms.NumericUpDown nudElevWeight;
        private System.Windows.Forms.CheckBox      ckEC;
        private System.Windows.Forms.NumericUpDown nudEcWeight;
        private System.Windows.Forms.GroupBox      grpSettings;
        private System.Windows.Forms.Label         lblZoneCount;
        private System.Windows.Forms.NumericUpDown nudZoneCount;
        private System.Windows.Forms.Label         lblMinSize;
        private System.Windows.Forms.NumericUpDown nudMinSize;
        private System.Windows.Forms.Label         lblHa;
        private System.Windows.Forms.Label         lblTotal;
        private System.Windows.Forms.Button        btnGenerate;
        private System.Windows.Forms.Button        btnCancel;
    }
}
