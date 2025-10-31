using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Microsoft.VSDiagnostics;

namespace RateController.Benchmarks
{
    [CPUUsageDiagnoser]
    // Compare LINQ-heavy vs. single-pass bounding box algorithms on synthetic polygon data
    public class MapBoundingBoxBenchmarks
    {
        public struct LatLng
        {
            public double Lat;
            public double Lng;
            public LatLng(double lat, double lng) { Lat = lat; Lng = lng; }
        }

        public sealed class Polygon
        {
            public List<LatLng> Points { get; }
            public Polygon(List<LatLng> points) { Points = points; }
        }

        private List<Polygon> _polygons;

        [GlobalSetup]
        public void Setup()
        {
            var rnd = new Random(42);
            _polygons = new List<Polygon>(400);

            double baseLat = 52.1;
            double baseLng = -106.7;

            for (int i = 0; i < 200; i++)
            {
                int points = 40 + rnd.Next(0, 40); // 40..80 points
                var pts = new List<LatLng>(points);
                double cx = baseLat + (rnd.NextDouble() - 0.5) * 0.1; // ~11km N/S span
                double cy = baseLng + (rnd.NextDouble() - 0.5) * 0.1; // ~7km E/W span
                double radius = 0.0005 + rnd.NextDouble() * 0.001;   // ~55..110m radius
                for (int k = 0; k < points; k++)
                {
                    double ang = (Math.PI * 2.0) * k / points;
                    double rr = radius * (0.8 + rnd.NextDouble() * 0.4); // some jaggedness
                    pts.Add(new LatLng(cx + Math.Sin(ang) * rr, cy + Math.Cos(ang) * rr));
                }
                _polygons.Add(new Polygon(pts));
            }
        }

        public readonly struct Rect
        {
            public readonly double Top;
            public readonly double Left;
            public readonly double Width;
            public readonly double Height;
            public Rect(double top, double left, double width, double height)
            {
                Top = top; Left = left; Width = width; Height = height;
            }
            public static readonly Rect Empty = new Rect(double.NaN, double.NaN, 0, 0);
        }

        [Benchmark(Baseline = true)]
        public Rect LinqBoundingBox()
        {
            double minLat = double.MaxValue;
            double maxLat = double.MinValue;
            double minLng = double.MaxValue;
            double maxLng = double.MinValue;

            foreach (var polygon in _polygons)
            {
                if (polygon.Points.Count == 0) continue;
                minLat = Math.Min(minLat, MinLat(polygon));
                maxLat = Math.Max(maxLat, MaxLat(polygon));
                minLng = Math.Min(minLng, MinLng(polygon));
                maxLng = Math.Max(maxLng, MaxLng(polygon));
            }

            return (minLat == double.MaxValue)
                ? Rect.Empty
                : new Rect(maxLat, minLng, maxLng - minLng, maxLat - minLat);

            // mimic current LINQ-based per-polygon Min/Max to be comparable
            double MinLat(Polygon p) => System.Linq.Enumerable.Min(p.Points, pt => pt.Lat);
            double MaxLat(Polygon p) => System.Linq.Enumerable.Max(p.Points, pt => pt.Lat);
            double MinLng(Polygon p) => System.Linq.Enumerable.Min(p.Points, pt => pt.Lng);
            double MaxLng(Polygon p) => System.Linq.Enumerable.Max(p.Points, pt => pt.Lng);
        }

        [Benchmark]
        public Rect SinglePassBoundingBox()
        {
            double minLat = double.MaxValue;
            double maxLat = double.MinValue;
            double minLng = double.MaxValue;
            double maxLng = double.MinValue;

            foreach (var polygon in _polygons)
            {
                var pts = polygon.Points;
                int count = pts.Count;
                for (int i = 0; i < count; i++)
                {
                    var pt = pts[i];
                    if (pt.Lat < minLat) minLat = pt.Lat;
                    if (pt.Lat > maxLat) maxLat = pt.Lat;
                    if (pt.Lng < minLng) minLng = pt.Lng;
                    if (pt.Lng > maxLng) maxLng = pt.Lng;
                }
            }

            return (minLat == double.MaxValue)
                ? Rect.Empty
                : new Rect(maxLat, minLng, maxLng - minLng, maxLat - minLat);
        }
    }
}