using GMap.NET;
using GMap.NET.WindowsForms;
using RateController.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.RateMap
{
    public class CoverageTrail
    {
        private const double NearZero = 0.01;
        private readonly object _lock = new object();
        private readonly List<Segment> _segments = new List<Segment>();
        private bool _hasPrev;
        private int _lastDrawnSegmentIndex = 0;
        private PointLatLng _prev;
        private int _runId;

        public void AddPoint(PointLatLng pos, double headingDeg, double rate, double implementWidthMeters)
        {
            lock (_lock)
            {
                if (rate <= NearZero)
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

        public void DrawTrail(GMapOverlay overlay, out List<double> PolyRates, bool RebuildPolygons = false, bool IsYieldData = false)
        {
            List<Segment> segmentsToDraw;

            if (RebuildPolygons || MapController.legendManager.LegendChanged)
            {
                MapController.legendManager.ResetLegendChanged();
                overlay.Polygons.Clear();
                segmentsToDraw = _segments.Where(s => s.Rate > NearZero).ToList();
                _lastDrawnSegmentIndex = _segments.Count;
            }
            else
            {
                segmentsToDraw = _segments
                    .Skip(_lastDrawnSegmentIndex)
                    .Where(s => s.Rate > NearZero)
                    .ToList();

                _lastDrawnSegmentIndex = _segments.Count;
            }

            PolyRates = new List<double>();
            if (overlay == null) return;

            lock (_lock)
            {
                var runs = segmentsToDraw
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
                        int j = i + 1;

                        int BandIndex = MapController.legendManager.BandIndex(segs[i].Rate);

                        while (j < segs.Count && MapController.legendManager.BandIndex(segs[j].Rate) == BandIndex)
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

                        var baseColor = Palette.GetColor(BandIndex, IsYieldData);

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

        public void UpdateTrail(GMapOverlay overlay, IReadOnlyList<RateReading> readings, PointLatLng tractorPos, double headingDegrees,
            double implementWidthMeters, double? appliedOverride, int rateIndex)
        {
            try
            {
                if (overlay != null && readings != null && readings.Count > 0)
                {
                    if (implementWidthMeters <= 0) implementWidthMeters = 0.01;

                    var last = readings.Last();

                    double currValue;
                    double appliedBase = (last.AppliedRates.Length > rateIndex) ? last.AppliedRates[rateIndex] : 0.0;
                    currValue = appliedOverride ?? appliedBase;

                    // Distance gate prevents trail bridging after relocation
                    const double maxSnapMeters = 5.0;
                    double distToLast = DistanceMeters(tractorPos, last.Latitude, last.Longitude);

                    if (currValue > NearZero && distToLast <= maxSnapMeters)
                    {
                        AddPoint(tractorPos, headingDegrees, currValue, implementWidthMeters);
                    }
                    else
                    {
                        Break();
                    }

                    List<double> temp = new List<double>();
                    DrawTrail(overlay, out temp);
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("CoverageTrail/UpdateTrail: " + ex.Message);
            }
        }

        private static PointLatLng Offset(PointLatLng src, double eastMeters, double northMeters, double metersPerDegLng, double metersPerDegLat)
        {
            double dLng = eastMeters / metersPerDegLng;
            double dLat = northMeters / metersPerDegLat;
            return new PointLatLng(src.Lat + dLat, src.Lng + dLng);
        }

        private void ComputeCorners(PointLatLng prev, PointLatLng curr, double widthMeters, out PointLatLng prevLeft,
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

        private double DistanceMeters(PointLatLng a, double bLat, double bLng)
        {
            const double metersPerDegLat = 111320.0;
            double latRad = a.Lat * Math.PI / 180.0;
            double metersPerDegLng = metersPerDegLat * Math.Cos(latRad);

            double dx = (bLng - a.Lng) * metersPerDegLng;
            double dy = (bLat - a.Lat) * metersPerDegLat;
            return Math.Sqrt(dx * dx + dy * dy);
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
