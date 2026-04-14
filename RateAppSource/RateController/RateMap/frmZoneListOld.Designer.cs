namespace RateController.Forms
{
    partial class frmZoneListOld
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
            this._dgv = new System.Windows.Forms.DataGridView();
            this.sep = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this._dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // _dgv
            // 
            this._dgv.AllowUserToAddRows = false;
            this._dgv.AllowUserToDeleteRows = false;
            this._dgv.AllowUserToResizeRows = false;
            this._dgv.BackgroundColor = System.Drawing.SystemColors.Window;
            this._dgv.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._dgv.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this._dgv.ColumnHeadersHeight = 30;
            this._dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dgv.GridColor = System.Drawing.Color.LightGray;
            this._dgv.Location = new System.Drawing.Point(0, 0);
            this._dgv.Name = "_dgv";
            this._dgv.ReadOnly = true;
            this._dgv.RowHeadersVisible = false;
            this._dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgv.Size = new System.Drawing.Size(700, 459);
            this._dgv.TabIndex = 0;
            this._dgv.SelectionChanged += new System.EventHandler(this.Dgv_SelectionChanged);
            // 
            // sep
            // 
            this.sep.BackColor = System.Drawing.Color.LightGray;
            this.sep.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.sep.Location = new System.Drawing.Point(0, 459);
            this.sep.Name = "sep";
            this.sep.Size = new System.Drawing.Size(700, 1);
            this.sep.TabIndex = 2;
            // 
            // frmZoneList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 460);
            this.Controls.Add(this._dgv);
            this.Controls.Add(this.sep);
            this.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(480, 320);
            this.Name = "frmZoneList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Zones";
            ((System.ComponentModel.ISupportInitialize)(this._dgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView _dgv;
        private System.Windows.Forms.Panel        sep;
    }
}
