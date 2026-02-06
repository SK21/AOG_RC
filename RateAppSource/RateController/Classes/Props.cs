using RateController.Forms;
using RateController.Language;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Windows.Forms;

namespace RateController.Classes
{
    public enum ApplicationMode
    { ControlledUPM, ConstantUPM, DocumentApplied, DocumentTarget }

    public enum CanDriver
    { SLCAN, InnoMaker, PCAN }

    public enum ControlTypeEnum
    { Valve, ComboClose, Motor, MotorWeights, Fan, ComboCloseTimed }

    public enum MasterSwitchMode
    { ControlAll, ControlMasterRelayOnly, Override };

    public enum ModuleTypes
    {
        unknown,
        Teensy_Rate,
        Nano_Rate,
        Nano_SwitchBox,
        ESP_Rate
    }

    public enum RateType
    { Applied, Target }

    public enum RelayTypes
    { Section, Slave, Master, Power, Invert_Section, HydUp, HydDown, TramRight, TramLeft, GeoStop, Switch, None, Invert_Master };

    public enum SpeedType
    { GPS, Wheel, Simulated, ISOBUS }

    public enum SwIDs
    {
        // 0 Used to be AutoSwitch. It was replaced with AutoSection and AutoRate.
        NotUsed, MasterOn, MasterOff, RateUp, RateDown, sw0, sw1, sw2, sw3, sw4, sw5,

        sw6, sw7, sw8, sw9, sw10, sw11, sw12, sw13, sw14, sw15, AutoSection, AutoRate, WorkSwitch
    };

    public static class Props
    {
        public static readonly int MaxModules = 8;
        public static readonly int MaxProducts = 7;
        public static readonly int MaxRelays = 16;
        public static readonly int MaxSections = 128;
        public static readonly int MaxSensorsPerModule = 16;
        public static readonly int MaxSwitches = 16;
        public static readonly double MPHtoKPH = 1.609344;
        public static string[] CoverageAbbr = new string[] { "Ac", "Ha", "Min", "Hr" };
        public static string[] CoverageDescriptions = new string[] { Lang.lgAcres, Lang.lgHectares, Lang.lgMinutes, Lang.lgHours };
        public static bool cShowCoverageRemaining;
        public static bool cShowQuantityRemaining;

        public static string[] TypeDescriptions = new string[] { Lang.lgSection, Lang.lgSlave, Lang.lgMaster, Lang.lgPower,
            Lang.lgInvertSection,Lang.lgHydUp,Lang.lgHydDown,Lang.lgTramRight,
            Lang.lgTramLeft,Lang.lgGeoStop,Lang.lgSwitch, Lang.lgNone,Lang.lgInvert_Master};

        private static string cActivityFileName = "";
        private static string cAppDate = "06-Feb-2026";
        private static string cApplicationFolder;
        private static string cAppName = "RateController";
        private static SortedDictionary<string, string> cAppProps = new SortedDictionary<string, string>();
        private static string cAppPropsFileName = "";
        private static string cAppVersion = "4.2.0";
        private static CanDriver cCurrentCanDriver = CanDriver.SLCAN;
        private static string cCurrentMenuName = "";
        private static int cCurrentProduct;
        private static string cDefaultDir;
        private static int cDefaultProduct;
        private static string cErrorsFileName = "";
        private static string cFieldNames;
        private static bool cIsobusEnabled = false;
        private static bool cMapPreview = false;
        private static MasterSwitchMode cMasterSwitchMode = MasterSwitchMode.ControlAll;
        private static int cPrimeDelay = 3;
        private static double cPrimeTime = 0;
        private static string cProfilesFolder;
        private static SortedDictionary<string, string> cProps = new SortedDictionary<string, string>();
        private static bool cRateCalibrationOn = false;
        private static bool cRateRecordEnabled;
        private static bool cReadOnly = false;
        private static bool cResumeAfterPrime;
        private static int cSensorSettingsMaxID = -1;
        private static bool cShowCanDiagnostics = false;
        private static string cCanPort = "COM7";
        private static bool cShowPressure;
        private static bool[] cShowScale = new bool[4];
        private static bool cShowSwitches;
        private static double cSimSpeed = 0;
        private static SpeedType cSpeedMode = SpeedType.GPS;
        private static bool cUseDualAuto;
        private static bool cUseMetric;
        private static bool cUseRateDisplay = false;
        private static bool cUseVariableRate = false;
        private static bool cUseZones = false;
        private static string[] LanguageIDs = new string[] { "en", "de", "hu", "nl", "pl", "ru", "fr", "lt" };
        private static string lastMessage = "";
        private static DateTime lastMessageTime = DateTime.MinValue;

        #region pressure calibration

        // cal 0    module 0, low voltage
        // cal 1    module 0, low pressure
        // cal 2    module 0, high voltage
        // cal 3    module 0, high pressure
        // cal 4    module 0, minimum voltage, below is 0 pressure
        // continues for each module, up to 8 modules

        private static double[] PressureCals = new double[40];

        #endregion pressure calibration

        #region flow adjustment defaults

