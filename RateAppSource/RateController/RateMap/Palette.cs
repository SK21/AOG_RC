using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.RateMap
{
    public static class Palette
    {
        public const byte TargetZoneTransparency = 100;
        public const byte ZoneTransparency = 150;

        private static readonly Color[] Colors = new Color[]
        {
            Color.Blue,     // lowest rate
            Color.Green,    // low–moderate
            Color.Yellow,   // moderate
            Color.Orange,   // high
            Color.Red       // highest rate
        };

        private static readonly Color[] YieldColors = new Color[]
        {
            Color.FromArgb(215, 25, 28),    // Red - low yield
            Color.FromArgb(253, 174, 97),   // Orange
            Color.FromArgb(255, 255, 191),  // Yellow
            Color.FromArgb(166, 217, 106),  // Light green
            Color.FromArgb(26, 150, 65)     // Dark green - high yield
        };

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