using System;
using System.Diagnostics;

namespace RateController
{
    public class clsArduino
    {
        private float cIntegral = 0;
        private readonly clsProduct Prd;

        private bool FlowEnabled = false;
        private bool AutoOn = true;
        private ControlTypeEnum ControlType;

        private double CurrentDuration;
        private bool cUseMultiPulse;
        private int cErrorRange = 4;        // % random error in flow rate, above and below target
        double ManualAdjust;

        private DateTime LastPulse = DateTime.Now;
        private float LastPWM;
        private DateTime LastTime;
        private DateTime LastScaleCountTime;
        private int LOOP_TIME = 50;
        private bool MasterOn = false;
        private byte MaxRPM = 255;
        private float MaxSimUPM = 40F;        // max rate of system in UPM
        private int SensorID;
        private float MeterCal;

        private float Oave;

        private UInt16 Ocount;

        private UInt32 Omax;

        private UInt32 Omin;

        private UInt32 Osum;

        // PID
        private double Kp = 20;
        private double Ki = 0;
        private double Kd = 0;
        private byte MinPWM = 50;
        private byte MaxPWM = 255;

        private byte BrakePoint = 20;
        private byte Deadband = 3;

        private UInt32 PPM;
        private float PPR = 1.0F;        // pulses per revolution

        private double PrevCount;
        private double pulseCount;
        private float pulseDuration;
        private float Pulses = 0;

        private float pwmSetting;
        private float rateError;
        private float rateSetPoint;
        private DateTime ReceiveTime;
        private byte RelayHi;
        private byte RelayLo;
        private float SimRPM = 0.0F;

        private int SimulateInterval;
        private DateTime SimulateTimeLast;

        private double TimedCounts;
        private DateTime TimedLast = DateTime.Now;
        private float TotalPulses;

        private float UPM;

        private float ValveAdjust = 0;   // % amount to open/close valve
        private float ValveOpen = 0;      // % valve is open
        private float ValveOpenTime = 4000;  // ms to fully open valve at max opening rate

        public float Integral { get => cIntegral; set => cIntegral = value; }
        public int ErrorRange { get => cErrorRange; set => cErrorRange = value; }

        private DateTime SendLast;
        private int SendTime = 200;

        public clsArduino(clsProduct CalledFrom)
        {
            Prd = CalledFrom;
            LastScaleCountTime = DateTime.Now;
        }

        public void MainLoop()
        {
            if ((DateTime.Now - LastTime).TotalMilliseconds >= LOOP_TIME)
            {
                LastTime = DateTime.Now;

                //// ReceiveSerial();
                //byte RelayControl;

                //RelayControl = RelayLo;
                //// RelayToAOG = 0;

                //bool ControllerConnected = ((DateTime.Now - ReceiveTime).TotalSeconds < 4);

                //FlowEnabled = (ControllerConnected && (RelayControl != 0) && (rateSetPoint > 0)) || (ControlType == ControlTypeEnum.Fan);

                FlowEnabled = ((DateTime.Now - ReceiveTime).TotalSeconds < 4)
                    && ((rateSetPoint > 0 && MasterOn)
                    || ((ControlType == ControlTypeEnum.Fan) && (rateSetPoint > 0))
                    || (!AutoOn && MasterOn));


                if (AutoOn)
                {
                    rateError = rateSetPoint - GetUPM();
                    switch (ControlType)
                    {
                        case ControlTypeEnum.Motor:
                        case ControlTypeEnum.MotorWeights:
                        case ControlTypeEnum.Fan:
                            // motor control
                            SimulateMotor(MaxPWM);
                            pwmSetting = PIDmotor((float)Kp, (float)Ki, (float)Kd, rateError, rateSetPoint, MinPWM, MaxPWM);
                            break;

                        default:
                            // valve control
                            SimulateValve(MinPWM, MaxPWM);
                            pwmSetting = PIDvalve((float)Kp, (float)Ki, (float)Kd, rateError, rateSetPoint, MinPWM, MaxPWM);
                            break;
                    }
                }
                else
                {
                    // manual control
                    pwmSetting = (float)ManualAdjust;
                    rateError = rateSetPoint - GetUPM();

                    switch (ControlType)
                    {
                        // calculate application rate
                        case ControlTypeEnum.Motor:
                        case ControlTypeEnum.MotorWeights:
                        case ControlTypeEnum.Fan:
                            // motor control
                            SimulateMotor(MaxPWM);
                            break;

                        default:
                            // valve control
                            SimulateValve(MinPWM, MaxPWM);
                            break;
                    }
                }

                if ((DateTime.Now - SendLast).TotalMilliseconds > SendTime)
                {
                    SendLast = DateTime.Now;
                    SendSerial();
                }
            }
        }

