using GMap.NET;
using GMap.NET.WindowsForms;
using RateController.RateMap;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace RateController.Classes
{
    public class CoverageTrail
    {
        // Use same epsilon as RateOverlayService
        private const double ZeroRate = 0.001;

        private readonly object _lock = new object();

        private readonly List<Segment> _segments = new List<Segment>();

        private bool _hasPrev;

        private PointLatLng _prev;

        private int _runId;

        public void AddPoint(PointLatLng pos, double headingDeg, double rate, double implementWidthMeters)
        {
            lock (_lock)
            {
                if (rate <= ZeroRate)
                {
                    _hasPrev = false;
                    _prev = pos;
                    return;
                }

                if (!_hasPrev)
                {
                    _prev = pos;
                    _hasPrev = true;
                    return;
                }

                if (implementWidthMeters <= 0)
                {
                    implementWidthMeters = 0.01;
                }

                var prev = _prev;
                var curr = pos;

                const double metersPerDegLat = 111320.0;
                double latRad = prev.Lat * Math.PI / 180.0;
                double metersPerDegLng = metersPerDegLat * Math.Cos(latRad);

                double dxMeters = (curr.Lng - prev.Lng) * metersPerDegLng;
                double dyMeters = (curr.Lat - prev.Lat) * metersPerDegLat;
                double distMeters = Math.Sqrt(dxMeters * dxMeters + dyMeters * dyMeters);

                // Large-gap break: if we jumped too far, start a new run and don't bridge.
                double maxGapMeters = 5.0; // tweak as needed
                if (distMeters > maxGapMeters)
                {
                    _hasPrev = false;
                    _prev = curr;
                    _runId++;
                    return;
                }

                // Very small movement: synthesize a minimal segment along heading
                if (distMeters < 0.01)
                {
                    double synth = Math.Max(implementWidthMeters * 0.02, 0.02);
                    double theta = headingDeg * Math.PI / 180.0;
                    double east = Math.Sin(theta);
                    double north = Math.Cos(theta);
                    double dLng = (synth * east) / metersPerDegLng;
                    double dLat = (synth * north) / metersPerDegLat;
                    curr = new PointLatLng(prev.Lat + dLat, prev.Lng + dLng);
                }

                PointLatLng prevLeft;
                PointLatLng prevRight;
                PointLatLng currLeft;
                PointLatLng currRight;
                ComputeCorners(prev, curr, implementWidthMeters, out prevLeft, out prevRight, out currLeft, out currRight);

                _segments.Add(new Segment
                {
                    Prev = prev,
                    Curr = curr,
                    Rate = rate,
                    ImplementWidthMeters = implementWidthMeters,
                    PrevLeft = prevLeft,
                    PrevRight = prevRight,
                    CurrLeft = currLeft,
                    CurrRight = currRight,
                    RunId = _runId
                });

                _prev = pos;
            }
        }

        public void Break()
        {
            lock (_lock)
            {
                _hasPrev = false;
                _runId++;
            }
        }

        public void DrawTrail(GMapOverlay overlay, double minRate, double maxRate, out List<double> PolyRates)
        {
            PolyRates = new List<double>();
            if (overlay == null) return;

            lock (_lock)
            {
                overlay.Polygons.Clear();

                // Build banded ribbons to keep rate shading while preventing overlap/striping
                const int steps = 5;

                var runs = _segments
                    .Where(s => s.Rate > ZeroRate)
                    .GroupBy(s => s.RunId)
                    .OrderBy(g => g.Key);

                foreach (var run in runs)
                {
                    var segs = run.ToList();
                    if (segs.Count == 0) continue;

                    // Walk contiguous segments grouped by band index
                    int i = 0;
                    while (i < segs.Count)
                    {
                        double RateTotal = segs[i].Rate;
                        int count = 1;

                        int band = BandIndex(minRate, maxRate, segs[i].Rate, steps);
                        int j = i + 1;
                        while (j < segs.Count && BandIndex(minRate, maxRate, segs[j].Rate, steps) == band)
                        {
                            RateTotal += segs[j].Rate;
                            j++;
                            count++;
                        }

                        PolyRates.Add(RateTotal / count);

                        // i..(j-1) is one contiguous band group
                        var leftChain = new List<PointLatLng>();
                        var rightChain = new List<PointLatLng>();

                        leftChain.Add(segs[i].PrevLeft);
                        rightChain.Add(segs[i].PrevRight);
                        for (int k = i; k < j; k++)
                        {
                            leftChain.Add(segs[k].CurrLeft);
                            rightChain.Add(segs[k].CurrRight);
                        }
                        rightChain.Reverse();

                        var polyPoints = new List<PointLatLng>(leftChain.Count + rightChain.Count + 1);
                        polyPoints.AddRange(leftChain);
                        polyPoints.AddRange(rightChain);
                        polyPoints.Add(leftChain[0]);

                        var baseColor = Palette.GetColor(band);

                        var poly = new GMapPolygon(polyPoints, "swath_band")
                        {
                            Stroke = Pens.Transparent,
                            Fill = new SolidBrush(baseColor)
                        };
                        overlay.Polygons.Add(poly);

                        i = j;
                    }
                }
            }
        }

        public void Reset()
        {
            lock (_lock)
            {
                _segments.Clear();
                _hasPrev = false;
                _prev = new PointLatLng(0, 0);
                _runId = 0;
            }
        }

        private static int BandIndex(double minRate, double maxRate, double value, int steps)
        {
            if (maxRate <= minRate) return 0;
            double t = (value - minRate) / (maxRate - minRate);
            if (t < 0)
            {
                t = 0;
            }
            else if (t > 1)
            {
                t = 1;
            }

            int idx = (int)Math.Floor(t * steps);
            if (idx == steps) idx = steps - 1;
            return idx;
        }

        private static void ComputeCorners(PointLatLng prev, PointLatLng curr, double widthMeters, out PointLatLng prevLeft,
            out PointLatLng prevRight, out PointLatLng currLeft, out PointLatLng currRight)
        {
            const double metersPerDegLat = 111320.0;
            double latRad = prev.Lat * Math.PI / 180.0;
            double metersPerDegLng = metersPerDegLat * Math.Cos(latRad);

            double dxMeters = (curr.Lng - prev.Lng) * metersPerDegLng;
            double dyMeters = (curr.Lat - prev.Lat) * metersPerDegLat;
            double len = Math.Sqrt(dxMeters * dxMeters + dyMeters * dyMeters);
            if (len < 1e-6) len = 1e-6;

            double ux = dxMeters / len;
            double uy = dyMeters / len;
            double px = -uy;
            double py = ux;
            double halfWidth = Math.Max(widthMeters / 2.0, 0.005);

            prevLeft = Offset(prev, px * halfWidth, py * halfWidth, metersPerDegLng, metersPerDegLat);
            prevRight = Offset(prev, -px * halfWidth, -py * halfWidth, metersPerDegLng, metersPerDegLat);
            currLeft = Offset(curr, px * halfWidth, py * halfWidth, metersPerDegLng, metersPerDegLat);
            currRight = Offset(curr, -px * halfWidth, -py * halfWidth, metersPerDegLng, metersPerDegLat);
        }

        private static PointLatLng Offset(PointLatLng src, double eastMeters, double northMeters, double metersPerDegLng, double metersPerDegLat)
        {
            double dLng = eastMeters / metersPerDegLng;
            double dLat = northMeters / metersPerDegLat;
            return new PointLatLng(src.Lat + dLat, src.Lng + dLng);
        }

        private sealed class Segment
        {
            public PointLatLng Curr;
            public PointLatLng CurrLeft;
            public PointLatLng CurrRight;
            public double ImplementWidthMeters;
            public PointLatLng Prev;
            public PointLatLng PrevLeft;
            public PointLatLng PrevRight;
            public double Rate;
            public int RunId;
        }
    }
}