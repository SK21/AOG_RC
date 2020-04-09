using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMUapp
{
    public class PGN32750
    {
        // PGN32750 IMU >IMU app > AgOpenGPS
        // 0 HeaderHi       127
        // 1 HeaderLo       238
        // 2 -
        // 3 -
        // 4 HeadingHi      actual X 16
        // 5 HeadingLo
        // 6 RollHi         actual X 16
        // 7 RollLo
        // 8 PitchHi        actual X 16
        // 9 PitchLo

        private const byte cByteCount = 10;
        private const byte HeaderHi = 127;
        private const byte HeaderLo = 238;

        private byte[] cData = new byte[10];
        private byte[] SendData = new byte[10];
        private FormMain mf;

        int Temp;
        Int16 Tmp16;
        public bool UsePitch = false;
        private string Message;

        private Int16 cInvert = 1;
        public bool UpdateEnabled = false;

        public PGN32750(FormMain CalledFrom)
        {
            mf = CalledFrom;
            SendData[0] = HeaderHi;
            SendData[1] = HeaderLo;
        }

        public bool Invert
        {
            get { return (cInvert == -1); }
            set
            {
                if (value)
                {
                    cInvert = -1;
                }
                else
                {
                    cInvert = 1;
                }
            }
        }

        public bool ParseStringData(string sentence)
        {
            bool Result = false;
            try
            {
                int end = sentence.IndexOf("\r");
                sentence = sentence.Substring(0, end);
                string[] Data = sentence.Split(',');

                if (Data.Length >= cByteCount)
                {
                    int.TryParse(Data[0], out Temp);
                    if (Temp == HeaderHi)
                    {
                        int.TryParse(Data[1], out Temp);
                        if (Temp == HeaderLo)
                        {
                            for (int i = 0; i < cByteCount; i++)
                            {
                                byte.TryParse(Data[i], out cData[i]);
                            }
                            Result = true;
                            Send();
                            UpdateDisplay();
                        }
                    }
                }

            }
            catch (Exception)
            {
                return false;
            }
            return Result;
        }

        public bool ParseByteData(byte[] Data)
        {
            bool Result = false;
            if (Data[0] == HeaderHi & Data[1] == HeaderLo & Data.Length >= cByteCount)
            {
                for (int i = 0; i < cByteCount; i++)
                {
                    cData[i] = Data[i];
                }
                Result = true;
                Send();
                UpdateDisplay();
            }
            return Result;
        }

        public void Send()
        {
            SendData[4] = cData[4];
            SendData[5] = cData[5];

            if (UsePitch)
            {
                Tmp16 = (Int16)(cData[8] << 8 | cData[9]);
                Tmp16 *= cInvert;
            }
            else
            {
                Tmp16 = (Int16)(cData[6] << 8 | cData[7]);
                Tmp16 *= cInvert;
            }
            SendData[6] = (byte)(Tmp16 >> 8);
            SendData[7] = (byte)Tmp16;

            mf.UDPlocal.SendUDPMessage(SendData);
        }

        private void UpdateDisplay()
        {
            if (UpdateEnabled)
            {
                Tmp16 = (Int16)(cData[4] << 8 | cData[5]);
                Message = "Heading: " + (Tmp16 / 16.0).ToString("N2");

                Tmp16 = (Int16)(cData[6] << 8 | cData[7]);
                Message += "  Roll: " + (Tmp16 / 16.0).ToString("N2");

                Tmp16 = (Int16)(cData[8] << 8 | cData[9]);
                Message += "  Pitch: " + (Tmp16 / 16.0).ToString("N2");

                mf.UpdateDisplay(Message);
            }
        }
    }
}
