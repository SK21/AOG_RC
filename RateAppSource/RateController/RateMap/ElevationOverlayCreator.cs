using GMap.NET;
using GMap.NET.WindowsForms;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.Strtree;
using RateController.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

namespace RateController.RateMap
{
    public class ContourLine
    {
        public double Level { get; set; }
        public List<PointLatLng> Points { get; set; }
    }

    public class ElevationOverlayCreator : IDisposable
    {
        private readonly GMapControl _map;
        private readonly GMapOverlay _fillOverlay;
        private readonly GMapOverlay _contourOverlay;
        private readonly GMapOverlay _labelOverlay;

        private bool _disposed;
        private bool _enabled;
        private string _elevationPath;   // last path passed to LoadElevationFile; re-read on every Build()
        private STRtree<FieldSample> _tree;
        private List<FieldSample> _readings;
        private Bitmap _currentBitmap;
        private int _gridRows, _gridCols;

        public ElevationOverlayCreator(GMapControl map)
        {
            _map = map;
            _fillOverlay = new GMapOverlay("elevationFill");
            _contourOverlay = new GMapOverlay("elevationContours");
            _labelOverlay = new GMapOverlay("elevationLabels");
            _enabled = bool.TryParse(Props.GetProp("MapShowElevation"), out bool sh) ? sh : false;
        }

