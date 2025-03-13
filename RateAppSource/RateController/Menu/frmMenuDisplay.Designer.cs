namespace RateController.Menu
{
    partial class frmMenuDisplay
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.gbDisplay = new System.Windows.Forms.GroupBox();
            this.ckReset = new System.Windows.Forms.CheckBox();
            this.ckLargeScreen = new System.Windows.Forms.CheckBox();
            this.ckSingle = new System.Windows.Forms.CheckBox();
            this.ckTransparent = new System.Windows.Forms.CheckBox();
            this.gbOther = new System.Windows.Forms.GroupBox();
            this.ckMetric = new System.Windows.Forms.CheckBox();
            this.tbSimSpeed = new System.Windows.Forms.TextBox();
            this.lbSimUnits = new System.Windows.Forms.Label();
            this.ckSimSpeed = new System.Windows.Forms.CheckBox();
            this.gbDisplay.SuspendLayout();
            this.gbOther.SuspendLayout();
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
            this.btnCancel.Location = new System.Drawing.Point(378, 546);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 72);
            this.btnCancel.TabIndex = 162;
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
            this.btnOK.TabIndex = 161;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // gbDisplay
            // 
            this.gbDisplay.Controls.Add(this.ckReset);
            this.gbDisplay.Controls.Add(this.ckLargeScreen);
            this.gbDisplay.Controls.Add(this.ckSingle);
            this.gbDisplay.Controls.Add(this.ckTransparent);
            this.gbDisplay.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbDisplay.Location = new System.Drawing.Point(54, 116);
            this.gbDisplay.Name = "gbDisplay";
            this.gbDisplay.Size = new System.Drawing.Size(418, 144);
            this.gbDisplay.TabIndex = 163;
            this.gbDisplay.TabStop = false;
            this.gbDisplay.Text = "Display";
            this.gbDisplay.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox1_Paint);
            // 
            // ckReset
            // 
            this.ckReset.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckReset.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckReset.Location = new System.Drawing.Point(241, 89);
            this.ckReset.Name = "ckReset";
            this.ckReset.Size = new System.Drawing.Size(164, 34);
            this.ckReset.TabIndex = 333;
            this.ckReset.Text = "Reset Products";
            this.ckReset.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckReset.UseVisualStyleBackColor = true;
            this.ckReset.CheckedChanged += new System.EventHandler(this.ckReset_CheckedChanged);
            // 
            // ckLargeScreen
            // 
            this.ckLargeScreen.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckLargeScreen.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckLargeScreen.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckLargeScreen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckLargeScreen.Location = new System.Drawing.Point(22, 28);
            this.ckLargeScreen.Name = "ckLargeScreen";
            this.ckLargeScreen.Size = new System.Drawing.Size(164, 34);
            this.ckLargeScreen.TabIndex = 127;
            this.ckLargeScreen.Text = "Large Screen";
            this.ckLargeScreen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckLargeScreen.UseVisualStyleBackColor = true;
            this.ckLargeScreen.CheckedChanged += new System.EventHandler(this.ckLargeScreen_CheckedChanged);
            // 
            // ckSingle
            // 
            this.ckSingle.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckSingle.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckSingle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckSingle.Location = new System.Drawing.Point(241, 28);
            this.ckSingle.Name = "ckSingle";
            this.ckSingle.Size = new System.Drawing.Size(164, 34);
            this.ckSingle.TabIndex = 128;
            this.ckSingle.Text = "Single Product";
            this.ckSingle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckSingle.UseVisualStyleBackColor = true;
            this.ckSingle.CheckedChanged += new System.EventHandler(this.ckSingle_CheckedChanged);
            // 
            // ckTransparent
            // 
            this.ckTransparent.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckTransparent.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckTransparent.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckTransparent.Location = new System.Drawing.Point(22, 89);
            this.ckTransparent.Name = "ckTransparent";
            this.ckTransparent.Size = new System.Drawing.Size(164, 34);
            this.ckTransparent.TabIndex = 119;
            this.ckTransparent.Text = "Transparent";
            this.ckTransparent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckTransparent.UseVisualStyleBackColor = true;
            this.ckTransparent.CheckedChanged += new System.EventHandler(this.ckLargeScreen_CheckedChanged);
            // 
            // gbOther
            // 
            this.gbOther.Controls.Add(this.ckMetric);
            this.gbOther.Controls.Add(this.tbSimSpeed);
            this.gbOther.Controls.Add(this.lbSimUnits);
            this.gbOther.Controls.Add(this.ckSimSpeed);
            this.gbOther.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbOther.Location = new System.Drawing.Point(54, 305);
            this.gbOther.Name = "gbOther";
            this.gbOther.Size = new System.Drawing.Size(418, 135);
            this.gbOther.TabIndex = 227;
            this.gbOther.TabStop = false;
            this.gbOther.Text = "Other";
            this.gbOther.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox1_Paint);
            // 
            // ckMetric
            // 
            this.ckMetric.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckMetric.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckMetric.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckMetric.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckMetric.Location = new System.Drawing.Point(22, 28);
            this.ckMetric.Name = "ckMetric";
            this.ckMetric.Size = new System.Drawing.Size(164, 34);
            this.ckMetric.TabIndex = 340;
            this.ckMetric.Text = "Metric Units";
            this.ckMetric.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckMetric.UseVisualStyleBackColor = true;
            this.ckMetric.CheckedChanged += new System.EventHandler(this.ckLargeScreen_CheckedChanged);
            // 
            // tbSimSpeed
            // 
            this.tbSimSpeed.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSimSpeed.Location = new System.Drawing.Point(284, 85);
            this.tbSimSpeed.MaxLength = 8;
            this.tbSimSpeed.Name = "tbSimSpeed";
            this.tbSimSpeed.Size = new System.Drawing.Size(65, 30);
            this.tbSimSpeed.TabIndex = 338;
            this.tbSimSpeed.Text = "0";
            this.tbSimSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSimSpeed.TextChanged += new System.EventHandler(this.ckLargeScreen_CheckedChanged);
            this.tbSimSpeed.Enter += new System.EventHandler(this.tbSimSpeed_Enter);
            this.tbSimSpeed.Validating += new System.ComponentModel.CancelEventHandler(this.tbSimSpeed_Validating);
            // 
            // lbSimUnits
            // 
            this.lbSimUnits.AutoSize = true;
            this.lbSimUnits.ForeColor = System.Drawing.Color.DarkGreen;
            this.lbSimUnits.Location = new System.Drawing.Point(355, 88);
            this.lbSimUnits.Name = "lbSimUnits";
            this.lbSimUnits.Size = new System.Drawing.Size(48, 24);
            this.lbSimUnits.TabIndex = 339;
            this.lbSimUnits.Text = "mph";
            this.lbSimUnits.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ckSimSpeed
            // 
            this.ckSimSpeed.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckSimSpeed.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckSimSpeed.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckSimSpeed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckSimSpeed.Location = new System.Drawing.Point(22, 83);
            this.ckSimSpeed.Name = "ckSimSpeed";
            this.ckSimSpeed.Size = new System.Drawing.Size(242, 34);
            this.ckSimSpeed.TabIndex = 337;
            this.ckSimSpeed.Text = "Simulate Speed (No AOG)";
            this.ckSimSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckSimSpeed.UseVisualStyleBackColor = true;
            this.ckSimSpeed.CheckedChanged += new System.EventHandler(this.ckLargeScreen_CheckedChanged);
            // 
            // frmMenuDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 630);
            this.Controls.Add(this.gbOther);
            this.Controls.Add(this.gbDisplay);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMenuDisplay";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuDisplay";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMenuDisplay_FormClosed);
            this.Load += new System.EventHandler(this.frmMenuDisplay_Load);
            this.gbDisplay.ResumeLayout(false);
            this.gbOther.ResumeLayout(false);
            this.gbOther.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox gbDisplay;
        private System.Windows.Forms.CheckBox ckReset;
        private System.Windows.Forms.CheckBox ckLargeScreen;
        private System.Windows.Forms.CheckBox ckSingle;
        private System.Windows.Forms.CheckBox ckTransparent;
        private System.Windows.Forms.GroupBox gbOther;
        private System.Windows.Forms.CheckBox ckMetric;
        private System.Windows.Forms.TextBox tbSimSpeed;
        private System.Windows.Forms.Label lbSimUnits;
        private System.Windows.Forms.CheckBox ckSimSpeed;
    }
}