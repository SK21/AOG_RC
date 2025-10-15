using GMap.NET.MapProviders;
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
    public partial class frmSaveType : Form
    {
        private int cResult = 0;

        public frmSaveType()
        {
            InitializeComponent();
        }

        public int Result
        { get { return cResult; } }

        private void bntOK_Click(object sender, EventArgs e)
        {
            if (ckShapeFile.Checked)
            {
                cResult = 1;
            }
            else
            {
                cResult = 2;
            }
            if (this.WindowState == FormWindowState.Normal)
            {
                Props.SaveFormLocation(this);
            }
            this.Hide();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                Props.SaveFormLocation(this);
            }
            cResult = 0;
            this.Hide();
        }

        private void frmSaveType_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
        }

        private void frmSaveType_Load(object sender, EventArgs e)
        {
            Props.LoadFormLocation(this);
            this.BackColor = Properties.Settings.Default.MainBackColour;
        }
    }
}