using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace RateController
{
    public partial class FormRateControl : Form
    {
        public CRateCals RC;
        public clsTools Tls;

        public UDPComm UDPLoopBack;
        public UDPComm UDPnetwork;
        private DateTime LastSave;

        private int RateType = 0;   // 0 current rate, 1 instantaneous rate, 2 overall rate
        public string LoopBackIP = "127.0.0.1";
        public string LoopBackBroadcastIP = "127.0.0.255";

        public SerialComm[] SER = new SerialComm[4];

        public FormRateControl()
        {
            InitializeComponent();
            #region // language

            lbRate.Text = Lang.lgCurrentRate;
            label1.Text = Lang.lgTargetRate;
            lbCoverage.Text = Lang.lgCoverage;
            label2.Text = Lang.lgQuantityApplied;
            button3.Text = Lang.lgSettings;
            bntOK.Text = Lang.lgClose;
            label34.Text = Lang.lgTank_Remaining;

            #endregion // language

            Tls = new clsTools(this);
            RC = new CRateCals(this);

            UDPLoopBack = new UDPComm(this, 8120, 9999, 1460, LoopBackBroadcastIP); // AOG
            UDPnetwork = new UDPComm(this, 8000, 9999, 1480, "", true);             // arduino

            for (int i=0;i<4;i++)
            {
                SER[i] = new SerialComm(this, i);
            }
        }

        public void StartSerial()
        {
            try
            {
                for (int i = 0; i < 4; i++)
                {
                    SER[i].RCportName = Tls.LoadProperty("RCportName" + i.ToString());

                    int tmp;
                    if (int.TryParse(Tls.LoadProperty("RCportBaud" + i.ToString()), out tmp))
                    {
                        SER[i].RCportBaud = tmp;
                    }
                    else
                    {
                        SER[i].RCportBaud = 38400;
                    }

                    bool tmp2;
                    bool.TryParse(Tls.LoadProperty("RCportSuccessful" + i.ToString()), out tmp2);
                    if (tmp2) SER[i].OpenRCport();
                }
            }
            catch (Exception ex)
            {
                Tls.WriteErrorLog("FormRateControl/StartSerial: " + ex.Message);
                Tls.TimedMessageBox(ex.Message);
            }
        }

        public void UpdateStatus()
        {
            lblUnits.Text = RC.Units();
            SetRate.Text = RC.RateSet.ToString("N1");
            AreaDone.Text = RC.CurrentCoverage();
            TankRemain.Text = RC.CurrentTankRemaining().ToString("N0");
            VolApplied.Text = RC.CurrentApplied();
            lbCoverage.Text = RC.CoverageDescription();

            switch (RateType)
            {
                case 1:
                    lbRate.Text = Lang.lgInstantRate;
                    lbRateAmount.Text = RC.CurrentRate();
                    break;
                case 2:
                    lbRate.Text = Lang.lgOverallRate;
                    lbRateAmount.Text = RC.AverageRate();
                    break;
                default:
                    lbRate.Text = Lang.lgCurrentRate;
                    lbRateAmount.Text = RC.SmoothRate();
                    break;
            }

            if (RC.ArduinoConnected)
            {
                lbArduinoConnected.BackColor = Color.LightGreen;
            }
            else
            {
                lbArduinoConnected.BackColor = Color.Red;
            }

            if (RC.AogConnected)
            {
                lbAogConnected.BackColor = Color.LightGreen;
            }
            else
            {
                lbAogConnected.BackColor = Color.Red;
            }

            SetTitle();
        }

        private void bntOK_Click(object sender, EventArgs e)
        {
            this.Close();
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
                Tls.TimedMessageBox(Lang.lgAlreadyRunning);
                this.Close();
            }

            // UDP
            UDPnetwork.StartUDPServer();
            if (!UDPnetwork.isUDPSendConnected)
            {
                Tls.TimedMessageBox("UDPnetwork failed to start.", "", 3000, true);
            }

            UDPLoopBack.StartUDPServer();
            if (!UDPLoopBack.isUDPSendConnected)
            {
                Tls.TimedMessageBox("UDPLoopBack failed to start.", "", 3000, true);
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

        private void NanoTimer_Tick(object sender, EventArgs e)
        {
            if (RC.SimulationType == SimType.VirtualNano) RC.Nano.MainLoop();
        }
    }
}