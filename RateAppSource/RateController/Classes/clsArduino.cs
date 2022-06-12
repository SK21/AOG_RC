using System;
using System.Diagnostics;

namespace RateController
{
    public class clsArduino
    {
        private readonly clsProduct RC;

        private bool ApplicationOn = false;
        private bool AutoOn = true;
        private bool ControllerConnected;
        private byte ControlType;        // 0 standard, 1 fast close, 2 motor

        private int ErrorRange = 4;        // % random error in flow rate, above and below target

        private byte InCommand;

        private DateTime LastPulse = DateTime.Now;
        private float LastPWM;
        private DateTime LastTime;
        private int LOOP_TIME = 50;
        private float MaxSimRate = 40F;        // max rate of system in UPM

        private byte MaxRPM = 255;
        private int mcID;
        private float MeterCal;

        // PID
        private byte PIDbrakePoint = 20;
        private byte PIDdeadband = 3;
        private byte PIDHighMax = 255;
        private byte PIDkp = 20;
        private byte PIDLowMax = 100;
        private byte PIDminPWM = 50;

        private float PPR = 1.0F;        // pulses per revolution

        private UInt16 pulseCount;
        private float pulseDuration;
        private float Pulses = 0;

        private float PulseTime = 0;

        private float pwmSetting;
        private float rateError;
        private int RateInterval;
        private float rateSetPoint;
        private DateTime ReceiveTime;
        private byte RelayControl;
        private byte RelayFromAOG;
        private byte RelayHi;

        private float SimRPM = 0.0F;

        private float SimTmp;
        private bool SimulateFlow;
        private int SimulateInterval;
        private DateTime SimulateTimeLast;
        private float SimUPM = 0;        // simulated units per minute

        private byte Temp;

        private DateTime TimedLast = DateTime.Now;
        private int Tmp;
        private float TotalPulses;

        private float UPM;

        private float ValveAdjust = 0;   // % amount to open/close valve
        private float ValveOpen = 0;      // % valve is open
        private float ValveOpenTime = 4000;  // ms to fully open valve at max opening rate

        private float ManualAdjust;
        private DateTime ManualLast;

        private bool cUseMultiPulse;

        public clsArduino(clsProduct CalledFrom)
        {
            RC = CalledFrom;
        }

        public void MainLoop()
        {
            // ReceiveSerial();

            RelayControl = RelayFromAOG;
            // RelayToAOG = 0;

            ControllerConnected = ((DateTime.Now - ReceiveTime).TotalSeconds < 4);

            ApplicationOn = (ControllerConnected & (RelayControl != 0) & (rateSetPoint > 0));

            if ((DateTime.Now - LastTime).TotalMilliseconds >= LOOP_TIME)
            {
                LastTime = DateTime.Now;
                if (AutoOn)
                {
                    switch (ControlType)
                    {
                        case 2:
                            // motor control
                            SimulateMotor(PIDHighMax);
                            rateError = rateSetPoint - GetUPM();

                            pwmSetting = ControlMotor(PIDkp, rateError, rateSetPoint, PIDminPWM,
                                PIDHighMax, PIDdeadband);
                            break;

                        default:
                            // valve control

                                SimulateValve(PIDminPWM, PIDHighMax);
                                rateError = rateSetPoint - GetUPM();
                                pwmSetting = DoPID(PIDkp, rateError, rateSetPoint, PIDminPWM, PIDLowMax,
                                    PIDHighMax, PIDbrakePoint, PIDdeadband);
                            break;
                    }
                }
                else
                {
                    // manual control
                    if ((DateTime.Now - ManualLast).TotalSeconds > 1.5)
                    {
                        ManualLast = DateTime.Now;

                        // adjust rate
                        if (rateSetPoint == 0) rateSetPoint = 1;

                        switch (ControlType)
                        {
                            case 2:
                                // motor control
                                if (ManualAdjust > 0)
                                {
                                    pwmSetting *= (float)1.10;
                                    if (pwmSetting < 1) pwmSetting = PIDminPWM;
                                }
                                else if (ManualAdjust < 0)
                                {
                                    pwmSetting *= (float)0.90;
                                    if (pwmSetting < PIDminPWM) pwmSetting = 0;
                                }
                                break;

                            default:
                                // valve control
                                pwmSetting = ManualAdjust;
                                break;
                        }
                    }

                    switch (ControlType)
                    {
                        // calculate application rate
                        case 2:
                            // motor control
                            SimulateMotor(PIDHighMax);
                            rateError = rateSetPoint - GetUPM();
                            break;

                        default:
                            // valve control
                            SimulateValve(PIDminPWM, PIDHighMax);
                            rateError = rateSetPoint - GetUPM();
                            break;
                    }
                }
                SendSerial();
            }
        }

