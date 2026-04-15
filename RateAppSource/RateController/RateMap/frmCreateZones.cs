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
        private bool HighLighting = false;
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
            HighLighting = true;
            AllYields = !AllYields;
            for (int i = 0; i < ckLBYields.Items.Count; i++)
            {
                ckLBYields.SetItemChecked(i, AllYields);
            }
            HighLighting = false;
        }

        private void ckEC_CheckedChanged(object sender, EventArgs e)
        {
            EnableWeightBoxes();
        }

        private void ckLBYields_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = ckLBYields.SelectedIndex;
            if (index >= 0)
            {
                // Optional: keep checkbox in sync when clicking text
                ckLBYields.SetItemChecked(index, !ckLBYields.GetItemChecked(index));
            }
            EnableWeightBoxes();
        }

        private void EnableBuild()
        {
            btnBuild.Enabled = (TotalWeight == 100 && !cEdited && ZonesToCreate > 0 && GridSize > 0);
        }

        private void EnableWeightBoxes()
        {
            tbWeightYield.Enabled = ckLBYields.SelectedIndex > -1;
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

            tbWeightYield.Text = WeightYield.ToString("0");
            tbWeightEC.Text = WeightEC.ToString("N0");
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

            if (ckLBYields.SelectedIndex > -1) TotalWeight += int.TryParse(tbWeightYield.Text, out int yw) ? yw : 0;
            if (ckEC.Checked) TotalWeight += int.TryParse(tbWeightEC.Text, out int ec) ? ec : 0;
            if (ckElevation.Checked) TotalWeight += int.TryParse(tbWeightElevation.Text, out int el) ? el : 0;

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

        private void ckLBYields_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!HighLighting)
            {
                // Delay selection update until after check state changes
                this.BeginInvoke((MethodInvoker)(() =>
                {
                    ckLBYields.SelectedIndex = e.Index;
                }));
            }
        }
    }
}