using RateController.Language;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RateController.Menu
{
    public partial class frmMenuNetwork : Form
    {
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
            UpdateForm();
            SetButtons(false);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                mf.UDPmodules.NetworkEP = cbEthernet.Text;
                SetButtons(false);
                UpdateForm();
                MainMenu.HighlightUpdateButton();
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmMenuNetwork/btnOk_Click: " + ex.Message);
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
            UpdateForm();
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
            lbSubnet.Text = Lang.lgSelectedSubnet;
            lbIP.Text = Lang.lgConfigIP;
        }

        private void UpdateForm()
        {
            Initializing = true;
            LoadCombo();
            lbModuleIP.Text = mf.UDPmodules.SubNet;
            Initializing = false;
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

        private void cbEthernet_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtons(true);
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

        private void btnRescan_Click(object sender, EventArgs e)
        {
            UpdateForm();
        }
    }
}