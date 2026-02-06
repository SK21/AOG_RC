using RateController.Classes;
using RateController.Forms;
using RateController.Language;
using System;
using System.IO;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuHelp : Form
    {
        private string AppURL = "https://github.com/SK21/AOG_RC";
        private int cModuleID;
        private bool FreezeUpdate;
        private frmMenu MainMenu;
        private string PCBsetupURL = "https://github.com/SK21/PCBsetup";
        private string PCBsURL = "https://github.com/AgOpenGPS-Official/Rate_Control";

        public frmMenuHelp(frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            this.Tag = false;
        }

        private void AppLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                VisitLink(AppURL);
                AppLink.LinkVisited = true;
            }
            catch (Exception ex)
            {
                Props.ShowMessage(ex.Message, "Help", 15000, true);
            }
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            if (cModuleID > 0)
            {
                cModuleID--;
                UpdateForm();
            }
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            if (cModuleID < Props.MaxModules)
            {
                cModuleID++;
                UpdateForm();
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (FreezeUpdate)
            {
                FreezeUpdate = false;
                btnStart.Image = Properties.Resources.Pause;
            }
            else
            {
                FreezeUpdate = true;
                btnStart.Image = Properties.Resources.Start;
            }
        }

        private void butActivity_Click(object sender, EventArgs e)
        {
            if (!Props.ShowLog("Activity Log.txt"))
            {
                Props.ShowMessage("File not found.");
            }
        }

        private void butErrors_Click(object sender, EventArgs e)
        {
            if (!Props.ShowLog("Error Log.txt"))
            {
                Props.ShowMessage("File not found.");
            }
        }

        private void butGraph_Click(object sender, EventArgs e)
        {
            Form fs = Props.IsFormOpen("frmMenuRateGraph");

            if (fs == null)
            {
                Form frm = new frmMenuRateGraph();
                frm.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private void butNet_Click(object sender, EventArgs e)
        {
            Core.UDPmodules.UpdateLog();
            if (!Props.ShowLog("Ethernet Log.txt"))
            {
                Props.ShowMessage("File not found.");
            }
        }

        private void butVersion_Click(object sender, EventArgs e)
        {
            Form fs = Props.IsFormOpen("frmVersion");

            if (fs == null)
            {
                Form frm = new frmVersion();
                frm.Show();
            }
        }

        private void frmMenuEthernet_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
            timer1.Enabled = false;
        }

        private void frmMenuEthernet_Load(object sender, EventArgs e)
        {
            SubMenuLayout.SetFormLayout(this, MainMenu, null);
            MainMenu.StyleControls(this);
            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            this.BackColor = Properties.Settings.Default.MainBackColour;
            PositionForm();
            timer1.Enabled = true;
            lbIP.Text = Core.UDPmodules.SubNet;
            lbAppVersion.Text = Props.AppVersion();
            lbDate.Text = Props.VersionDate();

            lbModID.Font = new System.Drawing.Font("Tahoma", 14, System.Drawing.FontStyle.Bold);
            lbInoID.Font = new System.Drawing.Font("Tahoma", 14, System.Drawing.FontStyle.Bold);
            lbTime.Font = new System.Drawing.Font("Tahoma", 14, System.Drawing.FontStyle.Bold);
            lbIP.Font = new System.Drawing.Font("Tahoma", 14, System.Drawing.FontStyle.Bold);
            lbAppVersion.Font = new System.Drawing.Font("Tahoma", 14, System.Drawing.FontStyle.Bold);
            lbDate.Font = new System.Drawing.Font("Tahoma", 14, System.Drawing.FontStyle.Bold);
            lbFile.Font = new System.Drawing.Font("Tahoma", 14, System.Drawing.FontStyle.Bold);
            lbLon.Font = new System.Drawing.Font("Tahoma", 14, System.Drawing.FontStyle.Bold);
            lbLat.Font = new System.Drawing.Font("Tahoma", 14, System.Drawing.FontStyle.Bold);
            ModuleIndicator.BackColor = this.BackColor;
            cModuleID = Core.Products.Item(Props.CurrentProduct).ModuleID;
        }

        private void MainMenu_MenuMoved(object sender, EventArgs e)
        {
            PositionForm();
        }

        private void PCBsetupLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                VisitLink(PCBsetupURL);
                PCBsetupLink.LinkVisited = true;
            }
            catch (Exception ex)
            {
                Props.ShowMessage(ex.Message, "Help", 15000, true);
            }
        }

        private void PCBsLink_Clicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                VisitLink(PCBsURL);
                PCBsLink.LinkVisited = true;
            }
            catch (Exception ex)
            {
                Props.ShowMessage(ex.Message, "Help", 15000, true);
            }
        }

        private void PositionForm()
        {
            this.Top = MainMenu.Top + SubMenuLayout.TopOffset;
            this.Left = MainMenu.Left + SubMenuLayout.LeftOffset;
        }

        private void SetLanguage()
        {
            butNet.Text = Lang.lgNetwork;
            butActivity.Text = Lang.lgActivity;
            butErrors.Text = Lang.lgErrors;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void UpdateForm()
        {
            if (Core.ModulesStatus.ElapsedTime(cModuleID) > 4000)
            {
                lbInoID.Text = "--";
                lbModID.Text = "--";
                lbTime.Text = "--";
            }
            else
            {
                lbInoID.Text = Core.ModulesStatus.ModuleVersion(cModuleID);
                lbModID.Text = cModuleID.ToString();
                lbTime.Text = (Core.ModulesStatus.ElapsedTime(cModuleID) / 1000.0).ToString("N3");
            }

            if (!FreezeUpdate)
            {
                // Show ISOBUS log if enabled, otherwise Ethernet log
                if (Props.IsobusEnabled && Core.IsobusComm != null)
                {
                    tbEthernet.Text = Core.IsobusComm.Log();
                }
                else
                {
                    tbEthernet.Text = Core.UDPmodules.Log();
                }
                tbEthernet.Select(tbEthernet.Text.Length, 0);
                tbEthernet.ScrollToCaret();
            }

            lbIP.Text = Core.UDPmodules.SubNet;
            lbFile.Text = Path.GetFileNameWithoutExtension(Properties.Settings.Default.CurrentFile);

            if (Core.SwitchBox.RealConnected())
            {
                ModuleIndicator.Image = Properties.Resources.On;
            }
            else
            {
                ModuleIndicator.Image = Properties.Resources.Off;
            }

            lbLon.Text = Core.GPS.Longitude.ToString("N7");
            lbLat.Text = Core.GPS.Latitude.ToString("N7");
        }

        private void VisitLink(string Link)
        {
            System.Diagnostics.Process.Start(Link);
        }
    }
}