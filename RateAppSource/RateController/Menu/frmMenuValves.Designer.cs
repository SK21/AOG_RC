namespace RateController.Menu
{
    partial class frmMenuValves
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMenuValves));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.rb3Wire = new System.Windows.Forms.RadioButton();
            this.rb2Wire = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
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
            this.btnCancel.TabIndex = 160;
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
            this.btnOK.TabIndex = 159;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::RateController.Properties.Resources.Valve2wire;
            this.pictureBox2.Location = new System.Drawing.Point(12, 197);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(229, 295);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 164;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(248, 197);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(229, 295);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 163;
            this.pictureBox1.TabStop = false;
            // 
            // rb3Wire
            // 
            this.rb3Wire.Appearance = System.Windows.Forms.Appearance.Button;
            this.rb3Wire.Checked = true;
            this.rb3Wire.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rb3Wire.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rb3Wire.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rb3Wire.Location = new System.Drawing.Point(363, 110);
            this.rb3Wire.Name = "rb3Wire";
            this.rb3Wire.Size = new System.Drawing.Size(117, 69);
            this.rb3Wire.TabIndex = 162;
            this.rb3Wire.TabStop = true;
            this.rb3Wire.Text = "3 Wire valves";
            this.rb3Wire.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rb3Wire.UseVisualStyleBackColor = true;
            this.rb3Wire.CheckedChanged += new System.EventHandler(this.rb2Wire_CheckedChanged);
            // 
            // rb2Wire
            // 
            this.rb2Wire.Appearance = System.Windows.Forms.Appearance.Button;
            this.rb2Wire.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.rb2Wire.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rb2Wire.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rb2Wire.Location = new System.Drawing.Point(89, 110);
            this.rb2Wire.Name = "rb2Wire";
            this.rb2Wire.Size = new System.Drawing.Size(117, 69);
            this.rb2Wire.TabIndex = 161;
            this.rb2Wire.Text = "2 Wire valves";
            this.rb2Wire.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rb2Wire.UseVisualStyleBackColor = true;
            this.rb2Wire.CheckedChanged += new System.EventHandler(this.rb2Wire_CheckedChanged);
            // 
            // frmMenuValves
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 678);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.rb3Wire);
            this.Controls.Add(this.rb2Wire);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMenuValves";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuValves";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMenuValves_FormClosed);
            this.Load += new System.EventHandler(this.frmMenuValves_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.RadioButton rb3Wire;
        private System.Windows.Forms.RadioButton rb2Wire;
    }
}