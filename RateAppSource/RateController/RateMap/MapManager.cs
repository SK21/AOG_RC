using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.Strtree;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq; // Linq used throughout
using System.Windows.Forms;
using System.Globalization;
using System.Text.RegularExpressions;

namespace RateController.Classes
{
    public class ArcGIS_World_Imagery_Provider : GMapProvider
    {
        public static readonly ArcGIS_World_Imagery_Provider Instance = new ArcGIS_World_Imagery_Provider();

        private static readonly TimeSpan MinRequestInterval = TimeSpan.FromMilliseconds(120);

        // Simple rate limiter: one request every 120 ms (adjust as needed)
        private static readonly object ThrottleLock = new object();

        private static DateTime _lastRequestUtc = DateTime.MinValue;
        public override Guid Id => new Guid("F4E1A7A7-3B5D-4FDC-9C84-9B2390C7B04C");
        public override string Name => "ArcGISWorldImagery";
        public override GMapProvider[] Overlays => new GMapProvider[] { this };
        public override PureProjection Projection => GMap.NET.Projections.MercatorProjection.Instance;

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            // Throttle access to avoid server blocking
            lock (ThrottleLock)
            {
                var now = DateTime.UtcNow;
                var elapsed = now - _lastRequestUtc;
                if (elapsed < MinRequestInterval)
                {
                    System.Threading.Thread.Sleep(MinRequestInterval - elapsed);
                }
                _lastRequestUtc = DateTime.UtcNow;
            }

