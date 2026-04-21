using System;

namespace RateController.Classes
{
    public class clsRateAdjustController
    {
        private const int AdjustDelay = 250;
        private const double AutoStepMultiplier = 0.025;
        private const int ManualMotorStepSize = 10;
        private const byte MaxSteps = 10;
        private const int StepDelay = 2000;

        private DateTime AdjustTime;
        private bool LastState;
        private bool Pressed;
        private double RateDir;
        private int RateStep;
        private bool StateChanged;
        private DateTime StepTime;
        private int ValveRampPerTick = 15;

        public clsRateAdjustController()
        {
            Core.SwitchBox.SwitchPGNreceived += SwitchBox_SwitchPGNreceived;
        }

        public void Update()
        {
            try
            {
                if (Core.SwitchBox.RateUp)
                {
                    Pressed = true;
                    RateDir = 1;
                }
                else if (Core.SwitchBox.RateDown)
                {
                    Pressed = true;
                    RateDir = -1;
                }
                else
                {
                    Pressed = false;
                    RateStep = 0;
                }

                StateChanged = (LastState != Pressed);
                LastState = Pressed;

                if (Pressed)
                {
                    // set step
                    if ((DateTime.Now - StepTime).TotalMilliseconds > StepDelay)
                    {
                        StepTime = DateTime.Now;
                        RateStep++;
                        if (RateStep > MaxSteps) RateStep = MaxSteps;
                    }

                    // adjust rate
                    if ((DateTime.Now - AdjustTime).TotalMilliseconds > AdjustDelay)
                    {
                        AdjustTime = DateTime.Now;
                        clsProduct Prd = Core.Products.Item(Props.CurrentProduct);

                        if (!Core.SwitchBox.MasterOn || !Core.SwitchBox.AutoRateOn)
                        {
                            // adjust PWM
                            bool IsValve = Prd.ControlType == ControlTypeEnum.Valve || Prd.ControlType == ControlTypeEnum.ComboClose || Prd.ControlType == ControlTypeEnum.ComboCloseTimed;
                            if (IsValve)
                            {
                                int minPWM = (int)(Prd.MinPWMadjust / 100.0 * 255.0);
                                int absPWM = Math.Abs(Prd.ManualPWM);
                                if (absPWM < minPWM)
                                {
                                    // first press: jump to minimum
                                    absPWM = minPWM;
                                }
                                else
                                {
                                    // hold: ramp up speed
                                    absPWM = Math.Min(255, absPWM + ValveRampPerTick);
                                }
                                Prd.ManualPWM = (int)(absPWM * RateDir);
                            }
                            else
                            {
                                // adjust motor
                                Prd.ManualPWM += (int)(RateDir * RateStep * ManualMotorStepSize);
                            }
                        }
                        else
                        {
                            // adjust target/acre - auto mode
                            double CurrentRate = Prd.RateSet;
                            if (CurrentRate == 0) CurrentRate = 1;

                            if (RateDir == 1)
                            {
                                CurrentRate = CurrentRate * (1 + (AutoStepMultiplier * RateStep));
                            }
                            else
                            {
                                CurrentRate = CurrentRate / (1 + (AutoStepMultiplier * RateStep));
                            }
                            Prd.RateSet = CurrentRate;
                        }
                    }
                }
                else
                {
                    if (StateChanged)
                    {
                        // stop manual adjusting flow valve when rate adjust buttons are not pushed
                        clsProduct Prd = Core.Products.Item(Props.CurrentProduct);

                        if (Prd.ControlType == ControlTypeEnum.Valve || Prd.ControlType == ControlTypeEnum.ComboClose || Prd.ControlType == ControlTypeEnum.ComboCloseTimed)
                        {
                            Prd.ManualPWM = 0;
                        }
                        AdjustTime = DateTime.MinValue;
                        StepTime = DateTime.MinValue;
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsRateAdjustController/ReadRateSwitches: " + ex.Message);
            }
        }

        private void SwitchBox_SwitchPGNreceived(object sender, EventArgs e)
        {
            Update();
        }
    }
}