        // Default 1m — appropriate for agricultural field data
        public double ContourInterval { get; set; } = 1.0;
        public double GridResolutionMeters { get; set; } = 10.0;

        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                Props.SetProp("MapShowElevation", _enabled.ToString());
                if (_enabled) Build();
                else Reset();
            }
        }

        // Returns a short quality summary for display in the Files panel label.
        // Empty string if no data loaded.
        public string GetQualitySummary()
        {
            if (_readings == null || _readings.Count == 0) return string.Empty;

            List<FieldSample> clean = FilterOutliers(_readings);
            int dropped = _readings.Count - clean.Count;
            if (clean.Count == 0) clean = _readings;

            double minElev = clean.Min(r => r.ElevationMeters);
            double maxElev = clean.Max(r => r.ElevationMeters);
            double range = maxElev - minElev;

            string rangeStr = Props.UseMetric
                ? string.Format("{0:F1}m", range)
                : string.Format("{0:F0}ft", range * 3.28084);

            string flag = (range < 0.5 || clean.Count < 15) ? " !" : "";
            string outlierNote = dropped > 0 ? string.Format(" -{0}outliers", dropped) : "";
            return string.Format("{0} pts  {1} range{2}{3}", clean.Count, rangeStr, flag, outlierNote);
        }

        // Generates a synthetic elevation CSV covering the given bounds and saves it to outputPath.
        // Produces a gentle slope + rolling terrain with ~3-5m total variation — enough for clear contours.
        public static void GenerateTestData(
            string outputPath,
            double minLat, double maxLat,
            double minLon, double maxLon,
            double baseElevation = 100.0)
        {
            const double spacingMeters = 25.0;
            double latStep = spacingMeters / 111000.0;
            double midLat = (minLat + maxLat) / 2.0;
            double lonStep = spacingMeters / (111000.0 * Math.Cos(midLat * Math.PI / 180.0));

            double latRange = maxLat - minLat;
            double lonRange = maxLon - minLon;

            var rng = new Random(4219);  // deterministic seed
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Lat,Lon,Elevation");

            for (double lat = minLat; lat <= maxLat + latStep * 0.5; lat += latStep)
            {
                for (double lon = minLon; lon <= maxLon + lonStep * 0.5; lon += lonStep)
                {
                    double nx = latRange > 0 ? (lat - minLat) / latRange : 0.5;
                    double ny = lonRange > 0 ? (lon - minLon) / lonRange : 0.5;

                    // 20ft (6.1m) drop west→east — ny=0 is west (high), ny=1 is east (low)
                    double slope = 6.1 * (1.0 - ny);

                    // Gentle N-S rolling (~1m amplitude) — curves the contour lines naturally
                    double roll = 0.5 * Math.Sin(nx * Math.PI * 4.0)
                                + 0.3 * Math.Cos(ny * Math.PI * 3.0);

                    // Subtle cross-feature (±0.2m noise-like variation)
                    double cross = 0.2 * Math.Sin((nx + ny) * Math.PI * 3.0);

                    // Small noise (±0.1m)
                    double noise = 0.1 * (rng.NextDouble() * 2.0 - 1.0);

                    double elev = baseElevation + slope + roll + cross + noise;
                    sb.AppendLine(string.Format(
                        System.Globalization.CultureInfo.InvariantCulture,
                        "{0:F6},{1:F6},{2:F3}", lat, lon, elev));
                }
            }

            System.IO.File.WriteAllText(outputPath, sb.ToString());
        }

        public void LoadElevationFile(string filePath)
        {
            _elevationPath = filePath;   // store so Build() can re-read when toggled or refreshed
            _readings = new List<FieldSample>();
            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath)) return;
            try
            {
                string[] lines = System.IO.File.ReadAllLines(filePath);
                for (int i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i])) continue;
                    string[] parts = lines[i].Split(',');
                    if (parts.Length < 3) continue;
                    if (!double.TryParse(parts[0], System.Globalization.NumberStyles.Float,
                            System.Globalization.CultureInfo.InvariantCulture, out double lat)) continue;
                    if (!double.TryParse(parts[1], System.Globalization.NumberStyles.Float,
                            System.Globalization.CultureInfo.InvariantCulture, out double lon)) continue;
                    if (!double.TryParse(parts[2], System.Globalization.NumberStyles.Float,
                            System.Globalization.CultureInfo.InvariantCulture, out double el)) continue;
                    _readings.Add(new FieldSample(DateTime.MinValue, lat, lon, 0, 0, el));
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ElevationOverlayCreator/LoadElevationFile: " + ex.Message);
                _readings = new List<FieldSample>();
            }
        }

        public void Build()
        {
            if (!_enabled || _disposed) return;

            ClearOverlays();

            // If the file was deleted since last load, clear readings so the overlay disappears.
            if (!string.IsNullOrEmpty(_elevationPath) && !System.IO.File.Exists(_elevationPath))
                _readings = null;

            try
            {
                if (_readings == null || _readings.Count < 3)
                {
                    _map.Refresh();
                    return;
                }

                // Filter outliers: discard readings more than 3 SD from the mean.
                // GPS altitude occasionally produces bogus near-zero fixes that collapse the colour range.
                List<FieldSample> clean = FilterOutliers(_readings);
                if (clean.Count < 3) clean = _readings;  // fall back if filtering removes too many

                _tree = new STRtree<FieldSample>();
                foreach (var r in clean)
                    _tree.Insert(new Envelope(r.Longitude, r.Longitude, r.Latitude, r.Latitude), r);

                var bounds = ComputeBounds(clean);
                double[,] grid = BuildGrid(bounds);
                grid = GaussianSmooth(grid, passes: 2);

                double minElev = clean.Min(r => r.ElevationMeters);
                double maxElev = clean.Max(r => r.ElevationMeters);

                BuildFillOverlay(grid, bounds, minElev, maxElev);
                BuildContourOverlay(grid, bounds, minElev, maxElev);

                MapController.AddOverlay(_fillOverlay);
                MapController.AddOverlay(_contourOverlay);
                MapController.AddOverlay(_labelOverlay);

                _map.Refresh();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ElevationOverlayCreator/Build: " + ex.Message);
            }
        }

        public void Reset()
        {
            if (_disposed) return;
            ClearOverlays();
            _map.Refresh();
        }

        public void Dispose()
        {
            if (_disposed) return;
            try
            {
                ClearOverlays();
                _fillOverlay.Dispose();
                _contourOverlay.Dispose();
                _labelOverlay.Dispose();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ElevationOverlayCreator/Dispose: " + ex.Message);
            }
            _disposed = true;
        }

        // ── Private: overlay management ─────────────────────────────────────────

        private void ClearOverlays()
        {
            MapController.RemoveOverlay(_fillOverlay);
            MapController.RemoveOverlay(_contourOverlay);
            MapController.RemoveOverlay(_labelOverlay);

            _fillOverlay.Markers.Clear();
            _contourOverlay.Routes.Clear();
            _labelOverlay.Markers.Clear();

            _currentBitmap?.Dispose();
            _currentBitmap = null;
        }

        // ── Private: grid construction ───────────────────────────────────────────

        // Remove readings whose elevation is more than 3 SD from the mean.
        private static List<FieldSample> FilterOutliers(List<FieldSample> src)
        {
            double mean = src.Average(r => r.ElevationMeters);
            double sd   = Math.Sqrt(src.Average(r => Math.Pow(r.ElevationMeters - mean, 2)));
            double lo   = mean - 3 * sd;
            double hi   = mean + 3 * sd;
            return src.Where(r => r.ElevationMeters >= lo && r.ElevationMeters <= hi).ToList();
        }

        private static (double minLat, double maxLat, double minLon, double maxLon) ComputeBounds(
            List<FieldSample> src) =>
        (
            src.Min(r => r.Latitude),
            src.Max(r => r.Latitude),
            src.Min(r => r.Longitude),
            src.Max(r => r.Longitude)
        );

        private double[,] BuildGrid((double minLat, double maxLat, double minLon, double maxLon) b)
        {
            double midLat = (b.minLat + b.maxLat) / 2;
            _gridRows = Math.Max(2, (int)(Haversine(b.minLat, 0, b.maxLat, 0) / GridResolutionMeters) + 1);
            _gridCols = Math.Max(2, (int)(Haversine(midLat, b.minLon, midLat, b.maxLon) / GridResolutionMeters) + 1);

            double[,] grid = new double[_gridRows, _gridCols];
            for (int r = 0; r < _gridRows; r++)
            {
                for (int c = 0; c < _gridCols; c++)
                {
                    double lat = b.minLat + (r / (double)(_gridRows - 1)) * (b.maxLat - b.minLat);
                    double lon = b.minLon + (c / (double)(_gridCols - 1)) * (b.maxLon - b.minLon);
                    grid[r, c] = IDW(lat, lon);
                }
            }
            return grid;
        }

        // Inverse distance weighting — expands search radius if no nearby points found
        private double IDW(double lat, double lon)
        {
            foreach (double delta in new[] { 0.0005, 0.005 })
            {
                var env = new Envelope(lon - delta, lon + delta, lat - delta, lat + delta);
                var candidates = _tree.Query(env);
                if (candidates.Count == 0) continue;

                var nearest = candidates
                    .Select(r => new { r, d = Haversine(lat, lon, r.Latitude, r.Longitude) })
                    .OrderBy(x => x.d)
                    .Take(8)
                    .ToList();

                if (nearest[0].d < 1.0) return nearest[0].r.ElevationMeters;

                double vSum = 0, wSum = 0;
                foreach (var n in nearest)
                {
                    double w = 1.0 / (n.d * n.d);
                    vSum += w * n.r.ElevationMeters;
                    wSum += w;
                }
                return vSum / wSum;
            }
            // Last resort: field average
            return _readings.Average(r => r.ElevationMeters);
        }

        private static double[,] GaussianSmooth(double[,] grid, int passes)
        {
            // 3×3 Gaussian kernel
            double[,] k =
            {
                { 0.0625, 0.125, 0.0625 },
                { 0.125,  0.25,  0.125  },
                { 0.0625, 0.125, 0.0625 }
            };
            int rows = grid.GetLength(0), cols = grid.GetLength(1);
            double[,] result = (double[,])grid.Clone();

            for (int p = 0; p < passes; p++)
            {
                double[,] temp = (double[,])result.Clone();
                for (int r = 1; r < rows - 1; r++)
                {
                    for (int c = 1; c < cols - 1; c++)
                    {
                        double sum = 0;
                        for (int dr = -1; dr <= 1; dr++)
                            for (int dc = -1; dc <= 1; dc++)
                                sum += k[dr + 1, dc + 1] * result[r + dr, c + dc];
                        temp[r, c] = sum;
                    }
                }
                result = temp;
            }
            return result;
        }

        // ── Private: color fill ──────────────────────────────────────────────────

        private void BuildFillOverlay(
            double[,] grid,
            (double minLat, double maxLat, double minLon, double maxLon) b,
            double minElev,
            double maxElev)
        {
            int rows = _gridRows, cols = _gridCols;
            double[,] hillshade = ComputeHillshade(grid, b);

            // Build pixel array — faster than SetPixel
            int[] pixels = new int[rows * cols];
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    double t = maxElev > minElev
                        ? (grid[r, c] - minElev) / (maxElev - minElev)
                        : 0.5;
                    t = Math.Max(0, Math.Min(1, t));

                    // Flip row: grid row 0 = minLat (bottom) → bitmap bottom row
                    pixels[(rows - 1 - r) * cols + c] =
                        ElevationColor(t, hillshade[r, c], alpha: 150).ToArgb();
                }
            }

            _currentBitmap = new Bitmap(cols, rows, PixelFormat.Format32bppArgb);
            var bmpData = _currentBitmap.LockBits(
                new Rectangle(0, 0, cols, rows),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);
            Marshal.Copy(pixels, 0, bmpData.Scan0, pixels.Length);
            _currentBitmap.UnlockBits(bmpData);

            _fillOverlay.Markers.Add(new ElevationBitmapMarker(
                _map,
                new PointLatLng(b.maxLat, b.minLon),   // top-left anchor
                new PointLatLng(b.minLat, b.maxLon),   // bottom-right extent
                _currentBitmap));
        }

        // Compute hillshade from elevation grid using NW light at 45° altitude (cartographic standard).
        // Returns values 0 (fully shadowed) to 1 (fully lit).
        private double[,] ComputeHillshade(
            double[,] grid,
            (double minLat, double maxLat, double minLon, double maxLon) b)
        {
            int rows = _gridRows, cols = _gridCols;
            double[,] hs = new double[rows, cols];

            double cellY = Haversine(b.minLat, 0, b.maxLat, 0) / (rows - 1);
            double midLat = (b.minLat + b.maxLat) / 2;
            double cellX = Haversine(midLat, b.minLon, midLat, b.maxLon) / (cols - 1);

            // NW light at 45° altitude — standard cartographic convention
            const double altitudeDeg = 45.0;
            const double azimuthDeg = 315.0;
            double zenithRad = (90.0 - altitudeDeg) * Math.PI / 180.0;
            double azimuthMathRad = (360.0 - azimuthDeg + 90.0) * Math.PI / 180.0;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    int r0 = Math.Max(0, r - 1), r2 = Math.Min(rows - 1, r + 1);
                    int c0 = Math.Max(0, c - 1), c2 = Math.Min(cols - 1, c + 1);

                    double dzdx = (grid[r, c2] - grid[r, c0]) / ((c2 - c0) * cellX);
                    double dzdy = (grid[r2, c] - grid[r0, c]) / ((r2 - r0) * cellY);

                    double slopeRad = Math.Atan(Math.Sqrt(dzdx * dzdx + dzdy * dzdy));

                    double aspectRad;
                    if (Math.Abs(dzdx) < 1e-10 && Math.Abs(dzdy) < 1e-10)
                        aspectRad = 0;
                    else
                    {
                        aspectRad = Math.Atan2(dzdy, -dzdx);
                        if (aspectRad < 0) aspectRad += 2 * Math.PI;
                    }

                    double h = Math.Cos(zenithRad) * Math.Cos(slopeRad)
                             + Math.Sin(zenithRad) * Math.Sin(slopeRad)
                             * Math.Cos(azimuthMathRad - aspectRad);

                    hs[r, c] = Math.Max(0, h);
                }
            }
            return hs;
        }

        // Elevation colour ramp: blue (low) → purple → magenta → red → orange (high).
        // 160° counterclockwise sweep avoids 120° (green) which blends with crop satellite imagery.
        // Alpha 150 (~59% opacity) keeps satellite visible while the colour shift is clearly readable.
        // Hillshade modulates brightness (0.1–1.0) for 3-D relief.
        private static Color ElevationColor(double t, double hillshade, int alpha)
        {
            // 240° (blue) → 400°%360=40° (orange): counterclockwise, 160° sweep
            double hue = (240.0 + t * 160.0) % 360.0;
            Color baseColor = HsvToRgb(hue, saturation: 1.0, value: 1.0);

            // Wider hillshade contrast: 0.1 floor (shadow stays very dark) → 1.0 peak
            double bright = 0.1 + 0.9 * hillshade;
            return Color.FromArgb(
                alpha,
                (int)Math.Min(255, baseColor.R * bright),
                (int)Math.Min(255, baseColor.G * bright),
                (int)Math.Min(255, baseColor.B * bright));
        }

        // Standard HSV → RGB conversion (h = 0–360°, s and v = 0–1).
        private static Color HsvToRgb(double h, double saturation, double value)
        {
            if (saturation <= 0)
            {
                int v = (int)(value * 255);
                return Color.FromArgb(v, v, v);
            }

            double hh = (h % 360) / 60.0;
            int    i  = (int)hh;
            double ff = hh - i;
            double p  = value * (1.0 - saturation);
            double q  = value * (1.0 - saturation * ff);
            double t2 = value * (1.0 - saturation * (1.0 - ff));

            double r, g, b;
            switch (i)
            {
                case 0:  r = value; g = t2;    b = p;     break;
                case 1:  r = q;     g = value; b = p;     break;
                case 2:  r = p;     g = value; b = t2;    break;
                case 3:  r = p;     g = q;     b = value; break;
                case 4:  r = t2;    g = p;     b = value; break;
                default: r = value; g = p;     b = q;     break;
            }
            return Color.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));
        }

        // ── Private: contour lines ───────────────────────────────────────────────

        private void BuildContourOverlay(
            double[,] grid,
            (double minLat, double maxLat, double minLon, double maxLon) b,
            double minElev,
            double maxElev)
        {
            var contours = GenerateContours(grid, b, minElev, maxElev);
            DrawContours(contours);
        }

        private List<ContourLine> GenerateContours(
            double[,] grid,
            (double minLat, double maxLat, double minLon, double maxLon) b,
            double minElev,
            double maxElev)
        {
            var result = new List<ContourLine>();
            double start = Math.Ceiling(minElev / ContourInterval) * ContourInterval;
            double end = Math.Floor(maxElev / ContourInterval) * ContourInterval;

            for (double level = start; level <= end + 1e-9; level += ContourInterval)
            {
                foreach (var line in MarchingSquares(grid, b, level))
                    result.Add(new ContourLine { Level = level, Points = line });
            }
            return result;
        }

        private void DrawContours(List<ContourLine> contours)
        {
            _contourOverlay.Routes.Clear();
            _labelOverlay.Markers.Clear();

            // Major contour every 2 minor intervals
            double majorEvery = ContourInterval * 2;
            var placedLabels = new List<PointLatLng>();
            const double minLabelSpacingMeters = 40;

            foreach (var group in contours.GroupBy(c => Math.Round(c.Level / ContourInterval) * ContourInterval))
            {
                bool isMajor = Math.Abs(group.Key % majorEvery) < ContourInterval * 0.01;

                var pen = isMajor
                    ? new Pen(Color.FromArgb(210, 60, 30, 0), 2.0f)
                    : new Pen(Color.FromArgb(150, 60, 30, 0), 1.5f);

                foreach (var contour in group)
                {
                    if (contour.Points.Count < 2) continue;
                    _contourOverlay.Routes.Add(new GMapRoute(contour.Points, "contour") { Stroke = pen });
                }

                if (!isMajor) continue;

                // Label every major contour ring that is long enough and not too close to an existing label
                foreach (var contour in group
                    .Where(c => c.Points.Count > 5)
                    .OrderByDescending(c => PolylineLength(c.Points)))
                {
                    var pos = contour.Points[contour.Points.Count / 2];
                    bool tooClose = placedLabels.Any(p =>
                        Haversine(p.Lat, p.Lng, pos.Lat, pos.Lng) < minLabelSpacingMeters);

                    if (!tooClose)
                    {
                        _labelOverlay.Markers.Add(new ElevationLabel(pos, group.Key));
                        placedLabels.Add(pos);
                    }
                }
            }
        }

        // ── Private: marching squares ────────────────────────────────────────────

        private List<List<PointLatLng>> MarchingSquares(
            double[,] grid,
            (double minLat, double maxLat, double minLon, double maxLon) b,
            double level)
        {
            int rows = grid.GetLength(0), cols = grid.GetLength(1);
            var segments = new List<Segment>();

            for (int r = 0; r < rows - 1; r++)
            {
                for (int c = 0; c < cols - 1; c++)
                {
                    double v0 = grid[r, c], v1 = grid[r, c + 1];
                    double v2 = grid[r + 1, c + 1], v3 = grid[r + 1, c];

                    int idx = 0;
                    if (v0 > level) idx |= 1;
                    if (v1 > level) idx |= 2;
                    if (v2 > level) idx |= 4;
                    if (v3 > level) idx |= 8;
                    if (idx == 0 || idx == 15) continue;

                    var edges = GetEdgeIntersections(r, c, v0, v1, v2, v3, level, b);
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

        private List<PointLatLng> GetEdgeIntersections(
            int r, int c,
            double v0, double v1, double v2, double v3,
            double level,
            (double minLat, double maxLat, double minLon, double maxLon) b)
        {
            var pts = new List<PointLatLng>();
            var p0 = GridPoint(r, c, b);
            var p1 = GridPoint(r, c + 1, b);
            var p2 = GridPoint(r + 1, c + 1, b);
            var p3 = GridPoint(r + 1, c, b);
            TryEdge(p0, p1, v0, v1, level, pts);
            TryEdge(p1, p2, v1, v2, level, pts);
            TryEdge(p2, p3, v2, v3, level, pts);
            TryEdge(p3, p0, v3, v0, level, pts);
            return pts;
        }

        private PointLatLng GridPoint(
            int r, int c,
            (double minLat, double maxLat, double minLon, double maxLon) b)
        {
            double lat = b.minLat + r * (b.maxLat - b.minLat) / (_gridRows - 1);
            double lon = b.minLon + c * (b.maxLon - b.minLon) / (_gridCols - 1);
            return new PointLatLng(lat, lon);
        }

        private static void TryEdge(
            PointLatLng a, PointLatLng b,
            double va, double vb,
            double level,
            List<PointLatLng> pts)
        {
            if ((va - level) * (vb - level) >= 0 || Math.Abs(vb - va) < 1e-9) return;
            double t = (level - va) / (vb - va);
            pts.Add(new PointLatLng(a.Lat + t * (b.Lat - a.Lat), a.Lng + t * (b.Lng - a.Lng)));
        }

        private static List<List<PointLatLng>> StitchSegments(List<Segment> segments)
        {
            var lines = new List<List<PointLatLng>>();
            const double tol = 1e-7;

            while (segments.Count > 0)
            {
                var seg = segments[0];
                segments.RemoveAt(0);
                var line = new List<PointLatLng> { seg.A, seg.B };
                bool extended;
                do
                {
                    extended = false;
                    for (int i = segments.Count - 1; i >= 0; i--)
                    {
                        var s = segments[i];
                        if (Close(line[line.Count - 1], s.A, tol)) { line.Add(s.B); segments.RemoveAt(i); extended = true; }
                        else if (Close(line[line.Count - 1], s.B, tol)) { line.Add(s.A); segments.RemoveAt(i); extended = true; }
                        else if (Close(line[0], s.B, tol)) { line.Insert(0, s.A); segments.RemoveAt(i); extended = true; }
                        else if (Close(line[0], s.A, tol)) { line.Insert(0, s.B); segments.RemoveAt(i); extended = true; }
                    }
                } while (extended);
                lines.Add(line);
            }
            return lines;
        }

        // ── Private: geometry helpers ────────────────────────────────────────────

        private static bool Close(PointLatLng a, PointLatLng b, double tol) =>
            Math.Abs(a.Lat - b.Lat) < tol && Math.Abs(a.Lng - b.Lng) < tol;

        private static double PolylineLength(List<PointLatLng> pts)
        {
            double len = 0;
            for (int i = 1; i < pts.Count; i++)
                len += Haversine(pts[i - 1].Lat, pts[i - 1].Lng, pts[i].Lat, pts[i].Lng);
            return len;
        }

        private static double Haversine(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000;
            double dLat = (lat2 - lat1) * Math.PI / 180;
            double dLon = (lon2 - lon1) * Math.PI / 180;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                     + Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180)
                     * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }

        private struct Segment { public PointLatLng A, B; }
    }

    // Renders a georeferenced Bitmap as a map overlay, scaling with zoom and pan.
    public class ElevationBitmapMarker : GMapMarker
    {
        private readonly GMapControl _map;
        private readonly PointLatLng _bottomRight;
        private readonly Bitmap _bitmap;

        public ElevationBitmapMarker(GMapControl map, PointLatLng topLeft, PointLatLng bottomRight, Bitmap bitmap)
            : base(topLeft)
        {
            _map = map;
            _bottomRight = bottomRight;
            _bitmap = bitmap;
            IsHitTestVisible = false;
        }

        public override void OnRender(Graphics g)
        {
            if (_bitmap == null) return;

            var br = _map.FromLatLngToLocal(_bottomRight);
            int x = LocalPosition.X;
            int y = LocalPosition.Y;
            int w = (int)br.X - x;
            int h = (int)br.Y - y;
            if (w <= 0 || h <= 0) return;

            var oldCompositing = g.CompositingMode;
            var oldInterp = g.InterpolationMode;

            g.CompositingMode = CompositingMode.SourceOver;  // ensure alpha blending
            g.InterpolationMode = InterpolationMode.Bilinear;
            g.DrawImage(_bitmap, new Rectangle(x, y, w, h));

            g.InterpolationMode = oldInterp;
            g.CompositingMode = oldCompositing;
        }
    }

    // Elevation text label drawn on the map at a contour position.
    public class ElevationLabel : GMapMarker
    {
        private static readonly Font LabelFont = new Font("Segoe UI", 9, FontStyle.Bold);
        private static readonly Brush TextBrush = new SolidBrush(Color.FromArgb(220, 0, 0, 0));
        private static readonly Brush BgBrush = new SolidBrush(Color.FromArgb(180, 255, 255, 220));
        private static readonly Pen BorderPen = new Pen(Color.FromArgb(180, 120, 80, 40), 1);
        private const int Pad = 2;

        public double ElevationMeters { get; }

        public ElevationLabel(PointLatLng position, double elevationMeters) : base(position)
        {
            ElevationMeters = elevationMeters;
            IsHitTestVisible = false;
        }

        public override void OnRender(Graphics g)
        {
            string text = Props.UseMetric
                ? string.Format("{0:F1} m", ElevationMeters)
                : string.Format("{0:F0} ft", ElevationMeters * 3.28084);

            SizeF sz = g.MeasureString(text, LabelFont);
            float w = sz.Width + Pad * 2;
            float h = sz.Height + Pad * 2;
            float x = LocalPosition.X - w / 2;
            float y = LocalPosition.Y - h / 2;

            g.FillRectangle(BgBrush, x, y, w, h);
            g.DrawRectangle(BorderPen, x, y, w, h);
            g.DrawString(text, LabelFont, TextBrush, x + Pad, y + Pad);
        }
    }
}
