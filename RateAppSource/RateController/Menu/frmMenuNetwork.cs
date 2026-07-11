using RateController.Classes;
using RateController.PGNs;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuNetwork : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;

        public frmMenuNetwork(frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            this.Tag = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SetButtons(false);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                // only load a preset when one was actually selected - the description
                // alone also enables Save, and that must not reset the module config
                if (rbNano.Checked)
                {
                    SetDefaults(0);
                }
                else if (rbESP32.Checked)
                {
                    SetDefaults(2);
                }
                else if (rbTeensy.Checked)
                {
                    SetDefaults(1);
                }

                SetButtons(false);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuNetwork/btnOk_Click: " + ex.Message);
            }
        }


        private void frmMenuNetwork_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainMenu.MenuMoved -= MainMenu_MenuMoved;
            Props.SaveFormLocation(this);
        }

        private void frmMenuNetwork_Load(object sender, EventArgs e)
        {
            SubMenuLayout.SetFormLayout(this, MainMenu, null);
            btnOK.Left = this.Width - 84;
            btnOK.Top = this.Height - 84;
            btnCancel.Left = btnOK.Left - 78;
            btnCancel.Top = btnOK.Top;

            // StyleControls restyles every label's font and colours; capture the hint
            // label's designer formatting and restore it afterwards so the hint keeps its look.
            Font exFont = lbExampleHint.Font;
            Color exFore = lbExampleHint.ForeColor;

            MainMenu.StyleControls(this);

            lbExampleHint.Font = exFont;
            lbExampleHint.ForeColor = exFore;

            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            this.BackColor = Properties.Settings.Default.MainBackColour;
            PositionForm();
        }

        private void MainMenu_MenuMoved(object sender, EventArgs e)
        {
            PositionForm();
        }

        private void PositionForm()
        {
            this.Top = MainMenu.Top + SubMenuLayout.TopOffset;
            this.Left = MainMenu.Left + SubMenuLayout.LeftOffset;
        }

        private void rbESP32_CheckedChanged(object sender, EventArgs e)
        {
            // Fires on uncheck too (SetButtons(false) clears the radios after
            // Save/Cancel) - only a genuine selection marks the form edited.
            // The description is left alone - it identifies the physical board,
            // so the user fills it in themselves.
            if (sender is RadioButton rb && rb.Checked)
            {
                SetButtons(true);
            }
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
                    rbNano.Checked = false;
                    rbTeensy.Checked = false;
                    rbESP32.Checked = false;
                }

                cEdited = Edited;
                this.Tag = cEdited;
            }
        }

        private void SetDefaults(int Brd)
        {
            PGN32700 Set = Core.ModuleConfig;
            byte[] Pins = new byte[16];

            for (int i = 0; i < 16; i++)
            {
                Pins[i] = 255;
            }

            // presets load a starting pin layout only - the target module ID
            // (config byte 2) is left alone
            switch (Brd)
            {
                case 1:
                    // RC11-2, Teensy
                    Set.SensorCount = 1;
                    Set.OnboardRelayType = 1;
                    Set.RemoteRelayType = 0;
                    Set.InvertRelay = true;
                    Set.InvertFlow = true;
                    Set.Momentary = false;
                    Set.ADS1115enabled = false;
                    Set.Sensor0Flow = 28;
                    Set.Sensor1Flow = 29;
                    Set.Sensor0Dir = 37;
                    Set.Sensor1Dir = 14;
                    Set.Sensor0PWM = 36;
                    Set.Sensor1PWM = 15;
                    Set.WorkPin = 30;
                    Set.PressurePin = 40;
                    Set.ClientMode = false;
                    Set.Is3Wire = true;

                    Pins[0] = 8;    // relay 1
                    Pins[1] = 9;
                    Pins[2] = 10;
                    Pins[3] = 11;
                    Pins[4] = 12;
                    Pins[5] = 25;
                    Pins[6] = 26;
                    Pins[7] = 27;
                    break;

                case 2:
                    // RC15, ESP32
                    Set.SensorCount = 1;
                    Set.OnboardRelayType = 5;
                    Set.RemoteRelayType = 0;
                    Set.InvertRelay = true;
                    Set.InvertFlow = true;
                    Set.Momentary = false;
                    Set.ADS1115enabled = true;
                    Set.Sensor0Flow = 17;
                    Set.Sensor1Flow = 16;
                    Set.Sensor0Dir = 32;
                    Set.Sensor1Dir = 25;
                    Set.Sensor0PWM = 33;
                    Set.Sensor1PWM = 26;
                    Set.WorkPin = 255;
                    Set.PressurePin = 255;
                    Set.ClientMode = false;
                    Set.Is3Wire = true;
                    break;

                default:
                    // RC12-3, Nano
                    Set.SensorCount = 1;
                    Set.OnboardRelayType = 4;
                    Set.RemoteRelayType = 0;
                    Set.InvertRelay = true;
                    Set.InvertFlow = true;
                    Set.Momentary = false;
                    Set.ADS1115enabled = false;
                    Set.Sensor0Flow = 3;
                    Set.Sensor1Flow = 2;
                    Set.Sensor0Dir = 4;
                    Set.Sensor1Dir = 6;
                    Set.Sensor0PWM = 5;
                    Set.Sensor1PWM = 9;
                    Set.WorkPin = 15;
                    Set.PressurePin = 14;
                    Set.ClientMode = false;
                    Set.Is3Wire = true;

                    Pins[0] = 0;    // relay 1
                    Pins[1] = 15;
                    Pins[2] = 1;
                    Pins[3] = 14;
                    Pins[4] = 2;
                    Pins[5] = 13;
                    Pins[6] = 3;
                    Pins[7] = 12;

                    Pins[8] = 4;
                    Pins[9] = 11;
                    Pins[10] = 5;
                    Pins[11] = 10;
                    Pins[12] = 6;
                    Pins[13] = 9;
                    Pins[14] = 7;
                    Pins[15] = 8;
                    break;
            }

            Set.RelayPins(Pins);
            Set.Save();

            // the sensor-pins config (PGN 32507) is the source the 32700 mirror reads
            // from at send, so board defaults must land there too; bin pins default off
            Core.SensorPins.SetPins(0, Set.Sensor0Flow, Set.Sensor0Dir, Set.Sensor0PWM, 255, false);
            Core.SensorPins.SetPins(1, Set.Sensor1Flow, Set.Sensor1Dir, Set.Sensor1PWM, 255, false);
            for (int i = 2; i < PGN32507.MaxSensors; i++)
            {
                Core.SensorPins.SetPins(i, 255, 255, 255, 255, false);
            }
            Core.SensorPins.Save();

            MainMenu.DefaultsSet();
            MainMenu.HighlightUpdateButton();
        }

        private void SetLanguage()
        {
        }

        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
            Props.DrawGroupBox((GroupBox)sender, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }
    }
}