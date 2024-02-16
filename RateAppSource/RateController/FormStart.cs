using RateController.Properties;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static RateController.PGN32618;

namespace RateController
{
    public enum SimType
    {
        None,
        VirtualNano,
        Speed
    }

    public partial class FormStart : Form
    {
        public readonly int MaxModules = 8;
        public readonly int MaxProducts = 6;
        public readonly int MaxRelays = 16;
        public readonly int MaxSections = 128;
        public readonly int MaxSensors = 8; // last two are fans
        public readonly int MaxSwitches = 16;
        public PGN32401 AnalogData;
        public PGN254 AutoSteerPGN;
        public string[] CoverageAbbr = new string[] { "Ac", "Ha", "Min", "Hr" };
        public string[] CoverageDescriptions = new string[] { Lang.lgAcres, Lang.lgHectares, Lang.lgMinutes, Lang.lgHours };
        public bool cUseInches;
        public bool LargeScreenExit = false;
        public frmLargeScreen Lscrn;
        public PGN238 MachineConfig;
        public PGN239 MachineData;
        public PGN32700 ModuleConfig;
        public clsPressures PressureObjects;
        public clsProducts Products;
        public clsAlarm RCalarm;
        public clsRelays RelayObjects;
        public bool Restart = false;
        public clsSectionControl SectionControl;
        public clsSections Sections;
        public PGN235 SectionsPGN;
        public SerialComm[] SER = new SerialComm[3];
        public Color SimColor = Color.FromArgb(255, 191, 0);
        private SimType cSimMode = SimType.None;
        public PGN32618 SwitchBox;
        public clsTools Tls;
        public PGN228 VRdata;

        public string[] TypeDescriptions = new string[] { Lang.lgSection, Lang.lgSlave, Lang.lgMaster, Lang.lgPower,
            Lang.lgInvertSection,Lang.lgHydUp,Lang.lgHydDown,Lang.lgTramRight,
            Lang.lgTramLeft,Lang.lgGeoStop,Lang.lgNone};

        public UDPComm UDPaog;
        public UDPComm UDPmodules;
        public clsVirtualSwitchBox vSwitchBox;
        public string WiFiIP;
        public clsZones Zones;


        private int cDefaultProduct = 0;
        private byte cPressureToShowID;
        private bool cShowPressure;
        private bool cShowSwitches = false;
        private int CurrentPage;
        private int CurrentPageLast;
        private bool cUseLargeScreen = false;
        private bool cUseTransparent = false;
        private Label[] Indicators;
        private bool LoadError = false;
        private DateTime[] ModuleTime;
        private Label[] ProdName;
        private Label[] Rates;
        private Label[] Targets;

        private int[] RateType = new int[6];    // 0 current rate, 1 instantaneous rate, 2 overall rate
        private PGN32501[] RelaySettings;
        public bool ShowCoverageRemaining;
        public bool ShowQuantityRemaining;

        private double cPrimeTime = 0;
        private int cPrimeDelay = 0;
        private double cSimSpeed = 0;

