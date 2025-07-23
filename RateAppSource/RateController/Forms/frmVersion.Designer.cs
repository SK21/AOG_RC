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
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbModuleLatest = new System.Windows.Forms.Label();
            this.lbModule = new System.Windows.Forms.Label();
            this.lbModuleCurrent = new System.Windows.Forms.Label();
            this.lbModuleID = new System.Windows.Forms.Label();
            this.tbModuleID = new System.Windows.Forms.TextBox();
            this.lbSwitchBoxCurrent = new System.Windows.Forms.Label();
            this.lbSwitchBoxLatest = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.linkLabel4 = new System.Windows.Forms.LinkLabel();
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
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(70, 295);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(88, 24);
            this.linkLabel1.TabIndex = 285;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Releases";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbModuleLatest);
            this.groupBox1.Controls.Add(this.lbModule);
            this.groupBox1.Controls.Add(this.lbModuleCurrent);
            this.groupBox1.Controls.Add(this.lbModuleID);
            this.groupBox1.Controls.Add(this.tbModuleID);
            this.groupBox1.Location = new System.Drawing.Point(13, 88);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(459, 106);
            this.groupBox1.TabIndex = 284;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Rate Module";
            this.groupBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox1_Paint);
            // 
            // lbModuleLatest
            // 
            this.lbModuleLatest.Location = new System.Drawing.Point(327, 71);
            this.lbModuleLatest.Name = "lbModuleLatest";
            this.lbModuleLatest.Size = new System.Drawing.Size(125, 24);
            this.lbModuleLatest.TabIndex = 180;
            this.lbModuleLatest.Text = "10-Jul-2025";
            this.lbModuleLatest.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbModule
            // 
            this.lbModule.AutoSize = true;
            this.lbModule.Location = new System.Drawing.Point(6, 71);
            this.lbModule.Name = "lbModule";
            this.lbModule.Size = new System.Drawing.Size(56, 24);
            this.lbModule.TabIndex = 156;
            this.lbModule.Text = "Nano";
            // 
            // lbModuleCurrent
            // 
            this.lbModuleCurrent.Location = new System.Drawing.Point(184, 71);
            this.lbModuleCurrent.Name = "lbModuleCurrent";
            this.lbModuleCurrent.Size = new System.Drawing.Size(125, 24);
            this.lbModuleCurrent.TabIndex = 181;
            this.lbModuleCurrent.Text = "10-Jul-2025";
            this.lbModuleCurrent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbModuleID
            // 
            this.lbModuleID.AutoSize = true;
            this.lbModuleID.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbModuleID.Location = new System.Drawing.Point(6, 34);
            this.lbModuleID.Name = "lbModuleID";
            this.lbModuleID.Size = new System.Drawing.Size(96, 24);
            this.lbModuleID.TabIndex = 266;
            this.lbModuleID.Text = "Module ID";
            // 
            // tbModuleID
            // 
            this.tbModuleID.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbModuleID.Location = new System.Drawing.Point(126, 31);
            this.tbModuleID.Name = "tbModuleID";
            this.tbModuleID.Size = new System.Drawing.Size(58, 29);
            this.tbModuleID.TabIndex = 267;
            this.tbModuleID.TabStop = false;
            this.tbModuleID.Text = "0";
            this.tbModuleID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbModuleID.Click += new System.EventHandler(this.tbModuleID_Enter);
            this.tbModuleID.Enter += new System.EventHandler(this.tbModuleID_Enter);
            // 
            // lbSwitchBoxCurrent
            // 
            this.lbSwitchBoxCurrent.Location = new System.Drawing.Point(197, 226);
            this.lbSwitchBoxCurrent.Name = "lbSwitchBoxCurrent";
            this.lbSwitchBoxCurrent.Size = new System.Drawing.Size(125, 24);
            this.lbSwitchBoxCurrent.TabIndex = 283;
            this.lbSwitchBoxCurrent.Text = "10-Jul-2025";
            this.lbSwitchBoxCurrent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbSwitchBoxLatest
            // 
            this.lbSwitchBoxLatest.Location = new System.Drawing.Point(340, 226);
            this.lbSwitchBoxLatest.Name = "lbSwitchBoxLatest";
            this.lbSwitchBoxLatest.Size = new System.Drawing.Size(125, 24);
            this.lbSwitchBoxLatest.TabIndex = 282;
            this.lbSwitchBoxLatest.Text = "10-Jul-2025";
            this.lbSwitchBoxLatest.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 226);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(98, 24);
            this.label6.TabIndex = 281;
            this.label6.Text = "SwitchBox";
            // 
            // linkLabel4
            // 
            this.linkLabel4.AutoSize = true;
            this.linkLabel4.Location = new System.Drawing.Point(202, 295);
            this.linkLabel4.Name = "linkLabel4";
            this.linkLabel4.Size = new System.Drawing.Size(93, 24);
            this.linkLabel4.TabIndex = 280;
            this.linkLabel4.TabStop = true;
            this.linkLabel4.Text = "PCBsetup";
            this.linkLabel4.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel4_LinkClicked);
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
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lbSwitchBoxCurrent);
            this.Controls.Add(this.lbSwitchBoxLatest);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.linkLabel4);
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

        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbModuleLatest;
        private System.Windows.Forms.Label lbModule;
        private System.Windows.Forms.Label lbModuleCurrent;
        private System.Windows.Forms.Label lbModuleID;
        private System.Windows.Forms.TextBox tbModuleID;
        private System.Windows.Forms.Label lbSwitchBoxCurrent;
        private System.Windows.Forms.Label lbSwitchBoxLatest;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.LinkLabel linkLabel4;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lbAppLatest;
        private System.Windows.Forms.Label lbAppCurrent;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Timer timer1;
    }
}