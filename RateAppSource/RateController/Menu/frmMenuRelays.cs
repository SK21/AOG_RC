using AgOpenGPS;
using RateController.Classes;
using RateController.Language;
using System;
using System.Data;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuRelays : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private bool Reset = false;

        public frmMenuRelays( frmMenu menu)
        {
            Initializing = true;
            InitializeComponent();
            MainMenu = menu;
            this.Tag = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (Reset)
                {
                    Reset = false;
                    Core.RelayObjects.Reset();
                    UpdateForm();
                }
                SaveData();
                SetButtons(false);
                UpdateForm();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuRelays/btnOk_Click: " + ex.Message);
            }
        }

        private void btnRenumber_Click(object sender, EventArgs e)
        {
            try
            {
                string val = DGV.Rows[DGV.CurrentRow.Index].Cells[2].EditedFormattedValue.ToString();
                int.TryParse(val, out int tmp);

                RelayTypes Tp = Core.RelayObjects.Item(DGV.CurrentRow.Index, cbModules.SelectedIndex).Type;

                if (Tp == RelayTypes.Section || Tp == RelayTypes.Invert_Section
                    || ((Tp == RelayTypes.TramRight || Tp == RelayTypes.TramLeft) && tmp > 0))
                {
                    Core.RelayObjects.Renumber(DGV.CurrentRow.Index, cbModules.SelectedIndex, tmp - 1);
                    SetButtons(true);
                    UpdateForm();
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmRelays/btnRenumber_Click: " + ex.Message);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset = true;
            SetButtons(true);
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
                        // either a section # (128) or a switch # (16)
                        int max = 128;
                        if (Core.RelayObjects.RelayTypeID(DGV.Rows[e.RowIndex].Cells[1].EditedFormattedValue.ToString()) == RelayTypes.Switch) max = 16;
                        double.TryParse(val, out Temp);
                        using (var form = new FormNumeric(0, max, Temp))
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
                Props.WriteErrorLog("frmRelays/CellClick: " + ex.Message);
            }
        }

        private void DGV_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!Initializing) SetButtons(true);
        }

        private void DGV_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            Props.WriteErrorLog("frmRelays/DGV_DataError: Row,Column: " + e.RowIndex.ToString() + ", " + e.ColumnIndex.ToString()
                + " Exception: " + e.Exception.ToString());
        }

        private void frmMenuRelays_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
        }

        private void frmMenuRelays_Load(object sender, EventArgs e)
        {
            SubMenuLayout.SetFormLayout(this, MainMenu, null);
            btnOK.Left = this.Width - 84;
            btnOK.Top = this.Height - 84;
            btnCancel.Left = btnOK.Left - 78;
            btnCancel.Top = btnOK.Top;
            btnRenumber.Left = btnCancel.Left - 78;
            btnRenumber.Top = btnOK.Top;
            btnReset.Left = btnRenumber.Left - 78;
            btnReset.Top = btnOK.Top;
            MainMenu.StyleControls(this);
            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            this.BackColor = Properties.Settings.Default.MainBackColour;
            PositionForm();

            cbModules.SelectedIndex = 0;

            DGV.BackgroundColor = DGV.DefaultCellStyle.BackColor;
            DGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            DataGridViewComboBoxColumn col = (DataGridViewComboBoxColumn)DGV.Columns[1];
            col.Name = "Type";
            col.DataSource = Props.TypeDescriptions;

            UpdateForm();
        }

        private void LoadData(bool UpdateObject = false)
        {
            try
            {
                if (UpdateObject) Core.RelayObjects.Load();

                dataSet1.Clear();
                foreach (clsRelay Rly in Core.RelayObjects.Items)
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

                            case RelayTypes.Switch:
                                if (Rly.SwitchID < 1)
                                {
                                    Rw[2] = "";
                                }
                                else
                                {
                                    Rw[2] = Rly.SwitchID + 1;
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
                Props.WriteErrorLog("frmRelays/LoadData: " + ex.Message);
            }
        }

        private void MainMenu_MenuMoved(object sender, EventArgs e)
        {
            PositionForm();
        }

        private void PositionForm()
        {
            this.Top = MainMenu.Top + SubMenuLayout.TopOffset;
            this.Left = MainMenu.Left + SubMenuLayout.LeftOffset;
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
                                Core.RelayObjects.Item(i, cbModules.SelectedIndex).TypeDescription = val;
                                break;

                            case 2:
                                if (Core.RelayObjects.Item(i, cbModules.SelectedIndex).Type == RelayTypes.Switch)
                                {
                                    // switch number
                                    Core.RelayObjects.Item(i, cbModules.SelectedIndex).SwitchID = Convert.ToInt32(val) - 1;
                                }
                                else
                                {
                                    // section number
                                    Core.RelayObjects.Item(i, cbModules.SelectedIndex).SectionID = Convert.ToInt32(val) - 1;
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmRelays/SaveData: " + ex.Message);
            }
            Core.RelayObjects.Save();
        }

        private void SetButtons(bool Edited)
        {
            if (!Initializing)
            {
                if (Edited)
                {
                    btnCancel.Enabled = true;
                    btnOK.Enabled = true;
                    cbModules.Enabled = false;
                    btnRenumber.Enabled = false;
                    btnReset.Enabled = false;
                }
                else
                {
                    btnCancel.Enabled = false;
                    btnOK.Enabled = false;
                    cbModules.Enabled = true;
                    btnRenumber.Enabled = true;
                    btnReset.Enabled = true;
                }

                cEdited = Edited;
                this.Tag = cEdited;
            }
        }

        private void SetLanguage()
        {
            DGV.Columns[0].HeaderText = Lang.lgRelay;
            DGV.Columns[1].HeaderText = Lang.lgType;
            DGV.Columns[2].HeaderText = Lang.lgRelayControlNumber;
        }

        private void SetModuleIndicator()
        {
            if (Core.ModulesStatus.Connected(cbModules.SelectedIndex))
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
            SetModuleIndicator();
            Initializing = false;
        }
    }
}