using libTeensySharp;
using lunOptics.libTeensySharp;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmFirmware : Form
    {
        // modified from https://github.com/luni64/TeensySharp/tree/master/src/Examples/07_WinForms%20Uploader
        // requires the NuGet package lunOptics.libTeensySharp
        // right click on project name in the solution explorer and select "Manage NuGet Packages..."

        private TeensyWatcher watcher;
        private FormStart mf;

        public frmFirmware(FormStart CallingForm)
        {
            InitializeComponent();
            mf = CallingForm;

            watcher = new TeensyWatcher(SynchronizationContext.Current);
            watcher.ConnectedTeensies.CollectionChanged += ConnectedTeensiesChanged;
            foreach (var teensy in watcher.ConnectedTeensies)
            {
                lbTeensies.Items.Add(teensy);
            }
            if (lbTeensies.Items.Count > 0) lbTeensies.SelectedIndex = 0;
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

        private async void btnUpload_Click(object sender, EventArgs e)
        {
            var teensy = lbTeensies.SelectedItem as ITeensy;
            if (teensy != null)
            {
                string filename = tbHexfile.Text;
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

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "hex files (*.hex)|*.hex|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 0;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    tbHexfile.Text = openFileDialog.FileName;

                    var fw = new TeensyFirmware(tbHexfile.Text);
                    lblFWType.Text = "Firmware type: " + fw.boardType.ToString();
                }
            }
        }

        private void frmFirmware_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
        }

        private void frmFirmware_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                mf.Tls.SaveFormData(this);
            }
        }

        private void bntOK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}