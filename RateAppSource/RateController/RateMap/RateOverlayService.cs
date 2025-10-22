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
        private readonly CoverageTrail _trail = new CoverageTrail();
        private const double RateEpsilon = 1e-6;

        public bool UpdateRatesOverlay(
            GMapOverlay overlay,
            IReadOnlyList<RateReading> readings,
            PointLatLng tractorPos,
            double headingDegrees,
            double implementWidthMeters,
            out Dictionary<string, Color> legend,
            RateType rateType,
            int rateIndex)
        {
            legend = new Dictionary<string, Color>();

            try
            {
                if (overlay == null) return false;
                if (readings == null || readings.Count == 0) return false;
                if (implementWidthMeters <= 0) implementWidthMeters = 0.01;

                var last = readings.Last();

                // Current selected rate
                double currRate = 0.0;
                if (rateType == RateType.Applied && last.AppliedRates.Length > rateIndex)
                    currRate = last.AppliedRates[rateIndex];
                else if (rateType == RateType.Target && last.TargetRates.Length > rateIndex)
                    currRate = last.TargetRates[rateIndex];

                // Build series for scaling (exclude zero/near-zero and invalids)
                IEnumerable<double> baseSeries = (rateType == RateType.Applied)
                    ? readings.Where(r => r.AppliedRates.Length > rateIndex).Select(r => r.AppliedRates[rateIndex])
                    : readings.Where(r => r.TargetRates.Length > rateIndex).Select(r => r.TargetRates[rateIndex]);

                if (!TryComputeScale(baseSeries, out double minRate, out double maxRate))
                    return false;

                // Add only non-zero rate points to the trail
                if (currRate > RateEpsilon)
                {
                    _trail.AddPoint(tractorPos, headingDegrees, currRate, implementWidthMeters);
                }

                // Render with robust min/max
                _trail.DrawTrail(overlay, minRate, maxRate);
                legend = _trail.CreateLegend(minRate, maxRate);

                return true;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog($"RateOverlayService: {ex.Message}");
                return false;
            }
        }

        public bool BuildFromHistory(
            GMapOverlay overlay,
            IReadOnlyList<RateReading> readings,
            double implementWidthMeters,
            RateType rateType,
            int rateIndex,
            out Dictionary<string, Color> legend)
        {
            legend = new Dictionary<string, Color>();
            try
            {
                if (overlay == null || readings == null || readings.Count < 2) return false;
                if (implementWidthMeters <= 0) implementWidthMeters = 0.01;

                int maxLen = readings.Max(r => rateType == RateType.Applied ? (r.AppliedRates?.Length ?? 0) : (r.TargetRates?.Length ?? 0));
                if (maxLen == 0) return false;
                if (rateIndex >= maxLen) rateIndex = 0;

                IEnumerable<double> baseSeries = (rateType == RateType.Applied)
                    ? readings.Where(r => r.AppliedRates.Length > rateIndex).Select(r => r.AppliedRates[rateIndex])
                    : readings.Where(r => r.TargetRates.Length > rateIndex).Select(r => r.TargetRates[rateIndex]);

                if (!TryComputeScale(baseSeries, out double minRate, out double maxRate))
                    return false;

                _trail.Reset();
                PointLatLng prev = new PointLatLng(readings[0].Latitude, readings[0].Longitude);
                for (int i = 0; i < readings.Count; i++)
                {
                    var r = readings[i];
                    var curr = new PointLatLng(r.Latitude, r.Longitude);
                    double heading = i == 0 ? 0.0 : BearingDegrees(prev, curr);

                    double rate = 0.0;
                    if (rateType == RateType.Applied && r.AppliedRates.Length > rateIndex)
                        rate = r.AppliedRates[rateIndex];
                    else if (rateType == RateType.Target && r.TargetRates.Length > rateIndex)
                        rate = r.TargetRates[rateIndex];

                    if (rate > RateEpsilon)
                    {
                        _trail.AddPoint(curr, heading, rate, implementWidthMeters);
                    }
                    prev = curr;
                }

                _trail.DrawTrail(overlay, minRate, maxRate);
                legend = _trail.CreateLegend(minRate, maxRate);
                return true;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog($"RateOverlayService.BuildFromHistory: {ex.Message}");
                return false;
            }
        }

        public void Reset() => _trail.Reset();

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

            // If collapsed, pad by 5% (or at least 1 unit) for visible gradient
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