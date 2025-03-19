using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RateController.Classes
{
    public enum RateType
    {
        Applied,
        Target
    }

    public static class Props
    {
        private static string cActivityFileName = "";
        private static string cErrorsFileName = "";
        private static SortedDictionary<string, string> cFormProps = new SortedDictionary<string, string>();
        private static string cFormPropsFileName = "";
        private static SortedDictionary<string, string> cProps = new SortedDictionary<string, string>();
        private static string cPropsFileName = "";
        private static bool cReadOnly = false;

        public static string FilePath
        {
            get { return cPropsFileName; }
            set
            {
                if (!File.Exists(value)) File.WriteAllText(value, ""); // Create empty property file
                cPropsFileName = value;
                Load(cProps, cPropsFileName);

                if (bool.TryParse(GetProp("ReadOnly"), out bool ro)) cReadOnly = ro;
                string FolderPath = Path.GetDirectoryName(cPropsFileName) ?? Directory.GetCurrentDirectory();

                cFormPropsFileName = Path.Combine(FolderPath, "FormData.txt");
                if (!File.Exists(cFormPropsFileName)) File.WriteAllText(cFormPropsFileName, "");
                Load(cFormProps, cFormPropsFileName);

                cErrorsFileName = Path.Combine(FolderPath, "Error Log.txt");
                if (!File.Exists(cErrorsFileName)) File.WriteAllText(cErrorsFileName, "");

                cActivityFileName = Path.Combine(FolderPath, "Activity Log.txt");
                if (!File.Exists(cErrorsFileName)) File.WriteAllText(cErrorsFileName, "");
            }
        }

        #region MainProperties

        public static int RateDisplayProduct
        {
            get { return int.TryParse(GetProp("RatesProduct"), out int rs) ? rs : 0; }
            set { SetProp("RatesProduct", value.ToString()); }
        }

        public static int RateDisplayRefresh
        {
            get { return int.TryParse(GetProp("RateDisplayRefresh"), out int rs) ? rs : 0; }
            set { SetProp("RateDisplayRefresh", value.ToString()); }
        }

        public static int RateDisplayResolution
        {
            get { return int.TryParse(GetProp("RateDisplayResolution"), out int rs) ? rs : 0; }
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

        #endregion MainProperties

        public static bool ReadOnly
        {
            get { return cReadOnly; }
            set
            {
                bool Changed = cReadOnly != value;
                cReadOnly = value;
                if (Changed) Save(cProps, cPropsFileName);
            }
        }

        public static bool FormIsClosed(string Name, bool SetFocus = true)
        {
            bool Result = true;
            Form frm = Application.OpenForms[Name];
            if (frm != null)
            {
                Result = false;
                if (SetFocus) frm.Focus();
            }
            return Result;
        }

        public static string GetProp(string key)
        {
            return cProps.TryGetValue(key, out var value) ? value : string.Empty;
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
                        Save(cProps, cPropsFileName);
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