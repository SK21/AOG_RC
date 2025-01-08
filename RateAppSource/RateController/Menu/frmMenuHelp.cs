using System;
using System.IO;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuHelp : Form
    {
        private bool FreezeUpdate;
        private frmMenu MainMenu;
        private FormStart mf;

        public frmMenuHelp(FormStart main, frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            mf = main;
            this.Tag = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                File.WriteAllText(mf.Tls.FilesDir() + "\\Ethernet Log.txt", tbEthernet.Text);
                mf.Tls.ShowHelp("File saved.", "Save", 10000);
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmModules/btnSave_Click: " + ex.Message);
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
            if (!mf.Tls.OpenTextFile("Activity Log.txt"))
            {
                mf.Tls.ShowHelp("File not found.");
            }
        }

        private void butErrors_Click(object sender, EventArgs e)
        {
            if (!mf.Tls.OpenTextFile("Error Log.txt"))
            {
                mf.Tls.ShowHelp("File not found.");
            }
        }

        private void butGraph_Click(object sender, EventArgs e)
        {
            Form fs = mf.Tls.IsFormOpen("frmMenuRateGraph");

            if (fs == null)
            {
                Form frm = new frmMenuRateGraph(mf);
                frm.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private void butNet_Click(object sender, EventArgs e)
        {
            mf.UDPmodules.UpdateLog();
            if (!mf.Tls.OpenTextFile("Ethernet Log.txt"))
            {
                mf.Tls.ShowHelp("File not found.");
            }
        }

        private void frmMenuEthernet_FormClosed(object sender, FormClosedEventArgs e)
        {
            mf.Tls.SaveFormData(this);
            timer1.Enabled = false;
        }

        private void frmMenuEthernet_Load(object sender, EventArgs e)
        {
            // menu 800,600
            // sub menu 540,630
            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            mf.Tls.LoadFormData(this, "", false);
            this.BackColor = Properties.Settings.Default.BackColour;
            this.Width = MainMenu.Width - 260;
            this.Height = MainMenu.Height - 50;
            MainMenu.StyleControls(this);
            PositionForm();
            timer1.Enabled = true;
            lbIP.Text = mf.UDPmodules.SubNet;
            lbAppVersion.Text = mf.Tls.AppVersion();
            lbDate.Text = mf.Tls.VersionDate();

            lbModID.Font = new System.Drawing.Font("Tahoma", 14, System.Drawing.FontStyle.Bold);
            lbInoID.Font = new System.Drawing.Font("Tahoma", 14, System.Drawing.FontStyle.Bold);
            lbTime.Font = new System.Drawing.Font("Tahoma", 14, System.Drawing.FontStyle.Bold);
            lbIP.Font = new System.Drawing.Font("Tahoma", 14, System.Drawing.FontStyle.Bold);
            lbAppVersion.Font = new System.Drawing.Font("Tahoma", 14, System.Drawing.FontStyle.Bold);
            lbDate.Font = new System.Drawing.Font("Tahoma", 14, System.Drawing.FontStyle.Bold);
            lbFile.Font = new System.Drawing.Font("Tahoma", 14, System.Drawing.FontStyle.Bold);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                VisitLink("https://github.com/SK21/AOG_RC/tree/master/Help");
                linkLabel1.LinkVisited = true;
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp(ex.Message, "Help", 15000, true);
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                VisitLink("https://github.com/SK21/AOG_RC/wiki");
                linkLabel2.LinkVisited = true;
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp(ex.Message, "Help", 15000, true);
            }
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                VisitLink("https://github.com/AgHardware/Rate_Control");
                linkLabel3.LinkVisited = true;
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp(ex.Message, "Help", 15000, true);
            }
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                VisitLink("https://github.com/SK21/PCBsetup");
                linkLabel4.LinkVisited = true;
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp(ex.Message, "Help", 15000, true);
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

        private void SetLanguage()
        {
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            clsProduct Prod = mf.Products.Item(mf.CurrentProduct());

            if (Prod.ElapsedTime > 4000)
            {
                lbInoID.Text = "--";
                lbModID.Text = "--";
                lbTime.Text = "--";
            }
            else
            {
                lbInoID.Text = mf.ModulesStatus.InoID(Prod.ModuleID).ToString();
                lbModID.Text = Prod.ModuleID.ToString();
                lbTime.Text = (Prod.ElapsedTime / 1000.0).ToString("N3");
            }

            if (!FreezeUpdate)
            {
                tbEthernet.Text = mf.UDPmodules.Log();
                tbEthernet.Select(tbEthernet.Text.Length, 0);
                tbEthernet.ScrollToCaret();
            }

            lbIP.Text = mf.UDPmodules.SubNet;
            lbFile.Text = Path.GetFileNameWithoutExtension(Properties.Settings.Default.FileName);
        }

        private void VisitLink(string Link)
        {
            System.Diagnostics.Process.Start(Link);
        }
    }
}