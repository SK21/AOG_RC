
namespace RateController
{
    partial class FormPressure
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPressure));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle37 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle45 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle38 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle39 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle40 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle41 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle42 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle43 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle44 = new System.Windows.Forms.DataGridViewCellStyle();
            this.bntOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.dataSet1 = new System.Data.DataSet();
            this.dataTable1 = new System.Data.DataTable();
            this.dataColumn1 = new System.Data.DataColumn();
            this.dataColumn2 = new System.Data.DataColumn();
            this.dataColumn3 = new System.Data.DataColumn();
            this.dataColumn5 = new System.Data.DataColumn();
            this.dataColumn6 = new System.Data.DataColumn();
            this.dataColumn7 = new System.Data.DataColumn();
            this.dataColumn4 = new System.Data.DataColumn();
            this.DGV = new System.Windows.Forms.DataGridView();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descriptionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.moduleIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sectionIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.unitsPerVoltDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pressureDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label28 = new System.Windows.Forms.Label();
            this.tbOffRate = new System.Windows.Forms.TextBox();
            this.ckOffRate = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DGV)).BeginInit();
            this.SuspendLayout();
            // 
            // bntOK
            // 
            this.bntOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bntOK.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.bntOK.Image = ((System.Drawing.Image)(resources.GetObject("bntOK.Image")));
            this.bntOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.bntOK.Location = new System.Drawing.Point(536, 332);
            this.bntOK.Name = "bntOK";
            this.bntOK.Size = new System.Drawing.Size(115, 72);
            this.bntOK.TabIndex = 261;
            this.bntOK.Text = "Close";
            this.bntOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.bntOK.UseVisualStyleBackColor = true;
            this.bntOK.Click += new System.EventHandler(this.bntOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnCancel.Enabled = false;
            this.btnCancel.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCancel.Location = new System.Drawing.Point(412, 332);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(115, 72);
            this.btnCancel.TabIndex = 296;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
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
            this.dataColumn1,
            this.dataColumn2,
            this.dataColumn3,
            this.dataColumn5,
            this.dataColumn6,
            this.dataColumn7,
            this.dataColumn4});
            this.dataTable1.TableName = "Table1";
            // 
            // dataColumn1
            // 
            this.dataColumn1.Caption = "ID";
            this.dataColumn1.ColumnName = "ID";
            this.dataColumn1.DataType = typeof(short);
            // 
            // dataColumn2
            // 
            this.dataColumn2.Caption = "Description";
            this.dataColumn2.ColumnName = "Description";
            // 
            // dataColumn3
            // 
            this.dataColumn3.Caption = "Module ID";
            this.dataColumn3.ColumnName = "ModuleID";
            this.dataColumn3.DataType = typeof(short);
            // 
            // dataColumn5
            // 
            this.dataColumn5.Caption = "Section ID";
            this.dataColumn5.ColumnName = "SectionID";
            this.dataColumn5.DataType = typeof(short);
            // 
            // dataColumn6
            // 
            this.dataColumn6.Caption = "Units/Volt";
            this.dataColumn6.ColumnName = "UnitsPerVolt";
            this.dataColumn6.DataType = typeof(float);
            // 
            // dataColumn7
            // 
            this.dataColumn7.ColumnName = "Pressure";
            this.dataColumn7.DataType = typeof(float);
            // 
            // dataColumn4
            // 
            this.dataColumn4.Caption = "Sensor";
            this.dataColumn4.ColumnName = "SensorID";
            // 
            // DGV
            // 
            this.DGV.AllowUserToAddRows = false;
            this.DGV.AllowUserToDeleteRows = false;
            this.DGV.AllowUserToResizeColumns = false;
            this.DGV.AllowUserToResizeRows = false;
            this.DGV.AutoGenerateColumns = false;
            dataGridViewCellStyle37.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle37.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle37.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle37.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle37.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle37.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle37.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DGV.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle37;
            this.DGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID,
            this.descriptionDataGridViewTextBoxColumn,
            this.moduleIDDataGridViewTextBoxColumn,
            this.Column2,
            this.sectionIDDataGridViewTextBoxColumn,
            this.unitsPerVoltDataGridViewTextBoxColumn,
            this.pressureDataGridViewTextBoxColumn});
            this.DGV.DataMember = "Table1";
            this.DGV.DataSource = this.dataSet1;
            dataGridViewCellStyle45.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle45.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle45.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle45.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle45.NullValue = "<dbnull>";
            dataGridViewCellStyle45.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle45.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle45.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DGV.DefaultCellStyle = dataGridViewCellStyle45;
            this.DGV.Location = new System.Drawing.Point(15, 15);
            this.DGV.Margin = new System.Windows.Forms.Padding(6);
            this.DGV.Name = "DGV";
            this.DGV.RowHeadersVisible = false;
            this.DGV.RowTemplate.Height = 40;
            this.DGV.Size = new System.Drawing.Size(636, 309);
            this.DGV.TabIndex = 297;
            this.DGV.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGV_CellClick);
            this.DGV.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.DGV_CellFormatting);
            this.DGV.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGV_CellValueChanged);
            // 
            // ID
            // 
            this.ID.DataPropertyName = "ID";
            dataGridViewCellStyle38.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ID.DefaultCellStyle = dataGridViewCellStyle38;
            this.ID.HeaderText = "ID";
            this.ID.Name = "ID";
            this.ID.Width = 50;
            // 
            // descriptionDataGridViewTextBoxColumn
            // 
            this.descriptionDataGridViewTextBoxColumn.DataPropertyName = "Description";
            dataGridViewCellStyle39.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.descriptionDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle39;
            this.descriptionDataGridViewTextBoxColumn.HeaderText = "Description";
            this.descriptionDataGridViewTextBoxColumn.Name = "descriptionDataGridViewTextBoxColumn";
            this.descriptionDataGridViewTextBoxColumn.Width = 150;
            // 
            // moduleIDDataGridViewTextBoxColumn
            // 
            this.moduleIDDataGridViewTextBoxColumn.DataPropertyName = "ModuleID";
            dataGridViewCellStyle40.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle40.NullValue = null;
            this.moduleIDDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle40;
            this.moduleIDDataGridViewTextBoxColumn.HeaderText = "Module";
            this.moduleIDDataGridViewTextBoxColumn.Name = "moduleIDDataGridViewTextBoxColumn";
            this.moduleIDDataGridViewTextBoxColumn.Width = 75;
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "SensorID";
            dataGridViewCellStyle41.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column2.DefaultCellStyle = dataGridViewCellStyle41;
            this.Column2.HeaderText = "Sensor";
            this.Column2.Name = "Column2";
            this.Column2.Width = 75;
            // 
            // sectionIDDataGridViewTextBoxColumn
            // 
            this.sectionIDDataGridViewTextBoxColumn.DataPropertyName = "SectionID";
            dataGridViewCellStyle42.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle42.Format = "N0";
            dataGridViewCellStyle42.NullValue = null;
            this.sectionIDDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle42;
            this.sectionIDDataGridViewTextBoxColumn.HeaderText = "Section";
            this.sectionIDDataGridViewTextBoxColumn.Name = "sectionIDDataGridViewTextBoxColumn";
            this.sectionIDDataGridViewTextBoxColumn.Width = 75;
            // 
            // unitsPerVoltDataGridViewTextBoxColumn
            // 
            this.unitsPerVoltDataGridViewTextBoxColumn.DataPropertyName = "UnitsPerVolt";
            dataGridViewCellStyle43.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle43.Format = "N1";
            dataGridViewCellStyle43.NullValue = null;
            this.unitsPerVoltDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle43;
            this.unitsPerVoltDataGridViewTextBoxColumn.HeaderText = "Units/Volt";
            this.unitsPerVoltDataGridViewTextBoxColumn.Name = "unitsPerVoltDataGridViewTextBoxColumn";
            this.unitsPerVoltDataGridViewTextBoxColumn.Width = 90;
            // 
            // pressureDataGridViewTextBoxColumn
            // 
            this.pressureDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.pressureDataGridViewTextBoxColumn.DataPropertyName = "Pressure";
            dataGridViewCellStyle44.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle44.Format = "N0";
            dataGridViewCellStyle44.NullValue = null;
            this.pressureDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle44;
            this.pressureDataGridViewTextBoxColumn.HeaderText = "Pressure";
            this.pressureDataGridViewTextBoxColumn.Name = "pressureDataGridViewTextBoxColumn";
            this.pressureDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.Location = new System.Drawing.Point(299, 355);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(29, 23);
            this.label28.TabIndex = 300;
            this.label28.Text = "%";
            // 
            // tbOffRate
            // 
            this.tbOffRate.Enabled = false;
            this.tbOffRate.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbOffRate.Location = new System.Drawing.Point(264, 351);
            this.tbOffRate.MaxLength = 8;
            this.tbOffRate.Name = "tbOffRate";
            this.tbOffRate.Size = new System.Drawing.Size(33, 30);
            this.tbOffRate.TabIndex = 299;
            this.tbOffRate.Text = "20";
            this.tbOffRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbOffRate.TextChanged += new System.EventHandler(this.tbOffRate_TextChanged);
            this.tbOffRate.Enter += new System.EventHandler(this.tbOffRate_Enter);
            this.tbOffRate.Validating += new System.ComponentModel.CancelEventHandler(this.tbOffRate_Validating);
            // 
            // ckOffRate
            // 
            this.ckOffRate.AutoSize = true;
            this.ckOffRate.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckOffRate.Location = new System.Drawing.Point(75, 353);
            this.ckOffRate.Name = "ckOffRate";
            this.ckOffRate.Size = new System.Drawing.Size(183, 28);
            this.ckOffRate.TabIndex = 298;
            this.ckOffRate.Text = "Off-Average Alarm";
            this.ckOffRate.UseVisualStyleBackColor = true;
            this.ckOffRate.CheckedChanged += new System.EventHandler(this.ckOffRate_CheckedChanged);
            // 
            // FormPressure
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(663, 416);
            this.Controls.Add(this.label28);
            this.Controls.Add(this.tbOffRate);
            this.Controls.Add(this.ckOffRate);
            this.Controls.Add(this.DGV);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.bntOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormPressure";
            this.ShowInTaskbar = false;
            this.Text = "Pressure";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormPressure_FormClosed);
            this.Load += new System.EventHandler(this.FormPressure_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DGV)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button bntOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Data.DataSet dataSet1;
        private System.Data.DataTable dataTable1;
        private System.Data.DataColumn dataColumn1;
        private System.Data.DataColumn dataColumn2;
        private System.Data.DataColumn dataColumn3;
        private System.Data.DataColumn dataColumn5;
        private System.Data.DataColumn dataColumn6;
        private System.Data.DataColumn dataColumn7;
        private System.Windows.Forms.DataGridView DGV;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.TextBox tbOffRate;
        private System.Windows.Forms.CheckBox ckOffRate;
        private System.Data.DataColumn dataColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn moduleIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn sectionIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn unitsPerVoltDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn pressureDataGridViewTextBoxColumn;
    }
}