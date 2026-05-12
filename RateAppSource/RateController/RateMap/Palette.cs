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
            Color.Blue,
            Color.Green,
            Color.Yellow,
            Color.Orange,
            Color.Red
        };

        // Yield overlay: Blue=low yield, Red=high yield
        private static readonly Color[] YieldColors = new Color[]
        {
            Color.FromArgb(215, 25, 28),    // Red   - low yield
            Color.FromArgb(253, 174, 97),   // Orange
            Color.FromArgb(255, 255, 191),  // Yellow
            Color.FromArgb(166, 217, 106),  // Light green
            Color.FromArgb(26, 150, 65)     // Dark green - high yield
        };

        // Productivity zones: Red=low potential, Green=high potential
        private static readonly Color[] ProductivityColors = new Color[]
        {
            Color.FromArgb(215,  25,  28),  // Red        - lowest potential
            Color.FromArgb(244, 109,  67),  // Red-orange
            Color.FromArgb(255, 200,   0),  // Amber      (replaces near-white yellow)
            Color.FromArgb(120, 198,  67),  // Mid green
            Color.FromArgb( 26, 150,  65),  // Dark green - highest potential
        };

        /// <summary>Returns a colour for a productivity zone where z=0 is lowest potential.</summary>
        public static Color GetProductivityColor(int z, int zoneCount, byte trns = TargetZoneTransparency)
        {
            // Map z to the 5-entry productivity palette so full range is always used.
            int idx = (zoneCount <= 1) ? 4
                : (int)Math.Round(z * (ProductivityColors.Length - 1.0) / (zoneCount - 1.0));
            idx = Math.Max(0, Math.Min(ProductivityColors.Length - 1, idx));
            return Color.FromArgb(trns, ProductivityColors[idx]);
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