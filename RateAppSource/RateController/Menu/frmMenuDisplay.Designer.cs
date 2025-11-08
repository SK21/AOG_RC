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
            this.ckRateDisplay = new System.Windows.Forms.CheckBox();
            this.ckLargeScreen = new System.Windows.Forms.CheckBox();
            this.ckTransparent = new System.Windows.Forms.CheckBox();
            this.ckMetric = new System.Windows.Forms.CheckBox();
            this.tbSimSpeed = new System.Windows.Forms.TextBox();
            this.lbSimUnits = new System.Windows.Forms.Label();
            this.ckSimSpeed = new System.Windows.Forms.CheckBox();
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
            this.btnCancel.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(70, 63);
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
            this.btnOK.Location = new System.Drawing.Point(458, 603);
            this.btnOK.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(70, 63);
            this.btnOK.TabIndex = 161;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // ckRateDisplay
            // 
            this.ckRateDisplay.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckRateDisplay.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckRateDisplay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckRateDisplay.Location = new System.Drawing.Point(284, 275);
            this.ckRateDisplay.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.ckRateDisplay.Name = "ckRateDisplay";
            this.ckRateDisplay.Size = new System.Drawing.Size(192, 34);
            this.ckRateDisplay.TabIndex = 334;
            this.ckRateDisplay.Text = "Rate";
            this.ckRateDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckRateDisplay.UseVisualStyleBackColor = true;
            this.ckRateDisplay.CheckedChanged += new System.EventHandler(this.ckLargeScreen_CheckedChanged);
            // 
            // ckLargeScreen
            // 
            this.ckLargeScreen.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckLargeScreen.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckLargeScreen.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckLargeScreen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckLargeScreen.Location = new System.Drawing.Point(29, 200);
            this.ckLargeScreen.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.ckLargeScreen.Name = "ckLargeScreen";
            this.ckLargeScreen.Size = new System.Drawing.Size(192, 34);
            this.ckLargeScreen.TabIndex = 127;
            this.ckLargeScreen.Text = "Large Screen";
            this.ckLargeScreen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckLargeScreen.UseVisualStyleBackColor = true;
            this.ckLargeScreen.CheckedChanged += new System.EventHandler(this.ckLargeScreen_CheckedChanged);
            // 
            // ckTransparent
            // 
            this.ckTransparent.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckTransparent.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckTransparent.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckTransparent.Location = new System.Drawing.Point(284, 200);
            this.ckTransparent.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.ckTransparent.Name = "ckTransparent";
            this.ckTransparent.Size = new System.Drawing.Size(192, 34);
            this.ckTransparent.TabIndex = 119;
            this.ckTransparent.Text = "Transparent";
            this.ckTransparent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckTransparent.UseVisualStyleBackColor = true;
            this.ckTransparent.CheckedChanged += new System.EventHandler(this.ckLargeScreen_CheckedChanged);
            // 
            // ckMetric
            // 
            this.ckMetric.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckMetric.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckMetric.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckMetric.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckMetric.Location = new System.Drawing.Point(29, 275);
            this.ckMetric.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.ckMetric.Name = "ckMetric";
            this.ckMetric.Size = new System.Drawing.Size(192, 34);
            this.ckMetric.TabIndex = 340;
            this.ckMetric.Text = "Metric Units";
            this.ckMetric.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckMetric.UseVisualStyleBackColor = true;
            this.ckMetric.CheckedChanged += new System.EventHandler(this.ckLargeScreen_CheckedChanged);
            // 
            // tbSimSpeed
            // 
            this.tbSimSpeed.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSimSpeed.Location = new System.Drawing.Point(338, 352);
            this.tbSimSpeed.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tbSimSpeed.MaxLength = 8;
            this.tbSimSpeed.Name = "tbSimSpeed";
            this.tbSimSpeed.Size = new System.Drawing.Size(78, 30);
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
            this.lbSimUnits.Location = new System.Drawing.Point(428, 355);
            this.lbSimUnits.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
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
            this.ckSimSpeed.Location = new System.Drawing.Point(29, 350);
            this.ckSimSpeed.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.ckSimSpeed.Name = "ckSimSpeed";
            this.ckSimSpeed.Size = new System.Drawing.Size(252, 34);
            this.ckSimSpeed.TabIndex = 337;
            this.ckSimSpeed.Text = "Simulate Speed (No AOG)";
            this.ckSimSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckSimSpeed.UseVisualStyleBackColor = true;
            this.ckSimSpeed.CheckedChanged += new System.EventHandler(this.ckLargeScreen_CheckedChanged);
            // 
            // frmMenuDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 678);
            this.Controls.Add(this.tbSimSpeed);
            this.Controls.Add(this.ckMetric);
            this.Controls.Add(this.lbSimUnits);
            this.Controls.Add(this.ckRateDisplay);
            this.Controls.Add(this.ckSimSpeed);
            this.Controls.Add(this.ckLargeScreen);
            this.Controls.Add(this.ckTransparent);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "frmMenuDisplay";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuDisplay";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMenuDisplay_FormClosed);
            this.Load += new System.EventHandler(this.frmMenuDisplay_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox ckLargeScreen;
        private System.Windows.Forms.CheckBox ckTransparent;
        private System.Windows.Forms.CheckBox ckMetric;
        private System.Windows.Forms.TextBox tbSimSpeed;
        private System.Windows.Forms.Label lbSimUnits;
        private System.Windows.Forms.CheckBox ckSimSpeed;
        private System.Windows.Forms.CheckBox ckRateDisplay;
    }
}