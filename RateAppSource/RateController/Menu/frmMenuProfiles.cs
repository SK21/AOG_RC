using RateController.Classes;
using RateController.Language;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (FileNameValidator.IsValidFolderName(tbName.Text) &&
                FileNameValidator.IsValidFileName(tbName.Text) &&
                !Directory.Exists(tbName.Text))
            {
                string NewFolder = Props.ProfilesFolder + "\\" + tbName.Text;
                Directory.CreateDirectory(NewFolder);
                File.WriteAllText(NewFolder + "\\" + tbName.Text + ".rcs", string.Empty);
                File.WriteAllText(NewFolder + "\\" + tbName.Text + "PressureData.csv", string.Empty);
                UpdateForm();
                tbName.Text = "";
            }
            else
            {
                mf.Tls.ShowMessage("Invalid file name.");
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
            lbProfile.Text ="Current Profle:  "+ Props.CurrentFileName();
        }
    }
}