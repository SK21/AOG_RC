namespace RateController.Menu
{
    partial class frmMenuJobs
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
            this.tbName = new System.Windows.Forms.TextBox();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lstJobs = new System.Windows.Forms.ListBox();
            this.lbJob = new System.Windows.Forms.Label();
            this.tbNotes = new System.Windows.Forms.TextBox();
            this.lb1 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lbFieldName = new System.Windows.Forms.Label();
            this.cbField = new System.Windows.Forms.ComboBox();
            this.ckJobs = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnDeleteField = new System.Windows.Forms.Button();
            this.tbDate = new System.Windows.Forms.TextBox();
            this.btnCalender = new System.Windows.Forms.Button();
            this.btnJobsUp = new System.Windows.Forms.Button();
            this.btnJobsDown = new System.Windows.Forms.Button();
            this.btnNotesDown = new System.Windows.Forms.Button();
            this.btnNotesUp = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(199, 42);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(258, 29);
            this.tbName.TabIndex = 369;
            this.tbName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnNew
            // 
            this.btnNew.FlatAppearance.BorderSize = 0;
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Image = global::RateController.Properties.Resources.NewFile;
            this.btnNew.Location = new System.Drawing.Point(7, 24);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(82, 64);
            this.btnNew.TabIndex = 368;
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.FlatAppearance.BorderSize = 0;
            this.btnCopy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCopy.Image = global::RateController.Properties.Resources.copy1;
            this.btnCopy.Location = new System.Drawing.Point(103, 26);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(82, 60);
            this.btnCopy.TabIndex = 367;
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.FlatAppearance.BorderSize = 0;
            this.btnLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoad.Image = global::RateController.Properties.Resources.VehFileLoad;
            this.btnLoad.Location = new System.Drawing.Point(7, 180);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(82, 64);
            this.btnLoad.TabIndex = 366;
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Image = global::RateController.Properties.Resources.Trash;
            this.btnDelete.Location = new System.Drawing.Point(7, 275);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(82, 64);
            this.btnDelete.TabIndex = 365;
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lstJobs
            // 
            this.lstJobs.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstJobs.FormattingEnabled = true;
            this.lstJobs.ItemHeight = 31;
            this.lstJobs.Location = new System.Drawing.Point(107, 180);
            this.lstJobs.Name = "lstJobs";
            this.lstJobs.ScrollAlwaysVisible = true;
            this.lstJobs.Size = new System.Drawing.Size(349, 159);
            this.lstJobs.Sorted = true;
            this.lstJobs.TabIndex = 364;
            // 
            // lbJob
            // 
            this.lbJob.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbJob.Location = new System.Drawing.Point(82, 26);
            this.lbJob.Name = "lbJob";
            this.lbJob.Size = new System.Drawing.Size(299, 23);
            this.lbJob.TabIndex = 363;
            this.lbJob.Text = "JobName";
            this.lbJob.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbNotes
            // 
            this.tbNotes.Location = new System.Drawing.Point(82, 122);
            this.tbNotes.MaxLength = 800;
            this.tbNotes.Multiline = true;
            this.tbNotes.Name = "tbNotes";
            this.tbNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbNotes.Size = new System.Drawing.Size(279, 116);
            this.tbNotes.TabIndex = 370;
            this.tbNotes.TextChanged += new System.EventHandler(this.ckJobs_CheckedChanged);
            // 
            // lb1
            // 
            this.lb1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb1.Location = new System.Drawing.Point(6, 122);
            this.lb1.Name = "lb1";
            this.lb1.Size = new System.Drawing.Size(70, 23);
            this.lb1.TabIndex = 371;
            this.lb1.Text = "Notes:";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(107, 98);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 29);
            this.label1.TabIndex = 372;
            this.label1.Text = "Date:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbFieldName
            // 
            this.lbFieldName.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbFieldName.Location = new System.Drawing.Point(107, 139);
            this.lbFieldName.Name = "lbFieldName";
            this.lbFieldName.Size = new System.Drawing.Size(70, 29);
            this.lbFieldName.TabIndex = 374;
            this.lbFieldName.Text = "Field:";
            this.lbFieldName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbField
            // 
            this.cbField.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cbField.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbField.FormattingEnabled = true;
            this.cbField.Location = new System.Drawing.Point(198, 133);
            this.cbField.MaxDropDownItems = 12;
            this.cbField.MaxLength = 20;
            this.cbField.Name = "cbField";
            this.cbField.Size = new System.Drawing.Size(258, 41);
            this.cbField.Sorted = true;
            this.cbField.TabIndex = 375;
            this.cbField.TextChanged += new System.EventHandler(this.ckJobs_CheckedChanged);
            this.cbField.Resize += new System.EventHandler(this.cbField_Resize);
            // 
            // ckJobs
            // 
            this.ckJobs.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckJobs.Checked = true;
            this.ckJobs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckJobs.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckJobs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckJobs.Location = new System.Drawing.Point(417, 18);
            this.ckJobs.Name = "ckJobs";
            this.ckJobs.Size = new System.Drawing.Size(89, 64);
            this.ckJobs.TabIndex = 376;
            this.ckJobs.Text = "Use Jobs";
            this.ckJobs.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckJobs.UseVisualStyleBackColor = true;
            this.ckJobs.CheckedChanged += new System.EventHandler(this.ckJobs_CheckedChanged);
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
            this.btnCancel.Location = new System.Drawing.Point(434, 88);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 72);
            this.btnCancel.TabIndex = 379;
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
            this.btnOK.Location = new System.Drawing.Point(434, 166);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(72, 72);
            this.btnOK.TabIndex = 378;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnDeleteField
            // 
            this.btnDeleteField.FlatAppearance.BorderSize = 0;
            this.btnDeleteField.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteField.Image = global::RateController.Properties.Resources.trash_small;
            this.btnDeleteField.Location = new System.Drawing.Point(462, 133);
            this.btnDeleteField.Name = "btnDeleteField";
            this.btnDeleteField.Size = new System.Drawing.Size(41, 41);
            this.btnDeleteField.TabIndex = 381;
            this.btnDeleteField.UseVisualStyleBackColor = true;
            this.btnDeleteField.Click += new System.EventHandler(this.btnDeleteField_Click);
            // 
            // tbDate
            // 
            this.tbDate.Location = new System.Drawing.Point(198, 98);
            this.tbDate.Name = "tbDate";
            this.tbDate.Size = new System.Drawing.Size(258, 29);
            this.tbDate.TabIndex = 382;
            this.tbDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbDate.TextChanged += new System.EventHandler(this.ckJobs_CheckedChanged);
            // 
            // btnCalender
            // 
            this.btnCalender.FlatAppearance.BorderSize = 0;
            this.btnCalender.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCalender.Image = global::RateController.Properties.Resources.calendar_blank;
            this.btnCalender.Location = new System.Drawing.Point(462, 92);
            this.btnCalender.Name = "btnCalender";
            this.btnCalender.Size = new System.Drawing.Size(41, 41);
            this.btnCalender.TabIndex = 383;
            this.btnCalender.UseVisualStyleBackColor = true;
            this.btnCalender.Click += new System.EventHandler(this.btnCalender_Click);
            // 
            // btnJobsUp
            // 
            this.btnJobsUp.FlatAppearance.BorderSize = 0;
            this.btnJobsUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJobsUp.Image = global::RateController.Properties.Resources.arrow_up;
            this.btnJobsUp.Location = new System.Drawing.Point(462, 198);
            this.btnJobsUp.Name = "btnJobsUp";
            this.btnJobsUp.Size = new System.Drawing.Size(41, 41);
            this.btnJobsUp.TabIndex = 385;
            this.btnJobsUp.UseVisualStyleBackColor = true;
            this.btnJobsUp.Click += new System.EventHandler(this.btnJobsUp_Click);
            // 
            // btnJobsDown
            // 
            this.btnJobsDown.FlatAppearance.BorderSize = 0;
            this.btnJobsDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJobsDown.Image = global::RateController.Properties.Resources.arrow_down;
            this.btnJobsDown.Location = new System.Drawing.Point(462, 281);
            this.btnJobsDown.Name = "btnJobsDown";
            this.btnJobsDown.Size = new System.Drawing.Size(41, 41);
            this.btnJobsDown.TabIndex = 386;
            this.btnJobsDown.UseVisualStyleBackColor = true;
            this.btnJobsDown.Click += new System.EventHandler(this.btnJobsDown_Click);
            // 
            // btnNotesDown
            // 
            this.btnNotesDown.FlatAppearance.BorderSize = 0;
            this.btnNotesDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNotesDown.Image = global::RateController.Properties.Resources.arrow_down;
            this.btnNotesDown.Location = new System.Drawing.Point(367, 198);
            this.btnNotesDown.Name = "btnNotesDown";
            this.btnNotesDown.Size = new System.Drawing.Size(41, 41);
            this.btnNotesDown.TabIndex = 387;
            this.btnNotesDown.UseVisualStyleBackColor = true;
            this.btnNotesDown.Click += new System.EventHandler(this.btnNotesDown_Click);
            // 
            // btnNotesUp
            // 
            this.btnNotesUp.FlatAppearance.BorderSize = 0;
            this.btnNotesUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNotesUp.Image = global::RateController.Properties.Resources.arrow_up;
            this.btnNotesUp.Location = new System.Drawing.Point(367, 122);
            this.btnNotesUp.Name = "btnNotesUp";
            this.btnNotesUp.Size = new System.Drawing.Size(41, 41);
            this.btnNotesUp.TabIndex = 388;
            this.btnNotesUp.UseVisualStyleBackColor = true;
            this.btnNotesUp.Click += new System.EventHandler(this.btnNotesUp_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbName);
            this.groupBox1.Controls.Add(this.btnJobsDown);
            this.groupBox1.Controls.Add(this.btnCopy);
            this.groupBox1.Controls.Add(this.btnJobsUp);
            this.groupBox1.Controls.Add(this.btnNew);
            this.groupBox1.Controls.Add(this.cbField);
            this.groupBox1.Controls.Add(this.btnDeleteField);
            this.groupBox1.Controls.Add(this.lbFieldName);
            this.groupBox1.Controls.Add(this.btnDelete);
            this.groupBox1.Controls.Add(this.btnLoad);
            this.groupBox1.Controls.Add(this.btnCalender);
            this.groupBox1.Controls.Add(this.tbDate);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.lstJobs);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(512, 355);
            this.groupBox1.TabIndex = 389;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "New Job";
            this.groupBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox1_Paint);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.lbJob);
            this.groupBox2.Controls.Add(this.ckJobs);
            this.groupBox2.Controls.Add(this.btnCancel);
            this.groupBox2.Controls.Add(this.btnNotesDown);
            this.groupBox2.Controls.Add(this.btnNotesUp);
            this.groupBox2.Controls.Add(this.btnOK);
            this.groupBox2.Controls.Add(this.tbNotes);
            this.groupBox2.Controls.Add(this.lb1);
            this.groupBox2.Location = new System.Drawing.Point(12, 373);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(512, 245);
            this.groupBox2.TabIndex = 390;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Current Job";
            this.groupBox2.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox1_Paint);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(6, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 23);
            this.label3.TabIndex = 387;
            this.label3.Text = "Name:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(6, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 23);
            this.label4.TabIndex = 390;
            this.label4.Text = "Date:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(82, 58);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(299, 23);
            this.label5.TabIndex = 389;
            this.label5.Text = "15-May-2025";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(6, 90);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(70, 23);
            this.label6.TabIndex = 392;
            this.label6.Text = "Field:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(82, 90);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(299, 23);
            this.label7.TabIndex = 391;
            this.label7.Text = "Home 1/4";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // frmMenuJobs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(530, 630);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "frmMenuJobs";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuJobs";
            this.Load += new System.EventHandler(this.frmMenuJobs_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ListBox lstJobs;
        private System.Windows.Forms.Label lbJob;
        private System.Windows.Forms.TextBox tbNotes;
        private System.Windows.Forms.Label lb1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbFieldName;
        private System.Windows.Forms.ComboBox cbField;
        private System.Windows.Forms.CheckBox ckJobs;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnDeleteField;
        private System.Windows.Forms.TextBox tbDate;
        private System.Windows.Forms.Button btnCalender;
        private System.Windows.Forms.Button btnJobsUp;
        private System.Windows.Forms.Button btnJobsDown;
        private System.Windows.Forms.Button btnNotesDown;
        private System.Windows.Forms.Button btnNotesUp;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}