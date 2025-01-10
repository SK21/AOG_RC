namespace RateController.Menu
{
    partial class frmMenuNetwork
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
            this.lbIP = new System.Windows.Forms.Label();
            this.lbModuleIP = new System.Windows.Forms.Label();
            this.lbSubnet = new System.Windows.Forms.Label();
            this.cbEthernet = new System.Windows.Forms.ComboBox();
            this.btnSendSubnet = new System.Windows.Forms.Button();
            this.btnRescan = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ckDefaultModule = new System.Windows.Forms.CheckBox();
            this.rbNano = new System.Windows.Forms.RadioButton();
            this.rbESP32 = new System.Windows.Forms.RadioButton();
            this.rbTeensy = new System.Windows.Forms.RadioButton();
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
            // lbIP
            // 
            this.lbIP.AutoSize = true;
            this.lbIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbIP.Location = new System.Drawing.Point(26, 74);
            this.lbIP.Name = "lbIP";
            this.lbIP.Size = new System.Drawing.Size(76, 24);
            this.lbIP.TabIndex = 223;
            this.lbIP.Text = "Local IP";
            // 
            // lbModuleIP
            // 
            this.lbModuleIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbModuleIP.Location = new System.Drawing.Point(191, 31);
            this.lbModuleIP.Name = "lbModuleIP";
            this.lbModuleIP.Size = new System.Drawing.Size(161, 24);
            this.lbModuleIP.TabIndex = 222;
            this.lbModuleIP.Text = "192.168.100.100";
            this.lbModuleIP.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbSubnet
            // 
            this.lbSubnet.AutoSize = true;
            this.lbSubnet.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSubnet.Location = new System.Drawing.Point(26, 31);
            this.lbSubnet.Name = "lbSubnet";
            this.lbSubnet.Size = new System.Drawing.Size(149, 24);
            this.lbSubnet.TabIndex = 221;
            this.lbSubnet.Text = "Selected Subnet";
            // 
            // cbEthernet
            // 
            this.cbEthernet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEthernet.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbEthernet.FormattingEnabled = true;
            this.cbEthernet.Location = new System.Drawing.Point(195, 71);
            this.cbEthernet.Name = "cbEthernet";
            this.cbEthernet.Size = new System.Drawing.Size(157, 32);
            this.cbEthernet.TabIndex = 220;
            this.cbEthernet.SelectedIndexChanged += new System.EventHandler(this.cbEthernet_SelectedIndexChanged);
            // 
            // btnSendSubnet
            // 
            this.btnSendSubnet.BackColor = System.Drawing.Color.Transparent;
            this.btnSendSubnet.FlatAppearance.BorderSize = 0;
            this.btnSendSubnet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSendSubnet.Image = global::RateController.Properties.Resources.Update4;
            this.btnSendSubnet.Location = new System.Drawing.Point(219, 127);
            this.btnSendSubnet.Name = "btnSendSubnet";
            this.btnSendSubnet.Size = new System.Drawing.Size(72, 72);
            this.btnSendSubnet.TabIndex = 219;
            this.btnSendSubnet.UseVisualStyleBackColor = false;
            this.btnSendSubnet.Click += new System.EventHandler(this.btnSendSubnet_Click);
            // 
            // btnRescan
            // 
            this.btnRescan.BackColor = System.Drawing.Color.Transparent;
            this.btnRescan.FlatAppearance.BorderSize = 0;
            this.btnRescan.FlatAppearance.MouseDownBackColor = System.Drawing.Color.LightGreen;
            this.btnRescan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRescan.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRescan.Image = global::RateController.Properties.Resources.Update;
            this.btnRescan.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnRescan.Location = new System.Drawing.Point(97, 127);
            this.btnRescan.Name = "btnRescan";
            this.btnRescan.Size = new System.Drawing.Size(72, 72);
            this.btnRescan.TabIndex = 218;
            this.btnRescan.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnRescan.UseVisualStyleBackColor = false;
            this.btnRescan.Click += new System.EventHandler(this.btnRescan_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ckDefaultModule);
            this.groupBox1.Controls.Add(this.rbNano);
            this.groupBox1.Controls.Add(this.rbESP32);
            this.groupBox1.Controls.Add(this.rbTeensy);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(62, 331);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(398, 200);
            this.groupBox1.TabIndex = 224;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Board";
            this.groupBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox2_Paint);
            // 
            // ckDefaultModule
            // 
            this.ckDefaultModule.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckDefaultModule.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.ckDefaultModule.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckDefaultModule.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckDefaultModule.Image = global::RateController.Properties.Resources.VehFileLoad;
            this.ckDefaultModule.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.ckDefaultModule.Location = new System.Drawing.Point(254, 51);
            this.ckDefaultModule.Name = "ckDefaultModule";
            this.ckDefaultModule.Size = new System.Drawing.Size(98, 112);
            this.ckDefaultModule.TabIndex = 226;
            this.ckDefaultModule.Text = "Load Defaults";
            this.ckDefaultModule.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.ckDefaultModule.UseVisualStyleBackColor = true;
            this.ckDefaultModule.CheckedChanged += new System.EventHandler(this.ckDefaultModule_CheckedChanged);
            // 
            // rbNano
            // 
            this.rbNano.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbNano.Checked = true;
            this.rbNano.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbNano.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbNano.Location = new System.Drawing.Point(58, 28);
            this.rbNano.Name = "rbNano";
            this.rbNano.Size = new System.Drawing.Size(170, 37);
            this.rbNano.TabIndex = 0;
            this.rbNano.TabStop = true;
            this.rbNano.Text = "Nano (RC12)";
            this.rbNano.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbNano.UseVisualStyleBackColor = true;
            this.rbNano.CheckedChanged += new System.EventHandler(this.rbNano_CheckedChanged);
            // 
            // rbESP32
            // 
            this.rbESP32.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbESP32.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbESP32.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbESP32.Location = new System.Drawing.Point(58, 150);
            this.rbESP32.Name = "rbESP32";
            this.rbESP32.Size = new System.Drawing.Size(170, 37);
            this.rbESP32.TabIndex = 46;
            this.rbESP32.Text = "ESP32 (RC15)";
            this.rbESP32.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbESP32.UseVisualStyleBackColor = true;
            this.rbESP32.CheckedChanged += new System.EventHandler(this.rbESP32_CheckedChanged);
            // 
            // rbTeensy
            // 
            this.rbTeensy.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbTeensy.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbTeensy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbTeensy.Location = new System.Drawing.Point(58, 89);
            this.rbTeensy.Name = "rbTeensy";
            this.rbTeensy.Size = new System.Drawing.Size(170, 37);
            this.rbTeensy.TabIndex = 45;
            this.rbTeensy.Text = "Teensy (RC11)";
            this.rbTeensy.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbTeensy.UseVisualStyleBackColor = true;
            this.rbTeensy.CheckedChanged += new System.EventHandler(this.rbTeensy_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbEthernet);
            this.groupBox2.Controls.Add(this.lbSubnet);
            this.groupBox2.Controls.Add(this.btnSendSubnet);
            this.groupBox2.Controls.Add(this.btnRescan);
            this.groupBox2.Controls.Add(this.lbIP);
            this.groupBox2.Controls.Add(this.lbModuleIP);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(62, 74);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(398, 215);
            this.groupBox2.TabIndex = 225;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Network";
            this.groupBox2.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox2_Paint);
            // 
            // frmMenuNetwork
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 630);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMenuNetwork";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuNetwork";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMenuNetwork_FormClosed);
            this.Load += new System.EventHandler(this.frmMenuNetwork_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lbIP;
        private System.Windows.Forms.Label lbModuleIP;
        private System.Windows.Forms.Label lbSubnet;
        private System.Windows.Forms.ComboBox cbEthernet;
        private System.Windows.Forms.Button btnSendSubnet;
        private System.Windows.Forms.Button btnRescan;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbNano;
        private System.Windows.Forms.RadioButton rbESP32;
        private System.Windows.Forms.RadioButton rbTeensy;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox ckDefaultModule;
    }
}