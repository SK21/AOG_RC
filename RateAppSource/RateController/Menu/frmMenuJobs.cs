using AgOpenGPS;
using RateController.Classes;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuJobs : Form
    {
        private const int SB_PAGEDOWN = 3;
        private const int SB_PAGEUP = 2;
        private const int WM_VSCROLL = 0x0115;
        private bool cEdited;
        private Job EditingJob = null;
        private bool Initializing = false;
        private bool IsNewJob = false;
        private int JobToCopyFromID = -1;
        private frmMenu MainMenu;
        private FormStart mf;

        public frmMenuJobs(FormStart main, frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            mf = main;
            this.Tag = false;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        private void btnCalender_Click(object sender, EventArgs e)
        {
            tbDate.Text = FormattedDate(DateTime.Now);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateEditingJob();
                UpdateForm();
                SetButtons(false);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuJobs/btnCancel_Click: " + ex.Message);
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                if (lvJobs.SelectedItems.Count > 0)
                {
                    Job SelectedJob = lvJobs.SelectedItems[0].Tag as Job;
                    if (SelectedJob != null)
                    {
                        JobToCopyFromID = SelectedJob.ID;
                        EditingJob = new Job();
                        EditingJob.Date = DateTime.Now;
                        EditingJob.FieldID = SelectedJob.FieldID;
                        EditingJob.Name = SelectedJob.Name + "_Copy";
                        EditingJob.Notes = "";
                        IsNewJob = true;
                        SetButtons(true);
                        UpdateForm();
                    }
                }
                else
                {
                    Props.ShowMessage("No file selected.");
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuJobs/btnCopy_Click: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (lvJobs.SelectedItems.Count > 0)
                {
                    Job SelectedJob = lvJobs.SelectedItems[0].Tag as Job;
                    if (SelectedJob != null && SelectedJob.ID != 0)    // keep 0, default job
                    {
                        var Hlp = new frmMsgBox(mf, "Confirm Delete [" + SelectedJob.Name + "] and all job data?", "Delete File", true);
                        Hlp.TopMost = true;

                        Hlp.ShowDialog();
                        bool Result = Hlp.Result;
                        Hlp.Close();
                        if (Result)
                        {
                            JobManager.DeleteJob(SelectedJob.ID);
                            UpdateEditingJob();
                            UpdateForm();
                        }
                    }
                    else
                    {
                        Props.ShowMessage("Can not delete file.", "Help", 20000, true);
                    }
                }
                else
                {
                    Props.ShowMessage("No file selected.");
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuJobs/btnDelete_Click: +" + ex.Message);
            }
        }

        private void btnDeleteField_Click(object sender, EventArgs e)
        {
            try
            {
                Parcel Fld = cbSearchField.SelectedItem as Parcel;
                if (Fld != null)
                {
                    string FieldToDelete = Fld.Name;
                    var Hlp = new frmMsgBox(mf, "Confirm Delete [" + FieldToDelete + "] from the Fields list?", "Delete Field", true);
                    Hlp.TopMost = true;

                    Hlp.ShowDialog();
                    bool Result = Hlp.Result;
                    Hlp.Close();
                    if (Result)
                    {
                        if (ParcelManager.DeleteParcel(Fld.ID, out bool FieldInUse))
                        {
                            UpdateForm();
                        }
                        else
                        {
                            if (FieldInUse)
                            {
                                Props.ShowMessage("Can not delete, field is in use.");
                            }
                            else
                            {
                                Props.ShowMessage("Field could not be deleted.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuJobs/btnDeleteField_Click: " + ex.Message);
            }
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

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                if (lvJobs.SelectedItems.Count > 0)
                {
                    Job selectedJob = lvJobs.SelectedItems[0].Tag as Job;
                    if (selectedJob != null)
                    {
                        Props.CurrentJobID = selectedJob.ID;
                        UpdateEditingJob();
                        UpdateForm();
                    }
                }
                else
                {
                    Props.ShowMessage("No file selected.");
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuJobs/btnLoad_Click: " + ex.Message);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                EditingJob = new Job();
                tbDate.Text = FormattedDate(DateTime.Now);
                tbNotes.Text = "";
                IsNewJob = true;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuJobs/btnNew_Click: " + ex.Message);
            }
        }

        private void btnNotesDown_Click(object sender, EventArgs e)
        {
            SendMessage(tbNotes.Handle, WM_VSCROLL, new IntPtr(SB_PAGEDOWN), IntPtr.Zero);
        }

        private void btnNotesUp_Click(object sender, EventArgs e)
        {
            SendMessage(tbNotes.Handle, WM_VSCROLL, new IntPtr(SB_PAGEUP), IntPtr.Zero);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                // save parcel
                int? selectedFieldID = cbField.SelectedValue as int?;
                Parcel selectedParcel = selectedFieldID.HasValue
                    ? ParcelManager.GetParcels().FirstOrDefault(p => p.ID == selectedFieldID.Value)
                    : null;

                // If no parcel is found, treat it as a new parcel.
                if (selectedParcel == null)
                {
                    // Use the text from the combo as the new parcel name.
                    selectedParcel = new Parcel { Name = cbField.Text.Trim()};
                    ParcelManager.AddParcel(selectedParcel);
                }
                else
                {
                    // update its name in case it changed.
                    selectedParcel.Name = cbField.Text;
                    ParcelManager.EditParcel(selectedParcel);
                }

                // save job
                if (EditingJob != null)
                {
                    EditingJob.Name = tbName.Text;
                    DateTime NewDate;
                    DateTime.TryParse(tbDate.Text, out NewDate);
                    EditingJob.Date = NewDate;
                    EditingJob.FieldID = selectedParcel.ID;
                    EditingJob.Notes = tbNotes.Text;

                    if (IsNewJob)
                    {
                        JobManager.AddJob(EditingJob);
                        IsNewJob = false;
                        if (JobToCopyFromID > -1)
                        {
                            // copy job data
                            JobManager.CopyJobData(JobToCopyFromID, EditingJob.ID);
                            JobToCopyFromID = -1;
                        }
                    }
                    else
                    {
                        JobManager.EditJob(EditingJob);
                    }

                    Props.CurrentJobID = EditingJob.ID;
                }

                SetButtons(false);
                UpdateForm();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuJobs/btnOk_Click: " + ex.Message);
            }
        }

        private void cbSearchField_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillJobsList();
        }

        private void ckJobs_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }


        private void ckResume_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing) Props.ShowJobs = ckResume.Checked;
        }

        private void FillCombos()
        {
            try
            {
                // Save selected ID values (not objects)
                int? previousSearchFieldID = (cbSearchField.SelectedItem as Parcel)?.ID;
                int? CurrentFieldID = EditingJob.FieldID;

                List<Parcel> parcels = ParcelManager.GetParcels();

                cbField.BeginUpdate();
                cbSearchField.BeginUpdate();

                try
                {
                    cbField.DisplayMember = cbSearchField.DisplayMember = "Name";
                    cbField.ValueMember = cbSearchField.ValueMember = "ID";

                    cbField.DataSource = new List<Parcel>(parcels);
                    cbSearchField.DataSource = new List<Parcel>(parcels);
                }
                finally
                {
                    cbField.EndUpdate();
                    cbSearchField.EndUpdate();
                }

                // Try to match by ID, after DataSource is assigned
                int? targetSearchFieldID = previousSearchFieldID ?? CurrentFieldID;

                if (targetSearchFieldID.HasValue)
                {
                    foreach (var item in cbSearchField.Items)
                    {
                        if (item is Parcel parcel && parcel.ID == targetSearchFieldID.Value)
                        {
                            cbSearchField.SelectedItem = parcel;
                            return;
                        }
                    }
                }

                // If no match found
                cbSearchField.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuJobs/FillCombos: " + ex.Message);
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

        private string FormattedDate(DateTime date)
        {
            CultureInfo culture = CultureInfo.CurrentCulture;
            string timeFormat = culture.DateTimeFormat.ShortTimePattern;
            string format = "dd-MMM-yyyy   " + timeFormat;
            return date.ToString(format, culture);
        }

        private void frmMenuJobs_Load(object sender, EventArgs e)
        {
            try
            {
                SubMenuLayout.SetFormLayout(this, MainMenu, null);

                PositionForm();
                MainMenu.MenuMoved += MainMenu_MenuMoved;
                MainMenu.StyleControls(this);
                SetLanguage();
                UpdateEditingJob();
                UpdateForm();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuJobs/frmMenuJobs_Load: " + ex.Message);
            }
        }

        private void gbCurrentJob_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            if (box != null)
            {
                Color borderColor = cEdited ? Color.Blue : Color.Blue; // Change color based on cEdited
                float borderWidth = cEdited ? 3 : 1; // Change thickness based on cEdited
                mf.Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, borderColor, borderWidth);
            }
        }

        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            if (box != null)
            {
                Color borderColor = cEdited ? Color.Red : Color.Blue; // Change color based on cEdited
                float borderWidth = cEdited ? 3 : 1; // Change thickness based on cEdited
                mf.Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, borderColor, borderWidth);
            }
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

        private void SetButtons(bool Edited)
        {
            if (!Initializing)
            {
                if (Edited)
                {
                    btnCancel.Enabled = true;
                    btnOK.Enabled = true;
                    btnLoad.Enabled = false;
                    btnCopy.Enabled = false;
                    btnDelete.Enabled = false;
                    btnDeleteField.Enabled = false;
                    btnNew.Enabled = false;
                    gbJobs.Enabled = false;
                }
                else
                {
                    btnCancel.Enabled = false;
                    btnOK.Enabled = false;
                    btnLoad.Enabled = true;
                    btnCopy.Enabled = true;
                    btnDelete.Enabled = true;
                    btnDeleteField.Enabled = true;
                    btnNew.Enabled = true;
                    gbJobs.Enabled = true;
                }

                cEdited = Edited;
                this.Tag = cEdited;
                gbCurrentJob.Invalidate();
                gbJobs.Invalidate();
            }
        }

        private void SetLanguage()
        {
            //grpSensor.Text = Lang.lgSensorLocation;
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

        private void tbSearchYear_TextChanged(object sender, EventArgs e)
        {
            // highlight btnRefeshJobs
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
            int JobID = Props.CurrentJobID;
            EditingJob = JobManager.SearchJob(JobID);
            if (EditingJob == null) EditingJob = JobManager.SearchJob(0);
            IsNewJob = false;
        }

        private void UpdateForm()
        {
            try
            {
                Initializing = true;

                FillCombos();
                FillJobsList();

                if (EditingJob == null)
                {
                    tbName.Text = string.Empty;
                    tbDate.Text = FormattedDate(DateTime.Now);
                    cbField.SelectedIndex = -1;
                    tbNotes.Text = string.Empty;
                }
                else
                {
                    tbName.Text = EditingJob.Name;
                    tbDate.Text = FormattedDate(EditingJob.Date);

                    // Use ParcelManager.SearchParcel to find the Parcel by FieldID
                    Parcel fieldParcel = ParcelManager.SearchParcel(EditingJob.FieldID);
                    if (fieldParcel == null)
                    {
                        cbField.SelectedIndex = -1;
                    }
                    else
                    {
                        var matchingParcel = cbField.Items
                                          .OfType<Parcel>()
                                          .FirstOrDefault(p => p.ID == fieldParcel.ID);

                        if (matchingParcel == null)
                        {
                            cbField.SelectedIndex = -1;  // Could not find a matching object
                        }
                        else
                        {
                            cbField.SelectedItem = matchingParcel;
                        }
                    }
                    tbNotes.Text = EditingJob.Notes;
                }

                tbNotes.SelectionStart = tbNotes.Text.Length;
                tbNotes.ScrollToCaret();

                // Trigger a repaint
                gbCurrentJob.Invalidate();
                gbJobs.Invalidate();

                // highlight the current job in the list
                foreach (ListViewItem item in lvJobs.Items)
                {
                    if (item.Tag is Job job && job.ID == Props.CurrentJobID)
                    {
                        item.Selected = true;
                        item.EnsureVisible();
                        break;
                    }
                }
                ckResume.Checked = Props.ShowJobs;

                Initializing = false;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuJobs/UpdateForm: " + ex.Message);
            }
        }
    }
}