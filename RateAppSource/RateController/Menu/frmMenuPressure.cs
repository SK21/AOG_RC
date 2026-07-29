using AgOpenGPS;
using RateController.Classes;
using RateController.Language;
using RateController.PGNs;
using System;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuPressure : Form
    {
        // Entry limit for the raw sensor reading fields (min/max cal points and the zero
        // reading). These are ADC counts, not pressure units. An ADS1115 is a signed 16 bit
        // converter reading up to 32767 - an RC15 runs well past the old 5000 limit, which
        // only ever suited a 12 bit analogRead (0-4095).
        private const int MaxRawReading = 32767;

        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;

        public frmMenuPressure(frmMenu menu)
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
                int Module = cbModules.SelectedIndex;
                PGN32505 Gate = new PGN32505();

                // What the module receives is the raw threshold (32505), derived from the max
                // pressure AND the calibration. Compare the computed threshold either side of
                // the save - the old check looked only at the max pressure, so editing the
                // calibration moved the threshold with nothing prompting the user to send it.
                ushort OldThreshold = Gate.RawThreshold(Module);

                Props.SetPressureCal(Module * 5, double.Parse(tbMinVol.Text));
                Props.SetPressureCal(Module * 5 + 1, double.Parse(tbMinPres.Text));
                Props.SetPressureCal(Module * 5 + 2, double.Parse(tbMaxVol.Text));
                Props.SetPressureCal(Module * 5 + 3, double.Parse(tbMaxPres.Text));
                Props.SetPressureCal(Module * 5 + 4, double.Parse(tbZeroReading.Text));
                Props.ShowPressure = ckPressure.Checked;

                double NewMax = double.TryParse(tbAlarmPressure.Text, out double mp) ? mp : 0;
                Props.SetMaxPressure(Module, NewMax);

                if (Gate.RawThreshold(Module) != OldThreshold)
                {
                    MainMenu.HighlightUpdateButton();
                    Props.ShowMessage("Pressure gate changed. Press Send to apply it to the module.", "Pressure", 4000);
                }

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
            MainMenu.MenuMoved -= MainMenu_MenuMoved;
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
            lbCurrent.Text = Lang.lgPressureCurrent;
            lbZero.Text = Lang.lgPressureZero;
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
            using (var form = new FormNumeric(0, MaxRawReading, temp))
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
            using (var form = new FormNumeric(0, MaxRawReading, temp))
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

        private void tbZeroReading_Enter(object sender, EventArgs e)
        {
            double temp;
            double.TryParse(tbZeroReading.Text, out temp);
            using (var form = new FormNumeric(0, MaxRawReading, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbZeroReading.Text = form.ReturnValue.ToString("N1");
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateRaw();
        }

        private void UpdateForm()
        {
            Initializing = true;
            try
            {
                tbMinVol.Text = Props.GetPressureCal(cbModules.SelectedIndex * 5).ToString("N0");
                tbMinPres.Text = Props.GetPressureCal(cbModules.SelectedIndex * 5 + 1).ToString("N2");
                tbMaxVol.Text = Props.GetPressureCal(cbModules.SelectedIndex * 5 + 2).ToString("N0");
                tbMaxPres.Text = Props.GetPressureCal(cbModules.SelectedIndex * 5 + 3).ToString("N2");
                tbZeroReading.Text = Props.GetPressureCal(cbModules.SelectedIndex * 5 + 4).ToString("N0");
                tbAlarmPressure.Text = Props.GetMaxPressure(cbModules.SelectedIndex).ToString("N2");

                ckPressure.Checked = Props.ShowPressure;
                SetModuleIndicator();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuPressure/UpdateForm: " + ex.Message);
            }
            Initializing = false;
        }

        private void UpdateRaw()
        {
            double Reading = Core.ModulesStatus.PressureReading(cbModules.SelectedIndex);
            lbRaw.Text = Reading.ToString("N0");
            lbPressureReading.Text = Props.PressureReading(cbModules.SelectedIndex, Reading).ToString("N2");
        }

        private void tbMaxPressure_Enter(object sender, EventArgs e)
        {
            double temp;
            double.TryParse(tbAlarmPressure.Text, out temp);
            using (var form = new FormNumeric(0, 200, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbAlarmPressure.Text = form.ReturnValue.ToString("N1");
                }
            }

        }
    }
}