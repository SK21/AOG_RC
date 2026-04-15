using AgOpenGPS;
using RateController.Classes;
using System;
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
        private int ZonesCreated = 0;
        private int ZonesToCreate = 5;

        public frmCreateZones()
        {
            InitializeComponent();
        }

        private void btnBuild_Click(object sender, EventArgs e)
        {
            if (Build())
            {
                Props.ShowMessage(ZonesCreated.ToString("N0 ") + " zones created.");
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
        }

        private void EnableBuild()
        {
            // todo check at least one of yield, ec, or elevation is selected
            // check at least one zone and grid size > 0

            btnBuild.Enabled = (TotalWeight == 100 && !cEdited);
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

                foreach (var file in Directory.GetFiles(YieldFolder))
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

            tbWeightYield.Text = WeightYield.ToString("N0");
            tbWeightEC.Text = WeightEC.ToString("N0");
            tbWeightElevation.Text = WeightElevation.ToString("N0");
            tbNumZones.Text = ZonesToCreate.ToString("N0");
            tbMinZoneSize.Text = GridSize.ToString("N0");

            if (Props.UseMetric)
            {
                lbArea.Text = "Ha";
            }
            else
            {
                lbArea.Text = "Ac";
            }

            EnableBuild();

            Initializing = false;
        }

        private void UpdateWeightTotal()
        {
            TotalWeight = int.TryParse(tbWeightYield.Text, out int yw) ? yw : 0;
            TotalWeight += int.TryParse(tbWeightEC.Text, out int ec) ? ec : 0;
            TotalWeight += int.TryParse(tbWeightElevation.Text, out int el) ? el : 0;
            lbTotal.Text = TotalWeight.ToString("N0");
            EnableBuild();
        }

        #region build zones

        private bool Build()
        {
            bool Result = false;

            return Result;
        }

        #endregion build zones
    }
}