using System;
using System.IO.Ports;
using System.Diagnostics;

namespace IMUapp
{
    public class SerialComm
    {
        public int RCportBaud = 38400;
        public string RCportName = "RCport";
        public SerialPort RCport = new SerialPort("RCport", 38400, Parity.None, 8, StopBits.One);

        private readonly FormMain mf;

        // new data event
        public delegate void NewDataDelegate(string Sentence);

        public SerialComm(FormMain CallingForm)
        {
            mf = CallingForm;
        }

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

                    Properties.Settings.Default.RCportSuccessful = false;
                    Properties.Settings.Default.Save();

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

                        Properties.Settings.Default.RCportSuccessful = false;
                        Properties.Settings.Default.Save();
                    }
                }

                if (RCport.IsOpen)
                {
                    RCport.DiscardOutBuffer();
                    RCport.DiscardInBuffer();

                    Properties.Settings.Default.RCportName = RCportName;
                    Properties.Settings.Default.RCportSuccessful = true;
                    Properties.Settings.Default.RCportBaud = RCportBaud;
                    Properties.Settings.Default.Save();
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
            if (RCport.IsOpen)
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
            mf.IMUdata.ParseStringData(sentence);
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
    }
}
