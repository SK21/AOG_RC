using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RateController
{
    public enum SimType
    {
        None,
        VirtualNano,
        Speed,
        PWM
    }

    public partial class FormStart : Form
    {
        public readonly int MaxRelays = 16;
        public readonly int MaxSections = 16;

        public PGN254 AutoSteerPGN;
        public string[] CoverageAbbr = new string[] { "Ac", "Ha", "Min", "Hr" };
        public string[] CoverageDescriptions = new string[] { Lang.lgAcres, Lang.lgHectares, Lang.lgHours, Lang.lgMinutes };
        public PGN32621 PressureData;
        public clsPressures PressureObjects;
        public byte PressureToShowID;
        public clsProducts Products;
        public clsAlarm RCalarm;
        public clsRelays RelayObjects;
        public clsSections Sections;
        public PGN235 SectionsPGN;
        public SerialComm[] SER = new SerialComm[3];
        public bool ShowPressure;
        public Color SimColor = Color.FromArgb(255, 191, 0);
        public PGN32618 SwitchBox;
        public PGN32620 SwitchIDs;
        public clsTools Tls;
        public string[] TypeDescriptions = new string[] { Lang.lgSection, Lang.lgSlave, Lang.lgMaster, Lang.lgPower, Lang.lgInvertSection };
        public UDPComm UDPaog;
        public UDPComm UDPmodules;
        public bool UseInches;
        public PGN230 VRdata;
        public string WiFiIP;
        private int cCurrentPage;

        private Label[] Indicators;
        private Label[] ProdName;
        private Label[] Rates;

        private int[] RateType = new int[5];    // 0 current rate, 1 instantaneous rate, 2 overall rate
        private bool ShowCoverageRemaining;
        private bool ShowQuantityRemaining;

        private bool SimFormLoaded;
        public event EventHandler ProductChanged;
        private int CurrentPageLast;

        public FormStart()
        {
            InitializeComponent();

            #region // language

            lbRate.Text = Lang.lgCurrentRate;
            lbTarget.Text = Lang.lgTargetRate;
            lbCoverage.Text = Lang.lgCoverage;
            lbRemaining.Text = Lang.lgTank_Remaining + " ...";

            mnuSettings.Items["MnuProducts"].Text = Lang.lgProducts;
            mnuSettings.Items["MnuSections"].Text = Lang.lgSection;
            mnuSettings.Items["MnuOptions"].Text = Lang.lgOptions;
            mnuSettings.Items["MnuComm"].Text = Lang.lgComm;
            mnuSettings.Items["MnuRelays"].Text = Lang.lgRelays;
            mnuSettings.Items["MnuPressures"].Text = Lang.lgPressure;

            MnuOptions.DropDownItems["MnuAbout"].Text = Lang.lgAbout;
            MnuOptions.DropDownItems["MnuNew"].Text = Lang.lgNew;
            MnuOptions.DropDownItems["MnuOpen"].Text = Lang.lgOpen;
            MnuOptions.DropDownItems["MnuSaveAs"].Text = Lang.lgSaveAs;
            MnuOptions.DropDownItems["MnuLanguage"].Text = Lang.lgLanguage;
            MnuOptions.DropDownItems["mnuMetric"].Text = Lang.lgMetric;
            MnuOptions.DropDownItems["mnuNetwork"].Text = Lang.lgNetwork;

            #endregion // language

            Tls = new clsTools(this);

            UDPaog = new UDPComm(this, 17777, 15555, 1460, "127.255.255.255", true, true);  // AOG
            UDPmodules = new UDPComm(this, 29999, 28888, 1480);    // arduino

            AutoSteerPGN = new PGN254(this);
            SectionsPGN = new PGN235(this);
            VRdata = new PGN230(this);

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
            RelayObjects = new clsRelays(this);

            timerMain.Interval = 1000;
        }

        public int CurrentProduct()
        {
            if (cCurrentPage < 2)
            {
                return 0;
            }
            else
            {
                return cCurrentPage - 1;
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
            PressureObjects.Load();
            RelayObjects.Load();
        }

        public void SendSerial(byte[] Data)
        {
            for (int i = 0; i < 3; i++)
            {
                SER[i].SendData(Data);
            }
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
            try
            {
                if (cCurrentPage == 0)
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
                    clsProduct Prd = Products.Item(cCurrentPage - 1);
                    lbProduct.Text = cCurrentPage.ToString() + ". " + Prd.ProductName;
                    lblUnits.Text = Prd.Units();
                    SetRate.Text = Prd.TargetRate().ToString("N1");

                    if (ShowCoverageRemaining)
                    {
                        lbCoverage.Text = CoverageDescriptions[Prd.CoverageUnits] + " Left ...";
                        double RT = Prd.SmoothRate();
                        if (RT == 0) RT = Prd.TargetRate();

                        if (Prd.ControlType == ControlTypeEnum.MotorWeights)
                        {
                            // using weights
                            if (Prd.Scale.Counts > 0)
                            {
                                AreaDone.Text = (Prd.CurrentWeight() / RT).ToString("N1");
                            }
                            else
                            {
                                AreaDone.Text = "0.0";
                            }
                        }
                        else
                        {
                            if ((RT > 0) & (Prd.TankStart > 0))
                            {
                                AreaDone.Text = ((Prd.TankStart - Prd.UnitsApplied()) / RT).ToString("N1");
                            }
                            else
                            {
                                AreaDone.Text = "0.0";
                            }
                        }
                    }
                    else
                    {
                        // show amount done
                        AreaDone.Text = Prd.CurrentCoverage().ToString("N1");
                        lbCoverage.Text = Prd.CoverageDescription() + " ...";
                    }

                    if (ShowQuantityRemaining)
                    {
                        lbRemaining.Text = Lang.lgTank_Remaining + " ...";
                        if (Prd.ControlType == ControlTypeEnum.MotorWeights)
                        {
                            // show weight
                            TankRemain.Text = (Prd.CurrentWeight()).ToString("N0");
                        }
                        else
                        {
                            // calculate remaining
                            TankRemain.Text = (Prd.TankStart - Prd.UnitsApplied()).ToString("N1");
                        }
                    }
                    else
                    {
                        // show amount done
                        lbRemaining.Text = Lang.lgQuantityApplied + " ...";
                        TankRemain.Text = Prd.UnitsApplied().ToString("N1");
                    }

                    switch (RateType[cCurrentPage - 1])
                    {
                        case 1:
                            lbRate.Text = Lang.lgInstantRate;
                            lbRateAmount.Text = Prd.CurrentRate().ToString("N1");
                            break;

                        case 2:
                            lbRate.Text = Lang.lgOverallRate;
                            lbRateAmount.Text = Prd.AverageRate().ToString("N1");
                            break;

                        default:
                            lbRate.Text = Lang.lgCurrentRate;
                            lbRateAmount.Text = Prd.SmoothRate().ToString("N1");
                            break;
                    }

                    if (Prd.SimulationType == SimType.None)
                    {
                        if (Prd.ArduinoModule.ModuleSending())
                        {
                            if (Prd.ArduinoModule.ModuleReceiving())
                            {
                                lbArduinoConnected.BackColor = Color.LightGreen;
                            }
                            else
                            {
                                lbArduinoConnected.BackColor = Color.LightBlue;
                            }
                        }
                        else
                        {
                            lbArduinoConnected.BackColor = Color.Red;
                        }
                    }
                    else
                    {
                        lbArduinoConnected.BackColor = SimColor;
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

                if (cCurrentPage == 0)
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

                // metric
                if (UseInches)
                {
                    MnuOptions.DropDownItems["mnuMetric"].Image = Properties.Resources.Xmark;
                }
                else
                {
                    MnuOptions.DropDownItems["mnuMetric"].Image = Properties.Resources.CheckMark;
                }

                if (cCurrentPage != CurrentPageLast)
                {
                    CurrentPageLast = cCurrentPage;
                    ProductChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                Tls.WriteErrorLog(ex.Message);
            }
        }

        private void btAlarm_Click(object sender, EventArgs e)
        {
            RCalarm.Silence();
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            if (cCurrentPage > 0)
            {
                cCurrentPage--;
                UpdateStatus();
            }
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            if (cCurrentPage < 5)
            {
                cCurrentPage++;
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

        private void CheckPressure()
        {
            try
            {
                if (ShowPressure)
                {
                    this.Height = 285;
                    btnSettings.Top = 182;
                    btnLeft.Top = 182;
                    btnRight.Top = 182;
                    lbArduinoConnected.Top = 182;
                    lbAogConnected.Top = 217;
                    lbPressure.Visible = true;
                    lbPressureValue.Visible = true;
                    lbPressure.Text = Lang.lgPressure + " " + PressureToShowID.ToString();
                    if (PressureToShowID < 1) PressureToShowID = 1;
                    float Prs = PressureObjects.Item(PressureToShowID - 1).Pressure();
                    lbPressureValue.Text = Prs.ToString("N1");
                }
                else
                {
                    this.Height = 256;
                    btnSettings.Top = 153;
                    btnLeft.Top = 153;
                    btnRight.Top = 153;
                    lbArduinoConnected.Top = 153;
                    lbAogConnected.Top = 188;
                    lbPressure.Visible = false;
                    lbPressureValue.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Tls.WriteErrorLog("FormStart/CheckPressure: " + ex.Message);
            }
        }

        private void FormRateControl_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                Tls.SaveFormData(this);
                Tls.SaveProperty("CurrentPage", cCurrentPage.ToString());
            }

            Sections.Save();
            Products.Save();

            Application.Exit();
        }

        private void FormStart_Load(object sender, EventArgs e)
        {
            cCurrentPage = 5;
            int.TryParse(Tls.LoadProperty("CurrentPage"), out cCurrentPage);

            Tls.LoadFormData(this);

            if (Tls.PrevInstance())
            {
                Tls.ShowHelp(Lang.lgAlreadyRunning, "Help", 3000);
                this.Close();
            }

            // UDP
            UDPmodules.StartUDPServer();
            if (!UDPmodules.isUDPSendConnected)
            {
                Tls.ShowHelp("UDPnetwork failed to start.", "", 3000, true, true);
            }

            UDPaog.StartUDPServer();
            if (!UDPaog.isUDPSendConnected)
            {
                Tls.ShowHelp("UDPagio failed to start.", "", 3000, true, true);
            }

            LoadSettings();
            UpdateStatus();

            LoadPressureSetting();
            Products.UpdatePID();

            // wifi
            WiFiIP = Tls.LoadProperty("WifiIP");
            UDPmodules.WifiEP = WiFiIP;

            // ethernet
            UDPmodules.EthernetEP = Tls.LoadProperty("EthernetEP");
        }

        private void groupBox3_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Black);
        }

        private void label34_Click(object sender, EventArgs e)
        {
            ShowQuantityRemaining = !ShowQuantityRemaining;
            UpdateStatus();
        }

        private void lbAogConnected_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void lbAogConnected_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Indicates if AgOpenGPS is connected. Green is connected, " +
                "red is not connected. Press to minimize window.";

            this.Tls.ShowHelp(Message, "AOG");
            hlpevent.Handled = true;
        }

        private void lbArduinoConnected_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void lbArduinoConnected_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Green indicates module is sending and receiving data, blue indicates module is sending but " +
                "not receiving (AOG needs to be connected for some Coverage Types), " +
                " red indicates module is not sending or receiving, yellow is simulation mode. Press to minimize window.";

            this.Tls.ShowHelp(Message, "MOD");
            hlpevent.Handled = true;
        }

        private void lbCoverage_Click(object sender, EventArgs e)
        {
            ShowCoverageRemaining = !ShowCoverageRemaining;
            UpdateStatus();
        }

        private void lbCoverage_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Shows either coverage done or area that can be done with the remaining quantity." +
                "\n Press to change.";

            Tls.ShowHelp(Message, "Coverage");
            hlpevent.Handled = true;
        }

        private void lbRate_Click(object sender, EventArgs e)
        {
            RateType[cCurrentPage - 1]++;
            if (RateType[cCurrentPage - 1] > 2) RateType[cCurrentPage - 1] = 0;
            UpdateStatus();
        }

        private void lbRate_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "1 - Current Rate, shows" +
                " the target rate when it is within 10% of target. Outside this range it" +
                " shows the exact rate being applied. \n 2 - Instant Rate, shows the exact rate." +
                "\n 3 - Overall, averages total quantity applied over area done." +
                "\n Press to change.";

            Tls.ShowHelp(Message, "Rate");
            hlpevent.Handled = true;
        }

        private void lbRemaining_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Shows either quantity applied or quantity remaining." +
                "\n Press to change.";

            Tls.ShowHelp(Message, "Remaining");
            hlpevent.Handled = true;
        }

        private void lbTarget_Click(object sender, EventArgs e)
        {
            if (lbTarget.Text == Lang.lgTargetRate)
            {
                lbTarget.Text = Lang.lgTargetRateAlt;
                Products.Item(cCurrentPage - 1).UseAltRate = true;
            }
            else
            {
                lbTarget.Text = Lang.lgTargetRate;
                Products.Item(cCurrentPage - 1).UseAltRate = false;
            }
        }

        private void lbTarget_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Press to switch between base rate and alternate rate.";

            Tls.ShowHelp(Message, "Target Rate");
            hlpevent.Handled = true;
        }

        private void LoadPressureSetting()
        {
            bool show;
            bool.TryParse(Tls.LoadProperty("ShowPressure"), out show);
            ShowPressure = show;
            byte.TryParse(Tls.LoadProperty("PressureID"), out PressureToShowID);
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

        private void metricToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UseInches = !UseInches;
            Tls.SaveProperty("UseInches", UseInches.ToString());
        }

        private void MnuAbout_Click_1(object sender, EventArgs e)
        {
            Form frmAbout = new FormAbout(this);
            frmAbout.ShowDialog();
        }

        private void MnuComm_Click(object sender, EventArgs e)
        {
            Form frm = new frmComm(this);
            frm.ShowDialog();
        }

        private void MnuDeustch_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.setF_culture = "de";
            Properties.Settings.Default.Save();
            Application.Restart();
        }

        private void MnuEnglish_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.setF_culture = "en";
            Properties.Settings.Default.Save();
            Application.Restart();
        }

        private void MnuNederlands_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.setF_culture = "nl";
            Properties.Settings.Default.Save();
            Application.Restart();
        }

        private void MnuRelays_Click_1(object sender, EventArgs e)
        {
            Form tmp = new frmRelays(this);
            tmp.ShowDialog();
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

        private void pressuresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frmPressure = new FormPressure(this);
            frmPressure.ShowDialog();
            LoadPressureSetting();
            CheckPressure();
        }

        private void productsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frm = new FormSettings(this, cCurrentPage);
            frm.ShowDialog();
        }

        private void russianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.setF_culture = "ru";
            Properties.Settings.Default.Save();
            Application.Restart();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void sectionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form Sec = new frmSections(this);
            Sec.ShowDialog();
        }

        private void serialMonitorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form Monitor = new frmMonitor(this);
            Monitor.ShowDialog();
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
            CheckPressure();
        }

        private void timerNano_Tick(object sender, EventArgs e)
        {
            Products.UpdateVirtualNano();
        }

        private void wifiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frmWifi = new frmWifi(this);
            frmWifi.ShowDialog();
        }

        private void MnuLanguage_Click(object sender, EventArgs e)
        {

        }

        private void simulationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!SimFormLoaded)
            {
                Form frmSim = new frmSimulation(this);
                frmSim.Show();
                SimFormLoaded = true;
            }
        }
    }
}