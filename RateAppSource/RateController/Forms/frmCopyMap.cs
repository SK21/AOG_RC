using AgOpenGPS;
using RateController.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace RateController.Forms
{
    public partial class frmCopyMap : Form
    {
        private Job EditingJob = null;
        private FormStart mf;

        public frmCopyMap(FormStart mf)
        {
            InitializeComponent();
            this.mf = mf;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnJobsDown_Click(object sender, EventArgs e)
        {
            if (lvJobs.Items.Count == 0) return;

            // Estimate items per page
            int itemHeight = lvJobs.Font.Height + 4; // Add small buffer for padding
            int itemsPerPage = lvJobs.ClientSize.Height / itemHeight;

            // Find the first visible item
            int currentTopIndex = lvJobs.TopItem?.Index ?? 0;

            // Scroll down by one page
            int newTopIndex = Math.Min(lvJobs.Items.Count - 1, currentTopIndex + itemsPerPage);
            lvJobs.EnsureVisible(newTopIndex);
        }

        private void btnJobsUp_Click(object sender, EventArgs e)
        {
            if (lvJobs.Items.Count == 0) return;

            int itemHeight = lvJobs.Font.Height + 4;
            int itemsPerPage = lvJobs.ClientSize.Height / itemHeight;

            int currentTopIndex = lvJobs.TopItem?.Index ?? 0;
            int newTopIndex = Math.Max(0, currentTopIndex - itemsPerPage);
            lvJobs.EnsureVisible(newTopIndex);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var Hlp = new frmMsgBox(mf, "Confirm replace current map with selected map?", "Copy Map", true);
            Hlp.TopMost = true;

            Hlp.ShowDialog();
            bool Result = Hlp.Result;
            Hlp.Close();
            if (Result)
            {
                if (CopyMap())
                {
                    Props.ShowMessage("Map copied successfully.");
                    this.Close();
                }
                else
                {
                    Props.ShowMessage("Failed to copy map.");
                    this.Close();
                }
            }
        }

        private void cbSearchField_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillJobsList();
        }

        private bool CopyMap()
        {
            bool Result = false;
            try
            {
                if (lvJobs.SelectedItems.Count > 0)
                {
                    Job SelectedJob = lvJobs.SelectedItems[0].Tag as Job;
                    if (SelectedJob != null)
                    {
                        Result = JobManager.CopyJobData(SelectedJob.ID, JobManager.CurrentJobID, ckErase.Checked);
                        if (Result) mf.Tls.Manager.LoadMap();
                    }
                }
                else
                {
                    Props.ShowMessage("No file selected.");
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmCopyMaps/CopMap: " + ex.Message);
            }
            return Result;
        }

        private void FillCombo()
        {
            try
            {
                List<Parcel> Flds = ParcelManager.GetParcels();

                // Sort and create independent lists for each ComboBox
                var fieldList = Flds.OrderBy(p => p.Name).ToList();
                var searchFieldList = Flds.OrderBy(p => p.Name).ToList();

                // Set data sources separately to prevent unwanted synchronization
                cbSearchField.DataSource = searchFieldList;

                // Assign display and value members
                cbSearchField.DisplayMember = "Name";
                cbSearchField.ValueMember = "ID";

                cbSearchField.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmCopyMap/FillCombos: " + ex.Message);
            }
        }

        private void FillJobsList()
        {
            lvJobs.Items.Clear();

            int yr;
            if (!int.TryParse(tbSearchYear.Text, out yr)) yr = DateTime.Now.Year;

            DateTime startDate = new DateTime(yr, 1, 1);
            DateTime endDate = new DateTime(yr, 12, 31);

            Parcel selectedParcel = cbSearchField.SelectedItem as Parcel;
            int? SelectedParcelID = selectedParcel?.ID;

            List<Job> filteredJobs = JobManager.FilterJobs(startDate, endDate, SelectedParcelID)
                             .OrderBy(job => job.Date)
                             .ThenBy(job => job.Name)
                             .ToList();

            foreach (var job in filteredJobs)
            {
                var item = new ListViewItem(job.Name);  // First column
                item.SubItems.Add(job.Date.ToString("dd-MMM"));  // Second column

                item.Tag = job;  // Store the Job object for later retrieval
                lvJobs.Items.Add(item);
            }
        }

        private void frmCopyMap_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
        }

        private void frmCopyMap_Load(object sender, EventArgs e)
        {
            Props.LoadFormLocation(this);
            this.BackColor = Properties.Settings.Default.MainBackColour;
            SetLanguage();
            UpdateEditingJob();
            FillCombo();

            // set current field & date
            tbSearchYear.Text = EditingJob.Date.Year.ToString();
            Parcel fieldParcel = ParcelManager.SearchParcel(EditingJob.FieldID);
            if (fieldParcel == null)
            {
                cbSearchField.SelectedIndex = -1;
            }
            else
            {
                var matchingParcel = cbSearchField.Items
                                  .OfType<Parcel>()
                                  .FirstOrDefault(p => p.ID == fieldParcel.ID);

                if (matchingParcel == null)
                {
                    cbSearchField.SelectedIndex = -1;  // Could not find a matching object
                }
                else
                {
                    cbSearchField.SelectedItem = matchingParcel;
                }
            }

            FillJobsList();
        }

        private void SetLanguage()
        {
        }

        private void tbSearchYear_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbSearchYear.Text, out tempD);
            using (var form = new FormNumeric(1900, 2100, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbSearchYear.Text = form.ReturnValue.ToString();
                    FillJobsList();
                }
            }
        }

        private void tbSearchYear_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbSearchYear.Text, out tempD);
            if (tempD < 1900 || tempD > 2100)
            {
                tbSearchYear.Text = DateTime.Now.Year.ToString();
            }
        }

        private void UpdateEditingJob()
        {
            int JobID = JobManager.CurrentJobID;
            EditingJob = JobManager.SearchJob(JobID);
            if (EditingJob == null) EditingJob = JobManager.SearchJob(0);
        }
    }
}