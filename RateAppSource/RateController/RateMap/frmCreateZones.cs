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

        public frmCreateZones()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (Create())
            {
                Close();
            }
            else
            {
                Props.ShowMessage("Could not create zones.");
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

        private void frmCreateZones_FormClosing(object sender, FormClosingEventArgs e)
        {
            Props.SaveFormLocation(this);
        }

        private void frmCreateZones_Load(object sender, EventArgs e)
        {
            Props.LoadFormLocation(this);
            this.BackColor = Properties.Settings.Default.MainBackColour;
            LoadYields();
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
            }
            UpdateForm();
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
            }
            UpdateForm();
        }

        private void UpdateForm()
        {
            double total = 0;
            total += double.TryParse(tbWeightYield.Text, out double yw) ? yw : 0;
            total += double.TryParse(tbWeightEC.Text, out double ec) ? ec : 0;
            total += double.TryParse(tbWeightElevation.Text, out double el) ? el : 0;
            lbTotal.Text = total.ToString("N0");
        }

        #region create zones

        private bool Create()
        {
            bool Result = false;

            return Result;
        }

        #endregion create zones
    }
}