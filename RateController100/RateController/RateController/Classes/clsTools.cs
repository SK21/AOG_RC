using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RateController
{
    public class clsTools
    {
        private string cAppName = "RateController";
        private string cVersionDate = "14/Mar/2020";
        private FormRateControl mf;
        private string SettingsDir;

        public clsTools(FormRateControl CallingForm)
        {
            mf = CallingForm;
            CheckFolders();
        }

        public bool IsOnScreen(Form form, bool PutOnScreen = false)
        {
            // Create rectangle
            Rectangle formRectangle = new Rectangle(form.Left, form.Top, form.Width, form.Height);

            // Test
            bool IsOn = Screen.AllScreens.Any(s => s.WorkingArea.IntersectsWith(formRectangle));

            if (!IsOn & PutOnScreen)
            {
                form.Top = 0;
                form.Left = 0;
            }

            return IsOn;
        }

        public bool PrevInstance()
        {
            string PrsName = Process.GetCurrentProcess().ProcessName;
            Process[] All = Process.GetProcessesByName(PrsName); //Get the name of all processes having the same name as this process name
            if (All.Length > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void TimedMessageBox(string s1, string s2 = "", int timeout = 3000, bool LogError = false)
        {
            var form = new FormTimedMessage(s1, s2, timeout);
            form.Show();
            if (LogError) WriteErrorLog(s1 + "  " + s2);
        }

        public string VersionDate()
        {
            return cVersionDate;
        }

        public void WriteActivityLog(string Message)
        {
            try
            {
                string FileName = SettingsDir + "\\Activity Log.txt";
                TrimFile(FileName);
                File.AppendAllText(FileName, DateTime.Now.ToString() + "  -  " + Message + "\r\n\r\n");
            }
            catch (Exception ex)
            {
                WriteErrorLog("Tools: WriteActivityLog: " + ex.Message);
            }
        }

        public void WriteErrorLog(string strErrorText)
        {
            try
            {
                string FileName = SettingsDir + "\\Error Log.txt";
                TrimFile(FileName);
                File.AppendAllText(FileName, DateTime.Now.ToString() + "  -  " + strErrorText + "\r\n\r\n");
            }
            catch (Exception ex)
            {
                //TimedMessageBox("Error in WriteErrorLog", ex.Message);
            }
        }

        private void CheckFolders()
        {
            try
            {
                // SettingsDir
                SettingsDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "\\" + cAppName + "\\Settings";
                if (!Directory.Exists(SettingsDir)) Directory.CreateDirectory(SettingsDir);
            }
            catch (Exception ex)
            {
                WriteErrorLog("Tools: CheckFolders: " + ex.Message);
            }
        }

        private void TrimFile(string FileName, int MaxSize = 25000)
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
            catch (Exception ex)
            {
                //WriteErrorLog("Tools: TrimFile: " + ex.Message);
            }
        }
    }
}