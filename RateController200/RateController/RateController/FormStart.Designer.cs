
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
            this.bntOK = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.timerMain = new System.Windows.Forms.Timer(this.components);
            this.lbArduinoConnected = new System.Windows.Forms.Label();
            this.lbAogConnected = new System.Windows.Forms.Label();
            this.panProducts = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lbProduct = new System.Windows.Forms.Label();
            this.SetRate = new System.Windows.Forms.Label();
            this.lbRate = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.TankRemain = new System.Windows.Forms.Label();
            this.lblUnits = new System.Windows.Forms.Label();
            this.VolApplied = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.AreaDone = new System.Windows.Forms.Label();
            this.lbRateAmount = new System.Windows.Forms.Label();
            this.lbCoverage = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
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
            this.btnLeft = new System.Windows.Forms.Button();
            this.timerNano = new System.Windows.Forms.Timer(this.components);
            this.panProducts.SuspendLayout();
            this.panSummary.SuspendLayout();
            this.SuspendLayout();
            // 
            // bntOK
            // 
            this.bntOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bntOK.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.bntOK.Image = global::RateController.Properties.Resources.OK64;
            this.bntOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.bntOK.Location = new System.Drawing.Point(141, 214);
            this.bntOK.Name = "bntOK";
            this.bntOK.Size = new System.Drawing.Size(130, 62);
            this.bntOK.TabIndex = 74;
            this.bntOK.Text = "Close";
            this.bntOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.bntOK.UseVisualStyleBackColor = true;
            this.bntOK.Click += new System.EventHandler(this.bntOK_Click);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Image = ((System.Drawing.Image)(resources.GetObject("button3.Image")));
            this.button3.Location = new System.Drawing.Point(4, 214);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(130, 62);
            this.button3.TabIndex = 73;
            this.button3.Text = "Settings";
            this.button3.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // btnRight
            // 
            this.btnRight.Image = global::RateController.Properties.Resources.ArrowRight1;
            this.btnRight.Location = new System.Drawing.Point(71, 150);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(63, 62);
            this.btnRight.TabIndex = 143;
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
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
            this.lbArduinoConnected.Location = new System.Drawing.Point(141, 153);
            this.lbArduinoConnected.Name = "lbArduinoConnected";
            this.lbArduinoConnected.Size = new System.Drawing.Size(130, 27);
            this.lbArduinoConnected.TabIndex = 144;
            this.lbArduinoConnected.Text = "Controller";
            this.lbArduinoConnected.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbAogConnected
            // 
            this.lbAogConnected.BackColor = System.Drawing.Color.Red;
            this.lbAogConnected.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbAogConnected.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbAogConnected.Location = new System.Drawing.Point(141, 185);
            this.lbAogConnected.Name = "lbAogConnected";
            this.lbAogConnected.Size = new System.Drawing.Size(130, 27);
            this.lbAogConnected.TabIndex = 145;
            this.lbAogConnected.Text = "AOG";
            this.lbAogConnected.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panProducts
            // 
            this.panProducts.Controls.Add(this.groupBox3);
            this.panProducts.Controls.Add(this.lbProduct);
            this.panProducts.Controls.Add(this.SetRate);
            this.panProducts.Controls.Add(this.lbRate);
            this.panProducts.Controls.Add(this.label1);
            this.panProducts.Controls.Add(this.TankRemain);
            this.panProducts.Controls.Add(this.lblUnits);
            this.panProducts.Controls.Add(this.VolApplied);
            this.panProducts.Controls.Add(this.label2);
            this.panProducts.Controls.Add(this.AreaDone);
            this.panProducts.Controls.Add(this.lbRateAmount);
            this.panProducts.Controls.Add(this.lbCoverage);
            this.panProducts.Controls.Add(this.label34);
            this.panProducts.Location = new System.Drawing.Point(0, 0);
            this.panProducts.Name = "panProducts";
            this.panProducts.Size = new System.Drawing.Size(270, 150);
            this.panProducts.TabIndex = 50;
            this.panProducts.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
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
            this.SetRate.Location = new System.Drawing.Point(173, 48);
            this.SetRate.Name = "SetRate";
            this.SetRate.Size = new System.Drawing.Size(102, 23);
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
            this.lbRate.Size = new System.Drawing.Size(159, 23);
            this.lbRate.TabIndex = 155;
            this.lbRate.Text = "Current Rate";
            this.lbRate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbRate.Click += new System.EventHandler(this.lbRate_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(159, 23);
            this.label1.TabIndex = 154;
            this.label1.Text = "Target Rate";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TankRemain
            // 
            this.TankRemain.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TankRemain.Location = new System.Drawing.Point(173, 98);
            this.TankRemain.Name = "TankRemain";
            this.TankRemain.Size = new System.Drawing.Size(102, 23);
            this.TankRemain.TabIndex = 153;
            this.TankRemain.Text = "5000";
            this.TankRemain.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            this.VolApplied.Location = new System.Drawing.Point(173, 123);
            this.VolApplied.Name = "VolApplied";
            this.VolApplied.Size = new System.Drawing.Size(102, 23);
            this.VolApplied.TabIndex = 148;
            this.VolApplied.Text = "0";
            this.VolApplied.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(0, 123);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(159, 23);
            this.label2.TabIndex = 151;
            this.label2.Text = "Quantity Applied";
            // 
            // AreaDone
            // 
            this.AreaDone.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AreaDone.Location = new System.Drawing.Point(173, 73);
            this.AreaDone.Name = "AreaDone";
            this.AreaDone.Size = new System.Drawing.Size(102, 23);
            this.AreaDone.TabIndex = 147;
            this.AreaDone.Text = "0";
            this.AreaDone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbRateAmount
            // 
            this.lbRateAmount.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRateAmount.Location = new System.Drawing.Point(173, 23);
            this.lbRateAmount.Name = "lbRateAmount";
            this.lbRateAmount.Size = new System.Drawing.Size(102, 23);
            this.lbRateAmount.TabIndex = 146;
            this.lbRateAmount.Text = "1,800.50";
            this.lbRateAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbCoverage
            // 
            this.lbCoverage.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCoverage.Location = new System.Drawing.Point(0, 73);
            this.lbCoverage.Name = "lbCoverage";
            this.lbCoverage.Size = new System.Drawing.Size(159, 23);
            this.lbCoverage.TabIndex = 150;
            this.lbCoverage.Text = "Coverage";
            // 
            // label34
            // 
            this.label34.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label34.Location = new System.Drawing.Point(0, 98);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(159, 23);
            this.label34.TabIndex = 149;
            this.label34.Text = "Tank Remaining";
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
            this.panSummary.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel2_MouseDown);
            // 
            // idc0
            // 
            this.idc0.BackColor = System.Drawing.Color.Red;
            this.idc0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.idc0.Cursor = System.Windows.Forms.Cursors.Default;
            this.idc0.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.prd4.Size = new System.Drawing.Size(123, 23);
            this.prd4.TabIndex = 121;
            this.prd4.Text = "5";
            // 
            // rt4
            // 
            this.rt4.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rt4.Location = new System.Drawing.Point(132, 124);
            this.rt4.Name = "rt4";
            this.rt4.Size = new System.Drawing.Size(102, 23);
            this.rt4.TabIndex = 120;
            this.rt4.Text = "0";
            this.rt4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // idc4
            // 
            this.idc4.BackColor = System.Drawing.Color.Red;
            this.idc4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.idc4.Cursor = System.Windows.Forms.Cursors.Default;
            this.idc4.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.prd3.Size = new System.Drawing.Size(123, 23);
            this.prd3.TabIndex = 118;
            this.prd3.Text = "4";
            // 
            // rt3
            // 
            this.rt3.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rt3.Location = new System.Drawing.Point(132, 93);
            this.rt3.Name = "rt3";
            this.rt3.Size = new System.Drawing.Size(102, 23);
            this.rt3.TabIndex = 117;
            this.rt3.Text = "0";
            this.rt3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // idc3
            // 
            this.idc3.BackColor = System.Drawing.Color.Red;
            this.idc3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.idc3.Cursor = System.Windows.Forms.Cursors.Default;
            this.idc3.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.prd2.Size = new System.Drawing.Size(123, 23);
            this.prd2.TabIndex = 115;
            this.prd2.Text = "3";
            // 
            // rt2
            // 
            this.rt2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rt2.Location = new System.Drawing.Point(132, 62);
            this.rt2.Name = "rt2";
            this.rt2.Size = new System.Drawing.Size(102, 23);
            this.rt2.TabIndex = 114;
            this.rt2.Text = "0";
            this.rt2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // idc2
            // 
            this.idc2.BackColor = System.Drawing.Color.Red;
            this.idc2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.idc2.Cursor = System.Windows.Forms.Cursors.Default;
            this.idc2.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.prd1.Size = new System.Drawing.Size(123, 23);
            this.prd1.TabIndex = 112;
            this.prd1.Text = "2";
            // 
            // rt1
            // 
            this.rt1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rt1.Location = new System.Drawing.Point(132, 31);
            this.rt1.Name = "rt1";
            this.rt1.Size = new System.Drawing.Size(102, 23);
            this.rt1.TabIndex = 111;
            this.rt1.Text = "0";
            this.rt1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // idc1
            // 
            this.idc1.BackColor = System.Drawing.Color.Red;
            this.idc1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.idc1.Cursor = System.Windows.Forms.Cursors.Default;
            this.idc1.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.prd0.Size = new System.Drawing.Size(123, 23);
            this.prd0.TabIndex = 109;
            this.prd0.Text = "1";
            // 
            // rt0
            // 
            this.rt0.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rt0.Location = new System.Drawing.Point(132, 0);
            this.rt0.Name = "rt0";
            this.rt0.Size = new System.Drawing.Size(102, 23);
            this.rt0.TabIndex = 108;
            this.rt0.Text = "0";
            this.rt0.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnLeft
            // 
            this.btnLeft.Image = global::RateController.Properties.Resources.ArrowLeft1;
            this.btnLeft.Location = new System.Drawing.Point(4, 150);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(60, 62);
            this.btnLeft.TabIndex = 142;
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // timerNano
            // 
            this.timerNano.Enabled = true;
            this.timerNano.Interval = 50;
            this.timerNano.Tick += new System.EventHandler(this.timerNano_Tick);
            // 
            // FormStart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(276, 281);
            this.Controls.Add(this.panSummary);
            this.Controls.Add(this.panProducts);
            this.Controls.Add(this.lbAogConnected);
            this.Controls.Add(this.lbArduinoConnected);
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.btnLeft);
            this.Controls.Add(this.bntOK);
            this.Controls.Add(this.button3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FormStart";
            this.Text = "Rate Controller";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormRateControl_FormClosed);
            this.Load += new System.EventHandler(this.FormStart_Load);
            this.panProducts.ResumeLayout(false);
            this.panSummary.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button bntOK;
        private System.Windows.Forms.Button button3;
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
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label AreaDone;
        private System.Windows.Forms.Label lbRateAmount;
        private System.Windows.Forms.Label lbCoverage;
        private System.Windows.Forms.Label label34;
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
    }
}