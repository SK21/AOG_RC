namespace RateController
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
            this.bntOK = new System.Windows.Forms.Button();
            this.tbMonitor = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lb3 = new System.Windows.Forms.Label();
            this.cboPort1 = new System.Windows.Forms.ComboBox();
            this.PortName = new System.Windows.Forms.Label();
            this.SuspendLayout();
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
            this.bntOK.Location = new System.Drawing.Point(503, 525);
            this.bntOK.Name = "bntOK";
            this.bntOK.Size = new System.Drawing.Size(72, 72);
            this.bntOK.TabIndex = 137;
            this.bntOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.bntOK.UseVisualStyleBackColor = false;
            this.bntOK.Click += new System.EventHandler(this.bntOK_Click_1);
            // 
            // tbMonitor
            // 
            this.tbMonitor.BackColor = System.Drawing.SystemColors.Window;
            this.tbMonitor.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbMonitor.Location = new System.Drawing.Point(15, 15);
            this.tbMonitor.Multiline = true;
            this.tbMonitor.Name = "tbMonitor";
            this.tbMonitor.ReadOnly = true;
            this.tbMonitor.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbMonitor.Size = new System.Drawing.Size(560, 501);
            this.tbMonitor.TabIndex = 138;
            this.tbMonitor.Click += new System.EventHandler(this.tbMonitor_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lb3
            // 
            this.lb3.AutoSize = true;
            this.lb3.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb3.Location = new System.Drawing.Point(103, 549);
            this.lb3.Name = "lb3";
            this.lb3.Size = new System.Drawing.Size(95, 23);
            this.lb3.TabIndex = 140;
            this.lb3.Text = "Serial Port";
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
            this.cboPort1.Location = new System.Drawing.Point(204, 541);
            this.cboPort1.Name = "cboPort1";
            this.cboPort1.Size = new System.Drawing.Size(47, 37);
            this.cboPort1.TabIndex = 141;
            this.cboPort1.SelectedIndexChanged += new System.EventHandler(this.cboPort1_SelectedIndexChanged);
            // 
            // PortName
            // 
            this.PortName.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PortName.Location = new System.Drawing.Point(276, 549);
            this.PortName.Name = "PortName";
            this.PortName.Size = new System.Drawing.Size(95, 23);
            this.PortName.TabIndex = 142;
            // 
            // frmMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(590, 609);
            this.Controls.Add(this.PortName);
            this.Controls.Add(this.cboPort1);
            this.Controls.Add(this.lb3);
            this.Controls.Add(this.tbMonitor);
            this.Controls.Add(this.bntOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
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

        private System.Windows.Forms.Button bntOK;
        private System.Windows.Forms.TextBox tbMonitor;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lb3;
        private System.Windows.Forms.ComboBox cboPort1;
        private System.Windows.Forms.Label PortName;
    }
}