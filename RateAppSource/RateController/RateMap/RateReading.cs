using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.RateMap
{
    public class RateReading
    {
        public RateReading(DateTime timestamp, double latitude, double longitude, double[] appliedRates, double widthMeters, double elevation = 0.0)
        {
            Timestamp = timestamp;
            Latitude = latitude;
            Longitude = longitude;
            AppliedRates = appliedRates;
            ImplementWidthMeters = widthMeters;
            Elevation = elevation;
            if (ImplementWidthMeters < 0.1) ImplementWidthMeters = 0.1;
        }

        public double[] AppliedRates { get; }
        public double Latitude { get; }
        public double Longitude { get; }
        public double ImplementWidthMeters { get; }
        public DateTime Timestamp { get; }
        public double Elevation { get; }
    }
}