using AgOpenGPS;
using GMap.NET.WindowsForms;
using RateController.Classes;
using RateController.Forms;
using RateController.Language;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuRateMap : Form
    {
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
        private bool EditZones = false;

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
            Form fs = Props.IsFormOpen("frmCopyMap");

            if (fs == null)
            {
                Form frm = new frmCopyMap(mf);
                frm.Show();
            }
        }

        private void btnCreateZone_Click(object sender, EventArgs e)
        {
            int RateA = 0;
            int RateB = 0;
            int RateC = 0;
            int RateD = 0;
            if (int.TryParse(tbP1.Text, out int p1)) RateA = p1;
            if (int.TryParse(tbP2.Text, out int p2)) RateB = p2;
            if (int.TryParse(tbP3.Text, out int p3)) RateC = p3;
            if (int.TryParse(tbP4.Text, out int p4)) RateD = p4;

            if (!mf.Tls.Manager.UpdateZone(tbName.Text, RateA, RateB, RateC, RateD, GetSelectedColor()))
            {
                mf.Tls.ShowMessage("Could not save Zone.");
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
                mf.Tls.ShowMessage("Zone could not be deleted.");
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

        private void btnPNG_Click(object sender, EventArgs e)
        {
            MapImageSaver saver = new MapImageSaver();
            saver.MapControl = mf.Tls.Manager.gmapObject;
            saver.LegendPanel = legendPanel;
            saver.SaveCompositeImageToFile(Props.CurrentFileName() + "_RateData_" + DateTime.Now.ToString("dd-MMM-yy"));
        }

        private void ckEdit_CheckedChanged(object sender, EventArgs e)
        {
            mf.Tls.Manager.EditModePolygons = ckEditPolygons.Checked;
            EnableButtons();
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
                }
                if (Props.MapShowRates) ShowLegend();
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
                }
                legendPanel.Visible = false;
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
            mf.Tls.Manager.ShowAppliedRatesOverlay();
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

        private void ShowLegend()
        {
            try
            {
                // Position the legendPanel relative to pictureBox1.
                legendPanel.Left = pictureBox1.Left + 10;
                legendPanel.Top = pictureBox1.Top + 10;

                // Define constants for margins and component sizes.
                int minPanelWidth = 150;            // Minimum fixed width.
                int leftMargin = 10;                // Left margin inside the panel.
                int colorBoxWidth = 20;             // Width of the color box.
                int gapBetween = 10;                // Gap between the color box and label.
                int rightMargin = 10;               // Right margin inside the panel.

                // Initially, use the minimum panel width.
                int panelWidth = minPanelWidth;

                // Set a border on the legendPanel.
                legendPanel.BorderStyle = BorderStyle.FixedSingle;

                // Clear any existing legend items.
                legendPanel.Controls.Clear();

                int yOffset = 10;
                int maxLabelWidth = 0;  // Track the widest label.

                // Loop through each legend entry.
                foreach (var entry in mf.Tls.Manager.Legend)
                {
                    // Create a color box.
                    Panel colorBox = new Panel
                    {
                        Size = new Size(colorBoxWidth, 20),
                        BackColor = entry.Value,
                        Location = new Point(leftMargin, yOffset) // Relative to legendPanel.
                    };

                    // Create a label displaying the range.
                    Label label = new Label
                    {
                        Text = entry.Key,
                        AutoSize = true,
                        Location = new Point(leftMargin + colorBoxWidth + gapBetween, yOffset)
                    };

                    // Measure the text size of the label.
                    Size textSize = TextRenderer.MeasureText(entry.Key, label.Font);
                    if (textSize.Width > maxLabelWidth)
                        maxLabelWidth = textSize.Width;

                    legendPanel.Controls.Add(colorBox);
                    legendPanel.Controls.Add(label);

                    yOffset += 20 + 10; // Increment y-offset for the next row.
                }

                // Calculate the required width: left margin + color box width + gap + widest label + right margin.
                int requiredWidth = leftMargin + colorBoxWidth + gapBetween + maxLabelWidth + rightMargin;
                panelWidth = Math.Max(minPanelWidth, requiredWidth);

                // Set the legendPanel's size with the computed width and the computed height.
                legendPanel.Size = new Size(panelWidth, yOffset);
                legendPanel.Visible = Props.MapShowRates;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuRateMap/ShowLegend: " + ex.Message);
            }
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
                    tbP1.Text = form.ReturnValue.ToString("N0");
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
                    tbP2.Text = form.ReturnValue.ToString("N0");
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
                    tbP3.Text = form.ReturnValue.ToString("N0");
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
                    tbP4.Text = form.ReturnValue.ToString("N0");
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
            tbP1.Text = mf.Tls.Manager.GetRate(0).ToString();
            tbP2.Text = mf.Tls.Manager.GetRate(1).ToString();
            tbP3.Text = mf.Tls.Manager.GetRate(2).ToString();
            tbP4.Text = mf.Tls.Manager.GetRate(3).ToString();
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
            VSzoom.Value = (int)((gmap.Zoom - gmap.MinZoom) * 100) / (gmap.MaxZoom - gmap.MinZoom);

            ckEnable.Checked = Props.VariableRateEnabled;
            ckSatView.Checked = mf.Tls.Manager.ShowTiles;
            ckZones.Checked = Props.MapShowZones;
            gbZone.Enabled = ckZones.Checked;

            ckEditPolygons.Checked = mf.Tls.Manager.EditModePolygons;

            Initializing = false;
        }

        private void VSzoom_Scroll(object sender, ScrollEventArgs e)
        {
            mf.Tls.Manager.gmapObject.Zoom = (mf.Tls.Manager.gmapObject.MaxZoom - mf.Tls.Manager.gmapObject.MinZoom) * VSzoom.Value / 100 + mf.Tls.Manager.gmapObject.MinZoom;
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

            if(ckEditZones.Checked)
            {
                ckEditZones.FlatAppearance.BorderSize = 1;
            }
            else
            {
                ckEditZones.FlatAppearance.BorderSize = 0;
                mf.Tls.Manager.EditModePolygons = false;
            }
        }
    }
}