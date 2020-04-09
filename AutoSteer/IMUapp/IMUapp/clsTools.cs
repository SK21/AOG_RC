using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace IMUapp
{
    public class clsTools
    {
        private string cAppName = "IMUapp";
        private string cVersionDate = "08-Apr-2020";
        private FormMain mf;
        private string SettingsDir;

        public clsTools(FormMain CallingForm)
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
                File.AppendAllText(FileName, DateTime.Now.ToString() + "  -  " + Message + "\r\n");
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
            catch (Exception)
            {
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
            catch (Exception)
            {
            }
        }

        private void TrimFile(string FileName, int MaxSize = 50000)
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
    }
}
