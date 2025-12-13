using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GMap.NET;
using GMap.NET.WindowsForms;

namespace RateController.Classes
{
    public sealed class RateOverlayService
    {
        // Match CoverageTrail deadband so "0" means no coverage
        private const double RateEpsilon = 0.01;

        private readonly CoverageTrail _trail = new CoverageTrail();

        public bool BuildFromHistory(
            GMapOverlay overlay,
            IReadOnlyList<RateReading> readings,
            double implementWidthMeters,
            RateType legendType,
            int rateIndex,
            out Dictionary<string, Color> legend)
        {
            legend = new Dictionary<string, Color>();
            try
            {
                if (overlay == null || readings == null || readings.Count < 2) return false;
                if (implementWidthMeters <= 0) implementWidthMeters = 0.01;

                int maxLen = readings.Max(r => (r.AppliedRates?.Length ?? 0));
                if (maxLen == 0) return false;
                if (rateIndex >= maxLen) rateIndex = 0;

                // Choose the series (applied or target) for scale computation based on legend type
                IEnumerable<double> baseSeries = (legendType == RateType.Applied)
                    ? readings.Where(r => r.AppliedRates.Length > rateIndex).Select(r => r.AppliedRates[rateIndex])
                    : readings.Where(r => r.TargetRates.Length > rateIndex).Select(r => r.TargetRates[rateIndex]);

                if (!TryComputeScale(baseSeries, out double minRate, out double maxRate))
                    return false;

                _trail.Reset();
                PointLatLng prevPoint = new PointLatLng(readings[0].Latitude, readings[0].Longitude);
                DateTime prevTime = readings[0].Timestamp;

                // Relocation / gap guards (mirrors live logic + time gap)
                const double maxSnapMeters = 5.0;          // break if spatial gap exceeds this
                const double MaxGapSeconds = 3.0;          // break if temporal gap (record interval ~1s) too large

                for (int i = 0; i < readings.Count; i++)
                {
                    var r = readings[i];
                    var currPoint = new PointLatLng(r.Latitude, r.Longitude);
                    double heading = i == 0 ? 0.0 : BearingDegrees(prevPoint, currPoint);

                    // Rate value chosen by legend type for coloring; bridging decision also uses this
                    double rateValue = legendType == RateType.Applied
                        ? (r.AppliedRates.Length > rateIndex ? r.AppliedRates[rateIndex] : 0.0)
                        : (r.TargetRates.Length > rateIndex ? r.TargetRates[rateIndex] : 0.0);

                    bool canBridge = rateValue > RateEpsilon;
                    if (i > 0)
                    {
                        double distToPrev = DistanceMeters(currPoint, prevPoint.Lat, prevPoint.Lng);
                        if (distToPrev > maxSnapMeters) canBridge = false;

                        TimeSpan gap = r.Timestamp - prevTime;
                        if (gap.TotalSeconds > MaxGapSeconds) canBridge = false;
                    }

                    if (canBridge)
                    {
                        _trail.AddPoint(currPoint, heading, rateValue, implementWidthMeters);
                    }
                    else
                    {
                        _trail.Break();
                    }

                    prevPoint = currPoint;
                    prevTime = r.Timestamp;
                }

                _trail.DrawTrail(overlay, minRate, maxRate);

                // Keep legend: shades derived from AOG color
                legend = _trail.CreateLegend(minRate, maxRate, 5);
                return true;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog($"RateOverlayService.BuildFromHistory: {ex.Message}");
                return false;
            }
        }

        public void Reset() => _trail.Reset();

