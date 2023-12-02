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
        }

        private void FormAbout_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);

            lbAppVersion.Text = mf.Tls.AppVersion();
            lbDate.Text = mf.Tls.VersionDate();
            this.BackColor = Properties.Settings.Default.DayColour;
        }
    }
}