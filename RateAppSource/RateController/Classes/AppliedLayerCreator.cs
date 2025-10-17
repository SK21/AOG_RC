using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using RateController;

namespace RateController.Classes
{
    public class AppliedLayerCreator
    {

        /// <summary>
        /// Updates the GMapOverlay using a pre-computed grid of cells.
        /// For each cell, the average rate is computed from readings that fall within its bounds.
        /// Each cell is colored according to its computed average rate using a five‑shade green scheme.
        /// If a non-null clsSections instance is provided the renderer will size each displayed cell
        /// so its width matches the implement TotalWidth (meters) returned by clsSections.TotalWidth().
        /// </summary>
        /// <param name="overlay">GMapOverlay to update.</param>
        /// <param name="readings">List of RateReading data.</param>
        /// <param name="grid">Pre‑computed grid cells as RectLatLng.</param>
        /// <param name="legend">Output dictionary mapping range labels to colors.</param>
        /// <param name="selectedRateType">Specifies whether to use Applied or Target rates.</param>
        /// <param name="selectedRateIndex">Index (0‑based) of the rate to use.</param>
        /// <param name="sections">Optional clsSections instance. When provided, sections.TotalWidth() (meters) is used as implement width.</param>
        /// <returns>True if update succeeds; otherwise, false.</returns>
        public bool UpdateRatesOverlay(
            ref GMapOverlay overlay,
            List<RateReading> readings,
            List<RectLatLng> grid,
            out Dictionary<string, Color> legend,
            RateType selectedRateType = RateType.Applied,
            int selectedRateIndex = 0,
            clsSections sections = null)
        {
            bool Result = false;
            legend = new Dictionary<string, Color>();
            overlay.Polygons.Clear();

            if (readings == null || readings.Count == 0 || grid == null || grid.Count == 0)
                return Result;

            // Determine implement width in meters (0.0 means use precomputed grid cell size).
            double implementWidthMeters = 0.0;
            if (sections != null)
            {
                try
                {
                    // clsSections.TotalWidth(false) returns meters when ReturnFeet==false
                    implementWidthMeters = sections.TotalWidth(false);
                }
                catch
                {
                    implementWidthMeters = 0.0;
                }
            }

            // Compute the average rate per grid cell.
            // Also keep the bounds we used per cell so rendering matches aggregation.
            Dictionary<int, double> cellAverages = new Dictionary<int, double>();
            int cellCount = grid.Count;
            double[] cellBottoms = new double[cellCount];
            double[] cellTops = new double[cellCount];
            double[] cellLefts = new double[cellCount];
            double[] cellRights = new double[cellCount];

            // Approximate conversion constants:
            // 1 degree latitude ~= 111,320 meters
            const double metersPerDegreeLat = 111320.0;

            for (int idx = 0; idx < cellCount; idx++)
            {
                RectLatLng cell = grid[idx];

                double cellBottom, cellTop, cellLeft, cellRight;

                if (implementWidthMeters > 0.0)
                {
                    // center of the provided grid-cell
                    double centerLat = (cell.Top + cell.Bottom) / 2.0;
                    double centerLng = (cell.Left + cell.Right) / 2.0;

                    // meters per degree longitude depends on latitude
                    double metersPerDegreeLng = metersPerDegreeLat * Math.Max(0.000001, Math.Cos(centerLat * Math.PI / 180.0));

                    double halfDegLat = (implementWidthMeters / 2.0) / metersPerDegreeLat;
                    double halfDegLng = (implementWidthMeters / 2.0) / metersPerDegreeLng;

                    cellBottom = centerLat - halfDegLat;
                    cellTop = centerLat + halfDegLat;
                    cellLeft = centerLng - halfDegLng;
                    cellRight = centerLng + halfDegLng;
                }
                else
                {
                    // Use existing precomputed cell bounds
                    cellBottom = cell.Bottom;
                    cellTop = cell.Top;
                    cellLeft = cell.Left;
                    cellRight = cell.Right;
                }

                // store bounds so polygon drawing uses same area
                cellBottoms[idx] = cellBottom;
                cellTops[idx] = cellTop;
                cellLefts[idx] = cellLeft;
                cellRights[idx] = cellRight;

                // Find readings within these bounds.
                List<RateReading> cellReadings = readings.FindAll(r =>
                    r.Latitude >= cellBottom &&
                    r.Latitude <= cellTop &&
                    r.Longitude >= cellLeft &&
                    r.Longitude <= cellRight);

                // Select valid rates for the chosen rate type and index.
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

            // Create a polygon for each grid cell using the stored bounds.
            for (int idx = 0; idx < cellCount; idx++)
            {
                double cellBottom = cellBottoms[idx];
                double cellTop = cellTops[idx];
                double cellLeft = cellLefts[idx];
                double cellRight = cellRights[idx];

                // Define cell polygon points (lat, lng).
                List<PointLatLng> points = new List<PointLatLng>
                {
                    new PointLatLng(cellBottom, cellLeft),
                    new PointLatLng(cellBottom, cellRight),
                    new PointLatLng(cellTop, cellRight),
                    new PointLatLng(cellTop, cellLeft)
                };

                GMapPolygon polygon = new GMapPolygon(points, $"Cell_{idx}");

                // Color the cell according to its computed average, if available.
                if (cellAverages.ContainsKey(idx) && rangeWidth > 0)
                {
                    double avgRate = cellAverages[idx];
                    int rangeIndex = (int)Math.Floor((avgRate - overallMin) / rangeWidth);
                    if (rangeIndex >= 5)
                        rangeIndex = 4;
                    if (rangeIndex < 0)
                        rangeIndex = 0;
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