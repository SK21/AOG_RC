using System;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmMonitor : Form
    {
        private int CommPort = 0; // 0-2
        private bool FreezeUpdate;
        private FormStart mf;
        

        public frmMonitor(FormStart CallingForm)
        {
            InitializeComponent();

            mf = CallingForm;
            this.BackColor = Properties.Settings.Default.DayColour;
        }

        private void bntOK_Click_1(object sender, EventArgs e)
        {
            Close();
        }

        private void cboPort1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CommPort = Convert.ToByte(cboPort1.SelectedIndex);
            PortName.Text = "(" + mf.SER[CommPort].RCportName + ")";
        }

        private void frmMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Enabled = false;
            if (this.WindowState == FormWindowState.Normal)
            {
                mf.Tls.SaveFormData(this);
            }

            for (int i = 0; i < 4; i++)
            {
                mf.Products.Item(i).DebugArduino = false;
            }
        }

        private void frmMonitor_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            timer1.Enabled = true;
            cboPort1.SelectedIndex = 0;

            for (int i = 0; i < 4; i++)
            {
                mf.Products.Item(i).DebugArduino = true;
            }
        }

        private void tbMonitor_Click(object sender, EventArgs e)
        {
            FreezeUpdate = !FreezeUpdate;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!FreezeUpdate)
            {
                tbMonitor.Text = mf.SER[CommPort].Log();
                tbMonitor.Select(tbMonitor.Text.Length, 0);
                tbMonitor.ScrollToCaret();
            }
        }
    }
}