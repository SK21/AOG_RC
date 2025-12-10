using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.Projections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.RateMap
{
    public class ArcGIS_World_Imagery_Provider : GMapProvider
    {
        public static readonly ArcGIS_World_Imagery_Provider Instance = new ArcGIS_World_Imagery_Provider();

        private static readonly TimeSpan MinRequestInterval = TimeSpan.FromMilliseconds(120);
        private static readonly object ThrottleLock = new object();
        private static DateTime _lastRequestUtc = DateTime.MinValue;

        public override Guid Id => new Guid("F4E1A7A7-3B5D-4FDC-9C84-9B2390C7B04C");
        public override string Name => "ArcGISWorldImagery";
        public override GMapProvider[] Overlays => new GMapProvider[] { this };
        public override PureProjection Projection => MercatorProjection.Instance;

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            // Throttle to avoid server bans
            lock (ThrottleLock)
            {
                var now = DateTime.UtcNow;
                var elapsed = now - _lastRequestUtc;

                if (elapsed < MinRequestInterval)
                    System.Threading.Thread.Sleep(MinRequestInterval - elapsed);

                _lastRequestUtc = DateTime.UtcNow;
            }

            string url =
                $"https://services.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{zoom}/{pos.Y}/{pos.X}";

            return GetTileImageUsingHttp(url);
        }
    }
}
