using GMap.NET.MapProviders;
using RateController.Forms;
using RateController.Language;
using RateController.Menu;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Security.Policy;
using System.Windows.Forms;

namespace RateController.Classes
{
    public enum ApplicationMode
    { ControlledUPM, ConstantUPM, DocumentApplied, DocumentTarget }

    public enum CalibrationMode
    { Off, SettingPWM, TestingRate }

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

    public enum SimType
    { Sim_None, Sim_Speed }

    public enum SwIDs
    {
        // 0 Used to be AutoSwitch. It was replaced with AutoSection and AutoRate.
        NotUsed, MasterOn, MasterOff, RateUp, RateDown, sw0, sw1, sw2, sw3, sw4, sw5,

        sw6, sw7, sw8, sw9, sw10, sw11, sw12, sw13, sw14, sw15, AutoSection, AutoRate, WorkSwitch
    };

    public static class Props
    {
        public static readonly int MaxModules = 8;
        public static readonly int MaxProducts = 6;
        public static readonly int MaxRelays = 16;
        public static readonly int MaxSections = 128;
        public static readonly int MaxSensorsPerModule = 16;
        public static readonly int MaxSwitches = 16;
        public static readonly double MPHtoKPH = 1.6092;
        public static bool cShowCoverageRemaining;
        public static bool cShowQuantityRemaining;
        private static string cActivityFileName = "";
        private static string cAppDate = "26-Oct-2025";
        private static string cApplicationFolder;
        private static string cAppName = "RateController";
        private static string cAppVersion = "4.1.2";
        private static string cCurrentMenuName;
        private static int cDefaultProduct;
        private static string cErrorsFileName = "";
        private static string cFieldNames;
        private static SortedDictionary<string, string> cFormProps = new SortedDictionary<string, string>();
        private static string cFormPropsFileName = "";
        private static bool cShowJobs;
        private static string cJobsDataPath;
        private static string cJobsFolder;
        private static bool cMapShowRates;
        private static bool cMapShowTiles;
        private static bool cMapShowZones;
        private static MasterSwitchMode cMasterSwitchMode = MasterSwitchMode.ControlAll;
        private static int cPrimeDelay = 3;
        private static double cPrimeTime = 0;
        private static string cProfilesFolder;
        private static SortedDictionary<string, string> cProps = new SortedDictionary<string, string>();
        private static bool cRateCalibrationOn = false;
        private static double cRateDisplayResolution;
        private static bool cRateRecordEnabled;
        private static int cRateType;
        private static bool cReadOnly = false;
        private static bool cResumeAfterPrime;
        private static bool cShowPressure;
        private static bool cShowSwitches;
        private static SimType cSimMode = SimType.Sim_None;
        private static double cSimSpeed = 0;
        private static bool cUseDualAuto;
        private static bool cUseLargeScreen = false;
        private static bool cUseMetric;
        private static bool cUseRateDisplay = false;
        private static bool cUseTransparent = false;
        private static bool cUseVariableRate = false;
        private static bool cUseZones = false;
        private static string lastMessage = "";
        private static DateTime lastMessageTime = DateTime.MinValue;
        private static FormStart mf;
        private static frmPressureDisplay PressureDisplay;
        private static frmRate RateDisplay;
        private static frmSwitches SwitchesForm;

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
        public static readonly byte KIdefault = 5;
        public static readonly byte KPdefault = 35;
        public static readonly byte MaxIntegralDefault = 1;
        public static readonly byte MaxPWMdefault = 100;
        public static readonly byte MinPWMdefault = 0;
        public static readonly byte PIDslowAdjustDefault = 30;
        public static readonly byte PIDtimeDefault = 100;
        public static readonly UInt16 PulseMaxHzDefault = 3000;
        public static readonly byte PulseMinHzDefault = 10;
        public static readonly byte PulseSampleSizeDefault = 12;
        public static readonly byte SlewRateDefault = 15;
        public static readonly byte TimedAdjustDefault = 80;
        public static readonly byte TimedMinStartDefault = 3;
        public static readonly UInt16 TimedPauseDefault = 400;

        #endregion flow adjustment defaults

        public static event EventHandler JobChanged;

        public static event EventHandler RateDataSettingsChanged;

        public static event EventHandler ScreensSwitched;

        public static event EventHandler UnitsChanged;

        #region MainProperties

        public static event EventHandler ResolutionChanged;

        public static string ApplicationFolder
        { get { return cApplicationFolder; } }

