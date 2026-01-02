using GMap.NET.WindowsForms;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri;
using NetTopologySuite.Operation.Union;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using RateController.RateMap;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace RateController.Classes
{
    public class ShapefileHelper
    {
        public List<MapZone> CreateZoneList(string shapefilePath, Dictionary<string, string> attributeMapping = null)
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
                            {
                                AddZone(feature, poly, attributeMapping, index++, zones);
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

        public bool SaveAppliedMap(string shapefilePath, GMapOverlay overlay, double minAreaAcres = 0.5, string rateField = ZoneFields.ProductA)
        {
            bool Result = false;
            try
            {
                var polygons = CollectOverlayPolygons(overlay, rateField);
                if (polygons.Count > 0)
                {
                    // rate bins
                    var values = polygons
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

                    var merged = MergeBins(polygons, bins, minAreaAcres);

                    // update shapefile
                    if (merged.Count > 0)
                    {
                        var features = new List<IFeature>();
                        foreach (var d in merged)
                        {
                            features.Add(new Feature(d.Item1, d.Item2));
                        }
                        Shapefile.WriteAllFeatures(features, shapefilePath);
                    }
                    else
                    {
                        DeleteShapefileSet(shapefilePath);
                    }
                    Result = true;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("SaveAppliedMap: " + ex.Message);
            }
            return Result;
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

        private static AttributesTable BuildAttributes(GMapPolygon gp, Dictionary<string, double> rates)
        {
            Color fill = Color.Transparent;
            if (gp.Fill is SolidBrush) fill = ((SolidBrush)gp.Fill).Color;

            double a = 0, b = 0, c = 0, d = 0;

            if (rates != null)
            {
                if (rates.ContainsKey(ZoneFields.ProductA)) a = rates[ZoneFields.ProductA];
                if (rates.ContainsKey(ZoneFields.ProductB)) b = rates[ZoneFields.ProductB];
                if (rates.ContainsKey(ZoneFields.ProductC)) c = rates[ZoneFields.ProductC];
                if (rates.ContainsKey(ZoneFields.ProductD)) d = rates[ZoneFields.ProductD];
            }

            return new AttributesTable
            {
                { ZoneFields.Name, string.IsNullOrWhiteSpace(gp.Name) ? "Coverage" : gp.Name },
                { ZoneFields.Color, ColorTranslator.ToHtml(Color.FromArgb(255, fill)) },
                { ZoneFields.ProductA, a },
                { ZoneFields.ProductB, b },
                { ZoneFields.ProductC, c },
                { ZoneFields.ProductD, d },
            };
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

        private static Polygon TransformRing(LinearRing ring, ICoordinateTransformation tf)
        {
            var coords = new List<Coordinate>();
            foreach (var c in ring.Coordinates)
            {
                var t = tf.MathTransform.Transform(new[] { c.X, c.Y });
                coords.Add(new Coordinate(t[0], t[1]));
            }
            return new Polygon(new LinearRing(coords.ToArray()));
        }

        private void AddZone(IFeature feature, Polygon polygon, Dictionary<string, string> mapping, int index, List<MapZone> zones)
        {
            var zone = CreateMapZone(feature, polygon, mapping, index);
            if (zone != null) zones.Add(zone);
        }

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

        private List<AppliedPolygon> CollectOverlayPolygons(GMapOverlay overlay, string rateField)
        {
            var results = new List<AppliedPolygon>();
            if (overlay == null || overlay.Polygons == null)
                return results;

            foreach (var gp in overlay.Polygons)
            {
                if (gp == null || gp.Points == null || gp.Points.Count < 3)
                    continue;

                try
                {
                    var poly = BuildPolygon(gp);
                    if (poly == null)
                        continue;

                    double acres = CalculateAcres(poly);

                    var rates = gp.Tag as Dictionary<string, double>;
                    if (rates == null)
                        continue;

                    double rateValue;
                    if (!rates.TryGetValue(rateField, out rateValue))
                        rateValue = 0;

                    results.Add(new AppliedPolygon
                    {
                        Geometry = poly,
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

            return results;
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
                    { ZoneFields.ProductD, 0 }
                };

                Color color = Palette.Colors[index % Palette.Colors.Length];

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

        private List<Tuple<Geometry, AttributesTable>> MergeBins(List<AppliedPolygon> polys, double[] bins, double minAreaAcres)
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

                // Assign a color from the palette for this bin
                Color binColor = Palette.Colors[bin % Palette.Colors.Length];
                string binColorHtml = ColorTranslator.ToHtml(binColor);

                foreach (var geom in SplitGeometry(merged))
                {
                    output.Add(Tuple.Create(
                        geom,
                        new AttributesTable
                        {
                            { ZoneFields.Name, "Applied Zone " + zoneCounter++ },
                            { ZoneFields.Color, binColorHtml },
                            { ZoneFields.ProductA, avgA },
                            { ZoneFields.ProductB, avgB },
                            { ZoneFields.ProductC, avgC },
                            { ZoneFields.ProductD, avgD },
                            { ZoneFields.ZoneType, ZoneType.Applied }
                        }));
                }
            }

            return output;
        }
    }

    internal static class ZoneFields
    {
        public const string Color = "Color";
        public const string Name = "Name";
        public const string ProductA = "ProductA";
        public const string ProductB = "ProductB";
        public const string ProductC = "ProductC";
        public const string ProductD = "ProductD";
        public const string ZoneType = "ZoneType";
    }

    internal sealed class AppliedPolygon
    {
        public double Acres { get; set; }
        public AttributesTable Attributes { get; set; }
        public Polygon Geometry { get; set; }
        public double RateValue { get; set; }
    }
}