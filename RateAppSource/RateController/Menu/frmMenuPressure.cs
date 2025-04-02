using AgOpenGPS;
using RateController.Classes;
using RateController.Language;
using System;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuPressure : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private FormStart mf;

        public frmMenuPressure(FormStart main, frmMenu menu)
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
                Props.SetPressureCal(cbModules.SelectedIndex * 4, double.Parse(tbMinVol.Text));
                Props.SetPressureCal(cbModules.SelectedIndex * 4 + 1, double.Parse(tbMinPres.Text));
                Props.SetPressureCal(cbModules.SelectedIndex * 4 + 2, double.Parse(tbMaxVol.Text));
                Props.SetPressureCal(cbModules.SelectedIndex * 4 + 3, double.Parse(tbMaxPres.Text));
                Props.ShowPressure = ckPressure.Checked;
                SetButtons(false);
                UpdateForm();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuPressure/btnOk_Click: " + ex.Message);
            }
        }

        private void cbModules_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void frmMenuPressure_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
            timer1.Enabled = false;
        }

        private void frmMenuPressure_Load(object sender, EventArgs e)
        {
            SubMenuLayout.SetFormLayout(this, MainMenu, btnOK);
            btnCancel.Left = btnOK.Left - SubMenuLayout.ButtonSpacing;
            btnCancel.Top = btnOK.Top;
            ckPressure.Left = btnCancel.Left - SubMenuLayout.ButtonSpacing - 20;
            ckPressure.Top = btnOK.Top - 10;

            MainMenu.StyleControls(this);
            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            this.BackColor = Properties.Settings.Default.MainBackColour;
            PositionForm();

            cbModules.SelectedIndex = 0;

            UpdateForm();
            timer1.Enabled = true;
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

        private void SetButtons(bool Edited)
        {
            if (!Initializing)
            {
                if (Edited)
                {
                    btnCancel.Enabled = true;
                    btnOK.Enabled = true;
                    cbModules.Enabled = false;
                }
                else
                {
                    btnCancel.Enabled = false;
                    btnOK.Enabled = false;
                    cbModules.Enabled = true;
                }

                cEdited = Edited;
                this.Tag = cEdited;
            }
        }

        private void SetLanguage()
        {
            lbMin.Text = Lang.lgPressureMin;
            lbMax.Text = Lang.lgPressureMax;
            lbPressure.Text = Lang.lgPressurePressure;
            lbVoltage.Text = Lang.lgPressureVoltage;
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

        private void tbMaxPres_Enter(object sender, EventArgs e)
        {
            double temp;
            double.TryParse(tbMaxPres.Text, out temp);
            using (var form = new FormNumeric(0, 200, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbMaxPres.Text = form.ReturnValue.ToString("N1");
                }
            }
        }

        private void tbMaxVol_Enter(object sender, EventArgs e)
        {
            double temp;
            double.TryParse(tbMaxVol.Text, out temp);
            using (var form = new FormNumeric(0, 5000, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbMaxVol.Text = form.ReturnValue.ToString("N1");
                }
            }
        }

        private void tbMinPres_Enter(object sender, EventArgs e)
        {
            double temp;
            double.TryParse(tbMinPres.Text, out temp);
            using (var form = new FormNumeric(0, 200, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbMinPres.Text = form.ReturnValue.ToString("N1");
                }
            }
        }

        private void tbMinVol_Enter(object sender, EventArgs e)
        {
            double temp;
            double.TryParse(tbMinVol.Text, out temp);
            using (var form = new FormNumeric(0, 5000, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbMinVol.Text = form.ReturnValue.ToString("N1");
                }
            }
        }

        private void tbMinVol_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateRaw();
        }

        private void UpdateForm()
        {
            Initializing = true;

            tbMinVol.Text = Props.GetPressureCal(cbModules.SelectedIndex * 4).ToString("N1");
            tbMinPres.Text = Props.GetPressureCal(cbModules.SelectedIndex * 4 + 1).ToString("N1");
            tbMaxVol.Text = Props.GetPressureCal(cbModules.SelectedIndex * 4 + 2).ToString("N1");
            tbMaxPres.Text = Props.GetPressureCal(cbModules.SelectedIndex * 4 + 3).ToString("N1");

            ckPressure.Checked = Props.ShowPressure;
            SetModuleIndicator();

            Initializing = false;
        }

        private void UpdateRaw()
        {
            double Reading = mf.ModulesStatus.PressureReading(cbModules.SelectedIndex);
            lbRaw.Text = Reading.ToString("N0");
            lbPressureReading.Text = Props.PressureReading(cbModules.SelectedIndex, Reading).ToString("N1");
        }
    }
}