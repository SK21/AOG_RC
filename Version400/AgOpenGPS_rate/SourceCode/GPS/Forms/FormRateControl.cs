using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AgOpenGPS.Forms
{
    public partial class FormRateControl : Form
    {
        private readonly FormGPS mf;
        private bool LastIsDay = true;
        private DateTime LastCheck;

        public FormRateControl(FormGPS CallingForm)
        {
            mf = CallingForm;
            InitializeComponent();
            UpdateStatus();
        }
        public void UpdateStatus()
        {
            lblUnits.Text = mf.RC.Units();
            SetRate.Text = mf.RC.RateSet.ToString("N1");
            CurRate.Text = mf.RC.CurrentRate();
            AreaDone.Text = mf.RC.CurrentCoverage();
            TankRemain.Text = mf.RC.CurrentTankRemaining();
            VolApplied.Text = mf.RC.CurrentApplied();
            if (mf.RC.ControllerConnected)
            {
                lbConnection.Text = "Controller Connected";
                lbConnection.BackColor = Color.LightGreen;
            }
            else
            {
                lbConnection.Text = "Controller Disconnected";
                lbConnection.BackColor = Color.Red;
            }
            DayNight();
        }

        private void FormRateControl_Load(object sender, EventArgs e)
        {
            this.Location = Properties.Settings.Default.FormRClocation;
        }

        private void FormRateControl_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                // save location and size if the state is normal
                Properties.Settings.Default.FormRClocation = this.Location;
            }
            else
            {
                // save the RestoreBounds if the form is minimized or maximized!
                Properties.Settings.Default.FormRClocation = this.RestoreBounds.Location;
            }

            // don't forget to save the settings
            Properties.Settings.Default.Save();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form frmRateSettings = new Forms.FormRateSettings(mf);
            frmRateSettings.ShowDialog();
            UpdateStatus();
        }

        private void bntOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DayNight()
        {
            if ((DateTime.Now - LastCheck).Seconds > 3)
            {
                if (Properties.Settings.Default.setDisplay_isDayMode != LastIsDay)
                {
                    LastIsDay = Properties.Settings.Default.setDisplay_isDayMode;
                    if (LastIsDay)
                    {
                        this.BackColor = mf.dayColor;
                        foreach (Control c in this.Controls)
                        {
                            {
                                c.ForeColor = Color.Black;
                            }
                        }
                    }
                    else
                    {
                        this.BackColor = mf.nightColor;
                        foreach (Control c in this.Controls)
                        {
                            {
                                c.ForeColor = Color.White;
                            }
                        }
                    }
                }
                LastCheck = DateTime.Now;
            }
        }
    }
}
