using RateController.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RateController.Forms
{
    public partial class frmRates: Form
    {
        private RateType RateToDisplay;
        private int ProductToDisplay;
        private int RefreshInterval;
        private double Resolution;
        public frmRates()
        {
            InitializeComponent();
        }

        private void rbProductA_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void lbHigh_Click(object sender, EventArgs e)
        {

        }

        private void HSlow_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void frmRates_Load(object sender, EventArgs e)
        {
            Props.LoadFormLocation(this);
        }

        private void frmRates_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
        }
    }
}
