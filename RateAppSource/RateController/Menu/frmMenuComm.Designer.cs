namespace RateController.Menu
{
    partial class frmMenuComm
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
            this.gbxDrivers = new System.Windows.Forms.GroupBox();
            this.rbAdapter1 = new System.Windows.Forms.RadioButton();
            this.rbAdapter3 = new System.Windows.Forms.RadioButton();
            this.rbAdapter2 = new System.Windows.Forms.RadioButton();
            this.gbxPort = new System.Windows.Forms.GroupBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.cbComPort = new System.Windows.Forms.ComboBox();
            this.lbDriverFound = new System.Windows.Forms.Label();
            this.lbConnected = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbCAN = new System.Windows.Forms.RadioButton();
            this.rbEthernet = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.gbxDrivers.SuspendLayout();
            this.gbxPort.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
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
            this.btnCancel.Margin = new System.Windows.Forms.Padding(6);
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
            this.btnOK.Margin = new System.Windows.Forms.Padding(6);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(70, 63);
            this.btnOK.TabIndex = 163;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // gbxDrivers
            // 
            this.gbxDrivers.Controls.Add(this.rbAdapter1);
            this.gbxDrivers.Controls.Add(this.rbAdapter3);
            this.gbxDrivers.Controls.Add(this.rbAdapter2);
            this.gbxDrivers.Location = new System.Drawing.Point(19, 17);
            this.gbxDrivers.Name = "gbxDrivers";
            this.gbxDrivers.Size = new System.Drawing.Size(250, 223);
            this.gbxDrivers.TabIndex = 348;
            this.gbxDrivers.TabStop = false;
            this.gbxDrivers.Text = "Can Driver";
            // 
            // rbAdapter1
            // 
            this.rbAdapter1.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbAdapter1.Checked = true;
            this.rbAdapter1.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbAdapter1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbAdapter1.Location = new System.Drawing.Point(31, 41);
            this.rbAdapter1.Name = "rbAdapter1";
            this.rbAdapter1.Size = new System.Drawing.Size(187, 36);
            this.rbAdapter1.TabIndex = 343;
            this.rbAdapter1.TabStop = true;
            this.rbAdapter1.Text = "SLCAN";
            this.rbAdapter1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbAdapter1.UseVisualStyleBackColor = true;
            // 
            // rbAdapter3
            // 
            this.rbAdapter3.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbAdapter3.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbAdapter3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbAdapter3.Location = new System.Drawing.Point(31, 157);
            this.rbAdapter3.Name = "rbAdapter3";
            this.rbAdapter3.Size = new System.Drawing.Size(187, 36);
            this.rbAdapter3.TabIndex = 346;
            this.rbAdapter3.Text = "PCAN";
            this.rbAdapter3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbAdapter3.UseVisualStyleBackColor = true;
            // 
            // rbAdapter2
            // 
            this.rbAdapter2.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbAdapter2.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbAdapter2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbAdapter2.Location = new System.Drawing.Point(31, 99);
            this.rbAdapter2.Name = "rbAdapter2";
            this.rbAdapter2.Size = new System.Drawing.Size(187, 36);
            this.rbAdapter2.TabIndex = 345;
            this.rbAdapter2.Text = "InnoMaker";
            this.rbAdapter2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbAdapter2.UseVisualStyleBackColor = true;
            // 
            // gbxPort
            // 
            this.gbxPort.Controls.Add(this.btnRefresh);
            this.gbxPort.Controls.Add(this.cbComPort);
            this.gbxPort.Location = new System.Drawing.Point(337, 17);
            this.gbxPort.Name = "gbxPort";
            this.gbxPort.Size = new System.Drawing.Size(119, 149);
            this.gbxPort.TabIndex = 358;
            this.gbxPort.TabStop = false;
            this.gbxPort.Text = "Port";
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.Transparent;
            this.btnRefresh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnRefresh.Image = global::RateController.Properties.Resources.Update;
            this.btnRefresh.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnRefresh.Location = new System.Drawing.Point(20, 74);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(6);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(70, 63);
            this.btnRefresh.TabIndex = 356;
            this.btnRefresh.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnRefresh.UseVisualStyleBackColor = false;
            // 
            // cbComPort
            // 
            this.cbComPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbComPort.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.cbComPort.FormattingEnabled = true;
            this.cbComPort.Location = new System.Drawing.Point(9, 34);
            this.cbComPort.Name = "cbComPort";
            this.cbComPort.Size = new System.Drawing.Size(100, 31);
            this.cbComPort.TabIndex = 353;
            // 
            // lbDriverFound
            // 
            this.lbDriverFound.BackColor = System.Drawing.SystemColors.Control;
            this.lbDriverFound.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDriverFound.Image = global::RateController.Properties.Resources.Off;
            this.lbDriverFound.Location = new System.Drawing.Point(333, 261);
            this.lbDriverFound.Name = "lbDriverFound";
            this.lbDriverFound.Size = new System.Drawing.Size(41, 37);
            this.lbDriverFound.TabIndex = 362;
            this.lbDriverFound.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbConnected
            // 
            this.lbConnected.BackColor = System.Drawing.SystemColors.Control;
            this.lbConnected.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbConnected.Image = global::RateController.Properties.Resources.Off;
            this.lbConnected.Location = new System.Drawing.Point(333, 307);
            this.lbConnected.Name = "lbConnected";
            this.lbConnected.Size = new System.Drawing.Size(41, 37);
            this.lbConnected.TabIndex = 360;
            this.lbConnected.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(78, 268);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(187, 23);
            this.label2.TabIndex = 361;
            this.label2.Text = "CanBus Driver Found";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(78, 314);
            this.label13.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(234, 23);
            this.label13.TabIndex = 359;
            this.label13.Text = "CanBus Module Connected";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbCAN);
            this.groupBox1.Controls.Add(this.rbEthernet);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(105, 38);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(324, 146);
            this.groupBox1.TabIndex = 363;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Communication Mode";
            // 
            // rbCAN
            // 
            this.rbCAN.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbCAN.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbCAN.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbCAN.Location = new System.Drawing.Point(71, 92);
            this.rbCAN.Name = "rbCAN";
            this.rbCAN.Size = new System.Drawing.Size(187, 36);
            this.rbCAN.TabIndex = 345;
            this.rbCAN.Text = "CanBus";
            this.rbCAN.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbCAN.UseVisualStyleBackColor = true;
            // 
            // rbEthernet
            // 
            this.rbEthernet.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbEthernet.Checked = true;
            this.rbEthernet.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbEthernet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbEthernet.Location = new System.Drawing.Point(71, 38);
            this.rbEthernet.Name = "rbEthernet";
            this.rbEthernet.Size = new System.Drawing.Size(187, 36);
            this.rbEthernet.TabIndex = 344;
            this.rbEthernet.TabStop = true;
            this.rbEthernet.Text = "Ethernet";
            this.rbEthernet.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbEthernet.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.gbxDrivers);
            this.panel1.Controls.Add(this.gbxPort);
            this.panel1.Controls.Add(this.lbDriverFound);
            this.panel1.Controls.Add(this.lbConnected);
            this.panel1.Controls.Add(this.label13);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(24, 205);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(468, 389);
            this.panel1.TabIndex = 364;
            // 
            // frmMenuComm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 678);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "frmMenuComm";
            this.Text = "frmMenuComm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMenuComm_FormClosing);
            this.Load += new System.EventHandler(this.frmMenuComm_Load);
            this.gbxDrivers.ResumeLayout(false);
            this.gbxPort.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox gbxDrivers;
        private System.Windows.Forms.RadioButton rbAdapter1;
        private System.Windows.Forms.RadioButton rbAdapter3;
        private System.Windows.Forms.RadioButton rbAdapter2;
        private System.Windows.Forms.GroupBox gbxPort;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.ComboBox cbComPort;
        private System.Windows.Forms.Label lbDriverFound;
        private System.Windows.Forms.Label lbConnected;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbCAN;
        private System.Windows.Forms.RadioButton rbEthernet;
        private System.Windows.Forms.Panel panel1;
    }
}