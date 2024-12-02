using AgOpenGPS;
using RateController.Properties;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmOptions : Form
    {
        public FormStart mf;
        private bool FormEdited;
        private bool Initializing = true;
        private string[] LanguageIDs;
        private RadioButton[] LanguageRBs;
        private TabPage[] Tabs;
        private bool tbSimSpeedChanged = false;
        private bool tbSpeedChanged = false;

        public frmOptions(FormStart CalledFrom)
        {
            InitializeComponent();

            #region // language

            tcOptions.TabPages[0].Text = Lang.lgPage1;
            tcOptions.TabPages[1].Text = Lang.lgPage2;
            tcOptions.TabPages[2].Text = Lang.lgPrimedStart;
            tcOptions.TabPages[3].Text = Lang.lgSwitches;
            tcOptions.TabPages[4].Text = Lang.lgLanguage;
            tcOptions.TabPages[5].Text = Lang.lgColor;

            ckMetric.Text = Lang.lgMetric;
            ckScreenSwitches.Text = Lang.lgSwitches;
            ckWorkSwitch.Text = Lang.lgWorkSwitch;
            ckLargeScreen.Text = Lang.lgLargeScreen;
            ckTransparent.Text = Lang.lgTransparent;

            lbOnTime.Text = Lang.lgOnTime;
            lbPrimedSpeed.Text = Lang.lgSpeed;
            lbDelay.Text = Lang.lgSwitchDelay;
            lbOnSeconds.Text = Lang.lgSeconds;
            lbDelaySeconds.Text = Lang.lgSeconds;

            #endregion // language

            mf = CalledFrom;

            LanguageRBs = new RadioButton[] { rbEnglish, rbDeustch, rbHungarian, rbNederlands, rbPolish, rbRussian, rbFrench };
            LanguageIDs = new string[] { "en", "de", "hu", "nl", "pl", "ru", "fr" };
            for (int i = 0; i < LanguageRBs.Length; i++)
            {
                LanguageRBs[i].CheckedChanged += Language_CheckedChanged;
            }

            Tabs = new TabPage[] { tabPage1, tabPage2, tabPage3, tabPage4, tabPage5,tabPage6 };
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm();
            btnOK.Focus();
            SetButtons(false);
            tbExample.ForeColor = Properties.Settings.Default.ForeColour;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (!FormEdited)
                {
                    this.Close();
                }
                else
                {
                    if (mf.Tls.ReadOnly)
                    {
                        mf.Tls.ShowHelp("File is read only.", "Help", 5000, false, false, true);
                    }
                    else
                    {
                        Save();
                        SetButtons(false);
                        if (ckDefaultProduct.Checked) mf.Products.Load(true);
                        UpdateForm();
                    }
                }
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp(ex.Message, this.Text, 3000, true);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            switch (tcOptions.SelectedIndex)
            {
                case 3:
                    mf.SwitchObjects.Reset();
                    SetButtons(true);
                    UpdateForm();
                    break;

                case 5:
                    tbExample.ForeColor = Color.Yellow;
                    SetButtons(true);
                    break;
            }
        }

        private void CheckRelayDefs(byte RelayID, byte ModuleID)
        {
            // check if relay is defined as 'Switch' type
            if (RelayID > 0)
            {
                clsRelay Rly = mf.RelayObjects.Item(RelayID - 1, ModuleID);
                if (Rly.Type != RelayTypes.Switch)
                {
                    var Hlp = new frmMsgBox(mf, "Change relay type from '" + Rly.TypeDescription + "' to 'Switch'?", "Switches", true);
                    Hlp.TopMost = true;
                    Hlp.ShowDialog();
                    if (Hlp.Result)
                    {
                        // change relay type
                        Rly.Type = RelayTypes.Switch;
                        Rly.Save();
                    }
                    Hlp.Close();
                }
            }
        }

        private void ckDefaultProduct_CheckedChanged(object sender, EventArgs e)
        {
            if (ckDefaultProduct.Checked)
            {
                var Hlp = new frmMsgBox(mf, "Confirm reset all products to default values?", "Reset", true);
                Hlp.TopMost = true;

                Hlp.ShowDialog();
                bool Result = Hlp.Result;
                Hlp.Close();
                if(Result)
                {
                SetButtons(true);
                }
                else
                {
                    ckDefaultProduct.Checked = false;
                }
            }
        }

        private void ckDualAuto_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void ckNoMaster_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void ckNoMaster_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Send master switch always on to modules.";

            mf.Tls.ShowHelp(Message, "Master Switch");
            hlpevent.Handled = true;
        }

        private void ckSingle_CheckedChanged(object sender, EventArgs e)
        {
            if (ckSingle.Checked) SetButtons(true);
        }

        private void ckTransparent_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void DGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                double Temp;
                string val = DGV.Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue.ToString();
                switch (e.ColumnIndex)
                {
                    case 2:
                        // module
                        double.TryParse(val, out Temp);
                        using (var form = new FormNumeric(0, 7, Temp))
                        {
                            var result = form.ShowDialog();
                            if (result == DialogResult.OK)
                            {
                                DGV.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = form.ReturnValue;
                            }
                        }
                        break;

                    case 3:
                        // relay
                        double.TryParse(val, out Temp);
                        using (var form = new FormNumeric(0, 16, Temp))
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
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmOptions/CellClick " + ex.Message);
            }
        }

        private void DGV_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!Initializing) SetButtons(true);
        }

        private void DGV_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            mf.Tls.WriteErrorLog("frmOptions/DGV_DataError: Row,Column: " + e.RowIndex.ToString() + ", " + e.ColumnIndex.ToString()
                + " Exception: " + e.Exception.ToString());
        }

        private void frmOptions_FormClosed(object sender, FormClosedEventArgs e)
        {
            mf.Tls.SaveFormData(this);
        }

        private void frmOptions_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);

            DGV.BackgroundColor = DGV.DefaultCellStyle.BackColor;
            DGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            SetDayMode();
            UpdateForm();
            colorDialog1.Color = Properties.Settings.Default.ForeColour;
            tbExample.ForeColor = colorDialog1.Color;
        }

        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            mf.Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }

        private void Language_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void LoadData(bool UpdateObject = false)
        {
            try
            {
                if (UpdateObject) mf.SwitchObjects.Load();
                dataSet1.Clear();
                foreach (clsSwitch SW in mf.SwitchObjects.Items)
                {
                    DataRow Rw = dataSet1.Tables[0].NewRow();
                    Rw[0] = SW.ID + 1;
                    Rw[1] = SW.Description;
                    Rw[2] = SW.ModuleID;

                    if (SW.RelayID == 0)
                    {
                        Rw[3] = "";
                    }
                    else
                    {
                        Rw[3] = SW.RelayID;
                    }

                    dataSet1.Tables[0].Rows.Add(Rw);
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmOptions/LoadData: " + ex.Message);
            }
        }

        private void rbLarge_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void Save()
        {
            try
            {
                if (tbSimSpeedChanged)
                {
                    if (double.TryParse(tbSimSpeed.Text, out double Speed)) mf.SimSpeed = Speed;
                    tbSimSpeedChanged = false;
                }
                else if (tbSpeedChanged)
                {
                    if (double.TryParse(tbSpeed.Text, out double Spd)) mf.SimSpeed = Spd;
                    tbSpeedChanged = false;
                }

                if (double.TryParse(tbTime.Text, out double Time)) mf.PrimeTime = Time;
                if (int.TryParse(tbDelay.Text, out int Delay)) mf.PrimeDelay = Delay;

                mf.MasterOverride = ckNoMaster.Checked;
                mf.UseTransparent = ckTransparent.Checked;
                mf.UseInches = !ckMetric.Checked;
                mf.ShowSwitches = ckScreenSwitches.Checked;
                mf.SwitchBox.UseWorkSwitch = ckWorkSwitch.Checked;
                mf.ShowPressure = ckPressure.Checked;
                if (double.TryParse(tbPressureCal.Text, out double Pressure)) mf.PressureCal = Pressure;
                if (double.TryParse(tbPressureOffset.Text, out double PresOff)) mf.PressureOffset = PresOff;

                if (ckSimSpeed.Checked)
                {
                    mf.SimMode = SimType.Sim_Speed;
                }
                else
                {
                    mf.SimMode = SimType.Sim_None;
                }

                mf.UseLargeScreen = ckLargeScreen.Checked;
                if (ckSingle.Checked) mf.SwitchScreens(true);

                mf.UseDualAuto = ckDualAuto.Checked;
                mf.ResumeAfterPrime = ckResume.Checked;

                SaveLanguage();

                // data grid
                for (int i = 0; i < DGV.Rows.Count; i++)
                {
                    clsSwitch SW = mf.SwitchObjects.Item(i);
                    for (int j = 1; j < 4; j++)
                    {
                        string val = DGV.Rows[i].Cells[j].EditedFormattedValue.ToString();
                        if (val == "") val = "0";
                        switch (j)
                        {
                            case 1:
                                // description
                                SW.Description = val;
                                break;

                            case 2:
                                // module
                                if (byte.TryParse(val, out byte md))
                                {
                                    SW.ModuleID = md;
                                }
                                break;

                            case 3:
                                // relay
                                if (byte.TryParse(val, out byte rly))
                                {
                                    SW.RelayID = rly;
                                }
                                break;
                        }
                    }
                    CheckRelayDefs(SW.RelayID, SW.ModuleID);
                }
                mf.SwitchObjects.Save();
                if (mf.SwitchesForm != null) mf.SwitchesForm.SetDescriptions();

                Properties.Settings.Default.ForeColour = tbExample.ForeColor;
                Properties.Settings.Default.Save();
                mf.RaiseColorChanged();
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmOptions/SaveData: " + ex.Message);
            }
        }

        private void SaveLanguage()
        {
            int Lan = 0;
            for (int i = 0; i < LanguageRBs.Length; i++)
            {
                if (LanguageRBs[i].Checked)
                {
                    Lan = i;
                    break;
                }
            }

            if (Properties.Settings.Default.setF_culture != LanguageIDs[Lan])
            {
                Properties.Settings.Default.setF_culture = LanguageIDs[Lan];
                Settings.Default.UserLanguageChange = true;
                Properties.Settings.Default.Save();

                Form fs = mf.Tls.IsFormOpen("frmLargeScreen");
                if (fs != null)
                {
                    mf.Restart = true;
                    mf.Lscrn.Close();
                }
                else
                {
                    mf.ChangeLanguage();
                }
            }
        }

        private void SetButtons(bool Edited = false)
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
                    btnOK.Enabled = true;
                }
                FormEdited = Edited;
            }
        }

        private void SetDayMode()
        {
            if (Properties.Settings.Default.IsDay)
            {
                this.BackColor = Properties.Settings.Default.DayColour;
                foreach (Control c in this.Controls)
                {
                    c.ForeColor = Color.Black;
                }

                for (int i = 0; i < Tabs.Length; i++)
                {
                    Tabs[i].BackColor = Properties.Settings.Default.DayColour;
                }
            }
            else
            {
                this.BackColor = Properties.Settings.Default.NightColour;
                foreach (Control c in this.Controls)
                {
                    c.ForeColor = Color.White;
                }

                for (int i = 0; i < Tabs.Length; i++)
                {
                    Tabs[i].BackColor = Properties.Settings.Default.NightColour;
                }
            }
        }

        private void tbDelay_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbDelay.Text, out tempD);
            using (var form = new FormNumeric(0, 8, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbDelay.Text = form.ReturnValue.ToString("N0");
                }
            }
        }

        private void tbDelay_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbDelay.Text, out tempD);
            if (tempD < 0 || tempD > 8)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbPressureCal_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbPressureCal.Text, out tempD);
            using (var form = new FormNumeric(0, 10000, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbPressureCal.Text = form.ReturnValue.ToString("N1");
                }
            }
        }

        private void tbPressureCal_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbPressureCal_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbPressureCal.Text, out tempD);
            if (tempD < 0 || tempD > 10000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbPressureOffset_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbPressureOffset.Text, out tempD);
            using (var form = new FormNumeric(-10000, 10000, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbPressureOffset.Text = form.ReturnValue.ToString("N1");
                }
            }
        }

        private void tbPressureOffset_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbPressureOffset.Text, out tempD);
            if (tempD < -10000 || tempD > 10000)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbSimSpeed_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbSimSpeed.Text, out tempD);
            using (var form = new FormNumeric(0, 40, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbSimSpeed.Text = form.ReturnValue.ToString("N1");
                }
            }
        }

        private void tbSimSpeed_TextChanged(object sender, EventArgs e)
        {
            if (!Initializing) tbSimSpeedChanged = true;
            SetButtons(true);
        }

        private void tbSimSpeed_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbSimSpeed.Text, out tempD);
            if (tempD < 0 || tempD > 40)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbSpeed_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbSpeed.Text, out tempD);
            using (var form = new FormNumeric(0, 40, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbSpeed.Text = form.ReturnValue.ToString("N1");
                }
            }
        }

        private void tbSpeed_TextChanged(object sender, EventArgs e)
        {
            if (!Initializing) tbSpeedChanged = true;
            SetButtons(true);
        }

        private void tbSpeed_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbSpeed.Text, out tempD);
            if (tempD < 0 || tempD > 40)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbTime_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbTime.Text, out tempD);
            using (var form = new FormNumeric(0, 30, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbTime.Text = form.ReturnValue.ToString("N1");
                }
            }
        }

        private void tbTime_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbTime.Text, out tempD);
            if (tempD < 0 || tempD > 30)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tcOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnReset.Visible = (tcOptions.SelectedIndex == 3||tcOptions.SelectedIndex==5);
        }

        private void UpdateForm(bool UpdateObject = false)
        {
            Initializing = true;

            tbSpeed.Text = mf.SimSpeed.ToString("N1");
            tbSimSpeed.Text = mf.SimSpeed.ToString("N1");

            tbTime.Text = mf.PrimeTime.ToString("N0");
            tbDelay.Text = mf.PrimeDelay.ToString("N0");

            ckTransparent.Checked = mf.UseTransparent;
            ckMetric.Checked = !mf.UseInches;
            ckScreenSwitches.Checked = mf.ShowSwitches;
            ckWorkSwitch.Checked = mf.SwitchBox.UseWorkSwitch;
            ckPressure.Checked = mf.ShowPressure;
            ckSimSpeed.Checked = (mf.SimMode == SimType.Sim_Speed);
            ckDualAuto.Checked = mf.UseDualAuto;
            ckResume.Checked = mf.ResumeAfterPrime;
            ckNoMaster.Checked = mf.MasterOverride;
            ckLargeScreen.Checked = mf.UseLargeScreen;
            tbPressureOffset.Text = mf.PressureOffset.ToString("N1");
            tbPressureCal.Text = mf.PressureCal.ToString("N1");

            // language
            for (int i = 0; i < LanguageRBs.Length; i++)
            {
                if (LanguageIDs[i] == Properties.Settings.Default.setF_culture)
                {
                    LanguageRBs[i].Checked = true;
                    break;
                }
            }

            if (mf.UseInches)
            {
                lbSpeed.Text = "MPH";
                lbSimUnits.Text = "MPH";
            }
            else
            {
                lbSpeed.Text = "KMH";
                lbSimUnits.Text = "KMH";
            }
            LoadData(UpdateObject);

            ckDefaultProduct.Checked = false;
            ckSingle.Checked = false;

            Initializing = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(colorDialog1.ShowDialog()==DialogResult.OK)
            {
                //Properties.Settings.Default.ForeColour = colorDialog1.Color;
                tbExample.ForeColor = colorDialog1.Color;
                SetButtons(true);
            }
        }
    }
}