namespace RateController.Menu
{
    partial class frmMenuRateMap
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
            this.tbMapName = new System.Windows.Forms.TextBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.colorComboBox = new System.Windows.Forms.ComboBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnCreateZone = new System.Windows.Forms.Button();
            this.lbP4 = new System.Windows.Forms.Label();
            this.tbP4 = new System.Windows.Forms.TextBox();
            this.lbP3 = new System.Windows.Forms.Label();
            this.ckEdit = new System.Windows.Forms.CheckBox();
            this.tbP3 = new System.Windows.Forms.TextBox();
            this.lbP2 = new System.Windows.Forms.Label();
            this.tbP2 = new System.Windows.Forms.TextBox();
            this.lbP1 = new System.Windows.Forms.Label();
            this.tbP1 = new System.Windows.Forms.TextBox();
            this.tbName = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ckFullScreen = new System.Windows.Forms.CheckBox();
            this.VSzoom = new System.Windows.Forms.VScrollBar();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.ckEnable = new System.Windows.Forms.CheckBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.lbArea = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // tbMapName
            // 
            this.tbMapName.Location = new System.Drawing.Point(6, 28);
            this.tbMapName.Name = "tbMapName";
            this.tbMapName.Size = new System.Drawing.Size(131, 29);
            this.tbMapName.TabIndex = 17;
            this.tbMapName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbMapName.TextChanged += new System.EventHandler(this.tbMapName_TextChanged);
            // 
            // btnLoad
            // 
            this.btnLoad.FlatAppearance.BorderSize = 0;
            this.btnLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoad.Image = global::RateController.Properties.Resources.VehFileLoad;
            this.btnLoad.Location = new System.Drawing.Point(30, 155);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(82, 64);
            this.btnLoad.TabIndex = 14;
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnNew
            // 
            this.btnNew.FlatAppearance.BorderSize = 0;
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Image = global::RateController.Properties.Resources.NewFile;
            this.btnNew.Location = new System.Drawing.Point(30, 76);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(82, 64);
            this.btnNew.TabIndex = 16;
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(8, 12);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(89, 64);
            this.btnImport.TabIndex = 342;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbArea);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.colorComboBox);
            this.groupBox1.Controls.Add(this.btnDelete);
            this.groupBox1.Controls.Add(this.btnCreateZone);
            this.groupBox1.Controls.Add(this.lbP4);
            this.groupBox1.Controls.Add(this.tbP4);
            this.groupBox1.Controls.Add(this.lbP3);
            this.groupBox1.Controls.Add(this.ckEdit);
            this.groupBox1.Controls.Add(this.tbP3);
            this.groupBox1.Controls.Add(this.lbP2);
            this.groupBox1.Controls.Add(this.tbP2);
            this.groupBox1.Controls.Add(this.lbP1);
            this.groupBox1.Controls.Add(this.tbP1);
            this.groupBox1.Controls.Add(this.tbName);
            this.groupBox1.Location = new System.Drawing.Point(255, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(278, 318);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Zone";
            this.groupBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox1_Paint);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 207);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 24);
            this.label1.TabIndex = 341;
            this.label1.Text = "Color";
            // 
            // colorComboBox
            // 
            this.colorComboBox.FormattingEnabled = true;
            this.colorComboBox.Items.AddRange(new object[] {
            "Blue",
            "Green",
            "Red ",
            "Purple",
            "Orange"});
            this.colorComboBox.Location = new System.Drawing.Point(80, 204);
            this.colorComboBox.Name = "colorComboBox";
            this.colorComboBox.Size = new System.Drawing.Size(186, 32);
            this.colorComboBox.TabIndex = 340;
            // 
            // btnDelete
            // 
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Image = global::RateController.Properties.Resources.Trash;
            this.btnDelete.Location = new System.Drawing.Point(10, 242);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(82, 64);
            this.btnDelete.TabIndex = 12;
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnCreateZone
            // 
            this.btnCreateZone.FlatAppearance.BorderSize = 0;
            this.btnCreateZone.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreateZone.Image = global::RateController.Properties.Resources.OK;
            this.btnCreateZone.Location = new System.Drawing.Point(184, 242);
            this.btnCreateZone.Name = "btnCreateZone";
            this.btnCreateZone.Size = new System.Drawing.Size(82, 64);
            this.btnCreateZone.TabIndex = 2;
            this.btnCreateZone.UseVisualStyleBackColor = true;
            this.btnCreateZone.Click += new System.EventHandler(this.btnCreateZone_Click);
            // 
            // lbP4
            // 
            this.lbP4.Location = new System.Drawing.Point(6, 171);
            this.lbP4.Name = "lbP4";
            this.lbP4.Size = new System.Drawing.Size(116, 24);
            this.lbP4.TabIndex = 11;
            this.lbP4.Text = "Product D";
            // 
            // tbP4
            // 
            this.tbP4.Location = new System.Drawing.Point(199, 169);
            this.tbP4.Name = "tbP4";
            this.tbP4.Size = new System.Drawing.Size(67, 29);
            this.tbP4.TabIndex = 10;
            this.tbP4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbP4.TextChanged += new System.EventHandler(this.tbName_TextChanged);
            this.tbP4.Enter += new System.EventHandler(this.tbP4_Enter);
            this.tbP4.Validating += new System.ComponentModel.CancelEventHandler(this.tbP4_Validating);
            // 
            // lbP3
            // 
            this.lbP3.Location = new System.Drawing.Point(6, 136);
            this.lbP3.Name = "lbP3";
            this.lbP3.Size = new System.Drawing.Size(116, 24);
            this.lbP3.TabIndex = 9;
            this.lbP3.Text = "Product C";
            // 
            // ckEdit
            // 
            this.ckEdit.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckEdit.FlatAppearance.BorderSize = 0;
            this.ckEdit.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckEdit.Image = global::RateController.Properties.Resources.FileEditName;
            this.ckEdit.Location = new System.Drawing.Point(97, 242);
            this.ckEdit.Name = "ckEdit";
            this.ckEdit.Size = new System.Drawing.Size(82, 64);
            this.ckEdit.TabIndex = 339;
            this.ckEdit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckEdit.UseVisualStyleBackColor = true;
            this.ckEdit.CheckedChanged += new System.EventHandler(this.ckEdit_CheckedChanged);
            // 
            // tbP3
            // 
            this.tbP3.Location = new System.Drawing.Point(199, 134);
            this.tbP3.Name = "tbP3";
            this.tbP3.Size = new System.Drawing.Size(67, 29);
            this.tbP3.TabIndex = 8;
            this.tbP3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbP3.TextChanged += new System.EventHandler(this.tbName_TextChanged);
            this.tbP3.Enter += new System.EventHandler(this.tbP3_Enter);
            this.tbP3.Validating += new System.ComponentModel.CancelEventHandler(this.tbP3_Validating);
            // 
            // lbP2
            // 
            this.lbP2.Location = new System.Drawing.Point(6, 101);
            this.lbP2.Name = "lbP2";
            this.lbP2.Size = new System.Drawing.Size(116, 24);
            this.lbP2.TabIndex = 7;
            this.lbP2.Text = "Product B";
            // 
            // tbP2
            // 
            this.tbP2.Location = new System.Drawing.Point(199, 99);
            this.tbP2.Name = "tbP2";
            this.tbP2.Size = new System.Drawing.Size(67, 29);
            this.tbP2.TabIndex = 6;
            this.tbP2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbP2.TextChanged += new System.EventHandler(this.tbName_TextChanged);
            this.tbP2.Enter += new System.EventHandler(this.tbP2_Enter);
            this.tbP2.Validating += new System.ComponentModel.CancelEventHandler(this.tbP2_Validating);
            // 
            // lbP1
            // 
            this.lbP1.Location = new System.Drawing.Point(6, 66);
            this.lbP1.Name = "lbP1";
            this.lbP1.Size = new System.Drawing.Size(116, 24);
            this.lbP1.TabIndex = 5;
            this.lbP1.Text = "Product A";
            // 
            // tbP1
            // 
            this.tbP1.Location = new System.Drawing.Point(199, 64);
            this.tbP1.Name = "tbP1";
            this.tbP1.Size = new System.Drawing.Size(67, 29);
            this.tbP1.TabIndex = 4;
            this.tbP1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbP1.TextChanged += new System.EventHandler(this.tbName_TextChanged);
            this.tbP1.Enter += new System.EventHandler(this.tbP1_Enter);
            this.tbP1.Validating += new System.ComponentModel.CancelEventHandler(this.tbP1_Validating);
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(10, 29);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(169, 29);
            this.tbName.TabIndex = 2;
            this.tbName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbName.TextChanged += new System.EventHandler(this.tbName_TextChanged);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnSave.Image = global::RateController.Properties.Resources.Save;
            this.btnSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSave.Location = new System.Drawing.Point(35, 234);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(72, 72);
            this.btnSave.TabIndex = 347;
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tbMapName);
            this.groupBox2.Controls.Add(this.btnSave);
            this.groupBox2.Controls.Add(this.btnNew);
            this.groupBox2.Controls.Add(this.btnLoad);
            this.groupBox2.Location = new System.Drawing.Point(106, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(143, 318);
            this.groupBox2.TabIndex = 348;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Map";
            this.groupBox2.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox1_Paint);
            // 
            // ckFullScreen
            // 
            this.ckFullScreen.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckFullScreen.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckFullScreen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckFullScreen.Location = new System.Drawing.Point(8, 172);
            this.ckFullScreen.Name = "ckFullScreen";
            this.ckFullScreen.Size = new System.Drawing.Size(89, 64);
            this.ckFullScreen.TabIndex = 348;
            this.ckFullScreen.Text = "Full Screen";
            this.ckFullScreen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckFullScreen.UseVisualStyleBackColor = true;
            this.ckFullScreen.CheckedChanged += new System.EventHandler(this.ckFullScreen_CheckedChanged);
            // 
            // VSzoom
            // 
            this.VSzoom.LargeChange = 1;
            this.VSzoom.Location = new System.Drawing.Point(9, 324);
            this.VSzoom.Name = "VSzoom";
            this.VSzoom.Size = new System.Drawing.Size(45, 294);
            this.VSzoom.TabIndex = 349;
            this.VSzoom.Scroll += new System.Windows.Forms.ScrollEventHandler(this.VSzoom_Scroll);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(66, 324);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(467, 294);
            this.pictureBox1.TabIndex = 350;
            this.pictureBox1.TabStop = false;
            // 
            // ckEnable
            // 
            this.ckEnable.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckEnable.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckEnable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckEnable.Location = new System.Drawing.Point(8, 252);
            this.ckEnable.Name = "ckEnable";
            this.ckEnable.Size = new System.Drawing.Size(89, 64);
            this.ckEnable.TabIndex = 351;
            this.ckEnable.Text = "Enable VR";
            this.ckEnable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckEnable.UseVisualStyleBackColor = true;
            this.ckEnable.CheckedChanged += new System.EventHandler(this.ckEnable_CheckedChanged);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // checkBox1
            // 
            this.checkBox1.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox1.FlatAppearance.BorderSize = 0;
            this.checkBox1.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.checkBox1.Location = new System.Drawing.Point(8, 92);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(89, 64);
            this.checkBox1.TabIndex = 352;
            this.checkBox1.Text = "Resume";
            this.checkBox1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // lbArea
            // 
            this.lbArea.Location = new System.Drawing.Point(199, 31);
            this.lbArea.Name = "lbArea";
            this.lbArea.Size = new System.Drawing.Size(67, 24);
            this.lbArea.TabIndex = 342;
            this.lbArea.Text = "9999.0";
            this.lbArea.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // frmMenuRateMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 630);
            this.ControlBox = false;
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.ckFullScreen);
            this.Controls.Add(this.ckEnable);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.VSzoom);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "frmMenuRateMap";
            this.ShowInTaskbar = false;
            this.Text = "Rate Map";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMenuRateMap_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.mnuRateMap_FormClosed);
            this.Load += new System.EventHandler(this.mnuRateMap_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TextBox tbMapName;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnCreateZone;
        private System.Windows.Forms.Label lbP4;
        private System.Windows.Forms.TextBox tbP4;
        private System.Windows.Forms.Label lbP3;
        private System.Windows.Forms.TextBox tbP3;
        private System.Windows.Forms.Label lbP2;
        private System.Windows.Forms.TextBox tbP2;
        private System.Windows.Forms.Label lbP1;
        private System.Windows.Forms.TextBox tbP1;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.CheckBox ckEdit;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.VScrollBar VSzoom;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox ckFullScreen;
        private System.Windows.Forms.CheckBox ckEnable;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox colorComboBox;
        private System.Windows.Forms.Label lbArea;
    }
}