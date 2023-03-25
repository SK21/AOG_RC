using ArduinoUploader;
using ArduinoUploader.Hardware;
using System;
using System.ComponentModel;
using System.IO;
using System.Timers;
using System.Windows.Forms;

namespace PCBsetup.Forms
{
    public partial class frmSwitchboxFirmware : Form
    {
        // requires the NuGet packages ArduinoUploader, IntelHexFormatReader, SerialPortStream
        // right click on project name in the solution explorer and select "Manage NuGet Packages..."

        public CheckBox[] CKs;
        public System.Timers.Timer timer = new System.Timers.Timer(1000);
        private frmMain mf;
        private int ProgressCount;
        private DateTime StartUpload;
        private UploadResult UploadStatus;
        private bool UserSelectedFile = true;
        private BackgroundWorker worker = new BackgroundWorker();

        public frmSwitchboxFirmware(frmMain CallingForm)
        {
            InitializeComponent();
            mf = CallingForm;

            CKs = new CheckBox[] { ckSWOldBootloader };
        }

        private void bntOK_Click(object sender, EventArgs e)
        {
            SaveSettings();
            Close();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            UserSelectedFile = true;
            tbHexfile.Text = "";
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "hex files (*.hex)|*.hex|All files (*.*)|*.*";
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
            tbHexfile.Text = "Default file version date:" + mf.Tls.SwitchboxFirmwareVersion();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                if (mf.CommPort.IsOpen())   // to prove comm port exists
                {
                    mf.CommPort.Close();
                    progressBar1.Visible = true;
                    ProgressCount = 0;
                    bntOK.Enabled = false;
                    StartUpload = DateTime.Now;
                    WatchDogTimer.Enabled = true;
                    worker.RunWorkerAsync();
                }
                else
                {
                    mf.Tls.ShowHelp("Comm Port not open.", this.Text, 3000);
                }
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp(ex.Message, this.Text, 3000, true);
                bntOK.Enabled = true;
            }
        }

        private void frmSwitchboxFirmware_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                mf.Tls.SaveFormData(this);
            }
        }

        private void frmSwitchboxFirmware_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);

            this.BackColor = PCBsetup.Properties.Settings.Default.DayColour;
            LoadSettings();

            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted +=
              new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

            UserSelectedFile = false;
            tbHexfile.Text = "Default file version date:" + mf.Tls.SwitchboxFirmwareVersion();

            lbWarning.Visible = !ckSWOldBootloader.Checked;
        }

        private void LoadSettings()
        {
            try
            {
                // check boxes
                bool Checked = false;
                for (int i = 0; i < CKs.Length; i++)
                {
                    bool.TryParse(mf.Tls.LoadProperty(CKs[i].Name), out Checked);
                    CKs[i].Checked = Checked;
                }
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp(ex.Message, this.Text, 3000, true);
            }
        }

        private void SaveSettings()
        {
            try
            {
                // check boxes
                for (int i = 0; i < CKs.Length; i++)
                {
                    mf.Tls.SaveProperty(CKs[i].Name, CKs[i].Checked.ToString());
                }
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp(ex.Message, this.Text, 3000, true);
            }
        }

        private void timer_elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                ProgressCount++;
                int ProgressPercent = (ProgressCount * 100) / 25;
                if (ProgressPercent > 100) ProgressPercent = 100;
                if (ProgressPercent < 0) ProgressPercent = 0;

                progressBar1.BeginInvoke(new Action(() => progressBar1.Value = ProgressPercent));
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog(this.Text + "/timer_elapsed " + ex.Message);
            }
        }

        private void WatchDogTimer_Tick(object sender, EventArgs e)
        {
            if ((DateTime.Now - StartUpload).TotalSeconds > 30)
            {
                timer.Stop();
                WatchDogTimer.Enabled = false;
                mf.Tls.ShowHelp("Upload failed, try switching bootloaders. App will restart.", this.Text, 30000, false, true);
                Application.Restart();
            }
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                string HexFile = "";
                UploadStatus = UploadResult.Failed;

                if (UserSelectedFile)
                {
                    HexFile = tbHexfile.Text;
                }
                else
                {
                    if (ckSWOldBootloader.Checked)
                    {
                        // old bootloader
                        HexFile = Path.GetTempFileName();
                        File.WriteAllBytes(HexFile, PCBsetup.Properties.Resources.SWarduinoOB_ino);
                    }
                    else 
                    {
                        // new bootloader
                        HexFile = Path.GetTempFileName();
                        File.WriteAllBytes(HexFile, PCBsetup.Properties.Resources.SWarduinoNB_ino);
                    }
                }

                if (File.Exists(HexFile))
                {
                    timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_elapsed);
                    timer.Start();

                    ArduinoModel Model;
                    if (ckSWOldBootloader.Checked)
                    {
                        Model = ArduinoModel.NanoR3;
                    }
                    else
                    {
                        // work-around for new bootloader
                        // https://github.com/twinearthsoftware/ArduinoSketchUploader/issues/32
                        Model = ArduinoModel.UnoR3;
                    }

                    var uploader = new ArduinoSketchUploader(
                    new ArduinoSketchUploaderOptions()
                    {
                        FileName = @HexFile,
                        PortName = mf.SelectedPortName(),
                        ArduinoModel = Model
                    });

                    uploader.UploadSketch();
                    timer.Stop();
                    UploadStatus = UploadResult.Completed;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("sync"))
                {
                    UploadStatus = UploadResult.Sync_Error;
                }

                timer.Stop();
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            switch (UploadStatus)
            {
                case UploadResult.Completed:
                    progressBar1.Value = progressBar1.Maximum;
                    mf.Tls.ShowHelp("File uploaded.", this.Text, 5000, false, true);
                    break;

                case UploadResult.Sync_Error:
                    mf.Tls.ShowHelp("Sync error, switch bootloaders and try again.", this.Text, 5000, false, true);
                    break;

                default:
                    mf.Tls.ShowHelp("File could not be uploaded.", this.Text, 5000, false, true);
                    break;
            }

            mf.CommPort.Open();
            bntOK.Enabled = true;
            WatchDogTimer.Enabled = false;
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

        private void ckSWOldBootloader_CheckedChanged(object sender, EventArgs e)
        {
            lbWarning.Visible = !ckSWOldBootloader.Checked;
        }
    }
}