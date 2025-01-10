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
    public partial class frmPins : Form
    {
        public FormStart mf;

        public frmPins(FormStart Main)
        {
            InitializeComponent();
            mf = Main;
        }

        private void frmPins_FormClosed(object sender, FormClosedEventArgs e)
        {
                mf.Tls.SaveFormData(this);
        }

        private void frmPins_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            this.BackColor = Properties.Settings.Default.MainBackColour;
        }
    }
}