        public void ReceiveSerial(byte[] Data)
        {
            //PGN32614 to Arduino from Rate Controller, 16 bytes
            //0	HeaderLo		102
            //1	HeaderHi		127
            //2 Controller ID
            //3	relay Lo		0 - 7
            //4	relay Hi		8 - 15
            //5	rate set Lo		10 X actual
            //6 rate set Mid
            //7	rate set Hi		10 X actual
            //8	Flow Cal Lo		1000 X actual
            //9	Flow Cal Mid
            //10 Flow Cal Hi
            //11	Command
            //	        - bit 0		    reset acc.Quantity
            //	        - bit 1,2,3		control type 0-4
            //	        - bit 4		    MasterOn
            //          - bit 5         0 - average time for multiple pulses, 1 - time for one pulse
            //          - bit 6         AutoOn
            //12    power relay Lo      list of power type relays 0-7
            //13    power relay Hi      list of power type relays 8-15
            //14    manual pwm Lo
            //15    manual pwm Hi
            //16    crc

            byte InCommand;

            int PGN = Data[1] << 8 | Data[0];
            if (PGN == 32614)
            {
                SensorID = Data[2];

                RelayLo = Data[3];
                RelayHi = Data[4];

                // rate setting, 1000 times actual
                UInt32 RateSet = Data[5] | (UInt32)Data[6] << 8 | (UInt32)Data[7] << 16;
                rateSetPoint = (float)(RateSet * 0.001);

                // meter cal, 1000 times actual
                UInt32 Cal = Data[8] | (UInt32)Data[9] << 8 | (UInt32)Data[10] << 16;
                MeterCal = (float)(Cal * 0.001);

                // command byte
                InCommand = Data[11];
                if ((InCommand & 1) == 1) TotalPulses = 0;    // reset accumulated count

                ControlType = 0;
                if ((InCommand & 2) == 2) ControlType += 1;
                if ((InCommand & 4) == 4) ControlType += 2;
                if ((InCommand & 8) == 8) ControlType += 4;

                MasterOn = ((InCommand & 16) == 16);
                cUseMultiPulse = ((InCommand & 32) == 32);
                AutoOn = ((InCommand & 64) == 64);

                Int16 tmp = (short)(Data[14] | Data[15] << 8);
                ManualAdjust = tmp;

                ReceiveTime = DateTime.Now;
            }

            if (PGN == 32616)
            {
                UInt32 tmp = Data[3] | (UInt32)Data[4] << 8 | (UInt32)Data[5] << 16 | (UInt32)Data[6] << 24;
                Kp = (double)(tmp * 0.0001);

                tmp = Data[7] | (UInt32)Data[8] << 8 | (UInt32)Data[9] << 16 | (UInt32)Data[10] << 24;
                Ki = (double)(tmp * 0.0001);

                tmp = Data[11] | (UInt32)Data[12] << 8 | (UInt32)Data[13] << 16 | (UInt32)Data[14] << 24;
                Kd = (double)(tmp * 0.0001);

                MinPWM = Data[15];
                MaxPWM = Data[16];
            }

            if (rateSetPoint > MaxSimUPM)
            {
                MaxSimUPM = (float)(rateSetPoint * 1.5);
            }
        }

        DateTime CurrentAdjustTime;
        float ErrorPercentCum;
        float IntegralVal;
        float ErrorPercentLast;

        private int PIDmotor(float sKP, float sKI, float sKD, float sError, float sSetPoint, byte sMinPWM, byte sMaxPWM)
        {
            float Result = 0;
            float ErrorPercent = 0;

            if (FlowEnabled && sSetPoint > 0)
            {
                Result = LastPWM;
                ErrorPercent = (float)(sError / sSetPoint * 100.0);

                if (Math.Abs(ErrorPercent) > Deadband)
                {
                    Result += sKP * ErrorPercent;

                    UInt32 elapsedTime = (uint)(DateTime.Now - CurrentAdjustTime).TotalMilliseconds;
                    CurrentAdjustTime = DateTime.Now;

                    ErrorPercentCum += (float)(ErrorPercent * (elapsedTime * 0.001) * 0.001);

                    IntegralVal += sKI * ErrorPercentCum;
                    if (IntegralVal > 10) IntegralVal = 10;
                    if (IntegralVal < -10) IntegralVal = -10;
                    if (sKI == 0)
                    {
                        IntegralVal = 0;
                        ErrorPercentCum = 0;
                    }

                    Result += IntegralVal;

                    Result += (float)(sKD * (ErrorPercent - ErrorPercentLast) / (elapsedTime * 0.001) * 0.001);

                    ErrorPercentLast = ErrorPercent;

                    if (Result > sMaxPWM) Result = sMaxPWM;
                    if (Result < sMinPWM) Result = sMinPWM;
                }
            }
            else
            {
                IntegralVal = 0;
            }

            LastPWM = Result;
            return (int)Result;
        }