        public bool UpdateRatesOverlayLive(
                    GMapOverlay overlay,
            IReadOnlyList<RateReading> readings,
            PointLatLng tractorPos,
            double headingDegrees,
            double implementWidthMeters,
            double? appliedOverride,
            out Dictionary<string, Color> legend,
            RateType legendType,
            int rateIndex)
        {
            legend = new Dictionary<string, Color>();

            try
            {
                if (overlay == null) return false;
                if (readings == null || readings.Count == 0) return false;
                if (implementWidthMeters <= 0) implementWidthMeters = 0.01;

                var last = readings.Last();

                // Select series for scale computation based on legend type
                IEnumerable<double> baseSeries = (legendType == RateType.Applied)
                    ? readings.Where(r => r.AppliedRates.Length > rateIndex).Select(r => r.AppliedRates[rateIndex])
                    : readings.Where(r => r.TargetRates.Length > rateIndex).Select(r => r.TargetRates[rateIndex]);

                if (!TryComputeScale(baseSeries, out double minRate, out double maxRate))
                    return false;

                // Choose current value based on legend type; appliedOverride only applies to Applied view
                double currValue;
                if (legendType == RateType.Applied)
                {
                    double appliedBase = (last.AppliedRates.Length > rateIndex) ? last.AppliedRates[rateIndex] : 0.0;
                    currValue = appliedOverride ?? appliedBase;
                }
                else
                {
                    currValue = (last.TargetRates.Length > rateIndex) ? last.TargetRates[rateIndex] : 0.0;
                }

                // Distance gate prevents trail bridging after relocation
                const double maxSnapMeters = 5.0;
                double distToLast = DistanceMeters(tractorPos, last.Latitude, last.Longitude);

                if (currValue > RateEpsilon && distToLast <= maxSnapMeters)
                {
                    _trail.AddPoint(tractorPos, headingDegrees, currValue, implementWidthMeters);
                }
                else
                {
                    _trail.Break();
                }

                _trail.DrawTrail(overlay, minRate, maxRate);

                // Keep legend: shades derived from AOG color
                legend = _trail.CreateLegend(minRate, maxRate, 5);

                return true;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog($"RateOverlayService.UpdateRatesOverlayLive: {ex.Message}");
                return false;
            }
        }

        private static double BearingDegrees(PointLatLng a, PointLatLng b)
        {
            double lat1 = a.Lat * Math.PI / 180.0;
            double lat2 = b.Lat * Math.PI / 180.0;
            double dLon = (b.Lng - a.Lng) * Math.PI / 180.0;

            double y = Math.Sin(dLon) * Math.Cos(lat2);
            double x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);
            double brng = Math.Atan2(y, x) * 180.0 / Math.PI;
            return (brng + 360.0) % 360.0;
        }

        private static double DistanceMeters(PointLatLng a, double bLat, double bLng)
        {
            const double metersPerDegLat = 111320.0;
            double latRad = a.Lat * Math.PI / 180.0;
            double metersPerDegLng = metersPerDegLat * Math.Cos(latRad);

            double dx = (bLng - a.Lng) * metersPerDegLng;
            double dy = (bLat - a.Lat) * metersPerDegLat;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        // Robust scale estimator: 2nd–98th percentiles over non-zero values.
        private static bool TryComputeScale(IEnumerable<double> values, out double minRate, out double maxRate)
        {
            var vals = values
                .Where(v => v > RateEpsilon && !double.IsNaN(v) && !double.IsInfinity(v))
                .OrderBy(v => v)
                .ToArray();

            minRate = 0; maxRate = 0;
            if (vals.Length == 0) return false;

            if (vals.Length < 10)
            {
                minRate = vals.First();
                maxRate = vals.Last();
            }
            else
            {
                int loIdx = (int)Math.Floor(0.02 * (vals.Length - 1));
                int hiIdx = (int)Math.Ceiling(0.98 * (vals.Length - 1));
                minRate = vals[loIdx];
                maxRate = vals[hiIdx];

                if (maxRate <= minRate)
                {
                    minRate = vals.First();
                    maxRate = vals.Last();
                }
            }

            if (Math.Abs(maxRate - minRate) < 1e-9)
            {
                double pad = Math.Max(0.05 * maxRate, 1.0);
                minRate = Math.Max(0, maxRate - pad);
                maxRate = maxRate + pad;
            }
            return true;
        }
    }
}