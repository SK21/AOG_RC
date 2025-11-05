using RateController.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RateController.Forms
{
    public class frmNewJobDialog : Form
    {
        private readonly TextBox tbName;
        private readonly ComboBox cbField;
        private readonly Button btnAddField;
        private readonly Button btnOk;
        private readonly Button btnCancel;

        public string JobName => tbName.Text;
        public int FieldID { get; private set; } = -1;

        public frmNewJobDialog()
        {
            this.Text = "Create New Job";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ClientSize = new Size(460, 170);

            var lbName = new Label { Left = 12, Top = 16, Width = 120, Text = "Job Name:" };
            tbName = new TextBox { Left = 140, Top = 12, Width = 300 };

            var lbField = new Label { Left = 12, Top = 56, Width = 120, Text = "Field:" };
            cbField = new ComboBox { Left = 140, Top = 52, Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
            btnAddField = new Button { Left = 368, Top = 50, Width = 72, Text = "Add..." };
            btnAddField.Click += BtnAddField_Click;

            btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Left = 264, Top = 112, Width = 84 };
            btnOk.Click += (s, e) => { ApplySelection(); };
            btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Left = 356, Top = 112, Width = 84 };

            this.Controls.Add(lbName);
            this.Controls.Add(tbName);
            this.Controls.Add(lbField);
            this.Controls.Add(cbField);
            this.Controls.Add(btnAddField);
            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;

            LoadFields();
        }

        private void LoadFields()
        {
            try
            {
                var parcels = ParcelManager.GetParcels();
                cbField.Items.Clear();
                foreach (var p in parcels)
                {
                    cbField.Items.Add(new ParcelItem { ID = p.ID, Name = p.Name });
                }
                if (cbField.Items.Count > 0) cbField.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmNewJobDialog/LoadFields: " + ex.Message);
            }
        }

        private void ApplySelection()
        {
            var sel = cbField.SelectedItem as ParcelItem;
            FieldID = sel != null ? sel.ID : -1;
        }

        private void BtnAddField_Click(object sender, EventArgs e)
        {
            using (var add = new frmTextInput("Enter new field name:", "New Field"))
            {
                if (add.ShowDialog(this) == DialogResult.OK)
                {
                    string name = (add.EnteredText ?? string.Empty).Trim();
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        var existing = ParcelManager.GetParcels().FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
                        if (existing == null)
                        {
                            var p = new Parcel { Name = name };
                            ParcelManager.AddParcel(p);
                            // reload list and select the newly added
                            LoadFields();
                            for (int i = 0; i < cbField.Items.Count; i++)
                            {
                                if ((cbField.Items[i] as ParcelItem)?.Name.Equals(name, StringComparison.OrdinalIgnoreCase) == true)
                                {
                                    cbField.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            // select existing
                            for (int i = 0; i < cbField.Items.Count; i++)
                            {
                                if ((cbField.Items[i] as ParcelItem)?.ID == existing.ID)
                                {
                                    cbField.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private class ParcelItem
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public override string ToString() => Name;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // frmNewJobDialog
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "frmNewJobDialog";
            this.Load += new System.EventHandler(this.frmNewJobDialog_Load);
            this.ResumeLayout(false);

        }

        private void frmNewJobDialog_Load(object sender, EventArgs e)
        {

        }
    }
}
