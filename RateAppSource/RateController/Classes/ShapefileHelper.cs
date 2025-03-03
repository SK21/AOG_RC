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