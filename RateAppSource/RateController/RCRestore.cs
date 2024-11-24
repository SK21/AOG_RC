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
        private frmLargeScreen LS;

        public RCRestore(frmLargeScreen CallingForm)
        {
            LS = CallingForm;

            this.TransparencyKey = this.BackColor;

            InitializeComponent();
        }

        private void RCRestore_Load(object sender, EventArgs e)
        {
            this.Top = LS.Top + LS.Height - this.Height;
            this.Left = LS.Left + LS.Width - this.Width;

            LS.WindowState = FormWindowState.Minimized;
            lbRateAmount.Font = new Font("MS Gothic", 16, FontStyle.Bold);
            lbRateAmount.ForeColor = Color.Yellow;
            timer1.Enabled = true;
        }

        private void RestoreLC_Click(object sender, EventArgs e)
        {
            LS.WindowState = FormWindowState.Normal;
            timer1.Enabled = false;
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            switch (LS.mf.RateType)
            {
                case 1:
                    lbRateAmount.Text = LS.Prd.CurrentRate().ToString("N1");
                    break;

                default:
                    lbRateAmount.Text = LS.Prd.SmoothRate().ToString("N1");
                    break;
            }
        }
    }
}