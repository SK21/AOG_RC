# Field-Based Data Model — Plan 2 Implementation Design

Every job must be assigned to a field (Parcel). No job without a field. This simplifies all path logic — no fallback branches needed.

**Why:** Field data (prescription maps, yield history, elevation, KML boundary) is field-scoped, not job-scoped. A job without a field is meaningless for rate control operations.

---

## Folder Structure

```
{Documents}/RateController/
├── Fields/
│   └── Field_{id}/
│       ├── Maps/          ← named prescription shapefiles
│       ├── Yield/         ← one CSV per harvest/year
│       ├── Elevation/     ← single file: Elevation.csv
│       └── Kml/           ← field boundary KML files
├── Jobs/
│   └── Job_{id}/
│       ├── JobData.txt    ← FieldID required (>= 0)
│       └── RateData.csv   ← per-job, unchanged
└── Application/
    └── FieldNames.txt     ← Parcel list, gains ActivePrescription field (ActiveElevation removed — single fixed file)
```

---

## Key Design Decisions

1. **Jobs require a field.** `FieldID = -1` is invalid. `IsJobValid()` rejects jobs without a field.

2. **No default job on first run.** `CheckDefaultJob()` is removed or disabled. On first launch with no jobs, the app opens `frmMenuJobs` and prompts the user to create a job (which requires naming a field).

3. **Existing jobs without fields.** On `JobManager.Initialize()`, scan for jobs with `FieldID < 0`. If any found, show a one-time migration prompt requiring the user to assign them to a field before continuing.

4. **KML files are field-scoped.** All KML files copy to `{FieldFolder}/Kml/` instead of `{JobFolder}/Map/`. On job load, all `*.kml` files in the field's Kml folder are loaded automatically — no `KmlJobFiles` prop needed for field jobs.

5. **Prescription save-as-new.** Saving zones prompts for a name → saves to `{FieldFolder}/Maps/{name}.shp` → optionally sets as active. The active prescription is what `JobManager.MapPath()` returns.

6. **Yield and elevation are independent files.** Multiple yield CSVs per field (one per harvest year/machine). One elevation CSV per field (`Elevation.csv`) — no selection needed, it either exists or it doesn't. Elevation data is extracted from a yield import if the user opts in.

---

## File-by-File Changes

### Step 1 — `ParcelManager.cs`
- Add to `Parcel` class: `ActivePrescription` (string)
- Add static methods:
  - `FieldFolder(int id)` — `{DefaultDir}/Fields/Field_{id}`
  - `MapsFolder(int id)`, `YieldFolder(int id)`, `ElevationFolder(int id)`, `KmlFolder(int id)`
  - `ActiveMapPath(int id)` — full path to active prescription shp, or null
  - `ElevationPath(int id)` — `{ElevationFolder}/Elevation.csv` if exists, else null
  - `GetPrescriptionFiles(int id)`, `GetYieldFiles(int id)`
  - `EnsureFieldFolders(int id)` — creates all four subfolders
  - `SetActivePrescription(int id, string filename)`

### Step 2 — `JobManager.cs`
- `MapPath(int jobId)` → returns `ParcelManager.ActiveMapPath(job.FieldID)` (or fallback shp if no active set yet)
- `CheckFolderStructure(Job)` → add `ParcelManager.EnsureFieldFolders(job.FieldID)` call
- `IsJobValid()` → add `job.FieldID >= 0` as validity condition
- `CheckDefaultJob()` → remove auto-creation of default job
- `HasFieldlessJobs` property → true if any job has FieldID < 0

### Step 3 — `FieldDataManager.cs` (new static class in Classes/)
- `SelectedYieldPath` — currently selected yield file full path
- `ElevationPath` — `ParcelManager.ElevationPath(job.FieldID)` (fixed filename, no user selection)
- `SelectionChanged` event — fired when selections change or job changes
- `Initialize()` — wire to `JobManager.JobChanged`
- `LoadForCurrentJob()` — sets defaults from field folder (first yield file; elevation path derived from fixed filename)
- `SetYieldPath(string)`

