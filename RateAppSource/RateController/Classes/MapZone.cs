using GMap.NET;
using GMap.NET.WindowsForms;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Drawing;

namespace RateController.Classes
{
    public class MapZone
    {
        public MapZone(string name, Polygon geometry, Dictionary<string, int> rates, Color zoneColor)
        {
            Name = name;
            Geometry = geometry;
            Rates = rates;
            ZoneColor = zoneColor; // Initialize color
        }

        public Polygon Geometry { get; set; }
        public string Name { get; set; }
        public Dictionary<string, int> Rates { get; set; }
        public Color ZoneColor { get; set; } 

        public bool Contains(PointLatLng point)
        {
            var coordinate = new Coordinate(point.Lng, point.Lat);
            var pointGeometry = new NetTopologySuite.Geometries.Point(coordinate);
            return Geometry.Contains(pointGeometry);
        }

        public GMapPolygon ToGMapPolygon()
        {
            var points = new List<PointLatLng>();
            foreach (var coord in Geometry.Coordinates)
            {
                points.Add(new PointLatLng(coord.Y, coord.X));
            }

            return new GMapPolygon(points, Name)
            {
                Stroke = new Pen(ZoneColor, 2), // Use the zone color
                Fill = new SolidBrush(Color.FromArgb(50, ZoneColor))
            };
        }
    }
}