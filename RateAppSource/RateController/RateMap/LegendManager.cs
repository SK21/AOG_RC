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
    public class LegendManager : IDisposable
    {

        private readonly GMapControl gmap;
        private bool cEnabled;
        private Dictionary<string, Color> lastLegend;
        private Bitmap legendBitmap;
        private Font legendFont;
        private PictureBox legendHost;
        private int legendRightMarginPx = 0;

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

        public static Dictionary<string, Color> CreateAppliedLegend(List<MapZone> zones)
        {
            if (zones == null)
                return null;

            var appliedZones = zones
                .Where(z => z.Name.StartsWith("Applied Zone", StringComparison.OrdinalIgnoreCase))
                .Where(z => z.Rates != null && z.Rates.ContainsKey("ProductA"))
                .ToList();

            if (appliedZones.Count == 0)
                return null;

            var legend = new Dictionary<string, Color>();

            var groups = appliedZones
                .GroupBy(z => new
                {
                    Rate = z.Rates["ProductA"],
                    z.ZoneColor
                })
                .OrderBy(g => g.Key.Rate);

            foreach (var group in groups)
            {
                double rate = group.Key.Rate;

                if (double.IsNaN(rate) || double.IsInfinity(rate))
                    continue;

                string label = rate.ToString("N1");

                // One entry per rate/color
                legend[label] = group.Key.ZoneColor;
            }

            return legend.Count > 0 ? legend : null;
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

        public void OnMapZoomChanged()
        {
            PositionLegendHost();
        }

        public void UpdateLegend(Dictionary<string, Color> legend)
        {
            if (!cEnabled || legend == null || legend.Count == 0)
            {
                Clear();
            }
            else
            {
                lastLegend = legend;

                double Val = 0;
                SortedDictionary<double, Color> SortedLegend = new SortedDictionary<double, Color>();
                foreach (var kv in legend)
                {
                    Val = double.TryParse(kv.Key, out Val) ? Val : 0;
                    if (Val > 0) SortedLegend.Add(Val, kv.Value);
                }

                const int itemHeight = 25;
                const int leftMargin = 10;
                const int swatch = 20;
                const int gap = 10;
                const int rightMargin = 10;

                // Measure max text width
                int maxTextWidth = 0;
                string Label = "";
                using (var bmp = new Bitmap(1, 1))
                using (var g = Graphics.FromImage(bmp))
                {
                    foreach (var kv in SortedLegend)
                    {
                        Label = kv.Key.ToString("N1");
                        var size = g.MeasureString(Label, legendFont);
                        maxTextWidth = Math.Max(maxTextWidth, (int)Math.Ceiling(size.Width));
                    }
                }

                int maxContentWidth = swatch + gap + maxTextWidth;
                int legendHeight = (legend.Count * itemHeight) + (leftMargin * 2);
                int legendWidth = Math.Max(120, leftMargin + maxContentWidth + rightMargin);

                legendBitmap?.Dispose();
                legendBitmap = new Bitmap(legendWidth, legendHeight);

                using (var g2 = Graphics.FromImage(legendBitmap))
                using (var backBrush = new SolidBrush(Color.Black))
                {
                    g2.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    g2.FillRectangle(backBrush, 0, 0, legendWidth, legendHeight);

                    int anchorStartX = Math.Max(leftMargin, (legendWidth - maxContentWidth) / 2);
                    int y = leftMargin;

                    foreach (var kv in SortedLegend)
                    {
                        Label = kv.Key.ToString("N1");
                        Color color = kv.Value;
                        var textSize = g2.MeasureString(Label, legendFont);
                        int swatchTop = y + (itemHeight - swatch) / 2;

                        using (var brush = new SolidBrush(color))
                        {
                            g2.FillRectangle(brush, anchorStartX, swatchTop, swatch, swatch);
                            g2.DrawRectangle(Pens.White, anchorStartX, swatchTop, swatch, swatch);
                        }

                        float textY = y + (itemHeight - textSize.Height) / 2f;
                        g2.DrawString(Label, legendFont, Brushes.White,
                            new PointF(anchorStartX + swatch + gap, textY));

                        y += itemHeight;
                    }
                }

                legendHost.Image = legendBitmap;
                legendHost.Visible = true;
                PositionLegendHost();
            }
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