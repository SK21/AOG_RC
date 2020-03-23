using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RateController
{
    public partial class FormRateControl : Form
    {
        public CRateCals RC;
        public clsTools Tls;
        public SerialComm SER;
        public UDPComm UDPnetwork;
        public UDPComm UDPlocal;

        public string NetworkIP;
        public string LocalIP;

        public FormRateControl()
        {
            InitializeComponent();
            RC = new CRateCals(this);
            Tls = new clsTools(this);
            SER = new SerialComm(this);
            UDPnetwork = new UDPComm(this, Properties.Settings.Default.DestinationIP, 8000, 2188);
            LocalIP = "127.100.0.0";
            UDPlocal = new UDPComm(this, LocalIP, 8888, 2388);
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

            // serial
            SER.RCportName = Properties.Settings.Default.RCportName;
            SER.RCportBaud = Properties.Settings.Default.RCportBaud;
            if (Properties.Settings.Default.RCportSuccessful) SER.OpenRCport();
            SetDayMode();
        }

        public void UpdateStatus()
        {
            lblUnits.Text = RC.Units();
            SetRate.Text = RC.RateSet.ToString("N1");
            CurRate.Text = RC.CurrentRate();
            AreaDone.Text = RC.CurrentCoverage();
            TankRemain.Text = RC.CurrentTankRemaining();
            VolApplied.Text = RC.CurrentApplied();

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

            if(RC.AogConnected)
            {
                lbAogConnected.Text = "AOG Connected";
                lbAogConnected.BackColor = Color.LightGreen;
            }
            else
            {
                lbAogConnected.Text = "AOG Disconnected";
                lbAogConnected.BackColor = Color.Red;
            }
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
            Properties.Settings.Default.Save();
            RC.SaveSettings();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form frmRateSettings = new FormRateSettings(this);
            frmRateSettings.ShowDialog();
            UpdateStatus();
            SetDayMode();
        }

        private void bntOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            RC.Update();
            UpdateStatus();
        }

        void SetDayMode()
        {
            if(Properties.Settings.Default.IsDay)
            {
                this.BackColor = Properties.Settings.Default.DayColour;
                foreach(Control c in this.Controls)
                {
                    c.ForeColor = Color.Black;
                }
            }
            else
            {
                this.BackColor = Properties.Settings.Default.NightColour;
                foreach(Control c in this.Controls)
                {
                    c.ForeColor = Color.White;
                }
            }
        }
    }
}
