using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;

namespace RateController
{
    public class SerialComm
    {
        private readonly String ID;
        private readonly FormStart mf;
        private SerialPort cArduinoPort = new SerialPort("RCport", 38400, Parity.None, 8, StopBits.One);
        private string cLog;
        private int cPortNumber;
        private int cRCportBaud = 38400;
        private string cRCportName;
        private byte HiByte;
        private byte LoByte;
        private int ReadErrorCount;
        private DateTime SBtime;
        private System.Windows.Forms.Timer Timer1 = new System.Windows.Forms.Timer();
        private int WriteErrorCount;

        public SerialComm(FormStart CallingForm, int PortNumber)
        {
            mf = CallingForm;
            cPortNumber = PortNumber;
            RCportName = "RCport" + cPortNumber.ToString();
            ID = "_" + PortNumber.ToString() + "_";
            ArduinoPort.ReadTimeout = 1000;
            ArduinoPort.WriteTimeout = 1000;
            Timer1.Interval = 45000;    // 45 seconds
            Timer1.Tick += new EventHandler(ResetErrors);
            Timer1.Enabled = true;
            mf.Tls.WriteLog("Serial Log.txt", "", true, true);
        }

        // new data event
        public delegate void NewDataDelegate(string Sentence);

        public SerialPort ArduinoPort { get => cArduinoPort; set => cArduinoPort = value; }

        public bool IsOpen
        { get { return ArduinoPort.IsOpen; } }

        public int RCportBaud { get => cRCportBaud; set => cRCportBaud = value; }

        public string RCportName { get => cRCportName; set => cRCportName = value; }

        public bool SwitchBoxConnected
        { get { return ((DateTime.Now - SBtime).TotalSeconds < 4); } }

        public void CloseRCport(string Successful="false")
        {
            try
            {
                if (ArduinoPort.IsOpen)
                {
                    ArduinoPort.DataReceived -= RCport_DataReceived;
                    try
                    {
                        ArduinoPort.Close();
                    }
                    catch (Exception e)
                    {
                        mf.Tls.ShowHelp("Could not close serial port. \n" + e.Message, "Comm", 3000, true);
                    }

                    mf.Tls.SaveProperty("RCportSuccessful" + ID + cPortNumber.ToString(), Successful);

                    ArduinoPort.Dispose();
                }
                mf.Tls.WriteLog("Serial Log.txt", cLog);
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("SerialComm/CloseRCport: " + ex.Message);
            }
        }

        public string Log()
        {
            mf.Tls.WriteLog("Serial Log.txt", cLog);
            return cLog;
        }

