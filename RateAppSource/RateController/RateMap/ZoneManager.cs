using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.Strtree;
using RateController.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace RateController.RateMap
{
    public static class CurrentZone
    {
        public static double Hectares { get; set; } = 0.0;
        public static bool IsDefined { get; set; } = false;
        public static MapZone Zone { get; set; } = new MapZone("Base Rate", null, new Dictionary<string, double>(), Color.Blue, ZoneType.Target);
    }

    public class ZoneManager
    {
        private const double NearZero = 0.01;
        private readonly CoverageTrail Trail = new CoverageTrail();
        private GMapOverlay cAppliedOverlay;
        private List<MapZone> cAppliedZonesList = new List<MapZone>();
        private double[] cLookAheadSeconds = new double[Props.MaxProducts - 2];
        private GMapOverlay cNewZoneMarkerOverlay;
        private bool cShowApplied;
        private bool cShowTarget;
        private GMapOverlay cTargetOverlay;
        private List<MapZone> cTargetZonesList = new List<MapZone>();
        private int LastHistoryCount;
        private DateTime LastHistoryLastTimestamp;
        private string LastLoadedMapPath;
        private int LastProductRates = -1;
        private List<PointLatLng> NewZoneVertices;
        private STRtree<MapZone> STRtreeZoneIndex;

        #region auto tune

        private bool cAutoTune = false;
        private DateTime TuningLoopTime = new DateTime();
        private int[] TuningState = new int[Props.MaxProducts - 2];
        private DateTime[] TuningTimeMatchApplied = new DateTime[Props.MaxProducts - 2];
        private DateTime[] TuningTimeMatchTarget = new DateTime[Props.MaxProducts - 2];
        private DateTime[] TuningTimeStart = new DateTime[Props.MaxProducts - 2];

        #endregion auto tune

        public ZoneManager()
        {
            cNewZoneMarkerOverlay = new GMapOverlay("tempMarkers");
            cAppliedOverlay = new GMapOverlay("AppliedRates");
            cTargetOverlay = new GMapOverlay("TargetRates");
            NewZoneVertices = new List<PointLatLng>();

            LoadData();
            Props.ProfileChanged += Props_ProfileChanged;
        }

        public event EventHandler ZonesChanged;

        public GMapOverlay AppliedOverlay
        { get { return cAppliedOverlay; } }

        public bool AppliedOverlayVisible
        {
            get { return cShowApplied; }
            set
            {
                cShowApplied = value;
                Props.SetProp("MapShowAppliedOverlay", cShowApplied.ToString());
                ShowAppliedOverlay();
            }
        }

        public List<MapZone> AppliedZonesList
        { get { return cAppliedZonesList; } }

        public bool AutoTune
        {
            get { return cAutoTune; }
            set
            {
                cAutoTune = value;
                Props.SetProp("MapAutoTune", cAutoTune.ToString());
            }
        }

        public double[] LookAheadSeconds
        {
            get { return cLookAheadSeconds; }
            set
            {
                if (value.Count() == cLookAheadSeconds.Count())
                {
                    cLookAheadSeconds = value;
                    for (int i = 0; i < cLookAheadSeconds.Count(); i++)
                    {
                        Props.SetProp("LookAhead" + i.ToString(), cLookAheadSeconds[i].ToString());
                    }
                }
            }
        }

        public GMapOverlay NewZoneMarkerOverlay
        { get { return cNewZoneMarkerOverlay; } }

        public STRtree<MapZone> Rtree
        { get { return STRtreeZoneIndex; } }

        public GMapOverlay TargetOverlay
        { get { return cTargetOverlay; } }

        public bool TargetOverlayVisible
        {
            get { return cShowTarget; }
            set
            {
                cShowTarget = value;
                Props.SetProp("MapShowTargetOverlay", cShowTarget.ToString());
                ShowTargetOverlay();
            }
        }

        public List<MapZone> TargetZoneslist
        { get { return cTargetZonesList; } }

        /// <summary>
        /// Creates a deep copy of the given zone for use as the CurrentZone.
        /// All public data is copied, including geometry and rates, so that
        /// mutations to CurrentZone.Zone do not affect the original MapZone.
        /// </summary>
        public static MapZone CloneZoneForCurrent(MapZone source)
        {
            if (source == null)
            {
                return null;
            }

            // Deep-copy geometry as a Polygon using NTS API if present
            Polygon clonedGeometry = null;
            if (source.Geometry != null)
            {
                // Geometry.Copy() preserves the concrete type when possible
                clonedGeometry = source.Geometry.Copy() as Polygon;
            }

            // Deep-copy rates dictionary
            var clonedRates = new Dictionary<string, double>(source.Rates.Count);
            foreach (var kvp in source.Rates)
            {
                clonedRates[kvp.Key] = kvp.Value;
            }

            // Construct new MapZone with copied data
            var clone = new MapZone(
                source.Name,
                clonedGeometry,
                clonedRates,
                source.ZoneColor,
                source.ZoneType);

            return clone;
        }

        public void AddVertex(PointLatLng point)
        {
            NewZoneVertices.Add(point);
            cNewZoneMarkerOverlay.Markers.Add(new GMarkerGoogle(point, GMarkerGoogleType.red_small));
        }

        public bool BuildAppliedFromHistory(GMapOverlay overlay, out Dictionary<string, Color> legend)
        {
            legend = new Dictionary<string, Color>();
            try
            {
                MapController.RateCollector.LoadData(); // ensure fresh data
                var readings = MapController.RateCollector.GetReadings();
                if (overlay == null || readings == null || readings.Count < 2) return false;

                int maxLen = readings.Max(r => (r.AppliedRates?.Length ?? 0));
                if (maxLen == 0) return false;

                int ProductFilter = MapController.ProductFilter;
                if (ProductFilter >= maxLen) ProductFilter = 0;

                var baseSeries = readings.Where(r => r.AppliedRates != null && r.AppliedRates.Length > ProductFilter).Select(r => r.AppliedRates[ProductFilter]);

                if (!MapController.TryComputeScale(baseSeries, out double minRate, out double maxRate))
                    return false;

                Trail.Reset();
                PointLatLng prevPoint = new PointLatLng(readings[0].Latitude, readings[0].Longitude);
                DateTime prevTime = readings[0].Timestamp;

                // Relocation / gap guards (mirrors live logic + time gap)
                const double maxSnapMeters = 5.0;          // break if spatial gap exceeds this
                const double MaxGapSeconds = 3.0;          // break if temporal gap (record interval ~1s) too large

                for (int i = 0; i < readings.Count; i++)
                {
                    var r = readings[i];
                    var currPoint = new PointLatLng(r.Latitude, r.Longitude);
                    double heading = i == 0 ? 0.0 : BearingDegrees(prevPoint, currPoint);

                    double rateValue = (r.AppliedRates.Length > ProductFilter ? r.AppliedRates[ProductFilter] : 0.0);

                    bool canBridge = rateValue > NearZero;
                    if (i > 0)
                    {
                        double distToPrev = DistanceMeters(currPoint, prevPoint.Lat, prevPoint.Lng);
                        if (distToPrev > maxSnapMeters) canBridge = false;

                        TimeSpan gap = r.Timestamp - prevTime;
                        if (gap.TotalSeconds > MaxGapSeconds) canBridge = false;
                    }

                    if (canBridge)
                    {
                        Trail.AddPoint(currPoint, heading, rateValue, r.ImplementWidthMeters);
                    }
                    else
                    {
                        Trail.Break();
                    }

                    prevPoint = currPoint;
                    prevTime = r.Timestamp;
                }

                List<double> AveRates = new List<double>();
                Trail.DrawTrail(overlay, minRate, maxRate, out AveRates);

                // After drawing the trail, assign applied rates to each polygon for shapefile export
                if (overlay.Polygons.Count > 0)
                {
                    int polygonCount = overlay.Polygons.Count;
                    int AveRatesCount = AveRates.Count;

                    for (int i = 0; i < polygonCount; i++)
                    {
                        double rate = 0;
                        if (i < AveRatesCount) rate = AveRates[i];

                        var rates = new Dictionary<string, double>
                        {
                            { ZoneFields.ProductA, 0 },
                            { ZoneFields.ProductB, 0 },
                            { ZoneFields.ProductC, 0 },
                            { ZoneFields.ProductD, 0 },
                        };
                        rates[ZoneFields.Products[ProductFilter]] = rate;

                        overlay.Polygons[i].Tag = rates;
                    }
                }

                legend = MapController.legendManager.CreateAppliedLegend(minRate, maxRate, 5);
                return true;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog($"ZoneManager/BuildFromHistory: {ex.Message}");
                return false;
            }
        }

        public bool BuildNewAppliedZones(out List<MapZone> NewAppliedZones)
        {
            bool Result = false;
            NewAppliedZones = new List<MapZone>();

            try
            {
                // check for recorded data
                bool RecordedData = false;
                for (int i = 0; i < 4; i++)
                {
                    if (MapController.RateCollector.DataPoints(i) > 0)
                    {
                        RecordedData = true;
                        break;
                    }
                }

                if (RecordedData)
                {
                    Dictionary<string, Color> histLegend;
                    GMapOverlay AppliedOverlay = new GMapOverlay();

                    bool histOk = BuildAppliedFromHistory(AppliedOverlay, out histLegend);
                    if (histOk && AppliedOverlay.Polygons.Count > 0)
                    {
                        int count = 0;
                        foreach (var polygon in AppliedOverlay.Polygons)
                        {
                            Color zoneColor = Color.AliceBlue;
                            if (polygon.Fill is SolidBrush sb)
                            {
                                zoneColor = Color.FromArgb(255, sb.Color);
                            }

                            NewAppliedZones.Add(new MapZone(
                                name: $"Applied Zone {count++}",
                                geometry: ConvertToNtsPolygon(polygon),
                                rates: (Dictionary<string, double>)polygon.Tag,
                                zoneColor: zoneColor,
                                zoneType: ZoneType.Applied));
                        }
                        Result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ZoneManager/BuildNewAppliedZones: " + ex.Message);
            }

            return Result;
        }

        public void Close()
        {
            LookAheadSeconds = cLookAheadSeconds;   // save any changes from auto tuning
            cAppliedOverlay = null;
            cTargetOverlay = null;
            cNewZoneMarkerOverlay = null;
            cAppliedZonesList = null;
            cTargetZonesList = null;
            ResetTrail();
            STRtreeZoneIndex = null;
            Props.ProfileChanged -= Props_ProfileChanged;
        }

        public bool CreateZone(string name, double Rt0, double Rt1, double Rt2, double Rt3, Color zoneColor, out int ErrorCode)
        {
            bool Result = false;
            ErrorCode = 0;
            try
            {
                if (ZoneNameFound(name))
                {
                    ErrorCode = 1;
                }
                else if (NewZoneVertices.Count < 3)
                {
                    ErrorCode = 2;
                }
                else
                {
                    var geometryFactory = new GeometryFactory();
                    var coordinates = NewZoneVertices.ConvertAll(p => new Coordinate(p.Lng, p.Lat)).ToArray();

                    if (!coordinates[0].Equals(coordinates[coordinates.Length - 1]))
                    {
                        Array.Resize(ref coordinates, coordinates.Length + 1);
                        coordinates[coordinates.Length - 1] = coordinates[0];
                    }
                    var polygon = geometryFactory.CreatePolygon(coordinates);

                    MapZone NewZone = new MapZone(name, polygon, new Dictionary<string, double>
                    {
                        { ZoneFields.ProductA, Rt0 },
                        { ZoneFields.ProductB, Rt1 },
                        { ZoneFields.ProductC, Rt2 },
                        { ZoneFields.ProductD, Rt3 }
                    }, zoneColor, ZoneType.Target);

                    cTargetZonesList.Add(NewZone);
                    AddPolygons(cTargetOverlay, NewZone.ToGMapPolygons(Palette.TargetZoneTransparency));

                    NewZoneVertices.Clear();
                    cNewZoneMarkerOverlay.Markers.Clear();

                    BuildTargetZonesIndex();

                    ZonesChanged?.Invoke(null, EventArgs.Empty);
                    Result = true;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("MapController/CreateZone: " + ex.Message);
            }
            return Result;
        }

        public void DeleteLastVertex()
        {
            try
            {
                if (NewZoneVertices.Count > 0)
                {
                    NewZoneVertices.RemoveAt(NewZoneVertices.Count - 1);
                    if (cNewZoneMarkerOverlay.Markers.Count > 0)
                    {
                        cNewZoneMarkerOverlay.Markers.RemoveAt(cNewZoneMarkerOverlay.Markers.Count - 1);
                    }
                    MapController.Refresh();
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ZoneManager/DeleteLastVertex: " + ex.Message);
            }
        }

        public bool DeleteZone(string name)
        {
            bool Result = false;
            try
            {
                if (string.IsNullOrEmpty(name) || cTargetZonesList == null || cTargetOverlay == null) return false;

                // collect all zones with the given name (multi-polygons often share the same name)
                var zonesToRemove = cTargetZonesList.Where(z => string.Equals(z.Name, name, StringComparison.Ordinal)).ToList();
                if (zonesToRemove.Count == 0) return false;

                foreach (var zone in zonesToRemove)
                {
                    // remove polygons from overlay that match the ones created for this zone
                    List<GMapPolygon> polygonsToRemove = zone.ToGMapPolygons(Palette.ZoneTransparency);
                    foreach (var polygonToRemove in polygonsToRemove)
                    {
                        if (polygonToRemove == null) continue;

                        var polygonInOverlay = cTargetOverlay.Polygons.FirstOrDefault(polygon => polygon.Points.SequenceEqual(polygonToRemove.Points));

                        if (polygonInOverlay != null)
                        {
                            cTargetOverlay.Polygons.Remove(polygonInOverlay);
                            Result = true;
                        }
                    }
                }

                // also remove any remaining polygons in the overlay that carry this name (safety)
                var leftovers = cTargetOverlay.Polygons.Where(p => string.Equals(p.Name, name, StringComparison.Ordinal) || (p.Name != null && p.Name.StartsWith(name + "_hole", StringComparison.Ordinal))).ToList();
                foreach (var p in leftovers) cTargetOverlay.Polygons.Remove(p);

                // remove zones from the internal list
                cTargetZonesList.RemoveAll(z => string.Equals(z.Name, name, StringComparison.Ordinal));

                if (Result)
                {
                    //RemoveOverlay(zoneOverlay);   todo: check if this is necessary
                    //AddOverlay(zoneOverlay);

                    BuildTargetZonesIndex();

                    MapController.Refresh();
                    MapController.SaveMap();
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ZoneManager/DeleteZone: " + ex.Message);
            }
            return Result;
        }

        public bool EditZone(string name, double Rt0, double Rt1, double Rt2, double Rt3, Color zoneColor, out int ErrorCode)
        {
            // rates, color
            bool Result = false;
            ErrorCode = 0;
            try
            {
                MapZone ZoneToEdit = CurrentZone.Zone;
                if (ZoneNameFound(name, ZoneToEdit))
                {
                    // check for duplicate name
                    ErrorCode = 1;
                }
                else
                {
                    ZoneToEdit.Name = name;
                    Dictionary<string, double> NewRates = new Dictionary<string, double>
                    {
                        { ZoneFields.Products[0], Rt0 },
                        { ZoneFields.Products[1], Rt1 },
                        { ZoneFields.Products[2], Rt2 },
                        { ZoneFields.Products[3], Rt3 }
                    };
                    ZoneToEdit.Rates = NewRates;
                    ZoneToEdit.ZoneColor = zoneColor;

                    // Refresh polygons in overlay to reflect new color
                    var polygonsForZone = ZoneToEdit.ToGMapPolygons(Palette.TargetZoneTransparency);
                    foreach (var polygonToReplace in polygonsForZone)
                    {
                        if (polygonToReplace == null) continue;
                        var existing = cTargetOverlay.Polygons.FirstOrDefault(p => p.Points.SequenceEqual(polygonToReplace.Points));
                        if (existing != null)
                        {
                            cTargetOverlay.Polygons.Remove(existing);
                        }
                    }
                    AddPolygons(cTargetOverlay, polygonsForZone);

                    ZonesChanged?.Invoke(null, EventArgs.Empty);
                    Result = true;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ZoneManger/EditZone: " + ex.Message);
            }
            return Result;
        }

        public double GetTargetRateWithLookAhead(int productIndex, PointLatLng tractorPos, double headingDegrees, double speedMetersPerSecond)
        {
            double targetRate = 0.0;

            try
            {
                if (productIndex >= 0 && productIndex < ZoneFields.Products.Length)
                {
                    MapZone zoneToUse = null;
                    if (productIndex < cLookAheadSeconds.Length && cLookAheadSeconds[productIndex] > 0 && speedMetersPerSecond > 0.0)
                    {
                        // use look-ahead rate

                        // look-ahead point
                        double delaySeconds = cLookAheadSeconds[productIndex];
                        double lookAheadDistanceMeters = speedMetersPerSecond * delaySeconds;
                        PointLatLng lookAheadPoint = ProjectPoint(tractorPos, headingDegrees, lookAheadDistanceMeters);

                        MapZone lookAheadZone = FindZoneAtPoint(lookAheadPoint);
                        if (lookAheadZone == null)
                        {
                            // use base rate at projected point
                            var rates = Props.MainForm.Products.BaseRates();
                            targetRate = rates[productIndex];
                        }
                        else
                        {
                            // use zone rate at project point
                            zoneToUse = lookAheadZone;
                            if (zoneToUse != null && zoneToUse.Rates != null)
                            {
                                string productKey = ZoneFields.Products[productIndex];

                                double value;
                                if (zoneToUse.Rates.TryGetValue(productKey, out value))
                                {
                                    targetRate = value;
                                }
                            }
                        }
                    }
                    else
                    {
                        // use current rate
                        zoneToUse = CurrentZone.Zone;
                        if (zoneToUse != null && zoneToUse.Rates != null)
                        {
                            string productKey = ZoneFields.Products[productIndex];

                            double value;
                            if (zoneToUse.Rates.TryGetValue(productKey, out value))
                            {
                                targetRate = value;
                            }
                        }
                    }

                    // auto-tune user-provided look-ahead time (persisted)
                    AutoTuneLookAhead(productIndex, targetRate);
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ZoneManager/GetTargetRateWithLookAhead: " + ex.Message);
            }

            return targetRate;
        }

        public void LoadZones()
        {
            var shapefileHelper = new ShapefileHelper();
            List<MapZone> mapZones = shapefileHelper.CreateZoneList(JobManager.CurrentMapPath);

            // split zones by type
            cTargetZonesList = mapZones.Where(z => z.ZoneType == ZoneType.Target).ToList();
            cAppliedZonesList = mapZones.Where(z => z.ZoneType == ZoneType.Applied).ToList();

            BuildTargetZonesIndex();
            ShowTargetOverlay();
            ShowAppliedOverlay();
        }

        public void ResetAppliedOverlay()
        {
            ResetTrail();
            MapController.legendManager.ShowLegend(null, false);
            AppliedOverlay.Clear();
            LastHistoryCount = 0;
            LastHistoryLastTimestamp = DateTime.MinValue;
        }

        public void ResetMarkers()
        {
            NewZoneVertices.Clear();
            cNewZoneMarkerOverlay.Markers.Clear();
        }

        public void ResetTrail() => Trail.Reset();

        public void ShowAppliedOverlay()
        {
            try
            {
                if (cShowApplied)
                {
                    // Decide whether to rebuild coverage from history
                    var readings = MapController.RateCollector.GetReadings();
                    bool OverlayIsCurrent = AppliedOverlayIsCurrent(JobManager.CurrentMapPath, readings);

                    if (!OverlayIsCurrent)
                    {
                        // rebuild applied overlay

                        cAppliedOverlay.Polygons.Clear();
                        Dictionary<string, Color> histLegend;
                        if (BuildAppliedFromHistory(cAppliedOverlay, out histLegend))
                        {
                            // Use legend returned from history build
                            LegendObject LegObj = new LegendObject
                            {
                                Legend = histLegend,
                                ProductName = Props.MainForm.Products.Item(MapController.ProductFilter).ProductName
                            };
                            MapController.legendManager.AppliedLegendObject = LegObj;

                            OverlayIsCurrent = true;
                        }
                        else
                        {
                            // use historical applied zones from shapefile
                            if (cAppliedZonesList.Count > 0)
                            {
                                int productIndex = MapController.ProductFilter;
                                string productKey = ZoneFields.Products[productIndex];

                                // Filter zones that actually have a rate for the selected product
                                List<MapZone> zonesForProduct = cAppliedZonesList
                                    .Where(z => z != null &&
                                                z.Rates != null &&
                                                z.Rates.ContainsKey(productKey) &&
                                                z.Rates[productKey] > NearZero)
                                    .ToList();

                                if (zonesForProduct.Count > 0)
                                {
                                    foreach (MapZone mapZone in zonesForProduct)
                                    {
                                        AddPolygons(cAppliedOverlay, mapZone.ToGMapPolygons(Palette.ZoneTransparency));
                                    }

                                    // Build legend that matches persisted applied zones for this product
                                    LegendObject LoadedLegend = MapController.legendManager.LoadPersistedLegend();
                                    if (LoadedLegend == null)
                                    {
                                        Dictionary<string, Color> AppliedLegend = MapController.legendManager.BuildAppliedZonesLegend(zonesForProduct, productIndex);

                                        LoadedLegend = new LegendObject
                                        {
                                            Legend = AppliedLegend,
                                            ProductName = Props.MainForm.Products.Item(productIndex).ProductName
                                        };
                                    }
                                    MapController.legendManager.AppliedLegendObject = LoadedLegend;

                                    OverlayIsCurrent = true;
                                }
                            }
                        }

                        if (OverlayIsCurrent)
                        {
                            // update signature after a successful build
                            LastLoadedMapPath = JobManager.CurrentMapPath;
                            if (readings != null && readings.Count > 0)
                            {
                                LastHistoryCount = readings.Count;
                                LastHistoryLastTimestamp = readings[readings.Count - 1].Timestamp;
                            }
                            else
                            {
                                LastHistoryCount = 0;
                                LastHistoryLastTimestamp = DateTime.MinValue;
                            }
                            LastProductRates = MapController.ProductFilter;
                        }
                    }

                    if (OverlayIsCurrent)
                    {
                        MapController.AddOverlay(cAppliedOverlay);
                        MapController.legendManager.ShowLegend();
                    }
                    else
                    {
                        ResetAppliedOverlay();
                    }
                }
                else
                {
                    MapController.RemoveOverlay(cAppliedOverlay);
                    MapController.legendManager.Clear();
                }
                MapController.Refresh();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ZoneManager/ShowAppliedOverlay: " + ex.Message);
            }
        }

        public void ShowTargetOverlay()
        {
            if (cShowTarget)
            {
                // Rebuild polygons to ensure correct rendering after map resize
                cTargetOverlay.Polygons.Clear();
                foreach (var mapZone in cTargetZonesList)
                {
                    AddPolygons(cTargetOverlay, mapZone.ToGMapPolygons(Palette.TargetZoneTransparency));
                }

                MapController.AddOverlay(cTargetOverlay);
            }
            else
            {
                MapController.RemoveOverlay(cTargetOverlay);
            }
            MapController.Refresh();
        }

        public int TargetZoneCount()
        {
            return cTargetZonesList.Count;
        }

        public void UpdateAppliedOverlay(double[] AppliedRates)
        {
            Dictionary<string, Color> newLegend = new Dictionary<string, Color>();
            try
            {
                if (cShowApplied && MapController.MapIsDisplayed && (MapController.State == MapState.Tracking || MapController.State == MapState.Preview))
                {
                    var readings = MapController.RateCollector.GetReadings();
                    if (readings == null || readings.Count == 0)
                    {
                        ResetAppliedOverlay();
                    }
                    else
                    {
                        double Rates = AppliedRates[MapController.ProductFilter];  // product rates to display

                        UpdateTrail(cAppliedOverlay, readings, MapController.TractorPosition, MapController.TravelHeading,
                           Props.MainForm.Sections.TotalWidth(), Rates, out newLegend, MapController.ProductFilter);
                    }
                }

                LegendObject LegObj = new LegendObject
                {
                    Legend = newLegend,
                    ProductName = Props.MainForm.Products.Item(MapController.ProductFilter).ProductName
                };
                MapController.legendManager.AppliedLegendObject = LegObj;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ZoneManager/UpdateAppliedOverlay: " + ex.Message);
            }
        }

        private void AddPolygons(GMapOverlay overlay, List<GMapPolygon> polygons)
        {
            foreach (var polygon in polygons)
            {
                // remove stroke(border) to match AOG polygon look overlap-free
                polygon.Stroke = Pens.Transparent;
                overlay.Polygons.Add(polygon);
            }
        }

        private bool AppliedOverlayIsCurrent(string mapPath, IReadOnlyList<RateReading> readings)
        {
            if (string.IsNullOrEmpty(mapPath) || readings == null || readings.Count == 0) return false;

            var lastTs = readings[readings.Count - 1].Timestamp;
            return string.Equals(LastLoadedMapPath, mapPath, StringComparison.Ordinal) &&
                   LastHistoryCount == readings.Count &&
                   LastHistoryLastTimestamp == lastTs &&
                   LastProductRates == MapController.ProductFilter &&                 // ensure product selection matches
                   cAppliedOverlay.Polygons != null &&
                   cAppliedOverlay.Polygons.Count > 0;
        }

        private void AutoTuneLookAhead(int ProductID, double LookAheadTargetRate)
        {
            try
            {
                if ((DateTime.Now - TuningLoopTime).TotalMilliseconds > 500)
                {
                    TuningLoopTime = DateTime.Now;
                    if (cAutoTune && Props.Speed_KMH > 3)
                    {
                        double CZR = CurrentZone.Zone.Rates[ZoneFields.Products[ProductID]];    // current zone target rate
                        double LZR = LookAheadTargetRate;      // look-ahead zone target rate

                        switch (TuningState[ProductID])
                        {
                            case 0:
                                // find zone boundary
                                if (CZR != LZR)
                                {
                                    TuningState[ProductID] = 1;
                                    TuningTimeStart[ProductID] = DateTime.Now;
                                }
                                break;

                            case 1:
                                // check for timed out
                                if ((DateTime.Now - TuningTimeStart[ProductID]).TotalSeconds < 20)
                                {
                                    // find when rate applied rate matches target rate, 10% leeway
                                    double diff = Math.Abs(LZR - Props.MainForm.Products.Item(ProductID).CurrentRate());
                                    if ((diff < (LZR * 0.1)) && TuningTimeMatchApplied[ProductID] == DateTime.MinValue)
                                    {
                                        TuningTimeMatchApplied[ProductID] = DateTime.Now;
                                    }

                                    // find when tractor is in new zone, either before or after rate matches
                                    if (CZR == LZR && TuningTimeMatchTarget[ProductID] == DateTime.MinValue)
                                    {
                                        TuningTimeMatchTarget[ProductID] = DateTime.Now;
                                    }

                                    // find when both times have occured
                                    if (TuningTimeMatchApplied[ProductID] != DateTime.MinValue && TuningTimeMatchTarget[ProductID] != DateTime.MinValue)
                                    {
                                        // adjust look-ahead time
                                        double AdjustSeconds = (TuningTimeMatchApplied[ProductID] - TuningTimeMatchTarget[ProductID]).TotalSeconds;
                                        AdjustSeconds = Math.Max(-3.0, Math.Min(3.0, AdjustSeconds));
                                        double newLookAhead = cLookAheadSeconds[ProductID] + AdjustSeconds * 0.25;
                                        if (newLookAhead >= 0 && newLookAhead < 10) cLookAheadSeconds[ProductID] = newLookAhead;

                                        TuningState[ProductID] = 0;
                                        TuningTimeMatchTarget[ProductID] = DateTime.MinValue;
                                        TuningTimeMatchApplied[ProductID] = DateTime.MinValue;
                                    }
                                }
                                else
                                {
                                    // cancel tuning
                                    TuningState[ProductID] = 0;
                                    TuningTimeMatchTarget[ProductID] = DateTime.MinValue;
                                    TuningTimeMatchApplied[ProductID] = DateTime.MinValue;
                                }
                                break;
                        }
                    }
                    else
                    {
                        TuningState[ProductID] = 0;
                        TuningTimeMatchTarget[ProductID] = DateTime.MinValue;
                        TuningTimeMatchApplied[ProductID] = DateTime.MinValue;
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ZoneManager/AutoTuneLookAhead: " + ex.Message);
            }
        }

        private double BearingDegrees(PointLatLng a, PointLatLng b)
        {
            double lat1 = a.Lat * Math.PI / 180.0;
            double lat2 = b.Lat * Math.PI / 180.0;
            double dLon = (b.Lng - a.Lng) * Math.PI / 180.0;

            double y = Math.Sin(dLon) * Math.Cos(lat2);
            double x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);
            double brng = Math.Atan2(y, x) * 180.0 / Math.PI;
            return (brng + 360.0) % 360.0;
        }

        private void BuildTargetZonesIndex()
        {
            // build a STRtree object for efficiently working with spatial objects (zones)
            try
            {
                STRtreeZoneIndex = new STRtree<MapZone>();
                foreach (var z in cTargetZonesList)
                {
                    if (z?.Geometry == null) continue;
                    var env = z.Geometry.EnvelopeInternal;
                    if (env == null) continue;
                    STRtreeZoneIndex.Insert(env, z);
                }
                STRtreeZoneIndex.Build();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ZoneManager/BuildZoneIndex: " + ex.Message);
                STRtreeZoneIndex = null; // fallback gracefully
            }
        }

        private Polygon ConvertToNtsPolygon(GMapPolygon gmapPolygon)
        {
            var coords = new List<Coordinate>();
            foreach (var point in gmapPolygon.Points)
            {
                coords.Add(new Coordinate(point.Lng, point.Lat));
            }
            // Ensure closed
            if (coords.Count > 0 && !coords[0].Equals(coords[coords.Count - 1]))
            {
                coords.Add(coords[0]);
            }
            return new Polygon(new LinearRing(coords.ToArray()));
        }

        private double DistanceMeters(PointLatLng a, double bLat, double bLng)
        {
            const double metersPerDegLat = 111320.0;
            double latRad = a.Lat * Math.PI / 180.0;
            double metersPerDegLng = metersPerDegLat * Math.Cos(latRad);

            double dx = (bLng - a.Lng) * metersPerDegLng;
            double dy = (bLat - a.Lat) * metersPerDegLat;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Uses the STRtree index to find the first target zone whose geometry
        /// contains the given point. Returns null if none is found.
        /// </summary>
        private MapZone FindZoneAtPoint(PointLatLng point)
        {
            if (STRtreeZoneIndex == null)
            {
                return null;
            }

            try
            {
                const double epsilon = 1e-6;
                var env = new Envelope(point.Lng - epsilon, point.Lng + epsilon, point.Lat - epsilon, point.Lat + epsilon);

                IList<MapZone> candidates = STRtreeZoneIndex.Query(env);
                if (candidates == null || candidates.Count == 0)
                {
                    return null;
                }

                var gf = new GeometryFactory();
                var pt = gf.CreatePoint(new Coordinate(point.Lng, point.Lat));

                foreach (MapZone zone in candidates)
                {
                    if (zone != null && zone.Geometry != null && zone.Geometry.Contains(pt))
                    {
                        return zone;
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ZoneManager/FindZoneAtPoint: " + ex.Message);
            }

            return null;
        }

        private void LoadData()
        {
            cShowApplied = bool.TryParse(Props.GetProp("MapShowAppliedOverlay"), out bool sr) ? sr : false;
            cShowTarget = bool.TryParse(Props.GetProp("MapShowTargetOverlay"), out bool sz) ? sz : true;
            cAutoTune = bool.TryParse(Props.GetProp("MapAutoTune"), out bool au) ? au : false;

            double tme = 0;
            for (int i = 0; i < cLookAheadSeconds.Count(); i++)
            {
                cLookAheadSeconds[i] = double.TryParse(Props.GetProp("LookAhead" + i.ToString()), out tme) ? tme : 1;
            }
        }

        /// <summary>
        /// Projects a point forward by distanceMeters along headingDegrees using
        /// a simple local-plane approximation consistent with DistanceMeters().
        /// </summary>
        private PointLatLng ProjectPoint(PointLatLng origin, double headingDegrees, double distanceMeters)
        {
            const double metersPerDegLat = 111320.0;
            double latRad = origin.Lat * Math.PI / 180.0;
            double metersPerDegLng = metersPerDegLat * Math.Cos(latRad);

            double headingRad = headingDegrees * Math.PI / 180.0;

            double dx = Math.Sin(headingRad) * distanceMeters; // east-west
            double dy = Math.Cos(headingRad) * distanceMeters; // north-south

            double dLat = dy / metersPerDegLat;
            double dLng = dx / metersPerDegLng;

            return new PointLatLng(origin.Lat + dLat, origin.Lng + dLng);
        }

        private void Props_ProfileChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private bool UpdateTrail(GMapOverlay overlay, IReadOnlyList<RateReading> readings, PointLatLng tractorPos, double headingDegrees,
                    double implementWidthMeters, double? appliedOverride, out Dictionary<string, Color> legend, int rateIndex)
        {
            legend = new Dictionary<string, Color>();

            try
            {
                if (overlay == null) return false;
                if (readings == null || readings.Count == 0) return false;
                if (implementWidthMeters <= 0) implementWidthMeters = 0.01;

                var last = readings.Last();

                IEnumerable<double> baseSeries = readings.Where(r => r.AppliedRates != null && r.AppliedRates.Length > rateIndex).Select(r => r.AppliedRates[rateIndex]);

                if (!MapController.TryComputeScale(baseSeries, out double minRate, out double maxRate))
                    return false;

                double currValue;
                double appliedBase = (last.AppliedRates.Length > rateIndex) ? last.AppliedRates[rateIndex] : 0.0;
                currValue = appliedOverride ?? appliedBase;

                // Distance gate prevents trail bridging after relocation
                const double maxSnapMeters = 5.0;
                double distToLast = DistanceMeters(tractorPos, last.Latitude, last.Longitude);

                if (currValue > NearZero && distToLast <= maxSnapMeters)
                {
                    Trail.AddPoint(tractorPos, headingDegrees, currValue, implementWidthMeters);
                }
                else
                {
                    Trail.Break();
                }

                List<double> temp = new List<double>();
                Trail.DrawTrail(overlay, minRate, maxRate, out temp);

                legend = MapController.legendManager.CreateAppliedLegend(minRate, maxRate, 5);

                return true;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog($"ZoneManager/UpdateRatesOverlayLive: {ex.Message}");
                return false;
            }
        }

        private bool ZoneNameFound(string Name, MapZone ExcludeZone = null)
        {
            bool Result = false;
            foreach (MapZone zn in cTargetZonesList)
            {
                if (string.Equals(zn.Name, Name, StringComparison.OrdinalIgnoreCase) && zn != ExcludeZone)
                {
                    Result = true;
                    break;
                }
            }
            return Result;
        }
    }
}