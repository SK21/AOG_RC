namespace PCBsetup.Forms
{
    partial class frmSwitchboxSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSwitchboxSettings));
            this.btnLoadDefaults = new System.Windows.Forms.Button();
            this.btnSendToModule = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.bntOK = new System.Windows.Forms.Button();
            this.tbMasterOn = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbAuto = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbRateUp = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbMasterOff = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbSW2 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbSW1 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbSW4 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbSW3 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbRateDown = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.lbIPpart4 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.tbIPaddress = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnLoadDefaults
            // 
            this.btnLoadDefaults.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnLoadDefaults.Location = new System.Drawing.Point(19, 268);
            this.btnLoadDefaults.Name = "btnLoadDefaults";
            this.btnLoadDefaults.Size = new System.Drawing.Size(115, 72);
            this.btnLoadDefaults.TabIndex = 23;
            this.btnLoadDefaults.Text = "Load Defaults";
            this.btnLoadDefaults.UseVisualStyleBackColor = true;
            this.btnLoadDefaults.Click += new System.EventHandler(this.btnLoadDefaults_Click);
            // 
            // btnSendToModule
            // 
            this.btnSendToModule.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSendToModule.Location = new System.Drawing.Point(154, 268);
            this.btnSendToModule.Name = "btnSendToModule";
            this.btnSendToModule.Size = new System.Drawing.Size(115, 72);
            this.btnSendToModule.TabIndex = 22;
            this.btnSendToModule.Text = "Send to Module";
            this.btnSendToModule.UseVisualStyleBackColor = true;
            this.btnSendToModule.Click += new System.EventHandler(this.btnSendToModule_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Enabled = false;
            this.btnCancel.Image = global::PCBsetup.Properties.Resources.Cancel64;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCancel.Location = new System.Drawing.Point(289, 268);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(115, 72);
            this.btnCancel.TabIndex = 21;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // bntOK
            // 
            this.bntOK.Image = global::PCBsetup.Properties.Resources.bntOK_Image;
            this.bntOK.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.bntOK.Location = new System.Drawing.Point(424, 268);
            this.bntOK.Name = "bntOK";
            this.bntOK.Size = new System.Drawing.Size(115, 72);
            this.bntOK.TabIndex = 20;
            this.bntOK.Text = "Close";
            this.bntOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.bntOK.UseVisualStyleBackColor = true;
            this.bntOK.Click += new System.EventHandler(this.bntOK_Click);
            // 
            // tbMasterOn
            // 
            this.tbMasterOn.Location = new System.Drawing.Point(171, 62);
            this.tbMasterOn.Name = "tbMasterOn";
            this.tbMasterOn.Size = new System.Drawing.Size(58, 29);
            this.tbMasterOn.TabIndex = 27;
            this.tbMasterOn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 24);
            this.label2.TabIndex = 26;
            this.label2.Text = "Master On";
            // 
            // tbAuto
            // 
            this.tbAuto.Location = new System.Drawing.Point(171, 12);
            this.tbAuto.Name = "tbAuto";
            this.tbAuto.Size = new System.Drawing.Size(58, 29);
            this.tbAuto.TabIndex = 25;
            this.tbAuto.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 24);
            this.label1.TabIndex = 24;
            this.label1.Text = "Auto";
            // 
            // tbRateUp
            // 
            this.tbRateUp.Location = new System.Drawing.Point(171, 162);
            this.tbRateUp.Name = "tbRateUp";
            this.tbRateUp.Size = new System.Drawing.Size(58, 29);
            this.tbRateUp.TabIndex = 31;
            this.tbRateUp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(31, 164);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 24);
            this.label3.TabIndex = 30;
            this.label3.Text = "Rate Up";
            // 
            // tbMasterOff
            // 
            this.tbMasterOff.Location = new System.Drawing.Point(171, 112);
            this.tbMasterOff.Name = "tbMasterOff";
            this.tbMasterOff.Size = new System.Drawing.Size(58, 29);
            this.tbMasterOff.TabIndex = 29;
            this.tbMasterOff.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(31, 114);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(94, 24);
            this.label4.TabIndex = 28;
            this.label4.Text = "Master Off";
            // 
            // tbSW2
            // 
            this.tbSW2.Location = new System.Drawing.Point(437, 62);
            this.tbSW2.Name = "tbSW2";
            this.tbSW2.Size = new System.Drawing.Size(58, 29);
            this.tbSW2.TabIndex = 35;
            this.tbSW2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(340, 64);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 24);
            this.label5.TabIndex = 34;
            this.label5.Text = "Switch 2";
            // 
            // tbSW1
            // 
            this.tbSW1.Location = new System.Drawing.Point(437, 12);
            this.tbSW1.Name = "tbSW1";
            this.tbSW1.Size = new System.Drawing.Size(58, 29);
            this.tbSW1.TabIndex = 33;
            this.tbSW1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(340, 14);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 24);
            this.label6.TabIndex = 32;
            this.label6.Text = "Switch 1";
            // 
            // tbSW4
            // 
            this.tbSW4.Location = new System.Drawing.Point(437, 162);
            this.tbSW4.Name = "tbSW4";
            this.tbSW4.Size = new System.Drawing.Size(58, 29);
            this.tbSW4.TabIndex = 39;
            this.tbSW4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(340, 164);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 24);
            this.label7.TabIndex = 38;
            this.label7.Text = "Switch 4";
            // 
            // tbSW3
            // 
            this.tbSW3.Location = new System.Drawing.Point(437, 112);
            this.tbSW3.Name = "tbSW3";
            this.tbSW3.Size = new System.Drawing.Size(58, 29);
            this.tbSW3.TabIndex = 37;
            this.tbSW3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(340, 114);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(80, 24);
            this.label8.TabIndex = 36;
            this.label8.Text = "Switch 3";
            // 
            // tbRateDown
            // 
            this.tbRateDown.Location = new System.Drawing.Point(171, 212);
            this.tbRateDown.Name = "tbRateDown";
            this.tbRateDown.Size = new System.Drawing.Size(58, 29);
            this.tbRateDown.TabIndex = 41;
            this.tbRateDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(31, 214);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(102, 24);
            this.label9.TabIndex = 40;
            this.label9.Text = "Rate Down";
            // 
            // lbIPpart4
            // 
            this.lbIPpart4.AutoSize = true;
            this.lbIPpart4.Location = new System.Drawing.Point(493, 214);
            this.lbIPpart4.Name = "lbIPpart4";
            this.lbIPpart4.Size = new System.Drawing.Size(45, 24);
            this.lbIPpart4.TabIndex = 44;
            this.lbIPpart4.Text = ".188";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(255, 214);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(181, 24);
            this.label30.TabIndex = 43;
            this.label30.Text = "IP Address  192.168.";
            // 
            // tbIPaddress
            // 
            this.tbIPaddress.Location = new System.Drawing.Point(437, 212);
            this.tbIPaddress.Name = "tbIPaddress";
            this.tbIPaddress.Size = new System.Drawing.Size(50, 29);
            this.tbIPaddress.TabIndex = 42;
            this.tbIPaddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // frmSwitchboxSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(557, 352);
            this.Controls.Add(this.lbIPpart4);
            this.Controls.Add(this.label30);
            this.Controls.Add(this.tbIPaddress);
            this.Controls.Add(this.tbRateDown);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.tbSW4);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tbSW3);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.tbSW2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbSW1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tbRateUp);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbMasterOff);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbMasterOn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbAuto);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnLoadDefaults);
            this.Controls.Add(this.btnSendToModule);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.bntOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSwitchboxSettings";
            this.ShowInTaskbar = false;
            this.Text = "Nano Switchbox Settings";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmSwitchboxSettings_FormClosed);
            this.Load += new System.EventHandler(this.frmSwitchboxSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLoadDefaults;
        private System.Windows.Forms.Button btnSendToModule;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button bntOK;
        private System.Windows.Forms.TextBox tbMasterOn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbAuto;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbRateUp;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbMasterOff;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbSW2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbSW1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbSW4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbSW3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbRateDown;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lbIPpart4;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.TextBox tbIPaddress;
    }
}