        public static string CurrentJobDescription
        {
            get
            {
                Job current = JobManager.SearchJob(Props.CurrentJobID);
                string fld = "";
                Parcel currentParcel = ParcelManager.SearchParcel(current.FieldID);
                if (currentParcel != null && currentParcel.Name.Trim()!="") fld = " - " + currentParcel.Name;
                return current.Name + fld;
            }
        }

        public static int CurrentJobID
        {
            get
            {
                Job current = JobManager.SearchJob(Properties.Settings.Default.CurrentJob);
                if (current == null)
                {
                    Properties.Settings.Default.CurrentJob = 0;
                    Properties.Settings.Default.Save();
                    current = JobManager.SearchJob(0);
                    SaveJobInfo();
                }
                return Properties.Settings.Default.CurrentJob;
            }
            set
            {
                Properties.Settings.Default.CurrentJob = value;
                Properties.Settings.Default.Save();
                JobChanged?.Invoke(null, EventArgs.Empty);
                SaveJobInfo();
            }
        }

        public static string CurrentMapPath
        {
            get
            {
                return JobManager.MapPath(Properties.Settings.Default.CurrentJob);
            }
        }

        public static string CurrentRateDataPath
        {
            get
            {
                return JobManager.RateDataPath(Properties.Settings.Default.CurrentJob);
            }
        }

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

        public static string JobsDataPath
        { get { return cJobsDataPath; } }

        public static string JobsFolder
        { get { return cJobsFolder; } }

        public static FormStart MainForm
        {
            get { return mf; }
            set { mf = value; }
        }

        public static string MapCache
        { get { return cApplicationFolder + "\\MapCache"; } }

        public static bool MapShowRates
        {
            get { return cMapShowRates; }
            set
            {
                cMapShowRates = value;
                SetProp("MapShowRates", cMapShowRates.ToString());
            }
        }

        public static bool MapShowTiles
        {
            get { return cMapShowTiles; }
            set
            {
                cMapShowTiles = value;
                SetProp("ShowTiles", cMapShowTiles.ToString());
            }
        }

