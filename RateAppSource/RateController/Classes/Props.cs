using GMap.NET.MapProviders;
using RateController.Language;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Windows.Forms;
using System.Xml.Linq;

namespace RateController.Classes
{
    public enum ApplicationMode
    { ControlledUPM, ConstantUPM, DocumentApplied, DocumentTarget }

    public enum ControlTypeEnum
    { Valve, ComboClose, Motor, MotorWeights, Fan, ComboCloseTimed }

    public enum MasterSwitchMode
    { ControlAll, ControlMasterRelayOnly, Override };

    public enum RateType
    { Applied, Target }

    public enum RelayTypes
    { Section, Slave, Master, Power, Invert_Section, HydUp, HydDown, TramRight, TramLeft, GeoStop, Switch, None };

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
        public static readonly int MaxSensors = 8;
        public static readonly int MaxSwitches = 16;
        public static readonly double MPHtoKPH = 1.6092;
        public static bool cShowCoverageRemaining;
        public static bool cShowQuantityRemaining;
        private static string cActivityFileName = "";
        private static string cAppDate = "19-Mar-2025";
        private static string cApplicationFolder;
        private static string cAppName = "RateController";
        private static string cAppVersion = "4.0.0-beta.9";
        private static string cCurrentMapName;
        private static string cCurrentRateDataName;
        private static string cDefaultJob;
        private static int cDefaultProduct;
        private static string cErrorsFileName = "";
        private static string cFieldNames;
        private static SortedDictionary<string, string> cFormProps = new SortedDictionary<string, string>();
        private static string cFormPropsFileName = "";
        private static SortedDictionary<string, string> cJobProps = new SortedDictionary<string, string>();
        private static string cJobsFolder;
        private static bool cMapShowRates;
        private static bool cMapShowTiles;
        private static bool cMapShowZones;
        private static MasterSwitchMode cMasterSwitchMode = MasterSwitchMode.ControlAll;
        private static int cPrimeDelay = 3;
        private static double cPrimeTime = 0;
        private static string cProfilesFolder;
        private static SortedDictionary<string, string> cProps = new SortedDictionary<string, string>();
        private static bool cRateRecordEnabled;
        private static int cRateRecordInterval;
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
        private static bool cUseTransparent = false;
        private static bool cUseVariableRate = false;
        private static bool cUseZones = false;
        private static FormStart mf;
        private static Dictionary<string, Form> openForms = new Dictionary<string, Form>();
        private static frmPressureDisplay PressureDisplay;
        private static frmSwitches SwitchesForm;

        #region // flow adjustment defaults

        public static readonly int HighAdjustDefault = 50;
        public static readonly int LowAdjustDefault = 20;
        public static readonly int MaxAdjustDefault = 100;
        public static readonly int MinAdjustDefault = 5;
        public static readonly int ScalingDefault = 48;
        public static readonly int ThresholdDefault = 50;

        #endregion // flow adjustment defaults

        public static event EventHandler JobChanged;

        public static event EventHandler MapShowRatesSettingsChanged;

        public static event EventHandler RecordSettingsChanged;

        public static event EventHandler UnitsChanged;

        #region MainProperties

        public static string ApplicationFolder
        { get { return cApplicationFolder; } }

        public static string CurrentJob
        {
            get
            {
                string Result = "";
                if (File.Exists(Properties.Settings.Default.CurrentJob))
                {
                    Result = Properties.Settings.Default.CurrentJob;
                }
                else
                {
                    if (File.Exists(cDefaultJob))
                    {
                        Result = cDefaultJob;
                    }
                    else
                    {
                        CheckFolders();
                        Result = cDefaultJob;
                    }
                }
                return Result;
            }
        }

        public static string CurrentMapName
        {
            get { return cCurrentMapName; }
        }

        public static string CurrentRateDataName
        {
            get { return cCurrentRateDataName; }
        }

        public static string DefaultJob
        {
            get
            {
                if (!File.Exists(cDefaultJob)) CheckFolders();
                return cDefaultJob;
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

        public static string FieldNames
        { get { return cFieldNames; } }

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
                if (value >= 0 && value < 30)
                {
                    cPrimeTime = value;
                    SetProp("PrimeTime", cPrimeTime.ToString());
                }
            }
        }

        public static string ProfilesFolder
        { get { return cProfilesFolder; } }

        public static int RateDisplayProduct
        {
            get { return int.TryParse(GetProp("RatesProduct"), out int rs) ? rs : 0; }
            set { SetProp("RatesProduct", value.ToString()); }
        }

