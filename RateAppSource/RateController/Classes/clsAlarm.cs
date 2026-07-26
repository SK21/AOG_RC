using System;

namespace RateController.Classes
{
    public enum AlarmKind
    { None, Rate, Pressure, Bin }

    public class clsAlarm
    {
        // rate must stay out of band this long before alarming, so normal settling
        // during headland turns does not flash the alarm; clearing is immediate
        private const double RatePersistSeconds = 5.0;

        private readonly System.Media.SoundPlayer sound;
        private DateTime? alarmStart;
        private bool cAlarmIsOn;
        private bool[] cAlarms;
        private bool[] cBinAlarms;
        private AlarmKind cLatestKind = AlarmKind.None;
        private int cLatestIndex = -1;
        private bool[] cPressureAlarms;
        private DateTime?[] cRateOutOfBandSince;
        private bool IsPlaying;
        private bool SilenceAlarm;

        public clsAlarm()
        {
            sound = new System.Media.SoundPlayer(RateController.Properties.Resources.Loud_Alarm_Clock_Buzzer_Muk1984_493547174);
            cAlarms = new bool[Props.MaxProducts];
            cBinAlarms = new bool[Props.MaxProducts];
            cPressureAlarms = new bool[Props.MaxModules];
            cRateOutOfBandSince = new DateTime?[Props.MaxProducts];
        }

        public bool AlarmIsOn
        { get { return cAlarmIsOn; } }

        public bool[] Alarms
        { get { return cAlarms; } }

        public bool BinAlarmIsOn { get; private set; }
        public bool[] BinAlarms { get { return cBinAlarms; } }

        public bool PressureAlarmIsOn { get; private set; }
        public bool[] PressureAlarms { get { return cPressureAlarms; } }

        public int ActiveCount
        {
            get
            {
                int Result = 0;
                for (int i = 0; i < Props.MaxProducts; i++)
                {
                    if (cAlarms[i]) Result++;
                    if (cBinAlarms[i]) Result++;
                }
                for (int i = 0; i < Props.MaxModules; i++)
                {
                    if (cPressureAlarms[i]) Result++;
                }
                return Result;
            }
        }

        // The alarm to announce on the alarm button: the most recently latched one,
        // falling back to any still-active alarm when the latest has cleared.
        public AlarmKind Announced(out int Index)
        {
            bool Valid = false;
            switch (cLatestKind)
            {
                case AlarmKind.Rate:
                    Valid = (cLatestIndex >= 0 && cLatestIndex < Props.MaxProducts && cAlarms[cLatestIndex]);
                    break;

                case AlarmKind.Bin:
                    Valid = (cLatestIndex >= 0 && cLatestIndex < Props.MaxProducts && cBinAlarms[cLatestIndex]);
                    break;

                case AlarmKind.Pressure:
                    Valid = (cLatestIndex >= 0 && cLatestIndex < Props.MaxModules && cPressureAlarms[cLatestIndex]);
                    break;
            }

            if (!Valid)
            {
                cLatestKind = AlarmKind.None;
                cLatestIndex = -1;
                for (int i = 0; i < Props.MaxModules && cLatestKind == AlarmKind.None; i++)
                {
                    if (cPressureAlarms[i])
                    {
                        cLatestKind = AlarmKind.Pressure;
                        cLatestIndex = i;
                    }
                }
                for (int i = 0; i < Props.MaxProducts && cLatestKind == AlarmKind.None; i++)
                {
                    if (cBinAlarms[i])
                    {
                        cLatestKind = AlarmKind.Bin;
                        cLatestIndex = i;
                    }
                }
                for (int i = 0; i < Props.MaxProducts && cLatestKind == AlarmKind.None; i++)
                {
                    if (cAlarms[i])
                    {
                        cLatestKind = AlarmKind.Rate;
                        cLatestIndex = i;
                    }
                }
            }

            Index = cLatestIndex;
            return cLatestKind;
        }

