using GMap.NET.WindowsForms;
using RateController.RateMap;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        public static Dictionary<string, Color> BuildAppliedZonesLegend(List<MapZone> zones, int ProductFilter = 0)
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

        public static Dictionary<string, Color> CreateAppliedLegend(double minRate, double maxRate, int steps = 5)
        {
            var legend = new Dictionary<string, Color>();

            if (minRate < maxRate)
            {
                double band = (maxRate - minRate) / steps;
                for (int i = 0; i < steps; i++)
                {
                    double a = minRate + (i * band);
                    double b = (i == steps - 1) ? maxRate : minRate + ((i + 1) * band);
                    var color = Palette.Colors[i % Palette.Colors.Length];
                    legend.Add(string.Format("{0:N1} - {1:N1}", a, b), color);
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

                const int itemHeight = 25;
                const int leftMargin = 10;
                const int swatch = 20;
                const int gap = 10;
                const int rightMargin = 10;

                // Measure max text width
                int maxTextWidth = 0;
                using (var bmp = new Bitmap(1, 1))
                using (var g = Graphics.FromImage(bmp))
                {
                    foreach (var kv in legend)
                    {
                        var size = g.MeasureString(kv.Key, legendFont);
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

                    foreach (var kv in legend)
                    {
                        Color color = kv.Value;
                        var textSize = g2.MeasureString(kv.Key, legendFont);
                        int swatchTop = y + (itemHeight - swatch) / 2;

                        using (var brush = new SolidBrush(color))
                        {
                            g2.FillRectangle(brush, anchorStartX, swatchTop, swatch, swatch);
                            g2.DrawRectangle(Pens.White, anchorStartX, swatchTop, swatch, swatch);
                        }

                        float textY = y + (itemHeight - textSize.Height) / 2f;
                        g2.DrawString(kv.Key, legendFont, Brushes.White,
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