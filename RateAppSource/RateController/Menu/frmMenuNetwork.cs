using AgOpenGPS;
using RateController.Language;
using RateController.PGNs;
using System;
using System.Drawing;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuNetwork : Form
    {
        private int BoardType = 0;
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private FormStart mf;

        public frmMenuNetwork(FormStart main, frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            mf = main;
            this.Tag = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (int.TryParse(mf.Tls.LoadProperty("BoardType"), out int bt)) BoardType = bt;
            UpdateForm();
            SetButtons(false);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (ckDefaultModule.Checked) SetDefaults();
                mf.UDPmodules.NetworkEP = cbEthernet.Text;
                SetButtons(false);
                UpdateForm();
                MainMenu.HighlightUpdateButton();
                mf.Tls.SaveProperty("BoardType", BoardType.ToString());
                if (int.TryParse(tbModuleIDtoUpdate.Text, out int id)) MainMenu.ModuleID = id;
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmMenuNetwork/btnOk_Click: " + ex.Message);
            }
        }

        private void btnRescan_Click(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void btnSendSubnet_Click(object sender, EventArgs e)
        {
            try
            {
                PGN4 SetSubnet = new PGN4(mf);
                if (SetSubnet.Send(mf.UDPmodules.NetworkEP))
                {
                    mf.Tls.ShowMessage("New Subnet address sent.", "Subnet", 10000);

                    // set app subnet
                    mf.UDPmodules.NetworkEP = cbEthernet.Text;
                }
                else
                {
                    mf.Tls.ShowMessage("New Subnet address not sent.", "Subnet", 10000);
                }
            }
            catch (Exception ex)
            {
                mf.Tls.ShowMessage(ex.Message, "frmModuleConfig/btnSendSubnet", 15000, true);
            }
        }

        private void cbEthernet_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void ckDefaultModule_CheckedChanged(object sender, EventArgs e)
        {
            if (ckDefaultModule.Checked)
            {
                SetButtons(true);
            }
        }

        private void frmMenuNetwork_FormClosed(object sender, FormClosedEventArgs e)
        {
            mf.Tls.SaveFormData(this);
        }

        private void frmMenuNetwork_Load(object sender, EventArgs e)
        {
            // menu 800,600
            // sub menu 540,630
            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            mf.Tls.LoadFormData(this, "", false);
            this.BackColor = Properties.Settings.Default.BackColour;
            this.Width = MainMenu.Width - 260;
            this.Height = MainMenu.Height - 50;
            btnOK.Left = this.Width - 84;
            btnOK.Top = this.Height - 84;
            btnCancel.Left = btnOK.Left - 78;
            btnCancel.Top = btnOK.Top;
            MainMenu.StyleControls(this);
            PositionForm();
            if (int.TryParse(mf.Tls.LoadProperty("BoardType"), out int bt)) BoardType = bt;
            UpdateForm();
        }

        private void frmMenuNetwork_Shown(object sender, EventArgs e)
        {
            // check for no settings
            if (!MainMenu.MenuNetworkHasRan)
            {
                MainMenu.MenuNetworkHasRan = true;
                if (MainMenu.ModuleConfig2.Sensor0Flow == 0 && MainMenu.ModuleConfig2.Sensor0Dir == 0 && MainMenu.ModuleConfig2.Sensor0PWM == 0
                    && MainMenu.ModuleConfig2.Sensor1Dir == 0 && MainMenu.ModuleConfig2.Sensor1Flow == 0 && MainMenu.ModuleConfig2.Sensor1PWM == 0)
                {
                    mf.Tls.ShowMessage("Empty settings, default values selected.", "Default Values");
                    ckDefaultModule.Checked = true;
                    SetButtons(true);
                }
            }
        }

        private void groupBox2_Paint(object sender, PaintEventArgs e)
        {
            mf.Tls.DrawGroupBox((GroupBox)sender, e.Graphics, this.BackColor, Color.Black, Color.Blue);
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

        private void MainMenu_MenuMoved(object sender, EventArgs e)
        {
            PositionForm();
        }

        private void PositionForm()
        {
            this.Top = MainMenu.Top + 30;
            this.Left = MainMenu.Left + 246;
        }

        private void rbESP32_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing)
            {
                BoardType = 2;
                ckDefaultModule.Checked = true;
                SetButtons(true);
            }
        }

        private void rbNano_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing)
            {
                BoardType = 0;
                ckDefaultModule.Checked = true;
                SetButtons(true);
            }
        }

        private void rbTeensy_CheckedChanged(object sender, EventArgs e)
        {
            BoardType = 1;
            ckDefaultModule.Checked = true;
            SetButtons(true);
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

        private void SetDefaults()
        {
            PGN15 Config1 = MainMenu.ModuleConfig1;
            PGN16 Config2 = MainMenu.ModuleConfig2;
            PGN17 Config3 = MainMenu.ModuleConfig3;
            PGN18 Config4 = MainMenu.ModuleConfig4;

            byte[] Pins = new byte[16];

            for (int i = 0; i < 16; i++)
            {
                Pins[i] = 255;
            }

            switch (BoardType)
            {
                case 1:
                    // RC11, Teensy
                    Config1.ModuleID = 0;
                    Config1.SensorCount = 2;
                    Config1.WifiPort = 1;
                    Config1.RelayType = 1;
                    Config1.InvertRelay = true;
                    Config1.InvertFlow = true;
                    Config1.Momentary = false;
                    Config2.Sensor0Flow = 28;
                    Config2.Sensor1Flow = 29;
                    Config2.Sensor0Dir = 37;
                    Config2.Sensor1Dir = 14;
                    Config2.Sensor0PWM = 36;
                    Config2.Sensor1PWM = 15;
                    Config1.WorkPin = 255;
                    Config1.PressurePin = 255;
                    Config1.ClientMode = false;
                    Config1.Is3Wire = true;
                    Config1.ADS1115enabled = false;

                    Pins[0] = 8;    // relay 1
                    Pins[1] = 9;
                    Pins[2] = 10;
                    Pins[3] = 11;
                    Pins[4] = 12;
                    Pins[5] = 25;
                    Pins[6] = 26;
                    Pins[7] = 27;
                    break;

                case 2:
                    // RC15, ESP32
                    Config1.ModuleID = 0;
                    Config1.SensorCount = 2;
                    Config1.WifiPort = 0;
                    Config1.RelayType = 5;
                    Config1.InvertRelay = true;
                    Config1.InvertFlow = true;
                    Config1.Momentary = false;
                    Config2.Sensor0Flow = 17;
                    Config2.Sensor1Flow = 16;
                    Config2.Sensor0Dir = 32;
                    Config2.Sensor1Dir = 25;
                    Config2.Sensor0PWM = 33;
                    Config2.Sensor1PWM = 26;
                    Config1.WorkPin = 255;
                    Config1.PressurePin = 255;
                    Config1.ClientMode = false;
                    Config1.Is3Wire = true;
                    Config1.ADS1115enabled = false;
                    break;

                default:
                    // RC12, Nano
                    Config1.ModuleID = 0;
                    Config1.SensorCount = 1;
                    Config1.WifiPort = 0;
                    Config1.RelayType = 2;
                    Config1.InvertRelay = true;
                    Config1.InvertFlow = true;
                    Config1.Momentary = false;
                    Config2.Sensor0Flow = 3;
                    Config2.Sensor1Flow = 255;
                    Config2.Sensor0Dir = 6;
                    Config2.Sensor1Dir = 255;
                    Config2.Sensor0PWM = 9;
                    Config2.Sensor1PWM = 255;
                    Config1.WorkPin = 255;
                    Config1.PressurePin = 255;
                    Config1.ClientMode = false;
                    Config1.Is3Wire = true;
                    Config1.ADS1115enabled = false;
                    break;
            }

            Config1.Save();
            Config2.Save();
            Config3.SetRelayPins(Pins);
            Config3.Save();
            Config4.SetRelayPins(Pins);
            Config4.Save();

            mf.NetworkConfig.NetworkName = "Tractor";
            mf.NetworkConfig.NetworkPassword = "111222333";
            mf.NetworkConfig.Save();

            MainMenu.DefaultsSet();
        }

        private void SetLanguage()
        {
            lbSubnet.Text = Lang.lgSelectedSubnet;
            lbIP.Text = Lang.lgConfigIP;
            gbNetwork.Text = Lang.lgNetwork;
            gbBoards.Text = Lang.lgBoards;
            ckDefaultModule.Text = Lang.lgDefaults;
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

        private void tbModuleIDtoUpdate_Enter(object sender, EventArgs e)
        {
            double temp;
            double.TryParse(tbModuleIDtoUpdate.Text, out temp);
            using (var form = new FormNumeric(0, 8, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbModuleIDtoUpdate.Text = form.ReturnValue.ToString("N0");
                }
            }
        }

        private void UpdateForm()
        {
            Initializing = true;
            LoadCombo();
            lbModuleIP.Text = mf.UDPmodules.SubNet;

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
            tbModuleIDtoUpdate.Text = MainMenu.ModuleID.ToString();
            Initializing = false;
        }
    }
}