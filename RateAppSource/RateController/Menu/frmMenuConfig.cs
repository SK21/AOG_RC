using AgOpenGPS;
using RateController.Classes;
using RateController.Language;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuConfig : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private Timer cModuleTimer;
        private Color cModuleColor;

        public frmMenuConfig(frmMenu menu)
        {
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
                // the module being edited is chosen on the Boards page, as are boards and
                // their descriptions
                if (byte.TryParse(tbSensorCount.Text, out byte ct)) Core.ModuleConfig.SensorCount = ct;
                Core.ModuleConfig.InvertRelay = ckRelayOn.Checked;
                Core.ModuleConfig.InvertFlow = ckFlowOn.Checked;
                Core.ModuleConfig.ADS1115enabled = ckADS1115enabled.Checked;
                Core.ModuleConfig.OnboardRelayType = (byte)cbOnboardRelays.SelectedIndex;
                Core.ModuleConfig.RemoteRelayType = (byte)cbRemoteRelays.SelectedIndex;

                Core.ModuleConfig.Save();

                SetButtons(false);
                UpdateForm();

                // refresh the other open module pages too - the Pins page row
                // count follows the sensor count saved here
                MainMenu.DefaultsSet();
                MainMenu.HighlightModuleSave();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuConfig/btnOk_Click: " + ex.Message);
            }
        }

        private void frmMenuConfig_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainMenu.MenuMoved -= MainMenu_MenuMoved;
            MainMenu.ModuleDefaultsSet -= MainMenu_ModuleDefaultsSet;
            if (cModuleTimer != null)
            {
                cModuleTimer.Stop();
                cModuleTimer.Dispose();
                cModuleTimer = null;
            }
            Props.SaveFormLocation(this);
        }

        private void frmMenuConfig_Load(object sender, EventArgs e)
        {
            SubMenuLayout.SetFormLayout(this, MainMenu, btnOK);

            btnCancel.Left = btnOK.Left - SubMenuLayout.ButtonSpacing;
            btnCancel.Top = btnOK.Top;
            MainMenu.StyleControls(this);

            lbModule.Font = new Font(lbModule.Font, FontStyle.Underline);
            cModuleColor = lbModule.ForeColor;

            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            MainMenu.ModuleDefaultsSet += MainMenu_ModuleDefaultsSet;
            PositionForm();

            UpdateForm();

            // keep the module identity line live while the form is open - the module
            // reports its label every ~2 s (PGN 32403) and connection state can change
            cModuleTimer = new Timer();
            cModuleTimer.Interval = 1000;
            cModuleTimer.Tick += ModuleTimer_Tick;
            cModuleTimer.Start();
        }

        private void ModuleTimer_Tick(object sender, EventArgs e)
        {
            byte id = Core.ModuleConfig.GetData()[2];
            // old firmware outranks a duplicate: nothing else can be done until it's updated
            bool rejected = Core.ModulesBoardID.Rejected(id);
            bool dup = !rejected && Core.DuplicateModule(id);

            string text;
            if (dup)
            {
                text = string.Format(Lang.lgTwoBoardsAnswering, id);
            }
            else
            {
                text = Core.ModuleConfigDescription();
            }

            if (lbModule.Text != text) lbModule.Text = text;
            Color fore = (dup || rejected) ? Color.Red : cModuleColor;
            if (lbModule.ForeColor != fore) lbModule.ForeColor = fore;
        }

        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
            Props.DrawGroupBox((GroupBox)sender, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }

        private void MainMenu_MenuMoved(object sender, EventArgs e)
        {
            PositionForm();
        }

        private void MainMenu_ModuleDefaultsSet(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
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
            lbSensorCount.Text = Lang.lgSensorCount;
            lbRelay.Text = Lang.lgRelayControl;
            lbRemoteRelay.Text = Lang.lgRemoteRelayControl;
            ckRelayOn.Text = Lang.lgInvertRelays;
            ckFlowOn.Text = Lang.lgInvertFlow;
        }

        private void Setting_Changed(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbSensorCount_Enter(object sender, EventArgs e)
        {
            double temp;
            double.TryParse(tbSensorCount.Text, out temp);

            // cap by the module's board type and firmware (2, or 6 on 6-product boards
            // with current firmware); the dropdown is live-synced to the config
            byte id = Core.ModuleConfig.GetData()[2];
            using (var form = new FormNumeric(0, Core.MaxSensorsForModule(id), temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbSensorCount.Text = form.ReturnValue.ToString("N0");
                }
            }
        }

        private void UpdateForm()
        {
            Initializing = true;
            try
            {
                byte[] data = Core.ModuleConfig.GetData();
                lbModule.Text = Core.ModuleConfigDescription();
                tbSensorCount.Text = data[3].ToString();
                cbOnboardRelays.SelectedIndex = data[5];
                cbRemoteRelays.SelectedIndex = data[6];
                ckRelayOn.Checked = Core.ModuleConfig.InvertRelay;
                ckFlowOn.Checked = Core.ModuleConfig.InvertFlow;
                ckADS1115enabled.Checked = Core.ModuleConfig.ADS1115enabled;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuConfig/UpdateForm: " + ex.Message);
            }
            Initializing = false;
        }
    }
}