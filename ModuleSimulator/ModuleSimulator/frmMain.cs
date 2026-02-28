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
        private const int RC_LISTEN_PORT  = 29999;
        private const int MOD_LISTEN_PORT = 28888;

        private const byte HDR32400_LO = 144, HDR32400_HI = 126;
        private const byte HDR32401_LO = 145, HDR32401_HI = 126;

        private const ushort INO_ID   = 27026;
        private const byte   INO_TYPE = 1;
        private const float  FastAdjustValve = 40.0f;

        private Socket    _sendSocket;
        private Socket    _recvSocket;
        private readonly byte[] _recvBuffer = new byte[256];
        private volatile bool   _running;

        private readonly ModState[] _mod = { new ModState(), new ModState() };
        private DateTime _lastLoopTime = DateTime.MinValue;
        private readonly Random _rng = new Random();

        // ── Per-module simulation state ───────────────────────────────────────────
        private class SensorState
        {
            public float    TargetUPM, MeterCal;
            public bool     MasterOn, AutoOn;
            public int      ControlType;
            public short    ManualAdjust;
            public DateTime CommTime = DateTime.MinValue;

            public float MaxPWM = 200f, MinPWM = 10f;
            public float Kp     = (float)Math.Pow(1.1, 100 - 120);
            public float Ki     = 0f;
            public float Deadband = 0.05f;
            public int   BrakePoint = 20, PIDslowAdjust = 50, SlewRate = 20, PIDtime = 100;
            public float MaxIntegral = 2.0f;

            public float    PWM, LastPWM, IntegralSum;
            public bool     ErrorIsPositive = true;
            public DateTime LastPIDCheck    = DateTime.MinValue;

            public float Hz, UPM;
            public bool  FlowEnabled;
        }

        private class ModState
        {
            public readonly SensorState Sensor = new SensorState();
            public double  ValvePos;
            public double  AccQty;
            public ushort  CmdRelays;
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

                    // Key format: "Subnet"/"Left"/"Top" (shared) or "0.ModuleID"/"1.ModuleID" (per-module)
                    // Backwards compat: bare key (no digit prefix) applies to module 0
                    int mod = 0;
                    string bare = key;
                    if (key.Length > 2 && key[1] == '.')
                    {
                        mod = key[0] - '0';
                        bare = key.Substring(2);
                        if (mod < 0 || mod > 1) continue;
                    }
                    ApplySetting(mod, bare, val);
                }
            }
            catch { }
        }

        private void ApplySetting(int mod, string key, string val)
        {
            decimal d;
            switch (key)
            {
                case "Subnet":     if (mod == 0) txtSubnet.Text = val; break;
                case "Left":       if (mod == 0 && int.TryParse(val, out int lv)) Left = lv; break;
                case "Top":        if (mod == 0 && int.TryParse(val, out int tv)) Top  = tv; break;
                case "ModuleID":   if (decimal.TryParse(val, out d)) nudModuleID[mod].Value   = Clamp(d, nudModuleID[mod].Minimum,   nudModuleID[mod].Maximum);   break;
                case "SensorID":   if (decimal.TryParse(val, out d)) nudSensorID[mod].Value   = Clamp(d, nudSensorID[mod].Minimum,   nudSensorID[mod].Maximum);   break;
                case "MaxHz":      if (decimal.TryParse(val, out d)) nudMaxHz[mod].Value      = Clamp(d, nudMaxHz[mod].Minimum,      nudMaxHz[mod].Maximum);      break;
                case "Noise":      if (decimal.TryParse(val, out d)) nudNoise[mod].Value      = Clamp(d, nudNoise[mod].Minimum,      nudNoise[mod].Maximum);      break;
                case "ValveLag":   if (decimal.TryParse(val, out d)) nudValveLag[mod].Value   = Clamp(d, nudValveLag[mod].Minimum,   nudValveLag[mod].Maximum);   break;
                case "WheelSpeed": if (decimal.TryParse(val, out d)) nudWheelSpeed[mod].Value = Clamp(d, nudWheelSpeed[mod].Minimum, nudWheelSpeed[mod].Maximum); break;
                case "Pressure":   if (decimal.TryParse(val, out d)) nudPressure[mod].Value   = Clamp(d, nudPressure[mod].Minimum,   nudPressure[mod].Maximum);   break;
                case "WorkSwitch": ckWorkSwitch[mod].Checked = val == "1"; break;
                case "Enabled":    if (mod == 1) ckEnable.Checked = val == "1"; break;
            }
        }

        private void SaveSettings()
        {
            try
            {
                string dir = Path.GetDirectoryName(ConfigPath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                var lines = new System.Collections.Generic.List<string>
                {
                    "Subnet=" + txtSubnet.Text.Trim(),
                };
                if (WindowState == FormWindowState.Normal)
                {
                    lines.Add("Left=" + Left);
                    lines.Add("Top="  + Top);
                }
                for (int i = 0; i < 2; i++)
                {
                    string p = i + ".";
                    lines.Add(p + "ModuleID="   + nudModuleID[i].Value);
                    lines.Add(p + "SensorID="   + nudSensorID[i].Value);
                    lines.Add(p + "MaxHz="      + nudMaxHz[i].Value);
                    lines.Add(p + "Noise="      + nudNoise[i].Value);
                    lines.Add(p + "ValveLag="   + nudValveLag[i].Value);
                    lines.Add(p + "WheelSpeed=" + nudWheelSpeed[i].Value);
                    lines.Add(p + "Pressure="   + nudPressure[i].Value);
                    lines.Add(p + "WorkSwitch=" + (ckWorkSwitch[i].Checked ? "1" : "0"));
                }
                lines.Add("1.Enabled=" + (ckEnable.Checked ? "1" : "0"));
                File.WriteAllLines(ConfigPath, lines);
            }
            catch { }
        }

        // ── Constructor ──────────────────────────────────────────────────────────
        public frmMain() { InitializeComponent(); BuildModuleTabs(); LoadSettings(); EnsureOnScreen(); }

        // ── BuildModuleTabs ───────────────────────────────────────────────────────
        private void BuildModuleTabs()
        {
            const int N = 2;
            nudModuleID    = new NumericUpDown[N];
            nudSensorID    = new NumericUpDown[N];
            nudMaxHz       = new NumericUpDown[N];
            nudNoise       = new NumericUpDown[N];
            nudValveLag    = new NumericUpDown[N];
            nudWheelSpeed  = new NumericUpDown[N];
            nudPressure    = new NumericUpDown[N];
            ckWorkSwitch   = new CheckBox[N];
            btnResetQty    = new Button[N];
            lblSimRate     = new Label[N];
            lblHz          = new Label[N];
            lblPWM         = new Label[N];
            lblValvePos    = new Label[N];
            lblAccQty      = new Label[N];
            lblCmdSetRate  = new Label[N];
            lblCmdFlowCal  = new Label[N];
            lblCmdMasterOn = new Label[N];
            lblCmdAutoOn   = new Label[N];
            lblCmdRelays   = new Label[N];
            lblCmdConfig   = new Label[N];

            int[] defModId = { 0, 1 };

            for (int i = 0; i < N; i++)
            {
                var page = new TabPage("Module " + (i + 1));

                // ── Config ────────────────────────────────────────────────────────
                var grpConfig = Grp("Config", 4, 6, 460, 66);
                Lbl(grpConfig, "Module ID:", 8, 24);
                nudModuleID[i] = Nud(grpConfig, 80, 20, 0, 127, defModId[i], 1, 60);
                Lbl(grpConfig, "Sensor ID:", 160, 24);
                nudSensorID[i] = Nud(grpConfig, 232, 20, 0, 15, 0, 1, 55);
                if (i == 1) { ckEnable.Location = new Point(322, 21); grpConfig.Controls.Add(ckEnable); }
                page.Controls.Add(grpConfig);

                // ── Sensor output ─────────────────────────────────────────────────
                var grpSensor = Grp("Sensor Output", 4, 78, 460, 88);
                Lbl(grpSensor, "Rate (UPM):", 8, 22);
                lblSimRate[i] = Val(grpSensor, "0.0", 90, 20, 65);
                lblSimRate[i].Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                Lbl(grpSensor, "Hz:", 168, 22);
                lblHz[i] = Val(grpSensor, "0.0", 188, 20, 58);
                Lbl(grpSensor, "PWM:", 256, 22);
                lblPWM[i] = Val(grpSensor, "0", 294, 20, 55);
                Lbl(grpSensor, "Acc Qty:", 8, 52);
                lblAccQty[i] = Val(grpSensor, "0.0", 66, 50, 65);
                btnResetQty[i] = new Button { Text = "Reset", Location = new Point(148, 46), Size = new Size(58, 24) };
                btnResetQty[i].Click += btnResetQty_Click;
                grpSensor.Controls.Add(btnResetQty[i]);
                Lbl(grpSensor, "Valve:", 230, 52);
                lblValvePos[i] = Val(grpSensor, "0", 274, 50, 55);
                page.Controls.Add(grpSensor);

                // ── Simulation params ─────────────────────────────────────────────
                var grpSim = Grp("Simulation", 4, 172, 460, 90);
                Lbl(grpSim, "Max Hz:", 8, 22);
                nudMaxHz[i] = Nud(grpSim, 68, 20, 1, 500, 100, 1, 72);
                Lbl(grpSim, "Noise %:", 160, 22);
                nudNoise[i] = Nud(grpSim, 224, 20, 0, 100, 5, 1, 60);
                Lbl(grpSim, "Valve ms:", 8, 54);
                nudValveLag[i] = Nud(grpSim, 76, 52, 500, 30000, 5000, 500, 82);
                page.Controls.Add(grpSim);

                // ── Sensor inputs ─────────────────────────────────────────────────
                var grpStatus = Grp("Sensor Inputs", 4, 268, 460, 62);
                Lbl(grpStatus, "Speed:", 8, 22);
                nudWheelSpeed[i] = Nud(grpStatus, 58, 20, 0, 100, 10, 1, 68);
                Lbl(grpStatus, "Pressure:", 148, 22);
                nudPressure[i] = Nud(grpStatus, 214, 20, 0, 500, 100, 1, 68);
                ckWorkSwitch[i] = new CheckBox { Text = "Work Switch", Location = new Point(306, 21), AutoSize = true };
                grpStatus.Controls.Add(ckWorkSwitch[i]);
                page.Controls.Add(grpStatus);

                // ── Commands from RC ──────────────────────────────────────────────
                var grpCmds = Grp("Commands from RC", 4, 336, 460, 130);
                Lbl(grpCmds, "Set Rate:", 8, 22);
                lblCmdSetRate[i]  = Val(grpCmds, "—", 72, 20, 90);
                Lbl(grpCmds, "Flow Cal:", 180, 22);
                lblCmdFlowCal[i]  = Val(grpCmds, "—", 248, 20, 90);
                Lbl(grpCmds, "Master:", 8, 48);
                lblCmdMasterOn[i] = Val(grpCmds, "off", 58, 46, 48);
                lblCmdMasterOn[i].ForeColor = Color.Gray;
                Lbl(grpCmds, "Auto:", 118, 48);
                lblCmdAutoOn[i]   = Val(grpCmds, "off", 152, 46, 48);
                lblCmdAutoOn[i].ForeColor = Color.Gray;
                Lbl(grpCmds, "Relays:", 8, 74);
                lblCmdRelays[i]   = Val(grpCmds, "0000000000000000", 62, 72, 360);
                lblCmdRelays[i].Font = new Font("Courier New", 8F);
                Lbl(grpCmds, "Config:", 8, 100);
                lblCmdConfig[i]   = Val(grpCmds, "—", 62, 98, 120);
                page.Controls.Add(grpCmds);

                tabModules.TabPages.Add(page);
            }
        }

        private static GroupBox Grp(string text, int x, int y, int w, int h)
        {
            return new GroupBox { Text = text, Location = new Point(x, y), Size = new Size(w, h) };
        }

        private static void Lbl(Control parent, string text, int x, int y)
        {
            parent.Controls.Add(new Label { Text = text, Location = new Point(x, y), AutoSize = true });
        }

        private static Label Val(Control parent, string text, int x, int y, int w = 70)
        {
            var l = new Label { Text = text, Location = new Point(x, y), Size = new Size(w, 18) };
            parent.Controls.Add(l);
            return l;
        }

        private static NumericUpDown Nud(Control parent, int x, int y,
            decimal min, decimal max, decimal val, decimal inc, int w = 72)
        {
            var n = new NumericUpDown
            {
                Minimum = min, Maximum = max, Value = val, Increment = inc,
                Location = new Point(x, y), Size = new Size(w, 22)
            };
            parent.Controls.Add(n);
            return n;
        }

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
                _lastLoopTime = DateTime.MinValue;
                for (int i = 0; i < 2; i++) ResetMod(i);
                UpdateCommandLabels(0);
                UpdateCommandLabels(1);

                _sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _sendSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
                _sendSocket.Bind(new IPEndPoint(IPAddress.Any, 0));

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

        private void ResetMod(int i)
        {
            var m = _mod[i]; var s = m.Sensor;
            m.ValvePos = 0; m.AccQty = 0; m.CmdRelays = 0;
            s.TargetUPM = 0; s.MeterCal = 0; s.MasterOn = false; s.AutoOn = false;
            s.CommTime = DateTime.MinValue;
            s.PWM = 0; s.LastPWM = 0; s.IntegralSum = 0;
            s.Hz = 0; s.UPM = 0;
            s.ErrorIsPositive = true;
            s.LastPIDCheck = DateTime.MinValue;
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

        private bool ModuleActive(int i) => i == 0 || ckEnable.Checked;

        // ── Loop tick (50 ms) ─────────────────────────────────────────────────────
        private void loopTimer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            double dt = (_lastLoopTime == DateTime.MinValue)
                ? 0.05 : (now - _lastLoopTime).TotalSeconds;
            if (dt > 0.2) dt = 0.05;
            _lastLoopTime = now;

            for (int i = 0; i < 2; i++)
            {
                if (!ModuleActive(i)) continue;
                SetSensorsEnabled(i, now);
                UpdatePWM(i, now);
                SimulateFlow(i, dt);
            }
            UpdateSimDisplay();
        }

        // ── Send tick (200 ms) ────────────────────────────────────────────────────
        private void sendTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < 2; i++)
            {
                if (!ModuleActive(i)) continue;
                _mod[i].AccQty += _mod[i].Sensor.UPM * (200.0 / 60000.0);
                SendPGN32401(i, (byte)nudModuleID[i].Value);
                SendPGN32400(i, (byte)nudModuleID[i].Value, (byte)nudSensorID[i].Value);
            }
        }

        // ── SetSensorsEnabled ─────────────────────────────────────────────────────
        private void SetSensorsEnabled(int i, DateTime now)
        {
            var s = _mod[i].Sensor;
            bool result = false;
            if (s.CommTime != DateTime.MinValue && (now - s.CommTime).TotalSeconds < 5.0)
            {
                if (s.TargetUPM > 0 && s.MasterOn) result = true;
                else if (s.MasterOn && !s.AutoOn)   result = true;
            }
            s.FlowEnabled = result;
        }

        // ── UpdatePWM ─────────────────────────────────────────────────────────────
        private void UpdatePWM(int i, DateTime now)
        {
            var s = _mod[i].Sensor;
            if (s.AutoOn)
                s.PWM = PIDvalve(s, now);
            else if (s.FlowEnabled)
                s.PWM = Math.Sign(s.ManualAdjust)
                        * Math.Min(Math.Abs((float)s.ManualAdjust), s.MaxPWM);
            else
                s.PWM = 0;
        }

        // ── PIDvalve (port of Teensy PID.ino:PIDvalve) ───────────────────────────
        private float PIDvalve(SensorState s, DateTime now)
        {
            float result = s.LastPWM;
            if (s.FlowEnabled && s.TargetUPM > 0)
            {
                if ((now - s.LastPIDCheck).TotalMilliseconds >= s.PIDtime)
                {
                    s.LastPIDCheck = now;
                    float rateError = s.TargetUPM - s.UPM;
                    bool  isPositive = rateError > 0;
                    if (isPositive != s.ErrorIsPositive)
                    { s.ErrorIsPositive = isPositive; s.IntegralSum = 0; }

                    if (Math.Abs(rateError) > s.Deadband * s.TargetUPM)
                    {
                        rateError = Clamp(rateError, -s.TargetUPM, s.TargetUPM);
                        s.IntegralSum += rateError * s.Ki;
                        if (s.Ki <= 0) s.IntegralSum = 0;
                        s.IntegralSum = Clamp(s.IntegralSum, -s.MaxIntegral, s.MaxIntegral);

                        float brakeFactor = Math.Abs(rateError) > s.TargetUPM * s.BrakePoint / 100.0f
                            ? FastAdjustValve
                            : s.PIDslowAdjust / 100.0f * FastAdjustValve;

                        float changeAmount = rateError * s.Kp * brakeFactor * 100.0f + s.IntegralSum;
                        if (Math.Abs(changeAmount) < 0.1f)
                            result = 0f;
                        else
                        {
                            result  = Clamp(Math.Abs(changeAmount) + s.MinPWM, s.MinPWM, s.MaxPWM);
                            result *= changeAmount >= 0f ? 1f : -1f;
                        }
                    }
                    else { result = 0f; s.IntegralSum = 0f; }
                }
            }
            else { s.IntegralSum = 0; result = 0; }
            s.LastPWM = result;
            return result;
        }

        // ── SimulateFlow ──────────────────────────────────────────────────────────
        private void SimulateFlow(int i, double dt)
        {
            var s = _mod[i].Sensor;
            bool relaysActive = _mod[i].CmdRelays > 0;

            if (!s.FlowEnabled || !relaysActive)
            {
                _mod[i].ValvePos = 0;
                s.Hz = 0;
            }
            else
            {
                double travelTime = Math.Max(0.1, (double)nudValveLag[i].Value / 1000.0);
                double maxRate    = 255.0 / travelTime;
                double speed      = s.MaxPWM > 0 ? (s.PWM / s.MaxPWM) * maxRate * dt : 0;
                _mod[i].ValvePos  = Math.Max(0.0, Math.Min(255.0, _mod[i].ValvePos + speed));

                double maxHz   = (double)nudMaxHz[i].Value;
                double idealHz = (_mod[i].ValvePos / 255.0) * maxHz;
                double u       = _rng.NextDouble() + _rng.NextDouble() - 1.0;
                double noise   = (double)nudNoise[i].Value / 100.0;
                s.Hz = (float)(Math.Max(0.0, idealHz + idealHz * noise * u) * 0.8 + s.Hz * 0.2);
            }
            s.UPM = s.MeterCal > 0 ? (float)(60.0 * s.Hz / s.MeterCal) : 0f;
        }

        // ── PGN senders ──────────────────────────────────────────────────────────
        private void SendPGN32401(int i, byte modId)
        {
            byte[] d = new byte[15];
            d[0] = HDR32401_LO; d[1] = HDR32401_HI;
            d[2] = modId;
            ushort pressure = (ushort)nudPressure[i].Value;
            d[3] = (byte)pressure; d[4] = (byte)(pressure >> 8);
            ushort ws = (ushort)((double)nudWheelSpeed[i].Value * 10.0);
            d[5] = (byte)ws; d[6] = (byte)(ws >> 8);
            d[7] = d[8] = d[9] = 0;
            d[10] = INO_TYPE;
            d[11] = (byte)(INO_ID & 0xFF);
            d[12] = (byte)(INO_ID >> 8);
            d[13] = 0b0011_0000;
            if (ckWorkSwitch[i].Checked) d[13] |= 0x01;
            d[14] = CalcCRC(d, 14);
            UdpSend(d);
        }

        private void SendPGN32400(int i, byte modId, byte senId)
        {
            var s = _mod[i].Sensor;
            byte[] d = new byte[15];
            d[0] = HDR32400_LO; d[1] = HDR32400_HI;
            d[2] = (byte)((modId << 4) | (senId & 0x0F));
            int r = (int)(s.UPM * 1000.0);
            d[3] = (byte)r; d[4] = (byte)(r >> 8); d[5] = (byte)(r >> 16);
            int q = (int)(_mod[i].AccQty * 10.0);
            d[6] = (byte)q; d[7] = (byte)(q >> 8); d[8] = (byte)(q >> 16);
            int pwm = (int)s.PWM;
            d[9] = (byte)pwm; d[10] = (byte)(pwm >> 8);
            d[11] = 0x01;
            int hz = (int)(s.Hz * 10.0);
            d[12] = (byte)hz; d[13] = (byte)(hz >> 8);
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
            if (data.Length < 3) return;
            int pgn      = data[0] | (data[1] << 8);
            int moduleId = data[2] >> 4;   // high nibble = module ID

            switch (pgn)
            {
                case 32500:
                    if (data.Length < 14) break;
                    int m500 = FindModule(moduleId);
                    if (m500 < 0) break;
                    {
                        var s = _mod[m500].Sensor;
                        s.TargetUPM    = (data[3] | (data[4] << 8) | (data[5] << 16)) / 1000.0f;
                        s.MeterCal     = (data[6] | (data[7] << 8) | (data[8] << 16)) / 1000.0f;
                        byte cmd       = data[9];
                        if ((cmd & 1) != 0) _mod[m500].AccQty = 0;
                        s.MasterOn     = (cmd & 0x10) != 0;
                        s.AutoOn       = (cmd & 0x40) != 0;
                        s.ManualAdjust = (short)(data[10] | (data[11] << 8));
                        s.CommTime     = DateTime.Now;
                        UpdateCommandLabels(m500);
                    }
                    break;

                case 32501:
                    if (data.Length < 10) break;
                    {
                        ushort relays = (ushort)(data[3] | (data[4] << 8));
                        int    m501   = FindModule(moduleId);
                        if (m501 >= 0)
                        {
                            _mod[m501].CmdRelays = relays;
                            UpdateCommandLabels(m501);
                        }
                        else   // broadcast — apply to all active modules
                        {
                            for (int ii = 0; ii < 2; ii++)
                                if (ModuleActive(ii)) { _mod[ii].CmdRelays = relays; UpdateCommandLabels(ii); }
                        }
                    }
                    break;

                case 32502:
                    if (data.Length < 24) break;
                    int m502 = FindModule(moduleId);
                    if (m502 < 0) break;
                    {
                        var s = _mod[m502].Sensor;
                        s.MaxPWM        = 255.0f * data[3] / 100.0f;
                        s.MinPWM        = 255.0f * data[4] / 100.0f;
                        s.Kp            = data[5] > 0 ? (float)Math.Pow(1.1, data[5] - 120) : 0f;
                        s.Ki            = data[6] > 0 ? (float)Math.Pow(1.1, data[6] - 120) : 0f;
                        s.Deadband      = data[7] / 1000.0f;
                        s.BrakePoint    = data[8];
                        s.PIDslowAdjust = data[9];
                        s.SlewRate      = data[10];
                        s.MaxIntegral   = data[11] / 10.0f;
                        s.PIDtime       = data[18];
                        UpdateCommandLabels(m502);
                    }
                    break;

                case 32700:
                    for (int ii = 0; ii < 2; ii++)
                    {
                        lblCmdConfig[ii].Text      = "Received";
                        lblCmdConfig[ii].ForeColor = Color.Green;
                    }
                    break;
            }
        }

        private int FindModule(int moduleId)
        {
            for (int i = 0; i < 2; i++)
                if (ModuleActive(i) && (int)nudModuleID[i].Value == moduleId)
                    return i;
            return -1;
        }

        // ── UI helpers ────────────────────────────────────────────────────────────
        private void UpdateCommandLabels(int i)
        {
            var s = _mod[i].Sensor;
            lblCmdSetRate[i].Text  = s.TargetUPM.ToString("F1");
            lblCmdFlowCal[i].Text  = s.MeterCal.ToString("F3");
            lblCmdMasterOn[i].Text = s.MasterOn ? "ON" : "off";
            lblCmdAutoOn[i].Text   = s.AutoOn   ? "ON" : "off";
            lblCmdMasterOn[i].ForeColor = s.MasterOn ? Color.Green : Color.Gray;
            lblCmdAutoOn[i].ForeColor   = s.AutoOn   ? Color.Green : Color.Gray;
            char[] bits = Convert.ToString(_mod[i].CmdRelays, 2).PadLeft(16, '0').ToCharArray();
            Array.Reverse(bits);
            lblCmdRelays[i].Text = new string(bits);
        }

        private void UpdateSimDisplay()
        {
            for (int i = 0; i < 2; i++)
            {
                if (!ModuleActive(i)) continue;
                var s = _mod[i].Sensor;
                lblSimRate[i].Text  = s.UPM.ToString("F1");
                lblHz[i].Text       = s.Hz.ToString("F1");
                lblPWM[i].Text      = ((int)s.PWM).ToString();
                lblValvePos[i].Text = ((int)_mod[i].ValvePos).ToString();
                lblAccQty[i].Text   = _mod[i].AccQty.ToString("F1");
            }
        }

        private void UpdateUI()
        {
            bool r = _running;
            btnStart.Enabled  = !r;
            btnStop.Enabled   = r;
            txtSubnet.Enabled = !r;
            for (int i = 0; i < 2; i++)
            {
                nudModuleID[i].Enabled = !r;
                nudSensorID[i].Enabled = !r;
            }
        }

        private void SetStatus(string text, Color color)
        {
            lblStatus.Text      = text;
            lblStatus.ForeColor = color;
        }

        private void btnResetQty_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 2; i++)
                if (sender == btnResetQty[i]) { _mod[i].AccQty = 0; return; }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SaveSettings();
            Stop();
            base.OnFormClosing(e);
        }

        private void EnsureOnScreen()
        {
            foreach (Screen s in Screen.AllScreens)
                if (s.WorkingArea.Contains(new System.Drawing.Point(Left + 20, Top + 20))) return;
            Left = Screen.PrimaryScreen.WorkingArea.Left + 40;
            Top  = Screen.PrimaryScreen.WorkingArea.Top  + 40;
        }

        private static float   Clamp(float   v, float   mn, float   mx) => Math.Max(mn, Math.Min(mx, v));
        private static decimal Clamp(decimal v, decimal mn, decimal mx) => Math.Max(mn, Math.Min(mx, v));
    }
}
