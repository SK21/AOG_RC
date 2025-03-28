using RateController.Classes;
using RateController.Language;
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
            if (int.TryParse(Props.GetProp("BoardType"), out int bt)) BoardType = bt;
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
                Props.SetProp("BoardType", BoardType.ToString());
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuNetwork/btnOk_Click: " + ex.Message);
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
                PGN32503 SetSubnet = new PGN32503(mf);
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
            Props.SaveFormLocation(this);
        }

        private void frmMenuNetwork_Load(object sender, EventArgs e)
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
            if (int.TryParse(Props.GetProp("BoardType"), out int bt)) BoardType = bt;
            UpdateForm();
        }

        private void frmMenuNetwork_Shown(object sender, EventArgs e)
        {
            // check for no settings
            if (!MainMenu.MenuNetworkHasRan)
            {
                MainMenu.MenuNetworkHasRan = true;
                if (mf.ModuleConfig.Sensor0Flow == 0 && mf.ModuleConfig.Sensor0Dir == 0 && mf.ModuleConfig.Sensor0PWM == 0
                    && mf.ModuleConfig.Sensor1Dir == 0 && mf.ModuleConfig.Sensor1Flow == 0 && mf.ModuleConfig.Sensor1PWM == 0)
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
                Props.WriteErrorLog("frmModuleConfig/LoadCombo " + ex.Message);
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
            PGN32700 Set = mf.ModuleConfig;
            byte[] Pins = new byte[16];

            for (int i = 0; i < 16; i++)
            {
                Pins[i] = 255;
            }

            switch (BoardType)
            {
                case 1:
                    // RC11, Teensy
                    Set.ModuleID = 0;
                    Set.SensorCount = 2;
                    Set.WifiPort = 1;
                    Set.RelayType = 1;
                    Set.InvertRelay = true;
                    Set.InvertFlow = true;
                    Set.Momentary = false;
                    Set.Sensor0Flow = 28;
                    Set.Sensor1Flow = 29;
                    Set.Sensor0Dir = 37;
                    Set.Sensor1Dir = 14;
                    Set.Sensor0PWM = 36;
                    Set.Sensor1PWM = 15;
                    Set.WorkPin = 255;
                    Set.PressurePin = 255;
                    Set.ClientMode = false;
                    Set.Is3Wire = true;

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
                    Set.ModuleID = 0;
                    Set.SensorCount = 2;
                    Set.WifiPort = 0;
                    Set.RelayType = 5;
                    Set.InvertRelay = true;
                    Set.InvertFlow = true;
                    Set.Momentary = false;
                    Set.Sensor0Flow = 17;
                    Set.Sensor1Flow = 16;
                    Set.Sensor0Dir = 32;
                    Set.Sensor1Dir = 25;
                    Set.Sensor0PWM = 33;
                    Set.Sensor1PWM = 26;
                    Set.WorkPin = 255;
                    Set.PressurePin = 255;
                    Set.ClientMode = false;
                    Set.Is3Wire = true;
                    break;

                default:
                    // RC12, Nano
                    Set.ModuleID = 0;
                    Set.SensorCount = 1;
                    Set.WifiPort = 0;
                    Set.RelayType = 2;
                    Set.InvertRelay = true;
                    Set.InvertFlow = true;
                    Set.Momentary = false;
                    Set.Sensor0Flow = 3;
                    Set.Sensor1Flow = 255;
                    Set.Sensor0Dir = 6;
                    Set.Sensor1Dir = 255;
                    Set.Sensor0PWM = 9;
                    Set.Sensor1PWM = 255;
                    Set.WorkPin = 255;
                    Set.PressurePin = 255;
                    Set.ClientMode = false;
                    Set.Is3Wire = true;
                    break;
            }

            Set.RelayPins(Pins);
            Set.Save();

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
            Initializing = false;
        }
    }
}