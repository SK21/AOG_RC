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

        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                if (FileNameValidator.IsValidFolderName(tbName.Text) &&
                    FileNameValidator.IsValidFileName(tbName.Text) &&
                    !Directory.Exists(tbName.Text) &&
                    lstProfiles.SelectedIndex >= 0)
                {
                    string NewFilePath = Props.ProfilesFolder + "\\" + tbName.Text;
                    Directory.CreateDirectory(NewFilePath);

                    string OldFileName = lstProfiles.SelectedItem.ToString();
                    string OldFileFullName = Props.ProfilesFolder + "\\" + OldFileName + "\\" + OldFileName + ".rcs";
                    File.Copy(OldFileFullName, NewFilePath + "\\" + tbName.Text + ".rcs");

                    string NewFilePressureName = NewFilePath + "\\" + tbName.Text + "PressureData.csv";
                    string OldFilePressureName = Props.ProfilesFolder + "\\" + OldFileName + "\\" + OldFileName + "PressureData.csv";
                    File.Copy(OldFilePressureName, NewFilePressureName);

                    Props.OpenFile(NewFilePath + "\\" + tbName.Text + ".rcs");
                    UpdateForm();
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
                            if (Props.SafeToDelete(FilePath))
                            {
                                Directory.Delete(FilePath, true);

                                // load default if current is deleted
                                if (Properties.Settings.Default.CurrentFile == FilePath + "\\" + FileToDelete + ".rcs")
                                {
                                    string name = Props.ProfilesFolder + "\\default\\default.rcs";
                                    Props.OpenFile(name);
                                    UpdateForm();
                                    tbName.Text = "";
                                    mf.LoadSettings();
                                    UpdateForm();
                                    MainMenu.ChangeProduct(0);
                                    MainMenu.ShowProfile();
                                }
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
                if (FileNameValidator.IsValidFolderName(tbName.Text) &&
                    FileNameValidator.IsValidFileName(tbName.Text) &&
                    !Directory.Exists(tbName.Text))
                {
                    string NewFolder = Props.ProfilesFolder + "\\" + tbName.Text;
                    Directory.CreateDirectory(NewFolder);
                    File.WriteAllText(NewFolder + "\\" + tbName.Text + ".rcs", string.Empty);
                    File.WriteAllText(NewFolder + "\\" + tbName.Text + "PressureData.csv", string.Empty);
                    Props.OpenFile(NewFolder + "\\" + tbName.Text + ".rcs");
                    UpdateForm();
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
            lbProfile.Font = new Font(lbProfile.Font, lbProfile.Font.Style | FontStyle.Underline);
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
            lstProfiles.Items.Clear();
            string[] folders = Directory.GetDirectories(Props.ProfilesFolder);
            foreach (string folder in folders)
            {
                lstProfiles.Items.Add(Path.GetFileName(folder));
            }
            lbProfile.Text = "Current Profle:  " + Props.CurrentFileName();
        }
    }
}