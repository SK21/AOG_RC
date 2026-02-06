using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.Classes
{
    public class clsAlarm
    {
        private readonly System.Media.SoundPlayer sound;
        private DateTime? alarmStart;
        private bool IsPlaying;
        private bool SilenceAlarm;


        public clsAlarm()
        {
            sound = new System.Media.SoundPlayer(RateController.Properties.Resources.Loud_Alarm_Clock_Buzzer_Muk1984_493547174);
        }

        public bool AlarmIsOn(out bool[] ProductAlarms)
        {
            ProductAlarms = new bool[Props.MaxProducts];
            double AlarmSetPoint;
            bool AlarmIsOn = false;

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
                            AlarmIsOn = true;
                            ProductAlarms[Prd.ID] = true;
                        }
                        if (!ProductAlarms[Prd.ID])
                        {
                            // too high?
                            AlarmSetPoint = (100 + Prd.OffRateSetting) / 100.0;
                            if (Prd.SmoothRate() > (Prd.TargetRate() * AlarmSetPoint))
                            {
                                AlarmIsOn = true;
                                ProductAlarms[Prd.ID] = true;
                            }
                        }
                    }
                }
            }

            UpdateSound(AlarmIsOn);

            return AlarmIsOn;
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
