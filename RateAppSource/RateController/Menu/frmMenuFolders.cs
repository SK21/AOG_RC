using RateController.Classes;
using System;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuFolders : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;

        public frmMenuFolders(frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            this.Tag = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
        }

        private void btnNewFolder_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select a folder";
                dialog.ShowNewFolderButton = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    tbNewLocation.Text = dialog.SelectedPath;
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var Hlp = new frmMsgBox("Change folders and restart?", "Data folder", true);
            Hlp.ShowDialog();
            bool Result = Hlp.Result;
            Hlp.Close();
            if (Result)
            {
                Props.ChangeDataFolder(tbNewLocation.Text, ckOverwrite.Checked, ckCopyData.Checked);
                UpdateForm();
            }
            else
            {
                UpdateForm();
                SetButtons(false);
            }
        }

        private void frmMenuFolders_FormClosing(object sender, FormClosingEventArgs e)
        {
            Props.SaveFormLocation(this);
        }

        private void frmMenuFolders_Load(object sender, EventArgs e)
        {
            SubMenuLayout.SetFormLayout(this, MainMenu, btnOK);

            btnCancel.Left = btnOK.Left - SubMenuLayout.ButtonSpacing;
            btnCancel.Top = btnOK.Top;
            MainMenu.StyleControls(this);
            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            this.BackColor = Properties.Settings.Default.MainBackColour;

            PositionForm();
            UpdateForm();
        }

        private void MainMenu_MenuMoved(object sender, EventArgs e)
        {
            PositionForm();
        }

        private void PositionForm()
        {
            this.Top = MainMenu.Top + SubMenuLayout.TopOffset;
            this.Left = MainMenu.Left + SubMenuLayout.LeftOffset;
        }

        private void SetButtons(bool Edited)
        {
            if (!Initializing)
            {
                if (Edited)
                {
                    btnCancel.Enabled = true;
                    btnOK.Enabled = true;
                }
                else
                {
                    btnCancel.Enabled = false;
                    btnOK.Enabled = false;
                }

                cEdited = Edited;
                this.Tag = cEdited;
            }
        }

        private void SetLanguage()
        {
        }

        private void tbNewLocation_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void UpdateForm()
        {
            Initializing = true;
            lbCurrent.Text = Props.DataFolder;
            Initializing = false;
        }
    }
}