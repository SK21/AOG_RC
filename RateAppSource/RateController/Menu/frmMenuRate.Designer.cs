namespace RateController.Menu
{
    partial class frmMenuRate
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
            this.lb0 = new System.Windows.Forms.Label();
            this.tbProduct = new System.Windows.Forms.TextBox();
            this.ValveType = new System.Windows.Forms.ComboBox();
            this.lb5 = new System.Windows.Forms.Label();
            this.lbStartQuantity = new System.Windows.Forms.Label();
            this.LabProdDensity = new System.Windows.Forms.Label();
            this.CbUseProdDensity = new System.Windows.Forms.CheckBox();
            this.ProdDensity = new System.Windows.Forms.TextBox();
            this.lbSensorCounts = new System.Windows.Forms.Label();
            this.AreaUnits = new System.Windows.Forms.ComboBox();
            this.tbVolumeUnits = new System.Windows.Forms.TextBox();
            this.TankSize = new System.Windows.Forms.TextBox();
            this.TankRemain = new System.Windows.Forms.TextBox();
            this.FlowCal = new System.Windows.Forms.TextBox();
            this.lb6 = new System.Windows.Forms.Label();
            this.tbAltRate = new System.Windows.Forms.TextBox();
            this.lbBaseRateDes = new System.Windows.Forms.Label();
            this.lb2 = new System.Windows.Forms.Label();
            this.lb1 = new System.Windows.Forms.Label();
            this.lbBaseRate = new System.Windows.Forms.TextBox();
            this.lbAltRate = new System.Windows.Forms.Label();
            this.lbProduct = new System.Windows.Forms.Label();
            this.pnlFan = new System.Windows.Forms.Panel();
            this.btnFan = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.lbFanStarted = new System.Windows.Forms.Label();
            this.lbFanErrorValue = new System.Windows.Forms.Label();
            this.lbFanError = new System.Windows.Forms.Label();
            this.lbFanRPMvalue = new System.Windows.Forms.Label();
            this.lbFanRPM = new System.Windows.Forms.Label();
            this.lbFanPWMvalue = new System.Windows.Forms.Label();
            this.lbFanPWM = new System.Windows.Forms.Label();
            this.lbCountsRPM = new System.Windows.Forms.Label();
            this.tbCountsRPM = new System.Windows.Forms.TextBox();
            this.lbTargetRPM = new System.Windows.Forms.Label();
            this.tbTargetRPM = new System.Windows.Forms.TextBox();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnResetTank = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pnlFan.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // lb0
            // 
            this.lb0.AutoSize = true;
            this.lb0.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb0.Location = new System.Drawing.Point(17, 13);
            this.lb0.Name = "lb0";
            this.lb0.Size = new System.Drawing.Size(129, 23);
            this.lb0.TabIndex = 138;
            this.lb0.Text = "Product Name";
            // 
            // tbProduct
            // 
            this.tbProduct.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbProduct.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbProduct.Location = new System.Drawing.Point(207, 9);
            this.tbProduct.MaxLength = 15;
            this.tbProduct.Name = "tbProduct";
            this.tbProduct.Size = new System.Drawing.Size(176, 30);
            this.tbProduct.TabIndex = 137;
            this.tbProduct.Text = "P1";
            this.tbProduct.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbProduct.TextChanged += new System.EventHandler(this.tbProduct_TextChanged);
            // 
            // ValveType
            // 
            this.ValveType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ValveType.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ValveType.FormattingEnabled = true;
            this.ValveType.ItemHeight = 23;
            this.ValveType.Items.AddRange(new object[] {
            "Standard Valve",
            "Fast Close Valve",
            "Motor",
            "Timed Valve"});
            this.ValveType.Location = new System.Drawing.Point(206, 57);
            this.ValveType.Name = "ValveType";
            this.ValveType.Size = new System.Drawing.Size(176, 31);
            this.ValveType.TabIndex = 139;
            this.ValveType.SelectedIndexChanged += new System.EventHandler(this.ValveType_SelectedIndexChanged);
            // 
            // lb5
            // 
            this.lb5.AutoSize = true;
            this.lb5.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb5.Location = new System.Drawing.Point(17, 60);
            this.lb5.Name = "lb5";
            this.lb5.Size = new System.Drawing.Size(116, 23);
            this.lb5.TabIndex = 140;
            this.lb5.Text = "Control Type";
            // 
            // lbStartQuantity
            // 
            this.lbStartQuantity.AutoSize = true;
            this.lbStartQuantity.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbStartQuantity.Location = new System.Drawing.Point(17, 438);
            this.lbStartQuantity.Name = "lbStartQuantity";
            this.lbStartQuantity.Size = new System.Drawing.Size(97, 23);
            this.lbStartQuantity.TabIndex = 158;
            this.lbStartQuantity.Text = "Tank Start";
            // 
            // LabProdDensity
            // 
            this.LabProdDensity.AutoSize = true;
            this.LabProdDensity.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabProdDensity.Location = new System.Drawing.Point(16, 250);
            this.LabProdDensity.Name = "LabProdDensity";
            this.LabProdDensity.Size = new System.Drawing.Size(71, 23);
            this.LabProdDensity.TabIndex = 157;
            this.LabProdDensity.Text = "Density";
            // 
            // CbUseProdDensity
            // 
            this.CbUseProdDensity.AutoSize = true;
            this.CbUseProdDensity.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.CbUseProdDensity.Location = new System.Drawing.Point(183, 254);
            this.CbUseProdDensity.Margin = new System.Windows.Forms.Padding(2);
            this.CbUseProdDensity.Name = "CbUseProdDensity";
            this.CbUseProdDensity.Size = new System.Drawing.Size(15, 14);
            this.CbUseProdDensity.TabIndex = 156;
            this.CbUseProdDensity.UseVisualStyleBackColor = true;
            this.CbUseProdDensity.CheckedChanged += new System.EventHandler(this.CbUseProdDensity_CheckedChanged);
            this.CbUseProdDensity.Click += new System.EventHandler(this.CbUseProdDensity_Click);
            // 
            // ProdDensity
            // 
            this.ProdDensity.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProdDensity.Location = new System.Drawing.Point(243, 246);
            this.ProdDensity.MaxLength = 8;
            this.ProdDensity.Name = "ProdDensity";
            this.ProdDensity.Size = new System.Drawing.Size(100, 30);
            this.ProdDensity.TabIndex = 155;
            this.ProdDensity.Text = "143";
            this.ProdDensity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ProdDensity.TextChanged += new System.EventHandler(this.tbProduct_TextChanged);
            this.ProdDensity.Enter += new System.EventHandler(this.ProdDensity_Enter);
            // 
            // lbSensorCounts
            // 
            this.lbSensorCounts.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSensorCounts.Location = new System.Drawing.Point(16, 203);
            this.lbSensorCounts.Name = "lbSensorCounts";
            this.lbSensorCounts.Size = new System.Drawing.Size(221, 23);
            this.lbSensorCounts.TabIndex = 150;
            this.lbSensorCounts.Text = "Sensor Counts / Unit";
            // 
            // AreaUnits
            // 
            this.AreaUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AreaUnits.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AreaUnits.FormattingEnabled = true;
            this.AreaUnits.Items.AddRange(new object[] {
            "Acre",
            "Hectare",
            "Minute",
            "Hour"});
            this.AreaUnits.Location = new System.Drawing.Point(206, 151);
            this.AreaUnits.Name = "AreaUnits";
            this.AreaUnits.Size = new System.Drawing.Size(176, 31);
            this.AreaUnits.TabIndex = 141;
            this.AreaUnits.SelectedIndexChanged += new System.EventHandler(this.tbProduct_TextChanged);
            // 
            // tbVolumeUnits
            // 
            this.tbVolumeUnits.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbVolumeUnits.Location = new System.Drawing.Point(206, 104);
            this.tbVolumeUnits.MaxLength = 15;
            this.tbVolumeUnits.Name = "tbVolumeUnits";
            this.tbVolumeUnits.Size = new System.Drawing.Size(176, 30);
            this.tbVolumeUnits.TabIndex = 154;
            this.tbVolumeUnits.Text = "Gallons";
            this.tbVolumeUnits.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbVolumeUnits.TextChanged += new System.EventHandler(this.tbProduct_TextChanged);
            // 
            // TankSize
            // 
            this.TankSize.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TankSize.Location = new System.Drawing.Point(243, 387);
            this.TankSize.MaxLength = 8;
            this.TankSize.Name = "TankSize";
            this.TankSize.Size = new System.Drawing.Size(100, 30);
            this.TankSize.TabIndex = 144;
            this.TankSize.Text = "0";
            this.TankSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TankSize.TextChanged += new System.EventHandler(this.tbProduct_TextChanged);
            this.TankSize.Enter += new System.EventHandler(this.TankSize_Enter);
            this.TankSize.Validating += new System.ComponentModel.CancelEventHandler(this.TankSize_Validating);
            // 
            // TankRemain
            // 
            this.TankRemain.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TankRemain.Location = new System.Drawing.Point(243, 434);
            this.TankRemain.MaxLength = 8;
            this.TankRemain.Name = "TankRemain";
            this.TankRemain.Size = new System.Drawing.Size(100, 30);
            this.TankRemain.TabIndex = 145;
            this.TankRemain.Text = "0";
            this.TankRemain.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TankRemain.TextChanged += new System.EventHandler(this.tbProduct_TextChanged);
            this.TankRemain.Enter += new System.EventHandler(this.TankRemain_Enter);
            this.TankRemain.Validating += new System.ComponentModel.CancelEventHandler(this.TankRemain_Validating);
            // 
            // FlowCal
            // 
            this.FlowCal.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FlowCal.Location = new System.Drawing.Point(243, 199);
            this.FlowCal.MaxLength = 8;
            this.FlowCal.Name = "FlowCal";
            this.FlowCal.Size = new System.Drawing.Size(100, 30);
            this.FlowCal.TabIndex = 142;
            this.FlowCal.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.FlowCal.TextChanged += new System.EventHandler(this.tbProduct_TextChanged);
            this.FlowCal.Enter += new System.EventHandler(this.FlowCal_Enter);
            this.FlowCal.Validating += new System.ComponentModel.CancelEventHandler(this.FlowCal_Validating);
            // 
            // lb6
            // 
            this.lb6.AutoSize = true;
            this.lb6.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb6.Location = new System.Drawing.Point(16, 391);
            this.lb6.Name = "lb6";
            this.lb6.Size = new System.Drawing.Size(90, 23);
            this.lb6.TabIndex = 151;
            this.lb6.Text = "Tank Size";
            // 
            // tbAltRate
            // 
            this.tbAltRate.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbAltRate.Location = new System.Drawing.Point(243, 340);
            this.tbAltRate.MaxLength = 8;
            this.tbAltRate.Name = "tbAltRate";
            this.tbAltRate.Size = new System.Drawing.Size(100, 30);
            this.tbAltRate.TabIndex = 153;
            this.tbAltRate.Text = "75";
            this.tbAltRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbAltRate.TextChanged += new System.EventHandler(this.tbProduct_TextChanged);
            this.tbAltRate.Enter += new System.EventHandler(this.tbAltRate_Enter);
            this.tbAltRate.Validating += new System.ComponentModel.CancelEventHandler(this.tbAltRate_Validating);
            // 
            // lbBaseRateDes
            // 
            this.lbBaseRateDes.AutoSize = true;
            this.lbBaseRateDes.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbBaseRateDes.Location = new System.Drawing.Point(16, 297);
            this.lbBaseRateDes.Name = "lbBaseRateDes";
            this.lbBaseRateDes.Size = new System.Drawing.Size(93, 23);
            this.lbBaseRateDes.TabIndex = 147;
            this.lbBaseRateDes.Text = "Base Rate";
            // 
            // lb2
            // 
            this.lb2.AutoSize = true;
            this.lb2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb2.Location = new System.Drawing.Point(16, 155);
            this.lb2.Name = "lb2";
            this.lb2.Size = new System.Drawing.Size(135, 23);
            this.lb2.TabIndex = 148;
            this.lb2.Text = "Coverage Units";
            // 
            // lb1
            // 
            this.lb1.AutoSize = true;
            this.lb1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb1.Location = new System.Drawing.Point(16, 108);
            this.lb1.Name = "lb1";
            this.lb1.Size = new System.Drawing.Size(127, 23);
            this.lb1.TabIndex = 149;
            this.lb1.Text = "Quantity Units";
            // 
            // lbBaseRate
            // 
            this.lbBaseRate.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbBaseRate.Location = new System.Drawing.Point(243, 293);
            this.lbBaseRate.MaxLength = 8;
            this.lbBaseRate.Name = "lbBaseRate";
            this.lbBaseRate.Size = new System.Drawing.Size(100, 30);
            this.lbBaseRate.TabIndex = 143;
            this.lbBaseRate.Text = "0";
            this.lbBaseRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.lbBaseRate.TextChanged += new System.EventHandler(this.tbProduct_TextChanged);
            this.lbBaseRate.Enter += new System.EventHandler(this.lbBaseRate_Enter);
            this.lbBaseRate.Validating += new System.ComponentModel.CancelEventHandler(this.lbBaseRate_Validating);
            // 
            // lbAltRate
            // 
            this.lbAltRate.AutoSize = true;
            this.lbAltRate.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbAltRate.Location = new System.Drawing.Point(16, 344);
            this.lbAltRate.Name = "lbAltRate";
            this.lbAltRate.Size = new System.Drawing.Size(120, 23);
            this.lbAltRate.TabIndex = 152;
            this.lbAltRate.Text = "Alt. Rate (%)";
            // 
            // lbProduct
            // 
            this.lbProduct.BackColor = System.Drawing.SystemColors.Control;
            this.lbProduct.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbProduct.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbProduct.Location = new System.Drawing.Point(71, 9);
            this.lbProduct.Name = "lbProduct";
            this.lbProduct.Size = new System.Drawing.Size(396, 36);
            this.lbProduct.TabIndex = 162;
            this.lbProduct.Text = "Product";
            this.lbProduct.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pnlFan
            // 
            this.pnlFan.Controls.Add(this.btnFan);
            this.pnlFan.Controls.Add(this.label7);
            this.pnlFan.Controls.Add(this.lbFanStarted);
            this.pnlFan.Controls.Add(this.lbFanErrorValue);
            this.pnlFan.Controls.Add(this.lbFanError);
            this.pnlFan.Controls.Add(this.lbFanRPMvalue);
            this.pnlFan.Controls.Add(this.lbFanRPM);
            this.pnlFan.Controls.Add(this.lbFanPWMvalue);
            this.pnlFan.Controls.Add(this.lbFanPWM);
            this.pnlFan.Controls.Add(this.lbCountsRPM);
            this.pnlFan.Controls.Add(this.tbCountsRPM);
            this.pnlFan.Controls.Add(this.lbTargetRPM);
            this.pnlFan.Controls.Add(this.tbTargetRPM);
            this.pnlFan.Location = new System.Drawing.Point(730, 61);
            this.pnlFan.Name = "pnlFan";
            this.pnlFan.Size = new System.Drawing.Size(396, 335);
            this.pnlFan.TabIndex = 163;
            // 
            // btnFan
            // 
            this.btnFan.BackColor = System.Drawing.Color.Transparent;
            this.btnFan.FlatAppearance.BorderSize = 0;
            this.btnFan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFan.Image = global::RateController.Properties.Resources.FanOff;
            this.btnFan.Location = new System.Drawing.Point(172, 276);
            this.btnFan.Name = "btnFan";
            this.btnFan.Size = new System.Drawing.Size(50, 50);
            this.btnFan.TabIndex = 202;
            this.btnFan.UseVisualStyleBackColor = false;
            this.btnFan.Click += new System.EventHandler(this.btnFan_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(52, 222);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 23);
            this.label7.TabIndex = 201;
            this.label7.Text = "Started";
            // 
            // lbFanStarted
            // 
            this.lbFanStarted.BackColor = System.Drawing.SystemColors.Control;
            this.lbFanStarted.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbFanStarted.Image = global::RateController.Properties.Resources.Off;
            this.lbFanStarted.Location = new System.Drawing.Point(282, 215);
            this.lbFanStarted.Name = "lbFanStarted";
            this.lbFanStarted.Size = new System.Drawing.Size(41, 37);
            this.lbFanStarted.TabIndex = 200;
            this.lbFanStarted.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbFanErrorValue
            // 
            this.lbFanErrorValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbFanErrorValue.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbFanErrorValue.Location = new System.Drawing.Point(251, 133);
            this.lbFanErrorValue.Name = "lbFanErrorValue";
            this.lbFanErrorValue.Size = new System.Drawing.Size(102, 30);
            this.lbFanErrorValue.TabIndex = 196;
            this.lbFanErrorValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbFanError
            // 
            this.lbFanError.AutoSize = true;
            this.lbFanError.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbFanError.Location = new System.Drawing.Point(48, 137);
            this.lbFanError.Name = "lbFanError";
            this.lbFanError.Size = new System.Drawing.Size(77, 23);
            this.lbFanError.TabIndex = 197;
            this.lbFanError.Text = "Error %";
            // 
            // lbFanRPMvalue
            // 
            this.lbFanRPMvalue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbFanRPMvalue.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbFanRPMvalue.Location = new System.Drawing.Point(251, 92);
            this.lbFanRPMvalue.Name = "lbFanRPMvalue";
            this.lbFanRPMvalue.Size = new System.Drawing.Size(102, 30);
            this.lbFanRPMvalue.TabIndex = 194;
            this.lbFanRPMvalue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbFanRPM
            // 
            this.lbFanRPM.AutoSize = true;
            this.lbFanRPM.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbFanRPM.Location = new System.Drawing.Point(48, 96);
            this.lbFanRPM.Name = "lbFanRPM";
            this.lbFanRPM.Size = new System.Drawing.Size(116, 23);
            this.lbFanRPM.TabIndex = 195;
            this.lbFanRPM.Text = "Current RPM";
            // 
            // lbFanPWMvalue
            // 
            this.lbFanPWMvalue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbFanPWMvalue.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbFanPWMvalue.Location = new System.Drawing.Point(251, 174);
            this.lbFanPWMvalue.Name = "lbFanPWMvalue";
            this.lbFanPWMvalue.Size = new System.Drawing.Size(102, 30);
            this.lbFanPWMvalue.TabIndex = 190;
            this.lbFanPWMvalue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbFanPWM
            // 
            this.lbFanPWM.AutoSize = true;
            this.lbFanPWM.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbFanPWM.Location = new System.Drawing.Point(48, 178);
            this.lbFanPWM.Name = "lbFanPWM";
            this.lbFanPWM.Size = new System.Drawing.Size(52, 23);
            this.lbFanPWM.TabIndex = 191;
            this.lbFanPWM.Text = "PWM";
            // 
            // lbCountsRPM
            // 
            this.lbCountsRPM.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCountsRPM.Location = new System.Drawing.Point(48, 14);
            this.lbCountsRPM.Name = "lbCountsRPM";
            this.lbCountsRPM.Size = new System.Drawing.Size(197, 23);
            this.lbCountsRPM.TabIndex = 117;
            this.lbCountsRPM.Text = "Sensor Counts / RPM";
            // 
            // tbCountsRPM
            // 
            this.tbCountsRPM.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbCountsRPM.Location = new System.Drawing.Point(251, 10);
            this.tbCountsRPM.MaxLength = 8;
            this.tbCountsRPM.Name = "tbCountsRPM";
            this.tbCountsRPM.Size = new System.Drawing.Size(102, 30);
            this.tbCountsRPM.TabIndex = 116;
            this.tbCountsRPM.TabStop = false;
            this.tbCountsRPM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbCountsRPM.TextChanged += new System.EventHandler(this.tbProduct_TextChanged);
            this.tbCountsRPM.Enter += new System.EventHandler(this.tbCountsRPM_Enter);
            this.tbCountsRPM.Validating += new System.ComponentModel.CancelEventHandler(this.tbCountsRPM_Validating);
            // 
            // lbTargetRPM
            // 
            this.lbTargetRPM.AutoSize = true;
            this.lbTargetRPM.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTargetRPM.Location = new System.Drawing.Point(48, 55);
            this.lbTargetRPM.Name = "lbTargetRPM";
            this.lbTargetRPM.Size = new System.Drawing.Size(108, 23);
            this.lbTargetRPM.TabIndex = 114;
            this.lbTargetRPM.Text = "Target RPM";
            // 
            // tbTargetRPM
            // 
            this.tbTargetRPM.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbTargetRPM.Location = new System.Drawing.Point(251, 51);
            this.tbTargetRPM.MaxLength = 8;
            this.tbTargetRPM.Name = "tbTargetRPM";
            this.tbTargetRPM.Size = new System.Drawing.Size(102, 30);
            this.tbTargetRPM.TabIndex = 113;
            this.tbTargetRPM.TabStop = false;
            this.tbTargetRPM.Text = "0";
            this.tbTargetRPM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbTargetRPM.TextChanged += new System.EventHandler(this.tbProduct_TextChanged);
            this.tbTargetRPM.Enter += new System.EventHandler(this.tbTargetRPM_Enter);
            this.tbTargetRPM.Validating += new System.ComponentModel.CancelEventHandler(this.tbTargetRPM_Validating);
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.tbProduct);
            this.pnlMain.Controls.Add(this.lb0);
            this.pnlMain.Controls.Add(this.lb5);
            this.pnlMain.Controls.Add(this.ValveType);
            this.pnlMain.Controls.Add(this.lbAltRate);
            this.pnlMain.Controls.Add(this.lbBaseRate);
            this.pnlMain.Controls.Add(this.lbStartQuantity);
            this.pnlMain.Controls.Add(this.lb1);
            this.pnlMain.Controls.Add(this.LabProdDensity);
            this.pnlMain.Controls.Add(this.lb2);
            this.pnlMain.Controls.Add(this.CbUseProdDensity);
            this.pnlMain.Controls.Add(this.lbBaseRateDes);
            this.pnlMain.Controls.Add(this.ProdDensity);
            this.pnlMain.Controls.Add(this.tbAltRate);
            this.pnlMain.Controls.Add(this.lbSensorCounts);
            this.pnlMain.Controls.Add(this.lb6);
            this.pnlMain.Controls.Add(this.AreaUnits);
            this.pnlMain.Controls.Add(this.FlowCal);
            this.pnlMain.Controls.Add(this.tbVolumeUnits);
            this.pnlMain.Controls.Add(this.TankRemain);
            this.pnlMain.Controls.Add(this.TankSize);
            this.pnlMain.Location = new System.Drawing.Point(71, 48);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(396, 480);
            this.pnlMain.TabIndex = 164;
            // 
            // btnRight
            // 
            this.btnRight.BackColor = System.Drawing.Color.Transparent;
            this.btnRight.FlatAppearance.BorderSize = 0;
            this.btnRight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRight.Image = global::RateController.Properties.Resources.ArrowRight1;
            this.btnRight.Location = new System.Drawing.Point(300, 546);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(72, 72);
            this.btnRight.TabIndex = 160;
            this.btnRight.UseVisualStyleBackColor = false;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnLeft
            // 
            this.btnLeft.BackColor = System.Drawing.Color.Transparent;
            this.btnLeft.FlatAppearance.BorderSize = 0;
            this.btnLeft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLeft.Image = global::RateController.Properties.Resources.ArrowLeft1;
            this.btnLeft.Location = new System.Drawing.Point(222, 546);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(72, 72);
            this.btnLeft.TabIndex = 159;
            this.btnLeft.UseVisualStyleBackColor = false;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnResetTank
            // 
            this.btnResetTank.BackColor = System.Drawing.Color.Transparent;
            this.btnResetTank.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnResetTank.FlatAppearance.BorderSize = 0;
            this.btnResetTank.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResetTank.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnResetTank.Image = global::RateController.Properties.Resources.Update;
            this.btnResetTank.Location = new System.Drawing.Point(144, 546);
            this.btnResetTank.Name = "btnResetTank";
            this.btnResetTank.Size = new System.Drawing.Size(72, 72);
            this.btnResetTank.TabIndex = 146;
            this.btnResetTank.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnResetTank.UseVisualStyleBackColor = false;
            this.btnResetTank.Click += new System.EventHandler(this.btnResetTank_Click);
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
            this.btnCancel.TabIndex = 136;
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
            this.btnOK.TabIndex = 135;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // frmMenuRate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1173, 630);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlFan);
            this.Controls.Add(this.lbProduct);
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.btnLeft);
            this.Controls.Add(this.btnResetTank);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMenuRate";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuRate";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMenuRate_FormClosed);
            this.Load += new System.EventHandler(this.frmMenuRate_Load);
            this.pnlFan.ResumeLayout(false);
            this.pnlFan.PerformLayout();
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lb0;
        private System.Windows.Forms.TextBox tbProduct;
        private System.Windows.Forms.ComboBox ValveType;
        private System.Windows.Forms.Label lb5;
        private System.Windows.Forms.Label lbStartQuantity;
        private System.Windows.Forms.Label LabProdDensity;
        private System.Windows.Forms.CheckBox CbUseProdDensity;
        private System.Windows.Forms.TextBox ProdDensity;
        private System.Windows.Forms.Label lbSensorCounts;
        private System.Windows.Forms.ComboBox AreaUnits;
        private System.Windows.Forms.TextBox tbVolumeUnits;
        private System.Windows.Forms.TextBox TankSize;
        private System.Windows.Forms.TextBox TankRemain;
        private System.Windows.Forms.TextBox FlowCal;
        private System.Windows.Forms.Label lb6;
        private System.Windows.Forms.TextBox tbAltRate;
        private System.Windows.Forms.Label lbBaseRateDes;
        private System.Windows.Forms.Label lb2;
        private System.Windows.Forms.Label lb1;
        private System.Windows.Forms.Button btnResetTank;
        private System.Windows.Forms.TextBox lbBaseRate;
        private System.Windows.Forms.Label lbAltRate;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Label lbProduct;
        private System.Windows.Forms.Panel pnlFan;
        private System.Windows.Forms.Button btnFan;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lbFanStarted;
        private System.Windows.Forms.Label lbFanErrorValue;
        private System.Windows.Forms.Label lbFanError;
        private System.Windows.Forms.Label lbFanRPMvalue;
        private System.Windows.Forms.Label lbFanRPM;
        private System.Windows.Forms.Label lbFanPWMvalue;
        private System.Windows.Forms.Label lbFanPWM;
        private System.Windows.Forms.Label lbCountsRPM;
        private System.Windows.Forms.TextBox tbCountsRPM;
        private System.Windows.Forms.Label lbTargetRPM;
        private System.Windows.Forms.TextBox tbTargetRPM;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Timer timer1;
    }
}