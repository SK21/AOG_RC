using System;

namespace RateController
{
    public class PGN32613
    {
        // to Rate Controller from Arduino
        //0	HeaderLo		    101
        //1	HeaderHi		    127
        //2 Mod/Sen ID          0-15/0-15
        //3	rate applied Lo 	10 X actual
        //4 rate applied Mid
        //5	rate applied Hi
        //6	acc.Quantity Lo		10 X actual
        //7	acc.Quantity Mid
        //8 acc.Quantity Hi
        //9 PWM Lo              10 X actual
        //10 PWM Hi
        //11 Status
        //      bit 0 - sensor 0 connected
        //      bit 1 - sensor 1 connected
        //      bit 2   - wifi rssi < -80
        //      bit 3	- wifi rssi < -70
        //      bit 4	- wifi rssi < -65
        //12 CRC

        private const byte cByteCount = 13;
        private const byte HeaderHi = 127;
        private const byte HeaderLo = 101;
        private bool cModuleIsReceivingData;
        private double cPWMsetting;
        private double cQuantity;
        private double cUPM;
        private clsProduct Prod;

        private double Ratio;
        private DateTime ReceiveTime;

        private bool[] SensorReceiving = new bool[2];
        private double Trate;
        private byte cWifiStrength;

        public PGN32613(clsProduct CalledFrom)
        {
            Prod = CalledFrom;
        }

        public double AccumulatedQuantity()
        {
            return cQuantity;
        }

        public bool Connected()
        {
            return ModuleReceiving() & ModuleSending();
        }

        public bool ModuleReceiving()
        {
            if (Prod.mf.SimMode == SimType.VirtualNano)
            {
                return true;
            }
            else
            {
                return cModuleIsReceivingData;
            }
        }

        public bool ModuleSending()
        {
            if (Prod.mf.SimMode == SimType.VirtualNano)
            {
                return true;
            }
            else
            {
                return ((DateTime.Now - ReceiveTime).TotalSeconds < 4);
            }
        }

        public bool ParseByteData(byte[] Data)
        {
            bool Result = false;
            if (Data[1] == HeaderHi && Data[0] == HeaderLo &&
                Data.Length >= cByteCount && Prod.mf.Tls.GoodCRC(Data))
            {
                int tmp = Prod.mf.Tls.ParseModID(Data[2]);
                if (Prod.ModuleID == tmp)
                {
                    tmp = Prod.mf.Tls.ParseSenID(Data[2]);
                    if (Prod.SensorID == tmp)
                    {
                        cUPM = (Data[5] << 16 | Data[4] << 8 | Data[3]) / 10.0;
                        cQuantity = (Data[8] << 16 | Data[7] << 8 | Data[6]) / 10.0;
                        cPWMsetting = (Int16)(Data[10] << 8 | Data[9]) / 10.0;  // need to cast to 16 bit integer to preserve the sign bit

                        // status
                        if (tmp == 0)
                        {
                            // sensor 0
                            cModuleIsReceivingData = ((Data[11] & 0b00000001) == 0b00000001);
                            CheckReceiving(0, cModuleIsReceivingData);
                        }
                        else
                        {
                            // sensor 1
                            cModuleIsReceivingData = ((Data[11] & 0b00000010) == 0b00000010);
                            CheckReceiving(1, cModuleIsReceivingData);
                        }

                        // wifi strength
                        cWifiStrength = 0;
                        if ((Data[11] & 0b00000100) == 0b00000100) cWifiStrength =1;
                        if ((Data[11] & 0b00001000) == 0b00001000) cWifiStrength =2;
                        if ((Data[11] & 0b00010000) == 0b00010000) cWifiStrength =3;
                        Prod.WifiStrength = cWifiStrength;

                        ReceiveTime = DateTime.Now;
                        Result = true;

                        //CheckRate();
                    }
                }
            }
            return Result;
        }

        public bool ParseStringData(string[] Data)
        {
            byte pwmHi;
            byte pwmLo;
            byte QuantityLo;
            byte QuantityMid;
            byte QuantityHi;
            byte RateHi;
            byte RateMid;
            byte RateLo;
            int Temp;
            bool Result = false;

            if (Data.Length >= cByteCount && Prod.mf.Tls.GoodCRC(Data))
            {
                int.TryParse(Data[0], out Temp);

                if (Temp == HeaderLo)
                {
                    int.TryParse(Data[1], out Temp);
                    if (Temp == HeaderHi)
                    {
                        int.TryParse(Data[2], out Temp);
                        int tmp = Prod.mf.Tls.ParseModID((byte)Temp);
                        if (Prod.ModuleID == tmp)
                        {
                            tmp = Prod.mf.Tls.ParseSenID((byte)Temp);
                            if (Prod.SensorID == tmp)
                            {
                                // rate applied, 10 X actual
                                byte.TryParse(Data[3], out RateLo);
                                byte.TryParse(Data[4], out RateMid);
                                byte.TryParse(Data[5], out RateHi);
                                cUPM = (RateHi << 16 | RateMid << 8 | RateLo) / 10.0;

                                // accumulated quantity
                                byte.TryParse(Data[6], out QuantityLo);
                                byte.TryParse(Data[7], out QuantityMid);
                                byte.TryParse(Data[8], out QuantityHi);
                                cQuantity = (QuantityHi << 16 | QuantityMid << 8 | QuantityLo) / 10.0;

                                // pwmSetting
                                byte.TryParse(Data[9], out pwmLo);
                                byte.TryParse(Data[10], out pwmHi);

                                cPWMsetting = (double)((Int16)(pwmHi << 8 | pwmLo)) / 10.0;

                                // status
                                byte Status;
                                byte.TryParse(Data[11], out Status);
                                if (tmp == 0)
                                {
                                    // sensor 0
                                    cModuleIsReceivingData = ((Status & 0b00000001) == 0b00000001);
                                }
                                else
                                {
                                    // sensor 1
                                    cModuleIsReceivingData = ((Status & 0b00000010) == 0b00000010);
                                }

                                ReceiveTime = DateTime.Now;
                                Result = true;

                                //CheckRate();
                            }
                        }
                    }
                }
            }
            return Result;
        }

        public double PWMsetting()
        {
            return cPWMsetting;
        }

        public double UPM()
        {
            return cUPM;
        }

        private void CheckRate()
        {
            Trate = Prod.TargetUPM();
            if (Trate > 0 && cUPM > 0)
            {
                Ratio = Math.Abs((cUPM / Trate) - 1);
                if (Ratio > 0.3)
                {
                    Prod.mf.Tls.WriteActivityLog("");
                    Prod.mf.Tls.WriteActivityLog("Current rate: " + cUPM.ToString("N2") + ", Ave. rate: " + Trate.ToString("N2") + ", Ratio: " + Ratio.ToString("N2")
                       + ", Quantity: " + cQuantity.ToString("N2") + ", PWM:" + cPWMsetting.ToString("N2"));
                }
            }
        }

        private void CheckReceiving(byte ID, bool Receiving)
        {
            if (Receiving != SensorReceiving[ID])
            {
                Prod.SendPID(); // update pid settings when connected
                Prod.mf.Tls.WriteActivityLog("Sensor " + ID.ToString() + " receiving: " + Receiving.ToString());
                //Debug.Print("Sensor " + ID.ToString() + " receiving: " + Receiving.ToString());
            }
            SensorReceiving[ID] = Receiving;
        }
    }
}