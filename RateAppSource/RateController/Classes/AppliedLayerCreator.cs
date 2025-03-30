using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace RateController.Classes
{
    public class AppliedLayerCreator
    {


        /// <summary>
        /// Updates the GMapOverlay using a pre-computed grid of cells.
        /// For each cell, the average rate is computed from readings that fall within its bounds.
        /// Each cell is colored according to its computed average rate using a five‑shade green scheme.
        /// </summary>
        /// <param name="overlay">GMapOverlay to update.</param>
        /// <param name="readings">List of RateReading data.</param>
        /// <param name="grid">Pre‑computed grid cells as RectLatLng.</param>
        /// <param name="legend">Output dictionary mapping range labels to colors.</param>
        /// <param name="selectedRateType">Specifies whether to use Applied or Target rates.</param>
        /// <param name="selectedRateIndex">Index (0‑based) of the rate to use.</param>
        /// <returns>True if update succeeds; otherwise, false.</returns>
        public bool UpdateRatesOverlay(
            ref GMapOverlay overlay,
            List<RateReading> readings,
            List<RectLatLng> grid,
            out Dictionary<string, Color> legend,
            RateType selectedRateType = RateType.Applied,
            int selectedRateIndex = 0)
        {
            bool Result = false;
            legend = new Dictionary<string, Color>();
            overlay.Polygons.Clear();

            if (readings == null || readings.Count == 0 || grid == null || grid.Count == 0)
                return Result;

            // Compute the average rate per grid cell.
            // We'll use the index in the grid list as the key.
            Dictionary<int, double> cellAverages = new Dictionary<int, double>();

            // For each grid cell, find the readings whose coordinates fall within the cell.
            // (Assumes that RectLatLng exposes Bottom, Top, Left, and Right.)
            for (int idx = 0; idx < grid.Count; idx++)
            {
                RectLatLng cell = grid[idx];
                List<RateReading> cellReadings = readings.FindAll(r =>
                    r.Latitude >= cell.Bottom &&
                    r.Latitude <= cell.Top &&
                    r.Longitude >= cell.Left &&
                    r.Longitude <= cell.Right);

                // Select valid rates.
                var validRates = cellReadings.Where(r =>
                        (selectedRateType == RateType.Applied && r.AppliedRates.Length > selectedRateIndex) ||
                        (selectedRateType == RateType.Target && r.TargetRates.Length > selectedRateIndex))
                    .Select(r => selectedRateType == RateType.Applied ? r.AppliedRates[selectedRateIndex] : r.TargetRates[selectedRateIndex])
                    .ToList();

                if (validRates.Count > 0)
                    cellAverages[idx] = validRates.Average();
            }

            // Define five shades of green (from very light to dark) with semi-transparency.
            Color[] shadesOfGreen = new Color[5]
            {
                Color.FromArgb(175,175, 240, 175), // Very light
                Color.FromArgb(175, 153, 225, 153), // Light minty
                Color.FromArgb(175, 102, 190, 102), // Vibrant lime
                Color.FromArgb(175, 51, 160, 51),   // Medium
                Color.FromArgb(175, 0, 130, 0)        // Dark
            };

            double overallMin = 0, overallMax = 0, rangeWidth = 0;
            if (cellAverages.Count > 0)
            {
                overallMin = cellAverages.Values.Min();
                overallMax = cellAverages.Values.Max();
                rangeWidth = (overallMax - overallMin) / 5.0;

                // Build the legend.
                for (int k = 0; k < 5; k++)
                {
                    double lowerBound = overallMin + k * rangeWidth;
                    double upperBound = overallMin + (k + 1) * rangeWidth;
                    string label = $"{lowerBound:F0} - {upperBound:F0}";
                    legend[label] = shadesOfGreen[k];
                }
            }

            // Create a polygon for each grid cell.
            for (int idx = 0; idx < grid.Count; idx++)
            {
                RectLatLng cell = grid[idx];

                // Define cell polygon points.
                List<PointLatLng> points = new List<PointLatLng>
                {
                    new PointLatLng(cell.Bottom, cell.Left),
                    new PointLatLng(cell.Bottom, cell.Right),
                    new PointLatLng(cell.Top, cell.Right),
                    new PointLatLng(cell.Top, cell.Left)
                };

                GMapPolygon polygon = new GMapPolygon(points, $"Cell_{idx}");

                // Color the cell according to its computed average, if available.
                if (cellAverages.ContainsKey(idx) && rangeWidth > 0)
                {
                    double avgRate = cellAverages[idx];
                    int rangeIndex = (int)Math.Floor((avgRate - overallMin) / rangeWidth);
                    if (rangeIndex >= 5)
                        rangeIndex = 4;
                    polygon.Stroke = new Pen(shadesOfGreen[rangeIndex], 2);
                    polygon.Fill = new SolidBrush(shadesOfGreen[rangeIndex]);
                }
                else
                {
                    // Use a light, transparent gray for cells with no data.
                    Color noDataColor = Color.FromArgb(25, Color.Gray);
                    polygon.Stroke = new Pen(noDataColor, 1);
                    polygon.Fill = new SolidBrush(noDataColor);
                }

                overlay.Polygons.Add(polygon);
            }

            Result = true;
            return Result;
        }
    }
}