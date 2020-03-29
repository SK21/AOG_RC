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
        float FlowRateFiltered;
        byte Temp;

        int accumulatedCounts;
        float MeterCal;
        float pwmSetting;

        private readonly CRateCals RC;
        byte RelayHi;
        byte RelayFromAOG;

        float rateSetPoint;
        byte InCommand;

        float KP;
        float KI;
        float KD;

        float DeadBand;
        byte MinPWMvalue;
        byte MaxPWMvalue;

        bool AOGconnected;
        int Tmp;
        byte ValveType;

        bool SimulateFlow;
        byte RelayControl;
        bool RelaysOn;

        DateTime RateCheckLast;
        int RateCheckInterval = 1000;
        float rateError;

        bool AutoOn;
        bool RateUpMan;
        bool RateDownMan;

        int pwmManualRatio;
        int pulseCount;
        int pulseDuration;

        float countsThisLoop;
        float pulseAverage;
        float FlowRate;

        float Pc;
        float P;
        const float varProcess = 10.0F;

        const float varRate = 10.0F;
        float Xp;
        float Zp;

        float G;
        DateTime LastTime;
        int LOOP_TIME = 200;

        float clErrorLast = 0;    // errorlast is the error in the previous iteration of the control loop
        float clCurrentError = 0;     // error is the difference between the target and the actual position
        float output = 0;         // output is the result from the control loop calculation
        float clIntegral = 0;
        float clDerivative = 0;

        float ValveAdjust = 0;   // % amount to open/close valve
        float ValveOpen = 0;      // % valve is open
        float Pulses = 0;
        float ValveOpenTime = 2000;  // ms to fully open valve at max opening rate
        float UPM = 0;     // simulated units per minute
        float MaxRate = 120;  // max rate of system in UPM
        int ErrorRange = 8;  // % random error in flow rate, above and below target
        float PulseTime = 0;
        float PWMnet = 0;   // pwmSetting - minPWM to account for motor lag

        DateTime SimulateTimeLast;
        int SimulateInterval;
        float RandomError;

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

            if ((DateTime.Now - LastTime).TotalMilliseconds >= LOOP_TIME)
            {
                LastTime = DateTime.Now;

                if (RelaysOn)
                {
                    pwmSetting = DoPID(rateError, rateSetPoint, LOOP_TIME, MinPWMvalue, MaxPWMvalue, KP, KI, KD, DeadBand);
                }

                //motorDrive();

                SendSerial();
            }
        }

        private void SendSerial()
        {
            Sentence = "137,128,";

            // rate applied
            Temp = (byte)((int)(FlowRateFiltered * 100) >> 8);
            Sentence += Temp.ToString();
            Sentence += ",";
            Temp = (byte)(FlowRateFiltered * 100);
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

            RC.CommFromArduino(Sentence);
        }

        public void ReceiveSerial(byte[] Data)
        {
            int PGN = Data[0] << 8 | Data[1];

            if (PGN == 35000)
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

            if (PGN == 35100)
            {
                KP = (float)(Data[2] * .1);
                KI = (float)(Data[3] * .0001);
                KD = (float)(Data[4] * .1);
                DeadBand = (float)(Data[5]);
                MinPWMvalue = Data[6];
                MaxPWMvalue = Data[7];

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
                    PWMnet = pwmSetting;
                    if (PWMnet < 0)
                    {
                        PWMnet += (float)(MinPWMvalue * .5);
                        if (PWMnet > 0) PWMnet = 0;
                    }
                    else
                    {
                        PWMnet -= (float)(MinPWMvalue * .5);
                        if (PWMnet < 0) PWMnet = 0;
                    }

                    ValveAdjust = (float)((PWMnet / 255) * (float)(SimulateInterval / ValveOpenTime) * 100.0);
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
                //PulseTime = (float)((RandomError / 100.0) * PulseTime + ((100.0 - ErrorRange) / 100.0) * PulseTime);
                pulseCount = (int)(SimulateInterval / PulseTime); // milliseconds * pulses/millsecond = pulses

                // pulse duration is the total time for all pulses in the loop
                pulseDuration = (int)(PulseTime * pulseCount);
            }

        }

        float CalRateError()
        {
            //accumulated counts from this cycle
            countsThisLoop = pulseCount;
            pulseCount = 0;
            accumulatedCounts += (int)countsThisLoop;

            if (countsThisLoop == 0 | MeterCal == 0)
            {
                FlowRate = 0;
            }
            else
            {
                pulseAverage = pulseDuration / countsThisLoop;
                pulseDuration = 0;

                //what is current flowrate from meter, Units/minute
                FlowRate = (float)(pulseAverage * 0.001); // change from milliseconds/pulse to seconds/pulse

                if (FlowRate < .001) FlowRate = 0.1F;    //prevent divide by zero      
                else FlowRate = (float)(((1.0 / FlowRate) * 60) / MeterCal); //pulses/minute divided by pulses/Unit (pulses/minute * Units/pulse = Units/minute)
            }

            //Kalman filter
            Pc = P + varProcess;
            G = Pc / (Pc + varRate);
            P = (1 - G) * Pc;
            Xp = FlowRateFiltered;
            Zp = Xp;
            FlowRateFiltered = G * (FlowRate - Zp) + Xp;

            return rateSetPoint - FlowRateFiltered;
        }

        int DoPID(float clError, float clSetPoint, int clInterval, byte MinPWM, byte MaxPWM,
            float clKP, float clKi, float clKd, float deadBand)
        {

            // Calculate how far we are from the target
            clErrorLast = clCurrentError;
            clCurrentError = clError;

            // If the error is within the specified deadband, and the motor is moving slowly enough
            // Or if the motor's target is a physical limit and that limit is hit
            // (within deadband margins)
            if (Math.Abs(clCurrentError) <= ((deadBand / 100) * clSetPoint) && Math.Abs(output) <= MinPWM)
            {
                // Stop the motor
                output = 0;
                clCurrentError = 0;
            }
            else
            {
                // Else, update motor duty cycle with the newest output value
                // This equation is a simple PID control loop
                output = ((clKP * clCurrentError) + (clKi * clIntegral)) + (clKd * clDerivative);
            }

            // Prevent output value from exceeding maximum output specified by user,
            // And prevent the duty cycle from falling below the minimum velocity (excluding zero)
            // The minimum velocity exists because some DC motors with gearboxes will not be able
            // to overcome the detent torque of the gearbox at low velocities.
            if (output >= MaxPWM)
                output = MaxPWM;
            else if (output <= -MaxPWM)
                output = -MaxPWM;
            else if (output < MinPWM && output > 0)
                output = MinPWM;
            else if (output > MinPWM * (-1) && output < 0)
                output = MinPWM * (-1);
            else
                clIntegral += (clCurrentError * (float)clInterval); // The integral is only accumulated when the duty cycle isn't saturated at 100% or -100%.

            // Calculate the derivative for the next iteration
            if (clInterval > 0) clDerivative = (clCurrentError - clErrorLast) / (float)clInterval;
            else clDerivative = 0;

            return (int)output;
        }
    }
}
