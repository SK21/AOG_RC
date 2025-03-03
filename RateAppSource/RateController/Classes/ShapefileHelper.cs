using GMap.NET;
using GMap.NET.WindowsForms;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri;
using NetTopologySuite.IO.Esri.Shapefiles.Writers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace RateController.Classes
{
    public class ShapefileHelper
    {
        private FormStart mf;
        private DateTime LastRateSave;

        public ShapefileHelper(FormStart mf)
        {
            this.mf = mf;
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
                mf.Tls.WriteErrorLog("ShapefileHelper/LoadAndMapShapefile: " + ex.Message);
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

        public List<GMapPolygon> LoadAppliedRateAreas(string shapefilePath)
        {
            var appliedPolygons = new List<GMapPolygon>();
            try
            {
                using (var shapefile = Shapefile.OpenRead(shapefilePath))
                {
                    foreach (var feature in shapefile)
                    {
                        if (feature.Geometry is Polygon polygon)
                        {
                            var points = polygon.Coordinates.Select(coord => new PointLatLng(coord.Y, coord.X)).ToList();
                            GMapPolygon gmapPolygon = new GMapPolygon(points, "LoadedAppliedArea")
                            {
                                Stroke = new Pen(Color.Black, 1),
                                Fill = new SolidBrush(Color.FromArgb(100, Color.Blue))
                            };
                            appliedPolygons.Add(gmapPolygon);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog($"ShapefileHelper/LoadAppliedRateAreas: {ex.Message}");
            }
            return appliedPolygons;
        }

        public void SaveAppliedRateAreas(string shapefilePath, List<GMapPolygon> appliedRatePolygons)
        {
            try
            {
                var appliedFeatures = new List<Feature>();
                foreach (var polygon in appliedRatePolygons)
                {
                    var coordinates = polygon.Points.Select(pt => new Coordinate(pt.Lng, pt.Lat)).ToArray();
                    var geometry = new Polygon(new LinearRing(coordinates));
                    var attributes = new AttributesTable { { "RateName", "Applied" } };
                    appliedFeatures.Add(new Feature(geometry, attributes));
                }

                if (appliedFeatures.Count > 0)
                {
                    Shapefile.WriteAllFeatures(appliedFeatures, shapefilePath);
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog($"ShapefileHelper/SaveAppliedRateAreas: {ex.Message}");
            }
        }

        public void SaveRates(string shapefilePath, List<GMapPolygon> appliedRatePolygons, int[] Rates)
        {
            try
            {
                if (DateTime.Now.Subtract(LastRateSave).TotalSeconds > 30)
                {
                    LastRateSave = DateTime.Now;
                    string RatesFolder = Path.Combine(shapefilePath, "Rates");
                    var appliedFeatures = new List<Feature>();
                    foreach (var polygon in appliedRatePolygons)
                    {
                        var coordinates = polygon.Points.Select(pt => new Coordinate(pt.Lng, pt.Lat)).ToArray();
                        var geometry = new Polygon(new LinearRing(coordinates));
                        var attributes=new AttributesTable
                        { 
                            { "ProductA", Rates[0] },
                            { "ProductB", Rates[1] },
                            { "ProductC", Rates[2] },
                            { "ProductD", Rates[3] }
                        };

                        appliedFeatures.Add(new Feature(geometry, attributes));
                    }

                    if (appliedFeatures.Count > 0)
                    {
                        // Ensure the applied rates subfolder exists
                        Directory.CreateDirectory(RatesFolder);

                        // Create a subfolder for each zone within applied rates (optional)
                        string sanitizedZoneName = string.Concat(mapZone.Name.Split(Path.GetInvalidFileNameChars()));
                        string zoneAppliedRatesFolder = Path.Combine(appliedRatesFolderPath, sanitizedZoneName);
                        Directory.CreateDirectory(zoneAppliedRatesFolder);

                        // Construct the file path for the "as applied" shapefile
                        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        string appliedShapefileName = $"AppliedRates_{timestamp}.shp";
                        string appliedShapefilePath = Path.Combine(zoneAppliedRatesFolder, appliedShapefileName);

                        Shapefile.WriteAllFeatures(appliedFeatures, appliedShapefilePath);

                    }
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog($"ShapefileHelper/SaveRates: {ex.Message}");
            }
        }

        public bool SaveMapZones(string zoneShapefilePath, List<MapZone> mapZones)
        {
            bool result = false;
            try
            {
                // Save zone polygons (unchanged)
                var zoneFeatures = new List<Feature>();

                foreach (var mapZone in mapZones)
                {
                    var attributes = new AttributesTable
            {
                { "Name", mapZone.Name },
                { "ProductA", mapZone.Rates["ProductA"] },
                { "ProductB", mapZone.Rates["ProductB"] },
                { "ProductC", mapZone.Rates["ProductC"] },
                { "ProductD", mapZone.Rates["ProductD"] },
                { "Color", ColorTranslator.ToHtml(mapZone.ZoneColor) }
            };

                    zoneFeatures.Add(new Feature(mapZone.Geometry, attributes));
                }

                if (zoneFeatures.Count > 0)
                {
                    Shapefile.WriteAllFeatures(zoneFeatures, zoneShapefilePath);
                }




                // Derive the applied rates folder path as a subfolder of zoneShapefilePath
                string zoneDirectory = Path.GetDirectoryName(zoneShapefilePath);
                string appliedRatesFolderPath = Path.Combine(zoneDirectory, "Rates");

                // Save "as applied" data for each zone
                foreach (var mapZone in mapZones)
                {
                    var appliedFeatures = new List<Feature>();

                    foreach (var appliedRateEntry in mapZone.AppliedRates)
                    {
                        var pointLatLng = appliedRateEntry.Key; // Location
                        var rateDictionary = appliedRateEntry.Value; // Dictionary<string, double>

                        foreach (var rateEntry in rateDictionary)
                        {
                            string rateName = rateEntry.Key;     // Applied Rate Name
                            double rateValue = rateEntry.Value;  // Applied Rate Value

                            // Create a point geometry at the applied rate location
                            var coordinate = new Coordinate(pointLatLng.Lng, pointLatLng.Lat); // Longitude (X), Latitude (Y)
                            var point = new NetTopologySuite.Geometries.Point(coordinate);

                            // Create attributes for the applied rate point
                            var appliedRateAttributes = new AttributesTable
                    {
                        { "ZoneName", mapZone.Name },
                        { "RateName", rateName },
                        { "AppliedRate", rateValue },
                        { "Latitude", pointLatLng.Lat },
                        { "Longitude", pointLatLng.Lng },
                    };

                            // Add the point feature to the list
                            appliedFeatures.Add(new Feature(point, appliedRateAttributes));
                        }
                    }

                    if (appliedFeatures.Count > 0)
                    {
                        // Ensure the applied rates subfolder exists
                        Directory.CreateDirectory(appliedRatesFolderPath);

                        // Create a subfolder for each zone within applied rates (optional)
                        string sanitizedZoneName = string.Concat(mapZone.Name.Split(Path.GetInvalidFileNameChars()));
                        string zoneAppliedRatesFolder = Path.Combine(appliedRatesFolderPath, sanitizedZoneName);
                        Directory.CreateDirectory(zoneAppliedRatesFolder);

                        // Construct the file path for the "as applied" shapefile
                        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        string appliedShapefileName = $"AppliedRates_{timestamp}.shp";
                        string appliedShapefilePath = Path.Combine(zoneAppliedRatesFolder, appliedShapefileName);

                        Shapefile.WriteAllFeatures(appliedFeatures, appliedShapefilePath);
                    }
                }

                result = true;
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog($"ShapefileHelper/SaveMapZones: {ex.Message}");
            }
            return result;
        }

        private MapZone CreateMapZone(IFeature feature, Polygon polygon, Dictionary<string, string> attributeMapping)
        {
            string Name = "Unnamed Zone";
            int RateA = 0;
            int RateB = 0;
            int RateC = 0;
            int RateD = 0;
            Color ZoneColor = Color.Blue;
            MapZone NewZone = null;

            try
            {
                if (attributeMapping == null)
                {
                    // loading RC shapefile
                    if (feature.Attributes.Exists("Name")) Name = feature.Attributes["Name"].ToString();
                    if (feature.Attributes.Exists("ProductA") && int.TryParse(feature.Attributes["ProductA"]?.ToString(), out int ra)) RateA = ra;
                    if (feature.Attributes.Exists("ProductB") && int.TryParse(feature.Attributes["ProductB"]?.ToString(), out int rb)) RateB = rb;
                    if (feature.Attributes.Exists("ProductC") && int.TryParse(feature.Attributes["ProductC"]?.ToString(), out int rc)) RateC = rc;
                    if (feature.Attributes.Exists("ProductD") && int.TryParse(feature.Attributes["ProductD"]?.ToString(), out int rd)) RateD = rd;
                    if (feature.Attributes.Exists("Color")) ZoneColor = ColorTranslator.FromHtml(feature.Attributes["Color"].ToString());
                }
                else
                {
                    // importing shapefile
                    foreach (KeyValuePair<string, string> kvp in attributeMapping)
                    {
                        if (feature.Attributes.Exists(kvp.Value))
                        {
                            switch (kvp.Key)
                            {
                                case "Name":
                                    Name = feature.Attributes[kvp.Value].ToString();
                                    break;

                                case "ProductA":
                                    if (int.TryParse(feature.Attributes[kvp.Value].ToString(), out int ra)) RateA = ra;
                                    break;

                                case "ProductB":
                                    if (int.TryParse(feature.Attributes[kvp.Value].ToString(), out int rb)) RateB = rb;
                                    break;

                                case "ProductC":
                                    if (int.TryParse(feature.Attributes[kvp.Value].ToString(), out int rc)) RateC = rc;
                                    break;

                                case "ProductD":
                                    if (int.TryParse(feature.Attributes[kvp.Value].ToString(), out int rd)) RateD = rd;
                                    break;

                                case "Color":
                                    ZoneColor = ColorTranslator.FromHtml(feature.Attributes[kvp.Value].ToString());
                                    break;
                            }
                        }
                    }
                }

                var rates = new Dictionary<string, int>
                {
                { "ProductA", RateA },
                { "ProductB", RateB },
                { "ProductC", RateC },
                { "ProductD", RateD }
                };

                NewZone = new MapZone(Name, polygon, rates, ZoneColor, mf);
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("ShapefileHelper/CreateMapZone: " + ex.Message);
            }
            return NewZone;
        }
    }
}