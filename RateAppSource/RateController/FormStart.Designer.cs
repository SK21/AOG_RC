
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
            this.lbTarget = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lbProduct = new System.Windows.Forms.Label();
            this.SetRate = new System.Windows.Forms.Label();
            this.lbRate = new System.Windows.Forms.Label();
            this.TankRemain = new System.Windows.Forms.Label();
            this.lblUnits = new System.Windows.Forms.Label();
            this.AreaDone = new System.Windows.Forms.Label();
            this.lbRateAmount = new System.Windows.Forms.Label();
            this.lbCoverage = new System.Windows.Forms.Label();
            this.lbRemaining = new System.Windows.Forms.Label();
            this.panSummary = new System.Windows.Forms.Panel();
            this.tg3 = new System.Windows.Forms.Label();
            this.tg2 = new System.Windows.Forms.Label();
            this.tg1 = new System.Windows.Forms.Label();
            this.tg0 = new System.Windows.Forms.Label();
            this.idc5 = new System.Windows.Forms.Label();
            this.rt5 = new System.Windows.Forms.Label();
            this.prd5 = new System.Windows.Forms.Label();
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
            this.mnuSettings = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MnuProducts = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuSections = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuRelays = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuComm = new System.Windows.Forms.ToolStripMenuItem();
            this.calibrateToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.networkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pressuresToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.commDiagnosticsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.panFan = new System.Windows.Forms.Panel();
            this.btnFan = new System.Windows.Forms.Button();
            this.lbOn = new System.Windows.Forms.Label();
            this.lbOff = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbTargetRPM = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbFanRate = new System.Windows.Forms.Label();
            this.lbCurrentRPM = new System.Windows.Forms.Label();
            this.lbFan = new System.Windows.Forms.Label();
            this.timerPIDs = new System.Windows.Forms.Timer(this.components);
            this.btnRight = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btAlarm = new System.Windows.Forms.Button();
            this.panProducts.SuspendLayout();
            this.panSummary.SuspendLayout();
            this.mnuSettings.SuspendLayout();
            this.panFan.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerMain
            // 
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
            this.panProducts.Controls.Add(this.lbTarget);
            this.panProducts.Controls.Add(this.groupBox3);
            this.panProducts.Controls.Add(this.lbProduct);
            this.panProducts.Controls.Add(this.SetRate);
            this.panProducts.Controls.Add(this.lbRate);
            this.panProducts.Controls.Add(this.TankRemain);
            this.panProducts.Controls.Add(this.lblUnits);
            this.panProducts.Controls.Add(this.AreaDone);
            this.panProducts.Controls.Add(this.lbRateAmount);
            this.panProducts.Controls.Add(this.lbCoverage);
            this.panProducts.Controls.Add(this.lbRemaining);
            this.panProducts.Location = new System.Drawing.Point(0, 0);
            this.panProducts.Name = "panProducts";
            this.panProducts.Size = new System.Drawing.Size(271, 150);
            this.panProducts.TabIndex = 50;
            // 
            // lbTarget
            // 
            this.lbTarget.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbTarget.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTarget.Location = new System.Drawing.Point(0, 60);
            this.lbTarget.Name = "lbTarget";
            this.lbTarget.Size = new System.Drawing.Size(201, 23);
            this.lbTarget.TabIndex = 159;
            this.lbTarget.Text = "Target Rate";
            this.lbTarget.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbTarget.Click += new System.EventHandler(this.lbTarget_Click);
            this.lbTarget.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.lbTarget_HelpRequested);
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
            this.SetRate.Location = new System.Drawing.Point(188, 60);
            this.SetRate.Name = "SetRate";
            this.SetRate.Size = new System.Drawing.Size(89, 23);
            this.SetRate.TabIndex = 156;
            this.SetRate.Text = "1,800.50";
            this.SetRate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbRate
            // 
            this.lbRate.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbRate.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRate.Location = new System.Drawing.Point(0, 30);
            this.lbRate.Name = "lbRate";
            this.lbRate.Size = new System.Drawing.Size(175, 23);
            this.lbRate.TabIndex = 155;
            this.lbRate.Text = "Current Rate";
            this.lbRate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbRate.Click += new System.EventHandler(this.lbRate_Click);
            this.lbRate.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.lbRate_HelpRequested);
            // 
            // TankRemain
            // 
            this.TankRemain.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TankRemain.Location = new System.Drawing.Point(188, 120);
            this.TankRemain.Name = "TankRemain";
            this.TankRemain.Size = new System.Drawing.Size(89, 23);
            this.TankRemain.TabIndex = 153;
            this.TankRemain.Text = "50000.1";
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
            // AreaDone
            // 
            this.AreaDone.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AreaDone.Location = new System.Drawing.Point(188, 90);
            this.AreaDone.Name = "AreaDone";
            this.AreaDone.Size = new System.Drawing.Size(89, 23);
            this.AreaDone.TabIndex = 147;
            this.AreaDone.Text = "0";
            this.AreaDone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.AreaDone.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.lbCoverage_HelpRequested);
            // 
            // lbRateAmount
            // 
            this.lbRateAmount.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRateAmount.Location = new System.Drawing.Point(188, 32);
            this.lbRateAmount.Name = "lbRateAmount";
            this.lbRateAmount.Size = new System.Drawing.Size(89, 23);
            this.lbRateAmount.TabIndex = 146;
            this.lbRateAmount.Text = "1,800.50";
            this.lbRateAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lbRateAmount.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.lbRate_HelpRequested);
            // 
            // lbCoverage
            // 
            this.lbCoverage.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCoverage.Location = new System.Drawing.Point(0, 90);
            this.lbCoverage.Name = "lbCoverage";
            this.lbCoverage.Size = new System.Drawing.Size(175, 23);
            this.lbCoverage.TabIndex = 150;
            this.lbCoverage.Text = "Coverage";
            this.lbCoverage.Click += new System.EventHandler(this.lbCoverage_Click);
            this.lbCoverage.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.lbCoverage_HelpRequested);
            // 
            // lbRemaining
            // 
            this.lbRemaining.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRemaining.Location = new System.Drawing.Point(0, 120);
            this.lbRemaining.Name = "lbRemaining";
            this.lbRemaining.Size = new System.Drawing.Size(201, 23);
            this.lbRemaining.TabIndex = 149;
            this.lbRemaining.Text = "Quantity Applied ...";
            this.lbRemaining.Click += new System.EventHandler(this.label34_Click);
            this.lbRemaining.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.lbRemaining_HelpRequested);
            // 
            // panSummary
            // 
            this.panSummary.Controls.Add(this.tg3);
            this.panSummary.Controls.Add(this.tg2);
            this.panSummary.Controls.Add(this.tg1);
            this.panSummary.Controls.Add(this.tg0);
            this.panSummary.Controls.Add(this.idc5);
            this.panSummary.Controls.Add(this.rt5);
            this.panSummary.Controls.Add(this.prd5);
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
            this.panSummary.Location = new System.Drawing.Point(301, 134);
            this.panSummary.Name = "panSummary";
            this.panSummary.Size = new System.Drawing.Size(270, 200);
            this.panSummary.TabIndex = 100;
            // 
            // tg3
            // 
            this.tg3.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tg3.Location = new System.Drawing.Point(55, 87);
            this.tg3.Name = "tg3";
            this.tg3.Size = new System.Drawing.Size(90, 23);
            this.tg3.TabIndex = 129;
            this.tg3.Text = "0";
            this.tg3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tg2
            // 
            this.tg2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tg2.Location = new System.Drawing.Point(55, 58);
            this.tg2.Name = "tg2";
            this.tg2.Size = new System.Drawing.Size(90, 23);
            this.tg2.TabIndex = 128;
            this.tg2.Text = "0";
            this.tg2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tg1
            // 
            this.tg1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tg1.Location = new System.Drawing.Point(55, 29);
            this.tg1.Name = "tg1";
            this.tg1.Size = new System.Drawing.Size(90, 23);
            this.tg1.TabIndex = 127;
            this.tg1.Text = "0";
            this.tg1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tg0
            // 
            this.tg0.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tg0.Location = new System.Drawing.Point(55, 0);
            this.tg0.Name = "tg0";
            this.tg0.Size = new System.Drawing.Size(90, 23);
            this.tg0.TabIndex = 126;
            this.tg0.Text = "0";
            this.tg0.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // idc5
            // 
            this.idc5.BackColor = System.Drawing.SystemColors.Control;
            this.idc5.Cursor = System.Windows.Forms.Cursors.Default;
            this.idc5.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.idc5.Image = ((System.Drawing.Image)(resources.GetObject("idc5.Image")));
            this.idc5.Location = new System.Drawing.Point(240, 145);
            this.idc5.Name = "idc5";
            this.idc5.Size = new System.Drawing.Size(30, 23);
            this.idc5.TabIndex = 125;
            this.idc5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // rt5
            // 
            this.rt5.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rt5.Location = new System.Drawing.Point(148, 145);
            this.rt5.Name = "rt5";
            this.rt5.Size = new System.Drawing.Size(90, 23);
            this.rt5.TabIndex = 124;
            this.rt5.Text = "0";
            this.rt5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // prd5
            // 
            this.prd5.Cursor = System.Windows.Forms.Cursors.Default;
            this.prd5.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prd5.Location = new System.Drawing.Point(3, 145);
            this.prd5.Name = "prd5";
            this.prd5.Size = new System.Drawing.Size(74, 23);
            this.prd5.TabIndex = 123;
            this.prd5.Text = "6";
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
            this.prd4.Location = new System.Drawing.Point(3, 116);
            this.prd4.Name = "prd4";
            this.prd4.Size = new System.Drawing.Size(74, 23);
            this.prd4.TabIndex = 121;
            this.prd4.Text = "5";
            // 
            // rt4
            // 
            this.rt4.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rt4.Location = new System.Drawing.Point(148, 116);
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
            this.idc4.Location = new System.Drawing.Point(240, 116);
            this.idc4.Name = "idc4";
            this.idc4.Size = new System.Drawing.Size(30, 23);
            this.idc4.TabIndex = 119;
            this.idc4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // prd3
            // 
            this.prd3.Cursor = System.Windows.Forms.Cursors.Default;
            this.prd3.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prd3.Location = new System.Drawing.Point(3, 87);
            this.prd3.Name = "prd3";
            this.prd3.Size = new System.Drawing.Size(50, 23);
            this.prd3.TabIndex = 118;
            this.prd3.Text = "4";
            // 
            // rt3
            // 
            this.rt3.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rt3.Location = new System.Drawing.Point(148, 87);
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
            this.idc3.Location = new System.Drawing.Point(240, 87);
            this.idc3.Name = "idc3";
            this.idc3.Size = new System.Drawing.Size(30, 23);
            this.idc3.TabIndex = 116;
            this.idc3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // prd2
            // 
            this.prd2.Cursor = System.Windows.Forms.Cursors.Default;
            this.prd2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prd2.Location = new System.Drawing.Point(3, 58);
            this.prd2.Name = "prd2";
            this.prd2.Size = new System.Drawing.Size(50, 23);
            this.prd2.TabIndex = 115;
            this.prd2.Text = "3";
            // 
            // rt2
            // 
            this.rt2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rt2.Location = new System.Drawing.Point(148, 58);
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
            this.idc2.Location = new System.Drawing.Point(240, 58);
            this.idc2.Name = "idc2";
            this.idc2.Size = new System.Drawing.Size(30, 23);
            this.idc2.TabIndex = 113;
            this.idc2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // prd1
            // 
            this.prd1.Cursor = System.Windows.Forms.Cursors.Default;
            this.prd1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prd1.Location = new System.Drawing.Point(3, 29);
            this.prd1.Name = "prd1";
            this.prd1.Size = new System.Drawing.Size(50, 23);
            this.prd1.TabIndex = 112;
            this.prd1.Text = "2";
            // 
            // rt1
            // 
            this.rt1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rt1.Location = new System.Drawing.Point(148, 29);
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
            this.idc1.Location = new System.Drawing.Point(240, 29);
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
            this.prd0.Size = new System.Drawing.Size(50, 23);
            this.prd0.TabIndex = 109;
            this.prd0.Text = "1";
            // 
            // rt0
            // 
            this.rt0.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rt0.Location = new System.Drawing.Point(147, 0);
            this.rt0.Name = "rt0";
            this.rt0.Size = new System.Drawing.Size(90, 23);
            this.rt0.TabIndex = 108;
            this.rt0.Text = "0";
            this.rt0.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // mnuSettings
            // 
            this.mnuSettings.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mnuSettings.ImageScalingSize = new System.Drawing.Size(36, 36);
            this.mnuSettings.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MnuProducts,
            this.MnuSections,
            this.MnuRelays,
            this.MnuComm,
            this.calibrateToolStripMenuItem1,
            this.networkToolStripMenuItem,
            this.pressuresToolStripMenuItem1,
            this.commDiagnosticsToolStripMenuItem,
            this.MnuOptions,
            this.toolStripSeparator1,
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator4,
            this.exitToolStripMenuItem});
            this.mnuSettings.Name = "mnuSettings";
            this.mnuSettings.Size = new System.Drawing.Size(254, 584);
            this.mnuSettings.Opening += new System.ComponentModel.CancelEventHandler(this.mnuSettings_Opening);
            // 
            // MnuProducts
            // 
            this.MnuProducts.Image = global::RateController.Properties.Resources.spray2;
            this.MnuProducts.Name = "MnuProducts";
            this.MnuProducts.Size = new System.Drawing.Size(253, 42);
            this.MnuProducts.Text = "Products";
            this.MnuProducts.Click += new System.EventHandler(this.productsToolStripMenuItem_Click);
            // 
            // MnuSections
            // 
            this.MnuSections.Image = global::RateController.Properties.Resources.Sec1;
            this.MnuSections.Name = "MnuSections";
            this.MnuSections.Size = new System.Drawing.Size(253, 42);
            this.MnuSections.Text = "Sections";
            this.MnuSections.Click += new System.EventHandler(this.sectionsToolStripMenuItem_Click);
            // 
            // MnuRelays
            // 
            this.MnuRelays.Image = global::RateController.Properties.Resources.Industry_Circuit_icon;
            this.MnuRelays.Name = "MnuRelays";
            this.MnuRelays.Size = new System.Drawing.Size(253, 42);
            this.MnuRelays.Text = "Relays";
            this.MnuRelays.Click += new System.EventHandler(this.MnuRelays_Click_1);
            // 
            // MnuComm
            // 
            this.MnuComm.Image = global::RateController.Properties.Resources.cableusb_119960;
            this.MnuComm.Name = "MnuComm";
            this.MnuComm.Size = new System.Drawing.Size(253, 42);
            this.MnuComm.Text = "Comm";
            this.MnuComm.Click += new System.EventHandler(this.MnuComm_Click);
            // 
            // calibrateToolStripMenuItem1
            // 
            this.calibrateToolStripMenuItem1.Image = global::RateController.Properties.Resources.RateCal;
            this.calibrateToolStripMenuItem1.Name = "calibrateToolStripMenuItem1";
            this.calibrateToolStripMenuItem1.Size = new System.Drawing.Size(253, 42);
            this.calibrateToolStripMenuItem1.Text = "Calibrate";
            this.calibrateToolStripMenuItem1.Click += new System.EventHandler(this.calibrateToolStripMenuItem1_Click);
            // 
            // networkToolStripMenuItem
            // 
            this.networkToolStripMenuItem.Image = global::RateController.Properties.Resources.SubnetSend;
            this.networkToolStripMenuItem.Name = "networkToolStripMenuItem";
            this.networkToolStripMenuItem.Size = new System.Drawing.Size(253, 42);
            this.networkToolStripMenuItem.Text = "Modules";
            this.networkToolStripMenuItem.Click += new System.EventHandler(this.networkToolStripMenuItem_Click);
            // 
            // pressuresToolStripMenuItem1
            // 
            this.pressuresToolStripMenuItem1.Image = global::RateController.Properties.Resources.pressure;
            this.pressuresToolStripMenuItem1.Name = "pressuresToolStripMenuItem1";
            this.pressuresToolStripMenuItem1.Size = new System.Drawing.Size(253, 42);
            this.pressuresToolStripMenuItem1.Text = "Pressures";
            this.pressuresToolStripMenuItem1.Click += new System.EventHandler(this.pressuresToolStripMenuItem1_Click);
            // 
            // commDiagnosticsToolStripMenuItem
            // 
            this.commDiagnosticsToolStripMenuItem.Image = global::RateController.Properties.Resources.Diagnostics;
            this.commDiagnosticsToolStripMenuItem.Name = "commDiagnosticsToolStripMenuItem";
            this.commDiagnosticsToolStripMenuItem.Size = new System.Drawing.Size(253, 42);
            this.commDiagnosticsToolStripMenuItem.Text = "Comm Diagnostics";
            this.commDiagnosticsToolStripMenuItem.Click += new System.EventHandler(this.commDiagnosticsToolStripMenuItem_Click);
            // 
            // MnuOptions
            // 
            this.MnuOptions.Image = global::RateController.Properties.Resources.Menu;
            this.MnuOptions.Name = "MnuOptions";
            this.MnuOptions.Size = new System.Drawing.Size(253, 42);
            this.MnuOptions.Text = "Options";
            this.MnuOptions.Click += new System.EventHandler(this.MnuOptions_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(250, 6);
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Image = global::RateController.Properties.Resources.FileNew1;
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(253, 42);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click_1);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = global::RateController.Properties.Resources.OpenFile1;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(253, 42);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Image = global::RateController.Properties.Resources.close2;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(253, 42);
            this.saveAsToolStripMenuItem.Text = "Save As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(250, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Image = global::RateController.Properties.Resources.SwitchOff;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(253, 42);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
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
            // panFan
            // 
            this.panFan.Controls.Add(this.btnFan);
            this.panFan.Controls.Add(this.lbOn);
            this.panFan.Controls.Add(this.lbOff);
            this.panFan.Controls.Add(this.groupBox1);
            this.panFan.Controls.Add(this.lbTargetRPM);
            this.panFan.Controls.Add(this.label4);
            this.panFan.Controls.Add(this.lbFanRate);
            this.panFan.Controls.Add(this.lbCurrentRPM);
            this.panFan.Controls.Add(this.lbFan);
            this.panFan.Location = new System.Drawing.Point(292, 362);
            this.panFan.Name = "panFan";
            this.panFan.Size = new System.Drawing.Size(270, 161);
            this.panFan.TabIndex = 157;
            // 
            // btnFan
            // 
            this.btnFan.BackColor = System.Drawing.Color.Transparent;
            this.btnFan.FlatAppearance.BorderSize = 0;
            this.btnFan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFan.Image = global::RateController.Properties.Resources.FanOff;
            this.btnFan.Location = new System.Drawing.Point(117, 98);
            this.btnFan.Name = "btnFan";
            this.btnFan.Size = new System.Drawing.Size(50, 50);
            this.btnFan.TabIndex = 203;
            this.btnFan.UseVisualStyleBackColor = false;
            this.btnFan.Click += new System.EventHandler(this.btnFan_Click);
            // 
            // lbOn
            // 
            this.lbOn.BackColor = System.Drawing.SystemColors.Control;
            this.lbOn.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbOn.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbOn.Image = global::RateController.Properties.Resources.OnSmall;
            this.lbOn.Location = new System.Drawing.Point(235, 3);
            this.lbOn.Name = "lbOn";
            this.lbOn.Size = new System.Drawing.Size(30, 23);
            this.lbOn.TabIndex = 169;
            this.lbOn.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lbOff
            // 
            this.lbOff.BackColor = System.Drawing.SystemColors.Control;
            this.lbOff.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbOff.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbOff.Image = global::RateController.Properties.Resources.OffSmall;
            this.lbOff.Location = new System.Drawing.Point(235, 3);
            this.lbOff.Name = "lbOff";
            this.lbOff.Size = new System.Drawing.Size(30, 23);
            this.lbOff.TabIndex = 168;
            this.lbOff.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(9, 24);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(252, 1);
            this.groupBox1.TabIndex = 165;
            this.groupBox1.TabStop = false;
            // 
            // lbTargetRPM
            // 
            this.lbTargetRPM.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTargetRPM.Location = new System.Drawing.Point(188, 60);
            this.lbTargetRPM.Name = "lbTargetRPM";
            this.lbTargetRPM.Size = new System.Drawing.Size(89, 23);
            this.lbTargetRPM.TabIndex = 162;
            this.lbTargetRPM.Text = "1,800.50";
            this.lbTargetRPM.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Cursor = System.Windows.Forms.Cursors.Default;
            this.label4.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(0, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(201, 23);
            this.label4.TabIndex = 161;
            this.label4.Text = "Target RPM";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbFanRate
            // 
            this.lbFanRate.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbFanRate.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbFanRate.Location = new System.Drawing.Point(0, 30);
            this.lbFanRate.Name = "lbFanRate";
            this.lbFanRate.Size = new System.Drawing.Size(175, 23);
            this.lbFanRate.TabIndex = 160;
            this.lbFanRate.Text = "Current RPM";
            this.lbFanRate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbCurrentRPM
            // 
            this.lbCurrentRPM.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCurrentRPM.Location = new System.Drawing.Point(188, 30);
            this.lbCurrentRPM.Name = "lbCurrentRPM";
            this.lbCurrentRPM.Size = new System.Drawing.Size(89, 23);
            this.lbCurrentRPM.TabIndex = 159;
            this.lbCurrentRPM.Text = "1,800.50";
            this.lbCurrentRPM.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbFan
            // 
            this.lbFan.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbFan.Location = new System.Drawing.Point(3, 0);
            this.lbFan.Name = "lbFan";
            this.lbFan.Size = new System.Drawing.Size(198, 23);
            this.lbFan.TabIndex = 158;
            this.lbFan.Text = "Herbicide";
            // 
            // timerPIDs
            // 
            this.timerPIDs.Enabled = true;
            this.timerPIDs.Interval = 5000;
            this.timerPIDs.Tick += new System.EventHandler(this.timerPIDs_Tick);
            // 
            // btnRight
            // 
            this.btnRight.BackColor = System.Drawing.Color.Transparent;
            this.btnRight.FlatAppearance.BorderSize = 0;
            this.btnRight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRight.Image = global::RateController.Properties.Resources.ArrowRight1;
            this.btnRight.Location = new System.Drawing.Point(138, 153);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(63, 62);
            this.btnRight.TabIndex = 143;
            this.btnRight.UseVisualStyleBackColor = false;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnLeft
            // 
            this.btnLeft.BackColor = System.Drawing.Color.Transparent;
            this.btnLeft.FlatAppearance.BorderSize = 0;
            this.btnLeft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLeft.Image = global::RateController.Properties.Resources.ArrowLeft1;
            this.btnLeft.Location = new System.Drawing.Point(71, 153);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(60, 62);
            this.btnLeft.TabIndex = 142;
            this.btnLeft.UseVisualStyleBackColor = false;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.BackColor = System.Drawing.Color.Transparent;
            this.btnSettings.FlatAppearance.BorderSize = 0;
            this.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSettings.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSettings.Image = global::RateController.Properties.Resources.SettingsGear64;
            this.btnSettings.Location = new System.Drawing.Point(4, 153);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(60, 62);
            this.btnSettings.TabIndex = 73;
            this.btnSettings.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnSettings.UseVisualStyleBackColor = false;
            this.btnSettings.Click += new System.EventHandler(this.button3_Click);
            // 
            // btAlarm
            // 
            this.btAlarm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btAlarm.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btAlarm.Image = global::RateController.Properties.Resources.Alarm1;
            this.btAlarm.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btAlarm.Location = new System.Drawing.Point(12, 238);
            this.btAlarm.Name = "btAlarm";
            this.btAlarm.Size = new System.Drawing.Size(168, 120);
            this.btAlarm.TabIndex = 146;
            this.btAlarm.Text = "Rate  Alarm  Pressure Alarm";
            this.btAlarm.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btAlarm.UseVisualStyleBackColor = true;
            this.btAlarm.Click += new System.EventHandler(this.btAlarm_Click);
            // 
            // FormStart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 552);
            this.Controls.Add(this.lbAogConnected);
            this.Controls.Add(this.lbArduinoConnected);
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.btnLeft);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.panProducts);
            this.Controls.Add(this.panSummary);
            this.Controls.Add(this.panFan);
            this.Controls.Add(this.btAlarm);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormStart";
            this.Text = "Rate Controller";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.Transparent;
            this.Activated += new System.EventHandler(this.FormStart_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormStart_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormRateControl_FormClosed);
            this.Load += new System.EventHandler(this.FormStart_Load);
            this.panProducts.ResumeLayout(false);
            this.panSummary.ResumeLayout(false);
            this.mnuSettings.ResumeLayout(false);
            this.panFan.ResumeLayout(false);
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
        private System.Windows.Forms.Label TankRemain;
        private System.Windows.Forms.Label lblUnits;
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
        private System.Windows.Forms.ContextMenuStrip mnuSettings;
        private System.Windows.Forms.ToolStripMenuItem MnuProducts;
        private System.Windows.Forms.ToolStripMenuItem MnuSections;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button btAlarm;
        private System.Windows.Forms.ToolStripMenuItem MnuOptions;
        private System.Windows.Forms.ToolStripMenuItem MnuComm;
        private System.Windows.Forms.ToolStripMenuItem MnuRelays;
        private System.Windows.Forms.Label lbTarget;
        private System.Windows.Forms.Panel panFan;
        private System.Windows.Forms.Label lbTargetRPM;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbFanRate;
        private System.Windows.Forms.Label lbCurrentRPM;
        private System.Windows.Forms.Label lbFan;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbOn;
        private System.Windows.Forms.Label lbOff;
        private System.Windows.Forms.Label idc5;
        private System.Windows.Forms.Label rt5;
        private System.Windows.Forms.Label prd5;
        private System.Windows.Forms.Timer timerPIDs;
        private System.Windows.Forms.Button btnFan;
        private System.Windows.Forms.ToolStripMenuItem networkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem calibrateToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Label tg3;
        private System.Windows.Forms.Label tg2;
        private System.Windows.Forms.Label tg1;
        private System.Windows.Forms.Label tg0;
        private System.Windows.Forms.ToolStripMenuItem pressuresToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem commDiagnosticsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
    }
}