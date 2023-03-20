using System;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Windows.Forms;

namespace PCBsetup.Forms
{
    public enum UploadResult
    {
        Completed,
        Failed,
        Sync_Error,
        Time_Out
    }

    public partial class frmMain : Form
    {
        public SerialComm CommPort;
        public clsTools Tls;
        public UDPComm UDPmodulesConfig;
        private byte cModule = 0;
        private int PortID = 1;
        private string cSelectedPortName;

        public frmMain()
        {
            InitializeComponent();
            Tls = new clsTools(this);
            CommPort = new SerialComm(this, PortID);
            UDPmodulesConfig = new UDPComm(this, 29900, 28800, 1482);     // pcb config
            CommPort.ModuleConnected += CommPort_ModuleConnected;
        }

        private string TrimPortName(string portName)
        {
            string tmp = portName;
            int End = tmp.IndexOf(" - ");
            if (End > 0) tmp = tmp.Substring(0, End);
            return tmp;
        }

        public string SelectedPortName()
        {
            return cSelectedPortName;
        }

        public bool OpenComm()
        {
            CommPort.SCportName = TrimPortName(cboPort1.Text);
            CommPort.SCportBaud = 38400;
            CommPort.Open();
            return CommPort.IsOpen();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form tmp = new frmAbout(this);
            tmp.ShowDialog();
        }

        private void btnConnect1_Click_1(object sender, EventArgs e)
        {
            if (btnConnect1.Text == Languages.Lang.lgConnect)
            {
                if (!OpenComm()) Tls.ShowHelp("Could not open comm port.", this.Text, 3000);
            }
            else
            {
                CommPort.Close();
            }
            UpdateForm();
        }

        private void btnFirmware_Click(object sender, EventArgs e)
        {
            switch (cModule)
            {
                case 0:
                    // Teensy AutoSteer
                    Form tmp = new frmFirmware(this, 0);
                    tmp.ShowDialog();
                    break;

                case 1:
                    // Teensy Rate
                    Form tmp1 = new frmFirmware(this, 1);
                    tmp1.ShowDialog();
                    break;

                case 2:
                    // Nano Rate
                    Form tmp2 = new frmNanoFirmware(this);
                    tmp2.ShowDialog();
                    break;

                case 3:
                    // Nano SwitchBox
                    Form tmp3 = new frmSwitchboxFirmware(this);
                    tmp3.ShowDialog();
                    break;

                case 4:
                    // wifi rate
                    Form tmp4 = new frmD1rate(this);
                    CommPort.Close();   // needs to be closed for esptool to connect
                    tmp4.ShowDialog();
                    break;
            }
        }

        private void btnRescan_Click(object sender, EventArgs e)
        {
            LoadPortsCombo();
            UpdateForm();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            switch (cModule)
            {
                case 0:
                    // Teensy AutoSteer
                    Form tmp = new frmPCBsettings(this);
                    tmp.ShowDialog();
                    break;

                case 1:
                    // Teensy Rate
                    Form tmp1 = new frmTRsettings(this);
                    tmp1.ShowDialog();
                    break;

                case 2:
                    // Nano Rate
                    Form tmp2 = new frmNanoSettings(this);
                    tmp2.ShowDialog();
                    break;

                case 3:
                    // Nano SwitchBox
                    Form tmp3 = new frmSwitchboxSettings(this);
                    tmp3.ShowDialog();
                    break;
            }
        }

        private void cbModule_SelectedIndexChanged(object sender, EventArgs e)
        {
            Tls.SaveProperty("Module", cbModule.SelectedIndex.ToString());
            cModule = (byte)cbModule.SelectedIndex;
        }

        private void CommPort_ModuleConnected(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmMain_Activated(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                Tls.SaveFormData(this);
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            if (Tls.PrevInstance())
            {
                Tls.ShowHelp(Languages.Lang.lgAlreadyRunning, "Help", 3000);
                this.Close();
            }

            Tls.LoadFormData(this);
            LoadSettings();
            SetDayMode();

            // ethernet
            UDPmodulesConfig.StartUDPServer();
            if (!UDPmodulesConfig.isUDPSendConnected)
            {
                Tls.ShowHelp("UDPconfig failed to start.", "", 3000, true);
            }
        }

        private void GroupBoxPaint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }

