using AgOpenGPS;
using GMap.NET;
using RateController.Classes;
using RateController.RateMap;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RateController.Forms
{
    public partial class frmMap : Form
    {
        private const double BASE_PAN_DISTANCE_MILES = 2;
        private bool EditInProgress = false;
        private bool Initializing = true;
        private int MainLeft = 0;
        private int MainTop = 0;
        private int PMheight;
        private int PMwidth;

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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SetEditMode(false, true);
            MapController.ResetMarkers();
            UpdateForm();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
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
                Props.RateCollector.ClearReadings();

                // Immediately clear coverage overlay and legend from the map
                MapController.ClearAppliedRatesOverlay();
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
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
                        MapController.SaveMapToFile(saveFileDialog.FileName);

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

        private void btnImportKML_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog { Title = "Open KML file.", Filter = "Shapefiles (*.kml)|*.kml" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    MapController.AddKmlLayer(ofd.FileName);
                }
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

        private void btnOK_Click(object sender, EventArgs e)
        {
            double RateA = double.TryParse(tbP1.Text, out double p1) ? p1 : 0;
            double RateB = double.TryParse(tbP2.Text, out double p2) ? p2 : 0;
            double RateC = double.TryParse(tbP3.Text, out double p3) ? p3 : 0;
            double RateD = double.TryParse(tbP4.Text, out double p4) ? p4 : 0;

            Color SelectedColor = (Color)(colorComboBox.SelectedItem ?? Color.Blue);

            int Error = 0;
            bool EditSaved = false;
            if (ckNew.Checked)
            {
                EditSaved = MapController.CreateZone(tbName.Text, RateA, RateB, RateC, RateD, SelectedColor, out Error);
            }
            else
            {
                EditSaved = MapController.EditZone(tbName.Text, RateA, RateB, RateC, RateD, SelectedColor, out Error);
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

                    default:
                        break;
                }
                Props.ShowMessage(Message);
            }
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            MapController.Map.Zoom += 1;
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            MapController.Map.Zoom -= 1;
        }

        private void butClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MapController.DeleteZone(tbName.Text))
            {
                SetEditMode(false, true);
                UpdateForm();
            }
            else
            {
                Props.ShowMessage("Zone could not be deleted.");
            }
        }

        private void ChangeMapSize()
        {
            try
            {
                LoadFormLocation();
                if (ckWindow.Checked)
                {
                    // small window
                    TopMost = true;

                    pnlTabs.Visible = false;
                    pnlControls.Visible = false;

                    this.Width = 403;
                    this.Height = 407;

                    pnlMain.Size = new Size(PMwidth, PMheight);
                    pnlMain.Location = new Point(0, 0);

                    if (Props.UseLargeScreen)
                    {
                        Props.MainForm.LSLeft = MainLeft;
                        Props.MainForm.LSTop = MainTop;
                    }
                    else
                    {
                        Props.MainForm.Left = MainLeft;
                        Props.MainForm.Top = MainTop;
                    }
                }
                else
                {
                    // full screen
                    TopMost = false;

                    pnlTabs.Visible = true;
                    pnlControls.Visible = true;

                    this.Bounds = Screen.GetWorkingArea(this);
                    pnlMain.Size = new Size(this.ClientSize.Width - 565, this.ClientSize.Height - 28);
                    pnlMain.Location = new Point(550, 14);

                    if (Props.UseLargeScreen)
                    {
                        Props.MainForm.LSLeft = this.Left + 25;
                        Props.MainForm.LSTop = this.Top + pnlTabs.Top + pnlTabs.Height + 55;
                    }
                    else
                    {
                        Props.MainForm.Left = this.Left + 25;
                        Props.MainForm.Top = this.Top + pnlTabs.Top + pnlTabs.Height + 55;
                    }
                }
                MapController.DisplaySizeUpdate(ckWindow.Checked);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMaps/ChangeMapSize: " + ex.Message);
            }
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

        private void ckNew_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing)
            {
                ckEdit.Enabled = !ckNew.Checked;
                MapController.EditingZones = ckNew.Checked;
                ckNew.FlatAppearance.BorderSize = ckNew.Checked ? 1 : 0;
                tbName.Text = "Zone " + (MapController.ZoneCount + 1).ToString("N0");
                SetEditMode(ckNew.Checked);
            }
        }

        private void ckRateData_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing) MapController.ShowRates = ckRateData.Checked;
        }

        private void ckRecord_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing)
            {
                Props.RateRecordEnabled = ckRecord.Checked;
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

        private void ckWindow_CheckedChanged(object sender, EventArgs e)
        {
            ChangeMapSize();
        }

        private void ckZones_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing) MapController.ShowZones = ckZones.Checked;
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

        private void frmMap_FormClosing(object sender, FormClosingEventArgs e)
        {
            Props.SetAppProp("MapWindow", ckWindow.Checked.ToString());
            ckWindow.Checked = false;
            ChangeMapSize();
            pnlMap.Controls.Remove(MapController.Map);

            MapController.MapZoomed -= MapController_MapZoomed;
            MapController.MapLeftClicked -= MapController_MapLeftClicked;
            Props.ScreensSwitched -= Props_ScreensSwitched;

            if (Props.UseLargeScreen)
            {
                Props.MainForm.LSLeft = MainLeft;
                Props.MainForm.LSTop = MainTop;
            }
            else
            {
                Props.MainForm.Left = MainLeft;
                Props.MainForm.Top = MainTop;
            }
            MapController.MapIsDisplayed = false;

            SaveFormLocation();
            timer1.Enabled = false;
        }

        private void frmMap_Load(object sender, EventArgs e)
        {
            InitializeColorComboBox();

            MapController.MapChanged += MapController_MapChanged;
            this.BackColor = Properties.Settings.Default.MainBackColour;

            tabControl1.ItemSize = new Size((tabControl1.Width - 10) / tabControl1.TabCount, tabControl1.ItemSize.Height);

            foreach (TabPage tb in tabControl1.TabPages)
            {
                tb.BackColor = Properties.Settings.Default.MainBackColour;
            }

            PMheight = pnlMain.Height;
            PMwidth = pnlMain.Width;

            pnlMap.Controls.Add(MapController.Map);

            MapController.MapZoomed += MapController_MapZoomed;
            MapController.MapLeftClicked += MapController_MapLeftClicked;
            MapController.LoadMap();
            MapController.LegendOverlayEnabled = !ckWindow.Checked;
            MapController.Enabled = true;

            Props.ScreensSwitched += Props_ScreensSwitched;
            if (Props.UseLargeScreen)
            {
                MainLeft = Props.MainForm.LSLeft;
                MainTop = Props.MainForm.LSTop;
            }
            else
            {
                MainLeft = Props.MainForm.Left;
                MainTop = Props.MainForm.Top;
            }

            ResizeControls();
            ChangeMapSize();
            UpdateScrollbars();
            UpdateForm();

            ckWindow.Checked = bool.TryParse(Props.GetAppProp("MapWindow"), out bool fs) ? fs : false;
            MapController.MapIsDisplayed = true;
            MapController.Map.Zoom = 16;
            MapController.CenterMap();

            timer1.Enabled = true;
            lbDataPoints.Text = Props.RateCollector.DataPoints.ToString("N0");
        }

        private void frmMap_Move(object sender, EventArgs e)
        {
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

        private void LoadFormLocation()
        {
            if (ckWindow.Checked)
            {
                this.Left = int.TryParse(Props.GetAppProp("MapWindow.Left"), out int lf) ? lf : 0;
                this.Top = int.TryParse(Props.GetAppProp("MapWindow.Top"), out int tp) ? tp : 0;
            }
            else
            {
                Props.LoadFormLocation(this);
            }
        }

        private void MapController_MapChanged(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void MapController_MapLeftClicked(object sender, EventArgs e)
        {
            if (ckWindow.Checked)
            {
                ckWindow.Checked = false;
                ChangeMapSize();
            }
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

        private void pnlMain_Resize(object sender, EventArgs e)
        {
            ResizeControls();
        }

        private void Props_ScreensSwitched(object sender, EventArgs e)
        {
            ChangeMapSize();
        }

        private void rbProductA_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing) UpdateProductToDisplay();
        }

        private void ResizeControls()
        {
            if (pnlMain.Width < 250 && pnlMain.Height < 250)
            {
                pnlMain.Width = 250;
                pnlMain.Height = 250;
            }

            pnlMap.Left = 0;
            pnlMap.Top = 0;

            if (ckWindow.Checked)
            {
                pnlMap.Width = this.Width;
                pnlMap.Height = this.Height;
            }
            else
            {
                pnlMap.Width = pnlMain.Width - VSB.Width;
                pnlMap.Height = pnlMain.Height - HSB.Height;
            }

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
        }

        private void SaveFormLocation()
        {
            if (ckWindow.Checked)
            {
                Props.SetAppProp("MapWindow.Left", this.Left.ToString());
                Props.SetAppProp("MapWindow.Top", this.Top.ToString());
            }
            else
            {
                Props.SaveFormLocation(this);
            }
        }

        private void SetEditMode(bool Editing, bool ResetButtons = false)
        {
            tbName.Enabled = Editing;
            tbP1.Enabled = Editing;
            tbP2.Enabled = Editing;
            tbP3.Enabled = Editing;
            tbP4.Enabled = Editing;
            colorComboBox.Enabled = Editing;
            btnCancel.Enabled = Editing;
            btnOK.Enabled = Editing;
            btnDeleteZone.Enabled = Editing;

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

        private void timer1_Tick(object sender, EventArgs e)
        {
            lbDataPoints.Text = Props.RateCollector.DataPoints.ToString("N0");
        }

        private void UpdateForm()
        {
            try
            {
                Initializing = true;
                ckUseVR.Checked = Props.VariableRateEnabled;
                ckSatView.Checked = MapController.ShowTiles;
                ckRateData.Checked = MapController.ShowRates;
                ckZones.Checked = MapController.ShowZones;

                if (!EditInProgress)
                {
                    tbName.Text = MapController.ZoneName;
                    tbP1.Text = MapController.GetRate(0).ToString("N1");
                    tbP2.Text = MapController.GetRate(1).ToString("N1");
                    tbP3.Text = MapController.GetRate(2).ToString("N1");
                    tbP4.Text = MapController.GetRate(3).ToString("N1");
                    SetSelectedColor(MapController.ZoneColor);
                }

                if (Props.UseMetric)
                {
                    lbArea.Text = MapController.ZoneHectares.ToString("N1");
                    lbAreaName.Text = "Hectares";
                }
                else
                {
                    lbArea.Text = (MapController.ZoneHectares * 2.47).ToString("N1");
                    lbAreaName.Text = "Acres";
                }

                ckRecord.Checked = Props.RateRecordEnabled;

                switch (MapController.ProductRates)
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

                    default:
                        rbProductA.Checked = true;
                        break;
                }

                Initializing = false;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMaps/UpdateForm: " + ex.Message);
                throw;
            }
        }

        private void UpdateProductToDisplay()
        {
            if (rbProductA.Checked) MapController.ProductRates = 0;
            else if (rbProductB.Checked) MapController.ProductRates = 1;
            else if (rbProductC.Checked) MapController.ProductRates = 2;
            else MapController.ProductRates = 3;
        }

        private void UpdateScrollbars()
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

        private void VSB_Scroll(object sender, ScrollEventArgs e)
        {
            // Reverse the vertical scroll direction
            double invertedValue = VSB.Maximum + VSB.Minimum - e.NewValue;

            double newLat = invertedValue / 1000.0;
            MapController.Map.Position = new PointLatLng(newLat, MapController.Map.Position.Lng);
        }
    }
}