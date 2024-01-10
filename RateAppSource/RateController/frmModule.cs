using System;
using System.Drawing;
using System.IO;
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

            label27.Text = Lang.lgSubnet;
            this.Text = Lang.lgCommDiagnostics;

            #endregion // language

            mf = CallingForm;
        }

        private void bntOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                File.WriteAllText(mf.Tls.FilesDir() + "\\Ethernet.txt", tbEthernet.Text);
                File.WriteAllText(mf.Tls.FilesDir() + "\\Serial.txt", tbSerial.Text);
                File.WriteAllText(mf.Tls.FilesDir() + "\\Activity.txt", tbActivity.Text);
                File.WriteAllText(mf.Tls.FilesDir() + "\\Errors.txt", tbErrors.Text);
                mf.Tls.ShowHelp("File saved.", "Save", 10000);
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmModules/btnSave_Click: " + ex.Message);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (FreezeUpdate)
            {
                FreezeUpdate = false;
                btnStart.Image = Properties.Resources.Stop;
            }
            else
            {
                FreezeUpdate = true;
                btnStart.Image = Properties.Resources.Start;
            }
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
        }

        private void frmModule_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            lbIP.Text = mf.UDPmodules.SubNet;
            lbAppVersion.Text = mf.Tls.AppVersion();
            lbDate.Text = mf.Tls.VersionDate();
            timer1.Enabled = true;

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
            UpdateLogs();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateLogs();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lbInoID.Text = mf.AnalogData.InoID.ToString();
            lbModID.Text = mf.AnalogData.ModuleID.ToString();
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

                UpdateLogs();
            }

            if (!mf.SER[CommPort].ArduinoPort.IsOpen) mf.SER[CommPort].OpenRCport(true);
        }

        private void UpdateLogs()
        {
            tbActivity.Text = mf.Tls.ReadTextFile("Activity Log.txt");
            tbActivity.Select(tbActivity.Text.Length, 0);
            tbActivity.ScrollToCaret();

            tbErrors.Text = mf.Tls.ReadTextFile("Error Log.txt");
            tbErrors.Select(tbErrors.Text.Length, 0);
            tbErrors.ScrollToCaret();
            //tbErrors.SelectionStart= tbErrors.Text.Length;
            //tbErrors.SelectionLength = 0;
        }
    }
}