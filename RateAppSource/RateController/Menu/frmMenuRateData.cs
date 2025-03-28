using RateController.Classes;
using System;
using System.Drawing;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace RateController.Forms
{
    public partial class frmMenuRateData : Form
    {
        private bool cEdited = false;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private int MaxRecordSeconds = 30;
        private int MaxRefreshSeconds = 30;
        private FormStart mf;
        private int MinRecordSeconds = 1;
        private int MinRefreshSeconds = 1;

        public frmMenuRateData(FormStart main, frmMenu menu)
        {
            Initializing = true;
            InitializeComponent();
            Props.UnitsChanged += Props_UnitsChanged;
            MainMenu = menu;
            mf = main;
            this.Tag = false;

            HSRecordInterval.Minimum = MinRecordSeconds;
            HSRecordInterval.Maximum = MaxRecordSeconds;

            HSrefreshMap.Minimum = MinRefreshSeconds;
            HSrefreshMap.Maximum = MaxRefreshSeconds;
            timer1.Enabled = true;
            lbDataPoints.Text = mf.Tls.RateCollector.DataPoints.ToString("N0");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var Hlp = new frmMsgBox(mf, "Confirm Delete all job data?", "Delete File", true);
            Hlp.TopMost = true;

            Hlp.ShowDialog();
            bool Result = Hlp.Result;
            Hlp.Close();
            if (Result)
            {
                mf.Tls.RateCollector.ClearReadings();
            }
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

                    Props.RateRecordEnabled = ckRecord.Checked;
                    Props.RateRecordInterval = HSRecordInterval.Value;

                    Props.RateDisplayRefresh = HSrefreshMap.Value;
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
                    Props.RaiseRateDataSettingsChanged();
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("frmRates/btnOK_Click: " + ex.Message);
                }
            }
        }

        private void frmMenuRateData_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer1.Enabled = false;
        }

        private void frmRates_Load(object sender, EventArgs e)
        {
            SubMenuLayout.SetFormLayout(this,MainMenu,btnOK);

            btnCancel.Left = btnOK.Left - SubMenuLayout.ButtonSpacing;
            btnCancel.Top = btnOK.Top;
            MainMenu.StyleControls(this);
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            PositionForm();
            SetLanguage();
            UpdateForm();

            Font ValFont = new Font(lbRefresh.Font.FontFamily, 14, FontStyle.Bold);
            lbRefresh.Font = ValFont;
            lbResolution.Font = ValFont;
            lbRecordInterval.Font = ValFont;
        }

        private void gbMap_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            Props.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }

        private void HSresolution_Scroll(object sender, ScrollEventArgs e)
        {
            UpdateControlDisplay();
            SetButtons(true);
        }

        private void MainMenu_MenuMoved(object sender, EventArgs e)
        {
            PositionForm();
        }

        private void PositionForm()
        {
            this.Top = MainMenu.Top + SubMenuLayout.TopOffset;
            this.Left = MainMenu.Left + SubMenuLayout.LeftOffset;
        }

        private void Props_UnitsChanged(object sender, EventArgs e)
        {
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            lbDataPoints.Text = mf.Tls.RateCollector.DataPoints.ToString("N0");
        }

        private void UpdateControlDisplay()
        {
            lbRefresh.Text = HSrefreshMap.Value.ToString("N0");
            if (Props.UseMetric)
            {
                lbResolution.Text = (HSresolution.Value * 0.020234).ToString("N2");
            }
            else
            {
                lbResolution.Text = (HSresolution.Value * 0.05).ToString("N2");
            }
            lbRecordInterval.Text = HSRecordInterval.Value.ToString("N0");
        }

        private void UpdateForm()
        {
            try
            {
                Initializing = true;
                rbApplied.Checked = (Props.RateDisplayType == RateType.Applied);
                rbTarget.Checked = !rbApplied.Checked;
                ckRecord.Checked = Props.RateRecordEnabled;
                HSrefreshMap.Value = Props.RateDisplayRefresh;
                HSresolution.Value = Props.RateDisplayResolution;
                HSRecordInterval.Value = Props.RateRecordInterval;

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