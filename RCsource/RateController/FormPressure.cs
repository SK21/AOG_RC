using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using AgOpenGPS;

namespace RateController
{
    public partial class FormPressure : Form
    {
        private bool Initializing;
        private FormStart mf;
        public FormPressure(FormStart CalledFrom)
        {
            Initializing = true;
            InitializeComponent();
            mf = CalledFrom;
            SetDayMode();
        }

        private void bntOK_Click(object sender, EventArgs e)
        {
            try
            {
                Button ButtonClicked = (Button)sender;
                if (ButtonClicked.Text == Lang.lgClose)
                {
                    this.Close();
                }
                else
                {
                    // save changes
                    SaveGrid();
                    mf.PressureObjects.UseAlarm = ckOffRate.Checked;
                    mf.PressureObjects.OffPressureSetting = (byte)mf.Tls.StringToInt(tbOffRate.Text);
                    
                    UpdateForm();
                    SetButtons(false);
                }
            }
            catch (Exception ex)
            {
                mf.Tls.TimedMessageBox(ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
        }

        private void ckOffRate_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);

            if (ckOffRate.Checked)
            {
                tbOffRate.Enabled = true;
            }
            else
            {
                tbOffRate.Enabled = false;
                tbOffRate.Text = "0";
            }
        }

        private void DGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            switch (e.ColumnIndex)
            {
                case 2:
                case 3:
                    // module, sensor
                    using (var form = new FormNumeric(0, 15, 0))
                    {
                        var result = form.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            DGV.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = form.ReturnValue;
                        }
                    }
                    break;

                case 4:
                    // section
                    using (var form = new FormNumeric(1, 16, 0))
                    {
                        var result = form.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            DGV.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = form.ReturnValue;
                        }
                    }
                    break;

                case 5:
                    // units/volt
                    using (var form = new FormNumeric(0, 1000, 0))
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

        private void DGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (IsBlankRow(e.RowIndex))
            {
                if (e.ColumnIndex > 1)
                {
                    // suppress 0's
                    if (e.Value.ToString() == "0")
                    {
                        e.Value = "";
                        e.FormattingApplied = true;
                    }
                }
            }
        }

        private void DGV_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!Initializing) SetButtons(true);
        }

        private void FormPressure_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                mf.Tls.SaveFormData(this);
            }
        }

        private void FormPressure_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);

            DGV.BackgroundColor = DGV.DefaultCellStyle.BackColor;
            DGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DGV.Columns[0].DefaultCellStyle.BackColor = Properties.Settings.Default.DayColour;
            DGV.Columns[6].DefaultCellStyle.BackColor = Properties.Settings.Default.DayColour;

            UpdateForm();
        }

        private bool IsBlankRow(int row)
        {
            string Des = DGV.Rows[row].Cells[1].Value.ToString();   // description
            string cal = DGV.Rows[row].Cells[5].Value.ToString();   // cal
            return (mf.Tls.StringToInt(cal) == 0 && Des == "");
        }
        private void LoadGrid()
        {
            try
            {
                dataSet1.Clear();
                foreach (clsPressure Pres in mf.PressureObjects.Items)
                {
                    DataRow Rw = dataSet1.Tables[0].NewRow();
                    Rw["ID"] = Pres.ID + 1;
                    Rw["Description"] = Pres.Description;
                    Rw["ModuleID"] = Pres.ModuleID;
                    Rw["SensorID"] = Pres.SensorID;
                    Rw["SectionID"] = Pres.SectionID + 1;
                    Rw["UnitsPerVolt"] = Pres.UnitsVolts;
                    Rw["Pressure"] = Pres.Pressure();
                    dataSet1.Tables[0].Rows.Add(Rw);
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("FormPressure/LoadGrid: " + ex.Message);
            }
        }

        private void SaveGrid()
        {
            try
            {
                for (int i = 0; i < DGV.Rows.Count; i++)
                {
                    for (int j = 1; j < 6; j++)
                    {
                        string val = DGV.Rows[i].Cells[j].EditedFormattedValue.ToString();
                        if (val == "") val = "0";
                        switch (j)
                        {
                            case 1:
                                // description
                                mf.PressureObjects.Item(i).Description = DGV.Rows[i].Cells[j].EditedFormattedValue.ToString();
                                break;

                            case 2:
                                // module ID
                                mf.PressureObjects.Item(i).ModuleID = mf.Tls.StringToInt(val);
                                break;

                            case 3:
                                // Sensor ID
                                mf.PressureObjects.Item(i).SensorID = mf.Tls.StringToInt(val);
                                break;

                            case 4:
                                // section
                                int sec = mf.Tls.StringToInt(val) - 1;
                                if (sec < 0) sec = 0;
                                mf.PressureObjects.Item(i).SectionID = sec;
                                break;
                            case 5:
                                // units/volts
                                mf.PressureObjects.Item(i).UnitsVolts = (float)Convert.ToDouble(val);
                                break;
                        }
                    }
                }
                mf.PressureObjects.Save();
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("FormPressure/SaveGrid: " + ex.Message);
                mf.Tls.TimedMessageBox(ex.Message);
            }
        }

        private void SetButtons(bool Edited)
        {
            if (!Initializing)
            {
                if (Edited)
                {
                    btnCancel.Enabled = true;
                    this.bntOK.Text = Lang.lgSave;
                }
                else
                {
                    btnCancel.Enabled = false;
                    this.bntOK.Text = Lang.lgClose;
                }
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
            //else
            //{
            //    this.BackColor = Properties.Settings.Default.NightColour;

            //    foreach (Control c in this.Controls)
            //    {
            //        c.ForeColor = Color.White;
            //    }
            //}
        }

        private void tbOffRate_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbOffRate.Text, out tempD);
            using (var form = new FormNumeric(0, 40, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbOffRate.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void tbOffRate_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbOffRate_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            int tempInt;
            int.TryParse(tbOffRate.Text, out tempInt);
            if (tempInt < 0 || tempInt > 40)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void UpdateForm()
        {
            Initializing = true;

            LoadGrid();
            ckOffRate.Checked = mf.PressureObjects.UseAlarm;
            tbOffRate.Text = mf.PressureObjects.OffPressureSetting.ToString();

            Initializing = false;
        }
    }
}