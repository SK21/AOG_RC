using RateController.Classes;
using System;

namespace RateController.PGNs
{
    public enum RemoteEndReasons
    { None, Cab, Timeout }

    public class PGN32618
    {
        // to Rate Controller from arduino switch box
        // 0   106
        // 1   127
        // 2   status
        //      - bit 0 -
        //      - bit 1 MasterOn
        //      - bit 2 MasterOff
        //      - bit 3 RateUp
        //      - bit 4 RateDown
        //      - bit 5 AutoSection
        //      - bit 6 AutoRate
        //		- bit 7 Work switch
        // 3    sw0 to sw7
        // 4    sw8 to sw15
        // 5    crc

        // alternate pgn format
        // 0   106
        // 1   127
        // 2   status
        //      - bit 0 -
        //      - bit 1 MasterOn
        //      - bit 2 MasterOff
        //      - bit 3 RateUp
        //      - bit 4 RateDown
        //      - bit 5 AutoSection
        //      - bit 6 AutoRate
        //		- bit 7 Work switch
        // 3    sw0 to sw7
        // 4    sw8 to sw15
        // 5    InoID Lo
        // 6    InoID Hi
        // 7    -
        // 8    crc

        private const byte HeaderHi = 127;
        private const byte HeaderLo = 106;
        private const byte MinByteCount = 6;
        private const int RemoteRateHoldMs = 750;
        private const int RemoteTimeoutSeconds = 30;
        private bool cAutoRateEnabled = false;
        private bool cAutoSectionEnabled = false;
        private UInt16 cInoID;
        private bool cMasterOn = false;
        private bool cRateDown = false;
        private bool cRateUp = false;
        private bool cRemoteActive = false;
        private bool cRemoteAutoRate = false;
        private bool cRemoteAutoSection = false;
        private RemoteEndReasons cRemoteEndReason = RemoteEndReasons.None;
        private bool cRemoteForcedOff = false;
        private bool cRemoteMaster = false;
        private int cRemoteRateDir = 0;          // 1 up, -1 down, 0 none
        private bool cWorkSwitchEnabled = false;
        private byte[][] LastData = new byte[2][];          // bytes 2-4 of last packet, 0 real, 1 virtual
        private DateTime[] LastDataTime = new DateTime[2];
        private DateTime ReceivedReal;
        private DateTime ReceiveTime;
        private DateTime RemoteLastPress;
        private DateTime RemoteRateTime;
        private bool[] RemoteSW = new bool[16];
        private System.Windows.Forms.Timer RemoteTimer = new System.Windows.Forms.Timer();
        private bool[] SW = new bool[24];

        public PGN32618()
        {
            SW[(int)SwIDs.AutoSection] = false;
            SW[(int)SwIDs.AutoRate] = false;

            cWorkSwitchEnabled = bool.TryParse(Props.GetProp("UseWorkSwitch"), out bool ws) ? ws : false;
            cAutoRateEnabled = bool.TryParse(Props.GetProp("SwitchboxAutoRateEnabled"), out bool ar) ? ar : true;
            cAutoSectionEnabled = bool.TryParse(Props.GetProp("SwitchboxAutoSectionEnabled"), out bool autosec) ? autosec : true;

            RemoteTimer.Interval = RemoteTimeoutSeconds * 1000;
            RemoteTimer.Tick += RemoteTimer_Tick;
        }

        public event EventHandler SwitchPGNreceived;

        public bool AutoRateEnabled
        {
            get { return cAutoRateEnabled; }
            set
            {
                cAutoRateEnabled = value;
                Props.SetProp("SwitchboxAutoRateEnabled", cAutoRateEnabled.ToString());
            }
        }

        public bool AutoRateOn
        {
            get
            {
                bool Result = false;
                if (AutoRateEnabled) Result = SW[(int)SwIDs.AutoRate];
                return Result;
            }
        }

        public bool AutoSectionEnabled
        {
            get { return cAutoSectionEnabled; }
            set
            {
                cAutoSectionEnabled = value;
                Props.SetProp("SwitchboxAutoSectionEnabled", cAutoSectionEnabled.ToString());
            }
        }