            string url = $"https://services.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{zoom}/{pos.Y}/{pos.X}";
            return GetTileImageUsingHttp(url);
        }
    }

    public class MapManager : IDisposable
    {
        private const int MapRefreshSeconds = 2;

        // Increased throttle to reduce UI redraws
        private static readonly TimeSpan MinAppliedOverlayUpdate = TimeSpan.FromMilliseconds(400);

        private readonly RateOverlayService overlayService = new RateOverlayService();

        private GMapOverlay AppliedOverlay;
        private System.Windows.Forms.Timer AppliedOverlayTimer;
        private bool cEditPolygons = false;
        private double[] cLastAppliedRates = new double[5];
        private Dictionary<string, Color> cLegend;
        private bool cMouseSetTractorPosition = false;
        private bool cSuppressTrailUntilNextGps = false;
        private PointLatLng cTractorPosition;
        private double cTravelHeading;
        private List<PointLatLng> currentZoneVertices;
        private Color cZoneColor;
        private double cZoneHectares;
        private string cZoneName = "Unnamed Zone";
        private double[] cZoneRates = new double[4];
        private bool disposed;
        private GMapControl gmap;
        private GMapOverlay gpsMarkerOverlay;
        private bool isDragging = false;

        // Throttle immediate applied overlay updates to avoid UI flooding
        private DateTime lastAppliedOverlayUpdateUtc = DateTime.MinValue;

        private System.Drawing.Point lastMousePosition;

        // Legend manager to handle legend rendering and layout
        private LegendManager legendManager;

        private List<MapZone> mapZones;
        private FormStart mf;
        private GMapOverlay tempMarkerOverlay;
        private GMarkerGoogle tractorMarker;
        private STRtree<MapZone> zoneIndex;        // spatial index for fast zone lookup
        private GMapOverlay zoneOverlay;

        public MapManager(FormStart main)
        {
            mf = main;

            InitializeMap();
            InitializeMapZones();
            gmap.MouseDown += Gmap_MouseDown;
            gmap.MouseMove += Gmap_MouseMove;
            gmap.MouseUp += Gmap_MouseUp;
            gmap.OnMapZoomChanged += Gmap_OnMapZoomChanged;

            AppliedOverlayTimer = new System.Windows.Forms.Timer();
            AppliedOverlayTimer.Interval = 60000;
            AppliedOverlayTimer.Tick += AppliedOverlayTimer_Tick;
            AppliedOverlayTimer.Enabled = false;

            JobManager.JobChanged += Props_JobChanged;
            Props.RateDataSettingsChanged += Props_MapShowRatesChanged;

            TilesGrayScale = false;

            LoadMap();
        }

        public event EventHandler MapChanged;

        public bool EditModePolygons
        {
            get { return cEditPolygons; }
            set
            {
                cEditPolygons = value;
                tempMarkerOverlay.Clear();
                currentZoneVertices.Clear();
            }
        }

        public GMapControl gmapObject => gmap;

        // allow form to control legend visibility (only show in full-screen)
        public bool LegendOverlayEnabled
        {
            get { return legendManager != null && legendManager.LegendOverlayEnabled; }
            set { if (legendManager != null) { legendManager.LegendOverlayEnabled = value; legendManager.UpdateLegend(cLegend); } }
        }

        public int LegendRightMarginPx
        {
            get { return legendManager != null ? legendManager.LegendRightMarginPx : 0; }
            set { if (legendManager != null) { legendManager.LegendRightMarginPx = value; legendManager.UpdateLegend(cLegend); } }
        }

        public bool MouseSetTractorPosition
        {
            get { return cMouseSetTractorPosition; }
            set { cMouseSetTractorPosition = value; }
        }

        public bool ShowTiles
        {
            get { return Props.MapShowTiles; }
            set
            {
                Props.MapShowTiles = value;
                gmap.MapProvider = value
                    ? (GMapProvider)ArcGIS_World_Imagery_Provider.Instance
                    : (GMapProvider)GMapProviders.EmptyProvider;
                Refresh();
            }
        }

        // Make satellite tiles grayscale to reduce intensity
        public bool TilesGrayScale
        {
            get => gmap?.GrayScaleMode ?? false;
            set
            {
                if (gmap != null)
                {
                    gmap.GrayScaleMode = value;
                    Refresh();
                }
            }
        }

        public Color ZoneColor => cZoneColor;

        public double ZoneHectares => cZoneHectares;

        public string ZoneName
        {
            get { return cZoneName; }
            set
            {
                if (value.Length > 0)
                {
                    if (value.Length > 12) value = value.Substring(0, 12);
                    cZoneName = value;
                }
                else
                {
                    cZoneName = "Unnamed Zone";
                }
            }
        }

        public void CenterMap()
        {
            if (!CenterOnAppliedDataIfAvailable())
            {
                // Fallback to zones coverage
                ZoomToFit();
            }
        }

        public void ClearAppliedRatesOverlay()
        {
            try
            {
                overlayService.Reset();
                if (AppliedOverlay != null) AppliedOverlay.Polygons.Clear();
                legendManager?.Clear();
                cLegend = null;
                Refresh();
                MapChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapManager/ClearAppliedRatesOverlay: " + ex.Message);
            }
        }

        public bool DeleteZone(string name)
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
                    List<GMapPolygon> polygonsToRemove = zone.ToGMapPolygons();
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

                    BuildZoneIndex();

                    Refresh();
                    UpdateTargetRates();
                    SaveMap();
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapManager/DeleteZone: " + ex.Message);
            }
            return Result;
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            try
            {
                AppliedOverlayTimer.Enabled = false;
                AppliedOverlayTimer.Tick -= AppliedOverlayTimer_Tick;
                JobManager.JobChanged -= Props_JobChanged;
                Props.RateDataSettingsChanged -= Props_MapShowRatesChanged;
                if (gmap != null)
                {
                    gmap.MouseDown -= Gmap_MouseDown;
                    gmap.MouseMove -= Gmap_MouseMove;
                    gmap.MouseUp -= Gmap_MouseUp;
                    gmap.OnMapZoomChanged -= Gmap_OnMapZoomChanged;
                }

                legendManager?.Dispose();

                AppliedOverlay?.Polygons.Clear();
                zoneOverlay?.Polygons.Clear();
                tempMarkerOverlay?.Markers.Clear();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapManager/Dispose: " + ex.Message);
            }
        }

        public RectLatLng GetOverallRectLatLng()
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

        public double GetRate(int RateID)
        {
            double Result = 0.0;
            if (RateID >= 0 && RateID < 4) Result = cZoneRates[RateID];
            return Result;
        }

        public bool LoadMap()
        {
            bool Result = false;
            try
            {
                var shapefileHelper = new ShapefileHelper();

                mapZones = shapefileHelper.CreateZoneList(JobManager.CurrentMapPath);

                zoneOverlay.Polygons.Clear();
                foreach (var mapZone in mapZones)
                {
                    zoneOverlay = AddPolygons(zoneOverlay, mapZone.ToGMapPolygons());
                }

                BuildZoneIndex();

                Refresh();
                Result = true;
                MapChanged?.Invoke(this, EventArgs.Empty);

                // Prefer centering on existing applied data (coverage) when opening
                ShowAppliedRatesOverlay();
                CenterMap();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapManager/LoadMap: " + ex.Message);
            }
            return Result;
        }

        public bool SaveMap(bool UpdateCache = true)
        {
            bool Result = false;
            var shapefileHelper = new ShapefileHelper();
            shapefileHelper.SaveMapZones(JobManager.CurrentMapPath, mapZones);
            if (UpdateCache) AddToCache();
            Result = true;
            return Result;
        }

        public void SaveMapToFile(string filePath)
        {
            var shapefileHelper = new ShapefileHelper();

            // If zones exist, save them as-is
            if (mapZones != null && mapZones.Count > 0)
            {
                shapefileHelper.SaveMapZones(filePath, mapZones);
                return;
            }

            // No zones available: try to export applied map data (coverage)
            try
            {
                if (AppliedOverlay == null) AppliedOverlay = new GMapOverlay("AppliedRates");

                // Ensure the overlay is populated at least from history
                var readings = Props.RateCollector?.GetReadings();
                if (readings != null && readings.Count > 0)
                {
                    AppliedOverlay.Polygons.Clear();

                    double implementWidth = 24.0;
                    if (mf.Sections != null)
                    {
                        try { implementWidth = mf.Sections.TotalWidth(false); }
                        catch (Exception ex) { Props.WriteErrorLog($"MapManager: implement width - {ex.Message}"); }
                    }

                    // Rebuild from history for a full consistent overlay
                    Dictionary<string, Color> legendFromHistory;
                    bool ok = overlayService.BuildFromHistory(
                        AppliedOverlay,
                        readings,
                        implementWidth,
                        Props.RateDisplayType,
                        Props.RateDisplayProduct,
                        out legendFromHistory
                    );

                    if (ok && AppliedOverlay.Polygons.Count > 0)
                    {
                        shapefileHelper.SaveOverlayPolygons(filePath, AppliedOverlay);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapManager/SaveMapToFile coverage fallback: " + ex.Message);
            }

            // Nothing to export, ensure any existing file is removed for cleanliness
            try
            {
                if (Props.IsPathSafe(filePath))
                {
                    string shp = System.IO.Path.ChangeExtension(filePath, ".shp");
                    string dbf = System.IO.Path.ChangeExtension(filePath, ".dbf");
                    string shx = System.IO.Path.ChangeExtension(filePath, ".shx");
                    string prj = System.IO.Path.ChangeExtension(filePath, ".prj");
                    string qix = System.IO.Path.ChangeExtension(filePath, ".qix");

                    if (System.IO.File.Exists(shp)) System.IO.File.Delete(shp);
                    if (System.IO.File.Exists(dbf)) System.IO.File.Delete(dbf);
                    if (System.IO.File.Exists(shx)) System.IO.File.Delete(shx);
                    if (System.IO.File.Exists(prj)) System.IO.File.Delete(prj);
                    if (System.IO.File.Exists(qix)) System.IO.File.Delete(qix);
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapManager/SaveMapToFile cleanup: " + ex.Message);
            }
        }

        public void SetTractorPosition(PointLatLng NewLocation, double[] AppliedRates, double[] TargetRates)
        {
            if (!cMouseSetTractorPosition)
            {
                // Resume plotting on next overlay update since this is a real GPS update
                cSuppressTrailUntilNextGps = false;

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

                // Always record reading so zeros are captured immediately
                var applied = (AppliedRates ?? Array.Empty<double>());
                var target = (TargetRates ?? Array.Empty<double>());
                Props.RateCollector?.RecordReading(NewLocation.Lat, NewLocation.Lng, applied, target);

                // NEW: cache latest applied array for live overlay update (no timer delay)
                // ensure fixed length (5 max supported by recorder)
                int len = Math.Min(applied.Length, 5);
                if (len == 0) Array.Clear(cLastAppliedRates, 0, cLastAppliedRates.Length);
                else
                {
                    if (cLastAppliedRates.Length != 5) cLastAppliedRates = new double[5];
                    Array.Clear(cLastAppliedRates, 0, cLastAppliedRates.Length);
                    Array.Copy(applied, cLastAppliedRates, len);
                }

                // Force immediate overlay update with a small throttle to avoid flooding UI
                // Only update the applied overlay live if the map is visible
                if (Props.MapShowRates && Props.RateMapIsVisible())
                {
                    var now = DateTime.UtcNow;
                    if (now - lastAppliedOverlayUpdateUtc >= MinAppliedOverlayUpdate)
                    {
                        cLegend = ShowAppliedLayer();
                        lastAppliedOverlayUpdateUtc = now;
                    }
                }

                Refresh();
                UpdateTargetRates();
                MapChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public Dictionary<string, Color> ShowAppliedLayer()
        {
            // Do nothing when not visible (avoid CPU). Return last legend if any.
            if (!Props.RateMapIsVisible())
                return cLegend ?? new Dictionary<string, Color>();

            Dictionary<string, Color> legend = new Dictionary<string, Color>();
            try
            {
                var readings = Props.RateCollector.GetReadings();
                if (readings == null || readings.Count == 0)
                {
                    ClearAppliedRatesOverlay();
                    return legend;
                }

                double implementWidth = 24.0;
                if (mf.Sections != null)
                {
                    try { implementWidth = mf.Sections.TotalWidth(false); }
                    catch (Exception ex) { Props.WriteErrorLog($"MapManager: implement width - {ex.Message}"); }
                }

                bool success = false;

                if (cMouseSetTractorPosition || cSuppressTrailUntilNextGps)
                {
                    Dictionary<string, Color> legendFromHistory;
                    success = overlayService.BuildFromHistory(
                        AppliedOverlay,
                        readings,
                        implementWidth,
                        Props.RateDisplayType,
                        Props.RateDisplayProduct,
                        out legendFromHistory
                    );

                    if (success) legend = legendFromHistory;
                }
                else
                {
                    // NEW: live update uses the latest applied rate we just received with the GPS tick
                    double? liveApplied = null;
                    int ridx = Props.RateDisplayProduct;
                    if (cLastAppliedRates != null && ridx >= 0 && ridx < cLastAppliedRates.Length)
                        liveApplied = cLastAppliedRates[ridx];

                    success = overlayService.UpdateRatesOverlayLive(
                        AppliedOverlay,
                        readings,
                        cTractorPosition,
                        cTravelHeading,
                        implementWidth,
                        liveApplied,
                        out legend,
                        Props.RateDisplayType,
                        Props.RateDisplayProduct
                    );

                    if (!success || AppliedOverlay.Polygons.Count == 0 || (cTractorPosition.Lat == 0 && cTractorPosition.Lng == 0))
                    {
                        Dictionary<string, Color> legendFromHistory;
                        bool replay = overlayService.BuildFromHistory(
                            AppliedOverlay,
                            readings,
                            implementWidth,
                            Props.RateDisplayType,
                            Props.RateDisplayProduct,
                            out legendFromHistory
                        );

                        if (replay)
                        {
                            legend = legendFromHistory;
                            success = true;
                        }
                    }
                }

                if (!success)
                {
                    ClearAppliedRatesOverlay();
                    return legend;
                }

                cLegend = new Dictionary<string, Color>(legend);

                if (!gmap.Overlays.Contains(AppliedOverlay))
                {
                    gmap.Overlays.Add(AppliedOverlay);
                }

                legendManager?.UpdateLegend(cLegend);

                Refresh();
                MapChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog($"MapManager/ShowAppliedLayer: {ex.Message}");
            }
            return legend;
        }

        public void ShowAppliedRatesOverlay()
        {
            if (Props.MapShowRates && Props.RateMapIsVisible())
            {
                try
                {
                    if (AppliedOverlay == null) AppliedOverlay = new GMapOverlay("AppliedRates");
                    overlayService.Reset();
                    AppliedOverlay.Polygons.Clear();

                    AddOverlay(AppliedOverlay);
                    cLegend = ShowAppliedLayer();

                    int RefreshIntervalSeconds = MapRefreshSeconds;
                    if (RefreshIntervalSeconds > 0 && RefreshIntervalSeconds < 61)
                    {
                        AppliedOverlayTimer.Interval = RefreshIntervalSeconds * 1000;
                        AppliedOverlayTimer.Enabled = true;
                    }
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("MapManager/ShowAppliedRatesOverlay: " + ex.Message);
                }
            }
            else
            {
                AppliedOverlayTimer.Enabled = false;
                overlayService.Reset();
                RemoveOverlay(AppliedOverlay);
                legendManager?.Clear();
                Refresh();
                MapChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void ShowZoneOverlay(bool Show)
        {
            Props.MapShowZones = Show;
            if (Props.MapShowZones)
            {
                try
                {
                    if (zoneOverlay == null) zoneOverlay = new GMapOverlay("mapzones");
                    AddOverlay(zoneOverlay);
                    Refresh();
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("MapManager/ShowZoneOverlay: " + ex.Message);
                }
            }
            else
            {
                RemoveOverlay(zoneOverlay);
                Refresh();
            }
        }

        public void UpdateTargetRates()
        {
            try
            {
                bool Found = false;

                if (zoneIndex != null)
                {
                    // Query spatial index for candidate zones near the tractor
                    var ptEnv = new Envelope(cTractorPosition.Lng, cTractorPosition.Lng, cTractorPosition.Lat, cTractorPosition.Lat);
                    var candidates = zoneIndex.Query(ptEnv);
                    if (candidates != null && candidates.Count > 0)
                    {
                        // emulate previous last-wins behavior: zones added later have priority
                        foreach (var zone in candidates.OrderByDescending(z => mapZones.IndexOf(z)))
                        {
                            if (zone.Contains(cTractorPosition))
                            {
                                cZoneName = zone.Name;
                                cZoneRates[0] = zone.Rates["ProductA"];
                                cZoneRates[1] = zone.Rates["ProductB"];
                                cZoneRates[2] = zone.Rates["ProductC"];
                                cZoneRates[3] = zone.Rates["ProductD"];
                                cZoneColor = zone.ZoneColor;
                                cZoneHectares = zone.Hectares();
                                Found = true;
                                break;
                            }
                        }
                    }
                }

                if (!Found)
                {
                    // Fallback to linear scan if index empty or no candidate matched
                    for (int i = mapZones.Count - 1; i >= 0; i--)
                    {
                        var zone = mapZones[i];
                        if (zone.Contains(cTractorPosition))
                        {
                            cZoneName = zone.Name;
                            cZoneRates[0] = zone.Rates["ProductA"];
                            cZoneRates[1] = zone.Rates["ProductB"];
                            cZoneRates[2] = zone.Rates["ProductC"];
                            cZoneRates[3] = zone.Rates["ProductD"];
                            cZoneColor = zone.ZoneColor;
                            cZoneHectares = zone.Hectares();
                            Found = true;
                            break;
                        }
                    }
                }

                if (!Found)
                {
                    cZoneName = "-";
                    cZoneRates[0] = 0;
                    cZoneRates[1] = 0;
                    cZoneRates[2] = 0;
                    cZoneRates[3] = 0;
                    cZoneColor = Color.Blue;
                    cZoneHectares = 0;
                }
            }
            catch (Exception ex)
            {
                Props.ShowMessage("MapManager.UpdateRates: " + ex.Message, "Error", 20000, true);
                throw;
            }
        }

        public bool UpdateZone(string name, double Rt0, double Rt1, double Rt2, double Rt3, Color zoneColor)
        {
            bool Result = false;
            if (currentZoneVertices.Count > 2)
            {
                var geometryFactory = new GeometryFactory();
                var coordinates = currentZoneVertices.ConvertAll(p => new Coordinate(p.Lng, p.Lat)).ToArray();

                if (!coordinates[0].Equals(coordinates[coordinates.Length - 1]))
                {
                    Array.Resize(ref coordinates, coordinates.Length + 1);
                    coordinates[coordinates.Length - 1] = coordinates[0];
                }
                var polygon = geometryFactory.CreatePolygon(coordinates);

                if (name == "") name = "Zone " + mapZones.Count.ToString();

                var mapZone = new MapZone(name, polygon, new Dictionary<string, double>
                    {
                        { "ProductA", Rt0 },
                        { "ProductB", Rt1 },
                        { "ProductC", Rt2 },
                        { "ProductD", Rt3 }
                    }, zoneColor);
                mapZones.Add(mapZone);
                zoneOverlay = AddPolygons(zoneOverlay, mapZone.ToGMapPolygons());

                currentZoneVertices.Clear();
                tempMarkerOverlay.Markers.Clear();

                BuildZoneIndex();
                Result = true;
            }
            else
            {
                bool Found = false;
                for (int i = mapZones.Count - 1; i >= 0; i--)
                {
                    var zone = mapZones[i];
                    if (zone.Contains(cTractorPosition))
                    {
                        zone.Name = name;
                        zone.Rates = new Dictionary<string, double>
                        {
                            { "ProductA", Rt0 },
                            { "ProductB", Rt1 },
                            { "ProductC", Rt2 },
                            { "ProductD", Rt3 }
                        };
                        zone.ZoneColor = zoneColor;
                        Found = true;
                        SaveMap(false);
                        LoadMap();
                        break;
                    }
                }
                Result = Found;
            }
            return Result;
        }

        public void ZoomToFit()
        {
            RectLatLng boundingBox = GetOverallRectLatLng();
            if (boundingBox != RectLatLng.Empty)
            {
                gmap.SetZoomToFitRect(boundingBox);
            }
        }

        private void AddOverlay(GMapOverlay NewOverlay)
        {
            if (NewOverlay != null)
            {
                bool overlayExists = gmap.Overlays.Any(o => o.Id == NewOverlay.Id);
                if (!overlayExists) gmap.Overlays.Add(NewOverlay);
                // keep legend host on top
                EnsureLegendTop();
            }
        }

        private GMapOverlay AddPolygons(GMapOverlay overlay, List<GMapPolygon> polygons)
        {
            foreach (var polygon in polygons)
            {
                // remove stroke to match AOG polygon look overlap-free
                polygon.Stroke = Pens.Transparent;
                overlay.Polygons.Add(polygon);
            }
            return overlay;
        }

        private void AddToCache()
        {
            var area = gmap.ViewArea;
            for (int zoom = 16; zoom < 17; zoom++)
            {
                var mapProvider = gmap.MapProvider;
                var projection = mapProvider.Projection;

                int tileSize = (int)projection.TileSize.Width;

                GPoint topLeftPixel = projection.FromLatLngToPixel(area.Top, area.Left, zoom);
                GPoint bottomRightPixel = projection.FromLatLngToPixel(area.Bottom, area.Right, zoom);

                GPoint topLeftTile = new GPoint(topLeftPixel.X / tileSize, topLeftPixel.Y / tileSize);
                GPoint bottomRightTile = new GPoint(bottomRightPixel.X / tileSize, bottomRightPixel.Y / tileSize);

                for (long x = topLeftTile.X; x <= bottomRightTile.X; x++)
                {
                    for (long y = bottomRightTile.Y; y >= topLeftTile.Y; y--)
                    {
                        GPoint tilePoint = new GPoint(x, y);
                        var tile = mapProvider.GetTileImage(tilePoint, zoom);
                        if (tile != null) tile.Dispose();
                    }
                }
            }
        }

        private void AppliedOverlayTimer_Tick(object sender, EventArgs e)
        {
            // Avoid rebuilding overlay if a recent live update already refreshed it
            if (DateTime.UtcNow - lastAppliedOverlayUpdateUtc < MinAppliedOverlayUpdate) return;
            cLegend = ShowAppliedLayer();
            lastAppliedOverlayUpdateUtc = DateTime.UtcNow;
        }

        private void BuildZoneIndex()
        {
            try
            {
                zoneIndex = new STRtree<MapZone>();
                if (mapZones != null)
                {
                    foreach (var z in mapZones)
                    {
                        if (z?.Geometry == null) continue;
                        var env = z.Geometry.EnvelopeInternal;
                        if (env == null) continue;
                        zoneIndex.Insert(env, z);
                    }
                    zoneIndex.Build();
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapManager/BuildZoneIndex: " + ex.Message);
                zoneIndex = null; // fallback gracefully
            }
        }

        // Center to applied data area if there are recorded non-zero applied readings
        private bool CenterOnAppliedDataIfAvailable()
        {
            try
            {
                var collector = Props.RateCollector;
                if (collector == null) return false;

                var readings = collector.GetReadings();
                if (readings == null || readings.Count == 0) return false;

                const double eps = 1e-3; // consider >0 as applied
                double minLat = double.MaxValue, maxLat = double.MinValue;
                double minLng = double.MaxValue, maxLng = double.MinValue;
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

                if (!any) return false;

                double width = Math.Max(maxLng - minLng, 0.0008);   // ensure reasonable width
                double height = Math.Max(maxLat - minLat, 0.0006);  // ensure reasonable height
                // pad a little
                double padW = width * 0.15;
                double padH = height * 0.15;
                var rect = new RectLatLng(maxLat + padH, minLng - padW, width + 2 * padW, height + 2 * padH);

                gmap.SetZoomToFitRect(rect);
                MapChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog($"MapManager/CenterOnAppliedDataIfAvailable: {ex.Message}");
                return false;
            }
        }

        private void EnsureLegendTop()
        {
            try
            {
                legendManager?.EnsureLegendTop();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapManager/EnsureLegendTop: " + ex.Message);
            }
        }

        private void Gmap_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (EditModePolygons)
                {
                    var point = gmap.FromLocalToLatLng(e.X, e.Y);
                    currentZoneVertices.Add(point);
                    tempMarkerOverlay.Markers.Add(new GMarkerGoogle(point, GMarkerGoogleType.red_small));
                }
                else
                {
                    PointLatLng Location = gmap.FromLocalToLatLng(e.X, e.Y);
                    cTractorPosition = Location;
                    tractorMarker.Position = Location;

                    // Prevent plotting at this clicked position in CoverageTrail until a real GPS update arrives
                    cSuppressTrailUntilNextGps = true;

                    Refresh();
                    MapChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void Gmap_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                isDragging = true;
                lastMousePosition = e.Location;
            }
        }

        private void Gmap_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                var deltaX = e.Location.X - lastMousePosition.X;
                var deltaY = e.Location.Y - lastMousePosition.Y;

                gmap.Position = new PointLatLng(
                    gmap.Position.Lat - (deltaY * 0.0001),
                    gmap.Position.Lng + (deltaX * 0.0001)
                );

                lastMousePosition = e.Location;
            }
        }

        private void Gmap_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                isDragging = false;
            }
        }

        private void Gmap_OnMapZoomChanged()
        {
            // Reposition legend overlay with the current view
            legendManager?.OnMapZoomChanged();
            MapChanged?.Invoke(this, EventArgs.Empty);
        }

        private void InitializeMap()
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            GMaps.Instance.PrimaryCache = new GMap.NET.CacheProviders.SQLitePureImageCache
            {
                CacheLocation = Props.MapCache
            };

            // Be polite: longer TTL reduces re-fetching tiles
            GMapProvider.TTLCache = 24 * 60; // minutes (e.g., 24 hours)

            double Lat = 0;
            double Lng = TimeZoneInfo.Local.BaseUtcOffset.TotalHours * 15.0;    // estimated longitude
            if (double.TryParse(Props.GetProp("LastMapLat"), out double latpos)) Lat = latpos;
            if (double.TryParse(Props.GetProp("LastMapLng"), out double lngpos)) Lng = lngpos;

            gmap = new GMapControl
            {
                Dock = DockStyle.Fill,
                Position = new PointLatLng(Lat, Lng),
                MinZoom = 2,
                MaxZoom = 20,
                Zoom = 16,
                ShowCenter = false
            };

            // Use the ArcGIS provider when tiles are enabled
            gmap.MapProvider = Props.MapShowTiles
                ? (GMapProvider)ArcGIS_World_Imagery_Provider.Instance
                : (GMapProvider)GMapProviders.EmptyProvider;

            gmap.MouseClick += Gmap_MouseClick;

            gmap.Zoom = 2;

            zoneOverlay = new GMapOverlay("mapzones");
            gpsMarkerOverlay = new GMapOverlay("gpsMarkers");
            tempMarkerOverlay = new GMapOverlay("tempMarkers");
            AppliedOverlay = new GMapOverlay("AppliedRates");

            tractorMarker = new GMarkerGoogle(new PointLatLng(Lat, Lng), GMarkerGoogleType.green);
            gpsMarkerOverlay.Markers.Add(tractorMarker);

            AddOverlay(gpsMarkerOverlay);
            AddOverlay(tempMarkerOverlay);
            AddOverlay(zoneOverlay);
            AddOverlay(AppliedOverlay);
            Refresh();

            // Initialize legend manager and host
            legendManager = new LegendManager(gmap);
        }

        private void InitializeMapZones()
        {
            mapZones = new List<MapZone>();
            currentZoneVertices = new List<PointLatLng>();
        }

        private void Props_JobChanged(object sender, EventArgs e)
        {
            LoadMap();
        }

        private void Props_MapShowRatesChanged(object sender, EventArgs e)
        {
            try
            {
                if (Props.MapShowRates)
                {
                    overlayService.Reset();

                    if (AppliedOverlay == null) AppliedOverlay = new GMapOverlay("AppliedRates");
                    AddOverlay(AppliedOverlay);
                    cLegend = ShowAppliedLayer();
                }
                else
                {
                    AppliedOverlayTimer.Enabled = false;
                    overlayService.Reset();
                    RemoveOverlay(AppliedOverlay);
                    legendManager?.Clear();
                    Refresh();
                    MapChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapManager/Props_MapShowRatesChanged: " + ex.Message);
            }
        }

        private void Refresh()
        {
            if (Props.RateMapIsVisible())
            {
                gmap.Refresh();
            }
        }

        private void RemoveOverlay(GMapOverlay overlay)
        {
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
                Props.WriteErrorLog("MapManager/RemoveLayer: " + ex.Message);
            }
        }
    }
}