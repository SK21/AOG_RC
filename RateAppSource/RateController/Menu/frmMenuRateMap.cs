using GMap.NET.WindowsForms;
using GMap.NET;
using RateController.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NetTopologySuite.Geometries;
using RateController.Forms;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms.Markers;
using System.IO;
using System.Linq;
using System.Security.Policy;

namespace RateController.Menu
{
    public partial class frmMenuRateMap : Form
    {
        private readonly Color[] zoneColors = { Color.Blue, Color.Green, Color.Red, Color.Purple, Color.Orange };
        private string CachePath;
        private bool cEdited;
        private string cRootPath;
        private PointLatLng currentLocation;
        private List<PointLatLng> currentZoneVertices;
        private bool EditMode = false;
        private GMapControl gmap;
        private GMapOverlay gpsMarkerOverlay;
        private bool Initializing = false;
        private bool isDragging = false;
        private System.Drawing.Point lastMousePosition;
        private frmMenu MainMenu;
        private List<MapZone> mapZones;
        private FormStart mf;
        private GMapOverlay tempMarkerOverlay;
        private GMapOverlay zoneOverlay;

        public frmMenuRateMap(FormStart main, frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            mf = main;
            this.Tag = false;
            cRootPath = GetFolder(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RateMap");
            CachePath = GetFolder(cRootPath, "MapCache");
            InitializeMap();
            InitializeMapZones();
            gmap.MouseDown += Gmap_MouseDown;
            gmap.MouseMove += Gmap_MouseMove;
            gmap.MouseUp += Gmap_MouseUp;
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

        private void btnCreateZone_Click(object sender, EventArgs e)
        {
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

                int RateA = 0;
                int RateB = 0;
                int RateC = 0;
                int RateD = 0;
                if (int.TryParse(tbP1.Text, out int p1)) RateA = p1;
                if (int.TryParse(tbP2.Text, out int p2)) RateB = p2;
                if (int.TryParse(tbP3.Text, out int p3)) RateC = p3;
                if (int.TryParse(tbP4.Text, out int p4)) RateD = p4;

                string name = "Zone " + mapZones.Count.ToString();
                if (tbName.Text != "") name = tbName.Text;

                var zoneColor = zoneColors[mapZones.Count % zoneColors.Length];
                var mapZone = new MapZone(name, polygon, new Dictionary<string, int>
                    {
                        { "ProductA", RateA },
                        { "ProductB", RateB },
                        { "ProductC", RateC },
                        { "ProductD", RateD }
                    }, zoneColor);

                mapZones.Add(mapZone);
                zoneOverlay.Polygons.Add(mapZone.ToGMapPolygon());

                currentZoneVertices.Clear();
                tempMarkerOverlay.Markers.Clear();
            }
            else
            {
                MessageBox.Show("A map zone must have at least three vertices.", "Error");
                return;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Find the zone to delete
            var zoneToRemove = mapZones.FirstOrDefault(zone => zone.Name == tbName.Text);

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
                    }
                    else
                    {
                        MessageBox.Show("Polygon not found in overlay.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Failed to convert zone to polygon.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Zone not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            Form frm = new RateController.Forms.frmImport(mf);
            frm.ShowDialog();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = cRootPath;
            openFileDialog1.Filter = "Shapefiles (*.shp)|*.shp|All files (*.*)|*.*";
            openFileDialog1.Title = "Select a Shapefile";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog1.FileName;
                if (FileNameValidator.IsValidFolderName(path))
                {
                    if (File.Exists(path))
                    {
                        tbMapName.Text = Path.GetFileNameWithoutExtension(path);
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
                        VSzoom.Value = (int)((gmap.Zoom - gmap.MinZoom) * 100 / (gmap.MaxZoom - gmap.MinZoom));
                    }
                    else
                    {
                        MessageBox.Show("File not found.");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid name.");
                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
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
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string path = GetFolder(cRootPath, tbMapName.Text);
            if (FileNameValidator.IsValidFolderName(tbMapName.Text) && FileNameValidator.IsValidFileName(tbMapName.Text))
            {
                var shapefileHelper = new ShapefileHelper();
                path += "\\" + tbMapName.Text;
                shapefileHelper.SaveMapZones(path, mapZones);
                AddToCache();
                MessageBox.Show("Map Zones saved successfully!");
            }
            else
            {
                MessageBox.Show("Invalid name.");
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

        private void ckEdit_CheckedChanged(object sender, EventArgs e)
        {
            EditMode = ckEdit.Checked;
            SetButtons();
        }

        private void ckFullScreen_CheckedChanged(object sender, EventArgs e)
        {
            if (ckFullScreen.Checked)
            {
                //this.FormBorderStyle = FormBorderStyle.None;
                this.Bounds = Screen.GetWorkingArea(this);
                //this.WindowState = FormWindowState.Maximized;
                pictureBox1.Size = new Size(this.ClientSize.Width - 565, this.ClientSize.Height - 28);
                pictureBox1.Location = new System.Drawing.Point(550, 14);
            }
            else
            {
                //this.FormBorderStyle = FormBorderStyle.None;
                //this.WindowState = FormWindowState.Normal;
                this.Width = 540;
                this.Height = 630;
                pictureBox1.Size = new Size(306, 238);
                pictureBox1.Location = new System.Drawing.Point(222, 380);
                PositionForm();
            }
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
                    UpdateRate(Location);
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

        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            mf.Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);
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
                Position = new PointLatLng(53.38955, -103.625677),
                MinZoom = 5,
                MaxZoom = 19,
                Zoom = 16,
                ShowCenter = false
            };

            gmap.MouseClick += Gmap_MouseClick;

            zoneOverlay = new GMapOverlay("mapzones");
            gpsMarkerOverlay = new GMapOverlay("gpsMarkers");
            tempMarkerOverlay = new GMapOverlay("tempMarkers");

            gmap.Overlays.Add(zoneOverlay);
            gmap.Overlays.Add(gpsMarkerOverlay);
            gmap.Overlays.Add(tempMarkerOverlay);

            pictureBox1.Controls.Add(gmap);

            VSzoom.Value = (int)((gmap.Zoom - gmap.MinZoom) * 100 / (gmap.MaxZoom - gmap.MinZoom));
        }

        private void InitializeMapZones()
        {
            mapZones = new List<MapZone>();
            currentZoneVertices = new List<PointLatLng>();
        }

        private void MainMenu_MenuMoved(object sender, EventArgs e)
        {
            PositionForm();
        }

        private void mnuRateMap_FormClosed(object sender, FormClosedEventArgs e)
        {
            mf.Tls.SaveFormData(this);
        }

        private void mnuRateMap_Load(object sender, EventArgs e)
        {
            // menu 800,600
            // sub menu 540,630
            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            mf.Tls.LoadFormData(this, "", false);
            this.BackColor = Properties.Settings.Default.BackColour;
            this.Width = MainMenu.Width - 260;
            this.Height = MainMenu.Height - 50;
            MainMenu.StyleControls(this);
            PositionForm();
            UpdateForm();
        }

        private void PositionForm()
        {
            this.Top = MainMenu.Top + 30;
            this.Left = MainMenu.Left + 246;
        }

        private void SetButtons()
        {
            tbName.Enabled = EditMode;
            tbP1.Enabled = EditMode;
            tbP2.Enabled = EditMode;
            tbP3.Enabled = EditMode;
            tbP4.Enabled = EditMode;
        }

        private void SetLanguage()
        {
        }

        private void UpdateForm()
        {
            Initializing = true;

            Initializing = false;
        }

        private void UpdateRate(PointLatLng Location)
        {
            bool Found = false;
            if (!EditMode)
            {
                for (int i = mapZones.Count - 1; i >= 0; i--)
                {
                    var zone = mapZones[i];
                    if (zone.Contains(Location))
                    {
                        tbName.Text = zone.Name;
                        tbP1.Text = zone.Rates["ProductA"].ToString();
                        tbP2.Text = zone.Rates["ProductB"].ToString();
                        tbP3.Text = zone.Rates["ProductC"].ToString();
                        tbP4.Text = zone.Rates["ProductD"].ToString();
                        Found = true;
                        break;
                    }
                }
                if (!Found)
                {
                    tbName.Text = "N/A (Outside Zones)";
                    tbP1.Text = "0";
                    tbP2.Text = "0";
                    tbP3.Text = "0";
                    tbP4.Text = "0";
                }
            }
        }

        private void VSzoom_Scroll(object sender, ScrollEventArgs e)
        {
            gmap.Zoom = (gmap.MaxZoom - gmap.MinZoom) * VSzoom.Value / 100 + gmap.MinZoom;
        }
    }
}