        public static readonly byte BrakePointDefault = 35;
        public static readonly byte DeadbandDefault = 15;
        public static readonly byte KIdefault = 65;
        public static readonly byte KPdefault = 65;
        public static readonly byte MaxIntegralDefault = 250;
        public static readonly byte MaxPWMdefault = 100;
        public static readonly byte MinPWMdefault = 5;
        public static readonly byte PIDslowAdjustDefault = 80;
        public static readonly byte PIDtimeDefault = 100;
        public static readonly UInt16 PulseMaxHzDefault = 3000;
        public static readonly byte PulseMinHzDefault = 10;
        public static readonly byte PulseSampleSizeDefault = 12;
        public static readonly byte SlewRateDefault = 25;
        public static readonly byte TimedAdjustDefault = 80;
        public static readonly byte TimedMinStartDefault = 50;
        public static readonly UInt16 TimedPauseDefault = 400;

        #endregion flow adjustment defaults

        public static event EventHandler UnitsChanged;

        #region MainProperties

        private static clsJobDataCollector cJobCollector;

        public static string ApplicationFolder
        { get { return cApplicationFolder; } }

        public static CanDriver CurrentCanDriver
        {
            get { return cCurrentCanDriver; }
            set
            {
                if (cCurrentCanDriver != value)
                {
                    cCurrentCanDriver = value;
                    SetAppProp("CanDriver", cCurrentCanDriver.ToString());
                }
            }
        }

        public static string CurrentMenuName
        {
            get { return cCurrentMenuName; }
            set
            {
                cCurrentMenuName = value;
            }
        }

        public static int CurrentProduct
        {
            get { return cCurrentProduct; }
            set
            {
                if (value >= 0 && value < MaxProducts - 2)
                {
                    cCurrentProduct = value;
                    SetProp("CurrentProduct", cCurrentProduct.ToString());
                }
            }
        }

        public static string DefaultDir
        { get { return cDefaultDir; } }

        public static int DefaultProduct
        {
            get { return cDefaultProduct; }
            set
            {
                if (value >= 0 && value < MaxProducts - 2)
                {
                    cDefaultProduct = value;
                    SetProp("DefaultProduct", cDefaultProduct.ToString());
                }
            }
        }

        public static string FieldNamesPath
        { get { return cFieldNames; } }

        public static bool IsobusEnabled
        {
            get { return cIsobusEnabled; }
            set
            {
                cIsobusEnabled = value;
                SetAppProp("IsobusEnabled", cIsobusEnabled.ToString());
            }
        }

        public static clsJobDataCollector JobCollector
        {
            get
            {
                if (cJobCollector == null) cJobCollector = new clsJobDataCollector();
                return cJobCollector;
            }
        }

        public static MasterSwitchMode MasterSwitchMode
        {
            get { return cMasterSwitchMode; }
            set
            {
                cMasterSwitchMode = value;
                SetProp("MasterSwitchMode", cMasterSwitchMode.ToString());
            }
        }

        public static int NextSensorSettingsID
        {
            get
            {
                cSensorSettingsMaxID++;
                SetProp("SensorSettingsMaxID", cSensorSettingsMaxID.ToString());
                return cSensorSettingsMaxID;
            }
        }

        public static int PrimeDelay
        {
            get { return cPrimeDelay; }
            set
            {
                if (value >= 3 && value < 9)
                {
                    cPrimeDelay = value;
                    SetProp("PrimeDelay", cPrimeDelay.ToString());
                }
            }
        }

        public static double PrimeTime
        {
            get { return cPrimeTime; }
            set
            {
                if (value >= 0 && value <= 300)
                {
                    cPrimeTime = value;
                    SetProp("PrimeTime", cPrimeTime.ToString());
                }
            }
        }

        public static string ProfilesFolder
        { get { return cProfilesFolder; } }

        public static bool RateCalibrationOn
        {
            get { return cRateCalibrationOn; }
            set { cRateCalibrationOn = value; }
        }

        public static bool ReadOnly
        {
            get { return cReadOnly; }
            set
            {
                bool Changed = cReadOnly != value;
                cReadOnly = value;
                if (Changed) Save(cProps, Properties.Settings.Default.CurrentFile);
            }
        }

        public static bool ResumeAfterPrime
        {
            get { return cResumeAfterPrime; }
            set
            {
                cResumeAfterPrime = value;
                SetProp("ResumeAfterPrime", cResumeAfterPrime.ToString());
            }
        }

        public static bool ShowCanDiagnostics
        {
            get { return cShowCanDiagnostics; }
            set
            {
                if (cShowCanDiagnostics != value)
                {
                    cShowCanDiagnostics = value;
                    SetAppProp("CanDiagnostics", cShowCanDiagnostics.ToString());
                }
            }
        }

        public static string CanPort
        {
            get { return cCanPort; }
            set
            {
                if (cCanPort != value)
                {
                    cCanPort = value;
                    SetAppProp("CanPort", cCanPort);
                }
            }
        }

        public static bool ShowCoverageRemaining
        {
            get { return cShowCoverageRemaining; }
            set
            {
                cShowCoverageRemaining = value;
                SetProp("ShowCoverageRemaining", cShowCoverageRemaining.ToString());
            }
        }

