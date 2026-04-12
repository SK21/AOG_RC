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
using System.Windows.Forms;

namespace RateController.RateMap
{
    public class ElevationOverlayCreator : IDisposable
    {
        private readonly GMapControl _map;

        // Legend hosted as a PictureBox on the GMapControl
        private PictureBox _legendHost;
        private Bitmap     _legendBitmap;

        // Pixel map — a single Bitmap stretched over the field in the Paint handler
        private Bitmap       _elevBitmap;
        private PointLatLng  _topLeft;
        private PointLatLng  _bottomRight;

        private bool _disposed;
        private bool _enabled;
        private string _elevationPath;
        private STRtree<FieldSample> _tree;
        private List<FieldSample>    _readings;
        private List<FieldSample>    _cleanReadings;

        // IDW grid resolution — bitmap rendering cost is O(1) regardless of this value,
        // so it can be much higher than the polygon cap without affecting pan/zoom performance.
        // Build time scales linearly; ~20 000 takes under a second for typical field data.
        private const int MaxCells = 20000;

        // Number of discrete colour bands
        public int ColorBands { get; set; } = 5;

        // Kept for API compatibility; not currently used
        public double ContourInterval { get; set; } = 1.0;

        // Colour palette — dark blue (low) → dark red (high), 5 bands
        private static readonly Color[] BandColors =
        {
            Color.DarkBlue,
            Color.Cyan,
            Color.Green,
            Color.Orange,
            Color.DarkRed
        };

        public ElevationOverlayCreator(GMapControl map)
        {
            _map = map;

            // Subscribe to the map's own Paint event.
            // GMapControl raises Paint after drawing tiles and overlays, so our bitmap
            // lands on top. e.Graphics is the control's client-area surface — the same
            // coordinate space that FromLatLngToLocal returns, so no offset ambiguity.
            _map.Paint += OnMapPaint;

            _legendHost = new PictureBox
            {
                BackColor = Color.Transparent,
                SizeMode  = PictureBoxSizeMode.AutoSize,
                Visible   = false
            };
            _map.Controls.Add(_legendHost);
            _legendHost.BringToFront();

            _enabled = bool.TryParse(Props.GetProp("MapShowElevation"), out bool sh) ? sh : false;
        }

        // The cleaned (non-zero, outlier-filtered) elevation readings used by the last Build().
        // Null until Build() has run successfully at least once.
        public IReadOnlyList<FieldSample> CleanReadings => _cleanReadings;

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
        public string GetQualitySummary()
        {
            if (_readings == null || _readings.Count == 0) return string.Empty;

            List<FieldSample> nonZero = _readings.Where(r => r.ElevationMeters != 0.0).ToList();
            if (nonZero.Count == 0) return string.Empty;

            List<FieldSample> clean = FilterOutliers(nonZero);
            int dropped = nonZero.Count - clean.Count;
            if (clean.Count == 0) clean = nonZero;

            double minElev = clean.Min(r => r.ElevationMeters);
            double maxElev = clean.Max(r => r.ElevationMeters);
            double range   = maxElev - minElev;

            string rangeStr = Props.UseMetric
                ? string.Format("{0:F1}m", range)
                : string.Format("{0:F0}ft", range * 3.28084);

            string flag        = (range < 0.5 || clean.Count < 15) ? " !" : "";
            string outlierNote = dropped > 0 ? string.Format(" -{0}outliers", dropped) : "";
            return string.Format("{0} pts  {1} range{2}{3}", clean.Count, rangeStr, flag, outlierNote);
        }

