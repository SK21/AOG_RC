using RateController.Classes;
using RateController.Language;
using RateController.RateMap;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RateController.Forms
{
    public partial class frmImport : Form
    {
        private Dictionary<string, string> attributeMapping;
        private string selectedShapefilePath;

        public frmImport()
        {
            InitializeComponent();
            dgvMapping.AutoGenerateColumns = false;
            dgvMapping.AllowUserToAddRows = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var Hlp = new frmMsgBox("Confirm replace current map with imported map?", "Import File", true);
            Hlp.TopMost = true;

            Hlp.ShowDialog();
            bool Result = Hlp.Result;
            Hlp.Close();
            if (Result)
            {
                if (SaveCrossReferencedShapefile())
                {
                    Props.ShowMessage("Cross-referenced shapefile saved successfully.");
                    this.Close();
                }
                else
                {
                    Props.ShowMessage("Failed to map attributes. File not saved.");
                }
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

                int rowIndex = dgvMapping.Rows.Add(predefined, matched);
            }
        }

        private bool SaveCrossReferencedShapefile()
        {
            bool Result = false;
            try
            {
                if (!string.IsNullOrEmpty(selectedShapefilePath))
                {
                    attributeMapping = new Dictionary<string, string>();
                    foreach (DataGridViewRow row in dgvMapping.Rows)
                    {
                        var predefined = row.Cells["PredefinedAttribute"].Value?.ToString();
                        var shapefileAttribute = row.Cells["ShapefileAttribute"].Value?.ToString();

                        if (!string.IsNullOrEmpty(predefined) && !string.IsNullOrEmpty(shapefileAttribute))
                        {
                            attributeMapping[predefined] = shapefileAttribute;
                        }
                    }

                    string MapPath = JobManager.MapPath(JobManager.CurrentJobID);

                    Dictionary<string, Color> SavedLegend = MapController.legendManager.LoadPersistedLegend(Path.ChangeExtension(selectedShapefilePath, null));
                    if (SavedLegend != null)
                    {
                        MapController.legendManager.SaveAppliedLegend(Path.ChangeExtension(MapPath, null), SavedLegend);
                    }

                    var shapefileHelper = new ShapefileHelper();
                    var mapZones = shapefileHelper.CreateZoneList(selectedShapefilePath, attributeMapping);

                    if (shapefileHelper.SaveMapZones(MapPath, mapZones))
                    {
                        MapController.LoadMap();
                        Result = true;
                    }
                }
                else
                {
                    Props.ShowMessage("Please select a shapefile first.");
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmImport/SaveCrossReferencedShapeFile: " + ex.Message);
            }
            return Result;
        }

        private void SelectShapefile(DataGridView DGV)
        {
            using (var ofd = new OpenFileDialog { Title = "Open shape file.", Filter = "Shapefiles (*.shp)|*.shp" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    selectedShapefilePath = ofd.FileName;
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
    }
}