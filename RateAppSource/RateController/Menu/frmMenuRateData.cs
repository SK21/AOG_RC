using RateController.Classes;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace RateController.Forms
{
    public partial class frmMenuRateData : Form
    {
        private bool cEdited = false;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private FormStart mf;

        public frmMenuRateData(FormStart main, frmMenu menu)
        {
            Initializing = true;
            InitializeComponent();
            Props.UnitsChanged += Props_UnitsChanged;
            MainMenu = menu;
            mf = main;
            this.Tag = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Props.CurrentDir();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!Props.OpenRateDataFile(openFileDialog1.FileName))
                {
                    mf.Tls.ShowMessage("Could not open file.");
                }
                tbRateDataFile.Text = Props.CurrentRateDataFile();
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = Props.CurrentDir();
            saveFileDialog1.Title = "New File";
            saveFileDialog1.FileName = Props.CurrentMapName + "_RateData_" + DateTime.Now.ToString("dd-MMM-yy");
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                    Props.OpenRateDataFile(saveFileDialog1.FileName, true);
                    tbRateDataFile.Text = Props.CurrentRateDataFile();
                }
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

                    Props.RecordRates = ckRecord.Checked;
                    Props.RateDisplayRefresh = HSrefreshMap.Value;
                    Props.RateDisplayResolution = HSresolution.Value;
                    Props.RateRecordInterval = HSRecordInterval.Value;

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
                    mf.Tls.Manager.UpdateRateMapDisplay();
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("frmRates/btnOK_Click: " + ex.Message);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = Props.CurrentDir();
            saveFileDialog1.Title = "Save As";
            saveFileDialog1.FileName = tbRateDataFile.Text;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                    Props.OpenRateDataFile(saveFileDialog1.FileName, true);
                    tbRateDataFile.Text = Props.CurrentRateDataFile();
                }
            }
        }

        private void frmRates_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
        }

        private void frmRates_Load(object sender, EventArgs e)
        {
            // menu 800,600
            // sub menu 540,630
            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            this.BackColor = Properties.Settings.Default.MainBackColour;
            this.Width = MainMenu.Width - 260;
            this.Height = MainMenu.Height - 50;
            btnOK.Left = this.Width - 84;
            btnOK.Top = this.Height - 84;
            btnCancel.Left = btnOK.Left - 78;
            btnCancel.Top = btnOK.Top;
            MainMenu.StyleControls(this);
            PositionForm();
            UpdateForm();

            Font ValFont = new Font(lbFileName.Font.FontFamily, 14, FontStyle.Bold);
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
            this.Top = MainMenu.Top + 30;
            this.Left = MainMenu.Left + 246;
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
                ckRecord.Checked = Props.RecordRates;
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
                tbRateDataFile.Text = Props.CurrentRateDataFile();

                Initializing = false;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmRates/UpdateForm: " + ex.Message);
            }
        }
    }
}