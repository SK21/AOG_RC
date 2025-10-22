using AgOpenGPS;
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

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Form frm = new frmCopyMap(mf);
            frm.ShowDialog();
            UpdateMap();
        }

        private void btnCreateZone_Click(object sender, EventArgs e)
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
            btnCreateZone.FlatAppearance.BorderSize = 0;
            ckEditPolygons.Checked = false;
            EnableButtons();
            mf.Tls.Manager.SaveMap();
            UpdateForm();
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            string Name = Props.CurrentFileName() + "_RateData_" + DateTime.Now.ToString("dd-MMM-yy");

            // Save as shapefile: show SaveFileDialog and save
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

                        // save image
                        MapImageSaver saver = new MapImageSaver
                        {
                            MapControl = mf.Tls.Manager.gmapObject
                        };

                        string ImageName = Path.GetDirectoryName(saveFileDialog.FileName);
                        ImageName = Path.Combine(ImageName, Path.GetFileNameWithoutExtension(saveFileDialog.FileName));
                        ImageName += ".png";

                        saver.SaveCompositeImageToFile(ImageName);
                        Props.ShowMessage("File saved successfully", "Save", 5000);
                    }
                    catch (Exception ex)
                    {
                        Props.ShowMessage("Error saving shapefile: " + ex.Message, "Save", 10000, true);
                    }
                }
            }
        }

        private void ckEditPolygons_CheckedChanged(object sender, EventArgs e)
        {
            mf.Tls.Manager.EditModePolygons = ckEditPolygons.Checked;

            if (ckEditPolygons.Checked)
            {
                ckEditPolygons.FlatAppearance.BorderSize = 1;
            }
            else
            {
                ckEditPolygons.FlatAppearance.BorderSize = 0;
            }
        }

        private void ckEditZones_CheckedChanged(object sender, EventArgs e)
        {
            EditZones = ckEditZones.Checked;
            mf.Tls.Manager.MouseSetTractorPosition = ckEditZones.Checked;
            EnableButtons();

            if (ckEditZones.Checked)
            {
                ckEditZones.FlatAppearance.BorderSize = 1;
            }
            else
            {
                ckEditZones.FlatAppearance.BorderSize = 0;
                mf.Tls.Manager.EditModePolygons = false;
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
                pictureBox1.Location = new System.Drawing.Point(550, 14);

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

                // Show legend overlay only in full-screen
                mf.Tls.Manager.LegendOverlayEnabled = true;
                mf.Tls.Manager.ShowAppliedLayer(); // refresh coverage + legend overlay
            }
            else
            {
                this.Width = FormWidth;
                this.Height = FormHeight;
                pictureBox1.Size = new Size(PicWidth, PicHeight);
                pictureBox1.Location = new System.Drawing.Point(PicLeft, PicTop);
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

                // Hide legend overlay when not full-screen
                mf.Tls.Manager.LegendOverlayEnabled = false;
                mf.Tls.Manager.ShowAppliedLayer(); // clears legend overlay via flag
            }
            mf.Tls.Manager.ZoomToFit();
        }

        private void ckMap_CheckedChanged(object sender, EventArgs e)
        {
            mf.Tls.Manager.ShowTiles = ckSatView.Checked;
        }

        private void ckRateData_CheckedChanged(object sender, EventArgs e)
        {
            Props.MapShowRates = ckRateData.Checked;
            mf.Tls.Manager.ShowAppliedRatesOverlay(); // this will update legend overlay if enabled
            // No panel legend; overlay legend is updated inside ShowAppliedRatesOverlay/ShowAppliedLayer
        }

        private void ckZones_CheckedChanged(object sender, EventArgs e)
        {
            gbZone.Enabled = ckZones.Checked;
            mf.Tls.Manager.ShowZoneOverlay(ckZones.Checked);
            if (ckZones.Checked) mf.Tls.Manager.ZoomToFit();
        }

        private void colorComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            // Get the color object to draw
            Color color = (Color)colorComboBox.Items[e.Index];

            // Draw the background
            e.DrawBackground();

            // Draw a rectangle filled with the color
            using (Brush brush = new SolidBrush(color))
            {
                int rectSize = e.Bounds.Height - 2;
                Rectangle rect = new Rectangle(e.Bounds.X + 2, e.Bounds.Y + 1, rectSize, rectSize);

                e.Graphics.FillRectangle(brush, rect);
                e.Graphics.DrawRectangle(Pens.Black, rect);
            }

            // Draw the color name next to the rectangle
            e.Graphics.DrawString(color.Name, e.Font, Brushes.Black, e.Bounds.X + e.Bounds.Height + 2, e.Bounds.Y);

            // Draw focus rectangle if the combo box has focus
            e.DrawFocusRectangle();
        }

        private void EnableButtons()
        {
            tbName.Enabled = EditZones;
            tbP1.Enabled = EditZones;
            tbP2.Enabled = EditZones;
            tbP3.Enabled = EditZones;
            tbP4.Enabled = EditZones;
            btnDelete.Enabled = EditZones;
            btnCreateZone.Enabled = EditZones;
            colorComboBox.Enabled = EditZones;
            ckEditPolygons.Enabled = EditZones;
        }

        private void frmMenuRateMap_FormClosing(object sender, FormClosingEventArgs e)
        {
            pictureBox1.Controls.Remove(mf.Tls.Manager.gmapObject);
        }

        private Color GetSelectedColor()
        {
            Color Result = Color.Blue;
            if (colorComboBox.SelectedItem != null)
            {
                Result = (Color)colorComboBox.SelectedItem;
            }
            return Result;
        }

        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            mf.Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }

        private void HighlightZoneSave()
        {
            btnCreateZone.FlatAppearance.BorderSize = 2;
            btnCreateZone.FlatAppearance.BorderColor = Color.Blue;
        }

        private void InitializeColorComboBox()
        {
            // Clear any existing items
            colorComboBox.Items.Clear();

            // Iterate through the KnownColor enum to get all known colors
            foreach (KnownColor knownColor in Enum.GetValues(typeof(KnownColor)))
            {
                Color color = Color.FromKnownColor(knownColor);
                colorComboBox.Items.Add(color);
            }

            // Optionally set a default selected item
            colorComboBox.SelectedIndex = 0;

            // Attach the DrawItem event
            colorComboBox.DrawMode = DrawMode.OwnerDrawFixed;
            colorComboBox.DrawItem += new DrawItemEventHandler(colorComboBox_DrawItem);
        }

        private void MainMenu_MenuMoved(object sender, EventArgs e)
        {
            PositionForm();
        }

        // Rebuild the panel legend whenever the map/legend changes or the toggle is used
        private void Manager_MapChanged(object sender, EventArgs e)
        {
            UpdateForm();
            // No panel legend refresh; overlay legend is handled in MapManager
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

            // Initialize legend overlay visibility based on current full-screen state
            mf.Tls.Manager.LegendOverlayEnabled = ckFullScreen.Checked;
            mf.Tls.Manager.ShowAppliedLayer();
        }

        private void PositionForm()
        {
            this.Top = MainMenu.Top + SubMenuLayout.TopOffset;
            this.Left = MainMenu.Left + SubMenuLayout.LeftOffset;
        }

        private void SetLanguage()
        {
            btnImport.Text = Lang.lgImport;
            ckFullScreen.Text = Lang.lgFullScreen;
            ckEnable.Text = Lang.lgEnableVR;
            gbZone.Text = Lang.lgZone;
        }

        private void tbName_TextChanged(object sender, EventArgs e)
        {
            if (ckEditPolygons.Checked)
            {
                HighlightZoneSave();
            }
        }

        private void tbP1_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbP1.Text, out tempD);
            using (var form = new FormNumeric(0, 10000, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
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
            double tempD;
            double.TryParse(tbP2.Text, out tempD);
            using (var form = new FormNumeric(0, 10000, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
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
            double tempD;
            double.TryParse(tbP3.Text, out tempD);
            using (var form = new FormNumeric(0, 10000, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
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
            double tempD;
            double.TryParse(tbP4.Text, out tempD);
            using (var form = new FormNumeric(0, 10000, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
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
            tbName.Text = mf.Tls.Manager.ZoneName;
            tbP1.Text = mf.Tls.Manager.GetRate(0).ToString("N1");
            tbP2.Text = mf.Tls.Manager.GetRate(1).ToString("N1");
            tbP3.Text = mf.Tls.Manager.GetRate(2).ToString("N1");
            tbP4.Text = mf.Tls.Manager.GetRate(3).ToString("N1");
            SetSelectedColor(mf.Tls.Manager.ZoneColor);
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

        private void VSzoom_Scroll(object sender, ScrollEventArgs e)
        {
            if (!updatingZoom)
                UpdateMapZoom();
        }

        private void VSzoom_ValueChanged(object sender, EventArgs e)
        {
            if (!updatingZoom)
                UpdateMapZoom();
        }
    }
}