        private void LoadPortsCombo()
        {
            //https://stackoverflow.com/questions/2837985/getting-serial-port-information

            cboPort1.Items.Clear();
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption like '%(COM%'"))
            {
                var portnames = SerialPort.GetPortNames();
                var ports = searcher.Get().Cast<ManagementBaseObject>().ToList().Select(p => p["Caption"].ToString());

                var portList = portnames.Select(n => n + " - " + ports.FirstOrDefault(s => s.Contains(n))).ToList();

                foreach (string s in portList)
                {
                    Console.WriteLine(s);
                    cboPort1.Items.Add(s);
                }
            }
        }

        private void LoadSettings()
        {
            try
            {
                this.Text = "PCBsetup [" + Path.GetFileNameWithoutExtension(Properties.Settings.Default.FileName) + "]";

                LoadPortsCombo();

                // start comm port
                string ID = "_" + PortID.ToString();
                string tmp = Tls.LoadProperty("SCportName" + ID);
                if (tmp == "")
                {
                    // select first port available
                    if (cboPort1.Items.Count > 0) cboPort1.SelectedItem = cboPort1.Items[0];
                }
                else
                {
                    // select previous port
                    int i = cboPort1.FindString(tmp);
                    if (i != -1)
                    {
                        cboPort1.SelectedIndex = i;
                    }
                }

                // module
                byte.TryParse(Tls.LoadProperty("Module"), out cModule);
                cbModule.SelectedIndex = cModule;
            }
            catch (Exception ex)
            {
                Tls.ShowHelp("frmMain/LoadSettings " + ex.Message, this.Text, 3000, true);
            }
        }

        private void ModuleIndicator_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Indicates module connected. The ESP8266 does not need to show connection.";

            Tls.ShowHelp(Message, this.Text, 3000);
            hlpevent.Handled = true;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = Tls.SettingsDir();
            saveFileDialog1.Title = "New File";
            saveFileDialog1.Filter = "Config Files|*.con";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                    Tls.NewFile(saveFileDialog1.FileName);
                    LoadSettings();
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Tls.SettingsDir();
            openFileDialog1.Filter = "Config Files|*.con";
            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Tls.PropertiesFile = openFileDialog1.FileName;
                LoadSettings();
            }
        }

        private void PortIndicator1_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Indicates serial port connected.";

            Tls.ShowHelp(Message, this.Text, 3000);
            hlpevent.Handled = true;
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = Tls.SettingsDir();
            saveFileDialog1.Title = "Save As";
            saveFileDialog1.Filter = "Config Files|*.con";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                    Tls.SaveFile(saveFileDialog1.FileName);
                    LoadSettings();
                }
            }
        }

        private void SerialMonitorItem_Click(object sender, EventArgs e)
        {
            Form tmp = new frmMonitor(this);
            tmp.ShowDialog();
            //tmp.Show();
        }

        private void SetDayMode()
        {
            if (Properties.Settings.Default.IsDay)
            {
                this.BackColor = Properties.Settings.Default.DayColour;
                PortIndicator1.BackColor = Properties.Settings.Default.DayColour;
                ModuleIndicator.BackColor = Properties.Settings.Default.DayColour;

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

        private void UpdateForm()
        {
            if (CommPort.IsOpen())
            {
                PortIndicator1.Image = Properties.Resources.On;
                btnConnect1.Text = Languages.Lang.lgDisconnect;
            }
            else
            {
                PortIndicator1.Image = Properties.Resources.Off;
                btnConnect1.Text = Languages.Lang.lgConnect;
            }

            if (CommPort.IsReceiveActive())
            {
                ModuleIndicator.Image = Properties.Resources.On;
            }
            else
            {
                ModuleIndicator.Image = Properties.Resources.Off;
            }
        }

        private void cboPort1_SelectedIndexChanged(object sender, EventArgs e)
        {
            cSelectedPortName = TrimPortName(cboPort1.SelectedItem.ToString());
        }

        private void cboPort1_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "List of connected serial ports.";

            Tls.ShowHelp(Message, this.Text);
            hlpevent.Handled = true;
        }
    }
}