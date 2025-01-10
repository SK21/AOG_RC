using RateController.Language;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmModule : Form
    {
        private int CommPort = 0;// 0-2
        private bool FreezeUpdate;
        private FormStart mf;

        public frmModule(FormStart CallingForm)
        {
            InitializeComponent();

            #region // language

            label27.Text = Lang.lgSubnet;
            this.Text = Lang.lgCommDiagnostics;

            #endregion // language

            mf = CallingForm;
        }

        private void bntOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            string FileName = "";
            switch (tabControl1.SelectedTab.Name)
            {
                case "tabPage1":
                    FileName = "Ethernet Log.txt";
                    mf.UDPmodules.UpdateLog();
                    break;

                case "tabPage2":
                    FileName = "Serial Log.txt";
                    mf.SER[CommPort].UpdateLog();
                    break;

                case "tabPage3":
                    FileName = "Activity Log.txt";
                    break;

                case "tabPage4":
                    FileName = "Error Log.txt";
                    break;
            }

            if (!mf.Tls.OpenTextFile(FileName))
            {
                mf.Tls.ShowHelp("File not found.");
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

        private void cboPort1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CommPort = Convert.ToByte(cboPort1.SelectedIndex);
            PortName.Text = "(" + mf.SER[CommPort].RCportName + ")";
        }

        private void frmModule_FormClosed(object sender, FormClosedEventArgs e)
        {
            mf.Tls.SaveFormData(this);
            timer1.Enabled = false;
        }

        private void frmModule_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            lbIP.Text = mf.UDPmodules.SubNet;
            lbAppVersion.Text = mf.Tls.AppVersion();
            lbDate.Text = mf.Tls.VersionDate();
            timer1.Enabled = true;

            this.BackColor = Properties.Settings.Default.MainBackColour;
            foreach (Control c in this.Controls)
            {
                c.ForeColor = Color.Black;
            }

            foreach (TabPage p in tabControl1.Controls)
            {
                p.BackColor = this.BackColor;
            }

            tbEthernet.BackColor = this.BackColor;
            tbSerial.BackColor = this.BackColor;
            tbActivity.BackColor = this.BackColor;
            tbErrors.BackColor = this.BackColor;
            ModuleIndicator.BackColor = this.BackColor;

            cboPort1.SelectedIndex = 0;
            UpdateForm();
            timer1_Tick(this, null);
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

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateForm();
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
                tbSerial.Text = mf.SER[CommPort].Log();
                tbSerial.Select(tbSerial.Text.Length, 0);
                tbSerial.ScrollToCaret();

                tbEthernet.Text = mf.UDPmodules.Log();
                tbEthernet.Select(tbEthernet.Text.Length, 0);
                tbEthernet.ScrollToCaret();

                UpdateForm();
            }

            lbIP.Text = mf.UDPmodules.SubNet;
            lbFile.Text = Path.GetFileNameWithoutExtension(Properties.Settings.Default.FileName);

            if (mf.SwitchBox.RealConnected())
            {
                ModuleIndicator.Image = Properties.Resources.On;
            }
            else
            {
                ModuleIndicator.Image = Properties.Resources.Off;
            }
        }

        private void UpdateForm()
        {
            tbActivity.Text = mf.Tls.ReadTextFile("Activity Log.txt");
            tbActivity.Select(tbActivity.Text.Length, 0);
            tbActivity.ScrollToCaret();

            tbErrors.Text = mf.Tls.ReadTextFile("Error Log.txt");
            tbErrors.Select(tbErrors.Text.Length, 0);
            tbErrors.ScrollToCaret();

            btnOpen.Visible = (tabControl1.SelectedTab.Name != "tabPage5");
            btnStart.Visible = btnOpen.Visible;
        }

        private void VisitLink(string Link)
        {
            System.Diagnostics.Process.Start(Link);
        }
    }
}