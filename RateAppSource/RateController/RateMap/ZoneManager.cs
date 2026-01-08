using GMap.NET;
using GMap.NET.WindowsForms;
using NetTopologySuite.Geometries;
using RateController.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.RateMap
{
    public class ZoneManager
    {
        private static Dictionary<string, Color> cAppliedLegend;
        private readonly CoverageTrail Trail = new CoverageTrail();
        private GMapOverlay cAppliedOverlay;
        private GMapOverlay cTargetOverlay;
        private const double NearZero = 0.01;

        public ZoneManager()
        {
            cAppliedOverlay = new GMapOverlay("AppliedRates");
            cTargetOverlay = new GMapOverlay("TargetRates");
            cAppliedLegend = new Dictionary<string, Color>();
        }

        public Dictionary<string, Color> AppliedLegend
        { get { return cAppliedLegend; } }

        public GMapOverlay AppliedOverlay
        { get { return cAppliedOverlay; } }

        public GMapOverlay TargetOverlay
        { get { return cTargetOverlay; } }

        public bool BuildAppliedFromHistory(GMapOverlay overlay, out Dictionary<string, Color> legend)
        {
            legend = new Dictionary<string, Color>();
            try
            {
                MapController.RateCollector.LoadData(); // ensure fresh data
                var readings = MapController.RateCollector.GetReadings();
                if (overlay == null || readings == null || readings.Count < 2) return false;

                int maxLen = readings.Max(r => (r.AppliedRates?.Length ?? 0));
                if (maxLen == 0) return false;

                int ProductFilter = MapController.ProductFilter;
                if (ProductFilter >= maxLen) ProductFilter = 0;

                IEnumerable<double> baseSeries = readings.Where(r => r.AppliedRates.Length > ProductFilter).Select(r => r.AppliedRates[ProductFilter]);

                if (!MapController.TryComputeScale(baseSeries, out double minRate, out double maxRate))
                    return false;

                Trail.Reset();
                PointLatLng prevPoint = new PointLatLng(readings[0].Latitude, readings[0].Longitude);
                DateTime prevTime = readings[0].Timestamp;

                // Relocation / gap guards (mirrors live logic + time gap)
                const double maxSnapMeters = 5.0;          // break if spatial gap exceeds this
                const double MaxGapSeconds = 3.0;          // break if temporal gap (record interval ~1s) too large

                for (int i = 0; i < readings.Count; i++)
                {
                    var r = readings[i];
                    var currPoint = new PointLatLng(r.Latitude, r.Longitude);
                    double heading = i == 0 ? 0.0 : BearingDegrees(prevPoint, currPoint);

                    double rateValue = (r.AppliedRates.Length > ProductFilter ? r.AppliedRates[ProductFilter] : 0.0);

                    bool canBridge = rateValue > NearZero;
                    if (i > 0)
                    {
                        double distToPrev = DistanceMeters(currPoint, prevPoint.Lat, prevPoint.Lng);
                        if (distToPrev > maxSnapMeters) canBridge = false;

                        TimeSpan gap = r.Timestamp - prevTime;
                        if (gap.TotalSeconds > MaxGapSeconds) canBridge = false;
                    }

                    if (canBridge)
                    {
                        Trail.AddPoint(currPoint, heading, rateValue, r.ImplementWidthMeters);
                    }
                    else
                    {
                        Trail.Break();
                    }

                    prevPoint = currPoint;
                    prevTime = r.Timestamp;
                }

                Trail.DrawTrail(overlay, minRate, maxRate);

                // After drawing the trail, assign applied rates to each polygon for shapefile export
                if (overlay.Polygons.Count > 0)
                {
                    int polygonCount = overlay.Polygons.Count;
                    int readingCount = readings.Count;
                    int maxProducts = readings.Max(r => r.AppliedRates?.Length ?? 0);

                    // Map polygons to readings (approximate: one polygon per segment)
                    for (int i = 0; i < polygonCount; i++)
                    {
                        // Use the i-th reading, or last if out of range
                        int readingIdx = Math.Min(i, readingCount - 1);
                        var rates = new Dictionary<string, double>
                        {
                            { "ProductA", (readings[readingIdx].AppliedRates.Length > 0) ? readings[readingIdx].AppliedRates[0] : 0.0 },
                            { "ProductB", (readings[readingIdx].AppliedRates.Length > 1) ? readings[readingIdx].AppliedRates[1] : 0.0 },
                            { "ProductC", (readings[readingIdx].AppliedRates.Length > 2) ? readings[readingIdx].AppliedRates[2] : 0.0 },
                            { "ProductD", (readings[readingIdx].AppliedRates.Length > 3) ? readings[readingIdx].AppliedRates[3] : 0.0 }
                        };
                        overlay.Polygons[i].Tag = rates;
                    }
                }
                legend = LegendManager.CreateAppliedLegend(minRate, maxRate, 5);
                return true;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog($"ZoneManager/BuildFromHistory: {ex.Message}");
                return false;
            }
        }

        public bool BuildNewAppliedZones(out List<MapZone> NewAppliedZones)
        {
            bool Result = false;
            NewAppliedZones = new List<MapZone>();

            try
            {
                // check for recorded data
                bool RecordedData = false;
                for (int i = 0; i < 4; i++)
                {
                    if (MapController.RateCollector.DataPoints(i) > 0)
                    {
                        RecordedData = true;
                        break;
                    }
                }

                if (RecordedData)
                {
                    Dictionary<string, Color> histLegend;
                    GMapOverlay AppliedOverlay = new GMapOverlay();

                    bool histOk = BuildAppliedFromHistory(AppliedOverlay, out histLegend);
                    if (histOk && AppliedOverlay.Polygons.Count > 0)
                    {
                        int count = 0;
                        foreach (var polygon in AppliedOverlay.Polygons)
                        {
                            Color zoneColor = Color.AliceBlue;
                            if (polygon.Fill is SolidBrush sb)
                            {
                                zoneColor = Color.FromArgb(255, sb.Color);
                            }

                            NewAppliedZones.Add(new MapZone(
                                name: $"Applied Zone {count++}",
                                geometry: ConvertToNtsPolygon(polygon),
                                rates: (Dictionary<string, double>)polygon.Tag,
                                zoneColor: zoneColor,
                                zoneType: ZoneType.Applied));
                        }
                        Result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ZoneManager/BuildNewAppliedZones: " + ex.Message);
            }

            return Result;
        }

        public void Close()
        {
            cAppliedOverlay = null;
            cTargetOverlay = null;
            ResetTrail();
        }

        public void ResetTrail() => Trail.Reset();

        public bool UpdateTrail(GMapOverlay overlay, IReadOnlyList<RateReading> readings, PointLatLng tractorPos, double headingDegrees,
            double implementWidthMeters, double? appliedOverride, out Dictionary<string, Color> legend, int rateIndex)
        {
            legend = new Dictionary<string, Color>();

            try
            {
                if (overlay == null) return false;
                if (readings == null || readings.Count == 0) return false;
                if (implementWidthMeters <= 0) implementWidthMeters = 0.01;

                var last = readings.Last();

                IEnumerable<double> baseSeries = readings.Where(r => r.AppliedRates.Length > rateIndex).Select(r => r.AppliedRates[rateIndex]);

                if (!MapController.TryComputeScale(baseSeries, out double minRate, out double maxRate))
                    return false;

                double currValue;
                double appliedBase = (last.AppliedRates.Length > rateIndex) ? last.AppliedRates[rateIndex] : 0.0;
                currValue = appliedOverride ?? appliedBase;

                // Distance gate prevents trail bridging after relocation
                const double maxSnapMeters = 5.0;
                double distToLast = DistanceMeters(tractorPos, last.Latitude, last.Longitude);

                if (currValue > NearZero && distToLast <= maxSnapMeters)
                {
                    Trail.AddPoint(tractorPos, headingDegrees, currValue, implementWidthMeters);
                }
                else
                {
                    Trail.Break();
                }

                Trail.DrawTrail(overlay, minRate, maxRate);

                legend = LegendManager.CreateAppliedLegend(minRate, maxRate, 5);

                return true;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog($"ZoneManager/UpdateRatesOverlayLive: {ex.Message}");
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

        private static double DistanceMeters(PointLatLng a, double bLat, double bLng)
        {
            const double metersPerDegLat = 111320.0;
            double latRad = a.Lat * Math.PI / 180.0;
            double metersPerDegLng = metersPerDegLat * Math.Cos(latRad);

            double dx = (bLng - a.Lng) * metersPerDegLng;
            double dy = (bLat - a.Lat) * metersPerDegLat;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private Polygon ConvertToNtsPolygon(GMapPolygon gmapPolygon)
        {
            var coords = new List<Coordinate>();
            foreach (var point in gmapPolygon.Points)
            {
                coords.Add(new Coordinate(point.Lng, point.Lat));
            }
            // Ensure closed
            if (coords.Count > 0 && !coords[0].Equals(coords[coords.Count - 1]))
            {
                coords.Add(coords[0]);
            }
            return new Polygon(new LinearRing(coords.ToArray()));
        }
    }
}