        public FormStart()
        {
            InitializeComponent();

            #region // language

            lbRate.Text = Lang.lgCurrentRate;
            lbTarget.Text = Lang.lgTargetRate;
            lbCoverage.Text = Lang.lgCoverage;
            lbRemaining.Text = Lang.lgTank_Remaining + " ...";

            mnuSettings.Items["MnuProducts"].Text = Lang.lgProducts;
            mnuSettings.Items["MnuSections"].Text = Lang.lgSections;
            mnuSettings.Items["MnuOptions"].Text = Lang.lgOptions;
            mnuSettings.Items["MnuComm"].Text = Lang.lgComm;
            mnuSettings.Items["MnuRelays"].Text = Lang.lgRelays;
            mnuSettings.Items["calibrateToolStripMenuItem1"].Text = Lang.lgCalibrate;
            mnuSettings.Items["networkToolStripMenuItem"].Text = Lang.lgModules;

            MnuOptions.DropDownItems["pressuresToolStripMenuItem"].Text = Lang.lgPressure;
            MnuOptions.DropDownItems["MnuNew"].Text = Lang.lgNew;
            MnuOptions.DropDownItems["MnuOpen"].Text = Lang.lgOpen;
            MnuOptions.DropDownItems["MnuSaveAs"].Text = Lang.lgSaveAs;
            MnuOptions.DropDownItems["MnuLanguage"].Text = Lang.lgLanguage;
            MnuOptions.DropDownItems["mnuMetric"].Text = Lang.lgMetric;
            MnuOptions.DropDownItems["switchesToolStripMenuItem1"].Text = Lang.lgSwitches;
            MnuOptions.DropDownItems["commDiagnosticToolStripMenuItem"].Text = Lang.lgCommDiagnostics;

            #endregion // language

            Tls = new clsTools(this);

            //UDPaog = new UDPComm(this, 16666, 17777, 16660, "127.0.0.255");       // AGIO

            UDPaog = new UDPComm(this, 17777, 15555, 1460, "UDPaog", "127.255.255.255");        // AOG
            UDPmodules = new UDPComm(this, 29999, 28888, 1480, "UDPmodules");                   // arduino

            AutoSteerPGN = new PGN254(this);
            SectionsPGN = new PGN235(this);
            MachineConfig = new PGN238(this);
            MachineData = new PGN239(this);
            VRdata = new PGN228(this);

            SwitchBox = new PGN32618(this);
            AnalogData = new PGN32401(this);

            Sections = new clsSections(this);
            Products = new clsProducts(this);
            RCalarm = new clsAlarm(this, btAlarm);

            for (int i = 0; i < 3; i++)
            {
                SER[i] = new SerialComm(this, i);
            }

            ProdName = new Label[] { prd0, prd1, prd2, prd3, prd4, prd5 };
            Rates = new Label[] { rt0, rt1, rt2, rt3, rt4, rt5 };
            Indicators = new Label[] { idc0, idc1, idc2, idc3, idc4, idc5 };
            Targets = new Label[] { tg0, tg1, tg2, tg3 };

            cUseInches = true;

            PressureObjects = new clsPressures(this);
            RelayObjects = new clsRelays(this);
            SectionControl = new clsSectionControl(this);

            timerMain.Interval = 1000;

            RelaySettings = new PGN32501[MaxModules];
            for (int i = 0; i < MaxModules; i++)
            {
                RelaySettings[i] = new PGN32501(this, i);
            }

            ModuleTime = new DateTime[MaxModules];
            Zones = new clsZones(this);
            vSwitchBox = new clsVirtualSwitchBox(this);
            ModuleConfig = new PGN32700(this);
        }

        public event EventHandler ProductChanged;
        public SimType SimMode
        {
            get { return cSimMode; }
            set
            {
                cSimMode = value;
            }
        }
        public int DefaultProduct
        {
            get { return cDefaultProduct; }
            set
            {
                if (value >= 0 && value < MaxProducts - 2)
                {
                    cDefaultProduct = value;
                    Tls.SaveProperty("DefaultProduct", cDefaultProduct.ToString());
                }
            }
        }

        public byte PressureToShow
        {
            get { return cPressureToShowID; }
            set
            {
                if (value >= 0 && value < 17)
                {
                    cPressureToShowID = value;
                }
            }
        }

        public bool ShowPressure
        {
            get { return cShowPressure; }
            set
            {
                cShowPressure = value;
                Tls.SaveProperty("ShowPressure", value.ToString());
                DisplayPressure();
            }
        }

        public bool ShowSwitches
        {
            get { return cShowSwitches; }
            set
            {
                cShowSwitches = value;
                Tls.SaveProperty("ShowSwitches", cShowSwitches.ToString());
                DisplaySwitches();
            }
        }

        public double SimSpeed
        {
            get { return cSimSpeed; }
            set
            {
                if (value >= 0 && value < 40) { cSimSpeed = value; }
            }
        }

        public double PrimeTime
        {
            get { return cPrimeTime; }
            set
            {
                if(value>=0 && value<30) { cPrimeTime = value; }
            }
        }

        public bool UseInches
        {
            get { return cUseInches; }
            set { cUseInches = value; }
        }

        public bool UseLargeScreen
        {
            get { return cUseLargeScreen; }
            set
            {
                cUseLargeScreen = value;
                Tls.SaveProperty("UseLargeScreen", cUseLargeScreen.ToString());
            }
        }