        public void OpenRCport(bool SuppressErrors = false)
        {
            try
            {
                if (SerialPortExists(RCportName))
                {
                    if (!ArduinoPort.IsOpen)
                    {
                        ArduinoPort.PortName = RCportName;
                        ArduinoPort.BaudRate = RCportBaud;
                        ArduinoPort.DataReceived += RCport_DataReceived;
                        ArduinoPort.DtrEnable = true;
                        ArduinoPort.RtsEnable = true;

                        try
                        {
                            ArduinoPort.Open();
                        }
                        catch (Exception e)
                        {
                            if (!SuppressErrors) mf.Tls.ShowHelp("Could not open serial port. \n" + e.Message, "Comm", 3000, true);

                            mf.Tls.SaveProperty("RCportSuccessful" + ID + cPortNumber.ToString(), "false");
                        }
                    }

                    if (ArduinoPort.IsOpen)
                    {
                        ArduinoPort.DiscardOutBuffer();
                        ArduinoPort.DiscardInBuffer();
                        ReadErrorCount = 0;
                        WriteErrorCount = 0;
                        Timer1.Start();

                        mf.Tls.SaveProperty("RCportName" + ID + cPortNumber.ToString(), RCportName);
                        mf.Tls.SaveProperty("RCportSuccessful" + ID + cPortNumber.ToString(), "true");
                        mf.Tls.SaveProperty("RCportBaud" + ID + cPortNumber.ToString(), RCportBaud.ToString());
                    }
                }
                else
                {
                    if (!SuppressErrors) mf.Tls.ShowHelp("Could not open serial port.", "Comm", 3000, true);

                    mf.Tls.SaveProperty("RCportSuccessful" + ID + cPortNumber.ToString(), "false");
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("SerialComm/OpenRCport: " + ex.Message);
            }
        }

        public void SendData(byte[] Data)
        {
            // send to arduino rate controller
            if (ArduinoPort.IsOpen)
            {
                try
                {
                    ArduinoPort.Write(Data, 0, Data.Length);
                }
                catch (Exception ex)
                {
                    if (ex is TimeoutException)
                    {
                        if (++WriteErrorCount > 6)
                        {
                            CloseRCport();
                            mf.Tls.ShowHelp("Serial Port " + RCportName + " not sending correctly. It has been closed.", "Serial Port", 5000, true, false, true);
                            WriteErrorCount = 0;
                        }
                    }
                    mf.Tls.WriteErrorLog("SerialComm/SendData (" + RCportName + "): " + ex.Message + ", Count = " + WriteErrorCount.ToString());
                }
            }
        }

        private void AddToLog(string NewData)
        {
            cLog += NewData + "\n";
            if (cLog.Length > 100000)
            {
                cLog = cLog.Substring(cLog.Length - 98000, 98000);
            }
            cLog = cLog.Replace("\0", string.Empty);
        }

        private void RCport_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (ArduinoPort.IsOpen)
            {
                try
                {
                    string sentence = ArduinoPort.ReadLine();
                    mf.BeginInvoke(new NewDataDelegate(ReceiveData), sentence);
                    if (ArduinoPort.BytesToRead > 150) ArduinoPort.DiscardInBuffer();
                }
                catch (Exception ex)
                {
                    if (ex is TimeoutException)
                    {
                        if (++ReadErrorCount > 6)
                        {
                            CloseRCport();
                            mf.Tls.ShowHelp("Serial Port " + RCportName + " not receiving correctly. It has been closed.", "Serial Port", 5000, true, false, true);
                            ReadErrorCount = 0;
                        }
                    }
                    mf.Tls.WriteErrorLog("SerialComm/RCport_DataReceived (" + RCportName + "): " + ex.Message + ", Count = " + ReadErrorCount.ToString());
                }
            }
        }

        private void ReceiveData(string sentence)
        {
            try
            {
                AddToLog(sentence);

                int CommaPosition = sentence.IndexOf(",", StringComparison.Ordinal);
                if (CommaPosition > -1)
                {
                    int CRposition = sentence.IndexOf("\r");
                    if (CRposition > -1)
                    {
                        // string data
                        sentence = sentence.Substring(0, CRposition);
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
                                        case 32400:
                                            foreach (clsProduct Prod in mf.Products.Items)
                                            {
                                                Prod.SerialFromAruduino(words);
                                            }
                                            break;

                                        case 32401:
                                            mf.ModulesStatus.ParseStringData(words);
                                            break;

                                        case 32618:
                                            if (mf.SwitchBox.ParseStringData(words))
                                            {
                                                SBtime = DateTime.Now;
                                                if (mf.vSwitchBox.Enabled) mf.vSwitchBox.Enabled = false;
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // check for byte data
                    }
                }
                else
                {
                    mf.ScaleIndicator.ParseStringData(sentence);
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("SerialComm/ReceiveData: " + ex.Message);
            }
        }

        private void ResetErrors(object myObject, EventArgs myEventArgs)
        {
            ReadErrorCount = 0;
            WriteErrorCount = 0;
        }

        private bool SerialPortExists(string PortName)
        {
            bool Result = false;
            foreach (String s in System.IO.Ports.SerialPort.GetPortNames())
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