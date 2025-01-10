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
    public partial class frmMsgBox : Form
    {
        private bool cResult;
        private FormStart mf;

        public frmMsgBox(FormStart CallingForm, string Message, string Title = "Help", bool Shrink = false)
        {
            mf = CallingForm;
            InitializeComponent();
            this.Text = Title;
            label1.Text = Message;

            if (Shrink)
            {
                panel1.Height = 60;
                this.Height = 198;
                btnCancel.Top = 78;
                bntOK.Top = 78;
            }
            else
            {
                panel1.Height = 303;
                this.Height = 441;
                btnCancel.Top = 321;
                bntOK.Top = 321;
            }
        }

        public bool Result { get => cResult; set => cResult = value; }

        private void bntOK_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                mf.Tls.SaveFormData(this);
            }
            Result = true;
            this.Hide();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                mf.Tls.SaveFormData(this);
            }
            Result = false;
            this.Hide();
        }

        private void frmMsgBox_FormClosed(object sender, FormClosedEventArgs e)
        {
            mf.Tls.SaveFormData(this);
        }

        private void frmMsgBox_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            this.BackColor = Properties.Settings.Default.MainBackColour;
        }
    }
}