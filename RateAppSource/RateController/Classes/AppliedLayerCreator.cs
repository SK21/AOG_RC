using GMap.NET.WindowsForms;
using GMap.NET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GMap.NET.MapProviders;
using System.Windows.Forms.DataVisualization.Charting;

namespace RateController.Classes
{
    public class AppliedLayerCreator
    {
        /// <summary>
        /// Creates a GMapOverlay with rotated rectangular boxes computed over segments of the entire
        /// list of RateReading data. Each segment (for example, 5 readings per segment) is processed to produce
        /// a box that is positioned at the segment’s midpoint and rotated according to its travel direction.
        /// The box’s color is chosen from five green shades according to a shading range that goes from 0
        /// to 10% over the largest target rate. A legend is built so that the mapping of intervals to color is clear.
        /// 
        /// This version clears all polygons before starting and then redraws a box for each segment,
        /// thereby “retaining”/redrawing all previous segments.
        /// 
        /// Parameters:
        ///   overlay         - The GMapOverlay to which the polygons will be added.
        ///   readings        - The list of RateReading data.
        ///   legend          - Output dictionary mapping range labels to colors.
        ///   cellAreaAcres   - The desired area for each rotated box (in acres).
        ///   boxWidthMeters  - The box width (the side perpendicular to the travel direction) in meters.
        ///   selectedRateType- Specifies whether to use the Applied or Target rate values.
        ///   selectedRateIndex - The index (0-based) of the rate within each reading’s array.
        /// </summary>
        public bool UpdateRatesOverlay(
            ref GMapOverlay overlay,
            List<RateReading> readings,
            out Dictionary<string, Color> legend,
            double cellAreaAcres = 0.1,
            double ImplementWidth_Feet = 100,
            RateType selectedRateType = RateType.Applied,
            int selectedRateIndex = 0)
        {
            bool Result = false;
            legend = new Dictionary<string, Color>();
            cellAreaAcres = 0.1;

            // Clear previously drawn polygons so that we will redraw them all.
            overlay.Polygons.Clear();

            try
            {
                if (readings != null && readings.Count > 0)
                {
                    #region build legend
                    // ------------------------------------------------------------------------
                    // 1. Compute shading boundaries:
                    //    - Lower bound is fixed at 0.
                    //    - Upper bound is 10% above the largest target rate.
                    //    These boundaries will be used to partition the range into 5 intervals.
                    // ------------------------------------------------------------------------
                    var validTargetRates = readings
                        .Where(r => r.TargetRates.Length > selectedRateIndex)
                        .Select(r => r.TargetRates[selectedRateIndex])
                        .ToList();
                    if (validTargetRates.Count == 0)
                        return false;

                    double shadingMin = 0.0;
                    double shadingMax = validTargetRates.Max() * 1.1;
                    double rangeWidth = (shadingMax - shadingMin) / 5.0;

                    // Define the 5 green shades (each with alpha = 175).
                    // (Note that the first color has been updated to use 4 parameters.)
                    Color[] shadesOfGreen = new Color[5]
                    {
                        Color.FromArgb(175, 175, 255, 175),  // Very light green.
                        Color.FromArgb(175, 153, 255, 153),  // Light, minty green.
                        Color.FromArgb(175, 102, 255, 102),  // Bright, vibrant green.
                        Color.FromArgb(175, 51, 204, 51),    // Medium green.
                        Color.FromArgb(175, 0, 153, 0)         // Dark, rich green.
                    };

                    // Build the legend mapping each interval to a green shade.
                    for (int k = 0; k < 5; k++)
                    {
                        double lowerBound = shadingMin + k * rangeWidth;
                        double upperBound = shadingMin + (k + 1) * rangeWidth;
                        string label = $"{lowerBound:F1} - {upperBound:F1}";
                        legend[label] = shadesOfGreen[k];
                    }
                    #endregion

                    // ------------------------------------------------------------------------
                    // 2. Partition the readings into segments and compute a rotated box for each segment.
                    //    Here we use segments of up to 5 readings. (If fewer exist at the end, we use the
                    //    last available reading for the segment.)
                    // ------------------------------------------------------------------------
                    for (int segStart = 0; segStart < readings.Count; segStart += 5)
                    {
                        #region segment rates and box color
                        int segEnd = (segStart + 4 < readings.Count) ? segStart + 4 : readings.Count - 1;
                        var segment = readings.GetRange(segStart, segEnd - segStart + 1);

                        // Filter for valid rates in this segment (using the selected rate type).
                        var validSegmentRates = segment
                            .Where(r => (selectedRateType == RateType.Applied && r.AppliedRates.Length > selectedRateIndex) ||
                                        (selectedRateType == RateType.Target && r.TargetRates.Length > selectedRateIndex))
                            .Select(r => selectedRateType == RateType.Applied ? r.AppliedRates[selectedRateIndex] : r.TargetRates[selectedRateIndex])
                            .ToList();
                        if (validSegmentRates.Count == 0)
                            continue; // Skip this segment if no valid rate data is available.

                        double segmentAvg = validSegmentRates.Average();
                        int shadeIndex = (rangeWidth == 0) ? 0 : (int)Math.Floor((segmentAvg - shadingMin) / rangeWidth);
                        if (shadeIndex < 0)
                            shadeIndex = 0;
                        if (shadeIndex >= 5)
                            shadeIndex = 4;
                        Color boxColor = shadesOfGreen[shadeIndex];
                        #endregion

                        #region travel direction and box center
                        // --------------------------------------------------------------------
                        // 3. Compute the segment’s travel direction and box center.
                        //    Use the first and last reading in the segment.
                        // --------------------------------------------------------------------
                        RateReading startReading = segment.First();
                        RateReading endReading = segment.Last();

                        double travelBearingDeg = CalculateBearing(
                            startReading.Latitude, startReading.Longitude,
                            endReading.Latitude, endReading.Longitude);

                        // Center the box at the midpoint of the segment.
                        double centerLat = (startReading.Latitude + endReading.Latitude) / 2.0;
                        double centerLng = (startReading.Longitude + endReading.Longitude) / 2.0;
                        #endregion

                        #region box dimensions
                        // --------------------------------------------------------------------
                        // 4. Compute the rectangle (box) dimensions.
                        //    Convert the desired area (in acres) to square meters.
                        //    Then compute the box’s length so that:
                        //         area = length * boxWidthMeters.
                        // --------------------------------------------------------------------
                        double areaMeters = cellAreaAcres * 4046.86;
                        double rectangleLength = areaMeters / (ImplementWidth_Feet * 0.3048);
                        double halfLength = rectangleLength / 2.0;
                        double halfWidth = (ImplementWidth_Feet * 0.3048) / 2.0;

                        // Convert travel bearing to radians.
                        double theta = travelBearingDeg * Math.PI / 180.0;

                        // Compute displacement vectors:
                        //   - V: along travel direction (half-length vector).
                        //   - W: perpendicular to travel direction (half-width vector).
                        double Vx = halfLength * Math.Sin(theta);
                        double Vy = halfLength * Math.Cos(theta);
                        double Wx = halfWidth * Math.Sin(theta + Math.PI / 2);
                        double Wy = halfWidth * Math.Cos(theta + Math.PI / 2);

                        // Determine the four corners of the rotated box via vector addition.
                        double dx1 = Vx + Wx;
                        double dy1 = Vy + Wy;
                        double dx2 = Vx - Wx;
                        double dy2 = Vy - Wy;
                        double dx3 = -Vx - Wx;
                        double dy3 = -Vy - Wy;
                        double dx4 = -Vx + Wx;
                        double dy4 = -Vy + Wy;
                        #endregion

                        #region box points
                        // --------------------------------------------------------------------
                        // 5. Convert meter offsets to latitude/longitude degrees.
                        //    (Using approximate conversion factors.)
                        // --------------------------------------------------------------------
                        double metersPerDegreeLat = 111320; // Approximate.
                        double metersPerDegreeLng = 111320 * Math.Cos(centerLat * Math.PI / 180.0);

                        double lat1 = centerLat + dy1 / metersPerDegreeLat;
                        double lng1 = centerLng + dx1 / metersPerDegreeLng;
                        double lat2 = centerLat + dy2 / metersPerDegreeLat;
                        double lng2 = centerLng + dx2 / metersPerDegreeLng;
                        double lat3 = centerLat + dy3 / metersPerDegreeLat;
                        double lng3 = centerLng + dx3 / metersPerDegreeLng;
                        double lat4 = centerLat + dy4 / metersPerDegreeLat;
                        double lng4 = centerLng + dx4 / metersPerDegreeLng;

                        List<PointLatLng> boxPoints = new List<PointLatLng>
                        {
                            new PointLatLng(lat1, lng1),
                            new PointLatLng(lat2, lng2),
                            new PointLatLng(lat3, lng3),
                            new PointLatLng(lat4, lng4)
                        };
                        #endregion

                        #region polygon
                        // Create the polygon for this segment.
                        GMapPolygon polygon = new GMapPolygon(boxPoints, $"SegmentBox_{segStart}_{segEnd}");
                        polygon.Stroke = new Pen(boxColor, 2);
                        polygon.Fill = new SolidBrush(boxColor);

                        overlay.Polygons.Add(polygon);
                        #endregion
                    }

                    Result = true;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("AppliedLayerCreator/UpdateRatesOverlay: " + ex.Message);
            }
            return Result;
        }

        /// <summary>
        /// Calculates the bearing (in degrees) from (lat1, lng1) to (lat2, lng2).
        /// Bearing is measured clockwise from north.
        /// </summary>
        private double CalculateBearing(double lat1, double lng1, double lat2, double lng2)
        {
            double radLat1 = lat1 * Math.PI / 180.0;
            double radLat2 = lat2 * Math.PI / 180.0;
            double deltaLng = (lng2 - lng1) * Math.PI / 180.0;

            double y = Math.Sin(deltaLng) * Math.Cos(radLat2);
            double x = Math.Cos(radLat1) * Math.Sin(radLat2) -
                       Math.Sin(radLat1) * Math.Cos(radLat2) * Math.Cos(deltaLng);
            double bearingRad = Math.Atan2(y, x);
            double bearingDeg = (bearingRad * 180.0 / Math.PI + 360) % 360;
            return bearingDeg;
        }


    }
}