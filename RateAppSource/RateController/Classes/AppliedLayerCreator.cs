using GMap.NET.WindowsForms;
using GMap.NET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET.MapProviders;

namespace RateController.Classes
{
    public class AppliedLayerCreator
    {
        /// <summary>
        /// Creates a GMapOverlay using a grid whose cell area is defined in acres.
        /// For each grid cell, the average rate is computed based on a user-selected rate value
        /// from either the applied or target rates.
        /// Each cell is colored with one of five shades of green based on the computed rate ranges.
        /// A legend mapping range labels to colors is output via the out parameter.
        /// </summary>
        /// <param name="readings">The list of RateReading data.</param>
        /// <param name="legend">Output dictionary mapping range labels to colors.</param>
        /// <param name="cellAreaAcres">
        /// The area for a square grid cell in acres. Defaults to 0.1 (i.e. 1/10 acre).
        /// </param>
        /// <param name="selectedRateType">
        /// Specifies whether to use the AppliedRates or TargetRates. Defaults to Applied.
        /// </param>
        /// <param name="selectedRateIndex">
        /// The index (0-based) of the rate to use in the selected rate array.
        /// Defaults to 0.
        /// </param>
        public bool UpdateRatesOverlay(ref GMapOverlay overlay, List<RateReading> readings, out Dictionary<string, Color> legend,
            RectLatLng OverallBounds, double cellAreaAcres = 0.1, RateType selectedRateType = RateType.Applied, int selectedRateIndex = 0)
        {
            bool Result = false;
            legend = new Dictionary<string, Color>();
            overlay.Polygons.Clear();
            try
            {
                if (readings != null && readings.Count > 0)
                {
                    #region build box
                    //// Determine the geographic bounding box.
                    //double minLat = readings.Min(r => r.Latitude);
                    //double maxLat = readings.Max(r => r.Latitude);
                    //double minLng = readings.Min(r => r.Longitude);
                    //double maxLng = readings.Max(r => r.Longitude);

                    double minLat = OverallBounds.Top;
                    double maxLat = OverallBounds.Bottom;
                    double minLng = OverallBounds.Left;
                    double maxLng = OverallBounds.Right;

                    // Convert the cell area (in acres) to square meters.
                    // 1 acre ≈ 4046.86 m².
                    double cellAreaMeters = cellAreaAcres * 4046.86;
                    // For a square cell, side = sqrt(area) (in meters).
                    double cellSideMeters = Math.Sqrt(cellAreaMeters);

                    // Convert cell side length from meters to degrees latitude.
                    // 1 degree latitude ≈ 111,320 meters.
                    double metersPerDegreeLat = 111320;
                    double cellSizeDegreesLat = cellSideMeters / metersPerDegreeLat;

                    // Convert cell side length from meters to degrees longitude using the average latitude.
                    double avgLat = (minLat + maxLat) / 2.0;
                    double metersPerDegreeLng = 111320 * Math.Cos(avgLat * Math.PI / 180.0);
                    double cellSizeDegreesLng = cellSideMeters / metersPerDegreeLng;
                    #endregion
                    #region group readings
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

                    // Compute the average rate per cell.
                    // Only include readings that actually have at least (selectedRateIndex+1) entries in the selected array.
                    Dictionary<(int i, int j), double> cellAverages = new Dictionary<(int, int), double>();
                    foreach (var kvp in grid)
                    {
                        // Get the valid rates from this cell.
                        var validRates = kvp.Value
                            .Where(r => (selectedRateType == RateType.Applied && r.AppliedRates.Length > selectedRateIndex) ||
                                        (selectedRateType == RateType.Target && r.TargetRates.Length > selectedRateIndex))
                            .Select(r => selectedRateType == RateType.Applied ? r.AppliedRates[selectedRateIndex] : r.TargetRates[selectedRateIndex])
                            .ToList();

                        if (validRates.Count > 0)
                        {
                            double cellAvg = validRates.Average();
                            cellAverages[kvp.Key] = cellAvg;
                        }
                    }
                    #endregion

                    // If no valid averages, return an empty overlay.
                    if (cellAverages.Count > 0)
                    {
                        #region Build Legend
                        // Determine overall minimum and maximum for the computed averages.
                        double overallMin = cellAverages.Values.Min();
                        double overallMax = cellAverages.Values.Max();

                        // Divide the range into five equal intervals.
                        double rangeWidth = (overallMax - overallMin) / 5.0;

                        // Define five shades of green (from light to dark).
                        Color[] shadesOfGreen = new Color[5]
                        {
                            Color.FromArgb(204, 255, 204),  // A very light green (almost pastel)
                            Color.FromArgb(153, 255, 153),  // A light, minty green
                            Color.FromArgb(102, 255, 102),  // A bright, vibrant lime green
                            Color.FromArgb(51, 204, 51),    // A medium green
                            Color.FromArgb(0, 153, 0)       // A dark, rich green
                        };
                        // Convert each color to a semi-transparent version:
                        for (int i = 0; i < shadesOfGreen.Length; i++)
                        {
                            shadesOfGreen[i] = Color.FromArgb(175, shadesOfGreen[i]);
                        }

                        // Build the legend.
                        for (int k = 0; k < 5; k++)
                        {
                            double lowerBound = overallMin + k * rangeWidth;
                            double upperBound = overallMin + (k + 1) * rangeWidth;
                            string label = $"{lowerBound:F2} - {upperBound:F2}";
                            legend[label] = shadesOfGreen[k];
                        }
                        #endregion
                        #region Create polygons
                        // Create a polygon for each grid cell, coloring it based on its average rate.
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

                            // Calculate cell boundaries.
                            double cellMinLat = minLat + cellKey.i * cellSizeDegreesLat;
                            double cellMinLng = minLng + cellKey.j * cellSizeDegreesLng;
                            double cellMaxLat = cellMinLat + cellSizeDegreesLat;
                            double cellMaxLng = cellMinLng + cellSizeDegreesLng;

                            List<PointLatLng> points = new List<PointLatLng>
                            {
                                new PointLatLng(cellMinLat, cellMinLng),
                                new PointLatLng(cellMinLat, cellMaxLng),
                                new PointLatLng(cellMaxLat, cellMaxLng),
                                new PointLatLng(cellMaxLat, cellMinLng)
                            };

                            GMapPolygon polygon = new GMapPolygon(points, $"Cell_{cellKey.i}_{cellKey.j}");
                            polygon.Stroke = new Pen(shadesOfGreen[rangeIndex], 2);
                            polygon.Fill = new SolidBrush(shadesOfGreen[rangeIndex]);

                            overlay.Polygons.Add(polygon);
                        }
                        Result = true;
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("AsAppliedCreator/CreateOverlay: " + ex.Message);
            }
            return Result;
        }
    }
}