namespace RateController.Forms
{
    partial class frmMap
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMap));
            this.ckRateData = new System.Windows.Forms.CheckBox();
            this.ckSatView = new System.Windows.Forms.CheckBox();
            this.ckZones = new System.Windows.Forms.CheckBox();
            this.ckUseVR = new System.Windows.Forms.CheckBox();
            this.btnDeleteData = new System.Windows.Forms.Button();
            this.lbDataPoints = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ckRecord = new System.Windows.Forms.CheckBox();
            this.pnlControls = new System.Windows.Forms.Panel();
            this.btnHelp = new System.Windows.Forms.Button();
            this.ckKML = new System.Windows.Forms.CheckBox();
            this.ckWindow = new System.Windows.Forms.CheckBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.btnZoomIn = new System.Windows.Forms.Button();
            this.btnZoomOut = new System.Windows.Forms.Button();
            this.VSB = new System.Windows.Forms.VScrollBar();
            this.HSB = new System.Windows.Forms.HScrollBar();
            this.pnlMap = new System.Windows.Forms.Panel();
            this.btnOK = new System.Windows.Forms.Button();
            this.ckEdit = new System.Windows.Forms.CheckBox();
            this.lbArea = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.colorComboBox = new System.Windows.Forms.ComboBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lbP4 = new System.Windows.Forms.Label();
            this.tbP4 = new System.Windows.Forms.TextBox();
            this.lbP3 = new System.Windows.Forms.Label();
            this.ckNew = new System.Windows.Forms.CheckBox();
            this.tbP3 = new System.Windows.Forms.TextBox();
            this.lbP2 = new System.Windows.Forms.Label();
            this.tbP2 = new System.Windows.Forms.TextBox();
            this.lbP1 = new System.Windows.Forms.Label();
            this.tbP1 = new System.Windows.Forms.TextBox();
            this.tbName = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabZones = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.lbAreaName = new System.Windows.Forms.Label();
            this.tabData = new System.Windows.Forms.TabPage();
            this.rbProductA = new System.Windows.Forms.RadioButton();
            this.rbProductD = new System.Windows.Forms.RadioButton();
            this.rbProductC = new System.Windows.Forms.RadioButton();
            this.rbProductB = new System.Windows.Forms.RadioButton();
            this.tabFiles = new System.Windows.Forms.TabPage();
            this.btnKMLdelete = new System.Windows.Forms.Button();
            this.btnImportKML = new System.Windows.Forms.Button();
            this.btnImportZones = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.pnlTabs = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tlpTitle = new System.Windows.Forms.TableLayoutPanel();
            this.btnTitleClose = new System.Windows.Forms.Button();
            this.btnTitleZoomIn = new System.Windows.Forms.Button();
            this.lbTitle = new System.Windows.Forms.Label();
            this.btnTitleZoomOut = new System.Windows.Forms.Button();
            this.pnlControls.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabZones.SuspendLayout();
            this.tabData.SuspendLayout();
            this.tabFiles.SuspendLayout();
            this.pnlTabs.SuspendLayout();
            this.tlpTitle.SuspendLayout();
            this.SuspendLayout();
            // 
            // ckRateData
            // 
            this.ckRateData.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckRateData.Checked = true;
            this.ckRateData.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckRateData.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckRateData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckRateData.Location = new System.Drawing.Point(5, 299);
            this.ckRateData.Name = "ckRateData";
            this.ckRateData.Size = new System.Drawing.Size(89, 64);
            this.ckRateData.TabIndex = 366;
            this.ckRateData.Text = "Rate Data";
            this.ckRateData.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckRateData.UseVisualStyleBackColor = true;
            this.ckRateData.CheckedChanged += new System.EventHandler(this.ckRateData_CheckedChanged);
            // 
            // ckSatView
            // 
            this.ckSatView.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckSatView.Checked = true;
            this.ckSatView.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckSatView.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckSatView.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckSatView.Location = new System.Drawing.Point(5, 226);
            this.ckSatView.Name = "ckSatView";
            this.ckSatView.Size = new System.Drawing.Size(89, 64);
            this.ckSatView.TabIndex = 362;
            this.ckSatView.Text = "Sat View";
            this.ckSatView.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckSatView.UseVisualStyleBackColor = true;
            this.ckSatView.CheckedChanged += new System.EventHandler(this.ckSatView_CheckedChanged);
            // 
            // ckZones
            // 
            this.ckZones.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckZones.Checked = true;
            this.ckZones.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckZones.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckZones.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckZones.Location = new System.Drawing.Point(5, 153);
            this.ckZones.Name = "ckZones";
            this.ckZones.Size = new System.Drawing.Size(89, 64);
            this.ckZones.TabIndex = 364;
            this.ckZones.Text = "Zones";
            this.ckZones.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckZones.UseVisualStyleBackColor = true;
            this.ckZones.CheckedChanged += new System.EventHandler(this.ckZones_CheckedChanged);
            // 
            // ckUseVR
            // 
            this.ckUseVR.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckUseVR.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckUseVR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckUseVR.Location = new System.Drawing.Point(5, 80);
            this.ckUseVR.Name = "ckUseVR";
            this.ckUseVR.Size = new System.Drawing.Size(89, 64);
            this.ckUseVR.TabIndex = 363;
            this.ckUseVR.Text = "Enable VR";
            this.ckUseVR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckUseVR.UseVisualStyleBackColor = true;
            this.ckUseVR.CheckedChanged += new System.EventHandler(this.ckUseVR_CheckedChanged);
            // 
            // btnDeleteData
            // 
            this.btnDeleteData.FlatAppearance.BorderSize = 0;
            this.btnDeleteData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteData.Image = global::RateController.Properties.Resources.Trash;
            this.btnDeleteData.Location = new System.Drawing.Point(231, 250);
            this.btnDeleteData.Name = "btnDeleteData";
            this.btnDeleteData.Size = new System.Drawing.Size(89, 64);
            this.btnDeleteData.TabIndex = 397;
            this.btnDeleteData.UseVisualStyleBackColor = true;
            this.btnDeleteData.Click += new System.EventHandler(this.btnDeleteData_Click);
            // 
            // lbDataPoints
            // 
            this.lbDataPoints.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDataPoints.Location = new System.Drawing.Point(246, 67);
            this.lbDataPoints.Name = "lbDataPoints";
            this.lbDataPoints.Size = new System.Drawing.Size(111, 23);
            this.lbDataPoints.TabIndex = 396;
            this.lbDataPoints.Text = "0";
            this.lbDataPoints.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(124, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 23);
            this.label1.TabIndex = 395;
            this.label1.Text = "Data points:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ckRecord
            // 
            this.ckRecord.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckRecord.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckRecord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckRecord.Image = global::RateController.Properties.Resources.record;
            this.ckRecord.Location = new System.Drawing.Point(105, 250);
            this.ckRecord.Name = "ckRecord";
            this.ckRecord.Size = new System.Drawing.Size(89, 64);
            this.ckRecord.TabIndex = 382;
            this.ckRecord.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckRecord.UseVisualStyleBackColor = true;
            this.ckRecord.CheckedChanged += new System.EventHandler(this.ckRecord_CheckedChanged);
            // 
            // pnlControls
            // 
            this.pnlControls.Controls.Add(this.btnHelp);
            this.pnlControls.Controls.Add(this.ckZones);
            this.pnlControls.Controls.Add(this.ckKML);
            this.pnlControls.Controls.Add(this.ckWindow);
            this.pnlControls.Controls.Add(this.ckSatView);
            this.pnlControls.Controls.Add(this.btnClose);
            this.pnlControls.Controls.Add(this.ckRateData);
            this.pnlControls.Controls.Add(this.ckUseVR);
            this.pnlControls.Location = new System.Drawing.Point(439, 2);
            this.pnlControls.Name = "pnlControls";
            this.pnlControls.Size = new System.Drawing.Size(97, 598);
            this.pnlControls.TabIndex = 386;
            // 
            // btnHelp
            // 
            this.btnHelp.BackColor = System.Drawing.Color.Transparent;
            this.btnHelp.FlatAppearance.BorderSize = 0;
            this.btnHelp.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkSlateBlue;
            this.btnHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHelp.Image = global::RateController.Properties.Resources.Help;
            this.btnHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnHelp.Location = new System.Drawing.Point(14, 445);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(70, 63);
            this.btnHelp.TabIndex = 392;
            this.btnHelp.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnHelp.UseVisualStyleBackColor = false;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // ckKML
            // 
            this.ckKML.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckKML.Checked = true;
            this.ckKML.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckKML.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckKML.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckKML.Location = new System.Drawing.Point(5, 372);
            this.ckKML.Name = "ckKML";
            this.ckKML.Size = new System.Drawing.Size(89, 64);
            this.ckKML.TabIndex = 371;
            this.ckKML.Text = "KML Files";
            this.ckKML.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckKML.UseVisualStyleBackColor = true;
            this.ckKML.CheckedChanged += new System.EventHandler(this.ckKML_CheckedChanged);
            // 
            // ckWindow
            // 
            this.ckWindow.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckWindow.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckWindow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckWindow.Location = new System.Drawing.Point(5, 7);
            this.ckWindow.Name = "ckWindow";
            this.ckWindow.Size = new System.Drawing.Size(89, 64);
            this.ckWindow.TabIndex = 391;
            this.ckWindow.Text = "Window";
            this.ckWindow.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckWindow.UseVisualStyleBackColor = true;
            this.ckWindow.CheckedChanged += new System.EventHandler(this.ckWindow_CheckedChanged);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkSlateBlue;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.Location = new System.Drawing.Point(14, 517);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(70, 63);
            this.btnClose.TabIndex = 386;
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.btnZoomIn);
            this.pnlMain.Controls.Add(this.btnZoomOut);
            this.pnlMain.Controls.Add(this.VSB);
            this.pnlMain.Controls.Add(this.HSB);
            this.pnlMain.Controls.Add(this.pnlMap);
            this.pnlMain.Location = new System.Drawing.Point(545, 2);
            this.pnlMain.Margin = new System.Windows.Forms.Padding(6);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(403, 407);
            this.pnlMain.TabIndex = 387;
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.BackColor = System.Drawing.Color.Transparent;
            this.btnZoomIn.FlatAppearance.BorderSize = 0;
            this.btnZoomIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZoomIn.Image = global::RateController.Properties.Resources.plus_square;
            this.btnZoomIn.Location = new System.Drawing.Point(347, 353);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(50, 50);
            this.btnZoomIn.TabIndex = 358;
            this.btnZoomIn.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnZoomIn.UseVisualStyleBackColor = false;
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.BackColor = System.Drawing.Color.Transparent;
            this.btnZoomOut.FlatAppearance.BorderSize = 0;
            this.btnZoomOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZoomOut.Image = global::RateController.Properties.Resources.minus_square;
            this.btnZoomOut.Location = new System.Drawing.Point(291, 353);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(50, 50);
            this.btnZoomOut.TabIndex = 357;
            this.btnZoomOut.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnZoomOut.UseVisualStyleBackColor = false;
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // VSB
            // 
            this.VSB.Location = new System.Drawing.Point(347, 6);
            this.VSB.Name = "VSB";
            this.VSB.Size = new System.Drawing.Size(50, 338);
            this.VSB.TabIndex = 2;
            this.VSB.Scroll += new System.Windows.Forms.ScrollEventHandler(this.VSB_Scroll);
            // 
            // HSB
            // 
            this.HSB.Location = new System.Drawing.Point(6, 353);
            this.HSB.Name = "HSB";
            this.HSB.Size = new System.Drawing.Size(282, 50);
            this.HSB.TabIndex = 1;
            this.HSB.Scroll += new System.Windows.Forms.ScrollEventHandler(this.HSB_Scroll);
            // 
            // pnlMap
            // 
            this.pnlMap.Location = new System.Drawing.Point(0, 0);
            this.pnlMap.Margin = new System.Windows.Forms.Padding(6);
            this.pnlMap.Name = "pnlMap";
            this.pnlMap.Size = new System.Drawing.Size(335, 338);
            this.pnlMap.TabIndex = 0;
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
            this.btnOK.Location = new System.Drawing.Point(12, 301);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(70, 63);
            this.btnOK.TabIndex = 344;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // ckEdit
            // 
            this.ckEdit.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckEdit.FlatAppearance.BorderSize = 0;
            this.ckEdit.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckEdit.Image = global::RateController.Properties.Resources.FileEditName;
            this.ckEdit.Location = new System.Drawing.Point(6, 76);
            this.ckEdit.Name = "ckEdit";
            this.ckEdit.Size = new System.Drawing.Size(82, 64);
            this.ckEdit.TabIndex = 343;
            this.ckEdit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckEdit.UseVisualStyleBackColor = true;
            this.ckEdit.CheckedChanged += new System.EventHandler(this.ckEdit_CheckedChanged);
            // 
            // lbArea
            // 
            this.lbArea.Location = new System.Drawing.Point(339, 67);
            this.lbArea.Name = "lbArea";
            this.lbArea.Size = new System.Drawing.Size(67, 24);
            this.lbArea.TabIndex = 342;
            this.lbArea.Text = "0.0";
            this.lbArea.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(146, 336);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 24);
            this.label2.TabIndex = 341;
            this.label2.Text = "Color";
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
            this.colorComboBox.Location = new System.Drawing.Point(244, 332);
            this.colorComboBox.Name = "colorComboBox";
            this.colorComboBox.Size = new System.Drawing.Size(162, 32);
            this.colorComboBox.TabIndex = 340;
            this.colorComboBox.SelectedIndexChanged += new System.EventHandler(this.colorComboBox_SelectedIndexChanged);
            this.colorComboBox.Click += new System.EventHandler(this.colorComboBox_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Enabled = false;
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Image = global::RateController.Properties.Resources.Trash;
            this.btnDelete.Location = new System.Drawing.Point(6, 154);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(82, 64);
            this.btnDelete.TabIndex = 12;
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lbP4
            // 
            this.lbP4.Location = new System.Drawing.Point(146, 280);
            this.lbP4.Name = "lbP4";
            this.lbP4.Size = new System.Drawing.Size(116, 24);
            this.lbP4.TabIndex = 11;
            this.lbP4.Text = "Product D";
            // 
            // tbP4
            // 
            this.tbP4.Location = new System.Drawing.Point(326, 278);
            this.tbP4.Name = "tbP4";
            this.tbP4.Size = new System.Drawing.Size(80, 29);
            this.tbP4.TabIndex = 10;
            this.tbP4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbP4.Enter += new System.EventHandler(this.tbP4_Enter);
            this.tbP4.Validating += new System.ComponentModel.CancelEventHandler(this.tbP4_Validating);
            // 
            // lbP3
            // 
            this.lbP3.Location = new System.Drawing.Point(146, 226);
            this.lbP3.Name = "lbP3";
            this.lbP3.Size = new System.Drawing.Size(116, 24);
            this.lbP3.TabIndex = 9;
            this.lbP3.Text = "Product C";
            // 
            // ckNew
            // 
            this.ckNew.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckNew.FlatAppearance.BorderSize = 0;
            this.ckNew.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckNew.Image = global::RateController.Properties.Resources.polygon;
            this.ckNew.Location = new System.Drawing.Point(6, 6);
            this.ckNew.Name = "ckNew";
            this.ckNew.Size = new System.Drawing.Size(82, 64);
            this.ckNew.TabIndex = 339;
            this.ckNew.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckNew.UseVisualStyleBackColor = true;
            this.ckNew.CheckedChanged += new System.EventHandler(this.ckNew_CheckedChanged);
            // 
            // tbP3
            // 
            this.tbP3.Location = new System.Drawing.Point(326, 224);
            this.tbP3.Name = "tbP3";
            this.tbP3.Size = new System.Drawing.Size(80, 29);
            this.tbP3.TabIndex = 8;
            this.tbP3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbP3.Enter += new System.EventHandler(this.tbP3_Enter);
            this.tbP3.Validating += new System.ComponentModel.CancelEventHandler(this.tbP3_Validating);
            // 
            // lbP2
            // 
            this.lbP2.Location = new System.Drawing.Point(146, 172);
            this.lbP2.Name = "lbP2";
            this.lbP2.Size = new System.Drawing.Size(116, 24);
            this.lbP2.TabIndex = 7;
            this.lbP2.Text = "Product B";
            // 
            // tbP2
            // 
            this.tbP2.Location = new System.Drawing.Point(326, 170);
            this.tbP2.Name = "tbP2";
            this.tbP2.Size = new System.Drawing.Size(80, 29);
            this.tbP2.TabIndex = 6;
            this.tbP2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbP2.Enter += new System.EventHandler(this.tbP2_Enter);
            this.tbP2.Validating += new System.ComponentModel.CancelEventHandler(this.tbP2_Validating);
            // 
            // lbP1
            // 
            this.lbP1.Location = new System.Drawing.Point(146, 118);
            this.lbP1.Name = "lbP1";
            this.lbP1.Size = new System.Drawing.Size(116, 24);
            this.lbP1.TabIndex = 5;
            this.lbP1.Text = "Product A";
            // 
            // tbP1
            // 
            this.tbP1.Location = new System.Drawing.Point(326, 116);
            this.tbP1.Name = "tbP1";
            this.tbP1.Size = new System.Drawing.Size(80, 29);
            this.tbP1.TabIndex = 4;
            this.tbP1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbP1.Enter += new System.EventHandler(this.tbP1_Enter);
            this.tbP1.Validating += new System.ComponentModel.CancelEventHandler(this.tbP1_Validating);
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(244, 13);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(162, 29);
            this.tbName.TabIndex = 2;
            this.tbName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbName_KeyPress);
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
            this.btnCancel.Location = new System.Drawing.Point(12, 228);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(70, 63);
            this.btnCancel.TabIndex = 345;
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabControl1.Controls.Add(this.tabZones);
            this.tabControl1.Controls.Add(this.tabData);
            this.tabControl1.Controls.Add(this.tabFiles);
            this.tabControl1.ItemSize = new System.Drawing.Size(100, 50);
            this.tabControl1.Location = new System.Drawing.Point(2, 2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(431, 435);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.TabIndex = 389;
            // 
            // tabZones
            // 
            this.tabZones.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tabZones.Controls.Add(this.label3);
            this.tabZones.Controls.Add(this.lbAreaName);
            this.tabZones.Controls.Add(this.btnOK);
            this.tabZones.Controls.Add(this.ckEdit);
            this.tabZones.Controls.Add(this.btnCancel);
            this.tabZones.Controls.Add(this.lbArea);
            this.tabZones.Controls.Add(this.tbName);
            this.tabZones.Controls.Add(this.label2);
            this.tabZones.Controls.Add(this.tbP1);
            this.tabZones.Controls.Add(this.colorComboBox);
            this.tabZones.Controls.Add(this.lbP1);
            this.tabZones.Controls.Add(this.btnDelete);
            this.tabZones.Controls.Add(this.tbP2);
            this.tabZones.Controls.Add(this.lbP4);
            this.tabZones.Controls.Add(this.lbP2);
            this.tabZones.Controls.Add(this.tbP4);
            this.tabZones.Controls.Add(this.tbP3);
            this.tabZones.Controls.Add(this.lbP3);
            this.tabZones.Controls.Add(this.ckNew);
            this.tabZones.Location = new System.Drawing.Point(4, 54);
            this.tabZones.Name = "tabZones";
            this.tabZones.Padding = new System.Windows.Forms.Padding(3);
            this.tabZones.Size = new System.Drawing.Size(423, 377);
            this.tabZones.TabIndex = 0;
            this.tabZones.Text = "Zones";
            this.tabZones.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(146, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 24);
            this.label3.TabIndex = 347;
            this.label3.Text = "Name";
            // 
            // lbAreaName
            // 
            this.lbAreaName.Location = new System.Drawing.Point(146, 67);
            this.lbAreaName.Name = "lbAreaName";
            this.lbAreaName.Size = new System.Drawing.Size(68, 24);
            this.lbAreaName.TabIndex = 346;
            this.lbAreaName.Text = "Acres";
            // 
            // tabData
            // 
            this.tabData.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tabData.Controls.Add(this.rbProductA);
            this.tabData.Controls.Add(this.rbProductD);
            this.tabData.Controls.Add(this.rbProductC);
            this.tabData.Controls.Add(this.rbProductB);
            this.tabData.Controls.Add(this.ckRecord);
            this.tabData.Controls.Add(this.btnDeleteData);
            this.tabData.Controls.Add(this.label1);
            this.tabData.Controls.Add(this.lbDataPoints);
            this.tabData.Location = new System.Drawing.Point(4, 54);
            this.tabData.Name = "tabData";
            this.tabData.Padding = new System.Windows.Forms.Padding(3);
            this.tabData.Size = new System.Drawing.Size(423, 377);
            this.tabData.TabIndex = 1;
            this.tabData.Text = "Data";
            this.tabData.UseVisualStyleBackColor = true;
            // 
            // rbProductA
            // 
            this.rbProductA.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbProductA.Checked = true;
            this.rbProductA.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbProductA.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbProductA.Location = new System.Drawing.Point(36, 147);
            this.rbProductA.Name = "rbProductA";
            this.rbProductA.Size = new System.Drawing.Size(66, 40);
            this.rbProductA.TabIndex = 401;
            this.rbProductA.TabStop = true;
            this.rbProductA.Text = "A";
            this.rbProductA.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbProductA.UseVisualStyleBackColor = true;
            this.rbProductA.CheckedChanged += new System.EventHandler(this.rbProductA_CheckedChanged);
            // 
            // rbProductD
            // 
            this.rbProductD.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbProductD.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbProductD.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbProductD.Location = new System.Drawing.Point(312, 147);
            this.rbProductD.Name = "rbProductD";
            this.rbProductD.Size = new System.Drawing.Size(66, 40);
            this.rbProductD.TabIndex = 398;
            this.rbProductD.Text = "D";
            this.rbProductD.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbProductD.UseVisualStyleBackColor = true;
            this.rbProductD.CheckedChanged += new System.EventHandler(this.rbProductA_CheckedChanged);
            // 
            // rbProductC
            // 
            this.rbProductC.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbProductC.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbProductC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbProductC.Location = new System.Drawing.Point(220, 147);
            this.rbProductC.Name = "rbProductC";
            this.rbProductC.Size = new System.Drawing.Size(66, 40);
            this.rbProductC.TabIndex = 399;
            this.rbProductC.Text = "C";
            this.rbProductC.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbProductC.UseVisualStyleBackColor = true;
            this.rbProductC.CheckedChanged += new System.EventHandler(this.rbProductA_CheckedChanged);
            // 
            // rbProductB
            // 
            this.rbProductB.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbProductB.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbProductB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbProductB.Location = new System.Drawing.Point(128, 147);
            this.rbProductB.Name = "rbProductB";
            this.rbProductB.Size = new System.Drawing.Size(66, 40);
            this.rbProductB.TabIndex = 400;
            this.rbProductB.Text = "B";
            this.rbProductB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbProductB.UseVisualStyleBackColor = true;
            this.rbProductB.CheckedChanged += new System.EventHandler(this.rbProductA_CheckedChanged);
            // 
            // tabFiles
            // 
            this.tabFiles.Controls.Add(this.btnKMLdelete);
            this.tabFiles.Controls.Add(this.btnImportKML);
            this.tabFiles.Controls.Add(this.btnImportZones);
            this.tabFiles.Controls.Add(this.btnExport);
            this.tabFiles.Location = new System.Drawing.Point(4, 54);
            this.tabFiles.Name = "tabFiles";
            this.tabFiles.Padding = new System.Windows.Forms.Padding(3);
            this.tabFiles.Size = new System.Drawing.Size(423, 377);
            this.tabFiles.TabIndex = 2;
            this.tabFiles.Text = "Files";
            this.tabFiles.UseVisualStyleBackColor = true;
            // 
            // btnKMLdelete
            // 
            this.btnKMLdelete.BackColor = System.Drawing.Color.Transparent;
            this.btnKMLdelete.FlatAppearance.BorderSize = 0;
            this.btnKMLdelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnKMLdelete.Image = global::RateController.Properties.Resources.folder_open_small;
            this.btnKMLdelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnKMLdelete.Location = new System.Drawing.Point(104, 280);
            this.btnKMLdelete.Name = "btnKMLdelete";
            this.btnKMLdelete.Size = new System.Drawing.Size(213, 64);
            this.btnKMLdelete.TabIndex = 372;
            this.btnKMLdelete.Text = "Delete KML File";
            this.btnKMLdelete.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnKMLdelete.UseVisualStyleBackColor = false;
            this.btnKMLdelete.Click += new System.EventHandler(this.btnKMLdelete_Click);
            // 
            // btnImportKML
            // 
            this.btnImportKML.BackColor = System.Drawing.Color.Transparent;
            this.btnImportKML.FlatAppearance.BorderSize = 0;
            this.btnImportKML.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImportKML.Image = global::RateController.Properties.Resources.folder_open_small;
            this.btnImportKML.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnImportKML.Location = new System.Drawing.Point(104, 198);
            this.btnImportKML.Name = "btnImportKML";
            this.btnImportKML.Size = new System.Drawing.Size(213, 64);
            this.btnImportKML.TabIndex = 370;
            this.btnImportKML.Text = "Import KML File";
            this.btnImportKML.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnImportKML.UseVisualStyleBackColor = false;
            this.btnImportKML.Click += new System.EventHandler(this.btnImportKML_Click);
            // 
            // btnImportZones
            // 
            this.btnImportZones.BackColor = System.Drawing.Color.Transparent;
            this.btnImportZones.FlatAppearance.BorderSize = 0;
            this.btnImportZones.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImportZones.Image = global::RateController.Properties.Resources.folder_open_small;
            this.btnImportZones.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnImportZones.Location = new System.Drawing.Point(104, 34);
            this.btnImportZones.Name = "btnImportZones";
            this.btnImportZones.Size = new System.Drawing.Size(213, 64);
            this.btnImportZones.TabIndex = 369;
            this.btnImportZones.Text = "Import Shape File";
            this.btnImportZones.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnImportZones.UseVisualStyleBackColor = false;
            this.btnImportZones.Click += new System.EventHandler(this.btnImportZones_Click);
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.Color.Transparent;
            this.btnExport.FlatAppearance.BorderSize = 0;
            this.btnExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExport.Image = global::RateController.Properties.Resources.folder_open_small;
            this.btnExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExport.Location = new System.Drawing.Point(104, 116);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(213, 64);
            this.btnExport.TabIndex = 368;
            this.btnExport.Text = "Export Map";
            this.btnExport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // pnlTabs
            // 
            this.pnlTabs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlTabs.Controls.Add(this.tabControl1);
            this.pnlTabs.Location = new System.Drawing.Point(2, 2);
            this.pnlTabs.Name = "pnlTabs";
            this.pnlTabs.Size = new System.Drawing.Size(433, 438);
            this.pnlTabs.TabIndex = 390;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // tlpTitle
            // 
            this.tlpTitle.ColumnCount = 4;
            this.tlpTitle.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.tlpTitle.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tlpTitle.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tlpTitle.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tlpTitle.Controls.Add(this.btnTitleClose, 3, 0);
            this.tlpTitle.Controls.Add(this.btnTitleZoomIn, 2, 0);
            this.tlpTitle.Controls.Add(this.lbTitle, 0, 0);
            this.tlpTitle.Controls.Add(this.btnTitleZoomOut, 1, 0);
            this.tlpTitle.Location = new System.Drawing.Point(307, 492);
            this.tlpTitle.Name = "tlpTitle";
            this.tlpTitle.RowCount = 1;
            this.tlpTitle.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpTitle.Size = new System.Drawing.Size(300, 45);
            this.tlpTitle.TabIndex = 391;
            this.tlpTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tlpTitle_MouseDown);
            this.tlpTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tlpTitle_MouseMove);
            // 
            // btnTitleClose
            // 
            this.btnTitleClose.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTitleClose.FlatAppearance.BorderSize = 0;
            this.btnTitleClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTitleClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTitleClose.Image = global::RateController.Properties.Resources.x_square48;
            this.btnTitleClose.Location = new System.Drawing.Point(258, 3);
            this.btnTitleClose.Name = "btnTitleClose";
            this.btnTitleClose.Size = new System.Drawing.Size(39, 39);
            this.btnTitleClose.TabIndex = 3;
            this.btnTitleClose.UseVisualStyleBackColor = true;
            this.btnTitleClose.Click += new System.EventHandler(this.btnTitleClose_Click);
            // 
            // btnTitleZoomIn
            // 
            this.btnTitleZoomIn.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTitleZoomIn.FlatAppearance.BorderSize = 0;
            this.btnTitleZoomIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTitleZoomIn.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTitleZoomIn.Image = global::RateController.Properties.Resources.plus_square48;
            this.btnTitleZoomIn.Location = new System.Drawing.Point(213, 3);
            this.btnTitleZoomIn.Name = "btnTitleZoomIn";
            this.btnTitleZoomIn.Size = new System.Drawing.Size(39, 39);
            this.btnTitleZoomIn.TabIndex = 2;
            this.btnTitleZoomIn.UseVisualStyleBackColor = true;
            this.btnTitleZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // lbTitle
            // 
            this.lbTitle.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbTitle.Location = new System.Drawing.Point(3, 0);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(159, 45);
            this.lbTitle.TabIndex = 0;
            this.lbTitle.Text = "Map";
            this.lbTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tlpTitle_MouseDown);
            this.lbTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tlpTitle_MouseMove);
            // 
            // btnTitleZoomOut
            // 
            this.btnTitleZoomOut.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTitleZoomOut.FlatAppearance.BorderSize = 0;
            this.btnTitleZoomOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTitleZoomOut.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTitleZoomOut.Image = global::RateController.Properties.Resources.minus_square48;
            this.btnTitleZoomOut.Location = new System.Drawing.Point(168, 3);
            this.btnTitleZoomOut.Name = "btnTitleZoomOut";
            this.btnTitleZoomOut.Size = new System.Drawing.Size(39, 39);
            this.btnTitleZoomOut.TabIndex = 1;
            this.btnTitleZoomOut.UseVisualStyleBackColor = true;
            this.btnTitleZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // frmMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(955, 616);
            this.Controls.Add(this.tlpTitle);
            this.Controls.Add(this.pnlTabs);
            this.Controls.Add(this.pnlControls);
            this.Controls.Add(this.pnlMain);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMap";
            this.ShowInTaskbar = false;
            this.Text = "Map";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMap_FormClosing);
            this.Load += new System.EventHandler(this.frmMap_Load);
            this.Move += new System.EventHandler(this.frmMap_Move);
            this.pnlControls.ResumeLayout(false);
            this.pnlMain.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabZones.ResumeLayout(false);
            this.tabZones.PerformLayout();
            this.tabData.ResumeLayout(false);
            this.tabFiles.ResumeLayout(false);
            this.pnlTabs.ResumeLayout(false);
            this.tlpTitle.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.CheckBox ckRateData;
        private System.Windows.Forms.CheckBox ckSatView;
        private System.Windows.Forms.CheckBox ckZones;
        private System.Windows.Forms.CheckBox ckUseVR;
        private System.Windows.Forms.Button btnDeleteData;
        private System.Windows.Forms.Label lbDataPoints;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox ckRecord;
        private System.Windows.Forms.Panel pnlControls;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.VScrollBar VSB;
        private System.Windows.Forms.HScrollBar HSB;
        private System.Windows.Forms.Panel pnlMap;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox ckEdit;
        private System.Windows.Forms.Label lbArea;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox colorComboBox;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label lbP4;
        private System.Windows.Forms.TextBox tbP4;
        private System.Windows.Forms.Label lbP3;
        private System.Windows.Forms.CheckBox ckNew;
        private System.Windows.Forms.TextBox tbP3;
        private System.Windows.Forms.Label lbP2;
        private System.Windows.Forms.TextBox tbP2;
        private System.Windows.Forms.Label lbP1;
        private System.Windows.Forms.TextBox tbP1;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabZones;
        private System.Windows.Forms.TabPage tabData;
        private System.Windows.Forms.Panel pnlTabs;
        private System.Windows.Forms.RadioButton rbProductA;
        private System.Windows.Forms.RadioButton rbProductD;
        private System.Windows.Forms.RadioButton rbProductC;
        private System.Windows.Forms.RadioButton rbProductB;
        private System.Windows.Forms.CheckBox ckWindow;
        private System.Windows.Forms.Label lbAreaName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TabPage tabFiles;
        private System.Windows.Forms.Button btnImportZones;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnImportKML;
        private System.Windows.Forms.CheckBox ckKML;
        private System.Windows.Forms.Button btnKMLdelete;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.TableLayoutPanel tlpTitle;
        private System.Windows.Forms.Button btnTitleClose;
        private System.Windows.Forms.Button btnTitleZoomIn;
        private System.Windows.Forms.Label lbTitle;
        private System.Windows.Forms.Button btnTitleZoomOut;
    }
}