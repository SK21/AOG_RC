using GMap.NET.WindowsForms;
using RateController.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace RateController.RateMap
{
    public static class FixedRateBands
    {
        // Ratios relative to target
        // 5 bands total
        public static readonly double[] Thresholds =
        {
            0.70,   // very low
            0.90,   // low
            1.10,   // on target
            1.30,   // high
                // >1.30 remainder 5th band
        };
    }

    public class Band
    {
        public string ColorHtml { get; set; }   // e.g. "#FF0000"
        public double Max { get; set; }
        public double Min { get; set; }
        public int ProductIndex { get; set; }   // 0 = ProductA, 1 = ProductB, ...
        public string ProductName { get; set; } // Store the product name used when the legend was saved
        public double TargetRate { get; set; }  // target rate used to determine band
    }

    /// <summary>
    /// Manages the legend rendering and layout for the map view.
    /// Renders a fixed-position legend using a PictureBox hosted on the GMap control.
    /// </summary>
    public class LegendManager : IDisposable
    {
        private readonly GMapControl gmap;
        private LegendObject cCurrentLegend;
        private bool cIsVisible;
        private bool cIsYieldData = false;
        private bool cLegendChanged = false;
        private int CurrentProduct = 0;
        private double LegendBaseValue = 0;
        private Bitmap legendBitmap;
        private Font legendFont;
        private PictureBox legendHost;
        private List<Band> LoadedBands;

        public LegendManager(GMapControl gmap, bool IsYield = false)
        {
            cIsYieldData = IsYield;
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

            JobManager.JobChanged += JobManager_JobChanged;

            Reset();

            //string Name = "";
            //if (cIsYieldData)
            //{
            //    if (Props.UseMetric)
            //    {
            //        Name = "T/Ha";
            //    }
            //    else
            //    {
            //        Name = "Bus/ac";
            //    }
            //}
            //else
            //{
            //    Name = Core.Products.Item(MapController.ProductFilter).ProductName;
            //}

            //cCurrentLegend = new LegendObject
            //{
            //    Legend = new Dictionary<string, Color>(),
            //    Title = Name
            //};
            //UpdateTargetRates();
        }

        public event EventHandler LegendUpdated;

        public bool IsVisible
        { get { return cIsVisible; } }

        public bool LegendChanged
        { get { return cLegendChanged; } }

        public int BandIndex(double value)
        {
            int Result = 0;
            if (LegendBaseValue > 0)
            {
                Result = 4;
                double ratio = value / LegendBaseValue;
                var t = FixedRateBands.Thresholds;

                for (int i = 0; i < t.Length; i++)
                {
                    if (ratio < t[i])
                    {
                        Result = i;
                        break;
                    }
                }
            }
            return Result;
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

                JobManager.JobChanged -= JobManager_JobChanged;
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

        public void Hide()
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
                cIsVisible = false;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("LegendManager/Clear: " + ex.Message);
            }
        }

        public void OnMapZoomChanged()
        {
            PositionLegendHost();
        }

        public void Reset()
        {
            Load();
            UpdateLBV();
        }

        public void ResetLegendChanged()
        {
            cLegendChanged = false;
        }

        public void Show()
        {
            if (cCurrentLegend == null || cCurrentLegend.Bands == null || cCurrentLegend.Bands.Count == 0
                || MapController.CurrentMapState == MapState.Preview)
            {
                Hide();
                return;
            }

            const int itemHeight = 25;
            const int leftMargin = 10;
            const int swatch = 20;
            const int gap = 10;
            const int rightMargin = 10;
            const int dashGap = 6;

            // Create underline font for title
            Font underlineFont = null;

            try
            {
                underlineFont = new Font(legendFont, legendFont.Style | FontStyle.Underline);

                // Measure max widths of left and right values and title
                float maxLeftWidth = 0;
                float maxRightWidth = 0;
                float dashWidth;
                float titleWidth = 0;
                float titleHeight = 0;

                using (var bmp = new Bitmap(1, 1))
                using (var g = Graphics.FromImage(bmp))
                {
                    dashWidth = g.MeasureString("-", legendFont).Width;

                    int count = 0;
                    foreach (var kv in cCurrentLegend.Bands)
                    {
                        var parts = kv.Key.Split('-');
                        if (parts.Length != 2)
                        {
                            continue;
                        }

                        var left = parts[0].Trim();
                        var right = parts[1].Trim();
                        if (count == 0) left = "0";
                        if (count == (cCurrentLegend.Bands.Count - 1)) right = "";
                        count++;

                        maxLeftWidth = Math.Max(maxLeftWidth, g.MeasureString(left, legendFont).Width);
                        maxRightWidth = Math.Max(maxRightWidth, g.MeasureString(right, legendFont).Width);
                    }

                    if (cCurrentLegend.Title != null)
                    {
                        var titleSize = g.MeasureString(cCurrentLegend.Title, underlineFont);
                        titleWidth = titleSize.Width;
                        titleHeight = titleSize.Height;
                    }
                }

                int textBlockWidth = (int)Math.Ceiling(maxLeftWidth + dashGap + dashWidth + dashGap + maxRightWidth);

                int contentWidth = swatch + gap + textBlockWidth;
                int legendItemsHeight = cCurrentLegend.Bands.Count * itemHeight;
                int titlePadding = 8; // vertical space between top and title / title and first item
                int legendHeight = (int)Math.Ceiling(titleHeight) + (titlePadding * 2) + legendItemsHeight + (leftMargin * 2);
                int legendWidth = Math.Max(120, (int)Math.Ceiling(Math.Max(titleWidth, leftMargin + contentWidth + rightMargin)));

                legendBitmap?.Dispose();
                legendBitmap = new Bitmap(legendWidth, legendHeight);

                using (var g2 = Graphics.FromImage(legendBitmap))
                using (var backBrush = new SolidBrush(Color.Black))
                {
                    g2.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    g2.FillRectangle(backBrush, 0, 0, legendWidth, legendHeight);

                    // Center the swatch + text block inside the legend
                    int anchorStartX = Math.Max(leftMargin, (legendWidth - contentWidth) / 2);

                    float titleY = leftMargin;
                    int y;

                    if (cCurrentLegend.Title != null)
                    {
                        var titleSize = g2.MeasureString(cCurrentLegend.Title, underlineFont);
                        float titleX = (legendWidth - titleSize.Width) / 2f;
                        g2.DrawString(cCurrentLegend.Title, underlineFont, Brushes.White, new PointF(titleX, titleY));

                        y = (int)(titleY + titleSize.Height + titlePadding);
                    }
                    else
                    {
                        // No title => start items just below margin
                        y = leftMargin;
                    }

                    int count = 0;
                    foreach (var kv in cCurrentLegend.Bands)
                    {
                        Color color = kv.Value;
                        int swatchTop = y + (itemHeight - swatch) / 2;

                        using (var brush = new SolidBrush(color))
                        {
                            g2.FillRectangle(brush, anchorStartX, swatchTop, swatch, swatch);
                            g2.DrawRectangle(Pens.White, anchorStartX, swatchTop, swatch, swatch);
                        }

                        var parts = kv.Key.Split('-');
                        string left = parts.Length > 0 ? parts[0].Trim() : string.Empty;
                        string right = parts.Length > 1 ? parts[1].Trim() : string.Empty;
                        if (count == 0) left = "0";
                        if (count == (cCurrentLegend.Bands.Count - 1)) right = "";
                        count++;

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
                cIsVisible = true;
                EnsureLegendTop();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("LegendManager/UpdateLegend: " + ex.Message);
            }
            finally
            {
                if (underlineFont != null)
                {
                    underlineFont.Dispose();
                }
            }
        }

        public void UpdateLBV()
        {
            if (!cIsYieldData)
            {
                double CurrentTarget = LegendBaseValue;

                clsProduct Prd = Core.Products.Item(MapController.ProductFilter);
                if (Prd.ProductOn(false))
                {
                    // product is being applied update legend target based on current rate
                    CurrentTarget = Prd.TargetRate(true);
                }

                bool Changed = ((LegendBaseValue != CurrentTarget) || (MapController.ProductFilter != CurrentProduct));

                if (Changed)
                {
                    CurrentProduct = MapController.ProductFilter;
                    LegendBaseValue = CurrentTarget;
                    string title = Prd.ProductName;
                    CreateLegend(LegendBaseValue, title);
                    LegendUpdated?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void CreateLegend(double BaseValue, string NewTitle)
        {
            try
            {
                var LegendBands = new Dictionary<string, Color>();

                // create bands
                if (BaseValue < 0.1) BaseValue = 10;  // placeholder for 0 target rate
                bool LargeNumbers = (BaseValue >= 1000);

                double[] Band = new double[4];
                for (int i = 0; i < 4; i++)
                {
                    Band[i] = FixedRateBands.Thresholds[i] * BaseValue;
                }

                // Determine formatting and increment rules based on magnitude
                string format = LargeNumbers ? "N0" : "N1";
                double increment = LargeNumbers ? 1.0 : 0.1;

                double min = 0;
                double max = 0;
                string mn = "0";
                string mx;
                for (int i = 0; i < 5; i++)
                {
                    if (i == 4)
                    {
                        mx = " ";
                    }
                    else
                    {
                        max = Math.Round((Band[i] - increment), 3);
                        mx = max.ToString(format, CultureInfo.CurrentCulture);
                    }

                    var color = Palette.GetColor(i, cIsYieldData, 255);
                    LegendBands.Add(string.Format(CultureInfo.CurrentCulture, "{0} - {1}", mn, mx), color);

                    if (i < 4)
                    {
                        min = Math.Round(Band[i], 3);
                        mn = min.ToString(format, CultureInfo.CurrentCulture);
                    }
                }

                cCurrentLegend = new LegendObject
                {
                    Bands = LegendBands,
                    Title = NewTitle
                };

                Save();
                if (cIsVisible) Show();  // update legend display
                cLegendChanged = true;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("LegendManager/CreateLegend: " + ex.Message);
            }
        }

        private void Gmap_SizeChanged(object sender, EventArgs e)
        {
            PositionLegendHost();
        }

        private void JobManager_JobChanged(object sender, EventArgs e)
        {
            Load();
        }

        private string LegendPath()
        {
            string Result = "";
            var basePath = Path.ChangeExtension(JobManager.CurrentMapPath, null); // strip .shp
            if (cIsYieldData)
            {
                Result = basePath + "_YieldLegend.json";
            }
            else
            {
                Result = basePath + "_AppliedLegend.json";
            }
            return Result;
        }

        private void Load()
        {
            try
            {
                bool Loaded = false;
                var CurrentBands = new Dictionary<string, Color>();
                string path = LegendPath();

                if (File.Exists(path))
                {
                    var json = File.ReadAllText(path);
                    LoadedBands = JsonConvert.DeserializeObject<List<Band>>(json);
                    if (LoadedBands != null && LoadedBands.Count > 0)
                    {
                        // Filter to current product
                        var filtered = LoadedBands
                                    .Where(b => b.ProductIndex == MapController.ProductFilter)
                                    .OrderBy(b => b.Min)
                                    .ToList();
                        if (filtered.Count > 0 && filtered.Count < 6)
                        {
                            // Determine formatting rules based on magnitude
                            double Max = filtered.Max(b => b.Max);
                            bool LargeNumbers = Max >= 1000;
                            string format = LargeNumbers ? "N0" : "N1";
                            string LegendTitle = filtered.FirstOrDefault().ProductName;
                            LegendBaseValue = filtered.FirstOrDefault().TargetRate;

                            for (int i = 0; i < filtered.Count; i++)
                            {
                                var Bnd = filtered[i];

                                string label = string.Format(CultureInfo.CurrentCulture, "{0} - {1}",
                                    Bnd.Min.ToString(format, CultureInfo.CurrentCulture),
                                    Bnd.Max.ToString(format, CultureInfo.CurrentCulture));

                                CurrentBands[label] = ColorTranslator.FromHtml(Bnd.ColorHtml);
                            }

                            cCurrentLegend = new LegendObject
                            {
                                Bands = CurrentBands,
                                Title = LegendTitle
                            };
                            Loaded = true;
                        }
                    }
                }

                if (!Loaded)
                {
                    if (cIsYieldData)
                    {
                        // todo create legend from yield data
                        cCurrentLegend = null;
                    }
                    else
                    {
                        LegendBaseValue = Core.Products.Item(MapController.ProductFilter).TargetRate(true);
                        string Title = Core.Products.Item(MapController.ProductFilter).ProductName;
                        CreateLegend(LegendBaseValue, Title);
                    }
                }
                LegendUpdated?.Invoke(this, EventArgs.Empty);
                if (cIsVisible) Show(); // update legend display
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("LegendManager/Load: " + ex.Message);
            }
        }

        private void PositionLegendHost()
        {
            if (legendHost == null || legendBitmap == null || !legendHost.Visible || gmap == null) return;

            int marginTop = 10;
            int marginRight = 10;

            // Default: applied data legend at top-right
            int rightOffset = 0;

            // If this instance is configured for yield, shift it left by its own width + a small gap
            if (cIsYieldData)
            {
                // 10px gap between yield (left) and applied (right)
                rightOffset = legendHost.Width + 10;
            }

            legendHost.Left = gmap.Width - legendHost.Width - marginRight - rightOffset;
            legendHost.Top = marginTop;
            legendHost.BringToFront();
        }

        private void Save()
        {
            try
            {
                if (LoadedBands == null) LoadedBands = new List<Band>();

                foreach (var kvp in cCurrentLegend.Bands)
                {
                    var parts = kvp.Key.Split('-');
                    if (parts.Length != 2) continue;

                    string leftText = parts[0].Trim();
                    string rightText = parts[1].Trim();

                    double min = double.TryParse(leftText, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out double mn) ? mn : 0;
                    double max = double.TryParse(rightText, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out double mx) ? mx : 0;

                    var colorHtml = ColorTranslator.ToHtml(kvp.Value);

                    var existing = LoadedBands.FirstOrDefault(b =>
                        b.ProductIndex == MapController.ProductFilter &&
                        Math.Abs(b.Min - min) < 0.0001 &&
                        Math.Abs(b.Max - max) < 0.0001);

                    if (existing == null)
                    {
                        LoadedBands.Add(new Band
                        {
                            Min = min,
                            Max = max,
                            ColorHtml = colorHtml,
                            ProductIndex = MapController.ProductFilter,
                            ProductName = cCurrentLegend.Title,
                            TargetRate = LegendBaseValue
                        });
                    }
                    else
                    {
                        // Update existing band
                        existing.ColorHtml = colorHtml;
                        existing.ProductName = cCurrentLegend.Title;
                        existing.TargetRate = LegendBaseValue;
                    }
                }

                if (LoadedBands.Count > 0)
                {
                    var json = JsonConvert.SerializeObject(LoadedBands, Formatting.Indented);
                    File.WriteAllText(LegendPath(), json);
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("LegendManager/Save: " + ex.Message);
            }
        }
    }

    public class LegendObject
    {
        public Dictionary<string, Color> Bands { get; set; }
        public string Title { get; set; }
    }
}