using System;
using System.Drawing;
using System.IO;
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
        public string PortName;
        public clsTools Tls;
        public UDPComm UDPmodulesConfig;
        private int PortID = 1;

        public frmMain()
        {
            InitializeComponent();
            Tls = new clsTools(this);
            CommPort = new SerialComm(this, PortID);
            UDPmodulesConfig = new UDPComm(this, 29900, 28800, 1482);     // pcb config
            CommPort.ModuleConnected += CommPort_ModuleConnected;
        }

        public bool OpenComm()
        {
            CommPort.SCportName = cboPort1.Text;
            CommPort.SCportBaud = 38400;
            CommPort.OpenSCport();
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
                OpenComm();
            }
            else
            {
                CommPort.CloseSCport();
            }
            UpdateForm();
        }

        private void btnRescan_Click(object sender, EventArgs e)
        {
            cboPort1.Items.Clear();
            foreach (String s in System.IO.Ports.SerialPort.GetPortNames())
            {
                cboPort1.Items.Add(s);
            }
            UpdateForm();
        }

        private void cboPort1_SelectedIndexChanged(object sender, EventArgs e)
        {
            PortName = cboPort1.SelectedItem.ToString();
        }

        private void CommPort_ModuleConnected(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void firmwareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form Sec = new frmFirmware(this);
            Sec.ShowDialog();
        }

        private void firmwareToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form tmp = new frmNanoFirmware(this);
            tmp.ShowDialog();
        }

        private void firmwareToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Form tmp = new frmSwitchboxFirmware(this);
            tmp.ShowDialog();
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

        private void LoadSettings()
        {
            try
            {
                this.Text = "PCBsetup [" + Path.GetFileNameWithoutExtension(Properties.Settings.Default.FileName) + "]";

                cboPort1.Items.Clear();
                foreach (String s in System.IO.Ports.SerialPort.GetPortNames())
                {
                    cboPort1.Items.Add(s);
                }

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
                    int i = cboPort1.FindStringExact(tmp);
                    if (i != -1)
                    {
                        cboPort1.SelectedIndex = i;
                    }
                }
            }
            catch (Exception ex)
            {
                Tls.ShowHelp("frmMain/LoadSettings " + ex.Message, this.Text, 3000, true);
            }
        }

        private void ModuleIndicator_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Indicates module connected.";

            Tls.ShowHelp(Message, this.Text, 3000);
            hlpevent.Handled = true;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = Tls.SettingsDir();
            saveFileDialog1.Title = "New File";
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
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Tls.PropertiesFile = openFileDialog1.FileName;
                LoadSettings();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
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
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                    Tls.SaveFile(saveFileDialog1.FileName);
                    LoadSettings();
                }
            }
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

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form tmp = new frmPCBsettings(this);
            tmp.ShowDialog();
        }

        private void settingsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form tmp = new frmNanoSettings(this);
            tmp.ShowDialog();
        }

        private void settingsToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Form tmp = new frmSwitchboxSettings(this);
            tmp.ShowDialog();
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

            if (CommPort.IsSerialActive())
            {
                ModuleIndicator.Image = Properties.Resources.On;
            }
            else
            {
                ModuleIndicator.Image = Properties.Resources.Off;
            }
        }
    }
}