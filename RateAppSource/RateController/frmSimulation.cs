using AgOpenGPS;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmSimulation : Form
    {
        private SimType CurrentSim;
        private FormStart mf;
        private bool[] SwON = new bool[9];

        public frmSimulation(FormStart CallingForm)
        {
            mf = CallingForm;
            InitializeComponent();
            UpdateSim();

            mf.ProductChanged += Mf_ProductChanged;
            mf.SwitchBox.SwitchPGNreceived += SwitchBox_SwitchPGNreceived;
            this.BackColor = Properties.Settings.Default.DayColour;
        }

        public void UpdateSim()
        {
            CurrentSim = mf.SimMode;
            switch (CurrentSim)
            {
                case SimType.VirtualNano:
                    rbRate.Checked = true;
                    break;

                case SimType.Speed:
                    rbSpeed.Checked = true;
                    break;

                default:
                    rbOff.Checked = true;
                    break;
            }

            if (mf.UseInches)
            {
                lbMPH.Text = Lang.lgMPH;
            }
            else
            {
                lbMPH.Text = Lang.lgKPH;
            }
            tbSpeed.Text = mf.SimSpeed.ToString("N1");
            tbPWM.Text = mf.Products.Item(mf.CurrentProduct()).CalPWM.ToString("N0");
        }

        private void bntOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmSimulation_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                mf.Tls.SaveFormData(this);
            }
            timer1.Enabled = false;
        }

        private void frmSimulation_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            UpdateForm();
            SwON = mf.SwitchBox.Switches;
            UpdateSwitches();
            timer1.Enabled = true;
        }

        private void grpSections_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            mf.Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }

        private void Mf_ProductChanged(object sender, EventArgs e)
        {
            UpdateSim();
        }

        private void rbOff_Click(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void swAuto_Click(object sender, EventArgs e)
        {
            mf.SwitchBox.PressSwitch(SwIDs.Auto);
        }

        private void swDown_Click(object sender, EventArgs e)
        {
            mf.SwitchBox.PressSwitch(SwIDs.RateDown);
        }

        private void swFour_Click(object sender, EventArgs e)
        {
            mf.SwitchBox.PressSwitch(SwIDs.sw3);
        }

        private void SwitchBox_SwitchPGNreceived(object sender, PGN32618.SwitchPGNargs e)
        {
            SwON = e.Switches;
            UpdateSwitches();
        }

        private void swMasterOff_Click(object sender, EventArgs e)
        {
            mf.SwitchBox.PressSwitch(SwIDs.MasterOff);
        }

        private void swMasterOn_Click(object sender, EventArgs e)
        {
            mf.SwitchBox.PressSwitch(SwIDs.MasterOn);
        }

        private void swOne_Click(object sender, EventArgs e)
        {
            mf.SwitchBox.PressSwitch(SwIDs.sw0);
        }

        private void swThree_Click(object sender, EventArgs e)
        {
            mf.SwitchBox.PressSwitch(SwIDs.sw2);
        }

        private void swTwo_Click(object sender, EventArgs e)
        {
            mf.SwitchBox.PressSwitch(SwIDs.sw1);
        }

        private void swUp_Click(object sender, EventArgs e)
        {
            mf.SwitchBox.PressSwitch(SwIDs.RateUp);
        }

        private void tbSpeed_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbSpeed.Text, out tempD);
            using (var form = new FormNumeric(1, 20, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbSpeed.Text = form.ReturnValue.ToString("N1");
                    mf.SimSpeed = form.ReturnValue;
                }
            }
            this.ActiveControl = lbMPH;
        }

        private void UpdateForm()
        {
            if (rbRate.Checked)
            {
                if (CurrentSim != SimType.VirtualNano)
                {
                    CurrentSim = SimType.VirtualNano;
                }
            }
            else if (rbSpeed.Checked)
            {
                if (CurrentSim != SimType.Speed)
                {
                    CurrentSim = SimType.Speed;
                }
            }
            else
            {
                // default to off
                if (CurrentSim != SimType.None)
                {
                    CurrentSim = SimType.None;
                }
            }

            mf.SimMode = CurrentSim;
            if (mf.UseInches)
            {
                lbMPH.Text = Lang.lgMPH;
            }
            else
            {
                lbMPH.Text = Lang.lgKPH;
            }
            tbSpeed.Text = mf.SimSpeed.ToString("N1");
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
                ckMaster.BackColor = Color.LightGreen;
            }

            if (SwON[2])
            {
                ckMaster.BackColor = Color.Red;
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


        private void ckMaster_CheckedChanged(object sender, EventArgs e)
        {
            if(ckMaster.Checked)
            {
                mf.SwitchBox.PressSwitch(SwIDs.MasterOn);
            }
            else
            {
                mf.SwitchBox.PressSwitch(SwIDs.MasterOff);
            }
        }

        private void btn1_Click(object sender, EventArgs e)
        {
            mf.SwitchBox.PressSwitch(SwIDs.sw0);
        }

        private void btn2_Click(object sender, EventArgs e)
        {
            mf.SwitchBox.PressSwitch(SwIDs.sw1);
        }

        private void btn3_Click(object sender, EventArgs e)
        {
            mf.SwitchBox.PressSwitch(SwIDs.sw2);
        }

        private void btn4_Click(object sender, EventArgs e)
        {
            mf.SwitchBox.PressSwitch(SwIDs.sw3);
        }

        private void btAuto_Click(object sender, EventArgs e)
        {
            mf.SwitchBox.PressSwitch(SwIDs.Auto);
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            mf.SwitchBox.PressSwitch(SwIDs.RateUp);
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            mf.SwitchBox.PressSwitch(SwIDs.RateDown);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateSim();
        }

        private void tbPWM_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbSpeed.Text, out tempD);
            using (var form = new FormNumeric(0,255, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbSpeed.Text = form.ReturnValue.ToString("N0");
                    mf.Products.Item(mf.CurrentProduct()).CalPWM = (byte)form.ReturnValue;
                }
            }
            this.ActiveControl = lbMPH;
        }
    }
}