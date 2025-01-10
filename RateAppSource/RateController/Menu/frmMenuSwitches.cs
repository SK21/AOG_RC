using AgOpenGPS;
using RateController.Language;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuSwitches : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private FormStart mf;

        public frmMenuSwitches(FormStart main, frmMenu menu)
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
                mf.ShowSwitches = ckScreenSwitches.Checked;
                mf.UseDualAuto = ckDualAuto.Checked;
                mf.SwitchBox.UseWorkSwitch = ckWorkSwitch.Checked;
                mf.MasterOverride = ckNoMaster.Checked;
                mf.SwitchBox.AutoRateDisabled = rbSections.Checked;
                // data grid
                for (int i = 0; i < DGV.Rows.Count; i++)
                {
                    clsSwitch SW = mf.SwitchObjects.Item(i);
                    for (int j = 1; j < 4; j++)
                    {
                        string val = DGV.Rows[i].Cells[j].EditedFormattedValue.ToString();
                        if (val == "") val = "0";
                        switch (j)
                        {
                            case 1:
                                // description
                                SW.Description = val;
                                break;

                            case 2:
                                // module
                                if (byte.TryParse(val, out byte md))
                                {
                                    SW.ModuleID = md;
                                }
                                break;

                            case 3:
                                // relay
                                if (byte.TryParse(val, out byte rly))
                                {
                                    SW.RelayID = rly;
                                }
                                break;
                        }
                    }
                    CheckRelayDefs(SW.RelayID, SW.ModuleID);
                }
                mf.SwitchObjects.Save();
                SetButtons(false);
                UpdateForm();
                MainMenu.HighlightUpdateButton();
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmMenuValves/btnOk_Click: " + ex.Message);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            mf.SwitchObjects.Reset();
            SetButtons(true);
            UpdateForm();
        }

        private void CheckRelayDefs(byte RelayID, byte ModuleID)
        {
            // check if relay is defined as 'Switch' type
            if (RelayID > 0)
            {
                clsRelay Rly = mf.RelayObjects.Item(RelayID - 1, ModuleID);
                if (Rly.Type != RelayTypes.Switch)
                {
                    var Hlp = new frmMsgBox(mf, "Change relay type from '" + Rly.TypeDescription + "' to 'Switch'?", "Switches", true);
                    Hlp.TopMost = true;
                    Hlp.ShowDialog();
                    if (Hlp.Result)
                    {
                        // change relay type
                        Rly.Type = RelayTypes.Switch;
                        Rly.Save();
                    }
                    Hlp.Close();
                }
            }
        }

        private void ckDualAuto_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void DGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                double Temp;
                string val = DGV.Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue.ToString();
                switch (e.ColumnIndex)
                {
                    case 2:
                        // module
                        double.TryParse(val, out Temp);
                        using (var form = new FormNumeric(0, 7, Temp))
                        {
                            var result = form.ShowDialog();
                            if (result == DialogResult.OK)
                            {
                                DGV.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = form.ReturnValue;
                            }
                        }
                        break;

                    case 3:
                        // relay
                        double.TryParse(val, out Temp);
                        using (var form = new FormNumeric(0, 16, Temp))
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
                mf.Tls.WriteErrorLog("frmMenuSwitches/CellClick " + ex.Message);
            }
        }

        private void DGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void DGV_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!Initializing) SetButtons(true);
        }

        private void DGV_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            mf.Tls.WriteErrorLog("frmMenuSwitches/DGV_DataError: Row,Column: " + e.RowIndex.ToString()
                + ", " + e.ColumnIndex.ToString() + " Exception: " + e.Exception.ToString());
        }

        private void frmMenuSwitches_FormClosed(object sender, FormClosedEventArgs e)
        {
            mf.Tls.SaveFormData(this);
        }

        private void frmMenuSwitches_Load(object sender, EventArgs e)
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
            DGV.BackgroundColor = DGV.DefaultCellStyle.BackColor;
            DGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            UpdateForm();
        }

        private void groupBox2_Paint(object sender, PaintEventArgs e)
        {
            mf.Tls.DrawGroupBox((GroupBox)sender, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }

        private void LoadData(bool UpdateObject = false)
        {
            try
            {
                if (UpdateObject) mf.SwitchObjects.Load();
                dataSet1.Clear();
                foreach (clsSwitch SW in mf.SwitchObjects.Items)
                {
                    DataRow Rw = dataSet1.Tables[0].NewRow();
                    Rw[0] = SW.ID + 1;
                    Rw[1] = SW.Description;
                    Rw[2] = SW.ModuleID;

                    if (SW.RelayID == 0)
                    {
                        Rw[3] = "";
                    }
                    else
                    {
                        Rw[3] = SW.RelayID;
                    }

                    dataSet1.Tables[0].Rows.Add(Rw);
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmOptions/LoadData: " + ex.Message);
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

        private void SetButtons(bool Edited)
        {
            if (!Initializing)
            {
                if (Edited)
                {
                    btnCancel.Enabled = true;
                    btnOK.Enabled = true;
                }
                else
                {
                    btnCancel.Enabled = false;
                    btnOK.Enabled = false;
                }

                cEdited = Edited;
                this.Tag = cEdited;
            }
        }

        private void SetLanguage()
        {
            ckScreenSwitches.Text = Lang.lgSwitches;
            ckWorkSwitch.Text = Lang.lgWorkSwitch;
        }

        private void UpdateForm(bool UpdateObject = false)
        {
            Initializing = true;
            ckScreenSwitches.Checked = mf.ShowSwitches;
            ckDualAuto.Checked = mf.UseDualAuto;
            ckWorkSwitch.Checked = mf.SwitchBox.UseWorkSwitch;
            ckNoMaster.Checked = mf.MasterOverride;
            LoadData(UpdateObject);

            rbSections.Checked = mf.SwitchBox.AutoRateDisabled;

            Initializing = false;
        }
    }
}