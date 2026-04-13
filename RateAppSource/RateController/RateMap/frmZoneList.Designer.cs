namespace RateController.Forms
{
    partial class frmZoneList
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
            this._dgv       = new System.Windows.Forms.DataGridView();
            this.pnlBottom  = new System.Windows.Forms.Panel();
            this._lblCount  = new System.Windows.Forms.Label();
            this._btnDelete = new System.Windows.Forms.Button();
            this._btnClose  = new System.Windows.Forms.Button();
            this.sep        = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this._dgv)).BeginInit();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            //
            // _dgv
            //
            this._dgv.AllowUserToAddRows          = false;
            this._dgv.AllowUserToDeleteRows       = false;
            this._dgv.AllowUserToResizeRows       = false;
            this._dgv.BackgroundColor             = System.Drawing.SystemColors.Window;
            this._dgv.BorderStyle                 = System.Windows.Forms.BorderStyle.None;
            this._dgv.CellBorderStyle             = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this._dgv.ColumnHeadersHeight         = 30;
            this._dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._dgv.Dock                        = System.Windows.Forms.DockStyle.Fill;
            this._dgv.GridColor                   = System.Drawing.Color.LightGray;
            this._dgv.MultiSelect                 = true;
            this._dgv.Name                        = "_dgv";
            this._dgv.ReadOnly                    = true;
            this._dgv.RowHeadersVisible           = false;
            this._dgv.SelectionMode               = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgv.TabIndex                    = 0;
            this._dgv.SelectionChanged           += new System.EventHandler(this.Dgv_SelectionChanged);
            //
            // pnlBottom
            //
            this.pnlBottom.Controls.Add(this._lblCount);
            this.pnlBottom.Controls.Add(this._btnDelete);
            this.pnlBottom.Controls.Add(this._btnClose);
            this.pnlBottom.Dock   = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Height = 48;
            this.pnlBottom.Name   = "pnlBottom";
            this.pnlBottom.TabIndex = 1;
            //
            // _lblCount
            //
            this._lblCount.AutoSize  = false;
            this._lblCount.Location  = new System.Drawing.Point(10, 10);
            this._lblCount.Name      = "_lblCount";
            this._lblCount.Size      = new System.Drawing.Size(200, 28);
            this._lblCount.TabIndex  = 0;
            this._lblCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // _btnDelete
            //
            this._btnDelete.Anchor  = System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Top;
            this._btnDelete.Enabled = false;
            this._btnDelete.Location = new System.Drawing.Point(480, 7);
            this._btnDelete.Name    = "_btnDelete";
            this._btnDelete.Size    = new System.Drawing.Size(100, 34);
            this._btnDelete.TabIndex = 1;
            this._btnDelete.Text    = "Delete";
            this._btnDelete.Click  += new System.EventHandler(this.BtnDelete_Click);
            //
            // _btnClose
            //
            this._btnClose.Anchor   = System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Top;
            this._btnClose.Location = new System.Drawing.Point(590, 7);
            this._btnClose.Name     = "_btnClose";
            this._btnClose.Size     = new System.Drawing.Size(100, 34);
            this._btnClose.TabIndex = 2;
            this._btnClose.Text     = "Close";
            this._btnClose.Click   += new System.EventHandler(this.BtnClose_Click);
            //
            // sep
            //
            this.sep.BackColor = System.Drawing.Color.LightGray;
            this.sep.Dock      = System.Windows.Forms.DockStyle.Bottom;
            this.sep.Height    = 1;
            this.sep.Name      = "sep";
            this.sep.TabIndex  = 2;
            //
            // frmZoneList
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(700, 460);
            this.Controls.Add(this._dgv);
            this.Controls.Add(this.sep);
            this.Controls.Add(this.pnlBottom);
            this.Font            = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MinimumSize     = new System.Drawing.Size(480, 320);
            this.Name            = "frmZoneList";
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text            = "Zones";
            ((System.ComponentModel.ISupportInitialize)(this._dgv)).EndInit();
            this.pnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridView _dgv;
        private System.Windows.Forms.Panel        pnlBottom;
        private System.Windows.Forms.Label        _lblCount;
        private System.Windows.Forms.Button       _btnDelete;
        private System.Windows.Forms.Button       _btnClose;
        private System.Windows.Forms.Panel        sep;
    }
}
