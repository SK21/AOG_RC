using ArduinoUploader;
using ArduinoUploader.Hardware;
using Configurator;
using System.ComponentModel;
using System.Timers;

namespace RateController
{
    public partial class frmNanoFirmware : Form
    {
        // requires the NuGet packages ArduinoUploader, IntelHexFormatReader, SerialPortStream
        // right click on project name in the solution explorer and select "Manage NuGet Packages..."

        public CheckBox[] CKs;
        public System.Timers.Timer timer = new System.Timers.Timer(1000);
        private bool Completed;
        private frmMain mf;
        private int ProgressCount;
        private bool UserSelectedFile = false;

        private BackgroundWorker worker = new BackgroundWorker();

        public frmNanoFirmware(frmMain CallingForm)
        {
            InitializeComponent();
            mf = CallingForm;

            CKs = new CheckBox[] { ckRtEthernet, ckRtOldBootloader };
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
            tbHexfile.Text = "Default file version date:" + mf.Tls.NanoFirmwareVersion();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                mf.CommPort.CloseSCport();
                progressBar1.Visible = true;
                bntOK.Enabled = false;
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp(ex.Message, this.Text, 3000, true);
                bntOK.Enabled = true;
            }
        }

        private void frmNanoFirmware_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                mf.Tls.SaveFormData(this);
            }
        }

        private void frmNanoFirmware_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);

            this.BackColor = Configurator.Properties.Settings.Default.DayColour;
            LoadSettings();

            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted +=
              new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
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

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                string HexFile = "";
                Completed = false;

                if (UserSelectedFile)
                {
                    HexFile = tbHexfile.Text;
                }
                else
                {
                    if (ckRtEthernet.Checked && ckRtOldBootloader.Checked)
                    {
                        // ethernet, old bootloader
                        HexFile = Path.GetTempFileName();
                        File.WriteAllBytes(HexFile, Configurator.Properties.Resources.RateOB_ino);
                    }
                    else if (ckRtEthernet.Checked && !ckRtOldBootloader.Checked)
                    {
                        // ethernet, new bootloader
                        HexFile = Path.GetTempFileName();
                        File.WriteAllBytes(HexFile, Configurator.Properties.Resources.Rate_ino);
                    }
                    else if (!ckRtEthernet.Checked && !ckRtOldBootloader.Checked)
                    {
                        // no ethernet, new bootloader
                        HexFile = Path.GetTempFileName();
                        File.WriteAllBytes(HexFile, Configurator.Properties.Resources.RateNE_ino);
                    }
                    else if (!ckRtEthernet.Checked && ckRtOldBootloader.Checked)
                    {
                        // no ethernet, old bootloader
                        HexFile = Path.GetTempFileName();
                        File.WriteAllBytes(HexFile, Configurator.Properties.Resources.RateOBNE_ino);
                    }
                }

                if (File.Exists(HexFile))
                {
                    timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_elapsed);
                    timer.Start();
                    var uploader = new ArduinoSketchUploader(
                    new ArduinoSketchUploaderOptions()
                    {
                        FileName = HexFile,
                        PortName = mf.PortName,
                        ArduinoModel = ArduinoModel.NanoR3
                    });

                    uploader.UploadSketch();
                    timer.Stop();
                    Completed = true;
                }
            }
            catch (Exception)
            {
                timer.Stop();
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Completed)
            {
                progressBar1.Value = progressBar1.Maximum;
                mf.Tls.ShowHelp("File uploaded.", this.Text, 5000, false, true);
            }
            else
            {
                mf.Tls.ShowHelp("File could not be uploaded.", this.Text, 5000, false, true);
            }
            mf.CommPort.OpenSCport();
            bntOK.Enabled = true;
        }
    }
}