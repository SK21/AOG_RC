namespace RateController.Menu
{
    partial class frmMenuRelays
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataSet1 = new System.Data.DataSet();
            this.dataTable1 = new System.Data.DataTable();
            this.dataRelay = new System.Data.DataColumn();
            this.dataType = new System.Data.DataColumn();
            this.dataSection = new System.Data.DataColumn();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnRenumber = new System.Windows.Forms.Button();
            this.lbModule = new System.Windows.Forms.Label();
            this.ModuleIndicator = new System.Windows.Forms.Label();
            this.cbModules = new System.Windows.Forms.ComboBox();
            this.DGV = new System.Windows.Forms.DataGridView();
            this.ColRelay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ColSection = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.dataSection});
            this.dataTable1.TableName = "Table1";
            // 
            // dataRelay
            // 
            this.dataRelay.Caption = "Relay";
            this.dataRelay.ColumnName = "cRelay";
            this.dataRelay.DataType = typeof(short);
            // 
            // dataType
            // 
            this.dataType.Caption = "Type";
            this.dataType.ColumnName = "cType";
            // 
            // dataSection
            // 
            this.dataSection.Caption = "Section";
            this.dataSection.ColumnName = "cSection";
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
            this.btnCancel.TabIndex = 176;
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
            this.btnOK.TabIndex = 175;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnReset
            // 
            this.btnReset.BackColor = System.Drawing.Color.Transparent;
            this.btnReset.FlatAppearance.BorderSize = 0;
            this.btnReset.FlatAppearance.MouseDownBackColor = System.Drawing.Color.LightGreen;
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReset.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReset.Image = global::RateController.Properties.Resources.Update;
            this.btnReset.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnReset.Location = new System.Drawing.Point(214, 543);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(72, 72);
            this.btnReset.TabIndex = 178;
            this.btnReset.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnReset.UseVisualStyleBackColor = false;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnRenumber
            // 
            this.btnRenumber.BackColor = System.Drawing.Color.Transparent;
            this.btnRenumber.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnRenumber.FlatAppearance.BorderSize = 0;
            this.btnRenumber.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRenumber.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnRenumber.Image = global::RateController.Properties.Resources.add;
            this.btnRenumber.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnRenumber.Location = new System.Drawing.Point(297, 543);
            this.btnRenumber.Margin = new System.Windows.Forms.Padding(6);
            this.btnRenumber.Name = "btnRenumber";
            this.btnRenumber.Size = new System.Drawing.Size(72, 72);
            this.btnRenumber.TabIndex = 177;
            this.btnRenumber.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnRenumber.UseVisualStyleBackColor = false;
            this.btnRenumber.Click += new System.EventHandler(this.btnRenumber_Click);
            // 
            // lbModule
            // 
            this.lbModule.AutoSize = true;
            this.lbModule.Location = new System.Drawing.Point(148, 15);
            this.lbModule.Name = "lbModule";
            this.lbModule.Size = new System.Drawing.Size(74, 24);
            this.lbModule.TabIndex = 181;
            this.lbModule.Text = "Module";
            // 
            // ModuleIndicator
            // 
            this.ModuleIndicator.BackColor = System.Drawing.SystemColors.Control;
            this.ModuleIndicator.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ModuleIndicator.Image = global::RateController.Properties.Resources.Off;
            this.ModuleIndicator.Location = new System.Drawing.Point(324, 9);
            this.ModuleIndicator.Name = "ModuleIndicator";
            this.ModuleIndicator.Size = new System.Drawing.Size(41, 37);
            this.ModuleIndicator.TabIndex = 0;
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
            this.cbModules.Location = new System.Drawing.Point(237, 12);
            this.cbModules.Name = "cbModules";
            this.cbModules.Size = new System.Drawing.Size(61, 31);
            this.cbModules.TabIndex = 179;
            this.cbModules.SelectedIndexChanged += new System.EventHandler(this.cbModules_SelectedIndexChanged);
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
            this.ColRelay,
            this.ColType,
            this.ColSection});
            this.DGV.DataMember = "Table1";
            this.DGV.DataSource = this.dataSet1;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.NullValue = "<dbnull>";
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DGV.DefaultCellStyle = dataGridViewCellStyle3;
            this.DGV.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
            this.DGV.Location = new System.Drawing.Point(69, 72);
            this.DGV.Margin = new System.Windows.Forms.Padding(11);
            this.DGV.Name = "DGV";
            this.DGV.RowHeadersVisible = false;
            this.DGV.RowTemplate.Height = 40;
            this.DGV.Size = new System.Drawing.Size(391, 454);
            this.DGV.TabIndex = 0;
            this.DGV.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGV_CellClick);
            this.DGV.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGV_CellValueChanged);
            this.DGV.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.DGV_DataError);
            // 
            // ColRelay
            // 
            this.ColRelay.DataPropertyName = "cRelay";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColRelay.DefaultCellStyle = dataGridViewCellStyle1;
            this.ColRelay.HeaderText = "Relay";
            this.ColRelay.Name = "ColRelay";
            this.ColRelay.ReadOnly = true;
            // 
            // ColType
            // 
            this.ColType.DataPropertyName = "cType";
            this.ColType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ColType.HeaderText = "Type";
            this.ColType.Items.AddRange(new object[] {
            "a",
            "b",
            "c",
            "d",
            "e"});
            this.ColType.Name = "ColType";
            this.ColType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ColType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ColType.Width = 150;
            // 
            // ColSection
            // 
            this.ColSection.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColSection.DataPropertyName = "cSection";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColSection.DefaultCellStyle = dataGridViewCellStyle2;
            this.ColSection.HeaderText = "Number";
            this.ColSection.Name = "ColSection";
            // 
            // frmMenuRelays
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 630);
            this.Controls.Add(this.DGV);
            this.Controls.Add(this.lbModule);
            this.Controls.Add(this.ModuleIndicator);
            this.Controls.Add(this.cbModules);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnRenumber);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "frmMenuRelays";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuRelays";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMenuRelays_FormClosed);
            this.Load += new System.EventHandler(this.frmMenuRelays_Load);
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
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnRenumber;
        private System.Windows.Forms.Label lbModule;
        private System.Windows.Forms.Label ModuleIndicator;
        private System.Windows.Forms.ComboBox cbModules;
        private System.Windows.Forms.DataGridView DGV;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColRelay;
        private System.Windows.Forms.DataGridViewComboBoxColumn ColType;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColSection;
    }
}