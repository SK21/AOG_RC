namespace RateController.Menu
{
    partial class frmMenuFolders
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
            this.ckOverwrite = new System.Windows.Forms.CheckBox();
            this.ckCopyData = new System.Windows.Forms.CheckBox();
            this.tbNewLocation = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbCurrent = new System.Windows.Forms.Label();
            this.btnNewFolder = new System.Windows.Forms.Button();
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
            this.btnCancel.Location = new System.Drawing.Point(377, 600);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(70, 63);
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
            this.btnOK.Location = new System.Drawing.Point(455, 600);
            this.btnOK.Margin = new System.Windows.Forms.Padding(6);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(70, 63);
            this.btnOK.TabIndex = 163;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // ckOverwrite
            // 
            this.ckOverwrite.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckOverwrite.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckOverwrite.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckOverwrite.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckOverwrite.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.ckOverwrite.Location = new System.Drawing.Point(298, 421);
            this.ckOverwrite.Name = "ckOverwrite";
            this.ckOverwrite.Size = new System.Drawing.Size(153, 52);
            this.ckOverwrite.TabIndex = 171;
            this.ckOverwrite.Text = "Overwrite";
            this.ckOverwrite.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckOverwrite.UseVisualStyleBackColor = true;
            // 
            // ckCopyData
            // 
            this.ckCopyData.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckCopyData.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckCopyData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckCopyData.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckCopyData.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.ckCopyData.Location = new System.Drawing.Point(76, 421);
            this.ckCopyData.Name = "ckCopyData";
            this.ckCopyData.Size = new System.Drawing.Size(153, 52);
            this.ckCopyData.TabIndex = 172;
            this.ckCopyData.Text = "Copy Data";
            this.ckCopyData.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckCopyData.UseVisualStyleBackColor = true;
            // 
            // tbNewLocation
            // 
            this.tbNewLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbNewLocation.Location = new System.Drawing.Point(54, 317);
            this.tbNewLocation.Multiline = true;
            this.tbNewLocation.Name = "tbNewLocation";
            this.tbNewLocation.Size = new System.Drawing.Size(440, 56);
            this.tbNewLocation.TabIndex = 245;
            this.tbNewLocation.TabStop = false;
            this.tbNewLocation.TextChanged += new System.EventHandler(this.tbNewLocation_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Enabled = false;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(54, 126);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(147, 24);
            this.label1.TabIndex = 246;
            this.label1.Text = "Current location:";
            // 
            // lbCurrent
            // 
            this.lbCurrent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbCurrent.Enabled = false;
            this.lbCurrent.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCurrent.Location = new System.Drawing.Point(54, 159);
            this.lbCurrent.Name = "lbCurrent";
            this.lbCurrent.Size = new System.Drawing.Size(440, 56);
            this.lbCurrent.TabIndex = 247;
            this.lbCurrent.Text = "Documents";
            // 
            // btnNewFolder
            // 
            this.btnNewFolder.AutoSize = true;
            this.btnNewFolder.Location = new System.Drawing.Point(54, 269);
            this.btnNewFolder.Name = "btnNewFolder";
            this.btnNewFolder.Size = new System.Drawing.Size(157, 42);
            this.btnNewFolder.TabIndex = 248;
            this.btnNewFolder.Text = "New location:";
            this.btnNewFolder.UseVisualStyleBackColor = true;
            this.btnNewFolder.Click += new System.EventHandler(this.btnNewFolder_Click);
            // 
            // frmMenuFolders
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 678);
            this.Controls.Add(this.btnNewFolder);
            this.Controls.Add(this.lbCurrent);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbNewLocation);
            this.Controls.Add(this.ckCopyData);
            this.Controls.Add(this.ckOverwrite);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "frmMenuFolders";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuFolders";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMenuFolders_FormClosing);
            this.Load += new System.EventHandler(this.frmMenuFolders_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox ckOverwrite;
        private System.Windows.Forms.CheckBox ckCopyData;
        private System.Windows.Forms.TextBox tbNewLocation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbCurrent;
        private System.Windows.Forms.Button btnNewFolder;
    }
}