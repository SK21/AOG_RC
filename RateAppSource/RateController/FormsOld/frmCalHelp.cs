using RateController.Language;
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
    public partial class frmCalHelp : Form
    {
        private FormStart mf;
        public frmCalHelp(FormStart Main)
        {
            InitializeComponent();
            mf = Main;
            textBox2.Text = Lang.lgCalHelp;
        }

        private void frmCalHelp_FormClosed(object sender, FormClosedEventArgs e)
        {
                mf.Tls.SaveFormData(this);
        }

        private void frmCalHelp_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            this.BackColor = Properties.Settings.Default.MainBackColour;
            textBox2.BackColor = Properties.Settings.Default.MainBackColour;
            textBox2.SelectionLength = 0;
            textBox2.SelectionStart = 0;
        }
    }
}
