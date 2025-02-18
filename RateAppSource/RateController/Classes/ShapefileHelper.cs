using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri;
using System.Collections.Generic;
using System.Drawing;

namespace RateController.Classes
{
    public class ShapefileHelper
    {
        private FormStart mf;

        public ShapefileHelper(FormStart mf)
        {
            this.mf = mf;
        }

        public List<string> GetShapefileAttributes(string shapefilePath)
        {
            using (var shapefile = Shapefile.OpenRead(shapefilePath))
            {
                //if (shapefile.Features.Count > 0)
                //{
                //    var feature = shapefile.Features[0];
                //    return new List<string>(feature.Attributes.GetNames());
                //}

                foreach (var feature in shapefile)
                {
                    return new List<string>(feature.Attributes.GetNames());
                }
            }

            return new List<string>();
        }

        public List<MapZone> LoadAndMapShapefile(string shapefilePath, Dictionary<string, string> attributeMapping)
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
                            ProcessPolygonWithMapping(feature, polygon, mapZones, attributeMapping);
                        }
                        else if (feature.Geometry is MultiPolygon multiPolygon)
                        {
                            foreach (var poly in multiPolygon.Geometries)
                            {
                                if (poly is Polygon multiPolygonPolygon)
                                {
                                    ProcessPolygonWithMapping(feature, multiPolygonPolygon, mapZones, attributeMapping);
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

        public List<MapZone> LoadMapZones(string shapefilePath)
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
                            ProcessPolygon(feature, polygon, mapZones);
                        }
                        else if (feature.Geometry is MultiPolygon multiPolygon)
                        {
                            foreach (var poly in multiPolygon.Geometries)
                            {
                                if (poly is Polygon multiPolygonPolygon)
                                {
                                    ProcessPolygon(feature, multiPolygonPolygon, mapZones);
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                mf.Tls.WriteErrorLog("ShapefileHelper/LoadMapZones: " + ex.Message);
            }

            return mapZones;
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

        private void ProcessPolygon(IFeature feature, Polygon polygon, List<MapZone> mapZones)
        {
            try
            {
                var name = feature.Attributes.Exists("Name")
                    ? feature.Attributes["Name"].ToString()
                    : "Unnamed Zone";

                var productARate = feature.Attributes.Exists("ProductA") && int.TryParse(feature.Attributes["ProductA"]?.ToString(), out var aRate)
                    ? aRate
                    : 0;

                var productBRate = feature.Attributes.Exists("ProductB") && int.TryParse(feature.Attributes["ProductB"]?.ToString(), out var bRate)
                    ? bRate
                    : 0;

                var productCRate = feature.Attributes.Exists("ProductC") && int.TryParse(feature.Attributes["ProductC"]?.ToString(), out var cRate)
                    ? cRate
                    : 0;

                var productDRate = feature.Attributes.Exists("ProductD") && int.TryParse(feature.Attributes["ProductD"]?.ToString(), out var dRate)
                    ? dRate
                    : 0;

                var colorString = feature.Attributes.Exists("Color") ? feature.Attributes["Color"].ToString() : "#FF0000"; // Default to red
                var zoneColor = ColorTranslator.FromHtml(colorString); // Convert HTML string to Color

                var rates = new Dictionary<string, int>
            {
                { "ProductA", productARate },
                { "ProductB", productBRate },
                { "ProductC", productCRate },
                { "ProductD", productDRate }
            };

                mapZones.Add(new MapZone(name, polygon, rates, zoneColor, mf)); // Pass color to MapZone
            }
            catch (System.Exception ex)
            {
                mf.Tls.WriteErrorLog("shapefilehelper/processpolygon: " + ex.Message);
            }
        }

        private void ProcessPolygonWithMapping(IFeature feature, Polygon polygon, List<MapZone> mapZones, Dictionary<string, string> attributeMapping)
        {
            try
            {
                string Name = "Unnamed Zone";
                int RateA = 0;
                int RateB = 0;
                int RateC = 0;
                int RateD = 0;
                Color ZoneColor = Color.Blue;

                foreach (KeyValuePair<string, string> kvp in attributeMapping)
                {
                    try
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
                    catch (System.Exception ex)
                    {
                        mf.Tls.WriteErrorLog("ShapefileHelper/ProcessPolygonWithMapping: " + ex.Message);
                    }
                }
                Dictionary<string, int> rates = new Dictionary<string, int>();

                rates.Add("ProductA", RateA);
                rates.Add("ProductB", RateB);
                rates.Add("ProductC", RateC);
                rates.Add("ProductD", RateD);

                mapZones.Add(new MapZone(Name, polygon, rates, ZoneColor, mf)); // Pass color to MapZone
            }
            catch (System.Exception ex)
            {
                mf.Tls.WriteErrorLog("ShapefileHelper/ProcessPolygonWithMapping: " + ex.Message);
            }
        }
    }
}