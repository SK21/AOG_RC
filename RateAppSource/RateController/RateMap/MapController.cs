using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using NetTopologySuite.Geometries;
using RateController.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

namespace RateController.RateMap
{
    public enum MapState
    { Preview, EditZones, Tracking, Positioning }

    public enum ZoneType
    {
        Target,
        Applied
    }

    // Preview - small preview window and tracking tractor position
    // EditZones - creating zones, no tracking
    // Tracking - following tractor position
    // Positioning - clicking on the map to check zones
    public static class MapController
    {
        #region GMap

        private static GMapControl gmap = new GMapControl();
        private static GMarkerGoogle tractorMarker;

        #endregion GMap

        #region Overlays

        private static GMapOverlay gpsMarkerOverlay;

        #endregion Overlays

        #region Saved Properties

        private static int cProductFilter;
        private static bool cShowTiles;

        #endregion Saved Properties

        public static readonly ZoneManager ZnOverlays = new ZoneManager();
        public static LegendManager legendManager;
        private static readonly KmlLayerManager kmlLayerManager = new KmlLayerManager();
        private static bool cMapIsDisplayed = false;
        private static DataCollector cRateCollector;
        private static MapState cState;
        private static PointLatLng cTractorPosition;
        private static double cTravelHeading;
        private static System.Windows.Forms.Timer UpdateTimer;

        public static event EventHandler MapChanged;

        public static event EventHandler MapLeftClicked;

        public static event EventHandler MapZoomed;

        public static bool EditingZones
        {
            get { return cState == MapState.EditZones; }
            set
            {
                if (value)
                {
                    cState = MapState.EditZones;
                }
                else
                {
                    cState = MapState.Tracking;
                }
            }
        }

        public static bool Enabled
        {
            get { return UpdateTimer.Enabled; }
            set { UpdateTimer.Enabled = value; }
        }

        public static GMapControl Map
        { get { return gmap; } }

        public static bool MapIsDisplayed
        { get { return cMapIsDisplayed; } set { cMapIsDisplayed = value; } }

        public static bool Positioning
        {
            get { return cState == MapState.Positioning; }
            set
            {
                if (value)
                {
                    cState = MapState.Positioning;
                }
                else
                {
                    cState = MapState.Tracking;
                }
            }
        }

        public static int ProductFilter
        {
            get { return cProductFilter; }
            set
            {
                if (value >= 0 && (value < Props.MaxProducts - 2))
                {
                    cProductFilter = value;
                    Props.SetProp("MapProductFilter", cProductFilter.ToString());
                    ZnOverlays.ShowAppliedOverlay();
                }
            }
        }

        public static DataCollector RateCollector
        { get { return cRateCollector; } }

        public static bool ShowTiles
        {
            get { return cShowTiles; }
            set
            {
                cShowTiles = value;
                Props.SetProp("MapShowTiles", cShowTiles.ToString());

                gmap.MapProvider = value
                    ? (GMapProvider)ArcGIS_World_Imagery_Provider.Instance
                    : (GMapProvider)GMapProviders.EmptyProvider;
                gmap.Refresh();
            }
        }

        public static MapState State
        { get { return cState; } }

        public static PointLatLng TractorPosition
        { get { return cTractorPosition; } }

        public static double TravelHeading
        { get { return cTravelHeading; } }

        // Add a KML layer to the map from a file path.
        public static bool AddKmlLayer(string filePath)
        {
            try
            {
                var jobCopyPath = PersistKmlToJob(filePath) ?? filePath;
                var overlay = kmlLayerManager.LoadKml(jobCopyPath);
                if (overlay == null) return false;

                AddOverlay(overlay);

                // ensure KML overlays are visible immediately
                SetKmlVisibility(true);
                Props.SetProp("KmlVisible", "True");

                CenterMap();
                gmap.Refresh();
                MapChanged?.Invoke(null, EventArgs.Empty);
                return true;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/AddKmlLayer: " + ex.Message);
                return false;
            }
        }

        public static void AddOverlay(GMapOverlay NewOverlay)
        {
            if (NewOverlay != null)
            {
                bool overlayExists = gmap.Overlays.Any(o => o.Id == NewOverlay.Id);
                if (!overlayExists) gmap.Overlays.Add(NewOverlay);
                legendManager.EnsureLegendTop();
            }
        }