        public static bool ShowPressure
        {
            get { return cShowPressure; }
            set
            {
                cShowPressure = value;
                SetAppProp("ShowPressure", value.ToString());
                DisplayPressure();
            }
        }

        public static bool ShowQuantityRemaining
        {
            get { return cShowQuantityRemaining; }
            set
            {
                cShowQuantityRemaining = value;
                SetProp("ShowQuantityRemaining", cShowQuantityRemaining.ToString());
            }
        }

        public static bool ShowSwitches
        {
            get { return cShowSwitches; }
            set
            {
                cShowSwitches = value;
                Props.SetAppProp("ShowSwitches", cShowSwitches.ToString());
                DisplaySwitches();
            }
        }

        public static double SimSpeed_KMH
        {
            get { return cSimSpeed; }
            set
            {
                if (value >= 0 && value < 40)
                {
                    cSimSpeed = value;
                    SetAppProp("SimSpeed", cSimSpeed.ToString());
                }
            }
        }

        public static double Speed_KMH
        {
            get
            {
                double Result = 0;
                if (Core.SectionControl.PrimeOn)
                {
                    Result = SimSpeed_KMH;
                }
                else
                {
                    switch (cSpeedMode)
                    {
                        case SpeedType.Wheel:
                            if (UseMetric)
                            {
                                Result = Core.ModulesStatus.WheelSpeed(Core.WheelSpeed.WheelModule);
                            }
                            else
                            {
                                Result = Core.ModulesStatus.WheelSpeed(Core.WheelSpeed.WheelModule) * MPHtoKPH;
                            }
                            break;

                        case SpeedType.Simulated:
                            Result = SimSpeed_KMH;
                            break;

                        case SpeedType.ISOBUS:
                            if (Core.IsobusComm != null && Core.IsobusComm.SpeedValid)
                            {
                                Result = Core.IsobusComm.Speed_KMH;
                            }
                            else
                            {
                                // Fallback to GPS if ISOBUS speed not available
                                Result = Core.GPS.Speed_KMH;
                            }
                            break;

                        default:
                            Result = Core.GPS.Speed_KMH;
                            break;
                    }
                }
                if (Result < 0 || Result > 10000) Result = 0;
                return Result;
            }
        }

        public static SpeedType SpeedMode
        {
            get { return cSpeedMode; }
            set
            {
                cSpeedMode = value;
                SetAppProp("SpeedMode", cSpeedMode.ToString());
            }
        }

        public static bool UseDualAuto
        {
            get { return cUseDualAuto; }
            set
            {
                cUseDualAuto = value;
                SetAppProp("UseDualAuto", cUseDualAuto.ToString());
            }
        }

        public static bool UseMapPreview
        {
            get { return cMapPreview; }
            set
            {
                if (cMapPreview != value)
                {
                    cMapPreview = value;
                    SetAppProp("MapPreview", cMapPreview.ToString());
                }
            }
        }

        public static bool UseMetric
        {
            get { return cUseMetric; }
            set
            {
                if (cUseMetric != value)
                {
                    cUseMetric = value;
                    SetAppProp("UseMetric", cUseMetric.ToString());
                    UnitsChanged?.Invoke(null, EventArgs.Empty);
                }
            }
        }

        public static bool UseRateDisplay
        {
            get { return cUseRateDisplay; }
            set
            {
                cUseRateDisplay = value;
                SetAppProp("UseRateDisplay", cUseRateDisplay.ToString());
                DisplayRate();
            }
        }


        public static bool UseZones
        {
            get { return cUseZones; }
            set
            {
                cUseZones = value;
                SetProp("UseZones", cUseZones.ToString());
            }
        }

        public static bool VariableRateEnabled
        {
            get { return cUseVariableRate; }
            set
            {
                cUseVariableRate = value;
                SetProp("UseVariableRate_" + CurrentFileName(), cUseVariableRate.ToString());
            }
        }

        public static string AppVersion()
        {
            return cAppVersion;
        }

        public static string ControlTypeDescription(ControlTypeEnum CT)
        {
            string Result = "";
            switch (CT)
            {
                case ControlTypeEnum.Valve:
                    Result = Lang.lgStandard;
                    break;

                case ControlTypeEnum.ComboClose:
                    Result = Lang.lgComboClose;
                    break;

                case ControlTypeEnum.Motor:
                    Result = Lang.lgMotor;
                    break;

                case ControlTypeEnum.MotorWeights:
                    Result = Lang.lgMotorWeight;
                    break;

                case ControlTypeEnum.Fan:
                    Result = Lang.lgFan;
                    break;

                case ControlTypeEnum.ComboCloseTimed:
                    Result = Lang.lgComboTimed;
                    break;
            }
            return Result;
        }

        public static string CurrentFileName()
        {
            return Path.GetFileNameWithoutExtension(Properties.Settings.Default.CurrentFile);
        }

