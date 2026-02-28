using AgOpenGPS;
using RateController.Classes;
using RateController.Language;
using System;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmResetQuantity : Form
    {
        private bool FormEdited = false;
        private bool Initializing = false;
        private clsProduct Prd;

        public frmResetQuantity()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            tbQuantity.Text = Prd.CurrentTankAmount.ToString("N1");
            ckFillTank.Checked = false;
            ckReset.Checked = false;
            SetButtons(false);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (ckReset.Checked) Prd.ResetApplied();

                Prd.CurrentTankAmount = double.TryParse(tbQuantity.Text, out double nv) ? nv : Prd.TankSize;

                SetButtons(false);
                this.Close();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmResetQuantity/btnOK: " + ex.Message);
            }
        }

        private void ckFillTank_CheckedChanged(object sender, EventArgs e)
        {
            if (ckFillTank.Checked) tbQuantity.Text = Prd.TankSize.ToString("N1");
            SetButtons(true);
        }

        private void ckReset_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void frmResetQuantity_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
        }

        private void frmResetQuantity_Load(object sender, EventArgs e)
        {
            Props.LoadFormLocation(this);
            this.BackColor = Properties.Settings.Default.MainBackColour;
            SetLanguage();

            Prd = Core.Products.Item(Props.CurrentProduct);
            tbQuantity.Text = Prd.CurrentTankAmount.ToString("N1");
            SetButtons(false);
        }

        private void SetButtons(bool Edited = false)
        {
            if (!Initializing)
            {
                if (Edited)
                {
                    btnCancel.Enabled = true;
                    btnOK.Image = Properties.Resources.Save;
                }
                else
                {
                    btnCancel.Enabled = false;
                    btnOK.Image = Properties.Resources.OK;
                }
                FormEdited = Edited;
            }
        }

        private void SetLanguage()
        {
            ckReset.Text = Lang.lgResetApplied;
        }

        private void tbQuantity_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbQuantity.Text, out tempD);
            using (var form = new FormNumeric(0, 100000, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbQuantity.Text = form.ReturnValue.ToString("N1");
                }
            }
        }

        private void tbQuantity_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }
    }
}