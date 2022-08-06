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
    public partial class frmMonitor : Form
    {
        private  FormStart mf;
        private bool FreezeUpdate;
        
        public frmMonitor(FormStart CallingForm)
        {
            InitializeComponent();
            mf = CallingForm;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!FreezeUpdate)
            {
                tbMonitor.Text = mf.SER[0].Log();
                tbMonitor.Select(tbMonitor.Text.Length, 0);
                tbMonitor.Focus();
                tbMonitor.ScrollToCaret();
            }
        }

        private void frmMonitor_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            timer1.Enabled = true;
            mf.Products.Item(0).DebugArduino = true;
        }

        private void frmMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Enabled = false;
            if (this.WindowState == FormWindowState.Normal)
            {
                mf.Tls.SaveFormData(this);
            }
            mf.Products.Item(0).DebugArduino = false;
        }

        private void tbMonitor_Click(object sender, EventArgs e)
        {
            FreezeUpdate = !FreezeUpdate;
        }

        private void bntOK_Click_1(object sender, EventArgs e)
        {
            Close();
        }
    }
}
