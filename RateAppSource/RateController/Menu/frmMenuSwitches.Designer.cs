namespace RateController.Menu
{
    partial class frmMenuSwitches
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
            this.ckWorkSwitch = new System.Windows.Forms.CheckBox();
            this.gbAutoSwitch = new System.Windows.Forms.GroupBox();
            this.ckRate = new System.Windows.Forms.CheckBox();
            this.ckSections = new System.Windows.Forms.CheckBox();
            this.gbMasterMode = new System.Windows.Forms.GroupBox();
            this.rbMasterOverride = new System.Windows.Forms.RadioButton();
            this.rbMasterAll = new System.Windows.Forms.RadioButton();
            this.gbMasterType = new System.Windows.Forms.GroupBox();
            this.rbMaintained = new System.Windows.Forms.RadioButton();
            this.rbMomentary = new System.Windows.Forms.RadioButton();
            this.ckOnScreen = new System.Windows.Forms.CheckBox();
            this.gbAutoSwitch.SuspendLayout();
            this.gbMasterMode.SuspendLayout();
            this.gbMasterType.SuspendLayout();
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
            this.btnCancel.Location = new System.Drawing.Point(380, 603);
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
            this.btnOK.Location = new System.Drawing.Point(458, 603);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(70, 63);
            this.btnOK.TabIndex = 163;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // ckWorkSwitch
            // 
            this.ckWorkSwitch.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckWorkSwitch.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckWorkSwitch.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckWorkSwitch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckWorkSwitch.Location = new System.Drawing.Point(309, 326);
            this.ckWorkSwitch.Name = "ckWorkSwitch";
            this.ckWorkSwitch.Size = new System.Drawing.Size(170, 37);
            this.ckWorkSwitch.TabIndex = 338;
            this.ckWorkSwitch.Text = "Work Switch";
            this.ckWorkSwitch.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckWorkSwitch.UseVisualStyleBackColor = true;
            this.ckWorkSwitch.CheckedChanged += new System.EventHandler(this.ckDualAuto_CheckedChanged);
            // 
            // gbAutoSwitch
            // 
            this.gbAutoSwitch.Controls.Add(this.ckRate);
            this.gbAutoSwitch.Controls.Add(this.ckSections);
            this.gbAutoSwitch.Location = new System.Drawing.Point(293, 127);
            this.gbAutoSwitch.Name = "gbAutoSwitch";
            this.gbAutoSwitch.Size = new System.Drawing.Size(200, 151);
            this.gbAutoSwitch.TabIndex = 340;
            this.gbAutoSwitch.TabStop = false;
            this.gbAutoSwitch.Text = "Auto Switch";
            this.gbAutoSwitch.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox2_Paint);
            // 
            // ckRate
            // 
            this.ckRate.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckRate.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckRate.Checked = true;
            this.ckRate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckRate.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckRate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckRate.Location = new System.Drawing.Point(16, 28);
            this.ckRate.Name = "ckRate";
            this.ckRate.Size = new System.Drawing.Size(170, 37);
            this.ckRate.TabIndex = 343;
            this.ckRate.Text = "Rate";
            this.ckRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckRate.UseVisualStyleBackColor = true;
            this.ckRate.CheckedChanged += new System.EventHandler(this.ckDualAuto_CheckedChanged);
            // 
            // ckSections
            // 
            this.ckSections.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckSections.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckSections.Checked = true;
            this.ckSections.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckSections.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckSections.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckSections.Location = new System.Drawing.Point(16, 97);
            this.ckSections.Name = "ckSections";
            this.ckSections.Size = new System.Drawing.Size(170, 37);
            this.ckSections.TabIndex = 344;
            this.ckSections.Text = "Sections";
            this.ckSections.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckSections.UseVisualStyleBackColor = true;
            this.ckSections.CheckedChanged += new System.EventHandler(this.ckDualAuto_CheckedChanged);
            // 
            // gbMasterMode
            // 
            this.gbMasterMode.Controls.Add(this.rbMasterOverride);
            this.gbMasterMode.Controls.Add(this.rbMasterAll);
            this.gbMasterMode.Location = new System.Drawing.Point(39, 313);
            this.gbMasterMode.Name = "gbMasterMode";
            this.gbMasterMode.Size = new System.Drawing.Size(200, 157);
            this.gbMasterMode.TabIndex = 342;
            this.gbMasterMode.TabStop = false;
            this.gbMasterMode.Text = "Master Switch Mode";
            this.gbMasterMode.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox2_Paint);
            // 
            // rbMasterOverride
            // 
            this.rbMasterOverride.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbMasterOverride.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbMasterOverride.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbMasterOverride.Location = new System.Drawing.Point(16, 97);
            this.rbMasterOverride.Name = "rbMasterOverride";
            this.rbMasterOverride.Size = new System.Drawing.Size(170, 37);
            this.rbMasterOverride.TabIndex = 4;
            this.rbMasterOverride.Text = "Master Override";
            this.rbMasterOverride.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbMasterOverride.UseVisualStyleBackColor = true;
            this.rbMasterOverride.Click += new System.EventHandler(this.ckDualAuto_CheckedChanged);
            // 
            // rbMasterAll
            // 
            this.rbMasterAll.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbMasterAll.Checked = true;
            this.rbMasterAll.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbMasterAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbMasterAll.Location = new System.Drawing.Point(16, 28);
            this.rbMasterAll.Name = "rbMasterAll";
            this.rbMasterAll.Size = new System.Drawing.Size(170, 37);
            this.rbMasterAll.TabIndex = 1;
            this.rbMasterAll.TabStop = true;
            this.rbMasterAll.Text = "Standard";
            this.rbMasterAll.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbMasterAll.UseVisualStyleBackColor = true;
            this.rbMasterAll.Click += new System.EventHandler(this.ckDualAuto_CheckedChanged);
            // 
            // gbMasterType
            // 
            this.gbMasterType.Controls.Add(this.rbMaintained);
            this.gbMasterType.Controls.Add(this.rbMomentary);
            this.gbMasterType.Location = new System.Drawing.Point(39, 127);
            this.gbMasterType.Name = "gbMasterType";
            this.gbMasterType.Size = new System.Drawing.Size(200, 151);
            this.gbMasterType.TabIndex = 343;
            this.gbMasterType.TabStop = false;
            this.gbMasterType.Text = "Master Switch Type";
            this.gbMasterType.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox2_Paint);
            // 
            // rbMaintained
            // 
            this.rbMaintained.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbMaintained.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbMaintained.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbMaintained.Location = new System.Drawing.Point(16, 97);
            this.rbMaintained.Name = "rbMaintained";
            this.rbMaintained.Size = new System.Drawing.Size(170, 37);
            this.rbMaintained.TabIndex = 5;
            this.rbMaintained.Text = "Maintained";
            this.rbMaintained.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbMaintained.UseVisualStyleBackColor = true;
            this.rbMaintained.CheckedChanged += new System.EventHandler(this.ckDualAuto_CheckedChanged);
            // 
            // rbMomentary
            // 
            this.rbMomentary.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbMomentary.Checked = true;
            this.rbMomentary.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbMomentary.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbMomentary.Location = new System.Drawing.Point(16, 28);
            this.rbMomentary.Name = "rbMomentary";
            this.rbMomentary.Size = new System.Drawing.Size(170, 37);
            this.rbMomentary.TabIndex = 4;
            this.rbMomentary.TabStop = true;
            this.rbMomentary.Text = "Momentary";
            this.rbMomentary.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbMomentary.UseVisualStyleBackColor = true;
            this.rbMomentary.CheckedChanged += new System.EventHandler(this.ckDualAuto_CheckedChanged);
            // 
            // ckOnScreen
            // 
            this.ckOnScreen.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckOnScreen.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckOnScreen.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckOnScreen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckOnScreen.Location = new System.Drawing.Point(309, 403);
            this.ckOnScreen.Name = "ckOnScreen";
            this.ckOnScreen.Size = new System.Drawing.Size(170, 67);
            this.ckOnScreen.TabIndex = 344;
            this.ckOnScreen.Text = "On-Screen Switches";
            this.ckOnScreen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckOnScreen.UseVisualStyleBackColor = true;
            this.ckOnScreen.CheckedChanged += new System.EventHandler(this.ckDualAuto_CheckedChanged);
            // 
            // frmMenuSwitches
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 678);
            this.Controls.Add(this.ckOnScreen);
            this.Controls.Add(this.gbMasterType);
            this.Controls.Add(this.gbMasterMode);
            this.Controls.Add(this.gbAutoSwitch);
            this.Controls.Add(this.ckWorkSwitch);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "frmMenuSwitches";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuSwitches";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMenuSwitches_FormClosed);
            this.Load += new System.EventHandler(this.frmMenuSwitches_Load);
            this.gbAutoSwitch.ResumeLayout(false);
            this.gbMasterMode.ResumeLayout(false);
            this.gbMasterType.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox ckWorkSwitch;
        private System.Windows.Forms.GroupBox gbAutoSwitch;
        private System.Windows.Forms.GroupBox gbMasterMode;
        private System.Windows.Forms.RadioButton rbMasterAll;
        private System.Windows.Forms.RadioButton rbMasterOverride;
        private System.Windows.Forms.CheckBox ckRate;
        private System.Windows.Forms.CheckBox ckSections;
        private System.Windows.Forms.GroupBox gbMasterType;
        private System.Windows.Forms.RadioButton rbMaintained;
        private System.Windows.Forms.RadioButton rbMomentary;
        private System.Windows.Forms.CheckBox ckOnScreen;
    }
}