using System;
using System.Drawing;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmWifi : Form
    {
        public FormStart mf;
        private bool FormEdited;
        private bool Initializing;

        public frmWifi(FormStart CalledFrom)
        {
            InitializeComponent();
            mf = CalledFrom;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (!FormEdited)
            {
                this.Close();
            }
            else
            {
                // IP
                mf.UDPmodules.WifiEP = cbNetworks.Text;
                mf.UDPmodules.EthernetEP = cbEthernet.Text;

                SetButtons(false);
                UpdateForm();
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
            PGN32503 SetSubnet = new PGN32503(this);
            if (SetSubnet.Send(mf.UDPmodules.EthernetEP))
            {
                mf.Tls.ShowHelp("New Subnet address sent.", "Subnet", 10000);
            }
            else
            {
                mf.Tls.ShowHelp("New Subnet address not sent.", "Subnet", 10000);
            }
        }

        private void btnSendSubnet_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Send Ethernet subnet address to modules.";

            mf.Tls.ShowHelp(Message, "Subnet");
            hlpevent.Handled = true;
        }

        private void btnSetEthernet_Click(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void btnSetIP_Click(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void frmWifi_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                mf.Tls.SaveFormData(this);
            }
        }

        private void frmWifi_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            SetDayMode();
            UpdateForm();
        }

        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            mf.Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }

        private void LoadCombo()
        {
            // https://stackoverflow.com/questions/6803073/get-local-ip-address
            try
            {
                cbNetworks.Items.Clear();
                foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (item.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 && item.OperationalStatus == OperationalStatus.Up)
                    {
                        foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                cbNetworks.Items.Add(ip.Address.ToString());
                            }
                        }
                    }
                }
                cbNetworks.SelectedIndex = cbNetworks.FindString(SubAddress(mf.UDPmodules.WifiEP));

                cbEthernet.Items.Clear();
                foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (item.NetworkInterfaceType == NetworkInterfaceType.Ethernet && item.OperationalStatus == OperationalStatus.Up)
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
                cbEthernet.SelectedIndex = cbEthernet.FindString(SubAddress(mf.UDPmodules.EthernetEP));
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmWifi/LoadCombo " + ex.Message);
            }
        }

        private void SetButtons(bool Edited)
        {
            if (!Initializing)
            {
                if (!Initializing)
                {
                    if (Edited)
                    {
                        btnCancel.Enabled = true;
                        btnClose.Image = Properties.Resources.Save;
                        btnRescan.Enabled = false;
                        cbNetworks.Enabled = false;
                    }
                    else
                    {
                        btnCancel.Enabled = false;
                        btnClose.Image = Properties.Resources.OK;
                        btnRescan.Enabled = true;
                        cbNetworks.Enabled = true;
                    }

                    FormEdited = Edited;
                }
            }
        }

        private void SetDayMode()
        {
            if (Properties.Settings.Default.IsDay)
            {
                this.BackColor = Properties.Settings.Default.DayColour;

                foreach (Control c in this.Controls)
                {
                    c.ForeColor = Color.Black;
                }
            }
            else
            {
                this.BackColor = Properties.Settings.Default.NightColour;

                foreach (Control c in this.Controls)
                {
                    c.ForeColor = Color.White;
                }
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

        private void UpdateForm()
        {
            Initializing = true;

            lbEthernet.Text = "Selected subnet:  " + mf.UDPmodules.EthernetEP;
            lbIP.Text = "Selected subnet:  " + mf.UDPmodules.WifiEP;
            LoadCombo();

            Initializing = false;
        }
    }
}