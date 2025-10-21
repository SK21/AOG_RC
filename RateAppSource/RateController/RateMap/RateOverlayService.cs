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

                // Get selected rate
                double currRate = 0.0;
                if (rateType == RateType.Applied && last.AppliedRates.Length > rateIndex)
                    currRate = last.AppliedRates[rateIndex];
                else if (rateType == RateType.Target && last.TargetRates.Length > rateIndex)
                    currRate = last.TargetRates[rateIndex];

                // Add latest point to trail
                _trail.AddPoint(tractorPos, headingDegrees, currRate, implementWidthMeters);

                // Determine min/max across available data for legend/color mapping
                IEnumerable<double> series = (rateType == RateType.Applied)
                    ? readings.Where(r => r.AppliedRates.Length > rateIndex).Select(r => r.AppliedRates[rateIndex])
                    : readings.Where(r => r.TargetRates.Length > rateIndex).Select(r => r.TargetRates[rateIndex]);

                if (!series.Any()) return false;

                double minRate = series.Min();
                double maxRate = series.Max();

                // Render
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

        // NEW: Rebuild from saved readings so historical data is visible
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

                // Clamp rate index to available series length
                int maxLen = readings.Max(r => rateType == RateType.Applied ? (r.AppliedRates?.Length ?? 0) : (r.TargetRates?.Length ?? 0));
                if (maxLen == 0) return false;
                if (rateIndex >= maxLen) rateIndex = 0;

                // Compute min/max across the full history
                IEnumerable<double> series = (rateType == RateType.Applied)
                    ? readings.Where(r => r.AppliedRates.Length > rateIndex).Select(r => r.AppliedRates[rateIndex])
                    : readings.Where(r => r.TargetRates.Length > rateIndex).Select(r => r.TargetRates[rateIndex]);

                if (!series.Any()) return false;
                double minRate = series.Min();
                double maxRate = series.Max();

                // Rebuild trail
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

                    _trail.AddPoint(curr, heading, rate, implementWidthMeters);
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
    }
}