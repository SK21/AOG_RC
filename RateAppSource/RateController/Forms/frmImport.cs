using RateController.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RateController.Forms
{
    public partial class frmImport : Form
    {
        private FormStart mf;
        private string selectedShapefilePath;
        private Dictionary<string, string> attributeMapping;
        public frmImport(FormStart CallingForm)
        {
            InitializeComponent();
            mf= CallingForm;
            dgvMapping.AutoGenerateColumns = false;
            dgvMapping.AllowUserToAddRows = false;
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            SelectShapefile(dgvMapping);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveCrossReferencedShapefile();
        }
        private void SelectShapefile(DataGridView dgvMapping)
        {
            using (var ofd = new OpenFileDialog { Filter = "Shapefiles (*.shp)|*.shp" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    selectedShapefilePath = ofd.FileName;
                    LoadShapefileAttributes(dgvMapping);
                }
            }
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
            var predefinedAttributes = new[] { "Name", "ProductA", "ProductB", "ProductC", "ProductD", "Color" };

            dgvMapping.Rows.Clear();

            // Create a ComboBox for each predefined attribute
            foreach (var predefined in predefinedAttributes)
            {
                // Create a new row in the DataGridView
                int rowIndex = dgvMapping.Rows.Add(predefined, string.Empty); // Add a row with the predefined attribute name
            }
        }
        private void SaveCrossReferencedShapefile()
        {
            if (string.IsNullOrEmpty(selectedShapefilePath))
            {
                MessageBox.Show("Please select a shapefile first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            attributeMapping = new Dictionary<string, string>();
            foreach (DataGridViewRow row in ((DataGridView)Controls[0]).Rows)
            {
                var predefined = row.Cells["PredefinedAttribute"].Value?.ToString();
                var shapefileAttribute = row.Cells["ShapefileAttribute"].Value?.ToString();

                if (!string.IsNullOrEmpty(predefined) && !string.IsNullOrEmpty(shapefileAttribute))
                {
                    attributeMapping[predefined] = shapefileAttribute;
                }
            }

            var shapefileHelper = new ShapefileHelper();
            var mapZones = shapefileHelper.LoadAndMapShapefile(selectedShapefilePath, attributeMapping);

            using (var sfd = new SaveFileDialog { Filter = "Shapefiles (*.shp)|*.shp" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    if (shapefileHelper.SaveMapZones(sfd.FileName, mapZones))
                    {
                        MessageBox.Show("Cross-referenced shapefile saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to map attributes. File not saved.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

        private void frmImport_FormClosed(object sender, FormClosedEventArgs e)
        {
            mf.Tls.SaveFormData(this);
        }

        private void frmImport_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            this.BackColor = Properties.Settings.Default.MainBackColour;
        }
    }
}