        public static void CenterMap()
        {
            if (!ZoomToFit())
            {
                try
                {
                    var collector = cRateCollector;
                    if (collector != null)
                    {
                        var readings = collector.GetReadings();
                        if (readings != null && readings.Count > 0)
                        {
                            const double eps = 0.1; // consider >0.1 as applied
                            double minLat = double.MaxValue;
                            double maxLat = double.MinValue;
                            double minLng = double.MaxValue;
                            double maxLng = double.MinValue;
                            bool any = false;
                            foreach (var r in readings)
                            {
                                bool hasApplied = (r.AppliedRates != null && r.AppliedRates.Any(a => a > eps));
                                if (!hasApplied) continue;
                                any = true;
                                if (r.Latitude < minLat) minLat = r.Latitude;
                                if (r.Latitude > maxLat) maxLat = r.Latitude;
                                if (r.Longitude < minLng) minLng = r.Longitude;
                                if (r.Longitude > maxLng) maxLng = r.Longitude;
                            }
                            if (any)
                            {
                                double width = Math.Max(maxLng - minLng, 0.0008);   // ensure reasonable width
                                double height = Math.Max(maxLat - minLat, 0.0006);  // ensure reasonable height
                                                                                    // pad a little
                                double padW = width * 0.15;
                                double padH = height * 0.15;
                                var rect = new RectLatLng(maxLat + padH, minLng - padW, width + 2 * padW, height + 2 * padH);

                                gmap.SetZoomToFitRect(rect);
                                MapChanged?.Invoke(null, EventArgs.Empty);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("MapController/CenterMap: " + ex.Message);
                }
            }
        }

        public static void ClearAppliedRatesOverlay()
        {
            try
            {
                ZnOverlays.ResetTrail();
                ZnOverlays.AppliedOverlay.Polygons.Clear();
                legendManager?.Clear();
                gmap.Refresh();
                MapChanged?.Invoke(null, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/ClearAppliedRatesOverlay: " + ex.Message);
            }
        }

        public static void Close()
        {
            try
            {
                Props.SetProp("LastMapLat", gmap.Position.Lat.ToString("N10"));
                Props.SetProp("LastMapLng", gmap.Position.Lng.ToString("N10"));

                UpdateTimer?.Stop();
                UpdateTimer.Tick -= UpdateTimer_Tick;

                cRateCollector.SaveData();
                cRateCollector = null;

                JobManager.JobChanged -= JobManager_JobChanged;

                if (gmap != null)
                {
                    gmap.OnMapZoomChanged -= Gmap_OnMapZoomChanged;
                    gmap.MouseClick -= Gmap_MouseClick;
                }

                GMaps.Instance.CancelTileCaching();

                legendManager?.Dispose();
                legendManager = null;
                ZnOverlays.Close();

                if (gmap != null)
                {
                    gmap.Overlays.Clear();
                }

                gpsMarkerOverlay = null;

                gmap?.Dispose();
                gmap = null;

                MapChanged = null;
                MapLeftClicked = null;
                MapZoomed = null;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/Close: " + ex.Message);
            }
        }

        public static void DeleteKmlLayer(string filePath)
        {
            try
            {
                var jobDir = System.IO.Directory.Exists(JobManager.CurrentMapPath)
                    ? JobManager.CurrentMapPath
                    : System.IO.Path.GetDirectoryName(JobManager.CurrentMapPath);

                var fileNameOnly = System.IO.Path.GetFileName(filePath);
                var jobFull = string.IsNullOrWhiteSpace(jobDir) ? null : System.IO.Path.Combine(jobDir, fileNameOnly);

                var overlay = kmlLayerManager.GetOverlay(jobFull ?? filePath);
                if (overlay == null) return;

                RemoveOverlay(overlay);
                kmlLayerManager.Remove(jobFull ?? filePath);

                var current = Props.GetProp("KmlJobFiles");
                var list = new List<string>(string.IsNullOrWhiteSpace(current) ? Array.Empty<string>() : current.Split('|'));
                list.RemoveAll(p => string.Equals(p, fileNameOnly, StringComparison.OrdinalIgnoreCase));
                Props.SetProp("KmlJobFiles", string.Join("|", list));

                // Delete the physical KML in the job folder (safe delete)
                if (!string.IsNullOrWhiteSpace(jobFull) &&
                    System.IO.File.Exists(jobFull) &&
                    Props.IsPathSafe(jobFull))
                {
                    try
                    {
                        System.IO.File.Delete(jobFull);
                    }
                    catch (Exception delEx)
                    {
                        Props.WriteErrorLog("MapController/RemoveKmlLayer delete: " + delEx.Message);
                    }
                }

                gmap.Refresh();
                MapChanged?.Invoke(null, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/RemoveKmlLayer: " + ex.Message);
            }
        }

        public static void DisplaySizeUpdate(bool Preview)
        {
            if (Preview)
            {
                cState = MapState.Preview;
            }
            else
            {
                cState = MapState.Tracking;
            }
            legendManager.ShowLegend();
        }

        public static double GetRate(int RateID)
        {
            double Result = 0.0;

            if (RateID >= 0 && RateID < ZoneFields.Products.Length)
            {
                string productKey = ZoneFields.Products[RateID];
                if (CurrentZone.Zone.Rates.TryGetValue(productKey, out double value)) Result = value;
            }

            return Result;
        }

        public static double GetRateWithLookAhead(int RateID)
        {
            double Result = 0.0;

            try
            {
                if (RateID >= 0 && RateID < ZoneFields.Products.Length)
                {
                    double speedMps = Props.Speed_KMH / 3.6;

                    // Delegate to ZoneManager helper, which:
                    // - uses cLookAheadSeconds[RateID] to compute look-ahead distance
                    // - finds the look-ahead zone, if any
                    // - falls back to CurrentZone.Zone when look-ahead is 0 or no zone

                    Result = ZnOverlays.GetTargetRateWithLookAhead(RateID, cTractorPosition, cTravelHeading, speedMps);
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/GetRateWithLookAhead: " + ex.Message);
            }

            return Result;
        }

        public static void Initialize()
        {
            cProductFilter = int.TryParse(Props.GetProp("MapProductFilter"), out int pr) ? pr : 0;
            cShowTiles = bool.TryParse(Props.GetProp("MapShowTiles"), out bool st) ? st : true;

            cRateCollector = new DataCollector();
            JobManager.JobChanged += JobManager_JobChanged;
            cState = MapState.Tracking;

            legendManager = new LegendManager(gmap);
            InitializeMap();

            gmap.OnMapZoomChanged += Gmap_OnMapZoomChanged;
            gmap.MouseClick += Gmap_MouseClick;

            UpdateTimer = new System.Windows.Forms.Timer();
            UpdateTimer.Interval = 1000; // ms
            UpdateTimer.Tick += UpdateTimer_Tick;
            UpdateTimer.Enabled = true;

            LoadMap();

            bool kmlVisible = bool.TryParse(Props.GetProp("KmlVisible"), out var v) ? v : true;
            SetKmlVisibility(kmlVisible);

            ZnOverlays.ZonesChanged += ZnOverlays_ZonesChanged;
        }

        public static bool LoadMap()
        {
            bool Result = false;
            try
            {
                legendManager.ClearAppliedLegendObject();
                ZnOverlays.LoadZones();

                // kml
                kmlLayerManager.ClearKmlOverlaysFromMap(gmap);
                ReloadJobKmls();

                gmap.Refresh();
                CenterMap();
                MapChanged?.Invoke(null, EventArgs.Empty);
                Result = true;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/LoadMap: " + ex.Message);
            }
            return Result;
        }

        public static void Refresh()
        {
            gmap.Refresh();
            MapChanged?.Invoke(null, EventArgs.Empty);
        }

        public static void RemoveOverlay(GMapOverlay overlay)
        {
            // create a list of overlays matching the ID.(there could be multiple)
            // Remove the overlays in the list from the overlays collection.
            try
            {
                if (overlay != null)
                {
                    var overlaysToRemove = gmap.Overlays.Where(o => o.Id == overlay.Id).ToList();
                    foreach (var o in overlaysToRemove) gmap.Overlays.Remove(o);
                    // keep legend host on top after any removal
                    legendManager.EnsureLegendTop();
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/RemoveLayer: " + ex.Message);
            }
        }

        public static void SaveMap(string filePath = null)
        {
            try
            {
                if (filePath == null) filePath = JobManager.CurrentMapPath;

                List<MapZone> zones = new List<MapZone>();
                zones.AddRange(ZnOverlays.TargetZoneslist);
                zones.AddRange(ZnOverlays.AppliedZonesList);

                var shapefileHelper = new ShapefileHelper();
                bool Result = shapefileHelper.SaveMapZones(filePath, zones);

                if (Result) legendManager.SaveAppliedLegend(filePath);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/SaveMap: " + ex.Message);
            }
        }

        public static bool SaveMapImage(string FilePath)
        {
            bool Result = false;
            bool PrevShow = legendManager.Enabled;
            try
            {
                legendManager.Enabled = true;
                gmap.Refresh();
                gmap.Update();

                using (var bmp = new Bitmap(gmap.Width, gmap.Height))
                {
                    gmap.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
                    bmp.Save(FilePath, ImageFormat.Png);
                    Result = true;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/SaveMapImage: " + ex.Message);
            }
            legendManager.Enabled = PrevShow;
            return Result;
        }

        public static void SetKmlVisibility(bool visible)
        {
            try
            {
                foreach (var overlay in kmlLayerManager.GetAllOverlays())
                {
                    if (overlay == null) continue;
                    if (visible)
                        AddOverlay(overlay);
                    else
                        RemoveOverlay(overlay);
                }

                gmap.Refresh();
                MapChanged?.Invoke(null, EventArgs.Empty);
                Props.SetProp("KmlVisible", visible.ToString());
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/SetKmlVisibility: " + ex.Message);
            }
        }

        public static void SetTractorPosition(PointLatLng NewLocation, bool CenterMap = false)
        {
            if (cState == MapState.Tracking || cState == MapState.Preview)
            {
                if (!cTractorPosition.IsEmpty)
                {
                    double deltaLng = NewLocation.Lng - cTractorPosition.Lng;
                    double deltaLat = NewLocation.Lat - cTractorPosition.Lat;

                    double movementThreshold = 0.0000001;
                    if (Math.Abs(deltaLng) > movementThreshold || Math.Abs(deltaLat) > movementThreshold)
                    {
                        cTravelHeading = (Math.Atan2(deltaLng, deltaLat) * 180.0 / Math.PI + 360.0) % 360.0;
                    }
                }

                cTractorPosition = NewLocation;
                tractorMarker.Position = NewLocation;

                if (cState == MapState.Preview || CenterMap) gmap.Position = NewLocation;
            }
        }

        public static bool TryComputeScale(IEnumerable<double> values, out double minRate, out double maxRate)
        {
            var vals = values
                .Where(v => v > 0.01 && !double.IsNaN(v) && !double.IsInfinity(v))
                .OrderBy(v => v)
                .ToArray();

            minRate = 0; maxRate = 0;
            if (vals.Length == 0) return false;

            if (vals.Length < 10)
            {
                minRate = vals.First();
                maxRate = vals.Last();
            }
            else
            {
                int loIdx = (int)Math.Floor(0.02 * (vals.Length - 1));
                int hiIdx = (int)Math.Ceiling(0.98 * (vals.Length - 1));
                minRate = vals[loIdx];
                maxRate = vals[hiIdx];

                if (maxRate <= minRate)
                {
                    minRate = vals.First();
                    maxRate = vals.Last();
                }
            }

            if (Math.Abs(maxRate - minRate) < 0.01)
            {
                double pad = Math.Max(0.05 * maxRate, 1.0);
                minRate = Math.Max(0, maxRate - pad);
                maxRate = maxRate + pad;
            }
            return true;
        }

        private static RectLatLng GetOverallRectLatLng()
        {
            RectLatLng Result = RectLatLng.Empty;
            if (ZnOverlays.TargetOverlay != null && ZnOverlays.TargetOverlay.Polygons.Count > 0)
            {
                double minLat = double.MaxValue;
                double maxLat = double.MinValue;
                double minLng = double.MaxValue;
                double maxLng = double.MinValue;

                foreach (var polygon in ZnOverlays.TargetOverlay.Polygons)
                {
                    var pts = polygon.Points;
                    int count = pts.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var p = pts[i];
                        if (p.Lat < minLat) minLat = p.Lat;
                        if (p.Lat > maxLat) maxLat = p.Lat;
                        if (p.Lng < minLng) minLng = p.Lng;
                        if (p.Lng > maxLng) maxLng = p.Lng;
                    }
                }

                if (minLat != double.MaxValue)
                {
                    Result = new RectLatLng(maxLat, minLng, maxLng - minLng, maxLat - minLat);
                }
            }
            return Result;
        }

        private static void Gmap_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                switch (cState)
                {
                    case MapState.Preview:
                        MapLeftClicked?.Invoke(null, EventArgs.Empty);
                        break;

                    case MapState.EditZones:
                        var point = gmap.FromLocalToLatLng(e.X, e.Y);
                        ZnOverlays.AddVertex(point);
                        break;

                    case MapState.Tracking:
                        MapLeftClicked?.Invoke(null, EventArgs.Empty);
                        break;

                    case MapState.Positioning:
                        PointLatLng Location = gmap.FromLocalToLatLng(e.X, e.Y);
                        cTractorPosition = Location;
                        tractorMarker.Position = Location;
                        UpdateVariableRates();
                        gmap.Refresh();
                        MapChanged?.Invoke(null, EventArgs.Empty);
                        break;

                    default:
                        break;
                }
            }
        }

        private static void Gmap_OnMapZoomChanged()
        {
            legendManager?.OnMapZoomChanged();
            MapZoomed?.Invoke(null, EventArgs.Empty);
        }

        private static void InitializeMap()
        {
            GMapProviders.List.Add(ArcGIS_World_Imagery_Provider.Instance);
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            GMaps.Instance.PrimaryCache = new GMap.NET.CacheProviders.SQLitePureImageCache
            {
                CacheLocation = Props.ApplicationFolder + "\\MapCache"
            };

            GMapProvider.TTLCache = 24 * 60 * 7; // minutes, 7 days

            double Lat = 0;
            double Lng = TimeZoneInfo.Local.BaseUtcOffset.TotalHours * 15.0;    // estimated longitude
            if (double.TryParse(Props.GetProp("LastMapLat"), out double latpos)) Lat = latpos;
            if (double.TryParse(Props.GetProp("LastMapLng"), out double lngpos)) Lng = lngpos;

            if (cShowTiles)
            {
                gmap.MapProvider = (GMapProvider)ArcGIS_World_Imagery_Provider.Instance;
            }
            else
            {
                gmap.MapProvider = (GMapProvider)GMapProviders.EmptyProvider;
            }

            gmap.Position = new PointLatLng(Lat, Lng);
            gmap.ShowCenter = false;
            gmap.MinZoom = 1;
            gmap.MaxZoom = 19;
            gmap.Zoom = 15;
            gmap.Dock = DockStyle.Fill;

            // overlays
            gpsMarkerOverlay = new GMapOverlay("gpsMarkers");

            tractorMarker = new GMarkerGoogle(new PointLatLng(Lat, Lng), GMarkerGoogleType.green);
            tractorMarker.IsHitTestVisible = false;
            gpsMarkerOverlay.Markers.Add(tractorMarker);

            PointLatLng Location = new PointLatLng(Lat, Lng);
            cTractorPosition = Location;
            tractorMarker.Position = Location;
            UpdateVariableRates();

            AddOverlay(gpsMarkerOverlay);
            AddOverlay(ZnOverlays.NewZoneMarkerOverlay);
            AddOverlay(ZnOverlays.TargetOverlay);
            AddOverlay(ZnOverlays.AppliedOverlay);
            gmap.Refresh();
        }

        private static void JobManager_JobChanged(object sender, EventArgs e)
        {
            LoadMap();
        }

        private static string PersistKmlToJob(string sourcePath)
        {
            try
            {
                var jobDir = JobManager.CurrentMapPath;
                if (string.IsNullOrWhiteSpace(jobDir))
                    return null;

                // If CurrentMapPath is a file, use its directory
                if (!System.IO.Directory.Exists(jobDir))
                    jobDir = System.IO.Path.GetDirectoryName(jobDir);

                if (string.IsNullOrWhiteSpace(jobDir) || !System.IO.Directory.Exists(jobDir))
                    return null;

                var fileName = System.IO.Path.GetFileName(sourcePath);
                var destPath = System.IO.Path.Combine(jobDir, fileName);

                System.IO.File.Copy(sourcePath, destPath, true);

                var current = Props.GetProp("KmlJobFiles");
                var list = new List<string>(string.IsNullOrWhiteSpace(current) ? Array.Empty<string>() : current.Split('|'));
                if (!list.Any(p => string.Equals(p, fileName, StringComparison.OrdinalIgnoreCase)))
                {
                    list.Add(fileName);
                    Props.SetProp("KmlJobFiles", string.Join("|", list));
                }
                return destPath;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/PersistKmlToJob: " + ex.Message);
                return null;
            }
        }

        private static void ReloadJobKmls()
        {
            try
            {
                var jobDir = JobManager.CurrentMapPath;
                if (!System.IO.Directory.Exists(jobDir))
                    jobDir = System.IO.Path.GetDirectoryName(jobDir);

                var current = Props.GetProp("KmlJobFiles");
                var list = new List<string>(string.IsNullOrWhiteSpace(current) ? Array.Empty<string>() : current.Split('|'));

                foreach (var fname in list)
                {
                    if (string.IsNullOrWhiteSpace(fname) || string.IsNullOrWhiteSpace(jobDir)) continue;
                    var full = System.IO.Path.Combine(jobDir, fname);
                    if (!System.IO.File.Exists(full)) continue;

                    var overlay = kmlLayerManager.LoadKml(full);
                    if (overlay != null) AddOverlay(overlay);
                }
                gmap.Refresh();
                MapChanged?.Invoke(null, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/ReloadJobKmls: " + ex.Message);
            }
        }

        private static void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (Props.MainForm.GPS.Connected())
            {
                PointLatLng Position = new PointLatLng(Props.MainForm.GPS.Latitude, Props.MainForm.GPS.Longitude);
                SetTractorPosition(Position);
                if (Props.MainForm.Products.ProductsAreOn() && (cState == MapState.Tracking || cState == MapState.Preview))
                {
                    cRateCollector.RecordReading(Position.Lat, Position.Lng, Props.MainForm.Products.ProductAppliedRates());
                    ZnOverlays.UpdateAppliedOverlay(Props.MainForm.Products.ProductAppliedRates());
                }
            }
            UpdateVariableRates();
        }

        private static void UpdateVariableRates()
        {
            try
            {
                CurrentZone.IsDefined = false;
                if (ZnOverlays.Rtree != null)
                {
                    // Query spatial index for candidate zones near the tractor
                    var ptEnv = new Envelope(cTractorPosition.Lng, cTractorPosition.Lng, cTractorPosition.Lat, cTractorPosition.Lat);
                    var candidates = ZnOverlays.Rtree.Query(ptEnv);
                    if (candidates != null && candidates.Count > 0)
                    {
                        // emulate previous last-wins behavior: zones added later have priority
                        foreach (var zone in candidates.OrderByDescending(z => ZnOverlays.TargetZoneslist.IndexOf(z)))
                        {
                            if (zone.Contains(cTractorPosition))
                            {
                                // Use a deep copy so CurrentZone.Zone is detached from the original
                                CurrentZone.Zone = ZoneManager.CloneZoneForCurrent(zone);
                                CurrentZone.Hectares = zone.Hectares();
                                CurrentZone.IsDefined = true;
                                break;
                            }
                        }
                    }
                }
                if (!CurrentZone.IsDefined && Props.MainForm.Products != null)
                {
                    // create a fresh base-rate zone instead of mutating an existing one
                    var baseZone = new MapZone(
                        "Base Rate",
                        null,
                        new Dictionary<string, double>(),
                        Color.Blue,
                        ZoneType.Target);

                    int count = 0;
                    foreach (double rate in Props.MainForm.Products.BaseRates())
                    {
                        if (count >= ZoneFields.Products.Length)
                        {
                            break;
                        }
                        baseZone.Rates[ZoneFields.Products[count++]] = rate;
                    }

                    CurrentZone.Zone = baseZone;
                    CurrentZone.Hectares = 0;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/UpdateVariableRates: " + ex.Message);
            }
        }

        private static void ZnOverlays_ZonesChanged(object sender, EventArgs e)
        {
            UpdateVariableRates();
            gmap.Refresh();
            MapChanged?.Invoke(null, EventArgs.Empty);
        }

        private static bool ZoomToFit()
        {
            bool Result = false;
            RectLatLng boundingBox = GetOverallRectLatLng();
            if (boundingBox != RectLatLng.Empty)
            {
                gmap.SetZoomToFitRect(boundingBox);
                Result = true;
            }
            return Result;
        }
    }
}