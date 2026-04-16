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
    /// Generates productivity management zones from yield data, EC data, elevation data,
    /// or any combination. At least one data source must be provided.
    ///
    /// Algorithm:
    ///   1. Parse each supplied data source; build one IDW grid per year/layer.
    ///   2. Percentile-normalise each grid to [0,1] by rank (robust to outliers).
    ///   3. Blend normalised grids by user-supplied fractions into a productivity index.
    ///   4. Quantile-classify into ZoneCount equal-area bands.
    ///   5. Apply 3×3 majority-filter smoothing and sieve.
    ///   6. Union same-zone cell rectangles into NTS polygons; optionally clip to
    ///      the supplied field boundary polygon → MapZones → target layer.
    /// </summary>
    public static class ProductivityZoneCreator
    {
        private const int MaxCells = 1200;   // larger cells → more contiguous polygons

        /// <summary>
        /// Generate productivity zones from yield files, an EC file, an elevation file, or any combination.
        /// At least one of <paramref name="yieldPaths"/>, <paramref name="ecPath"/>, or
        /// <paramref name="elevationPath"/> must be provided.
        /// </summary>
        /// <param name="yieldPaths">Yield CSV files to use (may be null/empty for EC/elevation-only).</param>
        /// <param name="yieldWeights">Per-file weights, normalised to sum=1. Null = equal weights.</param>
        /// <param name="zoneCount">Number of productivity zones (2–5).</param>
        /// <param name="yieldFraction">Share of productivity index from yield (0–1).</param>
        /// <param name="ecPath">EC CSV file path (Lat,Lon,EC format), or null to skip EC layer.</param>
        /// <param name="ecFraction">Share of productivity index from EC (0–1).</param>
        /// <param name="elevationPath">Elevation CSV file path (Lat,Lon,Elevation format), or null to skip.</param>
        /// <param name="elevationFraction">Share of productivity index from elevation (0–1).</param>
        /// <param name="minZoneHa">Minimum polygon area in hectares; smaller fragments are discarded.</param>
        /// <param name="boundaryKmlPath">Full path to a boundary KML file for clipping, or null for bounding-box behaviour.</param>
        /// <returns>Empty string on success, or a user-readable error message.</returns>
        public static string Generate(
            List<string> yieldPaths,
            List<double> yieldWeights     = null,
            double       yieldFraction    = 1.0,
            string       ecPath           = null,
            double       ecFraction       = 0.0,
            string       elevationPath    = null,
            double       elevationFraction = 0.0,
            int          zoneCount        = 3,
            double       minZoneHa        = 0.5,
            string       boundaryKmlPath  = null)
        {
            try
            {
                bool hasYield     = yieldPaths != null && yieldPaths.Count > 0;
                bool hasEc        = !string.IsNullOrEmpty(ecPath);
                bool hasElevation = !string.IsNullOrEmpty(elevationPath);
                if (!hasYield && !hasEc && !hasElevation)
                    return "No data files selected. Select at least one yield, EC, or elevation file.";

                zoneCount = Math.Max(2, Math.Min(5, zoneCount));

                // ── Parse yield files (if provided) ──────────────────────────────
                var yearSets = new List<List<FieldSample>>();
                if (hasYield)
                {
                    foreach (string path in yieldPaths)
                    {
                        var samples = ParseYieldFile(path);
                        if (samples.Count >= 10) yearSets.Add(samples);
                    }
                    if (yearSets.Count == 0)
                        return "No usable yield data found in the selected files (need at least 10 points per file).";
                }

                // Normalise per-year weights to sum = 1.0 (equal weights if not supplied)
                double[] wArr = hasYield ? BuildWeights(yieldWeights, yearSets.Count) : Array.Empty<double>();

                // ── Field bounds ─────────────────────────────────────────────────
                // Derived from yield samples when available; otherwise from the first
                // non-yield layer that was parsed (EC or elevation).
                double minLat, maxLat, minLon, maxLon;
                List<FieldSample> ecSamplesCache        = null;
                List<FieldSample> elevationSamplesCache = null;

                if (hasYield)
                {
                    var allYield = yearSets.SelectMany(s => s).ToList();
                    minLat = allYield.Min(r => r.Latitude);  maxLat = allYield.Max(r => r.Latitude);
                    minLon = allYield.Min(r => r.Longitude); maxLon = allYield.Max(r => r.Longitude);
                }
                else if (hasEc)
                {
                    ecSamplesCache = ParseEcFile(ecPath);
                    if (ecSamplesCache.Count < 3)
                        return "EC file contains fewer than 3 usable points.";
                    minLat = ecSamplesCache.Min(s => s.Latitude);  maxLat = ecSamplesCache.Max(s => s.Latitude);
                    minLon = ecSamplesCache.Min(s => s.Longitude); maxLon = ecSamplesCache.Max(s => s.Longitude);
                }
                else
                {
                    elevationSamplesCache = ParseElevationFile(elevationPath);
                    if (elevationSamplesCache.Count < 3)
                        return "Elevation file contains fewer than 3 usable points.";
                    minLat = elevationSamplesCache.Min(s => s.Latitude);  maxLat = elevationSamplesCache.Max(s => s.Latitude);
                    minLon = elevationSamplesCache.Min(s => s.Longitude); maxLon = elevationSamplesCache.Max(s => s.Longitude);
                }

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

                if (hasYield)
                {
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
                }

                // ── Layer blend ──────────────────────────────────────────────────
                // Each active layer is percentile-normalised to [0,1] by rank (robust
                // to outliers) then blended by the user-supplied fractions.
                // yieldGrid is kept un-blended so per-zone average yield can be reported.
                double[,] classifyGrid = yieldGrid;

                {
                    double[,] normYield = hasYield
                        ? NormalizePercentile(yieldGrid, rows, cols)
                        : null;

                    double[,] normEc = null;
                    if (hasEc)
                    {
                        var ecSamples = ecSamplesCache ?? ParseEcFile(ecPath);
                        if (ecSamples.Count >= 3)
                        {
                            var    ecTree     = BuildTree(ecSamples);
                            double ecFallback = ecSamples.Average(s => s.EcValue);
                            double[,] ecGrid  = new double[rows, cols];
                            for (int r = 0; r < rows; r++)
                                for (int c = 0; c < cols; c++)
                                {
                                    double lat = minLat + (r + 0.5) / rows * (maxLat - minLat);
                                    double lon = minLon + (c + 0.5) / cols * (maxLon - minLon);
                                    ecGrid[r, c] = IDW(ecTree, lat, lon, ecFallback, s => s.EcValue);
                                }
                            normEc = NormalizePercentile(ecGrid, rows, cols);
                        }
                    }

                    double[,] normElev = null;
                    if (hasElevation)
                    {
                        var elevSamples = elevationSamplesCache ?? ParseElevationFile(elevationPath);
                        if (elevSamples.Count >= 3)
                        {
                            var    elevTree     = BuildTree(elevSamples);
                            double elevFallback = elevSamples.Average(s => s.ElevationMeters);
                            double[,] elevGrid  = new double[rows, cols];
                            for (int r = 0; r < rows; r++)
                                for (int c = 0; c < cols; c++)
                                {
                                    double lat = minLat + (r + 0.5) / rows * (maxLat - minLat);
                                    double lon = minLon + (c + 0.5) / cols * (maxLon - minLon);
                                    elevGrid[r, c] = IDW(elevTree, lat, lon, elevFallback, s => s.ElevationMeters);
                                }
                            normElev = NormalizePercentile(elevGrid, rows, cols);
                        }
                    }

                    // If more than one layer is active, blend by fractions.
                    // Single-layer: use that layer's normalised grid directly.
                    int activeCount = (normYield != null ? 1 : 0)
                                    + (normEc    != null ? 1 : 0)
                                    + (normElev  != null ? 1 : 0);

                    if (activeCount > 1)
                    {
                        classifyGrid = new double[rows, cols];
                        for (int r = 0; r < rows; r++)
                            for (int c = 0; c < cols; c++)
                                classifyGrid[r, c] =
                                    (normYield != null ? yieldFraction     * normYield[r, c] : 0.0) +
                                    (normEc    != null ? ecFraction        * normEc[r, c]    : 0.0) +
                                    (normElev  != null ? elevationFraction * normElev[r, c]  : 0.0);
                    }
                    else if (normYield != null) classifyGrid = normYield;
                    else if (normEc    != null) classifyGrid = normEc;
                    else if (normElev  != null) classifyGrid = normElev;
                }

                // ── Pre-classification smoothing ─────────────────────────────────
                // Box-blur the productivity grid before quantile classification.
                // Reduces noise so zone boundaries are spatially coherent.
                // Without this, noisy IDW output produces hundreds of tiny fragments
                // that the post-classification majority filter cannot fully merge.
                classifyGrid = BoxBlur(classifyGrid, rows, cols);
                classifyGrid = BoxBlur(classifyGrid, rows, cols);
                classifyGrid = BoxBlur(classifyGrid, rows, cols);

                // ── Load boundary polygon (optional) ─────────────────────────────
                Polygon boundary = BoundaryKmlExtractor.LoadBoundaryPolygon(boundaryKmlPath);

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

                // ── Build MapZones ────────────────────────────────────────────────
                // Short source label — kept brief so names fit the 20-char column limit.
                // Single yield year: omitted (name is "Z1 Low").
                // Multi-year yield:  "(Nyr)"   → "Z1 Low (3yr)"
                // EC-only:           "EC"      → "Z1 Low EC"
                // Elevation-only:    "El"      → "Z1 Low El"
                string sourceLabel;
                if (!hasYield)
                    sourceLabel = hasEc ? "EC" : "El";
                else if (yearSets.Count > 1)
                    sourceLabel = string.Format("({0}yr)", yearSets.Count);
                else
                    sourceLabel = string.Empty;

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

                    // Clip merged polygon to field boundary if one is set
                    if (boundary != null && !boundary.IsEmpty)
                    {
                        try { merged = merged.Intersection(boundary); } catch { }
                        if (merged == null || merged.IsEmpty) continue;
                    }

                    Color  color = Palette.GetProductivityColor(z, zoneCount);
                    string label = z < prodLabels.Length ? prodLabels[z] : (z + 1).ToString();
                    string name  = string.IsNullOrEmpty(sourceLabel)
                        ? string.Format("Z{0} {1}", z + 1, label)
                        : string.Format("Z{0} {1} {2}", z + 1, label, sourceLabel);
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

        /// <summary>
        /// Percentile-normalises the grid to [0, 1] using rank order.
        /// Each cell's value is replaced by the fraction of cells with a strictly
        /// lower value. Ties share the same lower-bound percentile.
        /// Robust to outliers — a single extreme value cannot skew the distribution.
        /// Flat fields return 0.5 uniformly so they contribute a neutral signal.
        /// </summary>
        private static double[,] NormalizePercentile(double[,] grid, int rows, int cols)
        {
            int n = rows * cols;
            if (n <= 1) { var t = new double[rows, cols]; t[0, 0] = 0.5; return t; }

            var sorted = new double[n];
            int idx = 0;
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    sorted[idx++] = grid[r, c];
            Array.Sort(sorted);

            // Flat field — min == max; return neutral 0.5
            if (sorted[n - 1] - sorted[0] < 1e-9)
            {
                double[,] flat = new double[rows, cols];
                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                        flat[r, c] = 0.5;
                return flat;
            }

            double[,] result = new double[rows, cols];
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                {
                    double v = grid[r, c];
                    // Lower-bound binary search: first index where sorted[i] >= v
                    int lo = 0, hi = n;
                    while (lo < hi) { int m = (lo + hi) >> 1; if (sorted[m] < v) lo = m + 1; else hi = m; }
                    result[r, c] = (double)lo / (n - 1);
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

        // ── Elevation CSV parsing (Lat,Lon,Elevation — header line skipped) ────────

        private static List<FieldSample> ParseElevationFile(string path)
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
                            CultureInfo.InvariantCulture, out double el))  continue;
                    if (el == 0.0) continue;   // GPS zero-sentinel
                    result.Add(new FieldSample(DateTime.MinValue, lat, lon, 0, 0, el));
                }

                // 3-sigma outlier filter — matches ElevationOverlayCreator.FilterOutliers()
                if (result.Count >= 4)
                {
                    double mean  = result.Average(s => s.ElevationMeters);
                    double sd    = Math.Sqrt(result.Average(s => Math.Pow(s.ElevationMeters - mean, 2)));
                    var    clean = result.Where(s => Math.Abs(s.ElevationMeters - mean) <= 3 * sd).ToList();
                    if (clean.Count >= 3) result = clean;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ProductivityZoneCreator/ParseElevationFile: " + ex.Message);
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
