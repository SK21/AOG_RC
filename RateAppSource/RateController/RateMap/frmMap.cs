using AgOpenGPS;
using GMap.NET;
using RateController.Classes;
using RateController.RateMap;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RateController.Forms
{
    public partial class frmMap : Form
    {
        private const double BASE_PAN_DISTANCE_MILES = 2;
        private bool EditInProgress = false;
        private bool Initializing = true;
        private bool IsShutDown = false;
        private int MainLeft = 0;
        private int MainTop = 0;
        private int MaxZoom = 10;
        private int MinViewLeft = 0;
        private int MinViewTop = 0;
        private int MinZoom = 10;
        private System.Drawing.Point MouseDownLocation;
        private bool UseMaxView = false;

        public frmMap()
        {
            InitializeComponent();
        }

        public void SetSelectedColor(Color color)
        {
            colorComboBox.SelectedIndex = 1;
            for (int i = 0; i < colorComboBox.Items.Count; i++)
            {
                if (colorComboBox.Items[i] is Color itemColor && itemColor == color)
                {
                    colorComboBox.SelectedIndex = i;
                    break;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw a 1px border inside the client area so it's not hidden by the non-client frame
            var rect = this.ClientRectangle;
            if (rect.Width <= 0 || rect.Height <= 0) return;

            Color borderColor = Properties.Settings.Default.DisplayForeColour;
            int borderWidth = 1;

            ControlPaint.DrawBorder(
                e.Graphics,
                rect,
                borderColor, borderWidth, ButtonBorderStyle.Solid,
                borderColor, borderWidth, ButtonBorderStyle.Solid,
                borderColor, borderWidth, ButtonBorderStyle.Solid,
                borderColor, borderWidth, ButtonBorderStyle.Solid
            );
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            // Ensure border repaints correctly after layout or size changes
            this.Invalidate();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SetEditMode(false, true);
            MapController.ZnOverlays.ResetMarkers();
            UpdateForm();
        }

        private void btnCentre_Click(object sender, EventArgs e)
        {
            MapController.CenterMap();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                Job JB = JobManager.CurrentJob;
                if (JB != null && JB.FieldID >= 0)
                {
                    var GetInput = new frmInput("File Name?", "Copy Prescription", true);
                    GetInput.ShowDialog();
                    bool Result = GetInput.Result;
                    string Fname = GetInput.InputValue;
                    GetInput.Close();

                    if (Result)
                    {
                        ParcelManager.EnsureFieldFolders(JB.FieldID);
                        string Folder = ParcelManager.MapsFolder(JB.FieldID);
                        string OldName = Path.GetFileNameWithoutExtension(lbRx.SelectedItem?.ToString());
                        string NewName = Path.GetFileName(Fname);

                        foreach (string file in Directory.GetFiles(Folder, OldName + ".*"))
                        {
                            string ext = Path.GetExtension(file); // keeps .txt, .pdf, etc.
                            string dest = Path.Combine(Folder, NewName + ext);

                            File.Copy(file, dest, overwrite: true);
                        }

                        Fname = Path.Combine(ParcelManager.MapsFolder(JB.FieldID), Fname + ".shp");
                        JobManager.SetActivePrescription(JB.ID, Fname);
                        UpdateForm();
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMap/btnCopy_Click: " + ex.Message);
                Props.ShowMessage("Error saving prescription: " + ex.Message, "Save Prescription", 8000, true);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            switch (MapController.State)
            {
                case MapState.EditZones:
                    MapController.ZnOverlays.DeleteLastVertex();
                    break;

                case MapState.Positioning:
                    if (MapController.ZnOverlays.DeleteZone(tbName.Text))
                    {
                        SetEditMode(false, true);
                        UpdateForm();
                    }
                    else
                    {
                        Props.ShowMessage("Zone could not be deleted.");
                    }
                    break;
            }
        }

        private void btnDeleteData_Click(object sender, EventArgs e)
        {
            var Hlp = new frmMsgBox("Confirm Delete all job data?", "Delete File", true);
            Hlp.TopMost = true;

            Hlp.ShowDialog();
            bool Result = Hlp.Result;
            Hlp.Close();
            if (Result)
            {
                MapController.RateCollector.ClearReadings();

                // Immediately clear coverage overlay and legend from the map
                MapController.ClearAppliedRatesOverlay();
            }
        }

        private void btnDeleteRx_Click(object sender, EventArgs e)
        {
            try
            {
                if (lbRx.SelectedItem.ToString() == "Zones.shp")
                {
                    Props.ShowMessage("Can not delete.", "Help", 10000);
                }
                else
                {
                    var MB = new frmMsgBox("Confirm Delete?", "Help", true);
                    MB.ShowDialog();
                    bool Result = MB.Result;
                    MB.Close();
                    if (Result)
                    {
                        Job JB = JobManager.CurrentJob;
                        if (JB != null && JB.FieldID >= 0)
                        {
                            ParcelManager.EnsureFieldFolders(JB.FieldID);
                            string Folder = ParcelManager.MapsFolder(JB.FieldID);
                            string OldName = Path.GetFileNameWithoutExtension(lbRx.SelectedItem?.ToString());
                            if (Props.IsPathSafe(Folder))
                            {
                                foreach (var file in Directory.GetFiles(Folder))
                                {
                                    if (Path.GetFileNameWithoutExtension(file).Equals(OldName, StringComparison.OrdinalIgnoreCase))
                                    {
                                        File.Delete(file);
                                    }
                                }
                            }
                        }
                        lbRx.SelectedItem = "Zones.shp";
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMap/btnDeleteRx_Click: " + ex.Message);
                Props.ShowMessage("Error deleting prescription: " + ex.Message, "Delete Prescription", 8000, true);
            }
        }


        private void btnHelp_Click(object sender, EventArgs e)
        {
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string FileName = exeDirectory + "Help\\frmMap.pdf";

            try
            {
                if (File.Exists(FileName))
                {
                    Process.Start(new ProcessStartInfo { FileName = FileName, UseShellExecute = true });
                }
                else
                {
                    Props.ShowMessage("No help available.");
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMap/bthHelp_Click: " + ex.Message);
            }
        }

        private void btnImportElevation_Click(object sender, EventArgs e)
        {
            try
            {
                Job job = JobManager.CurrentJob;
                if (job == null || job.FieldID < 0) return;
                using (var dlg = new OpenFileDialog
                {
                    Title = "Import Elevation File",
                    Filter = "CSV files (*.csv)|*.csv",
                    CheckFileExists = true
                })
                {
                    if (dlg.ShowDialog() != DialogResult.OK) return;
                    ParcelManager.EnsureFieldFolders(job.FieldID);
                    string elevFolder = ParcelManager.ElevationFolder(job.FieldID);
                    string elevPath = Path.Combine(elevFolder, "Elevation.csv");
                    string bakPath = Path.Combine(elevFolder, "Elevation.bak");
                    if (File.Exists(elevPath))
                        File.Copy(elevPath, bakPath, true);
                    File.Copy(dlg.FileName, elevPath, true);
                    string ep = FieldDataManager.ElevationPath;
                    if (ep != null) MapController.ElevationCreator.LoadElevationFile(ep);
                    if (MapController.ElevationCreator.Enabled) MapController.ElevationCreator.Build();
                    UpdateFieldDataPanel();
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMap/btnImportElevation_Click: " + ex.Message);
                Props.ShowMessage("Error importing elevation: " + ex.Message, "Import Elevation", 8000, true);
            }
        }

        private void btnImportKML_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog { Title = "Open KML file.", Filter = "Shapefiles (*.kml)|*.kml" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (MapController.AddKmlLayer(ofd.FileName))
                    {
                        ckKML.Checked = true; // reflect visible state
                    }
                }
            }
        }

        private void btnImportRx_Click(object sender, EventArgs e)
        {
            Form fs = Props.IsFormOpen("frmImport");
            if (fs == null)
            {
                fs = new frmImport();
                fs.ShowDialog();
            }
            else
            {
                fs.Focus();
            }
            UpdateForm();
        }

        private void btnImportYield_Click(object sender, EventArgs e)
        {
            try
            {
                Job job = JobManager.CurrentJob;
                if (job == null || job.FieldID < 0) return;

                using (var dlg = new OpenFileDialog
                {
                    Title = "Import Yield File",
                    Filter = "CSV files (*.csv)|*.csv",
                    CheckFileExists = true
                })
                {
                    if (dlg.ShowDialog() != DialogResult.OK) return;

                    string destFolder = ParcelManager.YieldFolder(job.FieldID);
                    ParcelManager.EnsureFieldFolders(job.FieldID);
                    string destPath = Path.Combine(destFolder, Path.GetFileName(dlg.FileName));

                    if (File.Exists(destPath))
                    {
                        using (var confirm = new frmMsgBox($"'{Path.GetFileName(destPath)}' already exists. Overwrite?", "Import Yield", true))
                        {
                            confirm.TopMost = true;
                            confirm.ShowDialog();
                            if (!confirm.Result) return;
                        }
                    }

                    File.Copy(dlg.FileName, destPath, true);

                    // Check for non-zero elevation data in the yield file
                    bool hasElevation = false;
                    string[] lines = File.ReadAllLines(dlg.FileName);
                    for (int i = 1; i < lines.Length && !hasElevation; i++)
                    {
                        string[] parts = lines[i].Split(',');
                        if (parts.Length >= 6 &&
                            double.TryParse(parts[5], System.Globalization.NumberStyles.Number,
                                System.Globalization.CultureInfo.InvariantCulture, out double el) &&
                            Math.Abs(el) > 0.01)
                        {
                            hasElevation = true;
                        }
                    }

                    if (hasElevation)
                    {
                        using (var ask = new frmMsgBox("Yield file contains elevation data. Extract to Elevation.csv?", "Import Yield", true))
                        {
                            ask.TopMost = true;
                            ask.ShowDialog();
                            if (ask.Result)
                            {
                                string elevFolder = ParcelManager.ElevationFolder(job.FieldID);
                                string elevPath = Path.Combine(elevFolder, "Elevation.csv");
                                string bakPath = Path.Combine(elevFolder, "Elevation.bak");

                                if (File.Exists(elevPath))
                                    File.Copy(elevPath, bakPath, true);

                                var sb = new System.Text.StringBuilder();
                                sb.AppendLine("Lat,Lon,Elevation");
                                for (int i = 1; i < lines.Length; i++)
                                {
                                    string[] parts = lines[i].Split(',');
                                    if (parts.Length < 6) continue;
                                    if (!double.TryParse(parts[1], System.Globalization.NumberStyles.Float,
                                            System.Globalization.CultureInfo.InvariantCulture, out double lat)) continue;
                                    if (!double.TryParse(parts[2], System.Globalization.NumberStyles.Float,
                                            System.Globalization.CultureInfo.InvariantCulture, out double lon)) continue;
                                    if (!double.TryParse(parts[5], System.Globalization.NumberStyles.Number,
                                            System.Globalization.CultureInfo.InvariantCulture, out double el)) continue;
                                    sb.AppendLine($"{lat.ToString(System.Globalization.CultureInfo.InvariantCulture)},{lon.ToString(System.Globalization.CultureInfo.InvariantCulture)},{el.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
                                }
                                File.WriteAllText(elevPath, sb.ToString());
                            }
                        }
                    }

                    FieldDataManager.SetYieldPath(destPath);
                    MapController.YieldCreator.LoadData(destPath);
                    if (MapController.YieldCreator.Enabled) MapController.YieldCreator.Build();
                    UpdateFieldDataPanel();
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMap/btnImportYield_Click: " + ex.Message);
                Props.ShowMessage("Error importing yield: " + ex.Message, "Import Yield", 8000, true);
            }
        }

        private void btnImportZones_Click(object sender, EventArgs e)
        {
            Form fs = Props.IsFormOpen("frmImport");
            if (fs == null)
            {
                fs = new frmImport();
                fs.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private void btnKMLdelete_Click(object sender, EventArgs e)
        {
            try
            {
                Job job = JobManager.CurrentJob;
                if (job == null || job.FieldID < 0)
                {
                    Props.ShowMessage("No field assigned to current job.", "Delete KML", 6000, true);
                    return;
                }

                string kmlFolder = ParcelManager.KmlFolder(job.FieldID);
                if (!Directory.Exists(kmlFolder) || Directory.GetFiles(kmlFolder, "*.kml").Length == 0)
                {
                    Props.ShowMessage("No KML files found for this field.", "Delete KML", 6000);
                    return;
                }

                using (var dlg = new OpenFileDialog
                {
                    Title = "Select KML to delete",
                    Filter = "KML files (*.kml)|*.kml",
                    InitialDirectory = kmlFolder,
                    CheckFileExists = true
                })
                {
                    if (dlg.ShowDialog() != DialogResult.OK) return;

                    using (var prompt = new frmMsgBox("Delete KML file from this field?", "Delete KML", true))
                    {
                        prompt.TopMost = true;
                        prompt.ShowDialog();
                        if (!prompt.Result) return;
                    }

                    MapController.DeleteKmlLayer(dlg.FileName);
                    Props.ShowMessage("KML deleted.", "Delete KML", 4000);
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMap/btnDeleteKml_Click: " + ex.Message);
                Props.ShowMessage("Error deleting KML: " + ex.Message, "Delete KML", 8000, true);
            }
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            SetMaxView(false);
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                Job JB = JobManager.CurrentJob;
                if (JB != null && JB.FieldID >= 0)
                {
                    var GetInput = new frmInput("File Name?", "New Prescription", true);
                    GetInput.ShowDialog();
                    bool Result = GetInput.Result;
                    string Fname = GetInput.InputValue;
                    GetInput.Close();

                    if (Result)
                    {
                        ParcelManager.EnsureFieldFolders(JB.FieldID);
                        MapController.ClearZones();
                        Fname = Path.Combine(ParcelManager.MapsFolder(JB.FieldID), Fname + ".shp");
                        MapController.SavePrescription(Fname);
                        UpdateForm();
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMap/btnNew_Click: " + ex.Message);
                Props.ShowMessage("Error creating prescription: " + ex.Message, "New Prescription", 8000, true);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            double RateA = double.TryParse(tbP1.Text, out double p1) ? p1 : 0;
            double RateB = double.TryParse(tbP2.Text, out double p2) ? p2 : 0;
            double RateC = double.TryParse(tbP3.Text, out double p3) ? p3 : 0;
            double RateD = double.TryParse(tbP4.Text, out double p4) ? p4 : 0;
            double RateE = double.TryParse(tbP5.Text, out double p5) ? p5 : 0;

            Color SelectedColor = (Color)(colorComboBox.SelectedItem ?? Color.Blue);

            int Error = 0;
            bool EditSaved = false;
            if (ckNew.Checked)
            {
                EditSaved = MapController.ZnOverlays.CreateZone(tbName.Text, RateA, RateB, RateC, RateD, RateE, SelectedColor, out Error);
            }
            else
            {
                EditSaved = MapController.ZnOverlays.EditZone(tbName.Text, RateA, RateB, RateC, RateD, RateE, SelectedColor, out Error);
            }

            if (EditSaved)
            {
                SetEditMode(false, true);
                MapController.SaveMap();
                UpdateForm();
            }
            else
            {
                string Message = "Could not save the changes.";
                switch (Error)
                {
                    case 1:
                        Message += "\n\nDuplicate name.";
                        break;

                    case 2:
                        Message += "\n\nNot enough points.";
                        break;

                    case 3:
                        Message += "\n\nZone not found.";
                        break;

                    default:
                        break;
                }
                Props.ShowMessage(Message);
            }
        }

        private void btnRestoreElevation_Click(object sender, EventArgs e)
        {
            try
            {
                Job job = JobManager.CurrentJob;
                if (job == null || job.FieldID < 0) return;
                string elevFolder = ParcelManager.ElevationFolder(job.FieldID);
                string elevPath = Path.Combine(elevFolder, "Elevation.csv");
                string bakPath = Path.Combine(elevFolder, "Elevation.bak");
                if (!File.Exists(bakPath)) return;
                if (File.Exists(elevPath))
                    File.Delete(elevPath);
                File.Move(bakPath, elevPath);
                string ep = FieldDataManager.ElevationPath;
                if (ep != null) MapController.ElevationCreator.LoadElevationFile(ep);
                if (MapController.ElevationCreator.Enabled) MapController.ElevationCreator.Build();
                UpdateFieldDataPanel();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMap/btnRestoreElevation_Click: " + ex.Message);
                Props.ShowMessage("Error restoring elevation: " + ex.Message, "Restore Elevation", 8000, true);
            }
        }

        private void btnTimeCancel_Click(object sender, EventArgs e)
        {
            Initializing = true;
            LoadTimes();
            Initializing = false;
            SetButtons(false);
        }

        private void btnTimeOK_Click(object sender, EventArgs e)
        {
            double[] times = new double[Props.MaxProducts - 2];
            double tme = 0;
            times[0] = double.TryParse(tbTime1.Text, out tme) ? tme : 0;
            times[1] = double.TryParse(tbTime2.Text, out tme) ? tme : 0;
            times[2] = double.TryParse(tbTime3.Text, out tme) ? tme : 0;
            times[3] = double.TryParse(tbTime4.Text, out tme) ? tme : 0;
            times[4] = double.TryParse(tbTime5.Text, out tme) ? tme : 0;

            MapController.ZnOverlays.LookAheadSeconds = times;

            Initializing = true;
            LoadTimes();
            Initializing = false;
            SetButtons(false);
        }

        private void btnTitleClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            MapController.Map.Zoom += 1;
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            MapController.Map.Zoom -= 1;
        }

        private void cbYield_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Initializing) return;
            Job job = JobManager.CurrentJob;
            if (job == null || job.FieldID < 0) return;
            string sel = cbYield.SelectedItem as string;
            if (sel == null || sel == "(none)")
                FieldDataManager.SetYieldPath(null);
            else
                FieldDataManager.SetYieldPath(Path.Combine(ParcelManager.YieldFolder(job.FieldID), sel));
            MapController.YieldCreator.LoadData();
            if (MapController.YieldCreator.Enabled) MapController.YieldCreator.Build();
        }

        private void ChangeMapSize()
        {
            try
            {
                tlpTitle.Visible = !UseMaxView;

                if (pnlMain.Width < 250 && pnlMain.Height < 250)
                {
                    pnlMain.Width = 250;
                    pnlMain.Height = 250;
                }

                if (UseMaxView)
                {
                    // full screen
                    TopMost = false;
                    MapController.Map.Zoom = MaxZoom;

                    this.FormBorderStyle = FormBorderStyle.FixedDialog;

                    pnlTabs.Visible = true;
                    pnlControls.Visible = true;
                    pnlControls2.Visible = true;

                    this.Bounds = Screen.GetWorkingArea(this);
                    pnlMain.Size = new Size(this.ClientSize.Width - 565, this.ClientSize.Height - 28);
                    pnlMain.Location = new System.Drawing.Point(550, 14);

                    pnlMap.Width = pnlMain.Width - VSB.Width;
                    pnlMap.Height = pnlMain.Height - HSB.Height;

                    VSB.Left = pnlMap.Width;
                    VSB.Top = 0;
                    VSB.Height = pnlMain.Height - btnZoomIn.Height;

                    HSB.Left = 0;
                    HSB.Top = pnlMap.Height;
                    HSB.Width = pnlMain.Width - btnZoomIn.Width - btnZoomOut.Width;

                    btnZoomOut.Top = HSB.Top;
                    btnZoomOut.Left = HSB.Width;

                    btnZoomIn.Top = VSB.Height;
                    btnZoomIn.Left = VSB.Left;

                    Core.MainForm.Left = this.Left + 25;
                    Core.MainForm.Top = this.Top + pnlTabs.Top + pnlTabs.Height + pnlControls2.Height + 40;
                }
                else
                {
                    // small window
                    TopMost = true;
                    this.Left = MinViewLeft;
                    this.Top = MinViewTop;
                    MapController.Map.Zoom = MinZoom;

                    this.FormBorderStyle = FormBorderStyle.None;

                    pnlTabs.Visible = false;
                    pnlControls.Visible = false;
                    pnlControls2.Visible = false;

                    this.Width = 300;
                    this.Height = 300;

                    // Respect padding so the custom border remains visible
                    tlpTitle.Left = this.Padding.Left;
                    tlpTitle.Top = this.Padding.Top;
                    tlpTitle.Width = this.ClientSize.Width - this.Padding.Horizontal;

                    pnlMain.Location = new System.Drawing.Point(this.Padding.Left, tlpTitle.Bottom);
                    pnlMain.Size = new Size(this.ClientSize.Width - this.Padding.Horizontal, this.ClientSize.Height - tlpTitle.Height - this.Padding.Vertical);
                    pnlMap.Size = pnlMain.Size;

                    Core.MainForm.Left = MainLeft;
                    Core.MainForm.Top = MainTop;
                }

                if (UseMaxView)
                {
                    MapController.legendManager.Show();
                }
                else
                {
                    MapController.legendManager.Hide();
                }
                MapController.DisplaySizeUpdate(!UseMaxView);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMaps/ChangeMapSize: " + ex.Message);
            }
        }

        private void ckAutoTune_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing) MapController.ZnOverlays.AutoTune = ckAutoTune.Checked;
        }

        private void ckEdit_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing)
            {
                ckNew.Enabled = !ckEdit.Checked;
                MapController.Positioning = ckEdit.Checked;
                ckEdit.FlatAppearance.BorderSize = ckEdit.Checked ? 1 : 0;
                SetEditMode(ckEdit.Checked);
            }
        }

        private void ckElevation_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing) MapController.ElevationCreator.Enabled = ckElevation.Checked;
        }

        private void ckKML_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing) MapController.SetKmlVisibility(ckKML.Checked);
        }

        private void ckNew_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing)
            {
                ckEdit.Enabled = !ckNew.Checked;
                MapController.EditingZones = ckNew.Checked;
                ckNew.FlatAppearance.BorderSize = ckNew.Checked ? 1 : 0;
                SetEditMode(ckNew.Checked);
                if (ckNew.Checked) EnterDefaultZoneValues();
            }
        }

        private void ckRateData_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing) MapController.ZnOverlays.AppliedOverlayVisible = ckRateData.Checked;
        }

        private void ckRecord_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing)
            {
                MapController.RateCollector.Enabled = ckRecord.Checked;
            }
        }

        private void ckSatView_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing) MapController.ShowTiles = ckSatView.Checked;
        }

        private void ckUseVR_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing)
            {
                Props.VariableRateEnabled = ckUseVR.Checked;
            }
        }

        private void ckYield_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing) MapController.YieldCreator.Enabled = ckYield.Checked;
        }

        private void ckZones_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing) MapController.ZnOverlays.TargetOverlayVisible = ckZones.Checked;
        }

        private void colorComboBox_Click(object sender, EventArgs e)
        {
            if (!Initializing) EditInProgress = true;
        }

        private void colorComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            Color color = (Color)colorComboBox.Items[e.Index];

            e.DrawBackground();

            using (Brush brush = new SolidBrush(color))
            {
                int rectSize = e.Bounds.Height - 2;
                Rectangle rect = new Rectangle(e.Bounds.X + 2, e.Bounds.Y + 1, rectSize, rectSize);
                e.Graphics.FillRectangle(brush, rect);
                e.Graphics.DrawRectangle(Pens.Black, rect);
            }

            e.Graphics.DrawString(color.Name, e.Font, Brushes.Black, e.Bounds.X + e.Bounds.Height + 2, e.Bounds.Y);
            e.DrawFocusRectangle();
        }

        private void colorComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Initializing) EditInProgress = true;
        }

        private void Core_AppExit(object sender, EventArgs e)
        {
            ShutDown();
        }

        private void Core_ProfileChanged(object sender, EventArgs e)
        {
            UpdateForm();
            LoadProductNames();
        }

        private void EnterDefaultZoneValues()
        {
            int zoneIndex = MapController.ZnOverlays.TargetZoneCount() + 1;
            tbName.Text = "Zone " + zoneIndex.ToString("N0");
            if (zoneIndex < colorComboBox.Items.Count)
            {
                colorComboBox.SelectedIndex = zoneIndex;
            }
            else
            {
                colorComboBox.SelectedIndex = 1;
            }
        }

        private void FieldDataManager_SelectionChanged(object sender, EventArgs e)
        {
            if (InvokeRequired) { Invoke(new Action(UpdateFieldDataPanel)); return; }
            UpdateFieldDataPanel();
        }

        private void FillPrescriptionsList()
        {
            try
            {
                Job job = JobManager.CurrentJob;
                Parcel parcel = job != null && job.FieldID >= 0 ? ParcelManager.SearchParcel(job.FieldID) : null;

                lbFieldName.Text = "Field: " + parcel?.Name ?? "(no field)";

                lbRx.Items.Clear();
                lbRx.Items.Add("Zones.shp");
                if (parcel == null)
                {
                    lbRx.SelectedIndex = 0;
                }
                else
                {
                    foreach (string rx in ParcelManager.GetPrescriptionFiles(job.FieldID))
                    {
                        if (rx != "Zones.shp") lbRx.Items.Add(rx);
                    }
                    string active = Path.GetFileName(job.ActivePrescription);
                    lbRx.SelectedItem = !string.IsNullOrEmpty(active) ? active : "Zones.shp";
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMaps/FillPrescriptionsList: " + ex.Message);
            }
        }

        private void frmMap_FormClosing(object sender, FormClosingEventArgs e)
        {
            ShutDown();
        }

        private void frmMap_Load(object sender, EventArgs e)
        {
            try
            {
                InitializeColorComboBox();

                MainLeft = Core.MainForm.Left;
                MainTop = Core.MainForm.Top;

                MapController.MapChanged += MapController_MapChanged;
                JobManager.JobChanged += JobManager_JobChanged;
                Core.AppExit += Core_AppExit;
                Core.ProfileChanged += Core_ProfileChanged;
                FieldDataManager.SelectionChanged += FieldDataManager_SelectionChanged;

                this.BackColor = Properties.Settings.Default.MainBackColour;
                tlpTitle.BackColor = Properties.Settings.Default.MainBackColour;
                lbTitle.BackColor = Properties.Settings.Default.MainBackColour;
                btnTitleClose.BackColor = Properties.Settings.Default.MainBackColour;
                btnTitleZoomIn.BackColor = Properties.Settings.Default.MainBackColour;
                btnTitleZoomOut.BackColor = Properties.Settings.Default.MainBackColour;

                tlpTitle.Top = 0;
                tlpTitle.Left = 0;

                tabControl1.ItemSize = new Size((tabControl1.Width - 14) / tabControl1.TabCount, tabControl1.ItemSize.Height);

                foreach (TabPage tb in tabControl1.TabPages)
                {
                    tb.BackColor = Properties.Settings.Default.MainBackColour;
                }

                pnlMap.Controls.Add(MapController.Map);

                MapController.MapZoomed += MapController_MapZoomed;
                MapController.MapLeftClicked += MapController_MapLeftClicked;
                MapController.LoadMap();
                MapController.Enabled = true;

                // Reserve 1px around the edges so the painted border remains visible
                this.Padding = new Padding(1);

                LoadFormLocation();

                UpdateScrollbars();
                UpdateForm();

                MapController.MapIsDisplayed = true;

                lbDataPoints.Text = MapController.RateCollector.DataPoints(MapController.ProductFilter).ToString("N0");

                // Sync checkbox with saved preference
                bool kmlVisible = bool.TryParse(Props.GetProp("KmlVisible"), out var v) ? v : true;
                ckKML.Checked = kmlVisible;
                MapController.SetKmlVisibility(kmlVisible);

                SetTitle();
                timer1.Enabled = true;
                SetEditMode(false, true);
                LoadProductNames();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMap/Load: " + ex.Message);
            }
        }

        private void HSB_Scroll(object sender, ScrollEventArgs e)
        {
            double newLng = e.NewValue / 1000.0;
            MapController.Map.Position = new PointLatLng(MapController.Map.Position.Lat, newLng);
        }

        private void InitializeColorComboBox()
        {
            colorComboBox.Items.Clear();

            foreach (KnownColor knownColor in Enum.GetValues(typeof(KnownColor)))
            {
                colorComboBox.Items.Add(Color.FromKnownColor(knownColor));
            }

            colorComboBox.SelectedIndex = 0;
            colorComboBox.DrawMode = DrawMode.OwnerDrawFixed;
            colorComboBox.DrawItem += new DrawItemEventHandler(colorComboBox_DrawItem);
        }

        private void JobManager_JobChanged(object sender, EventArgs e)
        {
            SetTitle();
            LoadProductNames();
        }

        private void lbRx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Initializing)
            {
                Job JB = JobManager.CurrentJob;
                if (JB != null && JB.FieldID >= 0)
                {
                    string sel = lbRx.SelectedItem as string;
                    JobManager.SetActivePrescription(JB.ID, sel);
                    MapController.LoadMap();
                }
            }
        }

        private void LoadFormLocation()
        {
            try
            {
                MinViewLeft = int.TryParse(Props.GetAppProp("MapMinLeft"), out int lf) ? lf : 0;
                MinViewTop = int.TryParse(Props.GetAppProp("MapMinTop"), out int tp) ? tp : 0;

                MinZoom = int.TryParse(Props.GetAppProp("MapMinZoom"), out int zm) ? zm : 16;
                MaxZoom = int.TryParse(Props.GetAppProp("MapMaxZoom"), out int mz) ? mz : 16;

                SetMaxView(!Props.UseMapPreview, true);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMap/LoadFormLocation: " + ex.Message);
            }
        }

        private void LoadProductNames()
        {
            lbP1.Text = Core.Products.Item(0).ProductName;
            lbP2.Text = Core.Products.Item(1).ProductName;
            lbP3.Text = Core.Products.Item(2).ProductName;
            lbP4.Text = Core.Products.Item(3).ProductName;
            lbP5.Text = Core.Products.Item(4).ProductName;
        }

        private void LoadTimes()
        {
            double[] times = MapController.ZnOverlays.LookAheadSeconds;
            int count = times.Count();
            if (count > 0) tbTime1.Text = times[0].ToString("N1");
            if (count > 1) tbTime2.Text = times[1].ToString("N1");
            if (count > 2) tbTime3.Text = times[2].ToString("N1");
            if (count > 3) tbTime4.Text = times[3].ToString("N1");
            if (count > 4) tbTime5.Text = times[4].ToString("N1");
        }

        private void MapController_MapChanged(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void MapController_MapLeftClicked(object sender, EventArgs e)
        {
            if (!UseMaxView) SetMaxView(true);
        }

        private void MapController_MapZoomed(object sender, EventArgs e)
        {
            UpdateScrollbars();
        }

        private double MilesToLatDegrees(double miles)
        {
            return miles / 69.0;
        }

        private double MilesToLngDegrees(double miles, double latitude)
        {
            return miles / (69.0 * Math.Cos(latitude * Math.PI / 180.0));
        }

        private void rbProductA_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing && ((RadioButton)sender).Checked)
            {
                UpdateProductToDisplay();
            }
        }

        private void SaveFormLocation()
        {
            Props.SetAppProp("MapMinLeft", MinViewLeft.ToString());
            Props.SetAppProp("MapMinTop", MinViewTop.ToString());

            Props.SetAppProp("MapMinZoom", MinZoom.ToString());
            Props.SetAppProp("MapMaxZoom", MaxZoom.ToString());

            Props.UseMapPreview = !UseMaxView;
        }

        private void SetButtons(bool Edited)
        {
            if (!Initializing)
            {
                if (Edited)
                {
                    btnTimeCancel.Enabled = true;
                    btnTimeOK.Enabled = true;
                    btnClose.Enabled = false;
                    btnMinimize.Enabled = false;
                }
                else
                {
                    btnTimeCancel.Enabled = false;
                    btnTimeOK.Enabled = false;
                    btnClose.Enabled = true;
                    btnMinimize.Enabled = true;
                }
            }
        }

        private void SetEditMode(bool Editing, bool ResetButtons = false)
        {
            tbName.Enabled = Editing;
            tbP1.Enabled = Editing;
            tbP2.Enabled = Editing;
            tbP3.Enabled = Editing;
            tbP4.Enabled = Editing;
            tbP5.Enabled = Editing;
            colorComboBox.Enabled = Editing;
            btnCancel.Enabled = Editing;
            btnOK.Enabled = Editing;
            btnDelete.Enabled = Editing;

            if (ResetButtons)
            {
                ckEdit.Checked = false;
                ckNew.Checked = false;
            }

            if (!Editing)
            {
                EditInProgress = false;
                UpdateForm();
            }
        }

        private void SetMaxView(bool UseMax = true, bool Initialize = false)
        {
            if (UseMaxView)
            {
                MaxZoom = (int)MapController.Map.Zoom;
            }
            else
            {
                if (Initialize)
                {
                    this.Left = MinViewLeft;
                    this.Top = MinViewTop;
                    MapController.Map.Zoom = MinZoom;
                }
                else
                {
                    MinViewLeft = this.Left;
                    MinViewTop = this.Top;
                    MinZoom = (int)MapController.Map.Zoom;
                }

                MainLeft = Core.MainForm.Left;
                MainTop = Core.MainForm.Top;
            }

            UseMaxView = UseMax;

            ChangeMapSize();
        }

        private void SetTitle()
        {
            string job = JobManager.CurrentJob.Name;
            lbTitle.Text = job.Length <= 15 ? job : job.Substring(0, 15);
        }

        private void ShutDown()
        {
            if (!IsShutDown)
            {
                try
                {
                    IsShutDown = true;
                    if (UseMaxView)
                    {
                        Core.MainForm.Left = MainLeft;
                        Core.MainForm.Top = MainTop;
                    }
                    else
                    {
                        MinViewTop = this.Top;
                        MinViewLeft = this.Left;
                    }

                    MapController.MapIsDisplayed = false;
                    timer1.Enabled = false;

                    // Remove the control from the panel first
                    pnlMap.Controls.Remove(MapController.Map);

                    // Unsubscribe UI-level events
                    MapController.MapZoomed -= MapController_MapZoomed;
                    MapController.MapLeftClicked -= MapController_MapLeftClicked;
                    Core.ProfileChanged -= Core_ProfileChanged;
                    Core.AppExit -= Core_AppExit;
                    FieldDataManager.SelectionChanged -= FieldDataManager_SelectionChanged;

                    SaveFormLocation();
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("frmMaps/CloseCleanup: " + ex.Message);
                }
            }
        }

        private void tbLat_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbLat.Text, out tempD);
            using (var form = new FormNumeric(-90, 90, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbLat.Text = form.ReturnValue.ToString("N7");
                }
            }
        }

        private void tbLat_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Initializing)
            {
                double Latitude;
                if (!double.TryParse(tbLat.Text, out Latitude) || Latitude < -90.0 || Latitude > 90.0)
                {
                    System.Media.SystemSounds.Exclamation.Play();
                    Props.ShowMessage("Latitude must be a number between -90 and 90.");
                    e.Cancel = true;
                }
                else
                {
                    double Longitude;
                    if (double.TryParse(tbLong.Text, out Longitude) || Longitude < -180.0 || Longitude > 180.0)
                    {
                        PointLatLng location = new PointLatLng(Latitude, Longitude);
                        MapController.SetTractorPosition(location, true);
                    }
                }
            }
        }

        private void tbLong_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbLong.Text, out tempD);
            using (var form = new FormNumeric(-180, 180, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbLong.Text = form.ReturnValue.ToString("N7");
                }
            }
        }

        private void tbLong_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Initializing)
            {
                double Longitude;
                if (!double.TryParse(tbLong.Text, out Longitude) || Longitude < -180.0 || Longitude > 180.0)
                {
                    System.Media.SystemSounds.Exclamation.Play();
                    Props.ShowMessage("Longitude must be a number between -180 and 180.");
                    e.Cancel = true;
                }
                else
                {
                    double Latitude;
                    if (double.TryParse(tbLat.Text, out Latitude) || Latitude < -90.0 || Latitude > 90.0)
                    {
                        PointLatLng location = new PointLatLng(Latitude, Longitude);
                        MapController.SetTractorPosition(location, true);
                    }
                }
            }
        }

        private void tbName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Initializing) EditInProgress = true;
        }

        private void tbP1_Enter(object sender, EventArgs e)
        {
            EditInProgress = true;

            double tempD;
            double.TryParse(tbP1.Text, out tempD);

            using (var form = new FormNumeric(0, 10000, tempD))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    tbP1.Text = form.ReturnValue.ToString("N1");
                }
            }
        }

        private void tbP1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbP1.Text, out tempD);

            if (tempD < 0 || tempD > 10000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbP2_Enter(object sender, EventArgs e)
        {
            EditInProgress = true;

            double tempD;
            double.TryParse(tbP2.Text, out tempD);

            using (var form = new FormNumeric(0, 10000, tempD))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    tbP2.Text = form.ReturnValue.ToString("N1");
                }
            }
        }

        private void tbP2_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbP2.Text, out tempD);

            if (tempD < 0 || tempD > 10000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbP3_Enter(object sender, EventArgs e)
        {
            EditInProgress = true;

            double tempD;
            double.TryParse(tbP3.Text, out tempD);

            using (var form = new FormNumeric(0, 10000, tempD))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    tbP3.Text = form.ReturnValue.ToString("N1");
                }
            }
        }

        private void tbP3_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbP3.Text, out tempD);

            if (tempD < 0 || tempD > 10000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbP4_Enter(object sender, EventArgs e)
        {
            EditInProgress = true;

            double tempD;
            double.TryParse(tbP4.Text, out tempD);

            using (var form = new FormNumeric(0, 10000, tempD))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    tbP4.Text = form.ReturnValue.ToString("N1");
                }
            }
        }

        private void tbP4_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbP4.Text, out tempD);

            if (tempD < 0 || tempD > 10000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbP5_Enter(object sender, EventArgs e)
        {
            EditInProgress = true;

            double tempD;
            double.TryParse(tbP5.Text, out tempD);

            using (var form = new FormNumeric(0, 10000, tempD))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    tbP5.Text = form.ReturnValue.ToString("N1");
                }
            }
        }

        private void tbP5_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbP5.Text, out tempD);

            if (tempD < 0 || tempD > 10000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbTime1_Enter(object sender, EventArgs e)
        {
            double temp;
            double.TryParse(tbTime1.Text, out temp);
            using (var form = new FormNumeric(0, 30, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbTime1.Text = form.ReturnValue.ToString("N1");
                }
            }
        }

        private void tbTime1_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbTime2_Enter(object sender, EventArgs e)
        {
            double temp;
            double.TryParse(tbTime2.Text, out temp);
            using (var form = new FormNumeric(0, 30, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbTime2.Text = form.ReturnValue.ToString("N1");
                }
            }
        }

        private void tbTime3_Enter(object sender, EventArgs e)
        {
            double temp;
            double.TryParse(tbTime3.Text, out temp);
            using (var form = new FormNumeric(0, 30, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbTime3.Text = form.ReturnValue.ToString("N1");
                }
            }
        }

        private void tbTime4_Enter(object sender, EventArgs e)
        {
            double temp;
            double.TryParse(tbTime4.Text, out temp);
            using (var form = new FormNumeric(0, 30, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbTime4.Text = form.ReturnValue.ToString("N1");
                }
            }
        }

        private void tbTime5_Enter(object sender, EventArgs e)
        {
            double temp;
            double.TryParse(tbTime5.Text, out temp);
            using (var form = new FormNumeric(0, 30, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbTime5.Text = form.ReturnValue.ToString("N1");
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (UseMaxView)
            {
                switch (tabControl1.SelectedTab.Name)
                {
                    case "tabData":
                        UpdateFileCount();
                        break;

                    case "tabVR":
                        if (!btnTimeOK.Enabled) // don't update when user is editing
                        {
                            Initializing = true;
                            LoadTimes();
                            Initializing = false;
                        }
                        break;

                    case "tabZones":
                        if (!btnOK.Enabled) // don't update when user is editing
                        {
                            Initializing = true;
                            UpdateZoneDetails();
                            Initializing = false;
                        }
                        if (TractorIsMoving()) UpdatePosition();
                        break;
                }
                tbLong.Enabled = !TractorIsMoving();
                tbLat.Enabled = !TractorIsMoving();
            }
        }

        private void tlpTitle_MouseDown(object sender, MouseEventArgs e)
        {
            MouseDownLocation = e.Location;
        }

        private void tlpTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Left) this.Location = new System.Drawing.Point(this.Left + e.X - MouseDownLocation.X, this.Top + e.Y - MouseDownLocation.Y);
        }

        private bool TractorIsMoving()
        {
            return Props.Speed_KMH > 0.5;
        }

        private void UpdateFieldDataPanel()
        {
            Initializing = true;
            try
            {
                Job job = JobManager.CurrentJob;
                Parcel parcel = job != null && job.FieldID >= 0 ? ParcelManager.SearchParcel(job.FieldID) : null;

                lbFieldName.Text = parcel?.Name ?? "(no field)";

                //// Prescription dropdown
                //cbPrescription.Items.Clear();
                //cbPrescription.Items.Add("(none)");
                //if (parcel != null)
                //{
                //    foreach (string f in ParcelManager.GetPrescriptionFiles(job.FieldID))
                //    {
                //        cbPrescription.Items.Add(f);
                //    }
                //    string active = job.ActivePrescription;
                //    cbPrescription.SelectedItem = !string.IsNullOrEmpty(active) ? active : "(none)";
                //}
                //else
                //{
                //    cbPrescription.SelectedIndex = 0;
                //}

                // Yield dropdown
                cbYield.Items.Clear();
                cbYield.Items.Add("(none)");
                if (parcel != null)
                {
                    foreach (string f in ParcelManager.GetYieldFiles(job.FieldID))
                        cbYield.Items.Add(f);
                    string sel = FieldDataManager.SelectedYieldPath != null
                        ? Path.GetFileName(FieldDataManager.SelectedYieldPath) : null;
                    cbYield.SelectedItem = sel ?? "(none)";
                }
                else
                {
                    cbYield.SelectedIndex = 0;
                }

                // Elevation label
                string elevPath = FieldDataManager.ElevationPath;
                lbElevationFile.Text = elevPath != null ? Path.GetFileName(elevPath) : "None";

                // Restore button visibility
                string bakPath = parcel != null
                    ? Path.Combine(ParcelManager.ElevationFolder(job.FieldID), "Elevation.bak") : null;
                btnRestoreElevation.Visible = bakPath != null && File.Exists(bakPath);
            }
            finally
            {
                Initializing = false;
            }
        }

        private void UpdateFileCount()
        {
            lbDataPoints.Text = MapController.RateCollector.DataPoints(MapController.ProductFilter).ToString("N0");
        }

        private void UpdateForm()
        {
            try
            {
                Initializing = true;
                ckUseVR.Checked = Props.VariableRateEnabled;
                ckSatView.Checked = MapController.ShowTiles;
                ckRateData.Checked = MapController.ZnOverlays.AppliedOverlayVisible;
                ckZones.Checked = MapController.ZnOverlays.TargetOverlayVisible;
                ckElevation.Checked = MapController.ElevationCreator.Enabled;
                ckYield.Checked = MapController.YieldCreator.Enabled;

                UpdateZoneDetails();
                UpdatePosition();

                ckRecord.Checked = MapController.RateCollector.Enabled;

                switch (MapController.ProductFilter)
                {
                    case 1:
                        rbProductB.Checked = true;
                        break;

                    case 2:
                        rbProductC.Checked = true;
                        break;

                    case 3:
                        rbProductD.Checked = true;
                        break;

                    case 4:
                        rbProductE.Checked = true;
                        break;

                    default:
                        rbProductA.Checked = true;
                        break;
                }

                LoadTimes();
                ckAutoTune.Checked = MapController.ZnOverlays.AutoTune;

                //UpdateFieldDataPanel();

                if (tabControl1.SelectedIndex == 0) FillPrescriptionsList();

                Initializing = false;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMaps/UpdateForm: " + ex.Message);
                throw;
            }
        }

        private void UpdatePosition()
        {
            tbLong.Text = MapController.TractorPosition.Lng.ToString("N7");
            tbLat.Text = MapController.TractorPosition.Lat.ToString("N7");
        }

        private void UpdateProductToDisplay()
        {
            if (rbProductA.Checked) MapController.ProductFilter = 0;
            else if (rbProductB.Checked) MapController.ProductFilter = 1;
            else if (rbProductC.Checked) MapController.ProductFilter = 2;
            else if (rbProductD.Checked) MapController.ProductFilter = 3;
            else MapController.ProductFilter = 4;

            lbDataPoints.Text = MapController.RateCollector.DataPoints(MapController.ProductFilter).ToString("N0");
        }

        private void UpdateScrollbars()
        {
            try
            {
                double lat = MapController.Map.Position.Lat;
                double lng = MapController.Map.Position.Lng;

                // Scale pan distance based on zoom
                double a = 0.25; // tuning constant
                if (MapController.Map.Zoom < 10) a = 0.6;
                double effectiveMiles = BASE_PAN_DISTANCE_MILES * Math.Exp(a * (MapController.Map.MaxZoom - MapController.Map.Zoom));

                double latOffset = MilesToLatDegrees(effectiveMiles);
                double lngOffset = MilesToLngDegrees(effectiveMiles, lat);

                VSB.Minimum = (int)((lat - latOffset) * 1000);
                VSB.Maximum = (int)((lat + latOffset) * 1000);
                VSB.Value = (int)(lat * 1000);

                HSB.Minimum = (int)((lng - lngOffset) * 1000);
                HSB.Maximum = (int)((lng + lngOffset) * 1000);
                HSB.Value = (int)(lng * 1000);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMap/UpdateScrollbars: " + ex.Message);
            }
        }

        private void UpdateZoneDetails()
        {
            if (!EditInProgress)
            {
                tbName.Text = CurrentZone.Zone.Name;
                tbP1.Text = MapController.GetRate(0).ToString("N1");
                tbP2.Text = MapController.GetRate(1).ToString("N1");
                tbP3.Text = MapController.GetRate(2).ToString("N1");
                tbP4.Text = MapController.GetRate(3).ToString("N1");
                tbP5.Text = MapController.GetRate(4).ToString("N1");
                SetSelectedColor(CurrentZone.Zone.ZoneColor);
            }

            if (Props.UseMetric)
            {
                lbArea.Text = CurrentZone.Hectares.ToString("N1");
                lbAreaName.Text = "Hectares";
            }
            else
            {
                lbArea.Text = (CurrentZone.Hectares * 2.47).ToString("N1");
                lbAreaName.Text = "Acres";
            }
        }

        private void VSB_Scroll(object sender, ScrollEventArgs e)
        {
            // Reverse the vertical scroll direction
            double invertedValue = VSB.Maximum + VSB.Minimum - e.NewValue;

            double newLat = invertedValue / 1000.0;
            MapController.Map.Position = new PointLatLng(newLat, MapController.Map.Position.Lng);
        }

        private void btnExportRx_Click(object sender, EventArgs e)
        {
            string Name = Props.CurrentFileName() + "_RateData_" + DateTime.Now.ToString("dd-MMM-yy");

            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Title = "Save Shapefile As";
                saveFileDialog.Filter = "Shapefile (*.shp)|*.shp|All Files (*.*)|*.*";
                saveFileDialog.DefaultExt = "shp";
                saveFileDialog.FileName = Name + ".shp";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        MapController.SaveMap(saveFileDialog.FileName);

                        string imageName = Path.GetDirectoryName(saveFileDialog.FileName);
                        imageName = Path.Combine(imageName, Path.GetFileNameWithoutExtension(saveFileDialog.FileName)) + ".png";

                        // Capture including legend
                        MapController.SaveMapImage(imageName);

                        Props.ShowMessage("File saved successfully", "Save", 5000);
                    }
                    catch (Exception ex)
                    {
                        Props.ShowMessage("Error saving shapefile: " + ex.Message, "Save", 10000, true);
                    }
                }
            }

        }
    }
}