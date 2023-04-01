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
            this.Left = LS.Left + LS.Width- this.Width;

            LS.WindowState = FormWindowState.Minimized;
        }

        private void RestoreLC_Click(object sender, EventArgs e)
        {
            LS.WindowState = FormWindowState.Normal;
            this.Close();
        }
    }
}
