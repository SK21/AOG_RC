using GMap.NET.WindowsForms; // added for overlay export
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq; // Add this at the top with other using directives

namespace RateController.Classes
{
    public class ShapefileHelper
    {
        private Color[] palette = new Color[]
            {
                Color.Red, Color.Green, Color.Blue, Color.Orange, Color.Purple, Color.Teal, Color.Brown, Color.Magenta
            };

        public ShapefileHelper()
        {
        }

        public List<MapZone> CreateZoneList(string shapefilePath, Dictionary<string, string> attributeMapping = null)
        {
            var mapZones = new List<MapZone>();

            try
            {
                using (var shapefile = Shapefile.OpenRead(shapefilePath))
                {
                    int zoneIndex = 0;
                    foreach (var feature in shapefile)
                    {
                        if (feature.Geometry is Polygon polygon)
                        {
                            var mapZone = CreateMapZone(feature, polygon, attributeMapping, zoneIndex);
                            if (mapZone != null) mapZones.Add(mapZone);
                            zoneIndex++;
                        }
                        else if (feature.Geometry is MultiPolygon multiPolygon)
                        {
                            foreach (var poly in multiPolygon.Geometries)
                            {
                                if (poly is Polygon multiPolygonPolygon)
                                {
                                    var mapZone = CreateMapZone(feature, multiPolygonPolygon, attributeMapping, zoneIndex);
                                    if (mapZone != null) mapZones.Add(mapZone);
                                    zoneIndex++;
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                if (ex.Message != "Invalid shapefile format.") Props.WriteErrorLog("ShapefileHelper/CreateZoneList: " + ex.Message);
            }
            return mapZones;
        }

        public List<string> GetShapefileAttributes(string shapefilePath)
        {
            using (var shapefile = Shapefile.OpenRead(shapefilePath))
            {
                foreach (var feature in shapefile)
                {
                    return new List<string>(feature.Attributes.GetNames());
                }
            }

            return new List<string>();
        }

        // NEW: Save polygons from a GMap overlay (e.g., applied coverage) to a shapefile with merging of small polygons

        public bool SaveAppliedMap(string shapefilePath, GMapOverlay overlay, double minAreaAcres = 0.25, string rateField = "ProductA")
        {
            bool result = false;
            try
            {
                if (overlay == null || overlay.Polygons == null || overlay.Polygons.Count == 0) return false;

                // 1. Collect all polygons and their rates
                var polygonData = new List<(Polygon Geometry, AttributesTable Attributes, double Acres, double RateValue)>();
                foreach (var gp in overlay.Polygons)
                {
                    try
                    {
                        if (gp == null || gp.Points == null || gp.Points.Count < 3) continue;

                        // Build ring coordinates in (lng, lat)
                        var coords = new List<Coordinate>(gp.Points.Count + 1);
                        for (int i = 0; i < gp.Points.Count; i++)
                        {
                            var p = gp.Points[i];
                            coords.Add(new Coordinate(p.Lng, p.Lat));
                        }
                        // Ensure closed
                        if (!coords[0].Equals2D(coords[coords.Count - 1]))
                        {
                            coords.Add(coords[0]);
                        }

                        var ring = new LinearRing(coords.ToArray());
                        var poly = new Polygon(ring);

                        // Calculate area in acres
                        double acres = CalculateAcres(poly);

                        // Attributes: carry over color if available
                        Color fillColor = Color.Transparent;
                        try
                        {
                            var sb = gp.Fill as SolidBrush;
                            if (sb != null) fillColor = sb.Color;
                        }
                        catch { }

                        var rates = gp.Tag as Dictionary<string, double>;
                        double rateValue = 0.0;
                        if (rates != null && rates.ContainsKey(rateField))
                            rateValue = rates[rateField];

                        var atts = new AttributesTable
                        {
                            { "Name", string.IsNullOrEmpty(gp.Name) ? "Coverage" : gp.Name },
                            { "Color", ColorTranslator.ToHtml(Color.FromArgb(255, fillColor)) },
                            { "ProductA", rates != null && rates.ContainsKey("ProductA") ? rates["ProductA"] : 0.0 },
                            { "ProductB", rates != null && rates.ContainsKey("ProductB") ? rates["ProductB"] : 0.0 },
                            { "ProductC", rates != null && rates.ContainsKey("ProductC") ? rates["ProductC"] : 0.0 },
                            { "ProductD", rates != null && rates.ContainsKey("ProductD") ? rates["ProductD"] : 0.0 }
                        };

                        polygonData.Add((poly, atts, acres, rateValue));
                    }
                    catch (Exception exi)
                    {
                        Props.WriteErrorLog("ShapefileHelper/SaveAppliedMap collect: " + exi.Message);
                    }
                }

                if (polygonData.Count == 0) return false;

                // 2. Compute 5 rate bins (quantiles)
                var validRates = polygonData.Select(d => d.RateValue).Where(v => !double.IsNaN(v) && !double.IsInfinity(v)).OrderBy(v => v).ToArray();
                if (validRates.Length == 0) return false;

                double[] bins = new double[6];
                for (int i = 0; i < 6; i++)
                {
                    int idx = (int)Math.Round((validRates.Length - 1) * i / 5.0);
                    bins[i] = validRates[idx];
                }

                // 3. Group polygons by rate bin
                var polygonsByBin = new List<List<(Polygon Geometry, AttributesTable Attributes, double Acres, double RateValue)>>();
                for (int i = 0; i < 5; i++) polygonsByBin.Add(new List<(Polygon, AttributesTable, double, double)>());

                foreach (var data in polygonData)
                {
                    int binIdx = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        if (data.RateValue >= bins[i] && (i == 4 || data.RateValue < bins[i + 1]))
                        {
                            binIdx = i;
                            break;
                        }
                    }
                    polygonsByBin[binIdx].Add(data);
                }

                // 4. For each bin, merge contiguous polygons and combine small ones
                var mergedPolygons = new List<(Geometry Geometry, AttributesTable Attributes)>();
                int appliedZoneCounter = 1; // Counter for output polygons

                for (int binIdx = 0; binIdx < 5; binIdx++)
                {
                    var group = polygonsByBin[binIdx];
                    if (group.Count == 0) continue;

                    // Separate large and small polygons
                    var largePolys = new List<(Polygon, AttributesTable, double, double)>();
                    var smallPolys = new List<(Polygon, AttributesTable, double, double)>();
                    foreach (var d in group)
                    {
                        if (d.Acres >= minAreaAcres)
                            largePolys.Add(d);
                        else
                            smallPolys.Add(d);
                    }

                    // Merge large polygons in this bin
                    Geometry merged = null;
                    foreach (var d in largePolys)
                    {
                        if (merged == null)
                            merged = d.Item1;
                        else
                        {
                            try
                            {
                                merged = merged.Union(d.Item1);
                            }
                            catch (Exception ex)
                            {
                                Props.WriteErrorLog("ShapefileHelper/SaveAppliedMap: Union failed, attempting Buffer(0): " + ex.Message);
                                try
                                {
                                    // Try to clean both geometries and union again
                                    var cleaned1 = merged.Buffer(0);
                                    var cleaned2 = d.Item1.Buffer(0);
                                    merged = cleaned1.Union(cleaned2);
                                }
                                catch (Exception ex2)
                                {
                                    Props.WriteErrorLog("ShapefileHelper/SaveAppliedMap: Union after Buffer(0) also failed: " + ex2.Message);
                                    // Optionally: skip this geometry or handle as needed
                                }
                            }
                        }
                    }

                    // If there are small polygons, merge them into the merged large geometry if possible
                    foreach (var d in smallPolys)
                    {
                        if (merged == null)
                            merged = d.Item1;
                        else
                            merged = merged.Union(d.Item1);
                    }

                    // Assign attributes for the merged polygon (use average rate for the bin)
                    double avgRateA = group.Average(x => x.Attributes.Exists("ProductA") ? Convert.ToDouble(x.Attributes["ProductA"]) : 0.0);
                    double avgRateB = group.Average(x => x.Attributes.Exists("ProductB") ? Convert.ToDouble(x.Attributes["ProductB"]) : 0.0);
                    double avgRateC = group.Average(x => x.Attributes.Exists("ProductC") ? Convert.ToDouble(x.Attributes["ProductC"]) : 0.0);
                    double avgRateD = group.Average(x => x.Attributes.Exists("ProductD") ? Convert.ToDouble(x.Attributes["ProductD"]) : 0.0);

                    // If merged is MultiPolygon, split into individual polygons and assign unique names
                    if (merged is MultiPolygon multiPoly)
                    {
                        foreach (Polygon poly in multiPoly.Geometries)
                        {
                            var atts = new AttributesTable
                            {
                                { "Name", $"Applied Zone {appliedZoneCounter}" },
                                { "Color", group[0].Attributes.Exists("Color") ? group[0].Attributes["Color"] : "#000000" },
                                { "ProductA", avgRateA },
                                { "ProductB", avgRateB },
                                { "ProductC", avgRateC },
                                { "ProductD", avgRateD }
                            };
                            mergedPolygons.Add((poly, atts));
                            appliedZoneCounter++;
                        }
                    }
                    else if (merged is Polygon poly)
                    {
                        var atts = new AttributesTable
                        {
                            { "Name", $"Applied Zone {appliedZoneCounter}" },
                            { "Color", group[0].Attributes.Exists("Color") ? group[0].Attributes["Color"] : "#000000" },
                            { "ProductA", avgRateA },
                            { "ProductB", avgRateB },
                            { "ProductC", avgRateC },
                            { "ProductD", avgRateD }
                        };
                        mergedPolygons.Add((poly, atts));
                        appliedZoneCounter++;
                    }
                }

                // 5. Save the merged polygons
                var features = new List<IFeature>();
                foreach (var (geometry, attributes) in mergedPolygons)
                {
                    features.Add(new Feature(geometry, attributes));
                }

                if (features.Count > 0)
                {
                    Shapefile.WriteAllFeatures(features, shapefilePath);
                    result = true;
                }
                else
                {
                    // No polygons to save: remove existing shapefile
                    try
                    {
                        if (Props.IsPathSafe(shapefilePath))
                        {
                            string shp = Path.ChangeExtension(shapefilePath, ".shp");
                            string dbf = Path.ChangeExtension(shapefilePath, ".dbf");
                            string shx = Path.ChangeExtension(shapefilePath, ".shx");
                            string prj = Path.ChangeExtension(shapefilePath, ".prj");
                            string qix = Path.ChangeExtension(shapefilePath, ".qix");

                            DeleteIfExists(shp);
                            DeleteIfExists(dbf);
                            DeleteIfExists(shx);
                            DeleteIfExists(prj);
                            DeleteIfExists(qix);
                        }
                        result = true; // treated as successful save of an empty set
                    }
                    catch (Exception ex)
                    {
                        Props.WriteErrorLog("ShapefileHelper/SaveAppliedMap cleanup: " + ex.Message);
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ShapefileHelper/SaveAppliedMap: " + ex.Message);
            }
            return result;
        }

        public bool SaveMapZones(string shapefilePath, List<MapZone> mapZones)
        {
            bool Result = false;
            try
            {
                var features = new List<IFeature>();

                foreach (var mapZone in mapZones)
                {
                    var attributes = new AttributesTable
                {
                    { "Name", mapZone.Name },
                    { "ProductA", mapZone.Rates["ProductA"] },
                    { "ProductB", mapZone.Rates["ProductB"] },
                    { "ProductC", mapZone.Rates["ProductC"] },
                    { "ProductD", mapZone.Rates["ProductD"] },
                    { "Color", ColorTranslator.ToHtml(mapZone.ZoneColor) } // Save color as HTML string
                };

                    features.Add(new Feature(mapZone.Geometry, attributes));
                }

                if (features.Count > 0)
                {
                    Shapefile.WriteAllFeatures(features, shapefilePath);
                    Result = true;
                }
                else
                {
                    // No zones to save: remove existing shapefile so deleted zones don't reappear on reload
                    try
                    {
                        if (Props.IsPathSafe(shapefilePath))
                        {
                            string shp = Path.ChangeExtension(shapefilePath, ".shp");
                            string dbf = Path.ChangeExtension(shapefilePath, ".dbf");
                            string shx = Path.ChangeExtension(shapefilePath, ".shx");
                            string prj = Path.ChangeExtension(shapefilePath, ".prj");
                            string qix = Path.ChangeExtension(shapefilePath, ".qix");

                            DeleteIfExists(shp);
                            DeleteIfExists(dbf);
                            DeleteIfExists(shx);
                            DeleteIfExists(prj);
                            DeleteIfExists(qix);
                        }
                        Result = true; // treated as successful save of an empty set
                    }
                    catch (Exception ex)
                    {
                        Props.WriteErrorLog("ShapefileHelper/DeleteEmptyShapefile: " + ex.Message);
                        Result = false;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Props.WriteErrorLog("ShapefileHelper/SaveMapZones: " + ex.Message);
            }
            return Result;
        }

        // NEW: Save polygons from a GMap overlay (e.g., applied coverage) to a shapefile
        private static void DeleteIfExists(string path)
        {
            try
            {
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ShapefileHelper/DeleteIfExists: " + ex.Message);
            }
        }

        // New helper method to calculate area in acres (copied/adapted from MapZone.Hectares for geographic polygons)
        private double CalculateAcres(Polygon polygon)
        {
            double totalArea = 0;
            try
            {
                // Determine the UTM zone from the first vertex
                int utmZone = (int)(((polygon.ExteriorRing.Coordinates[0].X + 180) / 6) + 1);

                // Define the source and target coordinate systems
                var geographicCS = GeographicCoordinateSystem.WGS84;
                var utmCS = ProjectedCoordinateSystem.WGS84_UTM(utmZone, true);

                // Create coordinate transformation
                var transformationFactory = new CoordinateTransformationFactory();
                var transform = transformationFactory.CreateFromCoordinateSystems(geographicCS, utmCS);

                // Transform the exterior polygon to the projected coordinate system
                var transformedExterior = TransformPolygon((LinearRing)polygon.ExteriorRing, transform);
                totalArea = transformedExterior.Area;

                // Subtract the area of each interior ring (hole)
                for (int i = 0; i < polygon.NumInteriorRings; i++)
                {
                    var transformedHole = TransformPolygon((LinearRing)polygon.GetInteriorRingN(i), transform);
                    totalArea -= transformedHole.Area;
                }

                // Convert to hectares, then to acres (1 hectare ≈ 2.47 acres)
                totalArea /= 10000;  // Square meters to hectares
                totalArea *= 2.47;   // Hectares to acres
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ShapefileHelper/CalculateAcres: " + ex.Message);
            }
            return totalArea;
        }

        private MapZone CreateMapZone(IFeature feature, Polygon polygon, Dictionary<string, string> attributeMapping, int zoneIndex)
        {
            string Name = null;
            double RateA = 0.0, RateB = 0.0, RateC = 0.0, RateD = 0.0;
            Color ZoneColor = palette[zoneIndex % palette.Length]; // Default color
            MapZone NewZone = null;

            try
            {
                if (attributeMapping == null)
                {
                    // loading RC shapefile
                    if (feature.Attributes.Exists("Name")) Name = feature.Attributes["Name"].ToString();
                    else if (feature.Attributes.Exists("ID")) Name = feature.Attributes["ID"].ToString();

                    if (feature.Attributes.Exists("ProductA") && double.TryParse(feature.Attributes["ProductA"]?.ToString(), out double ra)) RateA = ra;
                    if (feature.Attributes.Exists("ProductB") && double.TryParse(feature.Attributes["ProductB"]?.ToString(), out double rb)) RateB = rb;
                    if (feature.Attributes.Exists("ProductC") && double.TryParse(feature.Attributes["ProductC"]?.ToString(), out double rc)) RateC = rc;
                    if (feature.Attributes.Exists("ProductD") && double.TryParse(feature.Attributes["ProductD"]?.ToString(), out double rd)) RateD = rd;
                    if (feature.Attributes.Exists("Color")) ZoneColor = ColorTranslator.FromHtml(feature.Attributes["Color"].ToString());
                }
                else
                {
                    // importing shapefile with mapping
                    foreach (var kvp in attributeMapping)
                    {
                        if (feature.Attributes.Exists(kvp.Value))
                        {
                            switch (kvp.Key)
                            {
                                case "Name":
                                    Name = feature.Attributes[kvp.Value].ToString();
                                    break;

                                case "ProductA":
                                    if (double.TryParse(feature.Attributes[kvp.Value].ToString(), out double ra2)) RateA = ra2;
                                    break;

                                case "ProductB":
                                    if (double.TryParse(feature.Attributes[kvp.Value].ToString(), out double rb2)) RateB = rb2;
                                    break;

                                case "ProductC":
                                    if (double.TryParse(feature.Attributes[kvp.Value].ToString(), out double rc2)) RateC = rc2;
                                    break;

                                case "ProductD":
                                    if (double.TryParse(feature.Attributes[kvp.Value].ToString(), out double rd2)) RateD = rd2;
                                    break;

                                case "Color":
                                    ZoneColor = ColorTranslator.FromHtml(feature.Attributes[kvp.Value].ToString());
                                    break;
                            }
                        }
                    }
                }

                // Assign default name if not found
                if (string.IsNullOrWhiteSpace(Name))
                    Name = $"Zone {zoneIndex + 1}";

                var rates = new Dictionary<string, double>
                {
                    { "ProductA", RateA },
                    { "ProductB", RateB },
                    { "ProductC", RateC },
                    { "ProductD", RateD }
                };

                NewZone = new MapZone(Name, polygon, rates, ZoneColor);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ShapefileHelper/CreateMapZone: " + ex.Message);
            }
            return NewZone;
        }

        // Helper method for polygon transformation (copied from MapZone)
        private Polygon TransformPolygon(LinearRing ring, ICoordinateTransformation transform)
        {
            var transformedCoordinates = new Coordinate[ring.Coordinates.Length];
            for (int i = 0; i < ring.Coordinates.Length; i++)
            {
                double[] xy = { ring.Coordinates[i].X, ring.Coordinates[i].Y };
                double[] transformedXY = transform.MathTransform.Transform(xy);
                transformedCoordinates[i] = new Coordinate(transformedXY[0], transformedXY[1]);
            }
            return new Polygon(new LinearRing(transformedCoordinates));
        }
    }
}