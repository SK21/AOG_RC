using RateController.Classes;
using RateController.Properties;
using System;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuLanguage : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private string[] LanguageIDs;
        private RadioButton[] LanguageRBs;
        private frmMenu MainMenu;
        private FormStart mf;

        public frmMenuLanguage(FormStart main, frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            mf = main;
            this.Tag = false;

            LanguageRBs = new RadioButton[] { rbEnglish, rbDeustch, rbHungarian, rbNederlands, rbPolish, rbRussian, rbFrench, rbLithuanian };
            LanguageIDs = new string[] { "en", "de", "hu", "nl", "pl", "ru", "fr", "lt" };
            for (int i = 0; i < LanguageRBs.Length; i++)
            {
                LanguageRBs[i].CheckedChanged += Language_CheckedChanged;
            }
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
                int Lan = 0;
                for (int i = 0; i < LanguageRBs.Length; i++)
                {
                    if (LanguageRBs[i].Checked)
                    {
                        Lan = i;
                        break;
                    }
                }

                if (Properties.Settings.Default.setF_culture != LanguageIDs[Lan])
                {
                    Properties.Settings.Default.setF_culture = LanguageIDs[Lan];
                    Settings.Default.UserLanguageChange = true;
                    Properties.Settings.Default.Save();

                    Form fs = Props.IsFormOpen("frmLargeScreen");
                    if (fs != null)
                    {
                        mf.Restart = true;
                        mf.Lscrn.Close();
                    }
                    else
                    {
                        mf.ChangeLanguage();
                    }
                }

                SetButtons(false);
                UpdateForm();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuLanguage/btnOk_Click: " + ex.Message);
            }
        }

        private void frmMenuLanguage_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
        }

        private void frmMenuLanguage_Load(object sender, EventArgs e)
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

        private void Language_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
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

        private void rbDeustch_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void rbFrench_CheckedChanged(object sender, EventArgs e)
        {
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

        private void UpdateForm()
        {
            Initializing = true;

            for (int i = 0; i < LanguageRBs.Length; i++)
            {
                if (LanguageIDs[i] == Properties.Settings.Default.setF_culture)
                {
                    LanguageRBs[i].Checked = true;
                    break;
                }
            }

            Initializing = false;
        }
    }
}