        public static string ParseDate(string input)
        {
            // input = ddmmy, no leading 0
            // output = v2025.7.1, year, month, day
            string Result = "";
            if (input.Length > 3)
            {
                int YR = int.Parse(input.Substring(input.Length - 1)) + 2020;
                int MN = int.Parse(input.Substring(input.Length - 3, 2));
                int DY = int.Parse(input.Substring(0, input.Length - 3));

                if (DY > 0 && DY < 32 && MN > 0 && MN < 13)
                {
                    Result = "v" + YR.ToString() + "." + MN.ToString("D2") + "." + DY.ToString("D2");
                }
            }
            return Result;
        }

        public static void ShowMessage(string Message, string Title = "Help", int timeInMsec = 20000, bool LogError = false, bool Modal = false, bool PlayErrorSound = false)
        {
            if (!LogError || Message != lastMessage || (DateTime.Now - lastMessageTime).TotalSeconds > 60)
            {
                var Hlp = new frmHelp(Message, Title, timeInMsec);
                if (Modal)
                {
                    Hlp.ShowDialog();
                }
                else
                {
                    Hlp.Show();
                }

                if (LogError) Props.WriteErrorLog(Message);
                if (PlayErrorSound) SystemSounds.Exclamation.Play();

                lastMessage = Message;
                lastMessageTime = DateTime.Now;
            }
        }

        public static string VersionDate()
        {
            return cAppDate;
        }

        #endregion MainProperties

        public static bool CheckFolders()
        {
            bool Result = false;
            try
            {
                // check for default dir and files
                cDefaultDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + cAppName;
                if (!Directory.Exists(cDefaultDir)) Directory.CreateDirectory(cDefaultDir);

                // application folder
                string name = cDefaultDir + "\\Application";
                if (!Directory.Exists(name)) Directory.CreateDirectory(name);
                cApplicationFolder = name;

                // profiles folder
                name = cDefaultDir + "\\Profiles";
                if (!Directory.Exists(name)) Directory.CreateDirectory(name);
                cProfilesFolder = name;

                string DefaultProfile = name + "\\" + "Default";
                if (!Directory.Exists(DefaultProfile)) Directory.CreateDirectory(DefaultProfile);
                if (!File.Exists(DefaultProfile + "\\Default.rcs")) File.WriteAllBytes(DefaultProfile + "\\Default.rcs", Properties.Resources.Default);
                if (!File.Exists(DefaultProfile + "\\DefaultPressureData.csv")) File.WriteAllText(DefaultProfile + "\\DefaultPressureData.csv", string.Empty);

                string ExampleProfile = name + "\\" + "Example";
                if (!Directory.Exists(ExampleProfile)) Directory.CreateDirectory(ExampleProfile);
                if (!File.Exists(ExampleProfile + "\\Example.rcs")) File.WriteAllBytes(ExampleProfile + "\\Example.rcs", Properties.Resources.Example);
                if (!File.Exists(ExampleProfile + "\\ExamplePressureData.csv")) File.WriteAllText(ExampleProfile + "\\ExamplePressureData.csv", string.Empty);

                // check user files, current profile
                if (!File.Exists(Properties.Settings.Default.CurrentFile))
                {
                    Properties.Settings.Default.CurrentFile = ExampleProfile + "\\Example.rcs";
                    Properties.Settings.Default.Save();
                }

                // create field names path
                cFieldNames = Path.Combine(ApplicationFolder, "FieldNames.txt");

                Result = true;
            }
            catch (Exception)
            {
            }
            return Result;
        }

        public static void DisplayMapPreview()
        {
            if (cMapPreview)
            {
                FormManager.ShowForm(new frmMap());
            }
            else
            {
                FormManager.CloseForm<frmMap>();
            }
        }

        public static void DisplayPressure()
        {
            if (cShowPressure)
            {
                FormManager.ShowForm(new frmPressureDisplay());
            }
            else
            {
                FormManager.CloseForm<frmPressureDisplay>();
            }
        }

        public static void DisplayRate()
        {
            if (cUseRateDisplay)
            {
                FormManager.ShowForm(new frmRate());
            }
            else
            {
                FormManager.CloseForm<frmRate>();
            }
        }

        public static void DisplaySwitches()
        {
            if (cShowSwitches)
            {
                FormManager.ShowForm(new frmSwitches());
            }
            else
            {
                FormManager.CloseForm<frmSwitches>();
            }
        }

