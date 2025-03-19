using RateController.Language;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuSwitches : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private FormStart mf;

        public frmMenuSwitches(FormStart main, frmMenu menu)
        {
            Initializing = true;
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
                mf.ShowSwitches = ckScreenSwitches.Checked;
                mf.UseDualAuto = ckDualAuto.Checked;
                mf.SwitchBox.UseWorkSwitch = ckWorkSwitch.Checked;
                mf.SwitchBox.AutoRateDisabled = rbSections.Checked;

                if (rbMasterAll.Checked)
                {
                    mf.Tls.MasterSwitchMode = MasterSwitchMode.ControlAll;
                }
                else if (rbMasterRelayOnly.Checked)
                {
                    mf.Tls.MasterSwitchMode = MasterSwitchMode.ControlMasterRelayOnly;
                }
                else if (rbMasterOverride.Checked)
                {
                    mf.Tls.MasterSwitchMode = MasterSwitchMode.Override;
                }

                SetButtons(false);
                UpdateForm();
                MainMenu.HighlightUpdateButton();
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmMenuValves/btnOk_Click: " + ex.Message);
            }
        }

        private void ckDualAuto_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void frmMenuSwitches_FormClosed(object sender, FormClosedEventArgs e)
        {
            mf.Tls.SaveFormData(this);
        }

        private void frmMenuSwitches_Load(object sender, EventArgs e)
        {
            // menu 800,600
            // sub menu 540,630
            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            mf.Tls.LoadFormData(this, "", false);
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

        private void groupBox2_Paint(object sender, PaintEventArgs e)
        {
            mf.Tls.DrawGroupBox((GroupBox)sender, e.Graphics, this.BackColor, Color.Black, Color.Blue);
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
            ckScreenSwitches.Text = Lang.lgSwitches;
            ckWorkSwitch.Text = Lang.lgWorkSwitch;
            gbOnScreen.Text = Lang.lgOnScreen;
            gbAutoSwitch.Text = Lang.lgAutoSwitch;
            ckDualAuto.Text = Lang.lgDualAuto;
            rbSections.Text = Lang.lgSections;
            rbAutoAll.Text = Lang.lgAutoAll;
            rbMasterAll.Text = Lang.lgMasterAll;
            rbMasterOverride.Text = Lang.lgMasterOverride;
            rbMasterRelayOnly.Text = Lang.lgMasterRelayOnly;
        }

        private void UpdateForm(bool UpdateObject = false)
        {
            Initializing = true;

            ckScreenSwitches.Checked = mf.ShowSwitches;
            ckDualAuto.Checked = mf.UseDualAuto;
            ckWorkSwitch.Checked = mf.SwitchBox.UseWorkSwitch;
            rbSections.Checked = mf.SwitchBox.AutoRateDisabled;

            switch (mf.Tls.MasterSwitchMode)
            {
                case MasterSwitchMode.ControlAll:
                    rbMasterAll.Checked = true;
                    break;

                case MasterSwitchMode.ControlMasterRelayOnly:
                    rbMasterRelayOnly.Checked = true;
                    break;

                case MasterSwitchMode.Override:
                    rbMasterOverride.Checked = true;
                    break;
            }

            Initializing = false;
        }
    }
}