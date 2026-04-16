using NetTopologySuite.Geometries;
using RateController.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace RateController.RateMap
{
    /// <summary>
    /// Extracts field boundary polygons from AOG Field.kml files, AOG Boundary.txt files,
    /// and generic KML files. Provides both KML string output (for import/display) and
    /// NTS Polygon output (for zone generation clipping).
    /// </summary>
    public static class BoundaryKmlExtractor
    {
        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>
        /// Extracts the boundary polygon from an AOG Field.kml (Boundaries folder only).
        /// Returns a clean single-polygon KML string, or null on failure.
        /// </summary>
        public static string ExtractAogBoundaryPolygon(string fieldKmlPath)
        {
            try
            {
                var doc = XDocument.Load(fieldKmlPath);
                var ns  = XNamespace.Get("http://www.opengis.net/kml/2.2");

                var folder = doc.Descendants(ns + "Folder")
                    .FirstOrDefault(f => (string)f.Element(ns + "name") == "Boundaries");
                if (folder == null)
                {
                    Props.ShowMessage("No 'Boundaries' folder found in this KML file.", "Import Boundary", 5000, false);
                    return null;
                }

                var outerEl = folder.Descendants(ns + "outerBoundaryIs")
                    .FirstOrDefault()
                    ?.Descendants(ns + "coordinates")
                    .FirstOrDefault();
                if (outerEl == null)
                {
                    Props.ShowMessage("No boundary polygon found in KML file.", "Import Boundary", 5000, false);
                    return null;
                }

                var innerCoords = folder.Descendants(ns + "innerBoundaryIs")
                    .Select(ib => ib.Descendants(ns + "coordinates").FirstOrDefault()?.Value.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList();

                return BuildBoundaryKml(outerEl.Value.Trim(), innerCoords);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("BoundaryKmlExtractor/ExtractAogBoundaryPolygon: " + ex.Message);
                Props.ShowMessage("Could not read KML file: " + ex.Message, "Import Boundary", 5000, false);
                return null;
            }
        }

        /// <summary>
        /// Reads AOG Boundary.txt + Field.txt from the same folder,
        /// converts local easting/northing to lat/lon, returns a KML string.
        /// </summary>
        public static string ConvertAogBoundaryTxt(string boundaryTxtPath)
        {
            try
            {
                string dir          = Path.GetDirectoryName(boundaryTxtPath);
                string fieldTxtPath = Path.Combine(dir, "Field.txt");

                if (!File.Exists(fieldTxtPath))
                {
                    Props.ShowMessage(
                        "Field.txt not found in the same folder — required for coordinate conversion.",
                        "Import Boundary", 5000, false);
                    return null;
                }

                // Read origin (StartFix) from Field.txt
                double lat0 = 0, lon0 = 0;
                bool   foundOrigin = false;
                string[] fieldLines = File.ReadAllLines(fieldTxtPath);
                for (int i = 0; i < fieldLines.Length - 1; i++)
                {
                    if (fieldLines[i].Trim() != "StartFix") continue;
                    string[] parts = fieldLines[i + 1].Split(',');
                    if (parts.Length >= 2 &&
                        double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out lat0) &&
                        double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out lon0))
                    { foundOrigin = true; break; }
                }

                if (!foundOrigin)
                {
                    Props.ShowMessage("StartFix not found in Field.txt.", "Import Boundary", 5000, false);
                    return null;
                }

                // AOG GeoConverter conversion constants (same formula as AOG source)
                double lat0Rad    = lat0 * Math.PI / 180.0;
                double mPerDegLat = 111132.92 - 559.82 * Math.Cos(2 * lat0Rad)
                                  + 1.175 * Math.Cos(4 * lat0Rad) - 0.0023 * Math.Cos(6 * lat0Rad);
                double mPerDegLon = 111412.84 * Math.Cos(lat0Rad)
                                  - 93.5 * Math.Cos(3 * lat0Rad) + 0.118 * Math.Cos(5 * lat0Rad);

                // Parse Boundary.txt — skip header, skip isDriveThru flag(s)
                string[] lines = File.ReadAllLines(boundaryTxtPath);
                int      idx   = 0;
                if (idx < lines.Length && lines[idx].TrimStart().StartsWith("$")) idx++;
                while (idx < lines.Length &&
                       (lines[idx].Trim() == "True" || lines[idx].Trim() == "False")) idx++;

                if (idx >= lines.Length ||
                    !int.TryParse(lines[idx].Trim(), NumberStyles.Integer,
                        CultureInfo.InvariantCulture, out int count) || count < 3)
                {
                    Props.ShowMessage("Boundary.txt contains no usable points.", "Import Boundary", 5000, false);
                    return null;
                }
                idx++;

                var coords = new StringBuilder();
                for (int i = 0; i < count && idx < lines.Length; i++, idx++)
                {
                    string[] p = lines[idx].Split(',');
                    if (p.Length < 2) continue;
                    if (!double.TryParse(p[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double easting))  continue;
                    if (!double.TryParse(p[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double northing)) continue;

                    double lat = lat0 + northing / mPerDegLat;
                    double lon = lon0 + easting  / mPerDegLon;

                    if (coords.Length > 0) coords.Append(' ');
                    coords.Append(lon.ToString("F7", CultureInfo.InvariantCulture));
                    coords.Append(',');
                    coords.Append(lat.ToString("F7", CultureInfo.InvariantCulture));
                    coords.Append(",0");
                }

                if (coords.Length == 0)
                {
                    Props.ShowMessage("No valid coordinates found in Boundary.txt.", "Import Boundary", 5000, false);
                    return null;
                }

                return BuildBoundaryKml(coords.ToString(), new List<string>());
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("BoundaryKmlExtractor/ConvertAogBoundaryTxt: " + ex.Message);
                Props.ShowMessage("Could not read Boundary.txt: " + ex.Message, "Import Boundary", 5000, false);
                return null;
            }
        }

        /// <summary>
        /// Loads a clean boundary KML file (as written by BuildBoundaryKml) and returns
        /// an NTS Polygon for use in zone generation clipping. Returns null if the file
        /// is absent, empty, or unparseable.
        /// </summary>
        public static Polygon LoadBoundaryPolygon(string kmlPath)
        {
            try
            {
                if (string.IsNullOrEmpty(kmlPath) || !File.Exists(kmlPath)) return null;

                var doc = XDocument.Load(kmlPath);
                var ns  = XNamespace.Get("http://www.opengis.net/kml/2.2");

                var polyEl = doc.Descendants(ns + "Polygon").FirstOrDefault();
                if (polyEl == null) return null;

                var outerCoords = ParseKmlCoordinates(
                    polyEl.Descendants(ns + "outerBoundaryIs")
                          .FirstOrDefault()
                          ?.Descendants(ns + "coordinates")
                          .FirstOrDefault()?.Value);
                if (outerCoords == null || outerCoords.Length < 3) return null;

                var factory   = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(4326);
                var outerRing = factory.CreateLinearRing(CloseRing(outerCoords));

                var holes = polyEl.Descendants(ns + "innerBoundaryIs")
                    .Select(ib => ib.Descendants(ns + "coordinates").FirstOrDefault()?.Value)
                    .Where(v => !string.IsNullOrEmpty(v))
                    .Select(v => ParseKmlCoordinates(v))
                    .Where(c => c != null && c.Length >= 3)
                    .Select(c => factory.CreateLinearRing(CloseRing(c)))
                    .ToArray();

                return factory.CreatePolygon(outerRing, holes);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("BoundaryKmlExtractor/LoadBoundaryPolygon: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Builds a minimal KML string containing a single boundary polygon.
        /// </summary>
        public static string BuildBoundaryKml(string outerCoords, IList<string> innerCoordsList)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<kml xmlns=\"http://www.opengis.net/kml/2.2\">");
            sb.AppendLine("  <Document><Placemark><name>Boundary</name>");
            sb.AppendLine("    <Style>");
            sb.AppendLine("      <LineStyle><color>ffdd00dd</color><width>3</width></LineStyle>");
            sb.AppendLine("      <PolyStyle><color>407f3f55</color></PolyStyle>");
            sb.AppendLine("    </Style>");
            sb.AppendLine("    <Polygon><tessellate>1</tessellate>");
            sb.AppendLine("      <outerBoundaryIs><LinearRing><coordinates>");
            sb.AppendLine("        " + outerCoords);
            sb.AppendLine("      </coordinates></LinearRing></outerBoundaryIs>");
            foreach (string inner in innerCoordsList)
            {
                sb.AppendLine("      <innerBoundaryIs><LinearRing><coordinates>");
                sb.AppendLine("        " + inner);
                sb.AppendLine("      </coordinates></LinearRing></innerBoundaryIs>");
            }
            sb.AppendLine("    </Polygon></Placemark></Document></kml>");
            return sb.ToString();
        }

        // ── Private helpers ───────────────────────────────────────────────────

        // Parses a KML coordinate string ("lon,lat,alt lon,lat,alt ...") into NTS Coordinates.
        private static Coordinate[] ParseKmlCoordinates(string coordText)
        {
            if (string.IsNullOrWhiteSpace(coordText)) return null;
            var list = new List<Coordinate>();
            foreach (var token in coordText.Split(new[] { ' ', '\n', '\r', '\t' },
                         StringSplitOptions.RemoveEmptyEntries))
            {
                string[] parts = token.Split(',');
                if (parts.Length < 2) continue;
                if (!double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double lon)) continue;
                if (!double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double lat)) continue;
                list.Add(new Coordinate(lon, lat));
            }
            return list.Count >= 3 ? list.ToArray() : null;
        }

        // Ensures the coordinate array forms a closed ring (first point == last point).
        private static Coordinate[] CloseRing(Coordinate[] coords)
        {
            if (coords[0].Equals2D(coords[coords.Length - 1])) return coords;
            var closed = new Coordinate[coords.Length + 1];
            Array.Copy(coords, closed, coords.Length);
            closed[coords.Length] = new Coordinate(coords[0].X, coords[0].Y);
            return closed;
        }
    }
}