### Step 4 — `MapController.cs`
- `Initialize()` → call `FieldDataManager.Initialize()` after wiring events
- `LoadMap()` → call `ElevationCreator.LoadElevationFile(FieldDataManager.ElevationPath)` before `ElevationCreator.Build()` (path may be null — LoadElevationFile handles that gracefully)
- `AddKmlLayer()` / `PersistKmlToJob()` → copy to `KmlFolder(fieldID)` instead of job Map dir
- `ReloadJobKmls()` → scan `KmlFolder(fieldID)` for `*.kml` files; no prop list needed
- `DeleteKmlLayer()` → delete from `KmlFolder(fieldID)`
- Add `SavePrescription(string filePath)` — saves target zones to named path, optionally sets active

### Step 5 — `YieldOverlayCreator.cs`
- `LoadData(string filePath = null)` — uses `filePath ?? FieldDataManager.SelectedYieldPath ?? JobManager.CurrentYieldDataPath`
- `JobManager_JobChanged` and `Core_ProfileChanged` call `LoadData()` with no argument

### Step 6 — `ElevationOverlayCreator.cs`
- Remove direct reference to `MapController.YieldCreator` inside `Build()`; fall back to `YieldCreator.FieldData` until Step 7 provides a real elevation file
- Remove `UseSimulatedData` field and `ApplySimulatedElevations()` call from `Build()` (keep method for future testing use)
- Add `LoadElevationFile(string filePath)` — parses `Lat,Lon,Elevation` CSV into FieldSample list; sets Readings; null/missing path → clears Readings and falls back to YieldCreator in Build()

### Step 7 — `frmMap.Designer.cs` + `frmMap.cs`
- Add a new **Field** tab as the first tab in the tab control
- Field tab contains:
  - Field name label
  - Prescription: dropdown + Save As New button
  - Yield record: dropdown + Import button
  - Elevation record: label (filename or "None") + Import button + Delete button
- Add `UpdateFieldDataPanel()` called from `UpdateForm()` and `FieldDataManager.SelectionChanged`
- Yield import: confirm overwrite if filename exists in YieldFolder; copy to `YieldFolder`; refresh dropdown; if yield CSV contains non-zero ElevationMeters values, ask user if they want to extract elevation data to `Elevation.csv`; if yes, back up existing `Elevation.csv` → `Elevation.bak` before writing
- Elevation import: copy a standalone `Lat,Lon,Elevation` CSV to `ElevationFolder/Elevation.csv`; if `Elevation.csv` already exists, back it up to `Elevation.bak` first; refresh label
- Elevation delete: delete `Elevation.csv`; refresh label
- Elevation restore: if `Elevation.bak` exists, rename it to `Elevation.csv`; refresh label; Restore button only visible when `Elevation.bak` exists
- Backup rule: any operation that overwrites `Elevation.csv` (import or yield extraction) first renames existing `Elevation.csv` → `Elevation.bak`; previous `Elevation.bak` is overwritten (one level deep)

### Step 8 — `frmMenuJobs.cs`
- `btnOK_Click` → reject save if `cbField.Text.Trim()` is empty (show message)
- First-run behaviour: if `JobManager.GetJobsList()` is empty, immediately put form into new-job mode
- Migration warning: if any jobs have `FieldID < 0`, show prompt to assign fields

---

## Branch
Implementation branch: `FieldBased`

## Status
All steps complete (2026-03-31).
- Steps 1–5: completed in prior session.
- Step 7 (frmMap Field tab): completed before Step 6; tab order Zones → Field → Data → Files → VR.
- Step 6 (ElevationOverlayCreator): `LoadElevationFile()` added; `Build()` uses Readings only — no yield fallback. `UseSimulatedData` retained for testing.
- Step 8 (frmMenuJobs): field required on save; first-run new-job mode; migration warning for fieldless jobs.
- Step 26 dropped: `UseSimulatedData` kept as-is (false in production).
Plan updated 2026-03-31: single elevation file (`Elevation.csv`), no ActiveElevation on Parcel, elevation extracted optionally from yield import.
