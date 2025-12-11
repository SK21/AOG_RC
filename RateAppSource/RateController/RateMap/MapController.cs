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
using System.Linq;
using System.Security.Policy;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace RateController.RateMap
{
    public enum MapState
    { Preview, EditZones, Tracking, Positioning }

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
        private static GMapOverlay tempMarkerOverlay;
        private static GMapOverlay zoneOverlay;

        #endregion Overlays

        #region Saved Properties

        private static int cProductRates;
        private static RateType cRateTypeDisplay;
        private static bool cShowRates;

        #endregion Saved Properties

        #region CurrentZone

        private static Color CurrentZoneColor;
        private static double CurrentZoneHectares;
        private static string CurrentZoneName = "";
        private static double[] CurrentZoneRates;

        #endregion CurrentZone

        private static readonly RateOverlayService overlayService = new RateOverlayService();
        private static bool cMapIsDisplayed = false;
        private static Dictionary<string, Color> ColorLegend;
        private static MapState cState;
        private static PointLatLng cTractorPosition;
        private static double cTravelHeading;
        private static MapZone CurrentZone;
        private static List<PointLatLng> currentZoneVertices;
        private static DateTime lastAppliedOverlayUpdateUtc = DateTime.MinValue;
        private static LegendManager legendManager;
        private static List<MapZone> mapZones;
        private static STRtree<MapZone> STRtreeZoneIndex;
        private static System.Windows.Forms.Timer UpdateTimer;
        // spatial index for fast zone lookup

        public static event EventHandler MapChanged;

        public static event EventHandler MapLeftClicked;

        public static event EventHandler MapZoomed;

        public static bool LegendOverlayEnabled
        {
            get { return legendManager != null && legendManager.LegendOverlayEnabled; }
            set
            {
                if (legendManager != null)
                {
                    legendManager.LegendOverlayEnabled = value;
                    legendManager.UpdateLegend(ColorLegend);
                }
            }
        }

        public static GMapControl Map
        { get { return gmap; } }

        public static bool MapIsDisplayed
        { get { return cMapIsDisplayed; } set { cMapIsDisplayed = value; } }

        public static int ProductRates
        {
            get { return cProductRates; }
            set
            {
                if (value >= 0 && (value < Props.MaxProducts - 2))
                {
                    cProductRates = value;
                    Props.SetProp("MapProductRates", cProductRates.ToString());
                }
            }
        }

        public static RateType RateTypeDisplay
        {
            get { return cRateTypeDisplay; }
            set
            {
                cRateTypeDisplay = value;
                Props.SetProp("RateDisplayType", value.ToString());
            }
        }

        public static bool ShowRates
        {
            get { return cShowRates; }
            set
            {
                cShowRates = value;
                Props.SetProp("MapShowRates", cShowRates.ToString());
            }
        }

        public static bool ShowTiles
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
        public static double ZoneHectares => CurrentZoneHectares;

        public static bool StateEditZones
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

        public static Color ZoneColor => CurrentZoneColor;

        public static void CenterMap()
        {
            bool Centered = false;
            try
            {
                var collector = Props.RateCollector;
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
                            Centered = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/CenterMap: " + ex.Message);
            }
            if (!Centered) ZoomToFit();
        }

        public static void ClearAppliedRatesOverlay()
        {
            try
            {
                overlayService.Reset();
                if (AppliedOverlay != null) AppliedOverlay.Polygons.Clear();
                legendManager?.Clear();
                ColorLegend = null;
                Refresh();
                MapChanged?.Invoke(null, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapManager/ClearAppliedRatesOverlay: " + ex.Message);
            }
        }

        public static void Close()
        {
            Props.SetProp("LastMapLat", gmap.Position.Lat.ToString("N10"));
            Props.SetProp("LastMapLng", gmap.Position.Lng.ToString("N10"));

            gmap.OnMapZoomChanged -= Gmap_OnMapZoomChanged;
            gmap.MouseClick -= Gmap_MouseClick;
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
        }

        public static RectLatLng GetOverallRectLatLng()
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

        public static void Initialize()
        {
            CurrentZoneRates = new double[Props.MaxProducts - 2];
            cState = MapState.Tracking;
            InitializeMap();
            legendManager = new LegendManager(gmap);

            // map zones
            mapZones = new List<MapZone>();
            currentZoneVertices = new List<PointLatLng>();

            gmap.OnMapZoomChanged += Gmap_OnMapZoomChanged;
            gmap.MouseClick += Gmap_MouseClick;

            UpdateTimer = new System.Windows.Forms.Timer();
            UpdateTimer.Interval = 1000; // ms
            UpdateTimer.Tick += UpdateTimer_Tick;
            UpdateTimer.Enabled = true;

            LoadData();
            UpdateCurrentZone();
            LoadMap();
        }

        public static bool LoadMap()
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
                ShowRatesOverlay();
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

        public static void SetTractorPosition(PointLatLng NewLocation)
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
            }
        }

        public static bool ShowHistoricalRateLayer()
        {
            bool Result = false;
            try
            {
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/ShowHistoricalRateLayer: " + ex.Message);
            }
            return Result;
        }

        public static void ShowRatesOverlay()
        {
            try
            {
                if (cShowRates)
                {
                    if (cMapIsDisplayed)
                    {
                        if (AppliedOverlay == null) AppliedOverlay = new GMapOverlay("AppliedRates");
                        overlayService.Reset();
                        AppliedOverlay.Polygons.Clear();
                        AddOverlay(AppliedOverlay);
                    }
                }
                else
                {
                    // remove rates overlay
                    overlayService.Reset();
                    RemoveOverlay(AppliedOverlay);
                    legendManager?.Clear();
                    Refresh();
                    MapChanged?.Invoke(null, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/ShowRatesOverlay: " + ex.Message);
            }
        }

        public static bool UpdateRateLayer(double[] AppliedRates, double[] TargetRates)
        {
            bool Result = false;
            Dictionary<string, Color> legend = new Dictionary<string, Color>();
            try
            {
                if (cShowRates && cMapIsDisplayed && (cState == MapState.Tracking || cState == MapState.Preview))
                {
                    var readings = Props.RateCollector.GetReadings();
                    if (readings == null || readings.Count == 0)
                    {
                        ClearAppliedRatesOverlay();
                    }
                    else
                    {
                        double Rates = AppliedRates[cProductRates];  // product rates to display

                        Result = overlayService.UpdateRatesOverlayLive(
                            AppliedOverlay,
                            readings,
                            cTractorPosition,
                            cTravelHeading,
                            Props.MainForm.Sections.TotalWidth(false),
                            Rates,
                            out legend,
                            cRateTypeDisplay,
                            cProductRates
                        );
                    }
                }
                ColorLegend = legend;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/UpdateRateLayer: " + ex.Message);
            }
            return Result;
        }

        public static void UpdateVariableRates()
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
                                break;
                            }
                        }
                    }
                }
                if (!ZoneFound)
                {
                    // use target rates
                    CurrentZoneName = "-";
                    CurrentZoneRates = Props.MainForm.Products.ProductTargetRates();
                    CurrentZoneColor = Color.Blue;
                    CurrentZoneHectares = 0;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/UpdateVariableRates: " + ex.Message);
            }
        }

        public static void ZoomToFit()
        {
            RectLatLng boundingBox = GetOverallRectLatLng();
            if (boundingBox != RectLatLng.Empty)
            {
                gmap.SetZoomToFitRect(boundingBox);
            }
        }

        private static void AddOverlay(GMapOverlay NewOverlay)
        {
            if (NewOverlay != null)
            {
                bool overlayExists = gmap.Overlays.Any(o => o.Id == NewOverlay.Id);
                if (!overlayExists) gmap.Overlays.Add(NewOverlay);
                // keep legend host on top
                //EnsureLegendTop();
                UpdateCurrentZone();
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

        private static void BuildZoneIndex()
        {
            // build a STRtree object for efficiently working with spatial objects (zones)
            try
            {
                STRtreeZoneIndex = new STRtree<MapZone>();
                if (mapZones != null)
                {
                    foreach (var z in mapZones)
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
                        currentZoneVertices.Add(point);
                        tempMarkerOverlay.Markers.Add(new GMarkerGoogle(point, GMarkerGoogleType.red_small));
                        break;

                    case MapState.Tracking:
                        break;

                    case MapState.Positioning:
                        PointLatLng Location = gmap.FromLocalToLatLng(e.X, e.Y);
                        cTractorPosition = Location;
                        tractorMarker.Position = Location;
                        Refresh();
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
                CacheLocation = Props.MapCache
            };

            GMapProvider.TTLCache = 24 * 60 * 7; // minutes (e.g., 24 hours) 7 days

            double Lat = 0;
            double Lng = TimeZoneInfo.Local.BaseUtcOffset.TotalHours * 15.0;    // estimated longitude
            if (double.TryParse(Props.GetProp("LastMapLat"), out double latpos)) Lat = latpos;
            if (double.TryParse(Props.GetProp("LastMapLng"), out double lngpos)) Lng = lngpos;

            gmap = new GMapControl();
            ShowTiles = Props.MapShowTiles;

            gmap.Position = new PointLatLng(Lat, Lng);
            gmap.ShowCenter = false;
            gmap.MinZoom = 1;
            gmap.MaxZoom = 19;
            gmap.Zoom = 15;
            gmap.Dock = DockStyle.Fill;

            // overlays
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
        }

        private static void LoadData()
        {
            cShowRates = bool.TryParse(Props.GetProp("MapShowRates"), out bool sr) ? sr : false;
            cProductRates = int.TryParse(Props.GetProp("MapProductRates"), out int pr) ? pr : 0;
            cRateTypeDisplay = Enum.TryParse(Props.GetProp("RateDisplayType"), out RateType tp) ? tp : RateType.Applied;
        }

        private static void Refresh()
        {
            if (cMapIsDisplayed)
            {
                gmap.Refresh();
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

        private static void UpdateCurrentZone()
        {
            try
            {
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/UpdateCurrentZone: " + ex.Message);
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
                    Props.RateCollector.RecordReading(Position.Lat, Position.Lng, Props.MainForm.Products.ProductAppliedRates(), Props.MainForm.Products.ProductTargetRates());
                    if (cShowRates && cMapIsDisplayed) UpdateRateLayer(Props.MainForm.Products.ProductAppliedRates(), Props.MainForm.Products.ProductTargetRates());
                }
            }
            UpdateVariableRates();
        }
    }
}