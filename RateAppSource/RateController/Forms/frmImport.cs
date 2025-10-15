using RateController.Classes;
using RateController.Language;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RateController.Forms
{
    public partial class frmImport : Form
    {
        private Dictionary<string, string> attributeMapping;
        private FormStart mf;
        private string selectedShapefilePath;

        public frmImport(FormStart CallingForm)
        {
            InitializeComponent();
            mf = CallingForm;
            dgvMapping.AutoGenerateColumns = false;
            dgvMapping.AllowUserToAddRows = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var Hlp = new frmMsgBox(mf, "Confirm replace current map with imported map?", "Import File", true);
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

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            SelectShapefile(dgvMapping);
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
        }

        private void LoadShapefileAttributes(DataGridView dgvMapping)
        {
            var shapefileHelper = new ShapefileHelper(mf);
            var shapefileAttributes = shapefileHelper.GetShapefileAttributes(selectedShapefilePath);

            // add shapefile attribute names
            if (dgvMapping.Columns["ShapefileAttribute"] is DataGridViewComboBoxColumn bx)
            {
                bx.Items.Clear();
                bx.Items.AddRange(shapefileAttributes.ToArray());
            }

            // Map predefined attributes to shapefile attributes
            var predefinedAttributes = new[] { "Name", "ProductA", "ProductB", "ProductC", "ProductD", "Color" };

            dgvMapping.Rows.Clear();

            // Create a ComboBox for each predefined attribute
            foreach (var predefined in predefinedAttributes)
            {
                // Create a new row in the DataGridView
                int rowIndex = dgvMapping.Rows.Add(predefined, string.Empty); // Add a row with the predefined attribute name
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

                    var shapefileHelper = new ShapefileHelper(mf);
                    var mapZones = shapefileHelper.CreateZoneList(selectedShapefilePath, attributeMapping);

                    string MapPath = JobManager.MapPath(Props.CurrentJobID);
                    if (shapefileHelper.SaveMapZones(MapPath, mapZones))
                    {
                        mf.Tls.Manager.LoadMap();
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
            using (var ofd = new OpenFileDialog { Filter = "Shapefiles (*.shp)|*.shp" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    selectedShapefilePath = ofd.FileName;
                    LoadShapefileAttributes(DGV);
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