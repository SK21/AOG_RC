using GMap.NET.MapProviders;
using RateController.Language;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

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
        private static string cActivityFileName = "";
        private static string cAppName = "RateController";
        private static string cAppVersion = "4.0.0-beta.9";
        private static string cErrorsFileName = "";
        private static SortedDictionary<string, string> cFormProps = new SortedDictionary<string, string>();
        private static string cFormPropsFileName = "";
        private static MasterSwitchMode cMasterSwitchMode = MasterSwitchMode.ControlAll;
        private static SortedDictionary<string, string> cProps = new SortedDictionary<string, string>();
        private static bool cReadOnly = false;
        private static bool cUseVariableRate = false;
        private static string cVersionDate = "19-Mar-2025";

        #region MainProperties

        public static MasterSwitchMode MasterSwitchMode
        {
            get { return cMasterSwitchMode; }
            set
            {
                cMasterSwitchMode = value;
                SetProp("MasterSwitchMode", cMasterSwitchMode.ToString());
            }
        }

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
            set { SetProp("RateDisplayResolution", value.ToString()); }
        }

        public static bool RateDisplayShow
        {
            get { return bool.TryParse(GetProp("DisplayRates"), out bool dr) ? dr : false; }
            set { SetProp("DisplayRates", value.ToString()); }
        }

        public static RateType RateDisplayType
        {
            get { return Enum.TryParse(GetProp("RateDisplayType"), out RateType tp) ? tp : RateType.Applied; }
            set { SetProp("RateDisplayType", value.ToString()); }
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

        public static bool RecordRates
        {
            get { return bool.TryParse(GetProp("RecordRates"), out bool rc) ? rc : false; }
            set { SetProp("RecordRates", value.ToString()); }
        }

        public static bool UseMetric
        {
            get { return bool.TryParse(GetProp("UseMetric"), out bool mt) ? mt : false; }
            set { SetProp("UseMetric", value.ToString()); }
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

        public static string VersionDate()
        {
            return cVersionDate;
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
                if (!File.Exists(cDefaultDir + "\\Example.rcs")) File.WriteAllBytes(cDefaultDir + "\\Example.rcs", Properties.Resources.Example);
                if (!File.Exists(cDefaultDir + "\\Default.rcs")) File.WriteAllBytes(cDefaultDir + "\\Default.rcs", Properties.Resources.Default);

                // check user file
                if (!File.Exists(Properties.Settings.Default.CurrentFile))
                {
                    Properties.Settings.Default.CurrentFile = cDefaultDir + "\\Default.rcs";
                    Properties.Settings.Default.Save();
                }
                Result = true;
            }
            catch (Exception)
            {
            }
            return Result;
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
            Form frm = Application.OpenForms[Name];
            if (frm != null && SetFocus) frm.Focus();
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

                cFormPropsFileName = Path.Combine(CurrentDir, "FormData.txt");
                if (!File.Exists(cFormPropsFileName)) File.WriteAllText(cFormPropsFileName, "");
                Load(cFormProps, cFormPropsFileName);

                cErrorsFileName = Path.Combine(CurrentDir, "Error Log.txt");
                if (!File.Exists(cErrorsFileName)) File.WriteAllText(cErrorsFileName, "");

                cActivityFileName = Path.Combine(CurrentDir, "Activity Log.txt");
                if (!File.Exists(cErrorsFileName)) File.WriteAllText(cErrorsFileName, "");

                LoadProperties();
                Result = true;
            }
            catch (Exception)
            {
            }
            return Result;
        }

        public static bool SaveAs(string FileName, bool OverWrite = false)
        {
            bool Result = false;
            if (!File.Exists(FileName) || OverWrite)
            {
                File.WriteAllText(FileName, ""); // Create empty property file

                if (File.Exists(FileName))
                {
                    Properties.Settings.Default.CurrentFile = FileName;
                    Properties.Settings.Default.Save();

                    string CurrentDir = Path.GetDirectoryName(Properties.Settings.Default.CurrentFile);

                    cFormPropsFileName = Path.Combine(CurrentDir, "FormData.txt");
                    if (!File.Exists(cFormPropsFileName)) File.WriteAllText(cFormPropsFileName, "");
                    Load(cFormProps, cFormPropsFileName);

                    cErrorsFileName = Path.Combine(CurrentDir, "Error Log.txt");
                    if (!File.Exists(cErrorsFileName)) File.WriteAllText(cErrorsFileName, "");

                    cActivityFileName = Path.Combine(CurrentDir, "Activity Log.txt");
                    if (!File.Exists(cErrorsFileName)) File.WriteAllText(cErrorsFileName, "");

                    Save(cProps, Properties.Settings.Default.CurrentFile);
                    Save(cFormProps, cFormPropsFileName);
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