        public void ReceiveSerial(byte[] Data)
        {
            int PGN = Data[1] << 8 | Data[0];
            if (PGN == 32614)
            {
                mcID = Data[2];

                RelayFromAOG = Data[3];
                RelayHi = Data[4];

                // rate setting, 10 times actual
                double TmpSetting = ((short)(Data[7] << 16 | Data[6] << 8 | Data[5])) * 0.1;

                // meter cal, 100 times actual
                Tmp = Data[9] << 8 | Data[8];
                MeterCal = (float)(Tmp * .01);

                // command byte
                InCommand = Data[10];
                if ((InCommand & 1) == 1) TotalPulses = 0;    // reset accumulated count

                ControlType = 0;
                if ((InCommand & 2) == 2) ControlType += 1;
                if ((InCommand & 4) == 4) ControlType += 2;

                SimulateFlow = ((InCommand & 8) == 8);

                cUseMultiPulse = ((InCommand & 16) == 16);

                AutoOn = ((InCommand & 32) == 32);
                if (AutoOn)
                {
                    rateSetPoint = (float)(TmpSetting);
                }
                else
                {
                    ManualAdjust = (float)(TmpSetting);
                }

                ReceiveTime = DateTime.Now;
            }

            if (PGN == 32616)
            {
                byte ConID = Data[2];
                PIDkp = Data[3];
                PIDminPWM = Data[4];
                PIDLowMax = Data[5];
                PIDHighMax = Data[6];
                PIDdeadband = Data[7];
                PIDbrakePoint = Data[8];

                ReceiveTime = DateTime.Now;
            }

            MaxSimRate = (float)(RC.TargetRate() * 1.5);
        }

        private int ControlMotor(byte sKP, float sError, float sSetPoint, byte sMinPWM,
             byte sHighMax, byte sDeadband)
        {
            float Result = 0;
            float ErrorPercent = 0;
            if (ApplicationOn)
            {
                Result = LastPWM;
                ErrorPercent = (float)(Math.Abs(sError / sSetPoint) * 100.0);
                float Max = (float)sHighMax;

                if (ErrorPercent > (float)sDeadband)
                {
                    Result += (float)((float)sKP / 255.0) * sError;

                    if (Result > Max) Result = Max;
                    if (Result < sMinPWM) Result = (float)sMinPWM;
                }
            }

            LastPWM = Result;
            return (int)Result;
        }

