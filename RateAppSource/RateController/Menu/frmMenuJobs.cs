using RateController.Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
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
        private string JobsDateFormat = "dd-MMM-yyyy   HH:mm";
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
            tbDate.Text = DateTime.Now.ToString(JobsDateFormat);
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
                if (lstJobs.SelectedIndex >= 0)
                {
                    Job SelectedJob = lstJobs.SelectedItem as Job;
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
                else
                {
                    mf.Tls.ShowMessage("No file selected.");
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
                if (lstJobs.SelectedItem is Job SelectedJob)
                {
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
                        mf.Tls.ShowMessage("Can not delete file.", "Help", 20000, true);
                    }
                }
                else
                {
                    mf.Tls.ShowMessage("No file selected.");
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
                if (cbField.SelectedItem != null)
                {
                    string FieldToDelete = cbField.SelectedItem.ToString();
                    var Hlp = new frmMsgBox(mf, "Confirm Delete [" + FieldToDelete + "] from the Fields list?", "Delete Field", true);
                    Hlp.TopMost = true;

                    Hlp.ShowDialog();
                    bool Result = Hlp.Result;
                    Hlp.Close();
                    if (Result)
                    {
                        if (ParcelManager.DeleteParcel(cbField.SelectedIndex, out bool FieldInUse))
                        {
                            UpdateForm();
                        }
                        else
                        {
                            if (FieldInUse)
                            {
                                mf.Tls.ShowMessage("Can not delete field in use.");
                            }
                            else
                            {
                                mf.Tls.ShowMessage("Field could not be deleted.");
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
            int itemsPerPage = lstJobs.ClientSize.Height / lstJobs.ItemHeight; // Calculate visible items
            lstJobs.TopIndex = Math.Min(lstJobs.Items.Count - 1, lstJobs.TopIndex + itemsPerPage); // Move down by 1 page
        }

        private void btnJobsUp_Click(object sender, EventArgs e)
        {
            int itemsPerPage = lstJobs.ClientSize.Height / lstJobs.ItemHeight; // Calculate visible items
            lstJobs.TopIndex = Math.Max(0, lstJobs.TopIndex - itemsPerPage); // Move up by 1 page
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstJobs.SelectedItem is Job selectedJob)
                {
                    Props.CurrentJobID = selectedJob.ID;
                    UpdateEditingJob();
                    UpdateForm();
                }
                else
                {
                    mf.Tls.ShowMessage("No file selected.");
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
                tbDate.Text = DateTime.Now.ToString(JobsDateFormat);
                tbNotes.Text = "";
                tbName.Text = "New Job";
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
                Properties.Settings.Default.UseJobs = ckJobs.Checked;
                Properties.Settings.Default.Save();

                // save parcel
                int? selectedFieldID = cbField.SelectedValue as int?;
                Parcel selectedParcel = selectedFieldID.HasValue
                    ? ParcelManager.GetParcels().FirstOrDefault(p => p.ID == selectedFieldID.Value)
                    : null;

                // If no parcel is found, treat it as a new parcel.
                if (selectedParcel == null)
                {
                    // Use the text from the combo as the new parcel name.
                    selectedParcel = new Parcel { Name = cbField.Text };
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

        private void ckJobs_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void FillCombos()
        {
            try
            {
                List<Parcel> Flds = ParcelManager.GetParcels();

                // Sort and create independent lists for each ComboBox
                var fieldList = Flds.OrderBy(p => p.Name).ToList();
                var searchFieldList = Flds.OrderBy(p => p.Name).ToList();

                // Set data sources separately to prevent unwanted synchronization
                cbField.DataSource = fieldList;
                cbSearchField.DataSource = searchFieldList;

                // Assign display and value members
                cbField.DisplayMember = cbSearchField.DisplayMember = "Name";
                cbField.ValueMember = cbSearchField.ValueMember = "ID";

                cbSearchField.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuJobs/FillCombos: " + ex.Message);
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
                Props.WriteErrorLog("frmMenuJobs/FillListBox: " + ex.Message);
            }
        }

        private void frmMenuJobs_Load(object sender, EventArgs e)
        {
            try
            {
                MainMenu.MenuMoved += MainMenu_MenuMoved;
                this.Width = MainMenu.Width - 260;
                this.Height = MainMenu.Height - 50;
                PositionForm();
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
            this.Top = MainMenu.Top + 30;
            this.Left = MainMenu.Left + 246;
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
                FillListBox();

                if (EditingJob == null)
                {
                    tbName.Text = string.Empty;
                    tbDate.Text = DateTime.Now.ToString(JobsDateFormat);
                    cbField.SelectedIndex = -1;
                    tbNotes.Text = string.Empty;
                }
                else
                {
                    tbName.Text = EditingJob.Name;
                    tbDate.Text = EditingJob.Date.ToString(JobsDateFormat);

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
                ckJobs.Checked = Properties.Settings.Default.UseJobs;

                // Trigger a repaint
                gbCurrentJob.Invalidate();
                gbJobs.Invalidate();


                Initializing = false;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuJobs/UpdateForm: " + ex.Message);
            }
        }
    }
}