using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCBsetup.Forms
{
    public partial class frmMonitor : Form
    {
        private frmMain mf;
        private bool FreezeUpdate;

        public frmMonitor(frmMain CallingForm)
        {
            InitializeComponent();
            mf = CallingForm;
            this.BackColor = PCBsetup.Properties.Settings.Default.DayColour;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!FreezeUpdate)
            {
                tbMonitor.Text = mf.CommPort.Log();
                tbMonitor.Select(tbMonitor.Text.Length, 0);
                tbMonitor.Focus();
                tbMonitor.ScrollToCaret();
            }
        }

        private void frmMonitor_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            timer1.Enabled = true;
        }

        private void frmMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Enabled = false;
            if (this.WindowState == FormWindowState.Normal)
            {
                mf.Tls.SaveFormData(this);
            }
        }

        private void bntOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tbMonitor_Click(object sender, EventArgs e)
        {
            FreezeUpdate = !FreezeUpdate;
        }
    }
}
