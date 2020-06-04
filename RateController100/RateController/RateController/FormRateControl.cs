using System;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace RateController
{
    public partial class FormRateControl : Form
    {
        public string LocalIP;
        public string NetworkIP;
        public CRateCals RC;
        public SerialComm SER;
        public clsTools Tls;
        public UDPComm UDPlocal;
        public UDPComm UDPnetwork;
        private bool ShowAverageRate = false;
        private DateTime LastSave;

        public FormRateControl()
        {
            InitializeComponent();
            RC = new CRateCals(this);
            Tls = new clsTools(this);
            SER = new SerialComm(this);

            LocalIP = "127.100.0.0";
            UDPlocal = new UDPComm(this, LocalIP, 8120, 2388);

            Properties.Settings.Default.DestinationIP = BroadcastIP(UDPlocal.GetLocalIP());

            UDPnetwork = new UDPComm(this, Properties.Settings.Default.DestinationIP, 8000, 2188);
        }

        public void StartSerial()
        {
            SER.RCportName = Properties.Settings.Default.RCportName;
            SER.RCportBaud = Properties.Settings.Default.RCportBaud;
            if (Properties.Settings.Default.RCportSuccessful) SER.OpenRCport();
        }

        public void UpdateStatus()
        {
            lblUnits.Text = RC.Units();
            SetRate.Text = RC.RateSet.ToString("N1");
            AreaDone.Text = RC.CurrentCoverage();
            TankRemain.Text = RC.CurrentTankRemaining();
            VolApplied.Text = RC.CurrentApplied();
            lbCoverage.Text = RC.CoverageDescription();

            if (ShowAverageRate)
            {
                lbRate.Text = "Overall Rate";
                lbRateAmount.Text = RC.AverageRate();
            }
            else
            {
                lbRate.Text = "Current Rate";
                lbRateAmount.Text = RC.CurrentRate();
            }

            if (RC.ArduinoConnected)
            {
                lbArduinoConnected.Text = "Controller Connected";
                lbArduinoConnected.BackColor = Color.LightGreen;
            }
            else
            {
                lbArduinoConnected.Text = "Controller Disconnected";
                lbArduinoConnected.BackColor = Color.Red;
            }

            if (RC.AogConnected)
            {
                lbAogConnected.Text = "AOG Connected";
                lbAogConnected.BackColor = Color.LightGreen;
            }
            else
            {
                lbAogConnected.Text = "AOG Disconnected";
                lbAogConnected.BackColor = Color.Red;
            }

            SetTitle();
        }

        private void bntOK_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void button3_Click(object sender, EventArgs e)
        {
            Form frmRateSettings = new FormRateSettings(this);
            frmRateSettings.ShowDialog();
            UpdateStatus();
            SetDayMode();
        }

        private void FormRateControl_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                // save location and size if the state is normal
                Properties.Settings.Default.FormRClocation = this.Location;
            }
            else
            {
                // save the RestoreBounds if the form is minimized or maximized!
                Properties.Settings.Default.FormRClocation = this.RestoreBounds.Location;
            }

            // don't forget to save the settings
            RC.SaveSettings();
        }

        private void lbRate_Click(object sender, EventArgs e)
        {
            ShowAverageRate = !ShowAverageRate;
            UpdateStatus();
        }

        private void RateControl_Load(object sender, EventArgs e)
        {
            this.Location = Properties.Settings.Default.FormRClocation;
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
            SetTitle();
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

        private void SetTitle()
        {
            SimType Mode = (SimType)Properties.Settings.Default.SimulateType;
            switch (Mode)
            {
                case SimType.VirtualNano:
                    this.Text = "Rate Controller (V)";
                    break;

                case SimType.RealNano:
                    this.Text = "Rate Controller (R)";
                    break;

                default:
                    this.Text = "Rate Controller";
                    break;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            RC.Update();
            UpdateStatus();

            if ((DateTime.Now - LastSave).TotalSeconds > 60)
            {
                RC.SaveSettings();
                LastSave = DateTime.Now;
            }
        }
    }
}