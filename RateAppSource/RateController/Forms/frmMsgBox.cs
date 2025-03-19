using RateController.Classes;
using System;
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
                Props.SaveFormLocation(this);
            }
            Result = true;
            this.Hide();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                Props.SaveFormLocation(this);
            }
            Result = false;
            this.Hide();
        }

        private void frmMsgBox_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
        }

        private void frmMsgBox_Load(object sender, EventArgs e)
        {
            Props.LoadFormLocation(this);
            this.BackColor = Properties.Settings.Default.MainBackColour;
        }
    }
}