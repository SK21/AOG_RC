using RateController.Classes;
using RateController.Language;
using RateController.RateMap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        private string[] _xmlLayerNames;

        public frmImport()
        {
            InitializeComponent();
            dgvMapping.AutoGenerateColumns = false;
            dgvMapping.AllowUserToAddRows = false;
            rbShapefile.CheckedChanged += new EventHandler(rbMode_CheckedChanged);
            rbXML.CheckedChanged += new EventHandler(rbMode_CheckedChanged);
        }

        private async void btnBuild_Click(object sender, EventArgs e)
        {
            try
            {
                if (rbXML.Checked)
                {
                    await SelectXmlFileAsync();
                }
                else if (!string.IsNullOrEmpty(selectedShapefilePath))
                {
                    BuildFromShapefile();
                }
                else
                {
                    SelectShapefile(dgvMapping);
                    if (!string.IsNullOrEmpty(selectedShapefilePath))
                        BuildFromShapefile();
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
                    if (rbShapefile.Checked)
                        RebuildMapZones();

                    int.TryParse(tbNumZones.Text, out int zoneCount);
                    double.TryParse(tbMinZoneSize.Text, out double enteredArea);
                    double minZoneHa = Props.UseMetric ? enteredArea : enteredArea * 0.404686;
                    double.TryParse(tbStep.Text, out double minRateStep);
                    Props.SetProp("ImportMinRateStep", tbStep.Text);
                    Props.SetProp("ImportMinZoneSize", tbMinZoneSize.Text);

                    string primaryProduct = ZoneFields.Products[Math.Max(0, Math.Min(ZoneFields.Products.Length - 1, cboProduct.SelectedIndex))];
                    if (rbXML.Checked)
                    {
                        var xmlHelper = new XmlHelper();
                        _simplifiedZones = xmlHelper.SimplifyZones(_mapZones, zoneCount, minZoneHa, minRateStep, primaryProduct);
                    }
                    else
                    {
                        var helper = new ShapefileHelper();
                        _simplifiedZones = helper.SimplifyPrescriptionGrid(_mapZones, zoneCount, minZoneHa, minRateStep, primaryProduct);
                    }

                    int distinctZones = _simplifiedZones
                        .Select(z => string.Join("|", ZoneFields.Products.Select(p => z.Rates.TryGetValue(p, out double v) ? ((long)Math.Round(v * 10)).ToString() : "0")))
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
                    if (rbShapefile.Checked && _simplifiedZones == null)
                        RebuildMapZones();

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
            cboProduct.Items.AddRange(new object[] { "A", "B", "C", "D", "E" });
            cboProduct.SelectedIndex = 0;
            SetLanguage();
            UpdateModeUI();
        }

        private void rbMode_CheckedChanged(object sender, EventArgs e)
        {
            selectedShapefilePath = string.Empty;
            _mapZones = null;
            _simplifiedZones = null;
            _xmlLayerNames = null;
            UpdateModeUI();
        }

        private void UpdateModeUI()
        {
            bool xmlMode = rbXML.Checked;
            dgvMapping.Enabled = !xmlMode;
            dgvMapping.Rows.Clear();
            dgvMapping.Columns[1].HeaderText = xmlMode ? "Product Name" : Lang.lgShapefileAttributes;
            if (xmlMode && _xmlLayerNames != null)
                ShowXmlLayerNames();
        }

        private void AutoSelectProduct()
        {
            if (_mapZones == null || _mapZones.Count == 0) return;
            string best = ShapefileHelper.DetectPrimaryProduct(_mapZones);
            int idx = Array.IndexOf(ZoneFields.Products, best);
            if (idx >= 0 && idx < cboProduct.Items.Count)
                cboProduct.SelectedIndex = idx;
        }

        private void ShowXmlLayerNames()
        {
            for (int i = 0; i < _xmlLayerNames.Length && i < ZoneFields.Products.Length; i++)
            {
                string name = _xmlLayerNames[i];
                if (string.IsNullOrEmpty(name)) continue;
                int rowIdx = dgvMapping.Rows.Add(ZoneFields.Products[i]);
                var cell = (DataGridViewComboBoxCell)dgvMapping.Rows[rowIdx].Cells[1];
                cell.Items.Clear();
                cell.Items.Add(name);
                cell.Value = name;
            }
        }

        private void RebuildMapZones()
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
        }

        private void BuildFromShapefile()
        {
            RebuildMapZones();
            tbNumZones.Text = _importedZoneCount.ToString();
            AutoSelectProduct();
        }

        private async Task SelectXmlFileAsync()
        {
            string path;
            using (var ofd = new OpenFileDialog { Title = "Open prescription XML.", Filter = "XML files (*.xml)|*.xml" })
            {
                if (ofd.ShowDialog() != DialogResult.OK) return;
                path = ofd.FileName;
            }

            btnBuild.Enabled = false;
            btnAdjust.Enabled = false;
            btnSave.Enabled = false;
            var importMsg = Props.ShowMessageForm("Importing...", "Import", 60000);

            List<MapZone> zones = null;
            string[] layerNames = null;
            try
            {
                (zones, layerNames) = await Task.Run(() =>
                {
                    var z = AgGrowXmlParser.Parse(path, out string[] names);
                    return (z, names);
                });
            }
            finally
            {
                if (!importMsg.IsDisposed) importMsg.Close();
                btnBuild.Enabled = true;
                btnAdjust.Enabled = true;
                btnSave.Enabled = true;
            }

            _mapZones = zones;
            _xmlLayerNames = layerNames;
            _simplifiedZones = null;
            _importedZoneCount = _mapZones?.Count ?? 0;
            tbNumZones.Text = _importedZoneCount.ToString();
            tbName.Text = Path.GetFileNameWithoutExtension(path);
            dgvMapping.Rows.Clear();
            ShowXmlLayerNames();
            AutoSelectProduct();
        }

        private void LoadShapefileAttributes(DataGridView DGV)
        {
            var shapefileHelper = new ShapefileHelper();
            var shapefileAttributes = shapefileHelper.GetShapefileAttributes(selectedShapefilePath);
            var numericVarying = shapefileHelper.GetNumericVaryingAttributes(selectedShapefilePath);

            if (DGV.Columns["ShapefileAttribute"] is DataGridViewComboBoxColumn bx)
            {
                bx.Items.Clear();
                bx.Items.AddRange(shapefileAttributes.ToArray());
            }

            var predefinedAttributes = new[] { ZoneFields.Name, ZoneFields.ProductA, ZoneFields.ProductB, ZoneFields.ProductC, ZoneFields.ProductD, ZoneFields.Color };
            var productFields = new HashSet<string> { ZoneFields.ProductA, ZoneFields.ProductB, ZoneFields.ProductC, ZoneFields.ProductD };
            var claimed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            DGV.Rows.Clear();

            foreach (var predefined in predefinedAttributes)
            {
                string matched = shapefileAttributes
                    .FirstOrDefault(attr => string.Equals(attr, predefined, StringComparison.OrdinalIgnoreCase)) ?? string.Empty;

                if (string.IsNullOrEmpty(matched) && productFields.Contains(predefined))
                    matched = numericVarying.FirstOrDefault(attr => !claimed.Contains(attr)) ?? string.Empty;

                if (!string.IsNullOrEmpty(matched))
                    claimed.Add(matched);

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
            using (var form = new AgOpenGPS.FormNumeric(2, 20, current))
            {
                form.Text = "Number of zones (2-20)";
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
