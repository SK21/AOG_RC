using AgOpenGPS;
using RateController.Classes;
using RateController.Language;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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

        private void btnCopy_Click(object sender, EventArgs e)
        {
            double slope = 0;
            double intercept = 0;
            if (double.TryParse(lbCalSlope.Text, out double sl)) slope = sl;
            if (double.TryParse(lbCalIntercept.Text, out double pt)) intercept = pt;
            tbSlope.Text = slope.ToString("N1");
            tbIntercept.Text = intercept.ToString("N1");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (DGV.CurrentCell != null)
            {
                int currentRowIndex = DGV.CurrentCell.RowIndex;
                if (currentRowIndex >= 0 && currentRowIndex < DGV.Rows.Count)
                {
                    DGV.Rows.RemoveAt(currentRowIndex);
                    SetButtons(true);
                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (DGV.Rows.Count < 10)
                {
                    DataRow RW = dataSet1.Tables[0].NewRow();
                    RW[0] = DGV.Rows.Count + 1;
                    RW[1] = mf.ModulesStatus.Pressure(cbModules.SelectedIndex);

                    double pressure = 0;
                    if (double.TryParse(tbPressure.Text, out double pr)) pressure = pr;
                    RW[2] = pr;

                    RW[3] = -1;
                    dataSet1.Tables[0].Rows.Add(RW);
                    SetButtons(true);
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmMenuPressure/btnNew: " + ex.Message);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                SaveData();
                SetButtons(false);
                UpdateForm();
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmMenuPressure/btnOk_Click: " + ex.Message);
            }
        }

        private void cbModules_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void DGV_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            mf.Tls.WriteErrorLog("frmMenuPressure/DGV_DataError: Row,Column: " + e.RowIndex.ToString() + ", " + e.ColumnIndex.ToString()
            + " Exception: " + e.Exception.ToString());
        }

        private void frmMenuPressure_FormClosed(object sender, FormClosedEventArgs e)
        {
            mf.Tls.SaveFormData(this);
            timer1.Enabled = false;
        }

        private void frmMenuPressure_Load(object sender, EventArgs e)
        {
            // menu 800,600
            // sub menu 540,630
            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            mf.Tls.LoadFormData(this, "", false);
            this.BackColor = Properties.Settings.Default.BackColour;
            this.Width = MainMenu.Width - 260;
            this.Height = MainMenu.Height - 50;
            btnOK.Left = this.Width - 84;
            btnOK.Top = this.Height - 84;
            btnCancel.Left = btnOK.Left - 78;
            btnCancel.Top = btnOK.Top;
            btnCopy.Left = btnCancel.Left - 78;
            btnCopy.Top = btnOK.Top;
            btnDelete.Left = btnCopy.Left - 78;
            btnDelete.Top = btnOK.Top;
            btnNew.Left = btnDelete.Left - 78;
            btnNew.Top = btnOK.Top;
            ckPressure.Left = btnNew.Left - 85;
            ckPressure.Top = btnOK.Top;

            MainMenu.StyleControls(this);
            PositionForm();

            cbModules.SelectedIndex = 0;

            DGV.BackgroundColor = DGV.DefaultCellStyle.BackColor;
            DGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            UpdateForm();
            timer1.Enabled = true;
        }

        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            mf.Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }

        private void LoadData()
        {
            try
            {
                dataSet1.Clear();
                double CalSlope = 0;
                double CalIntercept = 0;
                double Slope = 0;
                double Intercept = 0;
                int MinRaw = 0;
                int count = 0;
                if (mf.PressureObjects.PressureItemFound(cbModules.SelectedIndex))
                {
                    clsPressure Pres = mf.PressureObjects.Item(cbModules.SelectedIndex);
                    Slope = Pres.Slope;
                    Intercept = Pres.Intercept;
                    MinRaw = Pres.MinimumRawData;

                    List<clsPressureRawData> RawData = mf.PressureObjects.GetCalDataByModuleID(cbModules.SelectedIndex);
                    if (RawData.Count > 0)
                    {
                        foreach (clsPressureRawData rawData in RawData)
                        {
                            DataRow RW = dataSet1.Tables[0].NewRow();
                            RW[0] = ++count;
                            RW[1] = rawData.RawData;
                            RW[2] = rawData.Pressure;
                            RW[3] = rawData.ID;
                            dataSet1.Tables[0].Rows.Add(RW);
                        }
                        (CalSlope, CalIntercept) = mf.PressureObjects.GetSlopeAndInterceptByModuleID(cbModules.SelectedIndex);
                    }
                }
                lbCalSlope.Text = CalSlope.ToString("N1");
                lbCalIntercept.Text = CalIntercept.ToString("N1");
                tbSlope.Text = Slope.ToString("N1");
                tbIntercept.Text = Intercept.ToString("N1");
                tbMin.Text = MinRaw.ToString("N0");
                ckPressure.Checked = mf.ShowPressure;
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmMenuPressure/LoadData:" + ex.Message);
            }
        }

        private void MainMenu_MenuMoved(object sender, EventArgs e)
        {
            PositionForm();
        }

        private void PositionForm()
        {
            this.Top = MainMenu.Top + 30;
            this.Left = MainMenu.Left + 246;
        }

        private void SaveData()
        {
            try
            {
                // DGV
                mf.PressureObjects.DeleteCalDataByModuleID(cbModules.SelectedIndex);
                for (int i = 0; i < DGV.Rows.Count; i++)
                {
                    clsPressureRawData NewData = new clsPressureRawData();
                    NewData.ModuleID = cbModules.SelectedIndex;
                    for (int j = 1; j < 4; j++)
                    {
                        string StringVal = DGV.Rows[i].Cells[j].EditedFormattedValue.ToString();
                        double Val = 0;
                        if (StringVal != "")
                        {
                            if (Double.TryParse(StringVal, out double v)) Val = v;
                        }
                        switch (j)
                        {
                            case 1:
                                // Raw Data
                                NewData.RawData = (int)Val;
                                break;

                            case 2:
                                // known pressure
                                NewData.Pressure = Val;
                                break;

                            case 3:
                                // ID
                                NewData.ID = (int)Val;
                                break;
                        }
                    }
                    mf.PressureObjects.AddCalData(NewData);
                }
                mf.PressureObjects.SaveCalData();

                // textboxes
                double slope = 0;
                double intercept = 0;
                int MinRaw = 0;
                if (double.TryParse(tbSlope.Text, out double sl)) slope = sl;
                if (double.TryParse(tbIntercept.Text, out double cpt)) intercept = cpt;
                if (int.TryParse(tbMin.Text, out int mn)) MinRaw = mn;
                clsPressure pres = mf.PressureObjects.Item(cbModules.SelectedIndex);
                pres.Slope = slope;
                pres.Intercept = intercept;
                pres.MinimumRawData = MinRaw;
                mf.PressureObjects.Save(cbModules.SelectedIndex);
                mf.ShowPressure = ckPressure.Checked;
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmMenuPressure/SaveData: " + ex.Message);
            }
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
                    btnCopy.Enabled = false;
                    btnDelete.Enabled = false;
                }
                else
                {
                    btnCancel.Enabled = false;
                    btnOK.Enabled = false;
                    cbModules.Enabled = true;
                    btnCopy.Enabled = true;
                    btnDelete.Enabled = true;
                }

                cEdited = Edited;
                this.Tag = cEdited;
            }
        }

        private void SetLanguage()
        {
            DGV.Columns[0].HeaderText = Lang.lgPressureReading;
            DGV.Columns[1].HeaderText = Lang.lgPressureRaw;
            DGV.Columns[2].HeaderText = Lang.lgPressurePressure;
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

        private void tbIntercept_Enter(object sender, EventArgs e)
        {
            double temp;
            double.TryParse(tbIntercept.Text, out temp);
            using (var form = new FormNumeric(-100, 100, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbIntercept.Text = form.ReturnValue.ToString("N1");
                }
            }
        }

        private void tbMin_Enter(object sender, EventArgs e)
        {
            double temp;
            double.TryParse(tbMin.Text, out temp);
            using (var form = new FormNumeric(0, 5000, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbMin.Text = form.ReturnValue.ToString("N0");
                }
            }
        }

        private void tbPressure_Enter(object sender, EventArgs e)
        {
            double temp;
            double.TryParse(tbPressure.Text, out temp);
            using (var form = new FormNumeric(0, 1000, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbPressure.Text = form.ReturnValue.ToString("N0");
                }
            }
        }

        private void tbSlope_Enter(object sender, EventArgs e)
        {
            double temp;
            double.TryParse(tbSlope.Text, out temp);
            using (var form = new FormNumeric(-100, 100, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbSlope.Text = form.ReturnValue.ToString("N1");
                }
            }
        }

        private void tbSlope_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lbRaw.Text = mf.ModulesStatus.Pressure(cbModules.SelectedIndex).ToString("N0");
        }

        private void UpdateForm()
        {
            Initializing = true;
            LoadData();
            SetModuleIndicator();
            Initializing = false;
        }
    }
}