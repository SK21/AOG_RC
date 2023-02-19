using AgOpenGPS;
using PCBsetup.Languages;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PCBsetup.Forms
{
    public partial class frmPCBsettings : Form
    {
        public clsTextBoxes Boxes;
        public CheckBox[] CKs;
        public frmMain mf;
        private bool Initializing = false;
        private bool[] TabEdited;
        private bool FormEdited = false;

        public frmPCBsettings(frmMain CallingForm)
        {
            InitializeComponent();

            mf = CallingForm;

            CKs = new CheckBox[] {ckUseRate,ckOnBoard,ckRelayOn,ckFlowOn,ckSwapPitchRoll
                ,ckInvertRoll,ckGyro,ckActuator};

            for (int i = 0; i < CKs.Length; i++)
            {
                CKs[i].CheckedChanged += tb_TextChanged;
            }

            TabEdited = new bool[3];

            Boxes = new clsTextBoxes(mf);
            BuildBoxes();
        }

        private void bntOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (!FormEdited)
                {
                    bool Edited = false;
                    for (int i = 0; i < 3; i++)
                    {
                        if (TabEdited[i])
                        {
                            Edited = true;
                            break;
                        }
                    }
                    if (Edited) mf.Tls.ShowHelp("Changes have not been sent to the module.", "Warning", 3000);

                    this.Close();
                }
                else
                {
                    SaveSettings();
                    SetButtons(false);
                    UpdateForm();
                }
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp(ex.Message, this.Text, 3000, true);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                TabEdited[i] = false;
            }
            UpdateForm();
            SetButtons(false);
        }

        private void btnLoadDefaults_Click(object sender, EventArgs e)
        {
            // AS14 PCB
            cbReceiver.SelectedIndex = 1;
            tbNMEAserialPort.Text = "4";
            tbRTCMserialPort.Text = "5";
            tbRTCM.Text = "2233";
            cbIMU.SelectedIndex = 1;
            tbIMUdelay.Text = "90";
            tbIMUinterval.Text = "40";
            tbZeroOffset.Text = "6500";
            cbAnalog.SelectedIndex = 1;
            cbRelayControl.SelectedIndex = 0;
            tbIPaddress.Text = "1";
            tbWemosSerialPort.Text = "3";

            tbMinSpeed.Text = "1";
            tbMaxSpeed.Text = "15";
            tbPulseCal.Text = "25.5";
            tbRS485port.Text = "8";
            tbModule.Text = "0";

            ckGyro.Checked = false;
            ckUseRate.Checked = false;
            ckOnBoard.Checked = true;
            ckRelayOn.Checked = false;
            ckFlowOn.Checked = false;
            ckSwapPitchRoll.Checked = true;
            ckInvertRoll.Checked = false;
            ckActuator.Checked = false;

            tbDir1.Text = "26";
            tbPwm1.Text = "25";
            tbDir2.Text = "28";
            tbPwm2.Text = "29";
            tbSteerRelay.Text = "36";
            tbWorkSwitch.Text = "27";
            tbSteerSwitch.Text = "39";
            tbEncoder.Text = "38";
            tbCurrentSensor.Text = "10";
            tbWAS.Text = "25";
            tbPressureSensor.Text = "26";
            tbSpeedPulse.Text = "37";
            tbSendEnable.Text = "33";
        }

        private void btnSendToModule_Click(object sender, EventArgs e)
        {
            bool Sent;
            try
            {
                PGN32622 PGN = new PGN32622(this);
                Sent = PGN.Send();
                PGN32623 PGN2 = new PGN32623(this);
                Sent = Sent & PGN2.Send();
                PGN32624 PGN3 = new PGN32624(this);
                Sent = Sent & PGN3.Send();

                if (Sent)
                {
                    mf.Tls.ShowHelp("Sent to module.", this.Text, 3000);
                    for (int i = 0; i < 3; i++)
                    {
                        TabEdited[i] = false;
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                switch (ex.Message)
                {
                    case "ModuleDisconnected":
                        mf.Tls.ShowHelp("Module disconnected. Wait for connection and retry.", this.Text, 3000);
                        break;

                    case "CommDisconnected":
                        mf.Tls.ShowHelp("Comm port is not open.", this.Text, 3000);
                        break;

                    default:
                        mf.Tls.ShowHelp(ex.Message, this.Text, 3000, true);
                        break;
                }
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp(ex.Message, this.Text, 3000, true);
            }
        }

        private void BuildBoxes()
        {
            int StartID = Boxes.Add(this.Text, tbNMEAserialPort, 8, 1);
            Boxes.Add(this.Text, tbRTCMserialPort, 8, 1);
            Boxes.Add(this.Text, tbRTCM, 9999);
            Boxes.Add(this.Text, tbIMUdelay, 100);
            Boxes.Add(this.Text, tbIMUinterval, 100);
            Boxes.Add(this.Text, tbZeroOffset, 10000);
            Boxes.Add(this.Text, tbIPaddress, 254);
            Boxes.Add(this.Text, tbMinSpeed, 50);
            Boxes.Add(this.Text, tbMaxSpeed, 50);
            Boxes.Add(this.Text, tbPulseCal);
            Boxes.Add(this.Text, tbRS485port, 8, 1);
            Boxes.Add(this.Text, tbModule, 15);
            Boxes.Add(this.Text, tbWemosSerialPort, 8, 1);

            Boxes.Add(this.Text, tbDir1, 41);
            Boxes.Add(this.Text, tbPwm1, 41);
            Boxes.Add(this.Text, tbSteerSwitch, 41);
            Boxes.Add(this.Text, tbWAS, 41);
            Boxes.Add(this.Text, tbSteerRelay, 41);
            Boxes.Add(this.Text, tbWorkSwitch, 41);
            Boxes.Add(this.Text, tbCurrentSensor, 41);
            Boxes.Add(this.Text, tbPressureSensor, 41);
            Boxes.Add(this.Text, tbEncoder, 41);
            Boxes.Add(this.Text, tbDir2, 41);
            Boxes.Add(this.Text, tbPwm2, 41);
            Boxes.Add(this.Text, tbSpeedPulse, 41);
            int EndID = Boxes.Add(this.Text, tbSendEnable, 41);

            for (int i = StartID; i < EndID + 1; i++)
            {
                Boxes.Item(i).TB.Tag = Boxes.Item(i).ID;
                Boxes.Item(i).TB.Enter += tb_Enter;
                Boxes.Item(i).TB.TextChanged += tb_TextChanged;
                Boxes.Item(i).TB.Validating += tb_Validating;
            }
        }

        private void cbAnalog_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Select method to read analog signals." +
                " (WAS, etc.)";

            mf.Tls.ShowHelp(Message, "Analog Input");
            hlpevent.Handled = true;
        }

        private void cbAnalog_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void cbIMU_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void cbReceiver_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void ckActuator_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Use linear actuator to position steer motor." +
                " Position is read on Pressure pin. Actuator is controlled" +
                " with Motor2 Dir and Motor2 PWM pins.";

            mf.Tls.ShowHelp(Message, "Actuator");
            hlpevent.Handled = true;
        }

        private void ckFlowOn_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Which signal (high or low) increases the flow rate" +
                " for rate control.";

            mf.Tls.ShowHelp(Message, "Flow on high");
            hlpevent.Handled = true;
        }

        private void ckOnBoard_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Use On-board TB6612 motor driver to control motor 2." +
                " Max 1.2 amps.";

            mf.Tls.ShowHelp(Message, "Motor Driver");
            hlpevent.Handled = true;
        }

        private void ckRelayOn_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Which signal (high or low) turns on the relay.";

            mf.Tls.ShowHelp(Message, "Relay on high");
            hlpevent.Handled = true;
        }

        private void frmPCBsettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                mf.Tls.SaveFormData(this);
            }
        }

        private void frmPCBsettings_Load(object sender, EventArgs e)
        {
            try
            {
                mf.Tls.LoadFormData(this);

                this.BackColor = PCBsetup.Properties.Settings.Default.DayColour;

                for (int i = 0; i < tabControl1.TabCount; i++)
                {
                    tabControl1.TabPages[i].BackColor = PCBsetup.Properties.Settings.Default.DayColour;
                }

                UpdateForm();
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp(ex.Message, this.Text, 3000, true);
            }
        }

        private void LoadSettings()
        {
            try
            {
                byte tmp;
                bool Checked;

                // textboxes
                Boxes.ReLoad();

                // combo boxes
                byte.TryParse(mf.Tls.LoadProperty("GPSreceiver"), out tmp);
                cbReceiver.SelectedIndex = tmp;

                byte.TryParse(mf.Tls.LoadProperty("IMU"), out tmp);
                cbIMU.SelectedIndex = tmp;

                byte.TryParse(mf.Tls.LoadProperty("RelayControl"), out tmp);
                cbRelayControl.SelectedIndex = tmp;

                byte.TryParse(mf.Tls.LoadProperty("AnalogMethod"), out tmp);
                cbAnalog.SelectedIndex = tmp;

                // check boxes
                for (int i = 0; i < CKs.Length; i++)
                {
                    bool.TryParse(mf.Tls.LoadProperty(CKs[i].Name), out Checked);
                    CKs[i].Checked = Checked;
                }
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp(ex.Message, this.Text, 3000, true);
            }
        }

        private void SaveSettings()
        {
            try
            {
                // textboxes
                Boxes.Save();

                // combo boxes
                mf.Tls.SaveProperty("GPSreceiver", cbReceiver.SelectedIndex.ToString());
                mf.Tls.SaveProperty("IMU", cbIMU.SelectedIndex.ToString());
                mf.Tls.SaveProperty("RelayControl", cbRelayControl.SelectedIndex.ToString());
                mf.Tls.SaveProperty("AnalogMethod", cbAnalog.SelectedIndex.ToString());

                // check boxes
                for (int i = 0; i < CKs.Length; i++)
                {
                    mf.Tls.SaveProperty(CKs[i].Name, CKs[i].Checked.ToString());
                }
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp(ex.Message, this.Text, 3000, true);
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
                    btnSendToModule.Enabled = false;
                    TabEdited[tabControl1.SelectedIndex] = true;
                }
                else
                {
                    btnCancel.Enabled = false;
                    bntOK.Image = Properties.Resources.bntOK_Image;
                    btnSendToModule.Enabled = true;
                }

                FormEdited = Edited;
            }
        }

        private void tb_Enter(object sender, EventArgs e)
        {
            int index = (int)((TextBox)sender).Tag;
            clsTextBox BX = Boxes.Item(index);
            double min = BX.MinValue;
            double max = BX.MaxValue;
            double Value = BX.Value();

            using (var form = new FormNumeric(min, max, Value))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    if (BX.TB.Name == "tbPulseCal")
                    {
                        BX.TB.Text = form.ReturnValue.ToString("N1");
                    }
                    else
                    {
                        BX.TB.Text = form.ReturnValue.ToString();
                    }
                }
            }
        }

        private void tb_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tb_Validating(object sender, CancelEventArgs e)
        {
            int index = (int)((TextBox)sender).Tag;
            clsTextBox BX = Boxes.Item(index);
            if (BX.Value() < BX.MinValue || BX.Value() > BX.MaxValue)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbIMUdelay_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Milliseconds delay in reading the IMU. This is used to match " +
                "the timing of the GPS data.";

            mf.Tls.ShowHelp(Message, "Read delay");
            hlpevent.Handled = true;
        }

        private void tbIMUinterval_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Milliseconds between updates from the IMU.";

            mf.Tls.ShowHelp(Message, "Report Interval");
            hlpevent.Handled = true;
        }

        private void tbPulseCal_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "The number of pulses per second output for monitors to read 1 KMH.";

            mf.Tls.ShowHelp(Message, "Speed pulse");
            hlpevent.Handled = true;
        }

        private void tbRS485port_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "The serial port used for RS485 relay control.";

            mf.Tls.ShowHelp(Message, "RS485 serial port");
            hlpevent.Handled = true;
        }

        private void tbRTCM_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "The port number for GPS corrections that matches the" +
                " port used in AGIO.";

            mf.Tls.ShowHelp(Message, "RTCM port");
            hlpevent.Handled = true;
        }

        private void tbSteerRelay_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Teensy pin used for the steer disconnect relay.";

            mf.Tls.ShowHelp(Message, "Steer relay");
            hlpevent.Handled = true;
        }

        private void UpdateForm()
        {
            Initializing = true;
            LoadSettings();
            Initializing = false;
        }

        private void tbWemosSerialPort_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Serial port Wemos D1 Mini is connected to." +
                " Can be used for ADS1115 or remote relay control.";

            mf.Tls.ShowHelp(Message, "Wemos D1 Mini");
            hlpevent.Handled = true;
        }

        private void tbNMEAserialPort_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "GPS data serial port from receiver.";

            mf.Tls.ShowHelp(Message, "Receive GPS data");
            hlpevent.Handled = true;
        }

        private void tbRTCMserialPort_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "GPS correction data serial port to receiver.";

            mf.Tls.ShowHelp(Message, "Correction data");
            hlpevent.Handled = true;
        }

        private void cbRelayControl_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void btnSendToModule_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Upload to module.";

            mf.Tls.ShowHelp(Message);
            hlpevent.Handled = true;
        }

        private void btnLoadDefaults_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Load defaults.";

            mf.Tls.ShowHelp(Message);
            hlpevent.Handled = true;
        }
    }
}