        public bool UseTransparent
        {
            get { return cUseTransparent; }
            set
            {
                cUseTransparent = value;
                Tls.SaveProperty("UseTransparent", cUseTransparent.ToString());
            }
        }

        public bool UseZones
        {
            get
            {
                bool tmp = false;
                if (bool.TryParse(Tls.LoadProperty("UseZones"), out bool tmp2)) tmp = tmp2;
                return tmp;
            }
            set { Tls.SaveProperty("UseZones", value.ToString()); }
        }

        public void ChangeLanguage()
        {
            Restart = true;
            Application.Restart();
        }

        public int CurrentProduct()
        {
            int Result = 0;
            if (cUseLargeScreen)
            {
                Result = Lscrn.CurrentProduct();
            }
            else
            {
                if (CurrentPage > 1) Result = CurrentPage - 1;
            }
            return Result;
        }

        public void DisplayPressure()
        {
            Form fs = Application.OpenForms["frmPressureDisplay"];

            if (cShowPressure)
            {
                if (fs == null)
                {
                    Form frm = new frmPressureDisplay(this);
                    frm.Show();
                }
                else
                {
                    fs.Focus();
                }
            }
            else
            {
                if (fs != null) fs.Close();
            }
        }

        public void DisplaySwitches()
        {
            Form fs = Application.OpenForms["frmSwitches"];

            if (cShowSwitches)
            {
                if (fs == null)
                {
                    Form frm = new frmSwitches(this);
                    frm.Show();
                }
                else
                {
                    fs.Focus();
                }
            }
            else
            {
                if (fs != null) fs.Close();
            }
        }
        public void LoadSettings()
        {
            StartSerial();
            SetDayMode();

            if (bool.TryParse(Tls.LoadProperty("UseInches"), out bool tmp)) cUseInches = tmp;
            if (bool.TryParse(Tls.LoadProperty("UseLargeScreen"), out bool LS)) cUseLargeScreen = LS;
            if (bool.TryParse(Tls.LoadProperty("UseTransparent"), out bool Ut)) cUseTransparent = Ut;
            if (bool.TryParse(Tls.LoadProperty("ShowSwitches"), out bool SS)) cShowSwitches = SS;
            if (bool.TryParse(Tls.LoadProperty("ShowPressure"), out bool SP)) cShowPressure = SP;
            if (byte.TryParse(Tls.LoadProperty("PressureID"), out byte ID)) cPressureToShowID = ID;
            if (bool.TryParse(Tls.LoadProperty("ShowQuantityRemaining"), out bool QR)) ShowQuantityRemaining = QR;
            if (bool.TryParse(Tls.LoadProperty("ShowCoverageRemaining"), out bool CR)) ShowCoverageRemaining = CR;

            if (int.TryParse(Tls.LoadProperty("PrimeDelay"), out int PD))
            {
                cPrimeDelay = PD;
            }
            else
            {
                cPrimeDelay = 3;
            }

            if (double.TryParse(Tls.LoadProperty("SimSpeed"), out double Spd))
            {
                cSimSpeed = Spd;
            }
            else
            {
                cSimSpeed = 5;
            }
    
            if (double.TryParse(Tls.LoadProperty("PrimeTime"), out double ptime))
            {
                cPrimeTime = ptime;
            }
            else
            {
                cPrimeTime = 5;
            }

            Sections.Load();
            Sections.CheckSwitchDefinitions();

            Products.Load();
            PressureObjects.Load();
            RelayObjects.Load();

            LoadDefaultProduct();
            Zones.Load();
            SetTransparent(cUseTransparent);
        }

        public void MnuDeustch_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.setF_culture = "de";
            Settings.Default.UserLanguageChange = true;
            Properties.Settings.Default.Save();
            ChangeLanguage();
        }

