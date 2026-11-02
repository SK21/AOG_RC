using RateController.Classes;
using RateController.Forms;
using RateController.Language;
using RateController.PGNs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuComm : Form
    {
        // shown once per page visit when this PC's WiFi is on 5 GHz (from YieldFlo clsTabletHotspot.BandFixText)
        private const string BandFixSteps =
            "Set this PC's WiFi to use 2.4 GHz:\r\n"
            + "1. Press Windows+R, type ncpa.cpl and press Enter.\r\n"
            + "2. Right-click Wi-Fi and choose Properties, then Configure, then the Advanced tab.\r\n"
            + "3. Set Preferred Band to Prefer 2.4GHz band. On some PCs it is called Band or Wireless Mode.\r\n"
            + "4. Press OK and wait for the PC to reconnect.\r\n"
            + "Or set the network itself to 2.4 GHz, if the phone or router has that setting.";

        private const string ModulePrefix = "RateModule";      // RC_ESP32 APname default, + "_" + MAC

        private bool BandTold = false;
        private bool cEdited;
        private int HotspotTicks = 0;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private string PcChannelSsid = "";
        private int PcChannel = 0;
        private string PcProfile = "";
        private string PcSsid = "";
        private ModuleProvisioner Provisioner;      // the setup run in progress, null when idle
        private bool RestoringPick = false;         // lstModules selection being put back during setup
        private string SetupSsid = "";              // module hotspot of the setup run in progress
        private Wlan.Adapter WifiAdapter;
        private bool WifiScanning = false;

        private sealed class ModuleItem
        {
            public string Ssid;
            public int Signal;
            public bool Secured;
            public int NameID = -1;     // ID from a RateModule_<ID>_<6 hex> name, -1 for older or other names

            public override string ToString()
            {
                // short: the list is 300 wide beside the Module ID box; the ID is in the name
                return Ssid + "  " + Signal + "%";
            }
        }

        private sealed class ModuleIDItem
        {
            public int ID;
            public bool Connected;

            public override string ToString()
            {
                return ID + (Connected ? " (Connected)" : "");
            }
        }

        private sealed class WifiTarget
        {
            public bool Hotspot;    // this tablet's own hotspot, else this PC's WiFi
            public string Ssid;

            public override string ToString()
            {
                return (Hotspot ? "Tablet hotspot: " : "This PC's WiFi: ") + Ssid;
            }
        }

        public frmMenuComm(frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            this.Tag = false;
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            SetButtons(false);
            UpdateForm();
        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            // Ethernet settings
            Core.UDPmodules.NetworkEP = cbEthernet.Text;

            // CAN settings
            bool CanSettingsChanged = false;
            if (rbAdapter2.Checked)
            {
                if (Props.CurrentCanDriver != CanDriver.InnoMaker) CanSettingsChanged = true;
                Props.CurrentCanDriver = CanDriver.InnoMaker;
            }
            else if (rbAdapter3.Checked)
            {
                if (Props.CurrentCanDriver != CanDriver.PCAN) CanSettingsChanged = true;
                Props.CurrentCanDriver = CanDriver.PCAN;
            }
            else
            {
                if (Props.CurrentCanDriver != CanDriver.SLCAN) CanSettingsChanged = true;
                Props.CurrentCanDriver = CanDriver.SLCAN;
            }

            if (cbComPort.SelectedItem != null)
            {
                if (Props.CurrentCanDriver == CanDriver.SLCAN && Props.CanPort != cbComPort.SelectedItem.ToString()) CanSettingsChanged = true;
                Props.CanPort = cbComPort.SelectedItem.ToString();
            }

            // driver / COM port changes only matter while CAN is in use; with Ethernet they're
            // just saved (a new install's default COM7 isn't on most PCs, so the dropdown's
            // first port looked like a change and restarted comms with an "Ethernet enabled" popup)
            if (!ckCanBus.Checked && !Props.CanEnabled) CanSettingsChanged = false;

            // Check if diagnostics setting changed
            bool diagnosticsChanged = Props.ShowCanDiagnostics != ckDiagnostics.Checked;
            if (diagnosticsChanged) Props.ShowCanDiagnostics = ckDiagnostics.Checked;

            // only a transport flip reaches the module - a driver or COM port change just
            // restarts the bridge locally, so it must not claim to have sent anything or
            // write the module config back to the profile
            bool ModeChanged = Props.CanEnabled != ckCanBus.Checked;
            if (ModeChanged) CanSettingsChanged = true;
            if (CanSettingsChanged)
            {
                int Result = Core.UseCanComm(ckCanBus.Checked);

                string Mes = "";
                switch (Result)
                {
                    case 1:
                        Mes = "CanBus enabled.";
                        break;

                    case 3:
                        Mes = "CanBus failed to start. Ethernet enabled.";
                        break;

                    default:
                        Mes = "Ethernet enabled.";
                        break;
                }

                if (ModeChanged)
                {
                    Core.ModuleConfig.Save();
                    Mes = "Comm change sent to module. " + Mes;
                }
                Props.ShowMessage(Mes, "Config", 10000);
            }

            // tablet hotspot (moved here from Options -> Phone)
            bool HotspotChanged = ckHotspot.Checked != Props.TabletHotspot;
            Props.TabletHotspot = ckHotspot.Checked;
            if (HotspotChanged) ApplyHotspot();

            SetButtons(false);
            UpdateForm();
        }

        private void ApplyHotspot()
        {
            if (ckHotspot.Checked)
            {
                // firewall rules need admin, only asked when a rule is missing
                if (Core.WebServer.IsRunning && !Core.WebServer.HotspotAllowed() && !Core.WebServer.AllowHotspot())
                {
                    Props.ShowMessage(Lang.lgFirewallNotAllowed, Lang.lgPhone, 5000);
                }
                lbHotspot.Text = Lang.lgStarting;
                Core.WebServer.HotspotWatchReset();
                Core.WebServer.HotspotAsync("start", HotspotDone);
            }
            else
            {
                lbHotspot.Text = Lang.lgStopping;
                Core.WebServer.HotspotAsync("stop", HotspotDone);
            }
        }

        private void btnWifiScan_Click(object sender, EventArgs e)
        {
            StartScan();
        }

        private void btnWifiSetup_Click(object sender, EventArgs e)
        {
            try
            {
                WifiTarget Target = cbWifiTarget.SelectedItem as WifiTarget;
                ModuleItem Module = lstModules.SelectedItem as ModuleItem;
                ModuleIDItem NewID = cbWifiModuleID.SelectedItem as ModuleIDItem;
                string Blocked = ModuleProvisioner.BlockedReason();
                bool Go = false;

                if (cEdited)
                {
                    Log("Save or cancel the changes on this page first.");
                }
                else if (Blocked != "")
                {
                    Log(Blocked);
                }
                else if (Target == null || Module == null || NewID == null || WifiAdapter == null)
                {
                    Log("Choose a network, a module and a module ID first.");
                }
                else if (Target.Hotspot && Core.WebServer.HotspotState != "On")
                {
                    Log("The tablet hotspot is not on. The module would have nothing to join.");
                }
                else
                {
                    // two boards with one ID both act on that module's commands
                    Go = !Core.ModulesStatus.Connected(NewID.ID)
                        || Confirm("Module " + NewID.ID + " is already connected. Only choose it if you are setting up that same module.");
                    if (Go && !Target.Hotspot && tbWifiKey.Text == "")
                    {
                        Go = Confirm("Send an empty password? Only for an open network.");
                    }
                }

                if (Go)
                {
                    WifiStore.ModuleKey = tbModuleKey.Text;
                    ProvisionRequest Req = new ProvisionRequest
                    {
                        Adapter = WifiAdapter.Guid,
                        ModuleSsid = Module.Ssid,
                        ModuleKey = tbModuleKey.Text,
                        ModuleID = NewID.ID
                    };

                    if (Target.Hotspot)
                    {
                        Req.TabletHotspot = true;
                        Req.HomeSsid = Core.WebServer.HotspotSsid;
                        Req.HomeKey = Core.WebServer.HotspotPassword;
                        Req.HomeProfile = PcProfile;    // the PC's own WiFi to go back to, "" = none
                        Req.UplinkSsid = PcSsid;
                    }
                    else
                    {
                        WifiStore.HomeSsid = Target.Ssid;
                        WifiStore.HomeKey = tbWifiKey.Text;
                        Req.HomeSsid = Target.Ssid;
                        Req.HomeKey = tbWifiKey.Text;
                        Req.HomeProfile = PcProfile;
                        Req.HomeChannel = (PcChannel >= 1 && PcChannel <= 13) ? PcChannel : 0;
                    }
                    StartSetup(Req);
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuComm/btnWifiSetup: " + ex.Message);
            }
        }

        private void btnWifiStop_Click(object sender, EventArgs e)
        {
            if (Provisioner != null)
            {
                Provisioner.Cancel();
                Log("Cancelling, this PC is put back on its network first ...");
                btnWifiStop.Enabled = false;
            }
        }

        private void cbWifiTarget_SelectedIndexChanged(object sender, EventArgs e)
        {
            // password box follows the target: the hotspot's password is known, a WiFi
            // network's is typed, prefilled only when the saved one belongs to that network
            WifiTarget Target = cbWifiTarget.SelectedItem as WifiTarget;
            if (Target != null && !Target.Hotspot)
            {
                tbWifiKey.Text = (WifiStore.HomeSsid == Target.Ssid) ? WifiStore.HomeKey : "";
            }
            else
            {
                tbWifiKey.Text = "";
            }
            ShowBand();
            EnableWifi();
        }

        private void ckHotspot_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
            ShowHotspot();      // the state line is blank while unticked
            ReadWifiNetwork();  // and the hotspot leaves (or rejoins) the network list
        }

        private bool Confirm(string Message)
        {
            var Hlp = new frmMsgBox(Message, "Module WiFi", true);
            Hlp.TopMost = true;
            Hlp.ShowDialog();
            bool Result = Hlp.Result;
            Hlp.Close();
            return Result;
        }

        private void FillModuleIDs(bool NextFree)
        {
            // 0..MaxModules-1, connected ones marked. Keeps the chosen ID unless NextFree (after
            // a successful run) or it is gone; otherwise the lowest unconnected ID
            int Keep = (cbWifiModuleID.SelectedItem as ModuleIDItem)?.ID ?? -1;
            bool[] Conn = new bool[Props.MaxModules];
            string Now = "";
            for (int i = 0; i < Props.MaxModules; i++)
            {
                Conn[i] = Core.ModulesStatus.Connected(i);
                Now += Conn[i] ? "1" : "0";
            }

            if (NextFree || Now != (string)cbWifiModuleID.Tag)
            {
                cbWifiModuleID.Tag = Now;
                cbWifiModuleID.Items.Clear();
                int Pick = -1;
                for (int i = 0; i < Props.MaxModules; i++)
                {
                    cbWifiModuleID.Items.Add(new ModuleIDItem { ID = i, Connected = Conn[i] });
                    if (!NextFree && i == Keep) Pick = i;
                }
                for (int i = 0; i < Props.MaxModules && Pick < 0; i++)
                {
                    if (!Conn[i]) Pick = i;
                }
                cbWifiModuleID.SelectedIndex = Pick;     // -1 when all are connected: the user chooses
            }
            ShowModuleIP();
        }

        private void cbWifiModuleID_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowModuleIP();
        }

        private void lnkModuleIP_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (e.Link.LinkData is string Url) System.Diagnostics.Process.Start(Url);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuComm/lnkModuleIP_LinkClicked: " + ex.Message);
            }
        }

        private void ShowModuleIP()
        {
            // the selected ID's web page. Only an ESP32 (InoType 4) has one, and only over WiFi:
            // its Ethernet chip runs a separate stack with no web server
            string Text = "";
            string Url = "";
            ModuleIDItem Sel = cbWifiModuleID.SelectedItem as ModuleIDItem;
            if (Sel != null && Core.ModulesStatus.Connected(Sel.ID))
            {
                string IP = Core.UDPmodules.ModuleIP(Sel.ID);
                if (Core.ModulesStatus.ModuleType(Sel.ID) != 4)
                {
                    Text = "No web page";
                }
                else if (Core.ModulesStatus.EthernetConnected(Sel.ID))
                {
                    Text = "Ethernet, no web page";
                }
                else if (IP != "")
                {
                    Url = "http://" + IP + "/";
                    Text = IP;
                }
            }

            if (lnkModuleIP.Text != Text || (lnkModuleIP.Links.Count > 0) != (Url != ""))
            {
                lnkModuleIP.Text = Text;
                lnkModuleIP.Links.Clear();
                if (Url != "") lnkModuleIP.Links.Add(0, Text.Length, Url);
            }
        }

        private void EnableWifi()
        {
            bool Running = Provisioner != null;
            WifiTarget Target = cbWifiTarget.SelectedItem as WifiTarget;

            ckHotspot.Enabled = !Running;
            cbWifiTarget.Enabled = !Running;
            tbWifiKey.Enabled = !Running && Target != null && !Target.Hotspot;
            tbModuleKey.Enabled = !Running;
            cbWifiModuleID.Enabled = !Running;
            btnWifiScan.Enabled = !Running && !WifiScanning;
            btnWifiSetup.Enabled = !Running && !WifiScanning && Target != null && lstModules.SelectedItem != null;
            btnWifiStop.Enabled = Running && !Provisioner.CancelRequested;
        }

        private void HotspotDone()
        {
            if (!IsDisposed)
            {
                ShowHotspot();
                ReadWifiNetwork();
                if (Provisioner == null)
                {
                    FillModuleIDs(false);       // connected marks, every 6 s on the WiFi tab
                }
            }
        }

        private void lnkHotspot_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start((string)e.Link.LinkData);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuComm/lnkHotspot_LinkClicked: " + ex.Message);
            }
        }

        // Asked once per app session on a WiFi tab visit (after a failed setup it is always asked).
        // Each check starts a short netsh, so it runs on tab visits and after setup, not on the timer
        private static bool FirewallAsked = false;

        private void AskFirewallOnce()
        {
            if (!FirewallAsked && !Core.WebServer.ModuleDataAllowed())
            {
                FirewallAsked = true;
                OfferModuleDataFix();
            }
        }

        private void OfferModuleDataFix()
        {
            WifiTarget Target = cbWifiTarget.SelectedItem as WifiTarget;
            string Network = (Target != null && Target.Ssid != "") ? Target.Ssid : "this network";
            using (frmMsgBox Box = new frmMsgBox("Windows Firewall may be blocking module data on "
                + Network + ".\r\n\r\nAllow it now? Windows will ask for permission.", "WiFi", false, true))
            {
                Box.ShowDialog(this);
                if (Box.Result)
                {
                    bool Ok = Core.WebServer.AllowModuleData();
                    Log(Ok ? "Firewall now allows module data." : "Firewall not changed.");
                }
            }
        }

        private void btnWifiLog_Click(object sender, EventArgs e)
        {
            // the log box is small; show the whole log in a larger, scrolling window
            string Text = tbWifiLog.Text.Trim();
            if (Text == "") Text = "The log is empty.";
            using (frmMsgBox Box = new frmMsgBox(Text, "WiFi Log", false, false))
            {
                Box.ShowDialog(this);
            }
        }

        private void Log(string Text)
        {
            if (!IsDisposed) tbWifiLog.AppendText(Text + Environment.NewLine);
        }

        private void lstModules_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!RestoringPick)
            {
                ModuleItem Item = lstModules.SelectedItem as ModuleItem;
                if (Provisioner != null)
                {
                    // during setup the list stays enabled (a disabled highlight is unreadable),
                    // so a click elsewhere is undone: the module being set up stays selected
                    if (Item == null || Item.Ssid != SetupSsid)
                    {
                        RestoringPick = true;
                        for (int i = 0; i < lstModules.Items.Count; i++)
                        {
                            if (lstModules.Items[i] is ModuleItem M && M.Ssid == SetupSsid) lstModules.SelectedIndex = i;
                        }
                        RestoringPick = false;
                    }
                }
                else if (Item != null && Item.NameID >= 0 && Item.NameID < Props.MaxModules)
                {
                    // an assigned module's hotspot: set it up again under the same ID
                    SelectModuleID(Item.NameID);
                }
                else if (Item != null && Item.NameID == Props.NewModuleID)
                {
                    // a new board: back to the lowest free ID, not one left from an earlier pick
                    FillModuleIDs(true);
                }
            }
            EnableWifi();
        }

        // RC_ESP32 names its hotspot APname + "_" + ID + "_" + 6 hex (chip MAC); -1 if not that shape.
        // Read from the end, so a custom APname containing '_' still works
        private static int IDFromName(string Ssid)
        {
            int Result = -1;
            string[] Parts = Ssid.Split('_');
            if (Parts.Length >= 3 && Parts[Parts.Length - 1].Length == 6
                && uint.TryParse(Parts[Parts.Length - 1], System.Globalization.NumberStyles.HexNumber, null, out _)
                && int.TryParse(Parts[Parts.Length - 2], out int ID) && ID >= 0 && ID <= 255)
            {
                Result = ID;
            }
            return Result;
        }

        private void SelectModuleID(int ID)
        {
            for (int i = 0; i < cbWifiModuleID.Items.Count; i++)
            {
                if (cbWifiModuleID.Items[i] is ModuleIDItem M && M.ID == ID) cbWifiModuleID.SelectedIndex = i;
            }
        }

        private void Post(Action A)
        {
            // from a worker thread; dropped once the form has closed
            try
            {
                if (!IsDisposed && IsHandleCreated) BeginInvoke(A);
            }
            catch (Exception)
            {
                // form closing
            }
        }

        private void ReadWifiNetwork()
        {
            // the PC's own WiFi connection and the tablet hotspot, as targets for the module.
            // The list is only rebuilt when it changes, so a periodic refresh doesn't reset a
            // selection or a typed password
            try
            {
                if (Provisioner == null)
                {
                    WifiAdapter = Wlan.PreferredAdapter();
                    PcSsid = "";
                    PcProfile = "";
                    PcChannel = 0;
                    if (WifiAdapter != null && WifiAdapter.IsConnected)
                    {
                        Wlan.Network N = Wlan.ConnectedNetwork(WifiAdapter.Guid);
                        if (N != null)
                        {
                            PcSsid = N.Ssid;
                            PcProfile = N.ProfileName;
                            PcChannel = Wlan.CurrentChannel(WifiAdapter.Guid);
                        }
                    }

                    List<WifiTarget> Targets = new List<WifiTarget>();
                    string HotspotSsid = Core.WebServer.HotspotSsid;
                    // the hotspot is a target only while Tablet Hotspot is ticked (RateController keeps it up)
                    if (ckHotspot.Checked && Core.WebServer.HotspotState == "On" && HotspotSsid != "") Targets.Add(new WifiTarget { Hotspot = true, Ssid = HotspotSsid });
                    if (PcSsid != "" && PcSsid != HotspotSsid) Targets.Add(new WifiTarget { Hotspot = false, Ssid = PcSsid });

                    string Now = string.Join("|", Targets);
                    string Shown = "";
                    foreach (object O in cbWifiTarget.Items) Shown += (Shown == "" ? "" : "|") + O.ToString();
                    if (Now != Shown)
                    {
                        WifiTarget Old = cbWifiTarget.SelectedItem as WifiTarget;
                        cbWifiTarget.Items.Clear();
                        foreach (WifiTarget T in Targets) cbWifiTarget.Items.Add(T);

                        // keep the kind chosen before, else the hotspot when Tablet Hotspot is on
                        bool WantHotspot = Old != null ? Old.Hotspot : Props.TabletHotspot;
                        int Pick = -1;
                        for (int i = 0; i < Targets.Count; i++)
                        {
                            if (Pick < 0 && Targets[i].Hotspot == WantHotspot) Pick = i;
                        }
                        if (Pick < 0 && Targets.Count > 0) Pick = 0;
                        cbWifiTarget.SelectedIndex = Pick;
                        if (Pick < 0) cbWifiTarget_SelectedIndexChanged(cbWifiTarget, EventArgs.Empty);
                    }

                    if (Targets.Count == 0 && Now != Shown) Log("No WiFi network: turn on the tablet hotspot, or connect this PC to WiFi.");
                    if (WifiAdapter == null && Now != Shown) Log("No WiFi adapter found on this PC.");
                }
                ShowBand();
                EnableWifi();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuComm/ReadWifiNetwork: " + ex.Message);
            }
        }

        public void SelectWifiTab()
        {
            // opened from the link on Options -> Phone
            tabControl1.SelectedTab = tabPage3;
        }

        private void SetHotspotLink()
        {
            // power saving turns the hotspot off after 5 min with no devices, it can only be changed in windows
            lnkHotspot.Links.Clear();
            int Start = lnkHotspot.Text.IndexOf(Lang.lgHotspotSettings);
            if (Start >= 0) lnkHotspot.Links.Add(Start, Lang.lgHotspotSettings.Length, "ms-settings:network-mobilehotspot");

            // theme text colour, the underline marks the link (default blue is unreadable on dark)
            lnkHotspot.LinkColor = lnkHotspot.ForeColor;
            lnkHotspot.ActiveLinkColor = lnkHotspot.ForeColor;
            lnkHotspot.VisitedLinkColor = lnkHotspot.ForeColor;
            lnkModuleIP.LinkColor = lnkModuleIP.ForeColor;
            lnkModuleIP.ActiveLinkColor = lnkModuleIP.ForeColor;
            lnkModuleIP.VisitedLinkColor = lnkModuleIP.ForeColor;
        }

        private void ShowBand()
        {
            // the ESP32 radio is 2.4 GHz only
            string Text = "";
            WifiTarget Target = cbWifiTarget.SelectedItem as WifiTarget;
            if (Target != null && Target.Hotspot)
            {
                string Band = Core.WebServer.HotspotBand;
                if (Core.WebServer.HotspotIs24GHz) Text = "Hotspot band: 2.4 GHz";
                else if (Core.WebServer.HotspotBandNote == "Unsupported") Text = "Hotspot band: Windows can't set 2.4 GHz on this adapter";
                else if (Band == "") Text = "Hotspot band: not reported by Windows";
                else Text = "Hotspot band: " + Band + " (set to 2.4 GHz on next start)";
            }

            if (PcChannel > 14)
            {
                Text += (Text == "" ? "" : ". ") + "This PC's WiFi is on 5 GHz";
                if (Target != null && !Target.Hotspot) Text += ", the module needs its 2.4 GHz band";
                if (!BandTold || PcChannelSsid != PcSsid)
                {
                    BandTold = true;
                    PcChannelSsid = PcSsid;
                    Log(PcSsid + " is on 5 GHz, so the module may not be able to join. " + BandFixSteps);
                }
            }

            string Result = Core.WebServer.HotspotResult;
            if (Core.WebServer.HotspotState != "On" && Result != "" && Result != "Success")
            {
                Text += (Text == "" ? "" : ". ") + string.Format(Lang.lgWindowsReason, Result);     // e.g. WiFiDeviceOff
            }
            lbHotspotBand.Text = Text;
        }

        private void ShowHotspot()
        {
            string State = Core.WebServer.HotspotState;
            string Text = State;      // InTransition / Unknown shown as Windows reports them
            if (State == "On") Text = Lang.lgOn;
            else if (State == "Off") Text = Lang.lgOff;
            if (Core.WebServer.HotspotBusy) Text = Lang.lgChecking;
            if (State == "On" && !Core.WebServer.HotspotAllowed()) Text += ", " + Lang.lgFirewallBlocked;
            if (Core.WebServer.HotspotFailing && ckHotspot.Checked) Text = Lang.lgRestartsFailed;
            if (!ckHotspot.Checked) Text = "";      // not managed by RateController: no state shown
            lbHotspot.Text = Text;
        }

        private void ShowModules(List<Wlan.Network> Nets, HashSet<string> Stale)
        {
            WifiScanning = false;
            if (!IsDisposed)
            {
                // module hotspots only (RateModule_...), by signal (Networks() already sorts by
                // signal). This PC's own WiFi and the tablet hotspot can't be modules, and a name
                // Windows still caches from before a reflash (Stale, same radio) can't be reached
                string Old = (lstModules.SelectedItem as ModuleItem)?.Ssid ?? "";
                string Skip1 = PcSsid;
                string Skip2 = Core.WebServer.HotspotSsid;
                lstModules.Items.Clear();
                if (Nets != null)
                {
                    foreach (Wlan.Network N in Nets)
                    {
                        if (N.Ssid.StartsWith(ModulePrefix, StringComparison.OrdinalIgnoreCase) && N.Ssid != Skip1 && N.Ssid != Skip2
                            && (Stale == null || !Stale.Contains(N.Ssid)))
                        {
                            int i = lstModules.Items.Add(new ModuleItem { Ssid = N.Ssid, Signal = N.SignalPercent, Secured = N.Secured, NameID = IDFromName(N.Ssid) });
                            if (N.Ssid == Old) lstModules.SelectedIndex = i;
                        }
                    }
                }
                Log("Scan finished.");
                if (lstModules.Items.Count == 0) Log("No " + ModulePrefix + "_... hotspots found.");

                // a lone module is picked automatically
                if (lstModules.Items.Count == 1 && lstModules.SelectedIndex < 0) lstModules.SelectedIndex = 0;
                EnableWifi();
            }
        }

        private void StartScan()
        {
            if (!WifiScanning && Provisioner == null)
            {
                if (WifiAdapter == null) WifiAdapter = Wlan.PreferredAdapter();
                if (WifiAdapter == null)
                {
                    Log("No WiFi adapter found on this PC.");
                }
                else
                {
                    // a scan takes a few seconds in the driver
                    WifiScanning = true;
                    Log("Scanning for networks ...");
                    EnableWifi();
                    Guid G = WifiAdapter.Guid;
                    ThreadPool.QueueUserWorkItem(o =>
                    {
                        List<Wlan.Network> Nets = null;
                        HashSet<string> Stale = null;
                        try
                        {
                            Wlan.RequestScan(G);
                            Thread.Sleep(3000);
                            Nets = Wlan.Networks(G);
                            Stale = Wlan.StaleNames(G, n => IDFromName(n) >= 0);
                        }
                        catch (Exception ex)
                        {
                            Props.WriteErrorLog("frmMenuComm/Scan: " + ex.Message);
                        }
                        Post(() => ShowModules(Nets, Stale));
                    });
                }
            }
        }

        private void StartSetup(ProvisionRequest Req)
        {
            tbWifiLog.Clear();
            ModuleProvisioner P = new ModuleProvisioner(s => Post(() => Log(s)));
            Provisioner = P;
            SetupSsid = Req.ModuleSsid;     // the list stays readable but locked to this pick
            EnableWifi();
            ThreadPool.QueueUserWorkItem(o =>
            {
                ProvisionResult R = null;
                try
                {
                    R = P.Run(Req);
                }
                catch (Exception ex)
                {
                    R = new ProvisionResult { Outcome = ProvisionOutcome.Aborted, Detail = ex.Message };
                }
                // always logged: the form may have closed during the run
                Props.WriteErrorLog("frmMenuComm/ModuleWifiSetup " + Req.ModuleSsid + " -> " + Req.HomeSsid + ": " + R.Outcome + (R.Detail != "" ? ", " + R.Detail : ""));
                Post(() => SetupDone(R));
            });
        }

        private void SetupDone(ProvisionResult R)
        {
            Provisioner = null;
            string Text;
            switch (R.Outcome)
            {
                case ProvisionOutcome.Success:
                    Text = "Done: module " + R.ModuleID + " is connected.";
                    break;

                case ProvisionOutcome.AuthRejected:
                    Text = "Password refused: " + R.Detail;
                    break;

                case ProvisionOutcome.NotConnected:
                    Text = "Not joined yet: " + R.Detail;
                    break;

                case ProvisionOutcome.NoPackets:
                    Text = "No data from a new module: " + R.Detail;
                    break;

                case ProvisionOutcome.HotspotFailed:
                    Text = "Could not join the module's hotspot: " + R.Detail;
                    break;

                case ProvisionOutcome.PortalFailed:
                    Text = "The module's settings page did not answer: " + R.Detail;
                    break;

                case ProvisionOutcome.HomeReturnFailed:
                    Text = "This PC did not get back: " + R.Detail;
                    break;

                case ProvisionOutcome.Cancelled:
                    Text = "Cancelled." + (R.Detail != "" ? " " + R.Detail : "");
                    break;

                default:
                    Text = R.Detail;
                    break;
            }
            Log(Text);
            if (R.Outcome == ProvisionOutcome.NoPackets && !Core.WebServer.ModuleDataAllowed())
            {
                // joined but nothing arrived: offer the firewall fix straight away
                OfferModuleDataFix();
            }
            ShowHotspot();
            ReadWifiNetwork();
            FillModuleIDs(R.Outcome == ProvisionOutcome.Success);    // after a success, on to the next free ID

            // the module restarted under its new ID, so its hotspot has a new name; scan again
            // so the list shows it (the old name is the same radio, which StaleNames hides)
            StartScan();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshComPorts();
        }

        private void btnRescan_Click(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void btnSendSubnet_Click(object sender, EventArgs e)
        {
            try
            {
                // set app subnet
                Core.UDPmodules.NetworkEP = cbEthernet.Text;
                PGN32503 SetSubnet = new PGN32503();
                if (SetSubnet.Send(Core.UDPmodules.NetworkEP))
                {
                    Props.ShowMessage("New Subnet address sent.", "Subnet", 10000);
                }
                else
                {
                    Props.ShowMessage("New Subnet address not sent.", "Subnet", 10000);
                }
            }
            catch (Exception ex)
            {
                Props.ShowMessage(ex.Message, "frmMenuComm/btnSendSubnet", 15000, true);
            }
        }

        private void cbComPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void ckCanBus_CheckedChanged(object sender, EventArgs e)
        {
            ckEthernet.Checked = !ckCanBus.Checked;
            SetButtons(true);
        }

        private void ckEthernet_CheckedChanged(object sender, EventArgs e)
        {
            ckCanBus.Checked = !ckEthernet.Checked;
            SetButtons(true);
        }

        private void frmMenuComm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // a setup run finishes in the background (that is how the PC gets back); its result
            // goes to the error log
            if (Provisioner != null) Provisioner.Cancel();
            timer1.Enabled = false;
            Props.SaveFormLocation(this);
        }

        private void frmMenuComm_Load(object sender, System.EventArgs e)
        {
            SubMenuLayout.SetFormLayout(this, MainMenu, btnOK);

            btnCancel.Left = btnOK.Left - SubMenuLayout.ButtonSpacing;
            btnCancel.Top = btnOK.Top;
            // StyleControls restyles every label's font; keep the designer fonts of the WiFi tab's small text
            Font HintFont = lnkHotspot.Font;
            Font BandFont = lbHotspotBand.Font;
            Font LogFont = tbWifiLog.Font;
            Font IPFont = lnkModuleIP.Font;
            MainMenu.StyleControls(this);
            lnkModuleIP.Font = IPFont;
            lnkHotspot.Font = HintFont;
            lbHotspotBand.Font = BandFont;
            tbWifiLog.Font = LogFont;

            SetLanguage();
            SetHotspotLink();   // after SetLanguage: finds the translated link words
            tbModuleKey.Text = WifiStore.ModuleKey;
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            this.BackColor = Properties.Settings.Default.MainBackColour;

            tabControl1.SizeMode = TabSizeMode.Fixed;
            tabControl1.ItemSize = new Size((tabControl1.Width - 4) / tabControl1.TabCount, tabControl1.ItemSize.Height);
            foreach (TabPage tp in tabControl1.TabPages)
                tp.BackColor = this.BackColor;

            timer1.Enabled = true;

            PositionForm();
            UpdateForm();
        }

        private void gbEthernet_Paint(object sender, PaintEventArgs e)
        {
            Props.DrawGroupBox((GroupBox)sender, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }

        private void LoadCombo()
        {
            // https://stackoverflow.com/questions/6803073/get-local-ip-address
            try
            {
                cbEthernet.Items.Clear();
                foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if ((item.NetworkInterfaceType == NetworkInterfaceType.Ethernet || item.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) && item.OperationalStatus == OperationalStatus.Up)
                    {
                        foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                cbEthernet.Items.Add(ip.Address.ToString());
                            }
                        }
                    }
                }
                cbEthernet.SelectedIndex = cbEthernet.FindString(SubAddress(Core.UDPmodules.NetworkEP));
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuComm/LoadCombo: " + ex.Message);
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

        private void rbAdapter1_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing)
            {
                SetButtons(true);
                gbxPort.Enabled = rbAdapter1.Checked; // enable for SLCAN
            }
        }

        private void RefreshComPorts()
        {
            cbComPort.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            foreach (string port in ports)
            {
                cbComPort.Items.Add(port);
            }

            // Select current port if it exists
            int index = cbComPort.FindStringExact(Props.CanPort);
            if (index >= 0)
            {
                cbComPort.SelectedIndex = index;
            }
            else if (cbComPort.Items.Count > 0)
            {
                cbComPort.SelectedIndex = 0;
            }
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
            ckHotspot.Text = Lang.lgTabletHotspot;
            lnkHotspot.Text = string.Format(Lang.lgPowerSavingHint, Lang.lgHotspotSettings);
        }

        private string SubAddress(string Address)
        {
            IPAddress IP;
            string[] data;
            string Result = "";

            if (IPAddress.TryParse(Address, out IP))
            {
                data = Address.Split('.');
                Result = data[0] + "." + data[1] + "." + data[2] + ".";
            }
            return Result;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // refreshed on every visit: the hotspot or the PC's WiFi may have changed since
            if (tabControl1.SelectedTab == tabPage3)
            {
                HotspotTicks = 0;
                ShowHotspot();
                ReadWifiNetwork();
                FillModuleIDs(false);
                if (!Core.WebServer.HotspotBusy) Core.WebServer.HotspotAsync("status", HotspotDone);     // catches changes made in Windows settings
                if (lstModules.Items.Count == 0) StartScan();

                // after the tab has painted, so the question doesn't open over a half-drawn page
                if (IsHandleCreated) BeginInvoke((Action)AskFirewallOnce);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // WiFi tab: hotspot state every 6 s (each check is a short powershell)
            if (tabControl1.SelectedTab == tabPage3)
            {
                HotspotTicks++;
                if (HotspotTicks >= 6 && !Core.WebServer.HotspotBusy)
                {
                    HotspotTicks = 0;
                    Core.WebServer.HotspotAsync("status", HotspotDone);
                }
            }

            if (Core.CanBridgeComm != null && Props.CanEnabled)
            {
                // lbConnected = module data (PGN 32400/32401) received within 2s
                lbConnected.Image = Core.CanBridgeComm.ModuleDataReceiving
                    ? Properties.Resources.On
                    : Properties.Resources.Off;

                // lbDriverFound = CAN adapter open and frames flowing within 4s
                lbDriverFound.Image = Core.CanBridgeComm.CanAdapterConnected
                    ? Properties.Resources.On
                    : Properties.Resources.Off;
            }
            else
            {
                lbConnected.Image = Properties.Resources.Off;
                lbDriverFound.Image = Properties.Resources.Off;
            }
        }

        private void UpdateForm()
        {
            Initializing = true;
            try
            {
                LoadCombo();
                lbModuleIP.Text = Core.UDPmodules.SubNet;

                if (Props.CanEnabled)
                {
                    ckCanBus.Checked = true;
                }
                else
                {
                    ckEthernet.Checked = true;
                }

                switch (Props.CurrentCanDriver)
                {
                    case CanDriver.InnoMaker:
                        rbAdapter2.Checked = true;
                        break;

                    case CanDriver.PCAN:
                        rbAdapter3.Checked = true;
                        break;

                    default:
                        rbAdapter1.Checked = true;  // SLCAN
                        break;
                }

                ckDiagnostics.Checked = Props.ShowCanDiagnostics;
                ckHotspot.Checked = Props.TabletHotspot;

                RefreshComPorts();
                gbxPort.Enabled = rbAdapter1.Checked; // enable for SLCAN
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuComm/UpdateForm: " + ex.Message);
            }
            Initializing = false;
        }
    }
}