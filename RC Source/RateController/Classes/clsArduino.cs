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

        private float CurrentCounts;
        private float CurrentDuration;
        private int ErrorRange = 4;        // % random error in flow rate, above and below target

        private byte InCommand;

        private float KalG = 0.0F;
        private float KalP = 1.0F;
        private float KalPc = 0.0F;
        private float KalProcess = 0.005F;        // smaller is more filtering
        private float KalResult = 0.0F;
        private float KalVariance = 0.01F;        // larger is more filtering

        private DateTime LastPulse = DateTime.Now;
        private float LastPWM;
        private DateTime LastTime;
        private int LOOP_TIME = 50;
        private float MaxSimRate = 150F;        // max rate of system in UPM

        private float MaxRPM = 100.0F;
        private int mcID;
        private float MeterCal;

        // PID
        private byte PIDbrakePoint = 20;
        private byte PIDdeadband = 3;
        private byte PIDHighMax = 255;
        private byte PIDkp = 20;
        private byte PIDLowMax = 100;
        private byte PIDminPWM = 50;

        private float PPM;
        private float PPR = 50.0F;        // pulses per revolution

        private float pulseCount;
        private float pulseDuration;
        private float Pulses = 0;

        private float PulseTime = 0;

        private float pwmSetting;
        private float RandomError;
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

        private float TimedCounts;
        private DateTime TimedLast = DateTime.Now;
        private int Tmp;
        private float TotalPulses;

        private float UPM;

        private float ValveAdjust = 0;   // % amount to open/close valve
        private float ValveOpen = 0;      // % valve is open
        private float ValveOpenTime = 4000;  // ms to fully open valve at max opening rate

        private float NewRateFactor;
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
                            if (SimulateFlow) SimulateMotor(PIDHighMax);
                            rateError = rateSetPoint - GetUPM();

                            pwmSetting = ControlMotor(PIDkp, rateError, rateSetPoint, PIDminPWM,
                                PIDHighMax, PIDdeadband);
                            break;

                        default:
                            // valve control

                                if (SimulateFlow) SimulateValve(PIDminPWM, PIDHighMax);
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
                                pwmSetting *= NewRateFactor;
                                if (pwmSetting == 0 & NewRateFactor > 1) pwmSetting = PIDminPWM;
                                break;

                            default:
                                // valve control
                                if (NewRateFactor < 1)
                                {
                                    // rate down
                                    pwmSetting = (1 - NewRateFactor) * ((PIDHighMax + PIDminPWM) / 2) * -1;
                                }
                                else
                                {
                                    // rate up
                                    pwmSetting = (NewRateFactor - 1) * ((PIDHighMax + PIDminPWM) / 2);
                                }
                                break;
                        }
                    }

                    switch (ControlType)
                    {
                        // calculate application rate
                        case 2:
                            // motor control
                            if (SimulateFlow) SimulateMotor(PIDHighMax);
                            rateError = rateSetPoint - GetUPM();
                            break;

                        default:
                            // valve control
                            if (SimulateFlow) SimulateValve(PIDminPWM, PIDHighMax);
                            rateError = rateSetPoint - GetUPM();
                            break;
                    }
                }
                SendSerial();
            }
        }

        public void ReceiveSerial(byte[] Data)
        {
            int PGN = Data[0] << 8 | Data[1];
            if (PGN == 32614)
            {
                mcID = Data[2];

                RelayHi = Data[3];
                RelayFromAOG = Data[4];

                // rate setting, 10 times actual
                double TmpSetting = Data[5] << 16 | Data[6] << 8 | Data[7];

                // meter cal, 100 times actual
                Tmp = Data[8] << 8 | Data[9];
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
                    rateSetPoint = (float)(TmpSetting * .1);
                }
                else
                {
                    NewRateFactor = (float)(TmpSetting * .1);
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

        private float GetUPM()
        {
            CurrentCounts = pulseCount;
            pulseCount = 0;

                if (pulseDuration > 5) CurrentDuration = pulseDuration;

            // check for no PPM
            if ((DateTime.Now - LastPulse).TotalMilliseconds > 4000)
            {
                pulseDuration = 0;
                CurrentDuration = 0;
                PPM = 0;
            }
            if (CurrentCounts > 0) LastPulse = DateTime.Now;

            // accumulated total
            TotalPulses += CurrentCounts;

            // ppm
            if (cUseMultiPulse)
            {
                // low ms/pulse
                TimedCounts += CurrentCounts;
                RateInterval = (int)(DateTime.Now - TimedLast).TotalMilliseconds;
                if (RateInterval > 200)
                {
                    TimedLast = DateTime.Now;
                    PPM = (float)((60000.0 * TimedCounts) / RateInterval);
                    TimedCounts = 0;
                }
            }
            else
            {
                // high ms/pulse
                if (CurrentDuration > 0) PPM = ((float)(60000.0 / CurrentDuration));
            }

            // Kalmen filter
            KalPc = KalP + KalProcess;
            KalG = KalPc / (KalPc + KalVariance);
            KalP = (float)((1.0 - KalG) * KalPc);
            KalResult = KalG * (PPM - KalResult) + KalResult;
            PPM = KalResult;

            // units per minute
            if (MeterCal > 0)
            {
                UPM = PPM / MeterCal;
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
            string[] words = new string[11];
            words[0] = "127";
            words[1] = "101";
            words[2] = mcID.ToString();

            // rate applied, 10 X actual
            Temp = (byte)((int)(UPM * 10) >> 16);
            words[3] = Temp.ToString();
            Temp = (byte)((int)(UPM * 10) >> 8);
            words[4] = Temp.ToString();
            Temp = (byte)(UPM * 10);
            words[5] = Temp.ToString();

            // accumulated quantity
            int Units = (int)(TotalPulses * 100 / MeterCal);
            Temp = (byte)(Units >> 16);
            words[6] = Temp.ToString();
            Temp = (byte)(Units >> 8);
            words[7] = Temp.ToString();
            Temp = (byte)Units;
            words[8] = Temp.ToString();

            //pwmSetting
            Temp = (byte)((int)(pwmSetting * 10) >> 8);
            words[9] = Temp.ToString();
            Temp = (byte)((int)(pwmSetting * 10));
            words[10] = Temp.ToString();

            RC.SerialFromAruduino(words, false);
        }

        private void SimulateMotor( byte sMax)
        {
            if (ApplicationOn)
            {
                var Rand = new Random();

                SimulateInterval = (int)(DateTime.Now - SimulateTimeLast).TotalMilliseconds;
                SimulateTimeLast = DateTime.Now;

                SimRPM += (float)(((pwmSetting / (float)sMax) * MaxRPM - SimRPM) * 0.25);    // update rpm
                RandomError = (float)((100.0 - ErrorRange) + (Rand.Next((int)(ErrorRange * 2.0))));
                SimRPM = (float)(SimRPM * RandomError / 100.0);
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

                pulseCount = (float)(SimRPM * PPR);
                pulseCount = (float)(pulseCount * (SimulateInterval / 60000.0)); // counts for time slice
            }
            else
            {
                pulseCount = 0;
            }
        }

        private void SimulateValve(byte Min, byte Max)
        {
            var Rand = new Random();

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

                RandomError = (100 - ErrorRange) + (Rand.Next(ErrorRange * 2));

                PulseTime = (float)(PulseTime * RandomError / 100);
                pulseCount = (float)(SimulateInterval / PulseTime); // milliseconds * pulses/millsecond = pulses

                // pulse duration is the time for one pulse
                pulseDuration = PulseTime;
            }
        }
    }
}