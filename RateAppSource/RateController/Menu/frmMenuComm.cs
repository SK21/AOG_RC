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
        private bool cFirewallAsking = false;       // module-data firewall check running (AskFirewallOnce)
        private bool cFirewallChecking = false;     // hotspot firewall check running (CheckFirewall)
        private bool cFirewallOk = true;            // hotspot firewall rules, from the last background check
        private bool cFirewallRecheck = false;      // another check was asked for while one was running
        private string cLastHotspotState = "";      // "" = check the firewall again when the hotspot shows On
        private WifiTarget cTarget;                 // network the module will join, null = none available
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

        private static bool SameModuleHotspot(string A, string B)
        {
            // true when A and B are the same module's hotspot: identical, or both RateModule names
            // with the same suffix after the last '_' (MAC bytes), which an ID change keeps
            bool Result = string.Equals(A, B, StringComparison.OrdinalIgnoreCase);
            if (!Result && A != null && B != null
                && A.StartsWith(ModulePrefix, StringComparison.OrdinalIgnoreCase)
                && B.StartsWith(ModulePrefix, StringComparison.OrdinalIgnoreCase))
            {
                string SA = A.Substring(A.LastIndexOf('_') + 1);
                string SB = B.Substring(B.LastIndexOf('_') + 1);
                Result = SA.Length > 0 && string.Equals(SA, SB, StringComparison.OrdinalIgnoreCase);
            }
            return Result;
        }

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
                return ID + (Connected ? " (" + Lang.lgConnected + ")" : "");
            }
        }

        private sealed class AdapterItem
        {
            public string IP;       // this PC's address on the adapter
            public string Name;     // Windows adapter name, e.g. Ethernet 2
            public string Subnet;   // a.b.c

            public override string ToString()
            {
                return Name + " (" + Subnet + ".x)";
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
            if (cbEthernet.SelectedItem is AdapterItem Adapter) Core.UDPmodules.NetworkEP = Adapter.IP;

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
                        // Ethernet: only a Teensy that was switched is worth a message
                        Mes = "";
                        break;
                }

                if (ModeChanged)
                {
                    Core.ModuleConfig.Save();
                    if (Core.CommModeNote != "") Mes = (Core.CommModeNote + " " + Mes).Trim();
                }
                if (Mes != "") Props.ShowMessage(Mes, "Config", 10000);
            }

            // tablet hotspot (moved here from Options -> Phone)
            bool HotspotChanged = rbHotspot.Checked != Props.TabletHotspot;
            Props.TabletHotspot = rbHotspot.Checked;
            if (HotspotChanged) ApplyHotspot();

            SetButtons(false);
            UpdateForm();
        }

        private void ApplyHotspot()
        {
            if (rbHotspot.Checked)
            {
                // firewall rules need admin, only asked when a rule is missing
                if (Core.WebServer.IsRunning && !Core.WebServer.HotspotAllowed() && !Core.WebServer.AllowHotspot())
                {
                    Props.ShowMessage(Lang.lgFirewallNotAllowed, Lang.lgPhone, 5000);
                }
                lbWifiTarget.Text = Lang.lgStarting;
                cLastHotspotState = "";     // the rules may have just been added: check again once On
                Core.WebServer.HotspotWatchReset();
                Core.WebServer.HotspotAsync("start", HotspotDone);
            }
            else
            {
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
                WifiTarget Target = cTarget;
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
                else if (!Target.Hotspot && SameModuleHotspot(Target.Ssid, Module.Ssid))
                {
                    Log(Module.Ssid + " can't join its own hotspot. Connect this PC to the network the module should join first.");
                }
                else if (ModuleProvisioner.CredentialTooLong(
                    Target.Hotspot ? Core.WebServer.HotspotSsid : Target.Ssid,
                    Target.Hotspot ? Core.WebServer.HotspotPassword : tbWifiKey.Text) != "")
                {
                    Log(ModuleProvisioner.CredentialTooLong(
                        Target.Hotspot ? Core.WebServer.HotspotSsid : Target.Ssid,
                        Target.Hotspot ? Core.WebServer.HotspotPassword : tbWifiKey.Text));
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

        private void TargetChanged()
        {
            // password box follows the target: the hotspot's password is known, a WiFi
            // network's is typed, prefilled only when the saved one belongs to that network
            if (cTarget != null && !cTarget.Hotspot)
            {
                tbWifiKey.Text = (WifiStore.HomeSsid == cTarget.Ssid) ? WifiStore.HomeKey : "";
            }
            else
            {
                tbWifiKey.Text = "";
            }
        }

        private void rbHotspot_CheckedChanged(object sender, EventArgs e)
        {
            // fires for both radio buttons (one turns off as the other turns on).
            // The hotspot itself starts/stops on Save
            SetButtons(true);
            SetHint();
            ReadWifiNetwork();
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

        // The user picked an ID: highlight that module's hotspot if it's in the list. User
        // picks only (not SelectedIndexChanged) - the app moves this box itself (connected-mark
        // refresh, next free ID after a new board is picked), and that must not take the
        // highlight off a new board. No hotspot for the ID: the list is left as it is.
        private void cbWifiModuleID_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (Provisioner == null && cbWifiModuleID.SelectedItem is ModuleIDItem Sel)
            {
                int Match = -1;
                for (int i = 0; i < lstModules.Items.Count && Match < 0; i++)
                {
                    if (lstModules.Items[i] is ModuleItem M && M.NameID == Sel.ID) Match = i;
                }
                if (Match >= 0 && lstModules.SelectedIndex != Match) lstModules.SelectedIndex = Match;
            }
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
            WifiTarget Target = cTarget;

            rbHotspot.Enabled = !Running;
            rbPcWifi.Enabled = !Running;
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
        // The check runs netsh and walks every firewall rule, so it runs off the UI thread
        private static bool FirewallAsked = false;

        private void AskFirewallOnce()
        {
            if (!FirewallAsked && !cFirewallAsking)
            {
                cFirewallAsking = true;
                ThreadPool.QueueUserWorkItem(o =>
                {
                    bool Ok = Core.WebServer.ModuleDataAllowed();
                    Post(() =>
                    {
                        cFirewallAsking = false;
                        if (!Ok && !FirewallAsked)
                        {
                            FirewallAsked = true;
                            OfferModuleDataFix();
                        }
                    });
                });
            }
        }

        private void CheckFirewall()
        {
            // the hotspot firewall note on the Network line. HotspotAllowed runs two netsh and
            // walks every firewall rule (most of a second), so off the UI thread, and only when
            // it can have changed: tab visit, hotspot coming on, rules just allowed
            if (cFirewallChecking)
            {
                cFirewallRecheck = true;
            }
            else
            {
                cFirewallChecking = true;
                ThreadPool.QueueUserWorkItem(o =>
                {
                    bool Ok = Core.WebServer.HotspotAllowed();
                    Post(() => FirewallChecked(Ok));
                });
            }
        }

        private void FirewallChecked(bool Ok)
        {
            cFirewallChecking = false;
            cFirewallOk = Ok;
            if (cFirewallRecheck)
            {
                cFirewallRecheck = false;
                CheckFirewall();
            }
            ShowTarget();
        }

        private void OfferModuleDataFix()
        {
            WifiTarget Target = cTarget;
            string Network = (Target != null && Target.Ssid != "") ? Target.Ssid : "this network";
            using (frmMsgBox Box = new frmMsgBox("Windows Firewall may be blocking module data on "
                + Network + ".\r\n\r\nAllow it now? Windows will ask for permission.", "WiFi", false, true))
            {
                Box.ShowDialog(this);
                if (Box.Result)
                {
                    bool Ok = Core.WebServer.AllowModuleData();
                    Log(Ok ? "Firewall now allows module data." : "Firewall not changed.");
                    CheckFirewall();    // it can also lift a block on this exe
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

                    // the radio buttons choose the kind: the tablet hotspot (RateController keeps it
                    // up while chosen) or whatever network this PC is connected to in Windows
                    WifiTarget Target = null;
                    string HotspotSsid = Core.WebServer.HotspotSsid;
                    if (rbHotspot.Checked)
                    {
                        if (Core.WebServer.HotspotState == "On" && HotspotSsid != "") Target = new WifiTarget { Hotspot = true, Ssid = HotspotSsid };
                    }
                    else if (PcSsid != "" && PcSsid != HotspotSsid)
                    {
                        Target = new WifiTarget { Hotspot = false, Ssid = PcSsid };
                    }

                    // only on a change, so a periodic refresh doesn't wipe a typed password
                    string Now = Target == null ? "" : Target.ToString();
                    string Shown = cTarget == null ? "" : cTarget.ToString();
                    if (Now != Shown)
                    {
                        cTarget = Target;
                        TargetChanged();
                        if (Target == null && rbPcWifi.Checked) Log("This PC is not on a WiFi network. Connect it in Windows WiFi settings, or choose Tablet Hotspot.");
                        if (WifiAdapter == null) Log("No WiFi adapter found on this PC.");
                    }
                }
                ShowTarget();
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

        private void SetHint()
        {
            // hotspot: power saving turns it off after 5 min with no devices, only changeable in windows.
            // This PC's WiFi: the PC is put on the network in windows, the module then gets the same one
            string LinkText;
            string Url;
            if (rbHotspot.Checked)
            {
                LinkText = Lang.lgHotspotSettings;
                Url = "ms-settings:network-mobilehotspot";
                lnkHotspot.Text = string.Format(Lang.lgPowerSavingHint, LinkText);
            }
            else
            {
                LinkText = Lang.lgWifiSettings;
                Url = "ms-settings:network-wifi";
                lnkHotspot.Text = string.Format(Lang.lgConnectPcFirst, LinkText);
            }
            lnkHotspot.Links.Clear();
            int Start = lnkHotspot.Text.IndexOf(LinkText);
            if (Start >= 0) lnkHotspot.Links.Add(Start, LinkText.Length, Url);

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
            WifiTarget Target = cTarget;
            if (Target != null && Target.Hotspot)
            {
                string Band = Core.WebServer.HotspotBand;
                if (Core.WebServer.HotspotIs24GHz) Text = string.Format(Lang.lgHotspotBand, "2.4 GHz");
                else if (Core.WebServer.HotspotBandNote == "Unsupported") Text = string.Format(Lang.lgHotspotBand, Lang.lgBandUnsupported);
                else if (Band == "") Text = string.Format(Lang.lgHotspotBand, Lang.lgBandNotReported);
                else Text = string.Format(Lang.lgHotspotBand, string.Format(Lang.lgBandNextStart, Band));
            }

            if (PcChannel > 14)
            {
                Text += (Text == "" ? "" : ". ") + Lang.lgPc5GHz;
                if (Target != null && !Target.Hotspot) Text += ", " + Lang.lgModuleNeeds24;
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

        private void ShowTarget()
        {
            // the network the module will join: the hotspot name with its state, or this PC's network
            string Text;
            if (rbHotspot.Checked)
            {
                string State = Core.WebServer.HotspotState;
                string StateText = State;      // InTransition / Unknown shown as Windows reports them
                if (State == "On") StateText = Lang.lgOn;
                else if (State == "Off") StateText = Lang.lgOff;
                if (Core.WebServer.HotspotBusy) StateText = Lang.lgChecking;
                if (State == "On" && cLastHotspotState != "On") CheckFirewall();     // result comes back through ShowTarget
                cLastHotspotState = State;
                if (State == "On" && !cFirewallOk) StateText += ", " + Lang.lgFirewallBlocked;
                if (Core.WebServer.HotspotFailing) StateText = Lang.lgRestartsFailed;
                string Ssid = Core.WebServer.HotspotSsid;
                Text = (Ssid != "" ? Ssid + " - " : "") + StateText;
            }
            else
            {
                Text = cTarget != null ? cTarget.Ssid : Lang.lgNotConnected;
            }
            lbWifiTarget.Text = Text;
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
                    Log("Scanning for modules ...");
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
                // the app's subnet follows the adapter, and the modules are told to move to it
                AdapterItem Adapter = cbEthernet.SelectedItem as AdapterItem;
                if (Adapter == null)
                {
                    Props.ShowMessage(Lang.lgSelectAdapterFirst, "Subnet", 10000);
                }
                else
                {
                    Core.UDPmodules.NetworkEP = Adapter.IP;
                    PGN32503 SetSubnet = new PGN32503();
                    if (SetSubnet.Send(Core.UDPmodules.NetworkEP))
                    {
                        Props.ShowMessage("New Subnet address sent.", "Subnet", 10000);
                    }
                    else
                    {
                        Props.ShowMessage("New Subnet address not sent.", "Subnet", 10000);
                    }
                    ShowSubnets();
                }
            }
            catch (Exception ex)
            {
                Props.ShowMessage(ex.Message, "frmMenuComm/btnSendSubnet", 15000, true);
            }
        }

        private void cbEthernet_SelectedIndexChanged(object sender, EventArgs e)
        {
            // a new adapter is saved first, then sent: Send stays off until Save (or Cancel)
            if (!Initializing)
            {
                SetButtons(true);
                btnSendSubnet.Enabled = false;
                lbSend.Enabled = false;
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
            Font HeadingFont = lbModulesHeading.Font;
            Font SmallFont = lbSteps.Font;      // Ethernet tab's 12 pt text
            MainMenu.StyleControls(this);
            lnkModuleIP.Font = IPFont;
            lbModulesHeading.Font = HeadingFont;
            foreach (Label Lb in new Label[] { lbSteps, lbHeard, lbRefresh, lbSend }) Lb.Font = SmallFont;
            lnkHotspot.Font = HintFont;
            lbHotspotBand.Font = BandFont;
            tbWifiLog.Font = LogFont;

            SetLanguage();
            SetHint();          // after SetLanguage: finds the translated link words
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
            // wired adapters only, by name: Send moves the modules on the cable to the chosen
            // adapter's subnet, so a WiFi address (tablet hotspot, PC WiFi) would strand them.
            // Link-local 169.254 = no address set on the adapter, not a usable subnet
            try
            {
                cbEthernet.Items.Clear();
                int Pick = -1;
                foreach (NetworkInterface Nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    NetworkInterfaceType Kind = Nic.NetworkInterfaceType;
                    if (Nic.OperationalStatus == OperationalStatus.Up && Kind != NetworkInterfaceType.Wireless80211
                        && Kind != NetworkInterfaceType.Loopback && Kind != NetworkInterfaceType.Tunnel)
                    {
                        foreach (UnicastIPAddressInformation Info in Nic.GetIPProperties().UnicastAddresses)
                        {
                            byte[] B = Info.Address.GetAddressBytes();
                            if (Info.Address.AddressFamily == AddressFamily.InterNetwork && !(B[0] == 169 && B[1] == 254)
                                && !UDPComm.IsTabletHotspot(Nic, Info.Address))
                            {
                                AdapterItem Item = new AdapterItem { Name = Nic.Name, IP = Info.Address.ToString(), Subnet = B[0] + "." + B[1] + "." + B[2] };
                                cbEthernet.Items.Add(Item);
                                if (Item.Subnet == Core.UDPmodules.SubNet) Pick = cbEthernet.Items.Count - 1;
                            }
                        }
                    }
                }
                // the saved subnet is on no adapter of this PC (new install, adapter changed):
                // switch to one that is. The adapter modules are heard on first, else the first
                if (Pick < 0 && cbEthernet.Items.Count > 0)
                {
                    Pick = 0;
                    for (int i = cbEthernet.Items.Count - 1; i >= 0; i--)
                    {
                        if (ModulesOn(((AdapterItem)cbEthernet.Items[i]).Subnet) > 0) Pick = i;
                    }
                    Core.UDPmodules.NetworkEP = ((AdapterItem)cbEthernet.Items[Pick]).IP;
                }
                cbEthernet.SelectedIndex = Pick;
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
            rbHotspot.Text = Lang.lgTabletHotspot;
            rbPcWifi.Text = Lang.lgCurrentWifiNetwork;
            lbWifiNetwork.Text = Lang.lgNetwork;
            lbModulesHeading.Text = Lang.lgDiscoveredModules;
            lbWifiKey.Text = Lang.lgNetworkPassword;
            lbModuleKey.Text = Lang.lgModulePassword;
            lbWifiModuleID.Text = Lang.lgSelectedModuleID;
            btnWifiSetup.Text = Lang.lgSetUp;
            btnWifiStop.Text = Lang.lgCancel;

            lbIP.Text = Lang.lgAdapter;
            lbSteps.Text = Lang.lgEthernetSteps;
            lbRefresh.Text = Lang.lgRefresh;
            lbSend.Text = Lang.lgSend;
            ckEthernet.Text = Lang.lgEnabled;
            ckCanBus.Text = Lang.lgEnabled;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage1) ShowSubnets();

            // refreshed on every visit: the hotspot or the PC's WiFi may have changed since
            if (tabControl1.SelectedTab == tabPage3)
            {
                HotspotTicks = 0;
                cLastHotspotState = "";     // firewall checked again on each visit
                ReadWifiNetwork();
                FillModuleIDs(false);
                if (!Core.WebServer.HotspotBusy) Core.WebServer.HotspotAsync("status", HotspotDone);     // catches changes made in Windows settings
                if (lstModules.Items.Count == 0) StartScan();

                // after the tab has painted, so the question doesn't open over a half-drawn page
                if (IsHandleCreated) BeginInvoke((Action)AskFirewallOnce);
            }
        }

        private int ModulesOn(string Subnet)
        {
            // connected modules last heard from an address on Subnet (a.b.c)
            int Count = 0;
            for (int i = 0; i < Props.MaxModules; i++)
            {
                if (Core.ModulesStatus.Connected(i) && Core.UDPmodules.ModuleIP(i).StartsWith(Subnet + ".")) Count++;
            }
            return Count;
        }

        private void ShowSubnets()
        {
            // one status line for the selected (always sent to) subnet: no adapter of this PC on
            // it (LoadCombo has already switched to one if any wired adapter has an address),
            // or how many connected modules are on it
            string Sub = Core.UDPmodules.SubNet;
            bool HasAdapter = UDPComm.AdapterName(Sub) != "";
            int Count = ModulesOn(Sub);
            string Status;
            if (!HasAdapter)
            {
                Status = string.Format(Lang.lgNoWiredAdapter, Sub);
            }
            else if (Count > 0)
            {
                Status = string.Format(Lang.lgModulesOnAdapter, Count);
            }
            else
            {
                Status = Lang.lgNoModulesOnAdapter;
            }
            if (lbHeard.Text != Status) lbHeard.Text = Status;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Ethernet tab: subnets modules are heard on, every 2 s
            if (tabControl1.SelectedTab == tabPage1 && DateTime.Now.Second % 2 == 0) ShowSubnets();

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
                ShowSubnets();
                btnSendSubnet.Enabled = true;       // the dropdown shows the saved adapter again
                lbSend.Enabled = true;

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
                if (Props.TabletHotspot)
                {
                    rbHotspot.Checked = true;
                }
                else
                {
                    rbPcWifi.Checked = true;
                }

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