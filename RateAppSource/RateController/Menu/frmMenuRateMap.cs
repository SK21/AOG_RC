using GMap.NET.WindowsForms;
using System;
using System.Drawing;
using System.Windows.Forms;

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

            if (!mf.Tls.Manager.CreateZone(tbName.Text, RateA, RateB, RateC, RateD))
            {
                MessageBox.Show("A map zone must have at least three vertices.", "Error");
            }
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
            if (!mf.Tls.Manager.NewMap())
            {
                MessageBox.Show("New zone could not be created.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (mf.Tls.Manager.SaveMap())
            {
                MessageBox.Show("Map saved successfully!");
            }
            else
            {
                MessageBox.Show("Map could not be saved.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ckEdit_CheckedChanged(object sender, EventArgs e)
        {
            mf.Tls.Manager.EditMode = ckEdit.Checked;
            SetButtons();
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
                pictureBox1.Size = new Size(467,294);
                pictureBox1.Location = new System.Drawing.Point(66, 324);
                PositionForm();
            }
        }

        private void frmMenuRateMap_FormClosing(object sender, FormClosingEventArgs e)
        {
            pictureBox1.Controls.Remove(mf.Tls.Manager.gmapObject);
        }

        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            mf.Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);
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
            SetButtons();
        }

        private void PositionForm()
        {
            this.Top = MainMenu.Top + 30;
            this.Left = MainMenu.Left + 246;
        }

        private void SetButtons()
        {
            tbName.Enabled = mf.Tls.Manager.EditMode;
            tbP1.Enabled = mf.Tls.Manager.EditMode;
            tbP2.Enabled = mf.Tls.Manager.EditMode;
            tbP3.Enabled = mf.Tls.Manager.EditMode;
            tbP4.Enabled = mf.Tls.Manager.EditMode;
        }

        private void SetLanguage()
        {
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