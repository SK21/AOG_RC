using PCBsetup.Forms;
using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Timers;

namespace PCBsetup
{
    public class SerialComm
    {
        public SerialPort ArduinoPort = new SerialPort("SCport", 38400, Parity.None, 8, StopBits.One);
        public string cSCportName;
        public int SCportBaud = 38400;
        private readonly frmMain mf;
        private bool ActivationNotified = false;
        private string cLog;
        private int cPortNumber;

        // prevents UI lock-up by only sending serial data after verfying connection
        private bool cReceiveActive = false;

        private byte HiByte;
        private string ID;
        private byte LoByte;

        public SerialComm(frmMain CallingForm, int PortNumber)
        {
            mf = CallingForm;
            cPortNumber = PortNumber;
            cSCportName = "SCport" + cPortNumber.ToString();
            ID = "_" + PortNumber.ToString();
            ArduinoPort.ReadTimeout = 500;
            ArduinoPort.WriteTimeout = 500;

            Timer Watchdog = new Timer(3000);
            Watchdog.Elapsed += new ElapsedEventHandler(CheckConnected);
            Watchdog.Start();
        }

        // new data event
        public delegate void NewDataDelegate(string Sentence);

        public event EventHandler ModuleConnected;

        public String SCportName
        {
            get { return cSCportName; }
            set
            {
                Close();
                cSCportName = value;
            }
        }

        public void Close()
        {
            try
            {
                if (ArduinoPort.IsOpen)
                {
                    ArduinoPort.DataReceived -= SCport_DataReceived;
                    try
                    {
                        ArduinoPort.Close();
                        cReceiveActive = false;
                    }
                    catch (Exception)
                    {
                        throw new InvalidOperationException("Could not close port.");
                    }

                    mf.Tls.SaveProperty("SCportSuccessful" + ID, "false");

                    ArduinoPort.Dispose();
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("SerialComm/CloseSCport: " + ex.Message);
            }
        }

        public bool IsOpen()
        {
            return ArduinoPort.IsOpen;
        }

        public bool IsReceiveActive()
        {
            return cReceiveActive;
        }

        public string Log()
        {
            return cLog;
        }

        public void Open()
        {
            try
            {
                if (SerialPortExists(cSCportName))
                {
                    if (!ArduinoPort.IsOpen)
                    {
                        ArduinoPort.PortName = cSCportName;
                        ArduinoPort.BaudRate = SCportBaud;
                        ArduinoPort.DataReceived += SCport_DataReceived;
                        ArduinoPort.DtrEnable = true;
                        ArduinoPort.RtsEnable = true;
                        ActivationNotified = false;

                        try
                        {
                            ArduinoPort.Open();
                        }
                        catch (Exception)
                        {
                            mf.Tls.SaveProperty("SCportSuccessful" + ID, "false");
                            throw new InvalidOperationException("Could not open port.");
                        }
                    }

                    if (ArduinoPort.IsOpen)
                    {
                        ArduinoPort.DiscardOutBuffer();
                        ArduinoPort.DiscardInBuffer();

                        mf.Tls.SaveProperty("SCportName" + ID, cSCportName);
                        mf.Tls.SaveProperty("SCportSuccessful" + ID, "true");
                        mf.Tls.SaveProperty("SCportBaud" + ID, SCportBaud.ToString());
                    }
                }
                else
                {
                    mf.Tls.SaveProperty("SCportSuccessful" + ID, "false");
                    throw new InvalidOperationException("Could not open port.");
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("SerialComm/OpenSCport: " + ex.Message);
            }
        }

        public bool Send(byte[] Data)
        {
            bool Result = false;
            if (ArduinoPort.IsOpen)
            {
                if (cReceiveActive)
                {
                    try
                    {
                        ArduinoPort.Write(Data, 0, Data.Length);
                        Result = true;
                    }
                    catch (Exception ex)
                    {
                        mf.Tls.WriteErrorLog("SerialComm/SendData: " + ex.Message);
                    }
                }
                else
                {
                    throw new InvalidOperationException("ModuleDisconnected");
                }
            }
            else
            {
                throw new InvalidOperationException("CommDisconnected");
            }
            return Result;
        }

        private void AddToLog(string NewData)
        {
            cLog += NewData + "\n";
            if (cLog.Length > 10000)
            {
                cLog = cLog.Substring(cLog.Length - 9800, 9800);
            }
        }

        private void CheckConnected(object sender, ElapsedEventArgs e)
        {
            if (!ArduinoPort.IsOpen) cReceiveActive = false;
        }

        private void ReceiveData(string sentence)
        {
            try
            {
                AddToLog(sentence);

                cReceiveActive = true;
                if (!ActivationNotified)
                {
                    ModuleConnected?.Invoke(this, new EventArgs());
                    ActivationNotified = true;
                }

                // look for ',' and '\r'. Return if not found
                int end = sentence.IndexOf(",", StringComparison.Ordinal);
                if (end == -1) return;

                end = sentence.IndexOf("\r");
                if (end == -1) return;

                sentence = sentence.Substring(0, end);
                string[] words = sentence.Split(',');

                if (words.Length > 1)
                {
                    if (byte.TryParse(words[0], out LoByte))
                    {
                        if (byte.TryParse(words[1], out HiByte))
                        {
                            int PGN = HiByte << 8 | LoByte;

                            switch (PGN)
                            {
                                //case 32618:
                                //    if (mf.SwitchBox.ParseStringData(words)) SerialActive = true;
                                //    break;

                                //case 32613:
                                //    foreach (clsProduct Prod in mf.Products.Items)
                                //    {
                                //        if (Prod.SerialFromAruduino(words)) SerialActive = true;
                                //    }
                                //    break;

                                //case 32621:
                                //    if (mf.PressureData.ParseStringData(words)) SerialActive = true;
                                //    break;

                                case 0xABC:
                                    // debug info from module
                                    Debug.Print("");
                                    for (int i = 0; i < words.Length; i++)
                                    {
                                        Debug.Print(DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString() + "  " + i.ToString() + " " + words[i].ToString());
                                    }
                                    Debug.Print("");
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("SerialComm/ReceiveData: " + ex.Message);
            }
        }

        private void SCport_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (ArduinoPort.IsOpen)
            {
                try
                {
                    string sentence = ArduinoPort.ReadLine();
                    mf.BeginInvoke(new NewDataDelegate(ReceiveData), sentence);
                    if (ArduinoPort.BytesToRead > 32) ArduinoPort.DiscardInBuffer();
                }
                catch (Exception ex)
                {
                    mf.Tls.WriteErrorLog("SerialComm/SCport_DataReceived: " + ex.Message);
                }
            }
        }

        private bool SerialPortExists(string PortName)
        {
            bool Result = false;
            foreach (string s in SerialPort.GetPortNames())
            {
                if (s == PortName)
                {
                    Result = true;
                    break;
                }
            }
            return Result;
        }
    }
}