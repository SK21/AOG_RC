using RateController.Classes;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.RateMap
{
    public partial class frmZoneList : Form
    {
        private bool cEdited = false;
        private bool Initializing = false;
        private bool Reset = false;

        public frmZoneList()
        {
            InitializeComponent();
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
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

                        dataSet1.Tables[0].Rows.Add(Rw);
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmZoneList/LoadData: " + ex.Message);
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
            Initializing = false;
        }

        private void DGV_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            Props.WriteErrorLog("frmRelays/DGV_DataError: Row,Column: " + e.RowIndex.ToString() + ", " + e.ColumnIndex.ToString()
    + " Exception: " + e.Exception.ToString());

        }

        private void DGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 7 && e.Value is int argb)
            {
                e.CellStyle.BackColor = Color.FromArgb(argb);
                e.FormattingApplied = true;
            }
        }
    }
}