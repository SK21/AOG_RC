using RateController.Classes;
using RateController.Language;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.Forms
{
    // Red Rx button tap (Group2_Step3_Prescription_State_Design.md § 4): which ISOBUS products aren't getting RC's
    // map rate and why. The form only reports the choice; frmMain acts on it.
    public partial class frmRxDialog : Form
    {
        private readonly List<string[]> Entries;      // per product: [0] "1. Seed: the implement refused.", [1] advice

        public frmRxDialog(List<string[]> ProductEntries, bool CanAskAgain)
        {
            InitializeComponent();
            Entries = ProductEntries;
            this.Text = Lang.lgRxTitle;
            btnAskAgain.Text = Lang.lgRxAskAgain;
            btnAskAgain.Enabled = CanAskAgain;     // cases 5 and 6 only: nothing to ask the implement
            btnVROff.Text = Lang.lgRxVROff;
            lbHelp.Text = Lang.lgRxAskAgainHelp + Environment.NewLine + Environment.NewLine + Lang.lgRxVROffHelp;
        }

        public enum Choices
        { Cancel, AskAgain, VROff }

        public Choices Choice { get; private set; } = Choices.Cancel;

        private void btnAskAgain_Click(object sender, EventArgs e)
        {
            Choice = Choices.AskAgain;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Choice = Choices.Cancel;
            this.Close();
        }

        private void btnVROff_Click(object sender, EventArgs e)
        {
            Choice = Choices.VROff;
            this.Close();
        }

        private void frmRxDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
        }

        private void frmRxDialog_Load(object sender, EventArgs e)
        {
            Props.LoadFormLocation(this);
            Color Back = Properties.Settings.Default.MainBackColour;
            Color Fore = Properties.Settings.Default.MainForeColour;
            this.BackColor = Back;
            rtbList.BackColor = Back;
            rtbList.ForeColor = Fore;
            lbHelp.ForeColor = Fore;
            btnAskAgain.ForeColor = Fore;
            btnVROff.ForeColor = Fore;
            ShowEntries();
        }

        // the top line, then per product its line at full size and the advice under it in the help lines' size,
        // with a gap between products
        private void ShowEntries()
        {
            Font Big = rtbList.Font;
            Font Small = lbHelp.Font;
            rtbList.Clear();
            AddText(Lang.lgRxIntro + Environment.NewLine, Big);
            foreach (string[] Entry in Entries)
            {
                AddText(Environment.NewLine + Entry[0] + Environment.NewLine, Big);
                if (Entry[1].Length > 0) AddText(Entry[1] + Environment.NewLine, Small);
            }
            rtbList.SelectionStart = 0;
            rtbList.SelectionLength = 0;
        }

        private void AddText(string Text, Font F)
        {
            rtbList.SelectionStart = rtbList.TextLength;
            rtbList.SelectionLength = 0;
            rtbList.SelectionFont = F;
            rtbList.SelectionColor = rtbList.ForeColor;
            rtbList.AppendText(Text);
        }
    }
}
