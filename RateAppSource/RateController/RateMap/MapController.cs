using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using NetTopologySuite.Index.Strtree;
using RateController.Classes;
using RateController.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private static MapState cState;
        private static PointLatLng cTractorPosition;
        private static MapZone CurrentZone;
        private static List<PointLatLng> currentZoneVertices;
        private static LegendManager legendManager;
        private static List<MapZone> mapZones;
        private static STRtree<MapZone> zoneIndex;        // spatial index for fast zone lookup

        public static event EventHandler MapLeftClicked;
        public static event EventHandler MapZoomed;
        public static event EventHandler MapChanged;

        public static GMapControl Map
        { get { return gmap; } }

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

        public static void Initialize()
        {
            cState = MapState.Tracking;
            InitializeMap();

            // map zones
            mapZones = new List<MapZone>();
            currentZoneVertices = new List<PointLatLng>();

            gmap.OnMapZoomChanged += Gmap_OnMapZoomChanged;
            gmap.MouseClick += Gmap_MouseClick;

            UpdateCurrentZone();
        }

        public static void SetTractorPosition(PointLatLng NewLocation, double[] AppliedRates, double[] TargetRates)
        {
        }

        public static void UpdateTargetRates()
        {
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

            // Initialize legend manager and host
            legendManager = new LegendManager(gmap);
        }

        private static void Refresh()
        {
            if (Props.RateMapIsVisible())
            {
                gmap.Refresh();
            }
        }

        private static void UpdateCurrentZone()
        {
            try
            {
                bool Found = false;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/UpdateCurrentZone: " + ex.Message);
            }
        }
    }
}