        public bool ModuleConnected(int ModuleID)
        {
            bool Result = false;
            if (ModuleID > -1 && ModuleID < MaxModules)
            {
                Result = (DateTime.Now - ModuleTime[ModuleID]).TotalSeconds < 5;
            }
            return Result;
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

        public void UpdateModuleConnected(int ModuleID)
        {
            if (ModuleID > -1 && ModuleID < MaxModules) ModuleTime[ModuleID] = DateTime.Now;
        }

        public void UpdateStatus()
        {
            try
            {
                this.Text = "RC [" + Path.GetFileNameWithoutExtension(Properties.Settings.Default.FileName) + "]";

                FormatDisplay();

                if (CurrentPage == 0)
                {
                    // summary
                    for (int i = 0; i < MaxProducts; i++)
                    {
                        ProdName[i].Text = Products.Item(i).ProductName;

                        if (cSimMode == SimType.None)
                        {
                            ProdName[i].ForeColor = SystemColors.ControlText;
                            ProdName[i].BackColor = Properties.Settings.Default.DayColour;
                            ProdName[i].BorderStyle = BorderStyle.None;
                        }
                        else
                        {
                            ProdName[i].BackColor = SimColor;
                            ProdName[i].BorderStyle = BorderStyle.FixedSingle;
                        }

                        Rates[i].Text = Products.Item(i).SmoothRate().ToString("N1");
                        if (i < 4)
                        {
                            Targets[i].Text = Products.Item(i).TargetRate().ToString("N1");
                        }

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
                    clsProduct Prd = Products.Item(CurrentPage - 1);


                    if (Prd.UseVR)
                    {
                        lbTarget.Text = "VR Target";
                    }
                    else if (Prd.UseAltRate)
                    {
                        lbTarget.Text = Lang.lgTargetRateAlt;
                    }
                    else
                    {
                        lbTarget.Text = Lang.lgTargetRate;
                    }

                    lbFan.Text = CurrentPage.ToString() + ". " + Prd.ProductName;
                    lbTargetRPM.Text = Prd.TargetRate().ToString("N0");
                    lbCurrentRPM.Text = Prd.SmoothRate().ToString("N0");
                    lbOn.Visible = Prd.FanOn;
                    lbOff.Visible = !Prd.FanOn;

                    lbProduct.Text = CurrentPage.ToString() + ". " + Prd.ProductName;
                    SetRate.Text = Prd.TargetRate().ToString("N1");
                    lblUnits.Text = Prd.Units();

                    if (ShowCoverageRemaining)
                    {
                        lbCoverage.Text = CoverageDescriptions[Prd.CoverageUnits] + " Left ...";
                        double RT = Prd.SmoothRate();
                        if (RT == 0) RT = Prd.TargetRate();

                        if ((RT > 0) & (Prd.TankStart > 0))
                        {
                            AreaDone.Text = ((Prd.TankStart - Prd.UnitsApplied()) / RT).ToString("N1");
                        }
                        else
                        {
                            AreaDone.Text = "0.0";
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
                        // calculate remaining
                        TankRemain.Text = (Prd.TankStart - Prd.UnitsApplied()).ToString("N1");
                    }
                    else
                    {
                        // show amount done
                        lbRemaining.Text = Lang.lgQuantityApplied + " ...";
                        TankRemain.Text = Prd.UnitsApplied().ToString("N1");
                    }

                    switch (RateType[CurrentPage - 1])
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

                    if (cSimMode == SimType.None)
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

                // alarm
                if (!cUseLargeScreen) RCalarm.CheckAlarms();

                // metric
                if (cUseInches)
                {
                    MnuOptions.DropDownItems["mnuMetric"].Image = Properties.Resources.Cancel40;
                }
                else
                {
                    MnuOptions.DropDownItems["mnuMetric"].Image = Properties.Resources.Check;
                }

                if (CurrentPage != CurrentPageLast)
                {
                    CurrentPageLast = CurrentPage;
                    ProductChanged?.Invoke(this, EventArgs.Empty);
                }

                // fan button
                if (CurrentPage > 0 && Products.Item(CurrentPage - 1).FanOn)
                {
                    btnFan.Image = Properties.Resources.FanOn;
                }
                else
                {
                    btnFan.Image = Properties.Resources.FanOff;
                }

                if (ShowSwitches)
                {
                    switchesToolStripMenuItem1.Image = Properties.Resources.OK;
                }
                else
                {
                    switchesToolStripMenuItem1.Image = Properties.Resources.Cancel64;
                }

                // transparent
                if (UseTransparent)
                {
                    MnuOptions.DropDownItems["transparentToolStripMenuItem"].Image = Properties.Resources.Check;
                }
                else
                {
                    MnuOptions.DropDownItems["transparentToolStripMenuItem"].Image = Properties.Resources.Cancel40;
                }

            }
            catch (Exception ex)
            {
                Tls.WriteErrorLog("FormStart/UpdateStatus: " + ex.Message);
            }
        }

        private void btAlarm_Click(object sender, EventArgs e)
        {
            RCalarm.Silence();
        }

        private void btnFan_Click(object sender, EventArgs e)
        {
            Products.Item(CurrentPage - 1).FanOn = !Products.Item(CurrentPage - 1).FanOn;
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
            if (CurrentPage < MaxProducts)
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

        private void calibrateToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //check if window already exists
            Form fs = Application.OpenForms["frmCalibrate"];

            if (fs == null)
            {
                Form frm = new frmCalibrate(this);
                frm.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private void commDiagnosticToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form fs = Application.OpenForms["frmModule"];

            if (fs == null)
            {
                Form frm = new frmModule(this);
                frm.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private void FormatDisplay()
        {
            try
            {
                int ID = CurrentPage - 1;
                if (ID < 0) ID = 0;
                clsProduct Prd = Products.Item(ID);

                this.Width = 290;

                btAlarm.Top = 21;
                btAlarm.Left = 33;
                btAlarm.Visible = false;

                if (CurrentPage == 0)
                {
                    // summary panel
                    panSummary.Visible = true;
                    panFan.Visible = false;
                    panProducts.Visible = false;
                    panSummary.Top = 0;
                    panSummary.Left = 0;

                    this.Height = 283;
                    btnSettings.Top = 180;
                    btnLeft.Top = 180;
                    btnRight.Top = 180;
                    lbArduinoConnected.Top = 180;
                    lbAogConnected.Top = 214;
                }
                else
                {
                    panSummary.Visible = false;
                    if (Prd.ControlType == ControlTypeEnum.Fan)
                    {
                        // fan panel
                        panProducts.Visible = false;
                        panFan.Visible = true;
                        panFan.Top = 0;
                        panFan.Left = 0;

                        this.Height = 257;
                        btnSettings.Top = 154;
                        btnLeft.Top = 154;
                        btnRight.Top = 154;
                        lbArduinoConnected.Top = 154;
                        lbAogConnected.Top = 188;
                    }
                    else
                    {
                        panProducts.Visible = true;
                        panFan.Visible = false;
                        panProducts.Top = 0;
                        panProducts.Left = 0;

                        // product panel
                        this.Height = 257;
                        btnSettings.Top = 154;
                        btnLeft.Top = 154;
                        btnRight.Top = 154;
                        lbArduinoConnected.Top = 154;
                        lbAogConnected.Top = 188;
                    }
                }
            }
            catch (Exception ex)
            {
                Tls.WriteErrorLog("FormStart/FormatDisplay: " + ex.Message);
            }
        }
        public int PrimeDelay
        {
            get { return cPrimeDelay; }
            set
            {
                if (value >= 0 && value < 9) { cPrimeDelay = value; }
            }
        }

        private void FormRateControl_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (this.WindowState == FormWindowState.Normal)
                {
                    Tls.SaveFormData(this);
                    Tls.SaveProperty("CurrentPage", CurrentPage.ToString());
                }

                Sections.Save();
                Products.Save();
                Tls.SaveProperty("ShowQuantityRemaining", ShowQuantityRemaining.ToString());
                Tls.SaveProperty("ShowCoverageRemaining", ShowCoverageRemaining.ToString());

                Tls.SaveProperty("PrimeTime", cPrimeTime.ToString());
                Tls.SaveProperty("PrimeDelay",cPrimeDelay.ToString());
                Tls.SaveProperty("SimSpeed", cSimSpeed.ToString());

                UDPaog.Close();
                UDPmodules.Close();

                timerMain.Enabled = false;
                timerPIDs.Enabled = false;
                Tls.WriteActivityLog("Stopped", true);
            }
            catch (Exception)
            {
            }

            Application.Exit();
        }

        private void FormStart_Activated(object sender, EventArgs e)
        {
            if (Restart)
            {
                ChangeLanguage();
            }
            else if (LargeScreenExit)
            {
                this.Close();
            }
        }

        private void FormStart_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!LargeScreenExit && !Restart && !LoadError && Products.Connected())
            {
                var Hlp = new frmMsgBox(this, "Confirm Exit?", "Exit", true);
                Hlp.TopMost = true;

                Hlp.ShowDialog();
                bool Result = Hlp.Result;
                Hlp.Close();
                if (!Result) e.Cancel = true;
            }
        }

        private void FormStart_Load(object sender, EventArgs e)
        {
            try
            {
                Tls.LoadFormData(this);

                CurrentPage = 5;
                int.TryParse(Tls.LoadProperty("CurrentPage"), out CurrentPage);

                if (Tls.PrevInstance())
                {
                    Tls.ShowHelp(Lang.lgAlreadyRunning, "Help", 3000);
                    this.Close();
                }

                // UDP
                UDPmodules.StartUDPServer();
                if (!UDPmodules.IsUDPSendConnected)
                {
                    Tls.ShowHelp("UDPnetwork failed to start.", "", 3000, true, true);
                }

                UDPaog.StartUDPServer();
                if (!UDPaog.IsUDPSendConnected)
                {
                    Tls.ShowHelp("UDPagio failed to start.", "", 3000, true, true);
                }

                LoadSettings();
                Products.UpdatePID();
                UpdateStatus();

                if (cUseLargeScreen) StartLargeScreen();
                DisplaySwitches();
                DisplayPressure();

                timerMain.Enabled = true;
            }
            catch (Exception ex)
            {
                Tls.ShowHelp("Failed to load properly: " + ex.Message, "Help", 30000, true);
                LoadError = true;
                Close();
            }
            SetLanguage();
            Tls.WriteActivityLog("Started", true);
        }

        private void frenchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.setF_culture = "fr";
            Settings.Default.UserLanguageChange = true;
            Properties.Settings.Default.Save();
            ChangeLanguage();
        }

        private void groupBox3_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Black);
        }

