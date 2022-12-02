using libTeensySharp;
using lunOptics.libTeensySharp;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace PCBsetup.Forms
{
    public partial class frmFirmware : Form
    {
        // modified from https://github.com/luni64/TeensySharp/tree/master/src/Examples/07_WinForms%20Uploader
        // requires the NuGet package lunOptics.libTeensySharp
        // right click on project name in the solution explorer and select "Manage NuGet Packages..."
        // teensy uploader and serial monitor need to be closed to free the serial port

        private frmMain mf;
        private bool UseDefault = false;
        private TeensyWatcher watcher;
        private byte SoftwareID;    // 0 autosteer, 1 rate

        public frmFirmware(frmMain CallingForm, byte ID)
        {
            InitializeComponent();
            mf = CallingForm;
            SoftwareID = ID;

            watcher = new TeensyWatcher(SynchronizationContext.Current);
            watcher.ConnectedTeensies.CollectionChanged += ConnectedTeensiesChanged;
            foreach (var teensy in watcher.ConnectedTeensies)
            {
                lbTeensies.Items.Add(teensy);
            }
            if (lbTeensies.Items.Count > 0) lbTeensies.SelectedIndex = 0;
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

        private void btnBrowse_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Search for new firmware (hex) files.";

            mf.Tls.ShowHelp(Message, "Browse");
            hlpevent.Handled = true;
        }

        private void btnDefault_Click(object sender, EventArgs e)
        {
            UseDefault = true;
            tbHexfile.Text = "Default file version date: " + mf.Tls.TeensyAutoSteerVersion();
        }

        private void btnDefault_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Use the base firmware included in the app.";

            mf.Tls.ShowHelp(Message, "Use default");
            hlpevent.Handled = true;
        }

        private async void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                mf.CommPort.Close();
                string filename = "";
                var teensy = lbTeensies.SelectedItem as ITeensy;
                if (teensy != null)
                {
                    if (UseDefault)
                    {
                        filename = Path.GetTempFileName();
                        switch (SoftwareID)
                        {
                            case 1:
                                // rate
                                File.WriteAllBytes(filename, PCBsetup.Properties.Resources.RCteensy_ino);
                                break;

                            default:
                                // autosteer
                                File.WriteAllBytes(filename, PCBsetup.Properties.Resources.AutoSteerTeensy2_ino);
                                break;
                        }
                    }
                    else
                    {
                        filename = tbHexfile.Text;
                    }

                    if (File.Exists(filename))
                    {
                        var progress = new Progress<int>(v => progressBar.Value = v);
                        progressBar.Visible = true;
                        var result = await teensy.UploadAsync(filename, progress);
                        mf.Tls.ShowHelp(result.ToString(), "Message", 3000);
                        progressBar.Visible = false;
                        progressBar.Value = 0;
                    }
                    else
                    {
                        mf.Tls.ShowHelp("File does not exist", "Error", 3000, true);
                    }
                }
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp(ex.Message, this.Text, 3000, true);
            }
        }

        private void btnUpload_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Upload new firmware to the Teensy.";

            mf.Tls.ShowHelp(Message, "Upload");
            hlpevent.Handled = true;
        }

        private void ConnectedTeensiesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var teensy in e.NewItems)
                    {
                        lbTeensies.Items.Add(teensy);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var teensy in e.OldItems)
                    {
                        lbTeensies.Items.Remove(teensy);
                    }
                    break;
            }
            if (lbTeensies.SelectedIndex == -1 && lbTeensies.Items.Count > 0) lbTeensies.SelectedIndex = 0;
        }

        private void frmFirmware_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                mf.Tls.SaveFormData(this);
            }
        }

        private void frmFirmware_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            this.BackColor = PCBsetup.Properties.Settings.Default.DayColour;

            UseDefault = true;
            switch (SoftwareID)
            {
                case 1:
                    // rate
                    tbHexfile.Text = "Default file version date: " + mf.Tls.TeensyRateVersion();
                    this.Text = "Teensy Rate Firmware";
                    break;

                default:
                    // autosteer
                    tbHexfile.Text = "Default file version date: " + mf.Tls.TeensyAutoSteerVersion();
                    this.Text = "Teensy AutoSteer Firmware";
                    break;
            }
        }

        private void lbTeensies_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Teensies connected to a serial (usb) port. " +
                "Select the Teensy to update.";

            mf.Tls.ShowHelp(Message, "Connected Teensies");
            hlpevent.Handled = true;
        }

        private void tbHexfile_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Filename of firmware to upload to the Teensy.";

            mf.Tls.ShowHelp(Message, "Firmware");
            hlpevent.Handled = true;
        }
    }
}