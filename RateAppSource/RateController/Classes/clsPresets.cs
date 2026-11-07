using RateController.Language;
using RateController.Menu;
using RateController.PGNs;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.Classes
{
    // Starting configurations for the known boards, loaded into the module being edited:
    // module config (sensor count, relay types, inverts, ADS1115, 3-wire), sensor pins and
    // relay pins. Network settings (ClientMode) are the board's own and are left alone.
    public static class clsPresets
    {
        public static bool Has(ModuleTypes Type)
        {
            return Type == ModuleTypes.ESP_Rate || Type == ModuleTypes.Teensy_Rate || Type == ModuleTypes.Nano_Rate;
        }

        public static string Name(ModuleTypes Type)
        {
            switch (Type)
            {
                case ModuleTypes.ESP_Rate: return "ESP32 (RC15)";
                case ModuleTypes.Teensy_Rate: return "Teensy (RC11-2)";
                case ModuleTypes.Nano_Rate: return "Nano (RC12-3)";
                default: return "";
            }
        }

        // asks which preset to load when no board is reporting at the module;
        // ModuleTypes.unknown when cancelled
        public static ModuleTypes Choose(IWin32Window Owner)
        {
            ModuleTypes Result = ModuleTypes.unknown;
            using (Form frm = new Form())
            {
                frm.Text = Lang.lgLoadPresetChoose;
                frm.FormBorderStyle = FormBorderStyle.FixedDialog;
                frm.MaximizeBox = false;
                frm.MinimizeBox = false;
                frm.ShowIcon = false;
                frm.ShowInTaskbar = false;
                frm.TopMost = true;
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.ClientSize = new Size(300, 260);
                frm.BackColor = Properties.Settings.Default.MainBackColour;

                ModuleTypes[] types = { ModuleTypes.Nano_Rate, ModuleTypes.Teensy_Rate, ModuleTypes.ESP_Rate };
                for (int i = 0; i < types.Length; i++)
                {
                    ModuleTypes t = types[i];
                    Button btn = new Button
                    {
                        Text = Name(t),
                        Font = new Font("Microsoft Sans Serif", 14.25F),
                        FlatStyle = FlatStyle.Flat,
                        Location = new Point(30, 15 + i * 55),
                        Size = new Size(240, 45),
                        ForeColor = Properties.Settings.Default.MainForeColour
                    };
                    btn.Click += (s, e) => { Result = t; frm.DialogResult = DialogResult.OK; };
                    frm.Controls.Add(btn);
                }

                Button cancel = new Button
                {
                    Image = Properties.Resources.Cancel64,
                    FlatStyle = FlatStyle.Flat,
                    Location = new Point(114, 182),
                    Size = new Size(72, 72)
                };
                cancel.FlatAppearance.BorderSize = 0;
                cancel.Click += (s, e) => { frm.DialogResult = DialogResult.Cancel; };
                frm.Controls.Add(cancel);
                frm.CancelButton = cancel;

                if (frm.ShowDialog(Owner) != DialogResult.OK) Result = ModuleTypes.unknown;
            }
            return Result;
        }

        // loads the preset into the module being edited, saved, ready for Send
        public static void Load(ModuleTypes Type, frmMenu MainMenu)
        {
            if (!Has(Type)) return;

            PGN32700 Set = Core.ModuleConfig;
            byte[] Pins = new byte[16];

            for (int i = 0; i < 16; i++)
            {
                Pins[i] = 255;
            }

            // the target module ID (config byte 2) is left alone
            switch (Type)
            {
                case ModuleTypes.Teensy_Rate:
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

                case ModuleTypes.ESP_Rate:
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

            MainMenu?.DefaultsSet();                // null: the menu is closed (new board setup)
            MainMenu?.HighlightUpdateButton();
        }
    }
}
