using AgOpenGPS;
using GMap.NET.WindowsForms;
using System;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RateController.Menu
{
    public partial class frmMenuRateMap : Form
    {
        private bool Initializing = false;
        private frmMenu MainMenu;
        private FormStart mf;

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
                MessageBox.Show("Could not save zone.", "Error");
            }
            btnCreateZone.FlatAppearance.BorderSize = 0;
            ckEdit.Checked = false;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!mf.Tls.Manager.DeleteZone(tbName.Text))
            {
                MessageBox.Show("Zone could not be deleted.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            Form frm = new RateController.Forms.frmImport(mf);
            frm.ShowDialog();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = mf.Tls.Manager.RootPath;
            openFileDialog1.Filter = "Shapefiles (*.shp)|*.shp|All files (*.*)|*.*";
            openFileDialog1.Title = "Select a Shapefile";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!mf.Tls.Manager.LoadMap(openFileDialog1.FileName))
                {
                    MessageBox.Show("Map could not be loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (mf.Tls.Manager.NewMap())
            {
                tbMapName.Text = "New Map";
            }
            else
            {
                MessageBox.Show("New zone could not be created.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (mf.Tls.Manager.SaveMap(tbMapName.Text))
            {
                MessageBox.Show("Map saved successfully!");
            }
            else
            {
                MessageBox.Show("Map could not be saved.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            btnSave.FlatAppearance.BorderSize = 0;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Initializing = true;
            if (!mf.Tls.Manager.LoadLastMap())
            {
                MessageBox.Show("Map could not be loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Initializing = false;
        }

        private void ckEdit_CheckedChanged(object sender, EventArgs e)
        {
            mf.Tls.Manager.EditMode = ckEdit.Checked;
            EnableButtons();
        }

        private void ckEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing) mf.Tls.VariableRateEnabled = ckEnable.Checked;
        }

        private void ckFullScreen_CheckedChanged(object sender, EventArgs e)
        {
            if (ckFullScreen.Checked)
            {
                this.Bounds = Screen.GetWorkingArea(this);
                pictureBox1.Size = new Size(this.ClientSize.Width - 565, this.ClientSize.Height - 28);
                pictureBox1.Location = new System.Drawing.Point(550, 14);
            }
            else
            {
                this.Width = 540;
                this.Height = 630;
                pictureBox1.Size = new Size(467, 294);
                pictureBox1.Location = new System.Drawing.Point(66, 324);
                PositionForm();
            }
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
            tbName.Enabled = mf.Tls.Manager.EditMode;
            tbP1.Enabled = mf.Tls.Manager.EditMode;
            tbP2.Enabled = mf.Tls.Manager.EditMode;
            tbP3.Enabled = mf.Tls.Manager.EditMode;
            tbP4.Enabled = mf.Tls.Manager.EditMode;
            btnDelete.Enabled = !mf.Tls.Manager.EditMode;
            btnCreateZone.Enabled = mf.Tls.Manager.EditMode;
            colorComboBox.Enabled = mf.Tls.Manager.EditMode;
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

        private void HighlightMapSave()
        {
            if (!Initializing)
            {
                btnSave.FlatAppearance.BorderSize = 2;
                btnSave.FlatAppearance.BorderColor = Color.Blue;
            }
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
            mf.Tls.SaveFormData(this);
        }

        private void mnuRateMap_Load(object sender, EventArgs e)
        {
            // menu 800,600
            // sub menu 540,630
            pictureBox1.Controls.Add(mf.Tls.Manager.gmapObject);
            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            mf.Tls.Manager.MapChanged += Manager_MapChanged;
            mf.Tls.LoadFormData(this, "", false);
            this.BackColor = Properties.Settings.Default.BackColour;
            this.Width = MainMenu.Width - 260;
            this.Height = MainMenu.Height - 50;
            MainMenu.StyleControls(this);
            PositionForm();
            UpdateForm();
            EnableButtons();
        }

        private void PositionForm()
        {
            this.Top = MainMenu.Top + 30;
            this.Left = MainMenu.Left + 246;
        }

        private void SetLanguage()
        {
        }

        private void tbMapName_TextChanged(object sender, EventArgs e)
        {
            HighlightMapSave();
        }

        private void tbName_TextChanged(object sender, EventArgs e)
        {
            if (ckEdit.Checked)
            {
                HighlightZoneSave();
                HighlightMapSave();
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

            int[] Rates = mf.Tls.Manager.GetRates();
            tbMapName.Text = mf.Tls.Manager.MapName;
            tbName.Text = mf.Tls.Manager.ZoneName;
            tbP1.Text = Rates[0].ToString();
            tbP2.Text = Rates[1].ToString();
            tbP3.Text = Rates[2].ToString();
            tbP4.Text = Rates[3].ToString();
            SetSelectedColor(mf.Tls.Manager.ZoneColor);

            GMapControl gmap = mf.Tls.Manager.gmapObject;
            VSzoom.Value = (int)((gmap.Zoom - gmap.MinZoom) * 100) / (gmap.MaxZoom - gmap.MinZoom);

            ckEnable.Checked = mf.Tls.VariableRateEnabled;
            Initializing = false;
        }

        private void VSzoom_Scroll(object sender, ScrollEventArgs e)
        {
            mf.Tls.Manager.gmapObject.Zoom = (mf.Tls.Manager.gmapObject.MaxZoom - mf.Tls.Manager.gmapObject.MinZoom) * VSzoom.Value / 100 + mf.Tls.Manager.gmapObject.MinZoom;
        }
    }
}