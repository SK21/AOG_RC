using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RateController
{
    public partial class FormAbout : Form
    {
        private FormSettings cf;

        public FormAbout(FormSettings CallingForm)
        {
            InitializeComponent();
            cf = CallingForm;
        }

        private void FormAbout_Load(object sender, EventArgs e)
        {
            cf.mf.Tls.LoadFormData(this);

            lbNetworkIP.Text = cf.mf.UDPnetwork.LocalIP();
            lbLocalIP.Text = cf.mf.LoopBackIP;
            lbVersion.Text = Lang.lgVersionDate + "   " + cf.mf.Tls.VersionDate();
            lbDestinationIP.Text = cf.mf.UDPnetwork.BroadcastIP();
            SetDayMode();
        }

        private void btnDay_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.IsDay)
            {
                Properties.Settings.Default.IsDay = false;
            }
            else
            {
                Properties.Settings.Default.IsDay = true;
            }
            SetDayMode();
        }

        private void FormAbout_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                cf.mf.Tls.SaveFormData(this);
            }
        }

        private void bntOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SetDayMode()
        {
            this.BackColor = Properties.Settings.Default.DayColour;
            if (Properties.Settings.Default.IsDay)
            {
                btnDay.Image = Properties.Resources.WindowDayMode;
            }
            else
            {
                btnDay.Image = Properties.Resources.WindowNightMode;
            }
        }

        private void groupBox3_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            cf.mf.Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);

        }

        private void groupBox4_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            cf.mf.Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }
    }
}
