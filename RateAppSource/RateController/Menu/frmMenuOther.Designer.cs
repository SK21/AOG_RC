namespace RateController.Menu
{
    partial class frmMenuOther
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
            this.ckNoMaster = new System.Windows.Forms.CheckBox();
            this.tbSimSpeed = new System.Windows.Forms.TextBox();
            this.lbSimUnits = new System.Windows.Forms.Label();
            this.ckSimSpeed = new System.Windows.Forms.CheckBox();
            this.ckMetric = new System.Windows.Forms.CheckBox();
            this.ckWorkSwitch = new System.Windows.Forms.CheckBox();
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
            this.btnCancel.TabIndex = 164;
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
            this.btnOK.TabIndex = 163;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // ckNoMaster
            // 
            this.ckNoMaster.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckNoMaster.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckNoMaster.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckNoMaster.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckNoMaster.Location = new System.Drawing.Point(284, 236);
            this.ckNoMaster.Name = "ckNoMaster";
            this.ckNoMaster.Size = new System.Drawing.Size(164, 34);
            this.ckNoMaster.TabIndex = 337;
            this.ckNoMaster.Text = "Master Override";
            this.ckNoMaster.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckNoMaster.UseVisualStyleBackColor = true;
            this.ckNoMaster.CheckedChanged += new System.EventHandler(this.ckMetric_CheckedChanged);
            // 
            // tbSimSpeed
            // 
            this.tbSimSpeed.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSimSpeed.Location = new System.Drawing.Point(337, 313);
            this.tbSimSpeed.MaxLength = 8;
            this.tbSimSpeed.Name = "tbSimSpeed";
            this.tbSimSpeed.Size = new System.Drawing.Size(65, 30);
            this.tbSimSpeed.TabIndex = 335;
            this.tbSimSpeed.Text = "0";
            this.tbSimSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSimSpeed.TextChanged += new System.EventHandler(this.ckMetric_CheckedChanged);
            this.tbSimSpeed.Enter += new System.EventHandler(this.tbSimSpeed_Enter);
            this.tbSimSpeed.Validating += new System.ComponentModel.CancelEventHandler(this.tbSimSpeed_Validating);
            // 
            // lbSimUnits
            // 
            this.lbSimUnits.ForeColor = System.Drawing.Color.DarkGreen;
            this.lbSimUnits.Location = new System.Drawing.Point(408, 316);
            this.lbSimUnits.Name = "lbSimUnits";
            this.lbSimUnits.Size = new System.Drawing.Size(57, 24);
            this.lbSimUnits.TabIndex = 336;
            this.lbSimUnits.Text = "mph";
            this.lbSimUnits.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ckSimSpeed
            // 
            this.ckSimSpeed.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckSimSpeed.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckSimSpeed.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckSimSpeed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckSimSpeed.Location = new System.Drawing.Point(54, 311);
            this.ckSimSpeed.Name = "ckSimSpeed";
            this.ckSimSpeed.Size = new System.Drawing.Size(242, 34);
            this.ckSimSpeed.TabIndex = 334;
            this.ckSimSpeed.Text = "Simulate Speed (No AOG)";
            this.ckSimSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckSimSpeed.UseVisualStyleBackColor = true;
            this.ckSimSpeed.CheckedChanged += new System.EventHandler(this.ckMetric_CheckedChanged);
            // 
            // ckMetric
            // 
            this.ckMetric.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckMetric.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckMetric.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckMetric.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckMetric.Location = new System.Drawing.Point(186, 161);
            this.ckMetric.Name = "ckMetric";
            this.ckMetric.Size = new System.Drawing.Size(164, 34);
            this.ckMetric.TabIndex = 332;
            this.ckMetric.Text = "Metric Units";
            this.ckMetric.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckMetric.UseVisualStyleBackColor = true;
            this.ckMetric.CheckedChanged += new System.EventHandler(this.ckMetric_CheckedChanged);
            // 
            // ckWorkSwitch
            // 
            this.ckWorkSwitch.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckWorkSwitch.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckWorkSwitch.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckWorkSwitch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckWorkSwitch.Location = new System.Drawing.Point(54, 236);
            this.ckWorkSwitch.Name = "ckWorkSwitch";
            this.ckWorkSwitch.Size = new System.Drawing.Size(164, 34);
            this.ckWorkSwitch.TabIndex = 333;
            this.ckWorkSwitch.Text = "Work Switch";
            this.ckWorkSwitch.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckWorkSwitch.UseVisualStyleBackColor = true;
            this.ckWorkSwitch.CheckedChanged += new System.EventHandler(this.ckMetric_CheckedChanged);
            // 
            // frmMenuOther
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 630);
            this.Controls.Add(this.ckNoMaster);
            this.Controls.Add(this.tbSimSpeed);
            this.Controls.Add(this.lbSimUnits);
            this.Controls.Add(this.ckSimSpeed);
            this.Controls.Add(this.ckMetric);
            this.Controls.Add(this.ckWorkSwitch);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "frmMenuOther";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuOther";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMenuOther_FormClosed);
            this.Load += new System.EventHandler(this.frmMenuOther_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox ckNoMaster;
        private System.Windows.Forms.TextBox tbSimSpeed;
        private System.Windows.Forms.Label lbSimUnits;
        private System.Windows.Forms.CheckBox ckSimSpeed;
        private System.Windows.Forms.CheckBox ckMetric;
        private System.Windows.Forms.CheckBox ckWorkSwitch;
    }
}