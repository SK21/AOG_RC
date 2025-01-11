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
            this.ckNoMaster = new System.Windows.Forms.CheckBox();
            this.ckWorkSwitch = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbSections = new System.Windows.Forms.RadioButton();
            this.rbAutoAll = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
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
            // ckNoMaster
            // 
            this.ckNoMaster.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckNoMaster.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckNoMaster.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckNoMaster.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckNoMaster.Location = new System.Drawing.Point(316, 302);
            this.ckNoMaster.Name = "ckNoMaster";
            this.ckNoMaster.Size = new System.Drawing.Size(170, 37);
            this.ckNoMaster.TabIndex = 339;
            this.ckNoMaster.Text = "Master Override";
            this.ckNoMaster.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckNoMaster.UseVisualStyleBackColor = true;
            this.ckNoMaster.CheckedChanged += new System.EventHandler(this.ckDualAuto_CheckedChanged);
            // 
            // ckWorkSwitch
            // 
            this.ckWorkSwitch.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckWorkSwitch.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckWorkSwitch.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckWorkSwitch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckWorkSwitch.Location = new System.Drawing.Point(49, 302);
            this.ckWorkSwitch.Name = "ckWorkSwitch";
            this.ckWorkSwitch.Size = new System.Drawing.Size(170, 37);
            this.ckWorkSwitch.TabIndex = 338;
            this.ckWorkSwitch.Text = "Work Switch";
            this.ckWorkSwitch.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckWorkSwitch.UseVisualStyleBackColor = true;
            this.ckWorkSwitch.CheckedChanged += new System.EventHandler(this.ckDualAuto_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbSections);
            this.groupBox1.Controls.Add(this.rbAutoAll);
            this.groupBox1.Location = new System.Drawing.Point(300, 119);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 151);
            this.groupBox1.TabIndex = 340;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Auto Switch";
            this.groupBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox2_Paint);
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
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ckScreenSwitches);
            this.groupBox2.Controls.Add(this.ckDualAuto);
            this.groupBox2.Location = new System.Drawing.Point(30, 119);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 151);
            this.groupBox2.TabIndex = 341;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "On-Screen";
            this.groupBox2.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox2_Paint);
            // 
            // frmMenuSwitches
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 630);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ckNoMaster);
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
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox ckDualAuto;
        private System.Windows.Forms.CheckBox ckScreenSwitches;
        private System.Windows.Forms.CheckBox ckNoMaster;
        private System.Windows.Forms.CheckBox ckWorkSwitch;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbAutoAll;
        private System.Windows.Forms.RadioButton rbSections;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}