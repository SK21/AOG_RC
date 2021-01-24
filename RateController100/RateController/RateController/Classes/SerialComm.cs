using System;
using System.IO.Ports;

namespace RateController
{
    public class SerialComm
    {
        public SerialPort RCport = new SerialPort("RCport", 38400, Parity.None, 8, StopBits.One);
        public int RCportBaud = 38400;
        public string RCportName;
        private readonly FormRateControl mf;
        private int cPortNumber;

        private bool SerialActive = false;  // prevents UI lock-up by only sending serial data after verfying connection

        public SerialComm(FormRateControl CallingForm,int PortNumber)
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
                if (RCport.IsOpen)
                {
                    RCport.DataReceived -= RCport_DataReceived;
                    try
                    {
                        RCport.Close();
                    }
                    catch (Exception e)
                    {
                        mf.Tls.TimedMessageBox("Could not close serial port.", e.Message, 3000, true);
                    }

                    mf.Tls.SaveProperty("RCportSuccessful"+cPortNumber.ToString(), "false");

                    RCport.Dispose();
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
                    if (!RCport.IsOpen)
                    {
                        RCport.PortName = RCportName;
                        RCport.BaudRate = RCportBaud;
                        RCport.DataReceived += RCport_DataReceived;
                        RCport.DtrEnable = true;
                        RCport.RtsEnable = true;

                        try
                        {
                            RCport.Open();
                        }
                        catch (Exception e)
                        {
                            mf.Tls.TimedMessageBox("Could not open serial port.", e.Message, 3000, true);

                            mf.Tls.SaveProperty("RCportSuccessful" + cPortNumber.ToString(), "false");
                        }
                    }

                    if (RCport.IsOpen)
                    {
                        RCport.DiscardOutBuffer();
                        RCport.DiscardInBuffer();

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

        public void SendtoRC(byte[] Data)
        {
            // send to arduino rate controller
            if (RCport.IsOpen & SerialActive)
            //if (RCport.IsOpen)
            {
                try
                {
                    RCport.Write(Data, 0, Data.Length);
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
            if (mf.RC.CommFromArduino(sentence)) SerialActive = true;
        }

        private void RCport_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (RCport.IsOpen)
            {
                try
                {
                    string sentence = RCport.ReadLine();
                    mf.BeginInvoke(new NewDataDelegate(HandleRCdata), sentence);
                    if (RCport.BytesToRead > 32) RCport.DiscardInBuffer();
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