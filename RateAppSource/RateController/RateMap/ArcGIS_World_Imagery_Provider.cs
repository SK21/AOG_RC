using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.Projections;
using RateController.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RateController.RateMap
{
    public class ArcGIS_World_Imagery_Provider : GMapProvider
    {
        public static readonly ArcGIS_World_Imagery_Provider Instance =
            new ArcGIS_World_Imagery_Provider();

        private const string TileUrlTemplate =
            "https://services.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{0}/{1}/{2}";

        private static readonly TimeSpan MinRequestInterval = TimeSpan.FromMilliseconds(120);
        private static readonly object ThrottleLock = new object();
        private static bool isTemporarilyUnavailable;
        private static DateTime lastFailure = DateTime.MinValue;
        private static DateTime LastRequest = DateTime.MinValue;

        public static bool IsTemporarilyUnavailable
        {
            get
            {
                lock (ThrottleLock)
                {
                    return isTemporarilyUnavailable;
                }
            }
        }

        public static DateTime LastFailure
        {
            get
            {
                lock (ThrottleLock)
                {
                    return lastFailure;
                }
            }
        }

        public override Guid Id => new Guid("F4E1A7A7-3B5D-4FDC-9C84-9B2390C7B04C");
        public override string Name => "ArcGISWorldImagery";
        public override GMapProvider[] Overlays => new GMapProvider[] { this };
        public override PureProjection Projection => MercatorProjection.Instance;

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            // Determine required delay without blocking the lock
            TimeSpan delay;

            lock (ThrottleLock)
            {
                var now = DateTime.Now;
                var elapsed = now - LastRequest;

                delay = elapsed < MinRequestInterval
                    ? MinRequestInterval - elapsed
                    : TimeSpan.Zero;

                // Reserve the next allowed request time
                LastRequest = now + delay;
            }

            // Sleep outside the lock to avoid blocking all tile threads
            if (delay > TimeSpan.Zero)
                Thread.Sleep(delay);

            string url = string.Format(TileUrlTemplate, zoom, pos.Y, pos.X);

            try
            {
                var image = GetTileImageUsingHttp(url);
                // if we got this far, consider provider healthy again
                if (image != null)
                {
                    isTemporarilyUnavailable = false;
                }
                return image;
            }
            catch (Exception ex)
            {
                lock (ThrottleLock)
                {
                    isTemporarilyUnavailable = true;
                    lastFailure = DateTime.Now;
                }
                Props.WriteErrorLog("ArcGIS_World_Imagery_Provider/GetTileImage: " + ex.Message);
                return null;
            }
        }
    }
}