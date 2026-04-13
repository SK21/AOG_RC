using GMap.NET;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.Strtree;
using NetTopologySuite.Operation.Union;
using RateController.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;

namespace RateController.RateMap
{
    /// <summary>
    /// Generates productivity management zones from one or more years of yield data.
    ///
    /// Algorithm:
    ///   1. Parse each yield CSV; build one IDW grid per year; average across years.
    ///   2. Quantile-classify the averaged grid into ZoneCount equal-area bands.
    ///   3. Apply 3×3 majority-filter smoothing.
    ///   4. Union same-zone cell rectangles into NTS polygons → MapZones → target layer.
    ///   Zone names include the average yield so the user knows what each zone represents.
    ///
    /// Future: additional layers (elevation, EC, NDVI) can be blended into a weighted
    /// productivity index before the quantile step.
    /// </summary>
    public static class ProductivityZoneCreator
    {
        private const int MaxCells = 1200;   // larger cells → more contiguous polygons

        public static int ZoneCount { get; set; } = 3;

        /// <summary>
        /// Generate productivity zones from the supplied yield file paths.
        /// Returns an empty string on success, or a user-readable error message.
        /// </summary>
        public static string Generate(List<string> yieldPaths)
        {
            try
            {
                if (yieldPaths == null || yieldPaths.Count == 0)
                    return "No yield files selected.";

                // ── Parse all yield files ────────────────────────────────────────
                var yearSets = new List<List<FieldSample>>();
                foreach (string path in yieldPaths)
                {
                    var samples = ParseYieldFile(path);
                    if (samples.Count >= 10) yearSets.Add(samples);
                }

                if (yearSets.Count == 0)
                    return "No usable yield data found in the selected files (need at least 10 points per file).";

                // Flatten all samples for bounds computation
                var allYield = yearSets.SelectMany(s => s).ToList();

                // ── Field bounds ─────────────────────────────────────────────────
                double minLat = allYield.Min(r => r.Latitude);
                double maxLat = allYield.Max(r => r.Latitude);
                double minLon = allYield.Min(r => r.Longitude);
                double maxLon = allYield.Max(r => r.Longitude);

                // ── Adaptive grid resolution ─────────────────────────────────────
                double midLat = (minLat + maxLat) / 2.0;
                double fieldH = Haversine(minLat, 0, maxLat, 0);
                double fieldW = Haversine(midLat, minLon, midLat, maxLon);
                double res    = Math.Max(1.0, Math.Sqrt(fieldH * fieldW / MaxCells));
                int    rows   = Math.Max(3, (int)(fieldH / res));
                int    cols   = Math.Max(3, (int)(fieldW / res));

                // ── Build averaged yield grid across all years ───────────────────
                double[,] yieldSum = new double[rows, cols];

                foreach (var yearSamples in yearSets)
                {
                    var tree     = BuildTree(yearSamples);
                    double fallback = yearSamples.Average(r => r.YieldKg);

                    for (int r = 0; r < rows; r++)
                    {
                        for (int c = 0; c < cols; c++)
                        {
                            double lat = minLat + (r + 0.5) / rows * (maxLat - minLat);
                            double lon = minLon + (c + 0.5) / cols * (maxLon - minLon);
                            yieldSum[r, c] += IDW(tree, lat, lon, fallback, s => s.YieldKg);
                        }
                    }
                }

                double[,] yieldGrid = new double[rows, cols];
                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                        yieldGrid[r, c] = yieldSum[r, c] / yearSets.Count;

                // ── Quantile classification (yield-only) ─────────────────────────
                // Collect all cell values, sort, then assign zones by rank so each
                // zone covers roughly equal field area regardless of value distribution.
                var flatValues = new List<double>(rows * cols);
                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                        flatValues.Add(yieldGrid[r, c]);
                flatValues.Sort();
                int total = flatValues.Count;

                int[,] zoneGrid = new int[rows, cols];
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        double v = yieldGrid[r, c];
                        int rank = flatValues.BinarySearch(v);
                        if (rank < 0) rank = ~rank;
                        rank = Math.Min(rank, total - 1);
                        zoneGrid[r, c] = Math.Min(ZoneCount - 1, rank * ZoneCount / total);
                    }
                }

