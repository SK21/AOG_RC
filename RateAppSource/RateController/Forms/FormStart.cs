using GMap.NET;
using RateController.Classes;
using RateController.Language;
using RateController.PGNs;
using RateController.Properties;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RateController
{
    public partial class FormStart : Form
    {
        public PGN229 AOGsections;
        public PGN254 AutoSteerPGN;
        public string[] CoverageAbbr = new string[] { "Ac", "Ha", "Min", "Hr" };
        public string[] CoverageDescriptions = new string[] { Lang.lgAcres, Lang.lgHectares, Lang.lgMinutes, Lang.lgHours };
        public PGN100 GPS;
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
        public Color SimColor = Color.FromArgb(255, 182, 0);
        public PGN32618 SwitchBox;
        public frmSwitches SwitchesForm;
        public clsTools Tls;

        public string[] TypeDescriptions = new string[] { Lang.lgSection, Lang.lgSlave, Lang.lgMaster, Lang.lgPower,
            Lang.lgInvertSection,Lang.lgHydUp,Lang.lgHydDown,Lang.lgTramRight,
            Lang.lgTramLeft,Lang.lgGeoStop,Lang.lgSwitch, Lang.lgNone,Lang.lgInvert_Master};

        public UDPComm UDPaog;
        public UDPComm UDPmodules;
        public clsVirtualSwitchBox vSwitchBox;
        public string WiFiIP;
        public clsZones Zones;
        private bool[] cShowScale = new bool[4];
        private DateTime cStartTime;
        private int CurrentPage;
        private int CurrentPageLast;
        private bool LoadError = false;
        private MouseButtons MouseButtonClicked;
        private Point MouseDownLocation;
        private Label[] ProdName;
        private Label[] Rates;
        private PGN32501[] RelaySettings;

        public FormStart()
        {
            InitializeComponent();

            #region // language

            lbRate.Text = Lang.lgCurrentRate;
            lbTarget.Text = Lang.lgTargetRate;
            lbCoverage.Text = Lang.lgCoverage;
            lbRemaining.Text = Lang.lgTank_Remaining;

            #endregion // language

            Props.MainForm = this;
            Props.CheckFolders();
            Props.OpenFile(Properties.Settings.Default.CurrentFile);
            Tls = new clsTools(this);
            Tls.StartMapManager();

            //UDPaog = new UDPComm(this, 16666, 17777, 16660, "127.0.0.255");       // AGIO

            UDPaog = new UDPComm(this, 17777, 15555, 1460, "UDPaog", "127.255.255.255");        // AOG
            UDPmodules = new UDPComm(this, 29999, 28888, 1480, "UDPmodules");                   // arduino

            AutoSteerPGN = new PGN254(this);
            SectionsPGN = new PGN235(this);
            MachineConfig = new PGN238(this);
            MachineData = new PGN239(this);

            SwitchBox = new PGN32618(this);
            ModulesStatus = new PGN32401(this);

            Sections = new clsSections(this);
            Products = new clsProducts(this);
            RCalarm = new clsAlarm(this, btAlarm);

            ProdName = new Label[] { prd0, prd1, prd2, prd3, prd4, prd5 };
            Rates = new Label[] { rt0, rt1, rt2, rt3, rt4, rt5 };

            RelayObjects = new clsRelays(this);

            RelaySettings = new PGN32501[Props.MaxModules];
            for (int i = 0; i < Props.MaxModules; i++)
            {
                RelaySettings[i] = new PGN32501(this, i);
            }

            Zones = new clsZones(this);
            vSwitchBox = new clsVirtualSwitchBox(this);
            ModuleConfig = new PGN32700(this);
            NetworkConfig = new PGN32702(this);
            AOGsections = new PGN229(this);
            SectionControl = new clsSectionControl(this);
            ScaleIndicator = new PGN32296(this);
            GPS = new PGN100(this);
        }

        public event EventHandler ColorChanged;

        public event EventHandler ProductChanged;

        public int LSLeft
        {
            get { return Lscrn.Left; }
            set { Lscrn.Left = value; }
        }

        public int LSTop
        {
            get { return Lscrn.Top; }
            set { Lscrn.Top = value; }
        }

        public void ChangeLanguage()
        {
            Restart = true;
            Application.Restart();
        }

        public int CurrentProduct()
        {
            int Result = 0;
            if (Props.UseLargeScreen)
            {
                Result = Lscrn.CurrentProduct();
            }
            else
            {
                if (CurrentPage > 1) Result = CurrentPage - 1;
            }
            return Result;
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

        public void LoadSettings()
        {
            SetDisplay();

            for (int i = 0; i < 4; i++)
            {
                cShowScale[i] = false;
                if (bool.TryParse(Props.GetProp("ShowScale_" + i.ToString()), out bool ss)) cShowScale[i] = ss;
            }

            Sections.Load();
            Sections.CheckSwitchDefinitions();

            Products.Load();
            RelayObjects.Load();

            LoadDefaultProduct();
            Zones.Load();

            Props.DisplaySwitches();

            // check loaded forms, reload to update
            Form Swt = Props.IsFormOpen("frmSwitches");
            if (Swt != null)
            {
                Swt.Close();
                Swt = new frmSwitches(this);
                Swt.Show();
            }

            Products.UpdatePID();
            UpdateStatus();
            DisplayScales();
        }

        public void RaiseColorChanged()
        {
            ColorChanged?.Invoke(this, EventArgs.Empty);
            SetDisplay();
        }

        public void SendRelays()
        {
            for (int i = 0; i < Props.MaxModules; i++)
            {
                if (ModulesStatus.Connected(i)) RelaySettings[i].Send();
            }
        }

        public void SetScale(int ProductID, bool Show)
        {
            if (ProductID < 4)
            {
                cShowScale[ProductID] = Show;
                Props.SetProp("ShowScale_" + ProductID.ToString(), cShowScale[ProductID].ToString());
                DisplayScales();
            }
        }

        public bool ShowScale(int ProductID)
        {
            bool Result = false;
            if (ProductID < 4) Result = cShowScale[ProductID];
            return Result;
        }

        public void UpdateStatus()
        {
            try
            {
                this.Text = "RC [" + Path.GetFileNameWithoutExtension(Properties.Settings.Default.CurrentFile) + "]";

                if (Props.SimMode == SimType.Sim_Speed || SectionControl.PrimeOn)
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
                    for (int i = 0; i < Props.MaxProducts; i++)
                    {
                        ProdName[i].Text = Products.Item(i).ProductName;

                        ProdName[i].BackColor = Color.Transparent;
                        ProdName[i].ForeColor = Properties.Settings.Default.DisplayForeColour;
                        ProdName[i].BorderStyle = BorderStyle.None;

                        Rates[i].Text = Products.Item(i).SmoothRate().ToString("N1");
                    }
                    lbArduinoConnected.Visible = false;
                }
                else
                {
                    // product pages
                    clsProduct Prd = Products.Item(CurrentPage - 1);

                    if (Props.VariableRateEnabled)
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

                    if (Props.ShowCoverageRemaining)
                    {
                        lbCoverage.Text = CoverageDescriptions[Prd.CoverageUnits] + " Left";
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
                        lbCoverage.Text = Prd.CoverageDescription();
                    }

                    double Tnk = 0;
                    if (Props.ShowQuantityRemaining)
                    {
                        lbRemaining.Text = Lang.lgTank_Remaining;
                        // calculate remaining
                        Tnk = Prd.TankStart - Prd.UnitsApplied();
                    }
                    else
                    {
                        // show amount done
                        lbRemaining.Text = Lang.lgQuantityApplied;
                        Tnk = Prd.UnitsApplied();
                    }
                    if (Math.Abs(Tnk) > 9999)
                    {
                        TankRemain.Text = Tnk.ToString("N0");
                    }
                    else
                    {
                        TankRemain.Text = Tnk.ToString("N1");
                    }

                    switch (Props.UserRateType)
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
                            lbArduinoConnected.BackColor = Color.DeepSkyBlue;
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
                if (!Props.UseLargeScreen) RCalarm.CheckAlarms();

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
                Props.WriteErrorLog("FormStart/UpdateStatus: " + ex.Message);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Define the border color and thickness
            Color borderColor = Properties.Settings.Default.DisplayForeColour;
            int borderWidth = 1;

            // Draw the border
            using (Pen pen = new Pen(borderColor, borderWidth))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1);
            }
        }

        private void AreaDone_Click(object sender, EventArgs e)
        {
            if (MouseButtonClicked == MouseButtons.Left)
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
            if (MouseButtonClicked == MouseButtons.Left)
            {
                if (CurrentPage > 0)
                {
                    CurrentPage--;
                    UpdateStatus();
                }
            }
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            if (MouseButtonClicked == MouseButtons.Left)
            {
                if (CurrentPage < Props.MaxProducts)
                {
                    CurrentPage++;
                    UpdateStatus();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ShowSettings(true);
        }

        private void FormatDisplay()
        {
            try
            {
                int ID = CurrentPage - 1;
                if (ID < 0) ID = 0;
                clsProduct Prd = Products.Item(ID);

                this.Width = 283;

                btAlarm.Top = 21;
                btAlarm.Left = 33;
                btAlarm.Visible = false;

                if (CurrentPage == 0)
                {
                    // summary panel
                    panSummary.Visible = true;
                    panFan.Visible = false;
                    panProducts.Visible = false;
                    panSummary.Top = 6;
                    panSummary.Left = 6;

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
                        panFan.Top = 6;
                        panFan.Left = 6;

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
                        panProducts.Top = 6;
                        panProducts.Left = 6;

                        // product panel
                        this.Height = 222;
                        btnSettings.Top = 154;
                        btnLeft.Top = 154;
                        btnRight.Top = 154;
                        lbArduinoConnected.Top = 154;
                        lbAogConnected.Top = 188;
                    }
                }
                Invalidate();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("FormStart/FormatDisplay: " + ex.Message);
            }
        }

        private void FormRateControl_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Props.SaveFormLocation(this);
                if (this.WindowState == FormWindowState.Normal)
                {
                    Props.SetProp("CurrentPage", CurrentPage.ToString());
                }

                Sections.Save();
                Products.Save();

                UDPaog.Close();
                UDPmodules.Close();

                timerMain.Enabled = false;
                timerPIDs.Enabled = false;
                timerRates.Enabled = false;
                Props.WriteActivityLog("Stopped");
                string mes = "Run time (hours): " + ((DateTime.Now - cStartTime).TotalSeconds / 3600.0).ToString("N1");
                Props.WriteActivityLog(mes);
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
                Props.LoadFormLocation(this);

                CurrentPage = 5;
                int.TryParse(Props.GetProp("CurrentPage"), out CurrentPage);

                if (Tls.PrevInstance())
                {
                    Tls.ShowMessage(Lang.lgAlreadyRunning, "Help", 3000);
                    this.Close();
                }

                // UDP
                UDPmodules.StartUDPServer();
                if (!UDPmodules.IsUDPSendConnected)
                {
                    Tls.ShowMessage("UDPnetwork failed to start.", "", 3000, true, true);
                }

                UDPaog.StartUDPServer();
                if (!UDPaog.IsUDPSendConnected)
                {
                    Tls.ShowMessage("UDPagio failed to start.", "", 3000, true, true);
                }

                LoadSettings();

                if (Props.UseLargeScreen) Props.SwitchScreens();
                Props.DisplaySwitches();
                Props.DisplayPressure();

                timerMain.Enabled = true;
                timerRates.Enabled = true;
            }
            catch (Exception ex)
            {
                Tls.ShowMessage("Failed to load properly: " + ex.Message, "Help", 30000, true);
                LoadError = true;
                Close();
            }
            SetLanguage();
            Props.WriteActivityLog("Started", true);
            cStartTime = DateTime.Now;

            if (Properties.Settings.Default.UseJobs)
            {
                // to do open jobs screen
            }
        }

        private void FormStart_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void groupBox3_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Properties.Settings.Default.DisplayForeColour);
        }

        private void label34_Click(object sender, EventArgs e)
        {
            if (MouseButtonClicked == MouseButtons.Left)
            {
                Props.ShowQuantityRemaining = !Props.ShowQuantityRemaining;
                UpdateStatus();
            }
        }

        private void lbAogConnected_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Indicates if AgOpenGPS is connected. Green is connected, " +
                "red is not connected. Press to minimize window.";

            this.Tls.ShowMessage(Message, "AOG");
            hlpevent.Handled = true;
        }

        private void lbArduinoConnected_Click(object sender, EventArgs e)
        {
            if (MouseButtonClicked == MouseButtons.Left)
            {
                int prod = CurrentPage - 1;
                if (prod < 0) prod = 0;
                Form restoreform = new RCRestore(this, Props.UserRateType, Products.Item(prod), this);
                restoreform.Show();
            }
        }

        private void lbArduinoConnected_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Green indicates module is sending and receiving data, blue indicates module is sending but " +
                "not receiving (AOG needs to be connected for some Coverage Types), " +
                " red indicates module is not sending or receiving, yellow is simulation mode. Press to minimize window.";

            this.Tls.ShowMessage(Message, "MOD");
            hlpevent.Handled = true;
        }

        private void lbCoverage_Click(object sender, EventArgs e)
        {
            if (MouseButtonClicked == MouseButtons.Left)
            {
                Props.ShowCoverageRemaining = !Props.ShowCoverageRemaining;
                UpdateStatus();
            }
        }

        private void lbCoverage_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Shows either coverage done or area that can be done with the remaining quantity." +
                "\n Press to change.";

            Tls.ShowMessage(Message, "Coverage");
            hlpevent.Handled = true;
        }

        private void lbRate_Click(object sender, EventArgs e)
        {
            if (MouseButtonClicked == MouseButtons.Left)
            {
                if (Props.UserRateType == 0)
                {
                    Props.UserRateType = 1;
                }
                else
                {
                    Props.UserRateType = 0;
                }
                UpdateStatus();
            }
        }

        private void lbRate_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "1 - Current Rate, shows" +
                " the target rate when it is within 10% of target. Outside this range it" +
                " shows the exact rate being applied. \n 2 - Instant Rate, shows the exact rate." +
                "\n 3 - Overall, averages total quantity applied over area done." +
                "\n Press to change.";

            Tls.ShowMessage(Message, "Rate");
            hlpevent.Handled = true;
        }

        private void lbRemaining_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Shows either quantity applied or quantity remaining." +
                "\n Press to change.";

            Tls.ShowMessage(Message, "Remaining");
            hlpevent.Handled = true;
        }

        private void lbTarget_Click(object sender, EventArgs e)
        {
            if (MouseButtonClicked == MouseButtons.Left)
            {
                if (!Props.VariableRateEnabled)
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
        }

        private void lbTarget_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Press to switch between base rate and alternate rate.";

            Tls.ShowMessage(Message, "Target Rate");
            hlpevent.Handled = true;
        }

        private void LoadDefaultProduct()
        {
            int count = 0;
            int tmp = 0;
            foreach (clsProduct Prd in Products.Items)
            {
                if (Prd.OnScreen && Prd.ID < Props.MaxProducts - 2)
                {
                    count++;
                    tmp = Prd.ID;
                }
            }
            if (count == 1) Props.DefaultProduct = tmp;

            CurrentPage = Props.DefaultProduct + 1;
        }

        private void mouseMove_MouseDown(object sender, MouseEventArgs e)
        {
            MouseButtonClicked = e.Button;
            if (e.Button == MouseButtons.Right ) MouseDownLocation = e.Location;
        }

        private void mouseMove_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right ) this.Location = new Point(this.Left + e.X - MouseDownLocation.X, this.Top + e.Y - MouseDownLocation.Y);
        }

        private void SetDisplay()
        {
            this.BackColor = Properties.Settings.Default.DisplayBackColour;
            foreach (Control c in this.Controls)
            {
                c.ForeColor = Properties.Settings.Default.DisplayForeColour;
            }
            lbAogConnected.ForeColor = Color.Black;
            lbArduinoConnected.ForeColor = Color.Black;

            lbOn.BackColor = Color.Transparent;
            lbOff.BackColor = Color.Transparent;

            for (int i = 0; i < Props.MaxProducts; i++)
            {
                ProdName[i].ForeColor = Properties.Settings.Default.DisplayForeColour;
            }
            groupBox3.ForeColor = Properties.Settings.Default.DisplayForeColour;
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
                Props.WriteErrorLog("FormStart/SetLanguage: " + ex.Message);
            }
        }

        private void ShowSettings(bool OpenLast = false)
        {
            //check if window already exists
            Form fs = Props.IsFormOpen("frmMenu");

            if (fs == null)
            {
                int Prd = CurrentPage - 1;
                if (Prd < 0) Prd = 0;
                Form frm = new frmMenu(this, Prd, OpenLast);
                frm.Show();
            }
        }

        private void TankRemain_Click(object sender, EventArgs e)
        {
            if (MouseButtonClicked == MouseButtons.Left)
            {
                //check if window already exists
                Form fs = Props.IsFormOpen("frmResetQuantity");

                if (fs != null)
                {
                    fs.Focus();
                    return;
                }

                Form frm = new frmResetQuantity(this);
                frm.ShowDialog();
            }
        }

        private void timerMain_Tick(object sender, EventArgs e)
        {
            UpdateStatus();
            SendRelays();
            Products.Save();
            SectionControl.ReadRateSwitches();
        }

        private void timerPIDs_Tick(object sender, EventArgs e)
        {
            Products.UpdatePID();
        }

        private void timerRates_Tick(object sender, EventArgs e)
        {
            if (GPS.Connected())
            {
                PointLatLng Position = new PointLatLng(GPS.Latitude, GPS.Longitude);
                Tls.Manager.SetTractorPosition(Position, Products.ProductAppliedRates(), Products.ProductTargetRates());
            }
            Tls.Manager.UpdateTargetRates();
        }

        private void lbProduct_MouseDown(object sender, MouseEventArgs e)
        {
            MouseButtonClicked = e.Button;
            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Left) MouseDownLocation = e.Location;
        }

        private void lbProduct_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Left) this.Location = new Point(this.Left + e.X - MouseDownLocation.X, this.Top + e.Y - MouseDownLocation.Y);
        }
    }
}