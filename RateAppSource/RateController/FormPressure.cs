using AgOpenGPS;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;

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
                    mf.Tls.SaveProperty("ShowPressure", ckShowPressure.Checked.ToString());
                    mf.Tls.SaveProperty("PressureID",tbPressureID.Text);

                    UpdateForm();
                    SetButtons(false);
                }
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp(ex.Message, "Pressure", 3000, true);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
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
                case 5:
                    // units/volt, offset
                    using (var form = new FormNumeric(0, 3000, 0))
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

            bool show;
            bool.TryParse(mf.Tls.LoadProperty("ShowPressure"), out show);
            mf.ShowPressure = show;
            ckShowPressure.Checked = show;

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
                    Rw["UnitsPerVolt"] = Pres.UnitsVolts;
                    Rw["Pressure"] = Pres.Pressure();
                    Rw["Offset"] = Pres.Offset;

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
                                // units/volts
                                mf.PressureObjects.Item(i).UnitsVolts = (float)Convert.ToDouble(val);
                                break;

                            case 5:
                                // offset
                                mf.PressureObjects.Item(i).Offset = (int)Convert.ToDouble(val);
                                break;
                        }
                    }
                }
                mf.PressureObjects.Save();
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("FormPressure/SaveGrid: " + ex.Message);
                mf.Tls.ShowHelp(ex.Message, "Pressure", 3000, true);
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
        }

        private void UpdateForm()
        {
            Initializing = true;

            LoadGrid();
            tbPressureID.Text = mf.Tls.LoadProperty("PressureID");

            Initializing = false;
        }

        private void ckShowPressure_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            LoadGrid();
        }

        private void tbPressureID_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbPressureID.Text, out tempD);
            using (var form = new FormNumeric(1, 16, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbPressureID.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void tbPressureID_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbPressureID_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            int tempInt;
            int.TryParse(tbPressureID.Text, out tempInt);
            if (tempInt < 1 || tempInt > 16)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }
    }
}