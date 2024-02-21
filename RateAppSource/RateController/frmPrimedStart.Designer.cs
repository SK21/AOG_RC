namespace RateController
{
    partial class frmPrimedStart
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPrimedStart));
            this.lbSpeed = new System.Windows.Forms.Label();
            this.tbSpeed = new System.Windows.Forms.TextBox();
            this.lbPrimedSpeed = new System.Windows.Forms.Label();
            this.lbOnTime = new System.Windows.Forms.Label();
            this.tbTime = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lbOnSeconds = new System.Windows.Forms.Label();
            this.lbDelaySeconds = new System.Windows.Forms.Label();
            this.tbDelay = new System.Windows.Forms.TextBox();
            this.lbDelay = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbSpeed
            // 
            this.lbSpeed.ForeColor = System.Drawing.Color.DarkGreen;
            this.lbSpeed.Location = new System.Drawing.Point(204, 42);
            this.lbSpeed.Name = "lbSpeed";
            this.lbSpeed.Size = new System.Drawing.Size(57, 24);
            this.lbSpeed.TabIndex = 317;
            this.lbSpeed.Text = "mph";
            this.lbSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbSpeed
            // 
            this.tbSpeed.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSpeed.Location = new System.Drawing.Point(133, 39);
            this.tbSpeed.MaxLength = 8;
            this.tbSpeed.Name = "tbSpeed";
            this.tbSpeed.Size = new System.Drawing.Size(65, 30);
            this.tbSpeed.TabIndex = 316;
            this.tbSpeed.Text = "0";
            this.tbSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSpeed.TextChanged += new System.EventHandler(this.tbSpeed_TextChanged);
            this.tbSpeed.Enter += new System.EventHandler(this.tbSpeed_Enter);
            this.tbSpeed.Validating += new System.ComponentModel.CancelEventHandler(this.tbSpeed_Validating);
            // 
            // lbPrimedSpeed
            // 
            this.lbPrimedSpeed.AutoSize = true;
            this.lbPrimedSpeed.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPrimedSpeed.Location = new System.Drawing.Point(10, 42);
            this.lbPrimedSpeed.Name = "lbPrimedSpeed";
            this.lbPrimedSpeed.Size = new System.Drawing.Size(63, 23);
            this.lbPrimedSpeed.TabIndex = 315;
            this.lbPrimedSpeed.Text = "Speed";
            // 
            // lbOnTime
            // 
            this.lbOnTime.AutoSize = true;
            this.lbOnTime.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbOnTime.Location = new System.Drawing.Point(12, 9);
            this.lbOnTime.Name = "lbOnTime";
            this.lbOnTime.Size = new System.Drawing.Size(81, 23);
            this.lbOnTime.TabIndex = 318;
            this.lbOnTime.Text = "On Time";
            // 
            // tbTime
            // 
            this.tbTime.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbTime.Location = new System.Drawing.Point(133, 3);
            this.tbTime.MaxLength = 8;
            this.tbTime.Name = "tbTime";
            this.tbTime.Size = new System.Drawing.Size(65, 30);
            this.tbTime.TabIndex = 319;
            this.tbTime.Text = "0";
            this.tbTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbTime.TextChanged += new System.EventHandler(this.tbTime_TextChanged);
            this.tbTime.Enter += new System.EventHandler(this.tbTime_Enter);
            this.tbTime.Validating += new System.ComponentModel.CancelEventHandler(this.tbTime_Validating);
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
            this.btnCancel.Location = new System.Drawing.Point(126, 114);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 72);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
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
            this.btnOK.Location = new System.Drawing.Point(205, 114);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(72, 72);
            this.btnOK.TabIndex = 2;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lbOnSeconds
            // 
            this.lbOnSeconds.ForeColor = System.Drawing.Color.DarkGreen;
            this.lbOnSeconds.Location = new System.Drawing.Point(204, 9);
            this.lbOnSeconds.Name = "lbOnSeconds";
            this.lbOnSeconds.Size = new System.Drawing.Size(89, 24);
            this.lbOnSeconds.TabIndex = 322;
            this.lbOnSeconds.Text = "seconds";
            this.lbOnSeconds.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbDelaySeconds
            // 
            this.lbDelaySeconds.ForeColor = System.Drawing.Color.DarkGreen;
            this.lbDelaySeconds.Location = new System.Drawing.Point(204, 78);
            this.lbDelaySeconds.Name = "lbDelaySeconds";
            this.lbDelaySeconds.Size = new System.Drawing.Size(89, 24);
            this.lbDelaySeconds.TabIndex = 325;
            this.lbDelaySeconds.Text = "seconds";
            this.lbDelaySeconds.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbDelay
            // 
            this.tbDelay.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDelay.Location = new System.Drawing.Point(133, 75);
            this.tbDelay.MaxLength = 8;
            this.tbDelay.Name = "tbDelay";
            this.tbDelay.Size = new System.Drawing.Size(65, 30);
            this.tbDelay.TabIndex = 324;
            this.tbDelay.Text = "0";
            this.tbDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbDelay.TextChanged += new System.EventHandler(this.tbSpeed_TextChanged);
            this.tbDelay.Enter += new System.EventHandler(this.tbDelay_Enter);
            this.tbDelay.Validating += new System.ComponentModel.CancelEventHandler(this.tbDelay_Validating);
            // 
            // lbDelay
            // 
            this.lbDelay.AutoSize = true;
            this.lbDelay.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDelay.Location = new System.Drawing.Point(10, 78);
            this.lbDelay.Name = "lbDelay";
            this.lbDelay.Size = new System.Drawing.Size(117, 23);
            this.lbDelay.TabIndex = 323;
            this.lbDelay.Text = "Switch Delay";
            // 
            // frmPrimedStart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 189);
            this.Controls.Add(this.lbDelaySeconds);
            this.Controls.Add(this.tbDelay);
            this.Controls.Add(this.lbDelay);
            this.Controls.Add(this.lbOnSeconds);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tbTime);
            this.Controls.Add(this.lbOnTime);
            this.Controls.Add(this.lbSpeed);
            this.Controls.Add(this.tbSpeed);
            this.Controls.Add(this.lbPrimedSpeed);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPrimedStart";
            this.Text = "Primed Start";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmPrimedStart_FormClosed);
            this.Load += new System.EventHandler(this.frmPrimedStart_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbSpeed;
        private System.Windows.Forms.TextBox tbSpeed;
        private System.Windows.Forms.Label lbPrimedSpeed;
        private System.Windows.Forms.Label lbOnTime;
        private System.Windows.Forms.TextBox tbTime;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lbOnSeconds;
        private System.Windows.Forms.Label lbDelaySeconds;
        private System.Windows.Forms.TextBox tbDelay;
        private System.Windows.Forms.Label lbDelay;
    }
}