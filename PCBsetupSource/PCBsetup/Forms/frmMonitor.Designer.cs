
namespace PCBsetup.Forms
{
    partial class frmMonitor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMonitor));
            this.tbMonitor = new System.Windows.Forms.TextBox();
            this.bntOK = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // tbMonitor
            // 
            this.tbMonitor.BackColor = System.Drawing.SystemColors.Window;
            this.tbMonitor.Location = new System.Drawing.Point(15, 15);
            this.tbMonitor.Margin = new System.Windows.Forms.Padding(6);
            this.tbMonitor.Multiline = true;
            this.tbMonitor.Name = "tbMonitor";
            this.tbMonitor.ReadOnly = true;
            this.tbMonitor.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbMonitor.Size = new System.Drawing.Size(560, 501);
            this.tbMonitor.TabIndex = 0;
            this.tbMonitor.Click += new System.EventHandler(this.tbMonitor_Click);
            // 
            // bntOK
            // 
            this.bntOK.BackColor = System.Drawing.Color.Transparent;
            this.bntOK.FlatAppearance.BorderSize = 0;
            this.bntOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bntOK.Image = global::PCBsetup.Properties.Resources.bntOK_Image;
            this.bntOK.Location = new System.Drawing.Point(460, 525);
            this.bntOK.Name = "bntOK";
            this.bntOK.Size = new System.Drawing.Size(72, 72);
            this.bntOK.TabIndex = 21;
            this.bntOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.bntOK.UseVisualStyleBackColor = false;
            this.bntOK.Click += new System.EventHandler(this.bntOK_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // frmMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(590, 601);
            this.Controls.Add(this.bntOK);
            this.Controls.Add(this.tbMonitor);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMonitor";
            this.ShowInTaskbar = false;
            this.Text = "Serial Monitor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMonitor_FormClosing);
            this.Load += new System.EventHandler(this.frmMonitor_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbMonitor;
        private System.Windows.Forms.Button bntOK;
        private System.Windows.Forms.Timer timer1;
    }
}