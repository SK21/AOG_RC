using System;
using System.Collections.Generic;
using System.Drawing;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace RateController.Classes
{
    public class CoverageTrail
    {
        public class TrailPoint
        {
            public PointLatLng Position { get; set; }
            public double Heading { get; set; }
            public double Rate { get; set; }
            public DateTime Timestamp { get; set; }
            public double ImplementWidthMeters { get; set; }

            public TrailPoint(PointLatLng pos, double heading, double rate, double width)
            {
                Position = pos;
                Heading = heading;
                Rate = rate;
                ImplementWidthMeters = width;
                Timestamp = DateTime.Now;
            }

            public List<PointLatLng> GetImplementEdges()
            {
                // Convert heading to radians
                // Note: Heading is 0° = North, 90° = East, etc.
                // We need to rotate 90° counter-clockwise to get implement width perpendicular to travel
                double perpHeadingRad = ((Heading - 90) % 360) * Math.PI / 180.0;

                // Calculate implement edge points
                const double metersPerDegreeLat = 111320.0;
                double metersPerDegreeLng = metersPerDegreeLat * Math.Cos(Position.Lat * Math.PI / 180.0);

                // Calculate offsets for implement edges perpendicular to travel direction
                double halfWidthLat = (ImplementWidthMeters / 2.0) * Math.Cos(perpHeadingRad) / metersPerDegreeLat;
                double halfWidthLng = (ImplementWidthMeters / 2.0) * Math.Sin(perpHeadingRad) / metersPerDegreeLng;

                return new List<PointLatLng>
                {
                    new PointLatLng(Position.Lat + halfWidthLat, Position.Lng + halfWidthLng),  // Left edge
                    new PointLatLng(Position.Lat - halfWidthLat, Position.Lng - halfWidthLng)   // Right edge
                };
            }
        }

        private readonly List<TrailPoint> trailPoints = new List<TrailPoint>();
        private const int MAX_TRAIL_POINTS = 1000;
        private const double MIN_DISTANCE_METERS = 0.1; // Reduced from 0.5 to allow more points

        public void AddPoint(PointLatLng position, double heading, double rate, double implementWidth)
        {
            Props.WriteErrorLog($"Adding point - Pos: ({position.Lat:F6}, {position.Lng:F6}), Heading: {heading:F1}, Rate: {rate:F2}, Width: {implementWidth:F1}");

            if (position.IsEmpty)
            {
                Props.WriteErrorLog("Skipping empty position");
                return;
            }

            // Skip if we're too close to the last point
            if (trailPoints.Count > 0)
            {
                var lastPoint = trailPoints[trailPoints.Count - 1];
                double dist = CalculateDistance(position, lastPoint.Position);
                if (dist < MIN_DISTANCE_METERS)
                {
                    Props.WriteErrorLog($"Skipping point - too close ({dist:F2}m)");
                    return;
                }
            }

            var point = new TrailPoint(position, heading, rate, implementWidth);
            trailPoints.Add(point);
            Props.WriteErrorLog($"Point added - Total points: {trailPoints.Count}");

            // Remove oldest points if we exceed the maximum
            while (trailPoints.Count > MAX_TRAIL_POINTS)
            {
                trailPoints.RemoveAt(0);
                Props.WriteErrorLog("Removed oldest point");
            }
        }

        public void DrawTrail(GMapOverlay overlay, double minRate, double maxRate)
        {
            Props.WriteErrorLog($"Drawing trail - Points: {trailPoints.Count}, Rate range: {minRate:F2} to {maxRate:F2}");
            
            overlay.Polygons.Clear();

            if (trailPoints.Count < 2)
            {
                Props.WriteErrorLog("Not enough points to draw trail");
                return;
            }

            // Ensure we have a valid range
            if (Math.Abs(maxRate - minRate) < 0.0001)
            {
                Props.WriteErrorLog("Rate range too small, adjusting");
                maxRate = minRate + 1;
            }

            int segmentsDrawn = 0;
            // Create trail segments between consecutive points
            for (int i = 1; i < trailPoints.Count; i++)
            {
                var prevPoint = trailPoints[i - 1];
                var currentPoint = trailPoints[i];

                try
                {
                    var prevEdges = prevPoint.GetImplementEdges();
                    var currentEdges = currentPoint.GetImplementEdges();

                    // Create polygon for this segment
                    var points = new List<PointLatLng>
                    {
                        prevEdges[0],      // Previous left edge
                        currentEdges[0],    // Current left edge
                        currentEdges[1],    // Current right edge
                        prevEdges[1],       // Previous right edge
                        prevEdges[0]        // Close the polygon
                    };

                    var polygon = new GMapPolygon(points, $"Trail_{i}");

                    // Calculate color based on rate
                    double rateRatio = (currentPoint.Rate - minRate) / (maxRate - minRate);
                    rateRatio = Math.Max(0, Math.Min(1, rateRatio));

                    Color fillColor = GetRateColor(rateRatio);
                    polygon.Fill = new SolidBrush(Color.FromArgb(150, fillColor));
                    polygon.Stroke = new Pen(Color.FromArgb(200, fillColor), 1);

                    overlay.Polygons.Add(polygon);
                    segmentsDrawn++;
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog($"Error drawing segment {i}: {ex.Message}");
                }
            }

            Props.WriteErrorLog($"Trail segments drawn: {segmentsDrawn}");
        }

        private double CalculateDistance(PointLatLng p1, PointLatLng p2)
        {
            const double R = 6371000; // Earth's radius in meters
            double lat1 = p1.Lat * Math.PI / 180;
            double lat2 = p2.Lat * Math.PI / 180;
            double dLat = (p2.Lat - p1.Lat) * Math.PI / 180;
            double dLon = (p2.Lng - p1.Lng) * Math.PI / 180;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                      Math.Cos(lat1) * Math.Cos(lat2) *
                      Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private Color GetRateColor(double ratio)
        {
            Color[] colors = new Color[]
            {
                Color.FromArgb(175, 240, 175), // Very light green
                Color.FromArgb(153, 225, 153), // Light minty green
                Color.FromArgb(102, 190, 102), // Vibrant lime green
                Color.FromArgb(51, 160, 51),   // Medium green
                Color.FromArgb(0, 130, 0)      // Dark green
            };

            int index = (int)(ratio * (colors.Length - 1));
            index = Math.Max(0, Math.Min(colors.Length - 1, index));
            return colors[index];
        }

        public Dictionary<string, Color> CreateLegend(double minRate, double maxRate)
        {
            var legend = new Dictionary<string, Color>();
            int steps = 5;

            // Ensure we have a valid range
            if (Math.Abs(maxRate - minRate) < 0.0001)
            {
                maxRate = minRate + 1;
            }

            for (int i = 0; i < steps; i++)
            {
                double ratio = i / (double)(steps - 1);
                double rate = minRate + (ratio * (maxRate - minRate));
                // Format legend labels with one decimal place and add units
                string label = $"{rate:F1} L/ha";
                Color color = GetRateColor(ratio);
                // Make legend colors fully opaque
                color = Color.FromArgb(255, color.R, color.G, color.B);
                legend[label] = color;
            }

            return legend;
        }

        public void Reset()
        {
            trailPoints.Clear();
            Props.WriteErrorLog("Trail reset");
        }
    }
}