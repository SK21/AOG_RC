using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri;
using NetTopologySuite.Operation.Union;
using RateController.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RateController.RateMap
{
    public class ShapefileHelper
    {
        public List<MapZone> CreateZoneList(string shapefilePath, Dictionary<string, string> attributeMapping = null)
        {
            var zones = new List<MapZone>();

            try
            {
                if (IsValidShapefile(shapefilePath))
                {
                    using (var shapefile = Shapefile.OpenRead(shapefilePath))
                    {
                        int index = 0;
                        foreach (var feature in shapefile)
                        {
                            if (feature.Geometry is Polygon)
                            {
                                var zone = CreateMapZone(feature, (Polygon)feature.Geometry, attributeMapping, index++);
                                if (zone != null) zones.Add(zone);
                            }
                            else if (feature.Geometry is MultiPolygon)
                            {
                                var mp = (MultiPolygon)feature.Geometry;
                                foreach (Polygon poly in mp.Geometries)
                                {
                                    var zone = CreateMapZone(feature, poly, attributeMapping, index++);
                                    if (zone != null) zones.Add(zone);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ShapefileHelper/CreateZoneList: " + ex.Message);
            }

            return zones;
        }

        public List<string> GetShapefileAttributes(string shapefilePath)
        {
            using (var shapefile = Shapefile.OpenRead(shapefilePath))
            {
                foreach (var feature in shapefile)
                    return feature.Attributes.GetNames().ToList();
            }
            return new List<string>();
        }

        public bool SaveMapZones(string shapefilePath, List<MapZone> ZonesToSave)
        {
            bool Result = false;
            try
            {
                var features = new List<IFeature>();

                // target zones
                if (ZonesToSave != null && ZonesToSave.Count > 0)
                {
                    var targetZones = ZonesToSave.Where(z => z.ZoneType == ZoneType.Target).ToList();

                    foreach (var z in targetZones)
                    {
                        features.Add(new Feature(
                            z.Geometry,
                            new AttributesTable
                            {
                            { ZoneFields.Name, z.Name },
                            { ZoneFields.ProductA, z.Rates[ZoneFields.ProductA] },
                            { ZoneFields.ProductB, z.Rates[ZoneFields.ProductB] },
                            { ZoneFields.ProductC, z.Rates[ZoneFields.ProductC] },
                            { ZoneFields.ProductD, z.Rates[ZoneFields.ProductD] },
                            { ZoneFields.ProductE, z.Rates[ZoneFields.ProductE] },
                            { ZoneFields.Color, ColorTranslator.ToHtml(z.ZoneColor) },
                            { ZoneFields.ZoneType, ZoneType.Target.ToString() }
                            }));
                    }
                }

                // applied zones
                List<MapZone> AppliedZones = new List<MapZone>();
                if (!MapController.ZnOverlays.BuildNewAppliedZones(out AppliedZones))
                {
                    // use existing applied zones
                    if (ZonesToSave != null) AppliedZones = ZonesToSave.Where(z => z.ZoneType == ZoneType.Applied).ToList();
                }

                if (AppliedZones.Count > 0)
                {
                    AppliedZones = MergeApplied(AppliedZones);

                    foreach (var z in AppliedZones)
                    {
                        features.Add(new Feature(
                            z.Geometry,
                            new AttributesTable
                            {
                            { ZoneFields.Name, z.Name },
                            { ZoneFields.ProductA, z.Rates[ZoneFields.ProductA] },
                            { ZoneFields.ProductB, z.Rates[ZoneFields.ProductB] },
                            { ZoneFields.ProductC, z.Rates[ZoneFields.ProductC] },
                            { ZoneFields.ProductD, z.Rates[ZoneFields.ProductD] },
                            { ZoneFields.ProductE, z.Rates[ZoneFields.ProductE] },
                            { ZoneFields.Color, ColorTranslator.ToHtml(z.ZoneColor) },
                            { ZoneFields.ZoneType, ZoneType.Applied.ToString() }
                            }));
                    }
                }

                if (features.Count > 0)
                {
                    Shapefile.WriteAllFeatures(features, shapefilePath);
                    Result = true;
                }
                else
                {
                    DeleteShapefileSet(shapefilePath);
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ShapefileHelper/SaveMapZones: " + ex.Message);
            }
            return Result;
        }

        private static void DeleteIfExists(string path)
        {
            try
            {
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                    File.Delete(path);
            }
            catch { }
        }

        private static void DeleteShapefileSet(string basePath)
        {
            if (!Props.IsPathSafe(basePath))
                return;

            foreach (var ext in new[] { ".shp", ".shx", ".dbf", ".prj", ".qix" })
                DeleteIfExists(System.IO.Path.ChangeExtension(basePath, ext));
        }

        private static IEnumerable<Geometry> SplitGeometry(Geometry g)
        {
            if (g is MultiPolygon)
                return ((MultiPolygon)g).Geometries;
            return new[] { g };
        }

        private MapZone CreateMapZone(IFeature feature, Polygon polygon, Dictionary<string, string> mapping, int index)
        {
            try
            {
                string name = "Zone " + (index + 1);
                var rates = new Dictionary<string, double>
                {
                    { ZoneFields.ProductA, 0 },
                    { ZoneFields.ProductB, 0 },
                    { ZoneFields.ProductC, 0 },
                    { ZoneFields.ProductD, 0 },
                    { ZoneFields.ProductE, 0 }
                };

                Color color = Palette.GetColor(index, trns: 255);

                if (mapping == null)
                {
                    if (feature.Attributes.Exists(ZoneFields.Name)) name = feature.Attributes[ZoneFields.Name].ToString();

                    foreach (var k in rates.Keys.ToList())
                    {
                        if (feature.Attributes.Exists(k)) rates[k] = Convert.ToDouble(feature.Attributes[k]);
                    }

                    if (feature.Attributes.Exists(ZoneFields.Color)) color = ColorTranslator.FromHtml(feature.Attributes[ZoneFields.Color].ToString());
                }
                else
                {
                    // Map the zone name
                    string nameField = mapping.ContainsKey(ZoneFields.Name) ? mapping[ZoneFields.Name] : ZoneFields.Name;
                    if (feature.Attributes.Exists(nameField)) name = feature.Attributes[nameField].ToString();

                    // Map the rates
                    foreach (var k in rates.Keys.ToList())
                    {
                        string mappedField = mapping.ContainsKey(k) ? mapping[k] : k;
                        if (feature.Attributes.Exists(mappedField)) rates[k] = Convert.ToDouble(feature.Attributes[mappedField]);
                    }

                    // Map the color
                    string colorField = mapping.ContainsKey(ZoneFields.Color) ? mapping[ZoneFields.Color] : ZoneFields.Color;
                    if (feature.Attributes.Exists(colorField)) color = ColorTranslator.FromHtml(feature.Attributes[colorField].ToString());
                }

                ZoneType cZoneType = ZoneType.Target;
                if (feature.Attributes.Exists(ZoneFields.ZoneType)) cZoneType = Enum.TryParse(feature.Attributes[ZoneFields.ZoneType].ToString(), out cZoneType) ? cZoneType : ZoneType.Target;

                return new MapZone(name, polygon, rates, color, cZoneType);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ShapefileHelper/CreateMapZone: " + ex.Message);
                return null;
            }
        }

        private bool IsValidShapefile(string fullPath)
        {
            string directory = Path.GetDirectoryName(fullPath);
            string filenameWithoutExt = Path.GetFileNameWithoutExtension(fullPath);
            string pathWithoutExtension = Path.Combine(directory, filenameWithoutExt);

            return File.Exists(pathWithoutExtension + ".shp") &&
                   File.Exists(pathWithoutExtension + ".shx") &&
                   File.Exists(pathWithoutExtension + ".dbf");
        }

        public List<MapZone> SimplifyPrescriptionGrid(List<MapZone> zones, int zoneCount, double minZoneHa = 1.0, double minRateStep = 0.0)
        {
            var result = new List<MapZone>();
            if (zones == null || zones.Count == 0) return result;

            zoneCount = Math.Max(2, Math.Min(8, zoneCount));

            // ── Build spatial grid from cell centroids ────────────────────────────
            double minX = zones.Min(z => z.Geometry.EnvelopeInternal.MinX);
            double maxX = zones.Max(z => z.Geometry.EnvelopeInternal.MaxX);
            double minY = zones.Min(z => z.Geometry.EnvelopeInternal.MinY);
            double maxY = zones.Max(z => z.Geometry.EnvelopeInternal.MaxY);

            var env0 = zones[0].Geometry.EnvelopeInternal;
            double cellW0 = env0.Width;
            double cellH0 = env0.Height;
            if (cellW0 < 1e-9 || cellH0 < 1e-9) return result;

            int cols = Math.Max(1, (int)Math.Round((maxX - minX) / cellW0));
            int rows = Math.Max(1, (int)Math.Round((maxY - minY) / cellH0));

            // Use exact average cell dimensions so constructed rectangles tile seamlessly
            double cellW = (maxX - minX) / cols;
            double cellH = (maxY - minY) / rows;

            var rateGrid = new double[rows, cols];
            var hasCell  = new bool[rows, cols];
            var cellIndex = new Dictionary<(int, int), MapZone>(zones.Count);

            foreach (var z in zones)
            {
                var env = z.Geometry.EnvelopeInternal;
                double cx = (env.MinX + env.MaxX) / 2.0;
                double cy = (env.MinY + env.MaxY) / 2.0;
                int c = Math.Max(0, Math.Min(cols - 1, (int)((cx - minX) / cellW)));
                int r = Math.Max(0, Math.Min(rows - 1, (int)((cy - minY) / cellH)));
                rateGrid[r, c] = z.Rates[ZoneFields.ProductA];
                hasCell[r, c]  = true;
                cellIndex[(r, c)] = z;
            }

            // ── Quantile-classify ─────────────────────────────────────────────────
            var flatRates = new List<double>(zones.Count);
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    if (hasCell[r, c]) flatRates.Add(rateGrid[r, c]);
            flatRates.Sort();
            int total = flatRates.Count;

            var binGrid = new int[rows, cols];
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                {
                    if (!hasCell[r, c]) continue;
                    int rank = flatRates.BinarySearch(rateGrid[r, c]);
                    if (rank < 0) rank = ~rank;
                    rank = Math.Min(rank, total - 1);
                    binGrid[r, c] = Math.Min(zoneCount - 1, rank * zoneCount / total);
                }

            // ── Spatial majority-filter smoothing (up to 30 passes) ───────────────
            for (int pass = 0; pass < 30; pass++)
            {
                var next = GridMajorityFilter(binGrid, hasCell, rows, cols);
                if (GridsEqual(binGrid, next, rows, cols)) break;
                binGrid = next;
            }

            // ── Sieve: absorb small connected regions into dominant neighbour ──────
            // Converts the ha threshold to cells, then reassigns any connected
            // component smaller than that into its most-common neighbouring zone.
            // Every field cell keeps a zone — nothing is left blank.
            double midLat    = (minY + maxY) / 2.0 * Math.PI / 180.0;
            double sqDegToHa = 111319.0 * 111319.0 * Math.Cos(midLat) / 10000.0;
            int    minCells  = Math.Max(1, (int)(minZoneHa / sqDegToHa / (cellW * cellH)));
            binGrid = GridSieveSmallRegions(binGrid, hasCell, rows, cols, minCells);

            // ── Fill empty cells (prescription gaps) ──────────────────────────────
            // Only fill small connected empty regions (edge artefacts where no centroid
            // landed). Large empty regions — yard sites, field-exterior bounding-box
            // corners — are intentional and must NOT be filled. Use minCells as the
            // upper limit so anything smaller than the minimum zone size is treated as
            // a gap, but anything larger is left blank.
            binGrid = FillEmptyCells(binGrid, hasCell, rows, cols, Math.Max(minCells, 4));

            // ── Union cell geometries per bin ─────────────────────────────────────
            // Construct fresh grid-aligned rectangles rather than using the original
            // shapefile geometries. Adjacent constructed rectangles share exact edge
            // coordinates (both computed as minX + k*cellW), so CascadedPolygonUnion
            // produces a seamless result with no micro-gaps or interior holes.
            var factory    = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(4326);
            var geomsByBin = new List<List<Geometry>>(zoneCount);
            for (int b = 0; b < zoneCount; b++) geomsByBin.Add(new List<Geometry>());

            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                {
                    if (!hasCell[r, c]) continue;
                    double left   = minX + c * cellW;
                    double right  = minX + (c + 1) * cellW;
                    double bottom = minY + r * cellH;
                    double top    = minY + (r + 1) * cellH;
                    var coords = new[]
                    {
                        new Coordinate(left, bottom), new Coordinate(right, bottom),
                        new Coordinate(right, top),   new Coordinate(left, top),
                        new Coordinate(left, bottom)
                    };
                    geomsByBin[binGrid[r, c]].Add(factory.CreatePolygon(coords));
                }

            // Minimum polygon area: sub-cell artifact filter only (sieve handles real minimums)
            double minArea = 4.0 * cellW * cellH;

            // ── Build result MapZones ─────────────────────────────────────────────
            for (int bin = 0; bin < zoneCount; bin++)
            {
                if (geomsByBin[bin].Count == 0) continue;

                Geometry merged;
                try { merged = CascadedPolygonUnion.Union(geomsByBin[bin]); }
                catch (TopologyException)
                {
                    merged = geomsByBin[bin][0];
                    for (int i = 1; i < geomsByBin[bin].Count; i++)
                        try { merged = merged.Union(geomsByBin[bin][i]); } catch { }
                }
                if (merged == null || merged.IsEmpty) continue;

                // Rates: median ProductA, average for other products
                var binZones = new List<MapZone>();
                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                        if (hasCell[r, c] && binGrid[r, c] == bin && cellIndex.TryGetValue((r, c), out var z))
                            binZones.Add(z);
                if (binZones.Count == 0) continue;

                var binRatesSorted = binZones.Select(z => z.Rates[ZoneFields.ProductA]).OrderBy(v => v).ToList();
                double medianRate  = binRatesSorted[binRatesSorted.Count / 2];

                var avgRates = new Dictionary<string, double>();
                foreach (var key in ZoneFields.Products)
                    avgRates[key] = binZones.Average(z => z.Rates[key]);
                avgRates[ZoneFields.ProductA] = medianRate;

                Color color = Palette.GetProductivityColor(bin, zoneCount, 255);

                foreach (var geom in SplitGeometry(merged))
                {
                    if (geom is Polygon poly && poly.Area >= minArea)
                    {
                        result.Add(new MapZone(
                            name: string.Format("Zone {0} ({1:F0})", bin + 1, medianRate),
                            geometry: poly,
                            rates: new Dictionary<string, double>(avgRates),
                            zoneColor: color,
                            zoneType: ZoneType.Target));
                    }
                }
            }

            // Enforce minimum rate step between zones ordered by ProductA rate.
            // Zones sharing the same rate value belong to the same bin — all get bumped together.
            if (minRateStep > 0 && result.Count > 1)
            {
                var stepGroups = result
                    .GroupBy(z => z.Rates[ZoneFields.ProductA])
                    .OrderBy(g => g.Key)
                    .ToList();

                double prevRate = stepGroups[0].Key;
                for (int gi = 1; gi < stepGroups.Count; gi++)
                {
                    double required = prevRate + minRateStep;
                    if (stepGroups[gi].Key < required)
                    {
                        foreach (var zone in stepGroups[gi])
                            zone.Rates[ZoneFields.ProductA] = required;
                        prevRate = required;
                    }
                    else
                    {
                        prevRate = stepGroups[gi].Key;
                    }
                }
            }

            // Renumber zones sequentially (some bins may produce no geometry and get skipped,
            // leaving gaps like Zone 1, 3, 5, 6) and update the rate in the name to reflect
            // any min-step adjustment made above.
            {
                var nameGroups = result
                    .GroupBy(z => z.Rates[ZoneFields.ProductA])
                    .OrderBy(g => g.Key)
                    .ToList();

                int zoneNum = 1;
                foreach (var group in nameGroups)
                {
                    double displayRate = group.Key;
                    int partNum = 1;
                    foreach (var zone in group)
                    {
                        zone.Name = partNum == 1
                            ? string.Format("Zone {0} ({1:F0})", zoneNum, displayRate)
                            : string.Format("Zone {0} ({1:F0}) ({2})", zoneNum, displayRate, partNum);
                        partNum++;
                    }
                    zoneNum++;
                }
            }

            return result;
        }

        private static int[,] FillEmptyCells(int[,] grid, bool[,] mask, int rows, int cols, int maxGapCells)
        {
            var result  = (int[,])grid.Clone();
            var visited = new bool[rows, cols];

            for (int r0 = 0; r0 < rows; r0++)
                for (int c0 = 0; c0 < cols; c0++)
                {
                    if (mask[r0, c0] || visited[r0, c0]) continue;

                    // BFS: find all cells in this connected empty region
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
                                && !mask[nr, nc] && !visited[nr, nc])
                            {
                                visited[nr, nc] = true;
                                queue.Enqueue((nr, nc));
                            }
                        }
                    }

                    // Skip large empty regions — yard sites, field-exterior corners
                    if (cells.Count > maxGapCells) continue;

                    // Flood-fill this small gap from its filled neighbours
                    bool anyFilled = true;
                    while (anyFilled)
                    {
                        anyFilled = false;
                        foreach (var (r, c) in cells)
                        {
                            if (mask[r, c]) continue;
                            var counts = new Dictionary<int, int>();
                            foreach (var (nr, nc) in new[] { (r-1,c),(r+1,c),(r,c-1),(r,c+1) })
                            {
                                if (nr >= 0 && nr < rows && nc >= 0 && nc < cols && mask[nr, nc])
                                {
                                    int z = result[nr, nc];
                                    counts[z] = counts.ContainsKey(z) ? counts[z] + 1 : 1;
                                }
                            }
                            if (counts.Count > 0)
                            {
                                result[r, c] = counts.OrderByDescending(kv => kv.Value).First().Key;
                                mask[r, c]   = true;
                                anyFilled    = true;
                            }
                        }
                    }
                }

            return result;
        }

        private static int[,] GridSieveSmallRegions(int[,] grid, bool[,] mask, int rows, int cols, int minCells)
        {
            var result    = (int[,])grid.Clone();
            bool anyMerged = true;
            while (anyMerged)
            {
                anyMerged = false;
                var visited = new bool[rows, cols];
                for (int r0 = 0; r0 < rows; r0++)
                    for (int c0 = 0; c0 < cols; c0++)
                    {
                        if (!mask[r0, c0] || visited[r0, c0]) continue;
                        int zoneId = result[r0, c0];

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
                                    && mask[nr, nc] && !visited[nr, nc] && result[nr, nc] == zoneId)
                                { visited[nr, nc] = true; queue.Enqueue((nr, nc)); }
                            }
                        }

                        if (cells.Count >= minCells) continue;

                        var neighborCounts = new Dictionary<int, int>();
                        foreach (var (r, c) in cells)
                            foreach (var (nr, nc) in new[] { (r-1,c),(r+1,c),(r,c-1),(r,c+1) })
                            {
                                if (nr >= 0 && nr < rows && nc >= 0 && nc < cols
                                    && mask[nr, nc] && result[nr, nc] != zoneId)
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
            return result;
        }

        private static int[,] GridMajorityFilter(int[,] grid, bool[,] mask, int rows, int cols)
        {
            var result = (int[,])grid.Clone();
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                {
                    if (!mask[r, c]) continue;
                    var counts = new Dictionary<int, int>();
                    for (int dr = -1; dr <= 1; dr++)
                        for (int dc = -1; dc <= 1; dc++)
                        {
                            int rr = r + dr, cc = c + dc;
                            if (rr < 0 || cc < 0 || rr >= rows || cc >= cols || !mask[rr, cc]) continue;
                            int z = grid[rr, cc];
                            counts[z] = counts.ContainsKey(z) ? counts[z] + 1 : 1;
                        }
                    if (counts.Count > 0)
                        result[r, c] = counts.OrderByDescending(x => x.Value).First().Key;
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

        private List<MapZone> MergeApplied(List<MapZone> appliedZones)
        {
            var result = new List<MapZone>();

            if (appliedZones == null || appliedZones.Count == 0)
                return result;

            // Only consider applied zones
            var appliedOnly = appliedZones
                .Where(z => z != null && z.ZoneType == ZoneType.Applied && z.Geometry != null)
                .ToList();

            if (appliedOnly.Count == 0)
                return result;

            int zoneCounter = 1;

            // Group zones by their existing color so we preserve the
            // color that was assigned in BuildNewAppliedZones / history overlay.
            var groups = appliedOnly
                .GroupBy(z => z.ZoneColor)
                .ToList();

            foreach (var group in groups)
            {
                // Clean geometries with a zero-width buffer to fix minor topology issues
                var cleaned = group
                    .Select(z =>
                    {
                        try
                        {
                            return z.Geometry.Buffer(0);
                        }
                        catch
                        {
                            return null;
                        }
                    })
                    .Where(g => g != null && !g.IsEmpty)
                    .ToList();

                if (cleaned.Count == 0)
                    continue;

                Geometry merged;

                try
                {
                    merged = UnaryUnionOp.Union(cleaned);
                }
                catch (TopologyException)
                {
                    // Last-resort fallback: union incrementally
                    merged = cleaned[0];
                    for (int i = 1; i < cleaned.Count; i++)
                    {
                        try
                        {
                            merged = merged.Union(cleaned[i]);
                        }
                        catch
                        {
                            // skip bad geometry
                        }
                    }
                }

                if (merged == null || merged.IsEmpty)
                    continue;

                // Average rates inside color group (so merged zone still represents
                // the same rate band, just with smoothed geometry)
                var avgRates = new Dictionary<string, double>
                {
                    { ZoneFields.ProductA, group.Average(z => z.Rates[ZoneFields.ProductA]) },
                    { ZoneFields.ProductB, group.Average(z => z.Rates[ZoneFields.ProductB]) },
                    { ZoneFields.ProductC, group.Average(z => z.Rates[ZoneFields.ProductC]) },
                    { ZoneFields.ProductD, group.Average(z => z.Rates[ZoneFields.ProductD]) },
                    { ZoneFields.ProductE, group.Average(z => z.Rates[ZoneFields.ProductE]) }
                };

                Color groupColor = group.Key;

                foreach (var geom in SplitGeometry(merged))
                {
                    if (geom is Polygon poly)
                    {
                        result.Add(new MapZone(
                            name: $"Applied Zone {zoneCounter++}",
                            geometry: poly,
                            rates: avgRates,
                            zoneColor: groupColor,
                            zoneType: ZoneType.Applied));
                    }
                }
            }

            return result;
        }
    }
}
