using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using RateController.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RateController.RateMap
{
    public static class MapController
    {
        #region GMap

        private static GMapOverlay AppliedOverlay;
        private static GMapControl gmap;
        private static GMapOverlay markersOverlay;

        #endregion GMap

        public static event EventHandler MapZoomed;
        public static event EventHandler MapLeftClicked;

        public static GMapControl Map
        { get { return gmap; } }

        public static void Close()
        {
            Props.SetProp("LastMapLat", gmap.Position.Lat.ToString("N10"));
            Props.SetProp("LastMapLng", gmap.Position.Lng.ToString("N10"));
        }

        public static void Initialize()
        {
            InitializeMap();
            gmap.OnMapZoomChanged += Gmap_OnMapZoomChanged;
            gmap.MouseClick += Gmap_MouseClick;
        }

        private static void Gmap_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MapLeftClicked?.Invoke(null, EventArgs.Empty);
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
            gmap.MapProvider = ArcGIS_World_Imagery_Provider.Instance;
            //  gmap.MapProvider = Props.MapShowTiles
            //? (GMapProvider)ArcGIS_World_Imagery_Provider.Instance
            //: (GMapProvider)GMapProviders.EmptyProvider;

            gmap.Position = new PointLatLng(Lat, Lng);
            gmap.ShowCenter = false;
            gmap.MinZoom = 1;
            gmap.MaxZoom = 19;
            gmap.Zoom = 15;
            gmap.Dock = DockStyle.Fill;

            // overlays
            markersOverlay = new GMapOverlay("markers");
            AddOverlay(markersOverlay);
        }
    }
}