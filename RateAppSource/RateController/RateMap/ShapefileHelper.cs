using GMap.NET.WindowsForms; // added for overlay export
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace RateController.Classes
{
    public class ShapefileHelper
    {
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
                    foreach (var feature in shapefile)
                    {
                        if (feature.Geometry is Polygon polygon)
                        {
                            var mapZone = CreateMapZone(feature, polygon, attributeMapping);
                            if (mapZone != null) mapZones.Add(mapZone);
                        }
                        else if (feature.Geometry is MultiPolygon multiPolygon)
                        {
                            foreach (var poly in multiPolygon.Geometries)
                            {
                                if (poly is Polygon multiPolygonPolygon)
                                {
                                    var mapZone = CreateMapZone(feature, multiPolygonPolygon, attributeMapping);
                                    if (mapZone != null) mapZones.Add(mapZone);
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

        // NEW: Save polygons from a GMap overlay (e.g., applied coverage) to a shapefile
        public bool SaveOverlayPolygons(string shapefilePath, GMapOverlay overlay)
        {
            bool result = false;
            try
            {
                if (overlay == null || overlay.Polygons == null || overlay.Polygons.Count == 0) return false;

                var features = new List<IFeature>();
                foreach (var gp in overlay.Polygons)
                {
                    try
                    {
                        if (gp == null || gp.Points == null || gp.Points.Count < 3) continue;

                        // build ring coordinates in (lng, lat)
                        var coords = new List<Coordinate>(gp.Points.Count + 1);
                        for (int i = 0; i < gp.Points.Count; i++)
                        {
                            var p = gp.Points[i];
                            coords.Add(new Coordinate(p.Lng, p.Lat));
                        }
                        // ensure closed
                        if (!coords[0].Equals2D(coords[coords.Count - 1]))
                        {
                            coords.Add(coords[0]);
                        }

                        var ring = new LinearRing(coords.ToArray());
                        var poly = new Polygon(ring);

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
                            { "Alpha", fillColor.A }
                        };

                        features.Add(new Feature(poly, atts));
                    }
                    catch (Exception exi)
                    {
                        Props.WriteErrorLog("ShapefileHelper/SaveOverlayPolygons item: " + exi.Message);
                    }
                }

                if (features.Count > 0)
                {
                    Shapefile.WriteAllFeatures(features, shapefilePath);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ShapefileHelper/SaveOverlayPolygons: " + ex.Message);
            }
            return result;
        }

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

        private MapZone CreateMapZone(IFeature feature, Polygon polygon, Dictionary<string, string> attributeMapping)
        {
            string Name = "Unnamed Zone";
            double RateA = 0.0;
            double RateB = 0.0;
            double RateC = 0.0;
            double RateD = 0.0;
            Color ZoneColor = Color.Blue;
            MapZone NewZone = null;

            try
            {
                if (attributeMapping == null)
                {
                    // loading RC shapefile
                    if (feature.Attributes.Exists("Name")) Name = feature.Attributes["Name"].ToString();
                    if (feature.Attributes.Exists("ProductA") && double.TryParse(feature.Attributes["ProductA"]?.ToString(), out double ra)) RateA = ra;
                    if (feature.Attributes.Exists("ProductB") && double.TryParse(feature.Attributes["ProductB"]?.ToString(), out double rb)) RateB = rb;
                    if (feature.Attributes.Exists("ProductC") && double.TryParse(feature.Attributes["ProductC"]?.ToString(), out double rc)) RateC = rc;
                    if (feature.Attributes.Exists("ProductD") && double.TryParse(feature.Attributes["ProductD"]?.ToString(), out double rd)) RateD = rd;
                    if (feature.Attributes.Exists("Color")) ZoneColor = ColorTranslator.FromHtml(feature.Attributes["Color"].ToString());
                }
                else
                {
                    // importing shapefile
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
    }
}