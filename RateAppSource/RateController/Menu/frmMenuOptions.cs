using AgOpenGPS;
using RateController.Classes;
using RateController.Language;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuOptions : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private int HotspotTicks = 0;
        private int PhoneIndex = 0;
        private List<string> PhoneList = new List<string>();
        private bool WheelSpeedChanged = false;

        public frmMenuOptions(frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            this.Tag = false;
        }

        private void btnCal_Click(object sender, EventArgs e)
        {
            double calNumber = 0;
            bool wasCanceled = true;

            using (var dlg = new RateController.Forms.frmSpeedCal())
            {
                var result = dlg.ShowDialog(this);
                wasCanceled = (result != DialogResult.OK) || dlg.Canceled;
                calNumber = dlg.CalNumber;
            }

            if (!wasCanceled) tbWheelCal.Text = calNumber.ToString("N0");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
            WheelSpeedChanged = false;
            butUpdateModules.Enabled = rbWheel.Checked;
        }

        private void ApplyHotspot()
        {
            if (ckHotspot.Checked)
            {
                // firewall rule and power saving need admin, only asked when the rule is missing
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

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (rbAOG.Checked)
                {
                    Props.SpeedMode = SpeedType.GPS;
                }
                else if (rbWheel.Checked)
                {
                    Props.SpeedMode = SpeedType.Wheel;
                }
                else
                {
                    Props.SpeedMode = SpeedType.Simulated;
                }

                if (int.TryParse(tbWheelModule.Text, out int wm)) Core.WheelSpeed.WheelModule = wm;

                if (int.TryParse(tbWheelPin.Text, out int wp) && (ValidPin(wp)))
                {
                    Core.WheelSpeed.WheelPin = wp;
                }
                else
                {
                    Core.WheelSpeed.WheelPin = 255;
                }

                if (double.TryParse(tbWheelCal.Text, out double wc)) Core.WheelSpeed.WheelCal = wc;

                if (double.TryParse(tbSimSpeed.Text, out double Speed))
                {
                    if (Props.UseMetric)
                    {
                        Props.SimSpeed_KMH = Speed;
                    }
                    else
                    {
                        Props.SimSpeed_KMH = Speed * Props.MPHtoKPH;
                    }
                }

                Props.UseMetric = ckMetric.Checked;
                Props.CompactDisplay = ckCompact.Checked;

                bool HotspotChanged = ckHotspot.Checked != Props.TabletHotspot;
                Props.TabletHotspot = ckHotspot.Checked;
                if (HotspotChanged) ApplyHotspot();

                if (WheelSpeedChanged)
                {
                    Core.WheelSpeed.Send();
                    WheelSpeedChanged = false;
                    butUpdateModules.Enabled = rbWheel.Checked;
                }

                SetButtons(false);
                UpdateForm();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuOptions/btnOk_Click: " + ex.Message);
            }
        }

        private void butUpdateModules_Click(object sender, EventArgs e)
        {
            Core.WheelSpeed.Send();
        }

        private void ckMetric_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void DrawPhone()
        {
            pbPhoneQR.Image?.Dispose();
            pbPhoneQR.Image = null;
            if (!Core.WebServer.IsRunning)
            {
                lbPhoneUrl.Text = string.Format(Lang.lgServerNotRunning, Props.WebServerPort);
            }
            else if (PhoneList.Count == 0)
            {
                lbPhoneUrl.Text = Lang.lgPhoneNoNetwork;
            }
            else
            {
                string Url = PhoneList[PhoneIndex];
                pbPhoneQR.Image = clsQrCode.Create(Url, 6);
                lbPhoneUrl.Text = Url.Substring("http://".Length);
                if (PhoneList.Count > 1) lbPhoneUrl.Text += "\n" + string.Format(Lang.lgTapForNext, PhoneIndex + 1, PhoneList.Count);
            }
        }

        private void frmMenuDisplay_FormClosed(object sender, FormClosedEventArgs e)
        {
            tmrPhone.Enabled = false;
            MainMenu.MenuMoved -= MainMenu_MenuMoved;
            pbPhoneQR.Image?.Dispose();
            Props.SaveFormLocation(this);
        }

        private void frmMenuDisplay_Load(object sender, EventArgs e)
        {
            SubMenuLayout.SetFormLayout(this, MainMenu, btnOK);

            btnCancel.Left = btnOK.Left - SubMenuLayout.ButtonSpacing;
            btnCancel.Top = btnOK.Top;
            // StyleControls restyles every label's font; keep the phone tab's smaller designer fonts
            Font urlFont = lbPhoneUrl.Font;
            Font helpFont = lbPhoneHelp.Font;
            Font netFont = lbHotspotNet.Font;
            Font hintFont = lnkHotspot.Font;
            MainMenu.StyleControls(this);
            lbPhoneUrl.Font = urlFont;
            lbPhoneHelp.Font = helpFont;
            lbHotspotNet.Font = netFont;
            lnkHotspot.Font = hintFont;

            SetLanguage();
            SetPhoneLinks();    // after SetLanguage: finds the translated link words
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            this.BackColor = Properties.Settings.Default.MainBackColour;
            lbPulses.Font = new Font(lbPulses.Font.FontFamily, 12f, lbPulses.Font.Style,
                                lbPulses.Font.Unit, lbPulses.Font.GdiCharSet, lbPulses.Font.GdiVerticalFont);

            tabControl1.SizeMode = TabSizeMode.Fixed;
            tabControl1.ItemSize = new Size((tabControl1.Width - 4) / tabControl1.TabCount, tabControl1.ItemSize.Height);
            foreach (TabPage tp in tabControl1.TabPages)
                tp.BackColor = this.BackColor;

            PositionForm();
            SetBoxes();
            UpdateForm();
        }

        private void groupBox2_Paint(object sender, PaintEventArgs e)
        {
            Props.DrawGroupBox((GroupBox)sender, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }

        private void HotspotDone()
        {
            // the hotspot adds or removes 192.168.137.1, so the address list is refreshed too
            if (!IsDisposed)
            {
                ShowHotspot();
                ShowPhone();
            }
        }

        private void HotspotStatusDone()
        {
            // periodic check, status only so a tapped-to address isn't reset
            if (!IsDisposed) ShowHotspot();
        }

        private void lbPhoneHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start((string)e.Link.LinkData);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuOptions/lbPhoneHelp_LinkClicked: " + ex.Message);
            }
        }

        private void MainMenu_MenuMoved(object sender, EventArgs e)
        {
            PositionForm();
        }

        private void pbPhoneQR_Click(object sender, EventArgs e)
        {
            // more than one network, show the next address
            if (PhoneList.Count > 1)
            {
                PhoneIndex = (PhoneIndex + 1) % PhoneList.Count;
                DrawPhone();
            }
        }

        private void PositionForm()
        {
            this.Top = MainMenu.Top + SubMenuLayout.TopOffset;
            this.Left = MainMenu.Left + SubMenuLayout.LeftOffset;
        }

        private void rbAOG_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing)
            {
                SetButtons(true);
                SetBoxes();
            }
        }

        private void rbWheel_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing)
            {
                WheelSpeedChanged = true;
                SetButtons(true);
                SetBoxes();
            }
        }

        private void SetBoxes()
        {
            tbWheelModule.Enabled = rbWheel.Checked;
            tbWheelPin.Enabled = rbWheel.Checked;
            tbWheelCal.Enabled = rbWheel.Checked;
            tbSimSpeed.Enabled = rbSimulated.Checked;
            lbCal.Enabled = rbWheel.Checked;
            lbModule.Enabled = rbWheel.Checked;
            lbPin.Enabled = rbWheel.Checked;
            butUpdateModules.Enabled = rbWheel.Checked;
            lbPulses.Enabled = rbWheel.Checked;
            btnCal.Enabled = rbWheel.Checked;
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

        private void SetPhoneLinks()
        {
            // windows settings pages, each link found by its (translated) words in the text
            lbPhoneHelp.Links.Clear();
            int Start = lbPhoneHelp.Text.IndexOf(Lang.lgWifiSettings);
            if (Start >= 0) lbPhoneHelp.Links.Add(Start, Lang.lgWifiSettings.Length, "ms-settings:network-wifi");

            // power saving turns the hotspot off after 5 min with no devices, it can only be changed in windows
            lnkHotspot.Links.Clear();
            Start = lnkHotspot.Text.IndexOf(Lang.lgHotspotSettings);
            if (Start >= 0) lnkHotspot.Links.Add(Start, Lang.lgHotspotSettings.Length, "ms-settings:network-mobilehotspot");

            // theme text colour, the underline marks the links (default blue is unreadable on dark)
            foreach (LinkLabel Lnk in new LinkLabel[] { lbPhoneHelp, lnkHotspot })
            {
                Lnk.LinkColor = Lnk.ForeColor;
                Lnk.ActiveLinkColor = Lnk.ForeColor;
                Lnk.VisitedLinkColor = Lnk.ForeColor;
            }
        }

        private void ShowHotspot()
        {
            string State = Core.WebServer.HotspotState;
            string Text = State;      // InTransition / Unknown shown as Windows reports them
            if (State == "On") Text = Lang.lgOn;
            else if (State == "Off") Text = Lang.lgOff;
            if (Core.WebServer.HotspotBusy) Text = Lang.lgChecking;
            if (State == "On" && !Core.WebServer.HotspotAllowed()) Text += ", " + Lang.lgFirewallBlocked;
            if (Core.WebServer.HotspotGaveUp && ckHotspot.Checked) Text = Lang.lgRestartsFailed;
            lbHotspot.Text = Text;

            if (State == "On")
            {
                lbHotspotNet.Text = Lang.lgNetwork + ": " + Core.WebServer.HotspotSsid + "\n" + Lang.lgPassword + ": " + Core.WebServer.HotspotPassword;
            }
            else if (Core.WebServer.HotspotResult != "" && Core.WebServer.HotspotResult != "Success")
            {
                lbHotspotNet.Text = string.Format(Lang.lgWindowsReason, Core.WebServer.HotspotResult);      // e.g. WiFiDeviceOff
            }
            else
            {
                lbHotspotNet.Text = "";
            }
        }

        private void ShowPhone()
        {
            PhoneList = new List<string>();
            if (Core.WebServer.IsRunning) PhoneList = Core.WebServer.PhoneUrls();
            PhoneIndex = 0;
            DrawPhone();
        }

        private void Setting_Changed(object sender, EventArgs e)
        {
            // SetButtons already ignores changes made while the form is loading
            SetButtons(true);
        }

        private void WheelSetting_Changed(object sender, EventArgs e)
        {
            // module, pin and cal all travel to the module in PGN 32504, so an edit has to
            // mark WheelSpeedChanged too - btnOK only sends when that is set, and until now
            // only the Wheel radio set it, so editing a pin alone saved without sending
            if (!Initializing)
            {
                WheelSpeedChanged = true;
                SetButtons(true);
            }
        }

        private void SetLanguage()
        {
            ckMetric.Text = Lang.lgMetric;
            rbSimulated.Text = Lang.lgSimulateSpeed;
            ckCompact.Text = Lang.lgCompactDisplay;
            tabPage1.Text = Lang.lgSpeed;
            tabPage2.Text = Lang.lgPhone;
            lbPhone.Text = Lang.lgPhoneAddress;
            ckHotspot.Text = Lang.lgTabletHotspot;
            lbPhoneHelp.Text = string.Format(Lang.lgPhoneHelp, Lang.lgWifiSettings);
            lnkHotspot.Text = string.Format(Lang.lgPowerSavingHint, Lang.lgHotspotSettings);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // refreshed on every visit: the hotspot may have been started since the page opened
            tmrPhone.Enabled = (tabControl1.SelectedTab == tabPage2);
            if (tabControl1.SelectedTab == tabPage2)
            {
                ShowPhone();
                ShowHotspot();
                Core.WebServer.HotspotAsync("status", HotspotDone);     // catches changes made in Windows settings
            }
        }

        private void tmrPhone_Tick(object sender, EventArgs e)
        {
            // adapters come up a few seconds after the hotspot reports On, and networks change
            List<string> Now = Core.WebServer.IsRunning ? Core.WebServer.PhoneUrls() : new List<string>();
            if (string.Join(",", Now) != string.Join(",", PhoneList))
            {
                // networks changed, back to the most likely address
                PhoneList = Now;
                PhoneIndex = 0;
                DrawPhone();
            }

            // hotspot state every 6 s (each check is a short powershell), catches changes made in windows
            HotspotTicks++;
            if (HotspotTicks >= 3 && !Core.WebServer.HotspotBusy)
            {
                HotspotTicks = 0;
                Core.WebServer.HotspotAsync("status", HotspotStatusDone);
            }
        }

        private void tbSimSpeed_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbSimSpeed.Text, out tempD);
            using (var form = new FormNumeric(0, 40, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbSimSpeed.Text = form.ReturnValue.ToString("N0");
                }
            }
        }

        private void tbSimSpeed_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbSimSpeed.Text, out tempD);
            if (tempD < 0 || tempD > 40)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbWheelCal_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbWheelCal.Text, out tempD);
            using (var form = new FormNumeric(0, 0xffffff, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbWheelCal.Text = form.ReturnValue.ToString("N0");
                }
            }
        }

        private void tbWheelCal_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbWheelCal.Text, out tempD);
            if (tempD < 0 || tempD > 0xffffff)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbWheelModule_Enter(object sender, EventArgs e)
        {
            int tempInt;
            int.TryParse(tbWheelModule.Text, out tempInt);
            using (var form = new FormNumeric(0, 7, tempInt))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbWheelModule.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void tbWheelModule_Validating(object sender, CancelEventArgs e)
        {
            int tempInt;
            int.TryParse(tbWheelModule.Text, out tempInt);
            if (tempInt < 0 || tempInt > 7)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbWheelPin_Enter(object sender, EventArgs e)
        {
            var bx = (System.Windows.Forms.TextBox)sender;
            double temp = 0;
            if (double.TryParse(bx.Text.Trim(), out double vl)) temp = vl;

            using (var form = new FormNumeric(0, 50, temp))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (form.IsBlank)
                    {
                        bx.Text = "-";
                    }
                    else
                    {
                        bx.Text = form.ReturnValue.ToString("N0");
                    }
                }
            }
        }

        private void tbWheelPin_Validating(object sender, CancelEventArgs e)
        {
            int tempInt;
            int.TryParse(tbWheelPin.Text, out tempInt);
            if (tempInt < 0 || tempInt > 50)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void UpdateForm()
        {
            Initializing = true;

            switch (Props.SpeedMode)
            {
                case SpeedType.Wheel:
                    rbWheel.Checked = true;
                    break;

                case SpeedType.Simulated:
                    rbSimulated.Checked = true;
                    break;

                default:
                    rbAOG.Checked = true;
                    break;
            }

            tbWheelModule.Text = Core.WheelSpeed.WheelModule.ToString("N0");

            if (Core.WheelSpeed.WheelPin == 255)
            {
                tbWheelPin.Text = "-";
            }
            else
            {
                tbWheelPin.Text = Core.WheelSpeed.WheelPin.ToString("N0");
            }

            tbWheelCal.Text = Core.WheelSpeed.WheelCal.ToString("N0");

            if (Props.UseMetric)
            {
                lbSimUnits.Text = "KMH";
                lbPulses.Text = "(pulses/km)";
                tbSimSpeed.Text = Props.SimSpeed_KMH.ToString("N1");
            }
            else
            {
                lbSimUnits.Text = "MPH";
                lbPulses.Text = "(pulses/mile)";
                tbSimSpeed.Text = (Props.SimSpeed_KMH / Props.MPHtoKPH).ToString("N1");
            }

            ckMetric.Checked = Props.UseMetric;
            ckCompact.Checked = Props.CompactDisplay;
            ckHotspot.Checked = Props.TabletHotspot;

            SetBoxes();

            Initializing = false;
        }

        private bool ValidPin(int pin)
        {
            bool Result = true;

            // The wheel sensor is wired to the module named in tbWheelModule, but the app
            // only holds pin data for the module currently loaded on the config pages -
            // when those are different modules there is nothing here to compare against.
            int.TryParse(tbWheelModule.Text, out int WheelModule);
            byte[] config = Core.ModuleConfig.GetData();

            if (WheelModule == config[2])
            {
                // The module attaches its own interrupt to the wheel pin and skips wheel
                // speed entirely when it duplicates an active sensor's flow pin, so check
                // every configured sensor from the pins config (PGN 32507) - not just the
                // two mirrored into the module config - and stop at the sensor count so an
                // unused sensor's pin cannot veto a legal wheel pin.
                int Count = config[3];
                int Max = Core.MaxSensorsForModule(config[2]);
                if (Count > Max) Count = Max;

                for (int i = 0; i < Count; i++)
                {
                    if (Core.SensorPins.FlowPin(i) == pin)
                    {
                        Result = false;
                        Props.ShowMessage("Invalid pin, duplicate of flow pin.");
                        break;
                    }
                }
            }
            return Result;
        }
    }
}