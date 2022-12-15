using System;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmWifi : Form
    {
        private bool Initializing;
        private FormStart mf;

        public frmWifi(FormStart CalledFrom)
        {
            InitializeComponent();
            mf = CalledFrom;

            #region // language

            btnCancel.Text = Lang.lgCancel;
            btnClose.Text = Lang.lgClose;
            btnRescan.Text = Lang.lgRescan;

            lbHeading.Text = Lang.lgWirelessNetwork;
            grpHotSpot.Text = Lang.lgHotspot;
            lbPassword.Text = Lang.lgPassword;

            btnStop.Text = Lang.lgStop;
            btnStart.Text = Lang.lgStart;
            btnRescan.Text = Lang.lgRescan;

            #endregion // language
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Button ButtonClicked = (Button)sender;
            if (ButtonClicked.Text == Lang.lgClose)
            {
                this.Close();
            }
            else
            {
                // save settings
                mf.Tls.SaveProperty("WifiSSID", tbSSID.Text);
                mf.Tls.SaveProperty("WifiPassword", tbPassword.Text);

                // IP
                mf.UDPmodules.SetWifiEP(cbNetworks.Text);

                SetButtons(false);
                UpdateForm();
            }
        }

        private void btnRescan_Click(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void btnSetIP_Click(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                mf.Tls.StartWifi();
                UpdateForm();
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp("Error starting wifi connection. " + ex.Message, "Wifi", 15000, true);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                mf.Tls.StopWifi();
                UpdateForm();
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp("Error closing wifi connection. " + ex.Message, "Wifi", 15000, true);
            }
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
            try
            {
                cbNetworks.Items.Clear();

                // https://stackoverflow.com/questions/6803073/get-local-ip-address

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
                cbNetworks.SelectedIndex = cbNetworks.FindStringExact(mf.UDPmodules.WifiIP());
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
                if (Edited)
                {
                    btnCancel.Enabled = true;
                    this.btnClose.Text = Lang.lgSave;

                    btnStop.Enabled = false;
                    btnStart.Enabled = false;
                    btnRescan.Enabled = false;
                    btnSetIP.Enabled = false;
                    cbNetworks.Enabled = false;
                    tbSSID.Enabled = false;
                    tbPassword.Enabled = false;
                }
                else
                {
                    btnCancel.Enabled = false;
                    this.btnClose.Text = Lang.lgClose;

                    btnStop.Enabled = true;
                    btnStart.Enabled = true;
                    btnRescan.Enabled = true;
                    btnSetIP.Enabled = true;
                    cbNetworks.Enabled = true;
                    tbSSID.Enabled = true;
                    tbPassword.Enabled = true;
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

        private void tbPassword_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbSSID_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void UpdateForm()
        {
            Initializing = true;
            tbSSID.Text = mf.Tls.LoadProperty("WifiSSID");
            if (tbSSID.Text == "") tbSSID.Text = "tractor";

            tbPassword.Text = mf.Tls.LoadProperty("WifiPassword");
            if (tbPassword.Text == "") tbPassword.Text = "111222333";

            // save in case credentials were blank
            mf.Tls.SaveProperty("WifiSSID", tbSSID.Text);
            mf.Tls.SaveProperty("WifiPassword", tbPassword.Text);

            lbIP.Text = "Selected IP:  " + mf.UDPmodules.WifiIP();
            LoadCombo();

            Initializing = false;
        }
    }
}