using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RateController.Classes
{
    public class clsCanBus
    {
        private const int MaxBufferSize = 100;
        private readonly string portName;
        private ConcurrentQueue<string> commandQueue = new ConcurrentQueue<string>();
        private AutoResetEvent commandReady = new AutoResetEvent(false);
        private FormStart mf;
        private Queue<string> receiveBuffer = new Queue<string>();
        private bool reconnecting = false;
        private Thread reconnectThread;
        private SerialPort serialPort;
        private bool stopReconnect = false;
        private System.Windows.Forms.Timer Timer1 = new System.Windows.Forms.Timer();

        public clsCanBus(FormStart CallingForm, string portName)
        {
            mf = CallingForm;
            this.portName = portName;
            serialPort = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);
            serialPort.DataReceived += SerialPort_DataReceived;
            serialPort.ErrorReceived += SerialPort_ErrorReceived;
            Timer1.Interval = 1000;
            Timer1.Tick += Timer1_Tick1;
            StartReconnectMonitor();
            StartCommandProcessor();
        }

        public bool Close()
        {
            bool result = false;
            try
            {
                Timer1.Stop();
                SendSlcanCommand("C"); // Close the CAN channel
                serialPort.Close();
                stopReconnect = true;
                result = true;
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog($"clsCanBus/Close: {ex.Message}");
            }
            return result;
        }

        public bool Open()
        {
            bool result = false;
            try
            {
                serialPort.Open();
                SendSlcanCommand("S6"); // Set bitrate to 500kbps
                SendSlcanCommand("O");  // Open the CAN channel
                Timer1.Start();
                result = true;
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog($"clsCanBus/Open: {ex.Message}");
                reconnecting = true;
            }
            return result;
        }

        public void SendCanMessage(int id, byte[] data)
        {
            if (data.Length > 8) throw new ArgumentException("CAN data length cannot exceed 8 bytes.");
            string idHex = id.ToString("X3");
            string dataHex = string.Concat(data.Select(b => b.ToString("X2")));
            string command = $"t{idHex}{data.Length}{dataHex}";
            SendSlcanCommand(command);
        }

        private CanMessageReceivedEventArgs ParseCanMessage(string message)
        {
            try
            {
                string idHex = message.Substring(1, 3); // Extract CAN ID
                int id = Convert.ToInt32(idHex, 16);

                int dataLength = int.Parse(message.Substring(4, 1)); // Extract length

                List<byte> dataBytes = new List<byte>();
                for (int i = 0; i < dataLength; i++)
                {
                    string byteHex = message.Substring(5 + (i * 2), 2); // Extract each byte
                    dataBytes.Add(Convert.ToByte(byteHex, 16));
                }

                return new CanMessageReceivedEventArgs(id, dataBytes.ToArray());
            }
            catch (Exception)
            {
                return null; // Ignore malformed messages
            }
        }

        private void SendSlcanCommand(string command)
        {
            commandQueue.Enqueue(command + "\r"); // Enqueue the command
            commandReady.Set(); // Signal that a command is ready to be processed
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string receivedData = serialPort.ReadExisting();
            if (receivedData.Contains('\r'))
            {
                string[] messages = receivedData.Split('\r');
                foreach (string message in messages)
                {
                    if (message.Length > 0 && message.StartsWith("t")) // Ensure it's a valid CAN message
                    {
                        lock (receiveBuffer)
                        {
                            if (receiveBuffer.Count >= MaxBufferSize)
                            {
                                receiveBuffer.Dequeue(); // Remove the oldest message
                            }
                            receiveBuffer.Enqueue(message);
                        }
                        CanMessageReceivedEventArgs parsedMessage = ParseCanMessage(message);
                        if (parsedMessage != null)
                        {
                            CanMessageReceived?.Invoke(this, parsedMessage);
                        }
                    }
                }
            }
        }

        private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            mf.Tls.WriteErrorLog("clsCanbus serial error detected. Restarting connection...");
            reconnecting = true;
        }

        private void StartCommandProcessor()
        {
            Task.Run(() =>
            {
                while (!stopReconnect)
                {
                    commandReady.WaitOne(); // Wait for a command to be ready
                    while (commandQueue.TryDequeue(out string command))
                    {
                        if (serialPort.IsOpen)
                        {
                            try
                            {
                                serialPort.Write(command); // Send the command
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error sending command: {ex.Message}");
                            }
                        }
                    }
                }
            });
        }

        private void StartReconnectMonitor()
        {
            reconnectThread = new Thread(() =>
            {
                while (!stopReconnect)
                {
                    if (reconnecting)
                    {
                        Console.WriteLine("Connection lost. Attempting to reconnect...");
                        while (reconnecting)
                        {
                            try
                            {
                                Thread.Sleep(5000); // Wait before retrying
                                string[] availablePorts = SerialPort.GetPortNames();
                                if (availablePorts.Contains(portName))
                                {
                                    serialPort = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);
                                    serialPort.DataReceived += SerialPort_DataReceived;
                                    serialPort.ErrorReceived += SerialPort_ErrorReceived;
                                    serialPort.Open();
                                    SendSlcanCommand("S6");
                                    SendSlcanCommand("O");
                                    Console.WriteLine("Reconnected successfully.");
                                    reconnecting = false;
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Reconnect attempt failed: {ex.Message}. Retrying...");
                            }
                        }
                    }
                    Thread.Sleep(1000); // Avoid CPU overuse
                }
            });
            reconnectThread.IsBackground = true;
            reconnectThread.Start();
        }

        private void Timer1_Tick1(object sender, EventArgs e)
        {
            if (!serialPort.IsOpen) reconnecting = true;
        }

        public class CanMessageReceivedEventArgs : EventArgs
        {
            public CanMessageReceivedEventArgs(int senderId, byte[] data)
            {
                SenderId = senderId;
                Data = data;
            }

            public byte[] Data { get; }
            public int SenderId { get; }
        }
    }
}
}