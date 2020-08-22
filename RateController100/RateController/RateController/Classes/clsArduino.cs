using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace RateController
{
    public class clsArduino
    {
        private string Sentence;
        byte Temp;

        int accumulatedCounts;
        float MeterCal;
        float pwmSetting;

        private readonly CRateCals RC;
        byte RelayHi;
        byte RelayFromAOG;

        float rateSetPoint;
        byte InCommand;

        byte MinPWMvalue;
        byte MaxPWMvalue;

        bool AOGconnected;
        int Tmp;
        byte ValveType;

        bool SimulateFlow;
        byte RelayControl;
        bool RelaysOn;

        DateTime RateCheckLast;
        int RateCheckInterval = 300;
        float rateError;

        bool AutoOn;
        bool RateUpMan;
        bool RateDownMan;
        int pwmManualRatio;

        int pulseCount;
        int pulseDuration;

        float FlowRate;

        DateTime LastTime;
        int LOOP_TIME = 200;

        float ValveAdjust = 0;   // % amount to open/close valve
        float ValveOpen = 0;      // % valve is open
        float Pulses = 0;
        float ValveOpenTime = 4000;  // ms to fully open valve at max opening rate
        float UPM = 0;     // simulated units per minute
        float MaxRate = 120;  // max rate of system in UPM
        int ErrorRange = 4;  // % random error in flow rate, above and below target
        float PulseTime = 0;

        DateTime SimulateTimeLast;
        int SimulateInterval;
        float RandomError;

        double Frequency;
        int CurrentCounts;

        long OldVCN;
        byte VCNbacklash;
        byte VCNspeed;
        byte VCNbrake;
        byte VCNdeadband;

        int NewPWM;
        float VCNerror;

        DateTime SendStart;
        DateTime WaitStart;

        byte AdjustmentState = 0;	// 0 waiting, 1 sending pwm

        long SendTime = 300;    // ms pwm is sent to valve
        long WaitTime = 750;    // ms to wait before adjusting valve again
        byte SlowSpeed = 9;     // low pwm rate
        long VCN = 743;

        bool LastDirectionPositive;     // adjustment direction
        bool UseBacklashAdjustment;
        int PartsTemp;

        float KalResult = 0.0F;
        float KalPc = 0.0F;
        float KalG = 0.0F;
        float KalP = 1.0F;
        float KalVariance = 0.01F;   // larger is more filtering
        float KalProcess = 0.005F;	// smaller is more filtering

        public clsArduino(CRateCals CalledFrom)
        {
            RC = CalledFrom;
        }

        public void MainLoop()
        {
            // ReceiveSerial();

            RelayControl = RelayFromAOG;
            // RelayToAOG = 0;
            AutoOn = true;

            RelaysOn = (AOGconnected & (RelayControl != 0));

            if ((DateTime.Now - RateCheckLast).TotalMilliseconds > RateCheckInterval)
            {
                RateCheckLast = DateTime.Now;
                if (RelaysOn)
                {
                    if (SimulateFlow) DoSimulate();
                    rateError = CalRateError();
                }
            }

            if(RelaysOn && AutoOn)
            {
                pwmSetting = VCNpwm(rateError, rateSetPoint, MinPWMvalue, MaxPWMvalue, VCN, FlowRate,
                    SendTime, WaitTime, SlowSpeed, ValveType);
            }

            if ((DateTime.Now - LastTime).TotalMilliseconds >= LOOP_TIME)
            {
                LastTime = DateTime.Now;

                SendSerial();
            }
        }

        private void SendSerial()
        {
            // PGN 32741

            Sentence = "127,229,";

            // rate applied
            Temp = (byte)((int)(FlowRate * 100) >> 8);
            Sentence += Temp.ToString();
            Sentence += ",";
            Temp = (byte)(FlowRate * 100);
            Sentence += Temp.ToString();
            Sentence += ",";

            // accumulated quantity
            int Units = (int)(accumulatedCounts * 100 / MeterCal);
            Temp = (byte)(Units >> 16);
            Sentence += Temp.ToString();
            Sentence += ",";
            Temp = (byte)(Units >> 8);
            Sentence += Temp.ToString();
            Sentence += ",";
            Temp = (byte)Units;
            Sentence += Temp.ToString();
            Sentence += ",";

            //pwmSetting
            Temp = (byte)((int)((300 - pwmSetting) * 10) >> 8);
            Sentence += Temp.ToString();
            Sentence += ",";
            Temp = (byte)((300 - pwmSetting) * 10);
            Sentence += Temp.ToString();
            Sentence += ",";

            Sentence += "0";

            Sentence += "\r";

            RC.CommFromArduino(Sentence, false);
        }

        public void ReceiveSerial(byte[] Data)
        {
            int PGN = Data[0] << 8 | Data[1];

            if (PGN == 32742)
            {
                RelayHi = Data[2];
                RelayFromAOG = Data[3];

                // rate setting, 100 times actual
                Tmp = Data[4] << 8 | Data[5];
                rateSetPoint = (float)(Tmp * .01);

                // meter cal, 100 times actual
                Tmp = Data[6] << 8 | Data[7];
                MeterCal = (float)(Tmp * .01);

                // command byte
                InCommand = Data[8];
                if ((InCommand & 1) == 1) accumulatedCounts = 0;    // reset accumulated count

                ValveType = 0;
                if ((InCommand & 2) == 2) ValveType += 1;
                if ((InCommand & 4) == 4) ValveType += 2;

                SimulateFlow = ((InCommand & 8) == 8);

                AOGconnected = true;
            }

            if (PGN == 32744)
            {
                VCN = (int)(Data[2] << 8 | Data[3]);
                SendTime = (int)(Data[4] << 8 | Data[5]);
                WaitTime = (int)(Data[6] << 8 | Data[7]);
                MaxPWMvalue = Data[8];
                MinPWMvalue = Data[9];

                AOGconnected = true;
            }
        }

        void DoSimulate()
        {
            var Rand = new Random();

            SimulateInterval = (int)(DateTime.Now - SimulateTimeLast).TotalMilliseconds;
            SimulateTimeLast = DateTime.Now;

            if (RelaysOn)
            {
                // relays on
                if (AutoOn)
                {
                    float Range = MaxPWMvalue - MinPWMvalue + 5;
                    if (Range == 0 | pwmSetting == 0)
                    {
                        ValveAdjust = 0;
                    }
                    else
                    {
                        float Percent = (float)((Math.Abs(pwmSetting) - MinPWMvalue + 5) / Range);
                        if (pwmSetting < 0)
                        {
                            Percent *= -1;
                        }

                        ValveAdjust = (float)(Percent * (float)(SimulateInterval / ValveOpenTime) * 100.0);
                    }
                }
                else
                {
                    // manual control
                    ValveAdjust = 0;
                    if (RateUpMan) ValveAdjust = (float)((SimulateInterval / ValveOpenTime) * 100.0 * (pwmManualRatio / 100));
                    if (RateDownMan) ValveAdjust = (float)((SimulateInterval / ValveOpenTime) * -100.0 * (pwmManualRatio / 100));
                }

                ValveOpen += ValveAdjust;
                if (ValveOpen < 0) ValveOpen = 0;
                if (ValveOpen > 100) ValveOpen = 100;
            }
            else
            {
                // relays off
                ValveOpen = 0;
            }

            UPM = (float)(MaxRate * ValveOpen / 100.0);

            Pulses = (float)((UPM * MeterCal) / 60000.0);  // (Units/min * pulses/Unit) = pulses/min / 60000 = pulses/millisecond
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
                pulseCount = (int)(SimulateInterval / PulseTime); // milliseconds * pulses/millsecond = pulses

                // pulse duration is the total time for all pulses in the loop
                //pulseDuration = (int)(PulseTime * pulseCount);

                // pulse duration is the time for one pulse
                pulseDuration = (int)PulseTime;
            }

        }

        float CalRateError()
        {
            // measure time for one pulse
            CurrentCounts = pulseCount;
            pulseCount = 0;
            accumulatedCounts += CurrentCounts;

            if(pulseDuration==0 | MeterCal==0)
            {
                FlowRate = 0;
            }
            else
            {
                Frequency = (1.0 / (double)pulseDuration) * 60000.0;    // pulses per minute
                FlowRate = (float)(Frequency / MeterCal);    // units per minute
            }

            //return rateSetPoint - FlowRate;

            // Kalmen filter
            KalPc = KalP + KalProcess;
            KalG = KalPc / (KalPc + KalVariance);
            KalP = (1 - KalG) * KalPc;
            KalResult = KalG * (FlowRate - KalResult) + KalResult;

            return rateSetPoint - KalResult;
        }

        //float CalRateError()
        //{
        //    // measure pulses over a time period
        //    Duration = (DateTime.Now - LastFlowCal).TotalMilliseconds / 60000.0;    // minutes
        //    LastFlowCal = DateTime.Now;
        //    CurrentCounts = pulseCount;
        //    pulseCount = 0;
        //    accumulatedCounts += CurrentCounts;

        //    if (Duration == 0 | MeterCal == 0)
        //    {
        //        FlowRate = 0;
        //    }
        //    else
        //    {
        //        Frequency = CurrentCounts / Duration;
        //        FlowRate = (float)(Frequency / MeterCal);    // units per minute
        //    }

        //    //return rateSetPoint - FlowRate;

        //    //Kalmen filter
        //    KalPc = KalP + KalProcess;
        //    KalG = KalPc / (KalPc + KalVariance);
        //    KalP = (1 - KalG) * KalPc;
        //    KalResult = KalG * (FlowRate - KalResult) + KalResult;

        //    return rateSetPoint - KalResult;
        //}

        int VCNpwm(float cError, float cSetPoint, byte MinPWM, byte MaxPWM, long cVCN,
                    float cFlowRate, long cSendTime, long cWaitTime, byte cSlowSpeed, byte cValveType)
        {
            VCNparts(cVCN);

            // deadband
            float DB = (float)(VCNdeadband / 100.0) * cSetPoint;
            if (Math.Abs(cError) <= DB)
            {
                // valve does not need to be adjusted
                NewPWM = 0;
            }
            else
            {
                // backlash
                if (!UseBacklashAdjustment && VCNbacklash > 0)
                {
                    if ((cError >= 0 && !LastDirectionPositive) | (cError < 0 && LastDirectionPositive))
                    {
                        UseBacklashAdjustment = true;
                        SendStart = DateTime.Now;
                    }
                    LastDirectionPositive = (cError >= 0);
                }

                if (UseBacklashAdjustment)
                {
                    if ((DateTime.Now - SendStart).TotalMilliseconds > (VCNbacklash * 10))
                    {
                        UseBacklashAdjustment = false;
                        LastDirectionPositive = (cError >= 0);
                        SendStart = DateTime.Now;
                    }
                    else
                    {
                        NewPWM = MaxPWM - (cSlowSpeed * (MaxPWM - MinPWM) / 9);
                        if (cError < 0) NewPWM *= -1;
                    }
                }
                else
                {
                    if (AdjustmentState == 0)
                    {
                        // waiting
                        if ((DateTime.Now - WaitStart).TotalMilliseconds > cWaitTime)
                        {
                            // waiting finished
                            AdjustmentState = 1;
                            SendStart = DateTime.Now;
                        }
                    }

                    if (AdjustmentState == 1)
                    {
                        // sending pwm
                        if ((DateTime.Now - SendStart).TotalMilliseconds > cSendTime)
                        {
                            // sending finished
                            AdjustmentState = 0;
                            WaitStart = DateTime.Now;
                            NewPWM = 0;
                        }
                        else
                        {
                            // get new pwm value
                            if (cFlowRate == 0 && cValveType == 1)
                            {
                                // open 'fast close' valve
                                NewPWM = MaxPWM;
                            }
                            else
                            {
                                // % error
                                if (cSetPoint > 0)
                                {
                                    VCNerror = (float)((cError / cSetPoint) * 100.0);
                                }
                                else
                                {
                                    VCNerror = 0;
                                }

                                // set pwm value
                                if (Math.Abs(VCNerror) < VCNbrake)
                                {
                                    // slow adjustment
                                    NewPWM = MaxPWM - ((MaxPWM - MinPWM) * cSlowSpeed / 9);
                                }
                                else
                                {
                                    // normal adjustment
                                    NewPWM = MaxPWM - ((MaxPWM - MinPWM) * VCNspeed / 9);
                                }

                                if (cError < 0) NewPWM *= -1;
                            }
                        }
                    }
                }
            }
            Debug.Print(NewPWM.ToString());
            return NewPWM;
        }

        void VCNparts(long NewVCN)
        {
            if ((NewVCN != OldVCN) && (NewVCN <= 9999) && (NewVCN >= 0))
            {
                VCNbacklash = (byte)(NewVCN / 1000);
                PartsTemp = (int)(NewVCN - VCNbacklash * 1000);

                VCNspeed = (byte)(PartsTemp / 100);
                PartsTemp = PartsTemp - VCNspeed * 100;

                VCNbrake = (byte)(PartsTemp / 10);

                VCNdeadband = (byte)(PartsTemp - VCNbrake * 10);

                VCNbrake *= 10;
                if (VCNbrake == 0) VCNbrake = 5;

                OldVCN = NewVCN;
            }
        }
    }
}
