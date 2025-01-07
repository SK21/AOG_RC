namespace RateController.Menu
{
    partial class frmMenuSections
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.ckZones = new System.Windows.Forms.CheckBox();
            this.btnEqual = new System.Windows.Forms.Button();
            this.lbPerZone = new System.Windows.Forms.Label();
            this.tbSectionsPerZone = new System.Windows.Forms.TextBox();
            this.lbFeet = new System.Windows.Forms.Label();
            this.lbNumZones = new System.Windows.Forms.Label();
            this.tbSectionCount = new System.Windows.Forms.TextBox();
            this.lbWidth = new System.Windows.Forms.Label();
            this.DGV = new System.Windows.Forms.DataGridView();
            this.SectionWidth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Switch = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DGV2 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.DGV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DGV2)).BeginInit();
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
            this.btnCancel.Location = new System.Drawing.Point(378, 546);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 72);
            this.btnCancel.TabIndex = 168;
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnCancel.UseVisualStyleBackColor = false;
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
            this.btnOK.Location = new System.Drawing.Point(456, 546);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(72, 72);
            this.btnOK.TabIndex = 167;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            // 
            // ckZones
            // 
            this.ckZones.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckZones.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckZones.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckZones.Image = global::RateController.Properties.Resources.SectionsNoZones2;
            this.ckZones.Location = new System.Drawing.Point(130, 515);
            this.ckZones.Name = "ckZones";
            this.ckZones.Size = new System.Drawing.Size(113, 100);
            this.ckZones.TabIndex = 169;
            this.ckZones.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckZones.UseVisualStyleBackColor = true;
            // 
            // btnEqual
            // 
            this.btnEqual.BackColor = System.Drawing.Color.Transparent;
            this.btnEqual.FlatAppearance.BorderSize = 0;
            this.btnEqual.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEqual.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEqual.Image = global::RateController.Properties.Resources.Copy;
            this.btnEqual.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnEqual.Location = new System.Drawing.Point(268, 543);
            this.btnEqual.Margin = new System.Windows.Forms.Padding(6);
            this.btnEqual.Name = "btnEqual";
            this.btnEqual.Size = new System.Drawing.Size(101, 72);
            this.btnEqual.TabIndex = 170;
            this.btnEqual.Text = "=1";
            this.btnEqual.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEqual.UseVisualStyleBackColor = false;
            // 
            // lbPerZone
            // 
            this.lbPerZone.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPerZone.Location = new System.Drawing.Point(310, 400);
            this.lbPerZone.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbPerZone.Name = "lbPerZone";
            this.lbPerZone.Size = new System.Drawing.Size(179, 28);
            this.lbPerZone.TabIndex = 178;
            this.lbPerZone.Text = "Sections per Zone";
            // 
            // tbSectionsPerZone
            // 
            this.tbSectionsPerZone.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSectionsPerZone.Location = new System.Drawing.Point(501, 400);
            this.tbSectionsPerZone.Margin = new System.Windows.Forms.Padding(6);
            this.tbSectionsPerZone.Name = "tbSectionsPerZone";
            this.tbSectionsPerZone.Size = new System.Drawing.Size(54, 29);
            this.tbSectionsPerZone.TabIndex = 174;
            this.tbSectionsPerZone.Text = "9999";
            this.tbSectionsPerZone.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbFeet
            // 
            this.lbFeet.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbFeet.Location = new System.Drawing.Point(205, 435);
            this.lbFeet.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbFeet.Name = "lbFeet";
            this.lbFeet.Size = new System.Drawing.Size(133, 24);
            this.lbFeet.TabIndex = 177;
            this.lbFeet.Text = "100.6 FT";
            this.lbFeet.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbNumZones
            // 
            this.lbNumZones.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbNumZones.Location = new System.Drawing.Point(8, 400);
            this.lbNumZones.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbNumZones.Name = "lbNumZones";
            this.lbNumZones.Size = new System.Drawing.Size(185, 28);
            this.lbNumZones.TabIndex = 176;
            this.lbNumZones.Text = "Number of Sections";
            // 
            // tbSectionCount
            // 
            this.tbSectionCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSectionCount.Location = new System.Drawing.Point(205, 400);
            this.tbSectionCount.Margin = new System.Windows.Forms.Padding(6);
            this.tbSectionCount.Name = "tbSectionCount";
            this.tbSectionCount.Size = new System.Drawing.Size(54, 29);
            this.tbSectionCount.TabIndex = 173;
            this.tbSectionCount.Text = "9999";
            this.tbSectionCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbWidth
            // 
            this.lbWidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbWidth.Location = new System.Drawing.Point(8, 435);
            this.lbWidth.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbWidth.Name = "lbWidth";
            this.lbWidth.Size = new System.Drawing.Size(187, 24);
            this.lbWidth.TabIndex = 175;
            this.lbWidth.Text = "Width:  1200 Inches";
            this.lbWidth.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DGV
            // 
            this.DGV.AllowUserToAddRows = false;
            this.DGV.AllowUserToDeleteRows = false;
            this.DGV.AllowUserToResizeColumns = false;
            this.DGV.AllowUserToResizeRows = false;
            this.DGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SectionWidth,
            this.Switch});
            this.DGV.DataMember = "Table1";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.NullValue = "<dbnull>";
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DGV.DefaultCellStyle = dataGridViewCellStyle3;
            this.DGV.Location = new System.Drawing.Point(97, 36);
            this.DGV.Margin = new System.Windows.Forms.Padding(6);
            this.DGV.Name = "DGV";
            this.DGV.RowHeadersVisible = false;
            this.DGV.RowTemplate.Height = 40;
            this.DGV.Size = new System.Drawing.Size(369, 352);
            this.DGV.TabIndex = 171;
            // 
            // SectionWidth
            // 
            this.SectionWidth.DataPropertyName = "Width";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.Format = "N0";
            dataGridViewCellStyle1.NullValue = null;
            this.SectionWidth.DefaultCellStyle = dataGridViewCellStyle1;
            this.SectionWidth.HeaderText = "Width";
            this.SectionWidth.Name = "SectionWidth";
            this.SectionWidth.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // Switch
            // 
            this.Switch.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Switch.DataPropertyName = "Switch";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Format = "N0";
            dataGridViewCellStyle2.NullValue = null;
            this.Switch.DefaultCellStyle = dataGridViewCellStyle2;
            this.Switch.HeaderText = "Switch";
            this.Switch.Name = "Switch";
            this.Switch.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // DGV2
            // 
            this.DGV2.AllowUserToAddRows = false;
            this.DGV2.AllowUserToDeleteRows = false;
            this.DGV2.AllowUserToResizeColumns = false;
            this.DGV2.AllowUserToResizeRows = false;
            this.DGV2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV2.DataMember = "Table1";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.NullValue = "<dbnull>";
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DGV2.DefaultCellStyle = dataGridViewCellStyle4;
            this.DGV2.Location = new System.Drawing.Point(8, 36);
            this.DGV2.Margin = new System.Windows.Forms.Padding(6);
            this.DGV2.Name = "DGV2";
            this.DGV2.RowHeadersVisible = false;
            this.DGV2.RowTemplate.Height = 40;
            this.DGV2.Size = new System.Drawing.Size(547, 352);
            this.DGV2.TabIndex = 172;
            // 
            // frmMenuSections
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 630);
            this.Controls.Add(this.lbPerZone);
            this.Controls.Add(this.tbSectionsPerZone);
            this.Controls.Add(this.lbFeet);
            this.Controls.Add(this.lbNumZones);
            this.Controls.Add(this.tbSectionCount);
            this.Controls.Add(this.lbWidth);
            this.Controls.Add(this.DGV);
            this.Controls.Add(this.DGV2);
            this.Controls.Add(this.ckZones);
            this.Controls.Add(this.btnEqual);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMenuSections";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuSections";
            ((System.ComponentModel.ISupportInitialize)(this.DGV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DGV2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox ckZones;
        private System.Windows.Forms.Button btnEqual;
        private System.Windows.Forms.Label lbPerZone;
        private System.Windows.Forms.TextBox tbSectionsPerZone;
        private System.Windows.Forms.Label lbFeet;
        private System.Windows.Forms.Label lbNumZones;
        private System.Windows.Forms.TextBox tbSectionCount;
        private System.Windows.Forms.Label lbWidth;
        private System.Windows.Forms.DataGridView DGV;
        private System.Windows.Forms.DataGridViewTextBoxColumn SectionWidth;
        private System.Windows.Forms.DataGridViewTextBoxColumn Switch;
        private System.Windows.Forms.DataGridView DGV2;
    }
}