using AgOpenGPS;
using PCBsetup.Languages;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PCBsetup.Forms
{
    public partial class frmNanoSettings : Form
    {
        public clsTextBoxes Boxes;
        public CheckBox[] CKs;
        public frmMain mf;
        private bool Initializing = false;
        private bool[] TabEdited;
        private bool FormEdited = false;

        public frmNanoSettings(frmMain CallingForm)
        {
            InitializeComponent();

            mf = CallingForm;

            CKs = new CheckBox[] { ckUseMCP23017, ckNanoRelayOn, ckNanoFlowOn };

            for (int i = 0; i < CKs.Length; i++)
            {
                CKs[i].CheckedChanged += tb_TextChanged;
            }

            TabEdited = new bool[2];

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
                    for (int i = 0; i < 2; i++)
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
            for (int i = 0; i < 2; i++)
            {
                TabEdited[i] = false;
            }
            UpdateForm();
            SetButtons(false);
        }

        private void btnLoadDefaults_Click(object sender, EventArgs e)
        {
            tbNanoModuleID.Text = "0";
            tbNanoSensorCount.Text = "1";
            ckUseMCP23017.Checked = true;
            ckNanoRelayOn.Checked = false;
            ckNanoFlowOn.Checked = false;

            tbNanoFlow1.Text = "2";
            tbNanoFlow2.Text = "3";
            tbNanoDir1.Text = "4";
            tbNanoDir2.Text = "6";
            tbNanoPWM1.Text = "5";
            tbNanoPWM2.Text = "9";

            tbNanoIP.Text = "1";
            tbNanoDebounce.Text = "3";
        }

        private void btnSendToModule_Click(object sender, EventArgs e)
        {
            bool Sent = false;
            try
            {
                PGN32625 PGN = new PGN32625(this);
                Sent = PGN.Send();
                PGN32626 PGN2 = new PGN32626(this);
                Sent = Sent & PGN2.Send();

                if (Sent)
                {
                    mf.Tls.ShowHelp("Sent to module.", this.Text, 3000);

                    for (int i = 0; i < 2; i++)
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
            int StartID = Boxes.Add(this.Text, tbNanoModuleID, 15);
            Boxes.Add(this.Text, tbNanoSensorCount, 2);
            Boxes.Add(this.Text, tbNanoIP, 254);

            Boxes.Add(this.Text, tbNanoFlow1, 21);
            Boxes.Add(this.Text, tbNanoFlow2, 21);
            Boxes.Add(this.Text, tbNanoDir1, 21);
            Boxes.Add(this.Text, tbNanoDir2, 21);
            Boxes.Add(this.Text, tbNanoPWM1, 21);
            Boxes.Add(this.Text, tbNanoPWM2, 21);
            Boxes.Add(this.Text, tbRelay1, 21);
            Boxes.Add(this.Text, tbRelay2, 21);
            Boxes.Add(this.Text, tbRelay3, 21);
            Boxes.Add(this.Text, tbRelay4, 21);
            Boxes.Add(this.Text, tbRelay5, 21);

            Boxes.Add(this.Text, tbRelay6, 21);
            Boxes.Add(this.Text, tbRelay7, 21);
            Boxes.Add(this.Text, tbRelay8, 21);
            Boxes.Add(this.Text, tbRelay9, 21);
            Boxes.Add(this.Text, tbRelay10, 21);
            Boxes.Add(this.Text, tbRelay11, 21);
            Boxes.Add(this.Text, tbRelay12, 21);
            Boxes.Add(this.Text, tbRelay13, 21);
            Boxes.Add(this.Text, tbRelay14, 21);
            Boxes.Add(this.Text, tbRelay15, 21);
            int EndID = Boxes.Add(this.Text, tbRelay16, 21);

            for (int i = StartID; i < EndID + 1; i++)
            {
                Boxes.Item(i).TB.Tag = Boxes.Item(i).ID;
                Boxes.Item(i).TB.Enter += tb_Enter;
                Boxes.Item(i).TB.TextChanged += tb_TextChanged;
                Boxes.Item(i).TB.Validating += tb_Validating;
            }

            Boxes.Add(this.Text, tbNanoDebounce, 100);
        }

        private void ckNanoFlowOn_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Which signal (high or low) increases the flow rate" +
                " for rate control.";

            mf.Tls.ShowHelp(Message, "Flow on high");
            hlpevent.Handled = true;
        }

        private void ckNanoRelayOn_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Which signal (high or low) turns on the relay.";

            mf.Tls.ShowHelp(Message, "Relay on high");
            hlpevent.Handled = true;
        }

        private void ckUseMCP23017_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Use the MCP23017 to control relays or use Nano pins.";

            mf.Tls.ShowHelp(Message, "Use MCP23017");
            hlpevent.Handled = true;
        }

        private void frmNanoSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                mf.Tls.SaveFormData(this);
            }
        }

        private void frmNanoSettings_Load(object sender, EventArgs e)
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
                bool Checked;

                // textboxes
                Boxes.ReLoad();

                // check boxes
                for (int i = 0; i < CKs.Length; i++)
                {
                    bool.TryParse(mf.Tls.LoadProperty(this.Text + "/" + CKs[i].Name), out Checked);
                    CKs[i].Checked = Checked;
                }
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp(ex.Message, this.Text, 3000, true);
            }
        }

        private void Pins_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Nano pin number for each function. Digital pins 2-13. For Analog pins A0-A7 use 14-21. " +
                "Nano relay pins only work if 'Use MCP23017' is unchecked." +
                " \n\nIf using the ENC28J60 ethernet shield these pins are used by it and" +
                " unavailable for relays: 7,8,10,11,12,13. It also pulls pin D2 high. " +
                "D2 can be used if pin D2 on the shield is cut off and then mount the" +
                " shield on top of the Nano.";
            mf.Tls.ShowHelp(Message, "Nano Pins", 60000);
            hlpevent.Handled = true;
        }

        private void SaveSettings()
        {
            try
            {
                // textboxes
                Boxes.Save();

                // check boxes
                for (int i = 0; i < CKs.Length; i++)
                {
                    mf.Tls.SaveProperty(this.Text + "/" + CKs[i].Name, CKs[i].Checked.ToString());
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
                    BX.TB.Text = form.ReturnValue.ToString();
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

        private void tbNanoSensorCount_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "# of flow sensors.";

            mf.Tls.ShowHelp(Message, "Sensor Count");
            hlpevent.Handled = true;
        }

        private void UpdateForm()
        {
            Initializing = true;
            LoadSettings();

            // IP address
            double val = Boxes.Value("tbNanoModuleID");
            lbIPpart4.Text = "." + (val + 50).ToString();

            Initializing = false;
        }

        private void ckUseMCP23017_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void tbNanoDebounce_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Mimiumum milliseconds between sensor pulses.";

            mf.Tls.ShowHelp(Message, "Debounce");
            hlpevent.Handled = true;
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

        private void tbNanoFlow1_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Pin number for function. For A0-A7 use 14-21.";

            mf.Tls.ShowHelp(Message);
            hlpevent.Handled = true;
        }
    }
}