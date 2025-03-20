namespace RateController.Menu
{
    partial class frmMenuSections
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.DGV = new System.Windows.Forms.DataGridView();
            this.dataSet1 = new System.Data.DataSet();
            this.dataTable1 = new System.Data.DataTable();
            this.dataColumn1 = new System.Data.DataColumn();
            this.dataColumn2 = new System.Data.DataColumn();
            this.dataColumn3 = new System.Data.DataColumn();
            this.DGV2 = new System.Windows.Forms.DataGridView();
            this.zoneDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.startDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.endDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.widthDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.switchDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.lbFeet = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnEqual = new System.Windows.Forms.Button();
            this.lbNumZones = new System.Windows.Forms.Label();
            this.tbSectionCount = new System.Windows.Forms.TextBox();
            this.lbWidth = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbDefaultWidth = new System.Windows.Forms.TextBox();
            this.lbUnits = new System.Windows.Forms.Label();
            this.sectionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.widthDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.switchDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.widthDataGridViewTextBoxColumn,
            this.switchDataGridViewTextBoxColumn});
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
            this.DGV.Location = new System.Drawing.Point(85, 30);
            this.DGV.Margin = new System.Windows.Forms.Padding(11);
            this.DGV.Name = "DGV";
            this.DGV.RowHeadersVisible = false;
            this.DGV.RowTemplate.Height = 40;
            this.DGV.Size = new System.Drawing.Size(369, 307);
            this.DGV.TabIndex = 171;
            this.DGV.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGV_CellClick);
            this.DGV.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.DGV_CellFormatting);
            this.DGV.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGV_CellValueChanged);
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
            this.widthDataGridViewTextBoxColumn1,
            this.switchDataGridViewTextBoxColumn1});
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
            this.DGV2.Location = new System.Drawing.Point(0, 30);
            this.DGV2.Margin = new System.Windows.Forms.Padding(11);
            this.DGV2.Name = "DGV2";
            this.DGV2.RowHeadersVisible = false;
            this.DGV2.RowTemplate.Height = 40;
            this.DGV2.Size = new System.Drawing.Size(540, 307);
            this.DGV2.TabIndex = 172;
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
            this.endDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle7;
            this.endDataGridViewTextBoxColumn.HeaderText = "End";
            this.endDataGridViewTextBoxColumn.Name = "endDataGridViewTextBoxColumn";
            this.endDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // widthDataGridViewTextBoxColumn1
            // 
            this.widthDataGridViewTextBoxColumn1.DataPropertyName = "Width";
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.Format = "N0";
            this.widthDataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle8;
            this.widthDataGridViewTextBoxColumn1.HeaderText = "Width";
            this.widthDataGridViewTextBoxColumn1.Name = "widthDataGridViewTextBoxColumn1";
            this.widthDataGridViewTextBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // switchDataGridViewTextBoxColumn1
            // 
            this.switchDataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.switchDataGridViewTextBoxColumn1.DataPropertyName = "Switch";
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.Format = "N0";
            this.switchDataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle9;
            this.switchDataGridViewTextBoxColumn1.HeaderText = "Switch";
            this.switchDataGridViewTextBoxColumn1.Name = "switchDataGridViewTextBoxColumn1";
            this.switchDataGridViewTextBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
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
            this.lbPerZone.Location = new System.Drawing.Point(140, 436);
            this.lbPerZone.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbPerZone.Name = "lbPerZone";
            this.lbPerZone.Size = new System.Drawing.Size(185, 29);
            this.lbPerZone.TabIndex = 202;
            this.lbPerZone.Text = "Sections per Zone";
            // 
            // tbSectionsPerZone
            // 
            this.tbSectionsPerZone.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSectionsPerZone.Location = new System.Drawing.Point(337, 436);
            this.tbSectionsPerZone.Margin = new System.Windows.Forms.Padding(6);
            this.tbSectionsPerZone.Name = "tbSectionsPerZone";
            this.tbSectionsPerZone.Size = new System.Drawing.Size(54, 29);
            this.tbSectionsPerZone.TabIndex = 198;
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
            this.ckZones.Location = new System.Drawing.Point(178, 518);
            this.ckZones.Name = "ckZones";
            this.ckZones.Size = new System.Drawing.Size(113, 100);
            this.ckZones.TabIndex = 1;
            this.ckZones.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckZones.UseVisualStyleBackColor = true;
            this.ckZones.CheckedChanged += new System.EventHandler(this.ckZones_CheckedChanged);
            // 
            // lbFeet
            // 
            this.lbFeet.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbFeet.Location = new System.Drawing.Point(333, 477);
            this.lbFeet.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbFeet.Name = "lbFeet";
            this.lbFeet.Size = new System.Drawing.Size(132, 29);
            this.lbFeet.TabIndex = 201;
            this.lbFeet.Text = "100.6 FT";
            this.lbFeet.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.btnCancel.Margin = new System.Windows.Forms.Padding(6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 72);
            this.btnCancel.TabIndex = 3;
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
            this.btnOK.Location = new System.Drawing.Point(451, 546);
            this.btnOK.Margin = new System.Windows.Forms.Padding(6);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(72, 72);
            this.btnOK.TabIndex = 4;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.bntOK_Click);
            // 
            // btnEqual
            // 
            this.btnEqual.BackColor = System.Drawing.Color.Transparent;
            this.btnEqual.FlatAppearance.BorderSize = 0;
            this.btnEqual.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEqual.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEqual.Image = global::RateController.Properties.Resources.add;
            this.btnEqual.Location = new System.Drawing.Point(300, 546);
            this.btnEqual.Margin = new System.Windows.Forms.Padding(6);
            this.btnEqual.Name = "btnEqual";
            this.btnEqual.Size = new System.Drawing.Size(72, 72);
            this.btnEqual.TabIndex = 2;
            this.btnEqual.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEqual.UseVisualStyleBackColor = false;
            this.btnEqual.Click += new System.EventHandler(this.btnEqual_Click);
            // 
            // lbNumZones
            // 
            this.lbNumZones.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbNumZones.Location = new System.Drawing.Point(140, 354);
            this.lbNumZones.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbNumZones.Name = "lbNumZones";
            this.lbNumZones.Size = new System.Drawing.Size(185, 29);
            this.lbNumZones.TabIndex = 200;
            this.lbNumZones.Text = "Number of Sections";
            // 
            // tbSectionCount
            // 
            this.tbSectionCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSectionCount.Location = new System.Drawing.Point(337, 354);
            this.tbSectionCount.Margin = new System.Windows.Forms.Padding(6);
            this.tbSectionCount.Name = "tbSectionCount";
            this.tbSectionCount.Size = new System.Drawing.Size(54, 29);
            this.tbSectionCount.TabIndex = 197;
            this.tbSectionCount.Text = "9999";
            this.tbSectionCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSectionCount.TextChanged += new System.EventHandler(this.tbSectionCount_TextChanged);
            this.tbSectionCount.Enter += new System.EventHandler(this.tbSectionCount_Enter);
            this.tbSectionCount.Validating += new System.ComponentModel.CancelEventHandler(this.tbSectionCount_Validating);
            // 
            // lbWidth
            // 
            this.lbWidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbWidth.Location = new System.Drawing.Point(140, 477);
            this.lbWidth.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbWidth.Name = "lbWidth";
            this.lbWidth.Size = new System.Drawing.Size(187, 29);
            this.lbWidth.TabIndex = 0;
            this.lbWidth.Text = "Width:  1200 Inches";
            this.lbWidth.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(140, 395);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(185, 29);
            this.label1.TabIndex = 204;
            this.label1.Text = "Default Width";
            // 
            // tbDefaultWidth
            // 
            this.tbDefaultWidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDefaultWidth.Location = new System.Drawing.Point(337, 395);
            this.tbDefaultWidth.Margin = new System.Windows.Forms.Padding(6);
            this.tbDefaultWidth.Name = "tbDefaultWidth";
            this.tbDefaultWidth.Size = new System.Drawing.Size(54, 29);
            this.tbDefaultWidth.TabIndex = 203;
            this.tbDefaultWidth.Text = "9999";
            this.tbDefaultWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbDefaultWidth.TextChanged += new System.EventHandler(this.tbSectionCount_TextChanged);
            this.tbDefaultWidth.Enter += new System.EventHandler(this.tbDefaultWidth_Enter);
            this.tbDefaultWidth.Validating += new System.ComponentModel.CancelEventHandler(this.tbDefaultWidth_Validating);
            // 
            // lbUnits
            // 
            this.lbUnits.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbUnits.Location = new System.Drawing.Point(403, 395);
            this.lbUnits.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbUnits.Name = "lbUnits";
            this.lbUnits.Size = new System.Drawing.Size(100, 29);
            this.lbUnits.TabIndex = 205;
            this.lbUnits.Text = "Inches";
            this.lbUnits.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            // widthDataGridViewTextBoxColumn
            // 
            this.widthDataGridViewTextBoxColumn.DataPropertyName = "Width";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Format = "N1";
            dataGridViewCellStyle2.NullValue = null;
            this.widthDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.widthDataGridViewTextBoxColumn.HeaderText = "Width";
            this.widthDataGridViewTextBoxColumn.Name = "widthDataGridViewTextBoxColumn";
            this.widthDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // switchDataGridViewTextBoxColumn
            // 
            this.switchDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.switchDataGridViewTextBoxColumn.DataPropertyName = "Switch";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.switchDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle3;
            this.switchDataGridViewTextBoxColumn.HeaderText = "Switch";
            this.switchDataGridViewTextBoxColumn.Name = "switchDataGridViewTextBoxColumn";
            this.switchDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // frmMenuSections
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 630);
            this.Controls.Add(this.lbUnits);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbDefaultWidth);
            this.Controls.Add(this.lbPerZone);
            this.Controls.Add(this.tbSectionsPerZone);
            this.Controls.Add(this.ckZones);
            this.Controls.Add(this.lbFeet);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnEqual);
            this.Controls.Add(this.lbNumZones);
            this.Controls.Add(this.tbSectionCount);
            this.Controls.Add(this.lbWidth);
            this.Controls.Add(this.DGV);
            this.Controls.Add(this.DGV2);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "frmMenuSections";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuSections";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMenuSections_FormClosed);
            this.Load += new System.EventHandler(this.frmMenuSections_Load);
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
        private System.Windows.Forms.DataGridView DGV2;
        private System.Windows.Forms.Label lbPerZone;
        private System.Windows.Forms.TextBox tbSectionsPerZone;
        private System.Windows.Forms.CheckBox ckZones;
        private System.Windows.Forms.Label lbFeet;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnEqual;
        private System.Windows.Forms.Label lbNumZones;
        private System.Windows.Forms.TextBox tbSectionCount;
        private System.Windows.Forms.Label lbWidth;
        private System.Data.DataSet dataSet1;
        private System.Data.DataTable dataTable1;
        private System.Data.DataColumn dataColumn1;
        private System.Data.DataColumn dataColumn2;
        private System.Data.DataColumn dataColumn3;
        private System.Data.DataSet dataSet2;
        private System.Data.DataTable dataTable2;
        private System.Data.DataColumn dataColumn4;
        private System.Data.DataColumn dataColumn5;
        private System.Data.DataColumn dataColumn6;
        private System.Data.DataColumn dataColumn7;
        private System.Data.DataColumn dataColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn zoneDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn startDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn endDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn widthDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn switchDataGridViewTextBoxColumn1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbDefaultWidth;
        private System.Windows.Forms.Label lbUnits;
        private System.Windows.Forms.DataGridViewTextBoxColumn sectionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn widthDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn switchDataGridViewTextBoxColumn;
    }
}