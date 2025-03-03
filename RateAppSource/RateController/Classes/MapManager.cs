using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace RateController.Classes
{
    public class MapManager
    {
        private string CachePath;
        private bool cEditMode;
        private string cMapName = "Unnamed Map";
        private string cRootPath;
        private bool cShowTiles = true;
        private PointLatLng cTractorPosition;
        private List<PointLatLng> currentZoneVertices;
        private Color cZoneColor;
        private double cZoneHectares;
        private string cZoneName = "Unnamed Zone";
        private int[] cZoneRates = new int[4];
        private GMapControl gmap;
        private GMapOverlay gpsMarkerOverlay;
        private bool isDragging = false;
        private string LastFile;
        private System.Drawing.Point lastMousePosition;
        private List<MapZone> mapZones;
        private FormStart mf;
        private GMapOverlay tempMarkerOverlay;
        private GMarkerGoogle tractorMarker;
        private GMapOverlay zoneOverlay;

        private GMapOverlay appliedRateOverlay;
        private string selectedAppliedRate = "ProductA"; // Default selection
        private const double AcresToSquareMeters = 4046.86; // Conversion factor
        private double implementWidthMeters = 30.48; // Default width in meters (100 feet)
        private PointLatLng? lastTractorPosition = null;
        private double accumulatedArea = 0; // Track area covered
        private List<GMapPolygon> appliedRatePolygons = new List<GMapPolygon>(); // Persisted applied rate areas
        private ShapefileHelper shapefileHelper;
        private string appliedRatesShapefilePath;
        private double cMinAppliedArea=0.1; // 1/10 acre
        private string cMapPath;

        public MapManager(FormStart main)
        {
            mf = main;
            cRootPath = GetFolder(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RateMap");
            CachePath = GetFolder(cRootPath, "MapCache");
            shapefileHelper = new ShapefileHelper(mf);

            if (bool.TryParse(mf.Tls.LoadProperty("ShowTiles"), out bool st)) cShowTiles = st;
            InitializeMap();
            InitializeMapZones();
            gmap.MouseDown += Gmap_MouseDown;
            gmap.MouseMove += Gmap_MouseMove;
            gmap.MouseUp += Gmap_MouseUp;
            gmap.OnMapZoomChanged += Gmap_OnMapZoomChanged;
        }

        public event EventHandler MapChanged;
        public string MapPath { get { return cMapPath; } }

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

        public string MapName
        {
            get { return cMapName; }
            set
            {
                if (value.Length > 0)
                {
                    if (value.Length > 12) value = value.Substring(0, 12);
                    cMapName = value;
                }
                else
                {
                    cMapName = "Unnamed Map";
                }
            }
        }

        public string RootPath
        { get { return cRootPath; } }

        public bool ShowTiles
        {
            get { return cShowTiles; }
            set
            {
                cShowTiles = value;
                if (value)
                {
                    gmap.MapProvider = GMapProviders.BingSatelliteMap;
                }
                else
                {
                    gmap.MapProvider = GMapProviders.EmptyProvider;
                }
                gmap.Refresh();
                mf.Tls.SaveProperty("ShowTiles", cShowTiles.ToString());
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
                            gmap.Overlays.Remove(zoneOverlay);
                            gmap.Overlays.Add(zoneOverlay);

                            gmap.Refresh();
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

        public bool LoadLastMap()
        {
            LastFile = mf.Tls.LoadProperty("LastMapFile");
            return LoadMap(LastFile);
        }

        public bool LoadMap(string FilePath)
        {
            bool Result = false;
            if (FileNameValidator.IsValidFolderName(FilePath))
            {
                if (File.Exists(FilePath))
                {
                    MapName = Path.GetFileNameWithoutExtension(FilePath);
                    cMapPath = Path.GetDirectoryName(FilePath);
                    InitializeMapZones();

                    mapZones = shapefileHelper.CreateZoneList(FilePath);
                    if (mapZones.Count > 0)
                    {
                        CenterMapToZone(mapZones[0]);
                        foreach (var mapZone in mapZones)
                        {
                            zoneOverlay = AddPolygons(zoneOverlay, mapZone.ToGMapPolygons());
                        }
                    }

                    gmap.Refresh();
                    mf.Tls.SaveProperty("LastMapFile", FilePath);
                    ZoomToFit();
                    MapChanged?.Invoke(this, EventArgs.Empty);
                    Result = true;
                }
            }
            return Result;
        }

        public bool NewMap(string name)
        {
            bool Result = false;
            try
            {
                bool ValidName = false;
                string FolderPath = Path.Combine(cRootPath, name);
                if (FileNameValidator.IsValidFolderName(name) && FileNameValidator.IsValidFileName(name))
                {
                    if (Directory.Exists(FolderPath))
                    {
                        var Hlp = new frmMsgBox(mf, "File exists, overwrite?", "File Exists", true);
                        Hlp.TopMost = true;
                        Hlp.ShowDialog();
                        bool Choice = Hlp.Result;
                        Hlp.Close();
                        if (Choice)
                        {
                            Directory.Delete(FolderPath, true);
                            ValidName = true;
                        }
                        else
                        {
                            mf.Tls.ShowMessage("File not created!", "File Error", 10000);
                        }
                    }
                    else
                    {
                        ValidName = true;
                    }
                }
                if (ValidName)
                {
                    if (!Directory.Exists(FolderPath))
                    {
                        Directory.CreateDirectory(FolderPath);
                        mf.Tls.ShowMessage("New file created.", "File Created", 10000);
                    }
                    MapName = name;
                    cMapPath = FolderPath;
                    InitializeMapZones();
                    gmap.Refresh();
                    Result = true;
                }
            }
            catch (Exception ex)
            {
                mf.Tls.ShowMessage("MapManager.NewMap: " + ex.Message, "Error", 20000);
            }
            return Result;
        }


        public bool SaveMap(bool UpdateCache = true)
        {
            bool Result = false;
            string FilePath = Path.Combine(cMapPath, MapName);
            shapefileHelper.SaveMapZones(FilePath, mapZones);
            if (UpdateCache) AddToCache();
            Result = true;
            return Result;
        }

        public void SetTractorPosition(PointLatLng NewLocation, int[] Rates = null, bool FromMouseClick = false)
        {
            if (FromMouseClick || (!cEditMode && !FromMouseClick))
            {
                cTractorPosition = NewLocation;
                tractorMarker.Position = NewLocation; // Update the marker position
                gmap.Refresh(); // Refresh the map to show the updated marker
                UpdateTargetRates();
                UpdateAppliedRates(NewLocation, Rates);
                MapChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void UpdateAppliedRates(PointLatLng NewLocation, int[] Rates)
        {
            if (Rates != null)
            {
                if (lastTractorPosition.HasValue)
                {
                    double distanceTraveled = GetDistance(lastTractorPosition.Value, NewLocation); // Meters
                    double areaCovered = distanceTraveled * implementWidthMeters; // Square meters
                    accumulatedArea += areaCovered;

                    if (accumulatedArea >= (AcresToSquareMeters * cMinAppliedArea)) // Check if minimum area is reached in acres
                    {
                        List<PointLatLng> areaCoveredPoints = GetCoveredArea(lastTractorPosition.Value, NewLocation);
                        for (int i = 0; i < Rates.Length; i++)
                        {
                            if (Rates[i] > 0)
                            {
                                double rateValue = Rates[i];
                                Color rateColor = GetRateColor(rateValue);
                                GMapPolygon areaPolygon = new GMapPolygon(areaCoveredPoints, "AppliedArea")
                                {
                                    Stroke = new Pen(Color.Black, 1),
                                    Fill = new SolidBrush(Color.FromArgb(100, rateColor))
                                };
                                appliedRatePolygons.Add(areaPolygon);
                                appliedRateOverlay.Polygons.Add(areaPolygon);
                            }
                        }
                        shapefileHelper.SaveAppliedRateAreas(appliedRatesShapefilePath, appliedRatePolygons);
                        accumulatedArea = 0; // Reset area tracker
                    }
                }
                lastTractorPosition = NewLocation;
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
                    cZoneName = "N/A (Outside Zones)";
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
                        SaveMap( false);
                        LoadLastMap();
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
            mf.Tls.SaveProperty("LastMapLat", centroid.Y.ToString());
            mf.Tls.SaveProperty("LastMapLng", centroid.X.ToString());
        }

        private string GetFolder(string StartFolder, string SubFolder, bool CreateNew = true)
        {
            string NewPath = Path.Combine(StartFolder, SubFolder);
            if (CreateNew && !Directory.Exists(NewPath)) Directory.CreateDirectory(NewPath);
            return NewPath;
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
                    SetTractorPosition(Location,null, true);
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
                CacheLocation = CachePath
            };

            double Lat = 200;
            double Lng = 200;
            if (double.TryParse(mf.Tls.LoadProperty("LastMapLat"), out double latpos)) Lat = latpos;
            if (double.TryParse(mf.Tls.LoadProperty("LastMapLng"), out double lngpos)) Lng = lngpos;

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

            if (cShowTiles)
            {
                gmap.MapProvider = GMapProviders.BingSatelliteMap;
            }
            else
            {
                gmap.MapProvider = GMapProviders.EmptyProvider;
            }

            gmap.MouseClick += Gmap_MouseClick;
        }

        private void InitializeMapZones()
        {
            mapZones = new List<MapZone>();
            currentZoneVertices = new List<PointLatLng>();

            gmap.Overlays.Clear();

            zoneOverlay = new GMapOverlay("mapzones");
            tempMarkerOverlay = new GMapOverlay("tempMarkers");
            appliedRateOverlay = new GMapOverlay("appliedRates");
            gpsMarkerOverlay = new GMapOverlay("gpsMarkers");
            tractorMarker = new GMarkerGoogle(new PointLatLng(0, 0), GMarkerGoogleType.green); // Initialize with a default position
            gpsMarkerOverlay.Markers.Add(tractorMarker); // Add the tractor marker to the overlay

            gmap.Overlays.Add(appliedRateOverlay);
            gmap.Overlays.Add(zoneOverlay);
            gmap.Overlays.Add(gpsMarkerOverlay);
            gmap.Overlays.Add(tempMarkerOverlay);
        }

        public void UpdateAppliedRateLayer(string rateName)
        {
            selectedAppliedRate = rateName;
            appliedRateOverlay.Markers.Clear();
            appliedRateOverlay.Polygons.Clear();

            foreach (var polygon in appliedRatePolygons)
            {
                appliedRateOverlay.Polygons.Add(polygon);
            }
            gmap.Refresh();
        }

        private List<PointLatLng> GetCoveredArea(PointLatLng start, PointLatLng end)
        {
            double halfWidth = (implementWidthMeters / 2) / 111320.0; // Convert meters to degrees
            return new List<PointLatLng>
        {
            new PointLatLng(start.Lat + halfWidth, start.Lng),
            new PointLatLng(end.Lat + halfWidth, end.Lng),
            new PointLatLng(end.Lat - halfWidth, end.Lng),
            new PointLatLng(start.Lat - halfWidth, start.Lng),
            new PointLatLng(start.Lat + halfWidth, start.Lng)
        };
        }

        private void SaveAppliedRateData()
        {
            shapefileHelper.SaveAppliedRateAreas(appliedRatesShapefilePath, appliedRatePolygons);
        }

        private void LoadAppliedRateData()
        {
            appliedRatePolygons = shapefileHelper.LoadAppliedRateAreas(appliedRatesShapefilePath);
        }

        private double GetDistance(PointLatLng p1, PointLatLng p2)
        {
            double R = 6371000; // Earth radius in meters
            double dLat = (p2.Lat - p1.Lat) * Math.PI / 180;
            double dLng = (p2.Lng - p1.Lng) * Math.PI / 180;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(p1.Lat * Math.PI / 180) * Math.Cos(p2.Lat * Math.PI / 180) *
                       Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c; // Distance in meters
        }

        private Color GetRateColor(double rateValue)
        {
            int intensity = (int)(Math.Min(255, rateValue * 10)); // Scale value to 0-255
            return Color.FromArgb(255, intensity, 255 - intensity, 0); // Gradient from green to red
        }
    }
}