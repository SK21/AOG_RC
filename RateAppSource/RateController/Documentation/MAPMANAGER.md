# MapManager: responsibilities, APIs, and workflows
Date Created: 2025-11-11

This document describes the `MapManager` class: its role, public API, internal behavior, and how it coordinates overlays, zones, GPS updates, and coverage rendering.

See also:
- `APPLIED_COVERAGE.md` for coverage (applied/target) generation and rendering details.
- `RateOverlayService`, `CoverageTrail`, `LegendManager`, `ShapefileHelper`, and `MapZone` for collaborating components.

## Purpose and scope
`MapManager` owns the GMap control and orchestrates:
- Base map provider and tile display (satellite or empty).
- Overlays: GPS marker, zones, temporary markers, and “applied coverage.”
- Coverage rebuild and live incremental updates via `RateOverlayService`.
- Legend layout and updates via `LegendManager`.
- Spatial queries for “target rate” lookup using a zone spatial index (STRtree).
- User interactions: mouse click/drag, polygon edit mode, zoom-to-fit, and centering.

All map work is gated by `Props.RateMapIsVisible()` to avoid off-screen CPU usage.

## Key collaborators
- GMap.NET:
  - `GMapControl`, `GMapOverlay`, `GMapPolygon`, `GMarkerGoogle`.
- NetTopologySuite (NTS):
  - `Geometry`, `Polygon`, and `STRtree<T>` for fast zone lookups.
- Project services:
  - `RateOverlayService`: builds/draws coverage polygons (live or history).
  - `LegendManager`: draws and positions the coverage legend.
  - `ShapefileHelper`: loads/saves zones and exports overlay polygons.
  - `MapZone`: holds polygon geometry, rates, color, and helpers.
  - `Props`: configuration, flags, and event sources.
  - `FormStart mf`: provides `Tls.RateCollector` and `Sections.TotalWidth`.

## Life cycle and initialization
- Constructed with `FormStart` (`mf`).
- Initializes the `GMapControl`, overlays, GPS marker, and `LegendManager`.
- Subscribes to:
  - `Props.JobChanged` → reloads zones.
  - `Props.RateDataSettingsChanged` → toggles coverage overlay.
  - GMap mouse events and zoom changed events.
- Loads zones, then calls `ShowAppliedRatesOverlay()` and `CenterMap()`.

Disposal unsubscribes handlers, clears overlays, and disposes the `LegendManager`.

## Overlays
- `gpsMarkerOverlay`: shows the tractor marker.
- `zoneOverlay`: shows user-defined zones (`MapZone -> GMapPolygon`).
- `tempMarkerOverlay`: assists polygon editing (vertex markers).
- `AppliedOverlay`: holds coverage polygons from `RateOverlayService`.

All zone/coverage polygons use a transparent stroke for an AOG-style filled appearance.

## Coverage rendering overview
- Timer-driven refresh every 2 seconds (config constant: `MapRefreshSeconds = 2`).
- Immediate “live” updates are throttled to at least every 400 ms (`MinAppliedOverlayUpdate`) to reduce UI redraws.
- Live vs. history paths:
  - Live: `RateOverlayService.UpdateRatesOverlayLive(...)` using the current GPS position/heading and the most recent applied value for the selected product.
  - History: `RateOverlayService.BuildFromHistory(...)` rebuilds from all readings.
- When the mouse sets the tractor position or the trail is suppressed until next GPS, the history path is used.
- Detailed geometry and scaling rules are documented in `APPLIED_COVERAGE.md`.

## Public API

### Event
- `event EventHandler MapChanged`  
  Raised after actions that change map content or view (e.g., overlay updates, centering, loading).

### Properties
- `bool EditModePolygons`  
  Enables polygon edit mode. Left-click adds vertices; temporary markers show points.
- `GMapControl gmapObject`  
  Exposes the underlying `GMapControl` for hosting or advanced actions.
- `bool LegendOverlayEnabled`  
  Gets/sets visibility of the legend overlay. Delegates to `LegendManager`.
- `int LegendRightMarginPx`  
  Controls legend’s right-margin placement.
- `bool MouseSetTractorPosition`  
  Whether the last position was set by mouse. When true, live coverage is suspended until real GPS arrives.
- `bool ShowTiles`  
  Toggles satellite tiles versus empty provider. Also refreshes the map.
- `bool TilesGrayScale`  
  Applies grayscale to satellite tiles to reduce visual intensity.
- `Color ZoneColor`, `double ZoneHectares`, `string ZoneName`  
  Details of the zone currently under the tractor (derived by spatial query).
- `string ZoneName` setter truncates to 12 chars; defaults to “Unnamed Zone”.

### Methods
- `void CenterMap()`  
  Centers on applied data if any non-zero applied readings exist; otherwise `ZoomToFit()` zones.
- `void ClearAppliedRatesOverlay()`  
  Clears coverage polygons and legend, resets the overlay service.
- `bool DeleteZone(string name)`  
  Removes all polygons/zones matching the name, rebuilds the spatial index, refreshes, and saves.
- `void Dispose()`  
  Unsubscribes events, clears overlays, and disposes legend resources.
- `RectLatLng GetOverallRectLatLng()`  
  Returns a bounding rectangle that includes all zone polygons (or empty if none).
- `double GetRate(int rateId)`  
  Returns one of four zone target rates for the current zone (A–D).
