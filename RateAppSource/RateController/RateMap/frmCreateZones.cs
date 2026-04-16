using AgOpenGPS;
using RateController.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace RateController.RateMap
{
    public partial class frmCreateZones : Form
    {
        private bool AllYields = false;
        private bool cEdited = false;
        private int GridSize = 1;   // hectares
        private bool Initializing = false;
        private int TotalWeight = 0;
        private int WeightEC = 20;
        private int WeightElevation = 5;
        private int WeightYield = 75;
        private int ZonesToCreate = 5;

        public frmCreateZones()
        {
            InitializeComponent();
        }

        private void btnBuild_Click(object sender, EventArgs e)
        {
            if (Build())
            {
                Props.ShowMessage("Zones created.");
            }
            else
            {
                Props.ShowMessage("Could not create zones.");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SetButtons(false);
            UpdateForm();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cEdited)
            {
                SaveData();
                SetButtons(false);
                EnableBuild();
            }
            else
            {
                Close();
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            AllYields = !AllYields;
            for (int i = 0; i < ckLBYields.Items.Count; i++)
            {
                ckLBYields.SetItemChecked(i, AllYields);
            }
            EnableWeightBoxes();
        }

        private void ckEC_CheckedChanged(object sender, EventArgs e)
        {
            EnableWeightBoxes();
        }

        private void ckLBYields_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableWeightBoxes();
        }

        private void EnableBuild()
        {
            btnBuild.Enabled = (TotalWeight == 100 && !cEdited && ZonesToCreate > 0 && GridSize > 0);
        }

        private void EnableWeightBoxes()
        {
            tbWeightYield.Enabled = ckLBYields.CheckedItems.Count > 0;
            tbWeightEC.Enabled = ckEC.Checked;
            tbWeightElevation.Enabled = ckElevation.Checked;
            UpdateWeightTotal();
        }

        private void frmCreateZones_FormClosing(object sender, FormClosingEventArgs e)
        {
            Props.SaveFormLocation(this);
        }

        private void frmCreateZones_Load(object sender, EventArgs e)
        {
            Props.LoadFormLocation(this);
            this.BackColor = Properties.Settings.Default.MainBackColour;
            LoadYields();
            SetButtons(false);
            LoadData();
            UpdateForm();
            UpdateWeightTotal();
        }

        private void lbTotal_TextChanged(object sender, EventArgs e)
        {
            int total = int.TryParse(lbTotal.Text, out int ttl) ? ttl : 0;
            if (total == 100)
            {
                lbTotal.ForeColor = System.Drawing.Color.Black;
            }
            else
            {
                lbTotal.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void LoadData()
        {
            WeightYield = int.TryParse(Props.GetProp("WeightYield"), out int wy) ? wy : 75;
            WeightEC = int.TryParse(Props.GetProp("WeightEC"), out int we) ? we : 20;
            WeightElevation = int.TryParse(Props.GetProp("WeightElevation"), out int wv) ? wv : 5;
            ZonesToCreate = int.TryParse(Props.GetProp("ZonesToCreate"), out int zc) ? zc : 5;
            GridSize = int.TryParse(Props.GetProp("ZoneGridSize"), out int gs) ? gs : 1;
        }

        private void LoadYields()
        {
            try
            {
                Job JB = JobManager.CurrentJob;
                ckLBYields.Items.Clear();
                string YieldFolder = ParcelManager.YieldFolder(JB.FieldID);

                foreach (var file in Directory.GetFiles(YieldFolder, "*.csv"))
                {
                    ckLBYields.Items.Add
                    (
                        new { Name = Path.GetFileName(file), FullPath = file }
                    );
                }

                ckLBYields.DisplayMember = "Name";
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmCreateZones/LoadYields: " + ex.Message);
            }
        }

        private void NumericTextBox_Enter(object sender, EventArgs e)
        {
            if (sender is TextBox tb)
            {
                double currentValue = 0;
                double.TryParse(tb.Text, out currentValue);

                using (var form = new FormNumeric(0, 100, currentValue))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        tb.Text = form.ReturnValue.ToString("N0");
                    }
                }
                UpdateWeightTotal();
            }
        }

        private void NumericTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sender is TextBox tb)
            {
                double tempD;
                double.TryParse(tb.Text, out tempD);

                if (tempD < 0 || tempD > 100)
                {
                    System.Media.SystemSounds.Exclamation.Play();
                    e.Cancel = true;
                }
                UpdateWeightTotal();
            }
        }

        private void SaveData()
        {
            WeightYield = int.TryParse(tbWeightYield.Text, out int wy) ? wy : 0;
            WeightEC = int.TryParse(tbWeightEC.Text, out int we) ? we : 0;
            WeightElevation = int.TryParse(tbWeightElevation.Text, out int wv) ? wv : 0;
            ZonesToCreate = int.TryParse(tbNumZones.Text, out int zc) ? zc : 0;
            GridSize = int.TryParse(tbMinZoneSize.Text, out int gs) ? gs : 0;

            Props.SetProp("WeightYield", WeightYield.ToString());
            Props.SetProp("WeightEC", WeightEC.ToString());
            Props.SetProp("WeightElevation", WeightElevation.ToString());
            Props.SetProp("ZonesToCreate", ZonesToCreate.ToString());
            Props.SetProp("ZoneGridSize", GridSize.ToString());
        }

        private void SetButtons(bool Edited)
        {
            if (!Initializing)
            {
                if (Edited)
                {
                    btnCancel.Enabled = true;
                    btnOK.Image = Properties.Resources.Save;
                    btnBuild.Enabled = false;
                }
                else
                {
                    btnCancel.Enabled = false;
                    btnOK.Image = Properties.Resources.OK;
                    EnableBuild();
                }
                cEdited = Edited;
            }
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void UpdateForm()
        {
            Initializing = true;

            tbWeightYield.Text = WeightYield.ToString("0");
            tbWeightEC.Text = WeightEC.ToString("0");
            tbWeightElevation.Text = WeightElevation.ToString("0");
            tbNumZones.Text = ZonesToCreate.ToString("0");
            tbMinZoneSize.Text = GridSize.ToString("0");

            if (Props.UseMetric)
            {
                lbArea.Text = "Ha";
            }
            else
            {
                lbArea.Text = "Ac";
            }

            EnableWeightBoxes();
            EnableBuild();

            Initializing = false;
        }

        private void UpdateWeightTotal()
        {
            TotalWeight = 0;

            if (ckLBYields.CheckedItems.Count > 0) TotalWeight += int.TryParse(tbWeightYield.Text, out int yw) ? yw : 0;
            if (ckEC.Checked) TotalWeight += int.TryParse(tbWeightEC.Text, out int ec) ? ec : 0;
            if (ckElevation.Checked) TotalWeight += int.TryParse(tbWeightElevation.Text, out int el) ? el : 0;

            lbTotal.Text = TotalWeight.ToString("N0");
            EnableBuild();
        }

        #region build zones

        private bool Build()
        {
            try
            {
                Job JB = JobManager.CurrentJob;
                if (JB == null || JB.FieldID < 0) return false;
                int fieldID = JB.FieldID;

                // Collect checked yield file paths.
                // Items are anonymous objects { Name, FullPath } — use reflection to read FullPath.
                var yieldPaths = new List<string>();
                foreach (object item in ckLBYields.CheckedItems)
                {
                    string path = item.GetType().GetProperty("FullPath")?.GetValue(item)?.ToString();
                    if (!string.IsNullOrEmpty(path)) yieldPaths.Add(path);
                }

                // EC and elevation paths — only if the layer checkbox is ticked
                string ecPath = ckEC.Checked ? ParcelManager.SelectedEcPath(fieldID) : null;
                string elevationPath = ckElevation.Checked ? ParcelManager.SelectedElevationPath(fieldID) : null;

                // Validate that checked layers have a file assigned on this field
                if (ckEC.Checked && ecPath == null)
                {
                    Props.ShowMessage("No EC file selected for this field.", "Create Zones", 5000, false);
                    return false;
                }
                if (ckElevation.Checked && elevationPath == null)
                {
                    Props.ShowMessage("No elevation file selected for this field.", "Create Zones", 5000, false);
                    return false;
                }

                // Boundary KML: the job's active KML file (repurposed as field boundary)
                string boundaryKmlPath = null;
                if (!string.IsNullOrEmpty(JB.ActiveKMLfile))
                {
                    string kmlFull = Path.Combine(ParcelManager.KmlFolder(fieldID), JB.ActiveKMLfile);
                    if (File.Exists(kmlFull)) boundaryKmlPath = kmlFull;
                }

                // Convert layer weights to fractions (weights already validated to sum to 100)
                double yieldFraction = ckLBYields.CheckedItems.Count > 0 ? WeightYield / 100.0 : 0.0;
                double ecFraction = ckEC.Checked ? WeightEC / 100.0 : 0.0;
                double elevationFraction = ckElevation.Checked ? WeightElevation / 100.0 : 0.0;

                // GridSize is stored in Ha (metric) or Ac (imperial) — Generate() expects Ha
                double minZoneHa = Props.UseMetric ? GridSize : GridSize * 0.404686;

                string error = ProductivityZoneCreator.Generate(
                    yieldPaths.Count > 0 ? yieldPaths : null,
                    null,              // equal weights across yield years
                    yieldFraction,
                    ecPath,
                    ecFraction,
                    elevationPath,
                    elevationFraction,
                    ZonesToCreate,
                    minZoneHa,
                    boundaryKmlPath);

                if (!string.IsNullOrEmpty(error))
                {
                    Props.ShowMessage(error, "Create Zones", 5000, false);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmCreateZones/Build: " + ex.Message);
                return false;
            }
        }

        #endregion build zones
    }
}