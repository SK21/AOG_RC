using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq; // Linq used throughout
using System.Windows.Forms;

namespace RateController.Classes
{
    public class MapManager
    {
        private const int MapRefreshSeconds = 2;

        private readonly RateOverlayService overlayService = new RateOverlayService();

        private GMapOverlay AppliedOverlay;
        private System.Windows.Forms.Timer AppliedOverlayTimer;
        private bool cEditPolygons = false;
        private Dictionary<string, Color> cLegend;
        private bool cMouseSetTractorPosition = false;
        private PointLatLng cTractorPosition;
        private double cTravelHeading;
        private List<PointLatLng> currentZoneVertices;
        private Color cZoneColor;
        private double cZoneHectares;
        private string cZoneName = "Unnamed Zone";
        private double[] cZoneRates = new double[4];
        private GMapControl gmap;
        private GMapOverlay gpsMarkerOverlay;
        private bool isDragging = false;
        private System.Drawing.Point lastMousePosition;

        // Legend overlay (bitmap marker anchored to top-right of the current view)
        private GMapOverlay legendOverlay;

        private List<MapZone> mapZones;
        private FormStart mf;
        private GMapOverlay tempMarkerOverlay;
        private GMarkerGoogle tractorMarker;
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

            Props.JobChanged += Props_JobChanged;
            Props.RateDataSettingsChanged += Props_MapShowRatesChanged;

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

        // NEW: allow form to control legend visibility (only show in full-screen)
        public bool LegendOverlayEnabled { get; set; } = false;

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
                    ? (GMapProvider)GMapProviders.BingSatelliteMap
                    : (GMapProvider)GMapProviders.EmptyProvider;
                gmap.Refresh();
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

        public void ClearAppliedRatesOverlay()
        {
            try
            {
                overlayService.Reset();
                if (AppliedOverlay != null) AppliedOverlay.Polygons.Clear();
                if (legendOverlay != null) legendOverlay.Markers.Clear();
                cLegend = null;
                gmap.Refresh();
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
            var zoneToRemove = mapZones.FirstOrDefault(zone => zone.Name == name);

            if (zoneToRemove != null)
            {
                List<GMapPolygon> polygonsToRemove = zoneToRemove.ToGMapPolygons();
                foreach (var polygonToRemove in polygonsToRemove)
                {
                    if (polygonToRemove != null)
                    {
                        var polygonInOverlay = zoneOverlay.Polygons
                            .FirstOrDefault(polygon => polygon.Points.SequenceEqual(polygonToRemove.Points));

                        if (polygonInOverlay != null)
                        {
                            zoneOverlay.Polygons.Remove(polygonInOverlay);
                            mapZones.Remove(zoneToRemove);

                            RemoveOverlay(zoneOverlay);
                            AddOverlay(zoneOverlay);

                            gmap.Refresh();
                            UpdateTargetRates();
                            SaveMap();
                            Result = true;
                        }
                    }
                }
            }
            return Result;
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
                    if (polygon.Points.Count == 0) continue;

                    minLat = Math.Min(minLat, polygon.Points.Min(p => p.Lat));
                    maxLat = Math.Max(maxLat, polygon.Points.Max(p => p.Lat));
                    minLng = Math.Min(minLng, polygon.Points.Min(p => p.Lng));
                    maxLng = Math.Max(maxLng, polygon.Points.Max(p => p.Lng));
                }

                Result = new RectLatLng(maxLat, minLng, maxLng - minLng, maxLat - minLat);
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
                var shapefileHelper = new ShapefileHelper(mf);

                mapZones = shapefileHelper.CreateZoneList(Props.CurrentMapPath);

                zoneOverlay.Polygons.Clear();
                foreach (var mapZone in mapZones)
                {
                    zoneOverlay = AddPolygons(zoneOverlay, mapZone.ToGMapPolygons());
                }

                gmap.Refresh();
                Result = true;
                MapChanged?.Invoke(this, EventArgs.Empty);
                ZoomToFit();
                ShowAppliedRatesOverlay();
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
            var shapefileHelper = new ShapefileHelper(mf);
            shapefileHelper.SaveMapZones(Props.CurrentMapPath, mapZones);
            if (UpdateCache) AddToCache();
            Result = true;
            return Result;
        }

        public void SaveMapToFile(string filePath)
        {
            var shapefileHelper = new ShapefileHelper(mf);
            shapefileHelper.SaveMapZones(filePath, mapZones);
        }

