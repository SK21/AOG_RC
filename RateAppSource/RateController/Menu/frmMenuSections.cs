using AgOpenGPS;
using RateController.Classes;
using RateController.Language;
using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuSections : Form
    {
        private bool cEdited;
        private double DefaultWidth;
        private bool Initializing = false;
        private double LastValue;
        private frmMenu MainMenu;
        private FormStart mf;
        private byte SecCount;
        private bool SectionCountChanged = false;
        private int SectionsPerZone = 10;
        private bool SectionsPerZoneChanged = false;
        private bool UseZones = false;

        public frmMenuSections(FormStart main, frmMenu menu)
        {
            Initializing = true;
            InitializeComponent();
            MainMenu = menu;
            mf = main;
            this.Tag = false;
        }

        private void bntOK_Click(object sender, EventArgs e)
        {
            try
            {
                // save changes
                SetSectionCount();
                if (UseZones)
                {
                    int.TryParse(tbSectionsPerZone.Text, out SectionsPerZone);
                    Props.SetProp("SectionsPerZone", SectionsPerZone.ToString());

                    if (SectionCountChanged || SectionsPerZoneChanged) mf.Zones.Build(SectionsPerZone);
                    SectionCountChanged = false;
                    SectionsPerZoneChanged = false;

                    SaveDGV2();
                    mf.Zones.Save();
                }
                else
                {
                    SaveDGV();
                    mf.Sections.Save();
                }

                if (double.TryParse(tbDefaultWidth.Text, out double dw))
                {
                    Props.SetProp("SectionDefaultWidth", dw.ToString());
                    DefaultWidth = dw;
                }

                mf.Sections.CheckSwitchDefinitions();
                SetButtons(false);
                UpdateForm();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuSections/btnOk_Click: " + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (UseZones) mf.Zones.Load();

            SectionCountChanged = false;
            SectionsPerZoneChanged = false;

            UpdateForm();
            SetButtons(false);
        }

        private void btnEqual_Click(object sender, EventArgs e)
        {
            double CurrentDefaultWidth = 300;
            if (double.TryParse(tbDefaultWidth.Text, out double wd)) CurrentDefaultWidth = wd;
            try
            {
                if (UseZones)
                {
                    for (int i = 0; i < DGV2.Rows.Count; i++)
                    {
                        if (mf.Zones.Item(i).Start > 0)
                        {
                            DGV2.Rows[i].Cells[3].Value = CurrentDefaultWidth;
                            DGV2.Rows[i].Cells[4].Value = i + 1;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < DGV.Rows.Count; i++)
                    {
                        DGV.Rows[i].Cells[1].Value = CurrentDefaultWidth;
                        DGV.Rows[i].Cells[2].Value = i + 1;
                    }
                }
                UpdateDisplayWidth();
            }
            catch (Exception ex)
            {
                mf.Tls.ShowMessage(ex.Message, this.Text, 3000, true);
            }
        }

        private void ckZones_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing)
            {
                UseZones = ckZones.Checked;
                Props.UseZones = UseZones;
                UpdateForm();
            }
        }

        private void DGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                double tempD;
                string val = DGV.Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue.ToString();
                switch (e.ColumnIndex)
                {
                    case 1:
                        // width
                        double.TryParse(val, out tempD);
                        if (tempD == 0) tempD = LastValue;
                        using (var form = new FormNumeric(0, 10000, tempD))
                        {
                            var result = form.ShowDialog();
                            if (result == DialogResult.OK)
                            {
                                DGV.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = form.ReturnValue;
                                LastValue = form.ReturnValue;
                            }
                        }
                        break;

                    case 2:
                        // switch
                        double.TryParse(val, out tempD);
                        using (var form = new FormNumeric(1, 16, tempD))
                        {
                            var result = form.ShowDialog();
                            if (result == DialogResult.OK)
                            {
                                DGV.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = form.ReturnValue;
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
            catch (Exception)
            {
            }
        }

        private void DGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 1 || e.ColumnIndex == 2)
            {
                if (e.Value.ToString() == "0")
                {
                    e.Value = "";
                    e.FormattingApplied = true;
                }
            }
            if (e.ColumnIndex == 0)
            {
                e.CellStyle.BackColor = this.BackColor;
            }
        }

        private void DGV_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!Initializing) SetButtons(true);
        }

        private void DGV2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                double tempD;
                string val = DGV2.Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue.ToString();
                switch (e.ColumnIndex)
                {
                    case 2:
                        // end section
                        double.TryParse(val, out tempD);
                        using (var form = new FormNumeric(1, mf.Sections.Count, tempD))
                        {
                            var result = form.ShowDialog();
                            if (result == DialogResult.OK)
                            {
                                DGV2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = form.ReturnValue;
                                string zz = DGV2.Rows[e.RowIndex].Cells[0].EditedFormattedValue.ToString();
                                int.TryParse(zz, out int Zone);
                                mf.Zones.Update(Zone - 1, (int)form.ReturnValue, SectionsPerZone);
                                LoadDGV2();
                            }
                        }
                        break;

                    case 3:
                        // width
                        double.TryParse(val, out tempD);
                        if (tempD == 0) tempD = LastValue;
                        using (var form = new FormNumeric(0, 10000, tempD))
                        {
                            var result = form.ShowDialog();
                            if (result == DialogResult.OK)
                            {
                                DGV2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = form.ReturnValue;
                                LastValue = form.ReturnValue;
                            }
                        }
                        break;

                    case 4:
                        // switch
                        double.TryParse(val, out tempD);
                        using (var form = new FormNumeric(1, 16, tempD))
                        {
                            var result = form.ShowDialog();
                            if (result == DialogResult.OK)
                            {
                                DGV2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = form.ReturnValue;
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
            catch (Exception)
            {
            }
        }

        private void DGV2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value.ToString() == "0")
            {
                e.Value = "";
                e.FormattingApplied = true;
            }
            if (e.ColumnIndex == 0 || e.ColumnIndex == 1)
            {
                e.CellStyle.BackColor = this.BackColor;
            }
        }

        private void DGV2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!Initializing) SetButtons(true);
        }

        private void frmMenuSections_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
        }

        private void frmMenuSections_Load(object sender, EventArgs e)
        {
            // menu 800,600
            // sub menu 540,630
            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            this.BackColor = Properties.Settings.Default.MainBackColour;
            this.Width = MainMenu.Width - 260;
            this.Height = MainMenu.Height - 50;
            btnOK.Left = this.Width - 84;
            btnOK.Top = this.Height - 84;
            btnCancel.Left = btnOK.Left - 78;
            btnCancel.Top = btnOK.Top;
            btnEqual.Left = btnCancel.Left - 78;
            btnEqual.Top = btnCancel.Top;
            ckZones.Left = btnEqual.Left - 122;
            ckZones.Top = btnCancel.Top - 25;
            MainMenu.StyleControls(this);
            PositionForm();

            DGV.BackgroundColor = DGV.DefaultCellStyle.BackColor;
            DGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DGV2.BackgroundColor = DGV.DefaultCellStyle.BackColor;
            DGV2.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            UseZones = Props.UseZones;
            int.TryParse(Props.GetProp("SectionsPerZone"), out SectionsPerZone);
            if (SectionsPerZone < 1) SectionsPerZone = 1;
            DefaultWidth = 300;
            if (Double.TryParse(Props.GetProp("SectionDefaultWidth"), out double dw)) DefaultWidth = dw;
            UpdateForm();
        }

        private void LoadDGV()
        {
            try
            {
                dataSet1.Clear();
                foreach (clsSection Sec in mf.Sections.Items)
                {
                    if (Sec.Enabled)
                    {
                        DataRow Rw = dataSet1.Tables[0].NewRow();
                        Rw[0] = Sec.ID + 1;

                        if (!Props.UseMetric)
                        {
                            Rw[1] = Sec.Width_inches;
                        }
                        else
                        {
                            Rw[1] = Sec.Width_cm;
                        }

                        Rw[2] = Sec.SwitchID + 1;

                        dataSet1.Tables[0].Rows.Add(Rw);
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("FormSections/LoadDGV: " + ex.Message);
            }
        }

        private void LoadDGV2()
        {
            try
            {
                dataTable2.Clear();
                foreach (clsZone Zn in mf.Zones.Items)
                {
                    DataRow Rw = dataSet2.Tables[0].NewRow();
                    Rw[0] = Zn.ID + 1;
                    Rw[1] = Zn.Start;
                    Rw[2] = Zn.End;
                    if (Zn.Start > 0)
                    {
                        if (!Props.UseMetric)
                        {
                            Rw[3] = Zn.Width_inches;
                        }
                        else
                        {
                            Rw[3] = Zn.Width_cm;
                        }
                        Rw[4] = Zn.SwitchID + 1;
                    }
                    else
                    {
                        Rw[3] = 0;
                        Rw[4] = 0;
                    }

                    dataSet2.Tables[0].Rows.Add(Rw);
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("FormSections/LoadDGV2: " + ex.Message);
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

        private void SaveDGV()
        {
            try
            {
                for (int i = 0; i < DGV.Rows.Count; i++)
                {
                    for (int j = 1; j < 3; j++)
                    {
                        string val = DGV.Rows[i].Cells[j].EditedFormattedValue.ToString();
                        if (val == "") val = "0";
                        switch (j)
                        {
                            case 1:
                                // width
                                if (!Props.UseMetric)
                                {
                                    mf.Sections.Item(i).Width_inches = (float)Convert.ToDouble(val);
                                }
                                else
                                {
                                    mf.Sections.Item(i).Width_cm = (float)Convert.ToDouble(val);
                                }
                                break;

                            case 2:
                                // switch
                                mf.Sections.Item(i).SwitchID = Convert.ToInt32(val) - 1;    // displayed as 1-16, saved as 0-15
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mf.Tls.ShowMessage(ex.Message, this.Text, 3000, true);
                LoadDGV();
            }
        }

        private void SaveDGV2()
        {
            try
            {
                for (int i = 0; i < DGV2.Rows.Count; i++)
                {
                    string St = DGV2.Rows[i].Cells[1].EditedFormattedValue.ToString();
                    int.TryParse(St, out int Start);
                    if (Start > 0)
                    {
                        for (int j = 1; j < 5; j++)
                        {
                            string val = DGV2.Rows[i].Cells[j].EditedFormattedValue.ToString();
                            if (val == "") val = "0";
                            switch (j)
                            {
                                case 3:
                                    // width
                                    if (!Props.UseMetric)
                                    {
                                        mf.Zones.Item(i).Width_inches = (float)Convert.ToDouble(val);
                                    }
                                    else
                                    {
                                        mf.Zones.Item(i).Width_cm = (float)Convert.ToDouble(val);
                                    }
                                    break;

                                case 4:
                                    // switch
                                    mf.Zones.Item(i).SwitchID = Convert.ToInt32(val) - 1;    // displayed as 1-16, saved as 0-15
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mf.Tls.ShowMessage(ex.Message, this.Text, 3000, true);
                LoadDGV2();
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
                    btnEqual.Enabled = false;
                    ckZones.Enabled = false;
                }
                else
                {
                    btnCancel.Enabled = false;
                    btnOK.Enabled = false;
                    btnEqual.Enabled = true;
                    ckZones.Enabled = true;
                }

                cEdited = Edited;
                this.Tag = cEdited;
            }
        }

        private void SetLanguage()
        {
            DGV.Columns[0].HeaderText = Lang.lgSection;
            DGV.Columns[1].HeaderText = Lang.lgWidth;
            DGV.Columns[2].HeaderText = Lang.lgSwitch;
            lbNumZones.Text = Lang.lgNumSections;

            lbWidth.Text = Lang.lgWidth;
            this.Text = Lang.lgSection;

            DGV2.Columns[0].HeaderText = Lang.lgZone;
            DGV2.Columns[1].HeaderText = Lang.lgStart;
            DGV2.Columns[2].HeaderText = Lang.lgEnd;
            DGV2.Columns[3].HeaderText = Lang.lgWidth;
            DGV2.Columns[4].HeaderText = Lang.lgSwitch;
            lbPerZone.Text = Lang.lgPerZone;
        }

        private void SetSectionCount()
        {
            int tmp = 0;
            if (int.TryParse(tbSectionCount.Text, out tmp)) mf.Sections.Count = tmp;
            for (int i = tmp; i < DGV.Rows.Count; i++)
            {
                DGV.Rows[i].Cells[1].Value = 0;
            }
        }

        private void tbDefaultWidth_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbDefaultWidth.Text, out tempD);
            using (var form = new FormNumeric(1, 1000, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbDefaultWidth.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void tbDefaultWidth_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                double tempD;
                double.TryParse(tbDefaultWidth.Text, out tempD);
                if (tempD < 1 || tempD > 1000)
                {
                    System.Media.SystemSounds.Exclamation.Play();
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                mf.Tls.ShowMessage(ex.Message, this.Text, 3000, true);
            }
        }

        private void tbSectionCount_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbSectionCount.Text, out tempD);
            using (var form = new FormNumeric(1, Props.MaxSections, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbSectionCount.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void tbSectionCount_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbSectionCount_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                double tempD;
                double.TryParse(tbSectionCount.Text, out tempD);
                if (tempD < 1 || tempD > Props.MaxSections)
                {
                    System.Media.SystemSounds.Exclamation.Play();
                    e.Cancel = true;
                }
                else
                {
                    SectionCountChanged = true;
                }
            }
            catch (Exception ex)
            {
                mf.Tls.ShowMessage(ex.Message, this.Text, 3000, true);
            }
        }

        private void tbSectionsPerZone_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbSectionsPerZone.Text, out tempD);
            using (var form = new FormNumeric(1, Props.MaxSections, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbSectionsPerZone.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void tbSectionsPerZone_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbSectionsPerZone_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                double tempD;
                double.TryParse(tbSectionsPerZone.Text, out tempD);
                if (tempD < 1 || tempD > Props.MaxSections)
                {
                    System.Media.SystemSounds.Exclamation.Play();
                    e.Cancel = true;
                }
                else
                {
                    SectionsPerZoneChanged = true;
                }
            }
            catch (Exception ex)
            {
                mf.Tls.ShowMessage(ex.Message, this.Text, 3000, true);
            }
        }

        private void UpdateDisplayWidth()
        {
            double DisplayWidth = 0;
            if (UseZones)
            {
                for (int i = 0; i < DGV2.Rows.Count; i++)
                {
                    if (mf.Zones.Item(i).Start > 0)
                    {
                        string val = DGV2.Rows[i].Cells[3].EditedFormattedValue.ToString();
                        DisplayWidth += Convert.ToDouble(val);
                    }
                }
            }
            else
            {
                for (int i = 0; i < DGV.Rows.Count; i++)
                {
                    string val = DGV.Rows[i].Cells[1].EditedFormattedValue.ToString();
                    DisplayWidth += Convert.ToDouble(val);
                }
            }

            if (!Props.UseMetric)
            {
                lbWidth.Text = Lang.lgWidth + ":  " + ((DisplayWidth).ToString("N0")) + " Inches";
                lbFeet.Text = (DisplayWidth / 12.0).ToString("N1") + "  FT";
                lbFeet.Visible = true;
            }
            else
            {
                lbWidth.Text = Lang.lgWidth + ":  " + (DisplayWidth * 100).ToString("N0") + " cm";
                lbFeet.Visible = false;
            }
        }

        private void UpdateForm()
        {
            Initializing = true;

            DGV.Visible = !UseZones;
            DGV2.Visible = UseZones;
            if (UseZones)
            {
                LoadDGV2();
                ckZones.Image = Properties.Resources.SectionsWithZones;
            }
            else
            {
                LoadDGV();
                ckZones.Image = Properties.Resources.SectionsNoZones2;
            }

            lbPerZone.Visible = UseZones;
            tbSectionsPerZone.Visible = UseZones;

            SecCount = (byte)mf.Sections.Count;
            tbSectionCount.Text = SecCount.ToString("N0");
            ckZones.Checked = UseZones;
            UpdateTotalWidth();
            tbSectionsPerZone.Text = SectionsPerZone.ToString("N0");
            tbDefaultWidth.Text = DefaultWidth.ToString("N0");
            Initializing = false;
        }

        private void UpdateTotalWidth()
        {
            if (!Props.UseMetric)
            {
                lbWidth.Text = Lang.lgWidth + ":  " + (mf.Sections.TotalWidth(true) * 12.0).ToString("N0") + " Inches";
                lbFeet.Text = (mf.Sections.TotalWidth(true)).ToString("N1") + "  FT";
                lbFeet.Visible = true;
                lbUnits.Text = "Inches";
            }
            else
            {
                lbWidth.Text = Lang.lgWidth + ":  " + (mf.Sections.TotalWidth(false) * 100).ToString("N0") + " cm";
                lbFeet.Visible = false;
                lbUnits.Text = "cm";
            }
        }
    }
}