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
    public partial class frmHelp : Form
    {
        private FormStart mf;

        public frmHelp(FormStart CallingForm, string Message, string Title = "Help", int timeInMsec = 30000)
        {
            mf = CallingForm;
            InitializeComponent();
            this.Text = Title;
            label1.Text = Message;
            timer1.Interval = timeInMsec;

            if (Message.Length < 40)
            {
                this.Height = 200;
                this.Width = 250;
            }
            else if (Message.Length < 100)
            {
                this.Height = 250;
                this.Width = 350;
            }
            else
            {
                this.Height = 300;
                this.Width = 500;
            }

            panel1.Width = this.Width - 40;
            panel1.Height = this.Height - 136;
            label1.MaximumSize = new Size(panel1.Width - 10, 0);
            bntOK.Location = new Point((this.Width - bntOK.Width) / 2, this.Height - 118);
        }

        private void bntOK_Click(object sender, EventArgs e)
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

        private void frmHelp_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
        }

        private void frmHelp_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                mf.Tls.SaveFormData(this);
            }
        }
    }
}