        public void SetTractorPosition(PointLatLng NewLocation, double[] AppliedRates, double[] TargetRates)
        {
            if (!cMouseSetTractorPosition)
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
                gmap.Refresh();
                UpdateTargetRates();
                if (mf.Products.ProductsAreOn())
                    mf.Tls.RateCollector.RecordReading(NewLocation.Lat, NewLocation.Lng, AppliedRates, TargetRates);
                MapChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public Dictionary<string, Color> ShowAppliedLayer()
        {
            Dictionary<string, Color> legend = new Dictionary<string, Color>();
            try
            {
                var readings = mf.Tls.RateCollector.GetReadings().ToList();
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

                if (cMouseSetTractorPosition)
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
                    success = overlayService.UpdateRatesOverlay(
                        AppliedOverlay,
                        readings,
                        cTractorPosition,
                        cTravelHeading,
                        implementWidth,
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

                // Draw/refresh legend overlay on the map (respects LegendOverlayEnabled)
                UpdateLegendOverlay();

                gmap.Refresh();
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
            if (Props.MapShowRates)
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
                if (legendOverlay != null) legendOverlay.Markers.Clear();
                gmap.Refresh();
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
                    gmap.Refresh();
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("MapManager/ShowZoneOverlay: " + ex.Message);
                }
            }
            else
            {
                RemoveOverlay(zoneOverlay);
                gmap.Refresh();
            }
        }

        public void UpdateTargetRates()
        {
            try
            {
                bool Found = false;
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
                    }, zoneColor, mf);
                mapZones.Add(mapZone);
                zoneOverlay = AddPolygons(zoneOverlay, mapZone.ToGMapPolygons());

                currentZoneVertices.Clear();
                tempMarkerOverlay.Markers.Clear();
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
            }
        }

        private GMapOverlay AddPolygons(GMapOverlay overlay, List<GMapPolygon> polygons)
        {
            foreach (var polygon in polygons)
            {
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
            cLegend = ShowAppliedLayer();
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
                    gmap.Refresh();
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
            UpdateLegendOverlay();
            MapChanged?.Invoke(this, EventArgs.Empty);
        }

        private void InitializeMap()
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            GMaps.Instance.PrimaryCache = new GMap.NET.CacheProviders.SQLitePureImageCache
            {
                CacheLocation = Props.MapCache
            };

            double Lat = 200;
            double Lng = 200;
            if (double.TryParse(Props.GetProp("LastMapLat"), out double latpos)) Lat = latpos;
            if (double.TryParse(Props.GetProp("LastMapLng"), out double lngpos)) Lng = lngpos;

            if (Lat < -90 || Lat > 90 || Lng < -180 || Lng > 180)
            {
                Lat = 52.157902;
                Lng = -106.670158;
            }

            gmap = new GMapControl
            {
                Dock = DockStyle.Fill,
                Position = new PointLatLng(Lat, Lng),
                MinZoom = 3,
                MaxZoom = 20,
                Zoom = 16,
                ShowCenter = false
            };

            gmap.MapProvider = Props.MapShowTiles
                ? (GMapProvider)GMapProviders.BingSatelliteMap
                : (GMapProvider)GMapProviders.EmptyProvider;
            gmap.MouseClick += Gmap_MouseClick;

            zoneOverlay = new GMapOverlay("mapzones");
            gpsMarkerOverlay = new GMapOverlay("gpsMarkers");
            tempMarkerOverlay = new GMapOverlay("tempMarkers");
            AppliedOverlay = new GMapOverlay("AppliedRates");
            legendOverlay = new GMapOverlay("legend");

            tractorMarker = new GMarkerGoogle(new PointLatLng(0, 0), GMarkerGoogleType.green);
            gpsMarkerOverlay.Markers.Add(tractorMarker);

            AddOverlay(gpsMarkerOverlay);
            AddOverlay(tempMarkerOverlay);
            AddOverlay(zoneOverlay);
            AddOverlay(AppliedOverlay);
            AddOverlay(legendOverlay);
            gmap.Refresh();
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
                    if (legendOverlay != null) legendOverlay.Markers.Clear();
                    gmap.Refresh();
                    MapChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapManager/Props_MapShowRatesChanged: " + ex.Message);
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
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapManager/RemoveLayer: " + ex.Message);
            }
        }

        private void UpdateLegendOverlay()
        {
            if (legendOverlay == null) return;

            legendOverlay.Markers.Clear();

            // Only render legend overlay if enabled, rates are on, and legend data available
            if (!LegendOverlayEnabled || !Props.MapShowRates || cLegend == null || cLegend.Count == 0)
            {
                gmap.Refresh();
                return;
            }

            // Build bitmap
            const int itemHeight = 25;
            const int leftMargin = 10;
            const int swatch = 20;
            const int gap = 10;
            const int rightMargin = 10;
            int legendHeight = (cLegend.Count * itemHeight) + (leftMargin * 2);
            int legendWidth = Math.Max(120, leftMargin + swatch + gap + 80 + rightMargin);

            var bmp = new Bitmap(legendWidth, legendHeight);
            using (var g2 = Graphics.FromImage(bmp))
            {
                g2.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g2.FillRectangle(new SolidBrush(Color.FromArgb(200, Color.White)), 0, 0, legendWidth, legendHeight);

                int y = leftMargin;
                foreach (var item in cLegend)
                {
                    using (var brush = new SolidBrush(item.Value))
                    {
                        g2.FillRectangle(brush, leftMargin, y + 3, swatch, swatch);
                        g2.DrawRectangle(Pens.Black, leftMargin, y + 3, swatch, swatch);
                    }
                    g2.DrawString(item.Key, new Font("Arial", 8), Brushes.Black, new PointF(leftMargin + swatch + gap, y + 4));
                    y += itemHeight;
                }
            }

            // Anchor legend at top-right of the current view
            var marker = new GMarkerGoogle(new PointLatLng(gmap.ViewArea.Top, gmap.ViewArea.Right), bmp)
            {
                Offset = new System.Drawing.Point(-legendWidth - leftMargin, leftMargin)
            };
            legendOverlay.Markers.Add(marker);
        }
    }
}