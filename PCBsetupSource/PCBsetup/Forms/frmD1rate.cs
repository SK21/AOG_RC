using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace PCBsetup.Forms
{
    public partial class frmD1rate : Form
    {
        // https://nerdiy.de/en/howto-esp8266-mit-dem-esptool-bin-dateien-unter-windows-flashen/

        private frmMain mf;
        private string NewBin;
        private string PathName;
        private bool UserSelectedFile = true;

        public frmD1rate(frmMain CallingForm)
        {
            InitializeComponent();
            mf = CallingForm;
        }

        private void bntOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            UserSelectedFile = true;
            tbHexfile.Text = "";
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "bin files (*.bin)|*.bin|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 0;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    tbHexfile.Text = openFileDialog.FileName;
                }
            }
        }

        private void btnDefault_Click(object sender, EventArgs e)
        {
            UserSelectedFile = false;
            tbHexfile.Text = "Default file version date:" + mf.Tls.D1RateFirmware();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                SetButtons(true);

                NewBin = Path.GetTempFileName();
                PathName = Path.GetDirectoryName(NewBin);
                if (UserSelectedFile)
                {
                    File.Copy(tbHexfile.Text, NewBin, true);
                }
                else
                {
                    File.WriteAllBytes(NewBin, PCBsetup.Properties.Resources.WifiAOG_ino);
                }
                File.WriteAllBytes(PathName + "//esptool.exe", PCBsetup.Properties.Resources.esptool);

                string cmd = PathName + "//esptool.exe";
                string arg = "--port " + mf.SelectedPortName() + " --baud " + "115200" + " write_flash 0x0 " + NewBin;

                Process myProcess = null;
                myProcess = Process.Start(cmd, arg);
                while (!myProcess.WaitForExit(1000)) ;
                if (myProcess.ExitCode != 0)
                {
                    mf.Tls.ShowHelp("Flash failed with arg" + arg, "Wifi Rate", 10000, true);
                }
                else
                {
                    mf.Tls.ShowHelp("Flash complete.", "Wifi Rate", 5000);
                }
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp(ex.Message, "Wifi Rate", 15000, true);
            }
            SetButtons(false);
        }

        private void frmD1rate_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                mf.Tls.SaveFormData(this);
            }
        }

        private void frmD1rate_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);

            this.BackColor = PCBsetup.Properties.Settings.Default.DayColour;

            UserSelectedFile = false;
            tbHexfile.Text = "Default file version date:" + mf.Tls.D1RateFirmware();
            SetButtons(false);
            lbPort.Text = "Serial Port: " + mf.SelectedPortName();
        }

        private void SetButtons(bool Edited)
        {
            if (Edited)
            {
                btnBrowse.Enabled = false;
                btnDefault.Enabled = false;
                btnUpload.Enabled = false;
            }
            else
            {
                btnBrowse.Enabled = true;
                btnDefault.Enabled = true;
                btnUpload.Enabled = true;
            }
        }

        private void btnUpload_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Upload to module.";

            mf.Tls.ShowHelp(Message);
            hlpevent.Handled = true;
        }

        private void btnDefault_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Load defaults.";

            mf.Tls.ShowHelp(Message);
            hlpevent.Handled = true;
        }
    }
}