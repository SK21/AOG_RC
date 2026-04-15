namespace RateController.RateMap
{
    partial class frmCreateZones
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCreateZones));
            this.label1 = new System.Windows.Forms.Label();
            this.ckLBYields = new System.Windows.Forms.CheckedListBox();
            this.ckEC = new System.Windows.Forms.CheckBox();
            this.ckElevation = new System.Windows.Forms.CheckBox();
            this.tbWeightYield = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbWeightEC = new System.Windows.Forms.TextBox();
            this.tbWeightElevation = new System.Windows.Forms.TextBox();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblZoneCount = new System.Windows.Forms.Label();
            this.tbNumZones = new System.Windows.Forms.TextBox();
            this.lblMinSize = new System.Windows.Forms.Label();
            this.tbMinZoneSize = new System.Windows.Forms.TextBox();
            this.lbArea = new System.Windows.Forms.Label();
            this.lbTotal = new System.Windows.Forms.Label();
            this.btnBuild = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(101, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Yield";
            // 
            // ckLBYields
            // 
            this.ckLBYields.CheckOnClick = true;
            this.ckLBYields.FormattingEnabled = true;
            this.ckLBYields.Location = new System.Drawing.Point(101, 37);
            this.ckLBYields.Name = "ckLBYields";
            this.ckLBYields.ScrollAlwaysVisible = true;
            this.ckLBYields.Size = new System.Drawing.Size(282, 124);
            this.ckLBYields.TabIndex = 1;
            this.ckLBYields.SelectedIndexChanged += new System.EventHandler(this.ckLBYields_SelectedIndexChanged);
            // 
            // ckEC
            // 
            this.ckEC.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckEC.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckEC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckEC.Location = new System.Drawing.Point(101, 182);
            this.ckEC.Name = "ckEC";
            this.ckEC.Size = new System.Drawing.Size(107, 64);
            this.ckEC.TabIndex = 372;
            this.ckEC.Text = "EC";
            this.ckEC.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckEC.UseVisualStyleBackColor = true;
            this.ckEC.CheckedChanged += new System.EventHandler(this.ckEC_CheckedChanged);
            // 
            // ckElevation
            // 
            this.ckElevation.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckElevation.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckElevation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckElevation.Location = new System.Drawing.Point(101, 252);
            this.ckElevation.Name = "ckElevation";
            this.ckElevation.Size = new System.Drawing.Size(107, 64);
            this.ckElevation.TabIndex = 373;
            this.ckElevation.Text = "Elevation";
            this.ckElevation.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckElevation.UseVisualStyleBackColor = true;
            this.ckElevation.CheckedChanged += new System.EventHandler(this.ckEC_CheckedChanged);
            // 
            // tbWeightYield
            // 
            this.tbWeightYield.Location = new System.Drawing.Point(12, 84);
            this.tbWeightYield.Name = "tbWeightYield";
            this.tbWeightYield.Size = new System.Drawing.Size(69, 29);
            this.tbWeightYield.TabIndex = 374;
            this.tbWeightYield.Text = "75";
            this.tbWeightYield.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbWeightYield.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            this.tbWeightYield.Enter += new System.EventHandler(this.NumericTextBox_Enter);
            this.tbWeightYield.Validating += new System.ComponentModel.CancelEventHandler(this.NumericTextBox_Validating);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 24);
            this.label2.TabIndex = 375;
            this.label2.Text = "Weight";
            // 
            // tbWeightEC
            // 
            this.tbWeightEC.Location = new System.Drawing.Point(12, 199);
            this.tbWeightEC.Name = "tbWeightEC";
            this.tbWeightEC.Size = new System.Drawing.Size(69, 29);
            this.tbWeightEC.TabIndex = 376;
            this.tbWeightEC.Text = "20";
            this.tbWeightEC.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbWeightEC.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            this.tbWeightEC.Enter += new System.EventHandler(this.NumericTextBox_Enter);
            this.tbWeightEC.Validating += new System.ComponentModel.CancelEventHandler(this.NumericTextBox_Validating);
            // 
            // tbWeightElevation
            // 
            this.tbWeightElevation.Location = new System.Drawing.Point(12, 269);
            this.tbWeightElevation.Name = "tbWeightElevation";
            this.tbWeightElevation.Size = new System.Drawing.Size(69, 29);
            this.tbWeightElevation.TabIndex = 377;
            this.tbWeightElevation.Text = "5";
            this.tbWeightElevation.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbWeightElevation.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            this.tbWeightElevation.Enter += new System.EventHandler(this.NumericTextBox_Enter);
            this.tbWeightElevation.Validating += new System.ComponentModel.CancelEventHandler(this.NumericTextBox_Validating);
            // 
            // btnSelect
            // 
            this.btnSelect.FlatAppearance.BorderSize = 0;
            this.btnSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelect.Image = global::RateController.Properties.Resources.selection;
            this.btnSelect.Location = new System.Drawing.Point(75, 419);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(72, 72);
            this.btnSelect.TabIndex = 3;
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnCancel.Image = global::RateController.Properties.Resources.Cancel64;
            this.btnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCancel.Location = new System.Drawing.Point(231, 419);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 72);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.Transparent;
            this.btnOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnOK.FlatAppearance.BorderSize = 0;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnOK.Image = global::RateController.Properties.Resources.OK;
            this.btnOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnOK.Location = new System.Drawing.Point(311, 419);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(72, 72);
            this.btnOK.TabIndex = 0;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblZoneCount
            // 
            this.lblZoneCount.AutoSize = true;
            this.lblZoneCount.Location = new System.Drawing.Point(97, 339);
            this.lblZoneCount.Name = "lblZoneCount";
            this.lblZoneCount.Size = new System.Drawing.Size(155, 24);
            this.lblZoneCount.TabIndex = 436;
            this.lblZoneCount.Text = "Number of zones";
            // 
            // tbNumZones
            // 
            this.tbNumZones.Location = new System.Drawing.Point(258, 337);
            this.tbNumZones.Name = "tbNumZones";
            this.tbNumZones.Size = new System.Drawing.Size(69, 29);
            this.tbNumZones.TabIndex = 437;
            this.tbNumZones.Text = "5";
            this.tbNumZones.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbNumZones.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            this.tbNumZones.Enter += new System.EventHandler(this.NumericTextBox_Enter);
            this.tbNumZones.Validating += new System.ComponentModel.CancelEventHandler(this.NumericTextBox_Validating);
            // 
            // lblMinSize
            // 
            this.lblMinSize.AutoSize = true;
            this.lblMinSize.Location = new System.Drawing.Point(97, 374);
            this.lblMinSize.Name = "lblMinSize";
            this.lblMinSize.Size = new System.Drawing.Size(83, 24);
            this.lblMinSize.TabIndex = 438;
            this.lblMinSize.Text = "Grid size";
            // 
            // tbMinZoneSize
            // 
            this.tbMinZoneSize.Location = new System.Drawing.Point(258, 372);
            this.tbMinZoneSize.Name = "tbMinZoneSize";
            this.tbMinZoneSize.Size = new System.Drawing.Size(69, 29);
            this.tbMinZoneSize.TabIndex = 439;
            this.tbMinZoneSize.Text = "1";
            this.tbMinZoneSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbMinZoneSize.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            this.tbMinZoneSize.Enter += new System.EventHandler(this.NumericTextBox_Enter);
            this.tbMinZoneSize.Validating += new System.ComponentModel.CancelEventHandler(this.NumericTextBox_Validating);
            // 
            // lbArea
            // 
            this.lbArea.AutoSize = true;
            this.lbArea.Location = new System.Drawing.Point(338, 374);
            this.lbArea.Name = "lbArea";
            this.lbArea.Size = new System.Drawing.Size(33, 24);
            this.lbArea.TabIndex = 440;
            this.lbArea.Text = "Ac";
            // 
            // lbTotal
            // 
            this.lbTotal.Location = new System.Drawing.Point(12, 328);
            this.lbTotal.Name = "lbTotal";
            this.lbTotal.Size = new System.Drawing.Size(69, 29);
            this.lbTotal.TabIndex = 441;
            this.lbTotal.Text = "100";
            this.lbTotal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbTotal.TextChanged += new System.EventHandler(this.lbTotal_TextChanged);
            // 
            // btnBuild
            // 
            this.btnBuild.FlatAppearance.BorderSize = 0;
            this.btnBuild.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuild.Image = global::RateController.Properties.Resources.Start;
            this.btnBuild.Location = new System.Drawing.Point(153, 419);
            this.btnBuild.Name = "btnBuild";
            this.btnBuild.Size = new System.Drawing.Size(72, 72);
            this.btnBuild.TabIndex = 2;
            this.btnBuild.UseVisualStyleBackColor = true;
            this.btnBuild.Click += new System.EventHandler(this.btnBuild_Click);
            // 
            // frmCreateZones
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 503);
            this.Controls.Add(this.btnBuild);
            this.Controls.Add(this.lbTotal);
            this.Controls.Add(this.lbArea);
            this.Controls.Add(this.tbMinZoneSize);
            this.Controls.Add(this.lblMinSize);
            this.Controls.Add(this.tbNumZones);
            this.Controls.Add(this.lblZoneCount);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.tbWeightElevation);
            this.Controls.Add(this.tbWeightEC);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbWeightYield);
            this.Controls.Add(this.ckElevation);
            this.Controls.Add(this.ckEC);
            this.Controls.Add(this.ckLBYields);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCreateZones";
            this.ShowInTaskbar = false;
            this.Text = "Create Zones";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmCreateZones_FormClosing);
            this.Load += new System.EventHandler(this.frmCreateZones_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckedListBox ckLBYields;
        private System.Windows.Forms.CheckBox ckEC;
        private System.Windows.Forms.CheckBox ckElevation;
        private System.Windows.Forms.TextBox tbWeightYield;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbWeightEC;
        private System.Windows.Forms.TextBox tbWeightElevation;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblZoneCount;
        private System.Windows.Forms.TextBox tbNumZones;
        private System.Windows.Forms.Label lblMinSize;
        private System.Windows.Forms.TextBox tbMinZoneSize;
        private System.Windows.Forms.Label lbArea;
        private System.Windows.Forms.Label lbTotal;
        private System.Windows.Forms.Button btnBuild;
    }
}