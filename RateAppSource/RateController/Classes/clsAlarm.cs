using System;

namespace RateController.Classes
{
    public class clsAlarm
    {
        private readonly System.Media.SoundPlayer sound;
        private DateTime? alarmStart;
        private bool cAlarmIsOn;
        private bool[] cAlarms;
        private bool[] cPressureAlarms;
        private bool IsPlaying;
        private bool SilenceAlarm;

        public clsAlarm()
        {
            sound = new System.Media.SoundPlayer(RateController.Properties.Resources.Loud_Alarm_Clock_Buzzer_Muk1984_493547174);
            cAlarms = new bool[Props.MaxProducts];
            cPressureAlarms = new bool[Props.MaxModules];
        }

        public bool AlarmIsOn
        { get { return cAlarmIsOn; } }

        public bool[] Alarms
        { get { return cAlarms; } }

        public bool PressureAlarmIsOn { get; private set; }
        public bool[] PressureAlarms { get { return cPressureAlarms; } }

        public bool CheckAlarms()
        {
            double AlarmSetPoint;
            bool CurrentState = false;
            cAlarms = new bool[Props.MaxProducts];

            if (Core.Sections.WorkRatePerHour() > 0)
            {
                foreach (clsProduct Prd in Core.Products.Items)
                {
                    if (Prd.Enabled && Prd.UseOffRateAlarm)
                    {
                        // too low?
                        AlarmSetPoint = (100 - Prd.OffRateSetting) / 100.0;
                        if (Prd.SmoothRate() < (Prd.TargetRate() * AlarmSetPoint))
                        {
                            CurrentState = true;
                            cAlarms[Prd.ID] = true;
                        }
                        if (!cAlarms[Prd.ID])
                        {
                            // too high?
                            AlarmSetPoint = (100 + Prd.OffRateSetting) / 100.0;
                            if (Prd.SmoothRate() > (Prd.TargetRate() * AlarmSetPoint))
                            {
                                CurrentState = true;
                                cAlarms[Prd.ID] = true;
                            }
                        }
                    }
                }
            }
            cPressureAlarms = new bool[Props.MaxModules];
            PressureAlarmIsOn = false;

            for (int i = 0; i < Props.MaxModules; i++)
            {
                double maxP = Props.GetMaxPressure(i);
                if (maxP > 0 && Core.ModulesStatus.Connected(i))
                {
                    double calibrated = Props.PressureReading(i, Core.ModulesStatus.PressureReading(i));
                    if (calibrated > maxP)
                    {
                        cPressureAlarms[i] = true;
                        PressureAlarmIsOn = true;
                    }
                }
            }

            cAlarmIsOn = CurrentState || PressureAlarmIsOn;
            UpdateSound(cAlarmIsOn);

            return cAlarmIsOn;
        }

        public void Silence()
        {
            SilenceAlarm = true;
            sound.Stop();
            IsPlaying = false;
        }

        private void UpdateSound(bool AlarmIsOn)
        {
            if (AlarmIsOn)
            {
                if (SilenceAlarm)
                {
                    sound.Stop();
                    IsPlaying = false;
                }
                else
                {
                    if (!alarmStart.HasValue) alarmStart = DateTime.Now;

                    if ((DateTime.Now - alarmStart.Value).TotalSeconds > 5)
                    {
                        if (!IsPlaying)
                        {
                            sound.PlayLooping();
                            IsPlaying = true;
                        }
                    }
                }
            }
            else
            {
                alarmStart = null;
                sound.Stop();
                IsPlaying = false;
                SilenceAlarm = false;
            }
        }
    }
}