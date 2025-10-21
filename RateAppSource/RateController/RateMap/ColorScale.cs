using System;
using System.Drawing;

namespace RateController.Classes
{
    /// <summary>
    /// Centralized color interpolation helpers for rate -> color mapping.
    /// </summary>
    public static class ColorScale
    {
        // Simple 5-stop scale (blue -> cyan -> green -> yellow -> red)
        private static readonly Color[] Stops = new[]
        {
            Color.FromArgb(0,  87, 255),   // Blue-ish
            Color.FromArgb(0, 191, 255),   // Deep sky
            Color.FromArgb(0, 200,  70),   // Green-ish
            Color.FromArgb(255, 215,  0),  // Gold
            Color.FromArgb(220,  20,  60)  // Crimson
        };

        /// <summary>
        /// Interpolates a color for the given value within [min, max].
        /// </summary>
        public static Color Interpolate(double min, double max, double value)
        {
            if (double.IsNaN(min) || double.IsNaN(max) || max <= min) return Color.Gray;

            double t = (value - min) / (max - min);
            if (t <= 0) return Stops[0];
            if (t >= 1) return Stops[Stops.Length - 1];

            double pos = t * (Stops.Length - 1);
            int i = (int)Math.Floor(pos);
            double frac = pos - i;

            var a = Stops[i];
            var b = Stops[i + 1];

            byte r = (byte)(a.R + (b.R - a.R) * frac);
            byte g = (byte)(a.G + (b.G - a.G) * frac);
            byte bl = (byte)(a.B + (b.B - a.B) * frac);

            return Color.FromArgb(r, g, bl);
        }
    }
}