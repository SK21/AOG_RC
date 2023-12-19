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
            this.btnMaster = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btn1 = new System.Windows.Forms.Button();
            this.btn2 = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btn3 = new System.Windows.Forms.Button();
            this.btn4 = new System.Windows.Forms.Button();
            this.btAuto = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tmrRelease = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // btnMaster
            // 
            this.btnMaster.BackColor = System.Drawing.Color.LightGreen;
            this.btnMaster.Location = new System.Drawing.Point(12, 12);
            this.btnMaster.Name = "btnMaster";
            this.btnMaster.Size = new System.Drawing.Size(64, 35);
            this.btnMaster.TabIndex = 167;
            this.btnMaster.Text = "MST";
            this.btnMaster.UseVisualStyleBackColor = false;
            this.btnMaster.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnMaster_MouseDown);
            this.btnMaster.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnMaster_MouseUp);
            // 
            // btnDown
            // 
            this.btnDown.BackColor = System.Drawing.Color.DimGray;
            this.btnDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDown.Location = new System.Drawing.Point(222, 12);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(64, 35);
            this.btnDown.TabIndex = 166;
            this.btnDown.Text = "-";
            this.btnDown.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnDown.UseVisualStyleBackColor = false;
            this.btnDown.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnDown_MouseDown);
            this.btnDown.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnDown_MouseUp);
            // 
            // btn1
            // 
            this.btn1.Location = new System.Drawing.Point(12, 53);
            this.btn1.Name = "btn1";
            this.btn1.Size = new System.Drawing.Size(64, 35);
            this.btn1.TabIndex = 160;
            this.btn1.Text = "1";
            this.btn1.UseVisualStyleBackColor = true;
            this.btn1.Click += new System.EventHandler(this.btn1_Click);
            // 
            // btn2
            // 
            this.btn2.Location = new System.Drawing.Point(82, 53);
            this.btn2.Name = "btn2";
            this.btn2.Size = new System.Drawing.Size(64, 35);
            this.btn2.TabIndex = 161;
            this.btn2.Text = "2";
            this.btn2.UseVisualStyleBackColor = true;
            this.btn2.Click += new System.EventHandler(this.btn2_Click);
            // 
            // btnUp
            // 
            this.btnUp.BackColor = System.Drawing.Color.DarkGray;
            this.btnUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUp.Location = new System.Drawing.Point(152, 12);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(64, 35);
            this.btnUp.TabIndex = 165;
            this.btnUp.Text = "+";
            this.btnUp.UseVisualStyleBackColor = false;
            this.btnUp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnUp_MouseDown);
            this.btnUp.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnUp_MouseUp);
            // 
            // btn3
            // 
            this.btn3.Location = new System.Drawing.Point(152, 53);
            this.btn3.Name = "btn3";
            this.btn3.Size = new System.Drawing.Size(64, 35);
            this.btn3.TabIndex = 162;
            this.btn3.Text = "3";
            this.btn3.UseVisualStyleBackColor = true;
            this.btn3.Click += new System.EventHandler(this.btn3_Click);
            // 
            // btn4
            // 
            this.btn4.Location = new System.Drawing.Point(222, 53);
            this.btn4.Name = "btn4";
            this.btn4.Size = new System.Drawing.Size(64, 35);
            this.btn4.TabIndex = 163;
            this.btn4.Text = "4";
            this.btn4.UseVisualStyleBackColor = true;
            this.btn4.Click += new System.EventHandler(this.btn4_Click);
            // 
            // btAuto
            // 
            this.btAuto.Location = new System.Drawing.Point(82, 12);
            this.btAuto.Name = "btAuto";
            this.btAuto.Size = new System.Drawing.Size(64, 35);
            this.btAuto.TabIndex = 164;
            this.btAuto.Text = "Auto";
            this.btAuto.UseVisualStyleBackColor = true;
            this.btAuto.Click += new System.EventHandler(this.btAuto_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // tmrRelease
            // 
            this.tmrRelease.Interval = 500;
            this.tmrRelease.Tick += new System.EventHandler(this.tmrRelease_Tick);
            // 
            // frmSwitches
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(298, 96);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnMaster);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btn2);
            this.Controls.Add(this.btAuto);
            this.Controls.Add(this.btn4);
            this.Controls.Add(this.btn3);
            this.Controls.Add(this.btn1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
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
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btn1;
        private System.Windows.Forms.Button btn2;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btn3;
        private System.Windows.Forms.Button btn4;
        private System.Windows.Forms.Button btAuto;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnMaster;
        private System.Windows.Forms.Timer tmrRelease;
    }
}