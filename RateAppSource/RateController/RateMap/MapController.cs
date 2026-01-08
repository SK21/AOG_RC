using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.Strtree;
using RateController.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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

        private static GMapControl gmap;
        private static GMarkerGoogle tractorMarker;

        #endregion GMap

        #region Overlays

        private static GMapOverlay AppliedOverlay;
        private static GMapOverlay gpsMarkerOverlay;
        private static GMapOverlay NewZoneMarkerOverlay;
        private static GMapOverlay zoneOverlay;

        #endregion Overlays

        #region Saved Properties

        private static int cProductFilter;
        private static bool cShowRates;
        private static bool cShowTiles;
        private static bool cShowZones;

        #endregion Saved Properties

        #region Zones

        private static MapZone CurrentZone = null;
        private static Color CurrentZoneColor;
        private static double CurrentZoneHectares;
        private static string CurrentZoneName = "";
        private static double[] CurrentZoneRates;
        private static List<MapZone> HistoricalAppliedZones = new List<MapZone>();
        private static List<MapZone> mapZones;
        private static List<PointLatLng> NewZoneVertices;
        private static STRtree<MapZone> STRtreeZoneIndex;

        #endregion Zones

        private static readonly KmlLayerManager kmlLayerManager = new KmlLayerManager();
        public static readonly ZoneManager ZnOverlays = new ZoneManager();
        private static int _lastHistoryCount;
        private static DateTime _lastHistoryLastTimestamp;
        private static string _lastLoadedMapPath;
        private static int _lastProductRates = -1;
        private static bool cMapIsDisplayed = false;
        private static Dictionary<string, Color> ColorLegend;
        private static DataCollector cRateCollector;
        private static MapState cState;
        private static PointLatLng cTractorPosition;
        private static double cTravelHeading;
        private static LegendManager legendManager;
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

        public static bool LegendOverlayEnabled
        {
            get { return legendManager != null && legendManager.Enabled; }
            set
            {
                if (legendManager != null)
                {
                    legendManager.Enabled = value;
                    ShowLegend(ColorLegend, true);
                }
            }
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
                    ShowAppliedZonesOverlay();
                }
            }
        }

        public static DataCollector RateCollector
        { get { return cRateCollector; } }

        public static bool ShowRates
        {
            get { return cShowRates; }
            set
            {
                cShowRates = value;
                Props.SetProp("MapShowRates", cShowRates.ToString());
                ShowAppliedZonesOverlay();
            }
        }

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

        public static bool ShowZones
        {
            get { return cShowZones; }
            set
            {
                cShowZones = value;
                Props.SetProp("MapShowZones", cShowZones.ToString());
                ShowTargetZonesOverlay();
            }
        }

        public static MapState State
        { get { return cState; } }

        public static PointLatLng TractorPosition
        { get { return cTractorPosition; } }

        public static Color ZoneColor => CurrentZoneColor;

        public static double ZoneHectares => CurrentZoneHectares;

        public static string ZoneName
        {
            get { return CurrentZoneName; }
            set
            {
                if (value.Length > 0)
                {
                    if (value.Length > 12) value = value.Substring(0, 12);
                    CurrentZoneName = value;
                }
                else
                {
                    CurrentZoneName = "Unnamed Zone";
                }
            }
        }

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
                ColorLegend = null;
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

                zoneOverlay = null;
                gpsMarkerOverlay = null;
                NewZoneMarkerOverlay = null;

                gmap?.Dispose();
                gmap = null;

                STRtreeZoneIndex = null;
                mapZones?.Clear();
                mapZones = null;

                MapChanged = null;
                MapLeftClicked = null;
                MapZoomed = null;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/Close: " + ex.Message);
            }
        }

        public static bool CreateZone(string name, double Rt0, double Rt1, double Rt2, double Rt3, Color zoneColor, out int ErrorCode)
        {
            bool Result = false;
            ErrorCode = 0;
            try
            {
                if (ZoneNameFound(name))
                {
                    ErrorCode = 1;
                }
                else if (NewZoneVertices.Count < 3)
                {
                    ErrorCode = 2;
                }
                else
                {
                    var geometryFactory = new GeometryFactory();
                    var coordinates = NewZoneVertices.ConvertAll(p => new Coordinate(p.Lng, p.Lat)).ToArray();

                    if (!coordinates[0].Equals(coordinates[coordinates.Length - 1]))
                    {
                        Array.Resize(ref coordinates, coordinates.Length + 1);
                        coordinates[coordinates.Length - 1] = coordinates[0];
                    }
                    var polygon = geometryFactory.CreatePolygon(coordinates);

                    MapZone NewZone = new MapZone(name, polygon, new Dictionary<string, double>
                    {
                        { "ProductA", Rt0 },
                        { "ProductB", Rt1 },
                        { "ProductC", Rt2 },
                        { "ProductD", Rt3 }
                    }, zoneColor, ZoneType.Target);

                    mapZones.Add(NewZone);
                    zoneOverlay = AddPolygons(zoneOverlay, NewZone.ToGMapPolygons(Palette.TargetZoneTransparency));

                    NewZoneVertices.Clear();
                    NewZoneMarkerOverlay.Markers.Clear();

                    BuildTargetZonesIndex();
                    UpdateVariableRates();
                    gmap.Refresh();
                    MapChanged?.Invoke(null, EventArgs.Empty);
                    Result = true;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/CreateZone: " + ex.Message);
            }
            return Result;
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

        public static void DeleteLastVertex()
        {
            try
            {
                if (NewZoneVertices.Count > 0)
                {
                    NewZoneVertices.RemoveAt(NewZoneVertices.Count - 1);
                    if (NewZoneMarkerOverlay.Markers.Count > 0)
                    {
                        NewZoneMarkerOverlay.Markers.RemoveAt(NewZoneMarkerOverlay.Markers.Count - 1);
                    }
                    gmap.Refresh();
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/DeleteLastVertex: " + ex.Message);
            }
        }

        public static bool DeleteZone(string name)
        {
            bool Result = false;
            try
            {
                if (string.IsNullOrEmpty(name) || mapZones == null || zoneOverlay == null) return false;

                // collect all zones with the given name (multi-polygons often share the same name)
                var zonesToRemove = mapZones.Where(z => string.Equals(z.Name, name, StringComparison.Ordinal)).ToList();
                if (zonesToRemove.Count == 0) return false;

                foreach (var zone in zonesToRemove)
                {
                    // remove polygons from overlay that match the ones created for this zone
                    List<GMapPolygon> polygonsToRemove = zone.ToGMapPolygons(Palette.ZoneTransparency);
                    foreach (var polygonToRemove in polygonsToRemove)
                    {
                        if (polygonToRemove == null) continue;

                        var polygonInOverlay = zoneOverlay.Polygons
                            .FirstOrDefault(polygon => polygon.Points.SequenceEqual(polygonToRemove.Points));

                        if (polygonInOverlay != null)
                        {
                            zoneOverlay.Polygons.Remove(polygonInOverlay);
                            Result = true;
                        }
                    }
                }

                // also remove any remaining polygons in the overlay that carry this name (safety)
                var leftovers = zoneOverlay.Polygons.Where(p => string.Equals(p.Name, name, StringComparison.Ordinal) || (p.Name != null && p.Name.StartsWith(name + "_hole", StringComparison.Ordinal))).ToList();
                foreach (var p in leftovers) zoneOverlay.Polygons.Remove(p);

                // remove zones from the internal list
                mapZones.RemoveAll(z => string.Equals(z.Name, name, StringComparison.Ordinal));

                if (Result)
                {
                    RemoveOverlay(zoneOverlay);
                    AddOverlay(zoneOverlay);

                    BuildTargetZonesIndex();

                    gmap.Refresh();
                    SaveMap();
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/DeleteZone: " + ex.Message);
            }
            return Result;
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
            ShowLegend(ColorLegend);
        }

        public static bool EditZone(string name, double Rt0, double Rt1, double Rt2, double Rt3, Color zoneColor, out int ErrorCode)
        {
            // rates, color
            bool Result = false;
            ErrorCode = 0;
            try
            {
                MapZone ZoneToEdit = CurrentZone;
                if (ZoneNameFound(name, ZoneToEdit))
                {
                    // check for duplicate name
                    ErrorCode = 1;
                }
                else
                {
                    ZoneToEdit.Name = name;
                    Dictionary<string, double> NewRates = new Dictionary<string, double>
                    {
                        { "ProductA", Rt0 },
                        { "ProductB", Rt1 },
                        { "ProductC", Rt2 },
                        { "ProductD", Rt3 }
                    };
                    ZoneToEdit.Rates = NewRates;
                    ZoneToEdit.ZoneColor = zoneColor;

                    // Refresh polygons in overlay to reflect new color
                    if (zoneOverlay != null)
                    {
                        var polygonsForZone = ZoneToEdit.ToGMapPolygons(Palette.TargetZoneTransparency);
                        foreach (var polygonToReplace in polygonsForZone)
                        {
                            if (polygonToReplace == null) continue;
                            var existing = zoneOverlay.Polygons
                                .FirstOrDefault(p => p.Points.SequenceEqual(polygonToReplace.Points));
                            if (existing != null)
                            {
                                zoneOverlay.Polygons.Remove(existing);
                            }
                        }
                        zoneOverlay = AddPolygons(zoneOverlay, polygonsForZone);
                    }

                    UpdateVariableRates();
                    gmap.Refresh();
                    MapChanged?.Invoke(null, EventArgs.Empty);
                    Result = true;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/EditZone: " + ex.Message);
            }
            return Result;
        }

        public static double GetRate(int RateID)
        {
            double Result = 0.0;
            if (RateID >= 0 && RateID < (Props.MaxProducts - 2)) Result = CurrentZoneRates[RateID];
            return Result;
        }

        public static void Initialize()
        {
            cRateCollector = new DataCollector();
            LoadData();
            JobManager.JobChanged += JobManager_JobChanged;

            CurrentZoneRates = new double[Props.MaxProducts - 2];
            cState = MapState.Tracking;
            InitializeMap();
            legendManager = new LegendManager(gmap);

            // map zones
            mapZones = new List<MapZone>();
            NewZoneVertices = new List<PointLatLng>();

            gmap.OnMapZoomChanged += Gmap_OnMapZoomChanged;
            gmap.MouseClick += Gmap_MouseClick;

            UpdateTimer = new System.Windows.Forms.Timer();
            UpdateTimer.Interval = 1000; // ms
            UpdateTimer.Tick += UpdateTimer_Tick;
            UpdateTimer.Enabled = true;

            LoadMap();

            bool kmlVisible = bool.TryParse(Props.GetProp("KmlVisible"), out var v) ? v : true;
            SetKmlVisibility(kmlVisible);
        }

        public static bool LoadMap()
        {
            bool Result = false;
            try
            {
                var shapefileHelper = new ShapefileHelper();

                mapZones = shapefileHelper.CreateZoneList(JobManager.CurrentMapPath);

                // split zones by type
                var targetZones = mapZones.Where(z => z.ZoneType == ZoneType.Target).ToList();
                HistoricalAppliedZones = mapZones.Where(z => z.ZoneType == ZoneType.Applied).ToList();

                // target zones
                zoneOverlay.Polygons.Clear();
                foreach (var mapZone in targetZones)
                {
                    zoneOverlay = AddPolygons(zoneOverlay, mapZone.ToGMapPolygons(Palette.TargetZoneTransparency));
                }
                BuildTargetZonesIndex();
                ShowTargetZonesOverlay();

                ShowAppliedZonesOverlay();

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

        public static Dictionary<string, Color> LoadPersistedLegend(string basePath = null)
        {
            try
            {
                if (basePath == null)
                {
                    basePath = Path.ChangeExtension(JobManager.CurrentMapPath, null);
                }

                string legendPath = basePath + "_AppliedLegend.json";
                if (!File.Exists(legendPath))
                {
                    return null;
                }

                var json = File.ReadAllText(legendPath);
                var bands = System.Text.Json.JsonSerializer.Deserialize<List<LegendBand>>(json);
                if (bands == null || bands.Count == 0)
                {
                    return null;
                }

                // Filter to current product
                var filtered = bands
                    .Where(b => b.ProductIndex == cProductFilter)
                    .OrderBy(b => b.Min)
                    .ToList();

                int Steps = filtered.Count;
                if (Steps == 0)
                {
                    return null;
                }

                double globalMin = filtered.First().Min;
                double globalMax = filtered.Max(b => b.Max);

                return LegendManager.CreateAppliedLegend(globalMin, globalMax, Steps);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/LoadPersistedLegend: " + ex.Message);
                return null;
            }
        }

        public static void ResetMarkers()
        {
            NewZoneVertices.Clear();
            NewZoneMarkerOverlay.Markers.Clear();
        }

        public static void SaveAppliedLegend(string legendPath, Dictionary<string, Color> LegendToSave = null)
        {
            try
            {
                if (LegendToSave == null) LegendToSave = ColorLegend;

                if (LegendToSave == null || LegendToSave.Count == 0)
                {
                    return;
                }

                var bands = new List<LegendBand>();
                foreach (var kvp in LegendToSave)
                {
                    var parts = kvp.Key.Split('-');
                    if (parts.Length != 2) continue;
                    if (!double.TryParse(parts[0], out double min)) continue;
                    if (!double.TryParse(parts[1], out double max)) continue;

                    bands.Add(new LegendBand
                    {
                        Min = min,
                        Max = max,
                        ColorHtml = ColorTranslator.ToHtml(kvp.Value),
                        ProductIndex = cProductFilter
                    });
                }

                if (bands.Count == 0)
                {
                    return;
                }

                var basePath = Path.ChangeExtension(legendPath, null); // strip .shp
                var appliedLegendPath = basePath + "_AppliedLegend.json";

                var json = System.Text.Json.JsonSerializer.Serialize(bands);
                File.WriteAllText(appliedLegendPath, json);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/SaveAppliedLegend: " + ex.Message);
            }
        }

        public static bool SaveMap()
        {
            bool Result = false;
            try
            {
                var shapefileHelper = new ShapefileHelper();
                Result = shapefileHelper.SaveMapZones(JobManager.CurrentMapPath, mapZones);

                if (Result) SaveAppliedLegend(JobManager.CurrentMapPath);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/SaveMap: " + ex.Message);
            }

            return Result;
        }

        public static bool SaveMapImage(string FilePath)
        {
            bool Result = false;
            bool PrevShow = LegendOverlayEnabled;
            try
            {
                LegendOverlayEnabled = true;
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
            LegendOverlayEnabled = PrevShow;
            return Result;
        }

        public static void SaveMapToFile(string filePath)
        {
            try
            {
                var shapefileHelper = new ShapefileHelper();

                // Save zone features to the specified filePath
                bool Result = shapefileHelper.SaveMapZones(filePath, mapZones);
                if (Result) SaveAppliedLegend(filePath);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/SaveMapToFile: " + ex.Message);
            }
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

        public static int ZoneCount(ZoneType type = ZoneType.Target)
        {
            int Result = 0;
            if (type == ZoneType.Target)
            {
                var targetZones = mapZones.Where(z => z.ZoneType == ZoneType.Target).ToList();
                Result = targetZones.Count;
            }
            else
            {
                var AppliedZones = mapZones.Where(z => z.ZoneType == ZoneType.Applied).ToList();
                Result = AppliedZones.Count;
            }
            return Result;
        }

        private static void AddOverlay(GMapOverlay NewOverlay)
        {
            if (NewOverlay != null)
            {
                bool overlayExists = gmap.Overlays.Any(o => o.Id == NewOverlay.Id);
                if (!overlayExists) gmap.Overlays.Add(NewOverlay);
                EnsureLegendTop();
            }
        }

        private static GMapOverlay AddPolygons(GMapOverlay overlay, List<GMapPolygon> polygons)
        {
            foreach (var polygon in polygons)
            {
                // remove stroke(border) to match AOG polygon look overlap-free
                polygon.Stroke = Pens.Transparent;
                overlay.Polygons.Add(polygon);
            }
            return overlay;
        }

        private static void BuildAppliedOverlay()
        {
            try
            {
                ZnOverlays.AppliedOverlay.Polygons.Clear();
                AddOverlay(ZnOverlays.AppliedOverlay);

                Dictionary<string, Color> histLegend;
                bool AppliedFound = false;
                if (ZnOverlays.BuildAppliedFromHistory(ZnOverlays.AppliedOverlay, out histLegend))
                {
                    // Use legend returned from history build
                    ColorLegend = histLegend;
                    AppliedFound = true;
                }
                else
                {
                    // use historical applied zones from shapefile
                    if (HistoricalAppliedZones.Count > 0)
                    {
                        foreach (var mapZone in HistoricalAppliedZones)
                        {
                            AppliedOverlay = AddPolygons(ZnOverlays.AppliedOverlay, mapZone.ToGMapPolygons(Palette.ZoneTransparency));
                        }
                        // Build legend that matches persisted applied zones
                        // Try to load a persisted legend first
                        ColorLegend = LoadPersistedLegend() ?? LegendManager.BuildAppliedZonesLegend(HistoricalAppliedZones, cProductFilter);
                        AppliedFound = true;
                    }
                }
                if (AppliedFound)
                {
                    ShowLegend(ColorLegend, true);
                }
                else
                {
                    legendManager.Clear();
                    ColorLegend = null;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/BuildAppliedZones: " + ex.Message);
            }
        }

        private static void BuildTargetZonesIndex()
        {
            // build a STRtree object for efficiently working with spatial objects (zones)
            try
            {
                STRtreeZoneIndex = new STRtree<MapZone>();
                if (mapZones != null)
                {
                    foreach (var z in mapZones.Where(z => z.ZoneType == ZoneType.Target))
                    {
                        if (z?.Geometry == null) continue;
                        var env = z.Geometry.EnvelopeInternal;
                        if (env == null) continue;
                        STRtreeZoneIndex.Insert(env, z);
                    }
                    STRtreeZoneIndex.Build();
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/BuildZoneIndex: " + ex.Message);
                STRtreeZoneIndex = null; // fallback gracefully
            }
        }

        private static void EnsureLegendTop()
        {
            try
            {
                legendManager?.EnsureLegendTop();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/EnsureLegendTop: " + ex.Message);
            }
        }

        private static RectLatLng GetOverallRectLatLng()
        {
            RectLatLng Result = RectLatLng.Empty;
            if (zoneOverlay != null && zoneOverlay.Polygons.Count > 0)
            {
                double minLat = double.MaxValue;
                double maxLat = double.MinValue;
                double minLng = double.MaxValue;
                double maxLng = double.MinValue;

                foreach (var polygon in zoneOverlay.Polygons)
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
                        NewZoneVertices.Add(point);
                        NewZoneMarkerOverlay.Markers.Add(new GMarkerGoogle(point, GMarkerGoogleType.red_small));
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

            GMapProvider.TTLCache = 24 * 60 * 7; // minutes (e.g., 24 hours) 7 days

            double Lat = 0;
            double Lng = TimeZoneInfo.Local.BaseUtcOffset.TotalHours * 15.0;    // estimated longitude
            if (double.TryParse(Props.GetProp("LastMapLat"), out double latpos)) Lat = latpos;
            if (double.TryParse(Props.GetProp("LastMapLng"), out double lngpos)) Lng = lngpos;

            gmap = new GMapControl();

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
            zoneOverlay = new GMapOverlay("mapzones");
            gpsMarkerOverlay = new GMapOverlay("gpsMarkers");
            NewZoneMarkerOverlay = new GMapOverlay("tempMarkers");

            tractorMarker = new GMarkerGoogle(new PointLatLng(Lat, Lng), GMarkerGoogleType.green);
            tractorMarker.IsHitTestVisible = false;
            gpsMarkerOverlay.Markers.Add(tractorMarker);

            PointLatLng Location = new PointLatLng(Lat, Lng);
            cTractorPosition = Location;
            tractorMarker.Position = Location;
            UpdateVariableRates();

            AddOverlay(gpsMarkerOverlay);
            AddOverlay(NewZoneMarkerOverlay);
            AddOverlay(zoneOverlay);
            AddOverlay(ZnOverlays.AppliedOverlay);
            gmap.Refresh();
        }

        private static void JobManager_JobChanged(object sender, EventArgs e)
        {
            LoadMap();
        }

        private static bool LegendsDiffer(Dictionary<string, Color> A, Dictionary<string, Color> B)
        {
            if (A == B) return false;
            if (A == null || B == null) return true;
            if (A.Count != B.Count) return true;

            foreach (var kvp in A)
            {
                if (!B.TryGetValue(kvp.Key, out Color bColor)) return true;
                if (bColor != kvp.Value) return true;
            }
            return false;
        }

        private static void LoadData()
        {
            cProductFilter = int.TryParse(Props.GetProp("MapProductFilter"), out int pr) ? pr : 0;
            cShowRates = bool.TryParse(Props.GetProp("MapShowRates"), out bool sr) ? sr : false;
            cShowZones = bool.TryParse(Props.GetProp("MapShowZones"), out bool sz) ? sz : true;
            cShowTiles = bool.TryParse(Props.GetProp("MapShowTiles"), out bool st) ? st : true;
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

        private static void Props_RateDataSettingsChanged(object sender, EventArgs e)
        {
            try
            {
                // Reload persisted settings and rebuild overlay accordingly
                LoadData();
                ShowAppliedZonesOverlay();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/Props_RateDataSettingsChanged: " + ex.Message);
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

        private static void RemoveOverlay(GMapOverlay overlay)
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
                    EnsureLegendTop();
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/RemoveLayer: " + ex.Message);
            }
        }

        private static bool ShouldSkipHistoryBuild(string mapPath, IReadOnlyList<RateReading> readings)
        {
            if (string.IsNullOrEmpty(mapPath) || readings == null || readings.Count == 0) return false;

            var lastTs = readings[readings.Count - 1].Timestamp;
            return string.Equals(_lastLoadedMapPath, mapPath, StringComparison.Ordinal) &&
                   _lastHistoryCount == readings.Count &&
                   _lastHistoryLastTimestamp == lastTs &&
                   _lastProductRates == cProductFilter &&                 // ensure product selection matches
                   AppliedOverlay != null &&
                   AppliedOverlay.Polygons != null &&
                   AppliedOverlay.Polygons.Count > 0;
        }

        private static void ShowAppliedZonesOverlay()
        {
            try
            {
                if (cShowRates)
                {
                    // Decide whether to rebuild coverage from history
                    var readings = cRateCollector?.GetReadings();
                    bool SkipRebuild = ShouldSkipHistoryBuild(JobManager.CurrentMapPath, readings);

                    if (SkipRebuild)
                    {
                        AddOverlay(AppliedOverlay);
                        ShowLegend(ColorLegend);
                    }
                    else
                    {
                        BuildAppliedOverlay();

                        // update signature after a successful build
                        _lastLoadedMapPath = JobManager.CurrentMapPath;
                        if (readings != null && readings.Count > 0)
                        {
                            _lastHistoryCount = readings.Count;
                            _lastHistoryLastTimestamp = readings[readings.Count - 1].Timestamp;
                        }
                        else
                        {
                            _lastHistoryCount = 0;
                            _lastHistoryLastTimestamp = DateTime.MinValue;
                        }
                        _lastProductRates = cProductFilter;
                    }
                    gmap.Refresh();
                    MapChanged?.Invoke(null, EventArgs.Empty);
                }
                else
                {
                    // remove rates overlay
                    ZnOverlays.ResetTrail();
                    RemoveOverlay(AppliedOverlay);
                    legendManager?.Clear();
                    gmap.Refresh();
                    MapChanged?.Invoke(null, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/ShowRatesOverlay: " + ex.Message);
            }
        }

        private static void ShowLegend(Dictionary<string, Color> LegendToShow, bool Show = true)
        {
            if (Show && cState != MapState.Preview)
            {
                legendManager?.UpdateLegend(LegendToShow);
            }
            else
            {
                legendManager?.Clear();
            }
        }

        private static void ShowTargetZonesOverlay()
        {
            if (cShowZones)
            {
                try
                {
                    if (zoneOverlay == null) zoneOverlay = new GMapOverlay("mapzones");

                    // Rebuild polygons to ensure correct rendering after map resize
                    zoneOverlay.Polygons.Clear();
                    if (mapZones != null)
                    {
                        var targetZones = mapZones.Where(z => z.ZoneType == ZoneType.Target).ToList();
                        foreach (var mapZone in targetZones)
                        {
                            AddPolygons(zoneOverlay, mapZone.ToGMapPolygons(Palette.TargetZoneTransparency));
                        }
                    }

                    AddOverlay(zoneOverlay);
                    gmap.Refresh();
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("MapController/ShowZoneOverlay: " + ex.Message);
                }
            }
            else
            {
                RemoveOverlay(zoneOverlay);
                gmap.Refresh();
            }
        }

        private static bool UpdateRateLayer(double[] AppliedRates)
        {
            bool Result = false;
            Dictionary<string, Color> newLegend = new Dictionary<string, Color>();
            try
            {
                if (cShowRates && cMapIsDisplayed && (cState == MapState.Tracking || cState == MapState.Preview))
                {
                    var readings = cRateCollector.GetReadings();
                    if (readings == null || readings.Count == 0)
                    {
                        ClearAppliedRatesOverlay();
                    }
                    else
                    {
                        double Rates = AppliedRates[cProductFilter];  // product rates to display

                        Result = ZnOverlays.UpdateTrail(
                            AppliedOverlay,
                            readings,
                            cTractorPosition,
                            cTravelHeading,
                            Props.MainForm.Sections.TotalWidth(),
                            Rates,
                            out newLegend,
                            cProductFilter
                        );
                    }
                }
                if (LegendsDiffer(ColorLegend, newLegend))
                {
                    ColorLegend = newLegend;
                    if (LegendOverlayEnabled) ShowLegend(ColorLegend);
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/UpdateRateLayer: " + ex.Message);
            }
            return Result;
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
                    if (cShowRates && cMapIsDisplayed) UpdateRateLayer(Props.MainForm.Products.ProductAppliedRates());
                }
            }
            UpdateVariableRates();
        }

        private static void UpdateVariableRates()
        {
            try
            {
                bool ZoneFound = false;
                if (STRtreeZoneIndex != null)
                {
                    // Query spatial index for candidate zones near the tractor
                    var ptEnv = new Envelope(cTractorPosition.Lng, cTractorPosition.Lng, cTractorPosition.Lat, cTractorPosition.Lat);
                    var candidates = STRtreeZoneIndex.Query(ptEnv);
                    if (candidates != null && candidates.Count > 0)
                    {
                        // emulate previous last-wins behavior: zones added later have priority
                        foreach (var zone in candidates.OrderByDescending(z => mapZones.IndexOf(z)))
                        {
                            if (zone.Contains(cTractorPosition))
                            {
                                CurrentZoneName = zone.Name;
                                CurrentZoneRates[0] = zone.Rates["ProductA"];
                                CurrentZoneRates[1] = zone.Rates["ProductB"];
                                CurrentZoneRates[2] = zone.Rates["ProductC"];
                                CurrentZoneRates[3] = zone.Rates["ProductD"];
                                CurrentZoneColor = zone.ZoneColor;
                                CurrentZoneHectares = zone.Hectares();
                                ZoneFound = true;
                                CurrentZone = zone;
                                break;
                            }
                        }
                    }
                }
                if (!ZoneFound && Props.MainForm.Products != null)
                {
                    // use target rates
                    CurrentZoneName = "Base Rate";
                    CurrentZoneRates = Props.MainForm.Products.BaseRates();
                    CurrentZoneColor = Color.Blue;
                    CurrentZoneHectares = 0;
                    CurrentZone = null;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/UpdateVariableRates: " + ex.Message);
            }
        }

        private static bool ZoneNameFound(string Name, MapZone ExcludeZone = null)
        {
            bool Result = false;
            foreach (MapZone zn in mapZones)
            {
                if (string.Equals(zn.Name, Name, StringComparison.OrdinalIgnoreCase) && zn != ExcludeZone)
                {
                    Result = true;
                    break;
                }
            }
            return Result;
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