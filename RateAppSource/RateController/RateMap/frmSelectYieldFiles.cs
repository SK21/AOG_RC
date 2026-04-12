using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RateController.Forms
{
    public partial class frmSelectYieldFiles : Form
    {
        private readonly List<string> _fullPaths;

        public List<string> SelectedPaths { get; private set; } = new List<string>();

        public frmSelectYieldFiles(List<string> fullPaths, string currentPath)
        {
            _fullPaths = fullPaths ?? new List<string>();
            InitializeComponent();

            foreach (string path in _fullPaths)
            {
                string name = Path.GetFileName(path);
                bool check = string.Equals(path, currentPath, StringComparison.OrdinalIgnoreCase);
                clbFiles.Items.Add(name, check);
            }

            lblCount.Text = string.Format("{0} file(s) available", _fullPaths.Count);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SelectedPaths = new List<string>();
            for (int i = 0; i < clbFiles.Items.Count; i++)
            {
                if (clbFiles.GetItemChecked(i))
                    SelectedPaths.Add(_fullPaths[i]);
            }

            if (SelectedPaths.Count == 0)
            {
                MessageBox.Show("Please select at least one yield file.", "Create Zones",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbFiles.Items.Count; i++)
                clbFiles.SetItemChecked(i, true);
        }

        private void btnSelectNone_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbFiles.Items.Count; i++)
                clbFiles.SetItemChecked(i, false);
        }
    }
}
