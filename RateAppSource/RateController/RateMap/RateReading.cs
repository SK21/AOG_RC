using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.RateMap
{
    public class RateReading
    {
        public RateReading(DateTime timestamp, double latitude, double longitude, double[] appliedRates, double widthMeters, double elevation = 0.0, byte?[] rxModes = null)
        {
            Timestamp = timestamp;
            Latitude = latitude;
            Longitude = longitude;
            AppliedRates = appliedRates;
            ImplementWidthMeters = widthMeters;
            Elevation = elevation;
            RxModes = rxModes;
            if (ImplementWidthMeters < 0.1) ImplementWidthMeters = 0.1;
        }

        public double[] AppliedRates { get; }

        /// <summary>Per product, DDI 288 as an ISOBUS implement reports it (CSV RxMode1-5); null = not an ISOBUS
        /// product, or a reading loaded from a file (only written, not read back).</summary>
        public byte?[] RxModes { get; }
        public double Latitude { get; }
        public double Longitude { get; }
        public double ImplementWidthMeters { get; }
        public DateTime Timestamp { get; }
        public double Elevation { get; }
    }
}