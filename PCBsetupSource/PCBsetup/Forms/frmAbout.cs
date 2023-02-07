using PCBsetup.Languages;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PCBsetup.Forms
{
    public partial class frmAbout : Form
    {
        private frmMain mf;

        public frmAbout(frmMain CallingForm)
        {
            InitializeComponent();

            #region // language

            lbIPdes.Text = Lang.lgLocalIP;

            #endregion // language

            mf = CallingForm;
        }

        private void bntOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmAbout_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                mf.Tls.SaveFormData(this);
            }
        }

        private void frmAbout_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);

            lbAppVersion.Text = mf.Tls.AppVersion() + "    " + mf.Tls.VersionDate();
            lbIP.Text = mf.UDPmodulesConfig.LocalIP();
            SetDayMode();
        }

        private void SetDayMode()
        {
            if (Properties.Settings.Default.IsDay)
            {
                this.BackColor = Properties.Settings.Default.DayColour;
                textBox1.BackColor = Properties.Settings.Default.DayColour;
                foreach (Control c in this.Controls)
                {
                    c.ForeColor = Color.Black;
                }
            }
            else
            {
                this.BackColor = Properties.Settings.Default.NightColour;
                foreach (Control c in this.Controls)
                {
                    c.ForeColor = Color.White;
                }
            }
        }
    }
}