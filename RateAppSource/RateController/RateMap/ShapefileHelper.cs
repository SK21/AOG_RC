using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri;
using System;
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
                Props.WriteErrorLog("ShapefileHelper/LoadAndMapShapefile: " + ex.Message);
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
                Props.WriteErrorLog("ShapefileHelper/SaveMapZones: " + ex.Message);
            }
            return Result;
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

                NewZone = new MapZone(Name, polygon, rates, ZoneColor, mf);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ShapefileHelper/CreateMapZone: " + ex.Message);
            }
            return NewZone;
        }
    }
}