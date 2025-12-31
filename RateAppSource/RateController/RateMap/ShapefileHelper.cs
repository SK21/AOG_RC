using GMap.NET.WindowsForms;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri;
using NetTopologySuite.Operation.Union;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace RateController.Classes
{
    internal static class ZoneFields
    {
        public const string Name = "Name";
        public const string Color = "Color";
        public const string ProductA = "ProductA";
        public const string ProductB = "ProductB";
        public const string ProductC = "ProductC";
        public const string ProductD = "ProductD";
    }

    internal sealed class AppliedPolygon
    {
        public Polygon Geometry { get; set; }
        public AttributesTable Attributes { get; set; }
        public double Acres { get; set; }
        public double RateValue { get; set; }
    }

    public class ShapefileHelper
    {
        private readonly Color[] palette = new Color[]
        {
            Color.Red, Color.Green, Color.Blue, Color.Orange,
            Color.Purple, Color.Teal, Color.Brown, Color.Magenta
        };

        // ============================================================
        // PUBLIC API
        // ============================================================

        public List<MapZone> CreateZoneList(string shapefilePath, Dictionary<string, string> attributeMapping=null)
        {
            var zones = new List<MapZone>();

            try
            {
                using (var shapefile = Shapefile.OpenRead(shapefilePath))
                {
                    int index = 0;
                    foreach (var feature in shapefile)
                    {
                        if (feature.Geometry is Polygon)
                        {
                            AddZone(feature, (Polygon)feature.Geometry, attributeMapping, index++, zones);
                        }
                        else if (feature.Geometry is MultiPolygon)
                        {
                            var mp = (MultiPolygon)feature.Geometry;
                            foreach (Polygon poly in mp.Geometries)
                                AddZone(feature, poly, attributeMapping, index++, zones);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("CreateZoneList: " + ex.Message);
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

        public bool SaveAppliedMap(string shapefilePath, GMapOverlay overlay, double minAreaAcres = 0.5, string rateField = ZoneFields.ProductA)
        {
            try
            {
                var polygons = CollectOverlayPolygons(overlay, rateField);
                if (polygons.Count == 0)
                    return false;

                var bins = ComputeRateBins(polygons);
                var merged = MergeBins(polygons, bins, minAreaAcres);

                return WriteOrCleanupShapefile(shapefilePath, merged);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("SaveAppliedMap: " + ex.Message);
                return false;
            }
        }

        public bool SaveMapZones(string shapefilePath, List<MapZone> mapZones)
        {
            try
            {
                if (mapZones == null || mapZones.Count == 0)
                {
                    DeleteShapefileSet(shapefilePath);
                    return true;
                }

                var features = new List<IFeature>();
                foreach (var z in mapZones)
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
                            { ZoneFields.Color, ColorTranslator.ToHtml(z.ZoneColor) }
                        }));
                }

                Shapefile.WriteAllFeatures(features, shapefilePath);
                return true;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("SaveMapZones: " + ex.Message);
                return false;
            }
        }

        // ============================================================
        // APPLIED MAP HELPERS
        // ============================================================

        private List<AppliedPolygon> CollectOverlayPolygons(GMapOverlay overlay, string rateField)
        {
            var result = new List<AppliedPolygon>();
            if (overlay == null || overlay.Polygons == null)
                return result;

            foreach (var gp in overlay.Polygons)
            {
                if (gp == null || gp.Points == null || gp.Points.Count < 3)
                    continue;

                try
                {
                    Polygon polygon = BuildPolygon(gp);
                    double acres = CalculateAcres(polygon);

                    double rateValue = 0;
                    var rates = gp.Tag as Dictionary<string, double>;
                    if (rates != null && rates.ContainsKey(rateField))
                        rateValue = rates[rateField];

                    result.Add(new AppliedPolygon
                    {
                        Geometry = polygon,
                        Acres = acres,
                        RateValue = rateValue,
                        Attributes = BuildAttributes(gp, rates)
                    });
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("CollectOverlayPolygons: " + ex.Message);
                }
            }

            return result;
        }

        private static Polygon BuildPolygon(GMapPolygon gp)
        {
            var coords = new List<Coordinate>();

            foreach (var p in gp.Points)
                coords.Add(new Coordinate(p.Lng, p.Lat));

            if (coords.Count < 3)
                return null;

            if (!coords[0].Equals2D(coords[coords.Count - 1]))
                coords.Add(coords[0]);

            var ring = new LinearRing(coords.ToArray());
            var poly = new Polygon(ring);

            poly.Normalize();

            if (!poly.IsValid)
                poly = (Polygon)poly.Buffer(0);

            return poly;
        }

        private static AttributesTable BuildAttributes(
            GMapPolygon gp,
            Dictionary<string, double> rates)
        {
            Color fill = Color.Transparent;
            if (gp.Fill is SolidBrush)
                fill = ((SolidBrush)gp.Fill).Color;

            return new AttributesTable
            {
                { ZoneFields.Name, string.IsNullOrWhiteSpace(gp.Name) ? "Coverage" : gp.Name },
                { ZoneFields.Color, ColorTranslator.ToHtml(Color.FromArgb(255, fill)) },
                { ZoneFields.ProductA, rates != null && rates.ContainsKey(ZoneFields.ProductA) ? rates[ZoneFields.ProductA] : 0 },
                { ZoneFields.ProductB, rates != null && rates.ContainsKey(ZoneFields.ProductB) ? rates[ZoneFields.ProductB] : 0 },
                { ZoneFields.ProductC, rates != null && rates.ContainsKey(ZoneFields.ProductC) ? rates[ZoneFields.ProductC] : 0 },
                { ZoneFields.ProductD, rates != null && rates.ContainsKey(ZoneFields.ProductD) ? rates[ZoneFields.ProductD] : 0 }
            };
        }

        private static double[] ComputeRateBins(List<AppliedPolygon> polys)
        {
            var values = polys
                .Select(p => p.RateValue)
                .Where(v => !double.IsNaN(v) && !double.IsInfinity(v))
                .OrderBy(v => v)
                .ToArray();

            var bins = new double[6];
            for (int i = 0; i < bins.Length; i++)
            {
                int idx = (int)Math.Floor((values.Length - 1) * i / 5.0);
                if (idx < 0) idx = 0;
                if (idx >= values.Length) idx = values.Length - 1;
                bins[i] = values[idx];
            }
            return bins;
        }

        private List<Tuple<Geometry, AttributesTable>> MergeBins(
            List<AppliedPolygon> polys,
            double[] bins,
            double minAreaAcres)
        {
            var output = new List<Tuple<Geometry, AttributesTable>>();
            int zoneCounter = 1;

            for (int bin = 0; bin < 5; bin++)
            {
                var group = polys
                    .Where(p => p.RateValue >= bins[bin] &&
                               (bin == 4 || p.RateValue < bins[bin + 1]))
                    .ToList();

                if (group.Count == 0)
                    continue;

                var merged = UnaryUnionOp.Union(group.Select(p => p.Geometry));

                double avgA = group.Average(p => Convert.ToDouble(p.Attributes[ZoneFields.ProductA]));
                double avgB = group.Average(p => Convert.ToDouble(p.Attributes[ZoneFields.ProductB]));
                double avgC = group.Average(p => Convert.ToDouble(p.Attributes[ZoneFields.ProductC]));
                double avgD = group.Average(p => Convert.ToDouble(p.Attributes[ZoneFields.ProductD]));

                foreach (var geom in SplitGeometry(merged))
                {
                    output.Add(Tuple.Create(
                        geom,
                        new AttributesTable
                        {
                            { ZoneFields.Name, "Applied Zone " + zoneCounter++ },
                            { ZoneFields.Color, group[0].Attributes[ZoneFields.Color] },
                            { ZoneFields.ProductA, avgA },
                            { ZoneFields.ProductB, avgB },
                            { ZoneFields.ProductC, avgC },
                            { ZoneFields.ProductD, avgD }
                        }));
                }
            }

            return output;
        }

        private static IEnumerable<Geometry> SplitGeometry(Geometry g)
        {
            if (g is MultiPolygon)
                return ((MultiPolygon)g).Geometries;
            return new[] { g };
        }

        private static bool WriteOrCleanupShapefile(
            string path,
            List<Tuple<Geometry, AttributesTable>> data)
        {
            if (data.Count == 0)
            {
                DeleteShapefileSet(path);
                return true;
            }

            var features = new List<IFeature>();
            foreach (var d in data)
                features.Add(new Feature(d.Item1, d.Item2));

            Shapefile.WriteAllFeatures(features, path);
            return true;
        }

        // ============================================================
        // ZONE IMPORT HELPERS
        // ============================================================

        private void AddZone(
            IFeature feature,
            Polygon polygon,
            Dictionary<string, string> mapping,
            int index,
            List<MapZone> zones)
        {
            var zone = CreateMapZone(feature, polygon, mapping, index);
            if (zone != null)
                zones.Add(zone);
        }

        private MapZone CreateMapZone(
            IFeature feature,
            Polygon polygon,
            Dictionary<string, string> mapping,
            int index)
        {
            try
            {
                string name = "Zone " + (index + 1);
                var rates = new Dictionary<string, double>
                {
                    { ZoneFields.ProductA, 0 },
                    { ZoneFields.ProductB, 0 },
                    { ZoneFields.ProductC, 0 },
                    { ZoneFields.ProductD, 0 }
                };

                Color color = palette[index % palette.Length];

                if (mapping == null)
                {
                    if (feature.Attributes.Exists(ZoneFields.Name))
                        name = feature.Attributes[ZoneFields.Name].ToString();

                    foreach (var k in rates.Keys.ToList())
                        if (feature.Attributes.Exists(k))
                            rates[k] = Convert.ToDouble(feature.Attributes[k]);

                    if (feature.Attributes.Exists(ZoneFields.Color))
                        color = ColorTranslator.FromHtml(feature.Attributes[ZoneFields.Color].ToString());
                }

                return new MapZone(name, polygon, rates, color);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("CreateMapZone: " + ex.Message);
                return null;
            }
        }

        // ============================================================
        // GEOMETRY & FILE HELPERS
        // ============================================================

        private double CalculateAcres(Polygon polygon)
        {
            try
            {
                var c = polygon.ExteriorRing.Coordinates[0];
                int zone = (int)((c.X + 180) / 6) + 1;
                bool north = c.Y >= 0;

                var geo = GeographicCoordinateSystem.WGS84;
                var utm = ProjectedCoordinateSystem.WGS84_UTM(zone, north);

                var tf = new CoordinateTransformationFactory()
                    .CreateFromCoordinateSystems(geo, utm);

                double area = TransformRing((LinearRing)polygon.ExteriorRing, tf).Area;

                for (int i = 0; i < polygon.NumInteriorRings; i++)
                    area -= TransformRing((LinearRing)polygon.GetInteriorRingN(i), tf).Area;

                return (area / 10000.0) * 2.47;
            }
            catch
            {
                return 0;
            }
        }

        private static Polygon TransformRing(
            LinearRing ring,
            ICoordinateTransformation tf)
        {
            var coords = new List<Coordinate>();
            foreach (var c in ring.Coordinates)
            {
                var t = tf.MathTransform.Transform(new[] { c.X, c.Y });
                coords.Add(new Coordinate(t[0], t[1]));
            }
            return new Polygon(new LinearRing(coords.ToArray()));
        }

        private static void DeleteShapefileSet(string basePath)
        {
            if (!Props.IsPathSafe(basePath))
                return;

            foreach (var ext in new[] { ".shp", ".shx", ".dbf", ".prj", ".qix" })
                DeleteIfExists(Path.ChangeExtension(basePath, ext));
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
    }
}
