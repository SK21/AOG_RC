namespace RateController.Forms
{
    partial class frmQuantityUnit
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
            this.lbName = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.lbType = new System.Windows.Forms.Label();
            this.cbType = new System.Windows.Forms.ComboBox();
            this.lbFactor = new System.Windows.Forms.Label();
            this.tbFactor = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lbName
            //
            this.lbName.Location = new System.Drawing.Point(12, 15);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(185, 29);
            this.lbName.TabIndex = 10;
            this.lbName.Text = "Name";
            //
            // tbName
            //
            this.tbName.Location = new System.Drawing.Point(203, 12);
            this.tbName.MaxLength = 15;
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(200, 29);
            this.tbName.TabIndex = 0;
            this.tbName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbName.TextChanged += new System.EventHandler(this.tbName_TextChanged);
            //
            // lbType
            //
            this.lbType.Location = new System.Drawing.Point(12, 60);
            this.lbType.Name = "lbType";
            this.lbType.Size = new System.Drawing.Size(185, 29);
            this.lbType.TabIndex = 11;
            this.lbType.Text = "Type";
            //
            // cbType
            //
            this.cbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbType.FormattingEnabled = true;
            this.cbType.Items.AddRange(new object[] {
            "None",
            "Volume",
            "Mass",
            "Count"});
            this.cbType.Location = new System.Drawing.Point(203, 57);
            this.cbType.Name = "cbType";
            this.cbType.Size = new System.Drawing.Size(200, 32);
            this.cbType.TabIndex = 1;
            this.cbType.SelectedIndexChanged += new System.EventHandler(this.cbType_SelectedIndexChanged);
            //
            // lbFactor
            //
            this.lbFactor.Location = new System.Drawing.Point(12, 102);
            this.lbFactor.Name = "lbFactor";
            this.lbFactor.Size = new System.Drawing.Size(185, 58);
            this.lbFactor.TabIndex = 12;
            this.lbFactor.Text = "Litres, kg or seeds per unit";
            //
            // tbFactor
            //
            this.tbFactor.Location = new System.Drawing.Point(203, 105);
            this.tbFactor.MaxLength = 10;
            this.tbFactor.Name = "tbFactor";
            this.tbFactor.Size = new System.Drawing.Size(120, 29);
            this.tbFactor.TabIndex = 2;
            this.tbFactor.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbFactor.TextChanged += new System.EventHandler(this.tbFactor_TextChanged);
            this.tbFactor.Enter += new System.EventHandler(this.tbFactor_Enter);
            //
            // btnCancel
            //
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnCancel.Image = global::RateController.Properties.Resources.Cancel64;
            this.btnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCancel.Location = new System.Drawing.Point(253, 166);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 72);
            this.btnCancel.TabIndex = 4;
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
            this.btnOK.Location = new System.Drawing.Point(331, 166);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(72, 72);
            this.btnOK.TabIndex = 3;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            //
            // frmQuantityUnit
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(415, 245);
            this.Controls.Add(this.lbName);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.lbType);
            this.Controls.Add(this.cbType);
            this.Controls.Add(this.lbFactor);
            this.Controls.Add(this.tbFactor);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmQuantityUnit";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Other";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmQuantityUnit_FormClosing);
            this.Load += new System.EventHandler(this.frmQuantityUnit_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Label lbType;
        private System.Windows.Forms.ComboBox cbType;
        private System.Windows.Forms.Label lbFactor;
        private System.Windows.Forms.TextBox tbFactor;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
    }
}