                // ── 3×3 majority-filter smoothing ────────────────────────────────
                int[,] smoothed = MajorityFilter(zoneGrid, rows, cols);

                // ── Union cell rectangles per zone → NTS polygons ────────────────
                var factory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(4326);

                var cellsByZone = new List<List<Geometry>>(ZoneCount);
                for (int z = 0; z < ZoneCount; z++) cellsByZone.Add(new List<Geometry>());

                double latStep = (maxLat - minLat) / rows;
                double lonStep = (maxLon - minLon) / cols;

                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        int z = smoothed[r, c];

                        double latB = minLat + r * latStep;
                        double latT = latB + latStep;
                        double lonL = minLon + c * lonStep;
                        double lonR = lonL + lonStep;

                        var coords = new[]
                        {
                            new Coordinate(lonL, latB),
                            new Coordinate(lonR, latB),
                            new Coordinate(lonR, latT),
                            new Coordinate(lonL, latT),
                            new Coordinate(lonL, latB)
                        };
                        cellsByZone[z].Add(factory.CreatePolygon(coords));
                    }
                }

                // ── Compute per-zone average yield ───────────────────────────────
                var zoneYieldSum   = new double[ZoneCount];
                var zoneYieldCount = new int[ZoneCount];
                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                    {
                        int z = smoothed[r, c];
                        zoneYieldSum[z]   += yieldGrid[r, c];
                        zoneYieldCount[z] += 1;
                    }

                // ── Build MapZones ────────────────────────────────────────────────
                string yearLabel = yearSets.Count > 1
                    ? string.Format("{0} yrs", yearSets.Count)
                    : Path.GetFileNameWithoutExtension(yieldPaths[0]);

                // Productivity labels: z=0 is lowest, z=ZoneCount-1 is highest
                string[] prodLabels = ZoneCount <= 3
                    ? new[] { "Low", "Med", "High" }
                    : ZoneCount == 4
                        ? new[] { "Low", "Med-Low", "Med-High", "High" }
                        : new[] { "Very Low", "Low", "Medium", "High", "Very High" };

                var mapZones = new List<MapZone>();

                for (int z = 0; z < ZoneCount; z++)
                {
                    if (cellsByZone[z].Count == 0) continue;

                    Geometry merged = CascadedPolygonUnion.Union(cellsByZone[z]);
                    if (merged == null || merged.IsEmpty) continue;

                    Color  color = Palette.GetProductivityColor(z, ZoneCount);
                    string label = z < prodLabels.Length ? prodLabels[z] : (z + 1).ToString();
                    double avgYield = zoneYieldCount[z] > 0
                        ? zoneYieldSum[z] / zoneYieldCount[z]
                        : 0;
                    string name = string.Format("Auto {0} Z{1} {2} (avg {3:F0})",
                        yearLabel, z + 1, label, avgYield);
                    var    rates = new Dictionary<string, double>();
                    foreach (var key in ZoneFields.Products) rates[key] = 0.0;

                    if (merged is Polygon poly)
                    {
                        mapZones.Add(new MapZone(name, poly, rates, color, ZoneType.Target));
                    }
                    else if (merged is GeometryCollection gc)
                    {
                        int part = 1;
                        foreach (Geometry g in gc.Geometries)
                        {
                            if (g is Polygon p && !p.IsEmpty)
                                mapZones.Add(new MapZone(
                                    string.Format("{0} ({1})", name, part++),
                                    p, new Dictionary<string, double>(rates),
                                    color, ZoneType.Target));
                        }
                    }
                }

                // ── Filter small zones and sort for correct draw order ───────────
                // Large zones first (drawn first = underneath);
                // small zones last (drawn last = on top, wins for rate lookup).
                // Minimum 2 ha removes grid-artefact slivers and isolated patches.
                mapZones = mapZones
                    .Select(z => new { Zone = z, Ha = z.Hectares() })
                    .Where(x => x.Ha >= 2.0)
                    .OrderByDescending(x => x.Ha)
                    .Select(x => x.Zone)
                    .ToList();

                if (mapZones.Count == 0)
                    return "Zone generation produced no polygons. Try selecting more yield data files or a larger field area.";

                MapController.ZnOverlays.AddAutoZones(mapZones);
                return string.Empty;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ProductivityZoneCreator/Generate: " + ex.Message);
                return "Error generating zones: " + ex.Message;
            }
        }

        // ── CSV parsing ───────────────────────────────────────────────────────────

        private static List<FieldSample> ParseYieldFile(string path)
        {
            var result = new List<FieldSample>();
            try
            {
                if (!File.Exists(path)) return result;

                string[] lines = File.ReadAllLines(path);
                if (lines.Length < 2) return result;

                // Validate header
                if (!lines[0].StartsWith("Timestamp", StringComparison.OrdinalIgnoreCase))
                    return result;

                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split(',');
                    if (parts.Length < 5) continue;

                    if (!DateTime.TryParse(parts[0], CultureInfo.InvariantCulture,
                            DateTimeStyles.RoundtripKind, out DateTime ts)) continue;
                    if (!double.TryParse(parts[1], NumberStyles.Float,
                            CultureInfo.InvariantCulture, out double lat)) continue;
                    if (!double.TryParse(parts[2], NumberStyles.Float,
                            CultureInfo.InvariantCulture, out double lon)) continue;
                    if (!double.TryParse(parts[4], NumberStyles.Float,
                            CultureInfo.InvariantCulture, out double yield)) continue;
                    if (yield <= 0.01) continue;

                    result.Add(new FieldSample(ts, lat, lon, yield, 0));
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ProductivityZoneCreator/ParseYieldFile: " + ex.Message);
            }
            return result;
        }

        // ── Spatial helpers ───────────────────────────────────────────────────────

        private static STRtree<FieldSample> BuildTree(List<FieldSample> samples)
        {
            var tree = new STRtree<FieldSample>();
            foreach (var s in samples)
                tree.Insert(new Envelope(s.Longitude, s.Longitude, s.Latitude, s.Latitude), s);
            return tree;
        }

        private static double IDW(STRtree<FieldSample> tree,
            double lat, double lon, double fallback, Func<FieldSample, double> getValue)
        {
            var env = new Envelope(lon - 0.002, lon + 0.002, lat - 0.002, lat + 0.002);
            var pts = tree.Query(env);

            if (pts.Count == 0) return fallback;

            double vSum = 0, wSum = 0;
            foreach (var p in pts.Take(8))
            {
                double d = Haversine(lat, lon, p.Latitude, p.Longitude);
                double w = 1.0 / (d * d + 0.0001);
                vSum += w * getValue(p);
                wSum += w;
            }
            return vSum / wSum;
        }

        private static int[,] MajorityFilter(int[,] grid, int rows, int cols)
        {
            int[,] result = new int[rows, cols];
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    var counts = new Dictionary<int, int>();
                    for (int dr = -1; dr <= 1; dr++)
                    {
                        for (int dc = -1; dc <= 1; dc++)
                        {
                            int rr = r + dr;
                            int cc = c + dc;
                            if (rr < 0 || cc < 0 || rr >= rows || cc >= cols) continue;
                            int z = grid[rr, cc];
                            if (!counts.ContainsKey(z)) counts[z] = 0;
                            counts[z]++;
                        }
                    }
                    result[r, c] = counts.OrderByDescending(x => x.Value).First().Key;
                }
            }
            return result;
        }

        private static double Haversine(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000;
            double dLat = (lat2 - lat1) * Math.PI / 180;
            double dLon = (lon2 - lon1) * Math.PI / 180;
            double a    = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                        + Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180)
                        * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }
    }
}
