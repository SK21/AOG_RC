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
        public UDPComm UDP;

        public FormRateControl()
        {
            InitializeComponent();
            RC = new CRateCals(this);
            Tls = new clsTools(this);
            UDP = new UDPComm(this);
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

            UDP.StartUDPServer();
            if (!UDP.isUDPSendConnected)
            {
                Tls.TimedMessageBox("UDP failed to start.", "", 3000, true);
            }
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
            //DayNight();
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
    }
}
