using System;
using System.IO.Ports;
using System.Diagnostics;

namespace RateController
{
    public class SerialComm
    {
        public SerialPort ArduinoPort = new SerialPort("RCport", 38400, Parity.None, 8, StopBits.One);
        public int RCportBaud = 38400;
        public string RCportName;
        private readonly FormStart mf;
        private int cPortNumber;

        private bool SerialActive = false;  // prevents UI lock-up by only sending serial data after verfying connection

        public SerialComm(FormStart CallingForm,int PortNumber)
        {
            mf = CallingForm;
            cPortNumber = PortNumber;
            RCportName = "RCport" + cPortNumber.ToString();
        }

        // new data event
        public delegate void NewDataDelegate(string Sentence);

        public void CloseRCport()
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
                        mf.Tls.TimedMessageBox("Could not close serial port.", e.Message, 3000, true);
                    }

                    mf.Tls.SaveProperty("RCportSuccessful"+cPortNumber.ToString(), "false");

                    ArduinoPort.Dispose();
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
                            mf.Tls.TimedMessageBox("Could not open serial port.", e.Message, 3000, true);

                            mf.Tls.SaveProperty("RCportSuccessful" + cPortNumber.ToString(), "false");
                        }
                    }

                    if (ArduinoPort.IsOpen)
                    {
                        ArduinoPort.DiscardOutBuffer();
                        ArduinoPort.DiscardInBuffer();

                        mf.Tls.SaveProperty("RCportName" + cPortNumber.ToString(), RCportName);
                        mf.Tls.SaveProperty("RCportSuccessful" + cPortNumber.ToString(), "true");
                        mf.Tls.SaveProperty("RCportBaud" + cPortNumber.ToString(), RCportBaud.ToString());
                    }
                }
                else
                {
                    mf.Tls.TimedMessageBox("Could not open serial port.", "", 3000, true);

                    mf.Tls.SaveProperty("RCportSuccessful" + cPortNumber.ToString(), "false");
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("SerialComm/OpenRCport: " + ex.Message);
            }
        }

        public void SendToArduino(byte[] Data)
        {
            // send to arduino rate controller
            if (ArduinoPort.IsOpen & SerialActive)
            {
                try
                {
                    ArduinoPort.Write(Data, 0, Data.Length);
                }
                catch (Exception ex)
                {
                    mf.Tls.WriteErrorLog("SerialComm/SendtoRC: " + ex.Message);
                }
            }
        }

        private void HandleRCdata(string sentence)
        {
            // Find end of sentence, if not a CR, return
            int end = sentence.IndexOf("\r");
            if (end == -1) return;
            end = sentence.IndexOf(",", StringComparison.Ordinal);
            if (end == -1) return;
            if (mf.RateCals[cPortNumber].CommFromArduino(sentence)) SerialActive = true;
        }

        private void RCport_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (ArduinoPort.IsOpen)
            {
                try
                {
                    string sentence = ArduinoPort.ReadLine();
                    mf.BeginInvoke(new NewDataDelegate(HandleRCdata), sentence);
                    if (ArduinoPort.BytesToRead > 32) ArduinoPort.DiscardInBuffer();
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