        // Generates a synthetic elevation CSV covering the given bounds.
        public static void GenerateTestData(
            string outputPath,
            double minLat, double maxLat,
            double minLon, double maxLon,
            double baseElevation = 100.0)
        {
            const double spacingMeters = 25.0;
            double latStep = spacingMeters / 111000.0;
            double midLat  = (minLat + maxLat) / 2.0;
            double lonStep = spacingMeters / (111000.0 * Math.Cos(midLat * Math.PI / 180.0));

            double latRange = maxLat - minLat;
            double lonRange = maxLon - minLon;

            var rng = new Random(4219);
            var sb  = new System.Text.StringBuilder();
            sb.AppendLine("Lat,Lon,Elevation");

            for (double lat = minLat; lat <= maxLat + latStep * 0.5; lat += latStep)
            {
                for (double lon = minLon; lon <= maxLon + lonStep * 0.5; lon += lonStep)
                {
                    double nx = latRange > 0 ? (lat - minLat) / latRange : 0.5;
                    double ny = lonRange > 0 ? (lon - minLon) / lonRange : 0.5;

                    double slope = 6.1 * (1.0 - ny);
                    double roll  = 0.5 * Math.Sin(nx * Math.PI * 4.0)
                                 + 0.3 * Math.Cos(ny * Math.PI * 3.0);
                    double cross = 0.2 * Math.Sin((nx + ny) * Math.PI * 3.0);
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
            _elevationPath = filePath;
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

            ClearBitmap();

            if (!string.IsNullOrEmpty(_elevationPath) && !System.IO.File.Exists(_elevationPath))
                _readings = null;

            try
            {
                if (_readings == null || _readings.Count < 3)
                {
                    _map.Refresh();
                    return;
                }

                // Strip GPS zero-sentinels before outlier detection.
                List<FieldSample> nonZero = _readings.Where(r => r.ElevationMeters != 0.0).ToList();
                if (nonZero.Count < 3)
                {
                    _map.Refresh();
                    return;
                }

                List<FieldSample> clean = FilterOutliers(nonZero);
                if (clean.Count < 3) clean = nonZero;

                _cleanReadings = clean;   // expose for ProductivityZoneCreator

                double minElev = clean.Min(r => r.ElevationMeters);
                double maxElev = clean.Max(r => r.ElevationMeters);

                if (maxElev - minElev < 0.1)
                {
                    _map.Refresh();
                    return;
                }

                // Build spatial index for IDW lookups.
                _tree = new STRtree<FieldSample>();
                foreach (var r in clean)
                    _tree.Insert(new Envelope(r.Longitude, r.Longitude, r.Latitude, r.Latitude), r);

                var bounds = ComputeBounds(clean);

                // 5% padding so the grid covers the outer edge of the data.
                double padLat = (bounds.maxLat - bounds.minLat) * 0.05;
                double padLon = (bounds.maxLon - bounds.minLon) * 0.05;
                bounds = (bounds.minLat - padLat, bounds.maxLat + padLat,
                          bounds.minLon - padLon, bounds.maxLon + padLon);

                // Adaptive resolution — cell count capped at MaxCells regardless of field size.
                double midLat = (bounds.minLat + bounds.maxLat) / 2.0;
                double fieldH = Haversine(bounds.minLat, 0, bounds.maxLat, 0);
                double fieldW = Haversine(midLat, bounds.minLon, midLat, bounds.maxLon);
                double res    = Math.Max(1.0, Math.Sqrt(fieldH * fieldW / MaxCells));
                int    rows   = Math.Max(2, (int)(fieldH / res));
                int    cols   = Math.Max(2, (int)(fieldW / res));

                // Build pixel array — row 0 in the grid = minLat (south) = bottom of bitmap,
                // so flip vertically so the bitmap's top matches the map's north edge.
                int[] pixels = new int[rows * cols];
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        double lat = bounds.minLat + (r + 0.5) / rows * (bounds.maxLat - bounds.minLat);
                        double lon = bounds.minLon + (c + 0.5) / cols * (bounds.maxLon - bounds.minLon);

                        double elev = IDW(lat, lon);
                        double t    = (elev - minElev) / (maxElev - minElev);
                        t = Math.Max(0, Math.Min(1, t));

                        Color color = GetBandColor(t);
                        // Flip row: bitmap row 0 = north (maxLat) = top of screen
                        pixels[(rows - 1 - r) * cols + c] = Color.FromArgb(100, color).ToArgb();
                    }
                }

                var bmp = new Bitmap(cols, rows, PixelFormat.Format32bppArgb);
                var bmpData = bmp.LockBits(
                    new Rectangle(0, 0, cols, rows),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format32bppArgb);
                Marshal.Copy(pixels, 0, bmpData.Scan0, pixels.Length);
                bmp.UnlockBits(bmpData);

                _elevBitmap  = bmp;
                _topLeft     = new PointLatLng(bounds.maxLat, bounds.minLon);   // NW corner
                _bottomRight = new PointLatLng(bounds.minLat, bounds.maxLon);   // SE corner

                ShowLegend(minElev, maxElev);
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
            ClearBitmap();
            HideLegend();
            _map.Refresh();
        }

        public void Dispose()
        {
            if (_disposed) return;
            try
            {
                _map.Paint -= OnMapPaint;

                ClearBitmap();
                HideLegend();

                if (_legendHost != null)
                {
                    if (_map.Controls.Contains(_legendHost))
                        _map.Controls.Remove(_legendHost);
                    _legendHost.Dispose();
                    _legendHost = null;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ElevationOverlayCreator/Dispose: " + ex.Message);
            }
            _disposed = true;
        }

        // ── Paint handler ────────────────────────────────────────────────────────

        // Called by GMapControl after it has finished drawing tiles and overlays.
        // e.Graphics is the control's client-area surface — same coordinate space
        // as FromLatLngToLocal — so the rectangle computed here is always correct
        // regardless of zoom or pan level.
        private void OnMapPaint(object sender, PaintEventArgs e)
        {
            if (_elevBitmap == null) return;

            var tl = _map.FromLatLngToLocal(_topLeft);
            var br = _map.FromLatLngToLocal(_bottomRight);
            int x = (int)tl.X;
            int y = (int)tl.Y;
            int w = (int)(br.X - tl.X);
            int h = (int)(br.Y - tl.Y);
            if (w <= 0 || h <= 0) return;

            var oldInterp = e.Graphics.InterpolationMode;
            e.Graphics.InterpolationMode = InterpolationMode.Bilinear;
            e.Graphics.DrawImage(_elevBitmap, new Rectangle(x, y, w, h));
            e.Graphics.InterpolationMode = oldInterp;
        }

        // ── Legend ───────────────────────────────────────────────────────────────

        private void ShowLegend(double minElev, double maxElev)
        {
            try
            {
                bool   metric = Props.UseMetric;
                string unit   = metric ? "m" : "ft";
                double scale  = metric ? 1.0 : 3.28084;
                double lo     = minElev * scale;
                double hi     = maxElev * scale;
                double step   = (hi - lo) / ColorBands;

                // Spacing constants matched to LegendManager
                const int itemHeight   = 25;
                const int leftMargin   = 10;
                const int swatch       = 20;
                const int gap          = 10;
                const int rightMargin  = 10;
                const int titlePadding = 8;

                using (var font      = new Font("Microsoft Sans Serif", 14))
                using (var titleFont = new Font("Microsoft Sans Serif", 14, FontStyle.Underline))
                {
                    string title = string.Format("Elevation ({0})", unit);

                    float maxLabelW = 0;
                    float titleW    = 0;
                    float titleH    = 0;

                    using (var tmp  = new Bitmap(1, 1))
                    using (var gTmp = Graphics.FromImage(tmp))
                    {
                        for (int i = 0; i < ColorBands; i++)
                        {
                            string lbl = FormatBand(lo + i * step, lo + (i + 1) * step);
                            maxLabelW = Math.Max(maxLabelW, gTmp.MeasureString(lbl, font).Width);
                        }
                        SizeF ts = gTmp.MeasureString(title, titleFont);
                        titleW = ts.Width;
                        titleH = ts.Height;
                    }

                    int contentW = swatch + gap + (int)Math.Ceiling(maxLabelW);
                    int bmpW     = Math.Max((int)Math.Ceiling(titleW) + leftMargin * 2,
                                           leftMargin + contentW + rightMargin);
                    int bmpH     = (int)Math.Ceiling(titleH) + titlePadding * 2
                                   + ColorBands * itemHeight + leftMargin;

                    _legendBitmap?.Dispose();
                    _legendBitmap = new Bitmap(bmpW, bmpH);

                    using (var g = Graphics.FromImage(_legendBitmap))
                    {
                        g.SmoothingMode = SmoothingMode.None;
                        g.FillRectangle(Brushes.Black, 0, 0, bmpW, bmpH);

                        SizeF ts = g.MeasureString(title, titleFont);
                        g.DrawString(title, titleFont, Brushes.White,
                            (bmpW - ts.Width) / 2f, titlePadding);

                        int itemsTop = (int)Math.Ceiling(ts.Height) + titlePadding * 2;
                        int anchorX  = Math.Max(leftMargin, (bmpW - contentW) / 2);

                        // Highest band at top, lowest at bottom
                        for (int i = ColorBands - 1; i >= 0; i--)
                        {
                            int   row      = ColorBands - 1 - i;
                            int   y        = itemsTop + row * itemHeight;
                            Color c        = BandColors[i];
                            int   swatchTop = y + (itemHeight - swatch) / 2;

                            g.FillRectangle(new SolidBrush(c), anchorX, swatchTop, swatch, swatch);
                            g.DrawRectangle(Pens.White,         anchorX, swatchTop, swatch, swatch);

                            string lbl  = FormatBand(lo + i * step, lo + (i + 1) * step);
                            SizeF  ls   = g.MeasureString(lbl, font);
                            g.DrawString(lbl, font, Brushes.White,
                                anchorX + swatch + gap, y + (itemHeight - ls.Height) / 2f);
                        }
                    }
                }

                _legendHost.Image   = _legendBitmap;
                _legendHost.Visible = true;
                _legendHost.Left    = 10;
                _legendHost.Top     = 10;
                _legendHost.BringToFront();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ElevationOverlayCreator/ShowLegend: " + ex.Message);
            }
        }

        private void HideLegend()
        {
            if (_legendHost != null) _legendHost.Visible = false;
            _legendBitmap?.Dispose();
            _legendBitmap = null;
        }

        // ── Private helpers ──────────────────────────────────────────────────────

        private void ClearBitmap()
        {
            _elevBitmap?.Dispose();
            _elevBitmap = null;
        }

        private static List<FieldSample> FilterOutliers(List<FieldSample> src)
        {
            double mean = src.Average(r => r.ElevationMeters);
            double sd   = Math.Sqrt(src.Average(r => Math.Pow(r.ElevationMeters - mean, 2)));
            double lo   = mean - 3 * sd;
            double hi   = mean + 3 * sd;
            return src.Where(r => r.ElevationMeters >= lo && r.ElevationMeters <= hi).ToList();
        }

        private double IDW(double lat, double lon)
        {
            var env = new Envelope(lon - 0.002, lon + 0.002, lat - 0.002, lat + 0.002);
            var pts = _tree.Query(env);

            if (pts.Count == 0)
                return _readings.Where(r => r.ElevationMeters != 0.0).Average(r => r.ElevationMeters);

            double vSum = 0, wSum = 0;
            foreach (var p in pts.Take(8))
            {
                double d = Haversine(lat, lon, p.Latitude, p.Longitude);
                double w = 1.0 / (d * d + 0.0001);
                vSum += w * p.ElevationMeters;
                wSum += w;
            }
            return vSum / wSum;
        }

        private Color GetBandColor(double t)
        {
            int band = (int)(t * ColorBands);
            return BandColors[Math.Min(band, BandColors.Length - 1)];
        }

        private static string FormatBand(double lo, double hi)
        {
            return string.Format("{0} - {1}", (int)Math.Round(lo), (int)Math.Round(hi));
        }

        private static (double minLat, double maxLat, double minLon, double maxLon) ComputeBounds(
            List<FieldSample> src) =>
        (
            src.Min(r => r.Latitude),
            src.Max(r => r.Latitude),
            src.Min(r => r.Longitude),
            src.Max(r => r.Longitude)
        );

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
    }
}
