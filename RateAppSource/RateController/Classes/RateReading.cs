using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.Classes
{
    public class RateReading
    {
        public RateReading(DateTime timestamp, double latitude, double longitude, double[] appliedRates, double[] targetRates)
        {
            Timestamp = timestamp;
            Latitude = latitude;
            Longitude = longitude;
            AppliedRates = appliedRates;
            TargetRates = targetRates;
        }

        public double[] AppliedRates { get; }
        public double Latitude { get; }
        public double Longitude { get; }
        public double[] TargetRates { get; }
        public DateTime Timestamp { get; }
    }
}
