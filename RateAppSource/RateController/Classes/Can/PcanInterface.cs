using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace RateController.Classes.Can
{
    /// <summary>
    /// CAN interface for Peak PCAN adapters via PCANBasic.dll.
    /// The DLL is installed by the PEAK driver package (PCAN-Basic) into System32 (64-bit)
    /// and SysWOW64 (32-bit); Windows resolves the matching one for the process, so nothing
    /// needs to ship with the application. If the driver is not installed, Open() logs a
    /// DllNotFoundException and returns false.
    ///
    /// Channel is fixed to PCAN_USBBUS1 — the COM port box in frmMenuComm is disabled for
    /// anything but SLCAN, so the port argument carries no usable channel information.
    /// </summary>
    public class PcanInterface : ICanInterface
    {
        // PCANBasic message structure (TPCANMsg)
        [StructLayout(LayoutKind.Sequential)]
        private struct TPCANMsg
        {
            public uint ID;         // 11-bit or 29-bit CAN ID
            public byte MSGTYPE;    // TPCANMessageType bit field
            public byte LEN;        // Data length (0-8)
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] DATA;
        }

        // PCANBasic timestamp structure (TPCANTimestamp) — required by CAN_Read, not used here
        [StructLayout(LayoutKind.Sequential)]
        private struct TPCANTimestamp
        {
            public uint millis;
            public ushort millis_overflow;
            public ushort micros;
        }

        private const ushort PCAN_USBBUS1 = 0x51;

        // TPCANBaudrate values (BTR0BTR1 register pairs)
        private const ushort PCAN_BAUD_1M = 0x0014;
        private const ushort PCAN_BAUD_500K = 0x001C;
        private const ushort PCAN_BAUD_250K = 0x011C;
        private const ushort PCAN_BAUD_125K = 0x031C;
        private const ushort PCAN_BAUD_100K = 0x432F;
        private const ushort PCAN_BAUD_50K = 0x472F;

        // TPCANMessageType flags
        private const byte PCAN_MESSAGE_EXTENDED = 0x02;
        private const byte PCAN_MESSAGE_STATUS = 0x80;
        private const byte PCAN_MESSAGE_ERRFRAME = 0x40;
        private const byte PCAN_MESSAGE_RTR = 0x01;

        // TPCANStatus values
        private const uint PCAN_ERROR_OK = 0x00000;
        private const uint PCAN_ERROR_QRCVEMPTY = 0x00020;

        private const ushort LANGUAGE_ENGLISH = 0x09;

        private ushort _channel = PCAN_USBBUS1;
        private Thread _receiveThread;
        private volatile bool _running = false;
        private bool _open = false;
        private DateTime _lastBusErrorLog = DateTime.MinValue;

        public event EventHandler<CanFrameEventArgs> FrameReceived;

        public bool IsOpen => _open;

        public bool Open(string port, int bitrate)
        {
            bool Result = false;

            if (_open)
            {
                Result = true;
            }
            else
            {
                try
                {
                    uint status = CAN_Initialize(_channel, BaudrateFromBitrate(bitrate), 0, 0, 0);
                    if (status == PCAN_ERROR_OK)
                    {
                        _open = true;
                        _running = true;
                        _receiveThread = new Thread(ReceiveLoop) { IsBackground = true, Name = "PcanRx" };
                        _receiveThread.Start();
                        Result = true;
                    }
                    else
                    {
                        Props.WriteErrorLog("PcanInterface/Open: Initialize failed - " + StatusText(status));
                    }
                }
                catch (DllNotFoundException ex)
                {
                    Props.WriteErrorLog("PcanInterface/Open: PCANBasic.dll not found, install the PEAK "
                        + "PCAN-Basic driver package - " + ex.Message);
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("PcanInterface/Open: " + ex.Message);
                }
            }

            return Result;
        }

        public void Close()
        {
            bool wasOpen = _open;
            _running = false;
            _open = false;
            try
            {
                // Wait for any pending CAN TX frames to be physically transmitted before
                // uninitializing the channel (same issue as SLCAN and InnoMaker — the driver
                // TX queue may not be flushed before the channel is torn down).
                Thread.Sleep(50);
                _receiveThread?.Join(1000);
                if (wasOpen) CAN_Uninitialize(_channel);
            }
            catch { }
            _receiveThread = null;
        }

        public bool Send(CanFrame frame)
        {
            bool Result = false;

            if (_open)
            {
                try
                {
                    var msg = new TPCANMsg
                    {
                        ID = frame.Id,
                        MSGTYPE = frame.IsExtended ? PCAN_MESSAGE_EXTENDED : (byte)0,
                        LEN = frame.Dlc > 8 ? (byte)8 : frame.Dlc,
                        DATA = new byte[8]
                    };
                    if (frame.Data != null)
                        Array.Copy(frame.Data, msg.DATA, Math.Min(msg.LEN, frame.Data.Length));

                    uint status = CAN_Write(_channel, ref msg);
                    Result = status == PCAN_ERROR_OK;
                    if (!Result) Props.WriteErrorLog("PcanInterface/Send: " + StatusText(status));
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("PcanInterface/Send: " + ex.Message);
                }
            }

            return Result;
        }

        private void ReceiveLoop()
        {
            while (_running)
            {
                try
                {
                    uint status = CAN_Read(_channel, out TPCANMsg msg, out TPCANTimestamp _);

                    if (status == PCAN_ERROR_QRCVEMPTY)
                    {
                        // Queue drained — yield before polling again.
                        Thread.Sleep(1);
                    }
                    else if (status == PCAN_ERROR_OK)
                    {
                        // Status and error frames carry no payload for the translator.
                        bool dataFrame = (msg.MSGTYPE & (PCAN_MESSAGE_STATUS | PCAN_MESSAGE_ERRFRAME | PCAN_MESSAGE_RTR)) == 0;
                        if (dataFrame && _running)
                        {
                            byte dlc = msg.LEN > 8 ? (byte)8 : msg.LEN;
                            byte[] data = new byte[dlc];
                            if (msg.DATA != null) Array.Copy(msg.DATA, data, dlc);

                            var frame = new CanFrame
                            {
                                Id = msg.ID & 0x1FFFFFFFu,
                                Dlc = dlc,
                                Data = data,
                                IsExtended = (msg.MSGTYPE & PCAN_MESSAGE_EXTENDED) != 0
                            };
                            FrameReceived?.Invoke(this, new CanFrameEventArgs(frame));
                        }
                    }
                    else
                    {
                        // Bus error (bus-off, overrun, wrong bitrate, ...). Keep polling — the
                        // adapter recovers on its own — but do not flood the log.
                        if ((DateTime.Now - _lastBusErrorLog).TotalSeconds > 10)
                        {
                            _lastBusErrorLog = DateTime.Now;
                            Props.WriteErrorLog("PcanInterface/ReceiveLoop: " + StatusText(status));
                        }
                        Thread.Sleep(10);
                    }
                }
                catch (Exception ex)
                {
                    if (_running) Props.WriteErrorLog("PcanInterface/ReceiveLoop: " + ex.Message);
                    Thread.Sleep(10);
                }
            }
        }

        /// <summary>Maps a bitrate in bits/second to the PCAN BTR0BTR1 constant. Defaults to 250 kbps.</summary>
        private ushort BaudrateFromBitrate(int bitrate)
        {
            ushort Result;
            switch (bitrate)
            {
                case 1000000: Result = PCAN_BAUD_1M; break;
                case 500000: Result = PCAN_BAUD_500K; break;
                case 250000: Result = PCAN_BAUD_250K; break;
                case 125000: Result = PCAN_BAUD_125K; break;
                case 100000: Result = PCAN_BAUD_100K; break;
                case 50000: Result = PCAN_BAUD_50K; break;
                default:
                    Props.WriteErrorLog("PcanInterface: unsupported bitrate " + bitrate + ", using 250000");
                    Result = PCAN_BAUD_250K;
                    break;
            }
            return Result;
        }

        /// <summary>Returns the driver's description of a TPCANStatus, or the raw code if unavailable.</summary>
        private string StatusText(uint status)
        {
            string Result = "0x" + status.ToString("X");
            try
            {
                var sb = new StringBuilder(256);
                if (CAN_GetErrorText(status, LANGUAGE_ENGLISH, sb) == PCAN_ERROR_OK)
                    Result = sb.ToString() + " (" + Result + ")";
            }
            catch { }
            return Result;
        }

        // P/Invoke — PCANBasic.dll is resolved from System32/SysWOW64 by the PEAK driver install.

        [DllImport("PCANBasic.dll", EntryPoint = "CAN_Initialize", CallingConvention = CallingConvention.Winapi)]
        private static extern uint CAN_Initialize(ushort channel, ushort btr0btr1, byte hwType,
            uint ioPort, ushort interrupt);

        [DllImport("PCANBasic.dll", EntryPoint = "CAN_Uninitialize", CallingConvention = CallingConvention.Winapi)]
        private static extern uint CAN_Uninitialize(ushort channel);

        [DllImport("PCANBasic.dll", EntryPoint = "CAN_Read", CallingConvention = CallingConvention.Winapi)]
        private static extern uint CAN_Read(ushort channel, out TPCANMsg message, out TPCANTimestamp timestamp);

        [DllImport("PCANBasic.dll", EntryPoint = "CAN_Write", CallingConvention = CallingConvention.Winapi)]
        private static extern uint CAN_Write(ushort channel, ref TPCANMsg message);

        [DllImport("PCANBasic.dll", EntryPoint = "CAN_GetErrorText", CallingConvention = CallingConvention.Winapi)]
        private static extern uint CAN_GetErrorText(uint error, ushort language, StringBuilder buffer);

        public void Dispose()
        {
            Close();
        }
    }
}
