namespace RateController.Menu
{
    partial class frmMenuPins
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMenuPins));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.tbPressure = new System.Windows.Forms.TextBox();
            this.lbPressure = new System.Windows.Forms.Label();
            this.ckMomentary = new System.Windows.Forms.CheckBox();
            this.tbWrk = new System.Windows.Forms.TextBox();
            this.lbWorkPin = new System.Windows.Forms.Label();
            this.btnRescan = new System.Windows.Forms.Button();
            this.lbModule = new System.Windows.Forms.Label();
            this.DGV = new System.Windows.Forms.DataGridView();
            this.ColSensor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColFlow = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColDir = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColPWM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColBin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColInvert = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ckInvert = new System.Windows.Forms.CheckBox();
            this.grpWorkSwitch = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.DGV)).BeginInit();
            this.grpWorkSwitch.SuspendLayout();
            this.SuspendLayout();
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
            this.btnCancel.TabIndex = 153;
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
            this.btnOK.TabIndex = 152;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // tbPressure
            // 
            this.tbPressure.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPressure.Location = new System.Drawing.Point(214, 529);
            this.tbPressure.Name = "tbPressure";
            this.tbPressure.Size = new System.Drawing.Size(58, 29);
            this.tbPressure.TabIndex = 238;
            this.tbPressure.TabStop = false;
            this.tbPressure.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbPressure
            // 
            this.lbPressure.AutoSize = true;
            this.lbPressure.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPressure.Location = new System.Drawing.Point(59, 531);
            this.lbPressure.Name = "lbPressure";
            this.lbPressure.Size = new System.Drawing.Size(117, 24);
            this.lbPressure.TabIndex = 237;
            this.lbPressure.Text = "Pressure Pin";
            // 
            // ckMomentary
            // 
            this.ckMomentary.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckMomentary.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckMomentary.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckMomentary.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckMomentary.Location = new System.Drawing.Point(294, 28);
            this.ckMomentary.Name = "ckMomentary";
            this.ckMomentary.Size = new System.Drawing.Size(129, 40);
            this.ckMomentary.TabIndex = 236;
            this.ckMomentary.Text = "Momentary";
            this.ckMomentary.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckMomentary.UseVisualStyleBackColor = true;
            this.ckMomentary.CheckedChanged += new System.EventHandler(this.Boxes_TextChanged);
            // 
            // tbWrk
            // 
            this.tbWrk.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbWrk.Location = new System.Drawing.Point(172, 59);
            this.tbWrk.Name = "tbWrk";
            this.tbWrk.Size = new System.Drawing.Size(58, 29);
            this.tbWrk.TabIndex = 235;
            this.tbWrk.TabStop = false;
            this.tbWrk.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbWorkPin
            // 
            this.lbWorkPin.AutoSize = true;
            this.lbWorkPin.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbWorkPin.Location = new System.Drawing.Point(17, 61);
            this.lbWorkPin.Name = "lbWorkPin";
            this.lbWorkPin.Size = new System.Drawing.Size(86, 24);
            this.lbWorkPin.TabIndex = 234;
            this.lbWorkPin.Text = "Work Pin";
            // 
            // btnRescan
            // 
            this.btnRescan.BackColor = System.Drawing.Color.Transparent;
            this.btnRescan.FlatAppearance.BorderSize = 0;
            this.btnRescan.FlatAppearance.MouseDownBackColor = System.Drawing.Color.LightGreen;
            this.btnRescan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRescan.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRescan.Image = ((System.Drawing.Image)(resources.GetObject("btnRescan.Image")));
            this.btnRescan.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnRescan.Location = new System.Drawing.Point(304, 605);
            this.btnRescan.Name = "btnRescan";
            this.btnRescan.Size = new System.Drawing.Size(70, 63);
            this.btnRescan.TabIndex = 239;
            this.btnRescan.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnRescan.UseVisualStyleBackColor = false;
            this.btnRescan.Click += new System.EventHandler(this.btnRescan_Click);
            // 
            // lbModule
            // 
            this.lbModule.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbModule.Location = new System.Drawing.Point(59, 18);
            this.lbModule.Name = "lbModule";
            this.lbModule.Size = new System.Drawing.Size(403, 24);
            this.lbModule.TabIndex = 241;
            this.lbModule.Text = "Module 0";
            this.lbModule.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DGV
            // 
            this.DGV.AllowUserToAddRows = false;
            this.DGV.AllowUserToDeleteRows = false;
            this.DGV.AllowUserToResizeColumns = false;
            this.DGV.AllowUserToResizeRows = false;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DGV.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.DGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColSensor,
            this.ColFlow,
            this.ColDir,
            this.ColPWM,
            this.ColBin,
            this.ColInvert});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DGV.DefaultCellStyle = dataGridViewCellStyle6;
            this.DGV.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
            this.DGV.Location = new System.Drawing.Point(30, 55);
            this.DGV.Margin = new System.Windows.Forms.Padding(11);
            this.DGV.Name = "DGV";
            this.DGV.RowHeadersVisible = false;
            this.DGV.RowTemplate.Height = 40;
            this.DGV.Size = new System.Drawing.Size(480, 274);
            this.DGV.TabIndex = 240;
            this.DGV.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGV_CellClick);
            this.DGV.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGV_CellValueChanged);
            this.DGV.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.DGV_DataError);
            // 
            // ColSensor
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColSensor.DefaultCellStyle = dataGridViewCellStyle5;
            this.ColSensor.HeaderText = "Sensor";
            this.ColSensor.Name = "ColSensor";
            this.ColSensor.ReadOnly = true;
            this.ColSensor.Width = 78;
            // 
            // ColFlow
            // 
            this.ColFlow.HeaderText = "Flow";
            this.ColFlow.Name = "ColFlow";
            this.ColFlow.Width = 78;
            // 
            // ColDir
            // 
            this.ColDir.HeaderText = "Dir";
            this.ColDir.Name = "ColDir";
            this.ColDir.Width = 78;
            // 
            // ColPWM
            // 
            this.ColPWM.HeaderText = "PWM";
            this.ColPWM.Name = "ColPWM";
            this.ColPWM.Width = 78;
            // 
            // ColBin
            // 
            this.ColBin.HeaderText = "Bin";
            this.ColBin.Name = "ColBin";
            this.ColBin.Width = 78;
            // 
            // ColInvert
            // 
            this.ColInvert.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColInvert.HeaderText = "Invert";
            this.ColInvert.Name = "ColInvert";
            // 
            // ckInvert
            // 
            this.ckInvert.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckInvert.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckInvert.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckInvert.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckInvert.Location = new System.Drawing.Point(294, 84);
            this.ckInvert.Name = "ckInvert";
            this.ckInvert.Size = new System.Drawing.Size(129, 40);
            this.ckInvert.TabIndex = 242;
            this.ckInvert.Text = "Invert";
            this.ckInvert.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckInvert.UseVisualStyleBackColor = true;
            this.ckInvert.CheckedChanged += new System.EventHandler(this.Boxes_TextChanged);
            // 
            // grpWorkSwitch
            // 
            this.grpWorkSwitch.Controls.Add(this.ckInvert);
            this.grpWorkSwitch.Controls.Add(this.ckMomentary);
            this.grpWorkSwitch.Controls.Add(this.lbWorkPin);
            this.grpWorkSwitch.Controls.Add(this.tbWrk);
            this.grpWorkSwitch.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpWorkSwitch.Location = new System.Drawing.Point(42, 343);
            this.grpWorkSwitch.Name = "grpWorkSwitch";
            this.grpWorkSwitch.Size = new System.Drawing.Size(456, 142);
            this.grpWorkSwitch.TabIndex = 243;
            this.grpWorkSwitch.TabStop = false;
            this.grpWorkSwitch.Text = "Work Switch";
            this.grpWorkSwitch.Paint += new System.Windows.Forms.PaintEventHandler(this.grpWorkSwitch_Paint);
            // 
            // frmMenuPins
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 678);
            this.Controls.Add(this.grpWorkSwitch);
            this.Controls.Add(this.lbModule);
            this.Controls.Add(this.DGV);
            this.Controls.Add(this.btnRescan);
            this.Controls.Add(this.tbPressure);
            this.Controls.Add(this.lbPressure);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMenuPins";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuPins";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMenuPins_FormClosed);
            this.Load += new System.EventHandler(this.frmMenuPins_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DGV)).EndInit();
            this.grpWorkSwitch.ResumeLayout(false);
            this.grpWorkSwitch.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox tbPressure;
        private System.Windows.Forms.Label lbPressure;
        private System.Windows.Forms.CheckBox ckMomentary;
        private System.Windows.Forms.TextBox tbWrk;
        private System.Windows.Forms.Label lbWorkPin;
        private System.Windows.Forms.Button btnRescan;
        private System.Windows.Forms.Label lbModule;
        private System.Windows.Forms.DataGridView DGV;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColSensor;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColFlow;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColDir;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColPWM;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColBin;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColInvert;
        private System.Windows.Forms.CheckBox ckInvert;
        private System.Windows.Forms.GroupBox grpWorkSwitch;
    }
}
