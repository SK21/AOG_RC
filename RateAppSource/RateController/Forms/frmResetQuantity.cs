using AgOpenGPS;
using System;
using System.Windows.Forms;
using RateController.Language;

namespace RateController
{
    public partial class frmResetQuantity : Form
    {
        private bool FormEdited = false;
        private bool Initializing = false;
        private FormStart mf;
        private double TankSize = 0;

        public frmResetQuantity(FormStart CallingForm)
        {
            InitializeComponent();
            mf = CallingForm;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (FormEdited)
                {
                    // save data
                    if (ckReset.Checked)
                    {
                        mf.Products.Item(mf.CurrentProduct()).ResetApplied();
                    }

                    if (ckFill.Checked)
                    {
                        if (double.TryParse(tbQuantity.Text, out double qu)) mf.Products.Item(mf.CurrentProduct()).TankStart = qu;
                    }
                    SetButtons(false);
                    UpdateForm();
                }
                else
                {
                    this.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void ckReset_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void frmResetQuantity_FormClosed(object sender, FormClosedEventArgs e)
        {
            mf.Tls.SaveFormData(this);
        }

        private void frmResetQuantity_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            TankSize = mf.Products.Item(mf.CurrentProduct()).TankSize;
            this.BackColor = Properties.Settings.Default.MainBackColour;
            UpdateForm();
            SetLanguage();
        }

        private void SetLanguage()
        {
            ckFill.Text = Lang.lgFillTank;
            ckReset.Text = Lang.lgResetApplied;
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

        private void tbQuantity_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbQuantity.Text, out tempD);
            using (var form = new FormNumeric(0, 100000, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbQuantity.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void tbQuantity_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
            if (!Initializing) ckFill.Checked = true;
        }

        private void UpdateForm()
        {
            Initializing = true;
            if (TankSize > 9999)
            {
                tbQuantity.Text = TankSize.ToString("N0");
            }
            else
            {
                tbQuantity.Text = TankSize.ToString("N1");
            }
            ckReset.Checked = false;
            ckFill.Checked = false;
            Initializing = false;
        }
    }
}