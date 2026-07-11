using AgOpenGPS;
using RateController.Classes;
using RateController.Language;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuConfig : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private Timer cModuleTimer;
        private Color cModuleColor;

        public frmMenuConfig(frmMenu menu)
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

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                // tbModuleID is live (applied on change), not part of Save

                // A new ID makes the next Send an ID assignment (PGN 32700 bit 6): the
                // board adopts byte 2 instead of filtering on it, so only one board may
                // be connected. The flag is read and cleared by butUpdateModules.
                // Saving without a new ID cancels any pending assignment, so byte 2
                // goes back to being a filter.
                if (byte.TryParse(tbNewID.Text, out byte newID))
                {
                    Core.ModuleConfig.ModuleID = newID;
                    Props.SetProp("ModuleConfig_AssignPending", "True");
                }
                else
                {
                    Props.SetProp("ModuleConfig_AssignPending", "False");
                }

                // description belongs to the module the board is about to be
                string desc = tbNewDescription.Text.Trim();
                if (desc.Length > 0)
                {
                    Props.SetModuleDescription(Core.ModuleConfig.GetData()[2], desc);
                }

                if (byte.TryParse(tbSensorCount.Text, out byte ct)) Core.ModuleConfig.SensorCount = ct;
                Core.ModuleConfig.InvertRelay = ckRelayOn.Checked;
                Core.ModuleConfig.InvertFlow = ckFlowOn.Checked;
                Core.ModuleConfig.ADS1115enabled = ckADS1115enabled.Checked;
                Core.ModuleConfig.OnboardRelayType = (byte)cbOnboardRelays.SelectedIndex;
                Core.ModuleConfig.RemoteRelayType = (byte)cbRemoteRelays.SelectedIndex;

                Core.ModuleConfig.Save();

                SetButtons(false);
                UpdateForm();

                // refresh the other open module pages too - the Pins page row
                // count follows the sensor count saved here
                MainMenu.DefaultsSet();
                MainMenu.HighlightUpdateButton();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuConfig/btnOk_Click: " + ex.Message);
            }
        }

        private void btnRescan_Click(object sender, EventArgs e)
        {
            // the module being edited (tbModuleID) is navigation, not a setting -
            // resetting defaults leaves it alone
            ckRelayOn.Checked = true;
            ckFlowOn.Checked = true;
            ckADS1115enabled.Checked = false;
            tbSensorCount.Text = "1";
            cbOnboardRelays.SelectedIndex = 0;
        }

        private void frmMenuConfig_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainMenu.MenuMoved -= MainMenu_MenuMoved;
            MainMenu.ModuleDefaultsSet -= MainMenu_ModuleDefaultsSet;
            if (cModuleTimer != null)
            {
                cModuleTimer.Stop();
                cModuleTimer.Dispose();
                cModuleTimer = null;
            }
            Props.SaveFormLocation(this);
        }

        private void frmMenuConfig_Load(object sender, EventArgs e)
        {
            SubMenuLayout.SetFormLayout(this, MainMenu, btnOK);

            btnCancel.Left = btnOK.Left - SubMenuLayout.ButtonSpacing;
            btnCancel.Top = btnOK.Top;
            MainMenu.StyleControls(this);

            lbModule.Font = new Font(lbModule.Font, FontStyle.Underline);
            cModuleColor = lbModule.ForeColor;

            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            MainMenu.ModuleDefaultsSet += MainMenu_ModuleDefaultsSet;
            PositionForm();

            // one entry per module; RefreshModuleList keeps the connection state current
            Initializing = true;
            for (int i = 0; i < Props.MaxModules; i++)
            {
                cbModuleID.Items.Add(i.ToString());
            }
            Initializing = false;
            RefreshModuleList();

            UpdateForm();

            // keep the module identity line live while the form is open - the module
            // reports its label every ~2 s (PGN 32403) and connection state can change
            cModuleTimer = new Timer();
            cModuleTimer.Interval = 1000;
            cModuleTimer.Tick += ModuleTimer_Tick;
            cModuleTimer.Start();
        }

        private void ModuleTimer_Tick(object sender, EventArgs e)
        {
            RefreshModuleList();

            byte id = Core.ModuleConfig.GetData()[2];
            bool dup = Core.DuplicateModule(id);

            string text;
            if (dup)
            {
                text = "Two boards are answering as module " + id.ToString() + "!";
            }
            else
            {
                text = Core.ModuleConfigDescription();
            }

            if (lbModule.Text != text) lbModule.Text = text;
            Color fore = dup ? Color.Red : cModuleColor;
            if (lbModule.ForeColor != fore) lbModule.ForeColor = fore;
        }

        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
            Props.DrawGroupBox((GroupBox)sender, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }

        private void MainMenu_MenuMoved(object sender, EventArgs e)
        {
            PositionForm();
        }

        private void MainMenu_ModuleDefaultsSet(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
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
            lbModuleID.Text = Lang.lgModuleToEdit;
            lbSensorCount.Text = Lang.lgSensorCount;
            lbRelay.Text = Lang.lgRelayControl;
            lbRemoteRelay.Text = Lang.lgRemoteRelayControl;
            ckRelayOn.Text = Lang.lgInvertRelays;
            ckFlowOn.Text = Lang.lgInvertFlow;
        }

        private void Setting_Changed(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void cbModuleID_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Selecting which module to edit is navigation, not a config change: it
            // takes effect immediately, without Save and without highlighting the
            // Send button. Refused while any module page holds unsaved edits, since
            // retargeting reloads them all.
            if (!Initializing && cbModuleID.SelectedIndex >= 0)
            {
                byte id = (byte)cbModuleID.SelectedIndex;
                byte current = Core.ModuleConfig.GetData()[2];
                if (id != current)
                {
                    bool edited = false;
                    foreach (Form frm in MainMenu.OwnedForms)
                    {
                        if (frm.Tag is bool b && b) edited = true;
                    }

                    if (edited)
                    {
                        Props.ShowMessage("Save or cancel the pending changes first.", "Config", 10000);
                        Initializing = true;
                        cbModuleID.SelectedIndex = current;
                        Initializing = false;
                    }
                    else
                    {
                        // a pending ID assignment belonged to the previous target -
                        // cancel it so byte 2 is a plain filter for the new one
                        Core.ModuleConfig.ModuleID = id;
                        Core.ModuleConfig.Save();
                        Props.SetProp("ModuleConfig_AssignPending", "False");
                        MainMenu.DefaultsSet();
                    }
                }
            }
        }

        private void RefreshModuleList()
        {
            // item text shows live connection state; item index == module ID.
            // Left alone while the list is dropped down so it doesn't repaint
            // under the finger.
            if (!cbModuleID.DroppedDown)
            {
                bool wasInitializing = Initializing;
                Initializing = true;
                for (int i = 0; i < cbModuleID.Items.Count; i++)
                {
                    string text = i.ToString() + (Core.ModulesStatus.Connected(i) ? "  (Connected)" : "  (not connected)");
                    if ((string)cbModuleID.Items[i] != text) cbModuleID.Items[i] = text;
                }
                Initializing = wasInitializing;
            }
        }

        private void tbNewID_Enter(object sender, EventArgs e)
        {
            double temp;
            double.TryParse(tbNewID.Text, out temp);
            using (var form = new FormNumeric(0, Props.MaxModules - 1, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbNewID.Text = form.ReturnValue.ToString("N0");
                }
            }
        }

        private void tbSensorCount_Enter(object sender, EventArgs e)
        {
            double temp;
            double.TryParse(tbSensorCount.Text, out temp);

            // cap by the target module's board type and firmware (2, or 6 on
            // 6-product boards with current firmware); a pending new ID wins
            if (!byte.TryParse(tbNewID.Text, out byte id))
            {
                id = Core.ModuleConfig.GetData()[2];   // the dropdown is live-synced to the config
            }
            using (var form = new FormNumeric(0, Core.MaxSensorsForModule(id), temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbSensorCount.Text = form.ReturnValue.ToString("N0");
                }
            }
        }

        private void UpdateForm()
        {
            Initializing = true;
            try
            {
                byte[] data = Core.ModuleConfig.GetData();
                lbModule.Text = Core.ModuleConfigDescription();
                if (data[2] < cbModuleID.Items.Count) cbModuleID.SelectedIndex = data[2];
                tbNewID.Text = "";
                tbNewDescription.Text = "";
                tbSensorCount.Text = data[3].ToString();
                cbOnboardRelays.SelectedIndex = data[5];
                cbRemoteRelays.SelectedIndex = data[6];
                ckRelayOn.Checked = Core.ModuleConfig.InvertRelay;
                ckFlowOn.Checked = Core.ModuleConfig.InvertFlow;
                ckADS1115enabled.Checked = Core.ModuleConfig.ADS1115enabled;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuConfig/UpdateForm: " + ex.Message);
            }
            Initializing = false;
        }

        private void lbModuleID_Click(object sender, EventArgs e)
        {

        }
    }
}