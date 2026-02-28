using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace ModuleSimulator
{
    public partial class frmMain : Form
    {
        // ── UDP ports (must match Core.cs UDPmodules constructor) ────────────────
        private const int RC_LISTEN_PORT  = 29999;  // RC listens here  → we send here
        private const int MOD_LISTEN_PORT = 28888;  // RC sends here    → we listen here

        // ── PGN headers ──────────────────────────────────────────────────────────
        private const byte HDR32400_LO = 144, HDR32400_HI = 126;
        private const byte HDR32401_LO = 145, HDR32401_HI = 126;

        private const ushort INO_ID   = 27026;
        private const byte   INO_TYPE = 1;

        // ── PID constant (matches Teensy PID.ino) ────────────────────────────────
        private const float FastAdjustValve = 40.0f;

        // ── UDP ──────────────────────────────────────────────────────────────────
        private Socket    _sendSocket;
        private Socket    _recvSocket;
        private readonly byte[] _recvBuffer = new byte[256];
        private volatile bool   _running;

        // ── Simulation state ─────────────────────────────────────────────────────
        private readonly SensorState _sensor = new SensorState();
        private double   _valvePos;          // simulated valve position 0-255 (with lag)
        private double   _accQty;            // accumulated quantity
        private ushort   _cmdRelays;         // relay bitmask from RC (PGN 32501)
        private readonly Random _rng = new Random();
        private DateTime _lastLoopTime = DateTime.MinValue;

        // ── SensorState ──────────────────────────────────────────────────────────
        // Mirrors Teensy's SensorConfig struct — populated from received PGNs.
        private class SensorState
        {
            // From PGN 32500
            public float    TargetUPM;
            public float    MeterCal;
            public bool     MasterOn;
            public bool     AutoOn;
            public int      ControlType;
            public short    ManualAdjust;
            public DateTime CommTime = DateTime.MinValue;

            // From PGN 32502 — defaults approximate typical RC settings
            public float MaxPWM        = 200f;
            public float MinPWM        = 10f;
            public float Kp            = (float)Math.Pow(1.1, 100 - 120); // ~0.0149
            public float Ki            = 0f;
            public float Deadband      = 0.05f;   // 5 % of target
            public int   BrakePoint    = 20;       // %
            public int   PIDslowAdjust = 50;       // %
            public int   SlewRate      = 20;
            public float MaxIntegral   = 2.0f;
            public int   PIDtime       = 100;      // ms

            // PID runtime state
            public float    PWM;
            public float    LastPWM;
            public float    IntegralSum;
            public bool     ErrorIsPositive = true;
            public DateTime LastPIDCheck    = DateTime.MinValue;

            // Outputs
            public float Hz;
            public float UPM;
            public bool  FlowEnabled;
        }

        // ── Settings persistence ─────────────────────────────────────────────────
        private static readonly string ConfigPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ModuleSimulator", "settings.ini");

        private void LoadSettings()
        {
            try
            {
                if (!File.Exists(ConfigPath)) return;
                foreach (string line in File.ReadAllLines(ConfigPath))
                {
                    int eq = line.IndexOf('=');
                    if (eq < 0) continue;
                    string key = line.Substring(0, eq).Trim();
                    string val = line.Substring(eq + 1).Trim();
                    ApplySetting(key, val);
                }
            }
            catch { }
        }

        private void ApplySetting(string key, string val)
        {
            decimal d;
            switch (key)
            {
                case "Subnet":     txtSubnet.Text    = val; break;
                case "ModuleID":   if (decimal.TryParse(val, out d)) nudModuleID.Value   = Clamp(d, nudModuleID.Minimum,   nudModuleID.Maximum);   break;
                case "SensorID":   if (decimal.TryParse(val, out d)) nudSensorID.Value   = Clamp(d, nudSensorID.Minimum,   nudSensorID.Maximum);   break;
                case "MaxHz":      if (decimal.TryParse(val, out d)) nudMaxHz.Value      = Clamp(d, nudMaxHz.Minimum,      nudMaxHz.Maximum);      break;
                case "Noise":      if (decimal.TryParse(val, out d)) nudNoise.Value      = Clamp(d, nudNoise.Minimum,      nudNoise.Maximum);      break;
                case "ValveLag":   if (decimal.TryParse(val, out d)) nudValveLag.Value   = Clamp(d, nudValveLag.Minimum,   nudValveLag.Maximum);   break;
                case "WheelSpeed": if (decimal.TryParse(val, out d)) nudWheelSpeed.Value = Clamp(d, nudWheelSpeed.Minimum, nudWheelSpeed.Maximum); break;
                case "Pressure":   if (decimal.TryParse(val, out d)) nudPressure.Value   = Clamp(d, nudPressure.Minimum,   nudPressure.Maximum);   break;
                case "WorkSwitch": ckWorkSwitch.Checked = val == "1"; break;
                case "Left":       if (int.TryParse(val, out int lv)) Left = lv; break;
                case "Top":        if (int.TryParse(val, out int tv)) Top  = tv; break;
            }
        }

        // Ensure title bar is reachable on at least one screen (matches RC IsOnScreen).
        private void EnsureOnScreen()
        {
            foreach (Screen s in Screen.AllScreens)
            {
                if (s.WorkingArea.Contains(new System.Drawing.Point(Left + 20, Top + 20)))
                    return;
            }
            // Not visible — fall back to primary screen top-left
            Left = Screen.PrimaryScreen.WorkingArea.Left + 40;
            Top  = Screen.PrimaryScreen.WorkingArea.Top  + 40;
        }

        private void SaveSettings()
        {
            try
            {
                string dir = Path.GetDirectoryName(ConfigPath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                var lines = new System.Collections.Generic.List<string>
                {
                    "Subnet="     + txtSubnet.Text.Trim(),
                    "ModuleID="   + nudModuleID.Value,
                    "SensorID="   + nudSensorID.Value,
                    "MaxHz="      + nudMaxHz.Value,
                    "Noise="      + nudNoise.Value,
                    "ValveLag="   + nudValveLag.Value,
                    "WheelSpeed=" + nudWheelSpeed.Value,
                    "Pressure="   + nudPressure.Value,
                    "WorkSwitch=" + (ckWorkSwitch.Checked ? "1" : "0"),
                };

                // Only save position when not minimised/maximised (matches RC SaveFormLocation)
                if (WindowState == FormWindowState.Normal)
                {
                    lines.Add("Left=" + Left);
                    lines.Add("Top="  + Top);
                }

                File.WriteAllLines(ConfigPath, lines);
            }
            catch { }
        }

        // ── Constructor ──────────────────────────────────────────────────────────
        public frmMain() { InitializeComponent(); LoadSettings(); EnsureOnScreen(); }

        // ── CRC (matches clsTools.CRC / Teensy CRC) ─────────────────────────────
        private static byte CalcCRC(byte[] data, int length)
        {
            int sum = 0;
            for (int i = 0; i < length; i++) sum += data[i];
            return (byte)sum;
        }

        // ── Start / Stop ─────────────────────────────────────────────────────────
        private void btnStart_Click(object sender, EventArgs e) => Start();
        private void btnStop_Click(object sender, EventArgs e)  => Stop();

        private void Start()
        {
            if (_running) return;
            try
            {
                // Reset state
                _accQty      = 0;
                _valvePos    = 0;
                _cmdRelays   = 0;
                _lastLoopTime = DateTime.MinValue;

                _sensor.TargetUPM      = 0; _sensor.MeterCal   = 0;
                _sensor.MasterOn       = false; _sensor.AutoOn  = false;
                _sensor.CommTime       = DateTime.MinValue;
                _sensor.PWM            = 0; _sensor.LastPWM    = 0;
                _sensor.IntegralSum    = 0; _sensor.Hz         = 0;
                _sensor.UPM            = 0;
                _sensor.ErrorIsPositive = true;
                _sensor.LastPIDCheck   = DateTime.MinValue;
                UpdateCommandLabels();

                // Send socket (broadcast)
                _sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _sendSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
                _sendSocket.Bind(new IPEndPoint(IPAddress.Any, 0));

                // Receive socket
                _recvSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _recvSocket.Bind(new IPEndPoint(IPAddress.Any, MOD_LISTEN_PORT));
                BeginReceive();

                _running = true;
                loopTimer.Start();
                sendTimer.Start();

                SetStatus("Running", Color.Green);
                UpdateUI();
            }
            catch (Exception ex)
            {
                SetStatus("Error: " + ex.Message, Color.Red);
                CleanupSockets();
            }
        }

        private void Stop()
        {
            if (!_running) return;
            _running = false;
            loopTimer.Stop();
            sendTimer.Stop();
            CleanupSockets();
            SetStatus("Stopped", Color.Gray);
            UpdateUI();
        }

        private void CleanupSockets()
        {
            try { _recvSocket?.Close(); } catch { }
            try { _sendSocket?.Close(); } catch { }
            _recvSocket = null;
            _sendSocket = null;
        }

        // ── Loop tick — 50 ms (matches Teensy LoopTime) ──────────────────────────
        private void loopTimer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            double dt = (_lastLoopTime == DateTime.MinValue)
                ? 0.05
                : (now - _lastLoopTime).TotalSeconds;
            if (dt > 0.2) dt = 0.05;   // clamp after pause/hitch
            _lastLoopTime = now;

            // 1. Determine if flow should be enabled (from Teensy SetSensorsEnabled)
            SetSensorsEnabled(now);

            // 2. PID control (from Teensy SetPWM / PIDvalve)
            if (_sensor.AutoOn)
                _sensor.PWM = PIDvalve(now);
            else if (_sensor.FlowEnabled)
                _sensor.PWM = Math.Sign(_sensor.ManualAdjust)
                              * Math.Min(Math.Abs((float)_sensor.ManualAdjust), _sensor.MaxPWM);
            else
                _sensor.PWM = 0;

            // 3. Physical flow simulation
            SimulateFlow(dt);

            // 4. Update display
            UpdateSimDisplay();
        }

        // ── Send tick — 200 ms (matches Teensy SendTime) ─────────────────────────
        private void sendTimer_Tick(object sender, EventArgs e)
        {
            byte modId = (byte)nudModuleID.Value;
            byte senId = (byte)nudSensorID.Value;

            _accQty += _sensor.UPM * (200.0 / 60000.0);

            SendPGN32401(modId);
            SendPGN32400(modId, senId);
        }

        // ── SetSensorsEnabled (port of Teensy RCteensy.ino:SetSensorsEnabled) ───
        private void SetSensorsEnabled(DateTime now)
        {
            bool result = false;
            if (_sensor.CommTime != DateTime.MinValue
                && (now - _sensor.CommTime).TotalSeconds < 5.0)
            {
                if (_sensor.TargetUPM > 0 && _sensor.MasterOn)
                    result = true;
                else if (_sensor.MasterOn && !_sensor.AutoOn)
                    result = true;
            }
            _sensor.FlowEnabled = result;
        }

        // ── PIDvalve (port of Teensy PID.ino:PIDvalve) ───────────────────────────
        private float PIDvalve(DateTime now)
        {
            float result = _sensor.LastPWM;

            if (_sensor.FlowEnabled && _sensor.TargetUPM > 0)
            {
                if ((now - _sensor.LastPIDCheck).TotalMilliseconds >= _sensor.PIDtime)
                {
                    _sensor.LastPIDCheck = now;

                    float rateError = _sensor.TargetUPM - _sensor.UPM;
                    bool  isPositive = rateError > 0;

                    if (isPositive != _sensor.ErrorIsPositive)
                    {
                        // zero crossing — reset integral to prevent overshoot
                        _sensor.ErrorIsPositive = isPositive;
                        _sensor.IntegralSum     = 0;
                    }

                    if (Math.Abs(rateError) > _sensor.Deadband * _sensor.TargetUPM)
                    {
                        rateError = Clamp(rateError, -_sensor.TargetUPM, _sensor.TargetUPM);

                        _sensor.IntegralSum += rateError * _sensor.Ki;
                        if (_sensor.Ki <= 0) _sensor.IntegralSum = 0;
                        _sensor.IntegralSum = Clamp(_sensor.IntegralSum,
                                                    -_sensor.MaxIntegral, _sensor.MaxIntegral);

                        float brakeFactor =
                            Math.Abs(rateError) > _sensor.TargetUPM * _sensor.BrakePoint / 100.0f
                            ? FastAdjustValve
                            : _sensor.PIDslowAdjust / 100.0f * FastAdjustValve;

                        float changeAmount = rateError * _sensor.Kp * brakeFactor * 100.0f
                                             + _sensor.IntegralSum;

                        if (Math.Abs(changeAmount) < 0.1f)
                        {
                            result = 0f;
                        }
                        else
                        {
                            result  = Clamp(Math.Abs(changeAmount) + _sensor.MinPWM,
                                            _sensor.MinPWM, _sensor.MaxPWM);
                            result *= changeAmount >= 0f ? 1f : -1f;
                        }
                    }
                    else
                    {
                        result              = 0f;
                        _sensor.IntegralSum = 0f;
                    }
                }
            }
            else
            {
                _sensor.IntegralSum = 0;
                result = 0;
            }

            _sensor.LastPWM = result;
            return result;
        }

        // ── Flow simulation ───────────────────────────────────────────────────────
        // Models: motor-driven valve → ideal Hz → noise → 80/20 filter → UPM
        //
        // The valve behaves like a motor-driven ball valve:
        //   positive PWM = open at that speed, negative PWM = close, zero = hold.
        // nudValveLag = time (ms) for the valve to travel full range at MaxPWM.
        // This matches real hardware behaviour and gives the PID something with
        // inertia to control — it holds position in the deadband rather than
        // springing back to zero, so the loop converges without oscillation.
        private void SimulateFlow(double dt)
        {
            bool relaysActive = _cmdRelays > 0;

            if (!_sensor.FlowEnabled || !relaysActive)
            {
                // Master off or no sections open — close valve immediately.
                _valvePos  = 0;
                _sensor.Hz = 0;
            }
            else
            {
                // Motor speed is proportional to PWM / MaxPWM.
                // nudValveLag (ms) = time to traverse 0→255 at full PWM.
                double travelTime = Math.Max(0.1, (double)nudValveLag.Value / 1000.0);
                double maxRate    = 255.0 / travelTime;   // valve-units / sec at MaxPWM
                double speed      = (_sensor.MaxPWM > 0)
                                    ? (_sensor.PWM / _sensor.MaxPWM) * maxRate * dt
                                    : 0;
                _valvePos = Math.Max(0.0, Math.Min(255.0, _valvePos + speed));

                // Ideal Hz proportional to valve opening
                double maxHz    = (double)nudMaxHz.Value;
                double idealHz  = (_valvePos / 255.0) * maxHz;

                // Noise: approximate Gaussian using sum of two uniform samples [-1,1]
                double u        = _rng.NextDouble() + _rng.NextDouble() - 1.0;
                double noisePct = (double)nudNoise.Value / 100.0;
                double noisyHz  = Math.Max(0.0, idealHz + idealHz * noisePct * u);

                // Low-pass filter — matches Teensy Rate.ino GetUPM():
                //   Sensor[i].Hz = hz * 0.8 + Sensor[i].Hz * 0.2
                _sensor.Hz = (float)(noisyHz * 0.8 + _sensor.Hz * 0.2);
            }

            // UPM from Hz — matches Teensy: UPM = 60 * Hz / MeterCal
            _sensor.UPM = _sensor.MeterCal > 0
                          ? (float)(60.0 * _sensor.Hz / _sensor.MeterCal)
                          : 0f;
        }

        // ── PGN senders ──────────────────────────────────────────────────────────
        private void SendPGN32401(byte modId)
        {
            byte[] d = new byte[15];
            d[0] = HDR32401_LO; d[1] = HDR32401_HI;
            d[2] = modId;

            ushort pressure = (ushort)nudPressure.Value;
            d[3] = (byte)pressure; d[4] = (byte)(pressure >> 8);

            ushort ws = (ushort)((double)nudWheelSpeed.Value * 10.0);
            d[5] = (byte)ws; d[6] = (byte)(ws >> 8);

            d[7] = d[8] = d[9] = 0;
            d[10] = INO_TYPE;
            d[11] = (byte)(INO_ID & 0xFF);
            d[12] = (byte)(INO_ID >> 8);

            d[13] = 0b0011_0000;   // EthernetConnected + GoodPins
            if (ckWorkSwitch.Checked) d[13] |= 0x01;

            d[14] = CalcCRC(d, 14);
            UdpSend(d);
        }

        private void SendPGN32400(byte modId, byte senId)
        {
            byte[] d = new byte[15];
            d[0] = HDR32400_LO; d[1] = HDR32400_HI;
            d[2] = (byte)((modId << 4) | (senId & 0x0F));

            int r = (int)(_sensor.UPM * 1000.0);
            d[3] = (byte)r; d[4] = (byte)(r >> 8); d[5] = (byte)(r >> 16);

            int q = (int)(_accQty * 10.0);
            d[6] = (byte)q; d[7] = (byte)(q >> 8); d[8] = (byte)(q >> 16);

            // PWM as Int16 (signed, matches Teensy send)
            int pwm = (int)_sensor.PWM;
            d[9]  = (byte)pwm;
            d[10] = (byte)(pwm >> 8);

            d[11] = 0x01;   // sensor connected

            int hzInt = (int)(_sensor.Hz * 10.0);
            d[12] = (byte)hzInt; d[13] = (byte)(hzInt >> 8);

            d[14] = CalcCRC(d, 14);
            UdpSend(d);
        }

        private void UdpSend(byte[] data)
        {
            if (_sendSocket == null) return;
            try
            {
                string subnet = txtSubnet.Text.Trim().TrimEnd('.');
                string[] parts = subnet.Split('.');
                string bc = parts.Length >= 3
                    ? $"{parts[0]}.{parts[1]}.{parts[2]}.255"
                    : "255.255.255.255";
                _sendSocket.SendTo(data, new IPEndPoint(IPAddress.Parse(bc), RC_LISTEN_PORT));
            }
            catch { }
        }

        // ── Receive ──────────────────────────────────────────────────────────────
        private void BeginReceive()
        {
            if (_recvSocket == null) return;
            EndPoint ep = new IPEndPoint(IPAddress.Any, 0);
            _recvSocket.BeginReceiveFrom(_recvBuffer, 0, _recvBuffer.Length,
                SocketFlags.None, ref ep, OnReceive, null);
        }

        private void OnReceive(IAsyncResult ar)
        {
            if (!_running) return;
            try
            {
                EndPoint ep  = new IPEndPoint(IPAddress.Any, 0);
                int      len = _recvSocket.EndReceiveFrom(ar, ref ep);
                if (len > 1)
                {
                    byte[] copy = new byte[len];
                    Array.Copy(_recvBuffer, copy, len);
                    BeginInvoke(new Action<byte[]>(ParseCommand), copy);
                }
                BeginReceive();
            }
            catch (ObjectDisposedException) { }
            catch { }
        }

        private void ParseCommand(byte[] data)
        {
            if (data.Length < 2) return;
            int pgn = data[0] | (data[1] << 8);

            switch (pgn)
            {
                case 32500:   // Rate settings (matches Teensy Receive.ino case 32500)
                    if (data.Length >= 14)
                    {
                        _sensor.TargetUPM = (data[3] | (data[4] << 8) | (data[5] << 16)) / 1000.0f;
                        _sensor.MeterCal  = (data[6] | (data[7] << 8) | (data[8] << 16)) / 1000.0f;

                        byte cmd = data[9];
                        if ((cmd & 1) != 0) _accQty = 0;    // reset accumulated quantity

                        _sensor.MasterOn     = (cmd & 0x10) != 0;
                        _sensor.AutoOn       = (cmd & 0x40) != 0;
                        _sensor.ManualAdjust = (short)(data[10] | (data[11] << 8));
                        _sensor.CommTime     = DateTime.Now;
                        UpdateCommandLabels();
                    }
                    break;

                case 32501:   // Relay states
                    if (data.Length >= 10)
                    {
                        _cmdRelays = (ushort)(data[3] | (data[4] << 8));
                        UpdateCommandLabels();
                    }
                    break;

                case 32502:   // PID / control settings (matches Teensy Receive.ino case 32502)
                    if (data.Length >= 24)
                    {
                        _sensor.MaxPWM        = 255.0f * data[3] / 100.0f;
                        _sensor.MinPWM        = 255.0f * data[4] / 100.0f;
                        _sensor.Kp            = data[5] > 0 ? (float)Math.Pow(1.1, data[5] - 120) : 0f;
                        _sensor.Ki            = data[6] > 0 ? (float)Math.Pow(1.1, data[6] - 120) : 0f;
                        _sensor.Deadband      = data[7] / 1000.0f;
                        _sensor.BrakePoint    = data[8];
                        _sensor.PIDslowAdjust = data[9];
                        _sensor.SlewRate      = data[10];
                        _sensor.MaxIntegral   = data[11] / 10.0f;
                        _sensor.PIDtime       = data[18];
                        UpdateCommandLabels();
                    }
                    break;

                case 32700:   // Module config
                    lblCmdConfig.Text      = "Received";
                    lblCmdConfig.ForeColor = Color.Green;
                    break;
            }
        }

        // ── UI helpers ───────────────────────────────────────────────────────────
        private void UpdateCommandLabels()
        {
            lblCmdSetRate.Text  = _sensor.TargetUPM.ToString("F1");
            lblCmdFlowCal.Text  = _sensor.MeterCal.ToString("F3");
            lblCmdMasterOn.Text = _sensor.MasterOn ? "ON" : "off";
            lblCmdAutoOn.Text   = _sensor.AutoOn   ? "ON" : "off";
            char[] relayBits = Convert.ToString(_cmdRelays, 2).PadLeft(16, '0').ToCharArray();
            Array.Reverse(relayBits);
            lblCmdRelays.Text = new string(relayBits);
            lblCmdMasterOn.ForeColor = _sensor.MasterOn ? Color.Green : Color.Gray;
            lblCmdAutoOn.ForeColor   = _sensor.AutoOn   ? Color.Green : Color.Gray;
        }

        private void UpdateSimDisplay()
        {
            lblSimRate.Text  = _sensor.UPM.ToString("F1");
            lblHz.Text       = _sensor.Hz.ToString("F1");
            lblPWM.Text      = ((int)_sensor.PWM).ToString();
            lblValvePos.Text = ((int)_valvePos).ToString();
            lblAccQty.Text   = _accQty.ToString("F1");
        }

        private void UpdateUI()
        {
            bool r = _running;
            btnStart.Enabled    = !r;
            btnStop.Enabled     = r;
            nudModuleID.Enabled = !r;
            nudSensorID.Enabled = !r;
            txtSubnet.Enabled   = !r;
        }

        private void SetStatus(string text, Color color)
        {
            lblStatus.Text      = text;
            lblStatus.ForeColor = color;
        }

        private void btnResetQty_Click(object sender, EventArgs e) { _accQty = 0; }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SaveSettings();
            Stop();
            base.OnFormClosing(e);
        }

        private static float   Clamp(float   v, float   mn, float   mx) => Math.Max(mn, Math.Min(mx, v));
        private static decimal Clamp(decimal v, decimal mn, decimal mx) => Math.Max(mn, Math.Min(mx, v));
    }
}
