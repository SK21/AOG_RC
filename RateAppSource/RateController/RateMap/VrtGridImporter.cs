using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Union;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using RateController.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace RateController.RateMap
{
    // Format-agnostic input: a georeferenced grid of per-cell rates.
    // Populated by a format-specific parser (e.g. AgGrowXmlParser) before
    // passing to VrtGridImporter.Import().
    public class VrtGrid
    {
        public double OriginEasting;    // West edge of grid, normalised UTM metres
        public double OriginNorthing;   // North edge of grid, normalised UTM metres
        public double CellSizeX;        // metres per cell, east direction
        public double CellSizeY;        // metres per cell, south direction
        public int CellsX;
        public int CellsY;
        public int UtmZone;
        // Rates[row * CellsX + col][productIndex 0..3], matching ZoneFields.Products order.
        public double[][] Rates;
    }

    public static class VrtGridImporter
    {
        private static readonly GeometryFactory GF = new GeometryFactory();

        public static List<MapZone> Import(VrtGrid grid)
        {
            var result = new List<MapZone>();
            try
            {
                var transform = BuildTransform(grid.UtmZone);

                // Group cell indices by rate combination; skip all-zero cells.
                var groups = new Dictionary<string, List<int>>();
                int total = grid.CellsY * grid.CellsX;
                for (int i = 0; i < total; i++)
                {
                    double[] rates = grid.Rates[i];
                    if (rates == null || rates.All(r => r == 0)) continue;
                    string key = RateKey(rates);
                    if (!groups.TryGetValue(key, out List<int> list))
                    {
                        list = new List<int>();
                        groups[key] = list;
                    }
                    list.Add(i);
                }

                int zoneIndex = 0;
                foreach (var kvp in groups)
                {
                    // Build cell polygons in UTM (metres). Unioning in projected space avoids
                    // the floating-point gaps that occur when tiny WGS84 rectangles are unioned
                    // — adjacent cells in UTM share exact edge coordinates and merge correctly.
                    var cellPolygons = new List<Geometry>(kvp.Value.Count);
                    foreach (int idx in kvp.Value)
                    {
                        int col = idx % grid.CellsX;
                        int row = idx / grid.CellsX;
                        cellPolygons.Add(MakeCellPolygonUtm(grid, col, row));
                    }

                    Geometry mergedUtm = null;
                    try { mergedUtm = CascadedPolygonUnion.Union(cellPolygons); }
                    catch { }
                    if (mergedUtm == null || mergedUtm.IsEmpty) continue;

                    // Convert the merged UTM geometry to WGS84 for MapZone storage.
                    Geometry mergedWgs84 = TransformToWgs84(mergedUtm, transform);
                    if (mergedWgs84 == null || mergedWgs84.IsEmpty) continue;

                    double[] groupRates = ParseKey(kvp.Key);
                    Dictionary<string, double> rateDict = BuildRateDict(groupRates);
                    Color color = Palette.GetColor(zoneIndex, trns: 255);

                    int countBefore = result.Count;
                    AddZones(result, mergedWgs84, rateDict, color, zoneIndex);
                    if (result.Count > countBefore) zoneIndex++;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("VrtGridImporter/Import: " + ex.Message);
            }
            return result;
        }

        public static List<MapZone> ImportCells(VrtGrid grid)
        {
            var result = new List<MapZone>();
            try
            {
                var transform = BuildTransform(grid.UtmZone);
                for (int row = 0; row < grid.CellsY; row++)
                {
                    for (int col = 0; col < grid.CellsX; col++)
                    {
                        int i = row * grid.CellsX + col;
                        double[] rates = grid.Rates[i];
                        if (rates == null || rates.All(r => r == 0)) continue;
                        var utmPoly = MakeCellPolygonUtm(grid, col, row);
                        var wgsPoly = TransformPolygon(utmPoly, transform);
                        if (wgsPoly == null || wgsPoly.IsEmpty) continue;
                        result.Add(new MapZone("Cell", wgsPoly, BuildRateDict(rates), Color.Gray, ZoneType.Target));
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("VrtGridImporter/ImportCells: " + ex.Message);
            }
            return result;
        }

        private static MathTransform BuildTransform(int utmZone)
        {
            var utmCS = ProjectedCoordinateSystem.WGS84_UTM(utmZone, true);
            var geoCS = GeographicCoordinateSystem.WGS84;
            return new CoordinateTransformationFactory()
                .CreateFromCoordinateSystems(utmCS, geoCS).MathTransform;
        }

        private static Polygon MakeCellPolygonUtm(VrtGrid grid, int col, int row)
        {
            double e0 = grid.OriginEasting + col * grid.CellSizeX;
            double e1 = e0 + grid.CellSizeX;
            double n1 = grid.OriginNorthing - row * grid.CellSizeY;
            double n0 = n1 - grid.CellSizeY;

            var coords = new[]
            {
                new Coordinate(e0, n0),  // SW
                new Coordinate(e1, n0),  // SE
                new Coordinate(e1, n1),  // NE
                new Coordinate(e0, n1),  // NW
                new Coordinate(e0, n0)   // close
            };
            return GF.CreatePolygon(coords);
        }

        private static Geometry TransformToWgs84(Geometry geom, MathTransform transform)
        {
            if (geom is Polygon poly)
                return TransformPolygon(poly, transform);

            if (geom is MultiPolygon mp)
            {
                var polys = new Polygon[mp.NumGeometries];
                for (int i = 0; i < mp.NumGeometries; i++)
                    polys[i] = TransformPolygon((Polygon)mp.GetGeometryN(i), transform);
                return GF.CreateMultiPolygon(polys);
            }

            return null;
        }

        private static Polygon TransformPolygon(Polygon utmPoly, MathTransform transform)
        {
            var extRing = TransformRing((LinearRing)utmPoly.ExteriorRing, transform);
            var holes = new LinearRing[utmPoly.NumInteriorRings];
            for (int i = 0; i < utmPoly.NumInteriorRings; i++)
                holes[i] = TransformRing((LinearRing)utmPoly.GetInteriorRingN(i), transform);
            return GF.CreatePolygon(extRing, holes);
        }

        private static LinearRing TransformRing(LinearRing utmRing, MathTransform transform)
        {
            var coords = utmRing.Coordinates.Select(c =>
            {
                double[] pt = transform.Transform(new[] { c.X, c.Y });
                return new Coordinate(pt[0], pt[1]); // lon, lat
            }).ToArray();
            return GF.CreateLinearRing(coords);
        }

        private static string RateKey(double[] rates)
            => string.Join("|", rates.Select(r => ((long)Math.Round(r * 10)).ToString()));

        private static double[] ParseKey(string key)
            => key.Split('|').Select(s => long.Parse(s) / 10.0).ToArray();

        private static Dictionary<string, double> BuildRateDict(double[] rates)
        {
            var d = new Dictionary<string, double>();
            for (int p = 0; p < ZoneFields.Products.Length; p++)
                d[ZoneFields.Products[p]] = p < rates.Length ? rates[p] : 0.0;
            return d;
        }

        private static void AddZones(List<MapZone> zones, Geometry merged,
            Dictionary<string, double> rates, Color color, int index)
        {
            string name = "Zone " + (index + 1);
            if (merged is Polygon poly)
            {
                zones.Add(new MapZone(name, poly, rates, color, ZoneType.Target));
            }
            else if (merged is MultiPolygon mp)
            {
                for (int i = 0; i < mp.NumGeometries; i++)
                {
                    if (mp.GetGeometryN(i) is Polygon p)
                        zones.Add(new MapZone(name + "_" + (i + 1), p,
                            new Dictionary<string, double>(rates), color, ZoneType.Target));
                }
            }
        }
    }
}
