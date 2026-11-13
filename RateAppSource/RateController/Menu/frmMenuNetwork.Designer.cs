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
            this.components = new System.ComponentModel.Container();
            this.lvBoards = new System.Windows.Forms.ListView();
            this.chModule = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chBoard = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chState = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ilRowHeight = new System.Windows.Forms.ImageList(this.components);
            this.lbModule = new System.Windows.Forms.Label();
            this.cbModule = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnPreset = new System.Windows.Forms.Button();
            this.lbDescription = new System.Windows.Forms.Label();
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.lnkWeb = new System.Windows.Forms.LinkLabel();
            this.lbBoardsFound = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lvBoards
            // 
            this.lvBoards.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chModule,
            this.chBoard,
            this.chState});
            this.lvBoards.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvBoards.FullRowSelect = true;
            this.lvBoards.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvBoards.HideSelection = false;
            this.lvBoards.Location = new System.Drawing.Point(20, 45);
            this.lvBoards.MultiSelect = false;
            this.lvBoards.Name = "lvBoards";
            this.lvBoards.Size = new System.Drawing.Size(500, 384);
            this.lvBoards.SmallImageList = this.ilRowHeight;
            this.lvBoards.TabIndex = 0;
            this.lvBoards.UseCompatibleStateImageBehavior = false;
            this.lvBoards.View = System.Windows.Forms.View.Details;
            this.lvBoards.SelectedIndexChanged += new System.EventHandler(this.lvBoards_SelectedIndexChanged);
            // 
            // chModule
            // 
            this.chModule.Text = "Module";
            this.chModule.Width = 70;
            // 
            // chBoard
            // 
            this.chBoard.Text = "Board";
            this.chBoard.Width = 224;
            // 
            // chState
            // 
            this.chState.Text = "State";
            this.chState.Width = 180;
            // 
            // ilRowHeight
            // 
            this.ilRowHeight.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.ilRowHeight.ImageSize = new System.Drawing.Size(1, 32);
            this.ilRowHeight.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // lbModule
            // 
            this.lbModule.AutoSize = true;
            this.lbModule.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbModule.Location = new System.Drawing.Point(20, 464);
            this.lbModule.Name = "lbModule";
            this.lbModule.Size = new System.Drawing.Size(74, 24);
            this.lbModule.TabIndex = 1;
            this.lbModule.Text = "Module";
            // 
            // cbModule
            // 
            this.cbModule.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbModule.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbModule.Location = new System.Drawing.Point(160, 460);
            this.cbModule.Name = "cbModule";
            this.cbModule.Size = new System.Drawing.Size(100, 32);
            this.cbModule.TabIndex = 2;
            this.cbModule.TabStop = false;
            this.cbModule.SelectedIndexChanged += new System.EventHandler(this.Edit_Changed);
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
            this.btnOK.Location = new System.Drawing.Point(458, 603);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(70, 63);
            this.btnOK.TabIndex = 7;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
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
            this.btnCancel.Location = new System.Drawing.Point(382, 603);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(70, 63);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // btnPreset
            //
            this.btnPreset.BackColor = System.Drawing.Color.Transparent;
            this.btnPreset.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnPreset.FlatAppearance.BorderSize = 0;
            this.btnPreset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPreset.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnPreset.Image = global::RateController.Properties.Resources.VehFileLoad;
            this.btnPreset.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnPreset.Location = new System.Drawing.Point(306, 603);
            this.btnPreset.Name = "btnPreset";
            this.btnPreset.Size = new System.Drawing.Size(70, 63);
            this.btnPreset.TabIndex = 10;
            this.btnPreset.TabStop = false;
            this.btnPreset.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnPreset.UseVisualStyleBackColor = false;
            this.btnPreset.Click += new System.EventHandler(this.btnPreset_Click);
            // 
            // lbDescription
            // 
            this.lbDescription.AutoSize = true;
            this.lbDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDescription.Location = new System.Drawing.Point(20, 518);
            this.lbDescription.Name = "lbDescription";
            this.lbDescription.Size = new System.Drawing.Size(104, 24);
            this.lbDescription.TabIndex = 4;
            this.lbDescription.Text = "Description";
            // 
            // tbDescription
            // 
            this.tbDescription.Enabled = false;
            this.tbDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDescription.Location = new System.Drawing.Point(160, 516);
            this.tbDescription.MaxLength = 16;
            this.tbDescription.Name = "tbDescription";
            this.tbDescription.Size = new System.Drawing.Size(240, 29);
            this.tbDescription.TabIndex = 5;
            this.tbDescription.TabStop = false;
            this.tbDescription.TextChanged += new System.EventHandler(this.Edit_Changed);
            // 
            // lnkWeb
            // 
            this.lnkWeb.AutoSize = true;
            this.lnkWeb.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lnkWeb.Location = new System.Drawing.Point(160, 560);
            this.lnkWeb.Name = "lnkWeb";
            this.lnkWeb.Size = new System.Drawing.Size(0, 24);
            this.lnkWeb.TabIndex = 6;
            this.lnkWeb.Visible = false;
            this.lnkWeb.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkWeb_LinkClicked);
            //
            // lbBoardsFound
            //
            this.lbBoardsFound.AutoSize = true;
            this.lbBoardsFound.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbBoardsFound.Location = new System.Drawing.Point(20, 12);
            this.lbBoardsFound.Name = "lbBoardsFound";
            this.lbBoardsFound.Size = new System.Drawing.Size(232, 24);
            this.lbBoardsFound.TabIndex = 9;
            this.lbBoardsFound.Text = "Boards found - select one";
            //
            // frmMenuNetwork
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 678);
            this.Controls.Add(this.lbBoardsFound);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnPreset);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lnkWeb);
            this.Controls.Add(this.tbDescription);
            this.Controls.Add(this.lbDescription);
            this.Controls.Add(this.cbModule);
            this.Controls.Add(this.lbModule);
            this.Controls.Add(this.lvBoards);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMenuNetwork";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuNetwork";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMenuNetwork_FormClosed);
            this.Load += new System.EventHandler(this.frmMenuNetwork_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvBoards;
        private System.Windows.Forms.ColumnHeader chModule;
        private System.Windows.Forms.ColumnHeader chBoard;
        private System.Windows.Forms.ColumnHeader chState;
        private System.Windows.Forms.ImageList ilRowHeight;
        private System.Windows.Forms.Label lbModule;
        private System.Windows.Forms.ComboBox cbModule;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnPreset;
        private System.Windows.Forms.Label lbDescription;
        private System.Windows.Forms.TextBox tbDescription;
        private System.Windows.Forms.LinkLabel lnkWeb;
        private System.Windows.Forms.Label lbBoardsFound;
    }
}