        public bool AutoSectionOn
        {
            get
            {
                bool Result = false;
                if (AutoSectionEnabled) Result = SW[(int)SwIDs.AutoSection];
                return Result;
            }
        }

        public bool MasterOn
        {
            get
            {
                bool Result = false;
                if (Props.RateCalibrationOn)
                {
                    Result = true;
                }
                else
                {
                    Result = cMasterOn;
                }
                return Result;
            }
            set { cMasterOn = value; }
        }

        public bool RateDown
        {
            get { return cRateDown; }
            set { cRateDown = value; }
        }

        public bool RateUp
        {
            get { return cRateUp; }
            set { cRateUp = value; }
        }

        public bool RemoteActive
        { get { return cRemoteActive; } }

        public RemoteEndReasons RemoteEndReason
        { get { return cRemoteEndReason; } }

        public int RemoteSecondsLeft
        {
            get
            {
                int Result = 0;
                if (cRemoteActive) Result = Math.Max(0, RemoteTimeoutSeconds - (int)(DateTime.Now - RemoteLastPress).TotalSeconds);
                return Result;
            }
        }

        public bool WorkOn
        {
            // returns true if switchbox work switch is on or any
            // module shows workswitch on
            get
            {
                bool Result = true;
                if (cWorkSwitchEnabled) Result = SW[23] || Core.ModulesStatus.WorkSwitchOn();
                return Result;
            }
        }

        public bool WorkSwitchEnabled
        {
            get { return cWorkSwitchEnabled; }
            set
            {
                cWorkSwitchEnabled = value;
                Props.SetProp("UseWorkSwitch", cWorkSwitchEnabled.ToString());
            }
        }

        public bool Connected()
        {
            bool Result = false;
            if (Props.RateCalibrationOn)
            {
                Result = true;
            }
            else
            {
                Result = (DateTime.Now - ReceiveTime).TotalSeconds < 4;
            }

            return Result;
        }

        public string ModuleVersion()
        {
            return Props.ParseDate(cInoID.ToString());
        }

        public bool ParseByteData(byte[] Data, bool RealSwitchBox = true)
        {
            bool Result = false;

            if (Data[0] == HeaderLo && Data[1] == HeaderHi && Data.Length >= MinByteCount && Core.Tls.GoodCRC(Data))
            {
                if (RealSwitchBox) ReceivedReal = DateTime.Now;

                CheckCabChanges(Data, RealSwitchBox ? 0 : 1);

                // master
                SW[1] = Core.Tls.BitRead(Data[2], 1);     // master on
                SW[2] = Core.Tls.BitRead(Data[2], 2);     // master off

                if (Props.MasterMaintained)
                {
                    // work around for a maintained master switch
                    //SW[1] = Core.Tls.BitRead(Data[3], 0);   // use section 1 to test
                    cMasterOn = SW[1];
                    SW[2] = !SW[1];
                }
                else
                {
                    if (SW[1]) cMasterOn = true;
                    else if (SW[2]) cMasterOn = false;
                }

                SW[3] = Core.Tls.BitRead(Data[2], 3);     // rate up
                SW[4] = Core.Tls.BitRead(Data[2], 4);     // rate down

                SW[21] = Core.Tls.BitRead(Data[2], 5);    // auto section
                SW[22] = Core.Tls.BitRead(Data[2], 6);    // auto rate
                SW[23] = Core.Tls.BitRead(Data[2], 7);    // work switch

                if (SW[3])
                {
                    cRateUp = true;
                    cRateDown = false;
                }
                else if (SW[4])
                {
                    cRateUp = false;
                    cRateDown = true;
                }
                else if (!SW[3] && !SW[4])
                {
                    cRateUp = false;
                    cRateDown = false;
                }

                // section switches
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        SW[5 + j + i * 8] = Core.Tls.BitRead(Data[i + 3], j);
                    }
                }

