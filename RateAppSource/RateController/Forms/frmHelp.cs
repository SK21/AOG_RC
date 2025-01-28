using System;
using System.Drawing;
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

            int len = Message.Length;
            this.Width = 450;

            int ht = 20 + (len / 34) * 40;
            if (ht < 160)
            {
                ht = 160;
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

        private void frmHelp_FormClosed(object sender, FormClosedEventArgs e)
        {
            mf.Tls.SaveFormData(this);
        }

        private void frmHelp_Load(object sender, EventArgs e)
        {
            try
            {
                mf.Tls.LoadFormData(this);
                this.BackColor = Properties.Settings.Default.MainBackColour;
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmHelp/frmHelp_Load: " + ex.Message);
            }
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

            mf.Tls.SaveFormData(this);
            Close();
        }
    }
}