using AgOpenGPS;
using RateController.Language;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuRelays : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private FormStart mf;
        private bool Reset = false;

        public frmMenuRelays(FormStart main, frmMenu menu)
        {
            Initializing = true;
            InitializeComponent();
            MainMenu = menu;
            mf = main;
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
                if(Reset)
                {
                    Reset = false;
                    mf.RelayObjects.Reset();
                    UpdateForm();
                }
                SaveData();
                SetButtons(false);
                UpdateForm();
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmMenuRelays/btnOk_Click: " + ex.Message);
            }
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
                        if (mf.RelayObjects.RelayTypeID(DGV.Rows[e.RowIndex].Cells[1].EditedFormattedValue.ToString()) == RelayTypes.Switch) max = 16;
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

        private void frmMenuRelays_FormClosed(object sender, FormClosedEventArgs e)
        {
            mf.Tls.SaveFormData(this);
        }

        private void frmMenuRelays_Load(object sender, EventArgs e)
        {
            // menu 800,600
            // sub menu 540,630
            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            mf.Tls.LoadFormData(this, "", false);
            this.BackColor = Properties.Settings.Default.BackColour;
            this.Width = MainMenu.Width - 260;
            this.Height = MainMenu.Height - 50;
            btnOK.Left = this.Width - 84;
            btnOK.Top = this.Height - 84;
            btnCancel.Left = btnOK.Left - 78;
            btnCancel.Top = btnOK.Top;
            MainMenu.StyleControls(this);
            PositionForm();

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
                mf.Tls.WriteErrorLog("frmRelays/LoadData: " + ex.Message);
            }
        }

        private void MainMenu_MenuMoved(object sender, EventArgs e)
        {
            PositionForm();
        }

        private void PositionForm()
        {
            this.Top = MainMenu.Top + 30;
            this.Left = MainMenu.Left + 246;
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
                                if (mf.RelayObjects.Item(i, cbModules.SelectedIndex).Type == RelayTypes.Switch)
                                {
                                    // switch number
                                    mf.RelayObjects.Item(i, cbModules.SelectedIndex).SwitchID = Convert.ToInt32(val) - 1;
                                }
                                else
                                {
                                    // section number
                                    mf.RelayObjects.Item(i, cbModules.SelectedIndex).SectionID = Convert.ToInt32(val) - 1;
                                }
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
            DGV.Columns[2].HeaderText = Lang.lgRelayControlNumer;
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
            SetModuleIndicator();
            Initializing = false;
        }
    }
}