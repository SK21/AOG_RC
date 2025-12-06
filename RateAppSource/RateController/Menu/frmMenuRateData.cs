using RateController.Classes;
using RateController.Language;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

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
            InitializeComponent();
            // No resolution/unit subscriptions anymore
            MainMenu = menu;
            mf = main;
            this.Tag = false;

            timer1.Enabled = true;
            lbDataPoints.Text = Props.RateCollector.DataPoints.ToString("N0");
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
                Props.RateCollector.ClearReadings();

                // Immediately clear coverage overlay and legend from the map
                mf.Tls.Manager.ClearAppliedRatesOverlay();
            }
        }
        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                string sourceFilePath = JobManager.CurrentRateDataPath;
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Title = "Save File As";
                    saveFileDialog.Filter = "All Files|*.csv";
                    saveFileDialog.FileName = System.IO.Path.GetFileName(sourceFilePath);

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string destinationFilePath = saveFileDialog.FileName;

                        try
                        {
                            System.IO.File.Copy(sourceFilePath, destinationFilePath, true);
                            Props.ShowMessage("File copied successfully!", "Import", 5000);
                        }
                        catch (Exception ex)
                        {
                            Props.ShowMessage("Error copying file: " + ex.Message, "Import", 10000, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Props.ShowMessage(ex.Message);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cEdited)
            {
                try
                {
                    Props.RateDisplayType = rbApplied.Checked ? RateType.Applied : RateType.Target;
                    Props.RateRecordEnabled = ckRecord.Checked;

                    if (rbProductA.Checked) Props.RateDisplayProduct = 0;
                    else if (rbProductB.Checked) Props.RateDisplayProduct = 1;
                    else if (rbProductC.Checked) Props.RateDisplayProduct = 2;
                    else Props.RateDisplayProduct = 3;

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
            SubMenuLayout.SetFormLayout(this, MainMenu, btnOK);

            btnCancel.Left = btnOK.Left - SubMenuLayout.ButtonSpacing;
            btnCancel.Top = btnOK.Top;
            MainMenu.StyleControls(this);
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            PositionForm();
            SetLanguage();
            UpdateForm();

            // Emphasize values that remain
            var valFont = new Font(lbDataPoints.Font.FontFamily, 14, FontStyle.Bold);
            lbDataPoints.Font = valFont;

            // Hook import button
            btnImport.Click += btnImport_Click;
        }

        private void gbMap_Paint(object sender, PaintEventArgs e)
        {
            var box = sender as GroupBox;
            Props.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }

        private void HSresolution_Scroll(object sender, ScrollEventArgs e)
        {
            // Resolution feature removed
            // Intentionally left blank to satisfy any designer wiring
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
            // Resolution-based unit display removed; keep form consistent otherwise
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
            // Removed resolution label updates
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lbDataPoints.Text = Props.RateCollector.DataPoints.ToString("N0");
        }

        private void UpdateForm()
        {
            try
            {
                Initializing = true;

                rbApplied.Checked = (Props.RateDisplayType == RateType.Applied);
                rbTarget.Checked = !rbApplied.Checked;
                ckRecord.Checked = Props.RateRecordEnabled;

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
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmRates/UpdateForm: " + ex.Message);
            }
            Initializing = false;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                using (var ofd = new OpenFileDialog { Filter = "CSV Files (*.csv)|*.csv", Title = "Select Rate Data CSV" })
                {
                    if (ofd.ShowDialog() != DialogResult.OK) return;

                    string csvPath = ofd.FileName;
                    if (!File.Exists(csvPath))
                    {
                        Props.ShowMessage("Selected file not found.", "Import", 8000, true);
                        return;
                    }

                    // Use frmImportCSV to create a job and attach the CSV
                    using (var dlg = new frmImportCSV())
                    {
                        dlg.CsvPath = csvPath;
                        if (dlg.ShowDialog(this) != DialogResult.OK)
                            return;

                        // After dialog creates and switches job, refresh display
                        lbDataPoints.Text = Props.RateCollector.DataPoints.ToString("N0");
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuRateData/btnImport_Click: " + ex.Message);
                Props.ShowMessage("Import failed: " + ex.Message, "Import", 15000, true, true);
            }
        }

        private void btnImport_Click_1(object sender, EventArgs e)
        {

        }
    }
}