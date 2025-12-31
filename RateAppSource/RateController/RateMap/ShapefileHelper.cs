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

        // NEW: Save polygons from a GMap overlay (e.g., applied coverage) to a shapefile with merging of small polygons
        public bool SaveOverlayPolygons(string shapefilePath, GMapOverlay overlay, double minAreaAcres = 0.25)
        {
            bool result = false;
            try
            {
                if (overlay == null || overlay.Polygons == null || overlay.Polygons.Count == 0) return false;

                // Collect all polygons with their geometries, areas, and attributes
                var polygonData = new List<(Polygon Geometry, AttributesTable Attributes, double Acres)>();
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

                        var atts = new AttributesTable
                        {
                            { "Name", string.IsNullOrEmpty(gp.Name) ? "Coverage" : gp.Name },
                            { "Color", ColorTranslator.ToHtml(Color.FromArgb(255, fillColor)) },
                            { "Alpha", fillColor.A },
                            { "Acres", acres }
                        };

                        polygonData.Add((poly, atts, acres));
                    }
                    catch (Exception exi)
                    {
                        Props.WriteErrorLog("ShapefileHelper/SaveOverlayPolygons collect: " + exi.Message);
                    }
                }

                // Separate into large and small polygons
                var largePolygons = new List<(Polygon Geometry, AttributesTable Attributes, double Acres)>();
                var smallPolygons = new List<(Polygon Geometry, AttributesTable Attributes, double Acres)>();
                foreach (var data in polygonData)
                {
                    if (data.Acres >= minAreaAcres)
                    {
                        largePolygons.Add(data);
                    }
                    else
                    {
                        smallPolygons.Add(data);
                    }
                }

                // Merge small polygons into the closest large ones
                var mergedPolygons = new List<(Geometry Geometry, AttributesTable Attributes)>();
                for (int i = 0; i < largePolygons.Count; i++)
                {
                    // Clean geometry if needed
                    var geom = largePolygons[i].Geometry;
                    if (!geom.IsValid)
                    {
                        Props.WriteErrorLog($"ShapefileHelper/SaveOverlayPolygons: Large polygon {i} invalid, cleaning with Buffer(0)");
                        var cleaned = geom.Buffer(0);
                        if (cleaned is Polygon cleanedPoly)
                        {
                            geom = cleanedPoly;
                        }
                        else if (cleaned is MultiPolygon cleanedMulti && cleanedMulti.NumGeometries > 0)
                        {
                            // Use the first polygon from the MultiPolygon
                            geom = cleanedMulti.GetGeometryN(0) as Polygon;
                        }
                        else
                        {
                            Props.WriteErrorLog("ShapefileHelper/SaveOverlayPolygons: Buffer(0) did not return a Polygon or MultiPolygon.");
                            continue;
                        }
                    }
                    mergedPolygons.Add((geom, largePolygons[i].Attributes));
                }

                foreach (var small in smallPolygons)
                {
                    if (largePolygons.Count == 0) continue; // No large to merge into

                    // Find the closest large polygon by centroid distance
                    double minDistance = double.MaxValue;
                    int closestIndex = -1;
                    var smallCentroid = small.Geometry.Centroid;
                    for (int i = 0; i < mergedPolygons.Count; i++)
                    {
                        var largeCentroid = mergedPolygons[i].Geometry.Centroid;
                        double distance = smallCentroid.Distance(largeCentroid);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closestIndex = i;
                        }
                    }

                    if (closestIndex >= 0)
                    {
                        // Clean small geometry if needed
                        var smallGeom = small.Geometry;
                        if (!smallGeom.IsValid)
                        {
                            Props.WriteErrorLog("ShapefileHelper/SaveOverlayPolygons: Small polygon invalid, cleaning with Buffer(0)");
                            var cleaned = smallGeom.Buffer(0);
                            if (cleaned is Polygon cleanedPoly)
                            {
                                smallGeom = cleanedPoly;
                            }
                            else if (cleaned is MultiPolygon cleanedMulti && cleanedMulti.NumGeometries > 0)
                            {
                                // Use the first polygon from the MultiPolygon
                                smallGeom = cleanedMulti.GetGeometryN(0) as Polygon;
                            }
                            else
                            {
                                Props.WriteErrorLog("ShapefileHelper/SaveOverlayPolygons: Buffer(0) did not return a Polygon or MultiPolygon.");
                                continue;
                            }
                        }
                        // Union the geometries, with error handling
                        try
                        {
                            var unionGeometry = mergedPolygons[closestIndex].Geometry.Union(smallGeom);

                            // If union is invalid, try cleaning
                            if (!unionGeometry.IsValid)
                            {
                                Props.WriteErrorLog("ShapefileHelper/SaveOverlayPolygons: Union geometry invalid, cleaning with Buffer(0)");
                                unionGeometry = unionGeometry.Buffer(0);
                            }

                            if (unionGeometry is Polygon || unionGeometry is MultiPolygon)
                            {
                                mergedPolygons[closestIndex] = (unionGeometry, mergedPolygons[closestIndex].Attributes);
                                // Recalculate area and update attributes
                                double newAcres = CalculateAcres(
                                    unionGeometry is Polygon p ? p : ((MultiPolygon)unionGeometry).Geometries[0] as Polygon
                                );
                                mergedPolygons[closestIndex].Attributes["Acres"] = newAcres;
                            }
                            else
                            {
                                Props.WriteErrorLog("ShapefileHelper/SaveOverlayPolygons: Union produced non-polygon geometry.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Props.WriteErrorLog("ShapefileHelper/SaveOverlayPolygons: Union failed: " + ex.Message);
                        }
                    }
                }

                // If no large polygons, include small ones as is
                if (largePolygons.Count == 0)
                {
                    foreach (var small in smallPolygons)
                    {
                        mergedPolygons.Add((small.Geometry, small.Attributes));
                    }
                }

                // Save the merged polygons
                var features = new List<IFeature>();
                foreach (var (geometry, attributes) in mergedPolygons)
                {
                    if (geometry is Polygon poly)
                    {
                        features.Add(new Feature(poly, attributes));
                    }
                    else if (geometry is MultiPolygon multiPoly)
                    {
                        foreach (Polygon polyPart in multiPoly.Geometries)
                        {
                            features.Add(new Feature(polyPart, attributes));
                        }
                    }
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
                        Props.WriteErrorLog("ShapefileHelper/SaveOverlayPolygons cleanup: " + ex.Message);
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ShapefileHelper/SaveOverlayPolygons: " + ex.Message);
            }
            return result;
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