namespace RateController.Forms
{
    partial class frmRxDialog
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
            this.rtbList = new System.Windows.Forms.RichTextBox();
            this.lbHelp = new System.Windows.Forms.Label();
            this.btnAskAgain = new System.Windows.Forms.Button();
            this.btnVROff = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // rtbList
            //
            this.rtbList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbList.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbList.Location = new System.Drawing.Point(12, 12);
            this.rtbList.Name = "rtbList";
            this.rtbList.ReadOnly = true;
            this.rtbList.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbList.Size = new System.Drawing.Size(464, 200);
            this.rtbList.TabIndex = 0;
            this.rtbList.TabStop = false;
            this.rtbList.Text = "";
            this.rtbList.WordWrap = true;
            //
            // lbHelp
            //
            this.lbHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbHelp.Location = new System.Drawing.Point(12, 218);
            this.lbHelp.Name = "lbHelp";
            this.lbHelp.Size = new System.Drawing.Size(464, 97);
            this.lbHelp.TabIndex = 4;
            this.lbHelp.Text = "Ask again: ask the implement to use RC\'s rate.";
            //
            // btnAskAgain
            //
            this.btnAskAgain.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAskAgain.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnAskAgain.Location = new System.Drawing.Point(12, 321);
            this.btnAskAgain.Name = "btnAskAgain";
            this.btnAskAgain.Size = new System.Drawing.Size(150, 72);
            this.btnAskAgain.TabIndex = 1;
            this.btnAskAgain.Text = "Ask again";
            this.btnAskAgain.UseVisualStyleBackColor = false;
            this.btnAskAgain.Click += new System.EventHandler(this.btnAskAgain_Click);
            //
            // btnVROff
            //
            this.btnVROff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVROff.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnVROff.Location = new System.Drawing.Point(168, 321);
            this.btnVROff.Name = "btnVROff";
            this.btnVROff.Size = new System.Drawing.Size(220, 72);
            this.btnVROff.TabIndex = 2;
            this.btnVROff.Text = "Use implement\'s rate";
            this.btnVROff.UseVisualStyleBackColor = false;
            this.btnVROff.Click += new System.EventHandler(this.btnVROff_Click);
            //
            // btnCancel
            //
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnCancel.Image = global::RateController.Properties.Resources.Cancel64;
            this.btnCancel.Location = new System.Drawing.Point(400, 321);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 72);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // frmRxDialog
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(488, 402);
            this.Controls.Add(this.rtbList);
            this.Controls.Add(this.lbHelp);
            this.Controls.Add(this.btnAskAgain);
            this.Controls.Add(this.btnVROff);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmRxDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Variable rate not reaching the implement";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmRxDialog_FormClosed);
            this.Load += new System.EventHandler(this.frmRxDialog_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbList;
        private System.Windows.Forms.Label lbHelp;
        private System.Windows.Forms.Button btnAskAgain;
        private System.Windows.Forms.Button btnVROff;
        private System.Windows.Forms.Button btnCancel;
    }
}
