using AgOpenGPS;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmRelays : Form
    {
        private bool Initializing;
        private FormStart mf;

        public frmRelays(FormStart CalledFrom)
        {
            Initializing = true;
            InitializeComponent();

            #region // language
            btnCancel.Text = Lang.lgCancel;
            bntOK.Text = Lang.lgClose;

            DGV.Columns[0].HeaderText = Lang.lgRelay;
            DGV.Columns[1].HeaderText = Lang.lgType;
            DGV.Columns[2].HeaderText = Lang.lgSectionNum;

            this.Text = Lang.lgRelays;
            btnLoadDefaults.Text = Lang.lgLoad_Defaults;
            #endregion // language

            mf = CalledFrom;
        }

        private void bntOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (bntOK.Text == Lang.lgClose)
                {
                    this.Close();
                }
                else
                {
                    SaveData();
                    UpdateForm();
                    SetButtons(false);
                }
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp(ex.Message, this.Text, 3000, true);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
        }

        private void btnLoadDefaults_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow Rw in DGV.Rows)
            {
                Rw.Cells[1].Value = RelayTypes.Section;
                Rw.Cells[2].Value = Rw.Cells[0].Value;
            }
        }

        private void DGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                double Temp;
                string val = DGV.Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue.ToString();
                switch (e.ColumnIndex)
                {
                    case 1:
                        // type, make combobox drop down
                        //SendKeys.Send("{F4}");
                        break;

                    case 2:
                        // section
                        double.TryParse(val, out Temp);
                        using (var form = new FormNumeric(1, 16, Temp))
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
                mf.Tls.WriteErrorLog("frmRelays/CellClick: " + ex.Message);
            }
        }

        private void DGV_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!Initializing) SetButtons(true);
        }

        private void DGV_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            mf.Tls.WriteErrorLog("frmRelays/DGV_DataError: " + e.ToString());
        }

        private void DGV_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Relay Type:\n" +
                "Section - relay controlled by section switch\n" +
                "Slave - relay is on when any section relay is on\n" +
                "        and off when all sections relays are off\n" +
                "Master - relay is on when any section relay is\n" +
                "          on and turns off before section relays \n" +
                "           turn off\n Power - on all the time\n" +
                "Invert_Section - relay is on when section is off\n" +
                "\n" +
                "Section #:\n" +
                "    - the section that controls the relay";

            mf.Tls.ShowHelp(Message, "Relays", 60000);
            hlpevent.Handled = true;
        }

        private void frmRelays_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                mf.Tls.SaveFormData(this);
            }
        }

        private void frmRelays_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);

            DGV.BackgroundColor = DGV.DefaultCellStyle.BackColor;
            DGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            DataGridViewComboBoxColumn col = (DataGridViewComboBoxColumn)DGV.Columns[1];
            col.Name = "Type";
            col.DataSource = Enum.GetValues(typeof(RelayTypes));
            col.ValueType = typeof(RelayTypes);

            UpdateForm();
        }

        private void LoadData()
        {
            try
            {
                dataSet1.Clear();
                foreach (clsRelay Rly in mf.RelayObjects.Items)
                {
                    DataRow Rw = dataSet1.Tables[0].NewRow();
                    Rw[0] = Rly.ID + 1;
                    Rw[1] = Rly.Type;

                    if (Rly.Type == RelayTypes.Section || Rly.Type == RelayTypes.Invert_Section)
                    {
                        Rw[2] = Rly.SectionID + 1;
                    }
                    else
                    {
                        Rw[2] = "";
                    }

                    dataSet1.Tables[0].Rows.Add(Rw);
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmRelays/LoadData: " + ex.Message);
            }
        }

        private void SaveData()
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
                                // Type
                                RelayTypes tmp;
                                if (Enum.TryParse(val, true, out tmp)) mf.RelayObjects.Item(i).Type = tmp;
                                break;

                            case 2:
                                // section
                                mf.RelayObjects.Item(i).SectionID = Convert.ToInt32(val) - 1;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmRelays/SaveData: " + ex.Message);
            }
            mf.RelayObjects.Save();
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
            else
            {
                this.BackColor = Properties.Settings.Default.NightColour;

                foreach (Control c in this.Controls)
                {
                    c.ForeColor = Color.White;
                }
            }
        }

        private void UpdateForm()
        {
            Initializing = true;
            LoadData();
            SetDayMode();
            Initializing = false;
        }
    }
}