                cInoID = 0;
                if (Data.Length > MinByteCount)
                {
                    // alternate pgn format
                    cInoID = (ushort)(Data[5] | Data[6] << 8);
                }

                if (cRemoteActive)
                {
                    ApplyRemote();
                }
                else if (cRemoteForcedOff)
                {
                    cMasterOn = false;
                    SW[1] = false;
                    SW[2] = true;
                }

                SwitchPGNreceived?.Invoke(this, EventArgs.Empty);

                ReceiveTime = DateTime.Now;
                Result = true;
            }
            return Result;
        }

        public bool RealConnected()
        {
            return (DateTime.Now - ReceivedReal).TotalSeconds < 4;
        }

        public void RemoteAutoRatePressed()
        {
            StartRemote();
            cRemoteAutoRate = !cRemoteAutoRate;
            SW[(int)SwIDs.AutoRate] = cRemoteAutoRate;
            SwitchPGNreceived?.Invoke(this, EventArgs.Empty);
        }

        public void RemoteAutoSectionPressed()
        {
            // web page auto section, lets the operator turn auto off from the boom
            StartRemote();
            cRemoteAutoSection = !cRemoteAutoSection;
            SW[(int)SwIDs.AutoSection] = cRemoteAutoSection;
            SwitchPGNreceived?.Invoke(this, EventArgs.Empty);
        }

        public void RemoteMasterPressed()
        {
            // web page master, toggles against the effective master state
            StartRemote();
            cRemoteMaster = !cMasterOn;
            cMasterOn = cRemoteMaster;
            SW[1] = cRemoteMaster;
            SW[2] = !cRemoteMaster;
            SwitchPGNreceived?.Invoke(this, EventArgs.Empty);
            if (!Props.MasterMaintained)
            {
                // momentary, pulse only so it isn't seen as a held press (prime)
                SW[1] = false;
                SW[2] = false;
            }
        }

        public void RemotePrimePressed()
        {
            // same as the on-screen PRM button: master on and prime, momentary master and stopped only
            if (!Props.MasterMaintained && Props.Speed_KMH < 0.1)
            {
                StartRemote();
                cRemoteMaster = true;
                cMasterOn = true;
                SW[1] = true;
                SW[2] = false;
                Core.SectionControl.StartPrime();
                SwitchPGNreceived?.Invoke(this, EventArgs.Empty);
                SW[1] = false;
            }
        }

        public void RemotePrimeEnded()
        {
            // prime timed out without resume: master off, session and section states kept
            cRemoteMaster = false;
            cMasterOn = false;
            SwitchPGNreceived?.Invoke(this, EventArgs.Empty);
        }

        public void RemoteRateHold(int Direction)
        {
            // 1 up, -1 down, 0 release. The page repeats a held press every 250 ms;
            // a hold not repeated within RemoteRateHoldMs is released (dropped phone).
            StartRemote();
            cRemoteRateDir = Direction;
            RemoteRateTime = DateTime.Now;
            ApplyRemoteRate();
            SwitchPGNreceived?.Invoke(this, EventArgs.Empty);
        }

        public void RemoteSectionPressed(int ID)
        {
            if ((ID >= 0) && (ID <= 15))
            {
                StartRemote();
                RemoteSW[ID] = !RemoteSW[ID];
                SW[ID + (int)SwIDs.sw0] = RemoteSW[ID];
                SwitchPGNreceived?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool SectionSwitchOn(int ID)
        {
            bool Result = false;
            if (Props.RateCalibrationOn)
            {
                Result = true;
            }
            else
            {
                if ((ID >= 0) && (ID <= 15))
                {
                    Result = SW[ID + (int)SwIDs.sw0];
                }
            }
            return Result;
        }

        public bool SwitchIsOn(SwIDs ID)
        {
            return SW[(int)ID];
        }

        private void ApplyRemote()
        {
            for (int i = 0; i < 16; i++)
            {
                SW[i + (int)SwIDs.sw0] = RemoteSW[i];
            }
            SW[(int)SwIDs.AutoSection] = cRemoteAutoSection;
            SW[(int)SwIDs.AutoRate] = cRemoteAutoRate;
            ApplyRemoteRate();

            // the page owns master while the session is active. A cab master press is an
            // edge and has already ended the session, so anything still here is a level
            // being resent (maintained switch, or the on-screen panel's held master-off bit)
            cMasterOn = cRemoteMaster;
            if (Props.MasterMaintained)
            {
                SW[1] = cRemoteMaster;
                SW[2] = !cRemoteMaster;
            }
            else
            {
                SW[1] = false;
                SW[2] = false;
            }
        }

        private void ApplyRemoteRate()
        {
            if ((DateTime.Now - RemoteRateTime).TotalMilliseconds > RemoteRateHoldMs) cRemoteRateDir = 0;
            cRateUp = cRemoteRateDir > 0;
            cRateDown = cRemoteRateDir < 0;
            SW[3] = cRateUp;
            SW[4] = cRateDown;
        }

        private void CheckCabChanges(byte[] Data, int Source)
        {
            // edges only, compared with the previous packet from the same source
            byte Diff = 0;
            bool SectionsChanged = false;
            if (LastData[Source] != null && (DateTime.Now - LastDataTime[Source]).TotalSeconds < 4)
            {
                Diff = (byte)(Data[2] ^ LastData[Source][0]);
                SectionsChanged = (Data[3] != LastData[Source][1]) || (Data[4] != LastData[Source][2]);
            }
            LastData[Source] = new byte[] { Data[2], Data[3], Data[4] };
            LastDataTime[Source] = DateTime.Now;

            if ((Diff & 0x06) != 0) cRemoteForcedOff = false;     // master

            // bits 1-6, work switch (bit 7) excluded
            if (cRemoteActive && (SectionsChanged || (Diff & 0x7E) != 0)) EndRemote(RemoteEndReasons.Cab);
        }

        private void EndRemote(RemoteEndReasons Reason)
        {
            cRemoteActive = false;
            cRemoteEndReason = Reason;
            RemoteTimer.Stop();

            // back to the physical switch positions of the newest source
            int Src = (LastDataTime[0] >= LastDataTime[1]) ? 0 : 1;
            if (LastData[Src] != null)
            {
                for (int i = 0; i < 16; i++)
                {
                    SW[i + (int)SwIDs.sw0] = Core.Tls.BitRead(LastData[Src][1 + i / 8], i % 8);
                }
                SW[(int)SwIDs.AutoSection] = Core.Tls.BitRead(LastData[Src][0], 5);
                SW[(int)SwIDs.AutoRate] = Core.Tls.BitRead(LastData[Src][0], 6);
            }

            // release any held rate press
            cRemoteRateDir = 0;
            cRateUp = false;
            cRateDown = false;
            SW[3] = false;
            SW[4] = false;
        }

        private void RemoteTimer_Tick(object sender, EventArgs e)
        {
            // dead-man, operator is away from the cab: release and turn master off
            EndRemote(RemoteEndReasons.Timeout);
            cMasterOn = false;
            SW[1] = false;
            SW[2] = true;
            SwitchPGNreceived?.Invoke(this, EventArgs.Empty);
            if (Props.MasterMaintained)
            {
                cRemoteForcedOff = true;    // stays off until the physical master is flipped
            }
            else
            {
                SW[2] = false;
            }
        }

        private void StartRemote()
        {
            if (!cRemoteActive)
            {
                // seed from the current state so takeover changes nothing
                for (int i = 0; i < 16; i++)
                {
                    RemoteSW[i] = SW[i + (int)SwIDs.sw0];
                }
                cRemoteAutoSection = SW[(int)SwIDs.AutoSection];
                cRemoteAutoRate = SW[(int)SwIDs.AutoRate];
                cRemoteRateDir = 0;
                cRemoteMaster = cMasterOn;
                cRemoteActive = true;
                cRemoteEndReason = RemoteEndReasons.None;
                cRemoteForcedOff = false;
            }
            RemoteLastPress = DateTime.Now;
            RemoteTimer.Stop();
            RemoteTimer.Start();
        }
    }
}