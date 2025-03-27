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
    public class clsTools
    {
        private string lastMessage;
        private DateTime lastMessageTime;
        private FormStart mf;

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
            CreateColorBitmap();

            lastMessage = string.Empty;
            lastMessageTime = DateTime.MinValue;
            cRateCollector=new DataCollector();
        }

        public MapManager Manager
        { get { return cManager; } }

        public DataCollector RateCollector
        { get { return cRateCollector; } }

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
        public void DrawGroupBox(GroupBox box, Graphics g, Color BackColor, Color textColor, Color borderColor, float borderWidth = 1)
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


        public void DrawGroupBox2(GroupBox box, Graphics g, Color BackColor, Color textColor, Color borderColor)
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

        public bool GoodCRC(byte[] Data, byte Start = 0)
        {
            bool Result = false;
            int Length = Data.Length;
            byte cr = CRC(Data, Length - 1, Start);
            Result = (cr == Data[Length - 1]);
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

                if (LogError) Props.WriteErrorLog(Message);
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
    }
}