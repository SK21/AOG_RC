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
        private double TankSize = 0;

        public frmResetQuantity()
        {
            InitializeComponent();
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
                if (ckReset.Checked)
                {
                    Core.Products.Item(Props.CurrentProduct).ResetApplied();
                }

                double NewValue = double.TryParse(tbQuantity.Text, out double nv) ? nv : TankSize;
                if (NewValue > TankSize) NewValue = TankSize;
                Core.Products.Item(Props.CurrentProduct).TankStart = NewValue;

                SetButtons(false);
                UpdateForm();
                this.Close();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmResetQuantity/btnOK: " + ex.Message);
            }
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
            TankSize = Core.Products.Item(Props.CurrentProduct).TankSize;
            this.BackColor = Properties.Settings.Default.MainBackColour;
            UpdateForm();
            SetLanguage();
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
                    tbQuantity.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void tbQuantity_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
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
            Initializing = false;
        }
    }
}