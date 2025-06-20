﻿using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RateController.Classes
{
    public class MapManager
    {
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
        private int[] cZoneRates = new int[4];
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
        const int MapRefreshSeconds = 2;

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

            // Get the overall geographic bounding box.
            RectLatLng overallRect = GetOverallRectLatLng();
            if (overallRect == RectLatLng.Empty)
                return grid;

            // Convert the desired cell area from acres to square meters.
            // 1 acre ≈ 4046.86 m².
            double squareMeters = acres * 4046.86;
            // Calculate the cell's side length in meters.
            double cellSideMeters = Math.Sqrt(squareMeters);

            // Convert the cell side length into degrees latitude.
            // Approximately: 1 degree latitude ≈ 111,000 meters.
            double cellHeight = cellSideMeters / 111000.0;

            // For longitude, the conversion is dependent on latitude.
            // We use the average latitude from the overall rectangle.
            double avgLat = (overallRect.Top + overallRect.Bottom) / 2.0;
            double cellWidth = cellSideMeters / (111000.0 * Math.Cos(avgLat * Math.PI / 180.0));

            // Determine the total width and height of the overall rectangle in degrees.
            double totalWidth = overallRect.Right - overallRect.Left;
            double totalHeight = overallRect.Top - overallRect.Bottom;

            // Determine how many full-width/height cells fit inside.
            int numFullCols = (int)Math.Floor(totalWidth / cellWidth);
            int numFullRows = (int)Math.Floor(totalHeight / cellHeight);

            // The remaining width/height (if any) along the borders.
            double remainderWidth = totalWidth - (numFullCols * cellWidth);
            double remainderHeight = totalHeight - (numFullRows * cellHeight);

            // Total columns and rows (adding an extra column/row if there is a remainder).
            int totalCols = numFullCols + (remainderWidth > 0.000001 ? 1 : 0);
            int totalRows = numFullRows + (remainderHeight > 0.000001 ? 1 : 0);

            // Loop to generate each grid cell.
            for (int row = 0; row < totalRows; row++)
            {
                // For rows, if this is the last row and there is a remainder, use the remainder as the cell height.
                double currentCellHeight = (row == totalRows - 1 && remainderHeight > 0.000001)
                                            ? remainderHeight
                                            : cellHeight;
                // Calculate the top coordinate for the row.
                // Note: overallRect.Top is the maximum latitude; each row descends by the full cellHeight.
                double cellTop = overallRect.Top - row * cellHeight;

                for (int col = 0; col < totalCols; col++)
                {
                    // For columns, if this is the last column and there is a remainder, reduce the cell's width.
                    double currentCellWidth = (col == totalCols - 1 && remainderWidth > 0.000001)
                                              ? remainderWidth
                                              : cellWidth;
                    // Calculate the left coordinate.
                    double cellLeft = overallRect.Left + col * cellWidth;

                    // Create a cell with the computed boundaries.
                    // RectLatLng expects (Top, Left, Width, Height), where Width is the difference in longitude and Height the difference in latitude.
                    RectLatLng cell = new RectLatLng(cellTop, cellLeft, currentCellWidth, currentCellHeight);
                    grid.Add(cell);
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

        public int GetRate(int RateID)
        {
            int Result = 0;
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

        public bool SaveMap(bool UpdateCache = true)
        {
            bool Result = false;
            var shapefileHelper = new ShapefileHelper(mf);
            shapefileHelper.SaveMapZones(Props.CurrentMapPath, mapZones);
            if (UpdateCache) AddToCache();
            Result = true;
            return Result;
        }

        public void SetTractorPosition(PointLatLng NewLocation, double[] AppliedRates, double[] TargetRates)
        {
            if (!cMouseSetTractorPosition)
            {
                cTractorPosition = NewLocation;
                tractorMarker.Position = NewLocation; // Update the marker position
                gmap.Refresh(); // Refresh the map to show the updated marker
                UpdateTargetRates();
                if (mf.Products.ProductsAreOn()) mf.Tls.RateCollector.RecordReading(NewLocation.Lat, NewLocation.Lng, AppliedRates, TargetRates);
                MapChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public Dictionary<string, Color> ShowAppliedLayer()
        {
            Dictionary<string, Color> legend = new Dictionary<string, Color>();
            try
            {
                // display layer
                var readings = mf.Tls.RateCollector.GetReadings().ToList();
                AppliedLayerCreator creator = new AppliedLayerCreator();

                creator.UpdateRatesOverlay(ref AppliedOverlay, readings, RateGrid, out legend, Props.RateDisplayType, Props.RateDisplayProduct);
                gmap.Refresh();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapManger/ShowAppliedLayer: " + ex.Message);
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
                mf.Tls.ShowMessage("MapManager.UpdateRates: " + ex.Message, "Error", 20000, true);
                throw;
            }
        }

        public bool UpdateZone(string name, int Rt0, int Rt1, int Rt2, int Rt3, Color zoneColor)
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

                var mapZone = new MapZone(name, polygon, new Dictionary<string, int>
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
                        zone.Rates = new Dictionary<string, int>
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
                if (NewOverlay.Id == "mapzones") RateGrid = CreateGrid(Props.RateDisplayResolution);
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

            tractorMarker = new GMarkerGoogle(new PointLatLng(0, 0), GMarkerGoogleType.green); // Initialize with a default position
            gpsMarkerOverlay.Markers.Add(tractorMarker); // Add the tractor marker to the overlay

            AddOverlay(gpsMarkerOverlay);
            AddOverlay(tempMarkerOverlay);
            AddOverlay(zoneOverlay);
            AddOverlay(AppliedOverlay);
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