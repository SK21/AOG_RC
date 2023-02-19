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
        private bool FormEdited = false;

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
                if (!FormEdited)
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
            Boxes.Add(this.Text, tbAuto, 21);
            Boxes.Add(this.Text, tbMasterOn, 21);
            Boxes.Add(this.Text, tbMasterOff, 21);
            Boxes.Add(this.Text, tbRateUp, 21);
            Boxes.Add(this.Text, tbRateDown, 21);
            Boxes.Add(this.Text, tbIPaddress, 254); 

            Boxes.Add(this.Text, tbSW1, 21);
            Boxes.Add(this.Text, tbSW2, 21);
            Boxes.Add(this.Text, tbSW3, 21);
            Boxes.Add(this.Text, tbSW4, 21);
            Boxes.Add(this.Text, tbSW5, 21);
            Boxes.Add(this.Text, tbSW6, 21);
            Boxes.Add(this.Text, tbSW7, 21);
            Boxes.Add(this.Text, tbSW8, 21);

            Boxes.Add(this.Text, tbSW9, 21);
            Boxes.Add(this.Text, tbSW10, 21);
            Boxes.Add(this.Text, tbSW11, 21);
            Boxes.Add(this.Text, tbSW12, 21);
            Boxes.Add(this.Text, tbSW13, 21);
            Boxes.Add(this.Text, tbSW14, 21);
            Boxes.Add(this.Text, tbSW15, 21);
            Boxes.Add(this.Text, tbSW16, 21);

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
                    bntOK.Image = Properties.Resources.Save;
                    btnSendToModule.Enabled = false;
                    ConfigEdited = true;
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

        private void btnSendToModule_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Send to module.";

            mf.Tls.ShowHelp(Message);
            hlpevent.Handled = true;
        }

        private void btnLoadDefaults_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Load defaults.";

            mf.Tls.ShowHelp(Message);
            hlpevent.Handled = true;
        }

        private void tbAuto_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string Message = "Pin number for function. For A0-A7 use 14-21.";

            mf.Tls.ShowHelp(Message);
            hlpevent.Handled = true;
        }
    }
}