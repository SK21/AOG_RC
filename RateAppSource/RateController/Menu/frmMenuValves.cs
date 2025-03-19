using RateController.Classes;
using RateController.Language;
using System;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuValves : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private FormStart mf;

        public frmMenuValves(FormStart main, frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            mf = main;
            this.Tag = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                mf.ModuleConfig.Is3Wire = rb3Wire.Checked;
                mf.ModuleConfig.Save();
                SetButtons(false);
                UpdateForm();
                MainMenu.HighlightUpdateButton();
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmMenuValves/btnOk_Click: " + ex.Message);
            }
        }

        private void frmMenuValves_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
        }

        private void frmMenuValves_Load(object sender, EventArgs e)
        {
            // menu 800,600
            // sub menu 540,630
            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            Props.LoadFormLocation(this, "", false);
            this.BackColor = Properties.Settings.Default.MainBackColour;
            this.Width = MainMenu.Width - 260;
            this.Height = MainMenu.Height - 50;
            btnOK.Left = this.Width - 84;
            btnOK.Top = this.Height - 84;
            btnCancel.Left = btnOK.Left - 78;
            btnCancel.Top = btnOK.Top;
            MainMenu.StyleControls(this);
            PositionForm();
            UpdateForm();
        }

        private void MainMenu_MenuMoved(object sender, EventArgs e)
        {
            PositionForm();
        }

        private void PositionForm()
        {
            this.Top = MainMenu.Top + 30;
            this.Left = MainMenu.Left + 246;
        }

        private void rb2Wire_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
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
            rb2Wire.Text = Lang.lg2Wire;
            rb3Wire.Text = Lang.lg3Wire;
        }

        private void UpdateForm()
        {
            Initializing = true;
            rb3Wire.Checked = mf.ModuleConfig.Is3Wire;
            rb2Wire.Checked = !rb3Wire.Checked;
            Initializing = false;
        }
    }
}