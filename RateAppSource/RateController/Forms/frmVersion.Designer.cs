namespace RateController.Forms
{
    partial class frmVersion
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmVersion));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbMod = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.lbModuleLatest = new System.Windows.Forms.Label();
            this.lbModule = new System.Windows.Forms.Label();
            this.lbModuleCurrent = new System.Windows.Forms.Label();
            this.lbSwitchBoxCurrent = new System.Windows.Forms.Label();
            this.lbSwitchBoxLatest = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lbAppLatest = new System.Windows.Forms.Label();
            this.lbAppCurrent = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbMod);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnRight);
            this.groupBox1.Controls.Add(this.btnLeft);
            this.groupBox1.Controls.Add(this.lbModuleLatest);
            this.groupBox1.Controls.Add(this.lbModule);
            this.groupBox1.Controls.Add(this.lbModuleCurrent);
            this.groupBox1.Location = new System.Drawing.Point(13, 88);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(459, 135);
            this.groupBox1.TabIndex = 284;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Rate Module";
            this.groupBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox1_Paint);
            // 
            // lbMod
            // 
            this.lbMod.Location = new System.Drawing.Point(86, 78);
            this.lbMod.Name = "lbMod";
            this.lbMod.Size = new System.Drawing.Size(59, 24);
            this.lbMod.TabIndex = 295;
            this.lbMod.Text = "0";
            this.lbMod.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 78);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 24);
            this.label1.TabIndex = 294;
            this.label1.Text = "Module";
            // 
            // btnRight
            // 
            this.btnRight.BackColor = System.Drawing.Color.Transparent;
            this.btnRight.FlatAppearance.BorderSize = 0;
            this.btnRight.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnRight.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnRight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRight.Image = global::RateController.Properties.Resources.ArrowRight;
            this.btnRight.Location = new System.Drawing.Point(289, 59);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(63, 62);
            this.btnRight.TabIndex = 293;
            this.btnRight.UseVisualStyleBackColor = false;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnLeft
            // 
            this.btnLeft.BackColor = System.Drawing.Color.Transparent;
            this.btnLeft.FlatAppearance.BorderSize = 0;
            this.btnLeft.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnLeft.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnLeft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLeft.Image = global::RateController.Properties.Resources.ArrowLeft;
            this.btnLeft.Location = new System.Drawing.Point(222, 59);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(60, 62);
            this.btnLeft.TabIndex = 292;
            this.btnLeft.UseVisualStyleBackColor = false;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // lbModuleLatest
            // 
            this.lbModuleLatest.Location = new System.Drawing.Point(327, 32);
            this.lbModuleLatest.Name = "lbModuleLatest";
            this.lbModuleLatest.Size = new System.Drawing.Size(125, 24);
            this.lbModuleLatest.TabIndex = 180;
            this.lbModuleLatest.Text = "10-Jul-2025";
            this.lbModuleLatest.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbModule
            // 
            this.lbModule.AutoSize = true;
            this.lbModule.Location = new System.Drawing.Point(6, 32);
            this.lbModule.Name = "lbModule";
            this.lbModule.Size = new System.Drawing.Size(56, 24);
            this.lbModule.TabIndex = 156;
            this.lbModule.Text = "Nano";
            // 
            // lbModuleCurrent
            // 
            this.lbModuleCurrent.Location = new System.Drawing.Point(184, 32);
            this.lbModuleCurrent.Name = "lbModuleCurrent";
            this.lbModuleCurrent.Size = new System.Drawing.Size(125, 24);
            this.lbModuleCurrent.TabIndex = 181;
            this.lbModuleCurrent.Text = "10-Jul-2025";
            this.lbModuleCurrent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbSwitchBoxCurrent
            // 
            this.lbSwitchBoxCurrent.Location = new System.Drawing.Point(197, 240);
            this.lbSwitchBoxCurrent.Name = "lbSwitchBoxCurrent";
            this.lbSwitchBoxCurrent.Size = new System.Drawing.Size(125, 24);
            this.lbSwitchBoxCurrent.TabIndex = 283;
            this.lbSwitchBoxCurrent.Text = "10-Jul-2025";
            this.lbSwitchBoxCurrent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbSwitchBoxLatest
            // 
            this.lbSwitchBoxLatest.Location = new System.Drawing.Point(340, 240);
            this.lbSwitchBoxLatest.Name = "lbSwitchBoxLatest";
            this.lbSwitchBoxLatest.Size = new System.Drawing.Size(125, 24);
            this.lbSwitchBoxLatest.TabIndex = 282;
            this.lbSwitchBoxLatest.Text = "10-Jul-2025";
            this.lbSwitchBoxLatest.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 240);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(98, 24);
            this.label6.TabIndex = 281;
            this.label6.Text = "SwitchBox";
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.Color.Transparent;
            this.btnUpdate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnUpdate.FlatAppearance.BorderSize = 0;
            this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdate.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnUpdate.Image = global::RateController.Properties.Resources.Update;
            this.btnUpdate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnUpdate.Location = new System.Drawing.Point(314, 271);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(72, 72);
            this.btnUpdate.TabIndex = 279;
            this.btnUpdate.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(373, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(58, 24);
            this.label8.TabIndex = 278;
            this.label8.Text = "Latest";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(223, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 24);
            this.label7.TabIndex = 277;
            this.label7.Text = "Current";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbAppLatest
            // 
            this.lbAppLatest.Location = new System.Drawing.Point(340, 41);
            this.lbAppLatest.Name = "lbAppLatest";
            this.lbAppLatest.Size = new System.Drawing.Size(125, 24);
            this.lbAppLatest.TabIndex = 276;
            this.lbAppLatest.Text = "4.1.0";
            this.lbAppLatest.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbAppCurrent
            // 
            this.lbAppCurrent.Location = new System.Drawing.Point(197, 41);
            this.lbAppCurrent.Name = "lbAppCurrent";
            this.lbAppCurrent.Size = new System.Drawing.Size(125, 24);
            this.lbAppCurrent.TabIndex = 275;
            this.lbAppCurrent.Text = "4.0.3";
            this.lbAppCurrent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 41);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 24);
            this.label5.TabIndex = 274;
            this.label5.Text = "RC App";
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
            this.btnOK.Location = new System.Drawing.Point(400, 271);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(72, 72);
            this.btnOK.TabIndex = 273;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // frmVersion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 353);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lbSwitchBoxCurrent);
            this.Controls.Add(this.lbSwitchBoxLatest);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lbAppLatest);
            this.Controls.Add(this.lbAppCurrent);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmVersion";
            this.Text = "Version Check";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmVersion_FormClosed);
            this.Load += new System.EventHandler(this.frmVersion_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbModuleLatest;
        private System.Windows.Forms.Label lbModule;
        private System.Windows.Forms.Label lbModuleCurrent;
        private System.Windows.Forms.Label lbSwitchBoxCurrent;
        private System.Windows.Forms.Label lbSwitchBoxLatest;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lbAppLatest;
        private System.Windows.Forms.Label lbAppCurrent;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lbMod;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnLeft;
    }
}