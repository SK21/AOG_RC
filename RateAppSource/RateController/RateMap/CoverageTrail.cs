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
    /// Adds "turn wedges" to fill gaps that appear on sharp heading changes.
    /// </summary>
    public class CoverageTrail
    {
        private readonly object _lock = new object();

        private sealed class Segment
        {
            public PointLatLng Prev;
            public PointLatLng Curr;
            public double Rate;
            public double ImplementWidthMeters;

            // Rectangle corners (computed once to support "turn wedges")
            public PointLatLng PrevLeft;
            public PointLatLng PrevRight;
            public PointLatLng CurrLeft;
            public PointLatLng CurrRight;
        }

        private readonly List<Segment> _segments = new List<Segment>();
        private bool _hasPrev;
        private PointLatLng _prev;

        /// <summary>
        /// Add the next point of travel. Creates a segment with the implement swath coverage
        /// between previous and current points using the last known motion vector.
        /// </summary>
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
                    double synth = Math.Max(implementWidthMeters * 0.02, 0.02);
                    double theta = headingDeg * Math.PI / 180.0;
                    double east = Math.Sin(theta);
                    double north = Math.Cos(theta);
                    double dLng = (synth * east) / metersPerDegLng;
                    double dLat = (synth * north) / metersPerDegLat;
                    curr = new PointLatLng(prev.Lat + dLat, prev.Lng + dLng);
                }

                // Compute and store rectangle corners for the segment
                PointLatLng prevLeft, prevRight, currLeft, currRight;
                ComputeCorners(prev, curr, implementWidthMeters, out prevLeft, out prevRight, out currLeft, out currRight);

                _segments.Add(new Segment
                {
                    Prev = prev,
                    Curr = curr,
                    Rate = rate,
                    ImplementWidthMeters = implementWidthMeters,
                    PrevLeft = prevLeft,
                    PrevRight = prevRight,
                    CurrLeft = currLeft,
                    CurrRight = currRight
                });

                _prev = pos;
            }
        }

        /// <summary>
        /// Draws the swath polygons on the provided overlay. Colors are mapped by rate against [minRate, maxRate].
        /// Also fills the inner "wedge" gaps that happen on sharp turns between consecutive segments.
        /// </summary>
        public void DrawTrail(GMapOverlay overlay, double minRate, double maxRate)
        {
            if (overlay == null) return;

            lock (_lock)
            {
                overlay.Polygons.Clear();

                // Draw segment rectangles
                for (int i = 0; i < _segments.Count; i++)
                {
                    var seg = _segments[i];
                    var polyPoints = new List<PointLatLng>
                    {
                        seg.PrevLeft,
                        seg.PrevRight,
                        seg.CurrRight,
                        seg.CurrLeft,
                        seg.PrevLeft // close polygon
                    };

                    var color = ColorScale.Interpolate(minRate, maxRate, seg.Rate);
                    var poly = new GMapPolygon(polyPoints, "swath")
                    {
                        Stroke = new Pen(Color.FromArgb(160, color), 1),
                        Fill = new SolidBrush(Color.FromArgb(70, color))
                    };
                    overlay.Polygons.Add(poly);
                }

                // Fill "turn wedges" between consecutive segments
                for (int i = 1; i < _segments.Count; i++)
                {
                    var prev = _segments[i - 1];
                    var curr = _segments[i];
                    var center = curr.Prev; // shared center point between segments

                    // If the left edges at the shared point diverge enough, fill the inner wedge
                    if (ShouldFillWedge(prev.CurrLeft, curr.PrevLeft))
                    {
                        var leftWedge = new List<PointLatLng>
                        {
                            prev.CurrLeft,
                            curr.PrevLeft,
                            center,
                            prev.CurrLeft
                        };
                        var c = ColorScale.Interpolate(minRate, maxRate, curr.Rate);
                        var poly = new GMapPolygon(leftWedge, "turn_left")
                        {
                            Stroke = new Pen(Color.FromArgb(0, c), 0), // no visible stroke
                            Fill = new SolidBrush(Color.FromArgb(70, c))
                        };
                        overlay.Polygons.Add(poly);
                    }

                    // Same for right edge wedge
                    if (ShouldFillWedge(prev.CurrRight, curr.PrevRight))
                    {
                        var rightWedge = new List<PointLatLng>
                        {
                            prev.CurrRight,
                            curr.PrevRight,
                            center,
                            prev.CurrRight
                        };
                        var c = ColorScale.Interpolate(minRate, maxRate, curr.Rate);
                        var poly = new GMapPolygon(rightWedge, "turn_right")
                        {
                            Stroke = new Pen(Color.FromArgb(0, c), 0),
                            Fill = new SolidBrush(Color.FromArgb(70, c))
                        };
                        overlay.Polygons.Add(poly);
                    }
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
                legend.Add(string.Format("{0:N1} - {1:N1}", a, b), color);
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

        private static bool ShouldFillWedge(PointLatLng a, PointLatLng b)
        {
            // Use a small geographic threshold to avoid creating degenerate tiny triangles.
            // ~0.05 meters in lat/long degrees near mid-latitudes.
            const double metersPerDegLat = 111320.0;
            // Convert difference to meters assuming similar latitude for both points
            double dLat = (a.Lat - b.Lat) * metersPerDegLat;
            // Approximate meters/deg lng using latitude of point a
            double metersPerDegLng = metersPerDegLat * Math.Cos(a.Lat * Math.PI / 180.0);
            double dLng = (a.Lng - b.Lng) * metersPerDegLng;

            double distMeters = Math.Sqrt(dLat * dLat + dLng * dLng);
            return distMeters > 0.05; // threshold
        }

        private static void ComputeCorners(
            PointLatLng prev,
            PointLatLng curr,
            double widthMeters,
            out PointLatLng prevLeft,
            out PointLatLng prevRight,
            out PointLatLng currLeft,
            out PointLatLng currRight)
        {
            const double metersPerDegLat = 111320.0;
            double latRad = prev.Lat * Math.PI / 180.0;
            double metersPerDegLng = metersPerDegLat * Math.Cos(latRad);

            double dxMeters = (curr.Lng - prev.Lng) * metersPerDegLng;
            double dyMeters = (curr.Lat - prev.Lat) * metersPerDegLat;

            double len = Math.Sqrt(dxMeters * dxMeters + dyMeters * dyMeters);
            if (len < 1e-6) len = 1e-6;

            double ux = dxMeters / len; // east along-track
            double uy = dyMeters / len; // north along-track

            double px = -uy; // east perpendicular (left)
            double py = ux;  // north perpendicular (left)

            double halfWidth = Math.Max(widthMeters / 2.0, 0.005);

            prevLeft = Offset(prev, px * halfWidth, py * halfWidth, metersPerDegLng, metersPerDegLat);
            prevRight = Offset(prev, -px * halfWidth, -py * halfWidth, metersPerDegLng, metersPerDegLat);
            currLeft = Offset(curr, px * halfWidth, py * halfWidth, metersPerDegLng, metersPerDegLat);
            currRight = Offset(curr, -px * halfWidth, -py * halfWidth, metersPerDegLng, metersPerDegLat);
        }

        private static PointLatLng Offset(PointLatLng src, double eastMeters, double northMeters, double metersPerDegLng, double metersPerDegLat)
        {
            double dLng = eastMeters / metersPerDegLng;
            double dLat = northMeters / metersPerDegLat;
            return new PointLatLng(src.Lat + dLat, src.Lng + dLng);
        }
    }
}