using AgOpenGPS;
using GMap.NET;
using GMap.NET.WindowsForms;
using RateController.Classes;
using RateController.Forms;
using RateController.Language;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuRateMap : Form
    {
        // Pan scale (pixels moved per scrollbar unit)
        private const int PanScalePxPerUnit = 5;

        private bool EditInProgress = false;
        private bool EditZones = false;
        private int FormHeight;
        private int FormWidth;
        private bool Initializing = false;
        private int MainLeft = 0;
        private frmMenu MainMenu;
        private int MainTop = 0;
        private FormStart mf;
        private int PicHeight;
        private int PicLeft;
        private int PicTop;
        private int PicWidth;
        private int PressureLeft = 0;
        private int PressureTop = 0;
        private bool suppressPan = false; // prevents initial centering from panning
        private bool updatingZoom = false;

        public frmMenuRateMap(FormStart main, frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            mf = main;
            this.Tag = false;
            InitializeColorComboBox();
        }

        public void SetSelectedColor(Color color)
        {
            for (int i = 0; i < colorComboBox.Items.Count; i++)
            {
                if (colorComboBox.Items[i] is Color itemColor && itemColor == color)
                {
                    colorComboBox.SelectedIndex = i;
                    return;
                }
            }

            colorComboBox.SelectedIndex = 1;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SetEditInProgress(false);
            ckEditPolygons.Checked = false;
            EnableButtons();
            UpdateForm();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Form frm = new frmCopyMap(mf);
            frm.ShowDialog();
            UpdateMap();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (mf.Tls.Manager.DeleteZone(tbName.Text))
            {
                UpdateForm();
            }
            else
            {
                Props.ShowMessage("Zone could not be deleted.");
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            Form fs = Props.IsFormOpen("frmImport");
            if (fs == null)
            {
                Form frm = new frmImport(mf);
                frm.Show();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            double RateA = 0;
            double RateB = 0;
            double RateC = 0;
            double RateD = 0;

            if (double.TryParse(tbP1.Text, out double p1)) RateA = p1;
            if (double.TryParse(tbP2.Text, out double p2)) RateB = p2;
            if (double.TryParse(tbP3.Text, out double p3)) RateC = p3;
            if (double.TryParse(tbP4.Text, out double p4)) RateD = p4;

            if (!mf.Tls.Manager.UpdateZone(tbName.Text, RateA, RateB, RateC, RateD, GetSelectedColor()))
            {
                Props.ShowMessage("Could not save Zone.");
            }

            SetEditInProgress(false);
            ckEditPolygons.Checked = false;
            EnableButtons();
            mf.Tls.Manager.SaveMap();
            UpdateForm();
        }

        private void btnSave_Click(object sender, EventArgs e)
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
                        mf.Tls.Manager.SaveMapToFile(saveFileDialog.FileName);

                        MapImageSaver saver = new MapImageSaver
                        {
                            MapControl = mf.Tls.Manager.gmapObject
                        };

                        string imageName = Path.GetDirectoryName(saveFileDialog.FileName);
                        imageName = Path.Combine(imageName, Path.GetFileNameWithoutExtension(saveFileDialog.FileName));
                        imageName += ".png";

                        saver.SaveCompositeImageToFile(imageName);
                        Props.ShowMessage("File saved successfully", "Save", 5000);
                    }
                    catch (Exception ex)
                    {
                        Props.ShowMessage("Error saving shapefile: " + ex.Message, "Save", 10000, true);
                    }
                }
            }
        }

        private void CenterPanScrollbars()
        {
            if (hsPan.Maximum <= hsPan.Minimum || vsPan.Maximum <= vsPan.Minimum)
            {
                return;
            }

            int hMid = (hsPan.Maximum + hsPan.Minimum) / 2;
            int vMid = (vsPan.Maximum + vsPan.Minimum) / 2;

            suppressPan = true;
            try
            {
                hsPan.Value = Math.Max(hsPan.Minimum, Math.Min(hMid, hsPan.Maximum));
                vsPan.Value = Math.Max(vsPan.Minimum, Math.Min(vMid, vsPan.Maximum));
            }
            catch
            {
                // ignore invalid assignment
            }
            finally
            {
                suppressPan = false;
            }
        }

        private void ckEditPolygons_CheckedChanged(object sender, EventArgs e)
        {
            mf.Tls.Manager.EditModePolygons = ckEditPolygons.Checked;
            ckEditPolygons.FlatAppearance.BorderSize = ckEditPolygons.Checked ? 1 : 0;
        }

        private void ckEditZones_CheckedChanged(object sender, EventArgs e)
        {
            EditZones = ckEditZones.Checked;
            mf.Tls.Manager.MouseSetTractorPosition = ckEditZones.Checked;
            EnableButtons();

            if (ckEditZones.Checked)
            {
                ckEditZones.FlatAppearance.BorderSize = 1;
                ckEditPolygons.Enabled = true;
                btnDelete.Enabled = true;
            }
            else
            {
                ckEditZones.FlatAppearance.BorderSize = 0;
                mf.Tls.Manager.EditModePolygons = false;
                ckEditPolygons.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        private void ckEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing)
            {
                Props.VariableRateEnabled = ckEnable.Checked;
            }
        }

        private void ckFullScreen_CheckedChanged(object sender, EventArgs e)
        {
            if (ckFullScreen.Checked)
            {
                this.Bounds = Screen.GetWorkingArea(this);
                pictureBox1.Size = new Size(this.ClientSize.Width - 565, this.ClientSize.Height - 28);
                pictureBox1.Location = new Point(550, 14);

                if (Props.UseLargeScreen)
                {
                    mf.LSLeft = PicLeft + this.Left;
                    mf.LSTop = PicTop + this.Top;
                }
                else
                {
                    mf.Left = PicLeft + this.Left;
                    mf.Top = PicTop + this.Top;

                    Form fs = Props.IsFormOpen("frmPressureDisplay", false);
                    if (fs != null)
                    {
                        fs.Left = PicLeft + this.Left;
                        fs.Top = PicTop + this.Top + mf.Height + 10;
                    }
                }

                mf.Tls.Manager.LegendOverlayEnabled = true;
                mf.Tls.Manager.ShowAppliedLayer();
            }
            else
            {
                this.Width = FormWidth;
                this.Height = FormHeight;

                pictureBox1.Size = new Size(PicWidth, PicHeight);
                pictureBox1.Location = new Point(PicLeft, PicTop);
                PositionForm();

                if (Props.UseLargeScreen)
                {
                    mf.LSLeft = MainLeft;
                    mf.LSTop = MainTop;
                }
                else
                {
                    mf.Left = MainLeft;
                    mf.Top = MainTop;

                    Form fs = Props.IsFormOpen("frmPressureDisplay", false);
                    if (fs != null)
                    {
                        fs.Left = PressureLeft;
                        fs.Top = PressureTop;
                    }
                }

                mf.Tls.Manager.LegendOverlayEnabled = false;
                mf.Tls.Manager.ShowAppliedLayer();
            }

            LayoutScrollBars();
            hsPan.BringToFront();
            vsPan.BringToFront();
            CenterPanScrollbars();
            mf.Tls.Manager.CenterMap();
        }

        private void ckMap_CheckedChanged(object sender, EventArgs e)
        {
            mf.Tls.Manager.ShowTiles = ckSatView.Checked;
        }

        private void ckRateData_CheckedChanged(object sender, EventArgs e)
        {
            Props.MapShowRates = ckRateData.Checked;
            mf.Tls.Manager.ShowAppliedRatesOverlay();
        }

        private void ckZones_CheckedChanged(object sender, EventArgs e)
        {
            gbZone.Enabled = ckZones.Checked;
            mf.Tls.Manager.ShowZoneOverlay(ckZones.Checked);
            if (ckZones.Checked)
            {
                mf.Tls.Manager.ZoomToFit();
            }
        }

        private void colorComboBox_Click(object sender, EventArgs e)
        {
            SetEditInProgress(true);
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

        private void EnableButtons()
        {
            tbName.Enabled = EditZones;
            tbP1.Enabled = EditZones;
            tbP2.Enabled = EditZones;
            tbP3.Enabled = EditZones;
            tbP4.Enabled = EditZones;
            colorComboBox.Enabled = EditZones;
            ckEditPolygons.Enabled = EditZones;
        }

        private void frmMenuRateMap_FormClosing(object sender, FormClosingEventArgs e)
        {
            try { mf.Tls.Manager.Dispose(); } catch { }
            try
            {
                if (pictureBox1.Controls.Contains(mf.Tls.Manager.gmapObject))
                    pictureBox1.Controls.Remove(mf.Tls.Manager.gmapObject);
            }
            catch { }
        }

        private void frmMenuRateMap_Resize(object sender, EventArgs e)
        {
            LayoutScrollBars();
        }

        private Color GetSelectedColor()
        {
            return (Color)(colorComboBox.SelectedItem ?? Color.Blue);
        }

        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            mf.Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }

        private void hsPan_Scroll(object sender, ScrollEventArgs e)
        {
            if (suppressPan) return;
            int delta = e.NewValue - e.OldValue;
            if (delta != 0)
            {
                // Use negative to match intuitive drag-left = move map right
                PanMap(-delta * PanScalePxPerUnit, 0);
            }
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

        private void LayoutScrollBars()
        {
            if (pictureBox1.Width <= 0 || pictureBox1.Height <= 0)
            {
                return;
            }

            int scale = ckFullScreen.Checked ? 2 : 1;
            int hThickness = Math.Max(10, SystemInformation.HorizontalScrollBarHeight * scale);
            int vThickness = Math.Max(10, SystemInformation.VerticalScrollBarWidth * scale);

            hsPan.Left = 0;
            hsPan.Top = pictureBox1.Height - hThickness;
            hsPan.Width = Math.Max(10, pictureBox1.Width - vThickness);
            hsPan.Height = hThickness;

            vsPan.Left = pictureBox1.Width - vThickness;
            vsPan.Top = 0;
            vsPan.Width = vThickness;
            vsPan.Height = Math.Max(10, pictureBox1.Height - hThickness);

            bool visible = pictureBox1.Width > 50 && pictureBox1.Height > 50;
            hsPan.Visible = visible;
            vsPan.Visible = visible;

            // Update legend right margin to avoid clipping behind the vertical scrollbar
            try
            {
                mf.Tls.Manager.LegendRightMarginPx = visible ? vThickness : 0;
            }
            catch { }
        }

        private void MainMenu_MenuMoved(object sender, EventArgs e)
        {
            PositionForm();
        }

        private void Manager_MapChanged(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void mnuRateMap_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
        }

        private void mnuRateMap_Load(object sender, EventArgs e)
        {
            SubMenuLayout.SetFormLayout(this, MainMenu, null);
            pictureBox1.Controls.Add(mf.Tls.Manager.gmapObject);
            SetLanguage();

            MainMenu.MenuMoved += MainMenu_MenuMoved;
            mf.Tls.Manager.MapChanged += Manager_MapChanged;

            this.BackColor = Properties.Settings.Default.MainBackColour;
            MainMenu.StyleControls(this);
            PositionForm();
            UpdateForm();
            EnableButtons();

            MainLeft = mf.Left;
            MainTop = mf.Top;

            Form fs = Props.IsFormOpen("frmPressureDisplay", false);
            if (fs != null)
            {
                PressureLeft = fs.Left;
                PressureTop = fs.Top;
            }

            PicTop = pictureBox1.Top;
            PicLeft = pictureBox1.Left;
            PicHeight = pictureBox1.Height;
            PicWidth = pictureBox1.Width;

            FormHeight = this.Height;
            FormWidth = this.Width;

            if (Props.UseLargeScreen)
            {
                MainLeft = mf.LSLeft;
                MainTop = mf.LSTop;
            }

            ckZones.Checked = Props.MapShowZones;
            ckSatView.Checked = Props.MapShowTiles;
            ckRateData.Checked = Props.MapShowRates;

            mf.Tls.Manager.LoadMap();
            mf.Tls.Manager.ShowZoneOverlay(ckZones.Checked);
            mf.Tls.Manager.LegendOverlayEnabled = ckFullScreen.Checked;
            mf.Tls.Manager.ShowAppliedLayer();

            hsPan.Parent = pictureBox1;
            vsPan.Parent = pictureBox1;
            hsPan.BringToFront();
            vsPan.BringToFront();

            this.Resize += frmMenuRateMap_Resize;
            pictureBox1.Resize += pictureBox1_Resize;

            LayoutScrollBars();
            CenterPanScrollbars();
        }

        private void PanMap(int dxPixels, int dyPixels)
        {
            var gmap = mf.Tls.Manager.gmapObject;

            int cx = gmap.Width / 2;
            int cy = gmap.Height / 2;

            PointLatLng newCenter = gmap.FromLocalToLatLng(cx - dxPixels, cy - dyPixels);
            gmap.Position = newCenter;
            gmap.Refresh();
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            LayoutScrollBars();
        }

        private void PositionForm()
        {
            this.Top = MainMenu.Top + SubMenuLayout.TopOffset;
            this.Left = MainMenu.Left + SubMenuLayout.LeftOffset;
        }

        private void SetEditInProgress(bool InProgress)
        {
            if (InProgress == EditInProgress)
            {
                return;
            }

            EditInProgress = InProgress;

            btnCancel.Enabled = InProgress;
            btnOK.Enabled = InProgress;

            ckEditZones.Enabled = !InProgress;
            ckEditPolygons.Enabled = !InProgress;
            btnDelete.Enabled = !InProgress;
        }

        private void SetLanguage()
        {
            btnImport.Text = Lang.lgImport;
            ckFullScreen.Text = Lang.lgFullScreen;
            ckEnable.Text = Lang.lgEnableVR;
            gbZone.Text = Lang.lgZone;
        }

        private void tbName_KeyPress(object sender, KeyPressEventArgs e)
        {
            SetEditInProgress(true);
        }

        private void tbP1_Enter(object sender, EventArgs e)
        {
            SetEditInProgress(true);

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
            SetEditInProgress(true);

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
            SetEditInProgress(true);

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
            SetEditInProgress(true);

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

        private void UpdateForm()
        {
            Initializing = true;

            mf.Tls.Manager.UpdateTargetRates();

            if (!EditInProgress)
            {
                tbName.Text = mf.Tls.Manager.ZoneName;
                tbP1.Text = mf.Tls.Manager.GetRate(0).ToString("N1");
                tbP2.Text = mf.Tls.Manager.GetRate(1).ToString("N1");
                tbP3.Text = mf.Tls.Manager.GetRate(2).ToString("N1");
                tbP4.Text = mf.Tls.Manager.GetRate(3).ToString("N1");
                SetSelectedColor(mf.Tls.Manager.ZoneColor);
            }

            if (!Props.UseMetric)
            {
                lbArea.Text = (mf.Tls.Manager.ZoneHectares * 2.47).ToString("N1");
            }
            else
            {
                lbArea.Text = mf.Tls.Manager.ZoneHectares.ToString("N1");
            }

            GMapControl gmap = mf.Tls.Manager.gmapObject;

            int newValue = (int)Math.Round((gmap.Zoom - gmap.MinZoom) * 100.0 / (gmap.MaxZoom - gmap.MinZoom));
            if (VSzoom.Value != newValue)
            {
                updatingZoom = true;
                VSzoom.Value = newValue;
                updatingZoom = false;
            }

            ckEnable.Checked = Props.VariableRateEnabled;
            ckSatView.Checked = mf.Tls.Manager.ShowTiles;
            ckZones.Checked = Props.MapShowZones;
            gbZone.Enabled = ckZones.Checked;

            ckEditPolygons.Checked = mf.Tls.Manager.EditModePolygons;

            Initializing = false;
        }

        private void UpdateMap()
        {
            if (!pictureBox1.Controls.Contains(mf.Tls.Manager.gmapObject))
            {
                pictureBox1.Controls.Add(mf.Tls.Manager.gmapObject);
            }

            mf.Tls.Manager.LoadMap();
            mf.Tls.Manager.ShowZoneOverlay(ckZones.Checked);
            mf.Tls.Manager.gmapObject.Refresh();
        }

        private void UpdateMapZoom()
        {
            var gmap = mf.Tls.Manager.gmapObject;
            double zoom = (gmap.MaxZoom - gmap.MinZoom) * VSzoom.Value / 100.0 + gmap.MinZoom;
            gmap.Zoom = Math.Round(zoom);
        }

        private void vsPan_Scroll(object sender, ScrollEventArgs e)
        {
            if (suppressPan)
            {
                return;
            }

            int delta = e.NewValue - e.OldValue;
            if (delta != 0)
            {
                PanMap(0, -delta * PanScalePxPerUnit);
            }
        }

        private void VSzoom_Scroll(object sender, ScrollEventArgs e)
        {
            if (!updatingZoom)
            {
                UpdateMapZoom();
            }
        }

        private void VSzoom_ValueChanged(object sender, EventArgs e)
        {
            if (!updatingZoom)
            {
                UpdateMapZoom();
            }
        }
    }
}