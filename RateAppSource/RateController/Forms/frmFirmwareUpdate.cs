using RateController.Classes;
using RateController.Language;
using RateController.PGNs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace RateController.Forms
{
    // Firmware update of one Teensy rate board, opened from the Boards page for the selected row.
    // The method follows the board's firmware date (Design Docs\Teensy_CAN_Firmware_Update.md §3.4):
    // MinProtocolDate or newer uses the new method on the app's link (and offers the old one on
    // Ethernet when it gets no reply); older firmware uses the old method, on Ethernet only.
    public partial class frmFirmwareUpdate : Form
    {
        private readonly ulong cMAC;            // 0 for an old-firmware row (no MAC known)
        private readonly int cModuleID;
        private CancellationTokenSource cCancel;
        private string cFile = "";              // the hex file chosen
        private string cFileVersion = "";       // "v2026.11.14", "" when unknown (a browsed file)
        private ushort cFileInoID = 0;          // InoID from the file's firmware tag, 0 without one
        private string cMessage = "";           // the last progress or result message
        private bool cBusy = false;             // updating or downloading
        private bool cUpdated = false;          // the update finished: its message stays, the board's gone from this row

        public frmFirmwareUpdate(ulong MAC, int ModuleID)
        {
            InitializeComponent();
            cMAC = MAC;
            cModuleID = ModuleID;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            ofdHex.Filter = Lang.lgFwHexFiles + " (*.hex)|*.hex";
            if (File.Exists(cFile)) ofdHex.InitialDirectory = Path.GetDirectoryName(cFile);
            if (ofdHex.ShowDialog(this) == DialogResult.OK)
            {
                SetFile(ofdHex.FileName, string.Equals(Path.GetFullPath(ofdHex.FileName), Path.GetFullPath(FirmwareFiles.TeensyHex), StringComparison.OrdinalIgnoreCase)
                    ? FirmwareFiles.TeensyVersion() : "");
                cMessage = "";
                UpdateForm();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            cCancel?.Cancel();
        }

        // Versions.json and ModulesHex.zip from the latest release, as PCBsetup gets them
        private async void btnCheck_Click(object sender, EventArgs e)
        {
            SetBusy(true);
            try
            {
                cMessage = Lang.lgFwChecking;
                UpdateForm();

                clsVersionChecker VC = new clsVersionChecker();
                bool Changed = await VC.HasVersionChanged();
                string Latest = VC.ModuleVersion((int)ModuleTypes.Teensy_Rate);
                if (Latest == "N/A") Latest = "";

                bool Get = Changed || Latest == "" || FirmwareFiles.TeensyVersion() != Latest;
                if (!Get) Get = Confirm(Lang.lgFwNoNew);

                if (Get)
                {
                    if (await FirmwareFiles.Download(Latest))
                    {
                        // the app's own file stays when it is newer than the release
                        string Hex = FirmwareFiles.BestTeensyHex(out string Version);
                        SetFile(Hex, Version);
                        cMessage = Lang.lgFwDownloaded;
                    }
                    else
                    {
                        cMessage = Lang.lgFwDownloadFailed;
                    }
                }
                else
                {
                    cMessage = "";
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmFirmwareUpdate/btnCheck_Click: " + ex.Message);
                cMessage = Lang.lgFwDownloadFailed;
            }
            SetBusy(false);
            UpdateForm();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            BoardInfo Brd = Board();
            List<byte[]> Records = FirmwareUpdater.ReadHex(cFile);
            bool Tagged = false;
            byte FileType = 0;
            ushort FileID = 0;
            if (Records != null) Tagged = FirmwareUpdater.FindTag(Records, out FileType, out FileID);

            if (Records == null)
            {
                cMessage = Lang.lgFwBadFile;
                UpdateForm();
            }
            else if (!Tagged)
            {
                // another kind of firmware, or a rate build from before the tag (PCBsetup installs those)
                cMessage = Lang.lgFwNoTag;
                UpdateForm();
            }
            else if (FileType != (byte)ModuleTypes.Teensy_Rate)
            {
                cMessage = Lang.lgFwWrongType;
                UpdateForm();
            }
            else if (Blocker(Brd) == "")
            {
                string Version = FileText();
                string Text = string.Format(Lang.lgFwConfirm, ModuleText(Brd), Version);
                int FileDate = PGN32403.InoDate(FileID);
                if (FileDate == 0)
                {
                    Text += "\n\n" + Lang.lgFwDateUnknown;
                }
                else if (Brd != null && FileDate < PGN32403.InoDate(Brd.InoID))
                {
                    Text += "\n\n" + Lang.lgFwOlder;
                }

                if (Confirm(Text)) await RunUpdate(Records, Brd);
            }
        }

        // the live board for this row, null for an old-firmware row or a board that went
        private BoardInfo Board()
        {
            return cMAC != 0 ? Core.ModulesBoardID.LiveBoard(cMAC) : null;
        }

        // why Update can't start now, "" when it can
        private string Blocker(BoardInfo Brd)
        {
            string Result = "";
            bool Connected = cMAC != 0 ? Brd != null : Core.ModulesBoardID.Rejected(cModuleID);
            if (!Connected)
            {
                Result = Lang.lgFwNotConnected;
            }
            else if (OldFirmware(Brd) && Props.CanEnabled)
            {
                Result = Lang.lgFwOldOnCan;
            }
            else if (OldFirmware(Brd) && ModuleID(Brd) > 15)
            {
                // the old method finds the board by module number, and a new board has none
                Result = Lang.lgFwNotConnected;
            }
            else if (!File.Exists(cFile))
            {
                Result = Lang.lgFwNone;
            }
            else if (Core.SectionControl.MasterOn)
            {
                Result = Lang.lgFwMasterOn;
            }
            return Result;
        }

        private bool Confirm(string Message)
        {
            bool Result;
            using (var Hlp = new frmMsgBox(Message.Replace("\r\n", "\n"), Lang.lgFwTitle))
            {
                Hlp.TopMost = true;
                Hlp.ShowDialog(this);
                Result = Hlp.Result;
            }
            return Result;
        }

        private void frmFirmwareUpdate_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer1.Enabled = false;
            Props.SaveFormLocation(this);
        }

        private void frmFirmwareUpdate_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cBusy)
            {
                e.Cancel = true;
                Props.ShowMessage(Lang.lgFwRunning, Lang.lgFwTitle, 5000);
            }
        }

        private void frmFirmwareUpdate_Load(object sender, EventArgs e)
        {
            Props.LoadFormLocation(this);
            this.BackColor = Properties.Settings.Default.MainBackColour;
            SetLanguage();
            string Hex = FirmwareFiles.BestTeensyHex(out string Version);
            if (Hex != "") SetFile(Hex, Version);
            UpdateForm();
            timer1.Enabled = true;

            // no firmware file at all: get it now, as the button does, once the page shows
            if (Hex == "") BeginInvoke((Action)(() => btnCheck_Click(btnCheck, EventArgs.Empty)));
        }

        // the file's firmware date from its tag (as the board's is shown), else its version or name
        private string FileText()
        {
            string Result;
            if (cFileInoID != 0)
            {
                Result = Props.ParseDate(cFileInoID.ToString());
            }
            else
            {
                Result = cFileVersion != "" ? cFileVersion : Path.GetFileName(cFile);
            }
            return Result;
        }

        private int ModuleID(BoardInfo Brd)
        {
            return Brd != null ? Brd.ID : cModuleID;
        }

        private string ModuleText(BoardInfo Brd)
        {
            int ID = ModuleID(Brd);
            return ID < Props.MaxModules ? ID.ToString() : Lang.lgNew;
        }

        // firmware from before MinProtocolDate, or no MAC known (pre-identity firmware)
        private bool OldFirmware(BoardInfo Brd)
        {
            return Brd == null || PGN32403.InoDate(Brd.InoID) < FirmwareUpdater.MinProtocolDate;
        }

        private string OutcomeText(FirmwareUpdater.Outcome Result)
        {
            string Text;
            switch (Result)
            {
                case FirmwareUpdater.Outcome.Done:
                    Text = Lang.lgFwDone;
                    break;

                case FirmwareUpdater.Outcome.NotBack:
                    Text = string.Format(Lang.lgFwNotBack, Lang.lgBoards);
                    break;

                case FirmwareUpdater.Outcome.NoReply:
                    Text = Lang.lgFwNoReply;
                    break;

                case FirmwareUpdater.Outcome.WrongType:
                    Text = Lang.lgFwWrongType;
                    break;

                case FirmwareUpdater.Outcome.DoneNew:
                    Text = Lang.lgFwDoneNew;
                    break;

                case FirmwareUpdater.Outcome.PortBusy:
                    Text = Lang.lgFwPortBusy;
                    break;

                case FirmwareUpdater.Outcome.WrongModule:
                    Text = Lang.lgFwWrongModule;
                    break;

                case FirmwareUpdater.Outcome.Refused:
                    Text = Lang.lgFwRefused;
                    break;

                case FirmwareUpdater.Outcome.NoAnswer:
                    Text = Lang.lgFwNoAnswer;
                    break;

                case FirmwareUpdater.Outcome.CountWrong:
                    Text = Lang.lgFwCountWrong;
                    break;

                case FirmwareUpdater.Outcome.Cancelled:
                    Text = Lang.lgFwCancelled;
                    break;

                default:
                    Text = Lang.lgFwFailed;
                    break;
            }
            return Text;
        }

        private async System.Threading.Tasks.Task RunUpdate(List<byte[]> Records, BoardInfo Brd)
        {
            cCancel = new CancellationTokenSource();
            SetBusy(true);
            pbProgress.Value = 0;
            FirmwareUpdater Updater = new FirmwareUpdater();
            Updater.ProgressChanged += Updater_ProgressChanged;
            Updater.StageChanged += Updater_StageChanged;
            Core.FirmwareUpdate = Updater;

            FirmwareUpdater.Outcome Result;
            try
            {
                if (OldFirmware(Brd))
                {
                    Result = await Updater.RunLegacyAsync(Records, ModuleID(Brd), cCancel.Token);
                }
                else
                {
                    Result = await Updater.RunAsync(Records, Brd.MAC, Props.CanEnabled, cCancel.Token);

                    // a board dated after MinProtocolDate without the new method doesn't answer it
                    if (Result == FirmwareUpdater.Outcome.NoReply && !Props.CanEnabled && Brd.ID <= 15
                        && Confirm(Lang.lgFwTryOld))
                    {
                        Result = await Updater.RunLegacyAsync(Records, Brd.ID, cCancel.Token);
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmFirmwareUpdate/RunUpdate: " + ex.Message);
                Result = FirmwareUpdater.Outcome.Failed;
            }

            Core.FirmwareUpdate = null;
            Updater.ProgressChanged -= Updater_ProgressChanged;
            Updater.StageChanged -= Updater_StageChanged;

            cUpdated = Result == FirmwareUpdater.Outcome.Done || Result == FirmwareUpdater.Outcome.DoneNew;
            if (cUpdated) pbProgress.Value = 100;
            cMessage = OutcomeText(Result);
            SetBusy(false);
            cCancel.Dispose();
            cCancel = null;
            UpdateForm();
        }

        private void SetBusy(bool Busy)
        {
            cBusy = Busy;
            btnCheck.Enabled = !Busy;
            btnBrowse.Enabled = !Busy;
            btnClose.Enabled = !Busy;
            btnCancel.Enabled = Busy && cCancel != null;
            if (Busy) btnStart.Enabled = false;
        }

        // the chosen file, and the InoID of its firmware tag when it is Teensy rate firmware
        private void SetFile(string FileName, string Version)
        {
            cFile = FileName;
            cFileVersion = Version;
            cFileInoID = 0;
            List<byte[]> Records = FirmwareUpdater.ReadHex(FileName);
            if (Records != null && FirmwareUpdater.FindTag(Records, out byte Type, out ushort ID)
                && Type == (byte)ModuleTypes.Teensy_Rate) cFileInoID = ID;
        }

        private void SetLanguage()
        {
            this.Text = Lang.lgFwTitle;
            lbBoardName.Text = Lang.lgBoard;
            lbModuleName.Text = Lang.lgModule;
            lbLinkName.Text = Lang.lgFwConnection;
            lbCurrentName.Text = Lang.lgFwBoardFirmware;
            lbFileName.Text = Lang.lgFwFile;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!cBusy) UpdateForm();
        }

        private void UpdateForm()
        {
            BoardInfo Brd = Board();
            lbBoard.Text = Brd != null ? Brd.Text : (cMAC != 0 ? "—" : string.Format(Lang.lgFwOldModule, cModuleID));
            lbModule.Text = ModuleText(Brd);
            lbLink.Text = Props.CanEnabled ? "CanBus" : "Ethernet";
            lbCurrent.Text = Brd != null ? Props.ParseDate(Brd.InoID.ToString()) : Lang.lgFwUnknown;
            lbFile.Text = !File.Exists(cFile) ? Lang.lgFwNone : FileText();

            string Block = Blocker(Brd);
            if (!cBusy) btnStart.Enabled = Block == "";

            // a reason Update can't start comes before an older message
            lbStatus.Text = !cBusy && !cUpdated && Block != "" && Block != Lang.lgFwNone ? Block : cMessage;
        }

        private void Updater_ProgressChanged(int Percent)
        {
            if (IsHandleCreated && !IsDisposed)
            {
                BeginInvoke((Action)(() =>
                {
                    pbProgress.Value = Math.Max(0, Math.Min(100, Percent));
                    cMessage = string.Format(Lang.lgFwSending, Percent);
                    lbStatus.Text = cMessage;
                }));
            }
        }

        private void Updater_StageChanged(FirmwareUpdater.Stage Stage)
        {
            if (IsHandleCreated && !IsDisposed)
            {
                BeginInvoke((Action)(() =>
                {
                    switch (Stage)
                    {
                        case FirmwareUpdater.Stage.Starting:
                            cMessage = Lang.lgFwStarting;
                            break;

                        case FirmwareUpdater.Stage.Sending:
                            cMessage = string.Format(Lang.lgFwSending, 0);
                            break;

                        case FirmwareUpdater.Stage.Installing:
                            cMessage = Lang.lgFwInstalling;
                            pbProgress.Value = 100;
                            break;
                    }
                    lbStatus.Text = cMessage;
                }));
            }
        }
    }
}
