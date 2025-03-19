using RateController.Classes;
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

namespace RateController
{
    public enum ApplicationMode
    { ControlledUPM, ConstantUPM, DocumentApplied, DocumentTarget }

    public enum ControlTypeEnum
    { Valve, ComboClose, Motor, MotorWeights, Fan, ComboCloseTimed }

    public enum MasterSwitchMode
    { ControlAll, ControlMasterRelayOnly, Override };

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

    public class clsTools
    {
        private string cAppName = "RateController";
        private string cAppVersion = "4.0.0-beta.9";
        private bool cIsReadOnly = false;
        private MasterSwitchMode cMasterSwitchMode = MasterSwitchMode.ControlAll;
        private string cPropertiesApp;
        private string cPropertiesFile;
        private string cSettingsDir;
        private bool cUseVariableRate = false;
        private string cVersionDate = "13-Mar-2025";
        private string lastMessage;
        private DateTime lastMessageTime;
        private FormStart mf;
        private Form[] OpenForms = new Form[30];    // make sure to allocate enough
        private SortedDictionary<string, string> PropsApp = new SortedDictionary<string, string>();
        private SortedDictionary<string, string> PropsDictionary = new SortedDictionary<string, string>();

        #region ScreenBitMap

        private MapManager cManager;
        private DataCollector cRateCollector;
        private Bitmap cScreenBitmap;
        private int cScreenBitmapHeight = 465;  // from frmMenuColor colorPanel
        private int cScreenBitmapWidth = 516;

        #endregion ScreenBitMap

        public clsTools(FormStart CallingForm)
        {
            mf = CallingForm;
            CheckFolders();
            OpenFile(Properties.Settings.Default.FileName);
            CreateColorBitmap();

            lastMessage = string.Empty;
            lastMessageTime = DateTime.MinValue;
        }

        public MapManager Manager
        { get { return cManager; } }

