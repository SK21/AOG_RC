using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using RateController;

namespace RateController.Classes
{
    public class AppliedLayerCreator
    {
        // Make trail static to maintain state between updates
        private static readonly CoverageTrail trail = new CoverageTrail();
        private static bool isFirstPoint = true;

        /// <summary>
        /// Updates the GMapOverlay to display the coverage trail of the tractor.
        /// The trail is colored according to the rate values using a gradient scheme.
        /// </summary>
        /// <param name="overlay">GMapOverlay to update.</param>
        /// <param name="readings">List of RateReading data.</param>
        /// <param name="tractorPos">Current position of the tractor.</param>
        /// <param name="heading">Current heading of the tractor in degrees.</param>
        /// <param name="implementWidth">Width of the implement in meters.</param>
        /// <param name="legend">Output dictionary mapping rate ranges to colors.</param>
        /// <param name="selectedRateType">Specifies whether to use Applied or Target rates.</param>
        /// <param name="selectedRateIndex">Index (0‑based) of the rate to use.</param>
        /// <returns>True if update succeeds; otherwise, false.</returns>
        public bool UpdateRatesOverlay(
            ref GMapOverlay overlay,
            List<RateReading> readings,
            PointLatLng tractorPos,
            double heading,
            double implementWidth,
            out Dictionary<string, Color> legend,
            RateType selectedRateType = RateType.Applied,
            int selectedRateIndex = 0)
        {
            legend = new Dictionary<string, Color>();
            try
            {
                // Validate inputs
                if (readings == null || readings.Count == 0)
                {
                    Props.WriteErrorLog("AppliedLayerCreator: No readings available");
                    return false;
                }

                if (overlay == null)
                {
                    Props.WriteErrorLog("AppliedLayerCreator: Overlay is null");
                    return false;
                }

                // Get current reading and rate
                var currentReading = readings.Last();
                double rate = 0;

                if (selectedRateType == RateType.Applied && currentReading.AppliedRates.Length > selectedRateIndex)
                {
                    rate = currentReading.AppliedRates[selectedRateIndex];
                }
                else if (selectedRateType == RateType.Target && currentReading.TargetRates.Length > selectedRateIndex)
                {
                    rate = currentReading.TargetRates[selectedRateIndex];
                }

                // Add point to trail (even when rate is 0)
                trail.AddPoint(tractorPos, heading, rate, implementWidth);

                // Calculate rate range from all readings
                var rates = readings.Select(r => selectedRateType == RateType.Applied ? 
                    r.AppliedRates[selectedRateIndex] : r.TargetRates[selectedRateIndex]);

                double minRate = rates.Min();
                double maxRate = rates.Max();

                // Draw the trail
                trail.DrawTrail(overlay, minRate, maxRate);
                legend = trail.CreateLegend(minRate, maxRate);

                return true;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog($"AppliedLayerCreator Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Resets the coverage trail
        /// </summary>
        public void Reset()
        {
            trail.Reset();
            isFirstPoint = true;
        }
    }
}