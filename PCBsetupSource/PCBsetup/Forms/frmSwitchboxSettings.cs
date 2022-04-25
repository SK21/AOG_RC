using AgOpenGPS;
using PCBsetup.Languages;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PCBsetup.Forms
{
    public partial class frmSwitchboxSettings : Form
    {
        public TextBox[] CFG;
        public frmMain mf;
        private bool ConfigEdited;
        private bool Initializing = false;

        public frmSwitchboxSettings(frmMain CallingForm)
        {
            InitializeComponent();
            mf = CallingForm;

            CFG = new TextBox[] {tbSW1,tbSW2,tbSW3,tbSW4,
                tbAuto,tbMasterOn,tbMasterOff,tbRateUp,tbRateDown};

            for (int i = 0; i < CFG.Length; i++)
            {
                CFG[i].Tag = i;
                CFG[i].Enter += tb_Enter;
                CFG[i].TextChanged += tb_TextChanged;
                CFG[i].Validating += tb_Validating;
                CFG[i].HelpRequested += Pins_HelpRequested;
            }
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
            tbMasterOn.Text = "5";
            tbMasterOff.Text = "3";
            tbRateUp.Text = "17";
            tbRateDown.Text = "16";
            tbSW1.Text = "18";
            tbSW2.Text = "9";
            tbSW3.Text = "6";
            tbSW4.Text = "4";
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
                double val;

                // textboxes
                for (int i = 0; i < CFG.Length; i++)
                {
                    double.TryParse(mf.Tls.LoadProperty(CFG[i].Name), out val);
                    CFG[i].Text = val.ToString("N0");
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
                for (int i = 0; i < CFG.Length; i++)
                {
                    mf.Tls.SaveProperty(CFG[i].Name, CFG[i].Text);
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
            int tmp;
            int max;
            int min;

            switch (index)
            {
                default:
                    // pins
                    max = 21;
                    min = 0;
                    break;
            }

            int.TryParse(CFG[index].Text, out tmp);
            using (var form = new FormNumeric(min, max, tmp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    CFG[index].Text = form.ReturnValue.ToString();
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
            int tmp;
            int max;
            int min;

            switch (index)
            {
                default:
                    // pins
                    max = 21;
                    min = 0;
                    break;
            }
            int.TryParse(CFG[index].Text, out tmp);
            if (tmp < min || tmp > max)
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
    }
}