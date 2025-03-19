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
    public partial class frmRates : Form
    {
        private bool cEdited = false;
        private bool Initializing = false;

        public frmRates()
        {
            InitializeComponent();
        }

        private void frmRates_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
        }

        private void frmRates_Load(object sender, EventArgs e)
        {
            Props.LoadFormLocation(this);
            this.BackColor = Properties.Settings.Default.BackColour;
            SetLanguage();
            UpdateForm();
        }

        private void rbProductA_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void SetButtons(bool Edited)
        {
            if (!Initializing)
            {
                if (Edited)
                {
                    btnCancel.Enabled = true;
                    btnOK.Enabled = true;
                }
                else
                {
                    btnCancel.Enabled = false;
                    btnOK.Enabled = false;
                }

                cEdited = Edited;
                this.Tag = cEdited;
            }
        }

        private void SetLanguage()
        {
            if (Props.UseMetric)
            {
                lbResolutionDescription.Text = "Resolution (Hectares)";
            }
            else
            {
                lbResolutionDescription.Text = "Resolution (Acres)";
            }
        }

        private void UpdateControlDisplay()
        {
            lbRefresh.Text = HSrefresh.Value.ToString("N0");
            if(Props.UseMetric)
            {
                lbResolution.Text = (HSresolution.Value * 0.020234).ToString("N2");
            }
            else
            {
                lbResolution.Text = (HSresolution.Value * 0.05).ToString("N2");
            }
        }

        private void UpdateForm()
        {
            try
            {
                Initializing = true;
                rbApplied.Checked = (Props.RateDisplayType == RateType.Applied);
                ckRecord.Checked = Props.RecordRates;
                ckDisplayRates.Checked = Props.RateDisplayShow;
                HSrefresh.Value = Props.RateDisplayRefresh;
                HSresolution.Value = Props.RateDisplayResolution;

                switch (Props.RateDisplayProduct)
                {
                    case 1:
                        rbProductB.Checked = true;
                        break;

                    case 2:
                        rbProductC.Checked = true;
                        break;

                    case 3:
                        rbProductD.Checked = true;
                        break;

                    default:
                        rbProductA.Checked = true;
                        break;
                }

                UpdateControlDisplay();
                SetLanguage();

                Initializing = false;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmRates/UpdateForm: " + ex.Message);
            }
        }
    }
}