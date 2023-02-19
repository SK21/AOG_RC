using AgOpenGPS;
using PCBsetup.Languages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCBsetup.Forms
{
    public partial class frmTRsettings : Form
    {
        public clsTextBoxes Boxes;
        public CheckBox[] CKs;
        public frmMain mf;
        private bool Initializing = false;
        private bool[] TabEdited;
        private bool FormEdited = false;

        public frmTRsettings(frmMain CallingForm)
        {
            InitializeComponent();
            mf = CallingForm;

            CKs = new CheckBox[] { ckNanoRelayOn, ckNanoFlowOn };

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
            tbTRModuleID.Text = "0";
            tbTRSensorCount.Text = "1";
            ckNanoRelayOn.Checked = false;
            ckNanoFlowOn.Checked = false;
            tbTRWemosPort.Text = "1";
            tbTRDebounce.Text = "3";

            tbTRFlow1.Text = "28";
            tbTRDir1.Text = "37";
            tbTRPWM1.Text = "36";

            tbTRFlow2.Text = "29";
            tbTRDir2.Text = "14";
            tbTRPWM2.Text = "15";

            tbTRIP.Text = "1";
            cbTRRelayControl.SelectedIndex = 5;

            tbTRRelay1.Text = "8";
            tbTRRelay2.Text = "9";
            tbTRRelay3.Text = "10";
            tbTRRelay4.Text = "11";
            tbTRRelay5.Text = "12";
            tbTRRelay6.Text = "25";
            tbTRRelay7.Text = "26";
            tbTRRelay8.Text = "27";
        }

        private void btnSendToModule_Click(object sender, EventArgs e)
        {
            bool Sent = false;
            try  
            {
                PGN32500 PGN = new PGN32500(this);
                Sent = PGN.Send();

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
            int StartID = Boxes.Add(this.Text, tbTRModuleID, 15);
            Boxes.Add(this.Text, tbTRSensorCount, 2);
            Boxes.Add(this.Text, tbTRIP, 254);
            Boxes.Add(this.Text, tbTRWemosPort, 8);

            Boxes.Add(this.Text, tbTRFlow1, 41);
            Boxes.Add(this.Text, tbTRFlow2, 41);
            Boxes.Add(this.Text, tbTRDir1, 41);
            Boxes.Add(this.Text, tbTRDir2, 41);
            Boxes.Add(this.Text, tbTRPWM1, 41);
            Boxes.Add(this.Text, tbTRPWM2, 41);
            Boxes.Add(this.Text, tbTRRelay1, 41);
            Boxes.Add(this.Text, tbTRRelay2, 41);
            Boxes.Add(this.Text, tbTRRelay3, 41);
            Boxes.Add(this.Text, tbTRRelay4, 41);
            Boxes.Add(this.Text, tbTRRelay5, 41);

            Boxes.Add(this.Text, tbTRRelay6, 41);
            Boxes.Add(this.Text, tbTRRelay7, 41);
            Boxes.Add(this.Text, tbTRRelay8, 41);
            Boxes.Add(this.Text, tbTRRelay9, 41);
            Boxes.Add(this.Text, tbTRRelay10, 41);
            Boxes.Add(this.Text, tbTRRelay11, 41);
            Boxes.Add(this.Text, tbTRRelay12, 41);
            Boxes.Add(this.Text, tbTRRelay13, 41);
            Boxes.Add(this.Text, tbTRRelay14, 41);
            Boxes.Add(this.Text, tbTRRelay15, 41);
            int EndID = Boxes.Add(this.Text, tbTRRelay16, 41);

            for (int i = StartID; i < EndID + 1; i++)
            {
                Boxes.Item(i).TB.Tag = Boxes.Item(i).ID;
                Boxes.Item(i).TB.Enter += tb_Enter;
                Boxes.Item(i).TB.TextChanged += tb_TextChanged;
                Boxes.Item(i).TB.Validating += tb_Validating;
            }

            Boxes.Add(this.Text, tbTRDebounce, 100);
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
            string Message = "Use the MCP23017 to control relays or use Teensy pins.";

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
                byte tmp;
                bool Checked;

                // textboxes
                Boxes.ReLoad();

                // combo boxes
                byte.TryParse(mf.Tls.LoadProperty(this.Text+"/"+"TRRelayControl"), out tmp);
                cbTRRelayControl.SelectedIndex = tmp;


                // check boxes
                for (int i = 0; i < CKs.Length; i++)
                {
                    bool.TryParse(mf.Tls.LoadProperty(this.Text+"/"+CKs[i].Name), out Checked);
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
                mf.Tls.SaveProperty(this.Text+"/"+"TRRelayControl", cbTRRelayControl.SelectedIndex.ToString());

                // check boxes
                for (int i = 0; i < CKs.Length; i++)
                {
                    mf.Tls.SaveProperty(this.Text+"/" + CKs[i].Name, CKs[i].Checked.ToString());
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
            double val = Boxes.Value("tbTRModuleID");
            lbIPpart4.Text = "." + (val + 60).ToString();

            Initializing = false;
        }

        private void cbRelayControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbTRDebounce_HelpRequested(object sender, HelpEventArgs hlpevent)
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
    }
}
