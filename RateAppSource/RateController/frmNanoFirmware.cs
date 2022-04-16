using ArduinoUploader;
using ArduinoUploader.Hardware;
using System;
using System.ComponentModel;
using System.IO;
using System.Timers;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmNanoFirmware : Form
    {
        public System.Timers.Timer timer = new System.Timers.Timer(1000);

        private bool Completed;
        private string ComPort;
        private FormStart mf;
        private int ProgressCount;

        // requires the NuGet package ArduinoUpdater
        // right click on project name in the solution explorer and select "Manage NuGet Packages..."
        private bool UseDefault;

        private BackgroundWorker worker = new BackgroundWorker();

        public frmNanoFirmware(FormStart CallingForm)
        {
            InitializeComponent();
            mf = CallingForm;

            cboPort1.Items.Clear();
            foreach (String s in System.IO.Ports.SerialPort.GetPortNames())
            {
                cboPort1.Items.Add(s);
            }
        }

        private void bntOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            UseDefault = false;
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
            UseDefault = true;
            tbHexfile.Text = "Default file version date:" + mf.Tls.NanoFirmwareVersion();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                // close ports
                for (int i = 0; i < 3; i++)
                {
                    mf.SER[i].CloseRCport();
                }

                progressBar1.Visible = true;
                bntOK.Enabled = false;
                worker.RunWorkerAsync();
                bntOK.Enabled = true;
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp(ex.Message, this.Text, 3000, true);
                bntOK.Enabled = true;
            }
        }

        private void btnUpload_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
        }

        private void cboPort1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComPort = cboPort1.SelectedItem.ToString();
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

            this.BackColor = Properties.Settings.Default.DayColour;

            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted +=
              new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
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
                string HexFile;
                Completed = false;
                if (UseDefault)
                {
                    HexFile = Path.GetTempFileName();
                    File.WriteAllBytes(HexFile, Properties.Resources.RCnano_ino);
                }
                else
                {
                    HexFile = tbHexfile.Text;
                }

                if (File.Exists(HexFile))
                {
                    timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_elapsed);
                    timer.Start();
                    var uploader = new ArduinoSketchUploader(
                    new ArduinoSketchUploaderOptions()
                    {
                        FileName = HexFile,
                        PortName = ComPort,
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

            // open port
            for (int i = 0; i < 3; i++)
            {
                mf.SER[i].OpenRCport(ComPort);
            }
        }
    }
}