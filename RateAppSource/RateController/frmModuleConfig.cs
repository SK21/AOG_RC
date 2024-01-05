using AgOpenGPS;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmModuleConfig : Form
    {
        public FormStart mf;
        private bool FormEdited;
        private bool Initializing;

        public frmModuleConfig(FormStart Main)
        {
            InitializeComponent();
            mf = Main;
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
                // exit
                this.Close();
            }
            else
            {
                // save
                // IP
                mf.UDPmodules.NetworkEP = cbEthernet.Text;

                Save();
                SetButtons(false);
                UpdateForm();
            }
        }

        private void btnLoadDefaults_Click(object sender, EventArgs e)
        {
            tbModuleID.Text = "0";
            tbSensorCount.Text = "1";
            ckRelayOn.Checked = false;
            ckFlowOn.Checked = false;
            tbWifiPort.Text = "1";

            tbFlow1.Text = "28";
            tbDir1.Text = "37";
            tbPWM1.Text = "36";

            tbFlow2.Text = "29";
            tbDir2.Text = "14";
            tbPWM2.Text = "15";

            cbRelayControl.SelectedIndex = 5;

            tbRelay1.Text = "8";
            tbRelay2.Text = "9";
            tbRelay3.Text = "10";
            tbRelay4.Text = "11";
            tbRelay5.Text = "12";
            tbRelay6.Text = "25";
            tbRelay7.Text = "26";
            tbRelay8.Text = "27";

            tbRelay9.Text = "0";
            tbRelay10.Text = "0";
            tbRelay11.Text = "0";
            tbRelay12.Text = "0";
            tbRelay13.Text = "0";
            tbRelay14.Text = "0";
            tbRelay15.Text = "0";
            tbRelay16.Text = "0";
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
            PGN32503 SetSubnet = new PGN32503(mf);
            if (SetSubnet.Send(mf.UDPmodules.NetworkEP))
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
            string Message = "Send subnet address to modules.";

            mf.Tls.ShowHelp(Message, "Subnet");
            hlpevent.Handled = true;
        }

        private void btnSendToModule_Click(object sender, EventArgs e)
        {
            try
            {
                mf.ModuleConfig.Send();
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp("frmModuleConfig/btnSendToModule  " + ex.Message, "Help", 20000, true, true);
            }
        }

        private void frmModuleConfig_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                mf.Tls.SaveFormData(this);
            }
        }

        private void frmModuleConfig_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            SetDayMode();
            UpdateForm();
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

            if (byte.TryParse(tbFlow1.Text, out val))
            {
                mf.ModuleConfig.Sensor0Flow = val;
            }
            if (byte.TryParse(tbFlow2.Text, out val))
            {
                mf.ModuleConfig.Sensor1Flow = val;
            }
            if (byte.TryParse(tbDir1.Text, out val))
            {
                mf.ModuleConfig.Sensor0Dir = val;
            }
            if (byte.TryParse(tbDir2.Text, out val))
            {
                mf.ModuleConfig.Sensor1Dir = val;
            }
            if (byte.TryParse(tbPWM1.Text, out val))
            {
                mf.ModuleConfig.Sensor0PWM = val;
            }
            if (byte.TryParse(tbPWM2.Text, out val))
            {
                mf.ModuleConfig.Sensor1PWM = val;
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

            mf.ModuleConfig.RelayPins(Pins);
            mf.ModuleConfig.Save();
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
                        using (var form = new FormNumeric(0, 50, temp))
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

            tbModuleID.Text = data[2].ToString();
            tbSensorCount.Text = data[3].ToString();
            ckRelayOn.Checked = ((data[4] & 1) == 1);
            ckFlowOn.Checked = ((data[4] & 2) == 2);
            cbRelayControl.SelectedIndex = data[5];
            tbWifiPort.Text = data[6].ToString();
            tbFlow1.Text = data[7].ToString();
            tbDir1.Text = data[8].ToString();
            tbPWM1.Text = data[9].ToString();
            tbFlow2.Text = data[10].ToString();
            tbDir2.Text = data[11].ToString();
            tbPWM2.Text = data[12].ToString();

            tbRelay1.Text = data[13].ToString();
            tbRelay2.Text = data[14].ToString();
            tbRelay3.Text = data[15].ToString();
            tbRelay4.Text = data[16].ToString();

            tbRelay5.Text = data[17].ToString();
            tbRelay6.Text = data[18].ToString();
            tbRelay7.Text = data[19].ToString();
            tbRelay8.Text = data[20].ToString();

            tbRelay9.Text = data[21].ToString();
            tbRelay10.Text = data[22].ToString();
            tbRelay11.Text = data[23].ToString();
            tbRelay12.Text = data[24].ToString();

            tbRelay13.Text = data[25].ToString();
            tbRelay14.Text = data[26].ToString();
            tbRelay15.Text = data[27].ToString();
            tbRelay16.Text = data[28].ToString();

            LoadCombo();
            lbModuleIP.Text = mf.UDPmodules.SubNet;

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

        private void btnLoadDefaults_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            Form fs = Application.OpenForms["frmPins"];

            if (fs == null)
            {
                Form frm = new frmPins(mf);
                frm.Show();
            }
            else
            {
                fs.Focus();
            }

            string Message = "Load defaults";
            mf.Tls.ShowHelp(Message, "Defaults");
            hlpevent.Handled = true;
        }

        private void btnSendToModule_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Send settings to module. Only have 1 module connected when sending.";
            mf.Tls.ShowHelp(Message, "Send Config");
            hlpevent.Handled = true;
        }

        private void cbEthernet_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }
    }
}