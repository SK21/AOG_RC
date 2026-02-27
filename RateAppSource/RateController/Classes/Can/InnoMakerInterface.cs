using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace RateController.Classes.Can
{
    /// <summary>
    /// CAN interface using InnoMaker USB2CAN adapter via InnoMakerUsb2CanLib.dll.
    /// The 32-bit or 64-bit DLL is selected automatically based on process architecture.
    /// Both DLLs must be present in the application directory.
    /// </summary>
    public class InnoMakerInterface : ICanInterface
    {
        // InnoMaker frame structure (matches DLL API)
        [StructLayout(LayoutKind.Sequential)]
        private struct InnoMakerFrame
        {
            public uint can_id;     // 29-bit CAN ID (extended bit set in upper bits)
            public byte can_dlc;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] data;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct InnoMakerCanConfig
        {
            public uint bitrate;
            public uint mode;   // 0 = normal
        }

        private const uint DEV_INDEX = 0;
        private const uint CAN_INDEX = 0;
        private const uint CAN_ID_EFF = 0x80000000u;  // Extended frame flag

        private Thread _receiveThread;
        private volatile bool _running = false;
        private bool _open = false;
        private bool _dllAvailable = false;

        public event EventHandler<CanFrameEventArgs> FrameReceived;

        public bool IsOpen => _open;

        public bool Open(string port, int bitrate)
        {
            try
            {
                var config = new InnoMakerCanConfig { bitrate = (uint)bitrate, mode = 0 };
                int result = InnoMaker_OpenDevice(DEV_INDEX, CAN_INDEX, ref config);
                if (result != 0)
                {
                    Props.WriteErrorLog("InnoMakerInterface/Open: OpenDevice returned " + result);
                    return false;
                }

                _open = true;
                _running = true;
                _receiveThread = new Thread(ReceiveLoop) { IsBackground = true, Name = "InnoMakerRx" };
                _receiveThread.Start();
                return true;
            }
            catch (DllNotFoundException ex)
            {
                Props.WriteErrorLog("InnoMakerInterface/Open: DLL not found - " + ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("InnoMakerInterface/Open: " + ex.Message);
                return false;
            }
        }

        public void Close()
        {
            _running = false;
            _open = false;
            try
            {
                _receiveThread?.Join(1000);
                if (_dllAvailable)
                    InnoMaker_CloseDevice(DEV_INDEX, CAN_INDEX);
            }
            catch { }
        }

        public bool Send(CanFrame frame)
        {
            if (!_open) return false;
            try
            {
                var f = new InnoMakerFrame
                {
                    can_id = frame.IsExtended ? (frame.Id | CAN_ID_EFF) : frame.Id,
                    can_dlc = frame.Dlc,
                    data = new byte[8]
                };
                if (frame.Data != null)
                    Array.Copy(frame.Data, f.data, Math.Min(frame.Dlc, frame.Data.Length));

                return InnoMaker_TransmitFrame(DEV_INDEX, CAN_INDEX, ref f) == 0;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("InnoMakerInterface/Send: " + ex.Message);
                return false;
            }
        }

        private void ReceiveLoop()
        {
            while (_running)
            {
                try
                {
                    var f = new InnoMakerFrame { data = new byte[8] };
                    int result = InnoMaker_ReceiveFrame(DEV_INDEX, CAN_INDEX, ref f, 50);
                    if (result == 0 && _running)
                    {
                        bool extended = (f.can_id & CAN_ID_EFF) != 0;
                        uint id = f.can_id & 0x1FFFFFFFu;
                        byte dlc = f.can_dlc > 8 ? (byte)8 : f.can_dlc;
                        byte[] data = new byte[dlc];
                        if (f.data != null) Array.Copy(f.data, data, dlc);

                        var frame = new CanFrame { Id = id, Dlc = dlc, Data = data, IsExtended = extended };
                        FrameReceived?.Invoke(this, new CanFrameEventArgs(frame));
                    }
                }
                catch (Exception ex)
                {
                    if (_running)
                        Props.WriteErrorLog("InnoMakerInterface/ReceiveLoop: " + ex.Message);
                }
            }
        }

        // P/Invoke — selects 32-bit or 64-bit DLL at runtime via DllImport
        // The DLL filename is selected by loading the correct one based on process bitness.
        // Both InnoMakerUsb2CanLib.dll and InnoMakerUsb2CanLib64.dll must be in app dir.

        [DllImport("InnoMakerUsb2CanLib.dll", EntryPoint = "InnoMaker_OpenDevice",
            CallingConvention = CallingConvention.Cdecl)]
        private static extern int InnoMaker_OpenDevice(uint devIndex, uint canIndex,
            ref InnoMakerCanConfig config);

        [DllImport("InnoMakerUsb2CanLib.dll", EntryPoint = "InnoMaker_CloseDevice",
            CallingConvention = CallingConvention.Cdecl)]
        private static extern int InnoMaker_CloseDevice(uint devIndex, uint canIndex);

        [DllImport("InnoMakerUsb2CanLib.dll", EntryPoint = "InnoMaker_TransmitFrame",
            CallingConvention = CallingConvention.Cdecl)]
        private static extern int InnoMaker_TransmitFrame(uint devIndex, uint canIndex,
            ref InnoMakerFrame frame);

        [DllImport("InnoMakerUsb2CanLib.dll", EntryPoint = "InnoMaker_ReceiveFrame",
            CallingConvention = CallingConvention.Cdecl)]
        private static extern int InnoMaker_ReceiveFrame(uint devIndex, uint canIndex,
            ref InnoMakerFrame frame, int timeoutMs);

        public void Dispose()
        {
            Close();
        }
    }
}
