using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmSwitches : Form
    {
        private bool DownPressed;
        private bool IsTransparent;
        private bool MasterPressed;
        private FormStart mf;
        private int mouseX = 0;
        private int mouseY = 0;
        private bool[] SwON = new bool[23];
        private int TransLeftOffset = 6;
        private int TransTopOffset = 30;
        private bool UpPressed;
        private int windowLeft = 0;
        private int windowTop = 0;

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
            if (btnMaster.BackColor == Color.LightGreen)
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
            if (mf.UseTransparent)
            {
                // move the window back to the default location
                this.Top += -TransTopOffset;
                this.Left += -TransLeftOffset;
            }
                mf.Tls.SaveFormData(this);
            timer1.Enabled = false;
            mf.vSwitchBox.SwitchScreenOn = false;
        }

        private void frmSimulation_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            SwON = mf.SwitchBox.Switches;
            UpdateForm();
            timer1.Enabled = true;
            mf.vSwitchBox.SwitchScreenOn = true;
            mf.vSwitchBox.PressSwitch(SwIDs.MasterOff);
            tmrRelease.Enabled = true;
            UpdateForm();
        }

        private void mouseMove_MouseDown(object sender, MouseEventArgs e)
        {
            // Log the current window location and the mouse location.
            if (e.Button == MouseButtons.Right)
            {
                windowTop = this.Top;
                windowLeft = this.Left;
                mouseX = e.X;
                mouseY = e.Y;
            }
        }

        private void mouseMove_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                windowTop = this.Top;
                windowLeft = this.Left;

                Point pos = new Point(0, 0);

                pos.X = windowLeft + e.X - mouseX;
                pos.Y = windowTop + e.Y - mouseY;
                this.Location = pos;
            }
        }

        private void SetTransparent()
        {
            if (mf.UseTransparent)
            {
                this.TransparencyKey = (Properties.Settings.Default.IsDay) ? Properties.Settings.Default.DayColour : Properties.Settings.Default.NightColour;
                this.ControlBox = false;
                this.FormBorderStyle = FormBorderStyle.None;
                this.Top += TransTopOffset;
                this.Left += TransLeftOffset;
                IsTransparent = true;
            }
            else
            {
                this.Text = "Switches";
                this.TransparencyKey = Color.Empty;
                this.ControlBox = true;
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.Top += -TransTopOffset;
                this.Left += -TransLeftOffset;
                IsTransparent = false;
            }
        }

        private void SwitchBox_SwitchPGNreceived(object sender, PGN32618.SwitchPGNargs e)
        {
            SwON = e.Switches;
            UpdateForm();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void tmrRelease_Tick(object sender, EventArgs e)
        {
            if (!UpPressed && !DownPressed && !MasterPressed)
            {
                mf.vSwitchBox.ReleaseSwitch();
                tmrRelease.Enabled = false;
            }
        }

        private void UpdateForm()
        {
            if (mf.UseTransparent != IsTransparent) SetTransparent();

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
                btnUp.BackColor = this.TransparencyKey;
            }

            if (SwON[4])
            {
                btnDown.BackColor = Color.LightGreen;
            }
            else
            {
                btnDown.BackColor = this.TransparencyKey;
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

            if (SwON[21])
            {
                btnAutoSection.BackColor = Color.LightGreen;
            }
            else
            {
                btnAutoSection.BackColor = Color.Red;
            }

            if (SwON[22])
            {
                btnAutoRate.BackColor = Color.LightGreen;
            }
            else
            {
                btnAutoRate.BackColor = Color.Red;
            }

            if (mf.UseDualAuto)
            {
                btAuto.Visible = false;
                btnMaster.Width = 142;
                btnAutoRate.Visible = true;
                btnAutoSection.Visible = true;
                this.Height = 212;
                // turn off auto button
                if (SwON[0]) mf.vSwitchBox.PressSwitch(SwIDs.Auto);
            }
            else
            {
                btAuto.Visible = true;
                btnMaster.Width = 64;
                btnAutoRate.Visible = false;
                btnAutoSection.Visible = false;
                this.Height = 161;
                // turn off auto rate, auto section
                if (SwON[21]) mf.vSwitchBox.PressSwitch(SwIDs.AutoSection);
                if (SwON[22]) mf.vSwitchBox.PressSwitch(SwIDs.AutoRate);
            }
        }

        private void btnAutoSection_Click(object sender, EventArgs e)
        {
            mf.vSwitchBox.PressSwitch(SwIDs.AutoSection);
        }

        private void btnAutoRate_Click(object sender, EventArgs e)
        {
            mf.vSwitchBox.PressSwitch(SwIDs.AutoRate);
        }
    }
}