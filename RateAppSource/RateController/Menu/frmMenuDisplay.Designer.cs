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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ckDefaultProduct = new System.Windows.Forms.CheckBox();
            this.ckLargeScreen = new System.Windows.Forms.CheckBox();
            this.ckSingle = new System.Windows.Forms.CheckBox();
            this.ckTransparent = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lbPressureOffset = new System.Windows.Forms.Label();
            this.tbPressureOffset = new System.Windows.Forms.TextBox();
            this.lbConID = new System.Windows.Forms.Label();
            this.tbPressureCal = new System.Windows.Forms.TextBox();
            this.ckPressure = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ckDefaultProduct);
            this.groupBox1.Controls.Add(this.ckLargeScreen);
            this.groupBox1.Controls.Add(this.ckSingle);
            this.groupBox1.Controls.Add(this.ckTransparent);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(45, 84);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(418, 115);
            this.groupBox1.TabIndex = 163;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Display";
            // 
            // ckDefaultProduct
            // 
            this.ckDefaultProduct.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckDefaultProduct.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckDefaultProduct.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckDefaultProduct.Location = new System.Drawing.Point(241, 68);
            this.ckDefaultProduct.Name = "ckDefaultProduct";
            this.ckDefaultProduct.Size = new System.Drawing.Size(164, 34);
            this.ckDefaultProduct.TabIndex = 333;
            this.ckDefaultProduct.Text = "Reset Products";
            this.ckDefaultProduct.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckDefaultProduct.UseVisualStyleBackColor = true;
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
            // 
            // ckTransparent
            // 
            this.ckTransparent.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckTransparent.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckTransparent.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckTransparent.Location = new System.Drawing.Point(22, 68);
            this.ckTransparent.Name = "ckTransparent";
            this.ckTransparent.Size = new System.Drawing.Size(164, 34);
            this.ckTransparent.TabIndex = 119;
            this.ckTransparent.Text = "Transparent";
            this.ckTransparent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckTransparent.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lbPressureOffset);
            this.groupBox2.Controls.Add(this.tbPressureOffset);
            this.groupBox2.Controls.Add(this.lbConID);
            this.groupBox2.Controls.Add(this.tbPressureCal);
            this.groupBox2.Controls.Add(this.ckPressure);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(45, 298);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(418, 133);
            this.groupBox2.TabIndex = 164;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Pressure";
            // 
            // lbPressureOffset
            // 
            this.lbPressureOffset.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPressureOffset.Location = new System.Drawing.Point(212, 81);
            this.lbPressureOffset.Name = "lbPressureOffset";
            this.lbPressureOffset.Size = new System.Drawing.Size(69, 23);
            this.lbPressureOffset.TabIndex = 155;
            this.lbPressureOffset.Text = "Offset";
            // 
            // tbPressureOffset
            // 
            this.tbPressureOffset.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPressureOffset.Location = new System.Drawing.Point(315, 78);
            this.tbPressureOffset.MaxLength = 8;
            this.tbPressureOffset.Name = "tbPressureOffset";
            this.tbPressureOffset.Size = new System.Drawing.Size(90, 30);
            this.tbPressureOffset.TabIndex = 153;
            this.tbPressureOffset.Text = "0";
            this.tbPressureOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbConID
            // 
            this.lbConID.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbConID.Location = new System.Drawing.Point(212, 33);
            this.lbConID.Name = "lbConID";
            this.lbConID.Size = new System.Drawing.Size(97, 23);
            this.lbConID.TabIndex = 154;
            this.lbConID.Text = "Cal Value";
            // 
            // tbPressureCal
            // 
            this.tbPressureCal.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPressureCal.Location = new System.Drawing.Point(315, 30);
            this.tbPressureCal.MaxLength = 8;
            this.tbPressureCal.Name = "tbPressureCal";
            this.tbPressureCal.Size = new System.Drawing.Size(90, 30);
            this.tbPressureCal.TabIndex = 152;
            this.tbPressureCal.Text = "0";
            this.tbPressureCal.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ckPressure
            // 
            this.ckPressure.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckPressure.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckPressure.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckPressure.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckPressure.Location = new System.Drawing.Point(22, 52);
            this.ckPressure.Name = "ckPressure";
            this.ckPressure.Size = new System.Drawing.Size(164, 34);
            this.ckPressure.TabIndex = 123;
            this.ckPressure.Text = "Show";
            this.ckPressure.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckPressure.UseVisualStyleBackColor = true;
            // 
            // frmMenuDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 630);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMenuDisplay";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuDisplay";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox ckDefaultProduct;
        private System.Windows.Forms.CheckBox ckLargeScreen;
        private System.Windows.Forms.CheckBox ckSingle;
        private System.Windows.Forms.CheckBox ckTransparent;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lbPressureOffset;
        private System.Windows.Forms.TextBox tbPressureOffset;
        private System.Windows.Forms.Label lbConID;
        private System.Windows.Forms.TextBox tbPressureCal;
        private System.Windows.Forms.CheckBox ckPressure;
    }
}