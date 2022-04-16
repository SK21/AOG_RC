namespace RateController
{
    partial class frmNanoSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNanoSettings));
            this.btnSendToModule = new System.Windows.Forms.Button();
            this.btnLoadDefaults = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.bntOK = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label7 = new System.Windows.Forms.Label();
            this.tbRelay16 = new System.Windows.Forms.TextBox();
            this.tbRelay15 = new System.Windows.Forms.TextBox();
            this.tbRelay14 = new System.Windows.Forms.TextBox();
            this.tbRelay5 = new System.Windows.Forms.TextBox();
            this.tbRelay4 = new System.Windows.Forms.TextBox();
            this.tbRelay3 = new System.Windows.Forms.TextBox();
            this.tbRelay13 = new System.Windows.Forms.TextBox();
            this.tbRelay12 = new System.Windows.Forms.TextBox();
            this.tbRelay2 = new System.Windows.Forms.TextBox();
            this.tbRelay11 = new System.Windows.Forms.TextBox();
            this.tbRelay10 = new System.Windows.Forms.TextBox();
            this.tbRelay6 = new System.Windows.Forms.TextBox();
            this.tbRelay8 = new System.Windows.Forms.TextBox();
            this.tbRelay9 = new System.Windows.Forms.TextBox();
            this.tbNanoDir2 = new System.Windows.Forms.TextBox();
            this.tbRelay7 = new System.Windows.Forms.TextBox();
            this.tbNanoDir1 = new System.Windows.Forms.TextBox();
            this.tbRelay1 = new System.Windows.Forms.TextBox();
            this.tbNanoPWM2 = new System.Windows.Forms.TextBox();
            this.tbNanoPWM1 = new System.Windows.Forms.TextBox();
            this.tbNanoFlow2 = new System.Windows.Forms.TextBox();
            this.tbNanoFlow1 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.ckUseMCP23017 = new System.Windows.Forms.CheckBox();
            this.ckNanoFlowOn = new System.Windows.Forms.CheckBox();
            this.ckNanoRelayOn = new System.Windows.Forms.CheckBox();
            this.ckUseEthernet = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tbNanoSensorCount = new System.Windows.Forms.TextBox();
            this.tbNanoModuleID = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSendToModule
            // 
            this.btnSendToModule.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSendToModule.Location = new System.Drawing.Point(117, 456);
            this.btnSendToModule.Name = "btnSendToModule";
            this.btnSendToModule.Size = new System.Drawing.Size(100, 72);
            this.btnSendToModule.TabIndex = 143;
            this.btnSendToModule.Text = "Send to Module";
            this.btnSendToModule.UseVisualStyleBackColor = true;
            this.btnSendToModule.Click += new System.EventHandler(this.btnSendToModule_Click);
            // 
            // btnLoadDefaults
            // 
            this.btnLoadDefaults.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoadDefaults.Location = new System.Drawing.Point(11, 456);
            this.btnLoadDefaults.Name = "btnLoadDefaults";
            this.btnLoadDefaults.Size = new System.Drawing.Size(100, 72);
            this.btnLoadDefaults.TabIndex = 142;
            this.btnLoadDefaults.Text = "Load Defaults";
            this.btnLoadDefaults.UseVisualStyleBackColor = true;
            this.btnLoadDefaults.Click += new System.EventHandler(this.btnLoadDefaults_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnCancel.Enabled = false;
            this.btnCancel.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCancel.Location = new System.Drawing.Point(223, 456);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(115, 72);
            this.btnCancel.TabIndex = 141;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // bntOK
            // 
            this.bntOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bntOK.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.bntOK.Image = ((System.Drawing.Image)(resources.GetObject("bntOK.Image")));
            this.bntOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.bntOK.Location = new System.Drawing.Point(347, 456);
            this.bntOK.Name = "bntOK";
            this.bntOK.Size = new System.Drawing.Size(115, 72);
            this.bntOK.TabIndex = 140;
            this.bntOK.Text = "Close";
            this.bntOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.bntOK.UseVisualStyleBackColor = true;
            this.bntOK.Click += new System.EventHandler(this.bntOK_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.tbRelay16);
            this.tabPage2.Controls.Add(this.tbRelay15);
            this.tabPage2.Controls.Add(this.tbRelay14);
            this.tabPage2.Controls.Add(this.tbRelay5);
            this.tabPage2.Controls.Add(this.tbRelay4);
            this.tabPage2.Controls.Add(this.tbRelay3);
            this.tabPage2.Controls.Add(this.tbRelay13);
            this.tabPage2.Controls.Add(this.tbRelay12);
            this.tabPage2.Controls.Add(this.tbRelay2);
            this.tabPage2.Controls.Add(this.tbRelay11);
            this.tabPage2.Controls.Add(this.tbRelay10);
            this.tabPage2.Controls.Add(this.tbRelay6);
            this.tabPage2.Controls.Add(this.tbRelay8);
            this.tabPage2.Controls.Add(this.tbRelay9);
            this.tabPage2.Controls.Add(this.tbNanoDir2);
            this.tabPage2.Controls.Add(this.tbRelay7);
            this.tabPage2.Controls.Add(this.tbNanoDir1);
            this.tabPage2.Controls.Add(this.tbRelay1);
            this.tabPage2.Controls.Add(this.tbNanoPWM2);
            this.tabPage2.Controls.Add(this.tbNanoPWM1);
            this.tabPage2.Controls.Add(this.tbNanoFlow2);
            this.tabPage2.Controls.Add(this.tbNanoFlow1);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.label22);
            this.tabPage2.Controls.Add(this.label23);
            this.tabPage2.Controls.Add(this.label24);
            this.tabPage2.Controls.Add(this.label31);
            this.tabPage2.Controls.Add(this.label34);
            this.tabPage2.Controls.Add(this.label25);
            this.tabPage2.Controls.Add(this.label26);
            this.tabPage2.Controls.Add(this.label27);
            this.tabPage2.Controls.Add(this.label28);
            this.tabPage2.Controls.Add(this.label29);
            this.tabPage2.Controls.Add(this.label30);
            this.tabPage2.Controls.Add(this.label32);
            this.tabPage2.Controls.Add(this.label33);
            this.tabPage2.Location = new System.Drawing.Point(4, 33);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(446, 401);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Nano Pins";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(232, 370);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(81, 23);
            this.label7.TabIndex = 227;
            this.label7.Text = "Relay 16";
            // 
            // tbRelay16
            // 
            this.tbRelay16.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRelay16.Location = new System.Drawing.Point(322, 366);
            this.tbRelay16.MaxLength = 8;
            this.tbRelay16.Name = "tbRelay16";
            this.tbRelay16.Size = new System.Drawing.Size(50, 30);
            this.tbRelay16.TabIndex = 226;
            this.tbRelay16.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbRelay15
            // 
            this.tbRelay15.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRelay15.Location = new System.Drawing.Point(322, 330);
            this.tbRelay15.MaxLength = 8;
            this.tbRelay15.Name = "tbRelay15";
            this.tbRelay15.Size = new System.Drawing.Size(50, 30);
            this.tbRelay15.TabIndex = 224;
            this.tbRelay15.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbRelay14
            // 
            this.tbRelay14.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRelay14.Location = new System.Drawing.Point(322, 294);
            this.tbRelay14.MaxLength = 8;
            this.tbRelay14.Name = "tbRelay14";
            this.tbRelay14.Size = new System.Drawing.Size(50, 30);
            this.tbRelay14.TabIndex = 222;
            this.tbRelay14.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbRelay5
            // 
            this.tbRelay5.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRelay5.Location = new System.Drawing.Point(137, 366);
            this.tbRelay5.MaxLength = 8;
            this.tbRelay5.Name = "tbRelay5";
            this.tbRelay5.Size = new System.Drawing.Size(50, 30);
            this.tbRelay5.TabIndex = 220;
            this.tbRelay5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbRelay4
            // 
            this.tbRelay4.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRelay4.Location = new System.Drawing.Point(137, 330);
            this.tbRelay4.MaxLength = 8;
            this.tbRelay4.Name = "tbRelay4";
            this.tbRelay4.Size = new System.Drawing.Size(50, 30);
            this.tbRelay4.TabIndex = 218;
            this.tbRelay4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbRelay3
            // 
            this.tbRelay3.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRelay3.Location = new System.Drawing.Point(137, 294);
            this.tbRelay3.MaxLength = 8;
            this.tbRelay3.Name = "tbRelay3";
            this.tbRelay3.Size = new System.Drawing.Size(50, 30);
            this.tbRelay3.TabIndex = 216;
            this.tbRelay3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbRelay13
            // 
            this.tbRelay13.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRelay13.Location = new System.Drawing.Point(322, 258);
            this.tbRelay13.MaxLength = 8;
            this.tbRelay13.Name = "tbRelay13";
            this.tbRelay13.Size = new System.Drawing.Size(50, 30);
            this.tbRelay13.TabIndex = 214;
            this.tbRelay13.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbRelay12
            // 
            this.tbRelay12.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRelay12.Location = new System.Drawing.Point(322, 222);
            this.tbRelay12.MaxLength = 8;
            this.tbRelay12.Name = "tbRelay12";
            this.tbRelay12.Size = new System.Drawing.Size(50, 30);
            this.tbRelay12.TabIndex = 212;
            this.tbRelay12.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbRelay2
            // 
            this.tbRelay2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRelay2.Location = new System.Drawing.Point(137, 258);
            this.tbRelay2.MaxLength = 8;
            this.tbRelay2.Name = "tbRelay2";
            this.tbRelay2.Size = new System.Drawing.Size(50, 30);
            this.tbRelay2.TabIndex = 210;
            this.tbRelay2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbRelay11
            // 
            this.tbRelay11.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRelay11.Location = new System.Drawing.Point(322, 186);
            this.tbRelay11.MaxLength = 8;
            this.tbRelay11.Name = "tbRelay11";
            this.tbRelay11.Size = new System.Drawing.Size(50, 30);
            this.tbRelay11.TabIndex = 208;
            this.tbRelay11.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbRelay10
            // 
            this.tbRelay10.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRelay10.Location = new System.Drawing.Point(322, 150);
            this.tbRelay10.MaxLength = 8;
            this.tbRelay10.Name = "tbRelay10";
            this.tbRelay10.Size = new System.Drawing.Size(50, 30);
            this.tbRelay10.TabIndex = 206;
            this.tbRelay10.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbRelay6
            // 
            this.tbRelay6.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRelay6.Location = new System.Drawing.Point(322, 6);
            this.tbRelay6.MaxLength = 8;
            this.tbRelay6.Name = "tbRelay6";
            this.tbRelay6.Size = new System.Drawing.Size(50, 30);
            this.tbRelay6.TabIndex = 204;
            this.tbRelay6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbRelay8
            // 
            this.tbRelay8.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRelay8.Location = new System.Drawing.Point(322, 78);
            this.tbRelay8.MaxLength = 8;
            this.tbRelay8.Name = "tbRelay8";
            this.tbRelay8.Size = new System.Drawing.Size(50, 30);
            this.tbRelay8.TabIndex = 184;
            this.tbRelay8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbRelay9
            // 
            this.tbRelay9.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRelay9.Location = new System.Drawing.Point(322, 114);
            this.tbRelay9.MaxLength = 8;
            this.tbRelay9.Name = "tbRelay9";
            this.tbRelay9.Size = new System.Drawing.Size(50, 30);
            this.tbRelay9.TabIndex = 190;
            this.tbRelay9.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbNanoDir2
            // 
            this.tbNanoDir2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbNanoDir2.Location = new System.Drawing.Point(137, 114);
            this.tbNanoDir2.MaxLength = 8;
            this.tbNanoDir2.Name = "tbNanoDir2";
            this.tbNanoDir2.Size = new System.Drawing.Size(50, 30);
            this.tbNanoDir2.TabIndex = 202;
            this.tbNanoDir2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbRelay7
            // 
            this.tbRelay7.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRelay7.Location = new System.Drawing.Point(322, 42);
            this.tbRelay7.MaxLength = 8;
            this.tbRelay7.Name = "tbRelay7";
            this.tbRelay7.Size = new System.Drawing.Size(50, 30);
            this.tbRelay7.TabIndex = 200;
            this.tbRelay7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbNanoDir1
            // 
            this.tbNanoDir1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbNanoDir1.Location = new System.Drawing.Point(137, 78);
            this.tbNanoDir1.MaxLength = 8;
            this.tbNanoDir1.Name = "tbNanoDir1";
            this.tbNanoDir1.Size = new System.Drawing.Size(50, 30);
            this.tbNanoDir1.TabIndex = 198;
            this.tbNanoDir1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbRelay1
            // 
            this.tbRelay1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRelay1.Location = new System.Drawing.Point(137, 222);
            this.tbRelay1.MaxLength = 8;
            this.tbRelay1.Name = "tbRelay1";
            this.tbRelay1.Size = new System.Drawing.Size(50, 30);
            this.tbRelay1.TabIndex = 196;
            this.tbRelay1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbNanoPWM2
            // 
            this.tbNanoPWM2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbNanoPWM2.Location = new System.Drawing.Point(137, 186);
            this.tbNanoPWM2.MaxLength = 8;
            this.tbNanoPWM2.Name = "tbNanoPWM2";
            this.tbNanoPWM2.Size = new System.Drawing.Size(50, 30);
            this.tbNanoPWM2.TabIndex = 194;
            this.tbNanoPWM2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbNanoPWM1
            // 
            this.tbNanoPWM1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbNanoPWM1.Location = new System.Drawing.Point(137, 150);
            this.tbNanoPWM1.MaxLength = 8;
            this.tbNanoPWM1.Name = "tbNanoPWM1";
            this.tbNanoPWM1.Size = new System.Drawing.Size(50, 30);
            this.tbNanoPWM1.TabIndex = 192;
            this.tbNanoPWM1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbNanoFlow2
            // 
            this.tbNanoFlow2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbNanoFlow2.Location = new System.Drawing.Point(137, 42);
            this.tbNanoFlow2.MaxLength = 8;
            this.tbNanoFlow2.Name = "tbNanoFlow2";
            this.tbNanoFlow2.Size = new System.Drawing.Size(50, 30);
            this.tbNanoFlow2.TabIndex = 188;
            this.tbNanoFlow2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbNanoFlow1
            // 
            this.tbNanoFlow1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbNanoFlow1.Location = new System.Drawing.Point(137, 6);
            this.tbNanoFlow1.MaxLength = 8;
            this.tbNanoFlow1.Name = "tbNanoFlow1";
            this.tbNanoFlow1.Size = new System.Drawing.Size(50, 30);
            this.tbNanoFlow1.TabIndex = 186;
            this.tbNanoFlow1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(232, 334);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(81, 23);
            this.label8.TabIndex = 225;
            this.label8.Text = "Relay 15";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(232, 298);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(81, 23);
            this.label9.TabIndex = 223;
            this.label9.Text = "Relay 14";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(54, 370);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 23);
            this.label4.TabIndex = 221;
            this.label4.Text = "Relay 5";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(54, 334);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 23);
            this.label5.TabIndex = 219;
            this.label5.Text = "Relay 4";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(54, 298);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 23);
            this.label6.TabIndex = 217;
            this.label6.Text = "Relay 3";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(232, 262);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 23);
            this.label3.TabIndex = 215;
            this.label3.Text = "Relay 13";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(232, 226);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 23);
            this.label2.TabIndex = 213;
            this.label2.Text = "Relay 12";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(54, 262);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 23);
            this.label1.TabIndex = 211;
            this.label1.Text = "Relay 2";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.Location = new System.Drawing.Point(232, 190);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(81, 23);
            this.label22.TabIndex = 209;
            this.label22.Text = "Relay 11";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(232, 154);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(81, 23);
            this.label23.TabIndex = 207;
            this.label23.Text = "Relay 10";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.Location = new System.Drawing.Point(232, 10);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(71, 23);
            this.label24.TabIndex = 205;
            this.label24.Text = "Relay 6";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label31.Location = new System.Drawing.Point(232, 118);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(71, 23);
            this.label31.TabIndex = 191;
            this.label31.Text = "Relay 9";
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label34.Location = new System.Drawing.Point(232, 82);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(71, 23);
            this.label34.TabIndex = 185;
            this.label34.Text = "Relay 8";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(54, 118);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(50, 23);
            this.label25.TabIndex = 203;
            this.label25.Text = "Dir 2";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.Location = new System.Drawing.Point(232, 46);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(71, 23);
            this.label26.TabIndex = 201;
            this.label26.Text = "Relay 7";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label27.Location = new System.Drawing.Point(54, 82);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(50, 23);
            this.label27.TabIndex = 199;
            this.label27.Text = "Dir 1";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.Location = new System.Drawing.Point(54, 226);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(71, 23);
            this.label28.TabIndex = 197;
            this.label28.Text = "Relay 1";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label29.Location = new System.Drawing.Point(54, 190);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(68, 23);
            this.label29.TabIndex = 195;
            this.label29.Text = "PWM 2";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label30.Location = new System.Drawing.Point(54, 154);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(68, 23);
            this.label30.TabIndex = 193;
            this.label30.Text = "PWM 1";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.Location = new System.Drawing.Point(54, 46);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(64, 23);
            this.label32.TabIndex = 189;
            this.label32.Text = "Flow 2";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label33.Location = new System.Drawing.Point(54, 10);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(64, 23);
            this.label33.TabIndex = 187;
            this.label33.Text = "Flow 1";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.ckUseMCP23017);
            this.tabPage1.Controls.Add(this.ckNanoFlowOn);
            this.tabPage1.Controls.Add(this.ckNanoRelayOn);
            this.tabPage1.Controls.Add(this.ckUseEthernet);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.tbNanoSensorCount);
            this.tabPage1.Controls.Add(this.tbNanoModuleID);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPage1.Location = new System.Drawing.Point(4, 33);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(446, 401);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Config 1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // ckUseMCP23017
            // 
            this.ckUseMCP23017.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckUseMCP23017.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckUseMCP23017.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ckUseMCP23017.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckUseMCP23017.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckUseMCP23017.Location = new System.Drawing.Point(70, 247);
            this.ckUseMCP23017.Margin = new System.Windows.Forms.Padding(2);
            this.ckUseMCP23017.Name = "ckUseMCP23017";
            this.ckUseMCP23017.Size = new System.Drawing.Size(122, 60);
            this.ckUseMCP23017.TabIndex = 155;
            this.ckUseMCP23017.Text = "Use MCP23017";
            this.ckUseMCP23017.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckUseMCP23017.UseVisualStyleBackColor = true;
            this.ckUseMCP23017.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ckUseMCP23017_HelpRequested);
            // 
            // ckNanoFlowOn
            // 
            this.ckNanoFlowOn.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckNanoFlowOn.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckNanoFlowOn.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ckNanoFlowOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckNanoFlowOn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckNanoFlowOn.Location = new System.Drawing.Point(243, 247);
            this.ckNanoFlowOn.Margin = new System.Windows.Forms.Padding(2);
            this.ckNanoFlowOn.Name = "ckNanoFlowOn";
            this.ckNanoFlowOn.Size = new System.Drawing.Size(122, 60);
            this.ckNanoFlowOn.TabIndex = 154;
            this.ckNanoFlowOn.Text = "Flow on high";
            this.ckNanoFlowOn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckNanoFlowOn.UseVisualStyleBackColor = true;
            this.ckNanoFlowOn.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ckNanoFlowOn_HelpRequested);
            // 
            // ckNanoRelayOn
            // 
            this.ckNanoRelayOn.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckNanoRelayOn.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckNanoRelayOn.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ckNanoRelayOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckNanoRelayOn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckNanoRelayOn.Location = new System.Drawing.Point(243, 177);
            this.ckNanoRelayOn.Margin = new System.Windows.Forms.Padding(2);
            this.ckNanoRelayOn.Name = "ckNanoRelayOn";
            this.ckNanoRelayOn.Size = new System.Drawing.Size(122, 60);
            this.ckNanoRelayOn.TabIndex = 153;
            this.ckNanoRelayOn.Text = "Relay on high";
            this.ckNanoRelayOn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckNanoRelayOn.UseVisualStyleBackColor = true;
            this.ckNanoRelayOn.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ckNanoRelayOn_HelpRequested);
            // 
            // ckUseEthernet
            // 
            this.ckUseEthernet.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckUseEthernet.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckUseEthernet.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ckUseEthernet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckUseEthernet.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckUseEthernet.Location = new System.Drawing.Point(70, 177);
            this.ckUseEthernet.Margin = new System.Windows.Forms.Padding(2);
            this.ckUseEthernet.Name = "ckUseEthernet";
            this.ckUseEthernet.Size = new System.Drawing.Size(122, 60);
            this.ckUseEthernet.TabIndex = 152;
            this.ckUseEthernet.Text = "Use Ethernet";
            this.ckUseEthernet.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckUseEthernet.UseVisualStyleBackColor = true;
            this.ckUseEthernet.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ckUseEthernet_HelpRequested);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(70, 122);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(122, 23);
            this.label10.TabIndex = 131;
            this.label10.Text = "Sensor Count";
            // 
            // tbNanoSensorCount
            // 
            this.tbNanoSensorCount.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbNanoSensorCount.Location = new System.Drawing.Point(279, 120);
            this.tbNanoSensorCount.MaxLength = 8;
            this.tbNanoSensorCount.Name = "tbNanoSensorCount";
            this.tbNanoSensorCount.Size = new System.Drawing.Size(50, 30);
            this.tbNanoSensorCount.TabIndex = 130;
            this.tbNanoSensorCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbNanoSensorCount.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.tbNanoSensorCount_HelpRequested);
            // 
            // tbNanoModuleID
            // 
            this.tbNanoModuleID.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbNanoModuleID.Location = new System.Drawing.Point(279, 74);
            this.tbNanoModuleID.MaxLength = 8;
            this.tbNanoModuleID.Name = "tbNanoModuleID";
            this.tbNanoModuleID.Size = new System.Drawing.Size(50, 30);
            this.tbNanoModuleID.TabIndex = 128;
            this.tbNanoModuleID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(70, 77);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(97, 23);
            this.label11.TabIndex = 129;
            this.label11.Text = "Module ID";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(454, 438);
            this.tabControl1.TabIndex = 0;
            // 
            // frmNanoSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(475, 537);
            this.Controls.Add(this.btnSendToModule);
            this.Controls.Add(this.btnLoadDefaults);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.bntOK);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmNanoSettings";
            this.ShowInTaskbar = false;
            this.Text = "Nano Settings";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmNanoSettings_FormClosed);
            this.Load += new System.EventHandler(this.frmNanoSettings_Load);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnSendToModule;
        private System.Windows.Forms.Button btnLoadDefaults;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button bntOK;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbRelay16;
        private System.Windows.Forms.TextBox tbRelay15;
        private System.Windows.Forms.TextBox tbRelay14;
        private System.Windows.Forms.TextBox tbRelay5;
        private System.Windows.Forms.TextBox tbRelay4;
        private System.Windows.Forms.TextBox tbRelay3;
        private System.Windows.Forms.TextBox tbRelay13;
        private System.Windows.Forms.TextBox tbRelay12;
        private System.Windows.Forms.TextBox tbRelay2;
        private System.Windows.Forms.TextBox tbRelay11;
        private System.Windows.Forms.TextBox tbRelay10;
        private System.Windows.Forms.TextBox tbRelay6;
        private System.Windows.Forms.TextBox tbRelay8;
        private System.Windows.Forms.TextBox tbRelay9;
        private System.Windows.Forms.TextBox tbNanoDir2;
        private System.Windows.Forms.TextBox tbRelay7;
        private System.Windows.Forms.TextBox tbNanoDir1;
        private System.Windows.Forms.TextBox tbRelay1;
        private System.Windows.Forms.TextBox tbNanoPWM2;
        private System.Windows.Forms.TextBox tbNanoPWM1;
        private System.Windows.Forms.TextBox tbNanoFlow2;
        private System.Windows.Forms.TextBox tbNanoFlow1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.CheckBox ckUseMCP23017;
        private System.Windows.Forms.CheckBox ckNanoFlowOn;
        private System.Windows.Forms.CheckBox ckNanoRelayOn;
        private System.Windows.Forms.CheckBox ckUseEthernet;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbNanoSensorCount;
        private System.Windows.Forms.TextBox tbNanoModuleID;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TabControl tabControl1;
    }
}