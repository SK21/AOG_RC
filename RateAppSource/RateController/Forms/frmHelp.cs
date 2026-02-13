using RateController.Classes;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmHelp : Form
    {
        // Track how many help windows are open
        private static int OpenHelpCount = 0;

        // Easy-to-change cascading offset
        private static int OffsetX = 50;
        private static int OffsetY = 50;

        // This instance's index in the cascade
        private int myIndex = 0;

        public frmHelp(string Message, string Title = "Help", int timeInMsec = 30000)
        {
            InitializeComponent();

            this.Text = Title;
            label1.Text = Message;
            timer1.Interval = timeInMsec;

            int len = Message.Length;
            this.Width = 450;

            int ht = 20 + (len / 34) * 40;
            if (ht < 160)
                ht = 160;
            else if (ht > 500)
                ht = 500;

            this.Height = ht;

            panel1.Width = this.Width - 40;
            panel1.Height = this.Height - 40;
            label1.MaximumSize = new Size(panel1.Width - 10, 0);

            // Assign index and increment global count
            myIndex = OpenHelpCount++;
        }

        private void frmHelp_Load(object sender, EventArgs e)
        {
            try
            {
                // Load last saved location
                Props.LoadFormLocation(this);

                // Apply cascading offset
                this.Left += myIndex * OffsetX;
                this.Top += myIndex * OffsetY;

                this.BackColor = Properties.Settings.Default.MainBackColour;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmHelp/frmHelp_Load: " + ex.Message);
            }
        }

        private void frmHelp_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Decrement count when closing
            if (OpenHelpCount > 0)
                OpenHelpCount--;

            Props.SaveFormLocation(this);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            timer1.Dispose();

            Props.SaveFormLocation(this);
            Close();
        }
    }
}