using AgOpenGPS;
using PCBsetup.Languages;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PCBsetup.Forms
{
    public partial class frmSwitchboxSettings : Form
    {
        public clsTextBoxes Boxes;
        public frmMain mf;
        private bool ConfigEdited;
        private bool Initializing = false;

        public frmSwitchboxSettings(frmMain CallingForm)
        {
            InitializeComponent();
            mf = CallingForm;

            Boxes = new clsTextBoxes(mf);
            BuildBoxes();
        }

        private void bntOK_Click(object sender, EventArgs e)
        {
            try
            {
                Button ButtonClicked = (Button)sender;
                if (ButtonClicked.Text == PCBsetup.Languages.Lang.lgClose)
                {
                    if (ConfigEdited) mf.Tls.ShowHelp("Changes have not been sent to the module.", "Warning", 3000);

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
            UpdateForm();
            SetButtons(false);
        }

        private void btnLoadDefaults_Click(object sender, EventArgs e)
        {
            tbAuto.Text = "19";
            tbMasterOn.Text = "3";
            tbMasterOff.Text = "5";
            tbRateUp.Text = "17";
            tbRateDown.Text = "16";
            tbIPaddress.Text = "1";

            tbSW1.Text = "18";
            tbSW2.Text = "9";
            tbSW3.Text = "6";
            tbSW4.Text = "4";
            tbSW5.Text = "0";
            tbSW6.Text = "0";
            tbSW7.Text = "0";
            tbSW8.Text = "0";

            tbSW9.Text = "0";
            tbSW10.Text = "0";
            tbSW11.Text = "0";
            tbSW12.Text = "0";
            tbSW13.Text = "0";
            tbSW14.Text = "0";
            tbSW15.Text = "0";
            tbSW16.Text = "0";

        }

        private void btnSendToModule_Click(object sender, EventArgs e)
        {
            bool Sent;
            try
            {
                PGN32627 PGN = new PGN32627(this);
                Sent = PGN.Send();

                if (Sent)
                {
                    mf.Tls.ShowHelp("Sent to module.", this.Text, 3000);
                    ConfigEdited = false;
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
            Boxes.Add(tbAuto, 21);
            Boxes.Add(tbMasterOn, 21);
            Boxes.Add(tbMasterOff, 21);
            Boxes.Add(tbRateUp, 21);
            Boxes.Add(tbRateDown, 21);
            Boxes.Add(tbIPaddress, 254); 

            Boxes.Add(tbSW1, 21);
            Boxes.Add(tbSW2, 21);
            Boxes.Add(tbSW3, 21);
            Boxes.Add(tbSW4, 21);
            Boxes.Add(tbSW5, 21);
            Boxes.Add(tbSW6, 21);
            Boxes.Add(tbSW7, 21);
            Boxes.Add(tbSW8, 21);

            Boxes.Add(tbSW9, 21);
            Boxes.Add(tbSW10, 21);
            Boxes.Add(tbSW11, 21);
            Boxes.Add(tbSW12, 21);
            Boxes.Add(tbSW13, 21);
            Boxes.Add(tbSW14, 21);
            Boxes.Add(tbSW15, 21);
            Boxes.Add(tbSW16, 21);

            for (int i = 0; i < Boxes.Count(); i++)
            {
                Boxes.Item(i).TB.Tag = Boxes.Item(i).ID;
                Boxes.Item(i).TB.Enter += tb_Enter;
                Boxes.Item(i).TB.TextChanged += tb_TextChanged;
                Boxes.Item(i).TB.Validating += tb_Validating;
            }
        }

        private void frmSwitchboxSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                mf.Tls.SaveFormData(this);
            }
        }

        private void frmSwitchboxSettings_Load(object sender, EventArgs e)
        {
            try
            {
                mf.Tls.LoadFormData(this);

                this.BackColor = PCBsetup.Properties.Settings.Default.DayColour;

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
                // textboxes
                Boxes.ReLoad();
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp(ex.Message, this.Text, 3000, true);
            }
        }

        private void Pins_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Nano pin number for each function. Digital pins 2-13. For Analog pins A0-A7 use 14-21. " +
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
                    this.bntOK.Text = Lang.lgSave;
                    btnSendToModule.Enabled = false;
                    ConfigEdited = true;
                }
                else
                {
                    btnCancel.Enabled = false;
                    this.bntOK.Text = Lang.lgClose;
                    btnSendToModule.Enabled = true;
                }
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

        private void UpdateForm()
        {
            Initializing = true;
            LoadSettings();
            Initializing = false;
        }

        private void tbIPaddress_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbRateDown_TextChanged(object sender, EventArgs e)
        {

        }
    }
}