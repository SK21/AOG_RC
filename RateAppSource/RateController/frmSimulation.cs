using AgOpenGPS;
using System;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmSimulation : Form
    {
        private SimType CurrentSim;
        private FormStart mf;

        public frmSimulation(FormStart CallingForm)
        {
            mf = CallingForm;
            InitializeComponent();
            UpdateProduct();

            mf.ProductChanged += Mf_ProductChanged;
        }

        public void UpdateProduct()
        {
            CurrentSim = mf.Products.Item(mf.CurrentProduct()).SimulationType;
            switch (CurrentSim)
            {
                case SimType.VirtualNano:
                    rbRate.Checked = true;
                    if (mf.UseInches)
                    {
                        lbMPH.Text = Lang.lgMPH;
                    }
                    else
                    {
                        lbMPH.Text = Lang.lgKPH;
                    }
                    tbSpeed.Text = mf.Products.Item(mf.CurrentProduct()).SimSpeed.ToString("N1");
                    break;

                case SimType.Speed:
                    rbSpeed.Checked = true;
                    if (mf.UseInches)
                    {
                        lbMPH.Text = Lang.lgMPH;
                    }
                    else
                    {
                        lbMPH.Text = Lang.lgKPH;
                    }
                    tbSpeed.Text = mf.Products.Item(mf.CurrentProduct()).SimSpeed.ToString("N1"); 
                    break;

                case SimType.PWM:
                    rbPWM.Checked = true;
                    lbMPH.Text = "PWM";
                    tbSpeed.Text = mf.Products.Item(mf.CurrentProduct()).SimPWM.ToString("N0"); 
                    break;

                default:
                    rbOff.Checked = true;
                    if (mf.UseInches)
                    {
                        lbMPH.Text = Lang.lgMPH;
                    }
                    else
                    {
                        lbMPH.Text = Lang.lgKPH;
                    }
                    tbSpeed.Text = mf.Products.Item(mf.CurrentProduct()).SimSpeed.ToString("N1");
                    break;
            }
        }

        private void frmSimulation_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                mf.Tls.SaveFormData(this);
            }
        }

        private void frmSimulation_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            UpdateForm();
        }

        private void Mf_ProductChanged(object sender, EventArgs e)
        {
            UpdateProduct();
        }

        private void rbOff_Click(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void tbSpeed_Enter(object sender, EventArgs e)
        {
            switch (CurrentSim)
            {
                case SimType.PWM:
                    byte temp;
                    byte.TryParse(tbSpeed.Text, out temp);
                    using (var form = new FormNumeric(1, 255, temp))
                    {
                        var result = form.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            tbSpeed.Text = form.ReturnValue.ToString("N0");
                            mf.Products.Item(mf.CurrentProduct()).SimPWM = (byte)form.ReturnValue;
                        }
                    }
                    this.ActiveControl = lbMPH;
                    break;

                default:
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
                    break;
            }
        }

        private void UpdateForm()
        {
            if (rbRate.Checked)
            {
                if (CurrentSim != SimType.VirtualNano)
                {
                    CurrentSim = SimType.VirtualNano;
                    mf.Products.Item(mf.CurrentProduct()).SimulationType = CurrentSim;
                }
            }
            else if (rbSpeed.Checked)
            {
                if (CurrentSim != SimType.Speed)
                {
                    CurrentSim = SimType.Speed;
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
            }
            else if (rbPWM.Checked)
            {
                if (CurrentSim != SimType.PWM)
                {
                    CurrentSim = SimType.PWM;
                    mf.Products.Item(mf.CurrentProduct()).SimulationType = CurrentSim;
                    lbMPH.Text = "PWM";
                    tbSpeed.Text = mf.Products.Item(mf.CurrentProduct()).SimPWM.ToString("N0");
                }
            }
            else
            {
                // default to off
                if (CurrentSim != SimType.None)
                {
                    CurrentSim = SimType.None;
                    mf.Products.Item(mf.CurrentProduct()).SimulationType = CurrentSim;
                }
            }
        }
    }
}