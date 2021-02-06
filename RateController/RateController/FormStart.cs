using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace RateController
{
    public enum SimType
    {
        None,
        VirtualNano,
        RealNano
    }

    public partial class FormStart : Form
    {
        public clsTools Tls;

        public UDPComm UDPLoopBack;
        public UDPComm UDPnetwork;

        private int[] RateType = new int[5];    // 0 current rate, 1 instantaneous rate, 2 overall rate
        public string LoopBackIP = "127.0.0.1";
        public string LoopBackBroadcastIP = "127.0.0.255";

        public SerialComm[] SER = new SerialComm[5];
        public PGN32612 AOG = new PGN32612();

        public string[] CoverageDescriptions = new string[] { "Acre", "Hectare", Lang.lgHour, Lang.lgMinute };
        public string[] CoverageAbbr = new string[] { "Ac", "Ha", "Hr", "Min" };

        public string[] QuantityDescriptions = new string[] { "Imp. Gallons", "US Gallons", "Lbs", "Litres", "Kgs" };
        public string[] QuantityAbbr = new string[] { "Imp Gal", "US Gal", "Lbs", "Ltr", "Kgs" };

        public CRateCals[] RateCals = new CRateCals[5];
        private int CurrentPage;
        private Label[] ProdName;

        private Label[] Rates;
        private Label[] Indicators;
        private DateTime LastSave;

        public FormStart()
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

            UDPLoopBack = new UDPComm(this, 8120, 9999, 1460, LoopBackBroadcastIP); // AOG
            UDPnetwork = new UDPComm(this, 8000, 9999, 1480, "", true);             // arduino

            for (int i = 0; i < 5; i++)
            {
                RateCals[i] = new CRateCals(this, i);
            }

            for (int i = 0; i < 5; i++)
            {
                SER[i] = new SerialComm(this, i);
            }

            ProdName = new Label[] { prd0, prd1, prd2, prd3, prd4 };
            Rates = new Label[] { rt0, rt1, rt2, rt3, rt4 };
            Indicators = new Label[] { idc0, idc1, idc2, idc3, idc4 };
        }

        private void FormStart_Load(object sender, EventArgs e)
        {
            CurrentPage = 5;
            int.TryParse(Tls.LoadProperty("CurrentPage"), out CurrentPage);

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
            UpdateStatus();
        }

        public void LoadSettings()
        {
            StartSerial();
            SetDayMode();
            this.Text = "RC [" + Path.GetFileNameWithoutExtension(Properties.Settings.Default.FileName) + "]";
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
                
                for(int i=0;i<5;i++)
                {
                    Indicators[i].BackColor = Properties.Settings.Default.DayColour;
                }
            }
            else
            {
                this.BackColor = Properties.Settings.Default.NightColour;
                foreach (Control c in this.Controls)
                {
                    c.ForeColor = Color.White;
                }

                for (int i = 0; i < 5; i++)
                {
                    Indicators[i].BackColor = Properties.Settings.Default.NightColour;
                }
            }
        }

        public void StartSerial()
        {
            try
            {
                for (int i = 0; i < 5; i++)
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
            if (CurrentPage == 0)
            {
                // summary
                for (int i = 0; i < 5; i++)
                {
                    ProdName[i].Text = RateCals[i].ProductName;
                    Rates[i].Text = RateCals[i].SmoothRate();
                    if (RateCals[i].ArduinoConnected)
                    {
                        Indicators[i].Image = Properties.Resources.OnSmall;
                    }
                    else
                    {
                        Indicators[i].Image = Properties.Resources.OffSmall;
                    }
                }
                lbArduinoConnected.Visible = false;
            }
            else
            {
                // product pages

                lbProduct.Text = CurrentPage.ToString() + ". " + RateCals[CurrentPage - 1].ProductName;
                lblUnits.Text = RateCals[CurrentPage - 1].Units();
                SetRate.Text = RateCals[CurrentPage - 1].RateSet.ToString("N1");
                AreaDone.Text = RateCals[CurrentPage - 1].CurrentCoverage();
                TankRemain.Text = RateCals[CurrentPage - 1].CurrentTankRemaining().ToString("N0");
                VolApplied.Text = RateCals[CurrentPage - 1].CurrentApplied();
                lbCoverage.Text = RateCals[CurrentPage - 1].CoverageDescription();

                switch (RateType[CurrentPage - 1])
                {
                    case 1:
                        lbRate.Text = Lang.lgInstantRate;
                        lbRateAmount.Text = RateCals[CurrentPage - 1].CurrentRate();
                        break;
                    case 2:
                        lbRate.Text = Lang.lgOverallRate;
                        lbRateAmount.Text = RateCals[CurrentPage - 1].AverageRate();
                        break;
                    default:
                        lbRate.Text = Lang.lgCurrentRate;
                        lbRateAmount.Text = RateCals[CurrentPage - 1].SmoothRate();
                        break;
                }


                if (RateCals[CurrentPage - 1].ArduinoConnected)
                {
                    lbArduinoConnected.BackColor = Color.LightGreen;
                }
                else
                {
                    lbArduinoConnected.BackColor = Color.Red;
                }
                lbArduinoConnected.Visible = true;

                if (RateCals[CurrentPage - 1].SimulationType == SimType.None)
                {
                    lbArduinoConnected.Text = "Controller";
                }
                else if(RateCals[CurrentPage-1].SimulationType==SimType.RealNano)
                {
                    lbArduinoConnected.Text = "Controller (R)";
                }
                else
                {
                    lbArduinoConnected.Text = "Controller (V)";
                }
            }


            if (AOG.Connected())
            {
                lbAogConnected.BackColor = Color.LightGreen;
            }
            else
            {
                lbAogConnected.BackColor = Color.Red;
            }

            if (CurrentPage == 0)
            {
                panProducts.Visible = false;
                panSummary.Visible = true;
            }
            else
            {
                panProducts.Visible = true;
                panSummary.Visible = false;
            }
        }

        private void FormRateControl_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                Tls.SaveFormData(this);
                Tls.SaveProperty("CurrentPage", CurrentPage.ToString());
            }
            for(int i=0;i<5;i++)
            {
                RateCals[i].SaveSettings();
            }
        }

        private void groupBox3_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Black);
        }

        private void FormStart_MouseDown(object sender, MouseEventArgs e)
        {
            MessageBox.Show("switch");
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            if (CurrentPage > 0)
            {
                CurrentPage--;
                UpdateStatus();
            }
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            if (CurrentPage < 5)
            {
                CurrentPage++;
                UpdateStatus();
            }
        }

        private void bntOK_Click(object sender, EventArgs e)
        {
            DialogResult Result;
            Result = MessageBox.Show("Confirm Close?", "Close", MessageBoxButtons.YesNo );
            if (Result == DialogResult.Yes) this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form frmRateSettings = new FormSettings(this,CurrentPage);
            frmRateSettings.ShowDialog();
            UpdateStatus();
            SetDayMode();
        }

        private void lbRate_Click(object sender, EventArgs e)
        {
            RateType[CurrentPage - 1]++;
            if (RateType[CurrentPage - 1] > 2) RateType[CurrentPage-1] = 0;
            UpdateStatus();
        }

        private void timerMain_Tick(object sender, EventArgs e)
        {
            UpdateStatus();

            for (int i = 0; i < 5; i++)
            {
                RateCals[i].Update();
                if ((DateTime.Now - LastSave).TotalSeconds > 60)
                {
                    RateCals[i].SaveSettings();
                    LastSave = DateTime.Now;
                }
            }
        }

        private void timerNano_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                if (RateCals[i].SimulationType == SimType.VirtualNano) RateCals[i].Nano.MainLoop();
            }
        }
    }
}
