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
using System.Linq;
using System.Windows.Forms;

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
        private static bool cShowTiles;
        private static bool cShowZones;

        #endregion Saved Properties

        #region CurrentZone

        private static MapZone CurrentZone = null;
        private static Color CurrentZoneColor;
        private static double CurrentZoneHectares;
        private static string CurrentZoneName = "";
        private static double[] CurrentZoneRates;

        #endregion CurrentZone

        private static readonly KmlLayerManager kmlLayerManager = new KmlLayerManager();
        private static readonly RateOverlayService overlayService = new RateOverlayService();
        private static int _lastHistoryCount;
        private static DateTime _lastHistoryLastTimestamp;
        private static string _lastLoadedMapPath;
        private static bool cMapIsDisplayed = false;
        private static Dictionary<string, Color> ColorLegend;
        private static MapState cState;
        private static PointLatLng cTractorPosition;
        private static double cTravelHeading;
        private static List<PointLatLng> currentZoneVertices;
        private static LegendManager legendManager;
        private static List<MapZone> mapZones;
        private static STRtree<MapZone> STRtreeZoneIndex;
        private static System.Windows.Forms.Timer UpdateTimer;
        private static byte ZoneTransparency = 190;

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
                    cState = MapState.Positioning;
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
            get { return legendManager != null && legendManager.LegendOverlayEnabled; }
            set
            {
                if (legendManager != null)
                {
                    legendManager.LegendOverlayEnabled = value;
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

        public static bool ShowRates
        {
            get { return cShowRates; }
            set
            {
                cShowRates = value;
                Props.SetProp("MapShowRates", cShowRates.ToString());
                ShowRatesOverlay();
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
                Refresh();
            }
        }

        public static bool ShowZones
        {
            get { return cShowZones; }
            set
            {
                cShowZones = value;
                Props.SetProp("MapShowZones", cShowZones.ToString());
                ShowZoneOverlay();
            }
        }

        public static Color ZoneColor => CurrentZoneColor;

        public static int ZoneCount
        { get { return STRtreeZoneIndex.Count; } }

        public static bool ZoneFound
        { get { return CurrentZone != null; } }

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
                var overlay = kmlLayerManager.LoadKml(filePath);
                if (overlay == null) return false;

                AddOverlay(overlay);
                Refresh();
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
                overlayService.Reset();
                if (AppliedOverlay != null) AppliedOverlay.Polygons.Clear();
                legendManager?.Clear();
                ColorLegend = null;
                Refresh();
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
                JobManager.JobChanged -= JobManager_JobChanged;
                Props.RateDataSettingsChanged -= Props_RateDataSettingsChanged;

                Props.SetProp("LastMapLat", gmap.Position.Lat.ToString("N10"));
                Props.SetProp("LastMapLng", gmap.Position.Lng.ToString("N10"));

                gmap.OnMapZoomChanged -= Gmap_OnMapZoomChanged;
                gmap.MouseClick -= Gmap_MouseClick;

                UpdateTimer.Enabled = false;
                UpdateTimer.Tick -= UpdateTimer_Tick;

                legendManager?.Dispose();
                AppliedOverlay?.Polygons.Clear();
                zoneOverlay?.Polygons.Clear();
                tempMarkerOverlay?.Markers.Clear();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/Close: " + ex.Message);
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
                    List<GMapPolygon> polygonsToRemove = zone.ToGMapPolygons(ZoneTransparency);
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
                CenterMap();
            }
            else
            {
                cState = MapState.Tracking;
            }
            ShowLegend(ColorLegend);
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

        public static double GetRate(int RateID)
        {
            double Result = 0.0;
            if (RateID >= 0 && RateID < (Props.MaxProducts - 2)) Result = CurrentZoneRates[RateID];
            return Result;
        }

        public static void Initialize()
        {
            LoadData();
            JobManager.JobChanged += JobManager_JobChanged;
            Props.RateDataSettingsChanged += Props_RateDataSettingsChanged;

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
                    zoneOverlay = AddPolygons(zoneOverlay, mapZone.ToGMapPolygons(ZoneTransparency));
                }

                BuildZoneIndex();
                ShowZoneOverlay();

                ShowRatesOverlay();
                Refresh();
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

        // Remove a KML layer previously added.
        public static void RemoveKmlLayer(string filePath)
        {
            try
            {
                var overlay = kmlLayerManager.GetOverlay(filePath);
                if (overlay == null) return;

                RemoveOverlay(overlay);
                kmlLayerManager.Remove(filePath);
                Refresh();
                MapChanged?.Invoke(null, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/RemoveKmlLayer: " + ex.Message);
            }
        }

        public static void ResetMarkers()
        {
            currentZoneVertices.Clear();
            tempMarkerOverlay.Markers.Clear();
        }

        public static bool SaveMap()
        {
            bool Result = false;
            try
            {
                var shapefileHelper = new ShapefileHelper();
                shapefileHelper.SaveMapZones(JobManager.CurrentMapPath, mapZones);
                Result = true;
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
            var shapefileHelper = new ShapefileHelper();

            try
            {
                var features = new List<NetTopologySuite.Features.IFeature>();

                // Collect zone features
                if (mapZones != null && mapZones.Count > 0)
                {
                    foreach (var mapZone in mapZones)
                    {
                        var zoneAtts = new NetTopologySuite.Features.AttributesTable
                        {
                            { "Name", mapZone.Name },
                            { "ProductA", mapZone.Rates["ProductA"] },
                            { "ProductB", mapZone.Rates["ProductB"] },
                            { "ProductC", mapZone.Rates["ProductC"] },
                            { "ProductD", mapZone.Rates["ProductD"] },
                            { "Color", ColorTranslator.ToHtml(mapZone.ZoneColor) },
                            { "Type", "Zone" }
                        };
                        features.Add(new NetTopologySuite.Features.Feature(mapZone.Geometry, zoneAtts));
                    }
                }

                // Collect applied overlay features (coverage polygons)
                if (AppliedOverlay == null) AppliedOverlay = new GMapOverlay("AppliedRates");

                bool haveCoverage = false;
                if (AppliedOverlay.Polygons != null && AppliedOverlay.Polygons.Count > 0)
                {
                    haveCoverage = true;
                }
                else
                {
                    var readings = Props.RateCollector?.GetReadings();
                    if (readings != null && readings.Count > 0)
                    {
                        AppliedOverlay.Polygons.Clear();

                        double implementWidth = 24.0;
                        if (Props.MainForm.Sections != null)
                        {
                            try { implementWidth = Props.MainForm.Sections.TotalWidth(false); }
                            catch (Exception ex) { Props.WriteErrorLog($"MapController: implement width - {ex.Message}"); }
                        }

                        Dictionary<string, Color> legendFromHistory;
                        haveCoverage = overlayService.BuildFromHistory(
                            AppliedOverlay,
                            readings,
                            implementWidth,
                            cRateTypeDisplay,
                            cProductRates,
                            out legendFromHistory
                        );
                    }
                }

                if (haveCoverage && AppliedOverlay.Polygons != null && AppliedOverlay.Polygons.Count > 0)
                {
                    foreach (var gp in AppliedOverlay.Polygons)
                    {
                        if (gp == null || gp.Points == null || gp.Points.Count < 3) continue;

                        var coords = new List<NetTopologySuite.Geometries.Coordinate>(gp.Points.Count + 1);
                        for (int i = 0; i < gp.Points.Count; i++)
                        {
                            var p = gp.Points[i];
                            coords.Add(new NetTopologySuite.Geometries.Coordinate(p.Lng, p.Lat));
                        }
                        if (!coords[0].Equals2D(coords[coords.Count - 1]))
                        {
                            coords.Add(coords[0]);
                        }

                        var ring = new NetTopologySuite.Geometries.LinearRing(coords.ToArray());
                        var poly = new NetTopologySuite.Geometries.Polygon(ring);

                        Color fillColor = Color.Transparent;
                        try
                        {
                            var sb = gp.Fill as SolidBrush;
                            if (sb != null) fillColor = sb.Color;
                        }
                        catch { }

                        var covAtts = new NetTopologySuite.Features.AttributesTable
                        {
                            { "Name", string.IsNullOrEmpty(gp.Name) ? "Coverage" : gp.Name },
                            { "Color", ColorTranslator.ToHtml(Color.FromArgb(255, fillColor)) },
                            { "Alpha", fillColor.A },
                            { "Type", "Coverage" }
                        };

                        features.Add(new NetTopologySuite.Features.Feature(poly, covAtts));
                    }
                }

                if (features.Count > 0)
                {
                    NetTopologySuite.IO.Esri.Shapefile.WriteAllFeatures(features, filePath);
                    return;
                }

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
                    Props.WriteErrorLog("MapController/SaveMapToFile cleanup: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/SaveMapToFile combined: " + ex.Message);
            }
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
                    CurrentZoneRates = Props.MainForm.Products.ProductsRateSet();
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

        public static bool UpdateZone(string name, double Rt0, double Rt1, double Rt2, double Rt3, Color zoneColor)
        {
            bool Result = false;
            MapZone ZoneToEdit = GetExistingZone();
            if (ZoneToEdit == null)
            {
                // create a new zone
                if (currentZoneVertices.Count > 2 && !ZoneNameFound(name))
                {
                    var geometryFactory = new GeometryFactory();
                    var coordinates = currentZoneVertices.ConvertAll(p => new Coordinate(p.Lng, p.Lat)).ToArray();

                    if (!coordinates[0].Equals(coordinates[coordinates.Length - 1]))
                    {
                        Array.Resize(ref coordinates, coordinates.Length + 1);
                        coordinates[coordinates.Length - 1] = coordinates[0];
                    }
                    var polygon = geometryFactory.CreatePolygon(coordinates);

                    ZoneToEdit = new MapZone(name, polygon, new Dictionary<string, double>
                    {
                        { "ProductA", Rt0 },
                        { "ProductB", Rt1 },
                        { "ProductC", Rt2 },
                        { "ProductD", Rt3 }
                    }, zoneColor);

                    mapZones.Add(ZoneToEdit);
                    zoneOverlay = AddPolygons(zoneOverlay, ZoneToEdit.ToGMapPolygons(ZoneTransparency));

                    currentZoneVertices.Clear();
                    tempMarkerOverlay.Markers.Clear();

                    BuildZoneIndex();
                    Result = true;
                    UpdateVariableRates();
                    Refresh();
                    MapChanged?.Invoke(null, EventArgs.Empty);
                }
            }
            else
            {
                // update zone
                if (!ZoneNameFound(name, ZoneToEdit))
                {
                    // Update properties
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
                        var polygonsForZone = ZoneToEdit.ToGMapPolygons(ZoneTransparency);
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

                    // Reorder to keep last-wins priority
                    mapZones.Remove(ZoneToEdit);
                    mapZones.Add(ZoneToEdit);

                    currentZoneVertices.Clear();
                    tempMarkerOverlay.Markers.Clear();

                    BuildZoneIndex();
                    Result = true;
                    UpdateVariableRates();
                    Refresh();
                    MapChanged?.Invoke(null, EventArgs.Empty);
                }
            }

            return Result;
        }

        public static bool ZoomToFit()
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

        private static void BuildCoverageFromHistory()
        {
            try
            {
                if (AppliedOverlay == null) AppliedOverlay = new GMapOverlay("AppliedRates");
                overlayService.Reset();
                AppliedOverlay.Polygons.Clear();
                AddOverlay(AppliedOverlay);

                Props.RateCollector.LoadData(); // ensure fresh data
                var readings = Props.RateCollector.GetReadings();
                if (readings == null || readings.Count == 0)
                {
                    legendManager?.Clear();
                    ColorLegend = null;
                    Refresh();
                }
                else if (Props.MainForm.Sections != null)
                {
                    Dictionary<string, Color> histLegend;
                    bool histOk = overlayService.BuildFromHistory(
                        AppliedOverlay,
                        readings,
                        Props.MainForm.Sections.TotalWidth(false),
                        cRateTypeDisplay,
                        cProductRates,
                        out histLegend
                    );
                    ColorLegend = histLegend;
                    ShowLegend(histLegend, histOk);

                    Refresh();
                    MapChanged?.Invoke(null, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/BuildCoverageFromHistory: " + ex.Message);
            }
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

        private static MapZone GetExistingZone()
        {
            // is pointer in an existing zone, load zone
            MapZone Result = null;
            if (gmap != null && cTractorPosition != null)
            {
                foreach (var zn in mapZones)
                {
                    if (zn.Contains(cTractorPosition))
                    {
                        Result = zn;
                        break;
                    }
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
                        currentZoneVertices.Add(point);
                        tempMarkerOverlay.Markers.Add(new GMarkerGoogle(point, GMarkerGoogleType.red_small));
                        break;

                    case MapState.Tracking:
                        MapLeftClicked?.Invoke(null, EventArgs.Empty);
                        break;

                    case MapState.Positioning:
                        PointLatLng Location = gmap.FromLocalToLatLng(e.X, e.Y);
                        cTractorPosition = Location;
                        tractorMarker.Position = Location;
                        UpdateVariableRates();
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
            tempMarkerOverlay = new GMapOverlay("tempMarkers");
            AppliedOverlay = new GMapOverlay("AppliedRates");

            tractorMarker = new GMarkerGoogle(new PointLatLng(Lat, Lng), GMarkerGoogleType.green);
            tractorMarker.IsHitTestVisible = false;
            gpsMarkerOverlay.Markers.Add(tractorMarker);

            PointLatLng Location = new PointLatLng(Lat, Lng);
            cTractorPosition = Location;
            tractorMarker.Position = Location;
            UpdateVariableRates();

            AddOverlay(gpsMarkerOverlay);
            AddOverlay(tempMarkerOverlay);
            AddOverlay(zoneOverlay);
            AddOverlay(AppliedOverlay);
            Refresh();
        }

        private static void JobManager_JobChanged(object sender, EventArgs e)
        {
            LoadMap();
        }

        private static void LoadData()
        {
            cProductRates = int.TryParse(Props.GetProp("MapProductRates"), out int pr) ? pr : 0;
            cRateTypeDisplay = Enum.TryParse(Props.GetProp("RateDisplayType"), out RateType tp) ? tp : RateType.Applied;
            cShowRates = bool.TryParse(Props.GetProp("MapShowRates"), out bool sr) ? sr : false;
            cShowZones = bool.TryParse(Props.GetProp("MapShowZones"), out bool sz) ? sz : true;
            cShowTiles = bool.TryParse(Props.GetProp("MapShowTiles"), out bool st) ? st : true;
        }

        private static void Props_RateDataSettingsChanged(object sender, EventArgs e)
        {
            try
            {
                if (cShowRates)
                {
                    overlayService.Reset();

                    if (AppliedOverlay == null) AppliedOverlay = new GMapOverlay("AppliedRates");
                    AddOverlay(AppliedOverlay);
                }
                else
                {
                    overlayService.Reset();
                    RemoveOverlay(AppliedOverlay);
                    legendManager?.Clear();
                    Refresh();
                    MapChanged?.Invoke(null, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/Props_RateDataSettingsChanged: " + ex.Message);
            }
        }

        private static void Refresh()
        {
            gmap.Refresh();
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
                   AppliedOverlay != null &&
                   AppliedOverlay.Polygons != null &&
                   AppliedOverlay.Polygons.Count > 0;
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
                //LegendToShow = null;
            }
        }

        private static void ShowRatesOverlay()
        {
            try
            {
                if (cShowRates)
                {
                    // Decide whether to rebuild coverage from history
                    var readings = Props.RateCollector?.GetReadings();
                    bool SkipRebuild = ShouldSkipHistoryBuild(JobManager.CurrentMapPath, readings);

                    if (SkipRebuild)
                    {
                        AddOverlay(AppliedOverlay);
                        ShowLegend(ColorLegend);
                    }
                    else
                    {
                        BuildCoverageFromHistory();

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
                    }
                    Refresh();
                    MapChanged?.Invoke(null, EventArgs.Empty);
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

        private static void ShowZoneOverlay()
        {
            if (cShowZones)
            {
                try
                {
                    if (zoneOverlay == null) zoneOverlay = new GMapOverlay("mapzones");
                    AddOverlay(zoneOverlay);
                    Refresh();
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("MapController/ShowZoneOverlay: " + ex.Message);
                }
            }
            else
            {
                RemoveOverlay(zoneOverlay);
                Refresh();
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
                    Props.RateCollector.RecordReading(Position.Lat, Position.Lng, Props.MainForm.Products.ProductAppliedRates(), Props.MainForm.Products.ProductsRateSet());
                    if (cShowRates && cMapIsDisplayed) UpdateRateLayer(Props.MainForm.Products.ProductAppliedRates(), Props.MainForm.Products.ProductsRateSet());
                }
            }
            UpdateVariableRates();
        }

        private static bool ZoneNameFound(string Name, MapZone ExcludeZone = null)
        {
            bool Result = false;
            foreach (MapZone zn in mapZones)
            {
                if (zn.Name == Name && zn != ExcludeZone)
                {
                    Result = true;
                    break;
                }
            }
            return Result;
        }
    }
}