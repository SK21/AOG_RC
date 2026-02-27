using System;

namespace RateController.Classes.Can
{
    /// <summary>
    /// CAN frame data structure.
    /// </summary>
    public struct CanFrame
    {
        public uint Id;
        public byte Dlc;
        public byte[] Data;
        public bool IsExtended;

        public CanFrame(uint id, byte[] data)
        {
            Id = id;
            Dlc = (byte)(data?.Length ?? 0);
            Data = data;
            IsExtended = true;
        }
    }

    public class CanFrameEventArgs : EventArgs
    {
        public CanFrame Frame { get; }
        public CanFrameEventArgs(CanFrame frame) { Frame = frame; }
    }

    /// <summary>
    /// Hardware abstraction for CAN bus adapters.
    /// All driver implementations (SLCAN, InnoMaker, PCAN) implement this interface.
    /// </summary>
    public interface ICanInterface : IDisposable
    {
        bool Open(string port, int bitrate);
        void Close();
        bool IsOpen { get; }
        bool Send(CanFrame frame);
        event EventHandler<CanFrameEventArgs> FrameReceived;
    }
}
