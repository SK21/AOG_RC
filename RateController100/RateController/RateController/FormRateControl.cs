using System;
using System.Drawing;
using System.IO;
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
        //private bool ShowAverageRate = false;
        private DateTime LastSave;
        private int RateType = 0;   // 0 current rate, 1 instantaneous rate, 2 overall rate

        public FormRateControl()
        {
            InitializeComponent();
            Tls = new clsTools(this);
            RC = new CRateCals(this);
            SER = new SerialComm(this);

            LocalIP = "127.100.0.0";
            UDPlocal = new UDPComm(this, LocalIP, 8120, 2388);

            Tls.SaveProperty("DestinationIP", BroadcastIP(UDPlocal.GetLocalIP()));

            UDPnetwork = new UDPComm(this, Tls.LoadProperty("DestinationIP"), 8000, 2188);
        }

        public void StartSerial()
        {
            SER.RCportName = Tls.LoadProperty("RCportName");

            int temp;
            if (int.TryParse(Tls.LoadProperty("RCportBaud"), out temp))
            {
                SER.RCportBaud = temp;
            }
            else
            {
                SER.RCportBaud = 38400;
            }

            bool tempB;
            bool.TryParse(Tls.LoadProperty("RCportSuccessful"), out tempB);
            if (tempB) SER.OpenRCport();
        }

        public void UpdateStatus()
        {
            lblUnits.Text = RC.Units();
            SetRate.Text = RC.RateSet.ToString("N1");
            AreaDone.Text = RC.CurrentCoverage();
            TankRemain.Text = RC.CurrentTankRemaining();
            VolApplied.Text = RC.CurrentApplied();
            lbCoverage.Text = RC.CoverageDescription();

            switch (RateType)
            {
                case 1:
                    lbRate.Text = "Instant Rate";
                    lbRateAmount.Text = RC.CurrentRate();
                    break;
                case 2:
                    lbRate.Text = "Overall Rate";
                    lbRateAmount.Text = RC.AverageRate();
                    break;
                default:
                    lbRate.Text = "Current Rate";
                    lbRateAmount.Text = RC.SmoothRate();
                    break;
            }

            //if (ShowAverageRate)
            //{
            //    lbRate.Text = "Overall Rate";
            //    lbRateAmount.Text = RC.AverageRate();
            //}
            //else
            //{
            //    lbRate.Text = "Current Rate";
            //    lbRateAmount.Text = RC.CurrentRate();
            //}

            if (RC.ArduinoConnected)
            {
                //lbArduinoConnected.Text = "Controller Connected";
                lbArduinoConnected.BackColor = Color.LightGreen;
            }
            else
            {
                //lbArduinoConnected.Text = "Controller Disconnected";
                lbArduinoConnected.BackColor = Color.Red;
            }

            if (RC.AogConnected)
            {
                //lbAogConnected.Text = "AOG Connected";
                lbAogConnected.BackColor = Color.LightGreen;
            }
            else
            {
                //lbAogConnected.Text = "AOG Disconnected";
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
                Tls.SaveFormData(this);
            }
            RC.SaveSettings();
        }

        private void lbRate_Click(object sender, EventArgs e)
        {
            RateType++;
            if (RateType > 2) RateType = 0;
            UpdateStatus();
        }

        private void RateControl_Load(object sender, EventArgs e)
        {
            Tls.LoadFormData(this);

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
            LoadSettings();
        }

        public void LoadSettings()
        {
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
            string Title = "RC [" + Path.GetFileNameWithoutExtension(Properties.Settings.Default.FileName) + "]";

            SimType Mode;
            Enum.TryParse(Tls.LoadProperty("cSimulationType"), true, out Mode);
            switch (Mode)
            {
                case SimType.VirtualNano:
                    this.Text = Title +" (V)";
                    break;

                case SimType.RealNano:
                    this.Text = Title +" (R)";
                    break;

                default:
                    this.Text = Title;
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

        private void lbArduinoConnected_Click(object sender, EventArgs e)
        {

        }
    }
}