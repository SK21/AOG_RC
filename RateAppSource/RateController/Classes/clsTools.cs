using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.Classes
{
    public class clsTools
    {
        #region ScreenBitMap

        private Bitmap cScreenBitmap;
        private int cScreenBitmapHeight = 465;  // from frmMenuColor colorPanel
        private int cScreenBitmapWidth = 516;

        #endregion ScreenBitMap

        public clsTools()
        {
            _ = InitializeAsync();
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

        public string ClipText(string text, int length)
        {
            string Result = text;
            if (text.Length > length)
            {
                Result = text.Substring(0, length);
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

        public bool UseLightContrast()
        {
            Color background = Properties.Settings.Default.DisplayBackColour;
            double luminance =
                0.2126 * background.R +
                0.7152 * background.G +
                0.0722 * background.B;

            return luminance < 128;
            // true  = background is dark → use light circle
            // false = background is light → use dark circle
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

        private async Task InitializeAsync()
        {
            await Task.Run(() => CreateColorBitmap());
        }

        #endregion ScreenBitMapCode
    }
}
