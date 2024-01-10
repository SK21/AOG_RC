using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RateController
{
    public class clsTools
    {
        #region Form Dragging API Support

        // https://www.c-sharpcorner.com/article/transparent-borderless-forms-in-C-Sharp/
        // add to form:
        // private void Form1_MouseDown(object sender, MouseEventArgs e)
        // {
        //    if (e.Button == MouseButtons.Left) Tls.DragForm(this);
        // }

        //ReleaseCapture releases a mouse capture
        [DllImportAttribute("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern bool ReleaseCapture();

        //The SendMessage function sends a message to a window or windows.
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        #endregion Form Dragging API Support

        private static Hashtable HTapp;
        private static Hashtable HTfiles;
        private string cAppName = "RateController";
        private string cAppVersion = "3.6.3";
        private string cPropertiesApp;
        private string cPropertiesFile;
        private string cSettingsDir;
        private string cVersionDate = "09-Jan-2024";
        private FormStart mf;

        public clsTools(FormStart CallingForm)
        {
            mf = CallingForm;
            CheckFolders();
            OpenFile(Properties.Settings.Default.FileName);
        }

        public string PropertiesFile
        {
            get
            {
                return cPropertiesFile;
            }
            set
            {
                if (File.Exists(value))
                {
                    OpenFile(value);
                }
            }
        }

        public string AppVersion()
        {
            return cAppVersion;
        }

        public byte BitClear(byte b, int pos)
        {
            byte msk = (byte)(1 << pos);
            msk = (byte)~msk;
            return (byte)(b & msk);
        }

        public bool BitRead(byte b, int pos)
        {
            return ((b >> pos) & 1) != 0;
        }

        public byte BitSet(byte b, int pos)
        {
            return (byte)(b | (1 << pos));
        }

        public byte BuildModSenID(byte ArdID, byte SenID)
        {
            return (byte)((ArdID << 4) | (SenID & 0b00001111));
        }

        public string ControlTypeDescription(ControlTypeEnum CT)
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

        public byte CRC(byte[] Data, int Length, byte Start = 0)
        {
            byte Result = 0;
            if (Length <= Data.Length)
            {
                int CK = 0;
                for (int i = Start; i < Length; i++)
                {
                    CK += Data[i];
                }
                Result = (byte)CK;
            }
            return Result;
        }

        public byte CRC(string[] Data, int Length, byte Start = 0)
        {
            byte Result = 0;
            if (Length <= Data.Length)
            {
                byte tmp;
                byte[] BD = new byte[Length];
                for (int i = 0; i < Length; i++)
                {
                    if (byte.TryParse(Data[i], out tmp)) BD[i] = tmp;
                }
                int CK = 0;
                for (int i = Start; i < Length; i++)
                {
                    CK += BD[i];
                }
                Result = (byte)CK;
            }
            return Result;
        }

        public void DragForm(Form Frm)
        {
            ReleaseCapture();
            SendMessage(Frm.Handle, 0xa1, 0x2, 0);
        }

        public void DrawGroupBox(GroupBox box, Graphics g, Color BackColor, Color textColor, Color borderColor)
        {
            // useage:
            // point the Groupbox paint event to this sub:
            //private void GroupBoxPaint(object sender, PaintEventArgs e)
            //{
            //    GroupBox box = sender as GroupBox;
            //    mf.Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);
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

        public string FilesDir()
        {
            return Properties.Settings.Default.FilesDir;
        }

        public bool GoodCRC(byte[] Data, byte Start = 0)
        {
            bool Result = false;
            int Length = Data.Length;
            byte cr = CRC(Data, Length - 1, Start);
            Result = (cr == Data[Length - 1]);
            return Result;
        }

        public bool GoodCRC(string[] Data, byte Start = 0)
        {
            bool Result = false;
            byte tmp;
            int Length = Data.Length;
            byte[] BD = new byte[Length];
            for (int i = 0; i < Length; i++)
            {
                if (byte.TryParse(Data[i], out tmp)) BD[i] = tmp;
            }
            byte cr = CRC(BD, Length - 1, Start);   // exclude existing crc
            Result = (cr == BD[Length - 1]);
            return Result;
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

        public string LoadAppProperty(string Key)
        {
            string Prop = "";
            if (HTapp.Contains(Key)) Prop = HTapp[Key].ToString();
            return Prop;
        }

        public void LoadFormData(Form Frm)
        {
            int Leftloc = 0;
            int.TryParse(LoadAppProperty(Frm.Name + ".Left"), out Leftloc);
            Frm.Left = Leftloc;

            int Toploc = 0;
            int.TryParse(LoadAppProperty(Frm.Name + ".Top"), out Toploc);
            Frm.Top = Toploc;

            IsOnScreen(Frm, true);
        }

        public string LoadProperty(string Key)
        {
            string Prop = "";
            if (HTfiles.Contains(Key)) Prop = HTfiles[Key].ToString();
            return Prop;
        }

        public double NoisyData(double CurrentData, double ErrorPercent = 5.0)
        {
            try
            {
                // error percent is above and below current data
                var Rand = new Random();
                int Max = (int)(CurrentData * ErrorPercent * 2.0);
                double Spd = (CurrentData * (1.0 - ErrorPercent / 100.0)) + ((double)Rand.Next(Max) / 100.0);
                return Spd;
            }
            catch (Exception)
            {
                return CurrentData;
            }
        }

        public void OpenFile(string NewFile)
        {
            try
            {
                string PathName = Path.GetDirectoryName(NewFile); // only works if file name present
                string FileName = Path.GetFileName(NewFile);
                if (FileName == "") PathName = NewFile;     // no file name present, fix path name
                if (Directory.Exists(PathName)) Properties.Settings.Default.FilesDir = PathName; // set the new files dir

                cPropertiesFile = Properties.Settings.Default.FilesDir + "\\" + FileName;
                if (!File.Exists(cPropertiesFile)) File.Create(cPropertiesFile).Dispose();
                LoadFilesData(cPropertiesFile);
                Properties.Settings.Default.FileName = FileName;
                Properties.Settings.Default.Save();

                cPropertiesApp = Properties.Settings.Default.FilesDir + "\\AppData.txt";
                if (!File.Exists(cPropertiesApp)) File.Create(cPropertiesApp).Dispose();
                LoadAppData(cPropertiesApp);
            }
            catch (Exception ex)
            {
                WriteErrorLog("Tools: OpenFile: " + ex.Message);
            }
        }

        public byte ParseModID(byte ID)
        {
            // top 4 bits
            return (byte)(ID >> 4);
        }

        public byte ParseSenID(byte ID)
        {
            // bottom 4 bits
            return (byte)(ID & 0b00001111);
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

        public string ReadTextFile(string FileName)
        {
            string Result = "";
            string Line;
            FileName = cSettingsDir + "\\" + FileName;
            try
            {
                StreamReader sr = new StreamReader(FileName);
                Line = sr.ReadLine();
                while (Line != null)
                {
                    Result += Line + Environment.NewLine;
                    Line = sr.ReadLine();
                }
                sr.Close();
            }
            catch (Exception)
            {
                //WriteErrorLog("ReadTextFile: " + ex.Message);
            }
            return Result;
        }

        public void SaveAppProperty(string Key, string Value)
        {
            bool Changed = false;
            if (HTapp.Contains(Key))
            {
                if (!HTapp[Key].ToString().Equals(Value))
                {
                    HTapp[Key] = Value;
                    Changed = true;
                }
            }
            else
            {
                HTapp.Add(Key, Value);
                Changed = true;
            }
            if (Changed) SaveAppProperties();
        }

        public void SaveFile(string NewFile)
        {
            try
            {
                string PathName = Path.GetDirectoryName(NewFile); // only works if file name present
                string FileName = Path.GetFileName(NewFile);
                if (FileName == "") PathName = NewFile;     // no file name present, fix path name
                if (Directory.Exists(PathName)) Properties.Settings.Default.FilesDir = PathName; // set the new files dir

                cPropertiesFile = Properties.Settings.Default.FilesDir + "\\" + FileName;
                if (!File.Exists(cPropertiesFile)) File.Create(cPropertiesFile).Dispose();

                SaveProperties();
                Properties.Settings.Default.FileName = FileName;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                WriteErrorLog("clsTools: SaveFile: " + ex.Message);
            }
        }

        public void SaveFormData(Form Frm)
        {
            try
            {
                SaveAppProperty(Frm.Name + ".Left", Frm.Left.ToString());
                SaveAppProperty(Frm.Name + ".Top", Frm.Top.ToString());
            }
            catch (Exception)
            {
            }
        }

        public void SaveProperty(string Key, string Value)
        {
            bool Changed = false;
            if (HTfiles.Contains(Key))
            {
                if (!HTfiles[Key].ToString().Equals(Value))
                {
                    HTfiles[Key] = Value;
                    Changed = true;
                }
            }
            else
            {
                HTfiles.Add(Key, Value);
                Changed = true;
            }
            if (Changed) SaveProperties();
        }

        public string SettingsDir()
        {
            return cSettingsDir;
        }

        public void ShowHelp(string Message, string Title = "Help",
            int timeInMsec = 30000, bool LogError = false, bool Modal = false)
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

            if (LogError) WriteErrorLog(Message);
        }

        public void StartWifi()
        {
            string SSID = LoadProperty("WifiSSID");
            string Password = LoadProperty("WifiPassword");

            string Start = "netsh wlan set hostednetwork mode=allow ssid=" + SSID + " key=" + Password + "\n";
            Start += "netsh wlan stop hostednetwork\n";
            Start += "netsh wlan start hostednetwork\n";

            string FileName = SettingsDir() + "\\StartWifi.bat";
            File.WriteAllText(FileName, Start);

            var psi = new ProcessStartInfo();
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.FileName = FileName;
            psi.Verb = "runas";

            var process = new Process();
            process.StartInfo = psi;
            process.Start();
            process.WaitForExit();
        }

        public void StopWifi()
        {
            string Stop = "netsh wlan stop hostednetwork\n";

            string FileName = SettingsDir() + "\\StopWifi.bat";
            File.WriteAllText(FileName, Stop);

            var psi = new ProcessStartInfo();
            psi.CreateNoWindow = true;
            psi.FileName = FileName;
            psi.Verb = "runas";

            var process = new Process();
            process.StartInfo = psi;
            process.Start();
            process.WaitForExit();
        }

        public int StringToInt(string S)
        {
            if (decimal.TryParse(S, out decimal tmp))
            {
                return (int)tmp;
            }
            return 0;
        }

        public string VersionDate()
        {
            return cVersionDate;
        }

        public void WriteActivityLog(string Message, bool Newline = false)
        {
            string Line = "";
            try
            {
                string FileName = cSettingsDir + "\\Activity Log.txt";
                TrimFile(FileName);

                if (Newline) Line = "\r\n";

                File.AppendAllText(FileName, Line + DateTime.Now.ToString("MMM-dd hh:mm:ss") + "  -  " + Message + "\r\n");
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
                string FileName = cSettingsDir + "\\Error Log.txt";
                TrimFile(FileName);
                File.AppendAllText(FileName, DateTime.Now.ToString("MMM-dd hh:mm:ss") + "  -  " + strErrorText + "\r\n\r\n");
            }
            catch (Exception)
            {
            }
        }

        private void CheckFolders()
        {
            try
            {
                // SettingsDir
                cSettingsDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + cAppName;

                if (!Directory.Exists(cSettingsDir)) Directory.CreateDirectory(cSettingsDir);
                if (!File.Exists(cSettingsDir + "\\Example.rcs")) File.WriteAllBytes(cSettingsDir + "\\Example.rcs", Properties.Resources.Example);

                string FilesDir = Properties.Settings.Default.FilesDir;
                if (!Directory.Exists(FilesDir)) Properties.Settings.Default.FilesDir = cSettingsDir;
            }
            catch (Exception)
            {
            }
        }

        private void LoadAppData(string path)
        {
            // property:  key=value  ex: "LastFile=Main.mdb"
            try
            {
                HTapp = new Hashtable();
                string[] lines = System.IO.File.ReadAllLines(path);
                foreach (string line in lines)
                {
                    if (line.Contains("=") && !String.IsNullOrEmpty(line.Split('=')[0]) && !String.IsNullOrEmpty(line.Split('=')[1]))
                    {
                        string[] splitText = line.Split('=');
                        HTapp.Add(splitText[0], splitText[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog("Tools: LoadProperties: " + ex.Message);
            }
        }

        private void LoadFilesData(string path)
        {
            // property:  key=value  ex: "LastFile=Main.mdb"
            try
            {
                HTfiles = new Hashtable();
                string[] lines = System.IO.File.ReadAllLines(path);
                foreach (string line in lines)
                {
                    if (line.Contains("=") && !String.IsNullOrEmpty(line.Split('=')[0]) && !String.IsNullOrEmpty(line.Split('=')[1]))
                    {
                        string[] splitText = line.Split('=');
                        HTfiles.Add(splitText[0], splitText[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog("Tools: LoadProperties: " + ex.Message);
            }
        }

        private void SaveAppProperties()
        {
            try
            {
                string[] NewLines = new string[HTapp.Count];
                int i = -1;
                foreach (DictionaryEntry Pair in HTapp)
                {
                    i++;
                    NewLines[i] = Pair.Key.ToString() + "=" + Pair.Value.ToString();
                }
                if (i > -1) File.WriteAllLines(cPropertiesApp, NewLines);
            }
            catch (Exception)
            {
            }
        }

        private void SaveProperties()
        {
            try
            {
                string[] NewLines = new string[HTfiles.Count];
                int i = -1;
                foreach (DictionaryEntry Pair in HTfiles)
                {
                    i++;
                    NewLines[i] = Pair.Key.ToString() + "=" + Pair.Value.ToString();
                }
                if (i > -1) File.WriteAllLines(cPropertiesFile, NewLines);
            }
            catch (Exception)
            {
            }
        }

        private void TrimFile(string FileName, int MaxSize = 100000)
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