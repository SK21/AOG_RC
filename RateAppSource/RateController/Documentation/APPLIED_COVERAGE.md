# Applied coverage rendering: architecture and flow
Date Created: 2025-11-11

This document explains how the “applied areas” (coverage) are created and rendered on the rate map.

## Overview
- Readings are collected continuously as GPS and rate data arrive.
- A coverage trail is built from those readings into a GMap overlay (`AppliedOverlay`).
- The overlay is updated either “live” (incremental) or rebuilt from “history” (all readings).
- Colors and legend are scaled from the selected rate series (Applied or Target) and product index.

Key classes:
- `MapManager` orchestrates visibility, timers, overlays, and calls into the service.
- `DataCollector` records `RateReading` samples (time, lat/lon, applied[], target[]).
- `RateOverlayService` converts readings into polygons using `CoverageTrail`.
- `CoverageTrail` builds and draws the swaths/polygons onto the overlay.
- `LegendManager` shows the color scale legend.

## Data collection
- `MapManager.SetTractorPosition()` is called on GPS ticks with arrays for `AppliedRates` and `TargetRates`.
- It forwards those to the data pipeline:
  - `mf.Tls.RateCollector.RecordReading(lat, lng, applied[], target[])` writes to `DataCollector`.
- `DataCollector`:
  - Throttles to 500 ms (`RecordIntervalMS`) via a timer and `ReadyForNewData`.
  - Ensures the tractor has moved enough (0.00001 deg) before adding a `RateReading`.
  - Periodically persists appended rows to CSV (30 sec cadence) with both Applied and Target columns.

`RateReading` carries:
- `Timestamp`, `Latitude`, `Longitude`
- `AppliedRates[]` and `TargetRates[]` (same length, up to 5 entries)

## Display pipeline
Entry points in `MapManager`:
- `ShowAppliedRatesOverlay()` toggles the overlay:
  - Creates/clears `AppliedOverlay` and subscribes a refresh timer.
  - Calls `ShowAppliedLayer()` immediately.
- `AppliedOverlayTimer_Tick` periodically calls `ShowAppliedLayer()` (default every 2 seconds).
- Immediate live refreshes are throttled by `MinAppliedOverlayUpdate` (400 ms) to reduce redraws.

### Live vs. history
- Live path (preferred while driving):
  - `RateOverlayService.UpdateRatesOverlayLive(...)`
    - Uses the selected series (`Props.RateDisplayType` = Applied or Target) for scale and legend.
    - For Applied view only, the latest applied rate from the current GPS tick (`cLastAppliedRates`) can override the last reading so the trail updates without timer delay.
    - Applies a distance gate (<= 5 m) to prevent “bridging” if the tractor has jumped or been relocated.
    - Calls `CoverageTrail.AddPoint(...)` or `Break()` accordingly and then draws onto the `AppliedOverlay`.

- History path (fallback and when mouse sets position):
  - `RateOverlayService.BuildFromHistory(...)`
    - Iterates all readings, uses the selected series (Applied or Target) to decide whether a segment is “painted.”
    - Computes heading between consecutive points.
    - Builds the entire trail then draws it onto the overlay.

Conditions:
- History path is used when `cMouseSetTractorPosition` or `cSuppressTrailUntilNextGps` is true, or when live failed (e.g., no polygons yet).

## Geometry generation
- Implement width (swath width):
  - Derived from `mf.Sections.TotalWidth(false)`, defaulting to 24.0 if not available.
  - Treated as meters by the trail generator.
- `CoverageTrail`:
  - `AddPoint(position, heading, value, implementWidthMeters)` grows a swath polygon centered on the path.
  - `Break()` starts a new segment to avoid connecting non-adjacent passes or zero-rate gaps.
  - `DrawTrail(overlay, minRate, maxRate)` finalizes polygons and pushes them into `GMapOverlay.Polygons`.
  - `CreateLegend(minRate, maxRate, steps)` returns a textual legend map used by `LegendManager`.
- Polygons are stroke-less (transparent pen) for clean AOG-style fills. Color is chosen from a scale based on rate.

### Segment length and RecordIntervalMS (sampling interval)
- Width is independent of `RecordIntervalMS` and comes solely from `implementWidthMeters`.
- Length of each polygon segment is the ground distance between consecutive recorded samples:
  - distance ≈ speed(m/s) × (`RecordIntervalMS` / 1000).
  - Default `RecordIntervalMS` is 500 ms (see `DataCollector`), so at 10 km/h (~2.78 m/s) segment length ≈ 1.39 m.
- Therefore, `RecordIntervalMS` affects visual granularity (how long each swath slice is), not the swath width.
- Very small movements: if the computed distance is < 0.01 m, `CoverageTrail` synthesizes a tiny forward movement of `max(implementWidth × 0.02, 0.02 m)` to ensure a drawable polygon.
- UI throttle vs. sampling: `MinAppliedOverlayUpdate` (400 ms) only throttles on-screen redraws; it does not change the sampling/logging interval or segment geometry.

## Scaling and legend
- Rate type and product index come from:
  - `Props.RateDisplayType` (Applied or Target)
  - `Props.RateDisplayProduct` (0-based)
- Scale range is found via robust percentiles:
  - `TryComputeScale` uses the 2nd–98th percentile of non-zero values in the selected series.
  - If range collapses, a fallback pad is applied.
- `LegendManager` renders the legend using the returned dictionary of labels-to-colors.

## Visibility and performance
- All work is gated by `Props.RateMapIsVisible()` to avoid off-screen CPU usage.
- Immediate updates are throttled by `MinAppliedOverlayUpdate` (400 ms).
- Timer-based refresh interval is `MapRefreshSeconds` (2 s).
- Mouse clicks (left) can set the tractor position when not in polygon edit; `cSuppressTrailUntilNextGps` prevents drawing coverage from that artificial position until a real GPS arrives.

## Export
- `MapManager.SaveMapToFile(...)`:
  - If user-defined zones exist, saves those.
  - Else rebuilds the current overlay from history using the selected `Props.RateDisplayType` and saves the overlay polygons via `ShapefileHelper.SaveOverlayPolygons`.
  - Produces standard Shapefile artifacts.

## Centering and overlays
- On load, `ShowAppliedRatesOverlay()` is invoked, then `CenterMap()`:
  - `CenterOnAppliedDataIfAvailable()` inspects readings for any non-zero Applied values to form a bounding rect and sets the map to fit. If none, it falls back to zone extents.
- Overlays:
  - `AppliedOverlay` holds coverage polygons.
  - `LegendManager` is kept on top to ensure the legend is visible.

## Configuration controls
- Rate type: `Props.RateDisplayType`:
  - Applied → uses applied rates for both trail and legend.
  - Target → uses target rates for both trail and legend.
- Product index: `Props.RateDisplayProduct`.
- Toggling rate data options should call `Props.RaiseRateDataSettingsChanged()` to notify listeners (MapManager refresh).

## Edge cases and safeguards
- Zeros and near-zeros don’t produce coverage (`RateEpsilon = 1e-3`).
- Mouse-set positions suspend live trail until the next GPS update.
- The distance gate in live mode avoids connecting remote trail segments.
- If readings are absent, the overlay is cleared and the legend is reset.