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
            lbVersionDate.Text = Lang.lgVersionDate;

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

            lbAppVersion.Text = mf.Tls.AppVersion();
            lbDate.Text = mf.Tls.VersionDate();
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
            int Elapsed = mf.Products.Item(mf.CurrentProduct()).ElapsedTime;
            if (Elapsed < 4000)
            {
                lbTime.Text = (Elapsed / 1000.0).ToString("N3");
            }
            else
            {
                lbTime.Text = "--";
            }
        }
    }
}