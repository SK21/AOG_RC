using System;
using System.Drawing;

namespace RateController.RateMap
{
    public static class Palette
    {
        public const byte TargetZoneTransparency = 150;
        public const byte ZoneTransparency = 150;

        // Rate zones: Blue=low rate, Red=high rate
        private static readonly Color[] Colors = new Color[]
        {
            Color.FromArgb(  0,   0, 180),  // Dark blue   - lowest rate
            Color.FromArgb(  0,  80, 255),  // Blue
            Color.FromArgb(  0, 200, 220),  // Cyan
            Color.FromArgb(  0, 180, 160),  // Teal
            Color.FromArgb(  0, 170,   0),  // Green
            Color.FromArgb(150, 210,   0),  // Yellow-green
            Color.FromArgb(255, 230,   0),  // Yellow
            Color.FromArgb(255, 165,   0),  // Amber
            Color.FromArgb(255,  80,   0),  // Orange
            Color.FromArgb(200,   0,   0),  // Red         - highest rate
        };

        // Yield overlay: Low yield=dark red, High yield=dark green
        private static readonly Color[] YieldColors = new Color[]
        {
            Color.FromArgb(140,   0,   0),  // Dark red    - lowest yield
            Color.FromArgb(215,  25,  28),  // Red
            Color.FromArgb(244, 109,  67),  // Red-orange
            Color.FromArgb(253, 174,  97),  // Orange
            Color.FromArgb(255, 220,   0),  // Amber
            Color.FromArgb(255, 255, 150),  // Light yellow
            Color.FromArgb(200, 230,  80),  // Yellow-green
            Color.FromArgb(166, 217, 106),  // Light green
            Color.FromArgb( 26, 150,  65),  // Green
            Color.FromArgb(  0,  90,  40),  // Dark green  - highest yield
        };

        // Productivity zones: Red=low potential, Green=high potential
        private static readonly Color[] ProductivityColors = new Color[]
        {
            Color.FromArgb(140,   0,   0),  // Dark red    - lowest potential
            Color.FromArgb(215,  25,  28),  // Red
            Color.FromArgb(244, 109,  67),  // Red-orange
            Color.FromArgb(255, 160,   0),  // Orange
            Color.FromArgb(255, 210,   0),  // Amber
            Color.FromArgb(240, 240,   0),  // Yellow
            Color.FromArgb(180, 220,  50),  // Yellow-green
            Color.FromArgb(120, 198,  67),  // Light green
            Color.FromArgb( 26, 150,  65),  // Green
            Color.FromArgb(  0,  90,  40),  // Dark green  - highest potential
        };

        /// <summary>Returns a colour for a productivity zone where z=0 is lowest potential.</summary>
        public static Color GetProductivityColor(int z, int zoneCount, byte trns = TargetZoneTransparency)
        {
            if (zoneCount <= 1)
                return Color.FromArgb(trns, ProductivityColors[ProductivityColors.Length - 1]);

            // Interpolate across the palette so every zone gets a distinct colour.
            double t  = (double)z / (zoneCount - 1) * (ProductivityColors.Length - 1);
            int    lo = Math.Max(0, Math.Min(ProductivityColors.Length - 2, (int)t));
            double f  = t - lo;
            Color  a  = ProductivityColors[lo];
            Color  b  = ProductivityColors[lo + 1];
            return Color.FromArgb(trns,
                (int)(a.R + (b.R - a.R) * f),
                (int)(a.G + (b.G - a.G) * f),
                (int)(a.B + (b.B - a.B) * f));
        }

        public static Color GetColor(int index, bool IsYieldData = false, byte trns = ZoneTransparency)
        {
            Color Result;

            if (IsYieldData)
            {
                int safeIndex = index % YieldColors.Length;
                if (safeIndex < 0) safeIndex += YieldColors.Length;  // in case of a negative index
                Result = YieldColors[safeIndex];
            }
            else
            {
                int safeIndex = index % Colors.Length;
                if (safeIndex < 0) safeIndex += Colors.Length;  // in case of a negative index
                Result = Colors[safeIndex];
            }

            return Color.FromArgb(trns, Result);
        }
    }
}