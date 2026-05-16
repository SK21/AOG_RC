using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Union;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace RateController.RateMap
{
    public class XmlHelper
    {
        public List<MapZone> SimplifyZones(List<MapZone> inputZones, int zoneCount,
            double minZoneHa, double minRateStep, string primaryProduct)
        {
            var result = new List<MapZone>();
            if (inputZones == null || inputZones.Count == 0) return result;

            if (string.IsNullOrEmpty(primaryProduct)) primaryProduct = ZoneFields.ProductA;
            zoneCount = System.Math.Max(2, System.Math.Min(20, zoneCount));

            var sorted = inputZones
                .OrderBy(z => z.Rates.TryGetValue(primaryProduct, out double v) ? v : 0)
                .ToList();
            int total = sorted.Count;
            int binCount = System.Math.Min(zoneCount, total);

            var binZones = Enumerable.Range(0, binCount).Select(_ => new List<MapZone>()).ToList();
            for (int i = 0; i < total; i++)
                binZones[System.Math.Min(binCount - 1, i * binCount / total)].Add(sorted[i]);

            // Sieve: merge bins below minZoneHa into their nearest neighbour by rate.
            if (minZoneHa > 0)
            {
                bool anyMerged = true;
                while (anyMerged && binZones.Count(b => b.Count > 0) > 2)
                {
                    anyMerged = false;
                    for (int b = 0; b < binZones.Count; b++)
                    {
                        if (binZones[b].Count == 0) continue;
                        double binHa = binZones[b].Sum(z => FastHa(z.Geometry));
                        if (binHa < minZoneHa)
                        {
                            int neighbor = -1;
                            for (int d = 1; d < binZones.Count; d++)
                            {
                                if (b - d >= 0 && binZones[b - d].Count > 0) { neighbor = b - d; break; }
                                if (b + d < binZones.Count && binZones[b + d].Count > 0) { neighbor = b + d; break; }
                            }
                            if (neighbor >= 0)
                            {
                                binZones[neighbor].AddRange(binZones[b]);
                                binZones[b].Clear();
                                anyMerged = true;
                            }
                        }
                    }
                }
            }

            var activeBins = binZones.Where(b => b.Count > 0).ToList();

            for (int b = 0; b < activeBins.Count; b++)
            {
                var bz = activeBins[b];

                var primaryRates = bz
                    .Select(z => z.Rates.TryGetValue(primaryProduct, out double v) ? v : 0)
                    .OrderBy(v => v).ToList();
                double medianRate = primaryRates[primaryRates.Count / 2];

                var avgRates = new Dictionary<string, double>();
                foreach (var key in ZoneFields.Products)
                    avgRates[key] = bz.Average(z => z.Rates.TryGetValue(key, out double v) ? v : 0);
                avgRates[primaryProduct] = medianRate;

                Color color = Palette.GetProductivityColor(b, activeBins.Count, 255);
                string tempName = string.Format("Zone {0} ({1:F0})", b + 1, medianRate);

                // Union all polygons in this bin so scattered patches of the same rate
                // are merged into the fewest possible output polygons.
                var geoms = bz.Select(z => (Geometry)z.Geometry).ToList();
                Geometry merged = null;
                try { merged = CascadedPolygonUnion.Union(geoms); }
                catch { merged = geoms.Count > 0 ? geoms[0] : null; }
                if (merged == null || merged.IsEmpty) continue;

                if (merged is Polygon poly)
                {
                    result.Add(new MapZone(tempName, poly,
                        new Dictionary<string, double>(avgRates), color, ZoneType.Target));
                }
                else if (merged is MultiPolygon mp)
                {
                    for (int j = 0; j < mp.NumGeometries; j++)
                    {
                        if (mp.GetGeometryN(j) is Polygon p && !p.IsEmpty)
                            result.Add(new MapZone(tempName, p,
                                new Dictionary<string, double>(avgRates), color, ZoneType.Target));
                    }
                }
            }

            // Enforce minimum rate step between zones ordered by primary product rate.
            if (minRateStep > 0 && result.Count > 1)
            {
                var stepGroups = result
                    .GroupBy(z => z.Rates.TryGetValue(primaryProduct, out double pv) ? pv : 0)
                    .OrderBy(g => g.Key).ToList();

                double prevRate = stepGroups[0].Key;
                for (int gi = 1; gi < stepGroups.Count; gi++)
                {
                    double required = prevRate + minRateStep;
                    if (stepGroups[gi].Key < required)
                    {
                        foreach (var zone in stepGroups[gi])
                            zone.Rates[primaryProduct] = required;
                        prevRate = required;
                    }
                    else prevRate = stepGroups[gi].Key;
                }
            }

            // Renumber zones sequentially by rate; add part suffix for multiple polygons per zone.
            {
                var nameGroups = result
                    .GroupBy(z => z.Rates.TryGetValue(primaryProduct, out double pv) ? pv : 0)
                    .OrderBy(g => g.Key).ToList();

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

        private static double FastHa(Polygon poly)
        {
            double midLatRad = poly.Centroid.Y * System.Math.PI / 180.0;
            return poly.Area * 111319.0 * 111319.0 * System.Math.Cos(midLatRad) / 10000.0;
        }
    }
}
