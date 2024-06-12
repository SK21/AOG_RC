using AgOpenGPS;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmSections : Form
    {
        private bool FormEdited;
        private bool Initializing;
        private double LastValue;
        private FormStart mf;
        private byte SecCount;
        private bool SectionCountChanged = false;
        private int SectionsPerZone = 10;
        private bool SectionsPerZoneChanged = false;
        private bool UseZones = false;

        public frmSections(FormStart CalledFrom)
        {
            Initializing = true;
            InitializeComponent();

            #region // language

            DGV.Columns[0].HeaderText = Lang.lgSection;
            DGV.Columns[1].HeaderText = Lang.lgWidth;
            DGV.Columns[2].HeaderText = Lang.lgSwitch;
            lbNumZones.Text = Lang.lgNumSections;

            lbWidth.Text = Lang.lgWidth;
            btnEqual.Text = Lang.lgEqual;
            this.Text = Lang.lgSection;

            DGV2.Columns[0].HeaderText = Lang.lgZone;
            DGV2.Columns[1].HeaderText = Lang.lgStart;
            DGV2.Columns[2].HeaderText = Lang.lgEnd;
            DGV2.Columns[3].HeaderText = Lang.lgWidth;
            DGV2.Columns[4].HeaderText = Lang.lgSwitch;
            lbPerZone.Text = Lang.lgPerZone;

            #endregion // language

            mf = CalledFrom;
            SetDayMode();
        }

        private void bntOK_Click(object sender, EventArgs e)
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
                        mf.Sections.CheckSwitchDefinitions();

                        UpdateForm();
                        SetButtons(false);
                    }
                }
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp(ex.Message, this.Text, 3000, true);
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

        private void btnEqual_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Copy the width of section 1 to the other sections.";

            mf.Tls.ShowHelp(Message, "Copy");
            hlpevent.Handled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (UseZones)
                {
                    string val = DGV2.Rows[0].Cells[3].EditedFormattedValue.ToString();

                    for (int i = 0; i < DGV2.Rows.Count; i++)
                    {
                        if (mf.Zones.Item(i).Start > 0) DGV2.Rows[i].Cells[3].Value = Convert.ToDouble(val);
                    }
                }
                else
                {
                    string val = DGV.Rows[0].Cells[1].EditedFormattedValue.ToString();

                    for (int i = 0; i < DGV.Rows.Count; i++)
                    {
                        DGV.Rows[i].Cells[1].Value = Convert.ToDouble(val);
                    }
                }
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp(ex.Message, this.Text, 3000, true);
            }
        }

        private void ckZones_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing)
            {
                UseZones = ckZones.Checked;
                mf.UseZones= UseZones;
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
            if (e.ColumnIndex == 0 )
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
            if(e.ColumnIndex==0||e.ColumnIndex==1)
            {
                e.CellStyle.BackColor = this.BackColor;
            }
        }

        private void DGV2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!Initializing) SetButtons(true);
        }

        private void frmSections_FormClosed(object sender, FormClosedEventArgs e)
        {
                mf.Tls.SaveFormData(this);
        }

        private void frmSections_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);

            DGV.BackgroundColor = DGV.DefaultCellStyle.BackColor;
            DGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DGV2.BackgroundColor = DGV.DefaultCellStyle.BackColor;
            DGV2.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            UseZones = mf.UseZones;
            int.TryParse(mf.Tls.LoadProperty("SectionsPerZone"), out SectionsPerZone);
            if (SectionsPerZone < 1) SectionsPerZone = 1;

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

                        if (mf.UseInches)
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
                mf.Tls.WriteErrorLog("FormSections/LoadDGV: " + ex.Message);
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
                        if (mf.UseInches)
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
                mf.Tls.WriteErrorLog("FormSections/LoadDGV2: " + ex.Message);
            }
        }

        private void Save()
        {
            // save changes
            SetSectionCount();
            if (UseZones)
            {
                int.TryParse(tbSectionsPerZone.Text, out SectionsPerZone);
                mf.Tls.SaveProperty("SectionsPerZone", SectionsPerZone.ToString());

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
                                if (mf.UseInches)
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
                mf.Tls.ShowHelp(ex.Message, this.Text, 3000, true);
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
                                    if (mf.UseInches)
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
                mf.Tls.ShowHelp(ex.Message, this.Text, 3000, true);
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
                    bntOK.Image = Properties.Resources.Save;
                    btnEqual.Enabled = false;
                    ckZones.Enabled = false;
                }
                else
                {
                    btnCancel.Enabled = false;
                    bntOK.Image = Properties.Resources.OK;
                    btnEqual.Enabled = true;
                    ckZones.Enabled = true;
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
            }
            else
            {
                this.BackColor = Properties.Settings.Default.NightColour;

                foreach (Control c in this.Controls)
                {
                    c.ForeColor = Color.White;
                }
            }
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

        private void tbSectionCount_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbSectionCount.Text, out tempD);
            using (var form = new FormNumeric(1, mf.MaxSections, tempD))
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
                if (tempD < 1 || tempD > mf.MaxSections)
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
                mf.Tls.ShowHelp(ex.Message, this.Text, 3000, true);
            }
        }

        private void tbSectionsPerZone_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbSectionsPerZone.Text, out tempD);
            using (var form = new FormNumeric(1, mf.MaxSections, tempD))
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
                if (tempD < 1 || tempD > mf.MaxSections)
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
                mf.Tls.ShowHelp(ex.Message, this.Text, 3000, true);
            }
        }

        private void UpdateForm()
        {
            int offset = 135;
            Initializing = true;

            DGV.Visible = !UseZones;
            DGV2.Visible = UseZones;
            if (UseZones)
            {
                LoadDGV2();
                ckZones.Image = Properties.Resources.SectionsWithZones;
                lbNumZones.Left = 15;
                tbSectionCount.Left = 212;
                lbWidth.Left = 15;
                lbFeet.Left = 212;
            }
            else
            {
                LoadDGV();
                ckZones.Image = Properties.Resources.SectionsNoZones2;
                lbNumZones.Left = 15 + offset;
                tbSectionCount.Left = 212 + offset;
                lbWidth.Left = 15 + offset;
                lbFeet.Left = 212 + offset;
            }

            lbPerZone.Visible = UseZones;
            tbSectionsPerZone.Visible = UseZones;

            SecCount = (byte)mf.Sections.Count;
            tbSectionCount.Text = SecCount.ToString("N0");
            ckZones.Checked = UseZones;
            UpdateTotalWidth();
            tbSectionsPerZone.Text = SectionsPerZone.ToString("N0");

            Initializing = false;
        }

        private void UpdateTotalWidth()
        {
            if (mf.UseInches)
            {
                lbWidth.Text = Lang.lgWidth + ":  " + (mf.Sections.TotalWidth(true) * 12).ToString("N0") + " Inches";
                lbFeet.Text = (mf.Sections.TotalWidth(true)).ToString("N1") + "  FT";
                lbFeet.Visible = true;
            }
            else
            {
                lbWidth.Text = Lang.lgWidth + ":  " + (mf.Sections.TotalWidth(false) * 100).ToString("N0") + " cm";
                lbFeet.Visible = false;
            }
        }
    }
}