        public static int RateDisplayRefresh
        {
            get { return int.TryParse(GetProp("RateDisplayRefresh"), out int rs) ? rs : 300; }
            set { SetProp("RateDisplayRefresh", value.ToString()); }
        }

        public static int RateDisplayResolution
        {
            get { return int.TryParse(GetProp("RateDisplayResolution"), out int rs) ? rs : 20; }
            set
            {
                SetProp("RateDisplayResolution", value.ToString());
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
                RecordSettingsChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public static int RateRecordInterval
        {
            get { return cRateRecordInterval; }
            set
            {
                cRateRecordInterval = value;
                SetProp("RateRecordInterval", cRateRecordInterval.ToString());
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

                // jobs folder
                name = cApplicationFolder + "\\Jobs";
                if (!Directory.Exists(name)) Directory.CreateDirectory(name);
                cJobsFolder = name;

                // check default field name exists. 
                cFieldNames = Path.Combine(ApplicationFolder, "FieldNames.txt");
                if (!File.Exists(cFieldNames)) File.WriteAllText(cFieldNames, "");

                string DefaultJobFolder = name + "\\" + "DefaultJob";
                if (!Directory.Exists(DefaultJobFolder)) Directory.CreateDirectory(DefaultJobFolder);
                cDefaultJob = DefaultJobFolder + "\\DefaultJob.jbs";
                if (!File.Exists(cDefaultJob)) File.WriteAllText(cDefaultJob, string.Empty);
                if (!File.Exists(DefaultJobFolder + "\\Default_RateData.csv")) File.WriteAllText(DefaultJobFolder + "\\Default_RateData.csv", string.Empty);

                string MapName = DefaultJobFolder + "\\Map";
                if (!Directory.Exists(MapName)) Directory.CreateDirectory(MapName);

                // check user files
                if (!File.Exists(Properties.Settings.Default.CurrentFile))
                {
                    Properties.Settings.Default.CurrentFile = DefaultProfile + "\\Default.rcs";
                    Properties.Settings.Default.UseJobs = true;
                    Properties.Settings.Default.Save();
                }

                Result = true;
            }
            catch (Exception)
            {
            }
            return Result;
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

        public static string[] GetFieldNames()
        {
            string[] items = null;
            try
            {
                items = File.ReadAllLines(cFieldNames);
            }
            catch (Exception ex)
            {
                WriteErrorLog("Props/GetFieldNames:" + ex.Message);
            }
            return items;
        }

        public static string GetJobProp(string key)
        {
            string prop = cJobProps.TryGetValue(key, out var value) ? value : string.Empty;
            return prop;
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
            Form frm = null;
            if (openForms.TryGetValue(Name, out frm))
            {
                if (SetFocus) frm.Focus();
            }
            return frm;
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

                // record open forms
                if (!openForms.ContainsKey(frm.Name))
                {
                    openForms.Add(frm.Name, frm);
                    // subscribe to the form close event to remove
                    frm.FormClosed += (s, e) => openForms.Remove(frm.Name);
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog("Props/LoadFormLocation: " + ex.Message);
            }
        }

        public static void LoadProperties()
        {
            cReadOnly = bool.TryParse(GetProp("ReadOnly"), out bool rd) ? rd : false;
            cUseVariableRate = bool.TryParse(GetProp("UseVariableRate_" + Props.CurrentDir()), out bool vr) ? vr : false;
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
            cRateRecordInterval = int.TryParse(GetProp("RateRecordInterval"), out int rr) ? rr : 300;
            cRateRecordEnabled = bool.TryParse(GetProp("RecordRates"), out bool rc) ? rc : true;
            cCurrentMapName = GetProp("CurrentMapName");
            cMapShowTiles = bool.TryParse(GetProp("ShowTiles"), out bool st) ? st : true;
            cMapShowZones = bool.TryParse(GetProp("MapShowZones"), out bool sz) ? sz : true;
            cMapShowRates = bool.TryParse(GetProp("MapShowRates"), out bool sr) ? sr : false;
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
                if (!File.Exists(cErrorsFileName)) File.WriteAllText(cErrorsFileName, "");

                LoadProperties();
                Result = true;
            }
            catch (Exception)
            {
            }
            return Result;
        }

        public static bool CheckYearExists(int yr, bool Create = false)
        {
            bool Result = false;
            string[] subFolders = Directory.GetDirectories(cJobsFolder);
            foreach (string subFolder in subFolders)
            {
                string folderName = Path.GetFileName(subFolder);
                int.TryParse(folderName, out int year);
                if (year > 1900 || year < 2100)
                {
                    Result = true;
                    break;
                }
            }

            if (!Result && Create)
            {
                if (yr > 1900 && yr < 2100)
                {
                    Directory.CreateDirectory(cJobsFolder + "\\" + yr.ToString());
                    Result = true;
                }
            }

            return Result;
        }

        public static bool OpenJob(string NewFile, bool IsNew = false)
        {
            Debug.Print("OpenJob");
            bool Result = false;
            try
            {
                string FileName = Path.GetFileNameWithoutExtension(NewFile);
                string FolderName = Props.JobsFolder + "\\" + FileName;
                string FullName = FolderName + "\\" + FileName + ".jbs";
                if (IsNew)
                {
                    if (FileNameValidator.IsValidFolderName(FolderName) &&
                        FileNameValidator.IsValidFileName(FileName) &&
                        !Directory.Exists(FolderName))
                    {
                        Directory.CreateDirectory(FolderName);
                        File.WriteAllText(FullName, string.Empty);
                        File.WriteAllText(FolderName + "\\" + FileName + "_RateData.csv", string.Empty);

                        Directory.CreateDirectory(FolderName + "\\Map");
                    }
                }

                if (File.Exists(FullName))
                {
                    Load(cJobProps, FullName);
                    Properties.Settings.Default.CurrentJob = FullName;
                    Properties.Settings.Default.Save();
                    cCurrentMapName = FolderName + "\\Map\\" + FileName + ".shp";
                    cCurrentRateDataName = FolderName + "\\" + FileName + "_RateData.csv";
                }
                else
                {
                    // use default job
                    Load(cJobProps, cDefaultJob);
                    Properties.Settings.Default.CurrentJob = cDefaultJob;
                    Properties.Settings.Default.Save();

                    string currentJobFolder = Path.GetDirectoryName(cDefaultJob);
                    cCurrentMapName = currentJobFolder + "\\Map\\Default.shp";
                    cCurrentRateDataName = currentJobFolder + "\\Default_RateData.csv";
                }

                Result = true;
                JobChanged?.Invoke(null, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                WriteErrorLog("Props/OpenJob: " + ex.Message);
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

        public static void RaiseMapShowRatesSettingsChanged()
        {
            MapShowRatesSettingsChanged?.Invoke(null, EventArgs.Empty);
        }

        public static bool SafeToDelete(string name)
        {
            bool Result = false;

            string myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string rateControllerPath = Path.Combine(myDocuments, "RateController");
            string fullPath = Path.GetFullPath(name);

            if (fullPath.StartsWith(rateControllerPath, StringComparison.OrdinalIgnoreCase))
            {
                if (Directory.Exists(fullPath))
                {
                    Result = true;
                }
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

        public static void SetFieldNames(string[] Names)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(cFieldNames))
                {
                    foreach (string name in Names)
                    {
                        writer.WriteLine(name);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog("Props/SetFieldNames: " + ex.Message);
            }
        }

        public static void SetJobProp(string key, string value, bool IgnoreReadOnly = false)
        {
            try
            {
                if (value != null && (!ReadOnly || IgnoreReadOnly))
                {
                    if (!cJobProps.TryGetValue(key, out var existingValue) || existingValue != value)
                    {
                        cJobProps[key] = value;
                        Save(cJobProps, Properties.Settings.Default.CurrentJob);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog("Props/SetJobProp: " + ex.Message);
            }
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

                        P3.HighAdjust = P0.HighAdjust;
                        P3.LowAdjust = P0.LowAdjust;
                        P3.Threshold = P0.Threshold;
                        P3.MaxAdjust = P0.Threshold;
                        P3.MinAdjust = P0.MinAdjust;

                        mf.Products.Item(2).BumpButtons = false;
                        P0.ModuleID = 6;
                        P3.ChangeID(0, 0);
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

        private static void CheckOnScreen(Form form)
        {
            try
            {
                // Create rectangle
                Rectangle formRectangle = new Rectangle(form.Left, form.Top, form.Width, form.Height);

                // Test
                bool IsOn = Screen.AllScreens.Any(s => s.WorkingArea.IntersectsWith(formRectangle));

                if (!IsOn) CenterForm(form);
            }
            catch (Exception ex)
            {
                WriteErrorLog("Props/CheckOnScreen: " + ex.Message);
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
}