using System.Drawing;
using System.Windows.Forms;

namespace RateController
{
    public class clsAlarm
    {
        private double AlarmDelay;
        private bool cShowAlarm;
        private bool cSilenceAlarm;
        private FormStart mf;
        private Button cAlarmButton;
        private bool AlarmColour;
        private bool cRateAlarm;
        private bool cPressureAlarm;
        private string cMessage;

        private System.Media.SoundPlayer Sounds;
        private System.IO.Stream Str;

        public clsAlarm(FormStart CallingFrom, Button AlarmButton)
        {
            mf = CallingFrom;
            cAlarmButton = AlarmButton;
            Str = Properties.Resources.Loud_Alarm_Clock_Buzzer_Muk1984_493547174;
            Sounds = new System.Media.SoundPlayer(Str);
        }

        public void Silence() { cSilenceAlarm = true; }

        public void CheckAlarms()
        {
            cRateAlarm = mf.Products.AlarmOn();
            cPressureAlarm = mf.PressureObjects.AlarmOn();

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
    }
}