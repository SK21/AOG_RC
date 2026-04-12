namespace RateController.Forms
{
    partial class frmSelectYieldFiles
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblPrompt      = new System.Windows.Forms.Label();
            this.clbFiles       = new System.Windows.Forms.CheckedListBox();
            this.lblCount       = new System.Windows.Forms.Label();
            this.btnSelectAll   = new System.Windows.Forms.Button();
            this.btnSelectNone  = new System.Windows.Forms.Button();
            this.btnOK          = new System.Windows.Forms.Button();
            this.btnCancel      = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblPrompt
            //
            this.lblPrompt.AutoSize  = true;
            this.lblPrompt.Location  = new System.Drawing.Point(12, 12);
            this.lblPrompt.Name      = "lblPrompt";
            this.lblPrompt.Size      = new System.Drawing.Size(250, 24);
            this.lblPrompt.TabIndex  = 0;
            this.lblPrompt.Text      = "Select yield files to include:";
            //
            // clbFiles
            //
            this.clbFiles.CheckOnClick    = true;
            this.clbFiles.FormattingEnabled = true;
            this.clbFiles.Location        = new System.Drawing.Point(12, 40);
            this.clbFiles.Name            = "clbFiles";
            this.clbFiles.Size            = new System.Drawing.Size(400, 196);
            this.clbFiles.TabIndex        = 1;
            //
            // lblCount
            //
            this.lblCount.AutoSize  = true;
            this.lblCount.Location  = new System.Drawing.Point(12, 244);
            this.lblCount.Name      = "lblCount";
            this.lblCount.Size      = new System.Drawing.Size(100, 24);
            this.lblCount.TabIndex  = 2;
            this.lblCount.Text      = "0 file(s) available";
            //
            // btnSelectAll
            //
            this.btnSelectAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectAll.Location  = new System.Drawing.Point(12, 274);
            this.btnSelectAll.Name      = "btnSelectAll";
            this.btnSelectAll.Size      = new System.Drawing.Size(90, 28);
            this.btnSelectAll.TabIndex  = 3;
            this.btnSelectAll.Text      = "All";
            this.btnSelectAll.Click    += new System.EventHandler(this.btnSelectAll_Click);
            //
            // btnSelectNone
            //
            this.btnSelectNone.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectNone.Location  = new System.Drawing.Point(108, 274);
            this.btnSelectNone.Name      = "btnSelectNone";
            this.btnSelectNone.Size      = new System.Drawing.Size(90, 28);
            this.btnSelectNone.TabIndex  = 4;
            this.btnSelectNone.Text      = "None";
            this.btnSelectNone.Click    += new System.EventHandler(this.btnSelectNone_Click);
            //
            // btnOK
            //
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Location  = new System.Drawing.Point(234, 274);
            this.btnOK.Name      = "btnOK";
            this.btnOK.Size      = new System.Drawing.Size(82, 28);
            this.btnOK.TabIndex  = 5;
            this.btnOK.Text      = "OK";
            this.btnOK.Click    += new System.EventHandler(this.btnOK_Click);
            //
            // btnCancel
            //
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location  = new System.Drawing.Point(322, 274);
            this.btnCancel.Name      = "btnCancel";
            this.btnCancel.Size      = new System.Drawing.Size(90, 28);
            this.btnCancel.TabIndex  = 6;
            this.btnCancel.Text      = "Cancel";
            this.btnCancel.Click    += new System.EventHandler(this.btnCancel_Click);
            //
            // frmSelectYieldFiles
            //
            this.AcceptButton          = this.btnOK;
            this.CancelButton          = this.btnCancel;
            this.AutoScaleDimensions   = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode         = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize            = new System.Drawing.Size(424, 314);
            this.Controls.Add(this.lblPrompt);
            this.Controls.Add(this.clbFiles);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.btnSelectAll);
            this.Controls.Add(this.btnSelectNone);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Font                  = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle       = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin                = new System.Windows.Forms.Padding(6);
            this.MaximizeBox           = false;
            this.MinimizeBox           = false;
            this.Name                  = "frmSelectYieldFiles";
            this.StartPosition         = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text                  = "Select Yield Files";
            this.TopMost               = true;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label          lblPrompt;
        private System.Windows.Forms.CheckedListBox clbFiles;
        private System.Windows.Forms.Label          lblCount;
        private System.Windows.Forms.Button         btnSelectAll;
        private System.Windows.Forms.Button         btnSelectNone;
        private System.Windows.Forms.Button         btnOK;
        private System.Windows.Forms.Button         btnCancel;
    }
}
