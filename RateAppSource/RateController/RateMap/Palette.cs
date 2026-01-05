using System.Drawing;

namespace RateController.RateMap
{
    public static class Palette
    {
        public const byte ZoneTransparency = 150;

        private static readonly Color[] Colors = new Color[]
        {
            Color.Red, Color.Green, Color.Blue, Color.Orange,
            Color.Purple, Color.Teal, Color.Brown, Color.Magenta
        };

        public static Color GetColor(int index, byte trns = ZoneTransparency)
        {
            int safeIndex = index % Colors.Length;
            if (safeIndex < 0) safeIndex += Colors.Length;  // in case of a negative index

            var baseColor = Colors[safeIndex];
            return Color.FromArgb(trns, baseColor);
        }
    }
}