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
            this.btnImport = new System.Windows.Forms.Button();
            this.gbZone = new System.Windows.Forms.GroupBox();
            this.lbArea = new System.Windows.Forms.Label();
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
            this.legendPanel = new System.Windows.Forms.Panel();
            this.ckSatView = new System.Windows.Forms.CheckBox();
            this.ckFullScreen = new System.Windows.Forms.CheckBox();
            this.VSzoom = new System.Windows.Forms.VScrollBar();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.ckEnable = new System.Windows.Forms.CheckBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.ckZones = new System.Windows.Forms.CheckBox();
            this.btnPNG = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.ckRateData = new System.Windows.Forms.CheckBox();
            this.btnCopy = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.gbZone.SuspendLayout();
            this.legendPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(8, 96);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(89, 64);
            this.btnImport.TabIndex = 342;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // gbZone
            // 
            this.gbZone.Controls.Add(this.lbArea);
            this.gbZone.Controls.Add(this.label1);
            this.gbZone.Controls.Add(this.colorComboBox);
            this.gbZone.Controls.Add(this.btnDelete);
            this.gbZone.Controls.Add(this.btnCreateZone);
            this.gbZone.Controls.Add(this.lbP4);
            this.gbZone.Controls.Add(this.tbP4);
            this.gbZone.Controls.Add(this.lbP3);
            this.gbZone.Controls.Add(this.ckEdit);
            this.gbZone.Controls.Add(this.tbP3);
            this.gbZone.Controls.Add(this.lbP2);
            this.gbZone.Controls.Add(this.tbP2);
            this.gbZone.Controls.Add(this.lbP1);
            this.gbZone.Controls.Add(this.tbP1);
            this.gbZone.Controls.Add(this.tbName);
            this.gbZone.Location = new System.Drawing.Point(106, 0);
            this.gbZone.Name = "gbZone";
            this.gbZone.Size = new System.Drawing.Size(427, 302);
            this.gbZone.TabIndex = 21;
            this.gbZone.TabStop = false;
            this.gbZone.Text = "Zone";
            this.gbZone.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox1_Paint);
            // 
            // lbArea
            // 
            this.lbArea.Location = new System.Drawing.Point(343, 31);
            this.lbArea.Name = "lbArea";
            this.lbArea.Size = new System.Drawing.Size(67, 24);
            this.lbArea.TabIndex = 342;
            this.lbArea.Text = "9999.0";
            this.lbArea.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(150, 260);
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
            this.colorComboBox.Location = new System.Drawing.Point(224, 256);
            this.colorComboBox.Name = "colorComboBox";
            this.colorComboBox.Size = new System.Drawing.Size(186, 32);
            this.colorComboBox.TabIndex = 340;
            // 
            // btnDelete
            // 
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Image = global::RateController.Properties.Resources.Trash;
            this.btnDelete.Location = new System.Drawing.Point(10, 124);
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
            this.btnCreateZone.Location = new System.Drawing.Point(10, 220);
            this.btnCreateZone.Name = "btnCreateZone";
            this.btnCreateZone.Size = new System.Drawing.Size(82, 64);
            this.btnCreateZone.TabIndex = 2;
            this.btnCreateZone.UseVisualStyleBackColor = true;
            this.btnCreateZone.Click += new System.EventHandler(this.btnCreateZone_Click);
            // 
            // lbP4
            // 
            this.lbP4.Location = new System.Drawing.Point(150, 212);
            this.lbP4.Name = "lbP4";
            this.lbP4.Size = new System.Drawing.Size(116, 24);
            this.lbP4.TabIndex = 11;
            this.lbP4.Text = "Product D";
            // 
            // tbP4
            // 
            this.tbP4.Location = new System.Drawing.Point(343, 210);
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
            this.lbP3.Location = new System.Drawing.Point(150, 166);
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
            this.ckEdit.Location = new System.Drawing.Point(10, 28);
            this.ckEdit.Name = "ckEdit";
            this.ckEdit.Size = new System.Drawing.Size(82, 64);
            this.ckEdit.TabIndex = 339;
            this.ckEdit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckEdit.UseVisualStyleBackColor = true;
            this.ckEdit.CheckedChanged += new System.EventHandler(this.ckEdit_CheckedChanged);
            // 
            // tbP3
            // 
            this.tbP3.Location = new System.Drawing.Point(343, 164);
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
            this.lbP2.Location = new System.Drawing.Point(150, 120);
            this.lbP2.Name = "lbP2";
            this.lbP2.Size = new System.Drawing.Size(116, 24);
            this.lbP2.TabIndex = 7;
            this.lbP2.Text = "Product B";
            // 
            // tbP2
            // 
            this.tbP2.Location = new System.Drawing.Point(343, 118);
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
            this.lbP1.Location = new System.Drawing.Point(150, 74);
            this.lbP1.Name = "lbP1";
            this.lbP1.Size = new System.Drawing.Size(116, 24);
            this.lbP1.TabIndex = 5;
            this.lbP1.Text = "Product A";
            // 
            // tbP1
            // 
            this.tbP1.Location = new System.Drawing.Point(343, 72);
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
            this.tbName.Location = new System.Drawing.Point(154, 29);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(169, 29);
            this.tbName.TabIndex = 2;
            this.tbName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbName.TextChanged += new System.EventHandler(this.tbName_TextChanged);
            // 
            // legendPanel
            // 
            this.legendPanel.Controls.Add(this.label2);
            this.legendPanel.Location = new System.Drawing.Point(133, 356);
            this.legendPanel.Name = "legendPanel";
            this.legendPanel.Size = new System.Drawing.Size(239, 245);
            this.legendPanel.TabIndex = 359;
            this.legendPanel.Visible = false;
            // 
            // ckSatView
            // 
            this.ckSatView.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckSatView.Checked = true;
            this.ckSatView.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckSatView.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckSatView.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckSatView.Location = new System.Drawing.Point(8, 516);
            this.ckSatView.Name = "ckSatView";
            this.ckSatView.Size = new System.Drawing.Size(89, 64);
            this.ckSatView.TabIndex = 349;
            this.ckSatView.Text = "Sat View";
            this.ckSatView.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckSatView.UseVisualStyleBackColor = true;
            this.ckSatView.CheckedChanged += new System.EventHandler(this.ckMap_CheckedChanged);
            // 
            // ckFullScreen
            // 
            this.ckFullScreen.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckFullScreen.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckFullScreen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckFullScreen.Location = new System.Drawing.Point(8, 348);
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
            this.VSzoom.Location = new System.Drawing.Point(106, 308);
            this.VSzoom.Name = "VSzoom";
            this.VSzoom.Size = new System.Drawing.Size(45, 358);
            this.VSzoom.TabIndex = 349;
            this.VSzoom.Scroll += new System.Windows.Forms.ScrollEventHandler(this.VSzoom_Scroll);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(154, 308);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(379, 358);
            this.pictureBox1.TabIndex = 350;
            this.pictureBox1.TabStop = false;
            // 
            // ckEnable
            // 
            this.ckEnable.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckEnable.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckEnable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckEnable.Location = new System.Drawing.Point(8, 264);
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
            // ckZones
            // 
            this.ckZones.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckZones.Checked = true;
            this.ckZones.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckZones.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckZones.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckZones.Location = new System.Drawing.Point(8, 432);
            this.ckZones.Name = "ckZones";
            this.ckZones.Size = new System.Drawing.Size(89, 64);
            this.ckZones.TabIndex = 354;
            this.ckZones.Text = "Zones";
            this.ckZones.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckZones.UseVisualStyleBackColor = true;
            this.ckZones.CheckedChanged += new System.EventHandler(this.ckZones_CheckedChanged);
            // 
            // btnPNG
            // 
            this.btnPNG.Location = new System.Drawing.Point(8, 180);
            this.btnPNG.Name = "btnPNG";
            this.btnPNG.Size = new System.Drawing.Size(89, 64);
            this.btnPNG.TabIndex = 356;
            this.btnPNG.Text = "Save Image";
            this.btnPNG.UseVisualStyleBackColor = true;
            this.btnPNG.Click += new System.EventHandler(this.btnPNG_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "PNG";
            // 
            // ckRateData
            // 
            this.ckRateData.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckRateData.Checked = true;
            this.ckRateData.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckRateData.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckRateData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckRateData.Location = new System.Drawing.Point(8, 600);
            this.ckRateData.Name = "ckRateData";
            this.ckRateData.Size = new System.Drawing.Size(89, 64);
            this.ckRateData.TabIndex = 358;
            this.ckRateData.Text = "Rate Data";
            this.ckRateData.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckRateData.UseVisualStyleBackColor = true;
            this.ckRateData.CheckedChanged += new System.EventHandler(this.ckRateData_CheckedChanged);
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(8, 12);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(89, 64);
            this.btnCopy.TabIndex = 359;
            this.btnCopy.Text = "Copy";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(161, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 24);
            this.label2.TabIndex = 0;
            this.label2.Text = "label2";
            // 
            // frmMenuRateMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 678);
            this.ControlBox = false;
            this.Controls.Add(this.legendPanel);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.btnPNG);
            this.Controls.Add(this.ckRateData);
            this.Controls.Add(this.ckSatView);
            this.Controls.Add(this.ckZones);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.ckFullScreen);
            this.Controls.Add(this.ckEnable);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.VSzoom);
            this.Controls.Add(this.gbZone);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "frmMenuRateMap";
            this.ShowInTaskbar = false;
            this.Text = "Rate Map";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMenuRateMap_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.mnuRateMap_FormClosed);
            this.Load += new System.EventHandler(this.mnuRateMap_Load);
            this.gbZone.ResumeLayout(false);
            this.gbZone.PerformLayout();
            this.legendPanel.ResumeLayout(false);
            this.legendPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox gbZone;
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
        private System.Windows.Forms.VScrollBar VSzoom;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox ckFullScreen;
        private System.Windows.Forms.CheckBox ckEnable;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox colorComboBox;
        private System.Windows.Forms.Label lbArea;
        private System.Windows.Forms.CheckBox ckSatView;
        private System.Windows.Forms.CheckBox ckZones;
        private System.Windows.Forms.Button btnPNG;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.CheckBox ckRateData;
        private System.Windows.Forms.Panel legendPanel;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Label label2;
    }
}