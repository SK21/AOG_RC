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
            this.ckDualAuto = new System.Windows.Forms.CheckBox();
            this.ckScreenSwitches = new System.Windows.Forms.CheckBox();
            this.ckWorkSwitch = new System.Windows.Forms.CheckBox();
            this.gbAutoSwitch = new System.Windows.Forms.GroupBox();
            this.rbSections = new System.Windows.Forms.RadioButton();
            this.rbAutoAll = new System.Windows.Forms.RadioButton();
            this.gbOnScreen = new System.Windows.Forms.GroupBox();
            this.gbMasterSwitch = new System.Windows.Forms.GroupBox();
            this.rbMasterOverride = new System.Windows.Forms.RadioButton();
            this.rbMasterRelayOnly = new System.Windows.Forms.RadioButton();
            this.rbMasterAll = new System.Windows.Forms.RadioButton();
            this.gbAutoSwitch.SuspendLayout();
            this.gbOnScreen.SuspendLayout();
            this.gbMasterSwitch.SuspendLayout();
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
            this.btnCancel.Location = new System.Drawing.Point(378, 546);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 72);
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
            this.btnOK.Location = new System.Drawing.Point(456, 546);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(72, 72);
            this.btnOK.TabIndex = 163;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // ckDualAuto
            // 
            this.ckDualAuto.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckDualAuto.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckDualAuto.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckDualAuto.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckDualAuto.Location = new System.Drawing.Point(16, 97);
            this.ckDualAuto.Name = "ckDualAuto";
            this.ckDualAuto.Size = new System.Drawing.Size(170, 37);
            this.ckDualAuto.TabIndex = 167;
            this.ckDualAuto.Text = "Dual Auto";
            this.ckDualAuto.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckDualAuto.UseVisualStyleBackColor = true;
            this.ckDualAuto.CheckedChanged += new System.EventHandler(this.ckDualAuto_CheckedChanged);
            // 
            // ckScreenSwitches
            // 
            this.ckScreenSwitches.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckScreenSwitches.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckScreenSwitches.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckScreenSwitches.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckScreenSwitches.Location = new System.Drawing.Point(16, 28);
            this.ckScreenSwitches.Name = "ckScreenSwitches";
            this.ckScreenSwitches.Size = new System.Drawing.Size(170, 37);
            this.ckScreenSwitches.TabIndex = 165;
            this.ckScreenSwitches.Text = "Use Switches";
            this.ckScreenSwitches.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckScreenSwitches.UseVisualStyleBackColor = true;
            this.ckScreenSwitches.CheckedChanged += new System.EventHandler(this.ckDualAuto_CheckedChanged);
            // 
            // ckWorkSwitch
            // 
            this.ckWorkSwitch.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckWorkSwitch.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckWorkSwitch.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckWorkSwitch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckWorkSwitch.Location = new System.Drawing.Point(53, 396);
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
            this.gbAutoSwitch.Controls.Add(this.rbSections);
            this.gbAutoSwitch.Controls.Add(this.rbAutoAll);
            this.gbAutoSwitch.Location = new System.Drawing.Point(300, 119);
            this.gbAutoSwitch.Name = "gbAutoSwitch";
            this.gbAutoSwitch.Size = new System.Drawing.Size(200, 151);
            this.gbAutoSwitch.TabIndex = 340;
            this.gbAutoSwitch.TabStop = false;
            this.gbAutoSwitch.Text = "Auto Switch";
            this.gbAutoSwitch.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox2_Paint);
            // 
            // rbSections
            // 
            this.rbSections.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbSections.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbSections.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbSections.Location = new System.Drawing.Point(16, 97);
            this.rbSections.Name = "rbSections";
            this.rbSections.Size = new System.Drawing.Size(170, 37);
            this.rbSections.TabIndex = 3;
            this.rbSections.Text = "Sections";
            this.rbSections.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbSections.UseVisualStyleBackColor = true;
            this.rbSections.Click += new System.EventHandler(this.ckDualAuto_CheckedChanged);
            // 
            // rbAutoAll
            // 
            this.rbAutoAll.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbAutoAll.Checked = true;
            this.rbAutoAll.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbAutoAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbAutoAll.Location = new System.Drawing.Point(16, 28);
            this.rbAutoAll.Name = "rbAutoAll";
            this.rbAutoAll.Size = new System.Drawing.Size(170, 37);
            this.rbAutoAll.TabIndex = 1;
            this.rbAutoAll.TabStop = true;
            this.rbAutoAll.Text = "Rate + Sections";
            this.rbAutoAll.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbAutoAll.UseVisualStyleBackColor = true;
            this.rbAutoAll.Click += new System.EventHandler(this.ckDualAuto_CheckedChanged);
            // 
            // gbOnScreen
            // 
            this.gbOnScreen.Controls.Add(this.ckScreenSwitches);
            this.gbOnScreen.Controls.Add(this.ckDualAuto);
            this.gbOnScreen.Location = new System.Drawing.Point(300, 299);
            this.gbOnScreen.Name = "gbOnScreen";
            this.gbOnScreen.Size = new System.Drawing.Size(200, 151);
            this.gbOnScreen.TabIndex = 341;
            this.gbOnScreen.TabStop = false;
            this.gbOnScreen.Text = "On-Screen";
            this.gbOnScreen.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox2_Paint);
            // 
            // gbMasterSwitch
            // 
            this.gbMasterSwitch.Controls.Add(this.rbMasterOverride);
            this.gbMasterSwitch.Controls.Add(this.rbMasterRelayOnly);
            this.gbMasterSwitch.Controls.Add(this.rbMasterAll);
            this.gbMasterSwitch.Location = new System.Drawing.Point(37, 119);
            this.gbMasterSwitch.Name = "gbMasterSwitch";
            this.gbMasterSwitch.Size = new System.Drawing.Size(200, 222);
            this.gbMasterSwitch.TabIndex = 342;
            this.gbMasterSwitch.TabStop = false;
            this.gbMasterSwitch.Text = "Master Switch";
            this.gbMasterSwitch.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox2_Paint);
            // 
            // rbMasterOverride
            // 
            this.rbMasterOverride.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbMasterOverride.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbMasterOverride.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbMasterOverride.Location = new System.Drawing.Point(16, 166);
            this.rbMasterOverride.Name = "rbMasterOverride";
            this.rbMasterOverride.Size = new System.Drawing.Size(170, 37);
            this.rbMasterOverride.TabIndex = 4;
            this.rbMasterOverride.Text = "Master Override";
            this.rbMasterOverride.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbMasterOverride.UseVisualStyleBackColor = true;
            this.rbMasterOverride.Click += new System.EventHandler(this.ckDualAuto_CheckedChanged);
            // 
            // rbMasterRelayOnly
            // 
            this.rbMasterRelayOnly.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbMasterRelayOnly.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbMasterRelayOnly.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbMasterRelayOnly.Location = new System.Drawing.Point(16, 97);
            this.rbMasterRelayOnly.Name = "rbMasterRelayOnly";
            this.rbMasterRelayOnly.Size = new System.Drawing.Size(170, 37);
            this.rbMasterRelayOnly.TabIndex = 3;
            this.rbMasterRelayOnly.Text = "Master Relay only";
            this.rbMasterRelayOnly.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbMasterRelayOnly.UseVisualStyleBackColor = true;
            this.rbMasterRelayOnly.Click += new System.EventHandler(this.ckDualAuto_CheckedChanged);
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
            this.rbMasterAll.Text = "Controll All";
            this.rbMasterAll.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbMasterAll.UseVisualStyleBackColor = true;
            this.rbMasterAll.Click += new System.EventHandler(this.ckDualAuto_CheckedChanged);
            // 
            // frmMenuSwitches
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 630);
            this.Controls.Add(this.gbMasterSwitch);
            this.Controls.Add(this.gbOnScreen);
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
            this.gbOnScreen.ResumeLayout(false);
            this.gbMasterSwitch.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox ckDualAuto;
        private System.Windows.Forms.CheckBox ckScreenSwitches;
        private System.Windows.Forms.CheckBox ckWorkSwitch;
        private System.Windows.Forms.GroupBox gbAutoSwitch;
        private System.Windows.Forms.RadioButton rbAutoAll;
        private System.Windows.Forms.RadioButton rbSections;
        private System.Windows.Forms.GroupBox gbOnScreen;
        private System.Windows.Forms.GroupBox gbMasterSwitch;
        private System.Windows.Forms.RadioButton rbMasterRelayOnly;
        private System.Windows.Forms.RadioButton rbMasterAll;
        private System.Windows.Forms.RadioButton rbMasterOverride;
    }
}