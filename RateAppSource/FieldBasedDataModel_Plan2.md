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
│       ├── Elevation/     ← one CSV per survey
│       └── Kml/           ← field boundary KML files
├── Jobs/
│   └── Job_{id}/
│       ├── JobData.txt    ← FieldID required (>= 0)
│       └── RateData.csv   ← per-job, unchanged
└── Application/
    └── FieldNames.txt     ← Parcel list, gains ActivePrescription + ActiveElevation fields
```

---

## Key Design Decisions

1. **Jobs require a field.** `FieldID = -1` is invalid. `IsJobValid()` rejects jobs without a field.

2. **No default job on first run.** `CheckDefaultJob()` is removed or disabled. On first launch with no jobs, the app opens `frmMenuJobs` and prompts the user to create a job (which requires naming a field).

3. **Existing jobs without fields.** On `JobManager.Initialize()`, scan for jobs with `FieldID < 0`. If any found, show a one-time migration prompt requiring the user to assign them to a field before continuing.

4. **KML files are field-scoped.** All KML files copy to `{FieldFolder}/Kml/` instead of `{JobFolder}/Map/`. On job load, all `*.kml` files in the field's Kml folder are loaded automatically — no `KmlJobFiles` prop needed for field jobs.

5. **Prescription save-as-new.** Saving zones prompts for a name → saves to `{FieldFolder}/Maps/{name}.shp` → optionally sets as active. The active prescription is what `JobManager.MapPath()` returns.

6. **Yield and elevation are independent files.** Multiple yield CSVs per field (one per harvest year/machine). One active elevation CSV per field. Selected via dropdowns in `frmMap` tabFiles panel.

---

## File-by-File Changes

### Step 1 — `ParcelManager.cs`
- Add to `Parcel` class: `ActivePrescription` (string), `ActiveElevation` (string)
- Add static methods:
  - `FieldFolder(int id)` — `{DefaultDir}/Fields/Field_{id}`
  - `MapsFolder(int id)`, `YieldFolder(int id)`, `ElevationFolder(int id)`, `KmlFolder(int id)`
  - `ActiveMapPath(int id)` — full path to active prescription shp, or null
  - `ActiveElevationPath(int id)` — full path to active elevation csv, or null
  - `GetPrescriptionFiles(int id)`, `GetYieldFiles(int id)`, `GetElevationFiles(int id)`
  - `EnsureFieldFolders(int id)` — creates all four subfolders
  - `SetActivePrescription(int id, string filename)`
  - `SetActiveElevation(int id, string filename)`

### Step 2 — `JobManager.cs`
- `MapPath(int jobId)` → returns `ParcelManager.ActiveMapPath(job.FieldID)` (or fallback shp if no active set yet)
- `CheckFolderStructure(Job)` → add `ParcelManager.EnsureFieldFolders(job.FieldID)` call
- `IsJobValid()` → add `job.FieldID >= 0` as validity condition
- `CheckDefaultJob()` → remove auto-creation of default job
- `HasFieldlessJobs` property → true if any job has FieldID < 0

### Step 3 — `FieldDataManager.cs` (new static class in Classes/)
- `SelectedYieldPath` — currently selected yield file full path
- `SelectedElevationPath` — currently selected elevation file full path (null = use yield sample elevations)
- `SelectionChanged` event — fired when selections change or job changes
- `Initialize()` — wire to `JobManager.JobChanged`
- `LoadForCurrentJob()` — sets defaults from field folder (first yield file, active elevation)
- `SetYieldPath(string)`, `SetElevationPath(string)`

### Step 4 — `MapController.cs`
- `Initialize()` → call `FieldDataManager.Initialize()` after wiring events
- `LoadMap()` → set elevation data source before `ElevationCreator.Build()`:
  - If `FieldDataManager.SelectedElevationPath` set → `ElevationCreator.LoadElevationFile(path)`
  - Else → `ElevationCreator.SetDataFromYieldSamples(YieldCreator.FieldData)`
- `AddKmlLayer()` / `PersistKmlToJob()` → copy to `KmlFolder(fieldID)` instead of job Map dir
- `ReloadJobKmls()` → scan `KmlFolder(fieldID)` for `*.kml` files; no prop list needed
- `DeleteKmlLayer()` → delete from `KmlFolder(fieldID)`
- Add `SavePrescription(string filePath)` — saves target zones to named path, optionally sets active

### Step 5 — `YieldOverlayCreator.cs`
- `LoadData(string filePath = null)` — uses `filePath ?? FieldDataManager.SelectedYieldPath ?? JobManager.CurrentYieldDataPath`
- `JobManager_JobChanged` and `Core_ProfileChanged` call `LoadData()` with no argument

### Step 6 — `ElevationOverlayCreator.cs`
- Remove direct reference to `MapController.YieldCreator` inside `Build()`
- Add `SetDataFromYieldSamples(List<FieldSample> samples)` — sets Readings, sets UseSimulatedData = (count == 0)
- Add `LoadElevationFile(string filePath)` — parses simple Lat,Lon,Elevation CSV into FieldSample list

### Step 7 — `frmMap.Designer.cs` + `frmMap.cs`
- Add a new **Field** tab as the first tab in the tab control
- Field tab contains:
  - Field name label
  - Prescription: dropdown + Save As New button
  - Yield record: dropdown + Import button
  - Elevation record: dropdown + Import button
- Add `UpdateFieldDataPanel()` called from `UpdateForm()` and `FieldDataManager.SelectionChanged`
- Yield import: confirm overwrite if filename exists in YieldFolder; copy to `YieldFolder`; refresh dropdown
- Elevation import: confirm overwrite; copy to `ElevationFolder`; refresh dropdown

### Step 8 — `frmMenuJobs.cs`
- `btnOK_Click` → reject save if `cbField.Text.Trim()` is empty (show message)
- First-run behaviour: if `JobManager.GetJobsList()` is empty, immediately put form into new-job mode
- Migration warning: if any jobs have `FieldID < 0`, show prompt to assign fields

---

## Branch
Implementation branch: `FieldBased`

## Status
Design complete 2026-03-30. Ready to implement — awaiting step-by-step approval.
