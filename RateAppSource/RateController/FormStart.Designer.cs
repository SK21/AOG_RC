
namespace RateController
{
    partial class FormStart
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormStart));
            this.timerMain = new System.Windows.Forms.Timer(this.components);
            this.lbArduinoConnected = new System.Windows.Forms.Label();
            this.lbAogConnected = new System.Windows.Forms.Label();
            this.panProducts = new System.Windows.Forms.Panel();
            this.btAlarm = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lbProduct = new System.Windows.Forms.Label();
            this.SetRate = new System.Windows.Forms.Label();
            this.lbRate = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.TankRemain = new System.Windows.Forms.Label();
            this.lblUnits = new System.Windows.Forms.Label();
            this.VolApplied = new System.Windows.Forms.Label();
            this.lbApplied = new System.Windows.Forms.Label();
            this.AreaDone = new System.Windows.Forms.Label();
            this.lbRateAmount = new System.Windows.Forms.Label();
            this.lbCoverage = new System.Windows.Forms.Label();
            this.lbRemaining = new System.Windows.Forms.Label();
            this.panSummary = new System.Windows.Forms.Panel();
            this.idc0 = new System.Windows.Forms.Label();
            this.prd4 = new System.Windows.Forms.Label();
            this.rt4 = new System.Windows.Forms.Label();
            this.idc4 = new System.Windows.Forms.Label();
            this.prd3 = new System.Windows.Forms.Label();
            this.rt3 = new System.Windows.Forms.Label();
            this.idc3 = new System.Windows.Forms.Label();
            this.prd2 = new System.Windows.Forms.Label();
            this.rt2 = new System.Windows.Forms.Label();
            this.idc2 = new System.Windows.Forms.Label();
            this.prd1 = new System.Windows.Forms.Label();
            this.rt1 = new System.Windows.Forms.Label();
            this.idc1 = new System.Windows.Forms.Label();
            this.prd0 = new System.Windows.Forms.Label();
            this.rt0 = new System.Windows.Forms.Label();
            this.timerNano = new System.Windows.Forms.Timer(this.components);
            this.mnuSettings = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MnuProducts = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuSections = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRelays = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuComm = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.MnuNew = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.MnuLanguage = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuDeustch = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuEnglish = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuNederlands = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.teensieToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nanoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuFirmware = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.teensieToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.nanoToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.panProducts.SuspendLayout();
            this.panSummary.SuspendLayout();
            this.mnuSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerMain
            // 
            this.timerMain.Enabled = true;
            this.timerMain.Interval = 1000;
            this.timerMain.Tick += new System.EventHandler(this.timerMain_Tick);
            // 
            // lbArduinoConnected
            // 
            this.lbArduinoConnected.BackColor = System.Drawing.Color.Red;
            this.lbArduinoConnected.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbArduinoConnected.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbArduinoConnected.Location = new System.Drawing.Point(208, 153);
            this.lbArduinoConnected.Name = "lbArduinoConnected";
            this.lbArduinoConnected.Size = new System.Drawing.Size(63, 27);
            this.lbArduinoConnected.TabIndex = 144;
            this.lbArduinoConnected.Text = "Mod";
            this.lbArduinoConnected.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbArduinoConnected.Click += new System.EventHandler(this.lbArduinoConnected_Click);
            this.lbArduinoConnected.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.lbArduinoConnected_HelpRequested);
            // 
            // lbAogConnected
            // 
            this.lbAogConnected.BackColor = System.Drawing.Color.Red;
            this.lbAogConnected.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbAogConnected.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbAogConnected.Location = new System.Drawing.Point(208, 188);
            this.lbAogConnected.Name = "lbAogConnected";
            this.lbAogConnected.Size = new System.Drawing.Size(63, 27);
            this.lbAogConnected.TabIndex = 145;
            this.lbAogConnected.Text = "AOG";
            this.lbAogConnected.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbAogConnected.Click += new System.EventHandler(this.lbAogConnected_Click);
            this.lbAogConnected.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.lbAogConnected_HelpRequested);
            // 
            // panProducts
            // 
            this.panProducts.Controls.Add(this.btAlarm);
            this.panProducts.Controls.Add(this.groupBox3);
            this.panProducts.Controls.Add(this.lbProduct);
            this.panProducts.Controls.Add(this.SetRate);
            this.panProducts.Controls.Add(this.lbRate);
            this.panProducts.Controls.Add(this.label1);
            this.panProducts.Controls.Add(this.TankRemain);
            this.panProducts.Controls.Add(this.lblUnits);
            this.panProducts.Controls.Add(this.VolApplied);
            this.panProducts.Controls.Add(this.lbApplied);
            this.panProducts.Controls.Add(this.AreaDone);
            this.panProducts.Controls.Add(this.lbRateAmount);
            this.panProducts.Controls.Add(this.lbCoverage);
            this.panProducts.Controls.Add(this.lbRemaining);
            this.panProducts.Location = new System.Drawing.Point(0, 0);
            this.panProducts.Name = "panProducts";
            this.panProducts.Size = new System.Drawing.Size(270, 150);
            this.panProducts.TabIndex = 50;
            // 
            // btAlarm
            // 
            this.btAlarm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btAlarm.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btAlarm.Image = ((System.Drawing.Image)(resources.GetObject("btAlarm.Image")));
            this.btAlarm.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btAlarm.Location = new System.Drawing.Point(12, 30);
            this.btAlarm.Name = "btAlarm";
            this.btAlarm.Size = new System.Drawing.Size(168, 120);
            this.btAlarm.TabIndex = 146;
            this.btAlarm.Text = "Rate  Alarm  Pressure Alarm";
            this.btAlarm.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btAlarm.UseVisualStyleBackColor = true;
            this.btAlarm.Visible = false;
            this.btAlarm.Click += new System.EventHandler(this.btAlarm_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Location = new System.Drawing.Point(9, 24);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(252, 1);
            this.groupBox3.TabIndex = 158;
            this.groupBox3.TabStop = false;
            this.groupBox3.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox3_Paint);
            // 
            // lbProduct
            // 
            this.lbProduct.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbProduct.Location = new System.Drawing.Point(3, 0);
            this.lbProduct.Name = "lbProduct";
            this.lbProduct.Size = new System.Drawing.Size(128, 23);
            this.lbProduct.TabIndex = 157;
            this.lbProduct.Text = "Herbicide";
            // 
            // SetRate
            // 
            this.SetRate.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SetRate.Location = new System.Drawing.Point(178, 48);
            this.SetRate.Name = "SetRate";
            this.SetRate.Size = new System.Drawing.Size(97, 23);
            this.SetRate.TabIndex = 156;
            this.SetRate.Text = "1,800.50";
            this.SetRate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbRate
            // 
            this.lbRate.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbRate.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRate.Location = new System.Drawing.Point(0, 23);
            this.lbRate.Name = "lbRate";
            this.lbRate.Size = new System.Drawing.Size(175, 23);
            this.lbRate.TabIndex = 155;
            this.lbRate.Text = "Current Rate";
            this.lbRate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbRate.Click += new System.EventHandler(this.lbRate_Click);
            this.lbRate.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.lbRate_HelpRequested);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(175, 23);
            this.label1.TabIndex = 154;
            this.label1.Text = "Target Rate";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // TankRemain
            // 
            this.TankRemain.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TankRemain.Location = new System.Drawing.Point(178, 98);
            this.TankRemain.Name = "TankRemain";
            this.TankRemain.Size = new System.Drawing.Size(97, 23);
            this.TankRemain.TabIndex = 153;
            this.TankRemain.Text = "5000";
            this.TankRemain.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.TankRemain.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.lbRemaining_HelpRequested);
            // 
            // lblUnits
            // 
            this.lblUnits.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUnits.Location = new System.Drawing.Point(145, 0);
            this.lblUnits.Name = "lblUnits";
            this.lblUnits.Size = new System.Drawing.Size(132, 23);
            this.lblUnits.TabIndex = 152;
            this.lblUnits.Text = "Imp Gal/Min";
            this.lblUnits.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // VolApplied
            // 
            this.VolApplied.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VolApplied.Location = new System.Drawing.Point(178, 123);
            this.VolApplied.Name = "VolApplied";
            this.VolApplied.Size = new System.Drawing.Size(97, 23);
            this.VolApplied.TabIndex = 148;
            this.VolApplied.Text = "0";
            this.VolApplied.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbApplied
            // 
            this.lbApplied.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbApplied.Location = new System.Drawing.Point(0, 123);
            this.lbApplied.Name = "lbApplied";
            this.lbApplied.Size = new System.Drawing.Size(192, 23);
            this.lbApplied.TabIndex = 151;
            this.lbApplied.Text = "Quantity Applied";
            this.lbApplied.Click += new System.EventHandler(this.label2_Click);
            // 
            // AreaDone
            // 
            this.AreaDone.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AreaDone.Location = new System.Drawing.Point(178, 73);
            this.AreaDone.Name = "AreaDone";
            this.AreaDone.Size = new System.Drawing.Size(97, 23);
            this.AreaDone.TabIndex = 147;
            this.AreaDone.Text = "0";
            this.AreaDone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbRateAmount
            // 
            this.lbRateAmount.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRateAmount.Location = new System.Drawing.Point(178, 23);
            this.lbRateAmount.Name = "lbRateAmount";
            this.lbRateAmount.Size = new System.Drawing.Size(97, 23);
            this.lbRateAmount.TabIndex = 146;
            this.lbRateAmount.Text = "1,800.50";
            this.lbRateAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lbRateAmount.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.lbRate_HelpRequested);
            // 
            // lbCoverage
            // 
            this.lbCoverage.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCoverage.Location = new System.Drawing.Point(0, 73);
            this.lbCoverage.Name = "lbCoverage";
            this.lbCoverage.Size = new System.Drawing.Size(175, 23);
            this.lbCoverage.TabIndex = 150;
            this.lbCoverage.Text = "Coverage";
            this.lbCoverage.Click += new System.EventHandler(this.lbCoverage_Click);
            // 
            // lbRemaining
            // 
            this.lbRemaining.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRemaining.Location = new System.Drawing.Point(0, 98);
            this.lbRemaining.Name = "lbRemaining";
            this.lbRemaining.Size = new System.Drawing.Size(192, 23);
            this.lbRemaining.TabIndex = 149;
            this.lbRemaining.Text = "Quantity Remain.";
            this.lbRemaining.Click += new System.EventHandler(this.label34_Click);
            this.lbRemaining.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.lbRemaining_HelpRequested);
            // 
            // panSummary
            // 
            this.panSummary.Controls.Add(this.idc0);
            this.panSummary.Controls.Add(this.prd4);
            this.panSummary.Controls.Add(this.rt4);
            this.panSummary.Controls.Add(this.idc4);
            this.panSummary.Controls.Add(this.prd3);
            this.panSummary.Controls.Add(this.rt3);
            this.panSummary.Controls.Add(this.idc3);
            this.panSummary.Controls.Add(this.prd2);
            this.panSummary.Controls.Add(this.rt2);
            this.panSummary.Controls.Add(this.idc2);
            this.panSummary.Controls.Add(this.prd1);
            this.panSummary.Controls.Add(this.rt1);
            this.panSummary.Controls.Add(this.idc1);
            this.panSummary.Controls.Add(this.prd0);
            this.panSummary.Controls.Add(this.rt0);
            this.panSummary.Location = new System.Drawing.Point(0, 0);
            this.panSummary.Name = "panSummary";
            this.panSummary.Size = new System.Drawing.Size(270, 150);
            this.panSummary.TabIndex = 100;
            // 
            // idc0
            // 
            this.idc0.BackColor = System.Drawing.SystemColors.Control;
            this.idc0.Cursor = System.Windows.Forms.Cursors.Default;
            this.idc0.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.idc0.Image = global::RateController.Properties.Resources.OffSmall;
            this.idc0.Location = new System.Drawing.Point(240, 0);
            this.idc0.Name = "idc0";
            this.idc0.Size = new System.Drawing.Size(30, 23);
            this.idc0.TabIndex = 122;
            this.idc0.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // prd4
            // 
            this.prd4.Cursor = System.Windows.Forms.Cursors.Default;
            this.prd4.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prd4.Location = new System.Drawing.Point(3, 124);
            this.prd4.Name = "prd4";
            this.prd4.Size = new System.Drawing.Size(143, 23);
            this.prd4.TabIndex = 121;
            this.prd4.Text = "5";
            // 
            // rt4
            // 
            this.rt4.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rt4.Location = new System.Drawing.Point(148, 124);
            this.rt4.Name = "rt4";
            this.rt4.Size = new System.Drawing.Size(90, 23);
            this.rt4.TabIndex = 120;
            this.rt4.Text = "0";
            this.rt4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // idc4
            // 
            this.idc4.BackColor = System.Drawing.SystemColors.Control;
            this.idc4.Cursor = System.Windows.Forms.Cursors.Default;
            this.idc4.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.idc4.Image = ((System.Drawing.Image)(resources.GetObject("idc4.Image")));
            this.idc4.Location = new System.Drawing.Point(240, 124);
            this.idc4.Name = "idc4";
            this.idc4.Size = new System.Drawing.Size(30, 23);
            this.idc4.TabIndex = 119;
            this.idc4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // prd3
            // 
            this.prd3.Cursor = System.Windows.Forms.Cursors.Default;
            this.prd3.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prd3.Location = new System.Drawing.Point(3, 93);
            this.prd3.Name = "prd3";
            this.prd3.Size = new System.Drawing.Size(143, 23);
            this.prd3.TabIndex = 118;
            this.prd3.Text = "4";
            // 
            // rt3
            // 
            this.rt3.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rt3.Location = new System.Drawing.Point(148, 93);
            this.rt3.Name = "rt3";
            this.rt3.Size = new System.Drawing.Size(90, 23);
            this.rt3.TabIndex = 117;
            this.rt3.Text = "0";
            this.rt3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // idc3
            // 
            this.idc3.BackColor = System.Drawing.SystemColors.Control;
            this.idc3.Cursor = System.Windows.Forms.Cursors.Default;
            this.idc3.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.idc3.Image = ((System.Drawing.Image)(resources.GetObject("idc3.Image")));
            this.idc3.Location = new System.Drawing.Point(240, 93);
            this.idc3.Name = "idc3";
            this.idc3.Size = new System.Drawing.Size(30, 23);
            this.idc3.TabIndex = 116;
            this.idc3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // prd2
            // 
            this.prd2.Cursor = System.Windows.Forms.Cursors.Default;
            this.prd2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prd2.Location = new System.Drawing.Point(3, 62);
            this.prd2.Name = "prd2";
            this.prd2.Size = new System.Drawing.Size(143, 23);
            this.prd2.TabIndex = 115;
            this.prd2.Text = "3";
            // 
            // rt2
            // 
            this.rt2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rt2.Location = new System.Drawing.Point(148, 62);
            this.rt2.Name = "rt2";
            this.rt2.Size = new System.Drawing.Size(90, 23);
            this.rt2.TabIndex = 114;
            this.rt2.Text = "0";
            this.rt2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // idc2
            // 
            this.idc2.BackColor = System.Drawing.SystemColors.Control;
            this.idc2.Cursor = System.Windows.Forms.Cursors.Default;
            this.idc2.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.idc2.Image = ((System.Drawing.Image)(resources.GetObject("idc2.Image")));
            this.idc2.Location = new System.Drawing.Point(240, 62);
            this.idc2.Name = "idc2";
            this.idc2.Size = new System.Drawing.Size(30, 23);
            this.idc2.TabIndex = 113;
            this.idc2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // prd1
            // 
            this.prd1.Cursor = System.Windows.Forms.Cursors.Default;
            this.prd1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prd1.Location = new System.Drawing.Point(3, 31);
            this.prd1.Name = "prd1";
            this.prd1.Size = new System.Drawing.Size(143, 23);
            this.prd1.TabIndex = 112;
            this.prd1.Text = "2";
            // 
            // rt1
            // 
            this.rt1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rt1.Location = new System.Drawing.Point(148, 31);
            this.rt1.Name = "rt1";
            this.rt1.Size = new System.Drawing.Size(90, 23);
            this.rt1.TabIndex = 111;
            this.rt1.Text = "0";
            this.rt1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // idc1
            // 
            this.idc1.BackColor = System.Drawing.SystemColors.Control;
            this.idc1.Cursor = System.Windows.Forms.Cursors.Default;
            this.idc1.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.idc1.Image = ((System.Drawing.Image)(resources.GetObject("idc1.Image")));
            this.idc1.Location = new System.Drawing.Point(240, 31);
            this.idc1.Name = "idc1";
            this.idc1.Size = new System.Drawing.Size(30, 23);
            this.idc1.TabIndex = 110;
            this.idc1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // prd0
            // 
            this.prd0.Cursor = System.Windows.Forms.Cursors.Default;
            this.prd0.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prd0.Location = new System.Drawing.Point(3, 0);
            this.prd0.Name = "prd0";
            this.prd0.Size = new System.Drawing.Size(143, 23);
            this.prd0.TabIndex = 109;
            this.prd0.Text = "1";
            // 
            // rt0
            // 
            this.rt0.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rt0.Location = new System.Drawing.Point(148, 0);
            this.rt0.Name = "rt0";
            this.rt0.Size = new System.Drawing.Size(90, 23);
            this.rt0.TabIndex = 108;
            this.rt0.Text = "0";
            this.rt0.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // timerNano
            // 
            this.timerNano.Enabled = true;
            this.timerNano.Interval = 50;
            this.timerNano.Tick += new System.EventHandler(this.timerNano_Tick);
            // 
            // mnuSettings
            // 
            this.mnuSettings.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mnuSettings.ImageScalingSize = new System.Drawing.Size(36, 36);
            this.mnuSettings.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MnuProducts,
            this.MnuSections,
            this.MnuOptions,
            this.MnuAbout});
            this.mnuSettings.Name = "mnuSettings";
            this.mnuSettings.Size = new System.Drawing.Size(201, 194);
            // 
            // MnuProducts
            // 
            this.MnuProducts.Image = ((System.Drawing.Image)(resources.GetObject("MnuProducts.Image")));
            this.MnuProducts.Name = "MnuProducts";
            this.MnuProducts.Size = new System.Drawing.Size(200, 42);
            this.MnuProducts.Text = "Products";
            this.MnuProducts.Click += new System.EventHandler(this.productsToolStripMenuItem_Click);
            // 
            // MnuSections
            // 
            this.MnuSections.Image = ((System.Drawing.Image)(resources.GetObject("MnuSections.Image")));
            this.MnuSections.Name = "MnuSections";
            this.MnuSections.Size = new System.Drawing.Size(200, 42);
            this.MnuSections.Text = "Sections";
            this.MnuSections.Click += new System.EventHandler(this.sectionsToolStripMenuItem_Click);
            // 
            // MnuOptions
            // 
            this.MnuOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuRelays,
            this.MnuComm,
            this.toolStripSeparator1,
            this.MnuNew,
            this.MnuOpen,
            this.MnuSaveAs,
            this.toolStripSeparator2,
            this.MnuLanguage,
            this.MnuConfig,
            this.MnuFirmware});
            this.MnuOptions.Image = global::RateController.Properties.Resources._7504229_hamburger_menu_list_options_icon;
            this.MnuOptions.Name = "MnuOptions";
            this.MnuOptions.Size = new System.Drawing.Size(200, 42);
            this.MnuOptions.Text = "Options";
            // 
            // mnuRelays
            // 
            this.mnuRelays.Image = global::RateController.Properties.Resources.Industry_Circuit_icon;
            this.mnuRelays.Name = "mnuRelays";
            this.mnuRelays.Size = new System.Drawing.Size(200, 42);
            this.mnuRelays.Text = "Relays";
            this.mnuRelays.Click += new System.EventHandler(this.MnuRelays_Click);
            // 
            // MnuComm
            // 
            this.MnuComm.Image = global::RateController.Properties.Resources.cableusb_119960;
            this.MnuComm.Name = "MnuComm";
            this.MnuComm.Size = new System.Drawing.Size(200, 42);
            this.MnuComm.Text = "Comm";
            this.MnuComm.Click += new System.EventHandler(this.commToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(197, 6);
            // 
            // MnuNew
            // 
            this.MnuNew.Image = global::RateController.Properties.Resources.FileNew;
            this.MnuNew.Name = "MnuNew";
            this.MnuNew.Size = new System.Drawing.Size(200, 42);
            this.MnuNew.Text = "New file";
            this.MnuNew.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // MnuOpen
            // 
            this.MnuOpen.Image = global::RateController.Properties.Resources.OpenFile;
            this.MnuOpen.Name = "MnuOpen";
            this.MnuOpen.Size = new System.Drawing.Size(200, 42);
            this.MnuOpen.Text = "Open";
            this.MnuOpen.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // MnuSaveAs
            // 
            this.MnuSaveAs.Image = global::RateController.Properties.Resources.close1;
            this.MnuSaveAs.Name = "MnuSaveAs";
            this.MnuSaveAs.Size = new System.Drawing.Size(200, 42);
            this.MnuSaveAs.Text = "Save As";
            this.MnuSaveAs.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(197, 6);
            // 
            // MnuLanguage
            // 
            this.MnuLanguage.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MnuDeustch,
            this.MnuEnglish,
            this.MnuNederlands});
            this.MnuLanguage.Image = global::RateController.Properties.Resources._7791659_language_speak_communication_speech_chat_icon;
            this.MnuLanguage.Name = "MnuLanguage";
            this.MnuLanguage.Size = new System.Drawing.Size(200, 42);
            this.MnuLanguage.Text = "Language";
            // 
            // MnuDeustch
            // 
            this.MnuDeustch.Name = "MnuDeustch";
            this.MnuDeustch.Size = new System.Drawing.Size(175, 28);
            this.MnuDeustch.Text = "Deustch";
            this.MnuDeustch.Click += new System.EventHandler(this.MnuDeustch_Click);
            // 
            // MnuEnglish
            // 
            this.MnuEnglish.Name = "MnuEnglish";
            this.MnuEnglish.Size = new System.Drawing.Size(175, 28);
            this.MnuEnglish.Text = "English";
            this.MnuEnglish.Click += new System.EventHandler(this.MnuEnglish_Click);
            // 
            // MnuNederlands
            // 
            this.MnuNederlands.Name = "MnuNederlands";
            this.MnuNederlands.Size = new System.Drawing.Size(175, 28);
            this.MnuNederlands.Text = "Nederlands";
            this.MnuNederlands.Click += new System.EventHandler(this.MnuNederlands_Click);
            // 
            // MnuConfig
            // 
            this.MnuConfig.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.teensieToolStripMenuItem,
            this.nanoToolStripMenuItem});
            this.MnuConfig.Image = global::RateController.Properties.Resources.wrench;
            this.MnuConfig.Name = "MnuConfig";
            this.MnuConfig.Size = new System.Drawing.Size(200, 42);
            this.MnuConfig.Text = "PCB config";
            // 
            // teensieToolStripMenuItem
            // 
            this.teensieToolStripMenuItem.Name = "teensieToolStripMenuItem";
            this.teensieToolStripMenuItem.Size = new System.Drawing.Size(180, 28);
            this.teensieToolStripMenuItem.Text = "Teensie";
            this.teensieToolStripMenuItem.Click += new System.EventHandler(this.teensieToolStripMenuItem_Click);
            // 
            // nanoToolStripMenuItem
            // 
            this.nanoToolStripMenuItem.Name = "nanoToolStripMenuItem";
            this.nanoToolStripMenuItem.Size = new System.Drawing.Size(180, 28);
            this.nanoToolStripMenuItem.Text = "Nano";
            this.nanoToolStripMenuItem.Click += new System.EventHandler(this.nanoToolStripMenuItem_Click);
            // 
            // MnuFirmware
            // 
            this.MnuFirmware.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.teensieToolStripMenuItem1,
            this.nanoToolStripMenuItem1});
            this.MnuFirmware.Image = global::RateController.Properties.Resources._3556117_arrow_ui_up_icon;
            this.MnuFirmware.Name = "MnuFirmware";
            this.MnuFirmware.Size = new System.Drawing.Size(200, 42);
            this.MnuFirmware.Text = "Firmware";
            // 
            // MnuAbout
            // 
            this.MnuAbout.Image = global::RateController.Properties.Resources.R674d5dd067acbd409ff50db6d0647f5d;
            this.MnuAbout.Name = "MnuAbout";
            this.MnuAbout.Size = new System.Drawing.Size(200, 42);
            this.MnuAbout.Text = "About";
            this.MnuAbout.Click += new System.EventHandler(this.MnuAbout_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "rcs";
            this.openFileDialog1.Filter = "RC Settings|*.rcs";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "rcs";
            this.saveFileDialog1.Filter = "RC Settings|*.rcs";
            // 
            // btnRight
            // 
            this.btnRight.Image = global::RateController.Properties.Resources.ArrowRight1;
            this.btnRight.Location = new System.Drawing.Point(138, 153);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(63, 62);
            this.btnRight.TabIndex = 143;
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnLeft
            // 
            this.btnLeft.Image = global::RateController.Properties.Resources.ArrowLeft1;
            this.btnLeft.Location = new System.Drawing.Point(71, 153);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(60, 62);
            this.btnLeft.TabIndex = 142;
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSettings.Image = ((System.Drawing.Image)(resources.GetObject("btnSettings.Image")));
            this.btnSettings.Location = new System.Drawing.Point(4, 153);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(60, 62);
            this.btnSettings.TabIndex = 73;
            this.btnSettings.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.button3_Click);
            // 
            // teensieToolStripMenuItem1
            // 
            this.teensieToolStripMenuItem1.Name = "teensieToolStripMenuItem1";
            this.teensieToolStripMenuItem1.Size = new System.Drawing.Size(180, 28);
            this.teensieToolStripMenuItem1.Text = "Teensie";
            this.teensieToolStripMenuItem1.Click += new System.EventHandler(this.teensieToolStripMenuItem1_Click);
            // 
            // nanoToolStripMenuItem1
            // 
            this.nanoToolStripMenuItem1.Name = "nanoToolStripMenuItem1";
            this.nanoToolStripMenuItem1.Size = new System.Drawing.Size(180, 28);
            this.nanoToolStripMenuItem1.Text = "Nano";
            this.nanoToolStripMenuItem1.Click += new System.EventHandler(this.nanoToolStripMenuItem1_Click);
            // 
            // FormStart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(276, 220);
            this.Controls.Add(this.panProducts);
            this.Controls.Add(this.panSummary);
            this.Controls.Add(this.lbAogConnected);
            this.Controls.Add(this.lbArduinoConnected);
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.btnLeft);
            this.Controls.Add(this.btnSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormStart";
            this.Text = "Rate Controller";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.Transparent;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormRateControl_FormClosed);
            this.Load += new System.EventHandler(this.FormStart_Load);
            this.panProducts.ResumeLayout(false);
            this.panSummary.ResumeLayout(false);
            this.mnuSettings.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Timer timerMain;
        private System.Windows.Forms.Label lbArduinoConnected;
        private System.Windows.Forms.Label lbAogConnected;
        private System.Windows.Forms.Panel panProducts;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lbProduct;
        private System.Windows.Forms.Label SetRate;
        private System.Windows.Forms.Label lbRate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label TankRemain;
        private System.Windows.Forms.Label lblUnits;
        private System.Windows.Forms.Label VolApplied;
        private System.Windows.Forms.Label lbApplied;
        private System.Windows.Forms.Label AreaDone;
        private System.Windows.Forms.Label lbRateAmount;
        private System.Windows.Forms.Label lbCoverage;
        private System.Windows.Forms.Label lbRemaining;
        private System.Windows.Forms.Panel panSummary;
        private System.Windows.Forms.Label idc0;
        private System.Windows.Forms.Label prd4;
        private System.Windows.Forms.Label rt4;
        private System.Windows.Forms.Label idc4;
        private System.Windows.Forms.Label prd3;
        private System.Windows.Forms.Label rt3;
        private System.Windows.Forms.Label idc3;
        private System.Windows.Forms.Label prd2;
        private System.Windows.Forms.Label rt2;
        private System.Windows.Forms.Label idc2;
        private System.Windows.Forms.Label prd1;
        private System.Windows.Forms.Label rt1;
        private System.Windows.Forms.Label idc1;
        private System.Windows.Forms.Label prd0;
        private System.Windows.Forms.Label rt0;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Timer timerNano;
        private System.Windows.Forms.ContextMenuStrip mnuSettings;
        private System.Windows.Forms.ToolStripMenuItem MnuProducts;
        private System.Windows.Forms.ToolStripMenuItem MnuSections;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button btAlarm;
        private System.Windows.Forms.ToolStripMenuItem MnuOptions;
        private System.Windows.Forms.ToolStripMenuItem MnuLanguage;
        private System.Windows.Forms.ToolStripMenuItem MnuConfig;
        private System.Windows.Forms.ToolStripMenuItem MnuFirmware;
        private System.Windows.Forms.ToolStripMenuItem MnuDeustch;
        private System.Windows.Forms.ToolStripMenuItem MnuEnglish;
        private System.Windows.Forms.ToolStripMenuItem MnuNederlands;
        private System.Windows.Forms.ToolStripMenuItem MnuComm;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem MnuNew;
        private System.Windows.Forms.ToolStripMenuItem MnuOpen;
        private System.Windows.Forms.ToolStripMenuItem MnuSaveAs;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mnuRelays;
        private System.Windows.Forms.ToolStripMenuItem MnuAbout;
        private System.Windows.Forms.ToolStripMenuItem teensieToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nanoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem teensieToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem nanoToolStripMenuItem1;
    }
}