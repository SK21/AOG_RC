﻿
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
            this.productsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sectionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pressureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.deustchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.englishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nederlandsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pCBConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.firmwareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
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
            this.SetRate.Click += new System.EventHandler(this.SetRate_Click);
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
            this.productsToolStripMenuItem,
            this.sectionsToolStripMenuItem,
            this.commToolStripMenuItem,
            this.pressureToolStripMenuItem,
            this.newToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripMenuItem1,
            this.pCBConfigToolStripMenuItem,
            this.firmwareToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.mnuSettings.Name = "mnuSettings";
            this.mnuSettings.Size = new System.Drawing.Size(201, 488);
            // 
            // productsToolStripMenuItem
            // 
            this.productsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("productsToolStripMenuItem.Image")));
            this.productsToolStripMenuItem.Name = "productsToolStripMenuItem";
            this.productsToolStripMenuItem.Size = new System.Drawing.Size(189, 42);
            this.productsToolStripMenuItem.Text = "Products";
            this.productsToolStripMenuItem.Click += new System.EventHandler(this.productsToolStripMenuItem_Click);
            // 
            // sectionsToolStripMenuItem
            // 
            this.sectionsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("sectionsToolStripMenuItem.Image")));
            this.sectionsToolStripMenuItem.Name = "sectionsToolStripMenuItem";
            this.sectionsToolStripMenuItem.Size = new System.Drawing.Size(189, 42);
            this.sectionsToolStripMenuItem.Text = "Sections";
            this.sectionsToolStripMenuItem.Click += new System.EventHandler(this.sectionsToolStripMenuItem_Click);
            // 
            // commToolStripMenuItem
            // 
            this.commToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("commToolStripMenuItem.Image")));
            this.commToolStripMenuItem.Name = "commToolStripMenuItem";
            this.commToolStripMenuItem.Size = new System.Drawing.Size(189, 42);
            this.commToolStripMenuItem.Text = "Comm";
            this.commToolStripMenuItem.Click += new System.EventHandler(this.commToolStripMenuItem_Click);
            // 
            // pressureToolStripMenuItem
            // 
            this.pressureToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("pressureToolStripMenuItem.Image")));
            this.pressureToolStripMenuItem.Name = "pressureToolStripMenuItem";
            this.pressureToolStripMenuItem.Size = new System.Drawing.Size(200, 42);
            this.pressureToolStripMenuItem.Text = "Pressure";
            this.pressureToolStripMenuItem.Visible = false;
            this.pressureToolStripMenuItem.Click += new System.EventHandler(this.pressureToolStripMenuItem_Click);
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newToolStripMenuItem.Image")));
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(189, 42);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Image = global::RateController.Properties.Resources.OpenFile;
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(200, 42);
            this.loadToolStripMenuItem.Text = "Open";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = global::RateController.Properties.Resources.close;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(189, 42);
            this.saveToolStripMenuItem.Text = "Save As";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deustchToolStripMenuItem,
            this.englishToolStripMenuItem,
            this.nederlandsToolStripMenuItem});
            this.toolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem1.Image")));
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(189, 42);
            this.toolStripMenuItem1.Text = "Language";
            // 
            // deustchToolStripMenuItem
            // 
            this.deustchToolStripMenuItem.Name = "deustchToolStripMenuItem";
            this.deustchToolStripMenuItem.Size = new System.Drawing.Size(175, 28);
            this.deustchToolStripMenuItem.Text = "Deustch";
            this.deustchToolStripMenuItem.Click += new System.EventHandler(this.deustchToolStripMenuItem_Click);
            // 
            // englishToolStripMenuItem
            // 
            this.englishToolStripMenuItem.Name = "englishToolStripMenuItem";
            this.englishToolStripMenuItem.Size = new System.Drawing.Size(175, 28);
            this.englishToolStripMenuItem.Text = "English";
            this.englishToolStripMenuItem.Click += new System.EventHandler(this.englishToolStripMenuItem_Click);
            // 
            // nederlandsToolStripMenuItem
            // 
            this.nederlandsToolStripMenuItem.Name = "nederlandsToolStripMenuItem";
            this.nederlandsToolStripMenuItem.Size = new System.Drawing.Size(175, 28);
            this.nederlandsToolStripMenuItem.Text = "Nederlands";
            this.nederlandsToolStripMenuItem.Click += new System.EventHandler(this.nederlandsToolStripMenuItem_Click);
            // 
            // pCBConfigToolStripMenuItem
            // 
            this.pCBConfigToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("pCBConfigToolStripMenuItem.Image")));
            this.pCBConfigToolStripMenuItem.Name = "pCBConfigToolStripMenuItem";
            this.pCBConfigToolStripMenuItem.Size = new System.Drawing.Size(189, 42);
            this.pCBConfigToolStripMenuItem.Text = "PCB config";
            this.pCBConfigToolStripMenuItem.Click += new System.EventHandler(this.pCBConfigToolStripMenuItem_Click);
            // 
            // firmwareToolStripMenuItem
            // 
            this.firmwareToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("firmwareToolStripMenuItem.Image")));
            this.firmwareToolStripMenuItem.Name = "firmwareToolStripMenuItem";
            this.firmwareToolStripMenuItem.Size = new System.Drawing.Size(189, 42);
            this.firmwareToolStripMenuItem.Text = "Firmware";
            this.firmwareToolStripMenuItem.Click += new System.EventHandler(this.firmwareToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Image = global::RateController.Properties.Resources.R674d5dd067acbd409ff50db6d0647f5d;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(189, 42);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
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
        private System.Windows.Forms.ToolStripMenuItem productsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sectionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem commToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button btAlarm;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem deustchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem englishToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nederlandsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pressureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem firmwareToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pCBConfigToolStripMenuItem;
    }
}