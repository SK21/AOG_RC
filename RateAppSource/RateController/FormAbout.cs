using System;
using System.Windows.Forms;

namespace RateController
{
    public partial class FormAbout : Form
    {
        private FormStart mf;

        public FormAbout(FormStart CallingForm)
        {
            InitializeComponent();

            #region // language

            label27.Text = Lang.lgLocalIP;
            this.Text = Lang.lgAbout;
            lbVersion.Text = Lang.lgVersion;

            #endregion // language

            mf = CallingForm;
        }

        private void bntOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormAbout_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                mf.Tls.SaveFormData(this);
            }
            timer1.Enabled = false;
            mf.SendStatusPGN = false;
        }

        private void FormAbout_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);

            lbAppVersion.Text = mf.Tls.AppVersion() + "    " + mf.Tls.VersionDate();
            lbIP.Text = mf.UDPmodules.EthernetIP();
            lbWifi.Text = mf.UDPmodules.WifiIP();
            this.BackColor = Properties.Settings.Default.DayColour;
            timer1.Enabled = true;
            mf.SendStatusPGN = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lbInoID.Text = mf.ModuleStatus.InoID.ToString();
            lbModID.Text = mf.ModuleStatus.ModuleID.ToString();
        }
    }
}