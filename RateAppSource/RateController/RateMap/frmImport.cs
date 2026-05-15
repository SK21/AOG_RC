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
        private List<MapZone> _simplifiedZones;
        private int _importedZoneCount;

        public frmImport()
        {
            InitializeComponent();
            dgvMapping.AutoGenerateColumns = false;
            dgvMapping.AllowUserToAddRows = false;
            rbShapefile.CheckedChanged += new EventHandler(rbMode_CheckedChanged);
            rbXML.CheckedChanged += new EventHandler(rbMode_CheckedChanged);
        }

        private void btnBuild_Click(object sender, EventArgs e)
        {
            try
            {
                if (rbXML.Checked)
                {
                    SelectXmlFile();
                }
                else if (!string.IsNullOrEmpty(selectedShapefilePath))
                {
                    BuildFromShapefile();
                }
                else
                {
                    SelectShapefile(dgvMapping);
                }
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

        private void btnAdjust_Click(object sender, EventArgs e)
        {
            try
            {
                if (_mapZones == null)
                {
                    Props.ShowMessage("Import data first (Step 1).");
                }
                else
                {
                    int.TryParse(tbNumZones.Text, out int zoneCount);
                    double.TryParse(tbMinZoneSize.Text, out double enteredArea);
                    double minZoneHa = Props.UseMetric ? enteredArea : enteredArea * 0.404686;
                    double.TryParse(tbStep.Text, out double minRateStep);
                    Props.SetProp("ImportMinRateStep", tbStep.Text);
                    Props.SetProp("ImportMinZoneSize", tbMinZoneSize.Text);

                    var helper = new ShapefileHelper();
                    _simplifiedZones = helper.SimplifyPrescriptionGrid(_mapZones, zoneCount, minZoneHa, minRateStep);

                    int distinctZones = _simplifiedZones
                        .Select(z => z.Rates[ZoneFields.ProductA])
                        .Distinct().Count();
                    Props.ShowMessage(string.Format("{0} zones created.", distinctZones));
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmImport/btnAdjust_Click: " + ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (_mapZones == null)
                {
                    Props.ShowMessage("Import data first (Step 1).");
                }
                else
                {
                    Job JB = JobManager.CurrentJob;
                    if (JB != null && JB.FieldID >= 0)
                    {
                        string Fname = Path.GetFileName(tbName.Text);
                        if (FileNameValidator.IsValidFileName(Fname))
                        {
                            ParcelManager.EnsureFieldFolders(JB.FieldID);
                            JobManager.SetActivePrescription(JB.ID, Fname + ".shp");
                            string MP = Path.Combine(ParcelManager.MapsFolder(JB.FieldID), JB.ActivePrescription);

                            var helper = new ShapefileHelper();
                            var zonesToSave = _simplifiedZones ?? _mapZones;

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
            lbAreaStep.Text = lbArea.Text;
            tbStep.Text = Props.GetProp("ImportMinRateStep").Length > 0 ? Props.GetProp("ImportMinRateStep") : "5";
            tbMinZoneSize.Text = Props.GetProp("ImportMinZoneSize").Length > 0 ? Props.GetProp("ImportMinZoneSize") : "0";
            SetLanguage();
            UpdateModeUI();
        }

        private void rbMode_CheckedChanged(object sender, EventArgs e)
        {
            selectedShapefilePath = string.Empty;
            _mapZones = null;
            _simplifiedZones = null;
            UpdateModeUI();
        }

        private void UpdateModeUI()
        {
            dgvMapping.Enabled = rbShapefile.Checked;
        }

        private void BuildFromShapefile()
        {
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
            _simplifiedZones = null;
            _importedZoneCount = _mapZones.Count;
            tbNumZones.Text = _importedZoneCount.ToString();
        }

        private void SelectXmlFile()
        {
            using (var ofd = new OpenFileDialog { Title = "Open prescription XML.", Filter = "XML files (*.xml)|*.xml" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _mapZones = AgGrowXmlParser.Parse(ofd.FileName);
                    _simplifiedZones = null;
                    _importedZoneCount = _mapZones.Count;
                    tbNumZones.Text = _importedZoneCount.ToString();
                    tbName.Text = Path.GetFileNameWithoutExtension(ofd.FileName);
                }
            }
        }

        private void LoadShapefileAttributes(DataGridView DGV)
        {
            var shapefileHelper = new ShapefileHelper();
            var shapefileAttributes = shapefileHelper.GetShapefileAttributes(selectedShapefilePath);

            if (DGV.Columns["ShapefileAttribute"] is DataGridViewComboBoxColumn bx)
            {
                bx.Items.Clear();
                bx.Items.AddRange(shapefileAttributes.ToArray());
            }

            var predefinedAttributes = new[] { ZoneFields.Name, ZoneFields.ProductA, ZoneFields.ProductB, ZoneFields.ProductC, ZoneFields.ProductD, ZoneFields.Color };

            DGV.Rows.Clear();

            foreach (var predefined in predefinedAttributes)
            {
                string matched = shapefileAttributes
                    .FirstOrDefault(attr => string.Equals(attr, predefined, StringComparison.OrdinalIgnoreCase)) ?? string.Empty;
                DGV.Rows.Add(predefined, matched);
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

        private void tbStep_Enter(object sender, EventArgs e)
        {
            double.TryParse(tbStep.Text, out double current);
            using (var form = new AgOpenGPS.FormNumeric(0, 9999, current))
            {
                form.Text = "Min rate step between zones";
                if (form.ShowDialog() == DialogResult.OK)
                    tbStep.Text = form.ReturnValue.ToString("N0");
            }
        }
    }
}
