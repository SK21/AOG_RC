using RateController.Classes;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuJobs : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private FormStart mf;

        public frmMenuJobs(FormStart main, frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            mf = main;
            this.Tag = false;
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
                        Props.OpenJob(tbName.Text);
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
                                if (Properties.Settings.Default.CurrentJob == FilePath + "\\" + FileToDelete + ".jbs")
                                {
                                    Props.OpenJob(Props.DefaultJob);
                                    tbName.Text = "";
                                    MainMenu.ShowProfile();
                                }
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

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstJobs.SelectedIndex >= 0)
                {
                    if (Props.OpenJob(lstJobs.SelectedItem.ToString()))
                    {
                        UpdateForm();
                        MainMenu.ShowProfile();
                    }
                    else
                    {
                        mf.Tls.ShowMessage("File not opened.");
                    }
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
                if (Props.OpenJob(tbName.Text,true))
                {
                    tbName.Text = "";
                    UpdateForm();
                    MainMenu.ShowProfile();
                }
                else
                {
                    mf.Tls.ShowMessage("Invalid file name.");
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuJobs/btnNew_Click: " + ex.Message);
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

        private void SetLanguage()
        {
            //grpSensor.Text = Lang.lgSensorLocation;
        }

        private void UpdateForm()
        {
            Initializing=true;

            lstJobs.Items.Clear();
            string[] folders = Directory.GetDirectories(Props.JobsFolder);
            foreach (string folder in folders)
            {
                lstJobs.Items.Add(Path.GetFileName(folder));
            }
            lbJob.Text = "Current Job:  " + Path.GetFileNameWithoutExtension(Props.CurrentJob);

            ckJobs.Checked = Properties.Settings.Default.UseJobs;
            tbNotes.Text = Props.GetJobProp("Notes");

            Initializing =false;
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

        private void ckJobs_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                Properties.Settings.Default.UseJobs = ckJobs.Checked;
                Properties.Settings.Default.Save();

            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuJobs/btnOk_Click: " + ex.Message);
            }
        }
    }
}