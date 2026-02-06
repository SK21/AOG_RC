using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController
{
    using RateController.Classes;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct IO_COUNTERS
    {
        public ulong ReadOperationCount;
        public ulong WriteOperationCount;
        public ulong OtherOperationCount;
        public ulong ReadTransferCount;
        public ulong WriteTransferCount;
        public ulong OtherTransferCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct JOBOBJECT_BASIC_LIMIT_INFORMATION
    {
        public long PerProcessUserTimeLimit;
        public long PerJobUserTimeLimit;
        public int LimitFlags;
        public UIntPtr MinimumWorkingSetSize;
        public UIntPtr MaximumWorkingSetSize;
        public int ActiveProcessLimit;
        public long Affinity;
        public int PriorityClass;
        public int SchedulingClass;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
    {
        public JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
        public IO_COUNTERS IoInfo;
        public UIntPtr ProcessMemoryLimit;
        public UIntPtr JobMemoryLimit;
        public UIntPtr PeakProcessMemoryUsed;
        public UIntPtr PeakJobMemoryUsed;
    }

    public static class ChildProcessTracker
    {
        private static IntPtr jobHandle;

        static ChildProcessTracker()
        {
            jobHandle = CreateJobObject(IntPtr.Zero, null);

            JOBOBJECT_BASIC_LIMIT_INFORMATION info = new JOBOBJECT_BASIC_LIMIT_INFORMATION
            {
                LimitFlags = 0x2000 // JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE
            };

            JOBOBJECT_EXTENDED_LIMIT_INFORMATION extendedInfo = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION
            {
                BasicLimitInformation = info
            };

            int length = Marshal.SizeOf(typeof(JOBOBJECT_EXTENDED_LIMIT_INFORMATION));
            IntPtr extendedInfoPtr = Marshal.AllocHGlobal(length);
            Marshal.StructureToPtr(extendedInfo, extendedInfoPtr, false);

            SetInformationJobObject(jobHandle, 9, extendedInfoPtr, (uint)length);
        }

        public static void AddProcess(Process process)
        {
            AssignProcessToJobObject(jobHandle, process.Handle);
        }

        [DllImport("kernel32.dll")]
        private static extern bool AssignProcessToJobObject(IntPtr job, IntPtr process);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr CreateJobObject(IntPtr lpJobAttributes, string name);

        [DllImport("kernel32.dll")]
        private static extern bool SetInformationJobObject(IntPtr job, int infoType, IntPtr lpJobObjectInfo, uint cbJobObjectInfoLength);
    }

    /// <summary>
    /// Manages ISOBUS Gateway process and UDP communication
    /// </summary>
    public class IsobusComm : IDisposable
    {
        public const byte SPEED_SOURCE_GROUND = 1;

        public const byte SPEED_SOURCE_MACHINE_SELECTED = 3;

        public const byte SPEED_SOURCE_NAVIGATION = 2;

        // Speed sources from gateway
        public const byte SPEED_SOURCE_WHEEL = 0;

        public const byte STATUS_ADDRESS_CLAIMED = 0x02;

        // Status flags
        public const byte STATUS_CAN_CONNECTED = 0x01;

        public const byte STATUS_SPEED_VALID = 0x08;

        public const byte STATUS_TC_CONNECTED = 0x04;

        // UDP Ports matching gateway Protocol.hpp
        private const int GatewayListenPort = 32700;  // Gateway receives from RC

        private const int GatewaySendPort = 32701;    // Gateway sends to RC

        private const byte PGN_ACTUAL_RATE_HI = 0x7F;

        private const byte PGN_ACTUAL_RATE_LO = 0x59;

        private const byte PGN_GATEWAY_STATUS_HI = 0x7F;

        private const byte PGN_GATEWAY_STATUS_LO = 0x5D;

        // Task Controller PGNs (matching gateway Protocol.hpp)
        private const int PGN_IMPLEMENT_CONNECTED = 32610;

        // 0x7F62
        private const int PGN_IMPLEMENT_DISCONNECTED = 32611;

        private const byte PGN_ISOBUS_RATE_HI = 0x7F;

        private const byte PGN_ISOBUS_RATE_LO = 0x58;

        private const byte PGN_ISOBUS_SPEED_HI = 0x7F;

        // 32600 = 0x7F58
        // 32601 = 0x7F59
        private const byte PGN_ISOBUS_SPEED_LO = 0x5C;

        private const byte PGN_MODULE_STATUS_HI = 0x7E;

        private const byte PGN_MODULE_STATUS_LO = 0x91;

        private const byte PGN_SENSOR_DATA_HI = 0x7E;

        // PGN Headers (matching gateway Protocol.hpp)
        private const byte PGN_SENSOR_DATA_LO = 0x90;      // 32400 = 0x7E90

        // 0x7F63
        private const int PGN_TC_STATUS = 32617;               // 0x7F69

        // PGN traffic log for diagnostics display
        private string cLog = "";

        private System.Collections.Generic.List<ConnectedImplement> connectedImplements =
            new System.Collections.Generic.List<ConnectedImplement>();

        private bool disposed = false;

        // 32401 = 0x7E91
        // 32604 = 0x7F5C
        // 32605 = 0x7F5D
        private Process gatewayProcess;

        private byte gatewayStatus = 0;
        private byte isobusAddress = 0xFE;

        // Speed data
        private double isobusSpeed_KMH = 0;

        private DateTime lastModuleDataTime = DateTime.MinValue;
        private DateTime lastSpeedTime = DateTime.MinValue;

        // Gateway status
        private DateTime lastStatusTime = DateTime.MinValue;

        private DateTime lastTCStatusTime = DateTime.MinValue;
        private byte[] receiveBuffer = new byte[256];
        private Socket receiveSocket;
        private IPEndPoint sendEndPoint;
        private Socket sendSocket;
        private byte speedSource = 0;
        private bool speedValid = false;
        private byte tcActiveCount = 0;
        private byte tcAddress = 0xFF;
        private byte tcConnectedCount = 0;

        // Number of connected implements
        // Number of active implements
        private byte tcErrorCode = 0;

        // Task Controller Server state
        private byte tcState = 0;

        private bool udpRunning = false;
        private byte vtAddress = 0xFF;

        // 0=Idle, 1=Active, 2=Error
        // TC error code

        #region Nested Types

        /// <summary>
        /// Represents a connected ISOBUS implement (TC Client)
        /// </summary>
        public class ConnectedImplement
        {
            public ConnectedImplement()
            {
                Designation = "";
                ConnectedTime = DateTime.Now;
            }

            public byte Address { get; set; }         // ISOBUS address
            public DateTime ConnectedTime { get; set; }
            public string Designation { get; set; }
            public byte Index { get; set; }           // Assigned index
            public ulong IsoName { get; set; }        // 64-bit ISO NAME
                                                      // Implement name/description
        }

        #endregion Nested Types

        #region Properties

        /// <summary>
        /// True if gateway has claimed an ISOBUS address
        /// </summary>
        public bool AddressClaimed
        {
            get { return (gatewayStatus & STATUS_ADDRESS_CLAIMED) != 0; }
        }

        /// <summary>
        /// True if gateway reports CAN hardware connected
        /// </summary>
        public bool CANConnected
        {
            get { return (gatewayStatus & STATUS_CAN_CONNECTED) != 0; }
        }

        /// <summary>
        /// List of connected ISOBUS implements
        /// </summary>
        public System.Collections.Generic.IReadOnlyList<ConnectedImplement> ConnectedImplements
        {
            get { return connectedImplements.AsReadOnly(); }
        }

        /// <summary>
        /// True if gateway process is running and responding
        /// </summary>
        public bool GatewayConnected
        {
            get { return (DateTime.Now - lastStatusTime).TotalSeconds < 4; }
        }

        /// <summary>
        /// Claimed ISOBUS address (0xFE if not claimed)
        /// </summary>
        public byte IsobusAddress
        {
            get { return isobusAddress; }
        }

        /// <summary>
        /// True if module data (PGN 32400/32401) received within last 2 seconds
        /// This indicates actual ISOBUS communication with modules
        /// </summary>
        public bool ModuleDataReceiving
        {
            get { return (DateTime.Now - lastModuleDataTime).TotalSeconds < 2; }
        }

        /// <summary>
        /// Current ISOBUS speed in km/h
        /// </summary>
        public double Speed_KMH
        {
            get { return SpeedValid ? isobusSpeed_KMH : 0; }
        }

        /// <summary>
        /// ISOBUS speed source type
        /// </summary>
        public byte SpeedSource
        {
            get { return speedSource; }
        }

        /// <summary>
        /// True if ISOBUS speed is valid and recent
        /// </summary>
        public bool SpeedValid
        {
            get { return speedValid && (DateTime.Now - lastSpeedTime).TotalSeconds < 2; }
        }

        /// <summary>
        /// True if Task Controller is connected
        /// </summary>
        public bool TaskControllerConnected
        {
            get { return (gatewayStatus & STATUS_TC_CONNECTED) != 0; }
        }

        /// <summary>
        /// Number of active implements (DDOP activated)
        /// </summary>
        public byte TCActiveCount
        {
            get { return tcActiveCount; }
        }

        /// <summary>
        /// Task Controller address (0xFF if not connected)
        /// </summary>
        public byte TCAddress
        {
            get { return tcAddress; }
        }

        /// <summary>
        /// Number of connected implements
        /// </summary>
        public byte TCConnectedCount
        {
            get { return tcConnectedCount; }
        }

        /// <summary>
        /// TC Server error code (0 = none)
        /// </summary>
        public byte TCErrorCode
        {
            get { return tcErrorCode; }
        }

        /// <summary>
        /// True if TC Server status is being received
        /// </summary>
        public bool TCServerActive
        {
            get { return (DateTime.Now - lastTCStatusTime).TotalSeconds < 4; }
        }

        /// <summary>
        /// TC Server state: 0=Idle, 1=Active, 2=Error
        /// </summary>
        public byte TCState
        {
            get { return tcState; }
        }

        /// <summary>
        /// Returns PGN traffic log for diagnostics display
        /// </summary>
        public string Log()
        {
            return cLog;
        }

        #endregion Properties

        #region Gateway Process Management

        /// <summary>
        /// Start the ISOBUS gateway process
        /// </summary>
        public bool StartGateway()
        {
            try
            {
                if (gatewayProcess != null && !gatewayProcess.HasExited)
                {
                    return true; // Already running
                }

                string gatewayPath = GetGatewayPath();
                if (string.IsNullOrEmpty(gatewayPath) || !File.Exists(gatewayPath))
                {
                    Props.WriteErrorLog("IsobusComm: Gateway executable not found at " + gatewayPath);
                    return false;
                }

                // Update gateway config with current driver setting
                UpdateGatewayConfig();

                string workingDir = Path.GetDirectoryName(gatewayPath);

                gatewayProcess = new Process();
                gatewayProcess.StartInfo.FileName = gatewayPath;
                gatewayProcess.StartInfo.WorkingDirectory = workingDir;

                if (Props.ShowCanDiagnostics)
                {
                    // Show console window for diagnostics
                    gatewayProcess.StartInfo.UseShellExecute = true;
                    gatewayProcess.StartInfo.CreateNoWindow = false;
                }
                else
                {
                    // Hide console window - UseShellExecute must be false for CreateNoWindow to work
                    gatewayProcess.StartInfo.UseShellExecute = false;
                    gatewayProcess.StartInfo.CreateNoWindow = true;
                }

                gatewayProcess.EnableRaisingEvents = true;
                gatewayProcess.Exited += GatewayProcess_Exited;

                if (gatewayProcess.Start())
                {
                    ChildProcessTracker.AddProcess(gatewayProcess);
                    Props.WriteActivityLog("ISOBUS Gateway started", true, true);
                    return true;
                }
                else
                {
                    Props.WriteErrorLog("IsobusComm: Failed to start gateway process");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("IsobusComm/StartGateway: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Stop the ISOBUS gateway process
        /// </summary>
        public void StopGateway()
        {
            try
            {
                if (gatewayProcess != null)
                {
                    if (!gatewayProcess.HasExited)
                    {
                        gatewayProcess.Kill();
                        gatewayProcess.WaitForExit(2000);
                    }
                    gatewayProcess.Dispose();
                    Props.WriteActivityLog("ISOBUS Gateway stopped", true, true);
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("IsobusComm/StopGateway: " + ex.Message);
            }
            gatewayProcess = null;  // Always reset, even if exception occurred
        }

        private void GatewayProcess_Exited(object sender, EventArgs e)
        {
            Props.WriteActivityLog("ISOBUS Gateway process exited", true, true);
        }

        private string GetGatewayPath()
        {
            // Look for gateway executable in application directory
            string appDir = AppDomain.CurrentDomain.BaseDirectory;
            string gatewayPath = Path.Combine(appDir, "IsobusGateway.exe");

            if (File.Exists(gatewayPath))
                return gatewayPath;

            // Also check subdirectory
            gatewayPath = Path.Combine(appDir, "IsobusGateway", "IsobusGateway.exe");
            if (File.Exists(gatewayPath))
                return gatewayPath;

            return null;
        }

        private void UpdateGatewayConfig()
        {
            try
            {
                string gatewayPath = GetGatewayPath();
                if (string.IsNullOrEmpty(gatewayPath)) return;

                string configPath = Path.Combine(Path.GetDirectoryName(gatewayPath), "gateway.json");
                if (!File.Exists(configPath)) return;

                string json = File.ReadAllText(configPath);

                // Update driver based on RC setting
                string driverName;
                switch (Props.CurrentCanDriver)
                {
                    case CanDriver.InnoMaker:
                        driverName = "innomaker";
                        break;

                    case CanDriver.PCAN:
                        driverName = "pcan";
                        break;

                    default:
                        driverName = "slcan";
                        break;
                }

                // Update driver field
                json = System.Text.RegularExpressions.Regex.Replace(
                    json,
                    @"""driver"":\s*""[^""]*""",
                    $@"""driver"": ""{driverName}"""
                );

                // Update COM port for SLCAN
                json = System.Text.RegularExpressions.Regex.Replace(
                    json,
                    @"""port"":\s*""[^""]*""",
                    $@"""port"": ""{Props.CanPort}"""
                );

                File.WriteAllText(configPath, json);
                Props.WriteActivityLog($"Gateway config updated: driver={driverName}, port={Props.CanPort}", false, true);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("IsobusComm/UpdateGatewayConfig: " + ex.Message);
            }
        }

        #endregion Gateway Process Management

        #region UDP Communication

        /// <summary>
        /// Send actual rate to gateway for ISOBUS reporting
        /// </summary>
        public void SendActualRate(byte productId, double rate, double quantity, bool sectionOn)
        {
            if (sendSocket == null) return;

            try
            {
                // PGN 32601 structure
                byte[] data = new byte[12];
                data[0] = PGN_ACTUAL_RATE_LO;
                data[1] = PGN_ACTUAL_RATE_HI;
                data[2] = productId;

                ushort rate_x100 = (ushort)(rate * 100);
                data[3] = (byte)(rate_x100 & 0xFF);
                data[4] = (byte)((rate_x100 >> 8) & 0xFF);

                uint qty_x10 = (uint)(quantity * 10);
                data[5] = (byte)(qty_x10 & 0xFF);
                data[6] = (byte)((qty_x10 >> 8) & 0xFF);
                data[7] = (byte)((qty_x10 >> 16) & 0xFF);
                data[8] = (byte)((qty_x10 >> 24) & 0xFF);

                data[9] = (byte)(sectionOn ? 0x01 : 0x00);
                data[10] = 0; // Reserved
                data[11] = CalculateCRC(data, 11);

                sendSocket.SendTo(data, sendEndPoint);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("IsobusComm/SendActualRate: " + ex.Message);
            }
        }

        /// <summary>
        /// Send module command to gateway for CAN transmission
        /// Used for PGN 32500, 32501, 32502, 32504
        /// </summary>
        public void SendModuleCommand(byte[] data)
        {
            if (sendSocket == null || !udpRunning)
            {
                if (Props.ShowCanDiagnostics)
                    Props.WriteActivityLog("IsobusComm: SendModuleCommand skipped - socket=" +
                        (sendSocket == null ? "null" : "ok") + ", running=" + udpRunning);
                return;
            }

            try
            {
                int pgn = data.Length >= 2 ? (data[0] | (data[1] << 8)) : 0;
                AddToLog("               > " + pgn.ToString());
                sendSocket.SendTo(data, sendEndPoint);
                if (Props.ShowCanDiagnostics)
                    Props.WriteActivityLog("IsobusComm: Sent PGN " + pgn + " (" + data.Length + " bytes)");
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("IsobusComm/SendModuleCommand: " + ex.Message);
            }
        }

        /// <summary>
        /// Start UDP communication with gateway
        /// </summary>
        public bool StartUDP()
        {
            // Check if already running
            if (udpRunning)
            {
                return true;
            }

            int maxRetries = 3;
            int retryDelayMs = 2000;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    // Create send socket FIRST (before async receive starts)
                    sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    sendEndPoint = new IPEndPoint(IPAddress.Loopback, GatewayListenPort);

                    // Create receive socket - bind to localhost specifically for Windows compatibility
                    receiveSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    receiveSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    receiveSocket.Bind(new IPEndPoint(IPAddress.Loopback, GatewaySendPort));

                    // Set flag AFTER sockets are ready, BEFORE starting async receive
                    udpRunning = true;

                    // Start async receive
                    EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                    receiveSocket.BeginReceiveFrom(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None,
                        ref remoteEP, new AsyncCallback(ReceiveCallback), null);

                    Props.WriteActivityLog("ISOBUS UDP started - Listen: " + GatewaySendPort + ", Send: " + GatewayListenPort, false, true);
                    return true;
                }
                catch (Exception ex)
                {
                    // Clean up and retry
                    udpRunning = false;
                    try { sendSocket?.Close(); sendSocket?.Dispose(); } catch { }
                    try { receiveSocket?.Close(); receiveSocket?.Dispose(); } catch { }
                    sendSocket = null;
                    receiveSocket = null;

                    if (attempt < maxRetries)
                    {
                        Props.WriteErrorLog("IsobusComm/StartUDP: Port " + GatewaySendPort + " error, retry " + attempt + "/" + maxRetries + "... " + ex.Message);
                        System.Threading.Thread.Sleep(retryDelayMs);
                    }
                    else
                    {
                        Props.WriteErrorLog("IsobusComm/StartUDP: " + ex.Message);

                        // Check for port blocked errors - requires computer restart
                        if (ex.Message.Contains("Only one usage") || ex.Message.Contains("forbidden") || ex.Message.Contains("permissions"))
                        {
                            System.Windows.Forms.MessageBox.Show(
                                "ISOBUS network port " + GatewaySendPort + " is blocked.\n\n" +
                                "Please restart your computer to fix this issue.",
                                "Network Error",
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Warning);
                            Environment.Exit(1);
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Stop UDP communication
        /// </summary>
        public void StopUDP()
        {
            udpRunning = false;  // Stop async loop first

            // Brief delay to let async callback complete
            System.Threading.Thread.Sleep(100);

            // Close receive socket (no Shutdown needed for UDP - connectionless)
            try
            {
                if (receiveSocket != null)
                {
                    receiveSocket.Close();
                    receiveSocket.Dispose();
                }
            }
            catch { }
            receiveSocket = null;

            try
            {
                if (sendSocket != null)
                {
                    sendSocket.Close();
                    sendSocket.Dispose();
                }
            }
            catch { }
            sendSocket = null;

            // Reset status since we're no longer receiving
            lastStatusTime = DateTime.MinValue;
            lastModuleDataTime = DateTime.MinValue;
            gatewayStatus = 0;
            cLog = "";

            // Reset TC state
            lastTCStatusTime = DateTime.MinValue;
            tcState = 0;
            tcConnectedCount = 0;
            tcActiveCount = 0;
            tcErrorCode = 0;
            connectedImplements.Clear();
        }

        private void AddToLog(string NewData)
        {
            cLog += DateTime.Now.Second.ToString() + "  " + NewData + Environment.NewLine;
            if (cLog.Length > 100000)
            {
                cLog = cLog.Substring(cLog.Length - 98000, 98000);
            }
            cLog = cLog.Replace("\0", string.Empty);
        }

        private byte CalculateCRC(byte[] data, int length)
        {
            byte crc = 0;
            for (int i = 0; i < length; i++)
            {
                crc = (byte)(crc + data[i]);
            }
            return crc;
        }

        private void ForwardModuleStatus(byte[] data)
        {
            // Forward PGN 32401 to ModulesStatus (same as UDPcomm.HandleData)
            try
            {
                lastModuleDataTime = DateTime.Now;  // Track actual ISOBUS data receipt
                Core.ModulesStatus.ParseByteData(data);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("IsobusComm/ForwardModuleStatus: " + ex.Message);
            }
        }

        private void ForwardSensorData(byte[] data)
        {
            // Forward PGN 32400 to all products (same as UDPcomm.HandleData)
            try
            {
                lastModuleDataTime = DateTime.Now;  // Track actual ISOBUS data receipt
                foreach (clsProduct prod in Core.Products.Items)
                {
                    if (!Core.IsShuttingDown)
                    {
                        prod.UDPcommFromArduino(data, 32400);
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("IsobusComm/ForwardSensorData: " + ex.Message);
            }
        }

        private void ParseImplementConnected(byte[] data)
        {
            // PGN 32610 structure:
            // 0-1: PGN (0x62, 0x7F)
            // 2: Address
            // 3: Index
            // 4-11: ISO NAME (8 bytes, little-endian)
            // 12+: Designation string (null-terminated)
            // Last byte: CRC

            if (data.Length >= 13)
            {
                byte address = data[2];
                byte index = data[3];

                // Parse ISO NAME (64-bit, little-endian)
                ulong isoName = 0;
                for (int i = 0; i < 8; i++)
                {
                    isoName |= ((ulong)data[4 + i]) << (i * 8);
                }

                // Parse designation string (starts at byte 12)
                string designation = "";
                if (data.Length > 13)
                {
                    int strLen = 0;
                    for (int i = 12; i < data.Length - 1 && data[i] != 0; i++)
                    {
                        strLen++;
                    }
                    if (strLen > 0)
                    {
                        designation = System.Text.Encoding.UTF8.GetString(data, 12, strLen);
                    }
                }

                // Check if this implement is already in the list
                var existing = connectedImplements.Find(impl => impl.Address == address);
                if (existing != null)
                {
                    // Update existing entry
                    existing.Index = index;
                    existing.IsoName = isoName;
                    existing.Designation = designation;
                }
                else
                {
                    // Add new implement
                    connectedImplements.Add(new ConnectedImplement
                    {
                        Address = address,
                        Index = index,
                        IsoName = isoName,
                        Designation = designation,
                        ConnectedTime = DateTime.Now
                    });
                }

                Props.WriteActivityLog($"ISOBUS Implement Connected: Addr=0x{address:X2}, Index={index}, Name={designation}", true, true);
            }
        }

        private void ParseImplementDisconnected(byte[] data)
        {
            // PGN 32611 structure:
            // 0-1: PGN (0x63, 0x7F)
            // 2: Address
            // 3: Index
            // 4: Reason (0=normal, 1=timeout, 2=error)
            // 5-6: Reserved
            // 7: CRC

            if (data.Length >= 8)
            {
                byte address = data[2];
                byte index = data[3];
                byte reason = data[4];

                // Remove from list
                connectedImplements.RemoveAll(impl => impl.Address == address);

                string reasonStr = reason == 0 ? "normal" : (reason == 1 ? "timeout" : "error");
                Props.WriteActivityLog($"ISOBUS Implement Disconnected: Addr=0x{address:X2}, Index={index}, Reason={reasonStr}", true, true);
            }
        }

        private void ParseRateMessage(byte[] data)
        {
            // PGN 32600 - Target rate from Task Controller
            // For future TC client implementation
            // Currently just log that we received it

            if (data.Length >= 8)
            {
                byte productId = data[2];
                ushort rate_x100 = (ushort)(data[3] | (data[4] << 8));
                double rate = rate_x100 / 100.0;

                // TODO: Apply TC rate to product if TC control is enabled
            }
        }

        private void ParseSpeedMessage(byte[] data)
        {
            // PGN 32604 structure:
            // 0-1: PGN (0x60, 0x7F)
            // 2-3: Speed (mm/s, uint16)
            // 4: Source
            // 5: Flags (bit 0 = valid)
            // 6: Reserved
            // 7: CRC

            if (data.Length >= 8)
            {
                ushort speed_mm_s = (ushort)(data[2] | (data[3] << 8));
                speedSource = data[4];
                speedValid = (data[5] & 0x01) != 0;

                // Convert mm/s to km/h: (mm/s) * 3600 / 1000000 = (mm/s) * 0.0036
                isobusSpeed_KMH = speed_mm_s * 0.0036;
                lastSpeedTime = DateTime.Now;
            }
        }

        private void ParseStatusMessage(byte[] data)
        {
            // PGN 32605 structure:
            // 0-1: PGN (0x65, 0x7F)
            // 2: Status flags
            // 3: Gateway ISOBUS address
            // 4: TC address
            // 5: VT address
            // 6: Reserved
            // 7: CRC

            if (data.Length >= 8)
            {
                gatewayStatus = data[2];
                isobusAddress = data[3];
                tcAddress = data[4];
                vtAddress = data[5];
                lastStatusTime = DateTime.Now;
            }
        }

        private void ParseTCStatus(byte[] data)
        {
            // PGN 32617 structure:
            // 0-1: PGN (0x69, 0x7F)
            // 2: State (0=Idle, 1=Active, 2=Error)
            // 3: Connected count
            // 4: Active count
            // 5: Error code
            // 6: Reserved
            // 7: CRC

            if (data.Length >= 8)
            {
                tcState = data[2];
                tcConnectedCount = data[3];
                tcActiveCount = data[4];
                tcErrorCode = data[5];
                lastTCStatusTime = DateTime.Now;
            }
        }


        private void ProcessMessage(byte[] data)
        {
            if (data.Length < 2 || Core.IsShuttingDown) return;

            byte pgnLo = data[0];
            byte pgnHi = data[1];
            int pgn = (pgnHi << 8) | pgnLo;
            AddToLog("< " + pgn.ToString());
            if (Props.ShowCanDiagnostics)
                Props.WriteActivityLog("ISOBUS UDP Rx: PGN " + pgn + " len=" + data.Length);

            if (pgnLo == PGN_SENSOR_DATA_LO && pgnHi == PGN_SENSOR_DATA_HI)
            {
                // PGN 32400 - Sensor Data (from module via gateway)
                ForwardSensorData(data);
            }
            else if (pgnLo == PGN_MODULE_STATUS_LO && pgnHi == PGN_MODULE_STATUS_HI)
            {
                // PGN 32401 - Module Status (from module via gateway)
                ForwardModuleStatus(data);
            }
            else if (pgnLo == PGN_ISOBUS_SPEED_LO && pgnHi == PGN_ISOBUS_SPEED_HI)
            {
                // PGN 32604 - ISOBUS Speed
                ParseSpeedMessage(data);
            }
            else if (pgnLo == PGN_GATEWAY_STATUS_LO && pgnHi == PGN_GATEWAY_STATUS_HI)
            {
                // PGN 32605 - Gateway Status
                ParseStatusMessage(data);
            }
            else if (pgnLo == PGN_ISOBUS_RATE_LO && pgnHi == PGN_ISOBUS_RATE_HI)
            {
                // PGN 32600 - ISOBUS Rate (from Task Controller)
                ParseRateMessage(data);
            }
            else if (pgn == PGN_IMPLEMENT_CONNECTED)
            {
                // PGN 32610 - Implement Connected
                ParseImplementConnected(data);
            }
            else if (pgn == PGN_IMPLEMENT_DISCONNECTED)
            {
                // PGN 32611 - Implement Disconnected
                ParseImplementDisconnected(data);
            }
            else if (pgn == PGN_TC_STATUS)
            {
                // PGN 32617 - TC Server Status
                ParseTCStatus(data);
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                if (!udpRunning || receiveSocket == null || Core.IsShuttingDown) return;

                EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                int bytesReceived = receiveSocket.EndReceiveFrom(ar, ref remoteEP);

                if (bytesReceived > 0)
                {
                    if (Props.ShowCanDiagnostics)
                        Props.WriteActivityLog("ISOBUS UDP: Received " + bytesReceived + " bytes");
                    byte[] data = new byte[bytesReceived];
                    Array.Copy(receiveBuffer, data, bytesReceived);
                    ProcessMessage(data);
                }

                // Continue receiving only if still running
                if (udpRunning && receiveSocket != null)
                {
                    remoteEP = new IPEndPoint(IPAddress.Any, 0);
                    receiveSocket.BeginReceiveFrom(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None,
                        ref remoteEP, new AsyncCallback(ReceiveCallback), null);
                }
            }
            catch (ObjectDisposedException)
            {
                // Socket closed, ignore
            }
            catch (SocketException)
            {
                // Socket error during shutdown, ignore
            }
            catch (Exception ex)
            {
                if (udpRunning)  // Only log if we're supposed to be running
                {
                    Props.WriteErrorLog("IsobusComm/ReceiveCallback: " + ex.Message);
                }
            }
        }

        #endregion UDP Communication

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    StopUDP();
                    StopGateway();
                }
                disposed = true;
            }
        }

        #endregion IDisposable
    }
}