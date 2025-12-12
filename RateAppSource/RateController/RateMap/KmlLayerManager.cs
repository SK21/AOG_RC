using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using RateController.Classes;
using SharpKml.Dom;
using SharpKml.Engine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Placemark = SharpKml.Dom.Placemark;

namespace RateController.RateMap
{
    // Responsible for importing a KML file and exposing a GMapOverlay layer.
    // Supports Polygon, LineString, Point and MultiGeometry.
    public sealed class KmlLayerManager
    {
        private readonly Dictionary<string, GMapOverlay> _overlaysByPath =
            new Dictionary<string, GMapOverlay>(StringComparer.OrdinalIgnoreCase);

        // Retrieve an overlay previously created for a file path.
        public GMapOverlay GetOverlay(string filePath)
        {
            _overlaysByPath.TryGetValue(filePath, out var overlay);
            return overlay;
        }

        // Load or re-load a KML file and build a GMapOverlay for it.
        // Returns the overlay instance or null on failure.
        public GMapOverlay LoadKml(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) return null;

                var overlay = new GMapOverlay("kml:" + Path.GetFileName(filePath));
                using (var stream = File.OpenRead(filePath))
                {
                    var kmlFile = KmlFile.Load(stream);
                    var root = kmlFile?.Root;
                    if (root == null) return null;

                    var placemarks = new List<Placemark>();
                    ExtractPlacemarksFromElement(root, placemarks);

                    foreach (var pm in placemarks)
                    {
                        AddPlacemarkToOverlay(pm, overlay);
                    }
                }

                if (overlay.Polygons.Count == 0 && overlay.Routes.Count == 0 && overlay.Markers.Count == 0)
                {
                    return null;
                }

                _overlaysByPath[filePath] = overlay;
                return overlay;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("KmlLayerManager/LoadKml: " + ex.Message);
                return null;
            }
        }

        // Remove a cached overlay by file path (does not remove from map; caller should).
        public bool Remove(string filePath)
        {
            return _overlaysByPath.Remove(filePath);
        }

        private static void AddLineString(Placemark pm, LineString line, GMapOverlay overlay)
        {
            var coords = line.Coordinates;
            if (coords == null) return;

            var pts = new List<GMap.NET.PointLatLng>();
            foreach (var c in coords)
            {
                pts.Add(new GMap.NET.PointLatLng(c.Latitude, c.Longitude));
            }
            if (pts.Count >= 2)
            {
                var route = new GMapRoute(pts, pm.Name ?? "KML Line");
                var lc = GetKmlLineColor(pm);
                route.Stroke = new Pen(Color.FromArgb(lc.A, lc), 2f);
                overlay.Routes.Add(route);
            }
        }

        private static void AddPlacemarkToOverlay(Placemark pm, GMapOverlay overlay)
        {
            try
            {
                if (pm?.Geometry == null) return;

                // Handle MultiGeometry by dispatching members
                if (pm.Geometry is MultipleGeometry mg && mg.Geometry != null)
                {
                    foreach (var g in mg.Geometry.OfType<Geometry>())
                    {
                        var tempPm = new Placemark
                        {
                            Name = pm.Name,
                            Geometry = g
                        };
                        AddPlacemarkToOverlay(tempPm, overlay);
                    }
                    return;
                }

                if (pm.Geometry is Polygon poly)
                {
                    AddPolygon(pm, poly, overlay);
                }
                else if (pm.Geometry is LineString line)
                {
                    AddLineString(pm, line, overlay);
                }
                else if (pm.Geometry is SharpKml.Dom.Point point)
                {
                    AddPoint(pm, point, overlay);
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("KmlLayerManager/AddPlacemarkToOverlay: " + ex.Message);
            }
        }

        private static void AddPoint(Placemark pm, SharpKml.Dom.Point point, GMapOverlay overlay)
        {
            var coord = point.Coordinate;
            var markerPos = new GMap.NET.PointLatLng(coord.Latitude, coord.Longitude);
            var marker = new GMarkerGoogle(markerPos, GMarkerGoogleType.blue_small)
            {
                ToolTipText = pm.Name ?? "KML Point"
            };
            overlay.Markers.Add(marker);
        }

        private static void AddPolygon(Placemark pm, Polygon poly, GMapOverlay overlay)
        {
            var outer = poly.OuterBoundary;
            if (outer?.LinearRing?.Coordinates != null)
            {
                var pts = new List<GMap.NET.PointLatLng>();
                foreach (var c in outer.LinearRing.Coordinates)
                {
                    // KML is lon,lat[,alt]
                    pts.Add(new GMap.NET.PointLatLng(c.Latitude, c.Longitude));
                }
                if (pts.Count >= 3)
                {
                    var gpoly = new GMapPolygon(pts, pm.Name ?? "KML Polygon");
                    // Match zone style: no stroke to avoid overlaps
                    gpoly.Stroke = Pens.Transparent;
                    var color = GetKmlPolyColor(pm);
                    gpoly.Fill = new SolidBrush(Color.FromArgb(color.A, color));
                    overlay.Polygons.Add(gpoly);
                }
            }

            // Holes (inner boundaries) rendered as fully transparent polygons
            if (poly.InnerBoundary != null)
            {
                foreach (var inner in poly.InnerBoundary)
                {
                    var ring = inner?.LinearRing?.Coordinates;
                    if (ring == null) continue;
                    var innerPts = new List<GMap.NET.PointLatLng>();
                    foreach (var c in ring)
                    {
                        innerPts.Add(new GMap.NET.PointLatLng(c.Latitude, c.Longitude));
                    }
                    if (innerPts.Count >= 3)
                    {
                        var hole = new GMapPolygon(innerPts, (pm.Name ?? "KML") + "_hole");
                        hole.Stroke = Pens.Transparent;
                        hole.Fill = new SolidBrush(Color.FromArgb(0, Color.White));
                        overlay.Polygons.Add(hole);
                    }
                }
            }
        }

        private static void ExtractPlacemarksFromElement(Element element, List<Placemark> list)
        {
            if (element == null) return;

            // If this element is a Kml root, start from its Feature
            if (element is Kml kml && kml.Feature != null)
            {
                ExtractPlacemarksFromFeature(kml.Feature, list);
                return;
            }

            // If this element is a Feature, traverse it
            var feature = element as Feature;
            if (feature != null)
            {
                ExtractPlacemarksFromFeature(feature, list);
                return;
            }

            // Otherwise, walk child elements to find features
            foreach (var child in element.Children)
            {
                ExtractPlacemarksFromElement(child, list);
            }
        }

        private static void ExtractPlacemarksFromFeature(Feature feature, List<Placemark> list)
        {
            if (feature == null) return;

            if (feature is Placemark pm)
            {
                list.Add(pm);
            }

            if (feature is Container container && container.Features != null)
            {
                foreach (var f in container.Features)
                {
                    ExtractPlacemarksFromFeature(f, list);
                }
            }
        }

        private static Color GetKmlLineColor(Placemark pm)
        {
            return Color.FromArgb(255, Color.Red);
        }

        private static Color GetKmlPolyColor(Placemark pm)
        {
            // Without resolving shared styles, default color
            return Color.FromArgb(120, Color.Lime);
        }
    }
}