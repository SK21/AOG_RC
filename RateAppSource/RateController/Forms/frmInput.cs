using RateController.Classes;
using System.IO;
using System.Windows.Forms;

namespace RateController.Forms
{
    public partial class frmInput : Form
    {
        private bool CheckFileName = false;
        private string cInputValue = "";
        private bool cResult;

        public frmInput(string Message, string Title = "Input", bool IsFileName = false)
        {
            InitializeComponent();
            label1.Text = Message;
            this.Text = Title;
            CheckFileName = IsFileName;
        }

        public string InputValue { get { return cInputValue; } }
        public bool Result { get { return cResult; } }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            cResult = false;
            this.Hide();
        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            bool Done = false;
            cInputValue = tbInput.Text;
            if (CheckFileName)
            {
                cInputValue = Path.GetFileName(cInputValue);
                if (FileNameValidator.IsValidFileName(cInputValue))
                {
                    Done = true;
                }
                else
                {
                    Props.ShowMessage("Invalid file name.", "Help", 10000);
                    Done = false;
                }
            }
            else
            {
                Done = true;
            }

            if (Done)
            {
                cResult = true;
                this.Hide();
            }
        }

        private void frmInput_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                Props.SaveFormLocation(this);
            }
        }

        private void frmInput_Load(object sender, System.EventArgs e)
        {
            Props.LoadFormLocation(this);
            this.BackColor = Properties.Settings.Default.MainBackColour;
        }
    }
}