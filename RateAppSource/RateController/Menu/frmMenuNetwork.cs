using RateController.Classes;
using RateController.Language;
using RateController.PGNs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.Menu
{
    // Boards page: every board the app hears (PGN 32403). The selected board's module and
    // description are edited here; Save holds the change as pending (Core.SetPending) and
    // highlights Send, and Send sends it (frmMenu.SendModuleConfig).
    public partial class frmMenuNetwork : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private Timer cTimer;

        // one list row: a live board (MAC > 0), or a module with old firmware (MAC 0)
        private class Row
        {
            public ulong MAC;
            public int ID;
            public string Module;
            public string Board;
            public string State;
            public Color Fore;
            public string Key => MAC != 0 ? "M" + MAC.ToString() : "R" + ID.ToString();
            public string Signature => Key + "|" + Module + "|" + Board + "|" + State + "|" + Fore.ToArgb();
        }

        private readonly List<Row> cRows = new List<Row>();
        private readonly List<int> cModules = new List<int>();     // cbModule item i = cModules[i]
        private string cSelectedKey = "";
        private bool cRebuilding = false;

        public frmMenuNetwork(frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            this.Tag = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SetButtons(false);
            LoadSelected();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                BoardInfo Brd = SelectedBoard();
                if (Brd == null)
                {
                    // gone since it was selected
                    Props.ShowMessage(Lang.lgBoardGone, "Boards", 10000);
                    SetButtons(false);
                    LoadSelected();
                    return;
                }

                PendingBoard old = Core.PendingFor(Brd.MAC);
                int newID = SelectedModule();
                if (newID == Brd.ID) newID = -1;                    // back to its own module: no change

                string desc = tbDescription.Enabled ? tbDescription.Text.Trim() : null;
                if (desc != null && desc == Brd.Label) desc = null; // unchanged

                bool idChange = newID >= 0 && (old == null || old.NewID != newID);

                // a saved move is being undone: the config pages go back to the board's own
                // module. A new board (25) has no module of its own, so its pages stay put.
                bool idRevert = newID < 0 && old != null && old.NewID >= 0;
                bool pagesMove = idChange || (idRevert && Brd.ID <= 15);

                PendingBoard otherMove = Core.PendingIDChange();

                if (idChange && otherMove != null && otherMove.MAC != Brd.MAC)
                {
                    // one ID change per Send: Send gives settings to one module
                    Props.ShowMessage(Lang.lgSendPendingFirst, "Boards", 10000);
                }
                else if (pagesMove && MainMenu.AnyPageEdited(this))
                {
                    // the config pages move to the new module, which reloads them
                    Props.ShowMessage(Lang.lgSaveOrCancelFirst, "Boards", 10000);
                }
                else
                {
                    Core.SetPending(Brd.MAC, newID, desc);

                    if (idChange)
                    {
                        // the next Send gives this module's settings to the board
                        Core.ModuleConfig.ModuleID = (byte)newID;
                        Core.ModuleConfig.Save();
                        MainMenu.DefaultsSet();

                        // a new board has no settings yet: offer the starting configuration
                        // for its type. A moved board takes the module's existing settings.
                        ModuleTypes type = (ModuleTypes)Brd.InoType;
                        if (Brd.ID == Props.NewModuleID && clsPresets.Has(type)
                            && Confirm(string.Format(Lang.lgLoadPresetConfirm, clsPresets.Name(type), newID)))
                        {
                            clsPresets.Load(type, MainMenu);
                        }
                    }
                    else if (idRevert && Brd.ID <= 15)
                    {
                        Core.ModuleConfig.ModuleID = (byte)Brd.ID;
                        Core.ModuleConfig.Save();
                        MainMenu.DefaultsSet();
                    }

                    SetButtons(false);
                    if (Core.HasPendingBoards) MainMenu.HighlightUpdateButton();
                    RefreshPage();
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuNetwork/btnOK_Click: " + ex.Message);
            }
        }

        private bool Confirm(string Message)
        {
            bool Result;
            using (var Hlp = new frmMsgBox(Message.Replace("\r\n", "\n"), Lang.lgBoards))
            {
                Hlp.TopMost = true;
                Hlp.ShowDialog(this);
                Result = Hlp.Result;
            }
            return Result;
        }

        private void Edit_Changed(object sender, EventArgs e)
        {
            if (!Initializing)
            {
                SetButtons(true);
                UpdateDescriptionEnabled();
            }
        }

        private void frmMenuNetwork_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainMenu.MenuMoved -= MainMenu_MenuMoved;
            if (cTimer != null)
            {
                cTimer.Stop();
                cTimer.Dispose();
                cTimer = null;
            }
            Props.SaveFormLocation(this);
        }

        private void frmMenuNetwork_Load(object sender, EventArgs e)
        {
            SubMenuLayout.SetFormLayout(this, MainMenu, btnOK);
            btnCancel.Left = btnOK.Left - SubMenuLayout.ButtonSpacing;
            btnCancel.Top = btnOK.Top;

            MainMenu.StyleControls(this);
            lvBoards.BackColor = Properties.Settings.Default.MainBackColour;
            lvBoards.ForeColor = Properties.Settings.Default.MainForeColour;

            // theme text colour, the underline marks the link (default blue is unreadable on dark)
            lnkWeb.LinkColor = lnkWeb.ForeColor;
            lnkWeb.ActiveLinkColor = lnkWeb.ForeColor;
            lnkWeb.VisitedLinkColor = lnkWeb.ForeColor;

            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            this.BackColor = Properties.Settings.Default.MainBackColour;
            PositionForm();

            FitLastColumn();
            RefreshPage();

            // boards come and go while the page is open (32403 every 2 s)
            cTimer = new Timer();
            cTimer.Interval = 1000;
            cTimer.Tick += (s, a) => RefreshPage();
            cTimer.Start();
        }

        // fills the Module list and Description box from the selected board (its pending
        // change if it has one): the board's own module first, then the free modules
        private void LoadSelected()
        {
            Initializing = true;
            try
            {
                BoardInfo Brd = SelectedBoard();
                PendingBoard p = Brd == null ? null : Core.PendingFor(Brd.MAC);

                var modules = new List<int>();
                int want = -1;
                if (Brd != null)
                {
                    for (int i = 0; i < Props.MaxModules; i++)
                    {
                        // in order: the board's own module, free modules, and the target already saved for this board
                        if (i == Brd.ID || !Core.ModuleTaken(i) || (p != null && p.NewID == i)) modules.Add(i);
                    }
                    // a new board starts blank: choosing its module is the edit that enables Save
                    want = p != null && p.NewID >= 0 ? p.NewID
                        : Brd.ID < Props.MaxModules ? Brd.ID : -1;
                }

                // redrawn only when something differs (this runs every second)
                bool same = modules.Count == cModules.Count;
                for (int i = 0; same && i < modules.Count; i++) same = modules[i] == cModules[i];
                if (!same)
                {
                    cbModule.BeginUpdate();
                    cbModule.Items.Clear();
                    cModules.Clear();
                    foreach (int m in modules)
                    {
                        cbModule.Items.Add(m.ToString());
                        cModules.Add(m);
                    }
                    cbModule.EndUpdate();
                }
                int sel = cModules.IndexOf(want);
                if (cbModule.SelectedIndex != sel) cbModule.SelectedIndex = sel;
                bool cbOn = Brd != null && cModules.Count > 0;
                if (cbModule.Enabled != cbOn) cbModule.Enabled = cbOn;

                string text = p != null && p.Description != null ? p.Description : (Brd?.Label ?? "");
                if (tbDescription.Text != text) tbDescription.Text = text;
            }
            finally
            {
                Initializing = false;
            }
            UpdateDescriptionEnabled();
        }

        private void lvBoards_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cRebuilding || Initializing) return;

            string key = lvBoards.SelectedItems.Count > 0 ? (string)lvBoards.SelectedItems[0].Tag : "";
            if (key == cSelectedKey) return;

            if (cEdited)
            {
                // the edits belong to the board they were made for
                Props.ShowMessage(Lang.lgSaveOrCancelFirst, "Boards", 10000);
                SelectKey(cSelectedKey);
            }
            else
            {
                cSelectedKey = key;
                LoadSelected();
            }
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

        // assigned boards by module, then new boards; a module with old firmware gets a row
        // of its own (its MAC isn't known). Saved, unsent changes show in State.
        private List<Row> BuildRows()
        {
            var rows = new List<Row>();
            for (int id = 0; id < Props.MaxModules; id++)
            {
                var boards = Core.ModulesBoardID.BoardsAt(id);
                boards.Sort((a, b) => a.MAC.CompareTo(b.MAC));      // steady order for duplicates
                bool dup = boards.Count > 1;
                foreach (BoardInfo Brd in boards)
                {
                    rows.Add(BoardRow(Brd, id.ToString(),
                        dup ? Lang.lgStateDuplicate : Lang.lgConnected, dup ? Color.Red : lvBoards.ForeColor));
                }

                if (Core.ModulesBoardID.Rejected(id))
                {
                    rows.Add(new Row
                    {
                        ID = id,
                        Module = id.ToString(),
                        Board = "—",
                        State = Lang.lgUpdateFirmware,
                        Fore = Color.Red
                    });
                }
            }

            var newBoards = Core.ModulesBoardID.NewBoards();
            newBoards.Sort((a, b) => a.MAC.CompareTo(b.MAC));
            foreach (BoardInfo Brd in newBoards)
            {
                // a new board on another network is moved there only if the user agrees at Send
                string state = Brd.OtherNetwork ? string.Format(Lang.lgStateNewNetwork, Brd.Network) : Lang.lgStateNew;
                rows.Add(BoardRow(Brd, Lang.lgNew, state, Color.Blue));
            }
            return rows;
        }

        private Row BoardRow(BoardInfo Brd, string Module, string State, Color Fore)
        {
            PendingBoard p = Core.PendingFor(Brd.MAC);
            if (p != null)
            {
                State = p.NewID >= 0 ? string.Format(Lang.lgPendingModule, p.NewID) : Lang.lgPendingDescription;
                Fore = Color.Blue;
            }
            return new Row { MAC = Brd.MAC, ID = Brd.ID, Module = Module, Board = Brd.Text, State = State, Fore = Fore };
        }

        private void RefreshPage()
        {
            RefreshRows();
            if (!cEdited && !cbModule.DroppedDown)
            {
                // free modules change as boards come and go; edits in progress are left alone
                LoadSelected();
            }
            ShowWebLink();
        }

        // The selected board's web page, as on the Comm page's WiFi tab. Only an ESP32 has one,
        // and only over WiFi: its Ethernet chip runs a separate stack with no web server. The
        // module IP is known by module ID, so only an assigned board with no duplicate gets one.
        private void ShowWebLink()
        {
            string Text = "";
            string Url = "";
            BoardInfo Brd = SelectedBoard();
            if (Brd != null && Brd.InoType == (byte)ModuleTypes.ESP_Rate && Brd.ID < Props.MaxModules
                && !Core.ModulesBoardID.DuplicateID(Brd.ID) && Core.ModulesStatus.Connected(Brd.ID))
            {
                if (Core.ModulesStatus.EthernetConnected(Brd.ID))
                {
                    Text = Lang.lgEthernetNoWebPage;
                }
                else
                {
                    string IP = Core.UDPmodules == null ? "" : Core.UDPmodules.ModuleIP(Brd.ID);
                    if (IP != "")
                    {
                        Url = "http://" + IP + "/";
                        Text = IP;
                    }
                }
            }

            if (lnkWeb.Text != Text || (lnkWeb.Links.Count > 0) != (Url != ""))
            {
                lnkWeb.Text = Text;
                lnkWeb.Links.Clear();
                if (Url != "") lnkWeb.Links.Add(0, Text.Length, Url);
            }
            bool show = Text != "";
            if (lnkWeb.Visible != show) lnkWeb.Visible = show;
        }

        private void lnkWeb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (e.Link.LinkData is string Url) System.Diagnostics.Process.Start(Url);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuNetwork/lnkWeb_LinkClicked: " + ex.Message);
            }
        }

        // rebuilt only when something changed, keeping the selection (by MAC)
        private void RefreshRows()
        {
            var rows = BuildRows();

            bool changed = rows.Count != cRows.Count;
            for (int i = 0; !changed && i < rows.Count; i++) changed = rows[i].Signature != cRows[i].Signature;

            if (changed)
            {
                // clearing the list fires SelectedIndexChanged; don't let it drop the selection
                cRebuilding = true;
                lvBoards.BeginUpdate();
                lvBoards.Items.Clear();
                cRows.Clear();
                foreach (Row r in rows)
                {
                    var item = new ListViewItem(new[] { r.Module, r.Board, r.State })
                    {
                        Tag = r.Key,
                        ForeColor = r.Fore,
                        UseItemStyleForSubItems = true
                    };
                    lvBoards.Items.Add(item);
                    cRows.Add(r);
                }
                lvBoards.EndUpdate();
                FitLastColumn();

                // keep the selected board; with nothing selected (or it went, and nothing is
                // being edited), pick the first new board
                string key = cSelectedKey;
                if (cRows.FindIndex(r => r.Key == key) < 0 && !cEdited)
                {
                    int n = cRows.FindIndex(r => r.ID == Props.NewModuleID);
                    key = n >= 0 ? cRows[n].Key : "";
                }
                SelectKey(key);
                cRebuilding = false;

                if (key != cSelectedKey && !cEdited)
                {
                    cSelectedKey = key;
                    LoadSelected();
                }
            }
        }

        // State takes the rest of the list's width, so there is no blank strip on the right.
        // ClientSize leaves out the scroll bar, which comes and goes with the row count.
        private void FitLastColumn()
        {
            int w = lvBoards.ClientSize.Width - chModule.Width - chBoard.Width;
            if (w > 0 && chState.Width != w) chState.Width = w;
        }

        private void SelectKey(string Key)
        {
            bool was = cRebuilding;
            cRebuilding = true;
            int i = cRows.FindIndex(r => r.Key == Key);
            if (i >= 0)
            {
                lvBoards.Items[i].Selected = true;
                lvBoards.Items[i].Focused = true;
            }
            else
            {
                lvBoards.SelectedItems.Clear();
            }
            cRebuilding = was;
        }

        // the live board in the selected row, or null (none selected, gone, or an old-firmware row)
        private BoardInfo SelectedBoard()
        {
            BoardInfo Result = null;
            int i = cRows.FindIndex(r => r.Key == cSelectedKey);
            if (i >= 0 && cRows[i].MAC != 0) Result = Core.ModulesBoardID.LiveBoard(cRows[i].MAC);
            return Result;
        }

        private int SelectedModule()
        {
            int i = cbModule.SelectedIndex;
            return (i >= 0 && i < cModules.Count) ? cModules[i] : -1;
        }

        private void SetButtons(bool Edited)
        {
            if (!Initializing)
            {
                btnCancel.Enabled = Edited;
                btnOK.Enabled = Edited;
                cEdited = Edited;
                this.Tag = cEdited;
            }
        }

        private void SetLanguage()
        {
            chModule.Text = Lang.lgModule;
            chBoard.Text = Lang.lgBoard;
            chState.Text = Lang.lgState;
            lbModule.Text = Lang.lgModule;
            lbDescription.Text = Lang.lgDescription;
        }

        // 32506 is addressed by module ID: it reaches the board at 0-15 when no other board
        // shares the ID, or at the new ID it is being given in the same Send. A new board
        // with no module chosen, or a duplicate, can't take a description. A Nano stores
        // no description (it has no 32506 handler), so it never takes one.
        private void UpdateDescriptionEnabled()
        {
            BoardInfo Brd = SelectedBoard();
            bool enabled = false;
            if (Brd != null && (ModuleTypes)Brd.InoType != ModuleTypes.Nano_Rate)
            {
                bool moving = SelectedModule() >= 0 && SelectedModule() != Brd.ID;
                bool reachable = Brd.ID < Props.MaxModules && !Core.ModulesBoardID.DuplicateID(Brd.ID);
                enabled = moving || reachable;
            }
            if (tbDescription.Enabled != enabled) tbDescription.Enabled = enabled;
        }
    }
}
