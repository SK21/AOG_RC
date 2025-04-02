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
            this.lbModule = new System.Windows.Forms.Label();
            this.ModuleIndicator = new System.Windows.Forms.Label();
            this.cbModules = new System.Windows.Forms.ComboBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.tbMinVol = new System.Windows.Forms.TextBox();
            this.ckPressure = new System.Windows.Forms.CheckBox();
            this.lbMin = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tbMinPres = new System.Windows.Forms.TextBox();
            this.lbVoltage = new System.Windows.Forms.Label();
            this.lbPressure = new System.Windows.Forms.Label();
            this.tbMaxPres = new System.Windows.Forms.TextBox();
            this.lbMax = new System.Windows.Forms.Label();
            this.tbMaxVol = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lbPressureReading = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbRaw = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbModule
            // 
            this.lbModule.AutoSize = true;
            this.lbModule.Location = new System.Drawing.Point(173, 103);
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
            this.ModuleIndicator.Location = new System.Drawing.Point(349, 97);
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
            this.cbModules.Location = new System.Drawing.Point(262, 100);
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
            // tbMinVol
            // 
            this.tbMinVol.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbMinVol.Location = new System.Drawing.Point(243, 206);
            this.tbMinVol.MaxLength = 8;
            this.tbMinVol.Name = "tbMinVol";
            this.tbMinVol.Size = new System.Drawing.Size(80, 30);
            this.tbMinVol.TabIndex = 190;
            this.tbMinVol.Text = "0";
            this.tbMinVol.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbMinVol.TextChanged += new System.EventHandler(this.tbMinVol_TextChanged);
            this.tbMinVol.Enter += new System.EventHandler(this.tbMinVol_Enter);
            // 
            // ckPressure
            // 
            this.ckPressure.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckPressure.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckPressure.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckPressure.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckPressure.Location = new System.Drawing.Point(269, 594);
            this.ckPressure.Name = "ckPressure";
            this.ckPressure.Size = new System.Drawing.Size(82, 72);
            this.ckPressure.TabIndex = 218;
            this.ckPressure.Text = "Show";
            this.ckPressure.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckPressure.UseVisualStyleBackColor = true;
            this.ckPressure.CheckedChanged += new System.EventHandler(this.tbMinVol_TextChanged);
            // 
            // lbMin
            // 
            this.lbMin.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMin.Location = new System.Drawing.Point(113, 209);
            this.lbMin.Name = "lbMin";
            this.lbMin.Size = new System.Drawing.Size(124, 23);
            this.lbMin.TabIndex = 216;
            this.lbMin.Text = "Minimum:";
            this.lbMin.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // tbMinPres
            // 
            this.tbMinPres.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbMinPres.Location = new System.Drawing.Point(366, 206);
            this.tbMinPres.MaxLength = 8;
            this.tbMinPres.Name = "tbMinPres";
            this.tbMinPres.Size = new System.Drawing.Size(80, 30);
            this.tbMinPres.TabIndex = 222;
            this.tbMinPres.Text = "0";
            this.tbMinPres.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbMinPres.TextChanged += new System.EventHandler(this.tbMinVol_TextChanged);
            this.tbMinPres.Enter += new System.EventHandler(this.tbMinPres_Enter);
            // 
            // lbVoltage
            // 
            this.lbVoltage.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbVoltage.Location = new System.Drawing.Point(228, 159);
            this.lbVoltage.Name = "lbVoltage";
            this.lbVoltage.Size = new System.Drawing.Size(95, 37);
            this.lbVoltage.TabIndex = 223;
            this.lbVoltage.Text = "Reading";
            this.lbVoltage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbPressure
            // 
            this.lbPressure.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPressure.Location = new System.Drawing.Point(351, 159);
            this.lbPressure.Name = "lbPressure";
            this.lbPressure.Size = new System.Drawing.Size(95, 37);
            this.lbPressure.TabIndex = 224;
            this.lbPressure.Text = "Pressure";
            this.lbPressure.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbMaxPres
            // 
            this.tbMaxPres.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbMaxPres.Location = new System.Drawing.Point(366, 253);
            this.tbMaxPres.MaxLength = 8;
            this.tbMaxPres.Name = "tbMaxPres";
            this.tbMaxPres.Size = new System.Drawing.Size(80, 30);
            this.tbMaxPres.TabIndex = 227;
            this.tbMaxPres.Text = "0";
            this.tbMaxPres.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbMaxPres.TextChanged += new System.EventHandler(this.tbMinVol_TextChanged);
            this.tbMaxPres.Enter += new System.EventHandler(this.tbMaxPres_Enter);
            // 
            // lbMax
            // 
            this.lbMax.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMax.Location = new System.Drawing.Point(113, 256);
            this.lbMax.Name = "lbMax";
            this.lbMax.Size = new System.Drawing.Size(124, 23);
            this.lbMax.TabIndex = 226;
            this.lbMax.Text = "Maximum:";
            this.lbMax.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbMaxVol
            // 
            this.tbMaxVol.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbMaxVol.Location = new System.Drawing.Point(243, 253);
            this.tbMaxVol.MaxLength = 8;
            this.tbMaxVol.Name = "tbMaxVol";
            this.tbMaxVol.Size = new System.Drawing.Size(80, 30);
            this.tbMaxVol.TabIndex = 225;
            this.tbMaxVol.Text = "0";
            this.tbMaxVol.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbMaxVol.TextChanged += new System.EventHandler(this.tbMinVol_TextChanged);
            this.tbMaxVol.Enter += new System.EventHandler(this.tbMaxVol_Enter);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(113, 328);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(178, 23);
            this.label4.TabIndex = 229;
            this.label4.Text = "Current Reading:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbPressureReading
            // 
            this.lbPressureReading.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbPressureReading.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPressureReading.Location = new System.Drawing.Point(297, 371);
            this.lbPressureReading.Name = "lbPressureReading";
            this.lbPressureReading.Size = new System.Drawing.Size(80, 30);
            this.lbPressureReading.TabIndex = 232;
            this.lbPressureReading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(113, 375);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(178, 23);
            this.label2.TabIndex = 231;
            this.label2.Text = "Current Pressure:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbRaw
            // 
            this.lbRaw.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbRaw.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRaw.Location = new System.Drawing.Point(297, 324);
            this.lbRaw.Name = "lbRaw";
            this.lbRaw.Size = new System.Drawing.Size(80, 30);
            this.lbRaw.TabIndex = 233;
            this.lbRaw.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frmMenuPressure
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 678);
            this.Controls.Add(this.lbRaw);
            this.Controls.Add(this.lbPressureReading);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbMaxPres);
            this.Controls.Add(this.lbMax);
            this.Controls.Add(this.tbMaxVol);
            this.Controls.Add(this.lbPressure);
            this.Controls.Add(this.lbVoltage);
            this.Controls.Add(this.tbMinPres);
            this.Controls.Add(this.lbMin);
            this.Controls.Add(this.ckPressure);
            this.Controls.Add(this.tbMinVol);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lbModule;
        private System.Windows.Forms.Label ModuleIndicator;
        private System.Windows.Forms.ComboBox cbModules;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox tbMinVol;
        private System.Windows.Forms.CheckBox ckPressure;
        private System.Windows.Forms.Label lbMin;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox tbMinPres;
        private System.Windows.Forms.Label lbVoltage;
        private System.Windows.Forms.Label lbPressure;
        private System.Windows.Forms.TextBox tbMaxPres;
        private System.Windows.Forms.Label lbMax;
        private System.Windows.Forms.TextBox tbMaxVol;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbPressureReading;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbRaw;
    }
}