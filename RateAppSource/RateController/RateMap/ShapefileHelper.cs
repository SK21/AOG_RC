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

        public bool SaveMapZones(string shapefilePath, List<MapZone> mapZones)
        {
            bool Result = false;
            try
            {
                if (mapZones == null || mapZones.Count == 0)
                {
                    DeleteShapefileSet(shapefilePath);
                }
                else
                {
                    // target zones
                    var targetZones = mapZones.Where(z => z.ZoneType == ZoneType.Target).ToList();

                    var features = new List<IFeature>();
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
                            { ZoneFields.Color, ColorTranslator.ToHtml(z.ZoneColor) },
                            { ZoneFields.ZoneType,ZoneType.Target }
                            }));
                    }

                    // applied zones
                    // check for recorded data
                    bool AppliedData = false;
                    for (int i = 0; i < 4; i++)
                    {
                        if (MapController.RateCollector.DataPoints(i) > 0)
                        {
                            AppliedData = true;
                            break;
                        }
                    }

                    List<MapZone> AppliedZones = new List<MapZone>();
                    if (AppliedData)
                    {
                        // create new applied zones

                    }
                    else
                    {
                        // use existing applied zones
                        AppliedZones = MergeApplied(mapZones.Where(z => z.ZoneType == ZoneType.Applied).ToList());
                    }

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
                            { ZoneFields.Color, ColorTranslator.ToHtml(z.ZoneColor) },
                            { ZoneFields.ZoneType,ZoneType.Applied }
                            }));
                    }

                    Shapefile.WriteAllFeatures(features, shapefilePath);
                }
                Result = true;
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

        private static int GetLegendBin(double rate, double minRate, double maxRate, int binCount)
        {
            if (rate <= minRate) return 0;
            if (rate >= maxRate) return binCount - 1;

            double t = (rate - minRate) / (maxRate - minRate);
            int bin = (int)Math.Floor(t * binCount);

            return Math.Max(0, Math.Min(bin, binCount - 1));
        }

        private static IEnumerable<Geometry> SplitGeometry(Geometry g)
        {
            if (g is MultiPolygon)
                return ((MultiPolygon)g).Geometries;
            return new[] { g };
        }

        private void AddZone(IFeature feature, Polygon polygon, Dictionary<string, string> mapping, int index, List<MapZone> zones)
        {
            var zone = CreateMapZone(feature, polygon, mapping, index);
            if (zone != null) zones.Add(zone);
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

        private List<MapZone> MergeApplied(List<MapZone> appliedZones)
        {
            var result = new List<MapZone>();

            if (appliedZones == null || appliedZones.Count == 0)
                return result;

            // Extract rates used by legend
            var rates = appliedZones
                .Where(z => z.ZoneType == ZoneType.Applied)
                .Select(z => z.Rates[ZoneFields.ProductA])
                .ToList();

            if (!MapController.TryComputeScale(rates, out double minRate, out double maxRate))
                return result;

            int binCount = Palette.Colors.Length;
            int zoneCounter = 1;

            // Group zones by legend bin
            var groups = appliedZones
                .Where(z => z.ZoneType == ZoneType.Applied)
                .GroupBy(z =>
                    GetLegendBin(
                        z.Rates[ZoneFields.ProductA],
                        minRate,
                        maxRate,
                        binCount))
                .OrderBy(g => g.Key);

            foreach (var group in groups)
            {
                Geometry merged = UnaryUnionOp.Union(group.Select(z => z.Geometry));

                if (merged == null || merged.IsEmpty)
                    continue;

                // Average rates inside bin (matches visual meaning)
                var avgRates = new Dictionary<string, double>
                {
                    { ZoneFields.ProductA, group.Average(z => z.Rates[ZoneFields.ProductA]) },
                    { ZoneFields.ProductB, group.Average(z => z.Rates[ZoneFields.ProductB]) },
                    { ZoneFields.ProductC, group.Average(z => z.Rates[ZoneFields.ProductC]) },
                    { ZoneFields.ProductD, group.Average(z => z.Rates[ZoneFields.ProductD]) }
                };

                Color binColor = Palette.Colors[group.Key % Palette.Colors.Length];

                foreach (var geom in SplitGeometry(merged))
                {
                    if (geom is Polygon poly)
                    {
                        result.Add(new MapZone(
                            name: $"Applied Zone {zoneCounter++}",
                            geometry: poly,
                            rates: avgRates,
                            zoneColor: binColor,
                            zoneType: ZoneType.Applied));
                    }
                }
            }

            return result;
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