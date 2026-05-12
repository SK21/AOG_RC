namespace RateController.Forms
{
    partial class frmImport
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvMapping = new System.Windows.Forms.DataGridView();
            this.PredefinedAttribute = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ShapeFileAttribute = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.btnBuild = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lbArea = new System.Windows.Forms.Label();
            this.tbMinZoneSize = new System.Windows.Forms.TextBox();
            this.lblMinSize = new System.Windows.Forms.Label();
            this.tbNumZones = new System.Windows.Forms.TextBox();
            this.lblZoneCount = new System.Windows.Forms.Label();
            this.tbStep = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lbAreaStep = new System.Windows.Forms.Label();
            this.btnAdjust = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMapping)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvMapping
            // 
            this.dgvMapping.AllowUserToAddRows = false;
            this.dgvMapping.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvMapping.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvMapping.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMapping.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PredefinedAttribute,
            this.ShapeFileAttribute});
            this.dgvMapping.Location = new System.Drawing.Point(36, 134);
            this.dgvMapping.Name = "dgvMapping";
            this.dgvMapping.RowHeadersVisible = false;
            this.dgvMapping.RowTemplate.Height = 40;
            this.dgvMapping.Size = new System.Drawing.Size(395, 301);
            this.dgvMapping.TabIndex = 8;
            // 
            // PredefinedAttribute
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.PredefinedAttribute.DefaultCellStyle = dataGridViewCellStyle2;
            this.PredefinedAttribute.HeaderText = "Zone Attribute";
            this.PredefinedAttribute.Name = "PredefinedAttribute";
            this.PredefinedAttribute.ReadOnly = true;
            this.PredefinedAttribute.Width = 200;
            // 
            // ShapeFileAttribute
            // 
            this.ShapeFileAttribute.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ShapeFileAttribute.DefaultCellStyle = dataGridViewCellStyle3;
            this.ShapeFileAttribute.HeaderText = "Shapefile Attribute";
            this.ShapeFileAttribute.Name = "ShapeFileAttribute";
            this.ShapeFileAttribute.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ShapeFileAttribute.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Image = global::RateController.Properties.Resources.Save;
            this.btnSave.Location = new System.Drawing.Point(414, 551);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(82, 64);
            this.btnSave.TabIndex = 7;
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
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
            this.btnCancel.Location = new System.Drawing.Point(319, 551);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(89, 64);
            this.btnCancel.TabIndex = 379;
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 90);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 24);
            this.label1.TabIndex = 380;
            this.label1.Text = "Name:";
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(124, 87);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(225, 29);
            this.tbName.TabIndex = 0;
            // 
            // btnBuild
            // 
            this.btnBuild.FlatAppearance.BorderSize = 0;
            this.btnBuild.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuild.Image = global::RateController.Properties.Resources.Start;
            this.btnBuild.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBuild.Location = new System.Drawing.Point(217, 9);
            this.btnBuild.Name = "btnBuild";
            this.btnBuild.Size = new System.Drawing.Size(132, 72);
            this.btnBuild.TabIndex = 381;
            this.btnBuild.Text = "Import";
            this.btnBuild.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnBuild.UseVisualStyleBackColor = true;
            this.btnBuild.Click += new System.EventHandler(this.btnBuild_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(167, 24);
            this.label2.TabIndex = 382;
            this.label2.Text = "Step 1 Import Data:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 465);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(181, 24);
            this.label3.TabIndex = 383;
            this.label3.Text = "Step 2 Adjust zones:";
            // 
            // lbArea
            // 
            this.lbArea.AutoSize = true;
            this.lbArea.Location = new System.Drawing.Point(253, 556);
            this.lbArea.Name = "lbArea";
            this.lbArea.Size = new System.Drawing.Size(33, 24);
            this.lbArea.TabIndex = 445;
            this.lbArea.Text = "Ac";
            // 
            // tbMinZoneSize
            // 
            this.tbMinZoneSize.Location = new System.Drawing.Point(173, 554);
            this.tbMinZoneSize.Name = "tbMinZoneSize";
            this.tbMinZoneSize.Size = new System.Drawing.Size(69, 29);
            this.tbMinZoneSize.TabIndex = 444;
            this.tbMinZoneSize.Text = "0";
            this.tbMinZoneSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbMinZoneSize.Enter += new System.EventHandler(this.tbMinZoneSize_Enter);
            // 
            // lblMinSize
            // 
            this.lblMinSize.AutoSize = true;
            this.lblMinSize.Location = new System.Drawing.Point(12, 556);
            this.lblMinSize.Name = "lblMinSize";
            this.lblMinSize.Size = new System.Drawing.Size(126, 24);
            this.lblMinSize.TabIndex = 443;
            this.lblMinSize.Text = "Min zone size";
            // 
            // tbNumZones
            // 
            this.tbNumZones.Location = new System.Drawing.Point(173, 519);
            this.tbNumZones.Name = "tbNumZones";
            this.tbNumZones.Size = new System.Drawing.Size(69, 29);
            this.tbNumZones.TabIndex = 442;
            this.tbNumZones.Text = "5";
            this.tbNumZones.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbNumZones.Enter += new System.EventHandler(this.tbNumZones_Enter);
            // 
            // lblZoneCount
            // 
            this.lblZoneCount.AutoSize = true;
            this.lblZoneCount.Location = new System.Drawing.Point(12, 521);
            this.lblZoneCount.Name = "lblZoneCount";
            this.lblZoneCount.Size = new System.Drawing.Size(105, 24);
            this.lblZoneCount.TabIndex = 441;
            this.lblZoneCount.Text = "Max Zones";
            // 
            // tbStep
            // 
            this.tbStep.Location = new System.Drawing.Point(173, 589);
            this.tbStep.Name = "tbStep";
            this.tbStep.Size = new System.Drawing.Size(69, 29);
            this.tbStep.TabIndex = 447;
            this.tbStep.Text = "5";
            this.tbStep.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbStep.Enter += new System.EventHandler(this.tbStep_Enter);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 591);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(128, 24);
            this.label4.TabIndex = 446;
            this.label4.Text = "Min zone step";
            // 
            // lbAreaStep
            // 
            this.lbAreaStep.AutoSize = true;
            this.lbAreaStep.Location = new System.Drawing.Point(253, 592);
            this.lbAreaStep.Name = "lbAreaStep";
            this.lbAreaStep.Size = new System.Drawing.Size(33, 24);
            this.lbAreaStep.TabIndex = 448;
            this.lbAreaStep.Text = "Ac";
            // 
            // btnAdjust
            // 
            this.btnAdjust.FlatAppearance.BorderSize = 0;
            this.btnAdjust.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdjust.Image = global::RateController.Properties.Resources.Start;
            this.btnAdjust.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdjust.Location = new System.Drawing.Point(217, 441);
            this.btnAdjust.Name = "btnAdjust";
            this.btnAdjust.Size = new System.Drawing.Size(132, 72);
            this.btnAdjust.TabIndex = 449;
            this.btnAdjust.Text = "Adjust";
            this.btnAdjust.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAdjust.UseVisualStyleBackColor = true;
            this.btnAdjust.Click += new System.EventHandler(this.btnAdjust_Click);
            // 
            // frmImport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(508, 627);
            this.Controls.Add(this.btnAdjust);
            this.Controls.Add(this.lbAreaStep);
            this.Controls.Add(this.tbStep);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lbArea);
            this.Controls.Add(this.tbMinZoneSize);
            this.Controls.Add(this.lblMinSize);
            this.Controls.Add(this.tbNumZones);
            this.Controls.Add(this.lblZoneCount);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnBuild);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.dgvMapping);
            this.Controls.Add(this.btnSave);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmImport";
            this.Text = "Import Prescription";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmImport_FormClosed);
            this.Load += new System.EventHandler(this.frmImport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMapping)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvMapping;
        private System.Windows.Forms.DataGridViewTextBoxColumn PredefinedAttribute;
        private System.Windows.Forms.DataGridViewComboBoxColumn ShapeFileAttribute;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Button btnBuild;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbArea;
        private System.Windows.Forms.TextBox tbMinZoneSize;
        private System.Windows.Forms.Label lblMinSize;
        private System.Windows.Forms.TextBox tbNumZones;
        private System.Windows.Forms.Label lblZoneCount;
        private System.Windows.Forms.TextBox tbStep;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbAreaStep;
        private System.Windows.Forms.Button btnAdjust;
    }
}