        private int DoPID(byte clKP, float clError, float clSetPoint, byte clMinPWM, byte clLowMax, byte clHighMax, byte clBrakePoint, byte clDeadband)
        {
            try
            {
                int Result = 0;
                if (ApplicationOn)
                {
                    float ErrorPercent = Math.Abs(clError / clSetPoint);
                    float ErrorBrake = (float)((float)(clBrakePoint / 100.0));
                    float Max = (float)clHighMax;

                    if (ErrorPercent > ((float)(clDeadband / 100.0)))
                    {
                        if (ErrorPercent <= ErrorBrake)
                        {
                            Max = (ErrorPercent / ErrorBrake) * clLowMax;
                        }

                        Result = (int)(clKP * clError);

                        bool IsPositive = (Result > 0);
                        Result = Math.Abs(Result);
                        if (Result > Max) Result = (int)Max;
                        if (Result < clMinPWM) Result = clMinPWM;
                        if (!IsPositive) Result *= -1;
                    }
                }
                return Result;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        UInt32 Osum;
        UInt32 Omax;
        UInt32 Omin;
        UInt16 Ocount;
        float Oave;
        UInt32 PPM;
        UInt16 TimedCounts;
        UInt16 CurrentCounts;
        UInt16 CurrentDuration;

        private float GetUPM()
        {
            if (pulseCount > 0)
            {
                CurrentCounts = pulseCount;
                pulseCount = 0;
                CurrentDuration = (ushort)pulseDuration;

                if (cUseMultiPulse)
                {
                    // low ms/pulse, use pulses over time
                    TimedCounts += CurrentCounts;
                    RateInterval = (int)(DateTime.Now - TimedLast).TotalMilliseconds;
                    if (RateInterval > 200)
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
                TotalPulses += CurrentCounts;
            }

            if ((DateTime.Now - LastPulse).TotalMilliseconds > 4000) PPM = 0;   // check for no flow

            // olympic average
            Osum += PPM;
            if (PPM > Omax) Omax = PPM;
            if (PPM < Omin) Omin = PPM;

            Ocount++;
            if (Ocount > 4)
            {
                Osum -= Omax;
                Osum -= Omin;
                Oave = (float)((float)Osum / 300.0);  // divide by 3 and divide by 100 
                Osum = 0;
                Omax = 0;
                Omin = 50000;
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
            string[] words = new string[12];
            words[0] = "101";
            words[1] = "127";
            words[2] = mcID.ToString();

            // rate applied, 10 X actual
            Temp = (byte)(UPM * 10);
            words[3] = Temp.ToString();
            Temp = (byte)((int)(UPM * 10) >> 8);
            words[4] = Temp.ToString();
            Temp = (byte)((int)(UPM * 10) >> 16);
            words[5] = Temp.ToString();

            // accumulated quantity
            int Units = (int)(TotalPulses * 10 / MeterCal);
            Temp = (byte)Units;
            words[6] = Temp.ToString();
            Temp = (byte)(Units >> 8);
            words[7] = Temp.ToString();
            Temp = (byte)(Units >> 16);
            words[8] = Temp.ToString();

            //pwmSetting
            Temp = (byte)((int)(pwmSetting * 10));
            words[9] = Temp.ToString();
            Temp = (byte)((int)(pwmSetting * 10) >> 8);
            words[10] = Temp.ToString();

            // crc
            words[11] = RC.mf.Tls.CRC(words, 12).ToString();

            RC.SerialFromAruduino(words, false);
        }

        double PrevCount;
        double CurCount;
        private void SimulateMotor( byte sMax)
        {
            if (ApplicationOn)
            {
                if (pwmSetting > 250) pwmSetting = 255;
                if (pwmSetting < -250) pwmSetting = -255;

                SimulateInterval = (int)(DateTime.Now - SimulateTimeLast).TotalMilliseconds;
                SimulateTimeLast = DateTime.Now;

                SimRPM += (float)(((pwmSetting / (float)sMax) * MaxRPM - SimRPM) * 0.35);    // update rpm
                RC.mf.Tls.NoisyData(SimRPM, ErrorRange);    // add error
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

                CurCount = SimRPM * PPR;
                PrevCount += CurCount * (SimulateInterval / 60000.0); // counts for time slice
                if(PrevCount>1)
                {
                    pulseCount = (ushort)PrevCount;
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
            SimulateInterval = (int)(DateTime.Now - SimulateTimeLast).TotalMilliseconds;
            SimulateTimeLast = DateTime.Now;

            if (ApplicationOn)
            {
                float Range = Max - Min + 5;
                if (Range == 0 | pwmSetting == 0)
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

            SimUPM = (float)(MaxSimRate * ValveOpen / 100.0);

            Pulses = (float)((SimUPM * MeterCal) / 60000.0);  // (Units/min * pulses/Unit) = pulses/min / 60000 = pulses/millisecond
            if (Pulses == 0)
            {
                pulseCount = 0;
                pulseDuration = 0;
            }
            else
            {
                PulseTime = (float)(1.0 / Pulses);   // milliseconds for each pulse

                PulseTime = (float)RC.mf.Tls.NoisyData(PulseTime, ErrorRange);
                PrevCount += SimulateInterval / PulseTime; // milliseconds * pulses/millsecond = pulses
                if(PrevCount>1)
                {
                    pulseCount = (ushort)PrevCount;
                    PrevCount = 0;
                }

                // pulse duration is the time for one pulse
                pulseDuration = PulseTime;
            }
        }
    }
}