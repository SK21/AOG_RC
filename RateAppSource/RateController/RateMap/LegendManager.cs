using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RateController.Classes
{
    /// <summary>
    /// Manages the legend rendering and layout for the map view.
    /// Renders a fixed-position legend using a PictureBox hosted on the GMap control.
    /// </summary>
    public sealed class LegendManager : IDisposable
    {
        // Regex to find standalone numeric tokens (with optional decimals) and not part of alpha words (e.g., avoid P1)
        private static readonly Regex LegendNumberRegex = new Regex(@"(?<![A-Za-z])(-?\d+(?:[\.,]\d+)?)(?![A-Za-z])", RegexOptions.Compiled);

        private readonly GMapControl gmap;
        private Dictionary<string, Color> lastLegend;
        private Bitmap legendBitmap;
        private Font legendFont;
        private PictureBox legendHost;
        private int legendRightMarginPx = 0;
        private string legendSignature;
        private bool legendOverlayEnabled; // backing field

        public LegendManager(GMapControl gmap)
        {
            this.gmap = gmap ?? throw new ArgumentNullException(nameof(gmap));
            legendFont = new Font("Arial", 11);

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
        }

        // Previously an auto-property. We now invoke update/clear when toggled so enabling later shows existing legend.
        public bool LegendOverlayEnabled
        {
            get { return legendOverlayEnabled; }
            set
            {
                if (legendOverlayEnabled != value)
                {
                    legendOverlayEnabled = value;
                    if (legendOverlayEnabled)
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

        public int LegendRightMarginPx
        {
            get { return legendRightMarginPx; }
            set
            {
                if (legendRightMarginPx != value)
                {
                    legendRightMarginPx = value < 0 ? 0 : value;
                    // reposition/redraw when margin changes
                    UpdateLegend(lastLegend);
                }
            }
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
                legendSignature = null;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("LegendManager/Clear: " + ex.Message);
            }
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
                legendSignature = null;
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

        public void OnMapZoomChanged()
        {
            PositionLegendHost();
        }

        public void UpdateLegend(Dictionary<string, Color> legend)
        {
            lastLegend = legend;

            if (legendHost == null) return;

            if (!LegendOverlayEnabled || legend == null || legend.Count == 0)
            {
                Clear();
                return;
            }

            // Build a signature to detect changes in legend content (order + color)
            string newSig;
            unchecked
            {
                var orderedForSig = OrderLegend(legend);
                var adjustedForSig = AdjustLegendLabels(orderedForSig);
                var parts = new List<string>(adjustedForSig.Count);
                foreach (var kv in adjustedForSig)
                {
                    var col = kv.Value;
                    string key = FormatLegendLabelNoDecimals(kv.Key);
                    parts.Add(key + "#" + col.ToArgb().ToString("X8"));
                }
                newSig = string.Join("|", parts);
            }

            const int itemHeight = 25;
            const int leftMargin = 10;
            const int swatch = 20;
            const int gap = 10;
            const int rightMargin = 10;

            var orderedItems = OrderLegend(legend);
            var adjustedItems = AdjustLegendLabels(orderedItems);

            int maxTextWidth = 0;
            using (var measurementBmp = new Bitmap(1, 1))
            using (var measurementG = Graphics.FromImage(measurementBmp))
            {
                foreach (var item in adjustedItems)
                {
                    string label = FormatLegendLabelNoDecimals(item.Key);
                    var textSize = measurementG.MeasureString(label, legendFont);
                    if (textSize.Width > maxTextWidth) maxTextWidth = (int)Math.Ceiling(textSize.Width);
                }
            }

            int maxContentWidth = swatch + gap + maxTextWidth;
            int legendHeight = (legend.Count * itemHeight) + (leftMargin * 2);
            int legendWidth = Math.Max(120, leftMargin + maxContentWidth + rightMargin);

            if (legendBitmap == null || !string.Equals(newSig, legendSignature, StringComparison.Ordinal))
            {
                legendBitmap?.Dispose();
                legendBitmap = new Bitmap(legendWidth, legendHeight);

                using (var g2 = Graphics.FromImage(legendBitmap))
                using (var backBrush = new SolidBrush(Color.Black))
                {
                    g2.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    g2.FillRectangle(backBrush, 0, 0, legendWidth, legendHeight);

                    int anchorStartX = Math.Max(leftMargin, (legendWidth - maxContentWidth) / 2);
                    int y = leftMargin;
                    foreach (var item in adjustedItems)
                    {
                        string label = FormatLegendLabelNoDecimals(item.Key);
                        var textSize = g2.MeasureString(label, legendFont);
                        int startX = anchorStartX;
                        int swatchTop = y + (itemHeight - swatch) / 2;
                        using (var brush = new SolidBrush(item.Value))
                        {
                            g2.FillRectangle(brush, startX, swatchTop, swatch, swatch);
                            g2.DrawRectangle(Pens.White, startX, swatchTop, swatch, swatch);
                        }
                        float textY = y + (itemHeight - textSize.Height) / 2f;
                        g2.DrawString(label, legendFont, Brushes.White, new PointF(startX + swatch + gap, textY));
                        y += itemHeight;
                    }
                }
                legendSignature = newSig;
            }

            legendHost.Image = legendBitmap;
            legendHost.Visible = true;
            PositionLegendHost();
        }

        private static List<KeyValuePair<string, Color>> AdjustLegendLabels(List<KeyValuePair<string, Color>> orderedLegend)
        {
            var result = new List<KeyValuePair<string, Color>>();
            if (orderedLegend == null || orderedLegend.Count == 0) return result;

            int? prevEnd = null;

            foreach (var kv in orderedLegend)
            {
                var nums = ExtractLegendNumbers(kv.Key);
                string newLabel = kv.Key;

                if (nums.Count >= 2)
                {
                    int lower = (int)Math.Round(nums[0]);
                    int upper = (int)Math.Round(nums[1]);

                    if (prevEnd.HasValue && lower <= prevEnd.Value)
                    {
                        lower = prevEnd.Value + 1;
                    }

                    if (lower > upper)
                    {
                        upper = lower;
                    }

                    newLabel = (lower == upper) ? lower.ToString(CultureInfo.InvariantCulture)
                                                : string.Format(CultureInfo.InvariantCulture, "{0}-{1}", lower, upper);
                    prevEnd = upper;
                }
                else if (nums.Count == 1)
                {
                    int val = (int)Math.Round(nums[0]);
                    if (prevEnd.HasValue && val <= prevEnd.Value)
                    {
                        val = prevEnd.Value + 1;
                    }
                    newLabel = val.ToString(CultureInfo.InvariantCulture);
                    prevEnd = val;
                }
                else
                {
                    newLabel = kv.Key;
                }

                result.Add(new KeyValuePair<string, Color>(newLabel, kv.Value));
            }

            return result;
        }

        private static List<double> ExtractLegendNumbers(string label)
        {
            var nums = new List<double>();
            if (string.IsNullOrEmpty(label)) return nums;
            foreach (Match m in LegendNumberRegex.Matches(label))
            {
                var s = m.Groups[1].Value.Replace(',', '.');
                double val;
                if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out val))
                {
                    nums.Add(val);
                }
            }
            return nums;
        }

        private static string FormatLegendLabelNoDecimals(string label)
        {
            if (string.IsNullOrEmpty(label)) return label ?? string.Empty;
            return LegendNumberRegex.Replace(label, m =>
            {
                var s = m.Groups[1].Value.Replace(',', '.');
                double val;
                if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out val))
                {
                    int ival = (int)Math.Round(val);
                    return ival.ToString(CultureInfo.InvariantCulture);
                }
                return m.Value;
            });
        }

        private static List<KeyValuePair<string, Color>> OrderLegend(Dictionary<string, Color> legend)
        {
            if (legend == null || legend.Count == 0) return new List<KeyValuePair<string, Color>>();

            var ordered = legend
                .Select(kv => new { kv.Key, kv.Value, nums = ExtractLegendNumbers(kv.Key) })
                .OrderBy(x => x.nums.Count > 0 ? 0 : 1)
                .ThenBy(x => x.nums.Count > 0 ? x.nums[0] : double.MaxValue)
                .ThenBy(x => x.nums.Count > 1 ? x.nums[1] : double.MaxValue)
                .ThenBy(x => x.Key, StringComparer.Ordinal)
                .Select(x => new KeyValuePair<string, Color>(x.Key, x.Value))
                .ToList();

            return ordered;
        }

        private void Gmap_SizeChanged(object sender, EventArgs e)
        {
            PositionLegendHost();
        }

        private void PositionLegendHost()
        {
            if (legendHost == null || legendBitmap == null || !legendHost.Visible || gmap == null) return;
            int marginTop = 10;
            int marginRight = 10 + legendRightMarginPx; // include scrollbar margin
            legendHost.Left = gmap.Width - legendHost.Width - marginRight;
            legendHost.Top = marginTop;
            legendHost.BringToFront();
        }
    }
}