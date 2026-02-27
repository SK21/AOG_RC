using System;
using System.IO.Ports;
using System.Text;

namespace RateController.Classes.Can
{
    /// <summary>
    /// CAN interface using SLCAN protocol over a serial (COM) port.
    /// Compatible with SH-C30A, Canable, and any SLCAN-firmware USB-CAN adapter.
    /// No native DLL required — uses System.IO.Ports.SerialPort only.
    /// </summary>
    public class SlcanInterface : ICanInterface
    {
        private SerialPort _port;
        private readonly StringBuilder _lineBuffer = new StringBuilder();
        private bool _open = false;

        public event EventHandler<CanFrameEventArgs> FrameReceived;

        public bool IsOpen => _open;

        public bool Open(string portName, int bitrate)
        {
            try
            {
                _port = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);
                _port.ReadTimeout = 500;
                _port.WriteTimeout = 500;
                _port.DataReceived += OnDataReceived;
                _port.Open();

                // Set bitrate before opening channel
                string bitrateCmd;
                if (bitrate >= 1000000) bitrateCmd = "S8";
                else if (bitrate >= 500000) bitrateCmd = "S6";
                else if (bitrate >= 250000) bitrateCmd = "S5";
                else if (bitrate >= 125000) bitrateCmd = "S4";
                else bitrateCmd = "S5";  // default 250k

                _port.Write(bitrateCmd + "\r");
                System.Threading.Thread.Sleep(100);
                _port.Write("O\r");  // Open CAN channel
                System.Threading.Thread.Sleep(100);

                _open = true;
                return true;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("SlcanInterface/Open: " + ex.Message);
                _open = false;
                try { _port?.Dispose(); } catch { }
                _port = null;
                return false;
            }
        }

        public void Close()
        {
            _open = false;
            try
            {
                if (_port?.IsOpen == true)
                {
                    _port.Write("C\r");  // Close CAN channel
                    System.Threading.Thread.Sleep(100);
                    _port.Close();
                }
                _port?.Dispose();
            }
            catch { }
            _port = null;
        }

        public bool Send(CanFrame frame)
        {
            if (!_open || _port == null || !_port.IsOpen) return false;
            try
            {
                string cmd;
                if (frame.IsExtended)
                    cmd = string.Format("T{0:X8}{1:X1}", frame.Id, frame.Dlc);
                else
                    cmd = string.Format("t{0:X3}{1:X1}", frame.Id & 0x7FFu, frame.Dlc);

                for (int i = 0; i < frame.Dlc && i < frame.Data.Length; i++)
                    cmd += string.Format("{0:X2}", frame.Data[i]);

                cmd += "\r";
                _port.Write(cmd);
                return true;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("SlcanInterface/Send: " + ex.Message);
                return false;
            }
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string received = _port.ReadExisting();
                foreach (char c in received)
                {
                    if (c == '\r')
                    {
                        ParseLine(_lineBuffer.ToString());
                        _lineBuffer.Clear();
                    }
                    else
                    {
                        _lineBuffer.Append(c);
                    }
                }
            }
            catch { }
        }

        private void ParseLine(string line)
        {
            if (line.Length < 5) return;
            try
            {
                char type = line[0];
                if (type != 'T' && type != 't') return;

                bool extended = (type == 'T');
                int idLen = extended ? 8 : 3;
                if (line.Length < 1 + idLen + 1) return;

                uint id = Convert.ToUInt32(line.Substring(1, idLen), 16);
                byte dlc = Convert.ToByte(line.Substring(1 + idLen, 1), 16);
                if (dlc > 8) return;

                int dataStart = 1 + idLen + 1;
                if (line.Length < dataStart + dlc * 2) return;

                byte[] data = new byte[dlc];
                for (int i = 0; i < dlc; i++)
                    data[i] = Convert.ToByte(line.Substring(dataStart + i * 2, 2), 16);

                var frame = new CanFrame { Id = id, Dlc = dlc, Data = data, IsExtended = extended };
                FrameReceived?.Invoke(this, new CanFrameEventArgs(frame));
            }
            catch { }
        }

        public void Dispose()
        {
            Close();
        }
    }
}
