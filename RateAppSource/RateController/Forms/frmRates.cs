using RateController.Classes;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace RateController.Forms
{
    public partial class frmRates : Form
    {
        private bool cEdited = false;
        private bool Initializing = false;
        private MapManager cMapManager;
        public frmRates()
        {
            InitializeComponent();
            Props.UnitsChanged += Props_UnitsChanged;
        }

        private void Props_UnitsChanged(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
        }
        public void SetManager(MapManager mapManager)
        {
            cMapManager= mapManager;
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cEdited)
            {
                try
                {
                    if (rbApplied.Checked)
                    {
                        Props.RateDisplayType = RateType.Applied;
                    }
                    else
                    {
                        Props.RateDisplayType = RateType.Target;
                    }

                    Props.RecordRates = ckRecord.Checked;
                    Props.RateDisplayShow = ckDisplayRates.Checked;
                    Props.RateDisplayRefresh = HSrefresh.Value;
                    Props.RateDisplayResolution = HSresolution.Value;

                    if (rbProductA.Checked)
                    {
                        Props.RateDisplayProduct = 0;
                    }
                    else if (rbProductB.Checked)
                    {
                        Props.RateDisplayProduct = 1;
                    }
                    else if (rbProductC.Checked)
                    {
                        Props.RateDisplayProduct = 2;
                    }
                    else
                    {
                        Props.RateDisplayProduct = 3;
                    }

                    SetButtons(false);
                    UpdateForm();
                    cMapManager.UpdateRateMapDisplay();
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("frmRates/btnOK_Click: " + ex.Message);
                }
            }
            else
            {
                this.Close();
            }
        }

        private void frmRates_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
        }

        private void frmRates_Load(object sender, EventArgs e)
        {
            Props.LoadFormLocation(this);
            this.BackColor = Properties.Settings.Default.MainBackColour;
            SetLanguage();
            UpdateForm();
        }

        private void HSresolution_Scroll(object sender, ScrollEventArgs e)
        {
            UpdateControlDisplay();
            SetButtons(true);
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
                    btnOK.Image = Properties.Resources.Save;
                }
                else
                {
                    btnCancel.Enabled = false;
                    btnOK.Image = Properties.Resources.OK;
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
            if (Props.UseMetric)
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
                rbTarget.Checked = !rbApplied.Checked;
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

        private void gbMap_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            Props.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }
    }
}