﻿using AgOpenGPS;
using RateController.Classes;
using RateController.Language;
using System;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuConfig : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private FormStart mf;

        public frmMenuConfig(FormStart main, frmMenu menu)
        {
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
                if (byte.TryParse(tbModuleID.Text, out byte id)) mf.ModuleConfig.ModuleID = id;
                if (byte.TryParse(tbSensorCount.Text, out byte ct)) mf.ModuleConfig.SensorCount = ct;
                if (byte.TryParse(tbWifiPort.Text, out byte pt))
                {
                    mf.ModuleConfig.WifiPort = pt;
                }
                else
                {
                    mf.ModuleConfig.WifiPort = 255;
                }
                    mf.ModuleConfig.InvertRelay = ckRelayOn.Checked;
                mf.ModuleConfig.InvertFlow = ckFlowOn.Checked;
                mf.ModuleConfig.ADS1115enabled = ckADS1115enabled.Checked;
                mf.ModuleConfig.RelayType = (byte)cbRelayControl.SelectedIndex;
                mf.ModuleConfig.Save();

                SetButtons(false);
                UpdateForm();
                MainMenu.HighlightUpdateButton();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuConfig/btnOk_Click: " + ex.Message);
            }
        }

        private void frmMenuConfig_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
        }

        private void frmMenuConfig_Load(object sender, EventArgs e)
        {
            SubMenuLayout.SetFormLayout(this, MainMenu, btnOK);

            btnCancel.Left = btnOK.Left - SubMenuLayout.ButtonSpacing;
            btnCancel.Top = btnOK.Top;
            MainMenu.StyleControls(this);
            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            MainMenu.ModuleDefaultsSet += MainMenu_ModuleDefaultsSet;
            PositionForm();
            UpdateForm();
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
            lbModuleID.Text = Lang.lgModuleID;
            lbSensorCount.Text = Lang.lgSensorCount;
            lbWifiPort.Text = Lang.lgWifiPort;
            lbRelay.Text = Lang.lgRelayControl;
            ckRelayOn.Text = Lang.lgInvertRelays;
            ckFlowOn.Text = Lang.lgInvertFlow;
        }

        private void tbModuleID_Enter(object sender, EventArgs e)
        {
            double temp;
            double.TryParse(tbModuleID.Text, out temp);
            using (var form = new FormNumeric(0, 8, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbModuleID.Text = form.ReturnValue.ToString("N0");
                }
            }
        }

        private void tbModuleID_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbSensorCount_Enter(object sender, EventArgs e)
        {
            double temp;
            double.TryParse(tbSensorCount.Text, out temp);
            using (var form = new FormNumeric(0, 2, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbSensorCount.Text = form.ReturnValue.ToString("N0");
                }
            }
        }

        private void tbWifiPort_Enter(object sender, EventArgs e)
        {
            double temp;
            double.TryParse(tbWifiPort.Text, out temp);
            using (var form = new FormNumeric(0, 8, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbWifiPort.Text = form.ReturnValue.ToString("N0");
                }
            }
        }

        private void UpdateForm()
        {
            Initializing = true;
            byte[] data = mf.ModuleConfig.GetData();
            tbModuleID.Text = data[2].ToString();
            tbSensorCount.Text = data[3].ToString();
            cbRelayControl.SelectedIndex = data[5];

            if (data[6]> 60)
            {
                tbWifiPort.Text = "-";
            }
            else
            {
                tbWifiPort.Text = data[6].ToString();
            }

            ckRelayOn.Checked = mf.ModuleConfig.InvertRelay;
            ckFlowOn.Checked = mf.ModuleConfig.InvertFlow;
            ckADS1115enabled.Checked = mf.ModuleConfig.ADS1115enabled;

            Initializing = false;
        }

        private void btnRescan_Click(object sender, EventArgs e)
        {
            ckRelayOn.Checked = true;
            ckFlowOn.Checked = true;
            ckADS1115enabled.Checked = false;
            tbModuleID.Text = "0";
            tbSensorCount.Text = "1";
            tbWifiPort.Text = "-";
            cbRelayControl.SelectedIndex = 0;
        }
    }
}