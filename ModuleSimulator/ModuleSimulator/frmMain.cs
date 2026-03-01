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
        private const float  FastAdjustValve  = 40.0f;
        private const byte   HDR32400_LO = 144, HDR32400_HI = 126;
        private const byte   HDR32401_LO = 145, HDR32401_HI = 126;
        private const ushort INO_ID      = 27026;
        private const byte   INO_TYPE    = 1;
        private const int    MOD_LISTEN_PORT = 28888;
        private const int    RC_LISTEN_PORT  = 29999;
        private const int    N = 5;

        private static readonly string ConfigPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ModuleSimulator", "settings.ini");

        private readonly Product[] Products    = new Product[N];
        private readonly byte[]    _recvBuffer = new byte[256];
        private readonly Random    _rng        = new Random();
        private DateTime _lastLoopTime    = DateTime.MinValue;
        private DateTime _lastDisplayTime = DateTime.MinValue;
        private Socket   _recvSocket;
        private Socket   _sendSocket;
        private volatile bool _running;
        private int      _activeProduct;   // 0-4

        // ── Product: settings + simulation state per product ─────────────────────
        private class Product
        {
            // User-configurable settings
            public bool Enabled;
            public int  ModuleID;
            public int  SensorID;
            public int  MaxHz    = 50;
            public int  Noise    = 5;
            public int  ValveMS  = 5000;
            public int  Speed    = 10;
            public int  Pressure = 100;
            public bool WorkSwitch;

            // Simulation outputs (display only)
            public float  UPM, Hz, PWM, LastPWM;
            public double Valve;       // 0–255
            public double Quantity;

            // Commands received from RC
            public float    TargetUPM, MeterCal;
            public bool     MasterOn, AutoOn;
            public short    ManualAdjust;
            public ushort   CmdRelays;
            public DateTime CommTime = DateTime.MinValue;

            // PID config (from PGN 32502)
            public float MaxPWM = 200f, MinPWM = 10f;
            public float Kp     = (float)Math.Pow(1.1, 100 - 120);
            public float Ki     = 0f;
            public float Deadband    = 0.05f;
            public int   BrakePoint  = 20, PIDslowAdjust = 50, PIDtime = 100;
            public float MaxIntegral = 2.0f;

            // PID runtime state
            public float    IntegralSum;
            public bool     ErrorIsPositive = true;
            public bool     FlowEnabled;
            public DateTime LastPIDCheck = DateTime.MinValue;
        }

        // ── Constructor ──────────────────────────────────────────────────────────
        public frmMain()
        {
            for (int i = 0; i < N; i++) Products[i] = new Product { ModuleID = i };
            Products[0].Enabled = true;
            InitializeComponent();
            SetupControls();
            HighlightProductButton(0);
            ProductToUI();
            LoadSettings();
            EnsureOnScreen();
        }

        // Configure nud ranges and wire live-edit events (Designer doesn't set ranges)
        private void SetupControls()
        {
            ModuleID.Minimum = 0;   ModuleID.Maximum = 127;  ModuleID.Value = 0;
            SensorID.Minimum = 0;   SensorID.Maximum = 15;   SensorID.Value = 0;
            MaxHz.Minimum    = 1;   MaxHz.Maximum    = 500;   MaxHz.Value    = 50;
            Noise.Minimum    = 0;   Noise.Maximum    = 100;   Noise.Value    = 5;
            ValveMS.Minimum  = 500; ValveMS.Maximum  = 30000; ValveMS.Value  = 5000;
            ValveMS.Increment = 500;
            Speed.Minimum    = 0;   Speed.Maximum    = 100;   Speed.Value    = 10;
            Pressure.Minimum = 0;   Pressure.Maximum = 500;   Pressure.Value = 100;

            lbRelays.Font = new Font("Courier New", 8F);

            // Live-update the active product whenever the user edits a control
            ModuleID.ValueChanged  += (s, e) => Products[_activeProduct].ModuleID   = (int)ModuleID.Value;
            SensorID.ValueChanged  += (s, e) => Products[_activeProduct].SensorID   = (int)SensorID.Value;
            MaxHz.ValueChanged     += (s, e) => Products[_activeProduct].MaxHz      = (int)MaxHz.Value;
            Noise.ValueChanged     += (s, e) => Products[_activeProduct].Noise      = (int)Noise.Value;
            ValveMS.ValueChanged   += (s, e) => Products[_activeProduct].ValveMS    = (int)ValveMS.Value;
            Speed.ValueChanged     += (s, e) => Products[_activeProduct].Speed      = (int)Speed.Value;
            Pressure.ValueChanged  += (s, e) => Products[_activeProduct].Pressure   = (int)Pressure.Value;
            ckWork.CheckedChanged  += (s, e) => Products[_activeProduct].WorkSwitch = ckWork.Checked;
            ckEnabled.CheckedChanged += (s, e) => Products[_activeProduct].Enabled  = ckEnabled.Checked;
        }

        // ── Product ↔ UI binding ──────────────────────────────────────────────────
        private void ProductToUI()
        {
            var p = Products[_activeProduct];
            ckEnabled.Checked = p.Enabled;
            ModuleID.Value    = Clamp((decimal)p.ModuleID, ModuleID.Minimum, ModuleID.Maximum);
            SensorID.Value    = Clamp((decimal)p.SensorID, SensorID.Minimum, SensorID.Maximum);
            MaxHz.Value       = Clamp((decimal)p.MaxHz,    MaxHz.Minimum,    MaxHz.Maximum);
            Noise.Value       = Clamp((decimal)p.Noise,    Noise.Minimum,    Noise.Maximum);
            ValveMS.Value     = Clamp((decimal)p.ValveMS,  ValveMS.Minimum,  ValveMS.Maximum);
            Speed.Value       = Clamp((decimal)p.Speed,    Speed.Minimum,    Speed.Maximum);
            Pressure.Value    = Clamp((decimal)p.Pressure, Pressure.Minimum, Pressure.Maximum);
            ckWork.Checked    = p.WorkSwitch;
            UpdateProductDisplay(_activeProduct);
            UpdateCommandLabels(_activeProduct);
        }

        // ── Product button clicks ─────────────────────────────────────────────────
        private void SwitchProduct(int idx)
        {
            _activeProduct = idx;
            HighlightProductButton(idx);
            ProductToUI();
        }

        private void HighlightProductButton(int idx)
        {
            btn1.BackColor = idx == 0 ? Color.LightSteelBlue : SystemColors.Control;
            btn2.BackColor = idx == 1 ? Color.LightSteelBlue : SystemColors.Control;
            btn3.BackColor = idx == 2 ? Color.LightSteelBlue : SystemColors.Control;
            btn4.BackColor = idx == 3 ? Color.LightSteelBlue : SystemColors.Control;
            btn5.BackColor = idx == 4 ? Color.LightSteelBlue : SystemColors.Control;
        }

        private void btn1_Click(object sender, EventArgs e) => SwitchProduct(0);
        private void btn2_Click(object sender, EventArgs e) => SwitchProduct(1);
        private void btn3_Click(object sender, EventArgs e) => SwitchProduct(2);
        private void btn4_Click(object sender, EventArgs e) => SwitchProduct(3);
        private void btn5_Click(object sender, EventArgs e) => SwitchProduct(4);

        // ── Settings persistence ──────────────────────────────────────────────────
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
                    int    prod = 0;
                    string bare = key;
                    if (key.Length > 2 && key[1] == '.')
                    {
                        prod = key[0] - '0';
                        bare = key.Substring(2);
                        if (prod < 0 || prod >= N) continue;
                    }
                    ApplySetting(prod, bare, val);
                }
            }
            catch { }
            ProductToUI();   // reload UI for active product after loading settings
        }

        private void ApplySetting(int p, string key, string val)
        {
            int iv;
            switch (key)
            {
                case "Subnet":     if (p == 0) txtSubnet.Text = val; break;
                case "Left":       if (p == 0 && int.TryParse(val, out iv)) Left = iv; break;
                case "Top":        if (p == 0 && int.TryParse(val, out iv)) Top  = iv; break;
                case "Enabled":    Products[p].Enabled    = val == "1"; break;
                case "ModuleID":   if (int.TryParse(val, out iv)) Products[p].ModuleID   = Math.Max(0,   Math.Min(127,   iv)); break;
                case "SensorID":   if (int.TryParse(val, out iv)) Products[p].SensorID   = Math.Max(0,   Math.Min(15,    iv)); break;
                case "MaxHz":      if (int.TryParse(val, out iv)) Products[p].MaxHz      = Math.Max(1,   Math.Min(500,   iv)); break;
                case "Noise":      if (int.TryParse(val, out iv)) Products[p].Noise      = Math.Max(0,   Math.Min(100,   iv)); break;
                case "ValveLag":   if (int.TryParse(val, out iv)) Products[p].ValveMS    = Math.Max(500, Math.Min(30000, iv)); break;
                case "WheelSpeed": if (int.TryParse(val, out iv)) Products[p].Speed      = Math.Max(0,   Math.Min(100,   iv)); break;
                case "Pressure":   if (int.TryParse(val, out iv)) Products[p].Pressure   = Math.Max(0,   Math.Min(500,   iv)); break;
                case "WorkSwitch": Products[p].WorkSwitch = val == "1"; break;
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
                for (int i = 0; i < N; i++)
                {
                    var p  = Products[i];
                    string px = i + ".";
                    lines.Add(px + "Enabled="    + (p.Enabled    ? "1" : "0"));
                    lines.Add(px + "ModuleID="   + p.ModuleID);
                    lines.Add(px + "SensorID="   + p.SensorID);
                    lines.Add(px + "MaxHz="      + p.MaxHz);
                    lines.Add(px + "Noise="      + p.Noise);
                    lines.Add(px + "ValveLag="   + p.ValveMS);
                    lines.Add(px + "WheelSpeed=" + p.Speed);
                    lines.Add(px + "Pressure="   + p.Pressure);
                    lines.Add(px + "WorkSwitch=" + (p.WorkSwitch ? "1" : "0"));
                }
                File.WriteAllLines(ConfigPath, lines);
            }
            catch { }
        }

        // ── Start / Stop ─────────────────────────────────────────────────────────
        private void btnStart_Click(object sender, EventArgs e) => Start();
        private void btnStop_Click(object sender, EventArgs e)  => Stop();

        private void Start()
        {
            if (_running) return;
            try
            {
                _lastLoopTime    = DateTime.MinValue;
                _lastDisplayTime = DateTime.MinValue;
                for (int i = 0; i < N; i++) ResetProduct(i);
                UpdateCommandLabels(_activeProduct);

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

        private void ResetProduct(int i)
        {
            var p = Products[i];
            p.Valve = 0; p.Quantity = 0; p.CmdRelays = 0;
            p.TargetUPM = 0; p.MeterCal = 0; p.MasterOn = false; p.AutoOn = false;
            p.CommTime = DateTime.MinValue;
            p.PWM = 0; p.LastPWM = 0; p.IntegralSum = 0;
            p.Hz = 0; p.UPM = 0;
            p.ErrorIsPositive = true;
            p.LastPIDCheck = DateTime.MinValue;
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

        // ── Loop tick (50 ms) ─────────────────────────────────────────────────────
        private void loopTimer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            double dt = (_lastLoopTime == DateTime.MinValue)
                ? 0.05 : (now - _lastLoopTime).TotalSeconds;
            if (dt > 0.2) dt = 0.05;
            _lastLoopTime = now;

            for (int i = 0; i < N; i++)
            {
                if (!Products[i].Enabled) continue;
                SetFlowEnabled(i, now);
                UpdatePWM(i, now);
                SimulateFlow(i, dt);
            }
            if ((now - _lastDisplayTime).TotalSeconds >= 1.0)
            {
                _lastDisplayTime = now;
                UpdateProductDisplay(_activeProduct);
            }
        }

        // ── Send tick (200 ms) ────────────────────────────────────────────────────
        private void sendTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < N; i++)
            {
                if (!Products[i].Enabled) continue;
                var p = Products[i];
                p.Quantity += p.UPM * (200.0 / 60000.0);
                SendPGN32401(i);
                SendPGN32400(i);
            }
        }

        // ── Simulation ───────────────────────────────────────────────────────────
        private void SetFlowEnabled(int i, DateTime now)
        {
            var p = Products[i];
            bool result = false;
            if (p.CommTime != DateTime.MinValue && (now - p.CommTime).TotalSeconds < 5.0)
            {
                if (p.TargetUPM > 0 && p.MasterOn) result = true;
                else if (p.MasterOn && !p.AutoOn)   result = true;
            }
            p.FlowEnabled = result;
        }

        private void UpdatePWM(int i, DateTime now)
        {
            var p = Products[i];
            if (p.AutoOn)
                p.PWM = PIDvalve(p, now);
            else if (p.FlowEnabled)
                p.PWM = Math.Sign(p.ManualAdjust)
                        * Math.Min(Math.Abs((float)p.ManualAdjust), p.MaxPWM);
            else
                p.PWM = 0;
        }

        private float PIDvalve(Product p, DateTime now)
        {
            float result = p.LastPWM;
            if (p.FlowEnabled && p.TargetUPM > 0)
            {
                if ((now - p.LastPIDCheck).TotalMilliseconds >= p.PIDtime)
                {
                    p.LastPIDCheck = now;
                    float rateError  = p.TargetUPM - p.UPM;
                    bool  isPositive = rateError > 0;
                    if (isPositive != p.ErrorIsPositive)
                    { p.ErrorIsPositive = isPositive; p.IntegralSum = 0; }

                    if (Math.Abs(rateError) > p.Deadband * p.TargetUPM)
                    {
                        rateError     = Clamp(rateError, -p.TargetUPM, p.TargetUPM);
                        p.IntegralSum += rateError * p.Ki;
                        if (p.Ki <= 0) p.IntegralSum = 0;
                        p.IntegralSum = Clamp(p.IntegralSum, -p.MaxIntegral, p.MaxIntegral);

                        float brakeFactor = Math.Abs(rateError) > p.TargetUPM * p.BrakePoint / 100.0f
                            ? FastAdjustValve
                            : p.PIDslowAdjust / 100.0f * FastAdjustValve;

                        float changeAmount = rateError * p.Kp * brakeFactor * 100.0f + p.IntegralSum;
                        if (Math.Abs(changeAmount) < 0.1f)
                            result = 0f;
                        else
                        {
                            result  = Clamp(Math.Abs(changeAmount) + p.MinPWM, p.MinPWM, p.MaxPWM);
                            result *= changeAmount >= 0f ? 1f : -1f;
                        }
                    }
                    else { result = 0f; p.IntegralSum = 0f; }
                }
            }
            else { p.IntegralSum = 0; result = 0; }
            p.LastPWM = result;
            return result;
        }

        private void SimulateFlow(int i, double dt)
        {
            var p = Products[i];
            if (!p.FlowEnabled || p.CmdRelays == 0)
            {
                p.Valve = 0;
                p.Hz    = 0;
            }
            else
            {
                double travelTime = Math.Max(0.1, p.ValveMS / 1000.0);
                double maxRate    = 255.0 / travelTime;
                double speed      = p.MaxPWM > 0 ? (p.PWM / p.MaxPWM) * maxRate * dt : 0;
                p.Valve = Math.Max(0.0, Math.Min(255.0, p.Valve + speed));

                double idealHz = (p.Valve / 255.0) * p.MaxHz;
                double u       = _rng.NextDouble() + _rng.NextDouble() - 1.0;
                double noise   = p.Noise / 100.0;
                p.Hz = (float)(Math.Max(0.0, idealHz + idealHz * noise * u) * 0.8 + p.Hz * 0.2);
            }
            p.UPM = p.MeterCal > 0 ? (float)(60.0 * p.Hz / p.MeterCal) : 0f;
        }

        // ── UDP senders ───────────────────────────────────────────────────────────
        private void SendPGN32400(int i)
        {
            var    p   = Products[i];
            byte[] d   = new byte[15];
            d[0] = HDR32400_LO; d[1] = HDR32400_HI;
            d[2] = (byte)((p.ModuleID << 4) | (p.SensorID & 0x0F));
            int r = (int)(p.UPM * 1000.0);
            d[3] = (byte)r; d[4] = (byte)(r >> 8); d[5] = (byte)(r >> 16);
            int q = (int)(p.Quantity * 10.0);
            d[6] = (byte)q; d[7] = (byte)(q >> 8); d[8] = (byte)(q >> 16);
            int pwm = (int)p.PWM;
            d[9] = (byte)pwm; d[10] = (byte)(pwm >> 8);
            d[11] = 0x01;
            int hz = (int)(p.Hz * 10.0);
            d[12] = (byte)hz; d[13] = (byte)(hz >> 8);
            d[14] = CalcCRC(d, 14);
            UdpSend(d);
        }

        private void SendPGN32401(int i)
        {
            var    p   = Products[i];
            byte[] d   = new byte[15];
            d[0] = HDR32401_LO; d[1] = HDR32401_HI;
            d[2] = (byte)p.ModuleID;
            ushort pressure = (ushort)p.Pressure;
            d[3] = (byte)pressure; d[4] = (byte)(pressure >> 8);
            ushort ws = (ushort)(p.Speed * 10);
            d[5] = (byte)ws; d[6] = (byte)(ws >> 8);
            d[7] = d[8] = d[9] = 0;
            d[10] = INO_TYPE;
            d[11] = (byte)(INO_ID & 0xFF);
            d[12] = (byte)(INO_ID >> 8);
            d[13] = 0b0011_0000;
            if (p.WorkSwitch) d[13] |= 0x01;
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
            int moduleId = data[2] >> 4;

            switch (pgn)
            {
                case 32500:
                    if (data.Length < 14) break;
                    int m500 = FindProduct(moduleId);
                    if (m500 < 0) break;
                    {
                        var p      = Products[m500];
                        p.TargetUPM    = (data[3] | (data[4] << 8) | (data[5] << 16)) / 1000.0f;
                        p.MeterCal     = (data[6] | (data[7] << 8) | (data[8] << 16)) / 1000.0f;
                        byte cmd       = data[9];
                        if ((cmd & 1) != 0) p.Quantity = 0;
                        p.MasterOn     = (cmd & 0x10) != 0;
                        p.AutoOn       = (cmd & 0x40) != 0;
                        p.ManualAdjust = (short)(data[10] | (data[11] << 8));
                        p.CommTime     = DateTime.Now;
                        if (m500 == _activeProduct) UpdateCommandLabels(m500);
                    }
                    break;

                case 32501:
                    if (data.Length < 10) break;
                    {
                        ushort relays = (ushort)(data[3] | (data[4] << 8));
                        int    m501   = FindProduct(moduleId);
                        if (m501 >= 0)
                        {
                            Products[m501].CmdRelays = relays;
                        }
                        else   // no match → broadcast to all enabled products
                        {
                            for (int ii = 0; ii < N; ii++)
                                if (Products[ii].Enabled) Products[ii].CmdRelays = relays;
                        }
                        UpdateCommandLabels(_activeProduct);
                    }
                    break;

                case 32502:
                    if (data.Length < 24) break;
                    int m502 = FindProduct(moduleId);
                    if (m502 < 0) break;
                    {
                        var p = Products[m502];
                        p.MaxPWM        = 255.0f * data[3] / 100.0f;
                        p.MinPWM        = 255.0f * data[4] / 100.0f;
                        p.Kp            = data[5] > 0 ? (float)Math.Pow(1.1, data[5] - 120) : 0f;
                        p.Ki            = data[6] > 0 ? (float)Math.Pow(1.1, data[6] - 120) : 0f;
                        p.Deadband      = data[7] / 1000.0f;
                        p.BrakePoint    = data[8];
                        p.PIDslowAdjust = data[9];
                        p.MaxIntegral   = data[11] / 10.0f;
                        p.PIDtime       = data[18];
                    }
                    break;

                case 32700:
                    lbConfig.Text      = "Received";
                    lbConfig.ForeColor = Color.Green;
                    break;
            }
        }

        private int FindProduct(int moduleId)
        {
            for (int i = 0; i < N; i++)
                if (Products[i].Enabled && Products[i].ModuleID == moduleId)
                    return i;
            return -1;
        }

        // ── UI update helpers ─────────────────────────────────────────────────────
        private void UpdateProductDisplay(int i)
        {
            var p = Products[i];
            lbUPM.Text      = p.UPM.ToString("F1");
            lbHz.Text       = p.Hz.ToString("F1");
            lbPWM.Text      = ((int)p.PWM).ToString();
            lbValve.Text    = ((int)p.Valve).ToString();
            lbQuantity.Text = p.Quantity.ToString("F1");
        }

        private void UpdateCommandLabels(int i)
        {
            var p = Products[i];
            lbSetRate.Text       = p.TargetUPM.ToString("F1");
            lbFlowCal.Text       = p.MeterCal.ToString("F3");
            lbMaster.Text        = p.MasterOn ? "ON" : "off";
            lbAuto.Text          = p.AutoOn   ? "ON" : "off";
            lbMaster.ForeColor   = p.MasterOn ? Color.Green : Color.Gray;
            lbAuto.ForeColor     = p.AutoOn   ? Color.Green : Color.Gray;
            char[] bits = Convert.ToString(p.CmdRelays, 2).PadLeft(16, '0').ToCharArray();
            Array.Reverse(bits);
            string s = new string(bits);
            lbRelays.Text = s.Substring(0, 4) + " " + s.Substring(4, 4) + " "
                          + s.Substring(8, 4) + " " + s.Substring(12, 4);
        }

        private void UpdateUI()
        {
            bool r = _running;
            btnStart.Enabled  = !r;
            btnStop.Enabled   = r;
            txtSubnet.Enabled = !r;
            ModuleID.Enabled  = !r;
            SensorID.Enabled  = !r;
        }

        private void SetStatus(string text, Color color)
        {
            lblStatus.Text      = text;
            lblStatus.ForeColor = color;
        }

        private void btnResetQty_Click(object sender, EventArgs e)
        {
            Products[_activeProduct].Quantity = 0;
        }

        // ── GroupBox custom paint (wired to all groups in Designer) ───────────────
        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            if (box == null) return;
            DrawGroupBox(box, e.Graphics, BackColor, Color.Black, Color.SteelBlue, 1.5f);
        }

        private static void DrawGroupBox(GroupBox box, Graphics g, Color backColor,
            Color textColor, Color borderColor, float borderWidth)
        {
            using (Brush textBrush = new SolidBrush(textColor))
            using (Pen   borderPen = new Pen(borderColor, borderWidth))
            {
                SizeF strSize = g.MeasureString(box.Text, box.Font);
                Rectangle rect = new Rectangle(
                    box.ClientRectangle.X,
                    box.ClientRectangle.Y + (int)(strSize.Height / 2),
                    box.ClientRectangle.Width  - 1,
                    box.ClientRectangle.Height - (int)(strSize.Height / 2) - 1);
                g.Clear(backColor);
                g.DrawString(box.Text, box.Font, textBrush, box.Padding.Left, 0);
                g.DrawLine(borderPen, rect.Location, new Point(rect.X, rect.Y + rect.Height));
                g.DrawLine(borderPen, new Point(rect.X + rect.Width, rect.Y),
                                      new Point(rect.X + rect.Width, rect.Y + rect.Height));
                g.DrawLine(borderPen, new Point(rect.X, rect.Y + rect.Height),
                                      new Point(rect.X + rect.Width, rect.Y + rect.Height));
                g.DrawLine(borderPen, new Point(rect.X, rect.Y),
                                      new Point(rect.X + box.Padding.Left, rect.Y));
                g.DrawLine(borderPen, new Point(rect.X + box.Padding.Left + (int)strSize.Width, rect.Y),
                                      new Point(rect.X + rect.Width, rect.Y));
            }
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
                if (s.WorkingArea.Contains(new Point(Left + 20, Top + 20))) return;
            Left = Screen.PrimaryScreen.WorkingArea.Left + 40;
            Top  = Screen.PrimaryScreen.WorkingArea.Top  + 40;
        }

        private static byte  CalcCRC(byte[] data, int length)
        {
            int sum = 0;
            for (int i = 0; i < length; i++) sum += data[i];
            return (byte)sum;
        }

        private static float    Clamp(float    v, float    mn, float    mx) => Math.Max(mn, Math.Min(mx, v));
        private static decimal  Clamp(decimal  v, decimal  mn, decimal  mx) => Math.Max(mn, Math.Min(mx, v));
    }
}
