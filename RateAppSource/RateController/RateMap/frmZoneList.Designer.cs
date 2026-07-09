namespace RateController.RateMap
{
    partial class frmZoneList
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmZoneList));
            this.dataSet1 = new System.Data.DataSet();
            this.dataTable1 = new System.Data.DataTable();
            this.dataRelay = new System.Data.DataColumn();
            this.dataType = new System.Data.DataColumn();
            this.dataSection = new System.Data.DataColumn();
            this.dataColumn1 = new System.Data.DataColumn();
            this.dataColumn2 = new System.Data.DataColumn();
            this.dataColumn3 = new System.Data.DataColumn();
            this.dataColumn4 = new System.Data.DataColumn();
            this.dataColumn5 = new System.Data.DataColumn();
            this.dataColumn6 = new System.Data.DataColumn();
            this.dataColumn7 = new System.Data.DataColumn();
            this.DGV = new System.Windows.Forms.DataGridView();
            this.cSelect = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.cNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cAreaDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cADataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cBDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cCDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cColorDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cNameIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnSelect = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnPrint = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DGV)).BeginInit();
            this.SuspendLayout();
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
            this.dataColumn1,
            this.dataColumn2,
            this.dataColumn3,
            this.dataColumn4,
            this.dataColumn5,
            this.dataColumn6,
            this.dataColumn7});
            this.dataTable1.TableName = "Table1";
            // 
            // dataRelay
            // 
            this.dataRelay.Caption = "Name";
            this.dataRelay.ColumnName = "cName";
            // 
            // dataType
            // 
            this.dataType.Caption = "Area";
            this.dataType.ColumnName = "cArea";
            this.dataType.DataType = typeof(decimal);
            this.dataType.ReadOnly = true;
            // 
            // dataSection
            // 
            this.dataSection.Caption = "A";
            this.dataSection.ColumnName = "cA";
            this.dataSection.DataType = typeof(decimal);
            // 
            // dataColumn1
            // 
            this.dataColumn1.Caption = "B";
            this.dataColumn1.ColumnName = "cB";
            this.dataColumn1.DataType = typeof(decimal);
            // 
            // dataColumn2
            // 
            this.dataColumn2.Caption = "C";
            this.dataColumn2.ColumnName = "cC";
            this.dataColumn2.DataType = typeof(decimal);
            // 
            // dataColumn3
            // 
            this.dataColumn3.Caption = "D";
            this.dataColumn3.ColumnName = "cD";
            this.dataColumn3.DataType = typeof(decimal);
            // 
            // dataColumn4
            // 
            this.dataColumn4.Caption = "E";
            this.dataColumn4.ColumnName = "cE";
            this.dataColumn4.DataType = typeof(decimal);
            // 
            // dataColumn5
            // 
            this.dataColumn5.Caption = "Color";
            this.dataColumn5.ColumnName = "cColor";
            this.dataColumn5.DataType = typeof(int);
            // 
            // dataColumn6
            // 
            this.dataColumn6.ColumnName = "cSelect";
            this.dataColumn6.DataType = typeof(bool);
            // 
            // dataColumn7
            // 
            this.dataColumn7.Caption = "NameIndex";
            this.dataColumn7.ColumnName = "cNameIndex";
            // 
            // DGV
            // 
            this.DGV.AllowUserToAddRows = false;
            this.DGV.AllowUserToResizeColumns = false;
            this.DGV.AllowUserToResizeRows = false;
            this.DGV.AutoGenerateColumns = false;
            this.DGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cSelect,
            this.cNameDataGridViewTextBoxColumn,
            this.cAreaDataGridViewTextBoxColumn,
            this.cADataGridViewTextBoxColumn,
            this.cBDataGridViewTextBoxColumn,
            this.cCDataGridViewTextBoxColumn,
            this.cDDataGridViewTextBoxColumn,
            this.cEDataGridViewTextBoxColumn,
            this.cColorDataGridViewTextBoxColumn,
            this.cNameIndex});
            this.DGV.DataMember = "Table1";
            this.DGV.DataSource = this.dataSet1;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle7.NullValue = "<dbnull>";
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DGV.DefaultCellStyle = dataGridViewCellStyle7;
            this.DGV.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
            this.DGV.Location = new System.Drawing.Point(20, 20);
            this.DGV.Margin = new System.Windows.Forms.Padding(11);
            this.DGV.Name = "DGV";
            this.DGV.RowHeadersVisible = false;
            this.DGV.RowTemplate.Height = 40;
            this.DGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.DGV.Size = new System.Drawing.Size(631, 307);
            this.DGV.TabIndex = 1;
            this.DGV.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGV_CellClick);
            this.DGV.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.DGV_CellFormatting);
            this.DGV.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGV_CellValueChanged);
            this.DGV.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.DGV_DataError);
            // 
            // cSelect
            // 
            this.cSelect.DataPropertyName = "cSelect";
            this.cSelect.HeaderText = "";
            this.cSelect.Name = "cSelect";
            this.cSelect.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.cSelect.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.cSelect.Width = 25;
            // 
            // cNameDataGridViewTextBoxColumn
            // 
            this.cNameDataGridViewTextBoxColumn.DataPropertyName = "cName";
            this.cNameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.cNameDataGridViewTextBoxColumn.Name = "cNameDataGridViewTextBoxColumn";
            this.cNameDataGridViewTextBoxColumn.Width = 150;
            // 
            // cAreaDataGridViewTextBoxColumn
            // 
            this.cAreaDataGridViewTextBoxColumn.DataPropertyName = "cArea";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle1.Format = "N1";
            dataGridViewCellStyle1.NullValue = null;
            this.cAreaDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle1;
            this.cAreaDataGridViewTextBoxColumn.HeaderText = "Area";
            this.cAreaDataGridViewTextBoxColumn.Name = "cAreaDataGridViewTextBoxColumn";
            this.cAreaDataGridViewTextBoxColumn.ReadOnly = true;
            this.cAreaDataGridViewTextBoxColumn.Width = 60;
            // 
            // cADataGridViewTextBoxColumn
            // 
            this.cADataGridViewTextBoxColumn.DataPropertyName = "cA";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.Format = "N1";
            dataGridViewCellStyle2.NullValue = null;
            this.cADataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.cADataGridViewTextBoxColumn.HeaderText = "A";
            this.cADataGridViewTextBoxColumn.Name = "cADataGridViewTextBoxColumn";
            this.cADataGridViewTextBoxColumn.Width = 60;
            // 
            // cBDataGridViewTextBoxColumn
            // 
            this.cBDataGridViewTextBoxColumn.DataPropertyName = "cB";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.Format = "N1";
            dataGridViewCellStyle3.NullValue = null;
            this.cBDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle3;
            this.cBDataGridViewTextBoxColumn.HeaderText = "B";
            this.cBDataGridViewTextBoxColumn.Name = "cBDataGridViewTextBoxColumn";
            this.cBDataGridViewTextBoxColumn.Width = 60;
            // 
            // cCDataGridViewTextBoxColumn
            // 
            this.cCDataGridViewTextBoxColumn.DataPropertyName = "cC";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.Format = "N1";
            dataGridViewCellStyle4.NullValue = null;
            this.cCDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle4;
            this.cCDataGridViewTextBoxColumn.HeaderText = "C";
            this.cCDataGridViewTextBoxColumn.Name = "cCDataGridViewTextBoxColumn";
            this.cCDataGridViewTextBoxColumn.Width = 60;
            // 
            // cDDataGridViewTextBoxColumn
            // 
            this.cDDataGridViewTextBoxColumn.DataPropertyName = "cD";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.Format = "N1";
            dataGridViewCellStyle5.NullValue = null;
            this.cDDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle5;
            this.cDDataGridViewTextBoxColumn.HeaderText = "D";
            this.cDDataGridViewTextBoxColumn.Name = "cDDataGridViewTextBoxColumn";
            this.cDDataGridViewTextBoxColumn.Width = 60;
            // 
            // cEDataGridViewTextBoxColumn
            // 
            this.cEDataGridViewTextBoxColumn.DataPropertyName = "cE";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.Format = "N1";
            dataGridViewCellStyle6.NullValue = null;
            this.cEDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle6;
            this.cEDataGridViewTextBoxColumn.HeaderText = "E";
            this.cEDataGridViewTextBoxColumn.Name = "cEDataGridViewTextBoxColumn";
            this.cEDataGridViewTextBoxColumn.Width = 60;
            // 
            // cColorDataGridViewTextBoxColumn
            // 
            this.cColorDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cColorDataGridViewTextBoxColumn.DataPropertyName = "cColor";
            this.cColorDataGridViewTextBoxColumn.HeaderText = "Color";
            this.cColorDataGridViewTextBoxColumn.Name = "cColorDataGridViewTextBoxColumn";
            this.cColorDataGridViewTextBoxColumn.ReadOnly = true;
            this.cColorDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // cNameIndex
            // 
            this.cNameIndex.DataPropertyName = "cNameIndex";
            this.cNameIndex.HeaderText = "cNameIndex";
            this.cNameIndex.Name = "cNameIndex";
            this.cNameIndex.Visible = false;
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
            this.btnCancel.Location = new System.Drawing.Point(513, 381);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(70, 63);
            this.btnCancel.TabIndex = 178;
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
            this.btnOK.Image = global::RateController.Properties.Resources.Save;
            this.btnOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnOK.Location = new System.Drawing.Point(589, 381);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(70, 63);
            this.btnOK.TabIndex = 177;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Image = global::RateController.Properties.Resources.Trash;
            this.btnDelete.Location = new System.Drawing.Point(425, 381);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(82, 64);
            this.btnDelete.TabIndex = 430;
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.FlatAppearance.BorderSize = 0;
            this.btnSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelect.Image = global::RateController.Properties.Resources.selection;
            this.btnSelect.Location = new System.Drawing.Point(337, 381);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(82, 64);
            this.btnSelect.TabIndex = 432;
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 401);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 24);
            this.label1.TabIndex = 433;
            this.label1.Text = "label1";
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.Color.Transparent;
            this.btnPrint.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnPrint.FlatAppearance.BorderSize = 0;
            this.btnPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrint.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnPrint.Image = global::RateController.Properties.Resources.printer;
            this.btnPrint.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnPrint.Location = new System.Drawing.Point(261, 378);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(70, 63);
            this.btnPrint.TabIndex = 434;
            this.btnPrint.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // frmZoneList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(671, 453);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.DGV);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmZoneList";
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmZoneList_FormClosing);
            this.Load += new System.EventHandler(this.frmZoneList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DGV)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Data.DataSet dataSet1;
        private System.Data.DataTable dataTable1;
        private System.Data.DataColumn dataRelay;
        private System.Data.DataColumn dataType;
        private System.Data.DataColumn dataSection;
        private System.Windows.Forms.DataGridView DGV;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnDelete;
        private System.Data.DataColumn dataColumn1;
        private System.Data.DataColumn dataColumn2;
        private System.Data.DataColumn dataColumn3;
        private System.Data.DataColumn dataColumn4;
        private System.Data.DataColumn dataColumn5;
        private System.Data.DataColumn dataColumn6;
        private System.Data.DataColumn dataColumn7;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cSelect;
        private System.Windows.Forms.DataGridViewTextBoxColumn cNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn cAreaDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn cADataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn cBDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn cCDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn cDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn cEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn cColorDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn cNameIndex;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnPrint;
    }
}