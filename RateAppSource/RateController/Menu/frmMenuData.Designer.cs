namespace RateController.Menu
{
    partial class frmMenuData
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMenuData));
            this.btnRight = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lbProduct = new System.Windows.Forms.Label();
            this.ckHours2 = new System.Windows.Forms.CheckBox();
            this.ckHours1 = new System.Windows.Forms.CheckBox();
            this.lbHours1value = new System.Windows.Forms.Label();
            this.lbHours1 = new System.Windows.Forms.Label();
            this.lbHours2value = new System.Windows.Forms.Label();
            this.lbHours2 = new System.Windows.Forms.Label();
            this.ckQuantity2 = new System.Windows.Forms.CheckBox();
            this.ckQuantity1 = new System.Windows.Forms.CheckBox();
            this.ckArea2 = new System.Windows.Forms.CheckBox();
            this.ckArea1 = new System.Windows.Forms.CheckBox();
            this.lbQuantity1 = new System.Windows.Forms.Label();
            this.lbGallons1 = new System.Windows.Forms.Label();
            this.lbQuantity2 = new System.Windows.Forms.Label();
            this.lbGallons2 = new System.Windows.Forms.Label();
            this.lbArea1 = new System.Windows.Forms.Label();
            this.lbAcres1 = new System.Windows.Forms.Label();
            this.lbArea2 = new System.Windows.Forms.Label();
            this.lbAcres2 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // btnRight
            // 
            this.btnRight.BackColor = System.Drawing.Color.Transparent;
            this.btnRight.FlatAppearance.BorderSize = 0;
            this.btnRight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRight.Image = global::RateController.Properties.Resources.ArrowRight1;
            this.btnRight.Location = new System.Drawing.Point(302, 603);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(70, 63);
            this.btnRight.TabIndex = 176;
            this.btnRight.UseVisualStyleBackColor = false;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnLeft
            // 
            this.btnLeft.BackColor = System.Drawing.Color.Transparent;
            this.btnLeft.FlatAppearance.BorderSize = 0;
            this.btnLeft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLeft.Image = global::RateController.Properties.Resources.ArrowLeft1;
            this.btnLeft.Location = new System.Drawing.Point(224, 603);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(70, 63);
            this.btnLeft.TabIndex = 175;
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
            this.btnCancel.Location = new System.Drawing.Point(380, 603);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(70, 63);
            this.btnCancel.TabIndex = 174;
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
            this.btnOK.TabIndex = 173;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lbProduct
            // 
            this.lbProduct.BackColor = System.Drawing.SystemColors.Control;
            this.lbProduct.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbProduct.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbProduct.Location = new System.Drawing.Point(71, 9);
            this.lbProduct.Name = "lbProduct";
            this.lbProduct.Size = new System.Drawing.Size(396, 36);
            this.lbProduct.TabIndex = 191;
            this.lbProduct.Text = "Product";
            this.lbProduct.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ckHours2
            // 
            this.ckHours2.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckHours2.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckHours2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckHours2.Image = ((System.Drawing.Image)(resources.GetObject("ckHours2.Image")));
            this.ckHours2.Location = new System.Drawing.Point(395, 464);
            this.ckHours2.Name = "ckHours2";
            this.ckHours2.Size = new System.Drawing.Size(52, 46);
            this.ckHours2.TabIndex = 209;
            this.ckHours2.UseVisualStyleBackColor = true;
            this.ckHours2.CheckedChanged += new System.EventHandler(this.ckArea1_CheckedChanged);
            // 
            // ckHours1
            // 
            this.ckHours1.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckHours1.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckHours1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckHours1.Image = ((System.Drawing.Image)(resources.GetObject("ckHours1.Image")));
            this.ckHours1.Location = new System.Drawing.Point(395, 390);
            this.ckHours1.Name = "ckHours1";
            this.ckHours1.Size = new System.Drawing.Size(52, 46);
            this.ckHours1.TabIndex = 208;
            this.ckHours1.UseVisualStyleBackColor = true;
            this.ckHours1.CheckedChanged += new System.EventHandler(this.ckArea1_CheckedChanged);
            // 
            // lbHours1value
            // 
            this.lbHours1value.BackColor = System.Drawing.Color.Transparent;
            this.lbHours1value.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbHours1value.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbHours1value.Location = new System.Drawing.Point(222, 401);
            this.lbHours1value.Name = "lbHours1value";
            this.lbHours1value.Size = new System.Drawing.Size(130, 25);
            this.lbHours1value.TabIndex = 206;
            this.lbHours1value.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbHours1
            // 
            this.lbHours1.AutoSize = true;
            this.lbHours1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbHours1.Location = new System.Drawing.Point(72, 402);
            this.lbHours1.Name = "lbHours1";
            this.lbHours1.Size = new System.Drawing.Size(75, 23);
            this.lbHours1.TabIndex = 207;
            this.lbHours1.Text = "Hours 1";
            // 
            // lbHours2value
            // 
            this.lbHours2value.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbHours2value.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbHours2value.Location = new System.Drawing.Point(222, 475);
            this.lbHours2value.Name = "lbHours2value";
            this.lbHours2value.Size = new System.Drawing.Size(130, 25);
            this.lbHours2value.TabIndex = 204;
            this.lbHours2value.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbHours2
            // 
            this.lbHours2.AutoSize = true;
            this.lbHours2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbHours2.Location = new System.Drawing.Point(72, 476);
            this.lbHours2.Name = "lbHours2";
            this.lbHours2.Size = new System.Drawing.Size(75, 23);
            this.lbHours2.TabIndex = 205;
            this.lbHours2.Text = "Hours 2";
            // 
            // ckQuantity2
            // 
            this.ckQuantity2.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckQuantity2.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckQuantity2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckQuantity2.Image = ((System.Drawing.Image)(resources.GetObject("ckQuantity2.Image")));
            this.ckQuantity2.Location = new System.Drawing.Point(395, 316);
            this.ckQuantity2.Name = "ckQuantity2";
            this.ckQuantity2.Size = new System.Drawing.Size(52, 46);
            this.ckQuantity2.TabIndex = 203;
            this.ckQuantity2.UseVisualStyleBackColor = true;
            this.ckQuantity2.CheckedChanged += new System.EventHandler(this.ckArea1_CheckedChanged);
            // 
            // ckQuantity1
            // 
            this.ckQuantity1.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckQuantity1.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckQuantity1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckQuantity1.Image = ((System.Drawing.Image)(resources.GetObject("ckQuantity1.Image")));
            this.ckQuantity1.Location = new System.Drawing.Point(395, 242);
            this.ckQuantity1.Name = "ckQuantity1";
            this.ckQuantity1.Size = new System.Drawing.Size(52, 46);
            this.ckQuantity1.TabIndex = 202;
            this.ckQuantity1.UseVisualStyleBackColor = true;
            this.ckQuantity1.CheckedChanged += new System.EventHandler(this.ckArea1_CheckedChanged);
            // 
            // ckArea2
            // 
            this.ckArea2.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckArea2.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckArea2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckArea2.Image = global::RateController.Properties.Resources.refresh;
            this.ckArea2.Location = new System.Drawing.Point(395, 168);
            this.ckArea2.Name = "ckArea2";
            this.ckArea2.Size = new System.Drawing.Size(52, 46);
            this.ckArea2.TabIndex = 201;
            this.ckArea2.UseVisualStyleBackColor = true;
            this.ckArea2.CheckedChanged += new System.EventHandler(this.ckArea1_CheckedChanged);
            // 
            // ckArea1
            // 
            this.ckArea1.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckArea1.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckArea1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckArea1.Image = global::RateController.Properties.Resources.refresh;
            this.ckArea1.Location = new System.Drawing.Point(395, 94);
            this.ckArea1.Name = "ckArea1";
            this.ckArea1.Size = new System.Drawing.Size(52, 46);
            this.ckArea1.TabIndex = 200;
            this.ckArea1.UseVisualStyleBackColor = true;
            this.ckArea1.CheckedChanged += new System.EventHandler(this.ckArea1_CheckedChanged);
            // 
            // lbQuantity1
            // 
            this.lbQuantity1.BackColor = System.Drawing.Color.Transparent;
            this.lbQuantity1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbQuantity1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbQuantity1.Location = new System.Drawing.Point(222, 253);
            this.lbQuantity1.Name = "lbQuantity1";
            this.lbQuantity1.Size = new System.Drawing.Size(130, 25);
            this.lbQuantity1.TabIndex = 198;
            this.lbQuantity1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbGallons1
            // 
            this.lbGallons1.AutoSize = true;
            this.lbGallons1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbGallons1.Location = new System.Drawing.Point(72, 254);
            this.lbGallons1.Name = "lbGallons1";
            this.lbGallons1.Size = new System.Drawing.Size(96, 23);
            this.lbGallons1.TabIndex = 199;
            this.lbGallons1.Text = "Quantity 1";
            // 
            // lbQuantity2
            // 
            this.lbQuantity2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbQuantity2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbQuantity2.Location = new System.Drawing.Point(222, 327);
            this.lbQuantity2.Name = "lbQuantity2";
            this.lbQuantity2.Size = new System.Drawing.Size(130, 25);
            this.lbQuantity2.TabIndex = 196;
            this.lbQuantity2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbGallons2
            // 
            this.lbGallons2.AutoSize = true;
            this.lbGallons2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbGallons2.Location = new System.Drawing.Point(72, 328);
            this.lbGallons2.Name = "lbGallons2";
            this.lbGallons2.Size = new System.Drawing.Size(96, 23);
            this.lbGallons2.TabIndex = 197;
            this.lbGallons2.Text = "Quantity 2";
            // 
            // lbArea1
            // 
            this.lbArea1.BackColor = System.Drawing.Color.Transparent;
            this.lbArea1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbArea1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbArea1.Location = new System.Drawing.Point(222, 105);
            this.lbArea1.Name = "lbArea1";
            this.lbArea1.Size = new System.Drawing.Size(130, 25);
            this.lbArea1.TabIndex = 194;
            this.lbArea1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbAcres1
            // 
            this.lbAcres1.AutoSize = true;
            this.lbAcres1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbAcres1.Location = new System.Drawing.Point(72, 106);
            this.lbAcres1.Name = "lbAcres1";
            this.lbAcres1.Size = new System.Drawing.Size(71, 23);
            this.lbAcres1.TabIndex = 195;
            this.lbAcres1.Text = "Acres 1";
            // 
            // lbArea2
            // 
            this.lbArea2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbArea2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbArea2.Location = new System.Drawing.Point(222, 179);
            this.lbArea2.Name = "lbArea2";
            this.lbArea2.Size = new System.Drawing.Size(130, 25);
            this.lbArea2.TabIndex = 192;
            this.lbArea2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbAcres2
            // 
            this.lbAcres2.AutoSize = true;
            this.lbAcres2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbAcres2.Location = new System.Drawing.Point(72, 180);
            this.lbAcres2.Name = "lbAcres2";
            this.lbAcres2.Size = new System.Drawing.Size(71, 23);
            this.lbAcres2.TabIndex = 193;
            this.lbAcres2.Text = "Acres 2";
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // frmMenuData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 678);
            this.Controls.Add(this.ckHours2);
            this.Controls.Add(this.ckHours1);
            this.Controls.Add(this.lbHours1value);
            this.Controls.Add(this.lbHours1);
            this.Controls.Add(this.lbHours2value);
            this.Controls.Add(this.lbHours2);
            this.Controls.Add(this.ckQuantity2);
            this.Controls.Add(this.ckQuantity1);
            this.Controls.Add(this.ckArea2);
            this.Controls.Add(this.ckArea1);
            this.Controls.Add(this.lbQuantity1);
            this.Controls.Add(this.lbGallons1);
            this.Controls.Add(this.lbQuantity2);
            this.Controls.Add(this.lbGallons2);
            this.Controls.Add(this.lbArea1);
            this.Controls.Add(this.lbAcres1);
            this.Controls.Add(this.lbArea2);
            this.Controls.Add(this.lbAcres2);
            this.Controls.Add(this.lbProduct);
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.btnLeft);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMenuData";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuData";
            this.Activated += new System.EventHandler(this.frmMenuData_Activated);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMenuData_FormClosed);
            this.Load += new System.EventHandler(this.frmMenuData_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lbProduct;
        private System.Windows.Forms.CheckBox ckHours2;
        private System.Windows.Forms.CheckBox ckHours1;
        private System.Windows.Forms.Label lbHours1value;
        private System.Windows.Forms.Label lbHours1;
        private System.Windows.Forms.Label lbHours2value;
        private System.Windows.Forms.Label lbHours2;
        private System.Windows.Forms.CheckBox ckQuantity2;
        private System.Windows.Forms.CheckBox ckQuantity1;
        private System.Windows.Forms.CheckBox ckArea2;
        private System.Windows.Forms.CheckBox ckArea1;
        private System.Windows.Forms.Label lbQuantity1;
        private System.Windows.Forms.Label lbGallons1;
        private System.Windows.Forms.Label lbQuantity2;
        private System.Windows.Forms.Label lbGallons2;
        private System.Windows.Forms.Label lbArea1;
        private System.Windows.Forms.Label lbAcres1;
        private System.Windows.Forms.Label lbArea2;
        private System.Windows.Forms.Label lbAcres2;
        private System.Windows.Forms.Timer timer1;
    }
}