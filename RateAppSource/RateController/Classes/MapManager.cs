using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RateController.Classes
{
    public class MapManager
    {
        private const int MapRefreshSeconds = 2;
        private GMapOverlay AppliedOverlay;
        private System.Windows.Forms.Timer AppliedOverlayTimer;
        private bool cEditPolygons = false;
        private Dictionary<string, Color> cLegend;
        private bool cMouseSetTractorPosition = false;
        private PointLatLng cTractorPosition;
        private List<PointLatLng> currentZoneVertices;
        private Color cZoneColor;
        private double cZoneHectares;
        private string cZoneName = "Unnamed Zone";
        private double[] cZoneRates = new double[4];
        private GMapControl gmap;
        private GMapOverlay gpsMarkerOverlay;
        private bool isDragging = false;
        private System.Drawing.Point lastMousePosition;
        private List<MapZone> mapZones;
        private FormStart mf;
        private List<RectLatLng> RateGrid;
        private GMapOverlay tempMarkerOverlay;
        private GMarkerGoogle tractorMarker;
        private GMapOverlay zoneOverlay;
        private double cTravelHeading; // in degrees, 0 = North, 90 = East, etc.
        private GMapOverlay legendOverlay;
        private const int LEGEND_MARGIN = 10;
        private const int LEGEND_WIDTH = 120;
        private const int LEGEND_ITEM_HEIGHT = 25;

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
            Props.ResolutionChanged += (s, e) => RefreshZoneOverlay();
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

        public PointLatLng GetTractorPosition
        { get { return cTractorPosition; } }

        public GMapControl gmapObject
        { get { return gmap; } }

        public Dictionary<string, Color> Legend
        { get { return cLegend; } }

        public bool MouseSetTractorPosition
        {
            get { return cMouseSetTractorPosition; }
            set
            {
                cMouseSetTractorPosition = value;
            }
        }

        public bool ShowTiles
        {
            get { return Props.MapShowTiles; }
            set
            {
                Props.MapShowTiles = value;
                if (value)
                {
                    gmap.MapProvider = GMapProviders.BingSatelliteMap;
                }
                else
                {
                    gmap.MapProvider = GMapProviders.EmptyProvider;
                }
                gmap.Refresh();
            }
        }

        public Color ZoneColor
        { get { return cZoneColor; } }

        public double ZoneHectares
        { get { return cZoneHectares; } }

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

        public List<RectLatLng> CreateGrid(double acres)
        {
            List<RectLatLng> grid = new List<RectLatLng>();

            RectLatLng overallRect = GetOverallRectLatLng();
            if (overallRect == RectLatLng.Empty)
                return grid;

            // Get implement width in meters
            double implementWidthMeters = 0.0;
            if (mf.Sections != null)
            {
                try
                {
                    implementWidthMeters = mf.Sections.TotalWidth(false);
                }
                catch
                {
                    implementWidthMeters = 24.0;
                }
            }
            if (implementWidthMeters <= 0) implementWidthMeters = 24.0;

            // Convert acres to square meters
            double desiredAreaM2 = acres * 4046.86;
            
            // Calculate required cell length in meters to achieve desired area
            double cellLengthMeters = desiredAreaM2 / implementWidthMeters;

            // Convert to degrees using approximate conversions
            const double metersPerDegreeLat = 111320.0;
            
            // Calculate at tractor's position for accuracy
            double metersPerDegreeLng = metersPerDegreeLat * Math.Cos(cTractorPosition.Lat * Math.PI / 180.0);

            // Convert implement width and cell length to degrees
            double widthDegrees = implementWidthMeters / metersPerDegreeLng;
            double lengthDegrees = cellLengthMeters / metersPerDegreeLat;

            // Convert heading to radians for rotation calculations
            double headingRad = cTravelHeading * Math.PI / 180.0;
            
            // Calculate the four corners of the center cell relative to tractor position
            // First calculate unrotated rectangle corners (implement width perpendicular to travel)
            double halfWidth = widthDegrees / 2.0;
            double halfLength = lengthDegrees / 2.0;

            // Calculate rotated corner points
            PointLatLng[] corners = new PointLatLng[4];
            
            // Calculate sin and cos once
            double sinHead = Math.Sin(headingRad);
            double cosHead = Math.Cos(headingRad);

            // Calculate rotated offsets for each corner
            double[] dLat = new double[4];
            double[] dLng = new double[4];

            // Front left
            dLat[0] = halfLength * cosHead - halfWidth * sinHead;
            dLng[0] = halfLength * sinHead + halfWidth * cosHead;

            // Front right
            dLat[1] = halfLength * cosHead + halfWidth * sinHead;
            dLng[1] = halfLength * sinHead - halfWidth * cosHead;

            // Back right
            dLat[2] = -halfLength * cosHead + halfWidth * sinHead;
            dLng[2] = -halfLength * sinHead - halfWidth * cosHead;

            // Back left
            dLat[3] = -halfLength * cosHead - halfWidth * sinHead;
            dLng[3] = -halfLength * sinHead + halfWidth * cosHead;

            // Create the center cell
            List<PointLatLng> points = new List<PointLatLng>();
            for (int i = 0; i < 4; i++)
            {
                points.Add(new PointLatLng(
                    cTractorPosition.Lat + dLat[i],
                    cTractorPosition.Lng + dLng[i]
                ));
            }

            // Create the cell using min/max bounds
            double cellTop = points.Max(p => p.Lat);
            double cellBottom = points.Min(p => p.Lat);
            double cellLeft = points.Min(p => p.Lng);
            double cellRight = points.Max(p => p.Lng);

            grid.Add(new RectLatLng(
                cellTop,
                cellLeft,
                cellRight - cellLeft,
                cellTop - cellBottom
            ));

            // Calculate additional cells needed in travel direction and perpendicular
            int cellsNeeded = (int)Math.Ceiling(Math.Max(
                overallRect.HeightLat / lengthDegrees,
                overallRect.WidthLng / widthDegrees
            ));

            // Add surrounding cells in a grid pattern
            for (int i = -cellsNeeded; i <= cellsNeeded; i++)
            {
                for (int j = -cellsNeeded; j <= cellsNeeded; j++)
                {
                    // Skip center cell as it's already added
                    if (i == 0 && j == 0) continue;

                    // Calculate offset for this cell
                    double offsetLat = i * lengthDegrees * cosHead - j * widthDegrees * sinHead;
                    double offsetLng = i * lengthDegrees * sinHead + j * widthDegrees * cosHead;

                    // Create new cell points
                    List<PointLatLng> cellPoints = new List<PointLatLng>();
                    for (int k = 0; k < 4; k++)
                    {
                        cellPoints.Add(new PointLatLng(
                            cTractorPosition.Lat + dLat[k] + offsetLat,
                            cTractorPosition.Lng + dLng[k] + offsetLng
                        ));
                    }

                    // Only add cell if it intersects with the overall bounds
                    double newCellTop = cellPoints.Max(p => p.Lat);
                    double newCellBottom = cellPoints.Min(p => p.Lat);
                    double newCellLeft = cellPoints.Min(p => p.Lng);
                    double newCellRight = cellPoints.Max(p => p.Lng);

                    if (newCellRight >= overallRect.Left && newCellLeft <= overallRect.Right &&
                        newCellTop >= overallRect.Bottom && newCellBottom <= overallRect.Top)
                    {
                        grid.Add(new RectLatLng(
                            newCellTop,
                            newCellLeft,
                            newCellRight - newCellLeft,
                            newCellTop - newCellBottom
                        ));
                    }
                }
            }

            return grid;
        }

        public bool DeleteZone(string name)
        {
            bool Result = false;
            // Find the zone to delete
            var zoneToRemove = mapZones.FirstOrDefault(zone => zone.Name == name);

            if (zoneToRemove != null)
            {
                // Convert to polygons
                List<GMapPolygon> polygonsToRemove = zoneToRemove.ToGMapPolygons();
                foreach (var polygonToRemove in polygonsToRemove)
                {
                    if (polygonToRemove != null)
                    {
                        // Find the actual polygon object in the overlay
                        var polygonInOverlay = zoneOverlay.Polygons
                            .FirstOrDefault(polygon => polygon.Points.SequenceEqual(polygonToRemove.Points));

                        if (polygonInOverlay != null)
                        {
                            // Remove the polygon from the overlay
                            zoneOverlay.Polygons.Remove(polygonInOverlay);

                            // Remove the zone from the list
                            mapZones.Remove(zoneToRemove);

                            // Re-bind the overlay to the map
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
            RectLatLng Result = new RectLatLng();
            Result = RectLatLng.Empty;
            if (zoneOverlay != null && zoneOverlay.Polygons.Count > 0)
            {
                // Initialize min and max values
                double minLat = double.MaxValue;
                double maxLat = double.MinValue;
                double minLng = double.MaxValue;
                double maxLng = double.MinValue;

                // Iterate through all polygons to find the overall bounding box
                foreach (var polygon in zoneOverlay.Polygons)
                {
                    if (polygon.Points.Count == 0) continue;

                    // Update min and max latitude and longitude
                    minLat = Math.Min(minLat, polygon.Points.Min(p => p.Lat));
                    maxLat = Math.Max(maxLat, polygon.Points.Max(p => p.Lat));
                    minLng = Math.Min(minLng, polygon.Points.Min(p => p.Lng));
                    maxLng = Math.Max(maxLng, polygon.Points.Max(p => p.Lng));
                }

                // Create the bounding box from the min and max values
                Result = new RectLatLng(maxLat, minLng, maxLng - minLng, maxLat - minLat);
            }
            return Result;
        }

        public double GetRate(int RateID)
        {
            double Result = 0.0;
            if (RateID >= 0 && RateID < 4)
            {
                Result = cZoneRates[RateID];
            }
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

        public void RefreshZoneOverlay()
        {
            // Recreate the grid with the new resolution
            RateGrid = CreateGrid(Props.RateDisplayResolution);

            // Optionally, clear and re-add polygons if needed
            RemoveOverlay(zoneOverlay);
            AddOverlay(zoneOverlay);

            gmap.Refresh();
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
                // Calculate heading if we have a previous position
                if (!cTractorPosition.IsEmpty)
                {
                    // Calculate heading from previous to new position
                    double deltaLng = NewLocation.Lng - cTractorPosition.Lng;
                    double deltaLat = NewLocation.Lat - cTractorPosition.Lat;
                    
                    // Only update heading if we've moved enough to get a reliable direction
                    double movementThreshold = 0.0000001; // adjust as needed
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

        private void UpdateLegend()
        {
            if (legendOverlay == null || cLegend == null) return;

            legendOverlay.Markers.Clear();

            // Create legend background
            int legendHeight = (cLegend.Count * LEGEND_ITEM_HEIGHT) + (LEGEND_MARGIN * 2);
            var legendBitmap = new Bitmap(LEGEND_WIDTH, legendHeight);
            using (var g = Graphics.FromImage(legendBitmap))
            {
                // Draw background
                g.FillRectangle(new SolidBrush(Color.FromArgb(200, Color.White)), 
                    0, 0, LEGEND_WIDTH, legendHeight);

                int y = LEGEND_MARGIN;
                foreach (var item in cLegend)
                {
                    // Draw color box
                    g.FillRectangle(new SolidBrush(item.Value), 
                        LEGEND_MARGIN, y, 20, 20);
                    g.DrawRectangle(Pens.Black, 
                        LEGEND_MARGIN, y, 20, 20);

                    // Draw text
                    g.DrawString(item.Key, 
                        new Font("Arial", 8), 
                        Brushes.Black, 
                        new PointF(LEGEND_MARGIN + 25, y + 2));

                    y += LEGEND_ITEM_HEIGHT;
                }
            }

            // Create marker for legend (positioned at top-right of map)
            var legendMarker = new GMarkerGoogle(
                new PointLatLng(gmap.ViewArea.Top, gmap.ViewArea.Right),
                legendBitmap);
            legendMarker.Offset = new Point(-LEGEND_WIDTH - LEGEND_MARGIN, LEGEND_MARGIN);
            legendOverlay.Markers.Add(legendMarker);
        }

        public Dictionary<string, Color> ShowAppliedLayer()
        {
            Dictionary<string, Color> legend = new Dictionary<string, Color>();
            try
            {
                var readings = mf.Tls.RateCollector.GetReadings().ToList();
                
                if (readings == null || readings.Count == 0)
                {
                    Props.WriteErrorLog("MapManager: No rate readings available");
                    return legend;
                }

                // Get implement width
                double implementWidth = 24.0; // default
                if (mf.Sections != null)
                {
                    try
                    {
                        implementWidth = mf.Sections.TotalWidth(false);
                    }
                    catch (Exception ex)
                    {
                        Props.WriteErrorLog($"MapManager: Error getting implement width - {ex.Message}");
                    }
                }

                // Update coverage
                AppliedLayerCreator creator = new AppliedLayerCreator();
                bool success = creator.UpdateRatesOverlay(
                    ref AppliedOverlay, 
                    readings,
                    cTractorPosition,
                    cTravelHeading,
                    implementWidth,
                    out legend,
                    Props.RateDisplayType, 
                    Props.RateDisplayProduct);

                if (!success)
                {
                    Props.WriteErrorLog("MapManager: Failed to update rates overlay");
                }

                // Store legend and update display
                cLegend = new Dictionary<string, Color>(legend);
                UpdateLegend();

                // Make sure overlay is added and visible
                if (!gmap.Overlays.Contains(AppliedOverlay))
                {
                    gmap.Overlays.Add(AppliedOverlay);
                }
                
                gmap.Refresh();
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
                RemoveOverlay(AppliedOverlay);
                gmap.Refresh();
                AppliedOverlayTimer.Enabled = false;
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
            // creating a zone
            bool Result = false;
            if (currentZoneVertices.Count > 2)
            {
                // creating a new zone
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
                // editing the current zone
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
                // Check if an overlay with the same Id already exists
                bool overlayExists = gmap.Overlays.Any(o => o.Id == NewOverlay.Id);

                if (!overlayExists)
                {
                    gmap.Overlays.Add(NewOverlay);
                }
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
            //for (int zoom = (int)gmap.MinZoom; zoom <= (int)gmap.MaxZoom; zoom++)
            {
                var mapProvider = gmap.MapProvider;
                var projection = mapProvider.Projection;

                int tileSize = (int)projection.TileSize.Width;

                // Convert latitude/longitude to pixel coordinates
                GPoint topLeftPixel = projection.FromLatLngToPixel(area.Top, area.Left, zoom);
                GPoint bottomRightPixel = projection.FromLatLngToPixel(area.Bottom, area.Right, zoom);

                // Convert pixel coordinates to tile indices
                GPoint topLeftTile = new GPoint(topLeftPixel.X / tileSize, topLeftPixel.Y / tileSize);
                GPoint bottomRightTile = new GPoint(bottomRightPixel.X / tileSize, bottomRightPixel.Y / tileSize);

                for (long x = topLeftTile.X; x <= bottomRightTile.X; x++)
                {
                    for (long y = topLeftTile.Y; y <= bottomRightTile.Y; y++)
                    {
                        GPoint tilePoint = new GPoint(x, y);
                        var tile = mapProvider.GetTileImage(tilePoint, zoom);

                        if (tile != null)
                        {
                            // Tile is cached automatically by GMaps.Instance
                            tile.Dispose();
                        }
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
                    // add vertices to the current zone
                    var point = gmap.FromLocalToLatLng(e.X, e.Y);
                    currentZoneVertices.Add(point);
                    tempMarkerOverlay.Markers.Add(new GMarkerGoogle(point, GMarkerGoogleType.red_small));
                }
                else
                {
                    // set tractor position
                    PointLatLng Location = gmap.FromLocalToLatLng(e.X, e.Y);
                    cTractorPosition = Location;
                    tractorMarker.Position = Location; // Update the marker position
                    gmap.Refresh(); // Refresh the map to show the updated marker
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
                    gmap.Position.Lat - (deltaY * 0.0001), // Adjust sensitivity
                    gmap.Position.Lng + (deltaX * 0.0001)  // Adjust sensitivity
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
            UpdateLegend(); // Update legend position when map is zoomed
            MapChanged?.Invoke(this, EventArgs.Empty);
        }

        private void InitializeMap()
        {
            //GMaps.Instance.Mode = AccessMode.CacheOnly;
            //GMaps.Instance.Mode = AccessMode.ServerOnly;
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
                // invalid position, use default
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

            if (Props.MapShowTiles)
            {
                gmap.MapProvider = GMapProviders.BingSatelliteMap;
            }
            else
            {
                gmap.MapProvider = GMapProviders.EmptyProvider;
            }

            gmap.MouseClick += Gmap_MouseClick;

            zoneOverlay = new GMapOverlay("mapzones");
            gpsMarkerOverlay = new GMapOverlay("gpsMarkers");
            tempMarkerOverlay = new GMapOverlay("tempMarkers");
            AppliedOverlay = new GMapOverlay("AppliedRates");
            legendOverlay = new GMapOverlay("legend"); // Add legend overlay

            tractorMarker = new GMarkerGoogle(new PointLatLng(0, 0), GMarkerGoogleType.green);
            gpsMarkerOverlay.Markers.Add(tractorMarker);

            AddOverlay(gpsMarkerOverlay);
            AddOverlay(tempMarkerOverlay);
            AddOverlay(zoneOverlay);
            AddOverlay(AppliedOverlay);
            AddOverlay(legendOverlay); // Add legend overlay
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
            ShowAppliedRatesOverlay();
        }

        private void RemoveOverlay(GMapOverlay overlay)
        {
            try
            {
                if (overlay != null)
                {
                    // Collect all overlays that match the passed overlay's Id.
                    var overlaysToRemove = gmap.Overlays.Where(o => o.Id == overlay.Id).ToList();

                    foreach (var o in overlaysToRemove)
                    {
                        gmap.Overlays.Remove(o);
                    }

                    if (overlay.Id == "mapzones") RateGrid = CreateGrid(Props.RateDisplayResolution);
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapManager/RemoveLayer: " + ex.Message);
            }
        }
    }
}