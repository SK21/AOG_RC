using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmModule : Form
    {
        private int CommPort = 0;// 0-2
        private bool FreezeUpdate;
        private FormStart mf;

        public frmModule(FormStart CallingForm)
        {
            InitializeComponent();

            #region // language

            label27.Text = Lang.lgLocalIP;
            this.Text = Lang.lgCommDiagnostics;

            #endregion // language

            mf = CallingForm;
        }

        private void bntOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnConnect1_Click(object sender, EventArgs e)
        {
            if (btnConnect1.Text == Lang.lgConnect)
            {
                mf.SER[CommPort].OpenRCport();
            }
            else
            {
                mf.SER[CommPort].CloseRCport();
            }
            SetPortButton();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            FreezeUpdate = false;
            btnStart.Enabled = false;
            btnStop.Enabled = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            FreezeUpdate = true;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        private void cboPort1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CommPort = Convert.ToByte(cboPort1.SelectedIndex);
            PortName.Text = "(" + mf.SER[CommPort].RCportName + ")";
        }

        private void frmModule_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                mf.Tls.SaveFormData(this);
            }
            timer1.Enabled = false;
            mf.SendStatusPGN = false;
        }

        private void frmModule_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            lbIP.Text = mf.UDPmodules.EthernetIP();
            lbWifi.Text = mf.UDPmodules.WifiIP();
            timer1.Enabled = true;
            mf.SendStatusPGN = true;

            this.BackColor = Properties.Settings.Default.DayColour;
            foreach (Control c in this.Controls)
            {
                c.ForeColor = Color.Black;
            }

            tabControl1.TabPages[0].BackColor = this.BackColor;
            tabControl1.TabPages[1].BackColor = this.BackColor;
            tbEthernet.BackColor = this.BackColor;
            tbSerial.BackColor = this.BackColor;

            cboPort1.SelectedIndex = 0;
            SetPortButton();
        }

        private void SetPortButton()
        {
            if (mf.SER[CommPort].ArduinoPort.IsOpen)
            {
                cboPort1.Enabled = false;
                btnConnect1.Text = Lang.lgDisconnect;
            }
            else
            {
                cboPort1.Enabled = true;
                btnConnect1.Text = Lang.lgConnect;
            }
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

            if (!FreezeUpdate)
            {
                tbSerial.Text = mf.SER[CommPort].Log();
                tbSerial.Select(tbSerial.Text.Length, 0);
                tbSerial.ScrollToCaret();

                tbEthernet.Text = mf.UDPmodules.Log();
                tbEthernet.Select(tbEthernet.Text.Length, 0);
                tbEthernet.ScrollToCaret();
            }
        }
    }
}