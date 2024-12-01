using RateController.Properties;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RateController
{
    public enum SimType
    {
        Sim_None,
        Sim_Speed
    }

    public partial class FormStart : Form
    {
        public readonly int MaxModules = 8;
        public readonly int MaxProducts = 6;// last two are fans
        public readonly int MaxRelays = 16;
        public readonly int MaxSections = 128;
        public readonly int MaxSensors = 8;
        public readonly int MaxSwitches = 16;
        public readonly double MPHtoKPH = 1.6092;
        public PGN229 AOGsections;
        public PGN254 AutoSteerPGN;
        public string[] CoverageAbbr = new string[] { "Ac", "Ha", "Min", "Hr" };
        public string[] CoverageDescriptions = new string[] { Lang.lgAcres, Lang.lgHectares, Lang.lgMinutes, Lang.lgHours };
        public bool cUseInches;
        public bool LargeScreenExit = false;
        public frmLargeScreen Lscrn;
        public PGN238 MachineConfig;
        public PGN239 MachineData;
        public PGN32700 ModuleConfig;
        public PGN32401 ModulesStatus;
        public PGN32702 NetworkConfig;
        public clsProducts Products;
        public clsAlarm RCalarm;
        public clsRelays RelayObjects;
        public bool Restart = false;
        public PGN32296 ScaleIndicator;
        public clsSectionControl SectionControl;
        public clsSections Sections;
        public PGN235 SectionsPGN;
        public SerialComm[] SER = new SerialComm[3];
        public bool ShowCoverageRemaining;
        public bool ShowQuantityRemaining;
        public Color SimColor = Color.FromArgb(255, 182, 0);
        public PGN32618 SwitchBox;
        public frmSwitches SwitchesForm;
        public clsSwitches SwitchObjects;
        public clsTools Tls;

        public string[] TypeDescriptions = new string[] { Lang.lgSection, Lang.lgSlave, Lang.lgMaster, Lang.lgPower,
            Lang.lgInvertSection,Lang.lgHydUp,Lang.lgHydDown,Lang.lgTramRight,
            Lang.lgTramLeft,Lang.lgGeoStop,Lang.lgSwitch, Lang.lgNone};

        public UDPComm UDPaog;
        public UDPComm UDPmodules;
        public PGN228 VRdata;
        public clsVirtualSwitchBox vSwitchBox;
        public string WiFiIP;
        public clsZones Zones;
        private int cDefaultProduct = 0;
        private bool cMasterOverride;
        private byte cPressureToShowID;
        private int cPrimeDelay = 3;
        private double cPrimeTime = 0;
        private int cRateType;
        private bool cResumeAfterPrime;
        private bool cShowPressure;
        private double cPressureCal;
        private double cPressureOffset;
        private bool[] cShowScale = new bool[4];
        private bool cShowSwitches = false;
        private SimType cSimMode = SimType.Sim_None;
        private double cSimSpeed = 0;
        private DateTime cStartTime;
        private int CurrentPage;
        private int CurrentPageLast;
        private bool cUseDualAuto;
        private bool cUseLargeScreen = false;
        private bool cUseTransparent = false;
        private Label[] Indicators;
        private bool LoadError = false;
        private Label[] ProdName;
        private Label[] Rates;
        private PGN32501[] RelaySettings;
        private Label[] Targets;
        private int mouseX = 0;
        private int mouseY = 0;
        private int windowLeft = 0;
        private int windowTop = 0;

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
            mnuSettings.Items["exitToolStripMenuItem"].Text = Lang.lgExit;

            mnuSettings.Items["commDiagnosticsToolStripMenuItem"].Text = Lang.lgCommDiagnostics;
            mnuSettings.Items["newToolStripMenuItem"].Text = Lang.lgNew; ;
            mnuSettings.Items["openToolStripMenuItem"].Text = Lang.lgOpen;
            mnuSettings.Items["saveAsToolStripMenuItem"].Text = Lang.lgSaveAs;

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
            ModulesStatus = new PGN32401(this);

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

            RelayObjects = new clsRelays(this);

            timerMain.Interval = 1000;

            RelaySettings = new PGN32501[MaxModules];
            for (int i = 0; i < MaxModules; i++)
            {
                RelaySettings[i] = new PGN32501(this, i);
            }

            Zones = new clsZones(this);
            vSwitchBox = new clsVirtualSwitchBox(this);
            ModuleConfig = new PGN32700(this);
            NetworkConfig = new PGN32702(this);
            AOGsections = new PGN229(this);
            SectionControl = new clsSectionControl(this);
            SwitchObjects = new clsSwitches(this);
            ScaleIndicator = new PGN32296(this);
        }

        public event EventHandler ProductChanged;
        private void mouseMove_MouseDown(object sender, MouseEventArgs e)
        {
            // Log the current window location and the mouse location.
            if (e.Button == MouseButtons.Right)
            {
                windowTop = this.Top;
                windowLeft = this.Left;
                mouseX = e.X;
                mouseY = e.Y;
            }
        }

        private void mouseMove_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                windowTop = this.Top;
                windowLeft = this.Left;

                Point pos = new Point(0, 0);

                pos.X = windowLeft + e.X - mouseX;
                pos.Y = windowTop + e.Y - mouseY;
                this.Location = pos;
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

        public bool MasterOverride
        {
            get { return cMasterOverride; }
            set
            {
                cMasterOverride = value;
                Tls.SaveProperty("MasterOverride", cMasterOverride.ToString());
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

        public int PrimeDelay
        {
            get { return cPrimeDelay; }
            set
            {
                if (value >= 3 && value < 9) { cPrimeDelay = value; }
            }
        }

        public double PrimeTime
        {
            get { return cPrimeTime; }
            set
            {
                if (value >= 0 && value < 30) { cPrimeTime = value; }
            }
        }

        public int RateType
        {
            // 0 current rate, 1 instantaneous rate
            get { return cRateType; }
            set
            {
                if (value >= 0 && value < 2) cRateType = value;
            }
        }

        public bool ResumeAfterPrime
        {
            get { return cResumeAfterPrime; }
            set
            {
                cResumeAfterPrime = value;
                Tls.SaveProperty("ResumeAfterPrime", cResumeAfterPrime.ToString());
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
        public double PressureCal
        {
            get { return cPressureCal; }
            set
            {
                cPressureCal = value;
                Tls.SaveProperty("PressureCal", value.ToString());
            }
        }

        public double PressureOffset
        {
            get { return cPressureOffset; }
            set
            {
                cPressureOffset = value;
                Tls.SaveProperty("PressureOffset",value.ToString());
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

        public SimType SimMode
        {
            get { return cSimMode; }
            set
            {
                cSimMode = value;
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

        public bool UseDualAuto
        { get { return cUseDualAuto; } set { cUseDualAuto = value; } }

        public bool UseInches
        {
            get { return cUseInches; }
            set
            {
                cUseInches = value;
                Tls.SaveProperty("UseInches", cUseInches.ToString());
            }
        }

        public bool UseLargeScreen
        {
            get { return cUseLargeScreen; }
            set
            {
                if (cUseLargeScreen != value)
                {
                    cUseLargeScreen = value;
                    Tls.SaveProperty("UseLargeScreen", cUseLargeScreen.ToString());
                    SwitchScreens();
                }
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
            Form fs = Tls.IsFormOpen("frmPressureDisplay");

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

        public void DisplayScales()
        {
            bool Found = false;
            for (int i = 0; i < 4; i++)
            {
                Found = false;
                if (cShowScale[i])
                {
                    // open instance
                    foreach (Form form in Application.OpenForms)
                    {
                        if (form.Text == "Scale " + (i + 1).ToString())
                        {
                            Found = true;
                            break;
                        }
                    }
                    if (!Found)
                    {
                        Form frm = new frmScaleDisplay(this, i);
                        frm.Show();
                    }
                }
                else
                {
                    // close instance
                    foreach (Form form in Application.OpenForms)
                    {
                        if (form.Text == "Scale " + (i + 1).ToString())
                        {
                            form.Close();
                            break;
                        }
                    }
                }
            }
        }

        public void DisplaySwitches()
        {
            Form fs = Tls.IsFormOpen("frmSwitches");

            if (cShowSwitches)
            {
                if (fs == null)
                {
                    SwitchesForm = new frmSwitches(this);
                    SwitchesForm.Show();
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
            SetDisplay();

            if (bool.TryParse(Tls.LoadProperty("UseInches"), out bool tmp)) cUseInches = tmp;
            if (bool.TryParse(Tls.LoadProperty("UseTransparent"), out bool Ut)) cUseTransparent = Ut;
            if (bool.TryParse(Tls.LoadProperty("ShowPressure"), out bool SP)) cShowPressure = SP;
            if (double.TryParse(Tls.LoadProperty("PressureCal"), out double pc)) cPressureCal = pc;
            if(double.TryParse(Tls.LoadProperty("PressureOffset"),out double po))cPressureOffset= po;

            for (int i = 0; i < 4; i++)
            {
                cShowScale[i] = false;
                if (bool.TryParse(Tls.LoadProperty("ShowScale_" + i.ToString()), out bool ss)) cShowScale[i] = ss;
            }

            if (byte.TryParse(Tls.LoadProperty("PressureID"), out byte ID)) cPressureToShowID = ID;
            if (bool.TryParse(Tls.LoadProperty("ShowQuantityRemaining"), out bool QR)) ShowQuantityRemaining = QR;
            if (bool.TryParse(Tls.LoadProperty("ShowCoverageRemaining"), out bool CR)) ShowCoverageRemaining = CR;
            if (bool.TryParse(Tls.LoadProperty("UseDualAuto"), out bool ud)) cUseDualAuto = ud;
            if (bool.TryParse(Tls.LoadProperty("ResumeAfterPrime"), out bool re)) cResumeAfterPrime = re;
            if (bool.TryParse(Tls.LoadProperty("MasterOverride"), out bool mo)) cMasterOverride = mo;
            if (int.TryParse(Tls.LoadProperty("PrimeDelay"), out int PD))
            {
                cPrimeDelay = PD;
            }
            if (cPrimeDelay < 3) cPrimeDelay = 3;

            if (double.TryParse(Tls.LoadProperty("SimSpeed"), out double Spd))
            {
                cSimSpeed = Spd;
            }
            else
            {
                cSimSpeed = 5;
            }

            if (Enum.TryParse(Tls.LoadProperty("SimMode"), out SimType SM))
            {
                cSimMode = SM;
            }
            else
            {
                cSimMode = SimType.Sim_None;
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
            RelayObjects.Load();

            LoadDefaultProduct();
            Zones.Load();
            SwitchObjects.Load();

            if (bool.TryParse(Tls.LoadProperty("UseLargeScreen"), out bool LS))
            {
                UseLargeScreen = LS;
            }
            else
            {
                UseLargeScreen = false;
            }

            if (bool.TryParse(Tls.LoadProperty("ShowSwitches"), out bool SS))
            {
                ShowSwitches = SS;
            }
            else
            {
                ShowSwitches = false;
            }

            // check loaded forms, reload to update
            Form Opts = Tls.IsFormOpen("frmOptions");
            if (Opts != null)
            {
                Opts.Close();
                Opts = new frmOptions(this);
                Opts.Show();
            }

            Form Swt = Tls.IsFormOpen("frmSwitches");
            if (Swt != null)
            {
                Swt.Close();
                Swt = new frmSwitches(this);
                Swt.Show();
            }

            if (int.TryParse(Tls.LoadProperty("RateType"), out int rt)) cRateType = rt;
        }

        public void NewFile()
        {
            saveFileDialog1.InitialDirectory = Tls.FilesDir();
            saveFileDialog1.Title = "New File";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                    Tls.OpenFile(saveFileDialog1.FileName, true);
                    LoadSettings();
                }
            }
        }

        public void OpenFile()
        {
            try
            {
                openFileDialog1.InitialDirectory = Tls.FilesDir();
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Tls.PropertiesFile = openFileDialog1.FileName;
                    Products.Load();
                    LoadSettings();
                }
            }
            catch (Exception ex)
            {
                Tls.WriteErrorLog("FormStart/OpenFile: " + ex.Message);
            }
        }

        public void SaveFileAs()
        {
            saveFileDialog1.InitialDirectory = Tls.FilesDir();
            saveFileDialog1.Title = "Save As";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                    Tls.SaveFile(saveFileDialog1.FileName);
                    Tls.ReadOnly = false;
                    LoadSettings();
                }
            }
        }

        public void SendRelays()
        {
            for (int i = 0; i < MaxModules; i++)
            {
                if (ModulesStatus.Connected(i)) RelaySettings[i].Send();
            }
        }

        public void SendSerial(byte[] Data)
        {
            for (int i = 0; i < 3; i++)
            {
                SER[i].SendData(Data);
            }
        }

        public void SetScale(int ProductID, bool Show)
        {
            cShowScale[ProductID] = Show;
            Tls.SaveProperty("ShowScale_" + ProductID.ToString(), cShowScale[ProductID].ToString());
            DisplayScales();
        }

        public bool ShowScale(int ProductID)
        {
            bool Result = false;
            if (ProductID < 4) Result = cShowScale[ProductID];
            return Result;
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

        public void SwitchScreens(bool SingleProduct = false)
        {
            try
            {
                Form fs = Tls.IsFormOpen("frmLargeScreen");
                if (cUseLargeScreen)
                {
                    if (SingleProduct)
                    {
                        // hide unused items, set product 4 as default, set product 4 id to 0
                        foreach (clsProduct Prd in Products.Items)
                        {
                            Prd.OnScreen = false;
                        }
                        clsProduct P0 = Products.Items[0];
                        clsProduct P3 = Products.Items[3];

                        P3.ProductName = P0.ProductName;
                        P3.ControlType = P0.ControlType;
                        P3.QuantityDescription = P0.QuantityDescription;
                        P3.CoverageUnits = P0.CoverageUnits;
                        P3.MeterCal = P0.MeterCal;
                        P3.ProdDensity = P0.ProdDensity;
                        P3.EnableProdDensity = P0.EnableProdDensity;
                        P3.RateSet = P0.RateSet;
                        P3.RateAlt = P0.RateAlt;
                        P3.TankSize = P0.TankSize;
                        P3.TankStart = P0.TankStart;

                        P3.UseVR = P0.UseVR;
                        P3.VRID = P0.VRID;
                        P3.VRmax = P0.VRmax;
                        P3.VRmin = P0.VRmin;

                        P3.PIDkp = P0.PIDkp;
                        P3.PIDki = P0.PIDki;
                        P3.PIDkd = P0.PIDkd;
                        P3.PIDmax = P0.PIDmax;
                        P3.PIDmin = P0.PIDmin;
                        P3.PIDscale = P0.PIDscale;

                        Products.Item(2).BumpButtons = false;
                        P0.ModuleID = 6;
                        P3.ChangeID(0, 0);
                        P3.OnScreen = true;
                        P3.AppMode = P0.AppMode;
                        P3.UseOffRateAlarm = P0.UseOffRateAlarm;
                        P3.OffRateSetting = P0.OffRateSetting;
                        P3.MinUPM = P0.MinUPM;
                        P3.BumpButtons = false;

                        P3.CountsRev = P0.CountsRev;
                        DefaultProduct = 3;
                        UseTransparent = true;
                    }

                    if (fs == null)
                    {
                        LargeScreenExit = false;
                        Restart = false;
                        this.WindowState = FormWindowState.Minimized;
                        this.ShowInTaskbar = false;
                        Lscrn = new frmLargeScreen(this);
                        Lscrn.ShowInTaskbar = true;
                        Lscrn.SetTransparent();
                        Lscrn.Show();
                    }
                }
                else
                {
                    // use standard screen
                    if (fs != null) Lscrn.SwitchToStandard();
                }
            }
            catch (Exception ex)
            {
                Tls.WriteErrorLog("SwitchScreens: " + ex.Message);
            }
        }

        public void UpdateStatus()
        {
            try
            {
                this.Text = "RC [" + Path.GetFileNameWithoutExtension(Properties.Settings.Default.FileName) + "]";

                if (cSimMode == SimType.Sim_Speed || SectionControl.PrimeOn)
                {
                    btnSettings.Image = Properties.Resources.SimGear;
                }
                else
                {
                    if (AutoSteerPGN.Connected())
                    {
                        btnSettings.Image = Properties.Resources.GreenGear;
                    }
                    else
                    {
                        btnSettings.Image = Properties.Resources.RedGear;
                    }
                }

                FormatDisplay();

                if (CurrentPage == 0)
                {
                    // summary
                    for (int i = 0; i < MaxProducts; i++)
                    {
                        ProdName[i].Text = Products.Item(i).ProductName;

                        ProdName[i].BackColor = Color.Transparent;
                        ProdName[i].ForeColor = SimColor;
                        ProdName[i].BorderStyle = BorderStyle.None;

                        Rates[i].Text = Products.Item(i).SmoothRate().ToString("N1");
                        if (i < 4)
                        {
                            Targets[i].Text = Products.Item(i).TargetRate().ToString("N1");
                        }

                        if (Products.Item(i).RateSensor.Connected())
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

                    switch (cRateType)
                    {
                        case 1:
                            lbRate.Text = Lang.lgInstantRate;
                            lbRateAmount.Text = Prd.CurrentRate().ToString("N1");
                            break;
                        default:
                            lbRate.Text = Lang.lgCurrentRate;
                            lbRateAmount.Text = Prd.SmoothRate().ToString("N1");
                            break;
                    }

                    if (Prd.RateSensor.ModuleSending())
                    {
                        if (Prd.RateSensor.ModuleReceiving())
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
            }
            catch (Exception ex)
            {
                Tls.WriteErrorLog("FormStart/UpdateStatus: " + ex.Message);
            }
        }

        private void AreaDone_Click(object sender, EventArgs e)
        {
            var Hlp = new frmMsgBox(this, "Reset area?", "Reset", true);
            Hlp.TopMost = true;

            Hlp.ShowDialog();
            bool Result = Hlp.Result;
            Hlp.Close();
            if (Result)
            {
                Products.Item(CurrentProduct()).ResetCoverage();
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
            SetDisplay();
        }

        private void calibrateToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //check if window already exists
            Form fs = Tls.IsFormOpen("frmCalibrate");

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

        private void commDiagnosticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form fs = Tls.IsFormOpen("frmModule");

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

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormatDisplay()
        {
            try
            {
                int ID = CurrentPage - 1;
                if (ID < 0) ID = 0;
                clsProduct Prd = Products.Item(ID);

                this.Width = 277;

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

                    this.Height = 248;
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

                        this.Height = 222;
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
                        this.Height = 222;
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

        private void FormRateControl_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Tls.SaveFormData(this);
                if (this.WindowState == FormWindowState.Normal)
                {
                    Tls.SaveProperty("CurrentPage", CurrentPage.ToString());
                }

                Sections.Save();
                Products.Save();
                Tls.SaveProperty("ShowQuantityRemaining", ShowQuantityRemaining.ToString());
                Tls.SaveProperty("ShowCoverageRemaining", ShowCoverageRemaining.ToString());

                Tls.SaveProperty("PrimeTime", cPrimeTime.ToString());
                Tls.SaveProperty("PrimeDelay", cPrimeDelay.ToString());
                Tls.SaveProperty("SimSpeed", cSimSpeed.ToString());
                Tls.SaveProperty("SimMode", cSimMode.ToString());
                Tls.SaveProperty("UseDualAuto", cUseDualAuto.ToString());

                UDPaog.Close();
                UDPmodules.Close();

                timerMain.Enabled = false;
                timerPIDs.Enabled = false;
                Tls.WriteActivityLog("Stopped");
                string mes = "Run time (hours): " + ((DateTime.Now - cStartTime).TotalSeconds / 3600.0).ToString("N1");
                Tls.WriteActivityLog(mes);

                for (int i = 0; i < 3; i++)
                {
                    SER[i].CloseRCport("true");
                }

                Tls.SaveProperty("RateType", cRateType.ToString());
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

                //SwitchScreens();
                DisplaySwitches();
                DisplayPressure();
                DisplayScales();

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
            cStartTime = DateTime.Now;
        }

        private void groupBox3_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Yellow);
        }

        private void label34_Click(object sender, EventArgs e)
        {
            ShowQuantityRemaining = !ShowQuantityRemaining;
            UpdateStatus();
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
            int prod = CurrentPage - 1;
            if (prod < 0) prod = 0;
            Form restoreform = new RCRestore(this, RateType, Products.Item(prod));
            restoreform.Show();
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

        private void lblUnits_Click(object sender, EventArgs e)
        {
            ShowSettings();
        }

        private void lbProduct_Click(object sender, EventArgs e)
        {
            ShowSettings();
        }

        private void lbRate_Click(object sender, EventArgs e)
        {
            cRateType++;
            if (cRateType > 1) cRateType = 0;
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

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowSettings();
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

        private void MnuComm_Click(object sender, EventArgs e)
        {
            Form fs = Tls.IsFormOpen("frmComm");

            if (fs == null)
            {
                Form frm = new frmComm(this);
                frm.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private void MnuOptions_Click(object sender, EventArgs e)
        {
            Form fs = Tls.IsFormOpen("frmOptions");

            if (fs == null)
            {
                Form frm = new frmOptions(this);
                frm.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private void MnuRelays_Click_1(object sender, EventArgs e)
        {
            Form fs = Tls.IsFormOpen("frmRelays");

            if (fs == null)
            {
                Form frm = new frmRelays(this);
                frm.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private void mnuSettings_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void networkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form fs = Tls.IsFormOpen("frmModuleConfig");

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

        private void newToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            NewFile();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }


        private void productsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSettings();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileAs();
        }

        private void sectionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form fs = Tls.IsFormOpen("frmSections");

            if (fs != null)
            {
                fs.Focus();
                return;
            }

            Form frm = new frmSections(this);
            frm.Show();
        }

        private void SetDisplay()
        {
            this.BackColor = Color.Black;
            foreach (Control c in this.Controls)
            {
                c.ForeColor = Color.Yellow;
            }
            lbAogConnected.ForeColor = Color.Black;
            lbArduinoConnected.ForeColor = Color.Black;

            for (int i = 0; i < 6; i++)
            {
                Indicators[i].BackColor = Color.Transparent;
            }

            lbOn.BackColor = Color.Transparent;
            lbOff.BackColor = Color.Transparent;
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

        private void ShowSettings()
        {
            //check if window already exists
            Form fs = Tls.IsFormOpen("FormSettings");

            if (fs != null)
            {
                fs.Focus();
                return;
            }

            Form frm = new FormSettings(this, CurrentPage);
            frm.Show();
        }

        private void TankRemain_Click(object sender, EventArgs e)
        {
            var Hlp = new frmMsgBox(this, "Reset quantity?", "Reset", true);
            Hlp.TopMost = true;

            Hlp.ShowDialog();
            bool Result = Hlp.Result;
            Hlp.Close();
            if (Result)
            {
                Products.Item(CurrentProduct()).ResetApplied();
            }
        }

        private void timerMain_Tick(object sender, EventArgs e)
        {
            UpdateStatus();
            SendRelays();
            Products.Update();
            SectionControl.ReadRateSwitches();
        }

        private void timerPIDs_Tick(object sender, EventArgs e)
        {
            Products.UpdatePID();
        }
    }
}