namespace RateController.Forms
{
    partial class frmSaveType
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
            this.bntOK = new System.Windows.Forms.Button();
            this.ckShapeFile = new System.Windows.Forms.CheckBox();
            this.ckImage = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
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
            this.btnCancel.Location = new System.Drawing.Point(12, 168);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 72);
            this.btnCancel.TabIndex = 140;
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // bntOK
            // 
            this.bntOK.BackColor = System.Drawing.Color.Transparent;
            this.bntOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bntOK.FlatAppearance.BorderSize = 0;
            this.bntOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bntOK.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.bntOK.Image = global::RateController.Properties.Resources.OK;
            this.bntOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.bntOK.Location = new System.Drawing.Point(90, 168);
            this.bntOK.Name = "bntOK";
            this.bntOK.Size = new System.Drawing.Size(72, 72);
            this.bntOK.TabIndex = 139;
            this.bntOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.bntOK.UseVisualStyleBackColor = false;
            this.bntOK.Click += new System.EventHandler(this.bntOK_Click);
            // 
            // ckShapeFile
            // 
            this.ckShapeFile.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckShapeFile.Checked = true;
            this.ckShapeFile.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckShapeFile.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckShapeFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckShapeFile.Location = new System.Drawing.Point(12, 12);
            this.ckShapeFile.Name = "ckShapeFile";
            this.ckShapeFile.Size = new System.Drawing.Size(150, 64);
            this.ckShapeFile.TabIndex = 404;
            this.ckShapeFile.Text = "Shape File";
            this.ckShapeFile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckShapeFile.UseVisualStyleBackColor = true;
            // 
            // ckImage
            // 
            this.ckImage.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckImage.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckImage.Location = new System.Drawing.Point(12, 90);
            this.ckImage.Name = "ckImage";
            this.ckImage.Size = new System.Drawing.Size(150, 64);
            this.ckImage.TabIndex = 405;
            this.ckImage.Text = "Image";
            this.ckImage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckImage.UseVisualStyleBackColor = true;
            // 
            // frmSaveType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(177, 252);
            this.Controls.Add(this.ckImage);
            this.Controls.Add(this.ckShapeFile);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.bntOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSaveType";
            this.Text = "Save Type";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmSaveType_FormClosed);
            this.Load += new System.EventHandler(this.frmSaveType_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button bntOK;
        private System.Windows.Forms.CheckBox ckShapeFile;
        private System.Windows.Forms.CheckBox ckImage;
    }
}