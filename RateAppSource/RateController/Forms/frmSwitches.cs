using RateController.Classes;
using RateController.Language;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmSwitches : Form
    {
        private int mouseX = 0;
        private int mouseY = 0;
        private int windowLeft = 0;
        private int windowTop = 0;

        public frmSwitches()
        {
            InitializeComponent();
        }

        public void SetDescriptions()
        {
            btn1.Text = "1";
            btn2.Text = "2";
            btn3.Text = "3";
            btn4.Text = "4";
            btn5.Text = "5";
            btn6.Text = "6";
            btn7.Text = "7";
            btn8.Text = "8";
        }

        private void btAuto_Click(object sender, EventArgs e)
        {
            Core.vSwitchBox.PressSwitch(SwIDs.AutoRate);
            Core.vSwitchBox.PressSwitch(SwIDs.AutoSection);
        }

        private void btn1_Click(object sender, EventArgs e)
        {
            Core.vSwitchBox.PressSwitch(SwIDs.sw0);
        }

        private void btn2_Click(object sender, EventArgs e)
        {
            Core.vSwitchBox.PressSwitch(SwIDs.sw1);
        }

        private void btn3_Click(object sender, EventArgs e)
        {
            Core.vSwitchBox.PressSwitch(SwIDs.sw2);
        }

        private void btn4_Click(object sender, EventArgs e)
        {
            Core.vSwitchBox.PressSwitch(SwIDs.sw3);
        }

        private void btn5_Click(object sender, EventArgs e)
        {
            Core.vSwitchBox.PressSwitch(SwIDs.sw4);
        }

        private void btn6_Click(object sender, EventArgs e)
        {
            Core.vSwitchBox.PressSwitch(SwIDs.sw5);
        }

        private void btn7_Click(object sender, EventArgs e)
        {
            Core.vSwitchBox.PressSwitch(SwIDs.sw6);
        }

        private void btn8_Click(object sender, EventArgs e)
        {
            Core.vSwitchBox.PressSwitch(SwIDs.sw7);
        }

        private void btnAutoRate_Click(object sender, EventArgs e)
        {
            Core.vSwitchBox.PressSwitch(SwIDs.AutoRate);
        }

        private void btnAutoSection_Click(object sender, EventArgs e)
        {
            Core.vSwitchBox.PressSwitch(SwIDs.AutoSection);
        }

        private void btnDown_MouseDown(object sender, MouseEventArgs e)
        {
            Core.vSwitchBox.RateDownPressed();
        }

        private void btnDown_MouseUp(object sender, MouseEventArgs e)
        {
            Core.vSwitchBox.RateDownReleased();
        }

        private void btnMaster_Click(object sender, EventArgs e)
        {
            Core.vSwitchBox.MasterPressed();
        }

        private void btnMaster_MouseUp(object sender, MouseEventArgs e)
        {
            Core.vSwitchBox.MasterReleased();
        }

        private void btnUp_MouseDown(object sender, MouseEventArgs e)
        {
            Core.vSwitchBox.RateUpPressed();
        }

        private void btnUp_MouseUp(object sender, MouseEventArgs e)
        {
            Core.vSwitchBox.RateUpReleased();
        }

        private void Core_ProfileChanged(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void frmSwitches_Closed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
            timer1.Enabled = false;
            Core.vSwitchBox.UsefrmSwitches = false;
            Core.SwitchBox.SwitchPGNreceived -= SwitchBox_SwitchPGNreceived;
            Core.ProfileChanged -= Core_ProfileChanged;
        }

        private void frmSwitches_Load(object sender, EventArgs e)
        {
            Core.SwitchBox.SwitchPGNreceived += SwitchBox_SwitchPGNreceived;
            Core.ProfileChanged += Core_ProfileChanged;
            this.BackColor = Properties.Settings.Default.MainBackColour;

            Props.LoadFormLocation(this);
            timer1.Enabled = true;
            Core.vSwitchBox.UsefrmSwitches = true;
            Core.vSwitchBox.PressSwitch(SwIDs.MasterOff);
            SetDescriptions();
            LoadSettings();
        }

        private void LoadSettings()
        {
            UpdateForm();
            SetLanguage();
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

        private void SetLanguage()
        {
            btnAutoRate.Text = Lang.lgAutoRate;
            btnAutoSection.Text = Lang.lgAutoSection;
            btAuto.Text = Lang.lgAuto;
        }

        private void SwitchBox_SwitchPGNreceived(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void UpdateForm()
        {
            if (Core.SwitchBox.AutoRateOn || Core.SwitchBox.AutoSectionOn)
            {
                btAuto.BackColor = Color.LightGreen;
            }
            else
            {
                btAuto.BackColor = Color.Red;
            }

            if (Core.SwitchBox.MasterOn)
            {
                btnMaster.BackColor = Color.LightGreen;
            }
            else
            {
                btnMaster.BackColor = Color.Red;
            }

            if (Core.SwitchBox.SwitchIsOn(SwIDs.RateUp))
            {
                btnUp.BackColor = Color.LightGreen;
            }
            else
            {
                btnUp.BackColor = this.TransparencyKey;
            }

            if (Core.SwitchBox.SwitchIsOn(SwIDs.RateDown))
            {
                btnDown.BackColor = Color.LightGreen;
            }
            else
            {
                btnDown.BackColor = this.TransparencyKey;
            }

            if (Core.SwitchBox.SwitchIsOn(SwIDs.sw0))
            {
                btn1.BackColor = Color.LightGreen;
            }
            else
            {
                btn1.BackColor = Color.Red;
            }

            if (Core.SwitchBox.SwitchIsOn(SwIDs.sw1))
            {
                btn2.BackColor = Color.LightGreen;
            }
            else
            {
                btn2.BackColor = Color.Red;
            }

            if (Core.SwitchBox.SwitchIsOn(SwIDs.sw2))
            {
                btn3.BackColor = Color.LightGreen;
            }
            else
            {
                btn3.BackColor = Color.Red;
            }

            if (Core.SwitchBox.SwitchIsOn(SwIDs.sw3))
            {
                btn4.BackColor = Color.LightGreen;
            }
            else
            {
                btn4.BackColor = Color.Red;
            }

            if (Core.SwitchBox.SwitchIsOn(SwIDs.sw4))
            {
                btn5.BackColor = Color.LightGreen;
            }
            else
            {
                btn5.BackColor = Color.Red;
            }

            if (Core.SwitchBox.SwitchIsOn(SwIDs.sw5))
            {
                btn6.BackColor = Color.LightGreen;
            }
            else
            {
                btn6.BackColor = Color.Red;
            }

            if (Core.SwitchBox.SwitchIsOn(SwIDs.sw6))
            {
                btn7.BackColor = Color.LightGreen;
            }
            else
            {
                btn7.BackColor = Color.Red;
            }

            if (Core.SwitchBox.SwitchIsOn(SwIDs.sw7))
            {
                btn8.BackColor = Color.LightGreen;
            }
            else
            {
                btn8.BackColor = Color.Red;
            }

            if (Core.SwitchBox.AutoSectionOn)
            {
                btnAutoSection.BackColor = Color.LightGreen;
            }
            else
            {
                btnAutoSection.BackColor = Color.Red;
            }

            if (Core.SwitchBox.AutoRateOn)
            {
                btnAutoRate.BackColor = Color.LightGreen;
            }
            else
            {
                btnAutoRate.BackColor = Color.Red;
            }

            if (Props.UseDualAuto)
            {
                btAuto.Visible = false;
                btnMaster.Width = 142;
                btnAutoRate.Visible = true;
                btnAutoSection.Visible = true;
                this.Height = 272;
            }
            else
            {
                btAuto.Visible = true;
                btnMaster.Width = 64;
                btnAutoRate.Visible = false;
                btnAutoSection.Visible = false;
                this.Height = 221;
            }
        }
    }
}