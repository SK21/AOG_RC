using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Timers;
using System.Windows.Forms;

namespace RateController.Classes
{
    public class MapManager
    {
        private double AppliedOverlayCellSize = 0.1;
        private int AppliedOverlayProductID = 0;
        private System.Timers.Timer AppliedOverlayTimer;
        private RateType AppliedOverlayType = RateType.Applied;
        private bool cEditMode;
        private Dictionary<string, Color> cLegend;
        private PointLatLng cTractorPosition;
        private GMapOverlay currentAppliedOverlay;
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

            AppliedOverlayTimer = new System.Timers.Timer(60);
            AppliedOverlayTimer.Elapsed += AppliedOverlayTimer_Elapsed;
            AppliedOverlayTimer.Enabled = false;

            Props.JobChanged += Props_JobChanged;
            Props.RateDataSettingsChanged += Props_MapShowRatesChanged;
            LoadMap();
        }

        public event EventHandler MapChanged;

        public bool EditMode
        {
            get { return cEditMode; }
            set
            {
                cEditMode = value;
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

                if (mapZones.Count > 0) CenterMapToZone(mapZones[0]);

                foreach (var mapZone in mapZones)
                {
                    zoneOverlay = AddPolygons(zoneOverlay, mapZone.ToGMapPolygons());
                }

                gmap.Refresh();
                gmap.Zoom = 16;
                Result = true;
                MapChanged?.Invoke(this, EventArgs.Empty);
                ZoomToFit();
                UpdateRateDataDisplay();
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

        public void SetTractorPosition(PointLatLng NewLocation, double[] AppliedRates, double[] TargetRates, bool FromMouseClick = false)
        {
            if (FromMouseClick || (!cEditMode && !FromMouseClick))
            {
                cTractorPosition = NewLocation;
                tractorMarker.Position = NewLocation; // Update the marker position
                gmap.Refresh(); // Refresh the map to show the updated marker
                UpdateTargetRates();
                mf.Tls.RateCollector.RecordReading(NewLocation.Lat, NewLocation.Lng, AppliedRates, TargetRates);
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
                currentAppliedOverlay = creator.CreateOverlay(readings, out legend, Props.RateDisplayResolution, Props.RateDisplayType, Props.RateDisplayProduct);
                AddOverlay(currentAppliedOverlay);
                gmap.Refresh();

                double RefreshIntervalSeconds = Props.RateDisplayRefresh;
                if (RefreshIntervalSeconds > 29 && RefreshIntervalSeconds < 1800)
                {
                    AppliedOverlayTimer.Interval = RefreshIntervalSeconds * 1000;
                    AppliedOverlayTimer.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapManger/ShowAppliedLayer: " + ex.Message);
            }
            return legend;
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

        public void UpdateRateDataDisplay()
        {
            if (Props.MapShowRates)
            {
                cLegend = ShowAppliedLayer();
                Debug.Print("MapManager.UpdateRateDataDisplay: Rates displayed.");
            }
            else
            {
                RemoveOverlay(currentAppliedOverlay);
                gmap.Refresh();
                AppliedOverlayTimer.Enabled = false;
                Debug.Print("MapManager.UpdateRateDataDisplay: Rates hidden.");
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
            if (zoneOverlay.Polygons.Count > 0)
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
                RectLatLng boundingBox = new RectLatLng(maxLat, minLng, maxLng - minLng, maxLat - minLat);
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

        private void AppliedOverlayTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Get updated data and re-create overlay; use the UI thread for map updates.
            List<RateReading> readings = mf.Tls.RateCollector.GetReadings().ToList();
            AppliedLayerCreator creator = new AppliedLayerCreator();
            Dictionary<string, Color> legend;
            GMapOverlay updatedOverlay = creator.CreateOverlay(readings, out legend, AppliedOverlayCellSize, AppliedOverlayType, AppliedOverlayProductID);

            // Since this event occurs on a secondary thread, perform UI updates on the main thread.
            gmap.Invoke((MethodInvoker)delegate
            {
                if (currentAppliedOverlay != null)
                {
                    RemoveOverlay(currentAppliedOverlay);
                    gmap.Refresh();
                }
                currentAppliedOverlay = updatedOverlay;
                AddOverlay(currentAppliedOverlay);
                gmap.Refresh();
            });
        }

        private Coordinate CalculateCentroid(Polygon polygon)
        {
            double xSum = 0;
            double ySum = 0;
            int numPoints = polygon.Coordinates.Length;

            foreach (var coord in polygon.Coordinates)
            {
                xSum += coord.X; // Longitude
                ySum += coord.Y; // Latitude
            }

            // Calculate the average to find the centroid
            return new Coordinate(xSum / numPoints, ySum / numPoints);
        }

        private void CenterMapToZone(MapZone zone)
        {
            var centroid = CalculateCentroid(zone.Geometry); // Assuming Geometry is a Polygon
            gmap.Position = new PointLatLng(centroid.Y, centroid.X); // Set the map position to the centroid
            Props.SetProp("LastMapLat", centroid.Y.ToString());
            Props.SetProp("LastMapLng", centroid.X.ToString());
        }

        private void Gmap_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (EditMode)
                {
                    var point = gmap.FromLocalToLatLng(e.X, e.Y);
                    currentZoneVertices.Add(point);
                    tempMarkerOverlay.Markers.Add(new GMarkerGoogle(point, GMarkerGoogleType.red_small));
                }
                else
                {
                    PointLatLng Location = gmap.FromLocalToLatLng(e.X, e.Y);
                    SetTractorPosition(Location, null, null, true);
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

            tractorMarker = new GMarkerGoogle(new PointLatLng(0, 0), GMarkerGoogleType.green); // Initialize with a default position
            gpsMarkerOverlay.Markers.Add(tractorMarker); // Add the tractor marker to the overlay

            AddOverlay(gpsMarkerOverlay);
            AddOverlay(tempMarkerOverlay);
            AddOverlay(zoneOverlay);
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
            UpdateRateDataDisplay();
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
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapManager/RemoveLayer: " + ex.Message);
            }
        }
    }
}