- `bool LoadMap()`  
  Loads zones from the current map path, populates overlays, rebuilds spatial index, refreshes, and shows coverage.
- `bool SaveMap(bool updateCache = true)`  
  Saves current zones to disk and optionally prefetches tiles into cache.
- `void SaveMapToFile(string filePath)`  
  Exports either:
  - Zones (if any), or
  - Coverage polygons rebuilt from history (applied/target based on `Props.RateDisplayType`),
  to standard Shapefile artifacts. Performs cleanup if export fails.
- `void SetTractorPosition(PointLatLng pos, double[] applied, double[] target)`  
  Handles GPS ticks:
  - Updates position and heading.
  - Records a reading to `RateCollector` (always, so zeros are captured).
  - Caches the latest applied array for the selected product’s live update.
  - Triggers a throttled live coverage refresh if the map is visible.
  - Updates zone-based target rates and raises `MapChanged`.
- `Dictionary<string, Color> ShowAppliedLayer()`  
  Rebuilds coverage overlay (live or history as needed) and updates the legend.
- `void ShowAppliedRatesOverlay()`  
  Toggles visibility and periodic refresh of the coverage overlay.
- `void ShowZoneOverlay(bool show)`  
  Toggles zone overlay visibility.
- `void UpdateTargetRates()`  
  Locates the current zone under the tractor via the spatial index (fallback linear scan) and updates `ZoneName/ZoneColor/ZoneHectares` and the four product rates.
- `bool UpdateZone(string name, double r0, double r1, double r2, double r3, Color color)`  
  - If new polygon vertices exist: creates a new `MapZone`, adds to overlay and index.
  - Else: updates the zone under the tractor (name, rates, color), persists, and reloads.
- `void ZoomToFit()`  
  Fits the view to encompass all zone polygons.

## Event and timer interactions
- `Props.JobChanged` → `LoadMap()` (rebuild overlays and index; show coverage; center).
- `Props.RateDataSettingsChanged` → toggles coverage overlay, resets service, updates legend.
- `AppliedOverlayTimer` (default 2s) → `ShowAppliedLayer()` (unless a recent live update already refreshed).
- GMap events:
  - Mouse left-click:
    - In polygon edit: add vertex.
    - Otherwise: set tractor position and suppress live trail until next GPS arrives.
  - Mouse right-drag: pans the map.
  - Zoom changed: repositions legend; raises `MapChanged`.

## Zone and spatial index
- Zones load via `ShapefileHelper.CreateZoneList`.
- Rendered as `GMapPolygon` with transparent stroke.
- `STRtree<MapZone>` index is built from each zone’s envelope.
- `UpdateTargetRates()` queries the index around the tractor point; ties resolved by “last wins.”

## Coverage specifics and scaling
- Implement width in meters from `mf.Sections.TotalWidth(false)`, default 24.0.
- Series and product selection:
  - `Props.RateDisplayType` = Applied or Target.
  - `Props.RateDisplayProduct` = 0-based product index.
- Live update uses last GPS tick’s applied value for the active product to minimize delay.
- Rendering, geometry, sampling, and legend scaling are described in `APPLIED_COVERAGE.md`.

## Visibility, throttling, and performance
- Most work is skipped if `Props.RateMapIsVisible()` is false.
- Redraw throttle: immediate live coverage updates require at least 400 ms between refreshes.
- Timer refresh default: every 2 seconds.
- Map tile prefetch performed by `SaveMap(updateCache: true)` to seed cache at current view.

## Mouse interactions summary
- Left-click:
  - Edit mode on → add polygon vertex; show temp marker.
  - Edit mode off → set tractor position; suppress live trail until next GPS.
- Right-click + drag:
  - Pan view (simple lat/lng delta adjustment).

## Error handling and logging
- All critical path methods are wrapped in `try/catch` and log via `Props.WriteErrorLog(...)` or notify via `Props.ShowMessage(...)`.
- Overlay mutations are defensive against nulls and unexpected states.

## Extension points and gotchas
- When adding new overlays, ensure `EnsureLegendTop()` is called to keep the legend visible.
- If altering refresh intervals, keep `MinAppliedOverlayUpdate` > 0 to avoid UI flooding.
- When editing zones:
  - Ensure polygon rings are closed (first/last vertex match) before constructing NTS polygons.
  - After batch edits, call `BuildZoneIndex()` to keep `UpdateTargetRates()` accurate.
- When positioning the tractor with the mouse, coverage drawing is suspended until a true GPS tick (see `cSuppressTrailUntilNextGps`).
- Export behavior:
  - Zones take precedence; only when no zones exist will coverage be rebuilt from history and exported.

## Minimal usage patterns

- Toggle satellite tiles:
  - Set `ShowTiles = true|false`. Optionally set `TilesGrayScale = true` for subdued imagery.

- Show/hide coverage:
  - Update `Props.MapShowRates` and raise `Props.RateDataSettingsChanged`. `MapManager` will add/remove the `AppliedOverlay`, reset the service, and refresh the legend.

- Add a zone:
  - Set `EditModePolygons = true`, left-click to add vertices.
  - Call `UpdateZone(name, r0, r1, r2, r3, color)`.
  - `SaveMap()` persists to the current map path.

- Export:
  - Call `SaveMapToFile(filePath)`. Exports zones if present; otherwise exports coverage polygons rebuilt from history.
