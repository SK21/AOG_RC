using GMap.NET;
using GMap.NET.WindowsForms;
using NetTopologySuite.Geometries;
using ProjNet.CoordinateSystems.Transformations;
using ProjNet.CoordinateSystems;
using System.Collections.Generic;
using System.Drawing;
using System;

namespace RateController.Classes
{
    public class MapZone
    {
        public MapZone(string name, Polygon geometry, Dictionary<string, int> rates, Color zoneColor)
        {
            Name = name;
            Geometry = geometry;
            Rates = rates;
            ZoneColor = zoneColor;
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

        public double Hectares()
        {
            return CalculateArea(Geometry);
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

        private double CalculateArea(Polygon polygon)
        {
            // Determine the UTM zone from the first vertex
            int utmZone = (int)(((polygon.Coordinates[0].X + 180) / 6) + 1);

            // Define the source and target coordinate systems
            var geographicCS = GeographicCoordinateSystem.WGS84;
            var utmCS = ProjectedCoordinateSystem.WGS84_UTM(utmZone, true); 

            // Create coordinate transformation
            var transformationFactory = new CoordinateTransformationFactory();
            var transform = transformationFactory.CreateFromCoordinateSystems(geographicCS, utmCS);

            // Transform the polygon to the projected coordinate system
            var transformedCoordinates = new Coordinate[polygon.Coordinates.Length];
            for (int i = 0; i < polygon.Coordinates.Length; i++)
            {
                double[] xy = { polygon.Coordinates[i].X, polygon.Coordinates[i].Y };
                double[] transformedXY = transform.MathTransform.Transform(xy);

                transformedCoordinates[i] = new Coordinate(transformedXY[0], transformedXY[1]);
            }

            var projectedPolygon = new Polygon(new LinearRing(transformedCoordinates));

            // Calculate area in square meters
            var areaSqMeters = projectedPolygon.Area;

            // Convert to hectares
            var areaHectares = areaSqMeters / 10000;
            return areaHectares;
        }
    }
}