        private int PIDvalve(float sKP, float sKI, float sKD, float sError, float sSetPoint, byte sMinPWM, byte sMaxPWM)
        {
            float Result = 0;
            if (FlowEnabled && sSetPoint > 0)
            {
                float ErrorPercent = (float)(sError / sSetPoint * 100.0);
                if (Math.Abs(ErrorPercent) > Deadband)
                {
                    Result = sKP * ErrorPercent;

                    UInt32 elapsedTime = (uint)(DateTime.Now - CurrentAdjustTime).TotalMilliseconds;
                    CurrentAdjustTime = DateTime.Now;

                    ErrorPercentCum += (float)(ErrorPercent * (elapsedTime * 0.001) * 0.001);

                    IntegralVal += sKI * ErrorPercentCum;
                    if (IntegralVal > 10) IntegralVal = 10;
                    if (IntegralVal < -10) IntegralVal = -10;
                    if (sKI == 0)
                    {
                        IntegralVal = 0;
                        ErrorPercentCum = 0;
                    }

                    Result += IntegralVal;

                    Result += (float)(sKD * (ErrorPercent - ErrorPercentLast) / (elapsedTime * 0.001) * 0.001);

                    ErrorPercentLast = ErrorPercent;

                    bool IsPositive = (Result > 0);
                    Result = Math.Abs(Result);

                    if (Result < sMinPWM)
                    {
                        Result = sMinPWM;
                    }
                    else
                    {
                        if (ErrorPercent < BrakePoint)
                        {
                            if (Result > sMinPWM * 3.0) Result = (float)(sMinPWM * 3.0);
                        }
                        else
                        {
                            if (Result > sMaxPWM) Result = sMaxPWM;
                        }
                    }

                    if (!IsPositive) Result *= -1;
                }
            }
            else
            {
                IntegralVal = 0;
            }

            return (int)Result;
        }

        private float GetUPM()
        {
            if (ControlType == ControlTypeEnum.MotorWeights)
            {
                UPM = MeterCal * pwmSetting;
                return UPM;
            }
            else
            {
                return GetUPMflow();
            }
        }

        private float GetUPMflow()
        {
            double CurrentCounts;
            int RateInterval;

            if (pulseCount > 0)
            {
                CurrentCounts = pulseCount;
                pulseCount = 0;
                CurrentDuration = pulseDuration;

                if (cUseMultiPulse)
                {
                    // low ms/pulse, use pulses over time
                    TimedCounts += CurrentCounts;
                    RateInterval = (int)(DateTime.Now - TimedLast).TotalMilliseconds;
                    if (RateInterval > 500)
                    {
                        TimedLast = DateTime.Now;
                        PPM = (uint)((6000000 * TimedCounts) / RateInterval);  // 100 X actual
                        TimedCounts = 0;
                    }
                }
                else
                {
                    // high ms/pulse, use time for one pulse
                    if (CurrentDuration > 0)
                    {
                        PPM = (uint)(6000000 / CurrentDuration); // 100 X actual
                    }
                    else
                    {
                        PPM = 0;
                    }
                }

                LastPulse = DateTime.Now;
                TotalPulses += (float)CurrentCounts;
            }

            if ((DateTime.Now - LastPulse).TotalMilliseconds > 4000) PPM = 0;   // check for no flow
            // olympic average
            Osum += PPM;
            if (Omax < PPM) Omax = PPM;
            if (Omin > PPM) Omin = PPM;

            Ocount++;
            if (Ocount > 9)
            {
                Osum -= Omax;
                Osum -= Omin;
                Oave = (float)((float)Osum / 300.0);  // divide by 3 and divide by 100
                Osum = 0;
                Omax = 0;
                Omin = 500000000;
                Ocount = 0;
            }

            // units per minute
            if (MeterCal > 0)
            {
                UPM = Oave / MeterCal;
            }
            else
            {
                UPM = 0;
            }
            return UPM;
        }

