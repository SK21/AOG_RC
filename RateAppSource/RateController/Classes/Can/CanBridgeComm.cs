using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RateController.Classes.Can
{
    /// <summary>
    /// Provides direct CAN bus communication with Teensy modules using proprietary
    /// frames 0xFF00-0xFF0B at 250 kbps. No subprocess required.
    ///
    /// Threading:
    ///   CAN receive runs on the driver's background thread (SerialPort.DataReceived or poll loop).
    ///   All RC handler calls are marshalled to the UI thread via MainForm.BeginInvoke.
    ///
    /// Module discovery:
    ///   Modules are considered connected when 0xFF08 (ident) is first received.
    ///   Modules are considered disconnected when 0xFF08 is absent for > 2 seconds.
    /// </summary>
    public class CanBridgeComm : IDisposable
    {
        private ICanInterface _can;
        private CanFrameTranslator _translator;
        private System.Timers.Timer _timeoutTimer;
        private volatile bool _running = false;
        private bool _disposed = false;

        private DateTime _lastModuleDataTime = DateTime.MinValue;
        private DateTime _lastFrameTime = DateTime.MinValue;
        private string _cLog = "";

        private readonly List<CanModule> _connectedModules = new List<CanModule>();
        private readonly object _modulesLock = new object();

        public IReadOnlyList<CanModule> ConnectedModules
        {
            get { lock (_modulesLock) { return _connectedModules.ToArray(); } }
        }

        /// <summary>Raised on UI thread when a module comes online (first 0xFF08 received).</summary>
        public event EventHandler<CanModule> ModuleConnected;

        /// <summary>Raised on UI thread when a module's heartbeat times out.</summary>
        public event EventHandler<CanModule> ModuleDisconnected;

        /// <summary>True if CAN adapter is open and at least one frame received within 4 seconds.</summary>
        public bool CanAdapterConnected => _running && (DateTime.Now - _lastFrameTime).TotalSeconds < 4;

        /// <summary>True if PGN 32400 or 32401 assembled within the last 2 seconds.</summary>
        public bool ModuleDataReceiving => (DateTime.Now - _lastModuleDataTime).TotalSeconds < 2;

        public DateTime LastModuleDataTime => _lastModuleDataTime;

        /// <summary>Returns PGN traffic log for diagnostics display.</summary>
        public string Log() => _cLog;

        public bool Start(CanDriver driver, string port)
        {
            Stop();
            try
            {
                _translator = new CanFrameTranslator();
                _translator.PGN32400Ready += OnPGN32400Ready;
                _translator.PGN32401Ready += OnPGN32401Ready;
                _translator.ModuleConnected += OnModuleConnected;
                _translator.ModuleDisconnected += OnModuleDisconnected;

                switch (driver)
                {
                    case CanDriver.InnoMaker:
                        _can = new InnoMakerInterface();
                        break;
                    case CanDriver.PCAN:
                        _can = new PcanInterface();
                        break;
                    default:
                        _can = new SlcanInterface();
                        break;
                }

                _can.FrameReceived += OnFrameReceived;

                if (!_can.Open(port, 250000))
                {
                    Props.WriteErrorLog("CanBridgeComm: Failed to open CAN adapter: driver=" + driver + " port=" + port);
                    _can.FrameReceived -= OnFrameReceived;
                    _can.Dispose();
                    _can = null;
                    return false;
                }

                _running = true;

                _timeoutTimer = new System.Timers.Timer(500);
                _timeoutTimer.Elapsed += (s, e) => { try { _translator?.CheckTimeouts(); } catch { } };
                _timeoutTimer.AutoReset = true;
                _timeoutTimer.Enabled = true;

                Props.WriteActivityLog("CAN Bridge started: driver=" + driver + " port=" + port, true, true);
                return true;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("CanBridgeComm/Start: " + ex.Message);
                return false;
            }
        }

        public void Stop()
        {
            _running = false;

            _timeoutTimer?.Dispose();
            _timeoutTimer = null;

            if (_can != null)
            {
                _can.FrameReceived -= OnFrameReceived;
                _can.Close();
                _can.Dispose();
                _can = null;
            }

            if (_translator != null)
            {
                _translator.PGN32400Ready -= OnPGN32400Ready;
                _translator.PGN32401Ready -= OnPGN32401Ready;
                _translator.ModuleConnected -= OnModuleConnected;
                _translator.ModuleDisconnected -= OnModuleDisconnected;
                _translator = null;
            }

            _lastModuleDataTime = DateTime.MinValue;
            _lastFrameTime = DateTime.MinValue;
            lock (_modulesLock) { _connectedModules.Clear(); }
            _cLog = "";

            Props.WriteActivityLog("CAN Bridge stopped", true, true);
        }

        /// <summary>
        /// Send a PGN byte array to modules via CAN.
        /// Called by PGN32500, PGN32501, PGN32502, PGN32504 Send() methods.
        /// </summary>
        public void SendModuleCommand(byte[] data)
        {
            if (!_running || _can == null || data == null) return;
            try
            {
                int pgn = data.Length >= 2 ? (data[0] | (data[1] << 8)) : 0;
                AddToLog("               > " + pgn);

                var frames = _translator.TranslateOutbound(data);
                foreach (var frame in frames)
                    _can.Send(frame);

                if (Props.ShowCanDiagnostics && frames.Count > 0)
                    Props.WriteActivityLog("CAN TX: PGN " + pgn + " → " + frames.Count + " frame(s)");
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("CanBridgeComm/SendModuleCommand: " + ex.Message);
            }
        }

        // --- CAN receive handlers (called on driver background thread) ---

        private void OnFrameReceived(object sender, CanFrameEventArgs e)
        {
            _lastFrameTime = DateTime.Now;
            try { _translator?.ProcessFrame(e.Frame); }
            catch (Exception ex) { Props.WriteErrorLog("CanBridgeComm/OnFrameReceived: " + ex.Message); }
        }

        private void OnPGN32400Ready(object sender, PgnAssembledEventArgs e)
        {
            _lastModuleDataTime = DateTime.Now;
            AddToLog("< 32400");
            InvokeOnMainForm(() =>
            {
                if (Core.IsShuttingDown) return;
                foreach (clsProduct prod in Core.Products.Items)
                    prod.UDPcommFromArduino(e.Data, 32400);
            });
        }

        private void OnPGN32401Ready(object sender, PgnAssembledEventArgs e)
        {
            _lastModuleDataTime = DateTime.Now;
            AddToLog("< 32401");
            InvokeOnMainForm(() =>
            {
                if (!Core.IsShuttingDown)
                    Core.ModulesStatus.ParseByteData(e.Data);
            });
        }

        private void OnModuleConnected(object sender, int moduleId)
        {
            InvokeOnMainForm(() =>
            {
                lock (_modulesLock)
                {
                    if (_connectedModules.Find(m => m.ModuleId == moduleId) == null)
                    {
                        var module = new CanModule { ModuleId = (byte)moduleId, LastSeen = DateTime.Now };
                        _connectedModules.Add(module);
                        ModuleConnected?.Invoke(this, module);
                        Props.WriteActivityLog("CAN Module connected: ID=" + moduleId, true, true);
                    }
                    else
                    {
                        var m = _connectedModules.Find(x => x.ModuleId == moduleId);
                        if (m != null) m.LastSeen = DateTime.Now;
                    }
                }
            });
        }

        private void OnModuleDisconnected(object sender, int moduleId)
        {
            InvokeOnMainForm(() =>
            {
                lock (_modulesLock)
                {
                    var module = _connectedModules.Find(m => m.ModuleId == moduleId);
                    if (module != null)
                    {
                        _connectedModules.Remove(module);
                        ModuleDisconnected?.Invoke(this, module);
                        Props.WriteActivityLog("CAN Module disconnected: ID=" + moduleId, true, true);
                    }
                }
            });
        }

        // --- UI thread marshalling ---

        private void InvokeOnMainForm(Action action)
        {
            var form = Core.MainForm;
            if (form == null || form.IsDisposed || !form.IsHandleCreated)
            {
                // No form yet (startup), run directly
                action();
                return;
            }
            if (form.InvokeRequired)
                form.BeginInvoke(action);
            else
                action();
        }

        private void AddToLog(string entry)
        {
            _cLog += DateTime.Now.Second + "  " + entry + Environment.NewLine;
            if (_cLog.Length > 100000)
                _cLog = _cLog.Substring(_cLog.Length - 98000);
            _cLog = _cLog.Replace("\0", string.Empty);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Stop();
                _disposed = true;
            }
        }
    }

    public class CanModule
    {
        public byte ModuleId { get; set; }
        public byte ModuleType { get; set; }
        public ushort FirmwareVersion { get; set; }
        public DateTime LastSeen { get; set; }
    }
}
