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
            int itemsPerPage = lstJobs.ClientSize.Height / lstJobs.ItemHeight; // Calculate visible items
            lstJobs.TopIndex = Math.Min(lstJobs.Items.Count - 1, lstJobs.TopIndex + itemsPerPage); // Move down by 1 page
        }

        private void btnJobsUp_Click(object sender, EventArgs e)
        {
            int itemsPerPage = lstJobs.ClientSize.Height / lstJobs.ItemHeight; // Calculate visible items
            lstJobs.TopIndex = Math.Max(0, lstJobs.TopIndex - itemsPerPage); // Move up by 1 page
        }

        private void btnRefeshJobs_Click(object sender, EventArgs e)
        {
            FillListBox();
        }

        private void btnRefeshJobs_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                cbSearchField.SelectedIndex = -1;
                tbSearchYear.Text = DateTime.Now.Year.ToString();
                FillListBox();
            }
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
                    mf.Tls.ShowMessage("Map copied successfully.");
                    this.Close();
                }
                else
                {
                    mf.Tls.ShowMessage("Failed to copy map.");
                    this.Close();
                }
            }
        }

        private bool CopyMap()
        {
            bool Result = false;
            try
            {
                Job SelectedJob = lstJobs.SelectedItem as Job;
                if (SelectedJob != null)
                {
                    Result = JobManager.CopyJobData(SelectedJob.ID, Props.CurrentJobID, ckErase.Checked);
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

        private void FillListBox()
        {
            try
            {
                int yr;
                if (!int.TryParse(tbSearchYear.Text, out yr)) yr = DateTime.Now.Year;

                DateTime startDate = new DateTime(yr, 1, 1);
                DateTime endDate = new DateTime(yr, 12, 31);

                int? selectedFieldID = cbSearchField.SelectedIndex >= 0 ? (int?)cbSearchField.SelectedValue : null;

                // Preserve currently selected Job ID before filtering
                int? selectedJobID = lstJobs.SelectedItem is Job selectedJob ? selectedJob.ID : (int?)null;

                List<Job> filteredJobs = JobManager.FilterJobs(startDate, endDate, selectedFieldID)
                                           .OrderBy(job => job.Date)
                                           .ThenBy(job => job.Name)
                                           .ToList();

                lstJobs.DataSource = filteredJobs;
                lstJobs.DisplayMember = "Name";

                // Restore previous selection by ID
                if (selectedJobID.HasValue)
                {
                    var jobToSelect = filteredJobs.FirstOrDefault(j => j.ID == selectedJobID);
                    if (jobToSelect != null)
                        lstJobs.SelectedItem = jobToSelect;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmCopMap/FillListBox: " + ex.Message);
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
            FillCombo();
            FillListBox();
        }

        private void SetLanguage()
        {
        }

        private void cbSearchField_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillListBox();
        }
    }
}