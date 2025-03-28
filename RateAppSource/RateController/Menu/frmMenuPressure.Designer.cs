namespace RateController.Menu
{
    partial class frmMenuPressure
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.DGV = new System.Windows.Forms.DataGridView();
            this.cReadingDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cRawDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cKnownPressureDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataSet1 = new System.Data.DataSet();
            this.dataTable1 = new System.Data.DataTable();
            this.dataRelay = new System.Data.DataColumn();
            this.dataType = new System.Data.DataColumn();
            this.dataSection = new System.Data.DataColumn();
            this.dataColumn1 = new System.Data.DataColumn();
            this.lbModule = new System.Windows.Forms.Label();
            this.ModuleIndicator = new System.Windows.Forms.Label();
            this.cbModules = new System.Windows.Forms.ComboBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lbCalSlopeDisplay = new System.Windows.Forms.Label();
            this.tbSlope = new System.Windows.Forms.TextBox();
            this.lbCalSlope = new System.Windows.Forms.Label();
            this.lbCalIntercept = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbIntercept = new System.Windows.Forms.TextBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.ckPressure = new System.Windows.Forms.CheckBox();
            this.grpSensor = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbPressure = new System.Windows.Forms.TextBox();
            this.lbRaw = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbMin = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnNew = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.DGV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).BeginInit();
            this.grpSensor.SuspendLayout();
            this.groupBox1.SuspendLayout();
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
            this.cReadingDataGridViewTextBoxColumn,
            this.cRawDataGridViewTextBoxColumn,
            this.cKnownPressureDataGridViewTextBoxColumn,
            this.ID});
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
            this.DGV.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
            this.DGV.Location = new System.Drawing.Point(14, 27);
            this.DGV.Margin = new System.Windows.Forms.Padding(11);
            this.DGV.Name = "DGV";
            this.DGV.ReadOnly = true;
            this.DGV.RowHeadersVisible = false;
            this.DGV.RowTemplate.Height = 40;
            this.DGV.Size = new System.Drawing.Size(391, 265);
            this.DGV.TabIndex = 182;
            this.DGV.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.DGV_DataError);
            // 
            // cReadingDataGridViewTextBoxColumn
            // 
            this.cReadingDataGridViewTextBoxColumn.DataPropertyName = "cReading";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.Format = "N0";
            dataGridViewCellStyle1.NullValue = null;
            this.cReadingDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle1;
            this.cReadingDataGridViewTextBoxColumn.HeaderText = "Reading";
            this.cReadingDataGridViewTextBoxColumn.Name = "cReadingDataGridViewTextBoxColumn";
            this.cReadingDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // cRawDataGridViewTextBoxColumn
            // 
            this.cRawDataGridViewTextBoxColumn.DataPropertyName = "cRaw";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.Format = "N0";
            dataGridViewCellStyle2.NullValue = null;
            this.cRawDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.cRawDataGridViewTextBoxColumn.HeaderText = "Raw Data";
            this.cRawDataGridViewTextBoxColumn.Name = "cRawDataGridViewTextBoxColumn";
            this.cRawDataGridViewTextBoxColumn.ReadOnly = true;
            this.cRawDataGridViewTextBoxColumn.Width = 150;
            // 
            // cKnownPressureDataGridViewTextBoxColumn
            // 
            this.cKnownPressureDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cKnownPressureDataGridViewTextBoxColumn.DataPropertyName = "cKnownPressure";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.Format = "N1";
            dataGridViewCellStyle3.NullValue = null;
            this.cKnownPressureDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle3;
            this.cKnownPressureDataGridViewTextBoxColumn.HeaderText = "Pressure";
            this.cKnownPressureDataGridViewTextBoxColumn.Name = "cKnownPressureDataGridViewTextBoxColumn";
            this.cKnownPressureDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // ID
            // 
            this.ID.DataPropertyName = "ID";
            this.ID.HeaderText = "ID";
            this.ID.Name = "ID";
            this.ID.ReadOnly = true;
            this.ID.Visible = false;
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
            this.dataRelay.AutoIncrement = true;
            this.dataRelay.Caption = "Reading";
            this.dataRelay.ColumnName = "cReading";
            this.dataRelay.DataType = typeof(short);
            this.dataRelay.ReadOnly = true;
            // 
            // dataType
            // 
            this.dataType.Caption = "Raw Value";
            this.dataType.ColumnName = "cRaw";
            this.dataType.DataType = typeof(short);
            this.dataType.DefaultValue = ((short)(0));
            // 
            // dataSection
            // 
            this.dataSection.Caption = "Pressure";
            this.dataSection.ColumnName = "cKnownPressure";
            this.dataSection.DataType = typeof(double);
            this.dataSection.DefaultValue = 0D;
            // 
            // dataColumn1
            // 
            this.dataColumn1.ColumnName = "ID";
            this.dataColumn1.DataType = typeof(int);
            // 
            // lbModule
            // 
            this.lbModule.AutoSize = true;
            this.lbModule.Location = new System.Drawing.Point(146, 14);
            this.lbModule.Name = "lbModule";
            this.lbModule.Size = new System.Drawing.Size(74, 24);
            this.lbModule.TabIndex = 189;
            this.lbModule.Text = "Module";
            // 
            // ModuleIndicator
            // 
            this.ModuleIndicator.BackColor = System.Drawing.SystemColors.Control;
            this.ModuleIndicator.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ModuleIndicator.Image = global::RateController.Properties.Resources.Off;
            this.ModuleIndicator.Location = new System.Drawing.Point(322, 8);
            this.ModuleIndicator.Name = "ModuleIndicator";
            this.ModuleIndicator.Size = new System.Drawing.Size(41, 37);
            this.ModuleIndicator.TabIndex = 183;
            this.ModuleIndicator.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbModules
            // 
            this.cbModules.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbModules.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbModules.FormattingEnabled = true;
            this.cbModules.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7"});
            this.cbModules.Location = new System.Drawing.Point(235, 11);
            this.cbModules.Name = "cbModules";
            this.cbModules.Size = new System.Drawing.Size(61, 31);
            this.cbModules.TabIndex = 188;
            this.cbModules.SelectedIndexChanged += new System.EventHandler(this.cbModules_SelectedIndexChanged);
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
            this.btnCancel.Location = new System.Drawing.Point(380, 603);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(70, 63);
            this.btnCancel.TabIndex = 185;
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
            this.btnOK.Location = new System.Drawing.Point(458, 603);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(70, 63);
            this.btnOK.TabIndex = 184;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lbCalSlopeDisplay
            // 
            this.lbCalSlopeDisplay.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCalSlopeDisplay.Location = new System.Drawing.Point(14, 307);
            this.lbCalSlopeDisplay.Name = "lbCalSlopeDisplay";
            this.lbCalSlopeDisplay.Size = new System.Drawing.Size(92, 23);
            this.lbCalSlopeDisplay.TabIndex = 191;
            this.lbCalSlopeDisplay.Text = "Slope";
            // 
            // tbSlope
            // 
            this.tbSlope.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSlope.Location = new System.Drawing.Point(101, 25);
            this.tbSlope.MaxLength = 8;
            this.tbSlope.Name = "tbSlope";
            this.tbSlope.Size = new System.Drawing.Size(80, 30);
            this.tbSlope.TabIndex = 190;
            this.tbSlope.Text = "0";
            this.tbSlope.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSlope.TextChanged += new System.EventHandler(this.tbSlope_TextChanged);
            this.tbSlope.Enter += new System.EventHandler(this.tbSlope_Enter);
            // 
            // lbCalSlope
            // 
            this.lbCalSlope.BackColor = System.Drawing.Color.Transparent;
            this.lbCalSlope.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbCalSlope.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCalSlope.Location = new System.Drawing.Point(112, 303);
            this.lbCalSlope.Name = "lbCalSlope";
            this.lbCalSlope.Size = new System.Drawing.Size(80, 30);
            this.lbCalSlope.TabIndex = 210;
            this.lbCalSlope.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbCalIntercept
            // 
            this.lbCalIntercept.BackColor = System.Drawing.Color.Transparent;
            this.lbCalIntercept.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbCalIntercept.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCalIntercept.Location = new System.Drawing.Point(325, 303);
            this.lbCalIntercept.Name = "lbCalIntercept";
            this.lbCalIntercept.Size = new System.Drawing.Size(80, 30);
            this.lbCalIntercept.TabIndex = 212;
            this.lbCalIntercept.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(226, 307);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 23);
            this.label2.TabIndex = 211;
            this.label2.Text = "Intercept";
            // 
            // tbIntercept
            // 
            this.tbIntercept.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbIntercept.Location = new System.Drawing.Point(325, 25);
            this.tbIntercept.MaxLength = 8;
            this.tbIntercept.Name = "tbIntercept";
            this.tbIntercept.Size = new System.Drawing.Size(80, 30);
            this.tbIntercept.TabIndex = 215;
            this.tbIntercept.Text = "0";
            this.tbIntercept.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbIntercept.TextChanged += new System.EventHandler(this.tbSlope_TextChanged);
            this.tbIntercept.Enter += new System.EventHandler(this.tbIntercept_Enter);
            // 
            // btnDelete
            // 
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Image = global::RateController.Properties.Resources.Trash;
            this.btnDelete.Location = new System.Drawing.Point(204, 603);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(70, 63);
            this.btnDelete.TabIndex = 216;
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.FlatAppearance.BorderSize = 0;
            this.btnCopy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCopy.Image = global::RateController.Properties.Resources.copy1;
            this.btnCopy.Location = new System.Drawing.Point(292, 603);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(70, 63);
            this.btnCopy.TabIndex = 217;
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // ckPressure
            // 
            this.ckPressure.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckPressure.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckPressure.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckPressure.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckPressure.Location = new System.Drawing.Point(12, 594);
            this.ckPressure.Name = "ckPressure";
            this.ckPressure.Size = new System.Drawing.Size(82, 72);
            this.ckPressure.TabIndex = 218;
            this.ckPressure.Text = "Show";
            this.ckPressure.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckPressure.UseVisualStyleBackColor = true;
            this.ckPressure.CheckedChanged += new System.EventHandler(this.tbSlope_TextChanged);
            // 
            // grpSensor
            // 
            this.grpSensor.Controls.Add(this.label7);
            this.grpSensor.Controls.Add(this.tbPressure);
            this.grpSensor.Controls.Add(this.lbRaw);
            this.grpSensor.Controls.Add(this.label4);
            this.grpSensor.Controls.Add(this.DGV);
            this.grpSensor.Controls.Add(this.lbCalSlope);
            this.grpSensor.Controls.Add(this.lbCalSlopeDisplay);
            this.grpSensor.Controls.Add(this.label2);
            this.grpSensor.Controls.Add(this.lbCalIntercept);
            this.grpSensor.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpSensor.Location = new System.Drawing.Point(54, 203);
            this.grpSensor.Name = "grpSensor";
            this.grpSensor.Size = new System.Drawing.Size(418, 372);
            this.grpSensor.TabIndex = 219;
            this.grpSensor.TabStop = false;
            this.grpSensor.Text = "Calibration";
            this.grpSensor.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox1_Paint);
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(226, 340);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(93, 23);
            this.label7.TabIndex = 218;
            this.label7.Text = "Pressure";
            // 
            // tbPressure
            // 
            this.tbPressure.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPressure.Location = new System.Drawing.Point(325, 336);
            this.tbPressure.MaxLength = 8;
            this.tbPressure.Name = "tbPressure";
            this.tbPressure.Size = new System.Drawing.Size(80, 30);
            this.tbPressure.TabIndex = 217;
            this.tbPressure.Text = "0";
            this.tbPressure.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbPressure.Enter += new System.EventHandler(this.tbPressure_Enter);
            // 
            // lbRaw
            // 
            this.lbRaw.BackColor = System.Drawing.Color.Transparent;
            this.lbRaw.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbRaw.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRaw.Location = new System.Drawing.Point(112, 336);
            this.lbRaw.Name = "lbRaw";
            this.lbRaw.Size = new System.Drawing.Size(80, 30);
            this.lbRaw.TabIndex = 214;
            this.lbRaw.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(14, 340);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 23);
            this.label4.TabIndex = 213;
            this.label4.Text = "Raw Data";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tbMin);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.tbSlope);
            this.groupBox1.Controls.Add(this.tbIntercept);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(54, 74);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(418, 114);
            this.groupBox1.TabIndex = 220;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Setting";
            this.groupBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox1_Paint);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(69, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 23);
            this.label1.TabIndex = 220;
            this.label1.Text = "Minimum Raw Data";
            // 
            // tbMin
            // 
            this.tbMin.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbMin.Location = new System.Drawing.Point(251, 70);
            this.tbMin.MaxLength = 8;
            this.tbMin.Name = "tbMin";
            this.tbMin.Size = new System.Drawing.Size(80, 30);
            this.tbMin.TabIndex = 219;
            this.tbMin.Text = "0";
            this.tbMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbMin.TextChanged += new System.EventHandler(this.tbSlope_TextChanged);
            this.tbMin.Enter += new System.EventHandler(this.tbMin_Enter);
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(21, 29);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 23);
            this.label5.TabIndex = 216;
            this.label5.Text = "Slope";
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(226, 29);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(93, 23);
            this.label6.TabIndex = 218;
            this.label6.Text = "Intercept";
            // 
            // btnNew
            // 
            this.btnNew.FlatAppearance.BorderSize = 0;
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Image = global::RateController.Properties.Resources.NewFile;
            this.btnNew.Location = new System.Drawing.Point(116, 603);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(70, 63);
            this.btnNew.TabIndex = 221;
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // frmMenuPressure
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 678);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grpSensor);
            this.Controls.Add(this.ckPressure);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.lbModule);
            this.Controls.Add(this.ModuleIndicator);
            this.Controls.Add(this.cbModules);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "frmMenuPressure";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuPressure";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMenuPressure_FormClosed);
            this.Load += new System.EventHandler(this.frmMenuPressure_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DGV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).EndInit();
            this.grpSensor.ResumeLayout(false);
            this.grpSensor.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView DGV;
        private System.Windows.Forms.Label lbModule;
        private System.Windows.Forms.Label ModuleIndicator;
        private System.Windows.Forms.ComboBox cbModules;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Data.DataSet dataSet1;
        private System.Data.DataTable dataTable1;
        private System.Data.DataColumn dataRelay;
        private System.Data.DataColumn dataType;
        private System.Data.DataColumn dataSection;
        private System.Windows.Forms.Label lbCalSlopeDisplay;
        private System.Windows.Forms.TextBox tbSlope;
        private System.Windows.Forms.Label lbCalSlope;
        private System.Windows.Forms.Label lbCalIntercept;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbIntercept;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.CheckBox ckPressure;
        private System.Windows.Forms.GroupBox grpSensor;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbMin;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbPressure;
        private System.Windows.Forms.Label lbRaw;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.DataGridViewTextBoxColumn cReadingDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn cRawDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn cKnownPressureDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Data.DataColumn dataColumn1;
        private System.Windows.Forms.Timer timer1;
    }
}