using AgOpenGPS;
using RateController.Classes;
using RateController.Language;
using System;
using System.Windows.Forms;

namespace RateController.Forms
{
    // "Other" quantity unit: the name RC shows, and optionally what it is for a task controller
    // (volume, mass or count, and how many litres, kg or seeds one unit is).
    public partial class frmQuantityUnit : Form
    {
        private static readonly ushort[] TypeDDI = { 0, 1, 6, 11 };     // None, Volume, Mass, Count
        private bool cResult;
        private ushort cUnitDDI;
        private double cUnitFactor;
        private string cUnitName = "";

        public frmQuantityUnit(string UnitName, ushort UnitDDI, double UnitFactor)
        {
            InitializeComponent();
            cUnitName = UnitName ?? "";
            cUnitDDI = UnitDDI;
            cUnitFactor = UnitFactor;
        }

        public bool Result { get { return cResult; } }
        public ushort UnitDDI { get { return cUnitDDI; } }
        public double UnitFactor { get { return cUnitFactor; } }
        public string UnitName { get { return cUnitName; } }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            cResult = false;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            cUnitName = tbName.Text.Trim();
            cUnitDDI = TypeDDI[cbType.SelectedIndex < 0 ? 0 : cbType.SelectedIndex];
            cUnitFactor = 0;
            if (cUnitDDI != 0 && Props.TryParseNumber(tbFactor.Text, out double f)) cUnitFactor = f;
            cResult = true;
            this.Close();
        }

        private void cbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtons();
        }

        private void frmQuantityUnit_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                Props.SaveFormLocation(this);
            }
        }

        private void frmQuantityUnit_Load(object sender, EventArgs e)
        {
            Props.LoadFormLocation(this);
            this.BackColor = Properties.Settings.Default.MainBackColour;

            this.Text = Lang.lgTCOther;
            lbName.Text = Lang.lgTCUnitName;
            lbType.Text = Lang.lgTCUnitType;
            lbFactor.Text = Lang.lgTCPerUnit;
            cbType.Items[0] = Lang.lgNone;
            cbType.Items[1] = Lang.lgTCVolume;
            cbType.Items[2] = Lang.lgTCMass;
            cbType.Items[3] = Lang.lgTCCount;

            tbName.Text = cUnitName;
            cbType.SelectedIndex = Math.Max(0, Array.IndexOf(TypeDDI, cUnitDDI));
            tbFactor.Text = cUnitFactor > 0 ? Props.NumberText(cUnitFactor) : "";
            SetButtons();
        }

        private void SetButtons()
        {
            bool HasType = cbType.SelectedIndex > 0;
            tbFactor.Enabled = HasType;

            // a name is needed; a type needs how many litres, kg or seeds one unit is
            bool FactorOK = Props.TryParseNumber(tbFactor.Text, out double f) && f > 0;
            btnOK.Enabled = tbName.Text.Trim().Length > 0 && (!HasType || FactorOK);
        }

        private void tbFactor_Enter(object sender, EventArgs e)
        {
            Props.TryParseNumber(tbFactor.Text, out double tempD);
            using (var form = new FormNumeric(0, 1000000, tempD))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    tbFactor.Text = Props.NumberText(form.ReturnValue);
                }
            }
            btnOK.Focus();      // the keypad opens again only when the box is entered again
        }

        private void tbName_TextChanged(object sender, EventArgs e)
        {
            SetButtons();
        }

        private void tbFactor_TextChanged(object sender, EventArgs e)
        {
            SetButtons();
        }
    }
}
