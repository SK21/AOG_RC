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
        private const double RateEpsilon = 1e-6;
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

                // Legend series per selection
                IEnumerable<double> baseSeries = (legendType == RateType.Applied)
                    ? readings.Where(r => r.AppliedRates.Length > rateIndex).Select(r => r.AppliedRates[rateIndex])
                    : readings.Where(r => r.TargetRates.Length > rateIndex).Select(r => r.TargetRates[rateIndex]);

                if (!TryComputeScale(baseSeries, out double minRate, out double maxRate))
                    return false;

                // Rebuild trail from Applied only, skipping zeroes
                _trail.Reset();
                PointLatLng prev = new PointLatLng(readings[0].Latitude, readings[0].Longitude);
                for (int i = 0; i < readings.Count; i++)
                {
                    var r = readings[i];
                    var curr = new PointLatLng(r.Latitude, r.Longitude);
                    double heading = i == 0 ? 0.0 : BearingDegrees(prev, curr);

                    double applied = (r.AppliedRates.Length > rateIndex) ? r.AppliedRates[rateIndex] : 0.0;
                    if (applied > RateEpsilon)
                    {
                        _trail.AddPoint(curr, heading, applied, implementWidthMeters);
                    }
                    prev = curr;
                }

                _trail.DrawTrail(overlay, minRate, maxRate);
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

        public bool UpdateRatesOverlay(
                            GMapOverlay overlay,
            IReadOnlyList<RateReading> readings,
            PointLatLng tractorPos,
            double headingDegrees,
            double implementWidthMeters,
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

                // Legend series follows user selection (Applied or Target)
                IEnumerable<double> baseSeries = (legendType == RateType.Applied)
                    ? readings.Where(r => r.AppliedRates.Length > rateIndex).Select(r => r.AppliedRates[rateIndex])
                    : readings.Where(r => r.TargetRates.Length > rateIndex).Select(r => r.TargetRates[rateIndex]);

                if (!TryComputeScale(baseSeries, out double minRate, out double maxRate))
                    return false;

                // Coverage is always drawn based on Applied > 0
                double currApplied = (last.AppliedRates.Length > rateIndex) ? last.AppliedRates[rateIndex] : 0.0;
                if (currApplied > RateEpsilon)
                {
                    _trail.AddPoint(tractorPos, headingDegrees, currApplied, implementWidthMeters);
                }

                _trail.DrawTrail(overlay, minRate, maxRate);
                legend = _trail.CreateLegend(minRate, maxRate, 5);

                return true;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog($"RateOverlayService: {ex.Message}");
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