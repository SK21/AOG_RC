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

        /// <summary>
        /// Generate productivity zones from the supplied yield file paths.
        /// </summary>
        /// <param name="yieldPaths">Yield CSV files to use.</param>
        /// <param name="yieldWeights">Per-file weights, normalised to sum=1. Null = equal weights.</param>
        /// <param name="zoneCount">Number of productivity zones (2–5).</param>
        /// <param name="yieldFraction">Share of productivity index from yield (0–1). yieldFraction + ecFraction should = 1.</param>
        /// <param name="ecPath">EC CSV file path (Lat,Lon,EC format), or null to skip EC layer.</param>
        /// <param name="ecFraction">Share of productivity index from EC (0–1).</param>
        /// <param name="minZoneHa">Minimum polygon area in hectares; smaller fragments are discarded.</param>
        /// <returns>Empty string on success, or a user-readable error message.</returns>
        public static string Generate(
            List<string> yieldPaths,
            List<double> yieldWeights  = null,
            double       yieldFraction = 1.0,
            string       ecPath        = null,
            double       ecFraction    = 0.0,
            int          zoneCount     = 3,
            double       minZoneHa     = 0.5)
        {
            try
            {
                if (yieldPaths == null || yieldPaths.Count == 0)
                    return "No yield files selected.";

                zoneCount = Math.Max(2, Math.Min(5, zoneCount));

                // ── Parse all yield files ────────────────────────────────────────
                var yearSets = new List<List<FieldSample>>();
                foreach (string path in yieldPaths)
                {
                    var samples = ParseYieldFile(path);
                    if (samples.Count >= 10) yearSets.Add(samples);
                }

                if (yearSets.Count == 0)
                    return "No usable yield data found in the selected files (need at least 10 points per file).";

                // Normalise per-year weights to sum = 1.0 (equal weights if not supplied)
                double[] wArr = BuildWeights(yieldWeights, yearSets.Count);

                // Flatten all samples for bounds computation
                var allYield = yearSets.SelectMany(s => s).ToList();

                // ── Field bounds ─────────────────────────────────────────────────
                double minLat = allYield.Min(r => r.Latitude);
                double maxLat = allYield.Max(r => r.Latitude);
                double minLon = allYield.Min(r => r.Longitude);
                double maxLon = allYield.Max(r => r.Longitude);

                // ── Adaptive grid resolution ─────────────────────────────────────
                double midLat      = (minLat + maxLat) / 2.0;
                double fieldH      = Haversine(minLat, 0, maxLat, 0);
                double fieldW      = Haversine(midLat, minLon, midLat, maxLon);
                double fieldAreaHa = fieldH * fieldW / 10000.0;
                double res         = Math.Max(1.0, Math.Sqrt(fieldH * fieldW / MaxCells));
                int    rows        = Math.Max(3, (int)(fieldH / res));
                int    cols        = Math.Max(3, (int)(fieldW / res));

                // ── Minimum zone area ────────────────────────────────────────────
                // Floor is the user's setting or 1/20 of expected zone size,
                // whichever is larger.  Tying to expected zone size (fieldArea/zoneCount)
                // rather than total field area avoids over-filtering on small fields.
                double effectiveMinHa = Math.Max(minZoneHa, fieldAreaHa / (zoneCount * 20.0));

                // ── Build weighted yield grid across all years ───────────────────
                // wArr is normalised (sum=1), so yieldSum IS already the weighted average.
                double[,] yieldGrid = new double[rows, cols];

                for (int yi = 0; yi < yearSets.Count; yi++)
                {
                    var    yearSamples = yearSets[yi];
                    var    tree        = BuildTree(yearSamples);
                    double fallback    = yearSamples.Average(r => r.YieldKg);
                    double w           = wArr[yi];

                    for (int r = 0; r < rows; r++)
                    {
                        for (int c = 0; c < cols; c++)
                        {
                            double lat = minLat + (r + 0.5) / rows * (maxLat - minLat);
                            double lon = minLon + (c + 0.5) / cols * (maxLon - minLon);
                            yieldGrid[r, c] += w * IDW(tree, lat, lon, fallback, s => s.YieldKg);
                        }
                    }
                }

                // ── Optional EC layer blend ──────────────────────────────────────
                // When EC data is supplied, normalise both grids to [0,1] and blend
                // them into a combined productivity index for classification.
                // yieldGrid is kept un-blended for per-zone average yield naming.
                double[,] classifyGrid = yieldGrid;

                if (!string.IsNullOrEmpty(ecPath) && ecFraction > 0.0)
                {
                    var ecSamples = ParseEcFile(ecPath);
                    if (ecSamples.Count >= 3)
                    {
                        var    ecTree    = BuildTree(ecSamples);
                        double ecFallback = ecSamples.Average(s => s.EcValue);

                        double[,] ecGrid = new double[rows, cols];
                        for (int r = 0; r < rows; r++)
                            for (int c = 0; c < cols; c++)
                            {
                                double lat = minLat + (r + 0.5) / rows * (maxLat - minLat);
                                double lon = minLon + (c + 0.5) / cols * (maxLon - minLon);
                                ecGrid[r, c] = IDW(ecTree, lat, lon, ecFallback, s => s.EcValue);
                            }

                        double[,] normYield = Normalize(yieldGrid, rows, cols);
                        double[,] normEc    = Normalize(ecGrid,    rows, cols);

                        classifyGrid = new double[rows, cols];
                        for (int r = 0; r < rows; r++)
                            for (int c = 0; c < cols; c++)
                                classifyGrid[r, c] = yieldFraction * normYield[r, c]
                                                   + ecFraction    * normEc[r, c];
                    }
                }

                // ── Pre-classification smoothing ─────────────────────────────────
                // Box-blur the productivity grid before quantile classification.
                // Reduces noise so zone boundaries are spatially coherent.
                // Without this, noisy IDW output produces hundreds of tiny fragments
                // that the post-classification majority filter cannot fully merge.
                classifyGrid = BoxBlur(classifyGrid, rows, cols);
                classifyGrid = BoxBlur(classifyGrid, rows, cols);
                classifyGrid = BoxBlur(classifyGrid, rows, cols);

                // ── Quantile classification ──────────────────────────────────────
                // Sort all cell values, assign zones by rank so each zone covers
                // roughly equal field area regardless of value distribution.
                var flatValues = new List<double>(rows * cols);
                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                        flatValues.Add(classifyGrid[r, c]);
                flatValues.Sort();
                int total = flatValues.Count;

                int[,] zoneGrid = new int[rows, cols];
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        double v = classifyGrid[r, c];
                        int rank = flatValues.BinarySearch(v);
                        if (rank < 0) rank = ~rank;
                        rank = Math.Min(rank, total - 1);
                        zoneGrid[r, c] = Math.Min(zoneCount - 1, rank * zoneCount / total);
                    }
                }

                // ── 3×3 majority-filter smoothing (convergence) ─────────────────
                // Run until the grid stops changing (converged) or 30 passes max.
                int[,] smoothed = zoneGrid;
                for (int pass = 0; pass < 30; pass++)
                {
                    int[,] next = MajorityFilter(smoothed, rows, cols);
                    if (GridsEqual(smoothed, next, rows, cols)) break;
                    smoothed = next;
                }

                // ── Sieve: merge small fragments into neighbouring zones ──────────
                // Any connected component smaller than 1/10 of expected zone size
                // is absorbed by its most-common neighbour.  Guarantees full field
                // coverage with no tiny isolated patches.
                int minCells = Math.Max(1, (rows * cols) / (zoneCount * 10));
                smoothed = SieveSmallRegions(smoothed, rows, cols, minCells);

                // ── Union cell rectangles per zone → NTS polygons ────────────────
                var factory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(4326);

                var cellsByZone = new List<List<Geometry>>(zoneCount);
                for (int z = 0; z < zoneCount; z++) cellsByZone.Add(new List<Geometry>());

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
                var zoneYieldSum   = new double[zoneCount];
                var zoneYieldCount = new int[zoneCount];
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

                // Productivity labels: z=0 is lowest, z=zoneCount-1 is highest
                string[] prodLabels = zoneCount <= 3
                    ? new[] { "Low", "Med", "High" }
                    : zoneCount == 4
                        ? new[] { "Low", "Med-Low", "Med-High", "High" }
                        : new[] { "Very Low", "Low", "Medium", "High", "Very High" };

                var mapZones = new List<MapZone>();

                for (int z = 0; z < zoneCount; z++)
                {
                    if (cellsByZone[z].Count == 0) continue;

                    Geometry merged = CascadedPolygonUnion.Union(cellsByZone[z]);
                    if (merged == null || merged.IsEmpty) continue;

                    Color  color = Palette.GetProductivityColor(z, zoneCount);
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
                mapZones = mapZones
                    .Select(z => new { Zone = z, Ha = z.Hectares() })
                    .Where(x => x.Ha >= effectiveMinHa)
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

        // ── Grid normalisation ────────────────────────────────────────────────────

        /// <summary>Scales all values in the grid to [0, 1].</summary>
        private static double[,] Normalize(double[,] grid, int rows, int cols)
        {
            double min = double.MaxValue, max = double.MinValue;
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                {
                    if (grid[r, c] < min) min = grid[r, c];
                    if (grid[r, c] > max) max = grid[r, c];
                }

            double[,] result = new double[rows, cols];
            double range = max - min;
            if (range < 1e-9)
            {
                // Flat field — return all 0.5 so it contributes a neutral signal
                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                        result[r, c] = 0.5;
            }
            else
            {
                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                        result[r, c] = (grid[r, c] - min) / range;
            }
            return result;
        }

        // ── Weight helper ─────────────────────────────────────────────────────────

        private static double[] BuildWeights(List<double> weights, int count)
        {
            if (weights != null && weights.Count == count)
            {
                double total = weights.Sum();
                if (total > 0)
                    return weights.Select(w => w / total).ToArray();
            }
            double eq = 1.0 / count;
            return Enumerable.Repeat(eq, count).ToArray();
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

        // ── EC CSV parsing (Lat,Lon,EC — header line skipped) ────────────────────

        private static List<FieldSample> ParseEcFile(string path)
        {
            var result = new List<FieldSample>();
            try
            {
                if (!File.Exists(path)) return result;
                string[] lines = File.ReadAllLines(path);
                for (int i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i])) continue;
                    string[] parts = lines[i].Split(',');
                    if (parts.Length < 3) continue;
                    if (!double.TryParse(parts[0], NumberStyles.Float,
                            CultureInfo.InvariantCulture, out double lat)) continue;
                    if (!double.TryParse(parts[1], NumberStyles.Float,
                            CultureInfo.InvariantCulture, out double lon)) continue;
                    if (!double.TryParse(parts[2], NumberStyles.Float,
                            CultureInfo.InvariantCulture, out double ec))  continue;
                    if (ec <= 0.0) continue;
                    result.Add(new FieldSample(DateTime.MinValue, lat, lon, 0, 0, 0, ec));
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ProductivityZoneCreator/ParseEcFile: " + ex.Message);
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

        private static double[,] BoxBlur(double[,] grid, int rows, int cols)
        {
            double[,] result = new double[rows, cols];
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    double sum = 0; int count = 0;
                    for (int dr = -1; dr <= 1; dr++)
                        for (int dc = -1; dc <= 1; dc++)
                        {
                            int rr = r + dr, cc = c + dc;
                            if (rr >= 0 && rr < rows && cc >= 0 && cc < cols)
                            { sum += grid[rr, cc]; count++; }
                        }
                    result[r, c] = sum / count;
                }
            }
            return result;
        }

        private static int[,] SieveSmallRegions(int[,] grid, int rows, int cols, int minCells)
        {
            int[,] result = (int[,])grid.Clone();
            bool anyMerged = true;
            while (anyMerged)
            {
                anyMerged = false;
                bool[,] visited = new bool[rows, cols];
                for (int r0 = 0; r0 < rows; r0++)
                {
                    for (int c0 = 0; c0 < cols; c0++)
                    {
                        if (visited[r0, c0]) continue;
                        int zoneId = result[r0, c0];

                        // BFS — find the full connected component
                        var cells = new List<(int r, int c)>();
                        var queue = new Queue<(int, int)>();
                        queue.Enqueue((r0, c0));
                        visited[r0, c0] = true;
                        while (queue.Count > 0)
                        {
                            var (r, c) = queue.Dequeue();
                            cells.Add((r, c));
                            foreach (var (nr, nc) in new[] { (r-1,c),(r+1,c),(r,c-1),(r,c+1) })
                            {
                                if (nr >= 0 && nr < rows && nc >= 0 && nc < cols
                                    && !visited[nr, nc] && result[nr, nc] == zoneId)
                                { visited[nr, nc] = true; queue.Enqueue((nr, nc)); }
                            }
                        }

                        if (cells.Count >= minCells) continue;

                        // Small fragment — find the dominant neighbouring zone
                        var neighborCounts = new Dictionary<int, int>();
                        foreach (var (r, c) in cells)
                            foreach (var (nr, nc) in new[] { (r-1,c),(r+1,c),(r,c-1),(r,c+1) })
                            {
                                if (nr >= 0 && nr < rows && nc >= 0 && nc < cols && result[nr, nc] != zoneId)
                                {
                                    int nz = result[nr, nc];
                                    neighborCounts[nz] = neighborCounts.ContainsKey(nz) ? neighborCounts[nz] + 1 : 1;
                                }
                            }

                        if (neighborCounts.Count == 0) continue;
                        int target = neighborCounts.OrderByDescending(kv => kv.Value).First().Key;
                        foreach (var (r, c) in cells) result[r, c] = target;
                        anyMerged = true;
                    }
                }
            }
            return result;
        }

        private static bool GridsEqual(int[,] a, int[,] b, int rows, int cols)
        {
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    if (a[r, c] != b[r, c]) return false;
            return true;
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
