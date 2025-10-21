using System;
using System.Collections.Generic;
using System.Drawing;
using GMap.NET;
using GMap.NET.WindowsForms;

namespace RateController.Classes
{
    /// <summary>
    /// Maintains a list of coverage swath rectangles between successive GPS points,
    /// computed from implement width and motion vector. Provides rendering and legend.
    /// </summary>
    public class CoverageTrail
    {
        private readonly object _lock = new object();

        // One segment = a swath polygon drawn between two points for the current implement width and rate.
        private sealed class Segment
        {
            public PointLatLng Prev { get; set; }
            public PointLatLng Curr { get; set; }
            public double Rate { get; set; }
            public double ImplementWidthMeters { get; set; }
        }

        private readonly List<Segment> _segments = new List<Segment>();
        private bool _hasPrev;
        private PointLatLng _prev;

        /// <summary>
        /// Add the next point of travel. Creates a segment with the implement swath coverage
        /// between previous and current points using the last known motion vector.
        /// </summary>
        /// <param name="pos">Current position.</param>
        /// <param name="headingDeg">Heading in degrees (0=N, 90=E). Only used for first segment when delta is tiny.</param>
        /// <param name="rate">Rate at this sample.</param>
        /// <param name="implementWidthMeters">Implement width (meters).</param>
        public void AddPoint(PointLatLng pos, double headingDeg, double rate, double implementWidthMeters)
        {
            lock (_lock)
            {
                if (!_hasPrev)
                {
                    _prev = pos;
                    _hasPrev = true;
                    return;
                }

                // Ignore invalid widths
                if (implementWidthMeters <= 0) implementWidthMeters = 0.01;

                // If no movement, synthesize a tiny forward movement based on heading, to avoid zero-area polygons.
                var prev = _prev;
                var curr = pos;

                const double metersPerDegLat = 111320.0;
                double latRad = prev.Lat * Math.PI / 180.0;
                double metersPerDegLng = metersPerDegLat * Math.Cos(latRad);

                double dxMeters = (curr.Lng - prev.Lng) * metersPerDegLng;
                double dyMeters = (curr.Lat - prev.Lat) * metersPerDegLat;
                double distMeters = Math.Sqrt(dxMeters * dxMeters + dyMeters * dyMeters);

                if (distMeters < 0.01)
                {
                    // Synthesize a very small forward move (2% of implement width) along heading.
                    double synth = Math.Max(implementWidthMeters * 0.02, 0.02);
                    double theta = headingDeg * Math.PI / 180.0;
                    double east = Math.Sin(theta);
                    double north = Math.Cos(theta);
                    double dLng = (synth * east) / metersPerDegLng;
                    double dLat = (synth * north) / metersPerDegLat;
                    curr = new PointLatLng(prev.Lat + dLat, prev.Lng + dLng);
                }

                _segments.Add(new Segment
                {
                    Prev = prev,
                    Curr = curr,
                    Rate = rate,
                    ImplementWidthMeters = implementWidthMeters
                });

                _prev = pos;
            }
        }

        /// <summary>
        /// Draws the swath polygons on the provided overlay. Colors are mapped by rate against [minRate, maxRate].
        /// </summary>
        public void DrawTrail(GMapOverlay overlay, double minRate, double maxRate)
        {
            if (overlay == null) return;

            lock (_lock)
            {
                overlay.Polygons.Clear();

                foreach (var seg in _segments)
                {
                    var polyPoints = BuildRectangle(seg.Prev, seg.Curr, seg.ImplementWidthMeters);
                    if (polyPoints == null || polyPoints.Count < 4) continue;

                    var color = ColorScale.Interpolate(minRate, maxRate, seg.Rate);
                    var poly = new GMapPolygon(polyPoints, "swath")
                    {
                        Stroke = new Pen(Color.FromArgb(160, color), 1),
                        Fill = new SolidBrush(Color.FromArgb(70, color))
                    };
                    overlay.Polygons.Add(poly);
                }
            }
        }

        /// <summary>
        /// Builds a simple N-step legend mapping textual ranges to representative colors.
        /// </summary>
        public Dictionary<string, Color> CreateLegend(double minRate, double maxRate, int steps = 6)
        {
            var legend = new Dictionary<string, Color>();
            if (steps < 1) steps = 1;

            // Guard against bad ranges
            if (double.IsNaN(minRate) || double.IsNaN(maxRate) || maxRate <= minRate)
            {
                legend.Add("No data", Color.Gray);
                return legend;
            }

            double step = (maxRate - minRate) / steps;
            for (int i = 0; i < steps; i++)
            {
                double a = minRate + (i * step);
                double b = (i == steps - 1) ? maxRate : minRate + ((i + 1) * step);
                var color = ColorScale.Interpolate(minRate, maxRate, (a + b) / 2.0);
                legend.Add($"{a:N1} - {b:N1}", color);
            }
            return legend;
        }

        /// <summary>
        /// Clears all segments and previous position.
        /// </summary>
        public void Reset()
        {
            lock (_lock)
            {
                _segments.Clear();
                _hasPrev = false;
                _prev = new PointLatLng(0, 0);
            }
        }

        // Compute a rectangle swath between prev and curr, widened by implement width.
        private static List<PointLatLng> BuildRectangle(PointLatLng prev, PointLatLng curr, double widthMeters)
        {
            const double metersPerDegLat = 111320.0;
            double latRad = prev.Lat * Math.PI / 180.0;
            double metersPerDegLng = metersPerDegLat * Math.Cos(latRad);

            // Vector from prev to curr in meters (east, north)
            double dxMeters = (curr.Lng - prev.Lng) * metersPerDegLng;
            double dyMeters = (curr.Lat - prev.Lat) * metersPerDegLat;

            double len = Math.Sqrt(dxMeters * dxMeters + dyMeters * dyMeters);
            if (len < 1e-6) len = 1e-6; // avoid division by zero

            // Unit vectors
            double ux = dxMeters / len; // east component along-track
            double uy = dyMeters / len; // north component along-track

            // Perpendicular (to the left) unit vector in meters
            double px = -uy; // east component
            double py = ux;  // north component

            double halfWidth = Math.Max(widthMeters / 2.0, 0.005);

            // Four corners: prevLeft, prevRight, currRight, currLeft
            var prevLeft = Offset(prev, px * halfWidth, py * halfWidth, metersPerDegLng, metersPerDegLat);
            var prevRight = Offset(prev, -px * halfWidth, -py * halfWidth, metersPerDegLng, metersPerDegLat);
            var currLeft = Offset(curr, px * halfWidth, py * halfWidth, metersPerDegLng, metersPerDegLat);
            var currRight = Offset(curr, -px * halfWidth, -py * halfWidth, metersPerDegLng, metersPerDegLat);

            return new List<PointLatLng>
            {
                prevLeft,
                prevRight,
                currRight,
                currLeft,
                prevLeft // close polygon
            };
        }

        private static PointLatLng Offset(PointLatLng src, double eastMeters, double northMeters, double metersPerDegLng, double metersPerDegLat)
        {
            double dLng = eastMeters / metersPerDegLng;
            double dLat = northMeters / metersPerDegLat;
            return new PointLatLng(src.Lat + dLat, src.Lng + dLng);
        }
    }
}