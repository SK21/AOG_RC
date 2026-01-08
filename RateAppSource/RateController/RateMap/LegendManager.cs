using GMap.NET.WindowsForms;
using RateController.RateMap;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RateController.Classes
{
    public class LegendBand
    {
        public string ColorHtml { get; set; }
        public double Max { get; set; }
        public double Min { get; set; }

        // e.g. "#FF0000"
        public int ProductIndex { get; set; }   // 0 = ProductA, 1 = ProductB, ...
    }

    /// <summary>
    /// Manages the legend rendering and layout for the map view.
    /// Renders a fixed-position legend using a PictureBox hosted on the GMap control.
    /// </summary>
    public class LegendManager : IDisposable
    {
        private readonly GMapControl gmap;
        private Dictionary<string, Color> cAppliedLegend;
        private bool cEnabled;
        private Dictionary<string, Color> lastLegend;
        private Bitmap legendBitmap;
        private Font legendFont;
        private PictureBox legendHost;

        public LegendManager(GMapControl gmap)
        {
            this.gmap = gmap ?? throw new ArgumentNullException(nameof(gmap));
            legendFont = new Font("Microsoft Sans Serif", 14);

            legendHost = new PictureBox
            {
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.AutoSize,
                Visible = false
            };
            this.gmap.Controls.Add(legendHost);
            legendHost.BringToFront();

            // Reposition on size changes
            this.gmap.SizeChanged += Gmap_SizeChanged;
            cAppliedLegend = new Dictionary<string, Color>();
        }

        public Dictionary<string, Color> AppliedLegend
        {
            get { return cAppliedLegend; }
            set
            {
                if (LegendsDiffer(cAppliedLegend, value)) cAppliedLegend = value;
            }
        }

        public bool Enabled
        {
            get { return cEnabled; }
            set
            {
                if (cEnabled != value)
                {
                    cEnabled = value;
                    if (cEnabled)
                    {
                        // Rebuild legend if we already have content
                        UpdateLegend(lastLegend);
                    }
                    else
                    {
                        Clear();
                    }
                }
            }
        }

        public Dictionary<string, Color> BuildAppliedZonesLegend(List<MapZone> zones, int ProductFilter = 0)
        {
            var legend = new Dictionary<string, Color>();

            string productKey = $"Product{(char)('A' + ProductFilter)}";
            var appliedRates = zones
                .Where(z => z.ZoneType == ZoneType.Applied)
                .Where(z => z.Rates != null && z.Rates.ContainsKey(productKey))
                .Select(z => z.Rates[productKey])
                .Where(r => r > 0.01);

            if (appliedRates.Any())
            {
                if (MapController.TryComputeScale(appliedRates, out double minRate, out double maxRate))
                {
                    legend = CreateAppliedLegend(minRate, maxRate, 5);
                }
            }
            else
            {
                legend.Add("No data", Color.Gray);
            }

            return legend;
        }

        public void Clear()
        {
            try
            {
                if (legendHost != null)
                {
                    if (legendHost.Image != null) legendHost.Image = null;
                    legendHost.Visible = false;
                }
                if (legendBitmap != null)
                {
                    legendBitmap.Dispose();
                    legendBitmap = null;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("LegendManager/Clear: " + ex.Message);
            }
        }

        public Dictionary<string, Color> CreateAppliedLegend(double minRate, double maxRate, int steps = 5)
        {
            var legend = new Dictionary<string, Color>();

            if (minRate < maxRate)
            {
                double band = (maxRate - minRate) / steps;

                // Determine formatting and increment rules based on magnitude
                bool largeNumbers = (minRate >= 1000 || maxRate >= 1000);
                string format = largeNumbers ? "N0" : "N1";
                double increment = largeNumbers ? 1.0 : 0.1;

                double a = minRate;

                for (int i = 0; i < steps; i++)
                {
                    double b = (i == steps - 1)
                        ? maxRate
                        : a + band;

                    // Round to avoid floating-point artifacts
                    a = Math.Round(a, 3);
                    b = Math.Round(b, 3);

                    var color = Palette.GetColor(i, 255);
                    legend.Add(string.Format(CultureInfo.CurrentCulture, "{0} - {1}", a.ToString(format, CultureInfo.CurrentCulture),
                        b.ToString(format, CultureInfo.CurrentCulture)), color);

                    // Next band starts just above the previous band
                    a = b + increment;
                }
            }
            else
            {
                legend.Add("No data", Color.Gray);
            }

            return legend;
        }

        public void Dispose()
        {
            try
            {
                if (gmap != null)
                {
                    gmap.SizeChanged -= Gmap_SizeChanged;
                }
                if (legendHost != null)
                {
                    if (gmap != null && gmap.Controls.Contains(legendHost))
                    {
                        gmap.Controls.Remove(legendHost);
                    }
                    legendHost.Image = null;
                    legendHost.Dispose();
                    legendHost = null;
                }
                legendBitmap?.Dispose();
                legendBitmap = null;
                legendFont?.Dispose();
                legendFont = null;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("LegendManager/Dispose: " + ex.Message);
            }
        }

        public void EnsureLegendTop()
        {
            // PictureBox is a control; ensure it stays on top
            legendHost?.BringToFront();
        }

        public bool LegendsDiffer(Dictionary<string, Color> A, Dictionary<string, Color> B)
        {
            if (A == B) return false;
            if (A == null || B == null) return true;
            if (A.Count != B.Count) return true;

            foreach (var kvp in A)
            {
                if (!B.TryGetValue(kvp.Key, out Color bColor)) return true;
                if (bColor != kvp.Value) return true;
            }
            return false;
        }

        public Dictionary<string, Color> LoadPersistedLegend(string basePath = null)
        {
            try
            {
                if (basePath == null)
                {
                    basePath = Path.ChangeExtension(JobManager.CurrentMapPath, null);
                }

                string legendPath = basePath + "_AppliedLegend.json";
                if (!File.Exists(legendPath))
                {
                    return null;
                }

                var json = File.ReadAllText(legendPath);
                var bands = System.Text.Json.JsonSerializer.Deserialize<List<LegendBand>>(json);
                if (bands == null || bands.Count == 0)
                {
                    return null;
                }

                // Filter to current product
                var filtered = bands
                    .Where(b => b.ProductIndex == MapController.ProductFilter)
                    .OrderBy(b => b.Min)
                    .ToList();

                int Steps = filtered.Count;
                if (Steps == 0)
                {
                    return null;
                }

                double globalMin = filtered.First().Min;
                double globalMax = filtered.Max(b => b.Max);

                return CreateAppliedLegend(globalMin, globalMax, Steps);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("LegendManager/LoadPersistedLegend: " + ex.Message);
                return null;
            }
        }

        public void OnMapZoomChanged()
        {
            PositionLegendHost();
        }

        public void SaveAppliedLegend(string legendPath, Dictionary<string, Color> LegendToSave = null)
        {
            try
            {
                if (LegendToSave == null) LegendToSave = cAppliedLegend;

                if (LegendToSave == null || LegendToSave.Count == 0)
                {
                    return;
                }

                var bands = new List<LegendBand>();
                foreach (var kvp in LegendToSave)
                {
                    var parts = kvp.Key.Split('-');
                    if (parts.Length != 2) continue;

                    string leftText = parts[0].Trim();
                    string rightText = parts[1].Trim();

                    // Parse using current culture because labels were formatted with CurrentCulture
                    if (!double.TryParse(leftText, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out double min)) continue;
                    if (!double.TryParse(rightText, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out double max)) continue;

                    bands.Add(new LegendBand
                    {
                        Min = min,
                        Max = max,
                        ColorHtml = ColorTranslator.ToHtml(kvp.Value),
                        ProductIndex = MapController.ProductFilter
                    });
                }

                if (bands.Count == 0)
                {
                    return;
                }

                var basePath = Path.ChangeExtension(legendPath, null); // strip .shp
                var appliedLegendPath = basePath + "_AppliedLegend.json";

                var json = System.Text.Json.JsonSerializer.Serialize(bands);
                File.WriteAllText(appliedLegendPath, json);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("LegendManager/SaveAppliedLegend: " + ex.Message);
            }
        }

        public void ShowLegend(Dictionary<string, Color> LegendToShow = null, bool Show = true)
        {
            if (Show && MapController.State != MapState.Preview && cEnabled)
            {
                if (LegendToShow == null) LegendToShow = cAppliedLegend;
                UpdateLegend(LegendToShow);
            }
            else
            {
                Clear();
            }
        }

        public void UpdateLegend(Dictionary<string, Color> legend)
        {
            if (!cEnabled || legend == null || legend.Count == 0)
            {
                Clear();
                return;
            }

            lastLegend = legend;

            const int itemHeight = 25;
            const int leftMargin = 10;
            const int swatch = 20;
            const int gap = 10;
            const int rightMargin = 10;
            const int dashGap = 6;

            // Measure max widths of left and right values
            float maxLeftWidth = 0;
            float maxRightWidth = 0;
            float dashWidth;

            using (var bmp = new Bitmap(1, 1))
            using (var g = Graphics.FromImage(bmp))
            {
                dashWidth = g.MeasureString("-", legendFont).Width;

                foreach (var kv in legend)
                {
                    var parts = kv.Key.Split('-');
                    if (parts.Length != 2)
                        continue;

                    var left = parts[0].Trim();
                    var right = parts[1].Trim();

                    maxLeftWidth = Math.Max(maxLeftWidth, g.MeasureString(left, legendFont).Width);
                    maxRightWidth = Math.Max(maxRightWidth, g.MeasureString(right, legendFont).Width);
                }
            }

            int textBlockWidth = (int)Math.Ceiling(
                maxLeftWidth + dashGap + dashWidth + dashGap + maxRightWidth);

            int contentWidth = swatch + gap + textBlockWidth;
            int legendHeight = (legend.Count * itemHeight) + (leftMargin * 2);
            int legendWidth = Math.Max(120, leftMargin + contentWidth + rightMargin);

            legendBitmap?.Dispose();
            legendBitmap = new Bitmap(legendWidth, legendHeight);

            using (var g2 = Graphics.FromImage(legendBitmap))
            using (var backBrush = new SolidBrush(Color.Black))
            {
                g2.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g2.FillRectangle(backBrush, 0, 0, legendWidth, legendHeight);

                int anchorStartX = Math.Max(leftMargin, (legendWidth - contentWidth) / 2);
                int y = leftMargin;

                foreach (var kv in legend)
                {
                    Color color = kv.Value;
                    int swatchTop = y + (itemHeight - swatch) / 2;

                    using (var brush = new SolidBrush(color))
                    {
                        g2.FillRectangle(brush, anchorStartX, swatchTop, swatch, swatch);
                        g2.DrawRectangle(Pens.White, anchorStartX, swatchTop, swatch, swatch);
                    }

                    var parts = kv.Key.Split('-');
                    string left = parts.Length > 0 ? parts[0].Trim() : "";
                    string right = parts.Length > 1 ? parts[1].Trim() : "";

                    var leftSize = g2.MeasureString(left, legendFont);
                    var rightSize = g2.MeasureString(right, legendFont);
                    var dashSize = g2.MeasureString("-", legendFont);

                    float textY = y + (itemHeight - leftSize.Height) / 2f;

                    float textStartX = anchorStartX + swatch + gap;

                    // Left value (left-aligned)
                    g2.DrawString(left, legendFont, Brushes.White,
                        new PointF(textStartX, textY));

                    // Dash (centered)
                    float dashX = textStartX + maxLeftWidth + dashGap;
                    g2.DrawString("-", legendFont, Brushes.White,
                        new PointF(dashX, textY));

                    // Right value (right-aligned)
                    float rightX = textStartX + maxLeftWidth + dashGap + dashSize.Width + dashGap
                                   + maxRightWidth - rightSize.Width;

                    g2.DrawString(right, legendFont, Brushes.White,
                        new PointF(rightX, textY));

                    y += itemHeight;
                }
            }

            legendHost.Image = legendBitmap;
            legendHost.Visible = true;
            PositionLegendHost();
        }

        private void Gmap_SizeChanged(object sender, EventArgs e)
        {
            PositionLegendHost();
        }

        private void PositionLegendHost()
        {
            if (legendHost == null || legendBitmap == null || !legendHost.Visible || gmap == null) return;
            int marginTop = 10;
            int marginRight = 10;
            legendHost.Left = gmap.Width - legendHost.Width - marginRight;
            legendHost.Top = marginTop;
            legendHost.BringToFront();
        }
    }
}