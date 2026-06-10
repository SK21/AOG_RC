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
                Props.MasterMaintained = rbMaintained.Checked;
                Props.ShowSwitches = ckOnScreen.Checked;
                int newCount = rb4.Checked ? 4 : rb12.Checked ? 12 : rb16.Checked ? 16 : 8;
                if (newCount < Props.SwitchCount)
                {
                    bool conflict = false;
                    foreach (var rly in Core.RelayObjects.Items)
                    {
                        if (rly.Type == RelayTypes.Switch && rly.SwitchID >= newCount)
                        {
                            conflict = true;
                            break;
                        }
                    }
                    if (conflict)
                        Props.ShowMessage("Warning: one or more relays are assigned to a switch that will no longer be available. Check relay switch assignments.", "Switch Count", 15000, true);
                }
                Props.SwitchCount = newCount;
                Core.SwitchBox.WorkSwitchEnabled = ckWorkSwitch.Checked;
                Core.SwitchBox.AutoRateEnabled = ckRate.Checked;
                Core.SwitchBox.AutoSectionEnabled = ckSections.Checked;

                if (rbMasterAll.Checked)
                {
                    Props.MasterSwitchMode = MasterSwitchMode.Standard;
                }
                else if (rbMasterOverride.Checked)
                {
                    Props.MasterSwitchMode = MasterSwitchMode.Override;
                }

                frmSwitches sw = Props.IsFormOpen("frmSwitches", false) as frmSwitches;
                if (sw != null)
                {
                    sw.CreateSwitchButtons();
                    sw.SetDescriptions();
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
            if (!Initializing)
                rb4.Enabled = rb8.Enabled = rb12.Enabled = rb16.Enabled = ckOnScreen.Checked;
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
            ckOnScreen.Text = Lang.lgEnabled;
            ckWorkSwitch.Text = Lang.lgWorkSwitch;
            gbAutoSwitch.Text = Lang.lgAutoSwitch;
            ckSections.Text = Lang.lgSections;
            ckRate.Text = Lang.lgRate;
            rbMasterAll.Text = Lang.lgMasterAll;
            rbMasterOverride.Text = Lang.lgMasterOverride;
        }

        private void UpdateForm()
        {
            Initializing = true;

            if (Props.MasterMaintained)
            {
                rbMaintained.Checked = true;
            }
            else
            {
                rbMomentary.Checked = true;
            }

            ckOnScreen.Checked = Props.ShowSwitches;
            rb4.Checked = Props.SwitchCount == 4;
            rb8.Checked = Props.SwitchCount == 8;
            rb12.Checked = Props.SwitchCount == 12;
            rb16.Checked = Props.SwitchCount == 16;
            rb4.Enabled = rb8.Enabled = rb12.Enabled = rb16.Enabled = ckOnScreen.Checked;
            ckWorkSwitch.Checked = Core.SwitchBox.WorkSwitchEnabled;
            ckRate.Checked = Core.SwitchBox.AutoRateEnabled;
            ckSections.Checked = Core.SwitchBox.AutoSectionEnabled;

            switch (Props.MasterSwitchMode)
            {
                case MasterSwitchMode.Standard:
                    rbMasterAll.Checked = true;
                    break;

                case MasterSwitchMode.Override:
                    rbMasterOverride.Checked = true;
                    break;
            }

            Initializing = false;
        }
    }
}