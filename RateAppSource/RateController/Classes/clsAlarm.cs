using System.Drawing;
using System.Windows.Forms;

namespace RateController
{
    public class clsAlarm
    {
        private bool AlarmColour;
        private double AlarmDelay;
        private Button cAlarmButton;
        private bool cShowAlarm;
        private bool cSilenceAlarm;
        private FormStart mf;
        private System.Media.SoundPlayer Sounds;

        public clsAlarm(FormStart CallingFrom, Button AlarmButton)
        {
            mf = CallingFrom;
            cAlarmButton = AlarmButton;
            System.IO.Stream Str = Properties.Resources.Loud_Alarm_Clock_Buzzer_Muk1984_493547174;
            Sounds = new System.Media.SoundPlayer(Str);
        }

        public void CheckAlarms()
        {
            bool cRateAlarm = mf.Products.AlarmOn();
            bool cPressureAlarm = mf.PressureObjects.AlarmOn();
            string cMessage;

            if (cRateAlarm || cPressureAlarm)
            {
                cMessage = "Alarm";
                if (cPressureAlarm) cMessage = "Pressure  " + cMessage;
                if (cRateAlarm) cMessage = "Rate  " + cMessage;
                cAlarmButton.Text = cMessage;

                if (cSilenceAlarm)
                {
                    Sounds.Stop();
                }
                else
                {
                    AlarmDelay++;
                    if (AlarmDelay > 5)
                    {
                        Sounds.Play();
                        cShowAlarm = true;
                    }
                }

                cAlarmButton.Visible = cShowAlarm;
                cAlarmButton.BringToFront();

                AlarmColour = !AlarmColour;
                if (AlarmColour)
                {
                    cAlarmButton.BackColor = Color.Red;
                }
                else
                {
                    cAlarmButton.BackColor = Color.Yellow;
                }
            }
            else
            {
                AlarmDelay = 0;
                Sounds.Stop();
                cSilenceAlarm = false;
                cShowAlarm = false;
                cAlarmButton.Visible = false;
            }
        }

        public void Silence()
        { cSilenceAlarm = true; }
    }
}