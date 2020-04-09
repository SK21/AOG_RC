using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace IMUapp
{
    public partial class FormMain : Form
    {
        public string LocalIP;
        public string NetworkIP;
        public SerialComm SER;
        public clsTools Tls;
        public UDPComm UDPlocal;
        public UDPComm UDPnetwork;

        public PGN32750 IMUdata;
        private int NetReceive = 8100;

        public FormMain()
        {
            InitializeComponent();
            Tls = new clsTools(this);
            SER = new SerialComm(this);

            LocalIP = "127.110.0.0";
            UDPlocal = new UDPComm(this, LocalIP, 8500, 1482);

            Properties.Settings.Default.DestinationIP = BroadcastIP(UDPlocal.GetLocalIP());

            UDPnetwork = new UDPComm(this, Properties.Settings.Default.DestinationIP, NetReceive, 1480);

            IMUdata = new PGN32750(this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Location = Properties.Settings.Default.FormMainLocation;
            Tls.IsOnScreen(this, true);

            if (Tls.PrevInstance())
            {
                Tls.TimedMessageBox("Already Running!");
                this.Close();
            }

            // UDP
            NetworkIP = UDPnetwork.StartUDPServer();
            if (!UDPnetwork.isUDPSendConnected)
            {
                Tls.TimedMessageBox("UDPnetwork failed to start.", "", 3000, true);
            }

            UDPlocal.StartUDPServer();
            if (!UDPlocal.isUDPSendConnected)
            {
                Tls.TimedMessageBox("UDPlocal failed to start.", "", 3000, true);
            }

            StartSerial();
            SetDayMode();
            LoadRCbox();
            UpdateStatus();

            lbNetworkIP.Text = NetworkIP;
            lbLocalIP.Text = LocalIP;
            lbVersion.Text = Tls.VersionDate();
            lbNetReceive.Text = NetReceive.ToString();
            lbDestinationIP.Text = Properties.Settings.Default.DestinationIP;

            IMUdata.UsePitch = Properties.Settings.Default.UsePitch;
            rbPitch.Checked = Properties.Settings.Default.UsePitch;
            ckInvert.Checked = Properties.Settings.Default.Invert;
        }

        public void StartSerial()
        {
            SER.RCportName = Properties.Settings.Default.RCportName;
            SER.RCportBaud = Properties.Settings.Default.RCportBaud;
            if (Properties.Settings.Default.RCportSuccessful) SER.OpenRCport();
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

                this.BackColor = Properties.Settings.Default.DayColour;
                for (int i = 0; i < 2; i++)
                {
                    tabControl1.TabPages[i].BackColor = Properties.Settings.Default.DayColour;
                }
            }
            else
            {
                this.BackColor = Properties.Settings.Default.NightColour;
                foreach (Control c in this.Controls)
                {
                    c.ForeColor = Color.White;
                }

                this.BackColor = Properties.Settings.Default.NightColour;
                for (int i = 0; i < 2; i++)
                {
                    tabControl1.TabPages[i].BackColor = Properties.Settings.Default.DayColour;
                }
            }
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                // save location and size if the state is normal
                Properties.Settings.Default.FormMainLocation = this.Location;
            }
            else
            {
                // save the RestoreBounds if the form is minimized or maximized!
                Properties.Settings.Default.FormMainLocation = this.RestoreBounds.Location;
            }

            Properties.Settings.Default.Save();
        }

        private void cboxArdPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            SER.RCportName = cboxArdPort.Text;
        }

        private void cboxBaud_SelectedIndexChanged(object sender, EventArgs e)
        {
            SER.RCport.BaudRate = Convert.ToInt32(cboxBaud.Text);
            SER.RCportBaud = Convert.ToInt32(cboxBaud.Text);
        }

        private void btnRescan_Click(object sender, EventArgs e)
        {
            LoadRCbox();
            UpdateStatus();
        }

        private void LoadRCbox()
        {
            cboxArdPort.Items.Clear();
            foreach (String s in System.IO.Ports.SerialPort.GetPortNames()) { cboxArdPort.Items.Add(s); }
        }

        private void btnCloseSerialArduino_Click(object sender, EventArgs e)
        {
            SER.CloseRCport();
            UpdateStatus();
        }

        private void btnOpenSerialArduino_Click(object sender, EventArgs e)
        {
            SER.OpenRCport();
            UpdateStatus();
        }

        private void btnDay_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.IsDay)
            {
                Properties.Settings.Default.IsDay = false;
            }
            else
            {
                Properties.Settings.Default.IsDay = true;
            }
            SetDayMode();
        }

        private void UpdateStatus()
        {
            cboxArdPort.SelectedIndex = cboxArdPort.FindStringExact(SER.RCportName);
            cboxBaud.SelectedIndex = cboxBaud.FindStringExact(SER.RCportBaud.ToString());

            if (SER.RCport.IsOpen)
            {
                cboxBaud.Enabled = false;
                cboxArdPort.Enabled = false;
                btnCloseSerialArduino.Enabled = true;
                btnOpenSerialArduino.Enabled = false;
                lbArduinoConnected.Text = SER.RCportName + " Connected";
                lbArduinoConnected.BackColor = Color.LightGreen;
                this.Text = "IMU App +";
            }
            else
            {
                cboxBaud.Enabled = true;
                cboxArdPort.Enabled = true;
                btnCloseSerialArduino.Enabled = false;
                btnOpenSerialArduino.Enabled = true;
                lbArduinoConnected.Text = SER.RCportName + " Disconnected";
                lbArduinoConnected.BackColor = Color.Red;
                this.Text = "IMU App";
            }
        }

        private void bntOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void UpdateDisplay(string Message, bool AddTime = false)
        {
            if (tbDisplay.Text.Length > 20000) tbDisplay.Text = "";

            if (AddTime) tbDisplay.Text += DateTime.Now.ToShortTimeString() + " - ";
            tbDisplay.Text +=  Message + "\r\n";

            tbDisplay.SelectionStart = tbDisplay.Text.Length;
            tbDisplay.ScrollToCaret();
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            if(WindowState==FormWindowState.Minimized)
            {
                ckBoxViewData.Checked = false;
            }
        }

        private string BroadcastIP(string IP)
        {
            string Result = "";
            string[] data = IP.Split('.');
            if (data.Length == 4)
            {
                Result = data[0] + "." + data[1] + "." + data[2] + ".255";
            }

            if (IPAddress.TryParse(Result, out IPAddress Tmp))
            {
                return Result;
            }
            else
            {
                return "192.168.1.255";
            }
        }

        private void GroupBoxPaint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (ckBoxViewData.Checked)
            {
                IMUdata.UpdateEnabled = true;
                UpdateDisplay("Viewing IMU data ...", true);
            }
            else
            {
                IMUdata.UpdateEnabled = false;
                UpdateDisplay("Stopped viewing IMU data.");
            }
        }

        private void ckInvert_CheckedChanged(object sender, EventArgs e)
        {
            IMUdata.Invert = ckInvert.Checked;
            Properties.Settings.Default.Invert = ckInvert.Checked;
            Properties.Settings.Default.Save();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            IMUdata.UsePitch = rbPitch.Checked;
            Properties.Settings.Default.UsePitch = rbPitch.Checked;
            Properties.Settings.Default.Save();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            IMUdata.UsePitch = rbPitch.Checked;
            Properties.Settings.Default.UsePitch = rbPitch.Checked;
            Properties.Settings.Default.Save();
        }
    }
}
