
namespace RateController
{
    partial class frmSections
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSections));
            this.DGV = new System.Windows.Forms.DataGridView();
            this.sectionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SectionWidth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Switch = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataSet1 = new System.Data.DataSet();
            this.dataTable1 = new System.Data.DataTable();
            this.dataColumn1 = new System.Data.DataColumn();
            this.dataColumn2 = new System.Data.DataColumn();
            this.dataColumn3 = new System.Data.DataColumn();
            this.lbWidth = new System.Windows.Forms.Label();
            this.tbSectionCount = new System.Windows.Forms.TextBox();
            this.lbNumZones = new System.Windows.Forms.Label();
            this.lbFeet = new System.Windows.Forms.Label();
            this.sectionDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.widthDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.switchDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sectionDataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sectionDataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.widthDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.switchDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DGV2 = new System.Windows.Forms.DataGridView();
            this.zoneDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.startDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.endDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.widthDataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.switchDataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataSet2 = new System.Data.DataSet();
            this.dataTable2 = new System.Data.DataTable();
            this.dataColumn4 = new System.Data.DataColumn();
            this.dataColumn5 = new System.Data.DataColumn();
            this.dataColumn6 = new System.Data.DataColumn();
            this.dataColumn7 = new System.Data.DataColumn();
            this.dataColumn8 = new System.Data.DataColumn();
            this.lbPerZone = new System.Windows.Forms.Label();
            this.tbSectionsPerZone = new System.Windows.Forms.TextBox();
            this.ckZones = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.bntOK = new System.Windows.Forms.Button();
            this.btnEqual = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.DGV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DGV2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable2)).BeginInit();
            this.SuspendLayout();
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
            this.sectionDataGridViewTextBoxColumn,
            this.SectionWidth,
            this.Switch});
            this.DGV.DataMember = "Table1";
            this.DGV.DataSource = this.dataSet1;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.NullValue = "<dbnull>";
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DGV.DefaultCellStyle = dataGridViewCellStyle4;
            this.DGV.Location = new System.Drawing.Point(104, 15);
            this.DGV.Margin = new System.Windows.Forms.Padding(6);
            this.DGV.Name = "DGV";
            this.DGV.RowHeadersVisible = false;
            this.DGV.RowTemplate.Height = 40;
            this.DGV.Size = new System.Drawing.Size(369, 352);
            this.DGV.TabIndex = 0;
            this.DGV.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGV_CellClick);
            this.DGV.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.DGV_CellFormatting);
            this.DGV.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGV_CellValueChanged);
            // 
            // sectionDataGridViewTextBoxColumn
            // 
            this.sectionDataGridViewTextBoxColumn.DataPropertyName = "Section";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.sectionDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle1;
            this.sectionDataGridViewTextBoxColumn.HeaderText = "Section";
            this.sectionDataGridViewTextBoxColumn.Name = "sectionDataGridViewTextBoxColumn";
            this.sectionDataGridViewTextBoxColumn.ReadOnly = true;
            this.sectionDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // SectionWidth
            // 
            this.SectionWidth.DataPropertyName = "Width";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Format = "N0";
            dataGridViewCellStyle2.NullValue = null;
            this.SectionWidth.DefaultCellStyle = dataGridViewCellStyle2;
            this.SectionWidth.HeaderText = "Width";
            this.SectionWidth.Name = "SectionWidth";
            this.SectionWidth.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // Switch
            // 
            this.Switch.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Switch.DataPropertyName = "Switch";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.Format = "N0";
            dataGridViewCellStyle3.NullValue = null;
            this.Switch.DefaultCellStyle = dataGridViewCellStyle3;
            this.Switch.HeaderText = "Switch";
            this.Switch.Name = "Switch";
            this.Switch.Resizable = System.Windows.Forms.DataGridViewTriState.False;
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
            this.dataColumn3});
            this.dataTable1.TableName = "Table1";
            // 
            // dataColumn1
            // 
            this.dataColumn1.Caption = "Section";
            this.dataColumn1.ColumnName = "Section";
            this.dataColumn1.DataType = typeof(short);
            // 
            // dataColumn2
            // 
            this.dataColumn2.Caption = "Width";
            this.dataColumn2.ColumnName = "Width";
            this.dataColumn2.DataType = typeof(double);
            // 
            // dataColumn3
            // 
            this.dataColumn3.ColumnName = "Switch";
            this.dataColumn3.DataType = typeof(short);
            // 
            // lbWidth
            // 
            this.lbWidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbWidth.Location = new System.Drawing.Point(15, 414);
            this.lbWidth.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbWidth.Name = "lbWidth";
            this.lbWidth.Size = new System.Drawing.Size(187, 24);
            this.lbWidth.TabIndex = 12;
            this.lbWidth.Text = "Width:  1200 Inches";
            this.lbWidth.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbSectionCount
            // 
            this.tbSectionCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSectionCount.Location = new System.Drawing.Point(212, 379);
            this.tbSectionCount.Margin = new System.Windows.Forms.Padding(6);
            this.tbSectionCount.Name = "tbSectionCount";
            this.tbSectionCount.Size = new System.Drawing.Size(54, 29);
            this.tbSectionCount.TabIndex = 6;
            this.tbSectionCount.Text = "9999";
            this.tbSectionCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSectionCount.TextChanged += new System.EventHandler(this.tbSectionCount_TextChanged);
            this.tbSectionCount.Enter += new System.EventHandler(this.tbSectionCount_Enter);
            this.tbSectionCount.Validating += new System.ComponentModel.CancelEventHandler(this.tbSectionCount_Validating);
            // 
            // lbNumZones
            // 
            this.lbNumZones.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbNumZones.Location = new System.Drawing.Point(15, 379);
            this.lbNumZones.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbNumZones.Name = "lbNumZones";
            this.lbNumZones.Size = new System.Drawing.Size(185, 28);
            this.lbNumZones.TabIndex = 14;
            this.lbNumZones.Text = "Number of Sections";
            // 
            // lbFeet
            // 
            this.lbFeet.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbFeet.Location = new System.Drawing.Point(212, 414);
            this.lbFeet.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbFeet.Name = "lbFeet";
            this.lbFeet.Size = new System.Drawing.Size(133, 24);
            this.lbFeet.TabIndex = 138;
            this.lbFeet.Text = "100.6 FT";
            this.lbFeet.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // sectionDataGridViewTextBoxColumn1
            // 
            this.sectionDataGridViewTextBoxColumn1.DataPropertyName = "Section";
            this.sectionDataGridViewTextBoxColumn1.HeaderText = "Section";
            this.sectionDataGridViewTextBoxColumn1.Name = "sectionDataGridViewTextBoxColumn1";
            // 
            // widthDataGridViewTextBoxColumn1
            // 
            this.widthDataGridViewTextBoxColumn1.DataPropertyName = "Width";
            this.widthDataGridViewTextBoxColumn1.HeaderText = "Width";
            this.widthDataGridViewTextBoxColumn1.Name = "widthDataGridViewTextBoxColumn1";
            // 
            // switchDataGridViewTextBoxColumn1
            // 
            this.switchDataGridViewTextBoxColumn1.DataPropertyName = "Switch";
            this.switchDataGridViewTextBoxColumn1.HeaderText = "Switch";
            this.switchDataGridViewTextBoxColumn1.Name = "switchDataGridViewTextBoxColumn1";
            // 
            // sectionDataGridViewTextBoxColumn2
            // 
            this.sectionDataGridViewTextBoxColumn2.DataPropertyName = "Section";
            this.sectionDataGridViewTextBoxColumn2.HeaderText = "Section";
            this.sectionDataGridViewTextBoxColumn2.Name = "sectionDataGridViewTextBoxColumn2";
            // 
            // sectionDataGridViewTextBoxColumn3
            // 
            this.sectionDataGridViewTextBoxColumn3.DataPropertyName = "Section";
            this.sectionDataGridViewTextBoxColumn3.HeaderText = "Section";
            this.sectionDataGridViewTextBoxColumn3.Name = "sectionDataGridViewTextBoxColumn3";
            // 
            // widthDataGridViewTextBoxColumn
            // 
            this.widthDataGridViewTextBoxColumn.DataPropertyName = "Width";
            this.widthDataGridViewTextBoxColumn.HeaderText = "Width";
            this.widthDataGridViewTextBoxColumn.Name = "widthDataGridViewTextBoxColumn";
            // 
            // switchDataGridViewTextBoxColumn
            // 
            this.switchDataGridViewTextBoxColumn.DataPropertyName = "Switch";
            this.switchDataGridViewTextBoxColumn.HeaderText = "Switch";
            this.switchDataGridViewTextBoxColumn.Name = "switchDataGridViewTextBoxColumn";
            // 
            // DGV2
            // 
            this.DGV2.AllowUserToAddRows = false;
            this.DGV2.AllowUserToDeleteRows = false;
            this.DGV2.AllowUserToResizeColumns = false;
            this.DGV2.AllowUserToResizeRows = false;
            this.DGV2.AutoGenerateColumns = false;
            this.DGV2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.zoneDataGridViewTextBoxColumn,
            this.startDataGridViewTextBoxColumn,
            this.endDataGridViewTextBoxColumn,
            this.widthDataGridViewTextBoxColumn2,
            this.switchDataGridViewTextBoxColumn2});
            this.DGV2.DataMember = "Table1";
            this.DGV2.DataSource = this.dataSet2;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle10.NullValue = "<dbnull>";
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DGV2.DefaultCellStyle = dataGridViewCellStyle10;
            this.DGV2.Location = new System.Drawing.Point(15, 15);
            this.DGV2.Margin = new System.Windows.Forms.Padding(6);
            this.DGV2.Name = "DGV2";
            this.DGV2.RowHeadersVisible = false;
            this.DGV2.RowTemplate.Height = 40;
            this.DGV2.Size = new System.Drawing.Size(547, 352);
            this.DGV2.TabIndex = 1;
            this.DGV2.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGV2_CellClick);
            this.DGV2.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.DGV2_CellFormatting);
            this.DGV2.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGV2_CellValueChanged);
            // 
            // zoneDataGridViewTextBoxColumn
            // 
            this.zoneDataGridViewTextBoxColumn.DataPropertyName = "Zone";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.zoneDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle5;
            this.zoneDataGridViewTextBoxColumn.HeaderText = "Zone";
            this.zoneDataGridViewTextBoxColumn.Name = "zoneDataGridViewTextBoxColumn";
            this.zoneDataGridViewTextBoxColumn.ReadOnly = true;
            this.zoneDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // startDataGridViewTextBoxColumn
            // 
            this.startDataGridViewTextBoxColumn.DataPropertyName = "Start";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.startDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle6;
            this.startDataGridViewTextBoxColumn.HeaderText = "Start";
            this.startDataGridViewTextBoxColumn.Name = "startDataGridViewTextBoxColumn";
            this.startDataGridViewTextBoxColumn.ReadOnly = true;
            this.startDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // endDataGridViewTextBoxColumn
            // 
            this.endDataGridViewTextBoxColumn.DataPropertyName = "End";
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.Format = "N0";
            dataGridViewCellStyle7.NullValue = null;
            this.endDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle7;
            this.endDataGridViewTextBoxColumn.HeaderText = "End";
            this.endDataGridViewTextBoxColumn.Name = "endDataGridViewTextBoxColumn";
            this.endDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // widthDataGridViewTextBoxColumn2
            // 
            this.widthDataGridViewTextBoxColumn2.DataPropertyName = "Width";
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.Format = "N0";
            dataGridViewCellStyle8.NullValue = null;
            this.widthDataGridViewTextBoxColumn2.DefaultCellStyle = dataGridViewCellStyle8;
            this.widthDataGridViewTextBoxColumn2.HeaderText = "Width";
            this.widthDataGridViewTextBoxColumn2.Name = "widthDataGridViewTextBoxColumn2";
            this.widthDataGridViewTextBoxColumn2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // switchDataGridViewTextBoxColumn2
            // 
            this.switchDataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.switchDataGridViewTextBoxColumn2.DataPropertyName = "Switch";
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.Format = "N0";
            dataGridViewCellStyle9.NullValue = null;
            this.switchDataGridViewTextBoxColumn2.DefaultCellStyle = dataGridViewCellStyle9;
            this.switchDataGridViewTextBoxColumn2.HeaderText = "Switch";
            this.switchDataGridViewTextBoxColumn2.Name = "switchDataGridViewTextBoxColumn2";
            this.switchDataGridViewTextBoxColumn2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // dataSet2
            // 
            this.dataSet2.DataSetName = "NewDataSet";
            this.dataSet2.Tables.AddRange(new System.Data.DataTable[] {
            this.dataTable2});
            // 
            // dataTable2
            // 
            this.dataTable2.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn4,
            this.dataColumn5,
            this.dataColumn6,
            this.dataColumn7,
            this.dataColumn8});
            this.dataTable2.TableName = "Table1";
            // 
            // dataColumn4
            // 
            this.dataColumn4.Caption = "Section";
            this.dataColumn4.ColumnName = "Zone";
            this.dataColumn4.DataType = typeof(short);
            // 
            // dataColumn5
            // 
            this.dataColumn5.Caption = "Width";
            this.dataColumn5.ColumnName = "Start";
            this.dataColumn5.DataType = typeof(short);
            // 
            // dataColumn6
            // 
            this.dataColumn6.ColumnName = "End";
            this.dataColumn6.DataType = typeof(short);
            // 
            // dataColumn7
            // 
            this.dataColumn7.ColumnName = "Width";
            this.dataColumn7.DataType = typeof(double);
            // 
            // dataColumn8
            // 
            this.dataColumn8.ColumnName = "Switch";
            this.dataColumn8.DataType = typeof(short);
            // 
            // lbPerZone
            // 
            this.lbPerZone.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPerZone.Location = new System.Drawing.Point(317, 379);
            this.lbPerZone.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbPerZone.Name = "lbPerZone";
            this.lbPerZone.Size = new System.Drawing.Size(179, 28);
            this.lbPerZone.TabIndex = 143;
            this.lbPerZone.Text = "Sections per Zone";
            // 
            // tbSectionsPerZone
            // 
            this.tbSectionsPerZone.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSectionsPerZone.Location = new System.Drawing.Point(508, 379);
            this.tbSectionsPerZone.Margin = new System.Windows.Forms.Padding(6);
            this.tbSectionsPerZone.Name = "tbSectionsPerZone";
            this.tbSectionsPerZone.Size = new System.Drawing.Size(54, 29);
            this.tbSectionsPerZone.TabIndex = 7;
            this.tbSectionsPerZone.Text = "9999";
            this.tbSectionsPerZone.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSectionsPerZone.TextChanged += new System.EventHandler(this.tbSectionsPerZone_TextChanged);
            this.tbSectionsPerZone.Enter += new System.EventHandler(this.tbSectionsPerZone_Enter);
            this.tbSectionsPerZone.Validating += new System.ComponentModel.CancelEventHandler(this.tbSectionsPerZone_Validating);
            // 
            // ckZones
            // 
            this.ckZones.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckZones.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckZones.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckZones.Image = global::RateController.Properties.Resources.SectionsNoZones2;
            this.ckZones.Location = new System.Drawing.Point(131, 453);
            this.ckZones.Name = "ckZones";
            this.ckZones.Size = new System.Drawing.Size(113, 100);
            this.ckZones.TabIndex = 2;
            this.ckZones.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckZones.UseVisualStyleBackColor = true;
            this.ckZones.CheckedChanged += new System.EventHandler(this.ckZones_CheckedChanged);
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
            this.btnCancel.Location = new System.Drawing.Point(395, 481);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 72);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // bntOK
            // 
            this.bntOK.BackColor = System.Drawing.Color.Transparent;
            this.bntOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bntOK.FlatAppearance.BorderSize = 0;
            this.bntOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bntOK.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.bntOK.Image = global::RateController.Properties.Resources.OK;
            this.bntOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.bntOK.Location = new System.Drawing.Point(492, 481);
            this.bntOK.Margin = new System.Windows.Forms.Padding(6);
            this.bntOK.Name = "bntOK";
            this.bntOK.Size = new System.Drawing.Size(72, 72);
            this.bntOK.TabIndex = 5;
            this.bntOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.bntOK.UseVisualStyleBackColor = false;
            this.bntOK.Click += new System.EventHandler(this.bntOK_Click);
            // 
            // btnEqual
            // 
            this.btnEqual.BackColor = System.Drawing.Color.Transparent;
            this.btnEqual.FlatAppearance.BorderSize = 0;
            this.btnEqual.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEqual.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEqual.Image = global::RateController.Properties.Resources.Copy;
            this.btnEqual.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnEqual.Location = new System.Drawing.Point(269, 481);
            this.btnEqual.Margin = new System.Windows.Forms.Padding(6);
            this.btnEqual.Name = "btnEqual";
            this.btnEqual.Size = new System.Drawing.Size(101, 72);
            this.btnEqual.TabIndex = 3;
            this.btnEqual.Text = "=1";
            this.btnEqual.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEqual.UseVisualStyleBackColor = false;
            this.btnEqual.Click += new System.EventHandler(this.button1_Click);
            this.btnEqual.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.btnEqual_HelpRequested);
            // 
            // frmSections
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(579, 564);
            this.Controls.Add(this.lbPerZone);
            this.Controls.Add(this.tbSectionsPerZone);
            this.Controls.Add(this.ckZones);
            this.Controls.Add(this.lbFeet);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.bntOK);
            this.Controls.Add(this.btnEqual);
            this.Controls.Add(this.lbNumZones);
            this.Controls.Add(this.tbSectionCount);
            this.Controls.Add(this.lbWidth);
            this.Controls.Add(this.DGV);
            this.Controls.Add(this.DGV2);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSections";
            this.ShowInTaskbar = false;
            this.Text = "Sections";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmSections_FormClosed);
            this.Load += new System.EventHandler(this.frmSections_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DGV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DGV2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView DGV;
        private System.Windows.Forms.Button btnEqual;
        private System.Windows.Forms.Label lbWidth;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button bntOK;
        private System.Data.DataSet dataSet1;
        private System.Data.DataTable dataTable1;
        private System.Data.DataColumn dataColumn1;
        private System.Data.DataColumn dataColumn2;
        private System.Data.DataColumn dataColumn3;
        private System.Windows.Forms.TextBox tbSectionCount;
        private System.Windows.Forms.Label lbNumZones;
        private System.Windows.Forms.Label lbFeet;
        private System.Windows.Forms.DataGridViewTextBoxColumn sectionDataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn sectionDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn widthDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn switchDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn sectionDataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn widthDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn switchDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridView DGV2;
        private System.Data.DataSet dataSet2;
        private System.Data.DataTable dataTable2;
        private System.Data.DataColumn dataColumn4;
        private System.Data.DataColumn dataColumn5;
        private System.Data.DataColumn dataColumn6;
        private System.Data.DataColumn dataColumn7;
        private System.Data.DataColumn dataColumn8;
        private System.Windows.Forms.CheckBox ckZones;
        private System.Windows.Forms.Label lbPerZone;
        private System.Windows.Forms.TextBox tbSectionsPerZone;
        private System.Windows.Forms.DataGridViewTextBoxColumn sectionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn SectionWidth;
        private System.Windows.Forms.DataGridViewTextBoxColumn Switch;
        private System.Windows.Forms.DataGridViewTextBoxColumn zoneDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn startDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn endDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn widthDataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn switchDataGridViewTextBoxColumn2;
    }
}