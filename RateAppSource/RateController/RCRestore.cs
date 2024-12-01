using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RateController
{
    public partial class RCRestore : Form
    {
        private clsProduct cCurrentProduct;
        private int cRateType;
        private Form FormToHide;

        public RCRestore(Form CallingForm, int RateType, clsProduct CurrentProduct)
        {
            FormToHide = CallingForm;
            cRateType = RateType;
            cCurrentProduct = CurrentProduct;
            this.TransparencyKey = this.BackColor;
            InitializeComponent();
        }

        private void RCRestore_Load(object sender, EventArgs e)
        {
            this.Top = FormToHide.Top + FormToHide.Height - this.Height;
            this.Left = FormToHide.Left + FormToHide.Width - this.Width;

            FormToHide.WindowState = FormWindowState.Minimized;
            timer1.Enabled = true;
        }

        private void RestoreLC_Click(object sender, EventArgs e)
        {
            FormToHide.WindowState = FormWindowState.Normal;
            timer1.Enabled = false;
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            switch (cRateType)
            {
                case 1:
                    lbRateAmount.Text = cCurrentProduct.CurrentRate().ToString("N1");
                    break;

                default:
                    lbRateAmount.Text = cCurrentProduct.SmoothRate().ToString("N1");
                    break;
            }
        }
    }
}