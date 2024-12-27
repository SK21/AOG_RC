namespace RateController
{
    partial class frmResetQuantity
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
            this.ckReset = new System.Windows.Forms.CheckBox();
            this.ckFill = new System.Windows.Forms.CheckBox();
            this.tbQuantity = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ckReset
            // 
            this.ckReset.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckReset.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckReset.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckReset.Location = new System.Drawing.Point(12, 63);
            this.ckReset.Name = "ckReset";
            this.ckReset.Size = new System.Drawing.Size(164, 34);
            this.ckReset.TabIndex = 124;
            this.ckReset.Text = "Reset Applied";
            this.ckReset.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckReset.UseVisualStyleBackColor = true;
            this.ckReset.CheckedChanged += new System.EventHandler(this.ckReset_CheckedChanged);
            // 
            // ckFill
            // 
            this.ckFill.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckFill.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckFill.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckFill.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckFill.Location = new System.Drawing.Point(12, 12);
            this.ckFill.Name = "ckFill";
            this.ckFill.Size = new System.Drawing.Size(164, 34);
            this.ckFill.TabIndex = 125;
            this.ckFill.Text = "Fill Tank";
            this.ckFill.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckFill.UseVisualStyleBackColor = true;
            this.ckFill.CheckedChanged += new System.EventHandler(this.tbQuantity_TextChanged);
            // 
            // tbQuantity
            // 
            this.tbQuantity.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbQuantity.Location = new System.Drawing.Point(182, 14);
            this.tbQuantity.MaxLength = 8;
            this.tbQuantity.Name = "tbQuantity";
            this.tbQuantity.Size = new System.Drawing.Size(90, 30);
            this.tbQuantity.TabIndex = 153;
            this.tbQuantity.Text = "1250";
            this.tbQuantity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbQuantity.TextChanged += new System.EventHandler(this.tbQuantity_TextChanged);
            this.tbQuantity.Enter += new System.EventHandler(this.tbQuantity_Enter);
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
            this.btnCancel.Location = new System.Drawing.Point(37, 112);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 72);
            this.btnCancel.TabIndex = 155;
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.Transparent;
            this.btnOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnOK.FlatAppearance.BorderSize = 0;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnOK.Image = global::RateController.Properties.Resources.OK;
            this.btnOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnOK.Location = new System.Drawing.Point(169, 112);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(72, 72);
            this.btnOK.TabIndex = 154;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // frmResetQuantity
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(285, 186);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tbQuantity);
            this.Controls.Add(this.ckFill);
            this.Controls.Add(this.ckReset);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmResetQuantity";
            this.Text = "Reset Quantity";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmResetQuantity_FormClosed);
            this.Load += new System.EventHandler(this.frmResetQuantity_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox ckReset;
        private System.Windows.Forms.CheckBox ckFill;
        private System.Windows.Forms.TextBox tbQuantity;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
    }
}