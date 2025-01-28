using System;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuBoards : Form
    {
        private int cBoard = 0;
        private bool cCancelled = true;
        private FormStart mf;

        public frmMenuBoards(FormStart CallingForm)
        {
            InitializeComponent();
            mf = CallingForm;
        }

        public int Board
        {
            get { return cBoard; }
            set
            {
                cBoard = value;
                SetOptions(cBoard);
            }
        }

        public bool Cancelled
        { get { return cCancelled; } }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mf.Tls.SaveFormData(this);
            cCancelled = true;
            this.Hide();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            mf.Tls.SaveFormData(this);
            if (rbESP32.Checked)
            {
                cBoard = 2;
            }
            else if (rbTeensy.Checked)
            {
                cBoard = 1;
            }
            else
            {
                cBoard = 0;
            }
            cCancelled = false;
            this.Hide();
        }

        private void frmMenuBoards_FormClosed(object sender, FormClosedEventArgs e)
        {
            mf.Tls.SaveFormData(this);
        }

        private void frmMenuBoards_Load(object sender, EventArgs e)
        {
            this.BackColor = Properties.Settings.Default.BackColour;
            mf.Tls.LoadFormData(this);
        }

        private void SetOptions(int board)
        {
            switch (board)
            {
                case 1:
                    rbTeensy.Checked = true;
                    break;

                case 2:
                    rbESP32.Checked = true;
                    break;

                default:
                    rbNano.Checked = true;
                    break;
            }
        }
    }
}