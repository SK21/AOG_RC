using AgOpenGPS;
using RateController.Classes;
using RateController.Language;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuPins : Form
    {
        private System.Windows.Forms.TextBox[] Boxes;
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;

        public frmMenuPins(frmMenu menu)
        {
            // guarded from construction (frmMenuRelays pattern) - the Designer wires
            // DGV.CellValueChanged before its column setup finishes, which fires the
            // edit handler inside InitializeComponent
            Initializing = true;
            InitializeComponent();
            MainMenu = menu;
            this.Tag = false;

            Boxes = new System.Windows.Forms.TextBox[] { tbWrk, tbPressure };

            for (int i = 0; i < Boxes.Length; i++)
            {
                Boxes[i].Enter += Boxes_Enter;
                Boxes[i].TextChanged += Boxes_TextChanged;
            }
        }

        private void Boxes_Enter(object sender, EventArgs e)
        {
            var bx = (System.Windows.Forms.TextBox)sender;
            double temp = 0;
            if (double.TryParse(bx.Text.Trim(), out double vl)) temp = vl;

            using (var form = new FormNumeric(0, 50, temp))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (form.IsBlank)
                    {
                        bx.Text = "-";          // sentinel for “no pin”
                    }
                    else
                    {
                        bx.Text = form.ReturnValue.ToString("N0");
                    }
                }
            }
        }

        private void Boxes_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
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
                if (DuplicatePins())
                {
                    Props.ShowMessage("Duplicate pin numbers.", "Pins", 10000);
                }
                else
                {
                    byte val;
                    Core.ModuleConfig.Momentary = ckMomentary.Checked;

                    // sensor pins from the grid
                    for (int i = 0; i < DGV.Rows.Count; i++)
                    {
                        Core.SensorPins.SetPins(i, CellPin(i, 1), CellPin(i, 2), CellPin(i, 3), CellPin(i, 4), CellChecked(i));
                    }

                    // mirror sensors 0/1 into the module config for older firmware
                    Core.ModuleConfig.Sensor0Flow = Core.SensorPins.FlowPin(0);
                    Core.ModuleConfig.Sensor0Dir = Core.SensorPins.DirPin(0);
                    Core.ModuleConfig.Sensor0PWM = Core.SensorPins.PWMPin(0);
                    Core.ModuleConfig.Sensor1Flow = Core.SensorPins.FlowPin(1);
                    Core.ModuleConfig.Sensor1Dir = Core.SensorPins.DirPin(1);
                    Core.ModuleConfig.Sensor1PWM = Core.SensorPins.PWMPin(1);

                    // Work Pin
                    if (byte.TryParse(tbWrk.Text, out val))
                    {
                        Core.ModuleConfig.WorkPin = val;
                    }
                    else
                    {
                        Core.ModuleConfig.WorkPin = 255;
                    }

                    // Pressure
                    if (byte.TryParse(tbPressure.Text, out val))
                    {
                        Core.ModuleConfig.PressurePin = val;
                    }
                    else
                    {
                        Core.ModuleConfig.PressurePin = 255;
                    }

                    Core.SensorPins.Save();
                    Core.ModuleConfig.Save();

                    SetButtons(false);
                    UpdateForm();
                    MainMenu.HighlightUpdateButton();
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuPins/btnOk_Click: " + ex.Message);
            }
        }

        private void btnRescan_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < DGV.Rows.Count; i++)
            {
                DGV.Rows[i].Cells[1].Value = "-";
                DGV.Rows[i].Cells[2].Value = "-";
                DGV.Rows[i].Cells[3].Value = "-";
                DGV.Rows[i].Cells[4].Value = "-";
                DGV.Rows[i].Cells[5].Value = false;
            }
            tbWrk.Text = "-";
            tbPressure.Text = "-";
            ckMomentary.Checked = false;
        }

        private byte CellPin(int Row, int Col)
        {
            byte Result = 255;
            if (byte.TryParse(DGV.Rows[Row].Cells[Col].EditedFormattedValue.ToString(), out byte val))
            {
                Result = val;
            }
            return Result;
        }

        private bool CellChecked(int Row)
        {
            bool Result = false;
            object vl = DGV.Rows[Row].Cells[5].EditedFormattedValue;
            if (vl != null) bool.TryParse(vl.ToString(), out Result);
            return Result;
        }

        private void DGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // pin columns open the numeric pad; Sensor (0) is read-only and
                // Invert (5) toggles natively
                if (e.RowIndex >= 0 && e.ColumnIndex >= 1 && e.ColumnIndex <= 4)
                {
                    string val = DGV.Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue.ToString();
                    double.TryParse(val, out double temp);

                    using (var form = new FormNumeric(0, 50, temp))
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            if (form.IsBlank)
                            {
                                DGV.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "-";
                            }
                            else
                            {
                                DGV.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = form.ReturnValue.ToString("N0");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuPins/DGV_CellClick: " + ex.Message);
            }
        }

        private void DGV_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            SetButtons(true);
        }

        private void DGV_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (DGV.IsCurrentCellDirty)
            {
                DGV.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void DGV_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            Props.WriteErrorLog("frmMenuPins/DGV_DataError: Row,Column: " + e.RowIndex.ToString() + ", " + e.ColumnIndex.ToString()
                + " " + e.Exception.Message);
        }

        private bool DuplicatePins()
        {
            bool Result = false;
            List<byte> Used = new List<byte>();

            for (int i = 0; i < DGV.Rows.Count; i++)
            {
                for (int j = 1; j <= 4; j++)
                {
                    byte pin = CellPin(i, j);
                    if (pin != 255)
                    {
                        if (Used.Contains(pin))
                        {
                            Result = true;
                        }
                        else
                        {
                            Used.Add(pin);
                        }
                    }
                }
            }

            if (byte.TryParse(tbWrk.Text, out byte wrk) && wrk != 255)
            {
                if (Used.Contains(wrk))
                {
                    Result = true;
                }
                else
                {
                    Used.Add(wrk);
                }
            }

            if (byte.TryParse(tbPressure.Text, out byte prs) && prs != 255)
            {
                if (Used.Contains(prs)) Result = true;
            }
            return Result;
        }

        private void frmMenuPins_FormClosed(object sender, FormClosedEventArgs e)
        {
            // unsubscribe so a disposed instance isn't called when the Boards page
            // fires ModuleDefaultsSet (its DGV has no columns once disposed)
            MainMenu.MenuMoved -= MainMenu_MenuMoved;
            MainMenu.ModuleDefaultsSet -= MainMenu_ModuleDefaultsSet;
            Props.SaveFormLocation(this);
        }

        private void frmMenuPins_Load(object sender, EventArgs e)
        {
            SubMenuLayout.SetFormLayout(this, MainMenu, null);
            btnOK.Left = this.Width - 84;
            btnOK.Top = this.Height - 84;
            btnCancel.Left = btnOK.Left - 78;
            btnCancel.Top = btnOK.Top;
            MainMenu.StyleControls(this);
            lbModule.Font = new Font(lbModule.Font, FontStyle.Underline);

            DGV.BackgroundColor = DGV.DefaultCellStyle.BackColor;
            DGV.ColumnHeadersDefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            DGV.CurrentCellDirtyStateChanged += DGV_CurrentCellDirtyStateChanged;

            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            MainMenu.ModuleDefaultsSet += MainMenu_ModuleDefaultsSet;
            this.BackColor = Properties.Settings.Default.MainBackColour;
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

        private string PinText(byte Pin)
        {
            string Result = "-";
            if (Pin <= 60) Result = Pin.ToString();
            return Result;
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
            lbWorkPin.Text = Lang.lgWorkPin;
            ckMomentary.Text = Lang.lgMomentary;
            lbPressure.Text = Lang.lgPressurePin;
        }

        private void UpdateForm()
        {
            Initializing = true;
            byte[] data = Core.ModuleConfig.GetData();

            lbModule.Text = Core.ModuleConfigDescription();

            // one grid row per sensor, capped by the target board type/firmware
            int Count = data[3];
            int Max = Core.MaxSensorsForModule(data[2]);
            if (Count < 1) Count = 1;
            if (Count > Max) Count = Max;

            DGV.Rows.Clear();
            for (int i = 0; i < Count; i++)
            {
                DGV.Rows.Add(
                    (i + 1).ToString(),
                    PinText(Core.SensorPins.FlowPin(i)),
                    PinText(Core.SensorPins.DirPin(i)),
                    PinText(Core.SensorPins.PWMPin(i)),
                    PinText(Core.SensorPins.BinPin(i)),
                    Core.SensorPins.InvertBin(i));
            }

            ckMomentary.Checked = Core.ModuleConfig.Momentary;

            // work pin
            if (data[29] > 60)
            {
                tbWrk.Text = "-";
            }
            else
            {
                tbWrk.Text = data[29].ToString();
            }

            // pressure pin
            if (data[30] > 60)
            {
                tbPressure.Text = "-";
            }
            else
            {
                tbPressure.Text = data[30].ToString();
            }

            Initializing = false;
        }
    }
}
