using RateController.Classes;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuProfiles : Form
    {
        private frmMenu MainMenu;
        private FormStart mf;

        public frmMenuProfiles(FormStart main, frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            mf = main;
            this.Tag = false;
        }
        //private void btnCopy_Click(object sender, EventArgs e)
        //{
        //    if (lstProfiles.SelectedIndex >= 0)
        //    {
        //    }
        //    else
        //    {
        //        mf.Tls.ShowMessage("No file selected.");
        //    }
        //}
        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                string NewFilePath = Props.ProfilesFolder + "\\" + tbName.Text;
                if (FileNameValidator.IsValidFolderName(tbName.Text) &&
                    FileNameValidator.IsValidFileName(tbName.Text) &&
                    !Directory.Exists(NewFilePath))
                {
                    if (lstProfiles.SelectedIndex >= 0)
                    {
                        Directory.CreateDirectory(NewFilePath);

                        string OldFileName = lstProfiles.SelectedItem.ToString();
                        string OldFileFullName = Props.ProfilesFolder + "\\" + OldFileName + "\\" + OldFileName + ".rcs";
                        File.Copy(OldFileFullName, NewFilePath + "\\" + tbName.Text + ".rcs");

                        string NewFilePressureName = NewFilePath + "\\" + tbName.Text + "PressureData.csv";
                        string OldFilePressureName = Props.ProfilesFolder + "\\" + OldFileName + "\\" + OldFileName + "PressureData.csv";
                        File.Copy(OldFilePressureName, NewFilePressureName);

                        Props.OpenFile(NewFilePath + "\\" + tbName.Text + ".rcs");
                        tbName.Text = "";
                        mf.LoadSettings();
                        UpdateForm();
                        MainMenu.ChangeProduct(0);
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
                Props.WriteErrorLog("frmMenuProfiles/btnCopy_Click: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstProfiles.SelectedIndex >= 0)
                {
                    string FileToDelete = lstProfiles.SelectedItem.ToString();
                    if (FileToDelete != "Default" && FileToDelete != "Example")
                    {
                        var Hlp = new frmMsgBox(mf, "Confirm Delete [" + FileToDelete + "]?", "Delete File", true);
                        Hlp.TopMost = true;

                        Hlp.ShowDialog();
                        bool Result = Hlp.Result;
                        Hlp.Close();
                        if (Result)
                        {
                            string FilePath = Props.ProfilesFolder + "\\" + FileToDelete;
                            if (Props.IsPathSafeToDelete(FilePath))
                            {
                                Directory.Delete(FilePath, true);

                                // load default if current is deleted
                                if (Properties.Settings.Default.CurrentFile == FilePath + "\\" + FileToDelete + ".rcs")
                                {
                                    string name = Props.ProfilesFolder + "\\default\\default.rcs";
                                    Props.OpenFile(name);
                                    tbName.Text = "";
                                    mf.LoadSettings();
                                    MainMenu.ChangeProduct(0);
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
                Props.WriteErrorLog("frmMenuProfiles/btnDelete_Click: +" + ex.Message);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstProfiles.SelectedIndex >= 0)
                {
                    string NewName = lstProfiles.SelectedItem.ToString();
                    string name = Props.ProfilesFolder + "\\" + NewName + "\\" + NewName + ".rcs";
                    Props.OpenFile(name);
                    mf.LoadSettings();
                    UpdateForm();
                    MainMenu.ChangeProduct(0);
                    MainMenu.ShowProfile();
                }
                else
                {
                    mf.Tls.ShowMessage("No file selected.");
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuProfiles/btnLoad_Click: " + ex.Message);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                string NewFolder = Props.ProfilesFolder + "\\" + tbName.Text;
                if (FileNameValidator.IsValidFolderName(tbName.Text) &&
                    FileNameValidator.IsValidFileName(tbName.Text) &&
                    !Directory.Exists(NewFolder))
                {
                    Directory.CreateDirectory(NewFolder);
                    File.WriteAllText(NewFolder + "\\" + tbName.Text + ".rcs", string.Empty);
                    File.WriteAllText(NewFolder + "\\" + tbName.Text + "PressureData.csv", string.Empty);
                    Props.OpenFile(NewFolder + "\\" + tbName.Text + ".rcs");
                    tbName.Text = "";
                    mf.LoadSettings();
                    UpdateForm();
                    MainMenu.ChangeProduct(0);
                    MainMenu.ShowProfile();
                }
                else
                {
                    mf.Tls.ShowMessage("Invalid file name.");
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuProfiles/btnNew_Click: " + ex.Message);
            }
        }

        private void frmMenuProfiles_Load(object sender, EventArgs e)
        {
            SubMenuLayout.SetFormLayout(this, MainMenu, null);
            PositionForm();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            MainMenu.StyleControls(this);
            SetLanguage();
            UpdateForm();
            lbProfile.Font = new Font(lbProfile.Font, lbProfile.Font.Style | FontStyle.Underline);
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

        private void SetLanguage()
        {
            //grpSensor.Text = Lang.lgSensorLocation;
        }

        private void UpdateForm()
        {
            lstProfiles.Items.Clear();
            string[] folders = Directory.GetDirectories(Props.ProfilesFolder);
            foreach (string folder in folders)
            {
                lstProfiles.Items.Add(Path.GetFileName(folder));
            }
            lbProfile.Text = "Current Profle:  " + Props.CurrentFileName();
        }

        private void btnProfilesUp_Click(object sender, EventArgs e)
        {
            int itemsPerPage = lstProfiles.ClientSize.Height / lstProfiles.ItemHeight; // Calculate visible items
            lstProfiles.TopIndex = Math.Max(0, lstProfiles.TopIndex - itemsPerPage); // Move up by 1 page
        }

        private void btnProfilesDown_Click(object sender, EventArgs e)
        {
            int itemsPerPage = lstProfiles.ClientSize.Height / lstProfiles.ItemHeight; // Calculate visible items
            lstProfiles.TopIndex = Math.Min(lstProfiles.Items.Count - 1, lstProfiles.TopIndex + itemsPerPage); // Move down by 1 page
        }
    }
}