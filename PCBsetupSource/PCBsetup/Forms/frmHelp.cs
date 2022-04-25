using System;
using System.Drawing;
using System.Windows.Forms;

namespace PCBsetup.Forms
{
    public partial class frmHelp : Form
    {
        private frmMain mf;

        public frmHelp(frmMain CallingForm, string Message, string Title = "Help", int timeInMsec = 30000)
        {
            mf = CallingForm;
            InitializeComponent();
            this.Text = Title;
            label1.Text = Message;
            timer1.Interval = timeInMsec;

            int len = Message.Length;
            this.Width = 450;

            int ht = 20 + (len / 34) * 40;
            if (ht < 150)
            {
                ht = 150;
            }
            else if (ht > 500)
            {
                ht = 500;
            }

            this.Height = ht;

            panel1.Width = this.Width - 40;
            panel1.Height = this.Height - 40;
            label1.MaximumSize = new Size(panel1.Width - 10, 0);
        }

        private void frmHelp_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmHelp_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                mf.Tls.SaveFormData(this);
            }
        }

        private void frmHelp_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            this.BackColor = PCBsetup.Properties.Settings.Default.DayColour;
            timer1.Enabled = true;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            timer1.Dispose();
            Dispose();

            if (this.WindowState == FormWindowState.Normal)
            {
                mf.Tls.SaveFormData(this);
            }
            Close();
        }
    }
}