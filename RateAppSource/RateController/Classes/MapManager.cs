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
using System.Security.Policy;
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
        private PointLatLng cTractorPosition;
        private List<PointLatLng> currentZoneVertices;
        private Color cZoneColor;
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

        public MapManager(FormStart main)
        {
            mf = main;
            cRootPath = GetFolder(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RateMap");
            CachePath = GetFolder(cRootPath, "MapCache");
            InitializeMap();
            InitializeMapZones();
            gmap.MouseDown += Gmap_MouseDown;
            gmap.MouseMove += Gmap_MouseMove;
            gmap.MouseUp += Gmap_MouseUp;
            gmap.OnMapZoomChanged += Gmap_OnMapZoomChanged;
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

        public Color ZoneColor
        { get { return cZoneColor; } }

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
                // Convert to polygon
                var polygonToRemove = zoneToRemove.ToGMapPolygon();

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
            return Result;
        }

        public int[] GetRates()
        {
            UpdateValuesFromZone();
            return cZoneRates;
        }

        public bool LoadLastMap()
        {
            LastFile = mf.Tls.LoadProperty("LastMapFile");
            return LoadMap(LastFile);
        }

        public bool LoadMap(string path)
        {
            bool Result = false;
            if (FileNameValidator.IsValidFolderName(path))
            {
                if (File.Exists(path))
                {
                    MapName = Path.GetFileNameWithoutExtension(path);
                    var shapefileHelper = new ShapefileHelper();
                    mapZones = shapefileHelper.LoadMapZones(path);

                    zoneOverlay.Polygons.Clear();

                    if (mapZones.Count > 0) CenterMapToZone(mapZones[0]);

                    foreach (var mapZone in mapZones)
                    {
                        zoneOverlay.Polygons.Add(mapZone.ToGMapPolygon());
                    }

                    gmap.Refresh();
                    gmap.Zoom = 16;
                    Result = true;
                    MapChanged?.Invoke(this, EventArgs.Empty);
                    mf.Tls.SaveProperty("LastMapFile", path);
                }
            }
            return Result;
        }

        public bool NewMap()
        {
            bool Result = false;
            gmap.Overlays.Clear();
            zoneOverlay = new GMapOverlay("zones");
            gpsMarkerOverlay = new GMapOverlay();
            tempMarkerOverlay = new GMapOverlay();
            gmap.Overlays.Add(zoneOverlay);
            gmap.Overlays.Add(gpsMarkerOverlay);
            gmap.Overlays.Add(tempMarkerOverlay);
            currentZoneVertices = new List<PointLatLng>();
            mapZones.Clear();
            gmap.Refresh();
            Result = true;
            return Result;
        }

        public bool SaveMap(string name, bool UpdateCache = true)
        {
            bool Result = false;
            string path = GetFolder(cRootPath, name);
            if (FileNameValidator.IsValidFolderName(name) && FileNameValidator.IsValidFileName(name))
            {
                var shapefileHelper = new ShapefileHelper();
                path += "\\" + name;
                shapefileHelper.SaveMapZones(path, mapZones);
                if (UpdateCache) AddToCache();
                Result = true;
                MapName = name;
            }
            return Result;
        }

        public void SetTractorPosition(PointLatLng NewLocation, bool FromMouseClick = false)
        {
            if (FromMouseClick || (!cEditMode && !FromMouseClick))
            {
                cTractorPosition = NewLocation;
                tractorMarker.Position = NewLocation; // Update the marker position
                gmap.Refresh(); // Refresh the map to show the updated marker
                UpdateValuesFromZone();
                MapChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool UpdateZone(string name, int Rt0, int Rt1, int Rt2, int Rt3, Color zoneColor)
        {
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
                    }, zoneColor);

                mapZones.Add(mapZone);
                zoneOverlay.Polygons.Add(mapZone.ToGMapPolygon());

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
                        SaveMap(MapName, false);
                        LoadLastMap();
                        break;
                    }
                }
                Result = Found;
            }
            return Result;
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
                    SetTractorPosition(Location, true);
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
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            GMaps.Instance.PrimaryCache = new GMap.NET.CacheProviders.SQLitePureImageCache
            {
                CacheLocation = CachePath
            };

            gmap = new GMapControl
            {
                Dock = DockStyle.Fill,
                MapProvider = GMapProviders.BingSatelliteMap,
                Position = new PointLatLng(52.157902, -106.670158),
                MinZoom = 5,
                MaxZoom = 19,
                Zoom = 16,
                ShowCenter = false
            };

            gmap.MouseClick += Gmap_MouseClick;

            zoneOverlay = new GMapOverlay("mapzones");
            gpsMarkerOverlay = new GMapOverlay("gpsMarkers");
            tempMarkerOverlay = new GMapOverlay("tempMarkers");

            tractorMarker = new GMarkerGoogle(new PointLatLng(0, 0), GMarkerGoogleType.green); // Initialize with a default position
            gpsMarkerOverlay.Markers.Add(tractorMarker); // Add the tractor marker to the overlay

            gmap.Overlays.Add(zoneOverlay);
            gmap.Overlays.Add(gpsMarkerOverlay);
            gmap.Overlays.Add(tempMarkerOverlay);
        }

        private void InitializeMapZones()
        {
            mapZones = new List<MapZone>();
            currentZoneVertices = new List<PointLatLng>();
        }

        private void UpdateValuesFromZone()
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
            }
        }
    }
}