        public static void DrawGroupBox(GroupBox box, Graphics g, Color BackColor, Color textColor, Color borderColor, float borderWidth = 1)
        {
            // useage:
            // point the Groupbox paint event to this sub:
            // private void groupBox1_Paint(object sender, PaintEventArgs e)
            //{
            //    GroupBox box = sender as GroupBox;
            // mf.Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Red, 3); // Red border with thickness 3
            //}
            if (box != null)
            {
                using (Brush textBrush = new SolidBrush(textColor))
                using (Pen borderPen = new Pen(borderColor, borderWidth))
                {
                    SizeF strSize = g.MeasureString(box.Text, box.Font);
                    Rectangle rect = new Rectangle(box.ClientRectangle.X,
                                                   box.ClientRectangle.Y + (int)(strSize.Height / 2),
                                                   box.ClientRectangle.Width - 1,
                                                   box.ClientRectangle.Height - (int)(strSize.Height / 2) - 1);

                    // Clear text and border
                    g.Clear(BackColor);

                    // Draw text
                    g.DrawString(box.Text, box.Font, textBrush, box.Padding.Left, 0);

                    // Drawing Border
                    // Left
                    g.DrawLine(borderPen, rect.Location, new Point(rect.X, rect.Y + rect.Height));
                    // Right
                    g.DrawLine(borderPen, new Point(rect.X + rect.Width, rect.Y), new Point(rect.X + rect.Width, rect.Y + rect.Height));
                    // Bottom
                    g.DrawLine(borderPen, new Point(rect.X, rect.Y + rect.Height), new Point(rect.X + rect.Width, rect.Y + rect.Height));
                    // Top1
                    g.DrawLine(borderPen, new Point(rect.X, rect.Y), new Point(rect.X + box.Padding.Left, rect.Y));
                    // Top2
                    g.DrawLine(borderPen, new Point(rect.X + box.Padding.Left + (int)(strSize.Width), rect.Y), new Point(rect.X + rect.Width, rect.Y));
                }
            }
        }

        public static String GetAppProp(string key)
        {
            return cAppProps.TryGetValue(key, out var value) ? value : string.Empty;
            //string prop = cAppProps.TryGetValue(key, out var value) ? value : string.Empty;
            //return int.TryParse(prop, out var vl) ? vl : -1;
        }

        public static double GetPressureCal(int Index)
        {
            return PressureCals[Index];
        }

        public static string GetProp(string key)
        {
            return cProps.TryGetValue(key, out var value) ? value : string.Empty;
        }

