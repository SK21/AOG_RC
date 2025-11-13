namespace RateController.Forms
{
    partial class frmSpeedCal
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSpeedCal));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCalStop = new System.Windows.Forms.Button();
            this.btnCalStart = new System.Windows.Forms.Button();
            this.lbDistance = new System.Windows.Forms.Label();
            this.tbDistance = new System.Windows.Forms.TextBox();
            this.lbPulses = new System.Windows.Forms.Label();
            this.lbCurrent = new System.Windows.Forms.Label();
            this.lbDistanceUnits = new System.Windows.Forms.Label();
            this.lbCalUnits = new System.Windows.Forms.Label();
            this.lbCal = new System.Windows.Forms.Label();
            this.lbWheelCal = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnCancel.Image = global::RateController.Properties.Resources.Cancel64;
            this.btnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCancel.Location = new System.Drawing.Point(52, 285);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(70, 63);
            this.btnCancel.TabIndex = 4;
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
            this.btnOK.Image = global::RateController.Properties.Resources.Save;
            this.btnOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnOK.Location = new System.Drawing.Point(134, 285);
            this.btnOK.Margin = new System.Windows.Forms.Padding(6);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(70, 63);
            this.btnOK.TabIndex = 0;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCalStop
            // 
            this.btnCalStop.BackColor = System.Drawing.Color.Transparent;
            this.btnCalStop.Enabled = false;
            this.btnCalStop.FlatAppearance.BorderSize = 0;
            this.btnCalStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCalStop.Image = global::RateController.Properties.Resources.Stop;
            this.btnCalStop.Location = new System.Drawing.Point(134, 210);
            this.btnCalStop.Margin = new System.Windows.Forms.Padding(6);
            this.btnCalStop.Name = "btnCalStop";
            this.btnCalStop.Size = new System.Drawing.Size(70, 63);
            this.btnCalStop.TabIndex = 6;
            this.btnCalStop.UseVisualStyleBackColor = false;
            this.btnCalStop.Click += new System.EventHandler(this.btnCalStop_Click);
            // 
            // btnCalStart
            // 
            this.btnCalStart.BackColor = System.Drawing.Color.Transparent;
            this.btnCalStart.FlatAppearance.BorderSize = 0;
            this.btnCalStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCalStart.Image = global::RateController.Properties.Resources.Start;
            this.btnCalStart.Location = new System.Drawing.Point(52, 210);
            this.btnCalStart.Margin = new System.Windows.Forms.Padding(6);
            this.btnCalStart.Name = "btnCalStart";
            this.btnCalStart.Size = new System.Drawing.Size(70, 63);
            this.btnCalStart.TabIndex = 5;
            this.btnCalStart.UseVisualStyleBackColor = false;
            this.btnCalStart.Click += new System.EventHandler(this.btnCalStart_Click);
            // 
            // lbDistance
            // 
            this.lbDistance.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDistance.Location = new System.Drawing.Point(15, 10);
            this.lbDistance.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbDistance.Name = "lbDistance";
            this.lbDistance.Size = new System.Drawing.Size(108, 56);
            this.lbDistance.TabIndex = 239;
            this.lbDistance.Text = "Distance To Travel";
            this.lbDistance.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbDistance
            // 
            this.tbDistance.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDistance.Location = new System.Drawing.Point(135, 23);
            this.tbDistance.Margin = new System.Windows.Forms.Padding(6);
            this.tbDistance.MaxLength = 8;
            this.tbDistance.Name = "tbDistance";
            this.tbDistance.Size = new System.Drawing.Size(100, 30);
            this.tbDistance.TabIndex = 238;
            this.tbDistance.TabStop = false;
            this.tbDistance.Text = "0";
            this.tbDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbDistance.TextChanged += new System.EventHandler(this.tbDistance_TextChanged);
            this.tbDistance.Enter += new System.EventHandler(this.tbDistance_Enter);
            // 
            // lbPulses
            // 
            this.lbPulses.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbPulses.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPulses.Location = new System.Drawing.Point(135, 89);
            this.lbPulses.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbPulses.Name = "lbPulses";
            this.lbPulses.Size = new System.Drawing.Size(100, 30);
            this.lbPulses.TabIndex = 237;
            this.lbPulses.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbCurrent
            // 
            this.lbCurrent.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCurrent.Location = new System.Drawing.Point(15, 88);
            this.lbCurrent.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbCurrent.Name = "lbCurrent";
            this.lbCurrent.Size = new System.Drawing.Size(108, 32);
            this.lbCurrent.TabIndex = 236;
            this.lbCurrent.Text = "Pulses";
            this.lbCurrent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbDistanceUnits
            // 
            this.lbDistanceUnits.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDistanceUnits.Location = new System.Drawing.Point(135, 55);
            this.lbDistanceUnits.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbDistanceUnits.Name = "lbDistanceUnits";
            this.lbDistanceUnits.Size = new System.Drawing.Size(100, 30);
            this.lbDistanceUnits.TabIndex = 346;
            this.lbDistanceUnits.Text = "(feet)";
            this.lbDistanceUnits.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lbCalUnits
            // 
            this.lbCalUnits.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCalUnits.Location = new System.Drawing.Point(135, 174);
            this.lbCalUnits.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbCalUnits.Name = "lbCalUnits";
            this.lbCalUnits.Size = new System.Drawing.Size(100, 30);
            this.lbCalUnits.TabIndex = 349;
            this.lbCalUnits.Text = "(pulses/mile)";
            this.lbCalUnits.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lbCal
            // 
            this.lbCal.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCal.Location = new System.Drawing.Point(15, 147);
            this.lbCal.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbCal.Name = "lbCal";
            this.lbCal.Size = new System.Drawing.Size(108, 24);
            this.lbCal.TabIndex = 347;
            this.lbCal.Text = "Cal #";
            this.lbCal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbWheelCal
            // 
            this.lbWheelCal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbWheelCal.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbWheelCal.Location = new System.Drawing.Point(135, 144);
            this.lbWheelCal.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbWheelCal.Name = "lbWheelCal";
            this.lbWheelCal.Size = new System.Drawing.Size(100, 30);
            this.lbWheelCal.TabIndex = 350;
            this.lbWheelCal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // frmSpeedCal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(268, 367);
            this.Controls.Add(this.lbWheelCal);
            this.Controls.Add(this.lbCalUnits);
            this.Controls.Add(this.lbCal);
            this.Controls.Add(this.lbDistanceUnits);
            this.Controls.Add(this.lbDistance);
            this.Controls.Add(this.tbDistance);
            this.Controls.Add(this.lbPulses);
            this.Controls.Add(this.lbCurrent);
            this.Controls.Add(this.btnCalStop);
            this.Controls.Add(this.btnCalStart);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSpeedCal";
            this.ShowInTaskbar = false;
            this.Text = "Speed Cal";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmSpeedCal_FormClosed);
            this.Load += new System.EventHandler(this.frmSpeedCal_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCalStop;
        private System.Windows.Forms.Button btnCalStart;
        private System.Windows.Forms.Label lbDistance;
        private System.Windows.Forms.TextBox tbDistance;
        private System.Windows.Forms.Label lbPulses;
        private System.Windows.Forms.Label lbCurrent;
        private System.Windows.Forms.Label lbDistanceUnits;
        private System.Windows.Forms.Label lbCalUnits;
        private System.Windows.Forms.Label lbCal;
        private System.Windows.Forms.Label lbWheelCal;
        private System.Windows.Forms.Timer timer1;
    }
}