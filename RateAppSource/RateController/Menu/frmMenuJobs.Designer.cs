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
            this.btnNew = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.tbNotes = new System.Windows.Forms.TextBox();
            this.lb1 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lbFieldName = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.tbDate = new System.Windows.Forms.TextBox();
            this.btnCalender = new System.Windows.Forms.Button();
            this.btnJobsUp = new System.Windows.Forms.Button();
            this.btnJobsDown = new System.Windows.Forms.Button();
            this.btnNotesDown = new System.Windows.Forms.Button();
            this.btnNotesUp = new System.Windows.Forms.Button();
            this.gbJobs = new System.Windows.Forms.GroupBox();
            this.lvJobs = new System.Windows.Forms.ListView();
            this.HdrName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.HdrDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cbSearchField = new System.Windows.Forms.ComboBox();
            this.btnDeleteField = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tbSearchYear = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.gbCurrentJob = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.cbField = new System.Windows.Forms.ComboBox();
            this.ckResume = new System.Windows.Forms.CheckBox();
            this.btnResetField = new System.Windows.Forms.Button();
            this.gbJobs.SuspendLayout();
            this.gbCurrentJob.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnNew
            // 
            this.btnNew.FlatAppearance.BorderSize = 0;
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Image = global::RateController.Properties.Resources.NewFile;
            this.btnNew.Location = new System.Drawing.Point(6, 157);
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
            this.btnCopy.Location = new System.Drawing.Point(6, 224);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(82, 64);
            this.btnCopy.TabIndex = 367;
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.FlatAppearance.BorderSize = 0;
            this.btnLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoad.Image = global::RateController.Properties.Resources.VehFileLoad;
            this.btnLoad.Location = new System.Drawing.Point(6, 89);
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
            this.btnDelete.Location = new System.Drawing.Point(6, 292);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(82, 64);
            this.btnDelete.TabIndex = 365;
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // tbNotes
            // 
            this.tbNotes.Location = new System.Drawing.Point(94, 133);
            this.tbNotes.MaxLength = 800;
            this.tbNotes.Multiline = true;
            this.tbNotes.Name = "tbNotes";
            this.tbNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbNotes.Size = new System.Drawing.Size(258, 78);
            this.tbNotes.TabIndex = 5;
            this.tbNotes.TextChanged += new System.EventHandler(this.ckJobs_CheckedChanged);
            // 
            // lb1
            // 
            this.lb1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb1.Location = new System.Drawing.Point(7, 133);
            this.lb1.Name = "lb1";
            this.lb1.Size = new System.Drawing.Size(70, 23);
            this.lb1.TabIndex = 371;
            this.lb1.Text = "Notes:";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 29);
            this.label1.TabIndex = 372;
            this.label1.Text = "Date:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbFieldName
            // 
            this.lbFieldName.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbFieldName.Location = new System.Drawing.Point(7, 97);
            this.lbFieldName.Name = "lbFieldName";
            this.lbFieldName.Size = new System.Drawing.Size(70, 29);
            this.lbFieldName.TabIndex = 374;
            this.lbFieldName.Text = "Field:";
            this.lbFieldName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.btnCancel.Location = new System.Drawing.Point(450, 79);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(70, 63);
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
            this.btnOK.Location = new System.Drawing.Point(450, 148);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(70, 63);
            this.btnOK.TabIndex = 6;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // tbDate
            // 
            this.tbDate.Location = new System.Drawing.Point(94, 60);
            this.tbDate.Name = "tbDate";
            this.tbDate.Size = new System.Drawing.Size(258, 29);
            this.tbDate.TabIndex = 3;
            this.tbDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbDate.TextChanged += new System.EventHandler(this.ckJobs_CheckedChanged);
            // 
            // btnCalender
            // 
            this.btnCalender.FlatAppearance.BorderSize = 0;
            this.btnCalender.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCalender.Image = global::RateController.Properties.Resources.calendar___large;
            this.btnCalender.Location = new System.Drawing.Point(358, 43);
            this.btnCalender.Name = "btnCalender";
            this.btnCalender.Size = new System.Drawing.Size(82, 64);
            this.btnCalender.TabIndex = 383;
            this.btnCalender.UseVisualStyleBackColor = true;
            this.btnCalender.Click += new System.EventHandler(this.btnCalender_Click);
            // 
            // btnJobsUp
            // 
            this.btnJobsUp.FlatAppearance.BorderSize = 0;
            this.btnJobsUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJobsUp.Image = global::RateController.Properties.Resources.arrow_up;
            this.btnJobsUp.Location = new System.Drawing.Point(479, 95);
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
            this.btnJobsDown.Location = new System.Drawing.Point(479, 298);
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
            this.btnNotesDown.Location = new System.Drawing.Point(379, 170);
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
            this.btnNotesUp.Location = new System.Drawing.Point(379, 115);
            this.btnNotesUp.Name = "btnNotesUp";
            this.btnNotesUp.Size = new System.Drawing.Size(41, 41);
            this.btnNotesUp.TabIndex = 388;
            this.btnNotesUp.UseVisualStyleBackColor = true;
            this.btnNotesUp.Click += new System.EventHandler(this.btnNotesUp_Click);
            // 
            // gbJobs
            // 
            this.gbJobs.Controls.Add(this.btnResetField);
            this.gbJobs.Controls.Add(this.lvJobs);
            this.gbJobs.Controls.Add(this.cbSearchField);
            this.gbJobs.Controls.Add(this.btnDeleteField);
            this.gbJobs.Controls.Add(this.label2);
            this.gbJobs.Controls.Add(this.btnNew);
            this.gbJobs.Controls.Add(this.btnJobsDown);
            this.gbJobs.Controls.Add(this.tbSearchYear);
            this.gbJobs.Controls.Add(this.label4);
            this.gbJobs.Controls.Add(this.btnJobsUp);
            this.gbJobs.Controls.Add(this.btnLoad);
            this.gbJobs.Controls.Add(this.btnCopy);
            this.gbJobs.Controls.Add(this.btnDelete);
            this.gbJobs.Location = new System.Drawing.Point(6, 226);
            this.gbJobs.Name = "gbJobs";
            this.gbJobs.Size = new System.Drawing.Size(528, 362);
            this.gbJobs.TabIndex = 390;
            this.gbJobs.TabStop = false;
            this.gbJobs.Text = "Jobs";
            this.gbJobs.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox1_Paint);
            // 
            // lvJobs
            // 
            this.lvJobs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.HdrName,
            this.HdrDate});
            this.lvJobs.FullRowSelect = true;
            this.lvJobs.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvJobs.HideSelection = false;
            this.lvJobs.Location = new System.Drawing.Point(94, 95);
            this.lvJobs.Name = "lvJobs";
            this.lvJobs.Size = new System.Drawing.Size(379, 244);
            this.lvJobs.TabIndex = 392;
            this.lvJobs.UseCompatibleStateImageBehavior = false;
            this.lvJobs.View = System.Windows.Forms.View.Details;
            // 
            // HdrName
            // 
            this.HdrName.Text = "Name";
            this.HdrName.Width = 275;
            // 
            // HdrDate
            // 
            this.HdrDate.Text = "Date";
            this.HdrDate.Width = 300;
            // 
            // cbSearchField
            // 
            this.cbSearchField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSearchField.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbSearchField.FormattingEnabled = true;
            this.cbSearchField.Location = new System.Drawing.Point(94, 52);
            this.cbSearchField.MaxDropDownItems = 12;
            this.cbSearchField.MaxLength = 20;
            this.cbSearchField.Name = "cbSearchField";
            this.cbSearchField.Size = new System.Drawing.Size(244, 32);
            this.cbSearchField.TabIndex = 385;
            this.cbSearchField.SelectedIndexChanged += new System.EventHandler(this.cbSearchField_SelectedIndexChanged);
            // 
            // btnDeleteField
            // 
            this.btnDeleteField.FlatAppearance.BorderSize = 0;
            this.btnDeleteField.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteField.Image = global::RateController.Properties.Resources.Trash;
            this.btnDeleteField.Location = new System.Drawing.Point(434, 20);
            this.btnDeleteField.Name = "btnDeleteField";
            this.btnDeleteField.Size = new System.Drawing.Size(82, 64);
            this.btnDeleteField.TabIndex = 389;
            this.btnDeleteField.UseVisualStyleBackColor = true;
            this.btnDeleteField.Click += new System.EventHandler(this.btnDeleteField_Click);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(90, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 29);
            this.label2.TabIndex = 384;
            this.label2.Text = "Field:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbSearchYear
            // 
            this.tbSearchYear.Location = new System.Drawing.Point(10, 55);
            this.tbSearchYear.Name = "tbSearchYear";
            this.tbSearchYear.Size = new System.Drawing.Size(63, 29);
            this.tbSearchYear.TabIndex = 386;
            this.tbSearchYear.Text = "2025";
            this.tbSearchYear.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSearchYear.Click += new System.EventHandler(this.tbSearchYear_Enter);
            this.tbSearchYear.TextChanged += new System.EventHandler(this.tbSearchYear_TextChanged);
            this.tbSearchYear.Enter += new System.EventHandler(this.tbSearchYear_Enter);
            this.tbSearchYear.Validating += new System.ComponentModel.CancelEventHandler(this.tbSearchYear_Validating);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(10, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 29);
            this.label4.TabIndex = 383;
            this.label4.Text = "Year:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gbCurrentJob
            // 
            this.gbCurrentJob.Controls.Add(this.label5);
            this.gbCurrentJob.Controls.Add(this.tbName);
            this.gbCurrentJob.Controls.Add(this.btnOK);
            this.gbCurrentJob.Controls.Add(this.btnCancel);
            this.gbCurrentJob.Controls.Add(this.label1);
            this.gbCurrentJob.Controls.Add(this.tbDate);
            this.gbCurrentJob.Controls.Add(this.btnCalender);
            this.gbCurrentJob.Controls.Add(this.lbFieldName);
            this.gbCurrentJob.Controls.Add(this.btnNotesUp);
            this.gbCurrentJob.Controls.Add(this.btnNotesDown);
            this.gbCurrentJob.Controls.Add(this.cbField);
            this.gbCurrentJob.Controls.Add(this.lb1);
            this.gbCurrentJob.Controls.Add(this.tbNotes);
            this.gbCurrentJob.Location = new System.Drawing.Point(6, 0);
            this.gbCurrentJob.Name = "gbCurrentJob";
            this.gbCurrentJob.Size = new System.Drawing.Size(528, 220);
            this.gbCurrentJob.TabIndex = 391;
            this.gbCurrentJob.TabStop = false;
            this.gbCurrentJob.Text = "Current Job";
            this.gbCurrentJob.Paint += new System.Windows.Forms.PaintEventHandler(this.gbCurrentJob_Paint);
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(6, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 29);
            this.label5.TabIndex = 390;
            this.label5.Text = "Name:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(94, 25);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(258, 29);
            this.tbName.TabIndex = 2;
            this.tbName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbName.TextChanged += new System.EventHandler(this.ckJobs_CheckedChanged);
            // 
            // cbField
            // 
            this.cbField.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cbField.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbField.FormattingEnabled = true;
            this.cbField.Location = new System.Drawing.Point(94, 95);
            this.cbField.MaxDropDownItems = 12;
            this.cbField.MaxLength = 20;
            this.cbField.Name = "cbField";
            this.cbField.Size = new System.Drawing.Size(258, 32);
            this.cbField.TabIndex = 4;
            this.cbField.TextChanged += new System.EventHandler(this.ckJobs_CheckedChanged);
            // 
            // ckResume
            // 
            this.ckResume.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckResume.Checked = true;
            this.ckResume.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckResume.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckResume.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckResume.Location = new System.Drawing.Point(212, 602);
            this.ckResume.Name = "ckResume";
            this.ckResume.Size = new System.Drawing.Size(108, 64);
            this.ckResume.TabIndex = 392;
            this.ckResume.Text = "Show at Start-Up";
            this.ckResume.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckResume.UseVisualStyleBackColor = true;
            this.ckResume.CheckedChanged += new System.EventHandler(this.ckResume_CheckedChanged);
            // 
            // btnResetField
            // 
            this.btnResetField.BackColor = System.Drawing.Color.Transparent;
            this.btnResetField.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnResetField.FlatAppearance.BorderSize = 0;
            this.btnResetField.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResetField.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnResetField.Image = global::RateController.Properties.Resources.Update;
            this.btnResetField.Location = new System.Drawing.Point(358, 21);
            this.btnResetField.Name = "btnResetField";
            this.btnResetField.Size = new System.Drawing.Size(70, 63);
            this.btnResetField.TabIndex = 393;
            this.btnResetField.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnResetField.UseVisualStyleBackColor = false;
            this.btnResetField.Click += new System.EventHandler(this.btnResetField_Click);
            // 
            // frmMenuJobs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 678);
            this.Controls.Add(this.ckResume);
            this.Controls.Add(this.gbCurrentJob);
            this.Controls.Add(this.gbJobs);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "frmMenuJobs";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuJobs";
            this.Load += new System.EventHandler(this.frmMenuJobs_Load);
            this.gbJobs.ResumeLayout(false);
            this.gbJobs.PerformLayout();
            this.gbCurrentJob.ResumeLayout(false);
            this.gbCurrentJob.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.TextBox tbNotes;
        private System.Windows.Forms.Label lb1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbFieldName;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox tbDate;
        private System.Windows.Forms.Button btnCalender;
        private System.Windows.Forms.Button btnJobsUp;
        private System.Windows.Forms.Button btnJobsDown;
        private System.Windows.Forms.Button btnNotesDown;
        private System.Windows.Forms.Button btnNotesUp;
        private System.Windows.Forms.GroupBox gbJobs;
        private System.Windows.Forms.ComboBox cbSearchField;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbSearchYear;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox gbCurrentJob;
        private System.Windows.Forms.Button btnDeleteField;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.ComboBox cbField;
        private System.Windows.Forms.ListView lvJobs;
        private System.Windows.Forms.ColumnHeader HdrName;
        private System.Windows.Forms.ColumnHeader HdrDate;
        private System.Windows.Forms.CheckBox ckResume;
        private System.Windows.Forms.Button btnResetField;
    }
}