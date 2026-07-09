using AgOpenGPS;
using RateController.Classes;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
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
        private int _printRow;
        private ComboBox cboOverrideProduct;
        private TextBox tbOverrideValue;
        private Button btnOverrideApply;

        public frmZoneList()
        {
            InitializeComponent();
        }

        private void btnOverrideApply_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboOverrideProduct.SelectedIndex < 0)
                {
                    System.Media.SystemSounds.Exclamation.Play();
                    return;
                }

                double value = 0;
                if (!double.TryParse(tbOverrideValue.Text, out value))
                {
                    using (var form = new FormNumeric(0, 9999, 0))
                    {
                        form.Text = "Override Value";
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            value = form.ReturnValue;
                            tbOverrideValue.Text = value.ToString("N1");
                        }
                        else
                        {
                            return;
                        }
                    }
                }

                int colIndex = 3 + cboOverrideProduct.SelectedIndex;
                bool anySelected = false;
                foreach (DataGridViewRow row in DGV.Rows)
                {
                    if (Convert.ToBoolean(row.Cells[0].Value))
                    {
                        row.Cells[colIndex].Value = (decimal)value;
                        anySelected = true;
                    }
                }

                if (!anySelected)
                {
                    Props.ShowMessage("No zones selected. Check the zones you want to override.");
                }
                else
                {
                    SetButtons(true);
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmZoneList/btnOverrideApply_Click: " + ex.Message);
            }
        }

        private void TbOverrideValue_Click(object sender, EventArgs e)
        {
            double.TryParse(tbOverrideValue.Text, out double current);
            using (var form = new FormNumeric(0, 9999, current))
            {
                form.Text = "Override Value";
                if (form.ShowDialog() == DialogResult.OK)
                    tbOverrideValue.Text = form.ReturnValue.ToString("N1");
            }
        }

        private void InitOverrideControls()
        {
            int y = DGV.Bottom + 4;

            var lblProduct = new Label();
            lblProduct.Text = "Product:";
            lblProduct.AutoSize = true;
            lblProduct.Location = new Point(DGV.Left, y + 4);
            this.Controls.Add(lblProduct);

            cboOverrideProduct = new ComboBox();
            cboOverrideProduct.DropDownStyle = ComboBoxStyle.DropDownList;
            cboOverrideProduct.Items.AddRange(new object[] { "A", "B", "C", "D", "E" });
            cboOverrideProduct.SelectedIndex = 0;
            cboOverrideProduct.Location = new Point(lblProduct.Right + 4, y);
            cboOverrideProduct.Size = new Size(45, 28);
            this.Controls.Add(cboOverrideProduct);

            var lblValue = new Label();
            lblValue.Text = "Value:";
            lblValue.AutoSize = true;
            lblValue.Location = new Point(cboOverrideProduct.Right + 8, y + 4);
            this.Controls.Add(lblValue);

            tbOverrideValue = new TextBox();
            tbOverrideValue.Location = new Point(lblValue.Right + 4, y);
            tbOverrideValue.Size = new Size(70, 28);
            tbOverrideValue.TextAlign = HorizontalAlignment.Right;
            tbOverrideValue.Click += TbOverrideValue_Click;
            this.Controls.Add(tbOverrideValue);

            btnOverrideApply = new Button();
            btnOverrideApply.Text = "Apply";
            btnOverrideApply.Location = new Point(tbOverrideValue.Right + 6, y - 2);
            btnOverrideApply.Size = new Size(80, 32);
            btnOverrideApply.FlatStyle = FlatStyle.Flat;
            btnOverrideApply.Click += btnOverrideApply_Click;
            this.Controls.Add(btnOverrideApply);
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

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                using (var pd = new PrintDocument())
                {
                    _printRow = 0;
                    pd.PrintPage += PrintZoneList;
                    using (var dlg = new PrintDialog { Document = pd, UseEXDialog = true })
                    {
                        if (dlg.ShowDialog() == DialogResult.OK)
                            pd.Print();
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmZoneList/btnPrint_Click: " + ex.Message);
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
                        Color c = Color.FromArgb(argb);
                        e.CellStyle.BackColor = Color.FromArgb(255, c.R, c.G, c.B);
                        e.Value = string.Empty;
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
            InitOverrideControls();
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
                    foreach (var zone in zones.OrderBy(z => ExtractZoneNumber(z.Name)).ThenBy(z => z.Name))
                    {
                        DataRow Rw = dataSet1.Tables[0].NewRow();
                        Rw[0] = zone.Name;

                        if (Props.UseMetric)
                        {
                            Rw[1] = (decimal)zone.Hectares();
                        }
                        else
                        {
                            Rw[1] = (decimal)Core.Tls.Acres(zone.Hectares());
                        }

                        Rw[2] = (decimal)(zone.Rates.TryGetValue(ZoneFields.ProductA, out double v2) ? v2 : 0);
                        Rw[3] = (decimal)(zone.Rates.TryGetValue(ZoneFields.ProductB, out double v3) ? v3 : 0);
                        Rw[4] = (decimal)(zone.Rates.TryGetValue(ZoneFields.ProductC, out double v4) ? v4 : 0);
                        Rw[5] = (decimal)(zone.Rates.TryGetValue(ZoneFields.ProductD, out double v5) ? v5 : 0);
                        Rw[6] = (decimal)(zone.Rates.TryGetValue(ZoneFields.ProductE, out double v6) ? v6 : 0);

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

        private void PrintZoneList(object sender, PrintPageEventArgs e)
        {
            var g = e.Graphics;
            float margin = 50f;
            float pageHeight = e.PageBounds.Height - 2 * margin;
            float y = margin;

            var titleFont = new Font("Arial", 13, FontStyle.Bold);
            var subFont = new Font("Arial", 9);
            var headerFont = new Font("Arial", 9, FontStyle.Bold);
            var cellFont = new Font("Arial", 9);

            try
            {
                if (_printRow == 0)
                {
                    Job JB = JobManager.CurrentJob;
                    string prescription = JB?.ActivePrescription ?? string.Empty;
                    string title = string.IsNullOrEmpty(prescription) ? "Zone List" : "Zone List — " + prescription;
                    g.DrawString(title, titleFont, Brushes.Black, margin, y);
                    y += titleFont.GetHeight(g) + 2;
                    g.DrawString(DateTime.Now.ToString("yyyy-MM-dd"), subFont, Brushes.Black, margin, y);
                    y += subFont.GetHeight(g) + 8;
                }

                string areaUnit = Props.UseMetric ? "Ha" : "Ac";
                string[] headers = { "Name", areaUnit, "A", "B", "C", "D", "E", "Color" };
                float[] colWidths = { 140f, 55f, 55f, 55f, 55f, 55f, 55f, 55f };
                int[] dgvCols = { 1, 2, 3, 4, 5, 6, 7, 8 };
                float rowHeight = headerFont.GetHeight(g) + 6;

                // Column headers on every page
                float x = margin;
                g.FillRectangle(Brushes.LightGray, x, y, colWidths.Sum(), rowHeight);
                for (int c = 0; c < headers.Length; c++)
                {
                    g.DrawRectangle(Pens.Black, x, y, colWidths[c], rowHeight);
                    var sf = new StringFormat
                    {
                        Alignment = c == 0 ? StringAlignment.Near : StringAlignment.Far,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(headers[c], headerFont, Brushes.Black,
                        new RectangleF(x + 2, y, colWidths[c] - 4, rowHeight), sf);
                    x += colWidths[c];
                }
                y += rowHeight;

                // Data rows
                while (_printRow < DGV.Rows.Count)
                {
                    if (y + rowHeight > margin + pageHeight)
                    {
                        e.HasMorePages = true;
                        return;
                    }

                    x = margin;
                    for (int c = 0; c < dgvCols.Length; c++)
                    {
                        g.DrawRectangle(Pens.Black, x, y, colWidths[c], rowHeight);

                        if (c == 7)
                        {
                            var raw = DGV.Rows[_printRow].Cells[dgvCols[c]].Value;
                            if (raw is int colorArgb)
                            {
                                Color zc = Color.FromArgb(colorArgb);
                                using (var brush = new SolidBrush(Color.FromArgb(255, zc.R, zc.G, zc.B)))
                                    g.FillRectangle(brush, x + 3, y + 3, colWidths[c] - 6, rowHeight - 6);
                            }
                        }
                        else
                        {
                            string val = DGV.Rows[_printRow].Cells[dgvCols[c]].FormattedValue?.ToString() ?? "";
                            var sf = new StringFormat
                            {
                                Alignment = c == 0 ? StringAlignment.Near : StringAlignment.Far,
                                LineAlignment = StringAlignment.Center
                            };
                            g.DrawString(val, cellFont, Brushes.Black,
                                new RectangleF(x + 2, y, colWidths[c] - 4, rowHeight), sf);
                        }
                        x += colWidths[c];
                    }
                    y += rowHeight;
                    _printRow++;
                }

                // Footer
                y += 4;
                g.DrawString(ZoneCountText(), subFont, Brushes.Black, margin, y);
                e.HasMorePages = false;
            }
            finally
            {
                titleFont.Dispose();
                subFont.Dispose();
                headerFont.Dispose();
                cellFont.Dispose();
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

        private static int ExtractZoneNumber(string name)
        {
            if (!name.StartsWith("Zone ")) return int.MaxValue;
            int start = 5, end = 5;
            while (end < name.Length && char.IsDigit(name[end])) end++;
            return end > start && int.TryParse(name.Substring(start, end - start), out int n) ? n : int.MaxValue;
        }

        private string ZoneCountText()
        {
            var zones = MapController.ZnOverlays?.TargetZoneslist;
            if (zones == null || zones.Count == 0) return "0 zones";
            int totalParts = zones.Count;
            int distinctZones = zones
                .Select(z => string.Join("|", ZoneFields.Products.Select(p => z.Rates.TryGetValue(p, out double v) ? ((long)Math.Round(v * 10)).ToString() : "0")))
                .Distinct().Count();
            return distinctZones == totalParts
                ? string.Format("{0} zones", totalParts)
                : string.Format("{0} zones, {1} parts", distinctZones, totalParts);
        }

        private void UpdateForm(bool UpdateObject = false)
        {
            Initializing = true;
            LoadData(UpdateObject);
            label1.Text = ZoneCountText();
            Initializing = false;
        }
    }
}