        private void hungarianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.setF_culture = "hu";
            Settings.Default.UserLanguageChange = true;
            Properties.Settings.Default.Save();
            ChangeLanguage();
        }

        private void label34_Click(object sender, EventArgs e)
        {
            ShowQuantityRemaining = !ShowQuantityRemaining;
            UpdateStatus();
        }

        private void largeScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartLargeScreen();
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
            RateType[CurrentPage - 1]++;
            if (RateType[CurrentPage - 1] > 2) RateType[CurrentPage - 1] = 0;
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
            if (!Products.Item(CurrentPage - 1).UseVR)
            {
                if (Products.Item(CurrentPage - 1).UseAltRate)
                {
                    lbTarget.Text = Lang.lgTargetRate;
                    Products.Item(CurrentPage - 1).UseAltRate = false;
                }
                else
                {
                    lbTarget.Text = Lang.lgTargetRateAlt;
                    Products.Item(CurrentPage - 1).UseAltRate = true;
                }
            }
        }

        private void lbTarget_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Press to switch between base rate and alternate rate.";

            Tls.ShowHelp(Message, "Target Rate");
            hlpevent.Handled = true;
        }

        private void LoadDefaultProduct()
        {
            if (int.TryParse(Tls.LoadProperty("DefaultProduct"), out int DP)) cDefaultProduct = DP;
            int count = 0;
            int tmp = 0;
            foreach (clsProduct Prd in Products.Items)
            {
                if (Prd.OnScreen && Prd.ID < MaxProducts - 2)
                {
                    count++;
                    tmp = Prd.ID;
                }
            }
            if (count == 1) DefaultProduct = tmp;

            CurrentPage = cDefaultProduct + 1;
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Tls.FilesDir();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Tls.PropertiesFile = openFileDialog1.FileName;
                Products.Load();
                LoadSettings();
            }
        }

        private void metricToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cUseInches = !cUseInches;
            Tls.SaveProperty("UseInches", cUseInches.ToString());
        }

        private void MnuComm_Click(object sender, EventArgs e)
        {
            Form frm = new frmComm(this);
            frm.ShowDialog();
        }

        private void MnuEnglish_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.setF_culture = "en";
            Settings.Default.UserLanguageChange = true;
            Properties.Settings.Default.Save();
            ChangeLanguage();
        }

        private void MnuNederlands_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.setF_culture = "nl";
            Settings.Default.UserLanguageChange = true;
            Properties.Settings.Default.Save();
            ChangeLanguage();
        }

        private void MnuRelays_Click_1(object sender, EventArgs e)
        {
            Form tmp = new frmRelays(this);
            tmp.ShowDialog();
        }

        private void networkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form fs = Application.OpenForms["frmModuleConfig"];

            if (fs == null)
            {
                Form frm = new frmModuleConfig(this);
                frm.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = Tls.FilesDir();
            saveFileDialog1.Title = "New File";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                    Tls.OpenFile(saveFileDialog1.FileName);
                    LoadSettings();
                }
            }
        }

        private void polishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.setF_culture = "pl";
            Settings.Default.UserLanguageChange = true;
            Properties.Settings.Default.Save();
            ChangeLanguage();
        }

        private void pressuresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form fs = Application.OpenForms["FormPressure"];

            if (fs == null)
            {
                Form frm = new FormPressure(this);
                frm.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private void productsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //check if window already exists
            Form fs = Application.OpenForms["FormSettings"];

            if (fs != null)
            {
                fs.Focus();
                return;
            }

            Form frm = new FormSettings(this, CurrentPage);
            frm.Show();
        }

        private void russianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.setF_culture = "ru";
            Settings.Default.UserLanguageChange = true;
            Properties.Settings.Default.Save();
            ChangeLanguage();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = Tls.FilesDir();
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

                lbOn.BackColor = Properties.Settings.Default.DayColour;
                lbOff.BackColor = Properties.Settings.Default.DayColour;
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

                lbOn.BackColor = Properties.Settings.Default.NightColour;
                lbOff.BackColor = Properties.Settings.Default.NightColour;
            }
        }

        private void SetLanguage()
        {
            try
            {
                if (Settings.Default.AOG_language == Settings.Default.setF_culture)
                {
                    Settings.Default.UserLanguageChange = false;
                    Settings.Default.Save();
                }
                else
                {
                    if (!Settings.Default.UserLanguageChange)
                    {
                        Settings.Default.setF_culture = Settings.Default.AOG_language;
                        Settings.Default.Save();
                        ChangeLanguage();
                    }
                }
            }
            catch (Exception ex)
            {
                Tls.WriteErrorLog("FormStart/SetLanguage: " + ex.Message);
            }
        }

        private void StartLargeScreen()
        {
            UseLargeScreen = true;
            LargeScreenExit = false;
            Restart = false;
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            Lscrn = new frmLargeScreen(this);
            Lscrn.ShowInTaskbar = true;
            Lscrn.SetTransparent(cUseTransparent);
            Lscrn.Show();
        }

        private void switchesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowSwitches = !cShowSwitches;
            DisplaySwitches();
        }

        private void timerMain_Tick(object sender, EventArgs e)
        {
            UpdateStatus();

            for (int i = 0; i < MaxModules; i++)
            {
                if (ModuleConnected(i)) RelaySettings[i].Send();
            }

            Products.Update();
            SectionControl.ReadRateSwitches();
        }

        private void timerPIDs_Tick(object sender, EventArgs e)
        {
            Products.UpdatePID();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void transparentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transparentToolStripMenuItem.Checked = !transparentToolStripMenuItem.Checked;
            SetTransparent(transparentToolStripMenuItem.Checked);
        }

        public void SetTransparent(bool frmtrans)
        {
            transparentToolStripMenuItem.Checked = frmtrans;
            if (transparentToolStripMenuItem.Checked)
            {
                UseTransparent = true;
            }
            else
            {
                UseTransparent = false;
            }
        }

        private void primedStartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form fs = Application.OpenForms["frmPrimedStart"];

            if (fs != null)
            {
                fs.Focus();
                return;
            }

            Form frm = new frmPrimedStart(this);
            frm.Show();
        }
    }
}