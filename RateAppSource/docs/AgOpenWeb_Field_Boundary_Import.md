# AgOpenWeb Field Boundary Import (planned)

Status: **planned, not started.** Noted 2026-10-31.

## Background

AgOpenWeb (AgOpenGPS-Official/AgOpenWeb) no longer keeps AgOpenGPS field files. On
open it imports `Field.txt`, `Boundary.txt` etc. once, deletes them, and stores the field
as a single `field.geojson` (plus sidecars). Rate Control's map boundary import
(`frmMap.ImportKml` → `BoundaryKmlExtractor`) only reads AOG `Field.kml`,
AOG `Boundary.txt` + `Field.txt`, or a generic KML, so with AgOpenWeb there is nothing
for it to import.

In AgOpenWeb issue #508 (rate controller compatibility, PR #509) the maintainers offered:

1. Rate Control reads `field.geojson` directly (their preference: it keeps working when
   a boundary is edited).
2. AgOpenWeb exports a plain KML per field (must be re-exported after every edit).

**Decision: option 1, to be implemented in Rate Control at a later date.** It is
independent of #508/#509: Rate Control needs no boundary at runtime. The boundary is
only used, optionally, for map display and for clipping productivity zones.

## Spec

AgOpenWeb `Docs/FIELD_FILE_FORMATS.md`, section 3, "Reading the boundary". Where the
spec and the AgOpenWeb code disagree, the code is authoritative.

- Location: `Documents/AgOpenWeb/Fields/<field name>/field.geojson` (default).
- One GeoJSON `FeatureCollection`, UTF-8. Each feature has a `role` property.
- Read only:
  - `"role": "outer-boundary"`: 0 or 1 feature, a Polygon with one closed ring.
  - `"role": "inner-boundary"`: 0 or more features, holes (`isDriveThrough` flags
    ones the tractor may cross; still holes for rate purposes).
- Coordinates are WGS84 `[longitude, latitude, heading]`. **The third value is a heading
  in radians, not an altitude.** Drop it (and don't write it into KML as altitude).
  No projection to undo.
- Ignore all other roles and properties (`metadata`, `headland`, `track`, `flag`, …).
- No outer boundary → tell the user the field has no boundary; import nothing.

## Implementation plan

1. `BoundaryKmlExtractor`: add `ConvertAgOpenWebGeoJson(string geojsonPath)` that returns
   a KML string. Parse with Newtonsoft.Json (already referenced), take the outer ring
   plus the inner rings, format coordinates as `lon,lat` with InvariantCulture, and build
   the output with the existing `BuildBoundaryKml(outerCoords, innerCoordsList)`. That way
   `LoadBoundaryPolygon` (NTS polygon for zone clipping) works unchanged.
2. `frmMap.ImportKml`: add the filter entry
   `"|AgOpenWeb Field (field.geojson)|field.geojson"` with a matching `case`, save as
   `boundary.kml` through `ImportProcessedKml`, the same as the `Boundary.txt` path.
   Mention the AgOpenWeb fields folder in the dialog title.
3. Optional later step (the reason the maintainers prefer option 1): remember the source
   `field.geojson` path per field and re-import when its timestamp is newer, so boundary
   edits in AgOpenWeb carry over without a manual re-import.
4. Help page / manual: add the new import source on the frmMap help page.

## Testing

- A field with an outer boundary only, and one with inner boundaries: check the map
  outline and the holes, and that zone generation clips correctly.
- A field with no `outer-boundary`: a clear message, nothing imported.
- Non-English culture (e.g. hu/de): coordinates are written with `.` (InvariantCulture).
- Sample file: the example in the spec, or any field saved by AgOpenWeb.
