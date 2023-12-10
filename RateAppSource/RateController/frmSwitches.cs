using AgOpenGPS;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmSwitches : Form
    {
        private bool DownPressed;
        private bool MasterPressed;
        private FormStart mf;
        private bool[] SwON = new bool[9];
        private bool UpPressed;

        public frmSwitches(FormStart CallingForm)
        {
            mf = CallingForm;
            InitializeComponent();

            mf.SwitchBox.SwitchPGNreceived += SwitchBox_SwitchPGNreceived;
            this.BackColor = Properties.Settings.Default.DayColour;
        }

        private void btAuto_Click(object sender, EventArgs e)
        {
            mf.vSwitchBox.PressSwitch(SwIDs.Auto);
        }

        private void btn1_Click(object sender, EventArgs e)
        {
            mf.vSwitchBox.PressSwitch(SwIDs.sw0);
        }

        private void btn2_Click(object sender, EventArgs e)
        {
            mf.vSwitchBox.PressSwitch(SwIDs.sw1);
        }

        private void btn3_Click(object sender, EventArgs e)
        {
            mf.vSwitchBox.PressSwitch(SwIDs.sw2);
        }

        private void btn4_Click(object sender, EventArgs e)
        {
            mf.vSwitchBox.PressSwitch(SwIDs.sw3);
        }

        private void btnDown_MouseDown(object sender, MouseEventArgs e)
        {
            mf.vSwitchBox.PressSwitch(SwIDs.RateDown);
            DownPressed = true;
            tmrRelease.Enabled = true;
        }

        private void btnDown_MouseUp(object sender, MouseEventArgs e)
        {
            DownPressed = false;
        }

        private void btnMaster_MouseDown(object sender, MouseEventArgs e)
        {
            if (mf.SectionControl.MasterOn())
            {
                mf.vSwitchBox.PressSwitch(SwIDs.MasterOff);
                MasterPressed = true;
                tmrRelease.Enabled = true;
            }
            else
            {
                mf.vSwitchBox.PressSwitch(SwIDs.MasterOn);
                MasterPressed = true;
                tmrRelease.Enabled = true;
            }
        }

        private void btnMaster_MouseUp(object sender, MouseEventArgs e)
        {
            MasterPressed = false;
        }

        private void btnUp_MouseDown(object sender, MouseEventArgs e)
        {
            mf.vSwitchBox.PressSwitch(SwIDs.RateUp);
            UpPressed = true;
            tmrRelease.Enabled = true;
        }

        private void btnUp_MouseUp(object sender, MouseEventArgs e)
        {
            UpPressed = false;
        }

        private void frmSimulation_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                mf.Tls.SaveFormData(this);
            }
            timer1.Enabled = false;
            mf.vSwitchBox.SwitchScreenOn = false;
        }

        private void frmSimulation_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            SwON = mf.SwitchBox.Switches;
            UpdateSwitches();
            timer1.Enabled = true;
            mf.vSwitchBox.SwitchScreenOn = true;
        }

        private void SwitchBox_SwitchPGNreceived(object sender, PGN32618.SwitchPGNargs e)
        {
            SwON = e.Switches;
            UpdateSwitches();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateSwitches();
        }

        private void tmrRelease_Tick(object sender, EventArgs e)
        {
            if (!UpPressed && !DownPressed && !MasterPressed)
            {
                mf.vSwitchBox.ReleaseSwitch();
                tmrRelease.Enabled = false;
            }
        }

        private void UpdateSwitches()
        {
            if (SwON[0])
            {
                btAuto.BackColor = Color.LightGreen;
            }
            else
            {
                btAuto.BackColor = Color.Red;
            }

            if (SwON[1])
            {
                btnMaster.BackColor = Color.LightGreen;
            }

            if (SwON[2])
            {
                btnMaster.BackColor = Color.Red;
            }

            if (SwON[3])
            {
                btnUp.BackColor = Color.LightGreen;
            }
            else
            {
                btnUp.BackColor = Properties.Settings.Default.DayColour;
            }

            if (SwON[4])
            {
                btnDown.BackColor = Color.LightGreen;
            }
            else
            {
                btnDown.BackColor = Properties.Settings.Default.DayColour;
            }

            if (SwON[5])
            {
                btn1.BackColor = Color.LightGreen;
            }
            else
            {
                btn1.BackColor = Color.Red;
            }

            if (SwON[6])
            {
                btn2.BackColor = Color.LightGreen;
            }
            else
            {
                btn2.BackColor = Color.Red;
            }

            if (SwON[7])
            {
                btn3.BackColor = Color.LightGreen;
            }
            else
            {
                btn3.BackColor = Color.Red;
            }

            if (SwON[8])
            {
                btn4.BackColor = Color.LightGreen;
            }
            else
            {
                btn4.BackColor = Color.Red;
            }
        }
    }
}