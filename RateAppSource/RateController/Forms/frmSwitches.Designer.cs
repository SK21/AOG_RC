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
            this.btnMaster = new RateController.Classes.RoundedButton();
            this.btnPrime = new RateController.Classes.RoundedButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnDown = new RateController.Classes.RoundedButton();
            this.btnUp = new RateController.Classes.RoundedButton();
            this.btnAutoRate = new RateController.Classes.RoundedButton();
            this.btnAutoSection = new RateController.Classes.RoundedButton();
            this.SuspendLayout();
            // 
            // btnMaster
            // 
            this.btnMaster.CornerRadius = 8;
            this.btnMaster.FlatAppearance.BorderSize = 0;
            this.btnMaster.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMaster.Location = new System.Drawing.Point(12, 12);
            this.btnMaster.Name = "btnMaster";
            this.btnMaster.Size = new System.Drawing.Size(64, 46);
            this.btnMaster.TabIndex = 167;
            this.btnMaster.Text = "MST";
            this.btnMaster.UseVisualStyleBackColor = false;
            this.btnMaster.Click += new System.EventHandler(this.btnMaster_Click);
            this.btnMaster.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button_MouseDown);
            this.btnMaster.MouseMove += new System.Windows.Forms.MouseEventHandler(this.button_MouseMove);
            this.btnMaster.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnMaster_MouseUp);
            this.btnMaster.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmSwitches_MouseUp);
            //
            // btnPrime
            // 
            this.btnPrime.CornerRadius = 8;
            this.btnPrime.FlatAppearance.BorderSize = 0;
            this.btnPrime.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrime.Location = new System.Drawing.Point(90, 12);
            this.btnPrime.Name = "btnPrime";
            this.btnPrime.Size = new System.Drawing.Size(64, 46);
            this.btnPrime.TabIndex = 164;
            this.btnPrime.Text = "PRM";
            this.btnPrime.UseVisualStyleBackColor = true;
            this.btnPrime.Click += new System.EventHandler(this.btnPrime_Click);
            this.btnPrime.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button_MouseDown);
            this.btnPrime.MouseMove += new System.Windows.Forms.MouseEventHandler(this.button_MouseMove);
            this.btnPrime.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmSwitches_MouseUp);
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnDown
            // 
            this.btnDown.CornerRadius = 8;
            this.btnDown.FlatAppearance.BorderSize = 0;
            this.btnDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDown.Location = new System.Drawing.Point(246, 12);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(64, 46);
            this.btnDown.TabIndex = 166;
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnDown_MouseDown);
            this.btnDown.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button_MouseDown);
            this.btnDown.MouseMove += new System.Windows.Forms.MouseEventHandler(this.button_MouseMove);
            this.btnDown.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnDown_MouseUp);
            this.btnDown.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmSwitches_MouseUp);
            // 
            // btnUp
            // 
            this.btnUp.CornerRadius = 8;
            this.btnUp.FlatAppearance.BorderSize = 0;
            this.btnUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUp.Location = new System.Drawing.Point(168, 12);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(64, 46);
            this.btnUp.TabIndex = 165;
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnUp_MouseDown);
            this.btnUp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button_MouseDown);
            this.btnUp.MouseMove += new System.Windows.Forms.MouseEventHandler(this.button_MouseMove);
            this.btnUp.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnUp_MouseUp);
            this.btnUp.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmSwitches_MouseUp);
            // 
            // btnAutoRate
            // 
            this.btnAutoRate.CornerRadius = 8;
            this.btnAutoRate.FlatAppearance.BorderSize = 0;
            this.btnAutoRate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAutoRate.Location = new System.Drawing.Point(12, 184);
            this.btnAutoRate.Name = "btnAutoRate";
            this.btnAutoRate.Size = new System.Drawing.Size(142, 46);
            this.btnAutoRate.TabIndex = 168;
            this.btnAutoRate.Text = "Auto Rate";
            this.btnAutoRate.UseVisualStyleBackColor = true;
            this.btnAutoRate.Click += new System.EventHandler(this.btnAutoRate_Click);
            this.btnAutoRate.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button_MouseDown);
            this.btnAutoRate.MouseMove += new System.Windows.Forms.MouseEventHandler(this.button_MouseMove);
            this.btnAutoRate.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmSwitches_MouseUp);
            // 
            // btnAutoSection
            // 
            this.btnAutoSection.CornerRadius = 8;
            this.btnAutoSection.FlatAppearance.BorderSize = 0;
            this.btnAutoSection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAutoSection.Location = new System.Drawing.Point(168, 184);
            this.btnAutoSection.Name = "btnAutoSection";
            this.btnAutoSection.Size = new System.Drawing.Size(142, 46);
            this.btnAutoSection.TabIndex = 169;
            this.btnAutoSection.Text = "Auto Section";
            this.btnAutoSection.UseVisualStyleBackColor = true;
            this.btnAutoSection.Click += new System.EventHandler(this.btnAutoSection_Click);
            this.btnAutoSection.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button_MouseDown);
            this.btnAutoSection.MouseMove += new System.Windows.Forms.MouseEventHandler(this.button_MouseMove);
            this.btnAutoSection.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmSwitches_MouseUp);
            //
            // frmSwitches
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(323, 239);
            this.Controls.Add(this.btnAutoSection);
            this.Controls.Add(this.btnAutoRate);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnMaster);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnPrime);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSwitches";
            this.ShowInTaskbar = false;
            this.Text = "Switches";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmSwitches_Closed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSwitches_FormClosing);
            this.Load += new System.EventHandler(this.frmSwitches_Load);
            this.LocationChanged += new System.EventHandler(this.frmSwitches_LocationChanged);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMove_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmSwitches_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
        private Classes.RoundedButton btnDown;
        private Classes.RoundedButton btnUp;
        private Classes.RoundedButton btnPrime;
        private System.Windows.Forms.Timer timer1;
        private Classes.RoundedButton btnMaster;
        private Classes.RoundedButton btnAutoRate;
        private Classes.RoundedButton btnAutoSection;
    }
}