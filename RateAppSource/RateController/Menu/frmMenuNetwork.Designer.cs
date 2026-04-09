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
            this.rbNano = new System.Windows.Forms.RadioButton();
            this.rbESP32 = new System.Windows.Forms.RadioButton();
            this.rbTeensy = new System.Windows.Forms.RadioButton();
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
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(70, 63);
            this.btnOK.TabIndex = 163;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // rbNano
            // 
            this.rbNano.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbNano.Checked = true;
            this.rbNano.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbNano.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbNano.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbNano.Location = new System.Drawing.Point(161, 223);
            this.rbNano.Name = "rbNano";
            this.rbNano.Size = new System.Drawing.Size(170, 37);
            this.rbNano.TabIndex = 0;
            this.rbNano.TabStop = true;
            this.rbNano.Text = "Nano (RC12-3)";
            this.rbNano.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbNano.UseVisualStyleBackColor = true;
            this.rbNano.CheckedChanged += new System.EventHandler(this.rbESP32_CheckedChanged);
            // 
            // rbESP32
            // 
            this.rbESP32.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbESP32.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rbESP32.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbESP32.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbESP32.Location = new System.Drawing.Point(161, 345);
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
            this.rbTeensy.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbTeensy.Location = new System.Drawing.Point(161, 284);
            this.rbTeensy.Name = "rbTeensy";
            this.rbTeensy.Size = new System.Drawing.Size(170, 37);
            this.rbTeensy.TabIndex = 45;
            this.rbTeensy.Text = "Teensy (RC11-2)";
            this.rbTeensy.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbTeensy.UseVisualStyleBackColor = true;
            this.rbTeensy.CheckedChanged += new System.EventHandler(this.rbESP32_CheckedChanged);
            // 
            // frmMenuNetwork
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 678);
            this.Controls.Add(this.rbNano);
            this.Controls.Add(this.rbESP32);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.rbTeensy);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMenuNetwork";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuNetwork";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMenuNetwork_FormClosed);
            this.Load += new System.EventHandler(this.frmMenuNetwork_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.RadioButton rbNano;
        private System.Windows.Forms.RadioButton rbESP32;
        private System.Windows.Forms.RadioButton rbTeensy;
    }
}