        public MasterSwitchMode MasterSwitchMode
        {
            get { return cMasterSwitchMode; }
            set
            {
                cMasterSwitchMode = value;
                SaveProperty("MasterSwitchMode", cMasterSwitchMode.ToString());
            }
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

        public DataCollector RateCollector
        { get { return cRateCollector; } }

        public bool ReadOnly
        {
            get { return cIsReadOnly; }
            set
            {
                cIsReadOnly = value;
                SaveProperty("ReadOnly", cIsReadOnly.ToString(), true);
            }
        }

        public bool VariableRateEnabled
        {
            get { return cUseVariableRate; }
            set
            {
                cUseVariableRate = value;
                SaveProperty("UseVariableRate_" + Properties.Settings.Default.FileName, cUseVariableRate.ToString());
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

        public void CenterForm(Form form)
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

        public bool IsFormNameValid(string formName)
        {
            // Get the current assembly
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Check if a type with the given name exists and inherits from Form
            var formType = assembly.GetTypes().FirstOrDefault(t => t.Name == formName && t.IsSubclassOf(typeof(Form)));
            return formType != null;
        }

        public Form IsFormOpen(string Name)
        {
            Form Result = null;
            for (int i = 0; i < OpenForms.Length; i++)
            {
                if (OpenForms[i] != null && OpenForms[i].Name == Name)
                {
                    Result = OpenForms[i];
                    break;
                }
            }
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
                CenterForm(form);
            }

            return IsOn;
        }

        public string LoadAppProperty(string Key)
        {
            string Prop = "";
            if (PropsApp.ContainsKey(Key)) Prop = PropsApp[Key].ToString();
            return Prop;
        }

        public void LoadFormData(Form Frm, string Instance = "", bool SetLocation = true)
        {
            if (SetLocation)
            {
                int Leftloc = -1;
                int Toploc = -1;
                if (int.TryParse(LoadAppProperty(Frm.Name + Instance + ".Left"), out int lft)) Leftloc = lft;
                if (int.TryParse(LoadAppProperty(Frm.Name + Instance + ".Top"), out int tl)) Toploc = tl;

                if (Leftloc == -1 || Toploc == -1)
                {
                    CenterForm(Frm);
                }
                else
                {
                    Frm.Left = Leftloc;
                    Frm.Top = Toploc;
                }
                IsOnScreen(Frm, true);
            }
            FormAdd(Frm);
        }

        public string LoadProperty(string Key)
        {
            string Prop = "";
            if (PropsDictionary.ContainsKey(Key)) Prop = PropsDictionary[Key].ToString();
            return Prop;
        }

        public void NewRateCollector(string FileName, bool Overwrite = false)
        {
            cRateCollector = new DataCollector(FileName, Overwrite);
        }

        public void OpenFile(string NewFile, bool IsNew = false)
        {
            try
            {
                string PathName = Path.GetDirectoryName(NewFile); // only works if file name present
                string FileName = Path.GetFileName(NewFile);
                if (FileName == "") PathName = NewFile;     // no file name present, fix path name
                if (Directory.Exists(PathName)) Properties.Settings.Default.FilesDir = PathName; // set the new files dir

                cPropertiesFile = Properties.Settings.Default.FilesDir + "\\" + FileName;
                if (!File.Exists(cPropertiesFile))
                {
                    if (IsNew)
                    {
                        // create new file
                        File.Create(cPropertiesFile).Dispose();
                    }
                    else
                    {
                        // file not found, use default file
                        cPropertiesFile = Properties.Settings.Default.FilesDir + "\\Default.rcs";
                        FileName = "Default.RCS";
                    }
                }
                Props.FilePath = cPropertiesFile;
                LoadFilesData(cPropertiesFile);
                Properties.Settings.Default.FileName = FileName;
                Properties.Settings.Default.Save();
                if (bool.TryParse(LoadProperty("ReadOnly"), out bool RO))
                {
                    cIsReadOnly = RO;
                }
                else
                {
                    cIsReadOnly = false;
                }

                cPropertiesApp = Properties.Settings.Default.FilesDir + "\\AppData.txt";
                if (!File.Exists(cPropertiesApp)) File.Create(cPropertiesApp).Dispose();
                LoadAppData(cPropertiesApp);
                if (bool.TryParse(LoadProperty("UseVariableRate_" + Properties.Settings.Default.FileName), out bool vr))
                {
                    cUseVariableRate = vr;
                }
                else
                {
                    cUseVariableRate = false;
                }

                if (Enum.TryParse(LoadProperty("MasterSwitchMode"), out MasterSwitchMode msm))
                {
                    cMasterSwitchMode = msm;
                }
                else
                {
                    cMasterSwitchMode = MasterSwitchMode.ControlAll;
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog("Tools: OpenFile: " + ex.Message);
            }
        }

        public bool OpenTextFile(string FileName)
        {
            bool Result = false;
            try
            {
                string Name = cSettingsDir + "\\" + FileName;
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
                if (File.Exists(FileName))
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
            }
            catch (Exception)
            {
                //WriteErrorLog("ReadTextFile: " + ex.Message);
            }
            return Result;
        }

        public void SaveAppProperty(string Key, string Value)
        {
            try
            {
                if (Value != null)
                {
                    bool Changed = false;
                    if (PropsApp.ContainsKey(Key))
                    {
                        if (!PropsApp[Key].ToString().Equals(Value))
                        {
                            PropsApp[Key] = Value;
                            Changed = true;
                        }
                    }
                    else
                    {
                        PropsApp.Add(Key, Value);
                        Changed = true;
                    }
                    if (Changed) SaveAppProperties();
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog("clsTools/SaveAppProperty: " + ex.Message);
            }
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

        public void SaveFormData(Form Frm, string Instance = "")
        {
            try
            {
                if (Frm.WindowState == FormWindowState.Normal)
                {
                    SaveAppProperty(Frm.Name + Instance + ".Left", Frm.Left.ToString());
                    SaveAppProperty(Frm.Name + Instance + ".Top", Frm.Top.ToString());
                }
                FormRemove(Frm);
            }
            catch (Exception)
            {
            }
        }

        public void SaveProperty(string Key, string Value, bool IgnoreReadOnly = false)
        {
            try
            {
                if (!ReadOnly || IgnoreReadOnly || Value != null)
                {
                    bool Changed = false;
                    if (PropsDictionary.ContainsKey(Key))
                    {
                        if (!PropsDictionary[Key].ToString().Equals(Value))
                        {
                            PropsDictionary[Key] = Value;
                            Changed = true;
                        }
                    }
                    else
                    {
                        PropsDictionary.Add(Key, Value);
                        Changed = true;
                    }
                    if (Changed) SaveProperties();
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog("clsTools/SaveProperty: " + ex.Message);
            }
        }

        public string SettingsDir()
        {
            return cSettingsDir;
        }

        public void ShowMessage(string Message, string Title = "Help",
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

                if (LogError) WriteErrorLog(Message);
                if (PlayErrorSound) SystemSounds.Exclamation.Play();

                lastMessage = Message;
                lastMessageTime = DateTime.Now;
            }
        }

        public void StartMapManager()
        {
            cManager = new MapManager(mf);
        }

        #region ScreenBitMapCode

        public Bitmap ScreenBitmap
        { get { return cScreenBitmap; } }

        public Color ColorFromHSV(float hue, float saturation, float brightness)
        {
            Color Result;
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            float f = (float)(hue / 60 - Math.Floor(hue / 60));
            brightness = brightness * 255;
            int v = Convert.ToInt32(brightness);
            int p = Convert.ToInt32(brightness * (1 - saturation));
            int q = Convert.ToInt32((brightness * (1 - f * saturation)));
            int t = Convert.ToInt32((brightness * (1 - (1 - f) * saturation)));
            if (v > 255) v = 255;
            if (p > 255) p = 255;
            if (q > 255) q = 255;
            if (t > 255) t = 255;
            if (v < 0) v = 0;
            if (p < 0) p = 0;
            if (q < 0) q = 0;
            if (t < 0) t = 0;

            switch (hi)
            {
                case 0:
                    Result = Color.FromArgb(255, v, t, p);
                    break;

                case 1:
                    Result = Color.FromArgb(255, q, v, p);
                    break;

                case 2:
                    Result = Color.FromArgb(255, p, v, t);
                    break;

                case 3:
                    Result = Color.FromArgb(255, p, q, v);
                    break;

                case 4:
                    Result = Color.FromArgb(255, t, p, v);
                    break;

                default:
                    Result = Color.FromArgb(255, v, p, q);
                    break;
            }
            return Result;
        }

        private void CreateColorBitmap()
        {
            cScreenBitmap = new Bitmap(cScreenBitmapWidth, cScreenBitmapHeight);
            for (int x = 0; x < cScreenBitmap.Width; x++)
            {
                for (int y = 0; y < cScreenBitmap.Height; y++)
                {
                    float hue = (float)x / cScreenBitmap.Width;
                    float brightness = 1 - (float)y / cScreenBitmap.Height;
                    Color color = ColorFromHSV(hue * 360, 1, brightness);
                    cScreenBitmap.SetPixel(x, y, color);
                }
            }
        }

        #endregion ScreenBitMapCode

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

        public void WriteActivityLog(string Message, bool Newline = false, bool NoDate = false)
        {
            string Line = "";
            string DF;
            try
            {
                string FileName = cSettingsDir + "\\Activity Log.txt";
                TrimFile(FileName);

                if (Newline) Line = "\r\n";

                if (NoDate)
                {
                    DF = "hh:mm:ss";
                }
                else
                {
                    DF = "MMM-dd hh:mm:ss";
                }

                File.AppendAllText(FileName, Line + DateTime.Now.ToString(DF) + "  -  " + Message + "\r\n");
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

        public void WriteLog(string LogName, string Message, bool NewLine = false, bool UseDate = false)
        {
            string Line = "";
            string DF = "";
            if (Message == null) Message = "";
            try
            {
                string FileName = cSettingsDir + "\\" + LogName;
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

        private void CheckFolders()
        {
            try
            {
                // SettingsDir
                cSettingsDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + cAppName;

                if (!Directory.Exists(cSettingsDir))
                {
                    Directory.CreateDirectory(cSettingsDir);
                    Properties.Settings.Default.FileName = "Default";
                }

                if (!File.Exists(cSettingsDir + "\\Example.rcs")) File.WriteAllBytes(cSettingsDir + "\\Example.rcs", Properties.Resources.Example);
                if (!File.Exists(cSettingsDir + "\\Default.rcs")) File.WriteAllBytes(cSettingsDir + "\\Default.rcs", Properties.Resources.Default);

                string FilesDir = Properties.Settings.Default.FilesDir;
                if (!Directory.Exists(FilesDir)) Properties.Settings.Default.FilesDir = cSettingsDir;
            }
            catch (Exception)
            {
            }
        }

        private void FormAdd(Form frm)
        {
            bool Found = false;
            for (int i = 0; i < OpenForms.Length; i++)
            {
                if (OpenForms[i] != null && OpenForms[i].Name == frm.Name)
                {
                    Found = true;
                    break;
                }
            }
            if (!Found)
            {
                for (int i = 0; i < OpenForms.Length; i++)
                {
                    if (OpenForms[i] == null)
                    {
                        OpenForms[i] = frm;
                        break;
                    }
                }
            }
        }

        private void FormRemove(Form frm)
        {
            for (int i = 0; i < OpenForms.Length; i++)
            {
                if (OpenForms[i] != null && OpenForms[i].Name == frm.Name)
                {
                    OpenForms[i] = null;
                    break;
                }
            }
        }

        private void LoadAppData(string path)
        {
            // property:  key=value  ex: "LastFile=Main.mdb"
            try
            {
                PropsApp.Clear();
                string[] lines = System.IO.File.ReadAllLines(path);
                foreach (string line in lines)
                {
                    if (line.Contains("=") && !String.IsNullOrEmpty(line.Split('=')[0]) && !String.IsNullOrEmpty(line.Split('=')[1]))
                    {
                        string[] splitText = line.Split('=');
                        PropsApp.Add(splitText[0], splitText[1]);
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
                PropsDictionary.Clear();
                string[] lines = System.IO.File.ReadAllLines(path);
                foreach (string line in lines)
                {
                    if (line.Contains("=") && !String.IsNullOrEmpty(line.Split('=')[0]) && !String.IsNullOrEmpty(line.Split('=')[1]))
                    {
                        string[] splitText = line.Split('=');
                        PropsDictionary.Add(splitText[0], splitText[1]);
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
                string[] NewLines = new string[PropsApp.Count];
                int i = -1;
                foreach (var Pair in PropsApp)
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
                string[] NewLines = new string[PropsDictionary.Count];
                int i = -1;
                foreach (var Pair in PropsDictionary)
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