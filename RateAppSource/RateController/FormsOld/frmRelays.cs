using AgOpenGPS;
using RateController.Language;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmRelays : Form
    {
        private bool FormEdited;
        private bool Initializing;
        private FormStart mf;

        public frmRelays(FormStart CalledFrom)
        {
            Initializing = true;
            InitializeComponent();

            #region // language

            DGV.Columns[0].HeaderText = Lang.lgRelay;
            DGV.Columns[1].HeaderText = Lang.lgType;
            DGV.Columns[2].HeaderText = Lang.lgSectionNum;

            this.Text = Lang.lgRelays;

            #endregion // language

            mf = CalledFrom;
        }

        private void bnReset_Click(object sender, EventArgs e)
        {
            mf.RelayObjects.Reset();
            SetButtons(true);
            UpdateForm();
        }

        private void bntOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (!FormEdited)
                {
                    this.Close();
                }
                else
                {
                    if (mf.Tls.ReadOnly)
                    {
                        mf.Tls.ShowHelp("File is read only.", "Help", 5000, false, false, true);
                    }
                    else
                    {
                        SaveData();
                        UpdateForm();
                        SetButtons(false);
                    }
                }
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp(ex.Message, this.Text, 3000, true);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm(true);
            SetButtons(false);
        }

        private void btnRenumber_Click(object sender, EventArgs e)
        {
            try
            {
                string val = DGV.Rows[DGV.CurrentRow.Index].Cells[2].EditedFormattedValue.ToString();
                int.TryParse(val, out int tmp);

                RelayTypes Tp = mf.RelayObjects.Item(DGV.CurrentRow.Index, cbModules.SelectedIndex).Type;

                if (Tp == RelayTypes.Section || Tp == RelayTypes.Invert_Section
                    || ((Tp == RelayTypes.TramRight || Tp == RelayTypes.TramLeft) && tmp > 0))
                {
                    mf.RelayObjects.Renumber(DGV.CurrentRow.Index, cbModules.SelectedIndex, tmp - 1);
                    SetButtons(true);
                    UpdateForm();
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmRelays/btnRenumber_Click: " + ex.Message);
            }
        }

        private void btnRenumber_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Renumber sections.";

            mf.Tls.ShowHelp(Message, "Renumber");
            hlpevent.Handled = true;
        }

        private void btnReset_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Reset relays.";

            mf.Tls.ShowHelp(Message, "Reset");
            hlpevent.Handled = true;
        }

        private void cbModules_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void DGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                double Temp;
                string val = DGV.Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue.ToString();
                switch (e.ColumnIndex)
                {
                    case 1:
                        // type, make combobox drop down
                        break;

                    case 2:
                        // section
                        double.TryParse(val, out Temp);
                        using (var form = new FormNumeric(0, 128, Temp))
                        {
                            var result = form.ShowDialog();
                            if (result == DialogResult.OK)
                            {
                                DGV.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = form.ReturnValue;
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmRelays/CellClick: " + ex.Message);
            }
        }

        private void DGV_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!Initializing) SetButtons(true);
        }

        private void DGV_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            mf.Tls.WriteErrorLog("frmRelays/DGV_DataError: Row,Column: " + e.RowIndex.ToString() + ", " + e.ColumnIndex.ToString()
                + " Exception: " + e.Exception.ToString());
        }

        private void DGV_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Relay Type:\n" +
                "  Section - relay controlled by section switch\n" +
                "  Slave - relay is on when any section relay is\n" +
                "          on and off when all sections relays\n" +
                "          are off\n" +
                "  Master - relay is on when any section relay is\n" +
                "            on and turns off before section relays \n" +
                "             turn off\n" +
                "  Power - on all the time\n" +
                "  Invert_Section - relay is on when section is off\n" +
                "  Hyd Up\n" +
                "  Hyd Down \n" +
                "  Tram Right\n" +
                "  Tram Left\n" +
                "  Geo Stop\n" +
                "\n" +
                "Section #:\n" +
                "    - the section that controls the relay";

            mf.Tls.ShowHelp(Message, "Relays", 60000);
            hlpevent.Handled = true;
        }

        private void frmRelays_FormClosed(object sender, FormClosedEventArgs e)
        {
            mf.Tls.SaveFormData(this);
        }

        private void frmRelays_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            cbModules.SelectedIndex = 0;

            DGV.BackgroundColor = DGV.DefaultCellStyle.BackColor;
            DGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            DataGridViewComboBoxColumn col = (DataGridViewComboBoxColumn)DGV.Columns[1];
            col.Name = "Type";
            col.DataSource = mf.TypeDescriptions;

            UpdateForm();
        }

        private void LoadData(bool UpdateObject = false)
        {
            try
            {
                if (UpdateObject) mf.RelayObjects.Load();

                dataSet1.Clear();
                foreach (clsRelay Rly in mf.RelayObjects.Items)
                {
                    if (Rly.ModuleID == cbModules.SelectedIndex)
                    {
                        DataRow Rw = dataSet1.Tables[0].NewRow();
                        Rw[0] = Rly.ID + 1;
                        Rw[1] = Rly.TypeDescription;

                        switch (Rly.Type)
                        {
                            case RelayTypes.Section:
                            case RelayTypes.Invert_Section:
                            case RelayTypes.TramRight:
                            case RelayTypes.TramLeft:
                                if (Rly.SectionID < 0)
                                {
                                    Rw[2] = "";
                                }
                                else
                                {
                                    Rw[2] = Rly.SectionID + 1;
                                }
                                break;

                            default:
                                Rw[2] = "";
                                break;
                        }

                        dataSet1.Tables[0].Rows.Add(Rw);
                    }
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmRelays/LoadData: " + ex.Message);
            }
        }

        private void ModuleIndicator_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Shows if module is connected.";

            mf.Tls.ShowHelp(Message, "Connected");
            hlpevent.Handled = true;
        }

        private void SaveData()
        {
            try
            {
                for (int i = 0; i < DGV.Rows.Count; i++)
                {
                    for (int j = 1; j < 3; j++)
                    {
                        string val = DGV.Rows[i].Cells[j].EditedFormattedValue.ToString();
                        if (val == "") val = "0";
                        switch (j)
                        {
                            case 1:
                                // Type
                                mf.RelayObjects.Item(i, cbModules.SelectedIndex).TypeDescription = val;
                                break;

                            case 2:
                                // section
                                mf.RelayObjects.Item(i, cbModules.SelectedIndex).SectionID = Convert.ToInt32(val) - 1;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmRelays/SaveData: " + ex.Message);
            }
            mf.RelayObjects.Save();
        }

        private void SetButtons(bool Edited)
        {
            if (!Initializing)
            {
                if (Edited)
                {
                    btnCancel.Enabled = true;
                    btnOK.Image = Properties.Resources.Save;
                    cbModules.Enabled = false;
                    btnRenumber.Enabled = false;
                    btnReset.Enabled = false;
                }
                else
                {
                    btnCancel.Enabled = false;
                    btnOK.Image = Properties.Resources.OK;
                    cbModules.Enabled = true;
                    btnRenumber.Enabled = true;
                    btnReset.Enabled = true;
                }

                FormEdited = Edited;
            }
        }

        private void SetDayMode()
        {
                this.BackColor = Properties.Settings.Default.MainBackColour;
                ModuleIndicator.BackColor = Properties.Settings.Default.MainBackColour;

                foreach (Control c in this.Controls)
                {
                    c.ForeColor = Color.Black;
                }
        }

        private void SetModuleIndicator()
        {
            if (mf.ModulesStatus.Connected(cbModules.SelectedIndex))
            {
                ModuleIndicator.Image = Properties.Resources.On;
            }
            else
            {
                ModuleIndicator.Image = Properties.Resources.Off;
            }
        }

        private void UpdateForm(bool UpdateObject = false)
        {
            Initializing = true;
            LoadData(UpdateObject);
            SetDayMode();
            SetModuleIndicator();
            Initializing = false;
        }
    }
}