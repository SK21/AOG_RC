namespace RateController.Menu
{
    partial class frmMenuProfiles
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
            this.lbProfile = new System.Windows.Forms.Label();
            this.lstProfiles = new System.Windows.Forms.ListBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.tbName = new System.Windows.Forms.TextBox();
            this.btnProfilesUp = new System.Windows.Forms.Button();
            this.btnProfilesDown = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbProfile
            // 
            this.lbProfile.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbProfile.Location = new System.Drawing.Point(12, 9);
            this.lbProfile.Name = "lbProfile";
            this.lbProfile.Size = new System.Drawing.Size(516, 23);
            this.lbProfile.TabIndex = 149;
            this.lbProfile.Text = "Current Profile:  ";
            this.lbProfile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lstProfiles
            // 
            this.lstProfiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstProfiles.FormattingEnabled = true;
            this.lstProfiles.ItemHeight = 31;
            this.lstProfiles.Location = new System.Drawing.Point(132, 116);
            this.lstProfiles.Name = "lstProfiles";
            this.lstProfiles.ScrollAlwaysVisible = true;
            this.lstProfiles.Size = new System.Drawing.Size(349, 469);
            this.lstProfiles.Sorted = true;
            this.lstProfiles.TabIndex = 150;
            // 
            // btnDelete
            // 
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Image = global::RateController.Properties.Resources.Trash;
            this.btnDelete.Location = new System.Drawing.Point(12, 207);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(82, 64);
            this.btnDelete.TabIndex = 358;
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.FlatAppearance.BorderSize = 0;
            this.btnLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoad.Image = global::RateController.Properties.Resources.VehFileLoad;
            this.btnLoad.Location = new System.Drawing.Point(12, 121);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(82, 60);
            this.btnLoad.TabIndex = 359;
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.FlatAppearance.BorderSize = 0;
            this.btnCopy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCopy.Image = global::RateController.Properties.Resources.copy1;
            this.btnCopy.Location = new System.Drawing.Point(100, 35);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(82, 60);
            this.btnCopy.TabIndex = 360;
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnNew
            // 
            this.btnNew.FlatAppearance.BorderSize = 0;
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Image = global::RateController.Properties.Resources.NewFile;
            this.btnNew.Location = new System.Drawing.Point(12, 35);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(82, 60);
            this.btnNew.TabIndex = 361;
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(192, 50);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(289, 29);
            this.tbName.TabIndex = 362;
            this.tbName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnProfilesUp
            // 
            this.btnProfilesUp.FlatAppearance.BorderSize = 0;
            this.btnProfilesUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProfilesUp.Image = global::RateController.Properties.Resources.arrow_up;
            this.btnProfilesUp.Location = new System.Drawing.Point(487, 265);
            this.btnProfilesUp.Name = "btnProfilesUp";
            this.btnProfilesUp.Size = new System.Drawing.Size(41, 41);
            this.btnProfilesUp.TabIndex = 386;
            this.btnProfilesUp.UseVisualStyleBackColor = true;
            this.btnProfilesUp.Click += new System.EventHandler(this.btnProfilesUp_Click);
            // 
            // btnProfilesDown
            // 
            this.btnProfilesDown.FlatAppearance.BorderSize = 0;
            this.btnProfilesDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProfilesDown.Image = global::RateController.Properties.Resources.arrow_down;
            this.btnProfilesDown.Location = new System.Drawing.Point(487, 342);
            this.btnProfilesDown.Name = "btnProfilesDown";
            this.btnProfilesDown.Size = new System.Drawing.Size(41, 41);
            this.btnProfilesDown.TabIndex = 387;
            this.btnProfilesDown.UseVisualStyleBackColor = true;
            this.btnProfilesDown.Click += new System.EventHandler(this.btnProfilesDown_Click);
            // 
            // frmMenuProfiles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 630);
            this.Controls.Add(this.btnProfilesDown);
            this.Controls.Add(this.btnProfilesUp);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.lstProfiles);
            this.Controls.Add(this.lbProfile);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "frmMenuProfiles";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuProfiles";
            this.Load += new System.EventHandler(this.frmMenuProfiles_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbProfile;
        private System.Windows.Forms.ListBox lstProfiles;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Button btnProfilesUp;
        private System.Windows.Forms.Button btnProfilesDown;
    }
}