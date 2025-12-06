using RateController.Classes;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RateController.Forms
{
    public partial class frmImportCSV : Form
    {
        private const int SB_PAGEDOWN = 3;
        private const int SB_PAGEUP = 2;
        private const int WM_VSCROLL = 0x0115;

        public frmImportCSV()
        {
            InitializeComponent();
            // Wire events
            btnOK.Click += BtnOK_Click;
            btnCancel.Click += BtnCancel_Click;
            btnCalender.Click += BtnCalender_Click;
            tbName.TextChanged += TbName_TextChanged;
            this.Load += FrmImportCSV_Load;
        }

        public string CsvPath { get; set; }

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        private void BtnCalender_Click(object sender, EventArgs e)
        {
            tbDate.Text = FormattedDate(DateTime.Now);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnNotesDown_Click(object sender, EventArgs e)
        {
            SendMessage(tbNotes.Handle, WM_VSCROLL, new IntPtr(SB_PAGEDOWN), IntPtr.Zero);
        }

        private void btnNotesUp_Click(object sender, EventArgs e)
        {
            SendMessage(tbNotes.Handle, WM_VSCROLL, new IntPtr(SB_PAGEUP), IntPtr.Zero);
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CsvPath) || !File.Exists(CsvPath))
                {
                    Props.ShowMessage("CSV file not found.", "Import", 8000, true);
                    return;
                }

                string jobName = (tbName.Text ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(jobName))
                {
                    Props.ShowMessage("Job name is required.", "Import", 8000, true);
                    return;
                }

                // Determine or create field ID
                int fieldID = -1;
                string fieldText = cbField.Text?.Trim();
                if (!string.IsNullOrEmpty(fieldText))
                {
                    var existing = ParcelManager.GetParcels().FirstOrDefault(p => string.Equals(p.Name, fieldText, StringComparison.OrdinalIgnoreCase));
                    if (existing != null)
                    {
                        fieldID = existing.ID;
                    }
                    else
                    {
                        var newParcel = new Parcel { Name = fieldText };
                        ParcelManager.AddParcel(newParcel);
                        fieldID = newParcel.ID;
                    }
                }

                // Parse date
                DateTime dateVal;
                if (!DateTime.TryParse(tbDate.Text, CultureInfo.CurrentCulture, DateTimeStyles.None, out dateVal))
                {
                    dateVal = DateTime.Now;
                }

                // Create and save job
                var newJob = new Job
                {
                    Name = jobName,
                    Date = dateVal,
                    FieldID = fieldID,
                    Notes = tbNotes.Text ?? string.Empty
                };

                JobManager.AddJob(newJob);

                // Copy CSV into the new job folder
                string destPath = Path.Combine(newJob.JobFolder, "RateData.csv");
                try
                {
                    File.Copy(CsvPath, destPath, true);
                }
                catch (Exception ex)
                {
                    Props.ShowMessage("Failed to copy CSV: " + ex.Message, "Import", 15000, true, true);
                    return;
                }

                // Switch to the new job
                JobManager.CurrentJobID = newJob.ID;

                Props.ShowMessage("CSV imported to new job [" + jobName + "].", "Import", 8000);

                this.DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmImportCSV/BtnOK_Click: " + ex.Message);
                Props.ShowMessage("Import failed: " + ex.Message, "Import", 15000, true, true);
            }
        }

        private string FormattedDate(DateTime date)
        {
            CultureInfo culture = CultureInfo.CurrentCulture;
            string timeFormat = culture.DateTimeFormat.ShortTimePattern;
            string format = "dd-MMM-yyyy   " + timeFormat;
            return date.ToString(format, culture);
        }

        private void FrmImportCSV_Load(object sender, EventArgs e)
        {
            this.BackColor = Properties.Settings.Default.MainBackColour;

            // Initialize defaults
            tbDate.Text = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            btnOK.Enabled = !string.IsNullOrWhiteSpace(tbName.Text);

            // Fill field combo with existing parcels (allow typing new values)
            try
            {
                var parcels = ParcelManager.GetParcels();
                cbField.Items.Clear();
                foreach (var p in parcels)
                {
                    cbField.Items.Add(new ParcelItem { ID = p.ID, Name = p.Name });
                }
                cbField.DropDownStyle = ComboBoxStyle.DropDown; // allow new entries
                if (cbField.Items.Count > 0) cbField.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmImportCSV_Load: " + ex.Message);
            }
        }

        private void TbName_TextChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = !string.IsNullOrWhiteSpace(tbName.Text);
        }

        private class ParcelItem
        {
            public int ID { get; set; }
            public string Name { get; set; }

            public override string ToString() => Name;
        }
    }
}