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
        private bool cRefusing = false;     // a New ID refusal message is queued
        private readonly List<ulong> cBoardMACs = new List<ulong>();   // cbBoard item i = cBoardMACs[i]

        private static string SendFirstMessage => Lang.lgSendBeforeIDChange;

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
                // cbModuleID is live (applied on change), not part of Save
                byte oldID = Core.ModuleConfig.GetData()[2];
                bool idChange = byte.TryParse(tbNewID.Text, out byte newID) && newID != oldID;
                if (idChange && MainMenu.ChangesWaitingToSend)
                {
                    // another page was saved after the New ID was entered: those changes
                    // go out on a normal Send first, then the ID can be changed
                    Props.ShowMessage(SendFirstMessage, "Config", 10000);
                }
                else if (idChange && SelectedBoardMAC() == 0)
                {
                    // the board to move must be picked first: the app doesn't guess (§6).
                    // The New ID stays entered so the board can be picked and OK pressed again.
                    Props.ShowMessage(cbBoard.Items.Count == 0 ? string.Format(Lang.lgNoBoardFound, oldID)
                        : Lang.lgChooseBoard, "Config", 10000);
                }
                else if (idChange)
                {
                    // An ID change is its own step: the rest of the page is locked while
                    // a New ID is entered, and it is sent as soon as it is confirmed.
                    // Cancel, or a refused send, leaves the module ID as it was.
                    string msg = string.Format(Lang.lgChangeIDConfirm, oldID, newID).Replace("\r\n", "\n");

                    bool confirmed;
                    using (var Hlp = new frmMsgBox(msg, Lang.lgChangeModuleID))
                    {
                        Hlp.TopMost = true;
                        Hlp.ShowDialog(this);
                        confirmed = Hlp.Result;
                    }

                    bool assigned = confirmed && MainMenu.AssignModuleID(newID, SelectedBoardMAC());

                    SetButtons(false);
                    UpdateForm();
                    if (assigned) MainMenu.DefaultsSet();   // every module page now edits the new ID
                }
                else
                {
                    // the board name is sent once with the next Send, then cleared
                    string desc = tbNewDescription.Text.Trim();
                    if (desc.Length > 0)
                    {
                        Props.SetPendingModuleDescription(oldID, desc);
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
            RefreshBoardList();

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
            RefreshBoardList();

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

            // an unassigned board (ID 25) is reporting: say so, and how to give it an ID
            var newBoards = Core.ModulesBoardID.NewBoards();
            bool newBoard = newBoards.Count > 0;
            if (newBoard)
            {
                string type = Enum.GetName(typeof(ModuleTypes), newBoards[0].InoType) ?? "unknown";
                string nb = string.Format(Lang.lgNewBoardLine, type);
                if (lbNewBoard.Text != nb) lbNewBoard.Text = nb;
            }
            if (lbNewBoard.Visible != newBoard) lbNewBoard.Visible = newBoard;
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
            label1.Text = Lang.lgNewModuleID;
            label2.Text = Lang.lgNewBoardDescription;
            lbBoard.Text = Lang.lgBoard;
        }

        private void Setting_Changed(object sender, EventArgs e)
        {
            SetButtons(true);
            if (sender == tbNewID) SetIDLock(tbNewID.Text.Length > 0);
        }

        // While a New ID is entered nothing else on the page can change: the ID change
        // is sent on its own when it is confirmed. Clearing the box (Cancel) unlocks.
        private void SetIDLock(bool Locked)
        {
            bool enabled = !Locked;
            cbModuleID.Enabled = enabled;
            tbNewDescription.Enabled = enabled;
            tbSensorCount.Enabled = enabled;
            cbOnboardRelays.Enabled = enabled;
            cbRemoteRelays.Enabled = enabled;
            ckRelayOn.Enabled = enabled;
            ckFlowOn.Enabled = enabled;
            ckADS1115enabled.Enabled = enabled;
            btnRescan.Enabled = enabled;
        }

        // true when this page or any other open module page holds unsaved edits
        private bool AnyPageEdited()
        {
            bool edited = false;
            foreach (Form frm in MainMenu.OwnedForms)
            {
                if (frm.Tag is bool b && b) edited = true;
            }
            return edited;
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
                    if (AnyPageEdited())
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

        private void tbNewID_Enter(object sender, EventArgs e)
        {
            // an ID change is sent on its own, so it can't start while other changes
            // are waiting to be saved (they would be locked out, not sent) or sent
            // (a pin the board rejects would reject the ID with it)
            string refusal = "";
            if (tbNewID.Text.Length == 0 && AnyPageEdited())
            {
                refusal = Lang.lgSaveOrCancelFirst;
            }
            else if (tbNewID.Text.Length == 0 && MainMenu.ChangesWaitingToSend)
            {
                refusal = SendFirstMessage;
            }

            if (refusal.Length > 0)
            {
                // Focus must be off the box before the message: the message window takes
                // focus, and when focus comes back to tbNewID this Enter runs again (a
                // second message). Changing focus inside Enter doesn't stick, so the move
                // and the message run after this event; cRefusing drops a repeat Enter
                // that arrives before then.
                if (!cRefusing)
                {
                    cRefusing = true;
                    BeginInvoke((Action)(() =>
                    {
                        cbModuleID.Focus();
                        Props.ShowMessage(refusal, "Config", 10000);
                        cRefusing = false;
                    }));
                }
            }
            else
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
            SetIDLock(false);   // New ID was just cleared
            RefreshBoardList();
        }

        // boards that can take a new ID: those at the module being edited, then new boards.
        // With exactly one it's picked; with more the user must choose (the app doesn't guess).
        // Picking a board is not a settings change: no Save, no Send highlight.
        private void RefreshBoardList()
        {
            if (cbBoard.DroppedDown) return;     // don't repaint under the finger

            byte id = Core.ModuleConfig.GetData()[2];
            var boards = Core.ModulesBoardID.BoardsAt(id);
            boards.AddRange(Core.ModulesBoardID.NewBoards());

            var macs = new List<ulong>();
            var texts = new List<string>();
            foreach (var Brd in boards)
            {
                string type = (Enum.GetName(typeof(ModuleTypes), Brd.InoType) ?? "").Replace('_', ' ');
                string at = Brd.ID == Props.NewModuleID ? Lang.lgNew : Brd.ID.ToString();
                string text = at + ": " + type + " " + Brd.ShortMAC;
                if (Brd.Label.Length > 0) text += " " + Brd.Label;
                macs.Add(Brd.MAC);
                texts.Add(text);
            }

            bool changed = macs.Count != cBoardMACs.Count || texts.Count != cbBoard.Items.Count;
            for (int i = 0; !changed && i < macs.Count; i++)
            {
                changed = macs[i] != cBoardMACs[i] || texts[i] != (string)cbBoard.Items[i];
            }

            if (changed)
            {
                ulong selected = SelectedBoardMAC();
                cbBoard.BeginUpdate();
                cbBoard.Items.Clear();
                cBoardMACs.Clear();
                for (int i = 0; i < macs.Count; i++)
                {
                    cbBoard.Items.Add(texts[i]);
                    cBoardMACs.Add(macs[i]);
                }
                int keep = cBoardMACs.IndexOf(selected);
                if (keep < 0 && cBoardMACs.Count == 1) keep = 0;    // only one choice: pick it
                cbBoard.SelectedIndex = keep;
                cbBoard.EndUpdate();
            }
        }

        // the MAC of the board picked in the Board list, 0 when none
        private ulong SelectedBoardMAC()
        {
            int i = cbBoard.SelectedIndex;
            return (i >= 0 && i < cBoardMACs.Count) ? cBoardMACs[i] : 0;
        }

        private void lbModuleID_Click(object sender, EventArgs e)
        {

        }
    }
}