        public static bool IsFormNameValid(string formName)
        {
            bool Result = false;
            if (!string.IsNullOrWhiteSpace(formName))
            {
                var assembly = Assembly.GetExecutingAssembly();

                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(Form)) && type.Name.Equals(formName, StringComparison.OrdinalIgnoreCase))
                    {
                        Result = true;
                        break;
                    }
                }
            }
            return Result;
        }

        public static Form IsFormOpen(string Name, bool SetFocus = true)
        {
            Form Result = null;
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.Name == Name)
                {
                    Result = frm;
                    if (SetFocus) frm.Focus();
                    break;
                }
            }
            return Result;
        }

        public static bool IsOnScreen(Control Ctrl, bool MakeOnScreen = true)
        {
            try
            {
                if (Ctrl == null)
                    return false;

                // Thread-safe: marshal to UI thread if needed
                if (Ctrl.InvokeRequired)
                {
                    return (bool)Ctrl.Invoke(new Func<bool>(() => IsOnScreen(Ctrl, MakeOnScreen)));
                }

                // --- Now we are guaranteed to be on the UI thread ---

                // If handle not created yet (e.g., before Show), assume valid
                if (!(Ctrl is Form) && !Ctrl.IsHandleCreated)
                    return true;

                Rectangle rect;

                if (Ctrl is Form frm)
                {
                    rect = new Rectangle(frm.Left, frm.Top, frm.Width, frm.Height);
                }
                else
                {
                    rect = Ctrl.RectangleToScreen(new Rectangle(Point.Empty, Ctrl.Size));
                }

                bool result = Screen.AllScreens.Any(s => s.WorkingArea.IntersectsWith(rect));

                if (!result && MakeOnScreen && Ctrl is Form theForm)
                {
                    CenterForm(theForm);
                }

                return result;
            }
            catch (Exception ex)
            {
                WriteErrorLog("Props/IsOnScreen: " + ex.Message);
                return false;
            }
        }

        public static bool IsPathSafe(string candidatePath)
        {
            bool result = false;
            try
            {
                if (!string.IsNullOrEmpty(candidatePath))
                {
                    string myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    string baseFolder = Path.Combine(myDocuments, "RateController");

                    string candidateFullPath = Path.GetFullPath(candidatePath);
                    string safeBaseFullPath = Path.GetFullPath(baseFolder);

                    // If the candidate is a file, use its parent folder for the containment check.
                    if (File.Exists(candidateFullPath))
                    {
                        candidateFullPath = Path.GetDirectoryName(candidateFullPath);
                    }

                    // Normalize paths
                    candidateFullPath = candidateFullPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    safeBaseFullPath = safeBaseFullPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                    // Check if the candidate path is within the safe base folder
                    string safeBaseWithSeparator = safeBaseFullPath + Path.DirectorySeparatorChar;
                    result = candidateFullPath.StartsWith(safeBaseWithSeparator, StringComparison.OrdinalIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog("Props/IsPathSafe: " + ex.Message);
            }
            return result;
        }

        public static string LanguageDescription(int ID)
        {
            string Result = LanguageIDs[0];

            if (ID >= 0 && ID < LanguageIDs.Count())
            {
                Result = LanguageIDs[ID];
            }

            return Result;
        }

        public static bool LanguageIsSupported(string descriptor)
        {
            bool Result = false;

            for (int i = 0; i < LanguageIDs.Count(); i++)
            {
                if (LanguageIDs[i] == descriptor)
                {
                    Result = true;
                    break;
                }
            }

            return Result;
        }

        public static void LoadFormLocation(Form frm, string Instance = "")
        {
            try
            {
                if (frm == null)
                    return;

                // Thread-safe: marshal to UI thread if needed
                if (frm.InvokeRequired)
                {
                    frm.Invoke(new Action(() => LoadFormLocation(frm, Instance)));
                    return;
                }

                // --- Now we are guaranteed to be on the UI thread ---

                string name = frm.Name + Instance + ".Left";
                int left = int.TryParse(GetAppProp(name), out int vl) ? vl : -1;

                name = frm.Name + Instance + ".Top";
                int top = int.TryParse(GetAppProp(name), out int tp) ? tp : -1;

                if (left == -1 || top == -1)
                {
                    CenterForm(frm);
                }
                else
                {
                    frm.Left = left;
                    frm.Top = top;
                }

                IsOnScreen(frm);
            }
            catch (Exception ex)
            {
                WriteErrorLog("Props/LoadFormLocation: " + ex.Message);
            }
        }

        public static void LoadProperties()
        {
            // profile properties

            cReadOnly = bool.TryParse(GetProp("ReadOnly"), out bool rd) ? rd : false;
            cUseVariableRate = bool.TryParse(GetProp("UseVariableRate_" + CurrentFileName()), out bool vr) ? vr : false;
            cMasterSwitchMode = Enum.TryParse(GetProp("MasterSwitchMode"), out MasterSwitchMode msm) ? msm : MasterSwitchMode.ControlAll;

            cDefaultProduct = int.TryParse(GetProp("DefaultProduct"), out int dp) ? dp : 0;
            CurrentProduct = cDefaultProduct;

            cPrimeDelay = int.TryParse(GetProp("PrimeDelay"), out int pd) ? pd : 3;
            cPrimeTime = int.TryParse(GetProp("PrimeTime"), out int pt) ? pt : 5;
            cResumeAfterPrime = bool.TryParse(GetProp("ResumeAfterPrime"), out bool rp) ? rp : false;
            cUseZones = bool.TryParse(GetProp("UseZones"), out bool uz) ? uz : false;
            cShowQuantityRemaining = bool.TryParse(GetProp("ShowQuantityRemaining"), out bool qr) ? qr : false;
            cShowCoverageRemaining = bool.TryParse(GetProp("ShowCoverageRemaining"), out bool cr) ? cr : false;
            cRateRecordEnabled = bool.TryParse(GetProp("RecordRates"), out bool rc) ? rc : true;
            cSensorSettingsMaxID = int.TryParse(GetProp("SensorSettingsMaxID"), out int mi) ? mi : 0;
            cCurrentMenuName = GetProp("LastScreen");

            bool NewVal = false;
            for (int i = 0; i < cShowScale.Count(); i++)
            {
                cShowScale[i] = bool.TryParse(GetProp("ShowScale_" + i.ToString()), out NewVal) ? NewVal : false;
            }

            // application properties
            cSimSpeed = double.TryParse(GetAppProp("SimSpeed"), out double spd) ? spd : 8;
            cSpeedMode = Enum.TryParse(GetAppProp("SpeedMode"), out SpeedType spt) ? spt : SpeedType.GPS;
            cIsobusEnabled = bool.TryParse(GetAppProp("IsobusEnabled"), out bool ibe) ? ibe : false;
            cUseMetric = bool.TryParse(GetAppProp("UseMetric"), out bool mt) ? mt : false;
            cShowPressure = bool.TryParse(GetAppProp("ShowPressure"), out bool sp) ? sp : false;
            cShowSwitches = bool.TryParse(GetAppProp("ShowSwitches"), out bool ss) ? ss : false;
            cUseDualAuto = bool.TryParse(GetAppProp("UseDualAuto"), out bool da) ? da : false;
            cUseRateDisplay = bool.TryParse(GetAppProp("UseRateDisplay"), out bool rtd) ? rtd : false;
            cMapPreview = bool.TryParse(GetAppProp("MapPreview"), out bool mp) ? mp : false;
            cCurrentCanDriver = Enum.TryParse(GetAppProp("CanDriver"), out CanDriver dr) ? dr : CanDriver.SLCAN;
            cShowCanDiagnostics = bool.TryParse(GetAppProp("CanDiagnostics"), out bool di) ? di : false;
            string port = GetAppProp("CanPort");
            cCanPort = string.IsNullOrEmpty(port) ? "COM7" : port;

            for (int i = 0; i < 40; i++)
            {
                string key = "PressureCal_" + i.ToString();
                if (double.TryParse(GetProp(key), out double pc)) PressureCals[i] = pc;
            }
        }

        public static bool OpenFile(string FileName, bool IsNew = false)
        {
            bool Result = false;
            try
            {
                CheckFolders();

                if (IsNew) File.WriteAllText(FileName, ""); // Create empty property file

                if (File.Exists(FileName))
                {
                    Properties.Settings.Default.CurrentFile = FileName;
                    Properties.Settings.Default.Save();
                    Load(cProps, Properties.Settings.Default.CurrentFile);
                    string CurrentDir = Path.GetDirectoryName(Properties.Settings.Default.CurrentFile);

                    cAppPropsFileName = Path.Combine(ApplicationFolder, "AppData.txt");
                    if (!File.Exists(cAppPropsFileName)) File.WriteAllText(cAppPropsFileName, "");
                    Load(cAppProps, cAppPropsFileName);

                    cErrorsFileName = Path.Combine(ApplicationFolder, "Error Log.txt");
                    if (!File.Exists(cErrorsFileName)) File.WriteAllText(cErrorsFileName, "");

                    cActivityFileName = Path.Combine(ApplicationFolder, "Activity Log.txt");
                    if (!File.Exists(cActivityFileName)) File.WriteAllText(cActivityFileName, "");

                    LoadProperties();
                    Result = true;
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Can not load profile: " + ex.Message);
            }
            return Result;
        }

        public static double PressureReading(int ModuleID, double Reading)
        {
            // y = Mx + B
            // M = (y2-y1)/(x2-x1)
            // B = y - Mx

            double Result = 0;
            try
            {
                double MinVol = GetPressureCal(ModuleID * 5);       // x1
                double MinPres = GetPressureCal(ModuleID * 5 + 1);  // y1
                double MaxVol = GetPressureCal(ModuleID * 5 + 2);   // x2
                double MaxPres = GetPressureCal(ModuleID * 5 + 3);  // y2
                double ZeroValue = GetPressureCal(ModuleID * 5 + 4);
                if ((MaxPres - MinPres) > 0 && Reading >= ZeroValue)
                {
                    double M = (MaxPres - MinPres) / (MaxVol - MinVol);
                    double B = MaxPres - M * MaxVol;
                    Result = M * Reading + B;
                    if (Result < 0) Result = 0;
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog("Props/PressureReading: " + ex.Message);
            }
            return Result;
        }

        public static void SaveFormLocation(Form frm, string Instance = "")
        {
            try
            {
                if (frm == null)
                    return;

                // Thread-safe: marshal to UI thread if needed
                if (frm.InvokeRequired)
                {
                    frm.Invoke(new Action(() => SaveFormLocation(frm, Instance)));
                    return;
                }

                // --- Now we are guaranteed to be on the UI thread ---

                if (frm.WindowState == FormWindowState.Normal)
                {
                    string name = frm.Name + Instance + ".Left";
                    SetAppProp(name, frm.Left.ToString());

                    name = frm.Name + Instance + ".Top";
                    SetAppProp(name, frm.Top.ToString());
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog("Props/SaveFormLocation: " + ex.Message);
            }
        }

        public static void SetAppProp(string key, string value)
        {
            try
            {
                if (value != null)
                {
                    if (!cAppProps.TryGetValue(key, out var existingValue) || existingValue != value)
                    {
                        cAppProps[key] = value;
                        Save(cAppProps, cAppPropsFileName);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog("SetAppProp: " + ex.Message);
            }
        }

        public static void SetPressureCal(int Index, double Value)
        {
            PressureCals[Index] = Value;
            string key = "PressureCal_" + Index.ToString();
            SetProp(key, Value.ToString());
        }

        public static void SetProp(string key, string value, bool IgnoreReadOnly = false)
        {
            try
            {
                if (value != null && (!ReadOnly || IgnoreReadOnly))
                {
                    if (!cProps.TryGetValue(key, out var existingValue) || existingValue != value)
                    {
                        cProps[key] = value;
                        Save(cProps, Properties.Settings.Default.CurrentFile);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog("Props/Set: " + ex.Message);
            }
        }

        public static bool ShowLog(string FileName)
        {
            bool Result = false;
            try
            {
                string Name = cApplicationFolder + "\\" + FileName;
                if (File.Exists(Name))
                {
                    Process.Start(new ProcessStartInfo(Name) { UseShellExecute = true });
                    Result = true;
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog("Tools: OpenTextFile: " + ex.Message);
            }
            return Result;
        }

        public static bool ShowScaleGet(int ID)
        {
            bool result = false;
            if (ID >= 0 && ID < cShowScale.Count())
            {
                result = cShowScale[ID];
            }
            return result;
        }

        public static void ShowScales()
        {
            for (int i = 0; i < cShowScale.Count(); i++)
            {
                if (cShowScale[i])
                {
                    FormManager.ShowForm(new frmScaleDisplay(i), i.ToString());
                }
                else
                {
                    FormManager.CloseForm<frmScaleDisplay>(i.ToString());
                }
            }
        }

        public static void ShowScaleSet(int ID, bool NewVal)
        {
            if (ID >= 0 && ID < cShowScale.Count())
            {
                cShowScale[ID] = NewVal;
                SetProp("ShowScale_" + ID.ToString(), cShowScale[ID].ToString());
            }
            ShowScales();
        }

        public static void WriteActivityLog(string Message, bool Newline = false, bool NoDate = false)
        {
            string Line = "";
            string DF;
            try
            {
                TrimFile(cActivityFileName);

                if (Newline) Line = "\r\n";

                if (NoDate)
                {
                    DF = "hh:mm:ss";
                }
                else
                {
                    DF = "MMM-dd hh:mm:ss";
                }

                File.AppendAllText(cActivityFileName, Line + DateTime.Now.ToString(DF) + "  -  " + Message + "\r\n");
            }
            catch (Exception ex)
            {
                WriteErrorLog("Props/WriteActivityLog: " + ex.Message);
            }
        }

        public static void WriteErrorLog(string error)
        {
            try
            {
                TrimFile(cErrorsFileName);
                File.AppendAllText(cErrorsFileName, DateTime.Now.ToString("MMM-dd hh:mm:ss") + "  -  " + error + "\r\n\r\n");
            }
            catch (Exception)
            {
            }
        }

        public static void WriteLog(string LogName, string Message, bool NewLine = false, bool UseDate = false)
        {
            string Line = "";
            string DF = "";
            if (Message == null) Message = "";
            try
            {
                string FileName = cApplicationFolder + "\\" + LogName;
                TrimFile(FileName);

                if (NewLine) Line = "\r\n";

                if (UseDate)
                {
                    DF = "MMM-dd hh:mm:ss";
                }
                else
                {
                    DF = "hh:mm:ss";
                }

                if (UseDate || Message.Length > 0)
                {
                    File.AppendAllText(FileName, Line + DateTime.Now.ToString(DF) + "  -  " + Message + "\r\n");
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog("Tools: WriteLog: " + ex.Message);
            }
        }

        private static void CenterForm(Form form)
        {
            try
            {
                if (form == null)
                    return;

                // Thread-safe: marshal to UI thread if needed
                if (form.InvokeRequired)
                {
                    form.Invoke(new Action(() => CenterForm(form)));
                    return;
                }

                // --- Now we are guaranteed to be on the UI thread ---

                int screenWidth = Screen.PrimaryScreen.Bounds.Width;
                int screenHeight = Screen.PrimaryScreen.Bounds.Height;

                int formWidth = form.Width;
                int formHeight = form.Height;

                int xPosition = (screenWidth / 2) - (formWidth / 2);
                int yPosition = (screenHeight / 2) - (formHeight / 2);

                form.Location = new Point(xPosition, yPosition);
            }
            catch (Exception ex)
            {
                WriteErrorLog("Props/CenterForm: " + ex.Message);
            }
        }

        private static void Load(SortedDictionary<string, string> PropsDct, string DctPath)
        {
            try
            {
                PropsDct.Clear();
                foreach (var line in File.ReadLines(DctPath))
                {
                    int separatorIndex = line.IndexOf('=');
                    if (separatorIndex > 0 && separatorIndex < line.Length - 1)
                    {
                        string key = line.Substring(0, separatorIndex).Trim();
                        string value = line.Substring(separatorIndex + 1).Trim();
                        PropsDct[key] = value;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog("Props/Load: " + ex.Message);
            }
        }

        private static void Save(SortedDictionary<string, string> PropsDct, string DctPath)
        {
            try
            {
                if (PropsDct.Count > 0 || !string.IsNullOrEmpty(DctPath))
                {
                    var lines = new List<string>();
                    foreach (var pair in PropsDct)
                    {
                        lines.Add($"{pair.Key}={pair.Value}");
                    }

                    File.WriteAllLines(DctPath, lines);
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog("Props/Save: " + ex.Message);
            }
        }

        private static void TrimFile(string FileName, int MaxSize = 100000)
        {
            try
            {
                if (File.Exists(FileName))
                {
                    long FileSize = new FileInfo(FileName).Length;
                    if (FileSize > MaxSize)
                    {
                        // trim file
                        string[] Lines = File.ReadAllLines(FileName);
                        int Len = (int)Lines.Length;
                        int St = (int)(Len * .1); // skip first 10% of old lines
                        string[] NewLines = new string[Len - St];
                        Array.Copy(Lines, St, NewLines, 0, Len - St);
                        File.Delete(FileName);
                        File.AppendAllLines(FileName, NewLines);
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }

    public static class SubMenuLayout
    {
        public const int ButtonLeftOffset = 458;

        public const int ButtonSpacing = 76;

        public const int ButtonTopOffset = 603;

        public const int HeightOffset = -2;

        public const int LeftOffset = 240;

        public const int MainMenuHeight = 680;

        // main menu 782,680
        // sub menu 540,598
        // button 70,63
        public const int MainMenuWidth = 782;

        public const int TopOffset = 1;
        public const int WidthOffset = -242;

        public static void SetFormLayout(Form Sub, Form Main, Button btn)
        {
            try
            {
                if (Sub == null || Main == null)
                    return;

                // Thread-safe: marshal to UI thread if needed
                if (Sub.InvokeRequired)
                {
                    Sub.Invoke(new Action(() => SetFormLayout(Sub, Main, btn)));
                    return;
                }

                // --- Now we are guaranteed to be on the UI thread ---

                Sub.Left = Main.Left + LeftOffset;
                Sub.Top = Main.Top + TopOffset;
                Sub.Width = Main.Width + WidthOffset;
                Sub.Height = Main.Height + HeightOffset;

                if (btn != null)
                {
                    btn.Left = ButtonLeftOffset;
                    btn.Top = ButtonTopOffset;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("Props/SetFormLayout: " + ex.Message);
            }
        }
    }
}