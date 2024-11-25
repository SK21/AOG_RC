using AgOpenGPS;
using System;
using System.Drawing;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmModuleConfig : Form
    {
        public FormStart mf;
        private int BoardType = 0;  // 0 nano, 1 Teensy, 2 ESP32
        private bool FormEdited;
        private bool Initializing;
        TextBox[] RelayTB;

        public frmModuleConfig(FormStart Main)
        {
            InitializeComponent();

            #region // language

            lbSubnet.Text = Lang.lgSelectedSubnet;
            lbIP.Text = Lang.lgConfigIP;
            lbModuleID.Text = Lang.lgModuleID;
            lbSensorCount.Text = Lang.lgSensorCount;
            lbWifiPort.Text = Lang.lgWifiPort;
            lbRelay.Text = Lang.lgRelayControl;
            ckRelayOn.Text = Lang.lgRelayOnHigh;
            ckFlowOn.Text = Lang.lgFlowOnHigh;

            tabControl1.TabPages[0].Text = Lang.lgBoards;
            tabControl1.TabPages[1].Text = Lang.lgNetwork;
            tabControl1.TabPages[2].Text = Lang.lgConfig;
            tabControl1.TabPages[3].Text = Lang.lgPins;
            tabControl1.TabPages[4].Text = Lang.lgRelays;
            tabControl1.TabPages[5].Text = Lang.lgWifiClient;
            tabControl1.TabPages[6].Text = Lang.lgValves;

            lbWorkPin.Text = Lang.lgWorkPin;

            #endregion // language

            mf = Main;
            mf.ModulesStatus.PinStatusChanged += ArduinoModule_PinStatusChanged;

            RelayTB = new TextBox[] { tbRelay1,tbRelay2,tbRelay3,tbRelay4,tbRelay5,tbRelay6,tbRelay7,tbRelay8,
            tbRelay9,tbRelay10,tbRelay11,tbRelay12,tbRelay13,tbRelay14,tbRelay15,tbRelay16};

            for (int i = 0; i < 16; i++)
            {
                RelayTB[i].Enter += tbRelays_enter;
                RelayTB[i].TextChanged += textbox_TextChanged;
            }
        }
        private void tbRelays_enter(object sender, EventArgs e)
        {
            TextBox bx = (TextBox)sender;
            double temp = 0;
            if (double.TryParse(bx.Text.Trim(), out double vl)) temp = vl;
            using (var form = new FormNumeric(0, 50, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    bx.Text = form.ReturnValue.ToString("N0");
                }
            }
        }
        private void ArduinoModule_PinStatusChanged(object sender, PGN32401.PinStatusArgs e)
        {
            if (!e.GoodPins) mf.Tls.ShowHelp("Pin configuration for Module" + e.Module.ToString() + " not correct. Using default values.");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (int.TryParse(mf.Tls.LoadProperty("BoardType"), out int bt)) BoardType = bt;
            UpdateForm();
            SetButtons(false);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (!FormEdited)
            {
                // exit
                this.Close();
            }
            else
            {
                if (mf.Tls.ReadOnly)
                {
                    mf.Tls.ShowHelp("File is read only.", "Help", 5000, false, false, true);
                }
                else
                {
                    // save
                    // IP
                    mf.UDPmodules.NetworkEP = cbEthernet.Text;

                    Save();
                    SetButtons(false);
                    UpdateForm();

                    btnSendToModule.Text = "C";
                    btnSendToModule.FlatAppearance.BorderSize = 3;
                    btnSendToModule.FlatAppearance.BorderColor = Color.DarkBlue;
                }
            }
        }

        private void btnPCB_Click(object sender, EventArgs e)
        {
            Form fs = mf.Tls.IsFormOpen("frmPins");

            if (fs == null)
            {
                Form frm = new frmPins(mf);
                frm.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private void btnRescan_Click(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void btnRescan_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Rescan";

            mf.Tls.ShowHelp(Message, "Rescan");
            hlpevent.Handled = true;
        }

        private void btnSendSubnet_Click(object sender, EventArgs e)
        {
            try
            {
                PGN32503 SetSubnet = new PGN32503(mf);
                if (SetSubnet.Send(mf.UDPmodules.NetworkEP))
                {
                    mf.Tls.ShowHelp("New Subnet address sent.", "Subnet", 10000);

                    // set app subnet
                    mf.UDPmodules.NetworkEP = cbEthernet.Text;
                }
                else
                {
                    mf.Tls.ShowHelp("New Subnet address not sent.", "Subnet", 10000);
                }
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp(ex.Message, "frmModuleConfig/btnSendSubnet", 15000, true);
            }
        }

        private void btnSendSubnet_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Send subnet address to modules.";

            mf.Tls.ShowHelp(Message, "Subnet");
            hlpevent.Handled = true;
        }

        private void btnSendToModule_Click(object sender, EventArgs e)
        {
            try
            {
                mf.ModuleConfig.Send();
                mf.NetworkConfig.Send();
                mf.Tls.ShowHelp("Settings sent to module", "Config", 10000);

                btnSendToModule.Text = "";
                btnSendToModule.FlatAppearance.BorderSize = 0;
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp("frmModuleConfig/btnSendToModule  " + ex.Message, "Help", 10000, true, true);
            }
        }

        private void btnSendToModule_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Send settings to module. Only have 1 module connected when sending. The button is highlighted when there are changes to be sent.";
            mf.Tls.ShowHelp(Message, "Send Config");
            hlpevent.Handled = true;
        }

        private void cbEthernet_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void cbRelayControl_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "1 - GPIOs, use the micro-controller pins.\n" +
                            "2 - PCA9555 8 relays, use 8 relay module.\n" +
                            "3 - PCA9555 16 relays, use 16 relay module.\n" +
                            "4 - MCP23017, use a MCP23017 IO expander.\n" +
                            "5 - PCA9685 single, use each PCA9685 pin to control a single relay.\n" +
                            "6 - PCA9685 paired, use consecutive pins to control relays in a complementary mode. One is on and the other off.\n" +
                            "7 - PCF8574";

            mf.Tls.ShowHelp(Message, "Relay Control");
            hlpevent.Handled = true;
        }

        private void ckClient_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void ckClient_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Module connects as a client of an external wifi network instead of connecting over its own access point network.";

            mf.Tls.ShowHelp(Message);
            hlpevent.Handled = true;
        }

        private void SwitchBoards()
        {
            switch (BoardType)
            {
                case 1:
                    // RC11, Teensy
                    tbModuleID.Text = "0";
                    tbSensorCount.Text = "2";
                    tbWifiPort.Text = "1";
                    cbRelayControl.SelectedIndex = 1;

                    ckRelayOn.Checked = true;
                    ckFlowOn.Checked = true;
                    ckMomentary.Checked = false;

                    tbFlow1.Text = "28";
                    tbFlow2.Text = "29";
                    tbDir1.Text = "37";
                    tbDir2.Text = "14";
                    tbPWM1.Text = "36";
                    tbPWM2.Text = "15";
                    tbWrk.Text = "-";

                    tbRelay1.Text = "8";
                    tbRelay2.Text = "9";
                    tbRelay3.Text = "10";
                    tbRelay4.Text = "11";
                    tbRelay5.Text = "12";
                    tbRelay6.Text = "25";
                    tbRelay7.Text = "26";
                    tbRelay8.Text = "27";

                    tbRelay9.Text = "-";
                    tbRelay10.Text = "-";
                    tbRelay11.Text = "-";
                    tbRelay12.Text = "-";
                    tbRelay13.Text = "-";
                    tbRelay14.Text = "-";
                    tbRelay15.Text = "-";
                    tbRelay16.Text = "-";

                    tbSSID.Text = "Tractor";
                    tbPassword.Text = "111222333";
                    ckClient.Checked = false;
                    break;

                case 2:
                    // RC17, ESP32
                    tbModuleID.Text = "0";
                    tbSensorCount.Text = "1";
                    tbWifiPort.Text = "0";
                    cbRelayControl.SelectedIndex = 4;

                    ckRelayOn.Checked = true;
                    ckFlowOn.Checked = true;
                    ckMomentary.Checked = false;

                    tbFlow1.Text = "-";
                    tbFlow2.Text = "-";
                    tbDir1.Text = "-";
                    tbDir2.Text = "-";
                    tbPWM1.Text = "-";
                    tbPWM2.Text = "-";
                    tbWrk.Text = "-";

                    tbRelay1.Text = "-";
                    tbRelay2.Text = "-";
                    tbRelay3.Text = "-";
                    tbRelay4.Text = "-";
                    tbRelay5.Text = "-";
                    tbRelay6.Text = "-";
                    tbRelay7.Text = "-";
                    tbRelay8.Text = "-";

                    tbRelay9.Text = "-";
                    tbRelay10.Text = "-";
                    tbRelay11.Text = "-";
                    tbRelay12.Text = "-";
                    tbRelay13.Text = "-";
                    tbRelay14.Text = "-";
                    tbRelay15.Text = "-";
                    tbRelay16.Text = "-";

                    tbSSID.Text = "Tractor";
                    tbPassword.Text = "111222333";
                    ckClient.Checked = false;
                    rb3Wire.Checked = true;
                    break;

                default:
                    // RC12, Nano
                    tbModuleID.Text = "0";
                    tbSensorCount.Text = "1";
                    tbWifiPort.Text = "0";
                    cbRelayControl.SelectedIndex = 2;

                    ckRelayOn.Checked = true;
                    ckFlowOn.Checked = true;
                    ckMomentary.Checked = false;

                    tbFlow1.Text = "3";
                    tbFlow2.Text = "-";
                    tbDir1.Text = "6";
                    tbDir2.Text = "-";
                    tbPWM1.Text = "9";
                    tbPWM2.Text = "-";
                    tbWrk.Text = "-";

                    tbRelay1.Text = "-";
                    tbRelay2.Text = "-";
                    tbRelay3.Text = "-";
                    tbRelay4.Text = "-";
                    tbRelay5.Text = "-";
                    tbRelay6.Text = "-";
                    tbRelay7.Text = "-";
                    tbRelay8.Text = "-";

                    tbRelay9.Text = "-";
                    tbRelay10.Text = "-";
                    tbRelay11.Text = "-";
                    tbRelay12.Text = "-";
                    tbRelay13.Text = "-";
                    tbRelay14.Text = "-";
                    tbRelay15.Text = "-";
                    tbRelay16.Text = "-";

                    tbSSID.Text = "Tractor";
                    tbPassword.Text = "111222333";
                    ckClient.Checked = false;
                    break;
            }
        }
        private void ckDefaultModule_CheckedChanged(object sender, EventArgs e)
        {
            if (ckDefaultModule.Checked)
            {
                SwitchBoards();
                SetButtons(true);
            }
        }

        private void frmModuleConfig_FormClosed(object sender, FormClosedEventArgs e)
        {
            mf.Tls.SaveFormData(this);
        }

        private void frmModuleConfig_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            SetDayMode();
            if (int.TryParse(mf.Tls.LoadProperty("BoardType"), out int bt)) BoardType = bt;
            UpdateForm();

            // check for no settings
            if ((tbFlow1.Text == "0") && (tbFlow2.Text == "0") && (tbDir1.Text == "0")
                && (tbDir2.Text == "0") && (tbPWM1.Text == "0") && (tbPWM2.Text == "0"))
            {
                ckDefaultModule.Checked = true;
            }
        }

        private void groupBox1_Paint_1(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            mf.Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);
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
                cbEthernet.SelectedIndex = cbEthernet.FindString(SubAddress(mf.UDPmodules.NetworkEP));
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmModuleConfig/LoadCombo " + ex.Message);
            }
        }

        private void ModuleID_Enter(object sender, EventArgs e)
        {
            double temp;
            double.TryParse(tbModuleID.Text, out temp);
            using (var form = new FormNumeric(0, 8, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbModuleID.Text = form.ReturnValue.ToString("N0");
                }
            }
        }

        private void rbESP32_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing)
            {
                BoardType = 2;
                SwitchBoards();
                ckDefaultModule.Checked = true;
                SetButtons(true);
            }
        }

        private void rbNano_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing)
            {
                BoardType = 0;
                SwitchBoards();
                ckDefaultModule.Checked = true;
                SetButtons(true);
            }
        }

        private void rbTeensy_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing)
            {
                BoardType = 1;
                SwitchBoards();
                ckDefaultModule.Checked = true;
                SetButtons(true);
            }
        }

        private void Save()
        {
            byte val;
            byte[] Pins = new byte[16];

            if (byte.TryParse(tbModuleID.Text, out val))
            {
                mf.ModuleConfig.ModuleID = val;
            }
            if (byte.TryParse(tbSensorCount.Text, out val))
            {
                mf.ModuleConfig.SensorCount = val;
            }
            if (byte.TryParse(tbWifiPort.Text, out val))
            {
                mf.ModuleConfig.WifiPort = val;
            }
            mf.ModuleConfig.RelayType = (byte)cbRelayControl.SelectedIndex;
            mf.ModuleConfig.RelayOnHigh = ckRelayOn.Checked;
            mf.ModuleConfig.FlowOnHigh = ckFlowOn.Checked;
            mf.ModuleConfig.Momentary = ckMomentary.Checked;
            mf.ModuleConfig.Is3Wire = rb3Wire.Checked;

            // flow
            if (byte.TryParse(tbFlow1.Text, out val))
            {
                mf.ModuleConfig.Sensor0Flow = val;
            }
            else
            {
                mf.ModuleConfig.Sensor0Flow = 255;
            }
            if (byte.TryParse(tbFlow2.Text, out val))
            {
                mf.ModuleConfig.Sensor1Flow = val;
            }
            else
            {
                mf.ModuleConfig.Sensor1Flow = 255;
            }

            // motor
            if (byte.TryParse(tbDir1.Text, out val))
            {
                mf.ModuleConfig.Sensor0Dir = val;
            }
            else
            {
                mf.ModuleConfig.Sensor0Dir = 255;
            }
            if (byte.TryParse(tbDir2.Text, out val))
            {
                mf.ModuleConfig.Sensor1Dir = val;
            }
            else
            {
                mf.ModuleConfig.Sensor1Dir = 255;
            }
            if (byte.TryParse(tbPWM1.Text, out val))
            {
                mf.ModuleConfig.Sensor0PWM = val;
            }
            else
            {
                mf.ModuleConfig.Sensor0PWM = 255;
            }
            if (byte.TryParse(tbPWM2.Text, out val))
            {
                mf.ModuleConfig.Sensor1PWM = val;
            }
            else
            {
                mf.ModuleConfig.Sensor1PWM = 255;
            }
            if (byte.TryParse(tbWrk.Text, out val))
            {
                mf.ModuleConfig.WorkPin = val;
            }
            else
            {
                mf.ModuleConfig.WorkPin = 255;
            }

            // Pins
            for (int i = 0; i < 16; i++)
            {
                Pins[i] = 255;
            }

            if (byte.TryParse(tbRelay1.Text, out val))
            {
                Pins[0] = val;
            }
            if (byte.TryParse(tbRelay2.Text, out val))
            {
                Pins[1] = val;
            }
            if (byte.TryParse(tbRelay3.Text, out val))
            {
                Pins[2] = val;
            }
            if (byte.TryParse(tbRelay4.Text, out val))
            {
                Pins[3] = val;
            }

            if (byte.TryParse(tbRelay5.Text, out val))
            {
                Pins[4] = val;
            }
            if (byte.TryParse(tbRelay6.Text, out val))
            {
                Pins[5] = val;
            }
            if (byte.TryParse(tbRelay7.Text, out val))
            {
                Pins[6] = val;
            }
            if (byte.TryParse(tbRelay8.Text, out val))
            {
                Pins[7] = val;
            }

            if (byte.TryParse(tbRelay9.Text, out val))
            {
                Pins[8] = val;
            }
            if (byte.TryParse(tbRelay10.Text, out val))
            {
                Pins[9] = val;
            }
            if (byte.TryParse(tbRelay11.Text, out val))
            {
                Pins[10] = val;
            }
            if (byte.TryParse(tbRelay12.Text, out val))
            {
                Pins[11] = val;
            }

            if (byte.TryParse(tbRelay13.Text, out val))
            {
                Pins[12] = val;
            }
            if (byte.TryParse(tbRelay14.Text, out val))
            {
                Pins[13] = val;
            }
            if (byte.TryParse(tbRelay15.Text, out val))
            {
                Pins[14] = val;
            }
            if (byte.TryParse(tbRelay16.Text, out val))
            {
                Pins[15] = val;
            }

            mf.ModuleConfig.ClientMode = ckClient.Checked;
            mf.ModuleConfig.RelayPins(Pins);
            mf.ModuleConfig.Save();

            mf.NetworkConfig.NetworkName = tbSSID.Text;
            mf.NetworkConfig.NetworkPassword = tbPassword.Text;
            mf.NetworkConfig.Save();

            // board type
            mf.Tls.SaveProperty("BoardType", BoardType.ToString());
        }

        private void SensorCount_Enter(object sender, EventArgs e)
        {
            double temp;
            double.TryParse(tbSensorCount.Text, out temp);
            using (var form = new FormNumeric(0, 2, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbSensorCount.Text = form.ReturnValue.ToString("N0");
                }
            }
        }

        private void SetButtons(bool Edited)
        {
            if (!Initializing)
            {
                if (Edited)
                {
                    btnCancel.Enabled = true;
                    btnClose.Image = Properties.Resources.Save;
                    btnRescan.Enabled = false;
                    btnSendSubnet.Enabled = false;
                    btnSendToModule.Enabled = false;
                }
                else
                {
                    btnCancel.Enabled = false;
                    btnClose.Image = Properties.Resources.OK;
                    btnRescan.Enabled = true;
                    btnSendSubnet.Enabled = true;
                    btnSendToModule.Enabled = true;
                }

                FormEdited = Edited;
            }
        }

        private void SetDayMode()
        {
            try
            {
                this.BackColor = Properties.Settings.Default.DayColour;

                for (int i = 0; i < tabControl1.TabCount; i++)
                {
                    tabControl1.TabPages[i].BackColor = Properties.Settings.Default.DayColour;
                }
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp(ex.Message, this.Text, 3000, true);
            }
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

        private void tbPassword_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Password for the network the module connects to in Client Mode.";

            mf.Tls.ShowHelp(Message);
            hlpevent.Handled = true;
        }

        private void tbPassword_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbSSID_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Name of the network the module connects to in Client Mode.";

            mf.Tls.ShowHelp(Message);
            hlpevent.Handled = true;
        }

        private void tbSSID_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void textBox_Enter(object sender, EventArgs e)
        {
            double temp;
            bool Found = false;
            for (int j = 0; j < tabControl1.TabCount; j++)
            {
                for (int i = 0; i < tabControl1.TabPages[j].Controls.Count; i++)
                {
                    if (sender.Equals(tabControl1.TabPages[j].Controls[i]))
                    {
                        double.TryParse(tabControl1.TabPages[j].Controls[i].Text, out temp);
                        using (var form = new FormNumeric(0, 255, temp))
                        {
                            var result = form.ShowDialog();
                            if (result == DialogResult.OK)
                            {
                                tabControl1.TabPages[j].Controls[i].Text = form.ReturnValue.ToString("N0");
                            }
                        }
                        Found = true;
                        break;
                    }
                }
                if (Found) break;
            }
        }

        private void textbox_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void UpdateForm()
        {
            Initializing = true;

            byte[] data = mf.ModuleConfig.GetData();
            string[] display = new string[data.Length];

            tbModuleID.Text = data[2].ToString();
            tbSensorCount.Text = data[3].ToString();
            ckRelayOn.Checked = ((data[4] & 1) == 1);
            ckFlowOn.Checked = ((data[4] & 2) == 2);
            cbRelayControl.SelectedIndex = data[5];
            tbWifiPort.Text = data[6].ToString();
            ckMomentary.Checked = ((data[4] & 8) == 8);
            rb3Wire.Checked = ((data[4] & 0b0001_0000) == 0b0001_0000);

            // flow, motor
            for (int i = 7; i < 13; i++)
            {
                if (data[i] > 60)
                {
                    display[i] = "-";
                }
                else
                {
                    display[i] = data[i].ToString();
                }
            }
            tbFlow1.Text = display[7].ToString();
            tbDir1.Text = display[8].ToString();
            tbPWM1.Text = display[9].ToString();
            tbFlow2.Text = display[10].ToString();
            tbDir2.Text = display[11].ToString();
            tbPWM2.Text = display[12].ToString();

            // work pin
            if (data[29] > 60)
            {
                tbWrk.Text = "-";
            }
            else
            {
                tbWrk.Text = data[29].ToString();
            }

            // relays
            for (int i = 13; i < 29; i++)
            {
                if (data[i] > 60)
                {
                    display[i] = "-";
                }
                else
                {
                    display[i] = data[i].ToString();
                }
            }
            tbRelay1.Text = display[13].ToString();
            tbRelay2.Text = display[14].ToString();
            tbRelay3.Text = display[15].ToString();
            tbRelay4.Text = display[16].ToString();

            tbRelay5.Text = display[17].ToString();
            tbRelay6.Text = display[18].ToString();
            tbRelay7.Text = display[19].ToString();
            tbRelay8.Text = display[20].ToString();

            tbRelay9.Text = display[21].ToString();
            tbRelay10.Text = display[22].ToString();
            tbRelay11.Text = display[23].ToString();
            tbRelay12.Text = display[24].ToString();

            tbRelay13.Text = display[25].ToString();
            tbRelay14.Text = display[26].ToString();
            tbRelay15.Text = display[27].ToString();
            tbRelay16.Text = display[28].ToString();

            LoadCombo();
            lbModuleIP.Text = mf.UDPmodules.SubNet;

            tbSSID.Text = mf.NetworkConfig.NetworkName;
            tbPassword.Text = mf.NetworkConfig.NetworkPassword;
            ckClient.Checked = ((data[4] & 4) == 4);

            switch (BoardType)
            {
                case 1:
                    rbTeensy.Checked = true; 
                    break;

                case 2:
                    rbESP32.Checked = true;
                    break;

                default:
                    rbNano.Checked = true;
                    break;
            }

            ckDefaultModule.Checked = false;

            Initializing = false;
        }

        private void WifiPort_Enter(object sender, EventArgs e)
        {
            double temp;
            double.TryParse(tbWifiPort.Text, out temp);
            using (var form = new FormNumeric(0, 8, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbWifiPort.Text = form.ReturnValue.ToString("N0");
                }
            }
        }

        private void tbSensorCount_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "The number of sensors connected.";

            mf.Tls.ShowHelp(Message, "Sensors");
            hlpevent.Handled = true;

        }
    }
}