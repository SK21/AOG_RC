using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace RateController.Classes
{
    public class ShapefileHelper
    {
        private FormStart mf;

        public ShapefileHelper(FormStart mf)
        {
            this.mf = mf;
        }

        public List<MapZone> CreateMapZones(string shapefilePath, Dictionary<string, string> attributeMapping = null)
        {
            var mapZones = new List<MapZone>();
            var geometryFactory = new GeometryFactory();

            try
            {
                using (var shapefile = Shapefile.OpenRead(shapefilePath))
                {
                    List<IFeature> featureList = shapefile.Cast<IFeature>().ToList();
                    mf.Tls.WriteActivityLog($"Loaded {featureList.Count} features from shapefile.");
                    List<Polygon> polygons = new List<Polygon>();
                    polygons = polygons.OrderByDescending(p => p.Area).ToList();
                    Dictionary<Polygon, IFeature> featureMapping = new Dictionary<Polygon, IFeature>();

                    // Extract all polygons and their features
                    foreach (var feature in featureList)
                    {
                        if (feature.Geometry is Polygon polygon)
                        {
                            polygons.Add(polygon);
                            featureMapping[polygon] = feature;
                        }
                        else if (feature.Geometry is MultiPolygon multiPolygon)
                        {
                            foreach (var geom in multiPolygon.Geometries)
                            {
                                if (geom is Polygon poly)
                                {
                                    polygons.Add(poly);
                                    featureMapping[poly] = feature;
                                }
                            }
                        }
                    }
                    mf.Tls.WriteActivityLog($"Extracted {polygons.Count} polygons from shapefile.");

                    // Detect outer polygons and holes
                    var processed = new HashSet<Polygon>();
                    foreach (var outer in polygons)
                    {
                        if (processed.Contains(outer)) continue;

                        var holes = polygons.Where(inner => outer != inner && outer.Contains(inner))
                            .ToList();

                        mf.Tls.WriteActivityLog($"Processing polygon with {outer.Coordinates.Length} vertices. Found {holes.Count} potential holes.");

                        if (outer.Coordinates.Length == 97)
                        {
                            if (holes.Any())
                            {
                                var outerRing = geometryFactory.CreateLinearRing(outer.ExteriorRing.Coordinates);
                                var holeRings = holes.Select(h => geometryFactory.CreateLinearRing(h.ExteriorRing.Coordinates)).ToArray();
                                var polygonWithHoles = geometryFactory.CreatePolygon(outerRing, holeRings);
                                var mapZone = CreateMapZone(featureMapping[outer], polygonWithHoles, attributeMapping);
                                if (mapZone != null)
                                {
                                    mapZones.Add(mapZone);
                                    mf.Tls.WriteActivityLog($"Added polygon with {holes.Count} holes: {mapZone.Name}");
                                }
                                else
                                {
                                    mf.Tls.WriteErrorLog("MapZone creation failed for a polygon with holes.");
                                }
                                processed.Add(outer);
                                foreach (var inner in holes)
                                {
                                    processed.Add(inner);
                                }
                            }
                            else
                            {
                                var singlePolygon = geometryFactory.CreatePolygon(outer.ExteriorRing.Coordinates);
                                var mapZone = CreateMapZone(featureMapping[outer], singlePolygon, attributeMapping);
                                if (mapZone != null)
                                {
                                    mapZones.Add(mapZone);
                                    mf.Tls.WriteActivityLog($"Added standalone polygon: {mapZone.Name}");
                                }
                                else
                                {
                                    mf.Tls.WriteErrorLog("MapZone creation failed for a standalone polygon.");
                                }
                                processed.Add(outer);
                            }
                        }
                    }
                    mf.Tls.WriteActivityLog($"Total MapZones created: {mapZones.Count}");
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("ShapefileHelper/CreateMapZones: " + ex.Message);
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
            }
            catch (System.Exception ex)
            {
                mf.Tls.WriteErrorLog("ShapefileHelper/SaveMapZones: " + ex.Message);
            }
            return Result;
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