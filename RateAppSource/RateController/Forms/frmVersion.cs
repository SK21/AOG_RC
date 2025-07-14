using RateController.Classes;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.Forms
{
    public partial class frmVersion : Form
    {
        private FormStart mf;

        public frmVersion(FormStart CallingForm)
        {
            InitializeComponent();
            mf = CallingForm;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmVersion_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
        }

        private void frmVersion_Load(object sender, EventArgs e)
        {
            Props.LoadFormLocation(this);
            this.BackColor = Properties.Settings.Default.MainBackColour;
            SetLanguage();
        }

        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
            mf.Tls.DrawGroupBox((GroupBox)sender, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                VisitLink("https://github.com/SK21/AOG_RC/releases");
                linkLabel4.LinkVisited = true;
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

        private void SetLanguage()
        {
        }

        private void VisitLink(string Link)
        {
            System.Diagnostics.Process.Start(Link);
        }
    }
}