        private void SendSerial()
        {
            // PGN 32613
            byte Temp;

            string[] words = new string[13];
            words[0] = "101";
            words[1] = "127";
            words[2] = SensorID.ToString();

            // rate applied, 1000 X actual
            Temp = (byte)(UPM * 1000);
            words[3] = Temp.ToString();
            Temp = (byte)((int)(UPM * 1000) >> 8);
            words[4] = Temp.ToString();
            Temp = (byte)((int)(UPM * 1000) >> 16);
            words[5] = Temp.ToString();

            // accumulated quantity
            if (MeterCal > 0)
            {
                int Units = (int)(TotalPulses * 10 / MeterCal);
                Temp = (byte)Units;
                words[6] = Temp.ToString();
                Temp = (byte)(Units >> 8);
                words[7] = Temp.ToString();
                Temp = (byte)(Units >> 16);
                words[8] = Temp.ToString();
            }
            else
            {
                words[6] = "0";
                words[7] = "0";
                words[8] = "0";
            }

            //pwmSetting
            Temp = (byte)((int)(pwmSetting));
            words[9] = Temp.ToString();
            Temp = (byte)((int)(pwmSetting) >> 8);
            words[10] = Temp.ToString();

            // status
            // bit 0    - sensor 0 receiving rate controller data
            // bit 1    - sensor 1 receiving rate controller data
            words[11] = "3";

            // crc
            words[12] = Prd.mf.Tls.CRC(words, 13).ToString();

            Prd.SerialFromAruduino(words, false);
        }

        private void SimulateMotor(byte sMax)
        {
            float SimTmp;

            if (FlowEnabled)
            {
                if (pwmSetting > 250) pwmSetting = 255;
                if (pwmSetting < -250) pwmSetting = -255;

                SimulateInterval = (int)(DateTime.Now - SimulateTimeLast).TotalMilliseconds;
                SimulateTimeLast = DateTime.Now;

                SimRPM += (float)(((pwmSetting / (float)sMax) * MaxRPM - SimRPM) * 0.35);    // update rpm
                Prd.mf.Tls.NoisyData(SimRPM, ErrorRange);    // add error
                if (SimRPM < 0) SimRPM = 0;

                SimTmp = PPR * SimRPM;
                if (SimTmp > 0)
                {
                    pulseDuration = ((float)(60000.0 / SimTmp));
                }
                else
                {
                    pulseDuration = 0;
                }

                double CurCount = SimRPM * PPR;
                PrevCount += CurCount * (SimulateInterval / 60000.0); // counts for time slice
                if (PrevCount > 1)
                {
                    pulseCount = PrevCount;
                    PrevCount = 0;
                }
            }
            else
            {
                pulseCount = 0;
            }
        }

        private void SimulateValve(byte Min, byte Max)
        {
            float PulseTime;
            float SimUPM;

            SimulateInterval = (int)(DateTime.Now - SimulateTimeLast).TotalMilliseconds;
            SimulateTimeLast = DateTime.Now;

            if (FlowEnabled)
            {
                float Range = Max - Min + 5;
                if (Range == 0 || pwmSetting == 0)
                {
                    ValveAdjust = 0;
                }
                else
                {
                    float Percent = (float)((Math.Abs(pwmSetting) - Min + 5) / Range);
                    if (pwmSetting < 0)
                    {
                        Percent *= -1;
                    }

                    ValveAdjust = (float)(Percent * (float)(SimulateInterval / ValveOpenTime) * 100.0);
                }

                ValveOpen += ValveAdjust;
                if (ValveOpen < 0) ValveOpen = 0;
                if (ValveOpen > 100) ValveOpen = 100;
            }
            else
            {
                ValveOpen = 0;
            }

            SimUPM = (float)(MaxSimUPM * ValveOpen / 100.0);

            Pulses = (float)((SimUPM * MeterCal) / 60000.0);  // (Units/min * pulses/Unit) = pulses/min / 60000 = pulses/millisecond
            if (Pulses == 0)
            {
                pulseCount = 0;
                pulseDuration = 0;
            }
            else
            {
                PulseTime = (float)(1.0 / Pulses);   // milliseconds for each pulse

                PulseTime = (float)Prd.mf.Tls.NoisyData(PulseTime, ErrorRange);
                PrevCount += SimulateInterval / PulseTime; // milliseconds * pulses/millsecond = pulses
                if (PrevCount > 1)
                {
                    pulseCount = PrevCount;
                    PrevCount = 0;
                }

                // pulse duration is the time for one pulse
                pulseDuration = PulseTime;
            }
        }
    }
}