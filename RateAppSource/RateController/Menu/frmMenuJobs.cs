using RateController.Classes;
using System;
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
        private string RCdateFormat = "dd-MMM-yyyy   HH:mm";
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

        private void btnAppendDate_Click(object sender, EventArgs e)
        {
            if (tbName.Text.Length > 0)
            {
                tbName.Text += "_" + DateTime.Now.ToString("yyyy-MM-dd");
            }
        }

        private void btnCalender_Click(object sender, EventArgs e)
        {
            ButtonDateEntry = true;
            tbDate.Text = DateTime.Now.ToString(RCdateFormat);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
            tbName.Text = "";
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                string NewFilePath = Props.JobsFolder + "\\" + tbName.Text;
                if (FileNameValidator.IsValidFolderName(tbName.Text) &&
                    FileNameValidator.IsValidFileName(tbName.Text) &&
                    !Directory.Exists(NewFilePath))
                {
                    if (lstJobs.SelectedIndex >= 0)
                    {
                        Directory.CreateDirectory(NewFilePath);

                        string OldFileName = lstJobs.SelectedItem.ToString();
                        string OldFileFullName = Props.JobsFolder + "\\" + OldFileName + "\\" + OldFileName + ".jbs";
                        File.Copy(OldFileFullName, NewFilePath + "\\" + tbName.Text + ".jbs");

                        string NewFileRatesName = NewFilePath + "\\" + tbName.Text + "RateData.csv";
                        string OldFileRatesName = Props.JobsFolder + "\\" + OldFileName + "\\" + OldFileName + "RateData.csv";
                        File.Copy(OldFileRatesName, NewFileRatesName);

                        Directory.CreateDirectory(NewFilePath + "\\Map");
                        //Props.OpenJob(tbName.Text);
                        tbName.Text = "";
                        UpdateForm();
                        MainMenu.ShowProfile();
                    }
                    else
                    {
                        mf.Tls.ShowMessage("No file selected.");
                    }
                }
                else
                {
                    mf.Tls.ShowMessage("Invalid file name.");
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
                        var Hlp = new frmMsgBox(mf, "Confirm Delete [" + FileToDelete + "]?", "Delete File", true);
                        Hlp.TopMost = true;

                        Hlp.ShowDialog();
                        bool Result = Hlp.Result;
                        Hlp.Close();
                        if (Result)
                        {
                            string FilePath = Props.JobsFolder + "\\" + FileToDelete;
                            if (Props.SafeToDelete(FilePath))
                            {
                                Directory.Delete(FilePath, true);

                                // load default if current is deleted
                                //if (Properties.Settings.Default.CurrentJob == FilePath + "\\" + FileToDelete + ".jbs")
                                //{
                                //    Props.OpenJob(Props.DefaultJob);
                                //    tbName.Text = "";
                                //    MainMenu.ShowProfile();
                                //}
                                UpdateForm();
                            }
                            else
                            {
                                mf.Tls.ShowMessage("Can not delete file.", "Help", 20000, true);
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
                    DeleteField = true;
                    SetButtons(true);
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
            //try
            //{
            //    if (lstJobs.SelectedIndex >= 0)
            //    {
            //        if (Props.OpenJob(lstJobs.SelectedItem.ToString()))
            //        {
            //            UpdateForm();
            //            MainMenu.ShowProfile();
            //        }
            //        else
            //        {
            //            mf.Tls.ShowMessage("File not opened.");
            //        }
            //    }
            //    else
            //    {
            //        mf.Tls.ShowMessage("No file selected.");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Props.WriteErrorLog("frmMenuJobs/btnLoad_Click: " + ex.Message);
            //}
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    if (Props.OpenJob(tbName.Text, true))
            //    {
            //        tbName.Text = "";
            //        UpdateForm();
            //        MainMenu.ShowProfile();
            //    }
            //    else
            //    {
            //        mf.Tls.ShowMessage("Invalid file name.");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Props.WriteErrorLog("frmMenuJobs/btnNew_Click: " + ex.Message);
            //}
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
            //try
            //{
            //    Properties.Settings.Default.UseJobs = ckJobs.Checked;
            //    Properties.Settings.Default.Save();
            //    Props.SetJobProp("Notes", tbNotes.Text);

            //    // fields
            //    if (DeleteField)
            //    {
            //        cbField.Items.Remove(cbField.SelectedItem);
            //        DeleteField = false;
            //    }

            //    string newItem = cbField.Text.Trim();
            //    if (!string.IsNullOrEmpty(newItem))
            //    {
            //        if (!cbField.Items.Contains(newItem))
            //        {
            //            cbField.Items.Add(newItem);
            //            cbField.SelectedItem = newItem;
            //        }
            //    }

            //    if (cbField.SelectedItem != null) Props.SetJobProp("Field", cbField.SelectedItem.ToString());

            //    string[] itemsArray = cbField.Items.Cast<string>().ToArray();
            //    Props.SetFieldNames(itemsArray);

            //    // date
            //    if (ButtonDateEntry)
            //    {
            //        ButtonDateEntry = false;
            //        Props.SetJobProp("Date", tbDate.Text);
            //    }
            //    else
            //    {
            //        DateTime NewDate;
            //        if (Props.ParseDateText(tbDate.Text, out NewDate))
            //        {
            //            Props.SetJobProp("Date", NewDate.ToString(RCdateFormat));
            //        }
            //    }

            //    SetButtons(false);
            //    UpdateForm();
            //}
            //catch (Exception ex)
            //{
            //    Props.WriteErrorLog("frmMenuJobs/btnOk_Click: " + ex.Message);
            //}
        }

        private void cbField_Resize(object sender, EventArgs e)
        {
            int cbCenter = cbField.Top + (cbField.Height / 2);
            btnDeleteField.Top = cbCenter - (btnDeleteField.Height / 2);
            lbFieldName.Top = cbCenter - (lbFieldName.Height / 2);
        }

        private void ckJobs_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
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
            lbJob.Font = new Font(lbJob.Font, lbJob.Font.Style | FontStyle.Underline);
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
            //grpSensor.Text = Lang.lgSensorLocation;
        }

        private void UpdateForm()
        {
            //Initializing = true;

            //lstJobs.Items.Clear();
            //string[] folders = Directory.GetDirectories(Props.JobsFolder);
            //foreach (string folder in folders)
            //{
            //    lstJobs.Items.Add(Path.GetFileName(folder));
            //}
            //lbJob.Text = "Current Job:  " + Path.GetFileNameWithoutExtension(Props.CurrentJobName);

            //ckJobs.Checked = Properties.Settings.Default.UseJobs;
            //tbNotes.Text = Props.GetJobProp("Notes");

            //// field
            //string[] items = Props.GetFieldNames();
            //if (items != null)
            //{
            //    cbField.Items.Clear();
            //    cbField.Items.AddRange(items);
            //}

            //string field = Props.GetJobProp("Field");
            //if (cbField.Items.Contains(field)) cbField.SelectedItem = field;

            //// date
            //tbDate.Text = Props.GetJobProp("Date");

            //Initializing = false;
        }

        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            mf.Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }
    }
}