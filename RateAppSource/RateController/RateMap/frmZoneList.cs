using AgOpenGPS;
using RateController.Classes;
using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace RateController.RateMap
{
    public partial class frmZoneList : Form
    {
        private bool AllSelected = false;
        private bool cEdited = false;
        private bool Initializing = false;

        public frmZoneList()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                bool Result = false;
                using (var Hlp = new frmMsgBox("Confirm Delete?", "Delete Zones", true))
                {
                    Hlp.TopMost = true;
                    Hlp.ShowDialog();
                    Result = Hlp.Result;
                }
                if (Result)
                {
                    bool DeleteCompleted = true;
                    for (int i = 0; i < DGV.Rows.Count; i++)
                    {
                        string Name = DGV.Rows[i].Cells[9].Value.ToString();
                        MapZone zone = MapController.ZnOverlays?.TargetZoneslist.FirstOrDefault(z => z.Name.Equals(Name, StringComparison.OrdinalIgnoreCase));
                        if (zone != null)
                        {
                            bool isChecked = Convert.ToBoolean(DGV.Rows[i].Cells[0].Value);
                            if (isChecked)
                            {
                                DeleteCompleted &= MapController.ZnOverlays.DeleteZone(zone.Name);
                            }
                        }
                    }
                    if (!DeleteCompleted)
                    {
                        Props.ShowMessage("Some zones could not be deleted.");
                    }
                    UpdateForm();
                    SetButtons(false);
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmZoneList/btnDelete_Click: " + ex.Message);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cEdited)
            {
                SaveData();
                UpdateForm();
                SetButtons(false);
            }
            else
            {
                Close();
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            try
            {
                AllSelected = !AllSelected;
                foreach (DataGridViewRow row in DGV.Rows)
                {
                    row.Cells[0].Value = AllSelected;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmZoneList/btnSelect_Click: " + ex.Message);
            }
        }

        private void DGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex == -1 && e.ColumnIndex == 0)
                {
                    AllSelected = !AllSelected;
                    foreach (DataGridViewRow row in DGV.Rows)
                    {
                        row.Cells[0].Value = AllSelected;
                    }
                }
                else
                {
                    string val = DGV.Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue.ToString();
                    switch (e.ColumnIndex)
                    {
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                            double tmp = double.TryParse(val, out double v) ? v : 0;
                            using (var form = new FormNumeric(0, 9999, tmp))
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
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmZoneList/DGV_CellClick: " + ex.Message);
            }
        }

        private void DGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            switch (e.ColumnIndex)
            {
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                    double val = double.TryParse(e.Value.ToString(), out double v) ? v : 0;
                    var culture = CultureInfo.CurrentCulture;
                    if (val >= 1000)
                    {
                        e.Value = val.ToString("N0", culture);
                    }
                    else
                    {
                        e.Value = val.ToString("N1", culture);
                    }
                    e.FormattingApplied = true;
                    break;

                case 8:
                    if (e.Value is int argb)
                    {
                        e.CellStyle.BackColor = Color.FromArgb(argb);
                        e.FormattingApplied = true;
                    }
                    break;
            }
        }

        private void DGV_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!Initializing && e.ColumnIndex != 0) SetButtons(true);
        }

        private void DGV_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            Props.WriteErrorLog("frmRelays/DGV_DataError: Row,Column: " + e.RowIndex.ToString() + ", " + e.ColumnIndex.ToString()
    + " Exception: " + e.Exception.ToString());
        }

        private void frmZoneList_FormClosing(object sender, FormClosingEventArgs e)
        {
            Props.SaveFormLocation(this);
        }

        private void frmZoneList_Load(object sender, EventArgs e)
        {
            Props.LoadFormLocation(this);
            this.BackColor = Properties.Settings.Default.MainBackColour;
            DGV.BackgroundColor = DGV.DefaultCellStyle.BackColor;
            DGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            UpdateForm();
            SetButtons(false);
        }

        private void LoadData(bool UpdateObject = false)
        {
            try
            {
                dataSet1.Clear();
                var zones = MapController.ZnOverlays?.TargetZoneslist;
                if (zones != null && zones.Count > 0)
                {
                    foreach (var zone in zones)
                    {
                        DataRow Rw = dataSet1.Tables[0].NewRow();
                        Rw[0] = zone.Name;

                        if (Props.UseMetric)
                        {
                            Rw[1] = zone.Hectares();
                        }
                        else
                        {
                            Rw[1] = Core.Tls.Acres(zone.Hectares());
                        }

                        Rw[2] = zone.Rates.TryGetValue(ZoneFields.ProductA, out double v2) ? v2 : 0;
                        Rw[3] = zone.Rates.TryGetValue(ZoneFields.ProductB, out double v3) ? v3 : 0;
                        Rw[4] = zone.Rates.TryGetValue(ZoneFields.ProductC, out double v4) ? v4 : 0;
                        Rw[5] = zone.Rates.TryGetValue(ZoneFields.ProductD, out double v5) ? v5 : 0;
                        Rw[6] = zone.Rates.TryGetValue(ZoneFields.ProductE, out double v6) ? v6 : 0;

                        Rw[7] = zone.ZoneColor.ToArgb();
                        Rw[8] = false;
                        Rw[9] = zone.Name;

                        dataSet1.Tables[0].Rows.Add(Rw);
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmZoneList/LoadData: " + ex.Message);
            }
        }

        private void SaveData()
        {
            try
            {
                for (int i = 0; i < DGV.Rows.Count; i++)
                {
                    string Name = DGV.Rows[i].Cells[9].Value.ToString();
                    MapZone zone = MapController.ZnOverlays?.TargetZoneslist.FirstOrDefault(z => z.Name.Equals(Name, StringComparison.OrdinalIgnoreCase));
                    if (zone != null)
                    {
                        for (int j = 0; j < DGV.Columns.Count; j++)
                        {
                            string val = DGV.Rows[i].Cells[j].EditedFormattedValue.ToString();
                            switch (j)
                            {
                                case 1:
                                    if (val != zone.Name && MapController.ZnOverlays.ZoneNameFound(val, zone))
                                    {
                                        Props.ShowMessage("Duplicate zone name: " + val);
                                    }
                                    else
                                    {
                                        zone.Name = val;
                                    }
                                    break;

                                case 3:
                                case 4:
                                case 5:
                                case 6:
                                case 7:
                                    double Amt = double.TryParse(val, out double v) ? v : 0;
                                    zone.Rates[ZoneFields.Products[j - 3]] = Amt;
                                    break;
                            }
                        }
                    }
                }
                MapController.SaveMap();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmZoneList/SaveData: " + ex.Message);
            }
        }

        private void SetButtons(bool Edited)
        {
            if (!Initializing)
            {
                if (Edited)
                {
                    btnCancel.Enabled = true;
                    btnOK.Image = Properties.Resources.Save;
                }
                else
                {
                    btnCancel.Enabled = false;
                    btnOK.Image = Properties.Resources.OK;
                }

                cEdited = Edited;
                this.Tag = cEdited;
            }
        }

        private void UpdateForm(bool UpdateObject = false)
        {
            Initializing = true;
            LoadData(UpdateObject);
            label1.Text = "Zone count: " + DGV.Rows.Count.ToString();
            Initializing = false;
        }
    }
}