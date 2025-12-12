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
        private int MainLeft = 0;
        private int MainTop = 0;
        private int PMheight;
        private int PMwidth;

        public frmMap()
        {
            InitializeComponent();
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

        private void ckRateData_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void ckSatView_CheckedChanged(object sender, EventArgs e)
        {
            MapController.ShowTiles = ckSatView.Checked;
        }

        private void ckWindow_CheckedChanged(object sender, EventArgs e)
        {
            ChangeMapSize();
        }


        private void frmMap_FormClosing(object sender, FormClosingEventArgs e)
        {
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
            Props.SetAppProp("MapWindow", ckWindow.Checked.ToString());
        }

        private void frmMap_Load(object sender, EventArgs e)
        {
            MapController.MapChanged += MapController_MapChanged;
            this.BackColor = Properties.Settings.Default.MainBackColour;

            tabPage1.BackColor = Properties.Settings.Default.MainBackColour;
            tabPage2.BackColor = Properties.Settings.Default.MainBackColour;
            tabControl1.ItemSize = new Size((tabControl1.Width - 10) / 2, tabControl1.ItemSize.Height);

            ckZones.Checked = Props.MapShowZones;
            ckSatView.Checked = Props.MapShowTiles;
            ckRateData.Checked = Props.MapShowRates;

            PMheight = pnlMain.Height;
            PMwidth = pnlMain.Width;

            pnlMap.Controls.Add(MapController.Map);

            MapController.MapZoomed += MapController_MapZoomed;
            MapController.MapLeftClicked += MapController_MapLeftClicked;
            MapController.LoadMap();
            MapController.ShowZoneOverlay(ckZones.Checked);
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

            ckWindow.Checked = bool.TryParse(Props.GetAppProp("MapWindow"), out bool fs) ? fs : true;
            MapController.MapIsDisplayed = true;
            MapController.Map.Zoom = 16;
            MapController.CenterMap();
        }

        private void frmMap_Move(object sender, EventArgs e)
        {
        }

        private void HSB_Scroll(object sender, ScrollEventArgs e)
        {
            double newLng = e.NewValue / 1000.0;
            MapController.Map.Position = new PointLatLng(MapController.Map.Position.Lat, newLng);
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
            }
            else
            {
                ckWindow.Checked = true;
            }
                ChangeMapSize();
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

        private void UpdateForm()
        {
            try
            {
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMaps/UpdateForm: " + ex.Message);
                throw;
            }
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

            tbName.Text = MapController.Map.Zoom.ToString();
        }

        private void VSB_Scroll(object sender, ScrollEventArgs e)
        {
            // Reverse the vertical scroll direction
            double invertedValue = VSB.Maximum + VSB.Minimum - e.NewValue;

            double newLat = invertedValue / 1000.0;
            MapController.Map.Position = new PointLatLng(newLat, MapController.Map.Position.Lng);
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {

        }
    }
}