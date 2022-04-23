using Configurator;

namespace RateController
{
    public partial class FormAbout : Form
    {
        private frmMain mf;

        public FormAbout(frmMain CallingForm)
        {
            InitializeComponent();

            #region // language

            label27.Text = Configurator.Languages.Lang.lgLocalIP;
            bntOK.Text = Configurator.Languages.Lang.lgClose;

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
        }

        private void FormAbout_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);

            lbAppVersion.Text = mf.Tls.AppVersion() + "    " + mf.Tls.VersionDate();
            lbIP.Text = mf.UDPmodulesConfig.LocalIP();
            SetDayMode();
        }

        private void SetDayMode()
        {
            if (Configurator.Properties.Settings.Default.IsDay)
            {
                this.BackColor = Configurator.Properties.Settings.Default.DayColour;
                foreach (Control c in this.Controls)
                {
                    c.ForeColor = Color.Black;
                }
            }
            else
            {
                this.BackColor = Configurator.Properties.Settings.Default.NightColour;
                foreach (Control c in this.Controls)
                {
                    c.ForeColor = Color.White;
                }
            }
        }
    }
}