        public static bool MapShowZones
        {
            get { return cMapShowZones; }
            set
            {
                cMapShowZones = value;
                SetProp("MapShowZones", cMapShowZones.ToString());
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

        public static int RateDisplayProduct
        {
            get { return int.TryParse(GetProp("RatesProduct"), out int rs) ? rs : 0; }
            set { SetProp("RatesProduct", value.ToString()); }
        }

        public static double RateDisplayResolution
        {
            get { return double.TryParse(GetProp("RateDisplayResolution"), out double rs) ? rs : 0.5; }
            set
            {
                SetProp("RateDisplayResolution", value.ToString());

                if (cRateDisplayResolution != value)
                {
                    cRateDisplayResolution = value;
                    ResolutionChanged?.Invoke(null, EventArgs.Empty);
                }
            }
        }

        public static RateType RateDisplayType
        {
            get { return Enum.TryParse(GetProp("RateDisplayType"), out RateType tp) ? tp : RateType.Applied; }
            set { SetProp("RateDisplayType", value.ToString()); }
        }

        public static bool RateRecordEnabled
        {
            get { return cRateRecordEnabled; }
            set
            {
                cRateRecordEnabled = value;
                SetProp("RecordRates", value.ToString());
            }
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
                SetProp("ShowPressure", value.ToString());
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
                Props.SetProp("ShowSwitches", cShowSwitches.ToString());
                DisplaySwitches();
            }
        }

        public static SimType SimMode
        {
            get { return cSimMode; }
            set
            {
                cSimMode = value;
                SetProp("SimMode", cSimMode.ToString());
            }
        }

        public static double SimSpeed
        {
            get { return cSimSpeed; }
            set
            {
                if (value >= 0 && value < 40)
                {
                    cSimSpeed = value;
                    SetProp("SimSpeed", cSimSpeed.ToString());
                }
            }
        }

        public static bool UseDualAuto
        {
            get { return cUseDualAuto; }
            set
            {
                cUseDualAuto = value;
                SetProp("UseDualAuto", cUseDualAuto.ToString());
            }
        }

        public static bool ShowJobs
        {
            get { return cShowJobs; }
            set
            {
                cShowJobs = value;
                SetProp("ShowJobs", cShowJobs.ToString());
            }
        }

        public static bool UseLargeScreen
        {
            get { return cUseLargeScreen; }
            set
            {
                if (cUseLargeScreen != value)
                {
                    cUseLargeScreen = value;
                    SetProp("UseLargeScreen", cUseLargeScreen.ToString());
                    SwitchScreens();
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
                    SetProp("UseMetric", cUseMetric.ToString());
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
                SetProp("UseRateDisplay", cUseRateDisplay.ToString());
                DisplayRate();
            }
        }

        public static int UserRateType
        {
            // to show user
            // 0 current rate, 1 instantaneous rate
            get { return cRateType; }
            set
            {
                if (value >= 0 && value < 2)
                {
                    cRateType = value;
                    SetProp("UserRateType", cRateType.ToString());
                }
            }
        }

        public static bool UseTransparent
        {
            get { return cUseTransparent; }
            set
            {
                cUseTransparent = value;
                SetProp("UseTransparent", cUseTransparent.ToString());
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

        public static string CurrentDir()
        {
            return Path.GetDirectoryName(Properties.Settings.Default.CurrentFile);
        }

        public static string CurrentFileName()
        {
            return Path.GetFileNameWithoutExtension(Properties.Settings.Default.CurrentFile);
        }

        public static string CurrentPressureFile()
        {
            string name = CurrentDir() + "\\" + CurrentFileName() + "PressureData.csv";
            return name;
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

        public static void ShowMessage(string Message, string Title = "Help",
                                                                                                                                                                                                                                                                                                                                                                            int timeInMsec = 20000, bool LogError = false, bool Modal = false
            , bool PlayErrorSound = false)
        {
            if (!LogError || Message != lastMessage || (DateTime.Now - lastMessageTime).TotalSeconds > 60)
            {
                var Hlp = new frmHelp(mf, Message, Title, timeInMsec);
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

        private static void SaveJobInfo()
        {
            string InfoPath = cJobsFolder + "\\CurrentJob.txt";
            string JobPath = "";
            Job current = JobManager.SearchJob(Properties.Settings.Default.CurrentJob);
            if (current != null)
            {
                JobPath = current.JobFolder;
            }
            File.WriteAllText(InfoPath, JobPath);
        }

        #endregion MainProperties

        public static string CurrentMenuName
        {
            get { return cCurrentMenuName; }
            set { cCurrentMenuName = value; }
        }

        public static bool CheckFolders()
        {
            bool Result = false;
            try
            {
                // check for default dir and files
                string cDefaultDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + cAppName;
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
                if (!File.Exists(ExampleProfile + "\\Example.rcs")) File.WriteAllBytes(ExampleProfile + "\\Example.rcs", Properties.Resources.Default);
                if (!File.Exists(ExampleProfile + "\\ExamplePressureData.csv")) File.WriteAllText(ExampleProfile + "\\ExamplePressureData.csv", string.Empty);

                // check user files, current profile
                if (!File.Exists(Properties.Settings.Default.CurrentFile))
                {
                    Properties.Settings.Default.CurrentFile = DefaultProfile + "\\Default.rcs";
                    Properties.Settings.Default.Save();
                }

                // jobs folder
                name = cDefaultDir + "\\Jobs";
                if (!Directory.Exists(name)) Directory.CreateDirectory(name);
                cJobsFolder = name;

                cJobsDataPath = cJobsFolder + "\\JobsData.jbs";
                if (!File.Exists(cJobsDataPath)) File.WriteAllText(cJobsDataPath, string.Empty);

                // check for default job
                JobManager.CheckDefaultJob();

                // check user files, current job
                int CurrentJob = Props.CurrentJobID;
                if (JobManager.SearchJob(CurrentJob) == null)
                {
                    Props.CurrentJobID = 0;
                }
                else
                {
                    Props.CurrentJobID = Properties.Settings.Default.CurrentJob;
                }

                // create field names path
                cFieldNames = Path.Combine(ApplicationFolder, "FieldNames.txt");

                SaveJobInfo();

                Result = true;
            }
            catch (Exception)
            {
            }
            return Result;
        }

        public static bool CheckOnScreen(Form form, bool MakeOnScreen = true)
        {
            bool IsOnScreen = false;
            try
            {
                // Create rectangle
                Rectangle formRectangle = new Rectangle(form.Left, form.Top, form.Width, form.Height);

                // Test
                IsOnScreen = Screen.AllScreens.Any(s => s.WorkingArea.IntersectsWith(formRectangle));

                if (!IsOnScreen && MakeOnScreen) CenterForm(form);
            }
            catch (Exception ex)
            {
                WriteErrorLog("Props/CheckOnScreen: " + ex.Message);
            }
            return IsOnScreen;
        }

        public static void DisplayPressure()
        {
            Form fs = IsFormOpen("frmPressureDisplay");

            if (cShowPressure)
            {
                if (fs == null)
                {
                    PressureDisplay = new frmPressureDisplay(mf);
                    PressureDisplay.Show();
                }
            }
            else
            {
                if (fs != null) fs.Close();
            }
        }

        public static void DisplayRate()
        {
            Form fs = IsFormOpen("frmRate");
            if (cUseRateDisplay)
            {
                if (fs == null)
                {
                    RateDisplay = new frmRate(mf);
                    RateDisplay.Show();
                }
            }
            else
            {
                if (fs != null) fs.Close();
            }
        }

        public static void DisplaySwitches()
        {
            Form fs = Props.IsFormOpen("frmSwitches");

            if (cShowSwitches)
            {
                if (fs == null)
                {
                    SwitchesForm = new frmSwitches(mf);
                    SwitchesForm.Show();
                }
            }
            else
            {
                if (fs != null) fs.Close();
            }
        }

        public static void DrawGroupBox(GroupBox box, Graphics g, Color BackColor, Color textColor, Color borderColor)
        {
            // useage:
            // point the Groupbox paint event to this sub:
            //private void GroupBoxPaint(object sender, PaintEventArgs e)
            //{
            //    GroupBox box = sender as GroupBox;
            //    Props.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);
            //}

            if (box != null)
            {
                Brush textBrush = new SolidBrush(textColor);
                Brush borderBrush = new SolidBrush(borderColor);
                Pen borderPen = new Pen(borderBrush);
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
                //Left
                g.DrawLine(borderPen, rect.Location, new Point(rect.X, rect.Y + rect.Height));
                //Right
                g.DrawLine(borderPen, new Point(rect.X + rect.Width, rect.Y), new Point(rect.X + rect.Width, rect.Y + rect.Height));
                //Bottom
                g.DrawLine(borderPen, new Point(rect.X, rect.Y + rect.Height), new Point(rect.X + rect.Width, rect.Y + rect.Height));
                //Top1
                g.DrawLine(borderPen, new Point(rect.X, rect.Y), new Point(rect.X + box.Padding.Left, rect.Y));
                //Top2
                g.DrawLine(borderPen, new Point(rect.X + box.Padding.Left + (int)(strSize.Width), rect.Y), new Point(rect.X + rect.Width, rect.Y));
            }
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
            // Get the current assembly
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Check if a type with the given name exists and inherits from Form
            var formType = assembly.GetTypes().FirstOrDefault(t => t.Name == formName && t.IsSubclassOf(typeof(Form)));
            return formType != null;
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

        public static bool IsPathSafeToDelete(string candidatePath)
        {
            bool result = false;
            try
            {
                string myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string baseFolder = Path.Combine(myDocuments, "RateController");

                if (!string.IsNullOrEmpty(candidatePath))
                {
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
                WriteErrorLog("Props/SafeToDelete: " + ex.Message);
            }
            return result;
        }

        public static void LoadFormLocation(Form frm, string Instance = "")
        {
            try
            {
                string name = frm.Name + Instance + ".Left";
                int Left = GetFormProp(name);

                name = frm.Name + Instance + ".Top";
                int Top = GetFormProp(name);

                if (Left == -1 || Top == -1)
                {
                    CenterForm(frm);
                }
                else
                {
                    frm.Left = Left;
                    frm.Top = Top;
                }
                CheckOnScreen(frm);
            }
            catch (Exception ex)
            {
                WriteErrorLog("Props/LoadFormLocation: " + ex.Message);
            }
        }

        public static void LoadProperties()
        {
            cReadOnly = bool.TryParse(GetProp("ReadOnly"), out bool rd) ? rd : false;
            cUseVariableRate = bool.TryParse(GetProp("UseVariableRate_" + CurrentFileName()), out bool vr) ? vr : false;
            cMasterSwitchMode = Enum.TryParse(GetProp("MasterSwitchMode"), out MasterSwitchMode msm) ? msm : MasterSwitchMode.ControlAll;
            cDefaultProduct = int.TryParse(GetProp("DefaultProduct"), out int dp) ? dp : 0;
            cPrimeDelay = int.TryParse(GetProp("PrimeDelay"), out int pd) ? pd : 3;
            cPrimeTime = int.TryParse(GetProp("PrimeTime"), out int pt) ? pt : 5;
            cRateType = int.TryParse(GetProp("UserRateType"), out int ut) ? ut : 0;
            cResumeAfterPrime = bool.TryParse(GetProp("ResumeAfterPrime"), out bool rp) ? rp : false;
            cShowPressure = bool.TryParse(GetProp("ShowPressure"), out bool sp) ? sp : false;
            cShowSwitches = bool.TryParse(GetProp("ShowSwitches"), out bool ss) ? ss : false;
            cSimMode = Enum.TryParse(GetProp("SimMode"), out SimType sm) ? sm : SimType.Sim_None;
            cSimSpeed = double.TryParse(GetProp("SimSpeed"), out double spd) ? spd : 0;
            cUseDualAuto = bool.TryParse(GetProp("UseDualAuto"), out bool da) ? da : false;
            cUseLargeScreen = bool.TryParse(GetProp("UseLargeScreen"), out bool ls) ? ls : false;
            cUseTransparent = bool.TryParse(GetProp("UseTransparent"), out bool utr) ? utr : false;
            cUseZones = bool.TryParse(GetProp("UseZones"), out bool uz) ? uz : false;
            cShowQuantityRemaining = bool.TryParse(GetProp("ShowQuantityRemaining"), out bool qr) ? qr : false;
            cShowCoverageRemaining = bool.TryParse(GetProp("ShowCoverageRemaining"), out bool cr) ? cr : false;
            cUseMetric = bool.TryParse(GetProp("UseMetric"), out bool mt) ? mt : false;
            cRateRecordEnabled = bool.TryParse(GetProp("RecordRates"), out bool rc) ? rc : true;
            cMapShowTiles = bool.TryParse(GetProp("ShowTiles"), out bool st) ? st : true;
            cMapShowZones = bool.TryParse(GetProp("MapShowZones"), out bool sz) ? sz : true;
            cMapShowRates = bool.TryParse(GetProp("MapShowRates"), out bool sr) ? sr : false;
            cUseRateDisplay = bool.TryParse(GetProp("UseRateDisplay"), out bool rtd) ? rtd : false;
            cShowJobs = bool.TryParse(GetProp("ShowJobs"), out bool ja) ? ja : false;

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
                bool FileFound = false;

                if (IsNew) File.WriteAllText(FileName, ""); // Create empty property file

                if (File.Exists(FileName))
                {
                    Properties.Settings.Default.CurrentFile = FileName;
                    Properties.Settings.Default.Save();
                    FileFound = true;
                }

                if (!FileFound) CheckFolders();
                Load(cProps, Properties.Settings.Default.CurrentFile);
                string CurrentDir = Path.GetDirectoryName(Properties.Settings.Default.CurrentFile);

                cFormPropsFileName = Path.Combine(ApplicationFolder, "FormData.txt");
                if (!File.Exists(cFormPropsFileName)) File.WriteAllText(cFormPropsFileName, "");
                Load(cFormProps, cFormPropsFileName);

                cErrorsFileName = Path.Combine(ApplicationFolder, "Error Log.txt");
                if (!File.Exists(cErrorsFileName)) File.WriteAllText(cErrorsFileName, "");

                cActivityFileName = Path.Combine(ApplicationFolder, "Activity Log.txt");
                if (!File.Exists(cActivityFileName)) File.WriteAllText(cActivityFileName, "");

                LoadProperties();
                Result = true;
            }
            catch (Exception)
            {
            }
            return Result;
        }

        public static bool ParseDateText(string input, out DateTime result)
        {
            int currentYear = DateTime.Now.Year;
            if (IsDayMonthOnly(input)) input += " " + currentYear.ToString();

            // Try to parse the date with different formats
            string[] formats = {
                "MM/dd/yyyy", "MM-dd-yyyy", "MM.dd.yyyy",
                "dd/MM/yyyy", "dd-MM-yyyy", "dd.MM.yyyy",
                "yyyy/MM/dd", "yyyy-MM-dd", "yyyy.MM.dd",
                "MMM dd, yyyy", "dd MMM yyyy", "MMM dd yyyy",
                "dd MMM yyyy"
            };

            return DateTime.TryParseExact(input, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
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

        public static void RaiseRateDataSettingsChanged()
        {
            RateDataSettingsChanged?.Invoke(null, EventArgs.Empty);
        }

        public static bool RateMapIsVisible()
        {
            bool Result = true;
            try
            {
                if (cCurrentMenuName != "frmMenuRateMap")
                {
                    Result = false;
                }
                else
                {
                    Form frm = IsFormOpen("frmMenuRateMap", false);
                    if ((frm == null) || !frm.Visible || (frm.WindowState == FormWindowState.Minimized))
                    {
                        Result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog("Props/IsRateMapVisible: " + ex.Message);
                Result = false;
            }
            return Result;
        }

        public static void SaveFormLocation(Form frm, string Instance = "")
        {
            try
            {
                if (frm.WindowState == FormWindowState.Normal)
                {
                    string name = frm.Name + Instance + ".Left";
                    SetFormProp(name, frm.Left.ToString());

                    name = frm.Name + Instance + ".Top";
                    SetFormProp(name, frm.Top.ToString());
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog("Props/SaveFormLocation: " + ex.Message);
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

        public static void SwitchScreens(bool SingleProduct = false)
        {
            try
            {
                Form fs = IsFormOpen("frmLargeScreen");
                if (cUseLargeScreen)
                {
                    if (SingleProduct)
                    {
                        // hide unused items, set product 4 as default, set product 4 id to 0
                        foreach (clsProduct Prd in mf.Products.Items)
                        {
                            Prd.OnScreen = false;
                        }
                        clsProduct P0 = mf.Products.Items[0];
                        clsProduct P3 = mf.Products.Items[3];

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

                        P3.MaxPWMadjust = P0.MaxPWMadjust;
                        P3.MinPWMadjust = P0.MinPWMadjust;
                        P3.KP = P0.KP;

                        mf.Products.Item(2).BumpButtons = false;
                        P0.EditSensorIDs(6, 0);
                        P3.LoadSensor(0, 0);
                        P3.OnScreen = true;
                        P3.AppMode = P0.AppMode;
                        P3.UseOffRateAlarm = P0.UseOffRateAlarm;
                        P3.OffRateSetting = P0.OffRateSetting;
                        P3.MinUPM = P0.MinUPM;
                        P3.BumpButtons = false;

                        P3.CountsRev = P0.CountsRev;
                        Props.DefaultProduct = 3;
                        UseTransparent = true;
                    }

                    if (fs == null)
                    {
                        mf.LargeScreenExit = false;
                        mf.Restart = false;
                        mf.WindowState = FormWindowState.Minimized;
                        mf.ShowInTaskbar = false;
                        mf.Lscrn = new frmLargeScreen(mf);
                        mf.Lscrn.ShowInTaskbar = true;
                        mf.Lscrn.SetTransparent();
                        mf.Lscrn.Show();
                    }
                }
                else
                {
                    // use standard screen
                    if (fs != null) mf.Lscrn.SwitchToStandard();
                }
                ScreensSwitched?.Invoke(null, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                WriteErrorLog("SwitchScreens: " + ex.Message);
            }
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
                // Retrieve screen width and height
                int screenWidth = Screen.PrimaryScreen.Bounds.Width;
                int screenHeight = Screen.PrimaryScreen.Bounds.Height;

                // Retrieve form width and height
                int formWidth = form.Width;
                int formHeight = form.Height;

                // Calculate the appropriate position
                int xPosition = (screenWidth / 2) - (formWidth / 2);
                int yPosition = (screenHeight / 2) - (formHeight / 2);

                form.Location = new Point(xPosition, yPosition);
            }
            catch (Exception ex)
            {
                WriteErrorLog("Props/CenterForm: " + ex.Message);
            }
        }

        private static int GetFormProp(string key)
        {
            string prop = cFormProps.TryGetValue(key, out var value) ? value : string.Empty;
            return int.TryParse(prop, out var vl) ? vl : -1;
        }

        private static bool IsDayMonthOnly(string input)
        {
            bool Result = false;
            // Check if the input matches day and month formats
            string[] dayMonthFormats = {
                "MM/dd", "MM-dd", "MM.dd",
                "dd/MM", "dd-MM", "dd.MM",
                "MMM dd", "dd MMM"
            };

            foreach (var format in dayMonthFormats)
            {
                if (DateTime.TryParseExact(input, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                {
                    Result = true;
                    break;
                }
            }

            return Result;
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

        private static void SetFormProp(string key, string value)
        {
            try
            {
                if (value != null)
                {
                    if (!cFormProps.TryGetValue(key, out var existingValue) || existingValue != value)
                    {
                        cFormProps[key] = value;
                        Save(cFormProps, cFormPropsFileName);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog("SetFormProp/Set: " + ex.Message);
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
    }
}