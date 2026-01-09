using GMap.NET.WindowsForms;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri;
using NetTopologySuite.Operation.Union;
using RateController.RateMap;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                    { ZoneFields.ProductD, 0 }
                };

                Color color = Palette.GetColor(index);

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
                    { ZoneFields.ProductD, group.Average(z => z.Rates[ZoneFields.ProductD]) }
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