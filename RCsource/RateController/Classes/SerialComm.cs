using System;
using System.IO.Ports;

namespace RateController
{
    public class SerialComm
    {
        private SerialPort cArduinoPort = new SerialPort("RCport", 38400, Parity.None, 8, StopBits.One);
        private int cRCportBaud = 38400;
        private string cRCportName;
        private readonly FormStart mf;
        private int cPortNumber;

        private byte HiByte;
        private byte LoByte;
        private bool SerialActive = false;  // prevents UI lock-up by only sending serial data after verfying connection
        String ID;

        public SerialComm(FormStart CallingForm, int PortNumber)
        {
            mf = CallingForm;
            cPortNumber = PortNumber;
            cRCportName = "RCport" + cPortNumber.ToString();
            ID = "_" + PortNumber.ToString() + "_";
        }

        public SerialPort ArduinoPort { get { return cArduinoPort; } }

        public int RCportBaud
        {
            get { return cRCportBaud; }
            set { cRCportBaud = value; }
        }

        public string RCportName
        {
            get { return cRCportName; }
            set { cRCportName = value; }
        }

        // new data event
        public delegate void NewDataDelegate(string Sentence);

        public void CloseRCport()
        {
            try
            {
                if (cArduinoPort.IsOpen)
                {
                    cArduinoPort.DataReceived -= RCport_DataReceived;
                    try
                    {
                        cArduinoPort.Close();
                    }
                    catch (Exception e)
                    {
                        mf.Tls.TimedMessageBox("Could not close serial port.", e.Message, 3000, true);
                    }

                    mf.Tls.SaveProperty("RCportSuccessful"+ID + cPortNumber.ToString(), "false");

                    cArduinoPort.Dispose();
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("SerialComm/CloseRCport: " + ex.Message);
            }
        }

        public void OpenRCport()
        {
            try
            {
                if (SerialPortExists(cRCportName))
                {
                    if (!cArduinoPort.IsOpen)
                    {
                        cArduinoPort.PortName = cRCportName;
                        cArduinoPort.BaudRate = cRCportBaud;
                        cArduinoPort.DataReceived += RCport_DataReceived;
                        cArduinoPort.DtrEnable = true;
                        cArduinoPort.RtsEnable = true;

                        try
                        {
                            cArduinoPort.Open();
                        }
                        catch (Exception e)
                        {
                            mf.Tls.TimedMessageBox("Could not open serial port.", e.Message, 3000, true);

                            mf.Tls.SaveProperty("RCportSuccessful"+ID + cPortNumber.ToString(), "false");
                        }
                    }

                    if (cArduinoPort.IsOpen)
                    {
                        cArduinoPort.DiscardOutBuffer();
                        cArduinoPort.DiscardInBuffer();

                        mf.Tls.SaveProperty("RCportName"+ID + cPortNumber.ToString(), cRCportName);
                        mf.Tls.SaveProperty("RCportSuccessful"+ID + cPortNumber.ToString(), "true");
                        mf.Tls.SaveProperty("RCportBaud"+ID + cPortNumber.ToString(), cRCportBaud.ToString());
                    }
                }
                else
                {
                    mf.Tls.TimedMessageBox("Could not open serial port.", "", 3000, true);

                    mf.Tls.SaveProperty("RCportSuccessful"+ID + cPortNumber.ToString(), "false");
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
            if (cArduinoPort.IsOpen && SerialActive)
            {
                try
                {
                    cArduinoPort.Write(Data, 0, Data.Length);
                }
                catch (Exception ex)
                {
                    mf.Tls.WriteErrorLog("SerialComm/SendData: " + ex.Message);
                }
            }
        }

        private void ReceiveData(string sentence)
        {
            try
            {
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
                                case 32618:
                                    if (mf.SwitchBox.ParseStringData(words)) SerialActive = true;
                                    break;

                                case 32613:
                                    foreach (clsProduct Prod in mf.Products.Items)
                                    {
                                        if (Prod.SerialFromAruduino(words)) SerialActive = true;
                                    }
                                    break;

                                case 32621:
                                    if (mf.PressureData.ParseStringData(words)) SerialActive = true;
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

        private void RCport_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (cArduinoPort.IsOpen)
            {
                try
                {
                    string sentence = cArduinoPort.ReadLine();
                    mf.BeginInvoke(new NewDataDelegate(ReceiveData), sentence);
                    if (cArduinoPort.BytesToRead > 32) cArduinoPort.DiscardInBuffer();
                }
                catch (Exception ex)
                {
                    mf.Tls.WriteErrorLog("SerialComm/RCport_DataReceived: " + ex.Message);
                }
            }
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