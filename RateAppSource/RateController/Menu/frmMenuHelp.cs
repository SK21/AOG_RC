﻿using RateController.Classes;
using RateController.Language;
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
                mf.Tls.ShowMessage("File not found.");
            }
        }

        private void butErrors_Click(object sender, EventArgs e)
        {
            if (!Props.ShowLog("Error Log.txt"))
            {
                mf.Tls.ShowMessage("File not found.");
            }
        }

        private void butGraph_Click(object sender, EventArgs e)
        {
            Form fs = Props.IsFormOpen("frmMenuRateGraph");

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
            if (!Props.ShowLog("Ethernet Log.txt"))
            {
                mf.Tls.ShowMessage("File not found.");
            }
        }

        private void frmMenuEthernet_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
            timer1.Enabled = false;
        }

        private void frmMenuEthernet_Load(object sender, EventArgs e)
        {
            // menu 800,600
            // sub menu 540,630
            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            this.BackColor = Properties.Settings.Default.MainBackColour;
            this.Width = MainMenu.Width - 260;
            this.Height = MainMenu.Height - 50;
            MainMenu.StyleControls(this);
            PositionForm();
            timer1.Enabled = true;
            lbIP.Text = mf.UDPmodules.SubNet;
            lbAppVersion.Text = Props.AppVersion();
            lbDate.Text = Props.VersionDate();

            lbModID.Font = new System.Drawing.Font("Tahoma", 14, System.Drawing.FontStyle.Bold);
            lbInoID.Font = new System.Drawing.Font("Tahoma", 14, System.Drawing.FontStyle.Bold);
            lbTime.Font = new System.Drawing.Font("Tahoma", 14, System.Drawing.FontStyle.Bold);
            lbIP.Font = new System.Drawing.Font("Tahoma", 14, System.Drawing.FontStyle.Bold);
            lbAppVersion.Font = new System.Drawing.Font("Tahoma", 14, System.Drawing.FontStyle.Bold);
            lbDate.Font = new System.Drawing.Font("Tahoma", 14, System.Drawing.FontStyle.Bold);
            lbFile.Font = new System.Drawing.Font("Tahoma", 14, System.Drawing.FontStyle.Bold);
            ModuleIndicator.BackColor = this.BackColor;
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
                mf.Tls.ShowMessage(ex.Message, "Help", 15000, true);
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
                mf.Tls.ShowMessage(ex.Message, "Help", 15000, true);
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
                mf.Tls.ShowMessage(ex.Message, "Help", 15000, true);
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
                mf.Tls.ShowMessage(ex.Message, "Help", 15000, true);
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
            butNet.Text = Lang.lgNetwork;
            butActivity.Text = Lang.lgActivity;
            butErrors.Text = Lang.lgErrors;
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
            lbFile.Text = Path.GetFileNameWithoutExtension(Properties.Settings.Default.CurrentFile);

            if (mf.SwitchBox.RealConnected())
            {
                ModuleIndicator.Image = Properties.Resources.On;
            }
            else
            {
                ModuleIndicator.Image = Properties.Resources.Off;
            }
        }

        private void VisitLink(string Link)
        {
            System.Diagnostics.Process.Start(Link);
        }
    }
}