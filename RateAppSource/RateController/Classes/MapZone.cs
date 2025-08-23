using GMap.NET;
using GMap.NET.WindowsForms;
using NetTopologySuite.Geometries;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace RateController.Classes
{
    public class MapZone
    {
        private FormStart mf;

        public MapZone(string name, Polygon geometry, Dictionary<string, double> rates, Color zoneColor, FormStart mf)
        {
            Name = name;
            Geometry = geometry;
            Rates = rates;
            ZoneColor = zoneColor;
            this.mf = mf;
        }

        public Polygon Geometry { get; set; }

        public string Name { get; set; }

        public Dictionary<string, double> Rates { get; set; }
        public Color ZoneColor { get; set; }

        public bool Contains(PointLatLng point)
        {
            var coordinate = new Coordinate(point.Lng, point.Lat);
            var pointGeometry = new NetTopologySuite.Geometries.Point(coordinate);
            return Geometry.Contains(pointGeometry);
        }

        public double Hectares()
        {
            double totalArea = 0;
            try
            {
                // Determine the UTM zone from the first vertex
                int utmZone = (int)(((Geometry.ExteriorRing.Coordinates[0].X + 180) / 6) + 1);

                // Define the source and target coordinate systems
                var geographicCS = GeographicCoordinateSystem.WGS84;
                var utmCS = ProjectedCoordinateSystem.WGS84_UTM(utmZone, true);

                // Create coordinate transformation
                var transformationFactory = new CoordinateTransformationFactory();
                var transform = transformationFactory.CreateFromCoordinateSystems(geographicCS, utmCS);

                // Transform the exterior polygon to the projected coordinate system
                var transformedExterior = TransformPolygon((LinearRing)Geometry.ExteriorRing, transform);
                totalArea = transformedExterior.Area;

                // Subtract the area of each interior ring (hole)
                for (int i = 0; i < Geometry.NumInteriorRings; i++)
                {
                    var transformedHole = TransformPolygon((LinearRing)Geometry.GetInteriorRingN(i), transform);
                    totalArea -= transformedHole.Area;
                }

                // Convert to hectares
                totalArea /= 10000;
            }
            catch (System.Exception ex)
            {
                mf.Tls.ShowMessage("MapZone.CalculateArea: " + ex.Message, "Help", 20000, true);
            }
            return totalArea;
        }

        public List<GMapPolygon> ToGMapPolygons()
        {
            var polygons = new List<GMapPolygon>();
            try
            {
                // Convert the outer boundary (shell) to a GMapPolygon
                var outerPoints = new List<PointLatLng>();
                foreach (var coord in Geometry.ExteriorRing.Coordinates)
                {
                    outerPoints.Add(new PointLatLng(coord.Y, coord.X));
                }

                // Ensure the polygon is closed
                if (!outerPoints[0].Equals(outerPoints[outerPoints.Count - 1]))
                {
                    outerPoints.Add(outerPoints[0]);
                }

                var outerPolygon = new GMapPolygon(outerPoints, Name)
                {
                    Stroke = new Pen(ZoneColor, 2),
                    Fill = new SolidBrush(Color.FromArgb(50, ZoneColor))
                };
                polygons.Add(outerPolygon);

                // Convert each interior ring (holes) to a GMapPolygon
                for (int i = 0; i < Geometry.NumInteriorRings; i++)
                {
                    var hole = Geometry.GetInteriorRingN(i);

                    // Validate interior ring
                    if (hole == null || hole.Coordinates.Length < 4)
                    {
                        continue; // Skip invalid holes
                    }

                    var holePoints = new List<PointLatLng>();
                    foreach (var coord in hole.Coordinates)
                    {
                        holePoints.Add(new PointLatLng(coord.Y, coord.X));
                    }

                    // Ensure the hole polygon is closed
                    if (!holePoints[0].Equals(holePoints[holePoints.Count - 1]))
                    {
                        holePoints.Add(holePoints[0]);
                    }

                    var holePolygon = new GMapPolygon(holePoints, Name + "_hole" + i)
                    {
                        Stroke = new Pen(ZoneColor, 2),
                        Fill = new SolidBrush(Color.Transparent) // Transparent to represent a hole
                    };
                    polygons.Add(holePolygon);
                }

            }
            catch (Exception ex)
            {
                mf.Tls.ShowMessage("MapZone.ToGMapPolygons: " + ex.Message, "Help", 20000, true);
            }
            return polygons;
        }

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