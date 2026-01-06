using System;

namespace RateController.Classes
{
    public class RateReading
    {
        public RateReading(DateTime timestamp, double latitude, double longitude, double[] appliedRates, double WidthMeters)
        {
            Timestamp = timestamp;
            Latitude = latitude;
            Longitude = longitude;
            AppliedRates = appliedRates;
            ImplementWidthMeters = WidthMeters;
            if (ImplementWidthMeters < 0.1) ImplementWidthMeters = 0.1;
        }

        public double[] AppliedRates { get; }
        public double Latitude { get; }
        public double Longitude { get; }
        public double ImplementWidthMeters { get; }
        public DateTime Timestamp { get; }
    }
}