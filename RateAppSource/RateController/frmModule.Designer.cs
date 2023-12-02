namespace RateController
{
    partial class frmModule
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmModule));
            this.tbEthernet = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lbTime = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbWifi = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lbModID = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbInoID = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lbIP = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tbSerial = new System.Windows.Forms.TextBox();
            this.bntOK = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.PortName = new System.Windows.Forms.Label();
            this.cboPort1 = new System.Windows.Forms.ComboBox();
            this.lb3 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnConnect1 = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbEthernet
            // 
            this.tbEthernet.Location = new System.Drawing.Point(11, 11);
            this.tbEthernet.Margin = new System.Windows.Forms.Padding(6);
            this.tbEthernet.Multiline = true;
            this.tbEthernet.Name = "tbEthernet";
            this.tbEthernet.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbEthernet.Size = new System.Drawing.Size(232, 309);
            this.tbEthernet.TabIndex = 221;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(15, 15);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(6);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(478, 369);
            this.tabControl1.TabIndex = 222;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.label12);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.tbEthernet);
            this.tabPage1.Location = new System.Drawing.Point(4, 33);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(6);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(6);
            this.tabPage1.Size = new System.Drawing.Size(470, 332);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "UDP";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnConnect1);
            this.tabPage2.Controls.Add(this.PortName);
            this.tabPage2.Controls.Add(this.cboPort1);
            this.tabPage2.Controls.Add(this.lb3);
            this.tabPage2.Controls.Add(this.tbSerial);
            this.tabPage2.Location = new System.Drawing.Point(4, 33);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(6);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(6);
            this.tabPage2.Size = new System.Drawing.Size(470, 332);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Serial";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // lbTime
            // 
            this.lbTime.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTime.Location = new System.Drawing.Point(192, 449);
            this.lbTime.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbTime.Name = "lbTime";
            this.lbTime.Size = new System.Drawing.Size(96, 23);
            this.lbTime.TabIndex = 242;
            this.lbTime.Text = "0";
            this.lbTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(19, 449);
            this.label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(118, 23);
            this.label4.TabIndex = 241;
            this.label4.Text = "Module Time";
            // 
            // lbWifi
            // 
            this.lbWifi.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbWifi.Location = new System.Drawing.Point(161, 507);
            this.lbWifi.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbWifi.Name = "lbWifi";
            this.lbWifi.Size = new System.Drawing.Size(159, 23);
            this.lbWifi.TabIndex = 240;
            this.lbWifi.Text = "192.168.255.255";
            this.lbWifi.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(19, 507);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 23);
            this.label3.TabIndex = 239;
            this.label3.Text = "Wifi IP";
            // 
            // lbModID
            // 
            this.lbModID.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbModID.Location = new System.Drawing.Point(192, 391);
            this.lbModID.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbModID.Name = "lbModID";
            this.lbModID.Size = new System.Drawing.Size(96, 23);
            this.lbModID.TabIndex = 238;
            this.lbModID.Text = "0";
            this.lbModID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(19, 391);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 23);
            this.label2.TabIndex = 237;
            this.label2.Text = "Module ID";
            // 
            // lbInoID
            // 
            this.lbInoID.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbInoID.Location = new System.Drawing.Point(192, 420);
            this.lbInoID.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbInoID.Name = "lbInoID";
            this.lbInoID.Size = new System.Drawing.Size(96, 23);
            this.lbInoID.TabIndex = 236;
            this.lbInoID.Text = "0";
            this.lbInoID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(19, 420);
            this.label6.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(138, 23);
            this.label6.TabIndex = 235;
            this.label6.Text = "Module Version";
            // 
            // lbIP
            // 
            this.lbIP.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbIP.Location = new System.Drawing.Point(161, 478);
            this.lbIP.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbIP.Name = "lbIP";
            this.lbIP.Size = new System.Drawing.Size(159, 23);
            this.lbIP.TabIndex = 234;
            this.lbIP.Text = "-.-.-.-";
            this.lbIP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label27.Location = new System.Drawing.Point(19, 478);
            this.label27.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(105, 23);
            this.label27.TabIndex = 233;
            this.label27.Text = "Ethernet IP";
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // tbSerial
            // 
            this.tbSerial.Location = new System.Drawing.Point(10, 12);
            this.tbSerial.Margin = new System.Windows.Forms.Padding(6);
            this.tbSerial.Multiline = true;
            this.tbSerial.Name = "tbSerial";
            this.tbSerial.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbSerial.Size = new System.Drawing.Size(448, 265);
            this.tbSerial.TabIndex = 222;
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
            this.bntOK.Location = new System.Drawing.Point(375, 458);
            this.bntOK.Margin = new System.Windows.Forms.Padding(6);
            this.bntOK.Name = "bntOK";
            this.bntOK.Size = new System.Drawing.Size(115, 72);
            this.bntOK.TabIndex = 210;
            this.bntOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.bntOK.UseVisualStyleBackColor = false;
            this.bntOK.Click += new System.EventHandler(this.bntOK_Click);
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.Transparent;
            this.btnStart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnStart.Enabled = false;
            this.btnStart.FlatAppearance.BorderSize = 0;
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnStart.Image = global::RateController.Properties.Resources.Start;
            this.btnStart.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnStart.Location = new System.Drawing.Point(378, 386);
            this.btnStart.Margin = new System.Windows.Forms.Padding(6);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(115, 72);
            this.btnStart.TabIndex = 243;
            this.btnStart.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.Color.Transparent;
            this.btnStop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnStop.FlatAppearance.BorderSize = 0;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnStop.Image = global::RateController.Properties.Resources.Stop;
            this.btnStop.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnStop.Location = new System.Drawing.Point(269, 386);
            this.btnStop.Margin = new System.Windows.Forms.Padding(6);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(115, 72);
            this.btnStop.TabIndex = 244;
            this.btnStop.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // PortName
            // 
            this.PortName.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PortName.Location = new System.Drawing.Point(186, 294);
            this.PortName.Name = "PortName";
            this.PortName.Size = new System.Drawing.Size(95, 23);
            this.PortName.TabIndex = 225;
            // 
            // cboPort1
            // 
            this.cboPort1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPort1.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold);
            this.cboPort1.FormattingEnabled = true;
            this.cboPort1.Items.AddRange(new object[] {
            "1",
            "2",
            "3"});
            this.cboPort1.Location = new System.Drawing.Point(114, 286);
            this.cboPort1.Name = "cboPort1";
            this.cboPort1.Size = new System.Drawing.Size(47, 37);
            this.cboPort1.TabIndex = 224;
            this.cboPort1.SelectedIndexChanged += new System.EventHandler(this.cboPort1_SelectedIndexChanged);
            // 
            // lb3
            // 
            this.lb3.AutoSize = true;
            this.lb3.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb3.Location = new System.Drawing.Point(13, 294);
            this.lb3.Name = "lb3";
            this.lb3.Size = new System.Drawing.Size(95, 23);
            this.lb3.TabIndex = 223;
            this.lb3.Text = "Serial Port";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(255, 294);
            this.label11.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(193, 23);
            this.label11.TabIndex = 253;
            this.label11.Text = "32621   pressure data";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(255, 254);
            this.label12.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(164, 23);
            this.label12.TabIndex = 252;
            this.label12.Text = "32620   switch IDs";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(255, 214);
            this.label8.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(166, 23);
            this.label8.TabIndex = 251;
            this.label8.Text = "32618   switch box";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(255, 174);
            this.label9.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(178, 23);
            this.label9.TabIndex = 250;
            this.label9.Text = "32616   PID settings";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(255, 134);
            this.label10.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(181, 23);
            this.label10.TabIndex = 249;
            this.label10.Text = "32614   rate settings";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(255, 94);
            this.label7.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(178, 23);
            this.label7.TabIndex = 248;
            this.label7.Text = "32613   rate applied";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(255, 54);
            this.label5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(195, 23);
            this.label5.TabIndex = 247;
            this.label5.Text = "32504   module status";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(255, 14);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(203, 23);
            this.label1.TabIndex = 246;
            this.label1.Text = "32503   subnet change";
            // 
            // btnConnect1
            // 
            this.btnConnect1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnConnect1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnConnect1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConnect1.Location = new System.Drawing.Point(310, 286);
            this.btnConnect1.Name = "btnConnect1";
            this.btnConnect1.Size = new System.Drawing.Size(127, 37);
            this.btnConnect1.TabIndex = 226;
            this.btnConnect1.Text = "Connect";
            this.btnConnect1.UseVisualStyleBackColor = false;
            this.btnConnect1.Click += new System.EventHandler(this.btnConnect1_Click);
            // 
            // frmModule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(505, 540);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.lbTime);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lbWifi);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbModID);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbInoID);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lbIP);
            this.Controls.Add(this.label27);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.bntOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmModule";
            this.Text = "Comm Diagnostics";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmModule_FormClosed);
            this.Load += new System.EventHandler(this.frmModule_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bntOK;
        private System.Windows.Forms.TextBox tbEthernet;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label lbTime;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbWifi;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbModID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbInoID;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lbIP;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox tbSerial;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label PortName;
        private System.Windows.Forms.ComboBox cboPort1;
        private System.Windows.Forms.Label lb3;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnConnect1;
    }
}