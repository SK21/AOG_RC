using RateController.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RateController.Forms
{
    public partial class frmGenerateZones : Form
    {
        private const double AcresPerHa = 2.47105;

        // ── Yield rows (parallel lists) ──────────────────────────────────────
        private readonly List<string>        _yieldPaths  = new List<string>();
        private readonly List<CheckBox>      _yieldChecks = new List<CheckBox>();
        private readonly List<NumericUpDown> _yieldNuds   = new List<NumericUpDown>();

        // ── EC ───────────────────────────────────────────────────────────────
        private readonly string _ecPath;

        // ── Results (populated on Generate click) ────────────────────────────
        public List<string> SelectedYieldPaths   { get; private set; }
        public List<double> SelectedYieldWeights { get; private set; }  // sum = 1.0 within yield
        public double       YieldFraction        { get; private set; }  // 0–1 share of productivity index
        public string       EcPath               { get; private set; }
        public double       EcFraction           { get; private set; }  // 0–1 share of productivity index
        public int          ZoneCount            { get; private set; }
        public double       MinZoneHa            { get; private set; }  // always in ha

        public frmGenerateZones(
            List<string> yieldFullPaths,
            string currentYieldPath,
            bool elevationLoaded,
            string ecPath)
        {
            _yieldPaths = yieldFullPaths ?? new List<string>();
            _ecPath     = ecPath;

            InitializeComponent();
            BuildYieldRows(currentYieldPath);
            SetupLayerRows(elevationLoaded, ecPath);
            SetupUnits();
            UpdateWeightVisibility();
        }

        // ── Dynamic yield row construction ───────────────────────────────────

        private void BuildYieldRows(string currentYieldPath)
        {
            const int rowH = 30;
            int y = 2;

            var sorted = _yieldPaths
                .Select(p => new { Path = p, Year = ExtractYear(Path.GetFileName(p)) })
                .OrderByDescending(x => x.Year)
                .ThenBy(x => x.Path)
                .ToList();

            _yieldPaths.Clear();
            foreach (var item in sorted) _yieldPaths.Add(item.Path);

            foreach (var item in sorted)
            {
                string fname = Path.GetFileName(item.Path);
                bool isSelected = string.Equals(item.Path, currentYieldPath,
                    StringComparison.OrdinalIgnoreCase);

                string display = string.IsNullOrEmpty(item.Year)
                    ? fname
                    : fname + new string(' ', Math.Max(1, 26 - fname.Length)) + item.Year;

                var ck = new CheckBox
                {
                    Text       = display,
                    Location   = new Point(2, y),
                    Width      = 310,
                    Height     = 26,
                    Checked    = isSelected,
                    FlatStyle  = FlatStyle.Flat,
                    CheckAlign = ContentAlignment.MiddleLeft,
                };

                var nud = new NumericUpDown
                {
                    Location  = new Point(316, y - 1),
                    Width     = 62,
                    Minimum   = 1,
                    Maximum   = 100,
                    Value     = 100,
                    Increment = 5,
                    Visible   = false,
                };

                ck.CheckedChanged += (s, e) => { UpdateWeightVisibility(); UpdateWeightTotal(); };
                nud.ValueChanged  += (s, e) => UpdateWeightTotal();

                pnlYield.Controls.Add(ck);
                pnlYield.Controls.Add(nud);
                _yieldChecks.Add(ck);
                _yieldNuds.Add(nud);

                y += rowH;
            }
        }

        private void SetupLayerRows(bool elevationLoaded, string ecPath)
        {
            ckElevation.Text    = elevationLoaded
                ? "Elevation / TWI  (loaded)"
                : "Elevation / TWI  (no data)";
            ckElevation.Enabled = elevationLoaded;
            ckElevation.Checked = false;
            // Elevation weight not yet integrated into algorithm — spinner hidden
            nudElevWeight.Visible = false;

            bool ecLoaded = !string.IsNullOrEmpty(ecPath) && File.Exists(ecPath);
            ckEC.Text    = ecLoaded ? "EC  (loaded)" : "EC  (no data)";
            ckEC.Enabled = ecLoaded;
            ckEC.Checked = false;

            ckElevation.CheckedChanged += (s, e) => { UpdateWeightVisibility(); UpdateWeightTotal(); };
            ckEC.CheckedChanged        += (s, e) => { UpdateWeightVisibility(); UpdateWeightTotal(); };
            nudEcWeight.ValueChanged   += (s, e) => UpdateWeightTotal();
        }

        private void SetupUnits()
        {
            if (Props.UseMetric)
            {
                nudMinSize.Minimum      = new decimal(new int[] { 1, 0, 0, 65536 }); // 0.1
                nudMinSize.Maximum      = 20;
                nudMinSize.Increment    = new decimal(new int[] { 1, 0, 0, 65536 }); // 0.1
                nudMinSize.Value        = new decimal(new int[] { 5, 0, 0, 65536 }); // 0.5
                nudMinSize.DecimalPlaces = 1;
                lblHa.Text              = "ha";
            }
            else
            {
                nudMinSize.Minimum      = new decimal(new int[] { 2, 0, 0, 65536 }); // 0.2
                nudMinSize.Maximum      = 50;
                nudMinSize.Increment    = new decimal(new int[] { 1, 0, 0, 65536 }); // 0.1
                nudMinSize.Value        = new decimal(new int[] { 12, 0, 0, 65536 }); // 1.2 ac ≈ 0.5 ha
                nudMinSize.DecimalPlaces = 1;
                lblHa.Text              = "ac";
            }
        }

        // ── Weight helpers ───────────────────────────────────────────────────

        private int ActiveLayerCount()
        {
            return _yieldChecks.Count(c => c.Checked)
                 + (ckEC.Checked ? 1 : 0);
        }

        private void UpdateWeightVisibility()
        {
            bool showWeights = ActiveLayerCount() > 1;

            lblWeightHeader.Visible = showWeights;

            for (int i = 0; i < _yieldChecks.Count; i++)
                _yieldNuds[i].Visible = showWeights && _yieldChecks[i].Checked;

            nudEcWeight.Visible = showWeights && ckEC.Checked;
        }

        private void UpdateWeightTotal()
        {
            if (ActiveLayerCount() <= 1)
            {
                lblTotal.Text = string.Empty;
                return;
            }

            double ySum  = _yieldChecks.Select((ck, i) => ck.Checked ? (double)_yieldNuds[i].Value : 0).Sum();
            double ecVal = ckEC.Checked ? (double)nudEcWeight.Value : 0;
            double total = ySum + ecVal;

            lblTotal.Text      = string.Format("Total: {0:F0} %", total);
            lblTotal.ForeColor = Math.Abs(total - 100) < 1.0
                ? SystemColors.ControlText
                : Color.Red;
        }

        // ── Button handlers ──────────────────────────────────────────────────

        private void btnAll_Click(object sender, EventArgs e)
        {
            foreach (var ck in _yieldChecks) ck.Checked = true;
        }

        private void btnNone_Click(object sender, EventArgs e)
        {
            foreach (var ck in _yieldChecks) ck.Checked = false;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            var selected = _yieldChecks
                .Select((ck, i) => new { ck, path = _yieldPaths[i], nud = _yieldNuds[i] })
                .Where(x => x.ck.Checked)
                .ToList();

            if (selected.Count == 0)
            {
                MessageBox.Show("Please select at least one yield file.", "Generate Zones",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            bool useEc         = ckEC.Checked && !string.IsNullOrEmpty(_ecPath);
            double yieldRawSum = selected.Sum(x => (double)x.nud.Value);
            double ecRaw       = useEc ? (double)nudEcWeight.Value : 0.0;
            double totalRaw    = yieldRawSum + ecRaw;
            if (totalRaw < 1) totalRaw = 1;

            SelectedYieldPaths   = selected.Select(x => x.path).ToList();
            SelectedYieldWeights = selected.Select(x => (double)x.nud.Value / yieldRawSum).ToList();
            YieldFraction        = yieldRawSum / totalRaw;
            EcPath               = useEc ? _ecPath : null;
            EcFraction           = ecRaw / totalRaw;

            ZoneCount = (int)nudZoneCount.Value;

            double sizeVal = (double)nudMinSize.Value;
            MinZoneHa = Props.UseMetric ? sizeVal : sizeVal / AcresPerHa;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        // ── Utility ─────────────────────────────────────────────────────────

        private static string ExtractYear(string filename)
        {
            var m = Regex.Match(filename, @"(20\d{2})");
            return m.Success ? m.Value : string.Empty;
        }
    }
}
