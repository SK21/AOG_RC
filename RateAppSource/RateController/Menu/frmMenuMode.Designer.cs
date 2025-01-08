namespace RateController.Menu
{
    partial class frmMenuMode
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
            this.btnRight = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.rbModeApplied = new System.Windows.Forms.RadioButton();
            this.rbModeTarget = new System.Windows.Forms.RadioButton();
            this.rbModeConstant = new System.Windows.Forms.RadioButton();
            this.rbModeControlledUPM = new System.Windows.Forms.RadioButton();
            this.lbProduct = new System.Windows.Forms.Label();
            this.SuspendLayout();
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
            this.btnRight.TabIndex = 168;
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
            this.btnLeft.TabIndex = 167;
            this.btnLeft.UseVisualStyleBackColor = false;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
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
            this.btnCancel.TabIndex = 166;
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
            this.btnOK.TabIndex = 165;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // rbModeApplied
            // 
            this.rbModeApplied.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbModeApplied.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbModeApplied.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbModeApplied.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbModeApplied.Location = new System.Drawing.Point(54, 312);
            this.rbModeApplied.Margin = new System.Windows.Forms.Padding(2);
            this.rbModeApplied.Name = "rbModeApplied";
            this.rbModeApplied.Size = new System.Drawing.Size(434, 74);
            this.rbModeApplied.TabIndex = 172;
            this.rbModeApplied.Tag = "0";
            this.rbModeApplied.Text = "3. Document applied rate, no rate control";
            this.rbModeApplied.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbModeApplied.UseVisualStyleBackColor = true;
            this.rbModeApplied.CheckedChanged += new System.EventHandler(this.rbModeControlledUPM_CheckedChanged);
            // 
            // rbModeTarget
            // 
            this.rbModeTarget.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbModeTarget.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbModeTarget.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbModeTarget.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbModeTarget.Location = new System.Drawing.Point(54, 432);
            this.rbModeTarget.Margin = new System.Windows.Forms.Padding(2);
            this.rbModeTarget.Name = "rbModeTarget";
            this.rbModeTarget.Size = new System.Drawing.Size(434, 74);
            this.rbModeTarget.TabIndex = 171;
            this.rbModeTarget.Tag = "0";
            this.rbModeTarget.Text = "4. Document target rate, no rate control,\r\nno module";
            this.rbModeTarget.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbModeTarget.UseVisualStyleBackColor = true;
            this.rbModeTarget.CheckedChanged += new System.EventHandler(this.rbModeControlledUPM_CheckedChanged);
            // 
            // rbModeConstant
            // 
            this.rbModeConstant.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbModeConstant.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbModeConstant.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbModeConstant.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbModeConstant.Location = new System.Drawing.Point(54, 192);
            this.rbModeConstant.Margin = new System.Windows.Forms.Padding(2);
            this.rbModeConstant.Name = "rbModeConstant";
            this.rbModeConstant.Size = new System.Drawing.Size(434, 74);
            this.rbModeConstant.TabIndex = 170;
            this.rbModeConstant.Tag = "0";
            this.rbModeConstant.Text = "2. Constant UPM, compensate total applied  (metered bypass valves)";
            this.rbModeConstant.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbModeConstant.UseVisualStyleBackColor = true;
            this.rbModeConstant.CheckedChanged += new System.EventHandler(this.rbModeControlledUPM_CheckedChanged);
            // 
            // rbModeControlledUPM
            // 
            this.rbModeControlledUPM.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbModeControlledUPM.Checked = true;
            this.rbModeControlledUPM.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbModeControlledUPM.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbModeControlledUPM.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbModeControlledUPM.Location = new System.Drawing.Point(54, 72);
            this.rbModeControlledUPM.Margin = new System.Windows.Forms.Padding(2);
            this.rbModeControlledUPM.Name = "rbModeControlledUPM";
            this.rbModeControlledUPM.Size = new System.Drawing.Size(434, 74);
            this.rbModeControlledUPM.TabIndex = 169;
            this.rbModeControlledUPM.TabStop = true;
            this.rbModeControlledUPM.Tag = "0";
            this.rbModeControlledUPM.Text = "1. Section controlled UPM, varies with width  (on/off valves)\r\n";
            this.rbModeControlledUPM.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbModeControlledUPM.UseVisualStyleBackColor = true;
            this.rbModeControlledUPM.CheckedChanged += new System.EventHandler(this.rbModeControlledUPM_CheckedChanged);
            // 
            // lbProduct
            // 
            this.lbProduct.BackColor = System.Drawing.SystemColors.Control;
            this.lbProduct.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbProduct.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbProduct.Location = new System.Drawing.Point(71, 9);
            this.lbProduct.Name = "lbProduct";
            this.lbProduct.Size = new System.Drawing.Size(396, 36);
            this.lbProduct.TabIndex = 189;
            this.lbProduct.Text = "Product";
            this.lbProduct.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // frmMenuMode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 630);
            this.Controls.Add(this.lbProduct);
            this.Controls.Add(this.rbModeApplied);
            this.Controls.Add(this.rbModeTarget);
            this.Controls.Add(this.rbModeConstant);
            this.Controls.Add(this.rbModeControlledUPM);
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.btnLeft);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMenuMode";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuMode";
            this.Activated += new System.EventHandler(this.frmMenuMode_Activated);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMenuMode_FormClosed);
            this.Load += new System.EventHandler(this.frmMenuMode_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.RadioButton rbModeApplied;
        private System.Windows.Forms.RadioButton rbModeTarget;
        private System.Windows.Forms.RadioButton rbModeConstant;
        private System.Windows.Forms.RadioButton rbModeControlledUPM;
        private System.Windows.Forms.Label lbProduct;
    }
}