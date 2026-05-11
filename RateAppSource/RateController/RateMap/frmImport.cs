using RateController.Classes;
using RateController.Language;
using RateController.RateMap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RateController.Forms
{
    public partial class frmImport : Form
    {
        private Dictionary<string, string> attributeMapping;
        private string selectedShapefilePath;
        private List<MapZone> _mapZones;
        private int _importedZoneCount;

        public frmImport()
        {
            InitializeComponent();
            dgvMapping.AutoGenerateColumns = false;
            dgvMapping.AllowUserToAddRows = false;
        }

        private void btnBuild_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(selectedShapefilePath)) return;

                attributeMapping = new Dictionary<string, string>();
                foreach (DataGridViewRow row in dgvMapping.Rows)
                {
                    var predefined = row.Cells["PredefinedAttribute"].Value?.ToString();
                    var shapefileAttribute = row.Cells["ShapefileAttribute"].Value?.ToString();
                    if (!string.IsNullOrEmpty(predefined) && !string.IsNullOrEmpty(shapefileAttribute))
                        attributeMapping[predefined] = shapefileAttribute;
                }

                var shapefileHelper = new ShapefileHelper();
                _mapZones = shapefileHelper.CreateZoneList(selectedShapefilePath, attributeMapping);
                _importedZoneCount = _mapZones.Count;
                tbNumZones.Text = _importedZoneCount.ToString();
                tbMinZoneSize.Text = "0";
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmImport/btnBuild_Click: " + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (_mapZones == null)
                {
                    Props.ShowMessage("Import the shapefile first (Step 1).");
                    return;
                }

                Job JB = JobManager.CurrentJob;
                if (JB != null && JB.FieldID >= 0)
                {
                    string Fname = Path.GetFileName(tbName.Text);
                    if (FileNameValidator.IsValidFileName(Fname))
                    {
                        ParcelManager.EnsureFieldFolders(JB.FieldID);
                        JobManager.SetActivePrescription(JB.ID, Fname + ".shp");
                        string MP = Path.Combine(ParcelManager.MapsFolder(JB.FieldID), JB.ActivePrescription);

                        int.TryParse(tbNumZones.Text, out int zoneCount);
                        double.TryParse(tbMinZoneSize.Text, out double enteredArea);
                        double minZoneHa = Props.UseMetric ? enteredArea : enteredArea * 0.404686;

                        var helper = new ShapefileHelper();
                        var zonesToSave = _mapZones;
                        if (zoneCount > 0 && (zoneCount < _importedZoneCount || enteredArea > 0))
                            zonesToSave = helper.SimplifyPrescriptionGrid(_mapZones, zoneCount, minZoneHa);

                        if (helper.SaveMapZones(MP, zonesToSave))
                        {
                            MapController.LoadMap();
                            JobManager.SetActivePrescription(JB.ID, Path.GetFileName(MP));
                            Props.ShowMessage("Prescription saved.");
                            this.Close();
                        }
                        else
                        {
                            Props.ShowMessage("Failed to save prescription.");
                        }
                    }
                    else
                    {
                        Props.ShowMessage("Invalid file name.", "Help", 10000);
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmImport/btnSave_Click: " + ex.Message);
            }
        }

        private void frmImport_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
        }

        private void frmImport_Load(object sender, EventArgs e)
        {
            Props.LoadFormLocation(this);
            this.BackColor = Properties.Settings.Default.MainBackColour;
            lbArea.Text = Props.UseMetric ? "Ha" : "Ac";
            SetLanguage();
            SelectShapefile(dgvMapping);
        }

        private void LoadShapefileAttributes(DataGridView dgvMapping)
        {
            var shapefileHelper = new ShapefileHelper();
            var shapefileAttributes = shapefileHelper.GetShapefileAttributes(selectedShapefilePath);

            // add shapefile attribute names
            if (dgvMapping.Columns["ShapefileAttribute"] is DataGridViewComboBoxColumn bx)
            {
                bx.Items.Clear();
                bx.Items.AddRange(shapefileAttributes.ToArray());
            }

            // Map predefined attributes to shapefile attributes
            var predefinedAttributes = new[] { ZoneFields.Name, ZoneFields.ProductA, ZoneFields.ProductB, ZoneFields.ProductC, ZoneFields.ProductD, ZoneFields.Color };

            dgvMapping.Rows.Clear();

            // Auto-match: try to find a shapefile attribute with the same name (case-insensitive)
            foreach (var predefined in predefinedAttributes)
            {
                string matched = shapefileAttributes
                    .FirstOrDefault(attr => string.Equals(attr, predefined, StringComparison.OrdinalIgnoreCase)) ?? string.Empty;

                dgvMapping.Rows.Add(predefined, matched);
            }
        }

        private void SelectShapefile(DataGridView DGV)
        {
            using (var ofd = new OpenFileDialog { Title = "Open shape file.", Filter = "Shapefiles (*.shp)|*.shp" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    selectedShapefilePath = ofd.FileName;
                    tbName.Text = Path.GetFileNameWithoutExtension(selectedShapefilePath);
                    tbName.SelectionStart = tbName.Text.Length;
                    tbName.SelectionLength = 0;
                    tbName.ScrollToCaret();
                    LoadShapefileAttributes(DGV);
                }
                else
                {
                    Close();
                }
            }
        }

        private void SetLanguage()
        {
            dgvMapping.Columns[0].HeaderText = Lang.lgZoneAttributes;
            dgvMapping.Columns[1].HeaderText = Lang.lgShapefileAttributes;
        }

        private void tbMinZoneSize_Enter(object sender, EventArgs e)
        {
            double.TryParse(tbMinZoneSize.Text, out double current);
            using (var form = new AgOpenGPS.FormNumeric(0, 50, current))
            {
                form.Text = string.Format("Min zone size ({0})", lbArea.Text);
                if (form.ShowDialog() == DialogResult.OK)
                    tbMinZoneSize.Text = form.ReturnValue.ToString("N1");
            }
        }

        private void tbNumZones_Enter(object sender, EventArgs e)
        {
            double.TryParse(tbNumZones.Text, out double current);
            using (var form = new AgOpenGPS.FormNumeric(2, 8, current))
            {
                form.Text = "Number of zones (2-8)";
                if (form.ShowDialog() == DialogResult.OK)
                    tbNumZones.Text = ((int)form.ReturnValue).ToString();
            }
        }
    }
}
