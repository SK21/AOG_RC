using RateController.Classes;
using RateController.Language;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RateController.Menu
{
    // Every link's PGN traffic with filters (Design Docs\PGN_Log_Design.md). Opened from the help page.
    // The view follows new entries only while it is scrolled to the bottom, so reading further up
    // doesn't jump back. UDP / CAN / TC filter the view; AOG also turns recording of AgIO traffic on or off.
    public partial class frmMenuPgnLog : Form
    {
        private const int MaxLines = 500;
        private bool Initializing;
        private bool Paused;

        public frmMenuPgnLog()
        {
            InitializeComponent();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Core.PgnLog.Clear();
            ShowLog(true);
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            Core.PgnLog.Flush();
            if (!Props.ShowFile(clsPgnLog.FileName))
            {
                Props.ShowMessage("File not found.");
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            Paused = !Paused;
            btnPause.Image = Paused ? Properties.Resources.Start : Properties.Resources.Pause;
            if (!Paused) ShowLog(true);
        }

        private void ckAOG_CheckedChanged(object sender, EventArgs e)
        {
            Core.PgnLog.AogOn = ckAOG.Checked;
            if (!Initializing) ShowLog(true);
        }

        private void Filter_Changed(object sender, EventArgs e)
        {
            if (!Initializing) ShowLog(true);
        }

        private void frmMenuPgnLog_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer1.Enabled = false;
            Props.SaveFormLocation(this);
        }

        private void frmMenuPgnLog_Load(object sender, EventArgs e)
        {
            Props.LoadFormLocation(this);
            this.BackColor = Properties.Settings.Default.MainBackColour;
            SetLanguage();

            Initializing = true;
            ckUDP.Checked = true;
            ckCAN.Checked = true;
            ckTC.Checked = true;
            ckAOG.Checked = Core.PgnLog.AogOn;

            cbModule.Items.Add(Lang.lgAll);
            for (int i = 0; i < Props.MaxModules; i++) cbModule.Items.Add(i.ToString());
            cbModule.SelectedIndex = 0;

            cbPGN.Items.Add(Lang.lgAll);
            cbPGN.SelectedIndex = 0;
            Initializing = false;

            ShowLog(true);
            timer1.Enabled = true;
        }

        private bool LinkShown(PgnLink Link)
        {
            bool Result;
            switch (Link)
            {
                case PgnLink.UDP: Result = ckUDP.Checked; break;
                case PgnLink.CAN: Result = ckCAN.Checked; break;
                case PgnLink.TC: Result = ckTC.Checked; break;
                default: Result = ckAOG.Checked; break;
            }
            return Result;
        }

        private void SetLanguage()
        {
            this.Text = Lang.lgPgnLog;
            lbModule.Text = Lang.lgModule;
            btnClear.Text = Lang.lgClear;
            btnFile.Text = Lang.lgFile;
        }

        // the module an entry belongs to: its own for module PGNs; for a TC line, the module of the product on
        // that channel (channel n = product n); -1 for notes and AgIO
        private static int EntryModule(clsPgnLog.Entry E)
        {
            int Result = E.Module;
            if (E.Link == PgnLink.TC)
            {
                Result = -1;
                if (E.Sensor >= 1 && E.Sensor <= Props.MaxProducts) Result = Core.Products.Item(E.Sensor - 1).ModuleID;
            }
            return Result;
        }

        // an entry passes the link, module and PGN filters
        private bool PassesFilters(clsPgnLog.Entry E)
        {
            bool Result = LinkShown(E.Link);
            if (Result && cbModule.SelectedIndex > 0) Result = EntryModule(E) == cbModule.SelectedIndex - 1;
            if (Result && cbPGN.SelectedIndex > 0) Result = E.PgnText == (string)cbPGN.SelectedItem;
            return Result;
        }

        // Force: rebuild even when scrolled up (a filter changed, cleared, resumed)
        private void ShowLog(bool Force)
        {
            int Visible = Math.Max(1, lstLog.ClientSize.Height / Math.Max(1, lstLog.ItemHeight));
            bool AtBottom = lstLog.Items.Count == 0 || lstLog.TopIndex + Visible >= lstLog.Items.Count;
            if (Force || AtBottom)
            {
                List<clsPgnLog.Entry> Entries = Core.PgnLog.Snapshot();
                UpdatePgnList(Entries);

                List<string> Lines = new List<string>();
                for (int i = Entries.Count - 1; i >= 0 && Lines.Count < MaxLines; i--)
                {
                    if (PassesFilters(Entries[i])) Lines.Add(clsPgnLog.Line(Entries[i]));
                }
                Lines.Reverse();

                lstLog.BeginUpdate();
                lstLog.Items.Clear();
                lstLog.Items.AddRange(Lines.ToArray());
                if (lstLog.Items.Count > 0) lstLog.TopIndex = lstLog.Items.Count - 1;
                lstLog.EndUpdate();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!Paused) ShowLog(false);
        }

        // the PGN list offers every PGN seen so far; the selection is kept
        private void UpdatePgnList(List<clsPgnLog.Entry> Entries)
        {
            foreach (clsPgnLog.Entry E in Entries)
            {
                if (!string.IsNullOrEmpty(E.PgnText) && !cbPGN.Items.Contains(E.PgnText)) cbPGN.Items.Add(E.PgnText);
            }
        }
    }
}
