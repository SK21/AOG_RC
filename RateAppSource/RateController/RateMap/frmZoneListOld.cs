using RateController.RateMap;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RateController.Forms
{
    /// <summary>
    /// Lists all target zones with name, area, and assigned rates.
    /// Supports multi-row selection and batch deletion.
    /// </summary>
    public partial class frmZoneListOld : Form
    {
        public frmZoneListOld()
        {
            InitializeComponent();
            BuildColumns();
            LoadZones();
        }

        // ── Column setup ─────────────────────────────────────────────────────────
        // Columns are built in code because product names come from ZoneFields at runtime.

        private void BuildColumns()
        {
            _dgv.RowTemplate.Height = 28;
            _dgv.RowTemplate.DefaultCellStyle.SelectionBackColor = Color.SteelBlue;
            _dgv.RowTemplate.DefaultCellStyle.SelectionForeColor = Color.White;

            // Color swatch
            var colColor = new DataGridViewImageColumn
            {
                HeaderText = "",
                Name = "Color",
                Width = 30,
                Resizable = DataGridViewTriState.False,
                ImageLayout = DataGridViewImageCellLayout.Zoom,
            };
            _dgv.Columns.Add(colColor);

            // Name
            var colName = new DataGridViewTextBoxColumn
            {
                HeaderText = "Name",
                Name = "Name",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 150,
                DefaultCellStyle = { Padding = new Padding(4, 0, 0, 0) },
            };
            _dgv.Columns.Add(colName);

            // Area
            var colHa = new DataGridViewTextBoxColumn
            {
                HeaderText = "ha",
                Name = "Ha",
                Width = 60,
                Resizable = DataGridViewTriState.False,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight,
                                     Padding   = new Padding(0, 0, 6, 0) },
                HeaderCell = { Style = { Alignment = DataGridViewContentAlignment.MiddleRight } },
            };
            _dgv.Columns.Add(colHa);

            // Rate columns — one per product; shown only if any zone uses it
            var products = ZoneFields.Products;
            string[] headers = { "A", "B", "C", "D", "E" };

            for (int i = 0; i < products.Length; i++)
            {
                var col = new DataGridViewTextBoxColumn
                {
                    HeaderText = headers[i],
                    Name = products[i],
                    Width = 60,
                    Resizable = DataGridViewTriState.False,
                    DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight,
                                         Padding   = new Padding(0, 0, 6, 0) },
                    HeaderCell = { Style = { Alignment = DataGridViewContentAlignment.MiddleRight } },
                    Visible = false,
                };
                _dgv.Columns.Add(col);
            }
        }

        // ── Data loading ─────────────────────────────────────────────────────────

        private void LoadZones()
        {
            _dgv.Rows.Clear();

            var zones = MapController.ZnOverlays?.TargetZoneslist;
            if (zones == null || zones.Count == 0)
            {
                //_lblCount.Text = "No zones";
                UpdateRateColumnVisibility();
                UpdateDeleteButton();
                return;
            }

            // Determine which rate products are used across all zones
            bool[] productUsed = new bool[ZoneFields.Products.Length];
            foreach (var z in zones)
                for (int i = 0; i < ZoneFields.Products.Length; i++)
                    if (z.Rates.TryGetValue(ZoneFields.Products[i], out double r) && r != 0)
                        productUsed[i] = true;

            foreach (var zone in zones)
            {
                var row = new DataGridViewRow();
                row.CreateCells(_dgv);

                row.Cells[0].Value = MakeColorSwatch(zone.ZoneColor);
                row.Cells[1].Value = zone.Name;

                double ha = zone.Hectares();
                row.Cells[2].Value = ha >= 0.05 ? ha.ToString("F1") : "<0.1";

                for (int i = 0; i < ZoneFields.Products.Length; i++)
                {
                    double r = zone.Rates.TryGetValue(ZoneFields.Products[i], out double v) ? v : 0;
                    row.Cells[3 + i].Value = r > 0 ? r.ToString("F0") : "";
                }

                row.Tag = zone.Name;
                _dgv.Rows.Add(row);
            }

            for (int i = 0; i < ZoneFields.Products.Length; i++)
                _dgv.Columns[ZoneFields.Products[i]].Visible = productUsed[i];

            //_lblCount.Text = string.Format("{0} zone{1}", zones.Count, zones.Count == 1 ? "" : "s");
            UpdateDeleteButton();
        }

        private void UpdateRateColumnVisibility()
        {
            foreach (string key in ZoneFields.Products)
                _dgv.Columns[key].Visible = false;
        }

        private void UpdateDeleteButton()
        {
            //_btnDelete.Enabled = _dgv.SelectedRows.Count > 0;
        }

        // ── Event handlers ────────────────────────────────────────────────────────

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            UpdateDeleteButton();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            var names = _dgv.SelectedRows
                .Cast<DataGridViewRow>()
                .Select(r => r.Tag as string)
                .Where(n => !string.IsNullOrEmpty(n))
                .Distinct()
                .ToList();

            if (names.Count == 0) return;

            string msg = names.Count == 1
                ? string.Format("Delete '{0}'?", names[0])
                : string.Format("Delete {0} zones?", names.Count);

            using (var confirm = new frmMsgBox(msg, "Delete Zone", true))
            {
                confirm.ShowDialog();
                if (!confirm.Result) return;
            }

            foreach (string name in names)
                MapController.ZnOverlays.DeleteZone(name);

            LoadZones();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static Bitmap MakeColorSwatch(Color c)
        {
            var bmp = new Bitmap(16, 16);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.FromArgb(220, c.R, c.G, c.B));
                g.DrawRectangle(Pens.DarkGray, 0, 0, 15, 15);
            }
            return bmp;
        }
    }
}
