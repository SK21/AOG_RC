using RateController.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RateController.Menu
{
    public partial class frmMenuJobs : Form
    {
        private const int SB_PAGEDOWN = 3;
        private const int SB_PAGEUP = 2;
        private const int WM_VSCROLL = 0x0115;
        private bool ButtonDateEntry = false;
        private bool cEdited;
        private bool DeleteField = false;
        private bool Initializing = false;
        private bool IsNewJob = false;
        private string JobsDateFormat = "dd-MMM-yyyy   HH:mm";
        private frmMenu MainMenu;
        private FormStart mf;
        private int NewJobID = -1;

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
            ButtonDateEntry = true;
            tbDate.Text = DateTime.Now.ToString(JobsDateFormat);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (IsNewJob)
            {
                Job.DeleteJob(NewJobID);
                IsNewJob = false;
                NewJobID = -1;
            }
            UpdateForm();
            SetButtons(false);
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstJobs.SelectedIndex >= 0)
                {
                    Job SelectedJob = lstJobs.SelectedItem as Job;
                    Job NewJob = Job.CopyJob(SelectedJob.ID);
                    if (NewJob == null)
                    {
                        mf.Tls.ShowMessage("Could not copy file.");
                    }
                    else
                    {
                        tbDate.Text = NewJob.Date.ToString(JobsDateFormat);
                        cbField.SelectedValue = NewJob.FieldID;
                        tbNotes.Text = NewJob.Notes;
                        NewJobID = NewJob.ID;
                        IsNewJob = true;
                    }
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
                if (lstJobs.SelectedIndex >= 0)
                {
                    string FileToDelete = lstJobs.SelectedItem.ToString();
                    if (FileToDelete != "DefaultJob")
                    {
                        var Hlp = new frmMsgBox(mf, "Confirm Delete [" + FileToDelete + "] and all job data?", "Delete File", true);
                        Hlp.TopMost = true;

                        Hlp.ShowDialog();
                        bool Result = Hlp.Result;
                        Hlp.Close();
                        if (Result)
                        {
                            Job SelectedJob = lstJobs.SelectedItem as Job;
                            if (SelectedJob != null)
                            {
                                Job.DeleteJob(SelectedJob.ID);
                                UpdateForm();
                            }
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
                if (lstJobs.SelectedIndex >= 0)
                {
                    Properties.Settings.Default.CurrentJob = lstJobs.SelectedIndex;
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
                Job NewJob = new Job();
                Job.AddJob(NewJob);
                tbDate.Text = NewJob.Date.ToString(JobsDateFormat);
                cbField.SelectedValue = NewJob.FieldID;
                tbNotes.Text = NewJob.Notes;
                NewJobID = NewJob.ID;
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

                int CurrentID = -1;
                if (NewJobID > -1)
                {
                    CurrentID = NewJobID;
                    NewJobID = -1;
                    IsNewJob = false;
                }
                else
                {
                    if (lstJobs.SelectedIndex >= 0) CurrentID = lstJobs.SelectedIndex;
                }

                if (CurrentID > -1)
                {
                    Job NewJob = Job.SearchJob(CurrentID);
                    if (NewJob != null)
                    {
                        DateTime NewDate;
                        DateTime.TryParse(tbDate.Text, out NewDate);
                        NewJob.Date = NewDate;

                        NewJob.FieldID = cbField.SelectedIndex;
                        NewJob.Notes = tbNotes.Text;
                    }
                }

                SetButtons(false);
                UpdateForm();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuJobs/btnOk_Click: " + ex.Message);
            }
        }

        private void ckJobs_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void FillCombos()
        {
            List<Parcel> Flds = ParcelManager.GetParcels();
            cbSearchField.DataSource = Flds;
            cbSearchField.DisplayMember = "Name";
            cbSearchField.ValueMember = "ID";

            cbField.DataSource = Flds;
            cbField.DisplayMember = "Name";
            cbField.ValueMember = "ID";
        }

        private void FillListBox()
        {
            try
            {
                int yr = 0;
                int.TryParse(tbSearchYear.Text, out yr);
                List<Job> filteredByDate;
                if (yr > 0)
                {
                    DateTime startDate = new DateTime(yr, 1, 1);
                    DateTime endDate = new DateTime(yr, 12, 31);

                    filteredByDate = Job.FilterByDate(startDate, endDate);
                }
                else
                {
                    filteredByDate = Job.GetJobs();
                }

                List<Job> filteredJobs = filteredByDate
                    .Where(job => job.FieldID == cbSearchField.SelectedIndex)
                    .ToList();
                lstJobs.Items.Clear();
                lstJobs.DataSource = filteredJobs;
                lstJobs.DisplayMember = "Name";
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuJobs/FillListBox: " + ex.Message);
            }
        }

        private void frmMenuJobs_Load(object sender, EventArgs e)
        {
            // menu 800,600
            // sub menu 540,630
            //SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            this.Width = MainMenu.Width - 260;
            this.Height = MainMenu.Height - 50;
            PositionForm();
            MainMenu.StyleControls(this);
            SetLanguage();
            UpdateForm();
        }

        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            mf.Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);
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
                }
                else
                {
                    btnCancel.Enabled = false;
                    btnOK.Enabled = false;
                    btnLoad.Enabled = true;
                    btnCopy.Enabled = true;
                    btnDelete.Enabled = true;
                    btnDeleteField.Enabled = true;
                }

                cEdited = Edited;
                this.Tag = cEdited;
            }
        }

        private void SetLanguage()
        {
            //grpSensor.Text = Lang.lgSensorLocation;
        }

        private void UpdateForm()
        {
            Initializing = true;

            FillListBox();
            FillCombos();

            Job jb = Job.SearchJob(Properties.Settings.Default.CurrentJob);

            tbDate.Text = jb.Date.ToString(JobsDateFormat);
            cbField.SelectedValue = jb.FieldID;
            tbNotes.Text = jb.Notes;
            tbNotes.SelectionStart = tbNotes.Text.Length;
            tbNotes.ScrollToCaret();

            ckJobs.Checked = Properties.Settings.Default.UseJobs;

            Initializing = false;
        }
    }
}