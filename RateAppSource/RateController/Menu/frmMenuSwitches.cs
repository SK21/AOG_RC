using RateController.Classes;
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

        public frmMenuSwitches(frmMenu menu)
        {
            Initializing = true;
            InitializeComponent();
            MainMenu = menu;
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
                Props.ShowSwitches = ckScreenSwitches.Checked;
                Props.UseDualAuto = ckDualAuto.Checked;
                Core.SwitchBox.WorkSwitchEnabled = ckWorkSwitch.Checked;
                Core.SwitchBox.AutoRateEnabled = ckRate.Checked;
                Core.SwitchBox.AutoSectionEnabled = ckSections.Checked;

                if (rbMasterAll.Checked)
                {
                    Props.MasterSwitchMode = MasterSwitchMode.ControlAll;
                }
                else if (rbMasterRelayOnly.Checked)
                {
                    Props.MasterSwitchMode = MasterSwitchMode.ControlMasterRelayOnly;
                }
                else if (rbMasterOverride.Checked)
                {
                    Props.MasterSwitchMode = MasterSwitchMode.Override;
                }

                SetButtons(false);
                UpdateForm();
                MainMenu.HighlightUpdateButton();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuValves/btnOk_Click: " + ex.Message);
            }
        }

        private void ckDualAuto_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void frmMenuSwitches_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
        }

        private void frmMenuSwitches_Load(object sender, EventArgs e)
        {
            SubMenuLayout.SetFormLayout(this, MainMenu, null);
            btnOK.Left = this.Width - 84;
            btnOK.Top = this.Height - 84;
            btnCancel.Left = btnOK.Left - 78;
            btnCancel.Top = btnOK.Top;
            MainMenu.StyleControls(this);
            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            this.BackColor = Properties.Settings.Default.MainBackColour;
            PositionForm();
            UpdateForm();
        }

        private void groupBox2_Paint(object sender, PaintEventArgs e)
        {
            Props.DrawGroupBox((GroupBox)sender, e.Graphics, this.BackColor, Color.Black, Color.Blue);
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
            ckScreenSwitches.Text = Lang.lgSwitches;
            ckWorkSwitch.Text = Lang.lgWorkSwitch;
            gbOnScreen.Text = Lang.lgOnScreen;
            gbAutoSwitch.Text = Lang.lgAutoSwitch;
            ckDualAuto.Text = Lang.lgDualAuto;
            ckSections.Text = Lang.lgSections;
            ckRate.Text = Lang.lgRate;
            rbMasterAll.Text = Lang.lgMasterAll;
            rbMasterOverride.Text = Lang.lgMasterOverride;
            rbMasterRelayOnly.Text = Lang.lgMasterRelayOnly;
        }

        private void UpdateForm()
        {
            Initializing = true;

            ckScreenSwitches.Checked = Props.ShowSwitches;
            ckDualAuto.Checked = Props.UseDualAuto;
            ckWorkSwitch.Checked = Core.SwitchBox.WorkSwitchEnabled;
            ckRate.Checked = Core.SwitchBox.AutoRateEnabled;
            ckSections.Checked = Core.SwitchBox.AutoSectionEnabled;

            switch (Props.MasterSwitchMode)
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