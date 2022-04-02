﻿using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

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
        public PGN254 AutoSteerPGN = new PGN254();
        public PGN32618 SwitchBox;
        public PGN32620 SwitchIDs;
        public PGN32621 PressureData;
        public clsAlarm RCalarm;
        public PGN230 VRdata = new PGN230();

        public double CalCounterEnd;
        public double CalCounterStart;

        public string[] CoverageAbbr = new string[] { "Ac", "Ha", "Hr", "Min" };
        public string[] CoverageDescriptions = new string[] { Lang.lgAcres, Lang.lgHectares, Lang.lgHours, Lang.lgMinutes };
        public bool DoCal;

        public clsProducts Products;

        public string[] QuantityAbbr = new string[] { "Imp Gal", "US Gal", "Lbs", "Ltr", "Kgs" };
        public string[] QuantityDescriptions = new string[] { "Imp. Gallons", "US Gallons", "Lbs", "Litres", "Kgs" };
        public clsSections Sections;

        public SerialComm[] SER = new SerialComm[3];
        public clsTools Tls;
       
        public UDPComm UDPagio;
        public UDPComm UDPnetwork;
        public UDPComm UDPconfig;

        public bool UseInches;
        private int CurrentPage;

        private Label[] Indicators;
        private Label[] ProdName;
        private Label[] Rates;

        private int[] RateType = new int[5];    // 0 current rate, 1 instantaneous rate, 2 overall rate

        public Color SimColor = Color.FromArgb(255, 191, 0);
        private bool ShowQuantityRemaining = true;
        public clsPressures PressureObjects;

        public FormStart()
        {
            InitializeComponent();

            #region // language

            lbRate.Text = Lang.lgCurrentRate;
            label1.Text = Lang.lgTargetRate;
            lbCoverage.Text = Lang.lgCoverage;
            lbApplied.Text = Lang.lgQuantityApplied;
            lbRemaining.Text = Lang.lgTank_Remaining + " ...";

            mnuSettings.Items[0].Text = Lang.lgProducts;
            mnuSettings.Items[1].Text = Lang.lgSection;
            mnuSettings.Items[3].Text = Lang.lgPressure;
            mnuSettings.Items[4].Text = Lang.lgNew;
            mnuSettings.Items[5].Text = Lang.lgLoad;
            mnuSettings.Items[6].Text = Lang.lgSaveAs;
            mnuSettings.Items[7].Text = Lang.lgLanguage;
            mnuSettings.Items[8].Text = Lang.lgPCBconfig;
            mnuSettings.Items[9].Text = Lang.lgFirmware;
            mnuSettings.Items[10].Text = Lang.lgAbout;

            #endregion // language

            Tls = new clsTools(this);
            UDPagio = new UDPComm(this, 16666, 17777, 16660, "127.0.0.255");       // AGIO
            //UDPagio = new UDPComm(this, 17777, 15555, 1460, "127.255.255.255");       // AOG

            UDPnetwork = new UDPComm(this, 29999, 28888, 1480, "192.168.1.255");    // arduino
            UDPconfig = new UDPComm(this, 29900, 28800, 1482, "192.168.1.255");     // pcb config

            SwitchBox = new PGN32618(this);
            SwitchIDs = new PGN32620(this);
            PressureData = new PGN32621(this);

            Sections = new clsSections(this);
            Products = new clsProducts(this);
            RCalarm = new clsAlarm(this, btAlarm);

            for (int i = 0; i < 3; i++)
            {
                SER[i] = new SerialComm(this, i);
            }

            ProdName = new Label[] { prd0, prd1, prd2, prd3, prd4 };
            Rates = new Label[] { rt0, rt1, rt2, rt3, rt4 };
            Indicators = new Label[] { idc0, idc1, idc2, idc3, idc4 };

            UseInches = true;

            PressureObjects = new clsPressures(this);
        }

        public byte CurrentProduct()
        {
            if (CurrentPage < 2)
            {
                return 1;
            }
            else
            {
                return (byte)CurrentPage;
            }
        }

        public void LoadSettings()
        {
            StartSerial();
            SetDayMode();
            this.Text = "RC [" + Path.GetFileNameWithoutExtension(Properties.Settings.Default.FileName) + "]";

            bool tmp;
            if (bool.TryParse(Tls.LoadProperty("UseInches"), out tmp)) UseInches = tmp;

            Sections.Load();
            Sections.CheckSwitchDefinitions();

            Products.Load();

            UDPnetwork.BroadCastIP = Tls.LoadProperty("BroadCastIP");
            UDPconfig.BroadCastIP = Tls.LoadProperty("BroadCastIP");

            PressureObjects.Load();
        }

        public void StartSerial()
        {
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    String ID = "_" + i.ToString() + "_";
                    SER[i].RCportName = Tls.LoadProperty("RCportName" + ID + i.ToString());

                    int tmp;
                    if (int.TryParse(Tls.LoadProperty("RCportBaud" + ID + i.ToString()), out tmp))
                    {
                        SER[i].RCportBaud = tmp;
                    }
                    else
                    {
                        SER[i].RCportBaud = 38400;
                    }

                    bool tmp2;
                    bool.TryParse(Tls.LoadProperty("RCportSuccessful" + ID + i.ToString()), out tmp2);
                    if (tmp2) SER[i].OpenRCport();
                }
            }
            catch (Exception ex)
            {
                Tls.WriteErrorLog("FormRateControl/StartSerial: " + ex.Message);
                Tls.ShowHelp(ex.Message, this.Text, 3000, true);
            }
        }

        public void UpdateStatus()
        {
            if (CurrentPage == 0)
            {
                // summary
                for (int i = 0; i < 5; i++)
                {
                    ProdName[i].Text = Products.Item(i).ProductName;

                    if (Products.Item(i).SimulationType == SimType.None)
                    {
                        ProdName[i].ForeColor = SystemColors.ControlText;
                        ProdName[i].BackColor = Properties.Settings.Default.DayColour;
                        ProdName[i].BorderStyle = BorderStyle.None;
                    }
                    else
                    {
                        //ProdName[i].ForeColor = SimColor;
                        ProdName[i].BackColor = SimColor;
                        ProdName[i].BorderStyle = BorderStyle.FixedSingle;
                    }

                    Rates[i].Text = Products.Item(i).SmoothRate().ToString("N1");
                    if (Products.Item(i).ArduinoModule.Connected())
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

                lbProduct.Text = CurrentPage.ToString() + ". " + Products.Item(CurrentPage - 1).ProductName;
                lblUnits.Text = Products.Item(CurrentPage - 1).Units();
                SetRate.Text = Products.Item(CurrentPage - 1).TargetRate().ToString("N1");
                AreaDone.Text = Products.Item(CurrentPage - 1).CurrentCoverage();
                VolApplied.Text = Products.Item(CurrentPage - 1).CurrentApplied();
                lbCoverage.Text = Products.Item(CurrentPage - 1).CoverageDescription();

                if (ShowQuantityRemaining)
                {
                    lbRemaining.Text = Lang.lgTank_Remaining + " ...";
                    TankRemain.Text = Products.Item(CurrentPage - 1).CurrentTankRemaining().ToString("N0");
                }
                else
                {
                    lbRemaining.Text = CoverageDescriptions[Products.Item(CurrentPage - 1).CoverageUnits] + " Left ...";
                    double RT = Products.Item(CurrentPage - 1).SmoothRate();
                    if (RT == 0) RT = Products.Item(CurrentPage - 1).TargetRate();

                    if ((RT > 0) & (Products.Item(CurrentPage - 1).CurrentTankRemaining() > 0))
                    {
                        TankRemain.Text = (Products.Item(CurrentPage - 1).CurrentTankRemaining() / RT).ToString("N1");
                    }
                    else
                    {
                        TankRemain.Text = "0.0";
                    }
                }

                switch (RateType[CurrentPage - 1])
                {
                    case 1:
                        lbRate.Text = Lang.lgInstantRate;
                        lbRateAmount.Text = Products.Item(CurrentPage - 1).CurrentRate().ToString("N1");
                        break;

                    case 2:
                        lbRate.Text = Lang.lgOverallRate;
                        lbRateAmount.Text = Products.Item(CurrentPage - 1).AverageRate().ToString("N1");
                        break;

                    default:
                        lbRate.Text = Lang.lgCurrentRate;
                        lbRateAmount.Text = Products.Item(CurrentPage - 1).SmoothRate().ToString("N1");
                        break;
                }

                if (Products.Item(CurrentPage - 1).ArduinoModule.Connected())
                {
                    if (Products.Item(CurrentPage - 1).SimulationType == SimType.None)
                    {
                        lbArduinoConnected.BackColor = Color.LightGreen;
                    }
                    else
                    {
                        lbArduinoConnected.BackColor = SimColor;
                    }
                }
                else
                {
                    lbArduinoConnected.BackColor = Color.Red;
                }
                lbArduinoConnected.Visible = true;
            }

            if (AutoSteerPGN.Connected())
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

            // alarm
            RCalarm.CheckAlarms();
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

        private void button3_Click(object sender, EventArgs e)
        {
            Button btnSender = (Button)sender;
            Point ptLowerLeft = new Point(0, btnSender.Height);
            ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
            mnuSettings.Show(ptLowerLeft);
            UpdateStatus();
            SetDayMode();
        }

        private void FormRateControl_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                Tls.SaveFormData(this);
                Tls.SaveProperty("CurrentPage", CurrentPage.ToString());
            }

            Sections.Save();
            Products.Save();

            Tls.SaveProperty("BroadCastIP", UDPnetwork.BroadCastIP);
        }

        private void FormStart_Load(object sender, EventArgs e)
        {
            CurrentPage = 5;
            int.TryParse(Tls.LoadProperty("CurrentPage"), out CurrentPage);

            Tls.LoadFormData(this);

            if (Tls.PrevInstance())
            {
                Tls.ShowHelp(Lang.lgAlreadyRunning,"Help",3000);
                this.Close();
            }

            // UDP
            UDPnetwork.StartUDPServer();
            if (!UDPnetwork.isUDPSendConnected)
            {
                Tls.ShowHelp("UDPnetwork failed to start.", "", 3000, true);
            }

            UDPagio.StartUDPServer();
            if (!UDPagio.isUDPSendConnected)
            {
                Tls.ShowHelp("UDPagio failed to start.", "", 3000, true);
            }

            UDPconfig.StartUDPServer();
            if (!UDPconfig.isUDPSendConnected)
            {
                Tls.ShowHelp("UDPconfig failed to start.", "", 3000, true);
            }

            LoadSettings();
            UpdateStatus();
        }

        private void groupBox3_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Black);
        }

        private void lbRate_Click(object sender, EventArgs e)
        {
            RateType[CurrentPage - 1]++;
            if (RateType[CurrentPage - 1] > 2) RateType[CurrentPage - 1] = 0;
            UpdateStatus();
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

                for (int i = 0; i < 5; i++)
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

        private void timerMain_Tick(object sender, EventArgs e)
        {
            UpdateStatus();
            Products.Update();
        }

        private void timerNano_Tick(object sender, EventArgs e)
        {
            Products.UpdateVirtualNano();
        }

        public void SendSerial(byte[] Data)
        {
            for (int i = 0; i < 3; i++)
            {
                SER[i].SendData(Data);
            }
        }

        private void productsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frm = new FormSettings(this, CurrentPage);
            frm.ShowDialog();
        }

        private void sectionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form Sec = new frmSections(this);
            Sec.ShowDialog();
        }

        private void commToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frm = new frmComm(this);
            frm.ShowDialog();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Tls.SettingsDir();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Tls.PropertiesFile = openFileDialog1.FileName;
                Products.Load();
                LoadSettings();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = Tls.SettingsDir();
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                    Tls.SaveFile(saveFileDialog1.FileName);
                    LoadSettings();
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frmAbout = new FormAbout(this);
            frmAbout.ShowDialog();
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void label34_Click(object sender, EventArgs e)
        {
            ShowQuantityRemaining = !ShowQuantityRemaining;
        }

        private void lbCoverage_Click(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void btAlarm_Click(object sender, EventArgs e)
        {
            RCalarm.Silence();
        }

        private void deustchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.setF_culture = "de";
            Properties.Settings.Default.Save();
            Application.Restart();
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.setF_culture = "en";
            Properties.Settings.Default.Save();
            Application.Restart();
        }

        private void nederlandsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.setF_culture = "nl";
            Properties.Settings.Default.Save();
            Application.Restart();
        }

        private void pressureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frm = new FormPressure(this);
            frm.ShowDialog();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = Tls.SettingsDir();
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                    Tls.NewFile(saveFileDialog1.FileName);
                    LoadSettings();
                }
            }
        }

        private void SetRate_Click(object sender, EventArgs e)
        {

        }

        private void lbRate_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "1 - Current Rate, shows" +
                " the target rate when it is within 10% of target. Outside this range it" +
                " shows the exact rate being applied. \n 2 - Instant Rate, shows the exact rate." +
                "\n 3 - Overall, averages total quantity applied over area done.";

            Tls.ShowHelp(Message, "Rate");
            hlpevent.Handled = true;
        }

        private void lbRemaining_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Shows either quantity remaining or area that can be done with the remaining quantity.";

            Tls.ShowHelp(Message, "Remaining");
            hlpevent.Handled = true;
        }

        private void lbArduinoConnected_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Indicates if the arduino module is connected. Green is connected, " +
                "yellow is simulation mode, red not connected.";

            this.Tls.ShowHelp(Message, "MOD");
            hlpevent.Handled = true;
        }

        private void lbAogConnected_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Indicates if AgOpenGPS is connected. Green is connected, " +
                "red is not connected.";

            this.Tls.ShowHelp(Message, "AOG");
            hlpevent.Handled = true;
        }

        private void firmwareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form Sec = new frmFirmware(this);
            Sec.ShowDialog();
        }

        private void pCBConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form tmp = new frmPCBsettings(this);
            tmp.ShowDialog();
        }
    }
}