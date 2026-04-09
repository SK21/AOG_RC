using RateController.Classes;
using RateController.PGNs;
using System;
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
            UpdateForm();
            SetButtons(false);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                int NewBoard;
                if (rbNano.Checked)
                {
                    NewBoard = 0;
                }
                else if (rbESP32.Checked)
                {
                    NewBoard = 2;
                }
                else
                {
                    NewBoard = 1;
                }

                if (NewBoard != CurrentBoardType())
                {
                    Props.SetProp("BoardType", NewBoard.ToString());
                    SetDefaults();
                }

                SetButtons(false);
                UpdateForm();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuNetwork/btnOk_Click: " + ex.Message);
            }
        }

        private int CurrentBoardType()
        {
            return int.TryParse(Props.GetProp("BoardType"), out int bt) ? bt : 1;
        }

        private void frmMenuNetwork_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
        }

        private void frmMenuNetwork_Load(object sender, EventArgs e)
        {
            SubMenuLayout.SetFormLayout(this, MainMenu, null);
            btnOK.Left = this.Width - 84;
            btnOK.Top = this.Height - 84;
            btnCancel.Left = btnOK.Left - 78;
            btnCancel.Top = btnOK.Top;
            MainMenu.StyleControls(this);
            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            this.BackColor = Properties.Settings.Default.MainBackColour;
            PositionForm();
            UpdateForm();
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
            SetButtons(true);
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

        private void SetDefaults()
        {
            PGN32700 Set = Core.ModuleConfig;
            byte[] Pins = new byte[16];

            for (int i = 0; i < 16; i++)
            {
                Pins[i] = 255;
            }

            switch (CurrentBoardType())
            {
                case 1:
                    // RC11-2, Teensy
                    Set.ModuleID = 0;
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
                    Set.ModuleID = 0;
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
                    Set.ModuleID = 0;
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

            MainMenu.DefaultsSet();
        }

        private void SetLanguage()
        {
        }

        private void UpdateForm()
        {
            Initializing = true;

            switch (CurrentBoardType())
            {
                case 1:
                    rbTeensy.Checked = true;
                    break;

                case 2:
                    rbESP32.Checked = true;
                    break;

                default:
                    rbNano.Checked = true;
                    break;
            }

            Initializing = false;
        }
    }
}