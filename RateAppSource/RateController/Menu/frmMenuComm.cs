using RateController.Classes;
using RateController.PGNs;
using System;
using System.Drawing;
using System.IO.Ports;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuComm : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;

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

            // Check if diagnostics setting changed
            bool diagnosticsChanged = Props.ShowCanDiagnostics != ckDiagnostics.Checked;
            if (diagnosticsChanged) Props.ShowCanDiagnostics = ckDiagnostics.Checked;

            if (Props.CanEnabled != ckCanBus.Checked) CanSettingsChanged = true;
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

                Core.ModuleConfig.Save();
                Props.ShowMessage("Comm change sent to module. " + Mes, "Config", 10000);
            }

            SetButtons(false);
            UpdateForm();
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
            timer1.Enabled = false;
            Props.SaveFormLocation(this);
        }

        private void frmMenuComm_Load(object sender, System.EventArgs e)
        {
            SubMenuLayout.SetFormLayout(this, MainMenu, btnOK);

            btnCancel.Left = btnOK.Left - SubMenuLayout.ButtonSpacing;
            btnCancel.Top = btnOK.Top;
            MainMenu.StyleControls(this);
            SetLanguage();
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

        private void timer1_Tick(object sender, EventArgs e)
        {
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
                lbModule.Text = Core.ModuleConfigDescription();
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