        public bool CheckAlarms()
        {
            double AlarmSetPoint;
            bool CurrentState = false;
            bool[] NewRate = new bool[Props.MaxProducts];
            bool[] NewBin = new bool[Props.MaxProducts];
            bool[] NewPressure = new bool[Props.MaxModules];

            if (Core.Sections.WorkRatePerHour() > 0)
            {
                foreach (clsProduct Prd in Core.Products.Items)
                {
                    if (Prd.Enabled && Prd.UseOffRateAlarm && Prd.AppMode != ApplicationMode.DocumentAppliedManual)
                    {
                        // too low?
                        bool OutOfBand = false;
                        AlarmSetPoint = (100 - Prd.OffRateSetting) / 100.0;
                        if (Prd.SmoothRate() < (Prd.TargetRate() * AlarmSetPoint))
                        {
                            OutOfBand = true;
                        }
                        if (!OutOfBand)
                        {
                            // too high?
                            AlarmSetPoint = (100 + Prd.OffRateSetting) / 100.0;
                            if (Prd.SmoothRate() > (Prd.TargetRate() * AlarmSetPoint))
                            {
                                OutOfBand = true;
                            }
                        }

                        if (OutOfBand)
                        {
                            if (!cRateOutOfBandSince[Prd.ID].HasValue) cRateOutOfBandSince[Prd.ID] = DateTime.Now;
                            if ((DateTime.Now - cRateOutOfBandSince[Prd.ID].Value).TotalSeconds >= RatePersistSeconds)
                            {
                                CurrentState = true;
                                NewRate[Prd.ID] = true;
                            }
                        }
                        else
                        {
                            cRateOutOfBandSince[Prd.ID] = null;
                        }
                    }
                }
            }
            else
            {
                // not applying - restart the rate persistence timers
                for (int i = 0; i < Props.MaxProducts; i++)
                {
                    cRateOutOfBandSince[i] = null;
                }
            }

            PressureAlarmIsOn = false;
            for (int i = 0; i < Props.MaxModules; i++)
            {
                double maxP = Props.GetMaxPressure(i);
                if (maxP > 0 && Core.ModulesStatus.Connected(i))
                {
                    double calibrated = Props.PressureReading(i, Core.ModulesStatus.PressureReading(i));
                    if (calibrated > maxP)
                    {
                        NewPressure[i] = true;
                        PressureAlarmIsOn = true;
                    }
                }
            }

            // bin state only matters while applying - unsuppressed it is a
            // distraction at startup, when bins are legitimately empty
            BinAlarmIsOn = false;
            if (Core.SectionControl.MasterOn)
            {
                foreach (clsProduct Prd in Core.Products.Items)
                {
                    // BinEmpty is already false when the module is not sending
                    if (Prd.Enabled && Prd.BinEmpty)
                    {
                        NewBin[Prd.ID] = true;
                        BinAlarmIsOn = true;
                    }
                }
            }

            // an alarm joining the active set becomes the announced alarm and
            // re-arms the sound, so a silenced buzzer still reports new trouble
            for (int i = 0; i < Props.MaxProducts; i++)
            {
                if (NewRate[i] && !cAlarms[i]) NewAlarm(AlarmKind.Rate, i);
                if (NewBin[i] && !cBinAlarms[i]) NewAlarm(AlarmKind.Bin, i);
            }
            for (int i = 0; i < Props.MaxModules; i++)
            {
                if (NewPressure[i] && !cPressureAlarms[i]) NewAlarm(AlarmKind.Pressure, i);
            }

            cAlarms = NewRate;
            cBinAlarms = NewBin;
            cPressureAlarms = NewPressure;

            cAlarmIsOn = CurrentState || PressureAlarmIsOn || BinAlarmIsOn;
            UpdateSound(cAlarmIsOn);

            return cAlarmIsOn;
        }

        private void NewAlarm(AlarmKind Kind, int Index)
        {
            cLatestKind = Kind;
            cLatestIndex = Index;
            SilenceAlarm = false;
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