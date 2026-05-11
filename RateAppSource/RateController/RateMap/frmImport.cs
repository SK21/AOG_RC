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
            try
            {
                Job JB = JobManager.CurrentJob;
                if (JB != null && JB.FieldID >= 0)
                {
                    string Fname = tbName.Text;
                    Fname = Path.GetFileName(Fname);
                    if (FileNameValidator.IsValidFileName(Fname))
                    {
                        ParcelManager.EnsureFieldFolders(JB.FieldID);
                        JobManager.SetActivePrescription(JB.ID, Fname + ".shp");
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

                    Job JB = JobManager.CurrentJob;
                    string MP = Path.Combine(ParcelManager.MapsFolder(JB.FieldID), JB.ActivePrescription);
                    var shapefileHelper = new ShapefileHelper();
                    var mapZones = shapefileHelper.CreateZoneList(selectedShapefilePath, attributeMapping);

                    if (mapZones.Count > 100)
                    {
                        var answer = MessageBox.Show(
                            string.Format("This prescription map has {0} zones, which may be unworkable in the field.\n\nSimplify to a smaller number of zones?", mapZones.Count),
                            "Simplify Prescription Map",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (answer == DialogResult.Yes)
                        {
                            int zoneCount = 5;
                            double minZoneHa = 1.0;

                            using (var numForm = new AgOpenGPS.FormNumeric(2, 8, 5))
                            {
                                numForm.Text = "Number of zones (2-8)";
                                if (numForm.ShowDialog() != DialogResult.OK) goto skipSimplify;
                                zoneCount = (int)numForm.ReturnValue;
                            }

                            string areaUnit = Props.UseMetric ? "Ha" : "Ac";
                            using (var areaForm = new AgOpenGPS.FormNumeric(0, 50, 1))
                            {
                                areaForm.Text = string.Format("Minimum zone size ({0})", areaUnit);
                                if (areaForm.ShowDialog() == DialogResult.OK)
                                {
                                    double enteredArea = areaForm.ReturnValue;
                                    minZoneHa = Props.UseMetric ? enteredArea : enteredArea * 0.404686;
                                }
                            }

                            mapZones = shapefileHelper.SimplifyPrescriptionGrid(mapZones, zoneCount, minZoneHa);
                            skipSimplify:;
                        }
                    }

                    if (shapefileHelper.SaveMapZones(MP, mapZones))
                    {
                        MapController.LoadMap();
                        Result = true;
                        JobManager.SetActivePrescription(JB.ID, Path.GetFileName(MP));
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
    }
}