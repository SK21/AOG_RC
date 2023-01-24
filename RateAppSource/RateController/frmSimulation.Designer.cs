namespace RateController
{
    partial class frmSimulation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSimulation));
            this.tbSpeed = new System.Windows.Forms.TextBox();
            this.rbRate = new System.Windows.Forms.RadioButton();
            this.rbSpeed = new System.Windows.Forms.RadioButton();
            this.lbMPH = new System.Windows.Forms.Label();
            this.rbOff = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.swMasterOff = new System.Windows.Forms.Label();
            this.swDown = new System.Windows.Forms.Label();
            this.swUp = new System.Windows.Forms.Label();
            this.swAuto = new System.Windows.Forms.Label();
            this.swFour = new System.Windows.Forms.Label();
            this.swThree = new System.Windows.Forms.Label();
            this.swTwo = new System.Windows.Forms.Label();
            this.swOne = new System.Windows.Forms.Label();
            this.swMasterOn = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.bntOK = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbSpeed
            // 
            this.tbSpeed.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSpeed.Location = new System.Drawing.Point(9, 160);
            this.tbSpeed.Margin = new System.Windows.Forms.Padding(6);
            this.tbSpeed.MaxLength = 8;
            this.tbSpeed.Name = "tbSpeed";
            this.tbSpeed.Size = new System.Drawing.Size(47, 30);
            this.tbSpeed.TabIndex = 4;
            this.tbSpeed.Text = "10.5";
            this.tbSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSpeed.Enter += new System.EventHandler(this.tbSpeed_Enter);
            // 
            // rbRate
            // 
            this.rbRate.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbRate.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.rbRate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbRate.Location = new System.Drawing.Point(7, 73);
            this.rbRate.Margin = new System.Windows.Forms.Padding(4);
            this.rbRate.Name = "rbRate";
            this.rbRate.Size = new System.Drawing.Size(113, 36);
            this.rbRate.TabIndex = 1;
            this.rbRate.Tag = "0";
            this.rbRate.Text = "Rate";
            this.rbRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbRate.UseVisualStyleBackColor = true;
            this.rbRate.Click += new System.EventHandler(this.rbOff_Click);
            // 
            // rbSpeed
            // 
            this.rbSpeed.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbSpeed.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.rbSpeed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbSpeed.Location = new System.Drawing.Point(7, 117);
            this.rbSpeed.Margin = new System.Windows.Forms.Padding(4);
            this.rbSpeed.Name = "rbSpeed";
            this.rbSpeed.Size = new System.Drawing.Size(113, 36);
            this.rbSpeed.TabIndex = 2;
            this.rbSpeed.Tag = "0";
            this.rbSpeed.Text = "Speed";
            this.rbSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbSpeed.UseVisualStyleBackColor = true;
            this.rbSpeed.Click += new System.EventHandler(this.rbOff_Click);
            // 
            // lbMPH
            // 
            this.lbMPH.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMPH.Location = new System.Drawing.Point(65, 159);
            this.lbMPH.Name = "lbMPH";
            this.lbMPH.Size = new System.Drawing.Size(55, 30);
            this.lbMPH.TabIndex = 152;
            this.lbMPH.Text = "MPH";
            this.lbMPH.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // rbOff
            // 
            this.rbOff.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbOff.Checked = true;
            this.rbOff.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.rbOff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbOff.Location = new System.Drawing.Point(7, 29);
            this.rbOff.Margin = new System.Windows.Forms.Padding(4);
            this.rbOff.Name = "rbOff";
            this.rbOff.Size = new System.Drawing.Size(113, 36);
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
            this.groupBox1.Controls.Add(this.swMasterOff);
            this.groupBox1.Controls.Add(this.swDown);
            this.groupBox1.Controls.Add(this.swUp);
            this.groupBox1.Controls.Add(this.swAuto);
            this.groupBox1.Controls.Add(this.swFour);
            this.groupBox1.Controls.Add(this.swThree);
            this.groupBox1.Controls.Add(this.swTwo);
            this.groupBox1.Controls.Add(this.swOne);
            this.groupBox1.Controls.Add(this.swMasterOn);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(125, 281);
            this.groupBox1.TabIndex = 153;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Switches";
            this.groupBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.grpSections_Paint);
            // 
            // swMasterOff
            // 
            this.swMasterOff.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.swMasterOff.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.swMasterOff.Location = new System.Drawing.Point(6, 55);
            this.swMasterOff.Name = "swMasterOff";
            this.swMasterOff.Size = new System.Drawing.Size(110, 23);
            this.swMasterOff.TabIndex = 213;
            this.swMasterOff.Text = "Master Off";
            this.swMasterOff.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.swMasterOff.Click += new System.EventHandler(this.swMasterOff_Click);
            // 
            // swDown
            // 
            this.swDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.swDown.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.swDown.Location = new System.Drawing.Point(6, 251);
            this.swDown.Name = "swDown";
            this.swDown.Size = new System.Drawing.Size(110, 23);
            this.swDown.TabIndex = 212;
            this.swDown.Text = "Rate Down";
            this.swDown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.swDown.Click += new System.EventHandler(this.swDown_Click);
            // 
            // swUp
            // 
            this.swUp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.swUp.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.swUp.Location = new System.Drawing.Point(6, 223);
            this.swUp.Name = "swUp";
            this.swUp.Size = new System.Drawing.Size(110, 23);
            this.swUp.TabIndex = 211;
            this.swUp.Text = "Rate Up";
            this.swUp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.swUp.Click += new System.EventHandler(this.swUp_Click);
            // 
            // swAuto
            // 
            this.swAuto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.swAuto.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.swAuto.Location = new System.Drawing.Point(6, 195);
            this.swAuto.Name = "swAuto";
            this.swAuto.Size = new System.Drawing.Size(110, 23);
            this.swAuto.TabIndex = 210;
            this.swAuto.Text = "Auto";
            this.swAuto.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.swAuto.Click += new System.EventHandler(this.swAuto_Click);
            // 
            // swFour
            // 
            this.swFour.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.swFour.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.swFour.Location = new System.Drawing.Point(6, 167);
            this.swFour.Name = "swFour";
            this.swFour.Size = new System.Drawing.Size(110, 23);
            this.swFour.TabIndex = 209;
            this.swFour.Text = "4";
            this.swFour.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.swFour.Click += new System.EventHandler(this.swFour_Click);
            // 
            // swThree
            // 
            this.swThree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.swThree.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.swThree.Location = new System.Drawing.Point(6, 139);
            this.swThree.Name = "swThree";
            this.swThree.Size = new System.Drawing.Size(110, 23);
            this.swThree.TabIndex = 208;
            this.swThree.Text = "3";
            this.swThree.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.swThree.Click += new System.EventHandler(this.swThree_Click);
            // 
            // swTwo
            // 
            this.swTwo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.swTwo.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.swTwo.Location = new System.Drawing.Point(6, 111);
            this.swTwo.Name = "swTwo";
            this.swTwo.Size = new System.Drawing.Size(110, 23);
            this.swTwo.TabIndex = 207;
            this.swTwo.Text = "2";
            this.swTwo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.swTwo.Click += new System.EventHandler(this.swTwo_Click);
            // 
            // swOne
            // 
            this.swOne.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.swOne.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.swOne.Location = new System.Drawing.Point(6, 83);
            this.swOne.Name = "swOne";
            this.swOne.Size = new System.Drawing.Size(110, 23);
            this.swOne.TabIndex = 206;
            this.swOne.Text = "1";
            this.swOne.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.swOne.Click += new System.EventHandler(this.swOne_Click);
            // 
            // swMasterOn
            // 
            this.swMasterOn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.swMasterOn.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.swMasterOn.Location = new System.Drawing.Point(6, 27);
            this.swMasterOn.Name = "swMasterOn";
            this.swMasterOn.Size = new System.Drawing.Size(110, 23);
            this.swMasterOn.TabIndex = 205;
            this.swMasterOn.Text = "Master On";
            this.swMasterOn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.swMasterOn.Click += new System.EventHandler(this.swMasterOn_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rbOff);
            this.groupBox3.Controls.Add(this.rbRate);
            this.groupBox3.Controls.Add(this.lbMPH);
            this.groupBox3.Controls.Add(this.tbSpeed);
            this.groupBox3.Controls.Add(this.rbSpeed);
            this.groupBox3.Location = new System.Drawing.Point(143, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(128, 201);
            this.groupBox3.TabIndex = 155;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Simulation";
            this.groupBox3.Paint += new System.Windows.Forms.PaintEventHandler(this.grpSections_Paint);
            // 
            // bntOK
            // 
            this.bntOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bntOK.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.bntOK.Image = ((System.Drawing.Image)(resources.GetObject("bntOK.Image")));
            this.bntOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.bntOK.Location = new System.Drawing.Point(156, 219);
            this.bntOK.Name = "bntOK";
            this.bntOK.Size = new System.Drawing.Size(115, 72);
            this.bntOK.TabIndex = 156;
            this.bntOK.Text = "Close";
            this.bntOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.bntOK.UseVisualStyleBackColor = true;
            // 
            // frmSimulation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(285, 302);
            this.Controls.Add(this.bntOK);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSimulation";
            this.ShowInTaskbar = false;
            this.Text = "Simulation";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmSimulation_FormClosed);
            this.Load += new System.EventHandler(this.frmSimulation_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox tbSpeed;
        private System.Windows.Forms.RadioButton rbRate;
        private System.Windows.Forms.RadioButton rbSpeed;
        private System.Windows.Forms.Label lbMPH;
        private System.Windows.Forms.RadioButton rbOff;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label swMasterOff;
        private System.Windows.Forms.Label swDown;
        private System.Windows.Forms.Label swUp;
        private System.Windows.Forms.Label swAuto;
        private System.Windows.Forms.Label swFour;
        private System.Windows.Forms.Label swThree;
        private System.Windows.Forms.Label swTwo;
        private System.Windows.Forms.Label swOne;
        private System.Windows.Forms.Label swMasterOn;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button bntOK;
    }
}