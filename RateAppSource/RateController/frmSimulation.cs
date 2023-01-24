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
            UpdateProduct();

            mf.ProductChanged += Mf_ProductChanged;
            mf.SwitchBox.SwitchPGNreceived += SwitchBox_SwitchPGNreceived;
            this.BackColor = Properties.Settings.Default.DayColour;
        }

        public void UpdateProduct()
        {
            CurrentSim = mf.Products.Item(mf.CurrentProduct()).SimulationType;
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
            tbSpeed.Text = mf.Products.Item(mf.CurrentProduct()).SimSpeed.ToString("N1");
        }

        private void frmSimulation_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                mf.Tls.SaveFormData(this);
            }
            mf.SimFormLoaded = false;
        }

        private void frmSimulation_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            UpdateForm();
            UpdateSwitches();
        }

        private void grpSections_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            mf.Tls.DrawGroupBox(box, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }

        private void Mf_ProductChanged(object sender, EventArgs e)
        {
            UpdateProduct();
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
                    mf.Products.Item(mf.CurrentProduct()).SimSpeed = form.ReturnValue;
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

            mf.Products.Item(mf.CurrentProduct()).SimulationType = CurrentSim;
            if (mf.UseInches)
            {
                lbMPH.Text = Lang.lgMPH;
            }
            else
            {
                lbMPH.Text = Lang.lgKPH;
            }
            tbSpeed.Text = mf.Products.Item(mf.CurrentProduct()).SimSpeed.ToString("N1");
        }

        private void UpdateSwitches()
        {
            if (SwON[0])
            {
                swAuto.BackColor = Color.LightGreen;
            }
            else
            {
                swAuto.BackColor = Color.Red;
            }

            if (SwON[1])
            {
                swMasterOn.BackColor = Color.LightGreen;
            }
            else
            {
                swMasterOn.BackColor = Properties.Settings.Default.DayColour;
            }

            if (SwON[2])
            {
                swMasterOff.BackColor = Color.LightGreen;
            }
            else
            {
                swMasterOff.BackColor = Properties.Settings.Default.DayColour;
            }

            if (SwON[3])
            {
                swUp.BackColor = Color.LightGreen;
            }
            else
            {
                swUp.BackColor = Properties.Settings.Default.DayColour;
            }

            if (SwON[4])
            {
                swDown.BackColor = Color.LightGreen;
            }
            else
            {
                swDown.BackColor = Properties.Settings.Default.DayColour;
            }

            if (SwON[5])
            {
                swOne.BackColor = Color.LightGreen;
            }
            else
            {
                swOne.BackColor = Color.Red;
            }

            if (SwON[6])
            {
                swTwo.BackColor = Color.LightGreen;
            }
            else
            {
                swTwo.BackColor = Color.Red;
            }

            if (SwON[7])
            {
                swThree.BackColor = Color.LightGreen;
            }
            else
            {
                swThree.BackColor = Color.Red;
            }

            if (SwON[8])
            {
                swFour.BackColor = Color.LightGreen;
            }
            else
            {
                swFour.BackColor = Color.Red;
            }
        }
    }
}