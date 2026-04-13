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
    public class EcOverlayCreator : IDisposable
    {
        private readonly GMapControl _map;

        private PictureBox _legendHost;
        private Bitmap     _legendBitmap;

        private Bitmap       _ecBitmap;
        private PointLatLng  _topLeft;
        private PointLatLng  _bottomRight;

        private bool _disposed;
        private bool _enabled;
        private string _ecPath;
        private STRtree<FieldSample> _tree;
        private List<FieldSample>    _readings;

        private const int MaxCells = 20000;

        public int ColorBands { get; set; } = 5;

        // Red (low EC / sandy) → Orange → Yellow → Light blue → Dark blue (high EC / clay)
        // Matches the common precision-ag EC display convention.
        private static readonly Color[] BandColors =
        {
            Color.FromArgb(215, 25,  28),   // Red        - low EC, sandy soil
            Color.FromArgb(253, 174, 97),   // Orange
            Color.FromArgb(255, 255, 191),  // Yellow
            Color.FromArgb(116, 173, 209),  // Light blue
            Color.FromArgb(44,  123, 182),  // Dark blue  - high EC, clay soil
        };

        public EcOverlayCreator(GMapControl map)
        {
            _map = map;
            _map.Paint += OnMapPaint;

            _legendHost = new PictureBox
            {
                BackColor = Color.Transparent,
                SizeMode  = PictureBoxSizeMode.AutoSize,
                Visible   = false
            };
            _map.Controls.Add(_legendHost);
            _legendHost.BringToFront();

            _enabled = bool.TryParse(Props.GetProp("MapShowEC"), out bool sh) ? sh : false;
        }

        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                Props.SetProp("MapShowEC", _enabled.ToString());
                if (_enabled) Build();
                else Reset();
            }
        }

        public string GetQualitySummary()
        {
            if (_readings == null || _readings.Count == 0) return string.Empty;
            var clean = FilterOutliers(_readings);
            if (clean.Count == 0) return string.Empty;
            double lo = clean.Min(r => r.EcValue);
            double hi = clean.Max(r => r.EcValue);
            return string.Format("{0} pts  {1:F0}–{2:F0} mS/m", clean.Count, lo, hi);
        }

        /// <summary>
        /// Generates a synthetic EC CSV covering the given bounds.
        /// Pattern: two clay-rich patches (high EC) embedded in a sandier background (low EC).
        /// </summary>
        public static void GenerateTestData(
            string outputPath,
            double minLat, double maxLat,
            double minLon, double maxLon,
            double minEc = 12.0,
            double maxEc = 68.0)
        {
            const double spacingMeters = 25.0;
            double latStep = spacingMeters / 111000.0;
            double midLat  = (minLat + maxLat) / 2.0;
            double lonStep = spacingMeters / (111000.0 * Math.Cos(midLat * Math.PI / 180.0));

            double latRange = maxLat - minLat;
            double lonRange = maxLon - minLon;

            var rng = new Random(7531);
            var sb  = new System.Text.StringBuilder();
            sb.AppendLine("Lat,Lon,EC");

            for (double lat = minLat; lat <= maxLat + latStep * 0.5; lat += latStep)
            {
                for (double lon = minLon; lon <= maxLon + lonStep * 0.5; lon += lonStep)
                {
                    double nx = latRange > 0 ? (lat - minLat) / latRange : 0.5;
                    double ny = lonRange > 0 ? (lon - minLon) / lonRange : 0.5;

                    // Two Gaussian clay patches
                    double blob1 = Math.Exp(-((nx - 0.30) * (nx - 0.30) + (ny - 0.35) * (ny - 0.35)) / 0.030);
                    double blob2 = Math.Exp(-((nx - 0.72) * (nx - 0.72) + (ny - 0.70) * (ny - 0.70)) / 0.025);

                    // Low-frequency background variation
                    double bg = 0.35
                              + 0.20 * Math.Sin(nx * Math.PI * 1.5)
                              + 0.15 * Math.Cos(ny * Math.PI * 2.0);

                    double pattern = 0.45 * blob1 + 0.35 * blob2 + 0.20 * bg;
                    pattern = Math.Max(0.0, Math.Min(1.0, pattern));

                    double noise = 0.04 * (rng.NextDouble() * 2.0 - 1.0);
                    double ec    = minEc + (pattern + noise) * (maxEc - minEc);
                    ec = Math.Max(minEc, Math.Min(maxEc, ec));

                    sb.AppendLine(string.Format(
                        System.Globalization.CultureInfo.InvariantCulture,
                        "{0:F6},{1:F6},{2:F2}", lat, lon, ec));
                }
            }

            System.IO.File.WriteAllText(outputPath, sb.ToString());
        }

        public void LoadEcFile(string filePath)
        {
            _ecPath   = filePath;
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
                            System.Globalization.CultureInfo.InvariantCulture, out double ec)) continue;
                    if (ec <= 0.0) continue;
                    _readings.Add(new FieldSample(DateTime.MinValue, lat, lon, 0, 0, 0, ec));
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("EcOverlayCreator/LoadEcFile: " + ex.Message);
                _readings = new List<FieldSample>();
            }
        }

        public void Build()
        {
            if (!_enabled || _disposed) return;

            ClearBitmap();

            if (!string.IsNullOrEmpty(_ecPath) && !System.IO.File.Exists(_ecPath))
                _readings = null;

            try
            {
                if (_readings == null || _readings.Count < 3)
                {
                    _map.Refresh();
                    return;
                }

                List<FieldSample> clean = FilterOutliers(_readings);
                if (clean.Count < 3) clean = _readings;

                double minEc = clean.Min(r => r.EcValue);
                double maxEc = clean.Max(r => r.EcValue);

                if (maxEc - minEc < 0.5)
                {
                    _map.Refresh();
                    return;
                }

                _tree = new STRtree<FieldSample>();
                foreach (var r in clean)
                    _tree.Insert(new Envelope(r.Longitude, r.Longitude, r.Latitude, r.Latitude), r);

                var bounds = ComputeBounds(clean);

                double padLat = (bounds.maxLat - bounds.minLat) * 0.05;
                double padLon = (bounds.maxLon - bounds.minLon) * 0.05;
                bounds = (bounds.minLat - padLat, bounds.maxLat + padLat,
                          bounds.minLon - padLon, bounds.maxLon + padLon);

                double midLat = (bounds.minLat + bounds.maxLat) / 2.0;
                double fieldH = Haversine(bounds.minLat, 0, bounds.maxLat, 0);
                double fieldW = Haversine(midLat, bounds.minLon, midLat, bounds.maxLon);
                double res    = Math.Max(1.0, Math.Sqrt(fieldH * fieldW / MaxCells));
                int    rows   = Math.Max(2, (int)(fieldH / res));
                int    cols   = Math.Max(2, (int)(fieldW / res));

                double fallback = clean.Average(r => r.EcValue);

                int[] pixels = new int[rows * cols];
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        double lat = bounds.minLat + (r + 0.5) / rows * (bounds.maxLat - bounds.minLat);
                        double lon = bounds.minLon + (c + 0.5) / cols * (bounds.maxLon - bounds.minLon);

                        double ec = IDW(lat, lon, fallback);
                        double t  = (ec - minEc) / (maxEc - minEc);
                        t = Math.Max(0, Math.Min(1, t));

                        Color color = GetBandColor(t);
                        pixels[(rows - 1 - r) * cols + c] = Color.FromArgb(100, color).ToArgb();
                    }
                }

                var bmp     = new Bitmap(cols, rows, PixelFormat.Format32bppArgb);
                var bmpData = bmp.LockBits(new Rectangle(0, 0, cols, rows),
                    ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                Marshal.Copy(pixels, 0, bmpData.Scan0, pixels.Length);
                bmp.UnlockBits(bmpData);

                _ecBitmap    = bmp;
                _topLeft     = new PointLatLng(bounds.maxLat, bounds.minLon);
                _bottomRight = new PointLatLng(bounds.minLat, bounds.maxLon);

                ShowLegend(minEc, maxEc);
                _map.Refresh();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("EcOverlayCreator/Build: " + ex.Message);
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
                Props.WriteErrorLog("EcOverlayCreator/Dispose: " + ex.Message);
            }
            _disposed = true;
        }

        // ── Paint handler ────────────────────────────────────────────────────────

        private void OnMapPaint(object sender, PaintEventArgs e)
        {
            if (_ecBitmap == null) return;

            var tl = _map.FromLatLngToLocal(_topLeft);
            var br = _map.FromLatLngToLocal(_bottomRight);
            int x = (int)tl.X;
            int y = (int)tl.Y;
            int w = (int)(br.X - tl.X);
            int h = (int)(br.Y - tl.Y);
            if (w <= 0 || h <= 0) return;

            var oldInterp = e.Graphics.InterpolationMode;
            e.Graphics.InterpolationMode = InterpolationMode.Bilinear;
            e.Graphics.DrawImage(_ecBitmap, new Rectangle(x, y, w, h));
            e.Graphics.InterpolationMode = oldInterp;
        }

        // ── Legend ───────────────────────────────────────────────────────────────

        private void ShowLegend(double minEc, double maxEc)
        {
            try
            {
                double step = (maxEc - minEc) / ColorBands;

                const int itemHeight  = 25;
                const int leftMargin  = 10;
                const int swatch      = 20;
                const int gap         = 10;
                const int rightMargin = 10;
                const int titlePadding = 8;

                using (var font      = new Font("Microsoft Sans Serif", 14))
                using (var titleFont = new Font("Microsoft Sans Serif", 14, FontStyle.Underline))
                {
                    string title = "EC (mS/m)";

                    float maxLabelW = 0;
                    float titleW    = 0;
                    float titleH    = 0;

                    using (var tmp  = new Bitmap(1, 1))
                    using (var gTmp = Graphics.FromImage(tmp))
                    {
                        for (int i = 0; i < ColorBands; i++)
                        {
                            string lbl = string.Format("{0:F0} – {1:F0}",
                                minEc + i * step, minEc + (i + 1) * step);
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

                        for (int i = ColorBands - 1; i >= 0; i--)
                        {
                            int   row      = ColorBands - 1 - i;
                            int   yPos     = itemsTop + row * itemHeight;
                            Color c        = BandColors[i];
                            int   swatchTop = yPos + (itemHeight - swatch) / 2;

                            g.FillRectangle(new SolidBrush(c), anchorX, swatchTop, swatch, swatch);
                            g.DrawRectangle(Pens.White,         anchorX, swatchTop, swatch, swatch);

                            string lbl = string.Format("{0:F0} – {1:F0}",
                                minEc + i * step, minEc + (i + 1) * step);
                            SizeF ls = g.MeasureString(lbl, font);
                            g.DrawString(lbl, font, Brushes.White,
                                anchorX + swatch + gap, yPos + (itemHeight - ls.Height) / 2f);
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
                Props.WriteErrorLog("EcOverlayCreator/ShowLegend: " + ex.Message);
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
            _ecBitmap?.Dispose();
            _ecBitmap = null;
        }

        private static List<FieldSample> FilterOutliers(List<FieldSample> src)
        {
            double mean = src.Average(r => r.EcValue);
            double sd   = Math.Sqrt(src.Average(r => Math.Pow(r.EcValue - mean, 2)));
            double lo   = mean - 3 * sd;
            double hi   = mean + 3 * sd;
            return src.Where(r => r.EcValue >= lo && r.EcValue <= hi).ToList();
        }

        private double IDW(double lat, double lon, double fallback)
        {
            var env = new Envelope(lon - 0.002, lon + 0.002, lat - 0.002, lat + 0.002);
            var pts = _tree.Query(env);

            if (pts.Count == 0) return fallback;

            double vSum = 0, wSum = 0;
            foreach (var p in pts.Take(8))
            {
                double d = Haversine(lat, lon, p.Latitude, p.Longitude);
                double w = 1.0 / (d * d + 0.0001);
                vSum += w * p.EcValue;
                wSum += w;
            }
            return vSum / wSum;
        }

        private Color GetBandColor(double t)
        {
            int band = (int)(t * ColorBands);
            return BandColors[Math.Min(band, BandColors.Length - 1)];
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
