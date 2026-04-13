using GMap.NET;
using GMap.NET.WindowsForms;
using RateController.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace RateController.RateMap
{
    public class FieldSample
    {
        public FieldSample(DateTime timestamp, double latitude, double longitude, double Yield, double widthMeters, double elevation = 0.0, double ecValue = 0.0)
        {
            Timestamp = timestamp;
            Latitude = latitude;
            Longitude = longitude;
            YieldKg = Yield;
            WidthMeters = widthMeters;
            ElevationMeters = elevation;
            EcValue = ecValue;
        }

        public double EcValue { get; }
        public double ElevationMeters { get; }
        public double Latitude { get; }
        public double Longitude { get; }
        public DateTime Timestamp { get; }
        public double WidthMeters { get; }
        public double YieldKg { get; }
    }

    public class YieldOverlayCreator : IDisposable
    {
        public const string CSVheader = "Timestamp,Latitude,Longitude,WidthMeters,YieldKg,ElevationMeters";
        public readonly List<FieldSample> FieldData = new List<FieldSample>();
        private const double NearZero = 0.01;
        private readonly GMapControl gmap;
        private readonly CoverageTrail Trail = new CoverageTrail();
        private bool cDisposed = false;
        private bool cEnabled = false;
        private LegendManager YieldLegend;
        private GMapOverlay YieldOverlay;

        public YieldOverlayCreator(GMapControl map)
        {
            gmap = map;
            YieldOverlay = new GMapOverlay("YieldOverlay");
            cEnabled = bool.TryParse(Props.GetProp("MapShowYield"), out bool sh) ? sh : false;
            YieldLegend = new LegendManager(map, true);
            JobManager.JobChanged += JobManager_JobChanged;
            Core.ProfileChanged += Core_ProfileChanged;
            ParcelManager.YieldSelectionChanged += ParcelManager_YieldSelectionChanged;
            LoadData();
        }

        public bool Enabled
        {
            get { return cEnabled; }
            set
            {
                cEnabled = value;
                Props.SetProp("MapShowYield", cEnabled.ToString());

                if (cEnabled)
                {
                    Build();
                }
                else
                {
                    Reset();
                }
            }
        }

        /// <summary>
        /// Generates a synthetic yield CSV covering the given lat/lng bounding box.
        /// Yield ranges from minYield (SW corner) to maxYield (NE corner) with small random noise.
        /// Simulates a harvester making E-W passes at 9.144 m (30 ft) width, 6 km/h.
        /// </summary>
        public static void GenerateTestData(
            string yieldPath,
            double minLat, double maxLat,
            double minLng, double maxLng,
            double minYield = 1800.0, double maxYield = 2400.0)
        {
            const double widthM = 9.144;
            const double speedMs = 1.667;          // 6 km/h
            const double metersPerDegLat = 111320.0;

            double midLat = (minLat + maxLat) / 2.0;
            double metersPerDegLng = metersPerDegLat * Math.Cos(midLat * Math.PI / 180.0);

            double fieldHeightM = (maxLat - minLat) * metersPerDegLat;
            double fieldWidthM  = (maxLng - minLng) * metersPerDegLng;
            double fieldHeightDeg = maxLat - minLat;
            double fieldWidthDeg  = maxLng - minLng;

            int passCount = Math.Max(1, (int)Math.Ceiling(fieldHeightM / widthM));
            double lngStepDeg = speedMs / metersPerDegLng;   // lng degrees per sample

            var sb = new StringBuilder();
            sb.AppendLine(CSVheader);

            var rng = new Random(42);
            var time = new DateTime(2026, 9, 15, 8, 0, 0, DateTimeKind.Utc);

            for (int pass = 0; pass < passCount; pass++)
            {
                double passLat = minLat + (pass + 0.5) * widthM / metersPerDegLat;
                if (passLat > maxLat) break;

                bool goingEast = (pass % 2 == 0);
                double startLng = goingEast ? minLng : maxLng;
                double endLng   = goingEast ? maxLng : minLng;
                double sign     = goingEast ? 1.0 : -1.0;

                double normLat = (passLat - minLat) / fieldHeightDeg;

                double lng = startLng;
                while (goingEast ? lng <= endLng : lng >= endLng)
                {
                    double normLng = (lng - minLng) / fieldWidthDeg;
                    double pos = (normLat + normLng) / 2.0;   // 0=SW, 1=NE

                    double noise = (rng.NextDouble() - 0.5) * 100.0;  // ±50
                    double yield = minYield + pos * (maxYield - minYield) + noise;
                    yield = Math.Max(minYield - 50, Math.Min(maxYield + 50, yield));

                    sb.AppendLine(string.Format(CultureInfo.InvariantCulture,
                        "{0:O},{1:F7},{2:F7},{3:F3},{4:F1},0.0",
                        time, passLat, lng, widthM, yield));

                    time = time.AddSeconds(1);
                    lng += sign * lngStepDeg;
                }

                // Turn-around gap — triggers Trail.Break() in Build()
                time = time.AddSeconds(30);
            }

            Directory.CreateDirectory(Path.GetDirectoryName(yieldPath));
            File.WriteAllText(yieldPath, sb.ToString());
        }

        public void Build()
        {
            try
            {
                var data = FieldData.Where(r => r.YieldKg > NearZero).Select(r => r.YieldKg).ToList();

                if (FieldData.Count > 0 && data.Count > 0 && MapController.TryComputeScale(data, out double MinYield, out double MaxYield))
                {
                    YieldLegend.SetYieldScale(MinYield, MaxYield);

                    Trail.Reset();

                    int startIndex = FieldData.FindIndex(r => r.YieldKg > NearZero);
                    if (startIndex >= 0)
                    {
                        var first = FieldData[startIndex];
                        PointLatLng prevPoint = new PointLatLng(first.Latitude, first.Longitude);
                        DateTime prevTime = first.Timestamp;

                        // Relocation / gap guards
                        const double maxSnapMeters = 5.0;          // break if spatial gap exceeds this
                        const double MaxGapSeconds = 3.0;          // break if temporal gap (record interval ~1s) too large

                        for (int i = startIndex; i < FieldData.Count; i++)
                        {
                            var sample = FieldData[i];
                            var currPoint = new PointLatLng(sample.Latitude, sample.Longitude);
                            double heading = (i == 0) ? 0.0 : BearingDegrees(prevPoint, currPoint);

                            double yield = sample.YieldKg;
                            bool canBridge = yield > NearZero;

                            if (i > 0)
                            {
                                double distToPrev = DistanceMeters(currPoint, prevPoint.Lat, prevPoint.Lng);
                                if (distToPrev > maxSnapMeters) canBridge = false;

                                TimeSpan gap = sample.Timestamp - prevTime;
                                if (gap.TotalSeconds > MaxGapSeconds) canBridge = false;
                            }

                            if (canBridge)
                            {
                                Trail.AddPoint(currPoint, heading, yield, sample.WidthMeters);
                            }
                            else
                            {
                                Trail.Break();
                            }

                            prevPoint = currPoint;
                            prevTime = sample.Timestamp;
                        }

                        List<double> AveYield = new List<double>();
                        Trail.DrawTrail(YieldOverlay, out AveYield, true, true, YieldLegend);

                        MapController.AddOverlay(YieldOverlay);
                        gmap.Refresh();

                        //YieldLegend.CreateLegendOld(MinYield, MaxYield, 5, true);
                    }
                    else
                    {
                        Reset();
                    }
                }
                else
                {
                    Reset();
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("YieldOverlayCreator/Build: " + ex.Message);
            }
        }

        public void Dispose()
        {
            if (cDisposed)
            {
                return;
            }
            cDisposed = true;

            try
            {
                JobManager.JobChanged -= JobManager_JobChanged;
                Core.ProfileChanged -= Core_ProfileChanged;
                ParcelManager.YieldSelectionChanged -= ParcelManager_YieldSelectionChanged;

                Reset();

                if (YieldLegend != null)
                {
                    YieldLegend.Dispose();
                    YieldLegend = null;
                }

                YieldOverlay = null;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("YieldOverlayCreator/Dispose: " + ex.Message);
            }
        }

        public void LoadData()
        {
            try
            {
                FieldData.Clear();

                string path = ParcelManager.SelectedYieldPath(JobManager.CurrentJob?.FieldID ?? -1);
                if (FileValid(path))
                {
                    string[] allLines = File.ReadAllLines(path);

                    // parse the lines (skip header)
                    for (int i = 1; i < allLines.Length; i++)
                    {
                        string line = allLines[i];
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        string[] parts = line.Split(',');

                        // expecting 6 parts: Timestamp, Lat, Lon, width, yield, elevation
                        if (parts.Length < 6)
                            continue;

                        DateTime timestamp;
                        double latitude, longitude;

                        if (!DateTime.TryParse(parts[0], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out timestamp) ||
                        !double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out latitude) ||
                        !double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out longitude))
                        {
                            continue;
                        }

                        // yield
                        double yield = double.TryParse(parts[4], NumberStyles.Number, CultureInfo.InvariantCulture, out double yd) ? yd : 0;

                        // implement width
                        double width = double.TryParse(parts[3], NumberStyles.Number, CultureInfo.InvariantCulture, out double wd) ? wd : 0;
                        if (width < 0.1) width = Core.Sections.TotalWidth();  // fall back to current implement width

                        // Parse elevation
                        double elevation = double.TryParse(parts[5], NumberStyles.Number, CultureInfo.InvariantCulture, out double el) ? el : 0;

                        var data = new FieldSample(timestamp, latitude, longitude, yield, width, elevation);
                        FieldData.Add(data);
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("YieldOverlayCreator/LoadData: " + ex.Message);
            }
        }

        public void Reset()
        {
            YieldLegend?.Hide();
            MapController.RemoveOverlay(YieldOverlay);
            gmap.Refresh();
        }

        private double BearingDegrees(PointLatLng a, PointLatLng b)
        {
            double lat1 = a.Lat * Math.PI / 180.0;
            double lat2 = b.Lat * Math.PI / 180.0;
            double dLon = (b.Lng - a.Lng) * Math.PI / 180.0;

            double y = Math.Sin(dLon) * Math.Cos(lat2);
            double x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);
            double brng = Math.Atan2(y, x) * 180.0 / Math.PI;
            return (brng + 360.0) % 360.0;
        }

        private void Core_ProfileChanged(object sender, EventArgs e)
        {
            LoadData();
            if (cEnabled) Build();
        }

        private double DistanceMeters(PointLatLng a, double bLat, double bLng)
        {
            const double metersPerDegLat = 111320.0;
            double latRad = a.Lat * Math.PI / 180.0;
            double metersPerDegLng = metersPerDegLat * Math.Cos(latRad);

            double dx = (bLng - a.Lng) * metersPerDegLng;
            double dy = (bLat - a.Lat) * metersPerDegLat;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private bool FileValid(string DataFilePath = null)
        {
            bool Result = false;
            try
            {
                if (DataFilePath != null && File.Exists(DataFilePath))
                {
                    string firstLine = null;
                    foreach (string line in File.ReadLines(DataFilePath))
                    {
                        firstLine = line;
                        break;
                    }

                    if (!string.IsNullOrWhiteSpace(firstLine) && firstLine.StartsWith("Timestamp", StringComparison.OrdinalIgnoreCase)) Result = true;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("YieldOverlayCreator/FileValid: " + ex.Message);
            }
            return Result;
        }

        private void JobManager_JobChanged(object sender, EventArgs e)
        {
            LoadData();
            if (cEnabled) Build();
        }

        private void ParcelManager_YieldSelectionChanged(object sender, EventArgs e)
        {
            LoadData();
            if (cEnabled) Build();
        }
    }
}