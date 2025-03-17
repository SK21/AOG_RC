using GMap.NET.WindowsForms;
using GMap.NET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.Classes
{
    public class AsAppliedMapLayerCreator
    {
        /// <summary>
        /// Creates a GMapOverlay using a grid whose cell area is defined in acres.
        /// For each grid cell the average applied rate (using the first applied rate value) is computed.
        /// Each cell is colored with one of five shades of green based on applied rate ranges.
        /// A legend mapping range labels to colors is output via the out parameter.
        /// </summary>
        /// <param name="readings">The list of RateReading data.</param>
        /// <param name="legend">Output dictionary with range labels mapped to their corresponding color.</param>
        /// <param name="cellAreaAcres">
        /// The area for a square grid cell in acres. Defaults to 0.1 (i.e. 1/10 acre).
        /// </param>
        /// <returns>A GMapOverlay containing the as‑applied map layer.</returns>
        public GMapOverlay CreateOverlay(List<RateReading> readings, out Dictionary<string, Color> legend, double cellAreaAcres = 0.1)
        {
            // Initialize the legend.
            legend = new Dictionary<string, Color>();

            // Create a new overlay for the map.
            GMapOverlay overlay = new GMapOverlay("asAppliedMap");

            if (readings == null || readings.Count == 0)
                return overlay;  // nothing to map

            // Determine the geographic bounding box.
            double minLat = readings.Min(r => r.Latitude);
            double maxLat = readings.Max(r => r.Latitude);
            double minLng = readings.Min(r => r.Longitude);
            double maxLng = readings.Max(r => r.Longitude);

            // Convert the cell area (in acres) to square meters.
            // 1 acre ≈ 4046.86 m².
            double cellAreaMeters = cellAreaAcres * 4046.86;
            // For a square cell, side = sqrt(area) (in meters).
            double cellSideMeters = Math.Sqrt(cellAreaMeters);

            // Convert cell side length from meters to degrees latitude.
            // 1 degree latitude ≈ 111,320 meters.
            double metersPerDegreeLat = 111320;
            double cellSizeDegreesLat = cellSideMeters / metersPerDegreeLat;

            // Convert cell side length from meters to degrees longitude.
            // Use the average latitude for adjustment.
            double avgLat = (minLat + maxLat) / 2.0;
            double metersPerDegreeLng = 111320 * Math.Cos(avgLat * Math.PI / 180.0);
            double cellSizeDegreesLng = cellSideMeters / metersPerDegreeLng;

            // Group readings into grid cells using calculated indices.
            Dictionary<(int i, int j), List<RateReading>> grid = new Dictionary<(int, int), List<RateReading>>();
            foreach (var reading in readings)
            {
                int i = (int)Math.Floor((reading.Latitude - minLat) / cellSizeDegreesLat);
                int j = (int)Math.Floor((reading.Longitude - minLng) / cellSizeDegreesLng);
                var key = (i, j);
                if (!grid.ContainsKey(key))
                    grid[key] = new List<RateReading>();
                grid[key].Add(reading);
            }

            // Compute the average applied rate per cell (using the first applied rate value).
            Dictionary<(int i, int j), double> cellAverages = new Dictionary<(int, int), double>();
            foreach (var kvp in grid)
            {
                double cellSum = kvp.Value.Sum(r => r.AppliedRates[0]);
                double cellAvg = cellSum / kvp.Value.Count;
                cellAverages[kvp.Key] = cellAvg;
            }

            // Determine overall minimum and maximum average applied rates.
            double overallMin = cellAverages.Values.Min();
            double overallMax = cellAverages.Values.Max();

            // Divide the range into five equal intervals.
            double rangeWidth = (overallMax - overallMin) / 5.0;

            // Define five shades of green (from light to dark).
            Color[] shadesOfGreen = new Color[5]
            {
                Color.LightGreen,
                Color.MediumSeaGreen,
                Color.SeaGreen,
                Color.ForestGreen,
                Color.DarkGreen
            };

            // Create range labels and build the legend.
            for (int k = 0; k < 5; k++)
            {
                double lowerBound = overallMin + k * rangeWidth;
                double upperBound = overallMin + (k + 1) * rangeWidth;
                string label = $"{lowerBound:F2} - {upperBound:F2}";
                legend[label] = shadesOfGreen[k];
            }

            // Create a polygon for each grid cell, color it based on its average applied rate.
            foreach (var kvp in cellAverages)
            {
                (int i, int j) cellKey = kvp.Key;
                double avgRate = kvp.Value;

                int rangeIndex;
                if (rangeWidth == 0)
                    rangeIndex = 0;
                else
                    rangeIndex = (int)Math.Floor((avgRate - overallMin) / rangeWidth);
                if (rangeIndex >= 5)
                    rangeIndex = 4;

                // Compute cell boundary.
                double cellMinLat = minLat + cellKey.i * cellSizeDegreesLat;
                double cellMinLng = minLng + cellKey.j * cellSizeDegreesLng;
                double cellMaxLat = cellMinLat + cellSizeDegreesLat;
                double cellMaxLng = cellMinLng + cellSizeDegreesLng;

                // Build polygon points for the cell.
                List<PointLatLng> points = new List<PointLatLng>
                {
                    new PointLatLng(cellMinLat, cellMinLng),
                    new PointLatLng(cellMinLat, cellMaxLng),
                    new PointLatLng(cellMaxLat, cellMaxLng),
                    new PointLatLng(cellMaxLat, cellMinLng)
                };

                // Create and style the polygon.
                GMapPolygon polygon = new GMapPolygon(points, $"Cell_{cellKey.i}_{cellKey.j}");
                polygon.Fill = new SolidBrush(shadesOfGreen[rangeIndex]);
                polygon.Stroke = new System.Drawing.Pen(System.Drawing.Color.Black, 1);

                overlay.Polygons.Add(polygon);
            }

            // Return the overlay. The legend dictionary can later be used to create a visual UI legend.
            return overlay;
        }
    }
}

