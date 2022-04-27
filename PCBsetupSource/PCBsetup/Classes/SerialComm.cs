using PCBsetup.Forms;
using System;
using System.IO.Ports;

namespace PCBsetup
{
    public class SerialComm
    {
        public SerialPort ArduinoPort = new SerialPort("SCport", 38400, Parity.None, 8, StopBits.One);
        public string cSCportName;
        public int SCportBaud = 38400;
        private readonly frmMain mf;
        private int cPortNumber;

        private byte HiByte;
        private string ID;
        private byte LoByte;
        private bool cSerialActive = false;  // prevents UI lock-up by only sending serial data after verfying connection
        private bool ActivationNotified = false;

        public event EventHandler ModuleConnected;

        public SerialComm(frmMain CallingForm, int PortNumber)
        {
            mf = CallingForm;
            cPortNumber = PortNumber;
            cSCportName = "SCport" + cPortNumber.ToString();
            ID = "_" + PortNumber.ToString();
        }

        // new data event
        public delegate void NewDataDelegate(string Sentence);

        public String SCportName
        {
            get { return cSCportName; }
            set
            {
                CloseSCport();
                cSCportName = value;
            }
        }

        public void CloseSCport()
        {
            try
            {
                if (ArduinoPort.IsOpen)
                {
                    ArduinoPort.DataReceived -= SCport_DataReceived;
                    try
                    {
                        ArduinoPort.Close();
                        cSerialActive = false;
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

        public void OpenSCport()
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
                        catch (Exception )
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

        public bool IsOpen()
        {
            return ArduinoPort.IsOpen;
        }

        public bool PortOpen(string Name)
        {
            bool Result = false;
            if (ArduinoPort.PortName == Name && ArduinoPort.IsOpen) Result = true;
            return Result;
        }

        public bool SendData(byte[] Data)
        {
            bool Result = false;
            if (ArduinoPort.IsOpen)
            {
                if (cSerialActive)
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
        public bool IsSerialActive()
        {
            return cSerialActive;
        }
        private void ReceiveData(string sentence)
        {
            try
            {
                    cSerialActive = true;
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

                            //switch (PGN)
                            //{
                            //    case 32618:
                            //        if (mf.SwitchBox.ParseStringData(words)) SerialActive = true;
                            //        break;

                            //    case 32613:
                            //        foreach (clsProduct Prod in mf.Products.Items)
                            //        {
                            //            if (Prod.SerialFromAruduino(words)) SerialActive = true;
                            //        }
                            //        break;

                            //    case 32621:
                            //        if (mf.PressureData.ParseStringData(words)) SerialActive = true;
                            //        break;
                            //}
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