namespace RateController.Forms
{
    partial class frmCopyMap
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
            this.cbSearchField = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnJobsDown = new System.Windows.Forms.Button();
            this.tbSearchYear = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnJobsUp = new System.Windows.Forms.Button();
            this.lstJobs = new System.Windows.Forms.ListBox();
            this.btnRefeshJobs = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.ckErase = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cbSearchField
            // 
            this.cbSearchField.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cbSearchField.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbSearchField.FormattingEnabled = true;
            this.cbSearchField.Location = new System.Drawing.Point(96, 38);
            this.cbSearchField.MaxDropDownItems = 12;
            this.cbSearchField.MaxLength = 20;
            this.cbSearchField.Name = "cbSearchField";
            this.cbSearchField.Size = new System.Drawing.Size(244, 32);
            this.cbSearchField.Sorted = true;
            this.cbSearchField.TabIndex = 395;
            this.cbSearchField.SelectedIndexChanged += new System.EventHandler(this.cbSearchField_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(92, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 29);
            this.label2.TabIndex = 394;
            this.label2.Text = "Field:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnJobsDown
            // 
            this.btnJobsDown.FlatAppearance.BorderSize = 0;
            this.btnJobsDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJobsDown.Image = global::RateController.Properties.Resources.arrow_down;
            this.btnJobsDown.Location = new System.Drawing.Point(397, 261);
            this.btnJobsDown.Name = "btnJobsDown";
            this.btnJobsDown.Size = new System.Drawing.Size(41, 41);
            this.btnJobsDown.TabIndex = 397;
            this.btnJobsDown.UseVisualStyleBackColor = true;
            this.btnJobsDown.Click += new System.EventHandler(this.btnJobsDown_Click);
            // 
            // tbSearchYear
            // 
            this.tbSearchYear.Location = new System.Drawing.Point(12, 40);
            this.tbSearchYear.Name = "tbSearchYear";
            this.tbSearchYear.Size = new System.Drawing.Size(63, 29);
            this.tbSearchYear.TabIndex = 398;
            this.tbSearchYear.Text = "2025";
            this.tbSearchYear.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 29);
            this.label4.TabIndex = 393;
            this.label4.Text = "Year:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnJobsUp
            // 
            this.btnJobsUp.FlatAppearance.BorderSize = 0;
            this.btnJobsUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJobsUp.Image = global::RateController.Properties.Resources.arrow_up;
            this.btnJobsUp.Location = new System.Drawing.Point(397, 82);
            this.btnJobsUp.Name = "btnJobsUp";
            this.btnJobsUp.Size = new System.Drawing.Size(41, 41);
            this.btnJobsUp.TabIndex = 396;
            this.btnJobsUp.UseVisualStyleBackColor = true;
            this.btnJobsUp.Click += new System.EventHandler(this.btnJobsUp_Click);
            // 
            // lstJobs
            // 
            this.lstJobs.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstJobs.FormattingEnabled = true;
            this.lstJobs.ItemHeight = 31;
            this.lstJobs.Location = new System.Drawing.Point(12, 82);
            this.lstJobs.Name = "lstJobs";
            this.lstJobs.ScrollAlwaysVisible = true;
            this.lstJobs.Size = new System.Drawing.Size(379, 221);
            this.lstJobs.TabIndex = 390;
            // 
            // btnRefeshJobs
            // 
            this.btnRefeshJobs.FlatAppearance.BorderSize = 0;
            this.btnRefeshJobs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefeshJobs.Image = global::RateController.Properties.Resources.Update;
            this.btnRefeshJobs.Location = new System.Drawing.Point(346, 12);
            this.btnRefeshJobs.Name = "btnRefeshJobs";
            this.btnRefeshJobs.Size = new System.Drawing.Size(82, 64);
            this.btnRefeshJobs.TabIndex = 399;
            this.btnRefeshJobs.UseVisualStyleBackColor = true;
            this.btnRefeshJobs.Click += new System.EventHandler(this.btnRefeshJobs_Click);
            this.btnRefeshJobs.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRefeshJobs_MouseDown);
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
            this.btnCancel.Location = new System.Drawing.Point(261, 321);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(89, 64);
            this.btnCancel.TabIndex = 402;
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Image = global::RateController.Properties.Resources.Save;
            this.btnSave.Location = new System.Drawing.Point(356, 321);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(82, 64);
            this.btnSave.TabIndex = 401;
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // ckErase
            // 
            this.ckErase.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckErase.Checked = true;
            this.ckErase.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckErase.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckErase.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckErase.Location = new System.Drawing.Point(12, 318);
            this.ckErase.Name = "ckErase";
            this.ckErase.Size = new System.Drawing.Size(104, 64);
            this.ckErase.TabIndex = 403;
            this.ckErase.Text = "Erase Rate Data";
            this.ckErase.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckErase.UseVisualStyleBackColor = true;
            // 
            // frmCopyMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(452, 394);
            this.Controls.Add(this.ckErase);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.cbSearchField);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnJobsDown);
            this.Controls.Add(this.tbSearchYear);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnJobsUp);
            this.Controls.Add(this.lstJobs);
            this.Controls.Add(this.btnRefeshJobs);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCopyMap";
            this.Text = "Copy Map";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmCopyMap_FormClosed);
            this.Load += new System.EventHandler(this.frmCopyMap_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbSearchField;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnJobsDown;
        private System.Windows.Forms.TextBox tbSearchYear;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnJobsUp;
        private System.Windows.Forms.ListBox lstJobs;
        private System.Windows.Forms.Button btnRefeshJobs;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox ckErase;
    }
}