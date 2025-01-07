namespace RateController.Menu
{
    partial class frmMenuSwitches
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.dataSet1 = new System.Data.DataSet();
            this.dataTable1 = new System.Data.DataTable();
            this.dataRelay = new System.Data.DataColumn();
            this.dataType = new System.Data.DataColumn();
            this.dataSection = new System.Data.DataColumn();
            this.dataColumn1 = new System.Data.DataColumn();
            this.ckDualAuto = new System.Windows.Forms.CheckBox();
            this.DGV = new System.Windows.Forms.DataGridView();
            this.iDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descriptionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.moduleDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Relay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ckScreenSwitches = new System.Windows.Forms.CheckBox();
            this.btnReset = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DGV)).BeginInit();
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
            this.btnCancel.Location = new System.Drawing.Point(378, 546);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 72);
            this.btnCancel.TabIndex = 164;
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
            this.btnOK.TabIndex = 163;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // dataSet1
            // 
            this.dataSet1.DataSetName = "NewDataSet";
            this.dataSet1.Tables.AddRange(new System.Data.DataTable[] {
            this.dataTable1});
            // 
            // dataTable1
            // 
            this.dataTable1.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataRelay,
            this.dataType,
            this.dataSection,
            this.dataColumn1});
            this.dataTable1.TableName = "Table1";
            // 
            // dataRelay
            // 
            this.dataRelay.Caption = "Relay";
            this.dataRelay.ColumnName = "ID";
            this.dataRelay.DataType = typeof(short);
            // 
            // dataType
            // 
            this.dataType.Caption = "Type";
            this.dataType.ColumnName = "Description";
            // 
            // dataSection
            // 
            this.dataSection.Caption = "Section";
            this.dataSection.ColumnName = "Module";
            // 
            // dataColumn1
            // 
            this.dataColumn1.ColumnName = "Relay";
            // 
            // ckDualAuto
            // 
            this.ckDualAuto.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckDualAuto.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckDualAuto.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckDualAuto.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckDualAuto.Location = new System.Drawing.Point(296, 32);
            this.ckDualAuto.Name = "ckDualAuto";
            this.ckDualAuto.Size = new System.Drawing.Size(164, 34);
            this.ckDualAuto.TabIndex = 167;
            this.ckDualAuto.Text = "Dual Auto";
            this.ckDualAuto.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckDualAuto.UseVisualStyleBackColor = true;
            this.ckDualAuto.CheckedChanged += new System.EventHandler(this.ckDualAuto_CheckedChanged);
            // 
            // DGV
            // 
            this.DGV.AllowUserToAddRows = false;
            this.DGV.AllowUserToDeleteRows = false;
            this.DGV.AllowUserToResizeColumns = false;
            this.DGV.AllowUserToResizeRows = false;
            this.DGV.AutoGenerateColumns = false;
            this.DGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.iDDataGridViewTextBoxColumn,
            this.descriptionDataGridViewTextBoxColumn,
            this.moduleDataGridViewTextBoxColumn,
            this.Relay});
            this.DGV.DataMember = "Table1";
            this.DGV.DataSource = this.dataSet1;
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle15.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle15.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle15.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle15.NullValue = "<dbnull>";
            dataGridViewCellStyle15.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle15.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DGV.DefaultCellStyle = dataGridViewCellStyle15;
            this.DGV.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
            this.DGV.Location = new System.Drawing.Point(55, 88);
            this.DGV.Margin = new System.Windows.Forms.Padding(11);
            this.DGV.Name = "DGV";
            this.DGV.RowHeadersVisible = false;
            this.DGV.RowTemplate.Height = 40;
            this.DGV.Size = new System.Drawing.Size(405, 444);
            this.DGV.TabIndex = 166;
            this.DGV.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGV_CellClick);
            this.DGV.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGV_CellValueChanged);
            this.DGV.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.DGV_DataError);
            // 
            // iDDataGridViewTextBoxColumn
            // 
            this.iDDataGridViewTextBoxColumn.DataPropertyName = "ID";
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.iDDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle11;
            this.iDDataGridViewTextBoxColumn.HeaderText = "ID";
            this.iDDataGridViewTextBoxColumn.Name = "iDDataGridViewTextBoxColumn";
            this.iDDataGridViewTextBoxColumn.ReadOnly = true;
            this.iDDataGridViewTextBoxColumn.Width = 50;
            // 
            // descriptionDataGridViewTextBoxColumn
            // 
            this.descriptionDataGridViewTextBoxColumn.DataPropertyName = "Description";
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.descriptionDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle12;
            this.descriptionDataGridViewTextBoxColumn.HeaderText = "Description";
            this.descriptionDataGridViewTextBoxColumn.Name = "descriptionDataGridViewTextBoxColumn";
            this.descriptionDataGridViewTextBoxColumn.Width = 120;
            // 
            // moduleDataGridViewTextBoxColumn
            // 
            this.moduleDataGridViewTextBoxColumn.DataPropertyName = "Module";
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.moduleDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle13;
            this.moduleDataGridViewTextBoxColumn.HeaderText = "Module";
            this.moduleDataGridViewTextBoxColumn.Name = "moduleDataGridViewTextBoxColumn";
            // 
            // Relay
            // 
            this.Relay.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Relay.DataPropertyName = "Relay";
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Relay.DefaultCellStyle = dataGridViewCellStyle14;
            this.Relay.HeaderText = "Relay";
            this.Relay.Name = "Relay";
            // 
            // ckScreenSwitches
            // 
            this.ckScreenSwitches.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckScreenSwitches.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckScreenSwitches.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckScreenSwitches.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckScreenSwitches.Location = new System.Drawing.Point(55, 32);
            this.ckScreenSwitches.Name = "ckScreenSwitches";
            this.ckScreenSwitches.Size = new System.Drawing.Size(164, 34);
            this.ckScreenSwitches.TabIndex = 165;
            this.ckScreenSwitches.Text = "Use Switches";
            this.ckScreenSwitches.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckScreenSwitches.UseVisualStyleBackColor = true;
            this.ckScreenSwitches.CheckedChanged += new System.EventHandler(this.ckDualAuto_CheckedChanged);
            // 
            // btnReset
            // 
            this.btnReset.BackColor = System.Drawing.Color.Transparent;
            this.btnReset.FlatAppearance.BorderSize = 0;
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReset.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReset.Image = global::RateController.Properties.Resources.Update;
            this.btnReset.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnReset.Location = new System.Drawing.Point(300, 546);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(72, 72);
            this.btnReset.TabIndex = 168;
            this.btnReset.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnReset.UseVisualStyleBackColor = false;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // frmMenuSwitches
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 630);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.ckDualAuto);
            this.Controls.Add(this.DGV);
            this.Controls.Add(this.ckScreenSwitches);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "frmMenuSwitches";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuSwitches";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMenuSwitches_FormClosed);
            this.Load += new System.EventHandler(this.frmMenuSwitches_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DGV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Data.DataSet dataSet1;
        private System.Data.DataTable dataTable1;
        private System.Data.DataColumn dataRelay;
        private System.Data.DataColumn dataType;
        private System.Data.DataColumn dataSection;
        private System.Data.DataColumn dataColumn1;
        private System.Windows.Forms.CheckBox ckDualAuto;
        private System.Windows.Forms.DataGridView DGV;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn moduleDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Relay;
        private System.Windows.Forms.CheckBox ckScreenSwitches;
        private System.Windows.Forms.Button btnReset;
    }
}