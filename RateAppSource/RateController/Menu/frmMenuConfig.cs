using AgOpenGPS;
using RateController.Classes;
using RateController.Language;
using System;
using System.Collections.Generic;
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
                // cbModuleID is live (applied on change), not part of Save; boards and their
                // descriptions are handled on the Boards page
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

        // Loads the starting configuration for the board reporting at this module, or for
        // the board type the user picks when none is reporting. Not while edits are unsaved:
        // the preset is saved and reloads every module page.
        private void btnPreset_Click(object sender, EventArgs e)
        {
            try
            {
                if (MainMenu.AnyPageEdited())
                {
                    Props.ShowMessage(Lang.lgSaveOrCancelFirst, "Config", 10000);
                }
                else
                {
                    byte id = Core.ModuleConfig.GetData()[2];
                    ModuleTypes type = Core.ModulesStatus.Connected(id)
                        ? (ModuleTypes)Core.ModulesBoardID.ModuleType(id) : ModuleTypes.unknown;

                    bool load;
                    if (clsPresets.Has(type))
                    {
                        string msg = string.Format(Lang.lgLoadPresetConfirm, clsPresets.Name(type), id).Replace("\r\n", "\n");
                        using (var Hlp = new frmMsgBox(msg, "Config"))
                        {
                            Hlp.TopMost = true;
                            Hlp.ShowDialog(this);
                            load = Hlp.Result;
                        }
                    }
                    else
                    {
                        type = clsPresets.Choose(this);
                        load = clsPresets.Has(type);
                    }

                    if (load) clsPresets.Load(type, MainMenu);     // DefaultsSet reloads this page
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuConfig/btnPreset_Click: " + ex.Message);
            }
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
            btnPreset.Left = btnCancel.Left - SubMenuLayout.ButtonSpacing;
            btnPreset.Top = btnOK.Top;
            float typeSize = lbModuleType.Font.Size;      // StyleControls sets every label to MenuFontSmall
            MainMenu.StyleControls(this);
            lbModuleType.Font = new Font(lbModuleType.Font.FontFamily, typeSize);

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
            // old firmware outranks a duplicate: nothing else can be done until it's updated
            bool rejected = Core.ModulesBoardID.Rejected(id);
            bool dup = !rejected && Core.DuplicateModule(id);

            string text;
            if (dup)
            {
                text = string.Format(Lang.lgTwoBoardsAnswering, id);
            }
            else
            {
                text = Core.ModuleConfigDescription();
            }

            if (lbModule.Text != text) lbModule.Text = text;
            Color fore = (dup || rejected) ? Color.Red : cModuleColor;
            if (lbModule.ForeColor != fore) lbModule.ForeColor = fore;

            // board type as the module reports it (PGN 32403), only while it is reporting
            string typeName = "";
            if (Core.ModulesStatus.Connected(id))
            {
                typeName = Enum.GetName(typeof(ModuleTypes), Core.ModulesBoardID.ModuleType(id)) ?? "";
                if (typeName == ModuleTypes.unknown.ToString()) typeName = "";
                typeName = typeName.Replace('_', ' ');
            }
            if (lbModuleType.Text != typeName) lbModuleType.Text = typeName;
            bool showType = typeName.Length > 0;
            if (lbModuleType.Visible != showType) lbModuleType.Visible = showType;
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
                    if (MainMenu.AnyPageEdited())
                    {
                        Props.ShowMessage(Lang.lgSaveOrCancelFirst, "Config", 10000);
                        Initializing = true;
                        cbModuleID.SelectedIndex = current;
                        Initializing = false;
                    }
                    else
                    {
                        Core.ModuleConfig.ModuleID = id;
                        Core.ModuleConfig.Save();
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
                    string state = Core.ModulesBoardID.Rejected(i) ? Lang.lgUpdateFirmware
                        : Core.ModulesStatus.Connected(i) ? Lang.lgConnected : Lang.lgNotConnected;
                    string text = i.ToString() + "  (" + state + ")";
                    if ((string)cbModuleID.Items[i] != text) cbModuleID.Items[i] = text;
                }
                Initializing = wasInitializing;
            }
        }

        private void tbSensorCount_Enter(object sender, EventArgs e)
        {
            double temp;
            double.TryParse(tbSensorCount.Text, out temp);

            // cap by the module's board type and firmware (2, or 6 on 6-product boards
            // with current firmware); the dropdown is live-synced to the config
            byte id = Core.ModuleConfig.GetData()[2];
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
    }
}