namespace RateController
{
    partial class frmSwitches
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSwitches));
            this.tbSpeed = new System.Windows.Forms.TextBox();
            this.rbRate = new System.Windows.Forms.RadioButton();
            this.rbSpeed = new System.Windows.Forms.RadioButton();
            this.lbMPH = new System.Windows.Forms.Label();
            this.rbOff = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ckMaster = new System.Windows.Forms.CheckBox();
            this.btnDown = new System.Windows.Forms.Button();
            this.btn1 = new System.Windows.Forms.Button();
            this.btn2 = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btn3 = new System.Windows.Forms.Button();
            this.btn4 = new System.Windows.Forms.Button();
            this.btAuto = new System.Windows.Forms.Button();
            this.grpSim = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbPWM = new System.Windows.Forms.TextBox();
            this.bntOK = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.grpSim.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbSpeed
            // 
            this.tbSpeed.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSpeed.Location = new System.Drawing.Point(8, 228);
            this.tbSpeed.Margin = new System.Windows.Forms.Padding(6);
            this.tbSpeed.MaxLength = 8;
            this.tbSpeed.Name = "tbSpeed";
            this.tbSpeed.Size = new System.Drawing.Size(47, 30);
            this.tbSpeed.TabIndex = 4;
            this.tbSpeed.Text = "10.5";
            this.tbSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSpeed.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.tbSpeed_HelpRequested);
            this.tbSpeed.Enter += new System.EventHandler(this.tbSpeed_Enter);
            // 
            // rbRate
            // 
            this.rbRate.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbRate.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbRate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbRate.Location = new System.Drawing.Point(8, 88);
            this.rbRate.Margin = new System.Windows.Forms.Padding(4);
            this.rbRate.Name = "rbRate";
            this.rbRate.Size = new System.Drawing.Size(113, 63);
            this.rbRate.TabIndex = 1;
            this.rbRate.Tag = "0";
            this.rbRate.Text = "Module";
            this.rbRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbRate.UseVisualStyleBackColor = true;
            this.rbRate.Click += new System.EventHandler(this.rbOff_Click);
            this.rbRate.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.rbRate_HelpRequested);
            // 
            // rbSpeed
            // 
            this.rbSpeed.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbSpeed.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbSpeed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbSpeed.Location = new System.Drawing.Point(8, 160);
            this.rbSpeed.Margin = new System.Windows.Forms.Padding(4);
            this.rbSpeed.Name = "rbSpeed";
            this.rbSpeed.Size = new System.Drawing.Size(113, 58);
            this.rbSpeed.TabIndex = 2;
            this.rbSpeed.Tag = "0";
            this.rbSpeed.Text = "Speed";
            this.rbSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbSpeed.UseVisualStyleBackColor = true;
            this.rbSpeed.Click += new System.EventHandler(this.rbOff_Click);
            this.rbSpeed.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.rbSpeed_HelpRequested);
            // 
            // lbMPH
            // 
            this.lbMPH.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMPH.Location = new System.Drawing.Point(67, 227);
            this.lbMPH.Name = "lbMPH";
            this.lbMPH.Size = new System.Drawing.Size(55, 32);
            this.lbMPH.TabIndex = 152;
            this.lbMPH.Text = "MPH";
            this.lbMPH.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // rbOff
            // 
            this.rbOff.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbOff.Checked = true;
            this.rbOff.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbOff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbOff.Location = new System.Drawing.Point(8, 29);
            this.rbOff.Margin = new System.Windows.Forms.Padding(4);
            this.rbOff.Name = "rbOff";
            this.rbOff.Size = new System.Drawing.Size(113, 50);
            this.rbOff.TabIndex = 0;
            this.rbOff.TabStop = true;
            this.rbOff.Tag = "0";
            this.rbOff.Text = "Off";
            this.rbOff.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbOff.UseVisualStyleBackColor = true;
            this.rbOff.Click += new System.EventHandler(this.rbOff_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ckMaster);
            this.groupBox1.Controls.Add(this.btnDown);
            this.groupBox1.Controls.Add(this.btn1);
            this.groupBox1.Controls.Add(this.btn2);
            this.groupBox1.Controls.Add(this.btnUp);
            this.groupBox1.Controls.Add(this.btn3);
            this.groupBox1.Controls.Add(this.btn4);
            this.groupBox1.Controls.Add(this.btAuto);
            this.groupBox1.Location = new System.Drawing.Point(5, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(101, 390);
            this.groupBox1.TabIndex = 153;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Switches";
            this.groupBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.grpSections_Paint);
            // 
            // ckMaster
            // 
            this.ckMaster.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckMaster.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckMaster.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckMaster.Location = new System.Drawing.Point(6, 28);
            this.ckMaster.Name = "ckMaster";
            this.ckMaster.Size = new System.Drawing.Size(89, 32);
            this.ckMaster.TabIndex = 159;
            this.ckMaster.Text = "Master";
            this.ckMaster.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckMaster.UseVisualStyleBackColor = true;
            this.ckMaster.CheckedChanged += new System.EventHandler(this.ckMaster_CheckedChanged);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(6, 325);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(89, 56);
            this.btnDown.TabIndex = 166;
            this.btnDown.Text = "Rate Down";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btn1
            // 
            this.btn1.Location = new System.Drawing.Point(6, 67);
            this.btn1.Name = "btn1";
            this.btn1.Size = new System.Drawing.Size(89, 32);
            this.btn1.TabIndex = 160;
            this.btn1.Text = "1";
            this.btn1.UseVisualStyleBackColor = true;
            this.btn1.Click += new System.EventHandler(this.btn1_Click);
            // 
            // btn2
            // 
            this.btn2.Location = new System.Drawing.Point(6, 106);
            this.btn2.Name = "btn2";
            this.btn2.Size = new System.Drawing.Size(89, 32);
            this.btn2.TabIndex = 161;
            this.btn2.Text = "2";
            this.btn2.UseVisualStyleBackColor = true;
            this.btn2.Click += new System.EventHandler(this.btn2_Click);
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(6, 262);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(89, 56);
            this.btnUp.TabIndex = 165;
            this.btnUp.Text = "Rate Up";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btn3
            // 
            this.btn3.Location = new System.Drawing.Point(6, 145);
            this.btn3.Name = "btn3";
            this.btn3.Size = new System.Drawing.Size(89, 32);
            this.btn3.TabIndex = 162;
            this.btn3.Text = "3";
            this.btn3.UseVisualStyleBackColor = true;
            this.btn3.Click += new System.EventHandler(this.btn3_Click);
            // 
            // btn4
            // 
            this.btn4.Location = new System.Drawing.Point(6, 184);
            this.btn4.Name = "btn4";
            this.btn4.Size = new System.Drawing.Size(89, 32);
            this.btn4.TabIndex = 163;
            this.btn4.Text = "4";
            this.btn4.UseVisualStyleBackColor = true;
            this.btn4.Click += new System.EventHandler(this.btn4_Click);
            // 
            // btAuto
            // 
            this.btAuto.Location = new System.Drawing.Point(6, 223);
            this.btAuto.Name = "btAuto";
            this.btAuto.Size = new System.Drawing.Size(89, 32);
            this.btAuto.TabIndex = 164;
            this.btAuto.Text = "Auto";
            this.btAuto.UseVisualStyleBackColor = true;
            this.btAuto.Click += new System.EventHandler(this.btAuto_Click);
            // 
            // grpSim
            // 
            this.grpSim.Controls.Add(this.label1);
            this.grpSim.Controls.Add(this.tbPWM);
            this.grpSim.Controls.Add(this.rbOff);
            this.grpSim.Controls.Add(this.lbMPH);
            this.grpSim.Controls.Add(this.rbRate);
            this.grpSim.Controls.Add(this.tbSpeed);
            this.grpSim.Controls.Add(this.rbSpeed);
            this.grpSim.Location = new System.Drawing.Point(114, 5);
            this.grpSim.Name = "grpSim";
            this.grpSim.Size = new System.Drawing.Size(128, 312);
            this.grpSim.TabIndex = 155;
            this.grpSim.TabStop = false;
            this.grpSim.Text = "Simulation";
            this.grpSim.Paint += new System.Windows.Forms.PaintEventHandler(this.grpSections_Paint);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(67, 269);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 32);
            this.label1.TabIndex = 159;
            this.label1.Text = "PWM";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbPWM
            // 
            this.tbPWM.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPWM.Location = new System.Drawing.Point(8, 270);
            this.tbPWM.Margin = new System.Windows.Forms.Padding(6);
            this.tbPWM.MaxLength = 8;
            this.tbPWM.Name = "tbPWM";
            this.tbPWM.Size = new System.Drawing.Size(47, 30);
            this.tbPWM.TabIndex = 158;
            this.tbPWM.Text = "10.5";
            this.tbPWM.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbPWM.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.tbPWM_HelpRequested);
            this.tbPWM.Enter += new System.EventHandler(this.tbPWM_Enter);
            // 
            // bntOK
            // 
            this.bntOK.BackColor = System.Drawing.Color.Transparent;
            this.bntOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bntOK.FlatAppearance.BorderSize = 0;
            this.bntOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bntOK.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.bntOK.Image = global::RateController.Properties.Resources.OK;
            this.bntOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.bntOK.Location = new System.Drawing.Point(121, 323);
            this.bntOK.Name = "bntOK";
            this.bntOK.Size = new System.Drawing.Size(115, 72);
            this.bntOK.TabIndex = 156;
            this.bntOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.bntOK.UseVisualStyleBackColor = false;
            this.bntOK.Click += new System.EventHandler(this.bntOK_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // frmSwitches
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(248, 401);
            this.Controls.Add(this.bntOK);
            this.Controls.Add(this.grpSim);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSwitches";
            this.ShowInTaskbar = false;
            this.Text = "Switches";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmSimulation_FormClosed);
            this.Load += new System.EventHandler(this.frmSimulation_Load);
            this.groupBox1.ResumeLayout(false);
            this.grpSim.ResumeLayout(false);
            this.grpSim.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox tbSpeed;
        private System.Windows.Forms.RadioButton rbRate;
        private System.Windows.Forms.RadioButton rbSpeed;
        private System.Windows.Forms.Label lbMPH;
        private System.Windows.Forms.RadioButton rbOff;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox grpSim;
        private System.Windows.Forms.Button bntOK;
        private System.Windows.Forms.CheckBox ckMaster;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btn1;
        private System.Windows.Forms.Button btn2;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btn3;
        private System.Windows.Forms.Button btn4;
        private System.Windows.Forms.Button btAuto;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbPWM;
    }
}