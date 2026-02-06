using GMap.NET;
using GMap.NET.WindowsForms;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.Strtree;
using RateController.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.RateMap
{
    public class ContourLine
    {
        public double Level { get; set; }
        public List<PointLatLng> Points { get; set; }
    }

    public class ElevationOverlayCreator : IDisposable
    {
        private readonly GMapOverlay _labelOverlay;
        private readonly GMapOverlay ContourOverlay;
        private readonly GMapControl gmap;

        private bool _disposed;

        // Index over FieldSample
        private STRtree<FieldSample> _readingTree;

        private bool cEnabled = false;
        private int gridCols;
        private int gridRows;

        // Use FieldSample instead of RateReading
        private List<FieldSample> Readings;

        private bool UseSimulatedData = true;

        public ElevationOverlayCreator(GMapControl map)
        {
            gmap = map;
            ContourOverlay = new GMapOverlay("elevationContours");
            cEnabled = bool.TryParse(Props.GetProp("MapShowElevation"), out bool sh) ? sh : false;
            _labelOverlay = new GMapOverlay("elevationLabels");
        }

        public double ContourInterval { get; set; } = 0.5;

        public bool Enabled
        {
            get { return cEnabled; }
            set
            {
                cEnabled = value;
                Props.SetProp("MapShowElevation", cEnabled.ToString());

                if (cEnabled)
                {
                    Build();
                }
                else
                {
                    Reset();
                }
            }
        }

        public double GridResolutionMeters { get; set; } = 10.0;

        public void Build()
        {
            if (!cEnabled || _disposed)
            {
                return;
            }

            try
            {
                var yieldCreator = MapController.YieldCreator;
                if ((yieldCreator == null || yieldCreator.FieldData == null || yieldCreator.FieldData.Count == 0) && !UseSimulatedData)
                {
                    Reset();
                    return;
                }

                Readings = yieldCreator.FieldData
                    .Where(f => !double.IsNaN(f.ElevationMeters))
                    .ToList();

                if (UseSimulatedData)
                {
                    ApplySimulatedElevations();
                }

                if (Readings == null || Readings.Count < 3)
                {
                    Reset();
                    return;
                }

                _readingTree = new STRtree<FieldSample>();
                foreach (FieldSample r in Readings)
                {
                    Envelope env = new Envelope(r.Longitude, r.Longitude, r.Latitude, r.Latitude);
                    _readingTree.Insert(env, r);
                }

                var bounds = ComputeBounds();
                double[,] grid = BuildGrid(bounds);

                grid = SmoothGrid(grid, passes: 1);

                List<ContourLine> contours = GenerateContours(grid, bounds);

                DrawContours(contours);

                MapController.AddOverlay(ContourOverlay);
                MapController.AddOverlay(_labelOverlay);

                gmap.Refresh();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ElevationOverlayCreator/Build: " + ex.Message);
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                // Ensure overlays are removed from the map first
                MapController.RemoveOverlay(ContourOverlay);
                MapController.RemoveOverlay(_labelOverlay);

                ContourOverlay.Dispose();
                _labelOverlay.Dispose();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ElevationOverlayCreator/Dispose: " + ex.Message);
            }

            _disposed = true;
        }

        public void Reset()
        {
            if (_disposed)
            {
                return;
            }

            MapController.RemoveOverlay(ContourOverlay);
            MapController.RemoveOverlay(_labelOverlay);
            ContourOverlay.Clear();
            _labelOverlay.Clear();
            gmap.Refresh();
        }

        private void ApplySimulatedElevations()
        {
            if (Readings == null || Readings.Count == 0)
            {
                return;
            }

            double minLat = Readings.Min(r => r.Latitude);
            double maxLat = Readings.Max(r => r.Latitude);
            double minLon = Readings.Min(r => r.Longitude);
            double maxLon = Readings.Max(r => r.Longitude);

            double latRange = maxLat - minLat;
            double lonRange = maxLon - minLon;

            Random rng = new Random(7895); // deterministic
            List<FieldSample> simulatedReadings = new List<FieldSample>(Readings.Count);

            foreach (FieldSample r in Readings)
            {
                double nx = (r.Longitude - minLon) / lonRange;
                double ny = (r.Latitude - minLat) / latRange;

                double baseElev = 300 + 2 * nx + 2 * ny;
                double wave =
                    1.5 * Math.Sin(nx * Math.PI * 2) +
                    1.5 * Math.Cos(ny * Math.PI * 2);
                double jitter = 0.5 * Math.Sin((nx + ny) * Math.PI * 1.5);

                double simulated = baseElev + wave + jitter;

                FieldSample newSample = new FieldSample(
                    r.Timestamp,
                    r.Latitude,
                    r.Longitude,
                    r.YieldKg,
                    r.WidthMeters,
                    simulated);

                simulatedReadings.Add(newSample);
            }

            Readings = simulatedReadings;
        }

        private double[,] BuildGrid((double minLat, double maxLat, double minLon, double maxLon) b)
        {
            gridRows = (int)(MetersBetweenLat(b.minLat, b.maxLat) / GridResolutionMeters) + 1;
            gridCols = (int)(MetersBetweenLon(b.minLon, b.maxLon, (b.minLat + b.maxLat) / 2) / GridResolutionMeters) + 1;

            double[,] grid = new double[gridRows, gridCols];

            for (int r = 0; r < gridRows; r++)
            {
                for (int c = 0; c < gridCols; c++)
                {
                    double lat = b.minLat + (r / (double)(gridRows - 1)) * (b.maxLat - b.minLat);
                    double lon = b.minLon + (c / (double)(gridCols - 1)) * (b.maxLon - b.minLon);

                    grid[r, c] = InterpolateElevation(lat, lon);
                }
            }

            return grid;
        }

        private bool Close(PointLatLng a, PointLatLng b, double tol)
        {
            return Math.Abs(a.Lat - b.Lat) < tol &&
                   Math.Abs(a.Lng - b.Lng) < tol;
        }

        private (double minLat, double maxLat, double minLon, double maxLon) ComputeBounds()
        {
            return (
                Readings.Min(r => r.Latitude),
                Readings.Max(r => r.Latitude),
                Readings.Min(r => r.Longitude),
                Readings.Max(r => r.Longitude)
            );
        }

        private void DrawContours(List<ContourLine> contours)
        {
            ContourOverlay.Clear();
            _labelOverlay.Clear();

            List<PointLatLng> allPlacedLabels = new List<PointLatLng>();
            const double minLabelDistanceMeters = 50;

            foreach (var group in contours.GroupBy(c => c.Level))
            {
                foreach (ContourLine contour in group)
                {
                    if (contour.Points.Count < 2)
                    {
                        continue;
                    }

                    ContourOverlay.Routes.Add(
                        new GMapRoute(contour.Points, "contour")
                        {
                            Stroke = new Pen(Color.DarkRed, 2)
                        });
                }

                var significantContours = group
                    .Where(c => c.Points.Count > 10)
                    .OrderByDescending(c => PolylineLengthMeters(c.Points));

                int labelCountThisLevel = 0;
                foreach (ContourLine contour in significantContours)
                {
                    if (labelCountThisLevel >= 2)
                    {
                        break;
                    }

                    var candidates = new[] { 0.3, 0.5, 0.7 }
                        .Select(ratio => contour.Points[(int)(contour.Points.Count * ratio)])
                        .ToList();

                    foreach (PointLatLng labelPos in candidates)
                    {
                        bool tooClose = allPlacedLabels.Any(p =>
                            Haversine(p.Lat, p.Lng, labelPos.Lat, labelPos.Lng) < minLabelDistanceMeters);

                        if (!tooClose)
                        {
                            _labelOverlay.Markers.Add(
                                new TextMarker(labelPos, group.Key));
                            allPlacedLabels.Add(labelPos);
                            labelCountThisLevel++;
                            break;
                        }
                    }
                }
            }
        }

        private List<ContourLine> GenerateContours(
            double[,] grid,
            (double minLat, double maxLat, double minLon, double maxLon) b)
        {
            List<ContourLine> contours = new List<ContourLine>();

            double minElev = Readings.Min(r => r.ElevationMeters);
            double maxElev = Readings.Max(r => r.ElevationMeters);

            double start = Math.Floor(minElev / ContourInterval) * ContourInterval;
            double end = Math.Ceiling(maxElev / ContourInterval) * ContourInterval;

            for (double level = start; level <= end + 1e-9; level += ContourInterval)
            {
                List<List<PointLatLng>> lines = MarchingSquares(grid, b, level);

                if (lines.Count < 1)
                {
                    continue;
                }

                foreach (List<PointLatLng> line in lines)
                {
                    contours.Add(new ContourLine
                    {
                        Level = level,
                        Points = line
                    });
                }
            }

            return contours;
        }

        private List<PointLatLng> GetEdgeIntersections(
            int r,
            int c,
            double v0,
            double v1,
            double v2,
            double v3,
            double level,
            (double minLat, double maxLat, double minLon, double maxLon) b)
        {
            List<PointLatLng> pts = new List<PointLatLng>();

            PointLatLng p0 = GridPoint(r, c, b);
            PointLatLng p1 = GridPoint(r, c + 1, b);
            PointLatLng p2 = GridPoint(r + 1, c + 1, b);
            PointLatLng p3 = GridPoint(r + 1, c, b);

            TryEdge(p0, p1, v0, v1, level, pts);
            TryEdge(p1, p2, v1, v2, level, pts);
            TryEdge(p2, p3, v2, v3, level, pts);
            TryEdge(p3, p0, v3, v0, level, pts);

            return pts;
        }

        private PointLatLng GridPoint(
            int r,
            int c,
            (double minLat, double maxLat, double minLon, double maxLon) b)
        {
            double lat = b.minLat + r * (b.maxLat - b.minLat) / (gridRows - 1);
            double lon = b.minLon + c * (b.maxLon - b.minLon) / (gridCols - 1);
            return new PointLatLng(lat, lon);
        }

        private double Haversine(double lat1, double lon1, double lat2, double lon2)
        {
            double R = 6371000;
            double dLat = (lat2 - lat1) * Math.PI / 180;
            double dLon = (lon2 - lon1) * Math.PI / 180;

            double a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180) *
                Math.Cos(lat2 * Math.PI / 180) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }

        private double InterpolateElevation(double lat, double lon)
        {
            double delta = 0.0005;

            Envelope queryEnv = new Envelope(lon - delta, lon + delta, lat - delta, lat + delta);
            var candidates = _readingTree.Query(queryEnv);

            if (candidates.Count == 0)
            {
                return Readings[0].ElevationMeters;
            }

            var nearest = candidates
                .Select(r => new { r, Dist = Haversine(lat, lon, r.Latitude, r.Longitude) })
                .OrderBy(x => x.Dist)
                .Take(8)
                .ToList();

            if (nearest[0].Dist < 1)
            {
                return nearest[0].r.ElevationMeters;
            }

            double valueSum = 0;
            double weightSum = 0;
            foreach (var n in nearest)
            {
                double weight = 1.0 / (n.Dist * n.Dist);
                valueSum += weight * n.r.ElevationMeters;
                weightSum += weight;
            }

            return valueSum / weightSum;
        }

        private List<List<PointLatLng>> MarchingSquares(
            double[,] grid,
            (double minLat, double maxLat, double minLon, double maxLon) b,
            double level)
        {
            List<Segment> segments = new List<Segment>();

            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            for (int r = 0; r < rows - 1; r++)
            {
                for (int c = 0; c < cols - 1; c++)
                {
                    double v0 = grid[r, c];
                    double v1 = grid[r, c + 1];
                    double v2 = grid[r + 1, c + 1];
                    double v3 = grid[r + 1, c];

                    int idx = 0;
                    if (v0 > level) idx |= 1;
                    if (v1 > level) idx |= 2;
                    if (v2 > level) idx |= 4;
                    if (v3 > level) idx |= 8;

                    if (idx == 0 || idx == 15)
                    {
                        continue;
                    }

                    List<PointLatLng> edges = GetEdgeIntersections(r, c, v0, v1, v2, v3, level, b);

                    if (edges.Count == 2)
                    {
                        segments.Add(new Segment { A = edges[0], B = edges[1] });
                    }
                    else if (edges.Count == 4)
                    {
                        double center = (v0 + v1 + v2 + v3) / 4.0;

                        if (center > level)
                        {
                            segments.Add(new Segment { A = edges[0], B = edges[3] });
                            segments.Add(new Segment { A = edges[1], B = edges[2] });
                        }
                        else
                        {
                            segments.Add(new Segment { A = edges[0], B = edges[1] });
                            segments.Add(new Segment { A = edges[2], B = edges[3] });
                        }
                    }
                }
            }

            return StitchSegments(segments);
        }

        private double MetersBetweenLat(double lat1, double lat2)
        {
            return Haversine(lat1, 0, lat2, 0);
        }

        private double MetersBetweenLon(double lon1, double lon2, double lat)
        {
            return Haversine(lat, lon1, lat, lon2);
        }

        private double PolylineLengthMeters(List<PointLatLng> line)
        {
            double length = 0;

            for (int i = 1; i < line.Count; i++)
            {
                length += Haversine(
                    line[i - 1].Lat, line[i - 1].Lng,
                    line[i].Lat, line[i].Lng);
            }

            return length;
        }

        private double[,] SmoothGrid(double[,] grid, int passes = 2)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);
            double[,] result = (double[,])grid.Clone();

            for (int p = 0; p < passes; p++)
            {
                double[,] temp = (double[,])result.Clone();
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        double sum = 0;
                        int count = 0;
                        for (int dr = -1; dr <= 1; dr++)
                        {
                            for (int dc = -1; dc <= 1; dc++)
                            {
                                int rr = r + dr;
                                int cc = c + dc;
                                if (rr >= 0 && rr < rows && cc >= 0 && cc < cols)
                                {
                                    sum += result[rr, cc];
                                    count++;
                                }
                            }
                        }

                        temp[r, c] = sum / count;
                    }
                }

                result = temp;
            }

            return result;
        }

        private List<List<PointLatLng>> StitchSegments(List<Segment> segments)
        {
            List<List<PointLatLng>> lines = new List<List<PointLatLng>>();
            double tol = 1e-7;

            while (segments.Count > 0)
            {
                Segment seg = segments[0];
                segments.RemoveAt(0);

                List<PointLatLng> line = new List<PointLatLng> { seg.A, seg.B };
                bool extended;

                do
                {
                    extended = false;

                    for (int i = segments.Count - 1; i >= 0; i--)
                    {
                        Segment s = segments[i];

                        if (Close(line[line.Count - 1], s.A, tol))
                        {
                            line.Add(s.B);
                            segments.RemoveAt(i);
                            extended = true;
                        }
                        else if (Close(line[line.Count - 1], s.B, tol))
                        {
                            line.Add(s.A);
                            segments.RemoveAt(i);
                            extended = true;
                        }
                        else if (Close(line[0], s.B, tol))
                        {
                            line.Insert(0, s.A);
                            segments.RemoveAt(i);
                            extended = true;
                        }
                        else if (Close(line[0], s.A, tol))
                        {
                            line.Insert(0, s.B);
                            segments.RemoveAt(i);
                            extended = true;
                        }
                    }
                } while (extended);

                lines.Add(line);
            }

            return lines;
        }

        private void TryEdge(PointLatLng a, PointLatLng b, double va, double vb, double level, List<PointLatLng> pts)
        {
            if ((va - level) * (vb - level) >= 0)
            {
                return;
            }

            if (Math.Abs(vb - va) < 1e-9)
            {
                return;
            }

            double t = (level - va) / (vb - va);

            pts.Add(new PointLatLng(
                a.Lat + t * (b.Lat - a.Lat),
                a.Lng + t * (b.Lng - a.Lng)));
        }

        private struct Segment
        {
            public PointLatLng A;
            public PointLatLng B;
        }
    }

    public class TextMarker : GMapMarker
    {
        private const int PadX = 3;
        private const int PadY = 1;

        private static readonly Brush BgBrush = new SolidBrush(Color.FromArgb(255, 255, 255, 200));
        private static readonly Pen BorderPen = new Pen(Color.FromArgb(255, 160, 160, 120), 1);
        private static readonly Brush TextBrush = new SolidBrush(Color.Black);
        private static readonly Font TextFont = new Font("Segoe UI", 11, FontStyle.Bold);

        public TextMarker(PointLatLng position, double elevationMeters)
            : base(position)
        {
            ElevationMeters = elevationMeters;
        }

        public double ElevationMeters { get; }

        public override void OnRender(Graphics g)
        {
            string text = GetDisplayText();
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            var oldMode = g.CompositingMode;
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;

            SizeF textSize = g.MeasureString(text, TextFont);

            float w = textSize.Width + PadX * 2;
            float h = textSize.Height + PadY * 2;

            float x = LocalPosition.X - w / 2;
            float y = LocalPosition.Y - h / 2;

            RectangleF rect = new RectangleF(x, y, w, h);

            g.FillRectangle(BgBrush, rect);

            g.CompositingMode = oldMode;

            g.DrawRectangle(BorderPen, rect.X, rect.Y, rect.Width, rect.Height);
            g.DrawString(text, TextFont, TextBrush, rect.X + PadX, rect.Y + PadY);
        }

        private string GetDisplayText()
        {
            if (Props.UseMetric)
            {
                return string.Format("{0:F1} m", ElevationMeters);
            }

            double feet = ElevationMeters * 3.28084;
            return string.Format("{0:0} ft", Math.Round(feet));
        }
    }
}